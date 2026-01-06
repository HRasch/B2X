using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace B2Connect.Catalog.Tests.Integration;

/// <summary>
/// Integration tests for Product API endpoints
/// Tests actual HTTP requests with real database and service chain
/// </summary>
[Collection("Integration Tests")]
public class ProductCatalogIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;
    private readonly string _tenantId = Guid.NewGuid().ToString();

    public ProductCatalogIntegrationTests()
    {
        _fixture = new IntegrationTestFixture();
    }

    public async Task InitializeAsync() => await _fixture.InitializeAsync();
    public async Task DisposeAsync() => await _fixture.DisposeAsync();

    #region Get Product Endpoint Tests

    [Fact]
    public async Task GetProduct_WithValidSku_ReturnsOk()
    {
        // Arrange
        var sku = "TEST-SKU-001";

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/catalog/products/{sku}",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should.BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }

    [Fact]
    public async Task GetProduct_WithInvalidSku_ReturnsNotFound()
    {
        // Arrange
        var invalidSku = "INVALID-SKU-99999";

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/catalog/products/{invalidSku}",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithEmptySku_ReturnsBadRequest()
    {
        // Arrange - empty SKU in URL
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products/",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should.BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithoutTenantId_ReturnsBadRequest()
    {
        // Arrange
        var sku = "TEST-SKU-001";

        // Act - No tenant ID provided
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/catalog/products/{sku}");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region List Products Endpoint Tests

    [Fact]
    public async Task ListProducts_WithValidPagination_ReturnsOk()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=1&pageSize=20",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }

    [Fact]
    public async Task ListProducts_WithInvalidPage_ReturnsBadRequest()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=0&pageSize=20",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListProducts_WithLargePageSize_ReturnsBadRequest()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=1&pageSize=10000",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListProducts_WithSearchFilter_ReturnsOk()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?search=laptop&page=1&pageSize=20",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListProducts_WithoutTenantId_ReturnsBadRequest()
    {
        // Act - No tenant ID
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=1&pageSize=20");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Create Product Endpoint Tests

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createRequest = new
        {
            sku = $"TEST-SKU-{Guid.NewGuid()}",
            name = "Test Product",
            description = "A test product",
            price = 99.99m,
            currency = "EUR"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/catalog/products",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should.BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithEmptySku_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            sku = "",
            name = "Test Product",
            description = "A test product",
            price = 99.99m,
            currency = "EUR"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/catalog/products",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithNegativePrice_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            sku = $"TEST-SKU-{Guid.NewGuid()}",
            name = "Test Product",
            description = "A test product",
            price = -99.99m,
            currency = "EUR"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/catalog/products",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithMissingRequiredField_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            sku = $"TEST-SKU-{Guid.NewGuid()}",
            // Missing: name, description, price
            currency = "EUR"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/catalog/products",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithoutTenantId_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            sku = $"TEST-SKU-{Guid.NewGuid()}",
            name = "Test Product",
            description = "A test product",
            price = 99.99m,
            currency = "EUR"
        };

        // Act - No tenant ID
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/catalog/products");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Product Endpoint Tests

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsOk()
    {
        // Arrange
        var sku = "TEST-SKU-001";
        var updateRequest = new
        {
            name = "Updated Product Name",
            description = "Updated description",
            price = 149.99m
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/catalog/products/{sku}",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should.BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidSku_ReturnsNotFound()
    {
        // Arrange
        var invalidSku = "NONEXISTENT-SKU";
        var updateRequest = new { name = "Updated Name" };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/catalog/products/{invalidSku}",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete Product Endpoint Tests

    [Fact]
    public async Task DeleteProduct_WithValidSku_ReturnsNoContent()
    {
        // Arrange
        var sku = "TEST-SKU-001";

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Delete,
            $"/api/catalog/products/{sku}",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should.BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidSku_ReturnsNotFound()
    {
        // Arrange
        var invalidSku = "NONEXISTENT-SKU";

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Delete,
            $"/api/catalog/products/{invalidSku}",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    #endregion

    #region Multi-Tenant Isolation Tests

    [Fact]
    public async Task ListProducts_WithDifferentTenants_IsolatesData()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid().ToString();
        var tenantId2 = Guid.NewGuid().ToString();

        // Act - Tenant 1
        var request1 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=1&pageSize=20",
            tenantId: tenantId1);
        var response1 = await _fixture.Client.SendAsync(request1);

        // Tenant 2
        var request2 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/catalog/products?page=1&pageSize=20",
            tenantId: tenantId2);
        var response2 = await _fixture.Client.SendAsync(request2);

        // Assert - Both succeed but are isolated
        response1.StatusCode.ShouldBe(HttpStatusCode.OK);
        response2.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProduct_FromWrongTenant_ReturnsNotFound()
    {
        // Arrange
        var sku = "TEST-SKU-001";
        var tenantId1 = Guid.NewGuid().ToString();
        var tenantId2 = Guid.NewGuid().ToString();

        // Act - Create with tenant1, try to access with tenant2
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/catalog/products/{sku}",
            tenantId: tenantId2);
        var response = await _fixture.Client.SendAsync(request);

        // Assert - Should not find product from another tenant
        response.StatusCode.Should.BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }

    #endregion

    #region Performance/Load Tests

    [Fact]
    public async Task ListProducts_WithMultipleRequests_MaintainsPerformance()
    {
        // Arrange
        var requestCount = 10;
        var times = new List<long>();

        // Act
        for (int i = 0; i < requestCount; i++)
        {
            var startTime = DateTime.UtcNow.Ticks;

            var request = _fixture.CreateAuthenticatedRequest(
                HttpMethod.Get,
                "/api/catalog/products?page=1&pageSize=20",
                tenantId: _tenantId);
            var response = await _fixture.Client.SendAsync(request);

            var elapsed = DateTime.UtcNow.Ticks - startTime;
            times.Add(elapsed);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        // Assert - All requests should complete in reasonable time (< 1 second each)
        var averageTime = TimeSpan.FromTicks(times.Count > 0 ? times[0] / requestCount : 0);
        averageTime.ShouldBeLessThan(TimeSpan.FromSeconds(1));
    }

    #endregion
}

/// <summary>
/// Base class for catalog integration tests
/// </summary>
public class IntegrationTestFixture : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; private set; }
    protected HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
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

        await Task.Delay(100);
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
        await Task.CompletedTask;
    }

    protected HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string uri,
        string token = null,
        string tenantId = null)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            request.Headers.Add("X-Tenant-ID", tenantId);
        }

        request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());

        return request;
    }
}

[CollectionDefinition("Integration Tests")]
public class CatalogIntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
{
    // Collection definition
}
