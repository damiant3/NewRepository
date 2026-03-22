# Current Plan

**Date**: 2026-03-22 (verified via system clock)

---

## Status

**Peak I (Self-Hosting) achieved.** The Codex compiler compiles itself. Fixed point proven.
**Camp II-A (IL Backend) summited.** Standalone `.exe` emission via IL, no C# compiler needed.
**Camp II-B (Native Codegen) summited.** RISC-V native backend: encoder, ELF writer, codegen,
bare metal target with UART MMIO. 13 RISC-V tests + 5 QEMU execution tests.

The C# bootstrap compiler is locked. All forward development happens in `.codex` source.
The RISC-V bare metal proof-of-concept demonstrated Codex → native binary → hardware
with zero runtime. Camp II-C (self-hosted native build chain) is deferred — the proof
exists, the ecosystem comes first.

### Snapshot

| Metric | Value |
|--------|-------|
| Self-hosted compiler | 26 files, ~4,900 lines |
| Prelude | 11 modules, ~1,200 lines |
| Backends | 12 transpilation + IL + RISC-V native + RISC-V bare metal |
| Tests | 854+ passing |
| Type debt | 0 |
| Fixed point | Proven (Stage 1 = Stage 3 at 255,344 chars) |
| Reference compiler | 🔒 Locked |
| Native targets | RISC-V 64 (Linux user + bare metal) |

---

## Completed Work (this cycle)

### Camp II-B — RISC-V Native Backend ✅ (2026-03-21)

- `Codex.Emit.RiscV`: RiscVEncoder (RV64IM instruction encoding), ElfWriter (ELF64 +
  flat binary), RiscVCodeGen (IR→machine code), RiscVEmitter (IAssemblyEmitter)
- Linux userspace: direct syscalls, no libc
- Bare metal: UART MMIO at 0x10000000, flat binary, J trampoline at byte 0
- Bugs found and fixed: ELF-at-byte-0, `lui` sign-extension on RV64, QEMU serial routing
- MCP tool name validation tests added (prevents spec violations at build time)

### Previously Completed

- P1 — Self-Hosted Builtin Expansion ✅
- P2 — File Input & Stage 1 Verification ✅
- R6 — IL Native Executable Bootstrap ✅

---

## Active Work

### V1 — Repository Views (Starting Now)

**The keystone feature.** Views are consistent selections of facts from the repository.
They replace branches, enable proof-carrying packages, and become the build system.

The FactStore already has: content-addressed facts, Definition/Supersession/Proposal/
Verdict/Trust kinds, consensus checking, single implicit view. What's missing:

#### Phase 1: Named Views
- Multiple named views (not just one `view.json`)
- View CRUD: create, list, switch, delete
- CLI integration: `codex view create <name>`, `codex view list`, `codex view use <name>`

#### Phase 2: View Consistency
- Type-check all definitions in a view together
- A view that doesn't type-check cannot be published
- This is the killer feature: the compiler is the gatekeeper

#### Phase 3: View Composition
- `base + override`: a view with one definition replaced
- `view-a ∪ view-b`: merge (fails on conflict)
- `view | filter`: restrict to certain modules

#### Phase 4: View-Aware Compilation
- `codex build --view <name>` compiles from a view, not from files
- The view IS the build manifest — no separate project files

### R2b — Stdlib Layer 4 (Effects)

Formalize effect definitions in `.codex` source: `Console`, `FileSystem`, `State`,
`Time`, `Random`. Currently hard-coded in the type environment. ~50 lines.
This enables capability enforcement — a function declares what effects it needs,
the type system enforces it.

---

## Forward Direction

### Near Term (this week)
| Task | What | Depends on |
|------|------|------------|
| V1 | Repository Views (named, consistent, composable) | Nothing — ready now |
| R2b-1 | Formalize effects in `.codex` | Nothing — ready now |

### Medium Term
- **V4 — Proof-carrying facts**: every published fact carries its proofs, views verify them
- **Linear resource protocol**: hardware handles as linear values, type-enforced lifecycle
- **Camp II-C**: self-hosted native build chain on RISC-V (deferred, proof exists)

### Long Term
- **V2 — Narration layer**: prose-aware compilation
- **V3 — Repository federation**: multi-repo sync, cross-repo trust
- **V5 — Intelligence layer**: AI agents as first-class participants
- **V6 — Trust lattice**: vouching with degrees, trust-ranked search

---

## Process

- **Reference compiler is LOCKED.** See `docs/REFERENCE-COMPILER-LOCK.md`.
- **Stdlib design**: `docs/Designs/STDLIB-AND-CONCURRENCY.md`
- **Agent toolkit**: `tools/codex-agent/` — peek, snap, build, test, handoff, doctor
- **MCP server**: `tools/Codex.Mcp/` — compiler-as-a-tool for agents
- **Principles**: `docs/10-PRINCIPLES.md` — unchanged, still governing.
- **Two-agent workflow**: Windows (Copilot/VS) builds + pushes, Linux (Claude/sandbox) tests + reviews. Git is the coordination protocol.
