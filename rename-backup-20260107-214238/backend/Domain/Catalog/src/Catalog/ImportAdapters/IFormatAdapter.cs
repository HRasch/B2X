using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.Catalog.ImportAdapters;

/// <summary>
/// Interface for catalog import adapters supporting different file formats
/// (BMEcat, Datanorm, CSV, etc.)
/// </summary>
public interface IFormatAdapter
{
    /// <summary>
    /// Unique identifier for this format (e.g., "bmecat", "datanorm", "csv")
    /// </summary>
    string FormatId { get; }

    /// <summary>
    /// Human-friendly name for display (e.g., "BMEcat 2005.2")
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// List of supported file extensions (e.g., ".xml", ".txt", ".csv")
    /// </summary>
    IReadOnlyList<string> SupportedExtensions { get; }

    /// <summary>
    /// Detect if content matches this format.
    /// Called in order of priority to determine which adapter to use.
    /// </summary>
    /// <param name="content">File content as string</param>
    /// <param name="fileName">Original filename for extension-based detection</param>
    /// <returns>Detection confidence (0.0 = not this format, 1.0 = definitely this format)</returns>
    double DetectFormat(string content, string fileName);

    /// <summary>
    /// Validate content against format specification.
    /// Must be called before Parse().
    /// </summary>
    /// <param name="content">File content to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with errors/warnings if any</returns>
    Task<ValidationResult> ValidateAsync(
        string content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Parse and normalize content to catalog entities.
    /// Assume ValidateAsync() was called successfully.
    /// </summary>
    /// <param name="content">File content to parse</param>
    /// <param name="metadata">Import metadata (user, tenant, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed catalog entities</returns>
    Task<ParseResult> ParseAsync(
        string content,
        ImportMetadata metadata,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Metadata provided by the import request
/// </summary>
public record ImportMetadata(
    string TenantId,
    string? SupplierId = null,
    string? SourceIdentifier = null,
    Dictionary<string, string>? CustomProperties = null);

/// <summary>
/// Result of format validation
/// </summary>
public record ValidationResult(
    bool IsValid,
    IReadOnlyList<ValidationError> Errors = default!,
    IReadOnlyList<ValidationWarning> Warnings = default!)
{
    public ValidationResult()
        : this(true, [], []) { }

    public static ValidationResult Success() => new(true, [], []);

    public static ValidationResult WithErrors(params ValidationError[] errors)
        => new(false, errors, []);

    public static ValidationResult WithWarnings(params ValidationWarning[] warnings)
        => new(true, [], warnings);

    public static ValidationResult WithBoth(
        ValidationError[] errors,
        ValidationWarning[] warnings)
        => new(errors.Length == 0, errors, warnings);
}

/// <summary>
/// Single validation error that prevents import
/// </summary>
public record ValidationError(
    string Code,
    string Message,
    string? ElementPath = null,
    int? LineNumber = null,
    string? Suggestion = null);

/// <summary>
/// Single validation warning that allows import to continue
/// </summary>
public record ValidationWarning(
    string Code,
    string Message,
    string? ElementPath = null,
    int? LineNumber = null);

/// <summary>
/// Result of parsing content to catalog entities
/// </summary>
public record ParseResult(
    IReadOnlyList<CatalogEntity> Entities,
    ImportStatistics Statistics,
    IReadOnlyList<ParseWarning> Warnings = default!)
{
    public ParseResult(IReadOnlyList<CatalogEntity> entities, ImportStatistics statistics)
        : this(entities, statistics, []) { }
}

/// <summary>
/// Statistics about the import operation
/// </summary>
public record ImportStatistics(
    int TotalItems,
    int ValidItems,
    int SkippedItems,
    DateTime ImportedAt = default)
{
    public ImportStatistics(int totalItems, int validItems, int skippedItems)
        : this(totalItems, validItems, skippedItems, DateTime.UtcNow) { }
}

/// <summary>
/// Warning that occurred during parsing
/// </summary>
public record ParseWarning(
    string Code,
    string Message,
    string? ItemIdentifier = null,
    string? Suggestion = null);

/// <summary>
/// Normalized catalog entity (all formats map to this)
/// </summary>
public record CatalogEntity(
    string ExternalId,
    string SupplierId,
    string Name,
    string? Description = null,
    string? Ean = null,
    string? ManufacturerPartNumber = null,
    decimal? ListPrice = null,
    string? Currency = null,
    Dictionary<string, string>? Attributes = null,
    Dictionary<string, string>? CustomProperties = null);
