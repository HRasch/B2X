---
docid: GL-060
title: GL 001 COMMUNICATION OVERVIEW
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Agent-SubAgent Communication Overview

**Status:** ✅ GUIDELINES DEFINED  
**Last Updated:** 30.12.2025  
**Scope:** Communication patterns for all Copilot agent-subagent interactions

---

## What's New

Comprehensive guidelines for how agents in Copilot communicate with their SubAgents have been defined across three complementary documents:

### 📚 Three-Part Documentation

#### 1. **SARAH-SUBAGENT-COORDINATION.md**
**Purpose:** Coordination framework and SARAH's role

**Covers:**
- SARAH's responsibilities (agent lifecycle, delegation routing, quality gating, monitoring)
- Agent creation and removal process
- Delegation request templates
- Communication protocol with 3 patterns
- Communication rules & guidelines (10 core rules)
- Communication best practices
- Complete workflow diagram
- Success metrics & KPIs

**Best for:** Understanding the overall system and SARAH's coordinating role

---

#### 2. **AGENT-SUBAGENT-COMMUNICATION.md**
**Purpose:** Detailed patterns with real-world examples

**Covers:**
- Quick reference (direct vs. routed decisions)
- 6 complete communication scenarios with full examples:
  - Quick research tasks
  - Complex security audits
  - Code generation with iteration
  - Architecture decision research
  - Documentation generation
  - Error handling & escalation
- Common issues & solutions
- Communication checklist

**Best for:** Learning through examples and understanding common patterns

---

#### 3. **AGENT-SUBAGENT-CHEATSHEET.md**
**Purpose:** Quick reference for busy agents

**Covers:**
- Direct SubAgent call format (< 10 lines)
- Route via SARAH format (< 10 lines)
- Response format expectation
- SubAgent types quick guide
- Decision tree (direct vs routed)
- Common response patterns
- Quality checklist
- Escalation guide
- 30-second tips
- Copy-paste request templates

**Best for:** Quick reference while working, no deep reading needed

---

## Communication Patterns

### Two Main Interaction Modes

#### 🔹 Mode 1: Direct Request to SubAgent
**When:** Task is simple, well-defined, < 10 min execution
**How:** `@SubAgent-{Type} TASK: [description]`
**Time:** 5-15 minutes total

```markdown
@SubAgent-Testing
TASK: Generate unit tests for UserModel
[... details ...]
```

#### 🔹 Mode 2: Routed via SARAH
**When:** Complex task, priority handling, quality gating needed
**How:** `@SARAH DELEGATION REQUEST: [details]`
**Time:** 5-25 minutes total (including routing)

```markdown
@SARAH
DELEGATION REQUEST: Security audit of auth module
[... details ...]
```

---

## 10 Core Communication Rules

| Rule | Requirement |
|------|-------------|
| **1. Mention Pattern** | Use `@Agent` at start; route complex via @SARAH |
| **2. Context Provision** | Always include scope, constraints, success criteria, output path, priority |
| **3. Response Format** | Status + file + summary + metrics + next steps |
| **4. Priority Handling** | CRITICAL <5min, HIGH <10min, NORMAL <15min, LOW <30min |
| **5. Context Management** | Main agent provides all context once; SubAgent doesn't ask for re-sends |
| **6. Error Communication** | Specific error type, root cause, suggestions, recommendation |
| **7. Handoff Protocol** | For multi-agent tasks: Agent → SARAH → Primary → Secondary → SARAH → Agent |
| **8. Documentation** | Every response must include output location, timestamp, confidence, next steps |
| **9. Escalation Triggers** | Time exceeded, quality issues, context problems, security concerns |
| **10. Quality Assurance** | SubAgent verifies: completeness, accuracy, format, success criteria match |

---

## Key Communication Elements

### Every Request MUST Include ✅
```
✅ Scope - What exactly is needed
✅ Constraints - Time, size, complexity limits
✅ Success criteria - How to know it's done
✅ Output location - Where to save
✅ Priority level - HIGH/NORMAL/LOW
```

### Every Response MUST Include ✅
```
✅ Acknowledgment - Task understood
✅ Status - COMPLETED/PARTIAL/FAILED
✅ Output file location - Where to read
✅ Summary - Key findings (1-3 sentences)
✅ Quality metrics - Time, confidence level
✅ Next steps - What to do with results
```

---

## SubAgent Types Guide

| Type | Specialization | Typical Tasks | Execution |
|------|---|---|---|
| **@SubAgent-Research** | Information gathering | Technology analysis, market research, documentation review | 5-10 min |
| **@SubAgent-Testing** | Quality assurance | Unit tests, integration tests, test coverage analysis | 5-15 min |
| **@SubAgent-Security** | Security analysis | Vulnerability audits, compliance checks, security reviews | 8-15 min |
| **@SubAgent-Documentation** | Tech writing | API docs, README, OpenAPI specs, inline documentation | 5-10 min |
| **@SubAgent-Review** | Quality checks | Code review, design review, best practices | 8-12 min |
| **@SubAgent-Architecture** | System design | Design analysis, pattern recommendations, tech decisions | 10-15 min |
| **@SubAgent-Optimization** | Performance | Refactoring, cleanup, performance optimization | 10-15 min |

---

## Decision Flow

