// <copyright file="PimModels.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Models;

/// <summary>
/// Product information from PIM system.
/// </summary>
public sealed record PimProduct
{
    public required string Id { get; init; }
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ShortDescription { get; init; }
    public string? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public string? ManufacturerId { get; init; }
    public string? ManufacturerName { get; init; }
    public string? Brand { get; init; }
    public decimal? BasePrice { get; init; }
    public string? Currency { get; init; }
    public string? TaxClass { get; init; }
    public decimal? Weight { get; init; }
    public string? WeightUnit { get; init; }
    public bool IsActive { get; init; }
    public bool IsInStock { get; init; }
    public int? StockQuantity { get; init; }
    public IReadOnlyList<PimAttribute>? Attributes { get; init; }
    public IReadOnlyList<PimImage>? Images { get; init; }
    public IReadOnlyList<string>? RelatedProductIds { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Product variant information.
/// </summary>
public sealed record PimProductVariant
{
    public required string Id { get; init; }
    public required string BaseProductId { get; init; }
    public required string Sku { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public bool IsActive { get; init; }
    public int? StockQuantity { get; init; }
    public IReadOnlyList<PimVariantAttribute>? Attributes { get; init; }
}

/// <summary>
/// Variant-specific attribute.
/// </summary>
public sealed record PimVariantAttribute
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}

/// <summary>
/// Product attribute/specification.
/// </summary>
public sealed record PimAttribute
{
    public required string Name { get; init; }
    public required string Value { get; init; }
    public string? Unit { get; init; }
    public string? Group { get; init; }
    public int SortOrder { get; init; }
    public bool IsFilterable { get; init; }
    public bool IsSearchable { get; init; }
}

/// <summary>
/// Product image.
/// </summary>
public sealed record PimImage
{
    public required string Url { get; init; }
    public string? Alt { get; init; }
    public string? Title { get; init; }
    public int SortOrder { get; init; }
    public bool IsPrimary { get; init; }
    public string? Type { get; init; }
}

/// <summary>
/// Product category.
/// </summary>
public sealed record PimCategory
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ParentId { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public int ProductCount { get; init; }
    public int Level { get; init; }
    public string? Path { get; init; }
}

/// <summary>
/// Category tree structure.
/// </summary>
public sealed record PimCategoryTree
{
    public required IReadOnlyList<PimCategoryNode> RootCategories { get; init; }
    public int TotalCategories { get; init; }
    public int MaxDepth { get; init; }
}

/// <summary>
/// Category tree node with children.
/// </summary>
public sealed record PimCategoryNode
{
    public required PimCategory Category { get; init; }
    public IReadOnlyList<PimCategoryNode>? Children { get; init; }
}

/// <summary>
/// Product pricing information.
/// </summary>
public sealed record PimPricing
{
    public required string ProductId { get; init; }
    public required decimal BasePrice { get; init; }
    public decimal? SalePrice { get; init; }
    public decimal? CustomerPrice { get; init; }
    public required string Currency { get; init; }
    public decimal? TaxRate { get; init; }
    public bool IncludesTax { get; init; }
    public IReadOnlyList<PimPriceTier>? TierPrices { get; init; }
    public DateTimeOffset? ValidFrom { get; init; }
    public DateTimeOffset? ValidUntil { get; init; }
}

/// <summary>
/// Tier pricing (quantity discounts).
/// </summary>
public sealed record PimPriceTier
{
    public int MinQuantity { get; init; }
    public int? MaxQuantity { get; init; }
    public decimal Price { get; init; }
    public decimal? DiscountPercent { get; init; }
}

/// <summary>
/// Stock/inventory information.
/// </summary>
public sealed record PimStock
{
    public required string ProductId { get; init; }
    public required int Quantity { get; init; }
    public int? ReservedQuantity { get; init; }
    public int AvailableQuantity => Quantity - (ReservedQuantity ?? 0);
    public bool IsInStock => AvailableQuantity > 0;
    public string? WarehouseId { get; init; }
    public string? WarehouseName { get; init; }
    public DateTimeOffset? ExpectedRestockDate { get; init; }
    public DateTimeOffset LastUpdated { get; init; }
}
