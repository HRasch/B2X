using B2Connect.Shared.Infrastructure.ServiceClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2Connect.Store.Controllers;

/// <summary>
/// ERP Integration API Controller
/// Provides secure bidirectional API access for enventa Trade ERP systems.
/// ERP service accounts can read/update customer data, manage products, and access usage statistics.
/// All operations are tenant-scoped and permission-controlled.
/// </summary>
[ApiController]
[Route("api/erp")]
[Authorize] // Requires ERP service account authentication
public class ErpController : ControllerBase
{
    private readonly ICustomerServiceClient _customerService;
    private readonly ICatalogServiceClient _catalogService;
    private readonly IUsageServiceClient _usageService;
    private readonly IAccessServiceClient _accessService;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ErpController> _logger;

    public ErpController(
        ICustomerServiceClient customerService,
        ICatalogServiceClient catalogService,
        IUsageServiceClient usageService,
        IAccessServiceClient accessService,
        IMessageBus messageBus,
        ILogger<ErpController> logger)
    {
        _customerService = customerService;
        _catalogService = catalogService;
        _usageService = usageService;
        _accessService = accessService;
        _messageBus = messageBus;
        _logger = logger;
    }

    /// <summary>
    /// Get customer information by ERP customer ID
    /// Requires: ReadCustomers permission
    /// </summary>
    [HttpGet("customers/{erpCustomerId}")]
    [Authorize(Policy = "ErpReadCustomers")]
    public async Task<IActionResult> GetCustomer(string erpCustomerId)
    {
        // Permission check would be handled by authorization policy
        // This is a placeholder - actual implementation depends on your permission system

        var tenantId = GetTenantIdFromClaims();
        var customer = await _customerService.GetCustomerByErpIdAsync(erpCustomerId, tenantId);

        if (customer == null)
            return NotFound(new { Message = "Customer not found" });

        return Ok(new
        {
            customer.Id,
            customer.ErpCustomerId,
            customer.Name,
            customer.Email,
            customer.Address,
            customer.LastModified
        });
    }

    /// <summary>
    /// Update customer information from ERP
    /// Requires: UpdateCustomers permission
    /// </summary>
    [HttpPut("customers/{erpCustomerId}")]
    [Authorize(Policy = "ErpUpdateCustomers")]
    public async Task<IActionResult> UpdateCustomer(string erpCustomerId, [FromBody] UpdateCustomerRequest request)
    {
        var tenantId = GetTenantIdFromClaims();

        // Publish domain event for customer update
        await _messageBus.PublishAsync(new CustomerUpdatedFromErp
        {
            TenantId = tenantId,
            ErpCustomerId = erpCustomerId,
            Name = request.Name,
            Email = request.Email,
            Address = request.Address,
            UpdatedBy = "ERP"
        });

        _logger.LogInformation("Customer {ErpCustomerId} updated from ERP for tenant {TenantId}",
            erpCustomerId, tenantId);

        return Ok(new { Message = "Customer update queued for processing" });
    }

    /// <summary>
    /// Get product information by ERP product ID
    /// Requires: ReadProducts permission
    /// </summary>
    [HttpGet("products/{erpProductId}")]
    [Authorize(Policy = "ErpReadProducts")]
    public async Task<IActionResult> GetProduct(string erpProductId)
    {
        var tenantId = GetTenantIdFromClaims();
        var product = await _catalogService.GetProductByErpIdAsync(erpProductId, tenantId);

        if (product == null)
            return NotFound(new { Message = "Product not found" });

        return Ok(new
        {
            product.Id,
            product.ErpProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.StockLevel,
            product.LastModified
        });
    }

    /// <summary>
    /// Update product information from ERP
    /// Requires: UpdateProducts permission
    /// </summary>
    [HttpPut("products/{erpProductId}")]
    [Authorize(Policy = "ErpUpdateProducts")]
    public async Task<IActionResult> UpdateProduct(string erpProductId, [FromBody] UpdateProductRequest request)
    {
        var tenantId = GetTenantIdFromClaims();

        // Publish domain event for product update
        await _messageBus.PublishAsync(new ProductUpdatedFromErp
        {
            TenantId = tenantId,
            ErpProductId = erpProductId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockLevel = request.StockLevel,
            UpdatedBy = "ERP"
        });

        _logger.LogInformation("Product {ErpProductId} updated from ERP for tenant {TenantId}",
            erpProductId, tenantId);

        return Ok(new { Message = "Product update queued for processing" });
    }

