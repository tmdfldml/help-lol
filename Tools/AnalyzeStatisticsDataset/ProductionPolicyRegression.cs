using System.Text.Json;
using System.Text.Json.Nodes;
using ProductionWilsonInterval = LoLMatchup.Models.Statistics.WilsonInterval;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace AnalyzeStatisticsDataset;

// Fixtures assert policy and structure, never an empirical champion's current result.
internal static class ProductionPolicyRegression
{
    public static void Verify()
    {
        Reliability();
        Outcome();
        Tempo();
        Combat();
        TeamSummary();
        Structure();
        Console.WriteLine("policyRegression=True (synthetic boundaries, symmetry, team summary, invalid data)");
    }

    private static void Require(bool valid, string name)
    {
        if (!valid) throw new InvalidDataException($"Policy regression failed: {name}.");
    }

    private static void Reliability()
    {
        var cases = new[] { (1, SampleReliabilityLevel.Poor), (4, SampleReliabilityLevel.Poor),
            (5, SampleReliabilityLevel.Reference), (19, SampleReliabilityLevel.Reference),
            (20, SampleReliabilityLevel.Moderate), (49, SampleReliabilityLevel.Moderate),
            (50, SampleReliabilityLevel.RelativelyStrong) };
        foreach (var (n, expected) in cases)
            Require(SampleReliabilityClassifier.Classify(n) == expected, $"Reliability n={n}");
    }

    private static void Outcome()
    {
        var cases = new[] { (0, 0, OutcomeAdvantageLevel.InsufficientEvidence),
            (1, 1, OutcomeAdvantageLevel.InsufficientEvidence),
            (19, 19, OutcomeAdvantageLevel.InsufficientEvidence),
            (10, 20, OutcomeAdvantageLevel.Neutral),
            (24, 40, OutcomeAdvantageLevel.Neutral),
            (25, 40, OutcomeAdvantageLevel.AllyAdvantage),
            (549, 1000, OutcomeAdvantageLevel.Neutral),
            (550, 1000, OutcomeAdvantageLevel.AllyAdvantage),
            (450, 1000, OutcomeAdvantageLevel.EnemyAdvantage),
            (451, 1000, OutcomeAdvantageLevel.Neutral),
            (8, 22, OutcomeAdvantageLevel.Neutral) };
        foreach (var (wins, n, expected) in cases)
        {
            Require(OutcomeAdvantageClassifier.Classify(wins, n).Level == expected,
                $"Outcome {wins}/{n}");
            var reverse = expected switch
            {
                OutcomeAdvantageLevel.AllyAdvantage => OutcomeAdvantageLevel.EnemyAdvantage,
                OutcomeAdvantageLevel.EnemyAdvantage => OutcomeAdvantageLevel.AllyAdvantage,
                _ => expected
            };
            Require(OutcomeAdvantageClassifier.Classify(n - wins, n).Level == reverse,
                $"Outcome reverse {wins}/{n}");
        }
        Require(OutcomeAdvantageClassifier.ClassifyEligible(550, 1000,
            new ProductionWilsonInterval(.5, .6)) == OutcomeAdvantageLevel.Neutral, "Wilson lower equality");
        Require(OutcomeAdvantageClassifier.ClassifyEligible(450, 1000,
            new ProductionWilsonInterval(.4, .5)) == OutcomeAdvantageLevel.Neutral, "Wilson upper equality");
    }

    private static void Tempo()
    {
        // Difference is (A-B)/1000: 49,50,51 are exactly the requested 4.9/5.0/5.1pp inputs.
        foreach (var difference in new[] { 49, 50, 51 })
        foreach (var sign in new[] { -1, 1 })
        {
            var expectedSignal = difference < 50 ? LevelTempoSignal.Neutral
                : sign > 0 ? LevelTempoSignal.Ally : LevelTempoSignal.Enemy;
            var result = Level(sign * difference, sign * difference, sign * difference);
            Require(new[] { result.Level2Signal, result.Level3Signal, result.Level6Signal }
                .All(value => value == expectedSignal), $"Tempo {sign * difference}/1000");
            var expected = difference < 50 ? LevelTempoLevel.Neutral
                : sign > 0 ? LevelTempoLevel.AllyTendency : LevelTempoLevel.EnemyTendency;
            Require(result.Level == expected, "Tempo boundary classification");
        }
        foreach (var sign in new[] { -1, 1 })
        for (var count = 0; count <= 3; count++)
        {
            var result = Level(count > 0 ? sign * 50 : 0,
                count > 1 ? sign * 50 : 0, count > 2 ? sign * 50 : 0);
            var expected = count < 2 ? LevelTempoLevel.Neutral
                : sign > 0 ? LevelTempoLevel.AllyTendency : LevelTempoLevel.EnemyTendency;
            Require(result.Level == expected, $"Tempo {count} qualifying signals, sign {sign}");
        }
        Require(Level(50, -50, 50).Level == LevelTempoLevel.AllyTendency, "Tempo mixed A/B/A");
        Require(Level(-50, 50, -50).Level == LevelTempoLevel.EnemyTendency, "Tempo mixed reverse");
        Require(LevelTempoClassifier.Classify(19, 19, 0, 0, 19, 0, 0, 19, 0, 0).Level
            == LevelTempoLevel.InsufficientEvidence, "Tempo n19");
    }

