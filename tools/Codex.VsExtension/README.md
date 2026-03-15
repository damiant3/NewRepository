# Codex Language Extension

Syntax highlighting and language support for `.codex` files in Visual Studio 2022 and VS Code.

## Features

- **Syntax highlighting** for all Codex constructs:
  - Keywords: `let`, `in`, `if`, `then`, `else`, `when`, `where`, `do`, `record`, `import`, `export`, `linear`
  - Proof keywords: `claim`, `proof`, `forall`, `exists`, `induction`, `assume`
  - Types: `Integer`, `Text`, `Number`, `Boolean`, `List`, `Shape`, etc. (any capitalized identifier)
  - Boolean literals: `True`, `False`
  - Strings: `"hello"` and `"""raw blocks"""`
  - Numbers: `42`, `3.14`, `1_000_000`
  - Operators: `=`, `→`, `->`, `++`, `::`, `==`, `===`, `|`, etc.
  - Annotations: `@annotation-name`
  - Prose headers: `Chapter:` and `Section:`
- **Bracket matching** for `()`, `[]`, `{}`
- **Auto-closing pairs** for brackets and strings
- **Smart indentation** for `let`, `if`, `when`, `do`, `where` blocks

## Install in Visual Studio 2022

### Quick install (local dev)

```powershell
pwsh -File install-vs.ps1
```

Then restart Visual Studio.

### VSIX install

```powershell
pwsh -File build-vsix.ps1
```

Then double-click `out/CodexLanguage.vsix`.

## Install in VS Code

Copy this folder to your VS Code extensions directory:

```powershell
# Windows
Copy-Item -Recurse . "$env:USERPROFILE\.vscode\extensions\codex-language"

# macOS / Linux
cp -r . ~/.vscode/extensions/codex-language
```

Then restart VS Code.

## Color mapping

| Codex construct | TextMate scope | Typical color |
|----------------|----------------|---------------|
| `let`, `if`, `when`, etc. | `keyword.control` | Purple/blue |
| `claim`, `proof`, `forall` | `keyword.other.proof` | Purple |
| `Integer`, `Text`, `Shape` | `entity.name.type` | Teal/green |
| `True`, `False` | `constant.language.boolean` | Blue |
| `"hello"` | `string.quoted.double` | Orange/brown |
| `42`, `3.14` | `constant.numeric` | Light green |
| `=`, `→`, `++` | `keyword.operator` | Gray/white |
| `@annotation` | `entity.name.tag` | Yellow |
| `Chapter:`, `Section:` | `keyword.control.header` | Bold purple |
| `my-function` | `variable.other` | Default text |
