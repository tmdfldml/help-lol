# Team Summary Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 31,075
- Matchup keys: 13,021
- Scope: `MASTER_PLUS_SEEDED` / `KR` / queue `420`
- Dataset revision: 1788710852

## Research Scope

Summarize five independent role-level categorical tendencies for faster reading; not a team win probability or composition model.

## Important Data Limitation

> The dataset contains independent role matchup aggregates, not observed five-role team combinations. Empirical runtime team-summary coverage and predictive validity cannot be measured from this file.

## Per-role Outcome Distribution

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Ally | 37 | 0.96% |
| TOP | Neutral | 317 | 8.23% |
| TOP | Enemy | 45 | 1.17% |
| TOP | Insufficient | 3,454 | 89.64% |
| JUNGLE | Ally | 31 | 1.53% |
| JUNGLE | Neutral | 319 | 15.75% |
| JUNGLE | Enemy | 41 | 2.02% |
| JUNGLE | Insufficient | 1,634 | 80.69% |
| MID | Ally | 53 | 1.68% |
| MID | Neutral | 306 | 9.71% |
| MID | Enemy | 64 | 2.03% |
| MID | Insufficient | 2,729 | 86.58% |
| ADC | Ally | 20 | 1.35% |
| ADC | Neutral | 192 | 12.93% |
| ADC | Enemy | 27 | 1.82% |
| ADC | Insufficient | 1,246 | 83.91% |
| SUPPORT | Ally | 55 | 2.19% |
| SUPPORT | Neutral | 240 | 9.58% |
| SUPPORT | Enemy | 38 | 1.52% |
| SUPPORT | Insufficient | 2,173 | 86.71% |

## Per-role Reliability Distribution

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Unknown | 0 | 0.00% |
| TOP | Poor | 2,503 | 64.96% |
| TOP | Reference | 951 | 24.68% |
| TOP | Moderate | 290 | 7.53% |
| TOP | RelativelyStrong | 109 | 2.83% |
| JUNGLE | Unknown | 0 | 0.00% |
| JUNGLE | Poor | 996 | 49.19% |
| JUNGLE | Reference | 638 | 31.51% |
| JUNGLE | Moderate | 267 | 13.19% |
| JUNGLE | RelativelyStrong | 124 | 6.12% |
| MID | Unknown | 0 | 0.00% |
| MID | Poor | 2,061 | 65.39% |
| MID | Reference | 668 | 21.19% |
| MID | Moderate | 278 | 8.82% |
| MID | RelativelyStrong | 145 | 4.60% |
| ADC | Unknown | 0 | 0.00% |
| ADC | Poor | 953 | 64.18% |
| ADC | Reference | 293 | 19.73% |
| ADC | Moderate | 116 | 7.81% |
| ADC | RelativelyStrong | 123 | 8.28% |
| SUPPORT | Unknown | 0 | 0.00% |
| SUPPORT | Poor | 1,619 | 64.60% |
| SUPPORT | Reference | 554 | 22.11% |
| SUPPORT | Moderate | 187 | 7.46% |
| SUPPORT | RelativelyStrong | 146 | 5.83% |

## Outcome × Reliability

Counts are empirical per-key classifier results; they are not observed team combinations.

| Role | Outcome | Reliability | Count |
|---|---|---|---:|
| TOP | Ally | Moderate | 25 |
| TOP | Ally | RelativelyStrong | 12 |
| TOP | Neutral | Moderate | 231 |
| TOP | Neutral | RelativelyStrong | 86 |
| TOP | Enemy | Moderate | 34 |
| TOP | Enemy | RelativelyStrong | 11 |
| TOP | Insufficient | Poor | 2,503 |
| TOP | Insufficient | Reference | 951 |
| JUNGLE | Ally | Moderate | 22 |
| JUNGLE | Ally | RelativelyStrong | 9 |
| JUNGLE | Neutral | Moderate | 225 |
| JUNGLE | Neutral | RelativelyStrong | 94 |
| JUNGLE | Enemy | Moderate | 20 |
| JUNGLE | Enemy | RelativelyStrong | 21 |
| JUNGLE | Insufficient | Poor | 996 |
| JUNGLE | Insufficient | Reference | 638 |
| MID | Ally | Moderate | 28 |
| MID | Ally | RelativelyStrong | 25 |
| MID | Neutral | Moderate | 208 |
| MID | Neutral | RelativelyStrong | 98 |
| MID | Enemy | Moderate | 42 |
| MID | Enemy | RelativelyStrong | 22 |
| MID | Insufficient | Poor | 2,061 |
| MID | Insufficient | Reference | 668 |
| ADC | Ally | Moderate | 6 |
| ADC | Ally | RelativelyStrong | 14 |
| ADC | Neutral | Moderate | 94 |
| ADC | Neutral | RelativelyStrong | 98 |
| ADC | Enemy | Moderate | 16 |
| ADC | Enemy | RelativelyStrong | 11 |
| ADC | Insufficient | Poor | 953 |
| ADC | Insufficient | Reference | 293 |
| SUPPORT | Ally | Moderate | 37 |
| SUPPORT | Ally | RelativelyStrong | 18 |
| SUPPORT | Neutral | Moderate | 134 |
| SUPPORT | Neutral | RelativelyStrong | 106 |
| SUPPORT | Enemy | Moderate | 16 |
| SUPPORT | Enemy | RelativelyStrong | 22 |
| SUPPORT | Insufficient | Poor | 1,619 |
| SUPPORT | Insufficient | Reference | 554 |

