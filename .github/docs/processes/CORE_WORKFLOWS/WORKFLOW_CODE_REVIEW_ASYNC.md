# WORKFLOW_CODE_REVIEW - Velocity-Based (No SLAs)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN ASYNC MODEL  
**Status**: ACTIVE - Replaces time-based model

---

## 🎯 Overview

Code review is continuous and async. No SLA timeboxes. Reviews happen when reviewer has capacity. The story doesn't count toward velocity until all reviews approved and code is merged.

---

## 📋 Participants

- **Developer**: Creates PR, addresses feedback
- **Peer Reviewer**: Quality & style check (Priority 1: review ASAP)
- **@tech-lead**: Architecture & pattern check (Normal priority)
- **@security-engineer**: Security check (Priority 1 if security-sensitive)
- **@qa-engineer**: Test coverage & quality gates (Normal priority)

---

## 🔄 Review Phases (No Timeboxes)

### **Phase 1: PR Creation**

**Developer** creates PR with:
- ✅ Code compiles (build succeeds)
- ✅ Tests pass locally (100%)
- ✅ Coverage ≥80%
- ✅ Commit message references issue #N
- ✅ Description explains changes

**Example PR Description**:
```
## What
Add product filtering by category in Store API

## Why
#123 - Users need to filter products to find what they want

## How
- New filter parameter in ProductQuery
- Elasticsearch integration for performance
- Unit tests for all scenarios

## Acceptance
- [x] Tests passing (100%)
- [x] Coverage ≥80%
- [x] Code review ready
```

**Trigger**: Developer marks PR as ready for review

---

### **Phase 2: Peer Review (Priority 1)**

**Reviewer**: Any available peer developer  
**Focus**: Code quality, style, clarity, best practices  
**Timing**: Review ASAP (not a formal SLA, just priority)

**Checklist**:
- [ ] Code is readable and understandable
- [ ] Variable/function names are clear
- [ ] No obvious bugs or logic errors
- [ ] Follows project coding standards
- [ ] No code duplication
- [ ] Comments are helpful (not redundant)

**Comment Types**:
- 🔴 **MUST FIX**: Blocking issue (approval blocked until fixed)
- 🟡 **SHOULD FIX**: Strong suggestion (non-blocking)
- 🟢 **NICE TO HAVE**: Optional improvement (suggestion only)

**Exit Criteria**: Peer approves (no MUST FIX items)

---

### **Phase 3: Architecture Review**

**Reviewer**: @tech-lead  
**Focus**: Architectural patterns, design decisions, Wolverine compliance  
**Timing**: When @tech-lead has capacity (normal priority)

**Checklist**:
- [ ] Follows Onion Architecture (Core → App → Infra → Presentation)
- [ ] Wolverine service pattern (NOT MediatR)
- [ ] Domain entities have no framework dependencies
- [ ] Repositories abstracted (interface in Core, impl in Infra)
- [ ] No circular dependencies
- [ ] Error handling appropriate
- [ ] Async/await used consistently
- [ ] CancellationToken passed through

**Comment Types**:
- 🔴 **MUST FIX**: Architectural violation (blocking)
- 🟡 **SHOULD FIX**: Better pattern available (non-blocking)
- 🟢 **NICE TO HAVE**: Alternative consideration

**Exit Criteria**: @tech-lead approves (no MUST FIX items)

---

### **Phase 4: Security Review**

**Reviewer**: @security-engineer (if security-sensitive) OR developer self-review  
**Focus**: OWASP Top 10, encryption, multi-tenancy, audit logging  
**Timing**: Priority 1 for security-sensitive code, normal otherwise

**Checklist**:
- [ ] No hardcoded secrets
- [ ] PII encrypted (email, phone, name, address, DOB)
- [ ] All queries filter by TenantId
- [ ] SQL injection prevented (parameterized queries)
- [ ] XSS prevented (output escaping)
- [ ] CSRF tokens used (if applicable)
- [ ] Audit logging in place (data modification)
- [ ] Input validation (FluentValidation)
- [ ] Error messages don't leak info
- [ ] No SQL/code injection vectors

**Comment Types**:
- 🔴 **MUST FIX**: Security violation (BLOCKING - no exceptions)
- 🟡 **SHOULD FIX**: Security improvement (non-blocking)
- 🟢 **NICE TO HAVE**: Defense-in-depth suggestion

