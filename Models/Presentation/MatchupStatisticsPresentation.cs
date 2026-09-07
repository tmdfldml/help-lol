using System.Windows.Media;
using LoLMatchup.Models.Statistics;

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
    CompactMatchupMetricsPresentation? CompactMetrics = null,
    SampleReliabilityLevel ReliabilityLevel = SampleReliabilityLevel.Unknown,
    string ReliabilityText = "",
    string ReliabilityDescription = "",
    OutcomeAdvantageResult? OutcomeAdvantage = null,
    string OutcomeAdvantageText = "",
    string OutcomeAdvantageDescription = "",
    LevelTempoResult? LevelTempo = null,
    string LevelTempoText = "",
    string LevelTempoDescription = "",
    CombatPressureResult? CombatPressure = null,
    string CombatPressureText = "",
    string CombatPressureDescription = "")
{
    public IReadOnlyList<MatchupMetricPresentation> DisplayMetrics => Metrics ?? [];
    public bool HasStatistics => CompactMetrics is not null;
    public bool HasReliability => HasStatistics
        && ReliabilityLevel != SampleReliabilityLevel.Unknown;
    public bool HasOutcomeEvaluation => HasStatistics
        && OutcomeAdvantage is not null;
    public OutcomeAdvantageLevel OutcomeAdvantageLevel =>
        OutcomeAdvantage?.Level ?? Models.Statistics.OutcomeAdvantageLevel.InsufficientEvidence;
    public bool HasLevelTempo => HasStatistics && LevelTempo is not null;
    public LevelTempoLevel LevelTempoLevel =>
        LevelTempo?.Level ?? Models.Statistics.LevelTempoLevel.InsufficientEvidence;
    public bool HasCombatPressure => HasStatistics && CombatPressure is not null;
    public CombatPressureLevel CombatPressureLevel =>
        CombatPressure?.Level ?? Models.Statistics.CombatPressureLevel.InsufficientEvidence;
    public bool CanOpenDetail => !string.IsNullOrWhiteSpace(QueryChampionId)
        && !string.IsNullOrWhiteSpace(OpponentChampionId);
}

public sealed record MatchupMetricPresentation(
    string Label,
    string QueryValue,
    string OpponentValue,
    string DetailText = "",
    string QueryDetailText = "",
    string OpponentDetailText = "");

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
    string StatusDetailText = "",
    TeamOutcomeSummaryPresentation? OutcomeSummary = null)
{
    public IReadOnlyList<MatchupStatisticsPresentation> DisplayMatchups => Matchups ?? [];
    public bool HasOutcomeSummary => State == MatchupPresentationState.LoadedStatistics
        && OutcomeSummary is not null;
    public TeamOutcomeSummaryLevel OutcomeSummaryLevel =>
        OutcomeSummary?.Result.Level ?? TeamOutcomeSummaryLevel.InsufficientEvidence;
    public string OutcomeSummaryTitle => OutcomeSummary?.Title ?? string.Empty;
    public string OutcomeSummaryText => OutcomeSummary?.StatusText ?? string.Empty;
    public string OutcomeSummaryEvidenceText => OutcomeSummary?.EvidenceText ?? string.Empty;
    public string OutcomeSummaryDescription => OutcomeSummary?.Description ?? string.Empty;
}
