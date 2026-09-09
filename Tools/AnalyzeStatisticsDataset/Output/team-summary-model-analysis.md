# Team Summary Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 51,649
- Matchup keys: 15,547
- Scope: `MASTER_PLUS_SEEDED` / `KR` / queue `420`
- Dataset revision: 1788854099077

## Research Scope

Summarize five independent role-level categorical tendencies for faster reading; not a team win probability or composition model.

## Important Data Limitation

> The dataset contains independent role matchup aggregates, not observed five-role team combinations. Empirical runtime team-summary coverage and predictive validity cannot be measured from this file.

## Per-role Outcome Distribution

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Ally | 63 | 1.36% |
| TOP | Neutral | 524 | 11.28% |
| TOP | Enemy | 71 | 1.53% |
| TOP | Insufficient | 3,988 | 85.84% |
| JUNGLE | Ally | 69 | 3.00% |
| JUNGLE | Neutral | 435 | 18.90% |
| JUNGLE | Enemy | 80 | 3.48% |
| JUNGLE | Insufficient | 1,718 | 74.63% |
| MID | Ally | 77 | 2.03% |
| MID | Neutral | 446 | 11.76% |
| MID | Enemy | 84 | 2.22% |
| MID | Insufficient | 3,185 | 83.99% |
| ADC | Ally | 27 | 1.51% |
| ADC | Neutral | 247 | 13.77% |
| ADC | Enemy | 51 | 2.84% |
| ADC | Insufficient | 1,469 | 81.88% |
| SUPPORT | Ally | 71 | 2.36% |
| SUPPORT | Neutral | 358 | 11.88% |
| SUPPORT | Enemy | 56 | 1.86% |
| SUPPORT | Insufficient | 2,528 | 83.90% |

## Per-role Reliability Distribution

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Unknown | 0 | 0.00% |
| TOP | Poor | 2,845 | 61.24% |
| TOP | Reference | 1,143 | 24.60% |
| TOP | Moderate | 430 | 9.26% |
| TOP | RelativelyStrong | 228 | 4.91% |
| JUNGLE | Unknown | 0 | 0.00% |
| JUNGLE | Poor | 1,048 | 45.53% |
| JUNGLE | Reference | 670 | 29.11% |
| JUNGLE | Moderate | 328 | 14.25% |
| JUNGLE | RelativelyStrong | 256 | 11.12% |
| MID | Unknown | 0 | 0.00% |
| MID | Poor | 2,378 | 62.71% |
| MID | Reference | 807 | 21.28% |
| MID | Moderate | 328 | 8.65% |
| MID | RelativelyStrong | 279 | 7.36% |
| ADC | Unknown | 0 | 0.00% |
| ADC | Poor | 1,115 | 62.15% |
| ADC | Reference | 354 | 19.73% |
| ADC | Moderate | 151 | 8.42% |
| ADC | RelativelyStrong | 174 | 9.70% |
| SUPPORT | Unknown | 0 | 0.00% |
| SUPPORT | Poor | 1,847 | 61.30% |
| SUPPORT | Reference | 681 | 22.60% |
| SUPPORT | Moderate | 252 | 8.36% |
| SUPPORT | RelativelyStrong | 233 | 7.73% |

## Outcome × Reliability

Counts are empirical per-key classifier results; they are not observed team combinations.

