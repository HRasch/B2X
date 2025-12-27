namespace B2Connect.Shared.User.Integrations;

/// <summary>
/// Integration mit enventa Trade ERP
/// Mappet ERP-Kundendaten zu User Custom Properties
/// </summary>
public class EnventaTradeEerIntegration
{
    private readonly IEntityExtensionService _extensionService;
    private readonly IExtensionSchemaRepository _schemaRepository;
    private readonly ILogger<EnventaTradeEerIntegration> _logger;
    private readonly HttpClient _httpClient;

    public EnventaTradeEerIntegration(
        IEntityExtensionService extensionService,
        IExtensionSchemaRepository schemaRepository,
        ILogger<EnventaTradeEerIntegration> logger,
        HttpClient httpClient)
    {
        _extensionService = extensionService;
        _schemaRepository = schemaRepository;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Synchronisiere User mit enventa Trade ERP Kundendaten
    /// </summary>
    public async Task SyncUserWithErpAsync(
        User user,
        string erpCustomerId,
        string erpApiKey,
        string erpBaseUrl)
    {
        try
        {
            // Rufe Kundendaten aus ERP ab
            var erpCustomer = await FetchErpCustomerAsync(erpCustomerId, erpApiKey, erpBaseUrl);

            // Prüfe ob die Custom Fields im Tenant konfiguriert sind
            var schemas = await _schemaRepository.GetSchemasForEntityAsync(
                user.TenantId,
                nameof(User));

            // Sync ERP Customer Number
            var customerNumberSchema = schemas.FirstOrDefault(s => s.FieldName == "erp_customer_number");
            if (customerNumberSchema?.IsActive == true)
            {
                var isValid = await _extensionService.ValidateCustomPropertyAsync(
                    user.TenantId,
                    nameof(User),
                    "erp_customer_number",
                    erpCustomer.CustomerNumber);

                if (isValid)
                {
                    var oldValue = _extensionService.GetCustomProperty<string>(user, "erp_customer_number");
                    _extensionService.SetCustomProperty(user, "erp_customer_number", erpCustomer.CustomerNumber);

                    await _extensionService.LogCustomPropertyChangeAsync(
                        user.Id,
                        user.TenantId,
                        nameof(User),
                        "erp_customer_number",
                        oldValue,
                        erpCustomer.CustomerNumber,
                        reason: "ERP Synchronization");
                }
            }

            // Sync ERP Customer ID
            var customerIdSchema = schemas.FirstOrDefault(s => s.FieldName == "erp_customer_id");
            if (customerIdSchema?.IsActive == true)
            {
                var isValid = await _extensionService.ValidateCustomPropertyAsync(
                    user.TenantId,
                    nameof(User),
                    "erp_customer_id",
                    erpCustomer.Id);

                if (isValid)
                {
                    _extensionService.SetCustomProperty(user, "erp_customer_id", erpCustomer.Id);
                }
            }

            // Sync weitere ERP-Felder
            var warehouseSchema = schemas.FirstOrDefault(s => s.FieldName == "warehouse_code");
            if (warehouseSchema?.IsActive == true && !string.IsNullOrEmpty(erpCustomer.WarehouseCode))
            {
                _extensionService.SetCustomProperty(user, "warehouse_code", erpCustomer.WarehouseCode);
            }

            _logger.LogInformation(
                "Successfully synchronized user {UserId} with ERP customer {ErpCustomerId}",
                user.Id, erpCustomerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync user {UserId} with ERP", user.Id);
            throw;
        }
    }

    /// <summary>
    /// Setup Custom Fields für enventa Integration
    /// Wird bei Tenant-Onboarding aufgerufen
    /// </summary>
    public async Task SetupTenantIntegrationAsync(Guid tenantId)
    {
        var schemas = new List<EntityExtensionSchema>
        {
            new()
            {
                TenantId = tenantId,
                EntityTypeName = nameof(User),
                FieldName = "erp_customer_id",
                DataType = "string",
                DisplayName = "ERP Customer ID",
                Description = "Eindeutige Kundenkennung aus enventa Trade ERP",
                IsRequired = false,
                MaxLength = 50,
                IsVisibleToUsers = false,
                IntegrationSource = "enventa_trade_erp",
                MappingPath = "ERP.Customer.Id",
                IsActive = true
            },
            new()
            {
                TenantId = tenantId,
                EntityTypeName = nameof(User),
                FieldName = "erp_customer_number",
                DataType = "string",
                DisplayName = "ERP Customer Number",
                Description = "Kundennummer aus enventa Trade ERP (z.B. CUST-123456)",
                IsRequired = false,
                MaxLength = 50,
                ValidationPattern = "^[A-Z0-9-]+$", // Nur Großbuchstaben, Ziffern, Bindestrich
                IsVisibleToUsers = true,
                IntegrationSource = "enventa_trade_erp",
                MappingPath = "ERP.Customer.CustomerNumber",
                IsActive = true
            },
            new()
            {
                TenantId = tenantId,
                EntityTypeName = nameof(User),
                FieldName = "warehouse_code",
                DataType = "string",
                DisplayName = "Warehouse Code",
                Description = "Standard-Lagerort für Lieferungen",
                IsRequired = false,
                MaxLength = 20,
                ValidationPattern = "^WH-[0-9]{3}$", // WH-001, WH-002, etc.
                IsVisibleToUsers = false,
                IntegrationSource = "enventa_trade_erp",
                MappingPath = "ERP.Customer.WarehouseCode",
                IsActive = true
            },
            new()
            {
                TenantId = tenantId,
                EntityTypeName = nameof(User),
                FieldName = "credit_limit",
                DataType = "decimal",
                DisplayName = "Credit Limit",
                Description = "Kreditlimit aus ERP",
                IsRequired = false,
                IsVisibleToUsers = false,
                IntegrationSource = "enventa_trade_erp",
                MappingPath = "ERP.Customer.CreditLimit",
                IsActive = true
            },
            new()
            {
                TenantId = tenantId,
                EntityTypeName = nameof(User),
                FieldName = "customer_segment",
                DataType = "string",
                DisplayName = "Customer Segment",
                Description = "Kundensegment (z.B. Premium, Standard, Budget)",
                IsRequired = false,
                MaxLength = 50,
                IsVisibleToUsers = true,
                IntegrationSource = "enventa_trade_erp",
                MappingPath = "ERP.Customer.Segment",
                IsActive = true
            }
        };

        foreach (var schema in schemas)
        {
            await _schemaRepository.AddAsync(schema);
        }

        await _schemaRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Set up {Count} enventa Trade ERP integration fields for tenant {TenantId}",
            schemas.Count, tenantId);
    }

    private async Task<ErpCustomer> FetchErpCustomerAsync(
        string customerId,
        string apiKey,
        string baseUrl)
    {
        // Bearer Token
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.GetAsync($"{baseUrl}/api/customers/{customerId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var customer = JsonSerializer.Deserialize<ErpCustomer>(json)
            ?? throw new InvalidOperationException("Failed to deserialize ERP customer");

        return customer;
    }
}

/// <summary>
/// DTO für enventa Trade ERP Kundendaten
/// </summary>
public class ErpCustomer
{
    public string Id { get; set; } = "";
    public string CustomerNumber { get; set; } = "";
    public string Name { get; set; } = "";
    public string? WarehouseCode { get; set; }
    public decimal CreditLimit { get; set; }
    public string? Segment { get; set; }
}
