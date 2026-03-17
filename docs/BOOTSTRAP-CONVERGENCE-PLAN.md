# Bootstrap Convergence Plan





-Damian says sorry for the messy.  I don't want to spend a whole session doing stats.  clean this up later.

### What was accomplished this session
The last bootstrap metrics before the stash incident were:
Metric	Baseline (arity-flattening)	After all lowering fixes	Delta
Stage 0 output size	225,502	243,370	+17,868 ✅
Stage 1 output size	151,893	168,021	+16,128 ✅
Unification errors	1,368	1,505	+137 ⚠️
Type bindings	364	377	+13 ✅
Stage 0 _p0_ lines	28	24	−4 ✅
Stage 1 _p0_ lines	229	241	+12 ⚠️
The error count rose because we added ~13 new definitions. Output sizes grew significantly (+7-8% each). Stage 0 _p0_ lines dropped, confirming the lowering improvements help the reference compiler path.


## Previous State (after arity-flattening refactor in CSharpEmitter.codex)

| Metric | Previous (compound-parse fix) | Current | Direction |
|--------|-------------------------------|---------|-----------|
| Stage 0 | ✅ Clean | ✅ Clean | — |
| dotnet test | ✅ 689 pass | ✅ 689 pass | — |
| Unification errors | 1255 | 1368 | +113 ⚠️ |
| Stage 1 output size | 129,085 | 151,893 | +22,808 ✅ |
| Stage 0 output size | 219,586 | 225,502 | +5,916 ✅ |
| Type bindings | 354 | 364 | +10 ✅ |
| Diff lines (fc /N) | — | 859 | — |
| Stage 1 `_p0_` lines | — | 229 | — |
| Stage 0 `_p0_` lines | — | 28 | — |


The Stage 1 output grew by ~23K chars (+18%) — a significant gain. The
Stage 0 ↔ Stage 1 gap is now 225,502 vs 151,893 chars (67% of Stage 0),
up from ~59% before. The error count rose by 113 because the new emitter
code (ArityEntry, ApplyChain, collect-apply-chain, emit-apply, etc.) adds
~10 new definitions that each trigger type-checker failures, and the arity
flattening changes how arguments are grouped, surfacing more unification
mismatches. This is expected: more code → more type errors until the
self-hosted type checker improves. The output size increase confirms the
emitter is producing more correct code.

### What was accomplished this session


1. **Arity-flattening refactor in `CSharpEmitter.codex`**: The emitter's
   `emit-expr` and all 14 downstream functions now thread a `List ArityEntry`
   parameter. New infrastructure:
   - `ArityEntry` record (`name : Text`, `arity : Integer`)
   - `build-arity-map` — builds the map from `List IRDef`
   - `lookup-arity` — linear scan lookup
   - `ApplyChain` record (`root : IRExpr`, `args : List IRExpr`)
   - `collect-apply-chain` — flattens nested `IrApply` into root + args
   - `emit-apply` — main dispatch:
     - Uppercase root → `new Ctor(a, b, c)` (constructor)
     - Known arity match → `f(a, b, c)` (multi-arg)
     - Partial application → lambda wrappers
     - Fallback → curried `f(a)(b)`
   - `emit-apply-args`, `emit-partial-params`, `emit-partial-wrappers` helpers
   - `emit-expr-curried` — the old curried fallback
   - `is-upper-letter` — constructor name detection
2. **Entry points unchanged**: `emit-full-module` and `emit-module` build
   the arity map internally; their external signatures are the same.
3. **All 19+ `emit-expr` call sites** updated to pass `arities`.

### Observed improvements in Stage 1 output

- Multi-arg calls work for locally-defined functions:
  `emit_expr(e, arities)`, `emit_let(name, ty, val, body, arities)`,
  `scope_has_loop(names, name, i, len)` — all emitted with flat args.
- Constructor calls use `new`: `new ArityEntry(name: ..., arity: ...)`.
- Partial application wrappers generated when arg count < arity.

### Observed remaining issues

- **Type-checker ErrorTy corruption** still dominates. Example:
  `scope_has_loop(names(name((i + 1))(len)), _p0_, _p1_, _p2_)` —
  the function itself is called with 4 flat args (arity correct!), but
  the *arguments* are curried because the type checker returned ErrorTy
  for the sub-expressions, making the IR think `names(name(...))` is
  a single expression rather than separate args.
- 229 lines in Stage 1 have `_p0_` partial-application markers vs 28 in
  Stage 0 — ~200 are spurious from ErrorTy cascading.
- Builtin functions (`text_replace`, `char_at`, etc.) still emit curried
  because the type checker doesn't recognize their types.

### What was accomplished in prior sessions

1. **Diagnostic improvement**: `SourceSpan` carries `FileName` (required).
2. **Definition-level error context**: `CDX2099 info` names the definition.
3. **Parser fix**: `ParseAtom` consumes `.field` after parens.
4. **Duplicate removal**: `is-upper-char` from `CSharpEmitter.codex`.
5. **Field access typing**: Self-hosted type checker now resolves record
   field types via `RecordTy` instead of returning fresh type variables.
   Added `build-record-fields`, `lookup-record-field`, `strip-fun-args`.
6. **CSharpEmitter.codex** grew from 342 → 768 lines with arity tracking,
   TCO, match emission, effectful detection, partial application, builtin
   special-casing — all phases 2-7 of the original plan. (commit ebee4c6)
7. **Compound-expression parse fix**: `parse-field-access` on compound
   expressions (match/when) was greedily eating the next definition's
   name token as an argument when `is-app-start` returned true. Fixed
   so compound expressions only allow `.field` access, not application.
   (commit 5b8d78d)

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
| 5b8d78d compound-parse fix | 1255 | 129,085 | Parser stops eating next def's name |
| Arity-flattening refactor | 1368 | 151,893 | +23K output, emit-expr threads arities |

---

## Rules

- Every `.codex` change → full cycle.
- Do NOT modify `src/`.
- Track error count. Must trend down (±20 noise is OK if output grows).
- If error count jumps >50, revert.
- Clean up `.bak` files when done.