## Aggregation Model Definitions

### A_EQUAL_MIN2 — Equal vote, minimum 2 eligible

Eligible roles < 2 => Insufficient; otherwise +1/-1 majority, tie => Neutral.

- Strength: Simplest and highly explainable.
- Risk: One directional role can dominate several Neutral roles.

### A_EQUAL_MIN3 — Equal vote, minimum 3 eligible

Eligible roles < 3 => Insufficient; otherwise +1/-1 majority, tie => Neutral.

- Strength: Robust to missing/insufficient roles.
- Risk: Still allows one directional role to determine the result.

### B_RELIABILITY_WEIGHTED — Reliability-weighted categorical vote

Minimum 3 eligible; Moderate=1, RelativelyStrong=1.5, then weighted directional majority.

- Strength: Acknowledges broad evidence bands.
- Risk: Weights are heuristic, not calibrated accuracy; harder to explain.

### C_SAMPLE_WEIGHTED_CAPPED — Capped sample-weighted categorical vote

Minimum 3 eligible; directional weight=sqrt(min(sampleSize,100)).

- Strength: Limits but does not remove sample dominance.
- Risk: A single large matchup may override multiple Role directions.

### D_DIRECTIONAL_MIN2 — Directional majority only

At least 2 directional roles; compare directional counts, tie => Neutral.

- Strength: Clear evidence-count interpretation.
- Risk: All-Neutral evidence becomes Insufficient despite being fully available.

### E_CONSERVATIVE_EQUAL — Conservative equal majority

Minimum 3 eligible; a side needs at least 2 directional roles and more votes than the other side; otherwise Neutral.

- Strength: Simple, resists one-role domination, and handles Neutral explicitly.
- Risk: May leave many runtime states Neutral; empirical frequency is unknown.

### F_STRONG_ONLY — RelativelyStrong-only

Only sampleSize >= 50 roles are eligible; at least 2 required, then equal directional majority.

- Strength: Uses the strongest existing reliability band only.
- Risk: Very low per-key availability and likely poor runtime coverage.

### Candidate comparison

| Model | Simplicity | Explainability | Conservative | Missing robustness | Reliability awareness | Dominance risk | UI clarity |
|---|---|---|---|---|---|---|---|
| A_EQUAL_MIN2 | High | High | Low | Medium | None | Low | High |
| A_EQUAL_MIN3 | High | High | Medium | High | None | Low | High |
| B_RELIABILITY_WEIGHTED | Medium | Medium | Medium | High | Heuristic | Medium | Medium |
| C_SAMPLE_WEIGHTED_CAPPED | Low | Low | Low | High | Raw sample | High | Low |
| D_DIRECTIONAL_MIN2 | High | High | Medium | Medium | None | Low | High |
| E_CONSERVATIVE_EQUAL | High | High | High | High | None | Low | High |
| F_STRONG_ONLY | High | High | High | Low | Categorical | Low | Medium |

## Equal Vote Analysis

Eligible roles < 2 => Insufficient; otherwise +1/-1 majority, tie => Neutral.

Strength: Simplest and highly explainable.
Risk: One directional role can dominate several Neutral roles.

## Reliability-weighted Analysis

Minimum 3 eligible; Moderate=1, RelativelyStrong=1.5, then weighted directional majority.

Strength: Acknowledges broad evidence bands.
Risk: Weights are heuristic, not calibrated accuracy; harder to explain.

## Sample-weighted Analysis

Minimum 3 eligible; directional weight=sqrt(min(sampleSize,100)).

Strength: Limits but does not remove sample dominance.
Risk: A single large matchup may override multiple Role directions.

## Conservative Model Analysis

Minimum 3 eligible; a side needs at least 2 directional roles and more votes than the other side; otherwise Neutral.

Strength: Simple, resists one-role domination, and handles Neutral explicitly.
Risk: May leave many runtime states Neutral; empirical frequency is unknown.

## Strong-only Analysis

