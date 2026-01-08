---
docid: UNKNOWN-063
title: PHASE1_TRAINING_MATERIALS
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# Phase 1 Training Materials

**Duration**: 45 minutes  
**Audience**: @Backend, @Frontend, @QA, @Security, @Legal teams  
**Format**: Live structured training with interactive Q&A  
**Date**: Execution day (structured for immediate delivery)  

---

## Training Agenda (45 minutes)

### Segment 1: System Overview (8 minutes)

**Title**: "What is the SubAgent Ecosystem?"

**Key Points**:
- 8 specialized agents available for Phase 1
- Each agent handles specific domain expertise
- Agents reduce context load on main agents
- Outputs go to `.ai/issues/{id}/` for team review
- Execute delegations immediately - no scheduling

**Script**:
```
"We've created 8 specialized SubAgents to help with common patterns and procedures.
Think of them as expert consultants - you ask a specific question, they provide 
a detailed answer.

These agents are NOT for active implementation - they're for design patterns, 
verification, and documentation generation.

When you delegate to a SubAgent, the output appears in .ai/issues/ for your team 
to review and integrate into your work."
```

---

### Segment 2: Your Team's Agents (16 minutes)

#### For @Backend Team (4 minutes)

**Available Agents**: 
1. @SubAgent-APIDesign
2. @SubAgent-DBDesign

**@SubAgent-APIDesign - When to Use**:
- Designing new HTTP endpoints
- Need REST pattern guidance
- Unsure about status codes or error formats
- Implementing API versioning

**Example Delegation**:
```
Request: "@SubAgent-APIDesign, design HTTP pattern for bulk user import endpoint"

Expected Output: 
.ai/issues/{task-id}/api-design.md
├─ Recommended patterns (5-6 options)
├─ Status code mapping
├─ Error response format
├─ Request/response examples
└─ Versioning strategy
```

**@SubAgent-DBDesign - When to Use**:
- Designing database schemas
- EF Core mapping questions
- Migration strategy decisions
- Query optimization guidance

**Example Delegation**:
```
Request: "@SubAgent-DBDesign, schema design for product catalog with attributes"

Expected Output:
.ai/issues/{task-id}/schema-design.md
├─ Entity relationships
├─ EF Core fluent mapping
├─ Migration strategy
├─ Query optimization tips
└─ Indexing recommendations
```

#### For @Frontend Team (4 minutes)

**Available Agents**:
1. @SubAgent-ComponentPatterns
2. @SubAgent-Accessibility

**@SubAgent-ComponentPatterns - When to Use**:
- Building complex components
- Need Vue 3 Composition API examples
- State management integration
- Reusability patterns

**Example Delegation**:
```
Request: "@SubAgent-ComponentPatterns, architecture for user profile form with validation"

Expected Output:
.ai/issues/{task-id}/component-design.md
├─ Composition API structure
├─ Props/emits design
├─ Form validation pattern
├─ State management integration
├─ Example implementation
└─ Slot patterns for extensibility
```

**@SubAgent-Accessibility - When to Use**:
- Component WCAG compliance audit
- ARIA pattern questions
- Keyboard navigation guidance
- Screen reader compatibility

**Example Delegation**:
```
Request: "@SubAgent-Accessibility, WCAG audit for user profile form"

Expected Output:
.ai/issues/{task-id}/a11y-audit.md
├─ WCAG 2.1 AA compliance check
├─ ARIA attributes needed
├─ Keyboard navigation issues
├─ Screen reader improvements
├─ Fixes with code examples
└─ Testing procedures
```

#### For @QA Team (4 minutes)

**Available Agents**:
1. @SubAgent-UnitTesting
2. @SubAgent-ComplianceTesting

**@SubAgent-UnitTesting - When to Use**:
- Need test patterns for new classes
- Mocking strategy questions
- Test data factory setup
- Coverage improvement

**Example Delegation**:
```
Request: "@SubAgent-UnitTesting, test setup for UserService class with dependencies"

Expected Output:
.ai/issues/{task-id}/test-setup.md
├─ xUnit test structure
├─ Moq mocking examples
├─ Arrange-Act-Assert pattern
├─ Test data factories
├─ Edge case scenarios
└─ Coverage targets
```

**@SubAgent-ComplianceTesting - When to Use**:
- GDPR compliance verification
- NIS2 requirements audit
- BITV 2.0 (accessibility) validation
- AI Act compliance check

