# 04 — Compiler Pipeline

## Overview

The Codex compiler is a multi-phase pipeline. Each phase transforms data from one representation to the next. Every phase is a standalone library that can be tested in isolation.

```
Source Text (.codex)
       │
       ▼
┌─────────────┐
│    Lexer     │  Codex.Syntax
│  (Tokenizer) │
└──────┬──────┘
       │  Token Stream
       ▼
┌─────────────┐
│    Parser    │  Codex.Syntax
│              │
└──────┬──────┘
       │  Concrete Syntax Tree (CST)
       ▼
┌─────────────┐
│  Desugarer   │  Codex.Ast
│              │
└──────┬──────┘
       │  Abstract Syntax Tree (AST)
       ▼
┌──────────────┐
│Name Resolver │  Codex.Semantics
│Scope Analyzer│
└──────┬───────┘
       │  Resolved AST (names bound to definitions)
       ▼
┌─────────────┐
│ Type Checker │  Codex.Types
│ + Elaborator │
└──────┬──────┘
       │  Elaborated AST (all types explicit, proofs checked)
       ▼
┌─────────────┐
│   Lowering   │  Codex.IR
│  AST → IR    │
└──────┬──────┘
       │  Intermediate Representation (typed, optimized)
       ▼
┌─────────────┐
│ Optimization │  Codex.IR
│   Passes     │
└──────┬──────┘
       │  Optimized IR
       ▼
┌─────────────┐
│   Emitter    │  Codex.Emit.*
│  (per target)│
└──────┬──────┘
       │  Target code (C#, Rust, JS, Python, WASM, LLVM IR)
       ▼
    Output
```

Each phase also feeds into the **Diagnostic System** — errors, warnings, and suggestions accumulate and are reported at the end (or incrementally in IDE mode).

---

## Phase 1: Lexer

### Input
Raw UTF-8 source text.

### Output
A stream of `Token` values, each with:
- `TokenKind` (enum: Identifier, Keyword, Operator, IntLiteral, TextLiteral, etc.)
- `Span` (start offset, end offset, line, column)
- `LeadingTrivia` (whitespace, newlines before the token)
- `TrailingTrivia` (whitespace, newlines after the token)

### Key Design Decisions

**Hand-written lexer**, not a generator (Flex, ANTLR, etc.). Reasons:
1. The prose/notation mode switching requires context-sensitive tokenization
2. We need perfect trivia tracking for round-trip fidelity (the CST must reproduce the source exactly)
3. Error recovery in the lexer is critical for IDE support — a generated lexer gives up too easily

**Indentation tracking**: The lexer tracks indentation depth and emits synthetic `Indent` / `Dedent` / `Newline` tokens (Python-style). These are used by the parser to determine block structure.

**Mode switching**: The lexer maintains a mode stack:
- `ProseMode`: tokenizes natural language, recognizes transition phrases
- `NotationMode`: tokenizes formal notation (identifiers, operators, keywords)
- `StringMode`: inside a text literal
- `InterpolationMode`: inside `${ ... }` in an interpolated string

Mode transitions:
- `ProseMode → NotationMode`: when indentation increases after a prose line
- `NotationMode → ProseMode`: when indentation returns to prose level
- Any mode → `StringMode`: when `"` is encountered
- `StringMode → InterpolationMode`: when `${` is encountered

### Implementation Notes

```csharp
public class Lexer
{
    private readonly SourceText _source;
    private int _position;
    private int _line;
    private int _column;
    private readonly Stack<LexerMode> _modeStack;
    private readonly Stack<int> _indentStack;
    
    public Token NextToken();
    public IEnumerable<Token> TokenizeAll();
}
```

The lexer is a pull-based iterator. The parser calls `NextToken()` on demand.

---

## Phase 2: Parser

### Input
Token stream from the lexer.

### Output
Concrete Syntax Tree (CST) — a lossless representation of the source. The CST preserves:
- All whitespace and formatting
- All prose text
- All trivia
- Parentheses, even when redundant
- The exact token sequence

### CST vs. AST

The CST is the parser's output. It is a tree of `SyntaxNode` values where every node contains its child tokens and nodes. It can reconstruct the source text exactly.

The AST is a simplified tree used for semantic analysis. It discards trivia and normalizes syntax.

We maintain both because:
- The CST is needed for the IDE (formatting, refactoring, syntax highlighting)
- The AST is needed for the compiler (type checking, code generation)

### Parser Design

**Recursive descent** with **Pratt parsing** for expressions.

Top-level structure:
```
parseDocument()
  → parseChapter()*

parseChapter()
  → expect("Chapter" header)
  → parseProse()
  → parseSection()*

parseSection()
  → expect("Section" header)
  → parseProse()
  → parseSectionItem()*

parseSectionItem()
  → parseDefinition()
  | parseClaim()
  | parseProof()
  | parseProseBlock()

parseDefinition()
  → parseProse()        -- the introductory explanation
  → parseSignature()    -- the type signature
  → parseBody()         -- the implementation
```

