namespace StatsCollector.Configuration;

public sealed class CollectorOptions
{
    public const string TargetPatchEnvironmentVariableName = "TARGET_PATCH";

    public const string TargetAcceptedMatchesEnvironmentVariableName =
        "TARGET_ACCEPTED_MATCHES";

    public string Region { get; init; } = "KR";

    public string RegionalRouting { get; init; } = "ASIA";

    public int QueueId { get; init; } = 420;

    public required string TargetPatch { get; init; }

    public TierScope TierScope { get; init; }

    public int TargetMatchCount { get; init; }

    public int MatchIdSeedLimit { get; init; } = 25;

    public int MatchIdsPageSize { get; init; } = 20;

    public int MaxMatchPagesPerSeed { get; init; } = 5;

    public int ProcessingMatchLimit { get; init; } = 100;

    public required int TargetAcceptedMatchCount { get; init; }

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
