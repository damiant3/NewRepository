# Build the Codex VS extension (syntax highlighting VSIX)
# Usage: pwsh -File build-vsix.ps1
#
# For full project template + Build menu support, also run:
#   pwsh -File install-vs.ps1

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$outDir = Join-Path $scriptDir "out"

if (Test-Path $outDir) { Remove-Item -Recurse -Force $outDir }
New-Item -ItemType Directory -Path $outDir | Out-Null

$vsixPath = Join-Path $outDir "CodexLanguage.vsix"

# A VSIX is just a ZIP with a specific structure
$staging = Join-Path $outDir "staging"
New-Item -ItemType Directory -Path $staging | Out-Null

# Copy manifest
Copy-Item (Join-Path $scriptDir "extension.vsixmanifest") $staging
# Copy pkgdef
Copy-Item (Join-Path $scriptDir "CodexLanguage.pkgdef") $staging
# Copy TextMate grammar
Copy-Item (Join-Path $scriptDir "codex.tmLanguage.json") $staging

# Create [Content_Types].xml (required by VSIX/OPC format)
$contentTypes = @'
<?xml version="1.0" encoding="utf-8"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="vsixmanifest" ContentType="text/xml" />
  <Default Extension="pkgdef" ContentType="text/plain" />
  <Default Extension="json" ContentType="application/json" />
</Types>
'@
Set-Content -LiteralPath (Join-Path $staging "[Content_Types].xml") -Value $contentTypes

# Zip it up
if (Test-Path $vsixPath) { Remove-Item $vsixPath }
Compress-Archive -Path (Join-Path $staging "*") -DestinationPath $vsixPath

Write-Output "VSIX built: $vsixPath"
Write-Output "Install: double-click the .vsix file or use vsixinstaller.exe"
Write-Output ""
Write-Output "This VSIX provides syntax highlighting only."
Write-Output "For project templates + Build menu, also run:"
Write-Output "  pwsh -File install-vs.ps1"
