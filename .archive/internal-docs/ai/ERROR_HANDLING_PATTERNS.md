# 🛡️ Error Handling Patterns

**Audience**: Backend developers  
**Purpose**: Consistent error handling across all services  
**Critical**: Wrong patterns cause cascading failures across contexts

---

## Error Hierarchy

```
Exception (Base)
├── ValidationException
│   └── Input validation, business rule violations
├── NotFoundException
│   └── Resource not found (404)
├── DomainException
│   └── Business logic violation
├── ConcurrencyException
│   └── Optimistic locking conflict
├── TenantException
│   └── Tenant isolation breach
└── InfrastructureException
    ├── DatabaseException
    ├── SearchException
    ├── CacheException
    └── MessageBusException
```

---

## Pattern 1: Validation Errors

**When**: User input is invalid

**Structure**:
1. Throw `ValidationException` with field names
2. Include reason message
3. Middleware maps to 400 Bad Request

**Example**:

```csharp
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // ✅ CORRECT: Validation before persistence
        if (string.IsNullOrWhiteSpace(cmd.Name))
            throw new ValidationException(
                fieldName: "Name",
                message: "Product name is required");
        
        if (cmd.Price <= 0)
            throw new ValidationException(
                fieldName: "Price",
                message: "Price must be greater than 0");
        
        if (cmd.Name.Length > 255)
            throw new ValidationException(
                fieldName: "Name",
                message: "Product name cannot exceed 255 characters");
        
        var product = new Product { Name = cmd.Name, Price = cmd.Price };
        await _repo.AddAsync(product, ct);
        return new CreateProductResponse { Id = product.Id };
    }
}

// Exception Definition
public class ValidationException : Exception
{
    public string FieldName { get; set; }
    public object AttemptedValue { get; set; }
    
    public ValidationException(string fieldName, string message)
        : base(message)
    {
        FieldName = fieldName;
    }
}

// API Response (Middleware maps this)
{
    "error": {
        "type": "ValidationError",
        "field": "Name",
        "message": "Product name is required",
        "statusCode": 400
    }
}
```

---

## Pattern 2: Business Logic Exceptions

**When**: Domain rule is violated

**Structure**:
1. Throw `DomainException` with business reason
2. Include context (which product, which rule)
3. Middleware maps to 422 Unprocessable Entity

**Example**:

```csharp
public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // Check business rules
        var existing = await _repo.GetByNameAsync(cmd.Name, ct);
        
        if (existing != null)
            throw new DomainException(
                code: "PRODUCT_ALREADY_EXISTS",
                message: $"A product named '{cmd.Name}' already exists",
                details: new { productId = existing.Id });
        
        // Category must exist
        var category = await _categoryRepo.GetByIdAsync(cmd.CategoryId, ct);
        if (category == null)
            throw new DomainException(
                code: "CATEGORY_NOT_FOUND",
                message: $"Category {cmd.CategoryId} does not exist");
        
        var product = new Product 
        { 
            Name = cmd.Name, 
            Price = cmd.Price,
            CategoryId = cmd.CategoryId 
        };
        
        await _repo.AddAsync(product, ct);
        return new CreateProductResponse { Id = product.Id };
    }
}

// Exception Definition
public class DomainException : Exception
{
    public string Code { get; set; }
    public object Details { get; set; }
    
    public DomainException(string code, string message, object details = null)
        : base(message)
    {
        Code = code;
        Details = details;
    }
}

// API Response (Middleware maps this)
{
    "error": {
        "type": "DomainError",
        "code": "PRODUCT_ALREADY_EXISTS",
        "message": "A product named 'Widget' already exists",
        "details": { "productId": "550e8400-e29b-41d4-a716-446655440000" },
        "statusCode": 422
    }
}
```

---

## Pattern 3: Not Found Errors

**When**: Resource doesn't exist

**Structure**:
1. Throw `NotFoundException` with resource type and ID
2. Middleware maps to 404 Not Found

**Example**:

```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    
    public async Task<ProductDetailResponse> GetProduct(
        Guid productId,
        CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        
        if (product == null)
            throw new NotFoundException(
                resourceType: "Product",
                resourceId: productId.ToString());
        
        return new ProductDetailResponse { ... };
    }
    
    public async Task<UpdateProductResponse> UpdateProduct(
        Guid productId,
        UpdateProductCommand cmd,
        CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        
        if (product == null)
            throw new NotFoundException(
                resourceType: "Product",
                resourceId: productId.ToString());
        
        product.Name = cmd.Name;
        product.Price = cmd.Price;
        
        await _repo.UpdateAsync(product, ct);
        return new UpdateProductResponse { Id = product.Id };
    }
}

// Exception Definition
public class NotFoundException : Exception
{
    public string ResourceType { get; set; }
    public string ResourceId { get; set; }
    
    public NotFoundException(string resourceType, string resourceId)
        : base($"{resourceType} with ID '{resourceId}' not found")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

// API Response
{
    "error": {
        "type": "NotFound",
        "resource": "Product",
        "resourceId": "550e8400-e29b-41d4-a716-446655440000",
        "message": "Product with ID '550e8400-e29b-41d4-a716-446655440000' not found",
        "statusCode": 404
    }
}
```

