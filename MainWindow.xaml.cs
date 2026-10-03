using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LoLMatchup.Models.Navigation;
using LoLMatchup.Models.Settings;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;
using LoLMatchup.Services.Logging;
using LoLMatchup.Services.Diagnostics;
#if DEBUG
using LoLMatchup.Services.Development;
#endif
using LoLMatchup.Services.Settings;
using LoLMatchup.Services.Navigation;
using LoLMatchup.Views;

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
    private readonly ChampionMetadataService _championMetadataService;
    private readonly StatisticsDataService _statisticsDataService;
    private readonly AppLogger _logger;
    private readonly AppDiagnosticsService _diagnostics;
    private readonly AppSettingsService _settingsService = new();
    private readonly AppNavigationService _navigationService = new();
    private readonly ChampionSkillDataService _skillDataService;
    private SettingsWindow? _settingsWindow;
#if DEBUG
    private readonly DeveloperTestModeService _developerTestMode = new();
    private DeveloperTestWindow? _developerTestWindow;
#endif
    private bool _hasStartedPolling;
    private bool _applyingSettings;
    private bool _gameActive;
    private StatisticsLayoutMode _layoutMode = StatisticsLayoutMode.Portrait;

    public MainWindow()
    {
        var app = (App)Application.Current;
        _logger = app.Logger;
        _skillDataService = new ChampionSkillDataService(logger: _logger);
        _diagnostics = app.Diagnostics;
        _statisticsDataService = new StatisticsDataService(logger: _logger);
        _championMetadataService = new ChampionMetadataService(_logger);
        InitializeComponent();
        ApplySavedSettings();
        _settingsService.SettingsChanged += SettingsService_SettingsChanged;
        _navigationService.NavigationChanged += NavigationService_NavigationChanged;
        HomeView.InGameRequested += HomeView_InGameRequested;
        InGameView.RoleSelected += InGameView_RoleSelected;
        InGameView.EnemySelected += InGameView_EnemySelected;
        ChampionSkillDetailView.BackRequested += MatchupDetailView_BackRequested;
        MatchupDetailView.BackRequested += MatchupDetailView_BackRequested;
#if DEBUG
        DeveloperTestButton.Visibility = Visibility.Visible;
        _developerTestMode.StateChanged += DeveloperTestMode_StateChanged;
#endif
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasStartedPolling)
        {
            return;
        }

        _hasStartedPolling = true;

        var dataset = await _statisticsDataService.LoadAsync();
        if (_pollingCancellation.IsCancellationRequested) return;
        if (dataset.Status != StatisticsDatasetLoadStatus.Loaded)
        {
            ((App)Application.Current).HandleFatal(new InvalidOperationException(), AppLogEvent.FatalStartup);
            return;
        }
        var manifest = await _statisticsDataService.LoadCompanionManifestAsync(dataset.Source);
        if (_pollingCancellation.IsCancellationRequested) return;
        _diagnostics.SetDataset(dataset, manifest);
        _logger.LogSnapshot(_diagnostics.Snapshot);
        HomeView.SetDataset(
            dataset.Dataset,
            dataset.Status == StatisticsDatasetLoadStatus.Invalid
                ? "통계 데이터 오류"
                : "통계 데이터 없음");

#if DEBUG
        await MatchupStatisticsDebugValidator.ValidateAsync(_championMetadataService);
