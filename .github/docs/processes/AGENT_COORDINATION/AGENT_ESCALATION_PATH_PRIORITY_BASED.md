# AGENT_ESCALATION_PATH - Priority-Based (No Time SLAs)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN PRIORITY ESCALATION  
**Status**: ACTIVE - Replaces response time SLAs

---

## 🎯 Core Principle

**Escalation based on priority and severity, not time. Priority 1 gets immediate attention. Priority 2/3 fit into capacity.**

---

## 📋 Escalation Path

### **Level 1: Direct Ask in Slack**

**When**: "I need help with X"  
**Where**: Relevant Slack channel or DM  
**Who**: The person who can help

**Response**:
- **Priority 1** (blocker, urgent): ASAP (within 1 hour)
- **Priority 2** (important): Same day (within 8 hours)
- **Priority 3** (useful): When capacity (within 48 hours)

**Example**:
```
@backend-dev: "Help! Database connection failing. How do I debug?"

@devops: "Check connection pool. Try this command: [...]"
```

---

### **Level 2: GitHub Mention**

**When**: Question needs documentation or async discussion  
**Where**: GitHub issue or PR comment  
**Who**: The person @mentioned

**Response Time** (by priority):
- **Priority 1**: < 4 hours (blocker)
- **Priority 2**: < 24 hours (important)
- **Priority 3**: < 48 hours (useful)

**Example**:
```
Issue: "How should we handle rate limiting?"

@tech-lead: "See [ADR-012]. Use token bucket. Here's why..."
```

---

### **Level 3: Create Escalation Issue**

**When**: Needs decision from leadership  
**Where**: GitHub issue with `type: escalation` label  
**Who**: @tech-lead or @software-architect

**Response Time** (by priority):
- **Priority 1**: < 8 hours (critical decision)
- **Priority 2**: < 24 hours (important)
- **Priority 3**: < 48 hours (nice to have)

**Example**:
```
Title: "[ESCALATION] Should we migrate to new ORM?"

Description: [Context, tradeoffs, question]
Label: type: escalation, priority: 2

@tech-lead decision needed by: EOD tomorrow
```

---

### **Level 4: Email to Leadership**

**When**: Production emergency + need executive decision  
**Where**: Email to @product-owner + @tech-lead  
**Who**: Leadership team

**Response Time**: < 1 hour (during business hours)

**Example**:
```
Subject: [URGENT] Production Database Corruption

Body: [Details of incident, immediate action taken, decision needed]

Priority: 1 (CRITICAL - system impact)
```

---

## 🎯 Priority Definitions

### **Priority 1 - BLOCKER / URGENT**

**Criteria**:
- Production system down or degraded
- Security issue affecting data
- Cannot proceed without resolution
- Cascading failure risk

**Who escalates**: Anyone on critical path  
**Who handles**: Whoever is assigned + escalate if stuck  
**Response**: ASAP (realtime as much as possible)

**Example Issues**:
- Database down (data loss risk)
- Authentication broken (users locked out)
- API returning 500s for all users
- Security breach discovered
- Critical bug in shipping logic

---

### **Priority 2 - IMPORTANT**

**Criteria**:
- Feature partially broken (workaround exists)
- Performance issue (3x slower than normal)
- Design decision needed (multiple options)
- Timeline critical (affects deadline)

**Who escalates**: Domain expert working on task  
**Who handles**: Relevant agent with some flexibility  
**Response**: Same day (within normal work hours)

**Example Issues**:
- One customer type can't login (others can)
- Dashboard takes 10 seconds to load
- Unclear if we should use pattern A or B
- Need senior review before proceeding

---

### **Priority 3 - USEFUL**

**Criteria**:
- Nice to have information
- Improvement suggestion
- Question about best practice
- Can proceed without answer

**Who escalates**: Anyone (low barrier)  
**Who handles**: When capacity available  
**Response**: Within 48 hours or add to backlog

**Example Issues**:
- "Best way to handle X scenario?"
- "Should we refactor Y module?"
- "Any examples of Z pattern?"
- "Suggestion: add feature W"

---

## 🔄 Escalation Decision Tree

```
START: "I need help"
  ↓
Is this blocking my work RIGHT NOW?
  ├─ YES → Priority 1 (escalate immediately via Slack/DM)
  └─ NO → Continue
  
Can I work around it for today?
  ├─ YES → Priority 3 (ask in GitHub when convenient)
  └─ NO → Priority 2 (mention in Slack/GitHub same day)

Is this a production issue?
  ├─ YES → Priority 1 (page on-call if after hours)
  └─ NO → Use criteria above

Need executive decision?
  ├─ YES → Create escalation issue + email if urgent
  └─ NO → Use standard escalation path
```

---

## 📋 Who Escalates What

### **Architecture Questions**

→ Escalate to: @tech-lead  
Expected response: 24 hours  
If critical: Mention in #development + email

### **Security/Compliance**

→ Escalate to: @security-engineer  
Expected response: 24 hours  
If critical: Page on-call

