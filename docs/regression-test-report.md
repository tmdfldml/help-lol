# Stage 9-6 — Full Regression & Bug Hunt

검증일: 2026-09-09 (Asia/Seoul). 이번 문서는 실행한 검사와 미확인 범위를 구분하는 회귀 checkpoint다. **전체 GUI 회귀 완료 또는 무조건적인 release 승인 문서가 아니다.**

## Stage 9-6B — Regression Fix Pass 재개 결과 (2026-09-09)

이 절이 최신 상태다. 아래 Stage 9-6 결과와 버그의 OPEN/FAIL 표기는 발견 당시 이력이며, 최신 판정은 이 절 및 갱신된 매트릭스를 따른다.

- B001 (Medium/P1): FIXED / VERIFIED. 이미 working tree에 있던 snapshot coupling 제거와 ProductionPolicyRegression을 검토하고 실행했다. production classifier/threshold 변경 없음. Outcome의 researchMatch는 같은 입력에서 계산된 연구 모델과의 비교이므로 과거 snapshot coupling이 아니다.
- B002 (Low/P3): FIXED / VERIFIED. Release Compile/Page 제외 및 DEBUG 참조 분리를 확인했다. PE metadata와 embedded .g.resources 검사: Debug 개발 타입 5/BAML 2, Release 개발 타입 0/BAML 0. Debug fixture 경로도 정상이다.
- 알려진 미해결 버그: P0 0 / P1 0 / P2 0 / P3 0. 미검증 범위를 버그 없음으로 판정하지 않는다.
- 갱신 매트릭스: PASS 74 / FAIL 0 / PARTIAL 53 / BLOCKED 0 / MANUAL VERIFICATION REQUIRED 19 (총 146). #60~62, #114, A01을 갱신했다.
- Stage 9-6B의 B001/B002 fix pass는 완료. Stage 10-A 준비·설계 진입 가능. 전체 GUI 회귀 gate 및 release 승인은 보류한다. Stage 10-A가 전체 회귀 통과를 선행 조건으로 요구한다면 그 진입은 보류한다.

### 이번 재개에서 실행한 검증

- `dotnet build LoLMatchup.csproj -c Debug`, `-c Release`, `dotnet build Tools/AnalyzeStatisticsDataset`: 각각 warning 0 / error 0. 변경 없는 Collector/Publisher의 기존 완료 build/self-test는 반복하지 않았다.
- `dotnet run --project Tools/AnalyzeStatisticsDataset --no-build`: exit 0. 현재 51,649 matches / 258,245 observations / 15,547 keys. policyRegression=True, structuralValidation=True, Outcome/Level/Combat reverseSymmetry=True.
- 현재 raw counts와 accepted/observations를 각각 2배로 만든 별도 synthetic input: exit 0. 103,298은 새로 수집한 경기 수가 아닌 합성 값이다. 동일 key 수에도 Level 1824/516/1908/11299, Combat 1377/1611/1260/11299로 분포가 바뀌어도 통과했다. fixture는 ignored `Tools/RegressionScratch/obj/stage96b-scaled-dataset.json`에 보관했다.
- 합성 실행 전 Analyzer Output 파일 bytes를 메모리에 보관하고 finally에서 복원했다. 최종 보고서는 실제 packaged 데이터 기준이다.
- `Tools/RegressionScratch/invalid.json` 입력: 예상 exit 1 / malformed JSON 거부. synthetic invalid canonical pair/duplicate/negative/level invariant 테스트도 통과.
- `dotnet run --project Tools/RegressionScratch --no-build`: 최종 exit 0. WPF DEBUG validators, 서비스/coordinator/presentation Start/Stop 20회(40 notifications), Partial/NotFound/Custom common/off-meta/absent, Stop 이후 no-game null signature, public UpToDate, Debug/Release artifact 검사 통과.
- 첫 scratch 실행의 public HTTPS 요청은 sandbox socket 제한으로 실패했다. 승인된 재실행에서 성공했으며 앱 버그로 집계하지 않았다.
- 정책 경계: reliability 1/4/5/19/20/49/50, Outcome n19/n20·5pp·Wilson 등호, Level ±4.9/5.0/5.1pp 및 0/1/2/3 신호, Combat ±0.09/0.10/0.11, team tie/insufficient, solo EVENT_COUNT>sample 통과.
- `git diff --check`: exit 0 (LF/CRLF 안내만 존재).

### 변경 파일과 보호 범위

- 재개 전에 이미 반영된 B001 파일: `Tools/AnalyzeStatisticsDataset/Program.cs`, `ProductionLaneIndicatorsVerification.cs`, `ProductionPolicyRegression.cs`, `AnalyzeStatisticsDataset.csproj` 및 Output의 JSON/Markdown 8개.
- 재개 전에 이미 반영된 B002 파일: `LoLMatchup.csproj`, `MainWindow.xaml.cs`, `Services/LiveStatisticsCoordinator.cs`. 회귀 실행기는 기존 `Tools/RegressionScratch/Program.cs`와 csproj를 사용했다.
- 이번 재개에서 production 코드 추가 수정은 필요하지 않았다. 상태 문서 2개를 갱신하고 현재 dataset Analyzer output을 재생성했다. 모든 기존 dirty files 및 invalid fixture를 보존했다. reset/revert/delete/commit은 하지 않았다.
- WPF GUI 실행·settings 쓰기·Collector 수집·설치·배포 없음. production dataset SHA256은 기존 기록과 동일한 `1aba43ed6108d5b2ba07b1ce170f546f8417d1c319f6a6459d97c077bb27fb14`이다.
- 서비스 Start/Stop 성공을 GUI 버튼 20회 성공으로 승격하지 않았다. GUI pixel/keyboard/DPI/multi-monitor, 실제 request 중 창 종료, 장기 memory/lookup stress 및 public newer revision 재시작 활성화는 기존 미완료 상태를 유지한다.

## Stage 9-6 당시 결과 및 Stage 10 판단

- 총 146개: 요청 #5~#149의 145개 항목 + 현재 dataset Analyzer 실행 A01.
- PASS 69, FAIL 2, PARTIAL 56, BLOCKED 0, MANUAL VERIFICATION REQUIRED 19.
- 발견 버그: Critical 0 / High 0 / Medium 1 / Low 1 / Cosmetic 0.
- 미해결 우선순위: P0 0 / P1 1 / P2 0 / P3 1.
- P0를 발견하지 않았다는 것은 미실행 시나리오까지 안전하다고 보장한다는 뜻이 아니다.
- **Stage 10 준비 작업은 가능하지만 regression gate 통과는 보류.** B001을 수정·재검증하고, GUI pixel/DPI/keyboard 및 취소·재시작·장시간 stress 미검증을 보완해야 한다.
- 이번 단계는 discovery 요청에 따라 production/분석 코드를 수정하지 않았다. 권장 수정안을 아래에 기록했다.

## 환경과 시작 working tree

- .NET SDK 10.0.400, runtime 10.0.11, Windows NT 10.0.26200, win-x64.
- WPF net10.0-windows, Debug/Release, assembly/file version 1.0.0.0.
- Active dataset: packaged baseline, patch 16.17, revision 1788854099077.
- Accepted 51,649 / observations 258,245 / matchup keys 15,547.
- LocalAppData에 설치된 update manifest 없음. 실제 Release Settings에서 `기본 제공` 확인.
- 시작 시 tracked modified 18개와 untracked status entry 5개(개별 file 4개 및 `docs/` directory)가 있었다. 기존 변경은 reset/revert/delete하지 않았다.
- Modified: `.gitignore`, packaged statistics JSON 2개, presentation model, update manifest model, PROJECT_STATUS, DebugValidator, Presenter, update options/service, StatisticsDataService, Publisher Program/csproj, Collector output JSON, Portrait/Settings XAML 및 code-behind, StatisticsViewResources.
- Untracked: CompactDirectionIndicatorPresentation, CloudflarePagesDeployment, RemoteDatasetVerifier, UpdatePipelineSelfTest, `docs/`. Git status의 directory 표기를 파일 전체 목록으로 오해하지 않는다.
- 이번 작업의 repository 편집: 이 보고서 추가와 PROJECT_STATUS에 검증 결과 추가. 패키지 검증 명령은 ignored deployment package를 재생성했으며, LocalAppData settings 테스트 영향은 아래에 별도로 기록했다.

## 방법과 증거의 범위

