using StatsCollector.Api.Dtos;

namespace StatsCollector.Analysis;

public static class ChampionPairNormalizer
{
    public static NormalizedRoleMatchup Normalize(
        StandardRole role,
        MatchParticipantDto team100Participant,
        MatchParticipantDto team200Participant)
    {
        var wasSwapped = StringComparer.OrdinalIgnoreCase.Compare(
            team100Participant.ChampionName,
            team200Participant.ChampionName) > 0;

        var participantA = wasSwapped
            ? team200Participant
            : team100Participant;
        var participantB = wasSwapped
            ? team100Participant
            : team200Participant;

        return new NormalizedRoleMatchup(
            role,
            participantA.ChampionName,
            participantB.ChampionName,
            participantA.ParticipantId,
            participantB.ParticipantId,
            participantA.Win,
            wasSwapped);
    }
}

public sealed record NormalizedRoleMatchup(
    StandardRole Role,
    string ChampionA,
    string ChampionB,
    int ParticipantAId,
    int ParticipantBId,
    bool ChampionAWon,
    bool WasSwapped);
