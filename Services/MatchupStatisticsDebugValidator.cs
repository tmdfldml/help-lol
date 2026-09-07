#if DEBUG
using System.Diagnostics;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Development;
using LoLMatchup.Services.Statistics;

namespace LoLMatchup.Services;

internal static class MatchupStatisticsDebugValidator
{
    private const double Tolerance = 1e-12;

    public static async Task ValidateAsync(
        ChampionMetadataService championMetadataService)
    {
        ValidateSampleReliabilityBoundaries();
        ValidateOutcomeAdvantageBoundaries();
        ValidateLaneIndicatorBoundaries();
        ValidateMatchupExplanations();
        ValidateTeamOutcomeSummary();
        ValidateDeveloperTestPresets();
        await ValidateChampionMetadataAsync(championMetadataService);
        await ValidateDeveloperPipelineAsync(championMetadataService);
        var dataService = new StatisticsDataService();
        var loadResult = await dataService.LoadAsync();

        if (loadResult.Status != StatisticsDatasetLoadStatus.Loaded
            || loadResult.Dataset is null)
        {
            Debug.WriteLine($"Statistics dataset load failed: {loadResult.Status}");
            return;
        }

        var dataset = loadResult.Dataset;
        Debug.WriteLine(
            $"Statistics dataset loaded: {dataset.TierScope} · "
            + $"Patch {dataset.Patch} · {dataset.PolicyKey ?? "no policy"}; "
            + $"matches={dataset.AcceptedMatches}, keys={loadResult.MatchupIndex.Count}");

        var sample = dataset.Matchups?.FirstOrDefault(matchup =>
            matchup.SampleSize > 0
            && !string.Equals(
                matchup.ChampionA,
                matchup.ChampionB,
                StringComparison.OrdinalIgnoreCase));

        if (sample is null)
        {
            Debug.WriteLine("Statistics calculation check skipped: no valid matchup.");
            return;
        }

        var statisticsService = new MatchupStatisticsService(dataService);
        var presenter = new MatchupStatisticsPresenter(championMetadataService);
        await ValidateSampleReliabilityPresentationAsync(
            dataset,
            statisticsService,
            presenter);
        await ValidateOutcomeAdvantageAsync(
            dataset,
            statisticsService,
            presenter);
        await ValidateLaneIndicatorsAsync(dataset, statisticsService, presenter);
        var forwardQuery = await statisticsService.GetStatisticsAsync(
            sample.Role,
            sample.ChampionA,
            sample.ChampionB);
        var reverseQuery = await statisticsService.GetStatisticsAsync(
            sample.Role,
            sample.ChampionB,
            sample.ChampionA);
        var forward = forwardQuery.Statistics;
        var reverse = reverseQuery.Statistics;
        var directionsValid = forwardQuery.Status == MatchupStatisticsQueryStatus.Success
            && reverseQuery.Status == MatchupStatisticsQueryStatus.Success
            && forward is not null
            && reverse is not null
            && !forward.WasReversed
            && reverse.WasReversed
            && forward.SampleSize == reverse.SampleSize
            && forward.WinsForQuery == reverse.WinsForOpponent
            && forward.WinsForOpponent == reverse.WinsForQuery
            && NearlyEqual(forward.QueryWinRate, reverse.OpponentWinRate)
            && LevelDirectionsMatch(forward.FirstLevel2, reverse.FirstLevel2)
            && LevelDirectionsMatch(forward.FirstLevel3, reverse.FirstLevel3)
            && LevelDirectionsMatch(forward.FirstLevel6, reverse.FirstLevel6)
            && forward.SoloKillsBefore15ForQuery
                == reverse.SoloKillsBefore15ForOpponent
            && forward.SoloKillsBefore15ForOpponent
                == reverse.SoloKillsBefore15ForQuery;
        var ratesValid = forward is not null
            && reverse is not null
            && StatisticsRatesAreValid(forward)
            && StatisticsRatesAreValid(reverse);

        if (forward is null)
        {
            Debug.WriteLine("Statistics calculation validation: False");
            return;
        }
        var formattingValid = MatchupStatisticsPresenter.FormatPercentage(0) == "0.0%"
            && MatchupStatisticsPresenter.FormatPercentage(0.5) == "50.0%"
            && MatchupStatisticsPresenter.FormatPercentage(1) == "100.0%"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(0) == "0.00 / 경기"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(0.284) == "0.28 / 경기"
            && MatchupStatisticsPresenter.FormatEventsPerMatch(1.2) == "1.20 / 경기";
        Debug.WriteLine(
            "Statistics validation: "
            + $"directions={directionsValid}, rates={ratesValid}, "
            + $"formatting={formattingValid}");
    }

