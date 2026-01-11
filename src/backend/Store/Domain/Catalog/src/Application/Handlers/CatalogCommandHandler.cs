using B2X.Catalog.Application.Commands;
using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Models;
using B2X.Types.DTOs;
using Wolverine;

namespace B2X.Catalog.Application.Handlers;

/// <summary>
/// Unified command handler for all catalog operations
/// Consolidates Category, Product, and Variant command handling
/// </summary>
public class CatalogCommandHandler
{
    // Category command handlers
    public static async Task<Category> Handle(CreateCategoryCommand command, ICatalogRepository repository)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Description = command.Description,
            Slug = command.Slug,
            ParentId = command.ParentId,
            ImageUrl = command.ImageUrl,
            Icon = command.Icon,
            DisplayOrder = command.DisplayOrder,
            MetaTitle = command.MetaTitle,
            MetaDescription = command.MetaDescription,
            IsActive = command.IsActive,
            IsVisible = command.IsVisible,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddCategoryAsync(category);
        return category;
    }

    public static async Task<Category> Handle(UpdateCategoryCommand command, ICatalogRepository repository)
    {
        var category = await repository.GetCategoryByIdAsync(command.Id, command.TenantId);
        if (category == null)
            throw new KeyNotFoundException($"Category {command.Id} not found");

        category.Name = command.Name;
        category.Description = command.Description;
        category.Slug = command.Slug;
        category.ParentId = command.ParentId;
        category.ImageUrl = command.ImageUrl;
        category.Icon = command.Icon;
        category.DisplayOrder = command.DisplayOrder;
        category.MetaTitle = command.MetaTitle;
        category.MetaDescription = command.MetaDescription;
        category.IsActive = command.IsActive;
        category.IsVisible = command.IsVisible;
        category.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateCategoryAsync(category);
        return category;
    }

    public static async Task Handle(DeleteCategoryCommand command, ICatalogRepository repository)
    {
        await repository.DeleteCategoryAsync(command.Id, command.TenantId);
    }

    // Variant command handlers
    public static async Task<Variant> Handle(CreateVariantCommand command, ICatalogRepository repository)
    {
        var variant = new Variant
        {
            Id = Guid.NewGuid(),
            ProductId = command.Variant.ProductId,
            Sku = command.Variant.Sku,
            Name = command.Variant.Name,
            Description = command.Variant.Description,
            Attributes = command.Variant.Attributes,
            Price = command.Variant.Price,
            CompareAtPrice = command.Variant.CompareAtPrice,
            StockQuantity = command.Variant.StockQuantity,
            TrackInventory = command.Variant.TrackInventory,
            AllowBackorders = command.Variant.AllowBackorders,
            ImageUrls = command.Variant.ImageUrls,
            PrimaryImageUrl = command.Variant.PrimaryImageUrl,
            IsActive = command.Variant.IsActive,
            DisplayOrder = command.Variant.DisplayOrder,
            Barcode = command.Variant.Barcode,
            Weight = command.Variant.Weight?.ToString(),
            Dimensions = command.Variant.Dimensions?.ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddVariantAsync(variant);
        return variant;
    }

    public static async Task<Variant> Handle(UpdateVariantCommand command, ICatalogRepository repository)
    {
        var variant = await repository.GetVariantByIdAsync(command.Id, command.TenantId);
        if (variant == null)
            throw new KeyNotFoundException($"Variant {command.Id} not found");

        variant.Sku = command.Variant.Sku;
        variant.Name = command.Variant.Name;
        variant.Description = command.Variant.Description;
        variant.Attributes = command.Variant.Attributes;
        variant.Price = command.Variant.Price;
        variant.CompareAtPrice = command.Variant.CompareAtPrice;
        variant.StockQuantity = command.Variant.StockQuantity;
        variant.TrackInventory = command.Variant.TrackInventory;
        variant.AllowBackorders = command.Variant.AllowBackorders;
        variant.ImageUrls = command.Variant.ImageUrls;
        variant.PrimaryImageUrl = command.Variant.PrimaryImageUrl;
        variant.IsActive = command.Variant.IsActive;
        variant.DisplayOrder = command.Variant.DisplayOrder;
        variant.Barcode = command.Variant.Barcode;
        variant.Weight = command.Variant.Weight?.ToString();
        variant.Dimensions = command.Variant.Dimensions?.ToString();
        variant.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateVariantAsync(variant);
        return variant;
    }

    public static async Task Handle(DeleteVariantCommand command, ICatalogRepository repository)
    {
        await repository.DeleteVariantAsync(command.Id, command.TenantId);
    }

    public static async Task Handle(UpdateVariantStockCommand command, ICatalogRepository repository)
    {
        var variant = await repository.GetVariantByIdAsync(command.Id, command.TenantId);
        if (variant == null)
            throw new KeyNotFoundException($"Variant {command.Id} not found");

        variant.StockQuantity = command.NewStockQuantity;
        variant.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateVariantAsync(variant);
    }

    // Product command handlers
    public static async Task<Product> Handle(CreateProductCommand command, ICatalogRepository repository)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Sku = command.Sku,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            DiscountPrice = command.DiscountPrice,
            StockQuantity = command.StockQuantity,
            IsActive = command.IsActive,
            BrandName = command.BrandName,
            Tags = command.Tags,
            Barcode = command.Barcode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        product.SetCategories(command.CategoryIds);

        await repository.AddProductAsync(product);
        return product;
    }

    public static async Task<Product> Handle(UpdateProductCommand command, ICatalogRepository repository)
    {
        var product = await repository.GetProductByIdAsync(command.Id, command.TenantId);
        if (product == null)
            throw new KeyNotFoundException($"Product {command.Id} not found");

        // Update only non-null fields (partial update)
        if (!string.IsNullOrEmpty(command.Sku))
            product.Sku = command.Sku;

        if (!string.IsNullOrEmpty(command.Name))
            product.Name = command.Name;

        if (!string.IsNullOrEmpty(command.Description))
            product.Description = command.Description;

        if (command.Price.HasValue)
            product.Price = command.Price.Value;

        if (command.DiscountPrice.HasValue)
            product.DiscountPrice = command.DiscountPrice;

        if (command.StockQuantity.HasValue)
            product.StockQuantity = command.StockQuantity.Value;

        if (command.IsActive.HasValue)
            product.IsActive = command.IsActive.Value;

        if (!string.IsNullOrEmpty(command.BrandName))
            product.BrandName = command.BrandName;

        if (command.Tags != null && command.Tags.Any())
            product.Tags = command.Tags;

        if (!string.IsNullOrEmpty(command.Barcode))
            product.Barcode = command.Barcode;

        if (command.CategoryIds != null && command.CategoryIds.Any())
            product.SetCategories(command.CategoryIds);

        product.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateProductAsync(product);
        return product;
    }

    public static async Task Handle(CategorizeProductCommand command, ICatalogRepository repository)
    {
        var product = await repository.GetProductByIdAsync(command.ProductId, command.TenantId);
        if (product == null)
            throw new KeyNotFoundException($"Product {command.ProductId} not found");

        product.AddCategory(command.CategoryId);
        await repository.UpdateProductAsync(product);
    }

    public static async Task Handle(RemoveProductFromCategoryCommand command, ICatalogRepository repository)
    {
        var product = await repository.GetProductByIdAsync(command.ProductId, command.TenantId);
        if (product == null)
            throw new KeyNotFoundException($"Product {command.ProductId} not found");

        product.RemoveCategory(command.CategoryId);
        await repository.UpdateProductAsync(product);
    }

    // Category query handlers
    public static async Task<Category> Handle(GetCategoryByIdQuery query, ICatalogRepository repository)
    {
        var category = await repository.GetCategoryByIdAsync(query.Id, query.TenantId);
        if (category == null)
            throw new KeyNotFoundException($"Category {query.Id} not found");
        return category;
    }

    public static async Task<Category> Handle(GetCategoryBySlugQuery query, ICatalogRepository repository)
    {
        var category = await repository.GetCategoryBySlugAsync(query.Slug, query.TenantId);
        if (category == null)
            throw new KeyNotFoundException($"Category with slug '{query.Slug}' not found");
        return category;
    }

    public static async Task<PagedResult<Category>> Handle(GetCategoriesQuery query, ICatalogRepository repository)
    {
        return await repository.GetCategoriesPagedAsync(
            query.TenantId,
            query.SearchTerm,
            query.ParentId,
            query.IsActive,
            query.Page,
            query.PageSize);
    }

    public static async Task<List<Category>> Handle(GetCategoryTreeQuery query, ICatalogRepository repository)
    {
        return await repository.GetCategoryHierarchyAsync(query.TenantId);
    }

    // Variant query handlers
    public static async Task<Variant> Handle(GetVariantByIdQuery query, ICatalogRepository repository)
    {
        var variant = await repository.GetVariantByIdAsync(query.Id, query.TenantId);
        if (variant == null)
            throw new KeyNotFoundException($"Variant {query.Id} not found");
        return variant;
    }

    public static async Task<Variant> Handle(GetVariantBySkuQuery query, ICatalogRepository repository)
    {
        var variant = await repository.GetVariantBySkuAsync(query.Sku, query.TenantId);
        if (variant == null)
            throw new KeyNotFoundException($"Variant with SKU {query.Sku} not found");
        return variant;
    }

    public static async Task<PagedResult<Variant>> Handle(GetVariantsByProductQuery query, ICatalogRepository repository)
    {
        var (items, totalCount) = await repository.GetVariantsByProductIdPagedAsync(query.ProductId, query.TenantId, query.Page, query.PageSize);
        return new PagedResult<Variant>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = query.Page,
            PageSize = query.PageSize
        };
    }

    public static async Task<PagedResult<Variant>> Handle(GetVariantsQuery query, ICatalogRepository repository)
    {
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var (items, totalCount) = await repository.SearchVariantsAsync(query.TenantId, query.SearchTerm, query.Page, query.PageSize);
            return new PagedResult<Variant>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = query.Page,
                PageSize = query.PageSize
            };
        }
        else
        {
            var (items, totalCount) = await repository.GetVariantsPagedAsync(query.TenantId, query.Page, query.PageSize);
            return new PagedResult<Variant>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = query.Page,
                PageSize = query.PageSize
            };
        }
    }

    public static async Task<PagedResult<Variant>> Handle(SearchVariantsQuery query, ICatalogRepository repository)
    {
        var (items, totalCount) = await repository.SearchVariantsAsync(query.TenantId, query.Query, query.Page, query.PageSize);
        return new PagedResult<Variant>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            PageNumber = query.Page,
            PageSize = query.PageSize
        };
    }

    // Product query handlers
    public static async Task<Product?> Handle(GetProductByIdQuery query, ICatalogRepository repository)
    {
        return await repository.GetProductByIdAsync(query.Id, query.TenantId);
    }

    public static async Task<IEnumerable<Product>> Handle(GetProductsByTenantQuery query, ICatalogRepository repository)
    {
        return await repository.GetProductsByTenantAsync(query.TenantId, query.Page, query.PageSize);
    }

    public static async Task<IEnumerable<Product>> Handle(GetProductsByCategoryQuery query, ICatalogRepository repository)
    {
        return await repository.GetProductsByCategoryAsync(query.CategoryId, query.TenantId, query.Page, query.PageSize);
    }

    public static async Task<Product?> Handle(GetProductBySkuQuery query, ICatalogRepository repository)
    {
        // Note: This would need to be implemented in the repository if needed
        // For now, we'll return null as it's not implemented
        return null;
    }
}
