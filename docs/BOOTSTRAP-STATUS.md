# Bootstrap Status — Step 1b Complete

**Date:** March 16, 2026
**State:** Stage 1 output (`stage1-output.cs`) successfully generated. Not yet at fixed point.

---

## What Just Happened

Ran `Codex.Bootstrap` which feeds the 21 `.codex` source files through the
Stage 1 compiler (the `Codex_Codex_Codex` class built from `out/Codex.Codex.cs`).
It successfully produced `Codex.Codex/stage1-output.cs`.

---

## Gap #1 Fix Iteration (TypeChecker.codex + Unifier.codex)

6 fixes were applied to the self-hosted type checker and unifier:

1. **infer-name**: Instantiate `ForAllTy` wrappers with fresh vars (polymorphic builtins)
2. **AFieldAccess/ARecordExpr**: Return fresh vars / `ConstructedTy` instead of `ErrorTy`
3. **resolve-type-expr for AAppType**: Handle `List` + type args instead of discarding them
4. **check-def**: Thread module env through so defs can see other defs' types
5. **unify-structural**: Handle `ConstructedTy`, `SumTy`, `RecordTy`, `ForAllTy`, cross-matching
6. **deep-resolve**: Recurse into `ConstructedTy`, `ForAllTy`, `SumTy`, `RecordTy`

### Results

| Metric | Before fixes | After fixes | Stage 0 target |
|--------|-------------|-------------|----------------|
| Unification errors | 1,864 | **1,066** | 0 |
| `object` type refs | 1,105 | **1,091** | 1 |
| `string` concrete | 87 | 81 | 475 |
| `long` concrete | 21 | 25 | 277 |
| `Func<` types | 2 | 2 | 618 |
| Defs / type-defs | 332 / 73 | 340 / 73 | 342 / 73 |

**Verdict:** Real progress on return types (many now concrete record types like
`ADef`, `AModule`, `ALetBind` instead of `object`). But parameter types are still
almost all `object` — the unification errors are still preventing type propagation
into function parameters. 1,066 errors remain to diagnose.

### Example: what Stage 0 produces vs Stage 1

```
Stage 0:  public static AExpr desugar_expr(Expr node)
Stage 1:  public static T1432 desugar_expr<T1432>(object node)
```

Both param type (`Expr` vs `object`) and return type (`AExpr` vs `T1432`) still differ.

---

## Remaining Gaps

### Gap #1: Type Inference (PARTIALLY FIXED — needs more work)

Still 1,066 unification errors. The remaining failures likely involve:
- Pattern match scrutinee types not propagating into branches
- `when` expression branch type unification
- Higher-order function types (passing functions as args to `map-list` etc.)
- Constructor application types

**Files:** `Codex.Codex/Types/TypeChecker.codex`, `Codex.Codex/Types/Unifier.codex`
**Reference:** `src/Codex.Types/TypeChecker.cs`, `src/Codex.Types/Unifier.cs`

### Gap #2: Let Expression Emission — `is var _` vs Lambda

`CSharpEmitter.codex` line 144 emits invalid C# pattern. Needs lambda form.
**File:** `Codex.Codex/Emit/CSharpEmitter.codex`

### Gap #3: String Concatenation — `+` vs `string.Concat`

**File:** `Codex.Codex/Emit/CSharpEmitter.codex`

### Gap #4: Curried vs Multi-Arg Calls

**File:** `Codex.Codex/Emit/CSharpEmitter.codex`

---

## Verification Commands

```sh
dotnet build Codex.sln              # Stage 0 clean
dotnet test Codex.sln               # Tests pass
codex build Codex.Codex             # Regenerate out/Codex.Codex.cs (Step 0c)
# copy out/Codex.Codex.cs → tools/Codex.Bootstrap/CodexLib.g.cs
dotnet build Codex.sln              # Rebuild with new Stage 1
dotnet run --project Codex.Bootstrap # Produce stage1-output.cs (Step 1b)
# diff/analyze out/Codex.Codex.cs vs stage1-output.cs
