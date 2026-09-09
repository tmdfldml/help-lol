# Lane Pressure Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 51,649
- Observations: 258,245
- Matchup keys: 15,547
- Scope: `MASTER_PLUS_SEEDED`; region `KR`; queue 420
- Minimum duration: 900s; revision: 1788854099077

## Research Scope

Research whether first-level timing and direct-opponent solo-kill frequency describe one coherent early-pressure concept.
This is research only. No production Lane Pressure classifier or UI is implemented.

## Metric Definitions

- Level differences: (FirstLevelForA - FirstLevelForB) / SampleSize; Unknown remains in the SampleSize denominator.
- Solo difference: SoloKillsBefore15ForA / SampleSize - SoloKillsBefore15ForB / SampleSize; EVENT_COUNT per match, not probability.
- Outcome separation: Game wins and OutcomeAdvantage are excluded from every lane-pressure model input.

## Metric Difference Distributions

### ALL (keys 15,547)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0065 | 0.0000 | 0.7575 | -1.0000 | -0.8182 | 0.7778 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0112 | 0.0000 | 0.7260 | -1.0000 | -0.6787 | 0.6000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0003 | 0.0000 | 0.6998 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0180 | 0.0000 | 1.0731 | -1.0000 | -0.2500 | 0.2857 | 1.0000 | 2.0000 | -10.0000 | 8.0000 |
| Level Unknown | 0.0015 | 0.0000 | 0.0152 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0016 | 0.0000 | 0.3333 |

## Role Comparison

### TOP (keys 4,646)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0433 | 0.0000 | 0.7930 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0334 | 0.0000 | 0.7737 | -1.0000 | -1.0000 | 0.8182 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0167 | 0.0000 | 0.7116 | -1.0000 | -0.5000 | 0.6000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0363 | 0.0000 | 1.5923 | -2.0000 | -0.8000 | 0.8571 | 2.0000 | 2.9643 | -10.0000 | 8.0000 |
| Level Unknown | 0.0007 | 0.0000 | 0.0101 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### JUNGLE (keys 2,302)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0046 | 0.0000 | 0.6717 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | 0.0076 | 0.0000 | 0.6749 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0108 | 0.0000 | 0.6259 | -1.0000 | -0.3333 | 0.3333 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | -0.0009 | 0.0000 | 0.4380 | -0.3333 | -0.0909 | 0.0909 | 0.3333 | 0.6667 | -4.0000 | 3.0000 |
| Level Unknown | 0.0028 | 0.0000 | 0.0189 | 0.0000 | 0.0000 | 0.0000 | 0.0036 | 0.0107 | 0.0000 | 0.3333 |

### MID (keys 3,792)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0097 | 0.0000 | 0.7715 | -1.0000 | -1.0000 | 0.8400 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0183 | 0.0000 | 0.7224 | -1.0000 | -0.6942 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0136 | 0.0000 | 0.7181 | -1.0000 | -0.5000 | 0.6667 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0299 | 0.0000 | 1.1112 | -1.0000 | -0.4651 | 0.5000 | 1.0000 | 2.0000 | -8.0000 | 7.0000 |
| Level Unknown | 0.0019 | 0.0000 | 0.0177 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0035 | 0.0000 | 0.3333 |

### ADC (keys 1,794)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0870 | 0.2000 | 0.7882 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | 0.0224 | 0.0000 | 0.7078 | -1.0000 | -0.5000 | 0.5675 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0031 | 0.0000 | 0.7173 | -1.0000 | -0.6000 | 0.5338 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0003 | 0.0000 | 0.6114 | -0.6667 | -0.1598 | 0.1126 | 0.6667 | 1.0000 | -3.0000 | 4.0000 |
| Level Unknown | 0.0008 | 0.0000 | 0.0124 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### SUPPORT (keys 3,013)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0099 | 0.0000 | 0.7209 | -1.0000 | -0.6000 | 0.5992 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0026 | 0.0000 | 0.7013 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0508 | 0.0000 | 0.6986 | -1.0000 | -0.6000 | 0.4286 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | -0.0002 | 0.0000 | 0.3272 | -0.2000 | 0.0000 | 0.0000 | 0.1667 | 0.5000 | -3.0000 | 3.0000 |
| Level Unknown | 0.0014 | 0.0000 | 0.0164 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

## Sample-size Volatility

### 1-4 (keys 9,233)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0086 | 0.0000 | 0.8872 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0128 | 0.0000 | 0.8729 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0078 | 0.0000 | 0.8606 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0180 | 0.0000 | 1.3236 | -1.0000 | -0.5000 | 0.5000 | 1.3333 | 2.0000 | -10.0000 | 8.0000 |
| Level Unknown | 0.0015 | 0.0000 | 0.0191 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### 5-9 (keys 2,066)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0122 | 0.0000 | 0.5734 | -0.7500 | -0.5000 | 0.4286 | 0.7500 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0076 | 0.0000 | 0.5072 | -0.6667 | -0.3333 | 0.3333 | 0.6667 | 0.7778 | -1.0000 | 1.0000 |
| L6 | -0.0121 | 0.0000 | 0.4537 | -0.6000 | -0.3333 | 0.3333 | 0.6000 | 0.7143 | -1.0000 | 1.0000 |
| Solo | 0.0220 | 0.0000 | 0.6837 | -0.7143 | -0.2857 | 0.2857 | 0.8000 | 1.2000 | -2.8333 | 5.0000 |
| Level Unknown | 0.0011 | 0.0000 | 0.0075 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0667 |

### 10-19 (keys 1,589)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0007 | 0.0000 | 0.5055 | -0.6667 | -0.4000 | 0.4000 | 0.6842 | 0.8182 | -1.0000 | 1.0000 |
| L3 | -0.0112 | 0.0000 | 0.4366 | -0.6000 | -0.3333 | 0.2857 | 0.5714 | 0.6772 | -1.0000 | 1.0000 |
| L6 | -0.0041 | 0.0000 | 0.3430 | -0.4444 | -0.2308 | 0.2308 | 0.4444 | 0.5385 | -1.0000 | 1.0000 |
| Solo | 0.0180 | 0.0000 | 0.5080 | -0.5000 | -0.1818 | 0.2222 | 0.5843 | 0.8667 | -2.3846 | 3.0714 |
| Level Unknown | 0.0015 | 0.0000 | 0.0069 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0185 | 0.0000 | 0.1333 |

### 20-49 (keys 1,489)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0064 | 0.0000 | 0.4886 | -0.6923 | -0.4194 | 0.3939 | 0.6432 | 0.7580 | -1.0000 | 1.0000 |
| L3 | -0.0266 | 0.0000 | 0.3794 | -0.5630 | -0.2667 | 0.2245 | 0.4599 | 0.5787 | -1.0000 | 1.0000 |
| L6 | -0.0317 | -0.0303 | 0.2702 | -0.3690 | -0.2222 | 0.1538 | 0.3333 | 0.3976 | -0.8000 | 0.8182 |
| Solo | 0.0055 | 0.0000 | 0.3944 | -0.4190 | -0.1481 | 0.1613 | 0.4484 | 0.6352 | -1.8684 | 2.5862 |
| Level Unknown | 0.0016 | 0.0000 | 0.0044 | 0.0000 | 0.0000 | 0.0000 | 0.0088 | 0.0123 | 0.0000 | 0.0317 |

### 50-99 (keys 655)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0094 | 0.0400 | 0.4414 | -0.6215 | -0.3377 | 0.3333 | 0.6000 | 0.6969 | -0.9677 | 0.9259 |
| L3 | 0.0113 | 0.0164 | 0.3205 | -0.4105 | -0.1795 | 0.2098 | 0.4153 | 0.5452 | -0.8652 | 0.8718 |
| L6 | -0.0005 | 0.0000 | 0.2323 | -0.2941 | -0.1733 | 0.1613 | 0.2846 | 0.3575 | -0.7097 | 0.7812 |
| Solo | 0.0289 | 0.0000 | 0.3561 | -0.3233 | -0.1169 | 0.1617 | 0.4337 | 0.6455 | -1.3200 | 1.4200 |
| Level Unknown | 0.0015 | 0.0000 | 0.0032 | 0.0000 | 0.0000 | 0.0000 | 0.0058 | 0.0067 | 0.0000 | 0.0278 |

### 100+ (keys 515)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0112 | 0.0056 | 0.4018 | -0.5419 | -0.2669 | 0.3090 | 0.5373 | 0.6955 | -0.9304 | 0.8923 |
| L3 | 0.0174 | 0.0253 | 0.2882 | -0.3420 | -0.1414 | 0.1557 | 0.3731 | 0.5055 | -0.8690 | 0.9057 |
| L6 | 0.0047 | 0.0000 | 0.1762 | -0.2106 | -0.1070 | 0.1297 | 0.2173 | 0.2649 | -0.5817 | 0.5965 |
| Solo | 0.0237 | 0.0000 | 0.2597 | -0.2341 | -0.0580 | 0.0810 | 0.3135 | 0.4802 | -0.9935 | 1.6316 |
| Level Unknown | 0.0016 | 0.0010 | 0.0020 | 0.0000 | 0.0000 | 0.0026 | 0.0044 | 0.0057 | 0.0000 | 0.0117 |

