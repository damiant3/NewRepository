# Bootstrap Status — Step 1b Complete

**Date:** March 16, 2026
**State:** Stage 1 output (`stage1-output.cs`) successfully generated. Not yet at fixed point.

---

## What Just Happened

Ran `Codex.Bootstrap` which feeds the 21 `.codex` source files through the
Stage 1 compiler (the `Codex_Codex_Codex` class built from `out/Codex.Codex.cs`).
It successfully produced `Codex.Codex/stage1-output.cs` (112,396 chars, 1053 lines).

The bootstrap log reports:
- **332 defs, 73 type-defs** — matches Stage 0 ✅
- **1864 unification errors** — Stage 1 type checker is losing type info ❌

---

## Diff Summary: Stage 0 vs Stage 1 Output

| Metric                  | Stage 0 (out/Codex.Codex.cs) | Stage 1 (stage1-output.cs) | Verdict       |
|-------------------------|-----------------------------:|---------------------------:|---------------|
| Lines                   | 3,758                        | 1,053                      | Formatting    |
| Function count          | 334                          | 335                        | ~Match        |
| sealed record types     | 260                          | 260                        | ✅ Match       |
| `object` type refs      | 1                            | 1,105                      | ❌ **Gap #1**  |
| `string`/`long` concrete| 481 / 250                    | 87 / 21                    | ❌ **Gap #1**  |
| `Func<` (lambda lets)   | 547                          | 2                          | ❌ **Gap #2**  |
| `is var _ ?` pattern    | 1 (string literal)           | 321                        | ❌ **Gap #2**  |
| `string.Concat`         | 180                          | 0                          | ❌ **Gap #3**  |
| Curried `)(` calls      | 1,037                        | 1,244                      | ❌ **Gap #4**  |

---

## The 4 Gaps (in priority order)

### Gap #1: Type Inference — 1864 Unification Errors

**Symptom:** Almost all function params are `object` instead of `string`, `long`,
`List<Token>`, `Func<...>`, etc.

**Root cause:** The self-hosted type checker (`TypeChecker.codex` / `Unifier.codex`)
is failing to propagate type info. Stage 0's C# type checker (`src/Codex.Types/`)
works correctly on the same source.

**Why it matters:** This is the BIGGEST gap. If types are wrong, every other
comparison is meaningless — params, return types, generic suffixes, let bindings
all depend on correct type inference.

**Where to fix:** `Codex.Codex/Types/TypeChecker.codex` and `Codex.Codex/Types/Unifier.codex`.
Compare their logic against `src/Codex.Types/TypeChecker.cs` and `src/Codex.Types/Unifier.cs`.

---

### Gap #2: Let Expression Emission — `is var _` vs Lambda

**Symptom:** Stage 1 emits `((object x = expr) is var _ ? body : default)` (invalid C#).
Stage 0 emits `((Func<T,R>)((x) => body))(expr)` (valid C#).

**Root cause:** `CSharpEmitter.codex` line 144:
```
"((" ++ cs-type ty ++ " " ++ sanitize name ++ " = " ++ emit-expr val ++ ") is var _ ? " ++ emit-expr body ++ " : default)"
```

**Where to fix:** `Codex.Codex/Emit/CSharpEmitter.codex`, the `emit-let` function.
Change to emit the lambda pattern that Stage 0 uses.

---

### Gap #3: String Concatenation — `++` vs `string.Concat`

**Symptom:** Stage 0 emits `string.Concat(a, b)` for text append.
Stage 1 emits `(a + b)`.

**Root cause:** Stage 0's `CSharpEmitter.Expressions.cs` has special handling for
`AppendText` binary ops → `string.Concat`. Stage 1's `CSharpEmitter.codex` just
emits `"+"` for `IrAppendText`.

**Where to fix:** `Codex.Codex/Emit/CSharpEmitter.codex`, the `emit-bin-op` function
and/or `emit-expr` for `IrBinary` with text append ops.

---

### Gap #4: Curried vs Multi-Arg Calls

**Symptom:** Stage 1 emits `f(a)(b)(c)`. Stage 0 emits `f(a, b, c)`.

**Root cause:** Stage 0's `CSharpEmitter.Expressions.cs` has `EmitApplyGeneral` which
collects curried apply chains and emits them as multi-arg calls when it knows the
function's arity. Stage 1's `CSharpEmitter.codex` line 101 just does naive
`emit-expr f ++ "(" ++ emit-expr a ++ ")"`.

**Where to fix:** `Codex.Codex/Emit/CSharpEmitter.codex`, the `emit-expr` case for
`IrApply`. Needs arity-aware uncurrying logic, which requires either:
- Passing definition arity info into the emitter, or
- Collecting the apply chain and matching against known def names

---

## Recommended Fix Order

```
  #1  Fix type inference (Gaps resolve: #1, partially #2)
       ↓
  #2  Fix emit-let to use lambda pattern (Gap #2)
       ↓
  #3  Fix string.Concat emission (Gap #3)
       ↓
  #4  Add uncurrying to emit-apply (Gap #4)
       ↓
  Re-run Stage 0c → Stage 1a → Stage 1b → Diff
```

Gap #1 is the foundation — everything downstream depends on correct types.

---

## Files to Edit

| Gap | File to edit | Reference (Stage 0 C#) |
|-----|-------------|----------------------|
| #1  | `Codex.Codex/Types/TypeChecker.codex` | `src/Codex.Types/TypeChecker.cs` |
| #1  | `Codex.Codex/Types/Unifier.codex` | `src/Codex.Types/Unifier.cs` |
| #2  | `Codex.Codex/Emit/CSharpEmitter.codex` line 142-144 | `src/Codex.Emit.CSharp/CSharpEmitter.Expressions.cs` line 454-464 |
| #3  | `Codex.Codex/Emit/CSharpEmitter.codex` line 138 | `src/Codex.Emit.CSharp/CSharpEmitter.Expressions.cs` line 394-400 |
| #4  | `Codex.Codex/Emit/CSharpEmitter.codex` line 101 | `src/Codex.Emit.CSharp/CSharpEmitter.Expressions.cs` line 237-292 |

---

## How to Verify After Each Fix

```sh
dotnet build Codex.sln              # Stage 0 still clean
dotnet test Codex.sln               # Tests still pass
codex build Codex.Codex             # Regenerate out/Codex.Codex.cs (Step 0c)
dotnet build Codex.Codex            # Stage 1 compiles (Step 1a)
dotnet run --project Codex.Bootstrap # Produce stage1-output.cs (Step 1b)
diff out/Codex.Codex.cs stage1-output.cs  # Check progress
```
