---
docid: CMP-006
title: COMPLIANCE_INTEGRATION_MAP
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 📊 Compliance Integration Overview

**Complete mapping of compliance into development workflow**

---

## Document Flow

```
.ai/knowledgebase/governance.md (Requirements)
    ↓
ACCESSIBILITY_COMPLIANCE_REPORT.md (WCAG standards)
ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md (EU legal)
docs/APPLICATION_SPECIFICATIONS.md (Complete specs)
    ↓
    ├─→ .github/prompts/compliance-integration.prompt.md
    │   ├─→ Implementation guidance
    │   ├─→ GitHub CLI commands
    │   └─→ Checklists & templates
    │
    ├─→ .ai/compliance/CMP-001-COMPLIANCE_QUICK_REFERENCE.md
    │   ├─→ One-page developer guide
    │   ├─→ Checklists for each category
    │   └─→ Common mistakes
    │
    ├─→ .github/prompts/sprint-cycle.prompt.md
    │   ├─→ Compliance in sprint planning
    │   ├─→ Compliance verification gate at closure
    │   └─→ GitHub CLI integration
    │
    └─→ .github/prompts/git-management.prompt.md
        ├─→ Compliance in code review
        ├─→ Reviewer assignments
        └─→ Branch protection rules
    
    ↓
.ai/compliance/COMPLIANCE_ADOPTION.md
└─→ Team adoption guide
```

---

## Compliance Categories Matrix

### WCAG 2.1 Level AA
| Aspect | Requirement | Code | Test | When |
|--------|-------------|------|------|------|
| Keyboard | Tab navigable | `@Component` | E2E | UI changes |
| Focus | Visible indicators | CSS `:focus` | Visual | UI changes |
| Contrast | ≥ 4.5:1 | Color values | Tool | All UI |
| HTML | Semantic tags | `<label>`, `<h1>` | Lint | All UI |
| Images | Alt text | `:alt="text"` | Review | All images |
| **Label** | `wcag-2.1-review` | - | @UI review | Every PR |

### GDPR Art. 13/14
| Aspect | Requirement | Code | Test | When |
|--------|-------------|------|------|------|
| Privacy | Notice shown | UI component | Manual | Data collection |
| Consent | Explicit opt-in | Checkbox/button | E2E | Data collection |
| Encryption | At rest + transit | `bcrypt`, TLS | Security scan | All data |
| Logging | User access tracked | Audit log | Integration | Data access |
| Rights | Export/delete | API endpoints | E2E | All features |
| **Label** | `gdpr` | - | @Security/@Legal | Data features |

### PAngV (Store)
| Aspect | Requirement | Code | Test | When |
|--------|-------------|------|------|------|
| Price | Include VAT | Template string | Unit | All pricing |
| Shipping | Pre-checkout | Form UI | E2E | Checkout |
| Tax | Breakdown | Invoice template | Unit | Invoices |
| Returns | 14-day period | Date calc | Unit | Orders |
| **Label** | `pangv` | - | @Legal review | Store features |

### Security
| Aspect | Requirement | Code | Test | When |
|--------|-------------|------|------|------|
| Secrets | Env vars | `process.env` | Scan | All code |
| Input | Validated | Validator fn | Unit | All endpoints |
| SQL | Parameterized | ORM/prepared | SAST | Databases |
| XSS | Output encoded | Template safe | DAST | All UI |
| Auth | Enforced | Middleware | E2E | Protected routes |
| **Label** | `security` | - | @Security | All code |

### Audit Logging
| Aspect | Requirement | Code | Test | When |
|--------|-------------|------|------|------|
| WHO | User captured | `userId, role` | Unit | Sensitive ops |
| WHAT | Action logged | `action: "..."` | Unit | Sensitive ops |
| WHEN | Timestamp | `ISO 8601` | Unit | Sensitive ops |
| HOW | Changes tracked | `before/after` | Unit | Modifications |
| CLEAN | No secrets | Filter pwd/token | Review | All logs |
| **Label** | `audit-logging` | - | @Security | Sensitive ops |

---

## Developer Workflow

