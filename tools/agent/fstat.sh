#!/bin/bash
# fstat.sh — Quick file stats: line count, char count, size.
# Bash equivalent of tools/agent/fstat.ps1
#
# Usage:
#   bash tools/agent/fstat.sh <file>
#   bash tools/agent/fstat.sh src/Codex.Types/*.cs
#   bash tools/agent/fstat.sh Codex.Codex/**/*.codex

set -e

if [ -z "$1" ]; then
    echo "Usage: fstat.sh <file|glob> [file2] ..." >&2
    exit 1
fi

TOTAL_LINES=0
TOTAL_CHARS=0
TOTAL_BYTES=0
COUNT=0

printf "%-55s %7s %8s %9s\n" "File" "Lines" "Chars" "Bytes"
printf "%-55s %7s %8s %9s\n" "$(printf '─%.0s' {1..55})" "-------" "--------" "---------"

for pattern in "$@"; do
    for f in $pattern; do
        [ ! -f "$f" ] && continue
        lines=$(wc -l < "$f")
        chars=$(wc -m < "$f")
        bytes=$(wc -c < "$f")
        TOTAL_LINES=$((TOTAL_LINES + lines))
        TOTAL_CHARS=$((TOTAL_CHARS + chars))
        TOTAL_BYTES=$((TOTAL_BYTES + bytes))
        COUNT=$((COUNT + 1))

        # Determine edit strategy hint
        hint=""
        if [ "$lines" -gt 300 ]; then
            hint=" ⚠ LARGE"
        elif [ "$lines" -gt 100 ]; then
            hint=" △ medium"
        fi

        short="${f#./}"
        printf "%-55s %7d %8d %9d%s\n" "$short" "$lines" "$chars" "$bytes" "$hint"
    done
done

if [ "$COUNT" -gt 1 ]; then
    printf "%-55s %7s %8s %9s\n" "$(printf '─%.0s' {1..55})" "-------" "--------" "---------"
    printf "%-55s %7d %8d %9d\n" "TOTAL ($COUNT files)" "$TOTAL_LINES" "$TOTAL_CHARS" "$TOTAL_BYTES"
fi
