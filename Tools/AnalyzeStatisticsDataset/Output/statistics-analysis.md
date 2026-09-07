# Statistics Reliability Analysis

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 31,075
- Observations: 155,375
- Matchup keys: 13,021
- Tier scope: `MASTER_PLUS_SEEDED` / `SEED_PLAYER_TIER_AT_COLLECTION`
- All participants tier verified: `false`
- Queue: 420; minimum duration: 900s
- Dataset revision: 1788710852

## Sample Size Distribution

Percentiles use linear interpolation at index `(N-1)*p`.

Min 1; mean 11.93; p25 1.00; median/p50 3.00; p75 9.00; p90 27.00; p95 49.00; p99 144.20; max 1,083.

Low-sample keys: n=1: 4,600, n=2: 1,802, n=3-4: 1,730, n=5-9: 1,832.

```json
[
  {
    "Threshold": 1,
    "Count": 13021,
    "Rate": 1
  },
  {
    "Threshold": 2,
    "Count": 8421,
    "Rate": 0.6467245219261194
  },
  {
    "Threshold": 3,
    "Count": 6619,
    "Rate": 0.5083326933415252
  },
  {
    "Threshold": 5,
    "Count": 4889,
    "Rate": 0.3754703939789571
  },
  {
    "Threshold": 10,
    "Count": 3057,
    "Rate": 0.23477459488518546
  },
  {
    "Threshold": 20,
    "Count": 1785,
    "Rate": 0.1370862452960602
  },
  {
    "Threshold": 30,
    "Count": 1190,
    "Rate": 0.09139083019737347
  },
  {
    "Threshold": 50,
    "Count": 647,
    "Rate": 0.04968896398126104
  },
  {
    "Threshold": 75,
    "Count": 377,
    "Rate": 0.028953229398663696
  },
  {
    "Threshold": 100,
    "Count": 245,
    "Rate": 0.018815759158282774
  },
  {
    "Threshold": 150,
    "Count": 126,
    "Rate": 0.009676676138545427
  },
  {
    "Threshold": 200,
    "Count": 70,
    "Rate": 0.005375931188080793
  }
]
```

## Role Distribution

| Role | Keys | Champions | Mean | Median | P75 | P90 | Max | >=5 | >=10 | >=20 | >=50 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| TOP | 3,853 | 162 | 8.07 | 2.0 | 7.0 | 20.0 | 262 | 1,350 (35.0%) | 771 (20.0%) | 399 (10.4%) | 109 (2.8%) |
| JUNGLE | 2,025 | 107 | 15.35 | 5.0 | 14.0 | 35.0 | 622 | 1,029 (50.8%) | 683 (33.7%) | 391 (19.3%) | 124 (6.1%) |
| MID | 3,152 | 160 | 9.86 | 2.0 | 8.0 | 26.0 | 234 | 1,091 (34.6%) | 700 (22.2%) | 423 (13.4%) | 145 (4.6%) |
| ADC | 1,485 | 137 | 20.93 | 2.0 | 9.0 | 38.6 | 1,083 | 532 (35.8%) | 363 (24.4%) | 239 (16.1%) | 123 (8.3%) |
| SUPPORT | 2,506 | 152 | 12.40 | 2.0 | 8.0 | 27.0 | 425 | 887 (35.4%) | 540 (21.5%) | 333 (13.3%) | 146 (5.8%) |

## Highest Sample Matchups

```json
[
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Kaisa",
    "SampleSize": 1083
  },
  {
    "Role": "ADC",
    "ChampionA": "Jhin",
    "ChampionB": "Kaisa",
    "SampleSize": 1062
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Yunara",
    "SampleSize": 1015
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Xayah",
    "SampleSize": 796
  },
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Jhin",
    "SampleSize": 685
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Vi",
    "SampleSize": 622
  },
  {
    "Role": "ADC",
    "ChampionA": "Jhin",
    "ChampionB": "Yunara",
    "SampleSize": 568
  },
  {
    "Role": "ADC",
    "ChampionA": "Caitlyn",
    "ChampionB": "Jhin",
    "SampleSize": 519
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Lucian",
    "SampleSize": 517
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Nidalee",
    "SampleSize": 511
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "Graves",
    "ChampionB": "LeeSin",
    "SampleSize": 509
  },
  {
    "Role": "ADC",
    "ChampionA": "Aphelios",
    "ChampionB": "Kaisa",
    "SampleSize": 460
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Qiyana",
    "SampleSize": 459
  },
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Yunara",
    "SampleSize": 458
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Lulu",
    "ChampionB": "Nautilus",
    "SampleSize": 425
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Sylas",
    "SampleSize": 406
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Nautilus",
    "ChampionB": "Thresh",
    "SampleSize": 405
  },
  {
    "Role": "ADC",
    "ChampionA": "Jinx",
    "ChampionB": "Kaisa",
    "SampleSize": 399
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Blitzcrank",
    "ChampionB": "Thresh",
    "SampleSize": 399
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Zeri",
    "SampleSize": 384
  }
]
```

