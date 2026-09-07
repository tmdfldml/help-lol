using LoLMatchup.Models.Presentation;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace LoLMatchup.Services;

public static class TeamOutcomeSummaryPresenter
{
    public static TeamOutcomeSummaryPresentation Create(
        IReadOnlyCollection<MatchupStatisticsPresentation> matchups)
    {
        var evidence = matchups.Select(matchup => new TeamOutcomeRoleEvidence(
            matchup.HasStatistics,
            matchup.HasOutcomeEvaluation ? matchup.OutcomeAdvantageLevel : null)).ToArray();
        var result = TeamOutcomeSummaryClassifier.Classify(evidence);
        return new TeamOutcomeSummaryPresentation(
            result,
            "팀 결과 경향",
            PresentLevel(result.Level),
            $"판정 {result.EligibleRoleCount}/{result.TotalRoleCount} · "
            + $"아군 {result.AllyDirectionalCount} · 적군 {result.EnemyDirectionalCount} · "
            + $"중립 {result.NeutralCount}",
            TeamOutcomeSummaryExplanationService.Explain(result));
    }

    private static string PresentLevel(TeamOutcomeSummaryLevel level) => level switch
    {
        TeamOutcomeSummaryLevel.AllyTendency => "아군 쪽 경향",
        TeamOutcomeSummaryLevel.EnemyTendency => "적군 쪽 경향",
        TeamOutcomeSummaryLevel.Neutral => "뚜렷한 차이 없음",
        _ => "판정 보류"
    };
}
