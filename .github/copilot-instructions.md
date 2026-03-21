# Copilot Instructions for Codex

> **ALL RULES ARE IN THIS FILE.** Do not skip sections. Do not defer to external
> files. Everything an agent needs to know is here.

## What This Repository Is

Codex is a self-hosting programming language compiler. The compiler is written in
Codex and compiles itself. The C# bootstrap implementation (.NET 8) is locked.
The solution is `Codex.sln`. The compiler pipeline:

```
Source (.codex) -> Lexer -> Parser -> Desugarer -> NameResolver -> TypeChecker -> Lowering -> Emitter -> dotnet/node/rustc/...
```

12 backends (C#, JavaScript, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage).
843+ tests. Self-hosting achieved. Content-addressed fact store with collaboration protocol.

Design docs: `docs/00-OVERVIEW.md` (project overview) and `docs/10-PRINCIPLES.md`
(engineering principles) live in docs root. Bootstrap-era design docs (01-09, Glossary)
are archived in `docs/MM1/`. Do not modify Vision docs without explicit permission.




---

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


---

## Agent Toolkit (USE THESE)

The primary agent tool is `codex-agent` — a unified toolkit written in Codex itself,
compiled to a standalone .exe via the IL backend. It lives in `tools/codex-agent/`.

### codex-agent (preferred)

```
dotnet tools/codex-agent/codex-agent.exe <command> [args]
```

| Command | What it does |
|---------|-------------|
| `peek <file> [start] [end]` | Read file lines safely with line numbers. Fixes the `get_file` first-line-drop bug. |
| `stat <file> [file2] ...` | File statistics: line count, char count, size hints, totals. **Run before editing.** |
| `snap save <file>` | Snapshot before editing. **Non-negotiable for files >100 lines.** |
| `snap diff <file>` | Compare current file to snapshot — verify only intended changes. |
| `snap restore <file>` | Restore from snapshot if edit goes wrong. |
| `status` | Project health check — verifies key files exist. |
| `plan add <task>` | Stash a task that survives context window loss. |
| `plan` / `plan show` | Show current task list. |
| `plan clear` | Clear the task list. |
| `check` | Cognitive load estimate — hot-path file sizes vs 60K budget, load level, plan status. |
| `help` | Usage information. |

### Session Start Checklist

```powershell
dotnet tools/codex-agent/codex-agent.exe check    # Cognitive load + plan
dotnet tools/codex-agent/codex-agent.exe status    # Key files present?
```

### Before Editing Any File

```powershell
dotnet tools/codex-agent/codex-agent.exe stat <file>       # How big is it?
dotnet tools/codex-agent/codex-agent.exe peek <file> 1 30  # Read the top
dotnet tools/codex-agent/codex-agent.exe snap save <file>  # Snapshot before edit
# ... make edit ...
dotnet tools/codex-agent/codex-agent.exe snap diff <file>  # Verify only intended changes
```

### If an Edit Goes Wrong

```powershell
dotnet tools/codex-agent/codex-agent.exe snap restore <file>  # Restore from snapshot
```

### Stashing Tasks Across Context Loss

The `plan` command writes to `.codex-agent/plan.txt` — a file that persists across
conversation resets. Use it to record multi-step plans, TODOs, or notes that would
otherwise be lost when the context window rolls over.

```powershell
dotnet tools/codex-agent/codex-agent.exe plan add "Fix the lowerer inference bug"
dotnet tools/codex-agent/codex-agent.exe plan add "Update tests for new pattern"
dotnet tools/codex-agent/codex-agent.exe plan       # Review at start of next session
```

### Legacy Shell Scripts (fallback)

The old PowerShell/Bash scripts in `tools/agent/` still work if `codex-agent.exe`
is unavailable. They are deprecated in favor of the unified tool.

| Tool | Windows | Linux |
|------|---------|-------|
| peek | `pwsh -File tools/agent/peek.ps1 <file> <start> <end>` | `bash tools/agent/peek.sh` |
| fstat | `pwsh -File tools/agent/fstat.ps1 <file>` | `bash tools/agent/fstat.sh` |
| sdiff | `pwsh -File tools/agent/sdiff.ps1 snap <file>` | `bash tools/agent/sdiff.sh` |
| trun | `pwsh -File tools/agent/trun.ps1` | `bash tools/agent/trun.sh` |
| gstat | `pwsh -File tools/agent/gstat.ps1` | `bash tools/agent/gstat.sh` |
| dashboard | `pwsh -File tools/codex-dashboard.ps1` | `bash tools/codexdashboard.sh` |

---

# Code Style Rules

These rules govern all source code in the Codex repository. They are enforced by
`TreatWarningsAsErrors` in `Directory.Build.props` — violations are build failures.

---

## C# Code Style

### Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Private instance fields | `m_` prefix | `m_diagnostics`, `m_localEnv` |
| Private readonly fields | `m_` prefix + `readonly` | `readonly List<Token> m_tokens` |
| Properties, types | PascalCase | `SourceSpan`, `TokenKind` |
| Local variables, parameters | camelCase | `localEnv`, `tokenIndex` |
| Constants | PascalCase | `MaxLineLength` |

### Type Declarations

- `sealed record` for immutable reference types.
- `readonly record struct` for small value types.
- Use `new()` instead of `new TypeName()` when the target type is declared on the left side.
  ```csharp
  // Good
  Map<string, CodexType> m_map = Map<string, CodexType>.s_empty;
  // Bad
  Map<string, CodexType> m_map = new Map<string, CodexType>();
  ```

### Accessibility Modifiers

- **Omit default modifiers.** Do not write `private` on class members or `internal` on
  top-level types — those are the C# defaults.
- Only write an accessibility modifier when it differs from the default (`public`, `protected`,
  `internal` on a member).

### Null Safety

- **Prefer `Map<K,V>` (in `Codex.Core`)** over `ImmutableDictionary` + `TryGetValue`.
  `Map<K,V>` returns `null` on missing keys instead of throwing or requiring `out` patterns.
- If a .NET abstraction has bad null behavior, clone it null-safe in `Codex.Core`.

### Using Directives

- Do not add `using` directives speculatively.
- These are **implicit** (global usings): `System`, `System.Collections.Generic`, `System.Linq`,
  `System.IO`, `System.Net.Http`, `System.Threading`, `System.Threading.Tasks`.
- `System.Collections.Immutable` is **NOT** implicit — add it only when the file uses
  `ImmutableArray`, `ImmutableDictionary`, etc.
- When you add a using, check if fully-qualified names in the same file can now be shortened.

### Formatting

- 4 spaces indentation (no tabs).
- UTF-8 encoding.
- Maximum 120 characters per line.
- End files with a single newline.

### What Not to Write

- **No XML doc comments.** Do not add `///` comments. Code should be self-documenting.
  Only add a comment if it prevents re-discovering a non-obvious decision.
- **No `var` when the type is not obvious** from the right-hand side.
- **No unused fields, variables, or parameters.** `TreatWarningsAsErrors` catches these.

---

## Codex (.codex) Code Style

Codex source files follow the language's own conventions:

- Boolean literals: `True` / `False` (capital T/F), not `true`/`false`.
- Function application is left-associative: `f x y` means `(f x) y`, not `f (x y)`.
- Pattern matching uses `when`/`if` syntax, not `match`/`case`.
- All functions are curried by default.

---

## Test Style

- Use xUnit for all test projects.
- Match the style of existing test files in each project.
- Test files live in the corresponding `tests/` project (see `CONTRIBUTING.md` for the mapping).
- Private fields in test classes also use the `m_` prefix.


---

# Terminal Discipline

Agents interact with the system through terminal commands. Each platform has its own
failure modes. These rules prevent the most common ones.

---

## Windows Agent (Copilot / VS Terminal / PowerShell)

### Never Run Multi-Line Scripts Inline

Multi-line PowerShell scripts pasted into the agent terminal hang waiting for input
the agent cannot provide. Always:

1. Write the script to a `.ps1` file using `create_file`.
2. Invoke it with `pwsh -File <script.ps1>`.
3. Delete the `.ps1` file when done.

### No Write-Output / Write-Host

`Write-Output`, `Write-Host`, and bare expressions are unreliable in the agent terminal.
Instead, write results to a temp file and read it back, or use `create_file` / `edit_file`.

### Hung Commands

If a terminal command takes more than a few seconds, assume it is hung. Kill it and
switch to a file-based approach.

### Allowed Terminal Uses

The terminal is for **read-only queries and build invocations only**:

```powershell
dotnet build Codex.sln
dotnet test Codex.sln
Select-String -Pattern "foo" -Path src/**/*.cs
Get-ChildItem -Recurse -Filter *.codex
git log --oneline -20
git status
git diff --stat
```

---

## Linux Agent (Claude / Bash)

### Prefer Simple Commands

Stick to one-liners where possible. For complex operations, write a bash script,
execute it, and clean up.

### Allowed Terminal Uses

```bash
dotnet build Codex.sln
dotnet test Codex.sln
grep -r "pattern" src/
find . -name "*.codex" -type f
git log --oneline -20
git status
git diff --stat
date +%Y-%m-%d    # checkdate()
```

### Long-Running Commands

If a command takes more than 30 seconds, it is probably wrong. Check:
- Are you building the entire solution when you only need one project?
- Are you running all tests when you only need one test class?

---

## Both Platforms

### Prefer File Tools Over Terminal for Mutations

- Use `edit_file` / `create_file` (Copilot) or `str_replace` / `create_file` (Claude)
  for all file modifications.
- The terminal is for reading, building, and testing — not for writing files via
  `echo`, `Set-Content`, or redirects.

### Git Commands in Terminal

Git commands are read-only by default. For write operations (commit, push, checkout),
see the [Git Workflow rules](07-GIT-WORKFLOW.md).


---

# File Editing Rules

File editing tools are the primary way agents modify code. They are also the primary
source of silent corruption. These rules exist because real bugs have been caused by
careless edits.

---

## Golden Rule: Read Before You Edit

**Always read a file before editing it**, unless you just created it in this session.
Use `peek` (not `get_file`) to avoid the first-line-drop bug:

```powershell
dotnet tools/codex-agent/codex-agent.exe peek <file> 1 30  # read lines 1-30
dotnet tools/codex-agent/codex-agent.exe peek <file> 0 0   # read entire file
```

---

## Backup Before Editing

Use `snap save` before making non-trivial edits. This replaces the manual `.bak`
workflow and provides diff verification:

```powershell
dotnet tools/codex-agent/codex-agent.exe stat <file>       # check size -> pick strategy
dotnet tools/codex-agent/codex-agent.exe snap save <file>  # snapshot before edit
# ... make edit ...
dotnet tools/codex-agent/codex-agent.exe snap diff <file>  # verify only intended changes
```

If an edit goes wrong:

```powershell
dotnet tools/codex-agent/codex-agent.exe snap restore <file>  # revert to snapshot
```

**Non-negotiable for files over 100 lines.** Clean up snapshots (delete `.snap` files)
before ending a session.

---

## Small Files (< 100 lines)

`edit_file` / `str_replace` is acceptable. Verify the result immediately after by
reading the file back.

---

## Medium Files (100–300 lines)

Use `edit_file` / `str_replace` with **generous context** — unique lines above and
below the change site so the tool can locate the edit unambiguously. If an edit fails,
re-read the file and provide more context. **Always `sdiff snap` first.**

---

## Large Files (> 300 lines)

### Copilot (Windows): Write-Full-File Strategy

The `edit_file` tool is unreliable on large files. It silently corrupts unrelated
lines or truncates content.

**Required workflow:**

1. `sdiff snap` the file.
2. Write the complete new file to `<filename>.new` using `create_file`.
3. Swap: copy `<filename>.new` to `<filename>` in the terminal.
4. `sdiff diff` to confirm only intended changes.
   Check line count: `fstat` on old vs new.
5. If the build fails: `sdiff restore`, inspect, and retry.
6. Clean up `.new` files when done.

### Partial Class Strategy

When a file exceeds ~300 lines and you need to add multiple methods, **use a partial
class file**. Create a second file (e.g., `Program.Collaboration.cs`) with `partial class`
containing the new methods. This keeps edits small and avoids the large-file corruption
problem. Merge back when stable.

### Claude (Linux): Iterative str_replace

Claude's `str_replace` tool requires exact unique string matches. For large files:

1. Use `view` with line ranges to inspect the area you need to change.
2. Apply `str_replace` with the exact string (no line-number prefixes).
3. Re-view the file after each edit — earlier view output is stale.
4. For multi-site edits, work top-to-bottom (earlier line numbers stay stable).

---

## What Never to Do

- **Never print a full file as a code block** and ask the user to paste it.
- **Never use terminal redirects** (`>`, `Set-Content`) for file creation when
  file-creation tools are available.
- **Never retry a failed edit approach.** If `edit_file` / `str_replace` fails,
  re-read the file and use a different strategy.


---

# Scope of Authority

What agents are allowed to modify, and what is off-limits.

---

## Freely Modifiable

Agents may create, edit, and delete files in these locations without special permission:

| Location | Contains |
|----------|----------|
| `src/` | All compiler source projects |
| `tests/` | All test projects |
| `tools/` | CLI, bootstrap, VS extension |
| `samples/` | Sample `.codex` programs |
| `editors/` | VS Code extension |
| `generated-output/` | Regenerated backend output corpus |
| `Codex.Codex/` | Self-hosted compiler source and output |
| `.github/agent-rules/` | These rule files (with user approval for policy changes) |

---

## Modifiable With Care

These files affect the whole repository. Edit only when the task requires it, and
verify the build afterward:

| File | What it governs |
|------|-----------------|
| `README.md` | Project overview — keep accurate |
| `CONTRIBUTING.md` | Contributor rules — keep in sync with agent rules |
| `.github/copilot-instructions.md` | Root agent instructions (references agent-rules/) |
| `copilot-instructions.md` | Legacy root instructions (keep in sync) |
| `.gitignore` | Ignored files — add patterns as needed |
| `Codex.sln` | Solution file — add new projects as needed |

---

## Do Not Modify (Without Explicit User Request)

| File(s) | Why |
|---------|-----|
| `Directory.Build.props` | Governs the entire solution build (TreatWarningsAsErrors, TFM, etc.) |
| `docs/Vision/NewRepository.txt` | Original vision document |
| `docs/Vision/IntelligenceLayer.txt` | Design philosophy essay |

If you believe one of these files needs updating, ask the user first and explain why.

---

## Handoff and Status Documents

Agents **should** create and update handoff documents in `docs/OldStatus/` or `docs/`
after meaningful work. Use the `checkdate()` rule for all dates.

| File pattern | Purpose |
|-------------|---------|
| `docs/OldStatus/ITERATION-N-HANDOFF.md` | Per-iteration summary |
| `docs/OldStatus/FORWARD-PLAN.md` | Single source of truth for project direction |
| `docs/OldStatus/DECISIONS.md` | Architecture decision log |
| `docs/PostFixedPointCleanUp.md` | Post-bootstrap cleanup tracker |


---

# Build and Verify

Every task that touches source code must end with a green build and passing tests.
No exceptions.

---

## The Two Commands

```sh
dotnet build Codex.sln    # Must produce zero warnings (warnings are errors)
dotnet test Codex.sln     # All tests must pass
```

Run both before:
- Concluding any task
- Creating a commit
- Writing a handoff document that says "done"

---

## What "Zero Warnings" Means

`TreatWarningsAsErrors` is `true` in `Directory.Build.props`. This means:

- An unused field is a build failure.
- An unused variable is a build failure.
- An unused parameter is a build failure.
- A missing `using` is a build failure.
- A redundant `using` is a build failure (in some configurations).

Do not leave dead code. If you add a field, use it. If you remove a usage, remove the field.

---

## Test Expectations

The test suite has 722+ tests across multiple projects:

| Test Project | Covers |
|-------------|--------|
| `Codex.Core.Tests` | Content hashing, Map, diagnostics |
| `Codex.Syntax.Tests` | Lexer, parser, prose parser |
| `Codex.Ast.Tests` | Desugarer |
| `Codex.Semantics.Tests` | Name resolution |
| `Codex.Types.Tests` | Type checker, unifier, integration tests |
| `Codex.Repository.Tests` | Fact store, collaboration, sync |

If you add a new feature, add tests. If you fix a bug, add a regression test.

---

## Quick Checklist

Before declaring any code task complete:

- [ ] `dotnet build Codex.sln` passes (zero warnings)
- [ ] `dotnet test Codex.sln` passes (all tests green)
- [ ] Private fields use the `m_` prefix
- [ ] No `var` where the type is non-obvious
- [ ] No XML doc comments added
- [ ] No unused fields, variables, or parameters
- [ ] `.bak` and `.new` temp files cleaned up

---

## Bootstrap Verification (When Touching Self-Hosted Pipeline)

If your changes affect `Codex.Codex/` or the bootstrap pipeline:

```sh
dotnet run --project tools/Codex.Bootstrap -- build Codex.Codex
```

Then verify that `Codex.Codex/stage1-output.cs` compiles and the output is
functionally equivalent to `Codex.Codex/out/Codex.Codex.cs`.


---

# Compiler Pipeline Guide

Reference for where things live in the Codex compiler.

---

## The Pipeline

```
Source (.codex) → Lexer → Parser → Desugarer → NameResolver → TypeChecker → Lowering → Emitter → dotnet/node/rustc/...
```

---

## Project Dependency Order

Strictly one-way (upstream → downstream). No reverse references.

```
Codex.Core → Codex.Syntax → Codex.Ast → Codex.Semantics → Codex.Types → Codex.IR → Codex.Emit → Codex.Emit.* → Codex.Cli
```

`Codex.Core` has zero project dependencies (root of graph).
`Codex.Cli` references everything (composition root).

---

## Where to Add Things

| What you're adding | Where it goes |
|--------------------|---------------|
| New CST node | `src/Codex.Syntax/SyntaxNodes.cs` |
| New parser rule | `src/Codex.Syntax/Parser.cs` (or `Parser.Expressions.cs`) |
| New AST node or desugaring | `src/Codex.Ast/` |
| New name resolution rule | `src/Codex.Semantics/NameResolver.cs` |
| New type | `src/Codex.Types/CodexType.cs` |
| New type checker rule | `src/Codex.Types/TypeChecker.cs` |
| New unification rule | `src/Codex.Types/Unifier.cs` |
| New IR node | `src/Codex.IR/IRModule.cs` |
| New lowering rule | `src/Codex.IR/Lowering.cs` |
| New C# emission rule | `src/Codex.Emit.CSharp/CSharpEmitter.cs` |
| New backend | New `src/Codex.Emit.<Target>/` implementing `ICodeEmitter` |
| New CLI command | `tools/Codex.Cli/Program.cs` |
| New LSP feature | `src/Codex.Lsp/` |
| New proof rule | `src/Codex.Proofs/` |
| New repository feature | `src/Codex.Repository/FactStore.cs` |

---

## Backends (12 Total)

| Backend | Project | Status |
|---------|---------|--------|
| C# | `Codex.Emit.CSharp` | Primary, fully featured |
| JavaScript | `Codex.Emit.JavaScript` | Full |
| Python | `Codex.Emit.Python` | Full |
| Rust | `Codex.Emit.Rust` | Full |
| C++ | `Codex.Emit.Cpp` | Full |
| Go | `Codex.Emit.Go` | Full |
| Java | `Codex.Emit.Java` | Full |
| Ada | `Codex.Emit.Ada` | Full |
| Fortran | `Codex.Emit.Fortran` | Full |
| COBOL | `Codex.Emit.Cobol` | Full |
| IL | `Codex.Emit.IL` | Full (generics, TCO, builtins, standalone .exe) |
| Babbage | `Codex.Emit.Babbage` | Analytical Engine, intentionally limited |

All mainstream backends support: records, sum types, pattern matching, recursion,
effects, and tail call optimization.

---

## Self-Hosted Pipeline

The compiler is also written in Codex itself (26 `.codex` files in `Codex.Codex/`).

```
.codex source → [Stage 0: C# compiler] → output.cs (Stage 1)
output.cs     → [dotnet build]          → Stage 1 binary
Stage 1       → [compiles .codex]       → stage1-output.cs (Stage 2)
Stage 2 ≈ Stage 1 → Fixed-point achieved
```

Key files:
- `Codex.Codex/out/Codex.Codex.cs` — Stage 0 output (reference)
- `Codex.Codex/stage1-output.cs` — Stage 2 output
- `tools/Codex.Bootstrap/` — Bootstrap runner


---

# Git Workflow — Who Watches the Watcher?

Two agents work on the Codex repository: a **Windows agent** (Copilot in VS) and a
**Linux agent** (Claude on claude.ai). Neither agent commits directly to `master`.
Instead, they use a branch-based workflow where each agent can review the other's
work before it lands.

---

## The Principle

> No agent merges its own work to master without review.

This is the recursive answer to "who watches the watcher?" — they watch each other.

---

## Branch Naming

| Branch | Purpose |
|--------|---------|
| `master` | Protected. Only receives merges from reviewed staging branches. |
| `windows/<topic>` | Windows agent's working branch |
| `linux/<topic>` | Linux agent's working branch |
| `staging/<topic>` | Reviewed and approved work ready for merge |

Examples:
- `windows/fix-lowerer-inference`
- `linux/rewrite-agent-rules`
- `staging/agent-rules-v2`

---

## The Workflow

### 1. Agent Creates a Working Branch

```bash
git checkout master
git pull origin master
git checkout -b windows/my-feature    # or linux/my-feature
```

### 2. Agent Does Work and Commits

Commits happen on the working branch. Use Conventional Commits:

```bash
git add -A
git commit -m "feat: add exhaustiveness checking for nested patterns"
```

Agents **may commit freely** to their own working branches. The old "no commit"
restriction is lifted. The safety net is the review step, not a prohibition on commits.

### 3. Agent Pushes and Requests Review

```bash
git push origin windows/my-feature
```

Then leave a note in a handoff document or a PR description explaining:
- What was changed and why
- What tests were added or affected
- Any known issues or things the reviewer should check

### 4. The Other Agent Reviews

The reviewing agent:

1. Fetches the branch: `git fetch origin windows/my-feature`
2. Checks it out or diffs it: `git diff master..origin/windows/my-feature`
3. Reads the changes. Checks for:
   - Build passes (`dotnet build Codex.sln`)
   - Tests pass (`dotnet test Codex.sln`)
   - Code style compliance (see [01-CODE-STYLE.md](01-CODE-STYLE.md))
   - No spec documents modified without permission
   - Dates are correct (see [00-META.md](00-META.md))
   - Commit messages are meaningful
4. If approved: merges to a staging branch or directly to master
5. If issues found: leaves notes in a review document for the original agent

### 5. Merge to Master

```bash
git checkout master
git merge --no-ff staging/my-feature -m "merge: agent-rules-v2 (reviewed by linux agent)"
git push origin master
```

The `--no-ff` flag preserves the branch history so it's clear what was reviewed.

---

## Simplified Flow (Single-Agent Sessions)

When only one agent is active and the user is present to supervise, the workflow
can be simplified:

1. Agent works on a topic branch (`windows/topic` or `linux/topic`).
2. User reviews the diff.
3. User approves or the agent merges with user confirmation.

The full dual-review process is for when both agents are working in parallel or
asynchronously.

---

## Pull Request Alternative (GitHub)

If preferred, agents can use GitHub PRs instead of manual branch review:

1. Agent pushes working branch.
2. Agent creates a PR via `gh pr create` (if GitHub CLI is available) or asks the user to create one.
3. The other agent or the user reviews via the GitHub UI.
4. Merge via the PR.

This provides a permanent record of reviews. The branch-based workflow above is
for cases where PR tooling isn't available to the agent.

---

## Emergency: Direct Commit to Master

In rare cases where a direct commit is necessary (e.g., fixing a broken build that
blocks all other work), an agent may commit directly to master with:

- User approval (explicit)
- A commit message prefixed with `EMERGENCY:` explaining why normal review was skipped
- The other agent should review the emergency commit at the next opportunity

---

## Commit Message Format

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add exhaustiveness checking for nested patterns
fix: correct lowerer type inference for binary expressions
docs: update ITERATION-12-HANDOFF.md
test: add regression test for nested match scoping
chore: clean up .bak files and regenerate output corpus
refactor: extract LinearityChecker into separate file
```

---

## What Gets Committed

- Source code changes (`src/`, `tests/`, `tools/`)
- Documentation updates (`docs/`, `README.md`, `CONTRIBUTING.md`)
- Agent rule updates (`.github/agent-rules/`, `.github/copilot-instructions.md`)
- Sample programs (`samples/`)
- Generated output corpus (`generated-output/`) — in separate commits
- Editor support files (`editors/`)

## What Does NOT Get Committed

- `.bak`, `.new`, `.tmp` files
- Build output (`bin/`, `obj/`)
- User-specific files (`.vs/`, `.user`)
- Local repository data (`.codex/`)
- Node modules (`node_modules/`)


---

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

---

## Cognitive Load Meter

The project includes a **cognitive load meter** that tracks how much context budget
the hot-path compiler files consume — predicting when an agent is likely to thrash.

### Quick Check (preferred)

```powershell
dotnet tools/codex-agent/codex-agent.exe check
```

This shows hot-path file sizes vs the 60K budget, load level (GREEN/YELLOW/ORANGE/RED),
and current plan status. Use this at session start and before touching hot-path files.

### Full Dashboard (legacy, more metrics)

```powershell
# Windows
pwsh -File tools/codex-dashboard.ps1

# Linux
bash tools/codexdashboard.sh

# JSON output (for programmatic use)
pwsh -File tools/codex-dashboard.ps1 -Json
