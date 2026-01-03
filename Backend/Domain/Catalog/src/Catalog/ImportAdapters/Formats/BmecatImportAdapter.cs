using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B2Connect.Catalog.ImportAdapters;
using B2Connect.Catalog.ImportAdapters.Models;

namespace B2Connect.Catalog.ImportAdapters.Formats;

/// <summary>
/// Import adapter for BMEcat format (Hybrid Approach)
/// 
/// Uses two-layer validation:
/// 1. XSD Schema validation (detailed error reporting with line numbers)
/// 2. XmlSerializer deserialization (type-safe parsing)
///
/// Supports BMEcat 1.2, 2005, 2005.1, and 2005.2 standards for product catalog import
/// See .ai/knowledgebase/bmecat.md for complete documentation
/// </summary>
public class BmecatImportAdapter : IFormatAdapter
{
    private readonly ILogger<BmecatImportAdapter> _logger;
    private readonly XmlSerializerFactory _serializerFactory = new();

    public string FormatId => "bmecat";
    public string FormatName => "BMEcat";
    public IReadOnlyList<string> SupportedExtensions => new[] { ".xml" };

    private static readonly Dictionary<string, string> SchemaPathsByVersion = new()
    {
        { "1.2", "schemas/bmecat-1-2.xsd" },
        { "2005", "schemas/bmecat-2005.xsd" },
        { "2005.1", "schemas/bmecat-2005-1.xsd" },
        { "2005.2", "schemas/bmecat-2005-2.xsd" }
    };

    public BmecatImportAdapter(ILogger<BmecatImportAdapter> logger)
    {
        _logger = logger;
    }

    public double DetectFormat(string content, string fileName)
    {
        // Extension-based detection
        if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            // Content-based confirmation
            if (content.Contains("<BMECAT", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("bmecat", StringComparison.OrdinalIgnoreCase))
                return 0.95; // Very confident

            return 0.5; // Uncertain - is XML but may not be BMEcat
        }

        // Content-based detection without extension
        if (content.Contains("<BMECAT", StringComparison.OrdinalIgnoreCase))
            return 0.9; // Strong indicator

        return 0.0;
    }

    public async Task<ValidationResult> ValidateAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        try
        {
            // Layer 1: Detect version
            var version = DetectBmecatVersion(content);
            if (version == null)
            {
                errors.Add(new ValidationError(
                    "BMECAT_VERSION_NOT_DETECTED",
                    "Unable to detect BMEcat version from XML content",
                    ElementPath: "/BMECAT"));
                return ValidationResult.WithErrors(errors.ToArray());
            }

            // Layer 2: Schema validation (detailed error reporting)
            var schemaValidationResult = await ValidateWithSchemaAsync(content, version, cancellationToken);
            errors.AddRange(schemaValidationResult.Errors);
            warnings.AddRange(schemaValidationResult.Warnings);

            if (errors.Any())
                return ValidationResult.WithBoth(errors.ToArray(), warnings.ToArray());

            // Layer 3: Deserialize with XmlSerializer (type-safe validation)
            var deserializationResult = await DeserializeAndValidateAsync(content, cancellationToken);
            errors.AddRange(deserializationResult.Errors);
            warnings.AddRange(deserializationResult.Warnings);

            return ValidationResult.WithBoth(errors.ToArray(), warnings.ToArray());
        }
        catch (XmlException ex)
        {
            errors.Add(new ValidationError(
                "BMECAT_XML_PARSE_ERROR",
                $"XML parsing error: {ex.Message}",
                LineNumber: ex.LineNumber,
                Suggestion: "Check XML structure and encoding"));
            return ValidationResult.WithErrors(errors.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating BMEcat content");
            errors.Add(new ValidationError(
                "BMECAT_VALIDATION_ERROR",
                $"Validation error: {ex.Message}"));
            return ValidationResult.WithErrors(errors.ToArray());
        }
    }

