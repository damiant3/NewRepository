#!/bin/bash
# Codex Project Health Dashboard — Linux sandbox edition
# Mirrors tools/codex-dashboard.ps1 for the claude.ai sandbox environment
#
# Usage: bash tools/codex-dashboard.sh [--json]

set -e

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

JSON_MODE=false
[ "$1" = "--json" ] && JSON_MODE=true

# ── Colors ──
DIM='\033[90m'
WHITE='\033[97m'
GREEN='\033[92m'
YELLOW='\033[93m'
RED='\033[91m'
CYAN='\033[96m'
BOLD='\033[1m'
BOLDCYAN='\033[1;96m'
RESET='\033[0m'

severity() {
    local val=$1 warn=$2 crit=$3
    if [ "$val" -le "$warn" ] 2>/dev/null; then echo -e "${GREEN}${val}${RESET}"
    elif [ "$val" -le "$crit" ] 2>/dev/null; then echo -e "${YELLOW}${val}${RESET}"
    else echo -e "${RED}${val}${RESET}"; fi
}

bar() {
    local val=$1 max=$2 width=${3:-20}
    [ "$max" -eq 0 ] && max=1
    local filled=$(( val * width / max ))
    [ "$filled" -gt "$width" ] && filled=$width
    local empty=$(( width - filled ))
    local pct=$(( val * 100 / max ))
    local color="$GREEN"
    [ "$pct" -gt 50 ] && color="$YELLOW"
    [ "$pct" -gt 80 ] && color="$RED"
    printf "${color}%s${DIM}%s${RESET} ${DIM}%d%%${RESET}" \
        "$(printf '█%.0s' $(seq 1 $filled 2>/dev/null) )" \
        "$(printf '░%.0s' $(seq 1 $empty 2>/dev/null) )" \
        "$pct"
}

SEP="${DIM}$(printf '─%.0s' {1..70})${RESET}"
HEAVY="${DIM}$(printf '═%.0s' {1..70})${RESET}"

# ═══════════════════════════════════════════════════════════════
# METRIC COLLECTORS
# ═══════════════════════════════════════════════════════════════

# Self-hosted .codex metrics
CODEX_FILES=$(find Codex.Codex -name "*.codex" -not -path "*/obj/*" -not -path "*/bin/*" 2>/dev/null)
FILE_COUNT=$(echo "$CODEX_FILES" | grep -c .)
TOTAL_LINES=0
TOTAL_CHARS=0
HOT_FILES=""

while IFS= read -r f; do
    [ -z "$f" ] && continue
    lines=$(wc -l < "$f")
    chars=$(wc -c < "$f")
    TOTAL_LINES=$((TOTAL_LINES + lines))
    TOTAL_CHARS=$((TOTAL_CHARS + chars))
    HOT_FILES="${HOT_FILES}${lines} ${chars} ${f}\n"
done <<< "$CODEX_FILES"

# Generated output metrics
S0_FILE="Codex.Codex/out/Codex.Codex.cs"
S1_FILE="Codex.Codex/stage1-output.cs"
S3_FILE="Codex.Codex/stage3-output.cs"

S0_CHARS=0; S0_LINES=0; S0_OBJECTS=0; S0_P0=0
if [ -f "$S0_FILE" ]; then
    S0_CHARS=$(wc -c < "$S0_FILE")
    S0_LINES=$(wc -l < "$S0_FILE")
    S0_OBJECTS=$(grep -o '\bobject\b' "$S0_FILE" | wc -l)
    S0_P0=$(grep -o '_p0_' "$S0_FILE" | wc -l)
fi

# Convergence
S1_CHARS=0; S3_CHARS=0; FIXED_POINT="unknown"
if [ -f "$S1_FILE" ] && [ -f "$S3_FILE" ]; then
    S1_CHARS=$(wc -c < "$S1_FILE")
    S3_CHARS=$(wc -c < "$S3_FILE")
    if diff -q "$S1_FILE" "$S3_FILE" > /dev/null 2>&1; then
        FIXED_POINT="true"
    else
        FIXED_POINT="false"
    fi
fi

# Git
GIT_BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "unknown")
GIT_HASH=$(git log -1 --pretty=format:"%h" 2>/dev/null || echo "?")
GIT_MSG=$(git log -1 --pretty=format:"%s" 2>/dev/null | head -c 72)
GIT_AGE=$(git log -1 --pretty=format:"%ar" 2>/dev/null || echo "?")
GIT_DIRTY=$(git diff --stat 2>/dev/null | grep '|' | wc -l)

