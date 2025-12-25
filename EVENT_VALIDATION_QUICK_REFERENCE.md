# Event Validation - Quick Reference

## 🚀 Schnellstart (2 Min)

### 1. Validator erstellen

```csharp
// File: /backend/services/CatalogService/src/Validators/EventValidators.cs
public class MyEventValidator : AbstractValidator<MyEvent>
{
    public MyEventValidator()
    {
        RuleFor(e => e.EventId).NotEmpty();
        RuleFor(e => e.AggregateId).NotEmpty();
        RuleFor(e => e.Sku).NotEmpty().Length(3, 50);
    }
}
```

### 2. In Program.cs registrieren

```csharp
builder.Services.AddEventValidation(typeof(Program));
```

### 3. Event publizieren

```csharp
var @event = new ProductCreatedEvent(...);
await _eventPublisher.PublishAsync(@event);  // Automatisch validiert!
```

---

## 📋 Häufige Validierungsregeln

### String-Validierung

```csharp
// Nicht leer
RuleFor(e => e.Sku).NotEmpty().WithMessage("SKU is required");

// Länge
RuleFor(e => e.Name).MinimumLength(1).MaximumLength(255);

// Muster (Regex)
RuleFor(e => e.Sku).Matches(@"^[A-Z0-9\-]+$").WithMessage("Invalid format");

// Email
RuleFor(e => e.Email).EmailAddress();

// URL
RuleFor(e => e.ImageUrl).Must(url => Uri.TryCreate(url, UriKind.Absolute, out _));
```

### Numerische Validierung

```csharp
// Größer als
RuleFor(e => e.Price).GreaterThan(0);

// Im Bereich
RuleFor(e => e.StockQuantity).GreaterThanOrEqualTo(0);

// Dezimalstellen
RuleFor(e => e.Price)
    .Must(p => decimal.Round(p, 2) == p)
    .WithMessage("Max 2 decimal places");

// Vergleich
RuleFor(e => e.B2bPrice)
    .LessThanOrEqualTo(e => e.Price)
    .WithMessage("B2B price cannot exceed regular price");
```

### GUID-Validierung

```csharp
// Nicht leer
RuleFor(e => e.ProductId).NotEqual(Guid.Empty);
RuleFor(e => e.TenantId).NotEqual(Guid.Empty);
```

### Array/Collection-Validierung

```csharp
// Array-Größe
RuleFor(e => e.Tags).Must(t => t.Length <= 20).WithMessage("Max 20 tags");

// Jedes Element prüfen
RuleFor(e => e.Tags)
    .ForEach(tag => tag
        .NotEmpty()
        .MaximumLength(50));

// Array nicht null
RuleFor(e => e.ImageUrls).NotNull();
```

### Zeitstempel-Validierung

```csharp
// Nicht in der Zukunft
RuleFor(e => e.Timestamp)
    .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1));

// Nach einem Datum
RuleFor(e => e.CreatedDate)
    .GreaterThan(new DateTime(2025, 1, 1));
```

---

## 🧪 Unit Tests

### Gültiges Event testen

```csharp
[Fact]
public async Task Validator_WithValidData_Succeeds()
{
    var @event = new ProductCreatedEvent(
        ProductId: Guid.NewGuid(),
        Sku: "TEST-001",
        Name: "Test Product",
        Price: 99.99m,
        TenantId: Guid.NewGuid());

    var result = await validator.ValidateAsync(@event);
    
    Assert.True(result.IsValid);
}
```

### Ungültiges Event testen

```csharp
[Fact]
public async Task Validator_WithInvalidSku_Fails()
{
    var @event = new ProductCreatedEvent(
        ProductId: Guid.NewGuid(),
        Sku: "invalid",  // Zu kurz!
        Name: "Test",
        Price: 99.99m,
        TenantId: Guid.NewGuid());

    var result = await validator.ValidateAsync(@event);
    
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
}
```

---

## 🔍 Validierung manuell ausführen

### Option 1: Mit Validator Klasse

```csharp
var validator = new ProductCreatedEventValidator();
var result = await validator.ValidateAsync(@event);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

### Option 2: Mit EventValidationService

```csharp
private readonly IEventValidationService _validation;

var (isValid, errors) = await _validation.ValidateEventAsync(@event);