## Level Progression Consistency

| n | threshold | AAA | BBB | consistent | reversal | NNN | mixed/neutral |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 20 | 2.5% | 613 | 628 | 1,241 | 994 | 2 | 422 |
| 20 | 5.0% | 516 | 536 | 1,052 | 727 | 14 | 866 |
| 20 | 7.5% | 463 | 461 | 924 | 568 | 33 | 1,134 |
| 50 | 2.5% | 298 | 253 | 551 | 395 | 2 | 222 |
| 50 | 5.0% | 254 | 219 | 473 | 293 | 11 | 393 |
| 50 | 7.5% | 227 | 172 | 399 | 199 | 26 | 546 |

## Metric Direction Agreement

Agreement is exact categorical agreement (A/Neutral/B), reported separately from Pearson correlation.

| n | level | solo | L2-L3 | L2-L6 | L3-L6 | L2-Solo | L3-Solo | L6-Solo | all same |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 20 | 2.5% | 0.05 | 71.23% | 55.55% | 61.83% | 36.82% | 41.03% | 47.57% | 596 |
| 20 | 2.5% | 0.10 | 71.23% | 55.55% | 61.83% | 30.24% | 33.66% | 40.80% | 484 |
| 20 | 2.5% | 0.20 | 71.23% | 55.55% | 61.83% | 22.75% | 25.57% | 31.59% | 360 |
| 20 | 5.0% | 0.05 | 65.74% | 49.91% | 55.36% | 37.01% | 40.39% | 46.41% | 531 |
| 20 | 5.0% | 0.10 | 65.74% | 49.91% | 55.36% | 31.37% | 35.43% | 42.31% | 440 |
| 20 | 5.0% | 0.20 | 65.74% | 49.91% | 55.36% | 25.20% | 29.90% | 35.50% | 338 |
| 20 | 7.5% | 0.05 | 63.14% | 46.82% | 51.34% | 37.23% | 40.73% | 45.58% | 490 |
| 20 | 7.5% | 0.10 | 63.14% | 46.82% | 51.34% | 32.42% | 37.42% | 43.51% | 417 |
| 20 | 7.5% | 0.20 | 63.14% | 46.82% | 51.34% | 27.00% | 33.43% | 39.23% | 332 |
| 50 | 2.5% | 0.05 | 70.85% | 55.21% | 63.16% | 31.97% | 38.46% | 45.47% | 238 |
| 50 | 2.5% | 0.10 | 70.85% | 55.21% | 63.16% | 24.87% | 31.20% | 38.55% | 185 |
| 50 | 2.5% | 0.20 | 70.85% | 55.21% | 63.16% | 18.89% | 23.59% | 29.74% | 134 |
| 50 | 5.0% | 0.05 | 66.75% | 50.68% | 56.92% | 32.99% | 38.63% | 44.53% | 216 |
| 50 | 5.0% | 0.10 | 66.75% | 50.68% | 56.92% | 26.67% | 34.02% | 40.51% | 173 |
| 50 | 5.0% | 0.20 | 66.75% | 50.68% | 56.92% | 21.54% | 29.40% | 34.53% | 132 |
| 50 | 7.5% | 0.05 | 63.42% | 47.01% | 52.31% | 33.59% | 39.91% | 43.85% | 199 |
| 50 | 7.5% | 0.10 | 63.42% | 47.01% | 52.31% | 28.29% | 37.78% | 42.74% | 167 |
| 50 | 7.5% | 0.20 | 63.42% | 47.01% | 52.31% | 24.02% | 34.70% | 40.09% | 132 |

## Model Definitions

- Model A: four-signal majority vote over L2/L3/L6/Solo.
- Model B: Level Progression majority; at least two of L2/L3/L6 point to one side.
- Model D: L2/L3 are combined into one EarlyLevel signal, then EarlyLevel/L6/Solo vote.
- Model E: Level Tempo (Model B) and Combat Pressure (Solo) remain separate.
- Model F standardized score was not promoted: patch-dependent scaling and arbitrary weighting reduce explainability.

## Parameter Sweep — Model A

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n5_l2_5_s5 | 2,530 | 1,158 | 2,626 | 9,233 | 33.16% | 79.64% | 15.0 | True |
| A_n5_l2_5_s10 | 2,597 | 1,055 | 2,662 | 9,233 | 33.83% | 81.54% | 16.0 | True |
| A_n5_l2_5_s15 | 2,656 | 941 | 2,717 | 9,233 | 34.56% | 82.90% | 16.0 | True |
| A_n5_l2_5_s20 | 2,691 | 884 | 2,739 | 9,233 | 34.93% | 83.69% | 16.0 | True |
| A_n5_l5_s5 | 2,532 | 1,125 | 2,657 | 9,233 | 33.38% | 79.92% | 16.0 | True |
| A_n5_l5_s10 | 2,581 | 1,055 | 2,678 | 9,233 | 33.83% | 80.95% | 16.0 | True |
| A_n5_l5_s15 | 2,629 | 963 | 2,722 | 9,233 | 34.42% | 81.94% | 16.0 | True |
| A_n5_l5_s20 | 2,651 | 927 | 2,736 | 9,233 | 34.65% | 82.40% | 16.0 | True |
| A_n5_l7_5_s5 | 2,558 | 1,085 | 2,671 | 9,233 | 33.63% | 79.50% | 16.0 | True |
| A_n5_l7_5_s10 | 2,594 | 1,038 | 2,682 | 9,233 | 33.94% | 80.96% | 16.0 | True |
| A_n5_l7_5_s15 | 2,627 | 975 | 2,712 | 9,233 | 34.34% | 81.10% | 16.0 | True |
| A_n5_l7_5_s20 | 2,645 | 952 | 2,717 | 9,233 | 34.49% | 81.40% | 15.0 | True |
| A_n10_l2_5_s5 | 1,733 | 766 | 1,749 | 11,299 | 22.40% | 75.34% | 27.0 | True |
| A_n10_l2_5_s10 | 1,800 | 663 | 1,785 | 11,299 | 23.06% | 77.24% | 27.0 | True |
| A_n10_l2_5_s15 | 1,840 | 590 | 1,818 | 11,299 | 23.53% | 78.48% | 27.0 | True |
| A_n10_l2_5_s20 | 1,877 | 528 | 1,843 | 11,299 | 23.93% | 79.29% | 27.0 | True |
| A_n10_l5_s5 | 1,735 | 733 | 1,780 | 11,299 | 22.61% | 75.63% | 27.0 | True |
| A_n10_l5_s10 | 1,784 | 663 | 1,801 | 11,299 | 23.06% | 76.65% | 26.0 | True |
| A_n10_l5_s15 | 1,813 | 612 | 1,823 | 11,299 | 23.39% | 77.53% | 26.0 | True |
| A_n10_l5_s20 | 1,837 | 571 | 1,840 | 11,299 | 23.65% | 77.99% | 26.0 | True |
| A_n10_l7_5_s5 | 1,761 | 693 | 1,794 | 11,299 | 22.87% | 75.20% | 27.0 | True |
| A_n10_l7_5_s10 | 1,797 | 646 | 1,805 | 11,299 | 23.17% | 76.66% | 26.0 | True |
| A_n10_l7_5_s15 | 1,811 | 624 | 1,813 | 11,299 | 23.31% | 76.68% | 26.0 | True |
| A_n10_l7_5_s20 | 1,831 | 596 | 1,821 | 11,299 | 23.49% | 77.00% | 26.0 | True |
| A_n20_l2_5_s5 | 1,113 | 452 | 1,094 | 12,888 | 14.20% | 68.61% | 44.0 | True |
| A_n20_l2_5_s10 | 1,152 | 400 | 1,107 | 12,888 | 14.53% | 70.24% | 44.0 | True |
| A_n20_l2_5_s15 | 1,182 | 342 | 1,135 | 12,888 | 14.90% | 71.39% | 44.0 | True |
| A_n20_l2_5_s20 | 1,203 | 307 | 1,149 | 12,888 | 15.13% | 72.06% | 44.0 | True |
| A_n20_l5_s5 | 1,115 | 419 | 1,125 | 12,888 | 14.41% | 68.90% | 44.0 | True |
| A_n20_l5_s10 | 1,136 | 400 | 1,123 | 12,888 | 14.53% | 69.65% | 44.0 | True |
| A_n20_l5_s15 | 1,155 | 364 | 1,140 | 12,888 | 14.76% | 70.43% | 44.0 | True |
| A_n20_l5_s20 | 1,163 | 350 | 1,146 | 12,888 | 14.85% | 70.76% | 44.0 | True |
| A_n20_l7_5_s5 | 1,121 | 407 | 1,131 | 12,888 | 14.49% | 68.29% | 44.0 | True |
| A_n20_l7_5_s10 | 1,135 | 397 | 1,127 | 12,888 | 14.55% | 69.56% | 44.0 | True |
| A_n20_l7_5_s15 | 1,144 | 381 | 1,134 | 12,888 | 14.65% | 69.55% | 44.0 | True |
| A_n20_l7_5_s20 | 1,148 | 378 | 1,133 | 12,888 | 14.67% | 69.75% | 44.0 | True |
| A_n30_l2_5_s5 | 820 | 319 | 794 | 13,614 | 10.38% | 63.10% | 58.0 | True |
| A_n30_l2_5_s10 | 852 | 275 | 806 | 13,614 | 10.66% | 64.64% | 59.0 | True |
| A_n30_l2_5_s15 | 870 | 239 | 824 | 13,614 | 10.90% | 65.59% | 58.0 | True |
| A_n30_l2_5_s20 | 887 | 213 | 833 | 13,614 | 11.06% | 66.17% | 58.0 | True |
| A_n30_l5_s5 | 821 | 301 | 811 | 13,614 | 10.50% | 63.25% | 58.0 | True |
| A_n30_l5_s10 | 836 | 285 | 812 | 13,614 | 10.60% | 63.96% | 58.0 | True |
| A_n30_l5_s15 | 845 | 266 | 822 | 13,614 | 10.72% | 64.59% | 58.0 | True |
| A_n30_l5_s20 | 852 | 254 | 827 | 13,614 | 10.80% | 64.90% | 58.0 | True |
| A_n30_l7_5_s5 | 830 | 287 | 816 | 13,614 | 10.59% | 62.66% | 57.0 | True |
| A_n30_l7_5_s10 | 838 | 280 | 815 | 13,614 | 10.63% | 63.90% | 58.0 | True |
| A_n30_l7_5_s15 | 837 | 281 | 815 | 13,614 | 10.63% | 63.73% | 58.0 | True |
| A_n30_l7_5_s20 | 840 | 280 | 813 | 13,614 | 10.63% | 63.91% | 58.0 | True |
| A_n50_l2_5_s5 | 521 | 184 | 465 | 14,377 | 6.34% | 53.80% | 90.0 | True |
| A_n50_l2_5_s10 | 542 | 153 | 475 | 14,377 | 6.54% | 55.15% | 90.0 | True |
| A_n50_l2_5_s15 | 549 | 138 | 483 | 14,377 | 6.64% | 55.80% | 90.0 | True |
| A_n50_l2_5_s20 | 559 | 123 | 488 | 14,377 | 6.73% | 56.23% | 90.0 | True |
| A_n50_l5_s5 | 517 | 181 | 472 | 14,377 | 6.36% | 53.73% | 90.0 | True |
| A_n50_l5_s10 | 530 | 167 | 473 | 14,377 | 6.45% | 54.42% | 89.0 | True |
| A_n50_l5_s15 | 533 | 159 | 478 | 14,377 | 6.50% | 54.89% | 90.0 | True |
| A_n50_l5_s20 | 537 | 152 | 481 | 14,377 | 6.55% | 55.13% | 90.0 | True |
| A_n50_l7_5_s5 | 523 | 173 | 474 | 14,377 | 6.41% | 53.06% | 89.0 | True |
| A_n50_l7_5_s10 | 532 | 165 | 473 | 14,377 | 6.46% | 54.33% | 89.0 | True |
| A_n50_l7_5_s15 | 529 | 167 | 474 | 14,377 | 6.45% | 54.15% | 89.0 | True |
| A_n50_l7_5_s20 | 532 | 162 | 476 | 14,377 | 6.48% | 54.39% | 89.0 | True |

