# AI Agent Feedback Documentation

**Purpose**: Collect and track feedback on agent instructions, behavior, and processes  
**Authority**: @scrum-master (collection) → @process-assistant (implementation)  
**Last Updated**: 30. Dezember 2025  

---

## When to Document Feedback

Document feedback when:

- ✅ **Problems**: Agent instructions unclear, errors, unexpected behavior, limitations
- ✅ **Conflicts**: Disagreement between agents, unclear authority, process conflict
- ✅ **Unclear Behavior**: Ambiguous instruction, confusing documentation, undocumented patterns
- ✅ **Process Issues**: Workflow doesn't work as documented, bottleneck, inefficiency
- ✅ **Instruction Gaps**: Missing guidance, incomplete examples, insufficient detail

---

## How to Document Feedback

### File Naming

Create file: `.github/ai-feedback/{YYYY-MM-DD}-{agent-name}-{issue-type}.md`

**Examples**:
- `.github/ai-feedback/2025-12-30-backend-developer-unclear-instruction.md`
- `.github/ai-feedback/2025-12-30-scrum-master-process-bottleneck.md`
- `.github/ai-feedback/2025-12-30-documentation-developer-conflict.md`

### Use the Template

See [Feedback Entry Template](../../.github/agents/scrum-master.agent.md#feedback-entry-template) in scrum-master.agent.md

**Sections**:
1. Issue Description + Context + Specific Example
2. Impact Assessment (who affected, cost, frequency)
3. Root Cause Analysis
4. Solution (immediate fix, recommended fix, alternatives)
5. Action Items + Owner + Timeline

### Be Specific

- Include real examples from actual work
- Assess severity honestly (🔴 Critical, 🟠 Major, 🟡 Minor, 🟢 Observation)
- Note frequency (first time, occasional, frequent)
- Explain root cause, not just symptom

---

## File Organization

Feedback is automatically organized by agent and type:

### by-agent/
Feedback grouped by affected agent:
```
by-agent/
├── backend-developer/
│   ├── 2025-12-30-build-timing-unclear.md
│   ├── 2025-12-31-wolverine-pattern-confusion.md
│   └── summary.md (monthly summary)
├── frontend-developer/
├── qa-engineer/
├── security-engineer/
├── devops-engineer/
├── scrum-master/
└── [other-agents]/
```

### by-type/
Feedback grouped by issue type:
```
by-type/
├── problems/
│   ├── 2025-12-30-build-timing-unclear.md
│   ├── 2025-12-30-encryption-key-rotation-broken.md
│   └── summary.md (monthly summary)
├── conflicts/
│   ├── 2025-12-30-documentation-authority-unclear.md
│   └── summary.md
└── unclear-behavior/
    ├── 2025-12-30-wolverine-routing-undocumented.md
    └── summary.md
```

### consolidated/
Monthly reports and trend analysis:
```
consolidated/
├── monthly-report-2025-12.md (Full month analysis)
├── monthly-report-2025-11.md
└── quarterly-trends-2025-Q4.md (Quarterly patterns)
```

---

## Feedback Types & Severity

### Issue Types

| Type | Example | Urgency |
|------|---------|---------|
| **Problem** | Build error, instruction bug, unexpected behavior | Varies |
| **Conflict** | Agent authority unclear, disagreement on process | High |
| **Unclear Behavior** | Ambiguous instruction, confusing docs | Medium |
| **Process Issue** | Workflow inefficiency, bottleneck | Medium |
| **Instruction Gap** | Missing guidance, incomplete examples | Low-Medium |

### Severity Levels

| Level | Example | Response |
|-------|---------|----------|
| 🔴 **Critical** | Blocks work, major impact, immediate fix needed | Submit same day |
| 🟠 **Major** | Affects team productivity, should fix soon | Submit within 3 days |
| 🟡 **Minor** | Inconvenience, can fix when convenient | Submit within 1 week |
| 🟢 **Observation** | Nice-to-have improvement, low impact | Submit when ready |

---

## Process Flow

```
┌─────────────────────────────────────────────────────┐
│ Agent encounters issue while working                │
│ (Or user reports issue to agent)                    │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ Document in .github/ai-feedback/                    │
│ - Follow template                                   │
│ - Be specific with examples                         │
│ - Assess severity and impact                        │
│ - Suggest solutions if known                        │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ @scrum-master collects feedback (weekly)            │
│ - Review new feedback entries                       │
│ - Organize by type/agent                            │
│ - Summarize findings                                │
│ - Identify patterns                                 │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ @scrum-master prioritizes & submits                 │
│ - Priority 1: Critical/blocking issues              │
│ - Priority 2: Major issues                          │
│ - Priority 3: Minor/observation items               │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ @process-assistant reviews & decides                │
│ - Valid? Consistent with existing process?          │
│ - Implement fix or request revision?                │
│ - Update instructions/workflow                      │
│ - Link back to feedback for traceability            │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ Resolution documented in feedback entry             │
│ - What was changed                                  │
│ - Link to updated file/PR                           │
│ - Mark status as Resolved                           │
└────────────┬────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────┐
│ Monthly report consolidates all feedback            │
│ - Patterns identified                               │
│ - Metrics tracked                                   │
│ - Next month improvements planned                   │
└─────────────────────────────────────────────────────┘
```

---

## Real-World Examples

See feedback entries in this folder for examples of:

- **Problems**: Build timing issues, encryption gaps, unclear instructions
- **Conflicts**: Authority disputes, dual documentation, process conflicts
- **Unclear Behavior**: Ambiguous instructions, missing documentation, confusing expectations

Each example shows:
- How the issue was discovered
- Why it matters (impact analysis)
- How root cause was identified
- What solution was recommended
- How resolution was tracked

---

## Who Can Document Feedback?

✅ **Anyone** can document feedback:
- @backend-developer, @frontend-developer, @qa-engineer
- @security-engineer, @devops-engineer, @scrum-master
- @process-assistant, @tech-lead, @product-owner
- **Users** (through agents) can report issues

**Best Practice**: Document as soon as you encounter the issue
- Don't wait for sprint end
- Don't wait for retrospective
- Real-time feedback enables quick fixes

---

## Integration with Agent Instructions

Feedback mechanism is documented in:
- [scrum-master.agent.md - AI Agent Feedback Documentation Section](../../.github/agents/scrum-master.agent.md#-ai-agent-feedback-documentation)

When agents encounter issues, they can:
1. Reference feedback documentation in scrum-master instructions
2. Create feedback entry following provided template
3. Trust that @scrum-master will collect and @process-assistant will act on it

---

## Metrics Tracked

@scrum-master tracks these metrics monthly:

- **Response Time**: Days from report to @process-assistant submission
- **Resolution Rate**: % of feedback implemented
- **Critical Issues**: 100% should be resolved
- **Average Time to Fix**: Days from feedback to implementation
- **Patterns**: Recurring issues identified and addressed
- **Quality**: Feedback specificity and actionability

---

## Quick Links

- **Agent Instructions**: [scrum-master.agent.md - Feedback Section](../../.github/agents/scrum-master.agent.md#-ai-agent-feedback-documentation)
- **Governance**: [GOVERNANCE_RULES.md](../../docs/processes/GOVERNANCE/GOVERNANCE_RULES.md)
- **Process Assistant Authority**: [process-assistant.agent.md](../../agents/process-assistant.agent.md)

---

## Questions?

- **How do I report an issue?** → Create file in `.github/ai-feedback/` using template
- **What if I'm not sure about severity?** → Describe honestly, provide context, let @scrum-master assess
- **What if my feedback isn't addressed?** → @scrum-master submits to @process-assistant, they decide and document reasoning
- **Can I report issues about other agents?** → Yes, but be specific and objective, focus on behavior/instruction not personality

---

**Last Updated**: 30. Dezember 2025  
**Maintained By**: @scrum-master (collection) + @process-assistant (implementation)  
**Status**: Active - Accepting feedback
