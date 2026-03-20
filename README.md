# Codex

**A programming language that compiles itself, reads like a book, and targets twelve backends.**

Codex is a statically typed, purely functional language where source files are
literate documents — prose explains intent, indented notation is executable.
The compiler is written in Codex. It compiles itself. The C# bootstrap that
brought it to life is locked and archived. The snake ate its own tail.

```codex
Chapter: Greeting

  A module that greets people by name.

Section: Functions

    greet : Text -> Text
    greet (name) = "Hello, " ++ name ++ "!"

    main : Text
    main = greet "World"
```

> **March 19, 2026 — Major Milestone 1: Self-Hosting Achieved.**
> The Codex compiler (4,900 lines of `.codex` across 26 source files) compiles
> itself through a verified two-stage bootstrap. Zero type debt. Zero unresolved
> types. 843 tests pass. 12 backends emit working code. The reference C# compiler
> is locked. All forward development happens in Codex.

---

## Why

Most compilers are written in languages that look nothing like what they compile.
Codex is the language, the compiler, and the document — all the same thing.

- **Literate by design.** Chapters and Sections aren't comments. They're structure.
  The compiler parses prose alongside code.
- **Self-hosting.** The compiler compiles itself. The bootstrap is proven:
  Stage 1 = Stage 2 = fixed point.
- **Twelve backends.** One language, twelve targets. C#, JavaScript, Python, Rust,
  C++, Go, Java, Ada, Fortran, COBOL, .NET IL, and — yes — Babbage's Analytical Engine.
- **Algebraic effects.** Side effects are declared in types and handled explicitly.
  No monads. No surprise mutations.
- **Content-addressed repository.** Code is facts. Facts are immutable, hashed,
  and append-only. No more "merge conflict."

---

## Quick Start

**Prerequisites**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```sh
# Build everything
dotnet build Codex.sln

# Run all 843 tests
dotnet test Codex.sln

# Compile and run a program
dotnet run --project tools/Codex.Cli -- run samples/hello.codex

# Compile to multiple targets
dotnet run --project tools/Codex.Cli -- build samples/hello.codex --targets cs,js,rust

# Build a multi-file project
dotnet run --project tools/Codex.Cli -- build samples/word-freq/

# Bootstrap: compile the compiler with itself
dotnet run --project tools/Codex.Bootstrap
```

---

## Language Features

```codex
-- Sum types (algebraic data types)
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
identity : a -> a
identity (x) = x

-- Let bindings
hypotenuse : Number -> Number -> Number
hypotenuse (a) (b) =
  let a2 = a * a
  in let b2 = b * b
    in a2 + b2

-- Effects and do-notation
main : [Console] Nothing
main = do
  print-line "What is your name?"
  let name = read-line ()
  print-line ("Hello, " ++ name ++ "!")
```

---

## Compilation Pipeline

```
Source (.codex)
    → Lexer         token stream
    → Parser        concrete syntax tree
    → Desugarer     abstract syntax tree
    → NameResolver  resolved names
    → TypeChecker   bidirectional type inference
    → Lowering      typed intermediate representation
    → Emitter       target source code
    → toolchain     executable
```

The entire pipeline exists twice: once in C# (the locked reference implementation)
and once in Codex (the self-hosted compiler, 26 files, ~4,900 lines). The Codex
version is the one that matters now.

---

## Twelve Backends

| Backend | Target | Status |
|---------|--------|--------|
| C# | `--targets cs` | Primary. Full pipeline. |
| JavaScript | `--targets js` | Full |
| Python | `--targets python` | Full |
| Rust | `--targets rust` | Full |
| C++ | `--targets cpp` | Full |
| Go | `--targets go` | Full |
| Java | `--targets java` | Full |
| Ada | `--targets ada` | Full |
| Fortran | `--targets fortran` | Full |
| COBOL | `--targets cobol` | Full |
| .NET IL | `--targets il` | Records, sums, pattern matching, builtins |
| Babbage | `--targets babbage` | Analytical Engine. Intentionally limited. |

All mainstream backends support: records, sum types, pattern matching, recursion,
effects, and tail-call optimization.

---

## Standard Library (Prelude)

11 modules, ~1,200 lines of Codex:

| Module | What it does |
|--------|-------------|
| `Maybe` | Option type — `Just a` or `Nothing` |
| `Result` | Error handling — `Ok a` or `Err e` |
| `Either` | Sum of two types |
| `Pair` | Product type |
| `List` | Functional list operations |
| `Hamt` | Hash Array Mapped Trie — persistent map |
| `Set` | Persistent set |
| `Queue` | Functional queue |
| `StringBuilder` | Efficient string building |
| `CCE` | Character code encoding |
| `TextSearch` | Text search utilities |

---

## Self-Hosting Bootstrap

The compiler compiles itself. Here's how:

```
.codex source ──→ [Stage 0: C# reference compiler] ──→ output.cs (Stage 1)
output.cs      ──→ [dotnet build]                   ──→ Stage 1 binary
Stage 1 binary ──→ [compiles .codex source]          ──→ stage1-output.cs (Stage 2)

Stage 2 ≈ Stage 1 → Fixed point achieved.
```

The self-hosted compiler covers: lexing, parsing, desugaring, name resolution,
bidirectional type checking with polymorphic instantiation, IR lowering, and
C# emission. It includes an import/module system for cross-file compilation.

