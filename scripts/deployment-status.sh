#!/usr/bin/env bash

# B2Connect Deployment Status Check Script
# Überprüft den Status aller Services und zeigt System-Informationen

set -euo pipefail

# Farben
BLUE='\033[0;34m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Hilfsfunktionen
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[✓]${NC} $1"
}

log_error() {
    echo -e "${RED}[✗]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[!]${NC} $1"
}

# Health Check für einen Service
check_service() {
    local service_name=$1
    local url=$2
    local port=$3
    
    echo -n "  $service_name: "
    
    if curl -s "$url" > /dev/null 2>&1; then
        log_success "Online (Port $port)"
        return 0
    else
        log_error "Offline"
        return 1
    fi
}

# Hauptfunktionen
check_local_deployment() {
    echo ""
    echo -e "${BLUE}=== Local Deployment Status ===${NC}"
    echo ""
    
    local healthy_count=0
    local total_count=5
    
    # Prüfe Services
    check_service "AppHost" "http://localhost:9000/health" "9000" && ((healthy_count++))
    check_service "API Gateway" "http://localhost:5000/health" "5000" && ((healthy_count++))
    check_service "Auth Service" "http://localhost:5001/health" "5001" && ((healthy_count++))
    check_service "Tenant Service" "http://localhost:5002/health" "5002" && ((healthy_count++))
    check_service "Localization Service" "http://localhost:5003/health" "5003" && ((healthy_count++))
    
    echo ""
    echo "Summary: $healthy_count/$total_count Services Online"
    
    if [ $healthy_count -eq $total_count ]; then
        log_success "All services are healthy!"
        return 0
    else
        log_warning "Some services are offline"
        return 1
    fi
}

check_docker_deployment() {
    echo ""
    echo -e "${BLUE}=== Docker Deployment Status ===${NC}"
    echo ""
    
    # Prüfe ob Docker läuft
    if ! command -v docker &> /dev/null; then
        log_error "Docker not installed"
        return 1
    fi
    
    log_info "Checking running containers..."
    
    # Gettings running containers für B2Connect
    containers=$(docker ps --filter "label=com.b2connect" -q 2>/dev/null || true)
    
    if [ -z "$containers" ]; then
        log_warning "No B2Connect containers running"
        echo ""
        echo "To start containers, run:"
        echo "  docker-compose -f backend/docker-compose.aspire.yml up -d"
        return 1
    fi
    
    docker ps --filter "label=com.b2connect" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
    
    log_success "Docker containers running"
    return 0
}

check_kubernetes_deployment() {
    echo ""
    echo -e "${BLUE}=== Kubernetes Deployment Status ===${NC}"
    echo ""
    
    # Prüfe ob kubectl installiert ist
    if ! command -v kubectl &> /dev/null; then
        log_error "kubectl not installed"
        return 1
    fi
    
    # Prüfe Namespace
    if ! kubectl get namespace b2connect 2>/dev/null; then
        log_warning "Namespace 'b2connect' not found"
        echo ""
        echo "To setup Kubernetes, run:"
        echo "  ./kubernetes-setup.sh"
        return 1
    fi
    
    log_info "Pods in b2connect namespace:"
    kubectl get pods -n b2connect --no-headers 2>/dev/null | while read line; do
        pod_name=$(echo "$line" | awk '{print $1}')
        status=$(echo "$line" | awk '{print $3}')
        
        if [ "$status" = "Running" ]; then
            echo -e "  ${GREEN}✓${NC} $pod_name ($status)"
        else
            echo -e "  ${RED}✗${NC} $pod_name ($status)"
        fi
    done
    
    echo ""
    log_info "Services in b2connect namespace:"
    kubectl get services -n b2connect --no-headers 2>/dev/null | while read line; do
        svc_name=$(echo "$line" | awk '{print $1}')
        cluster_ip=$(echo "$line" | awk '{print $3}')
        echo "  $svc_name: $cluster_ip"
    done
    
    log_success "Kubernetes deployment active"
    return 0
}

