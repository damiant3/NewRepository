# Current Plan

**Date**: 2026-03-20 (verified via system clock)

---

## Status

**Major Milestone 1 is achieved.** The Codex compiler is self-hosting. The C# bootstrap
compiler is locked. All forward development happens in `.codex` source.

The original design documents (01–09, Glossary) and the final pre-MM1 plan are archived
in `docs/MM1/`.

### Snapshot

| Metric | Value |
|--------|-------|
| Self-hosted compiler | 26 files, ~4,900 lines |
| Prelude | 11 modules, ~1,200 lines |
| Backends | 12 |
| Tests | 854 passing, 2 skipped |
| Type debt | 0 |
| Fixed point | Broken (re-verify needed after P1/P2 changes) |
| Reference compiler | 🔒 Locked (with `read-file` builtin override) |

---

## Completed Work

### P1 — Self-Hosted Builtin Expansion ✅

**Completed.** All 22 builtins are now inlined by the self-hosted emitter. `read-line`
was the last gap; added in the linux/p1-read-line branch.

### P2 — File Input & Stage 1 Verification ✅

**Completed.** The self-hosted compiler now accepts file input:

```
echo path/to/file.codex | dotnet run --project Codex.Codex
```

Changes:
- Added `read-file : Text -> [FileSystem] Text` builtin to the reference compiler
  (override of lock — authorized by user). Emits `File.ReadAllText(path)`.
- Updated `main.codex` to read a file path from stdin, read the file, compile it,
  and print the C# output.
- Fixed `emit-do` in the self-hosted emitter: `NothingTy`/`VoidTy` do-blocks now
  emit `Func<object>` with `return null;` instead of `Action` (which returns `void`
  and isn't assignable to `object`).
- Verified: `samples/stage1-test.codex` compiles through Stage 1 and the output
  runs correctly, printing `Hello, Codex!` and `49`.

---

## Active Work

### R2b — Stdlib Completion

The prelude shipped 11 modules (1,208 lines) covering Layers 1–3 of the design
(`docs/Designs/STDLIB-AND-CONCURRENCY.md`). One gap remains, plus several
hardening tasks surfaced by the audit.

#### Stdlib Audit Summary

| Layer | Module | Lines | Status |
|-------|--------|-------|--------|
| 1 — Core Types | Maybe | 28 | ✅ Complete |
| 1 — Core Types | Result | 28 | ✅ Complete |
| 1 — Core Types | Either | 28 | ✅ Complete |
| 1 — Core Types | Pair | 13 | ✅ Complete |
| 2 — Collections | List (ConsList) | 100 | ✅ Complete |
| 2 — Collections | Hamt | 271 | ✅ Complete |
| 2 — Collections | Set | 110 | ✅ Complete (Text-keyed only) |
| 2 — Collections | Queue | 51 | ✅ Complete |
| 3 — Text | CCE | 353 | ✅ Complete |
| 3 — Text | StringBuilder | 84 | ✅ Complete |
| 3 — Text | TextSearch | 142 | ✅ Complete |
| 4 — Effects | (formalized) | — | ❌ Not started |

#### Tasks

| # | Task | Effort | Priority | Why |
|---|------|--------|----------|-----|
| R2b-1 | **Formalize effects in `.codex`** | ~50 lines | High | Last R2 gap. Declare `Console`, `FileSystem`, `State`, `Time`, `Random` as effect definitions in `.codex` source instead of hard-coding in the type env. Makes the effect system self-documenting. |
| R2b-2 | **Add `hamt-map` / `hamt-filter`** | ~30 lines | Medium | Natural operations missing from an otherwise complete Hamt module. |
| R2b-3 | **Add `list-sort`** (built-in List) | ~40 lines | Medium | Merge sort on index-based list. Needed for any program that ranks or orders results. |
| R2b-4 | **Add `Number` math module** | ~60 lines | Medium | `abs`, `min`, `max`, `floor`, `ceil`, `round`. Currently no floating-point support beyond arithmetic operators. |
| R2b-5 | **Reconcile ConsList vs built-in List** | Design decision | Low | The prelude defines `ConsList a` (Cons/Nil) but the compiler and all samples use the built-in `List` with `list-length`/`list-at`. These don't interoperate. Either retire ConsList or make it canonical. |

#### Known Limitations (not blocking, noted for future)

- `Set` is `TextSet` only — generic sets need type classes or a hash/compare constraint.
- `Hamt` is `Text`-keyed only — same constraint.
- `StringBuilder` uses a list-of-parts model (`O(n)` join) — adequate for current use but
  not a true rope. Sufficient until profiling says otherwise.

---

### R6 — Native Executable Bootstrap ✅

**Completed.** The IL emitter (2,300 lines across 4 files) produces standalone `.exe`
assemblies with `runtimeconfig.json`. Supports: records, sum types, pattern matching,
generics, tail-call optimization, builtins (show, print-line, do-blocks), generic sum
boxing, and entry point generation.

```sh
dotnet run --project tools/Codex.Cli -- build samples/hello.codex --target il
# → outputs hello.exe + hello.runtimeconfig.json
dotnet hello.exe
```

Still depends on the .NET runtime (`dotnet` to execute), but the C# *compiler* toolchain
is no longer required. The `.exe` is pure IL, not C# source.

---

## Forward Direction

The project moves away from the C# codebase and toward a self-sustaining Codex ecosystem.

### Near Term

| Task | What | Depends on |
|------|------|------------|
| R2b | Stdlib completion (effects, hamt-map, sort, math) | Nothing — ready now |

### Medium Term
- **V1 — Views**: first-class consistent selections of facts from the repository
- **V2 — Narration layer**: prose-aware compilation where English text is load-bearing

### Long Term
- **V3 — Repository federation**: multi-repo sync, cross-repo trust and identity
- **V4 — Proof-carrying packages**: every published fact carries its proofs
- **V5 — Intelligence layer**: AI agents as first-class participants
- **V6 — Trust lattice**: vouching with degrees, trust-ranked search
- **V7 — Type-level function reduction**: proof steps that unfold function definitions

---

## Process

- **Reference compiler is LOCKED.** See `docs/REFERENCE-COMPILER-LOCK.md`.
- **Stdlib design**: `docs/Designs/STDLIB-AND-CONCURRENCY.md` — small core, 4 layers, concurrency via effects (V5+).
- **Agent toolkit**: `tools/agent/` — PowerShell + Bash: `peek`, `fstat`, `sdiff`, `trun`, `gstat`.
- **Cognitive dashboard**: `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux).
- **Principles**: `docs/10-PRINCIPLES.md` — unchanged, still governing.
