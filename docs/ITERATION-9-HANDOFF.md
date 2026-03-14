# Iteration 9 — Handoff Summary

**Date**: 2026-03-15
**Branch**: `master`
**Remote**: https://github.com/damiant3/NewRepository

---

## What Was Done

### Milestone Status

| Milestone | Status | Notes |
|-----------|--------|-------|
| M0–M7 | ✅ Complete | See ITERATION-7-HANDOFF.md |
| M9: LSP & Editor | ✅ Complete | See ITERATION-8-HANDOFF.md |
| **M8: Dependent Types (Basic)** | **✅ Complete** | Dependent function types, type-level arithmetic, Vector type support |

### M8 — Dependent Types (Basic)

#### New Type System Types (`src/Codex.Types/CodexType.cs`)

| Type | Purpose |
|------|---------|
| `DependentFunctionType(ParamName, ParamType, Body)` | `(n : Integer) → Vector n a → ...` |
| `TypeLevelValue(Value)` | Literal integer in type position (e.g., `5` in `Vector 5 a`) |
| `TypeLevelBinary(Op, Left, Right)` | Type-level arithmetic (e.g., `m + n`) |
| `TypeLevelVar(Name)` | Type-level variable (e.g., `n` from a dependent binder) |
| `ProofType(Claim)` | Proof obligation placeholder |
| `LessThanClaim(Left, Right)` | `i < n` claim for proof obligations |
| `TypeLevelOp` enum | `Add`, `Sub`, `Mul` |

#### New Syntax Nodes

| Layer | Node | Purpose |
|-------|------|---------|
| CST (`SyntaxNodes.cs`) | `DependentTypeNode` | `(n : Integer) → Body` |
| CST (`SyntaxNodes.cs`) | `IntegerTypeNode` | Integer literal in type position |
| CST (`SyntaxNodes.cs`) | `BinaryTypeNode` | `(m + n)` in type position |
| AST (`AstNodes.cs`) | `DependentTypeExpr` | Desugared dependent function type |
| AST (`AstNodes.cs`) | `IntegerLiteralTypeExpr` | Desugared integer literal type |
| AST (`AstNodes.cs`) | `BinaryTypeExpr` | Desugared type-level binary expression |

#### Parser Changes (`src/Codex.Syntax/Parser.cs`)

- `ParseTypeAtom`: detects `(name : Type) →` pattern via `IsDependentTypeLookahead()` lookahead
- `ParseTypeAtom`: handles `IntegerLiteral` tokens as type atoms
- `ParseTypeAtom`: detects `+`, `-`, `*` inside parenthesized type expressions → `BinaryTypeNode`
- Type application argument loop now accepts `IntegerLiteral` tokens

#### Desugarer Changes (`src/Codex.Ast/Desugarer.cs`)

- Maps `DependentTypeNode` → `DependentTypeExpr`
- Maps `IntegerTypeNode` → `IntegerLiteralTypeExpr`
- Maps `BinaryTypeNode` → `BinaryTypeExpr`

#### TypeChecker Changes (`src/Codex.Types/TypeChecker.cs`)

- `m_typeLevelEnv`: tracks type-level variable bindings (e.g., `n` from `(n : Integer) → ...`)
- `ResolveDependentType`: binds parameter name as `TypeLevelVar` in type-level env, resolves body
- `ResolveTypeLevelBinary`: resolves both sides, normalizes if both are constants
- `NormalizeTypeLevelExpr`: constant-folds `TypeLevelBinary` when both operands are `TypeLevelValue`
- `ResolveNamedType`: checks `m_typeLevelEnv` first for type-level variables
- `InferDefinition`: unwraps `DependentFunctionType` alongside `FunctionType`
- `ExtractEffects`: unwraps `DependentFunctionType` to find effect tail
- `SubstituteVar`: handles all new types

#### Unifier Changes (`src/Codex.Types/Unifier.cs`)

- Unifies `DependentFunctionType` with `DependentFunctionType` (param type + body)
- Cross-unifies `DependentFunctionType` ↔ `FunctionType` (erased dependency)
- Unifies `TypeLevelValue` by value equality
- Normalizes `TypeLevelBinary` before comparing
- Unifies `TypeLevelVar` by name
- Unifies `ProofType`, `LessThanClaim` structurally
- `DeepResolve` and `OccursIn` handle all new types
- `NormalizeTypeLevelExpr`: static normalizer for type-level arithmetic

