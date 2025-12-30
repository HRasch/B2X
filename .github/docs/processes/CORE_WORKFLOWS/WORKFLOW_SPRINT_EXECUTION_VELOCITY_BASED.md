# WORKFLOW_SPRINT_EXECUTION - Velocity-Based Model

**Owner**: @process-assistant (exclusive authority)  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN VELOCITY MODEL  
**Status**: ACTIVE - Replaces time-based sprint model

---

## 🎯 Core Principle

**No time-based sprints. No fixed timeboxes. Just velocity and story points.**

Development is continuous. We pull stories from the backlog, estimate story points, deliver them, and track velocity. When we hit our velocity target for an iteration, we stop and retrospective. Timeboxes are removed - agents work at their own pace with quality gates.

---

## 📋 Key Concepts

### **Velocity**
- **Definition**: Story points completed and merged per iteration
- **Calculation**: Sum of story points for all closed issues
- **Baseline**: First iteration establishes baseline (e.g., 35 points)
- **Target**: Next iterations aim for ±20% of baseline
- **Tracking**: Weekly dashboard showing cumulative points

### **Story Points**
- **Scale**: 1, 2, 3, 5, 8, 13, 21 (Fibonacci)
- **Estimation**: Team estimates at backlog refinement
- **Counted When**: Code merged to main AND tests passing
- **Accuracy**: Track estimated vs actual for retrospective learning
- **Velocity Impact**: Only merged, working code counts toward velocity

### **Iteration**
- **Start**: When previous iteration complete (not calendar-based)
- **End**: When velocity target reached OR all items complete
- **Duration**: Variable (3-14 days typical, depends on story complexity)
- **Scope**: Whatever fits within velocity target
- **Success**: Story points completed + code quality maintained

---

## 📋 Participants

- **Developers (All Roles)**: Pick up stories, code, submit PRs, deliver
- **@product-owner**: Prioritizes backlog, defines story acceptance criteria
- **@scrum-master**: Facilitates retrospectives, removes blockers
- **@tech-lead**: Architecture review, approves stories
- **@security-engineer**: Security reviews, compliance checks
- **@qa-engineer**: QA sign-off on code review

---

## 🔄 Continuous Delivery Flow

### **Phase 1: Backlog Intake**

**Trigger**: Story ready for development  
**Process**:
1. @product-owner prioritizes next story
2. Story has acceptance criteria defined
3. Team estimates story points
4. Developer picks up story

**Entry Criteria**: Story fully specified, points estimated, ready to code  
**Exit Criteria**: Developer assigned, story moved to "In Progress"  
**Duration**: As needed (not time-boxed)

---

### **Phase 2: Development & Testing**

**Trigger**: Developer picks up story  
**Process**:
1. Create feature branch
2. Code implementation
3. Write unit + integration tests
4. `dotnet build` succeeds
5. `dotnet test` passes (100%)
6. Coverage ≥80%

**Daily Standup** (10:00 AM UTC, 15 min):
- What stories did I progress?
- What am I working on next?
- Any blockers?

**Note**: Standup is status-only, no time reporting. We're not tracking hours - just story point progress.

**Entry Criteria**: Story assigned to developer  
**Exit Criteria**: PR created, code ready for review  
**Duration**: Depends on story points (5pt story ≠ 8pt story)

---

### **Phase 3: Code Review**

**Trigger**: PR created  
**Process**: See WORKFLOW_CODE_REVIEW.md (phases: Peer → Architecture → Security → QA)  
**Key Change**: No SLA timeboxes. Reviews happen when reviewer capacity available.

**Review Expectation**:
- Peer review: Priority 1 (as soon as possible)
- Architecture: Within normal work (not critical path)
- Security: Priority 1 if security-sensitive
- QA: Within normal work

**Feedback Loop**: Developer addresses comments, re-requests review

**Entry Criteria**: PR created, tests passing  
**Exit Criteria**: All reviews approved, ready to merge  
**Duration**: Variable (depends on reviewer availability)

---

### **Phase 4: Merge & Closure**

