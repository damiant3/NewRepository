#!/bin/bash
# trun.sh — Test runner with filtered, non-truncated output.
# Bash equivalent of tools/agent/trun.ps1
#
# Usage:
#   bash tools/agent/trun.sh                     — run all tests, summary only
#   bash tools/agent/trun.sh -p Types            — run tests matching project name
#   bash tools/agent/trun.sh -f "Linear"         — run tests matching display name
#   bash tools/agent/trun.sh --full              — show all output (no filter)

set -e

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$REPO_ROOT"

PROJECT=""
FILTER=""
FULL=false
TMPFILE="/tmp/codex-test-output.txt"

while [ $# -gt 0 ]; do
    case "$1" in
        -p|--project) PROJECT="$2"; shift 2 ;;
        -f|--filter)  FILTER="$2"; shift 2 ;;
        --full)       FULL=true; shift ;;
        *)            echo "Unknown arg: $1" >&2; exit 1 ;;
    esac
done

# Build the dotnet test command
CMD="dotnet test Codex.sln --no-build -v q"

if [ -n "$FILTER" ]; then
    CMD="$CMD --filter \"DisplayName~$FILTER\""
fi

# Run and capture
echo "Running: $CMD"
echo ""
eval "$CMD" > "$TMPFILE" 2>&1 || true

if $FULL; then
    cat "$TMPFILE"
else
    # Extract failures and summary
    FAILURES=$(grep -A2 "Failed " "$TMPFILE" 2>/dev/null || true)
    SUMMARY=$(grep -E "(Passed|Failed|Skipped|Total)" "$TMPFILE" | tail -20)

    if [ -n "$PROJECT" ]; then
        # Filter to matching project lines
        SUMMARY=$(grep -i "$PROJECT" "$TMPFILE" | grep -E "(Passed|Failed)" || echo "(no matches for project '$PROJECT')")
    fi

    if [ -n "$FAILURES" ]; then
        echo "═══ FAILURES ═══"
        echo "$FAILURES"
        echo ""
    fi

    echo "═══ SUMMARY ═══"
    echo "$SUMMARY"

    # Count
    PASSED=$(grep -oP 'Passed:\s+\K\d+' "$TMPFILE" 2>/dev/null | awk '{s+=$1}END{print s+0}')
    FAILED=$(grep -oP 'Failed:\s+\K\d+' "$TMPFILE" 2>/dev/null | awk '{s+=$1}END{print s+0}')
    echo ""
    echo "Total passed: $PASSED   Failed: $FAILED"
fi

rm -f "$TMPFILE"
