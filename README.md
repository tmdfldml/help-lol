# LoL Match Info

League of Legends 정글 유저를 위한 Windows 전용 라인 상성 정보 앱.

현재 버전: **1.0.0 stable release**. 검증된 출시 후보의 기능을 유지하며 정식 버전으로 승격했습니다.

C#, .NET 10, WPF. Windows x64 self-contained 배포에는 Runtime 별도 설치가 필요하지 않습니다. 개발에는 Windows와 .NET 10 SDK, installer 빌드에는 Inno Setup 6이 필요합니다.

`LoLMatchInfo-1.0.0-win-x64-setup.exe`를 실행하거나 portable ZIP 전체를 풀고 `LoLMatchInfo.exe`를 실행하세요. 게임 시작 시 5역할 상성, Team Summary, Matchup Detail 및 상대 QWER 참조를 확인합니다. 사용자 데이터는 `%LocalAppData%\LoLMatchInfo`에 저장됩니다.

```powershell
dotnet build .\LoLMatchup.csproj
dotnet build .\LoLMatchup.csproj -c Release
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-release.ps1
```

[1.0.0 release notes](docs/release-notes-1.0.0.md), [배포 안내](docs/windows-distribution.md), [검증 checklist](docs/release-checklist.md). 서명되지 않은 실행 파일이며 SmartScreen 경고가 발생할 수 있습니다. Riot Games 공식 제품이 아닙니다.
