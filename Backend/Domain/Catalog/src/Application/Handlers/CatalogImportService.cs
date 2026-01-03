using B2Connect.Catalog.Application.Adapters;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Handlers;

/// <summary>
/// Wolverine Handler for BMEcat catalog import
/// POST /api/catalog/import/bmecat
/// </summary>
public class CatalogImportService
{
    private readonly ICatalogImportAdapter _adapter;
    private readonly ICatalogImportRepository _importRepository;
    private readonly ICatalogProductRepository _productRepository;
    private readonly ILogger<CatalogImportService> _logger;

    public CatalogImportService(
        ICatalogImportAdapter adapter,
        ICatalogImportRepository importRepository,
        ICatalogProductRepository productRepository,
        ILogger<CatalogImportService> logger)
    {
        _adapter = adapter;
        _importRepository = importRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Import BMEcat catalog
    /// </summary>
    public async Task<CatalogImportResponse> ImportBmecat(ImportBmecatRequest request)
    {
        _logger.LogInformation(
            "Starting BMEcat import for tenant {TenantId}, supplier {SupplierId}",
            request.TenantId, request.Metadata.SupplierId);

        // Create catalog import record
        var catalogImport = new CatalogImport
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            SupplierId = request.Metadata.SupplierId,
            CatalogId = request.Metadata.CatalogId,
            ImportTimestamp = request.Metadata.ImportTimestamp,
            Version = request.Metadata.Version,
            Description = request.Metadata.Description,
            Status = ImportStatus.Processing,
            ProductCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _importRepository.AddAsync(catalogImport);

        try
        {
            // Update metadata with catalog import ID for products
            request.Metadata.TenantId = catalogImport.Id; // Note: reusing TenantId field temporarily

            // Import using adapter
            var result = await _adapter.ImportAsync(request.CatalogStream, request.Metadata);

            if (result.Success)
            {
                // Create products
                var products = result.Products;
                // Set correct catalog import ID
                foreach (var product in products)
                {
                    product.CatalogImportId = catalogImport.Id;
                }
                await _productRepository.AddRangeAsync(products);

                // Update import record
                catalogImport.Status = ImportStatus.Completed;
                catalogImport.ProductCount = result.ProductCount;
                catalogImport.UpdatedAt = DateTime.UtcNow;

                await _importRepository.UpdateAsync(catalogImport);

                _logger.LogInformation(
                    "Successfully imported BMEcat catalog {ImportId} with {ProductCount} products",
                    catalogImport.Id, result.ProductCount);

                return new CatalogImportResponse
                {
                    Success = true,
                    ImportId = catalogImport.Id,
                    ProductCount = result.ProductCount,
                    Message = $"Successfully imported {result.ProductCount} products"
                };
            }
            else
            {
                // Mark as failed
                catalogImport.Status = ImportStatus.Failed;
                catalogImport.UpdatedAt = DateTime.UtcNow;
                await _importRepository.UpdateAsync(catalogImport);

                _logger.LogWarning(
                    "BMEcat import failed for {ImportId}: {Error}",
                    catalogImport.Id, result.ErrorMessage);

                return new CatalogImportResponse
                {
                    Success = false,
                    ImportId = catalogImport.Id,
                    ErrorMessage = result.ErrorMessage,
                    ValidationErrors = result.ValidationErrors
                };
            }
        }
        catch (Exception ex)
        {
            // Mark as failed
            catalogImport.Status = ImportStatus.Failed;
            catalogImport.UpdatedAt = DateTime.UtcNow;
            await _importRepository.UpdateAsync(catalogImport);

            _logger.LogError(ex, "Unexpected error during BMEcat import {ImportId}", catalogImport.Id);

            return new CatalogImportResponse
            {
                Success = false,
                ImportId = catalogImport.Id,
                ErrorMessage = $"Import failed: {ex.Message}"
            };
        }
    }

    private IEnumerable<CatalogProduct> CreateProductsFromResult(CatalogImportResult result, Guid catalogImportId)
    {
        // Note: In a real implementation, the adapter would return the products
        // For now, this is a placeholder - the adapter needs to be updated to return products
        return Enumerable.Empty<CatalogProduct>();
    }
}

/// <summary>
/// Request for BMEcat import
/// </summary>
public class ImportBmecatRequest
{
    public Guid TenantId { get; set; }
    public required Stream CatalogStream { get; set; }
    public required CatalogMetadata Metadata { get; set; }
}

/// <summary>
/// Response for catalog import
/// </summary>
public class CatalogImportResponse
{
    public bool Success { get; set; }
    public Guid ImportId { get; set; }
    public int ProductCount { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}