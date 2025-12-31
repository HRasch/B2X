# ⚡ Quick Command Reference - Iteration 001

**For:** All team members needing fast access to documents  
**Update Frequency:** Daily (add new day sections)  
**Responsibility:** @ScrumMaster maintains this list

---

## 🚀 JUST STARTING? READ THESE FIRST

### Your First Stop (5 min)
```
👉 PHASE_1_DEVELOPMENT_START.md
   Find your name (Issue #57, #56, Architecture, Planning)
   Read your task breakdown
   Start first task today
```

### Your Role Navigation (3 min)
```
👉 ITERATION_001_INDEX.md
   Find your role section
   Bookmark all linked documents
   Save quick shortcuts
```

---

## 📅 DAILY CHECKLIST

### Morning (Start of Day)
```bash
# 1. Open standup template
   → DAILY_STANDUP_TEMPLATE.md

# 2. Report yesterday's work
   Update ITERATION_001_STANDUP_LOG.md

# 3. Confirm today's tasks
   → PHASE_1_DEVELOPMENT_START.md
```

### During Day (As You Work)
```bash
# 1. Work on your assigned task
   Create PR, request review

# 2. Log work completion
   Update ITERATION_001_DAY1_LOG.md

# 3. If blocked, report immediately
   Update ITERATION_001_STANDUP_LOG.md
   Mention impact/ETA fix
```

### End of Day (EOD)
```bash
# 1. Final work log
   Complete entry in ITERATION_001_DAY1_LOG.md
   Record SP completed

# 2. EOD standup update
   ITERATION_001_STANDUP_LOG.md (final for day)
   Tomorrow's focus

# 3. @ScrumMaster refreshes metrics
   ITERATION_001_METRICS.md (updates dashboard)
```

---

## 📊 DOCUMENT QUICK ACCESS

### For Developers

**During Development:**
- 📖 [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) - Task details
- 📝 [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md) - Log completed SP
- 📅 [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) - Daily reports
- 🎯 [DAILY_STANDUP_TEMPLATE.md](./DAILY_STANDUP_TEMPLATE.md) - Standup format

**Reference:**
- 📋 [ITERATION_001_PLAN.md](./ITERATION_001_PLAN.md) - Full plan & acceptance criteria
- 🔄 [ITERATION_001_INDEX.md](./ITERATION_001_INDEX.md) - Navigation by role

---

### For @ScrumMaster (Daily Tasks)

**Every Morning:**
- 📝 Prepare standup template
- 📊 Check yesterday's velocity logging

**Every EOD:**
1. Collect SP from all teams
2. Calculate daily total: Sum all SP logged today
3. Calculate running total: Yesterday's total + today's SP
4. Calculate pace: Running total / Days elapsed
5. Calculate ETA: (28 - Running total) / Pace
6. Update dashboard:
   - → [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md)
   - → [ITERATION_001_TRACKING.md](./ITERATION_001_TRACKING.md)

**Process Guide:**
- 📖 [CONTINUOUS_TRACKING_PROCESS.md](./CONTINUOUS_TRACKING_PROCESS.md) - Daily refresh checklist

---

### For @TechLead

**Daily Code Reviews:**
- @Frontend Issue #56 has scheduled daily reviews
- Check PRs from Issue #56 work
- Approve or request changes

**Guidance & Mentoring:**
- Review blockers in [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)
- Support @Backend with dependencies analysis
- Support @Architect with technical decisions

**Reference:**
- 🏛️ [SPRINT_001_ARCHITECT_TECHLEAD_REVIEW.md](./SPRINT_001_ARCHITECT_TECHLEAD_REVIEW.md) - Architecture assessment (8.5/10)

---

### For @Architect

**Daily Contributions:**
- Document decisions in ADRs
- Support Issue #57 & #56 with architectural guidance
- Update service boundary documentation

**Reference:**
- 📋 [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) - Architecture tasks
- 🏛️ [SPRINT_001_ARCHITECT_TECHLEAD_REVIEW.md](./SPRINT_001_ARCHITECT_TECHLEAD_REVIEW.md) - Architecture baseline

---

### For @ProductOwner

**Daily Contributions:**
- Document feature specifications
- Answer requirements clarifications
- Validate acceptance criteria

