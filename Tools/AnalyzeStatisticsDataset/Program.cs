using System.Text;
using System.Text.Json;
using AnalyzeStatisticsDataset;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

var root = FindRepositoryRoot();
var inputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(root, "Data", "Statistics", "master_plus.json");
var outputDirectory = Path.Combine(root, "Tools", "AnalyzeStatisticsDataset", "Output");
var jsonOutputPath = Path.Combine(outputDirectory, "statistics-analysis.json");
var markdownOutputPath = Path.Combine(outputDirectory, "statistics-analysis.md");
var advantageJsonOutputPath = Path.Combine(outputDirectory, "advantage-model-analysis.json");
var advantageMarkdownOutputPath = Path.Combine(outputDirectory, "advantage-model-analysis.md");
var lanePressureJsonOutputPath = Path.Combine(outputDirectory, "lane-pressure-model-analysis.json");
var lanePressureMarkdownOutputPath = Path.Combine(outputDirectory, "lane-pressure-model-analysis.md");
var teamSummaryJsonOutputPath = Path.Combine(outputDirectory, "team-summary-model-analysis.json");
var teamSummaryMarkdownOutputPath = Path.Combine(outputDirectory, "team-summary-model-analysis.md");

Console.WriteLine("LoL Matchup statistics reliability analysis");
Console.WriteLine($"Input: {inputPath}");

