using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoLMatchup.Services;

public sealed class LiveClientService : IDisposable
{
    private static readonly Uri BaseAddress =
        new("https://127.0.0.1:2999/liveclientdata/");

    private static readonly Uri PlayerListEndpoint =
        new(BaseAddress, "playerlist");

    private static readonly Uri ActivePlayerNameEndpoint =
        new(BaseAddress, "activeplayername");

    private readonly HttpClientHandler _handler;
    private readonly HttpClient _httpClient;

    public LiveClientService()
    {
        _handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (request, _, _, _) =>
                    request.RequestUri?.Scheme == Uri.UriSchemeHttps
                    && request.RequestUri.Host == BaseAddress.Host
                    && request.RequestUri.Port == BaseAddress.Port
        };
        _httpClient = new HttpClient(_handler);
    }

    public async Task<int?> GetPlayerCountAsync()
    {
        var players = await GetPlayersAsync();

        return players?.Count;
    }

    public async Task<IReadOnlyList<LiveClientPlayer>?> GetPlayersAsync(
        CancellationToken cancellationToken = default)
    {
        var playerListJson = await GetPlayerListAsync(cancellationToken);

        if (playerListJson is null)
        {
            return null;
        }

        try
        {
            var players = JsonSerializer.Deserialize<List<LiveClientPlayer?>>(playerListJson);

            return players?.OfType<LiveClientPlayer>().ToList();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task<LiveClientPlayer?> GetActivePlayerAsync(
        IReadOnlyList<LiveClientPlayer> players,
        CancellationToken cancellationToken = default)
    {
        var riotId = await GetActivePlayerRiotIdAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(riotId))
        {
            return null;
        }

        return players.FirstOrDefault(player =>
            string.Equals(player.RiotId, riotId, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<string?> GetActivePlayerRiotIdAsync(
        CancellationToken cancellationToken = default)
    {
        var responseBody = await GetResponseBodyAsync(
            ActivePlayerNameEndpoint,
            cancellationToken);

        if (responseBody is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<string>(responseBody);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public Task<string?> GetPlayerListAsync(
        CancellationToken cancellationToken = default)
    {
        return GetResponseBodyAsync(PlayerListEndpoint, cancellationToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _handler.Dispose();
    }

    private async Task<string?> GetResponseBodyAsync(
        Uri endpoint,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(
                endpoint,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }
}

public sealed class LiveClientPlayer
{
    [JsonPropertyName("riotId")]
    public string RiotId { get; init; } = string.Empty;

    [JsonPropertyName("championName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("rawChampionName")]
    public string RawChampionName { get; init; } = string.Empty;

    [JsonIgnore]
    public string? ChampionId => ChampionIdParser.Parse(RawChampionName);

    [JsonPropertyName("position")]
    public string Position { get; init; } = string.Empty;

    public string NormalizedPosition => PositionNormalizer.Normalize(Position);

    [JsonPropertyName("team")]
    public string Team { get; init; } = string.Empty;
}
