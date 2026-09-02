using System.Net;
using System.Diagnostics;
using System.Text.Json;
using StatsCollector.Api.Dtos;
using StatsCollector.Configuration;

namespace StatsCollector.Api;

public sealed class RiotApiClient : IDisposable
{
    private readonly HttpClient _platformHttpClient;
    private readonly HttpClient _regionalHttpClient;
    private readonly string _apiKey;
    private readonly int _maxTransientRetries;
    private readonly TimeSpan _initialRetryDelay;

    public RiotApiClient(CollectorOptions options, string apiKey)
    {
        _apiKey = apiKey;
        _maxTransientRetries = options.MaxTransientRetries >= 0
            ? options.MaxTransientRetries
            : throw new ArgumentOutOfRangeException(
                nameof(options),
                "MaxTransientRetries cannot be negative.");
        _initialRetryDelay = options.InitialRetryDelay >= TimeSpan.Zero
            ? options.InitialRetryDelay
            : throw new ArgumentOutOfRangeException(
                nameof(options),
                "InitialRetryDelay cannot be negative.");
        _platformHttpClient = new HttpClient
        {
            BaseAddress = new Uri(
                $"https://{options.Region.Trim().ToLowerInvariant()}.api.riotgames.com/")
        };
        _regionalHttpClient = new HttpClient
        {
            BaseAddress = new Uri(
                $"https://{options.RegionalRouting.Trim().ToLowerInvariant()}.api.riotgames.com/")
        };
    }

    public RiotApiRetryDiagnostics RetryDiagnostics { get; } = new();

    public Task<RiotApiResult<LeagueListDto>> GetLeagueAsync(
        LeagueTier tier,
        CancellationToken cancellationToken = default)
    {
        var tierPath = tier switch
        {
            LeagueTier.Challenger => "challengerleagues",
            LeagueTier.Grandmaster => "grandmasterleagues",
            LeagueTier.Master => "masterleagues",
            _ => throw new ArgumentOutOfRangeException(nameof(tier))
        };

        return SendAsync<LeagueListDto>(
            _platformHttpClient,
            $"lol/league/v4/{tierPath}/by-queue/RANKED_SOLO_5x5",
            RiotApiRequestCategory.League,
            cancellationToken);
    }

    public Task<RiotApiResult<SummonerDto>> GetSummonerByIdAsync(
        string encryptedSummonerId,
        CancellationToken cancellationToken = default)
    {
        var encodedId = Uri.EscapeDataString(encryptedSummonerId);
        return SendAsync<SummonerDto>(
            _platformHttpClient,
            $"lol/summoner/v4/summoners/{encodedId}",
            RiotApiRequestCategory.Summoner,
            cancellationToken);
    }

    public Task<RiotApiResult<List<string>>> GetMatchIdsAsync(
        string puuid,
        int queueId,
        int start,
        int count,
        CancellationToken cancellationToken = default)
    {
        var encodedPuuid = Uri.EscapeDataString(puuid);
        return SendAsync<List<string>>(
            _regionalHttpClient,
            $"lol/match/v5/matches/by-puuid/{encodedPuuid}/ids"
            + $"?queue={queueId}&start={start}&count={count}",
            RiotApiRequestCategory.MatchIds,
            cancellationToken);
    }

    public Task<RiotApiResult<MatchDto>> GetMatchAsync(
        string matchId,
        CancellationToken cancellationToken = default)
    {
        var encodedMatchId = Uri.EscapeDataString(matchId);
        return SendAsync<MatchDto>(
            _regionalHttpClient,
            $"lol/match/v5/matches/{encodedMatchId}",
            RiotApiRequestCategory.MatchDetail,
            cancellationToken);
    }

    public Task<RiotApiResult<MatchTimelineDto>> GetTimelineAsync(
        string matchId,
        CancellationToken cancellationToken = default)
    {
        var encodedMatchId = Uri.EscapeDataString(matchId);
        return SendAsync<MatchTimelineDto>(
            _regionalHttpClient,
            $"lol/match/v5/matches/{encodedMatchId}/timeline",
            RiotApiRequestCategory.Timeline,
            cancellationToken);
    }

