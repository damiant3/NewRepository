# Forward Plan

*Updated March 2026, post-bootstrap.*

This document captures what's done, what's next, and what the open questions are.
It replaces the iteration handoff docs as the single source of truth for project status.

---

## Where We Are

The Codex compiler is self-hosting. The full pipeline works:

```
Source (.codex) → Lex → Parse → Desugar → NameResolve → TypeCheck → Lower → Emit → dotnet/node/rustc
```

| Metric | Value |
|--------|-------|
| C# projects | 22 |
| Test count | 285 (all passing) |
| Codex source | ~2,500 lines across 14 .codex files |
| Bootstrap parity | 264/264 records, 0 missing functions |
| Backends | C# (primary), JavaScript, Rust |
| LSP | Diagnostics, hover, symbols, semantic tokens |
| Repository | Content-addressed fact store with proposals/verdicts |

---

## What's Next (Priority Order)

### Tier 1: Solidify What Exists

These are low-risk, high-value tasks that strengthen the foundation.

**1. ~~Backend integration tests~~** — ✅ Done.
39 integration tests across 13 samples × 3 backends. Plus 3 TCO-specific tests.

**2. LSP completion and go-to-definition**
The LSP server has diagnostics, hover, and symbols but no completion or
go-to-definition. These are the two most impactful editor features for
daily use. Estimated: medium.

**3. Stage 2 verification**
Run the Stage 1 binary on `codex-src/` and verify it produces output identical
to Stage 1's output. This proves the bootstrap is a true fixed point (modulo
type annotations). Estimated: small — the infrastructure exists, just needs a
CI script.

**4. Error recovery in parser**
The parser currently stops at the first error. For IDE use, it should skip to
the next definition and continue. The CST has `ErrorNode` support; the parser
just needs synchronization points. Estimated: medium.

### Tier 2: Complete Partial Milestones

These finish work that's 60–80% done.

**5. Linearity checker (M6)**
Linear type annotations exist and parse. The checker that rejects programs
using a linear value twice (or not at all) is missing. The type checker
already tracks linearity annotations — it needs a usage-counting pass.
Estimated: medium.

**6. Effect polymorphism (M5)**
`map` should propagate effects: if the function argument is effectful, `map`
is effectful. Currently effects are checked but not polymorphic. Requires
effect row variables in the type checker. Estimated: medium-large.

**7. Prose templates (M4)**
"An Account is a record containing:" should parse as a record type definition.
The prose parser recognizes Chapter/Section headers but not semantic templates.
Estimated: medium.

**8. Repository imports (M7)**
`import Account` should resolve from the content-addressed store. The store
exists, facts are publishable, but the compiler doesn't query the store during
name resolution. Estimated: medium.

### Tier 3: New Capabilities

These open new doors.

**9. Codex-side type checker (M13 completion)**
Write the bidirectional type checker in Codex. This is the biggest remaining
piece for true self-hosting. Challenges:
- Unification requires mutable state or monadic threading
- Type environments need efficient maps (Codex has `Map` but no balanced trees)
- Error reporting needs source spans threaded through

This is a separate milestone. Estimated: large.

**10. Induction proofs (M10)** — **Priority: Critical**
Proof by induction on data structures. The proof system has `Refl`, `sym`,
`trans`, `cong` but no structural induction. Requires:
- Induction principle generation for each sum type
- Proof obligation for base case and inductive step
- Integration with the type checker

Estimated: large.

**11. Additional backends**
Python is the most requested. WASM/LLVM are aspirational. A Python backend
would follow the same pattern as JS (dynamic types, runtime assertions).
Estimated: medium per backend.

**12. Package manager / dependency resolution**
The repository stores facts but there's no dependency resolution across
modules. `import` needs to resolve transitively, handle version conflicts,
and support views. Estimated: large.

---

## Resolved Questions

These were open questions. Damian answered them; decisions are recorded here
and in [DECISIONS.md](DECISIONS.md).

### Language Design

1. **Module system** → **Prose-style imports.** Not `import X` but something
   like `I need: access to the filesystem to write files.` Declarative,
   natural language, consistent with the prose-first philosophy. The compiler
   resolves capabilities from these declarations. Design work needed.

