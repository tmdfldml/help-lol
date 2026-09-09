using System.Diagnostics;
using LoLMatchup.Models.Presentation;
#if DEBUG
using LoLMatchup.Services.Development;
#endif

namespace LoLMatchup.Services;

public sealed class LiveStatisticsCoordinator
{
    private readonly LiveClientService _liveClientService;
    private readonly StatisticsDataService _statisticsDataService;
    private readonly MatchupStatisticsService _statisticsService;
    private readonly MatchupStatisticsPresenter _presenter;
#if DEBUG
    private readonly DeveloperTestModeService? _developerTestMode;
#endif
    private string? _lastWaitingReason;

    public LiveStatisticsCoordinator(
        LiveClientService liveClientService,
        StatisticsDataService? statisticsDataService = null,
        MatchupStatisticsPresenter? presenter = null
#if DEBUG
        , DeveloperTestModeService? developerTestMode = null
#endif
        )
    {
        _liveClientService = liveClientService;
        _statisticsDataService = statisticsDataService ?? new StatisticsDataService();
        _statisticsService = new MatchupStatisticsService(_statisticsDataService);
        _presenter = presenter
            ?? new MatchupStatisticsPresenter(new ChampionMetadataService());
#if DEBUG
        _developerTestMode = developerTestMode;
#endif
    }

    public async Task<LiveTeamMatchupSignature?> GetCurrentMatchupAsync(
        CancellationToken cancellationToken = default)
    {
#if DEBUG
        var testScenario = _developerTestMode?.CurrentScenario;
        var players = testScenario?.Players
            ?? await _liveClientService.GetPlayersAsync(cancellationToken);
#else
        var players = await _liveClientService.GetPlayersAsync(cancellationToken);
#endif
        if (players is null)
        {
            return Wait("playerlist fetch failed");
        }

#if DEBUG
        var activePlayer = testScenario is null
            ? await _liveClientService.GetActivePlayerAsync(players, cancellationToken)
            : players.FirstOrDefault(player => string.Equals(
                player.RiotId,
                testScenario.ActivePlayerRiotId,
                StringComparison.OrdinalIgnoreCase));
#else
        var activePlayer = await _liveClientService.GetActivePlayerAsync(players, cancellationToken);
#endif
        if (activePlayer is null)
        {
            return Wait("active player lookup failed");
        }

        if (string.IsNullOrWhiteSpace(activePlayer.Team))
        {
            return Wait("opponent team lookup failed");
        }

        bool IsAlly(LiveClientPlayer player) => string.Equals(
            player.Team,
            activePlayer.Team,
            StringComparison.OrdinalIgnoreCase);
        var alliedTeam = new TeamComposition(players.Where(IsAlly));
        var opposingTeam = new TeamComposition(players.Where(player =>
            !string.IsNullOrWhiteSpace(player.Team) && !IsAlly(player)));
        var generated = RoleMatchup.CreateAll(alliedTeam, opposingTeam);
        var roles = PositionNormalizer.StandardPositions
            .Select(role =>
            {
                var matchup = generated.First(item => item.Role == role);
                return new LiveRoleMatchupSignature(
                    role,
                    matchup.AlliedPlayer?.ChampionId,
                    matchup.OpposingPlayer?.ChampionId);
            })
            .ToArray();

        _lastWaitingReason = null;
#if DEBUG
        var sourceKey = testScenario is null
            ? "LIVE"
            : $"TEST:{_developerTestMode!.Version}";
#else
        const string sourceKey = "LIVE";
#endif
        return new LiveTeamMatchupSignature(
            sourceKey + "|" + string.Join("|", roles.Select(role =>
                $"{role.Role}:{role.QueryChampion ?? "-"}:{role.OpponentChampion ?? "-"}")),
            roles);
    }

    public async Task<TeamMatchupStatisticsPresentation> GetPresentationAsync(
        LiveTeamMatchupSignature signature)
    {
        var loadResult = await _statisticsDataService.LoadAsync();
        if (loadResult.Status == StatisticsDatasetLoadStatus.NotAvailable)
        {
            return GlobalStatus(MatchupStatisticsQueryStatus.DatasetNotAvailable);
        }

        if (loadResult.Status != StatisticsDatasetLoadStatus.Loaded || loadResult.Dataset is null)
        {
            return GlobalStatus(MatchupStatisticsQueryStatus.DatasetInvalid);
        }

        var presentations = new List<MatchupStatisticsPresentation>(5);
        foreach (var role in signature.Roles)
        {
            if (string.IsNullOrWhiteSpace(role.QueryChampion)
                || string.IsNullOrWhiteSpace(role.OpponentChampion))
            {
                presentations.Add(await _presenter.PresentRoleStatusAsync(
                    MatchupStatisticsQueryStatus.InvalidQuery,
                    role.Role,
                    role.QueryChampion,
                    role.OpponentChampion));
                continue;
            }

            var query = await _statisticsService.GetStatisticsAsync(
                role.Role,
                role.QueryChampion,
                role.OpponentChampion);
            Debug.WriteLine($"Statistics lookup [{role.Role}]: {query.Status}");
            presentations.Add(query.Status == MatchupStatisticsQueryStatus.Success
                && query.Statistics is not null
                    ? await _presenter.PresentAsync(query.Statistics, loadResult.Dataset)
                    : await _presenter.PresentRoleStatusAsync(
                        query.Status,
                        role.Role,
                        role.QueryChampion,
                        role.OpponentChampion));
        }

        return new TeamMatchupStatisticsPresentation(
            MatchupPresentationState.LoadedStatistics,
            string.Empty,
            $"패치 {loadResult.Dataset.Patch}",
            presentations,
            OutcomeSummary: TeamOutcomeSummaryPresenter.Create(presentations));
    }

    private TeamMatchupStatisticsPresentation GlobalStatus(
        MatchupStatisticsQueryStatus queryStatus)
    {
        var status = _presenter.PresentStatus(queryStatus);
        return new TeamMatchupStatisticsPresentation(
            status.State,
            status.StatusText,
            StatusDetailText: status.StatusDetailText);
    }

    private LiveTeamMatchupSignature? Wait(string reason)
    {
        if (!string.Equals(_lastWaitingReason, reason, StringComparison.Ordinal))
        {
            Debug.WriteLine($"Live Statistics waiting: {reason}");
            _lastWaitingReason = reason;
        }

        return null;
    }
}

public sealed record LiveTeamMatchupSignature(
    string CacheKey,
    IReadOnlyList<LiveRoleMatchupSignature> Roles);

public sealed record LiveRoleMatchupSignature(
    string Role,
    string? QueryChampion,
    string? OpponentChampion);
