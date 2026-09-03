namespace StatsCollector.Configuration;

public sealed class CollectorOptions
{
    public const int DefaultMinimumGameDurationSeconds = 900;

    public const string TargetPatchEnvironmentVariableName = "TARGET_PATCH";

    public const string TargetAcceptedMatchesEnvironmentVariableName =
        "TARGET_ACCEPTED_MATCHES";

    public const string AutoContinueCollectionEnvironmentVariableName =
        "AUTO_CONTINUE_COLLECTION";

    public const string MinimumGameDurationSecondsEnvironmentVariableName =
        "MIN_GAME_DURATION_SECONDS";

    public string Region { get; init; } = "KR";

    public string RegionalRouting { get; init; } = "ASIA";

    public int QueueId { get; init; } = 420;

    public required string TargetPatch { get; init; }

    public TierScope TierScope { get; init; }

    public int TargetMatchCount { get; init; }

    public int DiscoverySeedBatchSize { get; init; } = 125;

    public int MatchIdsPageSize { get; init; } = 20;

    public int MatchIdPagesPerSeed { get; init; } = 1;

    public int ProcessingMatchLimit { get; init; } = 100;

    public required int TargetAcceptedMatchCount { get; init; }

    public bool AutoContinueCollection { get; init; }

    public int MinimumGameDurationSeconds { get; init; } =
        DefaultMinimumGameDurationSeconds;

    public string DurationPolicyKey =>
        $"MIN{MinimumGameDurationSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)}S";

    public int MaxTransientRetries { get; init; } = 3;

    public TimeSpan InitialRetryDelay { get; init; } = TimeSpan.FromSeconds(1);

    public string StateRootPath { get; init; } = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "State"));

    public string OutputRootPath { get; init; } = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "Output"));
}

public enum TierScope
{
    MasterPlusSeeded,
    EmeraldPlusSeeded
}

public static class TierScopeNames
{
    public static string ToDirectoryName(this TierScope tierScope) => tierScope switch
    {
        TierScope.MasterPlusSeeded => "MASTER_PLUS_SEEDED",
        TierScope.EmeraldPlusSeeded => "EMERALD_PLUS_SEEDED",
        _ => throw new ArgumentOutOfRangeException(nameof(tierScope))
    };
}
