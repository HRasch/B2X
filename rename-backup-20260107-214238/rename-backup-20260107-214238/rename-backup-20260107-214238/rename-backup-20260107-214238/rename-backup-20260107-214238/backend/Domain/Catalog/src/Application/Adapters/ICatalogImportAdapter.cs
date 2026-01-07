using B2Connect.Catalog.Core.Entities;

namespace B2Connect.Catalog.Application.Adapters;

/// <summary>
/// Adapter interface for importing product catalogs from various formats
/// Follows Adapter Pattern to support multiple catalog formats (BMEcat, CSV, DATANORM)
/// </summary>
public interface ICatalogImportAdapter
{
    /// <summary>
    /// Supported catalog format (e.g., "bmecat", "csv", "datanorm")
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Import catalog from stream
    /// </summary>
    /// <param name="catalogStream">Stream containing the catalog data</param>
    /// <param name="metadata">Catalog metadata for identification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result with success/failure and product count</returns>
    Task<CatalogImportResult> ImportAsync(
        Stream catalogStream,
        CatalogMetadata metadata,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a catalog import operation
/// </summary>
public class CatalogImportResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int ProductCount { get; set; }
    public Guid ImportId { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public List<CatalogProduct> Products { get; set; } = new();
    public string? SupplierId { get; set; }
    public string? CatalogId { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
}
/// <summary>
/// Metadata for catalog identification and processing
/// </summary>
public class CatalogMetadata
{
    public Guid TenantId { get; set; }
    public string? SupplierId { get; set; }
    public string? CatalogId { get; set; }
    public DateTime ImportTimestamp { get; set; } = DateTime.UtcNow;
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string? CustomSchemaPath { get; set; }
}