check_system_resources() {
    echo ""
    echo -e "${BLUE}=== System Resources ===${NC}"
    echo ""
    
    # CPU
    cpu_count=$(sysctl -n hw.ncpu 2>/dev/null || nproc 2>/dev/null || echo "unknown")
    log_info "CPU Cores: $cpu_count"
    
    # Memory
    if [[ "$OSTYPE" == "darwin"* ]]; then
        total_mem=$(sysctl -n hw.memsize | awk '{print $1/1024/1024/1024}')
        used_mem=$(vm_stat | grep "Pages active" | awk '{print $3}' | tr -d '.' | awk '{print $1*4096/1024/1024/1024}')
        echo "  Total: ${total_mem:.1f}GB"
        echo "  In Use: ~${used_mem:.1f}GB"
    else
        free -h | grep Mem
    fi
    
    # Disk
    echo ""
    log_info "Disk Space:"
    df -h . | tail -1 | awk '{print "  Available: " $4 " / Total: " $2}'
    
    # Docker
    echo ""
    log_info "Docker Status:"
    if command -v docker &> /dev/null; then
        if docker ps > /dev/null 2>&1; then
            log_success "Docker daemon running"
            docker images --filter "reference=b2connect*" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" 2>/dev/null | head -5
        else
            log_error "Docker daemon not responding"
        fi
    else
        log_error "Docker not installed"
    fi
}

check_dependencies() {
    echo ""
    echo -e "${BLUE}=== Installed Tools ===${NC}"
    echo ""
    
    check_tool() {
        local tool=$1
        local min_version=$2
        
        echo -n "  $tool: "
        if command -v "$tool" &> /dev/null; then
            version=$("$tool" --version 2>&1 | head -1)
            log_success "$version"
        else
            log_error "Not installed"
        fi
    }
    
    check_tool "dotnet" "10.0"
    check_tool "node" "18.0"
    check_tool "npm" "10.0"
    check_tool "docker" "20.10"
    check_tool "kubectl" "1.24"
    check_tool "helm" "3.0"
    check_tool "git" "2.0"
}

# Health endpoint detailed info
show_health_details() {
    echo ""
    echo -e "${BLUE}=== Detailed Health Information ===${NC}"
    echo ""
    
    if curl -s http://localhost:9000/api/health > /tmp/health.json 2>/dev/null; then
        log_success "Health endpoint responding"
        echo ""
        cat /tmp/health.json | jq '.' 2>/dev/null || cat /tmp/health.json
    else
        log_warning "Health endpoint not responding"
    fi
}

# Hauptmenü
main() {
    echo ""
    echo -e "${BLUE}╔════════════════════════════════════════════╗${NC}"
    echo -e "${BLUE}║  B2Connect Deployment Status Check        ║${NC}"
    echo -e "${BLUE}║  Aspire 10 - Microservices Hosting       ║${NC}"
    echo -e "${BLUE}╚════════════════════════════════════════════╝${NC}"
    echo ""
    
    # Check modes
    if [ "$1" == "local" ]; then
        check_local_deployment
    elif [ "$1" == "docker" ]; then
        check_docker_deployment
    elif [ "$1" == "kubernetes" ]; then
        check_kubernetes_deployment
    elif [ "$1" == "health" ]; then
        show_health_details
    elif [ "$1" == "resources" ]; then
        check_system_resources
    elif [ "$1" == "dependencies" ]; then
        check_dependencies
    elif [ "$1" == "all" ]; then
        check_dependencies
        check_system_resources
        check_local_deployment || true
        check_docker_deployment || true
        check_kubernetes_deployment || true
        show_health_details || true
    else
        # Default: Quick check
        log_info "Quick Status Check (use --help for more options)"
        echo ""
        echo "Usage: ./deployment-status.sh [option]"
        echo ""
        echo "Options:"
        echo "  local       - Check local bash deployment (aspire-start.sh)"
        echo "  docker      - Check Docker Compose deployment"
        echo "  kubernetes  - Check Kubernetes deployment"
        echo "  health      - Show detailed health endpoint info"
        echo "  resources   - Show system resource usage"
        echo "  dependencies - Check installed tools"
        echo "  all         - Run all checks (verbose)"
        echo ""
        
        # Quick check
        check_local_deployment || true
    fi
}

# Execute
main "$@"
