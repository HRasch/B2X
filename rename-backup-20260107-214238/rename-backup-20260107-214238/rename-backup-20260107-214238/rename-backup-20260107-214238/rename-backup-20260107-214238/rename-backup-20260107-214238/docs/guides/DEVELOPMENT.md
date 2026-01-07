# Development Guidelines

Guidelines for working on B2Connect features, architecture decisions, and coding standards.

## 📋 What's in This Document

- **Development Workflow**: How to approach new features
- **Code Standards**: What we follow (see [.copilot-specs.md](.copilot-specs.md) for full details)
- **Architecture**: Service boundaries, communication patterns
- **Testing**: TDD approach, coverage targets
- **Tools & Environment**: VS Code setup, debugging

## 🚀 Development Workflow

### 1. Start a Feature

```bash
git checkout -b feature/issue-123-feature-name
```

Create feature in:
- Backend: New handler in appropriate microservice
- Frontend: New Vue component or Pinia store
- Tests: Write tests FIRST (TDD)

### 2. Test-Driven Development (TDD)

**All new features require tests FIRST:**

```csharp
// ✅ Write test first
[Fact]
public async Task CreateProduct_WithValidData_ReturnsSuccess()
{
    // Arrange, Act, Assert...
}

// Then implement the feature
public async Task CreateProductAsync(CreateProductRequest request)
{
    // Implementation
}
```

**Coverage targets:**
- Business logic: 80-90%
- Critical paths: 100%
- Controllers/UI: 50%+

### 3. Code Review Checklist

Before merging, verify:
- ✅ Tests written FIRST (TDD)
- ✅ All tests passing locally
- ✅ Code follows .copilot-specs.md standards
- ✅ No hardcoded secrets or sensitive data
- ✅ Multitenant isolation verified
- ✅ Error handling with Result pattern
- ✅ Type-safe error codes (not magic strings)
- ✅ Documentation updated

## 🏗️ Architecture Overview

### Microservices Structure
```
AppHost (Orchestration)
├── ServiceDefaults (Shared config)
├── auth-service (Authentication)
├── tenant-service (Tenant Management)
├── catalog-service (Product Catalog)
├── order-service (Orders)
└── payment-service (Payments)
```

Each service:
- 🔒 Owns its own database
- 📨 Communicates via Wolverine messaging
- ✅ Implements own health checks
- 📝 Has own tests (xUnit)

### Communication Patterns

**Commands** (Request-Reply):
```csharp
var result = await messageBus.InvokeAsync(
    new CreateProductCommand(tenantId, sku, name)
);
```

**Events** (Publish-Subscribe):
```csharp
await messageBus.PublishAsync(
    new ProductCreatedEvent(tenantId, productId, sku)
);
```

See [.copilot-specs.md Section 12](.copilot-specs.md) for Wolverine patterns.

## 🧪 Testing Approach

### Unit Tests (xUnit + Moq)
```csharp
// Test one behavior in isolation
[Fact]
public async Task GetProduct_WithValidId_ReturnsProduct()
{
    var mock = new Mock<IRepository>();
    mock.Setup(r => r.GetAsync(id)).ReturnsAsync(product);
    
    var service = new ProductService(mock.Object);
    var result = await service.GetProductAsync(id);
    
    Assert.NotNull(result);
}
```

### Integration Tests (TestContainers)
```csharp
// Test with actual database
[Fact]
public async Task CreateProduct_PersistsToDatabase()
{
    var container = new PostgreSqlContainer().Start();
    // Run test against real DB
}
```

### Frontend Tests (Vitest + Vue Test Utils)
```typescript
it('renders product form', () => {
  const wrapper = mount(ProductForm)
  expect(wrapper.find('input[type="text"]').exists()).toBe(true)
})
```

## 🔐 Security & Multitenant

**Every feature must:**
- ✅ Include tenant ID in requests
- ✅ Filter queries by TenantId
- ✅ Validate tenant ownership before access
- ✅ Use parameterized queries (prevent SQL injection)
- ✅ Never expose other tenant data in errors

Example:
```csharp
// ✅ CORRECT: Includes tenant filter
public async Task<Product> GetProductAsync(Guid tenantId, Guid productId)
{
    return await _context.Products
        .Where(p => p.TenantId == tenantId && p.Id == productId)
        .FirstOrDefaultAsync();
}

// ❌ WRONG: Missing tenant filter
public async Task<Product> GetProductAsync(Guid productId)
{
    return await _context.Products.FindAsync(productId);
}
```

## 📝 Code Standards (Quick Reference)

Full standards in [.copilot-specs.md](.copilot-specs.md):

### C# Guidelines
- Enable `#nullable enable` in all files
- Use Result pattern instead of exceptions for expected errors
- Use type-safe error codes (not magic strings)
- XML documentation on all public types
- Fluent validation for input validation
- Async/await with `ConfigureAwait(false)`

### Vue.js Guidelines
- Use Composition API with `<script setup>`
- TypeScript for all components
- Pinia stores for state management
- Vitest + Vue Test Utils for tests
- Tailwind CSS for styling

### Bash Scripts
- Shebang: `#!/usr/bin/env bash`
- Error handling: `set -euo pipefail`
- Portable paths (Windows/Linux/macOS)
- Always quote variables: `"$VAR"`

See [.copilot-specs.md](.copilot-specs.md) for complete standards.

## 🐛 Debugging

### VS Code Launch Configs
1. Press **F5**
2. Select a configuration:
   - **Aspire + Frontend**: Full stack debugging
   - **Backend Only**: Just microservices
   - **Frontend Only**: Just Vue app

See [docs/guides/DEBUG_QUICK_REFERENCE.md](docs/guides/DEBUG_QUICK_REFERENCE.md) for detailed debugging.

### Common Issues

**Port Already in Use:**
```bash
# Windows
netstat -ano | findstr :9000
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :9000 | grep LISTEN | awk '{print $2}' | xargs kill -9
```

**Tests Failing:**
- Ensure database is running (or using InMemory)
- Check test isolation (no shared state)
- Verify mocks are set up correctly

**Frontend Not Loading:**
- Clear node_modules: `rm -rf frontend/node_modules`
- Reinstall: `npm install --prefix frontend`
- Check Vite is running on port 5173

## 📚 Related Documentation

| Resource | Purpose |
|----------|---------|
| [.copilot-specs.md](.copilot-specs.md) | Complete coding standards (24 sections) |
| [GETTING_STARTED.md](GETTING_STARTED.md) | First-time setup |
| [docs/architecture/ASPIRE_GUIDE.md](docs/architecture/ASPIRE_GUIDE.md) | Microservices & Aspire |
| [docs/guides/TESTING_GUIDE.md](docs/guides/TESTING_GUIDE.md) | Detailed testing guide |
| [docs/features/](docs/features/) | Feature implementation docs |

---

**Tldr:** Write tests first, follow .copilot-specs.md, verify tenant isolation, use Result pattern, commit with clear messages.
