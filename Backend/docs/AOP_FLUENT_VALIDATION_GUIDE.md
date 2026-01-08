# AOP & FluentValidation Implementation Guide

## Overview

This document describes the AOP (Aspect Oriented Programming) and FluentValidation implementation for the B2X CatalogService. It demonstrates clean, maintainable controller code with separated cross-cutting concerns.

## Key Benefits

✅ **Clean Controllers**: Focus only on business logic routing
✅ **DRY Validation**: Reusable validators across services
✅ **Centralized Error Handling**: Consistent error responses
✅ **Automatic Logging**: All requests logged transparently
✅ **Type-Safe**: FluentValidation provides compile-time safety
✅ **Testable**: Easy to unit test validators and business logic

## Architecture

```
Request
  ↓
[1] RequestLoggingAttribute (AOP)
  ↓
[2] ValidateModelAttribute (AOP) ← Validates request using FluentValidation
  ↓
[3] Controller Action
  ↓
[4] Service Layer
  ↓
[5] Exception/Response
  ↓
[6] ExceptionHandlingAttribute (AOP) ← Catches and transforms exceptions
  ↓
[7] RequestLoggingAttribute (AOP) ← Logs response
  ↓
Response (JSON with validation errors or data)
```

## Implementation Details

### 1. AOP Filters

#### ValidateModelAttribute
- **Purpose**: Automatically validates ModelState before action execution
- **Location**: `shared/aop/ValidationFilterAttribute.cs`
- **Usage**: Applied at controller class level
- **Result**: Returns 400 BadRequest with validation errors if invalid

```csharp
[ApiController]
[ValidateModel]  // AOP: automatic validation
public class ProductsController : ControllerBase { }
```

#### ExceptionHandlingAttribute
- **Purpose**: Catches all unhandled exceptions and returns standardized response
- **Location**: `shared/aop/ExceptionHandlingAttribute.cs`
- **Usage**: Applied at controller class level
- **Logs**: All exceptions for diagnostics
- **Result**: Returns 500 InternalServerError with error details

```csharp
[ApiController]
[ExceptionHandling]  // AOP: centralized error handling
public class ProductsController : ControllerBase { }
```

#### RequestLoggingAttribute
- **Purpose**: Logs incoming requests and outgoing responses
- **Location**: `shared/aop/RequestLoggingAttribute.cs`
- **Usage**: Applied at controller class level
- **Format**: `→ METHOD PATH | User` for requests, `← STATUS PATH` for responses

```csharp
[ApiController]
[RequestLogging]  // AOP: request/response logging
public class ProductsController : ControllerBase { }
```

### 2. FluentValidation Validators

All validators are defined in `CatalogService/src/Validators/CatalogValidators.cs`:

#### CreateProductRequestValidator
Validates product creation requests with rules for:
- SKU: Required, 3-50 chars, uppercase + numbers + hyphens only
- Name: Required, 2-200 chars
- Description: Required, max 1000 chars
- Price: > 0, max 2 decimals
- StockQuantity: >= 0
- Tags: At least 1, non-empty values
- LocalizedNames: Required with at least English (en)

```csharp
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[A-Z0-9\-]+$");
            
        // ... more rules
    }
}
```

#### UpdateProductRequestValidator
Allows partial updates with optional fields:
- Validates only provided properties
- Uses `.When()` to conditionally apply rules

```csharp
RuleFor(x => x.Price)
    .GreaterThan(0)
        .When(x => x.Price.HasValue)  // Only validate if provided
        .WithMessage("Price must be greater than 0");
```

#### CreateCategoryRequestValidator & CreateBrandRequestValidator
Similar structure to product validator, optimized for category/brand fields.

### 3. Request DTOs

All request models defined in `CatalogService/src/Models/RequestDtos.cs`:

```csharp
// Create request
public record CreateProductRequest(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    decimal? B2bPrice,
    int StockQuantity,
    string[] Tags,
    Dictionary<string, LocalizedContent> LocalizedNames
);

// Update request (all optional)
public record UpdateProductRequest(
    string? Sku,
    string? Name,
    string? Description,
    decimal? Price,
    decimal? B2bPrice,
    int? StockQuantity,
    string[]? Tags,
    Dictionary<string, LocalizedContent>? LocalizedNames
);
```

## Usage in Controllers

