using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Service interface for Product operations
/// </summary>
public interface IProductService
{
    /// <summary>Gets a product by ID with all related data</summary>
    Task<ProductDto?> GetProductAsync(Guid id);

    /// <summary>Gets a product by SKU</summary>
    Task<ProductDto?> GetProductBySkuAsync(string sku);

    /// <summary>Gets a product by slug</summary>
    Task<ProductDto?> GetProductBySlugAsync(string slug);

    /// <summary>Gets all products</summary>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();

    /// <summary>Gets products by category</summary>
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);

    /// <summary>Gets products by brand</summary>
    Task<IEnumerable<ProductDto>> GetProductsByBrandAsync(Guid brandId);

    /// <summary>Gets featured products</summary>
    Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10);

    /// <summary>Gets new products</summary>
    Task<IEnumerable<ProductDto>> GetNewProductsAsync(int take = 10);

    /// <summary>Searches products</summary>
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);

    /// <summary>Gets paginated products</summary>
    Task<(IEnumerable<ProductDto> Items, int Total)> GetProductsPagedAsync(int pageNumber, int pageSize);

    /// <summary>Creates a new product</summary>
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);

    /// <summary>Updates an existing product</summary>
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto);

    /// <summary>Deletes a product</summary>
    Task<bool> DeleteProductAsync(Guid id);
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? ShortDescription { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SpecialPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int VariantCount { get; set; }
    public int ImageCount { get; set; }
    public List<CategoryDto> Categories { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
}

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string> AttributeValues { get; set; } = new();
}

public class CreateProductDto
{
    public string Sku { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? ShortDescription { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SpecialPrice { get; set; }
    public int StockQuantity { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid> CategoryIds { get; set; } = new();
}

public class UpdateProductDto
{
    public Dictionary<string, string>? Name { get; set; }
    public Dictionary<string, string>? ShortDescription { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? SpecialPrice { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    public Guid? BrandId { get; set; }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? Description { get; set; }
    public int ProductCount { get; set; }
    public bool IsActive { get; set; }
}
