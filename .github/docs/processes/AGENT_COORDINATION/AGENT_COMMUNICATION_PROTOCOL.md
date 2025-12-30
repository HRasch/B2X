# AGENT_COMMUNICATION_PROTOCOL.md

**Owner**: @process-assistant (exclusive authority)  
**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE

---

## 🎯 Overview

This protocol defines how B2Connect agents communicate, coordinate, and share information. Clear communication ensures alignment, prevents duplicated work, and enables rapid problem-solving.

---

## 📋 Communication Channels

### **Synchronous (Real-Time)**

**Slack**:
- General discussion
- Quick questions
- Status updates
- Emergency alerts

**Usage**:
- `#general`: Whole team
- `#backend-dev`: Backend developers
- `#frontend-dev`: Frontend developers
- `#qa-team`: QA and testing
- `#devops`: Infrastructure
- `#incident-[id]`: Active incidents
- DMs: Private discussions

**Best Practices**:
- Thread replies to keep conversations organized
- @ mention relevant people
- Don't expect immediate response (async mindset)
- Use relevant emoji for priority:
  - 🔴 URGENT: Requires response < 30 min
  - 🟠 HIGH: Requires response < 4 hours
  - 🟡 NORMAL: Response expected < 24 hours

**Standup Format** (Daily, 10:00 AM UTC):
```
@[agent-name]:
✅ Completed yesterday: [Item 1], [Item 2]
🔄 Working on today: [Item 1], [Item 2]
🚫 Blockers: [If any]
```

### **Asynchronous (Written Record)**

**GitHub Issues**:
- Feature requests
- Bug reports
- Task tracking
- Decision documentation

**PR Comments**:
- Code review feedback
- Technical discussion
- Design review
- Implementation notes

**Email**:
- Formal notifications
- Legal/compliance items
- Executive updates
- External communication

---

## 🤝 Coordination Patterns

### **Pattern 1: Daily Standup**

**When**: 10:00 AM UTC (15 minutes)

**Format**:
1. Team members post in #general:
   ```
   ✅ Yesterday: [completed items]
   🔄 Today: [planned items]
   🚫 Blockers: [if any]
   ```

2. @scrum-master summarizes blockers
3. Any urgent issues escalated immediately
4. Team disperses to continue work

**Outcome**: Team alignment, blocker identification

---

### **Pattern 2: Code Review**

**When**: Throughout sprint (see WORKFLOW_CODE_REVIEW.md)

**Format**:
1. PR author requests review from specific people
2. Reviewers post comments with feedback
3. Author responds to comments, pushes changes
4. Reviewers approve when concerns addressed

**Coordination**:
- Use GitHub PR interface for all code comments
- Reply in threads to keep organization
- Don't re-request same changes
- Approve promptly once satisfied

**Outcome**: Code quality, knowledge sharing, architecture alignment

---

### **Pattern 3: Architecture Discussion**

**When**: Major features or tech decisions

**Format**:
1. @tech-lead writes design document in issue comments
2. Team reviews and provides feedback
3. @tech-lead updates design based on feedback
4. Final approval before implementation

**Escalation**:
- If disagreement: Follow WORKFLOW decision-making rules
- If architectural: @software-architect involved
- If complex: Schedule design review meeting

**Outcome**: Aligned technical approach, reduced rework

---

### **Pattern 4: Incident Response**

**When**: Critical issue in production

**Format**:
1. Issue declared in #incident-[id]
2. War room link posted
3. Updates every 5 minutes in Slack
4. Final update when resolved

**Communication**:
- Keep updates concise
- Don't chat in war room (focus on fixing)
- Document decisions in Slack thread
- Preserve for post-incident analysis

**Outcome**: Coordinated crisis response, documented recovery

---

## 🔄 Decision-Making Protocol

### **Simple Decision** (2 people)

**Process**:
1. Person A proposes in Slack/GitHub
2. Person B provides feedback (within 4 hours)
3. Decision made based on discussion

**Escalate if**: Disagreement or uncertainty

**Outcome**: Quick consensus, documented in Slack

---

### **Complex Decision** (3+ people)

**Process**:
1. One person documents options in GitHub issue
2. Team provides input (24-hour window)
3. @tech-lead makes final decision
4. Decision documented in issue

**Voting** (if consensus needed):
- Ask for explicit votes (✅ / ⚠️ / ❌)
- Majority wins
- Document reasoning

**Escalation**: To @software-architect if still deadlocked

