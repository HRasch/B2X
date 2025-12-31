# Agent-SubAgent Communication Cheatsheet

Quick reference for all Copilot agents when communicating with SubAgents.

---

## Direct SubAgent Call (Simple Tasks)

```markdown
@SubAgent-{Type}
TASK: [Clear, specific task]

Context:
- What to analyze/generate/fix
- Key constraints
- Expected format/output

Output: [specific file path]
```

**Response expected in:** < 10 minutes

---

## Route via SARAH (Complex/Priority Tasks)

```markdown
@SARAH
DELEGATION REQUEST:

Task: [Task description]
Priority: HIGH | NORMAL | LOW
Deadline: [Date Time]

Details:
- Scope: [What's included]
- Context: [Relevant background]
- Success criteria: [How to know it's done]

Output: [File path]
```

**Response expected in:** < 5 minutes (routing) + execution time

---

## Receive Response from SubAgent

The response will follow this format:

```markdown
@Your-Agent
STATUS: ✅ COMPLETED | ⚠️ PARTIAL | ❌ FAILED

Output: [file location]

Summary: [Key findings/results - 2-3 sentences]

Key points:
- Point 1
- Point 2
- Point 3

Metrics:
- Time: X min
- Quality: [confidence level]
- [Other relevant metrics]
```

---

## SubAgent Types Quick Guide

| SubAgent | Best for | Typical time |
|----------|----------|--------------|
| **@SubAgent-Research** | Market research, technology analysis, documentation review | 5-10 min |
| **@SubAgent-Testing** | Unit tests, integration tests, test coverage | 5-15 min |
| **@SubAgent-Security** | Security audits, vulnerability analysis, compliance | 8-15 min |
| **@SubAgent-Documentation** | API docs, README, inline docs, OpenAPI specs | 5-10 min |
| **@SubAgent-Review** | Code review, design review, quality checks | 8-12 min |
| **@SubAgent-Architecture** | Design analysis, pattern recommendations, tech decisions | 10-15 min |
| **@SubAgent-Optimization** | Performance optimization, refactoring suggestions, cleanup | 10-15 min |

---

## Communication Priorities

### What MUST be in every request:

1. ✅ **Clear task definition** - What exactly is needed
2. ✅ **Scope boundaries** - What's in/out
3. ✅ **Success criteria** - How to know it's complete
4. ✅ **Output location** - Where to save results
5. ✅ **Deadline** - When it's needed

### What SHOULD be in requests:

6. ✅ Context/background information
7. ✅ Constraints (time, size, quality)
8. ✅ Priority level
9. ✅ Related files/issues
10. ✅ Any known limitations

---

## Decision Tree: Direct vs Routed?

```
Is the task simple & well-defined?
├─ YES → Can execute in <10 min? 
│   ├─ YES → Direct @SubAgent call
│   └─ NO → Route via @SARAH
│
└─ NO (Complex/ambiguous)
    └─ Route via @SARAH
```

**Examples:**

Direct calls:
- "Generate unit tests for PaymentService"
- "Research GraphQL federation best practices"
- "Fix the typo in component.tsx"

Routed calls:
- "Complete security audit of auth module"
- "Analyze microservices vs monolithic for our platform"
- "Help me refactor the entire API layer"

---

## Common Response Patterns

### Success Response
```markdown
@Backend
COMPLETED: [task name]

Output: .ai/path/to/file.md

Summary: [Key result in 1-2 sentences]

Time: 8 min | Quality: HIGH
```

### Partial Response (Need clarification)
```markdown
@Backend
CLARIFICATION NEEDED: [task name]

Question: [What you need to know]

Options:
1. [Option A]
2. [Option B]

Waiting for your input...
```

### Failed/Blocked Response
```markdown
@SARAH
DELEGATION BLOCKED: [task ID]

Reason: [Why it can't continue]

Suggestions:
1. [Fix option 1]
2. [Fix option 2]

Awaiting your guidance...
```

---

## Quality Checklist for Responses

Before accepting SubAgent output, verify:

