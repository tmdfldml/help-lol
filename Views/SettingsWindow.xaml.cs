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
    private bool _applyingSettings;
    private DatasetUpdateManifest? _availableUpdate;
    private long _currentDatasetRevision;

    public SettingsWindow(
        AppSettingsService settingsService,
        StatisticsDataService statisticsDataService)
    {
        _settingsService = settingsService;
        _statisticsDataService = statisticsDataService;
        _datasetUpdateService = new DatasetUpdateService(
            new DatasetUpdateOptions(),
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
        _currentDatasetRevision = manifest?.DatasetRevision ?? 0;
        ApplyDatasetInformation(result, manifest);
    }

    private void SettingsWindow_Closed(object? sender, EventArgs e)
    {
        _settingsService.SettingsChanged -= SettingsService_SettingsChanged;
        _datasetUpdateService.Dispose();
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
        SetUpdateBusy(true, "업데이트 확인 중...");
        var result = await _datasetUpdateService.CheckAsync(_currentDatasetRevision);
        _availableUpdate = result.Status == DatasetUpdateStatus.UpdateAvailable
            ? result.Manifest
            : null;
        UpdateStatusText.Text = result.Message;
        InstallUpdateButton.Visibility = _availableUpdate is null
            ? Visibility.Collapsed
            : Visibility.Visible;
        SetUpdateBusy(false);
    }

    private async void InstallUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_availableUpdate is null)
        {
            return;
        }

        SetUpdateBusy(true, "다운로드 중...");
        var result = await _datasetUpdateService.InstallAsync(_availableUpdate);
        UpdateStatusText.Text = result.Message;
        InstallUpdateButton.Visibility = Visibility.Collapsed;
        _availableUpdate = null;
        SetUpdateBusy(false);
    }

    private void SetUpdateBusy(bool busy, string? message = null)
    {
        CheckUpdateButton.IsEnabled = !busy;
        InstallUpdateButton.IsEnabled = !busy;
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