## Parameter Sweep — Model B

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| B_n5_l2_5 | 2,859 | 500 | 2,955 | 9,233 | 37.40% | 86.17% | 16.0 | True |
| B_n5_l5 | 2,728 | 713 | 2,873 | 9,233 | 36.03% | 80.21% | 15.0 | True |
| B_n5_l7_5 | 2,606 | 946 | 2,762 | 9,233 | 34.53% | 71.67% | 14.0 | True |
| B_n10_l2_5 | 1,955 | 303 | 1,990 | 11,299 | 25.37% | 81.37% | 26.0 | True |
| B_n10_l5 | 1,824 | 516 | 1,908 | 11,299 | 24.00% | 75.40% | 26.0 | True |
| B_n10_l7_5 | 1,702 | 749 | 1,797 | 11,299 | 22.51% | 66.87% | 25.0 | True |
| B_n20_l2_5 | 1,246 | 171 | 1,242 | 12,888 | 16.00% | 73.63% | 43.0 | True |
| B_n20_l5 | 1,115 | 384 | 1,160 | 12,888 | 14.63% | 67.67% | 43.0 | True |
| B_n20_l7_5 | 1,029 | 543 | 1,087 | 12,888 | 13.61% | 59.62% | 42.0 | True |
| B_n30_l2_5 | 909 | 134 | 890 | 13,614 | 11.57% | 67.20% | 57.0 | True |
| B_n30_l5 | 817 | 281 | 835 | 13,614 | 10.63% | 61.87% | 57.0 | True |
| B_n30_l7_5 | 736 | 434 | 763 | 13,614 | 9.64% | 53.88% | 56.0 | True |
| B_n50_l2_5 | 560 | 90 | 520 | 14,377 | 6.95% | 56.59% | 89.0 | True |
| B_n50_l5 | 507 | 180 | 483 | 14,377 | 6.37% | 52.10% | 88.5 | True |
| B_n50_l7_5 | 455 | 280 | 435 | 14,377 | 5.72% | 44.89% | 86.5 | True |

