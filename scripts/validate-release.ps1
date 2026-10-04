[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $PublishDirectory,
    [string] $ReleaseDirectory
)

$ErrorActionPreference = 'Stop'
$publishPath = [IO.Path]::GetFullPath($PublishDirectory)
if (-not (Test-Path -LiteralPath $publishPath -PathType Container)) {
    throw "Publish directory does not exist: $publishPath"
}

$errors = [Collections.Generic.List[string]]::new()
function Require-Path([string] $relativePath, [string] $description) {
    if (-not (Test-Path -LiteralPath (Join-Path $publishPath $relativePath))) {
        $errors.Add("Missing $description ($relativePath)")
    }
}

Require-Path 'LoLMatchInfo.exe' 'application executable'
Require-Path 'Data\Statistics\master_plus.json' 'packaged statistics baseline'
Require-Path 'Data\Statistics\manifest.json' 'packaged statistics manifest'
Require-Path 'Data\Champions\champions.ko.json' 'champion metadata'
Require-Path 'Data\ChampionSkills\manifest.json' 'skill data manifest'
Require-Path 'Data\ChampionSkills\orders.json' 'optional skill order dataset'
$skillMetadata = Get-Content -Raw -Encoding UTF8 (Join-Path $publishPath 'Data\Champions\champions.ko.json') | ConvertFrom-Json
foreach ($champion in $skillMetadata.champions) {
    Require-Path "Data\Champions\Icons\$($champion.id).png" 'champion icon'
    Require-Path "Data\ChampionSkills\$($champion.id).json" 'champion skills'
    foreach ($key in @('Q','W','E','R')) { Require-Path "Data\ChampionSkills\Icons\$($champion.id)_$key.png" 'spell icon' }
}

$icons = Get-ChildItem -LiteralPath (Join-Path $publishPath 'Data\Champions\Icons') -Filter '*.png' -File -ErrorAction SilentlyContinue
if (-not $icons) { $errors.Add('Missing champion icon assets') }

$forbiddenNames = @(
    'StatsCollector.exe',
    'PublishStatisticsDataset.exe',
    'AnalyzeStatisticsDataset.exe',
    'ChampionSkillDataCollector.exe',
    'ChampionSkillDataCollector.dll',
    'RegressionScratch.exe',
    'RegressionScratch.dll',
    '.env'
)
$allFiles = @(Get-ChildItem -LiteralPath $publishPath -Recurse -File)
foreach ($file in $allFiles) {
    if ($file.FullName -match '(?i)[\\/](Tools|Fixtures|Development|skill-source-cache)[\\/]' -or $file.Name -match '(?i)^(StatsCollector|PublishStatisticsDataset|AnalyzeStatisticsDataset|ChampionSkillDataCollector|RegressionScratch)\.') {
        $errors.Add("Development artifact: $($file.FullName)")
    }
    if ($forbiddenNames -contains $file.Name) { $errors.Add("Forbidden file: $($file.FullName)") }
    if ($file.Name -like '.env*' -or $file.Name -match '(?i)credentials|secrets|fixture' -or $file.Extension -in '.pfx','.pem','.key') { $errors.Add("Sensitive/development file: $($file.FullName)") }
    if ($file.Extension -in '.cs', '.xaml', '.csproj', '.sln', '.pdb') {
        $errors.Add("Development/source artifact: $($file.FullName)")
    }
}

