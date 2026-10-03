# Champion skill cross-validation 16.17.1 — 조회 로그

조회 구간: 2026-10-03 21:40–21:48 UTC. 아래 CheckedAt은 행별 최종 자료 검토 2026-10-03 21:48:08 UTC이다. 개별 HTTP 응답 시각을 소급하여 만들어 기록하지 않았다.

## 방식

- web 도구 open/search/find로 제한된 공개 자료를 검토했다.
- `DIRECT_OPEN`은 웹 도구 원문 추출, `SEARCH_ONLY`는 검색 발췌다. 실제 인터랙티브 브라우저에서 읽은 페이지는 **0개**다.
- 유일한 browser attempt: IAB, 카서스 문서 URL `https://namu.wiki/w/%EC%B9%B4%EC%84%9C%EC%8A%A4`; 실패: `Browser not available: iab`.
- NamuWiki 정책/스킬 검색에서 유효한 문서 내용을 얻지 못했다. source-wide 제한 후 27개 개별 페이지 반복 접근을 하지 않았다.
- Wiki robots 제한이 도구에 표시되어 직접 skill page open/scraping을 하지 않았다. 공개 검색 결과만 검토했다.
- 브라우저 로그라는 사용자 요구에 맞춘 별도 조회 기록이며, search를 browser 방문으로 표시하지 않는다.

## 정책 및 공식 페이지

| Source / Page | URL | Method | Result | CheckedAt UTC |
| --- | --- | --- | --- | --- |
| NamuWiki robots | https://namu.wiki/robots.txt | DIRECT_OPEN attempt | non-retryable 접근 불가; 내용 미확인 | 2026-10-03 21:48:08 |
| NamuWiki 이용약관 | https://namu.wiki/w/나무위키:이용약관 | DIRECT_OPEN attempt + search | 내용 미확보; 정책상 금지라고 단정하지 않음 | 2026-10-03 21:48:08 |
| NamuWiki skill searches | https://namu.wiki/ | SEARCH_ONLY | Karthus Q, Zeri Q, Gangplank E, Zyra W 검색; 유효한 해당 스킬 수치 미확보 | 2026-10-03 21:48:08 |
| Wiki robots | https://wiki.leagueoflegends.com/robots.txt | DIRECT_OPEN attempt | 내용 접근 불가; skill 검색 도구에 Blocked by robots.txt 표시 | 2026-10-03 21:48:08 |
| Wiki 운영사 약관 | https://weirdgloop.org/terms/ | DIRECT_OPEN + find | 2025-01-31 갱신; 사전 동의 없는 자동 이용 제한 | 2026-10-03 21:48:08 |
| Riot robots | https://www.leagueoflegends.com/robots.txt | DIRECT_OPEN | Allow: / | 2026-10-03 21:48:08 |
| Riot terms | https://www.riotgames.com/en/terms-of-service | DIRECT_OPEN + find | 내용 읽음; find automated/scrap 결과만으로 포괄 수집 허가 주장하지 않음 | 2026-10-03 21:48:08 |
| Riot developer DD docs | https://developer.riotgames.com/docs/lol | DIRECT_OPEN + find | DD 공개 개발 데이터, 수동 갱신/버전 맥락 확인 | 2026-10-03 21:48:08 |
| DD CDN robots | https://ddragon.leagueoflegends.com/robots.txt | DIRECT_OPEN attempt | 내용 확보 실패 | 2026-10-03 21:48:08 |
| Official patch 16.17 guessed URL | https://www.leagueoflegends.com/en-us/news/game-updates/patch-16-17-notes/ | DIRECT_OPEN attempt | 도구 접근 불가; 없음이라고 단정하지 않음 | 2026-10-03 21:48:08 |
| Official patch 26.17 | https://www.leagueoflegends.com/en-us/news/game-updates/league-of-legends-patch-26-17-notes/ | DIRECT_OPEN + find Zeri/Gangplank | 성공; 2026-08-25 발행; 해당 문자열 find 없음은 변경 이력 전체 부재 증거가 아님 | 2026-10-03 21:48:08 |
| Official patch 14.21 | https://www.leagueoflegends.com/en-us/news/game-updates/patch-14-21-notes/ | DIRECT_OPEN | GP E 회복 변경 확인 | 2026-10-03 21:48:08 |
| Official patch 13.18 | https://www.leagueoflegends.com/en-us/news/game-updates/patch-13-18-notes/ | DIRECT_OPEN | Bard W 저장 2/stack time 18/성숙 5 확인 | 2026-10-03 21:48:08 |
| Official patch 12.19 | https://www.leagueoflegends.com/en-us/news/game-updates/patch-12-19-notes/ | DIRECT_OPEN + find | Syndra Q 진화 조건/2저장/1.25간격 확인 | 2026-10-03 21:48:08 |
| Official patch 12.7 | https://www.leagueoflegends.com/en-us/news/game-updates/patch-12-7-notes/ | DIRECT_OPEN + find | Karthus Q는 monster damage 변경; CD 변경 오연결 제외 | 2026-10-03 21:48:08 |
| Riot Singed page | https://www.leagueoflegends.com/en-us/champions/singed/ | DIRECT_OPEN | 열람 성공; Q toggle 상세 추출 불충분 | 2026-10-03 21:48:08 |
| Riot Shyvana page | https://www.leagueoflegends.com/en-us/champions/shyvana/ | DIRECT_OPEN | 열람 성공; 최신 kit 이름이 표시됨, R 자원 상세 추출 불충분 | 2026-10-03 21:48:08 |
| Riot Aphelios page | https://www.leagueoflegends.com/en-us/champions/aphelios/ | DIRECT_OPEN | Weapon Queue System 이름 확인; E 버튼 없음 문장 독립 추출 부족 | 2026-10-03 21:48:08 |

