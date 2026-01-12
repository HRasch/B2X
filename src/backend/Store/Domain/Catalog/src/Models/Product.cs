namespace B2X.Catalog.Models;

/// <summary>
/// Product aggregate root
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    // Updated: Proper category relationships instead of strings
    private readonly List<Guid> _categoryIds = new();
    public IReadOnlyCollection<Guid> CategoryIds => _categoryIds.AsReadOnly();

    public string? BrandName { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Barcode { get; set; } // Added for barcode scanning support
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsAvailable => StockQuantity > 0 && IsActive;

    // Business methods for category management
    public void AddCategory(Guid categoryId)
    {
        if (!_categoryIds.Contains(categoryId))
        {
            _categoryIds.Add(categoryId);
        }
    }

    public void RemoveCategory(Guid categoryId)
    {
        _categoryIds.Remove(categoryId);
    }

    public void SetCategories(IEnumerable<Guid> categoryIds)
    {
        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds.Distinct());
    }
}
