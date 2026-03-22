# MCP Server Setup — Codex Compiler as an AI Tool

The Codex MCP server lets AI agents (Claude Desktop, VS Code Copilot, and other
MCP-compatible clients) call directly into the Codex compiler pipeline. Instead of
guessing about types or parsing build output, the AI gets structured diagnostics,
type information, and compilation results through the Model Context Protocol.

---

## Prerequisites

1. **.NET 8 SDK** — `dotnet --version` should show `8.x.x` or higher.

---

## VS Code Setup

The repo already includes a `.vscode/mcp.json` that registers the MCP server. When
you open this workspace in VS Code, any MCP-compatible extension (GitHub Copilot,
Continue, etc.) will automatically discover the Codex tools.

The config lives at `.vscode/mcp.json`:

```json
{
  "servers": {
    "codex": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "-v", "q",
        "--project",
        "${workspaceFolder}/tools/Codex.Mcp/Codex.Mcp.csproj"
      ]
    }
  }
}
```

**That's it.** Open the workspace, and the tools are available to any MCP client
running in VS Code. The server auto-builds on first launch (~3–6 seconds).
The `-v q` flag keeps MSBuild output off stdout so it doesn't pollute the
JSON-RPC stream.

> **Important:** The MCP server has to embed the proper dependencies for the AI
> tools to work. That's why you must build the solution first. The `dotnet run`
> in the config above will automatically build if needed.

---

## Claude Desktop Setup (Windows)

The config file location depends on how you installed Claude Desktop:

| Installation | Config path |
|-------------|-------------|
| **Microsoft Store** | `%LOCALAPPDATA%\Packages\Claude_*\LocalCache\Roaming\Claude\claude_desktop_config.json` |
| **Standard installer** | `%APPDATA%\Claude\claude_desktop_config.json` |

To find which one you have, check if the folder exists:

```
dir "%LOCALAPPDATA%\Packages\Claude_*\LocalCache\Roaming\Claude"
```

If that folder exists, you have the Microsoft Store version. Otherwise check `%APPDATA%\Claude`.

### Step 1 — Create or edit the config file

If the file doesn't exist yet, create it. If it already exists, add the `"codex"`
entry to the `"mcpServers"` object.

```json
{
  "mcpServers": {
    "codex": {
      "command": "dotnet",
      "args": [
        "run",
        "-v", "q",
        "--project",
        "D:/Projects/NewRepository/tools/Codex.Mcp/Codex.Mcp.csproj"
      ]
    }
  }
}
```

> **Important:** Use forward slashes (`/`) in the path, or escaped backslashes
> (`\\`). Adjust `D:/Projects/NewRepository` to wherever you cloned the repo.

### Step 2 — Restart Claude Desktop

Close and reopen Claude Desktop completely (quit from the system tray, not just
close the window). On restart, Claude will launch the Codex MCP server automatically.

### Step 3 — Verify

In a Claude Desktop conversation, you should see a hammer/tools icon (🔨) indicating
MCP tools are available. You can ask Claude:

> "Use the codex-check tool to type-check `samples/hello.codex`"

If Claude calls the tool and returns diagnostics, everything is working.

---

## Available Tools

| Tool | Description | Key Arguments |
|------|-------------|---------------|
| `codex-check` | Type-check a `.codex` file, returns diagnostics with spans | `file` (path) |
| `codex-build` | Compile to one or more targets (C#, IL, etc.) | `path`, `targets` (comma-separated) |
| `codex-hover` | Look up the type of a name in a checked file | `file`, `name` |
| `codex-parse` | Parse a file and return token/CST/AST summary | `file`, `mode` (tokens/cst/ast) |

## Available Resources

| Resource | URI | Description |
|----------|-----|-------------|
| Builtins | `codex://builtins` | All built-in functions with type signatures |

---

## Troubleshooting

### "File not found" errors from the tools

The MCP server resolves file paths relative to its working directory. Use absolute
paths, or paths relative to the repo root.

### Claude Desktop doesn't show the tools icon

- Check that `claude_desktop_config.json` is valid JSON (no trailing commas).
- Check that the path to `Codex.Mcp.csproj` is correct and the solution is built.
- Fully restart Claude Desktop (system tray → Quit, then reopen).
- Check Claude Desktop's developer logs: `Help → Developer → Open Logs`.

### "spawn dotnet ENOENT"

`dotnet` is not on your PATH. Either:
- Restart Claude Desktop / VS Code after installing the .NET SDK.
- Use the full path to `dotnet.exe` in the config, e.g.:
  ```json
  "command": "C:/Program Files/dotnet/dotnet.exe"
  ```

### Server starts but tools return errors

Make sure the solution is built:
```
dotnet build Codex.sln
```
The `--no-build` flag means the MCP server expects pre-compiled binaries.

---

## Using a Pre-Built Executable (Optional, Faster Startup)

For faster startup (skipping `dotnet run`), publish the MCP server:

```
dotnet publish tools/Codex.Mcp/Codex.Mcp.csproj -c Release -r win-x64 --self-contained -o out/mcp
```

Then update the configs to point directly at the executable:

**VS Code** (`.vscode/mcp.json`):
```json
{
  "servers": {
    "codex": {
      "type": "stdio",
      "command": "${workspaceFolder}/out/mcp/Codex.Mcp.exe",
      "args": []
    }
  }
}
```

**Claude Desktop** (`claude_desktop_config.json`):
```json
{
  "mcpServers": {
    "codex": {
      "command": "D:/Projects/NewRepository/out/mcp/Codex.Mcp.exe",
      "args": []
    }
  }
}
```

This gives near-instant startup since there's no `dotnet run` overhead.
