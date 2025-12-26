# Presentation Layer (Äußerster Ring)

## Beschreibung
Die Presentation Layer definiert die API-Schnittstelle und verarbeitet HTTP-Requests.

## Inhalte

### Controllers
- API endpoints
- Request/Response handling
- HTTP routing

### Middleware
- Authentication/Authorization
- Error handling
- Request/Response logging

### Configuration
- Dependency injection
- Service registration
- API documentation

## Wichtige Regeln
- ✅ Abhängig von allen inneren Layer
- ✅ HTTP-Details hier
- ✅ Controller/Endpoints definieren
- ❌ Business Logic vermeiden
- ❌ Datenbankzugriff direkt

## Beispielstruktur
```
Presentation/
  ├── Controllers/
  │   ├── ProductsController.cs
  │   ├── CategoriesController.cs
  │   └── BrandsController.cs
  ├── Middleware/
  │   ├── ErrorHandlingMiddleware.cs
  │   └── AuthenticationMiddleware.cs
  └── Configuration/
      └── ServiceConfiguration.cs
```
