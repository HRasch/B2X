# Documentation Integration Prompt

**Type**: Standard Process Prompt
**Audience**: All agents (for referencing & integrating documentation)
**Purpose**: Centralize and reference project documentation structure
**Status**: Active (Created Dec 30, 2025)

---

## What is This?

This prompt helps agents:
- Locate relevant documentation quickly
- Understand documentation organization
- Reference docs in their work
- Integrate new documentation into the system
- Maintain documentation consistency

---

## Documentation Structure

### 📚 Knowledge Base (KB Phase 1) - `/.ai/knowledgebase/`

**Purpose**: Reference patterns for implementation

| File | Purpose | Trigger Keywords |
|------|---------|------------------|
| [WOLVERINE_PATTERN_REFERENCE.md](../../.ai/knowledgebase/WOLVERINE_PATTERN_REFERENCE.md) | HTTP endpoints, CQRS patterns, command handlers | wolverine, http endpoint, cqrs, command, event handler, service method |
| [DDD_BOUNDED_CONTEXTS_REFERENCE.md](../../.ai/knowledgebase/DDD_BOUNDED_CONTEXTS_REFERENCE.md) | Service placement, bounded contexts, architecture | ddd, bounded context, service, domain, catalog, cms, identity, tenancy, admin |
| [ERROR_HANDLING_PATTERNS.md](../../.ai/knowledgebase/ERROR_HANDLING_PATTERNS.md) | Exception types, validation, logging | exception, validation, error, logging, try-catch, middleware |
| [VUE3_COMPOSITION_PATTERNS.md](../../.ai/knowledgebase/VUE3_COMPOSITION_PATTERNS.md) | Vue3 components, Pinia stores, composables | vue3, component, pinia, store, composable, typescript |
| [ASPIRE_ORCHESTRATION_REFERENCE.md](../../.ai/knowledgebase/ASPIRE_ORCHESTRATION_REFERENCE.md) | Service orchestration, local dev setup | aspire, orchestration, service discovery, program.cs, health checks |
| [FEATURE_IMPLEMENTATION_PATTERNS.md](../../.ai/knowledgebase/FEATURE_IMPLEMENTATION_PATTERNS.md) | End-to-end feature workflow | feature, implementation, aggregate root, domain event, e2e testing |

**How to Use**:
```markdown
When implementing [FEATURE], reference [KB_FILE]:

✅ Example: Implementing product creation endpoint
→ Reference: WOLVERINE_PATTERN_REFERENCE.md (service method pattern)
→ Reference: DDD_BOUNDED_CONTEXTS_REFERENCE.md (Catalog service location)
→ Reference: ERROR_HANDLING_PATTERNS.md (validation & exceptions)
→ Reference: FEATURE_IMPLEMENTATION_PATTERNS.md (full workflow)
```

---

### 📖 Essential Guides - `/docs/guides/`

**Purpose**: Step-by-step setup & workflows

| File | Purpose | When to Use |
|------|---------|-------------|
| [GETTING_STARTED.md](../../docs/guides/GETTING_STARTED.md) | Local dev setup, service startup | New developer onboarding, environment issues |
| [DEVELOPER_GUIDE.md](../../docs/guides/DEVELOPER_GUIDE.md) | Project structure, service list, workflow | Architecture questions, service discovery |
| [DEVELOPER_GUIDE_PRICE_VAT.md](../../docs/guides/DEVELOPER_GUIDE_PRICE_VAT.md) | Price & VAT specific implementation | E-commerce pricing feature implementation |
| [TESTING_STRATEGY.md](../../docs/guides/TESTING_STRATEGY.md) | Test types, coverage, tools | @QA delegation, test planning |

---

### 🏗️ Architecture Documentation - `/docs/architecture/`

**Contains**: Design decisions, patterns, system design

**Subdirectories**:
- `decisions/` - Architecture Decision Records (ADRs)
- `services/` - Service-level architecture
- `data/` - Database schemas & migrations
- `deployment/` - Deployment architecture

---

### 🔒 Compliance Documentation - `/docs/compliance/`

**Contains**: Legal, security, accessibility requirements

**Subdirectories**:
- `gdpr/` - GDPR compliance
- `nis2/` - NIS2 directive
- `bitv/` - BITV 2.0 accessibility
- `ai-act/` - AI Act compliance

---

### 💼 Role-Based Documentation - `/docs/by-role/`

**Contains**: Role-specific guides and workflows

**Subdirectories**:
- `BACKEND_DEVELOPER.md` - Backend-specific workflows
- `FRONTEND_DEVELOPER.md` - Frontend-specific workflows
- `QA_ENGINEER.md` - QA procedures
- `DEVOPS_ENGINEER.md` - Infrastructure workflows
- `TECH_LEAD.md` - Leadership & decision-making
- `PRODUCT_OWNER.md` - Requirements & prioritization
- `SECURITY_ENGINEER.md` - Security reviews & audits
- `LEGAL_COMPLIANCE.md` - Legal considerations

---

### 🎯 Feature Documentation - `/docs/features/`

**Contains**: Feature guides & implementation details

**Subdirectories**:
- `authentication/` - Auth implementation
- `catalog/` - Product catalog
- `cms/` - Content management
- `checkout/` - Purchase flow
- `admin/` - Admin operations

---

### 📋 Process Documentation - `/docs/processes/`

**Contains**: Development workflows & ceremonies

**Subdirectories**:
- `code-review/` - Code review procedures
- `deployment/` - Release procedures
- `incident-response/` - Incident handling
- `retrospectives/` - Sprint retrospectives

---

### 👥 User Documentation - `/docs/user-guides/`

