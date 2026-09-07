using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

internal static class TeamSummaryModelResearch
{
    private static readonly string[] Roles = ["TOP", "JUNGLE", "MID", "ADC", "SUPPORT"];

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static TeamSummaryResearchReport Analyze(
        StatisticsDataset dataset,
        long? datasetRevision,
        StatisticsMatchupEntry[] entries)
    {
        var classified = entries.Select(Classify).ToArray();
        var roleDistributions = Roles.Select(role => BuildRoleDistribution(
            role,
            classified.Where(item => item.Role == role).ToArray())).ToArray();
        var overall = BuildRoleDistribution("ALL", classified);
        var candidates = CandidateDefinitions();
        var fixtures = CreateFixtures();
        var fixtureResults = fixtures.Select(fixture => new FixtureEvaluation(
            fixture.Id,
            fixture.Description,
            fixture.Roles.Count(item => item.Available),
            fixture.Roles.Count(item => item.Available
                && item.Direction != ResearchDirection.Insufficient),
            fixture.Roles.Count(item => item.Direction == ResearchDirection.Ally),
            fixture.Roles.Count(item => item.Direction == ResearchDirection.Neutral),
            fixture.Roles.Count(item => item.Direction == ResearchDirection.Enemy),
            fixture.Roles.Count(item => !item.Available),
            candidates.Select(candidate => new CandidateFixtureResult(
                candidate.Id,
                Evaluate(candidate.Id, fixture.Roles))).ToArray())).ToArray();
        ValidateFixtureExpectations(fixtureResults);

        return new TeamSummaryResearchReport(
            new TeamSummaryDataset(
                dataset.Patch,
                dataset.AcceptedMatches,
                entries.Length,
                dataset.TierScope,
                dataset.Region,
                dataset.QueueId,
                dataset.MinimumGameDurationSeconds,
                datasetRevision),
            "Summarize five independent role-level categorical tendencies for faster reading; not a team win probability or composition model.",
            "The dataset contains independent role matchup aggregates, not observed five-role team combinations. Empirical runtime team-summary coverage and predictive validity cannot be measured from this file.",
            overall,
            roleDistributions,
            candidates,
            fixtureResults,
            new TeamSummaryRecommendation(
                "A_OUTCOME_ONLY",
                "E_CONSERVATIVE_EQUAL",
                3,
                2,
                "Equal categorical votes; no reliability, role-importance, raw win-rate, or sample-size weighting.",
                "Eligible roles below 3 produce InsufficientEvidence. With sufficient eligible roles, at least two same-side directional roles and a strict majority over the opposite side are required; ties and weaker evidence are Neutral.",
                "팀 결과 경향",
                "아군 쪽 경향 / 적군 쪽 경향 / 뚜렷한 차이 없음 / 판정 보류",
                "InGame dashboard header compact strip (approximately 40-70 px).",
                "Show eligible roles and Ally / Enemy / Neutral / unavailable counts; do not create a Team Reliability badge.",
                "Recommend Outcome Summary only. Keep Level Tempo and Combat Pressure on role cards because aggregate data cannot validate team-level meaning, and direct-opponent solo kills vary in interpretation for JUNGLE/SUPPORT."),
            new[]
            {
                "No observed five-role combination rows are available, so fixture outcomes are behavioral tests rather than empirical frequencies.",
                "Reliability bands are categorical communication aids, not calibrated probabilities or accuracy multipliers.",
                "No candidate estimates team win probability, composition synergy, role importance, or an overall advantage score.",
                "Sample sizes from different matchup keys must not be summed into a team sample size.",
                "Missing and NotFound roles are unavailable, while Neutral is an eligible non-directional result and Insufficient lacks minimum evidence."
            });
    }

