using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

internal static class LanePressureModelResearch
{
    private static readonly string[] Roles = ["TOP", "JUNGLE", "MID", "ADC", "SUPPORT"];
    private static readonly double[] LevelThresholds = [0, .025, .05, .075, .10];
    private static readonly double[] SoloThresholds = [0, .05, .10, .15, .20, .30];
    private static readonly int[] MinimumSamples = [5, 10, 20, 30, 50];
    private static readonly string[] BucketLabels = ["1-4", "5-9", "10-19", "20-49", "50-99", "100+"];

    public static JsonSerializerOptions JsonOptions { get; } = CreateJsonOptions();

    public static LanePressureResearchReport Analyze(
        StatisticsDataset dataset,
        long? datasetRevision,
        StatisticsMatchupEntry[] entries)
    {
        var metrics = entries.Select(ToMetrics).ToArray();
        var distributions = MetricDistributions("ALL", metrics);
        var roleDistributions = Roles.Select(role =>
            MetricDistributions(role, metrics.Where(x => x.Entry.Role == role).ToArray())).ToArray();
        var volatility = BucketLabels.Select(label =>
            MetricDistributions(label, metrics.Where(x => Bucket(x.Entry.SampleSize) == label).ToArray())).ToArray();

        var majority = Sweep("A", metrics, (x, l, s) => Majority(
            Signal(x.L2, l), Signal(x.L3, l), Signal(x.L6, l), Signal(x.Solo, s)), true);
        var levelOnly = Sweep("B", metrics, (x, l, _) => LevelMajority(x, l), false);
        var decorrelated = Sweep("D", metrics, (x, l, s) => Majority(
            EarlySignal(x, l), Signal(x.L6, l), Signal(x.Solo, s)), true);
        var split = CreateSplitEvaluations(metrics);

        var progression = new[] { 20, 50 }.SelectMany(minimum =>
            new[] { .025, .05, .075 }.Select(threshold =>
                Progression(minimum, threshold, metrics))).ToArray();
        var agreement = new[] { 20, 50 }.SelectMany(minimum =>
            new[] { .025, .05, .075 }.SelectMany(level =>
                new[] { .05, .10, .20 }.Select(solo =>
                    Agreement(minimum, level, solo, metrics)))).ToArray();
        var signalThresholdCoverage = CreateSignalThresholdCoverage(metrics);

        var byId = majority.Concat(levelOnly).Concat(decorrelated)
            .ToDictionary(x => x.Definition.Id, StringComparer.Ordinal);
        var shortlist = new[]
        {
            byId["A_n20_l5_s10"],
            byId["B_n20_l5"],
            byId["D_n20_l5_s10"],
            byId["D_n20_l7_5_s15"]
        };
        var recommendedSplit = split.Single(x =>
            x.MinimumSample == 20 && x.LevelThreshold == .05 && x.SoloThreshold == .10);
        var recommendedLevel = byId["B_n20_l5"];
        var recommendedCombined = byId["D_n20_l5_s10"];
        var examples = CreateExamples(metrics, recommendedCombined, recommendedSplit);
        var outcomeDiagnostic = OutcomeDiagnostic(metrics, recommendedCombined);
        var highSample = new[] { 20, 50, 100 }.Select(minimum =>
            SubsetSummary(minimum, recommendedCombined.Definition, metrics)).ToArray();
        var sensitivity = CreateSensitivity(metrics);

        var splitRecommendation = recommendedSplit.ConflictAmongBothDirectional > .20
            || recommendedSplit.OneNeutralRate > .35;
        var decision = splitRecommendation
            ? "B_LEVEL_TEMPO_AND_COMBAT_PRESSURE_SPLIT"
            : "A_SINGLE_LANE_PRESSURE_CLASSIFIER";

        return new LanePressureResearchReport(
            new LaneDatasetSummary(dataset.Patch, dataset.AcceptedMatches, dataset.Observations,
                entries.Length, dataset.TierScope, dataset.Region, dataset.QueueId,
                dataset.MinimumGameDurationSeconds, datasetRevision),
            "Research whether first-level timing and direct-opponent solo-kill frequency describe one coherent early-pressure concept.",
            new MetricDefinitionSummary(
                "(FirstLevelForA - FirstLevelForB) / SampleSize; Unknown remains in the SampleSize denominator.",
                "SoloKillsBefore15ForA / SampleSize - SoloKillsBefore15ForB / SampleSize; EVENT_COUNT per match, not probability.",
                "Game wins and OutcomeAdvantage are excluded from every lane-pressure model input."),
            distributions, roleDistributions, volatility, progression, agreement,
            signalThresholdCoverage,
            majority, levelOnly, decorrelated, split, shortlist, highSample, sensitivity,
            examples, outcomeDiagnostic,
            new LaneRecommendation(
                decision,
                "B_n20_l5",
                .10,
                20,
                .05,
                [
                    "L2 and L3 are correlated and should not be counted as fully independent votes.",
                    "Solo kills are event frequency and represent combat pressure rather than level tempo.",
                    "Keeping Level Tempo and Combat Pressure separate preserves observed disagreement instead of hiding it in one score.",
                    "Use categorical tendencies with Neutral and InsufficientEvidence; do not create an arbitrary numeric score."
                ],
                "레벨 템포 / 교전 압박",
                "경향 / 뚜렷한 차이 없음 / 판정 보류"),
            [
                "Aggregate data cannot provide a true train/test split or per-match stability test.",
                "Observed metrics omit CS, gold, XP differences, turret plates, objectives, roams, damage, assists, and third-party deaths.",
                "JUNGLE and SUPPORT are not always well described by the term lane pressure.",
                "Role-specific rules were not fitted; all roles use the same research candidates.",
                "Outcome agreement is diagnostic only and disagreement is not treated as model error."
            ]);
    }