## Parameter Sweep — Model D

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| D_n5_l2_5_s5 | 2,674 | 841 | 2,799 | 9,233 | 35.20% | 81.96% | 15.0 | True |
| D_n5_l2_5_s10 | 2,641 | 928 | 2,745 | 9,233 | 34.64% | 81.04% | 15.0 | True |
| D_n5_l2_5_s15 | 2,597 | 1,016 | 2,701 | 9,233 | 34.08% | 80.20% | 15.0 | True |
| D_n5_l2_5_s20 | 2,570 | 1,078 | 2,666 | 9,233 | 33.68% | 79.16% | 15.0 | True |
| D_n5_l5_s5 | 2,644 | 890 | 2,780 | 9,233 | 34.89% | 79.22% | 15.0 | True |
| D_n5_l5_s10 | 2,618 | 972 | 2,724 | 9,233 | 34.36% | 79.78% | 15.0 | True |
| D_n5_l5_s15 | 2,581 | 1,048 | 2,685 | 9,233 | 33.87% | 79.25% | 15.0 | True |
| D_n5_l5_s20 | 2,554 | 1,105 | 2,655 | 9,233 | 33.50% | 78.45% | 15.0 | True |
| D_n5_l7_5_s5 | 2,635 | 935 | 2,744 | 9,233 | 34.60% | 77.70% | 15.0 | True |
| D_n5_l7_5_s10 | 2,617 | 1,000 | 2,697 | 9,233 | 34.18% | 79.07% | 15.0 | True |
| D_n5_l7_5_s15 | 2,581 | 1,074 | 2,659 | 9,233 | 33.70% | 78.54% | 15.0 | True |
| D_n5_l7_5_s20 | 2,562 | 1,120 | 2,632 | 9,233 | 33.41% | 78.22% | 15.0 | True |
| D_n10_l2_5_s5 | 1,839 | 532 | 1,877 | 11,299 | 23.90% | 77.43% | 26.0 | True |
| D_n10_l2_5_s10 | 1,806 | 619 | 1,823 | 11,299 | 23.34% | 76.52% | 27.0 | True |
| D_n10_l2_5_s15 | 1,774 | 689 | 1,785 | 11,299 | 22.89% | 75.72% | 27.0 | True |
| D_n10_l2_5_s20 | 1,750 | 743 | 1,755 | 11,299 | 22.54% | 74.71% | 26.0 | True |
| D_n10_l5_s5 | 1,809 | 581 | 1,858 | 11,299 | 23.59% | 74.69% | 26.0 | True |
| D_n10_l5_s10 | 1,783 | 663 | 1,802 | 11,299 | 23.06% | 75.25% | 26.0 | True |
| D_n10_l5_s15 | 1,758 | 721 | 1,769 | 11,299 | 22.69% | 74.78% | 26.0 | True |
| D_n10_l5_s20 | 1,734 | 770 | 1,744 | 11,299 | 22.37% | 74.00% | 26.0 | True |
| D_n10_l7_5_s5 | 1,800 | 626 | 1,822 | 11,299 | 23.30% | 73.17% | 26.0 | True |
| D_n10_l7_5_s10 | 1,782 | 691 | 1,775 | 11,299 | 22.88% | 74.54% | 26.0 | True |
| D_n10_l7_5_s15 | 1,758 | 747 | 1,743 | 11,299 | 22.52% | 74.07% | 26.0 | True |
| D_n10_l7_5_s20 | 1,742 | 785 | 1,721 | 11,299 | 22.27% | 73.76% | 26.0 | True |
| D_n20_l2_5_s5 | 1,161 | 323 | 1,175 | 12,888 | 15.03% | 70.13% | 44.0 | True |
| D_n20_l2_5_s10 | 1,151 | 363 | 1,145 | 12,888 | 14.77% | 69.46% | 44.0 | True |
| D_n20_l2_5_s15 | 1,128 | 407 | 1,124 | 12,888 | 14.49% | 68.80% | 44.0 | True |
| D_n20_l2_5_s20 | 1,108 | 451 | 1,100 | 12,888 | 14.20% | 67.83% | 44.0 | True |
| D_n20_l5_s5 | 1,131 | 372 | 1,156 | 12,888 | 14.71% | 67.40% | 43.0 | True |
| D_n20_l5_s10 | 1,128 | 407 | 1,124 | 12,888 | 14.49% | 68.20% | 43.0 | True |
| D_n20_l5_s15 | 1,112 | 439 | 1,108 | 12,888 | 14.28% | 67.85% | 44.0 | True |
| D_n20_l5_s20 | 1,092 | 478 | 1,089 | 12,888 | 14.03% | 67.12% | 44.0 | True |
| D_n20_l7_5_s5 | 1,119 | 409 | 1,131 | 12,888 | 14.47% | 65.93% | 43.0 | True |
| D_n20_l7_5_s10 | 1,121 | 432 | 1,106 | 12,888 | 14.32% | 67.51% | 43.0 | True |
| D_n20_l7_5_s15 | 1,106 | 463 | 1,090 | 12,888 | 14.12% | 67.17% | 44.0 | True |
| D_n20_l7_5_s20 | 1,092 | 493 | 1,074 | 12,888 | 13.93% | 66.90% | 44.0 | True |
| D_n30_l2_5_s5 | 848 | 236 | 849 | 13,614 | 10.92% | 64.20% | 57.0 | True |
| D_n30_l2_5_s10 | 843 | 262 | 828 | 13,614 | 10.75% | 63.65% | 57.0 | True |
| D_n30_l2_5_s15 | 826 | 292 | 815 | 13,614 | 10.56% | 63.11% | 57.0 | True |
| D_n30_l2_5_s20 | 811 | 328 | 794 | 13,614 | 10.32% | 62.23% | 57.0 | True |
| D_n30_l5_s5 | 825 | 277 | 831 | 13,614 | 10.65% | 61.55% | 57.0 | True |
| D_n30_l5_s10 | 822 | 302 | 809 | 13,614 | 10.49% | 62.43% | 57.0 | True |
| D_n30_l5_s15 | 811 | 322 | 800 | 13,614 | 10.36% | 62.19% | 57.0 | True |
| D_n30_l5_s20 | 797 | 351 | 785 | 13,614 | 10.18% | 61.56% | 57.0 | True |
| D_n30_l7_5_s5 | 814 | 313 | 806 | 13,614 | 10.42% | 60.10% | 56.0 | True |
| D_n30_l7_5_s10 | 815 | 327 | 791 | 13,614 | 10.33% | 61.75% | 57.0 | True |
| D_n30_l7_5_s15 | 805 | 346 | 782 | 13,614 | 10.21% | 61.50% | 57.0 | True |
| D_n30_l7_5_s20 | 797 | 366 | 770 | 13,614 | 10.08% | 61.33% | 58.0 | True |
| D_n50_l2_5_s5 | 536 | 141 | 493 | 14,377 | 6.62% | 54.34% | 88.0 | True |
| D_n50_l2_5_s10 | 532 | 157 | 481 | 14,377 | 6.52% | 53.92% | 88.0 | True |
| D_n50_l2_5_s15 | 520 | 175 | 475 | 14,377 | 6.40% | 53.54% | 89.0 | True |
| D_n50_l2_5_s20 | 514 | 193 | 463 | 14,377 | 6.28% | 52.94% | 89.0 | True |
| D_n50_l5_s5 | 520 | 173 | 477 | 14,377 | 6.41% | 51.82% | 88.0 | True |
| D_n50_l5_s10 | 522 | 185 | 463 | 14,377 | 6.34% | 52.89% | 88.0 | True |
| D_n50_l5_s15 | 513 | 196 | 461 | 14,377 | 6.26% | 52.75% | 88.0 | True |
| D_n50_l5_s20 | 509 | 208 | 453 | 14,377 | 6.19% | 52.39% | 89.0 | True |
| D_n50_l7_5_s5 | 516 | 195 | 459 | 14,377 | 6.27% | 50.57% | 88.0 | True |
| D_n50_l7_5_s10 | 521 | 197 | 452 | 14,377 | 6.26% | 52.41% | 88.0 | True |
| D_n50_l7_5_s15 | 512 | 204 | 454 | 14,377 | 6.21% | 52.33% | 88.0 | True |
| D_n50_l7_5_s20 | 514 | 205 | 451 | 14,377 | 6.21% | 52.42% | 89.0 | True |

## Coverage Comparison

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_l5_s10 | 1,136 | 400 | 1,123 | 12,888 | 14.53% | 69.65% | 44.0 | True |
| B_n20_l5 | 1,115 | 384 | 1,160 | 12,888 | 14.63% | 67.67% | 43.0 | True |
| D_n20_l5_s10 | 1,128 | 407 | 1,124 | 12,888 | 14.49% | 68.20% | 43.0 | True |
| D_n20_l7_5_s15 | 1,106 | 463 | 1,090 | 12,888 | 14.12% | 67.17% | 44.0 | True |

## Reliability Comparison

### A_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 9,233 | 0.00% |
| Reference | 0 | 0 | 0 | 3,655 | 0.00% |
| Moderate | 606 | 233 | 650 | 0 | 84.35% |
| RelativelyStrong | 530 | 167 | 473 | 0 | 85.73% |

### B_n20_l5

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 9,233 | 0.00% |
| Reference | 0 | 0 | 0 | 3,655 | 0.00% |
| Moderate | 608 | 204 | 677 | 0 | 86.30% |
| RelativelyStrong | 507 | 180 | 483 | 0 | 84.62% |

### D_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 9,233 | 0.00% |
| Reference | 0 | 0 | 0 | 3,655 | 0.00% |
| Moderate | 606 | 222 | 661 | 0 | 85.09% |
| RelativelyStrong | 522 | 185 | 463 | 0 | 84.19% |

### D_n20_l7_5_s15

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 9,233 | 0.00% |
| Reference | 0 | 0 | 0 | 3,655 | 0.00% |
| Moderate | 594 | 259 | 636 | 0 | 82.61% |
| RelativelyStrong | 512 | 204 | 454 | 0 | 82.56% |

## Role Coverage Comparison

### A_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 237 | 121 | 300 | 3,988 | 11.56% |
| JUNGLE | 283 | 72 | 229 | 1,718 | 22.24% |
| MID | 275 | 89 | 243 | 3,185 | 13.66% |
| ADC | 158 | 59 | 108 | 1,469 | 14.83% |
| SUPPORT | 183 | 59 | 243 | 2,528 | 14.14% |

### B_n20_l5

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 255 | 46 | 357 | 3,988 | 13.17% |
| JUNGLE | 290 | 66 | 228 | 1,718 | 22.50% |
| MID | 266 | 86 | 255 | 3,185 | 13.74% |
| ADC | 138 | 88 | 99 | 1,469 | 13.21% |
| SUPPORT | 166 | 98 | 221 | 2,528 | 12.84% |

### D_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 300 | 51 | 307 | 3,988 | 13.07% |
| JUNGLE | 256 | 99 | 229 | 1,718 | 21.07% |
| MID | 266 | 102 | 239 | 3,185 | 13.32% |
| ADC | 148 | 64 | 113 | 1,469 | 14.55% |
| SUPPORT | 158 | 91 | 236 | 2,528 | 13.08% |

### D_n20_l7_5_s15

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 283 | 72 | 303 | 3,988 | 12.61% |
| JUNGLE | 251 | 126 | 207 | 1,718 | 19.90% |
| MID | 266 | 105 | 236 | 3,185 | 13.24% |
| ADC | 146 | 71 | 108 | 1,469 | 14.16% |
| SUPPORT | 160 | 89 | 236 | 2,528 | 13.14% |

## Threshold Sensitivity

Individual metric signal coverage across the complete requested threshold range:

