# Windows 배포 및 설치

## 배포 기준

LoL Match Info `1.0.0`은 Windows x64용 .NET 10 WPF stable release 앱이다. 기본 배포는 `win-x64` self-contained다. 테스터 PC에 .NET Desktop Runtime을 미리 설치할 필요가 없고 초기 지원 부담이 작다는 장점이 패키지 크기와 업데이트 전송량 증가보다 중요하기 때문이다. ARM64와 다른 운영 체제는 Stage 10-C 범위가 아니다.

publish는 single-file, trimming, ReadyToRun을 사용하지 않는다. 앱은 읽기 전용 packaged dataset과 다수 champion asset을 외부 파일로 로드한다. WPF reflection/resource 안전성을 충분히 검증하지 않은 trimming이나, 현재 규모에서 이점이 입증되지 않은 ReadyToRun을 켜지 않는다. 여러 runtime 파일은 installer와 ZIP이 사용자에게 묶어서 제공한다. Release PDB는 설치 패키지와 portable ZIP에서 제외한다. 필요한 경우 별도의 내부 symbol archive를 만들어 접근을 제한한다.

제품명, assembly 이름과 버전의 source of truth는 `LoLMatchup.csproj`다. 사용자-facing 이름은 `LoL Match Info`, executable은 `LoLMatchInfo.exe`, 현재 SemVer는 `1.0.0`이다. About와 diagnostics는 assembly metadata를 읽는다.

## 사용자 설치

Inno Setup installer는 관리자 권한이 필요 없는 per-user 방식이며 기본 위치는 `%LocalAppData%\Programs\LoLMatchInfo`다. Start Menu shortcut은 항상 만들고 Desktop shortcut은 설치 화면에서 선택할 때만 만든다. 설치된 앱 목록에는 제품명, 버전, publisher와 uninstall 항목이 표시된다. 설치 자체와 packaged baseline 실행에는 네트워크가 필요하지 않다.

업그레이드 전 실행 중인 앱을 종료한다. 같은 AppId의 새 installer를 기존 설치 위에 실행해 앱 파일을 바꾼다. Inno Setup의 close-applications 처리를 사용하며 잠긴 파일 때문에 조용히 실패하도록 구성하지 않는다. release 담당자는 더 낮은 버전 installer를 배포하지 않아야 한다. 자동 downgrade 차단 및 앱 binary updater는 아직 구현하지 않았다.

uninstall은 설치 파일과 installer가 만든 shortcut/등록 정보만 제거한다. `%LocalAppData%\LoLMatchInfo`의 `settings.json`, `Logs`, `Data\Statistics`는 기본적으로 보존한다. 완전 삭제가 필요하면 uninstall 후 해당 폴더를 사용자가 직접 삭제한다. installer가 이 폴더를 패키징하거나 덮어쓰지 않는다.

portable ZIP은 installer 문제를 분리하고 별도 실행 테스트를 지원한다. 압축을 푼 폴더의 `LoLMatchInfo.exe`를 실행한다. portable 모드도 설정, 로그, 내려받은 dataset을 executable 옆이 아닌 `%LocalAppData%\LoLMatchInfo`에 저장한다.

LoL Match Info는 Riot Games의 공식 제품이 아니며 Riot Games의 보증이나 승인을 받았다는 의미가 없다. Stage 10-D의 제품 아이콘 Assets/AppIcon.ico를 executable과 installer에 유지한다. DPI/창 수동 acceptance는 Stage 11-B에서 사용자 확인으로 완료했다.

서명되지 않은 installer/executable에는 Windows SmartScreen 경고가 표시될 수 있다. 파일 출처와 `SHA256SUMS.txt`를 확인한다. Windows 보안 기능을 끄거나 우회하지 않는다.

## 개발자 publish

필수 도구는 .NET 10 SDK다. repository root의 PowerShell에서 다음 중 하나를 실행한다.

