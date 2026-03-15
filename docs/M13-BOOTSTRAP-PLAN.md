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

### Phase 5 Progress

**Completed**: IR node types (`IRModule.codex`), Lowering (`Lowering.codex`),
CodexType representations (`CodexType.codex`), and C# Emitter (`CSharpEmitter.codex`).

The type checker is NOT written in Codex — it requires mutable environments and
unification state that would need monadic threading. The Stage 0 type checker
handles all codex-src code. The Lowering and Emitter in Codex produce simplified
output (no full type propagation; uses `ErrorTy` placeholders for types not
available without a Codex-side type checker).

**Stage 0 compiler changes for Phase 5**:
- **Let-generalization** (`TypeChecker.cs`): After type-checking a definition with
  a type annotation, the declared type is immediately generalized (free type vars
  wrapped in `ForAllType`). At each call site, `Instantiate` creates fresh copies.
  This enables polymorphic reuse of functions like `map-list`.
- **Implicit type parameters** (`TypeChecker.cs`): Lowercase type names that don't
  resolve to any known type are treated as implicit type variables (fresh `TypeVariable`
  bound in `m_typeParamEnv`, scoped per definition).
- **New built-ins**: `integer-to-text`, `text-replace` added to NameResolver,
  TypeEnvironment, Lowering built-in maps, and CSharpEmitter emission.

## Phase 6: Bootstrap Verification

1. Stage 0 compiles `codex-src/` → `stage1.cs`
2. Compile `stage1.cs` with `dotnet` → `stage1.exe`
3. `stage1.exe` compiles `codex-src/` → `stage2.cs`
4. Verify `stage1.cs` ≡ `stage2.cs` (byte-for-byte)

## Estimated Effort

| Phase | Scope | Sessions |
|-------|-------|----------|
| Phase 1 | Core types in Codex | ✅ 1 |
| Phase 2 | String/list primitives | ✅ 1 |
| Phase 3 | Lexer in Codex | ✅ 1 |
| Phase 4 | Parser + AST + Desugarer | ✅ 2 |
| Phase 5 | IR + Lowering + Emitter | ✅ 1 |
| Phase 6 | Bootstrap verification | ⏳ next |
| **Total** | | **6 sessions so far** |

---

## Session Log

### Session 5 — Phase 4 completion + Phase 5 (AST, Desugarer, IR, Emitter)

**New codex-src files (6)**:
- `codex-src/Core/Collections.codex` — `map-list`, `fold-list` using accumulator loops
- `codex-src/Ast/AstNodes.codex` — AST node types (AExpr, APat, ATypeExpr, ADef, ATypeDef, AModule)
- `codex-src/Ast/Desugarer.codex` — CST → AST transformation (all match branches on single lines)
- `codex-src/Types/CodexType.codex` — Type representation sum type
- `codex-src/IR/IRModule.codex` — IR node types
- `codex-src/IR/Lowering.codex` — AST → IR transformation
- `codex-src/Emit/CSharpEmitter.codex` — IR → C# text emission
- `codex-src/Main.codex` — `compile` pipeline entry point

**Stage 0 compiler changes**:
1. **Let-generalization** (`src/Codex.Types/TypeChecker.cs`):
   - Annotated definitions are generalized immediately in pass 1 (free type vars → `ForAllType`)
   - `Instantiate` at call sites creates fresh copies of type vars
   - Result map strips `ForAllType` for external consumers
2. **Implicit type parameters** (`src/Codex.Types/TypeChecker.cs`):
   - `ResolveNamedType` treats lowercase names not found in any env as fresh type vars
   - `m_typeParamEnv` scoped per definition so `a` in different definitions → different vars
3. **New built-ins**: `integer-to-text` (Integer → Text), `text-replace` (Text → Text → Text → Text)
   - Added to: `NameResolver.cs`, `TypeEnvironment.cs`, `Lowering.cs`, `CSharpEmitter.cs`
4. **Sub-parser continuation line fix**: `++` at start of continuation lines is not supported
   by the notation-block sub-parser. Multi-line `++` chains must use helper functions.

**Verification**:
- `dotnet build Codex.sln` — zero warnings
- `dotnet test Codex.sln` — 246/246 tests pass
- `codex build codex-src` — compiles all 16 files (Core 4 + Syntax 5 + Ast 2 + Types 1 + IR 2 + Emit 1 + Main 1)
- Generated `codex-src/output.cs` — 94KB of C# code

Root Cause Found: Stage 0 Parser Greedy Branch Consumption
