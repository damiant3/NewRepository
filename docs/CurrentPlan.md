# Current Plan

**Date**: 2026-03-22 (verified via system clock)

---

## Status

**Peak I (Self-Hosting) achieved.** The Codex compiler compiles itself. Fixed point proven.
**Camp II-A (IL Backend) summited.** Standalone `.exe` emission via IL, no C# compiler needed.
**Camp II-B (Native Codegen) summited.** RISC-V native + WASM backends. Three binary targets.
**V1 (Repository Views) complete.** Named views, consistency, composition, view-aware build.
**R2b (Effects Formalized) complete.** Five effects as `.codex` source, loaded by parser.
**Camp III-B (Capability System) complete.** CapabilityChecker + CLI `--capabilities` enforcement merged.
**Camp III-A (Linear Allocator) Phase 1 complete.** IRRegion node + WASM region-based allocator merged.

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

### Camp III-B — Capability System (Phase 2) ✅

CLI `--capabilities Console,FileSystem` flag wired into `codex build` and
`codex check`. Enforcement at compile time: `CDX4001` fires when a required
capability isn't granted. `PrintCapabilityReport` shows required capabilities
in build/check output. Merged from `linux/camp3b-capability-checker`.

### Camp III-A — Linear Allocator (Phase 1) ✅

`IRRegion` IR node wraps every definition body. WASM backend implements real
region-based allocation: push heap pointer on enter, restore on exit (bulk free).
Escape promotion skips heap-returning types (Phase 2). All 5 backends updated.
1 new WASM test (1000-iteration stability). Merged from `linux/camp3a-region-allocator`.

### WASM Backend ✅

**Phase 1**: Direct bytecode emission (no Cranelift), WASI fd_write, bump allocator,
length-prefixed strings, runtime helpers (print i64/bool). 10 tests.

**Phase 2**: String equality (byte-by-byte with pointer fast path), text builtins
(text-to-integer, integer-to-text, char-at, substring, negate), f64.neg opcode.
13 new tests (23 total WASM, all wasmtime-verified).

### Camp II-B — RISC-V Native Backend ✅ (2026-03-21)

RiscVEncoder, ElfWriter, RiscVCodeGen, bare metal UART. 13 + 5 QEMU tests.

### RISC-V Feature Parity ✅ (2026-03-22)

~1,000 lines of RISC-V machine code generation in 6 phases:
- **Phase 1**: Bump-alloc heap (S1 register, brk for Linux, fixed addr bare metal)
- **Phase 2**: Records (heap-alloc + field store/load at 8-byte offsets)
- **Phase 3**: Sum types (tagged unions `[tag:8B][fields...]`)
- **Phase 4**: Pattern matching (wildcard, variable, literal, constructor patterns)
- **Phase 5**: Text builtins (text-length, char-at, substring, to/from integer,
  show, string equality via __str_eq, concatenation via __str_concat)
- **Phase 7**: Region-based allocation (push/pop heap ptr, text escape)

Register allocator split: temps (T3-T6, recycled) vs locals (S2-S11, monotonic).
Equality bug fixed (slti→sltu+xori). 15 new tests (34 total RISC-V, all QEMU-verified).
Design: `docs/Designs/RISCV-PARITY.md`. Review: `docs/Reviews/RISCV-PARITY-PHASES1-4-REVIEW.md`.

### Previously Completed

- P1 — Self-Hosted Builtin Expansion ✅
- P2 — File Input & Stage 1 Verification ✅
- R6 — IL Native Executable Bootstrap ✅

---

### V2 — Narration Layer (CPL Implementation) ✅

All 6 CPL sentence forms implemented in one session (2026-03-22):
- **Form 1**: Type declarations (record/variant) — V1, extended with constraints
- **Form 2**: Constraint templates (`such that`, `where`, `provided that`)
- **Form 3**: Function templates (`To V (x : T) gives Y, failing if P`)
- **Form 4**: Proof assertions (`Claim:` / `Proof:` with CDX1105 validation)
- **Form 5**: Procedure steps (`first,`/`then,`/`finally,` with let/return/if)
- **Form 6**: Quantified statements (`for every`, `there exists`, `no`)

