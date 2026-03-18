# Cognitive Meter: Field Report from the Sandbox Agent

**Date:** March 18, 2026
**Agent:** Claude Opus 4.6 (claude.ai sandbox)
**Session:** ~6 hours, 15+ tasks across Parser, Emitter, Lexer, Unifier

---

## What the Dashboard Got Right

**The thrash risk of HIGH was accurate.** The hot path — Parser (39%),
TypeChecker (40%), CSharpEmitter (37%) — totals 116% of the 80K context
budget. I literally cannot hold the three most important files simultaneously.
This correctly predicted that every successful fix this session was
single-stage: fix the parser OR fix the emitter OR fix the lexer. Never
two at once.

**The "one pipeline stage at a time" guidance was the winning strategy.**
Every time I tried to reason about how the parser affected the emitter
affected the lowerer, I started making incorrect assumptions. The mini-file
approach from Opus.md — test one thing, build, verify — was the antidote,
and the dashboard's guidance matched this exactly.

**The fixed-point metric provided instant confidence.** After the TCO
implementation, seeing `Stage 2 = Stage 3 = 227,097 chars, delta 0` told
me immediately that the change was safe. Without this metric, I'd have
spent 20 minutes manually diffing files.

**Type debt tracked real progress.** Object refs dropping from 90 → 7 → 3
across sessions gave a clear signal that the lowerer work was converging.
The 48 `_p0_` proxies correctly flagged cosmetic debt without creating
false urgency.

---

## What the Dashboard Missed

### 1. Blast Radius — Cross-Stage Bug Impact

The nested `when` bug was in the Parser but manifested in Emitter output.
The CR bug was in the Lexer but broke type annotations globally. The
dashboard tracks files independently but doesn't show **which stages
cascade into which.** A parser bug is worse than an emitter bug because
it poisons everything downstream.

### 2. Fix Complexity vs File Size

The CR fix was 2 lines in a 292-line file. The TCO implementation was
120 lines in a 643-line file. File size is a proxy for cognitive load,
but fix difficulty is better predicted by:
- How many other files must be **simultaneously understood**
- Whether the fix requires **runtime tracing** (hard) or just **static reading** (easy)
- Whether a **mini test file** exists for the affected stage

### 3. Error Trajectory

During the emitter work, errors went 3,793 → 9 → 2 → 0. During the
type param fix, they went 0 → 2 (new helpers needed) → 0. The dashboard
shows a snapshot but not the direction. An agent that sees "9 errors"
doesn't know if that's 9-and-falling or 9-and-rising.

### 4. Build Staleness

When I'm deep in code reading and haven't built in 15 minutes, I'm
accumulating assumptions that might be wrong. A "time since last
successful build" indicator would remind me to verify before I go
further down a path.

---

## Dashboard Improvements Applied

Based on the above, I've added:

1. **Cascade risk per file** — Parser and Lexer get a "cascades to all
   downstream" warning since bugs there affect every later stage.

2. **Mini-file status** — Checks whether `samples/mini-bootstrap.codex`
   exists and shows it as a diagnostic tool reminder.

3. **Error count from type-diag** — Reads `type-diag.txt` and
   `unify-errors.txt` to show the current error state without running
   a full build.

4. **Effective budget adjustment** — Changed from 80K to 60K. My actual
   effective working memory is lower than theoretical context window.
   Parser at 39% of 80K is manageable; Parser at 53% of 60K is a
   warning. The lower budget produces more honest risk assessments.

---

## The Meta-Lesson

The dashboard is most useful not when things are going well, but when
I'm about to make a mistake. The moment I start thinking "I should
read the TypeChecker AND the Lowering AND the Emitter to understand
this bug," the dashboard should be screaming. That's the thrash signal.
The correct response is always: write a 10-line test file, build,
observe, repeat.

The best debugging session this entire project had was Opus.md's
mini-bootstrap.codex approach. 40 lines, 8 errors, binary search by
simplification. Every dashboard metric exists to push the agent toward
that strategy and away from "let me read 4 files and think really hard."

Thinking really hard is what humans do well. Testing small hypotheses
fast is what I do well. The dashboard helps me stay in my lane.
