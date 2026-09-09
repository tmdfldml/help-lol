# Outcome Advantage Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 51,649
- Observations: 258,245
- Matchup keys: 15,547
- Scope: `MASTER_PLUS_SEEDED` / `SEED_PLAYER_TIER_AT_COLLECTION`
- All participants tier verified: `false`
- Queue: 420; minimum duration: 900s
- Dataset revision: 1788854099077

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
| A_n5_e2_5 | 2,613 | 2,578 | 1,123 | 9,233 | 40.61% | 33.39% | 66.64% | 13.0 | 10.00% | 3,231 |
| A_n5_e5 | 2,200 | 2,119 | 1,995 | 9,233 | 40.61% | 27.78% | 42.25% | 12.0 | 11.54% | 2,932 |
| A_n5_e7_5 | 1,704 | 1,665 | 2,945 | 9,233 | 40.61% | 21.67% | 25.30% | 10.0 | 14.29% | 2,463 |
| A_n5_e10 | 1,506 | 1,430 | 3,378 | 9,233 | 40.61% | 18.88% | 17.21% | 9.0 | 16.67% | 2,313 |
| A_n10_e2_5 | 1,656 | 1,703 | 889 | 11,299 | 27.32% | 21.61% | 61.94% | 24.0 | 8.33% | 1,399 |
| A_n10_e5 | 1,243 | 1,244 | 1,761 | 11,299 | 27.32% | 16.00% | 37.55% | 22.0 | 10.00% | 1,100 |
| A_n10_e7_5 | 920 | 951 | 2,377 | 11,299 | 27.32% | 12.03% | 21.61% | 19.0 | 12.07% | 965 |
| A_n10_e10 | 722 | 716 | 2,810 | 11,299 | 27.32% | 9.25% | 13.52% | 17.0 | 13.93% | 815 |
| A_n20_e2_5 | 963 | 997 | 699 | 12,888 | 17.10% | 12.61% | 54.53% | 41.0 | 7.14% | 0 |
| A_n20_e5 | 687 | 700 | 1,272 | 12,888 | 17.10% | 8.92% | 31.73% | 38.0 | 9.09% | 0 |
| A_n20_e7_5 | 433 | 473 | 1,753 | 12,888 | 17.10% | 5.83% | 16.61% | 34.0 | 11.29% | 0 |
| A_n20_e10 | 303 | 320 | 2,036 | 12,888 | 17.10% | 4.01% | 9.37% | 31.0 | 13.33% | 0 |
| A_n30_e2_5 | 685 | 728 | 520 | 13,614 | 12.43% | 9.09% | 49.44% | 54.0 | 6.31% | 0 |
| A_n30_e5 | 441 | 474 | 1,018 | 13,614 | 12.43% | 5.89% | 27.34% | 50.0 | 8.33% | 0 |
| A_n30_e7_5 | 260 | 301 | 1,372 | 13,614 | 12.43% | 3.61% | 13.40% | 45.0 | 10.61% | 0 |
| A_n30_e10 | 163 | 182 | 1,588 | 13,614 | 12.43% | 2.22% | 6.83% | 42.0 | 12.69% | 0 |
| A_n50_e2_5 | 387 | 399 | 384 | 14,377 | 7.53% | 5.06% | 40.23% | 88.0 | 5.75% | 0 |
| A_n50_e5 | 235 | 234 | 701 | 14,377 | 7.53% | 3.02% | 20.76% | 78.0 | 7.63% | 0 |
| A_n50_e7_5 | 113 | 128 | 929 | 14,377 | 7.53% | 1.55% | 8.76% | 73.0 | 10.00% | 0 |
| A_n50_e10 | 64 | 64 | 1,042 | 14,377 | 7.53% | 0.82% | 3.71% | 63.0 | 12.19% | 0 |
| A_n75_e2_5 | 239 | 241 | 250 | 14,817 | 4.70% | 3.09% | 33.16% | 120.0 | 5.26% | 0 |
| A_n75_e5 | 129 | 126 | 475 | 14,817 | 4.70% | 1.64% | 15.85% | 113.0 | 7.28% | 0 |
| A_n75_e7_5 | 53 | 61 | 616 | 14,817 | 4.70% | 0.73% | 5.86% | 108.0 | 9.26% | 0 |
| A_n75_e10 | 24 | 21 | 685 | 14,817 | 4.70% | 0.29% | 1.84% | 100.0 | 11.36% | 0 |
| A_n100_e2_5 | 164 | 169 | 182 | 15,032 | 3.31% | 2.14% | 28.31% | 147.0 | 4.98% | 0 |
| A_n100_e5 | 84 | 82 | 349 | 15,032 | 3.31% | 1.07% | 12.91% | 139.0 | 7.01% | 0 |
| A_n100_e7_5 | 33 | 34 | 448 | 15,032 | 3.31% | 0.43% | 4.30% | 132.0 | 9.06% | 0 |
| A_n100_e10 | 12 | 11 | 492 | 15,032 | 3.31% | 0.15% | 1.11% | 128.0 | 12.00% | 0 |