    public async Task<ParseResult> ParseAsync(
        string content,
        ImportMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var entities = new List<CatalogEntity>();
        var warnings = new List<ParseWarning>();
        var validItems = 0;
        var skippedItems = 0;

        try
        {
            // Deserialize document
            var doc = await DeserializeDocumentAsync(content, cancellationToken);
            if (doc == null)
            {
                return new ParseResult(entities, new ImportStatistics(0, 0, 0), warnings);
            }

            // Extract supplier ID from header
            var supplierId = doc.Header?.SupplierId ?? metadata.SourceIdentifier ?? "unknown";

            // Parse header metadata
            var catalogName = doc.Header?.CatalogName ?? "BMEcat Catalog";
            var catalogVersion = doc.Header?.CatalogVersion ?? "1.0";

            // Parse articles
            foreach (var article in doc.Articles)
            {
                try
                {
                    var entity = MapArticleToCatalogEntity(article, supplierId, metadata);
                    if (entity != null)
                    {
                        entities.Add(entity);
                        validItems++;
                    }
                    else
                    {
                        skippedItems++;
                        warnings.Add(new ParseWarning(
                            Code: "ARTICLE_SKIPPED",
                            Message: $"Article {article.Details?.ArticleNumber} could not be mapped",
                            ItemIdentifier: article.Details?.ArticleNumber ?? "unknown"));
                    }
                }
                catch (Exception ex)
                {
                    skippedItems++;
                    warnings.Add(new ParseWarning(
                        Code: "ARTICLE_PARSE_ERROR",
                        Message: $"Error parsing article: {ex.Message}",
                        ItemIdentifier: article.Details?.ArticleNumber ?? "unknown"));
                }
            }

            var statistics = new ImportStatistics(
                entities.Count + skippedItems,
                validItems,
                skippedItems);

            return new ParseResult(entities, statistics, warnings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing BMEcat content");
            var statistics = new ImportStatistics(0, 0, 0);
            return new ParseResult(entities, statistics, new[] {
                new ParseWarning("PARSE_ERROR", ex.Message, null)
            });
        }
    }

    /// <summary>
    /// Layer 1: Schema validation with detailed error reporting
    /// </summary>
    private async Task<ValidationResult> ValidateWithSchemaAsync(
        string content,
        string version,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        try
        {
            // Load schema for this version
            if (!SchemaPathsByVersion.TryGetValue(version, out var schemaPath))
            {
                errors.Add(new ValidationError(
                    "BMECAT_SCHEMA_NOT_FOUND",
                    $"No XSD schema available for BMEcat version {version}",
                    ElementPath: "/BMECAT/@version",
                    Suggestion: $"Supported versions: {string.Join(", ", SchemaPathsByVersion.Keys)}"));
                return ValidationResult.WithErrors(errors.ToArray());
            }

            var schemaSet = new XmlSchemaSet();

            // Load embedded schema resource (or file system)
            var schemaContent = LoadSchemaContent(schemaPath);
            if (schemaContent == null)
            {
                warnings.Add(new ValidationWarning(
                    Code: "SCHEMA_NOT_AVAILABLE",
                    Message: $"XSD schema for version {version} not available - skipping schema validation"));
                return ValidationResult.WithWarnings(warnings.ToArray());
            }

            using var schemaReader = new System.IO.StringReader(schemaContent);
            var schema = XmlSchema.Read(schemaReader, SchemaValidationHandler);
            if (schema != null)
            {
                schemaSet.Add(schema);
            }

            // Validate document
            var doc = new XmlDocument();
            doc.LoadXml(content);

            var validationEventHandler = new ValidationEventHandler((sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Error)
                {
                    errors.Add(new ValidationError(
                        "BMECAT_SCHEMA_VIOLATION",
                        args.Message,
                        LineNumber: args.Exception?.LineNumber ?? 0,
                        ElementPath: args.Exception?.SourceUri ?? "/BMECAT"));
                }
                else
                {
                    warnings.Add(new ValidationWarning(
                        Code: "BMECAT_SCHEMA_WARNING",
                        Message: args.Message));
                }
            });

            doc.Schemas.Add(schemaSet);
            doc.Validate(validationEventHandler);

            return ValidationResult.WithBoth(errors.ToArray(), warnings.ToArray());
        }
        catch (Exception ex)
        {
            errors.Add(new ValidationError(
                "BMECAT_SCHEMA_VALIDATION_ERROR",
                $"Schema validation failed: {ex.Message}"));
            return ValidationResult.WithErrors(errors.ToArray());
        }
    }

    /// <summary>
    /// Layer 2: XmlSerializer deserialization with type-safe validation
    /// </summary>
    private async Task<ValidationResult> DeserializeAndValidateAsync(
        string content,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        try
        {
            var doc = await DeserializeDocumentAsync(content, cancellationToken);
            if (doc == null)
            {
                errors.Add(new ValidationError(
                    "BMECAT_DESERIALIZATION_FAILED",
                    "Failed to deserialize BMEcat document"));
                return ValidationResult.WithErrors(errors.ToArray());
            }

            // Validate required elements
            if (doc.Header == null)
            {
                errors.Add(new ValidationError(
                    "BMECAT_MISSING_HEADER",
                    "Document must contain a HEADER element",
                    ElementPath: "/BMECAT/HEADER"));
            }

            if (!doc.Articles.Any())
            {
                errors.Add(new ValidationError(
                    "BMECAT_NO_ARTICLES",
                    "Document must contain at least one ARTICLE element",
                    ElementPath: "/BMECAT"));
            }

            // Validate article structure
            foreach (var article in doc.Articles)
            {
                if (article.Details == null || string.IsNullOrEmpty(article.Details.ArticleNumber))
                {
                    warnings.Add(new ValidationWarning(
                        Code: "ARTICLE_MISSING_NUMBER",
                        Message: "Article missing ARTICLE_NUMBER"));
                }

                if (string.IsNullOrEmpty(article.Details?.Description))
                {
                    warnings.Add(new ValidationWarning(
                        Code: "ARTICLE_MISSING_DESCRIPTION",
                        Message: $"Article {article.Details?.ArticleNumber} missing DESCRIPTION"));
                }
            }

            return ValidationResult.WithBoth(errors.ToArray(), warnings.ToArray());
        }
        catch (InvalidOperationException ex)
        {
            errors.Add(new ValidationError(
                "BMECAT_DESERIALIZATION_ERROR",
                $"XML structure error: {ex.Message}",
                Suggestion: "Check that XML element names and nesting match BMEcat schema"));
            return ValidationResult.WithErrors(errors.ToArray());
        }
        catch (Exception ex)
        {
            errors.Add(new ValidationError(
                "BMECAT_VALIDATION_ERROR",
                $"Validation error: {ex.Message}"));
            return ValidationResult.WithErrors(errors.ToArray());
        }
    }

