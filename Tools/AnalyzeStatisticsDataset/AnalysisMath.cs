namespace AnalyzeStatisticsDataset;

internal static class AnalysisMath
{
    public static Distribution Describe(IEnumerable<double> source)
    {
        var values = source.Order().ToArray();
        if (values.Length == 0) return new();
        return new Distribution(
            values[0], values[^1], values.Average(), Percentile(values, .5),
            Percentile(values, .1), Percentile(values, .25), Percentile(values, .5), Percentile(values, .75),
            Percentile(values, .9), Percentile(values, .95), Percentile(values, .99),
            StandardDeviation(values));
    }

    // Linear interpolation between adjacent ranks using index (N - 1) * p.
    public static double Percentile(IReadOnlyList<double> sorted, double probability)
    {
        if (sorted.Count == 0) return 0;
        var rank = (sorted.Count - 1) * probability;
        var lower = (int)Math.Floor(rank);
        var upper = (int)Math.Ceiling(rank);
        return lower == upper
            ? sorted[lower]
            : sorted[lower] + (sorted[upper] - sorted[lower]) * (rank - lower);
    }

    public static double StandardDeviation(IReadOnlyCollection<double> values)
    {
        if (values.Count == 0) return 0;
        var mean = values.Average();
        return Math.Sqrt(values.Sum(value => Math.Pow(value - mean, 2)) / values.Count);
    }

    public static WilsonInterval Wilson(int wins, int sampleSize)
        => Wilson(wins, sampleSize, .95);

    public static WilsonInterval Wilson(int wins, int sampleSize, double confidenceLevel)
        => WilsonAtRate(
            sampleSize <= 0 ? 0 : (double)wins / sampleSize,
            sampleSize,
            confidenceLevel);

    public static WilsonInterval WilsonAtRate(double p, int sampleSize)
        => WilsonAtRate(p, sampleSize, .95);

    public static WilsonInterval WilsonAtRate(
        double p,
        int sampleSize,
        double confidenceLevel)
    {
        if (sampleSize <= 0) return new();
        var z = confidenceLevel switch
        {
            .80 => 1.2815515655446004,
            .90 => 1.6448536269514722,
            .95 => 1.959963984540054,
            _ => throw new ArgumentOutOfRangeException(
                nameof(confidenceLevel),
                "Supported confidence levels are 0.80, 0.90, and 0.95.")
        };
        var z2 = z * z;
        var denominator = 1 + z2 / sampleSize;
        var center = (p + z2 / (2 * sampleSize)) / denominator;
        var margin = z * Math.Sqrt((p * (1 - p) + z2 / (4 * sampleSize)) / sampleSize) / denominator;
        return new WilsonInterval(Math.Max(0, center - margin), Math.Min(1, center + margin));
    }

    public static double Pearson(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
        if (x.Count != y.Count || x.Count < 2) return 0;
        var meanX = x.Average();
        var meanY = y.Average();
        var numerator = 0d;
        var squareX = 0d;
        var squareY = 0d;
        for (var index = 0; index < x.Count; index++)
        {
            var dx = x[index] - meanX;
            var dy = y[index] - meanY;
            numerator += dx * dy;
            squareX += dx * dx;
            squareY += dy * dy;
        }
        var denominator = Math.Sqrt(squareX * squareY);
        return denominator == 0 ? 0 : numerator / denominator;
    }
}

internal sealed record Distribution(
    double Min = 0, double Max = 0, double Mean = 0, double Median = 0,
    double P10 = 0, double P25 = 0, double P50 = 0, double P75 = 0, double P90 = 0,
    double P95 = 0, double P99 = 0, double StandardDeviation = 0);

internal sealed record WilsonInterval(double Lower = 0, double Upper = 0)
{
    public double Width => Upper - Lower;
}