```json
[
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0,
    "Counts": {
      "Total": 2659,
      "SideA": 1328,
      "Neutral": 47,
      "SideB": 1284,
      "Insufficient": 0,
      "Directional": 2612,
      "DirectionalRate": 0.9823241820233171
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0,
    "Counts": {
      "Total": 2659,
      "SideA": 1301,
      "Neutral": 76,
      "SideB": 1282,
      "Insufficient": 0,
      "Directional": 2583,
      "DirectionalRate": 0.9714178262504701
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0,
    "Counts": {
      "Total": 2659,
      "SideA": 1230,
      "Neutral": 99,
      "SideB": 1330,
      "Insufficient": 0,
      "Directional": 2560,
      "DirectionalRate": 0.9627679578789018
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.025,
    "Counts": {
      "Total": 2659,
      "SideA": 1298,
      "Neutral": 102,
      "SideB": 1259,
      "Insufficient": 0,
      "Directional": 2557,
      "DirectionalRate": 0.9616397141782626
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.025,
    "Counts": {
      "Total": 2659,
      "SideA": 1264,
      "Neutral": 154,
      "SideB": 1241,
      "Insufficient": 0,
      "Directional": 2505,
      "DirectionalRate": 0.9420834900338473
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.025,
    "Counts": {
      "Total": 2659,
      "SideA": 1180,
      "Neutral": 211,
      "SideB": 1268,
      "Insufficient": 0,
      "Directional": 2448,
      "DirectionalRate": 0.9206468597216999
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.05,
    "Counts": {
      "Total": 2659,
      "SideA": 1234,
      "Neutral": 219,
      "SideB": 1206,
      "Insufficient": 0,
      "Directional": 2440,
      "DirectionalRate": 0.9176382098533283
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.05,
    "Counts": {
      "Total": 2659,
      "SideA": 1146,
      "Neutral": 363,
      "SideB": 1150,
      "Insufficient": 0,
      "Directional": 2296,
      "DirectionalRate": 0.86348251222264
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.05,
    "Counts": {
      "Total": 2659,
      "SideA": 1047,
      "Neutral": 452,
      "SideB": 1160,
      "Insufficient": 0,
      "Directional": 2207,
      "DirectionalRate": 0.8300112824370064
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.075,
    "Counts": {
      "Total": 2659,
      "SideA": 1193,
      "Neutral": 304,
      "SideB": 1162,
      "Insufficient": 0,
      "Directional": 2355,
      "DirectionalRate": 0.8856713050018804
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.075,
    "Counts": {
      "Total": 2659,
      "SideA": 1064,
      "Neutral": 519,
      "SideB": 1076,
      "Insufficient": 0,
      "Directional": 2140,
      "DirectionalRate": 0.8048138397893945
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.075,
    "Counts": {
      "Total": 2659,
      "SideA": 971,
      "Neutral": 636,
      "SideB": 1052,
      "Insufficient": 0,
      "Directional": 2023,
      "DirectionalRate": 0.7608123354644604
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.1,
    "Counts": {
      "Total": 2659,
      "SideA": 1149,
      "Neutral": 397,
      "SideB": 1113,
      "Insufficient": 0,
      "Directional": 2262,
      "DirectionalRate": 0.8506957502820609
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.1,
    "Counts": {
      "Total": 2659,
      "SideA": 970,
      "Neutral": 695,
      "SideB": 994,
      "Insufficient": 0,
      "Directional": 1964,
      "DirectionalRate": 0.73862354268522
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.1,
    "Counts": {
      "Total": 2659,
      "SideA": 867,
      "Neutral": 825,
      "SideB": 967,
      "Insufficient": 0,
      "Directional": 1834,
      "DirectionalRate": 0.689732982324182
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0,
    "Counts": {
      "Total": 2659,
      "SideA": 1235,
      "Neutral": 159,
      "SideB": 1265,
      "Insufficient": 0,
      "Directional": 2500,
      "DirectionalRate": 0.9402030838661151
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.05,
    "Counts": {
      "Total": 2659,
      "SideA": 994,
      "Neutral": 696,
      "SideB": 969,
      "Insufficient": 0,
      "Directional": 1963,
      "DirectionalRate": 0.7382474614516735
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.1,
    "Counts": {
      "Total": 2659,
      "SideA": 804,
      "Neutral": 1123,
      "SideB": 732,
      "Insufficient": 0,
      "Directional": 1536,
      "DirectionalRate": 0.5776607747273411
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.15,
    "Counts": {
      "Total": 2659,
      "SideA": 640,
      "Neutral": 1432,
      "SideB": 587,
      "Insufficient": 0,
      "Directional": 1227,
      "DirectionalRate": 0.4614516735614893
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.2,
    "Counts": {
      "Total": 2659,
      "SideA": 535,
      "Neutral": 1644,
      "SideB": 480,
      "Insufficient": 0,
      "Directional": 1015,
      "DirectionalRate": 0.3817224520496427
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.3,
    "Counts": {
      "Total": 2659,
      "SideA": 401,
      "Neutral": 1922,
      "SideB": 336,
      "Insufficient": 0,
      "Directional": 737,
      "DirectionalRate": 0.2771718691237307
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0,
    "Counts": {
      "Total": 1170,
      "SideA": 601,
      "Neutral": 9,
      "SideB": 560,
      "Insufficient": 0,
      "Directional": 1161,
      "DirectionalRate": 0.9923076923076923
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0,
    "Counts": {
      "Total": 1170,
      "SideA": 608,
      "Neutral": 20,
      "SideB": 542,
      "Insufficient": 0,
      "Directional": 1150,
      "DirectionalRate": 0.9829059829059829
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0,
    "Counts": {
      "Total": 1170,
      "SideA": 576,
      "Neutral": 28,
      "SideB": 566,
      "Insufficient": 0,
      "Directional": 1142,
      "DirectionalRate": 0.976068376068376
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1170,
      "SideA": 578,
      "Neutral": 55,
      "SideB": 537,
      "Insufficient": 0,
      "Directional": 1115,
      "DirectionalRate": 0.9529914529914529
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1170,
      "SideA": 578,
      "Neutral": 81,
      "SideB": 511,
      "Insufficient": 0,
      "Directional": 1089,
      "DirectionalRate": 0.9307692307692308
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1170,
      "SideA": 537,
      "Neutral": 118,
      "SideB": 515,
      "Insufficient": 0,
      "Directional": 1052,
      "DirectionalRate": 0.8991452991452992
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1170,
      "SideA": 551,
      "Neutral": 102,
      "SideB": 517,
      "Insufficient": 0,
      "Directional": 1068,
      "DirectionalRate": 0.9128205128205128
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1170,
      "SideA": 523,
      "Neutral": 175,
      "SideB": 472,
      "Insufficient": 0,
      "Directional": 995,
      "DirectionalRate": 0.8504273504273504
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1170,
      "SideA": 482,
      "Neutral": 220,
      "SideB": 468,
      "Insufficient": 0,
      "Directional": 950,
      "DirectionalRate": 0.811965811965812
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1170,
      "SideA": 524,
      "Neutral": 155,
      "SideB": 491,
      "Insufficient": 0,
      "Directional": 1015,
      "DirectionalRate": 0.8675213675213675
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1170,
      "SideA": 467,
      "Neutral": 274,
      "SideB": 429,
      "Insufficient": 0,
      "Directional": 896,
      "DirectionalRate": 0.7658119658119659
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1170,
      "SideA": 444,
      "Neutral": 333,
      "SideB": 393,
      "Insufficient": 0,
      "Directional": 837,
      "DirectionalRate": 0.7153846153846154
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1170,
      "SideA": 504,
      "Neutral": 197,
      "SideB": 469,
      "Insufficient": 0,
      "Directional": 973,
      "DirectionalRate": 0.8316239316239317
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1170,
      "SideA": 422,
      "Neutral": 363,
      "SideB": 385,
      "Insufficient": 0,
      "Directional": 807,
      "DirectionalRate": 0.6897435897435897
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1170,
      "SideA": 385,
      "Neutral": 429,
      "SideB": 356,
      "Insufficient": 0,
      "Directional": 741,
      "DirectionalRate": 0.6333333333333333
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0,
    "Counts": {
      "Total": 1170,
      "SideA": 572,
      "Neutral": 47,
      "SideB": 551,
      "Insufficient": 0,
      "Directional": 1123,
      "DirectionalRate": 0.9598290598290599
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1170,
      "SideA": 422,
      "Neutral": 375,
      "SideB": 373,
      "Insufficient": 0,
      "Directional": 795,
      "DirectionalRate": 0.6794871794871795
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1170,
      "SideA": 325,
      "Neutral": 578,
      "SideB": 267,
      "Insufficient": 0,
      "Directional": 592,
      "DirectionalRate": 0.505982905982906
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.15,
    "Counts": {
      "Total": 1170,
      "SideA": 252,
      "Neutral": 703,
      "SideB": 215,
      "Insufficient": 0,
      "Directional": 467,
      "DirectionalRate": 0.39914529914529917
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.2,
    "Counts": {
      "Total": 1170,
      "SideA": 213,
      "Neutral": 790,
      "SideB": 167,
      "Insufficient": 0,
      "Directional": 380,
      "DirectionalRate": 0.3247863247863248
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.3,
    "Counts": {
      "Total": 1170,
      "SideA": 158,
      "Neutral": 905,
      "SideB": 107,
      "Insufficient": 0,
      "Directional": 265,
      "DirectionalRate": 0.2264957264957265
    }
  }
]
```

Combined de-correlated model sensitivity:

