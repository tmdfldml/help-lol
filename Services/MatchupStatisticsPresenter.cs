using System.Globalization;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

public sealed class MatchupStatisticsPresenter
{
    private static readonly CultureInfo DisplayCulture =
        CultureInfo.GetCultureInfo("ko-KR");

    public MatchupStatisticsPresentation Present(
        MatchupStatisticsResult statistics,
        StatisticsDataset dataset)
    {
        var metadataParts = new List<string>
        {
            dataset.TierScope,
            $"Patch {dataset.Patch}"
        };

        if (!string.IsNullOrWhiteSpace(dataset.PolicyKey))
        {
            metadataParts.Add(dataset.PolicyKey);
        }

        return new MatchupStatisticsPresentation(
            MatchupPresentationState.LoadedStatistics,
            string.Empty,
            statistics.Role,
            statistics.QueryChampion,
            statistics.OpponentChampion,
            $"표본 {statistics.SampleSize:N0}경기",
            string.Join(" · ", metadataParts),
            [
                new MatchupMetricPresentation(
                    "승률",
                    FormatPercentage(statistics.QueryWinRate),
                    FormatPercentage(statistics.OpponentWinRate),
                    $"{statistics.WinsForQuery:N0}승 / {statistics.SampleSize:N0}경기"),
                PresentLevel("선 2레벨", statistics.FirstLevel2),
                PresentLevel("선 3레벨", statistics.FirstLevel3),
                PresentLevel("선 6레벨", statistics.FirstLevel6),
                new MatchupMetricPresentation(
                    "15분 이전 맞상대 솔로킬",
                    FormatEventsPerMatch(statistics.SoloKillsPerMatchForQuery),
                    FormatEventsPerMatch(statistics.SoloKillsPerMatchForOpponent),
                    $"총 {statistics.SoloKillsBefore15ForQuery:N0}회 / "
                    + $"{statistics.SoloKillsBefore15ForOpponent:N0}회")
            ]);
    }

    public MatchupStatisticsPresentation PresentStatus(
        MatchupStatisticsQueryStatus status) => status switch
        {
            MatchupStatisticsQueryStatus.NotFound => new(
                MatchupPresentationState.MatchupNotFound,
                "해당 매치업의 통계가 없습니다."),
            MatchupStatisticsQueryStatus.DatasetNotAvailable => new(
                MatchupPresentationState.DatasetNotAvailable,
                "통계 데이터를 사용할 수 없습니다."),
            MatchupStatisticsQueryStatus.DatasetInvalid => new(
                MatchupPresentationState.DatasetInvalid,
                "통계 데이터가 올바르지 않습니다."),
            MatchupStatisticsQueryStatus.InvalidAggregate => new(
                MatchupPresentationState.DatasetInvalid,
                "통계 데이터가 올바르지 않습니다."),
            _ => new(
                MatchupPresentationState.Waiting,
                "게임을 기다리는 중...")
        };

    internal static string FormatPercentage(double value) =>
        value.ToString("P1", DisplayCulture);

    internal static string FormatEventsPerMatch(double value) =>
        $"{value.ToString("F2", DisplayCulture)} / 경기";

    private static MatchupMetricPresentation PresentLevel(
        string label,
        LevelRaceStatistics level) => new(
            label,
            FormatPercentage(level.QueryRate),
            FormatPercentage(level.OpponentRate),
            $"미확인 {FormatPercentage(level.UnknownRate)}");
}
