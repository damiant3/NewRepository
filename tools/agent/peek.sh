#!/bin/bash
# peek.sh — Reliable file reader with line numbers.
# Bash equivalent of tools/agent/peek.ps1
#
# Usage:
#   bash tools/agent/peek.sh <file>                — first 50 lines
#   bash tools/agent/peek.sh <file> 10 30          — lines 10-30
#   bash tools/agent/peek.sh <file> 0 0            — full file with line count

set -e

if [ -z "$1" ]; then
    echo "Usage: peek.sh <file> [startLine] [endLine]" >&2
    exit 1
fi

FILE="$1"
START="${2:-1}"
END="${3:-50}"

if [ ! -f "$FILE" ]; then
    echo "File not found: $FILE" >&2
    exit 1
fi

TOTAL=$(wc -l < "$FILE")

# Full file mode: 0 0
if [ "$START" -eq 0 ] && [ "$END" -eq 0 ]; then
    echo "── $FILE ($TOTAL lines) ──"
    nl -ba -w6 "$FILE"
    echo "── EOF ($TOTAL lines) ──"
    exit 0
fi

# Clamp range
[ "$START" -lt 1 ] && START=1
[ "$END" -gt "$TOTAL" ] && END=$TOTAL

echo "── $FILE  lines ${START}–${END} of ${TOTAL} ──"
sed -n "${START},${END}p" "$FILE" | nl -ba -w6 -v "$START"
echo "── (showing ${START}–${END} of ${TOTAL}) ──"
