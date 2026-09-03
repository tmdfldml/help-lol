using StatsCollector.Analysis;
using StatsCollector.Api;
using StatsCollector.Configuration;

namespace StatsCollector.Collection;

public sealed class MatchProcessor
{
    private readonly RiotApiClient _riotApiClient;
    private readonly CollectorOptions _options;
    private readonly PatchVersion _targetPatch;
    private readonly RoleMatcher _roleMatcher = new();
    private readonly TimelineMetricExtractor _metricExtractor = new();

    public MatchProcessor(RiotApiClient riotApiClient, CollectorOptions options)
    {
        _riotApiClient = riotApiClient;
        _options = options;

        if (!PatchVersionParser.TryParseTargetPatch(
                options.TargetPatch,
                out _targetPatch))
        {
            throw new ArgumentException(
                "TargetPatch must use the major.minor format.",
                nameof(options));
        }
    }

    public async Task<MatchProcessingResult> ProcessAsync(
        string matchId,
        CancellationToken cancellationToken = default)
    {
        var matchResponse = await _riotApiClient.GetMatchAsync(
            matchId,
            cancellationToken);

        if (matchResponse.Status != RiotApiRequestStatus.Success
            || matchResponse.Data is null)
        {
            return MatchProcessingResult.ApiFailure(
                "match detail request failed",
                matchResponse.Status,
                matchResponse.RetryAfter);
        }

        var match = matchResponse.Data;

        if (match.Info.QueueId != _options.QueueId)
        {
            return MatchProcessingResult.Rejected(MatchValidationOutcome.WrongQueue);
        }

        if (!PatchVersionParser.TryParseGameVersion(
                match.Info.GameVersion,
                out var matchPatch))
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.InvalidGameVersion);
        }

        if (matchPatch != _targetPatch)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.PatchMismatch);
        }

        if (match.Info.GameDuration < _options.MinimumGameDurationSeconds)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.InsufficientGameDuration);
        }

        if (match.Info.Participants.Count != 10)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.InvalidParticipantCount);
        }

        var roleResult = _roleMatcher.CreateMatchups(match.Info.Participants);

        if (!roleResult.IsValid)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.InvalidRoleLayout);
        }

        if (roleResult.Matchups.Count != RoleNormalizer.StandardRoles.Count)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.InvalidMatchupCount);
        }

        var timelineResponse = await _riotApiClient.GetTimelineAsync(
            matchId,
            cancellationToken);

        if (timelineResponse.Status != RiotApiRequestStatus.Success
            || timelineResponse.Data is null)
        {
            return MatchProcessingResult.ApiFailure(
                "timeline request failed",
                timelineResponse.Status,
                timelineResponse.RetryAfter);
        }

        var extraction = _metricExtractor.Extract(
            roleResult.Matchups,
            timelineResponse.Data);

        // A match is accepted only after all five observations are available.
        if (!extraction.IsSuccess
            || extraction.Observations.Count != RoleNormalizer.StandardRoles.Count)
        {
            return MatchProcessingResult.Rejected(
                MatchValidationOutcome.ObservationFailure);
        }

        return new MatchProcessingResult(
            MatchProcessingStatus.Accepted,
            "accepted",
            MatchValidationOutcome.Accepted,
            extraction.Observations,
            extraction.AssistFreeCandidateCount,
            extraction.AssistedCandidateCount,
            match.Info.GameDuration,
            extraction.KillEventsWithAssists,
            extraction.KillEventsWithNullAssistList,
            extraction.KillEventsWithEmptyAssistList,
            extraction.Level2UnknownReasons,
            extraction.Level3UnknownReasons,
            extraction.Level6UnknownReasons);
    }
}

public sealed record MatchProcessingResult(
    MatchProcessingStatus Status,
    string Message,
    MatchValidationOutcome? ValidationOutcome,
    IReadOnlyList<MatchupObservation> Observations,
    int AssistFreeCandidateCount,
    int AssistedCandidateCount,
    long GameDurationSeconds = 0,
    int KillEventsWithAssists = 0,
    int KillEventsWithNullAssistList = 0,
    int KillEventsWithEmptyAssistList = 0,
    MilestoneUnknownReasonCounts? Level2UnknownReasons = null,
    MilestoneUnknownReasonCounts? Level3UnknownReasons = null,
    MilestoneUnknownReasonCounts? Level6UnknownReasons = null,
    RiotApiRequestStatus? ApiStatus = null,
    TimeSpan? RetryAfter = null)
{
    public static MatchProcessingResult Rejected(MatchValidationOutcome outcome) =>
        new(
            Status: MatchProcessingStatus.Rejected,
            Message: outcome.ToString(),
            ValidationOutcome: outcome,
            Observations: [],
            AssistFreeCandidateCount: 0,
            AssistedCandidateCount: 0);

    public static MatchProcessingResult ApiFailure(
        string operation,
        RiotApiRequestStatus apiStatus,
        TimeSpan? retryAfter)
    {
        var status = apiStatus == RiotApiRequestStatus.AuthenticationFailed
            ? MatchProcessingStatus.AuthenticationFailed
            : apiStatus == RiotApiRequestStatus.RateLimited
                ? MatchProcessingStatus.RateLimitExhausted
                : apiStatus == RiotApiRequestStatus.Cancelled
                    ? MatchProcessingStatus.Cancelled
                    : MatchProcessingStatus.ApiFailure;

        return new MatchProcessingResult(
            Status: status,
            Message: $"{operation}: {GetApiFailureDescription(apiStatus)}",
            ValidationOutcome: null,
            Observations: [],
            AssistFreeCandidateCount: 0,
            AssistedCandidateCount: 0,
            ApiStatus: apiStatus,
            RetryAfter: retryAfter);
    }

    private static string GetApiFailureDescription(RiotApiRequestStatus status) =>
        status switch
        {
            RiotApiRequestStatus.AuthenticationFailed => "authentication failed",
            RiotApiRequestStatus.RateLimited => "rate limited",
            RiotApiRequestStatus.ServerError => "Riot server error",
            RiotApiRequestStatus.NetworkError => "network error",
            RiotApiRequestStatus.InvalidResponse => "invalid JSON response",
            RiotApiRequestStatus.Cancelled => "request cancelled",
            _ => "unexpected API response"
        };
}

public enum MatchProcessingStatus
{
    Accepted,
    Rejected,
    ApiFailure,
    AuthenticationFailed,
    RateLimitExhausted,
    Cancelled
}

public enum MatchValidationOutcome
{
    Accepted,
    WrongQueue,
    PatchMismatch,
    InsufficientGameDuration,
    InvalidGameVersion,
    InvalidParticipantCount,
    InvalidRoleLayout,
    InvalidMatchupCount,
    ObservationFailure
}
