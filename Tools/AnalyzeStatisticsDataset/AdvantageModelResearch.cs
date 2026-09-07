using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

internal static class AdvantageModelResearch
{
    private static readonly string[] Roles = ["TOP", "JUNGLE", "MID", "ADC", "SUPPORT"];
    private const string RecommendedModelId = "C_w80_n20_e5";

    public static AdvantageResearchReport Analyze(
        StatisticsDataset dataset,
        long? datasetRevision,
        StatisticsMatchupEntry[] entries)
    {
        var modelA = CreateModelA().Select(model => Evaluate(model, entries)).ToArray();
        var modelB = CreateModelB().Select(model => Evaluate(model, entries)).ToArray();
        var modelC = CreateModelC().Select(model => Evaluate(model, entries)).ToArray();
        var adaptive = CreateAdaptiveModels().Select(model => Evaluate(model, entries)).ToArray();
        var all = modelA.Concat(modelB).Concat(modelC).Concat(adaptive)
            .ToDictionary(result => result.Model.Id, StringComparer.Ordinal);
        var shortlistIds = new[]
        {
            "A_n20_e5",
            "C_w80_n10_e5",
            "C_w80_n20_e5",
            "C_w90_n20_e5",
            "D_Balanced"
        };
        var shortlist = shortlistIds.Select(id => all[id]).ToArray();
        var recommended = all[RecommendedModelId];
        var outcomes = ClassifyAll(recommended.Model, entries);

        return new AdvantageResearchReport(
            new DatasetResearchSummary(
                dataset.Patch,
                dataset.AcceptedMatches,
                dataset.Observations,
                entries.Length,
                dataset.TierScope,
                dataset.TierScopeMethod,
                dataset.AllParticipantsTierVerified,
                dataset.Region,
                dataset.QueueId,
                dataset.MinimumGameDurationSeconds,
                datasetRevision),
            "Outcome Advantage describes a final-game win-rate tendency, not a guaranteed lane or game result.",
            modelA,
            modelB,
            modelC,
            adaptive,
            shortlist,
            new RecommendationSummary(
                RecommendedModelId,
                "Minimum sample 20; |win rate - 50%| at least 5 percentage points; "
                    + "80% Wilson interval must exclude 50%; no reliability-specific exception.",
                "우세 경향 / 뚜렷한 차이 없음 / 열세 경향",
                recommended.Counts,
                recommended.EligibleKeyRate,
                recommended.DirectionalKeyCoverage,
                recommended.SampleWeightedDirectionalCoverage,
                [
                    "Protects all n<20 matchups from an outcome judgment.",
                    "Requires both statistical evidence and a user-visible practical effect.",
                    "Keeps one symmetric, explainable rule across every role.",
                    "Uses 80% rather than 95% evidence to avoid making the current sparse dataset almost entirely unclassified."
                ]),
            CreateBoundaryExamples(shortlist, entries),
            StrongestExamples(outcomes, AdvantageOutcome.SideAAdvantage),
            StrongestExamples(outcomes, AdvantageOutcome.SideBAdvantage),
            NeutralExamples(outcomes, entries),
            [
                CompareModels(all["A_n20_e5"], all["C_w90_n20_e5"], entries),
                CompareModels(all["B_w80_n1"], all["B_w95_n1"], entries)
            ],
            MetricDirectionDiagnostic(outcomes));
    }

