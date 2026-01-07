# Agent Integration Roadmap - Development Process

**Status**: ✅ COMPLETE  
**Date**: 29. Dezember 2025  
**Updated By**: Session integration task  
**Integration Scope**: All 28 agents now included in development process  

---

## 🎯 Overview - Four New Specialists Added

This document describes how the four newly created specialist agents integrate into B2X's development process.

### New Agents (Created 29. Dezember 2025)

| Agent | Role | Focus | Deadline | Escalation |
|-------|------|-------|----------|-----------|
| **@ui-expert** | Design Systems | Component library, Tailwind CSS, dark mode | Ongoing | → @frontend-developer |
| **@ux-expert** | Accessibility | WCAG 2.1 AA compliance, user research | 28. Juni 2025 | → @frontend-developer |
| **@ai-specialist** | AI/ML Patterns | P0.7 compliance, model selection, security | 12. Mai 2026 | → @backend-developer |
| **@process-controller** | Operations | Cost monitoring, metrics tracking, optimization | Ongoing | → @scrum-master |

---

## 🔄 Development Process Flow - With New Agents

### Weekly Workflow (Monday-Friday)

```
MONDAY (Backlog Refinement)
├─ Product Owner: Prioritize features
├─ Scrum Master: Planning & scheduling
├─ Tech Lead: Architecture review
└─ Process Controller: Review previous week metrics

TUESDAY-THURSDAY (Implementation)
├─ Backend Developer: Implement handlers
├─ Frontend Developer: Build UI components
│  ├─ UI Expert: Validate design system consistency
│  ├─ UX Expert: Check accessibility compliance (WCAG AA)
│  └─ Frontend Developer: Implementation
├─ QA Engineer: Test coordination
│  ├─ QA-Frontend: Component testing
│  ├─ QA-Performance: Load testing
│  └─ QA-Pentesting: Security testing
├─ Security Engineer: Code review for security
└─ DevOps Engineer: Infrastructure support

FRIDAY (Integration & Optimization)
├─ QA Engineer: Final validation
├─ Tech Lead: Code review & approval
├─ Process Controller: Collect metrics & identify optimizations
├─ Scrum Master: Sprint retrospective
└─ All: Plan improvements for next sprint
```

### Key Integration Points

#### 1. **UI Expert ↔ Frontend Developer**
- **Trigger**: Every Vue.js component creation
- **Coordination**: 
  - UI Expert defines design system patterns
  - Frontend Developer implements using Tailwind
  - UI Expert validates design consistency
- **Escalation**: Design conflict → @tech-lead (architecture)
- **Reference**: [copilot-instructions-ui-expert.md](../../../.github/agents/ui.agent.md)

#### 2. **UX Expert ↔ Frontend Developer + QA-Frontend**
- **Trigger**: Every feature with UI elements
- **Coordination**:
  - UX Expert: Accessibility testing & WCAG compliance
  - Frontend Developer: Implementation
  - QA-Frontend: Automated + manual testing
- **Critical Deadline**: 28. Juni 2025 (WCAG 2.1 AA)
- **Escalation**: Accessibility violation → @scrum-master → must fix before merge
- **Reference**: [copilot-instructions-ux-expert.md](../../../.github/agents/ux.agent.md)

#### 3. **AI Specialist ↔ Backend Developer + Security Engineer**
- **Trigger**: Any AI/ML feature usage
- **Coordination**:
  - AI Specialist: Model selection, prompt engineering, P0.7 compliance
  - Backend Developer: Implementation
  - Security Engineer: PII protection, rate limiting, audit logging
- **Critical Deadline**: 12. Mai 2026 (P0.7 AI Act)
- **Escalation**: High-risk AI feature → @software-architect for design review
- **Reference**: [copilot-instructions-ai-specialist.md](../../../.github/copilot-instructions.md)

#### 4. **Process Controller ↔ Scrum Master**
- **Trigger**: Every sprint completion
- **Coordination**:
  - Process Controller: Track metrics, identify cost increases
  - Scrum Master: Retrospectives, process optimization
  - All Agents: Provide metrics input