    public static string BuildMarkdown(TeamSummaryResearchReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Team Summary Model Research").AppendLine();
        builder.AppendLine("## Dataset Summary").AppendLine()
            .AppendLine($"- Patch: `{report.Dataset.Patch}`")
            .AppendLine($"- Accepted matches: {report.Dataset.AcceptedMatches:N0}")
            .AppendLine($"- Matchup keys: {report.Dataset.MatchupKeys:N0}")
            .AppendLine($"- Scope: `{report.Dataset.TierScope}` / `{report.Dataset.Region}` / queue `{report.Dataset.QueueId}`")
            .AppendLine($"- Dataset revision: {report.Dataset.DatasetRevision?.ToString() ?? "unknown"}").AppendLine();
        builder.AppendLine("## Research Scope").AppendLine().AppendLine(report.ResearchScope).AppendLine();
        builder.AppendLine("## Important Data Limitation").AppendLine().AppendLine($"> {report.ImportantDataLimitation}").AppendLine();
        AppendDistribution(builder, "Per-role Outcome Distribution", report.RoleDistributions, item => item.Outcome);
        AppendDistribution(builder, "Per-role Reliability Distribution", report.RoleDistributions, item => item.Reliability);
        builder.AppendLine("## Outcome × Reliability").AppendLine();
        builder.AppendLine("Counts are empirical per-key classifier results; they are not observed team combinations.").AppendLine();
        builder.AppendLine("| Role | Outcome | Reliability | Count |").AppendLine("|---|---|---|---:|");
        foreach (var role in report.RoleDistributions)
        foreach (var cell in role.OutcomeReliability.Where(item => item.Count > 0))
            builder.AppendLine($"| {role.Role} | {cell.Outcome} | {cell.Reliability} | {cell.Count:N0} |");
        builder.AppendLine();
        builder.AppendLine("## Aggregation Model Definitions").AppendLine();
        foreach (var candidate in report.Candidates)
            builder.AppendLine($"### {candidate.Id} — {candidate.Name}\n\n{candidate.Rule}\n\n- Strength: {candidate.Strength}\n- Risk: {candidate.Risk}\n");
        builder.AppendLine("### Candidate comparison").AppendLine()
            .AppendLine("| Model | Simplicity | Explainability | Conservative | Missing robustness | Reliability awareness | Dominance risk | UI clarity |")
            .AppendLine("|---|---|---|---|---|---|---|---|");
        foreach (var candidate in report.Candidates)
            builder.AppendLine($"| {candidate.Id} | {candidate.Simplicity} | {candidate.Explainability} | {candidate.ConservativeBehavior} | {candidate.MissingRobustness} | {candidate.ReliabilityAwareness} | {candidate.DominanceRisk} | {candidate.UiClarity} |");
        builder.AppendLine();
        AppendCandidateSection(builder, report, "A_EQUAL_MIN2", "Equal Vote Analysis");
        AppendCandidateSection(builder, report, "B_RELIABILITY_WEIGHTED", "Reliability-weighted Analysis");
        AppendCandidateSection(builder, report, "C_SAMPLE_WEIGHTED_CAPPED", "Sample-weighted Analysis");
        AppendCandidateSection(builder, report, "E_CONSERVATIVE_EQUAL", "Conservative Model Analysis");
        AppendCandidateSection(builder, report, "F_STRONG_ONLY", "Strong-only Analysis");
        builder.AppendLine("## Missing/NotFound/Insufficient Behavior").AppendLine()
            .AppendLine("Missing and NotFound are unavailable and excluded from eligible-role counts. Insufficient classifier results are also not eligible. Neutral is eligible but contributes no directional vote. Dataset unavailable/invalid prevents the entire summary.").AppendLine();
        builder.AppendLine("## Representative Team Fixtures").AppendLine();
        builder.AppendLine("These deterministic fixtures test rule behavior only; they are not sampled games or coverage estimates.").AppendLine();
        builder.AppendLine("| Fixture | Available | Eligible | A | Neutral | B | Missing | Candidate results |").AppendLine("|---|---:|---:|---:|---:|---:|---:|---|");
        foreach (var fixture in report.Fixtures)
            builder.AppendLine($"| {fixture.Id} | {fixture.AvailableRoles} | {fixture.EligibleRoles} | {fixture.AllyRoles} | {fixture.NeutralRoles} | {fixture.EnemyRoles} | {fixture.UnavailableRoles} | {string.Join("; ", fixture.Results.Select(result => $"{result.ModelId}={result.Result}"))} |");
        builder.AppendLine();
        builder.AppendLine("## Outcome Summary Recommendation").AppendLine()
            .AppendLine("Recommend an Outcome-only categorical summary using the conservative equal-vote rule. It summarizes role-level final-result tendencies and must not be described as team win probability.").AppendLine();
        AppendDistribution(builder, "Level Tempo Summary Evaluation", report.RoleDistributions, item => item.LevelTempo);
        builder.AppendLine("The same categorical mechanics are technically possible, but the aggregate file cannot validate whether five independent level-timing tendencies form a meaningful team-level tempo measure. Keep Level Tempo on role cards for now.").AppendLine();
        AppendDistribution(builder, "Combat Pressure Summary Evaluation", report.RoleDistributions, item => item.CombatPressure);
        builder.AppendLine("Do not promote a team Combat Summary yet. Direct-opponent solo-kill frequency has role-dependent meaning, especially for JUNGLE and SUPPORT, and omits other combat participation.").AppendLine();
        builder.AppendLine("## UI Terminology Recommendation").AppendLine()
            .AppendLine("Use `팀 결과 경향`, with `아군 쪽 경향`, `적군 쪽 경향`, `뚜렷한 차이 없음`, or `판정 보류`. Avoid team advantage, win probability, composition advantage, and confidence percentages.").AppendLine();
        builder.AppendLine("## UI Placement Recommendation").AppendLine()
            .AppendLine("Use a compact 40–70 px strip above the InGame role dashboard. Show `판정 가능 4/5 · 아군 3 · 적군 1 · 중립 0` rather than a Team Reliability badge. Long explanation belongs in a later expanded view.").AppendLine();
        builder.AppendLine("## Limitations").AppendLine();
        foreach (var limitation in report.Limitations) builder.AppendLine($"- {limitation}");
        builder.AppendLine();
        builder.AppendLine("## Production Recommendation").AppendLine()
            .AppendLine($"Decision: `{report.Recommendation.FinalDecision}`")
            .AppendLine($"Recommended model: `{report.Recommendation.RecommendedModelId}`")
            .AppendLine($"Rule: {report.Recommendation.Rule}")
            .AppendLine($"Rationale: {report.Recommendation.Rationale}").AppendLine();
        builder.AppendLine("## Next Stage").AppendLine()
            .AppendLine("If accepted, Stage 9-4B may implement only the Outcome Team Summary as runtime-derived presentation. No production Team Summary classifier or UI was added in this research stage.");
        return builder.ToString();
    }

