// <copyright file="FakePimProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Contracts;
using B2X.ERP.Core;
using B2X.ERP.Models;

namespace B2X.ERP.Providers.Fake;

/// <summary>
/// Fake implementation of the PIM provider interface for development and testing.
/// Provides mock data for product information management operations.
/// </summary>
public sealed class FakePimProvider : IPimProvider
{
    /// <inheritdoc/>
    public string ProviderType => "fake-pim";

    /// <inheritdoc/>
    public string Version => "1.0.0-fake";

    /// <inheritdoc/>
    public bool IsConnected => true;

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
        CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
        ModifiedAt = DateTimeOffset.UtcNow,
        CategoryId = "CAT001",
        BasePrice = 99.99m,
        Currency = "EUR",
        StockQuantity = 100,
        Weight = 1.5m,
        WeightUnit = "kg"
    };

    private static PimCategory CreateFakeCategory(string categoryId) => new PimCategory
    {
        Id = categoryId,
        Name = $"Fake Category {categoryId}",
        Description = $"Description for {categoryId}",
        IsActive = true,
        SortOrder = 1,
        ProductCount = 10,
        Level = 1
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
        Id = $"{baseProductId}-VAR1",
        BaseProductId = baseProductId,
        Sku = $"{baseProductId}-VAR1",
        Name = $"Variant of {baseProductId}",
        Price = 99.99m,
        IsActive = true,
        StockQuantity = 50,
        Attributes = new[] { CreateFakeVariantAttribute() }
    };

    private static PimVariantAttribute CreateFakeVariantAttribute() => new PimVariantAttribute
    {
        Name = "Color",
        Value = "Red"
    };

    private static PimAttribute CreateFakeAttribute() => new PimAttribute
    {
        Name = "Color",
        Value = "Red",
        Unit = null,
        Group = "Appearance",
        SortOrder = 1,
        IsFilterable = true,
        IsSearchable = false
    };

    private static PimPricing CreateFakePricing() => new PimPricing
    {
        ProductId = "PROD001",
        BasePrice = 99.99m,
        Currency = "EUR",
        IncludesTax = true,
        ValidFrom = DateTimeOffset.UtcNow.AddDays(-30),
        ValidUntil = null
    };

    private static PimStock CreateFakeStock(string productId) => new PimStock
    {
        ProductId = productId,
        Quantity = 100,
        ReservedQuantity = 10,
        LastUpdated = DateTimeOffset.UtcNow,
        WarehouseId = "WH001"
    };
}
