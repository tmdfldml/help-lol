# LoLMatchup Project Status

## 프로젝트

- 프로젝트: `LoLMatchup`
- 기술: C# / .NET 10 / WPF
- 플랫폼: Windows 전용
- 목표: League of Legends 게임 중 현재 Role matchup을 식별하고, Riot 공식 API로 자체 수집한 통계를 이용해 matchup 정보를 제공한다.

## Stage 진행 상황

- Stage 1~4: WPF 기본 UI, Live Client Data API, Role matchup 생성, `TEST_ONLY` matchup 데이터 구현
- Stage 5: Riot 공식 API 기반 `StatsCollector` 엔진 구현 완료
- Stage 6: production statistics 표시 파이프라인 구현 완료
- Stage 7-1: local champion display metadata와 한국어 이름/아이콘 표시 구현 완료
- Stage 7-2: WPF 통계 화면 layout 및 visual polish 완료

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

WPF runtime은 `Data/Statistics/master_plus.json`의 packaged production aggregate만 사용한다. Stage 4 `TEST_ONLY` score 파일과 loader는 Stage 6-5에서 제거했다.

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

## Stage 5-19 discovery 확장

- Match ID discovery는 seed당 깊은 pagination보다 더 많은 MASTER+ seed를 순환하는 breadth-first 방식을 사용한다.
- 기본 discovery window는 정렬된 seed pool에서 최대 125명을 선택하고, seed당 20개 Match ID를 한 page만 조회한다.
- `discovery-state.json`에는 dataset identity별 다음 seed cursor만 저장하며 PUUID나 플레이어 식별자는 기록하지 않는다.
- 기존 pending Match를 먼저 처리하고 pending이 없을 때만 다음 discovery window를 실행한다.
- 한 window에서 새 Match가 0개여도 종료하지 않으며, 전체 seed pool을 새 발견 없이 한 바퀴 성공적으로 확인했을 때만 `NoNewMatches`로 판단한다.
- session validation 비율은 이번 실행의 terminal 결과만 분모로 사용하고, 누적 persistent validation 통계와 별도로 출력한다.
- `match-results.jsonl`과 기존 aggregate replay 구조는 변경하지 않으며 discovery cursor는 운영 상태일 뿐 통계 source of truth가 아니다.

## Stage 5-20 duration quality gate

- production 통계는 기본적으로 `gameDuration >= 900`인 경기만 Accepted 처리한다.
- 최소 길이는 `MIN_GAME_DURATION_SECONDS`로 설정할 수 있고 기본 policy identity는 `MIN900S`다.
- 새 state/output은 `{patch}/MIN900S/`처럼 policy별 경로에 저장한다.
- 최소 길이 정책 이전의 legacy `{patch}/` state와 output은 수정하거나 migration하지 않고 개발 검증 snapshot으로 보존한다.
- `InsufficientGameDuration`은 deterministic terminal reject이며 Timeline 요청 전에 판정한다.

## Stage 6 production pipeline

WPF runtime은 `Tools/StatsCollector` 경로에 의존하지 않는다. 현재 구조는 다음과 같다.

```text
StatsCollector
  → aggregate dataset
  → WPF statistics data/cache
  → StatisticsDataService
  → UI
```

현재 production dataset 기준은 `MASTER_PLUS_SEEDED`와 `MIN900S`다. 3,500 accepted matches는 현재 배포 파일의 검증 snapshot일 뿐 코드나 정책에 고정된 값이 아니다.

## Stage 6-1~6-2

- `StatisticsDataService`가 WPF 배포 데이터의 schema, metadata, aggregate 불변식을 검증하고 정규화된 matchup index를 제공한다.
- `MatchupStatisticsService`와 `MatchupStatisticsCalculator`는 파일 I/O와 분리되어 raw aggregate를 query champion 관점으로 변환한다.
- 승률 및 선 2·3·6레벨 비율의 분모는 `SampleSize`이며 presentation rounding은 하지 않는다.
- 15분 이전 맞상대 솔로킬은 확률이 아니라 경기당 평균 event 수로 제공한다.
- dataset unavailable/invalid, matchup not found, invalid query 및 0 이하 sample을 명시적인 실패 상태로 처리한다.

## Stage 6-3 presentation

- `MatchupStatisticsPresenter`가 계산 결과를 percentage 1자리와 경기당 event 2자리의 표시 문자열로 변환한다.
- WPF 통계 패널은 Role, champion pair, 표본, dataset metadata와 5개 지표를 query/opponent 비교 형태로 표시한다.
- first-level Unknown은 숨기지 않으며 솔로킬은 확률이 아닌 경기당 평균과 총 event 수로 표시한다.
- DEBUG에서만 실제 dataset의 첫 유효 matchup을 화면에 연결하고 Release 기본 화면은 기존 Waiting 상태를 유지한다.

## Stage 6-4 live integration

- `LiveStatisticsCoordinator`가 기존 active-player, team 분리 및 Role matchup 결과를 현재 플레이어 관점의 statistics query로 연결한다.
- `LiveStatisticsPollingService`는 약 2초 간격으로 순차 polling하며 동일한 Role/champion signature의 lookup과 UI 갱신을 반복하지 않는다.
- 게임 시작 시 실제 matchup 통계로 전환하고 Live Client가 종료되면 Waiting 상태로 복귀한다.
- Window 종료 시 cancellation으로 polling을 끝내며 DEBUG sample은 검증만 수행하고 화면에는 표시하지 않는다.