## Win Rate Distribution

Small-sample extreme rates are descriptive sampling behavior, not matchup strength labels.

```json
[
  {
    "Label": "1-4",
    "Count": 8132,
    "Distribution": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.4945995245122133,
      "Median": 0.5,
      "P10": 0,
      "P25": 0,
      "P50": 0.5,
      "P75": 1,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.43109037045540377
    },
    "Extremes": {
      "AtMost30": {
        "Count": 3154,
        "Rate": 0.3878504672897196
      },
      "AtMost40": {
        "Count": 3540,
        "Rate": 0.4353172651254304
      },
      "AtLeast60": {
        "Count": 3463,
        "Rate": 0.42584849975405803
      },
      "AtLeast70": {
        "Count": 3060,
        "Rate": 0.37629119527791444
      }
    }
  },
  {
    "Label": "5-9",
    "Count": 1832,
    "Distribution": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.508306907187907,
      "Median": 0.5,
      "P10": 0.2222222222222222,
      "P25": 0.375,
      "P50": 0.5,
      "P75": 0.6666666666666666,
      "P90": 0.8,
      "P95": 0.8333333333333334,
      "P99": 1,
      "StandardDeviation": 0.19903885564523482
    },
    "Extremes": {
      "AtMost30": {
        "Count": 277,
        "Rate": 0.15120087336244542
      },
      "AtMost40": {
        "Count": 636,
        "Rate": 0.3471615720524017
      },
      "AtLeast60": {
        "Count": 687,
        "Rate": 0.375
      },
      "AtLeast70": {
        "Count": 305,
        "Rate": 0.16648471615720525
      }
    }
  },
  {
    "Label": "10-19",
    "Count": 1272,
    "Distribution": {
      "Min": 0,
      "Max": 0.9,
      "Mean": 0.4979600607914744,
      "Median": 0.5,
      "P10": 0.3157894736842105,
      "P25": 0.4,
      "P50": 0.5,
      "P75": 0.6,
      "P90": 0.6824561403508788,
      "P95": 0.7142857142857143,
      "P99": 0.8181818181818182,
      "StandardDeviation": 0.13931188075009904
    },
    "Extremes": {
      "AtMost30": {
        "Count": 110,
        "Rate": 0.08647798742138364
      },
      "AtMost40": {
        "Count": 342,
        "Rate": 0.2688679245283019
      },
      "AtLeast60": {
        "Count": 325,
        "Rate": 0.2555031446540881
      },
      "AtLeast70": {
        "Count": 95,
        "Rate": 0.07468553459119497
      }
    }
  },
  {
    "Label": "20-49",
    "Count": 1138,
    "Distribution": {
      "Min": 0.1724137931034483,
      "Max": 0.8461538461538461,
      "Mean": 0.49670536820715244,
      "Median": 0.5,
      "P10": 0.375,
      "P25": 0.43478260869565216,
      "P50": 0.5,
      "P75": 0.5586525307797537,
      "P90": 0.6153846153846154,
      "P95": 0.6538461538461539,
      "P99": 0.75,
      "StandardDeviation": 0.09678257609855764
    },
    "Extremes": {
      "AtMost30": {
        "Count": 24,
        "Rate": 0.0210896309314587
      },
      "AtMost40": {
        "Count": 183,
        "Rate": 0.1608084358523726
      },
      "AtLeast60": {
        "Count": 169,
        "Rate": 0.14850615114235502
      },
      "AtLeast70": {
        "Count": 25,
        "Rate": 0.021968365553602813
      }
    }
  },
  {
    "Label": "50-99",
    "Count": 402,
    "Distribution": {
      "Min": 0.3068181818181818,
      "Max": 0.6891891891891891,
      "Mean": 0.5005083732593653,
      "Median": 0.5,
      "P10": 0.4168918918918919,
      "P25": 0.45268123138033767,
      "P50": 0.5,
      "P75": 0.5451668584579976,
      "P90": 0.5936342592592593,
      "P95": 0.6180946450809465,
      "P99": 0.6666216216216216,
      "StandardDeviation": 0.0694859894801553
    },
    "Extremes": {
      "AtMost30": {
        "Count": 0,
        "Rate": 0
      },
      "AtMost40": {
        "Count": 29,
        "Rate": 0.07213930348258707
      },
      "AtLeast60": {
        "Count": 38,
        "Rate": 0.0945273631840796
      },
      "AtLeast70": {
        "Count": 0,
        "Rate": 0
      }
    }
  },
  {
    "Label": "100\u002B",
    "Count": 245,
    "Distribution": {
      "Min": 0.35,
      "Max": 0.6074766355140186,
      "Mean": 0.4940611107724794,
      "Median": 0.4943820224719101,
      "P10": 0.43322995126273817,
      "P25": 0.46473029045643155,
      "P50": 0.4943820224719101,
      "P75": 0.5255813953488372,
      "P90": 0.5545552874615193,
      "P95": 0.5694615384615384,
      "P99": 0.6032822821353427,
      "StandardDeviation": 0.04558621894447986
    },
    "Extremes": {
      "AtMost30": {
        "Count": 0,
        "Rate": 0
      },
      "AtMost40": {
        "Count": 1,
        "Rate": 0.004081632653061225
      },
      "AtLeast60": {
        "Count": 3,
        "Rate": 0.012244897959183673
      },
      "AtLeast70": {
        "Count": 0,
        "Rate": 0
      }
    }
  }
]
```