if (!isValid)
{
    Console.WriteLine(string.Join("; ", errors));
}
```

### Option 3: Mit PublishValidatedEventAsync

```csharp
try
{
    await _validation.PublishValidatedEventAsync(@event);
    // Event ist valide, bereit zu publizieren
}
catch (InvalidOperationException ex)
{
    // Event-Validierung fehlgeschlagen
    Console.WriteLine(ex.Message);
}
```

---

## 🏗️ Base Event Validator

Verwenden Sie abstrakte Base Validators für gemeinsame Regeln:

```csharp
public abstract class DomainEventValidator<TEvent> : AbstractValidator<TEvent>
    where TEvent : DomainEvent
{
    protected DomainEventValidator()
    {
        // Alle Events müssen diese erfüllen:
        RuleFor(e => e.EventId).NotEmpty();
        RuleFor(e => e.Timestamp).LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1));
        RuleFor(e => e.AggregateId).NotEqual(Guid.Empty);
    }
}

// Erben Sie davon für spezifische Validierung:
public class ProductCreatedEventValidator 
    : DomainEventValidator<ProductCreatedEvent>
{
    public ProductCreatedEventValidator()
    {
        // Zusätzliche Regeln
        RuleFor(e => e.Sku).NotEmpty();
    }
}
```

---

## 📂 Dateien & Locations

| Datei | Zweck |
|-------|-------|
| `backend/shared/aop/EventValidationInterceptor.cs` | Validierungs-Service & Publisher |
| `backend/shared/validators/EventValidators.cs` | Base Event Validators |
| `backend/services/CatalogService/src/Validators/EventValidators.cs` | Service-spezifische Validatoren |
| `backend/Tests/CatalogService.Tests/EventValidatorsTests.cs` | Unit Tests |

---

## 🔗 Integration mit IEventPublisher

```csharp
public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : DomainEvent;
}

// In Ihrem Service:
public class ProductService
{
    private readonly IEventPublisher _publisher;

    public async Task CreateProductAsync(CreateProductRequest request)
    {
        var product = await _repository.CreateAsync(request);
        
        var @event = new ProductCreatedEvent(...);
        
        // Automatisch validiert:
        await _publisher.PublishAsync(@event);
    }
}
```

---

## ⚠️ Häufige Fehler

### Fehler: "Event validation failed"

**Problem**: Event erfüllt Validierungsregeln nicht

**Lösung**: Prüfen Sie die Fehler:
```csharp
var result = await validator.ValidateAsync(@event);
foreach (var error in result.Errors)
    Console.WriteLine(error.ErrorMessage);
```

### Fehler: Validatoren nicht registriert

**Problem**: `IEventPublisher` nicht injizierbar

**Lösung**: Registrieren Sie Validatoren:
```csharp
builder.Services.AddEventValidation(typeof(Program));
```

### Fehler: Falsche Event-Daten

**Problem**: Event hat ungültige Werte

**Lösung**: Validieren Sie bevor Sie das Event erstellen:
```csharp
if (string.IsNullOrEmpty(sku)) throw new ArgumentException("Invalid SKU");

var @event = new ProductCreatedEvent(
    ProductId: productId,
    Sku: sku,  // Jetzt sicher gültig
    ...
);
```

---

## 📊 Validierungsergebnis auslesen

```csharp
var result = await validator.ValidateAsync(@event);

// Boolean Check
if (result.IsValid) { /* OK */ }

// Errors sammeln
var errorMessages = result.Errors
    .Select(e => e.ErrorMessage)
    .ToList();

// Nach Property filtern
var skuErrors = result.Errors
    .Where(e => e.PropertyName == "Sku")
    .ToList();
```

---

## 🚀 Komplettes Beispiel

```csharp
public class ProductService
{
    private readonly IEventPublisher _eventPublisher;

    public async Task CreateProductAsync(CreateProductRequest request)
    {
        // Validiere Request
        var requestValidator = new CreateProductRequestValidator();
        await requestValidator.ValidateAndThrowAsync(request);

        // Erstelle Product
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku,
            Name = request.Name,
            Price = request.Price,
            TenantId = _tenantContext.TenantId
        };
        await _repository.CreateAsync(product);

        // Erstelle Event
        var @event = new ProductCreatedEvent(
            ProductId: product.Id,
            Sku: product.Sku,
            Name: product.Name,
            Description: request.Description,
            Category: request.Category,
            Price: product.Price,
            B2bPrice: request.B2bPrice,
            StockQuantity: request.StockQuantity,
            Tags: request.Tags,
            Attributes: request.Attributes,
            ImageUrls: request.ImageUrls,
            TenantId: product.TenantId);

        // Publiziere Event (automatisch validiert)
        await _eventPublisher.PublishAsync(@event);
    }
}
```

---

## 📚 Weitere Ressourcen

- [EVENT_VALIDATION_GUIDE.md](EVENT_VALIDATION_GUIDE.md) - Umfassender Guide
- [EventValidatorsTests.cs](backend/Tests/CatalogService.Tests/EventValidatorsTests.cs) - Test-Beispiele
- [FluentValidation Docs](https://docs.fluentvalidation.net/)

---

**Version**: 1.0  
**Last Updated**: 2025-12-26
