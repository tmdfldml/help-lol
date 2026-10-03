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
- Stage 8: application shell, navigation 및 matchup detail 구현 완료
- Stage 9-5: statistics update pipeline의 publish/update/install 및 public hosting 연결 완료

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

## Developer Test Mode

- Debug 빌드에서만 in-app synthetic Live Client simulation 진입 버튼과 개발자 제어창을 제공한다.
- 정상 5v5, TOP 1v1, 일부 Role 누락, 실제 dataset NotFound 및 custom 5-Role champion 선택을 지원한다.
- synthetic player list는 실제 `LiveClientPlayer`, `ChampionIdParser`, `TeamComposition`, `RoleMatchup` 이후 production statistics pipeline을 그대로 사용하며 가짜 통계를 만들지 않는다.
- champion 선택 목록은 packaged champion metadata를 한국어 이름순으로 재사용하고 `없음`을 선택할 수 있다.
- 테스트 활성 상태는 저장하지 않으며 종료 시 실제 Live Client polling으로 복귀한다. Release UI에는 테스트 진입점과 TEST MODE 표시가 노출되지 않는다.
- 개발자 창의 Button, TextBox, popup 및 list item은 명시적인 dark-theme enabled/hover/selected/disabled 색상을 사용한다.
- 재사용 가능한 searchable champion picker가 한국어 이름 substring과 영문 ChampionId case-insensitive 검색을 지원하며, 각 picker는 독립된 filtered collection을 유지한다.

## Stage 7-3 Window Usability & Persistence

- `%LocalAppData%/LoLMatchInfo/settings.json`에 Always on Top, 창 위치/크기, 마지막 layout mode와 Portrait/Landscape별 최근 크기를 저장한다.
- 기본값은 Always on Top 비활성, 위치 및 크기 기억 활성이다. 설정 파일이 없거나 손상되면 안전하게 기본값을 사용한다.
- MainWindow의 PIN toggle은 `Topmost`를 즉시 적용하고 저장하며, 정상 종료 시 `RestoreBounds` 기반의 normal bounds를 저장한다.
- 저장 위치가 현재 virtual desktop과 충분히 겹치지 않으면 primary work area 중앙으로 복구하고 비정상 크기는 현재 desktop 범위로 제한한다.
- orientation profile은 기록만 하며 Portrait/Landscape 전환 시 저장 크기로 창을 강제 변경하지 않는다.
- 설정 저장은 temporary file을 완전히 쓴 뒤 교체하며 Stage 7-4 Settings Window에서 재사용할 수 있는 `AppSettingsService` 기반으로 분리했다.

## Stage 7-4 In-App Settings System

- dark theme의 `SettingsWindow`에 일반, 화면, 데이터, 정보 섹션을 제공한다.
- 설정 변경은 공유 `AppSettingsService`에 즉시 반영하고 저장하며, change event를 통해 MainWindow PIN과 Always on Top 설정을 양방향 동기화한다.
- 창 위치/크기 기억 정책과 Portrait/Landscape 최근 크기를 앱 내부에서 확인하고 제어할 수 있다.
- 설정 초기화는 `AppSettings`의 공식 기본값을 재사용하며 현재 MainWindow bounds를 즉시 강제 변경하지 않는다.
- 데이터 페이지는 실제 WPF runtime dataset의 상태, patch, accepted match 수와 해석 가능한 scope/duration만 사용자 친화적으로 표시한다.
- 정보 페이지의 앱 버전은 assembly metadata에서 읽고 Riot API key나 StatsCollector secret은 설정 UI에 노출하지 않는다.

## Stage 7-5 Statistics Dataset Update & Publishing System

- Settings 데이터 페이지의 경기 수는 Collector 진행량이 아니라 현재 WPF가 실제로 적용한 dataset의 `AcceptedMatches`임을 명확히 표시한다.
- `PublishStatisticsDataset` 개발자 도구가 production Collector output을 공통 validation으로 검증한 뒤 WPF packaged baseline과 manifest로 명시적으로 publish한다.
- WPF와 publish 도구는 동일한 schema, metadata, aggregate count 및 L2/L3/L6 불변식 검증을 사용한다. Solo Kill은 event count이므로 sample size보다 클 수 있다.
- runtime dataset은 `%LocalAppData%/LoLMatchInfo/Data/Statistics/master_plus.json`의 유효한 업데이트 파일을 우선하고, 없거나 손상되면 packaged `Data/Statistics/master_plus.json`으로 fallback한다.
- remote manifest는 Riot patch와 별도의 단조 증가 `datasetRevision`, dataset identity, file size, SHA-256 및 download URL을 표현한다.
- update service는 정상 TLS의 전용 `HttpClient`로 manifest 확인, 다운로드, SHA-256 및 dataset validation, LocalAppData atomic installation을 담당하며 실패 시 기존 dataset을 유지한다.
- 원격 manifest URL은 한 configuration 객체에서 관리하며, 실제 hosting 주소가 확정되기 전에는 미설정 상태로 안전하게 동작한다.
- Settings에서 현재 적용 dataset의 상태, patch, accepted matches, scope, duration, source와 revision을 확인하고 수동 업데이트 확인/적용을 실행할 수 있다.

## Stage 8

### Stage 8-1 App Shell & Navigation Foundation

- `MainWindow`를 window lifecycle, global header, overlay navigation 및 page hosting을 담당하는 application shell로 전환했다.
- `HomeView`는 polling의 실제 game state와 공유 `StatisticsDataService`가 로드한 patch 및 applied match count를 표시한다.
- `InGameView`가 기존 Waiting 상태와 `PortraitStatisticsView` / `LandscapeStatisticsView` hosting을 담당하며 Stage 7 dashboard 디자인은 유지한다.
- 작은 `AppNavigationService`가 `Home`과 `InGame` current page 및 change event만 관리하며 외부 navigation/MVVM framework는 사용하지 않는다.
- navigation drawer는 content 위 overlay로 열려 닫힌 상태에서 dashboard usable width를 줄이지 않으며 홈, 인게임, 기존 SettingsWindow 진입을 제공한다.
- polling과 statistics lookup은 앱 전체에서 기존처럼 한 번만 실행하고 Home/InGame view instance와 마지막 presentation을 MainWindow lifetime 동안 재사용한다.
- 새 `no game → active game` 전이에서 한 번만 InGame으로 자동 이동하고, active game 중 사용자가 Home으로 이동한 선택은 이후 polling에서 존중한다.
- 기존 orientation threshold/hysteresis, Developer Test Mode, PIN/Topmost, window persistence, Settings와 dataset update 기능을 유지한다.

### Stage 8-2 App Shell & In-Game UX Polish

- global header를 compact dark controls로 정리하고 공유 game-active 상태에 따라 `WAITING`, `LIVE`, `TEST MODE`를 일관되게 표시한다.
- Shell 전용 button/toggle/navigation style을 분리해 normal, hover, pressed, selected 및 disabled 상태가 WPF 기본 흰색 UI에 의존하지 않게 했다.
- overlay navigation drawer는 현재 page 이름과 accent selected state를 표시하며 기존 backdrop click 및 ESC close 동작을 유지한다.
- Home은 중복 product title 대신 간결한 page identity, game status card, runtime patch/applied match count card와 InGame quick action을 제공한다.
- InGame은 기존 statistics 상단 여백을 compact page context로 치환하고 Waiting 문구를 page 역할에 맞게 다듬었다.
- Portrait compact dashboard와 Landscape detailed dashboard의 정보 구조 및 orientation threshold/hysteresis는 변경하지 않았다.
- InGameView가 dashboard hosting 경계를 유지해 다음 Matchup Detail navigation을 MainWindow에 다시 결합하지 않고 확장할 수 있다.

