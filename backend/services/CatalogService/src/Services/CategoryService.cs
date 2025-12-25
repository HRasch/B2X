using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Repositories;
using B2Connect.Types.Localization;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Implementation of category service
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CategoryDto?> GetCategoryAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
    {
        var category = await _repository.GetBySlugAsync(slug);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        var categories = await _repository.GetRootCategoriesAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetChildCategoriesAsync(Guid parentId)
    {
        var categories = await _repository.GetChildCategoriesAsync(parentId);
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync()
    {
        var categories = await _repository.GetHierarchyAsync();
        return categories.Select(c => MapToDtoWithChildren(c));
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _repository.GetActiveAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Slug = dto.Slug,
            Name = LocalizedContent.FromDictionary(dto.Name),
            Description = LocalizedContent.FromDictionary(dto.Description),
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = dto.IsActive
        };

        await _repository.CreateAsync(category);
        await _repository.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        if (dto.Name != null)
            category.Name = LocalizedContent.FromDictionary(dto.Name);
        if (dto.Description != null)
            category.Description = LocalizedContent.FromDictionary(dto.Description);
        if (dto.IsActive.HasValue)
            category.IsActive = dto.IsActive.Value;
        if (dto.DisplayOrder.HasValue)
            category.DisplayOrder = dto.DisplayOrder.Value;

        category.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(category);
        await _repository.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        return await _repository.DeleteAsync(id) &&
               await _repository.SaveChangesAsync() > 0;
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = category.Name?.Translations ?? new(),
            Description = category.Description?.Translations,
            IsActive = category.IsActive,
            ProductCount = category.ProductCategories.Count
        };
    }

    private CategoryDto MapToDtoWithChildren(Category category)
    {
        var dto = MapToDto(category);
        // Note: Children would need to be loaded and mapped recursively
        return dto;
    }
}
