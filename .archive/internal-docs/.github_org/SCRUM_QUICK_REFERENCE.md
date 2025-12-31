# 🎯 B2Connect Customized Scrum Process - Quick Reference

**Version**: 1.0  
**Date**: 29. Dezember 2025  
**Model**: Event-Driven Sprints (~50 story points, no time frames)

---

## 📚 Documentation Overview

### **Core Process Document**
- [SCRUM_PROCESS_CUSTOMIZED.md](./SCRUM_PROCESS_CUSTOMIZED.md) - Complete process definition, roles, workflow

### **Role-Specific Instructions**
- [team-assistant.agent.md](./agents/team-assistant.agent.md) - Sprint coordination, feedback collection, token tracking
- [product-owner-instructions.md](./agents/product-owner-instructions.md) - Sprint planning, feedback filtering, PR merging
- [process-controller-instructions.md](./agents/process-controller-instructions.md) - Metrics, reporting, cost tracking

---

## 🔄 Sprint Workflow (High Level)

```
SPRINT N STARTS
    ↓
[1] BACKLOG REFINEMENT (Team)
    - Discuss unrefined issues
    - Define acceptance criteria
    - Estimate story points
    - Result: Issues "Refined"
    ↓
[2] SPRINT PLANNING (@product-owner)
    - Select ~50 story points
    - Move to "Ready" status
    - Announce sprint start
    ↓
[3] ARCHITECTURE REVIEW (@software-architect, @tech-lead)
    - Review specifications
    - Approve or request changes
    - Result: Issues approved to start
    ↓
[4] PARALLEL DEVELOPMENT (Developers, QA, Docs)
    - Developers implement features
    - QA tests simultaneously
    - Documentation written
    - Code reviewed by peers
    - Result: PR created when feature complete
    ↓
[5] STAKEHOLDER REVIEW (All relevant parties)
    - @ui-expert, @ux-expert (frontend)
    - @ai-specialist (AI features)
    - @legal-compliance (legal)
    - @security-engineer (security)
    - @devops-engineer (ops)
    - @tech-lead (architecture)
    - Result: Feedback posted to GitHub
    ↓
[6] FEEDBACK PROCESSING (@product-owner)
    - IN-SCOPE: Update requirements, restart dev loop
    - OUT-OF-SCOPE: Create new issues, link back
    - Result: Issue ready for final review OR changes needed
    ↓
[7] FINAL QA REVIEW (@qa-review)
    - Verify acceptance criteria met
    - Verify coverage 80%+
    - Verify documentation complete
    - Verify accessibility (if UI)
    - Result: "✅ APPROVED FOR MERGE"
    ↓
[8] MERGE (@product-owner)
    - Merge PR to main
    - Close issue
    - Post completion
    - Result: Issue status = "Done"
    ↓
REPEAT [4-8] FOR EACH ISSUE until 50+ points done
    ↓
SPRINT COMPLETE
    ↓
[9] METRICS REPORT (@process-controller)
    - Compile all sprint metrics
    - Calculate velocity, costs
    - Analyze trends
    - Post final report
    ↓
SPRINT N+1 STARTS IMMEDIATELY (No waiting for calendar)
```

---

## 👥 Role Matrix

| Role | Responsibility | Authority |
|------|-----------------|-----------|
| **@product-owner** | Sprint planning, feedback filtering, PR merging | Final scope decisions |
| **@software-architect** | Architecture review before development | Approve/reject design |
| **@tech-lead** | Technical review, code review | Approve/reject implementation |
| **@developers** | Feature implementation, testing | Daily decisions on approach |
| **@qa-engineer** | Testing, quality validation | Identify issues |
| **@qa-review** | Final quality gate | Approve/reject for merge |
| **@ui-expert, @ux-expert** | UI/UX design, feedback | Design approval |
| **@ai-specialist** | AI/ML implementation, feedback | Algorithm approval |
| **@legal-compliance** | Legal compliance check | Flag legal risks |
| **@security-engineer** | Security review | Flag security risks |
| **@devops-engineer** | Deployment, ops review | Infrastructure approval |
| **@team-assistant** | Feedback collection, status tracking, token logging | Process facilitation |
| **@process-controller** | Metrics, reporting, cost tracking | Final sprint reports |

---

## 📊 Key Differences from Calendar-Based Sprints

| Traditional Sprint | B2Connect Custom Sprint |
|-------------------|------------------------|
| Fixed dates (2 weeks) | Event-driven (complete when done) |
| Daily standups | Feedback-driven updates |
| Time-based metrics | Velocity + quality + cost |
| Hard sprint deadline | Soft 50-point target |
| Planned interruptions | Rapid blocker removal |

---

## 🎯 Success Criteria per Sprint

**Velocity**:
- Target: ~50 story points
- Acceptable range: 40-60 points

**Quality**:
- Code coverage: 80%+
- Test pass rate: 100%
- Post-merge regressions: 0

**Cost**:
- Cost per story point tracked
- Trend: Should be stable or decreasing

**Team**:
- All team members contributing
- Feedback iterations <2 per issue (ideal 0-1)
- Cycle time <3 days average

---

## 📋 Daily Responsibilities

### **For @product-owner**:
```
✅ Review feedback comments on issues
✅ Filter in-scope vs out-of-scope
✅ Update requirements if needed
✅ Merge approved PRs
✅ Celebrate completions
```

