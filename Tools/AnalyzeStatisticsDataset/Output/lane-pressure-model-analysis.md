# Lane Pressure Model Research

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 31,075
- Observations: 155,375
- Matchup keys: 13,021
- Scope: `MASTER_PLUS_SEEDED`; region `KR`; queue 420
- Minimum duration: 900s; revision: 1788710852

## Research Scope

Research whether first-level timing and direct-opponent solo-kill frequency describe one coherent early-pressure concept.
This is research only. No production Lane Pressure classifier or UI is implemented.

## Metric Definitions

- Level differences: (FirstLevelForA - FirstLevelForB) / SampleSize; Unknown remains in the SampleSize denominator.
- Solo difference: SoloKillsBefore15ForA / SampleSize - SoloKillsBefore15ForB / SampleSize; EVENT_COUNT per match, not probability.
- Outcome separation: Game wins and OutcomeAdvantage are excluded from every lane-pressure model input.

## Metric Difference Distributions

### ALL (keys 13,021)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0072 | 0.0000 | 0.7723 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0112 | 0.0000 | 0.7434 | -1.0000 | -0.8333 | 0.7143 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0051 | 0.0000 | 0.7202 | -1.0000 | -0.6000 | 0.6000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0129 | 0.0000 | 1.0890 | -1.0000 | -0.2500 | 0.2667 | 1.0000 | 2.0000 | -10.0000 | 8.0000 |
| Level Unknown | 0.0014 | 0.0000 | 0.0144 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

## Role Comparison

### TOP (keys 3,853)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0507 | 0.0000 | 0.8082 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0301 | 0.0000 | 0.7928 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0136 | 0.0000 | 0.7341 | -1.0000 | -0.6000 | 0.8000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0267 | 0.0000 | 1.6278 | -2.0000 | -0.9310 | 1.0000 | 2.0000 | 3.0000 | -10.0000 | 8.0000 |
| Level Unknown | 0.0007 | 0.0000 | 0.0091 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### JUNGLE (keys 2,025)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0197 | 0.0000 | 0.6845 | -1.0000 | -0.6000 | 0.5000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0001 | 0.0000 | 0.6925 | -1.0000 | -0.5789 | 0.5088 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0107 | 0.0000 | 0.6453 | -1.0000 | -0.3617 | 0.4286 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | -0.0031 | 0.0000 | 0.4479 | -0.4000 | -0.1000 | 0.1000 | 0.3610 | 0.7500 | -4.0000 | 3.0000 |
| Level Unknown | 0.0028 | 0.0000 | 0.0191 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0107 | 0.0000 | 0.3333 |

### MID (keys 3,152)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0044 | 0.0000 | 0.7894 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0100 | 0.0000 | 0.7363 | -1.0000 | -1.0000 | 0.6667 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0010 | 0.0000 | 0.7351 | -1.0000 | -0.6923 | 0.6978 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0223 | 0.0000 | 1.1255 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 2.0000 | -8.0000 | 6.0000 |
| Level Unknown | 0.0017 | 0.0000 | 0.0152 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0026 | 0.0000 | 0.3333 |

### ADC (keys 1,485)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.1246 | 0.2727 | 0.7871 | -1.0000 | -0.7278 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | 0.0177 | 0.0000 | 0.7191 | -1.0000 | -0.6000 | 0.6000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0144 | 0.0000 | 0.7277 | -1.0000 | -0.6000 | 0.6522 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0138 | 0.0000 | 0.6205 | -0.6667 | -0.1111 | 0.1382 | 0.6667 | 1.0000 | -3.0000 | 4.0000 |
| Level Unknown | 0.0008 | 0.0000 | 0.0129 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### SUPPORT (keys 2,506)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0117 | 0.0000 | 0.7430 | -1.0000 | -1.0000 | 0.6667 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0097 | 0.0000 | 0.7269 | -1.0000 | -0.6667 | 0.6000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0661 | 0.0000 | 0.7292 | -1.0000 | -1.0000 | 0.4723 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | -0.0076 | 0.0000 | 0.3207 | -0.2000 | 0.0000 | 0.0000 | 0.1429 | 0.4214 | -3.0000 | 2.0000 |
| Level Unknown | 0.0013 | 0.0000 | 0.0162 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

## Sample-size Volatility

### 1-4 (keys 8,132)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0107 | 0.0000 | 0.8902 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0143 | 0.0000 | 0.8767 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| L6 | -0.0028 | 0.0000 | 0.8671 | -1.0000 | -1.0000 | 1.0000 | 1.0000 | 1.0000 | -1.0000 | 1.0000 |
| Solo | 0.0064 | 0.0000 | 1.3177 | -1.0000 | -0.5000 | 0.5000 | 1.0000 | 2.0000 | -10.0000 | 8.0000 |
| Level Unknown | 0.0013 | 0.0000 | 0.0174 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.3333 |

### 5-9 (keys 1,832)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0032 | 0.0000 | 0.5755 | -0.7143 | -0.5000 | 0.4286 | 0.7500 | 1.0000 | -1.0000 | 1.0000 |
| L3 | -0.0027 | 0.0000 | 0.5252 | -0.7143 | -0.4071 | 0.3333 | 0.6667 | 1.0000 | -1.0000 | 1.0000 |
| L6 | 0.0002 | 0.0000 | 0.4542 | -0.6000 | -0.3333 | 0.3333 | 0.6000 | 0.7143 | -1.0000 | 1.0000 |
| Solo | 0.0276 | 0.0000 | 0.6577 | -0.6625 | -0.2500 | 0.2857 | 0.8000 | 1.1667 | -2.8571 | 4.6000 |
| Level Unknown | 0.0016 | 0.0000 | 0.0093 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.1429 |

### 10-19 (keys 1,272)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | -0.0148 | 0.0000 | 0.5145 | -0.7632 | -0.4229 | 0.4000 | 0.6667 | 0.8182 | -1.0000 | 1.0000 |
| L3 | -0.0266 | -0.0263 | 0.4277 | -0.6000 | -0.2941 | 0.2727 | 0.5385 | 0.6746 | -1.0000 | 1.0000 |
| L6 | -0.0233 | 0.0000 | 0.3429 | -0.4545 | -0.2656 | 0.2222 | 0.4118 | 0.5385 | -1.0000 | 0.8889 |
| Solo | 0.0210 | 0.0000 | 0.4816 | -0.5237 | -0.1778 | 0.2308 | 0.6000 | 0.8439 | -2.3000 | 2.5833 |
| Level Unknown | 0.0013 | 0.0000 | 0.0061 | 0.0000 | 0.0000 | 0.0000 | 0.0000 | 0.0175 | 0.0000 | 0.0667 |

### 20-49 (keys 1,138)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0121 | 0.0253 | 0.4804 | -0.6667 | -0.3636 | 0.3939 | 0.6522 | 0.7696 | -1.0000 | 1.0000 |
| L3 | -0.0016 | 0.0000 | 0.3592 | -0.4783 | -0.2353 | 0.2381 | 0.4675 | 0.5883 | -1.0000 | 1.0000 |
| L6 | -0.0149 | 0.0000 | 0.2751 | -0.3725 | -0.2000 | 0.1724 | 0.3333 | 0.4212 | -0.9130 | 0.7838 |
| Solo | 0.0184 | 0.0000 | 0.3952 | -0.3636 | -0.1495 | 0.1724 | 0.4637 | 0.6957 | -2.2609 | 2.1290 |
| Level Unknown | 0.0017 | 0.0000 | 0.0048 | 0.0000 | 0.0000 | 0.0000 | 0.0085 | 0.0123 | 0.0000 | 0.0435 |

### 50-99 (keys 402)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0086 | 0.0153 | 0.4552 | -0.6388 | -0.3626 | 0.3801 | 0.6346 | 0.7187 | -0.9394 | 0.9444 |
| L3 | 0.0130 | 0.0114 | 0.3352 | -0.4052 | -0.1848 | 0.2302 | 0.4286 | 0.5454 | -0.9524 | 0.9722 |
| L6 | -0.0025 | 0.0120 | 0.2187 | -0.2908 | -0.1463 | 0.1525 | 0.2673 | 0.3333 | -0.6970 | 0.6667 |
| Solo | 0.0198 | 0.0000 | 0.3156 | -0.3029 | -0.0970 | 0.1174 | 0.3324 | 0.6362 | -1.1296 | 1.2400 |
| Level Unknown | 0.0016 | 0.0000 | 0.0028 | 0.0000 | 0.0000 | 0.0036 | 0.0057 | 0.0065 | 0.0000 | 0.0150 |

### 100+ (keys 245)

