# 📋 Agent-Role Documentation Guidelines

**Purpose**: Standards for creating documentation that belongs to agent instruction files  
**Owner**: @process-assistant (enforces standards), @software-architect (reviews)  
**Last Updated**: 29. Dezember 2025  
**Status**: Active - Binding for all agent-role documentation

---

## 🎯 What Is Agent-Role Documentation?

Documentation that:
- ✅ Describes **how an agent/role should behave**
- ✅ Provides **specific guidance** for a particular agent
- ✅ Belongs in `.github/copilot-instructions-*.md` or `.github/agents/*.md` files
- ✅ Helps agents understand **their responsibilities, authority, processes**
- ✅ Is **referenced by other agents** in their work

**Examples**:
- Backend Developer instructions → What patterns to use, code quality standards
- Security Engineer instructions → Encryption standards, audit logging requirements
- QA Engineer instructions → Testing framework, compliance test structure
- Scrum Master instructions → Coordination processes, retrospective format

---

## 📐 Standard Structure (REQUIRED)

### 1. Header Section (Non-negotiable)

```markdown
# [Role Name] - [Focus Area]

**Focus**: [Main responsibility area]  
**Agent**: @[agent-name] (or specialized: @agent1, @agent2)  
**Escalation**: Problem type → @contact | Problem type → @contact  
**For full reference**: [Link to main instructions](path)

---
```

**Why**: Immediately tells reader what this document is about and who owns it

**Example**:
```markdown
# Backend Developer - AI Agent Instructions

**Focus**: Wolverine services, onion architecture, database design  
**Agent**: @backend-developer (or specialized: @backend-admin, @backend-store)  
**Escalation**: Complex implementation → @tech-lead | System structure → @software-architect  
**For full reference**: [copilot-instructions.md](./copilot-instructions.md)
```

---

### 2. Mission/Purpose Section

```markdown
## 🎯 Your Mission

[What this role does in the project]
[What problems they solve]
[Why this role exists]

---
```

**Content should**:
- Be 2-3 sentences max
- Explain the role's core value
- Connect to project goals
- Be action-oriented

**Example**:
```markdown
## 🎯 Your Mission

As the Backend Developer, you implement business logic in Wolverine microservices using DDD patterns. 
Your responsibility is to:
- ✅ Build services following Onion Architecture
- ✅ Ensure multi-tenant security (TenantId filtering)
- ✅ Write testable, maintainable code (80%+ coverage)
- ✅ Follow security-first approach (encryption, audit logging)
```

---

### 3. Critical Rules Section

```markdown
## ⚡ Critical Rules

List 5-8 non-negotiable rules this role MUST follow.

1. **Rule Title**: Rule description with rationale
2. **Rule Title**: Rule description with consequences
...

---
```

**Format requirements**:
- **BOLD** title
- Clear, actionable description
- Consequences if broken
- Rules agents **will be checked on**

**Example**:
```markdown
## ⚡ Critical Rules

1. **Build-First Rule (CRITICAL)**: Generate files → `dotnet build B2Connect.slnx` immediately → Fix errors → Test
   - **Why**: Deferred build validation cascades to 38+ test failures
   - **Prevents**: Cascading failures across multiple files
   - **Impact**: Pattern: Code → Build → Test → Commit (never defer build)

2. **Tenant Isolation (SECURITY)**: EVERY query must filter by `TenantId`
   - **Why**: Prevents data breach
   - **Enforced By**: Code review checklist
   - **Impact**: Security gate before merge

3. **No MediatR Pattern (ARCHITECTURE)**: Use Wolverine, never MediatR
   - **Why**: B2Connect requires distributed messaging
   - **Impact**: Complete refactor if wrong pattern used
   - **Reference**: Copy from `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`
```

---

### 4. Quick Commands Section

```markdown
## 🚀 Quick Commands

Essential commands this role will run repeatedly.

```bash
command 1           # Purpose
command 2           # Purpose
command 3           # Purpose
```

---
```

**What to include**:
- Commands you run **multiple times per day**
- Build, test, deployment commands
- Debugging commands
- Status check commands
- **NOT**: One-time setup commands (those go in README)

**Example**:
```markdown
## 🚀 Quick Commands

```bash
dotnet build B2Connect.slnx                     # Build everything
dotnet test backend/Domain/[Service]/tests      # Test specific service
cd backend/Orchestration && dotnet run          # Start all services
dotnet ef migrations add [Name]                 # Create migration
```
---
```

---

### 5. Before-You-Code Checklist

```markdown
## 📋 Before Implementing [Thing]

### [Category 1]
- [ ] Check 1
- [ ] Check 2
- [ ] Check 3

### [Category 2]
- [ ] Check 1
- [ ] Check 2

**Rationale**: [Why this prevents problems]

---
```