**Trigger**: All reviews approved  
**Process**:
1. Merge PR to main branch
2. Verify: Tests pass in main
3. Verify: Coverage ≥80%
4. Close GitHub issue
5. Story points counted toward velocity

**Entry Criteria**: All reviewers approved  
**Exit Criteria**: Code in main branch, tests passing, issue closed  
**Duration**: Minutes

**Story Point Counted When**: Code merged + main branch tests passing

---

### **Iteration Complete When**:

- ✅ Story points accumulated ≥ velocity target (e.g., 35 points)
- ✅ OR backlog empty / all high-priority items done
- ✅ OR 14 days elapsed (max iteration length)

Then:
- Team retrospective (async, GitHub issue)
- Next iteration begins immediately

---

## ✅ Quality Gates (Per Story Point)

Before story point counted as complete:
- [ ] Code compiles (0 errors, 0 warnings)
- [ ] Tests pass (100% of suite)
- [ ] Coverage ≥80%
- [ ] Code reviewed (all 4 checkpoints: peer, architecture, security, QA)
- [ ] No security issues
- [ ] Architectural patterns followed (Wolverine, Onion)
- [ ] TenantId filtering verified (multi-tenant safety)
- [ ] Audit logging in place (data modification)
- [ ] Documentation updated
- [ ] @tech-lead approval
- [ ] Merged to main branch

**No story points counted until ALL gates passed.**

---

## 🎯 Critical Rules (Every Story Point)

### **1. Build-First Rule** (MANDATORY)
- After creating code files → Immediately run `dotnet build`
- Before PR creation → All tests pass
- Prevents cascading failures
- Stop work if build fails

### **2. Test Coverage** (MANDATORY)
- Minimum: ≥80% for business logic
- All features require unit + integration tests
- 100% of test suite passes before merge
- Target: 0 regressions

### **3. Tenant Isolation** (MANDATORY)
- Every query filters by `TenantId`
- Code review verifies this
- Breach if missed = data leakage

### **4. Audit Logging** (MANDATORY)
- Every data modification logged
- Who, when, before/after values
- Encrypted for sensitive data

### **5. Security Review** (MANDATORY)
- OWASP Top 10 compliance
- PII encrypted (email, phone, name, address)
- No hardcoded secrets
- @security-engineer approval

---

## 📊 Velocity Tracking

### **Metrics (Per Iteration)**

| Metric | Purpose | Tracked |
|--------|---------|---------|
| **Story Points Completed** | Velocity baseline | Every iteration |
| **Estimated vs Actual** | Estimation accuracy | Retrospective |
| **Build Success Rate** | Code quality | Per story point |
| **Test Pass Rate** | Regression detection | Before merge |
| **Code Coverage** | Safety net | Per story point |
| **Blockers Encountered** | Risk identification | Per story point |
| **Review Turnaround** | Flow efficiency | Per story point |

### **Velocity Dashboard (Weekly)**

```
Week of 29. Dec:
  Monday:   15 points merged
  Tuesday:  12 points merged (1 blocked, 8 points waiting)
  Wednesday: 8 points merged
  Thursday:  10 points merged
  Friday:    5 points merged
  ─────────────────────
  Total:    50 points
  Target:   40-50 (in range ✅)
```

### **Long-Term Velocity Trends**

- **Iteration 1**: 42 points (baseline)
- **Iteration 2**: 45 points (+7%)
- **Iteration 3**: 40 points (-11%) ← Investigate why lower
- **Iteration 4**: 46 points (+15%)
- **Average**: 43 points/iteration

---

## 🚀 Escalation Rules (Agent-Driven)

### **Blocking Issue**
- Announce in Slack immediately
- Add "BLOCKED" label
- Document blocking reason
- Any agent can help unblock (not waiting for scrum master)
- Continue other stories while blocked

### **Build Failure**
- Stop current work
- Fix build immediately
- Re-run tests
- Continue when fixed

### **Test Failure**
- Investigate immediately
- Fix or revert
- Verify fix
- Document in GitHub

