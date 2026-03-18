# Forward Plan

**Date**: 2026-03-18 (verified via system clock)
**Supersedes**: `docs/OldStatus/FORWARD-PLAN.md` (archived, stale numbers)

This is the single source of truth for project direction.

---

## Where We Are

The Codex compiler is **self-hosting with a proven fixed point**. The full pipeline works
across 12 backends. The language has algebraic types, pattern matching, effects, linear
types, dependent types, proofs, and literate programming. The tooling includes an LSP
server, IDE extensions, a content-addressed repository with collaboration protocol, and
a cognitive load dashboard for agent sessions.

### Snapshot (2026-03-18)

| Metric | Value |
|--------|-------|
| .codex source | 21 files, 4,411 lines, 176K chars |
| C# reference projects | 32 |
| Test count | 722 (all passing) |
| Backends | 12 (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage) |
| Bootstrap | Stage 2 = Stage 3 (227,301 chars, byte-for-byte) |
| Type debt | 7 `object` refs (all legitimate), 48 `_p0_` proxies (cosmetic) |
| LSP | Diagnostics, hover, completion, go-to-def, symbols, semantic tokens |
| Repository | Content-addressed facts, proposals/verdicts, sync, trust/vouching |
| Agent workflow | Dual-agent mutual review, cognitive load monitoring, modular rules |

### What's Complete (M0–M13+)

| Milestone | What |
|-----------|------|
| M0 | Foundation: project structure, core primitives |
| M1 | Notation: lexer, parser, CST/AST |
| M2 | Type checking: bidirectional, sum/record types, exhaustiveness |
| M3 | Execution: IR, C# emitter, `codex build/run` |
| M4 | Prose integration: literate programming, `codex read` |
| M5 | Effects: Console, State, FileSystem, `run-state` handler |
| M6 | Linear types: linearity checker, FileHandle |
| M7 | Repository: content-addressed fact store, `init/publish/history` |
| M8 | Dependent types: type-level arithmetic, proof obligations, Vector |
| M9 | LSP & editor: full language server, VS 2022 + VS Code extensions |
| M10 | Proofs: refl, sym, trans, cong, induction, lemma application |
| M11 | Collaboration: proposals, verdicts, trust, sync |
| M12 | 12 backends: all mainstream targets + IL + Babbage |
| M13 | Self-hosting: compiler written in Codex, fixed point achieved |
| — | Post-M13: error recovery, parser robustness, multi-file compilation |
| — | Post-M13: 12-backend audit, TCO in all emitters, nested match fixes |
| — | Post-M13: agent workflow (dual-agent review, cognitive dashboard) |

---

## The Three Horizons

### Horizon 1: Language Freedom

> The language stands on its own. A programmer can write real programs in Codex
> without constantly hitting missing features.