```json
[
  {
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.05,
    "Counts": {
      "Total": 15547,
      "SideA": 1161,
      "Neutral": 323,
      "SideB": 1175,
      "Insufficient": 12888,
      "Directional": 2336,
      "DirectionalRate": 0.1502540683089985
    },
    "KeyDirectionalCoverage": 0.1502540683089985,
    "SampleWeightedDirectionalCoverage": 0.7013340045305815
  },
  {
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 15547,
      "SideA": 1151,
      "Neutral": 363,
      "SideB": 1145,
      "Insufficient": 12888,
      "Directional": 2296,
      "DirectionalRate": 0.14768122467357048
    },
    "KeyDirectionalCoverage": 0.14768122467357048,
    "SampleWeightedDirectionalCoverage": 0.6946117059381595
  },
  {
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 15547,
      "SideA": 1108,
      "Neutral": 451,
      "SideB": 1100,
      "Insufficient": 12888,
      "Directional": 2208,
      "DirectionalRate": 0.14202096867562874
    },
    "KeyDirectionalCoverage": 0.14202096867562874,
    "SampleWeightedDirectionalCoverage": 0.6783480803113323
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.05,
    "Counts": {
      "Total": 15547,
      "SideA": 1131,
      "Neutral": 372,
      "SideB": 1156,
      "Insufficient": 12888,
      "Directional": 2287,
      "DirectionalRate": 0.14710233485559915
    },
    "KeyDirectionalCoverage": 0.14710233485559915,
    "SampleWeightedDirectionalCoverage": 0.6739840074348002
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 15547,
      "SideA": 1128,
      "Neutral": 407,
      "SideB": 1124,
      "Insufficient": 12888,
      "Directional": 2252,
      "DirectionalRate": 0.1448510966745996
    },
    "KeyDirectionalCoverage": 0.1448510966745996,
    "SampleWeightedDirectionalCoverage": 0.6819725454510251
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 15547,
      "SideA": 1092,
      "Neutral": 478,
      "SideB": 1089,
      "Insufficient": 12888,
      "Directional": 2181,
      "DirectionalRate": 0.1402842992217148
    },
    "KeyDirectionalCoverage": 0.1402842992217148,
    "SampleWeightedDirectionalCoverage": 0.6712424248291351
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.05,
    "Counts": {
      "Total": 15547,
      "SideA": 1119,
      "Neutral": 409,
      "SideB": 1131,
      "Insufficient": 12888,
      "Directional": 2250,
      "DirectionalRate": 0.1447224544928282
    },
    "KeyDirectionalCoverage": 0.1447224544928282,
    "SampleWeightedDirectionalCoverage": 0.6593157660361285
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 15547,
      "SideA": 1121,
      "Neutral": 432,
      "SideB": 1106,
      "Insufficient": 12888,
      "Directional": 2227,
      "DirectionalRate": 0.14324306940245707
    },
    "KeyDirectionalCoverage": 0.14324306940245707,
    "SampleWeightedDirectionalCoverage": 0.6751456949795737
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 15547,
      "SideA": 1092,
      "Neutral": 493,
      "SideB": 1074,
      "Insufficient": 12888,
      "Directional": 2166,
      "DirectionalRate": 0.13931948285842927
    },
    "KeyDirectionalCoverage": 0.13931948285842927,
    "SampleWeightedDirectionalCoverage": 0.668977134116827
  }
]
```

## High-sample Subset Comparison

```json
[
  {
    "MinimumSample": 20,
    "Counts": {
      "Total": 2659,
      "SideA": 1128,
      "Neutral": 407,
      "SideB": 1124,
      "Insufficient": 0,
      "Directional": 2252,
      "DirectionalRate": 0.8469349379465965
    }
  },
  {
    "MinimumSample": 50,
    "Counts": {
      "Total": 1170,
      "SideA": 522,
      "Neutral": 185,
      "SideB": 463,
      "Insufficient": 0,
      "Directional": 985,
      "DirectionalRate": 0.8418803418803419
    }
  },
  {
    "MinimumSample": 100,
    "Counts": {
      "Total": 515,
      "SideA": 231,
      "Neutral": 96,
      "SideB": 188,
      "Insufficient": 0,
      "Directional": 419,
      "DirectionalRate": 0.8135922330097087
    }
  }
]
```

## Split Level Tempo / Combat Pressure Research

```json
[
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.05,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1246,
      "Neutral": 171,
      "SideB": 1242,
      "Insufficient": 12888,
      "Directional": 2488,
      "DirectionalRate": 0.16003087412362513
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 994,
      "Neutral": 696,
      "SideB": 969,
      "Insufficient": 12888,
      "Directional": 1963,
      "DirectionalRate": 0.1262623014086319
    },
    "SameDirectionAmongBothDirectional": 0.5610151187904968,
    "ConflictAmongBothDirectional": 0.43898488120950324,
    "OneNeutralRate": 0.2809326814591952,
    "BothNeutralRate": 0.022564874012786763
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1246,
      "Neutral": 171,
      "SideB": 1242,
      "Insufficient": 12888,
      "Directional": 2488,
      "DirectionalRate": 0.16003087412362513
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 804,
      "Neutral": 1123,
      "SideB": 732,
      "Insufficient": 12888,
      "Directional": 1536,
      "DirectionalRate": 0.09879719560043738
    },
    "SameDirectionAmongBothDirectional": 0.5648592999313659,
    "ConflictAmongBothDirectional": 0.4351407000686342,
    "OneNeutralRate": 0.4174501692365551,
    "BothNeutralRate": 0.034599473486273034
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1246,
      "Neutral": 171,
      "SideB": 1242,
      "Insufficient": 12888,
      "Directional": 2488,
      "DirectionalRate": 0.16003087412362513
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 640,
      "Neutral": 1432,
      "SideB": 587,
      "Insufficient": 12888,
      "Directional": 1227,
      "DirectionalRate": 0.07892197851675564
    },
    "SameDirectionAmongBothDirectional": 0.58147512864494,
    "ConflictAmongBothDirectional": 0.41852487135506006,
    "OneNeutralRate": 0.5201203459947349,
    "BothNeutralRate": 0.04136893569010906
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1246,
      "Neutral": 171,
      "SideB": 1242,
      "Insufficient": 12888,
      "Directional": 2488,
      "DirectionalRate": 0.16003087412362513
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 535,
      "Neutral": 1644,
      "SideB": 480,
      "Insufficient": 12888,
      "Directional": 1015,
      "DirectionalRate": 0.06528590724898695
    },
    "SameDirectionAmongBothDirectional": 0.5985552115583075,
    "ConflictAmongBothDirectional": 0.40144478844169246,
    "OneNeutralRate": 0.588567130500188,
    "BothNeutralRate": 0.04701015419330575
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.05,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1115,
      "Neutral": 384,
      "SideB": 1160,
      "Insufficient": 12888,
      "Directional": 2275,
      "DirectionalRate": 0.14633048176497074
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 994,
      "Neutral": 696,
      "SideB": 969,
      "Insufficient": 12888,
      "Directional": 1963,
      "DirectionalRate": 0.1262623014086319
    },
    "SameDirectionAmongBothDirectional": 0.5618043350908026,
    "ConflictAmongBothDirectional": 0.4381956649091974,
    "OneNeutralRate": 0.3098909364422715,
    "BothNeutralRate": 0.04813839789394509
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1115,
      "Neutral": 384,
      "SideB": 1160,
      "Insufficient": 12888,
      "Directional": 2275,
      "DirectionalRate": 0.14633048176497074
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 804,
      "Neutral": 1123,
      "SideB": 732,
      "Insufficient": 12888,
      "Directional": 1536,
      "DirectionalRate": 0.09879719560043738
    },
    "SameDirectionAmongBothDirectional": 0.5694135115070527,
    "ConflictAmongBothDirectional": 0.4305864884929473,
    "OneNeutralRate": 0.42008273787138023,
    "BothNeutralRate": 0.07333584054155698
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1115,
      "Neutral": 384,
      "SideB": 1160,
      "Insufficient": 12888,
      "Directional": 2275,
      "DirectionalRate": 0.14633048176497074
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 640,
      "Neutral": 1432,
      "SideB": 587,
      "Insufficient": 12888,
      "Directional": 1227,
      "DirectionalRate": 0.07892197851675564
    },
    "SameDirectionAmongBothDirectional": 0.5883441258094357,
    "ConflictAmongBothDirectional": 0.4116558741905643,
    "OneNeutralRate": 0.5039488529522377,
    "BothNeutralRate": 0.08950733358405416
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1115,
      "Neutral": 384,
      "SideB": 1160,
      "Insufficient": 12888,
      "Directional": 2275,
      "DirectionalRate": 0.14633048176497074
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 535,
      "Neutral": 1644,
      "SideB": 480,
      "Insufficient": 12888,
      "Directional": 1015,
      "DirectionalRate": 0.06528590724898695
    },
    "SameDirectionAmongBothDirectional": 0.6035437430786268,
    "ConflictAmongBothDirectional": 0.3964562569213732,
    "OneNeutralRate": 0.558104550582926,
    "BothNeutralRate": 0.10229409552463332
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.05,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1029,
      "Neutral": 543,
      "SideB": 1087,
      "Insufficient": 12888,
      "Directional": 2116,
      "DirectionalRate": 0.1361034283141442
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 994,
      "Neutral": 696,
      "SideB": 969,
      "Insufficient": 12888,
      "Directional": 1963,
      "DirectionalRate": 0.1262623014086319
    },
    "SameDirectionAmongBothDirectional": 0.5647571606475716,
    "ConflictAmongBothDirectional": 0.4352428393524284,
    "OneNeutralRate": 0.3260624294847687,
    "BothNeutralRate": 0.06995110943963896
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1029,
      "Neutral": 543,
      "SideB": 1087,
      "Insufficient": 12888,
      "Directional": 2116,
      "DirectionalRate": 0.1361034283141442
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 804,
      "Neutral": 1123,
      "SideB": 732,
      "Insufficient": 12888,
      "Directional": 1536,
      "DirectionalRate": 0.09879719560043738
    },
    "SameDirectionAmongBothDirectional": 0.5736677115987461,
    "ConflictAmongBothDirectional": 0.4263322884012539,
    "OneNeutralRate": 0.41368935690109065,
    "BothNeutralRate": 0.10643098909364422
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1029,
      "Neutral": 543,
      "SideB": 1087,
      "Insufficient": 12888,
      "Directional": 2116,
      "DirectionalRate": 0.1361034283141442
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 640,
      "Neutral": 1432,
      "SideB": 587,
      "Insufficient": 12888,
      "Directional": 1227,
      "DirectionalRate": 0.07892197851675564
    },
    "SameDirectionAmongBothDirectional": 0.5928085519922255,
    "ConflictAmongBothDirectional": 0.4071914480077745,
    "OneNeutralRate": 0.48326438510718317,
    "BothNeutralRate": 0.12974802557352388
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 15547,
      "SideA": 1029,
      "Neutral": 543,
      "SideB": 1087,
      "Insufficient": 12888,
      "Directional": 2116,
      "DirectionalRate": 0.1361034283141442
    },
    "CombatCounts": {
      "Total": 15547,
      "SideA": 535,
      "Neutral": 1644,
      "SideB": 480,
      "Insufficient": 12888,
      "Directional": 1015,
      "DirectionalRate": 0.06528590724898695
    },
    "SameDirectionAmongBothDirectional": 0.6106604866743917,
    "ConflictAmongBothDirectional": 0.38933951332560834,
    "OneNeutralRate": 0.5283941331327566,
    "BothNeutralRate": 0.1470477623166604
  }
]
```