---

## Pattern 4: Concurrency Errors

**When**: Optimistic locking conflict (stale data)

**Structure**:
1. Use `RowVersion` for optimistic locking
2. Throw `ConcurrencyException` on conflict
3. Client retries or resolves conflict

**Example**:

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // ✅ For optimistic locking
    [Timestamp] // EF Core attribute
    public byte[] RowVersion { get; set; }
}

public class ProductService
{
    public async Task<UpdateProductResponse> UpdateProduct(
        Guid productId,
        UpdateProductCommand cmd,
        CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        
        if (product == null)
            throw new NotFoundException("Product", productId.ToString());
        
        product.Name = cmd.Name;
        product.Price = cmd.Price;
        
        try
        {
            // EF Core checks RowVersion match
            await _repo.UpdateAsync(product, ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Stale data - someone else modified it
            throw new ConcurrencyException(
                resourceType: "Product",
                resourceId: productId.ToString(),
                message: "Product was modified by another user");
        }
        
        return new UpdateProductResponse { Id = product.Id };
    }
}

// Exception Definition
public class ConcurrencyException : Exception
{
    public string ResourceType { get; set; }
    public string ResourceId { get; set; }
    
    public ConcurrencyException(
        string resourceType,
        string resourceId,
        string message)
        : base(message)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

// API Response
{
    "error": {
        "type": "ConcurrencyError",
        "resource": "Product",
        "resourceId": "550e8400-e29b-41d4-a716-446655440000",
        "message": "Product was modified by another user",
        "statusCode": 409
    }
}
```

---

## Pattern 5: Cross-Context Errors

**When**: One context calls another and fails

**Structure**:
1. Throw specific exception from calling service
2. Catch and re-throw as `ServiceException`
3. Include original error for debugging

**Example**:

```csharp
// Catalog Service (Store Context)
public class CatalogService
{
    private readonly IHttpClientFactory _httpFactory;
    
    public async Task<ProductDetailResponse> GetProductWithReviews(
        Guid productId,
        CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(productId, ct);
        
        if (product == null)
            throw new NotFoundException("Product", productId.ToString());
        
        try
        {
            // Call another service (hypothetical Reviews context)
            var client = _httpFactory.CreateClient("ReviewsService");
            var reviews = await client.GetAsync(
                $"/api/reviews?productId={productId}", ct);
            
            reviews.EnsureSuccessStatusCode();
            
            return new ProductDetailResponse 
            { 
                Product = product,
                Reviews = await reviews.Content.ReadAsAsync<List<ReviewDto>>()
            };
        }
        catch (HttpRequestException ex)
        {
            // Service unavailable - degrade gracefully
            throw new ServiceException(
                serviceName: "ReviewsService",
                operation: "GetReviews",
                message: "Failed to fetch reviews",
                innerException: ex,
                canRetry: true);
        }
    }
}

// Exception Definition
public class ServiceException : Exception
{
    public string ServiceName { get; set; }
    public string Operation { get; set; }
    public bool CanRetry { get; set; }
    
    public ServiceException(
        string serviceName,
        string operation,
        string message,
        Exception innerException = null,
        bool canRetry = false)
        : base(message, innerException)
    {
        ServiceName = serviceName;
        Operation = operation;
        CanRetry = canRetry;
    }
}

// API Response
{
    "error": {
        "type": "ServiceError",
        "service": "ReviewsService",
        "operation": "GetReviews",
        "message": "Failed to fetch reviews",
        "canRetry": true,
        "statusCode": 503
    }
}
```

---

## Pattern 6: Event Publishing Errors

**When**: Publishing an event fails (Wolverine)

**Structure**:
1. Try-catch around `IMessageBus.PublishAsync`
2. Decide: Fail or log & continue
3. Include event details for debugging

**Example**:

```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IMessageBus _messageBus;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        var product = new Product { Name = cmd.Name, Price = cmd.Price };
        await _repo.AddAsync(product, ct);
        
        var @event = new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        
        try
        {
            // Publish to event bus (triggers Search indexing, etc.)
            await _messageBus.PublishAsync(@event, ct);
        }
        catch (Exception ex)
        {
            // ✅ Product persisted but event not published
            // Search will be out of sync until next reindex
            throw new MessageBusException(
                eventType: nameof(ProductCreatedEvent),
                resourceId: product.Id.ToString(),
                message: "Failed to publish ProductCreatedEvent",
                severity: ErrorSeverity.Warning, // Not critical
                innerException: ex);
        }
        
        return new CreateProductResponse { Id = product.Id };
    }
}

// Exception Definition
public enum ErrorSeverity { Warning, Critical }

public class MessageBusException : Exception
{
    public string EventType { get; set; }
    public string ResourceId { get; set; }
    public ErrorSeverity Severity { get; set; }
    
