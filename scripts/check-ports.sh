#!/usr/bin/env bash

# B2X - Port Availability Checker
# Checks the status of all required ports and provides options to free them
# Usage: ./check-ports.sh [options] [timeout]
# Compatible with macOS bash 3.2

set -o pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Port configuration (macOS bash 3.x compatible - parallel arrays)
SERVICE_NAMES=("Auth_Service" "Tenant_Service" "Localization_Service" "Catalog_Service" "Theming_Service" "Store_Gateway" "Admin_Gateway" "MCP_Server" "Frontend_Store" "Frontend_Admin" "Frontend_Management" "Aspire_Dashboard" "Redis" "PostgreSQL" "Elasticsearch" "RabbitMQ")
SERVICE_PORTS=(7002 7003 7004 7005 7008 8000 8080 8090 5173 5174 5175 15500 6379 5432 9200 5672)

# Command type
COMMAND="${1:-check}"
WAIT_TIMEOUT="${2:-30}"

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${PURPLE}  B2X - Port Availability Checker${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo ""

# Function to check if a port is in use
is_port_in_use() {
    local port=$1
    lsof -ti:$port >/dev/null 2>&1
    return $?
}

# Function to get the process using a port
get_process_using_port() {
    local port=$1
    lsof -i ":$port" 2>/dev/null | awk 'NR>1 {print $1, "(PID:" $2 ")"}' | head -1
}

# Check port availability
check_ports() {
    echo -e "${CYAN}Port Availability Status:${NC}"
    echo ""
    
    local available_count=0
    local occupied_count=0
    local total_count=${#SERVICE_PORTS[@]}
    
    for i in "${!SERVICE_PORTS[@]}"; do
        local port=${SERVICE_PORTS[$i]}
        local service=${SERVICE_NAMES[$i]}
        local display_name=$(echo "$service" | tr '_' ' ')
        
        if is_port_in_use "$port"; then
            echo -e "  ${RED}✗${NC} $display_name (Port $port): ${RED}IN USE${NC}"
            local process=$(get_process_using_port "$port")
            [ -n "$process" ] && echo -e "     ${YELLOW}Process: $process${NC}"
            occupied_count=$((occupied_count + 1))
        else
            echo -e "  ${GREEN}✓${NC} $display_name (Port $port): ${GREEN}AVAILABLE${NC}"
            available_count=$((available_count + 1))
        fi
    done
    
    echo ""
    echo -e "${CYAN}Summary:${NC}"
    echo -e "  ${GREEN}Available:${NC} $available_count/$total_count"
    echo -e "  ${RED}In Use:${NC}    $occupied_count/$total_count"
    echo ""
    
    return $occupied_count
}

# Kill process using a port
free_port() {
    local port=$1
    local service=$2
    
    if is_port_in_use "$port"; then
        echo -e "${YELLOW}[*] Freeing port $port ($service)...${NC}"
        
        local pids=$(lsof -ti:$port 2>/dev/null | sort -u)
        
        if [ -z "$pids" ]; then
            echo -e "${YELLOW}[!] Could not identify process on port $port${NC}"
            return 1
        fi
        
        # SIGTERM first
        for pid in $pids; do
            kill -TERM "$pid" 2>/dev/null || true
        done
        sleep 1
        
        # Check if ports are freed, else SIGKILL
        if is_port_in_use "$port"; then
            for pid in $pids; do
                kill -9 "$pid" 2>/dev/null || true
            done
            sleep 1
        fi
        
        if is_port_in_use "$port"; then
            echo -e "${RED}[✗] Failed to free port $port${NC}"
            return 1
        else
            echo -e "${GREEN}[✓] Port $port freed${NC}"
            return 0
        fi
    else
        return 0
    fi
}

# Free occupied ports
free_ports() {
    echo -e "${YELLOW}Freeing occupied ports...${NC}"
    echo ""
    
    local success_count=0
    local failed_count=0
    
    for i in "${!SERVICE_PORTS[@]}"; do
        local port=${SERVICE_PORTS[$i]}
        local service=${SERVICE_NAMES[$i]}
        local display_name=$(echo "$service" | tr '_' ' ')
        
        if is_port_in_use "$port"; then
            if free_port "$port" "$display_name"; then
                success_count=$((success_count + 1))
            else
                failed_count=$((failed_count + 1))
            fi
        fi
    done
    
    echo ""
    echo -e "${CYAN}Cleanup Summary:${NC}"
    echo -e "  ${GREEN}Freed:${NC}  $success_count"
    echo -e "  ${RED}Failed:${NC} $failed_count"
    echo ""
    
    return $failed_count
}

# Wait for ports to become available
wait_for_ports() {
    local timeout=${WAIT_TIMEOUT:-30}
    local check_interval=2
    local elapsed=0
    
    echo -e "${YELLOW}⏳ Waiting for ports to become available (timeout: ${timeout}s)...${NC}"
    echo ""
    
    while [ $elapsed -lt $timeout ]; do
        local blocked_count=0
        
        for i in "${!SERVICE_PORTS[@]}"; do
            local port=${SERVICE_PORTS[$i]}
            if is_port_in_use "$port"; then
                blocked_count=$((blocked_count + 1))
            fi
        done
        
        if [ $blocked_count -eq 0 ]; then
            echo -e "${GREEN}✅ All ports are now available!${NC}"
            return 0
        fi
        
        echo -e "  ${YELLOW}[${elapsed}s] ${blocked_count} port(s) still in use...${NC}"
        sleep $check_interval
        elapsed=$((elapsed + check_interval))
    done
    
    echo -e "${RED}❌ Timeout: Some ports are still in use after ${timeout}s${NC}"
    return 1
}

# Pre-start check: wait, then free if needed, then verify
pre_start_check() {
    echo -e "${CYAN}🚀 Pre-Start Port Check${NC}"
    echo ""
    
    # First, quick check
    local blocked_count=0
    for i in "${!SERVICE_PORTS[@]}"; do
        local port=${SERVICE_PORTS[$i]}
        if is_port_in_use "$port"; then
            blocked_count=$((blocked_count + 1))
        fi
    done
    
    if [ $blocked_count -eq 0 ]; then
        echo -e "${GREEN}✅ All ports available - ready to start!${NC}"
        return 0
    fi
    
    echo -e "${YELLOW}⚠️  $blocked_count port(s) in use. Attempting cleanup...${NC}"
    echo ""
    
    # Try waiting briefly first (services might be shutting down)
    WAIT_TIMEOUT=10
    if wait_for_ports; then
        return 0
    fi
    
    echo ""
    echo -e "${YELLOW}🔧 Ports still blocked. Attempting to free...${NC}"
    echo ""
    
    # Free the ports
    free_ports
    
    # Final verification
    echo ""
    echo -e "${CYAN}🔍 Final verification...${NC}"
    
    blocked_count=0
    for i in "${!SERVICE_PORTS[@]}"; do
        local port=${SERVICE_PORTS[$i]}
        local service=${SERVICE_NAMES[$i]}
        local display_name=$(echo "$service" | tr '_' ' ')
        
        if is_port_in_use "$port"; then
            echo -e "  ${RED}❌ Port $port ($display_name) - STILL BLOCKED${NC}"
            blocked_count=$((blocked_count + 1))
        fi
    done
    
    echo ""
    
    if [ $blocked_count -gt 0 ]; then
        echo -e "${RED}❌ Pre-start check FAILED: $blocked_count port(s) could not be freed${NC}"
        echo ""
        echo -e "${YELLOW}💡 Try: ./scripts/kill-all-services.sh${NC}"
        return 1
    fi
    
    echo -e "${GREEN}✅ Pre-start check PASSED - ready to start!${NC}"
    return 0
}

# Monitor ports in real-time
monitor_ports() {
    echo -e "${YELLOW}Monitoring ports (Press Ctrl+C to exit)...${NC}"
    echo ""
    
    while true; do
        clear
        echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
        echo -e "${PURPLE}  B2X - Port Monitor${NC}"
        echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
        echo ""
        echo -e "${CYAN}Real-time Port Status:${NC}"
        echo ""
        
        for i in "${!SERVICE_PORTS[@]}"; do
            local port=${SERVICE_PORTS[$i]}
            local service=${SERVICE_NAMES[$i]}
            local display_name=$(echo "$service" | tr '_' ' ')
            
            if is_port_in_use "$port"; then
                echo -e "  ${RED}✗${NC} $display_name (Port $port): ${RED}IN USE${NC}"
                local process=$(get_process_using_port "$port")
                [ -n "$process" ] && echo -e "     ${YELLOW}$process${NC}"
            else
                echo -e "  ${GREEN}✓${NC} $display_name (Port $port): ${GREEN}AVAILABLE${NC}"
            fi
        done
        
        echo ""
        echo -e "${YELLOW}(Updated every 3 seconds - Press Ctrl+C to exit)${NC}"
        sleep 3
    done
}

# Help message
show_help() {
    echo -e "${CYAN}Usage:${NC} ./check-ports.sh [options] [timeout]"
    echo ""
    echo -e "${CYAN}Options:${NC}"
    echo -e "  ${BLUE}--check${NC}      Check port availability (default)"
    echo -e "  ${BLUE}--free${NC}       Free up occupied ports"
    echo -e "  ${BLUE}--wait${NC}       Wait for ports to become available"
    echo -e "  ${BLUE}--pre-start${NC}  Pre-start check with automatic cleanup"
    echo -e "  ${BLUE}--monitor${NC}    Monitor ports in real-time"
    echo -e "  ${BLUE}--help${NC}       Show this help message"
    echo ""
    echo -e "${CYAN}Examples:${NC}"
    echo -e "  ${BLUE}./check-ports.sh${NC}                  # Check port status"
    echo -e "  ${BLUE}./check-ports.sh --free${NC}           # Free occupied ports"
    echo -e "  ${BLUE}./check-ports.sh --wait 60${NC}        # Wait up to 60s for ports"
    echo -e "  ${BLUE}./check-ports.sh --pre-start${NC}      # Full pre-start check"
    echo ""
}

# Main logic
case "$COMMAND" in
    --check|check)
        check_ports
        ;;
    --free|free)
        check_ports
        echo ""
        free_ports
        echo ""
        check_ports
        ;;
    --wait|wait)
        wait_for_ports
        ;;
    --pre-start|pre-start)
        pre_start_check
        ;;
    --monitor|monitor)
        monitor_ports
        ;;
    --help|-h|help)
        show_help
        ;;
    *)
        echo -e "${RED}[✗] Unknown command: $COMMAND${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
