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

> **Status (March 15 2026):** Self-hosting achieved. The Codex compiler (3,386 lines
> of Codex across 21 source files) compiles itself through a two-stage bootstrap.
> Stage 1 output compiles to valid C#, serves as its own Stage 0, and produces
> functionally identical Stage 2 output. 700 tests pass. See [Opus.md](Opus.md)
> for the story.

---

## Quick Start

**Prerequisites**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```sh
# Build everything
dotnet build Codex.sln

# Run tests (700 tests across 7 projects)
dotnet test Codex.sln

# Compile and run a program
dotnet run --project tools/Codex.Cli -- run samples/hello.codex

# Compile to C#
dotnet run --project tools/Codex.Cli -- build samples/hello.codex

# Bootstrap: compile the compiler with itself
dotnet run --project tools/Codex.Bootstrap
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

-- Polymorphism
map-list : (a -> b) -> List a -> List b
map-list (f) (xs) = map-list-loop f xs 0 (list-length xs) []

-- Let bindings
hypotenuse : Number -> Number -> Number
hypotenuse (a) (b) =
  let a2 = a * a
  in let b2 = b * b
    in a2 + b2

-- Field access
greet : Person -> Text
greet (p) = "Hello, " ++ p.name ++ "!"

-- Do notation (effects)
main : [Console] Nothing
main = do
  print-line (greet Person { name = "World", age = 0 })
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
    → Emitter       (Codex.Emit.*)       target source text
    → toolchain                          executable
```

The entire pipeline also exists in Codex (`Codex.Codex/`, 21 files, 3,386 lines)
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
│   ├── Codex.Core               SourceText, Span, Diagnostic, DiagnosticBag
│   ├── Codex.Syntax             Lexer, Parser, ProseParser, Token, CST nodes
│   ├── Codex.Ast                Desugarer, AST nodes (Module, Definition, Expr)
│   ├── Codex.Semantics          Name resolution, scope analysis
│   ├── Codex.Types              Bidirectional type checker, unification
│   ├── Codex.IR                 IR nodes, Lowering (AST → IR)
│   ├── Codex.Emit               ICodeEmitter interface
│   ├── Codex.Emit.CSharp        C# backend
│   ├── Codex.Emit.Python        Python backend
│   ├── Codex.Emit.JavaScript    JavaScript backend
│   ├── Codex.Emit.Java          Java backend
│   ├── Codex.Emit.Rust          Rust backend
│   ├── Codex.Emit.Go            Go backend
│   ├── Codex.Emit.Cpp           C++ backend
│   ├── Codex.Emit.IL            .NET IL backend
│   ├── Codex.Emit.Ada           Ada backend
│   ├── Codex.Emit.Fortran       Fortran backend
│   ├── Codex.Emit.Cobol         COBOL backend
│   ├── Codex.Emit.Babbage       Babbage backend
│   ├── Codex.Lsp                Language Server Protocol implementation
│   ├── Codex.Repository         Content-addressed fact store
│   ├── Codex.Narration          Prose rendering
│   └── Codex.Proofs             Proof terms and verification
├── tests/
│   ├── Codex.Core.Tests         (16 tests)
│   ├── Codex.Syntax.Tests       (88 tests)
│   ├── Codex.Ast.Tests          (11 tests)
│   ├── Codex.Semantics.Tests    (15 tests)
│   ├── Codex.Types.Tests        (529 tests)
│   ├── Codex.Repository.Tests   (23 tests)
│   └── Codex.Lsp.Tests          (18 tests)
├── tools/
│   ├── Codex.Cli                Command-line compiler
│   ├── Codex.Bootstrap          Stage 1 bootstrap harness
│   └── Codex.VsExtension        Visual Studio extension
├── Codex.Codex/                 The compiler written in Codex (self-hosting)
│   ├── Core/                    Diagnostics, Name, SourceText, Collections
│   ├── Syntax/                  TokenKind, Lexer, Parser, SyntaxNodes
│   ├── Ast/                     AstNodes, Desugarer
│   ├── Semantics/               NameResolver
│   ├── Types/                   CodexType, TypeChecker, Unifier, TypeEnv
│   ├── IR/                      IRModule, Lowering
│   ├── Emit/                    CSharpEmitter
│   └── main.codex               Entry point
├── editors/
│   └── vscode/                  VS Code extension (syntax highlighting + LSP)
├── samples/                     Example programs
└── docs/                        Design documents (00-OVERVIEW through 10-PRINCIPLES)
```

---

## Bootstrap

Codex is self-hosting. The compiler written in Codex (`Codex.Codex/`, 3,386 lines
across 21 files) compiles itself through a two-stage bootstrap:

| Stage | Description | Compiled By |
|-------|-------------|-------------|
| **Stage 0** | Reference C# implementation in `src/` | `dotnet build` |
| **Stage 1** | Codex source compiled to C# | Stage 0 (via `Codex.Bootstrap`) |
| **Stage 2** | Same source compiled again | Stage 1 (used as Stage 0) |

Stage 1 compiles to valid C#. Stage 2 is functionally identical to Stage 1.
The compiler has reached a fixed point — it can compile itself.

```sh
# Run the bootstrap (Stage 0 → Stage 1)
dotnet run --project tools/Codex.Bootstrap

