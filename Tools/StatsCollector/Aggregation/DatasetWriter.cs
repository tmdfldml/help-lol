using System.Text.Json;
using StatsCollector.Analysis;
using StatsCollector.Configuration;

namespace StatsCollector.Aggregation;

public sealed class DatasetWriter
{
    private const int SchemaVersion = 1;
    private readonly CollectorOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public DatasetWriter(CollectorOptions options)
    {
        _options = options;
    }

    public async Task<DatasetWriteResult> WriteAsync(
        MatchupAccumulator accumulator,
        int acceptedMatches,
        int observationCount,
        CancellationToken cancellationToken = default)
    {
        if (!accumulator.InvariantsValid)
        {
            throw new InvalidOperationException(
                "Cannot write a dataset with invalid aggregate invariants.");
        }

        if (observationCount != acceptedMatches * RoleNormalizer.StandardRoles.Count)
        {
            throw new InvalidOperationException(
                "Cannot write a dataset with an invalid observation count.");
        }

        var roleOrder = RoleNormalizer.StandardRoles
            .Select((role, index) => (role, index))
            .ToDictionary(item => item.role, item => item.index);
        var matchups = accumulator.Aggregates.Values
            .OrderBy(aggregate => roleOrder[aggregate.Key.Role])
            .ThenBy(
                aggregate => aggregate.Key.ChampionA,
                StringComparer.OrdinalIgnoreCase)
            .ThenBy(
                aggregate => aggregate.Key.ChampionA,
                StringComparer.Ordinal)
            .ThenBy(
                aggregate => aggregate.Key.ChampionB,
                StringComparer.OrdinalIgnoreCase)
            .ThenBy(
                aggregate => aggregate.Key.ChampionB,
                StringComparer.Ordinal)
            .Select(ToEntry)
            .ToList();
        var dataset = new MatchupDataset(
            SchemaVersion,
            "SELF_RIOT",
            _options.TierScope.ToDirectoryName(),
            "SEED_PLAYER_TIER_AT_COLLECTION",
            false,
            _options.Region.Trim().ToUpperInvariant(),
            _options.QueueId,
            _options.TargetPatch,
            DateTimeOffset.UtcNow,
            acceptedMatches,
            observationCount,
            matchups);
        var outputDirectory = Path.Combine(
            _options.OutputRootPath,
            _options.TierScope.ToDirectoryName(),
            _options.Region.Trim().ToUpperInvariant(),
            _options.QueueId.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            _options.TargetPatch);
        Directory.CreateDirectory(outputDirectory);
        var outputPath = Path.Combine(outputDirectory, "matchups.json");
        var temporaryPath = outputPath + ".tmp";

        await using (var stream = new FileStream(
                         temporaryPath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None))
        {
            await JsonSerializer.SerializeAsync(
                stream,
                dataset,
                _jsonOptions,
                cancellationToken);
            await stream.FlushAsync(cancellationToken);
            stream.Flush(true);
        }

        File.Move(temporaryPath, outputPath, true);
        var relativePath = Path.GetRelativePath(
            Path.GetFullPath(Path.Combine(_options.OutputRootPath, "..", "..")),
            outputPath);

        return new DatasetWriteResult(
            outputPath,
            relativePath,
            acceptedMatches,
            observationCount,
            matchups.Count,
            new FileInfo(outputPath).Length);
    }

    private static MatchupDatasetEntry ToEntry(MatchupAggregate aggregate) => new(
        RoleNormalizer.ToDisplayName(aggregate.Key.Role),
        aggregate.Key.ChampionA,
        aggregate.Key.ChampionB,
        aggregate.SampleSize,
        aggregate.WinsForA,
        aggregate.FirstLevel2ForA,
        aggregate.FirstLevel2ForB,
        aggregate.FirstLevel2Unknown,
        aggregate.FirstLevel3ForA,
        aggregate.FirstLevel3ForB,
        aggregate.FirstLevel3Unknown,
        aggregate.FirstLevel6ForA,
        aggregate.FirstLevel6ForB,
        aggregate.FirstLevel6Unknown,
        aggregate.SoloKillsBefore15ForA,
        aggregate.SoloKillsBefore15ForB);
}

public sealed record DatasetWriteResult(
    string OutputPath,
    string RelativePath,
    int AcceptedMatches,
    int Observations,
    int MatchupKeys,
    long FileSizeBytes);

internal sealed record MatchupDataset(
    int SchemaVersion,
    string Source,
    string TierScope,
    string TierScopeMethod,
    bool AllParticipantsTierVerified,
    string Region,
    int QueueId,
    string Patch,
    DateTimeOffset GeneratedAtUtc,
    int AcceptedMatches,
    int Observations,
    IReadOnlyList<MatchupDatasetEntry> Matchups);

internal sealed record MatchupDatasetEntry(
    string Role,
    string ChampionA,
    string ChampionB,
    int SampleSize,
    int WinsForA,
    int FirstLevel2ForA,
    int FirstLevel2ForB,
    int FirstLevel2Unknown,
    int FirstLevel3ForA,
    int FirstLevel3ForB,
    int FirstLevel3Unknown,
    int FirstLevel6ForA,
    int FirstLevel6ForB,
    int FirstLevel6Unknown,
    int SoloKillsBefore15ForA,
    int SoloKillsBefore15ForB);
