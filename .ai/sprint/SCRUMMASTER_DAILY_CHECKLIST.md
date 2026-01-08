---
docid: SPR-065
title: SCRUMMASTER_DAILY_CHECKLIST
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 📋 @ScrumMaster Daily Operations Checklist

**Responsibility:** Daily standup facilitation, velocity tracking, metrics refresh  
**Time Required:** ~30 min morning + 30 min EOD  
**Tools:** Daily Standup Template, Tracking documents, Metrics dashboard  
**Frequency:** Every working day during Iteration 001

---

## 🌅 MORNING CHECKLIST (9:00 AM)

### Prepare Standup (10 minutes)

- [ ] Open [DAILY_STANDUP_TEMPLATE.md](./DAILY_STANDUP_TEMPLATE.md)
- [ ] Review yesterday's logged SP from [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)
- [ ] Check previous day's blockers (resolve status?)
- [ ] Create standup section for today
  ```markdown
  ## Day 1 Standup - [Date HH:MM]
  
  ### @Backend
  - Completed: [yesterday's work]
  - SP Logged: [X]
  - In Progress: [task]
  - Blockers: [none or describe]
  - Notes: [any notes]
  
  [Repeat for @Frontend, @Architect, @ProductOwner]
  ```
- [ ] Save template to [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)

### Run Standup (15 minutes)

- [ ] Start at scheduled time (9:00 AM)
- [ ] Ask each team lead:
  1. **Completed Yesterday:** What SP did you log?
  2. **In Progress:** What are you working on today?
  3. **Blockers:** Any blockers or risks?
  4. **Next Steps:** What's planned for today?
- [ ] Record answers in standup document
- [ ] Note any blockers or risks
- [ ] Confirm upcoming code reviews or architecture discussions

### Post-Standup (5 minutes)

- [ ] Review blockers from standup
- [ ] Escalate critical blockers to @TechLead or @Architect
- [ ] Confirm teams have what they need to execute
- [ ] Remind: Log work as it completes, not at EOD

---

## 💼 DURING DAY (Ongoing)

### Monitor Progress

- [ ] Check [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md) periodically
- [ ] Note SP being logged throughout day
- [ ] Watch for emerging blockers
- [ ] Provide support if teams need help

### Track Blockers

- [ ] Monitor [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) for blocker updates
- [ ] If critical blocker appears:
  - Note impact (how many SP affected?)
  - Escalate immediately to @TechLead or @Architect
  - Check status by end of day
- [ ] Don't let blockers extend more than 1 day without escalation

### Code Review Coordination

