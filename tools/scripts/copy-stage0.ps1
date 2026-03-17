$c = Get-Content "Codex.Codex\out\Codex.Codex.cs" -Raw
$c = $c -replace 'Codex_Codex_Codex\.main\(\);[\r\n]+', ''
Set-Content "tools\Codex.Bootstrap\CodexLib.g.cs" -Value $c -Encoding utf8 -NoNewline
Write-Output "Done. Size: $($c.Length) chars"
