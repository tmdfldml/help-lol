using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using LoLMatchup.Models.Settings;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services;
using LoLMatchup.Services.Settings;
using LoLMatchup.Services.Statistics;

namespace LoLMatchup.Views;

public partial class SettingsWindow : Window
{
    private readonly AppSettingsService _settingsService;
    private readonly StatisticsDataService _statisticsDataService;
    private readonly DatasetUpdateService _datasetUpdateService;
    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private readonly bool _updateEndpointConfigured;
    private bool _applyingSettings;
    private bool _updateBusy;
    private DatasetUpdateManifest? _availableUpdate;
    private DatasetUpdateManifest? _currentDatasetManifest;
    private long _currentDatasetRevision;
    private int _currentAcceptedMatches;

    public SettingsWindow(
        AppSettingsService settingsService,
        StatisticsDataService statisticsDataService)
    {
        _settingsService = settingsService;
        _statisticsDataService = statisticsDataService;
        var updateOptions = DatasetUpdateOptions.FromEnvironment();
        _updateEndpointConfigured = updateOptions.ManifestUri is not null;
        _datasetUpdateService = new DatasetUpdateService(
            updateOptions,
            _statisticsDataService.UpdatedDataFilePath);
        InitializeComponent();

        SettingsPathText.Text = _settingsService.SettingsPath;
        VersionText.Text = $"Version {GetApplicationVersion()}";
        ApplySettings(_settingsService.Current);
        _settingsService.SettingsChanged += SettingsService_SettingsChanged;
        Loaded += SettingsWindow_Loaded;
        Closed += SettingsWindow_Closed;
    }

