using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class TeamOutcomeSummaryClassifier
{
    public const int MinimumEligibleRoles = 3;
    public const int MinimumDirectionalRolesForTendency = 2;

    public static TeamOutcomeSummaryResult Classify(
        IReadOnlyCollection<TeamOutcomeRoleEvidence> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);

        var available = roles.Count(role => role.IsAvailable
            && role.OutcomeLevel is not null);
        var ally = roles.Count(role => IsEligible(role)
            && role.OutcomeLevel == OutcomeAdvantageLevel.AllyAdvantage);
        var enemy = roles.Count(role => IsEligible(role)
            && role.OutcomeLevel == OutcomeAdvantageLevel.EnemyAdvantage);
        var neutral = roles.Count(role => IsEligible(role)
            && role.OutcomeLevel == OutcomeAdvantageLevel.Neutral);
        var insufficient = roles.Count(role => role.IsAvailable
            && role.OutcomeLevel == OutcomeAdvantageLevel.InsufficientEvidence);
        var unavailable = roles.Count - available;
        var eligible = ally + enemy + neutral;
        var level = eligible < MinimumEligibleRoles
            ? TeamOutcomeSummaryLevel.InsufficientEvidence
            : ally >= MinimumDirectionalRolesForTendency && ally > enemy
                ? TeamOutcomeSummaryLevel.AllyTendency
                : enemy >= MinimumDirectionalRolesForTendency && enemy > ally
                    ? TeamOutcomeSummaryLevel.EnemyTendency
                    : TeamOutcomeSummaryLevel.Neutral;

        return new TeamOutcomeSummaryResult(
            level,
            roles.Count,
            available,
            eligible,
            ally,
            enemy,
            neutral,
            insufficient,
            unavailable);
    }

    private static bool IsEligible(TeamOutcomeRoleEvidence role) =>
        role.IsAvailable
        && role.OutcomeLevel is OutcomeAdvantageLevel.AllyAdvantage
            or OutcomeAdvantageLevel.Neutral
            or OutcomeAdvantageLevel.EnemyAdvantage;
}