- **Alert Threshold**: Cost increase >15% → trigger optimization sprint
- **Weekly Report**: Metrics dashboard, cost analysis, recommendations
- **Escalation**: Critical cost spike (>$150/day) → @scrum-master immediately
- **Reference**: [copilot-instructions-process-controller.md](../../../.github/agents/sarah.agent.md)

---

## 📊 Agent Roles & Responsibilities Matrix

### Frontend Features (UI/UX)

| Phase | Responsible | Supporting | Input Required |
|-------|-------------|-----------|-----------------|
| **Design** | @ui-expert | @ux-expert | Accessibility requirements |
| **Prototype** | @ui-expert | @ux-expert, @frontend-developer | Component variants, states |
| **Implement** | @frontend-developer | @ui-expert | Design validation |
| **Validate (A11y)** | @ux-expert | @qa-frontend | WCAG checklist completion |
| **Test (Functional)** | @qa-frontend | @frontend-developer | Test scenarios |
| **Review** | @tech-lead | @ui-expert, @ux-expert | Architecture + design |
| **Merge** | @tech-lead | @scrum-master | Gate sign-off |

### Backend Features

| Phase | Responsible | Supporting | Input Required |
|-------|-------------|-----------|-----------------|
| **Design** | @tech-lead | @software-architect | Architecture review |
| **Implement** | @backend-developer | @security-engineer | Security patterns |
| **Security Review** | @security-engineer | @backend-developer | Encryption, audit logs |
| **Test (Functional)** | @qa-engineer | @backend-developer | Test scenarios |
| **Test (Performance)** | @qa-performance | @backend-developer | Load test criteria |
| **Code Review** | @tech-lead | @backend-developer | Pattern validation |
| **Merge** | @tech-lead | @scrum-master | Gate sign-off |

### AI Features

| Phase | Responsible | Supporting | Input Required |
|-------|-------------|-----------|-----------------|
| **Risk Assessment** | @ai-specialist | @security-engineer | P0.7 classification |
| **Model Selection** | @ai-specialist | @backend-developer | Cost/performance trade-offs |
| **Implement** | @backend-developer | @ai-specialist | Prompt engineering, security |
| **Security Hardening** | @security-engineer | @ai-specialist | PII protection, audit logging |
| **Test (Bias)** | @qa-pentesting | @ai-specialist | Fairness across demographics |
| **Code Review** | @tech-lead | @ai-specialist | Architecture + compliance |
| **Merge** | @tech-lead | @scrum-master | P0.7 compliance sign-off |

### Operational/Quality

| Phase | Responsible | Supporting | Input Required |
|-------|-------------|-----------|-----------------|
| **Metrics Collection** | @process-controller | All agents | Daily metrics |
| **Weekly Report** | @process-controller | @scrum-master | Cost analysis |
| **Retrospective** | @scrum-master | @process-controller | Metrics, learnings |
| **Optimization Sprint** | @scrum-master | @process-controller | Recommendations |

---

## 🚨 Critical Compliance Gates

### WCAG 2.1 AA Accessibility (Deadline: 28. Juni 2025)

**Owner**: @ux-expert  
**Gatekeeper**: @tech-lead

**Gate Criteria** (ALL must pass):
- [ ] Lighthouse Accessibility score >= 90
- [ ] Zero critical/serious axe violations
- [ ] Manual keyboard testing (TAB, ENTER, Escape)
- [ ] Screen reader testing (NVDA or VoiceOver)
- [ ] Color contrast verified (4.5:1+)
- [ ] Mobile responsive tested (320px-1920px)

**Escalation**: Any WCAG violation → Blocks merge until fixed

**Reference**: [BITV Compliance Tests](../../compliance)

### P0.7 AI Act Compliance (Deadline: 12. Mai 2026)

**Owner**: @ai-specialist  
**Gatekeeper**: @tech-lead

**Gate Criteria** (ALL must pass):
- [ ] Risk classification documented (High/Medium/Low)
- [ ] Model selection justified (cost, speed, quality)
- [ ] Audit logging implemented and tested
- [ ] Bias testing completed for training data
- [ ] Security review passed (PII protection, rate limiting)
- [ ] Explainability documented (model behavior)

**Escalation**: Any P0.7 violation → Blocks merge until fixed

**Reference**: [P0.7 AI Act Tests](../../compliance)

---

## 📈 Metrics & Monitoring