**Exit Criteria**: @security-engineer approves (no MUST FIX items) OR self-review complete if no sensitive code

---

### **Phase 5: QA Review**

**Reviewer**: @qa-engineer  
**Focus**: Test coverage, test quality, test scenarios  
**Timing**: Normal priority

**Checklist**:
- [ ] Coverage ≥80%
- [ ] Unit tests present for business logic
- [ ] Integration tests present for boundaries
- [ ] Tests verify happy path + error cases
- [ ] Tests cover edge cases
- [ ] Tests don't depend on each other
- [ ] Mock external dependencies properly
- [ ] Test names are descriptive
- [ ] Cross-tenant access blocked (if applicable)
- [ ] Encryption round-trip tested (if applicable)

**Comment Types**:
- 🔴 **MUST FIX**: Missing critical test (blocking)
- 🟡 **SHOULD FIX**: Better test coverage (non-blocking)
- 🟢 **NICE TO HAVE**: Additional edge case test

**Exit Criteria**: @qa-engineer approves (no MUST FIX items) OR coverage ≥80% verified

---

### **Phase 6: Address Feedback**

**Developer** addresses all feedback:
- [ ] Fix MUST FIX items
- [ ] Consider SHOULD FIX items
- [ ] Document decisions for NICE TO HAVE items
- [ ] Re-request review if code changed
- [ ] Mark conversations as resolved (GitHub)

**Timing**: Work on feedback when developer has capacity

---

### **Phase 7: Re-Review & Merge**

**Reviewers** re-verify fixes (if code changed significantly)

**Merge Criteria** (all must be true):
- ✅ All 4 reviews approved (Peer + Architecture + Security + QA)
- ✅ All tests passing
- ✅ Coverage ≥80%
- ✅ No unresolved comments
- ✅ Build succeeds

**Developer**: Merge PR, close issue, story points counted

---

## 📊 No SLA Times (Agent-Driven)

### **What Changed**

❌ **Old Model (SLAs)**:
- Peer review: 4 hours
- Architecture: 8 hours
- Security: 8 hours
- QA: 8 hours
- Re-review: 4 hours

✅ **New Model (Async, Capacity-Driven)**:
- Peer review: ASAP (Priority 1 - reviewers have code review time daily)
- Architecture: When @tech-lead available (normal priority)
- Security: ASAP if security-sensitive, otherwise normal
- QA: When @qa-engineer available (normal priority)
- Re-review: When developer submits fix

### **Key Principle**

**We don't wait for reviews in time slots. Reviewers block time for code review daily. Story counts when DONE (all reviews + merged), not when started.**

---

## 🚀 Escalation

### **Blocking on Review**

If PR waiting for review for 2+ days:
- Post in Slack #code-review channel
- Tag specific reviewer
- Explain blocker impact (blocks other story, blocks deployment, etc.)
- Any available peer can review OR ask @scrum-master for help

### **Review Dispute**

If developer disagrees with reviewer:
- Discuss in PR comments with full context
- If can't agree: Escalate to next level (peer → @tech-lead, @tech-lead → @software-architect)
- Final decision authority: @tech-lead (architecture), @security-engineer (security)

### **MUST FIX Not Getting Fixed**

If developer doesn't address MUST FIX comment:
- Reviewer marks review "Request Changes" (blocks merge)
- Re-request conversation in Slack
- If unresponsive: Escalate to @scrum-master for priority alignment

---

## 📈 Metrics (Per Story Point)

| Metric | How Measured |
|--------|------|
| **Reviews Completed** | Approved PRs |
| **Feedback Addressed** | % of MUST FIX items fixed |
| **Re-review Needed** | % requiring additional pass |
| **Merge-Ready** | All 4 approvals received |
| **Quality Maintained** | Coverage ≥80%, tests passing |

---

## 🔗 Related Workflows

- [WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md](./WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md) - How stories flow
- [GOVERNANCE_RULES.md](../GOVERNANCE/GOVERNANCE_RULES.md) - Authority structure

---

**Owner**: @process-assistant  
**Version**: 2.0 (No SLAs)  
**Status**: ACTIVE  
**Key Change**: No timeboxes. Async reviews. Story points counted at merge.

