# Copilot Instructions for Codex

> Single file — VS Copilot only reads `.github/copilot-instructions.md`.
> Keep this lean. Every char here is burned on every prompt.

## What This Repository Is

Codex is a self-hosting programming language compiler (written in Codex, compiles itself).
C# bootstrap (.NET 8) is locked. Solution: `Codex.sln`. Pipeline:

```
Source (.codex) → Lexer → Parser → Desugarer → NameResolver → TypeChecker → Lowering → Emitter
```

12 backends. 800+ tests. Self-hosting achieved. Content-addressed fact store.

Design docs: `docs/00-OVERVIEW.md`, `docs/10-PRINCIPLES.md`.
Do not modify Vision docs (`docs/Vision/`) without explicit permission.

---

## Session Rules

1. **Read before you write.** Always read a file before editing it.
2. **Build before you commit.** `dotnet build Codex.sln` + `dotnet test Codex.sln`.
3. **Clean up temp files.** Delete `.bak`, `.new`, `.tmp`, `.snap`, scratch scripts before ending.
4. **One logical change per commit.** Don't bundle unrelated fixes.
5. **Leave a handoff.** After meaningful work, update `docs/OldStatus/` handoff docs.
6. **Ask the user** when unsure. A 10-second question beats a 10-minute wrong turn.
7. **Two-failures rule.** If the same approach fails twice, stop and switch strategies.

### checkdate()

Never trust training data for dates. Before writing any date, run:
```powershell
Get-Date -Format "yyyy-MM-dd"
```
The Codex project began ~**2026-03-13**. Any earlier date in project docs is wrong.

---

## Agent Toolkit — codex-agent

**Use this for all file ops.** It's a Codex-compiled .exe in `tools/codex-agent/`.

```
dotnet tools/codex-agent/codex-agent.exe <command> [args]
```

| Command | What it does |
|---------|-------------|
| `peek <file> [start] [end]` | Read file lines with line numbers (use `0 0` for whole file) |
| `stat <file> [file2] ...` | File stats: lines, chars, size category |
| `snap save <file>` | Snapshot before editing. **Required for files >100 lines.** |
| `snap diff <file>` | Compare current file to snapshot |
| `snap restore <file>` | Restore from snapshot if edit goes wrong |
| `status` | Project health check |
| `plan add <task>` | Stash a task (survives context loss) |
| `plan` / `plan show` | Show current task list |
| `plan clear` | Clear task list |
| `check` | Cognitive load estimate — hot-path sizes vs budget |
| `doctor` | Known conditions & diagnostics briefing (run at session start) |
| `log <message>` | Append to persistent session log (decisions, errors, notes) |
| `recall [n]` | Show last N session log entries (default 10) |
| `build` | `dotnet build Codex.sln` — stores full log, prints compact summary |
| `test [filter]` | `dotnet test Codex.sln` — stores full log, prints compact summary |

### Session Start

```powershell
dotnet tools/codex-agent/codex-agent.exe doctor
dotnet tools/codex-agent/codex-agent.exe check
dotnet tools/codex-agent/codex-agent.exe status
```

### Before Editing Any File

```powershell
dotnet tools/codex-agent/codex-agent.exe stat <file>
dotnet tools/codex-agent/codex-agent.exe peek <file> 1 30
dotnet tools/codex-agent/codex-agent.exe snap save <file>
# ... make edit ...
dotnet tools/codex-agent/codex-agent.exe snap diff <file>
```

---

## C# Code Style

### Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Private instance fields | `m_` prefix | `m_diagnostics` |
| Private readonly fields | `m_` prefix + `readonly` | `readonly List<Token> m_tokens` |
| Properties, types | PascalCase | `SourceSpan`, `TokenKind` |
| Locals, parameters | camelCase | `localEnv`, `tokenIndex` |
| Constants | PascalCase | `MaxLineLength` |

### Key Rules

- `sealed record` for immutable ref types. `readonly record struct` for small value types.
- Use `new()` when the target type is on the left side.
- **Omit default modifiers.** Don't write `private` on members or `internal` on top-level types.
- **No XML doc comments** (`///`). Code should be self-documenting.
- **No `var`** when the type is not obvious from the RHS.
- **No unused fields/variables/parameters.** `TreatWarningsAsErrors` catches these.
- Prefer `Map<K,V>` (in `Codex.Core`) over `ImmutableDictionary`.

