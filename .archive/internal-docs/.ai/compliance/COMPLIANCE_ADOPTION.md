# 📋 Compliance Documentation Adoption Guide

**Status**: ✅ COMPLETE  
**Date**: 30. Dezember 2025  
**Integration**: All compliance documents now integrated into development workflow

---

## What's Been Integrated

### 1. Compliance Documents Adopted

✅ **[ACCESSIBILITY_COMPLIANCE_REPORT.md](ACCESSIBILITY_COMPLIANCE_REPORT.md)**
- WCAG 2.1 Level AA compliance standards
- Color contrast requirements, keyboard navigation, semantic HTML
- Used in: UI/frontend code reviews, sprint planning

✅ **[ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md)**
- EU e-commerce legal requirements (PAngV, VVVG, GDPR, TMG)
- Price transparency, 14-day returns, VAT handling
- Used in: Store/payment features, sprint planning, acceptance criteria

✅ **[.ai/knowledgebase/governance.md](.ai/knowledgebase/governance.md)**
- Requirements governance, P0 critical items
- Complete requirements map and process documentation
- Used in: Sprint planning, architecture decisions, quality gates

✅ **[docs/APPLICATION_SPECIFICATIONS.md](docs/APPLICATION_SPECIFICATIONS.md)**
- Complete system specifications + compliance requirements
- Functional, security, API, database, audit & compliance sections
- Used in: Design phase, API development, infrastructure

### 2. New Prompts Created

✅ **[.github/prompts/compliance-integration.prompt.md](.github/prompts/compliance-integration.prompt.md)**
- Comprehensive guide to integrating compliance into workflows
- Covers: WCAG 2.1, GDPR, PAngV, Security, Audit Logging
- GitHub CLI commands for compliance management
- **Use for**: Sprint planning, code review gates, compliance verification

✅ **[.github/prompts/git-management.prompt.md](.github/prompts/git-management.prompt.md)** (UPDATED)
- Added compliance gate in code review checklist
- Requires @Security/@Legal/@UI review for compliance issues
- References compliance-integration.prompt.md

✅ **[.github/prompts/sprint-cycle.prompt.md](.github/prompts/sprint-cycle.prompt.md)** (UPDATED)
- Integrated compliance requirements into sprint planning
- Added compliance verification gate for sprint closure
- Compliance custom field in GitHub Projects
- References all compliance documents

### 3. Workflow Integration Points

#### Sprint Planning Phase
**File**: [.github/prompts/sprint-cycle.prompt.md](.github/prompts/sprint-cycle.prompt.md)

Before planning, review:
- ACCESSIBILITY_COMPLIANCE_REPORT.md (WCAG requirements)
- ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md (EU legal requirements)
- .ai/knowledgebase/governance.md (requirements governance)
- compliance-integration.prompt.md (implementation guidance)

**Check Compliance Categories**:
- [ ] WCAG 2.1 AA (accessibility)
- [ ] GDPR (data protection)
- [ ] PAngV (store/pricing)
- [ ] Security standards
- [ ] Audit logging

#### Daily Standup Checks
```bash
# Include compliance status in daily standup
gh issue list --label "sprint-12" --state open \
  | grep -E "wcag|gdpr|pangv|security|audit" || echo "All compliance items on track"
```

#### Code Review Gate
**File**: [.github/prompts/git-management.prompt.md](.github/prompts/git-management.prompt.md)

Compliance checklist required before merge:
- [ ] WCAG 2.1 AA (if UI change)
- [ ] GDPR data protection (if personal data)
- [ ] PAngV price transparency (if pricing/store)
- [ ] Security standards (all code)
- [ ] Audit logging (sensitive operations)

**Assign Compliance Reviewers**:
- @UI - WCAG 2.1 AA compliance
- @Security - GDPR, Security, Audit logging
- @Legal - PAngV, legal requirements
- @TechLead - Overall quality gate

#### Sprint Review & Closure
**File**: [.github/prompts/sprint-cycle.prompt.md](.github/prompts/sprint-cycle.prompt.md)

