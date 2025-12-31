# ✅ COMPLIANCE_INTEGRATION - Governance & Compliance in Development Workflow

**Trigger**: Any feature development, sprint planning, code review, release
**Coordinator**: @Security, @Legal (compliance verification) + @TechLead (code review gate)
**Output**: Compliance checklist, audit trail, governance decision logs
**Integration**: GitHub Projects (custom field: "Compliance Status"), GitHub CLI, CODEOWNERS enforcement

---

## Quick Start

### Compliance Checklist (Add to Every Issue)

```bash
# Create issue with compliance template
gh issue create --title "FEAT-001: Feature Name" \
  --body "
## Compliance Requirements
- [ ] WCAG 2.1 AA (if UI)
- [ ] GDPR Art. 13/14 (if personal data)
- [ ] PAngV (if prices/payments)
- [ ] Security standards (@Security review)
- [ ] Audit logging (if sensitive)

See COMPLIANCE_INTEGRATION.md for details
" \
  --label "needs-compliance-review" \
  --assign @legal-team

# Mark compliance reviewed
gh issue comment ISSUE_ID --body "✅ Compliance verified by @Security"
```

### Governance Documents (Reference)

| Document | Purpose | Owner | When to Use |
|----------|---------|-------|------------|
| [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md) | WCAG 2.1 AA, PAngV, accessibility | @UI, @Frontend | All UI/frontend work |
| [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](../../ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md) | EU e-commerce legal (PAngV, GDPR, TMG, AStV) | @Legal, @ProductOwner | Store/payment features |
| [.ai/knowledgebase/governance.md](../../.ai/knowledgebase/governance.md) | Requirements & process governance | @ProductOwner, @SARAH | Sprint planning, architecture |
| [docs/APPLICATION_SPECIFICATIONS.md](../../docs/APPLICATION_SPECIFICATIONS.md) | Complete system specs + compliance | @Architect, @Backend | Design phase, API development |

---

## Compliance Categories & Requirements

### 1. WCAG 2.1 Level AA (Web Accessibility)

**Applies To**: All UI components, frontend changes, public-facing content

**Requirements**:
- ✅ Keyboard navigation (Tab, Enter, Escape, Arrow keys)
- ✅ Focus indicators (visible on all interactive elements)
- ✅ Color contrast (4.5:1 for text, 3:1 for graphics)
- ✅ Semantic HTML (`<label>`, `<h1>-<h6>`, `<button>`, ARIA roles)
- ✅ Alt text on images
- ✅ Video captions (if applicable)
- ✅ Error identification and suggestion
- ✅ Motion/animation not required for functionality

**Reference**: [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md)

**Code Review Checklist**:
```markdown
## Accessibility Review
- [ ] Keyboard navigable (Tab through all elements)
- [ ] Focus indicators visible
- [ ] Color contrast meets 4.5:1
- [ ] Semantic HTML used correctly
- [ ] Alt text on images
- [ ] Form labels properly associated
- [ ] Error messages in `role="alert"`
- [ ] No motion/animation barriers
- [ ] ARIA attributes used correctly
```

**GitHub CLI Integration**:
```bash
# Add accessibility label to frontend PRs
gh pr create --title "feat(ui): new form component" \
  --label "frontend,wcag-2.1-review" \
  --reviewer @UI-team

# Track compliance across team
gh issue list --label "wcag-2.1-review" --state open \
  | grep -c "WCAG" || echo "All accessibility issues resolved"
```

---

### 2. GDPR Art. 13/14 (Data Protection & Privacy)

**Applies To**: Any feature handling personal data, authentication, user profiles, order history

**Requirements**:
- ✅ Privacy notice at point of collection (Art. 13)
- ✅ Privacy notice before communication (Art. 14)
- ✅ Lawful basis documented (consent, contract, legal obligation, vital interest)
- ✅ Data retention policy defined
- ✅ Right to access (data export)
- ✅ Right to erasure ("right to be forgotten")
- ✅ Right to rectification (data correction)
- ✅ Right to restrict processing
- ✅ Data processing agreement (DPA) with vendors
- ✅ Encryption at rest and in transit
- ✅ Audit logging for access/changes

**Reference**: .ai/knowledgebase/governance.md → P0 Requirements → Data Protection

**Code Review Checklist**:
```markdown
## GDPR Compliance Review
- [ ] Privacy notice added to UI (if data collection)
- [ ] Lawful basis documented (consent, contract, etc.)
- [ ] Data retention policy defined (TTL, deletion)
- [ ] Encryption implemented (at rest + in transit)
- [ ] Data access logging added (who, what, when)
- [ ] No sensitive data in logs/error messages
- [ ] Vendor DPA in place (if data shared)
- [ ] Data export functionality tested
- [ ] Deletion tested (cascading deletes, backups)
```

