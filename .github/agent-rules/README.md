# Agent Rules

Modular behavior rules for AI agents working on the Codex repository.

## Who Watches the Watcher?

Two agents collaborate on this repo — a Windows agent (Copilot in VS) and a Linux
agent (Claude on claude.ai). These rules ensure both agents work safely, consistently,
and can review each other's work.

## File Index

| # | File | One-Line Summary |
|---|------|-----------------|
| 00 | [META](00-META.md) | Date checking, identity, session hygiene |
| 01 | [CODE-STYLE](01-CODE-STYLE.md) | C# naming, formatting, type conventions |
| 02 | [TERMINAL](02-TERMINAL.md) | Platform-specific terminal rules |
| 03 | [FILE-EDITING](03-FILE-EDITING.md) | Edit strategies by file size |
| 04 | [SCOPE](04-SCOPE.md) | What you can and can't modify |
| 05 | [BUILD-VERIFY](05-BUILD-VERIFY.md) | Build/test requirements |
| 06 | [PIPELINE](06-PIPELINE.md) | Compiler architecture quick reference |
| 07 | [GIT-WORKFLOW](07-GIT-WORKFLOW.md) | Dual-agent branch + review workflow |
| 08 | [PROJECT-MGMT](08-PROJECT-MGMT.md) | Handoffs, decisions, stuck-recovery |
| 09 | [WINDOWS-NOTES](09-WINDOWS-NOTES.md) | VS-specific tool quirks, PowerShell pitfalls, review of Linux agent's work |

## How to Use

**At session start**, read `00-META.md` (always applies) plus whichever files are
relevant to your task. For a typical coding session, that's `01`, `03`, `05`, and `07`.
For a docs-only session, `00`, `04`, and `08`.

**Don't read all files every time** — they're separated so you can load only what you need.
