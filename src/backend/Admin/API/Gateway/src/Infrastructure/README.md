# Infrastructure Layer

## Beschreibung
Die Infrastructure Layer enthält technische Implementierungen wie Datenbankzugriff, externe Services und andere Adapter.

## Inhalte

### Repositories
- Implementierung der Repository Interfaces
- Datenbankzugriff
- Query implementations

### Data
- DbContext
- Entity Framework mappings
- Migrations

### External Services
- HTTP Clients
- Service integrations
- Caching implementations

### Persistence
- Database initialization
- Seed data

## Wichtige Regeln
- ✅ Abhängig von Core und Application Layer
- ✅ Repository Interfaces implementieren
- ✅ Alle technischen Details hier
- ❌ Keine direkten Controller-Abhängigkeiten
- ❌ Business Logic vermeiden

## Beispielstruktur
```
Infrastructure/
  ├── Repositories/
  │   ├── ProductRepository.cs
  │   └── CategoryRepository.cs
  ├── Data/
  │   ├── AdminDbContext.cs
  │   └── Configurations/
  ├── External/
  │   └── ExternalServiceClient.cs
  └── Persistence/
      └── DatabaseInitializer.cs
```
