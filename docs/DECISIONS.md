# Decision Log

Significant design and engineering decisions are recorded here in chronological order.

---

## Decision: Bootstrap Language is C# on .NET 8
**Date**: 2025-06-20
**Context**: We need a language to write the first Codex compiler in. The compiler must be correct, maintainable, and productive to develop in.
**Options considered**: C#, Rust, Haskell, TypeScript, F#
**Decision**: C# 12 on .NET 8
**Rationale**: 
- Strong type system with nullable reference types catches many bugs at compile time
- Excellent tooling (Visual Studio, Rider, VS Code with C# Dev Kit)
- .NET ecosystem provides everything we need (immutable collections, cryptography, JSON, CLI)
- The first transpilation target is C# — writing the compiler in C# means we deeply understand the target
- F# was considered (its algebraic types are closer to what we're building) but C# has broader familiarity and better tooling for large projects
- Rust was considered but the development velocity for a compiler is lower than C#
**Consequences**: 
- The C# backend is the first and most important backend
- We use C# idioms (records, pattern matching, nullable annotations) pervasively
- The self-hosting path requires the C# backend to be complete before the Codex compiler can compile itself

---

## Decision: Hand-Written Lexer and Parser
**Date**: 2025-06-20
**Context**: We need to tokenize and parse Codex source, which has an unusual structure (prose/notation mode switching, indentation sensitivity).
**Options considered**: Hand-written, ANTLR, parser combinators (Sprache/Pidgin), tree-sitter
**Decision**: Hand-written lexer and recursive descent parser
**Rationale**:
- The prose/notation mode switching is too context-sensitive for most generators
- We need perfect error recovery for IDE support — hand-written parsers give full control
- We need full trivia tracking for round-trip fidelity
- Hand-written parsers are easier to debug and modify during language evolution
- Roslyn (C# compiler) uses the same approach and it works exceptionally well
**Consequences**:
- More initial development work than using a generator
- Full control over error messages and recovery
- No external dependency for parsing

---

## Decision: Solution Structure — Multi-Project with Layer Separation
**Date**: 2025-06-20
**Context**: The compiler has many distinct phases that should be testable independently.
**Options considered**: Monolithic project, multi-project layered, microservice (rejected immediately)
**Decision**: Multi-project solution with one project per compiler phase
**Rationale**:
- Each phase is independently testable
- Dependency direction is enforced by project references
- The compiler-as-library pattern enables CLI, LSP, and environment to share the same core
- Standard practice for large .NET solutions
**Consequences**:
- More projects to manage
- Must be disciplined about not creating circular dependencies
- Build time may increase (mitigated by incremental builds)

---

*Further decisions will be appended as they are made during implementation*

---

## Decision: IR is Not A-Normal Form
**Date**: 2025-07 (M3)
**Context**: The original spec called for A-Normal Form IR where all intermediate values are named. During implementation, this added complexity with no benefit — each backend emits differently, and ANF forced premature linearization.
**Decision**: Let IR expressions nest freely. `IRApply(IRApply(f, x), y)` is valid.
**Rationale**: Simpler lowering pass, each backend handles linearization if needed. C# and JS both naturally nest expressions. Rust handles it with blocks.
**Consequences**: No optimization passes rely on ANF. If we add optimization later, we may introduce a local normalization step.

---

## Decision: Curried Application in IR
**Date**: 2025-07 (M3)
**Context**: Codex functions are curried: `f a b` is `(f a) b`. The IR could flatten this to multi-arg calls or keep it curried.
**Decision**: IR uses single-arg `IRApply`. Multi-arg calls are nested: `IRApply(IRApply(f, a), b)`.
**Rationale**: Matches the language semantics. Each backend decides how to collapse curried calls — C# and JS emit multi-arg calls for known-arity functions, Rust does the same. The emitter has `CollectApplyArgs` to flatten when useful.
**Consequences**: Backends need arity tracking to emit efficient multi-arg calls.

---

## Decision: No Separate Runtime Library
**Date**: 2025-08 (M3–M5)
**Context**: The original design called for a hand-written runtime library per backend (Unit.cs, Maybe.cs, etc.).
**Decision**: No separate runtime. Built-in functions are emitted inline. Type definitions come from user code, not a library.
**Rationale**: The Codex type system is expressive enough that users define their own `Maybe`, `Result`, etc. Built-ins like `print-line`, `char-at`, `text-length` are simple enough to emit inline in each backend.
**Consequences**: No runtime dependency for generated code. Each backend is self-contained. New built-ins require changes to all three emitters.

---

## Decision: Direct I/O for Effects (No Monadic Encoding)
**Date**: 2025-09 (M5)
**Context**: Effects could be encoded as monads (Reader/Writer), interface injection, or direct I/O.
**Decision**: Direct I/O. `print-line` emits `Console.WriteLine` (C#), `console.log` (JS), `println!` (Rust). The effect type annotation is checked but not reified at runtime.
**Rationale**: Simple, working, and sufficient for the bootstrap. The effect type system prevents pure functions from calling effectful ones — that's the safety guarantee. Full algebraic effect handlers can be added later without changing existing code.
**Consequences**: No effect handlers yet. `run-state` and user-defined effects are deferred.

---

## Decision: long/double Instead of BigInteger/decimal
**Date**: 2025-08 (M3)
**Context**: The original design used `BigInteger` for Integer and `decimal` for Number.
**Decision**: Use `long` for Integer and `double` for Number.
**Rationale**: 64-bit primitives are fast and sufficient for the bootstrap. The compiler itself doesn't need arbitrary precision. Upgrading to BigInteger/BigRational later is a backend change, not a language change.
**Consequences**: Integer overflow is possible but not checked. Number precision is IEEE 754 double.

---

## Decision: Self-Hosting Without Type Checker in Codex
**Date**: 2026-03 (M13)
**Context**: Full byte-identical self-hosting requires a Codex-side type checker. Writing a bidirectional type checker with unification in purely functional Codex is a major undertaking.
**Decision**: Declare structural parity (264/264 records, 0 missing functions) as the M13 success criterion. The Stage 0 type checker handles all type checking. Stage 1 emits `object` types.
**Rationale**: The bootstrap proves the pipeline works end-to-end. The type checker can be added incrementally. Waiting for a perfect fixed point would block everything else.
**Consequences**: Stage 1 output compiles and runs but is not byte-identical to Stage 0. Full fixed-point is a future milestone.

---

## Decision: Codex Main Renamed to codex_main in Rust Backend
**Date**: 2026-03 (M12)
**Context**: Rust reserves `fn main()` as the entry point. The Codex `main` function collided.
**Decision**: Emit the Codex `main` as `codex_main`, and generate a Rust `fn main()` that calls it.
**Rationale**: Simple, no namespace pollution, clear separation between Codex semantics and Rust entry point convention.
**Consequences**: Rust output always has a `codex_main` function.