Expression parsing uses Pratt parsing with precedence levels:

| Precedence | Operators |
|-----------|-----------|
| 1 (lowest) | `→`, `←` |
| 2 | `\|\|` |
| 3 | `&&` |
| 4 | `≡`, `≠`, `<`, `>`, `≤`, `≥` |
| 5 | `++` |
| 6 | `+`, `-` |
| 7 | `*`, `/` |
| 8 | `^` |
| 9 | unary `-`, `not` |
| 10 (highest) | function application (juxtaposition) |

### Error Recovery

The parser must be **error-tolerant**. A syntax error in one definition should not prevent parsing the rest of the document. Recovery strategies:

1. **Synchronization tokens**: on error, skip tokens until we find a chapter header, section header, or definition header at the expected indentation level
2. **Error nodes**: insert `ErrorNode` in the CST containing the skipped tokens
3. **Partial parse**: if a definition's signature parses but its body doesn't, keep the signature

This is essential for IDE support — the user is always in the middle of typing, and the parser must produce useful output from incomplete/incorrect input.

---

## Phase 3: Desugaring (CST → AST)

### Input
CST from the parser.

### Output
AST — a clean, normalized representation with:
- Prose blocks preserved but separated from notation
- Syntactic sugar expanded
- All structure explicit

### Desugaring Rules

| Sugar | Expansion |
|-------|-----------|
| `if a then b else c` | `when a if True → b if False → c` |
| `a \|\| b` | `when a if True → True if False → b` |
| `a && b` | `when a if False → False if True → b` |
| `[1, 2, 3]` | `Cons 1 (Cons 2 (Cons 3 Nil))` |
| `do { x ← e; body }` | `bind e (λx → body)` |
| `f \|> g` | `g (f)` (pipe) |
| Record update `r with { f = v }` | Constructor call with shared fields |

### Prose Template Matching

During desugaring, recognized prose templates are matched and converted to AST nodes:

```
"An Account is a record containing:" 
  → RecordTypeHeader { name = "Account" }

"such that balance equals the sum of all amounts in history"
  → DependentConstraint { ... }
```

Unrecognized prose becomes `ProseBlock` nodes in the AST — preserved for documentation, but not semantically analyzed.

---

## Phase 4: Name Resolution & Scope Analysis

### Input
Raw AST.

### Output
Resolved AST where every identifier is bound to its declaration.

### Process

1. **Collect declarations**: walk the AST and record all top-level definitions (types, functions, claims, proofs) in a symbol table
2. **Build scope tree**: nested scopes for chapters, sections, let-bindings, lambda parameters, pattern variables
3. **Resolve references**: walk every expression and resolve identifiers to their declarations
4. **Detect errors**: undefined names, ambiguous names, shadowing warnings

### Module Resolution

When an `import` is encountered:
1. Look up the chapter by name in the current document
2. If not found, look in the repository (content-addressed lookup)
3. Bind the imported names into the current scope

### Order Independence