| Metric | mean | median | stddev | p10 | p25 | p75 | p90 | p95 | min | max |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| L2 | 0.0057 | 0.0000 | 0.3616 | -0.4894 | -0.2459 | 0.2713 | 0.4799 | 0.5652 | -0.8256 | 0.8832 |
| L3 | 0.0229 | 0.0220 | 0.2504 | -0.2472 | -0.1084 | 0.1291 | 0.3327 | 0.4734 | -0.7414 | 0.8200 |
| L6 | 0.0132 | 0.0085 | 0.1540 | -0.1791 | -0.0811 | 0.1247 | 0.2036 | 0.2448 | -0.4679 | 0.4600 |
| Solo | 0.0403 | 0.0028 | 0.2479 | -0.1248 | -0.0370 | 0.0776 | 0.2619 | 0.5179 | -1.0278 | 1.6182 |
| Level Unknown | 0.0016 | 0.0006 | 0.0021 | 0.0000 | 0.0000 | 0.0028 | 0.0044 | 0.0058 | 0.0000 | 0.0112 |

## Level Progression Consistency

| n | threshold | AAA | BBB | consistent | reversal | NNN | mixed/neutral |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 20 | 2.5% | 430 | 398 | 828 | 670 | 1 | 286 |
| 20 | 5.0% | 370 | 346 | 716 | 489 | 6 | 574 |
| 20 | 7.5% | 320 | 294 | 614 | 392 | 17 | 762 |
| 50 | 2.5% | 164 | 137 | 301 | 207 | 0 | 139 |
| 50 | 5.0% | 139 | 119 | 258 | 154 | 1 | 234 |
| 50 | 7.5% | 111 | 99 | 210 | 105 | 8 | 324 |

## Metric Direction Agreement

Agreement is exact categorical agreement (A/Neutral/B), reported separately from Pearson correlation.

| n | level | solo | L2-L3 | L2-L6 | L3-L6 | L2-Solo | L3-Solo | L6-Solo | all same |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 20 | 2.5% | 0.05 | 70.14% | 56.41% | 61.12% | 35.24% | 40.39% | 46.78% | 371 |
| 20 | 2.5% | 0.10 | 70.14% | 56.41% | 61.12% | 28.40% | 33.39% | 39.83% | 296 |
| 20 | 2.5% | 0.20 | 70.14% | 56.41% | 61.12% | 19.78% | 24.54% | 30.03% | 203 |
| 20 | 5.0% | 0.05 | 66.33% | 51.20% | 54.90% | 35.52% | 40.45% | 46.39% | 341 |
| 20 | 5.0% | 0.10 | 66.33% | 51.20% | 54.90% | 29.92% | 34.79% | 42.07% | 276 |
| 20 | 5.0% | 0.20 | 66.33% | 51.20% | 54.90% | 22.24% | 27.96% | 35.24% | 190 |
| 20 | 7.5% | 0.05 | 62.46% | 47.73% | 51.43% | 35.74% | 40.73% | 46.50% | 309 |
| 20 | 7.5% | 0.10 | 62.46% | 47.73% | 51.43% | 30.81% | 37.09% | 43.59% | 256 |
| 20 | 7.5% | 0.20 | 62.46% | 47.73% | 51.43% | 24.09% | 31.99% | 38.49% | 182 |
| 50 | 2.5% | 0.05 | 68.93% | 55.80% | 61.36% | 27.98% | 33.85% | 39.88% | 102 |
| 50 | 2.5% | 0.10 | 68.93% | 55.80% | 61.36% | 20.56% | 26.43% | 31.99% | 71 |
| 50 | 2.5% | 0.20 | 68.93% | 55.80% | 61.36% | 14.22% | 19.47% | 24.42% | 46 |
| 50 | 5.0% | 0.05 | 65.69% | 49.61% | 56.41% | 27.98% | 35.86% | 40.03% | 95 |
| 50 | 5.0% | 0.10 | 65.69% | 49.61% | 56.41% | 22.26% | 30.14% | 34.93% | 67 |
| 50 | 5.0% | 0.20 | 65.69% | 49.61% | 56.41% | 16.38% | 24.73% | 30.45% | 43 |
| 50 | 7.5% | 0.05 | 60.43% | 46.21% | 51.93% | 29.68% | 37.87% | 41.73% | 88 |
| 50 | 7.5% | 0.10 | 60.43% | 46.21% | 51.93% | 24.57% | 35.55% | 37.87% | 65 |
| 50 | 7.5% | 0.20 | 60.43% | 46.21% | 51.93% | 19.63% | 31.99% | 34.93% | 45 |

## Model Definitions

- Model A: four-signal majority vote over L2/L3/L6/Solo.
- Model B: Level Progression majority; at least two of L2/L3/L6 point to one side.
- Model D: L2/L3 are combined into one EarlyLevel signal, then EarlyLevel/L6/Solo vote.
- Model E: Level Tempo (Model B) and Combat Pressure (Solo) remain separate.
- Model F standardized score was not promoted: patch-dependent scaling and arbitrary weighting reduce explainability.

## Parameter Sweep — Model A

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n5_l2_5_s5 | 1,969 | 913 | 2,007 | 8,132 | 30.54% | 76.79% | 13.0 | True |
| A_n5_l2_5_s10 | 2,009 | 848 | 2,032 | 8,132 | 31.03% | 77.34% | 13.0 | True |
| A_n5_l2_5_s15 | 2,046 | 770 | 2,073 | 8,132 | 31.63% | 78.45% | 13.0 | True |
| A_n5_l2_5_s20 | 2,075 | 724 | 2,090 | 8,132 | 31.99% | 79.57% | 13.0 | True |
| A_n5_l5_s5 | 1,979 | 896 | 2,014 | 8,132 | 30.67% | 76.49% | 13.0 | True |
| A_n5_l5_s10 | 2,010 | 847 | 2,032 | 8,132 | 31.04% | 77.28% | 13.0 | True |
| A_n5_l5_s15 | 2,042 | 779 | 2,068 | 8,132 | 31.56% | 78.50% | 13.0 | True |
| A_n5_l5_s20 | 2,062 | 748 | 2,079 | 8,132 | 31.80% | 79.21% | 13.0 | True |
| A_n5_l7_5_s5 | 1,996 | 856 | 2,037 | 8,132 | 30.97% | 76.09% | 13.0 | True |
| A_n5_l7_5_s10 | 2,022 | 814 | 2,053 | 8,132 | 31.30% | 77.70% | 13.0 | True |
| A_n5_l7_5_s15 | 2,045 | 768 | 2,076 | 8,132 | 31.65% | 78.54% | 13.0 | True |
| A_n5_l7_5_s20 | 2,057 | 751 | 2,081 | 8,132 | 31.78% | 78.81% | 13.0 | True |
| A_n10_l2_5_s5 | 1,234 | 574 | 1,249 | 9,964 | 19.07% | 70.47% | 24.0 | True |
| A_n10_l2_5_s10 | 1,274 | 509 | 1,274 | 9,964 | 19.57% | 71.02% | 23.0 | True |
| A_n10_l2_5_s15 | 1,294 | 469 | 1,294 | 9,964 | 19.88% | 71.94% | 24.0 | True |
| A_n10_l2_5_s20 | 1,322 | 415 | 1,320 | 9,964 | 20.29% | 73.09% | 24.0 | True |
| A_n10_l5_s5 | 1,244 | 557 | 1,256 | 9,964 | 19.20% | 70.17% | 24.0 | True |
| A_n10_l5_s10 | 1,275 | 508 | 1,274 | 9,964 | 19.58% | 70.95% | 24.0 | True |
| A_n10_l5_s15 | 1,290 | 478 | 1,289 | 9,964 | 19.81% | 71.99% | 24.0 | True |
| A_n10_l5_s20 | 1,309 | 439 | 1,309 | 9,964 | 20.11% | 72.73% | 23.0 | True |
| A_n10_l7_5_s5 | 1,261 | 517 | 1,279 | 9,964 | 19.51% | 69.77% | 23.0 | True |
| A_n10_l7_5_s10 | 1,287 | 475 | 1,295 | 9,964 | 19.83% | 71.38% | 23.0 | True |
| A_n10_l7_5_s15 | 1,293 | 467 | 1,297 | 9,964 | 19.89% | 72.03% | 23.0 | True |
| A_n10_l7_5_s20 | 1,304 | 442 | 1,311 | 9,964 | 20.08% | 72.33% | 23.0 | True |
| A_n20_l2_5_s5 | 764 | 297 | 724 | 11,236 | 11.43% | 61.73% | 38.0 | True |
| A_n20_l2_5_s10 | 781 | 269 | 735 | 11,236 | 11.64% | 61.94% | 38.0 | True |
| A_n20_l2_5_s15 | 793 | 241 | 751 | 11,236 | 11.86% | 62.73% | 38.0 | True |
| A_n20_l2_5_s20 | 811 | 211 | 763 | 11,236 | 12.09% | 63.66% | 38.0 | True |
| A_n20_l5_s5 | 774 | 280 | 731 | 11,236 | 11.56% | 61.43% | 38.0 | True |
| A_n20_l5_s10 | 782 | 268 | 735 | 11,236 | 11.65% | 61.87% | 38.0 | True |
| A_n20_l5_s15 | 789 | 250 | 746 | 11,236 | 11.79% | 62.79% | 38.0 | True |
| A_n20_l5_s20 | 798 | 235 | 752 | 11,236 | 11.90% | 63.30% | 38.0 | True |
| A_n20_l7_5_s5 | 773 | 277 | 735 | 11,236 | 11.58% | 60.62% | 38.0 | True |
| A_n20_l7_5_s10 | 782 | 264 | 739 | 11,236 | 11.68% | 61.98% | 38.0 | True |
| A_n20_l7_5_s15 | 785 | 257 | 743 | 11,236 | 11.73% | 62.63% | 38.0 | True |
| A_n20_l7_5_s20 | 792 | 245 | 748 | 11,236 | 11.83% | 62.83% | 38.0 | True |
| A_n30_l2_5_s5 | 520 | 203 | 467 | 11,831 | 7.58% | 53.99% | 54.0 | True |
| A_n30_l2_5_s10 | 530 | 186 | 474 | 11,831 | 7.71% | 54.03% | 54.0 | True |
| A_n30_l2_5_s15 | 540 | 165 | 485 | 11,831 | 7.87% | 54.71% | 53.0 | True |
| A_n30_l2_5_s20 | 550 | 145 | 495 | 11,831 | 8.03% | 55.48% | 53.0 | True |
| A_n30_l5_s5 | 520 | 195 | 475 | 11,831 | 7.64% | 53.55% | 54.0 | True |
| A_n30_l5_s10 | 528 | 182 | 480 | 11,831 | 7.74% | 54.01% | 54.0 | True |
| A_n30_l5_s15 | 534 | 169 | 487 | 11,831 | 7.84% | 54.84% | 53.0 | True |
| A_n30_l5_s20 | 539 | 158 | 493 | 11,831 | 7.93% | 55.30% | 54.0 | True |
| A_n30_l7_5_s5 | 519 | 191 | 480 | 11,831 | 7.67% | 52.77% | 53.0 | True |
| A_n30_l7_5_s10 | 528 | 177 | 485 | 11,831 | 7.78% | 54.14% | 53.0 | True |
| A_n30_l7_5_s15 | 531 | 172 | 487 | 11,831 | 7.82% | 54.76% | 53.0 | True |
| A_n30_l7_5_s20 | 534 | 164 | 492 | 11,831 | 7.88% | 54.90% | 53.0 | True |
| A_n50_l2_5_s5 | 297 | 96 | 254 | 12,374 | 4.23% | 43.35% | 83.0 | True |
| A_n50_l2_5_s10 | 303 | 89 | 255 | 12,374 | 4.29% | 43.16% | 81.0 | True |
| A_n50_l2_5_s15 | 303 | 85 | 259 | 12,374 | 4.32% | 43.43% | 81.0 | True |
| A_n50_l2_5_s20 | 307 | 76 | 264 | 12,374 | 4.39% | 43.94% | 81.0 | True |
| A_n50_l5_s5 | 292 | 97 | 258 | 12,374 | 4.22% | 42.72% | 83.0 | True |
| A_n50_l5_s10 | 299 | 89 | 259 | 12,374 | 4.29% | 43.06% | 82.0 | True |
| A_n50_l5_s15 | 298 | 86 | 263 | 12,374 | 4.31% | 43.66% | 82.0 | True |
| A_n50_l5_s20 | 300 | 79 | 268 | 12,374 | 4.36% | 44.00% | 81.0 | True |
| A_n50_l7_5_s5 | 289 | 101 | 257 | 12,374 | 4.19% | 41.71% | 82.5 | True |
| A_n50_l7_5_s10 | 297 | 92 | 258 | 12,374 | 4.26% | 42.99% | 82.0 | True |
| A_n50_l7_5_s15 | 294 | 92 | 261 | 12,374 | 4.26% | 43.51% | 83.0 | True |
| A_n50_l7_5_s20 | 295 | 88 | 264 | 12,374 | 4.29% | 43.53% | 82.0 | True |