**Example Delegation**:
```
Request: "@SubAgent-ComplianceTesting, GDPR compliance for user data export feature"

Expected Output:
.ai/issues/{task-id}/compliance-audit.md
├─ GDPR article checklist
├─ Data protection measures
├─ Consent verification
├─ Audit trail requirements
├─ Verification checklist
└─ Remediation steps
```

#### For @Security Team (2 minutes)

**Available Agents**:
1. @SubAgent-Encryption

**@SubAgent-Encryption - When to Use**:
- Encrypting PII (personally identifiable information)
- Key management strategy
- Algorithm selection (AES-256, RSA, etc.)
- GDPR/NIS2 encryption requirements

**Example Delegation**:
```
Request: "@SubAgent-Encryption, encrypt customer email per GDPR Article 32"

Expected Output:
.ai/issues/{task-id}/encryption-strategy.md
├─ AES-256 implementation
├─ Key management approach
├─ Database schema changes
├─ Performance impact
├─ Implementation code
└─ Testing strategy
```

#### For @Legal Team (2 minutes)

**Available Agents**:
1. @SubAgent-GDPR

**@SubAgent-GDPR - When to Use**:
- GDPR compliance questions
- Data protection guidance
- Consent documentation
- Data rights implementation

**Example Delegation**:
```
Request: "@SubAgent-GDPR, verify GDPR compliance for user registration flow"

Expected Output:
.ai/issues/{task-id}/gdpr-compliance.md
├─ GDPR article mapping
├─ Consent requirements
├─ Privacy notice checklist
├─ Data rights implementation
├─ DPA requirements
└─ Verification checklist
```

---

### Segment 3: How to Delegate (15 minutes)

#### When to Delegate (✅ YES / ❌ NO)

**✅ DELEGATE TO SUBAGENTS FOR**:
- Pattern/design questions
- Standard procedures  
- Compliance verification
- Code architecture reviews
- Documentation generation
- Best practice guidance

**❌ DON'T DELEGATE FOR**:
- Active task implementation
- Current sprint decisions
- Real-time problem-solving
- Emergency troubleshooting
- Team decisions
- Priority/timeline questions

#### How to Write a Clear Request

**Formula**: `@AgentName, [specific question about domain]`

**Good Examples**:
```
✅ "@SubAgent-APIDesign, design REST pattern for product bulk import endpoint"
✅ "@SubAgent-DBDesign, EF Core mapping for multi-tenant user data isolation"
✅ "@SubAgent-ComponentPatterns, Vue 3 architecture for complex data table"
✅ "@SubAgent-UnitTesting, test patterns for UserService with 5+ dependencies"
✅ "@SubAgent-GDPR, verify consent flow compliance with GDPR Article 7"
```

**Bad Examples**:
```
❌ "@SubAgent-APIDesign, help" (too vague)
❌ "@SubAgent-ComponentPatterns, build this component" (is implementation, not pattern)
❌ "@SubAgent-GDPR, should we delete this user?" (is a decision, not verification)
```

#### Where Outputs Go

**Location**: `.ai/issues/{task-id}/[output-name].md`

**Structure**:
```
.ai/issues/
├─ task-123-user-import/
│  ├─ api-design.md          ← SubAgent-APIDesign output
│  ├─ schema-design.md       ← SubAgent-DBDesign output
│  ├─ test-setup.md          ← SubAgent-UnitTesting output
│  └─ notes.md               ← Your team's integration notes
└─ task-124-user-export/
   ├─ gdpr-compliance.md     ← SubAgent-GDPR output
   └─ encryption-strategy.md ← SubAgent-Encryption output
```

**Team Process**:
1. Delegate to SubAgent with clear request
2. SubAgent creates output in `.ai/issues/{id}/`
3. Your team reviews output
4. Your team integrates recommendations into implementation
5. Attach link to output in PR/commit

#### Execution Rules

**Immediate Execution**:
- Submit delegation NOW
- Don't wait for "right time"
- Don't schedule for later
- Execute as soon as needed
- All delegations happen immediately

**Parallel Work**:
- Multiple teams can delegate in parallel
- No queuing or scheduling
- Each delegation independent
- All execute simultaneously

**Output Quality**:
- Outputs are recommendations, not requirements
- Your team makes final decisions
- Can request clarifications/revisions
- Use what's useful, adapt as needed

