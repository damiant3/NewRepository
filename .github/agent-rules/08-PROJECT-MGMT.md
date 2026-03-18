# Project Management

Rules for handoffs, getting stuck, and managing multi-session work.

---

## Handoff Documents

After any session that accomplishes meaningful work, create or update a handoff document.

### Template

```markdown
# Iteration N — Handoff Summary

**Date**: [run checkdate() — do NOT type from memory]
**Agent**: [name, model, platform]
**Branch**: `master` (or working branch name)
**Remote**: https://github.com/damiant3/NewRepository

---

## What Was Done

[Brief summary of work completed]

## What's Left

[Anything incomplete or deferred]

## Known Issues

[Bugs, limitations, or things the next agent should watch out for]

## Build Status

- `dotnet build Codex.sln`: [PASS/FAIL]
- `dotnet test Codex.sln`: [PASS — N tests]
```

### Where Handoffs Live

- `docs/OldStatus/ITERATION-N-HANDOFF.md` — per-iteration summaries
- `docs/OldStatus/FORWARD-PLAN.md` — single source of truth for project direction
- `docs/OldStatus/DECISIONS.md` — architecture decision log (append-only)

---

## When You Get Stuck

### The Two-Failures Rule

If a tool call fails or produces no output, do not retry the same approach.
If you are about to attempt something you've already failed at, **stop**.

Two failures on the same approach means the approach is wrong.

### Switching Strategies

| Failed approach | Try instead |
|----------------|-------------|
| `edit_file` on a large file | Write-full-file strategy or partial class |
| Terminal command hung | Write a script file, execute it, read results |
| Build fails after edit | Restore from `.bak`, re-read the file, try a smaller change |
| Can't find the right file | `grep -r` or `Select-String` for a known string |
| Type error you don't understand | Write a minimal reproduction in `samples/` |

### Ask the User

The user is available mid-task. If you need:
- A design decision
- Clarification on requirements
- Permission to modify a spec document
- Help understanding an error

...just ask. A 10-second question is better than a 10-minute wrong turn.

---

## Decision Logging

When you make a non-trivial architectural decision during a session, log it in
`docs/OldStatus/DECISIONS.md` using this format:

```markdown
## Decision: [Short Title]
**Date**: [checkdate()]
**Context**: [What problem or question prompted this]
**Decision**: [What was decided]
**Rationale**: [Why this choice over alternatives]
**Consequences**: [What this enables or limits]
```

---

## Multi-Agent Coordination

When both agents are active on the same repository:

1. **Check `git status` and `git log` at session start.** See what the other agent did.
2. **Work on separate areas.** If the Windows agent is on the type checker, the Linux
   agent should work on docs or a different subsystem.
3. **Use branches.** See [07-GIT-WORKFLOW.md](07-GIT-WORKFLOW.md).
4. **Leave breadcrumbs.** If you notice something the other agent should know about,
   leave a note in the relevant handoff doc or create a `TODO:` comment in the code.
