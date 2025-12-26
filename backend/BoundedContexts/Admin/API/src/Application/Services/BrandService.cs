using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Types.Localization;

namespace B2Connect.Admin.Application.Services;

/// <summary>
/// Implementation of brand service
/// </summary>
public class BrandService : IBrandService
{
    private readonly IBrandRepository _repository;

    public BrandService(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BrandDto?> GetBrandAsync(Guid id)
    {
        var brand = await _repository.GetByIdAsync(id);
        return brand != null ? MapToDto(brand) : null;
    }

    public async Task<BrandDto?> GetBrandBySlugAsync(string slug)
    {
        var brand = await _repository.GetBySlugAsync(slug);
        return brand != null ? MapToDto(brand) : null;
    }

    public async Task<IEnumerable<BrandDto>> GetActiveBrandsAsync()
    {
        var brands = await _repository.GetActiveAsync();
        return brands.Select(MapToDto);
    }

    public async Task<(IEnumerable<BrandDto> Items, int Total)> GetBrandsPagedAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _repository.GetPagedAsync(pageNumber, pageSize);
        return (items.Select(MapToDto), total);
    }

    public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
    {
        var brand = new Brand
        {
            Slug = dto.Slug,
            Name = LocalizedContent.FromDictionary(dto.Name),
            Description = LocalizedContent.FromDictionary(dto.Description),
            LogoUrl = dto.LogoUrl,
            WebsiteUrl = dto.WebsiteUrl,
            IsActive = dto.IsActive
        };

        await _repository.CreateAsync(brand);
        await _repository.SaveChangesAsync();

        return MapToDto(brand);
    }

    public async Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto dto)
    {
        var brand = await _repository.GetByIdAsync(id);
        if (brand == null)
            throw new KeyNotFoundException($"Brand with ID {id} not found");

        if (dto.Name != null)
            brand.Name = LocalizedContent.FromDictionary(dto.Name);
        if (dto.Description != null)
            brand.Description = LocalizedContent.FromDictionary(dto.Description);
        if (dto.LogoUrl != null)
            brand.LogoUrl = dto.LogoUrl;
        if (dto.WebsiteUrl != null)
            brand.WebsiteUrl = dto.WebsiteUrl;
        if (dto.IsActive.HasValue)
            brand.IsActive = dto.IsActive.Value;
        if (dto.DisplayOrder.HasValue)
            brand.DisplayOrder = dto.DisplayOrder.Value;

        brand.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(brand);
        await _repository.SaveChangesAsync();

        return MapToDto(brand);
    }

    public async Task<bool> DeleteBrandAsync(Guid id)
    {
        return await _repository.DeleteAsync(id) &&
               await _repository.SaveChangesAsync() > 0;
    }

    private BrandDto MapToDto(Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Slug = brand.Slug,
            Name = brand.Name?.Translations ?? new(),
            Description = brand.Description?.Translations,
            LogoUrl = brand.LogoUrl,
            WebsiteUrl = brand.WebsiteUrl,
            IsActive = brand.IsActive,
            ProductCount = brand.Products.Count
        };
    }
}
