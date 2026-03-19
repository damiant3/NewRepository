# peek.ps1 — Reliable file reader that never drops line 1.
# Usage: pwsh -File tools/agent/peek.ps1 <file> [startLine] [endLine]
# Defaults to first 50 lines. Use 0 0 for full file with line count.
param(
    [Parameter(Mandatory)][string]$Path,
    [int]$Start = 1,
    [int]$End = 50
)
if (-not (Test-Path $Path)) { Write-Error "File not found: $Path"; exit 1 }
$lines = [System.IO.File]::ReadAllLines((Resolve-Path $Path).Path)
$total = $lines.Count
if ($Start -eq 0 -and $End -eq 0) {
    Write-Output "--- $Path ($total lines) ---"
    for ($i = 0; $i -lt $total; $i++) {
        Write-Output ("{0,4}: {1}" -f ($i + 1), $lines[$i])
    }
} else {
    $s = [Math]::Max(0, $Start - 1)
    $e = [Math]::Min($total, $End)
    Write-Output "--- $Path  lines $Start..$e of $total ---"
    for ($i = $s; $i -lt $e; $i++) {
        Write-Output ("{0,4}: {1}" -f ($i + 1), $lines[$i])
    }
}
