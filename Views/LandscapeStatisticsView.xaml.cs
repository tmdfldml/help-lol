using System.Windows.Controls;
using System.Windows.Input;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services.Navigation;

namespace LoLMatchup.Views;

public partial class LandscapeStatisticsView : UserControl
{
    public event EventHandler<MatchupStatisticsPresentation>? RoleSelected;
    public ICommand RoleSelectionCommand { get; }

    public LandscapeStatisticsView()
    {
        RoleSelectionCommand = new DelegateCommand<MatchupStatisticsPresentation>(
            presentation => RoleSelected?.Invoke(this, presentation),
            presentation => presentation.CanOpenDetail);
        InitializeComponent();
    }
}
