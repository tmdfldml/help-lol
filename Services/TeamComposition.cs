namespace LoLMatchup.Services;

public sealed class TeamComposition
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<LiveClientPlayer>>
        _playersByPosition;

    public TeamComposition(IEnumerable<LiveClientPlayer> players)
    {
        _playersByPosition = players
            .Where(player => player.NormalizedPosition != PositionNormalizer.Unknown)
            .GroupBy(player => player.NormalizedPosition)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LiveClientPlayer>)group.ToList());
    }

    public IReadOnlyList<LiveClientPlayer> GetPlayers(string position)
    {
        return _playersByPosition.TryGetValue(position, out var players)
            ? players
            : Array.Empty<LiveClientPlayer>();
    }
}
