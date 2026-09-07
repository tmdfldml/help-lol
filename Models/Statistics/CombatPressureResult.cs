namespace LoLMatchup.Models.Statistics;

public sealed record CombatPressureResult(
    CombatPressureLevel Level,
    int SampleSize,
    double AllyEventsPerMatch,
    double EnemyEventsPerMatch,
    double DifferencePerMatch);
