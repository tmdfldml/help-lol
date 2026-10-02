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
                StatusDetailText = "앱을 켜 둔 채 League 게임을 시작하세요.\n게임이 감지되면 매치업을 자동으로 표시합니다."
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
            "게임을 기다리는 중입니다",
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
