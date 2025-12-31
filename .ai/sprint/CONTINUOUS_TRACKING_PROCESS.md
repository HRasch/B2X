# 🔄 Continuous Velocity Tracking & Daily Refresh

**Purpose:** Automate daily metrics updates and ensure consistent tracking  
**Responsibility:** @ScrumMaster + Team  
**Frequency:** Daily (EOD each day)

---

## 📊 Daily Refresh Process

### 1. Team Logs Work (EOD Daily)

**Each team member updates:**
- [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md) - Work completed table
- [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) - Daily standup entry
- [ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md) - Velocity tracking table

**Example entry:**
```
| Dependency Audit | @Backend | 3 SP | Package analysis complete, 5 breaking changes found |
```

### 2. Calculate Daily Total

**Formula:**
```
Daily Total = Sum of all SP logged in day's work
Example: 3 + 2 + 1 + 1 = 7 SP today
```

### 3. Update Running Total

**Formula:**
```
Running Total = Yesterday's Total + Today's Total
Example: 0 + 7 = 7 SP cumulative
```

### 4. Update Metrics Dashboard

**Update [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md):**

```markdown
Day 1:    0 SP / 28 SP target (0%)
Day 2:    7 SP / 28 SP target (25%)  ███████░░░░░░░░░░░░░░░░░░░░
```

### 5. Calculate Pace

**Formula:**
```
Pace = Running Total SP / Days Elapsed
Example: 7 SP / 1 day = 7 SP/day

Estimated Days Remaining = (28 - Running Total) / Pace
Example: (28 - 7) / 7 = 3 days remaining
```

### 6. Assess Team Health

**Check:** 
- [ ] All SP accurately logged?
- [ ] Blockers noted and escalated?
- [ ] Code reviews completed?
- [ ] Tests passing?
- [ ] Architecture decisions logged?

---

## 📈 Daily Tracking Files

### Primary Files (Update Daily)

1. **[ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)**
   - What: Daily standup entries
   - Who: All team members
   - When: EOD each day
   - Update: Add standup section, log SP

2. **[ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md)**
   - What: Work log with completed tasks
   - Who: @ScrumMaster collects from team
   - When: EOD each day
   - Update: Add completed work rows, update daily total

3. **[ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md)**
   - What: Dashboard with velocity progress
   - Who: @ScrumMaster updates
   - When: After collecting day's work
   - Update: Daily velocity log, progress bar, pace calculation

### Secondary Files (Update as Needed)

4. **[ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md)**
   - What: Detailed tracking and work item status
   - Who: @ScrumMaster or owners
   - When: As work progresses
   - Update: Work item progress %, cumulative velocity log

---

## 🔄 Daily Refresh Workflow

```
1. Team Completes Work (During Day)
   ↓
2. Team Logs to ITERATION_001_DAY1_LOG.md (EOD)
   ↓
3. @ScrumMaster Collects All Entries (EOD)
   ↓
4. Calculate Daily + Running Totals
   ↓
5. Update ITERATION_001_METRICS.md (Master Dashboard)
   ↓
6. Update ITERATION_001_STANDUP_LOG.md (Next Day Standup)
   ↓
7. Assess Team Health & Flag Blockers
   ↓
8. Prepare for Next Day
```

---

## 📋 Checklist: Daily Refresh (EOD)

**@ScrumMaster Daily Tasks:**