```
┌─ Need SubAgent help?
│
├─ Is task simple & well-defined?
│  ├─ YES ➜ Can do in <10 min? ➜ YES ➜ Direct @SubAgent call
│  │                                  
│  │                                └─ NO ➜ Route via @SARAH
│  │
│  └─ NO (Complex/ambiguous) ➜ Route via @SARAH
│
└─ Format request with all 5 required elements
   └─ Send to appropriate channel
      └─ Receive response with all 6 required elements
         └─ Verify against success criteria
            └─ Use results or escalate if needed
```

---

## Communication Workflow

```
┌─────────────────────────────────────────────────┐
│ Agent-SubAgent Communication Workflow           │
└─────────────────────────────────────────────────┘

1️⃣ REQUEST PREPARATION (Requesting Agent)
   ✓ Define clear scope
   ✓ Include all context
   ✓ Set deadline & priority
   ✓ Specify output location

2️⃣ SUBMISSION (Requesting Agent)
   Direct to SubAgent: Simple tasks
   Via @SARAH: Complex/priority tasks

3️⃣ ROUTING (SARAH - if applicable)
   ✓ Validate request completeness
   ✓ Select appropriate SubAgent
   ✓ Create delegation record
   ✓ Notify SubAgent with context

4️⃣ EXECUTION (SubAgent)
   ✓ Acknowledge receipt
   ✓ Ask clarifying questions if needed
   ✓ Execute task
   ✓ Save results to location

5️⃣ RESPONSE (SubAgent)
   ✓ Deliver summary message
   ✓ Provide key findings
   ✓ Reference output file
   ✓ Include quality metrics

6️⃣ QUALITY GATE (SARAH - if routed)
   ✓ Verify output completeness
   ✓ Check format/quality
   ✓ Validate against criteria
   ✓ Forward to requesting agent

7️⃣ IMPLEMENTATION (Requesting Agent)
   ✓ Review output file
   ✓ Implement recommendations
   ✓ Provide feedback if needed
   ✓ Close delegation
```

---

## Success Metrics

### Execution Performance
- ✅ Main agent context: < 10 KB
- ✅ SubAgent response: < 10 min (avg 6 min)
- ✅ Quality score: > 95%
- ✅ Uptime: 100%
- ✅ Token savings: 35-40%
- ✅ Error rate: < 2%
- ✅ Communication clarity: < 2% re-requests

### Quality Indicators
- ✅ Output completeness: 100%
- ✅ Accuracy: > 95%
- ✅ Format compliance: 100%
- ✅ Documentation: Always present
- ✅ Actionability: High

### Adoption Rate
- ✅ Target: > 60% of tasks delegated
- ✅ Current: Pilot week exceeded targets

---

## Quick Start for Agents

### If you need something from a SubAgent:

1. **Check the Cheatsheet** → [AGENT-SUBAGENT-CHEATSHEET.md](AGENT-SUBAGENT-CHEATSHEET.md)
2. **Find your request template** → Copy & customize
3. **Identify SubAgent type** → Simple task or complex?
4. **Send message** → Direct or via SARAH
5. **Receive results** → In specified file location
6. **Verify quality** → Against success criteria
7. **Escalate if issues** → Contact SARAH with details

---

## Common Scenarios

**"I need quick research"** → Direct: `@SubAgent-Research`  
**"I need security audit"** → Routed: `@SARAH Delegation Request`  
**"Generate unit tests"** → Direct: `@SubAgent-Testing`  
**"Complex architecture decision"** → Routed: `@SARAH`  
**"Write API documentation"** → Direct: `@SubAgent-Documentation`  
**"Code review needed"** → Direct/Routed: Either works  

---

## Resources

### 📖 Full Documentation
- **[SARAH-SUBAGENT-COORDINATION.md](SARAH-SUBAGENT-COORDINATION.md)** — Complete coordination guide
- **[AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md)** — Detailed patterns with examples
- **[AGENT-SUBAGENT-CHEATSHEET.md](AGENT-SUBAGENT-CHEATSHEET.md)** — Quick reference card

### 🎯 For SARAH
- **[SARAH.agent.md](../../.github/agents/SARAH.agent.md)** — Agent definition
- **[subagent-delegation.prompt.md](../../.github/prompts/subagent-delegation.prompt.md)** — Routing prompts

### 🔧 Related Guidelines
- **[SUBAGENT_DELEGATION.md](SUBAGENT_DELEGATION.md)** — SubAgent capabilities
- **[subagent-delegation.workflow.md](../../.ai/workflows/subagent-delegation.workflow.md)** — Workflow details

---

## Key Takeaways

1. **Agents have two ways to request help:** Direct for simple, routed via SARAH for complex
2. **Every request must have:** Scope, constraints, success criteria, output path, priority
3. **Every response includes:** Status, output file, summary, metrics, next steps
4. **SARAH coordinates:** Routing, quality gating, performance monitoring, escalations
5. **Quality is built-in:** Success criteria specified, metrics tracked, escalation when needed
6. **Communication is explicit:** No assumptions, all context provided upfront, clear formats

---

**Status:** ✅ Guidelines complete and operational  
**Version:** 1.0  
**Effective:** 30.12.2025  
**Maintained by:** @SARAH  

**Questions?** Refer to [AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md) for detailed examples.
