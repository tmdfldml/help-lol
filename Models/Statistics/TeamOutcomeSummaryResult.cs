namespace LoLMatchup.Models.Statistics;

public sealed record TeamOutcomeRoleEvidence(
    bool IsAvailable,
    OutcomeAdvantageLevel? OutcomeLevel);

public sealed record TeamOutcomeSummaryResult(
    TeamOutcomeSummaryLevel Level,
    int TotalRoleCount,
    int AvailableRoleCount,
    int EligibleRoleCount,
    int AllyDirectionalCount,
    int EnemyDirectionalCount,
    int NeutralCount,
    int InsufficientCount,
    int UnavailableCount);
