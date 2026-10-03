using System.Windows.Controls;
using System.Windows.Input;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services.Navigation;

namespace LoLMatchup.Views;

public partial class PortraitStatisticsView : UserControl
{
    public event EventHandler<MatchupStatisticsPresentation>? RoleSelected;
    public ICommand RoleSelectionCommand { get; }
    public event EventHandler<MatchupStatisticsPresentation>? EnemySelected;
    public ICommand EnemySelectionCommand { get; }

    public PortraitStatisticsView()
    {
        EnemySelectionCommand = new DelegateCommand<MatchupStatisticsPresentation>(
            presentation => EnemySelected?.Invoke(this, presentation),
            presentation => !string.IsNullOrWhiteSpace(presentation.OpponentChampionId));
        RoleSelectionCommand = new DelegateCommand<MatchupStatisticsPresentation>(
            presentation => RoleSelected?.Invoke(this, presentation),
            presentation => presentation.CanOpenDetail);
        InitializeComponent();
    }
}
