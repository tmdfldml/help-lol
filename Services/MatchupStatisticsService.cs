using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services;

public sealed class MatchupStatisticsService
{
    private readonly StatisticsDataService _dataService;
    private readonly MatchupStatisticsCalculator _calculator;

    public MatchupStatisticsService(
        StatisticsDataService? dataService = null,
        MatchupStatisticsCalculator? calculator = null)
    {
        _dataService = dataService ?? new StatisticsDataService();
        _calculator = calculator ?? new MatchupStatisticsCalculator();
    }

    public async Task<MatchupStatisticsQueryResult> GetStatisticsAsync(
        string role,
        string queryChampion,
        string opponentChampion)
    {
        if (string.IsNullOrWhiteSpace(role)
            || string.IsNullOrWhiteSpace(queryChampion)
            || string.IsNullOrWhiteSpace(opponentChampion)
            || !PositionNormalizer.StandardPositions.Contains(
                role.Trim().ToUpperInvariant(),
                StringComparer.Ordinal))
        {
            return MatchupStatisticsQueryResult.InvalidQuery;
        }

        var loadResult = await _dataService.LoadAsync();

        if (loadResult.Status == StatisticsDatasetLoadStatus.NotAvailable)
        {
            return MatchupStatisticsQueryResult.DatasetNotAvailable;
        }

        if (loadResult.Status == StatisticsDatasetLoadStatus.Invalid)
        {
            return MatchupStatisticsQueryResult.DatasetInvalid;
        }

        var lookup = await _dataService.FindMatchupAsync(
            role,
            queryChampion,
            opponentChampion);

        if (lookup is null)
        {
            return MatchupStatisticsQueryResult.NotFound;
        }

        return _calculator.TryCalculate(
            lookup,
            queryChampion,
            opponentChampion,
            out var statistics)
            ? new MatchupStatisticsQueryResult(
                MatchupStatisticsQueryStatus.Success,
                statistics)
            : MatchupStatisticsQueryResult.InvalidAggregate;
    }
}

public enum MatchupStatisticsQueryStatus
{
    Success,
    NotFound,
    DatasetNotAvailable,
    DatasetInvalid,
    InvalidQuery,
    InvalidAggregate
}

public sealed record MatchupStatisticsQueryResult(
    MatchupStatisticsQueryStatus Status,
    MatchupStatisticsResult? Statistics = null)
{
    public static MatchupStatisticsQueryResult NotFound { get; } =
        new(MatchupStatisticsQueryStatus.NotFound);

    public static MatchupStatisticsQueryResult DatasetNotAvailable { get; } =
        new(MatchupStatisticsQueryStatus.DatasetNotAvailable);

    public static MatchupStatisticsQueryResult DatasetInvalid { get; } =
        new(MatchupStatisticsQueryStatus.DatasetInvalid);

    public static MatchupStatisticsQueryResult InvalidQuery { get; } =
        new(MatchupStatisticsQueryStatus.InvalidQuery);

    public static MatchupStatisticsQueryResult InvalidAggregate { get; } =
        new(MatchupStatisticsQueryStatus.InvalidAggregate);
}
