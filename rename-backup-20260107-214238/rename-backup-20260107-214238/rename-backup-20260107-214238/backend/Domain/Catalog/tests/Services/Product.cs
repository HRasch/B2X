using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Product entity (simplified for tests)
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
