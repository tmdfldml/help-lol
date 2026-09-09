# Statistics Reliability Analysis

## Dataset Summary

- Patch: `16.17`
- Accepted matches: 51,649
- Observations: 258,245
- Matchup keys: 15,547
- Tier scope: `MASTER_PLUS_SEEDED` / `SEED_PLAYER_TIER_AT_COLLECTION`
- All participants tier verified: `false`
- Queue: 420; minimum duration: 900s
- Dataset revision: 1788854099077

## Sample Size Distribution

Percentiles use linear interpolation at index `(N-1)*p`.

Min 1; mean 16.61; p25 1.00; median/p50 3.00; p75 11.00; p90 38.00; p95 71.00; p99 208.00; max 1,816.

Low-sample keys: n=1: 5,055, n=2: 2,072, n=3-4: 2,106, n=5-9: 2,066.

```json
[
  {
    "Threshold": 1,
    "Count": 15547,
    "Rate": 1
  },
  {
    "Threshold": 2,
    "Count": 10492,
    "Rate": 0.6748568855727793
  },
  {
    "Threshold": 3,
    "Count": 8420,
    "Rate": 0.541583585257606
  },
  {
    "Threshold": 5,
    "Count": 6314,
    "Rate": 0.4061233678523188
  },
  {
    "Threshold": 10,
    "Count": 4248,
    "Rate": 0.27323599408245963
  },
  {
    "Threshold": 20,
    "Count": 2659,
    "Rate": 0.17102978066508007
  },
  {
    "Threshold": 30,
    "Count": 1933,
    "Rate": 0.12433266868206085
  },
  {
    "Threshold": 50,
    "Count": 1170,
    "Rate": 0.07525567633627066
  },
  {
    "Threshold": 75,
    "Count": 730,
    "Rate": 0.04695439634656204
  },
  {
    "Threshold": 100,
    "Count": 515,
    "Rate": 0.03312536180613623
  },
  {
    "Threshold": 150,
    "Count": 276,
    "Rate": 0.017752621084453593
  },
  {
    "Threshold": 200,
    "Count": 180,
    "Rate": 0.011577796359426256
  }
]
```

## Role Distribution

| Role | Keys | Champions | Mean | Median | P75 | P90 | Max | >=5 | >=10 | >=20 | >=50 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| TOP | 4,646 | 165 | 11.12 | 3.0 | 9.0 | 28.0 | 457 | 1,801 (38.8%) | 1,158 (24.9%) | 658 (14.2%) | 228 (4.9%) |
| JUNGLE | 2,302 | 121 | 22.44 | 6.0 | 20.0 | 53.9 | 1,013 | 1,254 (54.5%) | 922 (40.1%) | 584 (25.4%) | 256 (11.1%) |
| MID | 3,792 | 164 | 13.62 | 3.0 | 9.0 | 36.0 | 444 | 1,414 (37.3%) | 936 (24.7%) | 607 (16.0%) | 279 (7.4%) |
| ADC | 1,794 | 144 | 28.79 | 3.0 | 10.0 | 49.0 | 1,816 | 679 (37.8%) | 463 (25.8%) | 325 (18.1%) | 174 (9.7%) |
| SUPPORT | 3,013 | 160 | 17.14 | 3.0 | 10.0 | 36.0 | 720 | 1,166 (38.7%) | 769 (25.5%) | 485 (16.1%) | 233 (7.7%) |

## Highest Sample Matchups

