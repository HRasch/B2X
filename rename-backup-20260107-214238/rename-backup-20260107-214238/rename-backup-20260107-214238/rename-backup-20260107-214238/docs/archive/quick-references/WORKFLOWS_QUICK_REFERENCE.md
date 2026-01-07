# B2X Workflows - Quick Reference Card (PHASE 4)

**Print & Bookmark This Page**  
**Last Updated**: 29. Dezember 2025  
**Model**: VELOCITY-BASED (No Timeboxes)

---

## 🚀 5-Minute Overview (Phase 4 - Velocity Model)

### **Iteration Cycle** (Continuous, Story-Point Driven)

```
VELOCITY-BASED FLOW (No Fixed Schedule)
──────────────────────────────────────────
Backlog Intake    Development         Review & Deploy
(Continuous)      (Async)             (Async)
↓                 ↓                   ↓
Story points      Code Review         Code green
refined ASAP      (async, P1 ASAP)   → Deploy anytime
When ready        Build passes        Monitoring validates
                  Tests 100% green    Rollback ready
                  
Iteration Ends: Velocity ≈ baseline ±20%
(Not calendar-based, metric-driven trigger)
```

### **Authority at a Glance**

| Decision | Owner | Escalate To |
|----------|-------|-------------|
| **Code Quality** | @tech-lead | @tech-lead approval needed |
| **Architecture** | @tech-lead | @software-architect if blocked |
| **Security** | @security-engineer | Page on-call if breach/critical |
| **Product Decisions** | @product-owner | Leadership if conflicting priorities |
| **Process/Workflows** | @process-assistant | EXCLUSIVE - no bypasses allowed |
| **Incident Response** | @devops-engineer | @tech-lead if critical |

---

## 📋 Async Standup (Continuous, No Fixed Time)