**Categories** (adjust per role):
- Code Structure
- Security & Data Protection
- Async & Performance
- Testing & Validation
- Code Review Readiness
- User-Facing Documentation

**Example (Backend)**:
```markdown
## 📋 Before Implementing a Handler

### Code Structure
- [ ] Is this a plain POCO command (no IRequest)?
- [ ] Is handler a public async method in a Service class?
- [ ] Does it follow the Wolverine pattern (NOT MediatR)?

### Security & Data Protection
- [ ] Does it filter queries by TenantId?
- [ ] Are PII fields encrypted (IEncryptionService)?
- [ ] Is there audit logging for data changes?
- [ ] Does it validate input with FluentValidation?

### Testing & Validation
- [ ] Build successful after code: `dotnet build B2Connect.slnx`?
- [ ] Tests passing: `dotnet test backend/Domain/[Service]/tests -v minimal`?

**Rationale**: Sprint 1 Phase A (Issue #30) revealed gaps in multi-stage validation. 
This comprehensive checklist prevents regressions.
```

---

### 6. Common Mistakes Section

```markdown
## 🛑 Common Mistakes

| Mistake | Prevention |
|---------|-----------|
| **Mistake description** | How to avoid it |
| **Example of wrong pattern** | Use this pattern instead |

---
```

**Format**:
- First column: What agents do wrong
- Second column: How to avoid it
- Include reference files if applicable

**Example**:
```markdown
## 🛑 Common Mistakes

| Mistake | Prevention |
|---------|-----------|
| Using MediatR | Copy from `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs` |
| Forgetting tenant filter | Add `.Where(x => x.TenantId == tenantId)` to EVERY query |
| No encryption for PII | Use `IEncryptionService.Encrypt()` for email, phone, address, DOB |
| Hardcoding secrets | Use `IConfiguration["Key"]` or `appsettings.json` |
```

---

### 7. Architecture/Pattern Section (if applicable)

```markdown
## 🎯 [Pattern Name] - REQUIRED

[Visual diagram explaining the pattern]

[Code example - ✅ DO THIS]

[Anti-pattern example - ❌ NEVER THIS]

---
```

**Include**:
- Mermaid diagram (visual)
- Working code example (copy-paste ready)
- Anti-pattern warning (what NOT to do)
- Link to reference implementation

**Example (Backend)**:
```markdown
## 🎯 Wolverine Handler Pattern (REQUIRED)

[Diagram showing request → handler → response]

✅ **CORRECT**
```csharp
public class CreateProductService {
    public async Task<Response> CreateAsync(CreateProductCommand cmd, CancellationToken ct) { }
}
```

❌ **NEVER**
```csharp
// NO: public record CreateProductCommand : IRequest<Response>
// NO: public class Handler : IRequestHandler<...>
```
---
```

---

### 8. Reference Files Section

```markdown
## 📚 Reference Files

| Document | Path | When to Use |
|----------|------|-------------|
| [Document Name] | [relative path] | When you need X |
| [Document Name] | [relative path] | When you need Y |

---
```

**Include**:
- Architecture docs
- Design patterns
- Testing guides
- Security docs
- Compliance docs
- Example implementations

**Example**:
```markdown
## 📚 Reference Files

| Document | Path | When to Use |
|----------|------|-------------|
| [Onion Architecture](../../docs/ONION_ARCHITECTURE.md) | docs/ | Understanding service structure |
| [Wolverine Example](../../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs) | backend/ | Copying handler pattern |
| [TESTING_GUIDE.md](../../docs/guides/TESTING_GUIDE.md) | docs/ | Writing unit/integration tests |
```

---

### 9. Escalation Path Section

```markdown
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **[Situation 1]**: Ask @agent → Details
- **[Situation 2]**: Ask @agent → Details
- **[Situation 3]**: Ask @agent → Escalate to @agent2

---
```

**Format**:
- Situation → Agent to contact → Details
- Shows decision tree
- Prevents wrong escalations

**Example**:
```markdown
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Complex implementation**: Ask @tech-lead for code patterns
- **Service design question**: Ask @tech-lead, they may escalate to @software-architect
- **Architectural decision**: Ask @software-architect directly
- **DevOps/Operations**: Ask @cli-developer if it should be a CLI command
- **Performance issue**: Ask @tech-lead, may escalate to @software-architect
```

---

### 10. Security/Compliance Checklist (if applicable)

```markdown
## 🔐 [Security Topic] Checklist

Before PR: Does your code include?
- [ ] Check 1
- [ ] Check 2
- [ ] Check 3

---
```

**Categories** (adjust per role):
- Encryption
- Audit Logging
- Validation
- Multi-tenant safety
- Access control
- Error handling
- Compliance

