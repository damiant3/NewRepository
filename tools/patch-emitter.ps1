$src = "D:\Projects\NewRepository\Codex.Codex\Emit\CSharpEmitter.codex"
$dst = "D:\Projects\NewRepository\Codex.Codex\Emit\CSharpEmitter.codex.new"
$lines = [System.IO.File]::ReadAllLines($src)
$out = [System.Collections.Generic.List[string]]::new()

for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($i -eq 286) {
        # Replace line 287: fix precedence with let binding
        # Old: else if is-catch-all (list-at branches i).pattern then True
        # New: use let to extract the branch first
        $out.Add('      else let b = list-at branches i')
        $out.Add('        in if is-catch-all b.pattern then True')
        $out.Add('          else has-any-catch-all branches (i + 1)')
        $i++ # skip line 288 ("else has-any-catch-all branches (i + 1)")
    } else {
        $out.Add($lines[$i])
    }
}

[System.IO.File]::WriteAllLines($dst, $out.ToArray())
Write-Output "Wrote $((Get-Content $dst).Count) lines (was 522)"
