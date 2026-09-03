using StatsCollector.Analysis;
using StatsCollector.Aggregation;
using StatsCollector.Api;
using StatsCollector.Configuration;
using StatsCollector.Storage;

namespace StatsCollector.Collection;

public sealed class CollectionRunner
{
    private readonly RiotApiClient _riotApiClient;
    private readonly CollectorOptions _options;
    private readonly CollectorStateStore _stateStore;
    private readonly MatchupAccumulator _accumulator;
    private readonly MatchProcessor _matchProcessor;
    private readonly SeedRotationStateStore _seedRotationStateStore;
    private readonly SeedRotationStateLoadResult _seedRotationLoad;
    private IReadOnlyList<string>? _orderedSeedPool;
    private int _nextSeedIndex;

    public CollectionRunner(
        RiotApiClient riotApiClient,
        CollectorOptions options,
        CollectorStateStore stateStore,
        MatchupAccumulator accumulator)
    {
        _riotApiClient = riotApiClient;
        _options = options;
        _stateStore = stateStore;
        _accumulator = accumulator;
        _matchProcessor = new MatchProcessor(riotApiClient, options);
        _seedRotationStateStore = new SeedRotationStateStore(
            stateStore.StateDirectory);
        _seedRotationLoad = _seedRotationStateStore.Load();
        _nextSeedIndex = _seedRotationLoad.State.NextSeedIndex;
    }

    public async Task<CollectionRunResult> RunAsync(
        CancellationToken cancellationToken)
    {
        var result = new MutableCollectionRunResult(_stateStore.AcceptedMatchCount);
        var batchNumber = 0;
        var discoveryWindowNumber = 0;
        var seedsScannedSinceNewMatch = 0;

        if (_seedRotationLoad.WasReset)
        {
            Console.WriteLine(
                "Discovery cursor warning: state was invalid; cursor reset to 0.");
        }

        while (_stateStore.AcceptedMatchCount < _options.TargetAcceptedMatchCount)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result.StopReason = CollectionStopReason.Cancelled;
                break;
            }

            var pending = GetPendingMatchIds();

            if (pending.Count == 0)
            {
                discoveryWindowNumber++;
                var discovery = await DiscoverAsync(
                    discoveryWindowNumber,
                    cancellationToken);

                if (discovery.FatalStopReason is { } fatalStopReason)
                {
                    result.StopReason = fatalStopReason;
                    break;
                }

                pending = GetPendingMatchIds();

                if (discovery.NewlyDiscoveredCount > 0)
                {
                    seedsScannedSinceNewMatch = 0;
                }
                else
                {
                    seedsScannedSinceNewMatch += discovery.SeedsSuccessfullyScanned;
                }

                if (pending.Count == 0
                    && seedsScannedSinceNewMatch >= discovery.SeedPoolSize)
                {
                    result.StopReason = CollectionStopReason.NoNewMatches;
                    break;
                }

                if (pending.Count == 0)
                {
                    if (!_options.AutoContinueCollection)
                    {
                        result.StopReason = CollectionStopReason.NoNewMatches;
                        break;
                    }

                    continue;
                }
            }

            batchNumber++;
            var batch = await ProcessBatchAsync(
                batchNumber,
                pending,
                result,
                cancellationToken);
            result.BatchesCompleted++;
            await _stateStore.WriteCheckpointAsync(_options, _accumulator);
            PrintBatchSummary(batch);

            if (batch.FatalStopReason is { } batchStopReason)
            {
                result.StopReason = batchStopReason;
                break;
            }

            if (batch.Accepted + batch.Rejected == 0)
            {
                result.StopReason = CollectionStopReason.NoProgress;
                break;
            }

            if (_stateStore.AcceptedMatchCount >= _options.TargetAcceptedMatchCount)
            {
                result.StopReason = CollectionStopReason.TargetReached;
                break;
            }

            if (!_options.AutoContinueCollection)
            {
                result.StopReason = CollectionStopReason.SingleBatchComplete;
                break;
            }