### **DevOps/Infrastructure**

→ Escalate to: @devops-engineer  
Expected response: 2 hours (if impacting service)  
If critical: Page on-call

### **Product/Prioritization**

→ Escalate to: @product-owner  
Expected response: 24 hours  
If critical: Email + mention in #general

### **Process/Governance**

→ Escalate to: @process-assistant  
Expected response: 24 hours  
If critical: Create issue + email

### **Technical Decision**

→ Escalate to: @software-architect  
Expected response: 24 hours  
If blocking: Mention in #development

---

## 🚀 Escalation Process

### **Step 1: Attempt Resolution Yourself** (Optional)

Before escalating, ask yourself:
- Can I find answer in docs?
- Can I try a solution?
- Can I ask a peer?

(Don't waste time - if stuck in 15 min → escalate)

---

### **Step 2: Choose Escalation Method**

**Quick answer needed?** → Slack mention  
**Documentation needed?** → GitHub issue  
**Decision required?** → Escalation issue  
**Emergency?** → Email + page on-call

---

### **Step 3: Escalate with Context**

Provide:
- [ ] What you're trying to do
- [ ] What you've tried
- [ ] What went wrong
- [ ] Why you need help
- [ ] When you need answer

**Good escalation**:
```
@tech-lead: "Implementing pagination for product list. 
Should we use offset/limit or cursor-based? 
Pros/cons of each? Needed by EOD for PR review."
```

**Bad escalation**:
```
"How do pagination?"
```

---

### **Step 4: Responder Provides Answer**

Responder should:
- [ ] Understand the context
- [ ] Provide clear answer
- [ ] Explain reasoning
- [ ] Document in issue (so others learn)

---

### **Step 5: Close Loop**

Original asker:
- [ ] Thanks responder
- [ ] Document decision
- [ ] Move forward
- [ ] Close issue (if created)

---

## 🔗 Special Escalation Cases

### **When Responder Doesn't Know**

"I'm not sure. Let me escalate to @X who knows better."

→ They escalate on your behalf (no ping-pong)

### **When Decision is Needed But Urgent**

1. Get @tech-lead or @software-architect input ASAP
2. If unavailable, @product-owner makes call
3. Document decision, implement
4. Retrospective discussion later

### **When Multiple Agents Disagree**

1. Discuss in GitHub issue (all perspectives)
2. @tech-lead or @software-architect decides
3. Document decision rationale
4. Move forward

---

## 📊 Escalation Metrics

Track these:

| Metric | Target |
|--------|--------|
| **P1 resolution time** | < 1 hour |
| **P2 resolution time** | < 24 hours |
| **P3 resolution time** | < 48 hours |
| **Escalation frequency** | < 5 per week |
| **Blocked time** | < 2 hours per sprint |

---

## ✅ Escalation Checklist

When escalating:

- [ ] Documented what I'm trying to do
- [ ] Explained what I've tried
- [ ] Stated priority level (1/2/3)
- [ ] Provided context needed to answer
- [ ] Mentioned response deadline (if applicable)
- [ ] Used appropriate channel (Slack/GitHub/Email)

---

## 🛑 What NOT to Escalate

**Don't escalate if**:
- You can find answer in docs/GitHub
- You haven't tried basic troubleshooting
- You haven't asked a peer
- It's not actually blocking you (just curious)

**Instead**: Search docs → Ask peer → Try solution → THEN escalate if stuck

---

## 🔐 After-Hours Escalation

### **When It's 2 AM and You Need Help**

**Criteria**: Only if production is down

**Process**:
1. **Page on-call**: Use PagerDuty / incident channel
2. **Self-remediate**: Try fixes while waiting
3. **Document**: Capture what happened for morning retrospective

**What's NOT production emergency**:
- I want to work but unsure how
- Question about best practice
- Feature request
- Non-urgent bug

(These wait until business hours)

---

## 🤝 Escalation Culture

### **Good Escalation**

- Ask clearly and early (don't wait until stuck)
- Provide context (don't make others guess)
- Respect responder's time (write well)
- Accept answer (don't argue decision)
- Help others learn (document resolution)

### **Bad Escalation**

- Vague questions ("does this work?")
- No context ("help plz")
- Ignoring documented answers
- Escalating without trying yourself
- Assuming realtime response

---

## 🔗 Related Documents

- [AGENT_COMMUNICATION_PROTOCOL_ASYNC.md](./AGENT_COMMUNICATION_PROTOCOL_ASYNC.md) - How agents communicate
- [GOVERNANCE_RULES.md](../GOVERNANCE/GOVERNANCE_RULES.md) - Authority structure

---

**Owner**: @process-assistant  
**Version**: 2.0 (Priority-Based, No Time SLAs)  
**Status**: ACTIVE  
**Key Change**: Escalation by priority (1/2/3), not response time SLAs. Priority 1 = ASAP, Priority 2 = same day, Priority 3 = 48 hours. Severity determines urgency, not clock.
