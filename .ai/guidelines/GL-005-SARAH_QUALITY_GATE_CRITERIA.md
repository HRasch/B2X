---
docid: GL-065
title: GL 005 SARAH_QUALITY_GATE_CRITERIA
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-005
title: SARAH Quality Gate - Single-Topic PR Review Criteria
owner: "@SARAH"
status: Active
date: 2025-01-02
---

# SARAH Quality Gate - Single-Topic PR Review Criteria

**DocID**: `GL-005`  
**Purpose**: Define SARAH's quality gate criteria for single-topic PR enforcement  
**Owner**: @SARAH  
**Applies To**: All PRs before merge

---

## Overview

As coordinator and quality gatekeeper, SARAH reviews PRs to ensure they follow the single-topic principle. This document defines the criteria SARAH uses when evaluating PR topic coherence.

> **Pre-Release Note (GL-014)**: During v0.x development, breaking changes are permitted without deprecation cycles. Focus quality gates on code quality and single-topic enforcement, not backwards compatibility.

---

## Review Triggers

SARAH reviews PRs when:

1. **Automated warning triggered** - GitHub Action detects potential mixed topics
2. **Developer requests exception** - PR includes mixed-topic justification
3. **High-risk changes** - Security, architecture, breaking changes
4. **Random sampling** - 10% of all PRs for quality assurance
5. **Team escalation** - Code reviewer requests SARAH review

---

## Review Criteria

### ✅ PASS - Single Topic Approved

PR is approved if it meets ANY of these patterns:

#### Pattern 1: Feature + Tests + Docs
```
✅ Backend feature implementation
✅ Unit tests for the feature
✅ Integration tests for the feature
✅ API documentation
✅ User-facing documentation
```
**Rationale**: Tests and docs are part of feature delivery

#### Pattern 2: Bug Fix + Regression Test
```
✅ Bug fix implementation
✅ Test that reproduces the bug
✅ Test that verifies the fix
✅ Related code cleanup
```
**Rationale**: Regression test prevents future bug recurrence

#### Pattern 3: Backend + Frontend for Single Feature
```
✅ Backend API endpoint
✅ Frontend component consuming the API
✅ Integration test for the flow
✅ Tests for both layers
```
**Rationale**: Both layers implement same feature

#### Pattern 4: Refactor + Updated Tests
```
✅ Code refactoring (no behavior change)
✅ Tests updated to reflect refactored structure
✅ Documentation updated
```
**Rationale**: Tests must match refactored code

#### Pattern 5: Database Schema + Migration
```
✅ Schema changes
✅ Migration script (up)
✅ Migration script (down/rollback)
✅ Updated models/entities
✅ Tests for new schema
```
**Rationale**: Cohesive database change

---

### ⚠️ CONDITIONAL PASS - Requires Justification

These patterns CAN be approved if properly justified:

#### Pattern 6: Multiple Services in Same Domain
```
⚠️ Catalog/ProductService changes
⚠️ Catalog/InventoryService changes
⚠️ Both related to same feature
```
**Required Justification**:
- Why both services must change together
- Dependency between the services
- Why it can't be split into 2 PRs

**SARAH Evaluates**:
- Are services tightly coupled for this feature?
- Could this be sequenced (refactor first, feature second)?
- Is there a legitimate technical reason?

#### Pattern 7: Enabling Refactor + Feature
```
⚠️ Refactor to extract reusable utility
⚠️ New feature uses the utility
```
**Required Justification**:
- Why refactor is needed for feature
- Why refactor can't be separate PR
- Risk of merging together

