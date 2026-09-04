using System.Windows.Media;

namespace LoLMatchup.Models.Champions;

public sealed record ChampionDisplayInfo(
    string ChampionId,
    string KoreanDisplayName,
    ImageSource? IconSource);
