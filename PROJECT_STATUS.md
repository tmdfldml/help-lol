# LoLMatchup Project Status

## 프로젝트

- 프로젝트: `LoLMatchup`
- 기술: C# / .NET 10 / WPF
- 플랫폼: Windows 전용
- 목표: League of Legends 게임 중 현재 Role matchup을 식별하고, Riot 공식 API로 자체 수집한 통계를 이용해 matchup 정보를 제공한다.

## Stage 진행 상황

- Stage 1~4: WPF 기본 UI, Live Client Data API, Role matchup 생성, `TEST_ONLY` matchup 데이터 구현
- Stage 5: Riot 공식 API 기반 `StatsCollector` 엔진 구현 완료
- Stage 6: 시작 전

## Stage 5 StatsCollector

`Tools/StatsCollector/`는 WPF 앱과 독립된 .NET 10 Console 프로젝트다. 현재 수집 파이프라인은 다음과 같다.

```text
MASTER+ seed 수집
  → Match-V5 Match ID discovery 및 pagination
  → persistent discovery/terminal dedupe
  → Match Detail 조회
  → queue / patch / participant / Role validation
  → TOP / JUNGLE / MID / ADC / SUPPORT matchup 생성
  → Timeline 조회
  → 승패 / 선 2·3·6레벨 / 15분 이전 맞상대 솔로킬 추출
  → MatchupObservation
  → append-only JSONL 상태 저장
  → observation replay 및 MatchupAccumulator 복구
  → DatasetWriter aggregate 출력
```

Detail만으로 판별할 수 있는 validation 실패는 Timeline 요청 전에 제외한다. 한 Match의 observation 5개가 모두 생성된 경우에만 terminal Accepted 결과와 aggregate에 반영한다.

## 통계 Dataset의 의미

Primary dataset scope는 `MASTER_PLUS_SEEDED`다. Master, Grandmaster, Challenger seed player의 Match history에서 발견한 경기라는 뜻이며 다음 metadata로 의미를 명시한다.

```ini
tierScopeMethod = SEED_PLAYER_TIER_AT_COLLECTION
allParticipantsTierVerified = false
```

따라서 참가자 10명 전원이 Master 이상으로 검증된 dataset이라는 의미가 아니다. 향후 보조 scope로 `EMERALD_PLUS_SEEDED`를 추가할 계획이지만 아직 구현하지 않았다.

## 현재 통계 지표

각 Role matchup에서 다음 다섯 지표만 수집한다.

1. 게임 승리
2. 선 2레벨
3. 선 3레벨
4. 선 6레벨
5. 15분 이전 직접 맞상대 솔로킬

CS, gold, XP 차이, KDA, damage 등은 현재 수집하지 않는다.

## Aggregate raw count

배포 dataset은 계산된 비율보다 다음 raw count를 보존한다.

```text
SampleSize
WinsForA

FirstLevel2ForA
FirstLevel2ForB
FirstLevel2Unknown

FirstLevel3ForA
FirstLevel3ForB
FirstLevel3Unknown

FirstLevel6ForA
FirstLevel6ForB
FirstLevel6Unknown

SoloKillsBefore15ForA
SoloKillsBefore15ForB
```

솔로킬 값은 경기 승리 여부가 아니라 event 횟수이므로 `SampleSize`보다 클 수 있다. 유리/불리 `+2/+1/0/-1/-2`, metric 가중치, minimum sample 및 confidence 기준은 아직 결정하지 않았다.

## Champion 및 Role 규칙

- 내부 champion identifier는 Riot Match-V5의 `championName` 기반 영문 ID를 사용한다. 예: `LeeSin`, `Viego`, `JarvanIV`, `Kaisa`.
- 표준 Role은 `TOP`, `JUNGLE`, `MID`, `ADC`, `SUPPORT`다.
- Match-V5 `teamPosition`의 `MIDDLE`, `BOTTOM`, `UTILITY`는 각각 `MID`, `ADC`, `SUPPORT`로 정규화한다.
- Riot `teamPosition`이 유효하면 비주류 pick도 유지한다.
- champion meta Role dictionary를 이용한 추정, 재배치 또는 필터링은 하지 않는다.

## Champion pair normalization

동일 matchup은 `Role + ChampionA + ChampionB` key로 한 번만 저장한다. ChampionA/B는 대소문자를 무시한 안정적인 영문 순서로 정규화한다. 정렬 과정에서 A/B가 바뀌면 participant, 승패, 레벨 및 솔로킬 방향도 함께 바뀐다.

## Timeline metric 정의

선 2/3/6레벨은 Timeline의 다음 `LEVEL_UP` event를 사용한다.

```text
participantId
level
timestamp
```

두 participant의 해당 레벨 최초 timestamp를 비교한다. 동시 도달이나 event 누락은 aggregate에서 `Unknown`이다. 진단에서는 현재 session의 Unknown을 `Tie`, `MissingA`, `MissingB`, `MissingBoth`로 구분하지만, 이 상세 원인은 JSONL schema에 저장하지 않는다. `participantFrames` fallback은 사용하지 않는다.

