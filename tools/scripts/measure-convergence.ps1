$stage0 = Get-Content "Codex.Codex\out\Codex.Codex.cs" -Raw
$stage1 = Get-Content "Codex.Codex\stage1-output.cs" -Raw

$s0lines = ($stage0 -split "`n").Count
$s1lines = ($stage1 -split "`n").Count

$s0p0lines = (Select-String -Path "Codex.Codex\out\Codex.Codex.cs" -Pattern "_p0_" | Measure-Object).Count
$s1p0lines = (Select-String -Path "Codex.Codex\stage1-output.cs" -Pattern "_p0_" | Measure-Object).Count

$s0objlines = (Select-String -Path "Codex.Codex\out\Codex.Codex.cs" -Pattern "\bobject\b" | Measure-Object).Count
$s1objlines = (Select-String -Path "Codex.Codex\stage1-output.cs" -Pattern "\bobject\b" | Measure-Object).Count

$s0funcs = ([regex]::Matches($stage0, 'public static')).Count
$s1funcs = ([regex]::Matches($stage1, 'public static')).Count

Write-Output "=== Convergence Metrics ==="
Write-Output "Stage 0 size: $($stage0.Length) chars"
Write-Output "Stage 1 size: $($stage1.Length) chars"
Write-Output "Gap: $($stage0.Length - $stage1.Length) chars"
Write-Output ""
Write-Output "Stage 0 lines: $s0lines"
Write-Output "Stage 1 lines: $s1lines"
Write-Output ""
Write-Output "Stage 0 _p0_ lines: $s0p0lines"
Write-Output "Stage 1 _p0_ lines: $s1p0lines"
Write-Output ""
Write-Output "Stage 0 object lines: $s0objlines"
Write-Output "Stage 1 object lines: $s1objlines"
Write-Output ""
Write-Output "Stage 0 functions: $s0funcs"
Write-Output "Stage 1 functions: $s1funcs"
