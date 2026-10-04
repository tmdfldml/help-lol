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

## RC verification — Stage 11-B

- [ ] 실제 게임: 감지 → 5역할 → 상대 초상화 → QWER → Back → 종료/Waiting
- [ ] Team Summary / Matchup Detail / Settings / About 직접 GUI 확인
- [ ] 통계 업데이트 / 실패 fallback / 재시작 후 activation
- [ ] install / Start Menu / uninstall / user-data preservation / upgrade / reinstall 전체 lifecycle
- [ ] clean first run / install directory write 부재
- [ ] DPI / portrait·landscape / multi-monitor / keyboard / screen reader
- [ ] 로그 retention 및 실제 fatal: critical log / crash file / 한국어 안내 / 종료
- [ ] 실제 내려받은 파일의 Defender/SmartScreen 경고 여부 기록 (보안 설정 변경 없음)

## Final 1.0 promotion

- [ ] Stage 11-B 검증 및 제한 수용, P0/P1 없음
- [ ] LoLMatchup.csproj Version만 1.0.0으로 변경; AssemblyVersion=1.0.0.0 유지
- [ ] README / CHANGELOG / PROJECT_STATUS / release notes / checklist / distribution / regression report 갱신
- [ ] fresh pipeline 재실행 (이름·hash·manifest 재생성; RC artifacts 재사용 금지)
- [ ] code signing 가능 여부 결정; 인증서 없이 임의 서명 금지
- [ ] 최종 release 승인 후 tag / upload를 별도 수행

RC tag 제안: `git tag -a v1.0.0-rc.1 -m "LoL Match Info 1.0.0-rc.1"`
Final tag 제안: `git tag -a v1.0.0 -m "LoL Match Info 1.0.0"`
먼저 변경을 검토·커밋한 뒤 사용한다. 이번 작업은 commit/tag/push/upload를 수행하지 않는다.

통계 16.20 준비는 app RC와 독립이다. 16.18은 publish하지 않으며 신규 통계를 별도 검증·배포해도 app version을 올릴 필요가 없다.
