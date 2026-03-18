<#
.SYNOPSIS
    Test what the self-hosted compiler produces for a minimal sum type.
#>
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot\..

$testDir = ".self-hosted-test"

# Use actual newlines via PowerShell, then C#-escape them
$source = @"
Result (a) =
  | Success (a)
  | Failure (Text)

main = Success 42
"@

# C# escape: backslash, quote, CR removal, newline to \n
$escaped = $source.Replace('\', '\\').Replace('"', '\"').Replace("`r", '').Replace("`n", '\n')

$testCs = "using System;`nvar source = `"$escaped`";`nvar result = Codex_Codex_Codex.compile(source, `"test`");`nConsole.WriteLine(result);"
[System.IO.File]::WriteAllText("$testDir\Test.cs", $testCs)

dotnet build "$testDir\SelfHostedTest.csproj" --verbosity quiet 2>&1 | Out-Null
Write-Host "=== Self-hosted output for minimal sum type ===" -ForegroundColor Cyan
dotnet run --project "$testDir\SelfHostedTest.csproj" --no-build 2>&1 | ForEach-Object { $_ }
