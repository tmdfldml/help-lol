using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

internal static class ProductionLaneIndicatorsVerification
{
    public static void Verify(IReadOnlyList<StatisticsMatchupEntry> entries)
    {
        var levelCounts = entries.Select(entry => Level(entry)).GroupBy(value => value)
            .ToDictionary(group => group.Key, group => group.Count());
        var combatCounts = entries.Select(entry => Combat(entry)).GroupBy(value => value)
            .ToDictionary(group => group.Key, group => group.Count());
        var reverseSymmetry = entries.All(entry =>
            Reverse(Level(entry)) == Level(entry, reverse: true)
            && Reverse(Combat(entry)) == Combat(entry, reverse: true));
        var researchMatch = entries.Count != 13_021
            || (Count(levelCounts, LevelTempoLevel.AllyTendency) == 780
                && Count(levelCounts, LevelTempoLevel.Neutral) == 240
                && Count(levelCounts, LevelTempoLevel.EnemyTendency) == 765
                && Count(levelCounts, LevelTempoLevel.InsufficientEvidence) == 11_236
                && Count(combatCounts, CombatPressureLevel.AllyTendency) == 531
                && Count(combatCounts, CombatPressureLevel.Neutral) == 781
                && Count(combatCounts, CombatPressureLevel.EnemyTendency) == 473
                && Count(combatCounts, CombatPressureLevel.InsufficientEvidence) == 11_236);
        var representativeCases =
            Expected("TOP", "Kled", "Nasus", LevelTempoLevel.AllyTendency, null)
            && Expected("TOP", "Gnar", "Kled", LevelTempoLevel.EnemyTendency, null)
            && Expected("ADC", "Ezreal", "Jhin", LevelTempoLevel.Neutral,
                CombatPressureLevel.EnemyTendency)
            && Expected("ADC", "Jinx", "Kaisa", LevelTempoLevel.AllyTendency,
                CombatPressureLevel.EnemyTendency)
            && Expected("ADC", "Kaisa", "Kalista", LevelTempoLevel.EnemyTendency,
                CombatPressureLevel.AllyTendency)
            && Expected("TOP", "Jayce", "KSante", LevelTempoLevel.AllyTendency,
                CombatPressureLevel.EnemyTendency);

        if (!researchMatch || !reverseSymmetry || !representativeCases)
        {
            throw new InvalidDataException(
                "Production Level Tempo / Combat Pressure regression failed: "
                + $"researchMatch={researchMatch}, reverseSymmetry={reverseSymmetry}, "
                + $"representativeCases={representativeCases}; "
                + $"level={Count(levelCounts, LevelTempoLevel.AllyTendency)}/"
                + $"{Count(levelCounts, LevelTempoLevel.Neutral)}/"
                + $"{Count(levelCounts, LevelTempoLevel.EnemyTendency)}/"
                + $"{Count(levelCounts, LevelTempoLevel.InsufficientEvidence)}, "
                + $"combat={Count(combatCounts, CombatPressureLevel.AllyTendency)}/"
                + $"{Count(combatCounts, CombatPressureLevel.Neutral)}/"
                + $"{Count(combatCounts, CombatPressureLevel.EnemyTendency)}/"
                + $"{Count(combatCounts, CombatPressureLevel.InsufficientEvidence)}.");
        }

        Console.WriteLine("Production lane indicators verification");
        Console.WriteLine($"Level Tempo: A={Count(levelCounts, LevelTempoLevel.AllyTendency)}, "
            + $"Neutral={Count(levelCounts, LevelTempoLevel.Neutral)}, "
            + $"B={Count(levelCounts, LevelTempoLevel.EnemyTendency)}, "
            + $"Insufficient={Count(levelCounts, LevelTempoLevel.InsufficientEvidence)}");
        Console.WriteLine($"Combat Pressure: A={Count(combatCounts, CombatPressureLevel.AllyTendency)}, "
            + $"Neutral={Count(combatCounts, CombatPressureLevel.Neutral)}, "
            + $"B={Count(combatCounts, CombatPressureLevel.EnemyTendency)}, "
            + $"Insufficient={Count(combatCounts, CombatPressureLevel.InsufficientEvidence)}");
        Console.WriteLine($"levelTempoResearchMatch={researchMatch}");
        Console.WriteLine($"combatPressureResearchMatch={researchMatch}");
        Console.WriteLine($"reverseSymmetry={reverseSymmetry}");
        Console.WriteLine($"representativeCases={representativeCases}");

        bool Expected(
            string role,
            string championA,
            string championB,
            LevelTempoLevel expectedLevel,
            CombatPressureLevel? expectedCombat)
        {
            var entry = entries.FirstOrDefault(value => value.Role == role
                && value.ChampionA == championA && value.ChampionB == championB);
            return entry is not null && Level(entry) == expectedLevel
                && (expectedCombat is null || Combat(entry) == expectedCombat);
        }
    }

    private static LevelTempoLevel Level(StatisticsMatchupEntry entry, bool reverse = false)
    {
        var result = LevelTempoClassifier.Classify(
            entry.SampleSize,
            reverse ? entry.FirstLevel2ForB : entry.FirstLevel2ForA,
            reverse ? entry.FirstLevel2ForA : entry.FirstLevel2ForB,
            entry.FirstLevel2Unknown,
            reverse ? entry.FirstLevel3ForB : entry.FirstLevel3ForA,
            reverse ? entry.FirstLevel3ForA : entry.FirstLevel3ForB,
            entry.FirstLevel3Unknown,
            reverse ? entry.FirstLevel6ForB : entry.FirstLevel6ForA,
            reverse ? entry.FirstLevel6ForA : entry.FirstLevel6ForB,
            entry.FirstLevel6Unknown);
        return result.Level;
    }

    private static CombatPressureLevel Combat(
        StatisticsMatchupEntry entry,
        bool reverse = false) => CombatPressureClassifier.Classify(
            entry.SampleSize,
            reverse ? entry.SoloKillsBefore15ForB : entry.SoloKillsBefore15ForA,
            reverse ? entry.SoloKillsBefore15ForA : entry.SoloKillsBefore15ForB).Level;

    private static LevelTempoLevel Reverse(LevelTempoLevel level) => level switch
    {
        LevelTempoLevel.AllyTendency => LevelTempoLevel.EnemyTendency,
        LevelTempoLevel.EnemyTendency => LevelTempoLevel.AllyTendency,
        _ => level
    };

    private static CombatPressureLevel Reverse(CombatPressureLevel level) => level switch
    {
        CombatPressureLevel.AllyTendency => CombatPressureLevel.EnemyTendency,
        CombatPressureLevel.EnemyTendency => CombatPressureLevel.AllyTendency,
        _ => level
    };

    private static int Count<T>(IReadOnlyDictionary<T, int> counts, T key)
        where T : notnull => counts.GetValueOrDefault(key);
}
