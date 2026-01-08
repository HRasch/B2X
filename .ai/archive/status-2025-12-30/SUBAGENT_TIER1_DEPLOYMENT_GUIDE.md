---
docid: UNKNOWN-075
title: SUBAGENT_TIER1_DEPLOYMENT_GUIDE
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# SubAgent Tier 1 Deployment Guide

**Status**: ✅ READY FOR DEPLOYMENT  
**Created**: 30.12.2025  
**For**: Team Activation Monday Jan 6, 2026  
**Duration**: 1 week (Jan 6-10)

---

## 🚀 Quick Start for Teams

### For @Backend
**Available SubAgents**:
- `@SubAgent-APIDesign` - HTTP patterns, versioning, error handling
- `@SubAgent-DBDesign` - Schema design, EF Core patterns, migrations

**How to Use**:
```
When you need: API design patterns
Ask: "@SubAgent-APIDesign, design pattern for user registration endpoint"
Get: .ai/issues/{id}/api-design.md with patterns & examples

When you need: Database schema design
Ask: "@SubAgent-DBDesign, schema design for product catalog"
Get: .ai/issues/{id}/schema-design.md with EF Core examples
```

### For @Frontend
**Available SubAgents**:
- `@SubAgent-ComponentPatterns` - Vue 3 architecture, reusability
- `@SubAgent-Accessibility` - WCAG 2.1 AA compliance, ARIA patterns

**How to Use**:
```
When you need: Component architecture
Ask: "@SubAgent-ComponentPatterns, design pattern for user profile form"
Get: .ai/issues/{id}/component-design.md with Composition API example

When you need: Accessibility audit
Ask: "@SubAgent-Accessibility, audit user profile form for WCAG compliance"
Get: .ai/issues/{id}/a11y-audit.md with issues & fixes
```

### For @QA
**Available SubAgents**:
- `@SubAgent-UnitTesting` - Test patterns, mocking, coverage
- `@SubAgent-ComplianceTesting` - GDPR, NIS2, BITV 2.0, AI Act verification

**How to Use**:
```
When you need: Unit test setup
Ask: "@SubAgent-UnitTesting, test pattern for UserService class"
Get: .ai/issues/{id}/test-setup.md with xUnit & Moq examples

When you need: Compliance audit
Ask: "@SubAgent-ComplianceTesting, GDPR compliance for user data export"
Get: .ai/issues/{id}/compliance-audit.md with verification checklist
```

### For @Security
**Available SubAgents**:
- `@SubAgent-Encryption` - Encryption strategies, key management

**How to Use**:
```
When you need: Encryption strategy
Ask: "@SubAgent-Encryption, encrypt customer email per GDPR Article 32"
Get: .ai/issues/{id}/encryption-strategy.md with AES-256 implementation
```

### For @Legal
**Available SubAgents**:
- `@SubAgent-GDPR` - GDPR compliance, data protection

**How to Use**:
```
When you need: GDPR compliance check
Ask: "@SubAgent-GDPR, verify GDPR compliance for user registration flow"
Get: .ai/issues/{id}/gdpr-compliance.md with checklist & actions
```

---

## 📅 Phase 1 Schedule (Jan 6-10)

```
MONDAY JAN 6
├─ 09:00 - 10:00: Team briefing
│  ├─ SubAgent system overview
│  ├─ When to delegate (Q&A)
│  ├─ Output formats & locations
│  └─ Live examples
├─ 10:00 - 17:00: Start using in tasks
└─ 17:00: First day retrospective

TUESDAY - THURSDAY (JAN 7-9)
├─ 09:00 - 17:00: Use SubAgents in all applicable tasks
├─ Daily check-ins: What's working? What's missing?
├─ Collect feedback: Issues, improvements, suggestions
└─ Adjust as needed

FRIDAY JAN 10
├─ 09:00 - 12:00: Final testing & validation
├─ 12:00 - 13:00: Phase 1 completion review
├─ 13:00 - 15:00: Retrospective & metrics review
│  ├─ Adoption rate (% of tasks using SubAgents)
│  ├─ Context reduction (actual measurements)
│  ├─ Speed improvements (task completion time)
│  └─ Quality metrics (bugs, regressions)
└─ 15:00 - 17:00: Phase 2 planning
```

---

## ✅ Deployment Checklist

