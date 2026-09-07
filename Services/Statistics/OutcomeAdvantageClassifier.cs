using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public static class OutcomeAdvantageClassifier
{
    public const int MinimumSampleSize = 20;
    public const int MinimumAbsoluteWinRateDifferencePercentagePoints = 5;
    public const double MinimumAbsoluteWinRateDifference =
        MinimumAbsoluteWinRateDifferencePercentagePoints / 100d;
    public const double WilsonConfidenceLevel = 0.80;

    // Two-sided 80% normal critical value used by the Stage 9-2A research model.
    private const double WilsonZ = 1.2815515655446004;
    private const int PercentageScale = 100;
    private const int NeutralWinRatePercentage = 50;

    public static OutcomeAdvantageResult Classify(int winsForAlly, int sampleSize)
    {
        ValidateInput(winsForAlly, sampleSize);

        var winRate = sampleSize == 0 ? 0 : (double)winsForAlly / sampleSize;
        var interval = WilsonIntervalCalculator.Calculate(
            winsForAlly,
            sampleSize,
            WilsonZ);
        var level = sampleSize < MinimumSampleSize
            ? OutcomeAdvantageLevel.InsufficientEvidence
            : ClassifyEligible(winsForAlly, sampleSize, interval);
        var reason = DetermineReason(winsForAlly, sampleSize, level);

        return new OutcomeAdvantageResult(
            level,
            sampleSize,
            winsForAlly,
            winRate,
            interval.LowerBound,
            interval.UpperBound,
            reason);
    }

    internal static OutcomeAdvantageLevel ClassifyEligible(
        int winsForAlly,
        int sampleSize,
        WilsonInterval interval)
    {
        var scaledWins = (long)winsForAlly * PercentageScale;
        var scaledSample = (long)sampleSize;

        if (scaledWins >= scaledSample
                * (NeutralWinRatePercentage
                    + MinimumAbsoluteWinRateDifferencePercentagePoints)
            && interval.LowerBound > 0.5)
        {
            return OutcomeAdvantageLevel.AllyAdvantage;
        }

        if (scaledWins <= scaledSample
                * (NeutralWinRatePercentage
                    - MinimumAbsoluteWinRateDifferencePercentagePoints)
            && interval.UpperBound < 0.5)
        {
            return OutcomeAdvantageLevel.EnemyAdvantage;
        }

        return OutcomeAdvantageLevel.Neutral;
    }

    private static void ValidateInput(int winsForAlly, int sampleSize)
    {
        if (sampleSize < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sampleSize),
                "Sample size cannot be negative.");
        }

        if (winsForAlly < 0 || winsForAlly > sampleSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(winsForAlly),
                "Wins must be between zero and the sample size.");
        }
    }

    private static OutcomeAdvantageReason DetermineReason(
        int winsForAlly,
        int sampleSize,
        OutcomeAdvantageLevel level)
    {
        if (level == OutcomeAdvantageLevel.InsufficientEvidence)
        {
            return OutcomeAdvantageReason.InsufficientSample;
        }

        if (level == OutcomeAdvantageLevel.AllyAdvantage)
        {
            return OutcomeAdvantageReason.AllyEffectAndConfidence;
        }

        if (level == OutcomeAdvantageLevel.EnemyAdvantage)
        {
            return OutcomeAdvantageReason.EnemyEffectAndConfidence;
        }

        var scaledWins = (long)winsForAlly * PercentageScale;
        var scaledSample = (long)sampleSize;
        var effectReachesThreshold = scaledWins >= scaledSample
                * (NeutralWinRatePercentage
                    + MinimumAbsoluteWinRateDifferencePercentagePoints)
            || scaledWins <= scaledSample
                * (NeutralWinRatePercentage
                    - MinimumAbsoluteWinRateDifferencePercentagePoints);
        return effectReachesThreshold
            ? OutcomeAdvantageReason.ConfidenceIncludesNeutral
            : OutcomeAdvantageReason.EffectBelowThreshold;
    }
}

public static class WilsonIntervalCalculator
{
    public static WilsonInterval Calculate(int successes, int sampleSize, double z)
    {
        if (sampleSize == 0)
        {
            return new WilsonInterval(0, 1);
        }

        var proportion = (double)successes / sampleSize;
        var zSquared = z * z;
        var denominator = 1 + zSquared / sampleSize;
        var center = (proportion + zSquared / (2 * sampleSize)) / denominator;
        var margin = z * Math.Sqrt(
            proportion * (1 - proportion) / sampleSize
            + zSquared / (4d * sampleSize * sampleSize)) / denominator;

        return new WilsonInterval(
            Math.Max(0, center - margin),
            Math.Min(1, center + margin));
    }
}
