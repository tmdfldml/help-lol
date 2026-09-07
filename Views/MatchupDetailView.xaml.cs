using System.Windows;
using System.Windows.Controls;
using LoLMatchup.Models.Navigation;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Views;

public partial class MatchupDetailView : UserControl
{
    public MatchupDetailView() => InitializeComponent();

    public event EventHandler? BackRequested;

    public void SetContext(MatchupDetailNavigationContext context)
    {
        var presentation = context.Presentation;
        DataContext = presentation;
        PageTitleText.Text = $"{context.Role} 매치업";
        AllyIcon.Source = presentation.QueryChampionIcon;
        EnemyIcon.Source = presentation.OpponentChampionIcon;
        AllyNameText.Text = presentation.QueryChampionDisplayName;
        EnemyNameText.Text = presentation.OpponentChampionDisplayName;
        AllyIdText.Text = context.AllyChampionId;
        EnemyIdText.Text = context.EnemyChampionId;
        SampleSizeText.Text = presentation.SampleSizeText;
        PatchText.Text = context.PatchDisplayText;
        MetricsItems.ItemsSource = presentation.DisplayMetrics;
        ReliabilityText.Text = presentation.ReliabilityText;
        ReliabilityDescriptionText.Text = presentation.ReliabilityDescription;
        ReliabilityPanel.Visibility = presentation.HasReliability
            ? Visibility.Visible
            : Visibility.Collapsed;
        OutcomeText.Text = presentation.OutcomeAdvantageText;
        OutcomeDescriptionText.Text = presentation.OutcomeAdvantageDescription;
        OutcomePanel.Visibility = presentation.HasOutcomeEvaluation
            ? Visibility.Visible
            : Visibility.Collapsed;
        LevelTempoText.Text = presentation.LevelTempoText;
        LevelTempoDescriptionText.Text = presentation.LevelTempoDescription;
        LevelTempoPanel.Visibility = presentation.HasLevelTempo
            ? Visibility.Visible
            : Visibility.Collapsed;
        CombatPressureText.Text = presentation.CombatPressureText;
        CombatPressureDescriptionText.Text = presentation.CombatPressureDescription;
        CombatPressurePanel.Visibility = presentation.HasCombatPressure
            ? Visibility.Visible
            : Visibility.Collapsed;

        var hasStatistics = presentation.HasStatistics;
        MetricsItems.Visibility = hasStatistics ? Visibility.Visible : Visibility.Collapsed;
        SoloKillExplanation.Visibility = hasStatistics ? Visibility.Visible : Visibility.Collapsed;
        DetailStatusPanel.Visibility = hasStatistics ? Visibility.Collapsed : Visibility.Visible;
        DetailStatusText.Text = presentation.State == MatchupPresentationState.MatchupNotFound
            ? "해당 매치업의 통계가 아직 없습니다."
            : presentation.StatusText;
        DetailStatusDescription.Text = hasStatistics
            ? string.Empty
            : "챔피언 정보는 감지되었지만 표시할 통계 데이터가 없습니다.";
    }

    private void Back_Click(object sender, RoutedEventArgs e) =>
        BackRequested?.Invoke(this, EventArgs.Empty);
}