**GitHub CLI Integration**:
```bash
# Create GDPR compliance issue template
gh issue create --title "GDPR: Data handling for [feature]" \
  --body "
## GDPR Compliance Checklist
- [ ] Art. 13/14 privacy notice added
- [ ] Lawful basis: [consent/contract/obligation/vital]
- [ ] Retention policy: [specify TTL]
- [ ] Encryption: [at rest] [in transit]
- [ ] Data access logging: [implemented]
- [ ] Review by @Legal
" \
  --label "gdpr,legal-review"
```

---

### 3. PAngV (Price Transparency & German E-Commerce Law)

**Applies To**: Store, checkout, pricing, shipping, discounts, invoices

**Requirements**:
- ✅ All prices displayed including VAT (inkl. MwSt)
- ✅ Shipping costs visible before checkout
- ✅ Country-specific shipping calculated dynamically
- ✅ Original price shown when discounted
- ✅ Tax breakdown on invoice (net + VAT + gross)
- ✅ Estimated delivery time shown
- ✅ All mandatory fields on checkout (billing, shipping address)
- ✅ 14-day withdrawal/return rights (VVVG)
- ✅ Return form generation
- ✅ Refund processing within 14 days

**Reference**: [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](../../ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md)

**Code Review Checklist**:
```markdown
## PAngV Price Transparency Review
- [ ] Price displayed with VAT (€99,99 inkl. MwSt)
- [ ] No hidden costs on checkout
- [ ] Shipping recalculated on country change
- [ ] Original price shown strikethrough if discounted
- [ ] Tax breakdown on invoice
- [ ] Delivery time displayed (est. days)
- [ ] Currency displayed (EUR)
- [ ] Return period (14 days) enforced
- [ ] Withdrawal form generated
- [ ] Refund processed within 14 days

## Test Cases Required
- [ ] DE customer: Full price with VAT
- [ ] AT/CH customer: Shipping calculated
- [ ] Discount applied: Original price visible
- [ ] Return request: Blocked after 14 days
- [ ] Refund: Processed within 14 days
```

**GitHub CLI Integration**:
```bash
# Label price/payment features for compliance
gh pr create --title "feat(checkout): add shipping calculator" \
  --label "pangv,pricing,legal-review" \
  --reviewer @Legal,@ProductOwner

# Find all PAngV issues
gh issue list --label "pangv" --state open
```

---

### 4. Security Standards (OWASP Top 10)

**Applies To**: All code, especially API endpoints, authentication, data validation

**Requirements**:
- ✅ No hardcoded secrets (credentials in environment variables)
- ✅ SQL injection prevention (parameterized queries)
- ✅ XSS protection (output encoding, CSP headers)
- ✅ CSRF protection (anti-CSRF tokens)
- ✅ Authentication/authorization (proper session management)
- ✅ Sensitive data exposure (encryption, HTTPS)
- ✅ XML external entities (XXE) prevention
- ✅ Broken access control (authorization checks everywhere)
- ✅ Using components with known vulnerabilities (dependency scanning)
- ✅ Insufficient logging (audit trail for sensitive operations)

**Reference**: `.github/instructions/security.instructions.md`

**Code Review Checklist**:
```markdown
## Security Review
- [ ] No hardcoded secrets/credentials
- [ ] Input validation on all endpoints (sanitize, limit length)
- [ ] SQL injection prevention (parameterized queries)
- [ ] XSS protection (output encoded, CSP headers)
- [ ] CSRF token implemented (if form submission)
- [ ] Authentication/authorization enforced
- [ ] HTTPS used (no HTTP for sensitive data)
- [ ] Sensitive data not in logs
- [ ] Dependencies scanned for vulnerabilities
- [ ] Audit logging for sensitive operations

## Security Scan Results
@Security to run:
- [ ] SonarQube scan
- [ ] OWASP ZAP scan (if web app)
- [ ] Dependency audit (npm audit, dotnet audit)
- [ ] Secret scanning (no API keys, passwords)
```

**GitHub CLI Integration**:
```bash
# Create security issue for code review
gh issue create --title "security: code review [feature]" \
  --label "security,needs-review" \
  --assign @Security

# Link PR to security issue
gh pr edit PR_NUMBER --add-issue SECURITY_ISSUE_ID
```

---

### 5. Audit Logging (Sensitive Operations)

**Applies To**: Authentication, authorization changes, data modifications, payments, deletions