**SARAH Evaluates**:
- Could refactor be done first (PR #1)?
- Is refactor truly a prerequisite?
- Can feature proceed without refactor?

#### Pattern 8: Emergency Hotfix with Multiple Fixes
```
⚠️ Critical production bug in Catalog
⚠️ Critical production bug in Identity
⚠️ Both causing production outage
```
**Required Justification**:
- Production incident severity
- Why fixes must be deployed together
- Rollback plan

**SARAH Evaluates**:
- Is this truly emergency?
- Can fixes be deployed separately?
- Is rollback safe?

---

### ❌ REJECT - Split Required

These patterns are **ALWAYS rejected**:

#### Anti-Pattern 1: Multiple Features
```
❌ Feature A: Add product search
❌ Feature B: Add user profile
```
**SARAH Decision**: Split into 2 PRs
**Reason**: Completely independent features

#### Anti-Pattern 2: Multiple Bug Fixes
```
❌ Fix: Catalog price calculation bug
❌ Fix: Identity login timeout bug
❌ Fix: CMS page rendering bug
```
**SARAH Decision**: 3 separate PRs
**Reason**: Independent bugs, independent fixes

#### Anti-Pattern 3: Feature + Unrelated Refactor
```
❌ Feature: Add product filtering
❌ Refactor: Rewrite authentication service
```
**SARAH Decision**: Split into 2 PRs
**Reason**: No dependency between changes

#### Anti-Pattern 4: Multiple Domain Changes
```
❌ Catalog: New feature
❌ Identity: Security fix
❌ CMS: Performance improvement
```
**SARAH Decision**: 3 separate PRs
**Reason**: Different domains, different topics

#### Anti-Pattern 5: Feature + Infrastructure Change
```
❌ Feature: Add search functionality
❌ Infrastructure: Upgrade Kubernetes config
```
**SARAH Decision**: Split (unless infrastructure is required for feature)
**Reason**: Independent concerns

---

## Review Process

### Step 1: Automated Pre-Check

GitHub Action runs:
- ✅ Branch name validation
- ✅ File path analysis
- ✅ Domain boundary detection
- ⚠️ Posts warning if mixed topics suspected

### Step 2: Developer Declaration

Developer fills PR template:
- ✅ Selects primary domain
- ✅ Selects change type
- ✅ Declares single/multiple topic
- ✅ Provides justification if multiple

### Step 3: SARAH Review (if needed)

SARAH reviews:
1. **Read PR description** - Understand the change
2. **Check changed files** - Analyze file paths and domains
3. **Review commits** - Check commit message coherence
4. **Evaluate justification** - If multiple topics, is it valid?
5. **Apply criteria** - Match to approved patterns
6. **Make decision** - Approve, Conditional, or Reject

### Step 4: SARAH Decision

SARAH posts comment with:
```markdown
## 🤖 SARAH Quality Gate Review

**PR**: #123 - Add product search
**Topic Coherence**: ✅ APPROVED

**Analysis**:
- Primary Domain: Catalog ✅
- Change Type: Feature ✅
- Changed Files: Backend API, Frontend UI, Tests ✅
- Pattern Match: Feature + Tests + Docs ✅

**Decision**: APPROVED - Single cohesive topic
All changes implement product search feature.

**Next Steps**: Proceed with code review
```

OR

```markdown
## 🤖 SARAH Quality Gate Review

**PR**: #124 - Multiple fixes
**Topic Coherence**: ❌ REJECTED

**Analysis**:
- Changed Domains: Catalog, Identity, CMS ❌
- Change Types: 3 independent bug fixes ❌
- Pattern Match: Anti-pattern #2 (Multiple Bug Fixes)

**Decision**: REJECTED - Mixed topics detected

**Required Action**: Split into 3 separate PRs:
1. PR #1: Fix Catalog price calculation
2. PR #2: Fix Identity login timeout
3. PR #3: Fix CMS page rendering

See: [GL-004](.ai/guidelines/GL-004-BRANCH_NAMING_STRATEGY.md)
```

---

## Exception Process

### When SARAH Might Grant Exception

1. **True Emergency**
   - Production outage impacting users
   - Multiple fixes required for single incident
   - Time-critical deployment

2. **Tightly Coupled Technical Constraint**
   - Changes are technically inseparable
   - Splitting would break functionality
   - Migration work spanning boundaries

3. **Dependency Chain**
   - Change B requires Change A
   - Both are part of single feature delivery
   - Sequence is clear and documented

### How to Request Exception

In PR description, add:

```markdown
## 🚨 SARAH Exception Request

**Reason**: [Detailed explanation]
**Why can't split**: [Technical/business justification]
**Alternative considered**: [Why alternatives won't work]
**Risk assessment**: [Risks of approving mixed-topic PR]
**Rollback plan**: [How to rollback if issues]
**Reviewers**: @SARAH @Architect @TechLead
```

SARAH evaluates with @Architect and @TechLead input.

### Exception Approval Criteria

Exception is granted ONLY if:
- ✅ Clear technical justification provided
- ✅ Splitting would create more risk
- ✅ All changes are essential for feature delivery
- ✅ Rollback plan is documented
- ✅ @Architect and @TechLead both approve
- ✅ Exception is documented in PR

---

## Metrics & Monitoring

### SARAH Tracks

| Metric | Target | Purpose |
|--------|--------|---------|
| % PRs passing topic check | >90% | Measure adherence |
| Exception requests/month | <5% | Monitor overuse |
| Rejected PRs | Track trend | Identify patterns |
| Time to split PRs | <4 hours | Measure friction |

### Monthly Review

SARAH publishes:
- Topic coherence statistics
- Common violation patterns
- Exception analysis
- Recommendations for improvement

**Report Location**: `.ai/logs/quality-gate/monthly-topic-review-YYYY-MM.md`

---

## Team Training

### For Developers

**Before Creating PR**:
1. ✅ Use proper branch naming convention
2. ✅ Focus on ONE topic per branch
3. ✅ Include tests and docs with feature
4. ✅ Fill topic declaration in PR template

**If Unsure**:
- Ask: "Can I describe this PR in one sentence?"
- Ask: "If this PR breaks, what ONE thing broke?"
- Ask: "Do all commits relate to same goal?"

### For Code Reviewers

**Topic Checklist**:
- [ ] PR has clear, single topic
- [ ] All changes support the stated topic
- [ ] No "while I'm here" changes included
- [ ] Tests and docs included (expected)

**If Mixed Topics Detected**:
1. Request split into separate PRs
2. Tag @SARAH for quality gate review
3. Explain which parts should be separate

---

## Integration with Existing Processes

### Aligns With
- **[ADR-020]** PR Quality Gate
- **[GL-004]** Branch Naming Strategy
- **[KB-014]** Git Commit Strategy
- **[WF-005]** GitHub CLI Implementation

### Enhances
- Code review efficiency (clear scope)
- Rollback safety (atomic changes)
- Git history clarity (one topic per merge)
- Feature traceability (clear mapping)

---

## FAQ

**Q: What if my feature requires changes in multiple domains?**
**A**: That's fine IF it's truly one feature. Example: "Add product search" touches Catalog (API), Search (Elasticsearch), and Store (UI) - all part of one feature. Document the relationship in PR description.

**Q: Can I include small code cleanup with my feature?**
**A**: Only if the cleanup is directly related to your feature. Unrelated cleanup should be separate PR.

**Q: I found a bug while working on feature. Include the fix?**
**A**: If the bug blocks your feature, yes. Otherwise, create separate PR for the bug fix.

**Q: What about dependency updates?**
**A**: Group related dependency updates (e.g., "Update Vue ecosystem to v5") but separate unrelated updates (Vue update vs .NET update).

**Q: Emergency production fix, can I skip SARAH review?**
**A**: No. Emergency PRs still need SARAH approval but with expedited review (30 min SLA).

---

## Related Documentation

- **[GL-004]** Branch Naming & Single-Topic Strategy
- **[ADR-020]** PR Quality Gate
- **[KB-014]** Git Commit Strategy
- **AGENT_COORDINATION.md** - SARAH responsibilities

---

## Changelog

| Date | Change | Author |
|------|--------|--------|
| 2025-01-02 | Initial version | @SARAH |

---

**Status**: ✅ Active  
**Next Review**: 2025-02-01  
**Feedback**: Tag @SARAH in PR comments or open issue with `quality-gate` label
