using LoLMatchup.Models.Presentation;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace LoLMatchup.Services;

public sealed class MatchupStatisticsPresenter
{
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
        var reliabilityLevel = SampleReliabilityClassifier.Classify(
            statistics.SampleSize);
        var reliabilityText = PresentReliability(reliabilityLevel);
        var outcomeAdvantage = OutcomeAdvantageClassifier.Classify(
            statistics.WinsForQuery,
            statistics.SampleSize);
        var outcomeText = PresentOutcomeAdvantage(outcomeAdvantage.Level);
        var levelTempo = LevelTempoClassifier.Classify(
            statistics.SampleSize,
            statistics.FirstLevel2.ForQuery,
            statistics.FirstLevel2.ForOpponent,
            statistics.FirstLevel2.Unknown,
            statistics.FirstLevel3.ForQuery,
            statistics.FirstLevel3.ForOpponent,
            statistics.FirstLevel3.Unknown,
            statistics.FirstLevel6.ForQuery,
            statistics.FirstLevel6.ForOpponent,
            statistics.FirstLevel6.Unknown);
        var levelTempoText = PresentLevelTempo(levelTempo.Level);
        var combatPressure = CombatPressureClassifier.Classify(
            statistics.SampleSize,
            statistics.SoloKillsBefore15ForQuery,
            statistics.SoloKillsBefore15ForOpponent);
        var combatPressureText = PresentCombatPressure(combatPressure.Level);
        var explanation = MatchupExplanationService.Create(
            reliabilityLevel,
            outcomeAdvantage,
            levelTempo,
            combatPressure);
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
                    + $"{statistics.SoloKillsBefore15ForOpponent:N0}회",
                    $"총 {statistics.SoloKillsBefore15ForQuery:N0}회",
                    $"총 {statistics.SoloKillsBefore15ForOpponent:N0}회")
            ],
            CompactMetrics: CreateCompactMetrics(statistics),
            ReliabilityLevel: reliabilityLevel,
            ReliabilityText: reliabilityText,
            ReliabilityDescription: explanation.Reliability,
            OutcomeAdvantage: outcomeAdvantage,
            OutcomeAdvantageText: outcomeText,
            OutcomeAdvantageDescription: explanation.Outcome,
            LevelTempo: levelTempo,
            LevelTempoText: levelTempoText,
            LevelTempoDescription: explanation.LevelTempo,
            CombatPressure: combatPressure,
            CombatPressureText: combatPressureText,
            CombatPressureDescription: explanation.CombatPressure);
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
        StatisticsDisplayFormatter.Percentage(value);

    internal static string FormatEventsPerMatch(double value) =>
        StatisticsDisplayFormatter.EventsPerMatch(value);

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
            $"{StatisticsDisplayFormatter.Decimal2(statistics.SoloKillsPerMatchForQuery)} "
                + $"({statistics.SoloKillsBefore15ForQuery:N0})",
            $"{StatisticsDisplayFormatter.Decimal2(statistics.SoloKillsPerMatchForOpponent)} "
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

    private static string PresentReliability(
        SampleReliabilityLevel level) => level switch
        {
            SampleReliabilityLevel.Poor => "표본 부족",
            SampleReliabilityLevel.Reference => "참고용",
            SampleReliabilityLevel.Moderate => "보통",
            SampleReliabilityLevel.RelativelyStrong => "상대적으로 충분",
            _ => string.Empty
        };

    private static string PresentOutcomeAdvantage(
        OutcomeAdvantageLevel level) => level switch
        {
            OutcomeAdvantageLevel.AllyAdvantage => "아군 우세 경향",
            OutcomeAdvantageLevel.EnemyAdvantage => "적군 우세 경향",
            OutcomeAdvantageLevel.Neutral => "뚜렷한 차이 없음",
            _ => "판정 보류"
        };

    private static string PresentLevelTempo(
        LevelTempoLevel level) => level switch
        {
            LevelTempoLevel.AllyTendency => "아군 쪽 경향",
            LevelTempoLevel.EnemyTendency => "적군 쪽 경향",
            LevelTempoLevel.Neutral => "뚜렷한 차이 없음",
            _ => "판정 보류"
        };

    private static string PresentCombatPressure(
        CombatPressureLevel level) => level switch
        {
            CombatPressureLevel.AllyTendency => "아군 쪽 경향",
            CombatPressureLevel.EnemyTendency => "적군 쪽 경향",
            CombatPressureLevel.Neutral => "뚜렷한 차이 없음",
            _ => "판정 보류"
        };
}