### Model B

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| B_w80_n1 | 1,524 | 1,520 | 12,503 | 0 | 100.00% | 19.58% | 32.81% | 5.0 | 50.00% | 2,355 |
| B_w80_n5 | 769 | 768 | 4,777 | 9,233 | 40.61% | 9.89% | 31.44% | 16.0 | 21.43% | 848 |
| B_w80_n10 | 484 | 516 | 3,248 | 11,299 | 27.32% | 6.43% | 30.12% | 32.0 | 15.38% | 311 |
| B_w80_n20 | 322 | 367 | 1,970 | 12,888 | 17.10% | 4.43% | 28.43% | 49.0 | 12.07% | 0 |
| B_w80_n30 | 246 | 285 | 1,402 | 13,614 | 12.43% | 3.42% | 26.96% | 65.0 | 10.71% | 0 |
| B_w80_n50 | 158 | 185 | 827 | 14,377 | 7.53% | 2.21% | 24.16% | 108.0 | 8.70% | 0 |
| B_w90_n1 | 579 | 626 | 14,342 | 0 | 100.00% | 7.75% | 20.58% | 9.0 | 34.62% | 803 |
| B_w90_n5 | 369 | 388 | 5,557 | 9,233 | 40.61% | 4.87% | 20.03% | 21.0 | 20.83% | 355 |
| B_w90_n10 | 275 | 299 | 3,674 | 11,299 | 27.32% | 3.69% | 19.51% | 33.0 | 17.57% | 172 |
| B_w90_n20 | 188 | 214 | 2,257 | 12,888 | 17.10% | 2.59% | 18.58% | 53.0 | 14.00% | 0 |
| B_w90_n30 | 145 | 168 | 1,620 | 13,614 | 12.43% | 2.01% | 17.75% | 69.0 | 12.50% | 0 |
| B_w90_n50 | 96 | 119 | 955 | 14,377 | 7.53% | 1.38% | 16.31% | 104.0 | 10.00% | 0 |
| B_w95_n1 | 253 | 239 | 15,055 | 0 | 100.00% | 3.16% | 13.96% | 18.0 | 27.78% | 259 |
| B_w95_n5 | 207 | 197 | 5,910 | 9,233 | 40.61% | 2.60% | 13.83% | 26.0 | 22.00% | 171 |
| B_w95_n10 | 151 | 157 | 3,940 | 11,299 | 27.32% | 1.98% | 13.58% | 41.0 | 17.61% | 75 |
| B_w95_n20 | 115 | 118 | 2,426 | 12,888 | 17.10% | 1.50% | 13.16% | 57.0 | 14.58% | 0 |
| B_w95_n30 | 89 | 99 | 1,745 | 13,614 | 12.43% | 1.21% | 12.73% | 73.5 | 13.52% | 0 |
| B_w95_n50 | 60 | 72 | 1,038 | 14,377 | 7.53% | 0.85% | 11.90% | 123.0 | 10.93% | 0 |

