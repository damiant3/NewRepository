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
| IL | `Codex.Emit.IL` | Limited (no generics/TCO) |
| Babbage | `Codex.Emit.Babbage` | Analytical Engine, intentionally limited |

All mainstream backends support: records, sum types, pattern matching, recursion,
effects, and tail call optimization.

---

## Self-Hosted Pipeline

The compiler is also written in Codex itself (21 `.codex` files in `Codex.Codex/`).

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
