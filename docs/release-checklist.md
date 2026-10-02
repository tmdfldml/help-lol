# LoL Match Info Release Checklist

각 항목을 `PASS`, `PARTIAL`, `MANUAL`, `BLOCKED` 중 하나로 기록하고 증거 경로/명령을 남긴다.

최근 전체 실행: Stage 10-C2(2026-09-11)는 installer build/install/Start Menu startup/uninstall/user-data preservation/reinstall/running-app upgrade까지 PASS했다. About 및 Settings Data의 직접 GUI 조작은 native automation 부재로 PARTIAL, signing은 UNSIGNED BETA, 최종 icon은 remaining branding polish다. 아래 목록은 다음 release에서도 새로 실행한다.

## Source and version

- [ ] working tree와 보존할 기존 변경 확인
- [ ] `LoLMatchup.csproj`의 SemVer, Product, Company, FileVersion, InformationalVersion 확인
- [ ] About/diagnostics/executable/installer version 일치 확인
- [ ] release notes 작성 및 알려진 제한 갱신

## Build and publish

- [ ] Debug/Release WPF와 3개 tool project가 warning 0/error 0
- [ ] fresh `artifacts/publish/win-x64` self-contained publish
- [ ] baseline dataset/manifest와 champion metadata/icons 포함
- [ ] source, PDB, DeveloperTestWindow, fixtures, collector/publisher/analyzer 제외
- [ ] `scripts/validate-release.ps1` PASS
- [ ] publish output 크기 기록

## Package and security

- [ ] Inno Setup installer와 portable ZIP 생성
- [ ] installer/ZIP 파일명과 non-zero 크기 확인
- [ ] ZIP file list 수동 확인
- [ ] secret, credential, player identifier, developer path scan PASS
- [ ] SHA-256와 byte size를 `SHA256SUMS.txt`에서 재확인
- [ ] release manifest version/hash/size 일치, URL은 배포 위치 확정 후 설정
- [ ] signing 완료 또는 `UNSIGNED BETA` 명시
- [ ] Defender/SmartScreen 결과 기록; 보안 설정은 변경하지 않음

## Isolated runtime

- [ ] project 외부 및 임의 current working directory에서 startup
- [ ] clean first run에서 LocalAppData/settings/log 생성
- [ ] 기존 settings/log/downloaded dataset 보존
- [ ] Home, Waiting, Settings, Data, About smoke
- [ ] production statistics check가 UpToDate
- [ ] controlled update failure에서 기존/local packaged fallback
- [ ] no-game 관찰 중 crash/polling spam 없음
- [ ] install directory에 runtime write 없음

## Installer lifecycle

- [ ] per-user install, UAC 불필요, 실제 install path 기록
- [ ] Start Menu shortcut 실행
- [ ] optional Desktop shortcut 동작
- [ ] Add/Remove Programs metadata 확인
- [ ] 실행 중 upgrade가 종료 요청/명확한 오류 제공
- [ ] 이전 beta 위에 upgrade 후 앱 파일 갱신 및 user data 보존
- [ ] accidental downgrade 검토
- [ ] uninstall 후 app/shortcut/entry 제거, LocalAppData 보존
- [ ] reinstall startup

## Release handoff

- [ ] installer, ZIP, checksums, manifest, release notes 준비
- [ ] 실제 remote upload/deployment는 별도 승인 후 수행
- [ ] `git diff --check`, `git status`, 변경 파일 구분
- [ ] 새 P0/P1 없음; 나머지 issue/수동 항목 기록