### Implicit Usings

These are implicit (don't add them): `System`, `System.Collections.Generic`, `System.Linq`,
`System.IO`, `System.Net.Http`, `System.Threading`, `System.Threading.Tasks`.

`System.Collections.Immutable` is **NOT** implicit — add only when needed.

### Formatting

- 4 spaces (no tabs). UTF-8. Max 120 chars/line. End files with single newline.

---

## Codex (.codex) Code Style

- Boolean literals: `True` / `False` (capital T/F).
- Function application is left-associative: `f x y` means `(f x) y`.
- Pattern matching uses `when`/`if`, not `match`/`case`.
- All functions are curried by default.

---

## File Editing Rules

### Small Files (< 100 lines)
`edit_file` is fine. Verify result by reading back.

### Medium Files (100–300 lines)
`edit_file` with generous context. Always `snap save` first. `snap diff` after.

### Large Files (> 300 lines)
`edit_file` can silently corrupt large files. Required workflow:

1. `snap save` the file.
2. Write complete new file to `<filename>.new` using `create_file`.
3. Swap in terminal: `Copy-Item <filename>.new <filename> -Force`
4. `snap diff` to confirm only intended changes. Check line count with `stat`.
5. If build fails: `snap restore`, inspect, retry.
6. Clean up `.new` files.

**Alternative: Partial class.** For >300-line files needing new methods, create a second
file (e.g., `Foo.Bar.cs`) with `partial class`. Merge when stable.

### Never Do

- Never print a full file as a code block for the user to paste.
- Never use terminal redirects (`>`, `Set-Content`) when file tools are available.
- Never retry a failed edit approach — switch strategies.

---

## Build and Verify

Every task touching source code must end with a green build.

```powershell
dotnet tools/codex-agent/codex-agent.exe build
dotnet tools/codex-agent/codex-agent.exe test
```

`TreatWarningsAsErrors` is on — unused fields, variables, parameters, and redundant
usings are all build failures.

### Quick Checklist

- [ ] Build passes (zero warnings)
- [ ] Tests pass
- [ ] `m_` prefix on private fields
- [ ] No `var` where type is non-obvious
- [ ] No XML doc comments
- [ ] No dead code
- [ ] Temp files cleaned up

---

## Scope of Authority

### Freely Modifiable
`src/`, `tests/`, `tools/`, `samples/`, `editors/`, `generated-output/`, `Codex.Codex/`

### Modifiable With Care
`README.md`, `CONTRIBUTING.md`, `.github/copilot-instructions.md`, `.gitignore`, `Codex.sln`

### Do Not Modify Without Permission
`Directory.Build.props`, `docs/Vision/NewRepository.txt`, `docs/Vision/IntelligenceLayer.txt`

---

## Project Structure (quick ref)

Dependencies flow one way: `Core → Syntax → Ast → Semantics → Types → IR → Emit → Emit.* → Cli`

`Codex.Core` has zero project deps (root). `Codex.Cli` references everything (composition root).

Tests: `Codex.Core.Tests`, `Codex.Syntax.Tests`, `Codex.Ast.Tests`, `Codex.Semantics.Tests`,
`Codex.Types.Tests`, `Codex.Repository.Tests`, `Codex.Lsp.Tests`

Self-hosted source: 26 `.codex` files in `Codex.Codex/`. Bootstrap runner: `tools/Codex.Bootstrap/`.

---

## Git Workflow

Branch naming: `windows/<topic>`, `linux/<topic>`, `staging/<topic>`.
No agent merges its own work to `master` without review (by the other agent or user).
Use Conventional Commits: `feat:`, `fix:`, `docs:`, `test:`, `chore:`, `refactor:`.

---

## Terminal Discipline

- **Never paste multi-line PowerShell.** Write to `.ps1` file, run with `pwsh -File`, delete after.
- Terminal is for **read-only queries and builds**. Use `edit_file`/`create_file` for mutations.
- If a command hangs, kill it and switch to a file-based approach.

---

## Handoffs

After meaningful sessions, create/update `docs/OldStatus/ITERATION-N-HANDOFF.md`.
Log architecture decisions in `docs/OldStatus/DECISIONS.md` (append-only).
Use `checkdate()` for all dates. See existing handoffs for format.
