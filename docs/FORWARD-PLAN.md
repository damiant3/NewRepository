# Forward Plan

*Updated March 15 2026, post-full-bootstrap.*

This document captures what's done, what's next, and what the open questions are.
It replaces the iteration handoff docs as the single source of truth for project direction.

Detailed deliverable checklists live in [08-MILESTONES.md](08-MILESTONES.md).
Design choices live in [DECISIONS.md](DECISIONS.md).

---

## Where We Are

The Codex compiler is self-hosting. The full pipeline works:

```
Source (.codex) → Lex → Parse → Desugar → NameResolve → TypeCheck → Lower → Emit → dotnet/node/rustc/...
```

| Metric | Value |
|--------|-------|
| C# projects | 31 |
| Test count | 451 (all passing) |
| Codex source | ~2,600 lines across 21 .codex files |
| Bootstrap parity | ~286 records, ~310 functions in output.cs |
| Backends | C#, JavaScript, Rust, Python, C++, Go, Java, Ada, Babbage, Fortran, COBOL (11 total) |
| LSP | Diagnostics, hover, completion, go-to-definition, symbols, semantic tokens |
| Repository | Content-addressed fact store with proposals/verdicts, import resolution |
| IDE support | TextMate grammar for VS 2022 + VS Code |
| Build system | Incremental builds, parallel front-end, parallel multi-target emission |
| Prose | Chapter/Section structure, prose templates (record & variant from bullet lists) |

---

## What's Done (Completed Milestones)

All of the following are ✅. See [08-MILESTONES.md](08-MILESTONES.md) for deliverable checklists.

| # | Milestone | Summary |
|---|-----------|---------|
| M0 | Foundation | Project structure, `Codex.Core`, content hashing, diagnostics |
| M1 | Hello Notation | Lexer, parser, CST/AST, desugaring |
| M2 | Type Checking | Bidirectional type checker, sum/record types, pattern matching |
| M3 | Execution via C# | IR, C# emitter, CLI (`codex check/build/run`) |
| M9 | LSP & Editor | Diagnostics, completion, hover, go-to-def, symbols, semantic tokens |
| M12 | JS & Rust backends | 3 backends total, 39 integration tests, TCO in all three |
| M13 | Self-hosting | Stage 0 → output.cs → Stage 1 → compiles Codex. C# generics. |
| M10 | Proofs | Refl, sym, trans, cong (bidirectional), induction with IH, lemma application |
| — | 8 more backends | Python, C++, Go, Java, Ada, Babbage, Fortran, COBOL. 165 integration tests. |
| — | IDE / syntax | TextMate grammar, VS 2022 `.pkgdef`, `codex.project.json`, `codex init` |
| — | Incremental builds | SHA256 content hashing, parallel front-end, parallel multi-target emission |

## What's Partially Done

| # | Milestone | What works | What's left |
|---|-----------|------------|-------------|
| M4 | Prose Integration | Chapter/Section parsing, prose templates (record/variant from bullets), prose-aware compilation | The Reader (`codex read`): formatted prose rendering to terminal |
| M5 | Effects | Effect rows, effect checking, effect polymorphism (row variables), Console/State, C# emission | Effect handlers (`run-state`), user-defined effects |
| M6 | Linear Types | Linearity annotations parse and type-check, C# runtime checks | **Linearity checker** — reject programs that use a linear value twice or not at all |
| M7 | Repository | Fact store, content hashing, CLI commands, `import` resolution from store via `IModuleLoader` | Views (single-user views, view consistency checking) |
| M8 | Dependent Types | Dependent function types, type-level arithmetic, proof obligations | Full `Vector` type with `append` end-to-end |
| M10 | Proofs | Induction, cong, lemma application, IH registration, 9 proofs in sample | Type-level function reduction (needed for non-trivial inductive steps), arithmetic induction with Peano encoding |
| M11 | Tests | Property-based tests, integration tests (451 total) | Fuzz testing, CI configuration |

---

## What's Next (Priority Order)

### Tier 1: Solidify What Exists

**1. Error recovery in parser**
The parser currently stops at the first error. For IDE use, it should skip to
the next definition and continue. The CST has `ErrorNode` support; the parser
just needs synchronization points. Estimated: medium.

**2. Linearity checker (M6)**
Linear type annotations exist and parse. The checker that rejects programs
using a linear value twice (or not at all) is missing. The type checker
already tracks linearity annotations — it needs a usage-counting pass.
Estimated: medium. See the Linear Haskell paper for the approach.

### Tier 2: Complete Partial Milestones

**3. Effect handlers (M5)**
`run-state` and user-defined effect handlers. The effect system already has
row variables for polymorphism — handlers need to eliminate effects from
the row. See [DECISIONS.md](DECISIONS.md): "Direct I/O for Effects."
Estimated: medium-large.

**4. Views (M7)**
The repository stores facts and resolves imports, but there's no view layer.
A View maps names to definitions such that all definitions are mutually
consistent. Single-user views first, then multi-user consensus.
Estimated: medium.

**5. The Reader (M4)**
`codex read <file>` renders a prose-mode document to the terminal with
formatted prose, highlighted notation blocks, and structured layout.
`Codex.Narration` is the project for this — currently empty.
Estimated: small-medium.

