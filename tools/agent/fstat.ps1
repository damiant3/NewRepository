# fstat.ps1 — Quick file stats: line count, char count, size.
# Usage: pwsh -File tools/agent/fstat.ps1 <file|glob> [file2] ...
# Examples:
#   pwsh -File tools/agent/fstat.ps1 src/Codex.Types/TypeChecker.cs
#   pwsh -File tools/agent/fstat.ps1 src/Codex.Types/*.cs
param([Parameter(ValueFromRemainingArguments)][string[]]$Paths)
if ($Paths.Count -eq 0) { Write-Error "Usage: fstat.ps1 <file> [file2] ..."; exit 1 }
$files = @()
foreach ($p in $Paths) {
    $files += Get-Item $p -ErrorAction SilentlyContinue
}
if ($files.Count -eq 0) { Write-Error "No files matched."; exit 1 }
$totalLines = 0; $totalChars = 0; $totalBytes = 0
foreach ($f in $files | Sort-Object FullName) {
    $content = [System.IO.File]::ReadAllText($f.FullName)
    $lineCount = ([System.IO.File]::ReadAllLines($f.FullName)).Count
    $charCount = $content.Length
    $byteCount = $f.Length
    $totalLines += $lineCount; $totalChars += $charCount; $totalBytes += $byteCount
    Write-Output ("{0,5} lines  {1,7} chars  {2,7} bytes  {3}" -f $lineCount, $charCount, $byteCount, $f.Name)
}
if ($files.Count -gt 1) {
    Write-Output ("─────────────────────────────────────────────")
    Write-Output ("{0,5} lines  {1,7} chars  {2,7} bytes  TOTAL ({3} files)" -f $totalLines, $totalChars, $totalBytes, $files.Count)
}
