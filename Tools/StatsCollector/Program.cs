using StatsCollector.Api;
using StatsCollector.Analysis;
using StatsCollector.Aggregation;
using StatsCollector.Collection;
using StatsCollector.Configuration;
using StatsCollector.Storage;

Console.WriteLine("LoL Matchup StatsCollector");
Console.WriteLine("Stage 5 collector scaffold ready.");

var targetPatch = Environment.GetEnvironmentVariable(
    CollectorOptions.TargetPatchEnvironmentVariableName);
var targetAcceptedMatchesText = Environment.GetEnvironmentVariable(
    CollectorOptions.TargetAcceptedMatchesEnvironmentVariableName);

if (!PatchVersionParser.TryParseTargetPatch(targetPatch, out var parsedTargetPatch))
{
    Console.WriteLine(
        $"{CollectorOptions.TargetPatchEnvironmentVariableName} is not configured "
        + "or does not use the major.minor format.");
    Console.WriteLine("Set the target patch before using the collector.");
    return;
}

if (!int.TryParse(targetAcceptedMatchesText, out var targetAcceptedMatches)
    || targetAcceptedMatches <= 0)
{
    Console.WriteLine(
        $"{CollectorOptions.TargetAcceptedMatchesEnvironmentVariableName} "
        + "is not configured or is not a positive integer.");
    Console.WriteLine("Set the persistent accepted Match target before collecting.");
    return;
}

var options = new CollectorOptions
{
    TargetPatch = parsedTargetPatch.ToString(),
    TargetAcceptedMatchCount = targetAcceptedMatches
};

Console.WriteLine($"Target patch: {options.TargetPatch}");
Console.WriteLine(
    $"Target accepted matches: {options.TargetAcceptedMatchCount}");

var accumulator = new MatchupAccumulator();
var stateStore = new CollectorStateStore(options);
var datasetWriter = new DatasetWriter(options);
CollectorStateRestoreSummary restoreSummary;

try
{
    restoreSummary = stateStore.Restore(accumulator.AddRange);
}
catch (CollectorStateCorruptionException exception)
{
    Console.WriteLine($"State restore failed: {exception.Message}");
    return;
}

Console.WriteLine();
Console.WriteLine("State restore");
Console.WriteLine(
    $"Discovered match IDs loaded: {restoreSummary.DiscoveredMatchCount}");
Console.WriteLine(
    $"Terminal results loaded: {restoreSummary.TerminalResultCount}");
Console.WriteLine(
    $"Accepted matches replayed: {restoreSummary.AcceptedMatchCount}");
Console.WriteLine($"Observations replayed: {restoreSummary.ObservationCount}");
Console.WriteLine(
    $"Duplicate terminal records: {restoreSummary.DuplicateTerminalRecordCount}");

if (restoreSummary.IgnoredIncompleteLastLineCount > 0)
{
    Console.WriteLine(
        "Incomplete final JSONL lines recovered: "
        + restoreSummary.IgnoredIncompleteLastLineCount);
}

var targetReached = stateStore.AcceptedMatchCount
    >= options.TargetAcceptedMatchCount;
Console.WriteLine($"Current accepted matches: {stateStore.AcceptedMatchCount}");
Console.WriteLine($"Target reached: {targetReached}");

if (targetReached)
{
    await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);
    return;
}

var apiKeyProvider = new RiotApiKeyProvider();
var apiKey = apiKeyProvider.GetApiKey();

