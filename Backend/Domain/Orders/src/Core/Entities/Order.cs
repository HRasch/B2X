using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2X.Orders.Core.Entities;

/// <summary>
/// Order entity representing a customer order
/// </summary>
[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [Column("customer_id")]
    public Guid? CustomerId { get; set; }

    [Column("order_number")]
    [StringLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "pending";

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column("subtotal", TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column("tax_amount", TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column("shipping_amount", TypeName = "decimal(18,2)")]
    public decimal ShippingAmount { get; set; }

    [Column("currency")]
    [StringLength(3)]
    public string Currency { get; set; } = "EUR";

    // Shipping Address
    [Column("shipping_first_name")]
    [StringLength(100)]
    public string ShippingFirstName { get; set; } = string.Empty;

    [Column("shipping_last_name")]
    [StringLength(100)]
    public string ShippingLastName { get; set; } = string.Empty;

    [Column("shipping_street")]
    [StringLength(200)]
    public string ShippingStreet { get; set; } = string.Empty;

    [Column("shipping_city")]
    [StringLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    [Column("shipping_postal_code")]
    [StringLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;

    [Column("shipping_country")]
    [StringLength(2)]
    public string ShippingCountry { get; set; } = "DE";

    // Payment
    [Column("payment_method")]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = "card";

    [Column("payment_status")]
    [StringLength(20)]
    public string PaymentStatus { get; set; } = "pending";

    // Timestamps
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
