using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Types.Localization;
using ShopEntity = B2Connect.Store.Core.Common.Entities.Shop;

namespace B2Connect.Store.Core.Store.Entities;

/// <summary>
/// Shipping method entity for delivery configuration
/// Manages shipping carriers, zones, and costs
/// Store-specific domain
/// </summary>
public class ShippingMethod
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Unique code for shipping method (e.g., DHL_STANDARD, UPS_EXPRESS)</summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Display name (e.g., Standard Shipping, Express Delivery)</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Detailed description</summary>
    public LocalizedContent? Description { get; set; }

    /// <summary>Shipping carrier (e.g., DHL, DPD, UPS, FedEx, Local Courier)</summary>
    [MaxLength(50)]
    public string? Carrier { get; set; }

    /// <summary>Carrier service name (e.g., Express, Priority, Standard)</summary>
    [MaxLength(100)]
    public string? ServiceName { get; set; }

    /// <summary>Tracking URL template (e.g., https://www.dhl.de/tracking?id={tracking_number})</summary>
    [MaxLength(500)]
    public string? TrackingUrlTemplate { get; set; }

    /// <summary>Base shipping cost</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal BaseCost { get; set; } = 0;

    /// <summary>Cost per kilogram</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? CostPerKg { get; set; }

    /// <summary>Cost per item</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? CostPerItem { get; set; }

    /// <summary>Free shipping threshold amount</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? FreeShippingThreshold { get; set; }

    /// <summary>Estimated delivery days minimum</summary>
    public int? EstimatedDaysMin { get; set; }

    /// <summary>Estimated delivery days maximum</summary>
    public int? EstimatedDaysMax { get; set; }

    /// <summary>Maximum package weight in kg</summary>
    public decimal? MaximumWeight { get; set; }

    /// <summary>Maximum package dimensions (LxWxH in cm, comma-separated)</summary>
    [MaxLength(100)]
    public string? MaximumDimensions { get; set; }

    /// <summary>Supported countries (comma-separated country codes)</summary>
    [MaxLength(500)]
    public string? SupportedCountries { get; set; }

    /// <summary>Countries excluded from this method (higher priority)</summary>
    [MaxLength(500)]
    public string? ExcludedCountries { get; set; }

    /// <summary>Requires signature on delivery</summary>
    public bool RequiresSignature { get; set; } = false;

    /// <summary>Allow residential delivery</summary>
    public bool AllowResidential { get; set; } = true;

    /// <summary>Allow PO boxes</summary>
    public bool AllowPoBox { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Reference to shop this method belongs to</summary>
    [ForeignKey(nameof(Shop))]
    public Guid StoreId { get; set; }
    public ShopEntity? Shop { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

