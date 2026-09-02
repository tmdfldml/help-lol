using StatsCollector.Analysis;

namespace StatsCollector.Aggregation;

public sealed class MatchupAccumulator
{
    private readonly Dictionary<MatchupKey, MatchupAggregate> _aggregates = [];

    public IReadOnlyDictionary<MatchupKey, MatchupAggregate> Aggregates =>
        _aggregates;

    public void Add(MatchupObservation observation)
    {
        var key = new MatchupKey(
            observation.Role,
            observation.ChampionA,
            observation.ChampionB);

        if (!_aggregates.TryGetValue(key, out var aggregate))
        {
            aggregate = new MatchupAggregate(key);
            _aggregates.Add(key, aggregate);
        }

        aggregate.Add(observation);
    }

    public void AddRange(IEnumerable<MatchupObservation> observations)
    {
        foreach (var observation in observations)
        {
            Add(observation);
        }
    }

    public bool InvariantsValid =>
        _aggregates.Values.All(aggregate => aggregate.InvariantsValid);
}

public readonly record struct MatchupKey(
    StandardRole Role,
    string ChampionA,
    string ChampionB);

public sealed class MatchupAggregate
{
    private readonly MilestoneCounts _level2 = new();
    private readonly MilestoneCounts _level3 = new();
    private readonly MilestoneCounts _level6 = new();

    public MatchupAggregate(MatchupKey key)
    {
        Key = key;
    }

    public MatchupKey Key { get; }

    public int SampleSize { get; private set; }

    public int WinsForA { get; private set; }

    public int FirstLevel2ForA => _level2.ForA;

    public int FirstLevel2ForB => _level2.ForB;

    public int FirstLevel2Unknown => _level2.Unknown;

    public int FirstLevel3ForA => _level3.ForA;

    public int FirstLevel3ForB => _level3.ForB;

    public int FirstLevel3Unknown => _level3.Unknown;

    public int FirstLevel6ForA => _level6.ForA;

    public int FirstLevel6ForB => _level6.ForB;

    public int FirstLevel6Unknown => _level6.Unknown;

    public int SoloKillsBefore15ForA { get; private set; }

    public int SoloKillsBefore15ForB { get; private set; }

    public bool InvariantsValid =>
        _level2.Total == SampleSize
        && _level3.Total == SampleSize
        && _level6.Total == SampleSize;

    internal void Add(MatchupObservation observation)
    {
        SampleSize++;

        if (observation.ChampionAWon)
        {
            WinsForA++;
        }

        _level2.Increment(observation.FirstLevel2Result);
        _level3.Increment(observation.FirstLevel3Result);
        _level6.Increment(observation.FirstLevel6Result);
        SoloKillsBefore15ForA += observation.SoloKillsBefore15ForA;
        SoloKillsBefore15ForB += observation.SoloKillsBefore15ForB;
    }
}

internal sealed class MilestoneCounts
{
    public int ForA { get; private set; }

    public int ForB { get; private set; }

    public int Unknown { get; private set; }

    public int Total => ForA + ForB + Unknown;

    public void Increment(MilestoneResult result)
    {
        switch (result)
        {
            case MilestoneResult.A:
                ForA++;
                break;
            case MilestoneResult.B:
                ForB++;
                break;
            default:
                Unknown++;
                break;
        }
    }
}
