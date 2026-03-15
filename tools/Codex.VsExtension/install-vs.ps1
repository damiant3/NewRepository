# Quick-install Codex syntax highlighting into Visual Studio 2022
# Usage: pwsh -File install-vs.ps1
# This copies the TextMate grammar directly - no VSIX packaging needed.

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# VS 2022 user extensions TextMate path
$vsDir = "$env:USERPROFILE\.vs\Extensions"
$codexDir = Join-Path $vsDir "CodexLanguage"

if (-not (Test-Path $vsDir)) {
    New-Item -ItemType Directory -Path $vsDir -Force | Out-Null
}
if (Test-Path $codexDir) {
    Remove-Item -Recurse -Force $codexDir
}
New-Item -ItemType Directory -Path $codexDir | Out-Null

# Copy grammar and pkgdef
Copy-Item (Join-Path $scriptDir "codex.tmLanguage.json") $codexDir
Copy-Item (Join-Path $scriptDir "CodexLanguage.pkgdef") $codexDir

Write-Output "Installed to: $codexDir"
Write-Output "Restart Visual Studio 2022 to see .codex syntax highlighting."
Write-Output ""
Write-Output "Alternative: use the VSIX for a proper install:"
Write-Output "  pwsh -File build-vsix.ps1"
Write-Output "  Then double-click out/CodexLanguage.vsix"
