# Agent Instructions

## Terminal discipline
- **Never run multi-line PowerShell scripts directly in the terminal.** Write a `.ps1` script file, then invoke it with `pwsh -File <path>`. This avoids quoting, escaping, and pipeline issues that cause the terminal to hang waiting for input the agent cannot provide.
- **Never use `Write-Output`, `Write-Host`, or bare expressions for terminal feedback.** These are unreliable in the agent terminal. Instead, write results to a temp file and read it back with `get_file`, or use `edit_file` / `create_file` directly.
- **If a terminal command takes more than a few seconds, assume it is hung.** Do not wait — switch to a file-based approach.
- **Prefer `edit_file` and `create_file` over terminal commands for all file mutations.** The edit_file tool understands code structure and is far more reliable than `Set-Content` or stream redirection through the terminal.
- **Terminal is for read-only queries and build invocations only.** Use it for `dotnet build`, `Select-String` searches, `Get-ChildItem` listings, and similar one-liner reads. Never for multi-step file edits.

## When you get stuck
- **The user is available mid-prompt.** If you need guidance, write a small script that prompts for console input (`Read-Host`) and run it. The user will answer.
- **If a tool call fails or produces no output, do not retry the same approach.** Switch strategies: use a different tool, write a script file, or ask the user.
- **If you are about to attempt something you've already failed at, stop and reconsider.** Two failures on the same approach means the approach is wrong.

## File editing rules
- **Always read a file before editing it** unless you just created it.
- **Use `edit_file` with enough surrounding context** (unique lines above and below the change) so the tool can locate the edit site unambiguously. If an edit fails, re-read the file and provide more context lines.
- **For XAML files, be especially careful** — the edit tool can misidentify repeated structural patterns. Provide the nearest unique `x:Key`, `x:Name`, or comment line as an anchor.

## Scope of authority
- This workspace contains multiple projects across `SpatialDbLib`, `SparseLattice`, `NeuralNavigator`, `Spark`, `SpatialGame`, `SpatialDbApp`, `Common.Core`, and `Common.Wpf`. External repositories (`helix-toolkit-develop3`, `LVSCP`) are referenced but not owned — do not modify files inside them.
- Each project with a `CONTRIBUTING.md` has its own style rules. Read and follow them. They override any general heuristic.
- Diagnostic-only code (`#if DIAGNOSTIC`) is a special case — skip it unless explicitly asked to modify it.
