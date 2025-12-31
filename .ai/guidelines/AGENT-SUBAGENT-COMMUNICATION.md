# Agent-SubAgent Communication Guide

**Version:** 1.0  
**Created:** 30.12.2025  
**Purpose:** Detailed communication patterns and real-world examples for all Agent-SubAgent interactions

---

## Quick Reference

### When to Use Direct vs. Routed

```
Direct @SubAgent call when:
✅ Task is simple & well-defined
✅ No priority/routing decision needed
✅ Execution time < 10 minutes expected
✅ No quality gate required
✅ Single SubAgent capable

Example: @SubAgent-Testing "Generate unit tests for UserModel"

---

Route via @SARAH when:
✅ Task is complex/ambiguous
✅ Need priority management
✅ Execution might exceed SLA
✅ Quality gate needed
✅ Multiple SubAgents might be involved
✅ Risk of failures

Example: @SARAH route "Complete security audit of payment service"
```

---

## Communication Scenarios & Templates

### Scenario 1: Quick Research Task

#### Request (Direct to SubAgent)

```markdown
@SubAgent-Research
QUICK RESEARCH: GraphQL Federation best practices

What I need:
- Current recommendations for federation patterns
- Trade-offs between Apollo Federation & Hasura
- Implementation complexity estimate
- When to use vs. monolithic approach

Constraints:
- Keep it concise (1 page max)
- Focus on practical, not theoretical
- Current 2024 best practices

Output: .ai/research/graphql-federation-2024.md
```

#### Response

```markdown
@Backend
RESEARCH COMPLETE: GraphQL Federation

File: .ai/research/graphql-federation-2024.md

Summary:
Federation recommended for multi-team setups. Apollo Federation mature, 
Hasura easier but less flexible. ~2 weeks learning + implementation.

Key points documented in output file with decision matrix.

Time: 7 min | Quality: HIGH
```

---

### Scenario 2: Complex Security Audit (Routed via SARAH)

#### Request (to SARAH)

```markdown
@SARAH
DELEGATION REQUEST: Security audit of authentication module

Priority: HIGH
Deadline: EOD today

Details:
- Module: src/services/auth/
- Focus: OAuth2 implementation review
- Scope: Token handling, session management, logout flow
- Context: Using jsonwebtoken + Redis sessions

What we need:
- Security vulnerabilities (categorized by severity)
- Best practice recommendations
- Implementation effort for fixes
- Priority order for remediation

Success criteria:
- All OWASP Top 10 relevant items covered
- Confidence level stated for each finding
- Actionable recommendations

Related: Issue #SEC-234
```

#### SARAH's Routing

```markdown
✅ ROUTED: Request accepted

Delegation ID: DEL-2025-12-30-089
SubAgent: @SubAgent-Security
ETA: 12 minutes
Priority: HIGH

Will notify you on completion.
```

#### SubAgent Execution & Response

```markdown
@SARAH
DELEGATION COMPLETED: DEL-2025-12-30-089

Task: Authentication module security audit
Output: .ai/issues/SEC-234/auth-security-audit.md

FINDINGS SUMMARY:
🔴 HIGH: Token expiration not validated (fix: 30 min)
🔴 HIGH: CORS allows any origin (fix: 1 hour)
🟡 MEDIUM: Missing input validation (fix: 2 hours)
🟡 MEDIUM: Weak session ID generation (fix: 1 hour)
🟢 LOW: Error messages leak info (fix: 30 min)

Total remediation effort: 5-6 hours
Overall risk: MEDIUM → LOW (post-fixes)

Detailed analysis with code examples in output file.
Time: 11 min | Confidence: HIGH
```

#### SARAH's Quality Check & Forward

```markdown
@Backend
SECURITY AUDIT COMPLETE: Authentication module

Output: .ai/issues/SEC-234/auth-security-audit.md

✅ Quality check passed
✅ All findings documented
✅ Recommendations prioritized

Next steps:
1. Review findings (priority order in file)
2. Plan remediation (5-6 hours total)
3. Execute fixes with security team
4. Request follow-up audit after fixes

Questions? See detailed analysis file.
```

---

### Scenario 3: Code Generation with Iteration

#### Request

```markdown
@SubAgent-Testing
TASK: Generate API endpoint tests for order service

Scope:
Module: src/api/orders/
Endpoints to test:
- POST /orders (create)
- GET /orders/:id (retrieve)
- PATCH /orders/:id (update status)
- DELETE /orders/:id (cancel)

Requirements:
✅ Happy path for each endpoint
✅ Error cases (validation, auth, not found)
✅ Edge cases (concurrent updates, large payloads)
✅ Mock dependencies (database, payment service)
✅ Use existing test patterns

Output: tests/api/orders.integration.test.ts
Max lines: 400
```

#### SubAgent Response - Partial