Only sampleSize >= 50 roles are eligible; at least 2 required, then equal directional majority.

Strength: Uses the strongest existing reliability band only.
Risk: Very low per-key availability and likely poor runtime coverage.

## Missing/NotFound/Insufficient Behavior

Missing and NotFound are unavailable and excluded from eligible-role counts. Insufficient classifier results are also not eligible. Neutral is eligible but contributes no directional vote. Dataset unavailable/invalid prevents the entire summary.

## Representative Team Fixtures

These deterministic fixtures test rule behavior only; they are not sampled games or coverage estimates.

| Fixture | Available | Eligible | A | Neutral | B | Missing | Candidate results |
|---|---:|---:|---:|---:|---:|---:|---|
| A_3A_2B | 5 | 5 | 3 | 0 | 2 | 0 | A_EQUAL_MIN2=AllyTendency; A_EQUAL_MIN3=AllyTendency; B_RELIABILITY_WEIGHTED=AllyTendency; C_SAMPLE_WEIGHTED_CAPPED=AllyTendency; D_DIRECTIONAL_MIN2=AllyTendency; E_CONSERVATIVE_EQUAL=AllyTendency; F_STRONG_ONLY=InsufficientEvidence |
| B_1A_4I | 5 | 1 | 1 | 0 | 0 | 0 | A_EQUAL_MIN2=InsufficientEvidence; A_EQUAL_MIN3=InsufficientEvidence; B_RELIABILITY_WEIGHTED=InsufficientEvidence; C_SAMPLE_WEIGHTED_CAPPED=InsufficientEvidence; D_DIRECTIONAL_MIN2=InsufficientEvidence; E_CONSERVATIVE_EQUAL=InsufficientEvidence; F_STRONG_ONLY=InsufficientEvidence |
| C_2A_2B_1N | 5 | 5 | 2 | 1 | 2 | 0 | A_EQUAL_MIN2=Neutral; A_EQUAL_MIN3=Neutral; B_RELIABILITY_WEIGHTED=Neutral; C_SAMPLE_WEIGHTED_CAPPED=Neutral; D_DIRECTIONAL_MIN2=Neutral; E_CONSERVATIVE_EQUAL=Neutral; F_STRONG_ONLY=InsufficientEvidence |
| D_1A_STRONG_2B | 5 | 5 | 1 | 2 | 2 | 0 | A_EQUAL_MIN2=EnemyTendency; A_EQUAL_MIN3=EnemyTendency; B_RELIABILITY_WEIGHTED=EnemyTendency; C_SAMPLE_WEIGHTED_CAPPED=AllyTendency; D_DIRECTIONAL_MIN2=EnemyTendency; E_CONSERVATIVE_EQUAL=EnemyTendency; F_STRONG_ONLY=InsufficientEvidence |
| E_2A_STRONG_1B | 5 | 5 | 2 | 2 | 1 | 0 | A_EQUAL_MIN2=AllyTendency; A_EQUAL_MIN3=AllyTendency; B_RELIABILITY_WEIGHTED=AllyTendency; C_SAMPLE_WEIGHTED_CAPPED=AllyTendency; D_DIRECTIONAL_MIN2=AllyTendency; E_CONSERVATIVE_EQUAL=AllyTendency; F_STRONG_ONLY=AllyTendency |
| F_ALL_NEUTRAL | 5 | 5 | 0 | 5 | 0 | 0 | A_EQUAL_MIN2=Neutral; A_EQUAL_MIN3=Neutral; B_RELIABILITY_WEIGHTED=Neutral; C_SAMPLE_WEIGHTED_CAPPED=Neutral; D_DIRECTIONAL_MIN2=InsufficientEvidence; E_CONSERVATIVE_EQUAL=Neutral; F_STRONG_ONLY=InsufficientEvidence |
| G_ALL_INSUFFICIENT | 5 | 0 | 0 | 0 | 0 | 0 | A_EQUAL_MIN2=InsufficientEvidence; A_EQUAL_MIN3=InsufficientEvidence; B_RELIABILITY_WEIGHTED=InsufficientEvidence; C_SAMPLE_WEIGHTED_CAPPED=InsufficientEvidence; D_DIRECTIONAL_MIN2=InsufficientEvidence; E_CONSERVATIVE_EQUAL=InsufficientEvidence; F_STRONG_ONLY=InsufficientEvidence |
| H_4_ELIGIBLE_1_MISSING | 4 | 4 | 2 | 1 | 1 | 1 | A_EQUAL_MIN2=AllyTendency; A_EQUAL_MIN3=AllyTendency; B_RELIABILITY_WEIGHTED=AllyTendency; C_SAMPLE_WEIGHTED_CAPPED=AllyTendency; D_DIRECTIONAL_MIN2=AllyTendency; E_CONSERVATIVE_EQUAL=AllyTendency; F_STRONG_ONLY=InsufficientEvidence |