    private static LevelTempoResult Level(int l2, int l3, int l6) =>
        LevelTempoClassifier.Classify(1000,
            400 + l2, 400, 200 - l2,
            400 + l3, 400, 200 - l3,
            400 + l6, 400, 200 - l6);

    private static void Combat()
    {
        foreach (var difference in new[] { -11, -10, -9, 9, 10, 11 })
        {
            var result = CombatPressureClassifier.Classify(100, 100 + difference, 100);
            var expected = Math.Abs(difference) < 10 ? CombatPressureLevel.Neutral
                : difference > 0 ? CombatPressureLevel.AllyTendency : CombatPressureLevel.EnemyTendency;
            Require(result.Level == expected, $"Combat {difference}/100 inclusive threshold");
        }
        Require(CombatPressureClassifier.Classify(19, 40, 0).Level
            == CombatPressureLevel.InsufficientEvidence, "Combat n19");
        Require(CombatPressureClassifier.Classify(20, 25, 25).Level
            == CombatPressureLevel.Neutral, "Solo event count above sample size");
    }

    private static void TeamSummary()
    {
        const OutcomeAdvantageLevel a = OutcomeAdvantageLevel.AllyAdvantage;
        const OutcomeAdvantageLevel b = OutcomeAdvantageLevel.EnemyAdvantage;
        const OutcomeAdvantageLevel n = OutcomeAdvantageLevel.Neutral;
        const OutcomeAdvantageLevel i = OutcomeAdvantageLevel.InsufficientEvidence;
        Check([a, a, i, null, null], TeamOutcomeSummaryLevel.InsufficientEvidence);
        Check([a, a, b, n, null], TeamOutcomeSummaryLevel.AllyTendency);
        Check([b, b, a, n, null], TeamOutcomeSummaryLevel.EnemyTendency);
        Check([a, a, b, b, n], TeamOutcomeSummaryLevel.Neutral);
        Check([n, n, n, null, null], TeamOutcomeSummaryLevel.Neutral);

        static void Check(OutcomeAdvantageLevel?[] levels, TeamOutcomeSummaryLevel expected)
        {
            var result = TeamOutcomeSummaryClassifier.Classify(levels
                .Select(level => new TeamOutcomeRoleEvidence(level.HasValue, level)).ToArray());
            Require(result.Level == expected, $"Team summary {expected}");
            Require(result.EligibleRoleCount == result.AllyDirectionalCount
                + result.EnemyDirectionalCount + result.NeutralCount, "Team summary counts");
        }
    }

    private static void Structure()
    {
        var valid = new StatisticsDataset
        {
            SchemaVersion = 1, Source = "SELF_RIOT", TierScope = "MASTER_PLUS_SEEDED",
            TierScopeMethod = "SEED_PLAYER_TIER_AT_COLLECTION", Region = "KR", QueueId = 420,
            Patch = "1.1", MinimumGameDurationSeconds = 900, AcceptedMatches = 1, Observations = 5,
            Matchups = new[] { "TOP", "JUNGLE", "MID", "ADC", "SUPPORT" }.Select(role =>
                new StatisticsMatchupEntry
                {
                    Role = role, ChampionA = "FixtureA", ChampionB = "FixtureB", SampleSize = 1,
                    WinsForA = 1, FirstLevel2ForA = 1, FirstLevel3ForA = 1, FirstLevel6ForA = 1,
                    SoloKillsBefore15ForA = 3
                }).ToList()
        };
        Require(StatisticsDatasetValidator.TryValidate(valid, out _, out _), "Valid synthetic dataset");
        Reject("canonical reversed", node =>
        {
            node["matchups"]![0]!["championA"] = "FixtureB";
            node["matchups"]![0]!["championB"] = "FixtureA";
        });
        Reject("duplicate", node => node["matchups"]!.AsArray().Add(node["matchups"]![0]!.DeepClone()));
        Reject("level invariant", node => node["matchups"]![0]!["firstLevel2Unknown"] = 1);
        Reject("negative count", node => node["matchups"]![0]!["winsForA"] = -1);
        Reject("malformed statistics", node => node["matchups"]![0]!["sampleSize"] = "invalid");
        Require(StatisticsDatasetValidator.TryCreateKey("TOP", "FixtureA", "FixtureB", out var forward, out var f)
            && StatisticsDatasetValidator.TryCreateKey("top", "fixtureb", "fixturea", out var reverse, out var r)
            && !f && r && StatisticsMatchupKeyComparer.Instance.Equals(forward, reverse), "Canonical reverse key");

        void Reject(string name, Action<JsonNode> mutate)
        {
            var node = JsonSerializer.SerializeToNode(valid)!;
            mutate(node);
            try
            {
                Require(!StatisticsDatasetValidator.TryValidate(node.Deserialize<StatisticsDataset>(),
                    out _, out _), $"Invalid dataset rejected: {name}");
            }
            catch (JsonException) when (name == "malformed statistics") { }
        }
    }
}
