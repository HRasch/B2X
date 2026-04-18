using B2X.Returns.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2X.Returns.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext für Returns (Withdrawal Rights Management)
/// 
/// Konfiguration:
/// - Multi-Tenant Isolation: TenantId on all entities
/// - Audit Trail: CreatedBy, CreatedAt, UpdatedAt tracking
/// - Generated Columns: DeliveryDate + 14 days = ReturnDeadline, IsWithinDeadline
/// - Cascade Delete: Deletes returns when order is deleted (foreign key constraint)
/// 
/// Schema: "returns" table
/// Indexes:
///   - (TenantId, OrderId) - Query by tenant + order
///   - (TenantId, Status, CreatedAt) - Filter by status + timeline
///   - ReturnDeadline - Deadline-based queries and reminders
/// </summary>
public class ReturnsDbContext : DbContext
{
    public DbSet<Return> Returns { get; set; } = null!;

    public ReturnsDbContext(DbContextOptions<ReturnsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureReturnEntity(modelBuilder);
    }

    /// <summary>
    /// Konfiguriert die Return-Entity mit:
    /// - Tabellen-Mapping: "returns"
    /// - Primary Key: Id (UUID)
    /// - Constraints: TenantId (Required), OrderId (Required)
    /// - Generated Columns: ReturnDeadline, IsWithinDeadline
    /// - Indexes: Multi-tenant, status-based, deadline-based queries
    /// - Foreign Keys: OrderId reference to Orders table (cascade delete)
    /// </summary>
    private static void ConfigureReturnEntity(ModelBuilder modelBuilder)
    {
        var returnEntity = modelBuilder.Entity<Return>();

        // Table mapping
        returnEntity.ToTable("returns");

        // Primary Key
        returnEntity.HasKey(r => r.Id);
        returnEntity.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();  // Explicitly set in entity constructor

        // Required fields
        returnEntity.Property(r => r.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        returnEntity.Property(r => r.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        returnEntity.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        // Return details
        returnEntity.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()  // Enum to string
            .IsRequired();

        returnEntity.Property(r => r.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500);

        // CRITICAL: Delivery Date (NOT order date!)
        returnEntity.Property(r => r.DeliveryDate)
            .HasColumnName("delivery_date")
            .IsRequired();

        // Generated column: delivery_date + 14 days
        returnEntity.Property(r => r.ReturnDeadline)
            .HasColumnName("return_deadline")
            .HasComputedColumnSql("(delivery_date + interval '14 days')", stored: true)
            .IsRequired();

        // Generated column: is delivery_date + 14 days still in future?
        returnEntity.Property(r => r.IsWithinDeadline)
            .HasColumnName("is_within_deadline")
            .HasComputedColumnSql("(CURRENT_TIMESTAMP <= (delivery_date + interval '14 days'))", stored: true)
            .IsRequired();

        // Refund tracking
        returnEntity.Property(r => r.RefundAmount)
            .HasColumnName("refund_amount")
            .HasPrecision(19, 2)  // DECIMAL(19,2) for currency
            .IsRequired();

        returnEntity.Property(r => r.OriginalOrderAmount)
            .HasColumnName("original_order_amount")
            .HasPrecision(19, 2)
            .IsRequired();

        returnEntity.Property(r => r.ShippingDeduction)
            .HasColumnName("shipping_deduction")
            .HasPrecision(19, 2)
            .IsRequired();

        returnEntity.Property(r => r.ItemsCount)
            .HasColumnName("items_count")
            .IsRequired();

        // Carrier information
        returnEntity.Property(r => r.ReturnLabelUrl)
            .HasColumnName("return_label_url")
            .HasMaxLength(2048);

        returnEntity.Property(r => r.CarrierName)
            .HasColumnName("carrier_name")
            .HasMaxLength(100);

        returnEntity.Property(r => r.TrackingNumber)
            .HasColumnName("tracking_number")
            .HasMaxLength(100);

        // Refund processing
        returnEntity.Property(r => r.RefundReference)
            .HasColumnName("refund_reference")
            .HasMaxLength(100);

        returnEntity.Property(r => r.RefundTrackingNumber)
            .HasColumnName("refund_tracking_number")
            .HasMaxLength(100);

        returnEntity.Property(r => r.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasMaxLength(500);

        // Audit trail
        returnEntity.Property(r => r.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        returnEntity.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        returnEntity.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        returnEntity.Property(r => r.CompletedAt)
            .HasColumnName("completed_at");

        // Indexes: Multi-tenant queries
        returnEntity.HasIndex(r => new { r.TenantId, r.OrderId })
            .HasDatabaseName("idx_returns_tenant_order");

        // Indexes: Status-based queries with timeline
        returnEntity.HasIndex(r => new { r.TenantId, r.Status, r.CreatedAt })
            .HasDatabaseName("idx_returns_tenant_status_created");

        // Indexes: Deadline-based queries (reminders, expiration)
        returnEntity.HasIndex(r => r.ReturnDeadline)
            .HasDatabaseName("idx_returns_deadline");

        // Indexes: Customer returns history
        returnEntity.HasIndex(r => new { r.TenantId, r.CustomerId, r.CreatedAt })
            .HasDatabaseName("idx_returns_customer_history");

        // Check constraint: ReturnDeadline >= DeliveryDate (enforces 14-day window)
        returnEntity.ToTable(t => t.HasCheckConstraint("ck_return_deadline_valid",
            "return_deadline >= delivery_date"));

        // Check constraint: RefundAmount <= OriginalOrderAmount
        returnEntity.ToTable(t => t.HasCheckConstraint("ck_refund_amount_valid",
            "refund_amount <= original_order_amount"));

        // Check constraint: ShippingDeduction >= 0
        returnEntity.ToTable(t => t.HasCheckConstraint("ck_shipping_deduction_nonnegative",
            "shipping_deduction >= 0"));

        // Check constraint: ItemsCount >= 1
        returnEntity.ToTable(t => t.HasCheckConstraint("ck_items_count_positive",
            "items_count >= 1"));

        // Check constraint: Status must be valid enum value
        returnEntity.ToTable(t => t.HasCheckConstraint("ck_status_valid",
            "status IN ('Initiated', 'Approved', 'Shipped', 'Refunded', 'Rejected')"));
    }
}
