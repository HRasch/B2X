using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Repositories;
using B2Connect.Types;
using B2Connect.Types.Localization;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Implementation of product service
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async Task<ProductDto?> GetProductAsync(Guid id)
    {
        var product = await _productRepository.GetWithDetailsAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku)
    {
        var product = await _productRepository.GetBySkuAsync(sku);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto?> GetProductBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);
        if (product == null) return null;
        return MapToDto(await _productRepository.GetWithDetailsAsync(product.Id) ?? product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByBrandAsync(Guid brandId)
    {
        var products = await _productRepository.GetByBrandAsync(brandId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10)
    {
        var products = await _productRepository.GetFeaturedAsync(take);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetNewProductsAsync(int take = 10)
    {
        var products = await _productRepository.GetNewAsync(take);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<(IEnumerable<ProductDto> Items, int Total)> GetProductsPagedAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _productRepository.GetPagedAsync(pageNumber, pageSize);
        return (items.Select(MapToDto), total);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Sku = dto.Sku,
            Slug = dto.Slug,
            Name = LocalizedContent.FromDictionary(dto.Name),
            ShortDescription = LocalizedContent.FromDictionary(dto.ShortDescription),
            Description = LocalizedContent.FromDictionary(dto.Description),
            Price = dto.Price,
            SpecialPrice = dto.SpecialPrice,
            StockQuantity = dto.StockQuantity,
            BrandId = dto.BrandId,
            IsActive = true,
            IsNew = true
        };

        await _productRepository.CreateAsync(product);

        // Add product categories
        if (dto.CategoryIds.Count > 0)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryList = categories.Where(c => dto.CategoryIds.Contains(c.Id)).ToList();

            foreach (var category in categoryList)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = category.Id,
                    IsPrimary = product.ProductCategories.Count == 0
                });
            }
        }

        await _productRepository.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return new Result<ProductDto>.Failure(ErrorCodes.NotFound, ErrorCodes.NotFound.ToMessage());

        if (dto.Name != null)
            product.Name = LocalizedContent.FromDictionary(dto.Name);
        if (dto.ShortDescription != null)
            product.ShortDescription = LocalizedContent.FromDictionary(dto.ShortDescription);
        if (dto.Description != null)
            product.Description = LocalizedContent.FromDictionary(dto.Description);
        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;
        if (dto.SpecialPrice != null)
            product.SpecialPrice = dto.SpecialPrice;
        if (dto.StockQuantity.HasValue)
            product.StockQuantity = dto.StockQuantity.Value;
        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;
        if (dto.IsFeatured.HasValue)
            product.IsFeatured = dto.IsFeatured.Value;
        if (dto.BrandId.HasValue)
            product.BrandId = dto.BrandId;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return new Result<ProductDto>.Success(MapToDto(product), "Product updated successfully");
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        return await _productRepository.DeleteAsync(id) &&
               await _productRepository.SaveChangesAsync() > 0;
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Slug = product.Slug,
            Name = product.Name?.Translations ?? new(),
            ShortDescription = product.ShortDescription?.Translations,
            Description = product.Description?.Translations,
            Price = product.Price,
            SpecialPrice = product.SpecialPrice,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            IsNew = product.IsNew,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name?.Get("en"),
            VariantCount = product.Variants.Count,
            ImageCount = product.Images.Count,
            Categories = product.ProductCategories
                .Select(pc => MapCategoryToDto(pc.Category))
                .ToList(),
            Variants = product.Variants
                .Where(v => v.IsActive)
                .Select(v => MapVariantToDto(v))
                .ToList()
        };
    }

    private CategoryDto MapCategoryToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = category.Name?.Translations ?? new(),
            Description = category.Description?.Translations,
            IsActive = category.IsActive
        };
    }

    private ProductVariantDto MapVariantToDto(ProductVariant variant)
    {
        var attributeValues = new Dictionary<string, string>();
        foreach (var attrVal in variant.AttributeValues.Where(av => av.IsActive || av.IsActive))
        {
            var label = attrVal.Option?.Label?.Get("en") ?? attrVal.Value ?? string.Empty;
            attributeValues[attrVal.Attribute.Code] = label;
        }

        return new ProductVariantDto
        {
            Id = variant.Id,
            Sku = variant.Sku,
            Name = variant.Name?.Translations ?? new(),
            Price = variant.Price,
            StockQuantity = variant.StockQuantity,
            IsActive = variant.IsActive,
            AttributeValues = attributeValues
        };
    }
}