## Wilson Confidence Analysis

95% Wilson score intervals are used; no production confidence label is applied.

```json
[
  {
    "SampleSize": 5,
    "Interval": {
      "Lower": 0.17042358064358776,
      "Upper": 0.8295764193564122,
      "Width": 0.6591528387128245
    }
  },
  {
    "SampleSize": 10,
    "Interval": {
      "Lower": 0.236593090512564,
      "Upper": 0.7634069094874361,
      "Width": 0.5268138189748721
    }
  },
  {
    "SampleSize": 20,
    "Interval": {
      "Lower": 0.29929800819821234,
      "Upper": 0.7007019918017876,
      "Width": 0.40140398360357527
    }
  },
  {
    "SampleSize": 30,
    "Interval": {
      "Lower": 0.33154125640533766,
      "Upper": 0.6684587435946623,
      "Width": 0.3369174871893246
    }
  },
  {
    "SampleSize": 50,
    "Interval": {
      "Lower": 0.3664451431682858,
      "Upper": 0.6335548568317142,
      "Width": 0.2671097136634284
    }
  },
  {
    "SampleSize": 75,
    "Interval": {
      "Lower": 0.3896326107008893,
      "Upper": 0.6103673892991107,
      "Width": 0.22073477859822144
    }
  },
  {
    "SampleSize": 100,
    "Interval": {
      "Lower": 0.4038315303659956,
      "Upper": 0.5961684696340044,
      "Width": 0.19233693926800877
    }
  },
  {
    "SampleSize": 200,
    "Interval": {
      "Lower": 0.43136085960389187,
      "Upper": 0.5686391403961082,
      "Width": 0.13727828079221632
    }
  }
]
```

Actual interval-width distribution: median 69.99%, p75 79.35%, p90 79.35%, p95 81.09%.

