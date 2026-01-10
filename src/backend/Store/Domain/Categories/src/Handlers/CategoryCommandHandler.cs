using B2X.Categories.Models;
using Wolverine;

namespace B2X.Categories.Handlers;

/// <summary>
/// Commands for category operations
/// </summary>
public record CreateCategoryCommand(
    Guid TenantId,
    string Name,
    string? Description,
    string? Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible
);

public record UpdateCategoryCommand(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    string? Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible
);

public record DeleteCategoryCommand(Guid Id, Guid TenantId);

/// <summary>
/// Command handler for category operations
/// </summary>
public class CategoryCommandHandler
{
    private readonly ICategoryRepository _repository;

    public CategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category> Handle(CreateCategoryCommand command)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Description = command.Description,
            Slug = command.Slug ?? GenerateSlug(command.Name),
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

        // Validate parent exists if specified
        if (command.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(command.ParentId.Value, command.TenantId);
            if (parent == null)
            {
                throw new InvalidOperationException($"Parent category {command.ParentId} not found");
            }
        }

        // Prevent circular references
        if (command.ParentId.HasValue)
        {
            await ValidateNoCircularReference(category.Id, command.ParentId.Value, command.TenantId);
        }

        await _repository.AddAsync(category);
        return category;
    }

    public async Task<Category> Handle(UpdateCategoryCommand command)
    {
        var category = await _repository.GetByIdAsync(command.Id, command.TenantId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category {command.Id} not found");
        }

        // Validate parent exists if specified
        if (command.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(command.ParentId.Value, command.TenantId);
            if (parent == null)
            {
                throw new InvalidOperationException($"Parent category {command.ParentId} not found");
            }
        }

        // Prevent circular references
        if (command.ParentId.HasValue && command.ParentId.Value != category.ParentId)
        {
            await ValidateNoCircularReference(command.Id, command.ParentId.Value, command.TenantId);
        }

        category.Name = command.Name;
        category.Description = command.Description;
        category.Slug = command.Slug ?? GenerateSlug(command.Name);
        category.ParentId = command.ParentId;
        category.ImageUrl = command.ImageUrl;
        category.Icon = command.Icon;
        category.DisplayOrder = command.DisplayOrder;
        category.MetaTitle = command.MetaTitle;
        category.MetaDescription = command.MetaDescription;
        category.IsActive = command.IsActive;
        category.IsVisible = command.IsVisible;
        category.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(category);
        return category;
    }

    public async Task Handle(DeleteCategoryCommand command)
    {
        var category = await _repository.GetByIdAsync(command.Id, command.TenantId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category {command.Id} not found");
        }

        // Check if category has children
        var children = await _repository.GetChildrenAsync(command.Id, command.TenantId);
        if (children.Any())
        {
            throw new InvalidOperationException($"Cannot delete category {command.Id} because it has child categories");
        }

        await _repository.DeleteAsync(command.Id, command.TenantId);
    }

    private async Task ValidateNoCircularReference(Guid categoryId, Guid parentId, Guid tenantId)
    {
        var current = parentId;
        var visited = new HashSet<Guid> { categoryId };

        while (current != Guid.Empty)
        {
            if (visited.Contains(current))
            {
                throw new InvalidOperationException("Circular reference detected in category hierarchy");
            }

            visited.Add(current);
            var parent = await _repository.GetByIdAsync(current, tenantId);
            current = parent?.ParentId ?? Guid.Empty;
        }
    }

    private string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("&", "and")
            .Replace("?", "")
            .Replace("#", "")
            .Replace("%", "")
            .Replace("*", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("@", "")
            .Replace("$", "")
            .Replace("^", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("|", "")
            .Replace("=", "")
            .Replace("+", "")
            .Replace("_", "-")
            .Trim('-');
    }
}