try
{
    var dataset = JsonSerializer.Deserialize<StatisticsDataset>(await File.ReadAllBytesAsync(inputPath));
    if (!StatisticsDatasetValidator.TryValidate(dataset, out var index, out var validationError))
    {
        throw new InvalidDataException(validationError);
    }

    var manifest = await LoadManifestAsync(Path.Combine(Path.GetDirectoryName(inputPath)!, "manifest.json"));
    var entries = index.Values.OrderBy(item => item.Role, StringComparer.Ordinal)
        .ThenBy(item => item.ChampionA, StringComparer.OrdinalIgnoreCase)
        .ThenBy(item => item.ChampionB, StringComparer.OrdinalIgnoreCase).ToArray();
    var sampleDistribution = AnalysisMath.Describe(entries.Select(item => (double)item.SampleSize));
    var thresholds = new[] { 1, 2, 3, 5, 10, 20, 30, 50, 75, 100, 150, 200 }
        .Select(value => Coverage(value, entries)).ToArray();
    var lowSamples = new Dictionary<string, int>
    {
        ["1"] = entries.Count(item => item.SampleSize == 1),
        ["2"] = entries.Count(item => item.SampleSize == 2),
        ["3-4"] = entries.Count(item => item.SampleSize is >= 3 and <= 4),
        ["5-9"] = entries.Count(item => item.SampleSize is >= 5 and <= 9)
    };
    var roles = new[] { "TOP", "JUNGLE", "MID", "ADC", "SUPPORT" }
        .Select(role => AnalyzeRole(role, entries)).ToArray();
    var topSamples = entries.OrderByDescending(item => item.SampleSize)
        .ThenBy(item => item.Role, StringComparer.Ordinal)
        .ThenBy(item => item.ChampionA, StringComparer.OrdinalIgnoreCase)
        .ThenBy(item => item.ChampionB, StringComparer.OrdinalIgnoreCase)
        .Take(20).Select(ToMatchupSummary).ToArray();

    var winBuckets = new[]
    {
        AnalyzeWinBucket("1-4", entries.Where(item => item.SampleSize is >= 1 and <= 4)),
        AnalyzeWinBucket("5-9", entries.Where(item => item.SampleSize is >= 5 and <= 9)),
        AnalyzeWinBucket("10-19", entries.Where(item => item.SampleSize is >= 10 and <= 19)),
        AnalyzeWinBucket("20-49", entries.Where(item => item.SampleSize is >= 20 and <= 49)),
        AnalyzeWinBucket("50-99", entries.Where(item => item.SampleSize is >= 50 and <= 99)),
        AnalyzeWinBucket("100+", entries.Where(item => item.SampleSize >= 100))
    };

    var wilsonReference = new[] { 5, 10, 20, 30, 50, 75, 100, 200 }
        .Select(n => new { SampleSize = n, Interval = AnalysisMath.WilsonAtRate(.5, n) }).ToArray();
    var wilsonWidths = entries.Select(item =>
        AnalysisMath.Wilson(item.WinsForA, item.SampleSize).Width).Order().ToArray();
    var wilsonDistribution = AnalysisMath.Describe(wilsonWidths);
    var wilsonByBucket = winBuckets.Select(bucket => new
    {
        bucket.Label,
        Width = AnalysisMath.Describe(bucket.Entries.Select(item =>
            AnalysisMath.Wilson(item.WinsForA, item.SampleSize).Width))
    }).ToArray();

    var levelQuality = new[]
    {
        AnalyzeLevel("L2", entries, item => item.FirstLevel2ForA, item => item.FirstLevel2Unknown),
        AnalyzeLevel("L3", entries, item => item.FirstLevel3ForA, item => item.FirstLevel3Unknown),
        AnalyzeLevel("L6", entries, item => item.FirstLevel6ForA, item => item.FirstLevel6Unknown)
    };
    var roleUnknown = roles.Select(role => new
    {
        role.Role,
        L2 = UnknownRate(role.Entries, item => item.FirstLevel2Unknown),
        L3 = UnknownRate(role.Entries, item => item.FirstLevel3Unknown),
        L6 = UnknownRate(role.Entries, item => item.FirstLevel6Unknown)
    }).ToArray();

    var soloValues = entries.SelectMany(item => new[]
    {
        (double)item.SoloKillsBefore15ForA / item.SampleSize,
        (double)item.SoloKillsBefore15ForB / item.SampleSize
    }).ToArray();
    var soloDistribution = AnalysisMath.Describe(soloValues);
    var soloByBucket = winBuckets.Select(bucket => new
    {
        bucket.Label,
        Distribution = AnalysisMath.Describe(bucket.Entries.SelectMany(item => new[]
        {
            (double)item.SoloKillsBefore15ForA / item.SampleSize,
            (double)item.SoloKillsBefore15ForB / item.SampleSize
        }))
    }).ToArray();
    var soloExtremes = entries.SelectMany(item => new[]
        {
            ToSoloSummary(item, "A", item.SoloKillsBefore15ForA),
            ToSoloSummary(item, "B", item.SoloKillsBefore15ForB)
        }).OrderByDescending(item => item.PerMatch).ThenByDescending(item => item.SampleSize)
        .ThenBy(item => item.Role, StringComparer.Ordinal).Take(20).ToArray();

    var correlations20 = Correlations(entries.Where(item => item.SampleSize >= 20).ToArray(), 20);
    var correlations50 = Correlations(entries.Where(item => item.SampleSize >= 50).ToArray(), 50);
    var directionAgreement20 = DirectionAgreement(entries.Where(item => item.SampleSize >= 20).ToArray(), 20);
    var directionAgreement50 = DirectionAgreement(entries.Where(item => item.SampleSize >= 50).ToArray(), 50);
    var totalSamples = entries.Sum(item => (long)item.SampleSize);
    var canonicalAWinRate = entries.Sum(item => (long)item.WinsForA) / (double)totalSamples;
    var reliabilityBands = new[]
    {
        Band("표본 부족", 1, 4, entries), Band("참고용", 5, 19, entries),
        Band("보통", 20, 49, entries), Band("충분", 50, int.MaxValue, entries)
    };
    var advantageResearch = AdvantageModelResearch.Analyze(
        dataset!,
        manifest?.DatasetRevision,
        entries);
    ProductionOutcomeVerification.Verify(entries, advantageResearch.Recommendation);
    var lanePressureResearch = LanePressureModelResearch.Analyze(
        dataset!, manifest?.DatasetRevision, entries);
    ProductionLaneIndicatorsVerification.Verify(entries);
    var teamSummaryResearch = TeamSummaryModelResearch.Analyze(
        dataset!, manifest?.DatasetRevision, entries);

    var result = new
    {
        Dataset = new { dataset!.Patch, dataset.AcceptedMatches, dataset.Observations,
            MatchupKeys = entries.Length, dataset.TierScope, dataset.TierScopeMethod,
            dataset.AllParticipantsTierVerified, dataset.Region, dataset.QueueId,
            dataset.MinimumGameDurationSeconds, DatasetRevision = manifest?.DatasetRevision },
        PercentileMethod = "linear interpolation at index (N-1)*p",
        SampleSize = sampleDistribution,
        ThresholdCoverage = thresholds,
        LowSampleCounts = lowSamples,
        Roles = roles.Select(role => new { role.Role, role.KeyCount, role.UniqueChampions,
            role.Distribution, Coverage = role.Coverage }).ToArray(),
        TopSamples = topSamples,
        WinRateBuckets = winBuckets.Select(bucket => new { bucket.Label, bucket.Count,
            bucket.Distribution, bucket.Extremes }).ToArray(),
        WilsonReference = wilsonReference,
        WilsonWidthDistribution = wilsonDistribution,
        WilsonWidthsBySampleBucket = wilsonByBucket,
        LevelQuality = levelQuality,
        RoleUnknownRates = roleUnknown,
        SoloKillPerMatchDistribution = soloDistribution,
        SoloKillBySampleBucket = soloByBucket,
        SoloKillExtremes = soloExtremes,
        CorrelationsSample20 = correlations20,
        CorrelationsSample50 = correlations50,
        DirectionAgreementSample20 = directionAgreement20,
        DirectionAgreementSample50 = directionAgreement50,
        CanonicalAWeightedWinRate = canonicalAWinRate,
        ReliabilityCandidate = reliabilityBands,
        CandidateThresholdCoverage = new[] { 5, 10, 20, 30, 50, 100 }.Select(value => Coverage(value, entries)).ToArray()
    };

    Directory.CreateDirectory(outputDirectory);
    await File.WriteAllTextAsync(jsonOutputPath, JsonSerializer.Serialize(result,
        new JsonSerializerOptions { WriteIndented = true }));
    await File.WriteAllTextAsync(markdownOutputPath, BuildMarkdown(dataset, manifest, entries,
        sampleDistribution, thresholds, lowSamples, roles, topSamples, winBuckets,
        wilsonReference, wilsonDistribution, wilsonByBucket, levelQuality, roleUnknown,
        soloDistribution, soloByBucket, soloExtremes, correlations20, correlations50,
        directionAgreement20, directionAgreement50, canonicalAWinRate, reliabilityBands));
    await File.WriteAllTextAsync(
        advantageJsonOutputPath,
        JsonSerializer.Serialize(advantageResearch, AdvantageModelResearch.JsonOptions));
    await File.WriteAllTextAsync(
        advantageMarkdownOutputPath,
        AdvantageModelResearch.BuildMarkdown(advantageResearch));
    await File.WriteAllTextAsync(
        lanePressureJsonOutputPath,
        JsonSerializer.Serialize(lanePressureResearch, LanePressureModelResearch.JsonOptions));
    await File.WriteAllTextAsync(
        lanePressureMarkdownOutputPath,
        LanePressureModelResearch.BuildMarkdown(lanePressureResearch));
    await File.WriteAllTextAsync(
        teamSummaryJsonOutputPath,
        JsonSerializer.Serialize(teamSummaryResearch, TeamSummaryModelResearch.JsonOptions));
    await File.WriteAllTextAsync(
        teamSummaryMarkdownOutputPath,
        TeamSummaryModelResearch.BuildMarkdown(teamSummaryResearch));

    PrintSummary(dataset, manifest, entries.Length, sampleDistribution, thresholds, roles,
        wilsonReference, levelQuality, correlations20, correlations50);
    Console.WriteLine($"JSON report: {jsonOutputPath}");
    Console.WriteLine($"Markdown report: {markdownOutputPath}");
    Console.WriteLine(
        $"Recommended advantage research model: {advantageResearch.Recommendation.ModelId}; "
        + $"directional keys={advantageResearch.Recommendation.DirectionalKeyCoverage:P2}, "
        + $"sample-weighted={advantageResearch.Recommendation.SampleWeightedDirectionalCoverage:P2}");
    Console.WriteLine($"Advantage JSON report: {advantageJsonOutputPath}");
    Console.WriteLine($"Advantage Markdown report: {advantageMarkdownOutputPath}");
    Console.WriteLine(
        $"Lane pressure recommendation: {lanePressureResearch.Recommendation.Decision}; "
        + $"level={lanePressureResearch.Recommendation.LevelModelId}, "
        + $"combat={lanePressureResearch.Recommendation.CombatThreshold:F2}");
    Console.WriteLine($"Lane pressure JSON report: {lanePressureJsonOutputPath}");
    Console.WriteLine($"Lane pressure Markdown report: {lanePressureMarkdownOutputPath}");
    Console.WriteLine(
        $"Team summary recommendation: {teamSummaryResearch.Recommendation.FinalDecision}; "
        + $"model={teamSummaryResearch.Recommendation.RecommendedModelId}");
    Console.WriteLine($"Team summary JSON report: {teamSummaryJsonOutputPath}");
    Console.WriteLine($"Team summary Markdown report: {teamSummaryMarkdownOutputPath}");
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
    or JsonException or InvalidDataException)
{
    Console.Error.WriteLine($"Analysis failed: {exception.Message}");
    Environment.ExitCode = 1;
}

