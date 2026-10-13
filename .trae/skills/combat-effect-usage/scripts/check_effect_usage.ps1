param([string]$RepoRoot = "")
if ([string]::IsNullOrEmpty($RepoRoot)) {
  $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
}
$effPath = Join-Path $RepoRoot 'Assets\Resources\Scripts\Configs\EffectConfig_s.cs'
$skPath  = Join-Path $RepoRoot 'Assets\Resources\Scripts\Configs\SkillConfig_s.cs'
foreach ($p in @($effPath, $skPath)) { if (-not (Test-Path $p)) { Write-Error ("file not found: " + $p); exit 1 } }

function C([int[]]$codes){ -join ($codes | ForEach-Object { [char]$_ }) }
$TARGET = C @(0x76EE,0x6807); $RANGE = C @(0x8303,0x56F4)
$DMG = C @(0x4F24,0x5BB3); $BUFF = C @(0x589E,0x76CA); $DEB = C @(0x8D1F,0x9762)

$effText = [System.IO.File]::ReadAllText($effPath, [System.Text.Encoding]::UTF8)
$skText  = [System.IO.File]::ReadAllText($skPath,  [System.Text.Encoding]::UTF8)

# ---- pools keyed by "Type/Area" ----
$pools = @{}
foreach ($k in @(($DMG+'/'+$TARGET),($DMG+'/'+$RANGE),($BUFF+'/'+$TARGET),($BUFF+'/'+$RANGE),($DEB+'/'+$TARGET),($DEB+'/'+$RANGE))) { $pools[$k] = New-Object System.Collections.ArrayList }
foreach ($line in ($effText -split "`r?`n")) {
  if ($line -match '=\s*new EffectConfig\(') {
    $m = [regex]::Matches($line, '"([^"]*)"')
    if ($m.Count -ge 4) {
      $k = $m[2].Groups[1].Value + '/' + $m[3].Groups[1].Value
      if ($pools.ContainsKey($k)) { [void]$pools[$k].Add($m[0].Groups[1].Value) }
    }
  }
}
$effCat = @{}
foreach ($kv in $pools.GetEnumerator()) { foreach ($e in $kv.Value) { $effCat[$e] = $kv.Key } }

Write-Output "== EFFECT POOLS (Type/Area) =="
foreach ($k in ($pools.Keys | Sort-Object)) { Write-Output ("  [{0}] n={1}  {2}" -f $k, $pools[$k].Count, ($pools[$k] -join ', ')) }

# ---- hero skill Type -> effect category ----
$heroTypes = @{
  (C @(0x5355,0x4F24))=$DMG; (C @(0x7FA4,0x4F24))=$DMG; (C @(0x53EC,0x5524))=$DMG;
  (C @(0x81EA,0x589E))=$BUFF; (C @(0x5355,0x589E))=$BUFF; (C @(0x5149,0x73AF))=$BUFF;
  (C @(0x5355,0x7597))=$BUFF; (C @(0x7FA4,0x589E))=$BUFF; (C @(0x5175,0x589E))=$BUFF; (C @(0x7FA4,0x7597))=$BUFF;
  (C @(0x5355,0x63A7))=$DEB; (C @(0x7FA4,0x63A7))=$DEB
}

$viol = New-Object System.Collections.ArrayList
$hitCount = @{}; $areaCount = @{}; $seen = @{}
foreach ($line in ($skText -split "`r?`n")) {
  if ($line -notmatch '=\s*new SkillConfig\(') { continue }
  $m = [regex]::Matches($line, '"([^"]*)"')
  if ($m.Count -lt 17) { continue }
  $t = $m[3].Groups[1].Value
  if (-not $heroTypes.ContainsKey($t)) { continue }
  $sn = $m[1].Groups[1].Value
  if ($seen.ContainsKey($sn)) { continue }
  $seen[$sn] = 1
  $ct = $heroTypes[$t]
  $h = $m[11].Groups[1].Value; $a = $m[12].Groups[1].Value
  $hc = if ($effCat.ContainsKey($h)) { $effCat[$h] } else { '?' }
  $ac = if ($a -eq '') { '' } elseif ($effCat.ContainsKey($a)) { $effCat[$a] } else { '?' }
  if ($h -ne '') { if ($hitCount.ContainsKey($h)) { $hitCount[$h]++ } else { $hitCount[$h] = 1 } }
  if ($a -ne '') { if ($areaCount.ContainsKey($a)) { $areaCount[$a]++ } else { $areaCount[$a] = 1 } }
  if ($hc -ne ($ct + '/' + $TARGET)) { [void]$viol.Add(("HIT  {0} [{1}] {2} = {3}  (expect {4}/{5})" -f $sn, $t, $h, $hc, $ct, $TARGET)) }
  if ($a -ne '' -and $ac -ne ($ct + '/' + $RANGE)) { [void]$viol.Add(("AREA {0} [{1}] {2} = {3}  (expect {4}/{5})" -f $sn, $t, $a, $ac, $ct, $RANGE)) }
}

Write-Output ""
Write-Output ("== RULE VIOLATIONS (hero skills) = " + $viol.Count + " ==")
foreach ($v in $viol) { Write-Output ("  " + $v) }
Write-Output ""
Write-Output "== TOP REUSE HitEffect (skills) =="
$hitCount.GetEnumerator()  | Sort-Object Value -Descending | Select-Object -First 12 | ForEach-Object { Write-Output ("  {0}`t{1}" -f $_.Value, $_.Key) }
Write-Output "== TOP REUSE AreaEffect (skills) =="
$areaCount.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 12 | ForEach-Object { Write-Output ("  {0}`t{1}" -f $_.Value, $_.Key) }
