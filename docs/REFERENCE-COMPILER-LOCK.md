# Reference Compiler Lock

**Date**: 2026-03-19 (verified via system clock)
**Locked at commit**: `6d8bb2c`
**Locked by**: Copilot (Windows agent)

---

## What This Means

The C# reference compiler (`src/` projects) is **frozen**. All future language
development happens in Codex source (`.codex` files), compiled through the
self-hosted pipeline.

The reference compiler's sole purpose going forward is to serve as **Stage 0** —
the bootstrap compiler that builds the self-hosted compiler from `.codex` source.
It should not receive new features. Bug fixes to Stage 0 are permitted only when
they are necessary to compile the self-hosted compiler's `.codex` source.

---

## Bootstrap Chain

```
Stage 0:  C# reference compiler (src/) → compiles .codex → Codex.Codex.cs
Stage 1:  Codex.Codex.cs (compiled by dotnet) → compiles .codex → stage1-output.cs
Stage 2+: stage1-output.cs → compiles .codex → stage2-output.cs  (= stage1-output.cs)
```

**Fixed point**: Stage 1 output = Stage 2 output (byte-for-byte identical).

---

## Checksums at Lock Time

| File | SHA-256 | Size |
|------|---------|------|
| `Codex.Codex/out/Codex.Codex.cs` (Stage 0 output) | `3E2E7796D9CF7C6745099ABA9C513261BB079FA36BBFBCE699A134CE2FBCC319` | 295,507 chars |
| `Codex.Codex/stage1-output.cs` (Stage 1 = fixed point) | `F4E12D0F66682502435AB5308A268241A237026C643015B6E9DD8E64FCCC4BEE` | 231,568 chars |
| `Codex.Codex/stage3-output.cs` (Stage 3 = Stage 1) | `F4E12D0F66682502435AB5308A268241A237026C643015B6E9DD8E64FCCC4BEE` | 231,568 chars |

---

## What the Reference Compiler Supports (Frozen Feature Set)

- Algebraic types (sum types, record types)
- Pattern matching with exhaustiveness checking
- Bidirectional type inference with unification
- Effects: built-in (Console, State, FileSystem) + user-defined (`effect ... where`)
- Effect handlers (`with Effect expr` + `resume` continuations)
- Do-notation with typed returns (works with any effect)
- Linear types
- Dependent types (type-level arithmetic, proof obligations)
- Proofs (refl, sym, trans, cong, induction)
- Module system (import/export, visibility control)
- Standard prelude (Maybe, Result, Either, Pair)
- String interpolation (`#{expr}`)
- Literate programming (prose documents)
- 12 backends (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage)
- LSP server (diagnostics, hover, completion, go-to-def, semantic tokens)
- Content-addressed repository with collaboration protocol

---

## Verification at Lock Time

| Check | Result |
|-------|--------|
| `dotnet build Codex.sln` | ✅ 0 errors, 0 warnings |
| `dotnet test Codex.sln` | ✅ 810 tests passing |
| Bootstrap fixed point | ✅ Stage 1 = Stage 3 (231,568 chars) |
| Type debt | 7 `object` refs, 0 `_p0_` proxies |
| Exit criterion | ✅ `expr-calculator.codex` — 125-line recursive descent parser, 10/10 PASS |

---

## Rules Going Forward

1. **Do not add features to `src/` projects.** New language features are implemented in `.codex` source.
2. **Bug fixes to Stage 0 are permitted** only when the self-hosted compiler cannot compile due to a Stage 0 bug.
3. **Test projects (`tests/`) may still be modified** — they test the reference compiler which remains the build system.
4. **The CLI (`tools/Codex.Cli/`) may still be modified** — it's the driver, not the compiler.
5. **Regenerating Stage 0 output** (`codex build Codex.Codex/`) is permitted and expected as `.codex` source evolves.

---

## Lock Overrides

### Override 1: `char-code-at` builtin (2026-03-20)

**Authorized by**: User (project owner)
**Agent**: Claude (Opus 4.6, Linux)
**Justification**: Performance-critical. The self-hosted lexer was **800× slower** than the
reference compiler's lexer because `char-at` returns `Text` (heap-allocated string per
character). For a 163K-char input, this produces billions of bytes of garbage.

**Change**: Added `char-code-at : Text -> Integer -> Integer` as a new builtin.
Emits `((long)source[(int)index])` — zero allocation.

**Files modified** (4 files, ~10 lines total):
- `src/Codex.Types/TypeEnvironment.cs` — type binding
- `src/Codex.IR/Lowering.cs` — builtin type map
- `src/Codex.Emit.CSharp/CSharpEmitter.Expressions.cs` — emission + multi-arg set
- `src/Codex.Semantics/NameResolver.cs` — builtin name set

**Scope**: This is a new builtin, not a language feature. It does not change parsing,
type checking, or any existing behavior. No existing tests are affected (836 passing).

**Impact**: Enables the self-hosted lexer to use integer character codes on the hot path,
eliminating per-character string allocation. Projected: lexer 800× → ~5×, overall 28× → ~4×.
