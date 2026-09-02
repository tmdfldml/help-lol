using StatsCollector.Api.Dtos;

namespace StatsCollector.Analysis;

public sealed class TimelineMetricExtractor
{
    private const long FifteenMinutesInMilliseconds = 900_000;

    public TimelineMetricExtractionResult Extract(
        IReadOnlyList<NormalizedRoleMatchup> matchups,
        MatchTimelineDto? timeline)
    {
        if (matchups.Count != RoleNormalizer.StandardRoles.Count
            || timeline?.Info?.Frames is not { Count: > 0 } frames)
        {
            return TimelineMetricExtractionResult.Failed;
        }

        var events = frames
            .SelectMany(frame => frame.Events ?? [])
            .ToList();
        var levelTimestamps = BuildLevelTimestampIndex(events);
        var allKillEvents = events
            .Where(timelineEvent => timelineEvent.Type == "CHAMPION_KILL")
            .ToList();
        var killEvents = allKillEvents
            .Where(timelineEvent =>
                timelineEvent.Timestamp is >= 0 and < FifteenMinutesInMilliseconds)
            .ToList();
        var observations = new List<MatchupObservation>(matchups.Count);
        var assistFreeCandidateCount = 0;
        var assistedCandidateCount = 0;
        var level2UnknownReasons = new MutableUnknownReasonCounts();
        var level3UnknownReasons = new MutableUnknownReasonCounts();
        var level6UnknownReasons = new MutableUnknownReasonCounts();

        foreach (var matchup in matchups)
        {
            var soloKillsForA = 0;
            var soloKillsForB = 0;

            foreach (var killEvent in killEvents)
            {
                var killedB = killEvent.KillerId == matchup.ParticipantAId
                    && killEvent.VictimId == matchup.ParticipantBId;
                var killedA = killEvent.KillerId == matchup.ParticipantBId
                    && killEvent.VictimId == matchup.ParticipantAId;

                if (!killedA && !killedB)
                {
                    continue;
                }

                if (HasAssists(killEvent))
                {
                    assistedCandidateCount++;
                    continue;
                }

                // Observed Riot timelines represent assist-free kills with either
                // a missing/null list or an empty list. Keep this rule centralized.
                assistFreeCandidateCount++;

                if (killedB)
                {
                    soloKillsForA++;
                }
                else
                {
                    soloKillsForB++;
                }
            }

            var level2 = CompareLevelTimes(matchup, 2, levelTimestamps);
            var level3 = CompareLevelTimes(matchup, 3, levelTimestamps);
            var level6 = CompareLevelTimes(matchup, 6, levelTimestamps);
            level2UnknownReasons.Add(level2.UnknownReason);
            level3UnknownReasons.Add(level3.UnknownReason);
            level6UnknownReasons.Add(level6.UnknownReason);

            observations.Add(new MatchupObservation(
                matchup.Role,
                matchup.ChampionA,
                matchup.ChampionB,
                matchup.ChampionAWon,
                level2.Result,
                level3.Result,
                level6.Result,
                soloKillsForA,
                soloKillsForB));
        }

        return new TimelineMetricExtractionResult(
            true,
            observations,
            assistFreeCandidateCount,
            assistedCandidateCount,
            allKillEvents.Count(kill =>
                kill.AssistingParticipantIds is { Count: > 0 }),
            allKillEvents.Count(kill => kill.AssistingParticipantIds is null),
            allKillEvents.Count(kill =>
                kill.AssistingParticipantIds is { Count: 0 }),
            level2UnknownReasons.ToImmutable(),
            level3UnknownReasons.ToImmutable(),
            level6UnknownReasons.ToImmutable());
    }

    private static Dictionary<(int ParticipantId, int Level), long>
        BuildLevelTimestampIndex(IEnumerable<TimelineEventDto> events)
    {
        var timestamps = new Dictionary<(int ParticipantId, int Level), long>();

        foreach (var timelineEvent in events)
        {
            if (timelineEvent.Type != "LEVEL_UP"
                || timelineEvent.ParticipantId is not { } participantId
                || timelineEvent.Level is not { } level
                || timelineEvent.Timestamp is not { } timestamp)
            {
                continue;
            }

            var key = (participantId, level);

            if (!timestamps.TryGetValue(key, out var existingTimestamp)
                || timestamp < existingTimestamp)
            {
                timestamps[key] = timestamp;
            }
        }

        return timestamps;
    }