#endif

        var presenter = new MatchupStatisticsPresenter(_championMetadataService);
        var coordinator = new LiveStatisticsCoordinator(
            _liveClientService,
            statisticsDataService: _statisticsDataService,
            presenter: presenter
#if DEBUG
            , developerTestMode: _developerTestMode
#endif
            );
        if (_pollingCancellation.IsCancellationRequested) return;
        var pollingService = new LiveStatisticsPollingService(coordinator, logger: _logger);

        try
        {
            await pollingService.RunAsync(
                (presentation, gameActive) => Dispatcher.InvokeAsync(
                    () => ApplyStatisticsPresentation(presentation, gameActive)).Task,
                _pollingCancellation.Token);
        }
        catch (OperationCanceledException) when (_pollingCancellation.IsCancellationRequested)
        {
            // Normal window shutdown.
        }
        finally
        {
            _liveClientService.Dispose();
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _settingsService.SettingsChanged -= SettingsService_SettingsChanged;
        _navigationService.NavigationChanged -= NavigationService_NavigationChanged;
        HomeView.InGameRequested -= HomeView_InGameRequested;
        InGameView.RoleSelected -= InGameView_RoleSelected;
        InGameView.EnemySelected -= InGameView_EnemySelected;
        ChampionSkillDetailView.BackRequested -= MatchupDetailView_BackRequested;
        MatchupDetailView.BackRequested -= MatchupDetailView_BackRequested;
        _pollingCancellation.Cancel();
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        SaveWindowSettings();
    }

    private void AlwaysOnTopToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_applyingSettings)
        {
            return;
        }

        var enabled = AlwaysOnTopToggle.IsChecked == true;
        Topmost = enabled;
        _settingsService.Update(settings => settings with { AlwaysOnTop = enabled });
        _settingsService.Save();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
        => OpenSettingsWindow();

    private void OpenSettingsWindow()
    {
        if (_settingsWindow is { IsLoaded: true })
        {
            _settingsWindow.Activate();
            _settingsWindow.Focus();
            return;
        }

        _settingsWindow = new SettingsWindow(
            _settingsService,
            _statisticsDataService, _diagnostics, _logger)
        {
            Owner = this
        };
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
    }

#if DEBUG
    internal void ShowUpdateFailureFixture()
    {
        _settingsWindow?.Close();
        _settingsWindow = new SettingsWindow(_settingsService, _statisticsDataService, _diagnostics, _logger,
            new LoLMatchup.Services.Statistics.DatasetUpdateOptions { ManifestUri = new Uri("http://127.0.0.1:9/manifest.json") })
        { Owner = this };
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
    }
#endif
    private void NavigationButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationOverlay.Visibility == Visibility.Visible)
        {
            CloseDrawer();
            return;
        }
        NavigationOverlay.Visibility = Visibility.Visible;
        HomeNavigationItem.Focus();
    }
    private void DrawerBackdrop_Click(object sender, RoutedEventArgs e) => CloseDrawer();

    private void HomeNavigation_Click(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigate(AppPage.Home);
        CloseDrawer();
    }

    private void InGameNavigation_Click(object sender, RoutedEventArgs e)
    {
        _navigationService.Navigate(AppPage.InGame);
        CloseDrawer();
    }

    private void DrawerSettings_Click(object sender, RoutedEventArgs e)
    {
        CloseDrawer();
        OpenSettingsWindow();
    }

    private void HomeView_InGameRequested(object? sender, EventArgs e) =>
        _navigationService.Navigate(AppPage.InGame);

    private void InGameView_RoleSelected(
        object? sender,
        MatchupStatisticsPresentation presentation)
    {
        if (!presentation.CanOpenDetail)
        {
            return;
        }

        _navigationService.NavigateToMatchupDetail(new MatchupDetailNavigationContext(
            presentation.Role,
            presentation.QueryChampionId,
            presentation.OpponentChampionId,
            presentation.PatchDisplayText,
            presentation));
    }

    private void MatchupDetailView_BackRequested(object? sender, EventArgs e) =>
        _navigationService.Navigate(AppPage.InGame);

    private void InGameView_EnemySelected(object? sender, MatchupStatisticsPresentation presentation)
    {
        if (!string.IsNullOrWhiteSpace(presentation.OpponentChampionId))
            _navigationService.NavigateToChampionSkillDetail(presentation);
    }

    private async void NavigationService_NavigationChanged(object? sender, EventArgs e)
    {
        var showHome = _navigationService.CurrentPage == AppPage.Home;
        var showDetail = _navigationService.CurrentPage == AppPage.MatchupDetail;
        var showSkills = _navigationService.CurrentPage == AppPage.ChampionSkillDetail;
        HomeView.Visibility = showHome ? Visibility.Visible : Visibility.Collapsed;
        InGameView.Visibility = !showHome && !showDetail && !showSkills ? Visibility.Visible : Visibility.Collapsed;
        ChampionSkillDetailView.Visibility = showSkills ? Visibility.Visible : Visibility.Collapsed;
        MatchupDetailView.Visibility = showDetail ? Visibility.Visible : Visibility.Collapsed;
        if (showDetail && _navigationService.DetailContext is not null)
        {
            MatchupDetailView.SetContext(_navigationService.DetailContext);
        }
        HomeNavigationItem.IsChecked = showHome;
        InGameNavigationItem.IsChecked = !showHome;
        CurrentPageText.Text = showHome ? "홈" : showSkills ? "인게임 · 상대 스킬" : showDetail ? "인게임 · 매치업 상세" : "인게임";
        if (showSkills && _navigationService.SkillContext is { } skill)
            await ChampionSkillDetailView.SetContextAsync(skill, _skillDataService, _diagnostics.Snapshot.CurrentGamePatch);
    }

    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && NavigationOverlay.Visibility == Visibility.Visible)
        {
            CloseDrawer();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape
                 && _navigationService.CurrentPage is AppPage.MatchupDetail or AppPage.ChampionSkillDetail)
        {
            _navigationService.Navigate(AppPage.InGame);
            e.Handled = true;
        }
    }

    private void CloseDrawer()
    {
        NavigationOverlay.Visibility = Visibility.Collapsed;
        NavigationButton.Focus();
    }

    private void SettingsService_SettingsChanged(object? sender, EventArgs e)
    {
        _applyingSettings = true;
        Topmost = _settingsService.Current.AlwaysOnTop;
        AlwaysOnTopToggle.IsChecked = _settingsService.Current.AlwaysOnTop;
        _applyingSettings = false;
    }

    private void DeveloperTestButton_Click(object sender, RoutedEventArgs e)
    {
#if DEBUG
        if (_developerTestWindow is { IsLoaded: true })
        {
            _developerTestWindow.Activate();
            return;
        }

        _developerTestWindow = new DeveloperTestWindow(
            _developerTestMode,
            _championMetadataService)
        {
            Owner = this
        };
        _developerTestWindow.Closed += (_, _) => _developerTestWindow = null;
        _developerTestWindow.Show();
#endif
    }

