---
docid: GL-058
title: COMMUNICATION VISUAL GUIDE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Agent-SubAgent Communication - Visual Guide

Quick visual reference for all communication patterns and workflows.

---

## Communication Decision Tree

```
┌─────────────────────────────────────────────────────────┐
│ "I need help from a SubAgent. Which path do I take?"    │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
        ┌─────────────────────────────────────┐
        │ Is the task simple & well-defined?  │
        └─────────────────────────────────────┘
                   │              │
               YES │              │ NO
                   ▼              ▼
        ┌──────────────────────┐ ┌──────────────────────┐
        │ Can execute <10 min? │ │ Use @SARAH routing   │
        └──────────────────────┘ │ (complex task)       │
              │         │        └──────────────────────┘
          YES │         │ NO
              ▼         ▼
    ┌──────────────┐ ┌──────────────┐
    │ Direct call: │ │ Route via:   │
    │ @SubAgent-   │ │ @SARAH       │
    │ {Type}       │ │ delegation   │
    └──────────────┘ └──────────────┘
```

---

## Request Format Templates

### 📌 Direct SubAgent Request (Simple)

```
┌─────────────────────────────────────────────┐
│ Direct Request (Simple Task, <10 min)       │
├─────────────────────────────────────────────┤
│                                             │
│ @SubAgent-{Type}                            │
│ TASK: [Clear, specific task]                │
│                                             │
│ Context:                                    │
│ - [What to do]                              │
│ - [Key constraints]                         │
│ - [Success criteria]                        │
│                                             │
│ Output: [file/path]                         │
│                                             │
└─────────────────────────────────────────────┘

Response: <10 minutes
```

---

### 📌 Routed Request (Complex/Priority)

```
┌─────────────────────────────────────────────┐
│ Routed Request (Complex/Priority Task)      │
├─────────────────────────────────────────────┤
│                                             │
│ @SARAH                                      │
│ DELEGATION REQUEST:                         │
│                                             │
│ Task: [Task description]                    │
│ Priority: HIGH | NORMAL | LOW               │
│ Deadline: [Date Time]                       │
│                                             │
│ Details:                                    │
│ - Scope: [What's included]                  │
│ - Context: [Background info]                │
│ - Success criteria: [Done = ?]              │
│ - Output: [file/path]                       │
│                                             │
└─────────────────────────────────────────────┘

Response: SARAH acknowledges in <2 min
         SubAgent delivers in <10 min
```

---

## Response Format Template

```
┌─────────────────────────────────────────────┐
│ SubAgent Response (Standard Format)         │
├─────────────────────────────────────────────┤
│                                             │
│ @Requesting-Agent                           │
│ STATUS: ✅ COMPLETED | ⚠️ PARTIAL | ❌ FAILED
│                                             │
│ Output: [file location]                     │
│                                             │
│ Summary: [2-3 sentence key findings]        │
│                                             │
│ Key findings:                               │
│ • Finding 1                                 │
│ • Finding 2                                 │
│ • Finding 3                                 │
│                                             │
│ Metrics:                                    │
│ • Time: X min                               │
│ • Quality/Confidence: HIGH|MEDIUM|LOW       │
│ • Files affected: N                         │
│                                             │
└─────────────────────────────────────────────┘

Everything needed in 1 response message
```

---

## Workflow Diagram

```
DIRECT PATH (Simple Tasks)

  Agent              SubAgent
    │                  │
    ├─── Request ────>│
    │                  ├─ Execute
    │                  │
    │<─── Response ────┤
    │                  │
    └─ Use Results    │

Time: ~6-10 minutes
Routing overhead: NONE
Quality gate: Optional


ROUTED PATH (Complex Tasks)

  Agent              SARAH           SubAgent
    │                  │                │
    ├─ Delegation ──> │                │
    │                  ├─ Routing ──-─>│
    │                  │                ├─ Execute
    │                  │<─ Complete ────┤
    │<─ Forward ─────┤
    │                  │                │
    └─ Use Results    │                │

Time: ~8-15 minutes
Routing overhead: 1-2 minutes
Quality gate: YES (SARAH)


MULTI-AGENT PATH (Very Complex)

  Agent              SARAH           SubAgent-1      SubAgent-2
    │                  │                  │                │
    ├─ Request ──────>│                  │                │
    │                  ├─ Route 1 ──────>│                │
    │                  │                  ├─ Execute      │
    │                  │<─ Result 1 ──────┤                │
    │                  │                  │                │
    │                  ├─ Route 2 ───────────────────────>│
    │                  │                  │                ├─ Execute
    │                  │<────────────────────── Result 2 ──┤
    │                  │                  │                │
    │<─ Consolidated ┤                  │                │
    │                  │                  │                │
    └─ Use Results    │                  │                │

Time: ~20-30 minutes
Routing overhead: 3-5 minutes
Quality gate: YES (SARAH)
Complexity: HIGH
```

