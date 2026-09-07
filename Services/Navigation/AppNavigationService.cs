using LoLMatchup.Models.Navigation;

namespace LoLMatchup.Services.Navigation;

public sealed class AppNavigationService
{
    public AppPage CurrentPage { get; private set; } = AppPage.Home;
    public MatchupDetailNavigationContext? DetailContext { get; private set; }
    public event EventHandler? NavigationChanged;

    public void Navigate(AppPage page)
    {
        if (CurrentPage == page)
        {
            return;
        }

        CurrentPage = page;
        if (page != AppPage.MatchupDetail)
        {
            DetailContext = null;
        }
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NavigateToMatchupDetail(MatchupDetailNavigationContext context)
    {
        DetailContext = context;
        CurrentPage = AppPage.MatchupDetail;
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }
}
