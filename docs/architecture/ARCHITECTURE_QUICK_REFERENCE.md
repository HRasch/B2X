# Architecture Quick Reference

**B2X Updated Architecture** | 28. Dezember 2025

---

## Services (Alle Wolverine-basiert)

| Service | Port | Purpose | Example Endpoint |
|---------|------|---------|-----------------|
| **Identity** | 7002 | Auth, JWT, Passkeys | `POST /login`, `POST /register` |
| **Tenancy** | 7003 | Multi-tenant isolation | `POST /tenant/create`, `GET /tenant/{id}` |
| **Catalog** | 7005 | Products, Categories | `GET /products`, `POST /product/create` |
| **CMS** | 7006 | Pages, Content | `GET /pages`, `POST /page/create` |
| **Theming** | 7008 | UI Themes, Layouts | `GET /themes`, `POST /theme/apply` |
| **Localization** | 7004 | i18n, Translations | `GET /translations/{locale}` |
| **Search** | 9300 | Elasticsearch, Full-text | `GET /search?q=term` |

---

## Creating a New Wolverine Service

### 1️⃣ Service Class (NOT MediatR!)

```csharp
// ✅ CORRECT
public class MyService
{
    public async Task<MyResponse> MyMethod(
        MyCommand request,
        CancellationToken cancellationToken)
    {
        return new MyResponse { /* ... */ };
    }
}

// ❌ WRONG
public record MyCommand : IRequest<MyResponse>;
public class Handler : IRequestHandler<MyCommand, MyResponse> { }
```

### 2️⃣ Register in DI

```csharp
builder.Services.AddScoped<MyService>();
// Wolverine auto-discovers & creates HTTP endpoint
// POST /mymethod (from class + method name)
```

### 3️⃣ Event Handler (if needed)

```csharp
public class MyEventHandlers
{
    public async Task Handle(SomeEvent @event)
    {
        // Auto-called when SomeEvent is published
    }
}
```

### 4️⃣ Publish Event

```csharp
await _messageBus.PublishAsync(new ProductCreatedEvent(productId, tenantId));
```

---

## Frontend Integration

### API Call Pattern

```typescript
// Service layer (not in component!)
export const catalogService = {
  async getProducts(tenantId: string) {
    return api.get('/catalog/products', {
      headers: { 'X-Tenant-ID': tenantId }
    })
  }
}

// Component
const products = await catalogService.getProducts(tenantId)
```

---

## CLI Commands

### Installation
```bash
dotnet build backend/CLI/B2X.CLI/B2X.CLI.csproj
dotnet tool install --global --add-source ./nupkg B2X.CLI
```

### Usage
```bash
# Auth
B2X auth create-user email@example.com --password secret123

# Tenant
B2X tenant create "Company" --admin-email admin@company.com

# Products
B2X product create "SKU-001" "Name" --price 99.99

# System
B2X migrate --service Identity
B2X seed --service Catalog --file data.json
B2X status --all
```

---

## Multi-Tenancy

**ALWAYS include tenant ID:**

```csharp
// Backend
var products = await _repository.GetByTenantAsync(tenantId);

// Frontend
fetch('...', {
  headers: { 'X-Tenant-ID': tenantId }
})
```

---

## Folder Structure

```
backend/
├── Domain/
│   ├── Identity/src/
│   ├── Catalog/src/
│   ├── CMS/src/
│   └── ...
├── CLI/B2X.CLI/
├── Orchestration/
└── shared/

Frontend/
├── Store/          (Port 5173)
└── Admin/          (Port 5174)
```

---

## Key Files

- **Instructions**: [.github/copilot-instructions.md](.github/copilot-instructions.md)
- **CLI Guide**: [CLI_IMPLEMENTATION_GUIDE.md](../guides/CLI_IMPLEMENTATION_GUIDE.md)
- **Architecture**: [ARCHITECTURE_UPDATE_SUMMARY.md](ARCHITECTURE_UPDATE_SUMMARY.md)
- **Specs**: [docs/APPLICATION_SPECIFICATIONS.md](docs/guides/index.md)

---

## Wolverine Patterns

✅ **DO:**
- Use `public async Task<T>` methods in services
- Plain POCO command/event classes
- Publish events via `IMessageBus`
- Auto-discovered handlers with `Handle()` methods
- Register services with `AddScoped`

❌ **DON'T:**
- Use `IRequest<T>`, `IRequestHandler`
- Add `AddMediatR()`
- Use `[ApiController]`, `[HttpPost]`
- Direct service-to-service HTTP calls
- Hardcode secrets in code

---

## Deployment

```bash
# Aspire (recommended)
dotnet run --project AppHost/B2X.AppHost.csproj

# Manual
dotnet run --project backend/Domain/Identity/src/B2X.Identity.csproj

# CLI
B2X start
```

---

**Last Updated**: 28. Dezember 2025 | **Status**: ✅ COMPLETE

