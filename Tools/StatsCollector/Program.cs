using StatsCollector.Api;
using StatsCollector.Analysis;
using StatsCollector.Aggregation;
using StatsCollector.Collection;
using StatsCollector.Configuration;
using StatsCollector.Storage;

Console.WriteLine("LoL Matchup StatsCollector");
Console.WriteLine("Stage 5 collector ready.");

var targetPatchText = Environment.GetEnvironmentVariable(
    CollectorOptions.TargetPatchEnvironmentVariableName);
var targetAcceptedText = Environment.GetEnvironmentVariable(
    CollectorOptions.TargetAcceptedMatchesEnvironmentVariableName);
var autoContinueText = Environment.GetEnvironmentVariable(
    CollectorOptions.AutoContinueCollectionEnvironmentVariableName);
var minimumGameDurationText = Environment.GetEnvironmentVariable(
    CollectorOptions.MinimumGameDurationSecondsEnvironmentVariableName);

if (!PatchVersionParser.TryParseTargetPatch(targetPatchText, out var targetPatch))
{
    Console.WriteLine("TARGET_PATCH must use the major.minor format.");
    return;
}

if (!int.TryParse(targetAcceptedText, out var targetAcceptedMatches)
    || targetAcceptedMatches <= 0)
{
    Console.WriteLine("TARGET_ACCEPTED_MATCHES must be a positive integer.");
    return;
}

if (!string.IsNullOrWhiteSpace(autoContinueText)
    && !bool.TryParse(autoContinueText, out _))
{
    Console.WriteLine("AUTO_CONTINUE_COLLECTION must be true or false.");
    return;
}

var minimumGameDurationSeconds =
    CollectorOptions.DefaultMinimumGameDurationSeconds;

if (!string.IsNullOrWhiteSpace(minimumGameDurationText)
    && (!int.TryParse(minimumGameDurationText, out minimumGameDurationSeconds)
        || minimumGameDurationSeconds <= 0))
{
    Console.WriteLine("MIN_GAME_DURATION_SECONDS must be a positive integer.");
    return;
}

var autoContinue = bool.TryParse(autoContinueText, out var parsedAutoContinue)
    && parsedAutoContinue;
var options = new CollectorOptions
{
    TargetPatch = targetPatch.ToString(),
    TargetAcceptedMatchCount = targetAcceptedMatches,
    AutoContinueCollection = autoContinue,
    MinimumGameDurationSeconds = minimumGameDurationSeconds
};
var accumulator = new MatchupAccumulator();
var stateStore = new CollectorStateStore(options);
var datasetWriter = new DatasetWriter(options);
CollectorStateRestoreSummary restore;

try
{
    restore = stateStore.Restore(accumulator.AddRange);
}
catch (CollectorStateCorruptionException exception)
{
    Console.WriteLine($"State restore failed: {exception.Message}");
    return;
}

Console.WriteLine($"Target patch: {options.TargetPatch}");
Console.WriteLine(
    $"Minimum game duration: {options.MinimumGameDurationSeconds} seconds "
    + $"({options.DurationPolicyKey})");
Console.WriteLine();
Console.WriteLine("State restore");
Console.WriteLine($"Discovered match IDs loaded: {restore.DiscoveredMatchCount}");
Console.WriteLine($"Terminal results loaded: {restore.TerminalResultCount}");
Console.WriteLine($"Accepted matches replayed: {restore.AcceptedMatchCount}");
Console.WriteLine($"Observations replayed: {restore.ObservationCount}");
Console.WriteLine($"Duplicate terminal records: {restore.DuplicateTerminalRecordCount}");

if (restore.IgnoredIncompleteLastLineCount > 0)
{
    Console.WriteLine(
        "Incomplete final JSONL lines recovered: "
        + restore.IgnoredIncompleteLastLineCount);
}

Console.WriteLine();
Console.WriteLine("Auto collection");
Console.WriteLine($"Enabled: {options.AutoContinueCollection}");
Console.WriteLine($"Target accepted matches: {options.TargetAcceptedMatchCount}");
Console.WriteLine($"Starting accepted matches: {stateStore.AcceptedMatchCount}");
Console.WriteLine($"Batch processing limit: {options.ProcessingMatchLimit}");

if (stateStore.AcceptedMatchCount >= options.TargetAcceptedMatchCount)
{
    PrintAutoCollectionSummary(
        CollectionStopReason.TargetReached,
        0,
        stateStore.AcceptedMatchCount,
        stateStore);
    PrintPersistentQuality(accumulator, stateStore);
    await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);
    return;
}

