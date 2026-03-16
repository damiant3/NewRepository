# Bootstrap Status — Step 1b Complete

**Date:** March 16, 2026
**State:** Stage 1 output (`stage1-output.cs`) successfully generated. Not yet at fixed point.

---

## What Just Happened

Ran `Codex.Bootstrap` which feeds the 21 `.codex` source files through the
Stage 1 compiler (the `Codex_Codex_Codex` class built from `out/Codex.Codex.cs`).
It successfully produced `Codex.Codex/stage1-output.cs`.

---

## Gap #1 Fix Iteration — Round 2 (TypeChecker.codex + Unifier.codex)

### Round 1 (6 fixes)

1. **infer-name**: Instantiate `ForAllTy` wrappers with fresh vars (polymorphic builtins)
2. **AFieldAccess/ARecordExpr**: Return fresh vars / `ConstructedTy` instead of `ErrorTy`
3. **resolve-type-expr for AAppType**: Handle `List` + type args instead of discarding them
4. **check-def**: Thread module env through so defs can see other defs' types
5. **unify-structural**: Handle `ConstructedTy`, `SumTy`, `RecordTy`, `ForAllTy`, cross-matching
6. **deep-resolve**: Recurse into `ConstructedTy`, `ForAllTy`, `SumTy`, `RecordTy`

### Round 2 (3 fixes)

7. **register-type-defs**: Register variant constructors and record types into the type
   environment before checking definitions. Each variant constructor gets a function
   type `Field1 -> Field2 -> ... -> VariantType`. Without this, all 196 constructor
   applications in the `.codex` source failed type checking.
8. **bind-pattern for ACtorPat**: Constructor patterns now look up the constructor type
   from the env, instantiate it, and decompose the function type to bind sub-pattern
   variables with correct field types. Also threads `UnificationState` through
   `bind-pattern` (new `PatBindResult` record).
9. **check-module**: Now calls `register-type-defs` on `mod.type-defs` before
   `register-all-defs`, mirroring the C# `RegisterTypeDefinitions` call.

### Results

| Metric | Original | After R1 | After R2 | Stage 0 target |
|--------|---------|---------|---------|----------------|
| Unification errors | 1,864 | 1,066 | **203** | 0 |
| `object` type refs | 1,105 | 1,091 | 1,222 | 5 |
| `string` concrete | 87 | 81 | 82 | 477 |
| `long` concrete | 21 | 25 | 25 | 292 |
| `Func<` types | 2 | 2 | 2 | 636 |
| Defs / type-defs | 332 / 73 | 340 / 73 | **346 / 74** | 342 / 73 |

**Verdict:** Unification errors dropped 89% (1,864 → 203). Return types are now
concrete for most functions (e.g. `AExpr`, `ALetBind`, `LiteralKind`, `BinaryOp`).
Parameter types remain mostly `object` — this is expected because the remaining
203 errors prevent constraint propagation into function parameters. The `object`
count rose slightly because more functions are now generated (346 vs 332).

### Example improvement

```
Round 1:  public static T1432 desugar_expr<T1432>(object node)
Round 2:  public static AExpr desugar_expr(object node)
```

Return type now correct (`AExpr`). Param type still `object`.

---

## Remaining Gaps

### Gap #1: Type Inference (203 errors remain)

The remaining 203 unification errors likely involve:
- Missing type annotation propagation (annotated types aren't generalized/instantiated
  in the two-pass `register-all-defs` / `check-all-defs` flow the way the C# version does)
- `string`/`long` types showing as `object` because `Text`/`Integer` builtins don't
  unify through complex patterns
- Higher-order function types through `map-list`, `fold-list` etc.

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
