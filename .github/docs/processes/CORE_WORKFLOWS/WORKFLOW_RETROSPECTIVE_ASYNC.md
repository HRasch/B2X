# WORKFLOW_RETROSPECTIVE - Async Iteration-Based (No Fixed Cadence)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN ITERATION RETROSPECTIVE  
**Status**: ACTIVE - Replaces sprint-end Friday cadence

---

## 🎯 Core Principle

**Retrospectives happen when an iteration completes (story points ±20% of velocity baseline), not on a calendar schedule. Async-first, optional sync discussion if needed.**

---

## 📋 When Iteration Ends (Retrospective Trigger)

### **Iteration Complete When**

**Primary Trigger**: Story points delivered ≈ Previous velocity baseline ±20%

**Example**:
- Velocity baseline: 21 story points/iteration
- Acceptable range: 17-25 story points
- When delivered = 17-25 points → Iteration complete → Trigger retrospective

**Secondary Trigger**: Team decides together
- "We're ready to reflect"
- May happen at 14 days even if below velocity
- Or happen at 10 days if velocity reached early

---

## 🔄 Retrospective Flow

### **Phase 1: Data Gathering** (2-4 hours)

**Owner**: @scrum-master  
**Trigger**: Iteration ends (velocity reached or time limit)

**What to Collect**:
1. **Build Status**
   - `dotnet build B2Connect.slnx` → Success/Fail
   - Build time trend
   - Warnings/errors count

2. **Test Results**
   - Test pass rate (target: 100%)
   - Coverage percentage (target: ≥80%)
   - Flaky tests (if any)
   - E2E test results

3. **Git History**
   - Commits created (count, quality)
   - PRs merged (count, cycle time)
   - Average PR size (lines changed)
   - Code review turnaround

4. **Story Points**
   - Committed: [X] points
   - Completed: [Y] points
   - Velocity trend: [compare to last 3 iterations]
   - Estimation accuracy: Estimated vs Actual per issue

5. **Code Quality**
   - Defects found (critical/high/medium/low)
   - Technical debt added or paid down
   - Security findings
   - Documentation updates

6. **Team Feedback** (async form or Slack poll)
   - Blockers encountered
   - What went well
   - What needs improvement
   - Suggestions for next iteration

**Duration**: 2-4 hours to collect (happens in parallel with team wrapping iteration)

---

### **Phase 2: Async Analysis** (Same day, 1-2 hours)

**Owner**: @scrum-master  
**Process**:

1. **Create Retrospective Document** (GitHub issue)
   ```
   Title: Iteration N Retrospective (Story Points: 21, Velocity: ✅ On target)
   
   ## Data Summary
   - Build: ✅ Passing
   - Tests: 204/204 (100%) ✅
   - Coverage: 82% ✅
   - PRs merged: 8
   - Avg cycle time: 4 hours
   - Velocity: 21 points ✅
   ```

2. **Post in Slack**: #retro-results with key metrics

3. **Invite Feedback** (async, 24-48 hours):
   - What went well?
   - What didn't?
   - What should we change?
   - React with emoji or comment with thoughts

---

### **Phase 3: Async Feedback Collection** (24-48 hours)

**Owner**: All team members  
**Process**:

Team responds asynchronously in GitHub issue:

```
✅ What Went Well:
- @backend-developer: "Build-first rule caught 3 errors early"
- @frontend-developer: "Component library stabilized"
- @qa-engineer: "100% test pass rate, no surprises"

⚠️ What Didn't Go Well:
- @devops-engineer: "CDP controller still blocking port 15500 on start"
- @tech-lead: "One blocker took 2 days to unblock"

💡 Suggestions:
- Automate port cleanup before startup
- Better dependency mapping in planning
- More documentation on X
```

**Duration**: 24-48 hours for feedback window (async, no meeting needed)

---

### **Phase 4: Prioritize Improvements** (1-2 hours)

**Owner**: @scrum-master  
**Trigger**: Feedback window closes

**Process**:

1. **Categorize Feedback**:
   - ✅ Positive (keep doing this)
   - ⚠️ Problem (needs fixing)
   - 💡 Opportunity (could improve)

2. **Assess Impact**:
   - High impact: Saves 4+ hours per iteration
   - Medium impact: Saves 1-4 hours per iteration
   - Low impact: Polish, nice to have

3. **Assess Effort**:
   - Low effort: < 2 hours to implement
   - Medium effort: 2-8 hours to implement
   - High effort: > 8 hours to implement

4. **Priority Matrix**:
   ```
   HIGH IMPACT + LOW EFFORT = Priority 1 (do immediately)
   HIGH IMPACT + MEDIUM EFFORT = Priority 2 (do next)
   MEDIUM IMPACT + LOW EFFORT = Priority 3 (do if time)
   LOW IMPACT + HIGH EFFORT = Don't do
   ```