```sh
# Run the bootstrap
dotnet run --project tools/Codex.Bootstrap

# Verify the fixed point
dotnet run --project tools/Codex.Bootstrap -- build Codex.Codex
```

---

## Project Structure

```
Codex.sln
├── src/                         Reference compiler (C#, locked)
│   ├── Codex.Core               Diagnostics, SourceText, Span, Map<K,V>
│   ├── Codex.Syntax             Lexer, Parser, ProseParser, CST
│   ├── Codex.Ast                Desugarer, AST nodes
│   ├── Codex.Semantics          Name resolution, scope analysis
│   ├── Codex.Types              Bidirectional type checker, unifier
│   ├── Codex.IR                 IR nodes, lowering
│   ├── Codex.Emit.*             12 backend emitters
│   ├── Codex.Lsp                Language Server Protocol
│   ├── Codex.Repository         Content-addressed fact store
│   ├── Codex.Narration          Prose rendering
│   └── Codex.Proofs             Proof terms and verification
├── Codex.Codex/                 Self-hosted compiler (26 .codex files)
├── prelude/                     Standard library (11 modules)
├── tests/                       843 tests across 7 projects
├── tools/
│   ├── Codex.Cli                Command-line interface
│   ├── Codex.Bootstrap          Bootstrap harness
│   └── Codex.VsExtension        Visual Studio extension
├── editors/vscode/              VS Code extension (syntax + LSP)
├── samples/                     24 example programs + 2 multi-file projects
├── generated-output/            Backend output corpus (10 languages)
└── docs/
    ├── 00-OVERVIEW.md           Project overview
    ├── 10-PRINCIPLES.md         Engineering principles
    ├── CurrentPlan.md           Active plan
    ├── MM1/                     Archived bootstrap-era design docs
    ├── Designs/                 Feature design documents
    └── Vision/                  Original vision documents
```

---

## CLI

```
codex run       <file.codex>              Compile and execute
codex build     <file.codex|dir>          Compile (multi-target, incremental)
codex check     <file.codex>              Type-check only
codex parse     <file.codex>              Print tokens / CST / AST
codex add       <package> [--version v]   Add a dependency
codex remove    <package>                 Remove a dependency
codex pack      <dir>                     Package a library
codex packages                            List installed packages
codex version                             Print version
```

---

## Editor Support

**VS Code** — install from `editors/vscode/`:

```sh
cd editors/vscode
npm install
npx vsce package && code --install-extension codex-lang-0.1.0.vsix
```

Syntax highlighting, bracket matching, auto-indentation, and LSP integration
(diagnostics, hover, go-to-definition).

**Visual Studio** — extension project at `tools/Codex.VsExtension/`.

---

## Samples

| File | What it demonstrates |
|------|---------------------|
| `hello.codex` | `square 5` → `25` |
| `factorial.codex` | Recursion: `factorial 10` → `3628800` |
| `fibonacci.codex` | TCO: `fib 20` → `6765` |
| `shapes.codex` | Sum types + pattern matching |
| `person.codex` | Records + field access |
| `effects-demo.codex` | Algebraic effects |
| `expr-calculator.codex` | Quine disproof — the program that proved self-hosting |
| `hamt-test.codex` | Persistent hash maps |
| `is-prime-fancy.codex` | Higher-order functions |
| `state-demo.codex` | Stateful effects |
| `word-freq/` | Multi-file project: word frequency counter |
| `mini-bootstrap.codex` | All-features smoke test |

---

## Milestone History

| # | Milestone | What happened |
|---|-----------|--------------|
| M0 | Foundation | Solution structure, diagnostics, SourceText |
| M1 | Notation | Lexer + parser for literate syntax |
| M2 | Types | Bidirectional type checker with unification |
| M3 | Execution | C# backend — programs run |
| M4–M7 | Type system | Polymorphism, effects, dependent types, proofs |
| M8–M10 | Infrastructure | LSP, repository, 11 more backends |
| M11–M12 | Self-hosting | Compiler written in Codex, bootstrap harness |
| M13 | Fixed point | Stage 2 = Stage 1. Self-hosting proven. |
| **MM1** | **Freedom** | Reference compiler locked. All development in Codex. |

---

## What's Ahead

- **V1 — Views**: first-class consistent selections of facts from the repository.
- **V2 — Narration layer**: prose-aware compilation where English is load-bearing.
- **V3 — Repository federation**: multi-repo sync with trust and identity.

See [docs/CurrentPlan.md](docs/CurrentPlan.md) for the full roadmap.

---

## Documentation

- [00-OVERVIEW.md](docs/00-OVERVIEW.md) — Project overview and status
- [10-PRINCIPLES.md](docs/10-PRINCIPLES.md) — Engineering principles
- [CurrentPlan.md](docs/CurrentPlan.md) — Active plan and direction
- [MM1/](docs/MM1/) — Archived bootstrap-era design documents (01–09, Glossary)
- [Vision/NewRepository.txt](docs/Vision/NewRepository.txt) — The original vision
- [Vision/IntelligenceLayer.txt](docs/Vision/IntelligenceLayer.txt) — The manifesto

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards, project conventions,
and agent collaboration rules. Two AI agents (Copilot and Claude) work on this
repository alongside a human. They review each other's work. Nobody merges
their own code to master without agent or human direction.
PUSH code to a feature branch __before__ running final tests verifying your changes.  This is to prevent accidental loss of local sandbox agent environments due to human error.

---

## License

See repository for license details.
