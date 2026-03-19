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

### Snapshot (2026-03-18, evening)

| Metric | Value |
|--------|-------|
| .codex source | 26 files, 4,469 lines, 168K chars |
| C# reference projects | 34 |
| Test count | 780 (all passing) |
| Backends | 12 (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage) |
| Bootstrap | Stage 1 = Stage 2 = Stage 3 (105,411 chars, byte-for-byte) |
| Type debt | 7 `object` refs (all legitimate), **0** `_p0_` proxies |
| Thrash risk | MEDIUM (2/6) — context budget 133%, type debt clean |
| LSP | Diagnostics, hover, completion, go-to-def, symbols, semantic tokens |
| Repository | Content-addressed facts, proposals/verdicts, sync, trust/vouching |
| Agent workflow | Dual-agent mutual review, cognitive dashboard, agent toolkit |

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
| — | Post-M13: agent workflow (dual-agent review, cognitive dashboard, agent toolkit) |

---

## The Three Horizons

### Horizon 1: Language Freedom

> The language stands on its own. A programmer can write real programs in Codex
> without constantly hitting missing features.

| # | Task | Status | What |
|---|------|--------|------|
| L1 | **User-defined effects** | 🔶 Phase 1 done | `effect MyEffect where ...` declarations land (parser, desugarer, name resolver, type checker). Remaining: handler syntax + `resume` continuation + lowering/emission. Linux agent working on Phase 2. |
| L2 | **Module system** | ⬜ Not started | Proper `import`/`export` between files. Namespace management. Multi-file compilation exists but has no module boundaries or visibility control. |
| L3 | **Standard prelude** | ⬜ Not started | `Maybe`, `Result`, `Either`, `Tuple`, `Map`, `Set` as library types defined in Codex. Blocked by L2. |
| L4 | **Do-notation completion** | ⬜ Not started | Full monadic do-notation: `let!`, `return`, `do` blocks that desugar to `bind`/`pure` for any effect. |
| L5 | **String interpolation** | ✅ Done | `"Hello, #{name}!"` syntax (hash-brace). Lexer, parser, desugarer, all backends, 12-backend corpus. |
| L6 | **REPL** | ✅ Done | `codex repl` — interactive evaluation loop with `:help`, `:type`, `:defs`, `:reset`. Compiles through full pipeline, executes via `dotnet run`. |
| L7 | **Better error messages** | 🔶 In progress | "Did you mean X?" for undefined names ✅. Readable type formatting with `TypeFormatter` ✅. Related spans on type mismatches ✅. Ongoing. |

**Exit criterion**: A non-trivial program (e.g., a JSON parser, a small web server,
or a toy database) can be written entirely in Codex without hitting language gaps.

---

### Horizon 2: Library & Runtime

> The language has a standard library and can produce executables without depending
> on external toolchains.

| # | Task | Status | What |
|---|------|--------|------|
| R1 | **IL emitter: generics + TCO** | ✅ Done | Generics (ForAllType → IL generic params, method specs), TCO (tail-recursive → loop transform), records, sum types, pattern matching. 50 IL tests passing. |
| R2 | **Standard library** | ⬜ Not started | Collections, string utilities, IO abstractions. Written in Codex, compiled to all backends. |
| R3 | **FFI / host interop** | ⬜ Not started | Call .NET/JS/C APIs from Codex. Per-backend with common interface. |
| R4 | **Package management** | ⬜ Not started | Repository-based package resolution. `codex add <package>`. |
| R5 | **Build system: `codex.project.json`** | 🔶 Started | `CodexProject` model + `LoadProjectFile` + source glob resolution in CLI. Remaining: wire into `codex build` flow, dependency resolution. |
| R6 | **Native executable bootstrap** | ⬜ Not started | Compile Codex compiler to native `.exe` via IL emitter. Depends on R1 (done) + standard library gaps. |

**Exit criterion**: `codex build myproject/ --target il` produces a runnable `.exe`
with standard library support, no C# toolchain needed.

---

### Horizon 3: Infinity & Beyond

> The full vision from `NewRepository.txt` — proof-carrying code, federated repositories,
> the intelligence layer, and Codex as a platform for verified software.

| # | Task | Status | What |
|---|------|--------|------|
| V1 | **Views** | ⬜ | First-class consistent selections of facts. |
| V2 | **The Narration layer** | ⬜ | Prose-aware compilation where English text is load-bearing. |
| V3 | **Repository federation** | ⬜ | Multi-repository sync, cross-repo trust. |
| V4 | **Proof-carrying packages** | ⬜ | Every fact carries its proofs. Verified on import. |
| V5 | **Intelligence layer** | ⬜ | AI agents as first-class participants. Dashboard is step one. |
| V6 | **Trust lattice** | ⬜ | Vouching with degrees, trust-ranked search. |
| V7 | **Type-level function reduction** | ⬜ | Proof steps that unfold function definitions. |

---

## Recommended Sequence

```
Now ──→ L1 Phase 2 (handlers) + L6 (REPL) ──→ L2 (modules) ──→ L3 (prelude) ──→ L4 (do-notation)
                    │
                    ├──→ R5 (build system) ──→ R6 (native bootstrap)
                    │
                    └──→ R2 (stdlib) + R3 (FFI) ──→ R4 (packages)
                                                          │
                                                          └──→ V1-V7 (the vision)
```

**Current parallel work**:
- Linux agent: L1 Phase 2 (effect handlers with resume)
- Windows agent: L6 (REPL) — orthogonal, small, high-value

---

## Process Notes

- **Agent toolkit**: `tools/agent/` — `peek`, `fstat`, `sdiff`, `trun`, `gstat`. Use these instead of unreliable built-in tools.
- **Cognitive load**: Use `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux) at session start.
- **Dates**: Always `checkdate()` before writing dates. Never trust training data.
- **Commits**: Working branches (`windows/<topic>`, `linux/<topic>`), review before merge.

---

## Archived Documents
