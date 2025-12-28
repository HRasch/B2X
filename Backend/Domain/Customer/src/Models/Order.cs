using System;
using System.Collections.Generic;

namespace B2Connect.Customer.Models;

/// <summary>
/// Order Entity - Represents a customer order (B2C/B2B)
/// Story 3: 14-Day Withdrawal Right (P0.6)
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    
    // Order Reference
    public string OrderNumber { get; set; } // e.g., "ORD-2025-001234"
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // Order Status
    public string Status { get; set; } // "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
    
    // Order Items
    public List<OrderItem> Items { get; set; } = new();
    
    // Pricing
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Customer Info (encrypted)
    public string CustomerNameEncrypted { get; set; }
    public string CustomerEmailEncrypted { get; set; }
    public string BillingAddressEncrypted { get; set; }
    public string ShippingAddressEncrypted { get; set; }
    
    // Country for Tax Purposes
    public string ShippingCountry { get; set; }
    public string BillingCountry { get; set; }
    
    // B2B specific
    public bool IsB2b { get; set; }
    public string VatIdValidated { get; set; } // Only if valid for B2B
    public bool ReverseChargeApplied { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Audit Trail
    public DateTime ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    
    /// <summary>
    /// Calculate days remaining for withdrawal (VVVG §357)
    /// Withdrawal deadline: 14 days from delivery
    /// </summary>
    public int GetWithdrawalDaysRemaining()
    {
        if (!DeliveredAt.HasValue)
            return -1; // Order not yet delivered
        
        var deadline = DeliveredAt.Value.AddDays(14);
        var daysRemaining = (int)(deadline - DateTime.UtcNow).TotalDays;
        
        return Math.Max(daysRemaining, 0);
    }
    
    /// <summary>
    /// Check if order is within withdrawal period
    /// </summary>
    public bool IsWithinWithdrawalPeriod()
    {
        return GetWithdrawalDaysRemaining() > 0;
    }
}

/// <summary>
/// OrderItem - Individual items in an order
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    
    // Product Details (snapshot at time of order)
    public string ProductSku { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
}
