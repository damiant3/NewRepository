$f = "D:\Projects\NewRepository\.stage2-diag\Compiler.cs"
$lines = Get-Content $f
$newLines = $lines | Where-Object { $_ -ne "Codex_Codex_Codex.main();" }
[System.IO.File]::WriteAllLines($f, $newLines)
Write-Output "Stripped main call. Lines: $($newLines.Count)"
