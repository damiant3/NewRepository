# Setting Up the Codex Extension in VS Code

This guide gets you from nothing to syntax highlighting, error squiggles, hover types,
go-to-definition, and completion for `.codex` files in Visual Studio Code.

---

## Prerequisites

You need three things installed before starting.

### 1. .NET 8 SDK
The LSP server is a .NET executable. Check if it is already installed:
```
dotnet --version
```
If the output is `8.x.x` or higher you are fine. Otherwise download it from
https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Node.js (LTS)
The extension build step uses `npm`. Check:
```
node --version
npm --version
```
If not installed, download the LTS release from https://nodejs.org.

### 3. Visual Studio Code
Download from https://code.visualstudio.com if you do not have it.

---

## One-time setup: build the extension

The VS Code extension lives in `editors\vscode` **inside the repo root** (not in any project `bin` folder).
Open a terminal at the repo root and run:

```
cd editors\vscode
npm install
npm run compile
```

`npm install` downloads the TypeScript compiler and the VS Code language client library.
`npm run compile` turns the TypeScript source in `src/extension.ts` into `out/extension.js`,
which is what VS Code actually loads. You only need to do this once (or again if `extension.ts` changes).

---

## Installing the extension into VS Code

VS Code extensions are installed from `.vsix` packages, or you can load an extension folder
directly in "development mode", which is what we do here.

### Option A — Development mode (quickest, no packaging needed)

1. Open VS Code.
2. Press `Ctrl+Shift+P` to open the Command Palette.
3. Type **"Extensions: Install from VSIX..."** — but do **not** click that yet.
   Instead, type **"Developer: Install Extension from Location..."** and press Enter.
4. Browse to `editors\vscode` inside the repo root and click **Select Folder**.
5. VS Code will ask you to reload — click **Reload**.

After reloading, open any `.codex` file. You should see syntax highlighting immediately.

> **Tip:** You can also open the repo root as a workspace folder (`File → Open Folder`) and
> VS Code will automatically find the extension via the `editors/vscode` folder if you add
> it to your workspace.

### Option B — Package and install (more permanent)

```
cd editors\vscode
npm install -g @vscode/vsce      # install the VS Code packaging tool once
vsce package                     # creates codex-lang-0.1.0.vsix
```

Then in VS Code: `Ctrl+Shift+P` → **"Extensions: Install from VSIX..."** → select the generated `.vsix` file.

---

## How the language server starts

When you open a `.codex` file, the extension automatically starts the Codex LSP server in the
background. By default it does this by running:

```
dotnet run --project <repo>\src\Codex.Lsp\Codex.Lsp.csproj --no-build
```

This means the first activation takes a few seconds while .NET starts up. After that it is
fast because the server stays running until you close VS Code.

> **If the server fails to start**, make sure you have built the solution first:
> ```
> dotnet build Codex.sln
> ```
> The `--no-build` flag means the extension will not recompile for you.

### Using a pre-built executable (optional, faster startup)

If you want instant startup instead of `dotnet run`, publish the server as a self-contained executable:

```
dotnet publish src\Codex.Lsp\Codex.Lsp.csproj -c Release -r win-x64 --self-contained -o out\lsp
```

Then tell the extension where to find it via VS Code settings.
Open `File → Preferences → Settings`, search for **codex.serverPath**, and set it to the
full path of the produced executable, e.g.:

```
D:\Projects\NewRepository\out\lsp\Codex.Lsp.exe
```

---

## What you get

| Feature | How to trigger |
|---------|----------------|
| Syntax highlighting | Automatic on `.codex` files |
| Error squiggles | Appear as you type (on save or change) |
| Hover — show type | Hover the mouse over any name |
| Go to definition | `F12` or right-click → **Go to Definition** |
| Peek definition | `Alt+F12` |
| Completion | `Ctrl+Space` — shows definitions, builtins, keywords |
| Document outline | Click the **Outline** panel in the Explorer sidebar |

---

## Troubleshooting

**I see no syntax highlighting.**
The extension did not activate. Make sure the file has a `.codex` extension. Check the bottom
status bar — it should say "Codex" as the language mode. If it says "Plain Text", click it and
type `codex` to change it manually, then check that the extension installed correctly.

**I see highlighting but no squiggles or hover.**
The language server is not running. Open `View → Output` (`Ctrl+Shift+U`), select
**Codex Language Server** from the dropdown, and look for error messages.
The most common cause is the solution has not been built — run `dotnet build Codex.sln`.

**The Output panel says "spawn dotnet ENOENT".**
`dotnet` is not on your PATH. Restart VS Code after installing the .NET SDK, or set
`codex.serverPath` to the full path of a pre-built executable as described above.

**I get an error about a missing project file.**
Make sure you opened the repo root folder in VS Code (`File → Open Folder → NewRepository`),
not a subfolder. The extension finds the server project relative to the workspace root.
