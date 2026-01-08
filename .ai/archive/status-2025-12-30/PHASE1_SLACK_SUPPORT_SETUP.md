---
docid: UNKNOWN-061
title: PHASE1_SLACK_SUPPORT_SETUP
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# Slack Support Setup

**Channel**: #subagent-support  
**Purpose**: Phase 1 SubAgent support, questions, blockers, feedback  
**Owner**: @TechLead  
**Response Time**: <15 minutes during business hours  

---

## Channel Configuration

### Channel Details
- **Name**: subagent-support
- **Topic**: SubAgent Phase 1 support, questions, blockers, feedback collection
- **Description**: Dedicated support for 8 Phase 1 SubAgents. Ask questions, report issues, provide feedback. Escalate blockers to @TechLead.

### Access
- **Public**: Yes (all team members)
- **Posting**: Anyone
- **Notification**: Low (muted by default, pinned messages)

### Pinned Messages (4)
1. **Agent Index**: `.ai/status/SUBAGENT_TIER1_DEPLOYMENT_GUIDE.md`
2. **Readiness Report**: `.ai/status/PHASE1_LAUNCH_READINESS.md`
3. **How to Delegate**: Link to training materials
4. **Escalation**: When & how to escalate blockers

---

## Response Templates

### Template 1: Agent Question (Immediate Response)

**User Message**:
```
@TechLead Which SubAgent should I use for [specific question]?
```

**Response Template**:
```
✅ Great question! Based on your need to [specific need], use:

**@SubAgent-[AgentName]** - [Agent expertise]

Example delegation:
"@SubAgent-[AgentName], [your specific question]"

Expected output:
- [Output item 1]
- [Output item 2]
- [Output item 3]

Output location: `.ai/issues/{your-task-id}/`

More info: [Link to deployment guide]
```

### Template 2: Blockers/Issues (Escalation)

**User Message**:
```
I delegated to @SubAgent-[Agent] but [blocker description]
```

**Response Template**:
```
🚨 Noted. This is now [PRIORITY LEVEL].

Details logged:
- Agent: @SubAgent-[Agent]
- Issue: [Blocker]
- Time: [timestamp]
- Status: **INVESTIGATING**

Next steps:
1. [Investigation step 1]
2. [Investigation step 2]
3. Update you in [timeframe]

In the meantime: [Workaround if available]

Questions? Reply here.
```

### Template 3: Feedback/Suggestions (Positive)

**User Message**:
```
The @SubAgent-[Agent] output was great! Suggestion: [improvement]
```

**Response Template**:
```
🎉 Thanks for the positive feedback! So glad the output was helpful.

Suggestion noted:
> [Suggestion]

Logged for:
- Learning system (weekly improvement cycle)
- Agent improvement tracking
- Next update planning

This will inform agent refinement in Phase 2.

Keep the feedback coming!
```

### Template 4: Suggestions for Improvement (Constructive)

**User Message**:
```
@SubAgent-[Agent] output was useful but [improvement area]
```

**Response Template**:
```
✅ Thanks for the constructive feedback!

Area for improvement:
> [Improvement suggestion]

Status:
- ✅ Logged in learning system
- 📋 Queued for agent update
- 🔄 Will be refined based on team feedback

Next steps:
1. Monitor similar feedback from other teams
2. Analyze patterns in improvements needed
3. Update agent instructions
4. Roll out improvements

Timeline: Weekly improvement cycle (next update [date])

More feedback welcome!
```

### Template 5: Escalation to @SARAH (Critical)

**User Message**:
```
Critical blocker: [Issue that blocks Phase 1 success]
```

**Response Template**:
```
🚨 CRITICAL - Escalating to @SARAH

Issue:
- [Description]
- Impact: [Team/task impact]
- Severity: [CRITICAL/HIGH/MEDIUM]
- Time logged: [timestamp]

@SARAH - This requires immediate decision/action.

Status: **ESCALATED**
Next update: [timeframe]
```

---

## Daily Support Schedule

### Morning (9:00-10:00)
- Answer overnight questions
- Check for new blockers
- Share daily update with teams

### Mid-Day (12:00-13:00)
- Quick check-in
- Address urgent blockers only
- Prepare for afternoon standups

### Evening (16:00-17:00)
- Answer day's accumulated questions
- Triage new issues
- Prepare morning briefing

### After Hours
- Blockers only (escalate to on-call)
- Non-urgent questions answered next business day

---

## Escalation Workflow

### Level 1: Team Support (@TechLead)
**Triggers**:
- Agent output unclear
- Delegation format question
- When to use agent question
- Suggestion for improvement

**Response Time**: <15 min  
**Resolution**: 1-2 hours  
**Action**: Answer, log feedback, move on

### Level 2: Technical Issue (@TechLead + @Architect)
**Triggers**:
- Agent output incorrect
- Multiple teams report same issue
- Output doesn't match expectations
- Quality degradation

**Response Time**: <30 min  
**Resolution**: 1-4 hours  
**Action**: Diagnose, document, plan fix

