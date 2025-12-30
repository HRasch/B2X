# WORKFLOW_BACKLOG_REFINEMENT - Async Continuous (No Friday Schedule)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN CONTINUOUS INTAKE  
**Status**: ACTIVE - Replaces Friday afternoon cadence

---

## 🎯 Core Principle

**Backlog refinement happens continuously, not on a schedule. When a new item is ready, it gets refined immediately by the right agents.**

---

## 📋 Backlog Refinement Flow

### **Phase 1: Intake** (Continuous - When Item Arrives)

**Trigger**: 
- Product Owner creates new GitHub issue
- Customer/stakeholder request comes in
- Tech debt discovered
- Bug reported

**Action** (@product-owner):
1. Write clear issue description
2. Include acceptance criteria
3. Tag with priority label
4. Assign to backlog project

**Duration**: 5-10 minutes (happens immediately when issue created)

---

### **Phase 2: Classification** (Within 24 Hours)

**Owner**: @product-owner + relevant domain agent  
**Process**:

1. **Assign Category**:
   - Feature (product enhancement)
   - Bug (defect)
   - Tech Debt (refactoring, testing, documentation)
   - Research (investigation needed)

2. **Initial Sizing**:
   - Small (1-3 story points)
   - Medium (5-8 story points)
   - Large (13+ story points - may need breakdown)

3. **Identify Risks**:
   - Dependencies on other work?
   - External API integration?
   - Security implications?
   - Compliance questions?

**Duration**: 30 min - 1 hour (can be done async)

**Output**: Issue labeled, estimated, dependencies noted

---

### **Phase 3: Stakeholder Review** (As Needed - Not Scheduled)

**Trigger**: Only if complex, risky, or involving external parties

**Participants**:
- @tech-lead (architecture review if complex)
- @security-engineer (if security implications)
- @product-owner (always involved)
- Domain expert (if specialized)

**Process**:
1. Async review in GitHub issue comments
2. Questions asked, answered
3. Concerns documented
4. Risks mitigated or accepted

**Duration**: Varies (1-2 hours typical)

**Exit**: Issue ready for development (or marked "waiting for clarification")

---

### **Phase 4: Ready for Development** (Continuous)

**Definition of Ready**:
- [ ] Clear description
- [ ] Acceptance criteria defined
- [ ] Story points assigned
- [ ] No blockers
- [ ] Dependencies identified
- [ ] Technical approach clear (no research needed)

**When Ready**:
1. Move to "Ready for Development" column in GitHub project
2. Add label: "ready-for-dev"
3. Notify team (optional announcement in Slack)

**Duration**: Continuous - as soon as above criteria met

---

## 🚀 Refinement Process (No Schedule)

### **How Refinement Actually Happens**

**OLD MODEL** (Friday afternoon 90 minutes):
- Fixed time every Friday 2-3:30 PM UTC
- All agents present
- Batch process: handle 5-10 items
- Often incomplete (run out of time)

**NEW MODEL** (continuous async):
- Item ready → @product-owner kicks off refinement
- Async review in GitHub issue
- Domain agent (backend/frontend/security) adds input when available
- Happens at normal work pace (no special meeting)
- Moves through phases as stakeholders engage

---

### **Example Refinement Timeline**

**Monday 10:00 AM**: 
- Customer requests feature: "Add bulk email export"
- @product-owner creates GitHub issue #500

**Monday 10:15 AM**:
- @product-owner fills in acceptance criteria
- Tags with "feature" + "medium-priority"
- Initial estimate: 8 story points

**Monday 2:00 PM** (during normal work):
- @backend-developer reviews, asks clarification: "What's max export size?"
- @frontend-developer reviews, sketches UI approach in comments

**Tuesday 9:00 AM**:
- @product-owner answers: "Max 100k records"
- @security-engineer chimes in: "Need rate limiting + audit logging"

**Tuesday 10:30 AM**:
- @tech-lead reviews: "Can use existing export service"
- Adds "no-spike" comment: This is straightforward

**Tuesday 11:00 AM**:
- @product-owner moves to "Ready for Development"
- Labels "ready-for-dev"
- Can be picked up immediately by available developer

