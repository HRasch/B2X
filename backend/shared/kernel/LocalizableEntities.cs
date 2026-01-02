using B2Connect.Types.Localization;

namespace B2Connect.Types.Domain;

/// <summary>
/// Represents a product or service with localized name and description
/// </summary>
public class Product : Entity
{
    /// <summary>
    /// Localized product name (stored as JSON)
    /// Usage: product.Name.Get("de"), product.Name.Set("fr", "Produit")
    /// </summary>
    public LocalizedContent Name { get; set; } = new();

    /// <summary>
    /// Localized product description (stored as JSON)
    /// </summary>
    public LocalizedContent Description { get; set; } = new();

    /// <summary>
    /// Product SKU (not localized)
    /// </summary>
    public required string Sku { get; set; }

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Tenant this product belongs to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Product status
    /// </summary>
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
}

/// <summary>
/// Product status enumeration
/// </summary>
public enum ProductStatus
{
    Draft = 0,
    Active = 1,
    Archived = 2,
    Discontinued = 3
}

/// <summary>
/// Represents a content page with localized title, description, and body
/// </summary>
public class ContentPage : Entity
{
    /// <summary>
    /// Localized page title
    /// </summary>
    public LocalizedContent Title { get; set; } = new();

    /// <summary>
    /// Localized page slug/URL identifier (e.g., "about-us", "impressum")
    /// Note: Ideally this should be language-specific, but for simplicity one slug per page
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Localized page description (meta description)
    /// </summary>
    public LocalizedContent Description { get; set; } = new();

    /// <summary>
    /// Localized page content/body
    /// </summary>
    public LocalizedContent Content { get; set; } = new();

    /// <summary>
    /// Tenant this page belongs to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Is this page published?
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Publication date
    /// </summary>
    public DateTime? PublishedAt { get; set; }
}

/// <summary>
/// Represents a navigation menu item with localized label
/// </summary>
public class MenuItem : Entity
{
    /// <summary>
    /// Localized menu label
    /// </summary>
    public LocalizedContent Label { get; set; } = new();

    /// <summary>
    /// Menu URL or path
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Parent menu item (for nested menus)
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Tenant this menu belongs to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Is this menu item visible?
    /// </summary>
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// Represents a FAQ entry with localized question and answer
/// </summary>
public class FaqEntry : Entity
{
    /// <summary>
    /// Localized question
    /// </summary>
    public LocalizedContent Question { get; set; } = new();

    /// <summary>
    /// Localized answer
    /// </summary>
    public LocalizedContent Answer { get; set; } = new();

    /// <summary>
    /// Category for organizing FAQs
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Display order within category
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Tenant this FAQ belongs to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Is this FAQ published?
    /// </summary>
    public bool IsPublished { get; set; } = true;
}

/// <summary>
/// Represents a feature or benefit with localized name and description
/// </summary>
public class Feature : Entity
{
    /// <summary>
    /// Localized feature name
    /// </summary>
    public LocalizedContent Name { get; set; } = new();

    /// <summary>
    /// Localized feature description
    /// </summary>
    public LocalizedContent Description { get; set; } = new();

    /// <summary>
    /// Feature icon/image URL
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Tenant this feature belongs to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Is this feature active?
    /// </summary>
    public bool IsActive { get; set; } = true;
}