**Requirements**:
- ✅ Who (user ID, role)
- ✅ What (action, resource, change details)
- ✅ When (timestamp, timezone)
- ✅ Where (IP address, device)
- ✅ Why (reason/justification)
- ✅ Logs immutable (no deletion, only append)
- ✅ Retention: 7 years (for financial transactions)

**Code Review Checklist**:
```markdown
## Audit Logging Review
- [ ] User identity captured (ID, not just username)
- [ ] Action logged (what was changed)
- [ ] Timestamp with timezone
- [ ] Change details logged (before/after values)
- [ ] Sensitive data not logged (passwords, tokens)
- [ ] Audit endpoint exists (show logs to admins)
- [ ] Logs immutable (append-only)
- [ ] Retention policy enforced (7 years for payments)

## Audit Log Entry Format
\`\`\`
{
  "timestamp": "2025-12-30T14:23:45Z",
  "userId": "user-123",
  "userRole": "admin",
  "action": "order.refund",
  "resource": "order-456",
  "status": "success",
  "changes": {
    "status": "completed -> refunded",
    "amount": "€99.99"
  },
  "ipAddress": "192.168.1.1",
  "reason": "Customer withdrawal request"
}
\`\`\`
```

---

## Sprint Integration Workflow

### Sprint Planning Phase

```bash
# Step 1: Review compliance requirements for sprint goal
cat ACCESSIBILITY_COMPLIANCE_REPORT.md
cat ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md
  cat ../../.ai/knowledgebase/governance.md

# Step 2: Create sprint with compliance label
gh project create --title "Sprint 12" \
  --description "includes 2 compliance requirements"

# Step 3: Add each issue with compliance category
gh issue create --title "UI: Update form for WCAG AA" \
  --label "sprint-12,wcag-2.1-review,frontend" \
  --body "Compliance: WCAG 2.1 AA
  
  See ACCESSIBILITY_COMPLIANCE_REPORT.md for requirements
  Review by: @UI-team"

# Step 4: Set compliance custom field in GitHub Projects
# (Manual in UI: custom field "Compliance Status" = "Needs Review")
```

### Daily Standup Phase

```bash
# Include compliance status in daily standup
echo "🔒 Compliance Status:"
gh issue list --label "sprint-12" --state open \
  | grep -E "wcag|gdpr|pangv|security|audit" || echo "All compliance items on track"

# Flag compliance blockers
gh issue list --label "sprint-12,blocker,wcag-2.1-review"
```

### Code Review Gate (TechLead + Security)

```bash
# Before merge: Check compliance checklist
gh pr view PR_NUMBER --json body | grep -A 20 "Compliance"

# Require specific approvers
gh pr create --title "feature: ..." \
  --required-review-count 2 \
  --reviewers @TechLead,@Security
```

### Sprint Review Phase

```bash
# Generate compliance report
echo "## Compliance Summary"
gh issue list --label "sprint-12" --state closed \
  --json title,labels | jq '.[] | 
  select(.labels | map(.name) | 
  index("wcag-2.1-review", "gdpr", "pangv", "security"))'

# Create retrospective: What worked for compliance?
gh issue create --title "Retrospective: Sprint 12 - Compliance Process" \
  --label "retrospective,process-improvement" \
  --body "## Compliance Questions
- [ ] Were compliance requirements clear?
- [ ] Did @Security reviews slow us down?
- [ ] Should we automate compliance checks?
- [ ] Missing compliance categories?"
```

---

## Compliance Checklist Template

**Use for every issue in GitHub**:

```markdown
## Compliance Checklist

### WCAG 2.1 AA (if UI change)
- [ ] Keyboard navigable
- [ ] Focus indicators visible
- [ ] Color contrast ≥ 4.5:1
- [ ] Semantic HTML
- [ ] Alt text on images

### GDPR (if personal data)
- [ ] Privacy notice displayed
- [ ] Lawful basis: [consent/contract/obligation/vital]
- [ ] Data retention TTL: [specify]
- [ ] Encryption at rest + transit
- [ ] Access logging implemented

### PAngV (if pricing/store)
- [ ] Prices display with VAT
- [ ] Shipping calculated dynamically
- [ ] Original price shown (if discounted)
- [ ] Tax breakdown on invoice
- [ ] 14-day return enforced

### Security
- [ ] No hardcoded secrets
- [ ] Input validation on all endpoints
- [ ] SQL injection prevention
- [ ] XSS protection (CSP headers)
- [ ] Authentication enforced

### Audit Logging (if sensitive operation)
- [ ] User identity captured
- [ ] Action logged
- [ ] Timestamp with timezone
- [ ] Change details logged
- [ ] Sensitive data not logged

### Documentation
- [ ] Compliance requirements documented in PR
- [ ] Tests include compliance test cases
- [ ] Review by appropriate agent (@Security, @Legal, @UI)
```

