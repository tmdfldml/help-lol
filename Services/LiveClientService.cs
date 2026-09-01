using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoLMatchup.Services;

public sealed class LiveClientService
{
    private static readonly Uri BaseAddress =
        new("https://127.0.0.1:2999/liveclientdata/");

    private static readonly Uri PlayerListEndpoint =
        new(BaseAddress, "playerlist");

    public async Task<int?> GetPlayerCountAsync()
    {
        var players = await GetPlayersAsync();

        return players?.Count;
    }

    public async Task<IReadOnlyList<LiveClientPlayer>?> GetPlayersAsync()
    {
        var playerListJson = await GetPlayerListAsync();

        if (playerListJson is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<LiveClientPlayer>>(playerListJson);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task<string?> GetPlayerListAsync()
    {
        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (request, _, _, _) =>
                    request.RequestUri?.Host == PlayerListEndpoint.Host
                    && request.RequestUri.Port == PlayerListEndpoint.Port
        };
        using var httpClient = new HttpClient(handler);

        try
        {
            using var response = await httpClient.GetAsync(PlayerListEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }
}

public sealed class LiveClientPlayer
{
    [JsonPropertyName("championName")]
    public string ChampionName { get; init; } = string.Empty;

    [JsonPropertyName("position")]
    public string Position { get; init; } = string.Empty;

    [JsonPropertyName("team")]
    public string Team { get; init; } = string.Empty;
}