### Clean Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateModel]        // AOP filters
[ExceptionHandling]
[RequestLogging]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    /// GET api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        // Simple, clean code - no validation, logging, or error handling boilerplate
        var product = await _service.GetProductAsync(id);
        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    /// POST api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductRequest request)  // Automatically validated by FluentValidation + AOP
    {
        // At this point, request is guaranteed valid
        var product = await _service.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// PUT api/products/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request)  // Partial update validation
    {
        // Allows null/optional fields
        var product = await _service.UpdateProductAsync(id, request);
        if (product == null)
            return NotFound();
        
        return Ok(product);
    }
}
```

## Response Formats

### Success Response (200 OK)
```json
{
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "sku": "PROD-001",
    "name": "Sample Product",
    "price": 99.99,
    "stockQuantity": 100
}
```

### Validation Error (400 Bad Request)
```json
{
    "status": "ValidationFailed",
    "message": "One or more validation errors occurred",
    "errors": {
        "Sku": [
            "SKU is required"
        ],
        "Price": [
            "Price must be greater than 0"
        ]
    },
    "timestamp": "2025-12-26T10:30:00Z"
}
```

### Server Error (500 Internal Server Error)
```json
{
    "status": "Error",
    "message": "Unexpected error occurred",
    "errorType": "NullReferenceException",
    "timestamp": "2025-12-26T10:30:00Z"
}
```

## Setup in Program.cs

```csharp
// Add services
builder.Services
    .AddCatalogServices(builder.Configuration)  // Registers AOP + Validators
    .AddControllers();

// Minimal error handling setup
var app = builder.Build();

// Middleware (request logging at pipeline level)
app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Testing Validators

Unit test example:

```csharp
[Fact]
public async Task CreateProductValidator_WithValidProduct_Succeeds()
{
    // Arrange
    var validator = new CreateProductRequestValidator();
    var request = new CreateProductRequest(
        Sku: "PROD-001",
        Name: "Test Product",
        Description: "A test product",
        Price: 99.99m,
        B2bPrice: null,
        StockQuantity: 100,
        Tags: new[] { "test" },
        LocalizedNames: new Dictionary<string, LocalizedContent>
        {
            { "en", new LocalizedContent("Test Product", "A test product") }
        }
    );

    // Act
    var result = await validator.ValidateAsync(request);

    // Assert
    Assert.True(result.IsValid);
}

[Fact]
public async Task CreateProductValidator_WithInvalidSku_Fails()
{
    // Arrange
    var validator = new CreateProductRequestValidator();
    var request = new CreateProductRequest(
        Sku: "prod-001",  // lowercase - invalid
        // ... other fields
    );

    // Act
    var result = await validator.ValidateAsync(request);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains("SKU must contain only uppercase letters", 
        result.Errors.Select(e => e.ErrorMessage));
}
```

## File Structure

```
backend/
├── shared/
│   ├── aop/
│   │   ├── ValidationFilterAttribute.cs
│   │   ├── ExceptionHandlingAttribute.cs
│   │   └── RequestLoggingAttribute.cs
│   └── extensions/
│       └── AopExtensions.cs
└── services/
    └── CatalogService/
        ├── src/
        │   ├── Controllers/
        │   │   ├── ProductsController.cs
        │   │   ├── CategoriesController.cs
        │   │   └── BrandsController.cs
        │   ├── Models/
        │   │   └── RequestDtos.cs
        │   ├── Validators/
        │   │   └── CatalogValidators.cs
        │   └── Extensions/
        │       └── CatalogServiceExtensions.cs
        └── Program.cs
```

## Best Practices

✅ **Always use validators** - FluentValidation provides type-safe validation
✅ **Keep controllers thin** - Delegate to services
✅ **Reuse DTOs** - One validator per request type
✅ **Use async validation** - For database lookups (e.g., SKU uniqueness)
✅ **Document rules** - Add XML comments to validators
✅ **Test validators** - Unit test all validation rules
✅ **Standardize responses** - Use consistent error formats

## Next Steps

1. **Apply to other controllers** - Extend to Categories, Brands controllers
2. **Add async validators** - Implement SKU uniqueness check
3. **Custom validators** - Add business-specific validation
4. **Localization** - Add multi-language error messages
5. **Monitoring** - Add metrics/tracing to AOP filters

---

**Document Version**: 1.0
**Created**: 2025-12-26
**Implementation Status**: Complete for CatalogService
