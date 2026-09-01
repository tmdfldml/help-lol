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

        foreach (var player in players)
        {
            Debug.WriteLine(
                $"{player.ChampionName} | {player.Position} | {player.Team}");
        }
    }
}
