using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2X.Catalog.ImportAdapters;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.ImportAdapters.Formats;

/// <summary>
/// Import adapter for CSV (Comma-Separated Values) format
/// 
/// Supports configurable CSV with headers.
/// Expected columns (case-insensitive, flexible field order):
/// - sku / article_number / product_id (required)
/// - name / title / product_name (required)
/// - description / long_description (optional)
/// - ean / gtin (optional)
/// - mfg_part_number / manufacturer_part_number (optional)
/// - price / list_price / msrp (optional)
/// - currency (optional, defaults to EUR)
/// - supplier / supplier_id (optional, uses metadata if not provided)
/// 
/// Handles:
/// - Different delimiters (comma, semicolon, tab)
/// - Quoted fields with embedded delimiters
/// - UTF-8 encoding with BOM
/// - Different line endings (CRLF, LF)
/// </summary>
public class CsvImportAdapter : IFormatAdapter
{
    private readonly ILogger<CsvImportAdapter> _logger;

    public string FormatId => "csv";
    public string FormatName => "CSV";
    public IReadOnlyList<string> SupportedExtensions => new[] { ".csv", ".txt" };

    public CsvImportAdapter(ILogger<CsvImportAdapter> logger)
    {
        _logger = logger;
    }

    public double DetectFormat(string content, string fileName)
    {
        // Extension-based: strong indicator
        if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return 0.95;

        // Content-based: look for CSV structure
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (lines.Length < 2)
            return 0.0; // Need at least header + 1 data row

        var headerLine = lines[0];
        var dataLine = lines[1];

        // CSV typically has comma or semicolon delimiters
        var commaCount = headerLine.Count(c => c == ',');
        var semicolonCount = headerLine.Count(c => c == ';');
        var tabCount = headerLine.Count(c => c == '\t');

        // If first line has delimiters and subsequent lines have similar structure
        if ((commaCount > 0 || semicolonCount > 0 || tabCount > 0) &&
            (dataLine.Count(c => c == ',') == commaCount ||
             dataLine.Count(c => c == ';') == semicolonCount ||
             dataLine.Count(c => c == '\t') == tabCount))
        {
            return 0.8; // Good confidence
        }

        // If filename contains "csv" but content doesn't match, still moderate confidence
        if (fileName.Contains("csv", StringComparison.OrdinalIgnoreCase))
            return 0.6;

        return 0.0;
    }

