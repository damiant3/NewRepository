# Current Plan

**Date**: 2026-03-19 (verified via system clock)

---

## Where We Are

The Codex compiler is self-hosting with a proven fixed point, a proven quine disproof,
and zero type debt. The reference C# compiler is **locked** (`REFERENCE-COMPILER-LOCK.md`).
All language development now happens in `.codex` source.

The L1–L4 feature port to the self-hosted pipeline is **complete**: effects, modules,
prelude, and do-notation are all wired from parser through C# emission in `.codex` source.

**P1 (built-in expansion) is RESOLVED.** The root cause was a bootstrap prose-extraction
bug: `indent >= 4` in `ExtractCodeBlocks` silently dropped all type definitions and
function signatures after the project moved to 2-space indentation. The fix
(`indent >= 2`) restored 91 type definitions, eliminated 1,014 unification errors,
and recovered ~30 missing functions in Stage 1 output including `is_builtin_name`,
`emit_builtin`, and the entire effects/handler subsystem. Fixed point re-verified.

### Snapshot

| Metric | Value |
|--------|-------|
| .codex source (compiler) | 26 files, 4,649 lines, 173K chars |
| .codex source (total) | 97 files (compiler + prelude + samples + tests) |
| Tests | **836** (836 passing + 2 skipped) |
| Backends | 12 (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage) |
| Bootstrap | Fixed point: Stage 1 = Stage 2 (244,363 chars) |
| Type debt | **0** (filtered), 0 ErrorTy bindings |
| Prelude | 7 modules: Maybe, Result, Either, Pair, CCE, Hamt, List |
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
| Emitter | `emit-handle`, `emit-handle-clauses`, `EffectfulTy` case in `cs-type`, built-in inlining (`is-builtin-name`/`emit-builtin`), typed do-blocks |

---

## What's Next

### Completed (This Session)

| # | Task | Status | What |
|---|------|--------|------|
| P1 | **Self-hosted built-in expansion** | ✅ Done | Root cause: bootstrap `ExtractCodeBlocks` required `indent >= 4` but source uses 2-space indent. Fix: `indent >= 2`. Stage 1 now inlines all built-ins. Fixed point verified. |
| L7 | **Better error messages** | ✅ Done | "Did you mean X?" via `StringDistance.FindClosest` ✅. Readable type formatting ✅. Related spans ✅. Error cap at 20 ✅. ErrorType cascade suppression ✅. Parser error recovery with 18 tests (CDX1021–CDX1032) ✅. |
| R5 | **Build system** | ✅ Done | `codex build <dir>` compiles multi-file projects via `codex.project.json`. Dependency resolution ✅. Multi-target (`--targets cs,js,rust`) ✅. Incremental builds (`--incremental`) ✅. Verified: `codex build Codex.Codex` compiles 26 files successfully. |

### Near Term: Make the Language Practical

| # | Task | Status | What |
|---|------|--------|------|
| E1 | **Exit criterion: real programs** | 🔶 Started | `expr-calculator.codex` (10/10 PASS) proves the compiler works. Next: something larger that exercises imports, prelude types, and effects together. |

### Medium Term: Library & Runtime

| # | Task | Status | What |
|---|------|--------|------|
| R2 | **Standard library** | 🔶 Started | First modules: CCE (character encoding, 353 lines), Hamt (persistent map, 271 lines), List (cons-list, 98 lines). Remaining: string utilities, IO abstractions. |
| R3 | **FFI / host interop** | ⬜ | Call .NET/JS/C APIs from Codex. Per-backend with common interface. |
| R4 | **Package management** | ⬜ | Repository-based package resolution. `codex add <package>`. |
| R6 | **Native executable bootstrap** | ⬜ Unblocked | Compile Codex compiler to native `.exe` via IL emitter. P1 resolved; depends on R2. |

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
Now ──→ E1 (real program)
            │
            ├──→ R2 (stdlib: expand CCE, Hamt, add collections)
            │       │
            │       └──→ R3 (FFI) ──→ R6 (native bootstrap via IL)
            │
            └──→ R4 (packages) ──→ V1-V7 (the vision)
```

**R6 is now unblocked.** P1, L7, and R5 are resolved. The next critical path is
R2 (standard library) to provide the runtime functions needed for standalone executables.

---

## Recent Session: P1/L7/R5 Resolution (2026-03-19)

**Agent**: Claude (Opus 4.6, Linux, claude.ai)
**Branch**: `linux/p1-builtin-expansion` → merged to `master`

### Investigation Trail

1. Diagnosed that Stage 1 output was missing ~30 functions (`is_builtin_name`, `emit_builtin`, entire effects subsystem, type parameter handling).
2. Found 1,014 unification errors in the self-hosted type checker, 976 of which were "Unknown name" for constructors and local variables.
3. Discovered the self-hosted parser found **0 type definitions** out of 91 expected.
4. Traced to the bootstrap's `ExtractCodeBlocks` function: `indent >= 4` threshold rejected all 2-space-indented code blocks, silently dropping every type-definition-only file (SyntaxNodes, TokenKind, CodexType, AstNodes, Token, Name, etc.).
5. One-line fix: `indent >= 4` → `indent >= 2` in `tools/Codex.Bootstrap/Program.cs`.

### Cherry-picked from `l1l4_partial`

Reviewed the other agent's branch. Accepted orthogonal fixes, rejected regressions:

- ✅ Unifier: ErrorType cascading suppression
- ✅ DiagnosticBag: 20-error cap
- ✅ NothingTy → "object" in cs-type
- ❌ Removal of `is-builtin-name`/`emit-builtin` (needed for P1)
- ❌ Removal of effect parsing (regression)
- ❌ Removal of `IrFieldAccess` (regression)

---

## Process Notes

- **Reference compiler is LOCKED.** New features go in `.codex` source only. See `REFERENCE-COMPILER-LOCK.md`.
- **Agent toolkit**: `tools/agent/` — PowerShell + Bash: `peek`, `fstat`, `sdiff`, `trun`, `gstat`.
- **Cognitive dashboard**: `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux).
- **Session init**: `bash tools/linux-session-init.sh` for fresh Linux sessions.
- **Dates**: Always `checkdate()` before writing dates. Never trust training data.
- **Commits**: Working branches (`windows/<topic>`, `linux/<topic>`), review before merge.
- **Archived history**: `docs/OldStatus/` contains M0–M13 iteration handoffs.
