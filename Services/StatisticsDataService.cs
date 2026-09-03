using System.IO;
using System.Text.Json;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

public sealed class StatisticsDataService
{
    private const int SupportedSchemaVersion = 1;
    private const int RankedSoloQueueId = 420;
    private static readonly HashSet<string> SupportedRoles =
        new(PositionNormalizer.StandardPositions, StringComparer.Ordinal);

    private readonly string _dataFilePath;
    private readonly Lazy<Task<StatisticsDatasetLoadResult>> _loadResult;

    public StatisticsDataService(string? dataFilePath = null)
    {
        _dataFilePath = dataFilePath ?? Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Statistics",
            "master_plus.json");
        _loadResult = new Lazy<Task<StatisticsDatasetLoadResult>>(LoadCoreAsync);
    }

    public Task<StatisticsDatasetLoadResult> LoadAsync() => _loadResult.Value;

    public async Task<StatisticsMatchupLookupResult?> FindMatchupAsync(
        string role,
        string firstChampionId,
        string secondChampionId)
    {
        if (!TryCreateKey(
                role,
                firstChampionId,
                secondChampionId,
                out var key,
                out var wasReversed))
        {
            return null;
        }

        var loadResult = await LoadAsync();

        return loadResult.Status == StatisticsDatasetLoadStatus.Loaded
            && loadResult.MatchupIndex.TryGetValue(key, out var matchup)
                ? new StatisticsMatchupLookupResult(matchup, wasReversed)
                : null;
    }

    private async Task<StatisticsDatasetLoadResult> LoadCoreAsync()
    {
        if (!File.Exists(_dataFilePath))
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }

        try
        {
            await using var stream = File.OpenRead(_dataFilePath);
            var dataset = await JsonSerializer.DeserializeAsync<StatisticsDataset>(stream);

            if (dataset is null || !ValidateMetadata(dataset))
            {
                return StatisticsDatasetLoadResult.Invalid;
            }

            var index = new Dictionary<StatisticsMatchupKey, StatisticsMatchupEntry>(
                StatisticsMatchupKeyComparer.Instance);

            foreach (var matchup in dataset.Matchups!)
            {
                if (!ValidateMatchup(matchup)
                    || !TryCreateKey(
                        matchup.Role,
                        matchup.ChampionA,
                        matchup.ChampionB,
                        out var key,
                        out var wasReversed)
                    || wasReversed
                    || !index.TryAdd(key, matchup))
                {
                    return StatisticsDatasetLoadResult.Invalid;
                }
            }

            if (dataset.Observations != (long)dataset.AcceptedMatches * 5
                || index.Values.Sum(matchup => (long)matchup.SampleSize)
                    != dataset.Observations)
            {
                return StatisticsDatasetLoadResult.Invalid;
            }

            return new StatisticsDatasetLoadResult(
                StatisticsDatasetLoadStatus.Loaded,
                dataset,
                index);
        }
        catch (IOException)
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }
        catch (UnauthorizedAccessException)
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }
        catch (JsonException)
        {
            return StatisticsDatasetLoadResult.Invalid;
        }
    }

    private static bool ValidateMetadata(StatisticsDataset dataset) =>
        dataset.SchemaVersion == SupportedSchemaVersion
        && !string.IsNullOrWhiteSpace(dataset.Source)
        && !string.IsNullOrWhiteSpace(dataset.TierScope)
        && !string.IsNullOrWhiteSpace(dataset.TierScopeMethod)
        && !string.IsNullOrWhiteSpace(dataset.Region)
        && dataset.QueueId == RankedSoloQueueId
        && !string.IsNullOrWhiteSpace(dataset.Patch)
        && dataset.AcceptedMatches >= 0
        && dataset.Observations >= 0
        && dataset.Matchups is not null;

    private static bool ValidateMatchup(StatisticsMatchupEntry matchup)
    {
        if (string.IsNullOrWhiteSpace(matchup.Role)
            || string.IsNullOrWhiteSpace(matchup.ChampionA)
            || string.IsNullOrWhiteSpace(matchup.ChampionB)
            || !SupportedRoles.Contains(matchup.Role.Trim().ToUpperInvariant()))
        {
            return false;
        }

        var counts = new[]
        {
            matchup.SampleSize,
            matchup.WinsForA,
            matchup.FirstLevel2ForA,
            matchup.FirstLevel2ForB,
            matchup.FirstLevel2Unknown,
            matchup.FirstLevel3ForA,
            matchup.FirstLevel3ForB,
            matchup.FirstLevel3Unknown,
            matchup.FirstLevel6ForA,
            matchup.FirstLevel6ForB,
            matchup.FirstLevel6Unknown,
            matchup.SoloKillsBefore15ForA,
            matchup.SoloKillsBefore15ForB
        };

        return counts.All(count => count >= 0)
            && matchup.WinsForA <= matchup.SampleSize
            && (long)matchup.FirstLevel2ForA
                + matchup.FirstLevel2ForB
                + matchup.FirstLevel2Unknown == matchup.SampleSize
            && (long)matchup.FirstLevel3ForA
                + matchup.FirstLevel3ForB
                + matchup.FirstLevel3Unknown == matchup.SampleSize
            && (long)matchup.FirstLevel6ForA
                + matchup.FirstLevel6ForB
                + matchup.FirstLevel6Unknown == matchup.SampleSize;
    }

    private static bool TryCreateKey(
        string? role,
        string? firstChampionId,
        string? secondChampionId,
        out StatisticsMatchupKey key,
        out bool wasReversed)
    {
        key = default;
        wasReversed = false;

        if (string.IsNullOrWhiteSpace(role)
            || string.IsNullOrWhiteSpace(firstChampionId)
            || string.IsNullOrWhiteSpace(secondChampionId))
        {
            return false;
        }

        var normalizedRole = role.Trim().ToUpperInvariant();

        if (!SupportedRoles.Contains(normalizedRole))
        {
            return false;
        }

        var championA = firstChampionId.Trim();
        var championB = secondChampionId.Trim();
        wasReversed = StringComparer.OrdinalIgnoreCase.Compare(
            championA,
            championB) > 0;

        if (wasReversed)
        {
            (championA, championB) = (championB, championA);
        }

        key = new StatisticsMatchupKey(normalizedRole, championA, championB);
        return true;
    }
}