    private static void ValidateLaneIndicatorBoundaries()
    {
        var insufficient = new[] { 0, 1, 19 }.All(sample =>
            Level(sample, 0, 0, sample, 0, 0, sample, 0, 0, sample)
                == LevelTempoLevel.InsufficientEvidence
            && CombatPressureClassifier.Classify(sample, 0, 0).Level
                == CombatPressureLevel.InsufficientEvidence);
        var levelBoundaries = Level(20, 10, 9, 1, 10, 9, 1, 9, 10, 1)
                == LevelTempoLevel.AllyTendency
            && Level(20, 9, 10, 1, 9, 10, 1, 10, 9, 1)
                == LevelTempoLevel.EnemyTendency
            && Level(100, 52, 48, 0, 50, 50, 0, 50, 50, 0)
                == LevelTempoLevel.Neutral
            && Level(20, 10, 9, 1, 9, 10, 1, 10, 10, 0)
                == LevelTempoLevel.Neutral;
        var signalPatterns = Level(20, 10, 9, 1, 10, 10, 0, 10, 9, 1)
                == LevelTempoLevel.AllyTendency
            && Level(20, 10, 9, 1, 9, 10, 1, 10, 9, 1)
                == LevelTempoLevel.AllyTendency;
        var combatBoundaries = CombatPressureClassifier.Classify(20, 2, 0).Level
                == CombatPressureLevel.AllyTendency
            && CombatPressureClassifier.Classify(20, 0, 2).Level
                == CombatPressureLevel.EnemyTendency
            && CombatPressureClassifier.Classify(20, 1, 0).Level
                == CombatPressureLevel.Neutral
            && CombatPressureClassifier.Classify(20, 0, 1).Level
                == CombatPressureLevel.Neutral
            && CombatPressureClassifier.Classify(20, 25, 25).Level
                == CombatPressureLevel.Neutral;
        var valid = insufficient && levelBoundaries && signalPatterns && combatBoundaries;
        Debug.Assert(valid, "Lane indicator boundary validation failed.");
        Debug.WriteLine($"Lane indicator boundaries: valid={valid}");

        static LevelTempoLevel Level(
            int sample,
            int l2a, int l2b, int l2u,
            int l3a, int l3b, int l3u,
            int l6a, int l6b, int l6u) => LevelTempoClassifier.Classify(
                sample, l2a, l2b, l2u, l3a, l3b, l3u, l6a, l6b, l6u).Level;
    }

    private static void ValidateMatchupExplanations()
    {
        var outcomeAlly = OutcomeAdvantageClassifier.Classify(70, 100);
        var outcomeEnemy = OutcomeAdvantageClassifier.Classify(30, 100);
        var outcomeSmallEffect = OutcomeAdvantageClassifier.Classify(52, 100);
        var outcomeUncertain = OutcomeAdvantageClassifier.Classify(8, 22);
        var outcomeInsufficient = OutcomeAdvantageClassifier.Classify(10, 19);
        var outcomeValid = outcomeAlly.Level == OutcomeAdvantageLevel.AllyAdvantage
            && MatchupExplanationService.ExplainOutcome(outcomeAlly).Contains("아군 쪽")
            && outcomeEnemy.Level == OutcomeAdvantageLevel.EnemyAdvantage
            && MatchupExplanationService.ExplainOutcome(outcomeEnemy).Contains("적군 쪽")
            && outcomeSmallEffect.Reason == OutcomeAdvantageReason.EffectBelowThreshold
            && MatchupExplanationService.ExplainOutcome(outcomeSmallEffect).Contains("차이가 현재 기준보다 작아")
            && outcomeUncertain.Reason == OutcomeAdvantageReason.ConfidenceIncludesNeutral
            && MatchupExplanationService.ExplainOutcome(outcomeUncertain).Contains("불확실성")
            && outcomeInsufficient.Reason == OutcomeAdvantageReason.InsufficientSample
            && MatchupExplanationService.ExplainOutcome(outcomeInsufficient).Contains("20경기 미만");

        var levelAlly = LevelTempoClassifier.Classify(
            20, 10, 9, 1, 10, 9, 1, 10, 10, 0);
        var levelAllyAlternate = LevelTempoClassifier.Classify(
            20, 10, 9, 1, 10, 10, 0, 10, 9, 1);
        var levelEnemy = LevelTempoClassifier.Classify(
            20, 9, 10, 1, 9, 10, 1, 10, 10, 0);
        var levelConflict = LevelTempoClassifier.Classify(
            20, 10, 9, 1, 9, 10, 1, 10, 10, 0);
        var levelInsufficient = LevelTempoClassifier.Classify(
            19, 9, 9, 1, 9, 9, 1, 9, 9, 1);
        var levelValid = MatchupExplanationService.ExplainLevelTempo(levelAlly).Contains("L2·L3")
            && MatchupExplanationService.ExplainLevelTempo(levelAllyAlternate).Contains("L2·L6")
            && MatchupExplanationService.ExplainLevelTempo(levelEnemy).Contains("적군 쪽")
            && MatchupExplanationService.ExplainLevelTempo(levelConflict).Contains("서로 다른 방향")
            && MatchupExplanationService.ExplainLevelTempo(levelInsufficient).Contains("20경기 미만");

        var combatAlly = CombatPressureClassifier.Classify(20, 2, 0);
        var combatEnemy = CombatPressureClassifier.Classify(20, 0, 2);
        var combatNeutral = CombatPressureClassifier.Classify(20, 1, 0);
        var combatInsufficient = CombatPressureClassifier.Classify(19, 10, 0);
        var combatValid = MatchupExplanationService.ExplainCombatPressure(combatAlly).Contains("아군 쪽")
            && MatchupExplanationService.ExplainCombatPressure(combatEnemy).Contains("적군 쪽")
            && MatchupExplanationService.ExplainCombatPressure(combatNeutral).Contains("0.10보다 작아")
            && MatchupExplanationService.ExplainCombatPressure(combatInsufficient).Contains("20경기 미만");
        var reverseValid = MatchupExplanationService.ExplainOutcome(outcomeAlly).Contains("아군 쪽")
            && MatchupExplanationService.ExplainOutcome(outcomeEnemy).Contains("적군 쪽")
            && MatchupExplanationService.ExplainLevelTempo(levelAlly).Contains("아군 쪽")
            && MatchupExplanationService.ExplainLevelTempo(levelEnemy).Contains("적군 쪽")
            && MatchupExplanationService.ExplainCombatPressure(combatAlly).Contains("아군 쪽")
            && MatchupExplanationService.ExplainCombatPressure(combatEnemy).Contains("적군 쪽");
        var valid = outcomeValid && levelValid && combatValid && reverseValid;
        Debug.Assert(valid, "Matchup explanation validation failed.");
        Debug.WriteLine("Matchup explanations: "
            + $"outcome={outcomeValid}, levelTempo={levelValid}, "
            + $"combatPressure={combatValid}, reverseDirection={reverseValid}");
    }

