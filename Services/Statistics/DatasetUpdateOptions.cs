namespace LoLMatchup.Services.Statistics;

public sealed record DatasetUpdateOptions
{
    public const string DatasetId = "master-plus-kr-420";
    public const string TierScope = "MASTER_PLUS_SEEDED";
    public const string Region = "KR";
    public const int QueueId = 420;

    // Configure only when an HTTPS-hosted production manifest is available.
    public Uri? ManifestUri { get; init; }
}
