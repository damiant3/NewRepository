# Known Conditions

Persistent record of known build/test conditions that agents should NOT re-investigate.
Read by `codex-agent doctor`. Last updated: 2026-03-21.

## Build

### CS5001 in Codex.Codex — PRE-EXISTING, IGNORE

`Codex.Codex.csproj` emits `CS5001: Program does not contain a static 'Main' method
suitable for an entry point`. This is **expected** — the project is a self-hosted Codex
compiler compiled by the Codex toolchain, not by C#. The csproj has `<NoWarn>CS5001</NoWarn>`
but CS5001 is an error, not a warning, so it isn't suppressed.

**Action**: When verifying builds, exclude `Codex.Codex` or accept 1 CS5001 error from it.
Use `dotnet build Codex.sln` and check that the ONLY error is this one.

## Tests

### Peek_non_numeric_start_does_not_crash — KNOWN BUG, DOCUMENTED

`Codex.AgentToolkit.Tests.CodexAgentExeTests.Peek_non_numeric_start_does_not_crash`
may fail because `text-to-integer` throws `FormatException` on non-numeric input.
The test is written to accept either exit code 0 or a FormatException in stderr.

**Action**: Do not investigate this failure. It is a known limitation of `parse-int-or`
using the raw `text-to-integer` builtin which lacks error handling.

## Agent Toolkit

### IL emitter — user-defined effectful helper functions

Calling user-defined functions with effectful return types (e.g., `[FileSystem] Nothing`)
from other effectful functions via `let _ = myHelper args` can produce
`InvalidProgramException: Common Language Runtime detected an invalid program`.

**Action**: Inline effectful logic (use `write-file` / `read-file` directly) instead of
extracting effectful helper functions. This is an IL emitter limitation, not a Codex
language issue.

### Codex.Codex.csproj in `dotnet test`

When running `dotnet test Codex.sln`, the `Codex.Codex` project will fail to build
(CS5001, see above). Test results from all other projects are valid. The overall test
command may report "build failed" but individual test project results are trustworthy.

## How to Use This File

Agents should run `codex-agent doctor` at session start. The doctor command checks
for these known conditions and prints a compact briefing so the agent doesn't waste
context re-investigating them.
