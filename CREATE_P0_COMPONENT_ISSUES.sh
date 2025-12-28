#!/bin/bash

# B2Connect P0 Component Issue Creator
# Creates all P0 compliance component issues and adds them to the Planner project
# Usage: ./CREATE_P0_COMPONENT_ISSUES.sh
# Requires: gh CLI, jq

set -e

OWNER="HRasch"
REPO="B2Connect"
PROJECT_ID="PVT_kwHOAWN9Gs4BLeEd"  # Planner project ID
READY_STATUS_ID="61e4505c"
BACKLOG_STATUS_ID="f75ad846"

echo "🚀 B2Connect P0 Component Issue Creator"
echo "======================================"
echo "Creating P0 component parent and child issues..."
echo ""

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;36m'
NC='\033[0m' # No Color

# Track created issues
CREATED_ISSUES=()

# Helper function to create issue and return ID
create_issue() {
    local title="$1"
    local body="$2"
    local labels="$3"
    
    echo -ne "${BLUE}Creating:${NC} $title... "
    
    local issue_id=$(gh issue create \
        --title "$title" \
        --body "$body" \
        --label "$labels" \
        --repo "$OWNER/$REPO" \
        --format json | jq -r '.id')
    
    CREATED_ISSUES+=("$issue_id")
    echo -e "${GREEN}✓${NC} (ID: $issue_id)"
    
    echo "$issue_id"
}

# Helper function to add issue to project
add_to_project() {
    local issue_id="$1"
    local status_id="$2"
    
    echo -ne "${BLUE}Adding to project:${NC} $issue_id... "
    
    gh api graphql -f query='
    mutation {
        addProjectV2ItemById(input: {
            projectId: "'$PROJECT_ID'"
            contentId: "'$issue_id'"
        }) {
            item {
                id
            }
        }
    }' > /tmp/add_result.json
    
    local item_id=$(jq -r '.data.addProjectV2ItemById.item.id' /tmp/add_result.json)
    
    # Set status
    gh api graphql -f query='
    mutation {
        updateProjectV2ItemFieldValue(input: {
            projectId: "'$PROJECT_ID'"
            itemId: "'$item_id'"
            fieldId: "PVTSSF_lAHOAWN9Gs4BLeEdzg7CLQI"
            value: {singleSelectOptionId: "'$status_id'"}
        }) {
            projectV2Item {
                id
            }
        }
    }' > /dev/null 2>&1
    
    echo -e "${GREEN}✓${NC}"
}

# ============================================================================
# P0.1: Audit Logging System
# ============================================================================
echo ""
echo -e "${YELLOW}P0.1: Audit Logging System${NC}"