## Parameter Sweep — Model B

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| B_n5_l2_5 | 2,183 | 420 | 2,286 | 8,132 | 34.32% | 83.12% | 13.0 | True |
| B_n5_l5 | 2,126 | 544 | 2,219 | 8,132 | 33.37% | 77.71% | 13.0 | True |
| B_n5_l7_5 | 2,034 | 727 | 2,128 | 8,132 | 31.96% | 70.19% | 12.0 | True |
| B_n10_l2_5 | 1,371 | 233 | 1,453 | 9,964 | 21.69% | 76.18% | 23.0 | True |
| B_n10_l5 | 1,314 | 357 | 1,386 | 9,964 | 20.74% | 70.77% | 23.0 | True |
| B_n10_l7_5 | 1,222 | 540 | 1,295 | 9,964 | 19.33% | 63.25% | 23.0 | True |
| B_n20_l2_5 | 837 | 116 | 832 | 11,236 | 12.82% | 65.89% | 37.0 | True |
| B_n20_l5 | 780 | 240 | 765 | 11,236 | 11.87% | 60.48% | 37.0 | True |
| B_n20_l7_5 | 725 | 346 | 714 | 11,236 | 11.05% | 53.79% | 36.0 | True |
| B_n30_l2_5 | 565 | 84 | 541 | 11,831 | 8.49% | 57.17% | 52.0 | True |
| B_n30_l5 | 527 | 160 | 503 | 11,831 | 7.91% | 52.52% | 52.0 | True |
| B_n30_l7_5 | 476 | 257 | 457 | 11,831 | 7.17% | 45.98% | 52.0 | True |
| B_n50_l2_5 | 307 | 57 | 283 | 12,374 | 4.53% | 44.61% | 80.0 | True |
| B_n50_l5 | 284 | 99 | 264 | 12,374 | 4.21% | 40.79% | 80.0 | True |
| B_n50_l7_5 | 255 | 155 | 237 | 12,374 | 3.78% | 35.26% | 78.0 | True |