```powershell
dotnet publish .\LoLMatchup.csproj -c Release -r win-x64 --self-contained true -p:PublishProfile=WinX64
```

전체 local release 흐름은 다음 명령이다.

```powershell
.\scripts\build-release.ps1
```

현재 process policy가 local script 실행을 막는 PC에서는 system policy를 영구 변경하지 말고 다음 one-shot 명령을 사용한다.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-release.ps1
```

스크립트는 `artifacts` 내부 staging만 지우고 `git clean`을 사용하지 않는다. publish 후 baseline, manifest, champion metadata/icons, 개발 도구·source·PDB 부재, high-risk secret 및 개인정보 marker, 개발자 profile 경로를 검사한다. 이어 portable ZIP, 가능한 경우 installer, `SHA256SUMS.txt`, future binary-updater용 `release-manifest.json`을 만든다. manifest의 URL은 local build에서 비워 두며 statistics dataset manifest와 별개다. production hosting이나 GitHub Release 업로드는 수행하지 않는다.

## Installer 빌드

선택 기술은 Inno Setup 6이다. 개인 개발에 무료로 사용할 수 있고, 단순한 source 파일로 per-user x64 설치, optional shortcut, upgrade, uninstall과 향후 signing을 지원해 WiX/MSIX보다 이 프로젝트의 Windows 배포 요구에 가볍다.

1. 공식 Inno Setup 6 compiler를 machine-wide 또는 per-user로 설치한다.
2. `.\scripts\build-release.ps1`을 실행한다. script는 명시적 `-InstallerCompiler` 뒤에 `%ProgramFiles(x86)%\Inno Setup 6`, `%ProgramFiles%\Inno Setup 6`, `%LocalAppData%\Programs\Inno Setup 6`, PATH의 `ISCC.exe` 순서로 검사한다.
3. 다른 위치라면 `-InstallerCompiler <ISCC.exe 경로>`를 전달한다. 경로 탐지는 known environment folder를 사용하며 사용자 이름을 source에 넣지 않는다.

compiler가 없으면 script는 portable/checksum/manifest를 생성하고 installer만 `BLOCKED` 경고로 남긴다. source는 `installer/LoLMatchInfo.iss`, 예상 결과는 `artifacts/installer/LoLMatchInfo-1.0.0-win-x64-setup.exe`다.

Stage 10-C2에서는 per-user Inno Setup 6.7.3을 자동 탐지해 실제 installer build, non-admin install, Start Menu 실행, uninstall/user-data 보존, reinstall과 isolated 0.9.1 running-app upgrade를 검증했다. installer의 자동 downgrade 차단은 구현하지 않았으므로 release 담당자는 이전 version을 배포하지 않는다.

## Signing readiness

현재 배포 상태는 `UNSIGNED`다. 인증서, `.pfx`, private key, password는 repository나 artifacts에 두지 않는다. 향후 release pipeline에서 publish 직후 `LoLMatchInfo.exe`(필요하면 함께 배포하는 DLL)를 먼저 `signtool sign /fd SHA256 /tr <RFC3161 URL> /td SHA256 ...`으로 서명하고, Inno Setup으로 패키징한 뒤 installer도 같은 방식으로 서명한다. 실제 인증서 선택과 secret 전달은 별도 보안 배포 단계에서 구성한다.

## 업데이트와 데이터 분리

앱 binary의 초기 업데이트 방식은 새 installer를 수동으로 다시 실행하는 것이다. `release-manifest.json`은 향후 GitHub Releases 또는 static HTTPS hosting에 연결할 수 있는 최소 기반일 뿐이며, 자동 updater UI나 server는 없다.

기존 Cloudflare statistics updater는 별도 기능이다. packaged baseline은 install directory에서 읽기 전용으로 사용하고 update는 `%LocalAppData%\LoLMatchInfo\Data\Statistics`에 원자적으로 설치된다. 네트워크 실패 시 기존 local dataset 또는 packaged baseline으로 복구 가능해야 한다.

## Release 검증

`scripts/validate-release.ps1`은 publish directory만 읽는다. 실제 release에서는 다음도 isolated test account/path에서 수행한다.

- project 밖의 임의 working directory에서 executable 실행, Home/Waiting/Settings/Data/About 확인
- clean first run과 기존 `%LocalAppData%\LoLMatchInfo` 보존 실행
- production statistics update가 UpToDate이며 실패 시 기존 dataset으로 복구되는지 확인
- `%LocalAppData%\LoLMatchInfo\Logs` 기록 및 install directory write 부재 확인
- installer/Start Menu 실행, uninstall, reinstall, 구버전→신버전 upgrade 확인
- installer와 ZIP archive 재검사, SHA-256 확인, Windows Defender 격리 여부 확인

시스템 네트워크나 보안 기능을 테스트 목적으로 끄지 않는다. 실제 수행하지 않은 GUI/install 검사는 PASS로 기록하지 않는다.


## Stage 11-A RC version policy (2026-10-04)

Version/InformationalVersion은 1.0.0-rc.1이며 Windows Assembly/FileVersion은 1.0.0.0이다. FileVersion과 InformationalVersion은 csproj 참조로 파생된다. 기존 About/Settings는 InformationalVersion을 읽어 suffix를 보존한다. Inno AppVersion/AppVerName/파일명은 RC 문자열, VersionInfoVersion은 숫자 AssemblyVersion을 받는다. prerelease를 거부하던 build script를 제한된 rc.N 형식으로 확장했다.

Installer/portable 이름은 LoLMatchInfo-1.0.0-rc.1-win-x64-setup.exe / LoLMatchInfo-1.0.0-rc.1-win-x64-portable.zip. Release manifest는 product/version/configuration/runtimeIdentifier/architecture/publishedAtUtc, 모든 artifact 이름·size·SHA256, packagedDataset, skillData를 기록한다. SHA256SUMS에는 installer/ZIP/manifest가 포함된다. manifest가 자기 자신의 hash를 포함하지 않아 순환을 피한다.

Downgrade 차단 부재는 기존 P3로 유지한다. 통계 updater는 독립적으로 revision downgrade를 거부하고 installer는 사용자 데이터 폴더를 덮어쓰지 않는다. 배포 담당자는 이전 binary를 재배포하지 않는다. 호환성 위험이 전혀 없다고 보장하지 않으며 upgrade/downgrade acceptance는 Stage 11-B에서 확인한다. 현재 기능 동결 범위의 RC blocker로 올리지 않는다.

Unsigned RC 유지. 인증서/self-signed 배포 또는 SmartScreen 우회는 추가하지 않았다. 테스트 실행만으로 인터넷 다운로드 시의 SmartScreen PASS를 주장하지 않는다.

Final promotion 변경 목록/제안 tag는 release-checklist.md 참조. app binary와 remote statistics 배포는 독립이며 16.20 배포가 app version 재빌드를 요구하지 않는다.

## 1.0.0 final release policy (2026-10-04)

현재 Version/InformationalVersion=1.0.0, Assembly/FileVersion=1.0.0.0. 기존 source-of-truth와 About/Settings binding을 유지한다. Installer는 display version 1.0.0과 numeric version 1.0.0.0을 받는다. 산출물은 LoLMatchInfo-1.0.0-win-x64-setup.exe / LoLMatchInfo-1.0.0-win-x64-portable.zip이며 새 manifest/checksum을 생성한다. RC 전체 release tree는 artifacts/rc/1.0.0-rc.1에 별도 보존한다. build-release.ps1은 staging을 재생성하므로 재실행 전에 기존 보관할 산출물을 별도 archive해야 한다. Unsigned/SmartScreen 및 downgrade P3 정책은 유지한다. 위 Stage 11-A 섹션은 RC 당시 이력이다.
