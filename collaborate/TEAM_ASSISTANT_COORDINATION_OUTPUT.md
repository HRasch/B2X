# Team Assistant Collaboration Output (GitHub Sync)

**How @team-assistant consolidates mailbox responses and updates GitHub issues**

---

## Overview

@team-assistant monitors agent mailboxes, collects responses, and **consolidates findings back to GitHub issues** so the full team stays informed.

```
Agent Mailboxes (collaborate/issue/{ID}/)
    ↓
    Requests & Responses accumulate
    ↓
@team-assistant daily review
    ↓
Consolidate findings
    ↓
Post to GitHub issue (comment)
    ↓
GitHub issue updated with all agent input
```

---

## Daily Workflow (10 minutes)

### Step 1: Review All Agent Folders

```bash
cd collaborate/issue/{ISSUE_ID}/

# Check each agent folder for new responses
ls @agent-1/
ls @agent-2/
ls @qa-engineer/
# etc.
```

### Step 2: Create Consolidation Summary

**File**: `collaborate/issue/{ISSUE_ID}/COORDINATION_SUMMARY.md`

**Template**:
```markdown
# Issue #{ID} Coordination Summary

**Last Updated**: 2025-12-31 15:30 UTC  
**Status**: In Progress / Blocked / Complete

## Pending Requests

| Agent | Request Type | From | Due | Status |
|-------|-------------|------|-----|--------|
| @backend-developer | API Specification | @product-owner | 2025-12-31 | ✅ Responded |
| @frontend-developer | Design Review | @ui-expert | 2026-01-02 | ⏳ Pending |
| @qa-engineer | Test Plan | @product-owner | 2025-12-31 | 🔴 Not Started |

## Agent Responses (Ready to Post to GitHub)

### @backend-developer - API Specification ✅

**Request**: Product search endpoint design  
**Response Posted**: 2025-12-31 10:00  
**Status**: Complete

**Key Findings**:
- Endpoint: `GET /api/products/search?category={}&price={}`
- Supports: Pagination, filtering, sorting
- Error codes: 400, 422, 500
- Response time target: < 200ms

**File**: `collaborate/issue/56/@backend-developer/backend-developer-response-2025-12-31-api-spec.md`

---

### @frontend-developer - Design Review ⏳

**Request**: Component library review  
**Status**: Pending (due 2026-01-02)  
**File**: `collaborate/issue/56/@frontend-developer/2025-12-31-from-ui-expert-design-review.md`

---

## GitHub Issue Update (Post This)

Copy this consolidation to GitHub issue comment:

```markdown
## 🔄 Collaboration Status Update

**As of**: 2025-12-31 15:30

### ✅ Completed Responses

**@backend-developer** - API Specification design:
- [✓] Product search endpoint designed
- [✓] Request/response schemas documented
- [✓] Error handling defined
- [✓] Performance targets set

See details: `collaborate/issue/56/@backend-developer/backend-developer-response-2025-12-31-api-spec.md`

---

### ⏳ Pending Responses

**@frontend-developer** - Design Review (due 2026-01-02)  
**@qa-engineer** - Test Plan (due 2025-12-31) 🔴 OVERDUE

See all: `collaborate/issue/56/COORDINATION_SUMMARY.md`

---

### 🔧 Next Steps

1. @frontend-developer: Begin implementation based on API spec
2. @qa-engineer: Start test plan (URGENT - overdue)
3. @product-owner: Review API response in collaboration folder

---

Last sync: 2025-12-31 15:30
```

---

## Weekly Consolidation (30 minutes)

At end of sprint or milestone, create **Sprint Consolidation Report**:

**File**: `collaborate/lessons-learned/consolidated-issue-{ID}.md`

```markdown
# Issue #56 Completion Consolidation

**Issue**: Store Frontend Modernization  
**Completed**: 2025-12-31  
**Sprint**: Sprint 3 Phase B

## All Responses Collected

### Responses by Agent

| Agent | Response Type | Status | Reference |
|-------|---|---|---|
| @backend-developer | API Specification | ✅ Complete | api-spec.md |
| @frontend-developer | Design Review | ✅ Complete | design-review.md |
| @qa-engineer | Test Plan | ✅ Complete | test-plan.md |
| @ui-expert | Component Library | ✅ Complete | components.md |
| @ux-expert | UX Validation | ✅ Complete | ux-validation.md |

## Key Findings Summary

**What @backend-developer provided**:
- API specification with error handling
- Performance targets (< 200ms)
- Schema validation rules

**What @frontend-developer provided**:
- Component design patterns
- Responsive breakpoints
- Accessibility notes

**What @qa-engineer provided**:
- Test strategy
- Coverage targets (80%+)
- Edge case scenarios

## Next Phase

All coordination complete. Ready for development.

See full responses in: `collaborate/issue/56/`

---

Date: 2025-12-31  
Compiled by: @team-assistant
```

---

## Integration with GitHub

### In GitHub Issue Comment

Post summary with link to collaboration folder:

```markdown
## 📋 Coordination Tracker

All agent responses collected in: `collaborate/issue/56/`

**View specific responses**:
- API Design: `collaborate/issue/56/@backend-developer/`
- UX Review: `collaborate/issue/56/@frontend-developer/`
- Test Strategy: `collaborate/issue/56/@qa-engineer/`

See consolidation: `collaborate/issue/56/COORDINATION_SUMMARY.md`