---

## Interaction Matrix

```
┌──────────────┬──────────────┬──────────┬─────────────────┐
│ Task Type    │ SubAgent     │ Direct?  │ SLA             │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Quick        │ Research     │ ✅ YES   │ < 10 min        │
│ research     │              │          │                 │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Unit tests   │ Testing      │ ✅ YES   │ < 15 min        │
│ generation   │              │          │                 │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Security     │ Security     │ ⚠️  BOTH | < 15 min       │
│ review       │              │          │ (quality gate)  │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ API docs     │ Documentation│ ✅ YES   │ < 10 min        │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Code review  │ Review       │ ⚠️ BOTH  │ < 12 min        │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Architecture │ Architecture │ 🔴 SARAH | < 20 min       │
│ decision     │              │ (complex)│ (complex)       │
├──────────────┼──────────────┼──────────┼─────────────────┤
│ Performance  │ Optimization │ ⚠️ BOTH  │ < 15 min        │
│ optimization │              │          │                 │
└──────────────┴──────────────┴──────────┴─────────────────┘

✅ YES  = Direct call recommended
⚠️ BOTH = Either works, SARAH for priority
🔴 SARAH = Route via SARAH (complex)
```

---

## Communication Layers

```
┌────────────────────────────────────────────────────────┐
│          AGENT-SUBAGENT COMMUNICATION SYSTEM           │
├────────────────────────────────────────────────────────┤
│                                                        │
│  LAYER 1: REQUEST FORMAT                              │
│  ├─ Task definition                                   │
│  ├─ Scope boundaries                                  │
│  ├─ Success criteria                                  │
│  ├─ Output location                                   │
│  └─ Priority level                                    │
│                    │                                   │
│  LAYER 2: ROUTING DECISION                            │
│  ├─ Direct to SubAgent (simple tasks)                 │
│  ├─ Via SARAH (complex/priority)                      │
│  └─ Multi-agent cascade (very complex)                │
│                    │                                   │
│  LAYER 3: EXECUTION                                   │
│  ├─ SubAgent executes task                            │
│  ├─ Saves results to location                         │
│  ├─ Tracks metrics (time, quality)                    │
│  └─ Handles errors/escalations                        │
│                    │                                   │
│  LAYER 4: RESPONSE FORMAT                             │
│  ├─ Status (✅/⚠️/❌)                                  │
│  ├─ Output file location                              │
│  ├─ Summary (key findings)                            │
│  ├─ Metrics (time, confidence)                        │
│  └─ Next steps                                        │
│                    │                                   │
│  LAYER 5: QUALITY GATE                                │
│  ├─ Verify completeness                               │
│  ├─ Check format compliance                           │
│  ├─ Validate accuracy                                 │
│  ├─ Check success criteria                            │
│  └─ Escalate if needed                                │
│                    │                                   │
│  LAYER 6: IMPLEMENTATION                              │
│  ├─ Requesting agent reviews                          │
│  ├─ Implements recommendations                        │
│  ├─ Provides feedback                                 │
│  └─ Closes delegation                                 │
│                                                        │
└────────────────────────────────────────────────────────┘
```

---

## Success Criteria Checklist

```
REQUEST CHECKLIST (Before sending)
┌─────────────────────────────────────┐
│ ☐ Scope clearly defined             │
│ ☐ Context/background provided       │
│ ☐ Success criteria specified         │
│ ☐ Output location clear              │
│ ☐ Deadline set                       │
│ ☐ Priority level stated              │
│ ☐ Any constraints mentioned          │
│ ☐ Related files/issues referenced    │
└─────────────────────────────────────┘

RESPONSE CHECKLIST (After receiving)
┌─────────────────────────────────────┐
│ ☐ File exists at location            │
│ ☐ Content is complete                │
│ ☐ Format is correct                  │
│ ☐ Matches success criteria            │
│ ☐ Conclusions are supported          │
│ ☐ Time is reasonable                 │
│ ☐ Summary is clear                   │
│ ☐ Confidence level stated            │
│ ☐ Next steps clear                   │
│ ☐ No obvious errors                  │
└─────────────────────────────────────┘
```