```markdown
@Backend
IN PROGRESS: Order API tests (DEL-2025-12-30-XX)

Generated: tests/api/orders.integration.test.ts

Status: ✅ GENERATED (needs review)
Lines: 380
Coverage: 88%

Notes:
- All CRUD operations covered
- Happy path + main error cases
- Concurrent update scenario included
- Payment service mock set up

Issue found:
- Missing edge case: canceled order update
- Need to clarify expected behavior

Can I add test for "attempting to update canceled order"?
Or should I assume it returns 409 Conflict?

Awaiting your input...
```

#### Follow-up Request

```markdown
@SubAgent-Testing
CLARIFICATION: Re: canceled order update

Answer: Should return 409 Conflict with message "Order is canceled"

Please add test for this edge case and regenate file.
```

#### Final Response

```markdown
@Backend
TESTS COMPLETED: Order API tests

File: tests/api/orders.integration.test.ts

Status: ✅ READY FOR USE

Updates:
- Added canceled order update edge case (409)
- Lines: 395 | Coverage: 92%
- All test patterns match existing codebase
- Ready to run: npm test tests/api/orders.integration.test.ts

Files generate: ~3 seconds
Time: 13 min total | Quality: HIGH
```

---

### Scenario 4: Architecture Decision Research

#### Request (Complex - via SARAH)

```markdown
@SARAH
RESEARCH REQUEST: Microservices vs Monolithic for new backend

Context:
- Current: Monolithic Express.js (~150K LOC)
- Team size: 8 engineers
- Domain: E-commerce (orders, payments, inventory, shipping)
- Current pain points: Slow deployments, tight coupling

What we need:
1. Trade-off analysis: pros/cons of each approach
2. Migration path (if microservices): phased approach
3. Effort estimate (months)
4. Recommendation with confidence level

Success criteria:
- Addresses our specific context, not generic
- Considers team size & skills
- Includes timeline
- Recommends concrete approach

Deadline: 3 days
Priority: HIGH (blocks Q1 planning)
```

#### SARAH Routes to Primary + Secondary SubAgents

```markdown
@SubAgent-Architecture
DELEGATION: DEL-2025-12-30-120

Task: Microservices vs Monolithic analysis
Context: 150K LOC monolith, 8 engineers, e-commerce

Initial analysis scope:
- Architecture patterns comparison
- Team topology implications
- Technical considerations

Will route to @SubAgent-Security & @SubAgent-DevOps 
after you complete architectural analysis.

Output: .ai/decisions/ARCH-ARCH-001/microservices-analysis.md
```

#### Architecture SubAgent Response

```markdown
@SARAH
COMPLETED (PARTIAL): DEL-2025-12-30-120

Architectural analysis: .ai/decisions/ARCH-ARCH-001/microservices-analysis.md

Summary:
✅ Microservices viable but complex
✅ Monolithic sustainable with better practices
✅ Hybrid approach: good middle ground

Detailed comparison with pros/cons matrix included.

⏳ Needs: Security & DevOps input on:
- Security implications of distributed systems
- Deployment & infrastructure requirements

Ready for secondary routing?
```

#### SARAH Routes Secondary Reviews

```markdown
@SubAgent-Security
DELEGATION: DEL-2025-12-30-120-SEC

Task: Security implications review for microservices approach

Input: .ai/decisions/ARCH-ARCH-001/microservices-analysis.md

Scope:
- Security benefits/risks vs monolithic
- Authentication/authorization in distributed system
- Data isolation & secrets management
- Compliance considerations

Add findings to: .ai/decisions/ARCH-ARCH-001/microservices-analysis.md
(Section: Security Implications)
```

#### Consolidated Final Response

```markdown
@Backend
ARCHITECTURE DECISION ANALYSIS COMPLETE

File: .ai/decisions/ARCH-ARCH-001/microservices-analysis.md

RECOMMENDATION: Hybrid approach (3-phase)
- Phase 1: Refactor monolith (3 months)
- Phase 2: Extract 2-3 critical services (2-3 months)
- Phase 3: Evaluate & expand if beneficial (ongoing)

Key findings:
✅ Monolithic improvements cheaper, faster
✅ Microservices valuable for scaling later
✅ Hybrid gives option to evolve
⚠️ Security more complex with microservices
⚠️ DevOps investment needed (containers, orchestration)

Detailed analysis with cost/benefit matrix in output file.

Confidence: HIGH
Time: 45 min (architecture + security + devops)
Ready for: Board presentation / ADR creation
```

---

### Scenario 5: Documentation Generation Task

#### Request

```markdown
@SubAgent-Documentation
TASK: Generate API documentation for payment service

Scope:
- Module: src/api/payments/
- Format: OpenAPI 3.0 YAML
- Include: All endpoints, schemas, examples
- Coverage: Setup, usage examples, error codes

Success criteria:
✅ Complete endpoint definitions
✅ Request/response schemas
✅ Example requests for each endpoint
✅ Error codes documented
✅ Authentication requirements clear
✅ Valid OpenAPI (passes validation)

Output: docs/api/payments-openapi.yaml
Related: Issue #INFRA-567
```

#### Response with Issues