    public static string BuildMarkdown(LanePressureResearchReport r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Lane Pressure Model Research").AppendLine();
        sb.AppendLine("## Dataset Summary").AppendLine()
            .AppendLine($"- Patch: `{r.Dataset.Patch}`")
            .AppendLine($"- Accepted matches: {r.Dataset.AcceptedMatches:N0}")
            .AppendLine($"- Observations: {r.Dataset.Observations:N0}")
            .AppendLine($"- Matchup keys: {r.Dataset.MatchupKeys:N0}")
            .AppendLine($"- Scope: `{r.Dataset.TierScope}`; region `{r.Dataset.Region}`; queue {r.Dataset.QueueId}")
            .AppendLine($"- Minimum duration: {r.Dataset.MinimumGameDurationSeconds}s; revision: {r.Dataset.DatasetRevision}").AppendLine();
        sb.AppendLine("## Research Scope").AppendLine().AppendLine(r.ResearchScope)
            .AppendLine("This is research only. No production Lane Pressure classifier or UI is implemented.").AppendLine();
        sb.AppendLine("## Metric Definitions").AppendLine()
            .AppendLine($"- Level differences: {r.Definitions.LevelDifference}")
            .AppendLine($"- Solo difference: {r.Definitions.SoloDifference}")
            .AppendLine($"- Outcome separation: {r.Definitions.OutcomeSeparation}").AppendLine();

        AppendDistribution(sb, "Metric Difference Distributions", r.MetricDistributions);
        sb.AppendLine("## Role Comparison").AppendLine();
        foreach (var role in r.RoleDistributions) AppendDistributionTable(sb, role);
        sb.AppendLine("## Sample-size Volatility").AppendLine();
        foreach (var bucket in r.SampleVolatility) AppendDistributionTable(sb, bucket);

        sb.AppendLine("## Level Progression Consistency").AppendLine();
        sb.AppendLine("| n | threshold | AAA | BBB | consistent | reversal | NNN | mixed/neutral |").AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var x in r.Progression) sb.AppendLine($"| {x.MinimumSample} | {x.Threshold:P1} | {x.AAA:N0} | {x.BBB:N0} | {x.ConsistentSameDirection:N0} | {x.Reversal:N0} | {x.NNN:N0} | {x.MixedOrNeutral:N0} |");
        sb.AppendLine();