**Reference:**
- 📋 [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) - Planning tasks
- 📋 [ITERATION_001_PLAN.md](./ITERATION_001_PLAN.md) - Full requirements

---

### For Stakeholders

**Daily (1 min check):**
- 📈 [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md) - Velocity progress dashboard

**Weekly (5 min overview):**
- 📊 [ITERATION_001_OVERVIEW.md](./ITERATION_001_OVERVIEW.md) - Executive summary

**Full Context:**
- 📋 [ITERATION_001_PLAN.md](./ITERATION_001_PLAN.md) - Complete plan

---

## 🎯 KEY METRICS AT A GLANCE

**Velocity Target:** 28 SP  
**Phase 1 Work:** 25 SP (all independent)  
**Buffer:** 6 SP (17%)  
**Team:** 6 leads + Contributors  
**Architecture:** 8.5/10 (Approved ✅)  
**Status:** Day 1 - 0 SP logged (Teams starting)  

**Update** [ITERATION_001_METRICS.md](./ITERATION_001_METRICS.md) **daily after logging.**

---

## 📌 IMPORTANT REMINDERS

✅ **Log work daily** - Don't wait for EOD or next day  
✅ **Update standup daily** - Report completed, in progress, blockers  
✅ **Flag blockers immediately** - Escalate same day, don't hold  
✅ **Request code reviews early** - Don't wait until PR is "final"  
✅ **Document decisions** - ADRs for architecture, comments for code  
✅ **Tests must pass** - Before merging any code  
✅ **No fixed dates** - Velocity-based, continue until 28 SP OR done  
✅ **Quality over speed** - Well-tested code beats quick code  

---

## 🚨 BLOCKER ESCALATION

**If you're blocked:**

1. Update [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) immediately
   - What: Describe the blocker
   - Impact: How many SP affected?
   - ETA: When do you expect it fixed?

2. Tag in standup: **Blocker - Issue #57 waiting for X**

3. Escalate if critical:
   - @TechLead for code/technical issues
   - @Architect for design decisions
   - @ProductOwner for requirements
   - @ScrumMaster if process blocked

4. Update daily until resolved

---

## 🔗 DOCUMENT LINKS

**Main Navigation:** [ITERATION_001_INDEX.md](./ITERATION_001_INDEX.md)  
**Sprint Hub:** [INDEX.md](./INDEX.md)  
**Execution Ready:** [EXECUTION_READY.md](./EXECUTION_READY.md)  
**Launch Status:** [ITERATION_001_LAUNCH_COMPLETE.md](./ITERATION_001_LAUNCH_COMPLETE.md)  
**Go/No-Go:** [ITERATION_001_READINESS.md](./ITERATION_001_READINESS.md) ✅  

---

## 📞 WHO TO ASK

| Question | Ask |
|----------|-----|
| What's my task? | See [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) |
| How do I log work? | See [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md) |
| Standup format? | See [DAILY_STANDUP_TEMPLATE.md](./DAILY_STANDUP_TEMPLATE.md) |
| Need code review? | Ask @TechLead |
| Need architectural guidance? | Ask @Architect |
| Need requirements clarification? | Ask @ProductOwner |
| Velocity/tracking questions? | Ask @ScrumMaster |
| General blockers? | Report in standup + flag in [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md) |

---

## ✅ YOUR FIRST DAY CHECKLIST

Day 1 Tasks:
- [ ] Read [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) (your role section)
- [ ] Create feature branch
- [ ] Start first task (see your SP allocation)
- [ ] Submit PR by EOD (even if WIP)
- [ ] Update [ITERATION_001_STANDUP_LOG.md](./ITERATION_001_STANDUP_LOG.md)
- [ ] Log SP to [ITERATION_001_DAY1_LOG.md](./ITERATION_001_DAY1_LOG.md)
- [ ] Attend standup meeting

---

**🚀 You're all set. Start with [PHASE_1_DEVELOPMENT_START.md](./PHASE_1_DEVELOPMENT_START.md) and begin Phase 1 execution!**

---

**Last Updated:** Day 1  
**Status:** ✅ GO  
**Next Update:** Daily (as new days added)  