### Model C

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| C_w80_n10_e2_5 | 484 | 515 | 3,249 | 11,299 | 27.32% | 6.43% | 29.72% | 32.0 | 15.38% | 311 |
| C_w80_n10_e5 | 469 | 491 | 3,288 | 11,299 | 27.32% | 6.17% | 22.16% | 30.0 | 15.71% | 311 |
| C_w80_n10_e7_5 | 430 | 448 | 3,370 | 11,299 | 27.32% | 5.65% | 14.30% | 27.5 | 16.67% | 311 |
| C_w80_n20_e2_5 | 322 | 366 | 1,971 | 12,888 | 17.10% | 4.43% | 28.03% | 49.0 | 12.12% | 0 |
| C_w80_n20_e5 | 307 | 342 | 2,010 | 12,888 | 17.10% | 4.17% | 20.47% | 47.0 | 12.50% | 0 |
| C_w80_n20_e7_5 | 268 | 299 | 2,092 | 12,888 | 17.10% | 3.65% | 12.61% | 41.0 | 13.53% | 0 |
| C_w80_n30_e2_5 | 246 | 284 | 1,403 | 13,614 | 12.43% | 3.41% | 26.56% | 65.0 | 10.74% | 0 |
| C_w80_n30_e5 | 231 | 260 | 1,442 | 13,614 | 12.43% | 3.16% | 18.99% | 60.0 | 11.11% | 0 |
| C_w80_n30_e7_5 | 192 | 217 | 1,524 | 13,614 | 12.43% | 2.63% | 11.13% | 51.0 | 11.82% | 0 |
| C_w80_n50_e2_5 | 158 | 184 | 828 | 14,377 | 7.53% | 2.20% | 23.76% | 107.0 | 8.71% | 0 |
| C_w80_n50_e5 | 143 | 160 | 867 | 14,377 | 7.53% | 1.95% | 16.20% | 97.0 | 9.15% | 0 |
| C_w80_n50_e7_5 | 104 | 117 | 949 | 14,377 | 7.53% | 1.42% | 8.34% | 76.0 | 10.29% | 0 |
| C_w90_n10_e2_5 | 275 | 299 | 3,674 | 11,299 | 27.32% | 3.69% | 19.51% | 33.0 | 17.57% | 172 |
| C_w90_n10_e5 | 273 | 287 | 3,688 | 11,299 | 27.32% | 3.60% | 15.49% | 32.0 | 17.74% | 172 |
| C_w90_n10_e7_5 | 260 | 264 | 3,724 | 11,299 | 27.32% | 3.37% | 10.06% | 30.0 | 18.18% | 172 |
| C_w90_n20_e2_5 | 188 | 214 | 2,257 | 12,888 | 17.10% | 2.59% | 18.58% | 53.0 | 14.00% | 0 |
| C_w90_n20_e5 | 186 | 202 | 2,271 | 12,888 | 17.10% | 2.50% | 14.56% | 50.5 | 14.15% | 0 |
| C_w90_n20_e7_5 | 173 | 179 | 2,307 | 12,888 | 17.10% | 2.26% | 9.13% | 46.0 | 14.71% | 0 |
| C_w90_n30_e2_5 | 145 | 168 | 1,620 | 13,614 | 12.43% | 2.01% | 17.75% | 69.0 | 12.50% | 0 |
| C_w90_n30_e5 | 143 | 156 | 1,634 | 13,614 | 12.43% | 1.92% | 13.72% | 66.0 | 12.90% | 0 |
| C_w90_n30_e7_5 | 130 | 133 | 1,670 | 13,614 | 12.43% | 1.69% | 8.30% | 58.0 | 13.64% | 0 |
| C_w90_n50_e2_5 | 96 | 119 | 955 | 14,377 | 7.53% | 1.38% | 16.31% | 104.0 | 10.00% | 0 |
| C_w90_n50_e5 | 94 | 107 | 969 | 14,377 | 7.53% | 1.29% | 12.29% | 99.0 | 10.53% | 0 |
| C_w90_n50_e7_5 | 81 | 84 | 1,005 | 14,377 | 7.53% | 1.06% | 6.86% | 86.0 | 11.19% | 0 |
| C_w95_n10_e2_5 | 151 | 157 | 3,940 | 11,299 | 27.32% | 1.98% | 13.58% | 41.0 | 17.61% | 75 |
| C_w95_n10_e5 | 150 | 150 | 3,948 | 11,299 | 27.32% | 1.93% | 10.83% | 39.0 | 17.74% | 75 |
| C_w95_n10_e7_5 | 146 | 134 | 3,968 | 11,299 | 27.32% | 1.80% | 6.63% | 34.5 | 18.97% | 75 |
| C_w95_n20_e2_5 | 115 | 118 | 2,426 | 12,888 | 17.10% | 1.50% | 13.16% | 57.0 | 14.58% | 0 |
| C_w95_n20_e5 | 114 | 111 | 2,434 | 12,888 | 17.10% | 1.45% | 10.41% | 56.0 | 14.71% | 0 |
| C_w95_n20_e7_5 | 110 | 95 | 2,454 | 12,888 | 17.10% | 1.32% | 6.21% | 50.0 | 15.62% | 0 |
| C_w95_n30_e2_5 | 89 | 99 | 1,745 | 13,614 | 12.43% | 1.21% | 12.73% | 73.5 | 13.52% | 0 |
| C_w95_n30_e5 | 88 | 92 | 1,753 | 13,614 | 12.43% | 1.16% | 9.98% | 68.5 | 13.78% | 0 |
| C_w95_n30_e7_5 | 84 | 76 | 1,773 | 13,614 | 12.43% | 1.03% | 5.78% | 61.5 | 14.12% | 0 |
| C_w95_n50_e2_5 | 60 | 72 | 1,038 | 14,377 | 7.53% | 0.85% | 11.90% | 123.0 | 10.93% | 0 |
| C_w95_n50_e5 | 59 | 65 | 1,046 | 14,377 | 7.53% | 0.80% | 9.15% | 115.5 | 11.35% | 0 |
| C_w95_n50_e7_5 | 55 | 49 | 1,066 | 14,377 | 7.53% | 0.67% | 4.95% | 93.5 | 12.86% | 0 |