```mermaid
graph TD
    A[New Issue/Feature] -->|Check Type| B{What Does It Involve?}
    B -->|UI Changes| C["🎨 WCAG 2.1<br/>Label: wcag-2.1-review<br/>Reviewer: @UI"]
    B -->|Personal Data| D["🔒 GDPR<br/>Label: gdpr<br/>Reviewer: @Security"]
    B -->|Pricing/Store| E["💰 PAngV<br/>Label: pangv<br/>Reviewer: @Legal"]
    B -->|Sensitive Operation| F["🔐 Security<br/>Label: security<br/>Reviewer: @Security"]
    B -->|Auth/Payment/Delete| G["📋 Audit Logging<br/>Label: audit-logging<br/>Reviewer: @Security"]
    
    C -->|Code| H["✅ WCAG Checklist<br/>- Keyboard nav<br/>- Focus indicators<br/>- 4.5:1 contrast<br/>- Semantic HTML<br/>- Alt text"]
    D -->|Code| I["✅ GDPR Checklist<br/>- Privacy notice<br/>- Encryption<br/>- Access logging<br/>- Data retention<br/>- User rights"]
    E -->|Code| J["✅ PAngV Checklist<br/>- VAT included<br/>- Shipping shown<br/>- Tax breakdown<br/>- 14-day return<br/>- Invoice proper"]
    F -->|Code| K["✅ Security Checklist<br/>- No secrets<br/>- Input validation<br/>- SQL injection fix<br/>- XSS protection<br/>- Auth enforced"]
    G -->|Code| L["✅ Audit Logging<br/>- WHO captured<br/>- WHAT logged<br/>- WHEN stamped<br/>- No secrets<br/>- 7yr retention"]
    
    H -->|PR| M["Code Review Gate"]
    I -->|PR| M
    J -->|PR| M
    K -->|PR| M
    L -->|PR| M
    
    M -->|@TechLead| N["Quality Gate<br/>- Code style<br/>- Architecture<br/>- Compliance ✓"]
    
    N -->|Approved| O["✅ Merge & Deploy"]
    N -->|Issues| P["❌ Request Changes"]
    P -->|Fix| H
```

---

## GitHub CLI Integration

### Create Issue with Compliance
```bash
# WCAG issue
gh issue create --title "UI: Accessibility audit" \
  --label "wcag-2.1-review,frontend" \
  --assign @UI

# GDPR issue
gh issue create --title "GDPR: Data protection review" \
  --label "gdpr,legal-review" \
  --assign @Security,@Legal

# PAngV issue
gh issue create --title "Legal: Store compliance check" \
  --label "pangv,legal-review" \
  --assign @Legal

# Security issue
gh issue create --title "Security: Code review required" \
  --label "security,needs-review" \
  --assign @Security
```

### Track Compliance Status
```bash
# All open compliance issues
gh issue list --label "wcag,gdpr,pangv,security,audit" --state open

# Sprint compliance status
gh issue list --label "sprint-12" --state open \
  | grep -E "wcag|gdpr|pangv|security|audit"

# Compliance verification (all closed = ready to release)
gh issue list --label "wcag-2.1-review,gdpr,security" --state open
# Result: ZERO = ✅ Safe to release
```

### Code Review with Compliance
```bash
# PR with compliance reviewers
gh pr create --title "feat: new feature" \
  --required-review-count 2 \
  --reviewers @Security,@TechLead \
  --label "needs-compliance-review"

# Check PR compliance status
gh pr view PR_ID --json body | grep -A 20 "Compliance"
```

---

## Team Structure

```
┌─────────────────────────────────────────────────────┐
│          Compliance Review Team                     │
├─────────────────────────────────────────────────────┤
│ @UI           → WCAG 2.1 AA (Accessibility)        │
│ @Security     → GDPR, Security, Audit Logging      │
│ @Legal        → GDPR, PAngV, Legal Requirements    │
│ @TechLead     → Quality Gate (Overall Compliance)  │
│ @ProductOwner → PAngV, Business Requirements       │
│ @SARAH        → Governance & Escalation            │
└─────────────────────────────────────────────────────┘

            ↓ Compliance Questions?

┌─────────────────────────────────────────────────────┐
│          Escalation Path                            │
├─────────────────────────────────────────────────────┤
│ Developer               (Asks)                      │
│    ↓                                                │
│ Compliance Reviewer     (@UI/@Security/@Legal)    │
│    ↓ (if blocked)                                   │
│ Tech Lead              (@TechLead)                 │
│    ↓ (if blocked)                                   │
│ Governance Coordinator (@SARAH)                    │
└─────────────────────────────────────────────────────┘
```

---

## Sprint Integration

