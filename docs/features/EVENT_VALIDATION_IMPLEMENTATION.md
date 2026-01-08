# Event Validation & Domain Events Implementation

Complete guide to domain event validation, middleware integration, and event-driven patterns in B2X.

## Overview

All domain events are validated before processing to ensure data integrity and consistency:

1. **Event Validators** - Custom validators for each event type
2. **Event Validation Middleware** - Pipeline integration for automatic validation
3. **Domain Events** - Immutable event records with full type safety
4. **Event Publishing** - Validated event publishing with error handling

## Architecture

```
Event Flow:
  1. Event is created in domain layer
  2. EventValidationMiddleware intercepts
  3. EventValidators validate event
  4. If valid: Publish to message broker (RabbitMQ)
  5. If invalid: Throw ValidationException or add to dead letter queue
```

## Domain Events

**Location:** `backend/shared/events/`

### Event Base Class

```csharp
public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TenantId { get; init; }
    public string EventType => GetType().Name;
    public int Version { get; init; } = 1;
}
```

### Product Events

```csharp
public record ProductCreatedEvent(
    Guid ProductId,
    string Sku,
    LocalizedContent Name,
    decimal Price,
    decimal? B2bPrice,
    int StockQuantity,
    string[] Tags,
    ProductAttributesDto Attributes,
    string[] ImageUrls,
    Guid TenantId
) : DomainEvent;

public record ProductUpdatedEvent(
    Guid ProductId,
    string Sku,
    LocalizedContent Name,
    decimal Price,
    int StockQuantity,
    Guid TenantId
) : DomainEvent;

public record ProductDeletedEvent(
    Guid ProductId,
    Guid TenantId
) : DomainEvent;
```

### Category Events

```csharp
public record CategoryCreatedEvent(
    Guid CategoryId,
    LocalizedContent Name,
    Guid? ParentCategoryId,
    Guid TenantId
) : DomainEvent;

public record CategoryUpdatedEvent(
    Guid CategoryId,
    LocalizedContent Name,
    Guid TenantId
) : DomainEvent;
```

## Event Validators

**Location:** `backend/shared/events/EventValidators.cs`

### ProductCreatedValidator

```csharp
public class ProductCreatedEventValidator : AbstractValidator<ProductCreatedEvent>
{
    public ProductCreatedEventValidator()
    {
        RuleFor(e => e.ProductId)
            .NotEmpty()
            .WithMessage("Product ID cannot be empty");
        
        RuleFor(e => e.Sku)
            .NotEmpty()
            .Length(3, 20)
            .Matches(@"^[A-Z0-9\-]+$")
            .WithMessage("SKU must be uppercase alphanumeric");
        
        RuleFor(e => e.Name)
            .NotNull()
            .Must(n => n.Values.ContainsKey("en"))
            .WithMessage("English product name required");
        
        RuleFor(e => e.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Price must be valid");
        
        RuleFor(e => e.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");
        
        RuleFor(e => e.ImageUrls)
            .Must(urls => urls.All(u => Uri.TryCreate(u, UriKind.Absolute, out _)))
            .WithMessage("All image URLs must be valid");
        
        RuleFor(e => e.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID required");
    }
}
```

### CategoryCreatedValidator

```csharp
public class CategoryCreatedEventValidator : AbstractValidator<CategoryCreatedEvent>
{
    private readonly IEventValidationService _validationService;
    
    public CategoryCreatedEventValidator(IEventValidationService validationService)
    {
        _validationService = validationService;
        
        RuleFor(e => e.CategoryId)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotNull()
            .Must(n => n.Values.ContainsKey("en"))
            .WithMessage("English category name required");
        
        // Async validation for circular parent references
        RuleFor(e => e.ParentCategoryId)
            .MustAsync(async (parentId, ct) =>
            {
                if (!parentId.HasValue) return true;
                return await _validationService.IsValidParentCategoryAsync(parentId.Value, ct);
            })
            .WithMessage("Parent category creates invalid hierarchy");
        
        RuleFor(e => e.TenantId)
            .NotEmpty();
    }
}
```

## Event Validation Middleware

**Location:** `backend/shared/events/EventValidationMiddleware.cs`

