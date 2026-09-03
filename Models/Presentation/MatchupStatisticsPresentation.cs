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
    string QueryChampion = "",
    string OpponentChampion = "",
    string SampleSizeText = "",
    string MetadataText = "",
    IReadOnlyList<MatchupMetricPresentation>? Metrics = null)
{
    public IReadOnlyList<MatchupMetricPresentation> DisplayMetrics => Metrics ?? [];
}

public sealed record MatchupMetricPresentation(
    string Label,
    string QueryValue,
    string OpponentValue,
    string DetailText = "");
