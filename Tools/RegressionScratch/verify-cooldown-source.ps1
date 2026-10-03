param([string] $RepositoryRoot = (Join-Path $PSScriptRoot '../..'))
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath($RepositoryRoot)
$sourcePath = Join-Path $root 'artifacts/cooldown-audit-source-16.17.1.json'
$source = Get-Content -Raw -Encoding UTF8 $sourcePath | ConvertFrom-Json
if ($source.version -ne '16.17.1') { throw 'Wrong source version' }
$metadata = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Data/Champions/champions.ko.json') | ConvertFrom-Json
$keys = @('Q','W','E','R')
$normal = @('Aphelios.E','Ashe.Q','Shyvana.R','Singed.Q','Teemo.E','TwistedFate.E','Vayne.W','Vi.W','Yunara.Q')
$display = @('Bard.W','Kalista.E','Rakan.E','TahmKench.R','Talon.E','Veigar.W','Yuumi.W','Zyra.W')
$unverified = @('Gangplank.E','Karthus.Q','Zeri.Q')
$entries = @($normal + $display + $unverified + @('Urgot.W') | Sort-Object)
$before = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'artifacts/cooldown-audit/before-manifest.json') | ConvertFrom-Json
if (@(Compare-Object @($before.unavailableCooldowns | Sort-Object) $entries).Count) { throw 'Original 21-item inventory mismatch' }
$rows = @()
$allZero = 0; $mixedZero = 0; $negative = 0; $empty = 0; $repeated = 0; $numeric = 0; $spellCount = 0
foreach ($champion in $metadata.champions) {
    $id = $champion.id
    $generated = Get-Content -Raw -Encoding UTF8 (Join-Path $root "Data/ChampionSkills/$id.json") | ConvertFrom-Json
    if ($source.data.$id.spells.Count -ne 4) { throw "Source slots: $id" }
    if ($generated.championId -cne $id -or $generated.patch -ne $source.version -or $generated.spells.Count -ne 4) { throw "Header/slots: $id" }
    for ($i=0; $i -lt 4; $i++) {
        $s = $source.data.$id.spells[$i]; $g = $generated.spells[$i]; $entry = "$id.$($keys[$i])"
        $raw = @($s.cooldown)
        if ($raw.Count -eq 0) { $empty++ }
        if (@($raw | Where-Object { $_ -lt 0 }).Count) { $negative++ }
        if ($raw.Count -ne $s.maxrank) { throw "Rank mismatch: $entry" }
        if ($g.key -cne $keys[$i] -or $g.name -cne $s.name -or $g.icon -cne "$($id)_$($keys[$i]).png") { throw "Slot mapping: $entry" }
        if (($raw | Select-Object -Unique | Measure-Object).Count -eq 1) { $repeated++ }
        $zeros = @($raw | Where-Object { $_ -eq 0 }).Count
        if ($zeros -eq $raw.Count) { $allZero++ }
        elseif ($zeros -gt 0) { $mixedZero++ }
        if ($g.cooldowns -ne $null) {
            $numeric++
            if (($g.cooldowns -join '/') -ne ($raw -join '/')) { throw "Value mismatch: $entry" }
        } elseif ($zeros -ne $raw.Count) { throw "Dropped numeric values: $entry" }
        if ($entries -contains $entry) {
            $burn = @($s.cooldownBurn.Split('/') | ForEach-Object { [double]::Parse($_,[Globalization.CultureInfo]::InvariantCulture) })
            if ($burn.Count -eq 1) { $burn = @($burn * $raw.Count) }
            if (($burn -join '/') -ne ($raw -join '/')) { throw "Burn mismatch: $entry" }
            $classification = if ($normal -contains $entry) { 'NORMAL_SPECIAL_CASE' } elseif ($display -contains $entry) { 'DISPLAY_IMPROVEMENT_NEEDED' } elseif ($unverified -contains $entry) { 'UNVERIFIED' } else { 'DATA_BUG' }
            $rows += [pscustomobject]@{ entry=$entry; name=$s.name; sourceSpellId=$s.id; sourceIndex=$i;
                maxrank=$s.maxrank; cooldownBurn=$s.cooldownBurn; sourceCooldowns=$raw; generatedBefore=$null;
                generatedAfter=$g.cooldowns; classification=$classification;
                sourceDescription=$s.description; sourceTooltip=$s.tooltip }
        }
        $spellCount++
    }
}
if ($rows.Count -ne 21 -or $metadata.champions.Count -ne 173 -or $spellCount -ne 692) { throw 'Coverage mismatch' }
$report = [ordered]@{ version=$source.version; sourceUrl='https://ddragon.leagueoflegends.com/cdn/16.17.1/data/ko_KR/championFull.json';
    sourceSha256=(Get-FileHash $sourcePath -Algorithm SHA256).Hash; champions=$metadata.champions.Count; spells=$spellCount;
    missingChampions=0; mappingErrors=0; rankCountErrors=0; sourceValueErrors=0; malformedCooldowns=0;
    allZeroArrays=$allZero; mixedZeroArrays=$mixedZero; negativeArrays=$negative; emptyArrays=$empty; repeatedArrays=$repeated;
    displayedNumericArrays=$numeric; exceptions=$rows }
$report | ConvertTo-Json -Depth 12 | Set-Content -Encoding UTF8 (Join-Path $root 'artifacts/cooldown-audit/results.json')
"PASS: champions=$($metadata.champions.Count), spells=$spellCount, missing=0, mapping/rank/value/malformed errors=0; numeric=$numeric, allZero=$allZero, mixedZero=$mixedZero, negative=$negative, empty=$empty, repeated=$repeated"
$rows | Group-Object classification | Select-Object Name,Count
