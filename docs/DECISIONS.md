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

*Further decisions will be appended as they are made during implementation.*