    private static void ValidateTeamOutcomeSummary()
    {
        var ally = Available(OutcomeAdvantageLevel.AllyAdvantage);
        var enemy = Available(OutcomeAdvantageLevel.EnemyAdvantage);
        var neutral = Available(OutcomeAdvantageLevel.Neutral);
        var insufficient = Available(OutcomeAdvantageLevel.InsufficientEvidence);
        var missing = new TeamOutcomeRoleEvidence(false, null);

        var cases = new (string Name, TeamOutcomeRoleEvidence[] Roles,
            TeamOutcomeSummaryLevel Expected)[]
        {
            ("3v2", [ally, ally, ally, enemy, enemy], TeamOutcomeSummaryLevel.AllyTendency),
            ("1v0+4 insufficient", [ally, insufficient, insufficient, insufficient, insufficient], TeamOutcomeSummaryLevel.InsufficientEvidence),
            ("2v2+neutral", [ally, ally, enemy, enemy, neutral], TeamOutcomeSummaryLevel.Neutral),
            ("1 strong vs 2 moderate", [ally, enemy, enemy, neutral, neutral], TeamOutcomeSummaryLevel.EnemyTendency),
            ("2 strong vs 1 moderate", [ally, ally, enemy, neutral, neutral], TeamOutcomeSummaryLevel.AllyTendency),
            ("all neutral", [neutral, neutral, neutral, neutral, neutral], TeamOutcomeSummaryLevel.Neutral),
            ("all insufficient", [insufficient, insufficient, insufficient, insufficient, insufficient], TeamOutcomeSummaryLevel.InsufficientEvidence),
            ("partial missing", [ally, ally, enemy, neutral, missing], TeamOutcomeSummaryLevel.AllyTendency),
            ("1 directional", [ally, neutral, neutral, neutral, neutral], TeamOutcomeSummaryLevel.Neutral),
            ("2v0", [ally, ally, neutral, neutral, neutral], TeamOutcomeSummaryLevel.AllyTendency),
            ("2v1", [ally, ally, enemy, neutral, neutral], TeamOutcomeSummaryLevel.AllyTendency)
        };
        var fixturesValid = cases.All(item =>
            TeamOutcomeSummaryClassifier.Classify(item.Roles).Level == item.Expected);
        var eligibleBoundaries =
            TeamOutcomeSummaryClassifier.Classify([]).Level
                == TeamOutcomeSummaryLevel.InsufficientEvidence
            && TeamOutcomeSummaryClassifier.Classify([neutral]).Level
                == TeamOutcomeSummaryLevel.InsufficientEvidence
            && TeamOutcomeSummaryClassifier.Classify([neutral, neutral]).Level
                == TeamOutcomeSummaryLevel.InsufficientEvidence
            && TeamOutcomeSummaryClassifier.Classify([neutral, neutral, neutral]).Level
                == TeamOutcomeSummaryLevel.Neutral;
        var partial = TeamOutcomeSummaryClassifier.Classify(
            [ally, ally, enemy, neutral, missing]);
        var diagnosticsValid = partial.TotalRoleCount == 5
            && partial.AvailableRoleCount == 4
            && partial.EligibleRoleCount == 4
            && partial.AllyDirectionalCount == 2
            && partial.EnemyDirectionalCount == 1
            && partial.NeutralCount == 1
            && partial.InsufficientCount == 0
            && partial.UnavailableCount == 1;
        var valid = fixturesValid && eligibleBoundaries && diagnosticsValid;
        Debug.Assert(valid, "Team Outcome Summary validation failed.");
        Debug.WriteLine("Team Outcome Summary: "
            + $"fixtures={fixturesValid}, boundaries={eligibleBoundaries}, "
            + $"diagnostics={diagnosticsValid}");

        static TeamOutcomeRoleEvidence Available(OutcomeAdvantageLevel level) =>
            new(true, level);
    }

