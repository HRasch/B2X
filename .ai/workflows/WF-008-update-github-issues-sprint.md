---
docid: WF-019
title: WF 008 Update Github Issues Sprint
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 📋 Instructions: Update GitHub Issue State After Iteration Planning

**Purpose:** Synchronize GitHub issue states with iteration planning decisions  
**Trigger:** After iteration planning is complete and all issues are assessed  
**Owner:** @ScrumMaster  
**Frequency:** End of each iteration planning session

---

## 🎯 Overview

After iteration planning is complete, GitHub issue states must be updated to reflect:
- Issues ready to start (velocity-based execution)
- Issues in specification/planning phase
- Issues deferred to future iterations
- New labels and assignments

This keeps GitHub as the source of truth for issue status. **Note:** No specific dates are assigned - work completes when velocity target is met or all items are done.

---

## 📊 Issue State Mapping

### Iteration 001 Planning Results

```
Current State → New State | GitHub Label | GitHub Assignment
────────────────────────────────────────────────────────────

Issue #57: Dependencies (8 SP)
  Backlog → In Progress | iteration/001 | @Backend
  Old Labels: chore, dependencies, technical-debt
  New Labels: iteration/001, ready-to-start, phase-2
  Milestone: Iteration 001
  Priority: P0

Issue #56: UI Modernization (13 SP)
  Backlog → In Progress | iteration/001 | @Frontend
  Old Labels: enhancement, ui-ux, frontend
  New Labels: iteration/001, ready-to-start, phase-2
  Milestone: Iteration 001
  Priority: P0
  Note: Add condition label for daily oversight

Issue #15: Compliance Planning (21 SP)
  Backlog → Deferred | iteration/002 | @ProductOwner
  Old Labels: feature, legal, compliance, p0.6
  New Labels: iteration/002, awaiting-legal, p0-critical
  Milestone: Iteration 002
  Priority: P0
  Assignees: @ProductOwner (Lead), @Legal (Review)
  Reason: Awaiting legal sign-off

Issue #48: Testing (13 SP)
  Backlog → Deferred | iteration/002 | @QA
  Old Labels: enhancement, testing, quality
  New Labels: iteration/002, deferred-approved
  Milestone: Iteration 002
  Priority: P0
  Note: Approved strategic deferral
```

---

## 🔄 Step-by-Step: Update Issue States

### Step 1: Prepare Labels (One-time setup if needed)

Create/verify these labels in GitHub:

```
iteration/001       - Color: Blue (#0075ca)
iteration/002       - Color: Blue (#0075ca)
ready-to-start      - Color: Green (#28a745)
specification-phase - Color: Yellow (#ffc300)
in-progress         - Color: Purple (#6f42c1)
phase-1             - Color: Blue (#0075ca)
phase-2             - Color: Blue (#0075ca)
deferred-approved   - Color: Gray (#cccccc)
p0-critical         - Color: Red (#d73a49)
has-conditions      - Color: Orange (#fb8500)
awaiting-legal      - Color: Red (#d73a49)
```

### Step 2: Update Issue #57 (Dependencies)

**Command:**
```bash
gh issue edit 57 \
  --milestone "Iteration 001" \
  --add-label iteration/001,ready-to-start,phase-2 \
  --remove-label backlog \
  --add-assignee Backend
```

**Manual Steps (GitHub UI):**
1. Go to Issue #57
2. Set Milestone: Iteration 001
3. Add Labels: `iteration/001`, `ready-to-start`, `phase-2`
4. Remove Label: `backlog` (if present)
5. Set Assignee: @Backend
6. Add Comment:
   ```
   ## Iteration 001 Status Update
   
   **Status:** Ready to Start (Phase 2)
   **Owner:** @Backend
   **Priority:** P0
   **Velocity Points:** 8 SP
   **Risk:** 🟢 LOW
   
   This issue is scheduled to begin in Phase 2 of Iteration 001. Work continues until velocity target (28 SP) is met. See ITERATION_001_PLAN.md for details.
   ```

