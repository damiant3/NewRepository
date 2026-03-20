# Current Plan

**Date**: 2026-03-19 (verified via system clock)

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
| Tests | 843 passing, 2 skipped |
| Type debt | 0 |
| Fixed point | Proven |
| Reference compiler | 🔒 Locked |

---

## Active Work

### R6 — Native Executable Bootstrap

**Goal**: `codex build myproject/ --target il` produces a runnable `.exe` with standard
library support. No C# toolchain required.

**Status**: Unblocked. IL emitter has basic builtins (show, print-line, do-blocks,
generic sum boxing). Needs: full standard library emission, entry point generation,
assembly linking.

**Why this matters**: R6 is the second liberation. MM1 freed the language from being
*written* in C#. R6 frees it from *depending* on C# to run.

---

## Forward Direction

The project moves away from the C# codebase and toward a self-sustaining Codex ecosystem.

### Near Term
- **R6**: Native executable bootstrap (IL emitter → standalone `.exe`)
- **Stdlib hardening**: exercise the prelude with real programs, fill gaps

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
- **Agent toolkit**: `tools/agent/` — PowerShell + Bash: `peek`, `fstat`, `sdiff`, `trun`, `gstat`.
- **Cognitive dashboard**: `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux).
- **Principles**: `docs/10-PRINCIPLES.md` — unchanged, still governing.