    private static async Task ValidateLaneIndicatorsAsync(
        StatisticsDataset dataset,
        MatchupStatisticsService statisticsService,
        MatchupStatisticsPresenter presenter)
    {
        var entries = dataset.Matchups ?? [];
        var reverseSymmetry = entries.All(entry =>
        {
            var forwardLevel = ClassifyLevel(entry, false);
            var reverseLevel = ClassifyLevel(entry, true);
            var forwardCombat = ClassifyCombat(entry, false);
            var reverseCombat = ClassifyCombat(entry, true);
            return ReverseLevel(forwardLevel) == reverseLevel
                && ReverseCombat(forwardCombat) == reverseCombat;
        });
        var levelCounts = entries.GroupBy(entry => ClassifyLevel(entry, false))
            .ToDictionary(group => group.Key, group => group.Count());
        var combatCounts = entries.GroupBy(entry => ClassifyCombat(entry, false))
            .ToDictionary(group => group.Key, group => group.Count());
        var researchMatch = entries.Count != 13_021
            || (levelCounts.GetValueOrDefault(LevelTempoLevel.AllyTendency) == 780
                && levelCounts.GetValueOrDefault(LevelTempoLevel.Neutral) == 240
                && levelCounts.GetValueOrDefault(LevelTempoLevel.EnemyTendency) == 765
                && levelCounts.GetValueOrDefault(LevelTempoLevel.InsufficientEvidence) == 11_236
                && combatCounts.GetValueOrDefault(CombatPressureLevel.AllyTendency) == 531
                && combatCounts.GetValueOrDefault(CombatPressureLevel.Neutral) == 781
                && combatCounts.GetValueOrDefault(CombatPressureLevel.EnemyTendency) == 473
                && combatCounts.GetValueOrDefault(CombatPressureLevel.InsufficientEvidence) == 11_236);
        var sample = entries.FirstOrDefault(entry => entry.SampleSize >= 20);
        var presentationValid = false;
        if (sample is not null)
        {
            var query = await statisticsService.GetStatisticsAsync(
                sample.Role, sample.ChampionA, sample.ChampionB);
            if (query.Statistics is not null)
            {
                var presentation = await presenter.PresentAsync(query.Statistics, dataset);
                presentationValid = presentation.HasLevelTempo
                    && presentation.HasCombatPressure;
            }
        }
        var notFound = presenter.PresentStatus(MatchupStatisticsQueryStatus.NotFound);
        var missing = await presenter.PresentRoleStatusAsync(
            MatchupStatisticsQueryStatus.InvalidQuery, "MID", null, null);
        var absentValid = !notFound.HasLevelTempo && !notFound.HasCombatPressure
            && !missing.HasLevelTempo && !missing.HasCombatPressure;
        var valid = reverseSymmetry && researchMatch && presentationValid && absentValid;
        Debug.Assert(valid, "Lane indicator production validation failed.");
        Debug.WriteLine("Production lane indicators: "
            + $"levelA={levelCounts.GetValueOrDefault(LevelTempoLevel.AllyTendency)}, "
            + $"levelNeutral={levelCounts.GetValueOrDefault(LevelTempoLevel.Neutral)}, "
            + $"levelB={levelCounts.GetValueOrDefault(LevelTempoLevel.EnemyTendency)}, "
            + $"levelInsufficient={levelCounts.GetValueOrDefault(LevelTempoLevel.InsufficientEvidence)}, "
            + $"combatA={combatCounts.GetValueOrDefault(CombatPressureLevel.AllyTendency)}, "
            + $"combatNeutral={combatCounts.GetValueOrDefault(CombatPressureLevel.Neutral)}, "
            + $"combatB={combatCounts.GetValueOrDefault(CombatPressureLevel.EnemyTendency)}, "
            + $"combatInsufficient={combatCounts.GetValueOrDefault(CombatPressureLevel.InsufficientEvidence)}, "
            + $"researchMatch={researchMatch}, symmetry={reverseSymmetry}, "
            + $"presentation={presentationValid}, absent={absentValid}");

        static LevelTempoLevel ClassifyLevel(StatisticsMatchupEntry entry, bool reverse) =>
            LevelTempoClassifier.Classify(
                entry.SampleSize,
                reverse ? entry.FirstLevel2ForB : entry.FirstLevel2ForA,
                reverse ? entry.FirstLevel2ForA : entry.FirstLevel2ForB,
                entry.FirstLevel2Unknown,
                reverse ? entry.FirstLevel3ForB : entry.FirstLevel3ForA,
                reverse ? entry.FirstLevel3ForA : entry.FirstLevel3ForB,
                entry.FirstLevel3Unknown,
                reverse ? entry.FirstLevel6ForB : entry.FirstLevel6ForA,
                reverse ? entry.FirstLevel6ForA : entry.FirstLevel6ForB,
                entry.FirstLevel6Unknown).Level;
        static CombatPressureLevel ClassifyCombat(StatisticsMatchupEntry entry, bool reverse) =>
            CombatPressureClassifier.Classify(
                entry.SampleSize,
                reverse ? entry.SoloKillsBefore15ForB : entry.SoloKillsBefore15ForA,
                reverse ? entry.SoloKillsBefore15ForA : entry.SoloKillsBefore15ForB).Level;
        static LevelTempoLevel ReverseLevel(LevelTempoLevel level) => level switch
        {
            LevelTempoLevel.AllyTendency => LevelTempoLevel.EnemyTendency,
            LevelTempoLevel.EnemyTendency => LevelTempoLevel.AllyTendency,
            _ => level
        };
        static CombatPressureLevel ReverseCombat(CombatPressureLevel level) => level switch
        {
            CombatPressureLevel.AllyTendency => CombatPressureLevel.EnemyTendency,
            CombatPressureLevel.EnemyTendency => CombatPressureLevel.AllyTendency,
            _ => level
        };
    }

