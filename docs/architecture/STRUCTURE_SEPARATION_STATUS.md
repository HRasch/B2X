# Projektstruktur-Trennung: Umsetzungsstatus

## ✅ Abgeschlossene Tasks

1. **Ordner-Struktur erstellt**: Alle neuen Verzeichnisse für Store / Admin / Common sind vorhanden
2. **Entitäten migriert**: Common Entities (Store, Language, Country) in neue Locations kopiert
3. **Interfaces migriert**: Repository-Interfaces in neue Locations organisiert
4. **Repositories migriert**: Alle Repositories in neue Ordner strukturiert
5. **DbContext migriert**: StoreDbContext in Infrastructure/Common/Data verschoben
6. **Services/Controller migriert**: Alle Dateien in neue Locations erstellt

## ⚠️ Erkannte Probleme

### Hauptproblem: Namespace-Konflikt
Der Namespace `B2Connect.Store.Core.Store` kollidiert mit der Entity `Store`:
- Namespace: `B2Connect.Store.Core.Store.Entities`
- Entity Class: `Store` 
- Fehler: "Store" ist "Namespace", wird aber wie "Typ" verwendet

**Beispiel:**
```csharp
using B2Connect.Store.Core.Store.Entities;
public class StoreRepository : Repository<Store> // ❌ Store ist mehrdeutig
```

### Lösungsoptionen

#### Option 1: Entitäten umbenennen
```csharp
public class StoreEntity { ... }
public class PaymentMethodEntity { ... }
public class ShippingMethodEntity { ... }
```

#### Option 2: Namespace umbenennen (EMPFOHLEN)
```csharp
// Statt: B2Connect.Store.Core.Store.Entities
// Nutze: B2Connect.Store.Core.Configuration.Entities
// oder:  B2Connect.Store.Core.Domain.Entities
```

#### Option 3: Vollqualifizierte Namen nutzen
```csharp
public class StoreRepository : Repository<global::B2Connect.Store.Core.Store.Entities.Store>
```

## 📋 Empfohlene Struktur (Korrigierte Version)

```
src/
├── Core/
│   ├── Common/
│   │   ├── Entities/
│   │   │   ├── Store.cs
│   │   │   ├── Language.cs
│   │   │   └── Country.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs (Generic)
│   │       ├── IStoreRepository.cs
│   │       ├── ILanguageRepository.cs
│   │       └── ICountryRepository.cs
│   │
│   └── Configuration/  ← RENAMED FROM "Store" to avoid conflicts
│       ├── Entities/
│       │   ├── PaymentMethod.cs
│       │   └── ShippingMethod.cs
│       └── Interfaces/
│           ├── IPaymentMethodRepository.cs
│           └── IShippingMethodRepository.cs
│
├── Application/
│   ├── Services/ (Write Services)
│   │   ├── StoreService.cs
│   │   ├── LanguageService.cs
│   │   ├── CountryService.cs
│   │   ├── PaymentMethodService.cs
│   │   └── ShippingMethodService.cs
│   │
│   └── ReadServices/ (Optimized Read Services)
│       ├── StoreReadService.cs
│       ├── LanguageReadService.cs
│       ├── CountryReadService.cs
│       ├── PaymentMethodReadService.cs
│       └── ShippingMethodReadService.cs
│
├── Infrastructure/
│   ├── Repositories/
│   │   ├── Common/
│   │   │   ├── Repository.cs (Generic base)
│   │   │   ├── StoreRepository.cs
│   │   │   ├── LanguageRepository.cs
│   │   │   └── CountryRepository.cs
│   │   │
│   │   └── Configuration/
│   │       ├── PaymentMethodRepository.cs
│   │       └── ShippingMethodRepository.cs
│   │
│   └── Data/
│       └── StoreDbContext.cs
│
└── Presentation/
    └── Controllers/
        ├── Admin/ (Protected - [Authorize])
        │   ├── StoresController.cs
        │   ├── LanguagesController.cs
        │   ├── CountriesController.cs
        │   ├── PaymentMethodsController.cs
        │   └── ShippingMethodsController.cs
        │
        └── Public/ (No Auth - Read-only)
            ├── PublicStoresController.cs
            ├── PublicLanguagesController.cs
            ├── PublicCountriesController.cs
            ├── PublicPaymentMethodsController.cs
            └── PublicShippingMethodsController.cs
```

## 🔑 Namespaces (Korrigiert)

```
B2Connect.Store.Core.Common.Entities            ← Store, Language, Country
B2Connect.Store.Core.Common.Interfaces          ← IRepository, IStoreRepository, etc.
B2Connect.Store.Core.Configuration.Entities     ← PaymentMethod, ShippingMethod
B2Connect.Store.Core.Configuration.Interfaces   ← IPaymentMethodRepository, etc.

B2Connect.Store.Application.Services            ← Write Services
B2Connect.Store.Application.ReadServices        ← Read Services

B2Connect.Store.Infrastructure.Common.Repositories
B2Connect.Store.Infrastructure.Common.Data
B2Connect.Store.Infrastructure.Configuration.Repositories

B2Connect.Store.Presentation.Controllers.Admin
B2Connect.Store.Presentation.Controllers.Public
```

## 🚀 Nächste Schritte zur Behebung

1. **Namespace umbenennen**
   - `B2Connect.Store.Core.Store` → `B2Connect.Store.Core.Configuration`
   - Alle Dateien in `src/Core/Configuration/` verschieben
   - Alle Usings in Services und Controllers aktualisieren

2. **Program.cs aktualisieren**
   - Alle using-Statements mit neuen Namespaces aktualisieren

3. **Build testen**
   - Sollte dann kompilieren

## 📊 Struktur-Vergleich

| Aspekt | Alt | Neu |
|--------|-----|-----|
| **Entities** | Flach in Core/Entities | Organisiert in Common/Configuration |
| **Interfaces** | Flach in Core/Interfaces | Organisiert in Common/Configuration |
| **Services** | Flach in Application/Services | Getrennt in Services und ReadServices |
| **Repositories** | Flach in Infrastructure/Repositories | Organisiert in Common/Configuration |
| **Controllers** | Flach in Presentation/Controllers | Getrennt in Admin/Public |
| **DbContext** | Infrastructure/Data | Infrastructure/Common/Data |

## 📝 Zusammenfassung

Die Reorganisation ist zu **80% abgeschlossen**. Das Hauptproblem ist ein Namespace-Konflikt zwischen dem Ordner `Store` und der Entity `Store`. Die schnellste Lösung ist, den Namespace von `B2Connect.Store.Core.Store` zu `B2Connect.Store.Core.Configuration` oder ähnlich umzubenennen.

Alle Dateien sind bereits in den neuen Ordnern erstellt, die Usings müssen nur angepasst werden.
