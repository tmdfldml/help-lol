# Outcome Advantage Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 31,075
- Observations: 155,375
- Matchup keys: 13,021
- Scope: `MASTER_PLUS_SEEDED` / `SEED_PLAYER_TIER_AT_COLLECTION`
- All participants tier verified: `false`
- Queue: 420; minimum duration: 900s
- Dataset revision: 1788710852

## Research Goal

Outcome Advantage describes a final-game win-rate tendency, not a guaranteed lane or game result.

First-level and solo-kill metrics are excluded from every outcome model. They appear only in a later exploratory direction diagnostic.

## Model Definitions

- Model A: minimum sample plus an absolute raw win-rate deviation from 50%.
- Model B: Wilson interval excludes 50%, with a practical minimum-sample option.
- Model C: minimum sample, practical win-rate effect, and Wilson evidence must all pass.
- Model D: Stage 9-1 reliability bands choose different raw practical effects; Poor is always insufficient.

## Parameter Sweep

### Model A

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n5_e2_5 | 2,023 | 2,048 | 818 | 8,132 | 37.55% | 31.26% | 67.96% | 12.0 | 10.00% | 2,737 |
| A_n5_e5 | 1,733 | 1,720 | 1,436 | 8,132 | 37.55% | 26.52% | 44.16% | 10.0 | 12.50% | 2,507 |
| A_n5_e7_5 | 1,370 | 1,357 | 2,162 | 8,132 | 37.55% | 20.94% | 29.02% | 10.0 | 15.38% | 2,109 |
| A_n5_e10 | 1,222 | 1,191 | 2,476 | 8,132 | 37.55% | 18.53% | 20.85% | 9.0 | 16.67% | 1,990 |
| A_n10_e2_5 | 1,193 | 1,268 | 596 | 9,964 | 23.48% | 18.90% | 61.16% | 21.0 | 8.33% | 1,127 |
| A_n10_e5 | 903 | 940 | 1,214 | 9,964 | 23.48% | 14.15% | 37.37% | 20.0 | 10.67% | 897 |
| A_n10_e7_5 | 683 | 721 | 1,653 | 9,964 | 23.48% | 10.78% | 23.65% | 18.0 | 13.16% | 786 |
| A_n10_e10 | 535 | 555 | 1,967 | 9,964 | 23.48% | 8.37% | 15.49% | 17.0 | 14.71% | 667 |
| A_n20_e2_5 | 625 | 709 | 451 | 11,236 | 13.71% | 10.24% | 51.12% | 36.0 | 7.14% | 0 |
| A_n20_e5 | 452 | 494 | 839 | 11,236 | 13.71% | 7.27% | 29.37% | 33.0 | 9.09% | 0 |
| A_n20_e7_5 | 295 | 323 | 1,167 | 11,236 | 13.71% | 4.75% | 16.78% | 31.0 | 11.11% | 0 |
| A_n20_e10 | 210 | 213 | 1,362 | 11,236 | 13.71% | 3.25% | 9.73% | 29.0 | 13.41% | 0 |
| A_n30_e2_5 | 422 | 458 | 310 | 11,831 | 9.14% | 6.76% | 44.12% | 51.0 | 6.25% | 0 |
| A_n30_e5 | 276 | 283 | 631 | 11,831 | 9.14% | 4.29% | 23.42% | 47.0 | 8.33% | 0 |
| A_n30_e7_5 | 169 | 176 | 845 | 11,831 | 9.14% | 2.65% | 12.57% | 43.0 | 10.53% | 0 |
| A_n30_e10 | 109 | 97 | 984 | 11,831 | 9.14% | 1.58% | 6.43% | 41.0 | 12.73% | 0 |
| A_n50_e2_5 | 211 | 245 | 191 | 12,374 | 4.97% | 3.50% | 33.90% | 77.5 | 5.52% | 0 |
| A_n50_e5 | 123 | 134 | 390 | 12,374 | 4.97% | 1.97% | 16.12% | 73.0 | 7.65% | 0 |
| A_n50_e7_5 | 70 | 66 | 511 | 12,374 | 4.97% | 1.04% | 7.57% | 70.5 | 10.00% | 0 |
| A_n50_e10 | 41 | 30 | 576 | 12,374 | 4.97% | 0.55% | 3.19% | 66.0 | 12.07% | 0 |
| A_n75_e2_5 | 103 | 134 | 140 | 12,644 | 2.90% | 1.82% | 25.35% | 115.0 | 4.95% | 0 |
| A_n75_e5 | 54 | 63 | 260 | 12,644 | 2.90% | 0.90% | 10.61% | 104.0 | 7.14% | 0 |
| A_n75_e7_5 | 26 | 27 | 324 | 12,644 | 2.90% | 0.41% | 4.35% | 100.0 | 9.26% | 0 |
| A_n75_e10 | 13 | 6 | 358 | 12,644 | 2.90% | 0.15% | 1.17% | 89.0 | 10.74% | 0 |
| A_n100_e2_5 | 62 | 90 | 93 | 12,776 | 1.88% | 1.17% | 20.66% | 146.0 | 4.63% | 0 |
| A_n100_e5 | 29 | 40 | 176 | 12,776 | 1.88% | 0.53% | 7.94% | 131.0 | 7.00% | 0 |
| A_n100_e7_5 | 10 | 17 | 218 | 12,776 | 1.88% | 0.21% | 2.92% | 118.0 | 9.00% | 0 |
| A_n100_e10 | 3 | 1 | 241 | 12,776 | 1.88% | 0.03% | 0.34% | 121.0 | 10.74% | 0 |