    private static TeamClassifiedEntry Classify(StatisticsMatchupEntry entry) => new(
        entry.Role,
        entry.SampleSize,
        ToDirection(OutcomeAdvantageClassifier.Classify(entry.WinsForA, entry.SampleSize).Level),
        SampleReliabilityClassifier.Classify(entry.SampleSize),
        ToDirection(LevelTempoClassifier.Classify(
            entry.SampleSize,
            entry.FirstLevel2ForA, entry.FirstLevel2ForB, entry.FirstLevel2Unknown,
            entry.FirstLevel3ForA, entry.FirstLevel3ForB, entry.FirstLevel3Unknown,
            entry.FirstLevel6ForA, entry.FirstLevel6ForB, entry.FirstLevel6Unknown).Level),
        ToDirection(CombatPressureClassifier.Classify(
            entry.SampleSize,
            entry.SoloKillsBefore15ForA,
            entry.SoloKillsBefore15ForB).Level));

    private static RoleDistribution BuildRoleDistribution(string role, TeamClassifiedEntry[] entries)
    {
        var outcome = CountDirections(entries.Select(item => item.Outcome));
        var level = CountDirections(entries.Select(item => item.LevelTempo));
        var combat = CountDirections(entries.Select(item => item.CombatPressure));
        var reliability = Enum.GetValues<SampleReliabilityLevel>().Select(levelValue =>
            new NamedCount(levelValue.ToString(), entries.Count(item => item.Reliability == levelValue), entries.Length)).ToArray();
        var cross = Enum.GetValues<ResearchDirection>().SelectMany(outcomeValue =>
            Enum.GetValues<SampleReliabilityLevel>().Select(reliabilityValue =>
                new CrossCount(outcomeValue, reliabilityValue,
                    entries.Count(item => item.Outcome == outcomeValue
                        && item.Reliability == reliabilityValue)))).ToArray();
        return new RoleDistribution(role, entries.Length, outcome, reliability, cross, level, combat);
    }

    private static NamedCount[] CountDirections(IEnumerable<ResearchDirection> values)
    {
        var array = values.ToArray();
        return Enum.GetValues<ResearchDirection>().Select(direction =>
            new NamedCount(direction.ToString(), array.Count(value => value == direction), array.Length)).ToArray();
    }