---

## GitHub CLI Commands Reference

### Finding Compliance Issues

```bash
# All WCAG accessibility issues
gh issue list --label "wcag-2.1-review" --state open

# All GDPR data protection issues
gh issue list --label "gdpr" --state open

# All PAngV pricing/legal issues
gh issue list --label "pangv" --state open

# All security review issues
gh issue list --label "security" --state open

# Issues needing compliance review
gh issue list --label "needs-compliance-review" --state open
```

### Creating Compliance Issues

```bash
# WCAG accessibility issue
gh issue create --title "Accessibility: [Component] WCAG AA compliance" \
  --label "wcag-2.1-review,frontend" \
  --assign @UI

# GDPR data protection issue
gh issue create --title "GDPR: [Feature] data protection review" \
  --label "gdpr,legal-review" \
  --assign @Security,@Legal

# PAngV store compliance issue
gh issue create --title "Legal: [Feature] PAngV compliance check" \
  --label "pangv,legal-review" \
  --assign @Legal

# Security issue
gh issue create --title "Security: [Feature] security review required" \
  --label "security,needs-review" \
  --assign @Security
```

### Compliance in Pull Requests

```bash
# Create PR with compliance checklist
gh pr create --title "feat: [feature]" \
  --body "
## Compliance Review
- [ ] WCAG 2.1 AA (if applicable)
- [ ] GDPR compliance (if applicable)
- [ ] Security review (@Security)
- [ ] PAngV requirements (if store feature)
- [ ] Audit logging (if sensitive)

See COMPLIANCE_INTEGRATION.md
" \
  --label "needs-compliance-review"

# Add compliance review requirement
gh pr edit PR_NUMBER --required-review-count 2 \
  --reviewers @Security,@TechLead
```

---

## Governance Decision Template

**When making architectural or compliance decisions**:

```markdown
# ADR: [Number] - [Title]

**Date**: YYYY-MM-DD  
**Status**: [Proposed | Accepted | Deprecated]  
**Compliance Impact**: [WCAG | GDPR | PAngV | Security | Audit Logging]

## Context
[Why we're making this decision]

## Decision
[What we decided]

## Rationale
[Why this approach meets compliance requirements]

## Compliance Checklist
- [ ] WCAG 2.1 AA compliant
- [ ] GDPR requirements met
- [ ] PAngV (if applicable) requirements met
- [ ] Security standards met
- [ ] Audit logging (if applicable) included

## Consequences
[What happens as a result]

## References
- [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md)
- [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](../../ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md)
- [.ai/knowledgebase/governance.md](../../.ai/knowledgebase/governance.md)
```

---

## Integration with Existing Workflows

### Sprint Planning
✅ Include compliance review as part of Definition of Done  
✅ Add compliance estimation (extra hours for security/legal review)  
✅ Assign @Security, @Legal, @UI as reviewers for compliance PRs

### Code Review Gate
✅ @TechLead checks code quality + compliance  
✅ @Security reviews for security vulnerabilities  
✅ @Legal reviews for GDPR/PAngV compliance  
✅ @UI reviews for WCAG 2.1 AA compliance

### GitHub Actions / CI/CD
✅ Automated lint & test on every PR  
✅ Security scanning (OWASP ZAP, SonarQube)  
✅ Dependency audit (npm audit, dotnet audit)  
✅ WCAG automated testing (axe-core, Pa11y)

### Release Checklist
```bash
# Before releasing to production
gh issue list --label "sprint-12,compliance" --state open
# Should be ZERO open compliance issues
```

---

## Key Documents

| File | Purpose | Owner |
|------|---------|-------|
| [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md) | WCAG 2.1 AA, PAngV price standards | @UI, @Frontend |
| [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](../../ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md) | EU e-commerce legal requirements (PAngV, GDPR, VVVG) | @Legal, @ProductOwner |
| [.ai/knowledgebase/governance.md](../../.ai/knowledgebase/governance.md) | Requirements governance, P0 critical items | @ProductOwner, @SARAH |
| [docs/APPLICATION_SPECIFICATIONS.md](../../docs/APPLICATION_SPECIFICATIONS.md) | Complete system specs + compliance requirements | @Architect |
| [.github/instructions/security.instructions.md](.github/instructions/security.instructions.md) | Security coding standards | @Security |

---

**Compliance Review Required**: @Security, @Legal, @TechLead  
**When Unsure**: Escalate to @SARAH for governance guidance
