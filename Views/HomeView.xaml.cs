using System.Windows;
using System.Windows.Controls;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Views;

public partial class HomeView : UserControl
{
    public HomeView() => InitializeComponent();

    public event EventHandler? InGameRequested;

    public void SetGameActive(bool active)
    {
        GameStatusText.Text = active ? "게임 감지됨" : "게임을 기다리는 중";
        GameStatusDetailText.Text = active
            ? "현재 게임의 5개 포지션 매치업을 확인할 수 있습니다."
            : "League of Legends 게임이 시작되면 자동으로 매치업을 감지합니다.";
    }

    public void SetDataset(StatisticsDataset? dataset, string unavailableText)
    {
        DatasetPatchText.Text = dataset is null ? unavailableText : $"패치 {dataset.Patch}";
        DatasetMatchesText.Text = dataset is null ? string.Empty : $"{dataset.AcceptedMatches:N0}경기 적용";
    }

    private void OpenInGame_Click(object sender, RoutedEventArgs e) =>
        InGameRequested?.Invoke(this, EventArgs.Empty);
}
