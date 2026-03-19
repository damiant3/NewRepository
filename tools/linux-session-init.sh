#!/bin/bash
# linux-session-init.sh — One-shot environment setup for Linux agent sessions.
#
# Run this at the start of every fresh Claude session to get a working
# Codex development environment. Handles: .NET SDK, repo clone/pull,
# build, test summary, dashboard, and CurrentPlan display.
#
# Usage (from a fresh session):
#   bash tools/linux-session-init.sh
#
# Or if the repo isn't cloned yet, fetch and run directly:
#   curl -sH "Authorization: token $GITHUB_PAT" \
#     https://raw.githubusercontent.com/damiant3/NewRepository/master/tools/linux-session-init.sh | bash
#
# The script reads the GitHub PAT from:
#   1. $GITHUB_PAT environment variable
#   2. /mnt/user-data/uploads/_claude.json (uploaded by user)

set -e

# ── Colors ──
DIM='\033[90m'
GREEN='\033[92m'
YELLOW='\033[93m'
RED='\033[91m'
BOLD='\033[1m'
CYAN='\033[96m'
RESET='\033[0m'

step() { echo -e "\n${BOLD}${CYAN}▶ $1${RESET}"; }
ok()   { echo -e "  ${GREEN}✓ $1${RESET}"; }
warn() { echo -e "  ${YELLOW}⚠ $1${RESET}"; }
fail() { echo -e "  ${RED}✗ $1${RESET}"; exit 1; }

REPO_DIR="/home/claude/NewRepository"
REPO_URL="https://github.com/damiant3/NewRepository.git"
OVERALL_START=$SECONDS

echo ""
echo -e "${BOLD}${CYAN}═══ CODEX SESSION INIT ═══${RESET}  ${DIM}$(date '+%Y-%m-%d %H:%M:%S')${RESET}"

# ═══════════════════════════════════════════════════════════════
# STEP 1: Resolve GitHub PAT
# ═══════════════════════════════════════════════════════════════

step "Step 1: Resolving GitHub credentials"

if [ -z "$GITHUB_PAT" ]; then
    CLAUDE_JSON="/mnt/user-data/uploads/_claude.json"
    if [ -f "$CLAUDE_JSON" ]; then
        GITHUB_PAT=$(grep -oP '"Authorization":\s*"Bearer \K[^"]+' "$CLAUDE_JSON" 2>/dev/null || true)
        if [ -z "$GITHUB_PAT" ]; then
            GITHUB_PAT=$(grep -oP 'github_pat_[A-Za-z0-9_]+' "$CLAUDE_JSON" 2>/dev/null || true)
        fi
    fi
fi

if [ -n "$GITHUB_PAT" ]; then
    AUTH_URL="https://${GITHUB_PAT}@github.com/damiant3/NewRepository.git"
    ok "PAT found"
else
    AUTH_URL="$REPO_URL"
    warn "No PAT found — will try unauthenticated clone (may fail for private repos)"
fi

# ═══════════════════════════════════════════════════════════════
# STEP 2: Install .NET 8 SDK
# ═══════════════════════════════════════════════════════════════

step "Step 2: Checking .NET SDK"

export PATH="$PATH:/root/.dotnet"
export DOTNET_ROOT="/root/.dotnet"

if command -v dotnet &>/dev/null && dotnet --version 2>/dev/null | grep -q "^8\."; then
    ok ".NET $(dotnet --version) already installed"
else
    echo "  Installing .NET 8 SDK..."
    wget -q https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 8.0 2>&1 | grep "installed\|finished" || true
    rm -f /tmp/dotnet-install.sh

    if command -v dotnet &>/dev/null; then
        ok ".NET $(dotnet --version) installed"
    else
        fail ".NET SDK installation failed"
    fi
fi

# ═══════════════════════════════════════════════════════════════
# STEP 3: Clone or pull repo
# ═══════════════════════════════════════════════════════════════

step "Step 3: Getting latest code"

if [ -d "$REPO_DIR/.git" ]; then
    cd "$REPO_DIR"
    # Update remote URL in case PAT changed
    git remote set-url origin "$AUTH_URL" 2>/dev/null || true
    BEFORE=$(git rev-parse HEAD)
    git pull --rebase origin master 2>&1 | tail -3
    AFTER=$(git rev-parse HEAD)
    if [ "$BEFORE" = "$AFTER" ]; then
        ok "Already up to date @ $(git log -1 --format='%h %s')"
    else
        NEW_COMMITS=$(git rev-list --count "$BEFORE..$AFTER")
        ok "Pulled $NEW_COMMITS new commit(s), now @ $(git log -1 --format='%h %s')"
    fi