**6. Type-level function reduction (M10)**
Proof steps that require unfolding function definitions currently use
`assume`. The proof checker needs to normalize type-level expressions
by inlining function bodies. Arithmetic induction with Peano encoding
also depends on this. Estimated: medium.

### Tier 3: New Capabilities

**7. Package manager / dependency resolution**
The repository stores facts but there's no transitive dependency resolution
across modules. `import` currently resolves one level deep — it needs to
resolve transitively, handle version conflicts, and support views.
Estimated: large.

**8. Full `Vector` type (M8)**
The dependent type infrastructure works. Wire it up end-to-end: `Vector n a`
with `append : Vector m a → Vector n a → Vector (m + n) a`, compile and run.
Estimated: medium.

---

## Resolved Questions

These were open questions. Damian answered them; decisions are recorded in
[DECISIONS.md](DECISIONS.md).

### Language Design

1. **Module system** → **Prose-style imports (long-term).** The vision
   ([NewRepository.txt](Vision/NewRepository.txt)) describes capability-based
   prose imports: `I need: access to the filesystem to write files.` For now,
   `import TypeName` works as the notation-mode syntax and resolves from the
   repository's fact store. The prose-import design is an open question.

2. **Mutable state** → **Named-purpose mutability.** No general `ref` types.
   Mutable values must declare their purpose by naming convention —
   `UnificationState`, not `MutableRef`. See [DECISIONS.md](DECISIONS.md):
   "Codex-Side Type Checker — Threaded UnificationState."

3. **Type classes / traits** → **No explicit type classes.** Polymorphism is
   the compiler's problem. Prose handles subtyping naturally. Design work needed.

4. **String interpolation** → **No.** `++` and named functions only.
   See [DECISIONS.md](DECISIONS.md): "No String Interpolation Syntax."

5. **Tail call optimization** → **✅ Done.** All 11 backends convert
   self-recursive tail calls to loops. See [DECISIONS.md](DECISIONS.md):
   "Tail Call Optimization via Loop Conversion."

### Tooling

6. **CI pipeline** → **Deferred.** Not until there are users or funding.

7. **VS Code extension publishing** → **Deferred.** Same.

8. **Documentation generation** → **Deferred.** Low priority.

### Architecture

9. **Incremental compilation** → **✅ Done.** SHA256 content hash + timestamp.
   Parallel front-end, parallel emission. See M11c in the completed list above.

10. **Test preservation** → **Policy enforced.** Sample `.codex` files go in
    `samples/`, integration tests call the compiler on them. Every sample is
    a permanent regression test. Currently 451 tests across 7 test projects.

---

## Open Questions (Remaining)

1. **Prose-import syntax** — What exactly does a prose import look like?
   `I need: access to the filesystem` implies capability-based imports rather
   than module-based. How does this interact with the effect system?
   (See [NewRepository.txt](Vision/NewRepository.txt) Chapter 4.)

2. **Auto-memoization** — Which functions get memoized? All recursive pure
   functions? Only those marked? Only those with primitive arguments? What's
   the eviction policy? This needs a design doc before implementation.
   (See the Named-purpose mutability decision.)

3. **Subtype-inference for prose polymorphism** — "Add this fish to the list
   of animals" implies structural subtyping or coercion. Codex currently has
   nominal sum types. How do we bridge the gap?

4. **Heap-allocated stack frames** — For non-tail recursive functions, should
   we offer CPS transform + trampoline? TCO covers the tail-call case;
   this would cover general deep recursion.

---

## Technical Debt

| Item | Location | Impact | Effort | Status |
|------|----------|--------|--------|--------|
| Rust backend doesn't `.clone()` for recursive calls | `RustEmitter.cs` | Generated Rust won't compile for recursive `String`/`Vec` args (mitigated by TCO) | Medium | Open |
| JS uses `BigInt` (`42n`) for integers | `JavaScriptEmitter.cs` | Can't mix with `number` in arithmetic without explicit conversion | Low | Intentional |
| `output.cs` is tracked in git | `codex-src/output.cs` | Large generated file in version control | Low | Convenient for now |
| Iteration handoff docs are stale | `docs/ITERATION-*-HANDOFF.md` | 10 docs from old iteration model, superseded by this plan | Low | Archive or delete |
| `Codex.Narration` project is empty | `src/Codex.Narration/` | No prose rendering capability | Low | Deferred — see "The Reader (M4)" above |
| `TypeVariable emits as object` decision is superseded | `DECISIONS.md` | Superseded by "C# Generics for Polymorphic Functions" | None | Marked in DECISIONS.md |

---

## Principles (Unchanged)

See [10-PRINCIPLES.md](10-PRINCIPLES.md). The core principles haven't changed:

1. Ship working software at every milestone
2. Correctness over performance
3. Immutability by default
4. No premature abstraction
5. The human holds the vision; the tools serve the human

---

*This document tracks direction. The milestones doc
([08-MILESTONES.md](08-MILESTONES.md)) tracks deliverable status. The decision
log ([DECISIONS.md](DECISIONS.md)) tracks design choices.*