    public static string BuildMarkdown(AdvantageResearchReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Outcome Advantage Model Research").AppendLine();
        sb.AppendLine("## Dataset Summary").AppendLine();
        sb.AppendLine($"- Patch: `{report.Dataset.Patch}`");
        sb.AppendLine($"- Accepted matches: {report.Dataset.AcceptedMatches:N0}");
        sb.AppendLine($"- Observations: {report.Dataset.Observations:N0}");
        sb.AppendLine($"- Matchup keys: {report.Dataset.MatchupKeys:N0}");
        sb.AppendLine($"- Scope: `{report.Dataset.TierScope}` / `{report.Dataset.TierScopeMethod}`");
        sb.AppendLine($"- All participants tier verified: `{report.Dataset.AllParticipantsTierVerified.ToString().ToLowerInvariant()}`");
        sb.AppendLine($"- Queue: {report.Dataset.QueueId}; minimum duration: {report.Dataset.MinimumGameDurationSeconds}s");
        sb.AppendLine($"- Dataset revision: {report.Dataset.DatasetRevision?.ToString() ?? "not available"}").AppendLine();

        sb.AppendLine("## Research Goal").AppendLine();
        sb.AppendLine(report.ResearchGoal).AppendLine();
        sb.AppendLine("First-level and solo-kill metrics are excluded from every outcome model. They appear only in a later exploratory direction diagnostic.").AppendLine();

        sb.AppendLine("## Model Definitions").AppendLine();
        sb.AppendLine("- Model A: minimum sample plus an absolute raw win-rate deviation from 50%.");
        sb.AppendLine("- Model B: Wilson interval excludes 50%, with a practical minimum-sample option.");
        sb.AppendLine("- Model C: minimum sample, practical win-rate effect, and Wilson evidence must all pass.");
        sb.AppendLine("- Model D: Stage 9-1 reliability bands choose different raw practical effects; Poor is always insufficient.").AppendLine();

        sb.AppendLine("## Parameter Sweep").AppendLine();
        AppendSweep(sb, "Model A", report.ModelA);
        AppendSweep(sb, "Model B", report.ModelB);
        AppendSweep(sb, "Model C", report.ModelC);
        AppendSweep(sb, "Reliability-aware Model D", report.ReliabilityAware);

        sb.AppendLine("## Coverage Comparison").AppendLine();
        AppendCandidateTable(sb, report.CandidateShortlist);

        var recommended = report.CandidateShortlist.First(item =>
            item.Model.Id == report.Recommendation.ModelId);
        sb.AppendLine("## Reliability Group Comparison").AppendLine();
        AppendBreakdownTable(sb, recommended.ReliabilityBreakdown);
        sb.AppendLine("## Role Comparison").AppendLine();
        AppendBreakdownTable(sb, recommended.RoleBreakdown);

        sb.AppendLine("## Effect Size Analysis").AppendLine();
        sb.AppendLine($"Recommended directional effects: min {recommended.DirectionalAbsoluteEffect.Min:P2}, median {recommended.DirectionalAbsoluteEffect.Median:P2}, p75 {recommended.DirectionalAbsoluteEffect.P75:P2}, p90 {recommended.DirectionalAbsoluteEffect.P90:P2}, max {recommended.DirectionalAbsoluteEffect.Max:P2}.");
        sb.AppendLine($"Recommended directional samples: min {recommended.DirectionalSampleSize.Min:N0}, p25 {recommended.DirectionalSampleSize.P25:N1}, median {recommended.DirectionalSampleSize.Median:N1}, p75 {recommended.DirectionalSampleSize.P75:N1}, p90 {recommended.DirectionalSampleSize.P90:N1}, max {recommended.DirectionalSampleSize.Max:N0}.").AppendLine();

        sb.AppendLine("## Small Sample Risk").AppendLine();
        sb.AppendLine("| Model | Directional n<5 | Directional n<10 | Directional n<20 | 0/100% at n=1-4 classified |");
        sb.AppendLine("|---|---:|---:|---:|---:|");
        foreach (var item in report.CandidateShortlist)
        {
            sb.AppendLine($"| {item.Model.Id} | {item.SmallSampleRisk.DirectionalBelow5:N0} | {item.SmallSampleRisk.DirectionalBelow10:N0} | {item.SmallSampleRisk.DirectionalBelow20:N0} | {item.SmallSampleRisk.ExtremeZeroOrOneBelow5Directional:N0} |");
        }
        sb.AppendLine();

        sb.AppendLine("Recommended-model treatment of observed extreme win rates:").AppendLine();
        sb.AppendLine("| Group | Total | A | Neutral | B | Insufficient |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|");
        foreach (var item in recommended.ExtremeTreatment)
        {
            sb.AppendLine($"| {item.Group} | {item.Counts.Total:N0} | {item.Counts.SideAAdvantage:N0} | {item.Counts.Neutral:N0} | {item.Counts.SideBAdvantage:N0} | {item.Counts.InsufficientEvidence:N0} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Wilson Confidence Comparison").AppendLine();
        AppendCandidateTable(sb, report.ModelB.Where(item => item.Model.MinimumSample == 20).ToArray());
        sb.AppendLine("A higher confidence level reduces directional coverage. This report does not treat 95% as automatically optimal for a user-facing reference tool.").AppendLine();

        sb.AppendLine("## Borderline Examples").AppendLine();
        AppendJson(sb, report.BoundaryExamples);
        sb.AppendLine("## Model Disagreement Examples").AppendLine();
        AppendJson(sb, report.Disagreements);
        sb.AppendLine("## Strong Directional Examples").AppendLine();
        sb.AppendLine("A-side:").AppendLine();
        AppendExampleTable(sb, report.StrongestAExamples);
        sb.AppendLine("B-side:").AppendLine();
        AppendExampleTable(sb, report.StrongestBExamples);
        sb.AppendLine("## Neutral Examples").AppendLine();
        AppendExampleTable(sb, report.NeutralExamples);

        sb.AppendLine("## Lane Metric Direction Diagnostic").AppendLine();
        sb.AppendLine("Exploratory only; these metrics are not inputs to the outcome rule.").AppendLine();
        sb.AppendLine("| Metric | Same direction | Opposite | Neutral | Agreement among non-neutral |");
        sb.AppendLine("|---|---:|---:|---:|---:|");
        foreach (var metric in report.LaneMetricDiagnostic.Metrics)
        {
            sb.AppendLine($"| {metric.Metric} | {metric.SameDirection:N0} | {metric.OppositeDirection:N0} | {metric.Neutral:N0} | {metric.AgreementAmongNonNeutral:P2} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Candidate Shortlist").AppendLine();
        AppendCandidateTable(sb, report.CandidateShortlist);
        sb.AppendLine("## Recommended Production Rule").AppendLine();
        sb.AppendLine($"- Model: `{report.Recommendation.ModelId}`");
        sb.AppendLine($"- Rule: {report.Recommendation.Rule}");
        sb.AppendLine($"- Suggested wording: {report.Recommendation.SuggestedTerminology}");
        sb.AppendLine($"- Directional key coverage: {recommended.DirectionalKeyCoverage:P2} ({recommended.Counts.Directional:N0} keys)");
        sb.AppendLine($"- Directional sample-weighted coverage: {recommended.SampleWeightedDirectionalCoverage:P2}");
        sb.AppendLine($"- Directional median sample/effect: {recommended.DirectionalSampleSize.Median:N1} / {recommended.DirectionalAbsoluteEffect.Median:P2}");
        sb.AppendLine($"- Directional classifications below n=20: {recommended.SmallSampleRisk.DirectionalBelow20:N0}");
        foreach (var reason in report.Recommendation.Reasons) sb.AppendLine($"- {reason}");
        sb.AppendLine();

        sb.AppendLine("## Limitations").AppendLine();
        sb.AppendLine("- This is an in-sample coverage and uncertainty study, not a future-match prediction accuracy test.");
        sb.AppendLine("- MASTER_PLUS_SEEDED means seed-player tier at collection; all ten participants were not tier-verified.");
        sb.AppendLine("- Results apply only to the loaded patch and duration policy.");
        sb.AppendLine("- Key-weighted and sample-weighted coverage are descriptive proxies, not literal user exposure probability.");
        sb.AppendLine("- Correlation or directional agreement with lane metrics does not establish causation.").AppendLine();

        sb.AppendLine("## Next Stage Recommendation").AppendLine();
        sb.AppendLine("Stage 9-2B should implement the recommended symmetric rule as a separate production outcome classifier while preserving Stage 9-1 reliability, raw statistics, Neutral, and InsufficientEvidence as distinct concepts. No production classifier or UI was changed by this research.");
        return sb.ToString();
    }

    public static JsonSerializerOptions JsonOptions { get; } = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private static IEnumerable<AdvantageModelDefinition> CreateModelA()
    {
        foreach (var minimum in new[] { 5, 10, 20, 30, 50, 75, 100 })
        foreach (var effect in new[] { .025, .05, .075, .10 })
            yield return new AdvantageModelDefinition(
                $"A_n{minimum}_e{EffectId(effect)}", "A_RawThreshold", minimum, effect);
    }

    private static IEnumerable<AdvantageModelDefinition> CreateModelB()
    {
        foreach (var confidence in new[] { .80, .90, .95 })
        foreach (var minimum in new[] { 1, 5, 10, 20, 30, 50 })
            yield return new AdvantageModelDefinition(
                $"B_w{confidence * 100:F0}_n{minimum}", "B_WilsonOnly", minimum,
                ConfidenceLevel: confidence);
    }

    private static IEnumerable<AdvantageModelDefinition> CreateModelC()
    {
        foreach (var confidence in new[] { .80, .90, .95 })
        foreach (var minimum in new[] { 10, 20, 30, 50 })
        foreach (var effect in new[] { .025, .05, .075 })
            yield return new AdvantageModelDefinition(
                $"C_w{confidence * 100:F0}_n{minimum}_e{EffectId(effect)}",
                "C_PracticalEffectWilson", minimum, effect, confidence);
    }

    private static IEnumerable<AdvantageModelDefinition> CreateAdaptiveModels()
    {
        yield return Adaptive("Conservative", .15, .10, .05);
        yield return Adaptive("Balanced", .125, .075, .05);
        yield return Adaptive("Permissive", .10, .05, .025);
    }

    private static AdvantageModelDefinition Adaptive(
        string name,
        double referenceEffect,
        double moderateEffect,
        double strongEffect) => new(
            $"D_{name}",
            "D_ReliabilityAware",
            5,
            AdaptiveThresholds:
            [
                new("Poor", 1, 4, null),
                new("Reference", 5, 19, referenceEffect),
                new("Moderate", 20, 49, moderateEffect),
                new("RelativelyStrong", 50, null, strongEffect)
            ]);

    private static string EffectId(double effect) =>
        (effect * 100).ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)
            .Replace('.', '_');

    private static ModelEvaluation Evaluate(
        AdvantageModelDefinition model,
        StatisticsMatchupEntry[] entries)
    {
        var outcomes = ClassifyAll(model, entries);
        var counts = Count(outcomes.Select(item => item.Outcome));
        var directional = outcomes.Where(item => IsDirectional(item.Outcome)).ToArray();
        var totalSamples = entries.Sum(item => (long)item.SampleSize);
        var directionalSamples = directional.Sum(item => (long)item.Entry.SampleSize);
        var roleBreakdown = Roles.Select(role => Breakdown(
            role,
            outcomes.Where(item => item.Entry.Role == role).ToArray())).ToArray();
        var reliabilityBreakdown = new[]
        {
            SampleReliabilityLevel.Poor,
            SampleReliabilityLevel.Reference,
            SampleReliabilityLevel.Moderate,
            SampleReliabilityLevel.RelativelyStrong
        }.Select(level => Breakdown(
            level.ToString(),
            outcomes.Where(item =>
                SampleReliabilityClassifier.Classify(item.Entry.SampleSize) == level).ToArray()))
            .ToArray();
        var smallRisk = new SmallSampleRisk(
            directional.Count(item => item.Entry.SampleSize < 5),
            directional.Count(item => item.Entry.SampleSize < 10),
            directional.Count(item => item.Entry.SampleSize < 20),
            directional.Count(item => item.Entry.SampleSize < 5
                && item.WinRate is 0 or 1));
        var symmetryValid = entries.All(entry =>
            Invert(Classify(model, entry.SampleSize - entry.WinsForA, entry.SampleSize))
            == Classify(model, entry.WinsForA, entry.SampleSize));

        return new ModelEvaluation(
            model,
            counts,
            (double)(counts.Total - counts.InsufficientEvidence) / counts.Total,
            (double)counts.Directional / counts.Total,
            totalSamples == 0 ? 0 : directionalSamples / (double)totalSamples,
            counts.Directional == 0 ? 0 : (double)counts.SideAAdvantage / counts.Directional,
            AnalysisMath.Describe(directional.Select(item =>
                Math.Abs(item.WinRate - .5))),
            AnalysisMath.Describe(directional.Select(item =>
                (double)item.Entry.SampleSize)),
            smallRisk,
            ExtremeTreatmentSummary(outcomes),
            roleBreakdown,
            reliabilityBreakdown,
            symmetryValid);
    }

    private static EntryOutcome[] ClassifyAll(
        AdvantageModelDefinition model,
        StatisticsMatchupEntry[] entries) => entries.Select(entry => new EntryOutcome(
            entry,
            (double)entry.WinsForA / entry.SampleSize,
            Classify(model, entry.WinsForA, entry.SampleSize))).ToArray();

    private static AdvantageOutcome Classify(
        AdvantageModelDefinition model,
        int winsForA,
        int sampleSize)
    {
        if (sampleSize < model.MinimumSample) return AdvantageOutcome.InsufficientEvidence;
        var winRate = (double)winsForA / sampleSize;
        var effect = Math.Abs(winRate - .5);

        if (model.AdaptiveThresholds is { Count: > 0 })
        {
            var band = model.AdaptiveThresholds.First(item =>
                sampleSize >= item.MinimumSample
                && (item.MaximumSample is null || sampleSize <= item.MaximumSample));
            if (band.PracticalEffect is null) return AdvantageOutcome.InsufficientEvidence;
            return RawOutcome(winRate, band.PracticalEffect.Value);
        }

        if (model.Family == "A_RawThreshold")
            return RawOutcome(winRate, model.PracticalEffect!.Value);

        var interval = AnalysisMath.Wilson(
            winsForA,
            sampleSize,
            model.ConfidenceLevel!.Value);
        var wilsonOutcome = interval.Lower > .5
            ? AdvantageOutcome.SideAAdvantage
            : interval.Upper < .5
                ? AdvantageOutcome.SideBAdvantage
                : AdvantageOutcome.Neutral;
        if (model.Family == "B_WilsonOnly") return wilsonOutcome;

        return effect >= model.PracticalEffect!.Value
            ? wilsonOutcome
            : AdvantageOutcome.Neutral;
    }

    private static AdvantageOutcome RawOutcome(double winRate, double effect) =>
        winRate >= .5 + effect
            ? AdvantageOutcome.SideAAdvantage
            : winRate <= .5 - effect
                ? AdvantageOutcome.SideBAdvantage
                : AdvantageOutcome.Neutral;

    private static AdvantageOutcome Invert(AdvantageOutcome outcome) => outcome switch
    {
        AdvantageOutcome.SideAAdvantage => AdvantageOutcome.SideBAdvantage,
        AdvantageOutcome.SideBAdvantage => AdvantageOutcome.SideAAdvantage,
        _ => outcome
    };

    private static bool IsDirectional(AdvantageOutcome outcome) =>
        outcome is AdvantageOutcome.SideAAdvantage or AdvantageOutcome.SideBAdvantage;

    private static OutcomeCounts Count(IEnumerable<AdvantageOutcome> outcomes)
    {
        var values = outcomes.ToArray();
        return new OutcomeCounts(
            values.Length,
            values.Count(value => value == AdvantageOutcome.SideAAdvantage),
            values.Count(value => value == AdvantageOutcome.SideBAdvantage),
            values.Count(value => value == AdvantageOutcome.Neutral),
            values.Count(value => value == AdvantageOutcome.InsufficientEvidence));
    }

    private static BreakdownSummary Breakdown(string name, EntryOutcome[] outcomes)
    {
        var counts = Count(outcomes.Select(item => item.Outcome));
        var total = counts.Total;
        return new BreakdownSummary(
            name,
            counts,
            total == 0 ? 0 : (double)counts.Directional / total,
            total == 0 ? 0 : (double)counts.Neutral / total,
            total == 0 ? 0 : (double)counts.InsufficientEvidence / total);
    }

    private static ExtremeTreatment[] ExtremeTreatmentSummary(EntryOutcome[] outcomes)
    {
        ExtremeTreatment Group(string name, Func<EntryOutcome, bool> predicate)
        {
            var values = outcomes.Where(predicate).ToArray();
            return new ExtremeTreatment(name, Count(values.Select(item => item.Outcome)));
        }

        return
        [
            Group("WinRate0", item => item.WinRate == 0),
            Group("WinRate100", item => item.WinRate == 1),
            Group("WinRateAtMost30", item => item.WinRate <= .3),
            Group("WinRateAtLeast70", item => item.WinRate >= .7),
            Group("Sample1To4WinRate0Or100", item =>
                item.Entry.SampleSize <= 4 && item.WinRate is 0 or 1)
        ];
    }

    private static BoundaryExampleSet[] CreateBoundaryExamples(
        ModelEvaluation[] models,
        StatisticsMatchupEntry[] entries) => models.Select(model =>
    {
        var outcomes = ClassifyAll(model.Model, entries);
        var directional = outcomes.Where(item => IsDirectional(item.Outcome))
            .OrderBy(item => Math.Abs(item.WinRate - .5))
            .ThenBy(item => item.Entry.SampleSize)
            .ThenBy(StableEntry)
            .Take(3).Select(ToExample).ToArray();
        var neutral = outcomes.Where(item => item.Outcome == AdvantageOutcome.Neutral)
            .OrderByDescending(item => Math.Abs(item.WinRate - .5))
            .ThenBy(item => item.Entry.SampleSize)
            .ThenBy(StableEntry)
            .Take(3).Select(ToExample).ToArray();
        return new BoundaryExampleSet(model.Model.Id, directional, neutral);
    }).ToArray();

    private static AdvantageExample[] StrongestExamples(
        EntryOutcome[] outcomes,
        AdvantageOutcome desired) => outcomes
            .Where(item => item.Outcome == desired && item.Entry.SampleSize >= 50)
            .OrderBy(item => desired == AdvantageOutcome.SideAAdvantage
                ? -item.WinRate
                : item.WinRate)
            .ThenByDescending(item => item.Entry.SampleSize)
            .ThenBy(StableEntry)
            .Take(10)
            .Select(ToExample)
            .ToArray();

    private static AdvantageExample[] NeutralExamples(
        EntryOutcome[] outcomes,
        StatisticsMatchupEntry[] entries) => outcomes
            .Where(item => item.Outcome == AdvantageOutcome.Neutral
                && item.Entry.SampleSize >= 50)
            .OrderBy(item => Math.Abs(item.WinRate - .5))
            .ThenByDescending(item => item.Entry.SampleSize)
            .ThenBy(StableEntry)
            .Take(10)
            .Select(ToExample)
            .ToArray();

    private static DisagreementSet CompareModels(
        ModelEvaluation first,
        ModelEvaluation second,
        StatisticsMatchupEntry[] entries)
    {
        var examples = entries.Select(entry =>
        {
            var firstOutcome = Classify(first.Model, entry.WinsForA, entry.SampleSize);
            var secondOutcome = Classify(second.Model, entry.WinsForA, entry.SampleSize);
            return new
            {
                Entry = entry,
                First = firstOutcome,
                Second = secondOutcome,
                WinRate = (double)entry.WinsForA / entry.SampleSize
            };
        }).Where(item => item.First != item.Second)
            .OrderByDescending(item => item.Entry.SampleSize)
            .ThenBy(item => Math.Abs(item.WinRate - .5))
            .ThenBy(item => item.Entry.Role, StringComparer.Ordinal)
            .ThenBy(item => item.Entry.ChampionA, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Entry.ChampionB, StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .Select(item => new DisagreementExample(
                ToExample(new EntryOutcome(item.Entry, item.WinRate, item.First)),
                item.Second))
            .ToArray();
        return new DisagreementSet(first.Model.Id, second.Model.Id, examples);
    }

    private static LaneMetricDiagnostic MetricDirectionDiagnostic(EntryOutcome[] outcomes)
    {
        var directional = outcomes.Where(item => IsDirectional(item.Outcome)).ToArray();
        MetricDirectionSummary Metric(
            string name,
            Func<StatisticsMatchupEntry, double> centeredValue)
        {
            var same = 0;
            var opposite = 0;
            var neutral = 0;
            foreach (var item in directional)
            {
                var value = centeredValue(item.Entry);
                if (value == 0) neutral++;
                else if ((item.Outcome == AdvantageOutcome.SideAAdvantage && value > 0)
                         || (item.Outcome == AdvantageOutcome.SideBAdvantage && value < 0)) same++;
                else opposite++;
            }

            return new MetricDirectionSummary(
                name,
                same,
                opposite,
                neutral,
                same + opposite == 0 ? 0 : (double)same / (same + opposite));
        }

        return new LaneMetricDiagnostic(
            RecommendedModelId,
            directional.Length,
            [
                Metric("FirstLevel2", item =>
                    (double)item.FirstLevel2ForA / item.SampleSize - .5),
                Metric("FirstLevel3", item =>
                    (double)item.FirstLevel3ForA / item.SampleSize - .5),
                Metric("FirstLevel6", item =>
                    (double)item.FirstLevel6ForA / item.SampleSize - .5),
                Metric("SoloKillDifference", item =>
                    (double)(item.SoloKillsBefore15ForA
                        - item.SoloKillsBefore15ForB) / item.SampleSize)
            ]);
    }

    private static AdvantageExample ToExample(EntryOutcome item)
    {
        var interval = AnalysisMath.Wilson(
            item.Entry.WinsForA,
            item.Entry.SampleSize,
            .95);
        return new AdvantageExample(
            item.Entry.Role,
            item.Entry.ChampionA,
            item.Entry.ChampionB,
            item.Entry.SampleSize,
            item.WinRate,
            SampleReliabilityClassifier.Classify(item.Entry.SampleSize),
            interval.Lower,
            interval.Upper,
            item.Outcome);
    }

    private static string StableEntry(EntryOutcome item) =>
        $"{item.Entry.Role}|{item.Entry.ChampionA}|{item.Entry.ChampionB}";

    private static void AppendSweep(
        StringBuilder sb,
        string title,
        IReadOnlyList<ModelEvaluation> results)
    {
        sb.AppendLine($"### {title}").AppendLine();
        sb.AppendLine("| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var item in results)
        {
            sb.AppendLine($"| {item.Model.Id} | {item.Counts.SideAAdvantage:N0} | {item.Counts.SideBAdvantage:N0} | {item.Counts.Neutral:N0} | {item.Counts.InsufficientEvidence:N0} | {item.EligibleKeyRate:P2} | {item.DirectionalKeyCoverage:P2} | {item.SampleWeightedDirectionalCoverage:P2} | {item.DirectionalSampleSize.Median:N1} | {item.DirectionalAbsoluteEffect.Median:P2} | {item.SmallSampleRisk.DirectionalBelow20:N0} |");
        }
        sb.AppendLine();
    }

    private static void AppendCandidateTable(
        StringBuilder sb,
        IReadOnlyList<ModelEvaluation> results)
    {
        sb.AppendLine("| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var item in results)
        {
            sb.AppendLine($"| {item.Model.Id} | {item.Counts.SideAAdvantage:N0} | {item.Counts.Neutral:N0} | {item.Counts.SideBAdvantage:N0} | {item.Counts.InsufficientEvidence:N0} | {item.DirectionalKeyCoverage:P2} | {item.SampleWeightedDirectionalCoverage:P2} | {item.DirectionalSampleSize.Median:N1} | {item.DirectionalAbsoluteEffect.Median:P2} | {item.SideAShareOfDirectional:P2} | {item.SymmetryValid} |");
        }
        sb.AppendLine();
    }

    private static void AppendBreakdownTable(
        StringBuilder sb,
        IReadOnlyList<BreakdownSummary> values)
    {
        sb.AppendLine("| Group | Total | A | Neutral | B | Insufficient | Directional | Neutral rate | Insufficient rate |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var value in values)
        {
            sb.AppendLine($"| {value.Name} | {value.Counts.Total:N0} | {value.Counts.SideAAdvantage:N0} | {value.Counts.Neutral:N0} | {value.Counts.SideBAdvantage:N0} | {value.Counts.InsufficientEvidence:N0} | {value.DirectionalRate:P2} | {value.NeutralRate:P2} | {value.InsufficientRate:P2} |");
        }
        sb.AppendLine();
    }

    private static void AppendExampleTable(
        StringBuilder sb,
        IReadOnlyList<AdvantageExample> values)
    {
        sb.AppendLine("| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |");
        sb.AppendLine("|---|---|---:|---:|---|---:|---|");
        foreach (var value in values)
        {
            sb.AppendLine($"| {value.Role} | {value.ChampionA} vs {value.ChampionB} | {value.SampleSize:N0} | {value.WinRateA:P2} | {value.Reliability} | {value.Wilson95Lower:P2}–{value.Wilson95Upper:P2} | {value.Result} |");
        }
        sb.AppendLine();
    }

    private static void AppendJson(StringBuilder sb, object value) =>
        sb.AppendLine("```json")
            .AppendLine(JsonSerializer.Serialize(value, JsonOptions))
            .AppendLine("```")
            .AppendLine();
}

internal enum AdvantageOutcome
{
    SideAAdvantage,
    Neutral,
    SideBAdvantage,
    InsufficientEvidence
}

internal sealed record AdaptiveThreshold(
    string Reliability,
    int MinimumSample,
    int? MaximumSample,
    double? PracticalEffect);

internal sealed record AdvantageModelDefinition(
    string Id,
    string Family,
    int MinimumSample,
    double? PracticalEffect = null,
    double? ConfidenceLevel = null,
    IReadOnlyList<AdaptiveThreshold>? AdaptiveThresholds = null);

internal sealed record OutcomeCounts(
    int Total,
    int SideAAdvantage,
    int SideBAdvantage,
    int Neutral,
    int InsufficientEvidence)
{
    public int Directional => SideAAdvantage + SideBAdvantage;
}

internal sealed record BreakdownSummary(
    string Name,
    OutcomeCounts Counts,
    double DirectionalRate,
    double NeutralRate,
    double InsufficientRate);

internal sealed record SmallSampleRisk(
    int DirectionalBelow5,
    int DirectionalBelow10,
    int DirectionalBelow20,
    int ExtremeZeroOrOneBelow5Directional);

internal sealed record ExtremeTreatment(string Group, OutcomeCounts Counts);

internal sealed record ModelEvaluation(
    AdvantageModelDefinition Model,
    OutcomeCounts Counts,
    double EligibleKeyRate,
    double DirectionalKeyCoverage,
    double SampleWeightedDirectionalCoverage,
    double SideAShareOfDirectional,
    Distribution DirectionalAbsoluteEffect,
    Distribution DirectionalSampleSize,
    SmallSampleRisk SmallSampleRisk,
    IReadOnlyList<ExtremeTreatment> ExtremeTreatment,
    IReadOnlyList<BreakdownSummary> RoleBreakdown,
    IReadOnlyList<BreakdownSummary> ReliabilityBreakdown,
    bool SymmetryValid);

internal sealed record DatasetResearchSummary(
    string Patch,
    int AcceptedMatches,
    long Observations,
    int MatchupKeys,
    string TierScope,
    string TierScopeMethod,
    bool AllParticipantsTierVerified,
    string Region,
    int QueueId,
    int? MinimumGameDurationSeconds,
    long? DatasetRevision);

internal sealed record RecommendationSummary(
    string ModelId,
    string Rule,
    string SuggestedTerminology,
    OutcomeCounts Counts,
    double EligibleKeyRate,
    double DirectionalKeyCoverage,
    double SampleWeightedDirectionalCoverage,
    IReadOnlyList<string> Reasons);

internal sealed record AdvantageExample(
    string Role,
    string ChampionA,
    string ChampionB,
    int SampleSize,
    double WinRateA,
    SampleReliabilityLevel Reliability,
    double Wilson95Lower,
    double Wilson95Upper,
    AdvantageOutcome Result);

internal sealed record BoundaryExampleSet(
    string ModelId,
    IReadOnlyList<AdvantageExample> DirectionalSmallestEffects,
    IReadOnlyList<AdvantageExample> NeutralLargestEffects);

internal sealed record DisagreementExample(
    AdvantageExample FirstModelResult,
    AdvantageOutcome SecondModelResult);

internal sealed record DisagreementSet(
    string FirstModelId,
    string SecondModelId,
    IReadOnlyList<DisagreementExample> Examples);

internal sealed record MetricDirectionSummary(
    string Metric,
    int SameDirection,
    int OppositeDirection,
    int Neutral,
    double AgreementAmongNonNeutral);

internal sealed record LaneMetricDiagnostic(
    string ModelId,
    int DirectionalMatchups,
    IReadOnlyList<MetricDirectionSummary> Metrics);

internal sealed record AdvantageResearchReport(
    DatasetResearchSummary Dataset,
    string ResearchGoal,
    IReadOnlyList<ModelEvaluation> ModelA,
    IReadOnlyList<ModelEvaluation> ModelB,
    IReadOnlyList<ModelEvaluation> ModelC,
    IReadOnlyList<ModelEvaluation> ReliabilityAware,
    IReadOnlyList<ModelEvaluation> CandidateShortlist,
    RecommendationSummary Recommendation,
    IReadOnlyList<BoundaryExampleSet> BoundaryExamples,
    IReadOnlyList<AdvantageExample> StrongestAExamples,
    IReadOnlyList<AdvantageExample> StrongestBExamples,
    IReadOnlyList<AdvantageExample> NeutralExamples,
    IReadOnlyList<DisagreementSet> Disagreements,
    LaneMetricDiagnostic LaneMetricDiagnostic);

internal sealed record EntryOutcome(
    StatisticsMatchupEntry Entry,
    double WinRate,
    AdvantageOutcome Outcome);