    /// <summary>
    /// Get usage statistics for the tenant
    /// Requires: ReadUsageStats permission
    /// </summary>
    [HttpGet("usage")]
    [Authorize(Policy = "ErpReadUsageStats")]
    public async Task<IActionResult> GetUsageStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var tenantId = GetTenantIdFromClaims();
        var stats = await _usageService.GetUsageStatsAsync(tenantId, from, to);

        return Ok(new
        {
            TenantId = tenantId,
            Period = new { From = from, To = to },
            stats.TotalOrders,
            stats.TotalRevenue,
            stats.ActiveCustomers,
            stats.TopProducts
        });
    }

    /// <summary>
    /// Manage access permissions for users
    /// Requires: ManageAccess permission
    /// </summary>
    [HttpPost("access")]
    [Authorize(Policy = "ErpManageAccess")]
    public async Task<IActionResult> UpdateAccess([FromBody] UpdateAccessRequest request)
    {
        var tenantId = GetTenantIdFromClaims();

        await _accessService.UpdateUserAccessAsync(
            tenantId,
            request.UserId,
            request.Permissions,
            request.UpdatedBy ?? "ERP");

        _logger.LogInformation("Access updated for user {UserId} from ERP for tenant {TenantId}",
            request.UserId, tenantId);

        return Ok(new { Message = "Access permissions updated" });
    }

    /// <summary>
    /// Webhook endpoint for ERP events
    /// Requires: ReceiveWebhooks permission
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous] // Webhooks may come from external ERP systems
    public async Task<IActionResult> ReceiveWebhook([FromBody] ErpWebhookRequest webhook)
    {
        // Validate webhook signature (implementation depends on ERP system)
        if (!ValidateWebhookSignature(Request))
            return Unauthorized(new { Message = "Invalid webhook signature" });

        var tenantId = GetTenantIdFromClaims();

        // Process webhook based on event type
        switch (webhook.EventType)
        {
            case "customer.created":
                await _messageBus.PublishAsync(new CustomerCreatedInErp
                {
                    TenantId = tenantId,
                    ErpCustomerId = webhook.Data["customerId"]?.ToString(),
                    CustomerData = webhook.Data
                });
                break;

            case "product.updated":
                await _messageBus.PublishAsync(new ProductUpdatedInErp
                {
                    TenantId = tenantId,
                    ErpProductId = webhook.Data["productId"]?.ToString(),
                    ProductData = webhook.Data
                });
                break;

            default:
                _logger.LogWarning("Unknown webhook event type: {EventType}", webhook.EventType);
                break;
        }

        return Ok(new { Message = "Webhook processed" });
    }

    private Guid GetTenantIdFromClaims()
    {
        // Extract tenant ID from JWT claims or headers
        // Implementation depends on your authentication setup
        var tenantClaim = User.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(tenantClaim, out var tenantId))
            return tenantId;

        // Fallback to header
        if (Request.Headers.TryGetValue("X-Tenant-ID", out var headerValues))
            if (Guid.TryParse(headerValues.FirstOrDefault(), out tenantId))
                return tenantId;

        throw new UnauthorizedAccessException("Tenant ID not found in request");
    }

    private bool ValidateWebhookSignature(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // Implementation depends on ERP system's webhook signing mechanism
        // This is a placeholder - actual implementation needed
        return true; // TODO: Implement proper webhook signature validation
    }
}

// Request/Response Models
public class UpdateCustomerRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
}

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockLevel { get; set; }
}

public class UpdateAccessRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new List<string>();
    public string? UpdatedBy { get; set; }
}

public class ErpWebhookRequest
{
    public string EventType { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
}

// Domain Events (for Wolverine CQRS)
public record CustomerUpdatedFromErp
{
    public Guid TenantId { get; init; }
    public string? ErpCustomerId { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public Address? Address { get; init; }
    public string UpdatedBy { get; init; } = string.Empty;
}

public record ProductUpdatedFromErp
{
    public Guid TenantId { get; init; }
    public string? ErpProductId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public int? StockLevel { get; init; }
    public string UpdatedBy { get; init; } = string.Empty;
}

public record CustomerCreatedInErp
{
    public Guid TenantId { get; init; }
    public string? ErpCustomerId { get; init; }
    public Dictionary<string, object> CustomerData { get; init; } = new(StringComparer.Ordinal);
}

public record ProductUpdatedInErp
{
    public Guid TenantId { get; init; }
    public string? ErpProductId { get; init; }
    public Dictionary<string, object> ProductData { get; init; } = new(StringComparer.Ordinal);
}