Definitions within a chapter can appear in any order (the reader's order, not the dependency order). The name resolver builds a dependency graph and topologically sorts it. Mutually recursive definitions are grouped into strongly connected components.

---

## Phase 5: Type Checking & Elaboration

### Input
Resolved AST.

### Output
Elaborated AST where:
- All types are explicit (no unresolved type variables)
- All implicit arguments are filled in
- All proof obligations are resolved or recorded as holes
- All linearity annotations are computed
- All effect rows are resolved

### Process

This is the big one. See `03-TYPE-SYSTEM.md` for the type system design. The implementation follows bidirectional type checking:

```
check(expr, expectedType) → ElaboratedExpr
infer(expr) → (ElaboratedExpr, Type)
```

Key sub-passes:
1. **Type inference**: bidirectional checking with unification
2. **Constraint solving**: unify type variables, solve effect rows
3. **Linearity checking**: verify usage counts for linear bindings
4. **Effect checking**: verify effect rows are consistent
5. **Proof checking**: verify proof terms for dependent type constraints
6. **Elaboration**: insert implicit arguments, coercions, proof witnesses

### Incremental Type Checking

For IDE support, the type checker must be **incremental**. When a single definition changes:
1. Re-check only that definition and its dependents
2. Reuse cached results for everything else

This requires tracking dependencies between definitions at the type-checking level.

---

## Phase 6: Lowering (AST → IR)

### Input
Elaborated AST.

### Output
Codex IR — a typed, lower-level representation suitable for optimization and code generation.

### IR Design

The IR is a **typed A-normal form** (ANF). Every intermediate computation is named. No nested expressions.

```
// AST: f (g x) + h y
// IR:
let t1 = call g x
let t2 = call f t1
let t3 = call h y
let t4 = add t2 t3
return t4
```

IR nodes:
- `IRLet (name, value, body)` — let binding
- `IRCall (function, args)` — function application
- `IRMatch (scrutinee, branches)` — pattern match
- `IRLiteral (value)` — literal value
- `IRLambda (params, body)` — lambda (closures)
- `IRReturn (value)` — return from function
- `IREffect (effect, operation, args)` — effect operation

Each IR node carries:
- Its type (fully resolved)
- Its effect row
- Its linearity annotation
- Source span (for debugging and error reporting)

---

## Phase 7: Optimization

### Input
Unoptimized IR.

### Output
Optimized IR.

### Optimization Passes

Passes are run in a fixed order. Each pass transforms the IR and must preserve types (the IR can be re-type-checked after each pass as a sanity check).

| Pass | What It Does |
|------|-------------|
| Dead code elimination | Remove unreachable definitions and unused let-bindings |
| Constant folding | Evaluate expressions with known values at compile time |
| Inlining | Inline small functions at their call sites |
| Common subexpression elimination | Share identical computations |
| Effect erasure | Remove effect annotations for pure sub-expressions |
| Monomorphization | Specialize generic functions for concrete types (needed for some backends) |
| Lambda lifting | Convert closures to top-level functions with explicit environments |
| Tail call optimization | Convert tail-recursive calls to loops |

Phase 1 implementation: only dead code elimination and constant folding. The rest are added as needed.

---

## Phase 8: Code Emission

### Input
Optimized IR.

### Output
Target language source code (or binary, for WASM/LLVM).

### Emitter Interface

```csharp
public interface ICodeEmitter
{
    string TargetName { get; }
    TargetCapabilities Capabilities { get; }
    EmitResult Emit(IRModule module, EmitOptions options);
}

public record TargetCapabilities(
    bool SupportsDependentTypes,
    bool SupportsLinearTypes,
    bool SupportsEffectTypes,
    bool SupportsHigherKindedTypes,
    bool SupportsTailCalls,
    bool SupportsArbitraryPrecision
);
```

Each backend implements `ICodeEmitter`. The emission framework:
1. Checks that the IR only uses features the target supports
2. For unsupported features, inserts runtime checks or rejects with an explanation
3. Emits target code

### Backend Priority

| Backend | Priority | Rationale |
|---------|----------|-----------|
| C# | **First** | Bootstrap target — Codex compiles to .NET, runs on .NET |
| JavaScript | Second | Browser target, widest deployment |
| Rust | Third | Full-fidelity target, proves the type system maps cleanly |
| Python | Fourth | Data science / scripting use case |
| WASM | Fifth | Universal binary target |
| LLVM IR | Sixth | Native compilation |

The C# backend is the only one needed for bootstrap. All others are post-bootstrap.

---

## Diagnostic System

All phases feed into a unified diagnostic system.

```csharp
public record Diagnostic(
    DiagnosticSeverity Severity,   // Error, Warning, Info, Hint
    string Code,                    // e.g., "CDX0042"
    string Message,                 // Human-readable description
    SourceSpan Location,            // Where in the source
    ImmutableArray<SourceSpan> RelatedLocations,  // Other relevant spans
    ImmutableArray<CodeFix> SuggestedFixes        // Automated fix suggestions
);
```

Diagnostics are:
- **Accumulated**, not thrown. Compilation continues past errors when possible.
- **Rich**: they include related locations and suggested fixes.
- **Unique**: each diagnostic has a code (e.g., `CDX0042`) for filtering and documentation.

### Diagnostic Categories

| Range | Category |
|-------|----------|
| CDX0001–CDX0999 | Lexer errors |
| CDX1000–CDX1999 | Parser errors |
| CDX2000–CDX2999 | Name resolution errors |
| CDX3000–CDX3999 | Type errors |
| CDX4000–CDX4999 | Linearity errors |
| CDX5000–CDX5999 | Effect errors |
| CDX6000–CDX6999 | Proof errors |
| CDX7000–CDX7999 | IR lowering errors |
| CDX8000–CDX8999 | Emission errors / target limitations |
| CDX9000–CDX9999 | Repository errors |

---

## Compilation Modes

| Mode | Behavior |
|------|----------|
| **Full** | All phases, produce output. Errors stop compilation at the failing phase. |
| **Check** | Phases 1–5 only (lex, parse, resolve, type-check). No IR, no emission. Fast. |
| **IDE** | Incremental, error-tolerant, produces partial results. Used by LSP. |
| **REPL** | Single expression/definition at a time, evaluated immediately. |

---

## Performance Considerations

The compiler must be fast enough for interactive use:
- **Target**: < 100ms for re-checking a single definition change in IDE mode
- **Target**: < 5s for full compilation of a 10,000-line Codex program
- **Strategy**: incremental checking, parallel type checking of independent definitions, lazy IR generation

These are aspirational targets. We measure early and often.
