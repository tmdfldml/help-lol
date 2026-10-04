# LoL Match Info Release Checklist

기록일: 2026-10-04 KST. 이번 RC의 명령·수치·결과는 [회귀 보고서](regression-test-report.md)의 Stage 11-A와 ignored artifacts/stage11a-* 로그를 참조한다. 미실행 항목은 PASS로 기록하지 않는다.

## RC candidate build — Stage 11-A

- [x] 시작 main / 2adbcce / clean, 기존 tags 기록
- [x] csproj 단일 version source: 1.0.0-rc.1; About informational version 유지
- [x] Debug / Release 및 StatsCollector, PublishStatisticsDataset, AnalyzeStatisticsDataset, ChampionSkillDataCollector build (0 warning / 0 error)
- [x] release notes / known issues / CHANGELOG 작성
- [x] fresh win-x64 self-contained publish, single-file/trimming/ReadyToRun off
- [x] installer / portable / release manifest / SHA256SUMS 생성 및 크기 기록
- [x] baseline / champion icons / 173 skill JSON / 692 skill icons / manifest
- [x] developer artifacts / synthetic UI / fixtures / source / PDB 제외
- [x] secret / privacy / local paths / manifest / checksum validator PASS
- [x] unsigned RC 및 SmartScreen 기록
- [x] installer silent install / installed EXE / launch / uninstall smoke
- [x] git diff --check 및 새 P0/P1 없음

## RC verification — Stage 11-B (historical pending snapshot; gate closure below supersedes)

- [ ] 실제 게임: 감지 → 5역할 → 상대 초상화 → QWER → Back → 종료/Waiting
- [ ] Team Summary / Matchup Detail / Settings / About 직접 GUI 확인
- [ ] 통계 업데이트 / 실패 fallback / 재시작 후 activation
- [ ] install / Start Menu / uninstall / user-data preservation / upgrade / reinstall 전체 lifecycle
- [ ] clean first run / install directory write 부재
- [ ] DPI / portrait·landscape / multi-monitor / keyboard / screen reader
- [ ] 로그 retention 및 실제 fatal: critical log / crash file / 한국어 안내 / 종료
- [ ] 실제 내려받은 파일의 Defender/SmartScreen 경고 여부 기록 (보안 설정 변경 없음)

## Final 1.0 promotion

- [x] Stage 11-B 검증 및 제한 수용, P0/P1 없음
- [x] LoLMatchup.csproj Version만 1.0.0으로 변경; AssemblyVersion=1.0.0.0 유지
- [x] README / CHANGELOG / PROJECT_STATUS / release notes / checklist / distribution / regression report 갱신
- [x] fresh pipeline 재실행 (이름·hash·manifest 재생성; RC artifacts 재사용 금지)
- [x] code signing 가능 여부 결정; 인증서 없이 임의 서명 금지
- [ ] 최종 release 승인 후 tag / upload를 별도 수행

RC tag 제안: `git tag -a v1.0.0-rc.1 -m "LoL Match Info 1.0.0-rc.1"`
Final tag 제안: `git tag -a v1.0.0 -m "LoL Match Info 1.0.0"`
먼저 변경을 검토·커밋한 뒤 사용한다. 이번 작업은 commit/tag/push/upload를 수행하지 않는다.

통계 16.20 준비는 app RC와 독립이다. 16.18은 publish하지 않으며 신규 통계를 별도 검증·배포해도 app version을 올릴 필요가 없다.

## Stage 11-B verification snapshot — 2026-10-04

**PARTIAL / MANUAL_PENDING; 1.0 promotion HOLD.** Start main / clean / 338a995 / v1.0.0-rc.1. User stopped Computer Use with Escape; direct GUI inputs were stopped. Detailed 46-item acceptance matrix and provenance are in regression-test-report.md.