**Total time**: ~24 hours (but mostly async, no meeting required)

---

## 📊 Backlog Refinement Metrics

Track these continuously (no weekly review needed):

| Metric | Target | How Tracked |
|--------|--------|-------------|
| **Issues Ready** | 20%+ of backlog | GitHub project board |
| **Avg Refinement Time** | < 24 hours | GitHub issue timestamps |
| **Blocker Rate** | < 5% items stuck | Blocked label count |
| **Estimation Accuracy** | 80%+ | Compare actual vs estimate |
| **Dependency Issues** | < 3 active | GitHub dependency tracking |

---

## 🎯 Checkpoints (Not Scheduled)

### **When to Pause Refinement**

**Checkpoint 1**: Before each iteration starts
- [ ] Do we have 1.5x velocity worth of ready items?
- [ ] Example: If velocity is 21 pts, have 30+ pts ready?
- [ ] If NO → Add more items to refinement

**Checkpoint 2**: Mid-iteration (when developers idle)
- [ ] Are developers blocked waiting for requirements?
- [ ] If YES → Priority refinement immediately

**Checkpoint 3**: Retrospective
- [ ] Did we estimate accurately?
- [ ] Were dependencies clear?
- [ ] What can improve next iteration?

---

## 📋 Backlog States

```
NEW (Created)
  ↓
CLASSIFIED (Category, estimate, risks identified)
  ↓
STAKEHOLDER REVIEW (if needed)
  ↓
READY FOR DEVELOPMENT
  ↓
SELECTED FOR ITERATION (picked by developers)
  ↓
IN DEVELOPMENT
  ↓
DONE
```

---

## 🔄 Refinement by Issue Type

### **Feature (Large)**
- 3 phases: Product Description → Technical Approach → Ready
- May need spike (research) first
- Typical: 2-4 days refinement

### **Feature (Small)**
- 2 phases: Product Description → Ready
- Usually clear technical approach
- Typical: 4-8 hours refinement

### **Bug**
- 2 phases: Reproduction Steps → Ready
- Often clear fix path
- Typical: 2-4 hours refinement

### **Tech Debt**
- 1 phase: Justification → Ready
- @tech-lead approval often needed
- Typical: 4-8 hours refinement

---

## 🚫 What Blocks Refinement

**Cannot proceed if**:
- Missing business context (what is this really for?)
- No acceptance criteria
- Unclear scope (too big, too vague)
- Dependency not ready (waiting for other work)
- Security implications not assessed

**How to unblock**:
- Spike (research task): 1-2 days investigation
- Break down: Split into smaller pieces
- Dependency: Link and wait or sequence differently

---

## 📞 Refinement Roles

| Role | Responsibility |
|------|-----------------|
| **@product-owner** | Create issue, acceptance criteria, prioritization, stakeholder coordination |
| **@tech-lead** | Architecture review, complexity assessment, spike recommendations |
| **@backend-developer** | Technical feasibility, estimation, implementation approach |
| **@frontend-developer** | UI/UX approach, scope assessment, estimation |
| **@security-engineer** | Security review, compliance check, encryption needs |
| **@qa-engineer** | Test coverage needs, acceptance criteria clarity |

---

## ✅ Definition of Ready (Pre-Development)

Item is ready when:

- [ ] **Product Owner**: "I understand what we're building"
- [ ] **Backend Dev**: "I can start implementing" (if backend work)
- [ ] **Frontend Dev**: "I can start building UI" (if frontend work)
- [ ] **Tech Lead**: "Architecture approach is clear"
- [ ] **Security Engineer**: "No security concerns" (or mitigated)
- [ ] **QA Engineer**: "Acceptance criteria is testable"

---

## 🔗 Related Documents

- [WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md](./WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md) - When refined items get selected
- [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md) - If refinement blocked

---

**Owner**: @process-assistant  
**Version**: 2.0 (Continuous Async, No Friday Schedule)  
**Status**: ACTIVE  
**Key Change**: Refinement happens continuously as items arrive, not in scheduled Friday meetings. Async review in GitHub comments. Moves to "Ready" when Definition of Ready met.
