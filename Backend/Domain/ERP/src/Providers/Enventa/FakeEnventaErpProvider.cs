// <copyright file="FakeEnventaErpProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Contracts;
using B2Connect.ERP.Core;
using B2Connect.ERP.Models;
using B2Connect.ERP.Services;

namespace B2Connect.ERP.Providers.Enventa;

/// <summary>
/// Fake implementation of all ERP provider interfaces for Mac development.
/// Real implementation will be built on Windows with .NET Framework 4.8.
/// </summary>
public sealed class FakeEnventaErpProvider : IErpProvider, IPimProvider, ICrmProvider
{
    /// <inheritdoc/>
    public string ProviderType => "enventa-fake";

    /// <inheritdoc/>
    public string Version => "1.0.0-fake";

    /// <inheritdoc/>
    public bool IsConnected => true;

    // PIM Provider methods
    /// <inheritdoc/>
    public Task<ProviderResult<PimProduct>> GetProductDetailsAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<PimProduct>.Success(CreateFakeProduct(productId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<PimCategory>>> GetCategoriesAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        var categories = new[] { CreateFakeCategory("CAT001") };
        return Task.FromResult(ProviderResult<IReadOnlyList<PimCategory>>.Success(categories));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PimCategoryTree>> GetCategoryTreeAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<PimCategoryTree>.Success(CreateFakeCategoryTree()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PagedResult<PimProduct>>> GetCatalogAsync(
        CatalogFilter filter,
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
    public Task<ProviderResult<IReadOnlyList<PimProductVariant>>> GetProductVariantsAsync(
        string baseProductId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var variants = new[] { CreateFakeVariant(baseProductId) };
        return Task.FromResult(ProviderResult<IReadOnlyList<PimProductVariant>>.Success(variants));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<PimAttribute>>> GetProductAttributesAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var attributes = new[] { CreateFakeAttribute() };
        return Task.FromResult(ProviderResult<IReadOnlyList<PimAttribute>>.Success(attributes));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PimPricing>> GetProductPricingAsync(
        string productId,
        string? customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<PimPricing>.Success(CreateFakePricing()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<PimStock>>> GetStockLevelsAsync(
        IEnumerable<string> productIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var stocks = productIds.Select(id => CreateFakeStock(id)).ToArray();
        return Task.FromResult(ProviderResult<IReadOnlyList<PimStock>>.Success(stocks));
    }

    // CRM Provider methods
    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> GetContactAsync(
        string contactId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(contactId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmContact>>> GetContactsAsync(
        IEnumerable<string> contactIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var contacts = contactIds.Select(id => CreateFakeContact(id)).ToArray();
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmContact>>.Success(contacts));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmActivity>>> GetActivitiesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var activities = new[] { CreateFakeActivity(customerId) };
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmActivity>>.Success(activities));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyDictionary<string, IReadOnlyList<CrmActivity>>>> GetActivitiesBatchAsync(
        IEnumerable<string> customerIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var result = customerIds.ToDictionary(
            id => id,
            id => (IReadOnlyList<CrmActivity>)new[] { CreateFakeActivity(id) });
        return Task.FromResult(ProviderResult<IReadOnlyDictionary<string, IReadOnlyList<CrmActivity>>>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PagedResult<CrmOrderHistory>>> GetOrderHistoryAsync(
        string customerId,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default)
    {
        var orders = new[] { CreateFakeOrderHistory(customerId) };
        var result = new PagedResult<CrmOrderHistory>(orders, null, 1);
        return Task.FromResult(ProviderResult<PagedResult<CrmOrderHistory>>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmAddress>>> GetAddressesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var addresses = new[] { CreateFakeAddress() };
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmAddress>>.Success(addresses));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmPricing>> GetCustomerPricingAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmPricing>.Success(CreateFakeCustomerPricing()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> CreateContactAsync(
        string customerId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(Guid.NewGuid().ToString())));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> UpdateContactAsync(
        string contactId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(contactId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmActivity>> LogActivityAsync(
        CrmActivityRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmActivity>.Success(CreateFakeActivity(request.CustomerId)));
    }

    // ERP Provider methods
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
        return Task.FromResult(ProviderResult<BatchResult>.Success(new BatchResult()));
    }

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

    /// <inheritdoc/>
    public Task<ProviderResult<Core.SyncResult>> SyncProductsAsync(
        Contracts.SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<Core.SyncResult>.Success(CreateFakeCoreSyncResult()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<Core.SyncResult>> SyncCustomersAsync(
        Contracts.SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<Core.SyncResult>.Success(CreateFakeCoreSyncResult()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<HealthCheckResult>> HealthCheckAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        var result = new HealthCheckResult
        {
            IsHealthy = true,
            Status = "Fake provider healthy",
            ResponseTime = TimeSpan.Zero,
            Details = new Dictionary<string, object> { ["message"] = "This is a fake implementation for Mac development" }
        };
        return Task.FromResult(ProviderResult<HealthCheckResult>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult> InitializeAsync(
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult.Success());
    }

    // Helper methods to create fake data
    private static Services.SyncResult CreateFakeSyncResult() => new Services.SyncResult(
        Guid.NewGuid().ToString(),
        SyncOperationState.Completed,
        100L,
        0L,
        DateTimeOffset.UtcNow,
        Array.Empty<string>());

    private static Core.SyncResult CreateFakeCoreSyncResult() => new Core.SyncResult
    {
        Created = 50,
        Updated = 30,
        Deleted = 5,
        Skipped = 10,
        Failed = 0,
        Duration = TimeSpan.FromSeconds(5),
        StartedAt = DateTimeOffset.UtcNow.AddSeconds(-5)
    };

    private static PimProduct CreateFakeProduct(string productId) => new PimProduct
    {
        Id = productId,
        Sku = productId,
        Name = $"Fake Product {productId}",
        Description = $"Description for {productId}",
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static PimCategory CreateFakeCategory(string categoryId) => new PimCategory
    {
        Id = categoryId,
        Name = $"Fake Category {categoryId}",
        Description = $"Description for {categoryId}",
        IsActive = true,
        SortOrder = 0
    };

    private static PimCategoryTree CreateFakeCategoryTree() => new PimCategoryTree
    {
        RootCategories = new[] { CreateFakeCategoryNode("CAT001") },
        TotalCategories = 1,
        MaxDepth = 1
    };

    private static PimCategoryNode CreateFakeCategoryNode(string categoryId) => new PimCategoryNode
    {
        Category = CreateFakeCategory(categoryId),
        Children = null
    };

    private static PimProductVariant CreateFakeVariant(string baseProductId) => new PimProductVariant
    {
        Id = $"{baseProductId}-VAR",
        BaseProductId = baseProductId,
        Sku = $"{baseProductId}-VAR",
        Name = $"Variant of {baseProductId}",
        IsActive = true
    };

    private static PimAttribute CreateFakeAttribute() => new PimAttribute
    {
        Name = "Fake Attribute",
        Value = "Fake Value",
        SortOrder = 0,
        IsFilterable = true,
        IsSearchable = true
    };

    private static PimPricing CreateFakePricing() => new PimPricing
    {
        ProductId = "PROD001",
        BasePrice = 99.99m,
        Currency = "EUR",
        IncludesTax = true
    };

    private static PimStock CreateFakeStock(string productId) => new PimStock
    {
        ProductId = productId,
        Quantity = 100,
        WarehouseId = "MAIN",
        LastUpdated = DateTimeOffset.UtcNow
    };

    private static CrmContact CreateFakeContact(string contactId) => new CrmContact
    {
        Id = contactId,
        CustomerId = "CUST001",
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        Phone = "+1234567890",
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static CrmActivity CreateFakeActivity(string customerId) => new CrmActivity
    {
        Id = Guid.NewGuid().ToString(),
        CustomerId = customerId,
        Type = CrmActivityType.Note,
        Subject = "Fake Activity",
        Description = "This is a fake activity for testing",
        Status = CrmActivityStatus.Completed,
        Priority = CrmActivityPriority.Normal,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static CrmOrderHistory CreateFakeOrderHistory(string customerId) => new CrmOrderHistory
    {
        OrderId = Guid.NewGuid().ToString(),
        OrderNumber = "ORD001",
        CustomerId = customerId,
        OrderDate = DateTimeOffset.UtcNow,
        TotalAmount = 99.99m,
        Currency = "EUR",
        Status = "Completed",
        ItemCount = 1,
        Items = new[] { CreateFakeOrderHistoryItem() }
    };

    private static CrmOrderHistoryItem CreateFakeOrderHistoryItem() => new CrmOrderHistoryItem
    {
        ProductId = "PROD001",
        Sku = "PROD001",
        Name = "Fake Product",
        Quantity = 1,
        UnitPrice = 99.99m,
        TotalPrice = 99.99m
    };

    private static CrmAddress CreateFakeAddress() => new CrmAddress
    {
        Id = Guid.NewGuid().ToString(),
        Type = CrmAddressType.Billing,
        Street1 = "123 Fake Street",
        City = "Fake City",
        PostalCode = "12345",
        CountryCode = "DE",
        IsActive = true
    };

    private static CrmPricing CreateFakeCustomerPricing() => new CrmPricing
    {
        CustomerId = "CUST001",
        ProductId = "PROD001",
        Price = 89.99m,
        Currency = "EUR"
    };

    private static CrmCustomer CreateFakeCustomer(string customerId) => new CrmCustomer
    {
        Id = customerId,
        CustomerNumber = customerId,
        Name = $"Fake Customer {customerId}",
        CustomerType = CrmCustomerType.Business,
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static ErpOrder CreateFakeOrder() => new ErpOrder
    {
        Id = Guid.NewGuid().ToString(),
        CustomerId = "CUST001",
        CustomerNumber = "CUST001",
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

    /// <inheritdoc/>
    public void Dispose()
    {
        // Fake provider - no resources to dispose
    }
}