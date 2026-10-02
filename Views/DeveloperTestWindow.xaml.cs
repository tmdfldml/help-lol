using System.Windows;
using LoLMatchup.Models.Champions;
using LoLMatchup.Models.Development;
using LoLMatchup.Services;
using LoLMatchup.Services.Development;
using LoLMatchup.Views.Controls;

namespace LoLMatchup.Views;

public partial class DeveloperTestWindow : Window
{
    private readonly DeveloperTestModeService _testMode;
    private readonly ChampionMetadataService _metadataService;

    public DeveloperTestWindow(
        DeveloperTestModeService testMode,
        ChampionMetadataService metadataService)
    {
        InitializeComponent();
        _testMode = testMode;
        _metadataService = metadataService;
        Loaded += DeveloperTestWindow_Loaded;
    }

    private async void DeveloperTestWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var champions = new List<ChampionDisplayInfo>
        {
            new(string.Empty, "없음", null)
        };
        champions.AddRange(await _metadataService.GetAllChampionsAsync());

        foreach (var picker in ChampionSelectors())
        {
            picker.SetChampions(champions);
        }
        UpdateCustomButtonState();
    }

    private void Normal5v5_Click(object sender, RoutedEventArgs e) =>
        StartPreset(DeveloperTestModeService.CreateNormal5v5());

    private void Top1v1_Click(object sender, RoutedEventArgs e) =>
        StartPreset(DeveloperTestModeService.CreateTop1v1());

    private void Partial_Click(object sender, RoutedEventArgs e) =>
        StartPreset(DeveloperTestModeService.CreatePartialRoles());

    private void NotFound_Click(object sender, RoutedEventArgs e) =>
        StartPreset(DeveloperTestModeService.CreateNotFound());

    private void Stop_Click(object sender, RoutedEventArgs e) => _testMode.Stop();

    private void PatchMismatch_Click(object sender, RoutedEventArgs e)
    {
        var diagnostics = ((App)Application.Current).Diagnostics;
        var patch = diagnostics.Snapshot.DatasetPatch?.Split('.');
        if (patch is { Length: 2 } && int.TryParse(patch[1], out var minor) && minor < int.MaxValue)
            diagnostics.SetCurrentGameVersion($"{patch[0]}.{minor + 1}.0");
    }

    private void PatchReset_Click(object sender, RoutedEventArgs e) =>
        ((App)Application.Current).Diagnostics.SetCurrentGameVersion(null);

    private void Viewport_Click(object sender, RoutedEventArgs e)
    {
        if (Owner is not { } owner || sender is not System.Windows.Controls.Button { Tag: string size }) return;
        var parts = size.Split(',');
        var width = int.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
        var height = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
        owner.WindowState = WindowState.Normal;
        owner.MinWidth = width > height ? 760 : 420;
        owner.MinHeight = width > height ? 420 : 760;
        owner.Width = width;
        owner.Height = height;
        Close();
    }

    private void UpdateFailure_Click(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow main) main.ShowUpdateFailureFixture();
        Close();
    }
    private void CrashUi_Click(object sender, RoutedEventArgs e) =>
        throw new InvalidOperationException("Controlled diagnostic fixture.", new ArgumentException("Inner fixture."));

    private void CrashTask_Click(object sender, RoutedEventArgs e)
    {
        CreateUnobservedTask();
        GC.Collect();
        // No WaitForPendingFinalizers: the fatal handler may show a dialog on that thread.
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void CreateUnobservedTask() =>
        _ = Task.FromException(new InvalidOperationException("Controlled task fixture."));

    private void StartCustom_Click(object sender, RoutedEventArgs e)
    {
        if (!ChampionSelectors().Any(picker => picker.SelectedChampionId is not null))
        {
            return;
        }

        _testMode.StartScenario(DeveloperTestModeService.CreateCustom(
            [Selected(AllyTop), Selected(AllyJungle), Selected(AllyMid), Selected(AllyAdc), Selected(AllySupport)],
            [Selected(EnemyTop), Selected(EnemyJungle), Selected(EnemyMid), Selected(EnemyAdc), Selected(EnemySupport)]));
    }

    private void StartPreset(DeveloperTestScenario scenario)
    {
        foreach (var (picker, role, team) in PickerRoles())
        {
            var championId = scenario.Players.FirstOrDefault(player =>
                player.NormalizedPosition == role && player.Team == team)?.ChampionId;
            picker.SelectChampion(championId);
        }
        _testMode.StartScenario(scenario);
        UpdateCustomButtonState();
    }

    private void Picker_SelectionChanged(object? sender, EventArgs e) =>
        UpdateCustomButtonState();

    private void UpdateCustomButtonState() => StartCustomButton.IsEnabled =
        ChampionSelectors().Any(picker => picker.SelectedChampionId is not null);

    private IEnumerable<SearchableChampionPicker> ChampionSelectors() =>
        [AllyTop, EnemyTop, AllyJungle, EnemyJungle, AllyMid, EnemyMid,
            AllyAdc, EnemyAdc, AllySupport, EnemySupport];

    private IEnumerable<(SearchableChampionPicker Picker, string Role, string Team)> PickerRoles() =>
    [
        (AllyTop, "TOP", "ORDER"), (EnemyTop, "TOP", "CHAOS"),
        (AllyJungle, "JUNGLE", "ORDER"), (EnemyJungle, "JUNGLE", "CHAOS"),
        (AllyMid, "MID", "ORDER"), (EnemyMid, "MID", "CHAOS"),
        (AllyAdc, "ADC", "ORDER"), (EnemyAdc, "ADC", "CHAOS"),
        (AllySupport, "SUPPORT", "ORDER"), (EnemySupport, "SUPPORT", "CHAOS")
    ];

    private static string? Selected(SearchableChampionPicker picker) => picker.SelectedChampionId;
}