            await Task.Yield();
        }

        if (_stateStore.AcceptedMatchCount >= _options.TargetAcceptedMatchCount)
        {
            result.StopReason = CollectionStopReason.TargetReached;
        }

        return result.ToImmutable(_stateStore.AcceptedMatchCount);
    }

    private async Task<DiscoveryRefreshResult> DiscoverAsync(
        int windowNumber,
        CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.WriteLine($"Discovery window {windowNumber}");

        if (_orderedSeedPool is null)
        {
            var seedProvider = new SeedProvider(_riotApiClient);
            var seedResult = await seedProvider.CollectMasterPlusAsync(
                cancellationToken);

            foreach (var tierResult in seedResult.TierResults)
            {
                Console.WriteLine(
                    tierResult.Status == RiotApiRequestStatus.Success
                        ? $"{tierResult.Tier} entries: {tierResult.EntryCount}"
                        : $"{tierResult.Tier} collection failed: {tierResult.Status}");
            }

            var fatalStatus = seedResult.TierResults
                .Select(item => item.Status)
                .FirstOrDefault(IsFatalStatus);

            if (IsFatalStatus(fatalStatus))
            {
                return new DiscoveryRefreshResult(
                    0,
                    0,
                    0,
                    ToStopReason(fatalStatus));
            }

            _orderedSeedPool = seedResult.Puuids
                .OrderBy(puuid => puuid, StringComparer.Ordinal)
                .ToList();
        }

        if (_orderedSeedPool.Count == 0)
        {
            return new DiscoveryRefreshResult(0, 0, 0);
        }

        var seedPoolSize = _orderedSeedPool.Count;
        var startingCursor = _nextSeedIndex % seedPoolSize;
        var seedsToSelect = Math.Min(
            _options.DiscoverySeedBatchSize,
            seedPoolSize);
        var selectedSeeds = Enumerable.Range(0, seedsToSelect)
            .Select(offset =>
                _orderedSeedPool[(startingCursor + offset) % seedPoolSize])
            .ToList();

        var matchIdProvider = new MatchIdProvider(_riotApiClient, _options);
        var matchIdResult = await matchIdProvider.CollectMatchIdsAsync(
            selectedSeeds,
            cancellationToken);
        var nextCursor = (startingCursor
            + matchIdResult.SuccessfullyScannedSeeds) % seedPoolSize;
        await _seedRotationStateStore.SaveAsync(
            nextCursor,
            seedPoolSize,
            CancellationToken.None);
        _nextSeedIndex = nextCursor;
        var fatalFailure = matchIdResult.Failures
            .FirstOrDefault(item => IsFatalStatus(item.Status));

        if (fatalFailure is not null)
        {
            return new DiscoveryRefreshResult(
                0,
                seedPoolSize,
                matchIdResult.SuccessfullyScannedSeeds,
                ToStopReason(fatalFailure.Status));
        }

        if (matchIdResult.Failures.Count > 0
            && matchIdResult.SuccessfullyScannedSeeds == 0)
        {
            return new DiscoveryRefreshResult(
                0,
                seedPoolSize,
                0,
                CollectionStopReason.NoProgress);
        }

        var alreadyKnown = matchIdResult.MatchIds.Count(
            _stateStore.DiscoveredMatchIds.Contains);
        var newlyDiscovered = _stateStore.AppendDiscovered(matchIdResult.MatchIds);
        Console.WriteLine($"Seed pool size: {seedPoolSize}");
        Console.WriteLine($"Starting seed cursor: {startingCursor}");
        Console.WriteLine($"Seeds selected: {selectedSeeds.Count}");
        Console.WriteLine(
            $"Seeds successfully scanned: {matchIdResult.SuccessfullyScannedSeeds}");
        Console.WriteLine($"Pages requested: {matchIdResult.PagesRequested}");
        Console.WriteLine($"Raw Match IDs returned: {matchIdResult.ReturnedCount}");
        Console.WriteLine(
            $"Duplicate IDs in this discovery: {matchIdResult.DuplicateCount}");
        Console.WriteLine($"Already known Match IDs: {alreadyKnown}");
        Console.WriteLine($"Newly discovered Match IDs: {newlyDiscovered}");
        Console.WriteLine($"Seed cursor: {startingCursor} -> {nextCursor}");

        return new DiscoveryRefreshResult(
            newlyDiscovered,
            seedPoolSize,
            matchIdResult.SuccessfullyScannedSeeds);
    }

    private async Task<BatchResult> ProcessBatchAsync(
        int batchNumber,
        IReadOnlyList<string> pending,
        MutableCollectionRunResult total,
        CancellationToken cancellationToken)
    {
        var batch = new BatchResult(batchNumber);
        var selected = pending.Take(_options.ProcessingMatchLimit).ToList();

        foreach (var matchId in selected)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                batch.FatalStopReason = CollectionStopReason.Cancelled;
                break;
            }

            if (_stateStore.AcceptedMatchCount >= _options.TargetAcceptedMatchCount)
            {
                break;
            }

            batch.Attempted++;
            total.MatchesAttempted++;
            var processing = await _matchProcessor.ProcessAsync(
                matchId,
                cancellationToken);

            if (processing.Status is MatchProcessingStatus.AuthenticationFailed
                or MatchProcessingStatus.RateLimitExhausted
                or MatchProcessingStatus.Cancelled)
            {
                batch.ApiFailures++;
                total.ApiFailures++;
                batch.FatalStopReason = processing.Status switch
                {
                    MatchProcessingStatus.AuthenticationFailed =>
                        CollectionStopReason.AuthenticationFailure,
                    MatchProcessingStatus.RateLimitExhausted =>
                        CollectionStopReason.RateLimitExhausted,
                    _ => CollectionStopReason.Cancelled
                };
                break;
            }

            if (processing.Status == MatchProcessingStatus.ApiFailure)
            {
                batch.ApiFailures++;
                total.ApiFailures++;
                continue;
            }

            if (processing.Status == MatchProcessingStatus.Rejected
                && processing.ValidationOutcome is { } validationOutcome)
            {
                _stateStore.AppendRejected(matchId, validationOutcome);
                batch.Rejected++;
                total.MatchesRejected++;
                total.SessionValidationCounts[validationOutcome]++;

                if (validationOutcome != MatchValidationOutcome.ObservationFailure)
                {
                    total.TimelineRequestsAvoidedByValidation++;
                }

                continue;
            }

            _stateStore.AppendAccepted(
                matchId,
                _options.TargetPatch,
                processing.Observations);
            _accumulator.AddRange(processing.Observations);
            batch.Accepted++;
            total.MatchesAccepted++;
            total.SessionValidationCounts[MatchValidationOutcome.Accepted]++;
            total.ObservationsAdded += processing.Observations.Count;
            total.AssistFreeCandidateCount += processing.AssistFreeCandidateCount;
            total.AssistedCandidateCount += processing.AssistedCandidateCount;
            total.KillEventsWithAssists += processing.KillEventsWithAssists;
            total.KillEventsWithNullAssistList +=
                processing.KillEventsWithNullAssistList;
            total.KillEventsWithEmptyAssistList +=
                processing.KillEventsWithEmptyAssistList;
            total.AcceptedGameDurations.Add(processing.GameDurationSeconds);
            total.Level2UnknownReasons += processing.Level2UnknownReasons ?? new();
            total.Level3UnknownReasons += processing.Level3UnknownReasons ?? new();
            total.Level6UnknownReasons += processing.Level6UnknownReasons ?? new();
        }

        batch.PendingAfter = GetPendingMatchIds().Count;
        return batch;
    }

    private List<string> GetPendingMatchIds() => _stateStore.DiscoveredMatchIds
        .Where(matchId => !_stateStore.TerminalMatchIds.Contains(matchId))
        .ToList();

    private void PrintBatchSummary(BatchResult batch)
    {
        Console.WriteLine();
        Console.WriteLine($"Batch {batch.BatchNumber} complete");
        Console.WriteLine($"Attempted: {batch.Attempted}");
        Console.WriteLine($"Accepted: {batch.Accepted}");
        Console.WriteLine($"Rejected: {batch.Rejected}");
        Console.WriteLine($"API failures: {batch.ApiFailures}");
        Console.WriteLine(
            $"Persistent accepted: {_stateStore.AcceptedMatchCount} / "
            + _options.TargetAcceptedMatchCount);
        Console.WriteLine($"Pending: {batch.PendingAfter}");
    }

    private static bool IsFatalStatus(RiotApiRequestStatus status) =>
        status is RiotApiRequestStatus.AuthenticationFailed
            or RiotApiRequestStatus.RateLimited
            or RiotApiRequestStatus.Cancelled;

    private static CollectionStopReason ToStopReason(RiotApiRequestStatus status) =>
        status switch
        {
            RiotApiRequestStatus.AuthenticationFailed =>
                CollectionStopReason.AuthenticationFailure,
            RiotApiRequestStatus.RateLimited =>
                CollectionStopReason.RateLimitExhausted,
            RiotApiRequestStatus.Cancelled => CollectionStopReason.Cancelled,
            _ => CollectionStopReason.FatalError
        };
}

