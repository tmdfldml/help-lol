[CmdletBinding()]
param(
    [switch] $SkipInstaller,
    [string] $InstallerCompiler
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$projectPath = Join-Path $repoRoot 'LoLMatchup.csproj'
$artifactsRoot = Join-Path $repoRoot 'artifacts'
$publishPath = Join-Path $artifactsRoot 'publish\win-x64'
$installerPath = Join-Path $artifactsRoot 'installer'
$portablePath = Join-Path $artifactsRoot 'portable'

[xml] $project = Get-Content -LiteralPath $projectPath
$versionNode = $project.SelectSingleNode('/Project/PropertyGroup/Version')
$version = if ($versionNode) { $versionNode.InnerText.Trim() } else { '' }
if ($version -notmatch '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(-rc\.[1-9]\d*)?$') { throw 'Version must be MAJOR.MINOR.PATCH or MAJOR.MINOR.PATCH-rc.N.' }
$fileVersion = $project.SelectSingleNode('/Project/PropertyGroup/AssemblyVersion').InnerText.Trim()
if ($fileVersion -ne (($version -split '-')[0] + '.0')) { throw 'AssemblyVersion must match the release version core.' }

foreach ($path in @($publishPath, $installerPath, $portablePath)) {
    $fullPath = [IO.Path]::GetFullPath($path)
    if (-not $fullPath.StartsWith($artifactsRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clean a path outside artifacts: $fullPath"
    }
    if (Test-Path -LiteralPath $fullPath) { Remove-Item -LiteralPath $fullPath -Recurse -Force }
    New-Item -ItemType Directory -Path $fullPath | Out-Null
}

& dotnet publish $projectPath -c Release -r win-x64 --self-contained true -p:PublishProfile=WinX64 -p:PublishDir="$publishPath\"
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE." }

& (Join-Path $PSScriptRoot 'validate-release.ps1') -PublishDirectory $publishPath

$portableName = "LoLMatchInfo-$version-win-x64-portable.zip"
$portableFile = Join-Path $portablePath $portableName
Compress-Archive -Path (Join-Path $publishPath '*') -DestinationPath $portableFile -CompressionLevel Optimal

$installerBuilt = $false
if (-not $SkipInstaller) {
    $isccCandidates = [Collections.Generic.List[string]]::new()
    if ($InstallerCompiler) { $isccCandidates.Add($InstallerCompiler) }
    if (${env:ProgramFiles(x86)}) { $isccCandidates.Add((Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe')) }
    if ($env:ProgramFiles) { $isccCandidates.Add((Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe')) }
    if ($env:LOCALAPPDATA) { $isccCandidates.Add((Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')) }

    $iscc = $isccCandidates |
        Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
        Select-Object -First 1
    if (-not $iscc) {
        $iscc = (Get-Command ISCC.exe -ErrorAction SilentlyContinue).Source
    }
    if ($iscc) {
        $iscc = [IO.Path]::GetFullPath($iscc)
        Write-Host "Inno Setup compiler: $iscc"
        & $iscc "/DMyAppVersion=$version" "/DMyAppFileVersion=$fileVersion" "/DPublishDir=$publishPath" "/DOutputDir=$installerPath" (Join-Path $repoRoot 'installer\LoLMatchInfo.iss')
        if ($LASTEXITCODE -ne 0) { throw "Inno Setup failed with exit code $LASTEXITCODE." }
        $installerBuilt = $true
    } else {
        Write-Warning 'Inno Setup 6 compiler not found. Installer build is BLOCKED; portable release remains available.'
    }
}

$releaseFiles = @(Get-ChildItem -LiteralPath $portablePath -File)
if ($installerBuilt) { $releaseFiles += Get-ChildItem -LiteralPath $installerPath -Filter '*.exe' -File }
$checksumPath = Join-Path $artifactsRoot 'SHA256SUMS.txt'
$checksumLines = foreach ($file in $releaseFiles | Sort-Object Name) {
    $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $($file.Name)  $($file.Length) bytes"
}

$primary = ($releaseFiles | Where-Object Name -Like '*setup.exe' | Select-Object -First 1)
if (-not $primary) { $primary = $releaseFiles | Where-Object Name -Like '*portable.zip' | Select-Object -First 1 }
$manifest = [ordered]@{
    schemaVersion = 1
    version = $version
    publishedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    product = $project.SelectSingleNode('/Project/PropertyGroup/Product').InnerText.Trim()
    configuration = 'Release'
    architecture = 'x64'
    channel = if ($version.Contains('-rc.')) { 'release-candidate' } else { 'stable' }
    runtimeIdentifier = 'win-x64'
    artifactType = if ($installerBuilt) { 'installer' } else { 'portable' }
    fileName = $primary.Name
    url = $null
    sha256 = (Get-FileHash -LiteralPath $primary.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    size = $primary.Length
    artifacts = @($releaseFiles | Sort-Object Name | ForEach-Object {
        [ordered]@{ fileName = $_.Name; size = $_.Length; sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
    })
    packagedDataset = Get-Content -Raw -Encoding UTF8 (Join-Path $publishPath 'Data\Statistics\manifest.json') | ConvertFrom-Json
    skillData = Get-Content -Raw -Encoding UTF8 (Join-Path $publishPath 'Data\ChampionSkills\manifest.json') | ConvertFrom-Json
}
$manifestPath = Join-Path $artifactsRoot 'release-manifest.json'
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding utf8
$manifestFile = Get-Item -LiteralPath $manifestPath
$checksumLines += "$((Get-FileHash -LiteralPath $manifestPath -Algorithm SHA256).Hash.ToLowerInvariant())  $($manifestFile.Name)  $($manifestFile.Length) bytes"
[IO.File]::WriteAllLines($checksumPath, $checksumLines, [Text.UTF8Encoding]::new($false))
& (Join-Path $PSScriptRoot 'validate-release.ps1') -PublishDirectory $publishPath -ReleaseDirectory $artifactsRoot

Write-Host "Publish: $publishPath"
Write-Host "Portable: $portableFile"
Write-Host "Checksums: $checksumPath"
if ($installerBuilt) { Write-Host "Installer: $installerPath" }
