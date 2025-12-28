# B2Connect Architecture Update Summary

**Status**: ✅ UPDATED  
**Date**: 28. Dezember 2025  
**Changes**: Projektstruktur + Wolverine Microservices + CLI Tool

---

## Was hat sich geändert?

### 1️⃣ Projektstruktur (Aktualisiert)

**Vorher:**
```
BoundedContexts/
├── Store/API/          # Gateway
├── Admin/API/          # Gateway
└── Shared/
    ├── Identity/
    └── Tenancy/
```

**Nachher:**
```
Domain/                 # Einzelne Microservices
├── Identity/          # Wolverine Service (Port 7002)
├── Tenancy/           # Wolverine Service (Port 7003)
├── Catalog/           # Wolverine Service (Port 7005)
├── CMS/               # Wolverine Service (Port 7006)
├── Theming/           # Wolverine Service (Port 7008)
├── Localization/      # Wolverine Service (Port 7004)
└── Search/            # Wolverine Service (Port 9300)

CLI/
└── B2Connect.CLI/     # Command Line Tool (NEW!)
    ├── Commands/
    ├── Services/
    └── Program.cs
```

### 2️⃣ Messaging-Architektur (Wolverine)

**Keine separaten Gateways mehr!**

Alle Services sind **selbstständige Wolverine-Microservices** mit:
- ✅ HTTP Endpoints (via Wolverine auto-discovery)
- ✅ Event-basierte Kommunikation (Event Bus)
- ✅ Direkter Frontend-Zugriff

**Vorteile:**
- 🚀 Schneller (keine Gateway-Indirektion)
- 🔄 Einfacher zu skalieren (jeder Service unabhängig)
- 📡 Event-driven für Echtzeit-Updates
- 🛠️ Leichter zu testen (isolierte Services)

### 3️⃣ CLI Tool (NEU!)

Alle Operationen jetzt auch über Kommandozeile ausführbar:

```bash
# User Management
b2connect auth create-user john@example.com --password secret123

# Tenant Management
b2connect tenant create "Acme Corp" --admin-email admin@acme.com

# Product Management
b2connect product create "SKU-001" "Product Name" --price 99.99

# System Operations
b2connect migrate --service Identity
b2connect seed --service Catalog --file data.json
b2connect status --all
```

---

## Service Ports (aktualisiert)

| Service | Port | Typ |
|---------|------|-----|
| Identity | 7002 | Wolverine Microservice |
| Tenancy | 7003 | Wolverine Microservice |
| Localization | 7004 | Wolverine Microservice |
| Catalog | 7005 | Wolverine Microservice |
| CMS | 7006 | Wolverine Microservice |
| Theming | 7008 | Wolverine Microservice |
| Search | 9300 | Wolverine Microservice |
| Frontend Store | 5173 | Vue.js |
| Frontend Admin | 5174 | Vue.js |
| Aspire Dashboard | 15500 | Orchestration |

---

## Kommunikation zwischen Services

### Asynchron (Empfohlen)
```csharp
// Service A publiziert Event
await _messageBus.PublishAsync(new ProductCreatedEvent(...));

// Service B abonniert automatisch
public class MyEventHandlers
{
    public async Task Handle(ProductCreatedEvent @event)
    {
        // Automatisch aufgerufen von Wolverine
        await _searchService.IndexProductAsync(@event.ProductId);
    }
}
```

### HTTP (Frontend)
```typescript
// Frontend ruft Service direkt auf
const response = await fetch('http://localhost:7005/catalog/products', {
  headers: { 'X-Tenant-ID': tenantId }
})
```

**Keine Service-zu-Service HTTP-Calls!** Nutze Event Bus.

---

## CLI Tool Struktur

```
backend/CLI/B2Connect.CLI/
├── Commands/
│   ├── AuthCommands/        # User management
│   ├── TenantCommands/      # Tenant CRUD
│   ├── ProductCommands/     # Catalog operations
│   ├── ContentCommands/     # CMS operations
│   └── SystemCommands/      # Migrations, Health
├── Services/
│   ├── CliHttpClient.cs
│   ├── ConfigurationService.cs
│   └── ConsoleOutputService.cs
└── Program.cs
```

---

## Wolverine Pattern (wichtig!)

### ✅ CORRECT

```csharp
// 1. Plain POCO Command
public class CheckRegistrationTypeCommand
{
    public string Email { get; set; }
}

// 2. Service mit public async methods
public class CheckRegistrationTypeService
{
    public async Task<Response> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        // Wolverine erzeugt automatisch:
        // POST /checkregistrationtype
    }
}

// 3. DI Registration
builder.Services.AddScoped<CheckRegistrationTypeService>();
```

### ❌ WRONG (MediatR - nicht verwenden!)

```csharp
// Nicht verwenden!
public record Command(...) : IRequest<Response>;
public class Handler : IRequestHandler<Command, Response> { }
builder.Services.AddMediatR();
```

---

## Konfiguration aktualisieren

### copilot-instructions.md
✅ Aktualisiert mit:
- Neue Projektstruktur (Domain/ statt BoundedContexts/)
- Wolverine als Primary Pattern
- CLI Tool Dokumentation
- Keine Gateway-Services mehr
- Event-basierte Kommunikation

### New Files
✅ `CLI_IMPLEMENTATION_GUIDE.md` - Komplette CLI Dokumentation
✅ `B2Connect.slnx` - Solution File (bereits vorhanden)

---

## Startanleitung

### 1. Services starten (mit Aspire)
```bash
cd backend/Orchestration
dotnet run
# Dashboard: http://localhost:15500
```

### 2. Oder manuell (einzelne Services)
```bash
dotnet run --project backend/Domain/Identity/src/B2Connect.Identity.csproj
dotnet run --project backend/Domain/Catalog/src/B2Connect.Catalog.csproj
```

### 3. CLI Tool installieren
```bash
cd backend/CLI/B2Connect.CLI
dotnet build
dotnet tool install --global --add-source ./nupkg B2Connect.CLI
```

### 4. CLI verwenden
```bash
b2connect auth create-user test@example.com
b2connect tenant list
b2connect system status
```

---

## Wichtige Links

- 📖 [copilot-instructions.md](.github/copilot-instructions.md) - Updated Architecture Guide
- 🛠️ [CLI_IMPLEMENTATION_GUIDE.md](CLI_IMPLEMENTATION_GUIDE.md) - CLI Details
- 🏗️ [WOLVERINE_ARCHITECTURE_ANALYSIS.md](WOLVERINE_ARCHITECTURE_ANALYSIS.md) - Pattern Analysis
- 🎯 [APPLICATION_SPECIFICATIONS.md](docs/APPLICATION_SPECIFICATIONS.md) - Specs

---

## Next Steps

1. ✅ Projektstruktur aktualisiert
2. ✅ Wolverine als Primary documented
3. ✅ CLI Tool planned & documented
4. ⏳ CLI Implementation (create-user, tenant CRUD)
5. ⏳ Refactor existing services if needed
6. ⏳ Update CI/CD pipelines

---

## Checklist für Developers

Bei neuen Features:
- [ ] Nutze Wolverine Services (nicht Gateway)
- [ ] Plain POCO Commands (nicht IRequest)
- [ ] Events für Service-Kommunikation (nicht HTTP)
- [ ] CLI Command hinzufügen (falls Operator-Funktion)
- [ ] Tests für Command Handler schreiben
- [ ] Tenant ID immer mitpassen (X-Tenant-ID Header)

---

**Status**: 🟢 **COMPLETE** - Ready for Implementation