public sealed record CollectionRunResult(
    CollectionStopReason StopReason,
    int BatchesCompleted,
    int AcceptedAtStart,
    int AcceptedAtEnd,
    int MatchesAttempted,
    int MatchesAccepted,
    int MatchesRejected,
    int ApiFailures,
    int ObservationsAdded,
    int AssistFreeCandidateCount,
    int AssistedCandidateCount,
    int KillEventsWithAssists,
    int KillEventsWithNullAssistList,
    int KillEventsWithEmptyAssistList,
    IReadOnlyList<long> AcceptedGameDurations,
    MilestoneUnknownReasonCounts Level2UnknownReasons,
    MilestoneUnknownReasonCounts Level3UnknownReasons,
    MilestoneUnknownReasonCounts Level6UnknownReasons,
    int TimelineRequestsAvoidedByValidation,
    IReadOnlyDictionary<MatchValidationOutcome, int> SessionValidationCounts);

public enum CollectionStopReason
{
    TargetReached,
    SingleBatchComplete,
    NoNewMatches,
    NoProgress,
    AuthenticationFailure,
    RateLimitExhausted,
    Cancelled,
    FatalError
}

internal sealed class MutableCollectionRunResult
{
    public MutableCollectionRunResult(int acceptedAtStart)
    {
        AcceptedAtStart = acceptedAtStart;
    }