- [ ] Collect SP logged from all teams (Issue #57, #56, Architect, ProductOwner)
- [ ] Verify accuracy (tests passing? code reviewed? done-done?)
- [ ] Calculate daily total SP
- [ ] Update running total: Previous + Today
- [ ] Calculate pace: Total SP / Days Elapsed
- [ ] Estimate ETA: (28 - Total) / Pace
- [ ] Update ITERATION_001_METRICS.md progress bar
- [ ] Update ITERATION_001_TRACKING.md cumulative log
- [ ] Check for blockers in standup entries
- [ ] Flag any risks to @TechLead or @Architect
- [ ] Prepare standup template for next day

---

## 📊 Example: Daily Refresh Scenario

### Day 2 Scenario

**Team Reports:**
- @Backend: Completed dependency audit (3 SP)
- @Frontend: Completed component inventory (3 SP)
- @Architect: Completed service boundary review (1 SP)
- @ProductOwner: Completed feature specs (1 SP)

**Calculations:**
```
Daily Total: 3 + 3 + 1 + 1 = 8 SP
Running Total: 0 + 8 = 8 SP
Days Elapsed: 2
Pace: 8 SP / 2 days = 4 SP/day
Days Remaining: (28 - 8) / 4 = 5 days
Estimated Completion: ~7 days from start

Progress: 8/28 = 28% ████████░░░░░░░░░░░░░░░░░░
```

**Files Updated:**
```
ITERATION_001_METRICS.md:
  Day 2: 8 SP / 28 SP target (28%) ████████░░░░░░░░░░░░░░░░░░
  Pace: 4 SP/day → ETA: 5 days remaining

ITERATION_001_STANDUP_LOG.md:
  Added Day 2 standup section with all team reports

ITERATION_001_TRACKING.md:
  Updated "Daily Velocity Log" with Day 2: 8 SP entry
```

---

## 🚨 Exception Handling

### If SP Not Logged by EOD

**Action:**
1. @ScrumMaster follows up with team member
2. Confirm work actually completed (tests? reviewed?)
3. If complete: log retroactively
4. If incomplete: move to next day's target

### If Blockers Identified

**Action:**
1. Flag in standup as "Blocker"
2. Note impact and owner
3. Escalate to @TechLead or @Architect if critical
4. Create unblocking task if needed

### If Pace Slowing

**Action (Only if significantly below target):**
1. Analyze root cause
2. Remove blockers
3. Adjust priorities if needed
4. Don't panic—velocity varies naturally

### If Pace Accelerating

**Action (If ahead of target):**
1. Monitor quality (tests? reviews? docs?)
2. Ensure accurate logging (not inflated estimates)
3. Celebrate momentum!
4. Plan Phase 2 transition early

---

## 📈 Metrics to Track

### Primary (Update Daily)

1. **Cumulative Velocity**
   - Current: X SP / 28 SP target
   - Percentage: (X / 28) * 100%
   - Progress bar: Visual representation

2. **Daily Pace**
   - Average SP per day
   - Trend: Accelerating? Stable? Slowing?

3. **Team Health**
   - Blockers count
   - Code reviews completed
   - Tests passing
   - Risk level (🟢 Low / 🟡 Medium / 🔴 High)

### Secondary (Update as Context Changes)

4. **Work Item Progress**
   - Issue #57 % complete
   - Issue #56 % complete
   - Architecture % complete
   - Planning % complete

5. **Risk Indicators**
   - New blockers identified
   - Quality issues found
   - Architecture decisions pending

---

## 🔗 Integration Points

**Daily Tracking Connects To:**

1. **GitHub Issues** (linked, not updated daily)
   - Status comments added manually
   - Labels already set (iteration/001, phase-1, velocity-tracked)

2. **Team Communications**
   - Daily standup shows progress
   - Blockers escalated in standup
   - Email or Slack: "Iteration 001 Day X: X SP logged"

3. **Planning Documents**
   - Completion % per issue
   - Work items checked off
   - Acceptance criteria met?

---

## 💡 Tips for Smooth Tracking

**For Team Members:**
- ✅ Log SP as work completes (don't wait for day-end)
- ✅ Update tracking file daily (takes 2 minutes)
- ✅ Flag blockers immediately (don't wait for standup)
- ✅ Be honest about % complete (done-done only)

**For @ScrumMaster:**
- ✅ Set daily reminder for metrics refresh
- ✅ Verify accuracy before updating dashboard
- ✅ Calculate pace and ETA daily
- ✅ Watch for trends (accelerating? slowing?)

**For Team Leads:**
- ✅ Review blockers from standup
- ✅ Support removing obstacles
- ✅ Validate quality of logged work
- ✅ Code review on schedule

---

## 🎯 Success Criteria

**Daily tracking is working when:**
- ✅ All SP logged accurately by EOD
- ✅ Running total updated daily
- ✅ Metrics dashboard current
- ✅ Blockers flagged immediately
- ✅ Pace visible and trending
- ✅ Team knows daily progress
- ✅ Stakeholders see real velocity

---

## 📞 Daily Standup Scripts

**Morning:**
```
"What did we complete yesterday?
What's the plan for today?
Any blockers?"
```

**EOD:**
```
"SP completed today: [X]
Running total: [X] / 28
New blockers: [None or describe]
Tomorrow's focus: [Tasks]
Next milestone: [When 28 SP reached or Phase 2 start]"
```

---

## 🚀 Automation Notes

**What's Manual:**
- Team logs SP (takes 2 min/team)
- @ScrumMaster reviews and summarizes

**What Can Be Automated (Future):**
- GitHub issue status synced to tracking
- Slack updates of daily progress
- Email summaries of blockers
- Metrics dashboard auto-generated

---

**Status:** Continuous Tracking - ACTIVE  
**Responsibility:** @ScrumMaster + All Teams  
**Frequency:** Daily EOD

Keep it simple. Track facts, not estimates. Celebrate progress.

