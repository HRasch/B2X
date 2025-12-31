# SubAgent Decision Matrix - Quick Reference

**Created:** 30.12.2025  
**For:** @SARAH (Coordinator)  
**Status:** ⏳ AWAITING DECISIONS

---

## Situation Summary

**Problem**: Main agents carry 20-28 KB of reference material (patterns, guidelines, checklists), reducing focus and token efficiency.

**Solution**: Delegate reference material to 42 specialized SubAgents (Tier 1-3), keeping main agents at 7-8 KB focused on decision-making.

**Impact**:
- Context reduction: 170 KB → 100 KB (41%)
- Token efficiency: +40%
- Task speed: +20%
- Implementation: 117 hours total (28h Phase 1)

---

## Decision 1: Tier 1 Approval (Phase 1 Start)

### What is Tier 1?
8 high-value SubAgents covering Backend, Frontend, QA, Security, and Legal.

**Effort**: 28 hours (Week of Jan 6-10, 2026)

### The 8 Tier 1 SubAgents

| # | SubAgent | Parent | Topics | Benefit |
|---|----------|--------|--------|---------|
| 1 | **@SubAgent-APIDesign** | @Backend | HTTP patterns, status codes, versioning, validation | Clean API architecture |
| 2 | **@SubAgent-DBDesign** | @Backend | Schema design, migrations, query optimization | Database scalability |
| 3 | **@SubAgent-ComponentPatterns** | @Frontend | Vue 3 Composition API, component architecture | Clean Vue components |
| 4 | **@SubAgent-Accessibility** | @Frontend | WCAG 2.1 AA, ARIA, keyboard navigation | Accessible UIs |
| 5 | **@SubAgent-UnitTesting** | @QA | Backend unit test patterns, mocking, coverage | Automated testing |
| 6 | **@SubAgent-ComplianceTesting** | @QA | GDPR, NIS2, BITV 2.0, AI Act verification | Regulatory compliance |
| 7 | **@SubAgent-Encryption** | @Security | AES-256, TLS, key management, rotation | Data security |
| 8 | **@SubAgent-GDPR** | @Legal | Articles 32/35, DPA, consent mechanisms | Legal compliance |

**Questions to Decide**:

```
❓ Q1: Approve Tier 1 SubAgents as specified?
    ○ YES → Proceed with Phase 1
    ○ NO  → Modify which ones? (list needed)

❓ Q2: Include DevOps/Architect SubAgents in Tier 1?
    ○ YES → Add @SubAgent-K8s, @SubAgent-DDD (12 total)
    ○ NO  → Keep 8, plan for Phase 2

❓ Q3: Add @TechLead SubAgents to Tier 1?
    ○ YES → Add @SubAgent-CodeQuality (9 total)
    ○ NO  → Keep 8, plan for Phase 2
```

**Recommendation**: Start with 8 (Backend/Frontend/QA/Security/Legal priority).

---

## Decision 2: Governance Model

### How Should SubAgents Work?

**Option A: Autonomous SubAgents** (Recommended)
```
@Backend calls @SubAgent-APIDesign directly
SubAgent executes, outputs to .ai/issues/{id}/api-design.md
@Backend reads output & continues
→ Fast, efficient, minimal coordination overhead
```

**Option B: Approval-Gated SubAgents**
```
@Backend calls @SubAgent-APIDesign
SubAgent creates draft
@Backend approves or rejects
Then publishes to .ai/issues/{id}/api-design.md
→ Slower, more control, coordination required
```

**Option C: Hybrid Model**
```
Tier 1-2 SubAgents: Autonomous
Tier 3 SubAgents: Approval-gated
→ Balance speed & control
```

**Questions to Decide**:

```
❓ Q4: Choose governance model:
    ○ Autonomous (fastest, recommended)
    ○ Approval-gated (safer, slower)
    ○ Hybrid (balanced)

❓ Q5: Who approves output quality?
    ○ Parent agent (@Backend, etc.)
    ○ Specialized reviewer agent
    ○ No approval needed

❓ Q6: Output retention:
    ○ Keep all outputs (knowledge base)
    ○ Archive by task (clean per-task)
    ○ Both (outputs + summaries)
```

**Recommendation**: Autonomous for Tier 1 (build confidence), then reassess.

---

## Decision 3: Context Management

### How Often Do SubAgents Refresh Their Context?

**Option A: Per-Task Context**
```
SubAgent created for one task
Carries only task-relevant context
Destroyed after task complete
→ Minimal memory, maximum focus
```