    public CollectionStopReason StopReason { get; set; } =
        CollectionStopReason.FatalError;
    public int BatchesCompleted { get; set; }
    public int AcceptedAtStart { get; }
    public int MatchesAttempted { get; set; }
    public int MatchesAccepted { get; set; }
    public int MatchesRejected { get; set; }
    public int ApiFailures { get; set; }
    public int ObservationsAdded { get; set; }
    public int AssistFreeCandidateCount { get; set; }
    public int AssistedCandidateCount { get; set; }
    public int KillEventsWithAssists { get; set; }
    public int KillEventsWithNullAssistList { get; set; }
    public int KillEventsWithEmptyAssistList { get; set; }
    public List<long> AcceptedGameDurations { get; } = [];
    public MilestoneUnknownReasonCounts Level2UnknownReasons { get; set; } = new();
    public MilestoneUnknownReasonCounts Level3UnknownReasons { get; set; } = new();
    public MilestoneUnknownReasonCounts Level6UnknownReasons { get; set; } = new();
    public int TimelineRequestsAvoidedByValidation { get; set; }
    public Dictionary<MatchValidationOutcome, int> SessionValidationCounts { get; } =
        Enum.GetValues<MatchValidationOutcome>()
            .ToDictionary(outcome => outcome, _ => 0);

    public CollectionRunResult ToImmutable(int acceptedAtEnd) => new(
        StopReason,
        BatchesCompleted,
        AcceptedAtStart,
        acceptedAtEnd,
        MatchesAttempted,
        MatchesAccepted,
        MatchesRejected,
        ApiFailures,
        ObservationsAdded,
        AssistFreeCandidateCount,
        AssistedCandidateCount,
        KillEventsWithAssists,
        KillEventsWithNullAssistList,
        KillEventsWithEmptyAssistList,
        AcceptedGameDurations,
        Level2UnknownReasons,
        Level3UnknownReasons,
        Level6UnknownReasons,
        TimelineRequestsAvoidedByValidation,
        SessionValidationCounts);
}

internal sealed record DiscoveryRefreshResult(
    int NewlyDiscoveredCount,
    int SeedPoolSize,
    int SeedsSuccessfullyScanned,
    CollectionStopReason? FatalStopReason = null);

internal sealed class BatchResult
{
    public BatchResult(int batchNumber)
    {
        BatchNumber = batchNumber;
    }

    public int BatchNumber { get; }
    public int Attempted { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public int ApiFailures { get; set; }
    public int PendingAfter { get; set; }
    public CollectionStopReason? FatalStopReason { get; set; }
}
