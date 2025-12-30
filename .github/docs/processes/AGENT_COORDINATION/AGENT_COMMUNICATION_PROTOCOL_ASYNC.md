# AGENT_COMMUNICATION_PROTOCOL - Async-First (Optional Sync)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN ASYNC COMMUNICATION  
**Status**: ACTIVE - Replaces 10:00 AM UTC daily standup requirement

---

## 🎯 Core Principle

**Agents communicate async via Slack + GitHub. Optional sync standup (15 min) if team prefers, but not required.**

---

## 📋 Async-First Communication

### **Daily Status Updates** (Async in Slack)

**Timing**: Anytime during your work day (no fixed time)

**Where**: `#standup-async` Slack channel  
**Format**:
```
@[your-name] standup for [date]

Yesterday: [1-2 sentences on what you did]
- Completed Issue #30 (database refactor)
- Code review for @someone

Today: [1-2 sentences on what you're doing]
- Working on Issue #31 (API integration)
- Planning tests

Blockers: [If any]
- Waiting on @devops feedback on ports

Story Points: Yesterday 5 pts delivered, Today targeting 8 pts
```

**Duration**: 2-3 minutes to write

**When**: Sometime between 8-11 AM your local time (we're distributed)

**Goal**: Team visibility into progress, not realtime standup

---

### **Real-Time Slack Communication** (Primary Channel)

**Channels**:
- `#general` - Random, off-topic, memes
- `#development` - Code questions, technical discussions
- `#devops` - Infrastructure, deployment, health
- `#design` - UI/UX, accessibility
- `#questions` - Ask teammates anything
- `#incident-[severity]` - During incidents

**Response Time**:
- **Same hour**: Technical question (block resolution)
- **Same day**: General question or feedback
- **Next day**: Non-urgent discussion

---

### **GitHub Communication** (Async Discussions)

**For**:
- Code review feedback
- Issue questions
- Design decisions
- Process discussions

**Format**: GitHub issue comments with threading

**Response Time**: Within 24 hours (non-blocking)

---

## 🎤 Optional Sync Standup

### **When to Have Sync Standup**

**Trigger** (any one of these):
1. Team feels disconnected (> 1 week without sync)
2. Complex coordination needed (> 3 teams blocked)
3. New team member (onboarding)
4. Major incident postmortem
5. Team wants to (no reason needed)

**Voting**: If someone says "should we do sync standup?", team votes in Slack. 50%+ = do it.

---

### **Sync Standup Format** (If Held)

**Timing**: 15 minutes, scheduled when best for team  
**Platform**: Slack voice or Zoom  
**Attendees**: Whoever wants to join (optional)

**Agenda** (5 min per segment):
1. **Blockers** (3 min) - Anyone blocked? How can we help?
2. **Wins** (1 min) - Celebrate completed work
3. **Upcoming** (1 min) - What's next priority?

**Recording**: Optional (for those who can't attend)

---

## 🚫 No Required Standup

### **Why Async-First**

```
OLD MODEL: Daily 10:00 AM UTC standup (required)
- Timezone unfriendly (some agents have to wake up early)
- Meeting tax: 15 min meeting × 5 days = 1.25 hours/week
- Interrupts deep work
- Less documentation (spoken only)

NEW MODEL: Async standup in Slack
- Timezone flexible (each agent posts when convenient)
- No meeting tax (asynchronous)
- Documented in Slack (searchable history)
- Allows deep work flow
- Optional sync if coordination needed
```

---

## 📞 Decision-Making Communication

### **For Quick Decisions** (< 1 hour needed)

**Channel**: `#questions` Slack  
**Process**:
1. Post question with context
2. Relevant agent responds in thread
3. Decision made in thread
4. Move on

**Example**:
```
@backend-dev: "Should we cache product list for 5 min or 1 hour?"

@tech-lead: "5 min is better for freshness. 1 hour only if heavy load."

@backend-dev: "5 min it is. Implementing now."
```

---

### **For Complex Decisions** (> 1 hour thinking needed)

**Channel**: Create GitHub issue or discussion  
**Process**:
1. Open GitHub issue with question
2. Add `type: decision` label
3. Async comments with reasoning
4. @tech-lead or @process-assistant makes final call
5. Document decision in comments

**Example**:
```
GitHub Issue: "Should we split Search service from Catalog?"

@tech-lead (comment):
"Pros: Independent scaling, cleaner separation
Cons: More services to manage, added complexity
Decision: No - keep together for now, revisit in 6 months"

Issue closed with decision documented.
```

---

## 🔄 Escalation Communication

### **When Normal Communication Isn't Working**

**Escalation Path**:
1. Direct Slack DM (urgent)
2. Slack @mention in channel (30 min response)
3. GitHub issue mention @person (24 hour response)
4. Email (rare, 24 hour response)

**When to use each**:
- **DM**: "I need decision in next 5 min" (rare)
- **Mention**: "Need feedback today" (common)
- **GitHub**: "Need response eventually" (most things)
- **Email**: Production emergency + SMS notification (very rare)

---

## 📋 Communication Best Practices

### **In Slack**

✅ **DO**:
- Be concise (1-2 sentences per message)
- Use threading for long conversations
- React with emoji instead of "+1" messages
- Share links to GitHub/docs for context
- Ask questions clearly

❌ **DON'T**:
- Ping someone at 2 AM (unless emergency)
- Expect realtime response (assume 24h response time)
- Write long essays in Slack (use GitHub instead)
- Leave context in DMs (post in public channels)

### **In GitHub**

✅ **DO**:
- Quote previous comments
- Link to related issues
- Use code blocks for examples
- Explain reasoning in detail

❌ **DON'T**:
- Expect same-day response (assume 48h)
- Use for urgent discussions (use Slack)
- Leave conversations unresolved (ping if stuck)

---

## 🎯 Meeting Attendance

### **Required Meetings** (None)

- Standup? **Optional** (async default)
- Planning? **Optional sync** (could be async)
- Retrospective? **Mostly async** (optional sync discussion)
- Code review? **Async in GitHub** (no meeting)

### **Optional Meetings** (As Needed)

- Complex design discussion (sync whiteboard)
- New team member onboarding (1:1 sync)
- Incident postmortem (sync discussion)
- Team building (sync social)

---

## 🔗 Communication Channels Map

| Channel | Purpose | Response Time |
|---------|---------|----------------|
| `#general` | Random, off-topic | No expectation |
| `#standup-async` | Daily status updates | Daily |
| `#development` | Code questions | Same hour |
| `#devops` | Infrastructure | Same hour (if critical) |
| `#design` | UI/UX | Same day |
| `#incident-[sev]` | During incidents | ASAP (severity dependent) |
| GitHub issues | Discussions, decisions | 24 hours |
| GitHub PRs | Code review | 24 hours |
| Email | Urgent escalation | 24 hours |

---

## 📊 Communication Metrics

Track these quarterly:

| Metric | Target |
|--------|--------|
| **Standup completion** | 80%+ agents posting daily |
| **Decision turnaround** | < 24 hours for quick decisions |
| **Issue response time** | < 24 hours average |
| **PR review time** | < 24 hours average |
| **Escalation frequency** | < 1 per week |

---

## 🔐 Sensitive Communication

### **What Goes Where**

- **Public Slack**: Technical discussions, code decisions
- **Private Slack DM**: Personal feedback, sensitive topics
- **GitHub private issue**: Security concerns, privacy matters
- **Email**: Compliance, legal, executive communication

---

## 🤝 New Agent Onboarding

### **Communication Orientation**

1. **Slack basics**
   - Join channels: #general, #development, #standup-async, #questions
   - Intro in #general
   - Observe for 2-3 days

2. **GitHub basics**
   - Watch current issue discussions
   - Comment on issues (read-only first)
   - Ask questions in issues

3. **First week**
   - Post daily standup async
   - Ask questions in #questions
   - No required sync meetings
   - Gradually increase participation

---

## ✅ Communication Checklist

For new agents:

- [ ] Joined `#standup-async` channel
- [ ] Posted first async standup
- [ ] Know how to escalate (DM for urgent, mention for medium)
- [ ] Know decision-making process (Slack for quick, GitHub for complex)
- [ ] Familiar with response time expectations
- [ ] Know optional meeting cadence (none required)

---

## 🔗 Related Documents

- [AGENT_ESCALATION_PATH.md](./AGENT_ESCALATION_PATH.md) - How to escalate
- [GOVERNANCE_RULES.md](../GOVERNANCE/GOVERNANCE_RULES.md) - Communication authority

---

**Owner**: @process-assistant  
**Version**: 2.0 (Async-First, Optional Sync)  
**Status**: ACTIVE  
**Key Change**: No required 10:00 AM UTC standup. Async standup in Slack + GitHub. Optional sync if team needs coordination. Response times: same hour (critical), same day (normal), 24h (standard).
