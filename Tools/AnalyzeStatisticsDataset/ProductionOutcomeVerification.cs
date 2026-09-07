using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

internal static class ProductionOutcomeVerification
{
    public static OutcomeCounts Verify(
        IReadOnlyCollection<StatisticsMatchupEntry> entries,
        RecommendationSummary researchRecommendation)
    {
        var outcomes = entries.Select(entry => new
        {
            Forward = OutcomeAdvantageClassifier.Classify(
                entry.WinsForA,
                entry.SampleSize).Level,
            Reverse = OutcomeAdvantageClassifier.Classify(
                entry.SampleSize - entry.WinsForA,
                entry.SampleSize).Level
        }).ToArray();
        var symmetryValid = outcomes.All(item => item.Reverse == Reverse(item.Forward));
        var counts = new OutcomeCounts(
            outcomes.Length,
            outcomes.Count(item => item.Forward == OutcomeAdvantageLevel.AllyAdvantage),
            outcomes.Count(item => item.Forward == OutcomeAdvantageLevel.EnemyAdvantage),
            outcomes.Count(item => item.Forward == OutcomeAdvantageLevel.Neutral),
            outcomes.Count(item => item.Forward == OutcomeAdvantageLevel.InsufficientEvidence));
        var matchesResearch = counts == researchRecommendation.Counts;

        if (!symmetryValid || !matchesResearch)
        {
            throw new InvalidDataException(
                "Production outcome classifier does not match the recommended research model.");
        }

        Console.WriteLine(
            "Production outcome verification: "
            + $"A/ally={counts.SideAAdvantage:N0}, neutral={counts.Neutral:N0}, "
            + $"B/enemy={counts.SideBAdvantage:N0}, "
            + $"insufficient={counts.InsufficientEvidence:N0}; "
            + $"researchMatch={matchesResearch}, reverseSymmetry={symmetryValid}");
        return counts;
    }

    private static OutcomeAdvantageLevel Reverse(OutcomeAdvantageLevel level) => level switch
    {
        OutcomeAdvantageLevel.AllyAdvantage => OutcomeAdvantageLevel.EnemyAdvantage,
        OutcomeAdvantageLevel.EnemyAdvantage => OutcomeAdvantageLevel.AllyAdvantage,
        _ => level
    };
}