```
PRE-DEPLOYMENT (By Dec 31)
═══════════════════════════
□ @SARAH has made 6 critical decisions
□ Team leadership briefed on SubAgent system
□ SubAgent definitions reviewed (✓ Done - in .github/agents/)
□ Output directories prepared (.ai/issues/)
□ Team notified of Monday kickoff

MONDAY KICKOFF (9:00 AM)
════════════════════════
□ 1-hour team training completed
□ All questions answered
□ Examples walked through
□ Delegations understood

DAILY (JAN 6-10)
════════════════
□ @Backend using APIDesign & DBDesign SubAgents
□ @Frontend using ComponentPatterns & Accessibility
□ @QA using UnitTesting & ComplianceTesting
□ @Security using Encryption SubAgent
□ @Legal using GDPR SubAgent
□ Feedback collected in daily retrospectives

FRIDAY VALIDATION (5:00 PM)
═══════════════════════════
□ Phase 1 success criteria met
□ >50% adoption rate observed
□ Context reduction measured (65%+ on main agents)
□ No quality degradation
□ Team confident for Phase 2
```

---

## 📊 Success Metrics (Phase 1)

### Target Adoption Rate
- **Goal**: >50% of applicable tasks use SubAgents
- **Measurement**: Track which tasks delegate vs. don't
- **Success**: 4+ of 5 agents actively delegating by Friday

### Context Reduction
- **Goal**: 65-70% reduction per delegating agent
- **Before**: @Backend 28 KB, @Frontend 24 KB, @QA 22 KB, etc.
- **After**: @Backend 8 KB, @Frontend 8 KB, @QA 8 KB, etc.
- **Measurement**: Use token counting tools to measure actual reduction

### Speed & Quality
- **Goal**: 20% faster task completion
- **Measurement**: Compare task duration before/after SubAgents
- **Quality**: Zero regressions in code quality metrics

### Team Satisfaction
- **Goal**: Positive feedback from all agents
- **Measurement**: Daily retrospectives, Friday survey
- **Success**: 4/5 agents recommend continuing in Phase 2

---

## 🎯 Key Guidelines

### When to Delegate
```
✅ USE SUBAGENTS FOR:
- Pattern/design questions
- Standard procedures
- Compliance verification
- Code reviews
- Documentation generation

❌ DON'T DELEGATE:
- Active decision-making
- Current task implementation
- Real-time feedback loops
- Emergency problem-solving
```

### How to Request
```
Clear request:
"@SubAgent-APIDesign, design HTTP pattern for bulk user import endpoint"

Good context:
- Component/class name
- Purpose/use case
- Technology stack
- Key requirements

SubAgent will provide:
- .ai/issues/{issue-id}/api-design.md
- Concrete examples
- Best practices
- Wolverine-specific patterns
```

### Output Locations
```
.ai/issues/{issue-id}/
├── api-design.md (from @SubAgent-APIDesign)
├── schema-design.md (from @SubAgent-DBDesign)
├── component-design.md (from @SubAgent-ComponentPatterns)
├── a11y-audit.md (from @SubAgent-Accessibility)
├── test-setup.md (from @SubAgent-UnitTesting)
├── compliance-audit.md (from @SubAgent-ComplianceTesting)
├── encryption-strategy.md (from @SubAgent-Encryption)
└── gdpr-compliance.md (from @SubAgent-GDPR)
```

---

## 🔧 Troubleshooting

### "SubAgent output is too generic"
**Solution**: Provide more context
- Current implementation details
- Specific edge cases to handle
- Performance/scale requirements
- Related components/systems

### "I don't think I need a SubAgent for this"
**Best practice**: If >2 KB of reference material needed, delegate it
- Frees up your context for decision-making
- Standardizes approach
- Builds knowledge base for others

### "Output location is wrong"
**Check**: Issue ID (use current issue number)
- Format: `.ai/issues/{issue-id}/`
- Example: `.ai/issues/FEAT-123/api-design.md`

### "SubAgent knowledge seems incomplete"
**Feedback channel**: Daily retrospectives
- Note what's missing
- Suggest improvements
- Provide examples
- Phase 2 refinements based on feedback

---

## 📋 Agent-by-Agent Deployment

