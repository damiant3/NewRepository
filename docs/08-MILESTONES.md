# 08 — Milestones

## Philosophy

We build in vertical slices, not horizontal layers. Each milestone produces a working system that can do something real, even if that something is small. No milestone is "implement the type system" — instead it's "compile and run a program that uses type X."

Every milestone ends with a demo: a Codex program that exercises the new capability, compiled and executed.

---

## Milestone 0: Foundation
**Goal**: Project structure, build system, core primitives. Nothing compiles yet, but everything builds.

### Deliverables
- [ ] Solution restructured into multi-project layout (see Architecture doc)
- [ ] `Codex.Core`: `ContentHash`, `Name`, `Span`, `SourceLocation`, `Diagnostic`, `DiagnosticBag`
- [ ] `Codex.Syntax`: Token types, `TokenKind` enum, `Span` on tokens
- [ ] `Codex.Ast`: AST node types (empty implementations, just the shape)
- [ ] `Codex.Core.Tests`: Tests for content hashing, name handling
- [ ] Build passes. All tests pass.
- [ ] `docs/` directory with all planning documents (this milestone is partially complete already)

### Demo
`dotnet build` succeeds. `dotnet test` passes. The project structure matches the architecture doc.

### Estimated Effort
Small. This is scaffolding.

---

## Milestone 1: Hello Notation
**Goal**: Lex and parse a minimal Codex program (notation only, no prose). Produce a CST.

### Deliverables
- [ ] Lexer: tokenizes identifiers, operators, literals, keywords, indentation
- [ ] Parser: parses simple expressions, let bindings, function definitions, type annotations
- [ ] CST: concrete syntax tree with full trivia
- [ ] AST: desugared abstract syntax tree
- [ ] Pretty printer: CST → formatted source text (round-trip test)
- [ ] `Codex.Syntax.Tests`: lexer and parser tests for each construct
- [ ] `Codex.Ast.Tests`: desugaring tests

### Grammar Subset
```
definition  = name ":" type "\n" name params "=" expr
type        = name | type "→" type | "(" type ")"
expr        = literal | name | expr expr | "let" bindings "in" expr 
            | "if" expr "then" expr "else" expr
literal     = integer | text | boolean
params      = name*
bindings    = name "=" expr ("," name "=" expr)*
```

### Demo Program
```
square : Integer → Integer
square (x) = x * x

main : Integer
main = square 5
```

Parse it. Print the CST. Print the AST. No type checking, no execution.

### Estimated Effort
Medium. The lexer with indentation tracking is the hardest part.

---

## Milestone 2: Type Checking (Simple)
**Goal**: Type-check programs with primitive types, functions, and simple algebraic types.

### Deliverables
- [ ] `Codex.Semantics`: name resolution, scope analysis
- [ ] `Codex.Types`: bidirectional type checker for simple types
- [ ] Type inference for let bindings and function definitions
- [ ] Type error diagnostics with source locations
- [ ] Sum type definitions and pattern matching (exhaustiveness check)
- [ ] Record type definitions and field access
- [ ] `Codex.Types.Tests`: type checking tests — both success and failure cases

### Grammar Extensions
```
type-def    = name "=" "|" constructor ("|" constructor)*
constructor = name type*
record-def  = name "=" "record" "{" field ("," field)* "}"
field       = name ":" type
match       = "when" expr branch+
branch      = "if" pattern "→" expr
pattern     = constructor pattern* | name | literal | "_"
```

### Demo Program
```
Result (a) =
  | Success (a)
  | Failure Text

safe-divide : Integer → Integer → Result Integer
safe-divide (x) (y) =
  if y == 0
    then Failure "division by zero"
    else Success (x / y)

describe : Result Integer → Text
describe (result) =
  when result
    if Success (n) → "got " ++ show n
    if Failure (msg) → "error: " ++ msg
```

Type-check it. Report any errors. No execution yet.

### Estimated Effort
Large. The type checker is the core of the project.

---

## Milestone 3: Execution via C#
**Goal**: Compile a type-checked Codex program to C# and run it.

### Deliverables
- [ ] `Codex.IR`: IR definition, lowering from elaborated AST → IR
- [ ] `Codex.Emit.CSharp`: C# code emitter
- [ ] Codex runtime library for C# (Unit, Maybe, Result, CodexList)
- [ ] `Codex.Cli`: `codex check`, `codex build`, `codex run` commands
- [ ] `Codex.Integration.Tests`: end-to-end tests (source → compile → run → verify output)

