using System.Windows;
using System.Windows.Controls;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Views;

public partial class InGameView : UserControl
{
    private string _patchDisplayText = string.Empty;

    public InGameView()
    {
        InitializeComponent();
        PortraitView.RoleSelected += StatisticsView_RoleSelected;
        LandscapeView.RoleSelected += StatisticsView_RoleSelected;
        ShowWaiting();
    }

    public event EventHandler<MatchupStatisticsPresentation>? RoleSelected;

    public void SetLandscape(bool isLandscape)
    {
        PortraitView.Visibility = isLandscape ? Visibility.Collapsed : Visibility.Visible;
        LandscapeView.Visibility = isLandscape ? Visibility.Visible : Visibility.Collapsed;
    }

    public void SetPresentation(TeamMatchupStatisticsPresentation presentation)
    {
        _patchDisplayText = presentation.PatchDisplayText;
        if (presentation.State == MatchupPresentationState.Waiting)
        {
            presentation = presentation with
            {
                StatusDetailText = "현재 인게임 데이터가 없습니다.\n게임이 시작되면 이 화면에 매치업 통계가 표시됩니다."
            };
        }

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

    private void ShowWaiting()
    {
        SetPresentation(new TeamMatchupStatisticsPresentation(
            MatchupPresentationState.Waiting,
            "게임을 기다리는 중...",
            StatusDetailText: "리그 오브 레전드 게임이 시작되면\n매치업 통계를 자동으로 표시합니다."));
    }

    private void StatisticsView_RoleSelected(
        object? sender,
        MatchupStatisticsPresentation presentation) =>
        RoleSelected?.Invoke(this, presentation with
        {
            PatchDisplayText = string.IsNullOrWhiteSpace(presentation.PatchDisplayText)
                ? _patchDisplayText
                : presentation.PatchDisplayText
        });
}