static object ToMatchupSummary(StatisticsMatchupEntry item) => new
    { item.Role, item.ChampionA, item.ChampionB, item.SampleSize };
static SoloSummary ToSoloSummary(StatisticsMatchupEntry item, string side, int events) => new(
    item.Role, item.ChampionA, item.ChampionB, item.SampleSize, side,
    events, (double)events / item.SampleSize);
static object Coverage(int threshold, IReadOnlyCollection<StatisticsMatchupEntry> entries)
{
    var count = entries.Count(item => item.SampleSize >= threshold);
    return new { Threshold = threshold, Count = count, Rate = (double)count / entries.Count };
}

static RoleAnalysis AnalyzeRole(string role, StatisticsMatchupEntry[] all)
{
    var entries = all.Where(item => item.Role == role).ToArray();
    return new RoleAnalysis(role, entries.Length,
        entries.SelectMany(item => new[] { item.ChampionA, item.ChampionB })
            .Distinct(StringComparer.OrdinalIgnoreCase).Count(),
        AnalysisMath.Describe(entries.Select(item => (double)item.SampleSize)),
        new[] { 5, 10, 20, 50 }.ToDictionary(value => value,
            value => entries.Count(item => item.SampleSize >= value)), entries);
}

static WinBucket AnalyzeWinBucket(string label, IEnumerable<StatisticsMatchupEntry> source)
{
    var entries = source.ToArray();
    var rates = entries.Select(item => (double)item.WinsForA / item.SampleSize).ToArray();
    object Frequency(Func<StatisticsMatchupEntry, bool> predicate)
    {
        var count = entries.Count(predicate);
        return new { Count = count, Rate = entries.Length == 0 ? 0 : (double)count / entries.Length };
    }
    return new WinBucket(label, entries.Length, AnalysisMath.Describe(rates), new
    {
        AtMost30 = Frequency(item => (double)item.WinsForA / item.SampleSize <= .3),
        AtMost40 = Frequency(item => (double)item.WinsForA / item.SampleSize <= .4),
        AtLeast60 = Frequency(item => (double)item.WinsForA / item.SampleSize >= .6),
        AtLeast70 = Frequency(item => (double)item.WinsForA / item.SampleSize >= .7)
    }, entries);
}

