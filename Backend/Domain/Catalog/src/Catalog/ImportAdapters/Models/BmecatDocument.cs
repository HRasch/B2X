using System.Xml.Serialization;

namespace B2X.Catalog.ImportAdapters.Models;

/// <summary>
/// BMEcat XML document model for deserialization.
/// Supports versions 1.2, 2005, 2005.1, 2005.2
/// </summary>
[XmlRoot("BMECAT", Namespace = "")]
public class BmecatDocument
{
    [XmlAttribute("version")]
    public string? Version { get; set; }

    [XmlElement("HEADER")]
    public BmecatHeader? Header { get; set; }

    [XmlElement("CATALOG")]
    public BmecatCatalog? Catalog { get; set; }

    [XmlElement("ARTICLE")]
    public List<BmecatArticle> Articles { get; set; } = [];
}

public class BmecatHeader
{
    [XmlElement("CATALOG_NAME")]
    public string? CatalogName { get; set; }

    [XmlElement("CATALOG_VERSION")]
    public string? CatalogVersion { get; set; }

    [XmlElement("CATALOG_DATE")]
    public DateTime? CatalogDate { get; set; }

    [XmlElement("SUPPLIER")]
    public string? Supplier { get; set; }

    [XmlElement("SUPPLIER_AID")]
    public string? SupplierId { get; set; }

    [XmlElement("CUSTOMER")]
    public string? Customer { get; set; }

    [XmlElement("CUSTOMER_AID")]
    public string? CustomerId { get; set; }

    [XmlElement("TERRITORY")]
    public string? Territory { get; set; }

    [XmlElement("CURRENCY")]
    public string? Currency { get; set; }

    [XmlElement("LANGUAGE")]
    public string? Language { get; set; }
}

public class BmecatCatalog
{
    [XmlElement("LANGUAGE")]
    public string? Language { get; set; }

    [XmlElement("TERRITORY")]
    public string? Territory { get; set; }
}

public class BmecatArticle
{
    [XmlElement("SUPPLIER_AID")]
    public string? SupplierId { get; set; }

    [XmlElement("ARTICLE_DETAILS")]
    public BmecatArticleDetails? Details { get; set; }

    [XmlElement("ARTICLE_FEATURES")]
    public BmecatArticleFeatures? Features { get; set; }

    [XmlElement("ARTICLE_REFERENCES")]
    public BmecatArticleReferences? References { get; set; }

    [XmlElement("ARTICLE_PRICE_DETAILS")]
    public BmecatArticlePriceDetails? PriceDetails { get; set; }

    [XmlElement("ARTICLE_ORDER_DETAILS")]
    public BmecatArticleOrderDetails? OrderDetails { get; set; }
}

public class BmecatArticleDetails
{
    [XmlElement("ARTICLE_NUMBER")]
    public string? ArticleNumber { get; set; }

    [XmlElement("DESCRIPTION")]
    public string? Description { get; set; }

    [XmlElement("EAN")]
    public string? Ean { get; set; }

    [XmlElement("SUPPLIER_AID_SUPPLEMENT")]
    public string? SupplierAidSupplement { get; set; }

    [XmlElement("MANUFACTURER")]
    public string? Manufacturer { get; set; }

    [XmlElement("MANUFACTURER_AID")]
    public string? ManufacturerAid { get; set; }

    [XmlElement("MANUFACTURER_AID_SUPPLEMENT")]
    public string? ManufacturerAidSupplement { get; set; }

    [XmlElement("ARTICLE_STATUS")]
    public string? ArticleStatus { get; set; }

    [XmlElement("ARTICLE_STATUS_DETAILS")]
    public string? ArticleStatusDetails { get; set; }

    [XmlElement("SPECIAL_TREATMENT_CLASS")]
    public string? SpecialTreatmentClass { get; set; }

    [XmlElement("KEYWORD")]
    public List<string> Keywords { get; set; } = [];

    [XmlElement("REMARKS")]
    public string? Remarks { get; set; }

    [XmlElement("MIME_INFO")]
    public BmecatMimeInfo? MimeInfo { get; set; }
}

public class BmecatArticleFeatures
{
    [XmlElement("FEATURE")]
    public List<BmecatFeature> Features { get; set; } = [];

    [XmlElement("FEATURE_GROUP")]
    public List<BmecatFeatureGroup> FeatureGroups { get; set; } = [];
}

public class BmecatFeature
{
    [XmlElement("FNAME")]
    public string? Name { get; set; }

    [XmlElement("FVALUE")]
    public string? Value { get; set; }

    [XmlElement("UNIT")]
    public string? Unit { get; set; }
}

public class BmecatFeatureGroup
{
    [XmlAttribute("id")]
    public string? Id { get; set; }

    [XmlElement("FEATURE")]
    public List<BmecatFeature> Features { get; set; } = [];
}

public class BmecatArticleReferences
{
    [XmlElement("PRODUCT_REFERENCE")]
    public List<BmecatProductReference> ProductReferences { get; set; } = [];
}

public class BmecatProductReference
{
    [XmlAttribute("type")]
    public string? Type { get; set; }

    [XmlElement("SUPPLIER_AID")]
    public string? SupplierId { get; set; }

    [XmlElement("ARTICLE_NUMBER")]
    public string? ArticleNumber { get; set; }

    [XmlElement("QUANTITY")]
    public decimal? Quantity { get; set; }
}

public class BmecatArticlePriceDetails
{
    [XmlElement("ARTICLE_PRICE")]
    public List<BmecatArticlePrice> Prices { get; set; } = [];
}

public class BmecatArticlePrice
{
    [XmlAttribute("price_type")]
    public string? PriceType { get; set; }

    [XmlElement("PRICE_AMOUNT")]
    public decimal? PriceAmount { get; set; }

    [XmlElement("PRICE_CURRENCY")]
    public string? PriceCurrency { get; set; }

    [XmlElement("PRICE_QUANTITY")]
    public BmecatPriceQuantity? PriceQuantity { get; set; }

    [XmlElement("LOWEST_PRICE")]
    public decimal? LowestPrice { get; set; }

    [XmlElement("VALID_START_DATE")]
    public DateTime? ValidStartDate { get; set; }

    [XmlElement("VALID_END_DATE")]
    public DateTime? ValidEndDate { get; set; }
}

public class BmecatPriceQuantity
{
    [XmlElement("PRICE_QTY")]
    public decimal? Quantity { get; set; }

    [XmlElement("QTY_UNIT")]
    public string? Unit { get; set; }
}

public class BmecatArticleOrderDetails
{
    [XmlElement("ORDER_UNIT")]
    public string? OrderUnit { get; set; }

    [XmlElement("CONTENT_UNIT")]
    public string? ContentUnit { get; set; }

    [XmlElement("PACKAGE_QUANTITY")]
    public decimal? PackageQuantity { get; set; }

    [XmlElement("DELIVERY_TIME")]
    public int? DeliveryTime { get; set; }

    [XmlElement("DELIVERY_TIME_UNIT")]
    public string? DeliveryTimeUnit { get; set; }
}

public class BmecatMimeInfo
{
    [XmlElement("MIME")]
    public List<BmecatMime> Mimes { get; set; } = [];
}

public class BmecatMime
{
    [XmlAttribute("type")]
    public string? Type { get; set; }

    [XmlElement("MIME_SOURCE")]
    public string? Source { get; set; }

    [XmlElement("MIME_DESCR")]
    public string? Description { get; set; }

    [XmlElement("MIME_ALTERNATIVE")]
    public string? Alternative { get; set; }
}
