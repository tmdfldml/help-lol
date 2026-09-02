using StatsCollector.Api;

namespace StatsCollector.Collection;

public sealed class SeedProvider
{
    private static readonly LeagueTier[] MasterPlusTiers =
        [LeagueTier.Challenger, LeagueTier.Grandmaster, LeagueTier.Master];

    private readonly RiotApiClient _riotApiClient;

    public SeedProvider(RiotApiClient riotApiClient)
    {
        _riotApiClient = riotApiClient;
    }

    public async Task<MasterPlusSeedResult> CollectMasterPlusAsync(
        CancellationToken cancellationToken = default)
    {
        var puuids = new HashSet<string>(StringComparer.Ordinal);
        var tierResults = new List<TierSeedResult>();
        var convertedSummonerIds = new Dictionary<string, string?>(
            StringComparer.Ordinal);
        var conversionCount = 0;
        var conversionFailureCount = 0;

        foreach (var tier in MasterPlusTiers)
        {
            var leagueResult = await _riotApiClient.GetLeagueAsync(
                tier,
                cancellationToken);

            if (leagueResult.Status != RiotApiRequestStatus.Success
                || leagueResult.Data is null)
            {
                tierResults.Add(new TierSeedResult(
                    tier,
                    0,
                    leagueResult.Status,
                    leagueResult.RetryAfter));

                if (leagueResult.Status is RiotApiRequestStatus.AuthenticationFailed
                    or RiotApiRequestStatus.RateLimited
                    or RiotApiRequestStatus.Cancelled)
                {
                    break;
                }

                continue;
            }

            var entries = leagueResult.Data.Entries;
            tierResults.Add(new TierSeedResult(
                tier,
                entries.Count,
                RiotApiRequestStatus.Success));

            foreach (var entry in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Puuid))
                {
                    puuids.Add(entry.Puuid);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(entry.SummonerId))
                {
                    conversionFailureCount++;
                    continue;
                }

                if (!convertedSummonerIds.TryGetValue(
                        entry.SummonerId,
                        out var convertedPuuid))
                {
                    conversionCount++;
                    var summonerResult = await _riotApiClient.GetSummonerByIdAsync(
                        entry.SummonerId,
                        cancellationToken);
                    convertedPuuid = summonerResult.Status == RiotApiRequestStatus.Success
                        ? summonerResult.Data?.Puuid
                        : null;
                    convertedSummonerIds[entry.SummonerId] = convertedPuuid;

                    if (string.IsNullOrWhiteSpace(convertedPuuid))
                    {
                        conversionFailureCount++;
                    }
                }

                if (!string.IsNullOrWhiteSpace(convertedPuuid))
                {
                    puuids.Add(convertedPuuid);
                }
            }
        }

        return new MasterPlusSeedResult(
            puuids,
            tierResults,
            conversionCount,
            conversionFailureCount);
    }
}

public sealed record MasterPlusSeedResult(
    IReadOnlySet<string> Puuids,
    IReadOnlyList<TierSeedResult> TierResults,
    int ConversionCount,
    int ConversionFailureCount);

public sealed record TierSeedResult(
    LeagueTier Tier,
    int EntryCount,
    RiotApiRequestStatus Status,
    TimeSpan? RetryAfter = null);
