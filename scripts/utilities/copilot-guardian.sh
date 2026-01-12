#!/usr/bin/env bash
# Copilot Guardian - Unified session and budget management
# Reference: GL-006 Token Optimization Strategy
# Usage: ./scripts/copilot-guardian.sh [command]

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
LOG_DIR="$PROJECT_ROOT/.ai/logs"
LOG_FILE="$LOG_DIR/copilot-usage.log"
SESSION_FILE="$LOG_DIR/.copilot-session"

RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m'

mkdir -p "$LOG_DIR"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*" >> "$LOG_FILE"
}

print_status() {
    local status=$1
    local message=$2
    case $status in
        ok)     echo -e "${GREEN}✓${NC} $message" ;;
        warn)   echo -e "${YELLOW}⚠${NC} $message" ;;
        error)  echo -e "${RED}✗${NC} $message" ;;
        info)   echo -e "${BLUE}ℹ${NC} $message" ;;
    esac
}

cmd_status() {
    echo ""
    echo "COPILOT GUARDIAN - Session Status"
    echo "=================================="
    echo ""
    echo "File Sizes:"
    echo "----------------------------------"
    
    local main_size=0
    local main_file="$PROJECT_ROOT/.github/copilot-instructions.md"
    if [[ -f "$main_file" ]]; then
        main_size=$(wc -c < "$main_file")
        if [[ $main_size -gt 10240 ]]; then
            print_status "warn" "copilot-instructions.md: ${main_size}B (max: 10240B)"
        else
            print_status "ok" "copilot-instructions.md: ${main_size}B (max: 10240B)"
        fi
    fi
    
    echo ""
    echo "Agent Files:"
    local agent_total=0
    for f in "$PROJECT_ROOT"/.github/agents/*.md; do
        [[ -f "$f" ]] || continue
        local size
        size=$(wc -c < "$f")
        agent_total=$((agent_total + size))
        if [[ $size -gt 3072 ]]; then
            print_status "warn" "$(basename "$f"): ${size}B"
        else
            print_status "ok" "$(basename "$f"): ${size}B"
        fi
    done
    echo "----------------------------------"
    echo "   Total agents: ${agent_total}B"
    
    echo ""
    echo "Instruction Files:"
    local instr_total=0
    for f in "$PROJECT_ROOT"/.github/instructions/*.md; do
        [[ -f "$f" ]] || continue
        local size
        size=$(wc -c < "$f")
        instr_total=$((instr_total + size))
        if [[ $size -gt 2048 ]]; then
            print_status "warn" "$(basename "$f"): ${size}B"
        else
            print_status "ok" "$(basename "$f"): ${size}B"
        fi
    done
    echo "----------------------------------"
    echo "   Total instructions: ${instr_total}B"
    
    echo ""
    echo "=================================="
    local grand_total=$((main_size + agent_total + instr_total))
    echo "   GRAND TOTAL: ${grand_total}B"
    echo "=================================="
    echo ""
}

cmd_warn() {
    echo ""
    echo "COPILOT GUARDIAN - Limit Warnings"
    echo ""
    
    local warnings=0
    local main_file="$PROJECT_ROOT/.github/copilot-instructions.md"
    
    if [[ -f "$main_file" ]]; then
        local size
        size=$(wc -c < "$main_file")
        if [[ $size -gt 10240 ]]; then
            print_status "error" "copilot-instructions.md exceeds 10KB limit!"
            warnings=$((warnings + 1))
        elif [[ $size -gt 8192 ]]; then
            print_status "warn" "copilot-instructions.md approaching limit (${size}B / 10240B)"
            warnings=$((warnings + 1))
        fi
    fi
    
    for f in "$PROJECT_ROOT"/.github/agents/*.md; do
        [[ -f "$f" ]] || continue
        local size
        size=$(wc -c < "$f")
        if [[ $size -gt 3072 ]]; then
            print_status "error" "$(basename "$f") exceeds 3KB limit!"
            warnings=$((warnings + 1))
        fi
    done
    
    for f in "$PROJECT_ROOT"/.github/instructions/*.md; do
        [[ -f "$f" ]] || continue
        local size
        size=$(wc -c < "$f")
        if [[ $size -gt 2048 ]]; then
            print_status "warn" "$(basename "$f") exceeds 2KB limit"
            warnings=$((warnings + 1))
        fi
    done
    
    echo ""
    if [[ $warnings -eq 0 ]]; then
        print_status "ok" "All files within limits"
    else
        print_status "warn" "$warnings warning(s) found"
        echo ""
        echo "Recommendations:"
        echo "  1. Move detailed content to .ai/knowledgebase/"
        echo "  2. Replace inline examples with [DocID] references"
        echo "  3. Run: ./scripts/copilot-guardian.sh audit"
    fi
    echo ""
    
    return $warnings
}

cmd_stop() {
    echo ""
    echo "COPILOT GUARDIAN - Emergency Stop Protocol"
    echo ""
    
    log "EMERGENCY STOP initiated"
    
    echo "Actions to take:"
    echo ""
    echo "1. Pause current AI session"
    echo "2. Save work in progress to a scratch file"
    echo "3. Take a 15-minute break (cooldown)"
    echo "4. Split remaining work into smaller tasks"
    echo ""
    
    cat > "$SESSION_FILE" << EOF
# Copilot Session Paused
# Created: $(date)
# Reason: Emergency stop

## Work in Progress
- File: [TODO: add file path]
- Function: [TODO: add function name]
- Next step: [TODO: describe next action]

## Context to Restore
- [TODO: add key context items]

## Resume Command
./scripts/copilot-guardian.sh recover
EOF
    
    print_status "info" "Session file created: $SESSION_FILE"
    log "Session paused - session file created"
}

cmd_recover() {
    echo ""
    echo "COPILOT GUARDIAN - Recovery Protocol"
    echo ""
    
    if [[ -f "$SESSION_FILE" ]]; then
        echo "Previous session found:"
        echo "----------------------------------"
        cat "$SESSION_FILE"
        echo "----------------------------------"
        echo ""
        
        read -p "Clear session file and continue? [y/N] " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            rm "$SESSION_FILE"
            print_status "ok" "Session cleared - ready to continue"
            log "Session recovered"
        fi
    else
        print_status "info" "No paused session found"
    fi
    
    echo ""
    echo "Recovery checklist:"
    echo "  - Review .ai/knowledgebase/lessons.md"
    echo "  - Start with small, focused task"
    echo "  - Provide minimal context (reference DocIDs)"
    echo ""
}

cmd_reset() {
    echo ""
    echo "COPILOT GUARDIAN - Reset"
    echo ""
    
    if [[ -f "$SESSION_FILE" ]]; then
        rm "$SESSION_FILE"
        print_status "ok" "Session file removed"
    fi
    
    print_status "ok" "Counters reset"
    log "Session reset"
    echo ""
}

cmd_audit() {
    echo ""
    echo "COPILOT GUARDIAN - Full Audit"
    echo ""
    echo "All Copilot-related files by size:"
    echo "=================================="
    
    find "$PROJECT_ROOT/.github" -name "*.md" -type f -exec wc -c {} \; 2>/dev/null | sort -rn
    
    echo "=================================="
    echo ""
    
    local total
    total=$(find "$PROJECT_ROOT/.github" -name "*.md" -type f -exec wc -c {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}')
    
    echo "Total .github markdown: ${total}B"
    echo ""
    
    if [[ $total -gt 50000 ]]; then
        print_status "warn" "Total exceeds 50KB - aggressive trimming needed"
    elif [[ $total -gt 30000 ]]; then
        print_status "info" "Total approaching 30KB - consider trimming"
    else
        print_status "ok" "Total within acceptable range"
    fi
    echo ""
}

cmd_help() {
    echo ""
    echo "Copilot Guardian - Session & Budget Management"
    echo ""
    echo "Usage: $0 <command>"
    echo ""
    echo "Commands:"
    echo "  status   Show current session and file size status"
    echo "  warn     Check for limit warnings"
    echo "  stop     Emergency stop protocol"
    echo "  recover  Recovery after limit hit"
    echo "  reset    Reset session counters"
    echo "  audit    Full file size audit"
    echo "  help     Show this help message"
    echo ""
    echo "Configuration: .ai/config/budget.yml"
    echo "Documentation: GL-006 Token Optimization Strategy"
    echo ""
}

case "${1:-help}" in
    status)  cmd_status ;;
    warn)    cmd_warn ;;
    stop)    cmd_stop ;;
    recover) cmd_recover ;;
    reset)   cmd_reset ;;
    audit)   cmd_audit ;;
    help)    cmd_help ;;
    *)       echo "Unknown command: $1"; cmd_help; exit 1 ;;
esac