P0_1_PARENT=$(create_issue \
    "P0.1: Audit Logging System (Immutable Event Tracking)" \
    "**Regulatory Requirement:** NIS2, GDPR
**Component Owner:** Security Engineer
**Testing:** 5 compliance tests mandatory
**Deadline:** Week 2

## Parent Epic
Implement comprehensive audit logging system for all data modifications (CREATE, UPDATE, DELETE).

## Key Requirements
- [ ] Immutable audit log (no update/delete possible)
- [ ] All CRUD operations logged with before/after values
- [ ] Tamper detection via hash verification
- [ ] SIEM integration ready
- [ ] Tenant isolation enforced

## Acceptance Criteria
- Audit logs cannot be modified or deleted
- All entity changes captured within 100ms
- Hash verification prevents tampering
- Logs formatted for SIEM ingestion

## Definition of Ready
- [ ] Security review completed
- [ ] Estimated effort: 1 FTE, 1 week
- [ ] No blockers (Encryption keys available)
- [ ] Type: P0, Priority: P0" \
    "type: epic,priority: p0,area: compliance")

add_to_project "$P0_1_PARENT" "$READY_STATUS_ID"

# P0.1 Child Issues
create_issue \
    "P0.1.1: Audit Log Entity & EF Core Configuration" \
    "Implement AuditLogEntry entity with immutable properties and EF Core mapping.

Parent: P0.1
Depends on: (None)
Effort: 1 day (S)

## Requirements
- Create AuditLogEntry entity (Id, TenantId, UserId, EntityType, Action, CreatedAt, BeforeValues, AfterValues)
- Configure EF Core mapping (table name, indexes, constraints)
- Add IAsyncInterceptor for EF Core to capture changes
- Mark as immutable (no updates allowed)

## Testing
- Unit tests for entity construction
- Integration test: verify audit logs created on CRUD operations
- Test immutability (update attempt fails)" \
    "type: task,priority: p0,area: compliance,area: backend" | xargs -I {} add_to_project {} "$READY_STATUS_ID"

create_issue \
    "P0.1.2: EF Core Interceptor for Automatic Logging" \
    "Implement SaveChangesInterceptor to automatically log all entity changes.

Parent: P0.1
Depends on: P0.1.1
Effort: 1 day (S)

## Requirements
- EF Core SaveChangesInterceptor implementation
- Capture before/after JSON snapshots
- Calculate hash for tamper detection
- Timestamp and user ID injection

## Testing
- Test: Interceptor fires for all CRUD operations
- Test: Before/after values correctly captured
- Test: Hash calculation prevents tampering" \
    "type: task,priority: p0,area: compliance,area: backend" | xargs -I {} add_to_project {} "$BACKLOG_STATUS_ID"

create_issue \
    "P0.1.3: Audit Logging Service & Query Interface" \
    "Create AuditService for querying audit logs and AuditRepository interface.

Parent: P0.1
Depends on: P0.1.2
Effort: 1 day (S)

## Requirements
- AuditService with methods: LogAsync(tenantId, userId, entity, action, before, after)
- AuditRepository with: GetByEntityAsync(tenantId, entityId)
- Query filters by TenantId, EntityType, DateRange, UserId
- Audit logs read-only (repository has no Update/Delete)

## Testing
- Test: Logs retrieved correctly
- Test: TenantId filter prevents cross-tenant access" \
    "type: task,priority: p0,area: compliance,area: backend" | xargs -I {} add_to_project {} "$BACKLOG_STATUS_ID"

create_issue \
    "P0.1.4: SIEM Integration & Event Forwarding" \
    "Implement audit log forwarding to SIEM (Splunk/ELK) for monitoring.

Parent: P0.1
Depends on: P0.1.3
Effort: 2 days (S)

## Requirements
- ISiemService interface for forwarding
- Batch sending (1000 logs per batch, 1min delay)
- Retry logic (exponential backoff)
- Dead-letter queue for failed events

## Testing
- Test: Logs forwarded to SIEM correctly
- Test: Batch logic works
- Test: Retry on failure" \
    "type: task,priority: p0,area: compliance,area: backend" | xargs -I {} add_to_project {} "$BACKLOG_STATUS_ID"

# ============================================================================
# P0.2: Encryption at Rest
# ============================================================================
echo ""
echo -e "${YELLOW}P0.2: Encryption at Rest (AES-256)${NC}"

P0_2_PARENT=$(create_issue \
    "P0.2: Encryption at Rest (AES-256-GCM)" \
    "**Regulatory Requirement:** GDPR, NIS2
**Component Owner:** Security Engineer
**Testing:** 5 compliance tests mandatory
**Deadline:** Week 2

## Parent Epic
Implement field-level AES-256-GCM encryption for all PII and sensitive data.

## Key Requirements
- [ ] PII encrypted: Email, Phone, Address, SSN, DOB, FirstName, LastName
- [ ] Product cost data encrypted
- [ ] Key rotation policy (annual for NIS2)
- [ ] Encryption/decryption transparent to business logic
- [ ] Performance impact < 5%

## Acceptance Criteria
- All PII encrypted at rest
- Encryption/decryption roundtrip verified
- Key rotation works without data loss
- No performance degradation

## Definition of Ready
- [ ] Security review completed
- [ ] Estimated effort: 1 FTE, 1 week
- [ ] Depends on: P0.5 (Key Management)
- [ ] Type: P0, Priority: P0" \
    "type: epic,priority: p0,area: compliance")

add_to_project "$P0_2_PARENT" "$BACKLOG_STATUS_ID"

# ============================================================================
# P0.6: E-Commerce Legal (14-Day Withdrawal)
# ============================================================================
echo ""
echo -e "${YELLOW}P0.6: E-Commerce Legal Compliance${NC}"

P0_6_PARENT=$(create_issue \
    "P0.6: E-Commerce Legal Compliance (B2B/B2C)" \
    "**Regulatory Requirement:** VVVG (German Consumer Rights), PAngV
**Component Owner:** Backend Developer + Legal
**Testing:** 15 compliance tests mandatory
**Deadline:** Week 4

## Parent Epic
Implement all B2C consumer protections and B2B legal requirements.

## Key Requirements
- [ ] 14-day withdrawal right for B2C
- [ ] Digital contract formation
- [ ] Terms & conditions versioning
- [ ] Right-to-be-forgotten support
- [ ] Audit trail for all legal changes

## Components
- P0.6.1: 14-Day Withdrawal Right
- P0.6.2: Digital Contract Formation
- P0.6.3: Terms & Conditions Versioning
- P0.6.4: Right-to-Be-Forgotten
- P0.6.5: Legal Audit Trail

## Acceptance Criteria
- All B2C customer protections implemented
- All B2B legal requirements met
- 100% test coverage for legal logic
- Legal review passed

## Definition of Ready
- [ ] Legal review completed
- [ ] Estimated effort: 2 weeks (split across team)
- [ ] No blockers
- [ ] Type: P0, Priority: P0" \
    "type: epic,priority: p0,area: compliance,area: e-commerce")

add_to_project "$P0_6_PARENT" "$READY_STATUS_ID"

# P0.6 Child Issues
create_issue \
    "P0.6.1: Implement 14-Day Withdrawal Right (VVVG §357)" \
    "Enable B2C customers to withdraw orders within 14 days of delivery.

Parent: P0.6
Depends on: (None)
Effort: 2 days (M)

## Requirements
- Withdrawal form in customer account
- 14-day countdown from delivery date
- Cannot withdraw after deadline (error)
- Return label auto-generated (DHL)
- Refund processed within 14 days
- Audit log for each withdrawal

## Testing
- Test: Withdrawal allowed within 14 days
- Test: Withdrawal blocked after 14 days
- Test: Return label generated
- Test: Refund processed correctly" \
    "type: task,priority: p0,area: compliance,area: e-commerce" | xargs -I {} add_to_project {} "$READY_STATUS_ID"

create_issue \
    "P0.6.2: Digital Contract Formation & Acceptance" \
    "Implement digital order confirmation and contract formation per VVVG.

Parent: P0.6
Depends on: P0.6.1
Effort: 2 days (M)

## Requirements
- Order confirmation email within 1 hour
- Contract formed on customer acceptance
- Timestamp proof of acceptance
- Unsigned (checkmark = consent)
- Audit trail of all contract events

## Testing
- Test: Email sent within SLA
- Test: Timestamp recorded
- Test: Contract status tracked" \
    "type: task,priority: p0,area: compliance,area: e-commerce" | xargs -I {} add_to_project {} "$READY_STATUS_ID"

create_issue \
    "P0.6.3: Terms & Conditions Versioning" \
    "Implement versioning for T&Cs with customer acceptance tracking.

Parent: P0.6
Depends on: P0.6.2
Effort: 1 day (S)

## Requirements
- T&C version control (v1.0, v1.1, etc.)
- Customers must re-accept on major version changes
- Audit trail: who accepted which version when
- Legal review per version

## Testing
- Test: Customers cannot proceed without accepting current version
- Test: Audit trail shows all acceptances" \
    "type: task,priority: p0,area: compliance,area: e-commerce" | xargs -I {} add_to_project {} "$READY_STATUS_ID"

# ============================================================================
# P0.7: AI Act Compliance
# ============================================================================
echo ""
echo -e "${YELLOW}P0.7: AI Act Compliance${NC}"

P0_7_PARENT=$(create_issue \
    "P0.7: AI Act Compliance (EU Regulation 2024/1689)" \
    "**Regulatory Requirement:** AI Act (EU 2024/1689)
**Component Owner:** Backend Developer + Security
**Testing:** 15 compliance tests mandatory
**Deadline:** Week 5

## Parent Epic
Implement AI transparency and human oversight requirements per EU AI Act.

## Key Requirements
- [ ] AI usage disclosure for customers
- [ ] Human override capability for all AI decisions
- [ ] Audit trail for all AI-assisted decisions
- [ ] Right to explanation (AI decision logging)
- [ ] Bias monitoring (fairness checks)

## Acceptance Criteria
- All AI usage disclosed
- Human override working
- Audit trail complete
- Bias monitoring active

## Definition of Ready
- [ ] Legal review completed
- [ ] Estimated effort: 1.5 weeks
- [ ] No blockers
- [ ] Type: P0, Priority: P0" \
    "type: epic,priority: p0,area: compliance,area: ai-act")

add_to_project "$P0_7_PARENT" "$READY_STATUS_ID"

# ============================================================================
# P0.8: Accessibility (BITV 2.0)
# ============================================================================
echo ""
echo -e "${YELLOW}P0.8: Accessibility Compliance (BITV 2.0)${NC}"

P0_8_PARENT=$(create_issue \
    "P0.8: Accessibility Compliance (BITV 2.0 / WCAG 2.1 AA)" \
    "**Regulatory Requirement:** BFSG (German Accessibility Act, deadline: 28 Juni 2025)
**Component Owner:** Frontend Developer
**Testing:** 12 compliance tests mandatory
**Deadline:** URGENT (Week 1)

## Parent Epic
Implement full WCAG 2.1 Level AA accessibility per German BFSG requirements.

## Key Requirements
- [ ] WCAG 2.1 Level AA compliance
- [ ] Keyboard navigation (all features)
- [ ] Screen reader support (ARIA)
- [ ] Color contrast (4.5:1 ratio)
- [ ] Text resizing support

## Acceptance Criteria
- Automated accessibility scan: 0 violations
- Manual testing: 100% keyboard navigable
- Screen reader tested
- Color contrast verified

## Definition of Ready
- [ ] Legal review completed
- [ ] Estimated effort: 1.5 weeks (frontend-heavy)
- [ ] Deadline: 28. Juni 2025 (BFSG)
- [ ] Type: P0, Priority: P0 (URGENT)" \
    "type: epic,priority: p0,area: compliance,area: accessibility")

add_to_project "$P0_8_PARENT" "$READY_STATUS_ID"

# ============================================================================
# P0.9: E-Rechnung (ZUGFeRD/UBL)
# ============================================================================
echo ""
echo -e "${YELLOW}P0.9: E-Rechnung Integration${NC}"

P0_9_PARENT=$(create_issue \
    "P0.9: E-Rechnung Integration (ZUGFeRD 3.0 / UBL)" \
    "**Regulatory Requirement:** ERechnungsVO (German E-Invoicing Ordinance)
**Component Owner:** Backend Developer
**Testing:** 10 compliance tests mandatory
**Deadline:** Week 6

## Parent Epic
Implement ZUGFeRD 3.0 and UBL 2.1 compliance for German e-invoicing.

## Key Requirements
- [ ] ZUGFeRD 3.0 XML generation
- [ ] Hybrid PDF (XML embedded)
- [ ] UBL 2.1 support
- [ ] Signature validation
- [ ] Archival (10-year retention)

## Components
- P0.9.1: ZUGFeRD XML Generation
- P0.9.2: Hybrid PDF Creation
- P0.9.3: UBL Support
- P0.9.4: Signature Validation
- P0.9.5: Invoice Archival

## Acceptance Criteria
- All invoices valid ZUGFeRD 3.0
- PDF/A-3 compliant
- Archival system working

## Definition of Ready
- [ ] Legal review completed
- [ ] Estimated effort: 1.5 weeks
- [ ] No blockers
- [ ] Type: P0, Priority: P0" \
    "type: epic,priority: p0,area: compliance,area: e-rechnung")

add_to_project "$P0_9_PARENT" "$READY_STATUS_ID"

echo ""
echo -e "${GREEN}✅ Issue Creation Complete!${NC}"
echo ""
echo "📊 Summary:"
echo "==========="
echo "Total issues created: ${#CREATED_ISSUES[@]}"
echo ""
echo "Parent Issues:"
echo "  - P0.1: Audit Logging (Ready status)"
echo "  - P0.6: E-Commerce Legal (Ready status)"
echo "  - P0.7: AI Act Compliance (Ready status)"
echo "  - P0.8: Accessibility/BITV (Ready status)"
echo "  - P0.9: E-Rechnung (Ready status)"
echo ""
echo "🔗 View on GitHub:"
echo "  Planner Project: https://github.com/users/HRasch/projects/5"
echo "  All Issues: https://github.com/HRasch/B2Connect/issues"
echo ""
echo -e "${YELLOW}⚠️  Next Steps:${NC}"
echo "1. Review all created issues in the Planner project"
echo "2. Add remaining P0.2-P0.5 (Infrastructure) components"
echo "3. Verify all child issues have correct dependency links"
echo "4. Schedule sprint kickoff when all P0 components are defined"
