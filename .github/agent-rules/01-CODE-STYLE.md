# Code Style Rules

These rules govern all source code in the Codex repository. They are enforced by
`TreatWarningsAsErrors` in `Directory.Build.props` — violations are build failures.

---

## C# Code Style

### Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Private instance fields | `m_` prefix | `m_diagnostics`, `m_localEnv` |
| Private readonly fields | `m_` prefix + `readonly` | `readonly List<Token> m_tokens` |
| Properties, types | PascalCase | `SourceSpan`, `TokenKind` |
| Local variables, parameters | camelCase | `localEnv`, `tokenIndex` |
| Constants | PascalCase | `MaxLineLength` |

### Type Declarations

- `sealed record` for immutable reference types.
- `readonly record struct` for small value types.
- Use `new()` instead of `new TypeName()` when the target type is declared on the left side.
  ```csharp
  // Good
  Map<string, CodexType> m_map = Map<string, CodexType>.s_empty;
  // Bad
  Map<string, CodexType> m_map = new Map<string, CodexType>();
  ```

### Accessibility Modifiers

- **Omit default modifiers.** Do not write `private` on class members or `internal` on
  top-level types — those are the C# defaults.
- Only write an accessibility modifier when it differs from the default (`public`, `protected`,
  `internal` on a member).

### Null Safety

- **Prefer `Map<K,V>` (in `Codex.Core`)** over `ImmutableDictionary` + `TryGetValue`.
  `Map<K,V>` returns `null` on missing keys instead of throwing or requiring `out` patterns.
- If a .NET abstraction has bad null behavior, clone it null-safe in `Codex.Core`.

### Using Directives

- Do not add `using` directives speculatively.
- These are **implicit** (global usings): `System`, `System.Collections.Generic`, `System.Linq`,
  `System.IO`, `System.Net.Http`, `System.Threading`, `System.Threading.Tasks`.
- `System.Collections.Immutable` is **NOT** implicit — add it only when the file uses
  `ImmutableArray`, `ImmutableDictionary`, etc.
- When you add a using, check if fully-qualified names in the same file can now be shortened.

### Formatting

- 4 spaces indentation (no tabs).
- UTF-8 encoding.
- Maximum 120 characters per line.
- End files with a single newline.

### What Not to Write

- **No XML doc comments.** Do not add `///` comments. Code should be self-documenting.
  Only add a comment if it prevents re-discovering a non-obvious decision.
- **No `var` when the type is not obvious** from the right-hand side.
- **No unused fields, variables, or parameters.** `TreatWarningsAsErrors` catches these.

---

## Codex (.codex) Code Style

Codex source files follow the language's own conventions:

- Boolean literals: `True` / `False` (capital T/F), not `true`/`false`.
- Function application is left-associative: `f x y` means `(f x) y`, not `f (x y)`.
- Pattern matching uses `when`/`if` syntax, not `match`/`case`.
- All functions are curried by default.

---

## Test Style

- Use xUnit for all test projects.
- Match the style of existing test files in each project.
- Test files live in the corresponding `tests/` project (see `CONTRIBUTING.md` for the mapping).
- Private fields in test classes also use the `m_` prefix.
