// <copyright file="IPimProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Models;

namespace B2Connect.ERP.Contracts;

/// <summary>
/// Product Information Management (PIM) provider interface.
/// Specialized interface for product catalog operations.
/// </summary>
public interface IPimProvider
{
    /// <summary>
    /// Gets detailed product information.
    /// </summary>
    Task<ProviderResult<PimProduct>> GetProductDetailsAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all product categories.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<PimCategory>>> GetCategoriesAsync(
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the category hierarchy/tree.
    /// </summary>
    Task<ProviderResult<PimCategoryTree>> GetCategoryTreeAsync(
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the full catalog with paging support.
    /// Optimized for large datasets.
    /// </summary>
    Task<ProviderResult<PagedResult<PimProduct>>> GetCatalogAsync(
        CatalogFilter filter,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets product variants for a base product.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<PimProductVariant>>> GetProductVariantsAsync(
        string baseProductId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets product attributes/specifications.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<PimAttribute>>> GetProductAttributesAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets product pricing information.
    /// </summary>
    Task<ProviderResult<PimPricing>> GetProductPricingAsync(
        string productId,
        string? customerId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets stock/inventory information for products.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<PimStock>>> GetStockLevelsAsync(
        IEnumerable<string> productIds,
        TenantContext context,
        CancellationToken ct = default);
}

/// <summary>
/// Filter for catalog queries.
/// </summary>
public sealed record CatalogFilter
{
    public string? CategoryId { get; init; }
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public bool? InStock { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public IReadOnlyList<string>? Attributes { get; init; }
    public DateTimeOffset? ModifiedSince { get; init; }
    public CatalogSortOrder SortBy { get; init; } = CatalogSortOrder.Default;
}

/// <summary>
/// Sort order for catalog queries.
/// </summary>
public enum CatalogSortOrder
{
    Default,
    NameAscending,
    NameDescending,
    PriceAscending,
    PriceDescending,
    Newest,
    Relevance
}