- computer-use/CUA 도구는 `failed to start Node runtime ... os error 3`로 시작하지 못했다.
- Windows UI Automation을 통해 실제 실행 중인 WPF control을 읽고 Invoke/SelectionItem/Window/Transform 패턴으로 조작했다. Drawer는 실제 mouse click도 사용했다.
- UIA control tree/rectangle은 text와 접근성/상태의 증거이며, screenshot 기반 색상·icon rendering·pixel clipping 증거는 아니다.
- 잘못 선택한 Invoke 패턴, 잘못된 Back 이름 및 PowerShell 구문 오류는 자동화 도구 오류로 분리했다. 수정된 절차로 재검증한 결과만 사용했다.
- updater 3회 빠른 호출 중 2회 `ElementNotEnabledException`은 UIA가 비활성 버튼을 호출한 결과다. 앱 예외가 아니며 중복 동작 방지의 증거다.
- Debug validator의 기존 synthetic fixture가 startup에서 실행되었고 앱이 UI에 도달했다. 전체 Debug trace를 영구 수집한 것은 아니다.
- 별도 screenshot/log 파일은 만들지 않았다. 아래 로그 excerpt는 이번 tool output에서 옮긴 최소 근거다.
- seed/Match 수집, 실제 게임 실행, production 원격 artifact 변경은 하지 않았다.

## 실제 실행 결과

### Build

| 대상 | Warning | Error |
|---|---:|---:|
| LoLMatchup Debug | 0 | 0 |
| LoLMatchup Release | 0 | 0 |
| StatsCollector | 0 | 0 |
| PublishStatisticsDataset | 0 | 0 |
| AnalyzeStatisticsDataset | 0 | 0 |

Build 성공과 Analyzer 실행 실패(B001)는 서로 다른 결과다.

### 실제 UI

- Debug/Release `dotnet run` startup 15초 이상 유지. 후속 UIA 조작에서 응답 확인.
- Home: WAITING, 패치 16.17, 51,649경기 적용.
- Release: TEST control 비노출. Settings Data에서 정상 / 16.17 / 51,649경기 / 마스터 이상(시드 기반) / 15분 / 기본 제공 / revision / 2026-09-08 16:54 확인.
- Release 업데이트 확인: `최신 데이터입니다.`, 버튼 `최신 상태`. 중복 클릭은 busy 상태에서 차단.
- Normal5v5: 5개 LoadedStatistics. Summary `판정 5/5 · 아군 0 · 적군 0 · 중립 5`.
- TOP1v1: TOP만 LoadedStatistics, 나머지 Waiting. Summary 1/5 판정 보류.
- Partial: MID Waiting, 나머지 4개 LoadedStatistics.
- NotFound: TOP MatchupNotFound, 나머지 Waiting. Summary 0/5 판정 보류.
- Stop: 1초 후 일시적 LIVE 잔류, 3/6/10초에 WAITING 및 기존 역할 카드 제거. 2초 polling과 local 연결 소요를 고려해야 한다.
- Portrait 440×820 실제 resize 확인. 5개 role bounds가 존재하고 offscreen=False; 하단 clipping은 별도 육안 검증 필요.
- Landscape의 Win/L2/L3/L6/Solo/Unknown text 확인. 처음 900×600 resize 요청은 이전 Portrait MinHeight로 900×760에 clamp되었다. 정확한 900×600 검증은 미완료.
- 20회 Resize 후 생존·응답. 모든 회차의 layout mode를 assertion한 것은 아니다.
- 5 Role 각각 Detail 제목, 5 metric, 3 explanation heading 확인. Detail→Back 20회 성공.
- Home↔InGame mouse sequence 10회 성공, drawer open/close 20회 성공. Settings 포함 navigation full stress는 미완료.
- TEMP working directory에서 Release 실행해 51,649경기 정상 로드.
- 두 Release instance 동시 실행·종료 성공, settings JSON은 schema1로 유효.

### Dataset / updater / hosting

- 전체 15,547 keys: L2/L3/L6 count invariant 위반 0, invalid role 0, invalid English champion identifier 0.
- Solo event count가 sampleSize보다 큰 key 4,858개도 유효 데이터로 처리.
- Role keys: TOP 4,646 / JUNGLE 2,302 / MID 3,792 / ADC 1,794 / SUPPORT 3,013.
- 17개 UpdatePipelineSelfTest scenario 전부 PASS: endpoint precedence, HTTPS 거부, latest, immutable conflict, newer/install, loopback install, SHA/size/JSON/dataset/metadata 오류, unsafe dataset URI, downgrade, network, interrupted download, atomic rollback, fallback.
- 원격 manifest와 dataset GET 200, application/json.
- Manifest Cache-Control: `no-cache, max-age=0, must-revalidate`.
- Dataset Cache-Control: `public, max-age=31536000, immutable`.
- Dataset bytes: 7,541,418.
- SHA256: `1aba43ed6108d5b2ba07b1ce170f546f8417d1c319f6a6459d97c077bb27fb14`.
- prepare-cloudflare-pages 결과 3개 파일: _headers / manifest / immutable dataset. allowlist·size·SHA·privacy scan 통과.
- Public newer revision 설치·재시작 활성화는 미검증. 현재 production revision과 baseline이 같아서 actual UI는 UpToDate다.
- Outcome 전체 결과: Ally 307 / Neutral 2,010 / Enemy 342 / Insufficient 12,888; researchMatch=True, reverseSymmetry=True.
- Level: 1,115 / 384 / 1,160 / 12,888; Combat: 804 / 1,123 / 732 / 12,888. reverseSymmetry=True이나 Analyzer 연구 대표 사례 assertion 실패.

### 자원 및 종료

| 관찰 시점 | CPU 누적 seconds | Working Set MiB | Handles | Threads | Responding |
|---|---:|---:|---:|---:|---|
| startup 약 2초 | 1.78 | 186.3 | 637 | 77 | True |
| 중간 표본 | 2.65625 | 188.6 | 640 | 73 | True |
| 중간 표본 | 2.875 | 189.4 | 631 | 69 | True |
| 9.73분 | 3.53125 | 196.82 | 638 | 70 | True |

초기 대비 약 10.5MiB 증가, handles는 비슷한 범위였다. 명백한 CPU runaway/handle explosion은 관찰하지 않았다. 이 표만으로 memory leak 부재를 증명하지 않는다. 10분 스크립트 종료 후 최종 출력이 유실되어 10~20분 연속 관찰 요구는 PARTIAL이다. 최종 LoLMatchup process count는 0.

Debug 10회 restart를 시도했지만 최초 5회 결과 출력이 유실되었다. Debug 6~10회와 Release 1~5회는 각 5초 후 alive=True/responding=True를 확보했다. 총 15회를 모두 증거 있는 PASS로 세지 않았다.

## 버그 목록

### B001 — Analyzer가 최신 dataset의 정상 분석을 차단

- Severity: Medium. Priority: P1 (Stage 10 전 수정). 상태: OPEN.
- Feature: AnalyzeStatisticsDataset / ProductionLaneIndicatorsVerification.
- Environment: 위 packaged dataset 15,547 keys; 도구 build는 0 warning/0 error.
- Preconditions: 현재 production dataset 사용.
- Reproduction:
  1. `dotnet run --project .\Tools\AnalyzeStatisticsDataset\AnalyzeStatisticsDataset.csproj -- .\Data\Statistics\master_plus.json` 실행.
  2. Outcome 검증 후 Level/Combat 대표 사례 검증 실패 확인.
- Expected: 현재 데이터로 research/production 수식을 대조하고 reverse invariant를 검증하며 최신 분석 생성.
- Actual: `representativeCases=False` 때문에 exit 1.
- Reproduction rate: 올바른 명령 1/1. 고정 데이터·고정 assertion의 결정적 실패.
- Suspected file: `Tools/AnalyzeStatisticsDataset/ProductionLaneIndicatorsVerification.cs`.
- Log evidence:
  `Production Level Tempo / Combat Pressure regression failed: researchMatch=True, reverseSymmetry=True, representativeCases=False; level=1115/384/1160/12888, combat=804/1123/732/12888.`