static object AnalyzeLevel(string label, StatisticsMatchupEntry[] entries,
    Func<StatisticsMatchupEntry, int> forA, Func<StatisticsMatchupEntry, int> unknown)
{
    var totalSamples = entries.Sum(item => (long)item.SampleSize);
    var totalUnknown = entries.Sum(item => (long)unknown(item));
    object Top(StatisticsMatchupEntry item) => new { item.Role, item.ChampionA, item.ChampionB,
        item.SampleSize, Unknown = unknown(item), UnknownRate = (double)unknown(item) / item.SampleSize };
    return new
    {
        Label = label, UnknownTotal = totalUnknown, UnknownRate = totalUnknown / (double)totalSamples,
        ASideRate = AnalysisMath.Describe(entries.Select(item => (double)forA(item) / item.SampleSize)),
        HighestUnknownAll = entries.OrderByDescending(item => (double)unknown(item) / item.SampleSize)
            .ThenByDescending(item => item.SampleSize).Take(10).Select(Top).ToArray(),
        HighestUnknownSample10 = entries.Where(item => item.SampleSize >= 10)
            .OrderByDescending(item => (double)unknown(item) / item.SampleSize)
            .ThenByDescending(item => item.SampleSize).Take(10).Select(Top).ToArray()
    };
}

static double UnknownRate(IEnumerable<StatisticsMatchupEntry> entries,
    Func<StatisticsMatchupEntry, int> selector)
{
    var values = entries.ToArray();
    var samples = values.Sum(item => (long)item.SampleSize);
    return samples == 0 ? 0 : values.Sum(item => (long)selector(item)) / (double)samples;
}