public enum StatisticsDatasetLoadStatus
{
    Loaded,
    NotAvailable,
    Invalid
}

public sealed record StatisticsDatasetLoadResult(
    StatisticsDatasetLoadStatus Status,
    StatisticsDataset? Dataset = null,
    IReadOnlyDictionary<StatisticsMatchupKey, StatisticsMatchupEntry>? Index = null)
{
    public static StatisticsDatasetLoadResult NotAvailable { get; } =
        new(StatisticsDatasetLoadStatus.NotAvailable);

    public static StatisticsDatasetLoadResult Invalid { get; } =
        new(StatisticsDatasetLoadStatus.Invalid);

    public IReadOnlyDictionary<StatisticsMatchupKey, StatisticsMatchupEntry>
        MatchupIndex => Index
            ?? new Dictionary<StatisticsMatchupKey, StatisticsMatchupEntry>();
}

public readonly record struct StatisticsMatchupKey(
    string Role,
    string ChampionA,
    string ChampionB);

public sealed record StatisticsMatchupLookupResult(
    StatisticsMatchupEntry Matchup,
    bool WasReversed);

internal sealed class StatisticsMatchupKeyComparer
    : IEqualityComparer<StatisticsMatchupKey>
{
    public static StatisticsMatchupKeyComparer Instance { get; } = new();

    public bool Equals(StatisticsMatchupKey x, StatisticsMatchupKey y) =>
        StringComparer.OrdinalIgnoreCase.Equals(x.Role, y.Role)
        && StringComparer.OrdinalIgnoreCase.Equals(x.ChampionA, y.ChampionA)
        && StringComparer.OrdinalIgnoreCase.Equals(x.ChampionB, y.ChampionB);

    public int GetHashCode(StatisticsMatchupKey key)
    {
        var hash = new HashCode();
        hash.Add(key.Role, StringComparer.OrdinalIgnoreCase);
        hash.Add(key.ChampionA, StringComparer.OrdinalIgnoreCase);
        hash.Add(key.ChampionB, StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }
}