### Demo
```
codex run hello.codex
> 25
```

Where `hello.codex` is the `square 5` program, and it actually executes.

### Estimated Effort
Medium-large. The C# emitter has many edge cases but the core is straightforward.

---

## Milestone 4: Prose Integration
**Goal**: Parse and process Codex source that includes prose. The literate programming model works.

### Deliverables
- [ ] Lexer: prose mode / notation mode switching
- [ ] Parser: chapter headers, section headers, prose blocks, prose templates
- [ ] Prose template matching: "An X is a record containing:", "X is either:", etc.
- [ ] The Reader: formatted prose output (CLI: `codex read <file>` renders to terminal)
- [ ] The account module example from `NewRepository.txt` parses and type-checks

### Demo Program
```codex
Chapter: Greeting

  This module provides a simple greeting function.
  Given a person's name, it produces a greeting message.

  We say:

    greet : Text → Text
    greet (name) = "Hello, " ++ name ++ "!"

  To greet the world:

    main : Text
    main = greet "World"
```

Parse it. Type-check it. Compile it. Run it. Print `Hello, World!`

### Estimated Effort
Medium. The prose lexer/parser is new territory but we've designed it well.

---

## Milestone 5: Effects
**Goal**: The effect system works. Pure functions are enforced. Effectful functions declare their effects.

### Deliverables
- [ ] Effect row types in `Codex.Types`
- [ ] Effect checking: pure functions cannot call effectful functions
- [ ] Effect polymorphism: `map` propagates effects
- [ ] Built-in effects: `Console` (read/write console), `State`
- [ ] Effect handlers: `run-state`
- [ ] C# backend: effects encoded as contexts/interfaces

### Demo Program
```codex
Chapter: Effectful Hello

  This program reads a name from the console
  and prints a greeting.

    main : [Console] Nothing
    main = do
      name ← read-line
      print-line ("Hello, " ++ name ++ "!")
```

Compile and run. It actually reads from stdin and writes to stdout.

### Estimated Effort
Large. Algebraic effects are complex to implement correctly.

---

## Milestone 6: Linear Types
**Goal**: Linear types enforce resource safety.

### Deliverables
- [ ] Linearity annotations in `Codex.Types`
- [ ] Linearity checker
- [ ] `FileHandle` as a linear type
- [ ] File system effect + linear file handles
- [ ] C# backend: linear types encoded as runtime checks

### Demo Program
```codex
Chapter: Safe File Reading

    read-entire-file : Path → [FileSystem] Result Text
    read-entire-file (path) = do
      handle ← open-file path
      contents ← read-all handle
      close-file handle
      succeed contents
```

If you forget `close-file`, the compiler rejects the program.

### Estimated Effort
Medium. The linearity checker is well-understood (Linear Haskell paper).

---

## Milestone 7: Repository (Local)
**Goal**: The local fact store works. Definitions are content-addressed.

### Deliverables
- [ ] `Codex.Repository`: local content-addressed store
- [ ] Facts: Definition, Supersession
- [ ] Views: single-user views
- [ ] CLI: `codex init`, `codex publish`, `codex history`
- [ ] Import from repository: `import Account` resolves from the store

### Demo
```
codex init my-project
codex publish account-module.codex
codex history Account.deposit
> v1 (2025-06-20) by damian: "Initial implementation"
```

### Estimated Effort
Medium. Content-addressed storage is well-understood.

---

## Milestone 8: Dependent Types (Basic)
**Goal**: Types can depend on values. Vector with length. Proof obligations generated.

### Deliverables
- [ ] Dependent function types: `(n : Integer) → Vector n a → ...`
- [ ] Type-level arithmetic: `m + n` evaluated during type checking
- [ ] Proof obligations: `index` requires proof that index < length
- [ ] Simple proof discharge: literal evidence and context-based evidence
- [ ] The `Vector` type with `append` having the correct dependent type

### Demo Program
```codex
Chapter: Vectors

    append : Vector m a → Vector n a → Vector (m + n) a
    
    example : Vector 5 Integer
    example = append [1, 2, 3] [4, 5]
```