### Stage 8-3 Matchup Detail Navigation

- `AppPage.MatchupDetail`과 선택된 Role/champion IDs, patch 및 기존 Role presentation을 담는 재사용 detail navigation context를 추가했다.
- Portrait와 Landscape의 Role card 전체가 동일한 command 경로로 mouse 및 keyboard 선택을 지원하며, champion이 누락된 Role은 비활성 상태를 유지한다.
- `InGameView`가 두 dashboard의 Role selection을 하나의 event로 전달하고 MainWindow는 추가 dataset lookup 없이 기존 presentation으로 detail context를 구성한다.
- `MatchupDetailView`는 아군/적군 champion icon과 이름, 표본, 승률, L2/L3/L6 및 15분 이전 솔로킬의 경기당 평균과 총 event 수를 표시한다.
- NotFound matchup은 champion context를 유지한 채 데이터 없음 상태를 표시하고 가짜 0% metric을 만들지 않는다.
- Detail back 또는 drawer의 InGame은 기존 InGameView instance로 복귀하며 Detail에서 ESC도 같은 경로를 사용한다.
- 게임 종료 또는 Developer Test Stop 시 stale detail context를 제거하고 InGame Waiting으로 자동 복귀한다.
- active presentation이 바뀌면 같은 Role/champion context만 갱신하고, 다른 game/champion이면 Detail을 재사용하지 않고 InGame으로 복귀한다.

## Stage 9 Statistics Reliability Research

- 현재 production dataset을 대상으로 sample-size 및 Role별 coverage 분포를 분석하는 독립 .NET 10 Console 도구 `Tools/AnalyzeStatisticsDataset/`를 추가했다.
- win-rate 분포와 극단값 빈도, Wilson 95% 신뢰구간, 레벨 Unknown/data-quality, 솔로킬 event-per-match 분포를 분석한다.
- `sampleSize >= 20`과 `sampleSize >= 50`에서 metric 간 Pearson correlation 및 방향 일치도를 연구용 지표로 계산하며 causation으로 해석하지 않는다.
- 공통 `StatisticsDatasetValidator`를 재사용해 duplicate/reversed canonical key, malformed aggregate 및 count 불변식을 분석 전에 검증한다.
- 분석 결과는 `Tools/AnalyzeStatisticsDataset/Output/statistics-analysis.json`과 `statistics-analysis.md`에 결정적으로 생성된다.
- 후보 reliability 구간과 coverage tradeoff를 제안했지만, production WPF reliability label이나 유리/불리 판정에는 아직 적용하지 않았다.

### Stage 9-1 Sample Reliability UI

Stage 9 production 분석을 근거로 모든 Role에 동일한 표본 상태 policy를 적용한다.

```text
1-4   = 표본 부족
5-19  = 참고용
20-49 = 보통
50+   = 상대적으로 충분
```

- WPF와 무관한 `SampleReliabilityClassifier` 한 곳에서 5/20/50 경계를 관리한다.
- matchup presentation에 reliability level, 사용자 표시문 및 설명을 포함한다.
- Portrait compact card와 Landscape card는 기존 표본 행에 중립적인 badge를 표시하며 champion side 색상과 분리한다.
- Matchup Detail은 badge와 표본 상태 설명을 함께 표시한다.
- 모든 표본에서 raw 통계를 그대로 유지하며 win rate 보정, metric 숨김, Wilson 직접 표시는 하지 않는다.
- NotFound, Missing Role, dataset unavailable/invalid는 표본 상태와 구분하며 reliability를 표시하지 않는다.

### Stage 9-2A Outcome Advantage Model Research

- 최종 게임 승률만 사용하는 raw win-rate threshold Model A, Wilson interval Model B, practical-effect + Wilson Model C를 전체 production aggregate에 적용했다.
- Stage 9-1 reliability band별 raw effect threshold를 달리하는 Conservative/Balanced/Permissive 연구 후보도 비교했다.
- `SideAAdvantage`, `Neutral`, `SideBAdvantage`, `InsufficientEvidence`를 분리하고 key/sample 가중 coverage, Role/reliability breakdown, 작은 표본 위험 및 canonical A/B 대칭성을 검증했다.
- L2/L3/L6/Solo Kill은 Outcome 판정 입력에서 제외하고 판정 후 방향 일치 진단에만 사용했다.
- Stage 9-2B 후보로 `sampleSize >= 20`, `|winRate - 50%| >= 5pp`, `80% Wilson interval excludes 50%`인 대칭 규칙을 추천한다.
- 연구 결과는 `Tools/AnalyzeStatisticsDataset/Output/advantage-model-analysis.json`과 `advantage-model-analysis.md`에 저장한다.
- Production advantage classifier, UI, 색상 및 유리/불리 문구는 아직 구현하지 않았다.

### Stage 9-2B Production Outcome Advantage

Stage 9-2A에서 실제 production dataset으로 선택한 다음 단일 정책을 runtime에 적용한다.

```text
sampleSize >= 20
AND |observed win rate - 50%| >= 5 percentage points
AND 80% Wilson interval excludes 50%
```

- `OutcomeAdvantageClassifier`가 raw query-side wins/sample만 입력받아 `아군 우세 경향`, `뚜렷한 차이 없음`, `적군 우세 경향`, `판정 보류`를 구분한다.
- 80% two-sided Wilson score interval과 production threshold는 한 domain policy에 중앙화되어 있으며 L2/L3/L6/솔로킬은 Outcome 입력으로 사용하지 않는다.
- query-oriented 통계를 사용하므로 canonical ChampionA/B 순서를 UI가 재해석하지 않으며 reverse query에서는 아군/적군 방향만 정확히 반전된다.
- 표본 reliability와 Outcome은 별도 enum/property로 유지하고 기존 raw metric 계산과 표시를 변경하지 않는다.
- Portrait는 기존 header 행의 compact indicator를 사용해 440×820의 Role card 높이를 늘리지 않으며, Landscape와 Matchup Detail에도 동일 presentation 결과와 설명을 표시한다.
- NotFound, Missing Role, dataset unavailable/invalid에는 Outcome을 생성하거나 표시하지 않는다.
- Outcome은 dataset schema나 cache에 저장하지 않고 현재 dataset의 raw aggregate로 presentation 생성 시 계산한다.
- production classifier를 전체 Stage 9-2A dataset에 적용한 결과는 A/아군 196, Neutral 1,374, B/적군 215, Insufficient 11,236으로 연구 권장 모델과 일치했다.
- Stage 9-2A의 분석 산출물은 developer research artifact로 유지하며 WPF runtime에서 읽거나 배포하지 않는다.

### Stage 9-2C-A Lane Pressure Model Research

- production aggregate에서 L2/L3/L6의 A-B rate difference와 15분 이전 맞상대 solo-kill event-per-match difference 분포를 분석했다.
- sample-size bucket과 Role별 volatility, level progression pattern, metric direction agreement 및 threshold sensitivity를 비교했다.
- simple four-signal majority, level-only progression majority, L2/L3를 결합한 de-correlated model, Level Tempo와 Combat Pressure를 분리한 model을 전체 parameter sweep으로 검증했다.
- 모든 후보에서 A/B reverse symmetry, key/sample-weighted coverage, Role/reliability breakdown 및 directional sample/effect 분포를 확인했다.
- 최종 게임 승률과 `OutcomeAdvantage`는 입력에서 제외하고 결과 방향 비교에만 사용했다.
- 연구 결과는 하나의 Lane Pressure label보다 `Level Tempo`와 `Combat Pressure`를 분리하는 방식을 권장한다.
- production 후보는 두 축 모두 minimum sample 20을 사용한다. Level Tempo는 L2/L3/L6 각각 5pp threshold에서 같은 방향 신호가 최소 2개일 때 방향을 내고, Combat Pressure는 solo-kill event 차이가 경기당 0.10 이상일 때 방향을 낸다. 나머지는 Neutral이며 n<20은 InsufficientEvidence다.
- 추천 사용자 용어는 `레벨 템포`, `교전 압박`, `경향`, `뚜렷한 차이 없음`, `판정 보류`다. 모든 Role을 포괄할 때는 `라인전 우세`보다 `초반 템포`가 안전하다.
- `Tools/AnalyzeStatisticsDataset/Output/lane-pressure-model-analysis.json`과 `lane-pressure-model-analysis.md`를 결정적으로 생성하며 두 번 실행한 SHA-256 일치를 확인했다.
- No production Lane Pressure classifier/UI implemented yet.

