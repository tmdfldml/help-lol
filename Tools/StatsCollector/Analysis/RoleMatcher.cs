using StatsCollector.Api.Dtos;

namespace StatsCollector.Analysis;

public sealed class RoleMatcher
{
    private const int Team100 = 100;
    private const int Team200 = 200;

    public RoleMatchResult CreateMatchups(
        IReadOnlyList<MatchParticipantDto> participants)
    {
        if (!TryBuildTeamRoles(participants, Team100, out var team100Roles)
            || !TryBuildTeamRoles(participants, Team200, out var team200Roles))
        {
            return RoleMatchResult.Invalid;
        }

        var matchups = RoleNormalizer.StandardRoles
            .Select(role => ChampionPairNormalizer.Normalize(
                role,
                team100Roles[role],
                team200Roles[role]))
            .ToList();

        return new RoleMatchResult(true, matchups);
    }

    private static bool TryBuildTeamRoles(
        IReadOnlyList<MatchParticipantDto> participants,
        int teamId,
        out Dictionary<StandardRole, MatchParticipantDto> roles)
    {
        roles = new Dictionary<StandardRole, MatchParticipantDto>();
        var teamParticipants = participants
            .Where(participant => participant.TeamId == teamId)
            .ToList();

        if (teamParticipants.Count != RoleNormalizer.StandardRoles.Count)
        {
            return false;
        }

        foreach (var participant in teamParticipants)
        {
            var role = RoleNormalizer.Normalize(participant.TeamPosition);

            if (role == StandardRole.Unknown
                || !roles.TryAdd(role, participant))
            {
                return false;
            }
        }

        return RoleNormalizer.StandardRoles.All(roles.ContainsKey);
    }
}

public sealed record RoleMatchResult(
    bool IsValid,
    IReadOnlyList<NormalizedRoleMatchup> Matchups)
{
    public static RoleMatchResult Invalid { get; } = new(false, []);
}