```json
[
  {
    "Label": "1-4",
    "Width": {
      "Min": 0.4898908364545973,
      "Max": 0.8109375885315386,
      "Mean": 0.7544523677286025,
      "Median": 0.7934506856227626,
      "P10": 0.6576197724933469,
      "P25": 0.730848454477556,
      "P50": 0.7934506856227626,
      "P75": 0.7934506856227626,
      "P90": 0.8109375885315386,
      "P95": 0.8109375885315386,
      "P99": 0.8109375885315386,
      "StandardDeviation": 0.06888924202214496
    }
  },
  {
    "Label": "5-9",
    "Width": {
      "Min": 0.32440756488388023,
      "Max": 0.6516549444913392,
      "Mean": 0.578343489121216,
      "Median": 0.5882412616050445,
      "P10": 0.5192453569377221,
      "P25": 0.5445701854068425,
      "P50": 0.5882412616050445,
      "P75": 0.6247673870346988,
      "P90": 0.6516549444913391,
      "P95": 0.6516549444913392,
      "P99": 0.6516549444913392,
      "StandardDeviation": 0.05666324579417111
    }
  },
  {
    "Label": "10-19",
    "Width": {
      "Min": 0.2775327998628892,
      "Max": 0.5268138189748721,
      "Mean": 0.4569310011322601,
      "Median": 0.4602855651587514,
      "P10": 0.4058220518329072,
      "P25": 0.4287165849758614,
      "P50": 0.4602855651587514,
      "P75": 0.4924368047325878,
      "P90": 0.5071957463660803,
      "P95": 0.5191459005601056,
      "P99": 0.5268138189748721,
      "StandardDeviation": 0.04057434447673904
    }
  },
  {
    "Label": "20-49",
    "Width": {
      "Min": 0.2486635252999425,
      "Max": 0.40140398360357527,
      "Mean": 0.3346934156359121,
      "Median": 0.33625311472426955,
      "P10": 0.28228582658817125,
      "P25": 0.3026409539041738,
      "P50": 0.33625311472426955,
      "P75": 0.36470197095530255,
      "P90": 0.38555787452549956,
      "P95": 0.392864593156214,
      "P99": 0.3997167992940334,
      "StandardDeviation": 0.037948896609136806
    }
  },
  {
    "Label": "50-99",
    "Width": {
      "Min": 0.18534151025142165,
      "Max": 0.26691119823377346,
      "Mean": 0.23072734437804338,
      "Median": 0.2296710507697889,
      "P10": 0.20116751075952724,
      "P25": 0.21400507038983574,
      "P50": 0.2296710507697889,
      "P75": 0.2501643854476939,
      "P90": 0.25937622542596067,
      "P95": 0.26210172522154096,
      "P99": 0.2653177276708598,
      "StandardDeviation": 0.0210327949495766
    }
  },
  {
    "Label": "100\u002B",
    "Width": {
      "Min": 0.05884192823925494,
      "Max": 0.19229989135531694,
      "Mean": 0.15061428320015244,
      "Median": 0.1573435367503111,
      "P10": 0.10401308790883634,
      "P25": 0.12976549870567367,
      "P50": 0.1573435367503111,
      "P75": 0.17612258917783224,
      "P90": 0.18419334877736898,
      "P95": 0.18949439057061399,
      "P99": 0.19174160009653438,
      "StandardDeviation": 0.0314946583037224
    }
  }
]
```

## Level Metric Quality