    private static LevelComparison CompareLevelTimes(
        NormalizedRoleMatchup matchup,
        int level,
        IReadOnlyDictionary<(int ParticipantId, int Level), long> timestamps)
    {
        var hasA = timestamps.TryGetValue(
            (matchup.ParticipantAId, level),
            out var timeA);
        var hasB = timestamps.TryGetValue(
            (matchup.ParticipantBId, level),
            out var timeB);

        if (!hasA && !hasB)
        {
            return new LevelComparison(
                MilestoneResult.Unknown,
                MilestoneUnknownReason.MissingBoth);
        }

        if (!hasA)
        {
            return new LevelComparison(
                MilestoneResult.Unknown,
                MilestoneUnknownReason.MissingA);
        }

        if (!hasB)
        {
            return new LevelComparison(
                MilestoneResult.Unknown,
                MilestoneUnknownReason.MissingB);
        }

        if (timeA == timeB)
        {
            return new LevelComparison(
                MilestoneResult.Unknown,
                MilestoneUnknownReason.Tie);
        }

        return new LevelComparison(
            timeA < timeB ? MilestoneResult.A : MilestoneResult.B,
            MilestoneUnknownReason.None);
    }

    private static bool HasAssists(TimelineEventDto killEvent)
    {
        return killEvent.AssistingParticipantIds is { Count: > 0 };
    }
}

public sealed record MatchupObservation(
    StandardRole Role,
    string ChampionA,
    string ChampionB,
    bool ChampionAWon,
    MilestoneResult FirstLevel2Result,
    MilestoneResult FirstLevel3Result,
    MilestoneResult FirstLevel6Result,
    int SoloKillsBefore15ForA,
    int SoloKillsBefore15ForB);

public sealed record TimelineMetricExtractionResult(
    bool IsSuccess,
    IReadOnlyList<MatchupObservation> Observations,
    int AssistFreeCandidateCount,
    int AssistedCandidateCount,
    int KillEventsWithAssists,
    int KillEventsWithNullAssistList,
    int KillEventsWithEmptyAssistList,
    MilestoneUnknownReasonCounts Level2UnknownReasons,
    MilestoneUnknownReasonCounts Level3UnknownReasons,
    MilestoneUnknownReasonCounts Level6UnknownReasons)
{
    public static TimelineMetricExtractionResult Failed { get; } =
        new(false, [], 0, 0, 0, 0, 0, new(), new(), new());
}

public enum MilestoneResult
{
    Unknown,
    A,
    B
}

public enum MilestoneUnknownReason
{
    None,
    Tie,
    MissingA,
    MissingB,
    MissingBoth
}

public sealed record MilestoneUnknownReasonCounts(
    int Tie = 0,
    int MissingA = 0,
    int MissingB = 0,
    int MissingBoth = 0)
{
    public int Total => Tie + MissingA + MissingB + MissingBoth;

    public static MilestoneUnknownReasonCounts operator +(
        MilestoneUnknownReasonCounts left,
        MilestoneUnknownReasonCounts right) => new(
            left.Tie + right.Tie,
            left.MissingA + right.MissingA,
            left.MissingB + right.MissingB,
            left.MissingBoth + right.MissingBoth);
}

internal readonly record struct LevelComparison(
    MilestoneResult Result,
    MilestoneUnknownReason UnknownReason);

internal sealed class MutableUnknownReasonCounts
{
    private int _tie;
    private int _missingA;
    private int _missingB;
    private int _missingBoth;

    public void Add(MilestoneUnknownReason reason)
    {
        switch (reason)
        {
            case MilestoneUnknownReason.Tie:
                _tie++;
                break;
            case MilestoneUnknownReason.MissingA:
                _missingA++;
                break;
            case MilestoneUnknownReason.MissingB:
                _missingB++;
                break;
            case MilestoneUnknownReason.MissingBoth:
                _missingBoth++;
                break;
        }
    }

    public MilestoneUnknownReasonCounts ToImmutable() =>
        new(_tie, _missingA, _missingB, _missingBoth);
}