**Contains**: End-user guides for the system

**Examples**: Store customer guide, admin user manual

---

## How to Reference Documentation

### In Code Comments
```csharp
// See: ../../.ai/knowledgebase/WOLVERINE_PATTERN_REFERENCE.md#http-endpoints
// Pattern: Service method approach for HTTP handlers
public async Task<CreateProductResult> CreateProduct(CreateProductCommand cmd)
{
    // Implementation...
}
```

### In Documentation
```markdown
For detailed implementation patterns, see:
- [Wolverine HTTP Endpoints](../../../.ai/knowledgebase/WOLVERINE_PATTERN_REFERENCE.md#http-endpoints)
- [DDD Bounded Contexts](../../../.ai/knowledgebase/DDD_BOUNDED_CONTEXTS_REFERENCE.md)
```

### In PR Descriptions
```markdown
## Changes
- Implements product creation endpoint (Catalog service)

## References
- See [Wolverine Pattern Reference](../../.ai/knowledgebase/WOLVERINE_PATTERN_REFERENCE.md) for HTTP handler pattern
- See [DDD Bounded Contexts](../../.ai/knowledgebase/DDD_BOUNDED_CONTEXTS_REFERENCE.md) for Catalog service location
- See [Error Handling Patterns](../../.ai/knowledgebase/ERROR_HANDLING_PATTERNS.md) for validation
```

---

## Adding New Documentation

### When to Create Documentation

Create new documentation when:
- ✅ Pattern emerges across multiple services
- ✅ Process changes significantly
- ✅ New technology integrated
- ✅ Compliance requirement introduced
- ✅ Team learning captured

### Where to Add

**Pattern/Reference Documentation**:
- Location: `/.ai/knowledgebase/`
- Format: `[PATTERN_NAME]_REFERENCE.md`
- Trigger: Keyword list at top
- Example: `AUTHENTICATION_PATTERNS.md`

**Process Documentation**:
- Location: `/docs/processes/[domain]/`
- Format: `[PROCESS_NAME]_PROCEDURE.md`

**Feature Documentation**:
- Location: `/docs/features/[feature_name]/`
- Format: `[FEATURE]_GUIDE.md`

**Compliance Documentation**:
- Location: `/docs/compliance/[standard]/`
- Format: `[STANDARD]_COMPLIANCE.md`

### Documentation Template

```markdown
# [Pattern/Feature/Process] Reference

**Audience**: [Who should read this?]
**Purpose**: [What problem does this solve?]
**Related**: [Links to other docs]

---

## When to Use

Trigger keywords: [list keywords that indicate when to use this doc]

---

## Quick Start

[Minimal working example]

---

## Detailed Patterns

[Full reference material]

---

## Common Mistakes

[Antipatterns to avoid]

---

## References

[Links to external resources]

---

*Updated: [DATE]*
*Framework: [Technology]*
```

---

## Agent Documentation Tasks

### @Backend Reference Pattern
```
If implementing a new Wolverine service:
→ Reference: WOLVERINE_PATTERN_REFERENCE.md
→ Reference: DDD_BOUNDED_CONTEXTS_REFERENCE.md
→ Reference: ERROR_HANDLING_PATTERNS.md
→ Reference: FEATURE_IMPLEMENTATION_PATTERNS.md
```

### @Frontend Reference Pattern
```
If implementing Vue3 components:
→ Reference: VUE3_COMPOSITION_PATTERNS.md
→ Reference: FEATURE_IMPLEMENTATION_PATTERNS.md (frontend section)
```

### @Architect Reference Pattern
```
For system design decisions:
→ Reference: /docs/architecture/decisions/
→ Create: Architecture Decision Record (ADR)
→ Document: In `/docs/architecture/decisions/`
```

### @QA Reference Pattern
```
For test strategy:
→ Reference: TESTING_STRATEGY.md
→ Reference: /docs/processes/code-review/
→ Reference: FEATURE_IMPLEMENTATION_PATTERNS.md (testing section)
```

### @Security Reference Pattern
```
For security implementation:
→ Reference: /docs/compliance/
→ Reference: ERROR_HANDLING_PATTERNS.md (logging)
→ Reference: FEATURE_IMPLEMENTATION_PATTERNS.md (security checklist)
```

---

## Integration Checklist

When integrating new documentation:

- [ ] File created in correct location
- [ ] Filename follows naming convention
- [ ] Top section has: Audience, Purpose, Related docs
- [ ] Trigger keywords documented
- [ ] Code examples included
- [ ] Links to related docs added
- [ ] Updated date included
- [ ] Committed with clear message
- [ ] Referenced in relevant KB files
- [ ] Announced to team (if significant)

---

## Quick Links

**Documentation Index**: [docs/](../../docs/)
**KB Phase 1**: [/.ai/knowledgebase/](../../.ai/knowledgebase/)
**Guides**: [docs/guides/](../../docs/guides/)
**Architecture**: [docs/architecture/](../../docs/architecture/)
**Compliance**: [docs/compliance/](../../docs/compliance/)
**By Role**: [docs/by-role/](../../docs/by-role/)
**Features**: [docs/features/](../../docs/features/)
**Processes**: [docs/processes/](../../docs/processes/)

---

## Example Integration Workflow

```
📝 New Pattern Discovered:
→ Document in /.ai/knowledgebase/[PATTERN]_REFERENCE.md
→ Add trigger keywords
→ Add to KB Phase 1 table (above)
→ Reference in related KB files
→ Commit with clear message
→ Announce in team channel
```

---

*Prompt Version: 1.0*
*Last Updated: 30. Dezember 2025*
*Maintained by: @SARAH*