else
    echo "  Cloning repository..."
    git clone "$AUTH_URL" "$REPO_DIR" 2>&1 | tail -2
    cd "$REPO_DIR"
    ok "Cloned @ $(git log -1 --format='%h %s')"
fi

# Set git identity for this session
git config user.email "claude@anthropic.com"
git config user.name "Claude (Opus 4.6, Linux)"

# ═══════════════════════════════════════════════════════════════
# STEP 4: Build
# ═══════════════════════════════════════════════════════════════

step "Step 4: Building Codex.sln"
START=$SECONDS

BUILD_OUTPUT=$(dotnet build Codex.sln -v q 2>&1)
BUILD_EXIT=$?
ERRORS=$(echo "$BUILD_OUTPUT" | grep -oP '\d+(?= Error)' | tail -1)
WARNINGS=$(echo "$BUILD_OUTPUT" | grep -oP '\d+(?= Warning)' | tail -1)
ELAPSED=$((SECONDS - START))

if [ "$BUILD_EXIT" -eq 0 ]; then
    ok "Build succeeded (${ELAPSED}s) — ${ERRORS:-0} errors, ${WARNINGS:-0} warnings"
else
    fail "Build failed (${ELAPSED}s) — check output above"
fi

# ═══════════════════════════════════════════════════════════════
# STEP 5: Run tests (summary only)
# ═══════════════════════════════════════════════════════════════

step "Step 5: Running tests"
START=$SECONDS

TEST_OUTPUT=$(dotnet test Codex.sln --no-build -v q 2>&1)
PASSED=$(echo "$TEST_OUTPUT" | grep -oP 'Passed:\s+\K\d+' | awk '{s+=$1}END{print s+0}')
FAILED=$(echo "$TEST_OUTPUT" | grep -oP 'Failed:\s+\K\d+' | awk '{s+=$1}END{print s+0}')
ELAPSED=$((SECONDS - START))

if [ "${FAILED:-0}" -eq 0 ]; then
    ok "All $PASSED tests passed (${ELAPSED}s)"
else
    warn "$PASSED passed, $FAILED FAILED (${ELAPSED}s)"
    echo "$TEST_OUTPUT" | grep -A2 "Failed " | head -20
fi

# ═══════════════════════════════════════════════════════════════
# STEP 6: Dashboard (if available)
# ═══════════════════════════════════════════════════════════════

DASHBOARD="$REPO_DIR/tools/codexdashboard.sh"
if [ -f "$DASHBOARD" ] && [ -x "$DASHBOARD" ]; then
    step "Step 6: Dashboard"
    bash "$DASHBOARD" 2>/dev/null || warn "Dashboard had errors (non-fatal)"
fi

# ═══════════════════════════════════════════════════════════════
# STEP 7: Show CurrentPlan summary
# ═══════════════════════════════════════════════════════════════

PLAN="$REPO_DIR/docs/CurrentPlan.md"
if [ -f "$PLAN" ]; then
    step "Step 7: Current Plan"
    # Show the snapshot table and horizons summary
    echo ""
    sed -n '/^### Snapshot/,/^---$/p' "$PLAN" | head -20
    echo ""
    echo -e "${DIM}── Horizon 1 (Language Freedom) ──${RESET}"
    grep "^| L[0-9]" "$PLAN" | head -10
    echo ""
    echo -e "${DIM}── Horizon 2 (Library & Runtime) ──${RESET}"
    grep "^| R[0-9]" "$PLAN" | head -10
    echo ""
    echo -e "${DIM}Full plan: docs/CurrentPlan.md${RESET}"
fi

# ═══════════════════════════════════════════════════════════════
# DONE
# ═══════════════════════════════════════════════════════════════

TOTAL=$((SECONDS - OVERALL_START))
echo ""
echo -e "${BOLD}${GREEN}═══ SESSION READY ═══${RESET}  ${DIM}Total: ${TOTAL}s${RESET}"
echo -e "${DIM}Working directory: $REPO_DIR${RESET}"
echo -e "${DIM}Date: $(date '+%Y-%m-%d')  Branch: $(git rev-parse --abbrev-ref HEAD)  Head: $(git log -1 --format='%h')${RESET}"
echo ""