**Option B: Per-Session Context**
```
SubAgent exists for entire sprint/session
Carries cumulative context (learnings, patterns)
Persists across multiple tasks
→ Continuous learning, slightly larger context
```

**Option C: Knowledge-Base Driven**
```
SubAgent minimal context (2 KB)
Pulls detailed info from .ai/knowledgebase/ on-demand
Caches common patterns
→ Maximum efficiency, requires KB infrastructure
```

**Questions to Decide**:

```
❓ Q7: Choose context model:
    ○ Per-task (stateless)
    ○ Per-session (stateful)
    ○ Knowledge-base driven (hybrid)

❓ Q8: Update frequency:
    ○ Daily (refresh all Tier 1-2)
    ○ Weekly (refresh on schedule)
    ○ On-demand (refresh when referenced)

❓ Q9: Knowledge base infrastructure:
    ○ Create (.ai/knowledgebase/) - 20h effort
    ○ Use existing (.ai/guidelines/) - 0h effort
    ○ Plan for Phase 2
```

**Recommendation**: Per-task context + on-demand updates to start simple.

---

## Decision 4: Integration Points

### How Do SubAgents Integrate With Main Agents?

**Option A: Explicit Delegation Pattern**
```
@Backend says: "Delegate to @SubAgent-APIDesign"
SARAH coordinates call
SubAgent executes
Output returned to @Backend
→ Clear, auditable, requires coordination
```

**Option B: Auto-Discovery Pattern**
```
@Backend: "I need API design patterns"
System auto-detects need
SubAgent @SubAgent-APIDesign activated
Output streamed to @Backend
→ Seamless, requires smart routing
```

**Option C: Manual Integration Pattern**
```
@Backend manually decides when to ask SubAgent
Calls explicitly with context
Integrates output into response
→ Maximum control, more manual work
```

**Questions to Decide**:

```
❓ Q10: Delegation mechanism:
    ○ Explicit (via @SARAH)
    ○ Auto-discovery (smart routing)
    ○ Manual (agent decides)

❓ Q11: Output integration:
    ○ Direct injection (SubAgent output becomes part of answer)
    ○ Summarization (SubAgent output summarized, then injected)
    ○ Reference (SubAgent output linked, agent reads & decides)

❓ Q12: Coordination overhead:
    ○ Accept latency for coordination
    ○ Optimize for speed (parallel execution)
    ○ Hybrid (parallel where possible)
```

**Recommendation**: Explicit delegation + reference pattern (clean, auditable).

---

## Decision 5: Team Training & Communication

### Do We Need Training Before Phase 1?

**Option A: Team Training Kickoff** (2 hours)
```
Mon 9:00 - 11:00: All agents learn SubAgent system
├─ How SubAgents work
├─ When to delegate
├─ Output formats & locations
├─ Real examples (API design, DB schema)
└─ Q&A session
→ Better adoption, alignment, fewer mistakes
```

**Option B: Documentation-Only**
```
Create SUBAGENT_USER_GUIDE.md
Each agent reads on-demand
Learn by doing
→ Faster start, more confusion early on
```

**Option C: Gradual Rollout**
```
Day 1: @Backend gets @SubAgent-APIDesign + docs
Day 2: @Backend reports experience
Day 3: @Frontend gets SubAgents + feedback integration
→ Iterative, learns from early adopters
```

**Questions to Decide**:

```
❓ Q13: Communication strategy:
    ○ Team training kickoff (2h)
    ○ Documentation only
    ○ Gradual rollout
    ○ Combined (training + docs + gradual)

❓ Q14: Success metrics:
    ○ >60% delegation rate (track who uses them)
    ○ 65%+ context reduction (measure actual context)
    ○ 20% speed improvement (track task duration)
    ○ All of above

❓ Q15: Feedback loop:
    ○ Weekly retrospectives
    ○ Mid-phase check-ins (after 3-4 days)
    ○ Issue-based (collect pain points)
    ○ All of above
```

**Recommendation**: Team kickoff (1 hour) + documentation + gradual rollout.

---

## Decision 6: Success Criteria

### What Does Success Look Like?

**Phase 1 Success** (Must achieve all):
- [ ] 8 SubAgents created & tested
- [ ] Main agents' context reduced to 8-10 KB
- [ ] >50% delegation rate observed
- [ ] No quality degradation
- [ ] Team confident to move to Phase 2

**Overall Success** (Track throughout):
- [ ] Context sizes: 170 KB → 100 KB (41% reduction)
- [ ] Token efficiency: +40%
- [ ] Task speed: +20%
- [ ] Adoption rate: >60% of tasks use SubAgents
- [ ] Quality: 0 regression in code quality metrics