**Example (Backend)**:
```markdown
## 🔐 Security Checklist (Mandatory for Features)

Before PR: Does your code include?
- [ ] FluentValidation validators
- [ ] TenantId filtering on all queries
- [ ] Audit logging via EF Core interceptor
- [ ] PII encryption (Email, Phone, FirstName, LastName, Address, DOB)
- [ ] `CancellationToken` passed through async calls
- [ ] No hardcoded secrets (use `IConfiguration`)
- [ ] Tests with 80%+ coverage
```

---

## 📝 Content Standards

### Writing Style

| Standard | Requirement |
|----------|------------|
| **Length** | 2-10 pages (2,000-5,000 words) |
| **Audience** | Developers with 1-3 years experience |
| **Tone** | Professional, direct, action-oriented |
| **Examples** | Every concept needs 2-3 code examples |
| **Links** | Every reference must be a link |
| **Grammar** | Proofread, bilingual if user-facing |

### Formatting

- ✅ **Bold** for critical concepts
- ✅ `Code` for class/method names
- ✅ > Quotes for important notes
- ✅ [Links](path) to related docs
- ✅ Tables for comparisons
- ✅ Mermaid diagrams for architecture
- ✅ Emojis for visual scanning
- ❌ Wall of text (break into sections)
- ❌ Nested bullet points (flatten to 1-2 levels)

### Code Examples

**REQUIRED for every concept**:
- ✅ At least 1 "correct" example
- ✅ At least 1 "wrong" example (anti-pattern)
- ✅ Annotation explaining why
- ✅ Link to reference implementation in codebase

**Format**:
```markdown
✅ **DO THIS**
[code]

❌ **NEVER THIS**
[code]

**Why**: [Explanation]

**Reference**: [Link to file]
```

---

## ✅ Quality Checklist (Before Submitting)

### Content Quality
- [ ] Header section present (role, focus, escalation, link)
- [ ] Mission/purpose explained in 2-3 sentences
- [ ] 5-8 critical rules listed with rationale
- [ ] Quick commands documented (most frequent operations)
- [ ] Before-you-code checklist has 10+ items
- [ ] Common mistakes covered (3-5 items)
- [ ] Pattern explanation with diagram + examples
- [ ] Reference files clearly linked
- [ ] Escalation path documented

### Formatting & Clarity
- [ ] No wall of text (sections < 200 words each)
- [ ] Code examples include ✅ correct and ❌ wrong
- [ ] All references are links (not plain text)
- [ ] Tables for comparisons
- [ ] Diagrams for architecture/flow
- [ ] Emoji for visual scanning (🎯, ⚡, 📚, 🚀, 🛑, ✅, ❌)
- [ ] No more than 2-3 levels of bullets
- [ ] Grammar/spelling correct

### Consistency
- [ ] Terminology matches other docs
- [ ] Patterns consistent with architecture (e.g., Wolverine, Onion)
- [ ] Examples follow project conventions
- [ ] Links work (relative paths correct)
- [ ] Cross-references to related docs
- [ ] No contradictions with other instructions

### Completeness
- [ ] Every rule has rationale/consequence
- [ ] Every pattern has working example
- [ ] Every concept has reference documentation
- [ ] All roles mentioned understand their responsibility
- [ ] Escalation covers all likely issues
- [ ] Security/compliance checklist if data-related

### Accessibility
- [ ] Readable by someone new to role (5-15 years experience)
- [ ] Can be understood in 15-30 minutes (with links for depth)
- [ ] Quick-start section at top
- [ ] Can search for specific pattern (good keywords)
- [ ] Bilingual if user-facing (EN + DE)

---

## 🔄 Integration with Agent Instructions

### How Documentation Becomes Agent Instructions

**Step 1: Create in Role-Specific Document**
- Write patterns, examples, checklists for your role
- Use this template
- Get reviewed by @software-architect

**Step 2: Submit to Instructions**
- Identify which `.github/copilot-instructions-*.md` file needs update
- Extract key patterns
- Submit as PR with reference to source documentation

**Step 3: Integrate into Agent Instructions**
- @process-assistant reviews for consistency
- @software-architect approves content
- PR merged into instruction file
- Documentation becomes binding for that role

**Step 4: Link Back to Source**
- Original doc referenced in agent instructions
- Agent instructions link to detailed guidance
- Circular reference helps agents find both

### Example Integration

**Original Documentation** (this file):
```markdown
## 🔐 Security Checklist (Mandatory for Features)
- [ ] FluentValidation validators
- [ ] TenantId filtering...
```

**Becomes in Agent Instructions**:
```markdown
## 🔐 Security Checklist (Mandatory for Features)

Before PR: Does your code include?
- [ ] FluentValidation validators
- [ ] TenantId filtering...

**Reference**: See [AGENT_ROLE_DOCUMENTATION_GUIDELINES.md](../docs/AGENT_ROLE_DOCUMENTATION_GUIDELINES.md) for detailed security patterns
```

