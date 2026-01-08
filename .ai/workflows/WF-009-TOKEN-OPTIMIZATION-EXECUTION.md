---
docid: WF-020
title: WF 009 TOKEN OPTIMIZATION EXECUTION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: WF-009
title: Token Optimization Implementation - 20-Day Execution Plan
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# WF-009: Token Optimization Implementation Plan

**Duration**: 20 days (Jan 7 - Jan 27, 2026)  
**Goal**: Implement GL-043/044/045/046/047/048 for **80% token reduction**  
**Owner**: @SARAH (Coordination), @CopilotExpert (Implementation)

---

## 🎯 Success Metrics

| Metric | Baseline | Target | Status |
|--------|----------|--------|--------|
| Instruction file size | 18.1 KB | <13.4 KB | Pending |
| Avg tokens/interaction | ~15,000 | ~3,000 | Pending |
| Rate limit incidents | Frequent | Zero | Pending |
| Monthly token budget relief | 0% | 70-80% | Pending |
| Agent adoption rate | 0% | 100% | Pending |

---

## 📅 WEEK 1: Foundation (Jan 7-13)

### Day 1-2: Setup & Documentation (Jan 7-8)
**Owner**: @SARAH + @CopilotExpert

#### Tasks
- [ ] Review all 6 Guidelines (GL-043 through GL-048)
- [ ] Create this execution document (WF-009)
- [ ] Run baseline audit: `scripts/audit-tokens.sh`
  - Measure current instruction file sizes
  - Document current KB article pre-loading patterns
  - Record baseline token metrics
- [ ] Document current state in `.ai/logs/`

**Deliverable**: `baseline-audit-report.md`

**Estimated Effort**: 2 hours

---

### Day 3: Agent Training (Jan 9)
**Owner**: @SARAH

#### Tasks
- [ ] Brief all agents on GL-043 (Smart Attachments)
  - Only load path-specific instructions
  - Always include security.instructions.md
  - Stop pre-loading other files
  
- [ ] Brief all agents on GL-044 (Fragment-Based Access)
  - Start with grep_search for locations
  - Read only targeted ranges (±5 lines context)
  - Use semantic_search for patterns
  
- [ ] Brief all agents on GL-045 (KB-MCP Queries)
  - Query KB on-demand instead of pre-loading
  - Use kb-mcp/search_knowledge_base for topics
  - Link via [KB-XXX] instead of attaching

**Deliverable**: Agent briefing completed

**Estimated Effort**: 1 hour

---

### Day 4-5: Immediate Implementation (Jan 10-11)
**Owner**: All Agents

#### Tasks
- [ ] Start applying GL-043 (Smart Attachments) to all work
- [ ] Start applying GL-044 (Fragment-Based Access) to all file reads
- [ ] Start applying GL-045 (KB-MCP Queries) for knowledge needs
- [ ] Document patterns observed in `.ai/logs/implementation-log.md`

**Success Criteria**:
- All new code work uses path-specific instructions
- No full-file reads without grep_search first
- No KB articles pre-loaded

**Estimated Effort**: Integrated into normal work

---

### Day 6: KB Articles Preparation (Jan 12)
**Owner**: @CopilotExpert

#### Tasks
- [ ] Plan 3 new KB articles for GL-048:
  - KB-065: MCP Testing Patterns (4 KB)
  - KB-066: Integration Testing Patterns (5 KB)
  - KB-067: DevOps MCP Workflows (3 KB)

- [ ] Prepare content extraction from:
  - testing.instructions.md (lines 50-150 → KB-065)
  - testing.instructions.md (lines 200-300 → KB-066)
  - devops.instructions.md (lines 40-100 → KB-067)

**Deliverable**: Content outline for 3 KB articles

**Estimated Effort**: 2 hours

---

### Day 7: Week 1 Review (Jan 13)
**Owner**: @SARAH

#### Tasks
- [ ] Run audit again: Measure adoption of GL-043/044/045
- [ ] Collect feedback from agents
- [ ] Document early wins in implementation log
- [ ] Identify blockers or challenges

**Success Criteria**:
- 80%+ of new work uses optimized patterns
- No blockers preventing GL-048 rollout

**Deliverable**: Week 1 status report

**Estimated Effort**: 1 hour

---

