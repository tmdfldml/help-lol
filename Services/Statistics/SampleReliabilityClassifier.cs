using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class SampleReliabilityClassifier
{
    // Stage 9 production analysis coverage: 1-4=62.45%, 5-19=23.84%,
    // 20-49=8.74%, and 50+=4.97% of canonical matchup keys.
    public const int ReferenceMinimumSampleSize = 5;
    public const int ModerateMinimumSampleSize = 20;
    public const int RelativelyStrongMinimumSampleSize = 50;

    public static SampleReliabilityLevel Classify(int sampleSize) => sampleSize switch
    {
        <= 0 => SampleReliabilityLevel.Unknown,
        < ReferenceMinimumSampleSize => SampleReliabilityLevel.Poor,
        < ModerateMinimumSampleSize => SampleReliabilityLevel.Reference,
        < RelativelyStrongMinimumSampleSize => SampleReliabilityLevel.Moderate,
        _ => SampleReliabilityLevel.RelativelyStrong
    };
}
