using System.Net;
using System.Net.Http.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using B2X.IdsConnectAdapter.Controllers;
using B2X.IdsConnectAdapter.Models;
using B2X.Shared.Infrastructure.ServiceClients;
using B2X.Domain.Search.Services;
using B2X.ERP.Abstractions;

namespace B2X.IdsConnectAdapter.Tests.Controllers;

public class IdsWarenkorbControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<ICatalogServiceClient> _catalogClientMock;
    private Mock<ICustomerServiceClient> _customerClientMock;
    private Mock<ITenantResolver> _tenantResolverMock;

    public IdsWarenkorbControllerTests(WebApplicationFactory<Program> factory)
    {
        _catalogClientMock = new Mock<ICatalogServiceClient>();
        _customerClientMock = new Mock<ICustomerServiceClient>();
        _tenantResolverMock = new Mock<ITenantResolver>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the service clients
                services.AddSingleton(_catalogClientMock.Object);
                services.AddSingleton(_customerClientMock.Object);
                services.AddSingleton(_tenantResolverMock.Object);
            });
        });
    }

    [Fact]
    public async Task WarenkorbSenden_ValidRequest_ReturnsOkWithBestellnummer()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        var customerId = "KUNDE001";
        var customer = new B2X.Shared.Infrastructure.ServiceClients.CustomerDto
        {
            Id = Guid.NewGuid(),
            ErpCustomerId = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            LastModified = DateTime.UtcNow
        };
        _customerClientMock.Setup(x => x.GetCustomerByErpIdAsync(customerId, tenantId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<B2X.Shared.Infrastructure.ServiceClients.CustomerDto?>(customer));

        var productSku = "ART001";
        var product = new ProductDto(
            Id: Guid.NewGuid(),
            ErpProductId: null,
            Sku: productSku,
            Name: "Test Product",
            Description: null,
            Price: 99.99m,
            StockLevel: 100,
            LastModified: DateTime.UtcNow,
            TenantId: tenantId);
        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(productSku, tenantId))
            .ReturnsAsync(product);

        var warenkorb = new IdsWarenkorbSenden
        {
            Version = "2.5",
            Kunde = new IdsKunde { Id = customerId, Name = "Test Kunde" },
            Positionen = new List<IdsWarenkorbPosition>
            {
                new IdsWarenkorbPosition
                {
                    Artikelnummer = productSku,
                    Menge = 2,
                    Einheit = "Stk"
                }
            }
        };

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Serialize to XML
        var xmlSerializer = new XmlSerializer(typeof(IdsWarenkorbSenden));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, warenkorb);
        var xmlContent = stringWriter.ToString();

        // Act
        var response = await client.PostAsync("/api/ids/warenkorb/senden",
            new StringContent(xmlContent, System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Bestellnummer", responseContent);
        Assert.Contains("IDS-", responseContent);
    }

    [Fact]
    public async Task WarenkorbSenden_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/ids/warenkorb/senden",
            new StringContent("<invalid></invalid>", System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task WarenkorbSenden_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var host = "test.example.com";
        _tenantResolverMock.Setup(x => x.ResolveTenantIdFromHost(host)).Returns(tenantId.ToString());

        var customerId = "KUNDE001";
        var customer = new B2X.Shared.Infrastructure.ServiceClients.CustomerDto
        {
            Id = Guid.NewGuid(),
            ErpCustomerId = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            LastModified = DateTime.UtcNow
        };
        _customerClientMock.Setup(x => x.GetCustomerByErpIdAsync(customerId, tenantId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<B2X.Shared.Infrastructure.ServiceClients.CustomerDto?>(customer));

        var productSku = "NONEXISTENT";
        _catalogClientMock.Setup(x => x.GetProductBySkuAsync(productSku, tenantId))
            .ReturnsAsync((ProductDto?)null);

        var warenkorb = new IdsWarenkorbSenden
        {
            Version = "2.5",
            Kunde = new IdsKunde { Id = customerId, Name = "Test Kunde" },
            Positionen = new List<IdsWarenkorbPosition>
            {
                new IdsWarenkorbPosition
                {
                    Artikelnummer = productSku,
                    Menge = 1,
                    Einheit = "Stk"
                }
            }
        };

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Host = host;

        // Serialize to XML
        var xmlSerializer = new XmlSerializer(typeof(IdsWarenkorbSenden));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, warenkorb);
        var xmlContent = stringWriter.ToString();

        // Act
        var response = await client.PostAsync("/api/ids/warenkorb/senden",
            new StringContent(xmlContent, System.Text.Encoding.UTF8, "application/xml"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WarenkorbEmpfangen_ValidBestellnummer_ReturnsOk()
    {
        // Arrange
        var bestellnummer = "TEST-ORDER-123";
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/ids/warenkorb/empfangen/{bestellnummer}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains(bestellnummer, responseContent);
        Assert.Contains("OK", responseContent);
    }

    [Fact]
    public async Task WarenkorbEmpfangen_InvalidBestellnummer_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ids/warenkorb/empfangen/");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}