### Model B

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| B_w80_n1 | 1,218 | 1,223 | 10,580 | 0 | 100.00% | 18.75% | 29.38% | 4.0 | 50.00% | 2,009 |
| B_w80_n5 | 577 | 591 | 3,721 | 8,132 | 37.55% | 8.97% | 27.46% | 13.0 | 23.68% | 736 |
| B_w80_n10 | 330 | 361 | 2,366 | 9,964 | 23.48% | 5.31% | 25.54% | 25.0 | 16.67% | 259 |
| B_w80_n20 | 204 | 228 | 1,353 | 11,236 | 13.71% | 3.32% | 23.16% | 43.0 | 13.13% | 0 |
| B_w80_n30 | 143 | 153 | 894 | 11,831 | 9.14% | 2.27% | 21.06% | 61.0 | 10.87% | 0 |
| B_w80_n50 | 86 | 100 | 461 | 12,374 | 4.97% | 1.43% | 18.31% | 90.5 | 8.95% | 0 |
| B_w90_n1 | 423 | 424 | 12,174 | 0 | 100.00% | 6.50% | 17.15% | 7.0 | 37.50% | 604 |
| B_w90_n5 | 247 | 252 | 4,390 | 8,132 | 37.55% | 3.83% | 16.43% | 18.0 | 23.68% | 256 |
| B_w90_n10 | 166 | 188 | 2,703 | 9,964 | 23.48% | 2.72% | 15.76% | 29.0 | 18.83% | 111 |
| B_w90_n20 | 118 | 125 | 1,542 | 11,236 | 13.71% | 1.87% | 14.77% | 45.0 | 15.12% | 0 |
| B_w90_n30 | 83 | 89 | 1,018 | 11,831 | 9.14% | 1.32% | 13.70% | 66.0 | 13.07% | 0 |
| B_w90_n50 | 53 | 58 | 536 | 12,374 | 4.97% | 0.85% | 12.18% | 92.0 | 10.26% | 0 |
| B_w95_n1 | 175 | 161 | 12,685 | 0 | 100.00% | 2.58% | 9.63% | 11.0 | 36.19% | 212 |
| B_w95_n5 | 133 | 126 | 4,630 | 8,132 | 37.55% | 1.99% | 9.44% | 19.0 | 28.57% | 135 |
| B_w95_n10 | 88 | 92 | 2,877 | 9,964 | 23.48% | 1.38% | 9.10% | 29.0 | 21.71% | 56 |
| B_w95_n20 | 62 | 62 | 1,661 | 11,236 | 13.71% | 0.95% | 8.60% | 45.0 | 16.86% | 0 |
| B_w95_n30 | 41 | 44 | 1,105 | 11,831 | 9.14% | 0.65% | 8.01% | 69.0 | 14.91% | 0 |
| B_w95_n50 | 25 | 32 | 590 | 12,374 | 4.97% | 0.44% | 7.30% | 92.0 | 11.86% | 0 |

