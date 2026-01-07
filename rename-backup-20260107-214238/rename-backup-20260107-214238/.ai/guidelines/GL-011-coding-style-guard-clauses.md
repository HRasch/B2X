# Guard Clauses - Coding Style Guidelines

**DocID**: `GL-010`  
**Status**: Active  
**Owner**: @TechLead  
**Last Updated**: 3. Januar 2026

## Overview

Guard clauses are the preferred pattern for input validation in B2X. They provide early returns for invalid inputs, reducing code nesting and improving readability. This guideline establishes consistent patterns across the codebase.

## Core Principles

### 1. Prefer Guard Clauses Over Nested If-Else
```csharp
// ❌ AVOID - Nested validation
public async Task<Result<Product>> GetProductAsync(string productId)
{
    if (productId != null)
    {
        if (!string.IsNullOrWhiteSpace(productId))
        {
            // Business logic here
            return await _repository.GetByIdAsync(productId);
        }
        else
        {
            return Result.Failure<Product>("Product ID cannot be empty");
        }
    }
    else
    {
        return Result.Failure<Product>("Product ID is required");
    }
}

// ✅ PREFER - Guard clauses
public async Task<Result<Product>> GetProductAsync(string productId)
{
    if (string.IsNullOrWhiteSpace(productId))
        return Result.Failure<Product>("Product ID is required");

    // Business logic here
    return await _repository.GetByIdAsync(productId);
}
```

### 2. Use Modern Exception Patterns (CA2208 Compliance)

#### For Method Parameters
```csharp
// ✅ Modern pattern (.NET 6+)
ArgumentNullException.ThrowIfNull(productId);

// ✅ Traditional pattern (all .NET versions)
if (productId == null) throw new ArgumentNullException(nameof(productId));

// ❌ AVOID - Wrong exception type
NullReferenceException.ThrowIfNull(productId); // Method doesn't exist!
```

#### For Business Logic Validation
```csharp
// ✅ ArgumentException for invalid values
if (string.IsNullOrWhiteSpace(productId))
    throw new ArgumentException("Product ID cannot be empty", nameof(productId));

// ✅ ArgumentOutOfRangeException for ranges
if (quantity <= 0)
    throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");

// ✅ InvalidOperationException for state violations
if (_disposed)
    throw new InvalidOperationException("Object is disposed");
```

### 3. Consistent Exception Messages

#### Parameter Validation
```csharp
// ✅ Clear, descriptive messages
ArgumentNullException.ThrowIfNull(tenantId);
if (string.IsNullOrWhiteSpace(name))
    throw new ArgumentException("Name cannot be null or empty", nameof(name));

// ❌ Vague messages
throw new ArgumentException("Invalid parameter");
```

#### Business Rule Validation
```csharp
// ✅ Specific business context
if (order.Total < 0)
    return Result.Failure("Order total cannot be negative");

// ✅ User-friendly messages
if (!await _userRepository.ExistsAsync(userId))
    return Result.Failure("User not found");
```

## Implementation Patterns

### 1. Controller Actions (API Endpoints)

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(string id)
{
    // Guard clauses for API parameters
    if (string.IsNullOrWhiteSpace(id))
        return BadRequest("Product ID is required");

    var product = await _productService.GetByIdAsync(id);
    if (product == null)
        return NotFound();

    return Ok(product);
}
```

### 2. Service Methods

```csharp
public async Task<Result<Product>> CreateProductAsync(CreateProductCommand command)
{
    // Input validation guards
    ArgumentNullException.ThrowIfNull(command);
    if (string.IsNullOrWhiteSpace(command.Name))
        return Result.Failure<Product>("Product name is required");
    if (command.Price < 0)
        return Result.Failure<Product>("Product price cannot be negative");

    // Business logic
    var product = new Product(command.Name, command.Price);
    await _repository.AddAsync(product);

    return Result.Success(product);
}
```

### 3. Repository Methods

```csharp
public async Task<Product?> GetByIdAsync(string id)
{
    // Repository guards (minimal validation)
    if (string.IsNullOrWhiteSpace(id))
        return null;

    // Database query with tenant isolation
    return await _dbContext.Products
        .Where(p => p.Id == id && p.TenantId == _tenantContext.TenantId)
        .FirstOrDefaultAsync();
}
```

### 4. Handler Methods (Wolverine CQRS)

```csharp
public async Task<Result> Handle(CreateProductCommand command)
{
    // Command validation guards
    ArgumentNullException.ThrowIfNull(command);
    if (string.IsNullOrWhiteSpace(command.Name))
        throw new ArgumentException("Product name is required", nameof(command.Name));

    // Domain logic
    var product = Product.Create(command.Name, command.Price);
    await _repository.AddAsync(product);

    return Result.Success();
}
```

### 5. Middleware and Filters

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Request validation guards
    var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(tenantId))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("X-Tenant-ID header is required");
        return;
    }

    // Continue processing
    await _next(context);
}
```

