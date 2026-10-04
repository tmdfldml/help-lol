# LoL Match Info 1.0.0-rc.1 — Release Candidate

Windows x64 출시 후보. Stage 10 기능을 동결했으며 정식 1.0 승인은 Stage 11-B 검증 이후입니다.

## 제공 기능

- InGame 자동 감지와 TOP/JUNGLE/MID/ADC/SUPPORT 5역할 상성.
- 승리·레벨 선행·15분 전 상대 역할 솔로킬 통계 해석, Team Summary, Matchup Detail.
- 상대 초상화에서 Champion QWER Skill Detail 열기 및 돌아가기.
- 173개 챔피언, 692개 QWER 로컬 설명·아이콘·대기시간. 충전형, 토글, 자원 조건, 지속 효과에 맞는 표시. 스킬 화면은 네트워크를 요청하지 않습니다.
- 정적 통계 업데이트, 창/설정 저장, 진단·로그·오류 처리.
- 관리자 권한이 필요 없는 installer와 portable ZIP.

## 알려진 제한

- 실제 역할별 스킬 순서 통계는 아직 제공하지 않습니다.
- 일부 특수 스킬은 Riot 정적 데이터에서 정확한 재충전 시간을 제공하지 않아 숫자 대신 “정보 없음” 또는 메커니즘 설명을 표시합니다. 아직 확인되지 않은 특수 대기시간도 있으며 변신·무기별 모든 파생 스킬을 포함하지 않습니다.
- 현재 게임 패치를 연결 정보에서 확인할 수 없으면 패치 일치 여부는 확인 불가로 표시합니다.
- 서명되지 않은 RC이며 Windows SmartScreen 경고가 표시될 수 있습니다.
- installer는 낮은 버전 설치를 기술적으로 차단하지 않습니다. 이전 버전 재설치를 피하고 배포 담당자가 버전 순서를 관리합니다.
- 앱 자동 업데이트는 제공하지 않습니다. 통계 업데이트는 앱 버전과 별개입니다.

기본/production 통계: KR Ranked Solo/Duo 16.17, revision 1788854099077. 스킬: Riot Data Dragon 16.17.1. 최신 게임과 일치한다고 보장하지 않습니다. 16.18은 배포하지 않으며 16.20 준비 후 별도 검증·배포할 계획입니다.

## 설치 및 업그레이드

1. 실행 중인 앱을 종료합니다.
2. `LoLMatchInfo-1.0.0-rc.1-win-x64-setup.exe`를 실행합니다.
3. 기존 설정·로그·내려받은 통계는 보존됩니다. Portable은 ZIP 전체를 풀고 `LoLMatchInfo.exe`를 실행합니다.

문제 보고에는 앱/Windows 버전, 시각, 재현 절차를 포함하세요. 로그: `%LocalAppData%\LoLMatchInfo\Logs`. 공유 전 필요한 파일의 개인정보 유무를 확인하세요.
