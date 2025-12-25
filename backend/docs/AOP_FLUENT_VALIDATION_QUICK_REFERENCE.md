# AOP & FluentValidation Quick Reference

## Apply AOP to a Controller

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateModel]        // ← Auto-validates requests
[ExceptionHandling]    // ← Catches exceptions
[RequestLogging]       // ← Logs requests/responses
public class YourController : ControllerBase
{
    // Clean controller code - no boilerplate!
}
```

## Create a Validator

```csharp
public class YourRequestValidator : AbstractValidator<YourRequest>
{
    public YourRequestValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty().WithMessage("Required")
            .Length(2, 50).WithMessage("2-50 chars")
            .Matches(@"^[A-Z]+$").WithMessage("Uppercase only");
    }
}
```

## Register in Program.cs

```csharp
// Option 1: Auto-register from assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Option 2: Use extension method
builder.Services.AddCatalogServices(builder.Configuration);
```

## Create Request DTO

```csharp
public record CreateYourRequest(
    string Property1,
    int Property2,
    Dictionary<string, LocalizedContent> LocalizedNames
);
```

## Use in Controller Action

```csharp
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<YourDto>> Create(
    [FromBody] CreateYourRequest request)  // Auto-validated
{
    // At this point, request is 100% valid
    var result = await _service.CreateAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

## Validation Rules Reference

### String Rules
```csharp
RuleFor(x => x.Name)
    .NotEmpty()
    .Length(2, 100)
    .Must(x => !x.StartsWith(" "));
```

### Numeric Rules
```csharp
RuleFor(x => x.Price)
    .GreaterThan(0)
    .LessThanOrEqualTo(999999.99m);
```

### Collection Rules
```csharp
RuleFor(x => x.Tags)
    .NotNull()
    .Must(x => x.Length > 0)
    .ForEach(tag => tag.NotEmpty());
```

### Conditional Rules
```csharp
RuleFor(x => x.B2bPrice)
    .GreaterThan(0)
    .When(x => x.B2bPrice.HasValue);  // Only if provided
```

### Async Rules (Database)
```csharp
RuleFor(x => x.Sku)
    .MustAsync(async (sku, ct) => !await _repo.ExistsAsync(sku, ct))
    .WithMessage("SKU '{PropertyValue}' already exists");
```

## Error Response Format

### Validation Error (400)
```json
{
    "status": "ValidationFailed",
    "message": "One or more validation errors occurred",
    "errors": {
        "PropertyName": ["Error message 1", "Error message 2"]
    },
    "timestamp": "2025-12-26T10:30:00Z"
}
```

### Server Error (500)
```json
{
    "status": "Error",
    "message": "Exception message",
    "errorType": "ExceptionTypeName",
    "timestamp": "2025-12-26T10:30:00Z"
}
```

## File Locations

```
shared/aop/
  ├── ValidationFilterAttribute.cs      # Model validation
  ├── ExceptionHandlingAttribute.cs     # Error handling
  └── RequestLoggingAttribute.cs        # Request logging

CatalogService/src/
  ├── Validators/CatalogValidators.cs   # All validators
  ├── Models/RequestDtos.cs             # Request DTOs
  └── Extensions/
      └── CatalogServiceExtensions.cs   # Setup
```

## Common Validation Patterns

### Required Field
```csharp
RuleFor(x => x.Field).NotEmpty().WithMessage("Field is required");
```

### Email
```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .EmailAddress().WithMessage("Invalid email format");
```

### URL
```csharp
RuleFor(x => x.Url)
    .NotEmpty()
    .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
    .WithMessage("Invalid URL");
```

### At Least One Item
```csharp
RuleFor(x => x.Items)
    .NotNull()
    .Must(x => x.Count > 0).WithMessage("At least one item required");
```

### Custom Logic
```csharp
RuleFor(x => x.EndDate)
    .Must((model, endDate) => endDate > model.StartDate)
    .WithMessage("End date must be after start date");
```

## Testing Validators

```csharp
[Fact]
public async Task Validator_WithValidData_Succeeds()
{
    var validator = new YourRequestValidator();
    var request = new YourRequest("valid-data");
    
    var result = await validator.ValidateAsync(request);
    
    Assert.True(result.IsValid);
}

[Fact]
public async Task Validator_WithInvalidData_Fails()
{
    var validator = new YourRequestValidator();
    var request = new YourRequest("");  // Invalid
    
    var result = await validator.ValidateAsync(request);
    
    Assert.False(result.IsValid);
    Assert.Single(result.Errors);
}
```

## Troubleshooting

**Q: Validator not running?**
A: Ensure `[ValidateModel]` attribute is on controller

**Q: Custom error message not showing?**
A: Use `.WithMessage()` in validator rule

**Q: Want to skip validation for one action?**
A: Apply `[ValidateModel(false)]` or create separate controller

**Q: How to run validator manually?**
A: 
```csharp
var validator = new YourValidator();
var result = await validator.ValidateAsync(request);
if (!result.IsValid)
{
    var errors = result.Errors;
}
```

---

**Last Updated**: 2025-12-26
