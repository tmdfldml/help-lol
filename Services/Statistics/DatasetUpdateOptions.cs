using System.Diagnostics;

namespace LoLMatchup.Services.Statistics;

public sealed record DatasetUpdateOptions
{
    public const string ManifestUrlEnvironmentVariableName =
        "LOLMATCHINFO_STATS_MANIFEST_URL";
    public const string DatasetId = "master-plus-kr-420";
    public const string TierScope = "MASTER_PLUS_SEEDED";
    public const string Region = "KR";
    public const int QueueId = 420;
    public static Uri ProductionManifestUri { get; } = new(
        "https://lol-match-info-stats.01024783207a.workers.dev/"
        + "statistics/master-plus/manifest.json");

    public Uri? ManifestUri { get; init; }

    public static DatasetUpdateOptions FromEnvironment()
    {
        var value = Environment.GetEnvironmentVariable(
            ManifestUrlEnvironmentVariableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            return new DatasetUpdateOptions { ManifestUri = ProductionManifestUri };
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var overrideUri))
        {
            Debug.WriteLine(
                $"Invalid {ManifestUrlEnvironmentVariableName} override; "
                + "the statistics update endpoint is disabled for this process.");
            return new DatasetUpdateOptions();
        }

        return new DatasetUpdateOptions
        {
            ManifestUri = overrideUri
        };
    }
}