```json
[
  {
    "Label": "L2",
    "UnknownTotal": 374,
    "UnknownRate": 0.0024070796460176993,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.49525457596360156,
      "Median": 0.5,
      "P10": 0,
      "P25": 0,
      "P50": 0.5,
      "P75": 1,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.38647082181090425
    },
    "HighestUnknownAll": [
      {
        "Role": "ADC",
        "ChampionA": "Kalista",
        "ChampionB": "Smolder",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "ADC",
        "ChampionA": "Singed",
        "ChampionB": "Ziggs",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Brand",
        "ChampionB": "Jayce",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Gragas",
        "ChampionB": "Rengar",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Morgana",
        "ChampionB": "Nocturne",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "JarvanIV",
        "ChampionB": "Syndra",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "KogMaw",
        "ChampionB": "Nocturne",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Sett",
        "ChampionB": "Vladimir",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Viktor",
        "ChampionB": "Zilean",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "TOP",
        "ChampionA": "Chogath",
        "ChampionB": "Warwick",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Ekko",
        "ChampionB": "Naafiri",
        "SampleSize": 19,
        "Unknown": 2,
        "UnknownRate": 0.10526315789473684
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "LeeSin",
        "ChampionB": "Volibear",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Lillia",
        "ChampionB": "Taliyah",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Rengar",
        "ChampionB": "Taliyah",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "MID",
        "ChampionA": "Fizz",
        "ChampionB": "Qiyana",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "Heimerdinger",
        "ChampionB": "Ryze",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Lillia",
        "ChampionB": "RekSai",
        "SampleSize": 21,
        "Unknown": 2,
        "UnknownRate": 0.09523809523809523
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Gwen",
        "ChampionB": "Hecarim",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "TOP",
        "ChampionA": "Kennen",
        "ChampionB": "Yasuo",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "TOP",
        "ChampionA": "Rumble",
        "ChampionB": "Zaahen",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      }
    ]
  },
  {
    "Label": "L3",
    "UnknownTotal": 250,
    "UnknownRate": 0.0016090104585679806,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.4938239872418264,
      "Median": 0.5,
      "P10": 0,
      "P25": 0.08333333333333333,
      "P50": 0.5,
      "P75": 0.8571428571428571,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.37180733014392203
    },
    "HighestUnknownAll": [
      {
        "Role": "MID",
        "ChampionA": "Annie",
        "ChampionB": "Nocturne",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Akshan",
        "ChampionB": "Veigar",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "MID",
        "ChampionA": "Cassiopeia",
        "ChampionB": "Karthus",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Hwei",
        "ChampionB": "Seraphine",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Karthus",
        "ChampionB": "Leona",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "TOP",
        "ChampionA": "Naafiri",
        "ChampionB": "Renekton",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "ADC",
        "ChampionA": "Zeri",
        "ChampionB": "Zyra",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Aatrox",
        "ChampionB": "XinZhao",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Quinn",
        "ChampionB": "Zac",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      },
      {
        "Role": "MID",
        "ChampionA": "Gangplank",
        "ChampionB": "Jayce",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "MID",
        "ChampionA": "Akali",
        "ChampionB": "Aurora",
        "SampleSize": 20,
        "Unknown": 2,
        "UnknownRate": 0.1
      },
      {
        "Role": "MID",
        "ChampionA": "Anivia",
        "ChampionB": "Talon",
        "SampleSize": 20,
        "Unknown": 2,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Briar",
        "ChampionB": "JarvanIV",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Rengar",
        "ChampionB": "Taliyah",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "MID",
        "ChampionA": "Fizz",
        "ChampionB": "Qiyana",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Karthus",
        "ChampionB": "MonkeyKing",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "MID",
        "ChampionA": "Lux",
        "ChampionB": "Qiyana",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "TOP",
        "ChampionA": "Darius",
        "ChampionB": "Vayne",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Khazix",
        "ChampionB": "MonkeyKing",
        "SampleSize": 23,
        "Unknown": 2,
        "UnknownRate": 0.08695652173913043
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Chogath",
        "ChampionB": "Trundle",
        "SampleSize": 12,
        "Unknown": 1,
        "UnknownRate": 0.08333333333333333
      }
    ]
  },
  {
    "Label": "L6",
    "UnknownTotal": 82,
    "UnknownRate": 0.0005277554304102977,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.49709031095870587,
      "Median": 0.5,
      "P10": 0,
      "P25": 0.2,
      "P50": 0.5,
      "P75": 0.8,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.36022882041101834
    },
    "HighestUnknownAll": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Irelia",
        "ChampionB": "LeeSin",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Galio",
        "ChampionB": "Nasus",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Karthus",
        "ChampionB": "Senna",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Leona",
        "ChampionB": "Malphite",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Senna",
        "ChampionB": "Twitch",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Maokai",
        "ChampionB": "Pantheon",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "TOP",
        "ChampionA": "DrMundo",
        "ChampionB": "MasterYi",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      },
      {
        "Role": "TOP",
        "ChampionA": "Heimerdinger",
        "ChampionB": "Taliyah",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      },
      {
        "Role": "MID",
        "ChampionA": "Aurora",
        "ChampionB": "Ekko",
        "SampleSize": 5,
        "Unknown": 1,
        "UnknownRate": 0.2
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Amumu",
        "ChampionB": "Pyke",
        "SampleSize": 5,
        "Unknown": 1,
        "UnknownRate": 0.2
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "TOP",
        "ChampionA": "Gangplank",
        "ChampionB": "Ornn",
        "SampleSize": 12,
        "Unknown": 1,
        "UnknownRate": 0.08333333333333333
      },
      {
        "Role": "MID",
        "ChampionA": "Akshan",
        "ChampionB": "Katarina",
        "SampleSize": 16,
        "Unknown": 1,
        "UnknownRate": 0.0625
      },
      {
        "Role": "TOP",
        "ChampionA": "Sett",
        "ChampionB": "Singed",
        "SampleSize": 16,
        "Unknown": 1,
        "UnknownRate": 0.0625
      },
      {
        "Role": "MID",
        "ChampionA": "Pantheon",
        "ChampionB": "Sylas",
        "SampleSize": 17,
        "Unknown": 1,
        "UnknownRate": 0.058823529411764705
      },
      {
        "Role": "MID",
        "ChampionA": "Hwei",
        "ChampionB": "Malzahar",
        "SampleSize": 18,
        "Unknown": 1,
        "UnknownRate": 0.05555555555555555
      },
      {
        "Role": "TOP",
        "ChampionA": "Kennen",
        "ChampionB": "Sylas",
        "SampleSize": 20,
        "Unknown": 1,
        "UnknownRate": 0.05
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Braum",
        "ChampionB": "Sylas",
        "SampleSize": 22,
        "Unknown": 1,
        "UnknownRate": 0.045454545454545456
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Khazix",
        "ChampionB": "MonkeyKing",
        "SampleSize": 23,
        "Unknown": 1,
        "UnknownRate": 0.043478260869565216
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Milio",
        "ChampionB": "Pantheon",
        "SampleSize": 23,
        "Unknown": 1,
        "UnknownRate": 0.043478260869565216
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Neeko",
        "ChampionB": "Poppy",
        "SampleSize": 26,
        "Unknown": 1,
        "UnknownRate": 0.038461538461538464
      }
    ]
  }
]
```

