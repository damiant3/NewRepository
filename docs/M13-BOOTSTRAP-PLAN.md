# M13 — Self-Hosting Bootstrap Plan

## Goal

Write the Codex compiler in Codex, then compile it with the Stage 0 (C#) compiler
to produce a Stage 1 binary that can compile its own source.

## Bootstrap Stages

| Stage | What It Is | Compiled By |
|-------|-----------|-------------|
| **Stage 0** | Current C# implementation (`src/`) | `dotnet build` |
| **Stage 1** | Codex source in `codex-src/` compiled to C# by Stage 0 | `codex build --target cs` |
| **Stage 2** | Same Codex source compiled by Stage 1 | Stage 1 binary |

**Success criterion**: Stage 1 output ≡ Stage 2 output (fixed point).

## Source Layout

```
codex-src/
  Core/
    ContentHash.codex     — SHA-256 hashing, hex encoding
    Name.codex            — Name and QualifiedName types
    SourceText.codex      — SourcePosition, SourceSpan, SourceText
    Diagnostic.codex      — DiagnosticSeverity, Diagnostic, DiagnosticBag
    Collections.codex     — List operations, Map, Set (functional)
  Syntax/
    TokenKind.codex       — Token kind enumeration
    Token.codex           — Token record
    Lexer.codex           — Lexer (functional, character-by-character)
    SyntaxNodes.codex     — CST node types
    Parser.codex          — Recursive descent parser
  Ast/
    AstNodes.codex        — AST node types
    Desugarer.codex       — CST → AST transformation
  Semantics/
    NameResolver.codex    — Scope analysis, name resolution
  Types/
    CodexType.codex       — Type representations
    TypeChecker.codex     — Bidirectional type checking
  IR/
    IRModule.codex        — IR node types
    Lowering.codex        — AST → IR transformation
  Emit/
    CSharpEmitter.codex   — IR → C# source text
  Main.codex              — Entry point, CLI dispatch
```

## Language Features Required

The Codex compiler needs these features to express itself:

| Feature | Status | Notes |
|---------|--------|-------|
| Sum types | ✅ Available | TokenKind, AST nodes, IR nodes, Types |
| Record types | ✅ Available | Token, Diagnostic, SourceSpan, etc. |
| Pattern matching | ✅ Available | Core of every compiler pass |
| Recursion | ✅ Available | Recursive descent parsing |
| Higher-order functions | ✅ Available | map, fold over lists |
| String operations | ⚠️ Partial | Need: char-at, length, substring, char predicates |
| List operations | ⚠️ Partial | Need: cons, head, tail, append, map, fold, filter |
| Effects (Console/IO) | ✅ Available | File reading, output writing |
| Let bindings | ✅ Available | Local variables |
| Do-notation | ✅ Available | Sequential effectful code |

## Phase 1: Core Types (Current)

Express the fundamental types that everything else depends on:
- `TokenKind` — a large sum type (enum)
- `Token` — a record
- `SourcePosition`, `SourceSpan` — records
- `Name` — a record with predicates
- `Diagnostic` — a record with severity

These files exercise sum types, record types, and simple functions.
They can be compiled by Stage 0 right now with no new features.

## Phase 2: String & List Primitives

Before we can write the Lexer in Codex, we need built-in primitives for:
- `char-at : Text -> Integer -> Text` (single character as text)
- `text-length : Text -> Integer`
- `substring : Text -> Integer -> Integer -> Text`
- `is-letter : Text -> Boolean`
- `is-digit : Text -> Boolean`
- `is-whitespace : Text -> Boolean`
- `text-to-integer : Text -> Integer`

These will be added as compiler built-ins (recognized by the type checker,
emitted as inline code by all backends).

## Phase 3: Lexer in Codex

The lexer is a natural starting point — it's a state machine that
consumes characters and produces tokens. In functional style:

```
LexState = record {
  source : Text,
  offset : Integer,
  line : Integer,
  column : Integer
}

LexResult (a) =
  | LexOk (a) (LexState)
  | LexError (Text) (LexState)
```

The lexer becomes a series of functions that thread `LexState` through.

## Phase 4: Parser, AST, Desugarer

Recursive descent parsing expressed functionally — each parse function
takes a token list + position, returns a (node, new-position) pair.

## Phase 5: Type Checker, IR, Emitter

The most complex pieces. The type checker needs Map for type environments.
The emitter needs StringBuilder-like text accumulation.

## Phase 6: Bootstrap Verification

1. Stage 0 compiles `codex-src/` → `stage1.cs`
2. Compile `stage1.cs` with `dotnet` → `stage1.exe`
3. `stage1.exe` compiles `codex-src/` → `stage2.cs`
4. Verify `stage1.cs` ≡ `stage2.cs` (byte-for-byte)

## Estimated Effort

| Phase | Scope | Sessions |
|-------|-------|----------|
| Phase 1 | Core types in Codex | 1 |
| Phase 2 | String/list primitives | 1 |
| Phase 3 | Lexer in Codex | 1 |
| Phase 4 | Parser + AST | 1-2 |
| Phase 5 | TypeChecker + IR + Emitter | 2-3 |
| Phase 6 | Bootstrap verification | 1 |
| **Total** | | **7-9 sessions** |


Root Cause Found: Stage 0 Parser Greedy Branch Consumption
The Stage 0 ParseMatchExpression (in src/Codex.Syntax/Parser.cs) parses match branches with while (Current.Kind == TokenKind.IfKeyword) — it greedily consumes ALL if branches regardless of indentation. When a when expression is nested inside a branch body of an outer when, the inner match steals all subsequent if branches from the outer match. This caused type errors in multi-file compilation where TokenKind branches were being swallowed by inner ParseTypeResult or ParseExprResult matches.
Parser.codex Rewrite
Completely rewrote codex-src/Syntax/Parser.codex with a strict rule: no when expression may contain another when in a non-final branch body. The techniques used:
1.	unwrap-X-ok functions — Dedicated single-branch when unwrappers (unwrap-expr-ok, unwrap-type-ok, unwrap-pat-ok, unwrap-pat-for-expr) that deconstruct result types and pass components to a continuation function. Since these have only one if branch, they never steal from an outer match.
2.	if/then/else + predicate helpers — All multi-way dispatch on TokenKind uses chains of if is-X (current-kind st) then ... else ... instead of when current-kind st. ~25 TokenKind → Boolean predicates were grouped into their own section.
3.	Continuation-passing — Complex parse functions are decomposed into small steps connected by continuations, e.g., parse-if-expr → parse-if-then → parse-if-else → finish-if.
Files Changed
File	Change
codex-src/Syntax/Parser.codex	Complete rewrite avoiding nested when
codex-src/Syntax/SyntaxNodes.codex	New file — CST node types (Expr, Pat, TypeExpr, Def, TypeDef, Document)
src/Codex.Types/Unifier.cs	Removed temporary debug output
Verification
•	dotnet build Codex.sln — zero warnings
•	dotnet test Codex.sln — 246/246 tests pass
•	codex build codex-src/Syntax — compiles all 5 files (TokenKind + Token + Lexer + SyntaxNodes + Parser)
•	codex build codex-src — compiles all 8 files (Core + Syntax)
