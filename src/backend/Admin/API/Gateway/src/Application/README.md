# Application Layer

## Beschreibung
Die Application Layer orchestriert die Business Logic und definiert Use Cases. Sie kommuniziert mit der Core Layer über Interfaces.

## Inhalte

### DTOs (Data Transfer Objects)
- Request/Response models
- Command/Query models
- Mapping von Domain zu DTO

### Services
- Application services (Use Cases)
- Service orchestration
- Transaction handling

### Handlers
- CQRS Command Handlers
- Query Handlers
- Event Handlers

## Wichtige Regeln
- ✅ Abhängig von Core Layer
- ✅ Business Logic orchestrieren
- ✅ Use Cases implementieren
- ❌ Keine direkte Datenbank-Queries
- ❌ Keine HTTP-Dependencies
- ❌ Repository-Interfaces verwenden

## Beispielstruktur
```
Application/
  ├── DTOs/
  │   ├── ProductDto.cs
  │   ├── CreateProductRequest.cs
  │   └── ProductListResponse.cs
  ├── Services/
  │   ├── IProductService.cs
  │   └── ProductService.cs
  └── Handlers/
      ├── CreateProductHandler.cs
      └── UpdateProductHandler.cs
```
