# Current Plan

**Date**: 2026-03-19 (verified via system clock)

---

## Where We Are

The Codex compiler is self-hosting with a proven fixed point and a proven
quine disproof. The reference C# compiler is **locked** (`REFERENCE-COMPILER-LOCK.md`).
All future language development happens in `.codex` source, compiled through the
self-hosted pipeline.

### Snapshot

| Metric | Value |
|--------|-------|
| .codex source | 26 files, 4,444 lines, 164K chars |
| Tests | 810 (all passing) |
| Backends | 12 (C#, JS, Python, Rust, C++, Go, Java, Ada, Fortran, COBOL, IL, Babbage) |
| Bootstrap | Fixed point: Stage 1 = Stage 3 (227,301 chars, byte-for-byte) |
| Type debt | 7 `object` refs (all legitimate), 0 `_p0_` proxies |
| Prelude | 4 modules: Maybe, Result, Either, Pair |
| Quine disproof | `samples/expr-calculator.codex` — 10/10 PASS |
| Ref compiler | 🔒 Locked at `6d8bb2c` on 2026-03-19 |

### The Language Today

The language has algebraic types, pattern matching, bidirectional type inference,
user-defined effects with handlers and `resume`, do-notation, linear types,
dependent types, proofs, literate programming, string interpolation, a module
system with imports/exports, and a standard prelude. The tooling includes an LSP
server, VS Code and VS 2022 extensions, a REPL, a content-addressed repository
with collaboration protocol, and a cognitive load dashboard for agent sessions.

The self-hosted emitter currently handles the C# backend. L1–L4 features
(effects, modules, prelude, do-notation) are being ported to the self-hosted
emitter by the Windows agent; assume complete.

---

## What's Next

### Near Term: Make the Language Practical

| # | Task | Status | What |
|---|------|--------|------|
| E1 | **Exit criterion: real programs** | 🔶 Started | `expr-calculator.codex` proves the compiler works. Next: something larger — a JSON parser, Markdown formatter, or small utility that exercises the full language including imports, prelude types, and effects. |
| L7 | **Better error messages** | 🔶 Ongoing | "Did you mean X?" ✅. Readable type formatting ✅. Related spans ✅. Remaining: error recovery improvements, source location accuracy in self-hosted pipeline. |
| R5 | **Build system** | 🔶 Started | `CodexProject` model + `LoadProjectFile` exist. Remaining: wire into `codex build` flow, multi-file dependency resolution, output directory management. |
| P1 | **Self-hosted built-in expansion** | ⬜ | The self-hosted emitter calls built-ins by name (`text_length`, `char_at`, `print_line`) instead of inlining them like the reference compiler does (`.Length`, `[i].ToString()`, `Console.WriteLine`). Stage 1 output needs a runtime shim or inline expansion to run standalone. |

### Medium Term: Library & Runtime

| # | Task | Status | What |
|---|------|--------|------|
| R2 | **Standard library** | ⬜ | Collections (`Map`, `Set`, `Array`), string utilities, IO abstractions. Written in Codex, compiled to all backends. Builds on the prelude. |
| R3 | **FFI / host interop** | ⬜ | Call .NET/JS/C APIs from Codex. Per-backend with a common interface declaration syntax. Required for any real-world program. |
| R4 | **Package management** | ⬜ | Repository-based package resolution. `codex add <package>`. Builds on the existing content-addressed fact store (M7/M11). |
| R6 | **Native executable bootstrap** | ⬜ | Compile the Codex compiler to a native `.exe` via the IL emitter. The IL emitter already handles generics, TCO, records, sum types, and pattern matching (50 tests). Gap: standard library functions and built-in expansion in IL. |

**Exit criterion**: `codex build myproject/ --target il` produces a runnable `.exe`
with standard library support, no C# toolchain needed.

### Long Term: The Vision

| # | Task | What |
|---|------|------|
| V1 | **Views** | First-class consistent selections of facts from the repository. |
| V2 | **The Narration layer** | Prose-aware compilation where English text is load-bearing. `Codex.Narration` project exists but is empty. |
| V3 | **Repository federation** | Multi-repository sync, cross-repo trust and identity. |
| V4 | **Proof-carrying packages** | Every published fact carries its proofs. Verified on import. |
| V5 | **Intelligence layer** | AI agents as first-class participants in the repository. Dashboard is step one. |
| V6 | **Trust lattice** | Vouching with degrees, trust-ranked search across contributors. |
| V7 | **Type-level function reduction** | Proof steps that unfold function definitions. Needed for non-trivial inductive proofs. |

---

## Recommended Sequence

```
Now ──→ P1 (built-in expansion) + E1 (real program) + R5 (build system)
            │
            ├──→ R2 (stdlib) + R3 (FFI)
            │       │
            │       └──→ R6 (native bootstrap via IL)
            │
            └──→ R4 (packages) ──→ V1-V7 (the vision)
```

**P1 is the critical path.** Until the self-hosted emitter can inline built-ins,
Stage 1 output requires a runtime shim to execute. Fixing this unblocks R6
(native bootstrap) and makes the self-hosted pipeline fully standalone.

**E1 validates everything.** Writing a real program against the full language
surface (imports, prelude, effects, do-notation, records, pattern matching)
will shake out any remaining gaps before investing in stdlib and FFI.

---

## Process Notes

- **Reference compiler is LOCKED.** New features go in `.codex` source only. Bug fixes to Stage 0 are permitted only when necessary to compile `.codex` source. See `REFERENCE-COMPILER-LOCK.md`.
- **Agent toolkit**: `tools/agent/` — PowerShell (`.ps1`) and Bash (`.sh`) versions of `peek`, `fstat`, `sdiff`, `trun`, `gstat`.
- **Cognitive dashboard**: `tools/codex-dashboard.ps1` (Windows) or `tools/codexdashboard.sh` (Linux). Run at session start.
- **Session init**: `bash tools/linux-session-init.sh` sets up a fresh Linux session (.NET, clone, build, tests, dashboard).
- **Dates**: Always `checkdate()` before writing dates. Never trust training data.
- **Commits**: Working branches (`windows/<topic>`, `linux/<topic>`), review before merge.
- **Archived milestone history**: `docs/OldStatus/` contains iteration handoffs M0–M13 and the old forward plan.