### Stage 9-2C-B Production Level Tempo + Combat Pressure

Stage 9-2C-A에서 선택한 두 개의 독립적인 early-game 지표를 production runtime에 적용한다.

```text
Common:
minimum sample = 20

Level Tempo:
L2/L3/L6 threshold = 5 percentage points
at least two same-side signals = side tendency
otherwise Neutral

Combat Pressure:
solo-kill event difference per match threshold = 0.10
otherwise Neutral
```

- `LevelTempoClassifier`는 L2/L3/L6의 `(AllyCount - EnemyCount) / SampleSize`를 각각 평가하며 Unknown을 분모에서 제거하지 않는다.
- `CombatPressureClassifier`는 15분 이전 직접 맞상대 솔로킬 event count의 경기당 차이를 평가한다. 솔로킬은 probability가 아니며 한 경기에서 여러 event가 발생할 수 있다.
- 두 classifier 모두 raw count만 사용하고 query 방향의 `AllyTendency`, `Neutral`, `EnemyTendency`, `InsufficientEvidence`를 반환한다.
- Outcome, Reliability, Level Tempo, Combat Pressure는 서로 별도 개념이다. 이들을 합친 Lane Pressure 점수는 사용하지 않는다.
- Portrait와 Landscape Role card에는 결과·템포·교전 경향을 compact하게 표시하고, Matchup Detail은 각 개념과 설명 및 기존 raw metric을 분리해 표시한다.
- NotFound, Missing Role, dataset unavailable/invalid에는 Level Tempo와 Combat Pressure를 표시하지 않는다. 통계가 존재하지만 n<20인 경우에만 `판정 보류`다.
- dataset schema, StatsCollector, polling, lookup 횟수는 변경하지 않으며 두 값은 presentation 생성 시 기존 query-oriented aggregate에서 파생한다.
- 전체 13,021 key 회귀 결과는 Level Tempo A 780 / Neutral 240 / B 765 / Insufficient 11,236, Combat Pressure A 531 / Neutral 781 / B 473 / Insufficient 11,236이며 연구 결과 및 reverse symmetry와 일치한다.

### Stage 9-3 Matchup Explanation UX

- 기존 Sample Reliability, Outcome Advantage, Level Tempo, Combat Pressure 결과와 raw metric만 사용하는 deterministic rule-based explanation을 추가했다.
- `MatchupExplanationService`가 사용자 설명 문구를 한 곳에서 생성하며 classifier의 판정이나 threshold를 다시 구현하지 않는다.
- Outcome 설명은 실제 표본 수와 아군 최종 승률을 사용하며, Neutral을 작은 effect와 표본 불확실성으로 구분한다. Wilson 같은 고급 통계 용어는 사용자 UI에 직접 노출하지 않는다.
- Level Tempo 설명은 classifier가 보존한 L2/L3/L6 signal을 사용해 실제 방향 근거를 `L2·L3` 등의 형태로 표시하고, 서로 충돌하는 signal과 한쪽으로 충분히 모이지 않은 경우를 구분한다.
- Combat Pressure 설명은 아군/적군의 15분 이전 직접 맞상대 솔로킬 event-per-match를 같은 formatting policy로 표시한다.
- Portrait와 Landscape dashboard는 기존 compact 결과만 유지하며 긴 설명을 추가하지 않는다. Matchup Detail에서 결과 경향, 레벨 템포, 교전 압박의 설명을 각각 독립적으로 표시한다.
- NotFound, Missing Role, dataset unavailable/invalid에는 설명을 만들지 않으며 실제 통계가 있는 n<20 matchup에서만 각 판정 보류 이유를 표시한다.
- 새로운 통계 모델, threshold, dataset lookup, Live Client 요청, persistence 또는 combined matchup verdict는 추가하지 않았다.

### Stage 9-4A Team Summary Model Research

- 현재 production dataset의 Outcome, Reliability, Level Tempo, Combat Pressure classifier 결과를 전체 및 Role별로 분석하고 Outcome × Reliability 교차표를 생성했다.
- dataset이 개별 Role matchup aggregate만 포함하고 실제 한 경기의 5-Role 조합 row를 포함하지 않는다는 한계를 명시했다. Team Summary는 실제 팀 승률, 조합 시너지 또는 승패 예측이 아니다.
- Equal Vote, minimum-evidence 변형, reliability-weighted, capped sample-weighted, directional-only, conservative equal majority, strong-only 후보를 deterministic fixture로 비교했다. fixture는 rule behavior 검증용이며 empirical game frequency로 해석하지 않는다.
- sample-size weighting은 큰 표본 matchup 하나가 반대 방향 Role 여러 개를 뒤집을 수 있고, reliability weighting은 categorical band를 보정 확률처럼 오해하게 만들 수 있어 production 추천에서 제외했다.
- production 후보는 Outcome Summary만 사용하는 `E_CONSERVATIVE_EQUAL`이다. 판정 가능한 Role이 최소 3개이고, 같은 방향 Role이 최소 2개이면서 반대 방향보다 많을 때만 해당 경향을 표시한다. 동률이나 약한 evidence는 Neutral, eligible Role 3개 미만은 InsufficientEvidence다.
- 추천 UI는 InGame dashboard 상단의 compact `팀 결과 경향` strip이며, Team Reliability badge 대신 eligible/아군/적군/중립/unavailable Role 수를 표시한다.
- Level Tempo와 Combat Pressure는 Role별 표시를 유지하도록 권장한다. 특히 Combat Pressure의 직접 맞상대 솔로킬은 JUNGLE/SUPPORT에서 team-level 의미가 제한적이다.
- 연구 산출물은 `Tools/AnalyzeStatisticsDataset/Output/team-summary-model-analysis.json`과 `.md`이며 deterministic output을 확인했다.
- No production Team Summary classifier/UI implemented yet.

### Stage 9-4B Production Outcome Team Summary

Stage 9-4A에서 선택한 conservative equal-vote 정책을 production WPF에 적용한다.

```text
Eligible roles < 3
→ InsufficientEvidence

Ally roles >= 2
AND Ally roles > Enemy roles
→ AllyTendency

Enemy roles >= 2
AND Enemy roles > Ally roles
→ EnemyTendency

otherwise
→ Neutral
```

- `TeamOutcomeSummaryClassifier`는 이미 생성된 5개 Role presentation의 query-oriented Outcome enum만 동일한 1표로 집계한다.
- Ally/Neutral/Enemy Outcome은 eligible이며, InsufficientEvidence, Missing Role, NotFound, dataset unavailable/invalid는 eligible이 아니다. Neutral은 unavailable과 구분한다.
- reliability, sample size, raw win rate 및 Role importance weighting은 사용하지 않는다. Team sample size나 평균 승률도 만들지 않는다.
- `TeamOutcomeSummaryResult`는 total/available/eligible Role 수와 Ally/Enemy/Neutral/Insufficient/unavailable count를 보존한다.
- InGame dashboard 상단에 compact `팀 결과 경향` strip을 표시하고 `판정 N/5 · 아군 N · 적군 N · 중립 N` evidence를 함께 제공한다. Waiting 및 전체 dataset 오류 상태에서는 표시하지 않는다.
- Team Summary는 기존 Role 통계 조회가 끝난 뒤 runtime에서 파생되며 추가 dataset lookup, Live Client 요청, polling 또는 persistence를 만들지 않는다.
- Team Summary aggregates only role-level Outcome categories. It does not calculate team win probability or composition advantage.
- Level Tempo and Combat Pressure remain role-level only.