    private static CandidateDefinition[] CandidateDefinitions() =>
    [
        new("A_EQUAL_MIN2", "Equal vote, minimum 2 eligible", "Eligible roles < 2 => Insufficient; otherwise +1/-1 majority, tie => Neutral.", "Simplest and highly explainable.", "One directional role can dominate several Neutral roles.", "High", "High", "Low", "Medium", "None", "Low", "High"),
        new("A_EQUAL_MIN3", "Equal vote, minimum 3 eligible", "Eligible roles < 3 => Insufficient; otherwise +1/-1 majority, tie => Neutral.", "Robust to missing/insufficient roles.", "Still allows one directional role to determine the result.", "High", "High", "Medium", "High", "None", "Low", "High"),
        new("B_RELIABILITY_WEIGHTED", "Reliability-weighted categorical vote", "Minimum 3 eligible; Moderate=1, RelativelyStrong=1.5, then weighted directional majority.", "Acknowledges broad evidence bands.", "Weights are heuristic, not calibrated accuracy; harder to explain.", "Medium", "Medium", "Medium", "High", "Heuristic", "Medium", "Medium"),
        new("C_SAMPLE_WEIGHTED_CAPPED", "Capped sample-weighted categorical vote", "Minimum 3 eligible; directional weight=sqrt(min(sampleSize,100)).", "Limits but does not remove sample dominance.", "A single large matchup may override multiple Role directions.", "Low", "Low", "Low", "High", "Raw sample", "High", "Low"),
        new("D_DIRECTIONAL_MIN2", "Directional majority only", "At least 2 directional roles; compare directional counts, tie => Neutral.", "Clear evidence-count interpretation.", "All-Neutral evidence becomes Insufficient despite being fully available.", "High", "High", "Medium", "Medium", "None", "Low", "High"),
        new("E_CONSERVATIVE_EQUAL", "Conservative equal majority", "Minimum 3 eligible; a side needs at least 2 directional roles and more votes than the other side; otherwise Neutral.", "Simple, resists one-role domination, and handles Neutral explicitly.", "May leave many runtime states Neutral; empirical frequency is unknown.", "High", "High", "High", "High", "None", "Low", "High"),
        new("F_STRONG_ONLY", "RelativelyStrong-only", "Only sampleSize >= 50 roles are eligible; at least 2 required, then equal directional majority.", "Uses the strongest existing reliability band only.", "Very low per-key availability and likely poor runtime coverage.", "High", "High", "High", "Low", "Categorical", "Low", "Medium")
    ];

    private static TeamResearchResult Evaluate(string modelId, IReadOnlyList<FixtureRole> roles)
    {
        var available = roles.Where(item => item.Available).ToArray();
        var eligible = available.Where(item => item.Direction != ResearchDirection.Insufficient).ToArray();
        if (modelId == "F_STRONG_ONLY")
        {
            eligible = eligible.Where(item => item.Reliability == SampleReliabilityLevel.RelativelyStrong).ToArray();
            return eligible.Length < 2 ? TeamResearchResult.InsufficientEvidence : EqualMajority(eligible);
        }
        if (modelId == "D_DIRECTIONAL_MIN2")
        {
            var directional = eligible.Where(IsDirectional).ToArray();
            return directional.Length < 2 ? TeamResearchResult.InsufficientEvidence : EqualMajority(directional);
        }

        var minimumEligible = modelId == "A_EQUAL_MIN2" ? 2 : 3;
        if (eligible.Length < minimumEligible) return TeamResearchResult.InsufficientEvidence;
        if (modelId == "B_RELIABILITY_WEIGHTED")
            return Weighted(eligible, item => item.Reliability == SampleReliabilityLevel.RelativelyStrong ? 1.5 : 1);
        if (modelId == "C_SAMPLE_WEIGHTED_CAPPED")
            return Weighted(eligible, item => Math.Sqrt(Math.Min(item.SampleSize, 100)));
        if (modelId == "E_CONSERVATIVE_EQUAL")
        {
            var ally = eligible.Count(item => item.Direction == ResearchDirection.Ally);
            var enemy = eligible.Count(item => item.Direction == ResearchDirection.Enemy);
            if (ally >= 2 && ally > enemy) return TeamResearchResult.AllyTendency;
            if (enemy >= 2 && enemy > ally) return TeamResearchResult.EnemyTendency;
            return TeamResearchResult.Neutral;
        }
        return EqualMajority(eligible);
    }

    private static TeamResearchResult EqualMajority(IReadOnlyCollection<FixtureRole> roles)
    {
        var ally = roles.Count(item => item.Direction == ResearchDirection.Ally);
        var enemy = roles.Count(item => item.Direction == ResearchDirection.Enemy);
        return ally > enemy ? TeamResearchResult.AllyTendency
            : enemy > ally ? TeamResearchResult.EnemyTendency
            : TeamResearchResult.Neutral;
    }

