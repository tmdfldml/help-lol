using System.Windows;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;

namespace LoLMatchup;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const double LandscapeEnterRatio = 1.15;
    private const double LandscapeExitRatio = 1.05;
    private const double PortraitMinWidth = 420;
    private const double PortraitMinHeight = 760;
    private const double LandscapeMinWidth = 760;
    private const double LandscapeMinHeight = 420;

    private readonly CancellationTokenSource _pollingCancellation = new();
    private readonly LiveClientService _liveClientService = new();
    private bool _hasStartedPolling;
    private StatisticsLayoutMode _layoutMode = StatisticsLayoutMode.Portrait;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasStartedPolling)
        {
            return;
        }

        _hasStartedPolling = true;

#if DEBUG
        var championMetadataService = new ChampionMetadataService();
        await MatchupStatisticsDebugValidator.ValidateAsync(championMetadataService);
#else
        var championMetadataService = new ChampionMetadataService();
#endif

        var presenter = new MatchupStatisticsPresenter(championMetadataService);
        var coordinator = new LiveStatisticsCoordinator(
            _liveClientService,
            presenter: presenter);
        var pollingService = new LiveStatisticsPollingService(coordinator);

        try
        {
            await pollingService.RunAsync(
                presentation => Dispatcher.InvokeAsync(
                    () => ApplyStatisticsPresentation(presentation)).Task,
                _pollingCancellation.Token);
        }
        catch (OperationCanceledException) when (_pollingCancellation.IsCancellationRequested)
        {
            // Normal window shutdown.
        }
        finally
        {
            _liveClientService.Dispose();
            _pollingCancellation.Dispose();
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _pollingCancellation.Cancel();
    }

    private void RootLayout_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Height <= 0)
        {
            return;
        }

        var ratio = e.NewSize.Width / e.NewSize.Height;

        if (_layoutMode == StatisticsLayoutMode.Portrait
            && ratio >= LandscapeEnterRatio)
        {
            SetLayoutMode(StatisticsLayoutMode.Landscape);
        }
        else if (_layoutMode == StatisticsLayoutMode.Landscape
                 && ratio <= LandscapeExitRatio)
        {
            SetLayoutMode(StatisticsLayoutMode.Portrait);
        }
    }

    private void SetLayoutMode(StatisticsLayoutMode mode)
    {
        if (_layoutMode == mode)
        {
            return;
        }

        _layoutMode = mode;
        var isLandscape = mode == StatisticsLayoutMode.Landscape;
        PortraitStatisticsView.Visibility = isLandscape
            ? Visibility.Collapsed
            : Visibility.Visible;
        LandscapeStatisticsView.Visibility = isLandscape
            ? Visibility.Visible
            : Visibility.Collapsed;
        MinWidth = isLandscape ? LandscapeMinWidth : PortraitMinWidth;
        MinHeight = isLandscape ? LandscapeMinHeight : PortraitMinHeight;
    }

    private void ApplyStatisticsPresentation(
        TeamMatchupStatisticsPresentation presentation)
    {
        if (presentation.State == MatchupPresentationState.LoadedStatistics)
        {
            StatisticsPanel.DataContext = presentation;
            StatisticsPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Collapsed;
            return;
        }

        StatisticsPanel.Visibility = Visibility.Collapsed;
        StatusPanel.DataContext = presentation;
        StatusPanel.Visibility = Visibility.Visible;
    }

    private enum StatisticsLayoutMode
    {
        Portrait,
        Landscape
    }
}
