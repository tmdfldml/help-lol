using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class CombatPressureClassifier
{
    public const int MinimumSampleSize = 20;
    public const double MinimumDifferencePerMatch = 0.10;

    public static CombatPressureResult Classify(
        int sampleSize,
        int soloKillsForAlly,
        int soloKillsForEnemy)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sampleSize);
        ArgumentOutOfRangeException.ThrowIfNegative(soloKillsForAlly);
        ArgumentOutOfRangeException.ThrowIfNegative(soloKillsForEnemy);

        var allyRate = sampleSize == 0 ? 0 : (double)soloKillsForAlly / sampleSize;
        var enemyRate = sampleSize == 0 ? 0 : (double)soloKillsForEnemy / sampleSize;
        var difference = sampleSize == 0
            ? 0
            : (double)(soloKillsForAlly - soloKillsForEnemy) / sampleSize;
        var level = sampleSize < MinimumSampleSize
            ? CombatPressureLevel.InsufficientEvidence
            : difference >= MinimumDifferencePerMatch
                ? CombatPressureLevel.AllyTendency
                : difference <= -MinimumDifferencePerMatch
                    ? CombatPressureLevel.EnemyTendency
                    : CombatPressureLevel.Neutral;

        return new CombatPressureResult(level, sampleSize, allyRate, enemyRate, difference);
    }
}
