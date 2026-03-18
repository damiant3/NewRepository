# Terminal Discipline

Agents interact with the system through terminal commands. Each platform has its own
failure modes. These rules prevent the most common ones.

---

## Windows Agent (Copilot / VS Terminal / PowerShell)

### Never Run Multi-Line Scripts Inline

Multi-line PowerShell scripts pasted into the agent terminal hang waiting for input
the agent cannot provide. Always:

1. Write the script to a `.ps1` file using `create_file`.
2. Invoke it with `powershell -File <script.ps1>` (or `pwsh -File` if PowerShell 7+ is installed).
3. Delete the `.ps1` file when done.

### No Write-Output / Write-Host

`Write-Output`, `Write-Host`, and bare expressions are unreliable in the agent terminal.
Instead, write results to a temp file and read it back, or use `create_file` / `edit_file`.

### Hung Commands

If a terminal command takes more than a few seconds, assume it is hung. Kill it and
switch to a file-based approach.

### Allowed Terminal Uses

The terminal is for **read-only queries and build invocations only**:

```powershell
dotnet build Codex.sln
dotnet test Codex.sln
Select-String -Pattern "foo" -Path src/**/*.cs
Get-ChildItem -Recurse -Filter *.codex
git log --oneline -20
git status
git diff --stat
```

---

## Linux Agent (Claude / Bash)

### Prefer Simple Commands

Stick to one-liners where possible. For complex operations, write a bash script,
execute it, and clean up.

### Allowed Terminal Uses

```bash
dotnet build Codex.sln
dotnet test Codex.sln
grep -r "pattern" src/
find . -name "*.codex" -type f
git log --oneline -20
git status
git diff --stat
date +%Y-%m-%d    # checkdate()
```

### Long-Running Commands

If a command takes more than 30 seconds, it is probably wrong. Check:
- Are you building the entire solution when you only need one project?
- Are you running all tests when you only need one test class?

---

## Both Platforms

### Prefer File Tools Over Terminal for Mutations

- Use `edit_file` / `create_file` (Copilot) or `str_replace` / `create_file` (Claude)
  for all file modifications.
- The terminal is for reading, building, and testing — not for writing files via
  `echo`, `Set-Content`, or redirects.

### Git Commands in Terminal

Git commands are read-only by default. For write operations (commit, push, checkout),
see the [Git Workflow rules](07-GIT-WORKFLOW.md).
