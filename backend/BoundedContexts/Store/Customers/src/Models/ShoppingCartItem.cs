namespace B2Connect.Store.Customers.Models;

/// <summary>
/// Item in a shopping cart
/// References product by ID only (no navigation property to maintain Bounded Context separation)
/// </summary>
public class ShoppingCartItem
{
    public Guid Id { get; set; }
    public Guid ShoppingCartId { get; set; }
    public Guid ProductId { get; set; } // Reference to Catalog.Product without direct navigation
    public Guid TenantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? SelectedVariant { get; set; } // e.g., "Size: L, Color: Red"
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ShoppingCart? ShoppingCart { get; set; }

    public decimal GetLineTotal()
    {
        var price = DiscountPrice ?? UnitPrice;
        return price * Quantity;
    }

    public bool IsStockValid(int availableStock)
    {
        return Quantity > 0 && Quantity <= availableStock;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 1)
            throw new ArgumentException("Quantity must be at least 1");

        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
