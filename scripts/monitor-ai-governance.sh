#!/bin/bash

# AI Governance Compliance Monitor
# Usage: ./monitor-ai-governance.sh [check-type]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
GOVERNANCE_FILE="$REPO_ROOT/.github/instructions/ai-governance.instructions.md"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_header() {
    echo -e "${BLUE}=== $1 ===${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Function to check if governance file exists
check_governance_file() {
    print_header "Governance File Check"
    if [ -f "$GOVERNANCE_FILE" ]; then
        print_success "AI governance instructions file exists"
        local file_size=$(stat -f%z "$GOVERNANCE_FILE" 2>/dev/null || stat -c%s "$GOVERNANCE_FILE" 2>/dev/null)
        echo "File size: $file_size bytes"
        local last_modified=$(stat -f%Sm -t "%Y-%m-%d %H:%M:%S" "$GOVERNANCE_FILE" 2>/dev/null || stat -c"%y" "$GOVERNANCE_FILE" 2>/dev/null | cut -d'.' -f1)
        echo "Last modified: $last_modified"
    else
        print_error "AI governance instructions file missing: $GOVERNANCE_FILE"
        return 1
    fi
}

# Function to check agent compliance
check_agent_compliance() {
    print_header "Agent Compliance Check"
    local agent_dir="$REPO_ROOT/.github/agents"
    local total_agents=0
    local compliant_agents=0

    if [ ! -d "$agent_dir" ]; then
        print_error "Agent directory not found: $agent_dir"
        return 1
    fi

    for agent_file in "$agent_dir"/*.agent.md; do
        if [ -f "$agent_file" ]; then
            total_agents=$((total_agents + 1))
            local filename=$(basename "$agent_file")

            # Check if agent file contains governance references
            if grep -q "governance\|compliance\|security\|performance" "$agent_file" 2>/dev/null; then
                print_success "$filename includes governance references"
                compliant_agents=$((compliant_agents + 1))
            else
                print_warning "$filename missing governance references"
            fi
        fi
    done

    echo "Total agents: $total_agents"
    echo "Compliant agents: $compliant_agents"
    local compliance_rate=$((compliant_agents * 100 / total_agents))
    echo "Compliance rate: ${compliance_rate}%"

    if [ $compliance_rate -ge 95 ]; then
        print_success "Agent compliance rate meets target (>95%)"
    else
        print_warning "Agent compliance rate below target"
    fi
}

# Function to check instruction files compliance
check_instruction_compliance() {
    print_header "Instruction Files Compliance Check"
    local instruction_dir="$REPO_ROOT/.github/instructions"
    local total_files=0
    local compliant_files=0

    if [ ! -d "$instruction_dir" ]; then
        print_error "Instructions directory not found: $instruction_dir"
        return 1
    fi

    for instruction_file in "$instruction_dir"/*.instructions.md; do
        if [ -f "$instruction_file" ] && [ "$instruction_file" != "$GOVERNANCE_FILE" ]; then
            total_files=$((total_files + 1))
            local filename=$(basename "$instruction_file")

            # Check if instruction file references lessons learned or governance
            if grep -q "lessons learned\|governance\|compliance" "$instruction_file" 2>/dev/null; then
                print_success "$filename includes governance integration"
                compliant_files=$((compliant_files + 1))
            else
                print_warning "$filename missing governance integration"
            fi
        fi
    done

    echo "Total instruction files: $total_files"
    echo "Compliant files: $compliant_files"
    local compliance_rate=$((compliant_files * 100 / total_files))
    echo "Compliance rate: ${compliance_rate}%"
}

# Function to check knowledge base structure
check_knowledge_structure() {
    print_header "Knowledge Base Structure Check"
    local kb_dir="$REPO_ROOT/.ai/knowledgebase"
    local lessons_dir="$REPO_ROOT/.ai/lessons"

    # Check knowledge base
    if [ -d "$kb_dir" ]; then
        local kb_files=$(find "$kb_dir" -name "*.md" | wc -l)
        print_success "Knowledge base exists with $kb_files files"
    else
        print_warning "Knowledge base directory missing: $kb_dir"
    fi

    # Check lessons learned structure
    if [ -d "$lessons_dir" ]; then
        local incident_files=$(find "$lessons_dir/incidents" -name "*.md" 2>/dev/null | wc -l || echo 0)
        local pattern_files=$(find "$lessons_dir/patterns" -name "*.md" 2>/dev/null | wc -l || echo 0)
        local prevention_files=$(find "$lessons_dir/prevention" -name "*.md" 2>/dev/null | wc -l || echo 0)

        print_success "Lessons learned structure exists"
        echo "Incidents: $incident_files files"
        echo "Patterns: $pattern_files files"
        echo "Prevention: $prevention_files files"
    else
        print_warning "Lessons learned directory missing: $lessons_dir"
    fi
}

# Function to generate compliance report
generate_report() {
    print_header "AI Governance Compliance Report"
    echo "Generated: $(date)"
    echo "Repository: $(basename "$REPO_ROOT")"
    echo ""

    check_governance_file
    echo ""
    check_agent_compliance
    echo ""
    check_instruction_compliance
    echo ""
    check_knowledge_structure
    echo ""

    print_header "Summary & Recommendations"

    # Calculate overall compliance score
    local score=0
    local max_score=4

    [ -f "$GOVERNANCE_FILE" ] && score=$((score + 1))
    [ -d "$REPO_ROOT/.github/agents" ] && score=$((score + 1))
    [ -d "$REPO_ROOT/.ai/lessons" ] && score=$((score + 1))
    [ -d "$REPO_ROOT/.ai/knowledgebase" ] && score=$((score + 1))

    local compliance_percentage=$((score * 100 / max_score))
    echo "Overall compliance score: ${compliance_percentage}%"

    if [ $compliance_percentage -ge 90 ]; then
        print_success "Excellent compliance - AI governance framework fully operational"
    elif [ $compliance_percentage -ge 75 ]; then
        print_warning "Good compliance - minor improvements needed"
    else
        print_error "Poor compliance - immediate attention required"
    fi

    echo ""
    echo "Recommendations:"
    echo "1. Regular monitoring (weekly) of compliance metrics"
    echo "2. Quarterly review of governance rules effectiveness"
    echo "3. Training sessions for new governance requirements"
    echo "4. Automated alerts for compliance violations"
}

# Main execution
case "${1:-all}" in
    "governance")
        check_governance_file
        ;;
    "agents")
        check_agent_compliance
        ;;
    "instructions")
        check_instruction_compliance
        ;;
    "knowledge")
        check_knowledge_structure
        ;;
    "all"|*)
        generate_report
        ;;
esac

print_success "Compliance check completed"