# Cognitive load estimate
# 60K is the honest effective working memory — not theoretical context window
CONTEXT_BUDGET=60000
HOT_PATH_FILES="Parser.codex TypeChecker.codex CSharpEmitter.codex Lowering.codex Unifier.codex Lexer.codex"
# Cascade risk: upstream files affect all downstream stages
CASCADE_FILES="Parser.codex Lexer.codex"
HOT_CHARS=0
HOT_COUNT=0
HOT_LINES=0
for name in $HOT_PATH_FILES; do
    f=$(find Codex.Codex -name "$name" 2>/dev/null | head -1)
    if [ -n "$f" ] && [ -f "$f" ]; then
        c=$(wc -c < "$f")
        l=$(wc -l < "$f")
        HOT_CHARS=$((HOT_CHARS + c))
        HOT_LINES=$((HOT_LINES + l))
        HOT_COUNT=$((HOT_COUNT + 1))
    fi
done

# Error state from diagnostic files (no build needed)
UNIFY_ERRORS=0
ERRORTYS=0
DIAG_FILE="Codex.Codex/type-diag.txt"
UNIFY_FILE="Codex.Codex/unify-errors.txt"
if [ -f "$UNIFY_FILE" ]; then
    UNIFY_ERRORS=$(grep -c . "$UNIFY_FILE" 2>/dev/null || true)
    UNIFY_ERRORS=${UNIFY_ERRORS:-0}
fi
if [ -f "$DIAG_FILE" ]; then
    ERRORTYS=$(grep -c "ERRORTY" "$DIAG_FILE" 2>/dev/null || true)
    ERRORTYS=${ERRORTYS:-0}
fi

# Mini-file status
HAS_MINI=false
[ -f "samples/mini-bootstrap.codex" ] && HAS_MINI=true

HOT_RATIO=$((HOT_CHARS * 100 / (CONTEXT_BUDGET > 0 ? CONTEXT_BUDGET : 1)))
TYPE_DEBT=$((S0_OBJECTS + S0_P0))
CASCADE_DEPTH=7

THRASH=0
[ "$HOT_RATIO" -gt 50 ] && THRASH=$((THRASH + 1))
[ "$HOT_RATIO" -gt 80 ] && THRASH=$((THRASH + 1))
[ "$TYPE_DEBT" -gt 10 ] && THRASH=$((THRASH + 1))
[ "$TYPE_DEBT" -gt 30 ] && THRASH=$((THRASH + 1))
[ "$GIT_DIRTY" -gt 5 ] && THRASH=$((THRASH + 1))
[ "$FIXED_POINT" = "false" ] && THRASH=$((THRASH + 1))

if [ "$THRASH" -le 1 ]; then RISK="LOW"; RISK_ICON="🟢"; RISK_COLOR="$GREEN"
elif [ "$THRASH" -le 2 ]; then RISK="MEDIUM"; RISK_ICON="🟡"; RISK_COLOR="$YELLOW"
elif [ "$THRASH" -le 4 ]; then RISK="HIGH"; RISK_ICON="🔴"; RISK_COLOR="$RED"
else RISK="CRITICAL"; RISK_ICON="🔥"; RISK_COLOR="${BOLD}${RED}"; fi

# Tests (static count)
TEST_COUNT=$(grep -r '\[Fact\]\|\[Theory\]' tests/ --include="*.cs" 2>/dev/null | wc -l)
TEST_FILES=$(find tests -name "*.cs" -not -path "*/obj/*" -not -path "*/bin/*" -not -name "*AssemblyInfo*" -not -name "*GlobalUsings*" 2>/dev/null | wc -l)

# ═══════════════════════════════════════════════════════════════
# JSON OUTPUT
# ═══════════════════════════════════════════════════════════════

