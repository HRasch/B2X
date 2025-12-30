# 🔐 Process Assistant - Quick Reference Guide

**For All Agents** | Quick answers to common questions  
**Created**: 29. Dezember 2025  
**Authority**: @process-assistant (EXCLUSIVE)

---

## ❓ Quick Q&A

### "Can I modify my own instructions?"
**NO.** Request changes via GitHub issue instead.
→ [GOVERNANCE_RULES.md](./GOVERNANCE/GOVERNANCE_RULES.md)

### "How do I request an instruction change?"
File GitHub issue:
```
Title: @process-assistant instruction-request: [brief description]
Body: 
  - File: [exact path]
  - Current: [what it says]
  - Desired: [what you want]
  - Rationale: [why]
  - Impact: [who else affected]
Label: process-change-request
```

### "Can I modify workflow files?"
**NO.** Only @process-assistant can. Request via issue.
→ [GOVERNANCE_RULES.md](./GOVERNANCE/GOVERNANCE_RULES.md)

### "Am I allowed to [action]?"
Check [PERMISSIONS_MATRIX.md](./GOVERNANCE/PERMISSIONS_MATRIX.md)

### "Who should I escalate to?"
See [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md)

### "What if I disagree with a rule?"
Escalate per [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md)

### "What if there's a conflict between workflow and instructions?"
Report to @process-assistant. They will fix it.

### "What happens if I violate these rules?"
- First: Change reverted + notification
- Second: Escalate to @tech-lead
- Third: Escalate to leadership

---

## 📋 The Core Rules

### Rule 1: Only @process-assistant Modifies Protected Files
```
Protected:
  .github/docs/processes/
  .github/agents/
  .github/copilot-instructions-*.md
  .github/GOVERNANCE.md
```

### Rule 2: Formal Change Request Process
```
Want change? → File issue → @process-assistant reviews → Decision → Change made (or rejected)
```

### Rule 3: Workflows & Instructions Must Match
```
Workflow says X → Instructions must say X (not Y!)
```

### Rule 4: All Changes Documented
```
Version number bumped
Rationale in commit message
Change logged
All related docs updated
```

---

## 🚨 Protected Files (Don't Touch!)

```
❌ DO NOT COMMIT TO:
  - .github/docs/processes/
  - .github/agents/
  - .github/copilot-instructions-*.md
  - .github/GOVERNANCE.md

✅ DO THIS INSTEAD:
  - File GitHub issue
  - @process-assistant reviews
  - @process-assistant makes change
```

---

## 📞 How to Get Things Done

| Want to... | Do This | Reference |
|------------|--------|-----------|
| Modify my instructions | File GitHub issue (process-change-request) | [GOVERNANCE_RULES.md](./GOVERNANCE/GOVERNANCE_RULES.md) |
| Understand my permissions | Check PERMISSIONS_MATRIX.md | [PERMISSIONS_MATRIX.md](./GOVERNANCE/PERMISSIONS_MATRIX.md) |
| Know who to escalate to | See AGENT_ESCALATION_PATH.md | [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md) |
| Report a conflict | Message @process-assistant | [process-assistant.agent.md](../../agents/process-assistant.agent.md) |
| Disagree with policy | Escalate per AGENT_ESCALATION_PATH.md | [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md) |

---

## 🎯 What's Off-Limits for Other Agents

### ❌ These Require @process-assistant:
- Modifying workflow definitions
- Modifying agent instructions (including own!)
- Modifying governance rules
- Changing protected files
- Creating new agents
- Modifying GOVERNANCE.md

### ✅ These You CAN Do:
- Write feature code
- Create tests
- Write feature documentation
- Request instruction changes
- Implement features
- Follow documented workflows

---

## 📊 Compliance at a Glance

```
Rule 1: Don't modify protected files
        Violation = Revert + notification
        
Rule 2: Use formal process for changes
        Violation = Change rejected, resubmit formally
        
Rule 3: Follow documented workflows
        Question = Check workflow document
        
Rule 4: Accept @process-assistant decisions
        Appeal = Escalate per AGENT_ESCALATION_PATH.md
```

---

## 🚀 Getting Started (RIGHT NOW)

### Step 1: Read These (MANDATORY)
1. [GOVERNANCE_RULES.md](./GOVERNANCE/GOVERNANCE_RULES.md) - 5 min
2. [PERMISSIONS_MATRIX.md](./GOVERNANCE/PERMISSIONS_MATRIX.md) - 3 min

### Step 2: Understand Your Role
- Find yourself in PERMISSIONS_MATRIX.md
- Verify what you can/cannot do
- Note who to escalate to

### Step 3: If You Need a Change
- Check if you have permission
- If not → File GitHub issue with @process-assistant
- Provide rationale + impact
- Wait for approval

---

## 📖 Full Documentation

| Document | Purpose | Read Time |
|----------|---------|-----------|
| [README.md](./README.md) | Index of all processes | 5 min |
| [GOVERNANCE_RULES.md](./GOVERNANCE/GOVERNANCE_RULES.md) | Rules everyone must follow | 10 min |
| [PERMISSIONS_MATRIX.md](./GOVERNANCE/PERMISSIONS_MATRIX.md) | Who can do what | 5 min |
| [ENFORCEMENT_RULES_AND_MONITORING.md](./GOVERNANCE/ENFORCEMENT_RULES_AND_MONITORING.md) | How violations are handled | 10 min |
| [process-assistant.agent.md](../../agents/process-assistant.agent.md) | Full agent definition | 15 min |

---

## ⚠️ CRITICAL REMINDERS

```
🔴 HIGH IMPACT:
   - Only @process-assistant modifies protected files
   - No exceptions without explicit approval
   - Violations are reverted immediately

🟡 IMPORTANT:
   - Always check PERMISSIONS_MATRIX.md first
   - Use formal process for changes
   - Document your decisions

🟢 HELPFUL:
   - Ask if you're unsure
   - Escalate appropriately
   - Follow documented workflows
```

---

## 🆘 Emergency Contact

**For urgent questions**: Tag @process-assistant on GitHub issue

**For violations**: Change will be reverted automatically + notification sent

**For appeals**: Escalate per AGENT_ESCALATION_PATH.md

---

## ✅ Checklist Before You Code

- [ ] Checked PERMISSIONS_MATRIX.md for this action?
- [ ] Do I have permission?
- [ ] If not, filed formal request?
- [ ] Waiting for approval?
- [ ] Ready to implement?

---

## 📅 Timeline

```
29. Dezember: Rules published, monitoring begins
Dec 30-Jan 5: Grace period (violations reported, not reverted)
Jan 6+: Full enforcement (violations reverted immediately)
```

---

## 🎯 Goal

**Clear governance, no conflicts, no confusion.**

Everyone knows:
- ✅ What they can do
- ✅ How to request changes
- ✅ Who makes decisions
- ✅ How violations are handled

---

**Status**: ACTIVE  
**Effective**: 29. Dezember 2025  
**Authority**: @process-assistant (EXCLUSIVE)

Questions? → Check the relevant document above or ask @process-assistant