- Root cause: Kled/Nasus, Gnar/Kled, Ezreal/Jhin, Jinx/Kaisa, Kaisa/Kalista, Jayce/KSante의 과거 실제 classification을 현재 모든 dataset에도 요구한다. 원본 count가 바뀌면 정상적으로 달라질 수 있다.
- 추가 위험: `entries.Count == 13_021`만으로 옛 snapshot을 식별하는 exact distribution guard도 revision identity가 아니다. 다른 13,021-key dataset에 오탐 가능.
- Workaround: WPF 실행과 updater는 계속 사용 가능. 공유 validator/reverse 검증의 확보 결과는 사용할 수 있으나 Analyzer 전체 완료를 주장할 수 없다.
- Recommended fix: 기본 분석 경로에서는 같은 입력에 대한 research/production 계산 일치 및 reverse invariant만 필수화한다. 특정 snapshot exact cases는 명시적 snapshot verification + SHA/revision identity로 제한하거나 synthetic fixture로 옮긴다. 단순 key count로 snapshot을 판정하지 않는다.
- 이번 단계에서는 코드 변경 없이 원인·우선순위를 기록했다.

### B002 — Release DLL에 개발 테스트 구현이 포함됨

- Severity: Low. Priority: P3. 상태: OPEN.
- Feature: Release packaging.
- Preconditions: Release build.
- Reproduction:
  1. `dotnet build .\LoLMatchup.csproj -c Release`.
  2. DLL의 metadata/BAML 문자열 또는 assembly resource를 검사.
- Expected: 요청 #114 기준 개발 테스트 파일/구현이 Release 산출물에서 제외.
- Actual: DeveloperTestWindow BAML, DeveloperTestModeService, preset 관련 method/type가 DLL에 존재.
- Reproduction rate: inspected Release build 1/1.
- Suspected files: `LoLMatchup.csproj`, `Views/DeveloperTestWindow.xaml(.cs)`, `Services/Development/`, `Models/Development/`.
- Evidence: DLL에 `LoLMatchup.Views.DeveloperTestWindow`, `Developer Test Mode`, `CreateNormal5v5` 존재. MainWindow의 TEST는 Collapsed이고 실행 hook은 DEBUG 조건이라 Release UI에는 표시되지 않음.
- Credential exposure: 발견하지 않음. synthetic data가 포함된 것과 실제 Riot 식별자 노출은 구분.
- Workaround: 일반 Release UI 경로에서는 접근 불가.
- Recommended fix: 모든 참조 의존성을 확인한 후 Release build의 Compile/Page/Resource item에서 개발 전용 구현 제외. cosmetic UI 변경은 필요 없음.

## 사용자 데이터 보호와 테스트 한계

- Collector state, production JSON bytes 및 사용자 installed dataset을 삭제/변조하지 않았다.
- UI resize 후 정상 종료가 기존 RememberWindowSize 기능을 통해 사용자 settings를 바꾼 사실을 확인했다. **테스트 전 byte-for-byte backup을 확보하지 못한 절차상 한계가 있다.**
- 관찰된 초기 창 위치 (408,36), 크기 783×935, Portrait와 현재 toggle 상태를 복구했다. LandscapeWidth/Height는 기본 900×600으로 복원했으나 기존 사용자 값이 정확히 이 값이었는지는 증명할 수 없다.
- 최종 확인 settings: false TopMost, true RememberPosition/Size, (408,36), 783×935 Portrait, portrait783×935 / landscape900×600.
- 사용자에게 원래 가로 크기가 달랐다면 재설정이 필요할 수 있음을 알린다. settings 손상/유실은 발견하지 않았지만 완전 무변경이라고 주장하지 않는다.
- updater self-test는 임시 GUID directory를 사용하고 cleanup을 수행한다. 그 cleanup 예외는 self-test에서 삼켜지므로 모든 임시 경로 잔류 전수 검사는 별도다.
- 실제 게임, multi-monitor, DPI 변경, pixel-level screenshots, keyboard 및 수백 회 UI fixture stress는 미검증 범위를 표에서 확인한다.

## 항목별 매트릭스

번호는 사용자 요청 번호다. PASS는 오른쪽에 적은 실제 확인 범위에 한한다. PARTIAL 항목의 미실행 부분은 수동/추가 자동화 검증이 필요하다.

