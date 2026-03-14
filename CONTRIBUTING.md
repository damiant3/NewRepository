# Contributing Guidelines for Codex

This file documents the coding standards, development workflow, testing requirements, and acceptance criteria that all contributors (including automated agents) must follow when working on the Codex compiler and tooling. These rules are strict and intended to preserve the style, architecture, and quality of the codebase across iterations.

---

## Purpose

Codex is a bootstrapped programming language implementation in C# (.NET 8). The goals are correctness, reproducibility, testable invariants, and deterministic builds so that future automated agents can safely extend the compiler and toolchain.

---

## General Principles

1. **Ship working software at every milestone.** A program goes in, a result comes out. If a milestone doesn't end with a demo, the milestone is wrong.
2. **Correctness over performance.** The bootstrap compiler does not need to be fast; it needs to be right. Optimization comes after correctness is proven by tests.
3. **Immutability by default.** AST nodes, IR nodes, types, and facts are immutable. Builders are mutable during construction, then frozen.
4. **No premature abstraction.** Do not create an interface until you have two implementations. Do not create a base class until three subclasses exist.
5. **One thing at a time.** Each file does one thing. Each class does one thing. Each method does one thing. Each commit does one thing.

See [docs/10-PRINCIPLES.md](docs/10-PRINCIPLES.md) for the full set of governing principles.

---

## Branching and Commits

- Main branch: `master`. All changes should be committed with clear messages.
- Use Conventional Commits style: `feat:`, `fix:`, `chore:`, `docs:`, `test:`, `refactor:`, `perf:`.
- Single logical change per commit.

---

## Code Style

These rules must be enforced by all contributors and automated agents when generating or modifying code:

- Use 4 spaces for indentation.
- End files with a single newline.
- Maximum line length: 120 characters.
- Encoding: UTF-8.
- **Private instance fields MUST use the `m_` prefix** (e.g. `m_root`, `m_diagnostics`, `m_localEnv`).
- Private `readonly` fields: `m_` prefix and `readonly` where appropriate.
- Property and type names: **PascalCase**. Local variables and parameters: **camelCase**.
- Constants: PascalCase.
- Avoid `var` when the type is not obvious from the right-hand side.
- Use explicit accessibility modifiers on all types and members.
- Prefer `readonly record struct` for small value types; `sealed record` for immutable reference types.
- `TreatWarningsAsErrors` is `true` in `Directory.Build.props`. Do not leave unused variables, fields, or parameters.

---

## Naming Conventions by Layer

| Layer | Key Types | Naming Pattern |
|-------|-----------|----------------|
| `Codex.Core` | `SourceText`, `Span`, `Diagnostic`, `DiagnosticBag` | Core prefix-free |
| `Codex.Syntax` | `Token`, `TokenKind`, CST nodes (`DocumentNode`, `DefinitionNode`) | `*Node` for CST |
| `Codex.Ast` | `Module`, `Definition`, `Expr`, `Desugarer` | No prefix |
| `Codex.Semantics` | `NameResolver`, `ResolvedModule` | `Resolved*` for outputs |
| `Codex.Types` | `TypeChecker`, `CodexType` and subclasses | `Codex*` for types |
| `Codex.IR` | `IRModule`, `IRDefinition`, `IRExpr` subclasses, `Lowering` | `IR*` prefix |
| `Codex.Emit` | `ICodeEmitter` | `I*` for interfaces |
| `Codex.Emit.CSharp` | `CSharpEmitter` | Target name prefix |
| `Codex.Cli` | `Program` | Standard CLI layout |

---

## XML Documentation

Every public type and every public method must have XML doc comments. Example:

```csharp
/// <summary>
/// Lowers an AST <see cref="Module"/> to a typed <see cref="IRModule"/>.
/// All expressions carry resolved <see cref="CodexType"/>s after lowering.
/// </summary>
public sealed class Lowering { ... }
```

---

## Testing Requirements

Every change must include tests. Tests must be deterministic, fast, and run headlessly.

Minimum requirements per compiler phase change:

1. **Positive tests** — programs that should succeed; verify the output is correct.
2. **Negative tests** — programs that should fail; verify the diagnostic code and message are correct.
3. **Round-trip tests** — parse → print → parse produces the same result (for syntax changes).
4. **End-to-end tests** — for pipeline changes, at minimum run `codex check` against a sample file.

Tests live in the corresponding test project under `tests/`:

| Source project | Test project |
|----------------|-------------|
| `Codex.Core` | `Codex.Core.Tests` |
| `Codex.Syntax` | `Codex.Syntax.Tests` |
| `Codex.Ast` | `Codex.Ast.Tests` |
| `Codex.Semantics` | `Codex.Semantics.Tests` |
| `Codex.Types` | `Codex.Types.Tests` |

Use xUnit. Match the style of existing test files in each project.

---

## Build and Test Commands

```sh
dotnet build Codex.sln          # Build the full solution
dotnet test Codex.sln           # Run all tests
dotnet run --project tools/Codex.Cli -- run samples/hello.codex
dotnet run --project tools/Codex.Cli -- check samples/arithmetic.codex
```

---

## Architecture Rules

- **Dependency direction is strictly one-way** (upstream → downstream):
  `Core` → `Syntax` → `Ast` → `Semantics` → `Types` → `IR` → `Emit` → `Emit.CSharp` → `Cli`
- No downstream project may reference an upstream project in the reverse direction.
- `Codex.Core` has no project dependencies — it is the root of the graph.
- `Codex.Cli` references everything; it is the composition root.

---

## Adding a New Backend

To add a new emission target (e.g., Rust, Python):

1. Create `src/Codex.Emit.<Target>/` implementing `ICodeEmitter` from `Codex.Emit`.
2. Register it in `tools/Codex.Cli/Program.cs` alongside `CSharpEmitter`.
3. Add tests for representative IR constructs.

---

## How AI Agents Should Operate

- **Always read a file before editing it.**
- **Run `dotnet build Codex.sln` and `dotnet test Codex.sln`** before concluding a task.
- **Produce minimal diffs** — do not reformat unrelated code, rename symbols, or restructure files unless that is the explicit task.
- **Follow the `m_` field prefix rule** without exception — `TreatWarningsAsErrors` is on and an unused field is a build failure.
- **Do not modify `docs/` planning documents** (`00-OVERVIEW.md` through `10-PRINCIPLES.md`) unless explicitly asked. They are the north-star specification.
- **When adding a new compiler phase**, update `docs/ITERATION-*-HANDOFF.md` with a brief summary of what was done.
- See [`.github/copilot-instructions.md`](.github/copilot-instructions.md) for terminal and file-editing discipline rules.

---

## CI Expectations

- Build must pass: `dotnet build Codex.sln` with no warnings (warnings are errors).
- All tests must pass: `dotnet test Codex.sln`.
- No new unused fields, variables, or parameters.
- XML docs on all new public surface.

---

## Quick Checklist for Changes

- [ ] `dotnet build Codex.sln` passes (zero warnings).
- [ ] `dotnet test Codex.sln` passes (all existing tests + new tests).
- [ ] New public types and methods have XML doc comments.
- [ ] Private fields use the `m_` prefix.
- [ ] No `var` where the type is non-obvious.
- [ ] Commit message follows Conventional Commits (`feat:`, `fix:`, etc.).
