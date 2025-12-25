# AOP Filters & Fluent Validation Implementation

Complete guide to ASP.NET Core AOP filters and FluentValidation framework for clean architecture.

## Overview

Three reusable AOP filters handle cross-cutting concerns, keeping controllers focused on business logic:

1. **ValidationFilterAttribute** - Validates requests before action execution
2. **ExceptionHandlingAttribute** - Catches exceptions and returns proper HTTP responses
3. **RequestLoggingAttribute** - Logs all HTTP requests and responses

Combined with **FluentValidation**, all inputs are validated consistently across services.

## Architecture

```
Request Flow:
  1. Request arrives
  2. RequestLoggingAttribute.OnActionExecuting() - Log request
  3. ValidationFilterAttribute.OnActionExecuting() - Validate input
  4. Controller.Action() - Execute business logic
  5. ExceptionHandlingAttribute.OnActionExecuted() - Handle errors
  6. RequestLoggingAttribute.OnActionExecuted() - Log response
  7. Response sent
```

## 1. Validation Filter

**Location:** `backend/shared/aop/ValidationFilterAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(m => m.Value.Errors.Count > 0)
                .ToDictionary(
                    m => m.Key,
                    m => m.Value.Errors.Select(e => e.ErrorMessage).ToArray());
            
            context.Result = new BadRequestObjectResult(
                new { errors = errors, message = "Validation failed" });
            return;
        }
        
        await next();
    }
}
```

**Usage on Controllers:**

```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    [ValidateModel]  // ← Applies validation filter
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductRequest request)
    {
        // request is guaranteed valid
        var product = await _service.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
```

## 2. FluentValidation Setup

**Location:** `backend/services/CatalogService/src/Validators/CatalogValidators.cs`

### Register Validators

```csharp
// In Program.cs or extension method
services
    .AddValidatorsFromAssemblyContaining<CreateProductValidator>()
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
```

### Create a Validator

```csharp
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        // SKU validation
        RuleFor(p => p.Sku)
            .NotEmpty()
            .Length(3, 20)
            .Matches(@"^[A-Z0-9\-]+$", RegexOptions.IgnoreCase)
            .WithMessage("SKU must be uppercase alphanumeric");
        
        // Name validation (multi-language)
        RuleFor(p => p.Name)
            .NotNull()
            .Must(n => n.ContainsKey("en"))
            .WithMessage("English name is required");
        
        // Price validation
        RuleFor(p => p.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Price must be between 0 and 999,999.99");
        
        // Stock validation
        RuleFor(p => p.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");
        
        // Tags validation
        RuleFor(p => p.Tags)
            .Must(t => t.Length <= 20)
            .WithMessage("Maximum 20 tags allowed");
        
        // Custom async validation (e.g., check SKU uniqueness in DB)
        RuleFor(p => p.Sku)
            .MustAsync(async (sku, ct) => !await _repository.SkuExistsAsync(sku, ct))
            .WithMessage("SKU must be unique");
    }
}
```

## 3. Exception Handling Filter

**Location:** `backend/shared/aop/ExceptionHandlingAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ExceptionHandlingAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlingAttribute> _logger;
    
    public ExceptionHandlingAttribute(ILogger<ExceptionHandlingAttribute> logger)
    {
        _logger = logger;
    }
    
    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred");
        
        var response = context.Exception switch
        {
            ValidationException ex => 
                BadRequest("Validation failed", ex.Message),
            
            EntityNotFoundException ex => 
                NotFound("Resource not found", ex.Message),
            
            UnauthorizedAccessException ex => 
                Unauthorized("Access denied", ex.Message),
            
            _ => 
                InternalServerError("An unexpected error occurred", context.Exception.Message)
        };
        
        context.Result = new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}
```

## 4. Request Logging Filter