Role-level Unknown rates:

```json
[
  {
    "Role": "TOP",
    "L2": 0.001512469831053902,
    "L3": 0.0006757843925985519,
    "L6": 0.0006436041834271923
  },
  {
    "Role": "JUNGLE",
    "L2": 0.0053740949316170555,
    "L3": 0.003024939662107804,
    "L6": 0.0002896218825422365
  },
  {
    "Role": "MID",
    "L2": 0.0029605792437650846,
    "L3": 0.002831858407079646,
    "L6": 0.0006114239742558326
  },
  {
    "Role": "ADC",
    "L2": 0.001094127111826227,
    "L3": 0.0008366854384553499,
    "L6": 0.00016090104585679806
  },
  {
    "Role": "SUPPORT",
    "L2": 0.001094127111826227,
    "L3": 0.0006757843925985519,
    "L6": 0.0009332260659694288
  }
]
```

Level rates use A count / full SampleSize, matching the production calculator; Unknown is not removed from the denominator.

## Solo Kill Distribution

Solo kills are event counts. Per-match values are events / SampleSize and may exceed 1.

Combined A/B per-match distribution: mean 0.6366, median 0.2759, p75 1.0000, p90 1.8750, p95 2.2090, max 10.0000.

```json
[
  {
    "Label": "1-4",
    "Distribution": {
      "Min": 0,
      "Max": 10,
      "Mean": 0.6619476553533385,
      "Median": 0,
      "P10": 0,
      "P25": 0,
      "P50": 0,
      "P75": 1,
      "P90": 2,
      "P95": 3,
      "P99": 4.5,
      "StandardDeviation": 1.0144263375979976
    }
  },
  {
    "Label": "5-9",
    "Distribution": {
      "Min": 0,
      "Max": 5,
      "Mean": 0.6305006454910939,
      "Median": 0.4,
      "P10": 0,
      "P25": 0.125,
      "P50": 0.4,
      "P75": 1,
      "P90": 1.6,
      "P95": 2,
      "P99": 2.8123333333333296,
      "StandardDeviation": 0.6776778617007427
    }
  },
  {
    "Label": "10-19",
    "Distribution": {
      "Min": 0,
      "Max": 3.5,
      "Mean": 0.6062976169321372,
      "Median": 0.4,
      "P10": 0.015789473684211124,
      "P25": 0.13333333333333333,
      "P50": 0.4,
      "P75": 0.9605263157894737,
      "P90": 1.4545454545454546,
      "P95": 1.7685520361990945,
      "P99": 2.466666666666667,
      "StandardDeviation": 0.5921414180148337
    }
  },
  {
    "Label": "20-49",
    "Distribution": {
      "Min": 0,
      "Max": 3.3636363636363638,
      "Mean": 0.587454951982725,
      "Median": 0.38095238095238093,
      "P10": 0.047619047619047616,
      "P25": 0.13333333333333333,
      "P50": 0.38095238095238093,
      "P75": 0.9302325581395349,
      "P90": 1.445906432748538,
      "P95": 1.7142857142857142,
      "P99": 2.273510971786834,
      "StandardDeviation": 0.5617889905589873
    }
  },
  {
    "Label": "50-99",
    "Distribution": {
      "Min": 0,
      "Max": 2.6206896551724137,
      "Mean": 0.5529168932745506,
      "Median": 0.32908944399677675,
      "P10": 0.04142185663924798,
      "P25": 0.1342948717948718,
      "P50": 0.32908944399677675,
      "P75": 0.8827791986359761,
      "P90": 1.4277777777777778,
      "P95": 1.6397831325301202,
      "P99": 2.112914769030579,
      "StandardDeviation": 0.546478190100943
    }
  },
  {
    "Label": "100\u002B",
    "Distribution": {
      "Min": 0,
      "Max": 2.46218487394958,
      "Mean": 0.3645029117191272,
      "Median": 0.22402597402597402,
      "P10": 0.03502722323049002,
      "P25": 0.09690877304513669,
      "P50": 0.22402597402597402,
      "P75": 0.3701073232323232,
      "P90": 1.0121942697414403,
      "P95": 1.3302984084880631,
      "P99": 2.0816209150326817,
      "StandardDeviation": 0.4338645254052587
    }
  }
]
```