## 대상별 검토 자료

주 대상 전부에 대해 LOCAL_DD 방식으로 보관된 공식 16.17.1 원본과 production JSON을 읽었다. source URL은 https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json 이며 로컬 파일은 artifacts/skill-source-cache/16.17.1/championFull.json 이다. 전체 자료를 재수집하지 않았다.

| Champion / Skill | Source | URL / query target | Method / outcome | CheckedAt UTC |
| --- | --- | --- | --- | --- |
| Kalista E | Wiki | https://wiki.leagueoflegends.com/en-us/V8.13 | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Karthus Q | Wiki | https://wiki.leagueoflegends.com/en-us/Damage_modifier | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Rakan E | Riot/Wiki | Wiki 공개 검색: Rakan E PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| TahmKench R | Riot/Wiki | https://www.leagueoflegends.com/en-au/news/game-updates/patch-12-11-notes/ | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| TahmKench R | 추가 Riot | https://www.leagueoflegends.com/en-gb/news/game-updates/league-of-legends-patch-26-9-notes/ | SEARCH_ONLY | 2026-10-03 21:48:08 |
| Talon E | Riot/Wiki | Wiki 공개 검색: Talon E PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Veigar W | Riot/Wiki | Wiki 공개 검색: Veigar W PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Yuumi W | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Yuumi/You_and_Me%21 | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Zeri Q | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Zeri/Burst_Fire | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Amumu Q | Riot/Wiki | https://www.leagueoflegends.com/en-gb/news/game-updates/patch-11-17-notes/ | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Ashe E | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Ashe/Hawkshot | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Azir W | Riot/Wiki | Wiki 공개 검색: Azir W PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Bard W | Riot/Wiki | https://www.leagueoflegends.com/en-us/news/game-updates/patch-13-18-notes/ | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Caitlyn W | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Caitlyn/Yordle_Snap_Trap | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Corki R | Riot/Wiki | Wiki 공개 검색: Corki R PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Gangplank E | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Gangplank/Powder_Keg | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Gangplank E | 추가 Riot | https://www.leagueoflegends.com/en-us/news/game-updates/patch-14-21-notes/ | DIRECT_OPEN | 2026-10-03 21:48:08 |
| Heimerdinger Q | Wiki | https://wiki.leagueoflegends.com/en-us/League_of_Legends_Wiki%3ATutorial | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Ivern W | Riot/Wiki | Wiki 공개 검색: Ivern W PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Jhin E | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Jhin/Captive_Audience | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Jhin E | 추가 Riot | https://www.leagueoflegends.com/en-gb/news/game-updates/patch-12-8-notes/ | SEARCH_ONLY | 2026-10-03 21:48:08 |
| Kalista W | Riot/Wiki | Wiki 공개 검색: Kalista W PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Milio E | Wiki | https://wiki.leagueoflegends.com/Milio | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Milio E | 추가 Riot | https://www.leagueoflegends.com/en-gb/news/game-updates/patch-13-20-notes/ | SEARCH_ONLY | 2026-10-03 21:48:08 |
| Nilah E | Wiki | https://wiki.leagueoflegends.com/Nilah | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Rumble E | Riot/Wiki | Wiki 공개 검색: Rumble E PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Syndra Q | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Syndra/Dark_Sphere | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Syndra Q | 추가 Riot | https://www.leagueoflegends.com/en-us/news/game-updates/patch-12-19-notes/ | DIRECT_OPEN | 2026-10-03 21:48:08 |
| Teemo R | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Teemo/Noxious_Trap | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Velkoz W | Riot/Wiki | Wiki 공개 검색: Velkoz W PC skill cooldown/recharge | SEARCH_ONLY: 충분한 PC 자료 없음 / source access restricted | 2026-10-03 21:48:08 |
| Vi E | Wiki | https://wiki.leagueoflegends.com/en-us/Template%3AData_Vi/Relentless_Force | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |
| Zyra W | Riot/Wiki | https://www.leagueoflegends.com/en-us/news/game-updates/patch-11-10-notes/ | SEARCH_ONLY (위 DIRECT_OPEN 목록과 겹치는 공식 페이지는 원문도 확인) | 2026-10-03 21:48:08 |

