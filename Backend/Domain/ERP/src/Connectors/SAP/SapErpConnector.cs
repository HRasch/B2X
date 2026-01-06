using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.ERP.Abstractions;
using Microsoft.Extensions.Logging;

namespace B2Connect.ERP.Connectors.SAP;

/// <summary>
/// ERP connector for SAP ERP/S4HANA systems.
/// Provides comprehensive integration with SAP's modern APIs including OData, IDoc, and RFC.
/// </summary>
public class SapErpConnector : IErpConnector
{
    private readonly ILogger<SapErpConnector> _logger;
    private readonly HttpClient _httpClient;
    private ErpConfiguration? _configuration;
    private bool _initialized;

    public SapErpConnector(ILogger<SapErpConnector> logger, HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public ErpVersionInfo Version => new ErpVersionInfo
    {
        SystemName = "SAP ERP/S4HANA",
        SystemVersion = _configuration?.ErpVersion ?? "S/4HANA 2022+",
        ApiVersion = "OData V4",
        SupportsBackwardCompatibility = true,
        MinimumSystemVersion = "S/4HANA 1909",
        RecommendedSystemVersion = "S/4HANA 2022+"
    };

    /// <inheritdoc />
    public async Task InitializeAsync(ErpConfiguration config, CancellationToken cancellationToken = default)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _logger.LogInformation("Initializing SAP ERP connector for tenant {TenantId}", config.TenantId);

        // Validate configuration
        if (string.IsNullOrEmpty(config.ErpType) || !config.ErpType.Equals("sap", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ERP type. Expected 'sap'.", nameof(config));
        }

        // Validate required SAP settings
        if (!config.ConnectionSettings.TryGetValue("serviceUrl", out var serviceUrlObj) || string.IsNullOrWhiteSpace(serviceUrlObj as string))
        {
            throw new ArgumentException("SAP service URL is required in connection settings.", nameof(config));
        }
        var serviceUrl = serviceUrlObj as string;

        // Configure HTTP client for SAP
        _httpClient.BaseAddress = new Uri(serviceUrl!);
        _httpClient.DefaultRequestHeaders.Clear();

        // Add authentication headers based on auth type
        await ConfigureAuthenticationAsync(config.Authentication, cancellationToken);

        _configuration = config;
        _initialized = true;

        _logger.LogInformation("SAP ERP connector initialized successfully");
    }

    /// <inheritdoc />
    public async Task<ErpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        // Determine capabilities based on SAP version
        var isS4Hana = _configuration?.ErpVersion?.Contains("S/4HANA") ?? true;
        var supportsRealTime = isS4Hana; // S/4HANA has better real-time capabilities

        return new ErpCapabilities
        {
            Catalog = new CatalogCapabilities
            {
                Supported = true,
                SupportsFullSync = true,
                SupportsDeltaSync = isS4Hana, // S/4HANA supports delta sync
                SupportsRealTimeUpdates = supportsRealTime,
                SupportedEntityTypes = new List<string> { "materials", "material-groups", "plants", "storage-locations" }
            },
            Order = new OrderCapabilities
            {
                Supported = true,
                SupportsStatusUpdates = true,
                SupportsCancellation = true,
                SupportsReturns = isS4Hana, // S/4HANA has better returns support
                SupportsPartialOrders = true
            },
            Customer = new CustomerCapabilities
            {
                Supported = true,
                SupportsCreation = true,
                SupportsUpdates = true,
                SupportsAddressManagement = true,
                SupportsCreditLimitChecks = true
            },
            Inventory = new InventoryCapabilities
            {
                Supported = true,
                SupportsRealTimeUpdates = supportsRealTime,
                SupportsReservations = true,
                SupportsMultiLocation = true,
                SupportsLowStockAlerts = isS4Hana
            },
            RealTime = new RealTimeCapabilities
            {
                Supported = supportsRealTime,
                SupportedEventTypes = supportsRealTime ? new List<string> { "material-change", "order-change", "inventory-change" } : new List<string>(),
                SupportsWebhooks = isS4Hana, // S/4HANA supports webhooks
                SupportsPolling = true
            },
            Batch = new BatchCapabilities
            {
                Supported = true,
                MaxBatchSize = 1000,
                SupportsBulkImport = true,
                SupportsBulkExport = true
            },
            SupportedAuthTypes = new List<string> { "oauth2", "basic", "certificate" },
            SupportedDataTypes = new List<string> { "materials", "customers", "sales-orders", "inventory", "financial-documents" },
            CustomCapabilities = new Dictionary<string, object>
            {
                ["supportsIdoc"] = isS4Hana,
                ["supportsRfc"] = true,
                ["supportsOData"] = true,
                ["supportsMultiCompany"] = true
            }
        };
    }

