using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Application.Services;

/// <summary>
/// Service interface for Category operations
/// </summary>
public interface ICategoryService
{
    /// <summary>Gets a category by ID</summary>
    Task<CategoryDto?> GetCategoryAsync(Guid id);

    /// <summary>Gets a category by slug</summary>
    Task<CategoryDto?> GetCategoryBySlugAsync(string slug);

    /// <summary>Gets all root categories</summary>
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();

    /// <summary>Gets child categories of a parent</summary>
    Task<IEnumerable<CategoryDto>> GetChildCategoriesAsync(Guid parentId);

    /// <summary>Gets the complete category hierarchy</summary>
    Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync();

    /// <summary>Gets all active categories</summary>
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();

    /// <summary>Creates a new category</summary>
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);

    /// <summary>Updates an existing category</summary>
    Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);

    /// <summary>Deletes a category</summary>
    Task<bool> DeleteCategoryAsync(Guid id);
}

public class CreateCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string>? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryDto
{
    public Dictionary<string, string>? Name { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public bool? IsActive { get; set; }
    public int? DisplayOrder { get; set; }
}