### Stage 9-4C Portrait Summary UI Simplification

- Portrait의 raw percentage/count 표를 제거하고 Outcome, Level Tempo, Combat Pressure 요약을 중심으로 구성했다.
- 표본 20경기 이상에서는 query 방향 raw count를 한 번 비교해 L2/L3/L6/솔로킬 compact 방향 indicator를 표시한다.
- 표본 20경기 미만에서는 production 판정 보류를 유지하고 raw 방향 indicator를 숨긴다.
- Landscape의 상세 통계와 Matchup Detail의 raw 수치 및 설명은 변경하지 않았다.
- statistics model, production classifier threshold, dataset schema, lookup 및 polling 동작은 변경하지 않았다.

### Stage 9-5 Statistics Update Pipeline Completion

- `PublishStatisticsDataset`은 명시적으로 선택한 Collector `MIN900S` output을 공통 validator로 검증한 뒤 packaged baseline과 remote-upload artifact를 생성한다.
- remote artifact는 `manifest.json`과 revision별 immutable `master_plus.{datasetRevision}.json`만 포함한다. 동일 source의 revision과 dataset SHA-256은 재실행해도 동일하며, 같은 revision의 다른 content를 덮어쓰지 않는다.
- manifest schema는 dataset schema/revision, patch, scope, region, queue, 최소 경기 길이, accepted matches, observations, matchup key 수, dataset 생성/배포 시각, file size, SHA-256 및 download URL을 포함한다.
- runtime manifest endpoint는 `LOLMATCHINFO_STATS_MANIFEST_URL` 환경 변수 하나로 구성한다. Release에서는 manifest와 dataset 모두 HTTPS만 허용하고, Debug에서만 loopback HTTP를 허용한다. relative dataset URL은 manifest URL 기준으로 해석한다.
- updater는 manifest 64 KiB 및 dataset 128 MiB size guard, streaming download, declared/actual size, SHA-256, 공통 dataset validation과 manifest-dataset metadata 일치를 모두 확인한 뒤에만 `%LocalAppData%/LoLMatchInfo/Data/Statistics/master_plus.json`에 설치한다.
- dataset과 companion manifest 교체는 임시 파일과 rollback 파일을 사용하는 원자적 흐름이며, 성공/실패 후 임시 rollback을 정리해 영구 `.bak`을 만들지 않는다. download 중단이나 교체 실패 시 기존 local dataset을 보존한다.
- revision이 같으면 최신 상태, 더 크면 update available, 더 작으면 downgrade 금지로 처리한다. current runtime 우선순위인 valid LocalAppData update → packaged baseline과 invalid-local fallback은 유지한다.
- Settings 데이터 화면은 현재 patch, 적용 경기 수, source, revision, dataset 생성 시각을 보여주고, 중복 클릭 없이 비동기로 확인/다운로드한다. 설치 후 hot reload 대신 `앱을 다시 시작하면 적용됩니다`를 안내한다.
- synthetic/temp-file 회귀 검증은 latest, newer/install, SHA mismatch, size mismatch, malformed manifest, invalid dataset, older/no downgrade, network failure, interrupted download, atomic rollback 및 packaged fallback을 포함한다.
- WPF runtime updater에는 Riot API key, GitHub token 또는 hosting credential이 없으며 private repository API에 의존하지 않는다.
- 정적 HTTPS hosting 인계 절차와 cache/upload 순서는 `docs/remote-statistics-hosting.md`에 기록했다.
- 실제 public remote hosting endpoint는 아직 배포되지 않았다. Runtime updater는 hosting-ready이며 temp files와 실제 loopback HTTP를 사용하는 integration fixture로 검증되었다.
- 현재 packaged baseline은 patch `16.17`, accepted matches `51,649`, observations `258,245`, matchup keys `15,547`, dataset revision `1788854099077`이다.

### Stage 9-5B-1 Cloudflare Pages Deployment Package

- `PublishStatisticsDataset --prepare-cloudflare-pages`가 검증된 publisher manifest와 manifest가 정확히 지정한 revision dataset으로 Direct Upload package를 생성한다.
- package root에는 Cloudflare cache policy용 `_headers`와 `/statistics/master-plus/` 아래의 manifest 및 단일 immutable revision dataset만 포함한다.
- dataset 전체 validation, manifest metadata, file size/SHA-256, 25 MiB per-file guard, file allowlist, credential pattern 및 개인 식별자 scan을 모두 통과한 뒤에만 기존 generated folder를 교체한다.
- dataset은 byte-for-byte 복사하므로 publisher artifact의 SHA-256이 유지된다. manifest의 같은-directory relative URL을 유지해 첫 배포 전에도 package를 만들 수 있고 pages.dev/custom domain에 독립적이다.
- generated `CloudflarePagesDeploy/` folder는 Git에서 제외한다. source code, docs 및 `Data/Statistics` baseline의 기존 추적 정책은 변경하지 않는다.
- manifest는 `no-cache, max-age=0, must-revalidate`, revision dataset은 `public, max-age=31536000, immutable`을 사용한다.
- package 생성 시 실제 deployment path에서 manifest/dataset HTTP 200과 dataset SHA를 확인한다. updater loopback 회귀도 `/statistics/master-plus/` 구조와 relative dataset resolution을 검증한다.
- Stage 9-5B-1에서 static upload package를 준비했으며, 실제 public Workers endpoint 연결 상태는 다음 Stage 9-5B-2에 기록한다.

### Stage 9-5B-2 Production Endpoint Final Connection

- Stage 9 Statistics / Publishing / Hosting complete.
- Cloudflare Workers Static Assets public endpoint를 production statistics distribution에 연결했다.
- production manifest endpoint는 `DatasetUpdateOptions` 한 곳에서 관리하며, 일반 사용자는 환경 변수를 설정할 필요가 없다. 명시적 `LOLMATCHINFO_STATS_MANIFEST_URL` override가 우선하고 Debug loopback HTTP fixture도 유지한다.
- Release updater의 HTTPS-only 정책과 기본 TLS certificate validation을 유지하며 Riot API key, Cloudflare token 또는 다른 client credential을 사용하지 않는다.
- manifest의 same-directory relative dataset URL이 Workers HTTPS URL로 정상 해석됨을 확인했다.
- 실제 remote manifest/dataset GET, file size, SHA-256, shared dataset validation 및 manifest↔dataset metadata 검증을 통과했다.
- 실제 Workers 응답은 manifest에 `no-cache, max-age=0, must-revalidate`, immutable revision dataset에 `public, max-age=31536000, immutable`을 제공하며 두 파일 모두 `application/json`이다.
- 현재 public revision `1788854099077`은 packaged baseline과 같아 Release Settings의 정상 결과는 `최신 상태`이며 restart-required가 아니다.
- local update가 없는 개발 PC에서는 현재 active revision source가 packaged baseline임을 확인했다.
- network/invalid manifest failure 시 기존 dataset 보존, packaged fallback, downgrade 방지 및 same-revision immutable identity conflict 회귀 검증을 유지한다.
- production deployment는 새 immutable dataset을 먼저 올리고 공개 GET/SHA를 확인한 뒤 manifest를 마지막에 교체한다.
- public production endpoint verified. Current remote revision equals packaged baseline. Public newer-revision download/install/restart activation will be verified with the next valid dataset revision; production artifact는 테스트 목적으로 조작하지 않는다.

