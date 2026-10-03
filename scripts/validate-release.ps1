[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $PublishDirectory
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
    '.env'
)
$allFiles = @(Get-ChildItem -LiteralPath $publishPath -Recurse -File)
foreach ($file in $allFiles) {
    if ($forbiddenNames -contains $file.Name) { $errors.Add("Forbidden file: $($file.FullName)") }
    if ($file.Extension -in '.cs', '.xaml', '.csproj', '.sln', '.pdb') {
        $errors.Add("Development/source artifact: $($file.FullName)")
    }
}

$highRiskPatterns = @('RGAPI-', 'RIOT_API_KEY')
$textExtensions = @('.json', '.config', '.xml', '.txt', '.md', '.ini', '.yml', '.yaml', '.ps1', '.iss')
$privacyPatterns = @('Authorization:', 'PUUID', 'riotId', 'summonerName', 'matchId')
$developerPathPattern = '(?i)[A-Z]:\\Users\\[^\\]+'
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

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    throw "Release validation failed with $($errors.Count) issue(s)."
}

$totalBytes = ($allFiles | Measure-Object -Property Length -Sum).Sum
Write-Host "Release validation passed: $($allFiles.Count) files, $totalBytes bytes, $($icons.Count) champion icons."
