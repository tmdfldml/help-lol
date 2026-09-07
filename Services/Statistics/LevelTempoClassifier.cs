using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class LevelTempoClassifier
{
    public const int MinimumSampleSize = 20;
    public const double MinimumDifference = 0.05;

    public static LevelTempoResult Classify(
        int sampleSize,
        int level2ForAlly,
        int level2ForEnemy,
        int level2Unknown,
        int level3ForAlly,
        int level3ForEnemy,
        int level3Unknown,
        int level6ForAlly,
        int level6ForEnemy,
        int level6Unknown)
    {
        ValidateLevelCounts(sampleSize, level2ForAlly, level2ForEnemy, level2Unknown);
        ValidateLevelCounts(sampleSize, level3ForAlly, level3ForEnemy, level3Unknown);
        ValidateLevelCounts(sampleSize, level6ForAlly, level6ForEnemy, level6Unknown);

        var level2Difference = Difference(level2ForAlly, level2ForEnemy, sampleSize);
        var level3Difference = Difference(level3ForAlly, level3ForEnemy, sampleSize);
        var level6Difference = Difference(level6ForAlly, level6ForEnemy, sampleSize);
        var level2Signal = Signal(level2Difference);
        var level3Signal = Signal(level3Difference);
        var level6Signal = Signal(level6Difference);
        var signals = new[] { level2Signal, level3Signal, level6Signal };
        var allySignals = signals.Count(value => value == LevelTempoSignal.Ally);
        var enemySignals = signals.Count(value => value == LevelTempoSignal.Enemy);
        var level = sampleSize < MinimumSampleSize
            ? LevelTempoLevel.InsufficientEvidence
            : allySignals >= 2
                ? LevelTempoLevel.AllyTendency
                : enemySignals >= 2
                    ? LevelTempoLevel.EnemyTendency
                    : LevelTempoLevel.Neutral;

        return new LevelTempoResult(
            level,
            sampleSize,
            level2Difference,
            level3Difference,
            level6Difference,
            level2Signal,
            level3Signal,
            level6Signal,
            allySignals,
            enemySignals);
    }

    private static double Difference(int ally, int enemy, int sampleSize) =>
        sampleSize == 0 ? 0 : (double)(ally - enemy) / sampleSize;

    private static LevelTempoSignal Signal(double difference) => difference switch
    {
        >= MinimumDifference => LevelTempoSignal.Ally,
        <= -MinimumDifference => LevelTempoSignal.Enemy,
        _ => LevelTempoSignal.Neutral
    };

    private static void ValidateLevelCounts(int sampleSize, int ally, int enemy, int unknown)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sampleSize);
        ArgumentOutOfRangeException.ThrowIfNegative(ally);
        ArgumentOutOfRangeException.ThrowIfNegative(enemy);
        ArgumentOutOfRangeException.ThrowIfNegative(unknown);
        if (ally + enemy + unknown != sampleSize)
        {
            throw new ArgumentException("Level counts must add up to the sample size.");
        }
    }
}
