# Agent Meta Rules

These rules apply to **all agents** working on the Codex repository, regardless of
platform (Windows/Copilot, Linux/Claude, or any future agent).

---

## Agent Identity

Each agent session must identify itself in any handoff document or commit message:

| Field | Example |
|-------|---------|
| Agent name | `Copilot (Claude Opus 4.6, VS 2022)` or `Claude (Opus 4.6, claude.ai)` |
| Platform | `Windows` or `Linux` |
| Session date | Result of `checkdate()` (see below) |

---

## The checkdate() Rule

**Never trust training-data memory for the current date.**

Before writing any date in a document, commit message, or handoff file, the agent
MUST query the system clock:

### Windows (PowerShell)
```powershell
Get-Date -Format "yyyy-MM-dd"
```

### Linux (Bash)
```bash
date +%Y-%m-%d
```

### The Rule

- If you are about to write a date, run the command first.
- If a handoff document template has a `**Date**:` field, fill it from the system clock.
- If you notice a date in an existing file that looks wrong (e.g., `2025-06-21` when the
  project started on `2026-03-13`), flag it to the user — do not silently "fix" dates in
  specification documents without confirmation.
- The Codex project began on or around **2026-03-13**. Any date before that in an
  iteration handoff, decision log, or session document is an error from a previous
  agent hallucinating dates from training data.

---

## Session Hygiene

1. **Read before you write.** Always read a file before editing it.
2. **Build before you commit.** Always run `dotnet build Codex.sln` and `dotnet test Codex.sln`.
3. **Clean up temp files.** Delete `.bak`, `.new`, `.tmp`, and scratch scripts before ending a session.
4. **One logical change per commit.** Do not bundle unrelated fixes.
5. **Leave a handoff.** If your session accomplishes anything meaningful, update or create
   a handoff document so the next agent (or the next session of you) can pick up cleanly.

---

## When You Don't Know Something

- **Ask the user.** The user is available mid-task. A 10-second question saves a 10-minute mistake.
- **Search before assuming.** If unsure about project conventions, check `CONTRIBUTING.md`,
  the copilot instructions, or the relevant `agent-rules/` file.
- **Don't invent conventions.** If you're about to introduce a new pattern (naming, file layout,
  architecture), check whether one already exists. If not, propose it to the user first.
