# Windows Agent Notes

Notes from the Windows agent (Copilot in VS) that supplement the other rule files.
Created 2026-03-18 during the first mutual-review session.

---

## Review of Linux Agent's Rule Decomposition

Reviewed by: Copilot (VS 2022, Windows)
Date: 2026-03-18 (verified via `Get-Date`)

### Overall Assessment

The decomposition from the monolithic `copilot-instructions.md` into 9 modular files
is **well done**. Nothing important was lost. The new structure is easier to navigate
and the per-file focus means agents can load only what they need. Specific findings:

### 00-META.md — ✅ Good
- The `checkdate()` rule is the single most important addition. Prior sessions
  hallucinated dates from training data (June 2025 for a project that started
  March 2026). This rule prevents recurrence.
- Session hygiene rules are clear and actionable.

### 01-CODE-STYLE.md — ✅ Good, matches CONTRIBUTING.md
- All rules from CONTRIBUTING.md are present.
- The `Map<K,V>` preference and `new()` target-type rules are good additions that
  weren't in the original CONTRIBUTING.md.
- Codex (.codex) style section is a useful addition.

### 02-TERMINAL.md — ⚠️ Minor correction needed
- See "PowerShell Version" section below.
- Otherwise accurate for the Windows agent's environment.

### 03-FILE-EDITING.md — ⚠️ See corrections below
- The "Write-Full-File Strategy" for large files is good advice but overstates the
  problem. See "edit_file Reliability" below.
- The backup workflow is sound practice.

### 04-SCOPE.md — ✅ Good
- Clear, complete, matches the old instructions.

### 05-BUILD-VERIFY.md — ✅ Good
- Bootstrap verification section is a valuable addition.
- Test count says "654+" — actual count is 722+ as of this session.

### 06-PIPELINE.md — ✅ Good
- Accurate and complete. The backend status table is useful.

### 07-GIT-WORKFLOW.md — ✅ Good, workable
- The dual-agent review workflow is clear and practical.
- The simplified flow for single-agent sessions with user supervision is a good escape
  hatch that avoids over-ceremony.
- The `--no-ff` merge flag is the right call for preserving review history.

### 08-PROJECT-MGMT.md — ✅ Good
- The "Two-Failures Rule" is excellent — matches real experience.
- Handoff template with `checkdate()` reminder is well-placed.

---

## PowerShell Version

This workspace has **PowerShell 7+** (`pwsh.exe`) installed, so the `pwsh -File`
command in `02-TERMINAL.md` is correct. Note that the VS terminal's
`run_command_in_terminal` tool runs commands directly and handles this transparently
— the `.ps1` file approach is rarely needed because single-line commands work fine
in the agent terminal.

If `pwsh` is ever unavailable, fall back to `powershell -File` (Windows PowerShell 5.1).

---

## edit_file Reliability

The `03-FILE-EDITING.md` rule states that `edit_file` is "unreliable on large files"
and "silently corrupts unrelated lines." From the Windows agent's experience:

### When edit_file Works Well
- Files of any size when the edit is **well-localized** (changing a few lines in one place).
- When sufficient context is provided (unique surrounding lines).
- When the agent provides concise diffs with `// ...existing code...` markers.

### When edit_file Struggles
- **Multiple dispersed edits** in a single call on a large file.
- **Ambiguous context** — if the same pattern appears multiple times in a file, the
  tool may edit the wrong occurrence.
- **Whitespace-sensitive edits** where indentation matters but the context doesn't
  make the indentation level clear.

### Practical Advice
- For large files, prefer **multiple small `edit_file` calls** over one big one.
- The "write-full-file" strategy (create `.new`, swap) is a last resort, not the default.
  It's needed maybe 5% of the time, not the majority.
- The partial class strategy is genuinely useful for adding methods to large classes.
- Always re-read the file after editing to verify the result.

---

## get_file Tool Quirk: First Line Dropped

The `get_file` tool frequently fails to return the first line of a file, especially
markdown files that start with `# Heading`. The tool shows the file starting at what
is actually line 2, making it appear the heading is missing.

**Workaround**: When the first line matters (e.g., verifying a heading exists), use
the terminal instead:

```powershell
Get-Content "path/to/file" -TotalCount 5
```

**Impact on edits**: If you trust `get_file` and think line 1 is blank, you may
accidentally delete the heading when editing. Always verify line 1 via the terminal
before editing the top of any file.

---

## Terminal Limitations in VS

### Output Truncation
The `run_command_in_terminal` tool truncates output beyond ~4,000 characters. For
commands that produce long output (e.g., `dotnet test` with many test results), only
the tail end is returned. Workarounds:
- Pipe to `Select-Object -Last N` for focused output.
- Use `Select-String` to filter for specific patterns (e.g., `Failed`).
- Use `Out-File` to write to a temp file, then read with `get_file`.

### No Interactive Input
The terminal cannot receive interactive input after a command starts. Commands that
prompt for input (e.g., `dotnet new` with template selection, `git` with credential
prompts) will hang. Always use non-interactive flags.

### Encoding
The terminal uses the system's default encoding, which on Windows is typically
Windows-1252, not UTF-8. This means Unicode characters in command output may appear
garbled (e.g., `—` appearing as `ΓÇö`). This is cosmetic and does not affect file
content written via `create_file` / `edit_file`, which correctly use UTF-8.

---

## PowerShell Pitfalls

### Semicolons for Multi-Statement Lines
PowerShell in the agent terminal treats each tool call as a single command. To chain
commands, use semicolons: `cd D:\path; git status`. Do NOT use `&&` (that's bash) or
line breaks (the tool sends the entire string as one command).

### Select-String vs grep
Use `Select-String -Pattern "foo" -Path src/**/*.cs` instead of `grep`. Note that
`-Path` with `**` globbing works in PowerShell 5.1 but only one level deep. For
truly recursive search: `Get-ChildItem -Recurse -Filter *.cs | Select-String "foo"`.

### Path Separators
Windows uses `\` but PowerShell accepts `/` in most contexts. Git commands should
use `/` for consistency with `.gitignore` patterns. The `get_file` and `edit_file`
tools accept both separators.

### Get-Content vs cat
`Get-Content` (alias `gc`, `cat`, `type`) returns an array of lines, not a string.
For line counting: `(Get-Content file.txt).Count`. For string operations, use
`Get-Content file.txt -Raw`.

---

## Things the Linux Agent Got Right

For the record, these things were particularly well done:

1. **The `checkdate()` concept itself.** This is the most impactful rule added. The
   date hallucination problem was real and caused confusion in the handoff docs.

2. **Decomposing into numbered files.** The ordering (`00` through `08`) creates a
   natural reading order and makes it easy to reference specific topics.

3. **The DATE-AUDIT.md document.** Cataloging the problems before fixing them is good
   practice. The suggested fixes with commit hashes are helpful.

4. **Keeping both copilot-instructions.md files in sync.** The root copy and the
   `.github/` copy are identical, which is correct — different tools read from
   different locations.

5. **The "Who Watches the Watcher?" framing.** The mutual-review workflow is a genuine
   improvement over the old "no commits" restriction, which was unworkable for
   productive sessions.
