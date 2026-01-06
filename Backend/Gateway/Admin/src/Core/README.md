# Core Layer (Innerster Ring)

## Beschreibung
Die Core Layer enthält die Geschäftslogik des Systems, unabhängig von Frameworks und technischen Details.

## Inhalte

### Entities
- Domain entities (Geschäftsobjekte)
- Business rules und Validierungen
- Value Objects

### Interfaces
- Repository interfaces
- Service interfaces
- Port definitions

### Exceptions
- Custom business exceptions
- Domain-specific error handling

## Wichtige Regeln
- ✅ Keine Abhängigkeiten auf andere Layer
- ✅ Geschäftsregeln sind hier implementiert
- ✅ Framework-agnostisch
- ❌ Keine Framework-Imports
- ❌ Keine Datenbank-Dependencies
- ❌ Keine UI-Abhängigkeiten

## Beispielstruktur
```
Core/
  ├── Entities/
  │   ├── Product.cs
  │   ├── Category.cs
  │   └── Brand.cs
  ├── Interfaces/
  │   ├── IProductRepository.cs
  │   └── ICategoryRepository.cs
  └── Exceptions/
      └── InvalidProductException.cs
```