## Model Disagreement Cases

```json
{
  "StrongestA": [
    {
      "Role": "TOP",
      "ChampionA": "Qiyana",
      "ChampionB": "Yone",
      "SampleSize": 29,
      "L2Difference": -0.7931034482758621,
      "L3Difference": -0.2413793103448276,
      "L6Difference": 0.1724137931034483,
      "SoloDifference": 2.586206896551724,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Cassiopeia",
      "ChampionB": "Yone",
      "SampleSize": 35,
      "L2Difference": 0.08571428571428572,
      "L3Difference": 0.7142857142857143,
      "L6Difference": 0.6,
      "SoloDifference": 2.342857142857143,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Illaoi",
      "ChampionB": "Nasus",
      "SampleSize": 24,
      "L2Difference": 0.9166666666666666,
      "L3Difference": 1,
      "L6Difference": 0.5833333333333334,
      "SoloDifference": 1.1666666666666667,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "KSante",
      "ChampionB": "Nasus",
      "SampleSize": 171,
      "L2Difference": 0.847953216374269,
      "L3Difference": 0.8245614035087719,
      "L6Difference": 0.2982456140350877,
      "SoloDifference": 1.631578947368421,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Kayle",
      "SampleSize": 22,
      "L2Difference": 0.7272727272727273,
      "L3Difference": 0.5454545454545454,
      "L6Difference": 0.8181818181818182,
      "SoloDifference": 1.3181818181818181,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Heimerdinger",
      "ChampionB": "Nasus",
      "SampleSize": 31,
      "L2Difference": 0.8709677419354839,
      "L3Difference": 0.8709677419354839,
      "L6Difference": 0.6129032258064516,
      "SoloDifference": 0.967741935483871,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jax",
      "ChampionB": "Nasus",
      "SampleSize": 106,
      "L2Difference": 0.8867924528301887,
      "L3Difference": 0.9056603773584906,
      "L6Difference": 0.5660377358490566,
      "SoloDifference": 0.9433962264150944,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Kled",
      "ChampionB": "Nasus",
      "SampleSize": 51,
      "L2Difference": 0.7254901960784313,
      "L3Difference": 0.8431372549019608,
      "L6Difference": 0.5098039215686274,
      "SoloDifference": 1.1372549019607843,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Kled",
      "ChampionB": "Vayne",
      "SampleSize": 24,
      "L2Difference": -0.5833333333333334,
      "L3Difference": -0.6666666666666666,
      "L6Difference": 0.08333333333333333,
      "SoloDifference": 1.875,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Malphite",
      "ChampionB": "Vayne",
      "SampleSize": 50,
      "L2Difference": -0.68,
      "L3Difference": -0.64,
      "L6Difference": 0.44,
      "SoloDifference": 1.42,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    }
  ],
  "StrongestB": [
    {
      "Role": "TOP",
      "ChampionA": "Camille",
      "ChampionB": "Warwick",
      "SampleSize": 22,
      "L2Difference": -0.7272727272727273,
      "L3Difference": -0.5454545454545454,
      "L6Difference": -0.45454545454545453,
      "SoloDifference": -1.7272727272727273,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Gwen",
      "ChampionB": "Rumble",
      "SampleSize": 22,
      "L2Difference": 1,
      "L3Difference": 0.8181818181818182,
      "L6Difference": -0.6363636363636364,
      "SoloDifference": -0.8181818181818182,
      "CombinedResult": "SideB",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "DrMundo",
      "ChampionB": "Heimerdinger",
      "SampleSize": 22,
      "L2Difference": -1,
      "L3Difference": -1,
      "L6Difference": -0.45454545454545453,
      "SoloDifference": -0.7272727272727273,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Tryndamere",
      "SampleSize": 44,
      "L2Difference": -0.8181818181818182,
      "L3Difference": -0.8181818181818182,
      "L6Difference": -0.7727272727272727,
      "SoloDifference": -0.75,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Garen",
      "ChampionB": "Kled",
      "SampleSize": 21,
      "L2Difference": -0.6190476190476191,
      "L3Difference": -0.7142857142857143,
      "L6Difference": -0.3333333333333333,
      "SoloDifference": -1.4761904761904763,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Ornn",
      "ChampionB": "Yone",
      "SampleSize": 43,
      "L2Difference": -0.7674418604651163,
      "L3Difference": -0.627906976744186,
      "L6Difference": -0.627906976744186,
      "SoloDifference": -1.0930232558139534,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Warwick",
      "SampleSize": 20,
      "L2Difference": -0.8,
      "L3Difference": -0.8,
      "L6Difference": -0.5,
      "SoloDifference": -1,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Yone",
      "SampleSize": 153,
      "L2Difference": -0.7908496732026143,
      "L3Difference": -0.7254901960784313,
      "L6Difference": -0.5816993464052288,
      "SoloDifference": -0.9934640522875817,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Varus",
      "SampleSize": 30,
      "L2Difference": -0.9333333333333333,
      "L3Difference": -0.7333333333333333,
      "L6Difference": -0.26666666666666666,
      "SoloDifference": -1.1,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Quinn",
      "SampleSize": 20,
      "L2Difference": -1,
      "L3Difference": -0.9,
      "L6Difference": -0.3,
      "SoloDifference": -0.8,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    }
  ],
  "Neutral": [
    {
      "Role": "ADC",
      "ChampionA": "Lucian",
      "ChampionB": "Yunara",
      "SampleSize": 618,
      "L2Difference": -0.20388349514563106,
      "L3Difference": 0.03559870550161812,
      "L6Difference": 0.1423948220064725,
      "SoloDifference": 0.008090614886731391,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Nautilus",
      "ChampionB": "Rell",
      "SampleSize": 572,
      "L2Difference": -0.1048951048951049,
      "L3Difference": 0.015734265734265736,
      "L6Difference": 0.07342657342657342,
      "SoloDifference": 0.008741258741258742,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Alistar",
      "ChampionB": "Rell",
      "SampleSize": 487,
      "L2Difference": -0.3326488706365503,
      "L3Difference": 0.055441478439425054,
      "L6Difference": 0.006160164271047228,
      "SoloDifference": -0.01026694045174538,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Bard",
      "ChampionB": "Lulu",
      "SampleSize": 461,
      "L2Difference": 0.1561822125813449,
      "L3Difference": 0.15835140997830802,
      "L6Difference": -0.07158351409978309,
      "SoloDifference": -0.0021691973969631237,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "MID",
      "ChampionA": "Syndra",
      "ChampionB": "Viktor",
      "SampleSize": 444,
      "L2Difference": -0.39414414414414417,
      "L3Difference": -0.11936936936936937,
      "L6Difference": 0.02252252252252252,
      "SoloDifference": 0.14414414414414414,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Olaf",
      "SampleSize": 367,
      "L2Difference": -0.12806539509536785,
      "L3Difference": 0.05722070844686648,
      "L6Difference": 0.13896457765667575,
      "SoloDifference": -0.5204359673024523,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Malphite",
      "SampleSize": 362,
      "L2Difference": 0.8922651933701657,
      "L3Difference": 0.6988950276243094,
      "L6Difference": 0.049723756906077346,
      "SoloDifference": -0.4558011049723757,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "JUNGLE",
      "ChampionA": "Qiyana",
      "ChampionB": "Talon",
      "SampleSize": 315,
      "L2Difference": -0.20634920634920634,
      "L3Difference": 0.08888888888888889,
      "L6Difference": -0.022222222222222223,
      "SoloDifference": -0.08888888888888889,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Bard",
      "ChampionB": "Karma",
      "SampleSize": 306,
      "L2Difference": 0.02287581699346405,
      "L3Difference": -0.026143790849673203,
      "L6Difference": -0.0196078431372549,
      "SoloDifference": -0.006535947712418301,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "MID",
      "ChampionA": "Galio",
      "ChampionB": "Locke",
      "SampleSize": 288,
      "L2Difference": -0.08680555555555555,
      "L3Difference": 0.06597222222222222,
      "L6Difference": 0.1111111111111111,
      "SoloDifference": -0.3090277777777778,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    }
  ],
  "LevelCombatConflicts": [
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "KSante",
      "SampleSize": 457,
      "L2Difference": 0.8774617067833698,
      "L3Difference": 0.8096280087527352,
      "L6Difference": 0.061269146608315096,
      "SoloDifference": -0.5995623632385121,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "MID",
      "ChampionA": "Syndra",
      "ChampionB": "Viktor",
      "SampleSize": 444,
      "L2Difference": -0.39414414414414417,
      "L3Difference": -0.11936936936936937,
      "L6Difference": 0.02252252252252252,
      "SoloDifference": 0.14414414414414414,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Olaf",
      "SampleSize": 367,
      "L2Difference": -0.12806539509536785,
      "L3Difference": 0.05722070844686648,
      "L6Difference": 0.13896457765667575,
      "SoloDifference": -0.5204359673024523,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Malphite",
      "SampleSize": 362,
      "L2Difference": 0.8922651933701657,
      "L3Difference": 0.6988950276243094,
      "L6Difference": 0.049723756906077346,
      "SoloDifference": -0.4558011049723757,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "MID",
      "ChampionA": "Galio",
      "ChampionB": "Locke",
      "SampleSize": 288,
      "L2Difference": -0.08680555555555555,
      "L3Difference": 0.06597222222222222,
      "L6Difference": 0.1111111111111111,
      "SoloDifference": -0.3090277777777778,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Ezreal",
      "ChampionB": "Tristana",
      "SampleSize": 286,
      "L2Difference": 0.3076923076923077,
      "L3Difference": 0,
      "L6Difference": 0.13286713286713286,
      "SoloDifference": -0.14335664335664336,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Jinx",
      "ChampionB": "Zeri",
      "SampleSize": 278,
      "L2Difference": 0.38848920863309355,
      "L3Difference": 0.02877697841726619,
      "L6Difference": 0.07553956834532374,
      "SoloDifference": -0.10431654676258993,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "MID",
      "ChampionA": "Sylas",
      "ChampionB": "TwistedFate",
      "SampleSize": 268,
      "L2Difference": -0.48134328358208955,
      "L3Difference": -0.22388059701492538,
      "L6Difference": 0.029850746268656716,
      "SoloDifference": 0.208955223880597,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "MID",
      "ChampionA": "Akali",
      "ChampionB": "Viktor",
      "SampleSize": 266,
      "L2Difference": -0.5864661654135338,
      "L3Difference": -0.08646616541353383,
      "L6Difference": 0.14285714285714285,
      "SoloDifference": 0.39097744360902253,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "ADC",
      "ChampionA": "Corki",
      "ChampionB": "Yunara",
      "SampleSize": 264,
      "L2Difference": -0.17424242424242425,
      "L3Difference": -0.06060606060606061,
      "L6Difference": 0.06818181818181818,
      "SoloDifference": 0.14393939393939395,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    }
  ],
  "CombinedModelDisagreements": [
    {
      "Role": "SUPPORT",
      "ChampionA": "Bard",
      "ChampionB": "Lulu",
      "SampleSize": 461,
      "L2Difference": 0.1561822125813449,
      "L3Difference": 0.15835140997830802,
      "L6Difference": -0.07158351409978309,
      "SoloDifference": -0.0021691973969631237,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "MID",
      "ChampionA": "Syndra",
      "ChampionB": "Viktor",
      "SampleSize": 444,
      "L2Difference": -0.39414414414414417,
      "L3Difference": -0.11936936936936937,
      "L6Difference": 0.02252252252252252,
      "SoloDifference": 0.14414414414414414,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Malphite",
      "SampleSize": 362,
      "L2Difference": 0.8922651933701657,
      "L3Difference": 0.6988950276243094,
      "L6Difference": 0.049723756906077346,
      "SoloDifference": -0.4558011049723757,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Nautilus",
      "ChampionB": "Pyke",
      "SampleSize": 287,
      "L2Difference": -0.08013937282229965,
      "L3Difference": -0.0627177700348432,
      "L6Difference": 0.08362369337979095,
      "SoloDifference": 0.003484320557491289,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "MID",
      "ChampionA": "Sylas",
      "ChampionB": "TwistedFate",
      "SampleSize": 268,
      "L2Difference": -0.48134328358208955,
      "L3Difference": -0.22388059701492538,
      "L6Difference": 0.029850746268656716,
      "SoloDifference": 0.208955223880597,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "MID",
      "ChampionA": "Akali",
      "ChampionB": "Viktor",
      "SampleSize": 266,
      "L2Difference": -0.5864661654135338,
      "L3Difference": -0.08646616541353383,
      "L6Difference": 0.14285714285714285,
      "SoloDifference": 0.39097744360902253,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "ADC",
      "ChampionA": "Corki",
      "ChampionB": "Yunara",
      "SampleSize": 264,
      "L2Difference": -0.17424242424242425,
      "L3Difference": -0.06060606060606061,
      "L6Difference": 0.06818181818181818,
      "SoloDifference": 0.14393939393939395,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jax",
      "ChampionB": "Olaf",
      "SampleSize": 242,
      "L2Difference": 0.15702479338842976,
      "L3Difference": 0.1322314049586777,
      "L6Difference": -0.14049586776859505,
      "SoloDifference": -0.024793388429752067,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Lulu",
      "ChampionB": "Yuumi",
      "SampleSize": 237,
      "L2Difference": 0.5991561181434599,
      "L3Difference": 0.0970464135021097,
      "L6Difference": -0.05485232067510549,
      "SoloDifference": 0,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "MID",
      "ChampionA": "Sylas",
      "ChampionB": "Syndra",
      "SampleSize": 225,
      "L2Difference": -0.39111111111111113,
      "L3Difference": -0.10222222222222223,
      "L6Difference": 0.04,
      "SoloDifference": 0.35555555555555557,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    }
  ]
}
```