    public void Dispose()
    {
        _platformHttpClient.Dispose();
        _regionalHttpClient.Dispose();
    }

    private async Task<RiotApiResult<T>> SendAsync<T>(
        HttpClient httpClient,
        string relativePath,
        RiotApiRequestCategory category,
        CancellationToken cancellationToken)
    {
        RetryDiagnostics.LogicalApiRequests++;
        RetryDiagnostics.IncrementCategory(category);

        for (var retryNumber = 0; ; retryNumber++)
        {
            RetryDiagnostics.HttpAttempts++;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, relativePath);
                request.Headers.TryAddWithoutValidation("X-Riot-Token", _apiKey);
                using var response = await httpClient.SendAsync(
                    request,
                    cancellationToken);
                RetryDiagnostics.CaptureRateLimitHeaders(response);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        await using var stream = await response.Content
                            .ReadAsStreamAsync(cancellationToken);
                        var data = await JsonSerializer.DeserializeAsync<T>(
                            stream,
                            cancellationToken: cancellationToken);

                        return data is not null
                            ? new RiotApiResult<T>(
                                RiotApiRequestStatus.Success,
                                data,
                                Category: category)
                            : new RiotApiResult<T>(
                                RiotApiRequestStatus.InvalidResponse,
                                Category: category);
                    }
                    catch (JsonException)
                    {
                        return new RiotApiResult<T>(
                            RiotApiRequestStatus.InvalidResponse,
                            Category: category);
                    }
                }

                if (response.StatusCode is HttpStatusCode.Unauthorized
                    or HttpStatusCode.Forbidden)
                {
                    return new RiotApiResult<T>(
                        RiotApiRequestStatus.AuthenticationFailed,
                        Category: category);
                }

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = GetRetryAfter(response);

                    if (retryNumber >= _maxTransientRetries)
                    {
                        RetryDiagnostics.RateLimitExhausted = true;
                        return new RiotApiResult<T>(
                            RiotApiRequestStatus.RateLimited,
                            RetryAfter: retryAfter,
                            Category: category);
                    }

                    var delay = retryAfter ?? GetTransientDelay(retryNumber);
                    RetryDiagnostics.RetriesDueToRateLimit++;
                    if (!await WaitBeforeRetryAsync(delay, cancellationToken))
                    {
                        return new RiotApiResult<T>(
                            RiotApiRequestStatus.Cancelled,
                            Category: category);
                    }

                    continue;
                }

                if ((int)response.StatusCode >= 500)
                {
                    if (retryNumber >= _maxTransientRetries)
                    {
                        return new RiotApiResult<T>(
                            RiotApiRequestStatus.ServerError,
                            Category: category);
                    }

                    RetryDiagnostics.RetriesDueToServerError++;
                    if (!await WaitBeforeRetryAsync(
                            GetTransientDelay(retryNumber),
                            cancellationToken))
                    {
                        return new RiotApiResult<T>(
                            RiotApiRequestStatus.Cancelled,
                            Category: category);
                    }

                    continue;
                }

                return new RiotApiResult<T>(
                    RiotApiRequestStatus.UnexpectedResponse,
                    Category: category);
            }
            catch (HttpRequestException)
            {
                if (retryNumber >= _maxTransientRetries)
                {
                    return new RiotApiResult<T>(
                        RiotApiRequestStatus.NetworkError,
                        Category: category);
                }

                RetryDiagnostics.RetriesDueToNetworkOrTimeout++;
                if (!await WaitBeforeRetryAsync(
                        GetTransientDelay(retryNumber),
                        cancellationToken))
                {
                    return new RiotApiResult<T>(
                        RiotApiRequestStatus.Cancelled,
                        Category: category);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return new RiotApiResult<T>(
                    RiotApiRequestStatus.Cancelled,
                    Category: category);
            }
            catch (TaskCanceledException)
            {
                if (retryNumber >= _maxTransientRetries)
                {
                    return new RiotApiResult<T>(
                        RiotApiRequestStatus.NetworkError,
                        Category: category);
                }

                RetryDiagnostics.RetriesDueToNetworkOrTimeout++;
                if (!await WaitBeforeRetryAsync(
                        GetTransientDelay(retryNumber),
                        cancellationToken))
                {
                    return new RiotApiResult<T>(
                        RiotApiRequestStatus.Cancelled,
                        Category: category);
                }
            }
        }
    }

    private TimeSpan GetTransientDelay(int retryNumber)
    {
        var exponentialMilliseconds = _initialRetryDelay.TotalMilliseconds
            * Math.Pow(2, retryNumber);
        var jitterMilliseconds = Random.Shared.NextDouble() * 250;
        return TimeSpan.FromMilliseconds(
            exponentialMilliseconds + jitterMilliseconds);
    }

    private async Task<bool> WaitBeforeRetryAsync(
        TimeSpan delay,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await Task.Delay(delay, cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        finally
        {
            RetryDiagnostics.TotalRetryWait += stopwatch.Elapsed;
        }
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta is { } delta)
        {
            return delta;
        }

        if (response.Headers.RetryAfter?.Date is { } date)
        {
            var delay = date - DateTimeOffset.UtcNow;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }

        return null;
    }
}

