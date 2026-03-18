<#
.SYNOPSIS
    Codex Project Health Dashboard — cognitive load monitor for AI-assisted compiler work.

.DESCRIPTION
    Inspired by ccstatusline (https://github.com/sirmalloc/ccstatusline) by Matthew Breedlove,
    which provides real-time status line metrics for Claude Code CLI. This tool serves the same
    purpose for GitHub Copilot in Visual Studio: giving the human visibility into project
    complexity metrics that predict when an AI agent will thrash.

    The key insight from Opus.md: the agent could only hold ~10% of the codebase in context
    at once. When the thoughtspace exceeds the thinkspace, the agent spirals — chasing red
    herrings, corrupting files, burning cycles on symptoms instead of root causes.

    This dashboard shows you WHEN to step in and simplify the problem.

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

# --- Find repo root (walk up from script location) ---
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
function Cyan($t)   { C $t "96" }
function Bold($t)   { C $t "1" }
function BoldCyan($t) { C $t "1;96" }

function Severity($value, $warnThreshold, $critThreshold, [switch]$Invert) {
    if ($Invert) {
        if ($value -ge $critThreshold) { return Green "$value" }
        if ($value -ge $warnThreshold) { return Yellow "$value" }
        return Red "$value"
    }
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

function Get-SelfHostedMetrics {
    $codexFiles = Get-ChildItem "Codex.Codex\*.codex" -Recurse |
        Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' }
    $totalChars = 0; $totalLines = 0; $fileMetrics = @()
    foreach ($f in $codexFiles) {
        $content = Get-Content $f.FullName -Raw
        if ($content) {
            $lines = ($content -split "`n").Count
            $chars = $content.Length
            $totalChars += $chars; $totalLines += $lines
            $fileMetrics += [PSCustomObject]@{
                Name  = $f.Name
                Lines = $lines
                Chars = $chars
            }
        }
    }
    return @{
        FileCount  = $codexFiles.Count
        TotalLines = $totalLines
        TotalChars = $totalChars
        Files      = $fileMetrics | Sort-Object Lines -Descending
    }
}

function Get-GeneratedMetrics {
    $genPath = "Codex.Codex\out\Codex.Codex.cs"
    if (-not (Test-Path $genPath)) { return $null }
    $content = Get-Content $genPath -Raw
    $lines = ($content -split "`n").Count
    $objects = ([regex]::Matches($content, '\bobject\b')).Count
    $p0 = ([regex]::Matches($content, '_p0_')).Count
    $errorTy = ([regex]::Matches($content, 'ErrorTy')).Count
    $casts = ([regex]::Matches($content, '\(\(Func<')).Count +
              ([regex]::Matches($content, '\(object\)')).Count
    return @{
        Chars   = $content.Length
        Lines   = $lines
        Objects = $objects
        P0      = $p0
        ErrorTy = $errorTy
        Casts   = $casts
    }
}

function Get-ConvergenceMetrics {
    $s0Path = "Codex.Codex\out\Codex.Codex.cs"
    $s1Path = "Codex.Codex\stage1-output.cs"
    $s3Path = "Codex.Codex\stage3-output.cs"

    $result = @{ HasStages = $false; FixedPoint = $false }

    if ((Test-Path $s1Path) -and (Test-Path $s3Path)) {
        $s1 = Get-Content $s1Path -Raw
        $s3 = Get-Content $s3Path -Raw
        $result.HasStages = $true
        $result.S1Chars = $s1.Length
        $result.S3Chars = $s3.Length
        $result.FixedPoint = ($s1 -eq $s3)
        $result.CharDelta = $s1.Length - $s3.Length
    }
    if (Test-Path $s0Path) {
        $s0 = Get-Content $s0Path -Raw
        $result.S0Chars = $s0.Length
    }
    return $result
}

function Get-BuildMetrics {
    $output = dotnet build Codex.sln --verbosity quiet 2>&1 | Out-String
    $warnings = ([regex]::Matches($output, 'warning CS\d+')).Count
    $errors = ([regex]::Matches($output, 'error CS\d+')).Count
    $succeeded = $output -match "Build succeeded"
    return @{
        Succeeded = $succeeded
        Warnings  = $warnings
        Errors    = $errors
    }
}

function Get-GitMetrics {
    $branch = git rev-parse --abbrev-ref HEAD 2>$null
    $lastMsg = git log -1 --pretty=format:"%s" 2>$null
    $lastDate = git log -1 --pretty=format:"%ar" 2>$null
    $lastHash = git log -1 --pretty=format:"%h" 2>$null
    $uncommitted = git diff --stat 2>$null
    $uncommittedFiles = if ($uncommitted) { ($uncommitted -split "`n" | Where-Object { $_ -match '\|' }).Count } else { 0 }
    $insertions = 0; $deletions = 0
    if ($uncommitted -match '(\d+) insertion') { $insertions = [int]$Matches[1] }
    if ($uncommitted -match '(\d+) deletion')  { $deletions  = [int]$Matches[1] }
    return @{
        Branch          = $branch
        LastCommit      = $lastMsg
        LastCommitAge   = $lastDate
        LastCommitHash  = $lastHash
        UncommittedFiles = $uncommittedFiles
        Insertions      = $insertions
        Deletions       = $deletions
    }
}

function Get-CognitiveLoadEstimate {
    param($SelfHosted, $Generated)

    # Context window budget for typical AI agents (chars)
    # Opus: ~200K context, but effective working memory is far less
    # Copilot: varies, but ~50-80K effective
    $contextBudget = 80000

    # Files that must be co-loaded to reason about a pipeline stage
    $hotFiles = @(
        "Codex.Codex\Syntax\Parser.codex"
        "Codex.Codex\Types\TypeChecker.codex"
        "Codex.Codex\Emit\CSharpEmitter.codex"
        "Codex.Codex\IR\Lowering.codex"
        "Codex.Codex\Types\Unifier.codex"
        "Codex.Codex\Syntax\Lexer.codex"
    )
    $hotChars = 0; $hotLines = 0; $hotCount = 0
    foreach ($path in $hotFiles) {
        $full = Join-Path $repoRoot $path
        if (Test-Path $full) {
            $content = Get-Content $full -Raw
            if ($content) {
                $hotChars += $content.Length
                $hotLines += ($content -split "`n").Count
                $hotCount++
            }
        }
    }

    # Cross-file dependency score: how many pipeline stages does a change touch?
    # Parser bug → type checker symptoms → emitter artifacts → convergence failure
    $cascadeDepth = 6  # Lexer → Parser → Desugarer → NameResolver → TypeChecker → Lowering → Emitter

    # Type system complexity: object/p0 count = unresolved type information
    $typeDebt = 0
    if ($Generated) { $typeDebt = $Generated.Objects + $Generated.P0 }

    # Cognitive load ratio: how much of the hot path fits in one context window?
    $hotPathRatio = if ($contextBudget -gt 0) { [Math]::Round(($hotChars / $contextBudget) * 100) } else { 0 }

    # Thrash risk: composite score
    # - Hot path > 100% of context = guaranteed thrashing
    # - Type debt > 20 = agent will chase symptoms
    # - Cascade depth 6+ = root cause is never where symptoms appear
    $thrashScore = 0
    if ($hotPathRatio -gt 100) { $thrashScore += 3 }
    elseif ($hotPathRatio -gt 70) { $thrashScore += 2 }
    elseif ($hotPathRatio -gt 50) { $thrashScore += 1 }
    if ($typeDebt -gt 50) { $thrashScore += 3 }
    elseif ($typeDebt -gt 20) { $thrashScore += 2 }
    elseif ($typeDebt -gt 5)  { $thrashScore += 1 }

    $risk = switch ($thrashScore) {
        { $_ -le 1 } { "LOW" }
        { $_ -le 3 } { "MEDIUM" }
        { $_ -le 5 } { "HIGH" }
        default       { "CRITICAL" }
    }

    return @{
        ContextBudget  = $contextBudget
        HotPathChars   = $hotChars
        HotPathLines   = $hotLines
        HotPathFiles   = $hotCount
        HotPathRatio   = $hotPathRatio
        TypeDebt       = $typeDebt
        CascadeDepth   = $cascadeDepth
        ThrashScore    = $thrashScore
        Risk           = $risk
    }
}

function Get-TestMetrics {
    # Quick check: just count test files and methods without running them
    $testFiles = Get-ChildItem "tests\*.cs" -Recurse |
        Where-Object { $_.DirectoryName -notmatch '\\obj\\|\\bin\\' -and $_.Name -notmatch 'AssemblyInfo|GlobalUsings' }
    $testCount = 0
    foreach ($f in $testFiles) {
        $content = Get-Content $f.FullName -Raw
        if ($content) {
            $testCount += ([regex]::Matches($content, '\[Fact\]|\[Theory\]')).Count
        }
    }
    return @{
        TestFiles  = $testFiles.Count
        TestCount  = $testCount
    }
}

# ═══════════════════════════════════════════════════════════════
# RENDER
# ═══════════════════════════════════════════════════════════════

function Render-Dashboard {
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

    # Collect all metrics
    $self      = Get-SelfHostedMetrics
    $gen       = Get-GeneratedMetrics
    $conv      = Get-ConvergenceMetrics
    $gitM      = Get-GitMetrics
    $cognitive = Get-CognitiveLoadEstimate -SelfHosted $self -Generated $gen
    $tests     = Get-TestMetrics

    # --- JSON mode ---
    if ($Json) {
        $payload = @{
            timestamp    = $timestamp
            selfHosted   = @{ files = $self.FileCount; lines = $self.TotalLines; chars = $self.TotalChars }
            generated    = if ($gen) { @{ chars = $gen.Chars; lines = $gen.Lines; objects = $gen.Objects; p0 = $gen.P0; errorTy = $gen.ErrorTy; casts = $gen.Casts } } else { $null }
            convergence  = $conv
            git          = $gitM
            cognitive    = $cognitive
            tests        = $tests
        }
        $payload | ConvertTo-Json -Depth 4
        return
    }

    # --- Formatted dashboard ---
    $sep = if ($NoColor) { "=" * 70 } else { Dim ("─" * 70) }
    $heavySep = if ($NoColor) { "═" * 70 } else { Dim ("═" * 70) }

    Clear-Host
    Write-Host ""
    Write-Host "  $(BoldCyan '⚡ CODEX COMPILER DASHBOARD')  $(Dim $timestamp)"
    Write-Host "  $(Dim 'Cognitive load monitor for AI-assisted compiler development')"
    Write-Host "  $(Dim 'Inspired by ccstatusline (github.com/sirmalloc/ccstatusline)')"
    Write-Host $heavySep

    # ── THRASH RISK (the #1 thing you care about) ──
    $riskColor = switch ($cognitive.Risk) {
        "LOW"      { "92" }
        "MEDIUM"   { "93" }
        "HIGH"     { "91" }
        "CRITICAL" { "1;91" }
    }
    $riskIcon = switch ($cognitive.Risk) {
        "LOW"      { "🟢" }
        "MEDIUM"   { "🟡" }
        "HIGH"     { "🔴" }
        "CRITICAL" { "🔥" }
    }
    Write-Host ""
    Write-Host "  $riskIcon $(C "AGENT THRASH RISK: $($cognitive.Risk)" $riskColor)  $(Dim "(score: $($cognitive.ThrashScore)/6)")"
    Write-Host ""
    Write-Host "    Context budget   $(Bar $cognitive.HotPathChars $cognitive.ContextBudget 30)"
    Write-Host "    $(Dim "Hot path: $($cognitive.HotPathChars) chars across $($cognitive.HotPathFiles) files  ·  Budget: $($cognitive.ContextBudget) chars")"
    Write-Host "    Type debt        $(Severity $cognitive.TypeDebt 5 20)  $(Dim "(object: $(if($gen){$gen.Objects}else{'?'})  _p0_: $(if($gen){$gen.P0}else{'?'}))")"
    Write-Host "    Cascade depth    $(Dim "$($cognitive.CascadeDepth) stages  (Lexer → Parser → Desugar → Resolve → TypeCheck → Lower → Emit)")"
    Write-Host $sep

    # ── GIT ──
    Write-Host ""
    Write-Host "  $(Bold '⎇') $(White $gitM.Branch)  $(Dim $gitM.LastCommitHash)  $(Dim $gitM.LastCommitAge)"
    $commitPreview = if ($gitM.LastCommit.Length -gt 72) { $gitM.LastCommit.Substring(0,72) + "…" } else { $gitM.LastCommit }
    Write-Host "    $(Dim $commitPreview)"
    if ($gitM.UncommittedFiles -gt 0) {
        Write-Host "    $(Yellow "uncommitted: $($gitM.UncommittedFiles) files")  $(Green "+$($gitM.Insertions)")  $(Red "-$($gitM.Deletions)")"
    }
    Write-Host $sep

    # ── SELF-HOSTED COMPILER ──
    Write-Host ""
    Write-Host "  $(Bold '📜 SELF-HOSTED COMPILER')  $(Dim "(.codex source)")"
    Write-Host "    Files: $(White $self.FileCount)   Lines: $(White $self.TotalLines)   Chars: $(White ('{0:N0}' -f $self.TotalChars))"
    Write-Host ""
    Write-Host "    $(Dim 'Hot files (must co-load for pipeline reasoning):')"
    $hotFiles = $self.Files | Select-Object -First 6
    foreach ($f in $hotFiles) {
        $pct = [Math]::Round(($f.Chars / $cognitive.ContextBudget) * 100)
        $warn = if ($pct -gt 30) { Yellow " ⚠ ${pct}% of context" } else { Dim " ${pct}% of context" }
        Write-Host "      $(White ('{0,-35}' -f $f.Name)) $(Dim ('{0,5}' -f $f.Lines)) lines  $(Dim ('{0,6:N0}' -f $f.Chars)) chars $warn"
    }
    Write-Host $sep

    # ── GENERATED OUTPUT ──
    Write-Host ""
    Write-Host "  $(Bold '⚙️  GENERATED C#')  $(Dim "(Codex.Codex.cs)")"
    if ($gen) {
        Write-Host "    Lines: $(White $gen.Lines)   Chars: $(White ('{0:N0}' -f $gen.Chars))"
        Write-Host "    $(Dim 'Type quality:')"
        Write-Host "      object refs   $(Severity $gen.Objects 3 10)    $(Dim '(unresolved types → agent sees "object" everywhere)')"
        Write-Host "      _p0_ proxies  $(Severity $gen.P0 10 30)   $(Dim '(partial-app placeholders → confusing signatures)')"
        Write-Host "      unsafe casts  $(Severity $gen.Casts 20 50)   $(Dim '(Func<>/object casts → noisy generated code)')"
    } else {
        Write-Host "    $(Red 'NOT FOUND')"
    }
    Write-Host $sep

    # ── CONVERGENCE ──
    Write-Host ""
    Write-Host "  $(Bold '🔄 BOOTSTRAP CONVERGENCE')"
    if ($conv.HasStages) {
        $fpIcon = if ($conv.FixedPoint) { Green "✓ FIXED POINT" } else { Yellow "✗ NOT CONVERGED" }
        Write-Host "    Stage 1: $(White ('{0:N0}' -f $conv.S1Chars)) chars"
        Write-Host "    Stage 3: $(White ('{0:N0}' -f $conv.S3Chars)) chars"
        Write-Host "    Delta:   $(if($conv.CharDelta -eq 0){ Green '0' } else { Red $conv.CharDelta }) chars"
        Write-Host "    Status:  $fpIcon"
    } else {
        Write-Host "    $(Dim 'Stage files not found — run bootstrap to populate')"
    }
    if ($conv.S0Chars) {
        Write-Host "    Stage 0: $(White ('{0:N0}' -f $conv.S0Chars)) chars $(Dim '(reference compiler output)')"
    }
    Write-Host $sep

    # ── TESTS ──
    Write-Host ""
    Write-Host "  $(Bold '🧪 TESTS')  $(Dim "(static count — run 'dotnet test' for live results)")"
    Write-Host "    Test files: $(White $tests.TestFiles)   Test methods: $(White $tests.TestCount)"
    Write-Host $sep

    # ── ACTIONABLE ADVICE ──
    Write-Host ""
    Write-Host "  $(Bold '💡 GUIDANCE')"
    if ($cognitive.Risk -eq "CRITICAL") {
        Write-Host "    $(Red '→ DO NOT assign multi-file changes to the agent right now.')"
        Write-Host "    $(Red '→ Create a mini repro file (like mini-bootstrap.codex) first.')"
        Write-Host "    $(Red '→ Isolate the pipeline stage before engaging the agent.')"
    }
    elseif ($cognitive.Risk -eq "HIGH") {
        Write-Host "    $(Yellow '→ Keep tasks to ONE pipeline stage at a time.')"
        Write-Host "    $(Yellow '→ Pre-load only the relevant .codex file + its test.')"
        Write-Host "    $(Yellow '→ If the agent asks to read >3 files, simplify the problem.')"
    }
    elseif ($cognitive.Risk -eq "MEDIUM") {
        Write-Host "    $(Dim '→ Agent can handle single-stage changes.')"
        Write-Host "    $(Dim '→ Watch for cascading errors — may need to simplify.')"
    }
    else {
        Write-Host "    $(Green '→ Complexity is manageable. Agent should be productive.')"
    }

    if ($gen -and $gen.Objects -gt 10) {
        Write-Host "    $(Yellow "→ $($gen.Objects) 'object' refs in generated C# — type-def map work needed.")"
    }
    if ($gen -and $gen.P0 -gt 20) {
        Write-Host "    $(Yellow "→ $($gen.P0) '_p0_' proxies — partial application type resolution needed.")"
    }
    if ($conv.HasStages -and -not $conv.FixedPoint) {
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
