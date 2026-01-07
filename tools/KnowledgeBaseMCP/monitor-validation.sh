#!/bin/bash
# KB-MCP Monitoring Script for Phase 2a Validation Period
# Runs automated checks and collects metrics for 1-week validation

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$SCRIPT_DIR")")"
LOG_FILE="$PROJECT_ROOT/.ai/logs/kb-mcp-monitoring-$(date +%Y%m%d-%H%M%S).log"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

log "${BLUE}🚀 Starting KB-MCP Validation Monitoring${NC}"
log "Log file: $LOG_FILE"

# Function to run validation suite
run_validation() {
    log "${YELLOW}Running validation suite...${NC}"

    if cd "$PROJECT_ROOT" && python3 tools/KnowledgeBaseMCP/validate.py >> "$LOG_FILE" 2>&1; then
        log "${GREEN}✅ Validation suite PASSED${NC}"
        return 0
    else
        log "${RED}❌ Validation suite FAILED${NC}"
        return 1
    fi
}

# Function to check MCP server status
check_mcp_status() {
    log "${YELLOW}Checking MCP server configuration...${NC}"

    if [ -f "$PROJECT_ROOT/.vscode/mcp.json" ]; then
        if grep -q "kb-mcp" "$PROJECT_ROOT/.vscode/mcp.json"; then
            log "${GREEN}✅ KB-MCP configured in .vscode/mcp.json${NC}"
            return 0
        else
            log "${RED}❌ KB-MCP not found in .vscode/mcp.json${NC}"
            return 1
        fi
    else
        log "${RED}❌ .vscode/mcp.json not found${NC}"
        return 1
    fi
}

# Function to check database integrity
check_database() {
    log "${YELLOW}Checking database integrity...${NC}"

    if [ -f "$PROJECT_ROOT/.ai/kb-index.db" ]; then
        # Get document count from database
        DOC_COUNT=$(sqlite3 "$PROJECT_ROOT/.ai/kb-index.db" "SELECT COUNT(*) FROM documents;" 2>/dev/null || echo "0")

        if [ "$DOC_COUNT" -eq 104 ]; then
            log "${GREEN}✅ Database integrity OK: $DOC_COUNT documents${NC}"
            return 0
        else
            log "${RED}❌ Database integrity issue: $DOC_COUNT documents (expected 104)${NC}"
            return 1
        fi
    else
        log "${RED}❌ Database file not found${NC}"
        return 1
    fi
}

# Function to collect performance metrics
collect_metrics() {
    log "${YELLOW}Collecting performance metrics from validation suite...${NC}"

    # Extract performance metrics from the latest validation report
    if [ -f "$PROJECT_ROOT/.ai/status/KB-MCP-VALIDATION-REPORT.json" ]; then
        AVG_SEARCH=$(grep -o '"avg_search_time":[0-9.]*' "$PROJECT_ROOT/.ai/status/KB-MCP-VALIDATION-REPORT.json" | cut -d':' -f2 2>/dev/null || echo "0")
        MAX_SEARCH=$(grep -o '"max_search_time":[0-9.]*' "$PROJECT_ROOT/.ai/status/KB-MCP-VALIDATION-REPORT.json" | cut -d':' -f2 2>/dev/null || echo "0")

        if [ "$(echo "$AVG_SEARCH > 0" | bc 2>/dev/null)" -eq 1 ]; then
            log "${GREEN}✅ Performance metrics from validation: Avg ${AVG_SEARCH}ms, Max ${MAX_SEARCH}ms${NC}"
            return 0
        else
            log "${YELLOW}⚠️  No recent performance metrics found, running quick validation...${NC}"
            # Fallback: run a quick validation to get metrics
            if cd "$PROJECT_ROOT" && python3 tools/KnowledgeBaseMCP/validate.py >/dev/null 2>&1; then
                log "${GREEN}✅ Performance validation completed${NC}"
                return 0
            fi
        fi
    else
        log "${YELLOW}⚠️  No validation report found, running validation...${NC}"
        if cd "$PROJECT_ROOT" && python3 tools/KnowledgeBaseMCP/validate.py >/dev/null 2>&1; then
            log "${GREEN}✅ Performance validation completed${NC}"
            return 0
        fi
    fi

    log "${RED}❌ Could not collect performance metrics${NC}"
    return 1
}

# Function to check file sizes
check_file_sizes() {
    log "${YELLOW}Checking file sizes...${NC}"

    # Check if compressed files exist
    COMPRESSED_FILES=(
        ".github/instructions/mcp-quick-reference.instructions.md"
        ".github/instructions/backend-essentials.instructions.md"
        ".github/instructions/frontend-essentials.instructions.md"
    )

    for file in "${COMPRESSED_FILES[@]}"; do
        if [ -f "$PROJECT_ROOT/$file" ]; then
            SIZE=$(stat -f%z "$PROJECT_ROOT/$file" 2>/dev/null || stat -c%s "$PROJECT_ROOT/$file" 2>/dev/null || echo "0")
            SIZE_KB=$(echo "scale=1; $SIZE/1024" | bc 2>/dev/null || echo "0")
            log "${GREEN}✅ $file: ${SIZE_KB} KB${NC}"
        else
            log "${RED}❌ $file not found${NC}"
            return 1
        fi
    done

    return 0
}

# Main monitoring loop
log "${BLUE}Starting monitoring checks...${NC}"

CHECKS_PASSED=0
CHECKS_TOTAL=0

# Run validation suite
((CHECKS_TOTAL++))
if run_validation; then
    ((CHECKS_PASSED++))
fi

# Check MCP status
((CHECKS_TOTAL++))
if check_mcp_status; then
    ((CHECKS_PASSED++))
fi

# Check database
((CHECKS_TOTAL++))
if check_database; then
    ((CHECKS_PASSED++))
fi

# Collect metrics
((CHECKS_TOTAL++))
if collect_metrics; then
    ((CHECKS_PASSED++))
fi

# Check file sizes
((CHECKS_TOTAL++))
if check_file_sizes; then
    ((CHECKS_PASSED++))
fi

# Summary
log "${BLUE}Monitoring Summary:${NC}"
log "Checks passed: $CHECKS_PASSED/$CHECKS_TOTAL"

if [ "$CHECKS_PASSED" -eq "$CHECKS_TOTAL" ]; then
    log "${GREEN}✅ All monitoring checks PASSED${NC}"
    echo "✅ KB-MCP monitoring completed successfully"
    exit 0
else
    log "${RED}❌ Some monitoring checks FAILED${NC}"
    echo "❌ KB-MCP monitoring found issues - check log: $LOG_FILE"
    exit 1
fi