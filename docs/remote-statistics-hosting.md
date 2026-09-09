# Remote statistics hosting

LoL Matchup은 private source repository와 public statistics distribution을 분리한다. 클라이언트는 인증 없이 읽을 수 있는 Cloudflare Workers Static Assets HTTPS endpoint만 사용하며, Riot API key나 Cloudflare credential을 포함하지 않는다.

## Production endpoint

고정 manifest URL은 다음과 같다.

```text
https://lol-match-info-stats.01024783207a.workers.dev/statistics/master-plus/manifest.json
```

앱은 manifest의 같은-directory 상대 `downloadUrl`을 기준 URL에 결합해 revision-specific dataset을 찾는다. 일반 사용자는 별도 설정이 필요 없다. 개발 중에는 `LOLMATCHINFO_STATS_MANIFEST_URL` 환경 변수로 endpoint를 명시적으로 override할 수 있다. 유효하지 않은 명시적 override는 production URL로 조용히 대체하지 않고 해당 process의 updater 구성을 실패시킨다.

Release는 public manifest와 dataset 모두 HTTPS만 허용한다. Debug에 한해 updater integration test를 위한 loopback HTTP(`localhost`, `127.0.0.1`)를 허용한다. TLS certificate validation 우회는 사용하지 않는다.

## Public artifact layout

공개 root에는 통계 배포 파일만 둔다.

```text
/statistics/master-plus/
  manifest.json
  master_plus.{datasetRevision}.json
```

허용되는 공개 내용은 aggregate matchup statistics와 dataset metadata뿐이다. API key, `.env`, source code, collector state, match ID, PUUID, Riot ID, summoner name, raw Match Detail 및 raw Timeline은 업로드하지 않는다.

`manifest.json`은 최신 production revision을 가리키는 current pointer다. `master_plus.{datasetRevision}.json`은 immutable artifact이며, 공개된 같은 revision 파일을 다른 bytes로 덮어쓰지 않는다. 수정이 필요하면 새 revision을 발행한다.

## Cache policy

Workers Static Assets deployment root의 `_headers`는 다음 정책을 사용한다.

```text
/statistics/master-plus/manifest.json
  Cache-Control: no-cache, max-age=0, must-revalidate

/statistics/master-plus/master_plus.{datasetRevision}.json
  Cache-Control: public, max-age=31536000, immutable
```

두 응답은 `application/json`으로 제공된다. WPF `HttpClient`는 browser CORS 제약을 받지 않으므로 별도 CORS 설정을 필수로 두지 않는다.

## Publish and deployment workflow

1. Collector가 새 valid aggregate dataset을 생성한다.
2. Publisher로 packaged baseline과 revision artifact를 생성한다.
3. Cloudflare deployment package를 생성하고 allowlist, secret/privacy, size, SHA 및 local HTTP 검증을 통과시킨다.
4. 새 revision dataset을 먼저 Workers Static Assets에 업로드한다.
5. 공개 dataset URL에서 GET, byte size 및 SHA-256을 확인한다.
6. `manifest.json`을 마지막에 업로드한다.
7. 공개 manifest를 다시 GET해 revision과 cache header를 확인한다.
8. WPF Settings → Data → 업데이트 확인으로 최종 client 동작을 확인한다.

```powershell
dotnet run --project .\Tools\PublishStatisticsDataset\PublishStatisticsDataset.csproj
dotnet run --project .\Tools\PublishStatisticsDataset\PublishStatisticsDataset.csproj -- --prepare-cloudflare-pages
```

기존 `--prepare-cloudflare-pages` 명령 이름은 Stage 9-5B-1에서 만든 static deployment package 명칭을 유지한다. 생성되는 `_headers` 및 `statistics/` root는 현재 Cloudflare Workers Static Assets 배포에도 그대로 사용된다.

생성 경로:

```text
Tools/PublishStatisticsDataset/CloudflarePagesDeploy/
  _headers
  statistics/
    master-plus/
      manifest.json
      master_plus.{datasetRevision}.json
```

배포 시 `CloudflarePagesDeploy` 폴더 이름이 public URL path에 들어가면 안 된다. 그 폴더 안의 `_headers`와 `statistics`가 public root 바로 아래에 놓여야 한다.

배포 순서는 반드시 **dataset 먼저, manifest 마지막**이다. manifest를 먼저 바꾸면 아직 존재하지 않거나 검증되지 않은 dataset을 client가 요청할 수 있다.

## Verification

Updater의 실제 download, size/SHA, shared dataset validation 및 manifest↔dataset metadata 일치를 read-only temporary install로 확인한다.

```powershell
dotnet run --project .\Tools\PublishStatisticsDataset\PublishStatisticsDataset.csproj -- --verify-remote https://lol-match-info-stats.01024783207a.workers.dev/statistics/master-plus/manifest.json
```

로컬 synthetic/loopback 회귀 검증:

```powershell
dotnet run --project .\Tools\PublishStatisticsDataset\PublishStatisticsDataset.csproj -- --self-test
```

수동 검증이 필요하면 manifest를 받은 뒤 그 `downloadUrl`을 manifest URL 기준으로 resolve하고, dataset을 내려받아 다음을 확인한다.

```powershell
$manifestUri = [Uri]'https://lol-match-info-stats.01024783207a.workers.dev/statistics/master-plus/manifest.json'
$manifest = Invoke-RestMethod -Uri $manifestUri
$datasetUri = [Uri]::new($manifestUri, $manifest.downloadUrl)
Invoke-WebRequest -Uri $datasetUri -OutFile '.\remote-dataset.json'
Get-Item '.\remote-dataset.json' | Select-Object Length
Get-FileHash '.\remote-dataset.json' -Algorithm SHA256
```

검증용 다운로드 파일에는 aggregate 통계만 있어야 하지만 작업 후 안전하게 정리한다. manifest SHA와 실제 downloaded bytes의 SHA가 일치해야 한다.

## Client behavior and failure policy

- 앱 시작은 네트워크에 의존하지 않는다. update check는 Settings에서 사용자가 요청할 때만 실행한다.
- valid LocalAppData update가 있으면 packaged baseline보다 우선하며, invalid local file이면 packaged baseline으로 fallback한다.
- remote revision이 같으면 `최신 상태`, 더 크면 update available, 더 작으면 downgrade 금지다.
- 같은 revision인데 content identity(SHA, size 또는 핵심 metadata)가 다르면 immutable revision conflict로 거부한다.
- network, DNS, HTTP, manifest 또는 dataset 검증 실패는 현재 active dataset을 삭제하거나 교체하지 않는다.
- 다운로드와 설치가 성공해도 현재 session은 hot reload하지 않으며 앱 재시작 후 적용한다.

## Rollback and operations notes

잘못된 manifest는 이전 valid revision을 가리키도록 되돌릴 수 있다. 다만 이미 더 높은 revision을 설치한 client는 downgrade 방지 때문에 자동 rollback되지 않는다. 이미 공개된 동일 revision dataset을 교체하지 말고 수정본을 새 revision으로 발행한다.

Publisher revision은 dataset `generatedAtUtc`의 Unix milliseconds를 사용한다. Publisher machine clock이나 upstream timestamp가 이전 발행보다 과거로 이동하면 monotonicity가 깨질 수 있으므로 운영자는 새 manifest를 올리기 전에 기존 production revision보다 큰지 확인해야 한다. 반복 운영이 안정화되기 전에는 credential-dependent 자동 업로드를 추가하지 않는다.