## Parameter Sweep — Model D

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| D_n5_l2_5_s5 | 2,108 | 667 | 2,114 | 8,132 | 32.42% | 79.81% | 13.0 | True |
| D_n5_l2_5_s10 | 2,082 | 728 | 2,079 | 8,132 | 31.96% | 77.56% | 13.0 | True |
| D_n5_l2_5_s15 | 2,047 | 789 | 2,053 | 8,132 | 31.49% | 76.87% | 13.0 | True |
| D_n5_l2_5_s20 | 2,038 | 817 | 2,034 | 8,132 | 31.27% | 76.54% | 13.0 | True |
| D_n5_l5_s5 | 2,087 | 704 | 2,098 | 8,132 | 32.14% | 77.10% | 13.0 | True |
| D_n5_l5_s10 | 2,065 | 758 | 2,066 | 8,132 | 31.73% | 76.46% | 13.0 | True |
| D_n5_l5_s15 | 2,036 | 811 | 2,042 | 8,132 | 31.32% | 76.46% | 13.0 | True |
| D_n5_l5_s20 | 2,029 | 833 | 2,027 | 8,132 | 31.15% | 76.17% | 13.0 | True |
| D_n5_l7_5_s5 | 2,073 | 733 | 2,083 | 8,132 | 31.92% | 75.81% | 13.0 | True |
| D_n5_l7_5_s10 | 2,056 | 777 | 2,056 | 8,132 | 31.58% | 75.87% | 13.0 | True |
| D_n5_l7_5_s15 | 2,028 | 824 | 2,037 | 8,132 | 31.22% | 76.13% | 13.0 | True |
| D_n5_l7_5_s20 | 2,018 | 848 | 2,023 | 8,132 | 31.03% | 75.98% | 13.0 | True |
| D_n10_l2_5_s5 | 1,338 | 390 | 1,329 | 9,964 | 20.48% | 73.22% | 23.0 | True |
| D_n10_l2_5_s10 | 1,312 | 451 | 1,294 | 9,964 | 20.01% | 70.97% | 23.0 | True |
| D_n10_l2_5_s15 | 1,294 | 489 | 1,274 | 9,964 | 19.72% | 70.42% | 23.0 | True |
| D_n10_l2_5_s20 | 1,290 | 502 | 1,265 | 9,964 | 19.62% | 70.14% | 23.0 | True |
| D_n10_l5_s5 | 1,317 | 427 | 1,313 | 9,964 | 20.20% | 70.52% | 23.0 | True |
| D_n10_l5_s10 | 1,295 | 481 | 1,281 | 9,964 | 19.78% | 69.87% | 23.0 | True |
| D_n10_l5_s15 | 1,283 | 511 | 1,263 | 9,964 | 19.55% | 70.01% | 23.0 | True |
| D_n10_l5_s20 | 1,281 | 518 | 1,258 | 9,964 | 19.50% | 69.78% | 23.0 | True |
| D_n10_l7_5_s5 | 1,303 | 456 | 1,298 | 9,964 | 19.98% | 69.23% | 23.0 | True |
| D_n10_l7_5_s10 | 1,286 | 500 | 1,271 | 9,964 | 19.64% | 69.29% | 23.0 | True |
| D_n10_l7_5_s15 | 1,275 | 524 | 1,258 | 9,964 | 19.45% | 69.68% | 23.0 | True |
| D_n10_l7_5_s20 | 1,270 | 533 | 1,254 | 9,964 | 19.38% | 69.58% | 23.0 | True |
| D_n20_l2_5_s5 | 809 | 213 | 763 | 11,236 | 12.07% | 63.59% | 38.0 | True |
| D_n20_l2_5_s10 | 791 | 247 | 747 | 11,236 | 11.81% | 61.57% | 38.0 | True |
| D_n20_l2_5_s15 | 776 | 274 | 735 | 11,236 | 11.60% | 61.11% | 38.0 | True |
| D_n20_l2_5_s20 | 776 | 286 | 723 | 11,236 | 11.51% | 60.83% | 38.0 | True |
| D_n20_l5_s5 | 788 | 250 | 747 | 11,236 | 11.79% | 60.88% | 37.0 | True |
| D_n20_l5_s10 | 774 | 277 | 734 | 11,236 | 11.58% | 60.47% | 37.0 | True |
| D_n20_l5_s15 | 765 | 296 | 724 | 11,236 | 11.44% | 60.70% | 38.0 | True |
| D_n20_l5_s20 | 767 | 302 | 716 | 11,236 | 11.39% | 60.47% | 38.0 | True |
| D_n20_l7_5_s5 | 773 | 278 | 734 | 11,236 | 11.57% | 59.60% | 37.0 | True |
| D_n20_l7_5_s10 | 765 | 296 | 724 | 11,236 | 11.44% | 59.88% | 37.0 | True |
| D_n20_l7_5_s15 | 756 | 310 | 719 | 11,236 | 11.33% | 60.37% | 37.0 | True |
| D_n20_l7_5_s20 | 760 | 310 | 715 | 11,236 | 11.33% | 60.36% | 38.0 | True |
| D_n30_l2_5_s5 | 552 | 140 | 498 | 11,831 | 8.06% | 55.51% | 53.0 | True |
| D_n30_l2_5_s10 | 538 | 167 | 485 | 11,831 | 7.86% | 53.59% | 53.0 | True |
| D_n30_l2_5_s15 | 529 | 184 | 477 | 11,831 | 7.73% | 53.30% | 53.0 | True |
| D_n30_l2_5_s20 | 528 | 188 | 474 | 11,831 | 7.70% | 53.14% | 53.0 | True |
| D_n30_l5_s5 | 531 | 172 | 487 | 11,831 | 7.82% | 52.89% | 52.0 | True |
| D_n30_l5_s10 | 521 | 190 | 479 | 11,831 | 7.68% | 52.62% | 52.0 | True |
| D_n30_l5_s15 | 515 | 203 | 472 | 11,831 | 7.58% | 52.94% | 53.0 | True |
| D_n30_l5_s20 | 515 | 204 | 471 | 11,831 | 7.57% | 52.78% | 53.0 | True |
| D_n30_l7_5_s5 | 518 | 197 | 475 | 11,831 | 7.63% | 51.65% | 52.0 | True |
| D_n30_l7_5_s10 | 514 | 206 | 470 | 11,831 | 7.56% | 52.08% | 52.0 | True |
| D_n30_l7_5_s15 | 508 | 214 | 468 | 11,831 | 7.50% | 52.66% | 53.0 | True |
| D_n30_l7_5_s20 | 510 | 209 | 471 | 11,831 | 7.53% | 52.72% | 53.0 | True |
| D_n50_l2_5_s5 | 306 | 82 | 259 | 12,374 | 4.34% | 43.70% | 82.0 | True |
| D_n50_l2_5_s10 | 301 | 96 | 250 | 12,374 | 4.23% | 42.11% | 81.0 | True |
| D_n50_l2_5_s15 | 295 | 103 | 249 | 12,374 | 4.18% | 42.03% | 82.0 | True |
| D_n50_l2_5_s20 | 295 | 104 | 248 | 12,374 | 4.17% | 41.96% | 82.0 | True |
| D_n50_l5_s5 | 287 | 109 | 251 | 12,374 | 4.13% | 41.20% | 83.0 | True |
| D_n50_l5_s10 | 288 | 112 | 247 | 12,374 | 4.11% | 41.30% | 82.0 | True |
| D_n50_l5_s15 | 284 | 115 | 248 | 12,374 | 4.09% | 41.83% | 83.0 | True |
| D_n50_l5_s20 | 283 | 115 | 249 | 12,374 | 4.09% | 41.71% | 82.0 | True |
| D_n50_l7_5_s5 | 281 | 120 | 246 | 12,374 | 4.05% | 40.27% | 83.0 | True |
| D_n50_l7_5_s10 | 284 | 120 | 243 | 12,374 | 4.05% | 40.95% | 83.0 | True |
| D_n50_l7_5_s15 | 280 | 120 | 247 | 12,374 | 4.05% | 41.72% | 84.0 | True |
| D_n50_l7_5_s20 | 281 | 116 | 250 | 12,374 | 4.08% | 41.77% | 83.0 | True |

## Coverage Comparison

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_l5_s10 | 782 | 268 | 735 | 11,236 | 11.65% | 61.87% | 38.0 | True |
| B_n20_l5 | 780 | 240 | 765 | 11,236 | 11.87% | 60.48% | 37.0 | True |
| D_n20_l5_s10 | 774 | 277 | 734 | 11,236 | 11.58% | 60.47% | 37.0 | True |
| D_n20_l7_5_s15 | 756 | 310 | 719 | 11,236 | 11.33% | 60.37% | 37.0 | True |

## Reliability Comparison

### A_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 8,132 | 0.00% |
| Reference | 0 | 0 | 0 | 3,104 | 0.00% |
| Moderate | 483 | 179 | 476 | 0 | 84.27% |
| RelativelyStrong | 299 | 89 | 259 | 0 | 86.24% |

### B_n20_l5

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 8,132 | 0.00% |
| Reference | 0 | 0 | 0 | 3,104 | 0.00% |
| Moderate | 496 | 141 | 501 | 0 | 87.61% |
| RelativelyStrong | 284 | 99 | 264 | 0 | 84.70% |

### D_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 8,132 | 0.00% |
| Reference | 0 | 0 | 0 | 3,104 | 0.00% |
| Moderate | 486 | 165 | 487 | 0 | 85.50% |
| RelativelyStrong | 288 | 112 | 247 | 0 | 82.69% |

### D_n20_l7_5_s15

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| Poor | 0 | 0 | 0 | 8,132 | 0.00% |
| Reference | 0 | 0 | 0 | 3,104 | 0.00% |
| Moderate | 476 | 190 | 472 | 0 | 83.30% |
| RelativelyStrong | 280 | 120 | 247 | 0 | 81.45% |

## Role Coverage Comparison

### A_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 155 | 69 | 175 | 3,454 | 8.56% |
| JUNGLE | 186 | 56 | 149 | 1,634 | 16.54% |
| MID | 191 | 61 | 171 | 2,729 | 11.48% |
| ADC | 122 | 45 | 72 | 1,246 | 13.06% |
| SUPPORT | 128 | 37 | 168 | 2,173 | 11.81% |

### B_n20_l5

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 171 | 17 | 211 | 3,454 | 9.91% |
| JUNGLE | 192 | 51 | 148 | 1,634 | 16.79% |
| MID | 190 | 55 | 178 | 2,729 | 11.68% |
| ADC | 115 | 48 | 76 | 1,246 | 12.86% |
| SUPPORT | 112 | 69 | 152 | 2,173 | 10.53% |

### D_n20_l5_s10

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 180 | 41 | 178 | 3,454 | 9.29% |
| JUNGLE | 172 | 71 | 148 | 1,634 | 15.80% |
| MID | 192 | 60 | 171 | 2,729 | 11.52% |
| ADC | 117 | 47 | 75 | 1,246 | 12.93% |
| SUPPORT | 113 | 58 | 162 | 2,173 | 10.97% |

### D_n20_l7_5_s15

| Group | A | Neutral | B | Insufficient | directional |
|---|---:|---:|---:|---:|---:|
| TOP | 171 | 51 | 177 | 3,454 | 9.03% |
| JUNGLE | 165 | 87 | 139 | 1,634 | 15.01% |
| MID | 187 | 66 | 170 | 2,729 | 11.33% |
| ADC | 117 | 55 | 67 | 1,246 | 12.39% |
| SUPPORT | 116 | 51 | 166 | 2,173 | 11.25% |

## Threshold Sensitivity

Individual metric signal coverage across the complete requested threshold range:

```json
[
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0,
    "Counts": {
      "Total": 1785,
      "SideA": 896,
      "Neutral": 30,
      "SideB": 859,
      "Insufficient": 0,
      "Directional": 1755,
      "DirectionalRate": 0.9831932773109243
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0,
    "Counts": {
      "Total": 1785,
      "SideA": 877,
      "Neutral": 61,
      "SideB": 847,
      "Insufficient": 0,
      "Directional": 1724,
      "DirectionalRate": 0.9658263305322129
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0,
    "Counts": {
      "Total": 1785,
      "SideA": 861,
      "Neutral": 73,
      "SideB": 851,
      "Insufficient": 0,
      "Directional": 1712,
      "DirectionalRate": 0.9591036414565827
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1785,
      "SideA": 882,
      "Neutral": 57,
      "SideB": 846,
      "Insufficient": 0,
      "Directional": 1728,
      "DirectionalRate": 0.9680672268907563
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1785,
      "SideA": 848,
      "Neutral": 119,
      "SideB": 818,
      "Insufficient": 0,
      "Directional": 1666,
      "DirectionalRate": 0.9333333333333333
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.025,
    "Counts": {
      "Total": 1785,
      "SideA": 830,
      "Neutral": 135,
      "SideB": 820,
      "Insufficient": 0,
      "Directional": 1650,
      "DirectionalRate": 0.9243697478991597
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1785,
      "SideA": 849,
      "Neutral": 130,
      "SideB": 806,
      "Insufficient": 0,
      "Directional": 1655,
      "DirectionalRate": 0.927170868347339
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1785,
      "SideA": 785,
      "Neutral": 246,
      "SideB": 754,
      "Insufficient": 0,
      "Directional": 1539,
      "DirectionalRate": 0.8621848739495799
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1785,
      "SideA": 746,
      "Neutral": 299,
      "SideB": 740,
      "Insufficient": 0,
      "Directional": 1486,
      "DirectionalRate": 0.8324929971988796
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1785,
      "SideA": 824,
      "Neutral": 187,
      "SideB": 774,
      "Insufficient": 0,
      "Directional": 1598,
      "DirectionalRate": 0.8952380952380953
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1785,
      "SideA": 732,
      "Neutral": 358,
      "SideB": 695,
      "Insufficient": 0,
      "Directional": 1427,
      "DirectionalRate": 0.7994397759103642
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.075,
    "Counts": {
      "Total": 1785,
      "SideA": 673,
      "Neutral": 435,
      "SideB": 677,
      "Insufficient": 0,
      "Directional": 1350,
      "DirectionalRate": 0.7563025210084033
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L2",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1785,
      "SideA": 797,
      "Neutral": 240,
      "SideB": 748,
      "Insufficient": 0,
      "Directional": 1545,
      "DirectionalRate": 0.865546218487395
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L3",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1785,
      "SideA": 676,
      "Neutral": 472,
      "SideB": 637,
      "Insufficient": 0,
      "Directional": 1313,
      "DirectionalRate": 0.7355742296918768
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "L6",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1785,
      "SideA": 602,
      "Neutral": 567,
      "SideB": 616,
      "Insufficient": 0,
      "Directional": 1218,
      "DirectionalRate": 0.6823529411764706
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0,
    "Counts": {
      "Total": 1785,
      "SideA": 830,
      "Neutral": 138,
      "SideB": 817,
      "Insufficient": 0,
      "Directional": 1647,
      "DirectionalRate": 0.9226890756302522
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.05,
    "Counts": {
      "Total": 1785,
      "SideA": 669,
      "Neutral": 501,
      "SideB": 615,
      "Insufficient": 0,
      "Directional": 1284,
      "DirectionalRate": 0.719327731092437
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.1,
    "Counts": {
      "Total": 1785,
      "SideA": 531,
      "Neutral": 781,
      "SideB": 473,
      "Insufficient": 0,
      "Directional": 1004,
      "DirectionalRate": 0.5624649859943978
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.15,
    "Counts": {
      "Total": 1785,
      "SideA": 420,
      "Neutral": 980,
      "SideB": 385,
      "Insufficient": 0,
      "Directional": 805,
      "DirectionalRate": 0.45098039215686275
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.2,
    "Counts": {
      "Total": 1785,
      "SideA": 359,
      "Neutral": 1127,
      "SideB": 299,
      "Insufficient": 0,
      "Directional": 658,
      "DirectionalRate": 0.3686274509803922
    }
  },
  {
    "MinimumSample": 20,
    "Metric": "Solo",
    "Threshold": 0.3,
    "Counts": {
      "Total": 1785,
      "SideA": 250,
      "Neutral": 1332,
      "SideB": 203,
      "Insufficient": 0,
      "Directional": 453,
      "DirectionalRate": 0.253781512605042
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0,
    "Counts": {
      "Total": 647,
      "SideA": 324,
      "Neutral": 7,
      "SideB": 316,
      "Insufficient": 0,
      "Directional": 640,
      "DirectionalRate": 0.9891808346213292
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0,
    "Counts": {
      "Total": 647,
      "SideA": 337,
      "Neutral": 11,
      "SideB": 299,
      "Insufficient": 0,
      "Directional": 636,
      "DirectionalRate": 0.9829984544049459
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0,
    "Counts": {
      "Total": 647,
      "SideA": 331,
      "Neutral": 16,
      "SideB": 300,
      "Insufficient": 0,
      "Directional": 631,
      "DirectionalRate": 0.9752704791344667
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.025,
    "Counts": {
      "Total": 647,
      "SideA": 312,
      "Neutral": 28,
      "SideB": 307,
      "Insufficient": 0,
      "Directional": 619,
      "DirectionalRate": 0.9567233384853169
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.025,
    "Counts": {
      "Total": 647,
      "SideA": 313,
      "Neutral": 57,
      "SideB": 277,
      "Insufficient": 0,
      "Directional": 590,
      "DirectionalRate": 0.9119010819165378
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.025,
    "Counts": {
      "Total": 647,
      "SideA": 308,
      "Neutral": 66,
      "SideB": 273,
      "Insufficient": 0,
      "Directional": 581,
      "DirectionalRate": 0.8979907264296755
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.05,
    "Counts": {
      "Total": 647,
      "SideA": 301,
      "Neutral": 50,
      "SideB": 296,
      "Insufficient": 0,
      "Directional": 597,
      "DirectionalRate": 0.9227202472952086
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.05,
    "Counts": {
      "Total": 647,
      "SideA": 289,
      "Neutral": 98,
      "SideB": 260,
      "Insufficient": 0,
      "Directional": 549,
      "DirectionalRate": 0.848531684698609
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.05,
    "Counts": {
      "Total": 647,
      "SideA": 272,
      "Neutral": 133,
      "SideB": 242,
      "Insufficient": 0,
      "Directional": 514,
      "DirectionalRate": 0.794435857805255
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.075,
    "Counts": {
      "Total": 647,
      "SideA": 285,
      "Neutral": 84,
      "SideB": 278,
      "Insufficient": 0,
      "Directional": 563,
      "DirectionalRate": 0.8701700154559505
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.075,
    "Counts": {
      "Total": 647,
      "SideA": 262,
      "Neutral": 156,
      "SideB": 229,
      "Insufficient": 0,
      "Directional": 491,
      "DirectionalRate": 0.758887171561051
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.075,
    "Counts": {
      "Total": 647,
      "SideA": 233,
      "Neutral": 198,
      "SideB": 216,
      "Insufficient": 0,
      "Directional": 449,
      "DirectionalRate": 0.6939721792890263
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L2",
    "Threshold": 0.1,
    "Counts": {
      "Total": 647,
      "SideA": 278,
      "Neutral": 103,
      "SideB": 266,
      "Insufficient": 0,
      "Directional": 544,
      "DirectionalRate": 0.8408037094281299
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L3",
    "Threshold": 0.1,
    "Counts": {
      "Total": 647,
      "SideA": 240,
      "Neutral": 207,
      "SideB": 200,
      "Insufficient": 0,
      "Directional": 440,
      "DirectionalRate": 0.6800618238021638
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "L6",
    "Threshold": 0.1,
    "Counts": {
      "Total": 647,
      "SideA": 202,
      "Neutral": 253,
      "SideB": 192,
      "Insufficient": 0,
      "Directional": 394,
      "DirectionalRate": 0.6089644513137558
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0,
    "Counts": {
      "Total": 647,
      "SideA": 320,
      "Neutral": 31,
      "SideB": 296,
      "Insufficient": 0,
      "Directional": 616,
      "DirectionalRate": 0.9520865533230294
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.05,
    "Counts": {
      "Total": 647,
      "SideA": 217,
      "Neutral": 240,
      "SideB": 190,
      "Insufficient": 0,
      "Directional": 407,
      "DirectionalRate": 0.6290571870170015
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.1,
    "Counts": {
      "Total": 647,
      "SideA": 159,
      "Neutral": 360,
      "SideB": 128,
      "Insufficient": 0,
      "Directional": 287,
      "DirectionalRate": 0.4435857805255023
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.15,
    "Counts": {
      "Total": 647,
      "SideA": 119,
      "Neutral": 428,
      "SideB": 100,
      "Insufficient": 0,
      "Directional": 219,
      "DirectionalRate": 0.3384853168469861
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.2,
    "Counts": {
      "Total": 647,
      "SideA": 99,
      "Neutral": 472,
      "SideB": 76,
      "Insufficient": 0,
      "Directional": 175,
      "DirectionalRate": 0.2704791344667697
    }
  },
  {
    "MinimumSample": 50,
    "Metric": "Solo",
    "Threshold": 0.3,
    "Counts": {
      "Total": 647,
      "SideA": 67,
      "Neutral": 527,
      "SideB": 53,
      "Insufficient": 0,
      "Directional": 120,
      "DirectionalRate": 0.18547140649149924
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
      "Total": 13021,
      "SideA": 809,
      "Neutral": 213,
      "SideB": 763,
      "Insufficient": 11236,
      "Directional": 1572,
      "DirectionalRate": 0.12072805468090009
    },
    "KeyDirectionalCoverage": 0.12072805468090009,
    "SampleWeightedDirectionalCoverage": 0.6358744971842317
  },
  {
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 13021,
      "SideA": 791,
      "Neutral": 247,
      "SideB": 747,
      "Insufficient": 11236,
      "Directional": 1538,
      "DirectionalRate": 0.11811688810383227
    },
    "KeyDirectionalCoverage": 0.11811688810383227,
    "SampleWeightedDirectionalCoverage": 0.6157232502011263
  },
  {
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 13021,
      "SideA": 776,
      "Neutral": 286,
      "SideB": 723,
      "Insufficient": 11236,
      "Directional": 1499,
      "DirectionalRate": 0.11512172644190154
    },
    "KeyDirectionalCoverage": 0.11512172644190154,
    "SampleWeightedDirectionalCoverage": 0.6083475462590506
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.05,
    "Counts": {
      "Total": 13021,
      "SideA": 788,
      "Neutral": 250,
      "SideB": 747,
      "Insufficient": 11236,
      "Directional": 1535,
      "DirectionalRate": 0.11788649105291452
    },
    "KeyDirectionalCoverage": 0.11788649105291452,
    "SampleWeightedDirectionalCoverage": 0.6088302493966211
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 13021,
      "SideA": 774,
      "Neutral": 277,
      "SideB": 734,
      "Insufficient": 11236,
      "Directional": 1508,
      "DirectionalRate": 0.11581291759465479
    },
    "KeyDirectionalCoverage": 0.11581291759465479,
    "SampleWeightedDirectionalCoverage": 0.6047369267900241
  },
  {
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 13021,
      "SideA": 767,
      "Neutral": 302,
      "SideB": 716,
      "Insufficient": 11236,
      "Directional": 1483,
      "DirectionalRate": 0.11389294217034023
    },
    "KeyDirectionalCoverage": 0.11389294217034023,
    "SampleWeightedDirectionalCoverage": 0.6046661303298472
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.05,
    "Counts": {
      "Total": 13021,
      "SideA": 773,
      "Neutral": 278,
      "SideB": 734,
      "Insufficient": 11236,
      "Directional": 1507,
      "DirectionalRate": 0.1157361185776822
    },
    "KeyDirectionalCoverage": 0.1157361185776822,
    "SampleWeightedDirectionalCoverage": 0.595951729686243
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.1,
    "Counts": {
      "Total": 13021,
      "SideA": 765,
      "Neutral": 296,
      "SideB": 724,
      "Insufficient": 11236,
      "Directional": 1489,
      "DirectionalRate": 0.11435373627217571
    },
    "KeyDirectionalCoverage": 0.11435373627217571,
    "SampleWeightedDirectionalCoverage": 0.5988415124698311
  },
  {
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.2,
    "Counts": {
      "Total": 13021,
      "SideA": 760,
      "Neutral": 310,
      "SideB": 715,
      "Insufficient": 11236,
      "Directional": 1475,
      "DirectionalRate": 0.11327855003455956
    },
    "KeyDirectionalCoverage": 0.11327855003455956,
    "SampleWeightedDirectionalCoverage": 0.6035655671761866
  }
]
```

