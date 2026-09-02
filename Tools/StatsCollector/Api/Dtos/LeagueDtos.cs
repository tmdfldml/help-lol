using System.Text.Json.Serialization;

namespace StatsCollector.Api.Dtos;

public sealed class LeagueListDto
{
    [JsonPropertyName("queue")]
    public string Queue { get; init; } = string.Empty;

    [JsonPropertyName("tier")]
    public string Tier { get; init; } = string.Empty;

    [JsonPropertyName("entries")]
    public List<LeagueEntryDto> Entries { get; init; } = [];
}

public sealed class LeagueEntryDto
{
    [JsonPropertyName("puuid")]
    public string? Puuid { get; init; }

    [JsonPropertyName("summonerId")]
    public string? SummonerId { get; init; }
}

public sealed class SummonerDto
{
    [JsonPropertyName("puuid")]
    public string Puuid { get; init; } = string.Empty;
}