if $JSON_MODE; then
    cat << ENDJSON
{
  "timestamp": "$(date '+%Y-%m-%d %H:%M:%S')",
  "selfHosted": { "files": $FILE_COUNT, "lines": $TOTAL_LINES, "chars": $TOTAL_CHARS },
  "generated": { "chars": $S0_CHARS, "lines": $S0_LINES, "objects": $S0_OBJECTS, "p0": $S0_P0 },
  "convergence": { "s0": $S0_CHARS, "s1": $S1_CHARS, "s3": $S3_CHARS, "fixedPoint": $FIXED_POINT },
  "git": { "branch": "$GIT_BRANCH", "hash": "$GIT_HASH", "dirty": $GIT_DIRTY },
  "cognitive": { "budget": $CONTEXT_BUDGET, "hotChars": $HOT_CHARS, "hotFiles": $HOT_COUNT, "typeDebt": $TYPE_DEBT, "thrash": $THRASH, "risk": "$RISK" },
  "errors": { "unification": $UNIFY_ERRORS, "errorTy": $ERRORTYS, "hasMiniFile": $HAS_MINI },
  "tests": { "files": $TEST_FILES, "methods": $TEST_COUNT }
}
ENDJSON
    exit 0
fi

# ═══════════════════════════════════════════════════════════════
# FORMATTED OUTPUT
# ═══════════════════════════════════════════════════════════════

echo ""
echo -e "  ${BOLDCYAN}⚡ CODEX COMPILER DASHBOARD${RESET}  ${DIM}$(date '+%Y-%m-%d %H:%M:%S')${RESET}"
echo -e "  ${DIM}Cognitive load monitor — Linux sandbox edition${RESET}"
echo -e "$HEAVY"

# Thrash risk
echo ""
echo -e "  ${RISK_ICON} ${RISK_COLOR}AGENT THRASH RISK: ${RISK}${RESET}  ${DIM}(score: ${THRASH}/6)${RESET}"
echo ""
echo -ne "    Context budget   "; bar $HOT_CHARS $CONTEXT_BUDGET 30; echo ""
echo -e "    ${DIM}Hot path: ${HOT_CHARS} chars across ${HOT_COUNT} files  ·  Budget: ${CONTEXT_BUDGET} chars${RESET}"
echo -e "    Type debt        $(severity $TYPE_DEBT 5 20)  ${DIM}(object: ${S0_OBJECTS}  _p0_: ${S0_P0})${RESET}"
echo -e "    Cascade depth    ${DIM}${CASCADE_DEPTH} stages  (Lex → Parse → Desugar → Resolve → TypeCheck → Lower → Emit)${RESET}"
echo -e "$SEP"

# Git
echo ""
echo -e "  ${BOLD}⎇${RESET} ${WHITE}${GIT_BRANCH}${RESET}  ${DIM}${GIT_HASH}${RESET}  ${DIM}${GIT_AGE}${RESET}"
echo -e "    ${DIM}${GIT_MSG}${RESET}"
[ "$GIT_DIRTY" -gt 0 ] && echo -e "    ${YELLOW}uncommitted: ${GIT_DIRTY} files${RESET}"
echo -e "$SEP"

# Self-hosted compiler
echo ""
echo -e "  ${BOLD}📜 SELF-HOSTED COMPILER${RESET}  ${DIM}(.codex source)${RESET}"
echo -e "    Files: ${WHITE}${FILE_COUNT}${RESET}   Lines: ${WHITE}${TOTAL_LINES}${RESET}   Chars: ${WHITE}$(printf '%d' $TOTAL_CHARS)${RESET}"
echo ""
echo -e "    ${DIM}Hot files (must co-load for pipeline reasoning):${RESET}"
for name in $HOT_PATH_FILES; do
    f=$(find Codex.Codex -name "$name" 2>/dev/null | head -1)
    if [ -n "$f" ] && [ -f "$f" ]; then
        l=$(wc -l < "$f")
        c=$(wc -c < "$f")
        pct=$((c * 100 / CONTEXT_BUDGET))
        cascade=""
        for cn in $CASCADE_FILES; do
            [ "$name" = "$cn" ] && cascade="${RED}↯${RESET} "
        done
        if [ "$pct" -gt 30 ]; then warn="${YELLOW} ⚠ ${pct}% of context${RESET}"
        else warn="${DIM} ${pct}% of context${RESET}"; fi
        printf "      %b${WHITE}%-35s${RESET} ${DIM}%5d${RESET} lines  ${DIM}%6d${RESET} chars %b\n" "$cascade" "$name" "$l" "$c" "$warn"
    fi
done
echo -e "    ${DIM}↯ = cascade risk: bugs here affect all downstream stages${RESET}"
echo -e "$SEP"

# Error state (from diagnostic files — no build needed)
echo ""
echo -e "  ${BOLD}🔍 ERROR STATE${RESET}  ${DIM}(from diagnostic files)${RESET}"
echo -e "    Unification errors  $(severity $UNIFY_ERRORS 0 5)"
echo -e "    ErrorTy bindings    $(severity $ERRORTYS 0 3)"
if $HAS_MINI; then
    echo -e "    Mini repro file     ${GREEN}✓ samples/mini-bootstrap.codex${RESET}"
