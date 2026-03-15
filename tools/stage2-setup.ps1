$tempDir = Join-Path $env:TEMP "codex_stage2_test"
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }
New-Item $tempDir -ItemType Directory | Out-Null
Copy-Item "D:\Projects\NewRepository\codex-src\output.cs" (Join-Path $tempDir "Program.cs")
$csproj = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup></Project>'
Set-Content (Join-Path $tempDir "Stage2.csproj") $csproj
Set-Content (Join-Path $env:TEMP "stage2_dir.txt") $tempDir