### Reliability-aware Model D

| Model | A | B | Neutral | Insufficient | Eligible | Directional keys | Sample-weighted directional | Median n | Median effect | n<20 directional |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| D_Conservative | 1,212 | 1,147 | 3,955 | 9,233 | 40.61% | 15.17% | 31.03% | 12.0 | 16.67% | 1,395 |
| D_Balanced | 1,461 | 1,386 | 3,467 | 9,233 | 40.61% | 18.31% | 34.56% | 13.0 | 16.67% | 1,713 |
| D_Permissive | 2,042 | 1,975 | 2,297 | 9,233 | 40.61% | 25.84% | 59.03% | 14.0 | 12.50% | 2,313 |

## Coverage Comparison

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_e5 | 687 | 1,272 | 700 | 12,888 | 8.92% | 31.73% | 38.0 | 9.09% | 49.53% | True |
| C_w80_n10_e5 | 469 | 3,288 | 491 | 11,299 | 6.17% | 22.16% | 30.0 | 15.71% | 48.85% | True |
| C_w80_n20_e5 | 307 | 2,010 | 342 | 12,888 | 4.17% | 20.47% | 47.0 | 12.50% | 47.30% | True |
| C_w90_n20_e5 | 186 | 2,271 | 202 | 12,888 | 2.50% | 14.56% | 50.5 | 14.15% | 47.94% | True |
| D_Balanced | 1,461 | 3,467 | 1,386 | 9,233 | 18.31% | 34.56% | 13.0 | 16.67% | 51.32% | True |

## Reliability Group Comparison

| Group | Total | A | Neutral | B | Insufficient | Directional | Neutral rate | Insufficient rate |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Poor | 9,233 | 0 | 0 | 0 | 9,233 | 0.00% | 0.00% | 100.00% |
| Reference | 3,655 | 0 | 0 | 0 | 3,655 | 0.00% | 0.00% | 100.00% |
| Moderate | 1,489 | 164 | 1,143 | 182 | 0 | 23.24% | 76.76% | 0.00% |
| RelativelyStrong | 1,170 | 143 | 867 | 160 | 0 | 25.90% | 74.10% | 0.00% |

## Role Comparison

| Group | Total | A | Neutral | B | Insufficient | Directional | Neutral rate | Insufficient rate |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| TOP | 4,646 | 63 | 524 | 71 | 3,988 | 2.88% | 11.28% | 85.84% |
| JUNGLE | 2,302 | 69 | 435 | 80 | 1,718 | 6.47% | 18.90% | 74.63% |
| MID | 3,792 | 77 | 446 | 84 | 3,185 | 4.25% | 11.76% | 83.99% |
| ADC | 1,794 | 27 | 247 | 51 | 1,469 | 4.35% | 13.77% | 81.88% |
| SUPPORT | 3,013 | 71 | 358 | 56 | 2,528 | 4.22% | 11.88% | 83.90% |

## Effect Size Analysis

Recommended directional effects: min 5.02%, median 12.50%, p75 15.85%, p90 20.00%, max 30.95%.
Recommended directional samples: min 20, p25 30.0, median 47.0, p75 89.0, p90 152.0, max 1,761.

## Small Sample Risk

| Model | Directional n<5 | Directional n<10 | Directional n<20 | 0/100% at n=1-4 classified |
|---|---:|---:|---:|---:|
| A_n20_e5 | 0 | 0 | 0 | 0 |
| C_w80_n10_e5 | 0 | 0 | 311 | 0 |
| C_w80_n20_e5 | 0 | 0 | 0 | 0 |
| C_w90_n20_e5 | 0 | 0 | 0 | 0 |
| D_Balanced | 0 | 1,148 | 1,713 | 0 |

Recommended-model treatment of observed extreme win rates:

| Group | Total | A | Neutral | B | Insufficient |
|---|---:|---:|---:|---:|---:|
| WinRate0 | 3,320 | 0 | 0 | 0 | 3,320 |
| WinRate100 | 3,309 | 0 | 0 | 0 | 3,309 |
| WinRateAtMost30 | 3,980 | 0 | 0 | 34 | 3,946 |
| WinRateAtLeast70 | 4,025 | 36 | 0 | 0 | 3,989 |
| Sample1To4WinRate0Or100 | 6,562 | 0 | 0 | 0 | 6,562 |

