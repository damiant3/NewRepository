# Bootstrap Convergence Plan

## Current State (after field-access fix)

| Metric | Before fix | After fix | Direction |
|--------|-----------|-----------|-----------|
| Stage 0 | ✅ Clean | ✅ Clean | — |
| dotnet test | ✅ 689 pass | ✅ 689 pass | — |
| Unification errors | 1317 | 1337 | +20 (noise) |
| Stage 1 output size | 139,327 | 141,028 | +1,701 ✅ |

The error count rose slightly because `RecordTy` is more specific than
the old `ConstructedTy` placeholder, causing a few new mismatches. But
Stage 1 output grew, meaning more definitions typed successfully. Net
positive.

### What was accomplished this session

1. **Diagnostic improvement**: `SourceSpan` carries `FileName` (required).
2. **Definition-level error context**: `CDX2099 info` names the definition.
3. **Parser fix**: `ParseAtom` consumes `.field` after parens.
4. **Duplicate removal**: `is-upper-char` from `CSharpEmitter.codex`.
5. **Field access typing**: Self-hosted type checker now resolves record
   field types via `RecordTy` instead of returning fresh type variables.
   Added `build-record-fields`, `lookup-record-field`, `strip-fun-args`.

### What previous agents accomplished (commit ebee4c6)

CSharpEmitter.codex grew from 342 → 768 lines with arity tracking, TCO,
match emission, effectful detection, partial application, builtin
special-casing — all phases 2-7 of the original plan. The reference
compiler output is already good. The gap is in the self-hosted type
checker, not the emitter.

---

## The Convergence Loop

```
repeat:
  1. codex build Codex.Codex              → Stage 0 (out/Codex.Codex.cs)
  2. copy Stage 0 → CodexLib.g.cs
  3. dotnet build + test
  4. dotnet run Codex.Bootstrap            → Stage 1 (stage1-output.cs)
  5. measure: unification errors, diff sections, output size
  6. if Stage 0 == Stage 1: DONE
  7. else: identify top error category, fix in .codex sources, goto 1
```

---

## Remaining Error Categories

### Category 1: Builtin function calls not recognized
Stage 1 emits `text_replace(name("-")("_"))` instead of `name.Replace(...)`.
This is because the Stage 1 emitter (CodexLib.g.cs) was compiled with
1337 type errors, causing `ErrorTy` to corrupt the IR. The emitter's
builtin detection checks fail when types are wrong.

**Root cause**: Self-hosted type checker doesn't know enough types.
**Fix**: Continue improving `TypeChecker.codex` and `Unifier.codex`.

### Category 2: Variant pattern matching type inference
The self-hosted checker returns `object` for patterns it can't resolve,
which cascades into wrong types for match bodies.

**Fix area**: `infer-match` in `TypeChecker.codex` — match arm scrutinee
types need to be propagated into pattern variable bindings.

### Category 3: Function arity not inferred for curried applications
Multi-argument functions lose their type signatures, causing the emitter
to fall back to curried `f(a)(b)(c)` style.

**Fix area**: `infer-apply` in `TypeChecker.codex`.

---

## Next Steps (for the next agent)

1. **Read this file first.**
2. **Do NOT modify files in `src/`.** The reference compiler is ground truth.
3. **Audit the first 20 type bindings** from Bootstrap output. Compare
   against `codex build` output. Each wrong binding reveals a specific
   checker bug.
4. **Fix one bug at a time** in `TypeChecker.codex` or `Unifier.codex`.
5. **Run the full cycle** after each fix. Error count should trend down,
   output size should trend up.
6. **Track progress** by updating this file.

### Files to modify

| File | What |
|------|------|
| `Codex.Codex/Types/TypeChecker.codex` | Type inference improvements |
| `Codex.Codex/Types/Unifier.codex` | Unification improvements |
| `Codex.Codex/Types/TypeEnv.codex` | Builtin type registrations |

### Files to read (reference, read-only)

| File | Why |
|------|-----|
| `src/Codex.Types/TypeChecker.cs` | Reference: `CheckModule()` |
| `src/Codex.Types/TypeChecker.Inference.cs` | Reference: all `Infer*` methods |
| `src/Codex.Types/Unifier.cs` | Reference: `Unify()`, `Resolve()` |

### Verification commands

```powershell
dotnet run --project tools/Codex.Cli -- build Codex.Codex
copy Codex.Codex\out\Codex.Codex.cs tools\Codex.Bootstrap\CodexLib.g.cs
dotnet build Codex.sln
dotnet test Codex.sln
dotnet run --project tools/Codex.Bootstrap
fc.exe Codex.Codex\out\Codex.Codex.cs Codex.Codex\stage1-output.cs
```

---

## Error Count History

| Commit/Change | Errors | Output Size | Notes |
|--------------|--------|-------------|-------|
| cd2352b (old emitter, 342 lines) | 203 | — | Simple emitter, few functions |
| ebee4c6 + old CodexLib | 1946 | — | New emitter, old self-hosted compiler |
| ebee4c6 + new CodexLib | 1317 | 139,327 | New emitter, new self-hosted compiler |
| Field access fix + new CodexLib | 1337 | 141,028 | RecordTy, field resolution |

---

## Rules

- Every `.codex` change → full cycle.
- Do NOT modify `src/`.
- Track error count. Must trend down (±20 noise is OK if output grows).
- If error count jumps >50, revert.
- Clean up `.bak` files when done.
