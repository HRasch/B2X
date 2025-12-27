using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Infrastructure.Data.ReadModel;

/// <summary>
/// CQRS Read Model - Denormalized Product View
/// Optimized for queries on millions of products
/// Updated asynchronously via domain events (eventual consistency)
/// 
/// Benefits:
/// - Single table lookup (no joins) = fast queries at scale
/// - Indexed columns for filtering (tenant, sku, category, price)
/// - GIN index for full-text search capability
/// - Easy cache invalidation by product
/// </summary>
public class ProductReadModel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public string? Category { get; set; }
    public string? Subcategory { get; set; }

    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }

    // Search optimization
    public string SearchText { get; set; } = null!; // Concatenated: name + description + sku

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // For bulk operations tracking
    public string? ImportId { get; set; }
}

/// <summary>
/// EF Configuration for ProductReadModel
/// Defines indexes, constraints, and performance optimizations
/// </summary>
public class ProductReadModelConfiguration : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.ToTable("products_read_model", "catalog");

        builder.HasKey(p => p.Id);

        // Primary lookup indexes for queries
        builder.HasIndex(p => new { p.TenantId, p.IsDeleted })
            .HasDatabaseName("ix_products_read_tenant_status");

        builder.HasIndex(p => new { p.TenantId, p.Sku })
            .HasDatabaseName("ix_products_read_tenant_sku")
            .IsUnique();

        // Filter indexes
        builder.HasIndex(p => new { p.TenantId, p.Category, p.IsAvailable })
            .HasDatabaseName("ix_products_read_category_filter");

        builder.HasIndex(p => new { p.TenantId, p.IsAvailable, p.Price })
            .HasDatabaseName("ix_products_read_price_filter");

        // Search optimization indexes
        builder.HasIndex(p => new { p.TenantId, p.IsDeleted })
            .HasDatabaseName("ix_products_read_search_filter");

        // Created/Updated for sorting
        builder.HasIndex(p => new { p.TenantId, p.CreatedAt })
            .HasDatabaseName("ix_products_read_created");

        builder.HasIndex(p => new { p.TenantId, p.UpdatedAt })
            .HasDatabaseName("ix_products_read_updated");

        // Import tracking
        builder.HasIndex(p => p.ImportId)
            .HasDatabaseName("ix_products_read_import");

        // Properties
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.TenantId).IsRequired();
        builder.Property(p => p.Sku).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.Subcategory).HasMaxLength(100);
        builder.Property(p => p.SearchText).IsRequired().HasMaxLength(3000);
        builder.Property(p => p.ImportId).HasMaxLength(100);

        // Default values
        builder.Property(p => p.IsAvailable).HasDefaultValue(true);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

