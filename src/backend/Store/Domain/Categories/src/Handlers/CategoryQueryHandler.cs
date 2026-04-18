using B2X.Categories.Models;
using B2X.Types.DTOs;
using Wolverine;

namespace B2X.Categories.Handlers;

/// <summary>
/// Query commands for category operations
/// </summary>
public record GetCategoryByIdQuery(Guid Id, Guid TenantId);

public record GetCategoriesQuery(
    Guid TenantId,
    string? SearchTerm = null,
    Guid? ParentId = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 50,
    string? SortBy = "DisplayOrder",
    bool SortDescending = false
);

public record GetCategoryHierarchyQuery(Guid TenantId, Guid? RootCategoryId = null);

/// <summary>
/// Query handler for category operations
/// </summary>
public class CategoryQueryHandler
{
    private readonly ICategoryRepository _repository;

    public CategoryQueryHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery query)
    {
        return await _repository.GetByIdAsync(query.Id, query.TenantId).ConfigureAwait(false);
    }

    public async Task<PagedResult<Category>> Handle(GetCategoriesQuery query)
    {
        return await _repository.GetPagedAsync(
            query.TenantId,
            query.SearchTerm,
            query.ParentId,
            query.IsActive,
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDescending
        ).ConfigureAwait(false);
    }

    public async Task<List<CategoryDto>> Handle(GetCategoryHierarchyQuery query)
    {
        var categories = await _repository.GetHierarchyAsync(query.TenantId, query.RootCategoryId).ConfigureAwait(false);
        return categories.Select(MapToDto).ToList();
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            ParentId = category.ParentId,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            ImageUrl = category.ImageUrl,
            Icon = category.Icon,
            DisplayOrder = category.DisplayOrder,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            IsActive = category.IsActive,
            IsVisible = category.IsVisible,
            Level = category.Level,
            FullPath = category.FullPath,
            Children = category.Children.Select(MapToDto).ToList(),
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