if (apiKey is null)
{
    Console.WriteLine("Riot API Key is not configured.");
    Console.WriteLine(
        "Set the RIOT_API_KEY environment variable before using the collector.");
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

var seedProvider = new SeedProvider(riotApiClient);
var seedResult = await seedProvider.CollectMasterPlusAsync(shutdown.Token);

Console.WriteLine("MASTER_PLUS seed collection");

foreach (var tierResult in seedResult.TierResults)
{
    if (tierResult.Status == RiotApiRequestStatus.Success)
    {
        Console.WriteLine(
            $"{tierResult.Tier} entries: {tierResult.EntryCount}");
        continue;
    }

    Console.WriteLine(
        $"{tierResult.Tier} collection failed: {GetStatusMessage(tierResult.Status)}");

    if (tierResult.Status == RiotApiRequestStatus.RateLimited
        && tierResult.RetryAfter is { } retryAfter)
    {
        Console.WriteLine(
            $"Retry-After: {Math.Ceiling(retryAfter.TotalSeconds)} seconds");
    }
}

Console.WriteLine($"Unique seed PUUIDs: {seedResult.Puuids.Count}");
Console.WriteLine($"PUUID conversions: {seedResult.ConversionCount}");

if (seedResult.ConversionFailureCount > 0)
{
    Console.WriteLine(
        $"PUUID conversion failures: {seedResult.ConversionFailureCount}");
}

if (seedResult.TierResults.Any(result =>
        result.Status is RiotApiRequestStatus.AuthenticationFailed
            or RiotApiRequestStatus.RateLimited
            or RiotApiRequestStatus.Cancelled))
{
    Console.WriteLine("Seed collection stopped before further API requests.");
    PrintRetrySummary(riotApiClient.RetryDiagnostics);
    return;
}

var matchIdProvider = new MatchIdProvider(riotApiClient, options);
var matchIdResult = await matchIdProvider.CollectMatchIdsAsync(
    seedResult.Puuids,
    shutdown.Token);

Console.WriteLine("Match-V5 test collection");
Console.WriteLine($"Seeds used: {matchIdResult.SeedsUsed}");
Console.WriteLine($"Pages requested: {matchIdResult.PagesRequested}");
Console.WriteLine($"Match IDs returned: {matchIdResult.ReturnedCount}");
Console.WriteLine($"Duplicate Match IDs: {matchIdResult.DuplicateCount}");
Console.WriteLine($"Unique Match IDs: {matchIdResult.MatchIds.Count}");

if (matchIdResult.InvalidCount > 0)
{
    Console.WriteLine($"Invalid Match IDs: {matchIdResult.InvalidCount}");
}

foreach (var failure in matchIdResult.Failures)
{
    Console.WriteLine(
        $"Seed request {failure.SeedNumber}, page {failure.PageNumber} failed: "
        + GetStatusMessage(failure.Status));

    if (failure.Status == RiotApiRequestStatus.RateLimited
        && failure.RetryAfter is { } retryAfter)
    {
        Console.WriteLine(
            $"Retry-After: {Math.Ceiling(retryAfter.TotalSeconds)} seconds");
    }
}

if (matchIdResult.Failures.Any(failure =>
        failure.Status is RiotApiRequestStatus.AuthenticationFailed
            or RiotApiRequestStatus.RateLimited
            or RiotApiRequestStatus.Cancelled))
{
    Console.WriteLine("Match ID collection stopped before further API requests.");
    PrintRetrySummary(riotApiClient.RetryDiagnostics);
    return;
}

var newlyDiscoveredCount = stateStore.AppendDiscovered(matchIdResult.MatchIds);
var pendingMatchIds = stateStore.DiscoveredMatchIds
    .Where(matchId => !stateStore.TerminalMatchIds.Contains(matchId))
    .ToList();

Console.WriteLine($"Newly discovered match IDs: {newlyDiscoveredCount}");
Console.WriteLine(
    $"Total discovered match IDs: {stateStore.DiscoveredMatchIds.Count}");
Console.WriteLine($"Pending match IDs: {pendingMatchIds.Count}");

var matchProcessor = new MatchProcessor(riotApiClient, options);
var matchIdsToProcess = pendingMatchIds
    .Take(options.ProcessingMatchLimit)
    .ToList();
var matchesAttempted = 0;
var matchesAccepted = 0;
var matchesRejected = 0;
var apiFailures = 0;
var observationsAdded = 0;
var assistFreeCandidateCount = 0;
var assistedCandidateCount = 0;
var killEventsWithAssists = 0;
var killEventsWithNullAssistList = 0;
var killEventsWithEmptyAssistList = 0;
var acceptedGameDurations = new List<long>();
var level2UnknownReasons = new MilestoneUnknownReasonCounts();
var level3UnknownReasons = new MilestoneUnknownReasonCounts();
var level6UnknownReasons = new MilestoneUnknownReasonCounts();
var timelineRequestsAvoidedByValidation = 0;

Console.WriteLine();
Console.WriteLine("Multi-match processing test");

foreach (var matchId in matchIdsToProcess)
{
    if (stateStore.AcceptedMatchCount >= options.TargetAcceptedMatchCount)
    {
        Console.WriteLine("Persistent accepted Match target reached.");
        break;
    }

    matchesAttempted++;
    var result = await matchProcessor.ProcessAsync(matchId, shutdown.Token);

    if (result.Status is MatchProcessingStatus.AuthenticationFailed
        or MatchProcessingStatus.RateLimitExhausted
        or MatchProcessingStatus.Cancelled)
    {
        apiFailures++;
        Console.WriteLine(
            $"Match {matchesAttempted}/{matchIdsToProcess.Count}: "
            + $"stopped - {result.Message}");
        break;
    }

    if (result.Status == MatchProcessingStatus.ApiFailure)
    {
        apiFailures++;

        Console.WriteLine(
            $"Match {matchesAttempted}/{matchIdsToProcess.Count}: "
            + $"skipped - {result.Message}");

        if (result.ApiStatus == RiotApiRequestStatus.RateLimited
            && result.RetryAfter is { } retryAfter)
        {
            Console.WriteLine(
                $"Retry-After: {Math.Ceiling(retryAfter.TotalSeconds)} seconds");
        }

        continue;
    }

    if (result.Status == MatchProcessingStatus.Rejected
        && result.ValidationOutcome is { } validationOutcome)
    {
        stateStore.AppendRejected(matchId, validationOutcome);
        matchesRejected++;

        if (validationOutcome != MatchValidationOutcome.ObservationFailure)
        {
            timelineRequestsAvoidedByValidation++;
        }
        Console.WriteLine(
            $"Match {matchesAttempted}/{matchIdsToProcess.Count}: "
            + $"skipped - {validationOutcome}");
        continue;
    }

    // Persist the complete terminal record before changing in-memory totals.
    stateStore.AppendAccepted(matchId, options.TargetPatch, result.Observations);
    accumulator.AddRange(result.Observations);
    matchesAccepted++;
    observationsAdded += result.Observations.Count;
    assistFreeCandidateCount += result.AssistFreeCandidateCount;
    assistedCandidateCount += result.AssistedCandidateCount;
    killEventsWithAssists += result.KillEventsWithAssists;
    killEventsWithNullAssistList += result.KillEventsWithNullAssistList;
    killEventsWithEmptyAssistList += result.KillEventsWithEmptyAssistList;
    acceptedGameDurations.Add(result.GameDurationSeconds);
    level2UnknownReasons += result.Level2UnknownReasons ?? new();
    level3UnknownReasons += result.Level3UnknownReasons ?? new();
    level6UnknownReasons += result.Level6UnknownReasons ?? new();
    Console.WriteLine(
        $"Match {matchesAttempted}/{matchIdsToProcess.Count}: accepted");
}

await stateStore.WriteCheckpointAsync(options, accumulator);

Console.WriteLine();
Console.WriteLine("Session summary");
Console.WriteLine($"Matches attempted this run: {matchesAttempted}");
Console.WriteLine($"Matches accepted this run: {matchesAccepted}");
Console.WriteLine($"Matches rejected this run: {matchesRejected}");
Console.WriteLine($"API failures this run: {apiFailures}");
Console.WriteLine($"Observations added this run: {observationsAdded}");
Console.WriteLine(
    $"Observations count valid: {observationsAdded == matchesAccepted * 5}");

var persistentPendingCount = stateStore.DiscoveredMatchIds.Count
    - stateStore.TerminalMatchIds.Count;
var storageSizeMegabytes = stateStore.GetStorageSizeBytes()
    / (1024d * 1024d);

Console.WriteLine();
Console.WriteLine("Persistent totals");
Console.WriteLine($"Discovered matches: {stateStore.DiscoveredMatchIds.Count}");
Console.WriteLine($"Terminal matches: {stateStore.TerminalMatchIds.Count}");
Console.WriteLine($"Accepted matches: {stateStore.AcceptedMatchCount}");
Console.WriteLine($"Rejected matches: {stateStore.RejectedMatchCount}");
Console.WriteLine($"Observations: {stateStore.ObservationCount}");
Console.WriteLine($"Aggregated matchup keys: {accumulator.Aggregates.Count}");
Console.WriteLine($"Pending matches: {persistentPendingCount}");
Console.WriteLine(
    "Observations count valid: "
    + (stateStore.ObservationCount == stateStore.AcceptedMatchCount * 5));
Console.WriteLine($"Invariants valid: {accumulator.InvariantsValid}");
Console.WriteLine($"State storage size: {storageSizeMegabytes:F2} MB");

var aggregates = accumulator.Aggregates.Values.ToList();
var observationCount = stateStore.ObservationCount;
var level2Unknown = aggregates.Sum(value => value.FirstLevel2Unknown);
var level3Unknown = aggregates.Sum(value => value.FirstLevel3Unknown);
var level6Unknown = aggregates.Sum(value => value.FirstLevel6Unknown);

Console.WriteLine();
Console.WriteLine("Metric quality");
PrintMetricQuality("First level 2 unknown", level2Unknown, observationCount);
PrintMetricQuality("First level 3 unknown", level3Unknown, observationCount);
PrintMetricQuality("First level 6 unknown", level6Unknown, observationCount);

Console.WriteLine();
Console.WriteLine("Level metric diagnostics (this run)");
PrintUnknownReasons("Level 2 unknown", level2UnknownReasons);
PrintUnknownReasons("Level 3 unknown", level3UnknownReasons);
PrintUnknownReasons("Level 6 unknown", level6UnknownReasons);

Console.WriteLine();
Console.WriteLine("Role unknown counts (level 2 / 3 / 6)");

foreach (var role in RoleNormalizer.StandardRoles)
{
    var roleAggregates = aggregates.Where(value => value.Key.Role == role).ToList();
    Console.WriteLine(
        $"{RoleNormalizer.ToDisplayName(role)}: "
        + $"{roleAggregates.Sum(value => value.FirstLevel2Unknown)} / "
        + $"{roleAggregates.Sum(value => value.FirstLevel3Unknown)} / "
        + roleAggregates.Sum(value => value.FirstLevel6Unknown));
}

Console.WriteLine();
Console.WriteLine("Persistent validation quality");

foreach (var outcome in Enum.GetValues<MatchValidationOutcome>())
{
    var count = stateStore.OutcomeCounts[outcome];
    Console.WriteLine(
        $"{outcome}: {count} ({GetPercentage(count, stateStore.TerminalMatchIds.Count):F2}%)");
}

Console.WriteLine();
Console.WriteLine("Role observation counts");
var roleObservationCounts = RoleNormalizer.StandardRoles.ToDictionary(
    role => role,
    role => aggregates
        .Where(value => value.Key.Role == role)
        .Sum(value => value.SampleSize));

foreach (var role in RoleNormalizer.StandardRoles)
{
    Console.WriteLine(
        $"{RoleNormalizer.ToDisplayName(role)}: {roleObservationCounts[role]}");
}

Console.WriteLine(
    "Role observation counts valid: "
    + roleObservationCounts.Values.All(count =>
        count == stateStore.AcceptedMatchCount));

Console.WriteLine();
Console.WriteLine("Matchup sample distribution");
Console.WriteLine($"Total keys: {aggregates.Count}");
Console.WriteLine(
    $"Sample >= 2: {aggregates.Count(value => value.SampleSize >= 2)}");
Console.WriteLine(
    $"Sample >= 3: {aggregates.Count(value => value.SampleSize >= 3)}");
Console.WriteLine(
    $"Sample >= 5: {aggregates.Count(value => value.SampleSize >= 5)}");
Console.WriteLine(
    $"Sample >= 10: {aggregates.Count(value => value.SampleSize >= 10)}");
Console.WriteLine(
    $"Sample >= 20: {aggregates.Count(value => value.SampleSize >= 20)}");
Console.WriteLine(
    $"Maximum sample size: {aggregates.Select(value => value.SampleSize).DefaultIfEmpty().Max()}");

var championRoleAppearances = new Dictionary<(StandardRole Role, string Champion), int>();

foreach (var aggregate in aggregates)
{
    AddChampionRoleAppearance(
        championRoleAppearances,
        aggregate.Key.Role,
        aggregate.Key.ChampionA,
        aggregate.SampleSize);
    AddChampionRoleAppearance(
        championRoleAppearances,
        aggregate.Key.Role,
        aggregate.Key.ChampionB,
        aggregate.SampleSize);
}

Console.WriteLine();
Console.WriteLine("Champion-role appearances");
Console.WriteLine($"Distinct champion-role keys: {championRoleAppearances.Count}");
Console.WriteLine("Most frequent (diagnostic only):");

foreach (var appearance in championRoleAppearances
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

Console.WriteLine();
Console.WriteLine("Assist semantics diagnostic (this run)");
Console.WriteLine(
    "Solo-kill candidate events with null/empty assists: "
    + assistFreeCandidateCount);
Console.WriteLine(
    "Solo-kill candidate events rejected because assists exist: "
    + assistedCandidateCount);
Console.WriteLine(
    $"Kill events with 1+ assists: {killEventsWithAssists}");
Console.WriteLine(
    $"Kill events with null/missing assist list: {killEventsWithNullAssistList}");
Console.WriteLine(
    $"Kill events with empty assist list: {killEventsWithEmptyAssistList}");

Console.WriteLine();
Console.WriteLine("Session game duration");

if (acceptedGameDurations.Count == 0)
{
    Console.WriteLine("No accepted matches this run.");
}
else
{
    Console.WriteLine($"Min: {FormatDuration(acceptedGameDurations.Min())}");
    Console.WriteLine(
        $"Average: {FormatDuration((long)acceptedGameDurations.Average())}");
    Console.WriteLine($"Max: {FormatDuration(acceptedGameDurations.Max())}");
}

PrintRetrySummary(riotApiClient.RetryDiagnostics);
Console.WriteLine(
    $"Timeline requests avoided by validation: {timelineRequestsAvoidedByValidation}");

await WriteDatasetAndPrintAsync(datasetWriter, accumulator, stateStore);

Console.WriteLine();
Console.WriteLine("Top aggregated samples");

foreach (var aggregate in accumulator.Aggregates.Values
             .OrderByDescending(aggregate => aggregate.SampleSize)
             .ThenBy(aggregate => aggregate.Key.Role)
             .ThenBy(aggregate => aggregate.Key.ChampionA, StringComparer.Ordinal)
             .ThenBy(aggregate => aggregate.Key.ChampionB, StringComparer.Ordinal)
             .Take(5))
{
    Console.WriteLine();
    Console.WriteLine(
        $"{RoleNormalizer.ToDisplayName(aggregate.Key.Role)} "
        + $"{aggregate.Key.ChampionA} vs {aggregate.Key.ChampionB}");
    Console.WriteLine($"Sample size: {aggregate.SampleSize}");
    Console.WriteLine($"Wins for A: {aggregate.WinsForA}");
    Console.WriteLine(
        "First level 2: "
        + $"A={aggregate.FirstLevel2ForA}, "
        + $"B={aggregate.FirstLevel2ForB}, "
        + $"Unknown={aggregate.FirstLevel2Unknown}");
    Console.WriteLine(
        "First level 3: "
        + $"A={aggregate.FirstLevel3ForA}, "
        + $"B={aggregate.FirstLevel3ForB}, "
        + $"Unknown={aggregate.FirstLevel3Unknown}");
    Console.WriteLine(
        "First level 6: "
        + $"A={aggregate.FirstLevel6ForA}, "
        + $"B={aggregate.FirstLevel6ForB}, "
        + $"Unknown={aggregate.FirstLevel6Unknown}");
    Console.WriteLine(
        "Solo kills before 15: "
        + $"A={aggregate.SoloKillsBefore15ForA}, "
        + $"B={aggregate.SoloKillsBefore15ForB}");
}

static string GetStatusMessage(RiotApiRequestStatus status)
{
    return status switch
    {
        RiotApiRequestStatus.AuthenticationFailed =>
            "API Key is invalid, expired, or unauthorized.",
        RiotApiRequestStatus.RateLimited => "Riot API rate limit exceeded.",
        RiotApiRequestStatus.ServerError => "Riot API server error.",
        RiotApiRequestStatus.NetworkError => "Network connection failed.",
        RiotApiRequestStatus.InvalidResponse => "Invalid Riot API response.",
        RiotApiRequestStatus.Cancelled => "Request cancelled.",
        _ => "Unexpected Riot API response."
    };
}

static void PrintRetrySummary(RiotApiRetryDiagnostics diagnostics)
{
    Console.WriteLine();
    Console.WriteLine("API retry summary");
    Console.WriteLine($"Logical API requests: {diagnostics.LogicalApiRequests}");
    Console.WriteLine($"HTTP attempts: {diagnostics.HttpAttempts}");
    Console.WriteLine($"429 retries: {diagnostics.RetriesDueToRateLimit}");
    Console.WriteLine($"5xx retries: {diagnostics.RetriesDueToServerError}");
    Console.WriteLine(
        "Network/timeout retries: "
        + diagnostics.RetriesDueToNetworkOrTimeout);
    Console.WriteLine(
        $"Retry wait: {diagnostics.TotalRetryWait.TotalSeconds:F1} seconds");
    Console.WriteLine(
        $"League requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.League)}");
    Console.WriteLine(
        $"Summoner requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.Summoner)}");
    Console.WriteLine(
        $"Match ID requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.MatchIds)}");
    Console.WriteLine(
        $"Match Detail requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.MatchDetail)}");
    Console.WriteLine(
        $"Timeline requests: {diagnostics.GetCategoryRequestCount(RiotApiRequestCategory.Timeline)}");

    if (diagnostics.ApplicationRateLimit is not null)
    {
        Console.WriteLine(
            $"Observed app rate limit: {diagnostics.ApplicationRateLimit}");
        Console.WriteLine(
            $"Observed app rate count: {diagnostics.ApplicationRateLimitCount ?? "not provided"}");
    }

    if (diagnostics.MethodRateLimit is not null)
    {
        Console.WriteLine(
            $"Observed method rate limit: {diagnostics.MethodRateLimit}");
        Console.WriteLine(
            $"Observed method rate count: {diagnostics.MethodRateLimitCount ?? "not provided"}");
    }
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

static void PrintUnknownReasons(
    string label,
    MilestoneUnknownReasonCounts counts)
{
    Console.WriteLine(label + ":");
    Console.WriteLine($"  Tie: {counts.Tie}");
    Console.WriteLine($"  Missing A: {counts.MissingA}");
    Console.WriteLine($"  Missing B: {counts.MissingB}");
    Console.WriteLine($"  Missing both: {counts.MissingBoth}");
}

static void AddChampionRoleAppearance(
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
        Console.WriteLine(
            $"File size: {result.FileSizeBytes / (1024d * 1024d):F2} MB");
    }
    catch (Exception exception) when (exception is IOException
        or UnauthorizedAccessException
        or InvalidOperationException)
    {
        Console.WriteLine($"Dataset output failed: {exception.Message}");
    }
}
