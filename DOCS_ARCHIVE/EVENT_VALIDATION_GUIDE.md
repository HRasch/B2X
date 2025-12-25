# FluentValidation für Domain Events

## 📋 Überblick

Implementierung von FluentValidation für **Domain Events** - die in Ihrem System publiziert werden, bevor sie zum Message Broker (RabbitMQ, Wolverine) gesendet werden.

## 🎯 Zweck

Domain Events sollten valide sein, bevor sie publiziert werden:
- ✅ Konsistenz zwischen Services sicherstellen
- ✅ Fehlerhafte Events früh erkennen
- ✅ Datenintegrität durch das System gewährleisten
- ✅ Automatische Validierung bei Publikation

## 🏗️ Architektur

```
Event entsteht in Service
        ↓
[EventValidationFilter] - Validiert automatisch
        ↓
Valide? Ja → [IEventPublisher] → RabbitMQ/Wolverine
        ↓ Nein
    Fehler → Exception + Logging
```

## 📂 Dateien

### Validatoren

**Location**: `/backend/services/CatalogService/src/Validators/EventValidators.cs`

```csharp
// Base Event Validator
public class ProductCreatedEventValidator : AbstractValidator<ProductCreatedEvent>
{
    public ProductCreatedEventValidator()
    {
        // Base properties
        RuleFor(e => e.EventId).NotEmpty();
        RuleFor(e => e.Timestamp).NotEqual(default(DateTime));
        
        // Domain-specific properties
        RuleFor(e => e.Sku).NotEmpty().Length(3, 50).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(e => e.Price).GreaterThan(0);
        RuleFor(e => e.Tags).Must(t => t.Length <= 20);
    }
}
```

### AOP Interceptor

**Location**: `/backend/shared/aop/EventValidationInterceptor.cs`

```csharp
// Validation Service
public interface IEventValidationService
{
    Task<(bool IsValid, List<string> Errors)> ValidateEventAsync(DomainEvent @event);
    Task PublishValidatedEventAsync<T>(T @event) where T : DomainEvent;
}

// Validated Publisher
public class ValidatedEventPublisher : IEventPublisher
{
    // Validates before publishing
    public async Task PublishAsync<T>(T @event) where T : DomainEvent
}
```

### Base Event Validators

**Location**: `/backend/shared/validators/EventValidators.cs`

```csharp
// Base validator mit gemeinsamen Regeln
public abstract class DomainEventValidator<TEvent> : AbstractValidator<TEvent>
    where TEvent : DomainEvent
{
    protected DomainEventValidator()
    {
        RuleFor(e => e.EventId).NotEmpty();
        RuleFor(e => e.Timestamp).LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1));
        RuleFor(e => e.AggregateId).NotEqual(Guid.Empty);
        // ... mehr Basis-Regeln
    }
}
```

## 💡 Verwendungsbeispiele

### In einem Service

```csharp
public class ProductService
{
    private readonly IEventValidationService _eventValidation;
    private readonly IEventPublisher _eventPublisher;

    public async Task CreateProductAsync(CreateProductRequest request)
    {
        var product = await _repository.CreateAsync(request);
        
        // Event erstellen
        var @event = new ProductCreatedEvent(
            ProductId: product.Id,
            Sku: product.Sku,
            Name: product.Name,
            // ... alle Properties
            TenantId: _tenantContext.TenantId
        );

        // Validiert automatisch und publiziert
        await _eventPublisher.PublishAsync(@event);
        // oder
        await _eventValidation.PublishValidatedEventAsync(@event);
    }
}
```

### Mit manueller Validierung

```csharp
public async Task PublishEventAsync(DomainEvent @event)
{
    var (isValid, errors) = await _eventValidation.ValidateEventAsync(@event);
    
    if (!isValid)
    {
        _logger.LogError("Event validation failed: {Errors}", 
            string.Join("; ", errors));
        throw new InvalidOperationException("Event is invalid");
    }

    // Event ist valide
    await _eventPublisher.PublishAsync(@event);
}
```

## 🧪 Validierungsregeln für ProductCreatedEvent

### Basis-Event-Properties

| Property | Regel | Beispiel |
|----------|-------|---------|
| EventId | NotEmpty | auto-generiert |
| Timestamp | ≤ jetzt | z.B. 2025-12-26T10:30:00Z |
| EventType | = "product.created" | konstant |
| AggregateType | = "Product" | konstant |

### Produkt-Eigenschaften

| Property | Regel | Beispiel |
|----------|-------|---------|
| ProductId | NotEmpty | Guid |
| Sku | [A-Z0-9\-]{3,50} | "PROD-001" |
| Name | MaxLength(255) | "Bluetooth Speaker" |
| Price | > 0, 2 Dezimalstellen | 99.99 |
| B2bPrice | ≤ Price | 89.99 |
| StockQuantity | ≥ 0 | 100 |
| Tags | ≤ 20, maximal 50 Zeichen | ["electronics", "audio"] |
| ImageUrls | ≤ 10, gültige URIs | ["https://..."] |

## 🔄 Request Lifecycle mit Event-Validierung

```
Client POST /api/products
        ↓
[ValidateModel] Filter - Request DTO validiert
        ↓
Service.CreateProductAsync()
        ↓
Event erstellen: new ProductCreatedEvent(...)
        ↓
[EventValidationInterceptor]
    ├─ ProductCreatedEventValidator lädt
    ├─ Alle Regeln prüfen
    └─ Gültig? Ja → Weitergabe zu Publisher
        ├─ Gültig? Nein → Exception werfen
        └─ Log: "Event validation failed"
        ↓
[ValidatedEventPublisher]
        ↓
RabbitMQ/Wolverine Publish
        ↓
Response 201 Created
```