static object Correlations(StatisticsMatchupEntry[] entries, int minimumSample)
{
    var vectors = new Dictionary<string, double[]>
    {
        ["WinRateA"] = entries.Select(item => (double)item.WinsForA / item.SampleSize).ToArray(),
        ["FirstLevel2RateA"] = entries.Select(item => (double)item.FirstLevel2ForA / item.SampleSize).ToArray(),
        ["FirstLevel3RateA"] = entries.Select(item => (double)item.FirstLevel3ForA / item.SampleSize).ToArray(),
        ["FirstLevel6RateA"] = entries.Select(item => (double)item.FirstLevel6ForA / item.SampleSize).ToArray(),
        ["SoloKillDifference"] = entries.Select(item =>
            (double)(item.SoloKillsBefore15ForA - item.SoloKillsBefore15ForB) / item.SampleSize).ToArray()
    };
    var pairs = new SortedDictionary<string, double>(StringComparer.Ordinal);
    var names = vectors.Keys.ToArray();
    for (var first = 0; first < names.Length; first++)
    for (var second = first + 1; second < names.Length; second++)
        pairs[$"{names[first]} vs {names[second]}"] =
            AnalysisMath.Pearson(vectors[names[first]], vectors[names[second]]);
    return new { MinimumSample = minimumSample, MatchupCount = entries.Length, Pairs = pairs };
}

static object DirectionAgreement(StatisticsMatchupEntry[] entries, int minimumSample)
{
    var favorable = entries.Where(item => (double)item.WinsForA / item.SampleSize > .5).ToArray();
    object Metric(string name, Func<StatisticsMatchupEntry, double> centeredValue)
    {
        var same = favorable.Count(item => centeredValue(item) > 0);
        var opposite = favorable.Count(item => centeredValue(item) < 0);
        var neutral = favorable.Length - same - opposite;
        return new { Metric = name, SameDirection = same, OppositeDirection = opposite, Neutral = neutral,
            AgreementAmongNonNeutral = same + opposite == 0 ? 0 : (double)same / (same + opposite) };
    }

    return new
    {
        MinimumSample = minimumSample,
        WinRateAAbove50Count = favorable.Length,
        Metrics = new[]
        {
            Metric("FirstLevel2RateA", item => (double)item.FirstLevel2ForA / item.SampleSize - .5),
            Metric("FirstLevel3RateA", item => (double)item.FirstLevel3ForA / item.SampleSize - .5),
            Metric("FirstLevel6RateA", item => (double)item.FirstLevel6ForA / item.SampleSize - .5),
            Metric("SoloKillDifference", item =>
                (double)(item.SoloKillsBefore15ForA - item.SoloKillsBefore15ForB) / item.SampleSize)
        }
    };
}

static object Band(string label, int minimum, int maximum, StatisticsMatchupEntry[] entries)
{
    var count = entries.Count(item => item.SampleSize >= minimum && item.SampleSize <= maximum);
    return new { Label = label, Minimum = minimum, Maximum = maximum == int.MaxValue ? (int?)null : maximum,
        Count = count, Rate = (double)count / entries.Length };
}

static async Task<DatasetUpdateManifest?> LoadManifestAsync(string path)
{
    if (!File.Exists(path)) return null;
    try { return JsonSerializer.Deserialize<DatasetUpdateManifest>(await File.ReadAllBytesAsync(path)); }
    catch (JsonException) { return null; }
}

