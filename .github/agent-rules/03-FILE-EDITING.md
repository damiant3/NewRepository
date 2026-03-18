# File Editing Rules

File editing tools are the primary way agents modify code. They are also the primary
source of silent corruption. These rules exist because real bugs have been caused by
careless edits.

---

## Golden Rule: Read Before You Edit

**Always read a file before editing it**, unless you just created it in this session.

---

## Backup Before Editing

Always back up a file locally before making non-trivial edits. The file-edit tool
occasionally corrupts content (renames variables, swaps function names, truncates files).
A `.bak` copy lets you recover in seconds instead of rewriting from scratch.

```
original.cs  →  original.cs.bak  →  edit original.cs  →  verify  →  delete .bak
```

Clean up `.bak` files before ending your session.

---

## Small Files (< 100 lines)

`edit_file` / `str_replace` is acceptable. Verify the result immediately after by
reading the file back.

---

## Medium Files (100–300 lines)

Use `edit_file` / `str_replace` with **generous context** — unique lines above and
below the change site so the tool can locate the edit unambiguously. If an edit fails,
re-read the file and provide more context.

---

## Large Files (> 300 lines)

### Copilot (Windows): Write-Full-File Strategy

The `edit_file` tool is unreliable on large files. It silently corrupts unrelated lines.

**Required workflow:**

1. Write the complete new file to `<filename>.new` using `create_file`.
2. Back up the current file: copy `<filename>` to `<filename>.bak`.
3. Swap: copy `<filename>.new` to `<filename>`.
4. Verify: diff against `.bak` to confirm only intended changes.
   Check line count: `$new.Count` should equal `$old.Count + expected delta`.
5. If the build fails: restore from `.bak`, inspect, and retry.
6. Clean up `.bak` and `.new` files when done.

### Partial Class Strategy

When a file exceeds ~300 lines and you need to add multiple methods, **use a partial
class file**. Create a second file (e.g., `Program.Collaboration.cs`) with `partial class`
containing the new methods. This keeps edits small and avoids the large-file corruption
problem. Merge back when stable.

### Claude (Linux): Iterative str_replace

Claude's `str_replace` tool requires exact unique string matches. For large files:

1. Use `view` with line ranges to inspect the area you need to change.
2. Apply `str_replace` with the exact string (no line-number prefixes).
3. Re-view the file after each edit — earlier view output is stale.
4. For multi-site edits, work top-to-bottom (earlier line numbers stay stable).

---

## What Never to Do

- **Never print a full file as a code block** and ask the user to paste it.
- **Never use terminal redirects** (`>`, `Set-Content`) for file creation when
  file-creation tools are available.
- **Never retry a failed edit approach.** If `edit_file` / `str_replace` fails,
  re-read the file and use a different strategy.
