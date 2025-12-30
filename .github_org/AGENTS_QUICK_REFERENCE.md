# 🎯 Copilot Agents Quick Reference Card

**Print this and keep it at your desk!**

---

## 🚀 Fastest Way to Start

**In Copilot Chat (Cmd+Shift+I), type:**

```
@[agent-name] [your question]
```

---

## 👥 Quick Agent Selection

### I'm a **Backend Developer**
```
@backend-developer Create Wolverine handler for product checkout
@backend-admin Implement JWT token validation
@backend-store Add Redis caching for product catalog
```

### I'm a **Frontend Developer**
```
@frontend-developer Make this component WCAG 2.1 AA accessible
@frontend-admin Create admin user management table
@frontend-store Build responsive product card
```

### I'm a **QA Engineer**
```
@qa-engineer Run P0.6 E-Commerce compliance tests
@qa-engineer Generate xUnit test for this service
@qa-engineer Write Playwright E2E test for checkout
```

### I'm a **Security Engineer**
```
@security-engineer Implement AES-256 encryption for PII
@security-engineer Design audit logging system
@security-engineer Set up NIS2 incident detection
```

### I'm a **DevOps Engineer**
```
@devops-engineer Configure network segmentation in AWS
@devops-engineer Set up Aspire orchestration
@devops-engineer Implement Azure KeyVault secret rotation
```

### I'm a **Tech Lead**
```
@tech-lead Review this architecture for Onion compliance
@tech-lead Enforce Wolverine pattern across team
@tech-lead Check for security gaps in this code
```

### I'm a **Product Owner**
```
@product-owner Define user story with acceptance criteria
@product-owner Check Phase 0 go/no-go gates
@product-owner Prioritize features vs compliance
```

### I'm a **Legal/Compliance Officer**
```
@legal-compliance Explain NIS2 incident notification requirements
@legal-compliance Review this feature for GDPR compliance
@legal-compliance Assess AI Act risk for fraud detection
```

---

## 🔧 Common Tasks

| Task | Agent | Command |
|------|-------|---------|
| Create HTTP handler | @backend-developer | Create Wolverine handler for... |
| Implement validation | @backend-developer | Add FluentValidation rules for... |
| Add encryption | @security-engineer | Encrypt this PII field... |
| Create test | @qa-engineer | Generate xUnit test for... |
| Make accessible | @frontend-developer | Make this WCAG 2.1 AA compliant... |
| Review code | @tech-lead | Review this for Onion Architecture... |
| Explain regulation | @legal-compliance | What does NIS2 require for...? |

---

## 🚨 CRITICAL PATTERNS

### ✅ DO: Wolverine (Backend)
```csharp
// Plain POCO command
public class CreateProductCommand { ... }

// Service handler with public async method
public class ProductService {
  public async Task<Response> CreateProduct(CreateProductCommand cmd, CancellationToken ct) { ... }
}

// Register in DI
builder.Services.AddScoped<ProductService>();
```

### ❌ DON'T: MediatR (NEVER USE!)
```csharp
// WRONG: No IRequest interface
public record CreateProductCommand : IRequest<Response>;

// WRONG: No IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Response> { ... }

// WRONG: No AddMediatR
builder.Services.AddMediatR();
```

---

## 📞 When to Use Each Agent

| Situation | Use This Agent |
|-----------|----------------|
| Building API endpoint | `@backend-developer` |
| Managing database/queries | `@backend-developer` |
| Encrypting sensitive data | `@security-engineer` |
| Setting up infrastructure | `@devops-engineer` |
| Creating Vue component | `@frontend-developer` |
| Testing/quality assurance | `@qa-engineer` |
| Architecture decisions | `@tech-lead` |
| Understanding regulations | `@legal-compliance` |
| Defining requirements | `@product-owner` |

---

## 📚 Critical Documentation

Every agent has access to these files:

- ✅ `.github/copilot-instructions.md` - Core patterns & rules
- ✅ `docs/by-role/[YOUR_ROLE].md` - Your role's detailed guide
- ✅ `docs/APPLICATION_SPECIFICATIONS.md` - Full requirements
- ✅ All source code in your context (defined in `.vscode/copilot-contexts.json`)

---

## ⚡ Power Tips

### Tip 1: Chain Agents
```
@backend-developer Create the handler
[copy result]

@qa-engineer Write tests for this handler
[paste result]
```

### Tip 2: Ask for Compliance
```
@backend-developer Create product checkout handler

Now mention @security-engineer to review for:
- PII encryption
- Audit logging
- Tenant isolation
```

### Tip 3: Get Architecture Review
```
@backend-developer Here's my design...

@tech-lead Is this following Onion Architecture?
```

---

## 🆘 Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Open Copilot Chat | `Cmd + Shift + I` (Mac) / `Ctrl + Shift + I` (Windows) |
| Inline Chat | `Cmd + I` (Mac) / `Ctrl + I` (Windows) |
| Submit Message | `Enter` |
| Clear Chat | `Cmd + L` (Mac) / `Ctrl + L` (Windows) |

---

## ✅ My First Steps

1. **Right now:** `Cmd + Shift + I` to open Copilot Chat
2. **Type:** `@backend-developer Hello! What can you help me with?`
3. **Wait:** Agent will explain its capabilities
4. **Bookmark:** Save this Quick Reference card
5. **Practice:** Try the examples above

---

**Still confused?** Open `.github/COPILOT_AGENTS_SETUP.md` for detailed guide!
