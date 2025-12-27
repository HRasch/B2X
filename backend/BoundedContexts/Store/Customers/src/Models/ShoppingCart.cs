namespace B2Connect.Store.Customers.Models;

/// <summary>
/// Shopping cart aggregate root
/// References User by UserId only (no direct navigation - maintains Bounded Context separation)
/// </summary>
public class ShoppingCart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Reference to Shared.User.Models.User
    public Guid TenantId { get; set; }
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public ShoppingCartStatus Status { get; set; } = ShoppingCartStatus.Active;
    public bool IsGuest { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total => SubTotal + TaxAmount + ShippingCost - DiscountAmount;
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CheckoutStartedAt { get; set; }
    public DateTime? CheckoutCompletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    public int GetItemCount() => Items?.Sum(i => i.Quantity) ?? 0;

    public void CalculateTotals()
    {
        if (Items == null || Items.Count == 0)
        {
            SubTotal = 0;
            TaxAmount = 0;
            ShippingCost = 0;
            return;
        }

        SubTotal = Items.Sum(i => i.UnitPrice * i.Quantity);
        // Tax calculation (simplified - 19% for example)
        TaxAmount = SubTotal * 0.19m;
    }

    public bool IsExpired(int expirationDays = 7)
    {
        return DateTime.UtcNow > UpdatedAt.AddDays(expirationDays);
    }
}

public enum ShoppingCartStatus
{
    Active = 1,
    Abandoned = 2,
    CheckoutStarted = 3,
    Completed = 4,
    Cancelled = 5
}