### Step 3: Update Issue #56 (UI Modernization)

**Command:**
```bash
gh issue edit 56 \
  --milestone "Iteration 001" \
  --add-label iteration/001,ready-to-start,phase-2,has-conditions \
  --remove-label backlog \
  --add-assignee Frontend
```

**Manual Steps (GitHub UI):**
1. Go to Issue #56
2. Set Milestone: Iteration 001
3. Add Labels: `iteration/001`, `ready-to-start`, `phase-2`, `has-conditions`
4. Remove Label: `backlog` (if present)
5. Set Assignee: @Frontend
6. Add Comment:
   ```
   ## Iteration 001 Status Update
   
   **Status:** Ready to Start (Phase 2)
   **Owner:** @Frontend
   **Priority:** P0
   **Velocity Points:** 13 SP
   **Risk:** 🟡 MEDIUM (complexity mitigation)
   
   **Conditions:**
   - Daily @TechLead oversight required
   - Component scope lock (no mid-iteration additions)
   - Accessibility (WCAG 2.1 AA) mandatory
   
   Iteration completes when velocity target (28 SP) is met or all committed items done. See ITERATION_001_PLAN.md for details.
   ```

### Step 4: Update Issue #15 (Compliance Planning)

**Command:**
```bash
gh issue edit 15 \
  --milestone "Iteration 002" \
  --add-label iteration/002,specification-phase,p0-critical,awaiting-legal \
  --add-assignee ProductOwner,Legal
```

**Manual Steps (GitHub UI):**
1. Go to Issue #15
2. Set Milestone: Iteration 002
3. Add Labels: `iteration/002`, `specification-phase`, `p0-critical`, `awaiting-legal`
4. Set Assignees: @ProductOwner (Primary), @Legal (Reviewer)
5. Add Comment:
   ```
   ## Iteration 001 Status Update
   
   **Status:** DEFERRED TO ITERATION 002 (Awaiting Legal Sign-Off)
   **Owner:** @ProductOwner (Lead), @Legal (Review)
   **Priority:** P0 (Critical - Regulatory)
   **Velocity Points:** 21 SP
   **Risk:** 🟡 MEDIUM (regulatory complexity)
   
   **Reason for Deferral:**
   - ⏳ @Legal formal sign-off required before implementation
   - Proper legal review needed upfront
   - Blocks all compliance sub-issues (#20-#28)
   
   **Sub-Issues:** #20, #21, #22, #23, #24, #25, #26, #27, #28
   
   **Acceptance Criteria (When Legal Approves):**
   - EU E-Commerce legal requirements mapped
   - P0.6 compliance framework defined
   - Implementation plan approved by @Legal
   
   Iteration continues without this item. Will be included in Iteration 002 planning once legal approval received.
   
   See ITERATION_001_PLAN.md for details.
   ```

### Step 5: Update Issue #48 (Testing)

**Command:**
```bash
gh issue edit 48 \
  --milestone "Iteration 002" \
  --add-label iteration/002,deferred-approved \
  --remove-label backlog \
  --add-assignee QA
```

**Manual Steps (GitHub UI):**
1. Go to Issue #48
2. Set Milestone: Iteration 002
3. Add Labels: `iteration/002`, `deferred-approved`
4. Remove Label: `backlog` (if present)
5. Set Assignee: @QA
6. Add Comment:
   ```
   ## Iteration 001 Status Update
   
   **Status:** APPROVED DEFERRED TO ITERATION 002
   **Owner:** @QA
   **Priority:** P0 (Quality)
   **Velocity Points:** 13 SP
   
   **Rationale for Deferral:**
   - Better sequencing: Test Iteration 001 code output
   - Avoid quality bottleneck in Iteration 001
   - Build testing infrastructure in parallel (Phase 2)
   - Use real code as test cases (better ROI)
   
   **Parallel Work in Iteration 001:**
   - Testing infrastructure setup
   - Test strategy finalization
   - Playwright setup & configuration
   
   Execution: Iteration 002 (after Iteration 001 completes)
   
   See ITERATION_001_PLAN.md for details.
   ```

