#!/bin/bash
# sdiff.sh — Snapshot-and-diff for verifying edits.
# Bash equivalent of tools/agent/sdiff.ps1
#
# Usage:
#   bash tools/agent/sdiff.sh snap <file>        — save a snapshot before editing
#   bash tools/agent/sdiff.sh diff <file>        — diff current vs snapshot
#   bash tools/agent/sdiff.sh restore <file>     — restore from snapshot
#   bash tools/agent/sdiff.sh clean              — delete all snapshots

set -e

SNAP_DIR="/tmp/codex-snapshots"

ACTION="${1:?Usage: sdiff.sh <snap|diff|restore|clean> [file]}"
FILE="$2"

case "$ACTION" in
    snap)
        [ -z "$FILE" ] && { echo "Usage: sdiff.sh snap <file>" >&2; exit 1; }
        [ ! -f "$FILE" ] && { echo "File not found: $FILE" >&2; exit 1; }
        mkdir -p "$SNAP_DIR"
        SAFE_NAME=$(echo "$FILE" | tr '/' '_')
        cp "$FILE" "$SNAP_DIR/$SAFE_NAME"
        LINES=$(wc -l < "$FILE")
        echo "✓ Snapshot saved: $FILE ($LINES lines)"
        ;;
    diff)
        [ -z "$FILE" ] && { echo "Usage: sdiff.sh diff <file>" >&2; exit 1; }
        SAFE_NAME=$(echo "$FILE" | tr '/' '_')
        SNAP="$SNAP_DIR/$SAFE_NAME"
        [ ! -f "$SNAP" ] && { echo "No snapshot found for $FILE. Run 'snap' first." >&2; exit 1; }
        [ ! -f "$FILE" ] && { echo "File not found: $FILE" >&2; exit 1; }
        OLD_LINES=$(wc -l < "$SNAP")
        NEW_LINES=$(wc -l < "$FILE")
        DELTA=$((NEW_LINES - OLD_LINES))
        echo "── Diff: $FILE ──"
        echo "  Before: $OLD_LINES lines   After: $NEW_LINES lines   Delta: ${DELTA}"
        echo ""
        diff --unified=3 "$SNAP" "$FILE" || true
        echo "── End diff ──"
        ;;
    restore)
        [ -z "$FILE" ] && { echo "Usage: sdiff.sh restore <file>" >&2; exit 1; }
        SAFE_NAME=$(echo "$FILE" | tr '/' '_')
        SNAP="$SNAP_DIR/$SAFE_NAME"
        [ ! -f "$SNAP" ] && { echo "No snapshot found for $FILE." >&2; exit 1; }
        cp "$SNAP" "$FILE"
        LINES=$(wc -l < "$FILE")
        echo "✓ Restored: $FILE ($LINES lines)"
        ;;
    clean)
        if [ -d "$SNAP_DIR" ]; then
            COUNT=$(find "$SNAP_DIR" -type f | wc -l)
            rm -rf "$SNAP_DIR"
            echo "✓ Cleaned $COUNT snapshot(s)"
        else
            echo "No snapshots to clean."
        fi
        ;;
    *)
        echo "Usage: sdiff.sh <snap|diff|restore|clean> [file]" >&2
        exit 1
        ;;
esac