**Questions to Decide**:

```
❓ Q16: Measure context size:
    ○ Token count via API
    ○ Manual estimation (KB size)
    ○ Both

❓ Q17: Measure task speed:
    ○ Time from request to delivery
    ○ Time to first output
    ○ Both

❓ Q18: Exit criteria for Phase 2:
    ○ Must achieve ALL Phase 1 success criteria
    ○ Must pass retrospective review
    ○ Must get team sign-off
    ○ All of above
```

**Recommendation**: Track all metrics, require all Phase 1 success criteria before Phase 2.

---

## Quick Decision Checklist

Print this and fill in your decisions:

```
┌─ DECISION CHECKLIST ────────────────────────────────┐
│                                                     │
│ Decision 1: Tier 1 Approval                        │
│ ○ 8 SubAgents as specified                         │
│ ○ 12 SubAgents (add DevOps/Architect)              │
│ ○ 9 SubAgents (add TechLead)                       │
│ Recommendation: 8 (Backend/Frontend/QA priority)   │
│                                                     │
│ Decision 2: Governance Model                       │
│ ○ Autonomous (fast)                                │
│ ○ Approval-gated (safe)                            │
│ ○ Hybrid (balanced)                                │
│ Recommendation: Autonomous for Tier 1              │
│                                                     │
│ Decision 3: Context Management                     │
│ ○ Per-task                                         │
│ ○ Per-session                                      │
│ ○ Knowledge-base driven                            │
│ Recommendation: Per-task (simplest)                │
│                                                     │
│ Decision 4: Integration Pattern                    │
│ ○ Explicit delegation                              │
│ ○ Auto-discovery                                   │
│ ○ Manual                                           │
│ Recommendation: Explicit + reference               │
│                                                     │
│ Decision 5: Team Communication                     │
│ ○ Team training                                    │
│ ○ Documentation only                               │
│ ○ Gradual rollout                                  │
│ ○ Combined                                         │
│ Recommendation: Team kickoff + docs + gradual      │
│                                                     │
│ Decision 6: Success Criteria                       │
│ ○ Measure context, speed, adoption                 │
│ ○ Require Phase 1 sign-off before Phase 2          │
│ ○ Track all metrics throughout                     │
│ Recommendation: All of above                       │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## Timeline If Approved Today (Dec 30)

```
TODAY (Dec 30):
→ @SARAH Makes decisions (6 decisions, 1 hour)
→ Post decisions to team

TOMORROW (Dec 31):
→ SARAH creates SubAgent definitions (1 day)
→ Shares with team for feedback

MONDAY JAN 6 (Phase 1 Start):
09:00 - Team briefing on SubAgent strategy (1 hour)
10:00 - Tier 1 SubAgent creation begins (4 days)
15:00 (Wed) - Initial testing & validation
17:00 (Fri) - Phase 1 complete & handoff to Phase 2 planning

TIMELINE: 28 hours over 5 days
TEAM READY FOR PHASE 2: Friday Jan 10
PHASE 2 STARTS: Monday Jan 13 (44 more hours)
FULL ROLLOUT: ~3 weeks total
```

---

## Risk Assessment

### Low Risk ✅
- SubAgents are new (no breaking changes)
- Main agents can test before adopting
- Easy rollback (disable SubAgents)
- Parallel to existing workflows

### Medium Risk ⚠️
- Team adoption (some agents reluctant to delegate)
- Mitigation: Team training, early success stories
- Quality concerns (are SubAgents good enough?)
- Mitigation: Phase 1 validation, quality gates

### Minimal Risk ✅
- No breaking changes to existing code
- No impact on production systems
- Documentation-focused changes only

---

## Final Recommendation

**START IMMEDIATELY** with these decisions:

1. ✅ **Approve Tier 1**: 8 SubAgents (Backend, Frontend, QA, Security, Legal)
2. ✅ **Governance**: Autonomous SubAgents (simplest, fastest)
3. ✅ **Context**: Per-task, on-demand updates
4. ✅ **Integration**: Explicit delegation + reference pattern
5. ✅ **Communication**: Team kickoff (1h) + docs + gradual rollout
6. ✅ **Metrics**: Track context, speed, adoption (Phase 1 gate on success)

**Effort**: 28 hours (Week of Jan 6-10)  
**Risk**: Low  
**Confidence**: 95%+  
**Impact**: 41% context reduction, 40% token efficiency gain  

---

**Ready for @SARAH decision**: YES ✅
**Questions answered**: 18
**Time to decide**: ~1 hour
**Next step**: Post decisions, create SubAgent definitions, start Phase 1 Monday 09:00