| 번호 | 결과 | 항목 | 근거·제한 |
|---|---|---|---|
| 5 | PASS | 환경 | SDK 10.0.400, Windows 10.0.26200, app 1.0.0.0, packaged revision 확인 |
| 6 | PASS | 5개 build | WPF Debug/Release 및 Collector/Publisher/Analyzer 모두 warning 0 / error 0 |
| 7 | PASS | Debug startup | dotnet run 15초 이상 생존, 후속 실제 UI 조작 성공 |
| 8 | PASS | Release startup | dotnet run 17초 이상 생존, 후속 실제 Settings 조작 성공 |
| 9 | PASS | Release 개발 UI 비노출 | TEST control이 Release 접근성 트리에 없음; 산출물 포함 여부는 #114 별도 |
| 10 | PASS | Home | 제목·WAITING·패치·51,649경기·설정 진입 확인 |
| 11 | PARTIAL | Shell navigation | 실제 마우스로 Home/InGame 10회 수행; Settings 포함 전체 sequence와 메모리 비교 미완료 |
| 12 | PASS | Drawer | open/close 20회 성공, 응답 정상 |
| 13 | MANUAL VERIFICATION REQUIRED | General 설정 | 초기화·변경·재실행 전체 시나리오 미실행 |
| 14 | PARTIAL | Display 설정 | 코드 검토; 모든 toggle의 실제 변경·복구 미실행 |
| 15 | PASS | Settings Data | patch/count/scope/duration/source/revision/time 실제 표시 확인 |
| 16 | PASS | Production update UI | Release Settings → Data → 업데이트 확인 → 최신 데이터입니다 |
| 17 | PASS | 업데이트 중복 클릭 | 첫 클릭 후 disabled; 후속 2회 UIA Invoke는 ElementNotEnabled, 앱 정상 |
| 18 | PASS | Production HTTP | manifest/dataset 실제 HTTPS GET 200 |
| 19 | PARTIAL | Network failure | isolated self-test 보존 확인; 실제 GUI 오류 표시 미검증 |
| 20 | PARTIAL | Invalid endpoint | override/unsafe URI fixture 통과; 404·unreachable GUI 개별 미검증 |
| 21 | PASS | Invalid local fallback | isolated packaged fallback self-test 통과 |
| 22 | PASS | Same revision conflict | same revision/different SHA 거부 fixture 통과 |
| 23 | PASS | Downgrade | older/no downgrade fixture 통과 |
| 24 | PARTIAL | Portrait 전체 | 실제 역할·이름·지표 text/UIA 확인; 픽셀 색상·아이콘 렌더 육안 미검증 |
| 25 | PASS | Portrait raw 회귀 | Text controls에 raw %/count 없음; XAML도 compact indicators만 바인딩 |
| 26 | PARTIAL | Portrait n<20 | DEBUG presentation fixture 통과; n<20 custom GUI 미실행 |
| 27 | PASS | Portrait n>=20 | 실제 5v5 각 카드 L2/L3/L6/솔로킬 chip 확인 |
| 28 | PARTIAL | Mixed direction | Aatrox 실데이터 L2/L3 적군, L6/솔로킬 아군 확인; synthetic GUI는 미실행 |
| 29 | PARTIAL | 좁은 Portrait | 440×820 실제 resize·role bounds 확인; 420×760/픽셀 clipping 미완료 |
| 30 | PASS | 5 roles 접근 | 440×820에서 5 role 모두 UIA offscreen=False; scroll fallback 존재 |
| 31 | PARTIAL | Landscape | 상세 5지표/Unknown 확인; resize 요청 900×600이 첫 전환에서 900×760으로 clamp됨 |
| 32 | PARTIAL | Orientation stress | 20회 Resize 응답 정상; requested dimensions clamp로 정확한 20회 orientation 상태 assertion 부족 |
| 33 | PASS | 5 Role Detail | 다섯 카드 각각 실제 Invoke, 올바른 Role 제목 확인 |
| 34 | PARTIAL | Detail content | 각 Role 5지표·3설명 확인; icons/픽셀 clipping 미검증 |
| 35 | PASS | Detail Back | 정확한 Back control로 20회 반복 성공 |
| 36 | PARTIAL | Card click target | 카드 버튼 Invoke 성공; icon/text/빈 영역 개별 좌표 클릭 미실행 |
| 37 | PASS | Normal5v5 | 5 LoadedStatistics, Team Summary 판정 5/5 중립 5 확인 |
| 38 | PASS | TOP1v1 | TOP LoadedStatistics, 나머지 Waiting, summary 판정 1/5 보류 |
| 39 | PASS | Partial | MID Waiting, 나머지 LoadedStatistics, 판정 4/5 |
| 40 | PASS | NotFound | TOP MatchupNotFound, 나머지 Waiting, 판정 0/5 |
| 41 | MANUAL VERIFICATION REQUIRED | Custom fixture | picker controls 확인; custom 입력 변경 실행 미완료 |
| 42 | MANUAL VERIFICATION REQUIRED | Start/Stop 20회 | preset 순차 테스트만 수행; 20회 반복 미실행 |
| 43 | PASS | Test Stop | 별도 재검증에서 1초 LIVE 잔류 후 3/6/10초 WAITING·역할 cards 없음 |
| 44 | PASS | No-game | Home waiting 실제 확인, 앱 생존 |
| 45 | PARTIAL | Polling no-game | 10분 process smoke; request 수/debug exception spam 직접 계측 없음 |
| 46 | PASS | Game detected | normal5v5 Start 후 Home에서 InGame으로 이동 |
| 47 | PARTIAL | 사용자 navigation 존중 | newlyDetected 조건 코드 검토; active fixture에서 Home 대기 미실행 |
| 48 | PARTIAL | Game end | Stop→Waiting 확인; 실제 League 종료 및 Detail 활성 종료 조합 미검증 |
| 49 | PASS | Team Summary counts | 실제 5 neutral Role ↔ 판정 5/5·아군0·적군0·중립5 대조 |
| 50 | PASS | Summary insufficient | TOP1v1 판정1/5와 보류, NotFound0/5 보류 확인 |
| 51 | PARTIAL | Summary tie | DEBUG synthetic validator 통과; directional tie GUI 미실행 |
| 52 | PARTIAL | Outcome UI symmetry | 전체 데이터 classifier reverse 통과; 양방향 GUI 직접 비교 미실행 |
| 53 | PARTIAL | Level UI symmetry | 전체 데이터 reverseSymmetry=True; 양방향 GUI 미실행 |
| 54 | PARTIAL | Combat UI symmetry | 전체 데이터 reverseSymmetry=True; 양방향 GUI 미실행 |
| 55 | PASS | Explanation consistency | DEBUG fixtures 및 5개 Detail text가 상태와 일치 |
| 56 | PASS | Reliability boundaries | 1/4/5/19/20/49/50 DEBUG fixture 통과 |
| 57 | PASS | Outcome n19/n20 | DEBUG classifier fixture 통과 |
| 58 | PASS | Outcome 5pp | DEBUG 549/550/450 of 1000 boundary fixture 통과 |
| 59 | PASS | Wilson boundary | DEBUG interval 포함/제외/등호 fixture 통과 |
| 60 | PASS | Level 4.9/5.0/5.1 | Stage 9-6B synthetic 양방향 경계 통과 |
| 61 | PASS | Level signal count | Stage 9-6B 0/1/2/3 신호 및 mixed 방향 fixture 통과 |
| 62 | PASS | Combat .09/.10/.11 | Stage 9-6B 양방향 inclusive 경계 통과 |
| 63 | PASS | Level invariants | 15,547 keys 전체 L2/L3/L6 위반 0 |
| 64 | PASS | Solo EVENT_COUNT | 표본보다 큰 solo count 4,858 keys도 shared validation 통과 |
| 65 | PASS | Canonical reverse | 현재 dataset 전체 Outcome/Level/Combat reverse 검증 통과 |
| 66 | PARTIAL | Champion assets | 6개 ID 한국어·PNG 존재/크기 확인; 전부 실제 아이콘 픽셀 확인 아님 |
| 67 | PARTIAL | Champion fallback | DEBUG metadata fallback 검증 및 XAML ? fallback; 누락 icon GUI 미실행 |
| 68 | PASS | 한국어 metadata | LeeSin/리 신, Viego/비에고, MonkeyKing/오공, Kaisa/카이사, Nautilus/노틸러스, Thresh/쓰레쉬 |
| 69 | MANUAL VERIFICATION REQUIRED | Long name layout | 긴 이름의 픽셀 clipping 미검증 |
| 70 | PARTIAL | 위치 persistence | 동일 (408,36) 재실행 관찰; 이동→재실행 독립 검증 없음 |
| 71 | PASS | 크기 persistence | 실제 resize 후 재실행에 900×760 저장·복원 관찰; 사용자 설정 복원 주의사항 참조 |
| 72 | PARTIAL | Invalid position | WindowBoundsService 코드 검토; synthetic 설정 실행 없음 |
| 73 | MANUAL VERIFICATION REQUIRED | Pin/TopMost | 실제 toggle·재실행 테스트 미완료 |
| 74 | MANUAL VERIFICATION REQUIRED | Multi-monitor | 실제 모니터 전환/제거 미실행 |
| 75 | MANUAL VERIFICATION REQUIRED | DPI 100/125/150 | 시스템 DPI 변경 미실행 |
| 76 | PARTIAL | Rapid resize | 20회 Resize 생존·응답; 픽셀 layout loop 관찰 제한 |
| 77 | PARTIAL | Minimum size | XAML/동적 Min 크기 검토; 최소치 전 view 버튼 접근 미검증 |
| 78 | PARTIAL | Malformed settings | JsonException→defaults 코드 검토; temp fixture 실행 없음 |
| 79 | MANUAL VERIFICATION REQUIRED | Settings open/close30 | 여러 번 수동 자동화 진입했지만 30회 미실행 |
| 80 | PARTIAL | Settings close mid-check | cancellation/dispose/IsLoaded guard 검토; 실제 request 중 close 미실행 |
| 81 | PASS | Close no-game | 테스트 PID 정상 종료/잔류 확인 |
| 82 | PASS | Close Test Mode | active preset 앱 CloseMainWindow 종료·후속 process 확인 |
| 83 | PARTIAL | Restart stress | Debug10/Release5 시도; Debug 최초5 결과 출력 유실, Debug6~10/Release1~5는 전부 생존·응답 확인 |
| 84 | PASS | Process leak | 테스트 프로세스 정리 후 최종 잔류 확인 |
| 85 | PARTIAL | Memory smoke | 10분 script 실행; 마지막 확보 표본 9.73분. 종료 시 마지막 출력 유실, 10~20분 연속 관찰 미충족 |
| 86 | PASS | Idle CPU | 10분 누적 CPU 및 응답 상태 표 참조 |
| 87 | MANUAL VERIFICATION REQUIRED | Lookup 수백회 UI | 전체 dataset classifier 검증과 별개로 UI lookup stress 미실행 |
| 88 | MANUAL VERIFICATION REQUIRED | NotFound stress | 1회 preset 검증; 연속 반복 미실행 |
| 89 | MANUAL VERIFICATION REQUIRED | Partial stress | 1회 preset 검증; 연속 반복 미실행 |
| 90 | PARTIAL | Invalid synthetic | NotFound/partial preset과 DEBUG validation; custom duplicate role 직접 주입 없음 |
| 91 | MANUAL VERIFICATION REQUIRED | Keyboard | Tab/Enter/Space 실제 접근성 전수 미검증 |
| 92 | PASS | Color-only | 아군/적군/중립 문구가 actual Text controls에도 존재 |
| 93 | PARTIAL | Tooltip | XAML 설명 binding 확인; 실제 hover 렌더 미검증 |
| 94 | MANUAL VERIFICATION REQUIRED | 긴 한국어 clipping | UIA full text는 확인했으나 픽셀 clipping 판별 없음 |
| 95 | PASS | Datetime | Settings 2026-09-08 16:54 표시; ToLocalTime 코드 일치 |
| 96 | PARTIAL | Dataset source | 기본 제공 실제 확인; 업데이트됨 GUI는 미검증 |
| 97 | PASS | Empty local update | LocalAppData update manifest 없음, packaged 51,649 표시 |
| 98 | PARTIAL | Newer local fixture | isolated newer install bytes/companion 검증; 새 StatisticsDataService UpdatedLocal 선택 fixture는 없음 |
| 99 | PARTIAL | Invalid newer fallback | malformed local→packaged fixture 통과; newer companion revision을 둔 조합은 미실행 |
| 100 | PASS | Remote=current | actual Release GUI 최신 상태 |
| 101 | PASS | Remote older | NoDowngrade self-test |
| 102 | PARTIAL | Newer install workflow | loopback Check/Install fixture 통과; GUI RestartRequired 전 flow 미실행 |
| 103 | PARTIAL | Restart after install | isolated install bytes 검증 및 source-priority 코드 검토; WPF 재실행/새 service selection 직접 fixture 없음 |
| 104 | PARTIAL | Interrupted cleanup | interrupted download 기존 bytes 보존 통과; interruption 경로 cleanup 파일 개수는 직접 assertion 없음 |
| 105 | PASS | Atomic rollback | companion manifest replacement failure self-test 통과 |
| 106 | PASS | Cache headers | actual manifest no-cache / dataset immutable |
| 107 | PASS | Content-Type | actual 두 파일 application/json |
| 108 | PASS | Updater self-test | 17개 named scenarios 모두 PASS |
| 109 | PASS | Deployment package | prepare-cloudflare-pages 성공 |
| 110 | PASS | Allowlist | _headers/manifest/revision dataset, 총 3개 |
| 111 | PASS | Package SHA | local/remote SHA 동일; 7,541,418 bytes |
| 112 | PARTIAL | Secret scan | 배포 artifact scan 통과; repo pattern은 CancellationToken 등 false positives, Git history 전체 audit 아님 |
| 113 | PASS | Tracked sensitive paths | .env/State/Samples/logs/credentials/generated deployment tracked 후보 0 |
| 114 | PASS | Release output | Stage 9-6B Release 개발 타입/BAML 0, Debug 유지; 기존 기타 산출물 검증 유지 (B002 resolved) |
| 115 | PASS | Packaged baseline | Release Data/Statistics/master_plus.json 존재 및 실제 로드 |
| 116 | PARTIAL | Champion Release assets | PNG/metadata copy 확인; 모든 PNG runtime 렌더 전수 미검증 |
| 117 | PARTIAL | Exceptions | 앱 crash 미관찰; 모든 Debug stream 수집 안 함; PowerShell 자동화 오류는 별도 제외 |
| 118 | PARTIAL | Silent close | 실행한 startup/fixture/detail/settings flow 정상; 미실행 flow는 보장하지 않음 |
| 119 | PASS | Dataset-independent startup | 51,649 dataset Debug startup 성공, current exact count assertion 없음 |
| 120 | PASS | WPF hardcoding scan | 연구 snapshot exact count/empirical classification assertion 없음; Analyzer는 B001 |
| 121 | PASS | TODO/FIXME | C#/XAML TODO/FIXME/HACK/NotImplementedException 검색 0 |
| 122 | PARTIAL | Empty catch | self-test cleanup에서만 empty IOException/UnauthorizedAccessException; production rollback rethrows 확인 |
| 123 | PASS | async void | 발견된 5개 모두 WPF event handler |
| 124 | PARTIAL | Task loss scan | 명백한 fire-and-forget 미발견; 정식 analyzer/profiler 검증 아님 |
| 125 | PARTIAL | Cancellation | polling/Settings token·close 흐름 검토; mid-download GUI close 미실행 |
| 126 | PARTIAL | Dispose | timer/HTTP/stream/CTS 코드 검토 및 자원 smoke; 전 경로 leak proof 아님 |
| 127 | PARTIAL | UI thread | Dispatcher.InvokeAsync 및 UI await 흐름 검토, 실제 주요 flow 성공 |
| 128 | PASS | Path robustness | AppContext.BaseDirectory/LocalAppData 사용, production local absolute path 0 |
| 129 | PASS | Different cwd | TEMP 작업 디렉터리 Release 실행, 51,649경기 실제 표시 |
| 130 | MANUAL VERIFICATION REQUIRED | Missing packaged copy | source 미변경; 독립 test-copy missing-file 실행 없음 |
| 131 | MANUAL VERIFICATION REQUIRED | Read-only fixture | ACL/읽기전용 test path 미실행 |
| 132 | PARTIAL | Disk write failure | atomic manifest replacement failure fixture 확인; disk full/기타 쓰기 실패 미실행 |
| 133 | MANUAL VERIFICATION REQUIRED | Unicode path | isolated Unicode path 테스트 미실행 |
| 134 | MANUAL VERIFICATION REQUIRED | Long path | deep path 테스트 미실행 |
| 135 | MANUAL VERIFICATION REQUIRED | Check+navigation race | request 중 navigation/reopen 반복 미실행 |
| 136 | MANUAL VERIFICATION REQUIRED | App close mid-download | interrupted stream self-test만 있음; 실제 WPF close flow 미실행 |
| 137 | PASS | Multiple instances | Release 2개 동시 실행 생존/응답·종료 확인 (동시 install 제외) |
| 138 | PARTIAL | Multi-instance settings | 2개 종료 후 JSON valid schema1; 반복 경쟁 조건 보장 아님 |
| 139 | PARTIAL | Offscreen recovery | virtual-desktop 교차검증 코드 확인; multi-monitor gap/removal 실행 없음 |
| 140 | PASS | Role order | 정상/partial 실제 UI 및 shared position list TOP/JUNGLE/MID/ADC/SUPPORT |
| 141 | PARTIAL | Side consistency | 실제 champion text/Detail source 일치; 색상/icon 픽셀 전수 미검증 |
| 142 | PASS | Detail orientation | 5개 실제 Role Detail context가 카드 query/opponent를 그대로 사용 |
| 143 | PASS | Reverse counts/classifiers | DEBUG raw reverse fixture·current dataset classifier symmetry 통과 |
| 144 | PASS | Summary source | Outcome-only classifier 코드/DEBUG fixture 및 실제 neutral5 UI 대조 |
| 145 | PASS | No fake team WR | Team Summary 실제 UI는 role vote/count, team 승률 값 없음 |
| 146 | PARTIAL | Deterministic explanation | 순수 함수/DEBUG expected phrase 검증; 동일 GUI 반복 text hash 비교 없음 |
| 147 | PASS | Missing explanation | NotFound/Waiting preset 가짜 결과 없음; HasStatistics guards 확인 |
| 148 | PARTIAL | Reliability consistency | shared presenter·DEBUG fixture; 모든 view 동일 fixture 육안 대조 미완료 |
| 149 | PARTIAL | Load-once update | cached LoadAsync 및 install 분리 self-test/code 검토; UI 열린 상태 실제 newer install 미실행 |
| A01 | PASS | 현재 dataset Analyzer | Stage 9-6B current/scaled valid input exit 0, malformed exit 1 (B001 resolved) |