$highRiskPatterns = @('RGAPI-', 'RIOT_API_KEY')
$textExtensions = @('.json', '.config', '.xml', '.txt', '.md', '.ini', '.yml', '.yaml', '.ps1', '.iss')
$privacyPatterns = @('Authorization:', 'X-Riot-Token', 'PUUID', 'riotId', 'summonerName', 'matchId', 'allPlayers', 'activePlayer')
$developerPathPattern = '(?i)[A-Z]:\\(Users\\[^\\]+|Projects\\)'
foreach ($file in $allFiles | Where-Object { $textExtensions -contains $_.Extension }) {
    $content = [IO.File]::ReadAllText($file.FullName)
    foreach ($pattern in ($highRiskPatterns + $privacyPatterns)) {
        if ($content.IndexOf($pattern, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            $errors.Add("Sensitive/privacy marker '$pattern': $($file.FullName)")
        }
    }
    if ($content -match $developerPathPattern) {
        $errors.Add("Developer profile path: $($file.FullName)")
    }
}

foreach ($file in $allFiles | Where-Object { $_.Extension -in '.exe', '.dll' }) {
    $bytes = [IO.File]::ReadAllBytes($file.FullName)
    $ascii = [Text.Encoding]::ASCII.GetString($bytes)
    $unicode = [Text.Encoding]::Unicode.GetString($bytes)
    foreach ($pattern in ($highRiskPatterns + @('DeveloperTestWindow'))) {
        if ($ascii.IndexOf($pattern, [StringComparison]::OrdinalIgnoreCase) -ge 0 -or
            $unicode.IndexOf($pattern, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            $errors.Add("Forbidden binary marker '$pattern': $($file.FullName)")
        }
    }
    if ($ascii -match $developerPathPattern -or $unicode -match $developerPathPattern) {
        $errors.Add("Developer profile path in binary: $($file.FullName)")
    }
}

$skillManifest = Get-Content -Raw -Encoding UTF8 (Join-Path $publishPath 'Data\ChampionSkills\manifest.json') | ConvertFrom-Json
$skillFiles = @(Get-ChildItem (Join-Path $publishPath 'Data\ChampionSkills') -Filter '*.json' -File | Where-Object Name -NotIn 'manifest.json','orders.json')
$spellCount = 0
foreach ($file in $skillFiles) { $spellCount += @((Get-Content -Raw -Encoding UTF8 $file.FullName | ConvertFrom-Json).spells).Count }
if ($skillFiles.Count -ne 173 -or $spellCount -ne 692 -or $skillManifest.championCount -ne 173 -or $skillManifest.spellCount -ne 692) { $errors.Add('Skill data counts must be 173 champions / 692 spells for this RC.') }
$datasetManifest = Get-Content -Raw -Encoding UTF8 (Join-Path $publishPath 'Data\Statistics\manifest.json') | ConvertFrom-Json
$datasetFile = Get-Item (Join-Path $publishPath 'Data\Statistics\master_plus.json')
if ($datasetFile.Length -ne $datasetManifest.fileSizeBytes -or (Get-FileHash $datasetFile.FullName -Algorithm SHA256).Hash -ne $datasetManifest.sha256) { $errors.Add('Packaged dataset size/hash mismatch.') }
if ($ReleaseDirectory) {
    $releaseRoot = [IO.Path]::GetFullPath($ReleaseDirectory)
    $releaseManifest = Get-Content -Raw -Encoding UTF8 (Join-Path $releaseRoot 'release-manifest.json') | ConvertFrom-Json
    $exeVersion = (Get-Item (Join-Path $publishPath 'LoLMatchInfo.exe')).VersionInfo.ProductVersion
    if ($releaseManifest.configuration -ne 'Release' -or $releaseManifest.runtimeIdentifier -ne 'win-x64' -or $releaseManifest.architecture -ne 'x64' -or -not $releaseManifest.publishedAtUtc -or @($releaseManifest.artifacts).Count -lt 1) { $errors.Add('Release manifest required fields mismatch.') }
    if ($releaseManifest.version -ne $exeVersion -or $releaseManifest.product -ne 'LoL Match Info' -or $releaseManifest.packagedDataset.datasetRevision -ne $datasetManifest.datasetRevision -or $releaseManifest.skillData.patch -ne $skillManifest.patch) { $errors.Add('Release manifest metadata mismatch.') }
    foreach ($artifact in $releaseManifest.artifacts) {
        $folder = if ($artifact.fileName -like '*setup.exe') { 'installer' } else { 'portable' }
        $file = Get-Item -LiteralPath (Join-Path (Join-Path $releaseRoot $folder) $artifact.fileName)
        if ($file.Length -ne $artifact.size -or (Get-FileHash $file.FullName -Algorithm SHA256).Hash -ne $artifact.sha256) { $errors.Add('Release artifact size/hash mismatch.') }
        if ($file.Extension -eq '.zip') {
            Add-Type -AssemblyName System.IO.Compression.FileSystem
            $zip = [IO.Compression.ZipFile]::OpenRead($file.FullName)
            try {
                $entries = @($zip.Entries | Where-Object { $_.Name })
                $entryMap = [Collections.Generic.Dictionary[string,IO.Compression.ZipArchiveEntry]]::new([StringComparer]::Ordinal)
                foreach ($entry in $entries) {
                    $entryName = $entry.FullName.Replace('\','/')
                    if ($entryMap.ContainsKey($entryName)) { $errors.Add("Duplicate portable ZIP entry: $entryName") }
                    else { $entryMap.Add($entryName, $entry) }
                }
                if ($entries.Count -ne $allFiles.Count) { $errors.Add('Portable ZIP file count mismatch.') }
                foreach ($publishedFile in $allFiles) {
                    $relative = $publishedFile.FullName.Substring($publishPath.Length + 1).Replace('\','/')
                    $entry = if ($entryMap.ContainsKey($relative)) { $entryMap[$relative] } else { $null }
                    if (-not $entry -or $entry.Length -ne $publishedFile.Length) { $errors.Add("Portable ZIP missing/size mismatch: $relative"); continue }
                    $stream = $entry.Open(); $sha = [Security.Cryptography.SHA256]::Create()
                    try { $hash = [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','') }
                    finally { $stream.Dispose(); $sha.Dispose() }
                    if ($hash -ne (Get-FileHash $publishedFile.FullName -Algorithm SHA256).Hash) { $errors.Add("Portable ZIP content mismatch: $relative") }
                }
            } finally { $zip.Dispose() }
        }
    }
    $checksumNames = [Collections.Generic.List[string]]::new()
    foreach ($line in Get-Content (Join-Path $releaseRoot 'SHA256SUMS.txt')) {
        if ($line -notmatch '^([a-f0-9]{64})  ([^\\/]+)  (\d+) bytes$') { $errors.Add('Invalid checksum line.'); continue }
        $expectedHash = $Matches[1]; $name = $Matches[2]; $expectedSize = [long]$Matches[3]
        if ($checksumNames.Contains($name)) { $errors.Add('Duplicate checksum entry.') }
        $checksumNames.Add($name)
        $folder = if ($name -like '*setup.exe') { 'installer' } elseif ($name -like '*portable.zip') { 'portable' } else { '' }
        $file = Get-Item -LiteralPath (Join-Path (Join-Path $releaseRoot $folder) $name)
        if ($file.Length -ne $expectedSize -or (Get-FileHash $file.FullName -Algorithm SHA256).Hash -ne $expectedHash) { $errors.Add('Checksum verification failed.') }
    }
    foreach ($name in (@($releaseManifest.artifacts | ForEach-Object fileName) + @('release-manifest.json'))) {
        if (-not $checksumNames.Contains($name)) { $errors.Add("Missing checksum: $name") }
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    throw "Release validation failed with $($errors.Count) issue(s)."
}

$totalBytes = ($allFiles | Measure-Object -Property Length -Sum).Sum
Write-Host "Release validation passed: $($allFiles.Count) files, $totalBytes bytes, $($icons.Count) champion icons."