## High-sample Subset Comparison

```json
[
  {
    "MinimumSample": 20,
    "Counts": {
      "Total": 1785,
      "SideA": 774,
      "Neutral": 277,
      "SideB": 734,
      "Insufficient": 0,
      "Directional": 1508,
      "DirectionalRate": 0.8448179271708683
    }
  },
  {
    "MinimumSample": 50,
    "Counts": {
      "Total": 647,
      "SideA": 288,
      "Neutral": 112,
      "SideB": 247,
      "Insufficient": 0,
      "Directional": 535,
      "DirectionalRate": 0.8268933539412674
    }
  },
  {
    "MinimumSample": 100,
    "Counts": {
      "Total": 245,
      "SideA": 111,
      "Neutral": 40,
      "SideB": 94,
      "Insufficient": 0,
      "Directional": 205,
      "DirectionalRate": 0.8367346938775511
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
      "Total": 13021,
      "SideA": 837,
      "Neutral": 116,
      "SideB": 832,
      "Insufficient": 11236,
      "Directional": 1669,
      "DirectionalRate": 0.12817755932724062
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 669,
      "Neutral": 501,
      "SideB": 615,
      "Insufficient": 11236,
      "Directional": 1284,
      "DirectionalRate": 0.09860993779279625
    },
    "SameDirectionAmongBothDirectional": 0.562962962962963,
    "ConflictAmongBothDirectional": 0.43703703703703706,
    "OneNeutralRate": 0.29299719887955183,
    "BothNeutralRate": 0.026330532212885154
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 837,
      "Neutral": 116,
      "SideB": 832,
      "Insufficient": 11236,
      "Directional": 1669,
      "DirectionalRate": 0.12817755932724062
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 531,
      "Neutral": 781,
      "SideB": 473,
      "Insufficient": 11236,
      "Directional": 1004,
      "DirectionalRate": 0.07710621304047308
    },
    "SameDirectionAmongBothDirectional": 0.5668058455114823,
    "ConflictAmongBothDirectional": 0.4331941544885177,
    "OneNeutralRate": 0.42408963585434173,
    "BothNeutralRate": 0.0392156862745098
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 837,
      "Neutral": 116,
      "SideB": 832,
      "Insufficient": 11236,
      "Directional": 1669,
      "DirectionalRate": 0.12817755932724062
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 420,
      "Neutral": 980,
      "SideB": 385,
      "Insufficient": 11236,
      "Directional": 805,
      "DirectionalRate": 0.06182320866292911
    },
    "SameDirectionAmongBothDirectional": 0.5710560625814863,
    "ConflictAmongBothDirectional": 0.42894393741851367,
    "OneNeutralRate": 0.5266106442577031,
    "BothNeutralRate": 0.043697478991596636
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.025,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 837,
      "Neutral": 116,
      "SideB": 832,
      "Insufficient": 11236,
      "Directional": 1669,
      "DirectionalRate": 0.12817755932724062
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 359,
      "Neutral": 1127,
      "SideB": 299,
      "Insufficient": 11236,
      "Directional": 658,
      "DirectionalRate": 0.05053375316795945
    },
    "SameDirectionAmongBothDirectional": 0.5764331210191083,
    "ConflictAmongBothDirectional": 0.42356687898089174,
    "OneNeutralRate": 0.6,
    "BothNeutralRate": 0.048179271708683476
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.05,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 780,
      "Neutral": 240,
      "SideB": 765,
      "Insufficient": 11236,
      "Directional": 1545,
      "DirectionalRate": 0.11865448122264036
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 669,
      "Neutral": 501,
      "SideB": 615,
      "Insufficient": 11236,
      "Directional": 1284,
      "DirectionalRate": 0.09860993779279625
    },
    "SameDirectionAmongBothDirectional": 0.5622241835834069,
    "ConflictAmongBothDirectional": 0.43777581641659313,
    "OneNeutralRate": 0.315406162464986,
    "BothNeutralRate": 0.04985994397759104
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 780,
      "Neutral": 240,
      "SideB": 765,
      "Insufficient": 11236,
      "Directional": 1545,
      "DirectionalRate": 0.11865448122264036
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 531,
      "Neutral": 781,
      "SideB": 473,
      "Insufficient": 11236,
      "Directional": 1004,
      "DirectionalRate": 0.07710621304047308
    },
    "SameDirectionAmongBothDirectional": 0.5672969966629589,
    "ConflictAmongBothDirectional": 0.43270300333704115,
    "OneNeutralRate": 0.4207282913165266,
    "BothNeutralRate": 0.07563025210084033
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 780,
      "Neutral": 240,
      "SideB": 765,
      "Insufficient": 11236,
      "Directional": 1545,
      "DirectionalRate": 0.11865448122264036
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 420,
      "Neutral": 980,
      "SideB": 385,
      "Insufficient": 11236,
      "Directional": 805,
      "DirectionalRate": 0.06182320866292911
    },
    "SameDirectionAmongBothDirectional": 0.5753803596127247,
    "ConflictAmongBothDirectional": 0.42461964038727523,
    "OneNeutralRate": 0.5064425770308123,
    "BothNeutralRate": 0.08851540616246499
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.05,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 780,
      "Neutral": 240,
      "SideB": 765,
      "Insufficient": 11236,
      "Directional": 1545,
      "DirectionalRate": 0.11865448122264036
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 359,
      "Neutral": 1127,
      "SideB": 299,
      "Insufficient": 11236,
      "Directional": 658,
      "DirectionalRate": 0.05053375316795945
    },
    "SameDirectionAmongBothDirectional": 0.5781512605042017,
    "ConflictAmongBothDirectional": 0.4218487394957983,
    "OneNeutralRate": 0.5675070028011204,
    "BothNeutralRate": 0.09915966386554621
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.05,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 725,
      "Neutral": 346,
      "SideB": 714,
      "Insufficient": 11236,
      "Directional": 1439,
      "DirectionalRate": 0.11051378542354658
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 669,
      "Neutral": 501,
      "SideB": 615,
      "Insufficient": 11236,
      "Directional": 1284,
      "DirectionalRate": 0.09860993779279625
    },
    "SameDirectionAmongBothDirectional": 0.5691439322671684,
    "ConflictAmongBothDirectional": 0.4308560677328316,
    "OneNeutralRate": 0.334453781512605,
    "BothNeutralRate": 0.0700280112044818
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.1,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 725,
      "Neutral": 346,
      "SideB": 714,
      "Insufficient": 11236,
      "Directional": 1439,
      "DirectionalRate": 0.11051378542354658
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 531,
      "Neutral": 781,
      "SideB": 473,
      "Insufficient": 11236,
      "Directional": 1004,
      "DirectionalRate": 0.07710621304047308
    },
    "SameDirectionAmongBothDirectional": 0.5741176470588235,
    "ConflictAmongBothDirectional": 0.4258823529411765,
    "OneNeutralRate": 0.4162464985994398,
    "BothNeutralRate": 0.10756302521008404
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.15,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 725,
      "Neutral": 346,
      "SideB": 714,
      "Insufficient": 11236,
      "Directional": 1439,
      "DirectionalRate": 0.11051378542354658
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 420,
      "Neutral": 980,
      "SideB": 385,
      "Insufficient": 11236,
      "Directional": 805,
      "DirectionalRate": 0.06182320866292911
    },
    "SameDirectionAmongBothDirectional": 0.5836972343522562,
    "ConflictAmongBothDirectional": 0.4163027656477438,
    "OneNeutralRate": 0.48739495798319327,
    "BothNeutralRate": 0.12773109243697478
  },
  {
    "MinimumSample": 20,
    "LevelThreshold": 0.075,
    "SoloThreshold": 0.2,
    "LevelCounts": {
      "Total": 13021,
      "SideA": 725,
      "Neutral": 346,
      "SideB": 714,
      "Insufficient": 11236,
      "Directional": 1439,
      "DirectionalRate": 0.11051378542354658
    },
    "CombatCounts": {
      "Total": 13021,
      "SideA": 359,
      "Neutral": 1127,
      "SideB": 299,
      "Insufficient": 11236,
      "Directional": 658,
      "DirectionalRate": 0.05053375316795945
    },
    "SameDirectionAmongBothDirectional": 0.5837742504409171,
    "ConflictAmongBothDirectional": 0.41622574955908287,
    "OneNeutralRate": 0.5394957983193277,
    "BothNeutralRate": 0.14285714285714285
  }
]
```