## 다음 검증 순서

1. B001 수정 후 현재 dataset Analyzer 재실행. shared classifier production rule 변경 없이 검증을 분리한다.
2. 보호된 설정/데이터 test profile에서 420×760, 정확한 900×600, pixel clipping/icon/contrast, keyboard, multi-monitor/DPI를 확인한다.
3. Settings30회, Test Start/Stop20회, lookup 수백회, request 중 close/reopen 및 slow download 중 종료를 검증한다.
4. temp 경로 malformed/offscreen settings, read-only/Unicode/deep paths, missing packaged files, install 후 실제 WPF restart selection을 보완한다.
5. 다음 valid public revision으로 newer download/install/restart activation을 검증한다.
6. 10~20분 리소스 test는 끝까지 로그를 확보한다. Stage 10 gate를 그 결과와 함께 다시 판단한다.


## Stage 9-6D — Portrait Priority Readability Polish (2026-09-10)

UI 리뷰에서 발견한 Portrait 핵심 판정 가독성 P1을 최소 범위로 수정했다. 기존 Stage 9-6/9-6B 결과는 이력으로 보존하며 전체 회귀 매트릭스나 release gate를 일괄 승격하지 않는다.

- 수정 파일: Views/PortraitStatisticsView.xaml, Views/StatisticsViewResources.xaml 및 이 보고서/PROJECT_STATUS.md. Git diff에는 이전 Stage의 dirty 변경도 포함되므로 전체 diff를 이번 작업량으로 해석하지 않는다.
- 세 판정 value 9→13 DIP, SemiBold 유지, label 10 DIP muted/중앙 정렬, 공통 행 높이 18 DIP. 기존 38 DIP label column과 남은 폭 value column 유지. 판정문 NoWrap/Trimming=None.
- 카드 padding 상하 6→4, 챔피언 영역 margin 상하 3→2. 아이콘/이름/VS 구조, 표본 및 신뢰도 badge 유지. 실제 카드 높이 약 118→130px, 다섯 카드 약 60px 증가. 작은 창 하단은 스크롤로 접근한다.
- chip 글씨는 9 DIP 그대로이며 border만 neutral #35404E / ally #486570 / enemy #60576F로 낮췄다. 텍스트의 기존 cyan/purple/gray 및 WrapPanel 유지. 새 계산·converter·property·문구·tooltip 없음.

