#!/bin/bash
# Copilot File Size Audit Script
# Run weekly to prevent rate limiting
# See GL-006 for targets

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "=== Copilot File Size Audit ==="
echo "Date: $(date '+%Y-%m-%d %H:%M')"
echo ""

# Main instructions file (target: <10KB = 10240 bytes)
echo "📄 Main Instructions (target: <10KB):"
MAIN_SIZE=$(wc -c < .github/copilot-instructions.md 2>/dev/null || echo 0)
MAIN_KB=$((MAIN_SIZE / 1024))
if [ "$MAIN_SIZE" -gt 10240 ]; then
    printf "   ${RED}%d KB ❌ OVER LIMIT${NC} - copilot-instructions.md\n" "$MAIN_KB"
    MAIN_STATUS="FAIL"
else
    printf "   ${GREEN}%d KB ✅${NC} - copilot-instructions.md\n" "$MAIN_KB"
    MAIN_STATUS="OK"
fi
echo ""

# Agent files (target: <3KB = 3072 bytes each)
echo "🤖 Agent Files (target: <3KB each):"
AGENT_OVER=0
for f in .github/agents/*.md; do
    if [ -f "$f" ]; then
        size=$(wc -c < "$f")
        name=$(basename "$f")
        if [ "$size" -gt 3072 ]; then
            printf "   ${RED}%4d B ❌${NC} %s\n" "$size" "$name"
            AGENT_OVER=$((AGENT_OVER + 1))
        elif [ "$size" -gt 2560 ]; then
            printf "   ${YELLOW}%4d B ⚠️${NC}  %s\n" "$size" "$name"
        else
            printf "   ${GREEN}%4d B ✅${NC} %s\n" "$size" "$name"
        fi
    fi
done | sort -rn
echo ""

# Instruction files (target: <2KB = 2048 bytes each)
echo "📝 Instruction Files (target: <2KB each):"
INS_OVER=0
for f in .github/instructions/*.md; do
    if [ -f "$f" ]; then
        size=$(wc -c < "$f")
        name=$(basename "$f")
        if [ "$size" -gt 2048 ]; then
            printf "   ${RED}%4d B ❌${NC} %s\n" "$size" "$name"
            INS_OVER=$((INS_OVER + 1))
        else
            printf "   ${GREEN}%4d B ✅${NC} %s\n" "$size" "$name"
        fi
    fi
done
echo ""

# Prompt files (target: <2KB = 2048 bytes each)
echo "📋 Prompt Files (target: <2KB each):"
PROMPT_OVER=0
PROMPT_COUNT=0
for f in .github/prompts/*.md; do
    if [ -f "$f" ]; then
        PROMPT_COUNT=$((PROMPT_COUNT + 1))
        size=$(wc -c < "$f")
        name=$(basename "$f")
        if [ "$size" -gt 2048 ]; then
            printf "   ${RED}%4d B ❌${NC} %s\n" "$size" "$name"
            PROMPT_OVER=$((PROMPT_OVER + 1))
        fi
    fi
done
if [ "$PROMPT_OVER" -eq 0 ]; then
    printf "   ${GREEN}All %d prompts OK ✅${NC}\n" "$PROMPT_COUNT"
fi
echo ""

# Summary
echo "═══════════════════════════════════════"
echo "📊 Summary:"
TOTAL_GITHUB=$(du -sk .github/ 2>/dev/null | cut -f1)
printf "   Total .github/ size: %d KB\n" "$TOTAL_GITHUB"
echo ""

ISSUES=0
[ "$MAIN_STATUS" = "FAIL" ] && ISSUES=$((ISSUES + 1))
[ "$AGENT_OVER" -gt 0 ] && ISSUES=$((ISSUES + AGENT_OVER))
[ "$INS_OVER" -gt 0 ] && ISSUES=$((ISSUES + INS_OVER))
[ "$PROMPT_OVER" -gt 0 ] && ISSUES=$((ISSUES + PROMPT_OVER))

if [ "$ISSUES" -gt 0 ]; then
    printf "${RED}❌ %d file(s) over limit - action required${NC}\n" "$ISSUES"
    echo ""
    echo "Recommended actions:"
    [ "$MAIN_STATUS" = "FAIL" ] && echo "  • Slim copilot-instructions.md (move details to .ai/guidelines/)"
    [ "$AGENT_OVER" -gt 0 ] && echo "  • Trim oversized agent files (move details to knowledgebase)"
    [ "$INS_OVER" -gt 0 ] && echo "  • Reduce instruction files (use references instead of inline)"
    [ "$PROMPT_OVER" -gt 0 ] && echo "  • Split large prompts into smaller files"
    exit 1
else
    printf "${GREEN}✅ All files within limits${NC}\n"
    exit 0
fi
