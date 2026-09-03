using System.Text.Json.Serialization;

namespace LoLMatchup.Models.Statistics;

public sealed class StatisticsDataset
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; init; }

    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    [JsonPropertyName("tierScope")]
    public string TierScope { get; init; } = string.Empty;

    [JsonPropertyName("tierScopeMethod")]
    public string TierScopeMethod { get; init; } = string.Empty;

    [JsonPropertyName("allParticipantsTierVerified")]
    public bool AllParticipantsTierVerified { get; init; }

    [JsonPropertyName("region")]
    public string Region { get; init; } = string.Empty;

    [JsonPropertyName("queueId")]
    public int QueueId { get; init; }

    [JsonPropertyName("patch")]
    public string Patch { get; init; } = string.Empty;

    [JsonPropertyName("minimumGameDurationSeconds")]
    public int? MinimumGameDurationSeconds { get; init; }

    [JsonPropertyName("durationFilterPolicy")]
    public string? DurationFilterPolicy { get; init; }

    [JsonPropertyName("policyKey")]
    public string? PolicyKey { get; init; }

    [JsonPropertyName("generatedAtUtc")]
    public DateTimeOffset GeneratedAtUtc { get; init; }

    [JsonPropertyName("acceptedMatches")]
    public int AcceptedMatches { get; init; }

    [JsonPropertyName("observations")]
    public int Observations { get; init; }

    [JsonPropertyName("matchups")]
    public List<StatisticsMatchupEntry>? Matchups { get; init; }
}

public sealed class StatisticsMatchupEntry
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("championA")]
    public string ChampionA { get; init; } = string.Empty;

    [JsonPropertyName("championB")]
    public string ChampionB { get; init; } = string.Empty;

    [JsonPropertyName("sampleSize")]
    public int SampleSize { get; init; }

    [JsonPropertyName("winsForA")]
    public int WinsForA { get; init; }

    [JsonPropertyName("firstLevel2ForA")]
    public int FirstLevel2ForA { get; init; }

    [JsonPropertyName("firstLevel2ForB")]
    public int FirstLevel2ForB { get; init; }

    [JsonPropertyName("firstLevel2Unknown")]
    public int FirstLevel2Unknown { get; init; }

    [JsonPropertyName("firstLevel3ForA")]
    public int FirstLevel3ForA { get; init; }

    [JsonPropertyName("firstLevel3ForB")]
    public int FirstLevel3ForB { get; init; }

    [JsonPropertyName("firstLevel3Unknown")]
    public int FirstLevel3Unknown { get; init; }

    [JsonPropertyName("firstLevel6ForA")]
    public int FirstLevel6ForA { get; init; }

    [JsonPropertyName("firstLevel6ForB")]
    public int FirstLevel6ForB { get; init; }

    [JsonPropertyName("firstLevel6Unknown")]
    public int FirstLevel6Unknown { get; init; }

    [JsonPropertyName("soloKillsBefore15ForA")]
    public int SoloKillsBefore15ForA { get; init; }

    [JsonPropertyName("soloKillsBefore15ForB")]
    public int SoloKillsBefore15ForB { get; init; }
}
