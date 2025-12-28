#!/bin/bash

##############################################################################
# Role-Based GitHub Issue Filter
# Filtert offene Issues nach Rolle und zeigt nur relevante Issues an
# Usage: ./role-based-issue-filter.sh [backend|frontend|security|devops|qa|product]
##############################################################################

set -e

ROLE="${1:-backend}"
WORKSPACE="/Users/holger/Documents/Projekte/B2Connect"

# Farben
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# GitHub Query Länge zählen (für Context-Management)
count_context_tokens() {
    local json="$1"
    # Vereinfachte Tokenisierung (4 Zeichen ≈ 1 Token)
    echo "${#json}" / 4 | bc
}

##############################################################################
# BACKEND DEVELOPER ISSUES
##############################################################################
filter_backend() {
    echo -e "${BLUE}💻 Backend Developer Issues${NC}"
    echo "Relevant P0 Components: P0.1 (Audit), P0.6 (E-Commerce), P0.7 (AI), P0.9 (E-Rechnung)"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:backend OR label:api OR label:database OR (label:P0 AND label:backend)" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("backend") or contains("api") or contains("database")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}Backend Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:backend" \
        --json title \
        --jq 'length' | xargs echo "Total Backend Issues:"
}

##############################################################################
# FRONTEND DEVELOPER ISSUES
##############################################################################
filter_frontend() {
    echo -e "${BLUE}🎨 Frontend Developer Issues${NC}"
    echo "Relevant P0 Components: P0.6 (E-Commerce UI), P0.8 (BITV Accessibility)"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:frontend OR label:ui OR label:accessibility OR (label:P0 AND label:frontend)" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("frontend") or contains("ui") or contains("accessibility")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}Frontend Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:frontend" \
        --json title \
        --jq 'length' | xargs echo "Total Frontend Issues:"
}

##############################################################################
# SECURITY ENGINEER ISSUES
##############################################################################
filter_security() {
    echo -e "${BLUE}🔐 Security Engineer Issues${NC}"
    echo "Relevant P0 Components: P0.1 (Audit), P0.2 (Encryption), P0.3 (Incident Response), P0.5 (Key Mgmt), P0.7 (AI Act)"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:security OR label:compliance OR label:encryption OR label:audit" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("security") or contains("compliance") or contains("encryption") or contains("audit")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}Security Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:security OR label:compliance" \
        --json title \
        --jq 'length' | xargs echo "Total Security Issues:"
}

##############################################################################
# DEVOPS ENGINEER ISSUES
##############################################################################
filter_devops() {
    echo -e "${BLUE}⚙️ DevOps Engineer Issues${NC}"
    echo "Relevant P0 Components: P0.3 (Incident Response), P0.4 (Network Segmentation), P0.5 (Key Management)"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:infrastructure OR label:devops OR label:ci-cd OR label:orchestration" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("infrastructure") or contains("devops") or contains("ci-cd") or contains("orchestration")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}DevOps Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:infrastructure OR label:devops" \
        --json title \
        --jq 'length' | xargs echo "Total DevOps Issues:"
}

##############################################################################
# QA ENGINEER ISSUES
##############################################################################
filter_qa() {
    echo -e "${BLUE}🧪 QA Engineer Issues${NC}"
    echo "Relevant P0 Components: P0.6 (E-Commerce Tests), P0.7 (AI Tests), P0.8 (BITV Tests), P0.9 (E-Rechnung Tests)"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:testing OR label:test OR label:qa OR (label:P0 AND (label:P0.6 OR label:P0.7 OR label:P0.8 OR label:P0.9))" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("test") or contains("qa") or contains("P0")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}QA Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:testing" \
        --json title \
        --jq 'length' | xargs echo "Total Testing Issues:"
}

##############################################################################
# PRODUCT OWNER ISSUES
##############################################################################
filter_product() {
    echo -e "${BLUE}📋 Product Owner Issues${NC}"
    echo "Relevant Documents: Roadmap, Specs, Requirements"
    echo ""
    
    gh issue list \
        --state open \
        --limit 50 \
        --search "label:documentation OR label:research OR label:epic" \
        --json number,title,labels,assignees,milestone \
        --jq '
            .[] |
            select(.labels[].name | contains("documentation") or contains("research") or contains("epic")) |
            "\(.number) | \(.title) | \(.milestone.title // "No Sprint") | \(.assignees[0].login // "Unassigned")"
        '
    
    echo ""
    echo -e "${YELLOW}Documentation Issues Summary:${NC}"
    gh issue list \
        --state open \
        --search "label:documentation" \
        --json title \
        --jq 'length' | xargs echo "Total Documentation Issues:"
}

##############################################################################
# MAIN
##############################################################################

case "$ROLE" in
    backend)
        filter_backend
        ;;
    frontend)
        filter_frontend
        ;;
    security)
        filter_security
        ;;
    devops)
        filter_devops
        ;;
    qa)
        filter_qa
        ;;
    product)
        filter_product
        ;;
    all)
        echo -e "${GREEN}=== ALL ROLES - ISSUE OVERVIEW ===${NC}"
        filter_backend && echo "" && echo "---" && echo ""
        filter_frontend && echo "" && echo "---" && echo ""
        filter_security && echo "" && echo "---" && echo ""
        filter_devops && echo "" && echo "---" && echo ""
        filter_qa && echo "" && echo "---" && echo ""
        filter_product
        ;;
    *)
        echo -e "${RED}Usage: $0 [backend|frontend|security|devops|qa|product|all]${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${GREEN}✅ Context optimized for role: $ROLE${NC}"