var apiKey = new RiotApiKeyProvider().GetApiKey();

if (apiKey is null)
{
    Console.WriteLine("Riot API Key is not configured.");
    Console.WriteLine(
        "Set the RIOT_API_KEY environment variable before using the collector.");
    PrintPersistentQuality(accumulator, stateStore);
    await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);
    return;
}

Console.WriteLine("Riot API Key: configured");
using var riotApiClient = new RiotApiClient(options, apiKey);
using var shutdown = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdown.Cancel();
};

var runner = new CollectionRunner(riotApiClient, options, stateStore, accumulator);
CollectionRunResult run;

try
{
    run = await runner.RunAsync(shutdown.Token);
}
catch (Exception exception) when (exception is IOException
    or UnauthorizedAccessException
    or CollectorStateCorruptionException)
{
    Console.WriteLine($"Collection stopped by a fatal state error: {exception.Message}");
    PrintAutoCollectionSummary(
        CollectionStopReason.FatalError,
        0,
        restore.AcceptedMatchCount,
        stateStore);
    PrintPersistentQuality(accumulator, stateStore);
    PrintRetrySummary(riotApiClient.RetryDiagnostics, 0);
    await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);
    return;
}

PrintAutoCollectionSummary(
    run.StopReason,
    run.BatchesCompleted,
    run.AcceptedAtStart,
    stateStore);

Console.WriteLine();
Console.WriteLine("Session summary");
Console.WriteLine($"Matches attempted this run: {run.MatchesAttempted}");
Console.WriteLine($"Matches accepted this run: {run.MatchesAccepted}");
Console.WriteLine($"Matches rejected this run: {run.MatchesRejected}");
Console.WriteLine($"API failures this run: {run.ApiFailures}");
Console.WriteLine($"Observations added this run: {run.ObservationsAdded}");
Console.WriteLine(
    "Observations count valid: "
    + (run.ObservationsAdded == run.MatchesAccepted * 5));

Console.WriteLine();
Console.WriteLine("Session validation quality");
var sessionTerminalCount = run.MatchesAccepted + run.MatchesRejected;

foreach (var outcome in Enum.GetValues<MatchValidationOutcome>())
{
    var count = run.SessionValidationCounts[outcome];
    Console.WriteLine(
        $"{outcome}: {count} "
        + $"({GetPercentage(count, sessionTerminalCount):F2}%)");
}

PrintPersistentQuality(accumulator, stateStore);

Console.WriteLine();
Console.WriteLine("Level metric diagnostics (this run)");
PrintUnknownReasons("Level 2 unknown", run.Level2UnknownReasons);
PrintUnknownReasons("Level 3 unknown", run.Level3UnknownReasons);
PrintUnknownReasons("Level 6 unknown", run.Level6UnknownReasons);

Console.WriteLine();
Console.WriteLine("Assist semantics diagnostic (this run)");
Console.WriteLine(
    "Solo-kill candidate events with null/empty assists: "
    + run.AssistFreeCandidateCount);
Console.WriteLine(
    "Solo-kill candidate events rejected because assists exist: "
    + run.AssistedCandidateCount);
Console.WriteLine($"Kill events with 1+ assists: {run.KillEventsWithAssists}");
Console.WriteLine(
    $"Kill events with null/missing assist list: {run.KillEventsWithNullAssistList}");
Console.WriteLine(
    $"Kill events with empty assist list: {run.KillEventsWithEmptyAssistList}");

Console.WriteLine();
Console.WriteLine("Session game duration");

if (run.AcceptedGameDurations.Count == 0)
{
    Console.WriteLine("No accepted matches this run.");
}
else
{
    Console.WriteLine($"Min: {FormatDuration(run.AcceptedGameDurations.Min())}");
    Console.WriteLine(
        $"Average: {FormatDuration((long)run.AcceptedGameDurations.Average())}");
    Console.WriteLine($"Max: {FormatDuration(run.AcceptedGameDurations.Max())}");
}

PrintRetrySummary(
    riotApiClient.RetryDiagnostics,
    run.TimelineRequestsAvoidedByValidation);
await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);

