#if DEBUG
using System.Diagnostics;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

internal static class MatchupStatisticsDebugValidator
{
    private const double Tolerance = 1e-12;

    public static async Task ValidateAsync()
    {
        var dataService = new StatisticsDataService();
        var loadResult = await dataService.LoadAsync();

        if (loadResult.Status != StatisticsDatasetLoadStatus.Loaded
            || loadResult.Dataset is null)
        {
            Debug.WriteLine($"Statistics dataset load failed: {loadResult.Status}");
            return;
        }

        var dataset = loadResult.Dataset;
        Debug.WriteLine(
            $"Statistics dataset loaded: {dataset.TierScope} · "
            + $"Patch {dataset.Patch} · {dataset.PolicyKey ?? "no policy"}; "
            + $"matches={dataset.AcceptedMatches}, keys={loadResult.MatchupIndex.Count}");

        var sample = dataset.Matchups?.FirstOrDefault(matchup =>
            matchup.SampleSize > 0
            && !string.Equals(
                matchup.ChampionA,
                matchup.ChampionB,
                StringComparison.OrdinalIgnoreCase));

        if (sample is null)
        {
            Debug.WriteLine("Statistics calculation check skipped: no valid matchup.");
            return;
        }

        var statisticsService = new MatchupStatisticsService(dataService);
        var forwardQuery = await statisticsService.GetStatisticsAsync(
            sample.Role,
            sample.ChampionA,
            sample.ChampionB);
        var reverseQuery = await statisticsService.GetStatisticsAsync(
            sample.Role,
            sample.ChampionB,
            sample.ChampionA);
        var forward = forwardQuery.Statistics;
        var reverse = reverseQuery.Statistics;
        var directionsValid = forwardQuery.Status == MatchupStatisticsQueryStatus.Success
            && reverseQuery.Status == MatchupStatisticsQueryStatus.Success
            && forward is not null
            && reverse is not null
            && !forward.WasReversed
            && reverse.WasReversed
            && forward.SampleSize == reverse.SampleSize
            && forward.WinsForQuery == reverse.WinsForOpponent
            && forward.WinsForOpponent == reverse.WinsForQuery
            && NearlyEqual(forward.QueryWinRate, reverse.OpponentWinRate)
            && LevelDirectionsMatch(forward.FirstLevel2, reverse.FirstLevel2)
            && LevelDirectionsMatch(forward.FirstLevel3, reverse.FirstLevel3)
            && LevelDirectionsMatch(forward.FirstLevel6, reverse.FirstLevel6)
            && forward.SoloKillsBefore15ForQuery
                == reverse.SoloKillsBefore15ForOpponent
            && forward.SoloKillsBefore15ForOpponent
                == reverse.SoloKillsBefore15ForQuery;
        var ratesValid = forward is not null
            && reverse is not null
            && StatisticsRatesAreValid(forward)
            && StatisticsRatesAreValid(reverse);

        if (forward is null)
        {
            Debug.WriteLine("Statistics calculation validation: False");
            return;
        }
        var formattingValid = MatchupStatisticsPresenter.FormatPercentage(0) == "0.0%"
            && MatchupStatisticsPresenter.FormatPercentage(0.5) == "50.0%"
            && MatchupStatisticsPresenter.FormatPercentage(1) == "100.0%"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(0) == "0.00 / 경기"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(0.284) == "0.28 / 경기"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(1.2) == "1.20 / 경기";
        Debug.WriteLine(
            "Statistics validation: "
            + $"directions={directionsValid}, rates={ratesValid}, "
            + $"formatting={formattingValid}");
    }

    private static bool LevelDirectionsMatch(
        LevelRaceStatistics forward,
        LevelRaceStatistics reverse) =>
        forward.ForQuery == reverse.ForOpponent
        && forward.ForOpponent == reverse.ForQuery
        && forward.Unknown == reverse.Unknown;

    private static bool StatisticsRatesAreValid(MatchupStatisticsResult value) =>
        IsUnitRate(value.QueryWinRate)
        && IsUnitRate(value.OpponentWinRate)
        && NearlyEqual(value.QueryWinRate + value.OpponentWinRate, 1)
        && LevelRatesAreValid(value.FirstLevel2)
        && LevelRatesAreValid(value.FirstLevel3)
        && LevelRatesAreValid(value.FirstLevel6)
        && value.SoloKillsPerMatchForQuery >= 0
        && value.SoloKillsPerMatchForOpponent >= 0;

    private static bool LevelRatesAreValid(LevelRaceStatistics value) =>
        IsUnitRate(value.QueryRate)
        && IsUnitRate(value.OpponentRate)
        && IsUnitRate(value.UnknownRate)
        && NearlyEqual(
            value.QueryRate + value.OpponentRate + value.UnknownRate,
            1);

    private static bool IsUnitRate(double value) => value is >= 0 and <= 1;

    private static bool NearlyEqual(double left, double right) =>
        Math.Abs(left - right) <= Tolerance;
}
#endif
