namespace LoLMatchup.Models.Statistics;

public sealed record LevelTempoResult(
    LevelTempoLevel Level,
    int SampleSize,
    double Level2Difference,
    double Level3Difference,
    double Level6Difference,
    LevelTempoSignal Level2Signal,
    LevelTempoSignal Level3Signal,
    LevelTempoSignal Level6Signal,
    int AllySignalCount,
    int EnemySignalCount);
