# Champion Skill Reference 외부 교차검증 — Data Dragon 16.17.1

검토 완료 시각: 2026-10-03 21:48:08 UTC. 외부 조회 구간: 2026-10-03 21:40–21:48 UTC. 보고서 전용 작업이며 앱 데이터 반영은 하지 않았다.

## 시작 상태와 보호 범위

branch `main`, 기존 dirty working tree. reset/revert/checkout/clean/commit을 실행하지 않았다. 시작 시 앱 코드, production JSON·아이콘, 개발 도구, 버전/배포 관련 파일과 기존 배포 산출물 1,199개 SHA-256을 메모리에 기록했다. 기존 문서를 우선 읽었다.

최근 커밋: `cfc7aba`, `aa35d13`, `634edaa`, `4d1123e`, `f12f872`. 상세 시작/종료 git 목록과 조회 방식은 [별도 로그](champion-skill-cross-validation-16.17.1-log.md)에 기록한다.

현재 앱 전체 분류: Standard 653, Charge 19, Toggle 4, ResourceBased 3, Passive 4, NotApplicable 1, Unknown 8. 173 champions / 692 spells. 주 대상은 Unknown 8 + Charge 19 = 중복 없는 **27개**. 아래 별도 특수 분류 점검 12개는 27개 통계에 포함하지 않는다.

## 접근 조건과 증거 한계