2. **Mutable state** → **Named-purpose mutability.** No general `ref` types.
   Mutable values must declare their purpose by naming convention —
   `UnificationVariable`, not `MutableRef`. The name answers "why are you
   changing this value?" Additionally, pure functions that are expensive
   (primes, fibonacci) should be auto-memoized by the runtime: mutable cache
   internally, pure interface to the language. The programmer never sees state.

3. **Type classes / traits** → **No explicit type classes.** Polymorphism is
   the compiler's problem, not the programmer's. Prose handles subtyping
   naturally: "I have a list of animals, add this fish to it." The compiler
   infers the necessary coercions. Design work needed on how this maps to
   the type checker.

4. **String interpolation** → **No.** Decided. `++` and named functions only.
   See [DECISIONS.md](DECISIONS.md).

5. **Tail call optimization** → **✅ Done.** All three backends now convert
   self-recursive tail calls to loops. Tested with 1,000,000-deep recursion.
   See [DECISIONS.md](DECISIONS.md).

### Tooling

6. **CI pipeline** → **Deferred.** Not until there are users or funding.

7. **VS Code extension publishing** → **Deferred.** Same.

8. **Documentation generation** → **Deferred.** Low priority.

### Architecture

9. **Incremental compilation** → **Priority raised.** File sizes are about to
   grow significantly. The LSP needs definition-level incremental type checking
   with dependency tracking. This is now a Tier 2 priority.

10. **Test preservation** → **New policy.** Test code created during development
    must be kept, not deleted. Sample `.codex` files go in `samples/`, and
    integration tests call the compiler on them via `Helpers.CompileToCS/JS/Rust`.
    Every sample is a permanent regression test. This is now enforced: 285 tests
    including 39 emitter integration tests across all samples × 3 backends.

---

## Open Questions (Remaining)

1. **Prose-import syntax** — What exactly does a prose import look like?
   `I need: access to the filesystem` implies capability-based imports rather
   than module-based. How does this interact with the effect system?

2. **Auto-memoization** — Which functions get memoized? All recursive pure
   functions? Only those marked? Only those with primitive arguments? What's
   the eviction policy? This needs a design doc before implementation.

3. **Subtype-inference for prose polymorphism** — "Add this fish to the list
   of animals" implies structural subtyping or coercion. Codex currently has
   nominal sum types. How do we bridge the gap?

4. **Heap-allocated stack frames** — Damian mentioned building stack frames
   on the heap for deep recursion (lattice project). For non-tail recursive
   functions, should we offer a similar strategy? CPS transform + trampoline?

---

## Technical Debt

| Item | Location | Impact | Effort | Status |
|------|----------|--------|--------|--------|
| Rust backend doesn't `.clone()` for recursive calls | `RustEmitter.cs` | Generated Rust won't compile for recursive `String`/`Vec` args (mitigated by TCO for tail-recursive functions) | Medium | Open |
| JS uses `BigInt` (`42n`) for integers | `JavaScriptEmitter.cs` | Can't mix with `number` in arithmetic without explicit conversion | Low | Intentional |
| ~~No test coverage for emitters~~ | `Codex.Emit.*` | ~~Emitter bugs found manually~~ | ~~Medium~~ | **Fixed** — 39 integration tests across 13 samples × 3 backends |
| `output.cs` is tracked in git | `codex-src/output.cs` | Large generated file in version control | Low | Convenient for now |
| Iteration handoff docs are stale | `docs/ITERATION-*-HANDOFF.md` | 12 docs from old iteration model, now superseded by this plan | Low | Archive or delete |
| `Codex.Narration` project is empty | `src/Codex.Narration/` | No prose rendering capability | Low | Deferred until M4 |
| `Codex.Proofs` is minimal | `src/Codex.Proofs/` | Basic proof terms exist, no proof checker | Low | **Priority: Critical** (M10) |

---

## Principles (Unchanged)

See [10-PRINCIPLES.md](10-PRINCIPLES.md). The core principles haven't changed:

1. Ship working software at every milestone
2. Correctness over performance
3. Immutability by default
4. No premature abstraction
5. The human holds the vision; the tools serve the human

---

*This document will be updated as priorities shift. The milestones doc
([08-MILESTONES.md](08-MILESTONES.md)) tracks deliverable status. The decision
log ([DECISIONS.md](DECISIONS.md)) tracks design choices. This doc tracks
direction.*
