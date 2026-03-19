# Current Plan

**Date**: 2026-03-19 (verified via system clock)

---

## Where We Are

The Codex compiler is self-hosting with a proven fixed point, a proven quine disproof,
and zero type debt. The reference C# compiler is **locked** (`REFERENCE-COMPILER-LOCK.md`).
All language development now happens in `.codex` source.

The L1–L4 feature port to the self-hosted pipeline is **complete**: effects, modules,
prelude, and do-notation are all wired from parser through C# emission in `.codex` source.

### Snapshot

| Metric | Value |
|--------|-------|
| .codex source (compiler) | 26 files, 4,649 lines, 173K chars |
| .codex source (total) | 97 files (compiler + prelude + samples + tests) |
| Tests | **821** (all passing) |
| Backends | 12 (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage) |
| Bootstrap | Fixed point: Stage 1 = Stage 3 (227,301 chars) |
| Type debt | **0** (filtered), 5 raw (all legitimate/locked) |
| Prelude | 6 modules: Maybe, Result, Either, Pair, CCE, Hamt |
| Quine disproof | `samples/expr-calculator.codex` — 10/10 PASS |
| Ref compiler | 🔒 Locked at `6d8bb2c` on 2026-03-19 |

### Self-Hosted Pipeline Coverage

The `.codex` compiler now handles:

| Stage | Features ported |
|-------|----------------|
| Lexer | `effect` and `with` keywords |
| Parser | Effect declarations (`effect X where`), handler expressions (`with Effect body op (resume) = ...`), multi-param handler clauses |
| Desugarer | `HandleExpr`, `HandleClause`, `EffectDef`, `EffectOpDef`, effect-defs in Document/AModule |
| Types | `EffectfulTy` in CodexType |
| IR | `IrHandle`, `IRHandleClause` |
| Lowering | `lower-handle`, `lower-handle-clauses` |
| Emitter | `emit-handle`, `emit-handle-clauses`, `EffectfulTy` case in `cs-type`, typed do-blocks (Action for void, Func<T> otherwise) |

---

## What's Next

### Near Term: Make the Language Practical

| # | Task | Status | What |
|---|------|--------|------|
| E1 | **Exit criterion: real programs** | 🔶 Started | `expr-calculator.codex` (10/10 PASS) proves the compiler works. Next: something larger that exercises imports, prelude types, and effects together. |
| L7 | **Better error messages** | 🔶 Ongoing | "Did you mean X?" ✅. Readable type formatting ✅. Related spans ✅. Remaining: error recovery, source location accuracy in self-hosted pipeline. |
| R5 | **Build system** | 🔶 Started | `CodexProject` model + `LoadProjectFile` exist. Remaining: wire into `codex build`, multi-file dependency resolution. |
| P1 | **Self-hosted built-in expansion** | ⬜ Critical | The self-hosted emitter calls built-ins by name (`text_length`, `print_line`) instead of inlining them (`.Length`, `Console.WriteLine`). Stage 1 output needs a runtime shim or inline expansion to run standalone. Blocks R6. |

### Medium Term: Library & Runtime

| # | Task | Status | What |
|---|------|--------|------|
| R2 | **Standard library** | 🔶 Started | First modules: CCE (character encoding, 353 lines), Hamt (persistent map, 271 lines). Remaining: collections, string utilities, IO abstractions. |
| R3 | **FFI / host interop** | ⬜ | Call .NET/JS/C APIs from Codex. Per-backend with common interface. |
| R4 | **Package management** | ⬜ | Repository-based package resolution. `codex add <package>`. |
| R6 | **Native executable bootstrap** | ⬜ | Compile Codex compiler to native `.exe` via IL emitter. Depends on P1 + R2. |

**Exit criterion**: `codex build myproject/ --target il` produces a runnable `.exe`
with standard library support, no C# toolchain needed.

### Long Term: The Vision

| # | Task | What |
|---|------|------|
| V1 | **Views** | First-class consistent selections of facts from the repository. |
| V2 | **The Narration layer** | Prose-aware compilation where English text is load-bearing. |
| V3 | **Repository federation** | Multi-repository sync, cross-repo trust and identity. |
| V4 | **Proof-carrying packages** | Every published fact carries its proofs. Verified on import. |
| V5 | **Intelligence layer** | AI agents as first-class participants. Dashboard is step one. |
| V6 | **Trust lattice** | Vouching with degrees, trust-ranked search across contributors. |
| V7 | **Type-level function reduction** | Proof steps that unfold function definitions. |

---

## Recommended Sequence

```
Now ──→ P1 (built-in expansion) + E1 (real program) + R5 (build system)
            │
            ├──→ R2 (stdlib: expand CCE, Hamt, add collections)
            │       │
            │       └──→ R3 (FFI) ──→ R6 (native bootstrap via IL)
            │
            └──→ R4 (packages) ──→ V1-V7 (the vision)
```

**P1 is the critical path.** Until the self-hosted emitter can inline built-ins,
Stage 1 output requires a runtime shim to execute.

---

## Process Notes

- **Reference compiler is LOCKED.** New features go in `.codex` source only. See `REFERENCE-COMPILER-LOCK.md`.
- **Agent toolkit**: `tools/agent/` — PowerShell + Bash: `peek`, `fstat`, `sdiff`, `trun`, `gstat`.
- **Cognitive dashboard**: `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux).
- **Session init**: `bash tools/linux-session-init.sh` for fresh Linux sessions.
- **Dates**: Always `checkdate()` before writing dates. Never trust training data.
- **Commits**: Working branches (`windows/<topic>`, `linux/<topic>`), review before merge.
- **Archived history**: `docs/OldStatus/` contains M0–M13 iteration handoffs.
