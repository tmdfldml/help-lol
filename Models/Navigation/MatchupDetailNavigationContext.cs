using LoLMatchup.Models.Presentation;

namespace LoLMatchup.Models.Navigation;

public sealed record MatchupDetailNavigationContext(
    string Role,
    string AllyChampionId,
    string EnemyChampionId,
    string PatchDisplayText,
    MatchupStatisticsPresentation Presentation);