### Stage 9-6 Full Regression & Bug Hunt

- 2026-09-09 검증 결과와 항목별 근거를 `docs/regression-test-report.md`에 기록했다. 전체 GUI 회귀 완료 또는 release 승인 상태는 아니다.
- 총 146개 검증 항목: PASS 69 / FAIL 2 / PARTIAL 56 / BLOCKED 0 / MANUAL VERIFICATION REQUIRED 19.
- 미해결 P0 0 / P1 1 / P2 0 / P3 1. 미검증 범위의 안전성을 보장하지 않는다.
- WPF Debug/Release, StatsCollector, Publisher, Analyzer의 5개 build 모두 warning 0 / error 0. 실제 Debug/Release startup, 주요 Developer Test preset, 5 Role Detail와 Back 반복, Release Settings production UpToDate를 확인했다.
- 현재 packaged/public dataset은 patch 16.17, accepted 51,649, observations 258,245, keys 15,547, revision 1788854099077. 전체 level invariants, production classifier reverse symmetry, updater self-test 및 public GET/size/SHA 검증이 통과했다.
- B001 (Medium/P1): Analyzer의 `ProductionLaneIndicatorsVerification`이 과거 연구 representative classification을 최신 dataset에도 강제하여 exit 1. WPF startup 문제가 아니며, snapshot-specific 검증을 명시적 identity/fixture로 분리한 후 재검증해야 한다.
- B002 (Low/P3): Release UI에는 TEST가 보이지 않지만 DeveloperTestWindow BAML 및 개발 fixture 구현은 DLL에 포함된다.
- 이번 단계는 discovery 결과를 기록했으며 production/분석 코드는 변경하지 않았다. GUI pixel/DPI/keyboard, 수백 회 lookup, 취소/재시작 및 장기 리소스 검증의 미완료 범위는 보고서에 명시했다.
- Stage 10 준비 작업은 가능하나 regression gate 통과는 보류한다. B001 수정과 남은 검증 보완 후 판단한다.
- UI resize 테스트가 사용자 settings에 저장되어 초기 관찰 창 위치/크기를 복원했다. 테스트 전 원본 byte backup이 없어 과거 landscape 크기의 정확한 복원은 보장하지 못하며, 상세 내역을 보고서에 남겼다.

### Stage 9-6B Regression Fix Pass — 재개 및 검증 완료 (2026-09-09)

- 기존 working tree의 B001/B002 수정과 회귀 실행기를 보존·검토한 뒤 중단 지점부터 검증했다. production 코드의 추가 수정은 필요하지 않았다.
- B001 FIXED/VERIFIED: Analyzer snapshot-specific representative/distribution assertion을 제거한 기존 변경과 synthetic 정책 검증 통과. 현재 dataset 및 분포가 달라지는 scaled synthetic dataset 모두 exit 0; malformed JSON exit 1. production threshold 변경 없음.
- B002 FIXED/VERIFIED: Release Compile/Page 제외와 DEBUG 참조 분리. PE metadata/BAML 검사에서 Release 개발 타입 0/BAML 0, Debug 5/2로 개발 테스트 유지 확인.
- WPF Debug/Release 및 Analyzer build warning 0/error 0. DEBUG validators, Start/Stop 서비스 계층 20회, partial/notfound/custom/off-meta, no-game 복귀, public UpToDate 통과. GUI 조작 테스트와 구분한다.
- 최신 회귀 매트릭스: PASS 74 / FAIL 0 / PARTIAL 53 / BLOCKED 0 / MANUAL VERIFICATION REQUIRED 19. 알려진 미해결 P0 0 / P1 0 / P2 0 / P3 0.
- Stage 10-A 준비·설계 진입 가능. 전체 GUI regression gate/release 승인은 보류. Stage 10-A가 전체 회귀 통과를 전제로 한다면 진입도 보류한다. pixel/DPI/multi-monitor/keyboard, 취소·재시작·장기 stress 및 실제 public newer revision activation은 미완료다.
- 상세 명령·결과·변경 파일·보호 범위는 docs/regression-test-report.md의 최신 Stage 9-6B 절 참조. 기존 Stage 9-6의 FAIL/OPEN 숫자는 당시 이력이다.

### Stage 9-6D Portrait Priority Readability Polish (2026-09-10 완료)

- Portrait summary hierarchy strengthened: 결과/초반/교전 label은 공통 10 DIP muted style, value는 9→13 DIP 및 기존 SemiBold를 사용한다. 38 DIP label column과 18 DIP 높이의 세 행을 공유한다.
- Outcome/Tempo/Combat value readability improved: 기존 문구·색상 trigger·binding을 유지하며 value에 ellipsis를 적용하지 않는다. 실제 normal5v5에서 긴 중립 문장과 양쪽 경향 문장을 확인했다.
- Compact chips visually demoted to secondary information: 글씨 크기(9 DIP)와 cyan/purple/gray, WrapPanel을 유지하고 테두리 대비만 낮췄다.
- 카드 패딩 9,6→9,4 및 챔피언 영역 상하 margin 3→2로 공간을 재배분했다. 이전/이후 화면의 카드 높이는 약 118→130px이며 다섯 카드 총 증가량은 약 60px이다. 작은 창에서는 스크롤로 전 역할에 접근한다.
- No statistics logic changed. C# model/presenter/classifier/threshold, wording, tooltip 및 navigation 변경 없음. Landscape/Detail unchanged; 실제 화면과 orientation 복귀도 확인했다. Team Summary strip 변경 없음.
- Debug/Release 최종 build 각각 warning 0/error 0. Debug 실제 normal5v5/Partial/NotFound/Detail/Landscape 및 완료된 Start/Stop 5회 확인. Release Home/WAITING/패치/경기 수 및 TEST 비노출 startup 확인.
- 최소 크기 목표 420×760와 440×820에 대응하도록 조작했으며 도구가 반환한 실제 캡처 영역은 406×753 및 426×813이다. OS 외곽 크기는 별도 API로 계측하지 않았으므로 정확한 외곽 크기 검증과 구분한다. 두 영역 모두 긴 판정문/신뢰도 표시, 수평 overflow 없음, 스크롤 하단 SUPPORT 접근을 확인했다.
- Portrait readability P1 = FIXED (관찰한 기본 DPI/fixture 범위). 3초 판단 가독성 개선은 렌더 기반 평가이며 사용자 실험 결과는 아니다.
- n<20 Custom 실제 UI는 picker 자동화 입력이 반영되지 않아 미확인. 기존 n<20 판정 보류/chip hidden 정책 및 binding은 수정하지 않았다. 재실행 뒤 남은 Start/Stop을 완료했으며 실패한 입력은 성공 횟수에 넣지 않았다.
- 범위 밖 Settings 대비/metadata, drawer/scrollbar, Detail affordance, Home/onboarding, tooltip/용어 개선은 남겨 두었다. 기존 dirty files와 문서 이력을 보존하고 commit/reset/revert/delete하지 않았다.

## Stage 10-A — Release Readiness Foundation (2026-09-10)

Stage 10-A 배포 준비 기반을 구현하고 아래 범위에서 검증했다. 시작 상태는 `main`, `aa35d13` (Stage 9 complete), clean working tree였다. 기존 Stage 9 통계/classifier/Team Summary 정책, dataset schema, collector, production endpoint와 updater 보안 정책을 유지한다.