## Stage 6-5 production cleanup

- Stage 4의 `Data/matchups.json`, `MatchupDataService`, `MatchupScoreFormatter`와 TEST_ONLY score runtime 경로를 제거했다.
- production dataset만 build/publish output에 포함하며 legacy backup은 runtime 복사에서 제외한다.
- 연속된 동일 Waiting reason은 한 번만 기록하고 matchup detection 및 statistics status는 signature 변경 시에만 기록한다.
- Stage 6-1 Dataset Loader, 6-2 query-oriented calculation, 6-3 presentation, 6-4 automatic Live Client integration, 6-5 production cleanup을 완료했다.

## Stage 7-1 champion display metadata

- 통계 조회와 Live Client signature는 기존 Riot/Data Dragon 호환 영문 `ChampionId`를 계속 사용한다.
- WPF presentation에서만 packaged `Data/Champions/champions.ko.json`을 조회해 한국어 챔피언 이름과 local icon을 붙인다.
- 아이콘은 `Data/Champions/Icons/`에서 `AppContext.BaseDirectory` 기준으로 읽으며, `OnLoad` 및 `Freeze` 후 memory cache에서 재사용한다.
- metadata는 앱 실행 중 한 번 load하여 case-insensitive dictionary로 조회한다.
- metadata 누락 시 영문 `ChampionId`, icon 누락 시 neutral placeholder로 안전하게 fallback하며 statistics 상태와 구분한다.
- build와 publish 결과에는 production statistics dataset, champion metadata 및 icon resource가 함께 포함된다.

## Stage 7-2 UI layout 및 visual polish

- 기존 dark theme과 440×700 기본 창 크기를 유지하면서 matchup header, dataset 정보, metric card의 정보 계층과 간격을 정리했다.
- Role과 60×60 champion icon, 한국어 표시명을 하나의 header card로 구성하고 query/opponent 열은 모든 지표에서 동일한 Grid column을 사용한다.
- 표본 수와 dataset metadata는 header 하단의 secondary information으로 유지한다.
- 승률, 선 2·3·6레벨, 15분 이전 솔로킬은 공통 card style과 정렬된 좌우 값으로 표시한다. Unknown과 경기당 솔로킬 의미는 기존 presentation 그대로 보존한다.
- Waiting, matchup 없음, dataset 없음/오류 상태에 서로 다른 보조 안내를 제공하며 Live Client 및 statistics pipeline은 변경하지 않았다.

### Stage 7-2.1 UI refinement

- 전체 metric이 기본 화면에 들어오도록 기본 창을 440×820, 최소 창을 420×760으로 조정하고 `ScrollViewer`는 작은 화면과 DPI scaling을 위한 안전장치로 유지한다.
- matchup header의 한국어 챔피언 이름은 metric header와 동일한 query/opponent accent brush를 명시적으로 사용한다.
- 사용자용 dataset metadata는 내부 tier scope와 policy key를 숨기고, 로드된 dataset의 patch를 이용한 `패치 {version}`만 표시한다.

### Stage 7-2.2 adaptive portrait / landscape UI

- 동일한 `MatchupStatisticsPresentation`을 공유하는 `PortraitStatisticsView`와 `LandscapeStatisticsView`를 분리했다.
- portrait는 기존 세로형 정보 흐름을 유지하면서 card 간격과 padding을 줄여 기본 440×820에서 scrollbar가 나타나지 않도록 구성했다.
- landscape는 matchup card와 2-column metric grid를 사용하며 솔로킬 card는 마지막 행 전체 폭을 사용한다.
- client layout 비율이 1.15 이상이면 landscape로, landscape 상태에서 1.05 이하이면 portrait로 복귀하는 hysteresis를 적용한다.
- layout 전환은 view visibility만 변경하며 DataContext, polling, statistics lookup을 다시 만들거나 실행하지 않는다.

### Stage 7-2.3 full team matchup dashboard

- active player의 team을 기준으로 왼쪽은 아군, 오른쪽은 적군으로 유지하며 TOP/JUNGLE/MID/ADC/SUPPORT 통계를 동시에 조회한다.
- 5개 Role을 고정 순서로 포함한 team-wide signature를 사용해 동일 조합의 polling 결과를 재조회하지 않는다.
- Role별 matchup 없음은 해당 card의 `통계 없음`, 부분 custom이나 준비 중인 Role은 `감지 중...`으로 표시하고 다른 Role 결과는 유지한다.
- portrait는 5개 compact Role card의 세로 dashboard, landscape는 전체 가로 공간을 사용하는 3-column dashboard로 구성한다.
- 두 adaptive view는 동일한 `TeamMatchupStatisticsPresentation`을 공유하며 resize는 statistics lookup을 발생시키지 않는다.

### Stage 7-2.4 portrait compact team dashboard

- portrait의 Role별 5개 세로 metric row를 `승률 / L2 / L3 / L6 / 솔킬` 2-row comparison table로 압축했다.
- 각 compact Role card는 30×30 icon, 한국어 이름, Role, 개별 표본 수와 아군/적군 값을 유지한다.
- 솔로킬은 `경기당 평균 (총 횟수)`로 표시하고, level Unknown은 0보다 큰 항목만 card 하단에 요약한다.
- landscape의 상세 dashboard와 공통 metric 표현은 변경하지 않았다.