    private static TeamResearchResult Weighted(
        IEnumerable<FixtureRole> roles,
        Func<FixtureRole, double> weight)
    {
        var score = roles.Sum(item => item.Direction switch
        {
            ResearchDirection.Ally => weight(item),
            ResearchDirection.Enemy => -weight(item),
            _ => 0
        });
        return score > 1e-12 ? TeamResearchResult.AllyTendency
            : score < -1e-12 ? TeamResearchResult.EnemyTendency
            : TeamResearchResult.Neutral;
    }

    private static bool IsDirectional(FixtureRole item) =>
        item.Direction is ResearchDirection.Ally or ResearchDirection.Enemy;

    private static Fixture[] CreateFixtures()
    {
        static FixtureRole R(ResearchDirection direction, int sample = 30) =>
            new(true, direction, sample >= 50
                ? SampleReliabilityLevel.RelativelyStrong
                : sample >= 20 ? SampleReliabilityLevel.Moderate
                : SampleReliabilityLevel.Reference, sample);
        static FixtureRole Missing() => new(false, ResearchDirection.Insufficient, 0, 0);
        return
        [
            new("A_3A_2B", "3 Ally / 2 Enemy", [R(ResearchDirection.Ally), R(ResearchDirection.Ally), R(ResearchDirection.Ally), R(ResearchDirection.Enemy), R(ResearchDirection.Enemy)]),
            new("B_1A_4I", "1 Ally / 4 Insufficient", [R(ResearchDirection.Ally), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10)]),
            new("C_2A_2B_1N", "2 Ally / 2 Enemy / 1 Neutral", [R(ResearchDirection.Ally), R(ResearchDirection.Ally), R(ResearchDirection.Enemy), R(ResearchDirection.Enemy), R(ResearchDirection.Neutral)]),
            new("D_1A_STRONG_2B", "1 Ally Strong / 2 Enemy Moderate / 2 Neutral", [R(ResearchDirection.Ally, 500), R(ResearchDirection.Enemy, 20), R(ResearchDirection.Enemy, 20), R(ResearchDirection.Neutral), R(ResearchDirection.Neutral)]),
            new("E_2A_STRONG_1B", "2 Ally Strong / 1 Enemy Moderate / 2 Neutral", [R(ResearchDirection.Ally, 100), R(ResearchDirection.Ally, 80), R(ResearchDirection.Enemy), R(ResearchDirection.Neutral), R(ResearchDirection.Neutral)]),
            new("F_ALL_NEUTRAL", "All Neutral", [R(ResearchDirection.Neutral), R(ResearchDirection.Neutral), R(ResearchDirection.Neutral), R(ResearchDirection.Neutral), R(ResearchDirection.Neutral)]),
            new("G_ALL_INSUFFICIENT", "All Insufficient", [R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10), R(ResearchDirection.Insufficient, 10)]),
            new("H_4_ELIGIBLE_1_MISSING", "2 Ally / 1 Enemy / 1 Neutral / 1 Missing", [R(ResearchDirection.Ally), R(ResearchDirection.Ally), R(ResearchDirection.Enemy), R(ResearchDirection.Neutral), Missing()])
        ];
    }

    private static void ValidateFixtureExpectations(FixtureEvaluation[] fixtures)
    {
        TeamResearchResult Result(string fixture, string model) => fixtures.Single(item => item.Id == fixture)
            .Results.Single(item => item.ModelId == model).Result;
        var valid = Result("A_3A_2B", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.AllyTendency
            && Result("B_1A_4I", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.InsufficientEvidence
            && Result("C_2A_2B_1N", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.Neutral
            && Result("D_1A_STRONG_2B", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.EnemyTendency
            && Result("D_1A_STRONG_2B", "C_SAMPLE_WEIGHTED_CAPPED") == TeamResearchResult.AllyTendency
            && Result("E_2A_STRONG_1B", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.AllyTendency
            && Result("F_ALL_NEUTRAL", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.Neutral
            && Result("G_ALL_INSUFFICIENT", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.InsufficientEvidence
            && Result("H_4_ELIGIBLE_1_MISSING", "E_CONSERVATIVE_EQUAL") == TeamResearchResult.AllyTendency;
        if (!valid) throw new InvalidDataException("Team Summary fixture behavior validation failed.");
    }

    private static ResearchDirection ToDirection(OutcomeAdvantageLevel value) => value switch
    {
        OutcomeAdvantageLevel.AllyAdvantage => ResearchDirection.Ally,
        OutcomeAdvantageLevel.EnemyAdvantage => ResearchDirection.Enemy,
        OutcomeAdvantageLevel.Neutral => ResearchDirection.Neutral,
        _ => ResearchDirection.Insufficient
    };
    private static ResearchDirection ToDirection(LevelTempoLevel value) => value switch
    {
        LevelTempoLevel.AllyTendency => ResearchDirection.Ally,
        LevelTempoLevel.EnemyTendency => ResearchDirection.Enemy,
        LevelTempoLevel.Neutral => ResearchDirection.Neutral,
        _ => ResearchDirection.Insufficient
    };
    private static ResearchDirection ToDirection(CombatPressureLevel value) => value switch
    {
        CombatPressureLevel.AllyTendency => ResearchDirection.Ally,
        CombatPressureLevel.EnemyTendency => ResearchDirection.Enemy,
        CombatPressureLevel.Neutral => ResearchDirection.Neutral,
        _ => ResearchDirection.Insufficient
    };

    private static void AppendDistribution(
        StringBuilder builder,
        string title,
        RoleDistribution[] roles,
        Func<RoleDistribution, NamedCount[]> selector)
    {
        builder.AppendLine($"## {title}").AppendLine()
            .AppendLine("| Role | State | Count | Percent |").AppendLine("|---|---|---:|---:|");
        foreach (var role in roles)
        foreach (var value in selector(role))
            builder.AppendLine($"| {role.Role} | {value.Name} | {value.Count:N0} | {value.Rate:P2} |");
        builder.AppendLine();
    }

    private static void AppendCandidateSection(
        StringBuilder builder,
        TeamSummaryResearchReport report,
        string modelId,
        string title)
    {
        var candidate = report.Candidates.Single(item => item.Id == modelId);
        builder.AppendLine($"## {title}").AppendLine()
            .AppendLine(candidate.Rule).AppendLine()
            .AppendLine($"Strength: {candidate.Strength}")
            .AppendLine($"Risk: {candidate.Risk}").AppendLine();
    }
}

internal enum ResearchDirection { Ally, Neutral, Enemy, Insufficient }
internal enum TeamResearchResult { InsufficientEvidence, Neutral, AllyTendency, EnemyTendency }
internal sealed record TeamClassifiedEntry(string Role, int SampleSize, ResearchDirection Outcome, SampleReliabilityLevel Reliability, ResearchDirection LevelTempo, ResearchDirection CombatPressure);
internal sealed record TeamSummaryDataset(string Patch, int AcceptedMatches, int MatchupKeys, string TierScope, string Region, int QueueId, int? MinimumGameDurationSeconds, long? DatasetRevision);
internal sealed record NamedCount(string Name, int Count, int Total) { public double Rate => Total == 0 ? 0 : (double)Count / Total; }
internal sealed record CrossCount(ResearchDirection Outcome, SampleReliabilityLevel Reliability, int Count);
internal sealed record RoleDistribution(string Role, int Total, NamedCount[] Outcome, NamedCount[] Reliability, CrossCount[] OutcomeReliability, NamedCount[] LevelTempo, NamedCount[] CombatPressure);
internal sealed record CandidateDefinition(string Id, string Name, string Rule, string Strength, string Risk, string Simplicity, string Explainability, string ConservativeBehavior, string MissingRobustness, string ReliabilityAwareness, string DominanceRisk, string UiClarity);
internal sealed record FixtureRole(bool Available, ResearchDirection Direction, SampleReliabilityLevel Reliability, int SampleSize);
internal sealed record Fixture(string Id, string Description, FixtureRole[] Roles);
internal sealed record CandidateFixtureResult(string ModelId, TeamResearchResult Result);
internal sealed record FixtureEvaluation(string Id, string Description, int AvailableRoles, int EligibleRoles, int AllyRoles, int NeutralRoles, int EnemyRoles, int UnavailableRoles, CandidateFixtureResult[] Results);
internal sealed record TeamSummaryRecommendation(string FinalDecision, string RecommendedModelId, int MinimumEligibleRoles, int MinimumDirectionalRolesForTendency, string Weighting, string Rule, string Title, string StateTerms, string UiPlacement, string EvidenceDisplay, string Rationale);
internal sealed record TeamSummaryResearchReport(TeamSummaryDataset Dataset, string ResearchScope, string ImportantDataLimitation, RoleDistribution OverallDistribution, RoleDistribution[] RoleDistributions, CandidateDefinition[] Candidates, FixtureEvaluation[] Fixtures, TeamSummaryRecommendation Recommendation, string[] Limitations);