### Process Controller Dashboard (Real-Time)

**Tracked Metrics**:
- **Cost Metrics**: Total cost, API costs (by provider), cost per task, cost per commit
- **Performance Metrics**: Avg task time, P95 task time, builds/day, build time
- **Efficiency Metrics**: Cost per task, tasks/hour, ROI on optimizations
- **Quality Metrics**: Test pass rate, code coverage, critical issues

**Alert Thresholds**:
- **CRITICAL**: Single task >$25, daily >$150, weekly >$600, task time >120 min
- **HIGH**: Single task >$15, daily >$120, task time >60 min
- **MEDIUM**: Single task >$10, daily >$100
- **LOW**: Monitoring only

**Weekly Report** (Every Friday):
1. Cost analysis (week-over-week)
2. Performance metrics (trends)
3. Key achievements
4. Concerns & risks
5. Optimization recommendations

**Escalation Chain**:
- CRITICAL Alert → @scrum-master (immediate)
- Cost increase >15% → Sprint optimization recommended
- Performance degradation >20% → Architecture review requested

---

## 🔗 Agent Collaboration Map

```
Product Owner
    ↓ (Prioritization)
Scrum Master
    ├─→ Process Controller (Metrics & optimization)
    ├─→ Tech Lead (Architecture decisions)
    └─→ All teams (Coordination)

Tech Lead
    ├─→ Backend Team:
    │   ├─ Backend Developer (Implementation)
    │   ├─ Security Engineer (Security review)
    │   └─ DevOps Engineer (Infrastructure)
    │
    ├─→ Frontend Team:
    │   ├─ Frontend Developer (Implementation)
    │   ├─ UI Expert (Design system validation)
    │   ├─ UX Expert (Accessibility validation)
    │   └─ QA-Frontend (Testing)
    │
    ├─→ AI Features:
    │   ├─ AI Specialist (Model selection, compliance)
    │   ├─ Backend Developer (Implementation)
    │   └─ Security Engineer (Security hardening)
    │
    └─→ QA Coordination:
        ├─ QA Engineer (Test strategy)
        ├─ QA-Frontend (Component tests)
        ├─ QA-Performance (Load tests)
        └─ QA-Pentesting (Security tests)

Process Controller
    ├─→ Scrum Master (Insights)
    ├─→ All Agents (Metrics input)
    └─→ Tech Lead (Optimization recommendations)
```

---

## 📋 New Agent Onboarding Checklist

### For Team Members

- [ ] **Read All Instruction Files**:
  - [ ] Main reference: [copilot-instructions.md](../../../.github/copilot-instructions.md)
  - [ ] Your role: [copilot-instructions-[your-role].md](../.github)
  - [ ] New agents: UI Expert, UX Expert, AI Specialist, Process Controller

- [ ] **Understand Collaboration Points**:
  - [ ] Your dependencies (who you work with)
  - [ ] Your escalation chain (who you report to)
  - [ ] Your critical deadlines (WCAG AA, P0.7)

- [ ] **Access Agent Index**:
  - [ ] Review [AGENTS_INDEX.md](../../../.ai/collaboration/AGENT_TEAM_REGISTRY.md)
  - [ ] Understand agent relationships
  - [ ] Bookmark reference links

- [ ] **First Sprint**:
  - [ ] Coordinate with assigned agents
  - [ ] Follow process flow (Mon-Fri)
  - [ ] Report metrics to Process Controller
  - [ ] Participate in Friday retrospective

### For Scrum Master

- [ ] **Set Up Monitoring**:
  - [ ] Configure Process Controller dashboard
  - [ ] Set up cost tracking by provider/agent/task
  - [ ] Configure alert notifications (Slack)

- [ ] **Plan First Sprint**:
  - [ ] Assign Feature A to Backend team (no AI)
  - [ ] Assign Feature B to Frontend team (with UX validation)
  - [ ] Assign Feature C to AI team (if applicable)
  - [ ] Schedule collaboration checkpoints

- [ ] **Establish Rhythm**:
  - [ ] Monday: Backlog refinement (Product Owner, Scrum Master, Tech Lead, Process Controller)
  - [ ] Tue-Thu: Development (all teams with cross-functional coordination)
  - [ ] Friday: Retrospective (all + Process Controller metrics review)