| 검증 | 결과 및 한계 |
|---|---|
| Debug build | 최종 dotnet build .\LoLMatchup.csproj: warning 0/error 0 |
| Release build | 최종 dotnet build .\LoLMatchup.csproj -c Release: warning 0/error 0 |
| Debug runtime | 빌드 후 --no-build 실행, 실제 normal5v5 렌더 확인 |
| 작은 Portrait | 420×760/440×820 목표 조작; 도구 캡처 406×753/426×813. OS 외곽 크기 별도 계측 없음 |
| 긴 문구/역할 접근 | 뚜렷한 차이 없음, 아군/적군 쪽 경향, 상대적으로 충분 잘림 없음. 수평 overflow 없음. 스크롤 하단 SUPPORT와 chip 확인 |
| chip wrapping | WrapPanel 유지. 관찰한 최소 폭의 4개 chip은 한 줄에 들어와 실제 줄바꿈 발생은 미관찰 |
| normal5v5 | 핵심 세 판정이 chip보다 크게 읽힘. 다섯 역할 및 Team Summary 판정 5/5 유지 |
| Partial | MID 감지 중, 나머지 판정 유지. MID의 이전 판정/chip 잔류 없음 |
| NotFound | 통계 없음/감지 중 표시, 가짜 판정/chip 잔류 없음 |
| n<20 | Custom picker 자동화가 입력을 반영하지 않아 실제 화면 미확인. 기존 정책/표시 guard는 변경 없음 |
| Navigation | TOP 카드→Matchup Detail→인게임 복귀 확인 |
| Landscape | 최대화 raw 수치 UI 유지, Portrait→Landscape→Portrait 새 스타일 복원 확인 |
| Start/Stop | 완료한 5쌍: normal5v5, Partial, NotFound, TOP1v1, normal5v5. 각 Stop 후 WAITING 확인. 여러 실행 세션에 걸친 결과이며 입력 미반영 시도/중단된 1회는 집계 제외 |
| Release startup | Home, WAITING, patch 16.17, 51,649경기 표시 및 TEST 비노출 확인 |
| Scope/diff | LandscapeStatisticsView/MatchupDetailView 변경 없음. git diff --check exit 0 (줄바꿈 안내만 존재) |

판정: **Portrait readability P1 = FIXED**, 실제 관찰한 기본 DPI·대표 fixture 범위에서 핵심 값의 크기/간격/보조 chip 대비가 개선됐다. 한 역할의 세 판단을 빠르게 읽기 쉬워졌다는 렌더 기반 평가이며, 3초 사용자 실험 또는 모든 DPI 통과를 의미하지 않는다.

검증 스크린샷은 tool output으로만 확인했다. 기존 사용자 설정 자동 저장을 피하도록 검증용 프로세스를 종료했으며, 설정 옵션·dataset·배포물은 편집하지 않았다. Settings contrast, Data metadata hierarchy, drawer/scrollbar, Detail affordance, onboarding, tooltip, wording 등 P2/P3는 이번 변경에 포함하지 않았다.

## Stage 10-A — Release Readiness Foundation (2026-09-10)

시작: `main` / `aa35d13`, working tree clean. 기존 Stage 9 보고서는 보존한다. 아래 PASS는 명시된 범위의 관찰 결과이며 이전 수동 항목 전체를 승격하지 않는다.

| 검증 | 결과 | 관찰/제한 |
|---|---|---|
| WPF Debug build | PASS | 최종 `dotnet build .\LoLMatchup.csproj`: warning 0/error 0 |
| WPF Release build | PASS | 최종 `dotnet build .\LoLMatchup.csproj -c Release`: warning 0/error 0 |
| StatsCollector build | PASS | warning 0/error 0 |
| PublishStatisticsDataset build | PASS | warning 0/error 0 |
| AnalyzeStatisticsDataset build | PASS | warning 0/error 0 |
| Diagnostics regression | PASS | `Tools/RegressionScratch/DiagnosticsRegression.cs`, temporary directory only |
| Patch normalization | PASS | 16.17.1 / 16.17 / 16.17.123.456 match; 16.18.0 mismatch; null/invalid component/invalid part count Unknown |
| Common snapshot | PASS | packaged/local sources, revision 7→8, missing generated time, immutable old snapshot, reset to Unknown |
| Logging concurrency/level | PASS | 500 parallel writes, 500 complete unique lines; Info threshold excludes Debug |
| Privacy and bounded crash | PASS | synthetic secret/identity/path/query markers and 100,000-character messages omitted; outer+2 inner preserved, null stack safe, depth 12 bounded |
| Logging failure | PASS | temp regular file blocks log-directory creation; Log/LogSnapshot/Cleanup do not throw; WriteCrash returns null |
| Retention | PASS | 31-day managed fake file removed, current daily file retained, unrelated old keep.log untouched, managed max count 100 |
| Invalid dataset fallback | PASS | malformed local warns and loads packaged; both-invalid returns failure for app fatal startup guard |
| Optional champion resources | PARTIAL | existing metadata fallback validator passes; missing icon behavior/code reviewed, no new full missing-icon GUI exercise |
| Statistics regression | PASS | existing DEBUG validators, all current dataset classifications/reverse invariants and 20 service fixture Start/Stop cycles |
| Updater regression | PASS | existing `--self-test`: all 19 named scenarios, including latest/conflict/newer/install/HTTPS/loopback/SHA/size/metadata/downgrade/network/interruption/cancel-dispose/atomic rollback/fallback |
| Production read-only | PASS | actual manifest and dataset GET, validation, size, SHA, metadata; no writes to production |
| Debug Home/Settings | PASS | actual Home patch/count/Waiting; Settings shared metadata and normal Unknown game patch |
| No-game 5 minutes | PASS | KST 02:40:31.249 Waiting → 02:45:35.119 observation (~304 sec), Waiting exactly 1, Error/Critical 0, crash files 0. 10 physical log lines included startup snapshot and 4 manual updater events |
| GUI transitions | PASS | normal5v5→Stop→Waiting three complete cycles. GameDetected UTC 17:45:50, 17:46:35, 17:47:11; Waiting 17:46:07, 17:46:53, 17:48:15. One event each, no polling spam |
| Synthetic mismatch GUI | PASS | Debug game 16.18 / dataset 16.17; Settings explicitly says existing statistics remain usable; 5-role dashboard and summary remain active; dataset files unchanged |
| Normal Debug shutdown | PASS | Shutdown UTC 17:48:43; no crash file, visible test process exited |
| Actual Dispatcher fatal | PASS | Debug UI crash button → Korean dialog + Critical FatalDispatcher + separate 1,807-byte crash with inner exception/method stack; acknowledge → process gone |
| Actual TaskScheduler fatal | PASS (one run) | faulted unobserved Task + GC → Critical FatalTask + 756-byte crash + Korean dialog; acknowledge → process gone. GC timing is not guaranteed across runtimes |
| AppDomain fatal | PARTIAL | handler registered and shared fatal path reviewed; no separate actual AppDomain crash injected |
| Release Home/Data | PASS | actual Home startup, no TEST/DEBUG/developer controls; Data patch/revision/generated/Unknown displayed |
| Release updater | PASS | actual Settings request shows 최신 데이터입니다 / 최신 상태; log CheckStarted/current/manifest revision/UpToDate |
| Release shutdown | PASS | Shutdown UTC 17:55:14; no remaining LoLMatchup process on final inspection; crash count stayed 2 (the deliberate Debug fixtures) |
| Actual log privacy scan | PASS | all generated local log files: API-key/header/player-ID/name/user-profile/match-ID patterns 0; no actual log copied into repo |
| Release PE/resources | PASS | Debug developer types 5/BAML 2/diagnostic fixture methods 4; Release 0/0/0 |
| Async/cancellation | PARTIAL | production async entrypoints awaited WPF events; no production fire-and-forget. Closing guards and polling cancellation reviewed; existing updater cancel/dispose tests pass. GUI slow-download/close stress not newly executed |
| Actual game patch | MANUAL/BLOCKED | no reliable version in existing calls; official sample gameData has no version. No guessed version or API key path added |
| Git hygiene | PASS | git diff --check; existing *.log ignore sufficient; no tracked/generated local logs or production dataset changes |

Production response observed during this run:

- Revision `1788854099077`, patch `16.17`, accepted matches `51,649`, observations `258,245`, matchup keys `15,547`.
- Dataset `master_plus.1788854099077.json`, `7,541,418` bytes.
- SHA-256 `1aba43ed6108d5b2ba07b1ce170f546f8417d1c319f6a6459d97c077bb27fb14`.
- Both actual Debug and Release updater checks returned UpToDate; revision was read from the response, not assumed from the Stage 9 report.

