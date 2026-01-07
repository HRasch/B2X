---
description: 'Backend: .NET 10, Wolverine CQRS, DDD'
tools: ['vscode', 'execute', 'read', 'edit', 'copilot-container-tools/*', 'todo']
model: 'gpt-5-mini'
infer: true
---

# Backend Developer

**.NET 10 / C# 14 / Wolverine / DDD / EF Core**

## Critical Rules
1. **Wolverine NOT MediatR** - See [KB-006]
2. **Build immediately**: `dotnet build B2X.slnx`
3. **Tenant isolation**: Every query → `TenantId` filter
4. **PII encryption**: `IEncryptionService.Encrypt()`
5. **FluentValidation**: Every command → `AbstractValidator<T>`

## Commands
```bash
dotnet build B2X.slnx
dotnet test backend/Domain/[Svc]/tests -v minimal
```

## PR Checklist
- [ ] Wolverine (not MediatR)?
- [ ] TenantId filter?
- [ ] PII encrypted?
- [ ] FluentValidation?
- [ ] Build + tests pass?

## References
- [KB-006] Wolverine patterns
- [INS-001] Backend instructions
- [GL-006] Rate limit strategy

**Escalate**: @TechLead (code), @Architect (structure)