### Level 3: Blocker (@TechLead + @SARAH)
**Triggers**:
- Blocks Phase 1 success
- Affects multiple teams
- Prevents adoption >50%
- Security/compliance issue

**Response Time**: IMMEDIATE  
**Resolution**: 15-60 min  
**Action**: Emergency fix or rollback decision

### Level 4: Policy/Governance (@SARAH Only)
**Triggers**:
- Conflict between agents
- Authorization question
- Major process change
- Phase gate decision

**Response Time**: Same-day  
**Resolution**: Decision + documentation  
**Action**: Clarify authority, update guidelines

---

## Daily Standup Integration

### Standup Agenda Item (2 minutes)
```
SUBAGENT STATUS:
- Blockers? → #subagent-support
- Usage feedback? → #subagent-support
- Questions? → Ask now or Slack
```

### Team Report Back (Optional)
```
Example from @Backend team:
"We delegated to @SubAgent-APIDesign twice today. 
Output was useful. Minor suggestion: [feedback]. 
Continuing to use in Phase 1."
```

---

## Weekly Feedback Collection

### Friday 17:00 - 17:30 Meeting
**Attendees**: Team leads (@Backend, @Frontend, @QA, @Security, @Legal)

**Agenda**:
1. Adoption rate this week
2. Top 3 blockers faced
3. Top 3 positive feedback items
4. Suggestions for improvement
5. Confidence vote: Continue to Phase 2? (Yes/No)

**Output**: Logged in `.ai/logs/phase1-weekly-feedback.md`

---

## Feedback Loop to Learning System

**Process**:
1. Issues/suggestions logged in #subagent-support
2. Daily log in `.ai/logs/phase1-daily.md`
3. Weekly analysis (Friday 17:30)
4. Feedback summary to learning system
5. Learning cycle processes improvements
6. Improvements rolled out next cycle

**Learning Inputs**:
- ✅ User satisfaction (1-5 scale)
- ✅ Specific suggestions
- ✅ Quality issues
- ✅ Missing features
- ✅ Usage patterns

---

## Metrics Tracking Template

### Daily Standup Summary (Sample)
```markdown
# Phase 1 Daily Summary - [Date]

## SubAgent Activity
- @SubAgent-APIDesign: 3 delegations today
- @SubAgent-DBDesign: 2 delegations today
- @SubAgent-ComponentPatterns: 4 delegations today
- @SubAgent-Accessibility: 1 delegation today
- @SubAgent-UnitTesting: 5 delegations today
- @SubAgent-ComplianceTesting: 1 delegation today
- @SubAgent-Encryption: 0 delegations today
- @SubAgent-GDPR: 2 delegations today

## Issues Reported
- [Brief description if any]

## Feedback Highlights
- [Positive feedback item]
- [Suggestion for improvement]

## Status
- ✅ All systems operational
- Adoption tracking: [%]
- Team confidence: [High/Medium/Low]
```

---

## FAQ for Support Responses

### "When should I delegate to an agent?"
> When you need **guidance**, **patterns**, **verification**, or **documentation**. 
> NOT for active implementation or decisions.

### "How long does a delegation take?"
> Usually 2-5 minutes from submission to output in `.ai/issues/`.

### "Can multiple teams delegate at the same time?"
> Yes - all execute in parallel. No queuing.

### "What if I disagree with the output?"
> Your team makes final decisions. Use what's useful, adapt as needed.
> Request clarification if needed.

### "Is there a wrong way to delegate?"
> Vague requests get vague outputs. Be specific about your need.

### "Can I request output revisions?"
> Yes - request clarification or different angle on question.

### "What if there's a critical blocker?"
> Post in #subagent-support with [CRITICAL] tag. @TechLead escalates to @SARAH.

### "When does Phase 2 start?"
> If Phase 1 adoption >50% by Friday (Jan 10), Phase 2 starts Jan 13.

### "Can I suggest improvements to agents?"
> Yes - feedback is valuable. Post in #subagent-support, it feeds learning cycle.

---

## Support Owner Checklist

**Daily**:
- [ ] Check #subagent-support first thing
- [ ] Respond to questions within 15 min
- [ ] Log daily activity in `.ai/logs/phase1-daily.md`
- [ ] Check for escalation-worthy issues

**Weekly (Friday)**:
- [ ] Run team feedback meeting
- [ ] Summarize week's feedback
- [ ] Calculate adoption metrics
- [ ] Prepare learning cycle input
- [ ] Brief @SARAH on phase gate readiness

**After Phase 1**:
- [ ] Final metrics report
- [ ] Team feedback summary
- [ ] Lessons learned
- [ ] Approval/adjustment for Phase 2

---

**Channel Status**: ✅ READY TO LAUNCH  
**Owner**: @TechLead  
**Support Model**: Responsive (15-min SLA)  
**Escalation**: Immediate for blockers  
**Feedback Loop**: Daily collection → Weekly analysis → Learning system  
