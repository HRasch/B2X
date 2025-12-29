# 🧪 Testing Framework & Guidelines (P0.5)

## Overview

B2Connect uses a comprehensive testing framework with:
- **xUnit** - Test framework
- **Moq** - Mocking library
- **Shouldly** - Assertion library
- **Coverlet** - Code coverage
- **InMemory Database** - Quick testing without database

## Test Pyramid

```
        /\
       /  \  E2E Tests (10%)
      /____\
     /      \
    /        \ Integration Tests (20%)
   /          \
  /____________\
 /              \
/________________\  Unit Tests (70%)
```

## Testing Standards

### 1. Unit Tests (Core Business Logic)

**Location**: `{Project}/tests/UnitTests/`

Test individual classes in isolation using mocks.

```csharp
[Collection(nameof(UnitTestCollection))]
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var productId = "prod-001";
        var expectedProduct = new Product { Id = productId, Name = "Test Product" };
        _mockRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _service.GetProductAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = "invalid-id";
        _mockRepository
            .Setup(r => r.GetByIdAsync(invalidId))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetProductAsync(invalidId));
        exception.Message.Should().Contain(invalidId);
    }
}
```

### 2. Integration Tests

**Location**: `{Project}/tests/IntegrationTests/`

Test multiple components together with a real (in-memory) database.

```csharp
[Collection(nameof(IntegrationTestCollection))]
public class ProductRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly DbContext _context;
    private readonly IProductRepository _repository;

    public ProductRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    public async Task InitializeAsync()
    {
        // Seed data
        var testProduct = new Product { Id = "1", Name = "Test" };
        _context.Products.Add(testProduct);
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProduct()
    {
        // Act
        var result = await _repository.GetByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }
}
```

### 3. Controller Tests

Test API endpoints with mocked services.

```csharp
[Collection(nameof(UnitTestCollection))]
public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    [Fact]
    public async Task GetProduct_WithValidId_Returns200Ok()
    {
        // Arrange
        var productId = "prod-001";
        var productDto = new ProductDto { Id = productId, Name = "Test" };
        _mockService
            .Setup(s => s.GetProductAsync(productId))
            .ReturnsAsync(productDto);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedDto = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedDto.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_Returns404NotFound()
    {
        // Arrange
        var invalidId = "invalid";
        _mockService
            .Setup(s => s.GetProductAsync(invalidId))
            .ThrowsAsync(new NotFoundException("Product not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetProduct(invalidId));
    }
}
```

## Test Naming Convention

Use the **Arrange-Act-Assert (AAA)** pattern with descriptive names:

```
MethodUnderTest_Scenario_ExpectedResult

✅ Good:
GetProduct_WithValidId_ReturnsProduct
GetProduct_WithInvalidId_ThrowsNotFoundException
CreateProduct_WithDuplicateSku_ThrowsValidationException

❌ Bad:
TestGetProduct
TestCreate
Should_Work
```

## Running Tests

### All Tests
```bash
dotnet test
```

### Specific Project
```bash
dotnet test backend/BoundedContexts/Shared/Identity/tests/B2Connect.Identity.Tests.csproj
```

### With Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutput=coverage
```

### Specific Test
```bash
dotnet test --filter "FullyQualifiedName~ProductServiceTests.GetProduct_WithValidId"
```

## Code Coverage Goals

| Area | Target |
|------|--------|
| Core Services | 80%+ |
| Repositories | 70%+ |
| Validators | 90%+ |
| Controllers | 70%+ |
| Utilities | 85%+ |
| Overall | 75%+ |

## Best Practices

### ✅ DO

- Write tests before or alongside code (TDD)
- Test one behavior per test
- Use descriptive test names
- Keep tests simple and focused
- Mock external dependencies
- Use in-memory database for integration tests
- Verify both happy path and error cases
- Test edge cases (null, empty, max values)

### ❌ DON'T

- Write tests that depend on other tests
- Use hardcoded delays (Thread.Sleep)
- Test implementation details, test behavior
- Create shared state between tests
- Mock the class under test
- Write overly complex test setup
- Ignore test failures

## Example Test Project Structure

```
backend/BoundedContexts/Store/Catalog/tests/
├── B2Connect.Catalog.Tests.csproj
├── Collections/
│   ├── IntegrationTestCollection.cs
│   └── UnitTestCollection.cs
├── Fixtures/
│   ├── DatabaseFixture.cs
│   └── ProductTestDataBuilder.cs
├── UnitTests/
│   ├── Services/
│   │   └── ProductServiceTests.cs
│   ├── Validators/
│   │   └── CreateProductValidatorTests.cs
│   └── Controllers/
│       └── ProductsControllerTests.cs
├── IntegrationTests/
│   ├── Repositories/
│   │   └── ProductRepositoryTests.cs
│   └── Endpoints/
│       └── GetProductEndpointTests.cs
└── Builders/
    └── ProductTestDataBuilder.cs
```

## Test Fixtures & Builders

### Test Collection
```csharp
[CollectionDefinition(nameof(UnitTestCollection))]
public class UnitTestCollection : ICollectionFixture<ConfigurationFixture>
{
    // Ensures consistent configuration across unit tests
}
```

### Test Data Builder
```csharp
public class ProductTestDataBuilder
{
    private string _id = "test-product-1";
    private string _name = "Test Product";

    public ProductTestDataBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public Product Build() => new() { Id = _id, Name = _name };
}

// Usage:
var product = new ProductTestDataBuilder()
    .WithId("custom-id")
    .Build();
```

## Continuous Integration

Tests run automatically on:
- Every commit (pre-commit hook)
- Pull requests (CI/CD pipeline)
- Before merge to main branch

Minimum requirements for merge:
- ✅ All tests passing
- ✅ Code coverage > 75%
- ✅ No new warnings/errors

## Security Testing

### Input Validation Tests
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("<script>alert('xss')</script>")]
public void ValidateInput_WithMaliciousInput_ThrowsValidationException(string input)
{
    var exception = Assert.Throws<ValidationException>(
        () => _validator.Validate(input));
    exception.Should().NotBeNull();
}
```

### SQL Injection Tests
```csharp
[Fact]
public async Task GetProduct_WithSqlInjectionAttempt_ReturnsSafely()
{
    // Arrange
    var maliciousId = "'; DROP TABLE Products; --";

    // Act
    var result = await _repository.GetByIdAsync(maliciousId);

    // Assert
    result.Should().BeNull(); // Not found, not executed
    var count = await _context.Products.CountAsync();
    count.Should().BeGreaterThan(0); // Table still exists
}
```

## Performance Testing

```csharp
[Fact]
public async Task GetProducts_ShouldComplete_WithinTimeout()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();

    // Act
    var results = await _service.GetAllProductsAsync();

    // Assert
    stopwatch.Stop();
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
}
```

## Documentation

Include XML comments in test code for clarity:

```csharp
/// <summary>
/// Tests that GetProduct returns the correct product when given a valid ID.
/// This ensures the happy path works correctly and data is properly retrieved.
/// </summary>
[Fact]
public async Task GetProduct_WithValidId_ReturnsCorrectProduct() { }
```

## References

- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore)
- [Moq Documentation](https://github.com/moq/moq4/wiki/Quickstart)
- [Shouldly Documentation](https://shouldly.readthedocs.io/)
- [AAA Pattern](https://www.linkedin.com/pulse/aaa-pattern-unit-testing-arrangeactassert/)