- [ ] Remind @TechLead of Issue #56 daily review schedule
- [ ] Ensure PRs are ready for daily review
- [ ] Help @Frontend get feedback quickly (don't queue PRs)

---

## 🌆 END OF DAY CHECKLIST (4:30 PM)

### Collect Team Data (15 minutes)

**Task: Get final work log from all teams**

- [ ] Ask each team lead to complete [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md) entry
  - What was completed today?
  - How many SP were logged?
  - What's the next task?
- [ ] Verify accuracy:
  - [ ] Tests passing?
  - [ ] Code reviewed (or PR created)?
  - [ ] SP count realistic?
  - [ ] Blocker status?
- [ ] Collect all entries by 4:45 PM

### Calculate Metrics (10 minutes)

**Task: Calculate velocity and update dashboard**

Use this formula:

```
1. Daily Total = Sum of all SP logged today
   Example: 3 (Backend) + 3 (Frontend) + 1 (Architect) + 1 (ProductOwner) = 8 SP

2. Running Total = Yesterday's Total + Today's Total
   Example: 0 (Day 0) + 8 (Day 1) = 8 SP

3. Days Elapsed = Current Day Number (Day 1, Day 2, etc.)
   Example: 2

4. Pace = Running Total / Days Elapsed
   Example: 8 SP / 2 days = 4 SP/day

5. Days Remaining = (28 - Running Total) / Pace
   Example: (28 - 8) / 4 = 5 days

6. Progress % = (Running Total / 28) * 100
   Example: (8 / 28) * 100 = 28%

7. Progress Bar = Visual representation
   Example: ████████░░░░░░░░░░░░░░░░░░ (8/28)
```

- [ ] Calculate daily total SP
- [ ] Calculate running total SP (cumulative)
- [ ] Calculate pace (SP per day average)
- [ ] Calculate estimated days remaining
- [ ] Calculate progress percentage

### Update Metrics Dashboard (5 minutes)

**Update [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md):**

- [ ] Update daily velocity log:
  ```markdown
  Day 1: 8 SP / 28 SP target (28%)
  ████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
  Pace: 4 SP/day | Remaining: 5 days
  ```

- [ ] Update team velocity table (optional):
  ```markdown
  | Team | Today | Total | % |
  |------|-------|-------|---|
  | @Backend | 3 | 3 | 11% |
  | @Frontend | 3 | 3 | 11% |
  | @Architect | 1 | 1 | 4% |
  | @ProductOwner | 1 | 1 | 4% |
  | **Total** | **8** | **8** | **28%** |
  ```

- [ ] Update cumulative velocity log in [ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md):
  ```markdown
  | Day | SP Logged | Running Total | Pace | ETA |
  |-----|-----------|----------------|------|-----|
  | 1 | 8 | 8 | 8 SP/day | 3 days |
  | 2 | 5 | 13 | 6.5 SP/day | 3 days |
  ```

### Update Standup Log (2 minutes)

- [ ] Finalize standup entries in [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)
- [ ] Add daily summary:
  ```markdown
  ### Daily Summary
  - **Total SP Logged:** 8 SP
  - **Running Total:** 8 / 28 (28%)
  - **Pace:** 4 SP/day
  - **Team Health:** ✅ Healthy
  - **Blockers:** None
  - **Next Day Focus:** [what's planned]
  ```

### Assess Team Health (5 minutes)

- [ ] Review blockers: How many? How long outstanding?
- [ ] Review code reviews: All completed today?
- [ ] Review tests: All passing?
- [ ] Review quality: Any concerns?
- [ ] Update team health indicator:
  - ✅ Healthy: No blockers, good pace, quality good
  - 🟡 Caution: 1-2 blockers, pace slowing, quality concerns
  - 🔴 Critical: 3+ blockers, pace very slow, quality issues

### Prepare Next Day (5 minutes)

- [ ] Note any blockers to follow up on
- [ ] Identify any teams needing support
- [ ] Prepare standup template for tomorrow
- [ ] Review velocity trend (accelerating? slowing?)
- [ ] Add notes for next day's standup

---

## 📊 WEEKLY REVIEW (Friday EOD)

### Velocity Trend Analysis (10 minutes)

- [ ] Review velocity from all days
- [ ] Calculate weekly average
- [ ] Identify trends:
  - Accelerating? ⬆️
  - Steady? ➡️
  - Slowing? ⬇️
- [ ] Project completion date
- [ ] Note any patterns

### Team Velocity Breakdown (5 minutes)

- [ ] Which team contributing most?
- [ ] Which team might need support?
- [ ] Are Phase 1 items tracking as planned?
- [ ] Any teams underperforming? Why?

### Risk Assessment (5 minutes)

- [ ] Total blockers this week?
- [ ] Longest blocker duration?
- [ ] Quality issues?
- [ ] Any risks emerging?

### Write Weekly Summary (5 minutes)

Add to [ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md):

```markdown
## Week 1 Summary (Dec 30 - Jan 3)

**Velocity:**
- Daily Average: X SP/day
- Weekly Total: X SP
- Running Total: X / 28 SP target
- Progress: X%

**Trends:**
- [Accelerating/Steady/Slowing]
- [Key observations]

**Team Health:**
- [Summary of each team's performance]

**Blockers:**
- [List of blockers, durations, status]

**Quality:**
- [Test pass rate, code review status, any concerns]

**Next Week Plan:**
- [What's expected]
- [Any changes to process or assignments]
```

---

## 📋 KEY DOCUMENTS TO MAINTAIN

### Daily Updates Required

| Document | Update When | What to Update |
|----------|------------|----------------|
| [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) | After standup + EOD | Team reports + daily summary |
| [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md) | EOD (after collecting SP) | Velocity progress + progress bar |
| [ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md) | EOD (after metrics) | Cumulative velocity log |

### Weekly Updates

| Document | Update When | What to Update |
|----------|------------|----------------|
| [ITERATION_001_OVERVIEW.md](./ITERATION_001_OVERVIEW.md) | End of week | Stakeholder summary |
| [EXECUTION_READY.md](./EXECUTION_READY.md) | As needed | Status changes |

---

## 🎯 Critical Success Factors

**Your Daily Success Depends On:**

✅ **Teams logging SP daily** - Not at EOD, as work completes  
✅ **Accurate velocity tracking** - Real numbers, not estimates  
✅ **Blockers escalated immediately** - Same day, not held  
✅ **Metrics updated EOD** - Fresh data for next standup  
✅ **Team health visible** - Everyone knows progress  
✅ **No surprises** - Communicate early and often  

---

## 🚨 Red Flags to Watch

| Red Flag | Action |
|----------|--------|
| No SP logged by noon | Check with teams, ensure work is happening |
| Blocker lasting > 1 day | Escalate to @TechLead or @Architect immediately |
| Velocity slowing significantly | Investigate cause, adjust support |
| Tests failing | Flag immediately, pause merges until fixed |
| Code reviews not happening | Remind @TechLead, reschedule if needed |
| Multiple teams blocked | War room with @Architect and @TechLead |

---

## 💡 Pro Tips

1. **Log work as it completes** - Tell teams: "Log SP when task done, not at EOD"
2. **Blockers = TOP PRIORITY** - Escalate immediately, even small ones
3. **Celebrate progress** - Show velocity daily in standup ("Great! 8 SP logged today!")
4. **Trend matters** - Watch pace, not just daily numbers
5. **Quality over speed** - If tests failing, pause work until fixed
6. **Communicate pace** - "At 4 SP/day, we'll hit 28 SP in 7 days"
7. **Support teams** - Remove obstacles, don't just track them
8. **Track blockers** - Keep list, follow up on resolution

---

## 📞 Escalation Path

**If you need help:**

| Issue | Escalate To |
|-------|-------------|
| Code/technical blocker | @TechLead |
| Architecture decision needed | @Architect |
| Requirements clarification | @ProductOwner |
| Process question | @TechLead or @Architect |
| Team conflict | Holger (project lead) |
| Velocity on track? | Review metrics weekly |

---

## ✅ Daily Checklist Summary

```
☑️ MORNING (30 min)
   ☐ Prepare standup (10 min)
   ☐ Run standup (15 min)
   ☐ Post-standup review (5 min)

☑️ DURING DAY (Ongoing)
   ☐ Monitor progress
   ☐ Watch for blockers
   ☐ Support teams

☑️ END OF DAY (30 min)
   ☐ Collect team data (15 min)
   ☐ Calculate metrics (10 min)
   ☐ Update dashboards (5 min)
   ☐ Assess health (5 min)
   ☐ Prepare next day (5 min)

☑️ WEEKLY (30 min on Friday)
   ☐ Analyze trends (10 min)
   ☐ Review team breakdown (5 min)
   ☐ Assess risks (5 min)
   ☐ Write summary (5 min)
```

---

## 📊 Template: Daily Refresh Process

**Save and reuse this every day:**

```
MORNING: Standup
├─ Prepare: 10 min
├─ Run: 15 min (9:00 AM)
└─ Review: 5 min

DURING: Monitor
└─ Check progress, watch blockers

EOD: Metrics Refresh (4:30 PM)
├─ Collect team data: 15 min
├─ Calculate metrics: 10 min
├─ Update dashboards: 5 min
├─ Assess health: 5 min
└─ Prepare next day: 5 min

Total Daily Time: ~1 hour
(30 min morning + 30 min EOD)
```

---

## 🎉 Your Mission

**Keep Iteration 001 running smoothly:**

✅ Teams know what they're working on  
✅ Progress visible daily  
✅ Blockers cleared immediately  
✅ Quality maintained  
✅ Team health good  
✅ Velocity target achievable  

**You're the keeper of the daily process. Teams depend on you to keep things moving!**

---

**Start First Standup:** Tomorrow 9:00 AM  
**Use Template:** [DAILY_STANDUP_TEMPLATE.md](./DAILY_STANDUP_TEMPLATE.md)  
**Log Location:** [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)  
**Metrics Dashboard:** [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md)  

**Good luck! 🚀**