Also: prose-notation consistency checking (CDX1101/CDX1102), inline code
refs (backtick), inline type refs (PascalCase), transition markers (`We say:`).
44 prose template tests. Design: `docs/Designs/V2-NARRATION-LAYER.md`.

### IL Emitter — maxstack fix ✅

Fixed `InvalidProgramException` caused by hardcoded `maxStack=32`. Now scales
with `max(16, max(locals.Count, exprDepth) + 16)` using `EstimateStackDepth`
recursive IR walker. Found by dogfooding codex-agent.

### codex-agent — per-agent cognitive check ✅

`check cam` uses 800K budget (1M context), `check windows`/`check linux` use
60K. Agent name, label, and budget-appropriate load assessment in output.

---

## Active Work

### Camp II-C — Self-Hosted on RISC-V (in progress)

The self-hosted compiler (493 defs, 26 .codex files) compiles to a 223KB
RISC-V ELF. The binary starts correctly (brk heap allocation succeeds)
then hits a **null pointer dereference** (`si_addr=NULL`).

**What works**: all 40 QEMU-verified test programs (records, sum types,
pattern matching, lists, function pointers, text ops, string equality,
register spills). Simple → medium complexity programs all run correctly.

**What's broken**: the full compiler binary segfaults. The null deref is
a runtime logic bug — some code path in the 493-definition compiler
dereferences a pointer that was never initialized. Not a regalloc or
spill issue (those are fixed and tested).

**Next step**: bisect which .codex module triggers the crash. Either:
1. Compile subsets of modules → find the smallest set that crashes
2. Use `qemu-riscv64 -g 1234` + GDB to trace to the faulting instruction

**Design doc**: `docs/Designs/CAMP-IIC-SELF-HOSTED-RISCV.md`

Branches pending review:
- `windows/v2-fail-clause` — fail clauses + gives articles on function templates
- `windows/v2-constraint-templates` — CPL Form 2 constraints
- `windows/v2-claim-proof-templates` — CPL Form 4 claim/proof templates
- `windows/v2-procedure-steps` — CPL Form 5 procedure steps
- `windows/v2-quantified-statements` — CPL Form 6 quantified statements

---

## Forward Direction — Next Rocks to Climb

### Ready Now
| Task | What | Why |
|------|------|-----|
| ~~RISC-V parity~~ | ~~Records, sum types, pattern matching, text builtins on RISC-V~~ | ✅ Done (2026-03-22) |
| V4 | Proof-carrying facts | Views verify proofs at composition time |

### Medium Term
- **Camp III-C**: Structured concurrency — `par`, `race`, work-stealing
- **Camp II-C**: Self-hosted native build chain on RISC-V (deferred, proof exists)
- **V3**: Repository federation — multi-repo sync, cross-repo trust
- **Network + Process effects**: Extend capability system beyond Console/FileSystem
- **x86-64 / ARM64 backends**: Extend native codegen beyond RISC-V

### Long Term
- **V5 — Intelligence layer**: AI agents as first-class participants
- **V6 — Trust lattice**: vouching with degrees, trust-ranked search
- **Peak IV — Codex.OS**: The summit

---

## Process

- **Reference compiler is LOCKED.** See `docs/REFERENCE-COMPILER-LOCK.md`.
- **Stdlib design**: `docs/Designs/STDLIB-AND-CONCURRENCY.md`
- **RISC-V parity plan**: `docs/Designs/RISCV-PARITY.md`
- **V2 narration design**: `docs/Designs/V2-NARRATION-LAYER.md`
- **Agent toolkit**: `tools/codex-agent/` — peek, snap, build, test, handoff, doctor
- **MCP server**: `tools/Codex.Mcp/` — compiler-as-a-tool for agents
- **Principles**: `docs/10-PRINCIPLES.md` — unchanged, still governing.
- **Three-agent workflow**: Windows (Copilot/VS) builds + pushes, Linux (Claude/sandbox) tests + reviews, Cam (Claude Code CLI) fast iteration + parallel work. Git is the coordination protocol. Cam works from `D:\Projects\NewRepository-cam` worktree.