        sb.AppendLine("## Metric Direction Agreement").AppendLine();
        sb.AppendLine("Agreement is exact categorical agreement (A/Neutral/B), reported separately from Pearson correlation.").AppendLine();
        sb.AppendLine("| n | level | solo | L2-L3 | L2-L6 | L3-L6 | L2-Solo | L3-Solo | L6-Solo | all same |").AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var x in r.Agreement) sb.AppendLine($"| {x.MinimumSample} | {x.LevelThreshold:P1} | {x.SoloThreshold:F2} | {x.L2L3:P2} | {x.L2L6:P2} | {x.L3L6:P2} | {x.L2Solo:P2} | {x.L3Solo:P2} | {x.L6Solo:P2} | {x.AllFourSame:N0} |");
        sb.AppendLine();

        sb.AppendLine("## Model Definitions").AppendLine()
            .AppendLine("- Model A: four-signal majority vote over L2/L3/L6/Solo.")
            .AppendLine("- Model B: Level Progression majority; at least two of L2/L3/L6 point to one side.")
            .AppendLine("- Model D: L2/L3 are combined into one EarlyLevel signal, then EarlyLevel/L6/Solo vote.")
            .AppendLine("- Model E: Level Tempo (Model B) and Combat Pressure (Solo) remain separate.")
            .AppendLine("- Model F standardized score was not promoted: patch-dependent scaling and arbitrary weighting reduce explainability.").AppendLine();
        AppendSweep(sb, "Parameter Sweep — Model A", r.ModelA);
        AppendSweep(sb, "Parameter Sweep — Model B", r.ModelB);
        AppendSweep(sb, "Parameter Sweep — Model D", r.ModelD);

        sb.AppendLine("## Coverage Comparison").AppendLine();
        AppendModelTable(sb, r.CandidateShortlist);
        sb.AppendLine("## Reliability Comparison").AppendLine();
        foreach (var model in r.CandidateShortlist)
        {
            sb.AppendLine($"### {model.Definition.Id}").AppendLine();
            AppendBreakdown(sb, model.ReliabilityBreakdown);
        }
        sb.AppendLine("## Role Coverage Comparison").AppendLine();
        foreach (var model in r.CandidateShortlist)
        {
            sb.AppendLine($"### {model.Definition.Id}").AppendLine();
            AppendBreakdown(sb, model.RoleBreakdown);
        }
        sb.AppendLine("## Threshold Sensitivity").AppendLine();
        sb.AppendLine("Individual metric signal coverage across the complete requested threshold range:").AppendLine();
        AppendJson(sb, r.SignalThresholdCoverage);
        sb.AppendLine("Combined de-correlated model sensitivity:").AppendLine();
        AppendJson(sb, r.ThresholdSensitivity);
        sb.AppendLine("## High-sample Subset Comparison").AppendLine();
        AppendJson(sb, r.HighSampleSubsets);
        sb.AppendLine("## Split Level Tempo / Combat Pressure Research").AppendLine();
        AppendJson(sb, r.SplitModels.Where(x => x.MinimumSample == 20).ToArray());
        sb.AppendLine("## Model Disagreement Cases").AppendLine();
        AppendJson(sb, r.Examples);
        sb.AppendLine("## Outcome Diagnostic Comparison").AppendLine()
            .AppendLine("Outcome is not an input. Opposite directions are valid observations, not errors.").AppendLine();
        AppendJson(sb, r.OutcomeDiagnostic);
        sb.AppendLine("## Candidate Shortlist").AppendLine();
        AppendModelTable(sb, r.CandidateShortlist);
        sb.AppendLine("## Single-vs-Split Recommendation").AppendLine()
            .AppendLine($"**{r.Recommendation.Decision}**").AppendLine();
        foreach (var reason in r.Recommendation.Reasons) sb.AppendLine($"- {reason}");
        sb.AppendLine();
        sb.AppendLine("## Recommended Production Rule(s)").AppendLine()
            .AppendLine($"- Minimum sample: {r.Recommendation.MinimumSample}")
            .AppendLine($"- Level Tempo: L2/L3/L6 threshold {r.Recommendation.LevelThreshold:P1}; at least two same-direction signals; otherwise Neutral.")
            .AppendLine($"- Combat Pressure: solo-kill event-rate difference threshold {r.Recommendation.CombatThreshold:F2} per match; otherwise Neutral.")
            .AppendLine("- Below minimum sample: InsufficientEvidence for both axes.")
            .AppendLine("- Do not combine the two outputs into one lane-advantage label.").AppendLine();
        sb.AppendLine("## Terminology Recommendation").AppendLine()
            .AppendLine($"- Axes: {r.Recommendation.Terms}")
            .AppendLine($"- States: {r.Recommendation.StateTerms}")
            .AppendLine("- For all-role UI, `초반 템포` is safer than claiming definitive `라인전 우세`, especially for JUNGLE and SUPPORT.").AppendLine();
        sb.AppendLine("## Limitations").AppendLine();
        foreach (var limitation in r.Limitations) sb.AppendLine($"- {limitation}");
        sb.AppendLine();
        sb.AppendLine("## Next Stage").AppendLine()
            .AppendLine("If production work proceeds, implement separate Level Tempo and Combat Pressure classifiers only after reviewing this report. No production code was changed by this research.");
        return sb.ToString();
    }

    private static EntryMetrics ToMetrics(StatisticsMatchupEntry e) => new(
        e,
        (double)(e.FirstLevel2ForA - e.FirstLevel2ForB) / e.SampleSize,
        (double)(e.FirstLevel3ForA - e.FirstLevel3ForB) / e.SampleSize,
        (double)(e.FirstLevel6ForA - e.FirstLevel6ForB) / e.SampleSize,
        (double)(e.SoloKillsBefore15ForA - e.SoloKillsBefore15ForB) / e.SampleSize,
        (double)(e.FirstLevel2Unknown + e.FirstLevel3Unknown + e.FirstLevel6Unknown)
            / (3 * e.SampleSize));

    private static MetricDistributionGroup MetricDistributions(string group, EntryMetrics[] values) => new(
        group, values.Length,
        AnalysisMath.Describe(values.Select(x => x.L2)),
        AnalysisMath.Describe(values.Select(x => x.L3)),
        AnalysisMath.Describe(values.Select(x => x.L6)),
        AnalysisMath.Describe(values.Select(x => x.Solo)),
        AnalysisMath.Describe(values.Select(x => x.UnknownRate)));

    private static LaneModelEvaluation[] Sweep(
        string family,
        EntryMetrics[] values,
        Func<EntryMetrics, double, double, LaneSignal> classify,
        bool includeSolo)
    {
        var output = new List<LaneModelEvaluation>();
        foreach (var minimum in MinimumSamples)
        foreach (var level in new[] { .025, .05, .075 })
        foreach (var solo in includeSolo ? new[] { .05, .10, .15, .20 } : new[] { 0d })
        {
            var id = includeSolo
                ? $"{family}_n{minimum}_l{Id(level)}_s{Id(solo)}"
                : $"{family}_n{minimum}_l{Id(level)}";
            var definition = new ModelDefinition(id, family, minimum, level,
                includeSolo ? solo : null);
            output.Add(Evaluate(definition, values, x => classify(x, level, solo)));
        }
        return output.ToArray();
    }

    private static LaneModelEvaluation Evaluate(
        ModelDefinition definition,
        EntryMetrics[] values,
        Func<EntryMetrics, LaneSignal> classify)
    {
        var outcomes = values.Select(x => new ClassifiedEntry(x,
            x.Entry.SampleSize < definition.MinimumSample
                ? LaneSignal.InsufficientEvidence : classify(x))).ToArray();
        var counts = Count(outcomes.Select(x => x.Result));
        var directional = outcomes.Where(x => IsDirectional(x.Result)).ToArray();
        var totalSamples = values.Sum(x => (long)x.Entry.SampleSize);
        var effectValues = directional.SelectMany(x => new[]
            { Math.Abs(x.Metrics.L2), Math.Abs(x.Metrics.L3), Math.Abs(x.Metrics.L6), Math.Abs(x.Metrics.Solo) }).ToArray();
        var symmetry = values.All(x => classify(Reverse(x)) == Reverse(classify(x)));
        return new LaneModelEvaluation(
            definition, counts,
            counts.Directional / (double)values.Length,
            directional.Sum(x => (long)x.Metrics.Entry.SampleSize) / (double)totalSamples,
            AnalysisMath.Describe(directional.Select(x => (double)x.Metrics.Entry.SampleSize)),
            AnalysisMath.Describe(effectValues), symmetry,
            Breakdowns(outcomes, x => x.Metrics.Entry.Role, Roles),
            Breakdowns(outcomes, x => SampleReliabilityClassifier.Classify(
                x.Metrics.Entry.SampleSize).ToString(),
                ["Poor", "Reference", "Moderate", "RelativelyStrong"]));
    }

    private static SplitEvaluation[] CreateSplitEvaluations(EntryMetrics[] values)
    {
        var result = new List<SplitEvaluation>();
        foreach (var minimum in MinimumSamples)
        foreach (var level in new[] { .025, .05, .075 })
        foreach (var solo in new[] { .05, .10, .15, .20 })
        {
            var pairs = values.Select(x => x.Entry.SampleSize < minimum
                ? new SignalPair(LaneSignal.InsufficientEvidence, LaneSignal.InsufficientEvidence)
                : new SignalPair(LevelMajority(x, level), Signal(x.Solo, solo))).ToArray();
            var eligible = pairs.Where(x => x.Level != LaneSignal.InsufficientEvidence).ToArray();
            var both = eligible.Where(x => IsDirectional(x.Level) && IsDirectional(x.Combat)).ToArray();
            result.Add(new SplitEvaluation(minimum, level, solo,
                Count(pairs.Select(x => x.Level)), Count(pairs.Select(x => x.Combat)),
                Rate(both.Count(x => x.Level == x.Combat), both.Length),
                Rate(both.Count(x => x.Level != x.Combat), both.Length),
                Rate(eligible.Count(x =>
                    (IsDirectional(x.Level) && x.Combat == LaneSignal.Neutral)
                    || (x.Level == LaneSignal.Neutral && IsDirectional(x.Combat))), eligible.Length),
                Rate(eligible.Count(x => x.Level == LaneSignal.Neutral
                    && x.Combat == LaneSignal.Neutral), eligible.Length)));
        }
        return result.ToArray();
    }

    private static ProgressionSummary Progression(int minimum, double threshold, EntryMetrics[] values)
    {
        var patterns = values.Where(x => x.Entry.SampleSize >= minimum)
            .Select(x => Pattern(Signal(x.L2, threshold), Signal(x.L3, threshold), Signal(x.L6, threshold)))
            .ToArray();
        var aaa = patterns.Count(x => x == "AAA");
        var bbb = patterns.Count(x => x == "BBB");
        var reversal = patterns.Count(x => x is "ABB" or "AAB" or "BAA" or "BBA" or "ABA" or "BAB");
        return new ProgressionSummary(minimum, threshold, patterns.Length, aaa, bbb,
            aaa + bbb, reversal, patterns.Count(x => x == "NNN"),
            patterns.Length - aaa - bbb - reversal - patterns.Count(x => x == "NNN"),
            patterns.GroupBy(x => x).OrderBy(x => x.Key, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.Count()));
    }

    private static AgreementSummary Agreement(int minimum, double level, double solo, EntryMetrics[] values)
    {
        var signals = values.Where(x => x.Entry.SampleSize >= minimum).Select(x => new[]
        { Signal(x.L2, level), Signal(x.L3, level), Signal(x.L6, level), Signal(x.Solo, solo) }).ToArray();
        double Agree(int a, int b) => Rate(signals.Count(x => x[a] == x[b]), signals.Length);
        return new AgreementSummary(minimum, level, solo, signals.Length,
            Agree(0, 1), Agree(0, 2), Agree(0, 3), Agree(1, 2), Agree(1, 3), Agree(2, 3),
            signals.Count(x => x.Distinct().Count() == 1),
            signals.Count(x => x.Contains(LaneSignal.SideA) && x.Contains(LaneSignal.SideB)));
    }

    private static IReadOnlyList<Breakdown> Breakdowns(
        ClassifiedEntry[] values, Func<ClassifiedEntry, string> selector, IEnumerable<string> groups) =>
        groups.Select(group =>
        {
            var selected = values.Where(x => selector(x) == group).ToArray();
            return new Breakdown(group, Count(selected.Select(x => x.Result)));
        }).ToArray();

    private static LaneCounts Count(IEnumerable<LaneSignal> source)
    {
        var values = source.ToArray();
        return new LaneCounts(values.Length, values.Count(x => x == LaneSignal.SideA),
            values.Count(x => x == LaneSignal.Neutral), values.Count(x => x == LaneSignal.SideB),
            values.Count(x => x == LaneSignal.InsufficientEvidence));
    }

    private static LaneSignal LevelMajority(EntryMetrics x, double threshold)
    {
        var signals = new[] { Signal(x.L2, threshold), Signal(x.L3, threshold), Signal(x.L6, threshold) };
        if (signals.Count(y => y == LaneSignal.SideA) >= 2) return LaneSignal.SideA;
        if (signals.Count(y => y == LaneSignal.SideB) >= 2) return LaneSignal.SideB;
        return LaneSignal.Neutral;
    }

    private static LaneSignal EarlySignal(EntryMetrics x, double threshold)
    {
        var l2 = Signal(x.L2, threshold);
        var l3 = Signal(x.L3, threshold);
        if (l2 == l3) return l2;
        if (l2 == LaneSignal.Neutral) return l3;
        if (l3 == LaneSignal.Neutral) return l2;
        return LaneSignal.Neutral;
    }

    private static LaneSignal Majority(params LaneSignal[] signals)
    {
        var a = signals.Count(x => x == LaneSignal.SideA);
        var b = signals.Count(x => x == LaneSignal.SideB);
        return a > b ? LaneSignal.SideA : b > a ? LaneSignal.SideB : LaneSignal.Neutral;
    }

    private static LaneSignal Signal(double value, double threshold) =>
        value >= threshold && value > 0 ? LaneSignal.SideA
        : value <= -threshold && value < 0 ? LaneSignal.SideB
        : LaneSignal.Neutral;

    private static EntryMetrics Reverse(EntryMetrics x) => x with
        { L2 = -x.L2, L3 = -x.L3, L6 = -x.L6, Solo = -x.Solo };
    private static LaneSignal Reverse(LaneSignal x) => x switch
    { LaneSignal.SideA => LaneSignal.SideB, LaneSignal.SideB => LaneSignal.SideA, _ => x };
    private static bool IsDirectional(LaneSignal x) => x is LaneSignal.SideA or LaneSignal.SideB;
    private static string Pattern(params LaneSignal[] values) => string.Concat(values.Select(x =>
        x == LaneSignal.SideA ? "A" : x == LaneSignal.SideB ? "B" : "N"));
    private static double Rate(int numerator, int denominator) => denominator == 0 ? 0 : (double)numerator / denominator;
    private static string Id(double value) => (value * 100).ToString("0.#", System.Globalization.CultureInfo.InvariantCulture).Replace('.', '_');
    private static string Bucket(int n) => n <= 4 ? "1-4" : n <= 9 ? "5-9" : n <= 19 ? "10-19" : n <= 49 ? "20-49" : n <= 99 ? "50-99" : "100+";

    private static ExampleSet CreateExamples(EntryMetrics[] metrics, LaneModelEvaluation model, SplitEvaluation split)
    {
        LaneSignal Result(EntryMetrics x) => x.Entry.SampleSize < model.Definition.MinimumSample
            ? LaneSignal.InsufficientEvidence
            : Majority(EarlySignal(x, model.Definition.LevelThreshold),
                Signal(x.L6, model.Definition.LevelThreshold), Signal(x.Solo, model.Definition.SoloThreshold ?? 0));
        Example ToExample(EntryMetrics x) => new(x.Entry.Role, x.Entry.ChampionA, x.Entry.ChampionB,
            x.Entry.SampleSize, x.L2, x.L3, x.L6, x.Solo, Result(x),
            LevelMajority(x, split.LevelThreshold), Signal(x.Solo, split.SoloThreshold));
        var eligible = metrics.Where(x => x.Entry.SampleSize >= 20).ToArray();
        return new ExampleSet(
            eligible.Where(x => Result(x) == LaneSignal.SideA).OrderByDescending(Strength).Take(10).Select(ToExample).ToArray(),
            eligible.Where(x => Result(x) == LaneSignal.SideB).OrderByDescending(Strength).Take(10).Select(ToExample).ToArray(),
            eligible.Where(x => Result(x) == LaneSignal.Neutral).OrderByDescending(x => x.Entry.SampleSize).Take(10).Select(ToExample).ToArray(),
            eligible.Where(x => IsDirectional(LevelMajority(x, split.LevelThreshold))
                && IsDirectional(Signal(x.Solo, split.SoloThreshold))
                && LevelMajority(x, split.LevelThreshold) != Signal(x.Solo, split.SoloThreshold))
                .OrderByDescending(x => x.Entry.SampleSize).Take(10).Select(ToExample).ToArray(),
            eligible.Where(x =>
            {
                var simple = Majority(Signal(x.L2, .05), Signal(x.L3, .05),
                    Signal(x.L6, .05), Signal(x.Solo, .10));
                return simple != Result(x);
            }).OrderByDescending(x => x.Entry.SampleSize).Take(10).Select(ToExample).ToArray());
        static double Strength(EntryMetrics x) => Math.Abs(x.L2) + Math.Abs(x.L3) + Math.Abs(x.L6) + Math.Abs(x.Solo);
    }

    private static OutcomeDiagnostic OutcomeDiagnostic(EntryMetrics[] metrics, LaneModelEvaluation model)
    {
        var rows = metrics.Where(x => x.Entry.SampleSize >= model.Definition.MinimumSample).Select(x =>
        {
            var lane = Majority(EarlySignal(x, model.Definition.LevelThreshold),
                Signal(x.L6, model.Definition.LevelThreshold), Signal(x.Solo, model.Definition.SoloThreshold ?? 0));
            var outcome = OutcomeAdvantageClassifier.Classify(x.Entry.WinsForA, x.Entry.SampleSize).Level;
            return (lane, outcome);
        }).Where(x => IsDirectional(x.lane)).ToArray();
        return new OutcomeDiagnostic(model.Definition.Id, rows.Length,
            rows.Count(x => (x.lane == LaneSignal.SideA && x.outcome == OutcomeAdvantageLevel.AllyAdvantage)
                || (x.lane == LaneSignal.SideB && x.outcome == OutcomeAdvantageLevel.EnemyAdvantage)),
            rows.Count(x => x.outcome == OutcomeAdvantageLevel.Neutral),
            rows.Count(x => (x.lane == LaneSignal.SideA && x.outcome == OutcomeAdvantageLevel.EnemyAdvantage)
                || (x.lane == LaneSignal.SideB && x.outcome == OutcomeAdvantageLevel.AllyAdvantage)),
            rows.Count(x => x.outcome == OutcomeAdvantageLevel.InsufficientEvidence));
    }

    private static SubsetSummary SubsetSummary(int minimum, ModelDefinition definition, EntryMetrics[] metrics)
    {
        var selected = metrics.Where(x => x.Entry.SampleSize >= minimum).ToArray();
        var outcomes = selected.Select(x => Majority(EarlySignal(x, definition.LevelThreshold),
            Signal(x.L6, definition.LevelThreshold), Signal(x.Solo, definition.SoloThreshold ?? 0)));
        return new SubsetSummary(minimum, Count(outcomes));
    }

    private static SensitivitySummary[] CreateSensitivity(EntryMetrics[] metrics) =>
        new[] { .025, .05, .075 }.SelectMany(level => new[] { .05, .10, .20 }.Select(solo =>
        {
            var definition = new ModelDefinition($"D_n20_l{Id(level)}_s{Id(solo)}", "D", 20, level, solo);
            var evaluation = Evaluate(definition, metrics, x => Majority(
                EarlySignal(x, level), Signal(x.L6, level), Signal(x.Solo, solo)));
            return new SensitivitySummary(level, solo, evaluation.Counts,
                evaluation.KeyDirectionalCoverage, evaluation.SampleWeightedDirectionalCoverage);
        })).ToArray();

    private static MetricSignalThresholdSummary[] CreateSignalThresholdCoverage(
        EntryMetrics[] metrics)
    {
        var output = new List<MetricSignalThresholdSummary>();
        foreach (var minimum in new[] { 20, 50 })
        {
            var eligible = metrics.Where(x => x.Entry.SampleSize >= minimum).ToArray();
            foreach (var threshold in LevelThresholds)
            foreach (var metric in new[] { "L2", "L3", "L6" })
            {
                var signals = eligible.Select(x => Signal(metric switch
                {
                    "L2" => x.L2,
                    "L3" => x.L3,
                    _ => x.L6
                }, threshold));
                output.Add(new MetricSignalThresholdSummary(
                    minimum, metric, threshold, Count(signals)));
            }

            foreach (var threshold in SoloThresholds)
            {
                output.Add(new MetricSignalThresholdSummary(
                    minimum,
                    "Solo",
                    threshold,
                    Count(eligible.Select(x => Signal(x.Solo, threshold)))));
            }
        }
        return output.ToArray();
    }

    private static void AppendDistribution(StringBuilder sb, string title, MetricDistributionGroup value)
    { sb.AppendLine($"## {title}").AppendLine(); AppendDistributionTable(sb, value); }
    private static void AppendDistributionTable(StringBuilder sb, MetricDistributionGroup value)
    {
        sb.AppendLine($"### {value.Group} (keys {value.Count:N0})").AppendLine();
        sb.AppendLine("| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |").AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        Row("L2", value.L2); Row("L3", value.L3); Row("L6", value.L6); Row("Solo", value.Solo); Row("Level Unknown", value.UnknownRate); sb.AppendLine();
        void Row(string name, Distribution d) => sb.AppendLine($"| {name} | {d.Mean:F4} | {d.Median:F4} | {d.StandardDeviation:F4} | {d.P10:F4} | {d.P25:F4} | {d.P75:F4} | {d.P90:F4} | {d.P95:F4} | {d.Min:F4} | {d.Max:F4} |");
    }
    private static void AppendSweep(StringBuilder sb, string title, IReadOnlyList<LaneModelEvaluation> values)
    { sb.AppendLine($"## {title}").AppendLine(); AppendModelTable(sb, values); }
    private static void AppendModelTable(StringBuilder sb, IReadOnlyList<LaneModelEvaluation> values)
    {
        sb.AppendLine("| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |").AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var x in values) sb.AppendLine($"| {x.Definition.Id} | {x.Counts.SideA:N0} | {x.Counts.Neutral:N0} | {x.Counts.SideB:N0} | {x.Counts.Insufficient:N0} | {x.KeyDirectionalCoverage:P2} | {x.SampleWeightedDirectionalCoverage:P2} | {x.DirectionalSampleSize.Median:N1} | {x.SymmetryValid} |");
        sb.AppendLine();
    }
    private static void AppendBreakdown(StringBuilder sb, IReadOnlyList<Breakdown> values)
    { sb.AppendLine("| Group | A | Neutral | B | Insufficient | directional |").AppendLine("|---|---:|---:|---:|---:|---:|"); foreach (var x in values) sb.AppendLine($"| {x.Group} | {x.Counts.SideA:N0} | {x.Counts.Neutral:N0} | {x.Counts.SideB:N0} | {x.Counts.Insufficient:N0} | {x.Counts.DirectionalRate:P2} |"); sb.AppendLine(); }
    private static void AppendJson(StringBuilder sb, object value) => sb.AppendLine("```json").AppendLine(JsonSerializer.Serialize(value, JsonOptions)).AppendLine("```").AppendLine();
    private static JsonSerializerOptions CreateJsonOptions() { var o = new JsonSerializerOptions { WriteIndented = true }; o.Converters.Add(new JsonStringEnumConverter()); return o; }
}