**Format** (Post in Slack #standup channel):
- Story Points Completed: [N points]
- Current Work: [Feature/Issue #XX]
- Blockers: [Any impediments?]
- Help Needed: [@mention specific agent]

**Response Times** (Priority-based, not time-SLA):
- 🟡 **P1 (Blocking Release)** → Immediate (same hour)
- 🟡 **P2 (Blocking Feature)** → Same day (by EOD)
- 🟢 **P3 (Standard)** → 48 hours
- 🔵 **Async** → No urgent escalation required

**No Fixed Meeting Time** – Async first, optional sync if team needs coordination

---

## 💻 Code Review Checklist

**Before creating PR**:
- [ ] `dotnet build` succeeds
- [ ] `dotnet test` passes (coverage ≥80%)
- [ ] No security violations
- [ ] Documentation updated

**PR Review SLAs**:
1. Peer review (4h) - Code quality
2. Architecture (8h) - Wolverine pattern, design
3. Security (8h) - Encryption, auth, PII protection
4. QA (8h) - Test coverage, test cases

**All 4 must approve before merge.**

---

## 🐛 Incident Response (If Production Is Down)

**CRITICAL Issue** (Outage, Data Loss, Breach):
1. **Immediate** (< 5 min): Page @devops-engineer + @tech-lead
2. **Mitigation** (< 15 min): Start incident response
3. **Resolution** (< 30 min): Get system back online
4. **War Room** (< 1h after recovery): Analyze root cause
5. **Action Items** (< 24h): Prevent recurrence

**Escalation Channel**: Slack #incident-[id]

---

## 📅 Important Dates

| Event | When | Duration |
|-------|------|----------|
| **Daily Standup** | 10:00 AM UTC | 15 min |
| **Code Review** | Within 4-8h of PR | Varies |
| **Backlog Refinement** | Friday afternoon | 90 min |
| **Sprint Retrospective** | Friday 4-5 PM | 90 min |
| **Deployment Window** | Tue-Thu 2-4 PM UTC | 1-2h |
| **NOT Deployment Times** | Mon morning, Fri afternoon, weekends | N/A |

---

## 🆘 Need Help? Quick Contact Matrix

| Need | Ask | Where | Respond |
|------|-----|-------|---------|
| **Sprint question** | @scrum-master | Slack | 1h |
| **Code review blocked** | @tech-lead | PR comment | 4h |
| **Security issue** | @security-engineer | Slack | 2h |
| **Production down** | @devops-engineer | Slack | <5 min |
| **Blocking my task** | @scrum-master | Slack | 1h |
| **Process improvement** | @process-assistant | GitHub issue | 48h |

---

## ✅ Definition of Done (Before Merge)

- [ ] Code compiles (0 errors, 0 warnings)
- [ ] Tests pass (100% of suite)
- [ ] Coverage ≥80%
- [ ] Code reviewed (4 checkpoints)
- [ ] No security issues
- [ ] Documentation updated
- [ ] Architectural pattern follows Wolverine (NOT MediatR)
- [ ] TenantId filtering in all queries
- [ ] Audit logging for data changes
- [ ] @tech-lead approval

---

## 🚀 Deploy Checklist (Before Production)

- [ ] All tests passing (>95%)
- [ ] Coverage ≥80%
- [ ] Security review approved
- [ ] @tech-lead go/no-go decision
- [ ] Deployment window (Tue-Thu 2-4 PM UTC)
- [ ] Staging deployment successful
- [ ] Smoke tests pass
- [ ] Rollback procedure tested (<5 min)
- [ ] Team notified (Slack #deployments)
- [ ] Post-deployment verification done

---

## 📊 Key Metrics (Track Weekly)

| Metric | Target | How to Check |
|--------|--------|-------------|
| **Build Time** | < 10s | `dotnet build` |
| **Test Pass Rate** | > 95% | `dotnet test` |
| **Code Coverage** | ≥ 80% | Test report |
| **Code Review SLA** | 90%+ on time | GitHub PR history |
| **Sprint Completion** | 100% | Sprint board |
| **Incident Response** | <5m triage | Incident log |

---

## 🔐 Security Reminders (CRITICAL!)

**Every PR must**:
- [ ] Use `IEncryptionService` for PII (email, phone, name, address)
- [ ] Filter queries by `TenantId` (multi-tenant safety!)
- [ ] Add audit logging for data changes
- [ ] Use FluentValidation for all inputs
- [ ] NO hardcoded secrets (use appsettings.json)
- [ ] NO stack traces in error messages
- [ ] NO SQL injection (parameterized queries only)

---

## 🎯 Wolverine Pattern (NOT MediatR!)

**✅ CORRECT**:
```csharp
public class MyService {
    public async Task<Response> MyMethod(Command cmd, CancellationToken ct) { }
}
```

**❌ WRONG**:
```csharp
public record MyCommand : IRequest<Response>;
public class MyHandler : IRequestHandler<MyCommand, Response> { }
```

---

## 📞 Key Links

| Link | Purpose |
|------|---------|
| [GOVERNANCE_RULES.md](../../../GOVERNANCE.md) | Authority rules |
| [AGENT_ESCALATION_PATH.md](../../../.ai/collaboration/AGENT_TEAM_REGISTRY.md) | Who decides what |
| [WORKFLOW_SPRINT_EXECUTION.md](../../../.github/prompts/iteration-cycle.prompt.md) | Your daily work |
| [WORKFLOW_CODE_REVIEW.md](../../../.github/prompts/code-review.prompt.md) | PR process |
| [WORKFLOW_INCIDENT_RESPONSE.md](../../../.ai/collaboration/PROMPTS_INDEX.md) | If something breaks |

---

## ⏱️ SLA Cheat Sheet

| Situation | SLA | Who |
|-----------|-----|-----|
| Code review peer | 4h | Peer reviewers |
| Code review architecture | 8h | @tech-lead |
| Code review security | 8h | @security-engineer |
| Code review QA | 8h | @qa-engineer |
| CRITICAL incident | 5 min | @devops-engineer |
| HIGH incident | 30 min | @tech-lead |
| MEDIUM issue | 4h | @tech-lead |
| Process change request | 48h | @process-assistant |

---

## 🎓 Training (One-Time, 1 Hour)

1. **Governance** (10 min) - Who has authority
2. **Workflows** (20 min) - Your daily rhythm
3. **Code Review** (15 min) - Quality gates
4. **Incident Response** (15 min) - If emergency happens

---

## 📝 Remember

- ✅ **Build first** - Run `dotnet build` immediately after creating files
- ✅ **Test early** - Run `dotnet test` before pushing
- ✅ **Review carefully** - Code review catches security issues
- ✅ **Escalate early** - Don't wait on blockers
- ✅ **Document decisions** - GitHub is the record of truth
- ✅ **Follow workflows** - They exist to help you succeed
- ✅ **Ask for help** - Team is here to support

---

## ❓ Quick Answers

**Q: "My PR is waiting for review, is that normal?"**  
A: Yes, 4-8h SLA per phase. Check [WORKFLOW_CODE_REVIEW.md](../../../.github/prompts/code-review.prompt.md)

**Q: "Can I deploy on Friday?"**  
A: NO - Safe window is Tue-Thu 2-4 PM UTC only. See [WORKFLOW_DEPLOYMENT.md](../../../.github/prompts/deploy.prompt.md)

**Q: "Production is down, what do I do?"**  
A: Page @devops-engineer immediately. Follow [WORKFLOW_INCIDENT_RESPONSE.md](../../../.ai/collaboration/PROMPTS_INDEX.md)

**Q: "How do I request a process change?"**  
A: File GitHub issue "@process-assistant process-change-request: [description]". Only @process-assistant can modify workflows.

**Q: "Who decides if we use MediatR vs Wolverine?"**  
A: Already decided - Wolverine only. See [copilot-instructions.md](.github/copilot-instructions.md)

---

**Bookmark This Page!**  
**Print for your desk!**  
**Questions?** Ask in standup or escalate via [AGENT_ESCALATION_PATH.md](../../../.ai/collaboration/AGENT_TEAM_REGISTRY.md)

---

**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Owner**: @process-assistant