#if DEBUG
    private void DeveloperTestMode_StateChanged(object? sender, EventArgs e)
    {
        UpdateHeaderStatus();
    }
#endif

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

        RememberLayoutSize(_layoutMode);
        _layoutMode = mode;
        var isLandscape = mode == StatisticsLayoutMode.Landscape;
        InGameView.SetLandscape(isLandscape);
        MinWidth = Math.Min(
            isLandscape ? LandscapeMinWidth : PortraitMinWidth,
            Math.Max(420, SystemParameters.WorkArea.Width));
        MinHeight = Math.Min(
            isLandscape ? LandscapeMinHeight : PortraitMinHeight,
            Math.Max(420, SystemParameters.WorkArea.Height));
        _settingsService.Update(settings => settings with
        {
            LastLayoutMode = isLandscape ? AppLayoutMode.Landscape : AppLayoutMode.Portrait
        });
    }

    private void ApplySavedSettings()
    {
        var settings = _settingsService.Load();
        _applyingSettings = true;
        Topmost = settings.AlwaysOnTop;
        AlwaysOnTopToggle.IsChecked = settings.AlwaysOnTop;

        var fallback = new Size(Width, Height);
        var size = settings.RememberWindowSize
            ? WindowBoundsService.ValidateSize(
                settings.LastWindowWidth,
                settings.LastWindowHeight,
                fallback)
            : fallback;
        if (size.Width / size.Height >= LandscapeEnterRatio)
        {
            SetLayoutMode(StatisticsLayoutMode.Landscape);
        }
        Width = size.Width;
        Height = size.Height;

        if (settings.RememberWindowPosition)
        {
            var position = WindowBoundsService.ValidatePosition(
                settings.LastWindowLeft,
                settings.LastWindowTop,
                size);
            if (position is not null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = position.Value.X;
                Top = position.Value.Y;
            }
            else if (settings.LastWindowLeft.HasValue || settings.LastWindowTop.HasValue)
            {
                var recovered = WindowBoundsService.CenterInPrimaryWorkArea(size);
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = recovered.X;
                Top = recovered.Y;
            }
        }

        _applyingSettings = false;
    }

    private void SaveWindowSettings()
    {
        var settings = _settingsService.Current;
        var bounds = WindowState == WindowState.Normal
            ? new Rect(Left, Top, ActualWidth, ActualHeight)
            : RestoreBounds;
        var validSize = WindowBoundsService.ValidateSize(
            bounds.Width,
            bounds.Height,
            new Size(Width, Height));
        var validPosition = WindowBoundsService.ValidatePosition(
            bounds.Left,
            bounds.Top,
            validSize) ?? WindowBoundsService.CenterInPrimaryWorkArea(validSize);

        _settingsService.Update(current => current with
        {
            LastWindowLeft = settings.RememberWindowPosition ? validPosition.X : current.LastWindowLeft,
            LastWindowTop = settings.RememberWindowPosition ? validPosition.Y : current.LastWindowTop,
            LastWindowWidth = settings.RememberWindowSize ? validSize.Width : current.LastWindowWidth,
            LastWindowHeight = settings.RememberWindowSize ? validSize.Height : current.LastWindowHeight,
            LastLayoutMode = _layoutMode == StatisticsLayoutMode.Landscape
                ? AppLayoutMode.Landscape
                : AppLayoutMode.Portrait,
            PortraitWidth = settings.RememberWindowSize
                && _layoutMode == StatisticsLayoutMode.Portrait
                ? validSize.Width
                : current.PortraitWidth,
            PortraitHeight = settings.RememberWindowSize
                && _layoutMode == StatisticsLayoutMode.Portrait
                ? validSize.Height
                : current.PortraitHeight,
            LandscapeWidth = settings.RememberWindowSize
                && _layoutMode == StatisticsLayoutMode.Landscape
                ? validSize.Width
                : current.LandscapeWidth,
            LandscapeHeight = settings.RememberWindowSize
                && _layoutMode == StatisticsLayoutMode.Landscape
                ? validSize.Height
                : current.LandscapeHeight
        });
        _settingsService.Save();
    }

    private void RememberLayoutSize(StatisticsLayoutMode mode)
    {
        if (!_settingsService.Current.RememberWindowSize
            || !double.IsFinite(ActualWidth) || !double.IsFinite(ActualHeight)
            || ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        _settingsService.Update(settings => mode == StatisticsLayoutMode.Portrait
            ? settings with { PortraitWidth = ActualWidth, PortraitHeight = ActualHeight }
            : settings with { LandscapeWidth = ActualWidth, LandscapeHeight = ActualHeight });
    }

    private void ApplyStatisticsPresentation(
        TeamMatchupStatisticsPresentation presentation,
        bool gameActive)
    {
        var newlyDetected = gameActive && !_gameActive;
        _gameActive = gameActive;
        if (!gameActive) _diagnostics.SetCurrentGameVersion(null);
        UpdateHeaderStatus();
        HomeView.SetGameActive(gameActive);
        InGameView.SetPresentation(presentation);

        if (_navigationService.CurrentPage == AppPage.ChampionSkillDetail
            && (!gameActive || _navigationService.SkillContext is not { } skill
                || !presentation.DisplayMatchups.Any(item => item.Role == skill.Role
                    && item.OpponentChampionId == skill.OpponentChampionId)))
            _navigationService.Navigate(AppPage.InGame);

        if (!gameActive && _navigationService.CurrentPage == AppPage.MatchupDetail)
        {
            _navigationService.Navigate(AppPage.InGame);
        }
        else if (gameActive
                 && _navigationService.CurrentPage == AppPage.MatchupDetail
                 && _navigationService.DetailContext is { } detail)
        {
            var updated = presentation.DisplayMatchups.FirstOrDefault(item =>
                item.Role == detail.Role
                && string.Equals(item.QueryChampionId, detail.AllyChampionId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.OpponentChampionId, detail.EnemyChampionId, StringComparison.OrdinalIgnoreCase));
            if (updated is null)
            {
                _navigationService.Navigate(AppPage.InGame);
            }
            else
            {
                _navigationService.NavigateToMatchupDetail(detail with
                {
                    PatchDisplayText = presentation.PatchDisplayText,
                    Presentation = updated
                });
            }
        }

        if (newlyDetected)
        {
            _navigationService.Navigate(AppPage.InGame);
        }
    }

    private void UpdateHeaderStatus()
    {
#if DEBUG
        if (_developerTestMode.IsActive)
        {
            ModeBadgeText.Text = "TEST MODE";
            return;
        }
#endif
        ModeBadgeText.Text = _gameActive ? "LIVE" : "WAITING";
    }

    private enum StatisticsLayoutMode
    {
        Portrait,
        Landscape
    }
}
