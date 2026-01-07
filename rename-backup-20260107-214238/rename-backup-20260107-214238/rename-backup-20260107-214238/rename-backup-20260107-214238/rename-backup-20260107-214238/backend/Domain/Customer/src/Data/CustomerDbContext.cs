using B2Connect.Customer.Models;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Customer.Data;

/// <summary>
/// Customer Service DbContext
/// Handles Order, Invoice, and related entities
/// </summary>
public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
    public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ShippingCountry).HasMaxLength(2);
            entity.Property(e => e.BillingCountry).HasMaxLength(2);
            entity.Property(e => e.VatIdValidated).HasMaxLength(50);

            entity.HasIndex(e => new { e.TenantId, e.Id });
            entity.HasIndex(e => e.OrderNumber);
            entity.HasIndex(e => e.CreatedAt);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductSku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.OrderId);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.SellerName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SellerVatId).HasMaxLength(50);
            entity.Property(e => e.SellerAddress).HasMaxLength(1000);
            entity.Property(e => e.BuyerVatId).HasMaxLength(50);
            entity.Property(e => e.BuyerCountry).HasMaxLength(2);
            entity.Property(e => e.ReverseChargeNote).HasMaxLength(500);
            entity.Property(e => e.PdfHash).HasMaxLength(64);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);

            entity.HasIndex(e => new { e.TenantId, e.Id });
            entity.HasIndex(e => e.InvoiceNumber);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.IssuedAt);

            entity.HasMany(e => e.LineItems)
                .WithOne(li => li.Invoice)
                .HasForeignKey(li => li.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // InvoiceLineItem configuration
        modelBuilder.Entity<InvoiceLineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductSku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.InvoiceId);
        });

        // InvoiceTemplate configuration
        modelBuilder.Entity<InvoiceTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CompanyVatId).HasMaxLength(50);
            entity.Property(e => e.CompanyAddress).HasMaxLength(1000);
            entity.Property(e => e.CompanyPhone).HasMaxLength(20);
            entity.Property(e => e.CompanyEmail).HasMaxLength(100);
            entity.Property(e => e.CompanyWebsite).HasMaxLength(200);
            entity.Property(e => e.FooterText).HasMaxLength(2000);
            entity.Property(e => e.PaymentInstructions).HasMaxLength(2000);
            entity.Property(e => e.DeliveryNotes).HasMaxLength(2000);

            entity.HasIndex(e => new { e.TenantId, e.IsDefault });
        });

        // Use snake_case for column names via NamingConventions (PostgreSQL convention)
        // The convention is configured in Program.cs via AddSnakeCaseNamingConvention()
    }
}
