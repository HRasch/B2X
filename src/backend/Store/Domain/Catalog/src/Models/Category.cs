using System.ComponentModel.DataAnnotations;

namespace B2X.Catalog.Models;

/// <summary>
/// Category aggregate root
/// Represents product categories in a hierarchical structure
/// </summary>
public class Category
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    // Category identification
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }

    // Hierarchy
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public List<Category> Children { get; set; } = new();

    // Display properties
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;

    // SEO properties
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    // Status
    public bool IsActive { get; set; } = true;
    public bool IsVisible { get; set; } = true;

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Computed properties
    public int Level => GetLevel();
    public string FullPath => GetFullPath();

    private int GetLevel()
    {
        if (!ParentId.HasValue)
            return 0;
        return (Parent?.Level ?? 0) + 1;
    }

    private string GetFullPath()
    {
        if (Parent == null)
            return Name;
        return $"{Parent.FullPath} > {Name}";
    }

    // Business methods
    public void AddChild(Category child)
    {
        if (!Children.Contains(child))
        {
            Children.Add(child);
            child.ParentId = Id;
            child.Parent = this;
        }
    }

    public void RemoveChild(Category child)
    {
        Children.Remove(child);
        child.ParentId = null;
        child.Parent = null;
    }
}

/// <summary>
/// DTO for category responses
/// </summary>
public class CategoryDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public int Level { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public List<CategoryDto> Children { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating categories
/// </summary>
public class CreateCategoryDto
{
    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }
    public string? Slug { get; set; }
    public Guid? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// DTO for updating categories
/// </summary>
public class UpdateCategoryDto
{
    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }
    public string? Slug { get; set; }
    public Guid? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisible { get; set; } = true;
}
