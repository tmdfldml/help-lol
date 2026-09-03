using System.Diagnostics;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Services;

public sealed class LiveStatisticsCoordinator
{
    private readonly LiveClientService _liveClientService;
    private readonly StatisticsDataService _statisticsDataService;
    private readonly MatchupStatisticsService _statisticsService;
    private readonly MatchupStatisticsPresenter _presenter;
    private string? _lastWaitingReason;

    public LiveStatisticsCoordinator(
        LiveClientService liveClientService,
        StatisticsDataService? statisticsDataService = null,
        MatchupStatisticsPresenter? presenter = null)
    {
        _liveClientService = liveClientService;
        _statisticsDataService = statisticsDataService ?? new StatisticsDataService();
        _statisticsService = new MatchupStatisticsService(_statisticsDataService);
        _presenter = presenter ?? new MatchupStatisticsPresenter();
    }

    public async Task<LiveMatchupSignature?> GetCurrentMatchupAsync(
        CancellationToken cancellationToken = default)
    {
        var players = await _liveClientService.GetPlayersAsync(cancellationToken);

        if (players is null)
        {
            return Wait("playerlist fetch failed");
        }

        var activePlayer = await _liveClientService.GetActivePlayerAsync(
            players,
            cancellationToken);

        if (activePlayer is null)
        {
            return Wait("active player lookup failed");
        }

        if (activePlayer.NormalizedPosition == PositionNormalizer.Unknown)
        {
            return Wait("active player role invalid");
        }

        if (string.IsNullOrWhiteSpace(activePlayer.Team)
            || players.Any(player => string.IsNullOrWhiteSpace(player.Team)))
        {
            return Wait("opponent team lookup failed");
        }

        if (string.IsNullOrWhiteSpace(activePlayer.ChampionId))
        {
            return Wait("query champion id parse failed");
        }

        bool IsAlly(LiveClientPlayer player) => string.Equals(
            player.Team,
            activePlayer.Team,
            StringComparison.OrdinalIgnoreCase);
        var alliedTeam = new TeamComposition(players.Where(IsAlly));
        var opposingTeam = new TeamComposition(players.Where(player => !IsAlly(player)));
        var roleMatchup = RoleMatchup.CreateAll(alliedTeam, opposingTeam)
            .FirstOrDefault(matchup =>
                matchup.Role == activePlayer.NormalizedPosition
                && ReferenceEquals(matchup.AlliedPlayer, activePlayer));
        if (roleMatchup?.OpposingPlayer is null)
        {
            return Wait("same-role opponent lookup failed");
        }

        var opponentChampion = roleMatchup.OpposingPlayer.ChampionId;

        if (string.IsNullOrWhiteSpace(opponentChampion))
        {
            return Wait("opponent champion id parse failed");
        }

        _lastWaitingReason = null;

        return new LiveMatchupSignature(
            activePlayer.NormalizedPosition,
            activePlayer.ChampionId,
            opponentChampion);
    }

    private LiveMatchupSignature? Wait(string reason)
    {
        if (!string.Equals(_lastWaitingReason, reason, StringComparison.Ordinal))
        {
            Debug.WriteLine($"Live Statistics waiting: {reason}");
            _lastWaitingReason = reason;
        }

        return null;
    }

    public async Task<MatchupStatisticsPresentation> GetPresentationAsync(
        LiveMatchupSignature signature)
    {
        Debug.WriteLine(
            $"Live matchup detected: {signature.Role} "
            + $"{signature.QueryChampion} vs {signature.OpponentChampion}");
        var query = await _statisticsService.GetStatisticsAsync(
            signature.Role,
            signature.QueryChampion,
            signature.OpponentChampion);
        Debug.WriteLine($"Statistics lookup: {query.Status}");

        if (query.Status != MatchupStatisticsQueryStatus.Success
            || query.Statistics is null)
        {
            return _presenter.PresentStatus(query.Status);
        }

        var loadResult = await _statisticsDataService.LoadAsync();

        if (loadResult.Dataset is null)
        {
            return _presenter.PresentStatus(
                MatchupStatisticsQueryStatus.DatasetNotAvailable);
        }

        return _presenter.Present(query.Statistics, loadResult.Dataset);
    }
}

public readonly record struct LiveMatchupSignature(
    string Role,
    string QueryChampion,
    string OpponentChampion);
