using System.Windows.Media;

namespace LoLMatchup.Models.Presentation;

public enum MatchupPresentationState
{
    Waiting,
    LoadedStatistics,
    MatchupNotFound,
    DatasetNotAvailable,
    DatasetInvalid
}

public sealed record MatchupStatisticsPresentation(
    MatchupPresentationState State,
    string StatusText,
    string Role = "",
    string QueryChampionId = "",
    string QueryChampionDisplayName = "",
    ImageSource? QueryChampionIcon = null,
    string OpponentChampionId = "",
    string OpponentChampionDisplayName = "",
    ImageSource? OpponentChampionIcon = null,
    string SampleSizeText = "",
    string PatchDisplayText = "",
    IReadOnlyList<MatchupMetricPresentation>? Metrics = null,
    string StatusDetailText = "",
    CompactMatchupMetricsPresentation? CompactMetrics = null)
{
    public IReadOnlyList<MatchupMetricPresentation> DisplayMetrics => Metrics ?? [];
    public bool HasStatistics => CompactMetrics is not null;
}

public sealed record MatchupMetricPresentation(
    string Label,
    string QueryValue,
    string OpponentValue,
    string DetailText = "");

public sealed record CompactMatchupMetricsPresentation(
    string QueryWinRate,
    string OpponentWinRate,
    string QueryLevel2,
    string OpponentLevel2,
    string QueryLevel3,
    string OpponentLevel3,
    string QueryLevel6,
    string OpponentLevel6,
    string QuerySoloKills,
    string OpponentSoloKills,
    string UnknownText);

public sealed record TeamMatchupStatisticsPresentation(
    MatchupPresentationState State,
    string StatusText,
    string PatchDisplayText = "",
    IReadOnlyList<MatchupStatisticsPresentation>? Matchups = null,
    string StatusDetailText = "")
{
    public IReadOnlyList<MatchupStatisticsPresentation> DisplayMatchups => Matchups ?? [];
}
