# Codex

A programming language where programs are literate documents.

Codex is statically typed, purely functional, and self-hosting. Source files are
organized into Chapters and Sections — prose explains intent, indented notation
is executable. The compiler is written in Codex and compiles itself.

```codex
Chapter: Greeting

  A module that greets people by name.

Section: Functions

    greet : Text -> Text
    greet (name) = "Hello, " ++ name ++ "!"

    main : Text
    main = greet "World"
```

> **Status**: Self-hosting bootstrap complete. The Codex compiler, written in Codex,
> compiles its own source through Stage 0 (C#) to produce a Stage 1 binary that
> compiles the same source — 264/264 type definitions, 0 missing functions.

---

## Quick Start

**Prerequisites**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```sh
# Build
dotnet build Codex.sln

# Run tests (246 tests)
dotnet test Codex.sln

# Compile and run a program
dotnet run --project tools/Codex.Cli -- run samples/hello.codex

# Compile to C#
dotnet run --project tools/Codex.Cli -- build samples/hello.codex

# Bootstrap: compile the compiler with itself
dotnet run --project tools/Codex.Cli -- build codex-src
dotnet run --project tools/Codex.Bootstrap -- build codex-src
```

---

## Language Features

```codex
-- Sum types
Shape =
  | Circle (Number)
  | Rectangle (Number) (Number)

-- Record types
Person = record {
  name : Text,
  age : Integer
}

-- Pattern matching
area : Shape -> Number
area (s) = when s
  if Circle (r) -> 3.14 * r * r
  if Rectangle (w) (h) -> w * h

-- Higher-order functions
apply-twice : (a -> a) -> a -> a
apply-twice (f) (x) = f (f x)

-- Let bindings
hypotenuse : Number -> Number -> Number
hypotenuse (a) (b) =
  let a2 = a * a
  in let b2 = b * b
    in a2 + b2

-- Field access
greet : Person -> Text
greet (p) = "Hello, " ++ p.name ++ "!"
```

---

## Compilation Pipeline

```
Source (.codex)
    → Lexer         (Codex.Syntax)       token stream
    → Parser        (Codex.Syntax)       concrete syntax tree
    → Desugarer     (Codex.Ast)          abstract syntax tree
    → NameResolver  (Codex.Semantics)    resolved names
    → TypeChecker   (Codex.Types)        bidirectional type inference
    → Lowering      (Codex.IR)           typed intermediate representation
    → CSharpEmitter (Codex.Emit.CSharp)  C# source text
    → dotnet                             executable
```

The entire pipeline (Lexer through CSharpEmitter) also exists in Codex in `codex-src/`
and compiles itself — see [Bootstrap](#bootstrap) below.

---

## CLI

```
codex run   <file.codex>         Compile and execute
codex build <file.codex|dir>     Compile to C#
codex check <file.codex>         Type-check only
codex parse <file.codex>         Print tokens / CST / AST
codex version                    Print version
```

---

## Project Structure

```
Codex.sln
├── src/
│   ├── Codex.Core             SourceText, Span, Diagnostic, DiagnosticBag
│   ├── Codex.Syntax           Lexer, Parser, ProseParser, Token, CST nodes
│   ├── Codex.Ast              Desugarer, AST nodes (Module, Definition, Expr)
│   ├── Codex.Semantics        Name resolution, scope analysis
│   ├── Codex.Types            Bidirectional type checker, unification
│   ├── Codex.IR               IR nodes, Lowering (AST → IR)
│   ├── Codex.Emit             ICodeEmitter interface
│   ├── Codex.Emit.CSharp      C# backend
│   ├── Codex.Lsp              Language Server Protocol implementation
│   ├── Codex.Repository       Content-addressed fact store
│   ├── Codex.Narration        Prose rendering
│   └── Codex.Proofs           Proof terms and verification
├── tests/
│   ├── Codex.Core.Tests       (16 tests)
│   ├── Codex.Syntax.Tests     (63 tests)
│   ├── Codex.Ast.Tests        (11 tests)
│   ├── Codex.Semantics.Tests  (10 tests)
│   ├── Codex.Types.Tests      (109 tests)
│   ├── Codex.Repository.Tests (23 tests)
│   └── Codex.Lsp.Tests        (14 tests)
├── tools/
│   ├── Codex.Cli              Command-line compiler
│   └── Codex.Bootstrap        Stage 1 bootstrap harness
├── codex-src/                 The compiler written in Codex
│   ├── Core/                  Diagnostics, Name, SourceText, Collections
│   ├── Syntax/                TokenKind, Lexer, Parser, SyntaxNodes
│   ├── Ast/                   AstNodes, Desugarer
│   ├── IR/                    IRModule, Lowering
│   └── Emit/                  CSharpEmitter
├── editors/
│   └── vscode/                VS Code extension (syntax highlighting + LSP)
├── samples/                   Example programs
└── docs/                      Design documents and reflections
```

---

## Bootstrap

Codex is self-hosting. The compiler written in Codex (`codex-src/`, ~2,500 lines
across 14 files) compiles itself through a two-stage bootstrap:

| Stage | Description | Compiled By |
|-------|-------------|-------------|
| **Stage 0** | C# implementation in `src/` | `dotnet build` |
| **Stage 1** | Codex source compiled to C# | Stage 0 |
| **Stage 2** | Same source compiled again | Stage 1 |

**Current status** (March 2026):

| Metric | Stage 0 | Stage 1 | Gap |
|--------|---------|---------|-----|
| Type definitions | 264 | 264 | **0** |
| Function definitions | 222 | 220 | **0** |
| Missing functions | — | 0 | **0** |

See [docs/M13-BOOTSTRAP-PLAN.md](docs/M13-BOOTSTRAP-PLAN.md) for details and
[docs/Reflections2.md](docs/Reflections2.md) for the story of closing the gap.

---

## Samples

| File | Description | Output |
|------|-------------|--------|
| `hello.codex` | `square 5` | `25` |
| `factorial.codex` | `factorial 10` | `3628800` |
| `fibonacci.codex` | `fib 20` | `6765` |
| `greeting.codex` | `greeting "World"` | `Hello, World!` |
| `shapes.codex` | Sum types + pattern matching | `78.5` |
| `person.codex` | Record types + field access | `Hello, Alice!` |
| `prose-greeting.codex` | Literate prose format | `Hello, World!` |
| `safe-divide.codex` | Option type | `Just 5` |
| `effects-demo.codex` | Effect types | — |

---

## Editor Support

**VS Code**: Install from `editors/vscode/`:

```sh
cd editors/vscode
npm install
# Then in VS Code: F5 to launch Extension Development Host
# Or: npx vsce package && code --install-extension codex-lang-0.1.0.vsix
```

Features: syntax highlighting in VS Code (keywords, types, operators, prose structure,
strings, numbers), bracket matching, auto-indentation, and LSP integration
for diagnostics and hover.

---

## Milestone History

| Milestone | Status |
|-----------|--------|
| M0: Foundation | ✅ |
| M1: Notation (lexer + parser) | ✅ |
| M2: Type Checking | ✅ |
| M3: Execution via C# | ✅ |
| M4–M12: Type system, IR, effects, proofs, LSP | ✅ |
| M13: Self-hosting bootstrap | ✅ |

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards and agent instructions.

See [docs/10-PRINCIPLES.md](docs/10-PRINCIPLES.md) for engineering principles.