## Outcome Diagnostic Comparison

Outcome is not an input. Opposite directions are valid observations, not errors.

```json
{
  "ModelId": "D_n20_l5_s10",
  "DirectionalLaneResults": 2252,
  "SameDirectionalOutcome": 313,
  "NeutralOutcome": 1712,
  "OppositeDirectionalOutcome": 227,
  "InsufficientOutcome": 0
}
```

## Candidate Shortlist

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_l5_s10 | 1,136 | 400 | 1,123 | 12,888 | 14.53% | 69.65% | 44.0 | True |
| B_n20_l5 | 1,115 | 384 | 1,160 | 12,888 | 14.63% | 67.67% | 43.0 | True |
| D_n20_l5_s10 | 1,128 | 407 | 1,124 | 12,888 | 14.49% | 68.20% | 43.0 | True |
| D_n20_l7_5_s15 | 1,106 | 463 | 1,090 | 12,888 | 14.12% | 67.17% | 44.0 | True |

## Single-vs-Split Recommendation

**B_LEVEL_TEMPO_AND_COMBAT_PRESSURE_SPLIT**

- L2 and L3 are correlated and should not be counted as fully independent votes.
- Solo kills are event frequency and represent combat pressure rather than level tempo.
- Keeping Level Tempo and Combat Pressure separate preserves observed disagreement instead of hiding it in one score.
- Use categorical tendencies with Neutral and InsufficientEvidence; do not create an arbitrary numeric score.

## Recommended Production Rule(s)

- Minimum sample: 20
- Level Tempo: L2/L3/L6 threshold 5.0%; at least two same-direction signals; otherwise Neutral.
- Combat Pressure: solo-kill event-rate difference threshold 0.10 per match; otherwise Neutral.
- Below minimum sample: InsufficientEvidence for both axes.
- Do not combine the two outputs into one lane-advantage label.

## Terminology Recommendation

- Axes: 레벨 템포 / 교전 압박
- States: 경향 / 뚜렷한 차이 없음 / 판정 보류
- For all-role UI, `초반 템포` is safer than claiming definitive `라인전 우세`, especially for JUNGLE and SUPPORT.

## Limitations

- Aggregate data cannot provide a true train/test split or per-match stability test.
- Observed metrics omit CS, gold, XP differences, turret plates, objectives, roams, damage, assists, and third-party deaths.
- JUNGLE and SUPPORT are not always well described by the term lane pressure.
- Role-specific rules were not fitted; all roles use the same research candidates.
- Outcome agreement is diagnostic only and disagreement is not treated as model error.

## Next Stage

If production work proceeds, implement separate Level Tempo and Combat Pressure classifiers only after reviewing this report. No production code was changed by this research.