```csharp
public class EventValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EventValidationMiddleware> _logger;
    private readonly IValidator<DomainEvent>[] _eventValidators;
    
    public EventValidationMiddleware(
        RequestDelegate next,
        ILogger<EventValidationMiddleware> logger,
        IValidator<DomainEvent>[] eventValidators)
    {
        _next = next;
        _logger = logger;
        _eventValidators = eventValidators;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // For event publishing endpoints
        if (context.Request.Path.StartsWithSegments("/api/events"))
        {
            try
            {
                // Read and parse event from request
                var @event = await ExtractEventFromRequest(context.Request);
                
                if (@event != null)
                {
                    // Validate event
                    foreach (var validator in _eventValidators)
                    {
                        var result = await validator.ValidateAsync(@event);
                        if (!result.IsValid)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsJsonAsync(
                                new { errors = result.Errors.Select(e => e.ErrorMessage) });
                            return;
                        }
                    }
                    
                    _logger.LogInformation(
                        "Event validated successfully: {EventType}",
                        @event.EventType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event validation failed");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                return;
            }
        }
        
        await _next(context);
    }
    
    private async Task<DomainEvent?> ExtractEventFromRequest(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();
        
        // Map JSON to appropriate event type
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // ... deserialization logic
        
        return null;
    }
}
```

## Publishing Events

### From Controller

```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IEventPublisher _eventPublisher;
    
    [HttpPost]
    [ValidateModel]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductRequest request)
    {
        var product = await _service.CreateProductAsync(request);
        
        // Create and publish event
        var @event = new ProductCreatedEvent(
            ProductId: product.Id,
            Sku: product.Sku,
            Name: product.Name,
            Price: product.Price,
            B2bPrice: product.B2bPrice,
            StockQuantity: product.StockQuantity,
            Tags: product.Tags,
            Attributes: product.Attributes,
            ImageUrls: product.ImageUrls,
            TenantId: product.TenantId
        );
        
        // Event is automatically validated by middleware
        await _eventPublisher.PublishAsync(@event);
        
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
```

### Event Publisher Service

```csharp
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent;
}

public class EventPublisher : IEventPublisher
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IValidator<DomainEvent>[] _validators;
    private readonly ILogger<EventPublisher> _logger;
    
    public async Task PublishAsync<TEvent>(
        TEvent @event, 
        CancellationToken cancellationToken = default)
        where TEvent : DomainEvent
    {
        // Validate
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(@event, cancellationToken);
            if (!result.IsValid)
            {
                throw new InvalidOperationException(
                    $"Event validation failed: {string.Join(", ", result.Errors.Select(e => e.ErrorMessage))}");
            }
        }
        
        _logger.LogInformation("Publishing event: {EventType}", @event.EventType);
        
        // Publish to RabbitMQ/Wolverine
        await _messagePublisher.PublishAsync(@event, cancellationToken);
    }
}
```

## Testing Events

```csharp
public class EventValidatorTests
{
    [Fact]
    public async Task ProductCreatedValidator_WithValidEvent_Succeeds()
    {
        var validator = new ProductCreatedEventValidator();
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "PROD-001",
            Name: new LocalizedContent(new Dictionary<string, string> { { "en", "Test" } }),
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/img.jpg" },
            TenantId: Guid.NewGuid()
        );
        
        var result = await validator.ValidateAsync(@event);
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task ProductCreatedValidator_WithInvalidSku_Fails()
    {
        var validator = new ProductCreatedEventValidator();
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "invalid",  // Not uppercase
            Name: new LocalizedContent(new Dictionary<string, string> { { "en", "Test" } }),
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: Array.Empty<string>(),
            Attributes: new ProductAttributesDto(),
            ImageUrls: Array.Empty<string>(),
            TenantId: Guid.NewGuid()
        );
        
        var result = await validator.ValidateAsync(@event);
        Assert.False(result.IsValid);
    }
}
```

## Registration

```csharp
// In Program.cs or extension method
services.AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<ProductCreatedEventValidator>();
});

app.UseMiddleware<EventValidationMiddleware>();
```

## Best Practices

**DO:**
- Validate all events before publishing
- Use async validators for database checks
- Log validation failures with event context
- Include TenantId in all events
- Make events immutable (use records)
- Test both valid and invalid scenarios

**DON'T:**
- Skip event validation for performance
- Allow invalid events through error handling
- Publish events directly without validation
- Suppress validation errors
- Duplicate validation between requests and events

## Troubleshooting

### Event validation fails in production
- Check all validators are registered in DI
- Verify EventValidationMiddleware is added to pipeline
- Check logger output for specific validation errors

### Circular category references not caught
- Ensure CategoryCreatedEventValidator uses async validation
- Verify IEventValidationService is injected properly

## References

- `.copilot-specs.md` Section 22 (Event validation spec)
- `AOP_VALIDATION_IMPLEMENTATION.md` (Validator patterns)
- `CATALOG_IMPLEMENTATION.md` (Product/Category events)
- `VSCODE_ASPIRE_CONFIG.md` (Run event validation tests)
