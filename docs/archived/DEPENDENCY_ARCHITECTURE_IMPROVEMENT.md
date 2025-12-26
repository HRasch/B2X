# 🏗️ Verbesserung der Projektarchitektur - Abhängigkeitsverwaltung

## 📊 Aktuelle Situation

### Identifizierte Probleme

1. **Inkonsistente Abhängigkeitsdeklaration**
   - CatalogService: Deklariert direkt `WolverineFx`, `Elasticsearch`, `Quartz`
   - AuthService: Sehr minimale Abhängigkeiten
   - Localization Service: Begrenzte Abhängigkeiten
   - **Problem**: Services haben unterschiedliche Abhängigkeiten, obwohl sie ähnliche Patterns verwenden

2. **Fehlende Cross-Cutting Concerns**
   - Shared Dependencies nicht zentral definiiert
   - ServiceDefaults.csproj als ProjectReference, aber nicht alle Services nutzen es
   - Wolverine-Pakete nur in CatalogService direkt deklariert

3. **Indirekte Abhängigkeiten**
   - Services verlassen sich auf transitive Abhängigkeiten von ProjectReferences
   - Kann zu "Assembly not found" Fehlern führen

4. **Build-Reihenfolge-Abhängigkeiten**
   - Compiler kann Abhängigkeiten nicht auflösen, wenn shared Projects nicht korrekt als NuGet-Pakete oder ProjectReferences deklariert sind

### Fehler-Beispiele
```
error CS0246: WolverineFx nicht gefunden
error CS0246: IElasticsearchClient nicht gefunden
```
➜ Ursache: Conditional includes oder transitive Abhängigkeits-Probleme

---

## ✅ Empfohlene Lösungen (3 Strategien)

### **STRATEGIE 1: Shared NuGet Packages für Cross-Cutting Concerns** ⭐ EMPFOHLEN

**Ansatz**: Erstelle lokale NuGet Packages für häufig verwendete Abhängigkeiten

#### Struktur:
```
backend/
├── shared/
│   ├── B2Connect.Shared.Core/
│   │   ├── B2Connect.Shared.Core.csproj (enthält Wolverine Setup, Logging, etc.)
│   │   └── (kein externe dependency files)
│   │
│   ├── B2Connect.Shared.Data/
│   │   └── B2Connect.Shared.Data.csproj (EF Core, Repository Pattern)
│   │
│   ├── B2Connect.Shared.Search/
│   │   └── B2Connect.Shared.Search.csproj (Elasticsearch Integration)
│   │
│   └── B2Connect.Shared.Messaging/
│       └── B2Connect.Shared.Messaging.csproj (RabbitMQ, Wolverine Transport)
│
├── services/
│   ├── CatalogService/
│   │   └── B2Connect.CatalogService.csproj
│   │       └── ProjectReference: Shared.Core, Shared.Data, Shared.Search, Shared.Messaging
│   │
│   ├── auth-service/
│   │   └── B2Connect.AuthService.csproj
│   │       └── ProjectReference: Shared.Core, Shared.Data
│   │
│   └── ...
```

**Vorteile:**
- ✅ Zentrale Verwaltung von Wolverine, Elasticsearch, RabbitMQ Setup
- ✅ Services deklarieren nur ProjectReferences, nicht einzelne Packages
- ✅ Typensicherheit - keine "namespace not found" Fehler
- ✅ Einfach zu testen (Mocking der Shared Packages)
- ✅ Klare Dependency Injection Patterns

**Nachteile:**
- Etwas mehr Initial-Setup
- Shared Packages müssen gewartet werden

---

### **STRATEGIE 2: Erweiterte Directory.Packages.props mit Impliziten PackageReferences**

**Ansatz**: Alle Packages automatisch allen Services verfügbar machen

#### Änderung in Directory.Packages.props:
```xml
<!-- Vor: Nur Versions-Definitionen -->
<PackageVersion Include="WolverineFx" Version="5.9.2" />

<!-- Nach: Auch bei jedem Service automatisch referenziert -->
<!-- Nicht empfohlen für Strategie 2, besser Strategie 1 verwenden -->
```

**Nachteile:**
- ❌ "Implicit Dependency Hell" - Services nutzen Packages, ohne sie zu deklarieren
- ❌ Schwer zu debuggen, wenn ein Package fehlt
- ❌ Services werden undurchsichtig

---

### **STRATEGIE 3: Service-Spezifische Dependency Layering**

**Ansatz**: Erstelle ausgefeilte Abhängigkeits-Profile basierend auf Service-Typ

