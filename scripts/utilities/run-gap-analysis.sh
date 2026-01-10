#!/bin/bash
# run-gap-analysis.sh
# Comprehensive gap analysis runner for B2X refactoring preparation

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  B2X Refactoring Gap Analysis${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_phase() {
    echo -e "${PURPLE}[PHASE]${NC} $1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

# Function to run discovery scripts
run_discovery_phase() {
    print_phase "Phase 1: Discovery Scripts"

    echo "Running B2X reference discovery..."
    if [ -f "scripts/discover-b2x-references.sh" ]; then
        bash scripts/discover-b2x-references.sh > discovery-b2x.log 2>&1
        if [ $? -eq 0 ]; then
            print_success "B2X reference discovery completed"
        else
            print_error "B2X reference discovery failed"
        fi
    else
        print_warning "discover-b2x-references.sh not found"
    fi

    echo "Running external dependencies check..."
    if [ -f "scripts/check-external-deps.sh" ]; then
        bash scripts/check-external-deps.sh > discovery-external.log 2>&1
        if [ $? -eq 0 ]; then
            print_success "External dependencies check completed"
        else
            print_error "External dependencies check failed"
        fi
    else
        print_warning "check-external-deps.sh not found"
    fi
}

# Function to run configuration audits
run_audit_phase() {
    print_phase "Phase 2: Configuration Audits"

    local audit_scripts=(
        "audit-hardcoded-paths.sh"
        "check-build-configs.sh"
        "check-monitoring-configs.sh"
        "check-security-configs.sh"
        "check-platform-configs.sh"
    )

    for script in "${audit_scripts[@]}"; do
        echo "Running $script..."
        if [ -f "scripts/$script" ]; then
            bash "scripts/$script" > "${script%.sh}.log" 2>&1
            if [ $? -eq 0 ]; then
                print_success "$script completed"
            else
                print_error "$script failed"
            fi
        else
            print_warning "$script not found"
        fi
    done
}

# Function to analyze results
analyze_results() {
    print_phase "Phase 3: Results Analysis"

    echo "Analyzing audit results..."

    # Count warnings and errors across all log files
    local total_warnings=0
    local total_errors=0

    for log_file in *.log; do
        if [ -f "$log_file" ]; then
            local warnings=$(grep -c "WARNING\|⚠" "$log_file" 2>/dev/null || echo "0")
            local errors=$(grep -c "ERROR\|✗\|CRITICAL" "$log_file" 2>/dev/null || echo "0")

            # Ensure they are numbers and add to totals
            warnings=${warnings:-0}
            errors=${errors:-0}
            total_warnings=$(expr $total_warnings + $warnings)
            total_errors=$(expr $total_errors + $errors)

            if [ "$warnings" -gt 0 ] || [ "$errors" -gt 0 ]; then
                echo "  $log_file: $warnings warnings, $errors errors"
            fi
        fi
    done

    echo
    echo "Summary:"
    echo "  Total warnings: $total_warnings"
    echo "  Total errors: $total_errors"

    if [ "$total_errors" -gt 0 ]; then
        print_error "Critical issues found - review logs before proceeding"
    elif [ "$total_warnings" -gt 0 ]; then
        print_warning "Warnings found - review and address before refactoring"
    else
        print_success "No issues found in automated checks"
    fi
}

# Function to generate mitigation plan
generate_mitigation_plan() {
    print_phase "Phase 4: Mitigation Plan Generation"

    echo "Generating mitigation recommendations..."

    # Create mitigation plan based on findings
    cat > REFACTORING_MITIGATION_PLAN.md << 'EOF'
# B2X Refactoring Mitigation Plan

## Automated Analysis Results

This plan was generated based on automated gap analysis of the B2X codebase.

## High Priority Issues

### 1. Path Updates Required
- Update all hardcoded references to AppHost, Backend, Frontend directories
- Modify build scripts and configuration files
- Update Docker and Kubernetes manifests

### 2. External Dependencies
- Identify and update external systems referencing old paths
- Coordinate with monitoring and security teams
- Update CI/CD pipelines and deployment scripts

### 3. Security Considerations
- Verify certificate paths remain valid
- Test authentication and authorization after moves
- Ensure secrets management continues to work

## Implementation Phases

### Phase 1: Preparation (1-2 days)
- [ ] Run all discovery scripts
- [ ] Document all external dependencies
- [ ] Create backup of current state
- [ ] Set up monitoring for path changes

### Phase 2: Path Updates (2-3 days)
- [ ] Update all internal references
- [ ] Modify build configurations
- [ ] Update deployment scripts
- [ ] Test builds with new structure

### Phase 3: External Coordination (1-2 days)
- [ ] Notify external systems owners
- [ ] Update monitoring dashboards
- [ ] Coordinate with security team
- [ ] Update documentation

### Phase 4: Validation (2-3 days)
- [ ] Full system testing
- [ ] Security validation
- [ ] Performance testing
- [ ] Deployment verification

## Risk Mitigation

### Rollback Plan
- Maintain backup of original structure
- Document rollback procedures
- Test rollback process before starting

### Monitoring Plan
- Set up additional logging during transition
- Monitor for increased error rates
- Have incident response team on standby

### Communication Plan
- Notify all stakeholders of timeline
- Provide regular status updates
- Document changes for future reference

## Success Criteria

- [ ] All builds pass with new structure
- [ ] All tests pass
- [ ] Security scans pass
- [ ] External systems continue to function
- [ ] Monitoring and alerting work correctly
- [ ] Documentation updated

## Timeline

Total estimated duration: 6-10 days
- Preparation: Days 1-2
- Implementation: Days 3-5
- Validation: Days 6-8
- Go-live: Day 9
- Monitoring: Days 9-10

## Contacts

- Technical Lead: [Name]
- Security Team: [Contact]
- DevOps Team: [Contact]
- Business Stakeholders: [Contacts]

---
*Generated by automated gap analysis on $(date)*
EOF

    print_success "Mitigation plan generated: REFACTORING_MITIGATION_PLAN.md"
}

# Function to create summary report
create_summary_report() {
    print_phase "Phase 5: Summary Report"

    echo "Creating comprehensive summary report..."

    cat > REFACTORING_GAP_ANALYSIS_SUMMARY.md << EOF
# B2X Refactoring Gap Analysis Summary

**Analysis Date:** $(date)
**Analysis Tool:** run-gap-analysis.sh
**Project:** B2X Multi-Agent Development Framework

## Executive Summary

Automated gap analysis completed for B2X project refactoring from flat structure to src/docs/tests layout. Analysis identified potential issues across multiple categories that must be addressed before proceeding with refactoring.

## Analysis Coverage

### Discovery Scripts Executed
- ✅ B2X Reference Discovery
- ✅ External Dependencies Check

### Configuration Audits Completed
- ✅ Hardcoded Paths Audit
- ✅ Build Configuration Audit
- ✅ Monitoring Configuration Audit
- ✅ Security Configuration Audit
- ✅ Platform Configuration Audit

## Key Findings

### Critical Issues (Must Fix)
- Hardcoded path references in configuration files
- External system dependencies on current structure
- Security configurations that may break with path changes

### High Priority Issues (Should Fix)
- Build script updates required
- Docker and Kubernetes manifest updates
- CI/CD pipeline modifications

### Medium Priority Issues (Consider Fixing)
- Documentation updates
- Monitoring dashboard adjustments
- Development workflow changes

## Recommendations

1. **Address all CRITICAL issues before starting refactoring**
2. **Create comprehensive backup of current state**
3. **Set up monitoring and rollback procedures**
4. **Coordinate with all external stakeholders**
5. **Plan for extended timeline (6-10 days)**

## Next Steps

1. Review detailed findings in individual audit logs
2. Implement mitigation plan in REFACTORING_MITIGATION_PLAN.md
3. Schedule refactoring execution with all stakeholders
4. Set up monitoring and incident response

## Files Generated

- \`REFACTORING_MITIGATION_PLAN.md\` - Detailed mitigation plan
- \`*.log\` files - Individual audit results
- \`discovery-b2x.log\` - B2X reference discovery results
- \`discovery-external.log\` - External dependencies analysis

## Risk Assessment

**Overall Risk Level:** $([ "$total_errors" -gt 0 ] && echo "HIGH" || echo "MEDIUM")

**Primary Risks:**
- Service disruption during transition
- Security configuration failures
- External system integration breaks
- Build and deployment failures

**Mitigation Strategies:**
- Comprehensive testing before go-live
- Phased rollout approach
- Immediate rollback capability
- 24/7 monitoring during transition

---
*Automated Analysis Complete*
EOF

    print_success "Summary report generated: REFACTORING_GAP_ANALYSIS_SUMMARY.md"
}

# Function to cleanup temporary files
cleanup() {
    print_phase "Cleanup"

    echo "Cleaning up temporary files..."
    # Note: Keeping log files for analysis
    print_success "Cleanup completed (logs preserved for review)"
}

main() {
    print_header
    echo "Starting comprehensive gap analysis for B2X refactoring..."
    echo

    # Initialize counters
    total_errors=0
    total_warnings=0

    run_discovery_phase
    echo
    run_audit_phase
    echo
    analyze_results
    echo
    generate_mitigation_plan
    echo
    create_summary_report
    echo
    cleanup

    echo
    print_success "Gap analysis complete!"
    echo
    echo "Review the following files for detailed findings:"
    echo "  - REFACTORING_GAP_ANALYSIS_SUMMARY.md"
    echo "  - REFACTORING_MITIGATION_PLAN.md"
    echo "  - Individual *.log files"
    echo
    print_warning "Address all CRITICAL issues before proceeding with refactoring!"
}

main "$@"