## 📅 WEEK 2: Refactoring (Jan 14-20)

### Day 8-9: Create KB Articles (Jan 14-15)
**Owner**: @CopilotExpert

#### Tasks
- [ ] Create KB-065: MCP Testing Patterns
  - Content from testing.instructions.md MCP section
  - Add concrete examples for each MCP tool
  - Include workflow diagrams
  - Link back from testing.instructions.md
  
- [ ] Create KB-066: Integration Testing Patterns
  - Content from Docker/Database/API testing sections
  - Extract reusable patterns
  - Link back from testing.instructions.md
  
- [ ] Create KB-067: DevOps MCP Workflows
  - Content from devops.instructions.md MCP section
  - Reference KB-055, KB-057, KB-061
  - Link back from devops.instructions.md

**Deliverables**: KB-065, KB-066, KB-067 created

**Estimated Effort**: 6 hours total (2 per article)

---

### Day 10: Trim testing.instructions.md (Jan 16)
**Owner**: @CopilotExpert

#### Tasks
- [ ] Delete lines 50-150 (move to KB-065)
- [ ] Delete lines 200-300 (move to KB-066)
- [ ] Delete other detailed examples
- [ ] Add references: "See [KB-065] MCP Testing Patterns"
- [ ] Keep: Test structure, coverage goals, warning policy
- [ ] Verify new size: 8.2 KB → 2.0 KB target

**Success Criteria**:
- File size < 2.5 KB
- All links to KB articles work
- Content is still actionable

**Deliverable**: Updated testing.instructions.md

**Estimated Effort**: 3 hours

---

### Day 11: Trim devops.instructions.md (Jan 17)
**Owner**: @CopilotExpert

#### Tasks
- [ ] Delete MCP section (move to KB-067)
- [ ] Delete detailed Docker/K8s examples
- [ ] Add references: "See [KB-055], [KB-057], [KB-061]"
- [ ] Keep: CI/CD basics, Docker core, monitoring overview
- [ ] Verify new size: 3.5 KB → 1.5 KB target

**Success Criteria**:
- File size < 2.0 KB
- All links work
- Content provides clear guidance

**Deliverable**: Updated devops.instructions.md

**Estimated Effort**: 2 hours

---

### Day 12: Create consolidated.instructions.md (Jan 18)
**Owner**: @CopilotExpert

#### Tasks
- [ ] Create new `.github/instructions/consolidated.instructions.md`
- [ ] Include:
  - Quick navigation for each domain
  - Links to path-specific files
  - Links to relevant KB articles
  - Overview of GL-043/044/045/047
  - 5-second rules for quick reference
- [ ] Verify size: ~2.5 KB

**Deliverable**: consolidated.instructions.md (2.5 KB)

**Estimated Effort**: 2 hours

---

### Day 13: Validation & Testing (Jan 19)
**Owner**: @CopilotExpert + Agents

#### Tasks
- [ ] Test all instruction file links
- [ ] Verify KB article queries return expected content
- [ ] Agents test path-specific loading with trimmed files
- [ ] Agents test KB-MCP queries for new articles
- [ ] Confirm no broken references

**Success Criteria**:
- All links functional
- KB queries return relevant content
- No errors or 404s

**Deliverable**: Validation report

**Estimated Effort**: 2 hours

---

### Day 14: Week 2 Review (Jan 20)
**Owner**: @SARAH

#### Tasks
- [ ] Run audit: Measure instruction file sizes
  - Target: 13.4 KB (achieved?)
  - Individual files within targets?
  
- [ ] Run audit: Measure token consumption
  - Instruction overhead reduced?
  - KB queries effective?
  
- [ ] Collect agent feedback
- [ ] Update DOCUMENT_REGISTRY with KB-065, KB-066, KB-067
- [ ] Document progress

**Deliverable**: Week 2 status report + updated DOCUMENT_REGISTRY

**Estimated Effort**: 2 hours

---

## 📅 WEEK 3: Optimization & Rollout (Jan 21-27)

### Day 15-16: Final Tuning (Jan 21-22)
**Owner**: @SARAH + @CopilotExpert

#### Tasks
- [ ] Analyze agent feedback from Weeks 1-2
- [ ] Identify any remaining bottlenecks
- [ ] Fine-tune GL-047 decision engine if needed
- [ ] Update guidelines based on real-world patterns
- [ ] Create agent-specific quick reference cards

