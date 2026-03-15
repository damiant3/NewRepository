$samples = Get-ChildItem "$PSScriptRoot\..\samples\*.codex"
$results = @()
foreach ($f in $samples) {
    $output = dotnet run --no-build --project "$PSScriptRoot\..\tools\Codex.Cli" -- run $f.FullName 2>&1
    $exitCode = $LASTEXITCODE
    $results += "$($f.Name): exit=$exitCode output=$($output | Select-Object -First 1)"
}
$results | Out-File "$PSScriptRoot\_run_results.txt"
