using LoLMatchup.Models.Presentation;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class MatchupExplanationService
{
    public static MatchupExplanation Create(
        SampleReliabilityLevel reliability,
        OutcomeAdvantageResult outcome,
        LevelTempoResult levelTempo,
        CombatPressureResult combatPressure) => new(
            ExplainReliability(reliability),
            ExplainOutcome(outcome),
            ExplainLevelTempo(levelTempo),
            ExplainCombatPressure(combatPressure));

    public static string ExplainReliability(SampleReliabilityLevel level) => level switch
    {
        SampleReliabilityLevel.Poor => "표본이 1~4경기로 매우 적어 수치 변동성이 큽니다.",
        SampleReliabilityLevel.Reference => "표본이 5~19경기로 적어 참고용으로 확인하는 것이 좋습니다.",
        SampleReliabilityLevel.Moderate => "표본이 20~49경기로 비교에 활용할 수 있지만 표본 오차가 있을 수 있습니다.",
        SampleReliabilityLevel.RelativelyStrong => "표본이 50경기 이상으로 현재 데이터셋에서 상대적으로 충분합니다.",
        _ => string.Empty
    };

    public static string ExplainOutcome(OutcomeAdvantageResult result) => result.Reason switch
    {
        OutcomeAdvantageReason.InsufficientSample =>
            $"표본이 {OutcomeAdvantageClassifier.MinimumSampleSize}경기 미만이어서 결과 경향을 판정하지 않습니다.",
        OutcomeAdvantageReason.AllyEffectAndConfidence =>
            $"{result.SampleSize:N0}경기에서 아군 최종 승률이 "
            + $"{StatisticsDisplayFormatter.Percentage(result.ObservedAllyWinRate)}였고, "
            + "현재 표본 기준에서 아군 쪽 승률 경향이 확인됩니다.",
        OutcomeAdvantageReason.EnemyEffectAndConfidence =>
            $"{result.SampleSize:N0}경기에서 아군 최종 승률이 "
            + $"{StatisticsDisplayFormatter.Percentage(result.ObservedAllyWinRate)}였고, "
            + "현재 표본 기준에서는 적군 쪽 승률 경향이 확인됩니다.",
        OutcomeAdvantageReason.ConfidenceIncludesNeutral =>
            $"아군 승률은 {StatisticsDisplayFormatter.Percentage(result.ObservedAllyWinRate)}이지만 "
            + "현재 표본의 불확실성을 고려하면 한쪽 결과 경향으로 판정할 근거가 충분하지 않습니다.",
        _ =>
            $"아군 승률이 {StatisticsDisplayFormatter.Percentage(result.ObservedAllyWinRate)}로 "
            + "50%와의 차이가 현재 기준보다 작아 뚜렷한 결과 경향으로 판정하지 않습니다."
    };

    public static string ExplainLevelTempo(LevelTempoResult result)
    {
        if (result.Level == LevelTempoLevel.InsufficientEvidence)
        {
            return $"표본이 {LevelTempoClassifier.MinimumSampleSize}경기 미만이어서 레벨 템포를 판정하지 않습니다.";
        }

        if (result.Level == LevelTempoLevel.AllyTendency)
        {
            return $"{Milestones(result, LevelTempoSignal.Ally)} 선취가 아군 쪽에서 기준 이상 높아 "
                + "아군 쪽 레벨 템포 경향으로 판정됩니다.";
        }

        if (result.Level == LevelTempoLevel.EnemyTendency)
        {
            return $"{Milestones(result, LevelTempoSignal.Enemy)} 선취가 적군 쪽에서 기준 이상 높아 "
                + "적군 쪽 레벨 템포 경향으로 판정됩니다.";
        }

        var hasConflict = result.AllySignalCount > 0 && result.EnemySignalCount > 0;
        return hasConflict
            ? "레벨 선취 지표가 서로 다른 방향을 보여 한쪽 레벨 템포 경향으로 판정하지 않습니다."
            : "L2·L3·L6 선취 신호가 한쪽으로 충분히 모이지 않아 뚜렷한 레벨 템포 차이로 판정하지 않습니다.";
    }

    public static string ExplainCombatPressure(CombatPressureResult result)
    {
        if (result.Level == CombatPressureLevel.InsufficientEvidence)
        {
            return $"표본이 {CombatPressureClassifier.MinimumSampleSize}경기 미만이어서 교전 압박을 판정하지 않습니다.";
        }

        var rates = "15분 이전 맞상대 솔로킬이 아군 "
            + $"{StatisticsDisplayFormatter.CompactEventsPerMatch(result.AllyEventsPerMatch)}, 적군 "
            + $"{StatisticsDisplayFormatter.CompactEventsPerMatch(result.EnemyEventsPerMatch)}로 나타나";
        return result.Level switch
        {
            CombatPressureLevel.AllyTendency =>
                $"{rates} 아군 쪽 교전 경향으로 판정됩니다.",
            CombatPressureLevel.EnemyTendency =>
                $"{rates} 적군 쪽 교전 경향으로 판정됩니다.",
            _ =>
                $"{rates} 양쪽 차이가 경기당 "
                + $"{CombatPressureClassifier.MinimumDifferencePerMatch:F2}보다 작아 "
                + "뚜렷한 교전 압박 차이로 판정하지 않습니다."
        };
    }

    private static string Milestones(LevelTempoResult result, LevelTempoSignal signal)
    {
        var values = new List<string>(3);
        if (result.Level2Signal == signal) values.Add("L2");
        if (result.Level3Signal == signal) values.Add("L3");
        if (result.Level6Signal == signal) values.Add("L6");
        return string.Join('·', values);
    }
}