static void PrintAutoCollectionSummary(
    CollectionStopReason stopReason,
    int batchesCompleted,
    int acceptedAtStart,
    CollectorStateStore stateStore)
{
    var pending = stateStore.DiscoveredMatchIds.Count
        - stateStore.TerminalMatchIds.Count;

    Console.WriteLine();
    Console.WriteLine("Auto collection summary");
    Console.WriteLine($"Stop reason: {stopReason}");
    Console.WriteLine($"Batches completed: {batchesCompleted}");
    Console.WriteLine($"Accepted at start: {acceptedAtStart}");
    Console.WriteLine($"Accepted at end: {stateStore.AcceptedMatchCount}");
    Console.WriteLine($"Accepted added: {stateStore.AcceptedMatchCount - acceptedAtStart}");
    Console.WriteLine($"Terminal matches: {stateStore.TerminalMatchIds.Count}");
    Console.WriteLine($"Pending matches: {pending}");
    Console.WriteLine(
        $"State storage size: {stateStore.GetStorageSizeBytes() / (1024d * 1024d):F2} MB");
}

static void PrintPersistentQuality(
    MatchupAccumulator accumulator,
    CollectorStateStore stateStore)
{
    var aggregates = accumulator.Aggregates.Values.ToList();
    var observationCount = stateStore.ObservationCount;
    var pending = stateStore.DiscoveredMatchIds.Count
        - stateStore.TerminalMatchIds.Count;

    Console.WriteLine();
    Console.WriteLine("Persistent totals");
    Console.WriteLine($"Discovered matches: {stateStore.DiscoveredMatchIds.Count}");
    Console.WriteLine($"Terminal matches: {stateStore.TerminalMatchIds.Count}");
    Console.WriteLine($"Accepted matches: {stateStore.AcceptedMatchCount}");
    Console.WriteLine($"Rejected matches: {stateStore.RejectedMatchCount}");
    Console.WriteLine($"Observations: {observationCount}");
    Console.WriteLine($"Aggregated matchup keys: {aggregates.Count}");
    Console.WriteLine($"Pending matches: {pending}");
    Console.WriteLine(
        "Observations count valid: "
        + (observationCount == stateStore.AcceptedMatchCount * 5));
    Console.WriteLine($"Invariants valid: {accumulator.InvariantsValid}");

    Console.WriteLine();
    Console.WriteLine("Metric quality");
    PrintMetricQuality(
        "First level 2 unknown",
        aggregates.Sum(value => value.FirstLevel2Unknown),
        observationCount);
    PrintMetricQuality(
        "First level 3 unknown",
        aggregates.Sum(value => value.FirstLevel3Unknown),
        observationCount);
    PrintMetricQuality(
        "First level 6 unknown",
        aggregates.Sum(value => value.FirstLevel6Unknown),
        observationCount);

    Console.WriteLine();
    Console.WriteLine("Persistent validation quality");

    foreach (var outcome in Enum.GetValues<MatchValidationOutcome>())
    {
        var count = stateStore.OutcomeCounts[outcome];
        Console.WriteLine(
            $"{outcome}: {count} "
            + $"({GetPercentage(count, stateStore.TerminalMatchIds.Count):F2}%)");
    }

    var roleCounts = RoleNormalizer.StandardRoles.ToDictionary(
        role => role,
        role => aggregates
            .Where(value => value.Key.Role == role)
            .Sum(value => value.SampleSize));
    Console.WriteLine();
    Console.WriteLine("Role observation counts");

    foreach (var role in RoleNormalizer.StandardRoles)
    {
        Console.WriteLine($"{RoleNormalizer.ToDisplayName(role)}: {roleCounts[role]}");
    }

    Console.WriteLine(
        "Role observation counts valid: "
        + roleCounts.Values.All(count => count == stateStore.AcceptedMatchCount));

    Console.WriteLine();
    Console.WriteLine("Matchup sample distribution");
    Console.WriteLine($"Total keys: {aggregates.Count}");

    foreach (var threshold in new[] { 2, 3, 5, 10, 20 })
    {
        Console.WriteLine(
            $"Sample >= {threshold}: "
            + aggregates.Count(value => value.SampleSize >= threshold));
    }

    Console.WriteLine(
        "Maximum sample size: "
        + aggregates.Select(value => value.SampleSize).DefaultIfEmpty().Max());
    PrintChampionRoleAppearances(aggregates);
}

static void PrintChampionRoleAppearances(IReadOnlyList<MatchupAggregate> aggregates)
{
    var appearances = new Dictionary<(StandardRole Role, string Champion), int>();

    foreach (var aggregate in aggregates)
    {
        AddAppearance(appearances, aggregate.Key.Role, aggregate.Key.ChampionA, aggregate.SampleSize);
        AddAppearance(appearances, aggregate.Key.Role, aggregate.Key.ChampionB, aggregate.SampleSize);
    }

    Console.WriteLine();
    Console.WriteLine("Champion-role appearances");
    Console.WriteLine($"Distinct champion-role keys: {appearances.Count}");

    foreach (var appearance in appearances
                 .OrderByDescending(item => item.Value)
                 .ThenBy(item => item.Key.Role)
                 .ThenBy(item => item.Key.Champion, StringComparer.OrdinalIgnoreCase)
                 .ThenBy(item => item.Key.Champion, StringComparer.Ordinal)
                 .Take(10))
    {
        Console.WriteLine(
            $"{RoleNormalizer.ToDisplayName(appearance.Key.Role)} "
            + $"{appearance.Key.Champion}: {appearance.Value}");
    }
}