    public MessageBusException(
        string eventType,
        string resourceId,
        string message,
        ErrorSeverity severity = ErrorSeverity.Critical,
        Exception innerException = null)
        : base(message, innerException)
    {
        EventType = eventType;
        ResourceId = resourceId;
        Severity = severity;
    }
}

// Logging
logger.LogWarning(
    exception: ex,
    message: "Failed to publish {EventType} for resource {ResourceId}. " +
             "Data persisted but event processing may be delayed.",
    args: new[] { @event.GetType().Name, product.Id });
```

---

## Pattern 7: Logging Errors Safely

**When**: Logging errors without exposing sensitive data

**Structure**:
1. Log exception type and code
2. Log business context (not sensitive data)
3. Log correlation ID for tracing
4. Mask sensitive fields

**Example**:

```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        string correlationId,
        CancellationToken ct)
    {
        try
        {
            // ✅ Business logic
            if (cmd.Price < 0)
                throw new ValidationException(
                    "Price", "Price cannot be negative");
            
            var product = new Product { Name = cmd.Name, Price = cmd.Price };
            await _repo.AddAsync(product, ct);
            
            _logger.LogInformation(
                "Product created: {ProductId}, Name: {Name}, Correlation: {CorrelationId}",
                product.Id, cmd.Name, correlationId);
            
            return new CreateProductResponse { Id = product.Id };
        }
        catch (ValidationException ex)
        {
            // ✅ Log validation error safely
            _logger.LogWarning(
                "Validation failed: {FieldName} = {Message}, Correlation: {CorrelationId}",
                ex.FieldName, ex.Message, correlationId);
            
            throw;
        }
        catch (Exception ex)
        {
            // ✅ Log unexpected error with correlation ID
            _logger.LogError(
                exception: ex,
                message: "Unexpected error creating product. " +
                         "Correlation: {CorrelationId}, " +
                         "ProductName: {ProductName}",
                args: new[] { correlationId, cmd.Name });
            
            throw new InfrastructureException(
                operation: "CreateProduct",
                message: "An unexpected error occurred",
                innerException: ex);
        }
    }
}

// ❌ WRONG: Don't log like this
_logger.LogError("Error: {Error}", ex.ToString()); // Too much detail
_logger.LogError("Error for user {UserId}", userId); // Don't expose IDs

// ✅ CORRECT: Log like this
_logger.LogError(ex, "Operation failed: {Operation}, Correlation: {Id}",
    nameof(CreateProduct), correlationId);
```

---

## Middleware: Global Exception Handler

**Purpose**: Map exceptions to HTTP responses consistently

**Example**:

```csharp
// Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;
        
        var response = ex switch
        {
            ValidationException ve => (
                statusCode: 400,
                body: new { 
                    type = "ValidationError",
                    field = ve.FieldName,
                    message = ve.Message
                }),
            
            NotFoundException nfe => (
                statusCode: 404,
                body: new { 
                    type = "NotFound",
                    resource = nfe.ResourceType,
                    resourceId = nfe.ResourceId,
                    message = nfe.Message
                }),
            
            DomainException de => (
                statusCode: 422,
                body: new { 
                    type = "DomainError",
                    code = de.Code,
                    message = de.Message,
                    details = de.Details
                }),
            
            ConcurrencyException ce => (
                statusCode: 409,
                body: new { 
                    type = "ConcurrencyError",
                    message = ce.Message
                }),
            
            _ => (
                statusCode: 500,
                body: new { 
                    type = "InternalServerError",
                    message = "An unexpected error occurred"
                })
        };
        
        context.Response.StatusCode = response.statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response.body);
    });
});
```

---

## Error Handling Checklist

- [ ] All validation errors throw `ValidationException`?
- [ ] All business rule violations throw `DomainException`?
- [ ] Missing resources throw `NotFoundException`?
- [ ] Concurrent updates throw `ConcurrencyException`?
- [ ] Cross-service calls catch `HttpRequestException`?
- [ ] Event publishing wrapped in try-catch?
- [ ] Logs include correlation ID?
- [ ] Logs don't expose sensitive data?
- [ ] Global exception handler configured?
- [ ] Correct HTTP status codes used?

---

## HTTP Status Codes

| Exception | Status | Meaning |
|-----------|--------|---------|
| ValidationException | 400 | Bad Request |
| DomainException | 422 | Unprocessable Entity |
| NotFoundException | 404 | Not Found |
| ConcurrencyException | 409 | Conflict |
| ServiceException | 503 | Service Unavailable |
| MessageBusException | 500+ | Server Error (severity-dependent) |
| Unexpected | 500 | Internal Server Error |

---

## References

- [Wolverine Pattern Reference](WOLVERINE_PATTERN_REFERENCE.md)
- [DDD Bounded Contexts](DDD_BOUNDED_CONTEXTS_REFERENCE.md)
- [FEATURE_IMPLEMENTATION_PATTERNS](FEATURE_IMPLEMENTATION_PATTERNS.md)

---

*Updated: 30. Dezember 2025*  
*Critical for: Logging, debugging, API reliability*