Top per-match entries:

```json
[
  {
    "Role": "TOP",
    "ChampionA": "Irelia",
    "ChampionB": "Locke",
    "SampleSize": 1,
    "Side": "B",
    "Events": 10,
    "PerMatch": 10
  },
  {
    "Role": "TOP",
    "ChampionA": "Nilah",
    "ChampionB": "Vladimir",
    "SampleSize": 1,
    "Side": "B",
    "Events": 9,
    "PerMatch": 9
  },
  {
    "Role": "TOP",
    "ChampionA": "Vayne",
    "ChampionB": "Viego",
    "SampleSize": 1,
    "Side": "A",
    "Events": 9,
    "PerMatch": 9
  },
  {
    "Role": "MID",
    "ChampionA": "Nocturne",
    "ChampionB": "Yone",
    "SampleSize": 1,
    "Side": "B",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Akali",
    "ChampionB": "Smolder",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "MasterYi",
    "ChampionB": "Poppy",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Nocturne",
    "ChampionB": "Sett",
    "SampleSize": 1,
    "Side": "B",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Qiyana",
    "ChampionB": "Riven",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Akali",
    "ChampionB": "Briar",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Azir",
    "ChampionB": "Vladimir",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Draven",
    "ChampionB": "Urgot",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Garen",
    "ChampionB": "Velkoz",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Gwen",
    "ChampionB": "MasterYi",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Karthus",
    "ChampionB": "LeeSin",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Leblanc",
    "ChampionB": "Velkoz",
    "SampleSize": 1,
    "Side": "A",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "MasterYi",
    "ChampionB": "Sett",
    "SampleSize": 1,
    "Side": "B",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Qiyana",
    "ChampionB": "Viktor",
    "SampleSize": 1,
    "Side": "A",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Talon",
    "ChampionB": "Vayne",
    "SampleSize": 1,
    "Side": "A",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "TOP",
    "ChampionA": "Irelia",
    "ChampionB": "XinZhao",
    "SampleSize": 4,
    "Side": "A",
    "Events": 24,
    "PerMatch": 6
  },
  {
    "Role": "MID",
    "ChampionA": "Corki",
    "ChampionB": "Irelia",
    "SampleSize": 1,
    "Side": "B",
    "Events": 6,
    "PerMatch": 6
  }
]
```

## Metric Correlations

Pearson correlations describe association across canonical matchup aggregates; they do not establish causation.

```json
[
  {
    "MinimumSample": 20,
    "MatchupCount": 1785,
    "Pairs": {
      "FirstLevel2RateA vs FirstLevel3RateA": 0.674515316200271,
      "FirstLevel2RateA vs FirstLevel6RateA": 0.2950305256160163,
      "FirstLevel2RateA vs SoloKillDifference": 0.014096606096834786,
      "FirstLevel3RateA vs FirstLevel6RateA": 0.44262117084261254,
      "FirstLevel3RateA vs SoloKillDifference": 0.14924661714730583,
      "FirstLevel6RateA vs SoloKillDifference": 0.373357116963373,
      "WinRateA vs FirstLevel2RateA": -0.000762266405739375,
      "WinRateA vs FirstLevel3RateA": 0.046638762208087936,
      "WinRateA vs FirstLevel6RateA": 0.09494442927145809,
      "WinRateA vs SoloKillDifference": 0.15492969230094988
    }
  },
  {
    "MinimumSample": 50,
    "MatchupCount": 647,
    "Pairs": {
      "FirstLevel2RateA vs FirstLevel3RateA": 0.695799678702756,
      "FirstLevel2RateA vs FirstLevel6RateA": 0.3182260719478626,
      "FirstLevel2RateA vs SoloKillDifference": -0.052285108644583744,
      "FirstLevel3RateA vs FirstLevel6RateA": 0.4940908131257615,
      "FirstLevel3RateA vs SoloKillDifference": 0.09130407167527453,
      "FirstLevel6RateA vs SoloKillDifference": 0.3436567925140982,
      "WinRateA vs FirstLevel2RateA": -0.04427784669814076,
      "WinRateA vs FirstLevel3RateA": 0.02050878686447132,
      "WinRateA vs FirstLevel6RateA": 0.046922719659059996,
      "WinRateA vs SoloKillDifference": 0.20529859105743478
    }
  }
]
```