### **For @developers**:
```
✅ Build features per acceptance criteria
✅ Write tests (80%+ coverage)
✅ Ensure builds pass
✅ Create PR when ready
✅ Address review feedback
```

### **For @qa-engineer**:
```
✅ Write test cases from requirements
✅ Test features as developed
✅ Report bugs to developers
✅ Verify fixes
✅ Sign off when feature complete
```

### **For @team-assistant**:
```
✅ Facilitate backlog refinement
✅ Collect feedback on issues
✅ Update issue status on GitHub
✅ Track AI token usage
✅ Prepare metrics for sprint report
```

### **For @process-controller**:
```
✅ Aggregate sprint metrics daily
✅ Track token usage per issue
✅ Calculate cost per story point
✅ Analyze trends
✅ Create final sprint report
```

---

## 🚨 Critical Feedback Rules

When stakeholders provide feedback:

```
IN-SCOPE = Directly targets acceptance criteria
Examples:
  - "Validation error message is wrong"
  - "German locale not working correctly"
  - "Missing unit test for edge case"
  
OUT-OF-SCOPE = Additional features/changes not in acceptance criteria
Examples:
  - "Can we add dark mode too?"
  - "Let's also support French"
  - "Change the color scheme"
  
@product-owner ACTION:
  IN-SCOPE: Update requirements, restart development
  OUT-OF-SCOPE: Create new issue, link back, defer to next sprint
```

---

## 📈 Metrics Tracked per Sprint

```
Velocity Metrics:
├─ Story points completed
├─ Issues finished
├─ Cycle time per issue
└─ Issues per developer

Quality Metrics:
├─ Code coverage %
├─ Tests passing %
├─ Regressions (0 = success)
└─ Quality grade (A/B/C)

Cost Metrics:
├─ Total AI tokens used
├─ Cost per story point
├─ Cost per issue
└─ Cost trend

Team Metrics:
├─ Issues per developer
├─ Story points per developer
├─ Code review time
└─ Feedback iterations
```

---

## 🔗 Key Documents (Full List)

| Document | Purpose |
|----------|---------|
| [SCRUM_PROCESS_CUSTOMIZED.md](./SCRUM_PROCESS_CUSTOMIZED.md) | Complete process definition |
| [team-assistant.agent.md](./agents/team-assistant.agent.md) | Team coordination agent |
| [product-owner-instructions.md](./agents/product-owner-instructions.md) | PO responsibilities |
| [process-controller-instructions.md](./agents/process-controller-instructions.md) | Metrics & reporting |
| [copilot-instructions.md](./../copilot-instructions.md) | General coding standards |
| [copilot-instructions-backend.md](./../copilot-instructions-backend.md) | Backend patterns |
| [copilot-instructions-frontend.md](./../copilot-instructions-frontend.md) | Frontend patterns |

---

## ⚡ Quick Start for New Sprints

1. **@product-owner**: "Next sprint starting"
2. **@team-assistant**: Facilitates backlog refinement
3. **@product-owner**: Selects ~50 points, moves to "Ready"
4. **@software-architect, @tech-lead**: Review architecture
5. **@developers**: Start development
6. **@qa-engineer**: Start testing
7. **@team-assistant**: Collect feedback when ready
8. **@product-owner**: Filter feedback, keep issue moving
9. **@qa-review**: Final quality gate
10. **@product-owner**: Merge when approved
11. **Repeat** until 50+ points done
12. **@process-controller**: Create final sprint report
13. **Next sprint starts immediately** (no waiting!)

---

## 🎓 Training for New Team Members

**New Developer?**
1. Read [SCRUM_PROCESS_CUSTOMIZED.md](./SCRUM_PROCESS_CUSTOMIZED.md) - Workflow section
2. Read [copilot-instructions-backend.md](./../copilot-instructions-backend.md) or frontend equivalent
3. Start with "Ready" issue, follow the development process
4. Ask @tech-lead for architecture review

**New Product Owner?**
1. Read [SCRUM_PROCESS_CUSTOMIZED.md](./SCRUM_PROCESS_CUSTOMIZED.md) - Sprint Overview & Workflow
2. Read [product-owner-instructions.md](./agents/product-owner-instructions.md)
3. Start with sprint planning
4. Practice feedback filtering (in-scope vs out-of-scope)

**New Team Assistant?**
1. Read [SCRUM_PROCESS_CUSTOMIZED.md](./SCRUM_PROCESS_CUSTOMIZED.md) - Overview
2. Read [team-assistant.agent.md](./agents/team-assistant.agent.md)
3. First sprint: Observe, then facilitate next sprint
4. Practice token tracking and metrics collection

---

## 📞 Contact & Support

| Question | Contact |
|----------|---------|
| Process questions | @scrum-master |
| Scope decisions | @product-owner |
| Architecture questions | @software-architect |
| Technical implementation | @tech-lead |
| Metrics & reporting | @process-controller |
| Team coordination | @team-assistant |

---

## 🚀 Success Indicators

A successful sprint should have:

```
✅ 40-60 story points completed
✅ 80%+ code coverage
✅ 100% test pass rate
✅ 0 post-merge regressions
✅ <3 days average cycle time
✅ All feedback processed quickly
✅ All acceptance criteria met
✅ Team satisfied with pace
✅ Cost per point tracked
✅ No major blockers unresolved
```

---

**Last Updated**: 29. Dezember 2025  
**Process Owner**: @product-owner + @team-assistant  
**For questions**: See Contact & Support section above

