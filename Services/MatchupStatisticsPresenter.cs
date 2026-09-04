using System.Globalization;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

public sealed class MatchupStatisticsPresenter
{
    private static readonly CultureInfo DisplayCulture =
        CultureInfo.GetCultureInfo("ko-KR");

    private readonly ChampionMetadataService _championMetadataService;

    public MatchupStatisticsPresenter()
        : this(new ChampionMetadataService())
    {
    }

    public MatchupStatisticsPresenter(ChampionMetadataService championMetadataService)
    {
        _championMetadataService = championMetadataService;
    }

    public async Task<MatchupStatisticsPresentation> PresentAsync(
        MatchupStatisticsResult statistics,
        StatisticsDataset dataset)
    {
        var queryChampion = await _championMetadataService.GetDisplayInfoAsync(
            statistics.QueryChampion);
        var opponentChampion = await _championMetadataService.GetDisplayInfoAsync(
            statistics.OpponentChampion);
        return new MatchupStatisticsPresentation(
            MatchupPresentationState.LoadedStatistics,
            string.Empty,
            statistics.Role,
            statistics.QueryChampion,
            queryChampion.KoreanDisplayName,
            queryChampion.IconSource,
            statistics.OpponentChampion,
            opponentChampion.KoreanDisplayName,
            opponentChampion.IconSource,
            $"표본 {statistics.SampleSize:N0}경기",
            $"패치 {dataset.Patch}",
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
            ],
            CompactMetrics: CreateCompactMetrics(statistics));
    }

    public MatchupStatisticsPresentation PresentStatus(
        MatchupStatisticsQueryStatus status) => status switch
        {
            MatchupStatisticsQueryStatus.NotFound => new(
                MatchupPresentationState.MatchupNotFound,
                "해당 매치업의 통계가 없습니다.",
                StatusDetailText: "게임은 감지되었지만 아직 충분한 통계가 수집되지 않았습니다."),
            MatchupStatisticsQueryStatus.DatasetNotAvailable => new(
                MatchupPresentationState.DatasetNotAvailable,
                "통계 데이터를 사용할 수 없습니다.",
                StatusDetailText: "배포된 통계 데이터 파일을 찾을 수 없습니다."),
            MatchupStatisticsQueryStatus.DatasetInvalid => new(
                MatchupPresentationState.DatasetInvalid,
                "통계 데이터가 올바르지 않습니다.",
                StatusDetailText: "통계 데이터의 형식이나 내용이 유효하지 않습니다."),
            MatchupStatisticsQueryStatus.InvalidAggregate => new(
                MatchupPresentationState.DatasetInvalid,
                "통계 데이터가 올바르지 않습니다.",
                StatusDetailText: "통계 데이터의 집계 값이 유효하지 않습니다."),
            _ => new(
                MatchupPresentationState.Waiting,
                "게임을 기다리는 중...",
                StatusDetailText: "리그 오브 레전드 게임이 시작되면\n매치업 통계를 자동으로 표시합니다.")
        };

    public async Task<MatchupStatisticsPresentation> PresentRoleStatusAsync(
        MatchupStatisticsQueryStatus status,
        string role,
        string? queryChampionId,
        string? opponentChampionId)
    {
        var query = await _championMetadataService.GetDisplayInfoAsync(queryChampionId);
        var opponent = await _championMetadataService.GetDisplayInfoAsync(opponentChampionId);
        var detected = !string.IsNullOrWhiteSpace(queryChampionId)
            && !string.IsNullOrWhiteSpace(opponentChampionId);

        return new MatchupStatisticsPresentation(
            status == MatchupStatisticsQueryStatus.NotFound
                ? MatchupPresentationState.MatchupNotFound
                : MatchupPresentationState.Waiting,
            detected ? "통계 없음" : "감지 중...",
            role,
            queryChampionId ?? string.Empty,
            query.KoreanDisplayName,
            query.IconSource,
            opponentChampionId ?? string.Empty,
            opponent.KoreanDisplayName,
            opponent.IconSource);
    }

    internal static string FormatPercentage(double value) =>
        value.ToString("P1", DisplayCulture);

    internal static string FormatEventsPerMatch(double value) =>
        $"{value.ToString("F2", DisplayCulture)} / 경기";

    private static CompactMatchupMetricsPresentation CreateCompactMetrics(
        MatchupStatisticsResult statistics)
    {
        var unknown = new List<string>();
        AddUnknown(unknown, "L2", statistics.FirstLevel2.UnknownRate);
        AddUnknown(unknown, "L3", statistics.FirstLevel3.UnknownRate);
        AddUnknown(unknown, "L6", statistics.FirstLevel6.UnknownRate);

        return new CompactMatchupMetricsPresentation(
            FormatPercentage(statistics.QueryWinRate),
            FormatPercentage(statistics.OpponentWinRate),
            FormatPercentage(statistics.FirstLevel2.QueryRate),
            FormatPercentage(statistics.FirstLevel2.OpponentRate),
            FormatPercentage(statistics.FirstLevel3.QueryRate),
            FormatPercentage(statistics.FirstLevel3.OpponentRate),
            FormatPercentage(statistics.FirstLevel6.QueryRate),
            FormatPercentage(statistics.FirstLevel6.OpponentRate),
            $"{statistics.SoloKillsPerMatchForQuery.ToString("F2", DisplayCulture)} "
                + $"({statistics.SoloKillsBefore15ForQuery:N0})",
            $"{statistics.SoloKillsPerMatchForOpponent.ToString("F2", DisplayCulture)} "
                + $"({statistics.SoloKillsBefore15ForOpponent:N0})",
            unknown.Count == 0 ? string.Empty : $"미확인: {string.Join(" · ", unknown)}");
    }

    private static void AddUnknown(List<string> values, string label, double rate)
    {
        if (rate > 0)
        {
            values.Add($"{label} {FormatPercentage(rate)}");
        }
    }

    private static MatchupMetricPresentation PresentLevel(
        string label,
        LevelRaceStatistics level) => new(
            label,
            FormatPercentage(level.QueryRate),
            FormatPercentage(level.OpponentRate),
            $"미확인 {FormatPercentage(level.UnknownRate)}");
}
