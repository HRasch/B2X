using System.Xml;
using System.Xml.Schema;
using B2X.Catalog.Application.Adapters;
using B2X.Catalog.Core.Entities;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Application.Adapters;

/// <summary>
/// BMEcat catalog import adapter
/// Supports BMEcat 1.2/2005 standard for product catalog import
/// </summary>
public class BmecatImportAdapter : ICatalogImportAdapter
{
    private readonly ILogger<BmecatImportAdapter> _logger;

    public string Format => "bmecat";

    public BmecatImportAdapter(ILogger<BmecatImportAdapter> logger)
    {
        _logger = logger;
    }

    public async Task<CatalogImportResult> ImportAsync(
        Stream catalogStream,
        CatalogMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var result = new CatalogImportResult
        {
            ImportId = Guid.NewGuid(),
            Success = false
        };

        try
        {
            // Validate stream size (100MB limit)
            if (catalogStream.Length > 100 * 1024 * 1024)
            {
                result.ErrorMessage = "Catalog file exceeds maximum size of 100MB";
                return result;
            }

            // Detect BMEcat version and set up validation
            var (version, settings) = await SetupValidationAsync(catalogStream, metadata, result, cancellationToken).ConfigureAwait(false);
            if (!result.Success)
            {
                return result;
            }

            // Reset stream position for parsing
            catalogStream.Position = 0;

            // Parse and validate XML
            var products = await ParseBmecatXmlAsync(catalogStream, metadata, result, cancellationToken).ConfigureAwait(false);

            if (!result.Success)
            {
                return result;
            }

            result.ProductCount = products.Count;
            result.Success = true;
            result.Products = products;

            _logger.LogInformation(
                "Successfully parsed and validated BMEcat {Version} catalog with {ProductCount} products for supplier {SupplierId}",
                version, result.ProductCount, metadata.SupplierId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import BMEcat catalog for supplier {SupplierId}", metadata.SupplierId);
            result.ErrorMessage = $"Import failed: {ex.Message}";
            result.ValidationErrors.Add(ex.Message);
        }

        return result;
    }

    private async Task<(string Version, XmlReaderSettings Settings)> SetupValidationAsync(
        Stream catalogStream,
        CatalogMetadata metadata,
        CatalogImportResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            // Read the beginning of the file to detect version
            var buffer = new byte[1024];
            var bytesRead = await catalogStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            var xmlStart = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Detect BMEcat version from XML declaration or root element
            var version = DetectBmecatVersion(xmlStart);
            if (string.IsNullOrEmpty(version))
            {
                result.ErrorMessage = "Unable to detect BMEcat version from XML";
                result.ValidationErrors.Add("Missing or invalid BMEcat version declaration");
                result.Success = false;
                return (string.Empty, null!);
            }

            // Create validation settings with appropriate schema
            var settings = CreateXmlReaderSettings(version, metadata.CustomSchemaPath);
            return (version, settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup XML validation for supplier {SupplierId}", metadata.SupplierId);
            result.ErrorMessage = $"Validation setup failed: {ex.Message}";
            result.ValidationErrors.Add(ex.Message);
            result.Success = false;
            return (string.Empty, null!);
        }
    }

    private string? DetectBmecatVersion(string xmlContent)
    {
        // Check for version attribute in BMECAT element
        var versionMatch = System.Text.RegularExpressions.Regex.Match(
            xmlContent,
            @"<BMECAT[^>]*version\s*=\s*[""']([^""']+)[""']",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (versionMatch.Success)
        {
            var version = versionMatch.Groups[1].Value;
            if (version.StartsWith("1.2", StringComparison.Ordinal))
                return "1.2";
            if (version.Equals("2005.2", StringComparison.Ordinal))
                return "2005.2";
            if (version.Equals("2005.1", StringComparison.Ordinal))
                return "2005.1";
            if (version.Equals("2005", StringComparison.Ordinal))
                return "2005";
            if (version.StartsWith("2005", StringComparison.Ordinal))
                return "2005"; // Default to 2005 base version
        }

        // Fallback: check for namespace declarations
        if (xmlContent.Contains("bmecat/1.2/"))
            return "1.2";
        if (xmlContent.Contains("bmecat/2005/"))
            return "2005";

        return null;
    }

    private async Task<List<CatalogProduct>> ParseBmecatXmlAsync(
        Stream catalogStream,
        CatalogMetadata metadata,
        CatalogImportResult result,
                CancellationToken cancellationToken)
    {
        var products = new List<CatalogProduct>();
        var readerSettings = CreateXmlReaderSettings();

        try
        {
            using var reader = XmlReader.Create(catalogStream, readerSettings);

            string? currentSupplierId = null;
            string? currentCatalogId = null;
            var inArticle = false;
            var currentArticle = new Dictionary<string, string>(StringComparer.Ordinal);
            string? currentElement = null;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = reader.Name;

                        if (string.Equals(reader.Name, "SUPPLIER_ID", StringComparison.Ordinal) && reader.Depth == 2) // HEADER/SUPPLIER_ID
                        {
                            currentSupplierId = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                        }
                        else if (string.Equals(reader.Name, "CATALOG_ID", StringComparison.Ordinal) && reader.Depth == 2) // HEADER/CATALOG_ID
                        {
                            currentCatalogId = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                        }
                        else if (string.Equals(reader.Name, "ARTICLE", StringComparison.Ordinal))
                        {
                            inArticle = true;
                            currentArticle.Clear();
                        }
                        else if (inArticle && IsProductElement(reader.Name))
                        {
                            // Store element for processing
                        }
                        break;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        if (inArticle && currentElement != null && IsProductElement(currentElement))
                        {
                            currentArticle[currentElement] = await reader.GetValueAsync().ConfigureAwait(false);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (string.Equals(reader.Name, "ARTICLE", StringComparison.Ordinal) && inArticle)
                        {
                            // Process completed article
                            var product = CreateCatalogProduct(currentArticle, metadata);
                            if (product != null)
                            {
                                products.Add(product);
                            }
                            inArticle = false;
                            currentArticle.Clear();
                        }
                        currentElement = null;
                        break;
                }
            }

            // Validate header information
            if (currentSupplierId != null && !string.Equals(currentSupplierId, metadata.SupplierId, StringComparison.Ordinal))
            {
                result.ValidationErrors.Add($"Supplier ID mismatch: expected {metadata.SupplierId}, found {currentSupplierId}");
            }

            if (currentCatalogId != null && !string.Equals(currentCatalogId, metadata.CatalogId, StringComparison.Ordinal))
            {
                result.ValidationErrors.Add($"Catalog ID mismatch: expected {metadata.CatalogId}, found {currentCatalogId}");
            }

            // Set success based on validation
            result.Success = !result.ValidationErrors.Any();
        }
        catch (XmlException ex)
        {
            result.ValidationErrors.Add($"XML parsing error: {ex.Message}");
            result.Success = false;
            throw;
        }

        return products;
    }

    private XmlReaderSettings CreateXmlReaderSettings(string? version = null, string? customSchemaPath = null)
    {
        var settings = new XmlReaderSettings
        {
            // Security: Disable DTD processing to prevent XXE attacks
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,

            // Performance: Use async reading
            Async = true,

            // Size limits
            MaxCharactersFromEntities = 1024,
            MaxCharactersInDocument = 100 * 1024 * 1024 // 100MB
        };

        // Set up XSD validation if version is specified
        if (!string.IsNullOrEmpty(version))
        {
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

            // Add validation event handlers
            var validationErrors = new List<string>();
            settings.ValidationEventHandler += (sender, e) =>
            {
                var message = $"XML Validation {e.Severity}: {e.Message}";
                if (e.Severity == XmlSeverityType.Error)
                {
                    validationErrors.Add(message);
                    _logger.LogWarning("BMEcat XML validation error: {Message} at line {Line}, position {Position}",
                        e.Message, e.Exception?.LineNumber ?? 0, e.Exception?.LinePosition ?? 0);
                }
                else
                {
                    _logger.LogInformation("BMEcat XML validation warning: {Message}", e.Message);
                }
            };

            // Load appropriate schema
            try
            {
                string schemaPath;
                if (!string.IsNullOrEmpty(customSchemaPath) && File.Exists(customSchemaPath))
                {
                    schemaPath = customSchemaPath;
                    _logger.LogInformation("Using custom BMEcat schema: {SchemaPath}", customSchemaPath);
                }
                else
                {
                    // Use built-in schema based on version
                    var schemaFile = version switch
                    {
                        "1.2" => "BMEcat1200.xsd",
                        "2005" => "BMEcat2005Base.xsd",
                        "2005.1" => "BMEcat20051.xsd",
                        "2005.2" => "BMEcat20052.xsd",
                        _ => "BMEcat20052.xsd"
                    };
                    schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "Infrastructure", "Schemas", schemaFile);

                    if (!File.Exists(schemaPath))
                    {
                        // Fallback to relative path from executing assembly
                        var assemblyLocation = Path.GetDirectoryName(typeof(BmecatImportAdapter).Assembly.Location);
                        schemaPath = Path.Combine(assemblyLocation!, "Infrastructure", "Schemas", schemaFile);
                    }

                    if (!File.Exists(schemaPath))
                    {
                        _logger.LogWarning("BMEcat schema file not found: {SchemaPath}, falling back to structure validation only", schemaPath);
                        settings.ValidationType = ValidationType.None;
                        return settings;
                    }
                }

                // Load and add schema
                using var schemaStream = File.OpenRead(schemaPath);
                using var reader = XmlReader.Create(schemaStream, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                });
                var schema = XmlSchema.Read(reader, (sender, e) =>
                {
                    _logger.LogError("Error loading BMEcat schema: {Message}", e.Message);
                });

                if (schema != null)
                {
                    settings.Schemas.Add(schema);
                    _logger.LogInformation("Loaded BMEcat {Version} schema for validation", version);
                }
                else
                {
                    _logger.LogWarning("Failed to load BMEcat schema, falling back to structure validation only");
                    settings.ValidationType = ValidationType.None;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up BMEcat schema validation, falling back to structure validation only");
                settings.ValidationType = ValidationType.None;
            }
        }
        else
        {
            // No version specified - structure validation only
            settings.ValidationType = ValidationType.None;
        }

        return settings;
    }