- Logging architecture: `App`이 `AppLogger`와 `AppDiagnosticsService`를 소유하며 생성자 주입으로 loader/polling/champion metadata/Settings updater에 연결한다. 새로운 DI 또는 logging package 없음. 공유 updater를 쓰는 console 도구는 기본 no-op logger를 사용한다.
- Log path: `%LocalAppData%\LoLMatchInfo\Logs`. UTF-8 일별 일반 로그 및 별도 crash 파일. 기존 settings/Data 경로 유지. 30일/최대 100개 managed 파일, 일별 일반 로그 약 4 MiB 상한, lock으로 thread 간 write 보호. 모든 logging I/O 실패는 best effort로 격리한다.
- Crash handling: Dispatcher/AppDomain/TaskScheduler handler를 resource 초기화 전에 등록한다. unexpected unhandled 및 필수 dataset startup 실패는 Critical → crash diagnostics → 한국어 안내 → 종료. interlocked 재진입 방어 및 마지막 process exit 경로가 있다. UI와 unobserved Task의 실제 fixture 종료를 확인했다.
- Recoverable: 정상 no-game/network 부재는 반복 Error로 남기지 않는다. invalid local dataset은 Warning과 packaged fallback을 유지한다. optional metadata/icon fallback은 비치명적이다. updater 실패는 기존 데이터 사용 가능/재시도 안내를 제공한다.
- Privacy rules: 로그 API에는 고정 enum 이벤트/숫자만 전달한다. 예외 원문 Message/Data/ToString, 파일 경로, raw player/HTTP/환경변수 데이터는 저장하지 않는다. 예외 타입/HResult/안전한 분류 메시지/파일 경로 없는 메서드 stack/inner chain만 남긴다. 예외 12개·각 48 frame 상한. 원문 메시지 제거에 따른 원인 정보 손실은 의도적인 privacy tradeoff다.
- Diagnostics model: immutable `AppDiagnosticsSnapshot`으로 app/build/.NET/OS/architecture와 active dataset patch/revision/generated/source/location/matches, current game patch/compatibility를 공유한다. Settings About/Data와 startup/crash가 같은 source를 사용한다. 설치됐지만 재시작되지 않은 revision은 active snapshot과 분리한다. app version은 assembly runtime metadata에서 읽는다.
- Patch compatibility source: 기존 `playerlist`/`activeplayername`에 game version이 없고 Riot 공식 Live Client sample의 `gameData`에도 version 필드가 없다. production game patch는 Unknown이다. 추가 Riot Web API/key 요청, 날짜/Data Dragon/manifest timestamp 추정 없음. 실제 게임 패치는 MANUAL/BLOCKED이며 확인했다고 주장하지 않는다.
- Patch normalization: 2~4개 숫자 component를 major.minor로 비교한다. CurrentPatch/PatchMismatch/Unknown, 잘못된 값은 Unknown. generated time만 오래됐다고 stale로 분류하지 않는다. Settings에 게임 패치/데이터 패치와 비차단 한국어 안내를 추가했다. 게임 종료 시 current game patch는 초기화한다.
- Debug/Release separation: Debug 개발자 창에 synthetic mismatch/reset 및 UI/Task crash 버튼을 추가했다. 기존 Release Compile/Page exclusion 유지. 최종 PE 검사: Release 개발 타입 0, 개발 BAML 0, crash/patch fixture method 0. Debug 각각 5/2/4 유지. Release의 기존 collapsed test shell markup은 실행 가능한 개발 기능을 제공하지 않는다.
- Tests performed: 요청한 5개 build invocation (앱 Debug/Release + 도구 3개) 모두 최종 warning 0/error 0. synthetic diagnostics, 500개 concurrent log write, write failure, cleanup, privacy, exception chain/long message/null stack/depth, loader fallback 통과. 기존 statistics regression 및 updater self-test 19개 통과. Debug normal5v5 Start/Stop 3회, Settings unknown/mismatch, UI fatal/Task fatal, Release Home/Data/UpToDate/정상 종료 확인. 실제 무게임 5분 로그 Waiting 1회/Error 0/crash 0. 정상 종료는 crash를 만들지 않는다.
- Production GET-only verification: revision `1788854099077`, patch `16.17`, accepted `51,649`, bytes `7,541,418`, SHA-256 `1aba43ed6108d5b2ba07b1ce170f546f8417d1c319f6a6459d97c077bb27fb14`. 실제 response 기반이며 production upload/deploy 없음.
- Known limitations: AppDomain 전용 실제 crash, missing packaged resource 실제 startup, ACL/disk-full, 다중 프로세스 log contention, 실제 게임 패치 연동은 전수 runtime 검증하지 않았다. Task fixture는 이번 실행에서 통과했지만 GC timing 일반 보장은 없다. Stage 9의 DPI/접근성/기타 기존 수동 항목을 이번 결과로 일괄 PASS 처리하지 않는다.

판정: **Stage 10-A foundation 완료**. 새로 확인된 미해결 P0/P1 없음. Stage 10-B 작업 계획을 시작할 수 있으나 전체 public release/installer/signing 승인 또는 실제 game patch 지원 완료를 의미하지 않는다. 상세 운영 정책은 `docs/diagnostics-and-logging.md`, 실행 증거와 제한은 `docs/regression-test-report.md` 참조.

## Stage 10-B — Product UX (2026-09-10)

Stage 10-A의 미커밋 변경을 보존한 상태에서 Product UX를 구현했다. 시작 브랜치는 `main`, HEAD는 `aa35d13`이다. 통계 metric/classifier/Team Summary 계산, dataset schema, Riot API, updater 검증·설치 정책 및 logging/crash 기반은 유지한다. 새로운 의존성은 없다.

- Home: 현재 상태 → 시작 행동 → 자동 감지/인게임 흐름 → 적용 통계 순서. 읽기 폭을 제한하고 상단에 배치하며 작은 창에서는 스크롤한다. 실제 렌더에서 시작 흐름이 충분하여 별도 modal onboarding/persistence는 추가하지 않았다.
- States: 정상 게임 대기, 역할 정보 확인 중, 수집된 조합 통계 없음, 근거 부족에 따른 판정 보류, 업데이트 실패, 게임 패치 확인 불가를 구분한다. Unknown은 현재 연결 정보의 한계를 설명하며 게임 실행 후 반드시 확인된다고 약속하지 않는다. mismatch는 non-blocking이다.
- Cards: 유효한 챔피언 쌍에만 작은 `›` 진입 표시. Hand/hover border/keyboard focus를 유지·보완한다. Portrait 결과·초반·교전 값 13 DIP SemiBold와 secondary chip/WrapPanel을 유지한다. 용어/레벨 선취/15분 이전 무도움 직접 맞상대 솔로킬 툴팁을 추가했다. reliability 이름과 1–4/5–19/20–49/50+ 경계는 변경하지 않았다.
- Team Summary: `라인별 결과 경향 요약` 설명을 추가했다. 팀 승리 확률로 표시하지 않고 기존 equal-vote 정책을 유지한다.
- Settings: section title/설명 대비, dark navigation/expander/toggle를 정리했다. 초기화 confirmation 유지. 저장 경로·기억된 정확한 창 크기·revision/source는 접힌 정보로 이동했다. 데이터 정상 여부와 원격 최신 여부를 별도 카드로 표시한다. About은 버전과 앱 설명 중심이다.
- Theme/Detail/Landscape: dark translucent drawer backdrop, native Track/Thumb 기반 16 DIP dark scrollbar, dark wrapping tooltip, 공통 focus visual. Detail 내부 champion ID 숨김, 설명 13 DIP/20 line height, 중복 solo total footer 제거. Landscape는 가용 폭 1080 미만 2열/이상 3열, 최대 폭 1260, 아군/적군 라벨 및 긴 지표명 줄바꿈을 사용한다.
- Debug-only: 기존 Developer Test 창에 420×760/440×820/760×420/1280×800 viewport 및 localhost 업데이트 실패 검증 버튼을 추가했다. endpoint 주입은 해당 Settings 인스턴스에 한정되고 환경변수·production endpoint는 변경하지 않는다. Release 제외 검사는 새 메서드까지 검사한다.
- Verification: WPF Debug/Release, StatsCollector/PublishStatisticsDataset/AnalyzeStatisticsDataset 모두 warning 0/error 0. 로그·통계 회귀/20회 fixture cycle/partial/notfound/Release 타입·BAML 분리 통과. 실제 Debug 화면 및 Release Home/Settings/About/Data/Unknown/production UpToDate 확인. 스크린샷은 도구 출력에만 남겼다.
- Scope limits: 실제 League 경기/게임 패치 수집, 모든 DPI·긴 이름 조합·스크린리더 전수 검증은 수행하지 않았다. 작은 창은 다섯 역할 및 상세 내용에 스크롤이 필요하다. synthetic NotFound는 실제 미등록 champion ID를 사용하는 기존 fixture이다. Settings 소유 창의 모든 키보드 경로는 자동화 도구 제약으로 전수 검증하지 않았다.

