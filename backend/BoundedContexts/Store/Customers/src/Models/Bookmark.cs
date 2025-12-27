namespace B2Connect.Store.Customers.Models;

/// <summary>
/// Bookmarked/favorited product
/// References User and Product by ID only (no navigation properties to maintain Bounded Context separation)
/// </summary>
public class Bookmark
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Reference to Shared.User.Models.User without direct navigation
    public Guid ProductId { get; set; } // Reference to Catalog.Product without direct navigation
    public Guid TenantId { get; set; }
    public string? Notes { get; set; }
    public BookmarkStatus Status { get; set; } = BookmarkStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsValid()
    {
        return Status == BookmarkStatus.Active;
    }
}

public enum BookmarkStatus
{
    Active = 1,
    Removed = 2,
    Archived = 3
}
