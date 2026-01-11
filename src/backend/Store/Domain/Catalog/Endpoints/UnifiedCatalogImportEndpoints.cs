using B2X.Catalog.ImportAdapters;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace B2X.Catalog.Endpoints;

/// <summary>
/// Unified Wolverine HTTP Endpoints for Catalog Import
/// Supports BMEcat, Datanorm, CSV, and extensible for additional formats
/// </summary>
public static class UnifiedCatalogImportEndpoints
{
    /// <summary>
    /// POST /api/v2/catalog/import
    /// Upload and auto-detect catalog format, then import
    /// 
    /// Supports:
    /// - BMEcat (XML) - versions 1.2, 2005, 2005.1, 2005.2
    /// - Datanorm (TXT) - German wholesale standard
    /// - CSV (CSV/TXT) - Comma/semicolon/tab separated with headers
    /// </summary>
    [WolverinePost("/api/v2/catalog/import")]
    public static async Task<IResult> ImportCatalogV2(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromForm] UnifiedCatalogImportRequest request,
        FormatRegistry formatRegistry,
        CancellationToken ct)
    {
        // Validate tenant
        if (tenantId == Guid.Empty)
            return Results.BadRequest(new { error = "X-Tenant-ID header is required" });

        // Validate file
        if (request.File == null || request.File.Length == 0)
            return Results.BadRequest(new { error = "File is required" });

        // Security: Limit file size (100MB)
        const long maxFileSize = 100 * 1024 * 1024;
        if (request.File.Length > maxFileSize)
            return Results.BadRequest(new { error = $"File exceeds maximum size of {maxFileSize / 1024 / 1024}MB" });

        try
        {
            // Read file content
            using var streamReader = new System.IO.StreamReader(request.File.OpenReadStream());
            var content = await streamReader.ReadToEndAsync(ct);

            // Auto-detect format or use explicit format if provided
            IFormatAdapter? adapter;
            if (!string.IsNullOrEmpty(request.Format))
            {
                adapter = formatRegistry.GetAdapterById(request.Format);
                if (adapter == null)
                {
                    return Results.BadRequest(new
                    {
                        error = $"Format '{request.Format}' not supported",
                        supportedFormats = formatRegistry.GetAllAdapters()
                            .Select(a => new { id = a.FormatId, name = a.FormatName })
                            .ToList()
                    });
                }
            }
            else
            {
                adapter = formatRegistry.DetectFormat(content, request.File.FileName);
                if (adapter == null)
                {
                    return Results.BadRequest(new
                    {
                        error = "Unable to detect catalog format. Please specify 'format' parameter",
                        supportedFormats = formatRegistry.GetAllAdapters()
                            .Select(a => new { id = a.FormatId, name = a.FormatName, extensions = a.SupportedExtensions })
                            .ToList()
                    });
                }
            }

            // Validate content
            var validationResult = await adapter.ValidateAsync(content, ct);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new
                {
                    error = "Validation failed",
                    format = adapter.FormatId,
                    errors = validationResult.Errors
                        .Select(e => new { code = e.Code, message = e.Message, suggestion = e.Suggestion })
                        .ToList(),
                    warnings = validationResult.Warnings
                        .Select(w => new { code = w.Code, message = w.Message })
                        .ToList()
                });
            }

            // Parse content
            var metadata = new ImportMetadata(
                TenantId: tenantId.ToString(),
                SupplierId: request.SupplierId,
                SourceIdentifier: request.SourceIdentifier,
                CustomProperties: request.CustomProperties);

            var parseResult = await adapter.ParseAsync(content, metadata, ct);

            return Results.Ok(new
            {
                success = true,
                format = adapter.FormatId,
                formatName = adapter.FormatName,
                statistics = new
                {
                    totalItems = parseResult.Statistics.TotalItems,
                    validItems = parseResult.Statistics.ValidItems,
                    skippedItems = parseResult.Statistics.SkippedItems,
                    importedAt = parseResult.Statistics.ImportedAt
                },
                entities = parseResult.Entities
                    .Select(e => new
                    {
                        externalId = e.ExternalId,
                        supplierId = e.SupplierId,
                        name = e.Name,
                        description = e.Description,
                        ean = e.Ean,
                        manufacturerPartNumber = e.ManufacturerPartNumber,
                        listPrice = e.ListPrice,
                        currency = e.Currency
                    })
                    .ToList(),
                warnings = parseResult.Warnings
                    .Select(w => new { code = w.Code, message = w.Message, itemId = w.ItemIdentifier })
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Import failed");
        }
    }

    /// <summary>
    /// GET /api/v2/catalog/import/formats
    /// List all supported import formats
    /// </summary>
    [WolverineGet("/api/v2/catalog/import/formats")]
    public static IResult GetSupportedFormats(
        FormatRegistry formatRegistry)
    {
        var formats = formatRegistry.GetAllAdapters()
            .Select(a => new
            {
                id = a.FormatId,
                name = a.FormatName,
                extensions = a.SupportedExtensions,
                description = GetFormatDescription(a.FormatId)
            })
            .ToList();

        return Results.Ok(new
        {
            supportedFormats = formats,
            total = formats.Count
        });
    }

    /// <summary>
    /// POST /api/v2/catalog/import/validate
    /// Validate a catalog file without importing
    /// </summary>
    [WolverinePost("/api/v2/catalog/import/validate")]
    public static async Task<IResult> ValidateCatalog(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromForm] UnifiedCatalogImportRequest request,
        FormatRegistry formatRegistry,
        CancellationToken ct)
    {
        if (tenantId == Guid.Empty)
            return Results.BadRequest(new { error = "X-Tenant-ID header is required" });

        if (request.File == null || request.File.Length == 0)
            return Results.BadRequest(new { error = "File is required" });

        try
        {
            using var streamReader = new System.IO.StreamReader(request.File.OpenReadStream());
            var content = await streamReader.ReadToEndAsync(ct);

            // Detect or use specified format
            IFormatAdapter? adapter = string.IsNullOrEmpty(request.Format)
                ? formatRegistry.DetectFormat(content, request.File.FileName)
                : formatRegistry.GetAdapterById(request.Format);

            if (adapter == null)
                return Results.BadRequest(new { error = "Unable to detect or invalid format specified" });

            // Validate
            var result = await adapter.ValidateAsync(content, ct);

            return Results.Ok(new
            {
                format = adapter.FormatId,
                isValid = result.IsValid,
                errors = result.Errors
                    .Select(e => new { code = e.Code, message = e.Message, line = e.LineNumber })
                    .ToList(),
                warnings = result.Warnings
                    .Select(w => new { code = w.Code, message = w.Message })
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Validation failed");
        }
    }

    private static string GetFormatDescription(string formatId) => formatId switch
    {
        "bmecat" => "BMEcat XML standard for product catalogs (v1.2, 2005, 2005.1, 2005.2)",
        "datanorm" => "Datanorm text format for German wholesale/retail trade",
        "csv" => "CSV format with flexible column headers",
        _ => "Catalog format"
    };
}

/// <summary>
/// Request model for unified catalog import
/// </summary>
public class UnifiedCatalogImportRequest
{
    public IFormFile File { get; set; } = null!;
    public string? Format { get; set; }
    public string? SupplierId { get; set; }
    public string? SourceIdentifier { get; set; }
    public Dictionary<string, string>? CustomProperties { get; set; }
}
