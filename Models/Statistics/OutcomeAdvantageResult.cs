namespace LoLMatchup.Models.Statistics;

public sealed record OutcomeAdvantageResult(
    OutcomeAdvantageLevel Level,
    int SampleSize,
    int WinsForAlly,
    double ObservedAllyWinRate,
    double WilsonLowerBound,
    double WilsonUpperBound,
    OutcomeAdvantageReason Reason);

public enum OutcomeAdvantageReason
{
    InsufficientSample,
    EffectBelowThreshold,
    ConfidenceIncludesNeutral,
    AllyEffectAndConfidence,
    EnemyEffectAndConfidence
}

public readonly record struct WilsonInterval(double LowerBound, double UpperBound);