```
Service Types:
├── CoreServices (Auth, Tenant)
│   └── Minimal: EF Core, JWT, Logging
│
├── DataServices (Catalog, Localization)
│   └── Extended: EF Core, Logging, Wolverine, RabbitMQ
│
├── SearchServices (Search, PIM Sync)
│   └── Full: DataServices + Elasticsearch
│
└── ApiServices (Gateway)
    └── Minimal + YARP
```

**Nachteile:**
- Komplex zu implementieren
- Viele Conditional PackageReference
- Schwer zu dokumentieren

---

## 🎯 Implementierungsplan (STRATEGIE 1)

### Phase 1: Shared Packages Erstellen

#### 1.1 B2Connect.Shared.Core.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- Core Wolverine Setup -->
    <PackageReference Include="WolverineFx" />
    <PackageReference Include="WolverineFx.Http" />
    <PackageReference Include="WolverineFx.PostgreSQL" />
    
    <!-- Logging & Observability -->
    <PackageReference Include="Serilog" />
    <PackageReference Include="Serilog.AspNetCore" />
    <PackageReference Include="Serilog.Sinks.Console" />
    <PackageReference Include="Serilog.Enrichers.Environment" />
    
    <!-- Configuration -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
    <PackageReference Include="Microsoft.Extensions.Logging" />
    <PackageReference Include="Microsoft.Extensions.Configuration" />
  </ItemGroup>
</Project>
```

**Enthält:**
- `WolverineSetup` Extension Method für Program.cs
- `LoggingSetup` Extension Method
- Common Exceptions & Result Types (wenn nicht bereits in shared/types)

---

#### 1.2 B2Connect.Shared.Data.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="../B2Connect.Shared.Core/B2Connect.Shared.Core.csproj" />
    
    <!-- Data Access -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    
    <!-- Validation -->
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
  </ItemGroup>
</Project>
```

---

#### 1.3 B2Connect.Shared.Search.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="../B2Connect.Shared.Core/B2Connect.Shared.Core.csproj" />
    
    <!-- Elasticsearch -->
    <PackageReference Include="Elastic.Clients.Elasticsearch" />
  </ItemGroup>
</Project>
```

---

#### 1.4 B2Connect.Shared.Messaging.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <ProjectReference Include="../B2Connect.Shared.Core/B2Connect.Shared.Core.csproj" />
    
    <!-- Message Queue & Job Scheduling -->
    <PackageReference Include="RabbitMQ.Client" />
    <PackageReference Include="WolverineFx.RabbitMq" />
    <PackageReference Include="Quartz" />
    <PackageReference Include="Quartz.Extensions.Hosting" />
  </ItemGroup>
</Project>
```

---

### Phase 2: Service ProjectReferences Aktualisieren

#### 2.1 CatalogService (VORHER)
```xml
<ItemGroup>
  <!-- ❌ Zu viele direkte PackageReferences -->
  <PackageReference Include="WolverineFx" />
  <PackageReference Include="WolverineFx.Http" />
  <PackageReference Include="WolverineFx.RabbitMq" />
  <PackageReference Include="Elastic.Clients.Elasticsearch" />
  <PackageReference Include="Quartz" />
  <!-- ... 20+ weitere Packages -->
</ItemGroup>
```

#### 2.1 CatalogService (NACHHER)
```xml
<ItemGroup>
  <!-- ✅ Nur Shared Projekt-Abhängigkeiten -->
  <ProjectReference Include="../../shared/types/B2Connect.Types.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Core/B2Connect.Shared.Core.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Data/B2Connect.Shared.Data.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Search/B2Connect.Shared.Search.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Messaging/B2Connect.Shared.Messaging.csproj" />
</ItemGroup>
```

**Vorteil**: Katalog-Service .csproj ist jetzt übersichtlich, alle Abhängigkeiten sind durch ProjectReferences dokumentiert

---

#### 2.2 AuthService (NACHHER)
```xml
<ItemGroup>
  <ProjectReference Include="../ServiceDefaults/B2Connect.ServiceDefaults.csproj" />
  <ProjectReference Include="../../shared/types/B2Connect.Types.csproj" />
  <ProjectReference Include="../../shared/utils/B2Connect.Utils.csproj" />
  <ProjectReference Include="../../shared/middleware/B2Connect.Middleware.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Core/B2Connect.Shared.Core.csproj" />
  <ProjectReference Include="../../shared/B2Connect.Shared.Data/B2Connect.Shared.Data.csproj" />
</ItemGroup>
```

