using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services.Navigation;

namespace LoLMatchup.Views;

public partial class LandscapeStatisticsView : UserControl
{
    public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(
        nameof(ColumnCount), typeof(int), typeof(LandscapeStatisticsView), new PropertyMetadata(2));

    public int ColumnCount
    {
        get => (int)GetValue(ColumnCountProperty);
        private set => SetValue(ColumnCountProperty, value);
    }

    public event EventHandler<MatchupStatisticsPresentation>? RoleSelected;
    public ICommand RoleSelectionCommand { get; }

    public LandscapeStatisticsView()
    {
        RoleSelectionCommand = new DelegateCommand<MatchupStatisticsPresentation>(
            presentation => RoleSelected?.Invoke(this, presentation),
            presentation => presentation.CanOpenDetail);
        InitializeComponent();
        SizeChanged += (_, e) => ColumnCount = e.NewSize.Width < 1080 ? 2 : 3;
    }
}