---

## SubAgent Types at a Glance

```
@SubAgent-Research      📚  Research, analysis, documentation
                            └─ 5-10 minutes

@SubAgent-Testing       ✅  Unit tests, integration tests
                            └─ 5-15 minutes

@SubAgent-Security      🔒  Security audits, vulnerabilities
                            └─ 8-15 minutes

@SubAgent-Documentation 📄  API docs, README, OpenAPI
                            └─ 5-10 minutes

@SubAgent-Review        👀  Code review, design review
                            └─ 8-12 minutes

@SubAgent-Architecture  🏗️   Design analysis, tech decisions
                            └─ 10-15 minutes

@SubAgent-Optimization  ⚡  Performance, refactoring
                            └─ 10-15 minutes
```

---

## Priority Mapping

```
PRIORITY → SLA REQUIREMENT

┌─────────────┬───────────┬─────────────────┐
│ Priority    │ SLA       │ Use Case        │
├─────────────┼───────────┼─────────────────┤
│ CRITICAL    │ < 5 min   │ Blocking issues │
│ 🔴 Red      │           │                 │
├─────────────┼───────────┼─────────────────┤
│ HIGH        │ < 10 min  │ Important tasks │
│ 🟠 Orange   │           │                 │
├─────────────┼───────────┼─────────────────┤
│ NORMAL      │ < 15 min  │ Standard tasks  │
│ 🟡 Yellow   │           │                 │
├─────────────┼───────────┼─────────────────┤
│ LOW         │ < 30 min  │ Nice to have    │
│ 🟢 Green    │           │                 │
└─────────────┴───────────┴─────────────────┘
```

---

## Communication Health Checklist

```
DAILY COMMUNICATION HEALTH

🔍 Request Quality
   ☐ Requests include all 5 required elements
   ☐ Scope is clear and specific
   ☐ Success criteria are measurable
   ☐ Output locations are correct

✅ Response Quality
   ☐ Responses follow standard format
   ☐ Output files exist and are readable
   ☐ Summaries are clear and actionable
   ☐ Confidence levels are stated

⏱️  Timing
   ☐ Direct requests: < 10 min avg
   ☐ Routed requests: < 12 min total
   ☐ No SLA violations
   ☐ Priority escalations handled

🎯 Success Rate
   ☐ > 95% of responses meet criteria
   ☐ < 2% re-request rate
   ☐ < 2% error rate
   ☐ 100% uptime
```

---

## Quick Troubleshooting

```
PROBLEM → SOLUTION

No response?
 → Check if SARAH received routing confirmation
 → Verify SubAgent type is correct
 → Check deadline hasn't been exceeded

Output doesn't match success criteria?
 → Escalate to SARAH for quality gate
 → Provide specific gap feedback
 → Resubmit with refined criteria

SubAgent is blocked?
 → Provide missing context immediately
 → Clarify ambiguous scope
 → Escalate to SARAH

Taking too long?
 → Interrupt if exceeds SLA by 50%
 → Check if task scope changed
 → Escalate to SARAH for investigation

Low quality output?
 → Verify success criteria were clear
 → Check if SubAgent asked clarifications (unanswered)
 → Request SARAH quality gate review
 → Escalate if pattern repeats
```

---

## Key Numbers to Remember

```
📊 PERFORMANCE TARGETS

SubAgent Response Time
  └─ Target: < 10 minutes
  └─ Average: 6 minutes
  └─ SLA: Depends on priority

Main Agent Context Size
  └─ Target: < 10 KB
  └─ Current: 8 KB average
  └─ Reduction: 68% by using delegations

Quality Metrics
  └─ Score target: > 95%
  └─ Accuracy target: > 95%
  └─ Completeness: 100%

Uptime & Reliability
  └─ Target: 100%
  └─ Error rate: < 2%
  └─ Re-request rate: < 2%

Token Efficiency
  └─ Savings target: 35-40%
  └─ Context reduction: 60-70%
```

---

**Related Documentation:**
- Full guide: [AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md)
- Quick reference: [AGENT-SUBAGENT-CHEATSHEET.md](AGENT-SUBAGENT-CHEATSHEET.md)
- Coordination: [SARAH-SUBAGENT-COORDINATION.md](SARAH-SUBAGENT-COORDINATION.md)