## Advanced Patterns

### 1. Guard Clause with Logging

```csharp
public async Task<Result<Order>> ProcessOrderAsync(Order order)
{
    if (order == null)
    {
        _logger.LogWarning("Received null order for processing");
        return Result.Failure<Order>("Order cannot be null");
    }

    if (order.Items.Count == 0)
    {
        _logger.LogWarning("Received empty order {OrderId}", order.Id);
        return Result.Failure<Order>("Order must contain at least one item");
    }

    // Process order...
}
```

### 2. Guard Clause with Performance Considerations

```csharp
public async Task<IReadOnlyList<Product>> SearchProductsAsync(string query, int page = 1, int pageSize = 20)
{
    // Input validation with performance guards
    if (string.IsNullOrWhiteSpace(query))
        return Array.Empty<Product>();

    if (page < 1)
        throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0");

    if (pageSize < 1 || pageSize > 100)
        throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100");

    // Database query...
}
```

### 3. Guard Clause in Property Setters

```csharp
private string _name;
public string Name
{
    get => _name;
    set => _name = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null");
}
```

### 4. Guard Clause with Custom Validation

```csharp
public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
{
    // Custom validation guards
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return Result.Failure<User>(validationResult.Errors.First().ErrorMessage);

    // Business logic...
}
```

## Code Examples from B2X

### Existing Good Examples

**ProductSearchController.cs** (Line 55-65):
```csharp
if (doc == null)
    return NotFound();
```

**EnventaActorPool.cs** (Guard for disposed state):
```csharp
if (_disposed)
    return;
```

### Anti-Patterns to Avoid

```csharp
// ❌ Deep nesting
if (condition1) {
    if (condition2) {
        if (condition3) {
            // Deep logic
        }
    }
}

// ❌ Multiple responsibilities in one guard
if (product == null || string.IsNullOrWhiteSpace(product.Name) || product.Price < 0)
    throw new ArgumentException("Invalid product");

// ✅ Separate guards for clarity
if (product == null)
    throw new ArgumentNullException(nameof(product));
if (string.IsNullOrWhiteSpace(product.Name))
    throw new ArgumentException("Product name is required", nameof(product.Name));
if (product.Price < 0)
    throw new ArgumentOutOfRangeException(nameof(product.Price), "Price cannot be negative");
```

## Integration with CA2208 Best Practices

### Correct Parameter Names in Exceptions

```csharp
// ✅ Correct - Use parameter name
public void Process(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("Input cannot be empty", nameof(input));
}

// ❌ Incorrect - Using property name instead of parameter
public void Process(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("Input cannot be empty", "input"); // Avoid magic strings
}
```

### Exception Type Selection

| Scenario | Exception Type | Example |
|----------|----------------|---------|
| Null parameter | `ArgumentNullException` | `ThrowIfNull(parameter)` |
| Invalid parameter value | `ArgumentException` | `new ArgumentException("message", nameof(param))` |
| Out of range value | `ArgumentOutOfRangeException` | `new ArgumentOutOfRangeException(nameof(param), value, "message")` |
| Invalid operation state | `InvalidOperationException` | `new InvalidOperationException("message")` |
| Business rule violation | Custom `DomainException` | `new ProductNotFoundException(productId)` |

## Testing Guard Clauses

```csharp
[Fact]
public async Task GetProductAsync_WithNullId_ReturnsFailure()
{
    // Arrange
    var service = new ProductService();

    // Act
    var result = await service.GetProductAsync(null);

    // Assert
    result.IsFailure.ShouldBeTrue();
    result.Error.ShouldBe("Product ID is required");
}

[Fact]
public async Task GetProductAsync_WithEmptyId_ThrowsArgumentException()
{
    // Arrange
    var service = new ProductService();

    // Act & Assert
    var exception = await Should.ThrowAsync<ArgumentException>(
        () => service.GetProductAsync("")
    );
    exception.ParamName.ShouldBe("productId");
}
```

## Migration Strategy

### For Existing Code

1. **Identify nested validation blocks**
2. **Extract to guard clauses at method start**
3. **Update exception messages to CA2208 compliance**
4. **Test thoroughly** - guard clause changes can affect behavior

### For New Code

1. **Write guard clauses first** in every method
2. **Use modern exception patterns**
3. **Follow established message formats**
4. **Review in code review** for consistency

## Related Guidelines

- [KB-010] OWASP Top Ten - Input validation security
- [GL-001] Communication Overview - Error message standards
- [ADR-001] Wolverine over MediatR - CQRS patterns
- [CA2208 Research](./lessons.md#ca2208-code-analysis) - Exception handling best practices

## Maintenance

- **Review**: Monthly for consistency
- **Update**: When new patterns emerge
- **Enforce**: Code review requirement
- **Document**: Add examples to lessons.md

---

**Adopted**: 3. Januar 2026  
**Next Review**: 1. Februar 2026</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/guidelines/GL-010-coding-style-guard-clauses.md