15분 이전 맞상대 솔로킬 정의는 다음과 같다.

```text
timestamp < 900000
killer/victim이 같은 Role의 직접 맞상대
assistingParticipantIds에 참가자가 없음
```

실제 표본에서는 다음 패턴이 반복 확인됐다.

- `assistingParticipantIds.Count > 0`: assist 있음
- `null` 또는 property missing: assist 없음
- 빈 배열: 아직 관찰되지 않음

## Persistent State

State는 dataset identity별로 다음 경로에 저장한다.

```text
Tools/StatsCollector/Data/State/
  MASTER_PLUS_SEEDED/
    KR/
      420/
        {patch}/
          discovered-matches.jsonl
          match-results.jsonl
          checkpoint.json
```

- `match-results.jsonl`이 처리 결과의 source of truth다.
- Accepted terminal record의 observation 5개를 replay하여 `MatchupAccumulator`를 복구한다.
- `checkpoint.json`은 JSONL에서 재생성할 수 있는 derived summary다.
- Match Detail 및 Timeline 원본 JSON, PUUID, Riot ID, summonerId는 영구 저장하지 않는다.
- `Tools/StatsCollector/Data/State/`는 Git ignore 대상이다.
- 마지막 불완전 JSONL append는 복구할 수 있으며, 중간 손상은 state corruption으로 보고한다.

## API 요청 및 Retry

`RiotApiClient`는 Platform/Regional `HttpClient`를 재사용하는 single-worker 순차 구조다.

```text
429             → Retry-After 우선, 제한 횟수 소진 시 session 중단
5xx             → bounded exponential retry
network/timeout → bounded retry
401/403         → retry 없이 session 중단
기타 4xx        → 자동 retry 없음
```

Transient API 실패는 terminal result로 기록하지 않으므로 다음 실행에서 다시 처리할 수 있다. 실제 검증 중 429 발생 후 `Retry-After` 대기와 정상 복구를 확인했다. API request category와 Riot rate-limit header는 개인정보나 raw URI 없이 진단한다.

## Output Dataset

`DatasetWriter`는 replay된 aggregate를 다음 경로에 derived artifact로 생성한다.

```text
Tools/StatsCollector/Data/Output/
  MASTER_PLUS_SEEDED/
    KR/
      420/
        {patch}/
          matchups.json
```

출력은 stable ordering을 사용하며 `matchups.json.tmp`에 완전히 기록한 뒤 교체한다. aggregate 불변식이나 `Observations == AcceptedMatches × 5`가 깨지면 정상 dataset으로 출력하지 않는다.

WPF 프로젝트의 `Data/matchups.json`은 Stage 4의 `TEST_ONLY` 샘플이다. StatsCollector의 실제 aggregate dataset과 혼동하거나 자동으로 덮어쓰면 안 된다.

## 개발 검증 Snapshot

Stage 5-17 실제 검증 시점의 development snapshot은 다음과 같다.

```text
Accepted matches: 101
Observations: 505
Aggregated matchup keys: 381

First level 2 unknown: 약 0.59%
First level 3 unknown: 약 0.20%
First level 6 unknown: 0%

Observations count valid: True
Aggregate invariants valid: True
Role observation counts valid: True
Duplicate terminal records: 0
```

이는 수집 파이프라인 검증용 snapshot이며 production 통계 표본이 확정됐다는 의미가 아니다.

## Stage 5-18 완료 기능

- paginated Match ID discovery
- persistent 목표 설정 `TARGET_ACCEPTED_MATCHES`
- session 처리 제한 `ProcessingMatchLimit`
- 목표 도달 시 API 처리 없이 state replay 및 dataset 재생성
- Unknown 원인 진단
- API request category 및 rate-limit header 진단
- validation으로 절약된 Timeline 요청 진단
- champion-role appearance 진단
- matchup sample-size 분포 진단

전체 목표와 session 제한의 의미는 다르다.

```text
TARGET_ACCEPTED_MATCHES = persistent dataset 전체 목표
ProcessingMatchLimit   = 한 번의 실행에서 시도할 최대 Match 수
```

Stage 5의 StatsCollector 엔진 구현은 완료된 것으로 본다.

## 다음 Stage

다음 작업은 Stage 6이다. StatsCollector를 더 확장하기보다 WPF 앱이 실제 aggregate `matchups.json`을 읽을 수 있도록 연결한다.

첫 작업은 다음과 같다.

```text
Stage 6-1
WPF StatisticsDataService 및 Dataset DTO
```

WPF runtime은 `Tools/StatsCollector` 경로에 의존하면 안 된다. 목표 구조는 다음과 같다.

```text
StatsCollector
  → aggregate dataset
  → WPF statistics data/cache
  → StatisticsDataService
  → UI
```

Stage 6에서는 StatsCollector output을 WPF 배포 구조로 전달하는 명확한 방법을 먼저 정하고, 기존 Stage 4 `TEST_ONLY` 데이터와 실제 통계를 구분해야 한다.
