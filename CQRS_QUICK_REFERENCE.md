# ⚡ CQRS Quick Reference Card

**For**: B2Connect Admin API Developers  
**Pattern**: CQRS with Wolverine Message Bus  
**Status**: ✅ Production Ready

---

## 🚀 Quick Start (TL;DR)

### Add a New GET Endpoint
```csharp
// 1️⃣ Define Query in XxxCommands.cs
public record GetProductByNameQuery(Guid TenantId, string Name) 
    : IRequest<ProductResult?>;

// 2️⃣ Create Handler in XxxHandlers.cs
public class GetProductByNameHandler 
    : IQueryHandler<GetProductByNameQuery, ProductResult?>
{
    public async Task<ProductResult?> Handle(GetProductByNameQuery query, CancellationToken ct)
    {
        var product = await _repository.GetByNameAsync(query.TenantId, query.Name, ct);
        return product == null ? null : _mapper.Map<ProductResult>(product);
    }
}

// 3️⃣ Use in Controller
[HttpGet("by-name")]
public async Task<ActionResult<ProductResult>> GetByName(string name, CancellationToken ct)
{
    var tenantId = GetTenantId();
    var query = new GetProductByNameQuery(tenantId, name);
    var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);
    
    return product == null 
        ? NotFoundResponse($"Product '{name}' not found")
        : OkResponse(product);
}
```

### Add a New POST Endpoint
```csharp
// 1️⃣ Define Command in XxxCommands.cs
public record CreateProductRequest(string Name, string Sku, decimal Price);

public record CreateProductCommand(Guid TenantId, string Name, string Sku, decimal Price)
    : IRequest<ProductResult>;

// 2️⃣ Create Validator (Optional but Recommended)
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// 3️⃣ Create Handler in XxxHandlers.cs
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        // Validate
        var validator = new CreateProductValidator();
        var validation = await validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Create entity
        var product = Product.Create(cmd.TenantId, cmd.Sku, cmd.Name, cmd.Price);
        
        // Persist
        await _repository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        // Log
        _logger.LogInformation("Product {ProductId} created by user", product.Id);
        
        // Return
        return _mapper.Map<ProductResult>(product);
    }
}

// 4️⃣ Use in Controller
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<ProductResult>> Create(
    [FromBody] CreateProductRequest request, 
    CancellationToken ct)
{
    var tenantId = GetTenantId();
    var command = new CreateProductCommand(tenantId, request.Name, request.Sku, request.Price);
    var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);
    
    return CreatedResponse(nameof(GetProduct), new { id = product.Id }, product);
}
```

---

## 📋 File Locations

### Command/Query Definitions
```
backend/BoundedContexts/Admin/API/src/Application/Commands/
├── Products/ProductCommands.cs       ← Define queries/commands
├── Categories/CategoryCommands.cs
└── Brands/BrandCommands.cs
```

### Handler Implementations
```
backend/BoundedContexts/Admin/API/src/Application/Handlers/
├── Products/ProductHandlers.cs       ← Implement handlers
├── Categories/CategoryHandlers.cs
└── Brands/BrandHandlers.cs
```

### API Controllers
```
backend/BoundedContexts/Admin/API/src/Presentation/Controllers/
├── ProductsController.cs             ← Dispatch via message bus
├── CategoriesController.cs
├── BrandsController.cs
└── UsersController.cs                ← Special case (BFF proxy)
```

---

## 🎯 Response Patterns

### Success Response
```csharp
// Inherits from ApiControllerBase which provides:
return OkResponse(data);                    // 200 OK
return OkResponse(data, "Custom message");  // 200 OK + message
return CreatedResponse(route, id, data);    // 201 Created
return NoContent();                         // 204 No Content
```

### Error Response
```csharp
return BadRequestResponse("Invalid input");           // 400 Bad Request
return UnauthorizedResponse("Not authenticated");     // 401 Unauthorized
return ForbiddenResponse("Not authorized");           // 403 Forbidden
return NotFoundResponse($"Item {id} not found");      // 404 Not Found
return ConflictResponse("Item already exists");       // 409 Conflict
return InternalServerErrorResponse("Server error");   // 500 Server Error
```

---

## 🔍 Common Patterns

### Multi-Tenancy (Required for ALL queries/commands)
```csharp
// ✅ CORRECT - Include TenantId
public record GetProductQuery(Guid TenantId, Guid ProductId) : IRequest<ProductResult?>;
public record CreateProductCommand(Guid TenantId, string Name) : IRequest<ProductResult>;

// ❌ WRONG - Missing TenantId
public record GetProductQuery(Guid ProductId) : IRequest<ProductResult?>;  // Data leak!
```

### Handler Logging (Recommended)
```csharp
public async Task<ProductResult> Handle(CreateProductCommand cmd, CancellationToken ct)
{
    _logger.LogInformation(
        "User {UserId} creating product {Name} for tenant {TenantId}",
        userId, cmd.Name, cmd.TenantId);
    
    // ... do work ...
    
    _logger.LogInformation("Product {ProductId} created successfully", product.Id);
    return result;
}
```

