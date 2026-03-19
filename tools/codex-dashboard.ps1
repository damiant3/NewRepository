<#
.SYNOPSIS
    Codex Project Health Dashboard — cognitive load monitor for AI-assisted compiler work.
    Windows/PowerShell edition — mirrors tools/codexdashboard.sh for the Linux sandbox.

.DESCRIPTION
    Inspired by ccstatusline (https://github.com/sirmalloc/ccstatusline) by Matthew Breedlove.
    Gives the human visibility into project complexity metrics that predict when an AI agent
    will thrash — chasing red herrings, corrupting files, burning cycles on symptoms.

    The key insight from Opus.md: the agent could only hold ~10% of the codebase in context.
    When the thoughtspace exceeds the thinkspace, the agent spirals.

.PARAMETER Watch
    Run in continuous watch mode, refreshing every N seconds.

.PARAMETER Interval
    Refresh interval in seconds for watch mode. Default: 10.

.PARAMETER NoColor
    Disable ANSI color output.

.PARAMETER Json
    Output metrics as JSON instead of the formatted dashboard.

.EXAMPLE
    pwsh -File tools/codex-dashboard.ps1
    pwsh -File tools/codex-dashboard.ps1 -Watch -Interval 5
    pwsh -File tools/codex-dashboard.ps1 -Json
#>
param(
    [switch]$Watch,
    [int]$Interval = 10,
    [switch]$NoColor,
    [switch]$Json
)

$ErrorActionPreference = 'SilentlyContinue'

# --- Find repo root ---
$repoRoot = $PSScriptRoot
while ($repoRoot -and -not (Test-Path (Join-Path $repoRoot "Codex.sln"))) {
    $repoRoot = Split-Path $repoRoot -Parent
}
if (-not $repoRoot) { Write-Error "Cannot find Codex.sln"; exit 1 }
Set-Location $repoRoot

# --- ANSI helpers ---
function C($text, $code) {
    if ($NoColor) { return $text }
    return "`e[${code}m${text}`e[0m"
}
function Dim($t)    { C $t "90" }
function White($t)  { C $t "97" }
function Green($t)  { C $t "92" }
function Yellow($t) { C $t "93" }
function Red($t)    { C $t "91" }
function Bold($t)   { C $t "1" }
function BoldCyan($t) { C $t "1;96" }

function Severity($value, $warnThreshold, $critThreshold) {
    if ($value -le $warnThreshold) { return Green "$value" }
    if ($value -le $critThreshold) { return Yellow "$value" }
    return Red "$value"
}

function Bar($value, $max, $width = 20) {
    if ($max -eq 0) { $max = 1 }
    $filled = [Math]::Min($width, [Math]::Round(($value / $max) * $width))
    $empty = $width - $filled
    $pct = [Math]::Round(($value / $max) * 100)
    $bar = ("█" * $filled) + ("░" * $empty)
    if ($pct -gt 80) { $color = "91" }
    elseif ($pct -gt 50) { $color = "93" }
    else { $color = "92" }
    return "$(C $bar $color) $(Dim "$pct%")"
}

# ═══════════════════════════════════════════════════════════════
# METRIC COLLECTORS
# ═══════════════════════════════════════════════════════════════

function Get-AllMetrics {
    # --- Self-hosted .codex metrics ---
    $codexFiles = Get-ChildItem "Codex.Codex\*.codex" -Recurse |
        Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' }
    $totalChars = 0; $totalLines = 0; $fileMetrics = @()
    foreach ($f in $codexFiles) {
        $content = Get-Content $f.FullName -Raw
        if ($content) {
            $lines = ($content -split "`n").Count
            $chars = $content.Length
            $totalChars += $chars; $totalLines += $lines
            $fileMetrics += [PSCustomObject]@{ Name = $f.Name; Lines = $lines; Chars = $chars }
        }
    }

    # --- Generated output ---
    $s0File = "Codex.Codex\out\Codex.Codex.cs"
    $s0Chars = 0; $s0Lines = 0; $s0Objects = 0; $s0P0 = 0; $s0Casts = 0
    if (Test-Path $s0File) {
        $s0Content = Get-Content $s0File -Raw
        $s0Chars = $s0Content.Length
        $s0Lines = ($s0Content -split "`n").Count
        $s0Objects = ([regex]::Matches($s0Content, '\bobject\b')).Count
        $s0P0 = ([regex]::Matches($s0Content, '_p0_')).Count
        $s0Casts = ([regex]::Matches($s0Content, '\(\(Func<')).Count +
                   ([regex]::Matches($s0Content, '\(object\)')).Count
    }

    # --- Convergence ---
    $s1Chars = 0; $s3Chars = 0; $fixedPoint = "unknown"
    if ((Test-Path "Codex.Codex\stage1-output.cs") -and (Test-Path "Codex.Codex\stage3-output.cs")) {
        $s1 = Get-Content "Codex.Codex\stage1-output.cs" -Raw
        $s3 = Get-Content "Codex.Codex\stage3-output.cs" -Raw
        $s1Chars = $s1.Length; $s3Chars = $s3.Length
        $fixedPoint = if ($s1 -eq $s3) { "true" } else { "false" }
    }

    # --- Git ---
    $gitBranch = git rev-parse --abbrev-ref HEAD 2>$null
    $gitHash = git log -1 --pretty=format:"%h" 2>$null
    $gitMsg = git log -1 --pretty=format:"%s" 2>$null
    $gitAge = git log -1 --pretty=format:"%ar" 2>$null
    $gitDirty = 0
    $diffStat = git diff --stat 2>$null
    if ($diffStat) { $gitDirty = ($diffStat | Where-Object { $_ -match '\|' }).Count }

    # --- Error state from diagnostic files (no build needed) ---
    $unifyErrors = 0; $errorTys = 0
    if (Test-Path "Codex.Codex\unify-errors.txt") {
        $unifyErrors = (Get-Content "Codex.Codex\unify-errors.txt" | Measure-Object -Line).Lines
    }
    if (Test-Path "Codex.Codex\type-diag.txt") {
        $errorTys = (Select-String -Path "Codex.Codex\type-diag.txt" -Pattern "ERRORTY" | Measure-Object).Count
    }
    $hasMini = Test-Path "samples\mini-bootstrap.codex"

    # --- Cognitive load ---
    # 60K is the honest effective working memory — matches linux dashboard
    $contextBudget = 60000
    $hotFileNames = @("Parser.codex","TypeChecker.codex","CSharpEmitter.codex","Lowering.codex","Unifier.codex","Lexer.codex")
    $cascadeFiles = @("Parser.codex","Lexer.codex")
    $hotChars = 0; $hotLines = 0; $hotCount = 0
    $hotDetails = @()
    foreach ($name in $hotFileNames) {
        $f = Get-ChildItem "Codex.Codex" -Recurse -Filter $name -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($f) {
            $c = (Get-Content $f.FullName -Raw).Length
            $l = (Get-Content $f.FullName).Count
            $hotChars += $c; $hotLines += $l; $hotCount++
            $isCascade = $cascadeFiles -contains $name
            $hotDetails += [PSCustomObject]@{ Name = $name; Lines = $l; Chars = $c; Cascade = $isCascade }
        }
    }
    $hotRatio = if ($contextBudget -gt 0) { [Math]::Round(($hotChars / $contextBudget) * 100) } else { 0 }
    $typeDebt = $s0Objects + $s0P0
    $cascadeDepth = 7

    # Thrash score (mirrors linux version exactly)
    $thrash = 0
    if ($hotRatio -gt 50) { $thrash++ }
    if ($hotRatio -gt 80) { $thrash++ }
    if ($typeDebt -gt 10) { $thrash++ }
    if ($typeDebt -gt 30) { $thrash++ }
    if ($gitDirty -gt 5) { $thrash++ }
    if ($fixedPoint -eq "false") { $thrash++ }

    $risk = switch ($thrash) {
        { $_ -le 1 } { "LOW"; break }
        { $_ -le 2 } { "MEDIUM"; break }
        { $_ -le 4 } { "HIGH"; break }
        default       { "CRITICAL" }
    }

    # --- Tests (static count) ---
    $testFiles = Get-ChildItem "tests\*.cs" -Recurse |
        Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' -and $_.Name -notmatch 'AssemblyInfo|GlobalUsings' }
    $testCount = 0
    foreach ($f in $testFiles) {
        $content = Get-Content $f.FullName -Raw
        if ($content) { $testCount += ([regex]::Matches($content, '\[Fact\]|\[Theory\]')).Count }
    }

    # --- Reference compiler lock ---
    $lockFile = Join-Path $repoRoot "REFERENCE-COMPILER-LOCK.md"
    $refLocked = Test-Path $lockFile
    $lockCommit = ""
    $lockDate = ""
    if ($refLocked) {
        $lockContent = Get-Content $lockFile -Raw
        if ($lockContent -match 'Locked at commit.*?`([a-f0-9]+)`') { $lockCommit = $Matches[1] }
        if ($lockContent -match '\*\*Date\*\*:\s*(\d{4}-\d{2}-\d{2})') { $lockDate = $Matches[1] }
    }

    # --- Prelude ---
    $preludeFiles = @()
    $preludeDir = Join-Path $repoRoot "prelude"
    if (Test-Path $preludeDir) {
        $preludeFiles = Get-ChildItem $preludeDir -Filter "*.codex" | Select-Object -ExpandProperty Name
    }

    return @{
        FileCount = $codexFiles.Count; TotalLines = $totalLines; TotalChars = $totalChars
        FileMetrics = $fileMetrics | Sort-Object Lines -Descending
        S0Chars = $s0Chars; S0Lines = $s0Lines; S0Objects = $s0Objects; S0P0 = $s0P0; S0Casts = $s0Casts
        S1Chars = $s1Chars; S3Chars = $s3Chars; FixedPoint = $fixedPoint
        GitBranch = $gitBranch; GitHash = $gitHash; GitMsg = $gitMsg; GitAge = $gitAge; GitDirty = $gitDirty
        UnifyErrors = $unifyErrors; ErrorTys = $errorTys; HasMini = $hasMini
        ContextBudget = $contextBudget; HotChars = $hotChars; HotLines = $hotLines
        HotCount = $hotCount; HotDetails = $hotDetails; HotRatio = $hotRatio
        TypeDebt = $typeDebt; CascadeDepth = $cascadeDepth
        Thrash = $thrash; Risk = $risk
        TestFiles = $testFiles.Count; TestCount = $testCount
        RefLocked = $refLocked; LockCommit = $lockCommit; LockDate = $lockDate
        PreludeFiles = $preludeFiles
    }
}

# ═══════════════════════════════════════════════════════════════
# RENDER
# ═══════════════════════════════════════════════════════════════

function Render-Dashboard {
    $m = Get-AllMetrics
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

    # --- JSON mode ---
    if ($Json) {
        @{
            timestamp   = $timestamp
            selfHosted  = @{ files = $m.FileCount; lines = $m.TotalLines; chars = $m.TotalChars }
            generated   = @{ chars = $m.S0Chars; lines = $m.S0Lines; objects = $m.S0Objects; p0 = $m.S0P0 }
            convergence = @{ s0 = $m.S0Chars; s1 = $m.S1Chars; s3 = $m.S3Chars; fixedPoint = $m.FixedPoint }
            git         = @{ branch = $m.GitBranch; hash = $m.GitHash; dirty = $m.GitDirty }
            cognitive   = @{ budget = $m.ContextBudget; hotChars = $m.HotChars; hotFiles = $m.HotCount; typeDebt = $m.TypeDebt; thrash = $m.Thrash; risk = $m.Risk }
            errors      = @{ unification = $m.UnifyErrors; errorTy = $m.ErrorTys; hasMiniFile = $m.HasMini }
            tests       = @{ files = $m.TestFiles; methods = $m.TestCount }
            refLock     = @{ locked = $m.RefLocked; commit = $m.LockCommit; date = $m.LockDate }
            prelude     = @{ modules = $m.PreludeFiles }
        } | ConvertTo-Json -Depth 4
        return
    }

    # --- Formatted dashboard ---
    $sep = if ($NoColor) { "=" * 70 } else { Dim ("─" * 70) }
    $heavySep = if ($NoColor) { "═" * 70 } else { Dim ("═" * 70) }

    Clear-Host
    Write-Host ""
    Write-Host "  $(BoldCyan '⚡ CODEX COMPILER DASHBOARD')  $(Dim $timestamp)"
    Write-Host "  $(Dim 'Cognitive load monitor — Windows/PowerShell edition')"
    Write-Host $heavySep

    # ── THRASH RISK ──
    $riskColor = switch ($m.Risk) {
        "LOW"      { "92" }
        "MEDIUM"   { "93" }
        "HIGH"     { "91" }
        "CRITICAL" { "1;91" }
    }
    $riskIcon = switch ($m.Risk) {
        "LOW"      { "🟢" }
        "MEDIUM"   { "🟡" }
        "HIGH"     { "🔴" }
        "CRITICAL" { "🔥" }
    }
    Write-Host ""
    Write-Host "  $riskIcon $(C "AGENT THRASH RISK: $($m.Risk)" $riskColor)  $(Dim "(score: $($m.Thrash)/6)")"
    Write-Host ""
    Write-Host "    Context budget   $(Bar $m.HotChars $m.ContextBudget 30)"
    Write-Host "    $(Dim "Hot path: $($m.HotChars) chars across $($m.HotCount) files  ·  Budget: $($m.ContextBudget) chars")"
    Write-Host "    Type debt        $(Severity $m.TypeDebt 5 20)  $(Dim "(object: $($m.S0Objects)  _p0_: $($m.S0P0))")"
    Write-Host "    Cascade depth    $(Dim "$($m.CascadeDepth) stages  (Lex → Parse → Desugar → Resolve → TypeCheck → Lower → Emit)")"
    Write-Host $sep

    # ── GIT ──
    Write-Host ""
    Write-Host "  $(Bold '⎇') $(White $m.GitBranch)  $(Dim $m.GitHash)  $(Dim $m.GitAge)"
    $commitPreview = if ($m.GitMsg.Length -gt 72) { $m.GitMsg.Substring(0,72) + "…" } else { $m.GitMsg }
    Write-Host "    $(Dim $commitPreview)"
    if ($m.GitDirty -gt 0) {
        Write-Host "    $(Yellow "uncommitted: $($m.GitDirty) files")"
    }
    Write-Host $sep

    # ── SELF-HOSTED COMPILER ──
    Write-Host ""
    Write-Host "  $(Bold '📜 SELF-HOSTED COMPILER')  $(Dim "(.codex source)")"
    Write-Host "    Files: $(White $m.FileCount)   Lines: $(White $m.TotalLines)   Chars: $(White ('{0:N0}' -f $m.TotalChars))"
    Write-Host ""
    Write-Host "    $(Dim 'Hot files (must co-load for pipeline reasoning):')"
    foreach ($h in $m.HotDetails) {
        $pct = [Math]::Round(($h.Chars / $m.ContextBudget) * 100)
        $cascade = if ($h.Cascade) { Red "↯ " } else { "  " }
        $warn = if ($pct -gt 30) { Yellow " ⚠ ${pct}% of context" } else { Dim " ${pct}% of context" }
        Write-Host "      ${cascade}$(White ('{0,-35}' -f $h.Name)) $(Dim ('{0,5}' -f $h.Lines)) lines  $(Dim ('{0,6:N0}' -f $h.Chars)) chars $warn"
    }
    Write-Host "    $(Dim '↯ = cascade risk: bugs here affect all downstream stages')"
    Write-Host $sep

    # ── ERROR STATE ──
    Write-Host ""
    Write-Host "  $(Bold '🔍 ERROR STATE')  $(Dim '(from diagnostic files)')"
    Write-Host "    Unification errors  $(Severity $m.UnifyErrors 0 5)"
    Write-Host "    ErrorTy bindings    $(Severity $m.ErrorTys 0 3)"
    if ($m.HasMini) {
        Write-Host "    Mini repro file     $(Green '✓ samples/mini-bootstrap.codex')"
    } else {
        Write-Host "    Mini repro file     $(Yellow '✗ not found — create one for focused debugging')"
    }
    Write-Host $sep

    # ── GENERATED OUTPUT ──
    Write-Host ""
    Write-Host "  $(Bold '⚙️  GENERATED C#')  $(Dim "(Codex.Codex.cs)")"
    if ($m.S0Chars -gt 0) {
        Write-Host "    Lines: $(White $m.S0Lines)   Chars: $(White ('{0:N0}' -f $m.S0Chars))"
        Write-Host "    $(Dim 'Type quality:')"
        Write-Host "      object refs   $(Severity $m.S0Objects 3 10)    $(Dim '(unresolved types)')"
        Write-Host "      _p0_ proxies  $(Severity $m.S0P0 10 30)   $(Dim '(partial-app placeholders)')"
    } else {
        Write-Host "    $(Red 'NOT FOUND')"
    }
    Write-Host $sep

    # ── CONVERGENCE ──
    Write-Host ""
    Write-Host "  $(Bold '🔄 BOOTSTRAP CONVERGENCE')"
    if ($m.S1Chars -gt 0) {
        Write-Host "    Stage 0: $(White ('{0:N0}' -f $m.S0Chars)) chars  $(Dim '(reference compiler output)')"
        Write-Host "    Stage 2: $(White ('{0:N0}' -f $m.S1Chars)) chars  $(Dim '(self-hosted output)')"
        Write-Host "    Stage 3: $(White ('{0:N0}' -f $m.S3Chars)) chars  $(Dim '(Stage 2 compiles itself)')"
        $delta = $m.S1Chars - $m.S3Chars
        Write-Host "    Delta:   $(if($delta -eq 0){ Green '0' } else { Red $delta }) chars"
        if ($m.FixedPoint -eq "true") {
            Write-Host "    Status:  $(Green '✓ FIXED POINT')"
        } else {
            Write-Host "    Status:  $(Yellow '✗ NOT CONVERGED')"
        }
    } else {
        Write-Host "    $(Dim 'Stage files not found — run bootstrap to populate')"
    }
    Write-Host $sep

    # ── REFERENCE COMPILER LOCK ──
    Write-Host ""
    Write-Host "  $(Bold '🔒 REFERENCE COMPILER')"
    if ($m.RefLocked) {
        Write-Host "    Status:  $(Green '✓ LOCKED')  $(Dim "at commit $($m.LockCommit) on $($m.LockDate)")"
        Write-Host "    $(Dim 'The C# reference compiler (src/) is frozen.')"
        Write-Host "    $(Dim 'New features go in .codex source only.')"
    } else {
        Write-Host "    Status:  $(Yellow '✗ UNLOCKED')  $(Dim '(no REFERENCE-COMPILER-LOCK.md found)')"
    }
    if ($m.PreludeFiles.Count -gt 0) {
        Write-Host "    Prelude: $(Green "$($m.PreludeFiles.Count) modules")  $(Dim "($($m.PreludeFiles -join ', '))")"
    }
    Write-Host $sep

    # ── TESTS ──
    Write-Host ""
    Write-Host "  $(Bold '🧪 TESTS')"
    Write-Host "    Test files: $(White $m.TestFiles)   Test methods: $(White $m.TestCount)"
    Write-Host $sep

    # ── GUIDANCE ──
    Write-Host ""
    Write-Host "  $(Bold '💡 GUIDANCE')"
    if ($m.RefLocked) {
        Write-Host "    $(Green '🔒 Reference compiler is LOCKED. All new features in .codex source.')"
    }
    switch ($m.Risk) {
        "CRITICAL" {
            Write-Host "    $(Red '→ DO NOT assign multi-file changes right now.')"
            Write-Host "    $(Red '→ Create a mini repro file first.')"
            Write-Host "    $(Red '→ Isolate the pipeline stage before engaging.')"
        }
        "HIGH" {
            Write-Host "    $(Yellow '→ Keep tasks to ONE pipeline stage at a time.')"
            Write-Host "    $(Yellow '→ Pre-load only the relevant .codex file + its test.')"
        }
        "MEDIUM" {
            Write-Host "    $(Dim '→ Can handle single-stage changes.')"
            Write-Host "    $(Dim '→ Watch for cascading errors.')"
        }
        "LOW" {
            Write-Host "    $(Green '→ Complexity is manageable. Agent should be productive.')"
        }
    }

    if ($m.S0Objects -gt 10) {
        Write-Host "    $(Yellow "→ $($m.S0Objects) 'object' refs in generated C# — type-def map work needed.")"
    }
    if ($m.S0P0 -gt 20) {
        Write-Host "    $(Yellow "→ $($m.S0P0) '_p0_' proxies — partial application type resolution needed.")"
    }
    if ($m.FixedPoint -eq "false") {
        Write-Host "    $(Yellow '→ Fixed point broken — any compiler change needs re-verification.')"
    }

    Write-Host ""
    Write-Host "  $(Dim 'Tip: Use -Json flag to pipe metrics to other tools.')"
    Write-Host "  $(Dim 'Tip: Use -Watch flag for continuous monitoring during agent sessions.')"
    Write-Host ""
}

# ═══════════════════════════════════════════════════════════════
# MAIN
# ═══════════════════════════════════════════════════════════════

if ($Watch) {
    while ($true) {
        Render-Dashboard
        Write-Host "  $(Dim "Refreshing in ${Interval}s... (Ctrl+C to stop)")"
        Start-Sleep -Seconds $Interval
    }
} else {
    Render-Dashboard
}
