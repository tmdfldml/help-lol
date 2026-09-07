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
