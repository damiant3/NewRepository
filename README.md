# Codex

Codex is a programming language, a repository protocol, and a unified development environment — built in C# (.NET 8) as the bootstrap implementation.

The language reads like literate prose, is statically typed with bidirectional type inference, and compiles via an intermediate representation to C# (and eventually other backends). The repository model is a content-addressed, append-only fact store. The environment is a unified reader/writer/verifier/explorer that presents code as formatted chapters.

> **Status**: Milestone 3 complete. The full compile-and-run pipeline is operational: `.codex` source → tokens → CST → AST → type check → IR → C# → executable output.

---

## Quick Start

**Prerequisites**: .NET 8 SDK

```sh
# Build the solution
dotnet build Codex.sln

# Run all tests
dotnet test Codex.sln

# Run the CLI
dotnet run --project tools/Codex.Cli -- run samples/hello.codex
dotnet run --project tools/Codex.Cli -- run samples/factorial.codex
dotnet run --project tools/Codex.Cli -- run samples/fibonacci.codex
dotnet run --project tools/Codex.Cli -- run samples/greeting.codex
```

---

## CLI Commands

```
codex run   <file.codex>   Compile and execute a Codex program
codex build <file.codex>   Compile to C# and write the output file
codex check <file.codex>   Type-check without compiling
codex parse <file.codex>   Print tokens, CST, and AST (debug)
codex version              Print version
```

---

## Compilation Pipeline

```
Source (.codex)
    → Lexer         (Codex.Syntax)       token stream
    → Parser        (Codex.Syntax)       CST (DocumentNode)
    → Desugarer     (Codex.Ast)          AST (Module)
    → NameResolver  (Codex.Semantics)    ResolvedModule
    → TypeChecker   (Codex.Types)        type map
    → Lowering      (Codex.IR)           IRModule (typed IR)
    → CSharpEmitter (Codex.Emit.CSharp)  C# source text
    → dotnet                             executable output
```

---

## Project Structure

```
Codex.sln
├── src/
│   ├── Codex.Core          Core primitives: SourceText, Span, Diagnostic, DiagnosticBag, Names
│   ├── Codex.Syntax        Lexer, Parser, Token, TokenKind, CST nodes
│   ├── Codex.Ast           Desugarer, AST node types (Module, Definition, Expr)
│   ├── Codex.Semantics     Name resolution, scope analysis
│   ├── Codex.Types         Bidirectional type checker, CodexType hierarchy, unification
│   ├── Codex.IR            IRModule, IRExpr hierarchy, Lowering (AST → IR)
│   ├── Codex.Emit          ICodeEmitter interface
│   ├── Codex.Emit.CSharp   CSharpEmitter — IR → C# source
│   ├── Codex.Proofs        (Planned) Proof terms and verification
│   ├── Codex.Repository    (Planned) Content-addressed fact store
│   └── Codex.Narration     (Planned) Prose mode rendering
├── tests/
│   ├── Codex.Core.Tests        (16 tests)
│   ├── Codex.Syntax.Tests      (29 tests)
│   ├── Codex.Ast.Tests         (11 tests)
│   ├── Codex.Semantics.Tests   (10 tests)
│   └── Codex.Types.Tests       (22 tests)
├── tools/
│   └── Codex.Cli           `codex` command-line tool
├── samples/
│   ├── hello.codex         square/double functions, main = square 5  → 25
│   ├── factorial.codex     recursive factorial,    main = factorial 10 → 3628800
│   ├── fibonacci.codex     recursive fibonacci,    main = fib 20 → 6765
│   ├── greeting.codex      text concatenation,     main = greeting "World" → Hello, World!
│   └── arithmetic.codex    max/abs/clamp (type-check demo)
└── docs/
    ├── 00-OVERVIEW.md          Project overview and three pillars
    ├── 01-ARCHITECTURE.md      System architecture and dependency graph
    ├── 02-LANGUAGE-DESIGN.md   Formal language specification plan
    ├── 03-TYPE-SYSTEM.md       Type system — dependent types, linear types, effects
    ├── 04-COMPILER-PIPELINE.md Compiler pipeline details
    ├── 05-REPOSITORY-MODEL.md  Content-addressed store design
    ├── 06-ENVIRONMENT.md       Unified IDE design
    ├── 07-TRANSPILATION.md     IR and target backends
    ├── 08-MILESTONES.md        Phased delivery plan
    ├── 09-RISKS.md             Technical risks and mitigations
    ├── 10-PRINCIPLES.md        Engineering principles
    └── ITERATION-3-HANDOFF.md  Latest iteration handoff summary
```

---

## Sample Programs

```codex
-- hello.codex
square : Integer -> Integer
square (x) = x * x

main : Integer
main = square 5
```

```codex
-- greeting.codex
greeting : Text -> Text
greeting (name) = "Hello, " ++ name ++ "!"

main : Text
main = greeting "World"
```

---

## Milestone Status

| Milestone | Status |
|-----------|--------|
| M0: Foundation | ✅ Complete |
| M1: Hello Notation | ✅ Complete |
| M2: Type Checking | ✅ Complete |
| M3: Execution via C# | ✅ Complete |
| M4: Prose Integration | 🔲 Next |

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards, naming conventions, testing requirements, and agent instructions.

See [docs/10-PRINCIPLES.md](docs/10-PRINCIPLES.md) for the engineering principles that govern all design decisions.
