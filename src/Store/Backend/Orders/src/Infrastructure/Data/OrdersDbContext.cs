using B2X.Orders.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2X.Orders.Infrastructure.Data;

/// <summary>
/// Database context for Orders Service
///
/// Handles all data access for:
/// - Orders
/// - Order Items
///
/// Multi-tenant isolation: All queries must include TenantId filter
/// </summary>
public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    /// <summary>Orders placed by customers</summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>Items within orders</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            entity.Property(e => e.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            entity.Property(e => e.CustomerEmail)
                .HasColumnName("customer_email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50)
                .HasDefaultValue("pending")
                .IsRequired();

            entity.Property(e => e.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .HasDefaultValue("EUR")
                .IsRequired();

            entity.Property(e => e.Subtotal)
                .HasColumnName("subtotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.TaxAmount)
                .HasColumnName("tax_amount")
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.DiscountAmount)
                .HasColumnName("discount_amount")
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.ShippingAmount)
                .HasColumnName("shipping_amount")
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(e => e.TotalAmount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("payment_method")
                .HasMaxLength(50)
                .HasDefaultValue("card")
                .IsRequired();

            entity.Property(e => e.PaymentStatus)
                .HasColumnName("payment_status")
                .HasMaxLength(50)
                .HasDefaultValue("pending")
                .IsRequired();

            entity.Property(e => e.PaymentProvider)
                .HasColumnName("payment_provider")
                .HasMaxLength(100);

            entity.Property(e => e.TransactionId)
                .HasColumnName("transaction_id")
                .HasMaxLength(255);

            // Shipping address
            entity.Property(e => e.ShippingFirstName)
                .HasColumnName("shipping_first_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ShippingLastName)
                .HasColumnName("shipping_last_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ShippingCompany)
                .HasColumnName("shipping_company")
                .HasMaxLength(100);

            entity.Property(e => e.ShippingStreet)
                .HasColumnName("shipping_street")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.ShippingCity)
                .HasColumnName("shipping_city")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ShippingPostalCode)
                .HasColumnName("shipping_postal_code")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ShippingCountry)
                .HasColumnName("shipping_country")
                .HasMaxLength(2)
                .HasDefaultValue("DE")
                .IsRequired();

            entity.Property(e => e.ShippingPhone)
                .HasColumnName("shipping_phone")
                .HasMaxLength(50);

            // Billing address (optional, defaults to shipping if not provided)
            entity.Property(e => e.BillingFirstName)
                .HasColumnName("billing_first_name")
                .HasMaxLength(100);

            entity.Property(e => e.BillingLastName)
                .HasColumnName("billing_last_name")
                .HasMaxLength(100);

            entity.Property(e => e.BillingCompany)
                .HasColumnName("billing_company")
                .HasMaxLength(100);

            entity.Property(e => e.BillingStreet)
                .HasColumnName("billing_street")
                .HasMaxLength(255);

            entity.Property(e => e.BillingCity)
                .HasColumnName("billing_city")
                .HasMaxLength(100);

            entity.Property(e => e.BillingPostalCode)
                .HasColumnName("billing_postal_code")
                .HasMaxLength(20);

            entity.Property(e => e.BillingCountry)
                .HasColumnName("billing_country")
                .HasMaxLength(2);

            entity.Property(e => e.BillingPhone)
                .HasColumnName("billing_phone")
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);

            entity.Property(e => e.DiscountCode)
                .HasColumnName("discount_code")
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            // Indexes for performance
            entity.HasIndex(e => e.TenantId).HasDatabaseName("IX_Orders_TenantId");
            entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Orders_CustomerId");
            entity.HasIndex(e => new { e.TenantId, e.Status }).HasDatabaseName("IX_Orders_TenantId_Status");
            entity.HasIndex(e => new { e.TenantId, e.PaymentStatus }).HasDatabaseName("IX_Orders_TenantId_PaymentStatus");
            entity.HasIndex(e => new { e.TenantId, e.CreatedAt }).HasDatabaseName("IX_Orders_TenantId_CreatedAt");
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            entity.Property(e => e.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            entity.Property(e => e.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.ProductSku)
                .HasColumnName("product_sku")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            entity.Property(e => e.UnitPrice)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.TotalPrice)
                .HasColumnName("total_price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItems_Orders_OrderId");

            // Indexes for performance
            entity.HasIndex(e => e.OrderId).HasDatabaseName("IX_OrderItems_OrderId");
            entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_OrderItems_ProductId");
        });
    }
}
