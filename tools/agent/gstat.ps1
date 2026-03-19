# gstat.ps1 — Git status + recent log in one shot.
# Usage: pwsh -File tools/agent/gstat.ps1
$repoRoot = Join-Path $PSScriptRoot ".." ".."
Push-Location $repoRoot
try {
    $branch = git rev-parse --abbrev-ref HEAD 2>&1
    $hash = git rev-parse --short HEAD 2>&1
    $dirty = @(git status --porcelain 2>&1)
    $ahead = git rev-list --count "origin/$branch..HEAD" 2>&1
    $behind = git rev-list --count "HEAD..origin/$branch" 2>&1

    Write-Output "=== GIT STATUS ==="
    Write-Output "Branch: $branch  Commit: $hash  Ahead: $ahead  Behind: $behind  Dirty: $($dirty.Count) files"
    if ($dirty.Count -gt 0 -and $dirty.Count -le 15) {
        foreach ($d in $dirty) { Write-Output "  $d" }
    } elseif ($dirty.Count -gt 15) {
        $dirty | Select-Object -First 10 | ForEach-Object { Write-Output "  $_" }
        Write-Output "  ... and $($dirty.Count - 10) more"
    }
    Write-Output ""
    Write-Output "=== RECENT COMMITS ==="
    git log --oneline -8 2>&1 | ForEach-Object { Write-Output "  $_" }

    # Remote branches
    $remotes = @(git branch -r 2>&1 | Where-Object { $_ -notmatch "HEAD" -and $_ -match "origin/" })
    $nonMaster = @($remotes | Where-Object { $_ -notmatch "origin/master" })
    if ($nonMaster.Count -gt 0) {
        Write-Output ""
        Write-Output "=== FEATURE BRANCHES ==="
        foreach ($r in $nonMaster) { Write-Output "  $($r.Trim())" }
    }
} finally { Pop-Location }