# Verify: swap Stage 1 in as Stage 0, produce Stage 2
cp Codex.Codex/stage1-output.cs tools/Codex.Bootstrap/CodexLib.g.cs
dotnet run --project tools/Codex.Bootstrap
# Stage 2 output matches Stage 1 functionally
```

The self-hosted compiler covers: lexing (247 lines), parsing (698 lines),
desugaring (117 lines), name resolution (230 lines), bidirectional type checking
with polymorphic instantiation (548 lines), IR lowering (326 lines), and
C# emission (413 lines).

---

## Samples

| File | Description |
|------|-------------|
| `hello.codex` | `square 5` → `25` |
| `factorial.codex` | `factorial 10` → `3628800` |
| `fibonacci.codex` | `fib 20` → `6765` |
| `greeting.codex` | Literate document, `greet "World"` → `Hello, World!` |
| `shapes.codex` | Sum types + pattern matching |
| `person.codex` | Record types + field access |
| `safe-divide.codex` | Option type |
| `effects-demo.codex` | Effect types |
| `tco-stress.codex` | Tail-call optimization stress test |
| `mini-bootstrap.codex` | All-features smoke test for self-hosting |

---

## Editor Support

**VS Code**: Install from `editors/vscode/`:

```sh
cd editors/vscode
npm install
npx vsce package && code --install-extension codex-lang-0.1.0.vsix
```

Features: syntax highlighting, bracket matching, auto-indentation, and LSP
integration for diagnostics and hover.

**Visual Studio**: Extension project at `tools/Codex.VsExtension/`.

---

## Milestone History

| Milestone | Status |
|-----------|--------|
| M0: Foundation | ✅ |
| M1: Notation (lexer + parser) | ✅ |
| M2: Type Checking | ✅ |
| M3: Execution via C# | ✅ |
| M4–M12: Type system, IR, effects, proofs, LSP, backends | ✅ |
| M13: Self-hosting bootstrap | ✅ |

---

## Documentation

Architecture and design docs live in `docs/`:

- [00-OVERVIEW.md](docs/00-OVERVIEW.md) — Project vision
- [01-ARCHITECTURE.md](docs/01-ARCHITECTURE.md) — System architecture
- [02-LANGUAGE-DESIGN.md](docs/02-LANGUAGE-DESIGN.md) — Language design
- [03-TYPE-SYSTEM.md](docs/03-TYPE-SYSTEM.md) — Type system
- [04-COMPILER-PIPELINE.md](docs/04-COMPILER-PIPELINE.md) — Compiler pipeline
- [10-PRINCIPLES.md](docs/10-PRINCIPLES.md) — Engineering principles
- [Opus.md](Opus.md) — The self-hosting story

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards and agent instructions.