## Model Disagreement Cases

```json
{
  "StrongestA": [
    {
      "Role": "TOP",
      "ChampionA": "Kled",
      "ChampionB": "Nasus",
      "SampleSize": 34,
      "L2Difference": 0.7647058823529411,
      "L3Difference": 0.8235294117647058,
      "L6Difference": 0.5588235294117647,
      "SoloDifference": 1.5,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "KSante",
      "ChampionB": "Nasus",
      "SampleSize": 110,
      "L2Difference": 0.8181818181818182,
      "L3Difference": 0.7818181818181819,
      "L6Difference": 0.3090909090909091,
      "SoloDifference": 1.6181818181818182,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jax",
      "ChampionB": "Nasus",
      "SampleSize": 72,
      "L2Difference": 0.9444444444444444,
      "L3Difference": 0.9722222222222222,
      "L6Difference": 0.6666666666666666,
      "SoloDifference": 0.9305555555555556,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Cassiopeia",
      "ChampionB": "Yone",
      "SampleSize": 22,
      "L2Difference": 0,
      "L3Difference": 0.9090909090909091,
      "L6Difference": 0.5454545454545454,
      "SoloDifference": 1.7727272727272727,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Aatrox",
      "ChampionB": "Nasus",
      "SampleSize": 78,
      "L2Difference": 0.7435897435897436,
      "L3Difference": 0.8205128205128205,
      "L6Difference": 0.6153846153846154,
      "SoloDifference": 1,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Aatrox",
      "ChampionB": "DrMundo",
      "SampleSize": 50,
      "L2Difference": 0.72,
      "L3Difference": 0.76,
      "L6Difference": 0.44,
      "SoloDifference": 1.24,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Irelia",
      "ChampionB": "Nasus",
      "SampleSize": 40,
      "L2Difference": 0.65,
      "L3Difference": 0.65,
      "L6Difference": 0.65,
      "SoloDifference": 1.125,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Kled",
      "ChampionB": "Yone",
      "SampleSize": 31,
      "L2Difference": -0.16129032258064516,
      "L3Difference": -0.03225806451612903,
      "L6Difference": 0.7419354838709677,
      "SoloDifference": 2.129032258064516,
      "CombinedResult": "SideA",
      "LevelTempo": "Neutral",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Darius",
      "ChampionB": "Nasus",
      "SampleSize": 27,
      "L2Difference": 0.7777777777777778,
      "L3Difference": 0.7777777777777778,
      "L6Difference": 0.3333333333333333,
      "SoloDifference": 1.037037037037037,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jax",
      "ChampionB": "Malphite",
      "SampleSize": 35,
      "L2Difference": 1,
      "L3Difference": 0.8285714285714286,
      "L6Difference": 0.37142857142857144,
      "SoloDifference": -0.6571428571428571,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    }
  ],
  "StrongestB": [
    {
      "Role": "TOP",
      "ChampionA": "Gnar",
      "ChampionB": "Kled",
      "SampleSize": 23,
      "L2Difference": -0.30434782608695654,
      "L3Difference": -0.043478260869565216,
      "L6Difference": -0.7391304347826086,
      "SoloDifference": -2.260869565217391,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Tryndamere",
      "SampleSize": 23,
      "L2Difference": -0.9130434782608695,
      "L3Difference": -0.8260869565217391,
      "L6Difference": -0.9130434782608695,
      "SoloDifference": -0.6521739130434783,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Renekton",
      "SampleSize": 35,
      "L2Difference": -1,
      "L3Difference": -0.8285714285714286,
      "L6Difference": -0.6,
      "SoloDifference": -0.8,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "DrMundo",
      "ChampionB": "Fiora",
      "SampleSize": 22,
      "L2Difference": -0.8181818181818182,
      "L3Difference": -0.9090909090909091,
      "L6Difference": -0.18181818181818182,
      "SoloDifference": -1.2272727272727273,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Ornn",
      "ChampionB": "Yone",
      "SampleSize": 21,
      "L2Difference": -0.8095238095238095,
      "L3Difference": -0.7142857142857143,
      "L6Difference": -0.5238095238095238,
      "SoloDifference": -1,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Gragas",
      "ChampionB": "KSante",
      "SampleSize": 29,
      "L2Difference": -0.8620689655172413,
      "L3Difference": -0.7241379310344828,
      "L6Difference": -0.4482758620689655,
      "SoloDifference": -0.9310344827586207,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Yone",
      "SampleSize": 92,
      "L2Difference": -0.782608695652174,
      "L3Difference": -0.782608695652174,
      "L6Difference": -0.5652173913043478,
      "SoloDifference": -0.8152173913043478,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Nasus",
      "ChampionB": "Pantheon",
      "SampleSize": 25,
      "L2Difference": -0.76,
      "L3Difference": -1,
      "L6Difference": -0.2,
      "SoloDifference": -0.92,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "DrMundo",
      "ChampionB": "KSante",
      "SampleSize": 31,
      "L2Difference": -0.7419354838709677,
      "L3Difference": -0.6129032258064516,
      "L6Difference": -0.22580645161290322,
      "SoloDifference": -1.1612903225806452,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Chogath",
      "ChampionB": "Jayce",
      "SampleSize": 23,
      "L2Difference": -0.8260869565217391,
      "L3Difference": -0.8260869565217391,
      "L6Difference": -0.5652173913043478,
      "SoloDifference": 0.4782608695652174,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    }
  ],
  "Neutral": [
    {
      "Role": "ADC",
      "ChampionA": "Ezreal",
      "ChampionB": "Jhin",
      "SampleSize": 685,
      "L2Difference": 0.19124087591240876,
      "L3Difference": 0.03941605839416058,
      "L6Difference": 0.03941605839416058,
      "SoloDifference": -0.11970802919708029,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Jinx",
      "ChampionB": "Kaisa",
      "SampleSize": 399,
      "L2Difference": 0.37092731829573933,
      "L3Difference": 0.13283208020050125,
      "L6Difference": -0.017543859649122806,
      "SoloDifference": -0.10275689223057644,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Kaisa",
      "ChampionB": "Tristana",
      "SampleSize": 371,
      "L2Difference": 0.22911051212938005,
      "L3Difference": -0.11590296495956873,
      "L6Difference": -0.013477088948787063,
      "SoloDifference": -0.09433962264150944,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Nautilus",
      "ChampionB": "Rell",
      "SampleSize": 366,
      "L2Difference": -0.16939890710382513,
      "L3Difference": 0.01912568306010929,
      "L6Difference": 0.07103825136612021,
      "SoloDifference": 0.00273224043715847,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Lucian",
      "ChampionB": "Yunara",
      "SampleSize": 343,
      "L2Difference": -0.1836734693877551,
      "L3Difference": 0.04956268221574344,
      "L6Difference": 0.14285714285714285,
      "SoloDifference": -0.026239067055393587,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Aphelios",
      "ChampionB": "Yunara",
      "SampleSize": 329,
      "L2Difference": -0.14285714285714285,
      "L3Difference": 0.07598784194528875,
      "L6Difference": 0.04559270516717325,
      "SoloDifference": -0.0121580547112462,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Aphelios",
      "ChampionB": "Jinx",
      "SampleSize": 278,
      "L2Difference": -0.4748201438848921,
      "L3Difference": 0.10071942446043165,
      "L6Difference": -0.014388489208633094,
      "SoloDifference": 0.02877697841726619,
      "CombinedResult": "Neutral",
      "LevelTempo": "Neutral",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Bard",
      "ChampionB": "Lulu",
      "SampleSize": 264,
      "L2Difference": 0.1553030303030303,
      "L3Difference": 0.13636363636363635,
      "L6Difference": -0.06818181818181818,
      "SoloDifference": 0.007575757575757576,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "KSante",
      "SampleSize": 261,
      "L2Difference": 0.8544061302681992,
      "L3Difference": 0.8199233716475096,
      "L6Difference": 0.0038314176245210726,
      "SoloDifference": -0.6091954022988506,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Olaf",
      "SampleSize": 236,
      "L2Difference": -0.09322033898305085,
      "L3Difference": 0.1016949152542373,
      "L6Difference": 0.16101694915254236,
      "SoloDifference": -0.5211864406779662,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    }
  ],
  "LevelCombatConflicts": [
    {
      "Role": "ADC",
      "ChampionA": "Jinx",
      "ChampionB": "Kaisa",
      "SampleSize": 399,
      "L2Difference": 0.37092731829573933,
      "L3Difference": 0.13283208020050125,
      "L6Difference": -0.017543859649122806,
      "SoloDifference": -0.10275689223057644,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Tristana",
      "ChampionB": "Yunara",
      "SampleSize": 308,
      "L2Difference": -0.2597402597402597,
      "L3Difference": -0.05194805194805195,
      "L6Difference": 0.14285714285714285,
      "SoloDifference": 0.22402597402597402,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "ADC",
      "ChampionA": "Kaisa",
      "ChampionB": "Kalista",
      "SampleSize": 290,
      "L2Difference": -0.32413793103448274,
      "L3Difference": -0.013793103448275862,
      "L6Difference": -0.11724137931034483,
      "SoloDifference": 0.10344827586206896,
      "CombinedResult": "SideB",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "KSante",
      "SampleSize": 261,
      "L2Difference": 0.8544061302681992,
      "L3Difference": 0.8199233716475096,
      "L6Difference": 0.0038314176245210726,
      "SoloDifference": -0.6091954022988506,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Olaf",
      "SampleSize": 236,
      "L2Difference": -0.09322033898305085,
      "L3Difference": 0.1016949152542373,
      "L6Difference": 0.16101694915254236,
      "SoloDifference": -0.5211864406779662,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "Malphite",
      "SampleSize": 214,
      "L2Difference": 0.883177570093458,
      "L3Difference": 0.6869158878504673,
      "L6Difference": 0.06542056074766354,
      "SoloDifference": -0.37850467289719625,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Corki",
      "ChampionB": "Yunara",
      "SampleSize": 176,
      "L2Difference": -0.13636363636363635,
      "L3Difference": -0.056818181818181816,
      "L6Difference": 0.125,
      "SoloDifference": 0.19886363636363635,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "ADC",
      "ChampionA": "Corki",
      "ChampionB": "Jhin",
      "SampleSize": 170,
      "L2Difference": -0.17647058823529413,
      "L3Difference": 0.12941176470588237,
      "L6Difference": -0.12941176470588237,
      "SoloDifference": 0.11764705882352941,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "ADC",
      "ChampionA": "Ezreal",
      "ChampionB": "Tristana",
      "SampleSize": 170,
      "L2Difference": 0.32941176470588235,
      "L3Difference": 0.047058823529411764,
      "L6Difference": 0.17647058823529413,
      "SoloDifference": -0.17647058823529413,
      "CombinedResult": "SideA",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "MID",
      "ChampionA": "Akali",
      "ChampionB": "Viktor",
      "SampleSize": 166,
      "L2Difference": -0.6265060240963856,
      "L3Difference": -0.10843373493975904,
      "L6Difference": 0.1686746987951807,
      "SoloDifference": 0.29518072289156627,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    }
  ],
  "CombinedModelDisagreements": [
    {
      "Role": "ADC",
      "ChampionA": "Jinx",
      "ChampionB": "Kaisa",
      "SampleSize": 399,
      "L2Difference": 0.37092731829573933,
      "L3Difference": 0.13283208020050125,
      "L6Difference": -0.017543859649122806,
      "SoloDifference": -0.10275689223057644,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "ADC",
      "ChampionA": "Tristana",
      "ChampionB": "Yunara",
      "SampleSize": 308,
      "L2Difference": -0.2597402597402597,
      "L3Difference": -0.05194805194805195,
      "L6Difference": 0.14285714285714285,
      "SoloDifference": 0.22402597402597402,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "SUPPORT",
      "ChampionA": "Bard",
      "ChampionB": "Lulu",
      "SampleSize": 264,
      "L2Difference": 0.1553030303030303,
      "L3Difference": 0.13636363636363635,
      "L6Difference": -0.06818181818181818,
      "SoloDifference": 0.007575757575757576,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "TOP",
      "ChampionA": "Jayce",
      "ChampionB": "KSante",
      "SampleSize": 261,
      "L2Difference": 0.8544061302681992,
      "L3Difference": 0.8199233716475096,
      "L6Difference": 0.0038314176245210726,
      "SoloDifference": -0.6091954022988506,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "SideB"
    },
    {
      "Role": "MID",
      "ChampionA": "Syndra",
      "ChampionB": "Viktor",
      "SampleSize": 234,
      "L2Difference": -0.405982905982906,
      "L3Difference": -0.19230769230769232,
      "L6Difference": 0.07692307692307693,
      "SoloDifference": 0.09401709401709402,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideB",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Ashe",
      "ChampionB": "Jhin",
      "SampleSize": 202,
      "L2Difference": 0.1485148514851485,
      "L3Difference": 0.054455445544554455,
      "L6Difference": -0.10891089108910891,
      "SoloDifference": -0.06930693069306931,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "JUNGLE",
      "ChampionA": "Qiyana",
      "ChampionB": "Vi",
      "SampleSize": 181,
      "L2Difference": 0.3701657458563536,
      "L3Difference": 0.5359116022099447,
      "L6Difference": -0.06077348066298342,
      "SoloDifference": 0.022099447513812154,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Ashe",
      "ChampionB": "Kaisa",
      "SampleSize": 179,
      "L2Difference": 0.2569832402234637,
      "L3Difference": 0.15083798882681565,
      "L6Difference": -0.09497206703910614,
      "SoloDifference": -0.09497206703910614,
      "CombinedResult": "Neutral",
      "LevelTempo": "SideA",
      "CombatPressure": "Neutral"
    },
    {
      "Role": "ADC",
      "ChampionA": "Corki",
      "ChampionB": "Yunara",
      "SampleSize": 176,
      "L2Difference": -0.13636363636363635,
      "L3Difference": -0.056818181818181816,
      "L6Difference": 0.125,
      "SoloDifference": 0.19886363636363635,
      "CombinedResult": "SideA",
      "LevelTempo": "SideB",
      "CombatPressure": "SideA"
    },
    {
      "Role": "MID",
      "ChampionA": "Akali",
      "ChampionB": "Viktor",
      "SampleSize": 166,
      "L2Difference": -0.6265060240963856,
      "L3Difference": -0.10843373493975904,
      "L6Difference": 0.1686746987951807,
      "SoloDifference": 0.29518072289156627,
      "CombinedResult": "SideA",
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
  "DirectionalLaneResults": 1508,
  "SameDirectionalOutcome": 202,
  "NeutralOutcome": 1159,
  "OppositeDirectionalOutcome": 147,
  "InsufficientOutcome": 0
}
```

## Candidate Shortlist

| Model | A | Neutral | B | Insufficient | directional keys | sample-weighted | median n | symmetric |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A_n20_l5_s10 | 782 | 268 | 735 | 11,236 | 11.65% | 61.87% | 38.0 | True |
| B_n20_l5 | 780 | 240 | 765 | 11,236 | 11.87% | 60.48% | 37.0 | True |
| D_n20_l5_s10 | 774 | 277 | 734 | 11,236 | 11.58% | 60.47% | 37.0 | True |
| D_n20_l7_5_s15 | 756 | 310 | 719 | 11,236 | 11.33% | 60.37% | 37.0 | True |

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