**Deliverable**: Final tuning complete, guidelines finalized

**Estimated Effort**: 3 hours

---

### Day 17: Full Adoption (Jan 23)
**Owner**: All Agents

#### Tasks
- [ ] All agents fully apply GL-043/044/045/047
- [ ] No manual workarounds needed
- [ ] All KB queries working smoothly
- [ ] Fragment-based access standard practice
- [ ] Path-specific attachments automatic

**Success Criteria**:
- 100% adoption of optimized patterns
- No slowdowns or bottlenecks
- Agents report improved efficiency

**Estimated Effort**: Integrated into normal work

---

### Day 18: Comprehensive Audit (Jan 24)
**Owner**: @SARAH

#### Tasks
- [ ] Run comprehensive token audit: `scripts/audit-tokens.sh --full`
  - Measure instruction file sizes (target: 13.4 KB)
  - Count KB articles pre-loaded (target: 0)
  - Analyze file access patterns
  - Calculate actual token savings
  - Compare vs baseline from Jan 7

- [ ] Document results:
  - Current state
  - Improvements achieved
  - Remaining opportunities
  - Recommended next steps

**Deliverable**: Comprehensive audit report

**Estimated Effort**: 3 hours

---

### Day 19: Documentation & Knowledge Transfer (Jan 25)
**Owner**: @SARAH + @DocMaintainer

#### Tasks
- [ ] Update `.ai/DOCUMENT_REGISTRY.md`
  - Add GL-043 through GL-048
  - Add KB-065, KB-066, KB-067
  - Update GL-047 entry
  
- [ ] Update `.github/copilot-instructions.md`
  - Reference new guidelines
  - Update quick reference section
  
- [ ] Create implementation summary
  - What was done
  - What was achieved
  - How to maintain going forward
  
- [ ] Train new agents on optimized patterns

**Deliverable**: Updated registry + comprehensive documentation

**Estimated Effort**: 3 hours

---

### Day 20: Final Review & Celebration (Jan 27)
**Owner**: @SARAH

#### Tasks
- [ ] Final audit: Confirm all metrics met
  - Instruction files: <13.4 KB ✓
  - Avg tokens/interaction: <3,500 ✓
  - Rate limit incidents: Zero ✓
  - Agent adoption: 100% ✓
  
- [ ] Create final report:
  - Baseline vs final metrics
  - Token savings achieved
  - Impact on rate limiting
  - Lessons learned
  
- [ ] Celebrate achievements! 🎉
- [ ] Plan maintenance strategy going forward

**Deliverable**: Final report + maintenance plan

**Estimated Effort**: 2 hours

---

## 📊 Daily Standup Template

Use this for daily tracking:

```markdown
## Daily Standup: [Date]

### Completed Today
- [ ] Task 1
- [ ] Task 2

### In Progress
- [ ] Task 3

### Blockers
- None / [Description]

### Notes
- [Any observations or learnings]

### Tomorrow
- [ ] Task 4
- [ ] Task 5
```

---

## 📈 Weekly Metrics Tracking

### Week 1 (Jan 7-13)
```
Target: Setup complete, GL-043/044/045 adoption started
Instruction file size:  18.1 KB (baseline)
Avg tokens/interaction: ~12,000 (reduced from 15,000)
Adoption rate:         60%
Blockers:              None
```

### Week 2 (Jan 14-20)
```
Target: GL-048 refactoring complete
Instruction file size:  <13.4 KB (target)
Avg tokens/interaction: ~5,000 (major reduction!)
Adoption rate:         95%
Blockers:              [Identify if any]
```

### Week 3 (Jan 21-27)
```
Target: 100% adoption, full optimization
Instruction file size:  13.4 KB (achieved)
Avg tokens/interaction: <3,500 (target)
Adoption rate:         100%
Rate limit relief:     ACHIEVED ✅
```

---

## 🚨 Risk Management

### Risk 1: Agents Not Adopting New Patterns
**Likelihood**: Medium  
**Impact**: Savings not realized  
**Mitigation**:
- Daily reminders during Week 1
- Clear examples in GL-043/044/045
- Weekly reviews with feedback
- Celebrate early wins publicly

---

