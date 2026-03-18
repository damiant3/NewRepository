<#
.SYNOPSIS
    Test the self-hosted compiler's compile() function with a given .codex source file.
    Strips the top-level main() call from Codex.Codex.cs to use it as a library.
#>
param(
    [Parameter(Mandatory)][string]$SampleFile
)
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot\..

$source = Get-Content $SampleFile -Raw
$moduleName = [System.IO.Path]::GetFileNameWithoutExtension($SampleFile)

# Escape for C# string literal
$escaped = $source.Replace('\', '\\').Replace('"', '\"').Replace("`r", '').Replace("`n", '\n')

$testDir = ".self-hosted-test"
if (-not (Test-Path $testDir)) { New-Item -ItemType Directory -Path $testDir | Out-Null }

# Copy Codex.Codex.cs but strip the top-level main() call (line like "Codex_Codex_Codex.main();")
$genLines = Get-Content "Codex.Codex\out\Codex.Codex.cs"
$libLines = $genLines | Where-Object { $_ -notmatch '^\s*Codex_Codex_Codex\.main\(\)' }
[System.IO.File]::WriteAllLines("$testDir\Codex.Codex.Library.cs", $libLines)

$testCs = @"
using System;

var source = "$escaped";
var result = Codex_Codex_Codex.compile(source, "$moduleName");
Console.WriteLine(result);
"@
[System.IO.File]::WriteAllText("$testDir\Test.cs", $testCs)

$csproj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
    <NoWarn>CS5001;CS8600;CS8601;CS8602;CS8603;CS8604</NoWarn>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Test.cs" />
    <Compile Include="Codex.Codex.Library.cs" />
  </ItemGroup>
</Project>
"@
[System.IO.File]::WriteAllText("$testDir\SelfHostedTest.csproj", $csproj)

Write-Host "=== Compiling self-hosted test harness ===" -ForegroundColor Cyan
$buildOut = dotnet build "$testDir\SelfHostedTest.csproj" --verbosity quiet 2>&1 | Out-String
if ($buildOut -match "error CS") {
    Write-Host $buildOut -ForegroundColor Red
    exit 1
}
Write-Host "Build OK" -ForegroundColor Green

Write-Host ""
Write-Host "=== Self-hosted output for: $SampleFile ===" -ForegroundColor Cyan
dotnet run --project "$testDir\SelfHostedTest.csproj" --no-build 2>&1 | ForEach-Object { $_ }