Last updated: 2025-12-31 15:30
```

### GitHub Issue Updates Schedule

- **Daily** (end of day): Post COORDINATION_SUMMARY.md findings to issue
- **Weekly** (Friday): Create consolidated report, archive old responses
- **At Completion**: Move all collaboration docs to `collaborate/lessons-learned/`

---

## @team-assistant Responsibilities

### Daily (5 min check)

```
1. Check all @agent folders in active issues
2. Count pending vs completed responses
3. Update COORDINATION_SUMMARY.md
4. Post GitHub comment if significant change
5. Flag overdue requests (> 48h) for escalation
```

### Weekly (Friday, 30 min)

```
1. Consolidate all week's responses
2. Create consolidated report in lessons-learned/
3. Archive completed issue folder
4. Update GitHub issue with "Coordination Complete"
5. Plan next sprint coordination
```

### Monthly (End of sprint, 60 min)

```
1. Review all consolidated reports
2. Identify coordination patterns
3. Update team communication standards if needed
4. Report metrics:
   - Avg response time (target: < 24h)
   - % of requests answered (target: 100%)
   - Escalations needed (target: 0)
5. Document lessons learned
```

---

## Example: Complete Workflow

### Day 1 - Request Created
```
@product-owner creates request:
  collaborate/issue/56/@backend-developer/
  2025-12-30-from-product-owner-api-spec.md

@team-assistant updates:
  COORDINATION_SUMMARY.md
  - Status: 🔴 Pending
  - Due: 2025-12-31 EOD

Posts to GitHub:
  "API specification request created. See: collaborate/issue/56/"
```

### Day 2 - Response Posted
```
@backend-developer creates response:
  collaborate/issue/56/@backend-developer/
  backend-developer-response-2025-12-31-api-spec.md

Deletes original request (marks processed)

@team-assistant updates:
  COORDINATION_SUMMARY.md
  - Status: ✅ Complete
  - Response summary added

Posts to GitHub:
  "✅ API specification complete. See: collaborate/issue/56/@backend-developer/"
```

### Day 5 - Sprint End
```
@team-assistant consolidates:
  Creates: collaborate/lessons-learned/consolidated-issue-56.md
  Archives: collaborate/issue/56/
  Summary: All responses collected, ready for next phase

Posts to GitHub:
  "📦 Coordination complete. All 5 agents responded.
   Consolidation: collaborate/lessons-learned/consolidated-issue-56.md"
```

---

## Output Templates

### Daily Update (Post to GitHub)

```markdown
## 🔄 Collaboration Status (Updated Today)

**Responses Received**: 
- ✅ @backend-developer - API specification
- ⏳ @frontend-developer - Design review (due tomorrow)

**Pending**: 
- 🔴 @qa-engineer - Test plan (OVERDUE)

See all details: `collaborate/issue/{ID}/COORDINATION_SUMMARY.md`
```

### Weekly Summary (Post to GitHub)

```markdown
## 📊 Weekly Coordination Summary

**This Week**:
- 5 requests created
- 4 responses completed (80%)
- 1 pending (due next week)
- 0 escalations

**Response Times**:
- Avg: 14 hours
- Fastest: 4 hours (@backend-dev)
- Slowest: 24 hours (@qa-engineer)

**Next Week**:
- @frontend-dev design review due
- @qa-engineer test plan overdue (escalate?)

See details: `collaborate/lessons-learned/consolidated-week-{N}.md`
```

### Completion Consolidation (Post to GitHub)

```markdown
## ✅ Issue Coordination Complete

**All Responses**:
- ✅ @backend-developer - API spec
- ✅ @frontend-developer - Design review
- ✅ @qa-engineer - Test plan
- ✅ @ui-expert - Component library
- ✅ @ux-expert - UX validation

**Consolidation Report**:
`collaborate/lessons-learned/consolidated-issue-56.md`

Ready to proceed with development.
```

---

## Automation Ideas (Future)

These could be automated by @process-assistant:

1. **Daily Digest**: Scan mailboxes, auto-post summary to GitHub
2. **Escalation Alert**: Flag requests unanswered > 48h
3. **Consolidation Report**: Auto-generate weekly summaries
4. **Metrics Dashboard**: Track response times, completion rates
5. **Archive Script**: Auto-move completed issues to lessons-learned/

---

## Key Rules

✅ **DO**:
- Post consolidation to GitHub daily (end of day)
- Link to specific collaboration files in comments
- Flag overdue requests immediately (> 48h)
- Archive completed issues weekly
- Update COORDINATION_SUMMARY.md before posting to GitHub

❌ **DON'T**:
- Copy-paste entire response files (link instead)
- Post to GitHub without first updating COORDINATION_SUMMARY.md
- Leave overdue requests without escalation
- Archive issues before consolidating
- Forget to mention collaborate/ folder path in GitHub comments

---

## Integration Points

```
GitHub Issue
    ↑ (posts updates from)
    │
@team-assistant
    │ (reads from)
    ↓
collaborate/issue/{ID}/
├── COORDINATION_SUMMARY.md ← Main consolidation hub
├── @agent-1/requests & responses
├── @agent-2/requests & responses
└── @agent-3/requests & responses
    │
    └─→ github/{issue-id}/comments
        (consolidated findings posted here)
```

---

## Related Documentation

- **Quick Start**: [AGENT_MAILBOX_QUICK_START.md](./AGENT_MAILBOX_QUICK_START.md)
- **System Architecture**: [AGENT_MAILBOX_SYSTEM_ARCHITECTURE.md](./AGENT_MAILBOX_SYSTEM_ARCHITECTURE.md)
- **Plain Communication Rule**: [PLAIN_COMMUNICATION_RULE.md](./PLAIN_COMMUNICATION_RULE.md)
- **Team Assistant Role**: [.github/agents/team-assistant.agent.md](../../.github/agents/team-assistant.agent.md)

---

**Last Updated**: 30. Dezember 2025  
**Owner**: @team-assistant  
**Frequency**: Daily updates, weekly consolidation