### Model C

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| C_w80_n10_e2_5 | 330 | 361 | 2,366 | 9,964 | 23.48% | 5.31% | 25.54% | 25.0 | 16.67% | 259 |
| C_w80_n10_e5 | 322 | 348 | 2,387 | 9,964 | 23.48% | 5.15% | 19.19% | 24.5 | 16.67% | 259 |
| C_w80_n10_e7_5 | 306 | 324 | 2,427 | 9,964 | 23.48% | 4.84% | 14.41% | 23.0 | 16.67% | 259 |
| C_w80_n20_e2_5 | 204 | 228 | 1,353 | 11,236 | 13.71% | 3.32% | 23.16% | 43.0 | 13.13% | 0 |
| C_w80_n20_e5 | 196 | 215 | 1,374 | 11,236 | 13.71% | 3.16% | 16.81% | 41.0 | 13.41% | 0 |
| C_w80_n20_e7_5 | 180 | 191 | 1,414 | 11,236 | 13.71% | 2.85% | 12.04% | 38.0 | 14.06% | 0 |
| C_w80_n30_e2_5 | 143 | 153 | 894 | 11,831 | 9.14% | 2.27% | 21.06% | 61.0 | 10.87% | 0 |
| C_w80_n30_e5 | 135 | 140 | 915 | 11,831 | 9.14% | 2.11% | 14.72% | 57.0 | 11.02% | 0 |
| C_w80_n30_e7_5 | 119 | 116 | 955 | 11,831 | 9.14% | 1.80% | 9.94% | 51.0 | 11.86% | 0 |
| C_w80_n50_e2_5 | 86 | 100 | 461 | 12,374 | 4.97% | 1.43% | 18.31% | 90.5 | 8.95% | 0 |
| C_w80_n50_e5 | 78 | 87 | 482 | 12,374 | 4.97% | 1.27% | 11.96% | 82.0 | 9.26% | 0 |
| C_w80_n50_e7_5 | 62 | 63 | 522 | 12,374 | 4.97% | 0.96% | 7.19% | 72.0 | 10.26% | 0 |
| C_w90_n10_e2_5 | 166 | 188 | 2,703 | 9,964 | 23.48% | 2.72% | 15.76% | 29.0 | 18.83% | 111 |
| C_w90_n10_e5 | 163 | 181 | 2,713 | 9,964 | 23.48% | 2.64% | 11.88% | 27.0 | 19.27% | 111 |
| C_w90_n10_e7_5 | 159 | 171 | 2,727 | 9,964 | 23.48% | 2.53% | 9.18% | 25.0 | 19.57% | 111 |
| C_w90_n20_e2_5 | 118 | 125 | 1,542 | 11,236 | 13.71% | 1.87% | 14.77% | 45.0 | 15.12% | 0 |
| C_w90_n20_e5 | 115 | 118 | 1,552 | 11,236 | 13.71% | 1.79% | 10.89% | 43.0 | 15.52% | 0 |
| C_w90_n20_e7_5 | 111 | 108 | 1,566 | 11,236 | 13.71% | 1.68% | 8.18% | 41.0 | 15.71% | 0 |
| C_w90_n30_e2_5 | 83 | 89 | 1,018 | 11,831 | 9.14% | 1.32% | 13.70% | 66.0 | 13.07% | 0 |
| C_w90_n30_e5 | 80 | 82 | 1,028 | 11,831 | 9.14% | 1.24% | 9.82% | 61.0 | 13.41% | 0 |
| C_w90_n30_e7_5 | 76 | 72 | 1,042 | 11,831 | 9.14% | 1.14% | 7.12% | 57.0 | 13.55% | 0 |
| C_w90_n50_e2_5 | 53 | 58 | 536 | 12,374 | 4.97% | 0.85% | 12.18% | 92.0 | 10.26% | 0 |
| C_w90_n50_e5 | 50 | 51 | 546 | 12,374 | 4.97% | 0.78% | 8.30% | 84.0 | 10.67% | 0 |
| C_w90_n50_e7_5 | 46 | 41 | 560 | 12,374 | 4.97% | 0.67% | 5.60% | 76.0 | 11.02% | 0 |
| C_w95_n10_e2_5 | 88 | 92 | 2,877 | 9,964 | 23.48% | 1.38% | 9.10% | 29.0 | 21.71% | 56 |
| C_w95_n10_e5 | 88 | 88 | 2,881 | 9,964 | 23.48% | 1.35% | 6.93% | 27.0 | 22.05% | 56 |
| C_w95_n10_e7_5 | 88 | 83 | 2,886 | 9,964 | 23.48% | 1.31% | 5.39% | 26.0 | 22.41% | 56 |
| C_w95_n20_e2_5 | 62 | 62 | 1,661 | 11,236 | 13.71% | 0.95% | 8.60% | 45.0 | 16.86% | 0 |
| C_w95_n20_e5 | 62 | 58 | 1,665 | 11,236 | 13.71% | 0.92% | 6.43% | 43.5 | 17.31% | 0 |
| C_w95_n20_e7_5 | 62 | 53 | 1,670 | 11,236 | 13.71% | 0.88% | 4.89% | 43.0 | 17.65% | 0 |
| C_w95_n30_e2_5 | 41 | 44 | 1,105 | 11,831 | 9.14% | 0.65% | 8.01% | 69.0 | 14.91% | 0 |
| C_w95_n30_e5 | 41 | 40 | 1,109 | 11,831 | 9.14% | 0.62% | 5.83% | 66.0 | 15.12% | 0 |
| C_w95_n30_e7_5 | 41 | 35 | 1,114 | 11,831 | 9.14% | 0.58% | 4.30% | 66.0 | 15.13% | 0 |
| C_w95_n50_e2_5 | 25 | 32 | 590 | 12,374 | 4.97% | 0.44% | 7.30% | 92.0 | 11.86% | 0 |
| C_w95_n50_e5 | 25 | 28 | 594 | 12,374 | 4.97% | 0.41% | 5.13% | 89.0 | 12.50% | 0 |
| C_w95_n50_e7_5 | 25 | 23 | 599 | 12,374 | 4.97% | 0.37% | 3.59% | 84.5 | 13.20% | 0 |