- **NamuWiki:** [robots](https://namu.wiki/robots.txt), [이용약관](https://namu.wiki/w/나무위키:이용약관) 및 챔피언 검색을 우선 시도했지만 도구가 페이지를 열지 못했고 사용 가능한 스킬 검색 발췌도 확보하지 못했다. IAB 브라우저 생성은 `Browser not available: iab`로 실패했다. 약관을 읽었다거나 모든 자동 접근을 금지한다고 단정하지 않는다. 출처 상태는 전 대상 **ACCESS_RESTRICTED**이며 숫자를 만들어 채우지 않았다. 동일 접근 문제 때문에 27개 문서를 반복 요청하지 않았다.
- **LoL Wiki:** 검색 도구에서 robots 제한을 명시했다. [root robots](https://wiki.leagueoflegends.com/robots.txt) 내용은 읽을 수 없었다. [운영사 이용약관](https://weirdgloop.org/terms/)은 2025-01-31 갱신본이며 사전 동의 없는 자동 이용을 제한한다. 직접 페이지 scraping, API 우회, 프록시, CAPTCHA 우회는 하지 않았다. 사용자 요청이 허용한 **공개 검색 결과**만 참고했다. 검색 결과의 “Crawled 1.2–1.3 years ago”는 현재 revision/16.17.1을 보증하지 않는다.
- **Riot:** [사이트 robots](https://www.leagueoflegends.com/robots.txt)는 Allow: /; [약관](https://www.riotgames.com/en/terms-of-service)과 [개발자 Data Dragon 문서](https://developer.riotgames.com/docs/lol#data-dragon)를 확인했다. Data Dragon은 제3자 개발자를 위한 static data이며 수동 갱신과 지역 클라이언트 버전 차이가 있을 수 있다. CDN robots 내용은 확인 불가. 로컬에 이미 보관된 공식 16.17.1 JSON을 읽어 추가 CDN 대량 요청을 피했다.
- 최신 champion page는 패치를 고정하지 않으며 일부 능력 설명이 정적 텍스트 추출에 나타나지 않는다. 그 페이지를 열었다는 사실만으로 CD/자원 메커니즘을 확정하지 않았다.
- Wild Rift, TFT, 팬 제작 `User:Emptylord`/가상 챔피언과 개인 수치표는 PC LoL 수치 근거에서 제외했다. Wiki Tutorial의 공식 능력 삽입 발췌는 제한적 참고만 했다. 개인 Sandbox 발췌는 별도 참고로 표시하고 숫자 확정/독립 출처 집계에 사용하지 않았다.

## 판정 및 통계 정의

한 스킬의 대표 판정은 확인된 상세가 있을 때 SECONDARY_ONLY_DETAIL, 동일 필드의 상충 주장이 있을 때 CONFLICT, 비교 가능한 외부 설명을 확보하지 못하고 접근이 제한됐을 때 ACCESS_RESTRICTED다. 따라서 나무위키 출처 27개가 막혔다고 전체 행을 모두 ACCESS_RESTRICTED로 덮어쓰지 않았다. null/미확인은 숫자 주장이 아니며 raw DD 0은 실제 0초로 해석하지 않는다.

**MATCH는 스킬 전체 판정**, “필드 일치”는 별도 기록이다. 외부 자료가 앱의 cast CD와 일치해도 앱에 없는 recharge 상세를 제공하면 대표 판정은 SECONDARY_ONLY_DETAIL이다. 앱 Unknown은 보수적인 저장 분류이지 스킬이 실제로 메커니즘을 갖지 않는다는 주장이 아니다.

| 대표 판정 | 개수 |
| --- | ---: |
| MATCH | 0 |
| SECONDARY_ONLY_DETAIL | 17 |
| CONFLICT | 1 |
| UNVERIFIED | 0 |
| ACCESS_RESTRICTED | 9 |
| 합계 | 27 |

**OFFICIALLY_CONFIRMED: 과거 특정 패치의 값/변경 기록을 공식 문서로 확인한 스킬 8개**(Amumu Q, Bard W, Gangplank E, Jhin E, Milio E, Syndra Q, Zyra W, TahmKench R). 이는 보조 표시이며 위 대표 판정 통계와 중복된다. **16.17.1의 누락 수치까지 확정된 항목은 0개**다. 과거 변경 이후 모든 후속 패치/핫픽스를 추적하지 못했으므로 현재 적용을 단정하지 않는다.

Charge 19개의 **16.17.1 외부 검증 완료** 집계: max charges 0, recharge time 0, cast cooldown 0, fully confirmed 0. 별도로, 로컬 공식 DD에서 현재 앱이 보존한 값은 max 8개, cast CD 15개, recharge 0개다. 이번 공개 검색 발췌에서 현재 앱 필드와 숫자가 일치한 참고 사례는 max 4개(Ashe/Jhin/Milio/Vi), cast 6개(Ashe/Caitlyn/Jhin/Milio/Teemo/Vi); Amumu 공식 과거 문서의 max 2/cast 3도 일치한다. 이 수치는 **패치 호환 확정 수에 합산하지 않는다**.

Unknown 8개의 16.17.1 해결 가능 0, 여전히 Unknown 8, 확정 CONFLICT 0. 5개는 추가 설명/과거 값 확보, 3개(Rakan/Talon/Veigar)는 허용된 외부 자료 부족. 실제 앱 분류는 모두 유지했다.

## 값 표기

시간 단위는 seconds. C=앱 cast cooldown, R=충전 1회 회복, M=최대 저장. 반복 값은 배열 그대로 표기한다. DD `maxammo`는 원본 scalar 메타데이터이며 all-rank maxCharges 배열과 같다고 가정하지 않는다. 모든 대상의 R 숫자는 앱에서 null이며 DD tooltip의 회복 placeholder는 미해결이다.

## 27개 비교 표

모든 행의 NamuWiki Value는 접근 제한이다. 각 행의 공식 source는 [Riot DD 16.17.1](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json)의 `data.<Champion>.spells[index]`; index는 Q=0,W=1,E=2,R=3. 확인 시각/secondary URL/patch context는 아래 출처표에 행별로 기록한다.

| Champion | Skill | Current App Value | Riot Value | NamuWiki Value | Additional Source Value | Mechanic | Classification | Patch Note | Recommended Action |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Kalista | E 뽑아 찢기 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | CD 10/9.5/9/8.5/8; 처치 시 초기화 | 지속 효과 + 능동 창 회수; 처치 초기화는 충전이 아님 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Karthus | Q 황폐화 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=2; rank=5 | ACCESS_RESTRICTED | CD 1초; charge 설명 없음 | 반복 능동 시전; DD maxammo=2는 저장 충전의 확정 증거가 아님 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Rakan | E 전쟁무도 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 아군 dash + 제한 시간 재시전; 두 번 이동은 시간 기반 저장 충전과 다름 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| TahmKench | R 집어삼키기 (3 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0; maxammo=-1; rank=3 | ACCESS_RESTRICTED | 공식 12.11: CD 130/110/90 → 120/100/80; 26.9: 아군 삼키기 MS 40% → 60% | 아군/적 삼키기와 뱉기 재시전; 적은 3중첩 필요; 재시전 상태와 최초 CD 분리 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 12.11) | NEEDS_OFFICIAL_CONFIRMATION |
| Talon | E 암살자의 길 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 지형별 재사용 제한 wallcd; 동일 벽 잠금과 전체 시전 간격 분리 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Veigar | W 암흑 물질 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 극악 중첩 의존 CD 감소; 고정 0초로 표현 금지 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Yuumi | W 너랑 유미랑! (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | level 기반 static CD 10/5/0; CC lockout 5; 부착 재시전 0.5; 탈착 0.25 | 아군 부착/이동/탈착과 CC 잠금; 5랭크와 3단계 level CD를 분리 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Zeri | Q 집중 사격 (5 ranks) | Unknown; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=-1; rank=5 | ACCESS_RESTRICTED | static CD=1/attack speed; 검색 캐시 cap 1.5 공격/초 | 버튼 입력 기본 공격; 공격 속도 의존, 5랭크 고정 CD 배열로 표현 불가 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Amumu | Q 붕대 던지기 (5 ranks) | Charge; C=3/3/3/3/3; R=미확인; M=2 | raw CD=3/3/3/3/3; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; recharge 16/15.5/15/14.5/14; cast CD 3 | 저장 2회; 회복과 시전 간격 분리 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 11.17) | NEEDS_OFFICIAL_CONFIRMATION |
| Ashe | E 매 날리기 (5 ranks) | Charge; C=5/5/5/5/5; R=미확인; M=2 | raw CD=5/5/5/5/5; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; recharge 90/80/70/60/50; cast CD 5 | 정찰용 저장 충전; 5초 시야 지속과 5초 CD는 별개 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Azir | W 일어나라! (5 ranks) | Charge; C=1.5/1.5/1.5/1.5/1.5; R=미확인; M=미확인 | raw CD=1.5/1.5/1.5/1.5/1.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 병사 저장 충전; maxammo placeholder와 설치 병사 수 분리 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Bard | W 수호자의 성소 (5 ranks) | Charge; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; stack 회복 18; 성소 성숙 5; 별도 cast CD 미확인 | 저장 충전 / 설치 성소 수 / 성숙 시간을 분리 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 13.18) | NEEDS_OFFICIAL_CONFIRMATION |
| Caitlyn | W 요들잡이 덫 (5 ranks) | Charge; C=0.5/0.5/0.5/0.5/0.5; R=미확인; M=미확인 | raw CD=0.5/0.5/0.5/0.5/0.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | CD 0.5; recharge 26/22/18/14/10; 표시된 trap 최대 3/3/4/4/5 | 저장 충전과 설치 덫 제한 분리; 검색의 Maximum Number of Traps를 maxCharges로 단정하지 않음 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Corki | R 미사일 폭격 (3 ranks) | Charge; C=2/2/2; R=미확인; M=미확인 | raw CD=2/2/2; maxammo=4; rank=3 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 미사일 저장 충전 + 챔피언 기본 공격으로 회복 감소 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Gangplank | E 화약통 (5 ranks) | Charge; C=미확인; R=미확인; M=미확인 | raw CD=0/0/0/0/0; maxammo=3; rank=5 | ACCESS_RESTRICTED | cast CD 0.5; recharge 17/16/15/14/13; 역사적 max 3/3/4/4/5 | Charge; 저장 수와 회복은 rank; 통 체력 감소 2/1/0.5는 champion level | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 14.21 / 11.17) | NEEDS_OFFICIAL_CONFIRMATION |
| Heimerdinger | Q H-28 G 진화형 포탑 (5 ranks) | Charge; C=1/1/1/1/1; R=미확인; M=미확인 | raw CD=1/1/1/1/1; maxammo=3; rank=5 | ACCESS_RESTRICTED | turret kit max 3; recharge 20; 별도 cast CD는 해당 PC 검색 발췌에 없음 | 저장 kit 3과 설치 포탑 3, 포탑 빔 충전을 각각 분리 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Ivern | W 수풀 가꾸기 (5 ranks) | Charge; C=0.5/0.5/0.5/0.5/0.5; R=미확인; M=미확인 | raw CD=0.5/0.5/0.5/0.5/0.5; maxammo=3; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 수풀 생성 능동 스킬 + 기본 공격 강화 지속 효과; raw maxammo 기반 Charge 후보 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Jhin | E 강제 관람 (5 ranks) | Charge; C=2/2/2/2/2; R=미확인; M=2 | raw CD=2/2/2/2/2; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; recharge 24/21.5/19/16.5/14; cast CD 2 | 지속 효과 처치 함정 + 능동 저장 함정; Passive 전용 스킬이 아님 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 12.8) | NEEDS_OFFICIAL_CONFIRMATION |
| Kalista | W 감시하는 혼 (5 ranks) | Charge; C=30/30/30/30/30; R=미확인; M=2 | raw CD=30/30/30/30/30; maxammo=2; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 대상별 지속 효과 CD와 정찰 혼 저장/회복 CD 분리 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Milio | E 따스한 포옹 (5 ranks) | Charge; C=0.5/0.5/0.5/0.5/0.5; R=미확인; M=2 | raw CD=0.5/0.5/0.5/0.5/0.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; recharge 17/16/15/14/13; static CD 0.5 | 저장 2회; 같은 대상 보호막 효과 중첩과 저장 수는 별개 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 13.20) | NEEDS_OFFICIAL_CONFIRMATION |
| Nilah | E 급류 (5 ranks) | Charge; C=0.5/0.5/0.5/0.5/0.5; R=미확인; M=미확인 | raw CD=0.5/0.5/0.5/0.5/0.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | PC 검색 발췌에서 max 2만 확인; recharge/cast 미확인 | 대상 지정 돌진 충전; Wild Rift의 처치/명중 초기화 설명은 제외 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Rumble | E 전기 작살 (5 ranks) | Charge; C=0.5/0.5/0.5/0.5/0.5; R=미확인; M=미확인 | raw CD=0.5/0.5/0.5/0.5/0.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 작살 저장 충전 + 열 생성/위험 상태; 자원 소모 전용으로 재분류하지 않음 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Syndra | Q 어둠 구체 (5 ranks) | Charge; C=미확인; R=미확인; M=미확인; base=7/7/7/7/7 | raw CD=7/7/7/7/7; maxammo=-1; rank=5 | ACCESS_RESTRICTED | base CD 7; 40조각 이후 max 2, static cast CD 1.25; Wiki recharge 7 | 조건부 Charge; base 7을 진화 후 cast CD로 대입 금지 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 12.19) | NEEDS_OFFICIAL_CONFIRMATION |
| Teemo | R 유독성 함정 (3 ranks) | Charge; C=0.25/0.25/0.25; R=미확인; M=미확인 | raw CD=0.25/0.25/0.25; maxammo=3; rank=3 | ACCESS_RESTRICTED | 같은 검색 결과 preview recharge 35/30/25, parameters 35/32.5/30/27.5/25 및 max 3/3.5/4/4.5/5; CD 0.25 | Charge; 3랭크에 5개/분수 charge 값이 섞인 캐시 내부 불일치 | CONFLICT | 16.17.1 호환 미확인 | REVIEW_PATCH_MISMATCH |
| Velkoz | W 공허 균열 (5 ranks) | Charge; C=1.5/1.5/1.5/1.5/1.5; R=미확인; M=2 | raw CD=1.5/1.5/1.5/1.5/1.5; maxammo=2; rank=5 | ACCESS_RESTRICTED | PC 해당 스킬의 충분한 검색 발췌 미확보; WR/팬 페이지 제외 | 균열 2회 저장; 첫 피해/폭발 지연과 회복 시간을 구분 | ACCESS_RESTRICTED | 16.17.1 호환 미확인 | KEEP_UNVERIFIED |
| Vi | E 끈질긴 힘 (5 ranks) | Charge; C=1/1/1/1/1; R=미확인; M=2 | raw CD=1/1/1/1/1; maxammo=2; rank=5 | ACCESS_RESTRICTED | max 2; recharge 12/11/10/9/8; static CD 1 | 저장 충전 + 다음 기본 공격 강화; cast time none과 CD 1은 다름 | SECONDARY_ONLY_DETAIL | 16.17.1 호환 미확인 | NEEDS_OFFICIAL_CONFIRMATION |
| Zyra | W 맹렬한 성장 (5 ranks) | Charge; C=미확인; R=미확인; M=2 | raw CD=0/0/0/0/0; maxammo=2; rank=5 | ACCESS_RESTRICTED | 공식 11.10: 20/18/16/14/12 → 18/16/14/12/10; 검색 참고: 2 seeds + 처치 회복 감소 | 능동 씨앗 저장 2; 시간 회복 + 처치 환급; 패시브 씨앗 생성 주기와 별개 | SECONDARY_ONLY_DETAIL | OFFICIALLY_CONFIRMED (historical 11.10) | NEEDS_OFFICIAL_CONFIRMATION |

