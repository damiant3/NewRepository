# Copilot Instructions for Codex

## What This Repository Is

Codex is a bootstrapped programming language compiler written in C# (.NET 8) that
compiles itself. The solution is `Codex.sln`. The compiler pipeline:

```
Source (.codex) → Lexer → Parser → Desugarer → NameResolver → TypeChecker → Lowering → Emitter → dotnet/node/rustc/...
```

12 backends (C#, JavaScript, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage).
654+ tests. Self-hosting achieved. Content-addressed fact store with collaboration protocol.

Design docs live in `docs/`. `00-OVERVIEW.md` through `10-PRINCIPLES.md` are the
north-star specification — do not modify them unless explicitly asked.

---

## Agent Rules (Modular)

All agent behavior rules have been decomposed into focused documents in
`.github/agent-rules/`. **Read the ones relevant to your current task.**

| File | Covers |
|------|--------|
| [00-META.md](agent-rules/00-META.md) | **checkdate() rule**, agent identity, session hygiene |
| [01-CODE-STYLE.md](agent-rules/01-CODE-STYLE.md) | C# naming, formatting, type conventions, test style |
| [02-TERMINAL.md](agent-rules/02-TERMINAL.md) | Platform-specific terminal discipline (Windows/Linux) |
| [03-FILE-EDITING.md](agent-rules/03-FILE-EDITING.md) | Edit strategies by file size, backup rules, corruption avoidance |
| [04-SCOPE.md](agent-rules/04-SCOPE.md) | What you can modify freely vs. what needs permission |
| [05-BUILD-VERIFY.md](agent-rules/05-BUILD-VERIFY.md) | Build/test requirements, quick checklist |
| [06-PIPELINE.md](agent-rules/06-PIPELINE.md) | Compiler architecture, where to add things, backend status |
| [07-GIT-WORKFLOW.md](agent-rules/07-GIT-WORKFLOW.md) | **Dual-agent mutual-review workflow**, branch naming, commit format |
| [08-PROJECT-MGMT.md](agent-rules/08-PROJECT-MGMT.md) | Handoffs, decision logging, stuck-recovery, multi-agent coordination |

### Critical Rules (Always Apply)

These rules apply to **every session**, regardless of task:

1. **checkdate()** — Never trust memory for dates. Query the system clock. (See 00-META)
2. **Build before commit** — `dotnet build Codex.sln` + `dotnet test Codex.sln`. (See 05-BUILD-VERIFY)
3. **Read before edit** — Always read a file before modifying it. (See 03-FILE-EDITING)
4. **No spec modifications** — `docs/00-OVERVIEW.md` through `docs/10-PRINCIPLES.md` are off-limits without user request. (See 04-SCOPE)
5. **m_ prefix** — Private instance fields use `m_` prefix, no exceptions. (See 01-CODE-STYLE)
6. **Branch workflow** — Commit to working branches, not master. Review before merge. (See 07-GIT-WORKFLOW)

---

## Non-Negotiable Code Rules (Quick Reference)

- **`m_` prefix** on private instance fields.
- **No XML doc comments** (`///`).
- **No `var`** when the type is not obvious.
- **Omit default accessibility** — don't write `private` on members or `internal` on top-level types.
- **`sealed record`** for immutable reference types; **`readonly record struct`** for small value types.
- **4 spaces**, UTF-8, max 120 chars/line.
- Use **`Map<K,V>`** from `Codex.Core` instead of `ImmutableDictionary` + `TryGetValue`.
- Use **`new()`** when target type is already declared on the left side.

---

## Build and Verify (Quick Reference)

```sh
dotnet build Codex.sln    # zero warnings (warnings are errors)
dotnet test Codex.sln     # all tests pass
```

---

## Adding to the Compiler Pipeline (Quick Reference)

| What | Where |
|------|-------|
| CST node | `src/Codex.Syntax/SyntaxNodes.cs` |
| AST node | `src/Codex.Ast/` |
| Type | `src/Codex.Types/CodexType.cs` |
| IR node | `src/Codex.IR/IRModule.cs` |
| Lowering | `src/Codex.IR/Lowering.cs` |
| C# emission | `src/Codex.Emit.CSharp/CSharpEmitter.cs` |
| CLI command | `tools/Codex.Cli/Program.cs` |
| New backend | `src/Codex.Emit.<Target>/` implementing `ICodeEmitter` |