### Reliability-aware Model D

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| D_Conservative | 933 | 905 | 3,051 | 8,132 | 37.55% | 14.12% | 29.43% | 10.0 | 18.18% | 1,229 |
| D_Balanced | 1,119 | 1,109 | 2,661 | 8,132 | 37.55% | 17.11% | 34.02% | 11.0 | 16.67% | 1,489 |
| D_Permissive | 1,552 | 1,583 | 1,754 | 8,132 | 37.55% | 24.08% | 58.27% | 12.0 | 13.16% | 1,990 |

## Coverage Comparison

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_e5 | 452 | 839 | 494 | 11,236 | 7.27% | 29.37% | 33.0 | 9.09% | 47.78% | True |
| C_w80_n10_e5 | 322 | 2,387 | 348 | 9,964 | 5.15% | 19.19% | 24.5 | 16.67% | 48.06% | True |
| C_w80_n20_e5 | 196 | 1,374 | 215 | 11,236 | 3.16% | 16.81% | 41.0 | 13.41% | 47.69% | True |
| C_w90_n20_e5 | 115 | 1,552 | 118 | 11,236 | 1.79% | 10.89% | 43.0 | 15.52% | 49.36% | True |
| D_Balanced | 1,119 | 2,661 | 1,109 | 8,132 | 17.11% | 34.02% | 11.0 | 16.67% | 50.22% | True |

## Reliability Group Comparison

| Group | Total | A | Neutral | B | Insufficient | Directional | Neutral rate | Insufficient rate |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Poor | 8,132 | 0 | 0 | 0 | 8,132 | 0.00% | 0.00% | 100.00% |
| Reference | 3,104 | 0 | 0 | 0 | 3,104 | 0.00% | 0.00% | 100.00% |
| Moderate | 1,138 | 118 | 892 | 128 | 0 | 21.62% | 78.38% | 0.00% |
| RelativelyStrong | 647 | 78 | 482 | 87 | 0 | 25.50% | 74.50% | 0.00% |

## Role Comparison

| Group | Total | A | Neutral | B | Insufficient | Directional | Neutral rate | Insufficient rate |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| TOP | 3,853 | 37 | 317 | 45 | 3,454 | 2.13% | 8.23% | 89.64% |
| JUNGLE | 2,025 | 31 | 319 | 41 | 1,634 | 3.56% | 15.75% | 80.69% |
| MID | 3,152 | 53 | 306 | 64 | 2,729 | 3.71% | 9.71% | 86.58% |
| ADC | 1,485 | 20 | 192 | 27 | 1,246 | 3.16% | 12.93% | 83.91% |
| SUPPORT | 2,506 | 55 | 240 | 38 | 2,173 | 3.71% | 9.58% | 86.71% |

## Effect Size Analysis

Recommended directional effects: min 5.11%, median 13.41%, p75 16.67%, p90 20.00%, max 34.62%.
Recommended directional samples: min 20, p25 26.0, median 41.0, p75 72.0, p90 118.0, max 1,083.

## Small Sample Risk

| Model | Directional n<5 | Directional n<10 | Directional n<20 | 0/100% at n=1-4 classified |
|---|---:|---:|---:|---:|
| A_n20_e5 | 0 | 0 | 0 | 0 |
| C_w80_n10_e5 | 0 | 0 | 259 | 0 |
| C_w80_n20_e5 | 0 | 0 | 0 | 0 |
| C_w90_n20_e5 | 0 | 0 | 0 | 0 |
| D_Balanced | 0 | 984 | 1,489 | 0 |

Recommended-model treatment of observed extreme win rates:

| Group | Total | A | Neutral | B | Insufficient |
|---|---:|---:|---:|---:|---:|
| WinRate0 | 3,005 | 0 | 0 | 0 | 3,005 |
| WinRate100 | 2,922 | 0 | 0 | 0 | 2,922 |
| WinRateAtMost30 | 3,565 | 0 | 0 | 24 | 3,541 |
| WinRateAtLeast70 | 3,485 | 25 | 0 | 0 | 3,460 |
| Sample1To4WinRate0Or100 | 5,873 | 0 | 0 | 0 | 5,873 |

## Wilson Confidence Comparison

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| B_w80_n20 | 204 | 1,353 | 228 | 11,236 | 3.32% | 23.16% | 43.0 | 13.13% | 47.22% | True |
| B_w90_n20 | 118 | 1,542 | 125 | 11,236 | 1.87% | 14.77% | 45.0 | 15.12% | 48.56% | True |
| B_w95_n20 | 62 | 1,661 | 62 | 11,236 | 0.95% | 8.60% | 45.0 | 16.86% | 50.00% | True |