else
    echo -e "    Mini repro file     ${YELLOW}✗ not found — create one for focused debugging${RESET}"
fi
echo -e "$SEP"

# Generated output
echo ""
echo -e "  ${BOLD}⚙️  GENERATED C#${RESET}  ${DIM}(Codex.Codex.cs)${RESET}"
if [ "$S0_CHARS" -gt 0 ]; then
    echo -e "    Lines: ${WHITE}${S0_LINES}${RESET}   Chars: ${WHITE}$(printf '%d' $S0_CHARS)${RESET}"
    echo -e "    ${DIM}Type quality:${RESET}"
    echo -e "      object refs   $(severity $S0_OBJECTS 3 10)    ${DIM}(unresolved types)${RESET}"
    echo -e "      _p0_ proxies  $(severity $S0_P0 10 30)   ${DIM}(partial-app placeholders)${RESET}"
else
    echo -e "    ${RED}NOT FOUND${RESET}"
fi
echo -e "$SEP"

# Convergence
echo ""
echo -e "  ${BOLD}🔄 BOOTSTRAP CONVERGENCE${RESET}"
if [ "$S1_CHARS" -gt 0 ]; then
    echo -e "    Stage 0: ${WHITE}$(printf '%d' $S0_CHARS)${RESET} chars  ${DIM}(reference compiler output)${RESET}"
    echo -e "    Stage 2: ${WHITE}$(printf '%d' $S1_CHARS)${RESET} chars  ${DIM}(self-hosted output)${RESET}"
    echo -e "    Stage 3: ${WHITE}$(printf '%d' $S3_CHARS)${RESET} chars  ${DIM}(Stage 2 compiles itself)${RESET}"
    DELTA=$((S1_CHARS - S3_CHARS))
    echo -e "    Delta:   $([ "$DELTA" -eq 0 ] && echo -e "${GREEN}0${RESET}" || echo -e "${RED}${DELTA}${RESET}") chars"
    if [ "$FIXED_POINT" = "true" ]; then
        echo -e "    Status:  ${GREEN}✓ FIXED POINT${RESET}"
    else
        echo -e "    Status:  ${YELLOW}✗ NOT CONVERGED${RESET}"
    fi
else
    echo -e "    ${DIM}Stage files not found — run bootstrap to populate${RESET}"
fi
echo -e "$SEP"

# Tests
echo ""
echo -e "  ${BOLD}🧪 TESTS${RESET}"
echo -e "    Test files: ${WHITE}${TEST_FILES}${RESET}   Test methods: ${WHITE}${TEST_COUNT}${RESET}"
echo -e "$SEP"

# Guidance
echo ""
echo -e "  ${BOLD}💡 GUIDANCE${RESET}"
case "$RISK" in
    CRITICAL)
        echo -e "    ${RED}→ DO NOT assign multi-file changes right now.${RESET}"
        echo -e "    ${RED}→ Create a mini repro file first.${RESET}"
        echo -e "    ${RED}→ Isolate the pipeline stage before engaging.${RESET}" ;;
    HIGH)
        echo -e "    ${YELLOW}→ Keep tasks to ONE pipeline stage at a time.${RESET}"
        echo -e "    ${YELLOW}→ Pre-load only the relevant .codex file + its test.${RESET}" ;;
    MEDIUM)
        echo -e "    ${DIM}→ Can handle single-stage changes.${RESET}"
        echo -e "    ${DIM}→ Watch for cascading errors.${RESET}" ;;
    LOW)
        echo -e "    ${GREEN}→ Complexity is manageable. Agent should be productive.${RESET}" ;;
esac

[ "$S0_OBJECTS" -gt 10 ] && echo -e "    ${YELLOW}→ ${S0_OBJECTS} 'object' refs in generated C# — type-def map work needed.${RESET}"
[ "$S0_P0" -gt 20 ] && echo -e "    ${YELLOW}→ ${S0_P0} '_p0_' proxies — partial application type resolution needed.${RESET}"
[ "$FIXED_POINT" = "false" ] && echo -e "    ${YELLOW}→ Fixed point broken — any compiler change needs re-verification.${RESET}"

echo ""
echo -e "  ${DIM}Tip: Use --json flag to pipe metrics to other tools.${RESET}"
echo ""