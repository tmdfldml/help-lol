using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class TeamOutcomeSummaryExplanationService
{
    public static string Explain(TeamOutcomeSummaryResult result) => result.Level switch
    {
        TeamOutcomeSummaryLevel.InsufficientEvidence =>
            $"판정 가능한 포지션이 {TeamOutcomeSummaryClassifier.MinimumEligibleRoles}개 미만이어서 "
            + "팀 결과 경향을 판정하지 않습니다.",
        TeamOutcomeSummaryLevel.AllyTendency =>
            $"판정 가능한 {result.EligibleRoleCount}개 포지션 중 아군 쪽 결과 경향이 "
            + $"{result.AllyDirectionalCount}개로 적군보다 많습니다.",
        TeamOutcomeSummaryLevel.EnemyTendency =>
            $"판정 가능한 {result.EligibleRoleCount}개 포지션 중 적군 쪽 결과 경향이 "
            + $"{result.EnemyDirectionalCount}개로 아군보다 많습니다.",
        _ when result.AllyDirectionalCount == result.EnemyDirectionalCount
            && result.AllyDirectionalCount > 0 =>
            "아군과 적군 방향의 포지션 수가 같아 한쪽 팀 결과 경향으로 요약하지 않습니다.",
        _ =>
            $"한쪽 방향으로 판정된 포지션이 "
            + $"{TeamOutcomeSummaryClassifier.MinimumDirectionalRolesForTendency}개 미만이어서 "
            + "뚜렷한 팀 결과 경향으로 요약하지 않습니다."
    };
}
