using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Catalog.ImportAdapters;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.ImportAdapters.Formats;

/// <summary>
/// Import adapter for Datanorm format (German product catalog standard)
/// 
/// Datanorm is a text-based format used in German wholesale and retail trade.
/// Records are separated by record type codes (0-9).
/// 
/// Record Types:
/// 0 = Header (document info)
/// 1 = Article master data
/// 2 = Pricing information
/// 3 = Availability
/// 4 = Descriptive texts
/// 8 = Notes/comments
/// 9 = Footer (end of document)
///
/// See: https://www.ean-search.org/datanorm/ (Reference)
/// </summary>
public class DatanormImportAdapter : IFormatAdapter
{
    private readonly ILogger<DatanormImportAdapter> _logger;

    public string FormatId => "datanorm";
    public string FormatName => "Datanorm";
    public IReadOnlyList<string> SupportedExtensions => new[] { ".txt", ".dn", ".datanorm" };

    public DatanormImportAdapter(ILogger<DatanormImportAdapter> logger)
    {
        _logger = logger;
    }

    public double DetectFormat(string content, string fileName)
    {
        // Extension-based detection
        if (SupportedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            return 0.7; // Good confidence from extension

        // Content-based detection: Look for Datanorm record markers at line starts
        // Pattern: First char is digit (record type), followed by fixed-width fields
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (lines.Length < 2)
            return 0.0;

        var datanormLines = 0;
        foreach (var line in lines.Take(100)) // Check first 100 lines
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Datanorm records start with a single digit (0-9)
            if (char.IsDigit(line[0]) && line.Length > 10)
                datanormLines++;
        }

        // If >50% of lines match pattern, likely Datanorm
        var confidence = (double)datanormLines / Math.Min(100, lines.Length);
        return confidence > 0.5 ? confidence * 0.9 : 0.0;
    }

    public Task<ValidationResult> ValidateAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // Check minimum structure
        if (!lines.Any(l => l.StartsWith('0')))
        {
            errors.Add(new ValidationError(
                "DATANORM_MISSING_HEADER",
                "Datanorm file must start with record type 0 (header)",
                LineNumber: 1));
        }

        if (!lines.Any(l => l.StartsWith('9')))
        {
            warnings.Add(new ValidationWarning(
                "DATANORM_MISSING_FOOTER",
                "Datanorm file should end with record type 9 (footer)"));
        }

        // Check for article records (type 1)
        var articleCount = lines.Count(l => l.StartsWith('1'));
        if (articleCount == 0)
        {
            errors.Add(new ValidationError(
                "DATANORM_NO_ARTICLES",
                "Datanorm file must contain at least one article record (type 1)"));
        }

