using B2X.Catalog.Core.Entities;
using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Infrastructure.Data;

/// <summary>
/// Repository for catalog import operations
/// </summary>
public class CatalogImportRepository : ICatalogImportRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogImportRepository> _logger;

    public CatalogImportRepository(CatalogDbContext context, ILogger<CatalogImportRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CatalogImport?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CatalogImports
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id, ct).ConfigureAwait(false);
    }

    public async Task<CatalogImport?> GetByCompositeKeyAsync(
        Guid tenantId,
        string supplierId,
        string catalogId,
        DateTime importTimestamp,
        CancellationToken ct = default)
    {
        return await _context.CatalogImports
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.SupplierId == supplierId &&
                x.CatalogId == catalogId &&
                x.ImportTimestamp == importTimestamp,
                ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatalogImport>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CatalogImports
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatalogImport>> GetByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default)
    {
        return await _context.CatalogImports
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<int> CountByTenantAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.CatalogImports
            .CountAsync(x => x.TenantId == tenantId, ct).ConfigureAwait(false);
    }

    public async Task AddAsync(CatalogImport catalogImport, CancellationToken ct = default)
    {
        await _context.CatalogImports.AddAsync(catalogImport, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation(
            "Created catalog import {ImportId} for tenant {TenantId}, supplier {SupplierId}",
            catalogImport.Id, catalogImport.TenantId, catalogImport.SupplierId);
    }

    public async Task UpdateAsync(CatalogImport catalogImport, CancellationToken ct = default)
    {
        _context.CatalogImports.Update(catalogImport);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation(
            "Updated catalog import {ImportId}, status: {Status}",
            catalogImport.Id, catalogImport.Status);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var catalogImport = await GetByIdAsync(id, ct).ConfigureAwait(false);
        if (catalogImport != null)
        {
            _context.CatalogImports.Remove(catalogImport);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);

            _logger.LogInformation(
                "Deleted catalog import {ImportId} with {ProductCount} products",
                id, catalogImport.ProductCount);
        }
    }
}

/// <summary>
/// Repository for catalog products
/// </summary>
public class CatalogProductRepository : ICatalogProductRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogProductRepository> _logger;

    public CatalogProductRepository(CatalogDbContext context, ILogger<CatalogProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CatalogProduct?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CatalogProducts
            .Include(x => x.CatalogImport)
            .FirstOrDefaultAsync(x => x.Id == id, ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatalogProduct>> GetByCatalogImportIdAsync(Guid catalogImportId, CancellationToken ct = default)
    {
        return await _context.CatalogProducts
            .Where(x => x.CatalogImportId == catalogImportId)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<CatalogProduct>> GetByImportIdAsync(Guid importId, int page, int pageSize, CancellationToken ct = default)
    {
        return await _context.CatalogProducts
            .Where(x => x.CatalogImportId == importId)
            .OrderBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<int> CountByImportIdAsync(Guid importId, CancellationToken ct = default)
    {
        return await _context.CatalogProducts
            .CountAsync(x => x.CatalogImportId == importId, ct).ConfigureAwait(false);
    }

    public async Task<CatalogProduct?> GetByImportAndAidAsync(Guid importId, string supplierAid, CancellationToken ct = default)
    {
        return await _context.CatalogProducts
            .FirstOrDefaultAsync(x => x.CatalogImportId == importId && x.SupplierAid == supplierAid, ct).ConfigureAwait(false);
    }

    public async Task AddAsync(CatalogProduct product, CancellationToken ct = default)
    {
        await _context.CatalogProducts.AddAsync(product, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Added catalog product {ProductId}", product.Id);
    }

    public async Task AddRangeAsync(IEnumerable<CatalogProduct> products, CancellationToken ct = default)
    {
        await _context.CatalogProducts.AddRangeAsync(products, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Added {Count} catalog products", products.Count());
    }

    public async Task UpdateAsync(CatalogProduct product, CancellationToken ct = default)
    {
        _context.CatalogProducts.Update(product);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Updated catalog product {ProductId}", product.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct).ConfigureAwait(false);
        if (product != null)
        {
            _context.CatalogProducts.Remove(product);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);

            _logger.LogInformation("Deleted catalog product {ProductId}", id);
        }
    }

    public async Task DeleteByImportIdAsync(Guid importId, CancellationToken ct = default)
    {
        var products = await GetByCatalogImportIdAsync(importId, ct).ConfigureAwait(false);
        _context.CatalogProducts.RemoveRange(products);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Deleted {Count} products for import {ImportId}", products.Count(), importId);
    }

    public async Task DeleteByCatalogImportIdAsync(Guid catalogImportId, CancellationToken ct = default)
    {
        var products = await GetByCatalogImportIdAsync(catalogImportId, ct).ConfigureAwait(false);
        _context.CatalogProducts.RemoveRange(products);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation(
            "Deleted {Count} products for catalog import {ImportId}",
            products.Count(), catalogImportId);
    }
}