A higher confidence level reduces directional coverage. This report does not treat 95% as automatically optimal for a user-facing reference tool.

## Borderline Examples

```json
[
  {
    "ModelId": "A_n20_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Cassiopeia",
        "ChampionB": "Jinx",
        "SampleSize": 20,
        "WinRateA": 0.45,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.2581978582556243,
        "Wilson95Upper": 0.6579146575496577,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Jayce",
        "ChampionB": "Kayn",
        "SampleSize": 20,
        "WinRateA": 0.45,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.2581978582556243,
        "Wilson95Upper": 0.6579146575496577,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Nocturne",
        "ChampionB": "RekSai",
        "SampleSize": 20,
        "WinRateA": 0.45,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.2581978582556243,
        "Wilson95Upper": 0.6579146575496577,
        "Result": "SideBAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "MID",
        "ChampionA": "Syndra",
        "ChampionB": "TwistedFate",
        "SampleSize": 171,
        "WinRateA": 0.4502923976608187,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.37763034161599474,
        "Wilson95Upper": 0.5251387149071832,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Zeri",
        "SampleSize": 384,
        "WinRateA": 0.4505208333333333,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4014915188454842,
        "Wilson95Upper": 0.5005303018346816,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Qiyana",
        "ChampionB": "XinZhao",
        "SampleSize": 71,
        "WinRateA": 0.5492957746478874,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4340106587147449,
        "Wilson95Upper": 0.6595203882517588,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "C_w80_n10_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "MID",
        "ChampionA": "Viktor",
        "ChampionB": "Yone",
        "SampleSize": 176,
        "WinRateA": 0.5511363636363636,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4773433181311621,
        "Wilson95Upper": 0.6227448379482791,
        "Result": "SideAAdvantage"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "LeeSin",
        "ChampionB": "Qiyana",
        "SampleSize": 459,
        "WinRateA": 0.44880174291938996,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4039126907395451,
        "Wilson95Upper": 0.4945406584488475,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "TOP",
        "ChampionA": "Jayce",
        "ChampionB": "Malphite",
        "SampleSize": 214,
        "WinRateA": 0.4485981308411215,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.383453231364216,
        "Wilson95Upper": 0.51555589151304,
        "Result": "SideBAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Lucian",
        "ChampionB": "Twitch",
        "SampleSize": 10,
        "WinRateA": 0.3,
        "Reliability": "Reference",
        "Wilson95Lower": 0.10779126740630099,
        "Wilson95Upper": 0.6032218525388546,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Viktor",
        "ChampionB": "Xerath",
        "SampleSize": 10,
        "WinRateA": 0.3,
        "Reliability": "Reference",
        "Wilson95Lower": 0.10779126740630099,
        "Wilson95Upper": 0.6032218525388546,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Ambessa",
        "ChampionB": "Naafiri",
        "SampleSize": 10,
        "WinRateA": 0.3,
        "Reliability": "Reference",
        "Wilson95Lower": 0.10779126740630099,
        "Wilson95Upper": 0.6032218525388546,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "C_w80_n20_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "MID",
        "ChampionA": "Viktor",
        "ChampionB": "Yone",
        "SampleSize": 176,
        "WinRateA": 0.5511363636363636,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4773433181311621,
        "Wilson95Upper": 0.6227448379482791,
        "Result": "SideAAdvantage"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "LeeSin",
        "ChampionB": "Qiyana",
        "SampleSize": 459,
        "WinRateA": 0.44880174291938996,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4039126907395451,
        "Wilson95Upper": 0.4945406584488475,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "TOP",
        "ChampionA": "Jayce",
        "ChampionB": "Malphite",
        "SampleSize": 214,
        "WinRateA": 0.4485981308411215,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.383453231364216,
        "Wilson95Upper": 0.51555589151304,
        "Result": "SideBAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Khazix",
        "ChampionB": "RekSai",
        "SampleSize": 22,
        "WinRateA": 0.36363636363636365,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.19733204670366122,
        "Wilson95Upper": 0.5704829190986899,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Kindred",
        "ChampionB": "XinZhao",
        "SampleSize": 22,
        "WinRateA": 0.36363636363636365,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.19733204670366122,
        "Wilson95Upper": 0.5704829190986899,
        "Result": "Neutral"
      },
      {
        "Role": "MID",
        "ChampionA": "Ekko",
        "ChampionB": "Locke",
        "SampleSize": 22,
        "WinRateA": 0.6363636363636364,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.4295170809013101,
        "Wilson95Upper": 0.8026679532963388,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "C_w90_n20_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "JUNGLE",
        "ChampionA": "LeeSin",
        "ChampionB": "Qiyana",
        "SampleSize": 459,
        "WinRateA": 0.44880174291938996,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4039126907395451,
        "Wilson95Upper": 0.4945406584488475,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Ezreal",
        "ChampionB": "Jinx",
        "SampleSize": 232,
        "WinRateA": 0.44396551724137934,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3814608173739649,
        "Wilson95Upper": 0.5082956311740942,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Leona",
        "ChampionB": "Nautilus",
        "SampleSize": 274,
        "WinRateA": 0.5583941605839416,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4991913607496071,
        "Wilson95Upper": 0.6159822355013805,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "MID",
        "ChampionA": "Irelia",
        "ChampionB": "Talon",
        "SampleSize": 21,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.17194752605083477,
        "Wilson95Upper": 0.5462654802574476,
        "Result": "Neutral"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Elise",
        "ChampionB": "Karma",
        "SampleSize": 21,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.17194752605083477,
        "Wilson95Upper": 0.5462654802574476,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Briar",
        "ChampionB": "Nidalee",
        "SampleSize": 24,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.1797219032729775,
        "Wilson95Upper": 0.5329368316186824,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "D_Balanced",
    "DirectionalSmallestEffects": [
      {
        "Role": "JUNGLE",
        "ChampionA": "JarvanIV",
        "ChampionB": "Sylas",
        "SampleSize": 80,
        "WinRateA": 0.45,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.34577689184711224,
        "Wilson95Upper": 0.5588049211064404,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Hecarim",
        "ChampionB": "Talon",
        "SampleSize": 80,
        "WinRateA": 0.55,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.44119507889355963,
        "Wilson95Upper": 0.6542231081528878,
        "Result": "SideAAdvantage"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Alistar",
        "ChampionB": "Bard",
        "SampleSize": 100,
        "WinRateA": 0.55,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.45244602997442135,
        "Wilson95Upper": 0.6438546202048803,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Corki",
        "ChampionB": "Xerath",
        "SampleSize": 13,
        "WinRateA": 0.6153846153846154,
        "Reliability": "Reference",
        "Wilson95Lower": 0.35522891512665694,
        "Wilson95Upper": 0.8229029220223743,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Yasuo",
        "ChampionB": "Ziggs",
        "SampleSize": 13,
        "WinRateA": 0.6153846153846154,
        "Reliability": "Reference",
        "Wilson95Lower": 0.35522891512665694,
        "Wilson95Upper": 0.8229029220223743,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Ekko",
        "ChampionB": "Evelynn",
        "SampleSize": 13,
        "WinRateA": 0.6153846153846154,
        "Reliability": "Reference",
        "Wilson95Lower": 0.35522891512665694,
        "Wilson95Upper": 0.8229029220223743,
        "Result": "Neutral"
      }
    ]
  }
]
```

