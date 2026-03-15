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
| Test count | 246 (all passing) |
| Codex source | ~2,500 lines across 14 .codex files |
| Bootstrap parity | 264/264 records, 0 missing functions |
| Backends | C# (primary), JavaScript, Rust |
| LSP | Diagnostics, hover, symbols, semantic tokens |
| Repository | Content-addressed fact store with proposals/verdicts |

---

## What's Next (Priority Order)

### Tier 1: Solidify What Exists

These are low-risk, high-value tasks that strengthen the foundation.

**1. Backend integration tests**
Write automated tests that compile each sample to all three backends and verify
output (JS via `node`, C# via `dotnet run`, Rust via `rustc`). Currently this
is done manually. Estimated: small.

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

**10. Induction proofs (M10)**
Proof by induction on data structures. The proof system has `Refl`, `sym`,
`trans`, `cong` but no structural induction. Requires:
- Induction principle generation for each sum type
- Proof obligation for base case and inductive step
- Integration with the type checker

Estimated: large.
Damian: Priority Critical.  QED.

**11. Additional backends**
Python is the most requested. WASM/LLVM are aspirational. A Python backend
would follow the same pattern as JS (dynamic types, runtime assertions).
Estimated: medium per backend.

**12. Package manager / dependency resolution**
The repository stores facts but there's no dependency resolution across
modules. `import` needs to resolve transitively, handle version conflicts,
and support views. Estimated: large.

---

## Open Questions

### Language Design

1. **Module system** — Codex files are compiled independently. How should
   multi-file projects work? Options: (a) explicit imports with path resolution,
   (b) repository-based resolution (content-addressed), (c) directory-based
   convention (all .codex files in a directory are one module). Currently the
   bootstrap uses (c) via the CLI `build <dir>` command.
Damian: Prosish? "I need: access to the filesystem to write files.  I need: access to to the internet to lookup a webpage and get some data."


2. **Mutable state** — The language is purely functional. The type checker needs
   mutable unification state. Options: (a) monadic state threading (verbose),
   (b) add `ref` types with effect tracking, (c) use the effect system with a
   `State` effect. Decision needed before writing the Codex-side type checker.
Damian: Use the best option that some PhD language designer isn't going to argue with, but it should be clear in the code and prose that its mutability is for a particular purpose.  Like "MoveableObject" or something like that.  maybe a naming convention requirement.  Adjectinve+Noun answering "why are you changing this value?" "Because it's a unification variable, and we need to unify it with other types as we go.  It's not a general-purpose mutable reference that you can do whatever you want with."  Something like that."
There... is sorta an idea i had about... this.  like how many programs are going to use prime numbers right.  and if we bother to do it, shouldn't we automemoize it?  like in the runtime, we start with a hardcoded list of the first 100 primes or something, and then if you ask for the 101st prime, we compute it, add it to the list, and then next time we need a prime, we check the list first.  That way we get memoization for free without having to expose mutable state in the language.  It's just an implementation detail of the runtime.  We can do that for any expensive computation that has a pure function interface.  Like Fibonacci numbers, or factorials, or whatever.  We can just have a memoized version in the runtime that uses mutable state internally but exposes a pure interface to the language.  That way we keep the language purely functional but still get performance benefits from memoization.
3. **Type classes / traits** — Codex has no ad-hoc polymorphism. `show` is a
   built-in, not a type class method. For a richer standard library, we'll need
   something like Haskell type classes or Rust traits. Design not yet started.
Damian: Polymorphism is your problem.  Prose should be "I have a list of animals, add this fish to it."  "Get a fish from the list of animals, or anything aquatic if you can't find a fish."

4. **String interpolation** — The lexer was designed with `${ }` interpolation
   in mind but it's not implemented. Is it needed? The `++` operator and
   `integer-to-text` cover most cases.
1. Damian: hell no.  ${} is ugly.  i can't read that.  no string decorations.  named functions sure but no special syntax for them.  if you want to build a string, use a function.  if you want to concatenate, use ++.  if you want to convert an integer to text, use integer-to-text.  it's not that hard.  we don't need to make it easier with special syntax that is also less readable.

5. **Tail call optimization** — Codex uses recursion for all iteration. The C#
   backend doesn't optimize tail calls (CLR doesn't guarantee TCO). The JS
   backend doesn't either. Rust does in some cases. Should the IR have explicit
   tail call markers? Should the emitter convert tail recursion to loops    ?
Damian: yes and yes  recursion is an impl detail, and has its own problems in modern computers.  it should be possible to opt in at compile time as a known risk?  optimizer hint here?  when I built the lattice, it could stack overflow and I built my whole own stackframe on the heap.  it was fast enough and dropped the risk of SO to zero, effectively.  so pure recursion is just asking for trouble.  like what about thread context switches... you want that shit droppin in and outta cache?  i dunno.

### Tooling

6. **CI pipeline** — No CI exists. The build and test commands are manual.
   GitHub Actions with `dotnet build` + `dotnet test` + bootstrap verification
   would catch regressions. Low effort, high value.
Damian: Not until we have users or people giving me money.
7. **VS Code extension publishing** — The extension works locally but isn't
   published to the VS Code marketplace. Publishing requires a publisher
   account and VSIX packaging.
Damian: same as above
8. **Documentation generation** — Codex source files are literate documents.
   There's no tool to render them as HTML/PDF. The `Codex.Narration` project
   exists but is empty.
Damian: meh.
### Architecture

9. **Incremental compilation** — The LSP re-runs the full pipeline on every
   change. For large files this will be slow. Incremental type checking requires
   dependency tracking between definitions. Not urgent yet — the compiler is
   fast enough for current file sizes.
Damian: Current file sizes are about to explode.  Raise priority.
10. **Diagnostics quality** — Error messages are functional but terse. The type
    checker reports "Cannot unify X with Y" but doesn't suggest fixes. The
    parser reports "Unexpected token" but doesn't show what was expected.
    Improving diagnostic quality is ongoing work.
Damian: Yes I noticed you often created test code, then deleted it.  I want you to change that by instead moving the test code to a folder with the file stamp.  I then we have a record of the kinds of errors you were encountering, and how you fixed them.  This is useful for me to understand your process, and also to have a record of the kinds of errors that come up in compiler development, which can be useful for future reference.
   And it sorta goes against the "no modes version of the world" principle if you have to nuke your tests.  I've thought along time during the grind that we didn't have enough testing (as part of the build/unit testing).  we should just be calling our compiler from our test lib with the files you create, every time.
---

## Technical Debt

| Item | Location | Impact | Effort |
|------|----------|--------|--------|
| Rust backend doesn't `.clone()` for recursive calls | `RustEmitter.cs` | Generated Rust won't compile for recursive functions using `String`/`Vec` args | Medium |
| JS uses `BigInt` (`42n`) for integers | `JavaScriptEmitter.cs` | Can't mix with `number` in arithmetic without explicit conversion | Low (intentional design) |
| No test coverage for emitters | `Codex.Emit.*` | Emitter bugs found manually, not by CI | Medium |
| `output.cs` is tracked in git | `codex-src/output.cs` | Large generated file in version control | Low (convenient for now) |
| Iteration handoff docs are stale | `docs/ITERATION-*-HANDOFF.md` | 12 docs from old iteration model, now superseded by this plan | Low (archive or delete) |
| `Codex.Narration` project is empty | `src/Codex.Narration/` | No prose rendering capability | Low until M4 prose templates |
| `Codex.Proofs` is minimal | `src/Codex.Proofs/` | Basic proof terms exist, no proof checker | Low until M10 induction |

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