```markdown
@Backend
DOCUMENTATION COMPLETE: Payment API (OpenAPI)

File: docs/api/payments-openapi.yaml

Status: ✅ GENERATED & VALIDATED

Summary:
- 12 endpoints documented
- All request/response schemas included
- Examples for each operation
- Error codes with descriptions
- Authentication (Bearer JWT) documented

Validation:
✅ OpenAPI 3.0.0 valid
✅ All examples executable
✅ No missing required fields

Time: 10 min | Quality: HIGH

Can generate HTML/PDF viewer if needed. Docs ready for portal!
```

---

### Scenario 6: Error & Escalation

#### SubAgent Hits Problem

```markdown
@SARAH
DELEGATION BLOCKED: DEL-2025-12-30-156

Task: Performance analysis of user service
Error: Cannot access source code files

Details:
- Expected path: src/services/user/
- Actual: Path not found
- Tried alternatives: src/user/, services/user/, backend/user/

Cannot proceed without code access.

Options:
1. Verify correct path
2. Provide code context directly
3. Retry with different scope

Waiting for guidance...
```

#### SARAH Resolves & Re-routes

```markdown
@SubAgent-Architecture
RESTARTED: DEL-2025-12-30-156

Correct path: src/services/v2/user/
Code now accessible.

Continue with performance analysis.
Output location: .ai/analysis/user-service-perf.md
```

#### SubAgent Completes After Re-route

```markdown
@SARAH
COMPLETED (AFTER REROUTE): DEL-2025-12-30-156

Performance analysis: .ai/analysis/user-service-perf.md

Summary:
- 3 N+1 queries identified
- Database indexes missing (users, profiles)
- Cache not utilized
- Estimated 60% improvement possible

Implementation: 1-2 days
Priority: MEDIUM

Time: 12 min (including restart) | Quality: HIGH
```

---

## Common Issues & Solutions

### Issue: Vague Request

**Bad Request:**
```markdown
@SubAgent-Research
Can you analyze our database?
```

**Good Request:**
```markdown
@SubAgent-Research
ANALYSIS: PostgreSQL query performance

Scope:
- Tables: users, orders, order_items, products
- Focus: Missing indexes, slow queries
- Constraints: < 20K records per table, < 100 queries

Output: .ai/analysis/db-perf-analysis.md
Deadline: Tomorrow 12:00
```

---

### Issue: Missing Context

**When SubAgent can't proceed:**
```markdown
@Backend
BLOCKED: Database analysis

Missing info:
1. What databases to analyze? (PostgreSQL, MongoDB, etc.)
2. What performance issues are happening?
3. Do you have slow query logs available?
4. What's the expected load?

Please provide context to continue.
```

**Prevention:** Include all details in initial request

---

### Issue: Scope Creep

**SubAgent detects scope expansion:**
```markdown
@Requesting-Agent
CLARIFICATION NEEDED: DEL-2025-12-30-XX

Original task: Analyze user authentication module

Request now includes:
- Authorization system (different scope)
- API rate limiting (different module)
- Password reset flow (related but separate)

Should I:
1. Stick to original auth module scope?
2. Expand to all security modules?
3. Create separate delegations for each?

Waiting for clarity before continuing...
```

---

### Issue: Quality Not Met

**SARAH detects quality issues:**
```markdown
@SubAgent-Testing
QUALITY GATE FAILED: DEL-2025-12-30-156

Output: tests/order.test.ts

Issues found:
- Missing test for main happy path
- Error case tests are incomplete
- No mock setup for dependencies
- Coverage only 62% (target: >85%)

Please revise and resubmit.
```

---

## Communication Checklist

### For Every Request

- [ ] **Scope:** Clearly defined
- [ ] **Context:** All needed information provided
- [ ] **Success criteria:** How to know it's done
- [ ] **Output location:** Where to write results
- [ ] **Deadline:** Realistic timeframe
- [ ] **Priority:** HIGH/NORMAL/LOW
- [ ] **Format:** Specific format requested
- [ ] **Constraints:** Size/time/quality limits

### For Every Response

- [ ] **Acknowledgment:** Task understood
- [ ] **Output file:** Created and readable
- [ ] **Summary:** Key findings in message
- [ ] **Quality metrics:** Time, confidence level
- [ ] **Next steps:** What to do with results
- [ ] **Caveats:** Any limitations/assumptions
- [ ] **Contact:** How to ask follow-ups

---

## Best Practices Summary

1. **Be Specific:** Vague requests → Vague responses
2. **Include Context:** Once, completely, clearly
3. **Set Expectations:** Deadline, quality, format
4. **Use Right Channel:** SARAH for complex, direct for simple
5. **Acknowledge Issues:** Escalate early when blocked
6. **Quality Check:** Verify output against criteria
7. **Provide Feedback:** Help improve SubAgent accuracy
8. **Document Lessons:** Share learnings with team

---

**Related Documents:**
- [SARAH-SUBAGENT-COORDINATION.md](SARAH-SUBAGENT-COORDINATION.md)
- [.github/agents/SARAH.agent.md](../../.github/agents/SARAH.agent.md)
- [.github/prompts/subagent-delegation.prompt.md](../../.github/prompts/subagent-delegation.prompt.md)
