# AGENT_ESCALATION_PATH.md

**Owner**: @process-assistant (exclusive authority)  
**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE

---

## 🎯 Overview

This document defines clear escalation paths for different types of issues. When blocked or needing decision authority, you know exactly who to contact and how quickly to expect a response.

---

## 🔄 Escalation Decision Tree

### **I Have a Question About...**

#### **Code/Architecture/Design**
```
Backend Developer:
1. Ask in #backend-dev channel
2. If no answer in 2 hours → @tech-lead in thread
3. If architectural: → @software-architect direct message

Frontend Developer:
1. Ask in #frontend-dev channel
2. If no answer in 2 hours → @tech-lead in thread
3. If UX: → @ux-expert direct message
4. If design: → @ui-expert direct message

Expected Response: 4 hours
```

#### **Testing/Quality Issues**
```
Developer:
1. Ask in #qa-team channel
2. If no answer in 2 hours → @qa-engineer in thread
3. If compliance: → @security-engineer

Expected Response: 4 hours
```

#### **Infrastructure/DevOps**
```
Developer:
1. Ask in #devops channel
2. If no answer in 2 hours → @devops-engineer in thread
3. If critical: → @devops-engineer page (Slack urgent mark)

Expected Response: 4 hours (30 min if critical)
```

#### **Security/Compliance**
```
Developer:
1. Ask in channel or DM @security-engineer
2. If urgent: Mark 🔴 URGENT
3. If regulatory: @tech-lead also aware

Expected Response: 2 hours (urgent) / 8 hours (normal)
```

#### **Product/Prioritization**
```
Team:
1. Ask in #general or tag @product-owner
2. If urgent/expensive: Schedule meeting with @product-owner + @tech-lead
3. If customer impact: @product-owner + sales team

Expected Response: 24 hours
```

---

## 🚫 When You're Blocked

**Severity 1 (Blocking All Work)**:
```
1. Post in #general with 🔴 URGENT tag
2. @ mention your direct stakeholder (@tech-lead for tech, @product-owner for product)
3. Expect response in 30 minutes
4. If still blocked after 1 hour → @scrum-master escalates to C-level
```

**Severity 2 (Blocking This Task)**:
```
1. Post in relevant channel (e.g., #backend-dev)
2. @ mention person you need (e.g., @devops-engineer)
3. Expect response in 2-4 hours
4. If blocked > 4 hours → @scrum-master involved
```

**Severity 3 (Nice to Have But Waiting)**:
```
1. Post in relevant channel
2. No @ mention
3. Continue working on alternate task
4. Follow up next day if no response
```

---

## 🎯 Authority Matrix

**Who Decides What?**

| Issue Type | Authority | Escalation | SLA |
|------------|-----------|-----------|-----|
| **Code Review** | Peer + @tech-lead | @software-architect | 8h |
| **Architecture** | @tech-lead | @software-architect | 8h |
| **Security** | @security-engineer | @tech-lead | 4h |
| **Performance** | @tech-lead | @software-architect | 8h |
| **Prioritization** | @product-owner | Leadership | 24h |
| **Timeline** | @scrum-master | @product-owner | 4h |
| **Process Changes** | @process-assistant | Leadership | 48h |
| **Go/No-Go Deploy** | @tech-lead | C-level | 1h |
| **Incident Response** | @devops-engineer / @tech-lead | C-level | Real-time |
| **Resource Allocation** | @product-owner | Leadership | 24h |
| **Tool/Framework** | @tech-lead | @software-architect | 24h |

---

## 📞 Contact Information

| Role | Primary Channel | Escalation | Response SLA |
|------|-----------------|-----------|--------------|
| **@tech-lead** | #general mention | C-level | 2 hours |
| **@product-owner** | @-mention in Slack | Leadership | 4 hours |
| **@scrum-master** | #general mention | @tech-lead | 1 hour |
| **@security-engineer** | DM or #general | @tech-lead | 2 hours |
| **@devops-engineer** | #devops or DM | @tech-lead | 1 hour |
| **@software-architect** | Via @tech-lead | C-level | 8 hours |
| **@process-assistant** | GitHub issue | C-level | 24 hours |

---

## 🚨 Critical Issues - Immediate Escalation

**Page immediately if**:
- Production outage (error rate > 1%)
- Security breach detected
- Data corruption
- Service down > 5 minutes
- Revenue-impacting feature broken

**Page**: @devops-engineer + @tech-lead + @product-owner

**Response Expected**: < 5 minutes

---

## ⏱️ Escalation Timing

### **Fast Track** (same business day)
- Code review decisions: 4-8 hours
- Architecture questions: 4-8 hours
- Technical blockers: 2-4 hours
- Security issues: 2-4 hours
- Incident response: Real-time

### **Normal Track** (within 1-2 days)
- Prioritization questions: 24 hours
- Process questions: 24 hours
- Tool/framework decisions: 24 hours
- Resource allocation: 24 hours
- Customer requests: 24-48 hours

### **Strategic Track** (within 1 week)
- Process changes: 48 hours
- Architecture redesign: 48 hours
- Major tool changes: 48-72 hours
- Roadmap decisions: Weekly in planning

---

## 🔴 Conflicting Guidance

**If two agents give conflicting advice**:

1. **For Technical Issues**:
   - @tech-lead decides
   - If architectural: @software-architect involved
   - Document decision in GitHub

2. **For Product/Prioritization**:
   - @product-owner decides
   - If resource constraint: @scrum-master mediates

3. **For Security Issues**:
   - @security-engineer decides
   - No override unless risk acceptable and documented

4. **For Process Issues**:
   - @process-assistant decides
   - No override (exclusive authority)

---

## 📋 Escalation Checklist

Before escalating:

- [ ] Waited recommended time (2-4 hours for normal issues)
- [ ] Provided clear context (what, why, impact)
- [ ] Explained what you've already tried
- [ ] Mentioned any time constraints
- [ ] Used proper channel (@-mention for urgent)
- [ ] Followed up once if no response

When escalating:

- [ ] Include all context from previous discussion
- [ ] Don't repeat questions already asked
- [ ] Acknowledge any constraints person mentioned
- [ ] Accept their decision gracefully
- [ ] Implement their guidance (don't forum-shop for different answer)

---

## 🔄 Related Workflows

- [AGENT_COMMUNICATION_PROTOCOL.md](./AGENT_COMMUNICATION_PROTOCOL.md) - How to communicate with escalations
- [WORKFLOW_SPRINT_EXECUTION.md](../../CORE_WORKFLOWS/WORKFLOW_SPRINT_EXECUTION.md) - Uses escalation during sprints

---

## ✅ Escalation Path Quick Reference

```
URGENT (< 30 min):
  Backend blocker → @tech-lead
  Security issue → @security-engineer
  Production down → @devops-engineer + @tech-lead
  Can't proceed → @scrum-master

NORMAL (< 4 hours):
  Code review → Peer + @tech-lead
  Architecture → @tech-lead
  Design → @ui-expert or @ux-expert
  Testing → @qa-engineer

PLANNING (< 24 hours):
  Prioritization → @product-owner
  Roadmap → @product-owner + @tech-lead
  Timelines → @scrum-master
  Process → @process-assistant

STRATEGIC (< 48 hours):
  Major decision → @software-architect
  Compliance → @security-engineer + legal
  Resource → Leadership
  Process change → @process-assistant
```

---

**Version History**:
- v1.0 (29 Dec 2025): Initial release - Clear escalation paths with SLAs

**Next Review**: After first month of execution (projected 29 Jan 2026)