| Role | Outcome | Reliability | Count |
|---|---|---|---:|
| TOP | Ally | Moderate | 39 |
| TOP | Ally | RelativelyStrong | 24 |
| TOP | Neutral | Moderate | 348 |
| TOP | Neutral | RelativelyStrong | 176 |
| TOP | Enemy | Moderate | 43 |
| TOP | Enemy | RelativelyStrong | 28 |
| TOP | Insufficient | Poor | 2,845 |
| TOP | Insufficient | Reference | 1,143 |
| JUNGLE | Ally | Moderate | 41 |
| JUNGLE | Ally | RelativelyStrong | 28 |
| JUNGLE | Neutral | Moderate | 240 |
| JUNGLE | Neutral | RelativelyStrong | 195 |
| JUNGLE | Enemy | Moderate | 47 |
| JUNGLE | Enemy | RelativelyStrong | 33 |
| JUNGLE | Insufficient | Poor | 1,048 |
| JUNGLE | Insufficient | Reference | 670 |
| MID | Ally | Moderate | 36 |
| MID | Ally | RelativelyStrong | 41 |
| MID | Neutral | Moderate | 252 |
| MID | Neutral | RelativelyStrong | 194 |
| MID | Enemy | Moderate | 40 |
| MID | Enemy | RelativelyStrong | 44 |
| MID | Insufficient | Poor | 2,378 |
| MID | Insufficient | Reference | 807 |
| ADC | Ally | Moderate | 11 |
| ADC | Ally | RelativelyStrong | 16 |
| ADC | Neutral | Moderate | 116 |
| ADC | Neutral | RelativelyStrong | 131 |
| ADC | Enemy | Moderate | 24 |
| ADC | Enemy | RelativelyStrong | 27 |
| ADC | Insufficient | Poor | 1,115 |
| ADC | Insufficient | Reference | 354 |
| SUPPORT | Ally | Moderate | 37 |
| SUPPORT | Ally | RelativelyStrong | 34 |
| SUPPORT | Neutral | Moderate | 187 |
| SUPPORT | Neutral | RelativelyStrong | 171 |
| SUPPORT | Enemy | Moderate | 28 |
| SUPPORT | Enemy | RelativelyStrong | 28 |
| SUPPORT | Insufficient | Poor | 1,847 |
| SUPPORT | Insufficient | Reference | 681 |

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
| TOP | Ally | 255 | 5.49% |
| TOP | Neutral | 46 | 0.99% |
| TOP | Enemy | 357 | 7.68% |
| TOP | Insufficient | 3,988 | 85.84% |
| JUNGLE | Ally | 290 | 12.60% |
| JUNGLE | Neutral | 66 | 2.87% |
| JUNGLE | Enemy | 228 | 9.90% |
| JUNGLE | Insufficient | 1,718 | 74.63% |
| MID | Ally | 266 | 7.01% |
| MID | Neutral | 86 | 2.27% |
| MID | Enemy | 255 | 6.72% |
| MID | Insufficient | 3,185 | 83.99% |
| ADC | Ally | 138 | 7.69% |
| ADC | Neutral | 88 | 4.91% |
| ADC | Enemy | 99 | 5.52% |
| ADC | Insufficient | 1,469 | 81.88% |
| SUPPORT | Ally | 166 | 5.51% |
| SUPPORT | Neutral | 98 | 3.25% |
| SUPPORT | Enemy | 221 | 7.33% |
| SUPPORT | Insufficient | 2,528 | 83.90% |

The same categorical mechanics are technically possible, but the aggregate file cannot validate whether five independent level-timing tendencies form a meaningful team-level tempo measure. Keep Level Tempo on role cards for now.

## Combat Pressure Summary Evaluation

| Role | State | Count | Percent |
|---|---|---:|---:|
| TOP | Ally | 296 | 6.37% |
| TOP | Neutral | 78 | 1.68% |
| TOP | Enemy | 284 | 6.11% |
| TOP | Insufficient | 3,988 | 85.84% |
| JUNGLE | Ally | 140 | 6.08% |
| JUNGLE | Neutral | 317 | 13.77% |
| JUNGLE | Enemy | 127 | 5.52% |
| JUNGLE | Insufficient | 1,718 | 74.63% |
| MID | Ally | 258 | 6.80% |
| MID | Neutral | 134 | 3.53% |
| MID | Enemy | 215 | 5.67% |
| MID | Insufficient | 3,185 | 83.99% |
| ADC | Ally | 78 | 4.35% |
| ADC | Neutral | 160 | 8.92% |
| ADC | Enemy | 87 | 4.85% |
| ADC | Insufficient | 1,469 | 81.88% |
| SUPPORT | Ally | 32 | 1.06% |
| SUPPORT | Neutral | 434 | 14.40% |
| SUPPORT | Enemy | 19 | 0.63% |
| SUPPORT | Insufficient | 2,528 | 83.90% |

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