static string BuildMarkdown(StatisticsDataset dataset, DatasetUpdateManifest? manifest,
    StatisticsMatchupEntry[] entries, Distribution samples, object[] thresholds,
    Dictionary<string, int> lowSamples, RoleAnalysis[] roles, object[] topSamples,
    WinBucket[] winBuckets, object[] wilsonReference, Distribution wilsonDistribution,
    object[] wilsonByBucket, object[] levelQuality, object[] roleUnknown,
    Distribution soloDistribution, object[] soloByBucket, object[] soloExtremes,
    object correlations20, object correlations50, object directionAgreement20,
    object directionAgreement50, double canonicalAWinRate, object[] bands)
{
    var sb = new StringBuilder();
    sb.AppendLine("# Statistics Reliability Analysis").AppendLine();
    sb.AppendLine("## Dataset Summary").AppendLine();
    sb.AppendLine($"- Patch: `{dataset.Patch}`");
    sb.AppendLine($"- Accepted matches: {dataset.AcceptedMatches:N0}");
    sb.AppendLine($"- Observations: {dataset.Observations:N0}");
    sb.AppendLine($"- Matchup keys: {entries.Length:N0}");
    sb.AppendLine($"- Tier scope: `{dataset.TierScope}` / `{dataset.TierScopeMethod}`");
    sb.AppendLine($"- All participants tier verified: `{dataset.AllParticipantsTierVerified.ToString().ToLowerInvariant()}`");
    sb.AppendLine($"- Queue: {dataset.QueueId}; minimum duration: {dataset.MinimumGameDurationSeconds}s");
    sb.AppendLine($"- Dataset revision: {(manifest?.DatasetRevision.ToString() ?? "not available")}").AppendLine();

    sb.AppendLine("## Sample Size Distribution").AppendLine();
    sb.AppendLine("Percentiles use linear interpolation at index `(N-1)*p`.").AppendLine();
    sb.AppendLine($"Min {samples.Min:N0}; mean {samples.Mean:F2}; p25 {samples.P25:F2}; median/p50 {samples.Median:F2}; p75 {samples.P75:F2}; p90 {samples.P90:F2}; p95 {samples.P95:F2}; p99 {samples.P99:F2}; max {samples.Max:N0}.").AppendLine();
    sb.AppendLine("Low-sample keys: " + string.Join(", ", lowSamples.Select(item => $"n={item.Key}: {item.Value:N0}")) + ".").AppendLine();
    AppendJsonTable(sb, thresholds);

    sb.AppendLine("## Role Distribution").AppendLine();
    sb.AppendLine("| Role | Keys | Champions | Mean | Median | P75 | P90 | Max | >=5 | >=10 | >=20 | >=50 |");
    sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
    foreach (var role in roles)
    {
        string CoverageText(int threshold) =>
            $"{role.Coverage[threshold]:N0} ({(double)role.Coverage[threshold] / role.KeyCount:P1})";
        sb.AppendLine($"| {role.Role} | {role.KeyCount:N0} | {role.UniqueChampions:N0} | {role.Distribution.Mean:F2} | {role.Distribution.Median:F1} | {role.Distribution.P75:F1} | {role.Distribution.P90:F1} | {role.Distribution.Max:N0} | {CoverageText(5)} | {CoverageText(10)} | {CoverageText(20)} | {CoverageText(50)} |");
    }
    sb.AppendLine();

    sb.AppendLine("## Highest Sample Matchups").AppendLine(); AppendJsonTable(sb, topSamples);
    sb.AppendLine("## Win Rate Distribution").AppendLine();
    sb.AppendLine("Small-sample extreme rates are descriptive sampling behavior, not matchup strength labels.").AppendLine();
    AppendJsonTable(sb, winBuckets.Select(item => new { item.Label, item.Count, item.Distribution, item.Extremes }).ToArray());

    sb.AppendLine("## Wilson Confidence Analysis").AppendLine();
    sb.AppendLine("95% Wilson score intervals are used; no production confidence label is applied.").AppendLine();
    AppendJsonTable(sb, wilsonReference);
    sb.AppendLine($"Actual interval-width distribution: median {wilsonDistribution.Median:P2}, p75 {wilsonDistribution.P75:P2}, p90 {wilsonDistribution.P90:P2}, p95 {wilsonDistribution.P95:P2}.").AppendLine();
    AppendJsonTable(sb, wilsonByBucket);

    sb.AppendLine("## Level Metric Quality").AppendLine(); AppendJsonTable(sb, levelQuality);
    sb.AppendLine("Role-level Unknown rates:").AppendLine(); AppendJsonTable(sb, roleUnknown);
    sb.AppendLine("Level rates use A count / full SampleSize, matching the production calculator; Unknown is not removed from the denominator.").AppendLine();

    sb.AppendLine("## Solo Kill Distribution").AppendLine();
    sb.AppendLine("Solo kills are event counts. Per-match values are events / SampleSize and may exceed 1.").AppendLine();
    sb.AppendLine($"Combined A/B per-match distribution: mean {soloDistribution.Mean:F4}, median {soloDistribution.Median:F4}, p75 {soloDistribution.P75:F4}, p90 {soloDistribution.P90:F4}, p95 {soloDistribution.P95:F4}, max {soloDistribution.Max:F4}.").AppendLine();
    AppendJsonTable(sb, soloByBucket); sb.AppendLine("Top per-match entries:").AppendLine(); AppendJsonTable(sb, soloExtremes);

