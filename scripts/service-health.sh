#!/usr/bin/env bash

# B2Connect Service Health Quick Check
# Fast detection of gateway-service communication issues
# 
# Usage: ./scripts/service-health.sh [options]
# Options:
#   -v, --verbose    Show detailed output
#   -w, --watch      Continuous monitoring (5s interval)
#   -q, --quick      Quick check (gateways only)
#
# Exit codes:
#   0 - All services healthy
#   1 - One or more services unhealthy
#   2 - Configuration/script error

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Settings
TIMEOUT=2
VERBOSE=false
WATCH_MODE=false
QUICK_MODE=false
WATCH_INTERVAL=5

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
NC='\033[0m'
BOLD='\033[1m'

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--verbose) VERBOSE=true; shift ;;
        -w|--watch) WATCH_MODE=true; shift ;;
        -q|--quick) QUICK_MODE=true; shift ;;
        -h|--help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  -v, --verbose    Show detailed output"
            echo "  -w, --watch      Continuous monitoring"
            echo "  -q, --quick      Quick check (gateways only)"
            exit 0
            ;;
        *) shift ;;
    esac
done

# Utility functions
timestamp() {
    date "+%Y-%m-%d %H:%M:%S"
}

check_http_service() {
    local name=$1
    local url=$2
    local http_code
    
    http_code=$(curl -s -o /dev/null -w "%{http_code}" --connect-timeout "$TIMEOUT" --max-time "$((TIMEOUT + 1))" "$url" 2>/dev/null) || http_code="000"
    
    if [[ "$http_code" == "200" ]]; then
        echo -e "${GREEN}✅${NC} $name"
        return 0
    elif [[ "$http_code" == "000" ]]; then
        echo -e "${RED}❌${NC} $name - ${YELLOW}Connection refused${NC}"
        return 1
    else
        echo -e "${YELLOW}⚠️${NC}  $name - HTTP $http_code"
        return 1
    fi
}

check_port() {
    local name=$1
    local port=$2
    
    if nc -z localhost "$port" 2>/dev/null; then
        echo -e "${GREEN}✅${NC} $name (port $port)"
        return 0
    else
        echo -e "${RED}❌${NC} $name (port $port) - ${YELLOW}Not listening${NC}"
        return 1
    fi
}

check_aspire_services() {
    local aspire_api="http://localhost:15500/api/v0/resources"
    local response
    
    if ! nc -z localhost 15500 2>/dev/null; then
        echo -e "  ${YELLOW}Aspire Dashboard not running${NC}"
        return 1
    fi
    
    if response=$(curl -s --connect-timeout 2 "$aspire_api" 2>/dev/null); then
        if command -v jq &>/dev/null; then
            local running stopped failed total
            running=$(echo "$response" | jq -r '[.[] | select(.state=="Running")] | length' 2>/dev/null || echo "0")
            stopped=$(echo "$response" | jq -r '[.[] | select(.state=="Stopped" or .state=="Exited")] | length' 2>/dev/null || echo "0")
            failed=$(echo "$response" | jq -r '[.[] | select(.state=="Failed" or .state=="FailedToStart")] | length' 2>/dev/null || echo "0")
            total=$(echo "$response" | jq -r '. | length' 2>/dev/null || echo "?")
            
            if [[ "$VERBOSE" == "true" ]]; then
                echo -e "\n  ${CYAN}Service States:${NC}"
                echo "$response" | jq -r '.[] | "    \(.displayName // .name): \(.state)"' 2>/dev/null || echo "    (Could not parse)"
            fi
            
            echo -e "  Running: ${GREEN}$running${NC} | Stopped: ${YELLOW}$stopped${NC} | Failed: ${RED}$failed${NC} | Total: $total"
            
            [[ "$failed" -gt 0 ]] && return 1
        else
            echo "  (Install jq for detailed service info)"
        fi
    else
        echo -e "  ${YELLOW}Could not reach Aspire API${NC}"
        return 1
    fi
    return 0
}

print_header() {
    echo -e "${CYAN}═══════════════════════════════════════════════════════════${NC}"
    echo -e "${CYAN}  B2Connect Service Health Check${NC}"
    echo -e "${CYAN}  $(timestamp)${NC}"
    echo -e "${CYAN}═══════════════════════════════════════════════════════════${NC}"
    echo ""
}

run_health_check() {
    local failed=0
    
    print_header
    
    # Gateways (always check)
    echo -e "${BLUE}${BOLD}🌐 API Gateways${NC}"
    check_http_service "Store Gateway" "http://localhost:8001/health" || ((failed++)) || true
    check_http_service "Admin Gateway" "http://localhost:8081/health" || ((failed++)) || true
    
    if [[ "$QUICK_MODE" == "true" ]]; then
        echo ""
        if [[ $failed -eq 0 ]]; then
            echo -e "${GREEN}✅ Gateways healthy${NC}"
        else
            echo -e "${RED}❌ Gateway issues detected${NC}"
        fi
        return $failed
    fi
    
    # Infrastructure
    echo ""
    echo -e "${BLUE}${BOLD}🏗️  Infrastructure${NC}"
    check_http_service "Aspire Dashboard" "http://localhost:15500" || ((failed++)) || true
    check_port "PostgreSQL" "5432" || ((failed++)) || true
    check_port "Redis" "6379" || ((failed++)) || true
    check_port "Elasticsearch" "9200" || ((failed++)) || true
    check_port "RabbitMQ" "5672" || ((failed++)) || true
    
    # Frontends (optional - don't count as failures)
    echo ""
    echo -e "${BLUE}${BOLD}🖥️  Frontends${NC}"
    check_http_service "Store Frontend" "http://localhost:5173" || true
    check_http_service "Admin Frontend" "http://localhost:5174" || true
    
    # Aspire-managed services
    echo ""
    echo -e "${BLUE}${BOLD}📡 Aspire-Managed Services${NC}"
    check_aspire_services || ((failed++)) || true
    
    # Summary
    echo ""
    echo -e "${CYAN}───────────────────────────────────────────────────────────${NC}"
    if [[ $failed -eq 0 ]]; then
        echo -e "${GREEN}${BOLD}✅ All services healthy${NC}"
    else
        echo -e "${YELLOW}${BOLD}⚠️  $failed issue(s) detected${NC}"
        echo ""
        echo -e "${CYAN}Quick Actions:${NC}"
        echo "  Start services:   ./scripts/start-all.sh"
        echo "  Check ports:      ./scripts/check-ports.sh"
        echo "  Kill processes:   ./scripts/kill-all-services.sh"
        echo "  Full diagnostic:  ./scripts/diagnose.sh"
        echo ""
        echo -e "${CYAN}Aspire Dashboard:${NC} http://localhost:15500"
    fi
    
    return $((failed > 0 ? 1 : 0))
}

# Main execution
if [[ "$WATCH_MODE" == "true" ]]; then
    echo "Watching services (Ctrl+C to stop)..."
    while true; do
        clear 2>/dev/null || true
        run_health_check || true
        sleep "$WATCH_INTERVAL"
    done
else
    run_health_check
    exit $?
fi