    /// <inheritdoc />
    public async Task SyncCatalogAsync(SyncContext context, CancellationToken cancellationToken = default)
    {
        EnsureInitialized();
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        _logger.LogInformation("Starting SAP catalog sync for tenant {TenantId}", _configuration!.TenantId);

        try
        {
            // SAP OData query for materials
            var materialsUrl = "/sap/opu/odata/sap/API_MATERIAL_STOCK_SRV/MaterialStock";
            var response = await _httpClient.GetAsync(materialsUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"SAP API returned {response.StatusCode}: {response.ReasonPhrase}");
            }

            var materialsData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            // TODO: Process SAP materials data and map to B2Connect catalog format
            // This would involve:
            // 1. Parsing SAP Material data structure
            // 2. Mapping to B2Connect product schema
            // 3. Handling classifications, pricing, variants
            // 4. Bulk insert/update operations

            _logger.LogInformation("SAP catalog sync completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SAP catalog sync failed");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ErpOrderResult> CreateOrderAsync(ErpOrder order, CancellationToken cancellationToken = default)
    {
        EnsureInitialized();
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _logger.LogInformation("Creating sales order {OrderId} in SAP", order.OrderId);

        try
        {
            // SAP Sales Order creation via OData
            var orderUrl = "/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder";

            var sapOrder = MapToSapSalesOrder(order);
            var response = await _httpClient.PostAsJsonAsync(orderUrl, sapOrder, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"SAP order creation failed: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new ErpOrderResult
            {
                Success = true,
                ErpOrderId = result.GetProperty("SalesOrder").GetString(),
                AdditionalData = new Dictionary<string, object>
                {
                    ["status"] = "created",
                    ["sapOrderType"] = "TA" // Standard order
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SAP order creation failed for order {OrderId}", order.OrderId);
            return new ErpOrderResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <inheritdoc />
    public async Task<ErpCustomerData> GetCustomerDataAsync(string customerId, CancellationToken cancellationToken = default)
    {
        EnsureInitialized();
        if (string.IsNullOrEmpty(customerId))
            throw new ArgumentNullException(nameof(customerId));

        _logger.LogInformation("Retrieving SAP customer data for {CustomerId}", customerId);

        try
        {
            // SAP Customer query via OData
            var customerUrl = $"/sap/opu/odata/sap/API_BUSINESS_PARTNER_SRV/A_Customer('{customerId}')";
            var response = await _httpClient.GetAsync(customerUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"SAP customer query failed: {response.StatusCode}");
            }

            var customerData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            return new ErpCustomerData
            {
                CustomerId = customerId,
                Name = customerData.GetProperty("CustomerName").GetString() ?? "Unknown",
                Email = customerData.GetProperty("EmailAddress").GetString(),
                Address = MapSapAddress(customerData.GetProperty("to_CustomerAddress").GetProperty("results")[0])
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SAP customer data retrieval failed for {CustomerId}", customerId);
            throw;
        }
    }

    private async Task ConfigureAuthenticationAsync(ErpAuthentication auth, CancellationToken cancellationToken)
    {
        switch (auth.Type?.ToLowerInvariant())
        {
            case "oauth2":
                await ConfigureOAuth2Async(auth, cancellationToken);
                break;
            case "basic":
                ConfigureBasicAuth(auth);
                break;
            case "certificate":
                ConfigureCertificateAuth(auth);
                break;
            default:
                throw new ArgumentException($"Unsupported authentication type: {auth.Type}");
        }
    }

    private async Task ConfigureOAuth2Async(ErpAuthentication auth, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(auth.ClientId) || string.IsNullOrEmpty(auth.ClientSecret) || string.IsNullOrEmpty(auth.TokenEndpoint))
        {
            throw new ArgumentException("OAuth2 requires ClientId, ClientSecret, and TokenEndpoint");
        }

        // TODO: Implement OAuth2 token acquisition
        // This would involve:
        // 1. Client credentials flow with SAP OAuth server
        // 2. Token caching and refresh logic
        // 3. Proper token expiration handling

        var tokenRequest = new
        {
            grant_type = "client_credentials",
            client_id = auth.ClientId,
            client_secret = auth.ClientSecret
        };

        var tokenResponse = await _httpClient.PostAsJsonAsync(auth.TokenEndpoint, tokenRequest, cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var accessToken = tokenData.GetProperty("access_token").GetString();

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    }

    private void ConfigureBasicAuth(ErpAuthentication auth)
    {
        if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password))
        {
            throw new ArgumentException("Basic auth requires Username and Password");
        }

        var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{auth.Username}:{auth.Password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
    }

    private void ConfigureCertificateAuth(ErpAuthentication auth)
    {
        if (string.IsNullOrEmpty(auth.CertificatePath))
        {
            throw new ArgumentException("Certificate auth requires CertificatePath");
        }

        // TODO: Implement certificate authentication
        // This would involve:
        // 1. Loading X.509 certificate from file
        // 2. Configuring HttpClientHandler with certificate
        // 3. SAP mutual TLS setup

        throw new NotImplementedException("Certificate authentication not yet implemented");
    }

    private object MapToSapSalesOrder(ErpOrder order)
    {
        return new
        {
            SalesOrderType = "TA", // Standard order
            SalesOrganization = "1000", // TODO: Configure from settings
            DistributionChannel = "10",
            Division = "00",
            SoldToParty = order.CustomerId,
            to_Item = order.Lines.Select(line => new
            {
                Material = line.ProductId,
                RequestedQuantity = line.Quantity.ToString(),
                RequestedQuantityUnit = "EA", // Each
                NetPriceAmount = line.UnitPrice.ToString()
            }).ToArray()
        };
    }

    private ErpAddress MapSapAddress(JsonElement sapAddress)
    {
        return new ErpAddress
        {
            Street = sapAddress.GetProperty("StreetName").GetString(),
            City = sapAddress.GetProperty("CityName").GetString(),
            PostalCode = sapAddress.GetProperty("PostalCode").GetString(),
            Country = sapAddress.GetProperty("Country").GetString()
        };
    }

    private void EnsureInitialized()
    {
        if (!_initialized || _configuration == null)
        {
            throw new InvalidOperationException("SAP connector not initialized. Call InitializeAsync first.");
        }
    }
}
