1,# GitHub Project Management Guide for B2Connect

**Version:** 1.0 | **Created:** 28. Dezember 2025 | **Status:** Active

---

## Table of Contents

1. [Overview](#overview)
2. [Project Structure](#project-structure)
3. [GitHub Issues Workflow](#github-issues-workflow)
4. [Project Board Management](#project-board-management)
5. [Compliance Issue Tracking](#compliance-issue-tracking)
6. [Team Coordination](#team-coordination)
7. [Common Patterns](#common-patterns)
8. [Troubleshooting](#troubleshooting)

---

## Overview

B2Connect uses **GitHub Projects v2** as the primary project management tool:
- **Single Source of Truth:** All work tracked in GitHub Issues
- **Visual Workflow:** Project boards show status (Backlog → Ready → In progress → In review → Done)
- **Compliance Tracking:** Special fields for P0 components (Priority, Status, Assignee, Labels)
- **CLI Integration:** `gh` CLI enables automation and bulk operations

### Key Metrics (Current State)

| Metric | Value | Status |
|--------|-------|--------|
| Total Issues in Planner | 34 | Active |
| Issues in "Ready" | 34 | All ready to start |
| Issues in "In progress" | 0 | (team hasn't started work yet) |
| Issues in "In review" | 0 | (no PRs yet) |
| Issues "Done" | 0 | (none completed yet) |

---

## Project Structure

### Main Project: **Planner** (Project #5)

**URL:** https://github.com/users/HRasch/projects/5

**Purpose:** Main development board for B2Connect MVP + P0 compliance

**Fields:**
- **Status** (Select): Backlog / Ready / In progress / In review / Done
- **Priority** (Select): P0 (Critical) / P1 (High) / P2 (Medium)
- **Assignee** (User): Who is responsible
- **Labels** (Multi-select): feature, bug, compliance, e-commerce, ai-act, etc.

**Current Items (34 total):**
- 8 items: Authentication & User Management (P0.1 + Identity)
- 6 items: E-Commerce Legal Compliance (P0.6)
- 6 items: AI Act Compliance (P0.7)
- 4 items: Accessibility/BITV (P0.8)
- 5 items: E-Rechnung Integration (P0.9)
- 5 items: Database & Infrastructure (P0.2-P0.5)

---

## GitHub Issues Workflow

### Issue Lifecycle

```
1. CREATE (in GitHub)
   ↓
2. ADD TO PROJECT (in Planner)
   ↓
3. SET STATUS → "Ready" (if Definition of Ready met)
   ↓
4. ASSIGN → Developer (when work starts)
   ↓
5. SET STATUS → "In progress" (work begins)
   ↓
6. CREATE PR (link to issue: "Closes #123")
   ↓
7. SET STATUS → "In review" (PR under review)
   ↓
8. MERGE PR (code approved)
   ↓
9. SET STATUS → "Done" (close issue automatically)
```

### Example: Creating an E-Commerce Issue

```markdown
## Title: P0.6.1: Implement 14-Day Withdrawal Right (B2C)

## Description
B2C customers must be able to withdraw/cancel orders within 14 days of delivery 
per VVVG §357 (German Consumer Rights).

## Regulatory Reference
- **Regulation:** VVVG (Verbrauchervertragsgesetz)
- **Article:** §357 (Right of Withdrawal)
- **Requirement:** 14-day cancellation right from delivery date
- **Penalty:** Non-compliance: Bußgeld 5.000-100.000€

## Functional Requirements
- [ ] Customer can request withdrawal in account
- [ ] 14-day countdown from delivery date
- [ ] Cannot withdraw after deadline (error message)
- [ ] Return label generated (carrier integration)
- [ ] Refund processed automatically within 14 days
- [ ] Audit trail logged for each withdrawal

## Acceptance Criteria
- [ ] "Withdraw Order" button appears in customer account
- [ ] Form shows days remaining (e.g., "10 days left")
- [ ] API validates: `POST /api/orders/{id}/withdrawal` returns 400 if > 14 days
- [ ] Return label email sent within 5 minutes
- [ ] Refund shows as "pending" in customer transactions
- [ ] AuditLog entry created (action: "OrderWithdrawn")

## Definition of Ready
- [x] Legal review completed ✅
- [x] Regulatory requirement clear ✅
- [x] Effort estimated: 48 hours (2 weeks, 1 Backend Dev) ✅
- [x] No blockers ✅
- [x] Acceptance criteria defined ✅

## Estimate
- Design/Spec: 4h
- Backend Implementation: 24h
- Testing: 16h
- Documentation: 4h
- **Total: 48h**

## Dependencies
- Requires: P0.1 (Audit Logging) - DONE
- Requires: P0.2 (Encryption) - DONE
- Blocks: None

## Labels (add to issue)
- `type: feature`
- `priority: p0`
- `area: e-commerce-legal`
- `area: b2c`
- `area: compliance`

## How to Test
```bash
# 1. Create order, deliver it
curl -X POST http://localhost:8000/api/orders/checkout

# 2. Within 14 days, request withdrawal
curl -X POST http://localhost:8000/api/orders/{id}/withdrawal \
  -H "Authorization: Bearer {token}"

# 3. Verify in audit logs
curl http://localhost:8000/api/audit-logs?action=OrderWithdrawn
```

## Definition of Done
- [ ] Code merged to main
- [ ] Tests passing (>80% coverage)
- [ ] Documentation updated
- [ ] PR reviewed by Tech Lead
- [ ] Deployed to staging
- [ ] QA manual test passed
```

---

## Project Board Management

### Daily Standup (5 min check)

**Each morning, run this check:**

```bash
# Show all items NOT in Done
gh project item-list --project "PVT_kwHOAWN9Gs4BLeEd" \
  --filter "status:!Done" \
  --format json | jq '.[] | select(.status != "Done") | 
  {title: .title, status: .status, assignee: .assignee, days_since_update: .updatedAt}'
```

**Questions to ask:**

1. ❓ **Any "In progress" > 3 days without PR?**
   - **Action:** Assignee explains blockage or moves to "In review"
   
2. ❓ **Any "In review" > 1 day without merge?**
   - **Action:** Reviewer unblocks or adds reviewer
   
3. ❓ **Any "Ready" items with no assignee?**
   - **Action:** Assign to developer or move to Backlog if lower priority
   
4. ❓ **Backlog items blocking others?**
   - **Action:** Prioritize & move to Ready

### Weekly Status Report (Friday)

```bash
#!/bin/bash
# Generate weekly status report

echo "## B2Connect Planner - Weekly Status (Week of $(date +%Y-%m-%d))"
echo ""

# Count by status
echo "### Status Snapshot"
for STATUS in "Backlog" "Ready" "In progress" "In review" "Done"; do
  COUNT=$(gh project item-list --project "PVT_kwHOAWN9Gs4BLeEd" \
    --filter "status:$STATUS" --format json | jq 'length')
  echo "- **$STATUS:** $COUNT items"
done

echo ""
echo "### This Week's Completed Items"
# Items moved to Done in last 7 days
gh project item-list --project "PVT_kwHOAWN9Gs4BLeEd" \
  --filter "status:Done" --format json | \
  jq '.[] | select(.updatedAt > "'$(date -u -d "7 days ago" +%Y-%m-%dT%H:%M:%SZ)'") | 
  {title: .title, closed_by: .closedBy, closed_at: .closedAt}' | \
  jq -r '.[] | "- \(.title) (\(.closed_at))"'

echo ""
echo "### Stuck Items (In progress > 3 days)"
# Items in progress, not updated in 3 days
gh project item-list --project "PVT_kwHOAWN9Gs4BLeEd" \
  --filter "status:In\ progress" --format json | \
  jq '.[] | select(.updatedAt < "'$(date -u -d "3 days ago" +%Y-%m-%dT%H:%M:%SZ)'") | 
  {title: .title, assignee: .assignee, last_update: .updatedAt}' | \
  jq -r '.[] | "- \(.title) (assigned to \(.assignee))"'
```

### Managing Project Fields

**Status (Required for all items):**
```bash
# Move item to "In progress"
gh api graphql -f query='
mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PVT_kwHOAWN9Gs4BLeEd"
    itemId: "<ITEM_ID>"
    fieldId: "PVTSSF_lAHOAWN9Gs4BLeEdzg7CLQI"
    value: {singleSelectOptionId: "47fc9ee4"}
  }) { projectV2Item { id } }
}'
```

**Priority (Required for P0 items):**
```bash
# Set Priority: P0
gh api graphql -f query='
mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PVT_kwHOAWN9Gs4BLeEd"
    itemId: "<ITEM_ID>"
    fieldId: "PVTSSF_lAHOAWN9Gs4BLeEdzg7CLQI"
    value: {singleSelectOptionId: "critical_p0_id"}
  }) { projectV2Item { id } }
}'
```

**Assignee (Set when moving to "In progress"):**
```bash
# Assign to specific user
gh api graphql -f query='
mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PVT_kwHOAWN9Gs4BLeEd"
    itemId: "<ITEM_ID>"
    fieldId: "PVTSSF_user_field_id"
    value: {userIds: ["MDQ6VXNlcjEyMzQ1Njc4OQ=="]}
  }) { projectV2Item { id } }
}'
```

---

## Compliance Issue Tracking

### P0 Components (Regulatory Critical)

Each P0 component is tracked as a **parent issue** with **3-5 child issues:**

#### P0.1: Audit Logging (Example)

```
PARENT (Issue #P0.1)
├─ Status: [Ready/In progress/Done]
├─ Priority: P0 (Critical)
├─ Assignee: [Security Engineer]
├─ Timeline: 1 week
└─ Child Issues:
    ├─ P0.1.1: AuditLogEntry Entity (Ready)
    ├─ P0.1.2: EF Core Interceptor (Backlog, blocked by P0.1.1)
    ├─ P0.1.3: Tests & Verification (Backlog)
    └─ P0.1.4: Integration (Backlog, blocked by P0.1.3)

PARENT STATUS = Done only when ALL children = Done
```

### Compliance Testing Gate

**Before Phase 1 features deploy to production:**

```
P0.1 (Audit Logging)
├─ Issue #P0.1.1: Done ✅
├─ Issue #P0.1.2: Done ✅
├─ Issue #P0.1.3: Done ✅
└─ Issue #P0.1.4: Done ✅

P0.2 (Encryption)
├─ Issue #P0.2.1: Done ✅
├─ Issue #P0.2.2: Done ✅
└─ Issue #P0.2.3: Done ✅

... (all P0 items must be Done)

GATE RESULT: ✅ APPROVED FOR PHASE 1
or
GATE RESULT: ❌ BLOCKED - Fix open items
```

### Tracking Dependencies

**Use issue linking to show dependencies:**

```markdown
## Issue: P0.1.2 - EF Core Interceptor

### Blocked By
- #P0.1.1 (AuditLogEntry Entity) - must exist first

### Blocks
- #P0.1.3 (Testing) - can't test without implementation
- #P0.1.4 (Integration) - depends on this to integrate

### Dependency Status
```mermaid
P0.1.1 (Entity)
    ↓
P0.1.2 (Interceptor) ← YOU ARE HERE
    ↓
P0.1.3 (Testing)
    ↓
P0.1.4 (Integration)
```

---

## Team Coordination

### Role-Specific Responsibilities

| Role | Responsibility | Frequency |
|------|-----------------|-----------|
| **Tech Lead** | Triage issues, set Ready status, prioritize | Daily |
| **Backend Dev** | Work on assigned issues, move to In progress | Per assignment |
| **Frontend Dev** | Work on assigned issues, move to In progress | Per assignment |
| **Security Eng** | Track P0.x items, verify compliance | Daily |
| **DevOps** | Track infrastructure items, verify deployment | Daily |
| **PO** | Prioritize backlog, update acceptance criteria | Weekly |
| **QA** | Verify Done items, test acceptance criteria | Per PR |

### Assignment Protocol

**Only 1 assignee per issue (no shared ownership):**

```bash
# ✅ CORRECT: Single assignee
gh issue edit <ISSUE_NUMBER> --assignee "username"

# ❌ WRONG: Multiple assignees (shared ownership confusion)
# Use mentions in comments instead:
# "Reviewed by @reviewer, approved by @tech-lead"
```

### Blocker Escalation

**If an issue is blocked:**

```markdown
## Issue: P0.1.2 - EF Core Interceptor

### Status
- Currently: **IN PROGRESS** ⏳
- Blocked: **YES** 🔴
- Days Blocked: 2

### Blocker
- Waiting for: P0.5 (Key Management) to provide encryption key interface
- Reason: Can't complete interceptor without IEncryptionService contract
- Severity: **CRITICAL** (blocking 3 other items)

### Action Required
- @tech-lead please prioritize P0.5, currently in Backlog
- Once P0.5 moves to Done, this can be unblocked

### Link
- Blocked by: #P0.5
- Blocks: #P0.1.3, #P0.1.4
```

---

## Common Patterns

### Bulk Issue Creation (Efficient)

```bash
#!/bin/bash
# Create 5 related E-Rechnung issues and add to project

ISSUES=()
PROJECT_ID="PVT_kwHOAWN9Gs4BLeEd"

# Issue 1
ID=$(gh issue create \
  --title "P0.9.1: ZUGFeRD 3.0 XML Generation" \
  --body "Generate ZUGFeRD format invoices per EN 16931 standard" \
  --label "p0,e-rechnung" \
  --format json | jq -r '.number')
ISSUES+=($ID)

# Issue 2
ID=$(gh issue create \
  --title "P0.9.2: Hybrid PDF Creation" \
  --body "Embed ZUGFeRD XML in PDF for compatibility" \
  --label "p0,e-rechnung" \
  --format json | jq -r '.number')
ISSUES+=($ID)

# Issue 3
ID=$(gh issue create \
  --title "P0.9.3: UBL 2.3 Alternative Format" \
  --body "Support UBL 2.3 as alternative to ZUGFeRD" \
  --label "p0,e-rechnung" \
  --format json | jq -r '.number')
ISSUES+=($ID)

# Issue 4
ID=$(gh issue create \
  --title "P0.9.4: Invoice Archival (10-Year)" \
  --body "Implement immutable storage for German tax requirements" \
  --label "p0,e-rechnung" \
  --format json | jq -r '.number')
ISSUES+=($ID)

# Issue 5
ID=$(gh issue create \
  --title "P0.9.5: Testing & Compliance Verification" \
  --body "Verify all formats pass schema validation" \
  --label "p0,e-rechnung" \
  --format json | jq -r '.number')
ISSUES+=($ID)

# Add all to project in bulk
echo "Adding ${#ISSUES[@]} issues to project..."
for ISSUE_NUM in "${ISSUES[@]}"; do
  ISSUE_ID=$(gh issue view "$ISSUE_NUM" --json id --jq '.id')
  gh project item-add --id "$PROJECT_ID" "$ISSUE_ID"
  echo "✅ Added issue #$ISSUE_NUM"
done

echo ""
echo "✅ Created ${#ISSUES[@]} issues and added to project"
echo "📊 View at: https://github.com/users/HRasch/projects/5"
```

### Priority Labels System

```
Labels (add multiple):
├─ type:
│   ├─ feature (new capability)
│   ├─ bug (fix existing)
│   ├─ refactor (improve code)
│   └─ docs (documentation)
│
├─ priority:
│   ├─ p0 (CRITICAL - blocks deployment)
│   ├─ p1 (HIGH - important but not blocking)
│   └─ p2 (MEDIUM - nice to have)
│
├─ area:
│   ├─ e-commerce-legal
│   ├─ ai-act
│   ├─ accessibility
│   ├─ security
│   ├─ performance
│   └─ infrastructure
│
└─ status:
    ├─ blocked (waiting for something)
    ├─ duplicate (merged with another)
    └─ wontfix (intentionally not fixing)
```

### Linking PR to Issues

**When creating a PR, link to the issue:**

```bash
# Create PR that closes the issue
gh pr create \
  --title "feat: Implement 14-day withdrawal right (#123)" \
  --body "Closes #123

## Changes
- Add withdrawal form to customer account
- Validate 14-day period
- Generate return labels
- Process refunds

## Testing
- [ ] Unit tests passing
- [ ] Integration tests passing
- [ ] Manual testing: withdrawal within 14 days
- [ ] Manual testing: withdrawal after 14 days (error)

## Compliance
- [x] Audit logging integrated
- [x] Encryption for PII verified
- [x] Tenant isolation checked" \
  --head "feature/p0.6.1-withdrawal-right" \
  --base "main"
```

**Issue will auto-close when PR merges** (Closes #123)

---

## Troubleshooting

### Issue: Status doesn't update

**Problem:** Item stuck in "In progress" for days

**Solution:**
1. Check if PR is created: `gh pr list --search "Closes #123"`
2. If no PR: Contact assignee, ask for status update
3. If PR exists: Check if it's stuck in review (needs reviewer)

### Issue: Item added to project but no status

**Problem:** Item appears on board but can't see status field

**Solution:**
```bash
# Update status explicitly
gh api graphql -f query='
mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PVT_kwHOAWN9Gs4BLeEd"
    itemId: "PVTI_item_id_here"
    fieldId: "PVTSSF_lAHOAWN9Gs4BLeEdzg7CLQI"
    value: {singleSelectOptionId: "61e4505c"}
  }) { projectV2Item { id } }
}'
```

### Issue: Can't find item ID for GraphQL mutation

**Solution:**
```bash
# Get item ID from issue
ITEM_ID=$(gh project item-list --project "PVT_kwHOAWN9Gs4BLeEd" \
  --filter "issue:123" --format json | jq -r '.[0].id')

echo "Item ID: $ITEM_ID"
```

### Issue: Bulk status update failing

**Problem:** Multiple mutations in loop timeout

**Solution:** Add delay between mutations
```bash
for ITEM_ID in "${ITEMS[@]}"; do
  gh api graphql -f query="mutation { ... }"
  sleep 0.5  # 500ms delay between mutations
done
```

---

## Best Practices Summary

| Practice | Benefit |
|----------|---------|
| ✅ Create issue → Add to project → Set status | Clear workflow |
| ✅ 1 assignee per issue | Clear ownership |
| ✅ Status updates before PRs | Team visibility |
| ✅ Dependency linking | Blockers visible |
| ✅ Label consistency | Easy filtering |
| ✅ Weekly status reviews | Track velocity |
| ✅ Bulk operations with scripts | Time savings |
| ✅ Comment for context | Knowledge capture |

---

**Document Owner:** Architecture Team  
**Last Updated:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026
