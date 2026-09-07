using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class StatisticsDatasetValidator
{
    public const int SupportedSchemaVersion = 1;
    public const int RankedSoloQueueId = 420;
    private static readonly HashSet<string> SupportedRoles =
        new(["TOP", "JUNGLE", "MID", "ADC", "SUPPORT"], StringComparer.Ordinal);

    public static bool TryValidate(StatisticsDataset? dataset,
        out IReadOnlyDictionary<StatisticsMatchupKey, StatisticsMatchupEntry> index,
        out string error)
    {
        var entries = new Dictionary<StatisticsMatchupKey, StatisticsMatchupEntry>(StatisticsMatchupKeyComparer.Instance);
        index = entries;
        if (dataset is null)
        {
            error = "Dataset is empty.";
            return false;
        }

        if (dataset.SchemaVersion != SupportedSchemaVersion
            || string.IsNullOrWhiteSpace(dataset.Source)
            || string.IsNullOrWhiteSpace(dataset.TierScope)
            || string.IsNullOrWhiteSpace(dataset.TierScopeMethod)
            || string.IsNullOrWhiteSpace(dataset.Region)
            || dataset.QueueId != RankedSoloQueueId
            || string.IsNullOrWhiteSpace(dataset.Patch)
            || dataset.AcceptedMatches <= 0 || dataset.Observations <= 0
            || dataset.MinimumGameDurationSeconds is null or <= 0
            || dataset.Matchups is null || dataset.Matchups.Count == 0)
        {
            error = "Dataset metadata is invalid or incomplete.";
            return false;
        }

        foreach (var matchup in dataset.Matchups)
        {
            if (!ValidateMatchup(matchup)
                || !TryCreateKey(matchup.Role, matchup.ChampionA, matchup.ChampionB,
                    out var key, out var wasReversed)
                || wasReversed || !entries.TryAdd(key, matchup))
            {
                error = "A matchup record is invalid, reversed, or duplicated.";
                return false;
            }
        }

        if (dataset.Observations != (long)dataset.AcceptedMatches * 5
            || entries.Values.Sum(matchup => (long)matchup.SampleSize) != dataset.Observations)
        {
            error = "Dataset observation counts are inconsistent.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool TryCreateKey(string? role, string? firstChampionId, string? secondChampionId,
        out StatisticsMatchupKey key, out bool wasReversed)
    {
        key = default;
        wasReversed = false;
        if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(firstChampionId)
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
        wasReversed = StringComparer.OrdinalIgnoreCase.Compare(championA, championB) > 0;
        if (wasReversed)
        {
            (championA, championB) = (championB, championA);
        }

        key = new StatisticsMatchupKey(normalizedRole, championA, championB);
        return true;
    }

    private static bool ValidateMatchup(StatisticsMatchupEntry matchup)
    {
        if (string.IsNullOrWhiteSpace(matchup.Role) || string.IsNullOrWhiteSpace(matchup.ChampionA)
            || string.IsNullOrWhiteSpace(matchup.ChampionB)
            || !SupportedRoles.Contains(matchup.Role.Trim().ToUpperInvariant()))
        {
            return false;
        }

        var counts = new[] { matchup.SampleSize, matchup.WinsForA,
            matchup.FirstLevel2ForA, matchup.FirstLevel2ForB, matchup.FirstLevel2Unknown,
            matchup.FirstLevel3ForA, matchup.FirstLevel3ForB, matchup.FirstLevel3Unknown,
            matchup.FirstLevel6ForA, matchup.FirstLevel6ForB, matchup.FirstLevel6Unknown,
            matchup.SoloKillsBefore15ForA, matchup.SoloKillsBefore15ForB };

        return counts.All(count => count >= 0)
            && matchup.WinsForA <= matchup.SampleSize
            && (long)matchup.FirstLevel2ForA + matchup.FirstLevel2ForB + matchup.FirstLevel2Unknown == matchup.SampleSize
            && (long)matchup.FirstLevel3ForA + matchup.FirstLevel3ForB + matchup.FirstLevel3Unknown == matchup.SampleSize
            && (long)matchup.FirstLevel6ForA + matchup.FirstLevel6ForB + matchup.FirstLevel6Unknown == matchup.SampleSize;
    }
}

public readonly record struct StatisticsMatchupKey(string Role, string ChampionA, string ChampionB);
public sealed record StatisticsMatchupLookupResult(StatisticsMatchupEntry Matchup, bool WasReversed);

public sealed class StatisticsMatchupKeyComparer : IEqualityComparer<StatisticsMatchupKey>
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
