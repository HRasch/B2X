using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Types.Localization;
using ShopEntity = B2X.Store.Core.Common.Entities.Shop;

namespace B2X.Store.Core.Store.Entities;

/// <summary>
/// Payment method entity for checkout configuration
/// Supports multiple payment providers and configurations
/// Store-specific domain
/// </summary>
public class PaymentMethod
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Unique code for payment method (e.g., PAYPAL, STRIPE, SEPA)</summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Display name (e.g., PayPal, Kreditkarte, SEPA-Überweisung)</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Detailed description for customer</summary>
    public LocalizedContent? Description { get; set; }

    /// <summary>Payment provider (e.g., PayPal, Stripe, Adyen, Wirecard)</summary>
    [MaxLength(50)]
    public string? Provider { get; set; }

    /// <summary>Payment category for grouping (e.g., CARD, WALLET, BANK_TRANSFER, INVOICE)</summary>
    [MaxLength(50)]
    public string? Category { get; set; }

    /// <summary>Icon/Logo URL</summary>
    [MaxLength(500)]
    public string? IconUrl { get; set; }

    /// <summary>Processing fee percentage (e.g., 2.9 for 2.9%)</summary>
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? ProcessingFeePercent { get; set; }

    /// <summary>Fixed processing fee</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? ProcessingFeeFix { get; set; }

    /// <summary>Minimum order amount for this payment method</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MinimumAmount { get; set; }

    /// <summary>Maximum order amount for this payment method</summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MaximumAmount { get; set; }

    /// <summary>Supported currencies (comma-separated, e.g., EUR,GBP,USD)</summary>
    [MaxLength(100)]
    public string? SupportedCurrencies { get; set; }

    /// <summary>Countries where this payment method is available</summary>
    [MaxLength(500)]
    public string? SupportedCountries { get; set; }

    /// <summary>Enable payment immediately or pending approval</summary>
    public bool CaptureImmediately { get; set; } = true;

    /// <summary>Allow partial refunds</summary>
    public bool AllowPartialRefund { get; set; } = true;

    /// <summary>Require 3D Secure verification (for cards)</summary>
    public bool RequireThreeDSecure { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Reference to shop this method belongs to</summary>
    [ForeignKey(nameof(Shop))]
    public Guid StoreId { get; set; }
    public ShopEntity? Shop { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

