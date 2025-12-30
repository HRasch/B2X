# Copilot Instructions Setup Guide

**Last Updated**: 28. Dezember 2025  
**Purpose**: Configure AI coding agents for B2Connect development

---

## ✅ Quick Start

1. **Read the Main Instructions**: See [copilot-instructions.md](./copilot-instructions.md)
2. **Choose Your Agent**: See `.github/agents/[your-role].agent.md`
3. **Follow the Workflow**: Implement → Build → Test → Commit

---

## 🎯 Role-Based Agents (24 Total)

### Core Development Roles
- [backend-developer.agent.md](./agents/backend-developer.agent.md) - API/service development
- [frontend-developer.agent.md](./agents/frontend-developer.agent.md) - Vue.js/UX development
- [qa-engineer.agent.md](./agents/qa-engineer.agent.md) - Testing/validation

### Specialized Roles
- [security-engineer.agent.md](./agents/security-engineer.agent.md) - Encryption/compliance
- [devops-engineer.agent.md](./agents/devops-engineer.agent.md) - Infrastructure/scaling
- [tech-lead.agent.md](./agents/tech-lead.agent.md) - Architecture/standards
- [product-owner.agent.md](./agents/product-owner.agent.md) - Requirements/prioritization
- [legal-compliance.agent.md](./agents/legal-compliance.agent.md) - Regulatory/legal

### Service-Specific Roles
- [backend-store.agent.md](./agents/backend-store.agent.md) - Store microservice
- [backend-admin.agent.md](./agents/backend-admin.agent.md) - Admin operations
- [frontend-store.agent.md](./agents/frontend-store.agent.md) - Store frontend
- [frontend-admin.agent.md](./agents/frontend-admin.agent.md) - Admin frontend

### QA Specializations
- [qa-frontend.agent.md](./agents/qa-frontend.agent.md) - Frontend testing
- [qa-pentesting.agent.md](./agents/qa-pentesting.agent.md) - Security testing
- [qa-performance.agent.md](./agents/qa-performance.agent.md) - Load/stress testing

### Expert Roles
- [ai-specialist.agent.md](./agents/ai-specialist.agent.md) - AI/ML implementation
- [ui-expert.agent.md](./agents/ui-expert.agent.md) - Design systems
- [ux-expert.agent.md](./agents/ux-expert.agent.md) - User experience

### Stakeholder Roles
- [stakeholder-crm.agent.md](./agents/stakeholder-crm.agent.md) - CRM integration
- [stakeholder-erp.agent.md](./agents/stakeholder-erp.agent.md) - ERP integration
- [stakeholder-pim.agent.md](./agents/stakeholder-pim.agent.md) - PIM integration
- [stakeholder-bi.agent.md](./agents/stakeholder-bi.agent.md) - Analytics/BI
- [stakeholder-reseller.agent.md](./agents/stakeholder-reseller.agent.md) - Reseller program

### Support
- [support-triage.agent.md](./agents/support-triage.agent.md) - GitHub issue automation

---

## 📋 Development Workflow (All Roles)

```
1. PLAN → Read your agent context
2. CODE → Follow patterns from copilot-instructions.md
3. BUILD → dotnet build (must pass, no warnings)
4. TEST → dotnet test (80%+ coverage)
5. COMMIT → Follow Git conventions
6. REVIEW → Code review checklist
7. MERGE → To feature branch, then main
```

---

## 🔐 Security-First Development

**Before writing ANY code, verify:**

✅ **Database Layer**
- TenantId field (multi-tenant isolation)
- CreatedAt, CreatedBy (audit trail)
- PII encrypted (Email, Phone, Address)
- Soft deletes enabled
- Query filters by TenantId

✅ **Service Layer**
- IEncryptionService injected
- Audit logging for CRUD
- FluentValidation validators
- Authorization checks
- CancellationToken propagated

✅ **API Layer**
- JWT validation required
- X-Tenant-ID header validated
- All inputs validated server-side
- No PII in errors
- Safe response structure

✅ **Testing**
- Cross-tenant access blocked
- Encryption round-trip tested
- Audit logs verified
- CancellationToken tested

---

## 📚 Documentation

- [copilot-instructions.md](./copilot-instructions.md) - Main guidelines (3,500+ lines)
- `agents/[role].agent.md` - Role-specific specifications
- `.github/AGENTS_CREATED.md` - Full agent registry
- `.github/AGENTS_QUICK_REFERENCE.md` - Quick lookup

---

## ⚡ Key Patterns

### Build → Test → Commit (Always)
```bash
dotnet build B2Connect.slnx           # No warnings!
dotnet test B2Connect.slnx -v minimal # 80%+ coverage
git commit -m "feat(service): description (#issue-id)"
```

### Wolverine (NOT MediatR!)
```csharp
// Plain POCO command (NO IRequest interface)
public class CreateProductCommand { public string Sku { get; set; } }

// Service handler (NO IRequestHandler attribute)
public class ProductService {
    public async Task<Response> CreateProduct(CreateProductCommand req, CancellationToken ct) { }
}

// Simple DI (NO AddMediatR)
builder.Services.AddScoped<ProductService>();
```

### Compliance (ALL Features)
- Audit logging for CRUD operations
- Encryption for PII fields
- Tenant isolation in queries
- Soft deletes (never hard delete)
- Tests for security scenarios

---

## 🚨 Common Issues

| Issue | Solution |
|-------|----------|
| Port already in use | Run `./scripts/kill-all-services.sh` |
| Compiler warnings | Fix before committing (CS codes) |
| Test failures | Run `dotnet test` locally first |
| Hardcoded secrets | Use environment variables or KeyVault |
| Cross-tenant data leak | Add TenantId filter to all queries |
| Missing encryption | PII fields encrypted in database |

---

## 📞 Escalation

- **Architecture Question** → tech-lead agent
- **Security Concern** → security-engineer agent
- **Compliance Question** → legal-compliance agent
- **Build Issues** → devops-engineer agent
- **Feature Prioritization** → product-owner agent

---

**See also**: [copilot-instructions.md](./copilot-instructions.md) for detailed guidelines