판정: **Stage 10-B 완료 (명시한 UX 범위)**. P2-1~P2-7 개선 완료, 새 미해결 P0/P1 없음. Stage 10-C 계획/구현을 시작할 수 있다. 전체 공개 배포·서명·installer 승인이나 실제 게임 패치 지원 완료를 의미하지 않는다. 상세 검증은 `docs/regression-test-report.md` 참조.

## Stage 10-C — Distribution & Release Packaging (2026-09-11)

Stage 10-A/B의 미커밋 변경을 보존한 채 Windows 배포 기반을 추가했다. 제품명 `LoL Match Info`, assembly/executable `LoLMatchInfo`, closed-beta 버전 `0.9.0`을 SDK-style project metadata 한 곳에서 관리한다. About와 diagnostics는 assembly version을 계속 읽으며 executable Product/File metadata도 같은 값을 사용한다.

- `WinX64.pubxml`: Windows x64 self-contained, multi-file, trimming/ReadyToRun 비활성 Release publish. installer를 통해 묶으므로 single-file보다 packaged dataset/champion asset의 단순하고 검증 가능한 경로를 우선했다.
- `scripts/build-release.ps1`: 안전하게 `artifacts` 하위 staging만 재생성하고 publish/검증/portable ZIP/Inno installer(compiler가 있을 때)/SHA-256/app release manifest를 만든다. Git clean, remote upload, production dataset 변경은 없다.
- `scripts/validate-release.ps1`: executable, 7,541,418-byte baseline과 manifest, champion metadata/173 icons를 확인하고 PDB/source/dev tools/DeveloperTestWindow/high-risk secret/privacy marker/developer path를 거부한다.
- `installer/LoLMatchInfo.iss`: Inno Setup 6 기반 per-user 설치. `%LocalAppData%\Programs\LoLMatchInfo`, Start Menu 필수, Desktop 선택, Add/Remove Programs와 uninstall, 같은 AppId upgrade, LocalAppData user data 보존을 정의한다.
- local artifact: publish 575 files / 158,844,328 bytes, portable ZIP 68,291,025 bytes. ZIP SHA-256 `edca2108c7ef4ef9e388c0ea5b31d2fe60d07ce97fba91bb88a0c798c6422b7d`. generated artifacts는 Git ignore다.
- 실제 publish 실행본은 project 밖 Temp working directory에서 5~8초 생존 후 정상 종료했고 창 제목 `LoL Match Info`, Release diagnostics version `0.9.0`, packaged revision `1788854099077`, Waiting/Shutdown 로그를 확인했다. publish 폴더 runtime write는 0개였다.
- production statistics endpoint read-only regression은 UpToDate, Release 개발 type/BAML/fixture method는 0, 기존 통계·logging 회귀는 통과했다. 요청한 앱 Debug/Release와 3개 tool build는 모두 warning 0/error 0이다.
- 최초 검사에서는 per-user Inno Setup 경로를 탐지하지 못해 installer lifecycle이 `BLOCKED`였다. Stage 10-C2에서 `%ProgramFiles(x86)%`, `%ProgramFiles%`, `%LocalAppData%\Programs`와 PATH를 모두 검사하도록 수정했다. signing certificate/signtool은 없고 앱 아이콘은 remaining branding polish다.

### Stage 10-C2 — Inno Setup Detection and Installer Lifecycle (2026-09-11)

- 실제 Inno Setup 6.7.3 compiler를 `%LocalAppData%\Programs\Inno Setup 6\ISCC.exe`에서 자동 탐지했다. source/script에 사용자 이름은 없다.
- 최종 0.9.0 installer build 성공: `LoLMatchInfo-0.9.0-win-x64-setup.exe`, 51,627,661 bytes, SHA-256 `905bc70aa386a9dea7644dc023c29996083b35ffb816bf392224ab187192c4e1`. checksum과 release manifest의 filename/version/size/hash가 일치한다.
- silent per-user install exit 0. Inno log는 user privileges `None`, administrative install mode `No`, restart 불필요를 기록했다. 실제 경로 `%LocalAppData%\Programs\LoLMatchInfo`, HKCU installed-app metadata 0.9.0/publisher/uninstall, Start Menu app/uninstall shortcut 생성, Desktop shortcut 기본 미생성을 확인했다.
- 설치본은 577개 파일이며 packaged publish 575개와 Inno uninstall 파일 2개다. baseline/manifest, champion metadata/173 icons, self-contained runtime이 있고 dev tool/source/PDB/fixture 파일은 없다.
- Start Menu shortcut 실행은 실제 installed executable을 열었고 title `LoL Match Info`, responding true, diagnostics version 0.9.0, packaged patch 16.17/revision 1788854099077/matches 51,649, Waiting 및 정상 Shutdown을 기록했다. 종료 후 orphan process 0, 575개 payload hash가 publish와 모두 일치해 install-directory runtime write가 없었다.
- uninstall exit 0 후 install directory, Start Menu group, HKCU uninstall entry가 제거됐다. `%LocalAppData%\LoLMatchInfo`와 기존 settings SHA-256 `532CD0343AE5614BC7EB441769465E83FF01D08E934D123AE67675F45E77848B`, 로그는 보존됐다. 동일 0.9.0 reinstall/Start Menu startup도 통과했다.
- source를 변경하지 않은 isolated 0.9.1 publish/installer로 running-app upgrade를 실행했다. Restart Manager가 실행 중 앱을 발견·종료했고 installer exit 0, installed/app version 0.9.1, executable hash 변경, settings/log 보존을 확인했다. 이후 0.9.0으로 재설치하고 최종 0.9.0 release를 다시 build했다.
- production endpoint는 기존 read-only regression에서 UpToDate다. native GUI automation surface가 없어 설치본의 About/Settings Data 버튼을 이번 실행에서 직접 조작하지 못했으므로 해당 화면 확인만 `PARTIAL`; process/metadata/log와 updater 회귀 증거는 PASS다. 기존 crash log 2개는 과거 Debug fixture이며 정상 installer 실행으로 새 crash는 생성되지 않았다.

판정: **Stage 10-C COMPLETE (closed-beta distribution scope)**. installer build/install/Start Menu launch/logging/uninstall/user-data preservation/reinstall/running-app upgrade/artifact validation을 실제 통과했다. 새 P0/P1/P2는 없고, automatic downgrade 차단 부재는 기존 release-process 정책으로 남는 P3다. signing은 `UNSIGNED BETA`, final `.ico`와 직접 GUI Settings smoke는 Stage 10-D 전 remaining manual polish다.

