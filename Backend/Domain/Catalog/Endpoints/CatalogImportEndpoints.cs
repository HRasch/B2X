using B2Connect.Catalog.Application.Handlers;
using B2Connect.Catalog.Models;
using B2Connect.Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace B2Connect.Catalog.Endpoints;

/// <summary>
/// Wolverine HTTP Endpoints for Catalog Import (BMEcat, icecat)
/// Handles file upload and processing of supplier catalogs
/// </summary>
public static class CatalogImportEndpoints
{
    /// <summary>
    /// POST /api/catalog/import
    /// Upload and process a catalog file (BMEcat XML, etc.)
    /// </summary>
    [WolverinePost("/api/catalog/import")]
    public static async Task<IResult> ImportCatalog(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromForm] CatalogImportRequest request,
        CatalogImportService catalogImportService,
        CancellationToken ct)
    {
        // Validate tenant
        if (tenantId == Guid.Empty)
        {
            return Results.BadRequest(new { Message = "X-Tenant-ID header is required" });
        }

        // Validate file
        if (request.File == null || request.File.Length == 0)
        {
            return Results.BadRequest(new { Message = "File is required" });
        }

        // Security: Limit file size (50MB)
        const long maxFileSize = 50 * 1024 * 1024;
        if (request.File.Length > maxFileSize)
        {
            return Results.BadRequest(new { Message = $"File size exceeds maximum allowed ({maxFileSize / 1024 / 1024}MB)" });
        }

        // Detect format from file extension or content type
        var format = DetectCatalogFormat(request.File.FileName, request.File.ContentType, request.Format);
        if (string.IsNullOrEmpty(format))
        {
            return Results.BadRequest(new { Message = "Unable to determine catalog format. Please specify 'format' parameter (bmecat, icecat)" });
        }

        try
        {
            using var stream = request.File.OpenReadStream();
            var result = await catalogImportService.ImportAsync(tenantId, stream, format, request.CustomSchemaPath, ct);

            return Results.Ok(new CatalogImportResponse
            {
                ImportId = result.Id,
                Status = result.Status.ToString(),
                SupplierId = result.SupplierId,
                CatalogId = result.CatalogId,
                ProductCount = result.ProductCount,
                ImportTimestamp = result.ImportTimestamp,
                Message = $"Successfully imported {result.ProductCount} products from {result.SupplierId}"
            });
        }
        catch (NotSupportedException ex)
        {
            return Results.BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.UnprocessableEntity(new { Message = $"Failed to parse catalog: {ex.Message}" });
        }
    }

    /// <summary>
    /// GET /api/catalog/imports
    /// List all catalog imports for a tenant
    /// </summary>
    [WolverineGet("/api/catalog/imports")]
    public static async Task<IResult> ListImports(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CatalogImportService catalogImportService = null!,
        CancellationToken ct = default)
    {
        if (tenantId == Guid.Empty)
        {
            return Results.BadRequest(new { Message = "X-Tenant-ID header is required" });
        }

        var imports = await catalogImportService.GetImportsAsync(tenantId, page, pageSize, ct);
        return Results.Ok(imports);
    }

    /// <summary>
    /// GET /api/catalog/imports/{id}
    /// Get details of a specific catalog import
    /// </summary>
    [WolverineGet("/api/catalog/imports/{id:guid}")]
    public static async Task<IResult> GetImport(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CatalogImportService catalogImportService,
        CancellationToken ct)
    {
        if (tenantId == Guid.Empty)
        {
            return Results.BadRequest(new { Message = "X-Tenant-ID header is required" });
        }

        var import = await catalogImportService.GetImportAsync(tenantId, id, ct);
        if (import == null)
        {
            return Results.NotFound(new { Message = $"Import with ID '{id}' not found" });
        }

        return Results.Ok(import);
    }

    /// <summary>
    /// GET /api/catalog/imports/{id}/products
    /// Get products from a specific catalog import
    /// </summary>
    [WolverineGet("/api/catalog/imports/{id:guid}/products")]
    public static async Task<IResult> GetImportProducts(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CatalogImportService catalogImportService = null!,
        CancellationToken ct = default)
    {
        if (tenantId == Guid.Empty)
        {
            return Results.BadRequest(new { Message = "X-Tenant-ID header is required" });
        }

        var products = await catalogImportService.GetImportProductsAsync(tenantId, id, page, pageSize, ct);
        return Results.Ok(products);
    }

    /// <summary>
    /// GET /api/catalog/formats
    /// List supported catalog formats
    /// </summary>
    [WolverineGet("/api/catalog/formats")]
    public static IResult ListSupportedFormats()
    {
        return Results.Ok(new
        {
            Formats = new[]
            {
                new { Code = "bmecat", Name = "BMEcat", Description = "BMEcat XML format for B2B product catalogs", FileExtensions = new[] { ".xml", ".bmecat" } },
                new { Code = "icecat", Name = "Icecat", Description = "Icecat XML format for product data", FileExtensions = new[] { ".xml" } }
            }
        });
    }

    private static string? DetectCatalogFormat(string fileName, string contentType, string? explicitFormat)
    {
        // If format explicitly specified, use it
        if (!string.IsNullOrEmpty(explicitFormat))
        {
            return explicitFormat.ToLowerInvariant() switch
            {
                "bmecat" => "bmecat",
                "icecat" => "icecat",
                _ => null
            };
        }

        // Try to detect from file extension
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        if (extension == ".bmecat")
        {
            return "bmecat";
        }

        // Default to bmecat for XML files (most common in B2B)
        if (extension == ".xml" || contentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            return "bmecat";
        }

        return null;
    }
}

/// <summary>
/// Request model for catalog import
/// </summary>
public class CatalogImportRequest
{
    /// <summary>
    /// The catalog file to import
    /// </summary>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Optional: Explicit format specification (bmecat, icecat)
    /// If not specified, format is detected from file extension/content
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Optional: Path to custom XSD schema file for validation
    /// If not specified, built-in BMEcat schemas are used
    /// </summary>
    public string? CustomSchemaPath { get; set; }
}

/// <summary>
/// Response model for catalog import
/// </summary>
public class CatalogImportResponse
{
    /// <summary>
    /// ID of the import record
    /// </summary>
    public Guid ImportId { get; set; }

    /// <summary>
    /// Status of the import (Pending, Processing, Completed, Failed)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Supplier ID from the catalog
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// Catalog ID from the catalog
    /// </summary>
    public string CatalogId { get; set; } = string.Empty;

    /// <summary>
    /// Number of products imported
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Timestamp of the import
    /// </summary>
    public DateTime ImportTimestamp { get; set; }

    /// <summary>
    /// Human-readable message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