Execution notes: an initial sandboxed/hidden `dotnet run` could not supply the normal desktop/log evidence and was excluded from smoke results; its remaining test process was stopped. Initial network-limited regression failed the public endpoint check, then completed successfully with read-only network access. During cleanup, one stale method signature and an executable lock caused intermediate build failures; both were resolved and final builds passed 0/0. UI automation occasionally returned stale indices/geometry; only visibly completed actions and logged transitions were counted. Desktop smoke used the actual built app executable through computer-use.

Known limits: original exception messages are intentionally omitted for privacy; filename/line numbers are not stored. Logging is best effort under disk/permission/multiprocess contention, and process kill/stack overflow/power loss may not yield a crash record. Real ACL/disk-full, separate AppDomain/missing-resource startup crash, every DPI and existing Stage 9 manual cases remain unverified. No new unresolved P0/P1 found; no unrelated P2/P3 polish was included.

Stage 10-A foundation: **COMPLETE within this scope**. Stage 10-B planning can proceed. This is not a blanket public-release gate approval and does not claim actual runtime game patch acquisition.

## Stage 10-B — Product UX regression (2026-09-10)

Start: `main`, HEAD `aa35d13`. Stage 10-A dirty files were retained; no reset/revert/delete/commit. Reviewed PROJECT_STATUS, this report, diagnostics-and-logging, and Stage 9 P2 feedback. No statistics policy/schema/production deployment/dependency changes.

| Check | Result | Evidence / limits |
|---|---|---|
| P2-1 Detail affordance | PASS | Small `›` only for CanOpenDetail; disabled missing role has no arrow; native Button command, Hand, hover border and gold keyboard focus. TOP entered with Enter |
| P2-2 Settings contrast | PASS | Explicit section text inheritance fixes dark title; readable secondary descriptions; dark list items, toggles and expanders; reset confirmation retained |
| P2-3 Data hierarchy | PASS | Normal dataset separate from unchecked/latest/update error; patch and accepted count visible; collection/technical details collapsed; About version 1.0.0 without build internals |
| P2-4 Drawer | PASS | Dark translucent backdrop stays dark; outside click closes; Space opens, Enter selects Home, focus returns to menu; Tab cycle retained |
| P2-5 Scrollbar | PASS | Native ScrollViewer/Track/Thumb, 16 DIP track, visible normal/hover/drag states. Portrait and Settings actual thumb drags scroll content; Portrait wheel observed |
| P2-6 States | PASS | Home no-game and active; Waiting normal action guidance; Partial MID says role information being checked; NotFound explains uncollected combination; mismatch remains usable |
| P2-7 Meaning | PASS | Outcome/early/combat labels and L2/L3/L6/solo chips have short explanations; dark tooltip actual render; sample reliability thresholds unchanged |
| Portrait 420×760 | PASS | Exact Debug viewport preset plus prior border resize. Tool capture about 406×753 (nonclient crop); core values/chips readable; no horizontal overflow; SUPPORT reachable by scrolling |
| Portrait 440×820 | PASS | Debug viewport preset, capture about 426×813; 13 DIP values and secondary chips preserved, 5 roles accessible by scrolling; final build rechecked |
| Landscape 760×420 | PASS | Two columns, normal cards and scrolling. Small height intentionally requires vertical scrolling |
| Landscape 1280×800 | PASS | Three columns, constrained content width, ally/enemy headings, full solo metric via wrap. NotFound synthetic long names prompted bounded text width fix |
| Detail TOP/MID | PASS | Both opened; Korean names and stronger wrapped explanations, no visible internal ID. MID bottom shows one total per side and per-game solo values |
| Patch Unknown | PASS | Release no-game Data: current connection cannot confirm game patch, statistics remain accessible. No unsupported promise to acquire patch after game start |
| Patch mismatch | PASS | Debug synthetic 16.18 vs dataset 16.17; nonblocking Korean explanation; active data/count retained |
| Updater failure UI | PASS | Debug instance-only http://127.0.0.1:9/manifest.json, actual update check failure; existing-data/retry guidance and enabled check button; production URI untouched |
| Release production update | PASS | Actual Settings check returned `최신 데이터입니다`, button `다시 확인`; GET-only, no download/install/deploy |
| Keyboard | PASS, bounded | Tab and Shift+Tab main navigation, Space drawer, Enter Home/InGame and TOP card; visible card focus. Owned Settings keyboard paths not exhaustively exercised because tool activates owner |
| Mouse/theme | PASS | Role card hover border, shell button feedback, dark drawer/nav selection, dark wrapping tooltip; no red/green result palette |
| Debug smoke | PASS | Actual built executable: Home, Portrait/Landscape, TOP/MID Detail, General/Display/Data and Developer Test. Viewport/failure fixtures remain Debug-only |
| Release smoke | PASS | Actual built executable: Home, Waiting, Settings General/Data/About, Unknown, production UpToDate; no TEST UI |
| WPF Debug build | PASS | dotnet build LoLMatchup.csproj: 0 warnings / 0 errors |
| WPF Release build | PASS | dotnet build LoLMatchup.csproj -c Release: 0 / 0 |
| StatsCollector build | PASS | dotnet build Tools/StatsCollector/StatsCollector.csproj: 0 / 0 |
| Publisher build | PASS | dotnet build Tools/PublishStatisticsDataset/PublishStatisticsDataset.csproj: 0 / 0 |
| Analyzer build | PASS | dotnet build Tools/AnalyzeStatisticsDataset/AnalyzeStatisticsDataset.csproj: 0 / 0 |
| Logging regression | PASS | UTF-8/concurrent 500 writes, exception privacy/depth, log I/O failure isolation, 30-day/100-file retention, dataset fallback, immutable diagnostics and patch cases |
| Statistics regression | PASS | Existing Debug validators, classifier/reliability/summary boundaries and reverse direction; 20 start/stop cycles/40 notifications; Partial/NotFound/custom/off-meta pipelines |
| Release artifacts | PASS | PE metadata/resource scan: Debug 5 developer types/2 BAML/7 fixture methods; Release 0/0/0 (includes viewport/update failure/crash/patch methods) |
| Privacy | PASS | UI/resources/Logging/Diagnostics and real runtime logs: RIOT_API_KEY, RGAPI-, PUUID, RiotId, summonerName, Authorization markers 0. No raw exception or HTTP/player payload logging added |
| Git | PASS | diff --check; no screenshots/logs/dataset modifications added to repo; Stage 10-A changes preserved |

Execution notes: desktop smoke launched built EXEs through computer-use (not a claim that a hidden sandbox `dotnet run` supplied desktop evidence). First regression run completed local cases but failed the network-restricted public endpoint check; read-only elevated rerun exited 0 and passed all cases including UpToDate and artifact scans. Failed/stale/out-of-bounds UI automation attempts were reobserved and not counted as successful tests. Viewport presets were added to avoid claiming exact dimensions from imprecise drags. Crash fixtures were not deliberately rerun in Stage 10-B; separation and logger tests cover the quick regression requirement. Original settings were restored byte-for-byte after verifying that only viewport geometry had changed and all LoLMatchup processes had exited. Final normal Shutdown was logged.

Product decision: Home provides persistent, dismiss-free first-use guidance, so no separate onboarding modal or boolean was added. No separate UX document was necessary. Stage 9 identity and Portrait P1 hierarchy remain. P2-1 through P2-7 addressed; no new unresolved P0/P1 or distinct blocking P2/P3 found in the observed scope. Remaining manual coverage: real League start/end, real game patch acquisition, all DPI/screen readers, every long localized name, exhaustive owned-window keyboard paths. This is not a public distribution/signing/installer approval.

Stage 10-B: **COMPLETE within the requested Product UX scope**. Stage 10-C can proceed.

Stage 10-B file list (21; additional working-tree files belong to preserved Stage 10-A work):

- MainWindow.xaml
- MainWindow.xaml.cs
- Services/MatchupStatisticsPresenter.cs
- Services/Diagnostics/AppDiagnosticsService.cs
- Tools/RegressionScratch/Program.cs
- Views/AppShellResources.xaml
- Views/StatisticsViewResources.xaml
- Views/DeveloperTestWindow.xaml
- Views/DeveloperTestWindow.xaml.cs
- Views/HomeView.xaml
- Views/HomeView.xaml.cs
- Views/InGameView.xaml
- Views/InGameView.xaml.cs
- Views/LandscapeStatisticsView.xaml
- Views/LandscapeStatisticsView.xaml.cs
- Views/MatchupDetailView.xaml
- Views/PortraitStatisticsView.xaml
- Views/SettingsWindow.xaml
- Views/SettingsWindow.xaml.cs
- PROJECT_STATUS.md
- docs/regression-test-report.md