## Model Disagreement Examples

```json
[
  {
    "FirstModelId": "A_n20_e5",
    "SecondModelId": "C_w90_n20_e5",
    "Examples": [
      {
        "FirstModelResult": {
          "Role": "TOP",
          "ChampionA": "Jayce",
          "ChampionB": "Malphite",
          "SampleSize": 214,
          "WinRateA": 0.4485981308411215,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.383453231364216,
          "Wilson95Upper": 0.51555589151304,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Caitlyn",
          "ChampionB": "Jinx",
          "SampleSize": 194,
          "WinRateA": 0.44329896907216493,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.375168426071244,
          "Wilson95Upper": 0.5136314234085415,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Viktor",
          "ChampionB": "Yone",
          "SampleSize": 176,
          "WinRateA": 0.5511363636363636,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4773433181311621,
          "Wilson95Upper": 0.6227448379482791,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "Graves",
          "ChampionB": "Nidalee",
          "SampleSize": 173,
          "WinRateA": 0.44508670520231214,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.3730226707972324,
          "Wilson95Upper": 0.5195364602645353,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Ahri",
          "ChampionB": "Sylas",
          "SampleSize": 156,
          "WinRateA": 0.44871794871794873,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.37283653745202533,
          "Wilson95Upper": 0.5270642760287351,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Alistar",
          "ChampionB": "Blitzcrank",
          "SampleSize": 150,
          "WinRateA": 0.5666666666666667,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48667987062755025,
          "Wilson95Upper": 0.6433240967900394,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "Nidalee",
          "ChampionB": "Talon",
          "SampleSize": 140,
          "WinRateA": 0.5571428571428572,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4744276571463636,
          "Wilson95Upper": 0.6368059199542749,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Galio",
          "ChampionB": "Viktor",
          "SampleSize": 139,
          "WinRateA": 0.43884892086330934,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.3590967234423357,
          "Wilson95Upper": 0.5218902103207831,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "Hecarim",
          "ChampionB": "Nidalee",
          "SampleSize": 135,
          "WinRateA": 0.4444444444444444,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.3633138095245094,
          "Wilson95Upper": 0.5286492962552155,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Locke",
          "ChampionB": "Syndra",
          "SampleSize": 131,
          "WinRateA": 0.5572519083969466,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4717670903549375,
          "Wilson95Upper": 0.6394746607443763,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      }
    ]
  },
  {
    "FirstModelId": "B_w80_n1",
    "SecondModelId": "B_w95_n1",
    "Examples": [
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "LeeSin",
          "ChampionB": "Vi",
          "SampleSize": 622,
          "WinRateA": 0.4646302250803859,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.42577194069692037,
          "Wilson95Upper": 0.5039227137880805,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Jhin",
          "ChampionB": "Yunara",
          "SampleSize": 568,
          "WinRateA": 0.47007042253521125,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4293636864623301,
          "Wilson95Upper": 0.5111792743958805,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Kaisa",
          "ChampionB": "Lucian",
          "SampleSize": 517,
          "WinRateA": 0.4700193423597679,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.427376831880019,
          "Wilson95Upper": 0.5131040966731896,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Aphelios",
          "ChampionB": "Kaisa",
          "SampleSize": 460,
          "WinRateA": 0.5391304347826087,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.49344246857695917,
          "Wilson95Upper": 0.584170257313325,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Lulu",
          "ChampionB": "Nautilus",
          "SampleSize": 425,
          "WinRateA": 0.5411764705882353,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.49364420229243694,
          "Wilson95Upper": 0.5879710410104126,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Nautilus",
          "ChampionB": "Thresh",
          "SampleSize": 405,
          "WinRateA": 0.4666666666666667,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4186202888775374,
          "Wilson95Upper": 0.5153394419240263,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Jinx",
          "ChampionB": "Kaisa",
          "SampleSize": 399,
          "WinRateA": 0.5463659147869674,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4973061632623616,
          "Wilson95Upper": 0.5945413841780541,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Kaisa",
          "ChampionB": "Zeri",
          "SampleSize": 384,
          "WinRateA": 0.4505208333333333,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4014915188454842,
          "Wilson95Upper": 0.5005303018346816,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Kaisa",
          "ChampionB": "Tristana",
          "SampleSize": 371,
          "WinRateA": 0.5336927223719676,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48284454903779694,
          "Wilson95Upper": 0.5838503146484496,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Yunara",
          "ChampionB": "Zeri",
          "SampleSize": 351,
          "WinRateA": 0.452991452991453,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4017046328516219,
          "Wilson95Upper": 0.505296087668256,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      }
    ]
  }
]
```

