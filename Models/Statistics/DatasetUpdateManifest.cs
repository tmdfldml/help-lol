using System.Text.Json.Serialization;

namespace LoLMatchup.Models.Statistics;

public sealed class DatasetUpdateManifest
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; init; }

    [JsonPropertyName("datasetSchemaVersion")]
    public int DatasetSchemaVersion { get; init; }

    [JsonPropertyName("datasetId")]
    public string DatasetId { get; init; } = string.Empty;

    [JsonPropertyName("datasetRevision")]
    public long DatasetRevision { get; init; }

    [JsonPropertyName("patch")]
    public string Patch { get; init; } = string.Empty;

    [JsonPropertyName("tierScope")]
    public string TierScope { get; init; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; init; } = string.Empty;

    [JsonPropertyName("queueId")]
    public int QueueId { get; init; }

    [JsonPropertyName("minimumGameDurationSeconds")]
    public int MinimumGameDurationSeconds { get; init; }

    [JsonPropertyName("acceptedMatches")]
    public int AcceptedMatches { get; init; }

    [JsonPropertyName("observations")]
    public int Observations { get; init; }

    [JsonPropertyName("matchupKeys")]
    public int MatchupKeys { get; init; }

    [JsonPropertyName("datasetGeneratedAtUtc")]
    public DateTimeOffset DatasetGeneratedAtUtc { get; init; }

    [JsonPropertyName("publishedAtUtc")]
    public DateTimeOffset PublishedAtUtc { get; init; }

    [JsonPropertyName("fileSizeBytes")]
    public long FileSizeBytes { get; init; }

    [JsonPropertyName("sha256")]
    public string Sha256 { get; init; } = string.Empty;

    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; init; }
}