    private async void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var result = await _statisticsDataService.LoadAsync();
        var manifest = await _statisticsDataService.LoadCompanionManifestAsync(result.Source);
        var installedManifest = await _statisticsDataService.LoadInstalledUpdateManifestAsync();
        var activeRevision = manifest?.DatasetRevision ?? 0;
        var installedRevision = installedManifest?.DatasetRevision ?? 0;
        _currentDatasetManifest = installedRevision > activeRevision
            ? installedManifest
            : manifest;
        _currentDatasetRevision = Math.Max(activeRevision, installedRevision);
        ApplyDatasetInformation(result, manifest);
        if (installedRevision > activeRevision)
        {
            UpdateStatusText.Text =
                "새 통계 데이터가 설치되어 있습니다. 앱을 다시 시작하면 적용됩니다.";
            CheckUpdateButton.Content = "재시작 필요";
        }
        else if (!_updateEndpointConfigured)
        {
            UpdateStatusText.Text = "업데이트 서버가 아직 설정되지 않았습니다.";
        }
    }

    private void SettingsWindow_Closed(object? sender, EventArgs e)
    {
        _lifetimeCancellation.Cancel();
        _settingsService.SettingsChanged -= SettingsService_SettingsChanged;
        _datasetUpdateService.Dispose();
        _lifetimeCancellation.Dispose();
    }

    private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GeneralPanel is null)
        {
            return;
        }

        var panels = new[] { GeneralPanel, DisplayPanel, DataPanel, AboutPanel };
        for (var index = 0; index < panels.Length; index++)
        {
            panels[index].Visibility = NavigationList.SelectedIndex == index
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void SettingToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_applyingSettings)
        {
            return;
        }

        _settingsService.Update(settings => settings with
        {
            AlwaysOnTop = AlwaysOnTopSettingToggle.IsChecked == true,
            RememberWindowPosition = RememberPositionToggle.IsChecked == true,
            RememberWindowSize = RememberSizeToggle.IsChecked == true
        });
        _settingsService.Save();
    }

    private void ResetSettings_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "모든 앱 설정을 기본값으로 되돌릴까요?",
            "설정 초기화",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        _settingsService.ResetToDefaults();
        _settingsService.Save();
    }

    private void SettingsService_SettingsChanged(object? sender, EventArgs e) =>
        ApplySettings(_settingsService.Current);

    private void ApplySettings(AppSettings settings)
    {
        _applyingSettings = true;
        AlwaysOnTopSettingToggle.IsChecked = settings.AlwaysOnTop;
        RememberPositionToggle.IsChecked = settings.RememberWindowPosition;
        RememberSizeToggle.IsChecked = settings.RememberWindowSize;
        PortraitSizeText.Text = FormatSize(settings.PortraitWidth, settings.PortraitHeight);
        LandscapeSizeText.Text = FormatSize(settings.LandscapeWidth, settings.LandscapeHeight);
        LastLayoutText.Text = settings.LastLayoutMode == AppLayoutMode.Landscape
            ? "최근 모드: 가로형"
            : "최근 모드: 세로형";
        _applyingSettings = false;
    }

    private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_updateBusy)
        {
            return;
        }

        SetUpdateBusy(true, "업데이트 확인 중...");
        var result = await _datasetUpdateService.CheckAsync(
            _currentDatasetManifest,
            _lifetimeCancellation.Token);
        if (!IsLoaded)
        {
            return;
        }

        _availableUpdate = result.Status == DatasetUpdateStatus.UpdateAvailable
            ? result.Manifest
            : null;
        UpdateStatusText.Text = _availableUpdate is null
            ? result.Message
            : $"새 통계 데이터가 있습니다. "
                + $"{_currentAcceptedMatches:N0} → {_availableUpdate.AcceptedMatches:N0}경기";
        InstallUpdateButton.Visibility = _availableUpdate is null
            ? Visibility.Collapsed
            : Visibility.Visible;
        CheckUpdateButton.Content = result.Status == DatasetUpdateStatus.UpToDate
            ? "최신 상태"
            : "업데이트 확인";
        SetUpdateBusy(false);
    }

    private async void InstallUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_availableUpdate is null || _updateBusy)
        {
            return;
        }

        SetUpdateBusy(true, "다운로드 중...");
        var result = await _datasetUpdateService.InstallAsync(
            _availableUpdate,
            _lifetimeCancellation.Token);
        if (!IsLoaded)
        {
            return;
        }

        UpdateStatusText.Text = result.Message;
        InstallUpdateButton.Visibility = Visibility.Collapsed;
        if (result.Status == DatasetUpdateStatus.Installed)
        {
            _currentDatasetRevision = result.Manifest?.DatasetRevision
                ?? _currentDatasetRevision;
            _currentDatasetManifest = result.Manifest;
        }
        _availableUpdate = null;
        CheckUpdateButton.Content = result.Status == DatasetUpdateStatus.Installed
            ? "재시작 필요"
            : "다시 시도";
        SetUpdateBusy(false);
    }

    private void SetUpdateBusy(bool busy, string? message = null)
    {
        _updateBusy = busy;
        CheckUpdateButton.IsEnabled = !busy;
        InstallUpdateButton.IsEnabled = !busy;
        if (busy)
        {
            CheckUpdateButton.Content = "확인 중...";
        }
        if (message is not null)
        {
            UpdateStatusText.Text = message;
        }
    }

    private void ApplyDatasetInformation(
        StatisticsDatasetLoadResult result,
        DatasetUpdateManifest? manifest)
    {
        DatasetStatusText.Text = result.Status switch
        {
            StatisticsDatasetLoadStatus.Loaded => "정상",
            StatisticsDatasetLoadStatus.NotAvailable => "데이터 파일 없음",
            _ => "데이터 손상 또는 호환되지 않음"
        };

        var dataset = result.Dataset;
        _currentAcceptedMatches = dataset?.AcceptedMatches ?? 0;
        DatasetPatchText.Text = dataset?.Patch ?? "—";
        AcceptedMatchesText.Text = dataset is null
            ? "—"
            : $"{dataset.AcceptedMatches:N0}경기";
        TierScopeText.Text = dataset?.TierScope switch
        {
            "MASTER_PLUS_SEEDED" => "마스터 이상 (시드 기반)",
            "EMERALD_PLUS_SEEDED" => "에메랄드 이상 (시드 기반)",
            _ => "—"
        };
        DurationPolicyText.Text = dataset?.MinimumGameDurationSeconds switch
        {
            900 => "15분",
            > 0 => $"{dataset.MinimumGameDurationSeconds.Value / 60.0:0.#}분",
            _ => "—"
        };
        DatasetSourceText.Text = result.Source switch
        {
            StatisticsDatasetSource.Packaged => "기본 제공",
            StatisticsDatasetSource.UpdatedLocal => "업데이트됨",
            _ => "—"
        };
        DatasetRevisionText.Text = manifest is { DatasetRevision: > 0 }
            ? manifest.DatasetRevision.ToString()
            : "기본 버전";
        DatasetGeneratedAtText.Text = dataset is null
            ? "—"
            : dataset.GeneratedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    }

    private static string FormatSize(double width, double height) =>
        $"{width:0} × {height:0}";

    private static string GetApplicationVersion()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        return version is null
            ? "알 수 없음"
            : $"{version.Major}.{version.Minor}.{version.Build}";
    }
}
