# Git Workflow — Who Watches the Watcher?

Two agents work on the Codex repository: a **Windows agent** (Copilot in VS) and a
**Linux agent** (Claude on claude.ai). Neither agent commits directly to `master`.
Instead, they use a branch-based workflow where each agent can review the other's
work before it lands.

---

## The Principle

> No agent merges its own work to master without review.

This is the recursive answer to "who watches the watcher?" — they watch each other.

---

## Branch Naming

| Branch | Purpose |
|--------|---------|
| `master` | Protected. Only receives merges from reviewed staging branches. |
| `windows/<topic>` | Windows agent's working branch |
| `linux/<topic>` | Linux agent's working branch |
| `staging/<topic>` | Reviewed and approved work ready for merge |

Examples:
- `windows/fix-lowerer-inference`
- `linux/rewrite-agent-rules`
- `staging/agent-rules-v2`

---

## The Workflow

### 1. Agent Creates a Working Branch

```bash
git checkout master
git pull origin master
git checkout -b windows/my-feature    # or linux/my-feature
```

### 2. Agent Does Work and Commits

Commits happen on the working branch. Use Conventional Commits:

```bash
git add -A
git commit -m "feat: add exhaustiveness checking for nested patterns"
```

Agents **may commit freely** to their own working branches. The old "no commit"
restriction is lifted. The safety net is the review step, not a prohibition on commits.

### 3. Agent Pushes and Requests Review

```bash
git push origin windows/my-feature
```

Then leave a note in a handoff document or a PR description explaining:
- What was changed and why
- What tests were added or affected
- Any known issues or things the reviewer should check

### 4. The Other Agent Reviews

The reviewing agent:

1. Fetches the branch: `git fetch origin windows/my-feature`
2. Checks it out or diffs it: `git diff master..origin/windows/my-feature`
3. Reads the changes. Checks for:
   - Build passes (`dotnet build Codex.sln`)
   - Tests pass (`dotnet test Codex.sln`)
   - Code style compliance (see [01-CODE-STYLE.md](01-CODE-STYLE.md))
   - No spec documents modified without permission
   - Dates are correct (see [00-META.md](00-META.md))
   - Commit messages are meaningful
4. If approved: merges to a staging branch or directly to master
5. If issues found: leaves notes in a review document for the original agent

### 5. Merge to Master

```bash
git checkout master
git merge --no-ff staging/my-feature -m "merge: agent-rules-v2 (reviewed by linux agent)"
git push origin master
```

The `--no-ff` flag preserves the branch history so it's clear what was reviewed.

---

## Simplified Flow (Single-Agent Sessions)

When only one agent is active and the user is present to supervise, the workflow
can be simplified:

1. Agent works on a topic branch (`windows/topic` or `linux/topic`).
2. User reviews the diff.
3. User approves or the agent merges with user confirmation.

The full dual-review process is for when both agents are working in parallel or
asynchronously.

---

## Pull Request Alternative (GitHub)

If preferred, agents can use GitHub PRs instead of manual branch review:

1. Agent pushes working branch.
2. Agent creates a PR via `gh pr create` (if GitHub CLI is available) or asks the user to create one.
3. The other agent or the user reviews via the GitHub UI.
4. Merge via the PR.

This provides a permanent record of reviews. The branch-based workflow above is
for cases where PR tooling isn't available to the agent.

---

## Emergency: Direct Commit to Master

In rare cases where a direct commit is necessary (e.g., fixing a broken build that
blocks all other work), an agent may commit directly to master with:

- User approval (explicit)
- A commit message prefixed with `EMERGENCY:` explaining why normal review was skipped
- The other agent should review the emergency commit at the next opportunity

---

## Commit Message Format

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add exhaustiveness checking for nested patterns
fix: correct lowerer type inference for binary expressions
docs: update ITERATION-12-HANDOFF.md
test: add regression test for nested match scoping
chore: clean up .bak files and regenerate output corpus
refactor: extract LinearityChecker into separate file
```

---

## What Gets Committed

- Source code changes (`src/`, `tests/`, `tools/`)
- Documentation updates (`docs/`, `README.md`, `CONTRIBUTING.md`)
- Agent rule updates (`.github/agent-rules/`, `.github/copilot-instructions.md`)
- Sample programs (`samples/`)
- Generated output corpus (`generated-output/`) — in separate commits
- Editor support files (`editors/`)

## What Does NOT Get Committed

- `.bak`, `.new`, `.tmp` files
- Build output (`bin/`, `obj/`)
- User-specific files (`.vs/`, `.user`)
- Local repository data (`.codex/`)
- Node modules (`node_modules/`)
