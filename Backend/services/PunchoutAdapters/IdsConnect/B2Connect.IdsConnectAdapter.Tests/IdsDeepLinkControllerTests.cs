using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using B2Connect.IdsConnectAdapter.Controllers;
using B2Connect.IdsConnectAdapter.Models;
using B2Connect.Shared.Infrastructure.ServiceClients;
using B2Connect.Domain.Search.Services;

namespace B2Connect.IdsConnectAdapter.Tests.Controllers;

public class IdsDeepLinkControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<ICatalogServiceClient> _catalogClientMock;
    private Mock<ITenantResolver> _tenantResolverMock;

    public IdsDeepLinkControllerTests(WebApplicationFactory<Program> factory)
    {
        _catalogClientMock = new Mock<ICatalogServiceClient>();
        _tenantResolverMock = new Mock<ITenantResolver>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the service clients
                services.AddSingleton(_catalogClientMock.Object);
                services.AddSingleton(_tenantResolverMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetArtikelDeepLink_ValidArtikelnummer_ReturnsOkWithDeepLink()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        var artikelnummer = "ART001";
        var productId = Guid.NewGuid();

        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        var product = new ProductDto(
            Id: productId,
            ErpProductId: null,
            Sku: artikelnummer,
            Name: "Test Product",
            Description: null,
            Price: 99.99m,
            StockLevel: 100,
            LastModified: DateTime.UtcNow,
            TenantId: tenantId);

        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(artikelnummer, tenantId))
            .ReturnsAsync(product);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Act
        var response = await client.GetAsync($"/api/ids/deeplink/artikel/{artikelnummer}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("DeepLink", responseContent);
        Assert.Contains("store/product/", responseContent);
        Assert.Contains(artikelnummer, responseContent);
        Assert.Contains("Test Product", responseContent);
    }

    [Fact]
    public async Task GetArtikelDeepLink_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        var artikelnummer = "NONEXISTENT";

        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(artikelnummer, tenantId))
            .ReturnsAsync((ProductDto?)null);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Act
        var response = await client.GetAsync($"/api/ids/deeplink/artikel/{artikelnummer}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetArtikelDeepLink_EmptyArtikelnummer_ReturnsMethodNotAllowed()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ids/deeplink/artikel/");

        // Assert
        // Empty route segment returns MethodNotAllowed because GET /api/ids/deeplink/artikel/ is not a valid route
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task CreateArtikelDeepLink_ValidRequest_ReturnsOkWithDeepLink()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        var artikelnummer = "ART001";
        var customerId = "KUNDE001";
        var productId = Guid.NewGuid();

        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        var product = new ProductDto(
            Id: productId,
            ErpProductId: null,
            Sku: artikelnummer,
            Name: "Test Product",
            Description: null,
            Price: 99.99m,
            StockLevel: 100,
            LastModified: DateTime.UtcNow,
            TenantId: tenantId);

        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(artikelnummer, tenantId))
            .ReturnsAsync(product);

        var request = new IdsDeepLinkRequest
        {
            Version = "2.5",
            Kunde = new IdsKunde { Id = customerId, Name = "Test Customer" },
            Artikelnummer = artikelnummer
        };

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Serialize to XML
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(IdsDeepLinkRequest));
        using var stringWriter = new System.IO.StringWriter();
        xmlSerializer.Serialize(stringWriter, request);
        var xmlContent = stringWriter.ToString();

        // Act
        var response = await client.PostAsync("/api/ids/deeplink/artikel",
            new System.Net.Http.StringContent(xmlContent, System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("DeepLink", responseContent);
        Assert.Contains("store/product/", responseContent);
        Assert.Contains($"customer={customerId}", responseContent);
        Assert.Contains(artikelnummer, responseContent);
    }

    [Fact]
    public async Task CreateArtikelDeepLink_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/ids/deeplink/artikel",
            new System.Net.Http.StringContent("<invalid></invalid>", System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateArtikelDeepLink_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        var artikelnummer = "NONEXISTENT";

        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(artikelnummer, tenantId))
            .ReturnsAsync((ProductDto?)null);

        var request = new IdsDeepLinkRequest
        {
            Version = "2.5",
            Kunde = new IdsKunde { Id = "KUNDE001", Name = "Test Customer" },
            Artikelnummer = artikelnummer
        };

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Serialize to XML
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(IdsDeepLinkRequest));
        using var stringWriter = new System.IO.StringWriter();
        xmlSerializer.Serialize(stringWriter, request);
        var xmlContent = stringWriter.ToString();

        // Act
        var response = await client.PostAsync("/api/ids/deeplink/artikel",
            new System.Net.Http.StringContent(xmlContent, System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}