### Risk 2: KB Articles Missing Content
**Likelihood**: Low  
**Impact**: Agents need to reference old files  
**Mitigation**:
- Thorough validation (Day 13)
- Create KB articles before deleting content
- Keep old content for 1 week as backup
- Agent feedback loop

---

### Risk 3: Broken Links After Consolidation
**Likelihood**: Low  
**Impact**: Agents confused, reduced adoption  
**Mitigation**:
- Comprehensive link validation (Day 13)
- Automated link checker in CI/CD
- Clear error messages if links break
- Rollback procedure ready

---

### Risk 4: Performance Issues with KB Queries
**Likelihood**: Very Low  
**Impact**: Queries slow, agents revert to pre-loading  
**Mitigation**:
- Test KB queries early (Week 1)
- Monitor response times
- Have fallback to KB articles if needed
- Escalate to KB team if issues

---

## ✅ Completion Checklist

### Foundation Phase (Week 1)
- [ ] All guidelines documented (GL-043 through GL-048)
- [ ] Baseline audit completed
- [ ] Agents briefed on new patterns
- [ ] GL-043/044/045 adoption started
- [ ] KB articles planned

### Refactoring Phase (Week 2)
- [ ] KB-065, KB-066, KB-067 created
- [ ] testing.instructions.md trimmed (8.2 → 2.0 KB)
- [ ] devops.instructions.md trimmed (3.5 → 1.5 KB)
- [ ] consolidated.instructions.md created
- [ ] All links validated
- [ ] Adoption at 95%+

### Optimization Phase (Week 3)
- [ ] Final tuning complete
- [ ] 100% agent adoption
- [ ] Comprehensive audit passed
- [ ] Documentation updated
- [ ] 80% token reduction achieved ✅
- [ ] Rate limiting problem solved 🎉

---

## 📞 Escalation Procedures

### During Implementation (Weeks 1-3)

**For technical issues**:
1. Document in implementation log
2. Reach out to @CopilotExpert
3. If blocking: escalate to @SARAH same day

**For adoption blockers**:
1. Brief agents on solution
2. Provide clear examples
3. Daily follow-up during Week 1
4. If persistent: escalate to @SARAH

**For metric gaps**:
1. Run audit to diagnose
2. Identify root cause
3. Adjust approach if needed
4. Report to @SARAH

---

## 🎓 Knowledge Transfer

### Documentation Created
- [ ] GL-043: Smart Attachment Strategy
- [ ] GL-044: Fragment-Based File Access
- [ ] GL-045: KB-MCP Query Strategy
- [ ] GL-046: Token Audit Framework
- [ ] GL-047: MCP-Orchestration Layer
- [ ] GL-048: Instruction File Consolidation
- [ ] WF-009: This execution plan
- [ ] KB-065, KB-066, KB-067: New KB articles
- [ ] scripts/audit-tokens.sh: Audit tool

### Agents Trained
- [ ] All current agents briefed
- [ ] New agents learn from documentation
- [ ] Clear examples available
- [ ] Fallback procedures documented

---

## 🎉 Expected Outcomes

### Token Consumption
```
BEFORE:    15,000 tokens/interaction (rate limited!)
AFTER:     3,000 tokens/interaction (optimal!)
SAVINGS:   80% ✅
MONTHLY:   375,000+ tokens saved
```

### Rate Limiting Impact
```
BEFORE:    Frequent incidents
AFTER:     Zero incidents
RELIEF:    Unlimited capacity! 🚀
```

### Agent Efficiency
```
BEFORE:    Bloated context, slow responses
AFTER:     Minimal context, fast responses
BENEFIT:   Better developer experience
```

---

## 📋 Sign-Off

**Execution Plan Created**: Jan 7, 2026  
**Owner**: @SARAH  
**Reviewer**: @CopilotExpert  
**Coordinator**: @SARAH  

---

**Last Updated**: 7. Januar 2026  
**Status**: Ready for Execution  
**Target Completion**: Jan 27, 2026

---

## Quick Links
- [GL-043] Smart Attachment Strategy
- [GL-044] Fragment-Based File Access
- [GL-045] KB-MCP Query Strategy
- [GL-046] Token Audit Framework
- [GL-047] MCP-Orchestration Layer
- [GL-048] Instruction File Consolidation