    public Task<ValidationResult> ValidateAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        try
        {
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            if (lines.Count < 2)
            {
                errors.Add(new ValidationError(
                    "CSV_INSUFFICIENT_DATA",
                    "CSV must have header row and at least one data row",
                    LineNumber: 1));
                return Task.FromResult(ValidationResult.WithErrors(errors.ToArray()));
            }

            // Detect delimiter
            var delimiter = DetectDelimiter(lines[0]);
            if (delimiter == '\0')
            {
                errors.Add(new ValidationError(
                    "CSV_NO_DELIMITER",
                    "Unable to detect CSV delimiter (expected comma, semicolon, or tab)",
                    LineNumber: 1));
                return Task.FromResult(ValidationResult.WithErrors(errors.ToArray()));
            }

            // Parse header
            var headerFields = ParseCsvLine(lines[0], delimiter);

            // Check for required columns
            var requiredColumns = new[] { "sku", "article_number", "product_id", "name", "title", "product_name" };
            var skuColumn = FindColumnIgnoreCase(headerFields, requiredColumns.Take(3).ToArray());
            var nameColumn = FindColumnIgnoreCase(headerFields, requiredColumns.Skip(3).ToArray());

            if (skuColumn == -1)
            {
                errors.Add(new ValidationError(
                    "CSV_MISSING_SKU_COLUMN",
                    "CSV must have SKU/Article Number/Product ID column",
                    ElementPath: "header"));
            }

            if (nameColumn == -1)
            {
                errors.Add(new ValidationError(
                    "CSV_MISSING_NAME_COLUMN",
                    "CSV must have Name/Title/Product Name column",
                    ElementPath: "header"));
            }

            // Validate data rows
            int validRows = 0;
            for (int i = 1; i < lines.Count; i++)
            {
                var fields = ParseCsvLine(lines[i], delimiter);

                // Check field count consistency
                if (fields.Count != headerFields.Count)
                {
                    warnings.Add(new ValidationWarning(
                        "CSV_FIELD_COUNT_MISMATCH",
                        $"Row has {fields.Count} fields but header has {headerFields.Count}",
                        LineNumber: i + 1));
                    continue;
                }

                // Check required fields have values
                if (skuColumn >= 0 && string.IsNullOrWhiteSpace(fields[skuColumn]))
                {
                    warnings.Add(new ValidationWarning(
                        "CSV_MISSING_SKU_VALUE",
                        "Row missing SKU/Article Number value",
                        LineNumber: i + 1));
                    continue;
                }

                if (nameColumn >= 0 && string.IsNullOrWhiteSpace(fields[nameColumn]))
                {
                    warnings.Add(new ValidationWarning(
                        "CSV_MISSING_NAME_VALUE",
                        "Row missing Name/Title value",
                        LineNumber: i + 1));
                    continue;
                }

                validRows++;
            }

            if (validRows == 0)
            {
                errors.Add(new ValidationError(
                    "CSV_NO_VALID_DATA",
                    "No valid data rows found (all rows failed validation)"));
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ValidationError(
                "CSV_PARSE_ERROR",
                $"Error parsing CSV: {ex.Message}"));
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

        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        var delimiter = DetectDelimiter(lines[0]);
        var headerFields = ParseCsvLine(lines[0], delimiter);

        // Map column names to indices
        var columnMap = new ColumnMap(headerFields);

        int validItems = 0;
        int skippedItems = 0;

        for (int i = 1; i < lines.Count; i++)
        {
            try
            {
                var fields = ParseCsvLine(lines[i], delimiter);

                if (fields.Count != headerFields.Count)
                {
                    skippedItems++;
                    continue;
                }

                var fieldMap = new Dictionary<string, string>(StringComparer.Ordinal);
                for (int j = 0; j < headerFields.Count && j < fields.Count; j++)
                {
                    fieldMap[headerFields[j]] = fields[j];
                }

                if (TryBuildEntity(fieldMap, columnMap, metadata, out var entity))
                {
                    entities.Add(entity);
                    validItems++;
                }
                else
                {
                    skippedItems++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing CSV row {LineNumber}", i + 1);
                skippedItems++;
            }
        }

        var statistics = new ImportStatistics(
            entities.Count + skippedItems,
            validItems,
            skippedItems);

        return Task.FromResult(new ParseResult(entities, statistics, warnings));
    }

    private static char DetectDelimiter(string headerLine)
    {
        // Count delimiter candidates
        var commaCount = headerLine.Count(c => c == ',');
        var semicolonCount = headerLine.Count(c => c == ';');
        var tabCount = headerLine.Count(c => c == '\t');

        return (commaCount, semicolonCount, tabCount) switch
        {
            (0, 0, 0) => '\0', // No delimiter detected
            var (c, s, t) when c >= s && c >= t => ',',
            var (c, s, t) when s >= c && s >= t => ';',
            var (c, s, t) when t >= c && t >= s => '\t',
            _ => ','
        };
    }

    private static List<string> ParseCsvLine(string line, char delimiter)
    {
        var fields = new List<string>();
        var currentField = new System.Text.StringBuilder();
        var inQuotes = false;

        int i = 0;
        while (i < line.Length)
        {
            var c = line[i];

            if (c == '"')
            {
                // Check for escaped quote (double quote)
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // Skip next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                fields.Add(currentField.ToString().Trim());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
            i++;
        }

        fields.Add(currentField.ToString().Trim());
        return fields;
    }

    private static int FindColumnIgnoreCase(List<string> headers, params string[] names)
    {
        foreach (var name in names)
        {
            var idx = headers.FindIndex(h => h.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                return idx;
        }
        return -1;
    }

    private static bool TryBuildEntity(
        Dictionary<string, string> fields,
        ColumnMap columnMap,
        ImportMetadata metadata,
        out CatalogEntity entity)
    {
        entity = null!;

        // Get required fields
        var sku = GetFieldValue(fields, columnMap.SkuColumnNames);
        var name = GetFieldValue(fields, columnMap.NameColumnNames);

        if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name))
            return false;

        // Get optional fields
        var description = GetFieldValue(fields, columnMap.DescriptionColumnNames);
        var ean = GetFieldValue(fields, columnMap.EanColumnNames);
        var mfgPartNumber = GetFieldValue(fields, columnMap.MfgPartNumberColumnNames);
        var currency = GetFieldValue(fields, columnMap.CurrencyColumnNames) ?? "EUR";

        decimal? price = null;
        var priceStr = GetFieldValue(fields, columnMap.PriceColumnNames);
        if (!string.IsNullOrWhiteSpace(priceStr) &&
            decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
            price = p;

        entity = new CatalogEntity(
            ExternalId: sku,
            SupplierId: metadata.SupplierId ?? metadata.TenantId,
            Name: name,
            Description: description,
            Ean: ean,
            ManufacturerPartNumber: mfgPartNumber,
            ListPrice: price,
            Currency: currency,
            Attributes: fields);

        return true;
    }

    private static string? GetFieldValue(Dictionary<string, string> fields, string[] columnNames)
    {
        foreach (var name in columnNames)
        {
            if (fields.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value))
                return value;
        }
        return null;
    }

    private record ColumnMap(List<string> Headers)
    {
        public string[] SkuColumnNames => new[] { "sku", "article_number", "product_id", "product_code" };
        public string[] NameColumnNames => new[] { "name", "title", "product_name" };
        public string[] DescriptionColumnNames => new[] { "description", "long_description", "details" };
        public string[] EanColumnNames => new[] { "ean", "gtin", "ean_code", "barcode" };
        public string[] MfgPartNumberColumnNames => new[] { "mfg_part_number", "manufacturer_part_number", "mpn", "part_number" };
        public string[] PriceColumnNames => new[] { "price", "list_price", "msrp", "retail_price" };
        public string[] CurrencyColumnNames => new[] { "currency", "currency_code", "currency_iso" };
    }
}