    private static void ValidateSampleReliabilityBoundaries()
    {
        var cases = new (int SampleSize, SampleReliabilityLevel Expected)[]
        {
            (0, SampleReliabilityLevel.Unknown),
            (1, SampleReliabilityLevel.Poor),
            (4, SampleReliabilityLevel.Poor),
            (5, SampleReliabilityLevel.Reference),
            (19, SampleReliabilityLevel.Reference),
            (20, SampleReliabilityLevel.Moderate),
            (49, SampleReliabilityLevel.Moderate),
            (50, SampleReliabilityLevel.RelativelyStrong),
            (100, SampleReliabilityLevel.RelativelyStrong)
        };
        var valid = cases.All(item =>
            SampleReliabilityClassifier.Classify(item.SampleSize) == item.Expected);

        Debug.Assert(valid, "Sample reliability boundary validation failed.");
        Debug.WriteLine(
            "Sample reliability boundaries: "
            + string.Join(", ", cases.Select(item =>
                $"{item.SampleSize}={SampleReliabilityClassifier.Classify(item.SampleSize)}"))
            + $"; valid={valid}");
    }

    private static void ValidateOutcomeAdvantageBoundaries()
    {
        var sampleBoundariesValid =
            OutcomeAdvantageClassifier.Classify(0, 0).Level
                == OutcomeAdvantageLevel.InsufficientEvidence
            && OutcomeAdvantageClassifier.Classify(1, 1).Level
                == OutcomeAdvantageLevel.InsufficientEvidence
            && OutcomeAdvantageClassifier.Classify(10, 19).Level
                == OutcomeAdvantageLevel.InsufficientEvidence
            && OutcomeAdvantageClassifier.Classify(10, 20).Level
                == OutcomeAdvantageLevel.Neutral;
        var effectBoundariesValid =
            OutcomeAdvantageClassifier.Classify(500, 1_000).Level
                == OutcomeAdvantageLevel.Neutral
            && OutcomeAdvantageClassifier.Classify(549, 1_000).Level
                == OutcomeAdvantageLevel.Neutral
            && OutcomeAdvantageClassifier.Classify(550, 1_000).Level
                == OutcomeAdvantageLevel.AllyAdvantage
            && OutcomeAdvantageClassifier.Classify(451, 1_000).Level
                == OutcomeAdvantageLevel.Neutral
            && OutcomeAdvantageClassifier.Classify(450, 1_000).Level
                == OutcomeAdvantageLevel.EnemyAdvantage;
        var strictWilsonBoundariesValid =
            OutcomeAdvantageClassifier.ClassifyEligible(
                550,
                1_000,
                new WilsonInterval(0.5, 0.6)) == OutcomeAdvantageLevel.Neutral
            && OutcomeAdvantageClassifier.ClassifyEligible(
                450,
                1_000,
                new WilsonInterval(0.4, 0.5)) == OutcomeAdvantageLevel.Neutral;
        var wilsonUncertaintyValid =
            OutcomeAdvantageClassifier.Classify(8, 22).Level
                == OutcomeAdvantageLevel.Neutral;
        var invalidInputsRejected = ThrowsOutOfRange(-1, 0)
            && ThrowsOutOfRange(0, -1)
            && ThrowsOutOfRange(2, 1);
        var valid = sampleBoundariesValid
            && effectBoundariesValid
            && strictWilsonBoundariesValid
            && wilsonUncertaintyValid
            && invalidInputsRejected;

        Debug.Assert(valid, "Outcome advantage boundary validation failed.");
        Debug.WriteLine(
            "Outcome advantage boundaries: "
            + $"samples={sampleBoundariesValid}, effect={effectBoundariesValid}, "
            + $"strictWilson={strictWilsonBoundariesValid}, "
            + $"uncertainty={wilsonUncertaintyValid}, "
            + $"invalidInputs={invalidInputsRejected}");

        static bool ThrowsOutOfRange(int wins, int sampleSize)
        {
            try
            {
                OutcomeAdvantageClassifier.Classify(wins, sampleSize);
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }
    }

    private static async Task ValidateOutcomeAdvantageAsync(
        StatisticsDataset dataset,
        MatchupStatisticsService statisticsService,
        MatchupStatisticsPresenter presenter)
    {
        var entries = dataset.Matchups ?? [];
        var levels = entries.Select(item => OutcomeAdvantageClassifier.Classify(
            item.WinsForA,
            item.SampleSize).Level).ToArray();
        var counts = levels.GroupBy(level => level)
            .ToDictionary(group => group.Key, group => group.Count());
        var symmetryValid = entries.All(item =>
            OutcomeAdvantageClassifier.Classify(
                item.SampleSize - item.WinsForA,
                item.SampleSize).Level
            == Reverse(OutcomeAdvantageClassifier.Classify(
                item.WinsForA,
                item.SampleSize).Level));
        var researchSnapshotMatches = entries.Count != 13_021
            || (Count(OutcomeAdvantageLevel.AllyAdvantage) == 196
                && Count(OutcomeAdvantageLevel.Neutral) == 1_374
                && Count(OutcomeAdvantageLevel.EnemyAdvantage) == 215
                && Count(OutcomeAdvantageLevel.InsufficientEvidence) == 11_236);
        var representativeCasesValid = IsExpected(
                "SUPPORT", "Thresh", "Yuumi", OutcomeAdvantageLevel.AllyAdvantage)
            && IsExpected(
                "TOP", "Ambessa", "Renekton", OutcomeAdvantageLevel.Neutral)
            && IsExpected(
                "JUNGLE", "Khazix", "RekSai", OutcomeAdvantageLevel.Neutral);
        var uncertaintyExplanationValid = await HasUncertaintyExplanationAsync(
            entries.FirstOrDefault(item => item.Role == "JUNGLE"
                && item.ChampionA == "Khazix" && item.ChampionB == "RekSai"));

        var directional = entries.FirstOrDefault(item =>
            OutcomeAdvantageClassifier.Classify(item.WinsForA, item.SampleSize).Level
                == OutcomeAdvantageLevel.AllyAdvantage);
        var neutral = entries.FirstOrDefault(item =>
            OutcomeAdvantageClassifier.Classify(item.WinsForA, item.SampleSize).Level
                == OutcomeAdvantageLevel.Neutral);
        var insufficient = entries.FirstOrDefault(item => item.SampleSize is > 0 and < 20);
        var presentationValid = await HasExpectedPresentationAsync(
                directional, OutcomeAdvantageLevel.AllyAdvantage)
            && await HasExpectedPresentationAsync(neutral, OutcomeAdvantageLevel.Neutral)
            && await HasExpectedPresentationAsync(
                insufficient, OutcomeAdvantageLevel.InsufficientEvidence);
        var notFound = presenter.PresentStatus(MatchupStatisticsQueryStatus.NotFound);
        var missingRole = await presenter.PresentRoleStatusAsync(
            MatchupStatisticsQueryStatus.InvalidQuery, "MID", null, null);
        var absentStatesValid = !notFound.HasOutcomeEvaluation
            && !missingRole.HasOutcomeEvaluation;
        var valid = symmetryValid
            && researchSnapshotMatches
            && representativeCasesValid
            && uncertaintyExplanationValid
            && presentationValid
            && absentStatesValid;

        Debug.Assert(valid, "Outcome advantage production validation failed.");
        Debug.WriteLine(
            "Outcome advantage production: "
            + $"ally={Count(OutcomeAdvantageLevel.AllyAdvantage)}, "
            + $"neutral={Count(OutcomeAdvantageLevel.Neutral)}, "
            + $"enemy={Count(OutcomeAdvantageLevel.EnemyAdvantage)}, "
            + $"insufficient={Count(OutcomeAdvantageLevel.InsufficientEvidence)}, "
            + $"symmetry={symmetryValid}, researchMatch={researchSnapshotMatches}, "
            + $"representativeCases={representativeCasesValid}, "
            + $"uncertaintyExplanation={uncertaintyExplanationValid}, "
            + $"presentation={presentationValid}, absentStates={absentStatesValid}");

        int Count(OutcomeAdvantageLevel level) => counts.GetValueOrDefault(level);

        bool IsExpected(
            string role,
            string championA,
            string championB,
            OutcomeAdvantageLevel expected)
        {
            var entry = entries.FirstOrDefault(item => item.Role == role
                && item.ChampionA == championA
                && item.ChampionB == championB);
            return entry is not null
                && OutcomeAdvantageClassifier.Classify(
                    entry.WinsForA,
                    entry.SampleSize).Level == expected;
        }

        async Task<bool> HasExpectedPresentationAsync(
            StatisticsMatchupEntry? entry,
            OutcomeAdvantageLevel expected)
        {
            if (entry is null)
            {
                return false;
            }

            var query = await statisticsService.GetStatisticsAsync(
                entry.Role,
                entry.ChampionA,
                entry.ChampionB);
            if (query.Status != MatchupStatisticsQueryStatus.Success
                || query.Statistics is null)
            {
                return false;
            }

            var presentation = await presenter.PresentAsync(query.Statistics, dataset);
            return presentation.HasOutcomeEvaluation
                && presentation.OutcomeAdvantageLevel == expected
                && !string.IsNullOrWhiteSpace(presentation.OutcomeAdvantageText)
                && !string.IsNullOrWhiteSpace(presentation.OutcomeAdvantageDescription)
                && presentation.DisplayMetrics.Count == 5;
        }

        async Task<bool> HasUncertaintyExplanationAsync(StatisticsMatchupEntry? entry)
        {
            if (entry is null)
            {
                return false;
            }

            var query = await statisticsService.GetStatisticsAsync(
                entry.Role, entry.ChampionA, entry.ChampionB);
            if (query.Statistics is null)
            {
                return false;
            }

            var presentation = await presenter.PresentAsync(query.Statistics, dataset);
            return presentation.OutcomeAdvantage?.Reason
                    == OutcomeAdvantageReason.ConfidenceIncludesNeutral
                && presentation.OutcomeAdvantageDescription.Contains("불확실성");
        }

        static OutcomeAdvantageLevel Reverse(OutcomeAdvantageLevel level) => level switch
        {
            OutcomeAdvantageLevel.AllyAdvantage => OutcomeAdvantageLevel.EnemyAdvantage,
            OutcomeAdvantageLevel.EnemyAdvantage => OutcomeAdvantageLevel.AllyAdvantage,
            _ => level
        };
    }

    private static async Task ValidateSampleReliabilityPresentationAsync(
        StatisticsDataset dataset,
        MatchupStatisticsService statisticsService,
        MatchupStatisticsPresenter presenter)
    {
        var entries = dataset.Matchups ?? [];
        var poorEntry = entries.FirstOrDefault(item => item.SampleSize is >= 1 and <= 4);
        var strongEntry = entries.FirstOrDefault(item =>
            item.SampleSize >= SampleReliabilityClassifier.RelativelyStrongMinimumSampleSize);

        async Task<bool> IsExpectedAsync(
            StatisticsMatchupEntry? entry,
            SampleReliabilityLevel expected,
            string expectedText)
        {
            if (entry is null)
            {
                return false;
            }

            var query = await statisticsService.GetStatisticsAsync(
                entry.Role,
                entry.ChampionA,
                entry.ChampionB);
            if (query.Status != MatchupStatisticsQueryStatus.Success
                || query.Statistics is null)
            {
                return false;
            }

            var presentation = await presenter.PresentAsync(query.Statistics, dataset);
            return presentation.HasReliability
                && presentation.ReliabilityLevel == expected
                && presentation.ReliabilityText == expectedText
                && !string.IsNullOrWhiteSpace(presentation.ReliabilityDescription)
                && presentation.DisplayMetrics.Count == 5;
        }

        var poorValid = await IsExpectedAsync(
            poorEntry,
            SampleReliabilityLevel.Poor,
            "표본 부족");
        var strongValid = await IsExpectedAsync(
            strongEntry,
            SampleReliabilityLevel.RelativelyStrong,
            "상대적으로 충분");
        var notFound = presenter.PresentStatus(MatchupStatisticsQueryStatus.NotFound);
        var missingRole = await presenter.PresentRoleStatusAsync(
            MatchupStatisticsQueryStatus.InvalidQuery,
            "MID",
            null,
            null);
        var absenceValid = !notFound.HasReliability && !missingRole.HasReliability;

        Debug.Assert(poorValid && strongValid && absenceValid,
            "Sample reliability presentation validation failed.");
        Debug.WriteLine(
            "Sample reliability presentation: "
            + $"actualPoor={poorValid} (n={poorEntry?.SampleSize}), "
            + $"actual50Plus={strongValid} (n={strongEntry?.SampleSize}), "
            + $"notFoundAndMissingHidden={absenceValid}");
    }

    private static async Task ValidateDeveloperPipelineAsync(
        ChampionMetadataService metadataService)
    {
        var testMode = new DeveloperTestModeService();
        testMode.StartScenario(DeveloperTestModeService.CreateNormal5v5());
        using var liveClient = new LiveClientService();
        var coordinator = new LiveStatisticsCoordinator(
            liveClient,
            presenter: new MatchupStatisticsPresenter(metadataService),
            developerTestMode: testMode);
        var signature = await coordinator.GetCurrentMatchupAsync();
        var presentation = signature is null
            ? null
            : await coordinator.GetPresentationAsync(signature);
        var valid = signature?.Roles.Count == 5
            && signature.Roles.All(role =>
                !string.IsNullOrWhiteSpace(role.QueryChampion)
                && !string.IsNullOrWhiteSpace(role.OpponentChampion))
            && presentation?.DisplayMatchups.Count == 5
            && presentation.HasOutcomeSummary
            && presentation.OutcomeSummary?.Result.TotalRoleCount == 5;
        testMode.Stop();
        Debug.WriteLine($"Developer test production pipeline: {valid}");
    }

    private static void ValidateDeveloperTestPresets()
    {
        var normal = DeveloperTestModeService.CreateNormal5v5();
        var oneVersusOne = DeveloperTestModeService.CreateTop1v1();
        var service = new DeveloperTestModeService();
        service.StartScenario(normal);
        service.Stop();
        var normalValid = normal.Players.Count == 10
            && PositionNormalizer.StandardPositions.All(role =>
                normal.Players.Count(player => player.NormalizedPosition == role) == 2);
        var oneVersusOneValid = oneVersusOne.Players.Count == 2
            && oneVersusOne.Players.All(player => player.NormalizedPosition == "TOP");
        Debug.WriteLine(
            $"Developer test presets: normal5v5={normalValid}, "
            + $"top1v1={oneVersusOneValid}, stop={!service.IsActive}");
    }

    private static async Task ValidateChampionMetadataAsync(
        ChampionMetadataService metadataService)
    {
        var expected = new Dictionary<string, string>
        {
            ["Aatrox"] = "아트록스",
            ["Akali"] = "아칼리",
            ["LeeSin"] = "리 신",
            ["Viego"] = "비에고"
        };
        var knownValid = true;

        foreach (var pair in expected)
        {
            var info = await metadataService.GetDisplayInfoAsync(pair.Key);
            knownValid &= info.KoreanDisplayName == pair.Value
                && info.IconSource is not null;
            Debug.WriteLine(
                $"Champion metadata: {pair.Key} -> {info.KoreanDisplayName}; "
                + $"icon={info.IconSource is not null}");
        }

        const string missingId = "UnknownChampion123";
        var fallback = await metadataService.GetDisplayInfoAsync(missingId);
        var fallbackValid = fallback.KoreanDisplayName == missingId
            && fallback.IconSource is null;
        Debug.WriteLine(
            $"Champion metadata validation: known={knownValid}, "
            + $"fallback={fallbackValid}");
    }

    private static bool LevelDirectionsMatch(
        LevelRaceStatistics forward,
        LevelRaceStatistics reverse) =>
        forward.ForQuery == reverse.ForOpponent
        && forward.ForOpponent == reverse.ForQuery
        && forward.Unknown == reverse.Unknown;

    private static bool StatisticsRatesAreValid(MatchupStatisticsResult value) =>
        IsUnitRate(value.QueryWinRate)
        && IsUnitRate(value.OpponentWinRate)
        && NearlyEqual(value.QueryWinRate + value.OpponentWinRate, 1)
        && LevelRatesAreValid(value.FirstLevel2)
        && LevelRatesAreValid(value.FirstLevel3)
        && LevelRatesAreValid(value.FirstLevel6)
        && value.SoloKillsPerMatchForQuery >= 0
        && value.SoloKillsPerMatchForOpponent >= 0;

    private static bool LevelRatesAreValid(LevelRaceStatistics value) =>
        IsUnitRate(value.QueryRate)
        && IsUnitRate(value.OpponentRate)
        && IsUnitRate(value.UnknownRate)
        && NearlyEqual(
            value.QueryRate + value.OpponentRate + value.UnknownRate,
            1);

    private static bool IsUnitRate(double value) => value is >= 0 and <= 1;

    private static bool NearlyEqual(double left, double right) =>
        Math.Abs(left - right) <= Tolerance;
}
#endif
