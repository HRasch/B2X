using System.Text.Json;
using B2Connect.Catalog.Application.Adapters;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Handlers;

/// <summary>
/// Wolverine Handler for catalog import (BMEcat, icecat, etc.)
/// Supports multiple import formats through adapter pattern
/// </summary>
public class CatalogImportService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICatalogImportRepository _importRepository;
    private readonly ICatalogProductRepository _productRepository;
    private readonly ILogger<CatalogImportService> _logger;

    public CatalogImportService(
        IServiceProvider serviceProvider,
        ICatalogImportRepository importRepository,
        ICatalogProductRepository productRepository,
        ILogger<CatalogImportService> logger)
    {
        _serviceProvider = serviceProvider;
        _importRepository = importRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Import catalog file using detected format
    /// </summary>
    public async Task<CatalogImport> ImportAsync(Guid tenantId, Stream stream, string format, string? customSchemaPath = null, CancellationToken ct = default)
    {
        // Get adapter for format
        var adapter = GetAdapterForFormat(format);
        if (adapter == null)
        {
            throw new NotSupportedException($"Catalog format '{format}' is not supported. Supported formats: bmecat, icecat");
        }

        _logger.LogInformation("Starting {Format} catalog import for tenant {TenantId}", format, tenantId);

        // Create import record
        var catalogImport = new CatalogImport
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SupplierId = "pending",
            CatalogId = "pending",
            ImportTimestamp = DateTime.UtcNow,
            Status = ImportStatus.Processing,
            ProductCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _importRepository.AddAsync(catalogImport, ct).ConfigureAwait(false);

        try
        {
            // Parse catalog
            var metadata = new CatalogMetadata
            {
                TenantId = tenantId,
                ImportTimestamp = DateTime.UtcNow,
                CustomSchemaPath = customSchemaPath
            };

            var result = await adapter.ImportAsync(stream, metadata, ct).ConfigureAwait(false);

            // Update import with catalog metadata
            catalogImport.SupplierId = result.SupplierId ?? "unknown";
            catalogImport.CatalogId = result.CatalogId ?? "unknown";
            catalogImport.Version = result.Version;
            catalogImport.Description = result.Description;

            if (result.Success)
            {
                // Create product records
                var catalogProducts = result.Products.ToList();
                foreach (var product in catalogProducts)
                {
                    product.CatalogImportId = catalogImport.Id;
                }

                await _productRepository.AddRangeAsync(catalogProducts, ct).ConfigureAwait(false);

                catalogImport.Status = ImportStatus.Completed;
                catalogImport.ProductCount = result.ProductCount;
                catalogImport.UpdatedAt = DateTime.UtcNow;

                await _importRepository.UpdateAsync(catalogImport, ct).ConfigureAwait(false);

                _logger.LogInformation(
                    "Successfully imported {Format} catalog {ImportId} with {ProductCount} products from supplier {SupplierId}",
                    format, catalogImport.Id, result.ProductCount, catalogImport.SupplierId);
            }
            else
            {
                catalogImport.Status = ImportStatus.Failed;
                catalogImport.Description = result.ErrorMessage;
                catalogImport.UpdatedAt = DateTime.UtcNow;

                await _importRepository.UpdateAsync(catalogImport, ct).ConfigureAwait(false);

                _logger.LogWarning("Catalog import failed for {ImportId}: {Error}", catalogImport.Id, result.ErrorMessage);
                throw new InvalidOperationException(result.ErrorMessage ?? "Import failed");
            }

            return catalogImport;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not NotSupportedException)
        {
            catalogImport.Status = ImportStatus.Failed;
            catalogImport.Description = ex.Message;
            catalogImport.UpdatedAt = DateTime.UtcNow;

            await _importRepository.UpdateAsync(catalogImport, ct).ConfigureAwait(false);

            _logger.LogError(ex, "Unexpected error during catalog import {ImportId}", catalogImport.Id);
            throw new InvalidOperationException($"Import failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get paginated list of imports for a tenant
    /// </summary>
    public async Task<PagedImportResult> GetImportsAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default)
    {
        var imports = await _importRepository.GetByTenantAsync(tenantId, page, pageSize, ct).ConfigureAwait(false);
        var totalCount = await _importRepository.CountByTenantAsync(tenantId, ct).ConfigureAwait(false);

        return new PagedImportResult
        {
            Items = imports.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    /// <summary>
    /// Get a specific import by ID
    /// </summary>
    public async Task<CatalogImportDto?> GetImportAsync(Guid tenantId, Guid importId, CancellationToken ct = default)
    {
        var import = await _importRepository.GetByIdAsync(importId, ct).ConfigureAwait(false);
        if (import == null || import.TenantId != tenantId)
        {
            return null;
        }

        return MapToDto(import);
    }

    /// <summary>
    /// Get products from a specific import
    /// </summary>
    public async Task<PagedProductResult> GetImportProductsAsync(Guid tenantId, Guid importId, int page, int pageSize, CancellationToken ct = default)
    {
        // Verify import belongs to tenant
        var import = await _importRepository.GetByIdAsync(importId, ct).ConfigureAwait(false);
        if (import == null || import.TenantId != tenantId)
        {
            return new PagedProductResult { Items = [], Page = page, PageSize = pageSize, TotalCount = 0, TotalPages = 0 };
        }

        var products = await _productRepository.GetByImportIdAsync(importId, page, pageSize, ct).ConfigureAwait(false);
        var totalCount = await _productRepository.CountByImportIdAsync(importId, ct).ConfigureAwait(false);

        return new PagedProductResult
        {
            Items = products.Select(p => new CatalogProductDto
            {
                Id = p.Id,
                SupplierAid = p.SupplierAid,
                ProductData = p.ProductData,
                CreatedAt = p.CreatedAt
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    /// <summary>
    /// Legacy method for BMEcat import - delegates to new ImportAsync
    /// </summary>
    public async Task<CatalogImportResponse> ImportBmecat(ImportBmecatRequest request)
    {
        try
        {
            var result = await ImportAsync(request.TenantId, request.CatalogStream, "bmecat").ConfigureAwait(false);
            return new CatalogImportResponse
            {
                Success = true,
                ImportId = result.Id,
                ProductCount = result.ProductCount,
                Message = $"Successfully imported {result.ProductCount} products"
            };
        }
        catch (Exception ex)
        {
            return new CatalogImportResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private ICatalogImportAdapter? GetAdapterForFormat(string format)
    {
        return format.ToLowerInvariant() switch
        {
            "bmecat" => _serviceProvider.GetService<ICatalogImportAdapter>(),
            "icecat" => null, // TODO: Implement IcecatImportAdapter
            _ => null
        };
    }

    private static CatalogImportDto MapToDto(CatalogImport import) => new()
    {
        Id = import.Id,
        SupplierId = import.SupplierId,
        CatalogId = import.CatalogId,
        ImportTimestamp = import.ImportTimestamp,
        Version = import.Version,
        Description = import.Description,
        Status = import.Status.ToString(),
        ProductCount = import.ProductCount,
        CreatedAt = import.CreatedAt,
        UpdatedAt = import.UpdatedAt
    };
}

/// <summary>
/// Request for BMEcat import (legacy)
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
    public List<string> ValidationErrors { get; set; } = [];
}

/// <summary>
/// DTO for catalog import
/// </summary>
public class CatalogImportDto
{
    public Guid Id { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string CatalogId { get; set; } = string.Empty;
    public DateTime ImportTimestamp { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for catalog product
/// </summary>
public class CatalogProductDto
{
    public Guid Id { get; set; }
    public string SupplierAid { get; set; } = string.Empty;
    public string ProductData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paged result for imports
/// </summary>
public class PagedImportResult
{
    public List<CatalogImportDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Paged result for products
/// </summary>
public class PagedProductResult
{
    public List<CatalogProductDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