```json
[
  {
    "Role": "ADC",
    "ChampionA": "Jhin",
    "ChampionB": "Kaisa",
    "SampleSize": 1816
  },
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Kaisa",
    "SampleSize": 1761
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Yunara",
    "SampleSize": 1655
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Xayah",
    "SampleSize": 1281
  },
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Jhin",
    "SampleSize": 1209
  },
  {
    "Role": "ADC",
    "ChampionA": "Jhin",
    "ChampionB": "Yunara",
    "SampleSize": 1035
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Vi",
    "SampleSize": 1013
  },
  {
    "Role": "ADC",
    "ChampionA": "Caitlyn",
    "ChampionB": "Jhin",
    "SampleSize": 906
  },
  {
    "Role": "ADC",
    "ChampionA": "Kaisa",
    "ChampionB": "Lucian",
    "SampleSize": 868
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "Graves",
    "ChampionB": "LeeSin",
    "SampleSize": 827
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Nidalee",
    "SampleSize": 777
  },
  {
    "Role": "ADC",
    "ChampionA": "Ezreal",
    "ChampionB": "Yunara",
    "SampleSize": 766
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Qiyana",
    "SampleSize": 766
  },
  {
    "Role": "ADC",
    "ChampionA": "Aphelios",
    "ChampionB": "Kaisa",
    "SampleSize": 757
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Lulu",
    "ChampionB": "Nautilus",
    "SampleSize": 720
  },
  {
    "Role": "ADC",
    "ChampionA": "Jinx",
    "ChampionB": "Kaisa",
    "SampleSize": 691
  },
  {
    "Role": "JUNGLE",
    "ChampionA": "LeeSin",
    "ChampionB": "Sylas",
    "SampleSize": 685
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Nautilus",
    "ChampionB": "Thresh",
    "SampleSize": 678
  },
  {
    "Role": "SUPPORT",
    "ChampionA": "Blitzcrank",
    "ChampionB": "Thresh",
    "SampleSize": 665
  },
  {
    "Role": "ADC",
    "ChampionA": "Lucian",
    "ChampionB": "Yunara",
    "SampleSize": 618
  }
]
```

## Win Rate Distribution

Small-sample extreme rates are descriptive sampling behavior, not matchup strength labels.