#### Lowering Changes (`src/Codex.IR/Lowering.cs`)

- `LowerDefinition`, `LowerLambda`, `LowerApply`: unwrap `DependentFunctionType` like `FunctionType` (runtime erasure)

#### CSharpEmitter Changes (`src/Codex.Emit.CSharp/CSharpEmitter.cs`)

- `EmitType`: `DependentFunctionType` → `Func<P, R>`, type-level types → `long` or `object`
- `GetReturnType`, `IsEffectfulDefinition`: unwrap `DependentFunctionType`
- Fixed unnecessary assignment warning on line 684 (pre-existing)

#### LinearityChecker Changes (`src/Codex.Types/LinearityChecker.cs`)

- Unwraps `DependentFunctionType` when walking parameter types

#### CLI Changes (`tools/Codex.Cli/Program.cs`)

- `FormatType` / `FormatTypeExpr`: display dependent types, integer literals, binary type expressions

### Bug Fixes

- Restored `TypeDef` hierarchy in `AstNodes.cs` (accidentally deleted)
- Restored `SkipToNextDefinition` in `Parser.cs` (accidentally deleted)
- Fixed `start.Through(endSpan)` → `startSpan.Through(endSpan)` typo in `ParseVariantTypeBody`
- Fixed missing `)` in `EmitType` string interpolation

### Test Count

**174 tests, all passing** (16 Core + 11 Ast + 55 Syntax + 10 Semantics + 68 Types + 14 LSP)

New tests:
- 5 TypeChecker tests: dependent function type resolution, type-level integer literals, type-level arithmetic normalization, constructed types with integer arguments, type-level binary addition
- 4 Parser tests: dependent function type parsing, integer literal type arguments, type-level binary in parens, nested dependent types

---

## Demo

```codex
append : (m : Integer) -> (n : Integer) -> Vector m a -> Vector n a -> Vector (m + n) a

example : Vector 5 Integer
example = append [1, 2, 3] [4, 5]
```

The type checker:
1. Parses `(m : Integer) →` as a dependent function type
2. Resolves `m` and `n` as type-level variables in the body
3. Evaluates `(m + n)` as type-level arithmetic
4. When applied with `Vector 3 Integer` and `Vector 2 Integer`, normalizes `3 + 2` to `5`
5. Unifies `Vector 5 Integer` with the declared return type ✓

---

## Known Limitations / Next

### M8 Remaining

- **Proof obligation generation** — `ProofType` and `LessThanClaim` are defined but not yet generated during type checking (e.g., `index` requiring `i < n`)
- **Proof discharge** — no automatic search or explicit proof terms yet
- **Type-level function evaluation** — only arithmetic on constants; no user-defined type-level functions
- **Totality checking** — not yet enforced for type-level expressions

### LSP Remaining (from M9)

- **Pre-built server binary** — currently requires `dotnet run`; should publish self-contained
- **Incremental analysis** — currently re-analyzes full document on every keystroke

### Next Milestones

| Priority | Milestone | Notes |
|----------|-----------|-------|
| 1 | M8 Phase 2 | Proof obligations, proof discharge, totality |
| 2 | M11: Collaboration | Multi-user repository sync |
| 3 | M10: Proofs | Full proof checking |

---

## Key Code Locations

| Task | File |
|------|------|
| Add new type-level construct | `src/Codex.Types/CodexType.cs` |
| Type-level normalization | `src/Codex.Types/TypeChecker.cs` (`NormalizeTypeLevelExpr`) and `src/Codex.Types/Unifier.cs` |
| Proof obligation generation | `src/Codex.Types/TypeChecker.cs` — add to `InferApplication` or `ResolveTypeExpr` |
| Dependent type parsing | `src/Codex.Syntax/Parser.cs` (`ParseTypeAtom`, `IsDependentTypeLookahead`) |
| Runtime erasure | `src/Codex.IR/Lowering.cs` — dependent types erase to plain functions |
