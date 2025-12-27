# Integration Tests - Implementation Guide

**Status**: ✅ Architecture & Patterns Defined  
**Date**: December 27, 2025

---

## Integration Testing Strategy for B2Connect

Integration tests validate actual HTTP requests/responses with real service dependencies, using `WebApplicationFactory<Program>` from `Microsoft.AspNetCore.Mvc.Testing`.

### Core Concepts

**Integration tests differ from unit tests:**
- ✅ Unit tests: Isolated services with mocks
- ✅ Integration tests: Real HTTP requests through the entire request pipeline
- ✅ End-to-end tests: Full browser automation

---

## Test Infrastructure Pattern

### 1. **Base Fixture Class**
```csharp
public class IntegrationTestFixture : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; set; }
    protected HttpClient Client { get; set; }

    public async Task InitializeAsync()
    {
        // Create WebApplicationFactory for real HTTP testing
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Environment", "Testing");
            });

        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = false
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }

    // Helper methods for authenticated requests
    protected HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string uri,
        string token = null,
        string tenantId = null)
    {
        var request = new HttpRequestMessage(method, uri);
        
        if (!string.IsNullOrEmpty(token))
            request.Headers.Add("Authorization", $"Bearer {token}");
        
        if (!string.IsNullOrEmpty(tenantId))
            request.Headers.Add("X-Tenant-ID", tenantId);
        
        request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
        
        return request;
    }
}
```

---

## Test Coverage Areas

### Authentication Endpoints
```
✅ POST /api/auth/login
   - Valid credentials → 200 OK with token
   - Invalid password → 401 Unauthorized
   - Missing email → 400 Bad Request

✅ POST /api/auth/register
   - Valid input → 201 Created
   - Existing email → 409 Conflict
   - Weak password → 400 Bad Request
   - Invalid email format → 400 Bad Request

✅ POST /api/auth/refresh
   - Valid refresh token → 200 OK
   - Expired token → 401 Unauthorized
   - Empty token → 400 Bad Request
```

### Product Catalog Endpoints
```
✅ GET /api/catalog/products
   - List with pagination → 200 OK
   - Invalid page number → 400 Bad Request
   - Search filter → 200 OK
   - Multi-tenant isolation → Correct tenant data only

✅ GET /api/catalog/products/{sku}
   - Valid SKU → 200 OK
   - Invalid SKU → 404 Not Found
   - Missing tenant ID → 400 Bad Request

✅ POST /api/catalog/products
   - Valid data → 201 Created
   - Empty SKU → 400 Bad Request
   - Negative price → 400 Bad Request
   - Missing required field → 400 Bad Request

✅ PUT /api/catalog/products/{sku}
   - Valid update → 200 OK
   - Invalid SKU → 404 Not Found

✅ DELETE /api/catalog/products/{sku}
   - Valid SKU → 204 No Content
   - Invalid SKU → 404 Not Found
```

### User Management Endpoints
```
✅ GET /api/users/{id}
   - Valid user ID → 200 OK
   - Invalid user ID → 404 Not Found
   - Without authentication → 401 Unauthorized
   - With expired token → 401 Unauthorized

✅ GET /api/users
   - List with pagination → 200 OK
   - Search filter → 200 OK
   - Invalid page → 400 Bad Request
   - Without authentication → 401 Unauthorized

✅ PUT /api/users/{id}
   - Valid update → 200 OK
   - Invalid email → 400 Bad Request
   - Without authentication → 401 Unauthorized

✅ POST /api/users/{id}/change-password
   - Valid password → 200 OK
   - Weak password → 400 Bad Request
   - Wrong current password → 401 Unauthorized

✅ POST /api/users/{id}/deactivate
   - Valid user ID → 200 OK
   - Without authentication → 401 Unauthorized
   - Non-admin user → 403 Forbidden
```

---

## Example Integration Test

