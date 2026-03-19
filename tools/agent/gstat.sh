#!/bin/bash
# gstat.sh — Git status + recent log in one shot.
# Bash equivalent of tools/agent/gstat.ps1
#
# Usage: bash tools/agent/gstat.sh

set -e

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$REPO_ROOT"

BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "unknown")
HASH=$(git rev-parse --short HEAD 2>/dev/null || echo "?")
DIRTY=$(git status --porcelain 2>/dev/null)
DIRTY_COUNT=$(echo "$DIRTY" | grep -c . 2>/dev/null || echo 0)
AHEAD=$(git rev-list --count "origin/$BRANCH..HEAD" 2>/dev/null || echo "?")
BEHIND=$(git rev-list --count "HEAD..origin/$BRANCH" 2>/dev/null || echo "?")

echo "═══ GIT STATUS ═══"
echo ""
echo "  Branch:  $BRANCH @ $HASH"
echo "  Remote:  ↑$AHEAD ahead  ↓$BEHIND behind"

if [ "$DIRTY_COUNT" -gt 0 ]; then
    echo "  Dirty:   $DIRTY_COUNT file(s)"
    echo ""
    echo "── Modified files ──"
    echo "$DIRTY" | head -20
    [ "$DIRTY_COUNT" -gt 20 ] && echo "  ... and $((DIRTY_COUNT - 20)) more"
else
    echo "  Dirty:   clean"
fi

echo ""
echo "── Recent commits ──"
git log --oneline -10 2>/dev/null

# Show feature branches
echo ""
echo "── Feature branches ──"
git branch -a 2>/dev/null | grep -E "windows/|linux/|staging/" | head -10 || echo "  (none)"

echo ""
echo "── Today: $(date +%Y-%m-%d) ──"