### Pagination Pattern
```csharp
public record GetProductsPagedQuery(
    Guid TenantId, 
    int PageNumber, 
    int PageSize) 
    : IRequest<(IEnumerable<ProductResult>, int)>;  // Returns items + total count

public async Task<(IEnumerable<ProductResult>, int)> Handle(
    GetProductsPagedQuery query, 
    CancellationToken ct)
{
    var (items, total) = await _repository.GetPagedAsync(
        query.TenantId, 
        query.PageNumber, 
        query.PageSize, 
        ct);
    
    return (items.Select(p => _mapper.Map<ProductResult>(p)), total);
}

// In Controller:
[HttpGet("paged")]
public async Task<ActionResult> GetPaged(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
{
    var tenantId = GetTenantId();
    var query = new GetProductsPagedQuery(tenantId, pageNumber, pageSize);
    var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<ProductResult>, int)>(query, ct);
    
    return OkResponse(new { items, total, pageNumber, pageSize });
}
```

### Error Handling
```csharp
public async Task<ProductResult> Handle(UpdateProductCommand cmd, CancellationToken ct)
{
    var product = await _repository.GetByIdAsync(cmd.TenantId, cmd.ProductId, ct);
    
    if (product == null)
        throw new NotFoundException($"Product {cmd.ProductId} not found");
    
    if (product.IsDeleted)
        throw new InvalidOperationException("Cannot update deleted product");
    
    product.Update(cmd.Name, cmd.Price);
    
    try
    {
        await _unitOfWork.SaveChangesAsync(ct);
    }
    catch (DbUpdateConcurrencyException)
    {
        throw new ConflictException("Product was modified by another user");
    }
    
    return _mapper.Map<ProductResult>(product);
}
```

---

## 🧪 Testing Patterns

### Test a Handler
```csharp
[Fact]
public async Task CreateProductHandler_WithValidInput_CreatesProduct()
{
    // Arrange
    var mockRepo = new Mock<IProductRepository>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockMapper = new Mock<IMapper>();
    
    var handler = new CreateProductHandler(mockRepo.Object, mockUnitOfWork.Object, mockMapper.Object);
    var command = new CreateProductCommand(Guid.NewGuid(), "SKU001", "Product", 99.99m);
    
    var expectedResult = new ProductResult(Guid.NewGuid(), command.TenantId, "SKU001", "Product", 99.99m);
    mockMapper.Setup(x => x.Map<ProductResult>(It.IsAny<Product>()))
        .Returns(expectedResult);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("SKU001", result.Sku);
    mockRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

### Test a Controller Method
```csharp
[Fact]
public async Task GetProduct_WithValidId_Returns200Ok()
{
    // Arrange
    var mockMessageBus = new Mock<IMessageBus>();
    var mockLogger = new Mock<ILogger<ProductsController>>();
    var controller = new ProductsController(mockMessageBus.Object, mockLogger.Object);
    
    var expectedProduct = new ProductResult(Guid.NewGuid(), Guid.NewGuid(), "SKU001", "Product", 99.99m);
    mockMessageBus.Setup(x => x.InvokeAsync<ProductResult?>(
            It.IsAny<GetProductQuery>(), 
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedProduct);
    
    // Act
    var result = await controller.GetProduct(Guid.NewGuid(), CancellationToken.None);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.NotNull(okResult.Value);
}
```

---

## 🚨 Common Mistakes to Avoid

### ❌ Forgetting TenantId
```csharp
// WRONG - Data leak!
var products = await _repository.GetAllAsync();

// CORRECT - Only this tenant's data
var products = await _repository.GetAllAsync(tenantId);
```

### ❌ Not Using Message Bus
```csharp
// WRONG - Business logic in controller
var product = await _service.GetProduct(id);

// CORRECT - Dispatch via message bus
var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);
```

### ❌ Missing CancellationToken
```csharp
// WRONG - No way to cancel
var product = await _repository.GetByIdAsync(tenantId, id);

// CORRECT - Support graceful cancellation
var product = await _repository.GetByIdAsync(tenantId, id, ct);
```

### ❌ Blocking on Async
```csharp
// WRONG - Deadlock risk
var product = _messageBus.InvokeAsync<ProductResult?>(query, ct).Result;

// CORRECT - Async all the way down
var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);
```

### ❌ Not Logging
```csharp
// WRONG - No visibility
var product = Product.Create(...);
await _repository.AddAsync(product);

// CORRECT - Log important actions
_logger.LogInformation("Product {ProductId} created by user {UserId}", product.Id, userId);
var product = Product.Create(...);
await _repository.AddAsync(product);
```

---

## 📚 Documentation References

| Document | Purpose |
|----------|---------|
| [CQRS_REFACTORING_COMPLETE.md](./CQRS_REFACTORING_COMPLETE.md) | Full architecture reference |
| [DEPLOYMENT_READY.md](./DEPLOYMENT_READY.md) | Deployment procedures |
| [SESSION_SUMMARY.md](./SESSION_SUMMARY.md) | What was done this session |
| [copilot-instructions.md](./.github/copilot-instructions.md) | General architecture guidelines |

---

## ✅ Checklist Before Submitting Code

- [ ] Handler includes logging
- [ ] Handler validates input (or has FluentValidator)
- [ ] Handler handles all error cases
- [ ] Controller method uses `_messageBus.InvokeAsync<T>()`
- [ ] Controller method extracts tenantId
- [ ] Controller method includes CancellationToken
- [ ] Command/Query includes TenantId field
- [ ] Response DTO properly mapped
- [ ] Tests written (100+ coverage)
- [ ] Build passes: `dotnet build B2Connect.slnx`
- [ ] No hardcoded secrets (use configuration)
- [ ] No `async void` methods (except event handlers)
- [ ] No `.Result` or `.Wait()` (use `await`)

---

**Last Updated**: 27. Dezember 2025  
**Pattern Status**: ✅ Production Ready  
**Applicable To**: ProductsController, CategoriesController, BrandsController