**Compliance Verification Gate**:
```bash
# CRITICAL: All compliance issues must be closed before sprint closure
gh issue list --label "sprint-12" --label "wcag-2.1-review,gdpr,security,pangv" --state open
# Result must be: ZERO
```

---

## Key Compliance Categories

### 1. WCAG 2.1 Level AA (Web Accessibility)

**Applies To**: All UI components, frontend changes, public-facing content

**Requirements**:
✅ Keyboard navigation (Tab, Enter, Escape, Arrow keys)  
✅ Focus indicators (visible on all interactive elements)  
✅ Color contrast (4.5:1 for text, 3:1 for graphics)  
✅ Semantic HTML (`<label>`, `<h1>-<h6>`, `<button>`, ARIA)  
✅ Alt text on images  
✅ Video captions (if applicable)  
✅ Error identification and suggestion

**Label**: `wcag-2.1-review`  
**Reviewers**: @UI  
**Reference**: [ACCESSIBILITY_COMPLIANCE_REPORT.md](ACCESSIBILITY_COMPLIANCE_REPORT.md)

---

### 2. GDPR Art. 13/14 (Data Protection & Privacy)

**Applies To**: Any feature handling personal data, authentication, user profiles, order history

**Requirements**:
✅ Privacy notice at point of collection  
✅ Lawful basis documented  
✅ Data retention policy defined  
✅ Right to access (data export)  
✅ Right to erasure ("right to be forgotten")  
✅ Encryption at rest and in transit  
✅ Audit logging for access/changes  
✅ No sensitive data in logs

**Label**: `gdpr`  
**Reviewers**: @Security, @Legal  
**Reference**: [.ai/knowledgebase/governance.md](.ai/knowledgebase/governance.md)

---

### 3. PAngV (Price Transparency & German E-Commerce Law)

**Applies To**: Store, checkout, pricing, shipping, discounts, invoices

**Requirements**:
✅ All prices displayed including VAT (€99,99 inkl. MwSt)  
✅ Shipping costs visible before checkout  
✅ Country-specific shipping calculated dynamically  
✅ Original price shown when discounted  
✅ Tax breakdown on invoice  
✅ 14-day withdrawal/return rights  
✅ Return form generation  
✅ Refund processing within 14 days

**Label**: `pangv`  
**Reviewers**: @Legal, @ProductOwner  
**Reference**: [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md)

---

### 4. Security Standards (OWASP Top 10)

**Applies To**: All code, especially API endpoints, authentication, data validation

**Requirements**:
✅ No hardcoded secrets (environment variables only)  
✅ SQL injection prevention (parameterized queries)  
✅ XSS protection (output encoding, CSP headers)  
✅ CSRF protection (anti-CSRF tokens)  
✅ Authentication/authorization enforcement  
✅ Sensitive data encryption (at rest + in transit)  
✅ Audit logging for sensitive operations  
✅ Input validation on all endpoints

**Label**: `security`  
**Reviewers**: @Security  
**Reference**: [.github/instructions/security.instructions.md](.github/instructions/security.instructions.md)

---

### 5. Audit Logging (Sensitive Operations)

**Applies To**: Authentication changes, authorization changes, data modifications, payments, deletions

**Requirements**:
✅ User identity captured (ID, not just username)  
✅ Action logged (what was changed)  
✅ Timestamp with timezone  
✅ Change details logged (before/after values)  
✅ Sensitive data NOT logged  
✅ Audit logs immutable (append-only)  
✅ Retention: 7 years (for financial transactions)

**Label**: `audit-logging`  
**Reviewers**: @Security  
**Audit Log Format**:
```json
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
```

---

## Using Compliance in GitHub CLI

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

### Finding Compliance Issues

```bash
# All WCAG issues
gh issue list --label "wcag-2.1-review" --state open

# All GDPR issues
gh issue list --label "gdpr" --state open

# All PAngV issues
gh issue list --label "pangv" --state open

# All security issues
gh issue list --label "security" --state open

# Issues needing compliance review
gh issue list --label "needs-compliance-review" --state open
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
  --label "needs-compliance-review" \
  --required-review-count 2 \
  --reviewers @Security,@TechLead
```