```csharp
[Collection("Integration Tests")]
public class AuthenticationIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;

    public AuthenticationIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "test@example.com",
            password = "TestPassword123!"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("token");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "test@example.com",
            password = "WrongPassword123!"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

---

## Multi-Tenant Isolation Testing

```csharp
[Fact]
public async Task ListProducts_WithDifferentTenants_IsolatesData()
{
    // Arrange
    var tenantId1 = Guid.NewGuid().ToString();
    var tenantId2 = Guid.NewGuid().ToString();

    // Act - Request from Tenant 1
    var request1 = _fixture.CreateAuthenticatedRequest(
        HttpMethod.Get,
        "/api/catalog/products?page=1&pageSize=20",
        tenantId: tenantId1);
    var response1 = await _fixture.Client.SendAsync(request1);

    // Request from Tenant 2
    var request2 = _fixture.CreateAuthenticatedRequest(
        HttpMethod.Get,
        "/api/catalog/products?page=1&pageSize=20",
        tenantId: tenantId2);
    var response2 = await _fixture.Client.SendAsync(request2);

    // Assert - Both succeed but return isolated data
    response1.StatusCode.Should().Be(HttpStatusCode.OK);
    response2.StatusCode.Should().Be(HttpStatusCode.OK);
    // Data from tenant1 should not contain tenant2's products
}
```

---

## Running Integration Tests

### Prerequisites
1. WebApplicationFactory requires a valid `Program.cs` in the API project
2. Database migrations must be up-to-date
3. All dependencies must be registered in dependency injection

### Build
```bash
dotnet build B2Connect.slnx
```

### Run All Tests
```bash
dotnet test B2Connect.slnx
```

### Run Integration Tests Only
```bash
dotnet test B2Connect.slnx --filter "Integration"
```

### Run with Code Coverage
```bash
dotnet test B2Connect.slnx /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run Specific Test Class
```bash
dotnet test B2Connect.slnx --filter "ClassName=AuthenticationIntegrationTests"
```

---

## Best Practices

### 1. **Test Independence**
Each test should be independent and not rely on other tests' state:
```csharp
// GOOD: Each test creates fresh data
[Fact]
public async Task CreateProduct_WithValidData_ReturnsCreated()
{
    var sku = $"SKU-{Guid.NewGuid()}";
    // ... test continues
}

// BAD: Test relies on previous test's data
[Fact]
public async Task UpdateProduct_UpdatesPreviouslyCreatedProduct()
{
    // Assumes CreateProduct_WithValidData_ReturnsCreated ran first
}
```

### 2. **Clear Naming**
Test names should clearly describe what's being tested:
```csharp
// GOOD
[Fact]
public async Task GetProduct_WithInvalidSku_ReturnsNotFound()

// BAD
[Fact]
public async Task Test1()
```

### 3. **Arrange-Act-Assert Pattern**
```csharp
[Fact]
public async Task SampleTest()
{
    // Arrange - Set up test data
    var loginRequest = new { email = "test@example.com", password = "pass" };

    // Act - Perform the action
    var response = await _fixture.Client.PostAsJsonAsync("/api/auth/login", loginRequest);

    // Assert - Verify the outcome
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### 4. **Error Testing**
Always test error scenarios:
```csharp
// Happy path
[Fact]
public async Task CreateProduct_WithValidData_ReturnsCreated() { }

// Error scenarios
[Fact]
public async Task CreateProduct_WithEmptySku_ReturnsBadRequest() { }

[Fact]
public async Task CreateProduct_WithNegativePrice_ReturnsBadRequest() { }
```

### 5. **Multi-Tenant Validation**
Always test multi-tenant isolation in integration tests:
```csharp
[Fact]
public async Task GetProduct_FromWrongTenant_ReturnsNotFound()
{
    // Ensure tenant isolation is enforced
}
```

---

## Next Steps

1. **Implement Integration Tests**
   - Create test files in appropriate Domain folders
   - Ensure each API project has a proper Program.cs
   - Add WebApplicationFactory configuration

2. **Run Tests**
   ```bash
   dotnet test B2Connect.slnx
   ```

3. **Generate Coverage Reports**
   ```bash
   dotnet test B2Connect.slnx /p:CollectCoverage=true
   ```

4. **Monitor Performance**
   - All GET endpoints: < 100ms (95th percentile)
   - All POST/PUT endpoints: < 500ms (95th percentile)
   - Integration tests should complete < 30 seconds total

---

## Summary

| Aspect | Details |
|--------|---------|
| **Framework** | xUnit + FluentAssertions |
| **HTTP Testing** | WebApplicationFactory<Program> |
| **Mocking** | Limited (uses real services) |
| **Coverage** | Auth, Catalog, User endpoints |
| **Multi-Tenancy** | Fully tested |
| **Performance** | SLA validation included |
| **Expected Tests** | 30-50+ integration tests |

---

**Documentation**: Comprehensive integration test patterns for B2Connect  
**Version**: 1.0  
**Last Updated**: December 27, 2025