## Stage 10-E — Champion Skill Reference & Skill Order (2026-10-03)

시작 working tree: `main`, clean, HEAD `cfc7aba`. 기존 Stage 10-A/B/C/D 및 통계 시스템을 유지했다. 커밋/리셋/리버트/기존 파일 삭제를 수행하지 않았다.

- 인게임 상대 초상화 → 같은 앱의 Champion Skill Detail → Back/Escape → 인게임. 기존 Role card → Matchup Detail 유지. Portrait/Landscape에서 클릭 충돌이 없음을 실제 UI로 확인했다.
- 한국어 챔피언/role/패치, QWER 아이콘·공식 짧은 설명·랭크별 기본 쿨타임 표시. dark/cyan/purple 테마와 세로 스크롤 사용. 420×760, 440×820, 760×420 및 TOP/JUNGLE/MID 검증.
- Riot Data Dragon 16.17.1(기존 메타데이터와 동일 패치), 전체 지원 173개 챔피언 / 692개 아이콘 / 누락 챔피언 0. 21개 소스 쿨타임은 유효한 양수 배열을 확인할 수 없어 정보 없음으로 표시하며 manifest에 기록한다. 현재 최신 버전이라고 주장하지 않는다.
- `Tools/ChampionSkillDataCollector`: developer-only .NET 10 Windows 콘솔, bulk JSON + 19 sprite 파일, 캐시/순차 저빈도 요청. `Data/ChampionSkills` 최종 데이터만 배포. 앱 런타임은 local read only, API key 불필요, startup I/O 없음, 챔피언 단위 lazy/cache 및 오류 fallback.
- OP.GG 도움말과 2026-09-14 이용약관이 자동 수집에 대해 충돌한다. 사용자 지정 implementation gate 미통과로 외부 scraping/API 수집은 구현하지 않았다. 실제 Skill Priority는 미제공. Champion+Role+Patch 모델/정확한 조회/출처 및 선택적 sequence UI/정상 empty state를 준비했고 synthetic 데이터는 테스트에만 사용했다.
- Debug/Release 앱 및 기존 도구 3개, 신규 생성기 build: warning 0/error 0. 기존 통계/진단 회귀, 전체 데이터 검사, 오류/중복/역할/패치 분리, Debug/Release WPF 컨트롤 smoke 통과. Release TEST UI 없음 직접 확인.
- 최종 Stage 10-C pipeline: warning 없이 publish/portable/Inno installer/checksum 생성 및 release dev/privacy/secret scan 통과. portable 1,442 files, skill files 867, 개발 산출물 0; Inno compiler도 스킬 파일 867개 포함. 설치/제거 라이프사이클은 이번에 재수행하지 않았다.
- publish: 159,188,392 → 163,132,360 bytes (+3,943,968 bytes, 약 2.48%). portable 72,506,324 bytes; installer 55,729,896 bytes. 산출물은 ignored `artifacts/` 아래에 있다.
- 상세 소스/포맷/업데이트 방법/정책 gate: `docs/champion-skill-data.md`. 회귀 결과: `docs/regression-test-report.md`.

판정: **QWER 1차 구현 및 Skill Order 확장/빈 상태 완료, 실제 Skill Order 데이터는 gate에 따라 보류**. Stage 10-E 전체 Skill Priority 제공 완료로 판정하지 않는다. 새 P0/P1 발견 없음. P2 제한 2건: 승인된 Skill Order 소스 미확보, 특수 스킬/21개 쿨타임의 공식 정적 데이터 한계. P3 개선 1건: 복잡한 챔피언 설명의 추가 편집/축약 검토. Stage 11 RC는 이 제한을 수용하고 실제 경기·DPI·배포 수동 acceptance를 진행하는 조건부 상태다. 기존 서명/브랜딩/다운그레이드 관련 제한은 유지한다.

### Stage 10-E cooldown 21개 집중 검증 정정 (2026-10-03)

- 위 초기 21개를 전부 원본 한계로 보던 설명을 정정한다. 동일 버전 16.17.1 공식 championFull.json을 새로 받아 캐시와 SHA-256 일치를 확인하고, 21개 각각의 cooldownBurn/배열/maxrank/QWER/tooltip을 대조했다. 상세: [cooldown audit](docs/cooldown-audit-16.17.1.md).
- 분류: NORMAL_SPECIAL_CASE 9, DISPLAY_IMPROVEMENT_NEEDED 8, DATA_BUG 1, UNVERIFIED 3. UNVERIFIED는 Gangplank E / Karthus Q / Zeri Q이며 임의 정상 처리하지 않았다.
- 실제 오류 Urgot W만 수정: `[12,9,6,3,0]` 복구 및 `5랭크: 토글 전환` 설명. 전체 생성기를 재실행했으며 skill directory 해시 변경은 Urgot.json/manifest.json 두 파일뿐이다. null cooldown은 21 → 20.
- 전체 173 champion / 692 spell / missing 0; QWER·rank·보존 수치 불일치 및 구조적 malformed 0. 20개의 all-zero 원본은 null fallback을 유지한다. 구조 통과를 UNVERIFIED 해결로 간주하지 않는다.
- Debug/Release build warning 0/error 0, skills-only 회귀 PASS. 실제 Debug 앱의 Urgot W / Singed Q / Vi W 세 스킬 페이지 확인: 우르곳 다섯 값과 토글 주석, 나머지 정보 없음 표시 정상.
- Stage 11 RC는 **조건부**, 전체 쿨타임 정확성 승인은 아직 미완료다. 이번 변경은 데이터 검증/최소 수정 범위이며 이전 publish/ZIP/installer는 재생성하지 않았으므로 위 배포 해시는 수정 전 패키지에 해당한다. 배포 전 기존 release pipeline 재실행 필요.

### Stage 10-E cooldown 21개 집중 검증 정정 (2026-10-03)

- 위 초기 21개를 전부 원본 한계로 보던 설명을 정정한다. 동일 버전 16.17.1 공식 championFull.json을 새로 받아 캐시와 SHA-256 일치를 확인하고, 21개 각각의 cooldownBurn/배열/maxrank/QWER/tooltip을 대조했다. 상세: [cooldown audit](docs/cooldown-audit-16.17.1.md).
- 분류: NORMAL_SPECIAL_CASE 9, DISPLAY_IMPROVEMENT_NEEDED 8, DATA_BUG 1, UNVERIFIED 3. UNVERIFIED는 Gangplank E / Karthus Q / Zeri Q이며 임의 정상 처리하지 않았다.
- 실제 오류 Urgot W만 수정: `[12,9,6,3,0]` 복구 및 `5랭크: 토글 전환` 설명. 전체 생성기를 재실행했으며 skill directory 해시 변경은 Urgot.json/manifest.json 두 파일뿐이다. null cooldown은 21 → 20.
- 전체 173 champion / 692 spell / missing 0; QWER·rank·보존 수치 불일치 및 구조적 malformed 0. 20개의 all-zero 원본은 null fallback을 유지한다. 구조 통과를 UNVERIFIED 해결로 간주하지 않는다.
- Debug/Release build warning 0/error 0, skills-only 회귀 PASS. 실제 Debug 앱의 Urgot W / Singed Q / Vi W 세 스킬 페이지 확인: 우르곳 다섯 값과 토글 주석, 나머지 정보 없음 표시 정상.
- Stage 11 RC는 **조건부**, 전체 쿨타임 정확성 승인은 아직 미완료다. 이번 변경은 데이터 검증/최소 수정 범위이며 이전 publish/ZIP/installer는 재생성하지 않았으므로 위 배포 해시는 수정 전 패키지에 해당한다. 배포 전 기존 release pipeline 재실행 필요.