static void PrintRetrySummary(
    RiotApiRetryDiagnostics diagnostics,
    int timelineRequestsAvoided)
{
    Console.WriteLine();
    Console.WriteLine("API retry summary");
    Console.WriteLine($"Logical API requests: {diagnostics.LogicalApiRequests}");
    Console.WriteLine($"HTTP attempts: {diagnostics.HttpAttempts}");
    Console.WriteLine($"429 retries: {diagnostics.RetriesDueToRateLimit}");
    Console.WriteLine($"5xx retries: {diagnostics.RetriesDueToServerError}");
    Console.WriteLine($"Network/timeout retries: {diagnostics.RetriesDueToNetworkOrTimeout}");
    Console.WriteLine($"Retry wait: {diagnostics.TotalRetryWait.TotalSeconds:F1} seconds");
    Console.WriteLine($"League requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.League)}");
    Console.WriteLine($"Summoner requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.Summoner)}");
    Console.WriteLine($"Match ID requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.MatchIds)}");
    Console.WriteLine($"Match Detail requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.MatchDetail)}");
    Console.WriteLine($"Timeline requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.Timeline)}");
    Console.WriteLine($"Timeline requests avoided by validation: {timelineRequestsAvoided}");

    if (diagnostics.ApplicationRateLimit is not null)
    {
        Console.WriteLine($"Observed app rate limit: {diagnostics.ApplicationRateLimit}");
        Console.WriteLine(
            $"Observed app rate count: {diagnostics.ApplicationRateLimitCount ?? "not provided"}");
    }

    if (diagnostics.MethodRateLimit is not null)
    {
        Console.WriteLine($"Observed method rate limit: {diagnostics.MethodRateLimit}");
        Console.WriteLine(
            $"Observed method rate count: {diagnostics.MethodRateLimitCount ?? "not provided"}");
    }
}

static void PrintUnknownReasons(string label, MilestoneUnknownReasonCounts counts)
{
    Console.WriteLine(label + ":");
    Console.WriteLine($"  Tie: {counts.Tie}");
    Console.WriteLine($"  Missing A: {counts.MissingA}");
    Console.WriteLine($"  Missing B: {counts.MissingB}");
    Console.WriteLine($"  Missing both: {counts.MissingBoth}");
}

static void PrintMetricQuality(string label, int unknownCount, int totalCount)
{
    Console.WriteLine(
        $"{label}: {unknownCount} / {totalCount} "
        + $"({GetPercentage(unknownCount, totalCount):F2}%)");
}

static double GetPercentage(int count, int total) =>
    total == 0 ? 0 : count * 100d / total;

static string FormatDuration(long totalSeconds)
{
    var duration = TimeSpan.FromSeconds(totalSeconds);
    return $"{(int)duration.TotalMinutes}m {duration.Seconds:D2}s";
}

static void AddAppearance(
    Dictionary<(StandardRole Role, string Champion), int> appearances,
    StandardRole role,
    string champion,
    int count)
{
    var key = (role, champion);
    appearances[key] = appearances.GetValueOrDefault(key) + count;
}

static async Task WriteDatasetAndPrintAsync(
    DatasetWriter writer,
    MatchupAccumulator accumulator,
    CollectorStateStore stateStore)
{
    try
    {
        var result = await writer.WriteAsync(
            accumulator,
            stateStore.AcceptedMatchCount,
            stateStore.ObservationCount);

        Console.WriteLine();
        Console.WriteLine("Dataset output");
        Console.WriteLine($"Path: {result.RelativePath}");
        Console.WriteLine($"Accepted matches: {result.AcceptedMatches}");
        Console.WriteLine($"Observations: {result.Observations}");
        Console.WriteLine($"Matchup keys: {result.MatchupKeys}");
        Console.WriteLine($"File size: {result.FileSizeBytes / (1024d * 1024d):F2} MB");
    }
    catch (Exception exception) when (exception is IOException
        or UnauthorizedAccessException
        or InvalidOperationException)
    {
        Console.WriteLine($"Dataset output failed: {exception.Message}");
    }
}
