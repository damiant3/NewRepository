# sdiff.ps1 — Snapshot-and-diff for verifying edits.
# Usage:
#   pwsh -File tools/agent/sdiff.ps1 snap <file>        — save a snapshot before editing
#   pwsh -File tools/agent/sdiff.ps1 diff <file>        — diff current vs snapshot
#   pwsh -File tools/agent/sdiff.ps1 restore <file>     — restore from snapshot
#   pwsh -File tools/agent/sdiff.ps1 clean              — delete all snapshots
param(
    [Parameter(Mandatory)][ValidateSet('snap','diff','restore','clean')][string]$Action,
    [string]$Path
)
$snapDir = Join-Path $PSScriptRoot ".snapshots"
if (-not (Test-Path $snapDir)) { New-Item -ItemType Directory -Path $snapDir -Force | Out-Null }

function Get-SnapPath($filePath) {
    $resolved = (Resolve-Path $filePath -ErrorAction SilentlyContinue)
    if ($resolved) { $filePath = $resolved.Path }
    $name = [System.IO.Path]::GetFileName($filePath)
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($filePath.ToLowerInvariant())
    $sha = [System.Security.Cryptography.SHA256]::HashData($bytes)
    $hash = [BitConverter]::ToString($sha, 0, 4).Replace('-','').ToLowerInvariant()
    return Join-Path $snapDir "$name.$hash"
}

switch ($Action) {
    'snap' {
        if (-not $Path) { Write-Error "snap requires a file path"; exit 1 }
        if (-not (Test-Path $Path)) { Write-Error "File not found: $Path"; exit 1 }
        $snapPath = Get-SnapPath $Path
        Copy-Item $Path $snapPath -Force
        $lines = ([System.IO.File]::ReadAllLines((Resolve-Path $Path).Path)).Count
        Write-Output "Snapshot saved: $Path ($lines lines) -> $snapPath"
    }
    'diff' {
        if (-not $Path) { Write-Error "diff requires a file path"; exit 1 }
        if (-not (Test-Path $Path)) { Write-Error "File not found: $Path"; exit 1 }
        $snapPath = Get-SnapPath $Path
        if (-not (Test-Path $snapPath)) { Write-Error "No snapshot for $Path. Run 'snap' first."; exit 1 }
        $oldLines = [System.IO.File]::ReadAllLines($snapPath)
        $newLines = [System.IO.File]::ReadAllLines((Resolve-Path $Path).Path)
        $oldCount = $oldLines.Count; $newCount = $newLines.Count
        Write-Output "--- $Path  snap: $oldCount lines -> current: $newCount lines (delta: $($newCount - $oldCount)) ---"
        $maxLines = [Math]::Max($oldCount, $newCount)
        $diffs = 0
        for ($i = 0; $i -lt $maxLines; $i++) {
            $old = if ($i -lt $oldCount) { $oldLines[$i] } else { $null }
            $new = if ($i -lt $newCount) { $newLines[$i] } else { $null }
            if ($old -cne $new) {
                $diffs++
                $lineNum = $i + 1
                if ($null -eq $old) {
                    Write-Output ("+{0,4}: {1}" -f $lineNum, $new)
                } elseif ($null -eq $new) {
                    Write-Output ("-{0,4}: {1}" -f $lineNum, $old)
                } else {
                    Write-Output ("-{0,4}: {1}" -f $lineNum, $old)
                    Write-Output ("+{0,4}: {1}" -f $lineNum, $new)
                }
                if ($diffs -ge 60) { Write-Output "... ($diffs+ differing lines, truncated)"; break }
            }
        }
        if ($diffs -eq 0) { Write-Output "No differences." }
        else { Write-Output "--- $diffs differing line(s) ---" }
    }
    'restore' {
        if (-not $Path) { Write-Error "restore requires a file path"; exit 1 }
        $snapPath = Get-SnapPath $Path
        if (-not (Test-Path $snapPath)) { Write-Error "No snapshot for $Path."; exit 1 }
        Copy-Item $snapPath $Path -Force
        Write-Output "Restored $Path from snapshot."
    }
    'clean' {
        if (Test-Path $snapDir) {
            $count = (Get-ChildItem $snapDir).Count
            Remove-Item $snapDir -Recurse -Force
            Write-Output "Cleaned $count snapshot(s)."
        } else {
            Write-Output "No snapshots to clean."
        }
    }
}
