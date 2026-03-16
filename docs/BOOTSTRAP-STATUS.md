# Bootstrap Status — Step 1b Complete

**Date:** March 16, 2026
**State:** Stage 1 output (`stage1-output.cs`) successfully generated. Not yet at fixed point.
**Verified:** Build ✅, 689 tests ✅, Stage 0 regen consistent ✅, Bootstrap produces output ✅

---

## Gap #1 Fix Iteration (TypeChecker.codex + Unifier.codex)

6 fixes were applied to the self-hosted type checker and unifier:

1. **infer-name**: Instantiate `ForAllTy` wrappers with fresh vars (polymorphic builtins)
2. **AFieldAccess/ARecordExpr**: Return fresh vars / `ConstructedTy` instead of `ErrorTy`
3. **resolve-type-expr for AAppType**: Handle `List` + type args instead of discarding them
4. **check-def**: Thread module env through so defs can see other defs' types
5. **unify-structural**: Handle `ConstructedTy`, `SumTy`, `RecordTy`, `ForAllTy`, cross-matching
6. **deep-resolve**: Recurse into `ConstructedTy`, `ForAllTy`, `SumTy`, `RecordTy`

### Verified Results

| Metric | Original | After round 1 | After round 2 (fresh regen) | Stage 0 target |
|--------|----------|---------------|----------------------------|----------------|
| Unification errors | 1,864 | 1,066 | **203** | 0 |
| `object` type refs | 1,105 | 1,091 | 1,113 | 1 |
| `string` concrete | 87 | 81 | 82 | 477 |
| `long` concrete | 21 | 25 | 25 | 292 |
| `Func<` types | 2 | 2 | 2 | 636 |
| `is var _` pattern | 321 | 330 | 346 | 1 (string lit) |
| `string.Concat` | 0 | 0 | 0 | 180 |
| sealed record types | 260 | 260 | 261 | 261 |
| Function count | 335 | 343 | 349 | 348 |
| Defs / type-defs | 332/73 | 340/73 | 346/74 | — |

**Key win:** Return types are now mostly concrete. The bootstrap log shows:
```
desugar-expr : AExpr       (was T334)
classify-literal : LiteralKind  (was object)
desugar-bin-op : BinaryOp  (was object)
desugar-pattern : APat     (was object)
desugar-type-expr : ATypeExpr  (was T1432)
```

**Remaining issue:** Parameter types are still all `object`. 203 unification errors
remain, likely involving higher-order types, match branch inference, and polymorphic
function instantiation at call sites.

---

## Remaining Gaps

### Gap #1: Type Inference (PARTIALLY FIXED — 203 errors remain)

Parameter types still `object`. `map-list` return still `List<T582>` (unresolved).
Remaining errors likely in:
- Higher-order function calls (passing functions to `map-list`, `fold-list`)
- Pattern match type propagation in some edge cases
- Polymorphic instantiation at call sites

**Files:** `Codex.Codex/Types/TypeChecker.codex`, `Codex.Codex/Types/Unifier.codex`
**Reference:** `src/Codex.Types/TypeChecker.cs`, `src/Codex.Types/Unifier.cs`

### Gap #2: Let Expression Emission — `is var _` vs Lambda

Stage 1 emits `((object x = expr) is var _ ? body : default)` (invalid C#).
Stage 0 emits `((Func<T,R>)((x) => body))(expr)` (valid C#).
**File:** `Codex.Codex/Emit/CSharpEmitter.codex` line 142-144

### Gap #3: String Concatenation — `+` vs `string.Concat`

**File:** `Codex.Codex/Emit/CSharpEmitter.codex`

### Gap #4: Curried vs Multi-Arg Calls

Stage 1: `f(a)(b)(c)`. Stage 0: `f(a, b, c)`.
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
```

## Verification Notes

- After fresh Stage 0 regeneration, all 260 function bodies are byte-identical
  to the committed version. Only type declaration ordering differs (cosmetic,
  due to file discovery order). No functional divergence.
- The `out/Codex.Codex.cs` and `CodexLib.g.cs` should always be regenerated
  together after `.codex` changes to stay in sync.