## Strong Directional Examples

A-side:

| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |
|---|---|---:|---:|---|---:|---|
| SUPPORT | Thresh vs Yuumi | 74 | 68.92% | RelativelyStrong | 57.66%–78.31% | SideAAdvantage |
| SUPPORT | Leona vs Pantheon | 57 | 68.42% | RelativelyStrong | 55.52%–79.00% | SideAAdvantage |
| JUNGLE | LeeSin vs Nocturne | 85 | 67.06% | RelativelyStrong | 56.52%–76.12% | SideAAdvantage |
| MID | Qiyana vs Sylas | 66 | 66.67% | RelativelyStrong | 54.66%–76.84% | SideAAdvantage |
| SUPPORT | Alistar vs Sylas | 51 | 66.67% | RelativelyStrong | 52.97%–78.03% | SideAAdvantage |
| MID | Katarina vs Locke | 74 | 66.22% | RelativelyStrong | 54.88%–75.95% | SideAAdvantage |
| MID | Ahri vs Yasuo | 57 | 64.91% | RelativelyStrong | 51.94%–76.00% | SideAAdvantage |
| ADC | Draven vs Yunara | 54 | 64.81% | RelativelyStrong | 51.48%–76.18% | SideAAdvantage |
| TOP | Fiora vs KSante | 92 | 64.13% | RelativelyStrong | 53.95%–73.18% | SideAAdvantage |
| SUPPORT | Braum vs Pyke | 64 | 64.06% | RelativelyStrong | 51.82%–74.71% | SideAAdvantage |

B-side:

| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |
|---|---|---:|---:|---|---:|---|
| ADC | Lucian vs Xayah | 88 | 30.68% | RelativelyStrong | 22.02%–40.96% | SideBAdvantage |
| SUPPORT | Milio vs Thresh | 69 | 31.88% | RelativelyStrong | 22.09%–43.58% | SideBAdvantage |
| MID | Leblanc vs Vex | 73 | 32.88% | RelativelyStrong | 23.19%–44.27% | SideBAdvantage |
| MID | Akali vs Yasuo | 74 | 33.78% | RelativelyStrong | 24.05%–45.12% | SideBAdvantage |
| MID | Taliyah vs TwistedFate | 50 | 34.00% | RelativelyStrong | 22.44%–47.85% | SideBAdvantage |
| JUNGLE | Graves vs Kindred | 97 | 34.02% | RelativelyStrong | 25.36%–43.90% | SideBAdvantage |
| TOP | Irelia vs Jax | 58 | 34.48% | RelativelyStrong | 23.56%–47.33% | SideBAdvantage |
| JUNGLE | MonkeyKing vs Qiyana | 66 | 34.85% | RelativelyStrong | 24.48%–46.89% | SideBAdvantage |
| JUNGLE | Jayce vs Nidalee | 100 | 35.00% | RelativelyStrong | 26.36%–44.75% | SideBAdvantage |
| JUNGLE | Graves vs RekSai | 51 | 35.29% | RelativelyStrong | 23.63%–49.01% | SideBAdvantage |