## Outcome Summary Recommendation

Recommend an Outcome-only categorical summary using the conservative equal-vote rule. It summarizes role-level final-result tendencies and must not be described as team win probability.

## Level Tempo Summary Evaluation

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Ally | 171 | 4.44% |
| TOP | Neutral | 17 | 0.44% |
| TOP | Enemy | 211 | 5.48% |
| TOP | Insufficient | 3,454 | 89.64% |
| JUNGLE | Ally | 192 | 9.48% |
| JUNGLE | Neutral | 51 | 2.52% |
| JUNGLE | Enemy | 148 | 7.31% |
| JUNGLE | Insufficient | 1,634 | 80.69% |
| MID | Ally | 190 | 6.03% |
| MID | Neutral | 55 | 1.74% |
| MID | Enemy | 178 | 5.65% |
| MID | Insufficient | 2,729 | 86.58% |
| ADC | Ally | 115 | 7.74% |
| ADC | Neutral | 48 | 3.23% |
| ADC | Enemy | 76 | 5.12% |
| ADC | Insufficient | 1,246 | 83.91% |
| SUPPORT | Ally | 112 | 4.47% |
| SUPPORT | Neutral | 69 | 2.75% |
| SUPPORT | Enemy | 152 | 6.07% |
| SUPPORT | Insufficient | 2,173 | 86.71% |

The same categorical mechanics are technically possible, but the aggregate file cannot validate whether five independent level-timing tendencies form a meaningful team-level tempo measure. Keep Level Tempo on role cards for now.

## Combat Pressure Summary Evaluation

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Ally | 174 | 4.52% |
| TOP | Neutral | 53 | 1.38% |
| TOP | Enemy | 172 | 4.46% |
| TOP | Insufficient | 3,454 | 89.64% |
| JUNGLE | Ally | 97 | 4.79% |
| JUNGLE | Neutral | 212 | 10.47% |
| JUNGLE | Enemy | 82 | 4.05% |
| JUNGLE | Insufficient | 1,634 | 80.69% |
| MID | Ally | 186 | 5.90% |
| MID | Neutral | 91 | 2.89% |
| MID | Enemy | 146 | 4.63% |
| MID | Insufficient | 2,729 | 86.58% |
| ADC | Ally | 57 | 3.84% |
| ADC | Neutral | 125 | 8.42% |
| ADC | Enemy | 57 | 3.84% |
| ADC | Insufficient | 1,246 | 83.91% |
| SUPPORT | Ally | 17 | 0.68% |
| SUPPORT | Neutral | 300 | 11.97% |
| SUPPORT | Enemy | 16 | 0.64% |
| SUPPORT | Insufficient | 2,173 | 86.71% |

Do not promote a team Combat Summary yet. Direct-opponent solo-kill frequency has role-dependent meaning, especially for JUNGLE and SUPPORT, and omits other combat participation.

## UI Terminology Recommendation

Use `팀 결과 경향`, with `아군 쪽 경향`, `적군 쪽 경향`, `뚜렷한 차이 없음`, or `판정 보류`. Avoid team advantage, win probability, composition advantage, and confidence percentages.

## UI Placement Recommendation

Use a compact 40–70 px strip above the InGame role dashboard. Show `판정 가능 4/5 · 아군 3 · 적군 1 · 중립 0` rather than a Team Reliability badge. Long explanation belongs in a later expanded view.

## Limitations

- No observed five-role combination rows are available, so fixture outcomes are behavioral tests rather than empirical frequencies.
- Reliability bands are categorical communication aids, not calibrated probabilities or accuracy multipliers.
- No candidate estimates team win probability, composition synergy, role importance, or an overall advantage score.
- Sample sizes from different matchup keys must not be summed into a team sample size.
- Missing and NotFound roles are unavailable, while Neutral is an eligible non-directional result and Insufficient lacks minimum evidence.

## Production Recommendation

Decision: `A_OUTCOME_ONLY`
Recommended model: `E_CONSERVATIVE_EQUAL`
Rule: Eligible roles below 3 produce InsufficientEvidence. With sufficient eligible roles, at least two same-side directional roles and a strict majority over the opposite side are required; ties and weaker evidence are Neutral.
Rationale: Recommend Outcome Summary only. Keep Level Tempo and Combat Pressure on role cards because aggregate data cannot validate team-level meaning, and direct-opponent solo kills vary in interpretation for JUNGLE/SUPPORT.

## Next Stage

If accepted, Stage 9-4B may implement only the Outcome Team Summary as runtime-derived presentation. No production Team Summary classifier or UI was added in this research stage.