### @Backend Deployment
```
SubAgents Available:
1. @SubAgent-APIDesign
2. @SubAgent-DBDesign

First Week Tasks:
- Implement 2-3 API endpoints using @SubAgent-APIDesign
- Design 1 new database schema using @SubAgent-DBDesign
- Measure context reduction (target: 8 KB)

Success Criteria:
- Both SubAgents used at least once
- >60% adoption on API/DB tasks
- Context reduced by 65%+
- Quality: No regressions
```

### @Frontend Deployment
```
SubAgents Available:
1. @SubAgent-ComponentPatterns
2. @SubAgent-Accessibility

First Week Tasks:
- Build 2 new components using @SubAgent-ComponentPatterns
- Audit 1 existing component with @SubAgent-Accessibility
- Measure context reduction (target: 8 KB)

Success Criteria:
- Both SubAgents used at least once
- >60% adoption on component tasks
- Context reduced by 65%+
- Quality: No accessibility regressions
```

### @QA Deployment
```
SubAgents Available:
1. @SubAgent-UnitTesting
2. @SubAgent-ComplianceTesting

First Week Tasks:
- Create test suite for 1 service using @SubAgent-UnitTesting
- Verify compliance for 1 feature using @SubAgent-ComplianceTesting
- Measure context reduction (target: 8 KB)

Success Criteria:
- Both SubAgents used at least once
- >50% adoption on test/compliance tasks
- Context reduced by 65%+
- Quality: Test coverage maintained
```

### @Security Deployment
```
SubAgents Available:
1. @SubAgent-Encryption

First Week Tasks:
- Design encryption for 1 PII field using @SubAgent-Encryption
- Verify implementation against patterns
- Measure context reduction (target: 6 KB)

Success Criteria:
- SubAgent used at least once
- Encryption implemented correctly
- Context reduced by 60%+
- Quality: Zero security regressions
```

### @Legal Deployment
```
SubAgents Available:
1. @SubAgent-GDPR

First Week Tasks:
- Verify compliance for 1 feature using @SubAgent-GDPR
- Create compliance checklist
- Measure context reduction (target: 6 KB)

Success Criteria:
- SubAgent used at least once
- Compliance checklist created
- Context reduced by 60%+
- Quality: Legal review positive
```

---

## 📞 Support & Escalation

### If You Have Questions
1. **First**: Check SubAgent description in `.github/agents/SubAgent-{Name}.agent.md`
2. **Then**: Ask during team briefing (Monday 9:00-10:00)
3. **Then**: Bring to daily retrospective
4. **Then**: Escalate to @SARAH if blocker

### If SubAgent Isn't Working
1. Check: Is input clear and specific?
2. Adjust: Provide more context, examples
3. Report: Note in daily retrospective
4. Escalate: To @SARAH if pattern issue

### If You Disagree with SubAgent Output
1. Use it as starting point
2. Provide feedback in retrospective
3. Phase 2 SubAgents refined based on feedback
4. Your input shapes next iteration

---

## 🎓 Team Training Topics (Monday 9:00-10:00)

1. **SubAgent System Overview** (5 min)
   - What are SubAgents?
   - Why delegation strategy?
   - How does it work?

2. **When to Delegate** (10 min)
   - Perfect for delegation checklist
   - Keep in main context
   - Examples by role

3. **How to Request** (10 min)
   - Clear request format
   - What context to provide
   - What to expect back

4. **Output Formats** (5 min)
   - Where outputs go
   - How to use them
   - How to integrate

5. **Live Demos** (15 min)
   - @Backend: API design delegation
   - @Frontend: Component audit
   - @QA: Test pattern creation

6. **Q&A** (15 min)
   - Open questions
   - Concerns
   - Clarifications

---

## 📈 Phase 1 Success = Phase 2 Green Light

**If Phase 1 succeeds**:
- ✅ >50% adoption rate
- ✅ 65%+ context reduction
- ✅ 20% speed improvement observed
- ✅ Team confident & satisfied

**Then**:
- Phase 2 kicks off Monday Jan 13
- 14 more SubAgents (Medium-priority)
- Extended coverage for DevOps, Architect, TechLead
- Full ecosystem deployment by early February

---

**Ready for Monday 9:00 AM Kickoff?** 🚀

Next: Team briefing, delegation practice, feedback collection
Goal: 50%+ adoption, 65%+ context reduction by Friday
Confidence: 95%+