    /// <summary>
    /// Deserialize BMEcat document using XmlSerializer
    /// </summary>
    private async Task<BmecatDocument?> DeserializeDocumentAsync(
        string content,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            try
            {
                var serializer = _serializerFactory.CreateSerializer<BmecatDocument>();
                using var reader = new System.IO.StringReader(content);
                return serializer.Deserialize(reader) as BmecatDocument;
            }
            catch
            {
                return null;
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Map BMEcat article to canonical CatalogEntity
    /// </summary>
    private CatalogEntity? MapArticleToCatalogEntity(
        BmecatArticle article,
        string supplierId,
        ImportMetadata metadata)
    {
        if (article.Details == null)
            return null;

        var details = article.Details;
        var name = details.Description ?? details.ArticleNumber ?? "Unknown";
        var externalId = details.ArticleNumber ?? article.SupplierId ?? Guid.NewGuid().ToString();

        // Extract price from price details
        decimal? price = null;
        string? currency = null;

        if (article.PriceDetails?.Prices.Any() == true)
        {
            var defaultPrice = article.PriceDetails.Prices.FirstOrDefault(p => p.PriceType == "net_price")
                ?? article.PriceDetails.Prices.FirstOrDefault();

            if (defaultPrice != null)
            {
                price = defaultPrice.PriceAmount;
                currency = defaultPrice.PriceCurrency ?? "EUR";
            }
        }

        var attributes = new Dictionary<string, string>();

        // Add keywords as attributes
        if (details.Keywords.Any())
            attributes["keywords"] = string.Join(", ", details.Keywords);

        // Add manufacturer information
        if (!string.IsNullOrEmpty(details.Manufacturer))
            attributes["manufacturer"] = details.Manufacturer;

        if (!string.IsNullOrEmpty(details.ManufacturerAid))
            attributes["manufacturer_part_number"] = details.ManufacturerAid;

        // Add features
        if (article.Features?.Features.Any() == true)
        {
            foreach (var feature in article.Features.Features)
            {
                if (!string.IsNullOrEmpty(feature.Name))
                    attributes[$"feature_{feature.Name}"] = feature.Value ?? "";
            }
        }

        // Add remarks
        if (!string.IsNullOrEmpty(details.Remarks))
            attributes["remarks"] = details.Remarks;

        return new CatalogEntity(
            ExternalId: externalId,
            SupplierId: supplierId,
            Name: name,
            Description: details.Remarks,
            Ean: details.Ean,
            ManufacturerPartNumber: details.ManufacturerAid,
            ListPrice: price,
            Currency: currency ?? "EUR",
            Attributes: attributes);
    }

    /// <summary>
    /// Detect BMEcat version from XML content
    /// </summary>
    private string? DetectBmecatVersion(string content)
    {
        // Try to extract version attribute
        var match = System.Text.RegularExpressions.Regex.Match(
            content,
            @"<BMECAT\s+version=""([^""]+)""",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (match.Success)
            return match.Groups[1].Value;

        // Fallback detection by content patterns
        if (content.Contains("xmlns:bmecat", StringComparison.OrdinalIgnoreCase))
            return "2005"; // 2005+ uses xmlns

        // Default to latest
        return "2005.2";
    }

    /// <summary>
    /// Load schema content (from resources or file system)
    /// Returns null if schema not found
    /// </summary>
    private string? LoadSchemaContent(string schemaPath)
    {
        try
        {
            // Try loading from embedded resource
            var assembly = typeof(BmecatImportAdapter).Assembly;
            var resourceName = $"B2Connect.Catalog.{schemaPath}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new System.IO.StreamReader(stream);
                return reader.ReadToEnd();
            }

            // Try loading from file system
            var fullPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(assembly.Location) ?? "",
                schemaPath);

            if (System.IO.File.Exists(fullPath))
                return System.IO.File.ReadAllText(fullPath);

            return null;
        }
        catch
        {
            return null;
        }
    }

    private void SchemaValidationHandler(object? sender, ValidationEventArgs args)
    {
        // Handler for schema loading validation events
        if (args.Severity == XmlSeverityType.Error)
            _logger.LogWarning("Schema validation event: {Message}", args.Message);
    }
}

/// <summary>
/// Factory for creating XmlSerializer instances
/// </summary>
internal class XmlSerializerFactory
{
    private readonly Dictionary<Type, XmlSerializer> _cache = new();

    public XmlSerializer CreateSerializer<T>()
    {
        var type = typeof(T);
        if (!_cache.TryGetValue(type, out var serializer))
        {
            serializer = new XmlSerializer(type);
            _cache[type] = serializer;
        }
        return serializer;
    }
}
