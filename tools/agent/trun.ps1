# trun.ps1 — Test runner with filtered, non-truncated output.
# Usage:
#   pwsh -File tools/agent/trun.ps1                     — run all tests, summary only
#   pwsh -File tools/agent/trun.ps1 -Project Types      — run tests matching project name
#   pwsh -File tools/agent/trun.ps1 -Filter "Linear"    — run tests matching display name
#   pwsh -File tools/agent/trun.ps1 -Full               — show all output (no filter)
param(
    [string]$Project = "",
    [string]$Filter = "",
    [switch]$Full
)
$sln = Join-Path $PSScriptRoot ".." ".." "Codex.sln"
$outFile = Join-Path $PSScriptRoot ".test-output.txt"

$args_ = @("test", $sln, "--no-restore", "--verbosity", "minimal")
if ($Project) {
    $csproj = Get-ChildItem (Join-Path $PSScriptRoot ".." ".." "tests") -Recurse -Filter "*.csproj" |
        Where-Object { $_.BaseName -match $Project } | Select-Object -First 1
    if ($csproj) {
        $args_ = @("test", $csproj.FullName, "--no-restore", "--verbosity", "minimal")
    } else {
        Write-Error "No test project matching '$Project'"; exit 1
    }
}
if ($Filter) {
    $args_ += @("--filter", "DisplayName~$Filter")
}

& dotnet @args_ 2>&1 | Out-File $outFile -Encoding utf8
$output = Get-Content $outFile -Raw

if ($Full) {
    Write-Output $output
} else {
    # Extract summary lines and failures
    $lines = Get-Content $outFile
    $failures = @()
    $summaries = @()
    $inFailure = $false
    foreach ($line in $lines) {
        if ($line -match "Failed\s+\S+") { $inFailure = $true }
        if ($inFailure) {
            $failures += $line
            if ($line -match "^\s*$" -and $failures.Count -gt 3) { $inFailure = $false }
        }
        if ($line -match "(Passed|Failed)!\s+-\s+Failed:") { $summaries += $line }
    }
    if ($failures.Count -gt 0) {
        Write-Output "=== FAILURES ==="
        $failures | ForEach-Object { Write-Output $_ }
        Write-Output ""
    }
    Write-Output "=== SUMMARY ==="
    $summaries | ForEach-Object { Write-Output $_ }
    $totalPassed = 0; $totalFailed = 0; $totalSkipped = 0
    foreach ($s in $summaries) {
        if ($s -match "Failed:\s+(\d+),\s+Passed:\s+(\d+),\s+Skipped:\s+(\d+)") {
            $totalFailed += [int]$Matches[1]; $totalPassed += [int]$Matches[2]; $totalSkipped += [int]$Matches[3]
        }
    }
    Write-Output ""
    $status = if ($totalFailed -eq 0) { "ALL PASS" } else { "FAILURES DETECTED" }
    Write-Output "$status  —  Passed: $totalPassed  Failed: $totalFailed  Skipped: $totalSkipped  Total: $($totalPassed + $totalFailed + $totalSkipped)"
}
Remove-Item $outFile -ErrorAction SilentlyContinue
