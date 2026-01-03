using System.Xml;
using System.Xml.Schema;
using B2Connect.Catalog.Application.Adapters;
using B2Connect.Catalog.Core.Entities;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Adapters;

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

            // Parse and validate XML
            var products = await ParseBmecatXmlAsync(catalogStream, metadata, result, cancellationToken);

            if (!result.Success)
            {
                return result;
            }

            result.ProductCount = products.Count;
            result.Success = true;
            result.Products = products;

            _logger.LogInformation(
                "Successfully parsed BMEcat catalog with {ProductCount} products for supplier {SupplierId}",
                result.ProductCount, metadata.SupplierId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import BMEcat catalog for supplier {SupplierId}", metadata.SupplierId);
            result.ErrorMessage = $"Import failed: {ex.Message}";
            result.ValidationErrors.Add(ex.Message);
        }

        return result;
    }

    private async Task<List<CatalogProduct>> ParseBmecatXmlAsync(
        Stream catalogStream,
        CatalogMetadata metadata,
        CatalogImportResult result,
        CancellationToken cancellationToken)
    {
        var products = new List<CatalogProduct>();
        var settings = CreateXmlReaderSettings();

        try
        {
            using var reader = XmlReader.Create(catalogStream, settings);

            string? currentSupplierId = null;
            string? currentCatalogId = null;
            var inArticle = false;
            var currentArticle = new Dictionary<string, string>();
            string? currentElement = null;

            while (await reader.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = reader.Name;

                        if (reader.Name == "SUPPLIER_ID" && reader.Depth == 2) // HEADER/SUPPLIER_ID
                        {
                            currentSupplierId = await reader.ReadElementContentAsStringAsync();
                        }
                        else if (reader.Name == "CATALOG_ID" && reader.Depth == 2) // HEADER/CATALOG_ID
                        {
                            currentCatalogId = await reader.ReadElementContentAsStringAsync();
                        }
                        else if (reader.Name == "ARTICLE")
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
                            currentArticle[currentElement] = await reader.GetValueAsync();
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == "ARTICLE" && inArticle)
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
            if (currentSupplierId != null && currentSupplierId != metadata.SupplierId)
            {
                result.ValidationErrors.Add($"Supplier ID mismatch: expected {metadata.SupplierId}, found {currentSupplierId}");
            }

            if (currentCatalogId != null && currentCatalogId != metadata.CatalogId)
            {
                result.ValidationErrors.Add($"Catalog ID mismatch: expected {metadata.CatalogId}, found {currentCatalogId}");
            }
        }
        catch (XmlException ex)
        {
            result.ValidationErrors.Add($"XML parsing error: {ex.Message}");
            throw;
        }

        return products;
    }

    private XmlReaderSettings CreateXmlReaderSettings()
    {
        var settings = new XmlReaderSettings
        {
            // Security: Disable DTD processing to prevent XXE attacks
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,

            // Performance: Use async reading
            Async = true,

            // Validation: Ignore missing schemas for now (we validate structure manually)
            ValidationType = ValidationType.None,

            // Size limits
            MaxCharactersFromEntities = 1024,
            MaxCharactersInDocument = 100 * 1024 * 1024 // 100MB
        };

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