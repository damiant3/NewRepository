# Agent Toolkit

PowerShell scripts that work around VS agent tool limitations.
These are **the** way to do file inspection, diffing, and test running
during agent sessions. Use them instead of the built-in tools always.

## Tools

| Script | Replaces | Why |
|--------|----------|-----|
| `peek.ps1` | `get_file` | Never drops line 1. Numbered output. Full-file mode. |
| `fstat.ps1` | (nothing) | Line/char/byte counts for files or globs. Essential for edit strategy decisions. |
| `sdiff.ps1` | manual `.bak` workflow | Snapshot before edit, diff after, restore if broken. |
| `trun.ps1` | `dotnet test` in terminal | Filters output to failures + summary. No truncation. |
| `gstat.ps1` | `git status` + `git log` | One command: branch, dirty files, ahead/behind, recent commits, feature branches. |

## Usage

```powershell
# Read a file reliably (lines 1-30)
pwsh -File tools/agent/peek.ps1 src/Codex.Types/TypeChecker.cs 1 30

# Full file with line count
pwsh -File tools/agent/peek.ps1 src/Codex.Types/TypeChecker.cs 0 0

# File stats (how big is this file? do I need the large-file strategy?)
pwsh -File tools/agent/fstat.ps1 src/Codex.Types/TypeChecker.cs
pwsh -File tools/agent/fstat.ps1 src/Codex.Types/*.cs

# Snapshot before editing
pwsh -File tools/agent/sdiff.ps1 snap src/Codex.Types/Unifier.cs

# Diff after editing (verify only intended changes)
pwsh -File tools/agent/sdiff.ps1 diff src/Codex.Types/Unifier.cs

# Restore if edit went wrong
pwsh -File tools/agent/sdiff.ps1 restore src/Codex.Types/Unifier.cs

# Clean up all snapshots at session end
pwsh -File tools/agent/sdiff.ps1 clean

# Run all tests (summary only)
pwsh -File tools/agent/trun.ps1

# Run one project's tests
pwsh -File tools/agent/trun.ps1 -Project Types

# Run tests matching a name
pwsh -File tools/agent/trun.ps1 -Filter "Linear"

# Git status overview
pwsh -File tools/agent/gstat.ps1
```

## Rules

1. **Use `peek.ps1` instead of `get_file`** when line 1 matters or you need reliable line numbers.
2. **Use `fstat.ps1` before editing** any file you haven't seen — know the size before choosing an edit strategy.
3. **Use `sdiff.ps1 snap` before every non-trivial edit** to large files. Diff after. This is non-negotiable.
4. **Use `trun.ps1` instead of raw `dotnet test`** — it captures full output to a file, then extracts failures and summaries. No truncation.
5. **Use `gstat.ps1` at session start** to orient yourself.
6. **Clean up snapshots** (`sdiff.ps1 clean`) before ending a session.
