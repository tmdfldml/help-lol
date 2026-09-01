namespace LoLMatchup.Services;

public sealed class RoleMatchup
{
    public RoleMatchup(
        string role,
        LiveClientPlayer? alliedPlayer,
        LiveClientPlayer? opposingPlayer)
    {
        Role = role;
        AlliedPlayer = alliedPlayer;
        OpposingPlayer = opposingPlayer;
    }

    public string Role { get; }

    public LiveClientPlayer? AlliedPlayer { get; }

    public LiveClientPlayer? OpposingPlayer { get; }

    public static IReadOnlyList<RoleMatchup> CreateAll(
        TeamComposition alliedTeam,
        TeamComposition opposingTeam)
    {
        var matchups = new List<RoleMatchup>();

        foreach (var role in PositionNormalizer.StandardPositions)
        {
            var alliedPlayers = alliedTeam.GetPlayers(role);
            var opposingPlayers = opposingTeam.GetPlayers(role);
            var matchupCount = Math.Max(
                1,
                Math.Max(alliedPlayers.Count, opposingPlayers.Count));

            for (var index = 0; index < matchupCount; index++)
            {
                matchups.Add(new RoleMatchup(
                    role,
                    index < alliedPlayers.Count ? alliedPlayers[index] : null,
                    index < opposingPlayers.Count ? opposingPlayers[index] : null));
            }
        }

        return matchups;
    }
}