---

### Segment 4: Success Criteria & Metrics (4 minutes)

**What We're Measuring**:

1. **Adoption Rate** (Primary)
   - Goal: >50% of applicable tasks use SubAgents
   - Measurement: Task count using delegation / total tasks
   - Success: 4+ out of 5 teams actively delegating

2. **Context Reduction**
   - Goal: 65-70% smaller agent context
   - Before: @Backend 28 KB, @Frontend 24 KB
   - After: @Backend 8 KB, @Frontend 8 KB
   - Measurement: Token counter on actual prompts

3. **Team Satisfaction**
   - Goal: Positive feedback from all teams
   - Measurement: Daily retrospectives + Friday survey
   - Success: All teams recommend continuing

4. **Quality**
   - Goal: Zero regressions in code quality
   - Measurement: Bug count, code review feedback
   - Success: No degradation in metrics

**Daily Feedback Cycles**:
- Morning standup: Any issues with SubAgents?
- Afternoon: Quick satisfaction check
- Evening: Collect suggestions for improvements

---

### Segment 5: Q&A & Confidence Check (2 minutes)

**Open Forum**:
- Any questions about agents?
- Unclear when to delegate?
- Concerned about output quality?
- Not sure which agent to use?

**Confidence Check**:
- Does everyone understand the system?
- Are you ready to delegate?
- Any blockers before starting?
- Any team-specific questions?

---

## Live Demo Script (Optional - if time permits)

### Demo Delegation (5-7 minutes)

**Scenario**: Design pattern for user registration endpoint

**Live Steps**:
1. Show request format: "@SubAgent-APIDesign, [clear question]"
2. Submit delegation to agent
3. Wait for output (show expected location: `.ai/issues/{id}/`)
4. Show example output structure
5. Walk through recommendations
6. Show how team integrates

**Talking Points**:
- Clear, specific question gets better output
- Output goes to team for review
- Team decides what to use
- Takes 2-3 minutes for response
- Multiple teams can delegate in parallel

---

## FAQ Reference (Trainer Quick Notes)

**Q: Can multiple teams delegate at the same time?**
A: Yes, all delegations execute in parallel. No queuing.

**Q: What if the output isn't what we expected?**
A: Request clarification or revision from the agent.

**Q: Can we delegate for active implementation?**
A: No - SubAgents are for patterns, verification, guidance. Not for doing the work.

**Q: How long does a delegation take?**
A: Usually 2-5 minutes from submission to output.

**Q: What if we disagree with the recommendation?**
A: Your team makes final decisions. Use what's useful, adapt as needed.

**Q: Can we delegate the same question twice?**
A: Sure - different wording might get better results. Use best output.

**Q: What if there's a blocker?**
A: Post in #subagent-support or escalate to @TechLead.

**Q: How do we know if Phase 1 is successful?**
A: >50% adoption by Friday. All teams confirm they'll continue in Phase 2.

**Q: What happens after Phase 1 (Friday)?**
A: Review metrics. If successful (>50%), start Phase 2 (14 more agents).

---

## Materials Checklist

**For Trainer**:
- [ ] Print/digital copy of this guide
- [ ] Open agent definitions (8 files in `.github/agents/`)
- [ ] Example outputs from demo delegations
- [ ] List of #subagent-support channel info
- [ ] Confidence survey (simple 1-5 scale)

**For Teams**:
- [ ] Bookmark: `.ai/status/SUBAGENT_TIER1_DEPLOYMENT_GUIDE.md`
- [ ] Bookmark: `#subagent-support` Slack channel
- [ ] Keep open: `.ai/issues/` directory structure
- [ ] Reference: When to delegate guidelines (segment 3)

---

## Trainer Tips

1. **Keep pace**: 45 minutes covers all segments - stay on schedule
2. **Interactive**: Ask "Questions?" after each segment
3. **Concrete**: Use specific examples, not abstract concepts
4. **Reassuring**: Emphasize this is low-risk - outputs are optional guidance
5. **Energetic**: SubAgents are a productivity tool - show enthusiasm
6. **Clear**: Repeat the execution rule: "Submit NOW, no scheduling"

---

**Training Status**: ✅ READY TO DELIVER  
**Format**: Live structured 45-minute session  
**Delivery Method**: In-person with screen sharing  
**Materials**: All prepared and linked  