    private bool IsProductElement(string elementName)
    {
        return elementName switch
        {
            "SUPPLIER_AID" => true,
            "ARTICLE_DETAILS" => true,
            "DESCRIPTION_SHORT" => true,
            "DESCRIPTION_LONG" => true,
            "ARTICLE_PRICE_DETAILS" => true,
            "ARTICLE_PRICE" => true,
            "PRICE_AMOUNT" => true,
            "PRICE_CURRENCY" => true,
            _ => false
        };
    }

    private CatalogProduct? CreateCatalogProduct(Dictionary<string, string> articleData, CatalogMetadata metadata)
    {
        if (!articleData.TryGetValue("SUPPLIER_AID", out var supplierAid) || string.IsNullOrEmpty(supplierAid))
        {
            _logger.LogWarning("Article missing SUPPLIER_AID, skipping");
            return null;
        }

        // Create JSON representation of product data
        var productJson = System.Text.Json.JsonSerializer.Serialize(new
        {
            supplierAid,
            shortDescription = articleData.GetValueOrDefault("DESCRIPTION_SHORT"),
            longDescription = articleData.GetValueOrDefault("DESCRIPTION_LONG"),
            priceAmount = articleData.GetValueOrDefault("PRICE_AMOUNT"),
            priceCurrency = articleData.GetValueOrDefault("PRICE_CURRENCY"),
            importedAt = DateTime.UtcNow,
            metadata
        });

        return new CatalogProduct
        {
            Id = Guid.NewGuid(),
            CatalogImportId = metadata.TenantId, // Will be set by caller
            SupplierAid = supplierAid,
            ProductData = productJson,
            CreatedAt = DateTime.UtcNow
        };
    }
}
