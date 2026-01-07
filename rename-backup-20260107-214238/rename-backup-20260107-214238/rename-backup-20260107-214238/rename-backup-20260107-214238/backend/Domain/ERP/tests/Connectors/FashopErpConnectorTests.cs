using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2X.ERP.Abstractions;
using B2X.ERP.Connectors;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace B2X.ERP.Tests.Connectors;

public class FashopErpConnectorTests
{
    private readonly ILogger<FashopErpConnector> _loggerMock;
    private readonly FashopErpConnector _connector;

    public FashopErpConnectorTests()
    {
        _loggerMock = Substitute.For<ILogger<FashopErpConnector>>();
        _connector = new FashopErpConnector(_loggerMock);
    }

    [Fact]
    public async Task InitializeAsync_ValidConfig_Succeeds()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "fashop",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["server"] = "localhost",
                ["database"] = "fashop_db"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };

        // Act
        await _connector.InitializeAsync(config);

        // Assert
        var capabilities = await _connector.GetCapabilitiesAsync();
        Assert.NotNull(capabilities);
        Assert.True(capabilities.Catalog.Supported);
        Assert.True(capabilities.Order.Supported);
        Assert.True(capabilities.Customer.Supported);
    }

    [Fact]
    public async Task InitializeAsync_InvalidErpType_ThrowsException()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "invalid",
            TenantId = "test-tenant"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _connector.InitializeAsync(config));
    }

    [Fact]
    public async Task GetCapabilitiesAsync_ReturnsRetailCapabilities()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "fashop",
            TenantId = "test-tenant"
        };
        await _connector.InitializeAsync(config);

        // Act
        var capabilities = await _connector.GetCapabilitiesAsync();

        // Assert
        Assert.True(capabilities.Catalog.Supported);
        Assert.True(capabilities.Order.Supported);
        Assert.True(capabilities.Customer.Supported);
        Assert.True(capabilities.Inventory.Supported);
        Assert.True(capabilities.RealTime.Supported);
        Assert.Equal(1000, capabilities.Batch.MaxBatchSize);
        Assert.Contains("products", capabilities.SupportedDataTypes);
        Assert.Contains("customers", capabilities.SupportedDataTypes);
        Assert.Contains("orders", capabilities.SupportedDataTypes);
        Assert.Contains("inventory", capabilities.SupportedDataTypes);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidOrder_ReturnsResult()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "fashop",
            TenantId = "test-tenant"
        };
        await _connector.InitializeAsync(config);

        var order = new ErpOrder
        {
            OrderId = "TEST-001",
            CustomerId = "CUST-001",
            Lines = new List<ErpOrderLine>
            {
                new ErpOrderLine
                {
                    ProductId = "PROD-001",
                    Quantity = 2,
                    UnitPrice = 10.00m
                }
            }
        };

        // Act
        var result = await _connector.CreateOrderAsync(order);

        // Assert
        Assert.True(result.Success);
        Assert.Equal($"FASHOP-{order.OrderId}", result.ErpOrderId);
        Assert.Equal("confirmed", result.AdditionalData["status"]);
        Assert.NotNull(result.AdditionalData["createdAt"]);
    }

    [Fact]
    public async Task GetCustomerDataAsync_ValidCustomerId_ReturnsData()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "fashop",
            TenantId = "test-tenant"
        };
        await _connector.InitializeAsync(config);

        // Act
        var customerData = await _connector.GetCustomerDataAsync("CUST-001");

        // Assert
        Assert.Equal("CUST-001", customerData.CustomerId);
        Assert.Equal("Sample Customer", customerData.Name);
        Assert.Equal("customer@example.com", customerData.Email);
    }

    [Fact]
    public async Task Operations_BeforeInitialization_ThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _connector.GetCapabilitiesAsync());
        await Assert.ThrowsAsync<InvalidOperationException>(() => _connector.CreateOrderAsync(new ErpOrder()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _connector.GetCustomerDataAsync("test"));
    }
}
