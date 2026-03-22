# Current Plan

**Date**: 2026-03-22 (verified via system clock)

---

## Status

**Peak I (Self-Hosting) achieved.** The Codex compiler compiles itself. Fixed point proven.
**Camp II-A (IL Backend) summited.** Standalone `.exe` emission via IL, no C# compiler needed.
**Camp II-B (Native Codegen) summited.** RISC-V native + WASM backends. Three binary targets.
**V1 (Repository Views) complete.** Named views, consistency, composition, view-aware build.
**R2b (Effects Formalized) complete.** Five effects as `.codex` source, loaded by parser.
**Camp III-B (Capability System) begun.** CapabilityChecker extracts + enforces effect grants.

The C# bootstrap compiler is locked. All forward development happens in `.codex` source.

### Snapshot

| Metric | Value |
|--------|-------|
| Self-hosted compiler | 26 files, ~4,900 lines |
| Prelude | 16 modules, ~1,250 lines (11 type + 5 effect) |
| Backends | 12 transpilation + IL + RISC-V native + RISC-V bare metal + WASM |
| Tests | 900+ passing |
| Type debt | 0 |
| Fixed point | Proven (Stage 1 = Stage 3 at 255,344 chars) |
| Reference compiler | 🔒 Locked |
| Binary targets | RISC-V 64 (Linux user + bare metal), WASM/WASI |

---

## Completed Work (this cycle — 2026-03-22)

### V1 — Repository Views ✅ COMPLETE

Four phases delivered in one day:
- **Phase 1**: Named views — CRUD, legacy bridge, name validation, existence guards (27 tests)
- **Phase 2**: View consistency — type-check all definitions in a view together (5 tests)
- **Phase 3**: View composition — Override, Merge (with conflict detection), Filter (14 tests)
- **Phase 4**: View-aware compilation — `codex build --view <n>` (the view IS the build manifest)

69 total ViewTests. The repository model is now a working build system.

### R2b — Formalize Effects ✅

Effect definitions moved from hard-coded TypeEnvironment to parsed `.codex` source:
`Console`, `FileSystem`, `State`, `Time`, `Random` (5 prelude files).
`BuiltinEffects.Load()` parses once, caches forever. 8 new prelude tests.

### Camp III-B — Capability System (Phase 1) ✅

`CapabilityChecker`: post-type-check pass that extracts effect annotations
and optionally enforces capability grants. `CDX4001` diagnostic when a
required capability is missing. Wired into all compile pipelines.
`CapabilityReport` carried on `IRCompilationResult`. 9 tests.

### WASM Backend ✅

**Phase 1**: Direct bytecode emission (no Cranelift), WASI fd_write, bump allocator,
length-prefixed strings, runtime helpers (print i64/bool). 10 tests.

**Phase 2**: String equality (byte-by-byte with pointer fast path), text builtins
(text-to-integer, integer-to-text, char-at, substring, negate), f64.neg opcode.
13 new tests (23 total WASM, all wasmtime-verified).

### Camp II-B — RISC-V Native Backend ✅ (2026-03-21)

RiscVEncoder, ElfWriter, RiscVCodeGen, bare metal UART. 13 + 5 QEMU tests.

### Previously Completed

- P1 — Self-Hosted Builtin Expansion ✅
- P2 — File Input & Stage 1 Verification ✅
- R6 — IL Native Executable Bootstrap ✅

---

## Active Work

### Camp III-B — Capability System (Phase 2) ← **IN REVIEW**

CapabilityChecker is on `linux/camp3b-capability-checker`, awaiting merge.
Next: wire `--capabilities Console,FileSystem` flag into CLI, enforcement on
`codex build` and `codex run`.

---

## Forward Direction — Next Rocks to Climb

### Ready Now
| Task | What | Why |
|------|------|-----|
| III-B Phase 2 | CLI `--capabilities` flag + enforcement | Completes the capability grant flow end-to-end |
| WASM Phase 3 | Records, sum types as tagged unions in linear memory | Unlocks real data structures in WASM |
| V2 | Narration layer — prose-aware compilation | `.codex` files that read as documents |
| V4 | Proof-carrying facts | Views verify proofs at composition time |

### Medium Term
- **Camp III-A**: Linear allocator — region-based, type-driven deallocation
- **Camp III-C**: Structured concurrency — `par`, `race`, work-stealing
- **Camp II-C**: Self-hosted native build chain on RISC-V (deferred, proof exists)
- **V3**: Repository federation — multi-repo sync, cross-repo trust

### Long Term
- **V5 — Intelligence layer**: AI agents as first-class participants
- **V6 — Trust lattice**: vouching with degrees, trust-ranked search
- **Peak IV — Codex.OS**: The summit

---

## Process

- **Reference compiler is LOCKED.** See `docs/REFERENCE-COMPILER-LOCK.md`.
- **Stdlib design**: `docs/Designs/STDLIB-AND-CONCURRENCY.md`
- **Agent toolkit**: `tools/codex-agent/` — peek, snap, build, test, handoff, doctor
- **MCP server**: `tools/Codex.Mcp/` — compiler-as-a-tool for agents
- **Principles**: `docs/10-PRINCIPLES.md` — unchanged, still governing.
- **Two-agent workflow**: Windows (Copilot/VS) builds + pushes, Linux (Claude/sandbox) tests + reviews. Git is the coordination protocol.