Type-checks and compiles. The compiler verifies that 3 + 2 = 5.

### Estimated Effort
Very large. This is the hardest type system feature.

---

## Milestone 9: LSP & Editor Integration
**Goal**: Write Codex in VS Code with syntax highlighting, error reporting, and hover.

### Deliverables
- [ ] `Codex.Lsp`: Language Server Protocol implementation
- [ ] Diagnostics (errors/warnings pushed to editor)
- [ ] Completion (type-aware)
- [ ] Hover (Narrator: type + prose)
- [ ] Go to definition
- [ ] Document symbols (outline)
- [ ] Semantic tokens (syntax highlighting)
- [ ] VS Code extension (thin wrapper)

### Demo
Open a `.codex` file in VS Code. See syntax highlighting. See type errors inline. Hover over a function to see its type and prose description.

### Estimated Effort
Medium-large. LSP is well-specified but the integration is fiddly.

---

## Milestone 10: Proofs
**Goal**: Users can write and verify proofs. The compiler checks them.

### Deliverables
- [ ] `Codex.Proofs`: proof terms, proof checker
- [ ] Proof by induction
- [ ] Proof by case analysis
- [ ] Proof by rewriting
- [ ] Claims and proofs in the source
- [ ] The reverse-reverse proof from `NewRepository.txt` works

### Demo Program
```codex
Claim: reversing a list twice returns the original list.

  reverse-reverse : ∀ (xs : List a) → reverse (reverse xs) ≡ xs

  Proof: by induction on xs.
    Base case: xs = []
      reverse (reverse []) = reverse [] = []  ✓
    Inductive step: xs = (head :: tail)
      Assume reverse (reverse tail) = tail.
      ...  ✓
```

The compiler verifies the proof.

### Estimated Effort
Very large. Proof checking is essentially theorem proving.

---

## Milestone 11: Collaboration
**Goal**: The proposal/verdict protocol works for multi-user collaboration.

### Deliverables
- [ ] Proposals and Verdicts in the repository
- [ ] Stakeholder management
- [ ] CLI: `codex propose`, `codex verdict`, `codex proposals`
- [ ] Fact synchronization between stores
- [ ] Trust facts: vouching

### Estimated Effort
Medium-large.

---

## Milestone 12: Additional Backends
**Goal**: Codex compiles to JavaScript and Rust.

### Deliverables
- [ ] `Codex.Emit.JavaScript`: TypeScript/JavaScript emitter
- [ ] `Codex.Emit.Rust`: Rust emitter
- [ ] Backend capability validation
- [ ] Integration tests for each backend

### Estimated Effort
Medium per backend.

---

## Milestone 13: Self-Hosting
**Goal**: The Codex compiler is written in Codex and compiles itself.

### Deliverables
- [ ] Codex compiler source in Codex (the lexer, parser, type checker, etc.)
- [ ] Stage 0 (C# compiler) compiles Stage 1 (Codex compiler)
- [ ] Stage 1 compiles itself to produce Stage 2
- [ ] Stage 1 output = Stage 2 output (bootstrap verified)

### Estimated Effort
Enormous. This is the culmination of the entire project.

---

## Summary Timeline

| Milestone | Name | Dependencies | Size |
|-----------|------|-------------|------|
| 0 | Foundation | — | S |
| 1 | Hello Notation | 0 | M |
| 2 | Type Checking | 1 | L |
| 3 | Execution via C# | 2 | M-L |
| 4 | Prose Integration | 1 | M |
| 5 | Effects | 2 | L |
| 6 | Linear Types | 2 | M |
| 7 | Repository | 0 | M |
| 8 | Dependent Types | 2 | XL |
| 9 | LSP & Editor | 3, 4 | M-L |
| 10 | Proofs | 8 | XL |
| 11 | Collaboration | 7 | M-L |
| 12 | Additional Backends | 3 | M×N |
| 13 | Self-Hosting | All | XXL |

### Critical Path
```
0 → 1 → 2 → 3 → (4,5,6 parallel) → 8 → 10 → 13
                                    ↗
                          7 → 11 →
```

Milestones 4, 5, 6, and 7 can proceed in parallel once Milestone 2 (type checking) is complete. Milestone 9 (LSP) can start after 3 and 4.