public sealed record RiotApiResult<T>(
    RiotApiRequestStatus Status,
    T? Data = default,
    TimeSpan? RetryAfter = null,
    RiotApiRequestCategory Category = RiotApiRequestCategory.Unknown);

public enum RiotApiRequestStatus
{
    Success,
    AuthenticationFailed,
    RateLimited,
    ServerError,
    NetworkError,
    InvalidResponse,
    UnexpectedResponse,
    Cancelled
}

public enum RiotApiRequestCategory
{
    Unknown,
    League,
    Summoner,
    MatchIds,
    MatchDetail,
    Timeline
}

public sealed class RiotApiRetryDiagnostics
{
    private readonly Dictionary<RiotApiRequestCategory, int> _categoryRequests = [];

    public int LogicalApiRequests { get; internal set; }

    public int HttpAttempts { get; internal set; }

    public int RetriesDueToRateLimit { get; internal set; }

    public int RetriesDueToServerError { get; internal set; }

    public int RetriesDueToNetworkOrTimeout { get; internal set; }

    public TimeSpan TotalRetryWait { get; internal set; }

    public bool RateLimitExhausted { get; internal set; }

    public string? ApplicationRateLimit { get; private set; }

    public string? ApplicationRateLimitCount { get; private set; }

    public string? MethodRateLimit { get; private set; }

    public string? MethodRateLimitCount { get; private set; }

    public int GetCategoryRequestCount(RiotApiRequestCategory category) =>
        _categoryRequests.GetValueOrDefault(category);

    internal void IncrementCategory(RiotApiRequestCategory category)
    {
        _categoryRequests[category] = GetCategoryRequestCount(category) + 1;
    }

    internal void CaptureRateLimitHeaders(HttpResponseMessage response)
    {
        ApplicationRateLimit = GetHeader(response, "X-App-Rate-Limit")
            ?? ApplicationRateLimit;
        ApplicationRateLimitCount = GetHeader(response, "X-App-Rate-Limit-Count")
            ?? ApplicationRateLimitCount;
        MethodRateLimit = GetHeader(response, "X-Method-Rate-Limit")
            ?? MethodRateLimit;
        MethodRateLimitCount = GetHeader(response, "X-Method-Rate-Limit-Count")
            ?? MethodRateLimitCount;
    }

    private static string? GetHeader(
        HttpResponseMessage response,
        string headerName)
    {
        return response.Headers.TryGetValues(headerName, out var values)
            ? string.Join(",", values)
            : null;
    }
}

public enum LeagueTier
{
    Challenger,
    Grandmaster,
    Master
}