### Sprint Planning Phase
```
1. Review governance documents
   ├─ .ai/knowledgebase/governance.md
   ├─ ACCESSIBILITY_COMPLIANCE_REPORT.md
   ├─ ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md
   └─ docs/APPLICATION_SPECIFICATIONS.md

2. Check sprint compliance requirements
   ├─ WCAG 2.1 AA? (Frontend changes)
   ├─ GDPR? (Personal data)
   ├─ PAngV? (Store features)
   ├─ Security? (All code)
   └─ Audit Logging? (Sensitive ops)

3. Create issues with compliance labels
   └─ Add to GitHub Projects

4. Assign compliance reviewers
   ├─ @UI (WCAG)
   ├─ @Security (GDPR, Security, Audit)
   ├─ @Legal (PAngV, Legal)
   └─ @TechLead (Overall gate)

5. Estimate compliance review hours
   └─ Add to sprint capacity
```

### Daily Standup
```
Team Status: ✓
Completed PRs: ✓
Compliance Items:
├─ WCAG: [X/Y] closed
├─ GDPR: [X/Y] closed
├─ PAngV: [X/Y] closed
├─ Security: [X/Y] closed
└─ Audit: [X/Y] closed

Blockers? If compliance review stuck
└─ Escalate to reviewer or @SARAH
```

### Sprint Review & Closure
```
COMPLIANCE VERIFICATION GATE (MUST PASS)
├─ gh issue list --label "sprint-12" --label "wcag" --state open
│  Result must be: ZERO ✓
├─ gh issue list --label "sprint-12" --label "gdpr" --state open
│  Result must be: ZERO ✓
├─ gh issue list --label "sprint-12" --label "security" --state open
│  Result must be: ZERO ✓
└─ All compliance issues closed? 
   YES → ✅ Safe to release
   NO  → ❌ Block release, fix issues
```

---

## Documentation Layers

```
Layer 1: Governance (What we must comply with)
├─ .ai/knowledgebase/governance.md
├─ ACCESSIBILITY_COMPLIANCE_REPORT.md
├─ ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md
└─ docs/APPLICATION_SPECIFICATIONS.md

Layer 2: Implementation (How we comply)
├─ .github/prompts/compliance-integration.prompt.md
├─ .github/prompts/git-management.prompt.md
└─ .github/prompts/sprint-cycle.prompt.md

Layer 3: Quick Reference (What developers need daily)
├─ .ai/compliance/CMP-001-COMPLIANCE_QUICK_REFERENCE.md
└─ .ai/compliance/COMPLIANCE_ADOPTION.md
```

---

## Success Metrics

| Metric | Target | How to Measure |
|--------|--------|-----------------|
| Compliance Issues Resolved | 100% per sprint | `gh issue list --label "compliance" --state closed` |
| Code Review Cycle Time | < 48 hours | GitHub PR review metrics |
| Compliance Violations | 0 critical | Security scan results |
| Team Training | 100% read QUICK_REFERENCE | Tracking |
| Automated Checks | 80%+ | CI/CD pass rate |

---

## File Locations Reference

```
Root
├── ACCESSIBILITY_COMPLIANCE_REPORT.md          [Referenced]
├── ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md    [Referenced]
├── .ai/knowledgebase/governance.md                               [Referenced]
│
├── docs/
│   └── APPLICATION_SPECIFICATIONS.md           [Referenced]
│
├── .github/
│   ├── prompts/
│   │   ├── compliance-integration.prompt.md    [Comprehensive guide]
│   │   ├── git-management.prompt.md            [UPDATED]
│   │   ├── sprint-cycle.prompt.md              [UPDATED]
│   │   └── ...other prompts
│   │
│   ├── instructions/
│   │   ├── security.instructions.md            [Referenced]
│   │   └── ...other instructions
│   │
│   └── agents/
│       └── ...agent definitions
│
└── .ai/
    ├── compliance/
    │   ├── CMP-001-COMPLIANCE_QUICK_REFERENCE.md       [Developer quick ref]
    │   ├── COMPLIANCE_ADOPTION.md              [Adoption guide]
    │   └── COMPLIANCE_ADOPTION_SUMMARY.md      [Summary]
    │
    └── workflows/
        ├── WF-004-GITHUB_CLI_QUICK_REFERENCE.md       [CLI quick ref]
        ├── WF-006-GITHUB_CLI_SPRINT_HOWTO.md          [CLI how-to]
        ├── WF-005-GITHUB_CLI_IMPLEMENTATION.md        [CLI impl guide]
        └── ...other workflows
```

---

**Integration Complete**: ✅ Full compliance workflow established  
**Team Ready**: ✅ All documentation available  
**Status**: 🟢 Ready for sprint execution

