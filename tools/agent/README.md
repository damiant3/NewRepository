# Agent Toolkit

Platform-specific scripts that work around agent tool limitations.
These are **the** way to do file inspection, diffing, and test running
during agent sessions. Use them instead of the built-in tools.

Each tool has a PowerShell (`.ps1`) version for the Windows agent and a
Bash (`.sh`) version for the Linux agent. They are functionally identical.

## Tools

| Purpose | Windows | Linux | Why |
|---------|---------|-------|-----|
| File reader | `peek.ps1` | `peek.sh` | Never drops line 1. Numbered output. Full-file mode. |
| File stats | `fstat.ps1` | `fstat.sh` | Line/char/byte counts for files or globs. Edit strategy decisions. |
| Snapshot/diff | `sdiff.ps1` | `sdiff.sh` | Snapshot before edit, diff after, restore if broken. |
| Test runner | `trun.ps1` | `trun.sh` | Filters output to failures + summary. No truncation. |
| Git status | `gstat.ps1` | `gstat.sh` | Branch, dirty files, ahead/behind, recent commits, feature branches. |

## Additional Linux Tools

| Script | Purpose |
|--------|---------|
| `tools/linux-bootstrap.sh` | Full Stage 0→1→2→3 bootstrap verification with fixed-point check |
| `tools/codexdashboard.sh` | Cognitive load dashboard (mirrors `codex-dashboard.ps1`) |

## Usage — Windows (PowerShell)

```powershell
pwsh -File tools/agent/peek.ps1 src/Codex.Types/TypeChecker.cs 1 30
pwsh -File tools/agent/peek.ps1 src/Codex.Types/TypeChecker.cs 0 0
pwsh -File tools/agent/fstat.ps1 src/Codex.Types/TypeChecker.cs
pwsh -File tools/agent/fstat.ps1 src/Codex.Types/*.cs
pwsh -File tools/agent/sdiff.ps1 snap src/Codex.Types/Unifier.cs
pwsh -File tools/agent/sdiff.ps1 diff src/Codex.Types/Unifier.cs
pwsh -File tools/agent/sdiff.ps1 restore src/Codex.Types/Unifier.cs
pwsh -File tools/agent/sdiff.ps1 clean
pwsh -File tools/agent/trun.ps1
pwsh -File tools/agent/trun.ps1 -Project Types
pwsh -File tools/agent/trun.ps1 -Filter "Linear"
pwsh -File tools/agent/gstat.ps1
```

## Usage — Linux (Bash)

```bash
bash tools/agent/peek.sh src/Codex.Types/TypeChecker.cs 1 30
bash tools/agent/peek.sh src/Codex.Types/TypeChecker.cs 0 0
bash tools/agent/fstat.sh src/Codex.Types/TypeChecker.cs
bash tools/agent/fstat.sh 'src/Codex.Types/*.cs'
bash tools/agent/sdiff.sh snap src/Codex.Types/Unifier.cs
bash tools/agent/sdiff.sh diff src/Codex.Types/Unifier.cs
bash tools/agent/sdiff.sh restore src/Codex.Types/Unifier.cs
bash tools/agent/sdiff.sh clean
bash tools/agent/trun.sh
bash tools/agent/trun.sh -p Types
bash tools/agent/trun.sh -f "Linear"
bash tools/agent/gstat.sh

# Full bootstrap verification (Stage 0 → 1 → 2 → 3, fixed-point check)
bash tools/linux-bootstrap.sh
bash tools/linux-bootstrap.sh --skip-build     # skip dotnet build Codex.sln
bash tools/linux-bootstrap.sh --stage1-only    # stop after Stage 1
```

## Rules

1. **Use `peek` instead of built-in file readers** when line 1 matters or you need reliable line numbers.
2. **Use `fstat` before editing** any file you haven't seen — know the size before choosing an edit strategy.
3. **Use `sdiff snap` before every non-trivial edit** to large files. Diff after. This is non-negotiable.
4. **Use `trun` instead of raw `dotnet test`** — it captures full output to a file, then extracts failures and summaries. No truncation.
5. **Use `gstat` at session start** to orient yourself.
6. **Clean up snapshots** (`sdiff clean`) before ending a session.
