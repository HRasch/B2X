// <copyright file="FakeErpProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Contracts;
using B2Connect.ERP.Core;
using B2Connect.ERP.Models;

namespace B2Connect.ERP.Providers.Fake;

/// <summary>
/// Fake implementation of the ERP provider interface for development and testing.
/// Provides mock data for ERP operations without requiring a real ERP system.
/// </summary>
public sealed class FakeErpProvider : IErpProvider
{
    /// <inheritdoc/>
    public string ProviderType => "fake-erp";

    /// <inheritdoc/>
    public string Version => "1.0.0-fake";

    /// <inheritdoc/>
    public bool IsConnected => true;

    // ========================================
    // Single Operations
    // ========================================

    /// <inheritdoc/>
    public Task<ProviderResult<PimProduct>> GetProductAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<PimProduct>.Success(CreateFakeProduct(productId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmCustomer>> GetCustomerAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmCustomer>.Success(CreateFakeCustomer(customerId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<ErpOrder>> CreateOrderAsync(
        OrderRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<ErpOrder>.Success(CreateFakeOrder()));
    }

    // ========================================
    // Bulk Operations
    // ========================================

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<PimProduct>>> GetProductsAsync(
        IEnumerable<string> productIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var products = productIds.Select(id => CreateFakeProduct(id)).ToArray();
        return Task.FromResult(ProviderResult<IReadOnlyList<PimProduct>>.Success(products));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmCustomer>>> GetCustomersAsync(
        IEnumerable<string> customerIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var customers = customerIds.Select(id => CreateFakeCustomer(id)).ToArray();
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmCustomer>>.Success(customers));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<BatchResult>> CreateOrdersAsync(
        IEnumerable<OrderRequest> requests,
        TenantContext context,
        CancellationToken ct = default)
    {
        var requestCount = requests.Count();
        var errors = new List<BatchError>();

        // Simulate some failures for testing
        if (requestCount > 2)
        {
            errors.Add(new BatchError("ORD003", "VALIDATION_FAILED", "Invalid customer ID"));
        }

        var batchResult = new BatchResult
        {
            SuccessCount = requestCount - errors.Count,
            FailureCount = errors.Count,
            Errors = errors,
            Duration = TimeSpan.FromMilliseconds(150)
        };

        return Task.FromResult(ProviderResult<BatchResult>.Success(batchResult));
    }

    // ========================================
    // Paged Operations
    // ========================================

    /// <inheritdoc/>
    public Task<ProviderResult<PagedResult<PimProduct>>> GetProductsPagedAsync(
        ProductFilter filter,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default)
    {
        var products = new[] { CreateFakeProduct("PROD001") };
        var result = new PagedResult<PimProduct>(products, null, 1);
        return Task.FromResult(ProviderResult<PagedResult<PimProduct>>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PagedResult<CrmCustomer>>> GetCustomersPagedAsync(
        CustomerFilter filter,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default)
    {
        var customers = new[] { CreateFakeCustomer("CUST001") };
        var result = new PagedResult<CrmCustomer>(customers, null, 1);
        return Task.FromResult(ProviderResult<PagedResult<CrmCustomer>>.Success(result));
    }

    // ========================================
    // Sync Operations
    // ========================================

    /// <inheritdoc/>
    public Task<ProviderResult<SyncResult>> SyncProductsAsync(
        SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default)
    {
        var syncResult = new SyncResult
        {
            Mode = request.Mode,
            TotalEntities = 100,
            ProcessedEntities = 100,
            FailedEntities = 0,
            Duration = TimeSpan.FromSeconds(1),
            Status = SyncStatus.Completed
        };

        return Task.FromResult(ProviderResult<SyncResult>.Success(syncResult));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<SyncResult>> SyncCustomersAsync(
        SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default)
    {
        var syncResult = new SyncResult
        {
            Mode = request.Mode,
            TotalEntities = 50,
            ProcessedEntities = 50,
            FailedEntities = 0,
            Duration = TimeSpan.FromSeconds(0.5),
            Status = SyncStatus.Completed
        };

        return Task.FromResult(ProviderResult<SyncResult>.Success(syncResult));
    }

    // ========================================
    // Health & Connection
    // ========================================

    /// <inheritdoc/>
    public Task<ProviderResult<HealthCheckResult>> HealthCheckAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        var healthResult = new HealthCheckResult
        {
            IsHealthy = true,
            Status = "Healthy",
            ResponseTime = TimeSpan.FromMilliseconds(10),
            Details = new Dictionary<string, object>
            {
                ["provider"] = ProviderType,
                ["version"] = Version,
                ["connection"] = "Mock"
            }
        };

        return Task.FromResult(ProviderResult<HealthCheckResult>.Success(healthResult));
    }

    /// <inheritdoc/>
    public Task<ProviderResult> InitializeAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult.Success());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Fake provider - no resources to dispose
    }

    // ========================================
    // Private helper methods
    // ========================================

    private static PimProduct CreateFakeProduct(string productId) => new PimProduct
    {
        Id = productId,
        Sku = productId,
        Name = $"Fake Product {productId}",
        Description = $"Description for {productId}",
        IsActive = true,
        CreatedDate = DateTimeOffset.UtcNow.AddDays(-30),
        ModifiedDate = DateTimeOffset.UtcNow,
        CategoryId = "CAT001",
        BasePrice = 99.99m,
        Currency = "EUR",
        StockQuantity = 100,
        Weight = 1.5m,
        Dimensions = new PimDimensions { Length = 10, Width = 5, Height = 2 }
    };

    private static CrmCustomer CreateFakeCustomer(string customerId) => new CrmCustomer
    {
        Id = customerId,
        Number = customerId,
        Name = $"Fake Customer {customerId}",
        Email = $"customer{customerId}@example.com",
        Phone = "+1234567890",
        IsActive = true,
        CustomerGroup = "Default",
        CreatedDate = DateTimeOffset.UtcNow.AddDays(-60),
        ModifiedDate = DateTimeOffset.UtcNow,
        BillingAddress = CreateFakeAddress(),
        ShippingAddress = CreateFakeAddress()
    };

    private static ErpOrder CreateFakeOrder() => new ErpOrder
    {
        Id = "ORD001",
        OrderNumber = "ORD001",
        Status = ErpOrderStatus.Confirmed,
        OrderDate = DateTimeOffset.UtcNow,
        TotalAmount = 99.99m,
        Currency = "EUR",
        Lines = new[] { CreateFakeOrderLine() },
        BillingAddress = CreateFakeErpOrderAddress(),
        ShippingAddress = CreateFakeErpOrderAddress()
    };

    private static ErpOrderAddress CreateFakeErpOrderAddress() => new ErpOrderAddress
    {
        Name = "John Doe",
        Company = "Test Company",
        Street1 = "123 Fake Street",
        City = "Fake City",
        PostalCode = "12345",
        CountryCode = "DE",
        Phone = "+1234567890",
        Email = "john.doe@example.com"
    };

    private static ErpOrderLine CreateFakeOrderLine() => new ErpOrderLine
    {
        LineNumber = 1,
        ProductId = "PROD001",
        Sku = "PROD001",
        Quantity = 1,
        UnitPrice = 99.99m,
        TotalPrice = 99.99m
    };

    private static CrmAddress CreateFakeAddress() => new CrmAddress
    {
        Street1 = "123 Fake Street",
        City = "Fake City",
        PostalCode = "12345",
        CountryCode = "DE"
    };
}