---

## 🔗 Update Issue #15 Sub-Issues

For each compliance sub-issue (#20-#28), add labels:

```bash
# For each issue (20, 21, 22, ... 28)
gh issue edit {ISSUE_NUM} \
  --add-label p0.6-compliance,iteration-002-planned,blocked-by-#15 \
  --add-assignee Backend  # or @Frontend for #26, @Legal for #25
```

**Manual Steps for each:**
1. Add Label: `p0.6-compliance` (indicates part of P0.6 suite)
2. Add Label: `iteration-002-planned` (target iteration)
3. Add Label: `blocked-by-#15` (blocked until #15 specification complete)
4. Verify Assignee (Backend, Frontend, or Legal)
5. Add Comment: "Part of P0.6 Legal Compliance Suite. Blocked by #15 specification."

---

## ✅ Verification Checklist

After updating all issues, verify:

### GitHub Issue Board
- [ ] Issue #57 shows "sprint/001", "ready-to-start", "week-1" labels
- [ ] Issue #56 shows "sprint/001", "ready-to-start", "week-2", "has-conditions" labels
- [ ] Issue #15 shows "sprint/001", "specification-phase", "awaiting-legal" labels
- [ ] Issue #48 shows "sprint/002", "deferred-approved" labels
- [ ] All sub-issues (#20-#28) show "p0.6-compliance" and "blocked-by-#15" labels

### Milestone Assignment
- [ ] Sprint 001: Contains issues #57, #56, #15 (4 main + sub-issues not yet)
- [ ] Sprint 002: Contains issue #48
- [ ] Phase 1: Contains backlog issues with p0.6-compliance label

### Assignees
- [ ] Issue #57: @Backend
- [ ] Issue #56: @Frontend
- [ ] Issue #15: @ProductOwner, @Legal
- [ ] Issue #48: @QA
- [ ] Sub-issues: Appropriate owners assigned

### Comments
- [ ] All 4 main issues have status update comments
- [ ] Comments include sprint target, risk level, and documentation links
- [ ] Conditions noted for Issue #56
- [ ] Blockers noted for Issue #15

---

## 🔄 Automation Option (GitHub CLI Script)

**File:** `scripts/update-sprint-001-issues.sh`

```bash
#!/bin/bash

# Script: Update GitHub issues for Sprint 001 planning completion
# Run after sprint planning is finalized

set -e

echo "Updating Sprint 001 GitHub Issues..."

# Issue #57: Dependencies
echo "Updating Issue #57..."
gh issue edit 57 \
  --milestone "Sprint 001" \
  --add-label sprint/001,ready-to-start,week-1 \
  --remove-label backlog || true

# Issue #56: UI Modernization
echo "Updating Issue #56..."
gh issue edit 56 \
  --milestone "Sprint 001" \
  --add-label sprint/001,ready-to-start,week-2,has-conditions \
  --remove-label backlog || true

# Issue #15: Compliance
echo "Updating Issue #15..."
gh issue edit 15 \
  --milestone "Sprint 001" \
  --add-label sprint/001,specification-phase,p0-critical,awaiting-legal || true

# Issue #48: Testing (deferred)
echo "Updating Issue #48..."
gh issue edit 48 \
  --milestone "Sprint 002" \
  --add-label sprint/002,deferred-approved \
  --remove-label backlog || true

# Update sub-issues #20-28
echo "Updating compliance sub-issues..."
for i in {20..28}; do
  gh issue edit "$i" \
    --add-label p0.6-compliance,blocked-by-#15 || true
done

echo "✅ Sprint 001 GitHub issues updated!"
echo ""
echo "Next steps:"
echo "1. Review issue comments in GitHub"
echo "2. Verify milestone assignments"
echo "3. Check label assignments"
echo "4. Confirm all assignees are correct"
```

**Usage:**
```bash
chmod +x scripts/update-sprint-001-issues.sh
./scripts/update-sprint-001-issues.sh
```

---

## 📝 Status Comment Template

Use this template for issue comments:

```markdown
## Sprint Planning Status Update

**Planning Date:** [DATE]
**Status:** [READY TO START | IN PROGRESS | DEFERRED | BLOCKED]

**Sprint Assignment:** Sprint 001
**Owner:** @[USERNAME]
**Priority:** [P0 | P1 | P2]

**Timeline:**
- Start: [DATE]
- Target Completion: [DATE]
- Risk Level: [🟢 LOW | 🟡 MEDIUM | 🔴 HIGH]

**Scope:** [Brief description of what will be done]

**Dependencies:** [List issues this depends on]
**Blockers:** [List current blockers if any]

**Related Documents:**
- Plan: [.ai/sprint/SPRINT_001_PLAN.md](/.ai/sprint/SPRINT_001_PLAN.md)
- Review: [SPRINT_001_TEAM_REVIEW.md](/.ai/sprint/SPRINT_001_TEAM_REVIEW.md)

[Additional details as needed]
```

---

## 🔍 Post-Update Verification

After running the script or manually updating issues:

### Check GitHub Project Board
```bash
# View all Sprint 001 issues
gh issue list --milestone "Sprint 001" --json number,title,labels

# View all Sprint 002 issues
gh issue list --milestone "Sprint 002" --json number,title,labels
```

### Expected Output for Sprint 001
```
Number  Title                                      Labels
57      chore(dependencies): Update to latest      sprint/001, ready-to-start, week-1
56      feat(store-frontend): Modernize UI/UX      sprint/001, ready-to-start, week-2, has-conditions
15      P0.6: Store Legal Compliance               sprint/001, specification-phase, p0-critical, awaiting-legal
```

### Expected Output for Sprint 002
```
Number  Title                                      Labels
48      Sprint 3.2: Testing & Refinement           sprint/002, deferred-approved
```

---

## 📌 Important Notes

### When to Update
- ✅ After sprint planning is finalized
- ✅ After all team reviews are complete
- ✅ After architecture review is approved
- ✅ Before sprint execution begins (by Jan 2)

### What NOT to Update
- ❌ Don't remove essential labels (e.g., `p0.6-compliance`)
- ❌ Don't change status during sprint (use new comments instead)
- ❌ Don't move issues between sprints without team discussion

### Labels to Preserve
- `p0.6-compliance` - Legal compliance suite marker
- `blocked-by-#15` - Dependency tracking
- `feature`, `chore`, `enhancement` - Type labels

---

## 🚀 Quick Checklist for @ScrumMaster

- [ ] Sprint 001 milestone exists in GitHub
- [ ] Sprint 002 milestone exists in GitHub
- [ ] All required labels created
- [ ] Issue #57 updated with status
- [ ] Issue #56 updated with status
- [ ] Issue #15 updated with status (awaiting-legal label)
- [ ] Issue #48 updated with status (deferred)
- [ ] All sub-issues #20-28 updated with labels
- [ ] Verification: `gh issue list --milestone "Sprint 001"` shows correct issues
- [ ] Team notified of status updates
- [ ] Links to planning documents added to issue comments

---

## 📞 Support & Questions

**Questions about updating GitHub issue state?**
- See: GitHub CLI documentation `gh issue edit --help`
- Refer: Sprint planning documents in `.ai/sprint/`
- Contact: @ScrumMaster

**Need to make changes later?**
- Issue status changes during sprint: Add comments with updates
- Mid-sprint scope changes: Update labels and add comment
- Deferral decisions: Add "deferred" label + comment
- Completion: Update issue status to closed with closing comment

---

**Last Updated:** December 30, 2025  
**Next Review:** After Sprint 2 planning  
**Maintained By:** @ScrumMaster

---

**IMPORTANT:** Run this process immediately after sprint planning is approved by @Architect and @TechLead. GitHub issues are the source of truth for sprint status.
