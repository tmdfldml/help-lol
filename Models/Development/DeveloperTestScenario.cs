using LoLMatchup.Services;

namespace LoLMatchup.Models.Development;

public sealed record DeveloperTestScenario(
    string Name,
    string ActivePlayerRiotId,
    IReadOnlyList<LiveClientPlayer> Players);

public sealed record DeveloperChampionOption(
    string? ChampionId,
    string DisplayName);
