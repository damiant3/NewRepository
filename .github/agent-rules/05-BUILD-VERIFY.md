# Build and Verify

Every task that touches source code must end with a green build and passing tests.
No exceptions.

---

## The Two Commands

```sh
dotnet build Codex.sln    # Must produce zero warnings (warnings are errors)
dotnet test Codex.sln     # All tests must pass
```

Run both before:
- Concluding any task
- Creating a commit
- Writing a handoff document that says "done"

---

## What "Zero Warnings" Means

`TreatWarningsAsErrors` is `true` in `Directory.Build.props`. This means:

- An unused field is a build failure.
- An unused variable is a build failure.
- An unused parameter is a build failure.
- A missing `using` is a build failure.
- A redundant `using` is a build failure (in some configurations).

Do not leave dead code. If you add a field, use it. If you remove a usage, remove the field.

---

## Test Expectations

The test suite has 722+ tests across multiple projects:

| Test Project | Covers |
|-------------|--------|
| `Codex.Core.Tests` | Content hashing, Map, diagnostics |
| `Codex.Syntax.Tests` | Lexer, parser, prose parser |
| `Codex.Ast.Tests` | Desugarer |
| `Codex.Semantics.Tests` | Name resolution |
| `Codex.Types.Tests` | Type checker, unifier, integration tests |
| `Codex.Repository.Tests` | Fact store, collaboration, sync |

If you add a new feature, add tests. If you fix a bug, add a regression test.

---

## Quick Checklist

Before declaring any code task complete:

- [ ] `dotnet build Codex.sln` passes (zero warnings)
- [ ] `dotnet test Codex.sln` passes (all tests green)
- [ ] Private fields use the `m_` prefix
- [ ] No `var` where the type is non-obvious
- [ ] No XML doc comments added
- [ ] No unused fields, variables, or parameters
- [ ] `.bak` and `.new` temp files cleaned up

---

## Bootstrap Verification (When Touching Self-Hosted Pipeline)

If your changes affect `Codex.Codex/` or the bootstrap pipeline:

```sh
dotnet run --project tools/Codex.Bootstrap -- build Codex.Codex
```

Then verify that `Codex.Codex/stage1-output.cs` compiles and the output is
functionally equivalent to `Codex.Codex/out/Codex.Codex.cs`.