| # | Task | What | Effort |
|---|------|------|--------|
| L1 | **User-defined effects** | `effect MyEffect where ...` declarations, custom handlers beyond built-in `run-state`. Algebraic effect handlers with `handle`/`resume`. | Medium |
| L2 | **Module system** | Proper `import`/`export` between files. Namespace management. The multi-file compilation exists but has no module boundaries or visibility control. | Medium |
| L3 | **Standard prelude** | `Maybe`, `Result`, `Either`, `Tuple`, `Map`, `Set` as library types defined in Codex (not hardcoded in the C# reference compiler). Bootstrap the prelude from `.codex` source. | Small-Medium |
| L4 | **Do-notation completion** | Full monadic do-notation: `let!`, `return`, `do` blocks that desugar to `bind`/`pure` for any effect. Currently `do` works but is limited to built-in effects. | Small |
| L5 | **String interpolation** | `"Hello, {name}!"` syntax that desugars to concatenation. Quality-of-life for real programs. | Small |
| L6 | **REPL** | `codex repl` — interactive evaluation loop. Parse, type-check, and evaluate expressions on the fly. Huge for learning and debugging. | Medium |
| L7 | **Better error messages** | Source-span-accurate errors with suggestions. "Did you mean X?" for misspelled names. Show expected vs actual types clearly. | Ongoing |

**Exit criterion**: A non-trivial program (e.g., a JSON parser, a small web server,
or a toy database) can be written entirely in Codex without hitting language gaps.

---

### Horizon 2: Library & Runtime

> The language has a standard library and can produce executables without depending
> on external toolchains.

| # | Task | What | Effort |
|---|------|------|--------|
| R1 | **IL emitter: generics + TCO** | Complete the IL emitter (`Codex.Emit.IL`) so it handles generic functions and tail-call optimization. This is the path to native `.exe` without the C# transpile step. | Medium |
| R2 | **Standard library** | Collections (Map, Set, Array, Queue), string utilities, IO abstractions, concurrency primitives. Written in Codex, compiled to all backends. | Large (incremental) |
| R3 | **FFI / host interop** | Call .NET APIs from Codex (for C#/IL targets), call JS APIs (for JS target), call C APIs (for Rust/C++ targets). Defined per-backend with a common interface. | Medium-Large |
| R4 | **Package management** | Repository-based package resolution. `codex add <package>`, dependency graphs, version resolution via the fact store. | Medium |
| R5 | **Build system: `codex.project.json`** | Project file format that specifies source files, dependencies, target backends, and build options. `codex build` reads this instead of requiring CLI flags. | Small-Medium |
| R6 | **Native executable bootstrap** | Once the IL emitter handles the full language, compile the Codex compiler to a native `.exe` that doesn't need `dotnet` at all. True independence. | Medium (depends on R1) |

**Exit criterion**: `codex build myproject/ --target il` produces a runnable `.exe`
with standard library support, no C# toolchain needed.

---

### Horizon 3: Infinity & Beyond

> The full vision from `NewRepository.txt` — proof-carrying code, federated repositories,
> the intelligence layer, and Codex as a platform for verified software.

| # | Task | What | Effort |
|---|------|------|--------|
| V1 | **Views** | First-class consistent selections of facts. Your dev environment shows a view. Production runs a view. Views compose. | Medium |
| V2 | **The Narration layer** | `Codex.Narration` — prose-aware compilation where English text is load-bearing. Template matching: "An X is a record containing:" generates a record type. | Large |
| V3 | **Repository federation** | Multi-repository sync, cross-repo trust, global namespace management. Facts flow between independent repos. | Large |
| V4 | **Proof-carrying packages** | Every fact in the repository carries its proofs. Import a function, get its correctness guarantee. The type checker verifies proofs on import. | Large |
| V5 | **Intelligence layer integration** | AI agents as first-class participants in the development process. The cognitive dashboard is step one. Next: agents that can propose facts, review proposals, vouch for correctness. | Exploratory |
| V6 | **Trust lattice** | Vouching with degrees (Reviewed, Tested, Verified, Critical). Trust-ranked search. Capability-based discovery. The social infrastructure for verified code. | Medium |
| V7 | **Type-level function reduction** | Proof steps that unfold function definitions. Arithmetic induction with Peano encoding. Full normalization-by-evaluation for type-level terms. | Medium-Large |

**Exit criterion**: A team of developers (human + AI) collaborates on a Codex project
using the repository protocol, with proofs verified on import and trust tracked across
contributors.

---

## Recommended Sequence

The horizons are not strictly sequential — work can happen in parallel. But the
dependencies suggest a natural flow:

```
Now ──→ L1 (user effects) + L2 (modules) ──→ L3 (prelude) ──→ L4-L7 (polish)
                    │
                    ├──→ R1 (IL generics) ──→ R6 (native bootstrap)
                    │
                    └──→ R2 (stdlib) + R3 (FFI) ──→ R4 (packages) + R5 (build)
                                                          │
                                                          └──→ V1-V7 (the vision)
```

**Immediate next session**: L1 (user-defined effects) or L2 (module system) — both
unblock the prelude (L3), which unblocks everything else.

---

## Process Notes

- **Agent workflow**: Dual-agent mutual review via branch workflow. See `.github/agent-rules/`.
- **Cognitive load**: Use `tools/codexdashboard.sh` (Linux) or `tools/codex-dashboard.ps1`
  (Windows) at session start. The hot-path files are 237% of context budget — work on
  one pipeline stage at a time.
- **Dates**: Always `checkdate()` before writing dates. Never trust training data.
- **Commits**: Working branches (`windows/<topic>`, `linux/<topic>`), review before merge.

---

## Archived Documents

The following documents are superseded by this plan and archived in `docs/OldStatus/`:

| Document | Status |
|----------|--------|
| `FORWARD-PLAN.md` | Superseded. Stale bootstrap numbers. |
| `PostFixedPointCleanUp.md` | Audited and closed. See `PostFixedPointCleanUp-AUDIT.md`. |
| `ITERATION-*-HANDOFF.md` (1–11) | Historical record. Date corrections applied. |
| `HANDOFF-BOOTSTRAP-FIXEDPOINT.md` | Complete. Fixed point achieved. |
| `HANDOFF-TYPED-LOWERING.md` | Complete. |
| `DECISIONS.md` | Still active — append new decisions here. |
| `REFLECTIONS.md` | Historical. |