**Location:** `backend/shared/aop/RequestLoggingAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequestLoggingAttribute : ActionFilterAttribute
{
    private readonly ILogger<RequestLoggingAttribute> _logger;
    
    public RequestLoggingAttribute(ILogger<RequestLoggingAttribute> logger)
    {
        _logger = logger;
    }
    
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        
        _logger.LogInformation(
            "HTTP {Method} {Path} started",
            request.Method,
            request.Path);
        
        var executedContext = await next();
        
        var statusCode = executedContext.HttpContext.Response.StatusCode;
        
        _logger.LogInformation(
            "HTTP {Method} {Path} completed with status {StatusCode}",
            request.Method,
            request.Path,
            statusCode);
    }
}
```

## 5. All Validator Rules

### Product Validators

| Field | Rule | Message |
|-------|------|---------|
| Sku | NotEmpty, Length(3,20), Uppercase | "SKU must be uppercase, 3-20 chars" |
| Name | NotNull, ContainsKey("en") | "English name required" |
| Price | > 0, <= 999999.99 | "Valid price required" |
| Stock | >= 0 | "Stock cannot be negative" |
| Tags | <= 20 | "Max 20 tags" |
| ImageUrls | Valid URLs | "Valid image URLs required" |

### Category Validators

| Field | Rule |
|-------|------|
| Name | NotEmpty, Length(2,100) |
| ParentId | MustAsync(NoCircular) |
| DisplayOrder | >= 0 |

### Brand Validators

| Field | Rule |
|-------|------|
| Name | NotEmpty, Length(2,100), Unique |
| LogoUrl | ValidUrl |

## Usage Patterns

### Simple Validation

```csharp
[HttpPost]
[ValidateModel]
public async Task CreateProduct([FromBody] CreateProductRequest request)
{
    // Request is already validated by FilterAttribute
    // Validators run automatically via ASP.NET Core
    await _service.CreateAsync(request);
}
```

### Complex Validation

```csharp
public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    private readonly IProductRepository _repo;
    
    public UpdateProductValidator(IProductRepository repo)
    {
        _repo = repo;
        
        RuleFor(p => p.Id)
            .MustAsync(async (id, ct) => await _repo.ExistsAsync(id, ct))
            .WithMessage("Product not found");
    }
}
```

### Conditional Validation

```csharp
RuleFor(p => p.B2bPrice)
    .GreaterThan(0)
    .When(p => p.B2bPrice.HasValue)
    .WithMessage("B2B price must be > 0 if specified");
```

## Extension Method Pattern

**Location:** `backend/shared/extensions/AopExtensions.cs`

```csharp
public static IServiceCollection AddAopFilters(
    this IServiceCollection services)
{
    services.AddScoped<ValidateModelAttribute>();
    services.AddScoped<ExceptionHandlingAttribute>();
    services.AddScoped<RequestLoggingAttribute>();
    
    return services;
}

// In Program.cs:
services.AddAopFilters();
```

## Testing Validators

```csharp
[Fact]
public void CreateProductValidator_WithValidRequest_Succeeds()
{
    var validator = new CreateProductValidator();
    var request = new CreateProductRequest
    {
        Sku = "PROD-001",
        Name = new Dictionary<string, string> { { "en", "Test" } },
        Price = 99.99m,
        StockQuantity = 100
    };
    
    var result = validator.Validate(request);
    Assert.True(result.IsValid);
}

[Fact]
public void CreateProductValidator_WithEmptySku_Fails()
{
    var validator = new CreateProductValidator();
    var request = new CreateProductRequest { Sku = "" };
    
    var result = validator.Validate(request);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
}
```

## Best Practices

**DO:**
- Use `[ValidateModel]` on all POST/PUT endpoints
- Inherit from base validators for consistency
- Use async validators for database checks
- Test both valid and invalid scenarios
- Log validation failures with context

**DON'T:**
- Skip validation to improve performance
- Duplicate validation logic between requests/events
- Use exceptions for validation flow control
- Suppress validation errors

## References

- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- `.copilot-specs.md` Section 19 (Validator patterns)
- `EVENT_VALIDATION_IMPLEMENTATION.md` (Event validators)
- `VSCODE_ASPIRE_CONFIG.md` (Running validator tests)
