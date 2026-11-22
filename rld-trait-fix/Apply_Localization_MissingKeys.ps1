param(
  [string]$ModDirectory = $PSScriptRoot
)
$ErrorActionPreference = "Stop"
try {
  $candidate = Join-Path $ModDirectory 'Localization\ko.csv'
  if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
    $known = 'G:\steam\steamapps\common\Probably Stolen\Mods\09_RedLightDistrict_v1.1.1_KO_Install'
    if (Test-Path -LiteralPath (Join-Path $known 'Localization\ko.csv') -PathType Leaf) {
      $ModDirectory = $known
      $candidate = Join-Path $ModDirectory 'Localization\ko.csv'
    } else {
      $ModDirectory = Read-Host 'RedLightDistrict mod directory (folder containing Localization\ko.csv)'
      $candidate = Join-Path $ModDirectory 'Localization\ko.csv'
    }
  }
  if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) { throw "Missing: $candidate" }
  $addition = Join-Path $PSScriptRoot 'Localization_MissingKeys_Only.csv'
  if (-not (Test-Path -LiteralPath $addition -PathType Leaf)) { throw "Missing additions: $addition" }
  $rows = @(Import-Csv -LiteralPath $addition -Header Key,Translation -Encoding UTF8)
  if ($rows.Count -ne 41) { throw "Expected 41 localized rows; found $($rows.Count)" }
  $seenPatch = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
  foreach($row in $rows) {
    if ($row.Key -notmatch '^rld_[A-Za-z0-9_]+$' -or [string]::IsNullOrEmpty($row.Translation)) { throw "Invalid addition row: $($row.Key)" }
    if (-not $seenPatch.Add($row.Key)) { throw "Duplicate addition key: $($row.Key)" }
  }
  $contents = [System.IO.File]::ReadAllText($candidate,[System.Text.Encoding]::UTF8)
  $present = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
  $regex = '(?m)^(?:\uFEFF)?(?<key>rld_[A-Za-z0-9_]+),'
  foreach($hit in [regex]::Matches($contents,$regex)) { [void]$present.Add($hit.Groups['key'].Value) }
  $missing = @($rows | Where-Object { -not $present.Contains($_.Key) })
  Write-Host "Target: $candidate"
  Write-Host "Localization keys to append: $($missing.Count) / 41"
  if ($missing.Count -eq 0) { Write-Host 'All 41 keys already exist; nothing changed.'; exit 0 }
  $backup = $candidate + '.backup_' + (Get-Date -Format 'yyyyMMdd_HHmmss')
  [System.IO.File]::Copy($candidate,$backup,$false)
  $crlf = [string][char]13 + [string][char]10
  $lf = [string][char]10
  $nl = if ($contents.Contains($crlf)) { $crlf } else { $lf }
  $text = ""
  if ($contents.Length -gt 0 -and -not $contents.EndsWith($lf)) { $text += $nl }
  foreach($row in $missing) {
    $key = $row.Key
    $value = $row.Translation.Replace('"','""')
    $text += $key + ',"' + $value + '"' + $nl
  }
  [System.IO.File]::AppendAllText($candidate,$text,(New-Object System.Text.UTF8Encoding($false)))
  $updated = [System.IO.File]::ReadAllText($candidate,[System.Text.Encoding]::UTF8)
  foreach($row in $rows) {
    $pattern = '(?m)^(?:\uFEFF)?' + [regex]::Escape($row.Key) + ','
    if ($updated -notmatch $pattern) { throw "Verification failed for $($row.Key)" }
  }
  Write-Host "Appended $($missing.Count) missing keys. Existing translations and custom rows were not replaced."
  Write-Host "Backup created: $backup"
} catch {
  Write-Error $_
  exit 1
}