---

## Compliance Checklist Template

Use for every issue in GitHub:

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
- [ ] Review by appropriate agent
```

---

## Integration Checklist

✅ ACCESSIBILITY_COMPLIANCE_REPORT.md adopted (WCAG 2.1)  
✅ ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md adopted (EU e-commerce)  
✅ .ai/knowledgebase/governance.md adopted (requirements governance)  
✅ docs/APPLICATION_SPECIFICATIONS.md referenced  

✅ compliance-integration.prompt.md created  
✅ git-management.prompt.md updated with compliance gate  
✅ sprint-cycle.prompt.md updated with compliance verification  

✅ GitHub CLI commands documented for compliance management  
✅ Label conventions established (wcag, gdpr, pangv, security, audit)  
✅ Reviewer assignments specified (@UI, @Security, @Legal, @TechLead)

---

## Next Steps

### Immediate (This Sprint)
1. **Team Training**: Review COMPLIANCE_INTEGRATION.md
2. **Label Setup**: Create GitHub labels for compliance categories
3. **Reviewer Assignment**: Add @UI, @Security, @Legal, @TechLead to CODEOWNERS
4. **Custom Fields**: Add "Compliance Status" to GitHub Projects

### Short Term (Next Sprint)
1. **Helper Scripts**: Deploy compliance check scripts to `scripts/` directory
2. **CI/CD Integration**: Add automated compliance checks to GitHub Actions
   - WCAG testing (axe-core, Pa11y)
   - Security scanning (SonarQube, OWASP ZAP)
   - Dependency audit (npm audit, dotnet audit)
3. **Documentation**: Link compliance docs in README and QUICK_START_GUIDE

### Medium Term (Ongoing)
1. **Compliance Metrics**: Track compliance issues resolved per sprint
2. **Retrospectives**: Assess compliance process effectiveness
3. **Automation**: Automate repetitive compliance checks
4. **Training**: Quarterly compliance training for team

---

## Key Documents Map

| Document | Purpose | When to Use |
|----------|---------|------------|
| [ACCESSIBILITY_COMPLIANCE_REPORT.md](ACCESSIBILITY_COMPLIANCE_REPORT.md) | WCAG 2.1 AA standards & examples | All UI/frontend work |
| [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md) | EU e-commerce legal requirements | Store/payment features, sprint planning |
| [.ai/knowledgebase/governance.md](.ai/knowledgebase/governance.md) | Requirements & process governance | Sprint planning, architecture decisions |
| [docs/APPLICATION_SPECIFICATIONS.md](docs/APPLICATION_SPECIFICATIONS.md) | Complete system specs + compliance | Design phase, API development |
| [.github/prompts/compliance-integration.prompt.md](.github/prompts/compliance-integration.prompt.md) | Compliance implementation guide | Code review, sprint planning |
| [.github/prompts/git-management.prompt.md](.github/prompts/git-management.prompt.md) | Git workflow with compliance gate | Every PR review |
| [.github/prompts/sprint-cycle.prompt.md](.github/prompts/sprint-cycle.prompt.md) | Sprint workflow with compliance verification | Every sprint |
| [.github/instructions/security.instructions.md](.github/instructions/security.instructions.md) | Security coding standards | All code |

---

## Support & Questions

**For Compliance Questions**:
- WCAG/Accessibility: @UI
- GDPR/Data Protection: @Security, @Legal
- PAngV/Store Legal: @Legal, @ProductOwner
- Security Standards: @Security
- Audit Logging: @Security

**For Process Questions**:
- Sprint Planning: @ScrumMaster
- Architecture Decisions: @Architect
- Code Review: @TechLead
- Governance: @SARAH

---

**Compliance Documentation Adoption**: ✅ COMPLETE  
**Date**: 30. Dezember 2025  
**Ready for Use**: Team can reference these documents and follow compliance workflows immediately