### **Security Issue**
- Notify @security-engineer immediately
- May block story point or require redesign
- Security is non-negotiable priority

### **Estimation Wrong**
- Discuss next standup
- Re-estimate if needed
- Track actual effort in retrospective
- Adjust future estimates if pattern emerges

---

## 📈 Iteration Retrospective

**Timing**: When iteration complete (story points reached target)  
**Duration**: 90 minutes (async, GitHub issue)  
**Process**:

1. **Metrics Review** (20 min)
   - Story points completed vs target
   - Blockers encountered
   - Quality metrics (coverage, test pass rate)
   - Estimation accuracy

2. **What Went Well** (20 min)
   - Successes to replicate
   - High-velocity stories
   - Smooth code reviews

3. **What Didn't Go Well** (20 min)
   - Blockers encountered
   - Slow reviews
   - Estimation misses
   - Quality issues

4. **Improvements** (20 min)
   - Identify 3-5 improvements
   - Prioritize (P1: do now, P2: next iteration, P3: backlog)
   - Assign owners

5. **Process Requests** (10 min)
   - Submit process improvements to @process-assistant via GitHub issue
   - @process-assistant reviews and updates workflows

---

## 🎯 No More Time-Based Constraints

### **What Changed**

❌ **Removed**:
- 5-day sprint cycles
- Monday kickoff / Friday retrospective
- Tuesday-Thursday deployment windows
- 4-hour code review SLAs
- 5-minute incident response times
- Daily standup attendance requirement
- Sprint velocity targets (replaced with flexible targets)

✅ **Added**:
- Continuous story-point tracking
- Velocity-based iteration boundaries
- Agent-driven workflow (no timeboxes)
- Async code reviews (reviewer-capacity driven)
- Flexible incident response (severity-driven, not time-driven)
- Standup as status update, not time report
- Velocity targets as ranges (±20% of baseline)

### **Key Principle**

**We measure progress in story points delivered, not hours worked.**

---

## 📞 Daily Standup (Simplified)

**Time**: 10:00 AM UTC (15 min)  
**Who**: All developers (async option via Slack if unavailable)  
**Format**:

```
@alice: "Finished 5pt story on product filtering. Starting 8pt story on search performance."
@bob: "In code review for 3pt story. Blocked on @tech-lead architecture approval (waiting 2 days)."
@charlie: "BLOCKED: Need database schema for order refunds (5pts). Can't proceed."
@devops: "No blockers. Reviewing 2 PRs. Will start 5pt deployment pipeline story next."
```

**Not Asked**: "How many hours?", "When will you finish?", "ETA?", "% complete?"

**Point**: Share story progress, identify blockers, keep team aligned.

---

## 📊 Success Definition (Per Iteration)

✅ **Iteration Success**:
- Story points reached target (±20%)
- All merged code has tests passing
- Coverage ≥80%
- No critical bugs found post-merge
- Blockers identified and escalated
- Retrospective completed with improvements captured

---

## 🔗 Related Workflows

- [WORKFLOW_CODE_REVIEW.md](./WORKFLOW_CODE_REVIEW.md) - How PRs are reviewed
- [WORKFLOW_BACKLOG_REFINEMENT.md](./WORKFLOW_BACKLOG_REFINEMENT.md) - Story estimation
- [WORKFLOW_RETROSPECTIVE.md](./WORKFLOW_RETROSPECTIVE.md) - Iteration reflection
- [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md) - When to escalate

---

## ✅ Checklist Per Story Point

Before counting story points:
- [ ] Code merged to main
- [ ] Tests passing (100%)
- [ ] Coverage ≥80%
- [ ] All reviews approved
- [ ] No security issues
- [ ] Build succeeds
- [ ] Documentation updated
- [ ] Issue closed in GitHub

---

**Owner**: @process-assistant  
**Version**: 2.0 (Velocity-Based)  
**Status**: ACTIVE - Use instead of time-based WORKFLOW_SPRINT_EXECUTION.md  
**Effective**: 29. Dezember 2025

