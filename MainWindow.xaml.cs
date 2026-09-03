using System.Windows;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;

namespace LoLMatchup;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _pollingCancellation = new();
    private readonly LiveClientService _liveClientService = new();
    private bool _hasStartedPolling;

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
        await MatchupStatisticsDebugValidator.ValidateAsync();
#endif

        var coordinator = new LiveStatisticsCoordinator(_liveClientService);
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

    private void ApplyStatisticsPresentation(
        MatchupStatisticsPresentation presentation)
    {
        if (presentation.State == MatchupPresentationState.LoadedStatistics)
        {
            StatisticsPanel.DataContext = presentation;
            StatisticsPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Collapsed;
            return;
        }

        StatisticsPanel.Visibility = Visibility.Collapsed;
        StatusText.Text = presentation.StatusText;
        StatusPanel.Visibility = Visibility.Visible;
    }
}
