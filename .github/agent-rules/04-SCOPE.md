# Scope of Authority

What agents are allowed to modify, and what is off-limits.

---

## Freely Modifiable

Agents may create, edit, and delete files in these locations without special permission:

| Location | Contains |
|----------|----------|
| `src/` | All compiler source projects |
| `tests/` | All test projects |
| `tools/` | CLI, bootstrap, VS extension |
| `samples/` | Sample `.codex` programs |
| `editors/` | VS Code extension |
| `generated-output/` | Regenerated backend output corpus |
| `Codex.Codex/` | Self-hosted compiler source and output |
| `.github/agent-rules/` | These rule files (with user approval for policy changes) |

---

## Modifiable With Care

These files affect the whole repository. Edit only when the task requires it, and
verify the build afterward:

| File | What it governs |
|------|-----------------|
| `README.md` | Project overview — keep accurate |
| `CONTRIBUTING.md` | Contributor rules — keep in sync with agent rules |
| `.github/copilot-instructions.md` | Root agent instructions (references agent-rules/) |
| `copilot-instructions.md` | Legacy root instructions (keep in sync) |
| `.gitignore` | Ignored files — add patterns as needed |
| `Codex.sln` | Solution file — add new projects as needed |

---

## Do Not Modify (Without Explicit User Request)

| File(s) | Why |
|---------|-----|
| `docs/00-OVERVIEW.md` through `docs/10-PRINCIPLES.md` | Architecture specification / north-star design docs |
| `Directory.Build.props` | Governs the entire solution build (TreatWarningsAsErrors, TFM, etc.) |
| `docs/Vision/NewRepository.txt` | Original vision document |
| `docs/Vision/IntelligenceLayer.txt` | Design philosophy essay |

If you believe one of these files needs updating, ask the user first and explain why.

---

## Handoff and Status Documents

Agents **should** create and update handoff documents in `docs/OldStatus/` or `docs/`
after meaningful work. Use the `checkdate()` rule for all dates.

| File pattern | Purpose |
|-------------|---------|
| `docs/OldStatus/ITERATION-N-HANDOFF.md` | Per-iteration summary |
| `docs/OldStatus/FORWARD-PLAN.md` | Single source of truth for project direction |
| `docs/OldStatus/DECISIONS.md` | Architecture decision log |
| `docs/PostFixedPointCleanUp.md` | Post-bootstrap cleanup tracker |