```json
[
  {
    "Label": "1-4",
    "Count": 9233,
    "Distribution": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.4980955991191028,
      "Median": 0.5,
      "P10": 0,
      "P25": 0,
      "P50": 0.5,
      "P75": 1,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.42816825185654755
    },
    "Extremes": {
      "AtMost30": {
        "Count": 3508,
        "Rate": 0.3799415141340843
      },
      "AtMost40": {
        "Count": 3981,
        "Rate": 0.4311708003899058
      },
      "AtLeast60": {
        "Count": 3924,
        "Rate": 0.4249972923210224
      },
      "AtLeast70": {
        "Count": 3483,
        "Rate": 0.3772338351565038
      }
    }
  },
  {
    "Label": "5-9",
    "Count": 2066,
    "Distribution": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.5098753053980498,
      "Median": 0.5,
      "P10": 0.2222222222222222,
      "P25": 0.375,
      "P50": 0.5,
      "P75": 0.6666666666666666,
      "P90": 0.8,
      "P95": 0.8333333333333334,
      "P99": 1,
      "StandardDeviation": 0.2014427776262772
    },
    "Extremes": {
      "AtMost30": {
        "Count": 305,
        "Rate": 0.14762826718296224
      },
      "AtMost40": {
        "Count": 714,
        "Rate": 0.345595353339787
      },
      "AtLeast60": {
        "Count": 784,
        "Rate": 0.37947725072604066
      },
      "AtLeast70": {
        "Count": 363,
        "Rate": 0.17570183930300096
      }
    }
  },
  {
    "Label": "10-19",
    "Count": 1589,
    "Distribution": {
      "Min": 0.07692307692307693,
      "Max": 0.9230769230769231,
      "Mean": 0.5018443437272978,
      "Median": 0.5,
      "P10": 0.3333333333333333,
      "P25": 0.4117647058823529,
      "P50": 0.5,
      "P75": 0.6,
      "P90": 0.6884615384615387,
      "P95": 0.7272727272727273,
      "P99": 0.8181818181818182,
      "StandardDeviation": 0.1375691129385735
    },
    "Extremes": {
      "AtMost30": {
        "Count": 133,
        "Rate": 0.08370044052863436
      },
      "AtMost40": {
        "Count": 396,
        "Rate": 0.24921334172435494
      },
      "AtLeast60": {
        "Count": 419,
        "Rate": 0.263687853996224
      },
      "AtLeast70": {
        "Count": 143,
        "Rate": 0.08999370673379484
      }
    }
  },
  {
    "Label": "20-49",
    "Count": 1489,
    "Distribution": {
      "Min": 0.19047619047619047,
      "Max": 0.7931034482758621,
      "Mean": 0.49841382791483324,
      "Median": 0.5,
      "P10": 0.37428571428571433,
      "P25": 0.42857142857142855,
      "P50": 0.5,
      "P75": 0.5625,
      "P90": 0.6206896551724138,
      "P95": 0.6666666666666666,
      "P99": 0.7204965517241374,
      "StandardDeviation": 0.09791263102089487
    },
    "Extremes": {
      "AtMost30": {
        "Count": 33,
        "Rate": 0.02216252518468771
      },
      "AtMost40": {
        "Count": 256,
        "Rate": 0.17192746809939558
      },
      "AtLeast60": {
        "Count": 239,
        "Rate": 0.1605104096709201
      },
      "AtLeast70": {
        "Count": 36,
        "Rate": 0.0241773002014775
      }
    }
  },
  {
    "Label": "50-99",
    "Count": 655,
    "Distribution": {
      "Min": 0.22,
      "Max": 0.6964285714285714,
      "Mean": 0.4994738419363772,
      "Median": 0.5,
      "P10": 0.41081213307240705,
      "P25": 0.45454545454545453,
      "P50": 0.5,
      "P75": 0.5454545454545454,
      "P90": 0.5844770959377701,
      "P95": 0.6171212121212121,
      "P99": 0.6491098901098902,
      "StandardDeviation": 0.06761981040682791
    },
    "Extremes": {
      "AtMost30": {
        "Count": 1,
        "Rate": 0.0015267175572519084
      },
      "AtMost40": {
        "Count": 53,
        "Rate": 0.08091603053435115
      },
      "AtLeast60": {
        "Count": 52,
        "Rate": 0.07938931297709924
      },
      "AtLeast70": {
        "Count": 0,
        "Rate": 0
      }
    }
  },
  {
    "Label": "100\u002B",
    "Count": 515,
    "Distribution": {
      "Min": 0.3560606060606061,
      "Max": 0.6574074074074074,
      "Mean": 0.5001076017803567,
      "Median": 0.5,
      "P10": 0.4344421454719083,
      "P25": 0.46527007906176154,
      "P50": 0.5,
      "P75": 0.5355030966978931,
      "P90": 0.5638004101161996,
      "P95": 0.5827669902912621,
      "P99": 0.6270618409803363,
      "StandardDeviation": 0.05058629212130002
    },
    "Extremes": {
      "AtMost30": {
        "Count": 0,
        "Rate": 0
      },
      "AtMost40": {
        "Count": 11,
        "Rate": 0.021359223300970873
      },
      "AtLeast60": {
        "Count": 12,
        "Rate": 0.02330097087378641
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

Actual interval-width distribution: median 65.76%, p75 79.35%, p90 79.35%, p95 81.09%.

```json
[
  {
    "Label": "1-4",
    "Width": {
      "Min": 0.4898908364545973,
      "Max": 0.8109375885315386,
      "Mean": 0.7519744715679524,
      "Median": 0.7934506856227626,
      "P10": 0.6576197724933469,
      "P25": 0.730848454477556,
      "P50": 0.7934506856227626,
      "P75": 0.7934506856227626,
      "P90": 0.8109375885315386,
      "P95": 0.8109375885315386,
      "P99": 0.8109375885315386,
      "StandardDeviation": 0.07069957156369955
    }
  },
  {
    "Label": "5-9",
    "Width": {
      "Min": 0.2991450484195441,
      "Max": 0.6516549444913392,
      "Mean": 0.5757614294129815,
      "Median": 0.5882412616050445,
      "P10": 0.519245356937722,
      "P25": 0.5445701854068425,
      "P50": 0.5882412616050445,
      "P75": 0.6032352737558275,
      "P90": 0.6516549444913391,
      "P95": 0.6516549444913392,
      "P99": 0.6516549444913392,
      "StandardDeviation": 0.05731810245663703
    }
  },
  {
    "Label": "10-19",
    "Width": {
      "Min": 0.3194290879684393,
      "Max": 0.5268138189748721,
      "Mean": 0.4583820519228027,
      "Median": 0.4602855651587514,
      "P10": 0.4058220518329072,
      "P25": 0.4287165849758614,
      "P50": 0.4602855651587514,
      "P75": 0.4924368047325878,
      "P90": 0.5071957463660803,
      "P95": 0.5191459005601056,
      "P99": 0.5268138189748721,
      "StandardDeviation": 0.04014219071502886
    }
  },
  {
    "Label": "20-49",
    "Width": {
      "Min": 0.23318765728390953,
      "Max": 0.40140398360357527,
      "Mean": 0.33162707889561027,
      "Median": 0.33089011629875675,
      "P10": 0.2803851160253831,
      "P25": 0.2993546178568003,
      "P50": 0.33089011629875675,
      "P75": 0.36320809077601,
      "P90": 0.3841991049845175,
      "P95": 0.392864593156214,
      "P99": 0.3997167992940334,
      "StandardDeviation": 0.03834392843658861
    }
  },
  {
    "Label": "50-99",
    "Width": {
      "Min": 0.1867157564262949,
      "Max": 0.2671097136634284,
      "Mean": 0.23215557570364329,
      "Median": 0.23407567910114857,
      "P10": 0.20079610937665787,
      "P25": 0.2141739601826603,
      "P50": 0.23407567910114857,
      "P75": 0.2512381908397985,
      "P90": 0.25992221037476804,
      "P95": 0.2639155487670979,
      "P99": 0.26691119823377346,
      "StandardDeviation": 0.021725795429803085
    }
  },
  {
    "Label": "100\u002B",
    "Width": {
      "Min": 0.04583377746985762,
      "Max": 0.19233693926800877,
      "Mean": 0.14674740358399652,
      "Median": 0.1544663923553169,
      "P10": 0.094720980761127,
      "P25": 0.12700578768795312,
      "P50": 0.1544663923553169,
      "P75": 0.1742267665580023,
      "P90": 0.18483876776694408,
      "P95": 0.18891249575281663,
      "P99": 0.19196685952146123,
      "StandardDeviation": 0.033487955286132465
    }
  }
]
```

## Level Metric Quality

```json
[
  {
    "Label": "L2",
    "UnknownTotal": 609,
    "UnknownRate": 0.0023582257158899496,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.4956429982360203,
      "Median": 0.5,
      "P10": 0,
      "P25": 0.08,
      "P50": 0.5,
      "P75": 0.8888888888888888,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.37904409403555556
    },
    "HighestUnknownAll": [
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
        "ChampionA": "Morgana",
        "ChampionB": "Nocturne",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Yone",
        "ChampionB": "Zyra",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Brand",
        "ChampionB": "Mel",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Chogath",
        "ChampionB": "Kennen",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Graves",
        "ChampionB": "Hwei",
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
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Malphite",
        "ChampionB": "Vi",
        "SampleSize": 10,
        "Unknown": 2,
        "UnknownRate": 0.2
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Ambessa",
        "ChampionB": "Karthus",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Skarner",
        "ChampionB": "Zac",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "MID",
        "ChampionA": "Garen",
        "ChampionB": "Sylas",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Blitzcrank",
        "ChampionB": "Shaco",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "FiddleSticks",
        "ChampionB": "KSante",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "Pantheon",
        "ChampionB": "Udyr",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Locke",
        "ChampionB": "XinZhao",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "MID",
        "ChampionA": "Annie",
        "ChampionB": "Zed",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "MID",
        "ChampionA": "Azir",
        "ChampionB": "Diana",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      }
    ]
  },
  {
    "Label": "L3",
    "UnknownTotal": 412,
    "UnknownRate": 0.0015953842281554338,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.49372693031615045,
      "Median": 0.5,
      "P10": 0,
      "P25": 0.15737874097007223,
      "P50": 0.5,
      "P75": 0.8,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.36314162271687955
    },
    "HighestUnknownAll": [
      {
        "Role": "ADC",
        "ChampionA": "Kaisa",
        "ChampionB": "Lissandra",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Malphite",
        "ChampionB": "Zac",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Annie",
        "ChampionB": "Nocturne",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Brand",
        "ChampionB": "TahmKench",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "TOP",
        "ChampionA": "Varus",
        "ChampionB": "Vex",
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
        "ChampionA": "Chogath",
        "ChampionB": "Poppy",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Nautilus",
        "ChampionB": "Talon",
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
        "ChampionA": "Cassiopeia",
        "ChampionB": "Karthus",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "JUNGLE",
        "ChampionA": "Malphite",
        "ChampionB": "Vi",
        "SampleSize": 10,
        "Unknown": 2,
        "UnknownRate": 0.2
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Graves",
        "ChampionB": "Malphite",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Khazix",
        "ChampionB": "Nunu",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Nami",
        "ChampionB": "Zyra",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "DrMundo",
        "ChampionB": "Qiyana",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "Jayce",
        "ChampionB": "Viktor",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "Kennen",
        "ChampionB": "Zaahen",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Khazix",
        "ChampionB": "Skarner",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Naafiri",
        "ChampionB": "Warwick",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      },
      {
        "Role": "MID",
        "ChampionA": "Akshan",
        "ChampionB": "Anivia",
        "SampleSize": 11,
        "Unknown": 1,
        "UnknownRate": 0.09090909090909091
      }
    ]
  },
  {
    "Label": "L6",
    "UnknownTotal": 142,
    "UnknownRate": 0.000549865437859397,
    "ASideRate": {
      "Min": 0,
      "Max": 1,
      "Mean": 0.49942983377827443,
      "Median": 0.5,
      "P10": 0,
      "P25": 0.25,
      "P50": 0.5,
      "P75": 0.75,
      "P90": 1,
      "P95": 1,
      "P99": 1,
      "StandardDeviation": 0.3501114155235424
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
        "Role": "JUNGLE",
        "ChampionA": "Maokai",
        "ChampionB": "Udyr",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "MID",
        "ChampionA": "Morgana",
        "ChampionB": "Veigar",
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
        "ChampionA": "Lissandra",
        "ChampionB": "Skarner",
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
        "Role": "TOP",
        "ChampionA": "Lissandra",
        "ChampionB": "Malzahar",
        "SampleSize": 1,
        "Unknown": 1,
        "UnknownRate": 1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Leona",
        "ChampionB": "Malphite",
        "SampleSize": 2,
        "Unknown": 1,
        "UnknownRate": 0.5
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Taliyah",
        "ChampionB": "Yuumi",
        "SampleSize": 3,
        "Unknown": 1,
        "UnknownRate": 0.3333333333333333
      }
    ],
    "HighestUnknownSample10": [
      {
        "Role": "ADC",
        "ChampionA": "Varus",
        "ChampionB": "Yasuo",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "MID",
        "ChampionA": "Gangplank",
        "ChampionB": "Sylas",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Amumu",
        "ChampionB": "Pyke",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "TOP",
        "ChampionA": "Gangplank",
        "ChampionB": "Zed",
        "SampleSize": 10,
        "Unknown": 1,
        "UnknownRate": 0.1
      },
      {
        "Role": "SUPPORT",
        "ChampionA": "Elise",
        "ChampionB": "Milio",
        "SampleSize": 12,
        "Unknown": 1,
        "UnknownRate": 0.08333333333333333
      },
      {
        "Role": "TOP",
        "ChampionA": "Anivia",
        "ChampionB": "Renekton",
        "SampleSize": 12,
        "Unknown": 1,
        "UnknownRate": 0.08333333333333333
      },
      {
        "Role": "JUNGLE",
        "ChampionA": "Belveth",
        "ChampionB": "Briar",
        "SampleSize": 14,
        "Unknown": 1,
        "UnknownRate": 0.07142857142857142
      },
      {
        "Role": "MID",
        "ChampionA": "Aurora",
        "ChampionB": "Ekko",
        "SampleSize": 14,
        "Unknown": 1,
        "UnknownRate": 0.07142857142857142
      },
      {
        "Role": "TOP",
        "ChampionA": "Gangplank",
        "ChampionB": "Poppy",
        "SampleSize": 14,
        "Unknown": 1,
        "UnknownRate": 0.07142857142857142
      },
      {
        "Role": "TOP",
        "ChampionA": "Mordekaiser",
        "ChampionB": "Singed",
        "SampleSize": 14,
        "Unknown": 1,
        "UnknownRate": 0.07142857142857142
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
    "L2": 0.0014521094309667176,
    "L3": 0.000658289608704912,
    "L6": 0.0006970125268640245
  },
  {
    "Role": "JUNGLE",
    "L2": 0.005382485624116634,
    "L3": 0.002865495943774323,
    "L6": 0.000329144804352456
  },
  {
    "Role": "MID",
    "L2": 0.002691242812058317,
    "L3": 0.0026718813529787605,
    "L6": 0.0005227593951480184
  },
  {
    "Role": "ADC",
    "L2": 0.0011810490038529303,
    "L3": 0.0009680729539778117,
    "L6": 0.00019361459079556234
  },
  {
    "Role": "SUPPORT",
    "L2": 0.0010842417084551493,
    "L3": 0.0008131812813413618,
    "L6": 0.0010067958721369243
  }
]
```

Level rates use A count / full SampleSize, matching the production calculator; Unknown is not removed from the denominator.

## Solo Kill Distribution

Solo kills are event counts. Per-match values are events / SampleSize and may exceed 1.

Combined A/B per-match distribution: mean 0.6428, median 0.3047, p75 1.0000, p90 1.8333, p95 2.1633, max 10.0000.

```json
[
  {
    "Label": "1-4",
    "Distribution": {
      "Min": 0,
      "Max": 10,
      "Mean": 0.6700557781869355,
      "Median": 0,
      "P10": 0,
      "P25": 0,
      "P50": 0,
      "P75": 1,
      "P90": 2,
      "P95": 3,
      "P99": 4.333333333333333,
      "StandardDeviation": 1.0110714943289971
    }
  },
  {
    "Label": "5-9",
    "Distribution": {
      "Min": 0,
      "Max": 5.2,
      "Mean": 0.6271085910969733,
      "Median": 0.4,
      "P10": 0,
      "P25": 0.125,
      "P50": 0.4,
      "P75": 1,
      "P90": 1.6,
      "P95": 2,
      "P99": 2.8230000000000017,
      "StandardDeviation": 0.6791613101282341
    }
  },
  {
    "Label": "10-19",
    "Distribution": {
      "Min": 0,
      "Max": 4,
      "Mean": 0.6220528809864089,
      "Median": 0.4,
      "P10": 0,
      "P25": 0.11764705882352941,
      "P50": 0.4,
      "P75": 1,
      "P90": 1.5272445820433442,
      "P95": 1.8235294117647058,
      "P99": 2.426432748538012,
      "StandardDeviation": 0.6108618687523886
    }
  },
  {
    "Label": "20-49",
    "Distribution": {
      "Min": 0,
      "Max": 3.5428571428571427,
      "Mean": 0.6165097253209989,
      "Median": 0.4142136248948696,
      "P10": 0.047619047619047616,
      "P25": 0.1388888888888889,
      "P50": 0.4142136248948696,
      "P75": 0.9767441860465116,
      "P90": 1.4472487160674985,
      "P95": 1.7214113873295913,
      "P99": 2.2734482758620693,
      "StandardDeviation": 0.5759702076012285
    }
  },
  {
    "Label": "50-99",
    "Distribution": {
      "Min": 0,
      "Max": 2.5849056603773586,
      "Mean": 0.5628807791927346,
      "Median": 0.35690653432588915,
      "P10": 0.052576754385964916,
      "P25": 0.14008771929824562,
      "P50": 0.35690653432588915,
      "P75": 0.8866133269644136,
      "P90": 1.3837402885682593,
      "P95": 1.629545454545454,
      "P99": 2.096366959064328,
      "StandardDeviation": 0.5346911552007412
    }
  },
  {
    "Label": "100\u002B",
    "Distribution": {
      "Min": 0,
      "Max": 2.526315789473684,
      "Mean": 0.45814002830887196,
      "Median": 0.24426557870349003,
      "P10": 0.039580572230451395,
      "P25": 0.11509459303887333,
      "P50": 0.24426557870349003,
      "P75": 0.6687139137325937,
      "P90": 1.2816875319366379,
      "P95": 1.5044295975901754,
      "P99": 2.0178431372549044,
      "StandardDeviation": 0.49611421931778904
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
    "Role": "MID",
    "ChampionA": "Chogath",
    "ChampionB": "Tryndamere",
    "SampleSize": 1,
    "Side": "A",
    "Events": 9,
    "PerMatch": 9
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
    "ChampionA": "Camille",
    "ChampionB": "RekSai",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Diana",
    "ChampionB": "TwistedFate",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Elise",
    "ChampionB": "Yone",
    "SampleSize": 1,
    "Side": "A",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "Illaoi",
    "ChampionB": "Kaisa",
    "SampleSize": 1,
    "Side": "B",
    "Events": 8,
    "PerMatch": 8
  },
  {
    "Role": "TOP",
    "ChampionA": "JarvanIV",
    "ChampionB": "Kennen",
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
    "Role": "MID",
    "ChampionA": "AurelionSol",
    "ChampionB": "Viego",
    "SampleSize": 1,
    "Side": "A",
    "Events": 7,
    "PerMatch": 7
  },
  {
    "Role": "MID",
    "ChampionA": "Irelia",
    "ChampionB": "Jhin",
    "SampleSize": 1,
    "Side": "A",
    "Events": 7,
    "PerMatch": 7
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
    "ChampionA": "Garen",
    "ChampionB": "XinZhao",
    "SampleSize": 1,
    "Side": "A",
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
  }
]
```

## Metric Correlations

Pearson correlations describe association across canonical matchup aggregates; they do not establish causation.

```json
[
  {
    "MinimumSample": 20,
    "MatchupCount": 2659,
    "Pairs": {
      "FirstLevel2RateA vs FirstLevel3RateA": 0.6850360682405076,
      "FirstLevel2RateA vs FirstLevel6RateA": 0.267319671753318,
      "FirstLevel2RateA vs SoloKillDifference": 0.03766106953159492,
      "FirstLevel3RateA vs FirstLevel6RateA": 0.45256331199551686,
      "FirstLevel3RateA vs SoloKillDifference": 0.15339165768450072,
      "FirstLevel6RateA vs SoloKillDifference": 0.3732956233353914,
      "WinRateA vs FirstLevel2RateA": -0.012123749073742416,
      "WinRateA vs FirstLevel3RateA": 0.039956209263488036,
      "WinRateA vs FirstLevel6RateA": 0.08856894776550704,
      "WinRateA vs SoloKillDifference": 0.16504363962454113
    }
  },
  {
    "MinimumSample": 50,
    "MatchupCount": 1170,
    "Pairs": {
      "FirstLevel2RateA vs FirstLevel3RateA": 0.7066120288135384,
      "FirstLevel2RateA vs FirstLevel6RateA": 0.28738214999804995,
      "FirstLevel2RateA vs SoloKillDifference": -0.02087072902566705,
      "FirstLevel3RateA vs FirstLevel6RateA": 0.4751657626046361,
      "FirstLevel3RateA vs SoloKillDifference": 0.12944833719940543,
      "FirstLevel6RateA vs SoloKillDifference": 0.39255840470238507,
      "WinRateA vs FirstLevel2RateA": -0.02852000913055944,
      "WinRateA vs FirstLevel3RateA": 0.05287866680714079,
      "WinRateA vs FirstLevel6RateA": 0.08478347929996541,
      "WinRateA vs SoloKillDifference": 0.2484458502100373
    }
  }
]
```

Directional agreement below is restricted to matchups where A win rate is above 50%; neutral metric values are reported separately.

```json
[
  {
    "MinimumSample": 20,
    "WinRateAAbove50Count": 1242,
    "Metrics": [
      {
        "Metric": "FirstLevel2RateA",
        "SameDirection": 606,
        "OppositeDirection": 612,
        "Neutral": 24,
        "AgreementAmongNonNeutral": 0.4975369458128079
      },
      {
        "Metric": "FirstLevel3RateA",
        "SameDirection": 618,
        "OppositeDirection": 598,
        "Neutral": 26,
        "AgreementAmongNonNeutral": 0.5082236842105263
      },
      {
        "Metric": "FirstLevel6RateA",
        "SameDirection": 614,
        "OppositeDirection": 584,
        "Neutral": 44,
        "AgreementAmongNonNeutral": 0.5125208681135225
      },
      {
        "Metric": "SoloKillDifference",
        "SameDirection": 651,
        "OppositeDirection": 517,
        "Neutral": 74,
        "AgreementAmongNonNeutral": 0.5573630136986302
      }
    ]
  },
  {
    "MinimumSample": 50,
    "WinRateAAbove50Count": 563,
    "Metrics": [
      {
        "Metric": "FirstLevel2RateA",
        "SameDirection": 270,
        "OppositeDirection": 290,
        "Neutral": 3,
        "AgreementAmongNonNeutral": 0.48214285714285715
      },
      {
        "Metric": "FirstLevel3RateA",
        "SameDirection": 290,
        "OppositeDirection": 266,
        "Neutral": 7,
        "AgreementAmongNonNeutral": 0.5215827338129496
      },
      {
        "Metric": "FirstLevel6RateA",
        "SameDirection": 288,
        "OppositeDirection": 260,
        "Neutral": 15,
        "AgreementAmongNonNeutral": 0.5255474452554745
      },
      {
        "Metric": "SoloKillDifference",
        "SameDirection": 310,
        "OppositeDirection": 223,
        "Neutral": 30,
        "AgreementAmongNonNeutral": 0.5816135084427767
      }
    ]
  }
]
```

Canonical A weighted win rate: 49.72%. ChampionA is lexical, so deviation from 50% is monitored as a normalization sanity check, not assumed to be causal.

## Candidate Reliability Thresholds

Candidate only; not applied to the WPF application.

```json
[
  {
    "Label": "\uD45C\uBCF8 \uBD80\uC871",
    "Minimum": 1,
    "Maximum": 4,
    "Count": 9233,
    "Rate": 0.5938766321476813
  },
  {
    "Label": "\uCC38\uACE0\uC6A9",
    "Minimum": 5,
    "Maximum": 19,
    "Count": 3655,
    "Rate": 0.2350935871872387
  },
  {
    "Label": "\uBCF4\uD1B5",
    "Minimum": 20,
    "Maximum": 49,
    "Count": 1489,
    "Rate": 0.09577410432880942
  },
  {
    "Label": "\uCDA9\uBD84",
    "Minimum": 50,
    "Maximum": null,
    "Count": 1170,
    "Rate": 0.07525567633627066
  }
]
```

The proposed exploratory bands are `<5`, `5-19`, `20-49`, and `50+`. They jointly expose coverage and Wilson-width tradeoffs without deleting rare/off-meta matchups. Stage 9-1 should use these as display reliability labels only, retaining raw statistics and a clear low-sample warning.

This dataset is MASTER_PLUS_SEEDED: seeds were Master/Grandmaster/Challenger at collection time; all ten participants were not tier-verified. Results are patch-specific and are not merged with other patches.