## Neutral Examples

| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |
|---|---|---:|---:|---|---:|---|
| TOP | Ambessa vs Renekton | 262 | 50.00% | RelativelyStrong | 43.99%–56.01% | Neutral |
| MID | TwistedFate vs Viktor | 158 | 50.00% | RelativelyStrong | 42.30%–57.70% | Neutral |
| SUPPORT | Leona vs Rell | 152 | 50.00% | RelativelyStrong | 42.15%–57.85% | Neutral |
| JUNGLE | Hecarim vs Qiyana | 140 | 50.00% | RelativelyStrong | 41.83%–58.17% | Neutral |
| JUNGLE | Nidalee vs Vi | 128 | 50.00% | RelativelyStrong | 41.47%–58.53% | Neutral |
| ADC | Jhin vs MissFortune | 120 | 50.00% | RelativelyStrong | 41.19%–58.81% | Neutral |
| JUNGLE | Graves vs Hecarim | 118 | 50.00% | RelativelyStrong | 41.12%–58.88% | Neutral |
| JUNGLE | LeeSin vs Lillia | 112 | 50.00% | RelativelyStrong | 40.89%–59.11% | Neutral |
| ADC | Ezreal vs Xayah | 108 | 50.00% | RelativelyStrong | 40.73%–59.27% | Neutral |
| ADC | Jhin vs Viktor | 108 | 50.00% | RelativelyStrong | 40.73%–59.27% | Neutral |

## Lane Metric Direction Diagnostic

Exploratory only; these metrics are not inputs to the outcome rule.

| Metric | Same direction | Opposite | Neutral | Agreement among non-neutral |
|---|---:|---:|---:|---:|
| FirstLevel2 | 203 | 199 | 9 | 50.50% |
| FirstLevel3 | 209 | 192 | 10 | 52.12% |
| FirstLevel6 | 225 | 167 | 19 | 57.40% |
| SoloKillDifference | 237 | 137 | 37 | 63.37% |

## Candidate Shortlist

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_e5 | 452 | 839 | 494 | 11,236 | 7.27% | 29.37% | 33.0 | 9.09% | 47.78% | True |
| C_w80_n10_e5 | 322 | 2,387 | 348 | 9,964 | 5.15% | 19.19% | 24.5 | 16.67% | 48.06% | True |
| C_w80_n20_e5 | 196 | 1,374 | 215 | 11,236 | 3.16% | 16.81% | 41.0 | 13.41% | 47.69% | True |
| C_w90_n20_e5 | 115 | 1,552 | 118 | 11,236 | 1.79% | 10.89% | 43.0 | 15.52% | 49.36% | True |
| D_Balanced | 1,119 | 2,661 | 1,109 | 8,132 | 17.11% | 34.02% | 11.0 | 16.67% | 50.22% | True |

## Recommended Production Rule

- Model: `C_w80_n20_e5`
- Rule: Minimum sample 20; |win rate - 50%| at least 5 percentage points; 80% Wilson interval must exclude 50%; no reliability-specific exception.
- Suggested wording: 우세 경향 / 뚜렷한 차이 없음 / 열세 경향
- Directional key coverage: 3.16% (411 keys)
- Directional sample-weighted coverage: 16.81%
- Directional median sample/effect: 41.0 / 13.41%
- Directional classifications below n=20: 0
- Protects all n<20 matchups from an outcome judgment.
- Requires both statistical evidence and a user-visible practical effect.
- Keeps one symmetric, explainable rule across every role.
- Uses 80% rather than 95% evidence to avoid making the current sparse dataset almost entirely unclassified.

## Limitations

- This is an in-sample coverage and uncertainty study, not a future-match prediction accuracy test.
- MASTER_PLUS_SEEDED means seed-player tier at collection; all ten participants were not tier-verified.
- Results apply only to the loaded patch and duration policy.
- Key-weighted and sample-weighted coverage are descriptive proxies, not literal user exposure probability.
- Correlation or directional agreement with lane metrics does not establish causation.

## Next Stage Recommendation

Stage 9-2B should implement the recommended symmetric rule as a separate production outcome classifier while preserving Stage 9-1 reliability, raw statistics, Neutral, and InsufficientEvidence as distinct concepts. No production classifier or UI was changed by this research.