---

### **Phase 5: Action Items** (1-2 hours)

**Owner**: @scrum-master + relevant agents  
**Process**:

1. **Create GitHub Issues** for Priority 1 & 2 improvements
   - Label: "process-improvement"
   - Assign owner + due date
   - Link to retrospective

2. **Examples**:
   - [ ] Automate CDP port cleanup script (@devops-engineer, due 3 days)
   - [ ] Add dependency tracking to planning (@scrum-master, due 5 days)
   - [ ] Documentation on [X] (@documentation-developer, due 1 week)

3. **Commit**: Team agrees to work on action items in next iteration

---

### **Phase 6: Documentation** (1 hour)

**Owner**: @scrum-master  
**Process**:

1. **Create Retrospective Summary**:
   - Iteration metrics (story points, test pass rate, velocity)
   - What went well (3-5 items)
   - What to improve (3-5 items)
   - Action items (with owners + deadlines)
   - Key learnings captured

2. **Learnings File** (if new learning discovered):
   - File: `.github/docs/RETROSPECTIVE_LEARNINGS.md`
   - Add validated learning from this iteration
   - Link to related retrospective

3. **Archive**: Keep retrospective issue for historical reference

---

## 🎯 No Fixed Cadence

### **OLD MODEL** (Friday 3:30-5:00 PM UTC every Friday)
- Fixed time
- Calendar-based
- Independent of actual iteration completion
- Sometimes mid-iteration

### **NEW MODEL** (When velocity target reached)
- Metric-based trigger
- Can be Tuesday, Thursday, or next Monday
- Happens after actual iteration complete
- Async-first (optional sync discussion if team prefers)

---

## 📊 Retrospective Metrics

Track these across iterations:

| Metric | Purpose |
|--------|---------|
| **Velocity** | Are we delivering consistently? |
| **Estimation Accuracy** | Story points vs actual |
| **Test Pass Rate** | Quality trend (target: 100%) |
| **Code Coverage** | Coverage trend (target: ≥80%) |
| **PR Cycle Time** | Review efficiency |
| **Action Items Closed** | Improvement follow-through |
| **Build Success** | System health |

---

## 🔄 Retrospective Cadence Example

**Iteration 1**:
- Starts: Mon Jan 6, 2025
- Velocity target: 21 points
- Completes: Wed Jan 8 (21 points delivered)
- **Retrospective**: Thu Jan 9 (next day after completion)

**Iteration 2**:
- Starts: Fri Jan 10, 2025
- Velocity target: 21 points
- Completes: Mon Jan 13 (23 points delivered)
- **Retrospective**: Tue Jan 14 (next day after completion)

**Iteration 3**:
- Starts: Wed Jan 15, 2025
- Velocity target: 21 points
- Takes longer... completes: Fri Jan 24 (19 points delivered, within range)
- **Retrospective**: Mon Jan 27 (same Monday, after completion)

---

## 🚫 What Triggers Special Retrospectives

**Emergency Retrospective** (immediate, not waiting for iteration end):
- Critical incident occurred (CRITICAL severity)
- Major blocker (2+ days of blocked work)
- Security finding
- Compliance issue

**Process**: Same as normal, but called immediately (async still)

---

## ✅ Retrospective Checklist

Before closing retrospective:

- [ ] **Metrics collected** (build, tests, velocity, PRs)
- [ ] **Feedback gathered** (team input captured)
- [ ] **Issues prioritized** (P1/P2/P3 assigned)
- [ ] **Action items created** (GitHub issues with owners)
- [ ] **Learnings documented** (if new learning)
- [ ] **Team agrees** (any process changes acknowledged)
- [ ] **Next iteration ready** (backlog refined, capacity planned)

---

## 📞 Retrospective Roles

| Role | Responsibility |
|------|-----------------|
| **@scrum-master** | Facilitate, gather data, document, prioritize improvements |
| **All Agents** | Provide feedback, suggest improvements, commit to action items |
| **@tech-lead** | Highlight technical learnings, support process changes |
| **@process-assistant** | Review process improvements, update workflows if needed |

---

## 🔗 Related Documents

- [WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md](./WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md) - Iteration triggering retrospective
- [RETROSPECTIVE_PROTOCOL.md](../../RETROSPECTIVE_PROTOCOL.md) - Detailed retrospective guidance

---

**Owner**: @process-assistant  
**Version**: 2.0 (Iteration-Based, No Friday Schedule)  
**Status**: ACTIVE  
**Key Change**: Retrospectives triggered by iteration completion (velocity ±20%), not calendar. Async-first feedback. Action items prioritized by impact/effort. Happens when iteration done, not on fixed day.
