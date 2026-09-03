namespace LoLMatchup.Models.Statistics;

public sealed record MatchupStatisticsResult(
    string Role,
    string QueryChampion,
    string OpponentChampion,
    int SampleSize,
    int WinsForQuery,
    int WinsForOpponent,
    double QueryWinRate,
    double OpponentWinRate,
    LevelRaceStatistics FirstLevel2,
    LevelRaceStatistics FirstLevel3,
    LevelRaceStatistics FirstLevel6,
    int SoloKillsBefore15ForQuery,
    int SoloKillsBefore15ForOpponent,
    double SoloKillsPerMatchForQuery,
    double SoloKillsPerMatchForOpponent,
    bool WasReversed);

public sealed record LevelRaceStatistics(
    int ForQuery,
    int ForOpponent,
    int Unknown,
    double QueryRate,
    double OpponentRate,
    double UnknownRate);
