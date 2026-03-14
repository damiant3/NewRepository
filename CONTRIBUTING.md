# Contributing Guidelines for SparseLattice

This file documents the coding standards, development workflow, testing requirements, and acceptance criteria that all contributors (including automated agents) must follow when working on the `SparseLattice` and `SparseLattice.Test` projects. These rules are strict and intended to preserve the style, architecture and quality exemplified by the locked-down reference projects (e.g. `SpatialDbLib`). Do not change any code in other projects — they are treated as canonical references for style and architecture.

## Purpose

The `SparseLattice` project is an experiment platform for sparse integer spatial indexing and adapters for embedding models. The goals are reproducibility, testable invariants, deterministic builds, and clear CI enforcement so that future automated agents can safely modify and extend code.

## General Principles

- Follow the existing codebase style (see examples in `SpatialDbLib`). Match naming, field prefixes, and architectural patterns rather than introducing new global style changes.
- Prefer clarity and correctness over micro-optimizations; micro-optimizations must be covered with benchmarks and justification in PR description.
- Keep the runtime insertion/build pipeline simple and the query path allocation-free and lock-free after the index is frozen.
- All experiments must be reproducible with test coverage and deterministic outputs (no randomness without an explicit and test-controlled seed).

## Branching and PRs

- Main branch: `master` (protected). All changes must be merged via PR.
- Feature branches: `feature/<short-desc>`, Bugfix: `fix/<short-desc>`, Chore: `chore/<short-desc>`.
- PR must include:
  - Clear description of intent and design rationale.
  - Reference to related issues and tests added/updated.
  - Benchmark results (if performance changes) in `SparseLattice.Test/Benchmarks`.
  - Checklist confirming all CI steps pass locally.

## Commit Messages

- Use Conventional Commits style: `feat:`, `fix:`, `chore:`, `docs:`, `test:`, `refactor:`, `perf:`.
- Single logical change per commit.

## Code Style

These rules must be enforced by the AI agent when generating or mutating code:

- Use 4 spaces for indentation.
- End files with a single newline.
- Maximum line length: 120 characters.
- Encoding: UTF-8.
- Private instance fields MUST use the `m_` prefix (e.g. `m_root`, `m_firstChild`) to match the reference code.
- Private readonly fields: `m_` prefix and `readonly` where appropriate.
- Property and type names use PascalCase; local variables and parameters use camelCase.
- Constants: PascalCase or ALL_CAPS? Use PascalCase for named constants.
- Avoid `var` for publicly visible fields or when the type is not obvious; local `var` ok when clear.
- Favor `readonly struct` for small value types (e.g. `LongVectorN`).
- Use explicit accessibility modifiers on all types and members.
- Prefer immutable data structures where possible for indexes after the freeze phase.

## Testing Requirements

Every change must include tests that validate both functional behavior and invariants. Tests should be deterministic and fast. Use the `SparseLattice.Test` project for unit and integration tests.

Minimum requirements per change:

1. Unit tests that validate the API surface and edge cases (null, empty, single-item, boundary values).
2. Invariant tests that assert structural guarantees (e.g. node sparsity, no pre-allocated children arrays, leaf capacity limits, monotonic ordering where applicable).
3. Round-trip tests for serialization/persistence (if implemented) that assert the frozen index round-trips exactly.
4. Integration tests for correctness against small, hand-crafted datasets where expected nearest neighbors are known.
5. Benchmark tests (separate folder) when performance changes are made; include before/after and a brief analysis.

Tests must run headlessly and pass in CI.

## CI Expectations

- CI must run:
  - Build for .NET 8 (SparseLattice) and test runner for test project.
  - Unit tests with coverage report.
  - Optional benchmarks run in a separate job.
- Tests must pass and coverage must not regress for changed modules. Aim for >= 80% coverage for new modules; critical modules should have near 100% invariants coverage.

## API Stability and Invariants

- Respect the following invariants (each must be covered by tests):
  1. After build/freeze, no mutating operations on the index are allowed — queries must be allocation-free.
  2. Sparse nodes contain only realized children; no pre-allocated arrays of size 2^N.
  3. The quantizer/adapter must be reversible within the configured threshold (quantize → dequantize → re-quantize yields same key).
  4. Distance computations for integer coordinates must be deterministic and overflow-safe.
  5. Any internal linked lists used for realized children must be convertible to compact arrays during `Freeze()`.

## Performance and Benchmarks

- Use `SparseLattice.Test/Benchmarks` for microbenchmarks. Benchmarks are required for any perf-sensitive change.
- Benchmarks must include representative workloads (random sparse vectors, clustered vectors, high-sparsity cases).

## Style and Architecture Review

- Do not alter `SpatialDbLib` or other existing projects. Use them as reference for architecture and coding style.
- AI agents must generate code that compiles and passes all tests locally before opening PRs.

## Documentation

- Every public type and major algorithm must have XML doc comments.
- High-level design documents, rationale and tradeoffs must live in `SparseLattice/docs/`.

## Security & Licensing

- Respect existing project license. Do not introduce third-party code incompatible with the repository license without explicit approval.

## How AI Agents Should Operate

- Always run `dotnet test` and `dotnet build` locally before proposing code changes.
- Produce minimal diffs and comprehensive tests.
- When unsure about a design choice, open an issue and include the design alternatives in the PR.

---

Appendix: Quick checklist for PRs

- [ ] Build passes: `dotnet build` for solution.
- [ ] Unit tests pass: `dotnet test`.
- [ ] New tests added and old tests updated as needed.
- [ ] Invariant tests added or updated.
- [ ] Benchmarks added when needed.
- [ ] XML docs for public surface.
- [ ] PR description includes rationale and references.
- [ ] CI config updated if required.
