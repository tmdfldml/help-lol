using LoLMatchup.Models.Navigation;
using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Services.Navigation;

public sealed class AppNavigationService
{
    public AppPage CurrentPage { get; private set; } = AppPage.Home;
    public MatchupDetailNavigationContext? DetailContext { get; private set; }
    public MatchupStatisticsPresentation? SkillContext { get; private set; }
    public event EventHandler? NavigationChanged;

    public void Navigate(AppPage page)
    {
        if (CurrentPage == page)
        {
            return;
        }

        CurrentPage = page;
        if (page != AppPage.ChampionSkillDetail) SkillContext = null;
        if (page != AppPage.MatchupDetail)
        {
            DetailContext = null;
        }
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NavigateToMatchupDetail(MatchupDetailNavigationContext context)
    {
        SkillContext = null;
        DetailContext = context;
        CurrentPage = AppPage.MatchupDetail;
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NavigateToChampionSkillDetail(MatchupStatisticsPresentation context)
    {
        SkillContext = context;
        DetailContext = null;
        CurrentPage = AppPage.ChampionSkillDetail;
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }
}
