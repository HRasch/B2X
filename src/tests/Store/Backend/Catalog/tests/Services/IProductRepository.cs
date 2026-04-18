using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Product Repository Mock Interface
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetBySkuAsync(Guid tenantId, string? sku, CancellationToken ct = default);
    Task<IReadOnlyCollection<Product>> GetAllAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<bool> ExistsBySkuAsync(Guid tenantId, string? sku, CancellationToken ct = default);
}