추가 SEARCH_ONLY 공식 결과: GP E [11.17](https://www.leagueoflegends.com/en-gb/news/game-updates/patch-11-17-notes/), [13.12](https://www.leagueoflegends.com/en-ph/news/game-updates/patch-13-12-notes/); Zeri [26.10](https://www.leagueoflegends.com/en-sg/news/game-updates/league-of-legends-patch-26-10-notes/), [26.1](https://www.leagueoflegends.com/en-us/news/game-updates/patch-26-1-notes/). 동일 UTC 구간에 검토했다.

제외한 검색 결과 예시: WR Rakan Battle Dance, WR Talon Assassin's Path, 옛 Tahm Abyssal Voyage, WR Corki Missile Barrage, WR Milio Warm Hugs, WR Nilah Slipstream, WR Rumble Electro Harpoon, WR Syndra Dark Sphere, WR Teemo Noxious Trap, WR Vi Relentless Force, WR Zyra Rampant Growth. WR 4랭크 수치를 PC 5랭크 값으로 변환하지 않았다. 개인 Sandbox의 Amumu/Bard/Zyra/Tahm 설명은 참고 후보만 기록; 팬 제작 Emptylord 자료와 가상 챔피언 표는 제외했다.

외부 비교 자료를 확보하지 못한 9개도 대조표에 현재/원본 값 및 제한 결과를 기록했다. 나무위키 27개를 모두 성공 열람했다는 뜻이 아니다.

## 시작 git 상태

branch: main

```text
cfc7aba Complete Stage 10 release readiness, UX, and distribution
aa35d13 Stage 9 complete: regression fixes and portrait readability polish
634edaa Make debug validation dataset revision independent
4d1123e Complete Stage 9 statistics and team summary
f12f872 feat: complete adaptive five-role matchup dashboard
```

```text
 M LoLMatchup.csproj
 M MainWindow.xaml
 M MainWindow.xaml.cs
 M Models/Navigation/AppPage.cs
 M PROJECT_STATUS.md
 M Services/Logging/IAppLogger.cs
 M Services/Navigation/AppNavigationService.cs
 M Tools/RegressionScratch/Program.cs
 M Tools/RegressionScratch/RegressionScratch.csproj
 M Views/InGameView.xaml.cs
 M Views/LandscapeStatisticsView.xaml.cs
 M Views/PortraitStatisticsView.xaml
 M Views/PortraitStatisticsView.xaml.cs
 M Views/StatisticsViewResources.xaml
 M docs/regression-test-report.md
 M scripts/validate-release.ps1
?? Data/ChampionSkills/
?? Models/Champions/ChampionSkillData.cs
?? Services/ChampionSkillDataService.cs
?? Tools/ChampionSkillDataCollector/
?? Tools/RegressionScratch/SkillDataRegression.cs
?? Tools/RegressionScratch/SkillUiRegression.cs
?? Tools/RegressionScratch/verify-cooldown-source.ps1
?? Tools/RegressionScratch/verify-skill-mechanics.ps1
?? Views/ChampionSkillDetailView.xaml
?? Views/ChampionSkillDetailView.xaml.cs
?? Views/Controls/EnemyPortraitButton.cs
?? docs/champion-skill-data.md
?? docs/charge-skills-16.17.1.md
?? docs/cooldown-audit-16.17.1.md
?? docs/special-cooldown-report.md
```

## 종료 검증

기존 dirty 항목을 그대로 보존했다. 이번 작업이 추가한 파일은 보고서와 이 로그 2개뿐이다.

SHA-256 보호 파일 1,199개: changed 0, added 0. 기존 docs 해시 변경 0. source model/generated JSON/version/installer/portable 변경 0. 임시 tool 추가 0. 앱 및 tool build 수행 0.

`git diff --check`: exit 0. 기존 tracked 파일의 LF→CRLF 안내만 출력, whitespace error 없음. 새 untracked 보고서/로그는 별도로 trailing whitespace 및 conflict-marker 검사를 수행한다.

종료 상태는 아래 시작 목록에 다음 두 파일을 추가한 것과 같다:

```text
?? docs/champion-skill-cross-validation-16.17.1.md
?? docs/champion-skill-cross-validation-16.17.1-log.md
```

<!-- FINAL_LOG_VERIFICATION -->