## 📝 Tests schreiben

```csharp
[Fact]
public async Task ProductCreatedEventValidator_WithValidData_Succeeds()
{
    // Arrange
    var @event = new ProductCreatedEvent(
        ProductId: Guid.NewGuid(),
        Sku: "TEST-001",
        Name: "Test Product",
        Price: 99.99m,
        TenantId: Guid.NewGuid());

    // Act
    var result = await validator.ValidateAsync(@event);

    // Assert
    Assert.True(result.IsValid);
}

[Fact]
public async Task ProductCreatedEventValidator_WithInvalidSku_Fails()
{
    // Arrange
    var @event = new ProductCreatedEvent(
        ProductId: Guid.NewGuid(),
        Sku: "invalid",  // Lowercase - invalid
        Name: "Test Product",
        Price: 99.99m,
        TenantId: Guid.NewGuid());

    // Act
    var result = await validator.ValidateAsync(@event);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
}
```

## 🛠️ Setup in Program.cs

```csharp
// In Program.cs
builder.Services.AddAopAndValidation(
    typeof(Program),  // Catalog Service
    typeof(EventValidators)  // Event validators assembly
);

// Oder einzeln
builder.Services.AddEventValidation(typeof(Program));
builder.Services.UseEventValidation();
```

## 🔌 Event Validator Factory

```csharp
public class EventValidatorFactory
{
    public async Task<bool> ValidateEventAsync(DomainEvent @event)
    {
        // Prüft alle registrierten Validatoren
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(@event);
            if (!result.IsValid) return false;
        }
        return true;
    }
}
```

## 📊 Fehlerformat bei ungültigen Events

```json
{
    "error": "Event validation failed",
    "eventType": "product.created",
    "eventId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "timestamp": "2025-12-26T10:30:00Z",
    "details": [
        {
            "property": "Sku",
            "message": "SKU must be between 3 and 50 characters"
        },
        {
            "property": "Price",
            "message": "Price must be greater than 0"
        }
    ]
}
```

## 🚀 Best Practices

### ✅ DO

```csharp
// ✅ Events validieren vor Publikation
var (isValid, errors) = await _validation.ValidateEventAsync(@event);
if (!isValid) throw new InvalidOperationException("...");

// ✅ Aussagekräftige Fehlermeldungen
RuleFor(e => e.Sku)
    .Matches(@"^[A-Z0-9\-]+$")
    .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");

// ✅ Base Validator für gemeinsame Regeln nutzen
public class ProductEventValidator : DomainEventValidator<dynamic> { }

// ✅ Validierung in die Service-Schicht integrieren
await _eventPublisher.PublishAsync(@event);  // Automatisch validiert
```

### ❌ DON'T

```csharp
// ❌ Events ohne Validierung publizieren
await _messageBus.PublishAsync(@event);  // Keine Validierung!

// ❌ Unklare Fehlermeldungen
RuleFor(e => e.Sku).NotEmpty();  // Zu generisch

// ❌ Duplikation von Validierungslogik
// Event-Validatoren + Request-Validatoren für gleiche Regeln

// ❌ Synchrone Validierung in kritischen Pfaden
var result = validator.Validate(@event);  // Blocking!
```

## 📈 Metriken & Logging

Die Validierung loggt automatisch:

```
INFO: Event validation succeeded for product.created (Id: f47ac10b-58cc-4372-a567-0e02b2c3d479)
WARN: Event validation failed for product.created (Id: ...). Errors: SKU invalid
INFO: Published validated event product.created (Id: ...)
```

## 🔗 Integration mit anderen Services

Für neue Services:

1. **Validatoren erstellen**
   ```csharp
   public class OrderCreatedEventValidator : AbstractValidator<OrderCreatedEvent>
   {
       // Regeln definieren
   }
   ```

2. **In Program.cs registrieren**
   ```csharp
   builder.Services.AddEventValidation(typeof(Program));
   ```

3. **In Service nutzen**
   ```csharp
   await _eventPublisher.PublishAsync(new OrderCreatedEvent(...));
   ```

## 📞 Troubleshooting

### Event wird mit Fehler validiert?

```csharp
var result = await validator.ValidateAsync(@event);
foreach (var error in result.Errors)
{
    Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
}
```

### Validierung wird nicht ausgeführt?

1. Prüfen: Sind Validatoren registriert?
   ```csharp
   builder.Services.AddEventValidation(typeof(Program));
   ```

2. Prüfen: Nutzen Sie ValidatedEventPublisher?
   ```csharp
   var publisher = serviceProvider.GetRequiredService<IEventPublisher>();
   ```

3. Prüfen: Befinden sich Validatoren im Assembly?

## 📚 Weiterführende Ressourcen

- [FluentValidation Dokumentation](https://fluentvalidation.net/)
- [AOP_FLUENT_VALIDATION_GUIDE.md](./AOP_FLUENT_VALIDATION_GUIDE.md)
- [EventValidatorsTests.cs](../Tests/CatalogService.Tests/EventValidatorsTests.cs)

---

**Zusammenfassung**: Domain Events sind kritische Komponenten für System-Konsistenz. Die Validierung vor Publikation garantiert, dass nur valide Events das System durchlaufen, was Fehler früh erkennt und Datenqualität sicherstellt.