    sb.AppendLine("## Metric Correlations").AppendLine();
    sb.AppendLine("Pearson correlations describe association across canonical matchup aggregates; they do not establish causation.").AppendLine();
    AppendJsonTable(sb, new[] { correlations20, correlations50 });
    sb.AppendLine("Directional agreement below is restricted to matchups where A win rate is above 50%; neutral metric values are reported separately.").AppendLine();
    AppendJsonTable(sb, new[] { directionAgreement20, directionAgreement50 });
    sb.AppendLine($"Canonical A weighted win rate: {canonicalAWinRate:P2}. ChampionA is lexical, so deviation from 50% is monitored as a normalization sanity check, not assumed to be causal.").AppendLine();

    sb.AppendLine("## Candidate Reliability Thresholds").AppendLine();
    sb.AppendLine("Candidate only; not applied to the WPF application.").AppendLine(); AppendJsonTable(sb, bands);
    sb.AppendLine("The proposed exploratory bands are `<5`, `5-19`, `20-49`, and `50+`. They jointly expose coverage and Wilson-width tradeoffs without deleting rare/off-meta matchups. Stage 9-1 should use these as display reliability labels only, retaining raw statistics and a clear low-sample warning.").AppendLine();
    sb.AppendLine("This dataset is MASTER_PLUS_SEEDED: seeds were Master/Grandmaster/Challenger at collection time; all ten participants were not tier-verified. Results are patch-specific and are not merged with other patches.");
    return sb.ToString();
}

static void AppendJsonTable(StringBuilder sb, object value) =>
    sb.AppendLine("```json").AppendLine(JsonSerializer.Serialize(value,
        new JsonSerializerOptions { WriteIndented = true })).AppendLine("```").AppendLine();

static void PrintSummary(StatisticsDataset dataset, DatasetUpdateManifest? manifest, int keys,
    Distribution samples, object[] thresholds, RoleAnalysis[] roles, object[] wilson,
    object[] levels, object correlations20, object correlations50)
{
    Console.WriteLine($"Patch: {dataset.Patch}");
    Console.WriteLine($"Accepted matches: {dataset.AcceptedMatches:N0}");
    Console.WriteLine($"Observations: {dataset.Observations:N0}");
    Console.WriteLine($"Matchup keys: {keys:N0}");
    Console.WriteLine($"Tier scope: {dataset.TierScope}");
    Console.WriteLine($"Queue: {dataset.QueueId}");
    Console.WriteLine($"Minimum duration: {dataset.MinimumGameDurationSeconds} seconds");
    Console.WriteLine($"Dataset revision: {manifest?.DatasetRevision.ToString() ?? "not available"}");
    Console.WriteLine($"Sample size min/mean/median/p75/p90/p95/max: {samples.Min:F0} / {samples.Mean:F2} / {samples.Median:F2} / {samples.P75:F2} / {samples.P90:F2} / {samples.P95:F2} / {samples.Max:F0}");
    Console.WriteLine("Role summary:");
    foreach (var role in roles) Console.WriteLine($"  {role.Role}: keys={role.KeyCount:N0}, median={role.Distribution.Median:F1}, >=10={role.Coverage[10]:N0}, >=20={role.Coverage[20]:N0}, >=50={role.Coverage[50]:N0}");
    Console.WriteLine("Wilson reference and detailed distributions written to reports.");
    Console.WriteLine("Level Unknown and correlation results written to reports.");
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "LoLMatchup.csproj"))) directory = directory.Parent;
    return directory?.FullName ?? throw new DirectoryNotFoundException("LoLMatchup repository root was not found.");
}

internal sealed record RoleAnalysis(string Role, int KeyCount, int UniqueChampions,
    Distribution Distribution, Dictionary<int, int> Coverage, StatisticsMatchupEntry[] Entries);
internal sealed record WinBucket(string Label, int Count, Distribution Distribution,
    object Extremes, StatisticsMatchupEntry[] Entries);
internal sealed record SoloSummary(string Role, string ChampionA, string ChampionB,
    int SampleSize, string Side, int Events, double PerMatch);