**Outcome**: Documented decision, team alignment

---

## 📊 Information Sharing

### **Weekly Metrics Report** (Friday 4 PM UTC)

**Format** (post in #general):
```
📊 Weekly Metrics (Week of [date])

Build Health:
- Success Rate: X% (Target: 100%)
- Average Time: Xs (Target: <10s)
- Warnings: [list if any]

Test Coverage:
- Pass Rate: X% (Target: >95%)
- Coverage: X% (Target: ≥80%)
- Failures: [any?]

Velocity:
- Points Delivered: X
- Planned: Y
- Variance: [+/- %]

Team Health:
- Open Blockers: [list]
- PRs Waiting Review: [count]
- On-Call Status: [who/status]

Next Week:
- Focus: [what's planned]
- Risks: [if any]
```

**Owner**: @scrum-master
**Audience**: Whole team + leadership

---

### **Sprint Summary** (End of day 5)

**Format** (GitHub release notes):
```markdown
## Sprint [N] Summary

### Completed Features
- [Issue #XX] [Title] ✅
- [Issue #YY] [Title] ✅

### Metrics
- Velocity: X points
- Quality: X% test pass rate
- Coverage: X%

### Known Issues
- [Issue #ZZ] [Title] (planned for Sprint N+1)

### Next Sprint
- Focus: [feature areas]
- Capacity: X hours
```

**Owner**: @scrum-master
**Audience**: Product, leadership, stakeholders

---

## ⚠️ Escalation Path

**If you need help**:
1. Ask in relevant Slack channel
2. If no response in 4 hours → Direct message specific person
3. If still no response → @scrum-master involved
4. @scrum-master escalates to @tech-lead if needed

**Priority Levels**:
- 🔴 **URGENT** (< 30 min): Mention in Slack, page if critical
- 🟠 **HIGH** (< 4 hours): @ mention in Slack
- 🟡 **NORMAL** (< 24 hours): Post in channel, no mention needed
- 🟢 **LOW** (< 1 week): Comment on GitHub issue

---

## 🚫 Communication Anti-Patterns

**Don't**:
- [ ] Make major decisions in Slack without documenting in GitHub
- [ ] Continue discussion in 1:1 without updating the team
- [ ] Assume someone knows what you're doing
- [ ] Wait more than 1 hour to report a blocker
- [ ] Send private messages for team-relevant discussion

**Do**:
- [x] Document decisions in GitHub
- [x] Keep team in Slack channel about progress
- [x] Share discoveries with team immediately
- [x] Report blockers in standup or #general
- [x] Use public channels for team matters

---

## 📱 Notification Settings

**Recommended Slack Settings**:
- Turn on notifications for:
  - Mentions (@[name])
  - Thread replies
  - #incident-[id] channel (all messages)
  - #critical alerts
- Turn off:
  - All message notifications
  - Scheduled digest at 10:00 AM (during standup)

**GitHub Notifications**:
- Turn on for:
  - Comments on your PRs
  - PR reviews requesting changes
  - Mentions
- Turn off:
  - All comment activity
- Use GitHub digest: Daily at 9:00 AM UTC

---

## 🔄 Related Workflows

- [WORKFLOW_SPRINT_EXECUTION.md](./WORKFLOW_SPRINT_EXECUTION.md) - Uses standup communication
- [WORKFLOW_CODE_REVIEW.md](./WORKFLOW_CODE_REVIEW.md) - Uses PR comments for coordination
- [WORKFLOW_INCIDENT_RESPONSE.md](./WORKFLOW_INCIDENT_RESPONSE.md) - Uses #incident channel

---

## ✅ Communication Checklist

**Before Asking for Help**:
- [ ] Tried to solve it yourself (30+ minutes)
- [ ] Searched existing documentation
- [ ] Searched GitHub issues for similar problems
- [ ] Described problem clearly
- [ ] Provided context (what, why, impact)

**Before Escalating**:
- [ ] Waited at least 4 hours for response
- [ ] Pinged person twice (once in thread, once direct)
- [ ] Confirmed person is not on vacation

**When Sharing Decision**:
- [ ] Documented in GitHub (not just Slack)
- [ ] Linked to any related discussions
- [ ] Explained reasoning and alternatives considered
- [ ] Noted any assumptions or risks

---

**Version History**:
- v1.0 (29 Dec 2025): Initial release - Communication channels and coordination patterns

**Next Review**: After first month of execution (projected 29 Jan 2026)