---

### Phase 3: Program.cs Refactoring

#### VORHER (CatalogService):
```csharp
builder.Services
    .AddLogging(/* Manual configuration */)
    .AddSerilog(/* Manual configuration */)
    .AddWolverine(/* Manual configuration */)
    .AddDbContext<CatalogDbContext>()
    // ... 50+ lines of setup
```

#### NACHHER:
```csharp
using B2Connect.Shared.Core.Extensions;
using B2Connect.Shared.Data.Extensions;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.Shared.Search.Extensions;

var builder = WebApplicationBuilder.CreateBuilder(args);

// ✅ Extension Methods aus Shared Packages
builder.Services
    .AddSharedCore(builder.Configuration)
    .AddSharedData(builder.Configuration)
    .AddSharedMessaging(builder.Configuration)
    .AddSharedSearch(builder.Configuration);

// Service-spezifische Configuration
builder.Services
    .AddScoped<ICatalogService, CatalogService>()
    .AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();
app.UseSharedDefaults();  // Logging, Middleware, etc.
app.Run();
```

**Vorteil**: Program.cs ist jetzt 20-30 Zeilen statt 100+, viel übersichtlicher

---

## 🛡️ Ausfallsicherheit (Fehlerbehandlung)

### Szenario 1: Transitive Abhängigkeit fehlt
**VORHER**: 
```
error CS0246: WolverineFx nicht gefunden
```
**NACHHER**: ProjectReference zu Shared.Core sichert Wolverine-Verfügbarkeit

---

### Szenario 2: Service braucht Elasticsearch, aber es nicht deklariert
**VORHER**: Runtime Error wenn nicht transitive verfügbar
**NACHHER**: Compiler-Error wenn Service Shared.Search nicht referenziert → sofort erkannt

---

## 📦 Dependency Graph (Nach Refactoring)

```
B2Connect.Shared.Core
├── WolverineFx
├── Serilog
└── Configuration

B2Connect.Shared.Data
├── B2Connect.Shared.Core
├── EF Core
└── FluentValidation

B2Connect.Shared.Search
├── B2Connect.Shared.Core
└── Elasticsearch

B2Connect.Shared.Messaging
├── B2Connect.Shared.Core
├── RabbitMQ
└── Quartz

CatalogService
├── B2Connect.Shared.Data
├── B2Connect.Shared.Search
└── B2Connect.Shared.Messaging

AuthService
├── B2Connect.Shared.Core
└── B2Connect.Shared.Data

LocalizationService
├── B2Connect.Shared.Core
└── B2Connect.Shared.Data
```

✅ **Klar, transparent, keine "versteckten" Abhängigkeiten**

---

## 🚀 Implementierungsschritte

1. ✅ **Shared Packages erstellen** (10 Min)
   - Create 4 new .csproj files
   - Move PackageReferences from individual services
   - Create Extension Methods für Setup

2. ✅ **Services aktualisieren** (30 Min)
   - Remove direct PackageReferences
   - Add ProjectReferences zu Shared Packages
   - Update using statements in Program.cs

3. ✅ **Testing** (20 Min)
   - `dotnet clean B2Connect.sln && dotnet build`
   - Run unit tests
   - Verify no "namespace not found" errors

4. ✅ **Documentation** (10 Min)
   - Update DEVELOPMENT.md
   - Document new Shared Package pattern
   - Update onboarding guide

---

## 📊 Vorher/Nachher Vergleich

| Aspekt | VORHER | NACHHER |
|--------|--------|---------|
| Services mit `WolverineFx` | Nur CatalogService | Zentral in Shared.Core |
| PackageReferences pro Service | 20-30 | 5-10 |
| Transitive Abhängigkeiten | Unklar | Explizit dokumentiert |
| Program.cs Länge | 80-100 Zeilen | 20-30 Zeilen |
| Fehlerquellen (fehlende Packages) | Hoch | Niedrig |
| Onboarding-Komplexität | Hoch | Niedrig |

---

## ✅ Nächste Schritte

**Sollen wir Strategie 1 implementieren?** Diese würde:
- ✅ 80% der "Namespace not found" Fehler beheben
- ✅ Build-Zeiten verkürzen
- ✅ Neue Services deutlich schneller erstellen
- ✅ Wartbarkeit massiv verbessern

**Zeitaufwand**: ~1-1,5 Stunden für vollständige Implementierung
**Schwierigkeitsgrad**: Mittel (viel Refactoring, aber keine komplizierten Änderungen)
