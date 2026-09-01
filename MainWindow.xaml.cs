using System.Diagnostics;
using System.Windows;
using LoLMatchup.Services;

namespace LoLMatchup;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _hasTestedLiveClientConnection;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasTestedLiveClientConnection)
        {
            return;
        }

        _hasTestedLiveClientConnection = true;

        var service = new LiveClientService();
        var players = await service.GetPlayersAsync();

        if (players is null)
        {
            Debug.WriteLine("게임 없음 또는 Live Client API 연결 실패");
            return;
        }

        Debug.WriteLine($"플레이어 수: {players.Count}");

        var activePlayer = await service.GetActivePlayerAsync(players);

        if (activePlayer is null)
        {
            Debug.WriteLine("현재 플레이어를 찾을 수 없습니다.");
            return;
        }

        Debug.WriteLine($"현재 플레이어: {activePlayer.RiotId}");
        Debug.WriteLine($"챔피언: {activePlayer.ChampionName}");
        var activePosition = activePlayer.NormalizedPosition;
        Debug.WriteLine($"포지션: {activePosition}");
        Debug.WriteLine($"팀: {activePlayer.Team}");

        if (activePosition == PositionNormalizer.Unknown)
        {
            Debug.WriteLine("현재 플레이어의 포지션을 확인할 수 없습니다.");
        }

        if (string.IsNullOrWhiteSpace(activePlayer.Team)
            || players.Any(player => string.IsNullOrWhiteSpace(player.Team)))
        {
            Debug.WriteLine("팀 정보를 확인할 수 없습니다.");
            return;
        }

        bool IsAlliedPlayer(LiveClientPlayer player) =>
            string.Equals(
                player.Team,
                activePlayer.Team,
                StringComparison.OrdinalIgnoreCase);

        var alliedPlayers = players
            .Where(IsAlliedPlayer)
            .ToList();

        var opposingPlayers = players
            .Where(player => !IsAlliedPlayer(player))
            .ToList();

        Debug.WriteLine($"내 팀: {activePlayer.Team}");
        Debug.WriteLine(string.Empty);
        var alliedTeam = new TeamComposition(alliedPlayers);
        var opposingTeam = new TeamComposition(opposingPlayers);

        WriteTeamComposition("[우리 팀]", alliedTeam);
        Debug.WriteLine(string.Empty);
        WriteTeamComposition("[상대 팀]", opposingTeam);

        Debug.WriteLine(string.Empty);
        Debug.WriteLine("=== ROLE MATCHUPS ===");
        Debug.WriteLine(string.Empty);

        foreach (var matchup in RoleMatchup.CreateAll(alliedTeam, opposingTeam))
        {
            var alliedChampion = matchup.AlliedPlayer?.ChampionName ?? "<없음>";
            var opposingChampion = matchup.OpposingPlayer?.ChampionName ?? "<없음>";

            Debug.WriteLine(
                $"{matchup.Role,-8} {alliedChampion} vs {opposingChampion}");
        }
    }

    private static void WriteTeamComposition(
        string heading,
        TeamComposition composition)
    {
        Debug.WriteLine(heading);

        foreach (var position in PositionNormalizer.StandardPositions)
        {
            var players = composition.GetPlayers(position);

            if (players.Count == 0)
            {
                Debug.WriteLine($"{position,-8} <없음>");
                continue;
            }

            foreach (var player in players)
            {
                Debug.WriteLine($"{position,-8} {player.ChampionName}");
            }
        }
    }
}