---

## 📋 Documentation Planning Template

Use this to plan your agent-role documentation:

```markdown
# [Role Name] Documentation Plan

## Scope
- What does this role do?
- What problems does it solve?
- Who reads this? (audience)

## Content Sections
- [ ] Header/Purpose (✅/❌)
- [ ] Critical Rules (5-8 required rules)
- [ ] Quick Commands (most frequent)
- [ ] Before-You-Code Checklist
- [ ] Common Mistakes (3-5)
- [ ] Pattern Explanation (with code)
- [ ] Reference Files
- [ ] Escalation Path
- [ ] Security/Compliance Checklist

## Code Examples Needed
- [ ] ✅ Correct pattern 1
- [ ] ❌ Wrong pattern 1
- [ ] ✅ Correct pattern 2
- [ ] ❌ Wrong pattern 2

## References to Link
- [ ] Architecture doc A
- [ ] Architecture doc B
- [ ] Implementation example in codebase
- [ ] Test example in codebase

## Quality Checks
- [ ] No wall of text (sections < 200 words)
- [ ] All examples have ✅/❌
- [ ] All references are links
- [ ] Tables for comparisons
- [ ] Emoji for visual scanning
- [ ] Grammar/spelling correct
- [ ] Consistent with other docs
- [ ] Bilingual if user-facing
```

---

## 🎓 Example: Complete Agent-Role Documentation

### Backend Developer - Example Section

**What it should look like**:

```markdown
# Backend Developer - AI Agent Instructions

**Focus**: Wolverine services, onion architecture, database design  
**Agent**: @backend-developer  
**Escalation**: Complex problems → @tech-lead | System structure → @software-architect  
**For full reference**: [copilot-instructions.md](./copilot-instructions.md)

---

## 🎯 Your Mission

Build business logic in Wolverine microservices using DDD patterns. You ensure:
- ✅ Onion Architecture is followed (domain layer has no dependencies)
- ✅ Multi-tenant security (every query filters by TenantId)
- ✅ Code quality (80%+ test coverage, no warnings)
- ✅ Security-first (encryption, audit logging, no secrets in code)

---

## ⚡ Critical Rules

1. **Build-First Rule (CRITICAL)**: Generate files → Build immediately → Fix errors → Test
   - Deferred builds cause 30+ cascading failures
   - Pattern: Code → Build → Test → Commit

2. **Tenant Isolation (SECURITY)**: EVERY query filters by `TenantId`
   - Prevents data breaches
   - Checked in code review

3. **No MediatR**: Use Wolverine, never MediatR
   - B2Connect requires distributed messaging
   - Reference: `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`

---

## 🚀 Quick Commands

```bash
dotnet build B2Connect.slnx                    # Build immediately after code
dotnet test backend/Domain/[Service]/tests     # Test after changes
cd backend/Orchestration && dotnet run         # Start all services
```

---

[Continue with other sections...]
```

---

## 🔒 Governance

**Who can create agent-role documentation?**
- @software-architect (main authority)
- Specialists in their domain (backend, frontend, QA, etc.)
- @process-assistant (reviews for standards compliance)

**Review Process**:
1. Create documentation following this template
2. Submit PR with reference to architecture/patterns
3. @software-architect reviews content
4. @process-assistant reviews for format/standards
5. Merged into `.github/` or `/docs/` as appropriate

**Authority**:
- ✅ @process-assistant can require format changes
- ✅ @software-architect can require content changes
- ❌ Cannot modify without approval from both

---

## 📞 Questions About This Guide?

| Question | Contact |
|----------|---------|
| "How do I document my role?" | Read this guide + use template below |
| "Is my documentation complete?" | Run quality checklist above |
| "Should this be in instructions or docs?" | Ask @process-assistant |
| "Should this be bilingual (EN/DE)?" | Ask @software-architect |
| "How do I get it approved?" | Submit PR with @software-architect + @process-assistant |

---

## 🎯 Summary

**Great agent-role documentation**:
- ✅ Follows this template structure
- ✅ Has 5-8 critical rules (with consequences)
- ✅ Includes before-you-code checklist (10+ items)
- ✅ Has code examples (✅ correct, ❌ wrong)
- ✅ Links to reference implementations
- ✅ Is readable in 15-30 minutes
- ✅ Can be searched for patterns
- ✅ Is reviewed and approved before merge
- ✅ Is integrated into agent instructions
- ✅ Is kept current (reviewed quarterly)

**Use this guide to create documentation that agents actually read and follow.**

---

**Created**: 29. Dezember 2025  
**Status**: ✅ ACTIVE  
**Owner**: @process-assistant (enforces), @software-architect (reviews)  
**Next Review**: Quarterly (with agent instruction updates)