Directional agreement below is restricted to matchups where A win rate is above 50%; neutral metric values are reported separately.

```json
[
  {
    "MinimumSample": 20,
    "WinRateAAbove50Count": 799,
    "Metrics": [
      {
        "Metric": "FirstLevel2RateA",
        "SameDirection": 403,
        "OppositeDirection": 381,
        "Neutral": 15,
        "AgreementAmongNonNeutral": 0.514030612244898
      },
      {
        "Metric": "FirstLevel3RateA",
        "SameDirection": 394,
        "OppositeDirection": 377,
        "Neutral": 28,
        "AgreementAmongNonNeutral": 0.5110246433203631
      },
      {
        "Metric": "FirstLevel6RateA",
        "SameDirection": 418,
        "OppositeDirection": 352,
        "Neutral": 29,
        "AgreementAmongNonNeutral": 0.5428571428571428
      },
      {
        "Metric": "SoloKillDifference",
        "SameDirection": 416,
        "OppositeDirection": 318,
        "Neutral": 65,
        "AgreementAmongNonNeutral": 0.5667574931880109
      }
    ]
  },
  {
    "MinimumSample": 50,
    "WinRateAAbove50Count": 301,
    "Metrics": [
      {
        "Metric": "FirstLevel2RateA",
        "SameDirection": 153,
        "OppositeDirection": 146,
        "Neutral": 2,
        "AgreementAmongNonNeutral": 0.5117056856187291
      },
      {
        "Metric": "FirstLevel3RateA",
        "SameDirection": 152,
        "OppositeDirection": 144,
        "Neutral": 5,
        "AgreementAmongNonNeutral": 0.5135135135135135
      },
      {
        "Metric": "FirstLevel6RateA",
        "SameDirection": 158,
        "OppositeDirection": 135,
        "Neutral": 8,
        "AgreementAmongNonNeutral": 0.5392491467576792
      },
      {
        "Metric": "SoloKillDifference",
        "SameDirection": 166,
        "OppositeDirection": 119,
        "Neutral": 16,
        "AgreementAmongNonNeutral": 0.5824561403508772
      }
    ]
  }
]
```

Canonical A weighted win rate: 49.62%. ChampionA is lexical, so deviation from 50% is monitored as a normalization sanity check, not assumed to be causal.

## Candidate Reliability Thresholds

Candidate only; not applied to the WPF application.

```json
[
  {
    "Label": "\uD45C\uBCF8 \uBD80\uC871",
    "Minimum": 1,
    "Maximum": 4,
    "Count": 8132,
    "Rate": 0.6245296060210429
  },
  {
    "Label": "\uCC38\uACE0\uC6A9",
    "Minimum": 5,
    "Maximum": 19,
    "Count": 3104,
    "Rate": 0.23838414868289687
  },
  {
    "Label": "\uBCF4\uD1B5",
    "Minimum": 20,
    "Maximum": 49,
    "Count": 1138,
    "Rate": 0.08739728131479917
  },
  {
    "Label": "\uCDA9\uBD84",
    "Minimum": 50,
    "Maximum": null,
    "Count": 647,
    "Rate": 0.04968896398126104
  }
]
```

The proposed exploratory bands are `<5`, `5-19`, `20-49`, and `50+`. They jointly expose coverage and Wilson-width tradeoffs without deleting rare/off-meta matchups. Stage 9-1 should use these as display reliability labels only, retaining raw statistics and a clear low-sample warning.

This dataset is MASTER_PLUS_SEEDED: seeds were Master/Grandmaster/Challenger at collection time; all ten participants were not tier-verified. Results are patch-specific and are not merged with other patches.
