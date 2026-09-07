using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Models.Presentation;

public sealed record TeamOutcomeSummaryPresentation(
    TeamOutcomeSummaryResult Result,
    string Title,
    string StatusText,
    string EvidenceText,
    string Description);
