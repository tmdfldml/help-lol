using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LoLMatchup.Models.Champions;

namespace LoLMatchup.Views.Controls;

public partial class SearchableChampionPicker : UserControl
{
    private readonly ObservableCollection<ChampionDisplayInfo> _filtered = [];
    private IReadOnlyList<ChampionDisplayInfo> _champions = [];
    private bool _updatingText;

    public SearchableChampionPicker()
    {
        InitializeComponent();
        ResultsList.ItemsSource = _filtered;
    }

    public ChampionDisplayInfo? SelectedChampion { get; private set; }
    public event EventHandler? SelectionChanged;
    public string? SelectedChampionId => string.IsNullOrEmpty(SelectedChampion?.ChampionId)
        ? null
        : SelectedChampion.ChampionId;

    public void SetChampions(IReadOnlyList<ChampionDisplayInfo> champions)
    {
        _champions = champions;
        ApplyFilter(string.Empty);
        SelectChampion(null);
    }

    public void SelectChampion(string? championId)
    {
        SelectedChampion = _champions.FirstOrDefault(champion => string.Equals(
            champion.ChampionId, championId ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        _updatingText = true;
        SearchBox.Text = SelectedChampion?.KoreanDisplayName ?? string.Empty;
        SearchBox.CaretIndex = SearchBox.Text.Length;
        _updatingText = false;
        ResultsPopup.IsOpen = false;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyFilter(string searchText)
    {
        _filtered.Clear();
        var query = searchText.Trim();
        foreach (var champion in _champions.Where(champion => query.Length == 0
                     || champion.KoreanDisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || champion.ChampionId.Contains(query, StringComparison.OrdinalIgnoreCase)))
        {
            _filtered.Add(champion);
        }
        EmptyMessage.Visibility = _filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updatingText) return;
        SelectedChampion = null;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
        ApplyFilter(SearchBox.Text);
        ResultsList.SelectedIndex = _filtered.Count > 0 ? 0 : -1;
        ResultsPopup.IsOpen = true;
    }

    private void SearchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ApplyFilter(SearchBox.Text);
        ResultsPopup.IsOpen = true;
    }

    private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            ResultsPopup.IsOpen = false;
            e.Handled = true;
        }
        else if (e.Key is Key.Down or Key.Up && _filtered.Count > 0)
        {
            ResultsList.SelectedIndex = Math.Clamp(
                ResultsList.SelectedIndex + (e.Key == Key.Down ? 1 : -1), 0, _filtered.Count - 1);
            ResultsList.ScrollIntoView(ResultsList.SelectedItem);
            ResultsPopup.IsOpen = true;
            e.Handled = true;
        }
        else if (e.Key == Key.Enter && ResultsList.SelectedItem is ChampionDisplayInfo selected)
        {
            SelectChampion(selected.ChampionId);
            e.Handled = true;
        }
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ResultsPopup.IsOpen) ApplyFilter(string.Empty);
        ResultsPopup.IsOpen = !ResultsPopup.IsOpen;
        SearchBox.Focus();
    }

    private void ResultsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ResultsList.SelectedItem is ChampionDisplayInfo selected) SelectChampion(selected.ChampionId);
    }

    private void ResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed
            && ResultsList.SelectedItem is ChampionDisplayInfo selected)
            SelectChampion(selected.ChampionId);
    }
}
