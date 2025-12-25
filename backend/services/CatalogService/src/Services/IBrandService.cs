using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Service interface for Brand operations
/// </summary>
public interface IBrandService
{
    /// <summary>Gets a brand by ID</summary>
    Task<BrandDto?> GetBrandAsync(Guid id);

    /// <summary>Gets a brand by slug</summary>
    Task<BrandDto?> GetBrandBySlugAsync(string slug);

    /// <summary>Gets all active brands</summary>
    Task<IEnumerable<BrandDto>> GetActiveBrandsAsync();

    /// <summary>Gets paginated brands</summary>
    Task<(IEnumerable<BrandDto> Items, int Total)> GetBrandsPagedAsync(int pageNumber, int pageSize);

    /// <summary>Creates a new brand</summary>
    Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);

    /// <summary>Updates an existing brand</summary>
    Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto dto);

    /// <summary>Deletes a brand</summary>
    Task<bool> DeleteBrandAsync(Guid id);
}

public class BrandDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CreateBrandDto
{
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateBrandDto
{
    public Dictionary<string, string>? Name { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool? IsActive { get; set; }
    public int? DisplayOrder { get; set; }
}