## 행별 출처·시각·맥락

아래 CheckedAt은 해당 행의 자료를 최종 대조한 UTC 시각이다. 실제 외부 요청은 위 조회 구간 내에 수행됐다. 링크가 있는 검색 자료도 직접 원문 페이지가 아니라 검색 발췌를 검토한 경우를 명시한다.

| Champion / Skill | Riot source | Secondary source / URL | CheckedAt UTC | Patch/context |
| --- | --- | --- | --- | --- |
| Kalista E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Kalista.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/V8.13) | 2026-10-03 21:48:08 UTC | 과거 패치 페이지의 검색 캐시/동적 삽입 설명; 16.17.1 미보증 |
| Karthus Q | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Karthus.spells[0] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Damage_modifier) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Rakan E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Rakan.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| TahmKench R | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.TahmKench.spells[3] | NamuWiki: 제한(문서별 성공 조회 없음); [Riot 공식 패치](https://www.leagueoflegends.com/en-au/news/game-updates/patch-12-11-notes/); [추가 공식](https://www.leagueoflegends.com/en-gb/news/game-updates/league-of-legends-patch-26-9-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 12.11; Wiki revision/16.17.1 호환 미확인 |
| Talon E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Talon.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Veigar W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Veigar.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Yuumi W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Yuumi.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Yuumi/You_and_Me%21) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Zeri Q | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Zeri.spells[0] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Zeri/Burst_Fire) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Amumu Q | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Amumu.spells[0] | NamuWiki: 제한(문서별 성공 조회 없음); [Riot 공식 패치](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-11-17-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 11.17; Wiki revision/16.17.1 호환 미확인 |
| Ashe E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Ashe.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Ashe/Hawkshot) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Azir W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Azir.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Bard W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Bard.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); [Riot 공식 패치](https://www.leagueoflegends.com/en-us/news/game-updates/patch-13-18-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 13.18; Wiki revision/16.17.1 호환 미확인 |
| Caitlyn W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Caitlyn.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Caitlyn/Yordle_Snap_Trap) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Corki R | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Corki.spells[3] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Gangplank E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Gangplank.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Gangplank/Powder_Keg); [추가 공식](https://www.leagueoflegends.com/en-us/news/game-updates/patch-14-21-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 14.21 / 11.17; Wiki revision/16.17.1 호환 미확인 |
| Heimerdinger Q | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Heimerdinger.spells[0] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/League_of_Legends_Wiki%3ATutorial) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Ivern W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Ivern.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Jhin E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Jhin.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Jhin/Captive_Audience); [추가 공식](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-12-8-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 12.8; Wiki revision/16.17.1 호환 미확인 |
| Kalista W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Kalista.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Milio E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Milio.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/Milio); [추가 공식](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-13-20-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 13.20; Wiki revision/16.17.1 호환 미확인 |
| Nilah E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Nilah.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/Nilah) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Rumble E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Rumble.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Syndra Q | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Syndra.spells[0] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Syndra/Dark_Sphere); [추가 공식](https://www.leagueoflegends.com/en-us/news/game-updates/patch-12-19-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 12.19; Wiki revision/16.17.1 호환 미확인 |
| Teemo R | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Teemo.spells[3] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Teemo/Noxious_Trap) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Velkoz W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Velkoz.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); Wiki 공개 검색: 해당 PC 수치 확보 실패 | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Vi E | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Vi.spells[2] | NamuWiki: 제한(문서별 성공 조회 없음); [Wiki 공개 검색 발췌](https://wiki.leagueoflegends.com/en-us/Template%3AData_Vi/Relentless_Force) | 2026-10-03 21:48:08 UTC | Wiki 과거 검색 캐시; 정확한 patch/revision 미확인 |
| Zyra W | [DD](https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json) data.Zyra.spells[1] | NamuWiki: 제한(문서별 성공 조회 없음); [Riot 공식 패치](https://www.leagueoflegends.com/en-us/news/game-updates/patch-11-10-notes/) | 2026-10-03 21:48:08 UTC | 공식 과거 11.10; Wiki revision/16.17.1 호환 미확인 |

## 비교 항목과 flags

각 행의 이름/key/rank는 앱과 고정 DD 16.17.1에서 동일하게 대조했다. 외부 자료에서 같은 영문 스킬을 식별할 수 있는 행만 해당 이름을 대응시켰다. 접근 제한된 행의 외부 rank/flags는 미확인이다. Teemo R은 외부 캐시 rank 구조가 상충한다.

| Champion / Skill | App type | Toggle / ResourceBased / Passive-only / NotApplicable | 실제 버튼/복합 지속 효과 | 외부 rank |
| --- | --- | --- | --- | --- |
| Kalista E | Unknown | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; 지속 효과도 포함; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Karthus Q | Unknown | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Rakan E | Unknown | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| TahmKench R | Unknown | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Talon E | Unknown | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Veigar W | Unknown | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Yuumi W | Unknown | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; 지속 효과도 포함; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Zeri Q | Unknown | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Amumu Q | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Ashe E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Azir W | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Bard W | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Caitlyn W | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Corki R | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Gangplank E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Heimerdinger Q | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Ivern W | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; 지속 효과도 포함; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Jhin E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; 지속 효과도 포함; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Kalista W | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; 지속 효과도 포함; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Milio E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Nilah E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Rumble E | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Syndra Q | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Teemo R | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 3랭크 preview / 5값 parameter 상충 |
| Velkoz W | Charge | 앱 분류상 모두 아니오; 외부 미확인 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Vi E | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |
| Zyra W | Charge | 앱 분류상 모두 아니오; 외부 해당 설명 범위만 검토 | 버튼 입력 스킬; DD description/tooltip 근거 | 현재 patch rank 독립 확정 불가 |

“Passive-only 아니오”는 지속 효과 부분이 없다는 의미가 아니다. “ResourceBased 아니오”는 마나/열 같은 자원 상호작용이 없다는 뜻이 아니라 앱의 주 CD 분류 기준이다.

## 주요 조사 결과

### Karthus Q

DD는 CD [0,0,0,0,0], maxammo=2지만 저장 충전 설명이 없다. PC Wiki 능력 삽입 검색 발췌는 1초 CD를 제시한다. 0을 실제 0초로 해석하거나 maxammo=2를 2회 Charge로 해석할 수 없다. 현 Unknown/null과 1초 주장은 직접 숫자 충돌이 아니므로 SECONDARY_ONLY_DETAIL이다. 12.7 공식 페이지를 확인했지만 Q 관련 변경은 몬스터 피해이며, 검색 요약에 나타난 “Q cooldown decreased”를 Karthus 변경으로 잘못 연결하지 않았다. 1초의 16.17.1 유지 여부는 미확인.

### Zeri Q

DD가 명시한 기본 공격 취급과 Wiki의 공격 속도 의존 설명이 메커니즘 수준에서 일치한다. 공격 속도 역수 형태의 간격은 랭크별 고정 CD와 다르다. 오래된 Wiki의 공격 속도 상한/초과 AS 변환 세부 숫자는 현재 값으로 사용하지 않는다. 공식 [26.10 패치](https://www.leagueoflegends.com/en-sg/news/game-updates/league-of-legends-patch-26-10-notes/)에는 초과 AS→AD 변환 변경이 검색되었다. 이는 오래된 70% 설명을 무조건 재사용하면 위험하다는 추가 맥락이며, Q 간격 공식을 현재 확정하는 증거는 아니다. Unknown 유지, 보완 후보는 메커니즘 설명 검토까지다.

### Gangplank E

[11.17 공식](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-11-17-notes/)은 저장 최대 3/3/4/4/5, 당시 recharge 18/17/16/15/14를 기록한다. [13.12 공식](https://www.leagueoflegends.com/en-ph/news/game-updates/patch-13-12-notes/)에도 회복 배열 변경 기록이 있고, [14.21 공식](https://www.leagueoflegends.com/en-us/news/game-updates/patch-14-21-notes/)의 17/16/15/14/13은 Wiki 검색 값과 일치한다. 이 변경 기록은 역사적으로 확정된다. 서로 다른 패치의 회복 배열은 같은 시점 주장 충돌로 평균내지 않는다.

저장 수/회복 배열은 spell rank, 통 체력 감소 주기는 champion level이며, 검색 cast time 0.25와 cast CD 0.5는 서로 다른 필드다. 앱은 max=null, recharge=null, cast=null이어서 raw maxammo=3을 전 rank maximum으로 잘못 표시하지 않는다. 16.17.1까지 변경 이력의 연속성이 미확인이라 SAFE_TO_ENRICH는 아니다.

### Zyra W

고정 DD tooltip에 2회 저장과 시간 회복, 미니언/몬스터 처치 및 champion takedown의 회복 감소가 명시되어 Charge 분류 근거가 있다. [11.10 공식](https://www.leagueoflegends.com/en-us/news/game-updates/patch-11-10-notes/)은 18/16/14/12/10 수치를 기록하지만 필드명을 cooldown이라고 쓴다. Wiki의 개인 Sandbox 발췌는 이를 recharge로 표시한다. 공식의 단어 하나를 근거로 앱의 cast CD에 그 배열을 넣지 않는다. recharge 해석과 현재 patch 호환은 추가 확인 대상이다.

Garden of Thorns 자동 씨앗 생성 주기, 지면에 존재하는 씨앗 제한, W 저장 2개, W 회복/처치 환급은 서로 다르다. 앱 cast CD null 및 recharge null 유지.

### Rakan / TahmKench / Yuumi

Rakan은 아군 dash의 재시전 window, Tahm은 삼키는 대상과 중첩 조건 및 뱉기 상태, Yuumi는 부착/탈착과 CC lockout을 따로 모델링해야 한다. 두 번 동작하거나 버튼이 상태에 따라 바뀌는 것을 Charge/Toggle/Passive-only로 자동 변경하지 않는다.

Tahm R은 공식 과거 12.11 CD 120/100/80 기록을 확보했지만 현재 값으로 반영하지 않는다. 26.9에는 아군 삼키기 이동 속도 변경이 있어 오래된 상세 설명을 그대로 최신 설명으로 부르는 것도 부적절하다. Yuumi의 level 기반 10/5/0은 스킬 5랭크 배열이 아니며 부착 재시전/CC 잠금과 섞지 않는다.

### Teemo R — CONFLICT

같은 PC 템플릿 검색 발췌 안에서 3랭크 preview와 5개 parameter 값 및 분수 최대 충전 값이 섞였다. 직접 원문/revision은 robots 제한으로 확인하지 못했으므로 어느 배열이 실제 원문 최신 값이라고 단정하지 않는다. **검색 캐시 내부 충돌**로 CONFLICT + REVIEW_PATCH_MISMATCH를 기록한다. NamuWiki↔Riot 충돌을 확인한 사례는 아니다. DD와 일치한 0.25 cast CD도 recharge/max의 충돌을 해결하지 않는다.

## 기존 특수 분류 12개 점검 (주 대상 통계 제외)

고정 DD 문구를 다시 확인했으며 외부 사이트 비교 가능 범위를 아래에 분리했다. 전체 분류가 외부에서 검증됐다고 과장하지 않는다.

| Champion / Skill | 현재 분류 | 고정 DD 메커니즘 | 외부 확인 |
| --- | --- | --- | --- |
| Amumu W | Toggle | 활성화/비활성화 <toggle> 명시 | Wiki Sandbox의 Toggle 참고는 있음; 확정 근거로 미집계 |
| Anivia R | Toggle | 활성화/비활성화 <toggle> 명시 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Aphelios E | NotApplicable | 세 번째 스킬 없음; 다음 무기 정보 표시 | [Riot champion page](https://www.leagueoflegends.com/en-us/champions/aphelios/) 열람; 해당 상세/버튼 상태 정적 추출 부족, 외부 확인 UNVERIFIED |
| Ashe Q | ResourceBased | 중첩/용의 분노 생성·소모 명시 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Karthus E | Toggle | 활성화/비활성화 <toggle> 명시 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Shyvana R | ResourceBased | 중첩/용의 분노 생성·소모 명시 | [Riot champion page](https://www.leagueoflegends.com/en-us/champions/shyvana/) 열람; 해당 상세/버튼 상태 정적 추출 부족, 외부 확인 UNVERIFIED |
| Singed Q | Toggle | 활성화/비활성화 <toggle> 명시 | [Riot champion page](https://www.leagueoflegends.com/en-us/champions/singed/) 열람; 해당 상세/버튼 상태 정적 추출 부족, 외부 확인 UNVERIFIED |
| Teemo E | Passive | 지속 효과만 설명; 별도 사용 동작 없음 | [PC Wiki 검색](https://wiki.leagueoflegends.com/Teemo)의 Toxic Shot Passive 설명과 일치(과거 캐시) |
| TwistedFate E | Passive | 지속 효과만 설명; 별도 사용 동작 없음 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Vayne W | Passive | 지속 효과만 설명; 별도 사용 동작 없음 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Vi W | Passive | 지속 효과만 설명; 별도 사용 동작 없음 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |
| Yunara Q | ResourceBased | 중첩/용의 분노 생성·소모 명시 | ACCESS_RESTRICTED / 독립 외부 설명 미확보 |

Urgot W는 전체 Toggle가 아니라 rank 5의 조건부 toggle인 기존 Standard 12/9/6/3/0 예외로 문서에서 확인했으며 이번 외부 검증의 전체 확정 사례로 집계하지 않았다.

## Patch/context 검토

[공식 26.17 패치노트](https://www.leagueoflegends.com/en-us/news/game-updates/league-of-legends-patch-26-17-notes/)는 2026-08-25 발행이다. “patch-16-17-notes” URL은 열리지 않았고 실제 공개 명칭은 26.17이다. DD 16.17.1과 26.17 표기는 동일한 2026 시즌의 서로 다른 명명 체계로 **추론**되며, Riot 26.1 페이지의 “original 16.1 patch” 문구도 이 맥락을 뒷받침한다. 이름 차이만으로 서로 다른 실제 패치라고 확정하지 않는다.

26.17에 특정 스킬 변경이 없다는 사실만으로 2021–2025의 숫자가 지금까지 유지됐음을 증명할 수 없다. 과거 공식 기록과 오래된 Wiki 캐시, 최신 미고정 champion page를 분리했다. patch mismatch 검토 사례는 GP 회복 이력, Zeri 과거 세부 수치, Tahm 최근 아군 동작 변경, Teemo 검색 내부 rank 배열이다.

## 반영 판단

- SAFE_TO_ENRICH: **없음**. 현재 patch 호환성과 두 독립 출처 일치까지 충족한 누락값을 확보하지 못했다. 서로 다른 언어의 Riot 패치 문서는 독립 출처 두 개로 세지 않는다.
- NEEDS_OFFICIAL_CONFIRMATION: Kalista E, Karthus Q, TahmKench R, Yuumi W, Zeri Q, Amumu Q, Ashe E, Bard W, Caitlyn W, Gangplank E, Heimerdinger Q, Jhin E, Milio E, Nilah E, Syndra Q, Vi E, Zyra W.
- KEEP_UNVERIFIED: Rakan E, Talon E, Veigar W, Azir W, Corki R, Ivern W, Kalista W, Rumble E, Velkoz W.
- REVIEW_PATCH_MISMATCH: Teemo R.
- KEEP_CURRENT: production은 모든 27개를 실제로 그대로 유지; 행별 다음 조사 권고와 별개다.

NamuWiki를 production source로 채택할 근거를 확보하지 못했다. 교차검증 참고 출처로만 취급해야 한다. 이번에 안전하게 보완할 수 있는 것은 보고서의 출처/과거 패치 메모이며 앱 숫자 보완은 없다. 다음 단계는 접근이 허용된 수동 확인으로 patch/revision을 기록하고, 공식 26.17 시점의 능력 데이터 또는 연속된 공식 변경 이력으로 누락 수치를 확정하는 것이다. 특히 Teemo 캐시 rank 불일치와 GP rank별 maxima, Syndra 진화 전/후 모델, Karthus/Zeri 간격 의미를 우선 확인한다.

## 종료 검증

검증용 script/tool은 새로 만들지 않았다. 앱 build와 installer 생성은 하지 않았다. 코드/production JSON/model/version/아이콘/배포 파일은 시작 SHA-256과 종료 SHA-256을 비교한다. 종료 결과는 아래에 기록하며 상세 git 상태는 별도 로그에 둔다.

보호 파일 1,199개 모두 해시 동일(changed 0, added 0); 기존 docs 변경 0. 이번 작업은 보고서와 별도 조회 로그 2개만 추가했다. `git diff --check` exit 0, 기존 LF→CRLF 안내 외 whitespace error 없음. 새 문서는 별도 공백/충돌 마커 검사 대상으로 확인했다.