internal enum LaneSignal { InsufficientEvidence, Neutral, SideA, SideB }
internal sealed record EntryMetrics(StatisticsMatchupEntry Entry, double L2, double L3, double L6, double Solo, double UnknownRate);
internal sealed record LaneDatasetSummary(string Patch, int AcceptedMatches, long Observations, int MatchupKeys, string TierScope, string Region, int QueueId, int? MinimumGameDurationSeconds, long? DatasetRevision);
internal sealed record MetricDefinitionSummary(string LevelDifference, string SoloDifference, string OutcomeSeparation);
internal sealed record MetricDistributionGroup(string Group, int Count, Distribution L2, Distribution L3, Distribution L6, Distribution Solo, Distribution UnknownRate);
internal sealed record ModelDefinition(string Id, string Family, int MinimumSample, double LevelThreshold, double? SoloThreshold);
internal sealed record LaneCounts(int Total, int SideA, int Neutral, int SideB, int Insufficient) { public int Directional => SideA + SideB; public double DirectionalRate => Total == 0 ? 0 : (double)Directional / Total; }
internal sealed record Breakdown(string Group, LaneCounts Counts);
internal sealed record LaneModelEvaluation(ModelDefinition Definition, LaneCounts Counts, double KeyDirectionalCoverage, double SampleWeightedDirectionalCoverage, Distribution DirectionalSampleSize, Distribution DirectionalMetricEffects, bool SymmetryValid, IReadOnlyList<Breakdown> RoleBreakdown, IReadOnlyList<Breakdown> ReliabilityBreakdown);
internal sealed record ClassifiedEntry(EntryMetrics Metrics, LaneSignal Result);
internal sealed record SignalPair(LaneSignal Level, LaneSignal Combat);
internal sealed record SplitEvaluation(int MinimumSample, double LevelThreshold, double SoloThreshold, LaneCounts LevelCounts, LaneCounts CombatCounts, double SameDirectionAmongBothDirectional, double ConflictAmongBothDirectional, double OneNeutralRate, double BothNeutralRate);
internal sealed record ProgressionSummary(int MinimumSample, double Threshold, int Eligible, int AAA, int BBB, int ConsistentSameDirection, int Reversal, int NNN, int MixedOrNeutral, IReadOnlyDictionary<string, int> Patterns);
internal sealed record AgreementSummary(int MinimumSample, double LevelThreshold, double SoloThreshold, int Eligible, double L2L3, double L2L6, double L2Solo, double L3L6, double L3Solo, double L6Solo, int AllFourSame, int ContainsOpposingSignals);
internal sealed record Example(string Role, string ChampionA, string ChampionB, int SampleSize, double L2Difference, double L3Difference, double L6Difference, double SoloDifference, LaneSignal CombinedResult, LaneSignal LevelTempo, LaneSignal CombatPressure);
internal sealed record ExampleSet(IReadOnlyList<Example> StrongestA, IReadOnlyList<Example> StrongestB, IReadOnlyList<Example> Neutral, IReadOnlyList<Example> LevelCombatConflicts, IReadOnlyList<Example> CombinedModelDisagreements);
internal sealed record OutcomeDiagnostic(string ModelId, int DirectionalLaneResults, int SameDirectionalOutcome, int NeutralOutcome, int OppositeDirectionalOutcome, int InsufficientOutcome);
internal sealed record SubsetSummary(int MinimumSample, LaneCounts Counts);
internal sealed record SensitivitySummary(double LevelThreshold, double SoloThreshold, LaneCounts Counts, double KeyDirectionalCoverage, double SampleWeightedDirectionalCoverage);
internal sealed record MetricSignalThresholdSummary(int MinimumSample, string Metric, double Threshold, LaneCounts Counts);
internal sealed record LaneRecommendation(string Decision, string LevelModelId, double CombatThreshold, int MinimumSample, double LevelThreshold, IReadOnlyList<string> Reasons, string Terms, string StateTerms);
internal sealed record LanePressureResearchReport(LaneDatasetSummary Dataset, string ResearchScope, MetricDefinitionSummary Definitions, MetricDistributionGroup MetricDistributions, IReadOnlyList<MetricDistributionGroup> RoleDistributions, IReadOnlyList<MetricDistributionGroup> SampleVolatility, IReadOnlyList<ProgressionSummary> Progression, IReadOnlyList<AgreementSummary> Agreement, IReadOnlyList<MetricSignalThresholdSummary> SignalThresholdCoverage, IReadOnlyList<LaneModelEvaluation> ModelA, IReadOnlyList<LaneModelEvaluation> ModelB, IReadOnlyList<LaneModelEvaluation> ModelD, IReadOnlyList<SplitEvaluation> SplitModels, IReadOnlyList<LaneModelEvaluation> CandidateShortlist, IReadOnlyList<SubsetSummary> HighSampleSubsets, IReadOnlyList<SensitivitySummary> ThresholdSensitivity, ExampleSet Examples, OutcomeDiagnostic OutcomeDiagnostic, LaneRecommendation Recommendation, IReadOnlyList<string> Limitations);