## Stage 10-C — Distribution & Release Packaging (2026-09-11)

Start: `main`, HEAD `aa35d13`. Stage 10-A/B dirty files were retained; no reset, revert, checkout overwrite, delete, clean, commit, production deployment, or statistics policy/schema change.

| Check | Result | Evidence / limits |
|---|---|---|
| Five requested builds | PASS | WPF Debug/Release, StatsCollector, PublishStatisticsDataset, AnalyzeStatisticsDataset: each warning 0/error 0 |
| Release profile | PASS | `win-x64`, self-contained, multi-file, no trimming, no ReadyToRun, Release PDB disabled |
| Fresh publish | PASS | `artifacts/publish/win-x64`, 575 files, 158,844,328 bytes (151.49 MiB) |
| Required content | PASS | `LoLMatchInfo.exe`; baseline 7,541,418 bytes; statistics manifest; champion metadata; 173 PNG icons |
| Forbidden content | PASS | no source/PDB/.env/tool executable; binary scan found no DeveloperTestWindow, RGAPI-, or RIOT_API_KEY |
| Privacy/path scan | PASS | text artifact scan found no Authorization header, PUUID, riotId, summonerName, matchId, or `C:\Users\<developer>` path |
| Executable metadata | PASS | Product/File description `LoL Match Info`; FileVersion `0.9.0.0`; ProductVersion `0.9.0`; filename `LoLMatchInfo.exe` |
| Portable archive | PASS | 575 entries, 68,291,025 bytes (65.13 MiB), no separate portable data mode |
| SHA-256 | PASS | `edca2108c7ef4ef9e388c0ea5b31d2fe60d07ce97fba91bb88a0c798c6422b7d`, filename and size recorded |
| App release manifest | PASS | schema 1, channel closed-beta, RID, version, type/file/hash/size; URL intentionally null; separate from dataset manifest |
| Arbitrary working directory startup | PASS | publish executable launched with Temp as CWD, remained alive after 5 and 8 seconds, title `LoL Match Info`, closed normally |
| Existing user data/logging | PASS | existing LocalAppData was not deleted/reset; Release log recorded Startup, diagnostics 0.9.0, packaged dataset, Waiting, Shutdown |
| Install-directory write | PASS | second startup snapshot found 0 created/size/timestamp changes under publish directory |
| Production stats updater | PASS | existing regression performed read-only public check and returned UpToDate; no deploy/download mutation |
| Statistics/logging regression | PASS | diagnostics/logger/privacy/fallback/classifier/lookup/20-cycle/partial/notfound/custom/off-meta suite exited 0 |
| Release development separation | PASS | developer types 0, developer BAML 0, diagnostic fixture methods 0 |
| Installer source | PASS | Inno Setup source defines per-user install, shortcuts, uninstall metadata, close-app behavior, stable AppId and user-data preservation |
| Installer build | BLOCKED | Inno Setup 6 `ISCC.exe` not installed; script reported blocker and did not claim an installer |
| Installer/install/uninstall/reinstall/upgrade | BLOCKED | no installer binary; no lifecycle claim. Downgrade prevention remains release-process policy rather than code enforcement |
| Clean first run | MANUAL | avoided deleting or redirecting the real user profile; isolated known-folder UI test remains |
| Screen smoke | MANUAL | native app automation API unavailable; process/title/log startup verified, but Home/Waiting/Settings/Data/About were not visually rechecked in this Stage |
| Offline/failure | PARTIAL | packaged baseline startup and existing recoverable-failure regression passed; system network was not disabled and installed-build offline UI was not exercised |
| Code signing/icon | MANUAL | `UNSIGNED BETA`, no certificate/signtool and no final `.ico`; targets and safe signing insertion points documented |
| Defender/SmartScreen | MANUAL | security settings were not changed; unsigned warning policy documented, quarantine/UI not automated |
| Git hygiene | PASS | `artifacts/` ignored; source/profile/scripts/docs remain trackable; `git diff --check` clean apart from line-ending notices |

The first self-contained restore was blocked by sandbox network and then completed with approved NuGet access. Two script compatibility defects (XML version selection and Windows PowerShell 5.1 string comparison) were found during real execution and fixed before the final artifact run. Renaming the assembly exposed two RegressionScratch hard-coded DLL paths; both were updated and the full harness then exited 0.

New unresolved issue count: P0 0 / P1 0 / P2 0 / P3 0. Installer tool absence and actual lifecycle/GUI checks are tracked as BLOCKED/MANUAL rather than defects. Stage 10-C is **PARTIAL** until Inno Setup builds the installer and isolated install → run → update/log → uninstall/reinstall/upgrade smoke is completed.

## Stage 10-C2 — Inno Setup Detection Fix and Installer Lifecycle (2026-09-11)

This section supersedes the Stage 10-C installer blocker above. Start remained `main` at `aa35d13`; all Stage 10-A/B/C dirty files and the newly observed untracked collector 16.18 output were preserved.

| Check | Result | Evidence / limits |
|---|---|---|
| Per-user ISCC detection | PASS | `%LocalAppData%\Programs\Inno Setup 6\ISCC.exe`, compiler 6.7.3; Program Files x86/64 and PATH fallbacks retained; no username in source |
| Final release pipeline | PASS | publish validation, portable ZIP, Inno compile, checksums and release manifest all completed |
| Final installer | PASS | `LoLMatchInfo-0.9.0-win-x64-setup.exe`; 51,627,661 bytes; SHA-256 `905bc70aa386a9dea7644dc023c29996083b35ffb816bf392224ab187192c4e1` |
| Checksum/manifest | PASS | installer and portable present in `SHA256SUMS.txt`; manifest installer filename/version/size/hash matched actual file, URL null |
| Install privilege/location | PASS | silent install exit 0; log: user privileges None, administrative mode No; `%LocalAppData%\Programs\LoLMatchInfo` |
| Shortcuts/installed-app entry | PASS | Start Menu app and uninstall shortcuts; Desktop absent by default; HKCU display name/version/publisher/location/uninstall verified |
| Installed payload | PASS | 577 files including 575 publish files plus 2 Inno uninstall files; baseline, manifest, champion metadata and 173 icons present |
| Installed dev artifacts | PASS | no PDB/source/.env/collector/publisher/analyzer/DeveloperTestWindow filenames; publish validator and Release PE separation retained |
| Start Menu startup | PASS | installed process path, title `LoL Match Info`, responding true; diagnostics 0.9.0 and packaged dataset; normal Waiting/Shutdown |
| Installed About/Settings UI | PARTIAL | assembly/file/diagnostics version and dataset metadata prove backing state; native UI automation exposed no app surface, so panels were not directly clicked |
| Production statistics update | PASS/PARTIAL | existing read-only regression returned UpToDate and installed payload hashes match; installed Settings button was not directly invoked |
| Installed logging/crash | PASS | startup/diagnostics/Waiting/Shutdown appended under LocalAppData; historical two deliberate Debug crash files remained, no new normal-run crash |
| Install-directory writes | PASS | all 575 installed payload hashes exactly matched final publish after runtime; settings/logs remained outside install root |
| Shutdown | PASS | CloseMainWindow completed; remaining LoLMatchInfo processes 0 |
| Uninstall | PASS | exit 0; install directory, Start Menu group and HKCU uninstall entry removed |
| User-data preservation | PASS | user-data directory/settings/logs preserved; settings hash remained `532CD0343AE5614BC7EB441769465E83FF01D08E934D123AE67675F45E77848B` |
| Reinstall | PASS | same 0.9.0 installer exit 0; recreated shortcut launched installed app normally with existing user data |
| Isolated upgrade | PASS | temporary 0.9.1 publish validated; running 0.9.0 detected and shut down by Restart Manager; installed/app version updated, executable replaced, settings/logs preserved |
| Version restoration | PASS | source never left 0.9.0; installed app returned to 0.9.0; final 0.9.0 release rebuilt successfully |
| Signing/SmartScreen/icon | MANUAL | UNSIGNED BETA; security settings unchanged; no SmartScreen automation; final `.ico` remains branding polish |

Final severity: P0 0 / P1 0 / P2 0 / P3 1. P3 is the already documented lack of installer-enforced downgrade blocking; closed-beta release procedure must not distribute an older installer. Direct About/Settings screen interaction remains PARTIAL, not falsely promoted to PASS. Stage 10-C is **COMPLETE within the closed-beta distribution scope**; Stage 10-D may proceed with unsigned-build disclosure and remaining manual branding/GUI checks.