        // Validate record structure
        int lineNum = 1;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                lineNum++;
                continue;
            }

            // Record type validation
            if (!char.IsDigit(line[0]))
            {
                errors.Add(new ValidationError(
                    "DATANORM_INVALID_RECORD",
                    "Record must start with digit (record type 0-9)",
                    LineNumber: lineNum));
            }

            // Minimum line length check (most records have fixed minimum width)
            if (line.Length < 5)
            {
                warnings.Add(new ValidationWarning(
                    "DATANORM_SHORT_RECORD",
                    "Record appears truncated (very short line)",
                    LineNumber: lineNum));
            }

            lineNum++;
        }

        var result = ValidationResult.WithBoth(errors.ToArray(), warnings.ToArray());
        return Task.FromResult(result);
    }

    public Task<ParseResult> ParseAsync(
        string content,
        ImportMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var entities = new List<CatalogEntity>();
        var warnings = new List<ParseWarning>();

        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        string? currentSupplierId = metadata.SupplierId;
        var currentArticle = new Dictionary<string, string>();
        var validItems = 0;
        var skippedItems = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var recordType = line[0];

                switch (recordType)
                {
                    case '0': // Header
                        ParseHeaderRecord(line, ref currentSupplierId);
                        break;

                    case '1': // Article master data
                        // Save previous article if exists
                        if (currentArticle.Count > 0)
                        {
                            if (TryBuildEntity(currentArticle, metadata, currentSupplierId, out var entity))
                            {
                                entities.Add(entity);
                                validItems++;
                            }
                            else
                            {
                                skippedItems++;
                            }
                        }

                        currentArticle = ParseArticleRecord(line);
                        break;

                    case '2': // Pricing
                        if (currentArticle.Count > 0)
                            ParsePricingRecord(line, currentArticle);
                        break;

                    case '4': // Descriptive text
                        if (currentArticle.Count > 0)
                            ParseDescriptionRecord(line, currentArticle);
                        break;

                    case '9': // Footer
                        // Save last article
                        if (currentArticle.Count > 0)
                        {
                            if (TryBuildEntity(currentArticle, metadata, currentSupplierId, out var entity))
                            {
                                entities.Add(entity);
                                validItems++;
                            }
                            else
                            {
                                skippedItems++;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing Datanorm record: {Record}", line[..Math.Min(50, line.Length)]);
                skippedItems++;
            }
        }

        var statistics = new ImportStatistics(
            entities.Count + skippedItems,
            validItems,
            skippedItems);

        return Task.FromResult(new ParseResult(entities, statistics, warnings));
    }

    private static void ParseHeaderRecord(string line, ref string? supplierId)
    {
        // Record type 0: Header
        // Format varies by implementation, typically contains supplier info
        // For now, we just note it was parsed
    }

    private static Dictionary<string, string> ParseArticleRecord(string line)
    {
        // Record type 1: Article master data
        // Typical format (may vary):
        // Position 1-2: Record type (1)
        // Position 3-12: EAN code
        // Position 13-30: Supplier article number
        // Position 31-90: Article name/description
        // Position 91-98: Supplier code

        var article = new Dictionary<string, string>();

        try
        {
            if (line.Length > 12)
                article["ean"] = ExtractField(line, 2, 10).Trim();

            if (line.Length > 30)
                article["mfg_part_number"] = ExtractField(line, 12, 18).Trim();

            if (line.Length > 90)
                article["name"] = ExtractField(line, 30, 60).Trim();

            if (line.Length > 98)
                article["supplier_code"] = ExtractField(line, 90, 8).Trim();
        }
        catch
        {
            // If parsing fails, just return what we got
        }

        return article;
    }

    private static void ParsePricingRecord(string line, Dictionary<string, string> article)
    {
        // Record type 2: Pricing
        // Position 3-14: Net price (numeric with decimal)
        // Position 15-16: Currency code

        try
        {
            if (line.Length > 14)
            {
                var priceStr = ExtractField(line, 2, 12).Trim();
                if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                    article["list_price"] = price.ToString(CultureInfo.InvariantCulture);
            }

            if (line.Length > 16)
                article["currency"] = ExtractField(line, 14, 2).Trim();
        }
        catch
        {
            // Graceful degradation
        }
    }

    private static void ParseDescriptionRecord(string line, Dictionary<string, string> article)
    {
        // Record type 4: Descriptive text
        // Typically contains longer description/text fields
        try
        {
            if (line.Length > 2)
            {
                var description = line[2..].Trim();
                if (description.Length > 0)
                    article["description"] = description;
            }
        }
        catch
        {
            // Graceful degradation
        }
    }

    private static bool TryBuildEntity(
        Dictionary<string, string> article,
        ImportMetadata metadata,
        string? supplierId,
        out CatalogEntity entity)
    {
        entity = null!;

        // Require at least EAN or manufacturer part number as external ID
        var externalId = article.GetValueOrDefault("ean") ??
                        article.GetValueOrDefault("mfg_part_number");

        if (string.IsNullOrWhiteSpace(externalId))
            return false;

        var name = article.GetValueOrDefault("name") ?? "Unknown";

        // Parse optional price
        decimal? price = null;
        if (article.TryGetValue("list_price", out var priceStr) &&
            decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
            price = p;

        entity = new CatalogEntity(
            ExternalId: externalId,
            SupplierId: supplierId ?? metadata.TenantId,
            Name: name,
            Description: article.GetValueOrDefault("description"),
            Ean: article.GetValueOrDefault("ean"),
            ManufacturerPartNumber: article.GetValueOrDefault("mfg_part_number"),
            ListPrice: price,
            Currency: article.GetValueOrDefault("currency", "EUR"),
            Attributes: article);

        return true;
    }

    private static string ExtractField(string line, int startIndex, int length)
    {
        if (startIndex >= line.Length)
            return string.Empty;

        var endIndex = Math.Min(startIndex + length, line.Length);
        return line[startIndex..endIndex];
    }
}
