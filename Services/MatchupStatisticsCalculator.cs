using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

public sealed class MatchupStatisticsCalculator
{
    public bool TryCalculate(
        StatisticsMatchupLookupResult lookup,
        string queryChampion,
        string opponentChampion,
        out MatchupStatisticsResult? result)
    {
        result = null;
        var matchup = lookup.Matchup;

        if (matchup.SampleSize <= 0
            || string.IsNullOrWhiteSpace(queryChampion)
            || string.IsNullOrWhiteSpace(opponentChampion))
        {
            return false;
        }

        var winsForOpponent = matchup.SampleSize - matchup.WinsForA;

        if (winsForOpponent < 0)
        {
            return false;
        }

        var winsForQuery = lookup.WasReversed
            ? winsForOpponent
            : matchup.WinsForA;
        var opponentWins = lookup.WasReversed
            ? matchup.WinsForA
            : winsForOpponent;
        var firstLevel2 = CreateLevelRace(
            matchup.FirstLevel2ForA,
            matchup.FirstLevel2ForB,
            matchup.FirstLevel2Unknown,
            matchup.SampleSize,
            lookup.WasReversed);
        var firstLevel3 = CreateLevelRace(
            matchup.FirstLevel3ForA,
            matchup.FirstLevel3ForB,
            matchup.FirstLevel3Unknown,
            matchup.SampleSize,
            lookup.WasReversed);
        var firstLevel6 = CreateLevelRace(
            matchup.FirstLevel6ForA,
            matchup.FirstLevel6ForB,
            matchup.FirstLevel6Unknown,
            matchup.SampleSize,
            lookup.WasReversed);
        var soloKillsForQuery = lookup.WasReversed
            ? matchup.SoloKillsBefore15ForB
            : matchup.SoloKillsBefore15ForA;
        var soloKillsForOpponent = lookup.WasReversed
            ? matchup.SoloKillsBefore15ForA
            : matchup.SoloKillsBefore15ForB;

        result = new MatchupStatisticsResult(
            matchup.Role,
            queryChampion.Trim(),
            opponentChampion.Trim(),
            matchup.SampleSize,
            winsForQuery,
            opponentWins,
            Divide(winsForQuery, matchup.SampleSize),
            Divide(opponentWins, matchup.SampleSize),
            firstLevel2,
            firstLevel3,
            firstLevel6,
            soloKillsForQuery,
            soloKillsForOpponent,
            Divide(soloKillsForQuery, matchup.SampleSize),
            Divide(soloKillsForOpponent, matchup.SampleSize),
            lookup.WasReversed);
        return true;
    }

    private static LevelRaceStatistics CreateLevelRace(
        int forA,
        int forB,
        int unknown,
        int sampleSize,
        bool wasReversed)
    {
        var forQuery = wasReversed ? forB : forA;
        var forOpponent = wasReversed ? forA : forB;

        return new LevelRaceStatistics(
            forQuery,
            forOpponent,
            unknown,
            Divide(forQuery, sampleSize),
            Divide(forOpponent, sampleSize),
            Divide(unknown, sampleSize));
    }

    private static double Divide(int numerator, int denominator) =>
        (double)numerator / denominator;
}