- [ ] File exists at specified location
- [ ] Content is complete (no truncations)
- [ ] Format is correct (markdown/JSON/code/etc)
- [ ] Matches success criteria from request
- [ ] Conclusions are supported by findings
- [ ] Time spent is reasonable
- [ ] Summary is actionable
- [ ] Confidence level matches quality

---

## Escalation Guide

**Escalate to @SARAH immediately if:**

🚨 SubAgent response exceeds SLA by >25%
🚨 Output doesn't meet success criteria
🚨 Quality issues (hallucinations, gaps)
🚨 Context/data unavailable
🚨 Security/compliance concerns
🚨 Multiple SubAgents needed

**Message template:**
```markdown
@SARAH
ESCALATION: [task ID] requires investigation

Issue: [What went wrong]
Impact: [Why it matters]
Attempted solutions: [What didn't work]

Need: [What would resolve it]
```

---

## 30-Second Communication Tips

✅ **BE SPECIFIC** — Vague requests = vague results
✅ **INCLUDE ALL CONTEXT** — Once, completely
✅ **SET EXPECTATIONS** — Deadline, format, quality
✅ **USE RIGHT CHANNEL** — SARAH if uncertain
✅ **PROVIDE FEEDBACK** — Tell them what worked/didn't
✅ **ESCALATE EARLY** — Don't wait for SLA to expire
✅ **VERIFY OUTPUT** — Check against success criteria
✅ **CLOSE LOOP** — Confirm you received results

---

## Example Requests (Copy & Modify)

### Research Task
```markdown
@SubAgent-Research
TOPIC: [What to research]

Need:
- [Specific finding 1]
- [Specific finding 2]
- [Specific finding 3]

Constraints: < 1 page, practical focus, 2024+ best practices

Output: .ai/research/[topic].md
```

### Testing Task
```markdown
@SubAgent-Testing
MODULE: [What to test]

Scenarios:
- Happy path: [main flow]
- Errors: [error cases]
- Edge cases: [edge cases]

Coverage target: >85%
Output: tests/[module].test.ts
```

### Security Audit
```markdown
@SubAgent-Security
MODULE: [What to audit]

Focus areas:
- [Security concern 1]
- [Security concern 2]
- [Security concern 3]

Output: .ai/security/[audit].md
```

### Architecture Analysis
```markdown
@SubAgent-Architecture
DECISION: [Architecture choice to analyze]

Compare: [Option A] vs [Option B]

Consider:
- Team size (X engineers)
- Scale requirements
- Current pain points
- Timeline constraints

Output: .ai/decisions/[decision].md
```

---

## Timing Expectations

**Execution Time Estimates:**
- Simple research: 5-7 minutes
- Code generation: 7-12 minutes
- Testing/audits: 10-15 minutes
- Complex analysis: 12-20 minutes

**Total time (including SARAH routing):** Add 1-2 minutes if routed via SARAH

**Response delay acceptable:**
- NORMAL: < 10 min (queue processing)
- PRIORITY: < 5 min (jumped queue)
- CRITICAL: < 2 min (immediate)

---

## Do's and Don'ts

### ✅ DO
- Ask specific questions
- Provide context upfront
- Set realistic deadlines
- Describe success clearly
- Escalate early if blocked

### ❌ DON'T
- Make vague requests
- Send context incrementally
- Change scope mid-task
- Expect mind-reading
- Wait until deadline passes

---

## Getting Better Responses

**Request Quality → Response Quality**

GOOD REQUEST:
```
Task: Review OAuth2 implementation in auth service
Focus: Token handling, refresh token rotation, session management
Scope: src/services/auth/
Success = list of vulnerabilities by severity + fixes
Output: .ai/security/auth-review.md
Deadline: Today 17:00
```

vs. BAD REQUEST:
```
Can you check the auth stuff?
```

💡 **The more specific your request, the better the response!**

---

**Need more examples?** → See [AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md)  
**Need coordination guidelines?** → See [SARAH-SUBAGENT-COORDINATION.md](SARAH-SUBAGENT-COORDINATION.md)
