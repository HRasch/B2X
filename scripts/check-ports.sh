#!/usr/bin/env bash

# B2Connect - Port Availability Checker
# Checks the status of all required ports and provides options to free them
# Usage: ./check-ports.sh [options]
# Options:
#   --check     Check port availability (default)
#   --free      Free up occupied ports
#   --monitor   Monitor ports in real-time

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Port configuration using associative arrays
declare -A PORTS=(
    [AuthService]="7002"
    [TenantService]="7003"
    [LocalizationService]="7004"
    [CatalogService]="7005"
    [ThemingService]="7008"
    [StoreGateway]="8000"
    [AdminGateway]="8080"
    [FrontendStore]="5173"
    [FrontendAdmin]="5174"
    [Dashboard]="15500"
    [Redis]="6379"
    [PostgreSQL]="5432"
    [Elasticsearch]="9200"
    [RabbitMQ]="5672"
)

declare -A SERVICE_DISPLAY=(
    [AuthService]="Auth Service"
    [TenantService]="Tenant Service"
    [LocalizationService]="Localization Service"
    [CatalogService]="Catalog Service"
    [ThemingService]="Theming Service"
    [StoreGateway]="Store Gateway"
    [AdminGateway]="Admin Gateway"
    [FrontendStore]="Frontend Store"
    [FrontendAdmin]="Frontend Admin"
    [Dashboard]="Aspire Dashboard"
    [Redis]="Redis Cache"
    [PostgreSQL]="PostgreSQL DB"
    [Elasticsearch]="Elasticsearch"
    [RabbitMQ]="RabbitMQ"
)

# Command type
COMMAND="${1:-check}"

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${PURPLE}  B2Connect - Port Availability Checker${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo ""

# Function to check if a port is in use
is_port_in_use() {
    local port=$1
    nc -z localhost "$port" 2>/dev/null
    return $?
}

# Function to get the process using a port
get_process_using_port() {
    local port=$1
    lsof -i ":$port" 2>/dev/null | awk 'NR>1 {print $1, "(" $2 ")"}' | head -1
}

# Check port availability
check_ports() {
    echo -e "${CYAN}Port Availability Status:${NC}"
    echo ""
    
    local available_count=0
    local occupied_count=0
    local total_count=${#PORTS[@]}
    
    for service in "${!PORTS[@]}"; do
        local port=${PORTS[$service]}
        local display_name=${SERVICE_DISPLAY[$service]:-$service}
        
        if is_port_in_use "$port"; then
            echo -e "  ${RED}✗${NC} $display_name (Port $port): ${RED}IN USE${NC}"
            local process=$(get_process_using_port "$port")
            [ ! -z "$process" ] && echo -e "     ${YELLOW}Process: $process${NC}"
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
}

# Kill process using a port
free_port() {
    local port=$1
    local service=$2
    
    if is_port_in_use "$port"; then
        echo -e "${YELLOW}[*] Attempting to free port $port ($service)...${NC}"
        
        local pids=$(lsof -i ":$port" 2>/dev/null | awk 'NR>1 {print $2}' | sort -u)
        
        if [ -z "$pids" ]; then
            echo -e "${YELLOW}[!] Could not identify process on port $port${NC}"
            return 1
        fi
        
        for pid in $pids; do
            echo -e "${YELLOW}    Sending SIGTERM to PID $pid...${NC}"
            kill -TERM "$pid" 2>/dev/null || true
        done
        
        # Wait a bit
        sleep 2
        
        # Check if ports are freed
        if is_port_in_use "$port"; then
            echo -e "${YELLOW}[!] Process still running, sending SIGKILL...${NC}"
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
        echo -e "${GREEN}[✓] Port $port is already available${NC}"
        return 0
    fi
}

# Free occupied ports
free_ports() {
    echo -e "${YELLOW}Freeing occupied ports...${NC}"
    echo ""
    
    local success_count=0
    local failed_count=0
    
    for service in "${!PORTS[@]}"; do
        local port=${PORTS[$service]}
        local display_name=${SERVICE_DISPLAY[$service]:-$service}
        
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
}

# Monitor ports in real-time
monitor_ports() {
    echo -e "${YELLOW}Monitoring ports (Press Ctrl+C to exit)...${NC}"
    echo ""
    
    while true; do
        clear
        echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
        echo -e "${PURPLE}  B2Connect - Port Monitor${NC}"
        echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
        echo ""
        echo -e "${CYAN}Real-time Port Status:${NC}"
        echo ""
        
        for service in "${!PORTS[@]}"; do
            local port=${PORTS[$service]}
            local display_name=${SERVICE_DISPLAY[$service]:-$service}
            
            if is_port_in_use "$port"; then
                echo -e "  ${RED}✗${NC} $display_name (Port $port): ${RED}IN USE${NC}"
                local process=$(get_process_using_port "$port")
                [ ! -z "$process" ] && echo -e "     ${YELLOW}$process${NC}"
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
    echo -e "${CYAN}Usage:${NC} ./check-ports.sh [options]"
    echo ""
    echo -e "${CYAN}Options:${NC}"
    echo -e "  ${BLUE}--check${NC}      Check port availability (default)"
    echo -e "  ${BLUE}--free${NC}       Free up occupied ports"
    echo -e "  ${BLUE}--monitor${NC}    Monitor ports in real-time"
    echo -e "  ${BLUE}--help${NC}       Show this help message"
    echo ""
    echo -e "${CYAN}Examples:${NC}"
    echo -e "  ${BLUE}./check-ports.sh${NC}             # Check port status"
    echo -e "  ${BLUE}./check-ports.sh --free${NC}      # Free occupied ports"
    echo -e "  ${BLUE}./check-ports.sh --monitor${NC}   # Monitor ports in real-time"
    echo ""
    echo -e "${CYAN}Ports Monitored:${NC}"
    for service in "${!PORTS[@]}"; do
        local display_name=${SERVICE_DISPLAY[$service]:-$service}
        echo -e "  ${BLUE}${display_name}${NC} (${PORTS[$service]})"
    done
    echo ""
}

# Main logic
case "$COMMAND" in
    --check)
        check_ports
        ;;
    --free)
        check_ports
        echo ""
        free_ports
        echo ""
        check_ports
        ;;
    --monitor)
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