- [x] Original and final RC artifact SHA256/size match; SHA256SUMS unchanged
- [x] Existing regression once without rebuild; updater 19 self-tests PASS
- [x] Final release validator PASS: baseline/icons/skills/ZIP/dev/secret/privacy/path/manifest/checksums
- [x] Installed RC Home/Waiting directly observed; version metadata 1.0.0-rc.1
- [x] Production manifest 16.17 / revision 1788854099077 confirmed; UpToDate regression PASS
- [x] Actual application log privacy/no new crash; Waiting and GameDetected once each
- [x] Unsigned confirmed; Defender enabled and no RC-specific threat record (read-only)
- [ ] Full real game: detection → 5-role mapping → portrait/QWER/back → game-end return
- [ ] Settings/About direct GUI and update action/restart activation
- [ ] Four exact viewport sizes, seven special skill displays, scroll/click conflict
- [ ] Actual DPI 100/125/150 and multi-monitor move/relaunch
- [ ] Window persistence, pin, reset/confirm, invalid/off-screen fallback
- [ ] Clean install/upgrade/running-app upgrade/uninstall/reinstall/isolated portable
- [ ] Actual fatal Korean dialog/controlled exit and native offline/failure smoke
- [ ] Downloaded-file SmartScreen observation; dedicated Defender scan if available

No version/artifact change, app rebuild, publish, commit, push or tag. Existing P3 downgrade policy remains. Do not mark composite manual items PASS based on automated fixtures. Stage 11-B final completion and Stage 11-C promotion require the open acceptance items.

### Manual continuation — 2026-10-04 KST

- [x] Actual installed RC InGame/LIVE and TOP Garen–Akali display directly observed (narrow observation only)
- [ ] Full Waiting/GameDetected/InGame/5-role/Skill Detail/QWER/Back/game-end sequence: MANUAL_PENDING
- [ ] Four viewports, three DPI scales, multi-monitor and Settings/persistence: MANUAL_PENDING
- [ ] Requested RC lifecycle/portable, synthetic fatal dialog/exit and downloaded-file SmartScreen: MANUAL_PENDING

Computer Use repeatedly failed portrait input with `SendInput sent 0 of 1 events; GetLastError=87`, including after the user paused input and the RC was reactivated. No confirmed new product defect; prior PASS checks were not repeated. No code/version/artifact/build/commit/push/tag change. Stage 11-B INCOMPLETE; promotion HOLD; 1.0.0-rc.1 retained. See the manual-continuation evidence in regression-test-report.md.

## Stage 11-B gate closure — 2026-10-04 KST

**COMPLETE / USER_CONFIRMED manual acceptance; promotion PROCEED.** The user's Stage 11-C request confirms all remaining manual items are normal. Previous PARTIAL/HOLD snapshots remain historical evidence, not current release decisions. This is user attestation, not a new assistant observation. No detailed game/DPI/fatal/SmartScreen evidence is fabricated; unsigned warning remains documented. Prior automated/privacy/security PASS is retained, P0/P1 0.

- [x] Actual game detection / 5-role / Skill Detail / Back / game-end return — USER_CONFIRMED
- [x] Viewports / DPI / multi-monitor / Settings and persistence — USER_CONFIRMED
- [x] Installer lifecycle / portable / remaining manual acceptance — USER_CONFIRMED

## Stage 11-C final verification

- [x] Stage 11-B user acceptance and prior automated gates reviewed; no P0/P1
- [x] Version 1.0.0, Assembly/FileVersion 1.0.0.0, derived InformationalVersion
- [x] RC publish/installer/portable/manifest/SHA256SUMS archived separately before final build
- [x] Final Debug/Release and four tools build
- [x] Final publish/installer/portable/manifest/checksums validator
- [x] Install/launch/version/uninstall/RC upgrade/portable smoke
- [x] Final scans, RC preservation and integrity
- [x] Final documentation and git diff review

No automatic commit/push/tag/upload; user review precedes publication.

### Stage 11-C final outcome — 2026-10-04 KST

**COMPLETE / local 1.0.0 release preparation ready.** All final builds warning 0/error 0, full pipeline exit 0 and validator PASS. RC→Final upgrade, installed payload hashes, launch/exit/version, uninstall/user data preservation, clean binary reinstall and isolated portable PASS. All final hashes match after testing; RC 1,446 files unchanged and separately preserved. About/Settings control text 버전 1.0.0 verified with the final Release assembly. Final installer/EXE NotSigned; no actual SmartScreen no-warning claim. User data retained and final installation left closed. See Stage 11-C results in regression-test-report.md for exact hashes/evidence and distinction from a fresh empty-profile test.

- [x] Final notes/README/CHANGELOG/status/checklist/distribution/regression finalized
- [x] New P0/P1/P2/P3 0; existing downgrade P3 documented, limitations disclosed
- [x] No feature/UI/statistics/source change; only project Version metadata promoted
- [x] No commit/push/tag/upload or GitHub Release creation
- [ ] User review, commit, annotated v1.0.0 tag and separate GitHub publication
