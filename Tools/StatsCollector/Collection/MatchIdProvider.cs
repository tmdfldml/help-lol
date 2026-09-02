using StatsCollector.Api;
using StatsCollector.Configuration;

namespace StatsCollector.Collection;

public sealed class MatchIdProvider
{
    private readonly RiotApiClient _riotApiClient;
    private readonly CollectorOptions _options;

    public MatchIdProvider(
        RiotApiClient riotApiClient,
        CollectorOptions options)
    {
        _riotApiClient = riotApiClient;
        _options = options;

        if (options.MatchIdsPageSize is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "MatchIdsPageSize must be between 1 and 100.");
        }

        if (options.MatchIdSeedLimit < 1
            || options.MaxMatchPagesPerSeed < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "Discovery seed and page limits must be positive.");
        }
    }

    public async Task<MatchIdCollectionResult> CollectMatchIdsAsync(
        IEnumerable<string> seedPuuids,
        CancellationToken cancellationToken = default)
    {
        var uniqueMatchIds = new HashSet<string>(StringComparer.Ordinal);
        var failures = new List<MatchIdRequestFailure>();
        var expectedPrefix = $"{_options.Region.Trim().ToUpperInvariant()}_";
        var seedsUsed = 0;
        var returnedCount = 0;
        var duplicateCount = 0;
        var invalidCount = 0;
        var pagesRequested = 0;
        var stopCollection = false;

        foreach (var puuid in seedPuuids.Take(_options.MatchIdSeedLimit))
        {
            seedsUsed++;

            for (var page = 0; page < _options.MaxMatchPagesPerSeed; page++)
            {
                pagesRequested++;
                var response = await _riotApiClient.GetMatchIdsAsync(
                    puuid,
                    _options.QueueId,
                    page * _options.MatchIdsPageSize,
                    _options.MatchIdsPageSize,
                    cancellationToken);

                if (response.Status != RiotApiRequestStatus.Success
                    || response.Data is null)
                {
                    failures.Add(new MatchIdRequestFailure(
                        seedsUsed,
                        page + 1,
                        response.Status,
                        response.RetryAfter));

                    if (response.Status is RiotApiRequestStatus.AuthenticationFailed
                        or RiotApiRequestStatus.RateLimited
                        or RiotApiRequestStatus.Cancelled)
                    {
                        stopCollection = true;
                    }

                    break;
                }

                returnedCount += response.Data.Count;

                foreach (var matchId in response.Data)
                {
                    if (string.IsNullOrWhiteSpace(matchId)
                        || !matchId.StartsWith(
                            expectedPrefix,
                            StringComparison.Ordinal))
                    {
                        invalidCount++;
                        continue;
                    }

                    if (!uniqueMatchIds.Add(matchId))
                    {
                        duplicateCount++;
                    }
                }

                if (response.Data.Count < _options.MatchIdsPageSize)
                {
                    break;
                }
            }

            if (stopCollection)
            {
                break;
            }
        }

        return new MatchIdCollectionResult(
            uniqueMatchIds,
            failures,
            seedsUsed,
            returnedCount,
            duplicateCount,
            invalidCount,
            pagesRequested);
    }
}

public sealed record MatchIdCollectionResult(
    IReadOnlySet<string> MatchIds,
    IReadOnlyList<MatchIdRequestFailure> Failures,
    int SeedsUsed,
    int ReturnedCount,
    int DuplicateCount,
    int InvalidCount,
    int PagesRequested);

public sealed record MatchIdRequestFailure(
    int SeedNumber,
    int PageNumber,
    RiotApiRequestStatus Status,
    TimeSpan? RetryAfter);