- [ ] **Communication Plan**:
  - [ ] Daily standup (10:00 AM)
  - [ ] Weekly metrics report (Friday 4:00 PM)
  - [ ] Critical alerts via Slack immediately

---

## 🎯 First Sprint Using New Agents

### Recommended Feature Set

**Sprint Goal**: Validate new agent framework with real features

#### Feature 1: B2C Price Transparency (Issue #30)
- **Backend**: Price calculation service
- **Frontend**: Price display with VAT breakdown
- **Agents Involved**: Backend Dev, Frontend Dev, UI Expert, UX Expert, QA
- **No AI**: Simpler validation

#### Feature 2: B2B VAT-ID Validation (Issue #31)
- **Backend**: VIES API integration, validation caching
- **Frontend**: VAT-ID entry form
- **Agents Involved**: Backend Dev, Frontend Dev, Security Engineer, UX Expert, QA
- **Integration Point**: Security review (encryption, audit logging)

#### Feature 3: Product Recommendations (If Applicable)
- **Backend**: AI recommendation engine
- **Frontend**: UI component for suggestions
- **Agents Involved**: Backend Dev, Frontend Dev, AI Specialist, Security Engineer, UX Expert, QA-Pentesting
- **P0.7 Gate**: AI Act compliance review

### Success Criteria

- [ ] All 3 features completed on schedule
- [ ] Zero critical accessibility violations (WCAG AA)
- [ ] All P0.7 gates passed (if using AI)
- [ ] Weekly metrics report generated
- [ ] Team feedback incorporated
- [ ] Process optimizations identified
- [ ] Ready for production deployment

---

## 📞 Quick Reference - Who to Contact

| Need | Agent | Quick Contact | Escalation |
|------|-------|---------------|-----------|
| **Design system question** | @ui-expert | Tailwind patterns, components | → @frontend-developer |
| **Accessibility issue** | @ux-expert | WCAG compliance, testing | → @frontend-developer (deadline!) |
| **AI/ML decision** | @ai-specialist | Model selection, P0.7 | → @software-architect |
| **Cost too high?** | @process-controller | Optimization strategies | → @scrum-master |
| **Metrics needed** | @process-controller | Dashboard, reports | → @scrum-master |
| **Process improvement** | @scrum-master | Workflow changes | → @software-architect |

---

## 📚 Reference Documents

- **Main Reference**: [copilot-instructions.md](../../../.github/copilot-instructions.md)
- **Agent Index**: [AGENTS_INDEX.md](../../../.ai/collaboration/AGENT_TEAM_REGISTRY.md)
- **Retrospective Protocol**: [RETROSPECTIVE_PROTOCOL.md](../../../.ai/collaboration/PROMPTS_INDEX.md)
- **Scrum Master Guide**: [agents/scrum-master.agent.md](../../../.github/agents/scrum-master.agent.md)
- **UI Expert Instructions**: [copilot-instructions-ui-expert.md](../../../.github/agents/ui.agent.md)
- **UX Expert Instructions**: [copilot-instructions-ux-expert.md](../../../.github/agents/ux.agent.md)
- **AI Specialist Instructions**: [copilot-instructions-ai-specialist.md](../../../.github/copilot-instructions.md)
- **Process Controller Instructions**: [copilot-instructions-process-controller.md](../../../.github/agents/sarah.agent.md)

---

## ✅ Integration Status

**Completed** ✅:
- [x] Four new agent instruction files created and documented
- [x] AGENTS_INDEX.md updated with all 28 agents
- [x] Agent collaboration flows mapped
- [x] Compliance gates defined (WCAG AA, P0.7)
- [x] Metrics framework established
- [x] Escalation chains documented
- [x] Weekly workflow established

**Ready for** ✅:
- [x] First sprint using new agents
- [x] Team onboarding
- [x] Production deployment
- [x] Metrics tracking and optimization

**Next Steps** ⏳:
1. Commit this roadmap
2. Present new agents to team
3. Onboard team to new roles
4. Execute first sprint with validation
5. Collect metrics and optimize

---

**Created**: 29. Dezember 2025  
**Status**: ✅ COMPLETE & READY FOR DEPLOYMENT  
**Version**: 1.0