## Wilson Confidence Comparison

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| B_w80_n20 | 322 | 1,970 | 367 | 12,888 | 4.43% | 28.43% | 49.0 | 12.07% | 46.73% | True |
| B_w90_n20 | 188 | 2,257 | 214 | 12,888 | 2.59% | 18.58% | 53.0 | 14.00% | 46.77% | True |
| B_w95_n20 | 115 | 2,426 | 118 | 12,888 | 1.50% | 13.16% | 57.0 | 14.58% | 49.36% | True |

A higher confidence level reduces directional coverage. This report does not treat 95% as automatically optimal for a user-facing reference tool.

## Borderline Examples

```json
[
  {
    "ModelId": "A_n20_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Fizz",
        "ChampionB": "Viego",
        "SampleSize": 20,
        "WinRateA": 0.45,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.2581978582556243,
        "Wilson95Upper": 0.6579146575496577,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "MID",
        "ChampionA": "Tristana",
        "ChampionB": "Yasuo",
        "SampleSize": 20,
        "WinRateA": 0.45,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.2581978582556243,
        "Wilson95Upper": 0.6579146575496577,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Anivia",
        "ChampionB": "Nautilus",
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
        "ChampionA": "Sylas",
        "ChampionB": "Yone",
        "SampleSize": 211,
        "WinRateA": 0.45023696682464454,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3845938861446776,
        "Wilson95Upper": 0.5176596169022993,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Ezreal",
        "ChampionB": "MissFortune",
        "SampleSize": 131,
        "WinRateA": 0.549618320610687,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.46421644795146716,
        "Wilson95Upper": 0.6321930696679379,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Yasuo",
        "SampleSize": 131,
        "WinRateA": 0.45038167938931295,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.367806930332062,
        "Wilson95Upper": 0.5357835520485328,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "C_w80_n10_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "SUPPORT",
        "ChampionA": "Camille",
        "ChampionB": "Nautilus",
        "SampleSize": 229,
        "WinRateA": 0.4497816593886463,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3867069356309431,
        "Wilson95Upper": 0.5145134049540565,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Yunara",
        "SampleSize": 1655,
        "WinRateA": 0.44954682779456195,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.42572507932109227,
        "Wilson95Upper": 0.47360224993512023,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Jhin",
        "ChampionB": "Lucian",
        "SampleSize": 552,
        "WinRateA": 0.5507246376811594,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.5090206228390016,
        "Wilson95Upper": 0.5917275295546914,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Caitlyn",
        "ChampionB": "Nilah",
        "SampleSize": 10,
        "WinRateA": 0.3,
        "Reliability": "Reference",
        "Wilson95Lower": 0.10779126740630099,
        "Wilson95Upper": 0.6032218525388546,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Mel",
        "ChampionB": "Tristana",
        "SampleSize": 10,
        "WinRateA": 0.3,
        "Reliability": "Reference",
        "Wilson95Lower": 0.10779126740630099,
        "Wilson95Upper": 0.6032218525388546,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Varus",
        "ChampionB": "Xerath",
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
        "Role": "SUPPORT",
        "ChampionA": "Camille",
        "ChampionB": "Nautilus",
        "SampleSize": 229,
        "WinRateA": 0.4497816593886463,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3867069356309431,
        "Wilson95Upper": 0.5145134049540565,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Yunara",
        "SampleSize": 1655,
        "WinRateA": 0.44954682779456195,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.42572507932109227,
        "Wilson95Upper": 0.47360224993512023,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Jhin",
        "ChampionB": "Lucian",
        "SampleSize": 552,
        "WinRateA": 0.5507246376811594,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.5090206228390016,
        "Wilson95Upper": 0.5917275295546914,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Cassiopeia",
        "ChampionB": "Xerath",
        "SampleSize": 22,
        "WinRateA": 0.6363636363636364,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.4295170809013101,
        "Wilson95Upper": 0.8026679532963388,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Corki",
        "ChampionB": "Varus",
        "SampleSize": 22,
        "WinRateA": 0.36363636363636365,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.19733204670366122,
        "Wilson95Upper": 0.5704829190986899,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "Corki",
        "ChampionB": "Ziggs",
        "SampleSize": 22,
        "WinRateA": 0.36363636363636365,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.19733204670366122,
        "Wilson95Upper": 0.5704829190986899,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "C_w90_n20_e5",
    "DirectionalSmallestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Yunara",
        "SampleSize": 1655,
        "WinRateA": 0.44954682779456195,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.42572507932109227,
        "Wilson95Upper": 0.47360224993512023,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Jhin",
        "ChampionB": "Lucian",
        "SampleSize": 552,
        "WinRateA": 0.5507246376811594,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.5090206228390016,
        "Wilson95Upper": 0.5917275295546914,
        "Result": "SideAAdvantage"
      },
      {
        "Role": "ADC",
        "ChampionA": "Jinx",
        "ChampionB": "Kaisa",
        "SampleSize": 691,
        "WinRateA": 0.5513748191027497,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.5141093932552752,
        "Wilson95Upper": 0.5880721894621743,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Kayn",
        "ChampionB": "XinZhao",
        "SampleSize": 21,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.17194752605083477,
        "Wilson95Upper": 0.5462654802574476,
        "Result": "Neutral"
      },
      {
        "Role": "MID",
        "ChampionA": "Diana",
        "ChampionB": "Lissandra",
        "SampleSize": 21,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.17194752605083477,
        "Wilson95Upper": 0.5462654802574476,
        "Result": "Neutral"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Galio",
        "ChampionB": "Pantheon",
        "SampleSize": 21,
        "WinRateA": 0.3333333333333333,
        "Reliability": "Moderate",
        "Wilson95Lower": 0.17194752605083477,
        "Wilson95Upper": 0.5462654802574476,
        "Result": "Neutral"
      }
    ]
  },
  {
    "ModelId": "D_Balanced",
    "DirectionalSmallestEffects": [
      {
        "Role": "MID",
        "ChampionA": "Akali",
        "ChampionB": "Lissandra",
        "SampleSize": 60,
        "WinRateA": 0.45,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3309365440675524,
        "Wilson95Upper": 0.5750806411466566,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Thresh",
        "ChampionB": "Zyra",
        "SampleSize": 60,
        "WinRateA": 0.45,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.3309365440675524,
        "Wilson95Upper": 0.5750806411466566,
        "Result": "SideBAdvantage"
      },
      {
        "Role": "MID",
        "ChampionA": "Qiyana",
        "ChampionB": "Yasuo",
        "SampleSize": 60,
        "WinRateA": 0.55,
        "Reliability": "RelativelyStrong",
        "Wilson95Lower": 0.4249193588533434,
        "Wilson95Upper": 0.6690634559324476,
        "Result": "SideAAdvantage"
      }
    ],
    "NeutralLargestEffects": [
      {
        "Role": "ADC",
        "ChampionA": "Caitlyn",
        "ChampionB": "Vayne",
        "SampleSize": 13,
        "WinRateA": 0.6153846153846154,
        "Reliability": "Reference",
        "Wilson95Lower": 0.35522891512665694,
        "Wilson95Upper": 0.8229029220223743,
        "Result": "Neutral"
      },
      {
        "Role": "ADC",
        "ChampionA": "KogMaw",
        "ChampionB": "Xayah",
        "SampleSize": 13,
        "WinRateA": 0.6153846153846154,
        "Reliability": "Reference",
        "Wilson95Lower": 0.35522891512665694,
        "Wilson95Upper": 0.8229029220223743,
        "Result": "Neutral"
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Ambessa",
        "ChampionB": "Rengar",
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
          "Role": "SUPPORT",
          "ChampionA": "Camille",
          "ChampionB": "Nautilus",
          "SampleSize": 229,
          "WinRateA": 0.4497816593886463,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.3867069356309431,
          "Wilson95Upper": 0.5145134049540565,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "TwistedFate",
          "ChampionB": "Yone",
          "SampleSize": 204,
          "WinRateA": 0.553921568627451,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48533860902096143,
          "Wilson95Upper": 0.6205113023722199,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Kaisa",
          "ChampionB": "Varus",
          "SampleSize": 201,
          "WinRateA": 0.5572139303482587,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48811087406533504,
          "Wilson95Upper": 0.6241710835638226,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "Nidalee",
          "ChampionB": "Sylas",
          "SampleSize": 199,
          "WinRateA": 0.5527638190954773,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48333334215859325,
          "Wilson95Upper": 0.620195789026385,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Akali",
          "ChampionB": "Locke",
          "SampleSize": 199,
          "WinRateA": 0.4472361809045226,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.379804210973615,
          "Wilson95Upper": 0.5166666578414066,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Alistar",
          "ChampionB": "Braum",
          "SampleSize": 194,
          "WinRateA": 0.5567010309278351,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4863685765914585,
          "Wilson95Upper": 0.624831573928756,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "TOP",
          "ChampionA": "Gangplank",
          "ChampionB": "KSante",
          "SampleSize": 179,
          "WinRateA": 0.5586592178770949,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48544297455619534,
          "Wilson95Upper": 0.6294106266754264,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Pyke",
          "ChampionB": "Rell",
          "SampleSize": 171,
          "WinRateA": 0.43859649122807015,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.36638096268117765,
          "Wilson95Upper": 0.5135102247886305,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Alistar",
          "ChampionB": "Bard",
          "SampleSize": 169,
          "WinRateA": 0.5562130177514792,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48088489585585675,
          "Wilson95Upper": 0.629042433672822,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "MID",
          "ChampionA": "Hwei",
          "ChampionB": "Viktor",
          "SampleSize": 157,
          "WinRateA": 0.554140127388535,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.47601905780316917,
          "Wilson95Upper": 0.6296750842709311,
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
          "Role": "ADC",
          "ChampionA": "Jhin",
          "ChampionB": "Yunara",
          "SampleSize": 1035,
          "WinRateA": 0.4782608695652174,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4479650109289633,
          "Wilson95Upper": 0.5087175034066846,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Caitlyn",
          "ChampionB": "Jhin",
          "SampleSize": 906,
          "WinRateA": 0.47019867549668876,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4378930985745544,
          "Wilson95Upper": 0.502755901889776,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Lulu",
          "ChampionB": "Nautilus",
          "SampleSize": 720,
          "WinRateA": 0.5277777777777778,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4912615832794755,
          "Wilson95Upper": 0.5639991364714619,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Nautilus",
          "ChampionB": "Thresh",
          "SampleSize": 678,
          "WinRateA": 0.46607669616519176,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.42882399567975965,
          "Wilson95Upper": 0.5037116408602051,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Caitlyn",
          "ChampionB": "Kaisa",
          "SampleSize": 604,
          "WinRateA": 0.4652317880794702,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4257984232738756,
          "Wilson95Upper": 0.505104611735473,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Jinx",
          "ChampionB": "Yunara",
          "SampleSize": 590,
          "WinRateA": 0.5305084745762711,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.49017105054799764,
          "Wilson95Upper": 0.5704511903974091,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "JUNGLE",
          "ChampionA": "LeeSin",
          "ChampionB": "Viego",
          "SampleSize": 522,
          "WinRateA": 0.5325670498084292,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.48968353430589984,
          "Wilson95Upper": 0.5749747375546947,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Aphelios",
          "ChampionB": "Yunara",
          "SampleSize": 512,
          "WinRateA": 0.470703125,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.4278469468194331,
          "Wilson95Upper": 0.5139956494143296,
          "Result": "SideBAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "ADC",
          "ChampionA": "Tristana",
          "ChampionB": "Yunara",
          "SampleSize": 506,
          "WinRateA": 0.5355731225296443,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.49201316747109275,
          "Wilson95Upper": 0.5785970180624083,
          "Result": "SideAAdvantage"
        },
        "SecondModelResult": "Neutral"
      },
      {
        "FirstModelResult": {
          "Role": "SUPPORT",
          "ChampionA": "Alistar",
          "ChampionB": "Rell",
          "SampleSize": 487,
          "WinRateA": 0.5359342915811088,
          "Reliability": "RelativelyStrong",
          "Wilson95Lower": 0.49153343557204165,
          "Wilson95Upper": 0.5797726845020282,
          "Result": "SideAAdvantage"
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
| TOP | Jayce vs Vayne | 56 | 69.64% | RelativelyStrong | 56.66%–80.10% | SideAAdvantage |
| ADC | Kalista vs MissFortune | 52 | 67.31% | RelativelyStrong | 53.76%–78.48% | SideAAdvantage |
| ADC | KogMaw vs Yunara | 64 | 67.19% | RelativelyStrong | 55.00%–77.43% | SideAAdvantage |
| SUPPORT | Braum vs Camille | 73 | 65.75% | RelativelyStrong | 54.33%–75.61% | SideAAdvantage |
| MID | Cassiopeia vs Ryze | 108 | 65.74% | RelativelyStrong | 56.39%–74.01% | SideAAdvantage |
| JUNGLE | Hecarim vs Naafiri | 96 | 65.62% | RelativelyStrong | 55.69%–74.36% | SideAAdvantage |
| TOP | Gangplank vs Renekton | 61 | 65.57% | RelativelyStrong | 53.05%–76.25% | SideAAdvantage |
| SUPPORT | Alistar vs Sylas | 80 | 65.00% | RelativelyStrong | 54.08%–74.55% | SideAAdvantage |
| MID | Leblanc vs Sylas | 91 | 64.84% | RelativelyStrong | 54.61%–73.86% | SideAAdvantage |
| MID | Malzahar vs Ryze | 51 | 64.71% | RelativelyStrong | 50.99%–76.37% | SideAAdvantage |

B-side:

| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |
|---|---|---:|---:|---|---:|---|
| JUNGLE | Vi vs Zyra | 50 | 22.00% | RelativelyStrong | 12.75%–35.24% | SideBAdvantage |
| TOP | Jax vs Singed | 58 | 32.76% | RelativelyStrong | 22.08%–45.58% | SideBAdvantage |
| MID | Aurora vs TwistedFate | 61 | 32.79% | RelativelyStrong | 22.34%–45.28% | SideBAdvantage |
| MID | Fizz vs Sylas | 61 | 32.79% | RelativelyStrong | 22.34%–45.28% | SideBAdvantage |
| TOP | Fiora vs Kennen | 60 | 35.00% | RelativelyStrong | 24.17%–47.64% | SideBAdvantage |
| MID | Fizz vs Yone | 51 | 35.29% | RelativelyStrong | 23.63%–49.01% | SideBAdvantage |
| MID | Nasus vs Yone | 62 | 35.48% | RelativelyStrong | 24.74%–47.92% | SideBAdvantage |
| MID | Akali vs Yasuo | 132 | 35.61% | RelativelyStrong | 27.95%–44.08% | SideBAdvantage |
| MID | Ahri vs Talon | 56 | 35.71% | RelativelyStrong | 24.46%–48.81% | SideBAdvantage |
| SUPPORT | Karma vs Rell | 134 | 35.82% | RelativelyStrong | 28.20%–44.23% | SideBAdvantage |

## Neutral Examples

| Role | Matchup | n | Win rate A | Reliability | Wilson 95% | Result |
|---|---|---:|---:|---|---:|---|
| ADC | Aphelios vs Caitlyn | 280 | 50.00% | RelativelyStrong | 44.18%–55.82% | Neutral |
| ADC | Jinx vs Zeri | 278 | 50.00% | RelativelyStrong | 44.16%–55.84% | Neutral |
| SUPPORT | Blitzcrank vs Rell | 158 | 50.00% | RelativelyStrong | 42.30%–57.70% | Neutral |
| TOP | Aatrox vs Gangplank | 156 | 50.00% | RelativelyStrong | 42.25%–57.75% | Neutral |
| MID | Lissandra vs Sylas | 146 | 50.00% | RelativelyStrong | 41.99%–58.01% | Neutral |
| JUNGLE | Kayn vs LeeSin | 136 | 50.00% | RelativelyStrong | 41.71%–58.29% | Neutral |
| TOP | Jayce vs Rumble | 136 | 50.00% | RelativelyStrong | 41.71%–58.29% | Neutral |
| SUPPORT | Bard vs Senna | 128 | 50.00% | RelativelyStrong | 41.47%–58.53% | Neutral |
| TOP | Ambessa vs Rumble | 122 | 50.00% | RelativelyStrong | 41.26%–58.74% | Neutral |
| MID | Locke vs Orianna | 100 | 50.00% | RelativelyStrong | 40.38%–59.62% | Neutral |

## Lane Metric Direction Diagnostic

Exploratory only; these metrics are not inputs to the outcome rule.

| Metric | Same direction | Opposite | Neutral | Agreement among non-neutral |
|---|---:|---:|---:|---:|
| FirstLevel2 | 300 | 338 | 11 | 47.02% |
| FirstLevel3 | 329 | 302 | 18 | 52.14% |
| FirstLevel6 | 352 | 283 | 14 | 55.43% |
| SoloKillDifference | 374 | 237 | 38 | 61.21% |

## Candidate Shortlist

| Model | A | Neutral | B | Insufficient | Key directional | Sample directional | Median n | Median effect | A share | Symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_e5 | 687 | 1,272 | 700 | 12,888 | 8.92% | 31.73% | 38.0 | 9.09% | 49.53% | True |
| C_w80_n10_e5 | 469 | 3,288 | 491 | 11,299 | 6.17% | 22.16% | 30.0 | 15.71% | 48.85% | True |
| C_w80_n20_e5 | 307 | 2,010 | 342 | 12,888 | 4.17% | 20.47% | 47.0 | 12.50% | 47.30% | True |
| C_w90_n20_e5 | 186 | 2,271 | 202 | 12,888 | 2.50% | 14.56% | 50.5 | 14.15% | 47.94% | True |
| D_Balanced | 1,461 | 3,467 | 1,386 | 9,233 | 18.31% | 34.56% | 13.0 | 16.67% | 51.32% | True |

## Recommended Production Rule

- Model: `C_w80_n20_e5`
- Rule: Minimum sample 20; |win rate - 50%| at least 5 percentage points; 80% Wilson interval must exclude 50%; no reliability-specific exception.
- Suggested wording: 우세 경향 / 뚜렷한 차이 없음 / 열세 경향
- Directional key coverage: 4.17% (649 keys)
- Directional sample-weighted coverage: 20.47%
- Directional median sample/effect: 47.0 / 12.50%
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
