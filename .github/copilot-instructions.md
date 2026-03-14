# Copilot Instructions for Codex

## What This Repository Is

Codex is a bootstrapped programming language compiler written in C# (.NET 8). The solution is `Codex.sln`.

The compilation pipeline is:
```
Source (.codex) → Lexer → Parser → Desugarer → NameResolver → TypeChecker → Lowering → CSharpEmitter → dotnet
```

Projects (in dependency order): `Codex.Core` → `Codex.Syntax` → `Codex.Ast` → `Codex.Semantics` → `Codex.Types` → `Codex.IR` → `Codex.Emit` → `Codex.Emit.CSharp` → `Codex.Cli`

Test projects: `Codex.Core.Tests`, `Codex.Syntax.Tests`, `Codex.Ast.Tests`, `Codex.Semantics.Tests`, `Codex.Types.Tests`

Design docs live in `docs/`. `00-OVERVIEW.md` through `10-PRINCIPLES.md` are the north-star specification — do not modify them unless explicitly asked.

---

## Non-Negotiable Code Rules

- **Private instance fields MUST use the `m_` prefix** — e.g. `m_diagnostics`, `m_localEnv`, `m_tokens`. `TreatWarningsAsErrors` is `true`; an unused field is a build failure.
- **No XML doc comments.** Do not add `///` comments. Code should be self-documenting. Only add a comment if the agent genuinely needs it to avoid re-discovering a non-obvious decision.
- **No `var` when the type is not obvious** from the right-hand side.
- **Minimize type information at callsites.** Use `new()` instead of `new TypeName()` when the target type is already specified by the declaration (field, variable, parameter, return type). Example: `Map<string, CodexType> m_map = Map<string, CodexType>.s_empty;` not `... = new Map<string, CodexType>(...)`.
- **Usings: trial-and-error, compile-and-test.** Do not add `using` directives speculatively. `System`, `System.Collections.Generic`, `System.Linq`, `System.IO`, `System.Net.Http`, `System.Threading`, `System.Threading.Tasks` are implicit. `System.Collections.Immutable` is NOT implicit — add it only when the file uses `ImmutableArray`, `ImmutableDictionary`, etc. When you add a using, check if there are now-redundant fully-qualified type names in the same file and shorten them.
- **Prefer null-safe abstractions over `TryGetValue`.** Use `Map<K,V>` (in `Codex.Core`) which returns `null` on missing keys instead of `ImmutableDictionary` + `TryGetValue`. If a .NET abstraction has bad null behavior (throws on missing key, requires `out` patterns), clone it null-safe in `Codex.Core` and use that.
- **Omit default accessibility modifiers.** Do not write `private` on class members or `internal` on top-level types — those are the C# defaults. Only write an accessibility modifier when it differs from the default (e.g., `public`, `protected`, `internal` on a member).
- 4 spaces indentation, UTF-8, max 120 characters per line.
- `sealed record` for immutable reference types; `readonly record struct` for small value types.

---

## Terminal Discipline

- **Never run multi-line PowerShell scripts directly in the terminal.** Write a `.ps1` script file, then invoke it with `pwsh -File <path>`. Multi-line scripts cause the terminal to hang waiting for input the agent cannot provide.
- **Never use `Write-Output`, `Write-Host`, or bare expressions for terminal feedback.** These are unreliable in the agent terminal. Write results to a temp file and read it back with `get_file`, or use `edit_file` / `create_file` directly.
- **If a terminal command takes more than a few seconds, assume it is hung.** Switch to a file-based approach.
- **Prefer `edit_file` and `create_file` over terminal commands for all file mutations.**
- **Terminal is for read-only queries and build invocations only:** `dotnet build`, `dotnet test`, `Select-String`, `Get-ChildItem`, and similar one-liners.

---

## File Editing Rules

- **Always read a file before editing it** unless you just created it.
- **Use `edit_file` with enough surrounding context** (unique lines above and below the change) so the tool can locate the edit site unambiguously. If an edit fails, re-read the file and provide more context lines.
- **Never print out a full file as a code block and ask the user to paste it.** Use `edit_file` or `create_file`.

---

## When You Get Stuck

- **If a tool call fails or produces no output, do not retry the same approach.** Switch strategies: use a different tool, write a script file, or ask the user.
- **If you are about to attempt something you've already failed at, stop and reconsider.** Two failures on the same approach means the approach is wrong.
- **The user is available mid-task.** If you need a design decision, ask.

---

## Scope of Authority

- Modify files in `src/`, `tests/`, `tools/`, `samples/`, and the three root docs (`README.md`, `CONTRIBUTING.md`, `.github/copilot-instructions.md`).
- **Do not modify** `docs/00-OVERVIEW.md` through `docs/10-PRINCIPLES.md` unless explicitly asked — they are the architecture specification.
- **Do not modify** `Directory.Build.props` without explicit instruction — it governs the whole solution build.
- Each project may have its own style rules in `CONTRIBUTING.md`. Read and follow them.

---

## Build and Verify

Before concluding any task that touches source:

```sh
dotnet build Codex.sln    # must produce zero warnings (warnings are errors)
dotnet test Codex.sln     # all tests must pass
```

---

## Adding to the Compiler Pipeline

| What you're adding | Where it goes |
|--------------------|---------------|
| New CST node | `Codex.Syntax/SyntaxNodes.cs` |
| New AST node or desugaring | `Codex.Ast/` |
| New type | `Codex.Types/` |
| New IR node | `Codex.IR/IRModule.cs` + lowering in `Codex.IR/Lowering.cs` |
| New C# emission rule | `Codex.Emit.CSharp/CSharpEmitter.cs` |
| New CLI command | `tools/Codex.Cli/Program.cs` |
| New backend | New `src/Codex.Emit.<Target>/` project implementing `ICodeEmitter` |
