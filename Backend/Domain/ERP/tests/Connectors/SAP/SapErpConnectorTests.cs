using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Connectors.SAP;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace B2Connect.ERP.Tests.Connectors.SAP;

public class SapErpConnectorTests : IDisposable
{
    private readonly ILogger<SapErpConnector> _loggerMock;
    private readonly HttpClient _httpClient;
    private readonly SapErpConnector _connector;

    public SapErpConnectorTests()
    {
        _loggerMock = Substitute.For<ILogger<SapErpConnector>>();

        // Create a test HttpClient with a custom handler that returns mock responses
        var handler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(handler);

        _connector = new SapErpConnector(_loggerMock, _httpClient);
    }

    [Fact]
    public async Task InitializeAsync_ValidSapConfig_Succeeds()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com",
                ["systemId"] = "PRD",
                ["client"] = "100"
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
    public async Task InitializeAsync_MissingServiceUrl_ThrowsException()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["systemId"] = "PRD"
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _connector.InitializeAsync(config));
    }

    [Fact]
    public async Task GetCapabilitiesAsync_ReturnsEnterpriseCapabilities()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ErpVersion = "S/4HANA 2022",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };
        await _connector.InitializeAsync(config);

        // Act
        var capabilities = await _connector.GetCapabilitiesAsync();

        // Assert - Catalog capabilities
        Assert.True(capabilities.Catalog.Supported);
        Assert.True(capabilities.Catalog.SupportsFullSync);
        Assert.True(capabilities.Catalog.SupportsDeltaSync); // S/4HANA supports delta
        Assert.True(capabilities.Catalog.SupportsRealTimeUpdates);
        Assert.Contains("materials", capabilities.Catalog.SupportedEntityTypes);

        // Assert - Order capabilities
        Assert.True(capabilities.Order.Supported);
        Assert.True(capabilities.Order.SupportsStatusUpdates);
        Assert.True(capabilities.Order.SupportsCancellation);
        Assert.True(capabilities.Order.SupportsReturns); // S/4HANA supports returns
        Assert.True(capabilities.Order.SupportsPartialOrders);

        // Assert - Customer capabilities
        Assert.True(capabilities.Customer.Supported);
        Assert.True(capabilities.Customer.SupportsCreation);
        Assert.True(capabilities.Customer.SupportsUpdates);
        Assert.True(capabilities.Customer.SupportsAddressManagement);
        Assert.True(capabilities.Customer.SupportsCreditLimitChecks);

        // Assert - Inventory capabilities
        Assert.True(capabilities.Inventory.Supported);
        Assert.True(capabilities.Inventory.SupportsRealTimeUpdates);
        Assert.True(capabilities.Inventory.SupportsReservations);
        Assert.True(capabilities.Inventory.SupportsMultiLocation);
        Assert.True(capabilities.Inventory.SupportsLowStockAlerts);

        // Assert - Real-time capabilities
        Assert.True(capabilities.RealTime.Supported);
        Assert.Contains("material-change", capabilities.RealTime.SupportedEventTypes);
        Assert.True(capabilities.RealTime.SupportsWebhooks); // S/4HANA supports webhooks
        Assert.True(capabilities.RealTime.SupportsPolling);

        // Assert - Batch capabilities
        Assert.True(capabilities.Batch.Supported);
        Assert.Equal(1000, capabilities.Batch.MaxBatchSize);
        Assert.True(capabilities.Batch.SupportsBulkImport);
        Assert.True(capabilities.Batch.SupportsBulkExport);

        // Assert - General capabilities
        Assert.Contains("oauth2", capabilities.SupportedAuthTypes);
        Assert.Contains("basic", capabilities.SupportedAuthTypes);
        Assert.Contains("certificate", capabilities.SupportedAuthTypes);
        Assert.Contains("materials", capabilities.SupportedDataTypes);
        Assert.Contains("customers", capabilities.SupportedDataTypes);

        // Assert - Custom capabilities
        Assert.True((bool)capabilities.CustomCapabilities["supportsIdoc"]);
        Assert.True((bool)capabilities.CustomCapabilities["supportsRfc"]);
        Assert.True((bool)capabilities.CustomCapabilities["supportsOData"]);
        Assert.True((bool)capabilities.CustomCapabilities["supportsMultiCompany"]);
    }

    [Fact]
    public async Task Version_ReturnsCorrectVersionInfo()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ErpVersion = "S/4HANA 2022",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };
        await _connector.InitializeAsync(config);

        // Act
        var version = _connector.Version;

        // Assert
        Assert.Equal("SAP ERP/S4HANA", version.SystemName);
        Assert.Equal("S/4HANA 2022", version.SystemVersion);
        Assert.Equal("OData V4", version.ApiVersion);
        Assert.True(version.SupportsBackwardCompatibility);
        Assert.Equal("S/4HANA 1909", version.MinimumSystemVersion);
        Assert.Equal("S/4HANA 2022+", version.RecommendedSystemVersion);
    }

    [Fact]
    public async Task Version_ReturnsDefaultWhenNoVersionSpecified()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };
        await _connector.InitializeAsync(config);

        // Act
        var version = _connector.Version;

        // Assert
        Assert.Equal("SAP ERP/S4HANA", version.SystemName);
        Assert.Equal("S/4HANA 2022+", version.SystemVersion);
    }

    [Fact]
    public async Task GetCapabilitiesAsync_EccVersion_HasLimitedFeatures()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ErpVersion = "SAP ECC 6.0", // Older version
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };
        await _connector.InitializeAsync(config);

        // Act
        var capabilities = await _connector.GetCapabilitiesAsync();

        // Assert - ECC has more limited features
        Assert.True(capabilities.Catalog.Supported);
        Assert.True(capabilities.Catalog.SupportsFullSync);
        Assert.False(capabilities.Catalog.SupportsDeltaSync); // ECC doesn't support delta sync as well
        Assert.False(capabilities.Catalog.SupportsRealTimeUpdates); // Limited real-time support

        Assert.True(capabilities.Order.Supported);
        Assert.True(capabilities.Order.SupportsStatusUpdates);
        Assert.True(capabilities.Order.SupportsCancellation);
        Assert.False(capabilities.Order.SupportsReturns); // ECC has limited returns support

        Assert.False(capabilities.RealTime.SupportsWebhooks); // ECC doesn't support webhooks
        Assert.True(capabilities.RealTime.SupportsPolling); // But supports polling

        Assert.True(capabilities.Batch.Supported);
        Assert.Equal(1000, capabilities.Batch.MaxBatchSize);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidOrder_ReturnsResult()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };

        await _connector.InitializeAsync(config);

        var order = new ErpOrder
        {
            OrderId = "SAP-TEST-001",
            CustomerId = "CUST-001",
            Lines = new List<ErpOrderLine>
            {
                new ErpOrderLine
                {
                    ProductId = "MAT-001",
                    Quantity = 10,
                    UnitPrice = 100.00m
                }
            }
        };

        // Act
        var result = await _connector.CreateOrderAsync(order);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("SAP-TEST-001", result.ErpOrderId); // SAP returns the order ID directly
        Assert.Equal("created", result.AdditionalData["status"]);
    }

    [Fact]
    public async Task GetCustomerDataAsync_ValidCustomerId_ReturnsData()
    {
        // Arrange
        var config = new ErpConfiguration
        {
            ErpType = "sap",
            TenantId = "test-tenant",
            ConnectionSettings = new()
            {
                ["serviceUrl"] = "https://sap-system.company.com"
            },
            Authentication = new ErpAuthentication
            {
                Type = "basic",
                Username = "testuser",
                Password = "testpass"
            }
        };

        await _connector.InitializeAsync(config);

        // Act
        var customerData = await _connector.GetCustomerDataAsync("CUST-001");

        // Assert
        Assert.Equal("CUST-001", customerData.CustomerId);
        Assert.Equal("Sample Customer", customerData.Name); // Note: This is stub data
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

    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Test HTTP message handler that returns mock responses for SAP API calls
/// </summary>
public class TestHttpMessageHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Mock customer data response
        if (request.RequestUri?.PathAndQuery.Contains("API_BUSINESS_PARTNER_SRV/A_Customer") == true)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"{
                    ""CustomerName"": ""Sample Customer"",
                    ""EmailAddress"": ""customer@example.com"",
                    ""to_CustomerAddress"": {
                        ""results"": [{
                            ""StreetName"": ""123 Main St"",
                            ""CityName"": ""Anytown"",
                            ""PostalCode"": ""12345"",
                            ""Country"": ""US""
                        }]
                    }
                }", System.Text.Encoding.UTF8, "application/json")
            };
        }

        // Mock order creation response
        if (request.RequestUri?.PathAndQuery.Contains("API_SALES_ORDER_SRV/A_SalesOrder") == true)
        {
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(@"{
                    ""SalesOrder"": ""SAP-TEST-001"",
                    ""SalesOrderType"": ""OR"",
                    ""SoldToParty"": ""CUST-001""
                }", System.Text.Encoding.UTF8, "application/json")
            };
        }

        // Default response for any other requests
        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }
}
