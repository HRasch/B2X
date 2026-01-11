using B2X.Catalog.Core.Entities;
using B2X.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace B2X.Catalog.Infrastructure.Data;

/// <summary>
/// Database context for Catalog Service
///
/// Handles all data access for:
/// - Products
/// - Tax Rates (for price calculations)
/// - Categories
///
/// Multi-tenant isolation: All queries must include TenantId filter
/// </summary>
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    /// <summary>Catalog share rules for tenant/customer include/exclude semantics</summary>
    public DbSet<CatalogShareRule> CatalogShareRules => Set<CatalogShareRule>();

    /// <summary>EU Tax Rates for VAT calculations</summary>
    public DbSet<B2X.Catalog.Core.Entities.TaxRate> TaxRates => Set<B2X.Catalog.Core.Entities.TaxRate>();

    /// <summary>Product categories</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Products in the catalog</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Product variants</summary>
    public DbSet<Variant> Variants => Set<Variant>();

    /// <summary>Catalog imports for tracking imported catalogs</summary>
    public DbSet<CatalogImport> CatalogImports => Set<CatalogImport>();

    /// <summary>Products from imported catalogs</summary>
    public DbSet<CatalogProduct> CatalogProducts => Set<CatalogProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TaxRate entity
        modelBuilder.Entity<B2X.Catalog.Core.Entities.TaxRate>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.CountryCode)
                .HasMaxLength(2)
                .IsRequired();

            entity.Property(x => x.StandardVatRate)
                .HasPrecision(5, 2)  // 0.00 to 999.99
                .IsRequired();

            entity.Property(x => x.ReducedVatRate)
                .HasPrecision(5, 2)  // Optional reduced rate
                .IsRequired(false);

            entity.Property(x => x.EffectiveDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.EndDate)
                .IsRequired(false);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired(false)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique constraint: only one active rate per country
            entity.HasIndex(x => x.CountryCode)
                .IsUnique()
                .HasDatabaseName("IX_TaxRate_CountryCode");

            // Default EU rates (as of 2024)
            entity.HasData(
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "AT", CountryName = "Austria", StandardVatRate = 20m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "BE", CountryName = "Belgium", StandardVatRate = 21m, ReducedVatRate = 6m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "BG", CountryName = "Bulgaria", StandardVatRate = 20m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "HR", CountryName = "Croatia", StandardVatRate = 25m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "CY", CountryName = "Cyprus", StandardVatRate = 19m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "CZ", CountryName = "Czech Republic", StandardVatRate = 21m, ReducedVatRate = 15m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "DK", CountryName = "Denmark", StandardVatRate = 25m, ReducedVatRate = 0m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "EE", CountryName = "Estonia", StandardVatRate = 20m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "FI", CountryName = "Finland", StandardVatRate = 24m, ReducedVatRate = 14m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "FR", CountryName = "France", StandardVatRate = 20m, ReducedVatRate = 5.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "DE", CountryName = "Germany", StandardVatRate = 19m, ReducedVatRate = 7m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "GR", CountryName = "Greece", StandardVatRate = 24m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "HU", CountryName = "Hungary", StandardVatRate = 27m, ReducedVatRate = 18m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "IE", CountryName = "Ireland", StandardVatRate = 23m, ReducedVatRate = 13.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "IT", CountryName = "Italy", StandardVatRate = 22m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "LV", CountryName = "Latvia", StandardVatRate = 21m, ReducedVatRate = 12m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "LT", CountryName = "Lithuania", StandardVatRate = 21m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "LU", CountryName = "Luxembourg", StandardVatRate = 17m, ReducedVatRate = 8m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "MT", CountryName = "Malta", StandardVatRate = 18m, ReducedVatRate = 7m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "NL", CountryName = "Netherlands", StandardVatRate = 21m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "PL", CountryName = "Poland", StandardVatRate = 23m, ReducedVatRate = 8m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "PT", CountryName = "Portugal", StandardVatRate = 23m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "RO", CountryName = "Romania", StandardVatRate = 19m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "SK", CountryName = "Slovakia", StandardVatRate = 20m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "SI", CountryName = "Slovenia", StandardVatRate = 22m, ReducedVatRate = 9.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "ES", CountryName = "Spain", StandardVatRate = 21m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new B2X.Catalog.Core.Entities.TaxRate { Id = Guid.NewGuid(), CountryCode = "SE", CountryName = "Sweden", StandardVatRate = 25m, ReducedVatRate = 12m, EffectiveDate = DateTime.UtcNow, EndDate = null }
            );
        });

        // Configure CatalogImport entity
        modelBuilder.Entity<CatalogImport>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.TenantId)
                .IsRequired();

            entity.Property(x => x.SupplierId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.CatalogId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Version)
                .HasMaxLength(50);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired(false);

            // Composite unique constraint for catalog identification
            entity.HasIndex(x => new { x.TenantId, x.SupplierId, x.CatalogId, x.ImportTimestamp })
                .IsUnique()
                .HasDatabaseName("IX_CatalogImport_CompositeKey");

            // Index for efficient tenant filtering
            entity.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_CatalogImport_TenantId");
        });

        // Configure CatalogProduct entity
        modelBuilder.Entity<CatalogProduct>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.CatalogImportId)
                .IsRequired();

            entity.Property(x => x.SupplierAid)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.ProductData)
                .IsRequired(); // JSON data

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationship
            entity.HasOne(x => x.CatalogImport)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CatalogImportId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for efficient queries
            entity.HasIndex(x => x.CatalogImportId)
                .HasDatabaseName("IX_CatalogProduct_CatalogImportId");

            // Composite index for tenant + supplier article ID
            entity.HasIndex(x => new { x.CatalogImportId, x.SupplierAid })
                .IsUnique()
                .HasDatabaseName("IX_CatalogProduct_ImportId_SupplierAid");
        });

        // Configure CatalogShareRule entity
        modelBuilder.Entity<CatalogShareRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.CatalogShareId)
                .IsRequired();

            entity.Property(x => x.TargetType)
                .IsRequired();

            entity.Property(x => x.TargetId)
                .IsRequired();

            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.Active)
                .IsRequired();

            entity.Property(x => x.AllowOverride)
                .IsRequired();

            // JSON columns stored as text
            entity.Property(x => x.IncludedCategorySlugsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.Property(x => x.ExcludedCategorySlugsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.Property(x => x.IncludedBrandIdsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.Property(x => x.ExcludedBrandIdsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.Property(x => x.IncludedProductIdsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.Property(x => x.ExcludedProductIdsJson)
                .HasColumnType("text")
                .IsRequired(false);

            entity.HasIndex(x => x.CatalogShareId)
                .HasDatabaseName("IX_CatalogShareRule_CatalogShareId");

            entity.HasIndex(x => new { x.TargetType, x.TargetId })
                .HasDatabaseName("IX_CatalogShareRule_Target");
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.TenantId)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Slug)
                .HasMaxLength(200);

            entity.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Icon)
                .HasMaxLength(100);

            entity.Property(x => x.MetaTitle)
                .HasMaxLength(200);

            entity.Property(x => x.MetaDescription)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Self-referencing relationship for parent-child hierarchy
            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of parent

            // Indexes for performance
            entity.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_Categories_TenantId");

            entity.HasIndex(x => new { x.TenantId, x.Slug })
                .IsUnique()
                .HasDatabaseName("IX_Categories_TenantId_Slug");

            entity.HasIndex(x => x.ParentId)
                .HasDatabaseName("IX_Categories_ParentId");

            entity.HasIndex(x => new { x.TenantId, x.IsActive, x.IsVisible })
                .HasDatabaseName("IX_Categories_TenantId_Active_Visible");
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.TenantId)
                .IsRequired();

            entity.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.BrandName)
                .HasMaxLength(200);

            entity.Property(x => x.Barcode)
                .HasMaxLength(100);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes for performance
            entity.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_Products_TenantId");

            entity.HasIndex(x => new { x.TenantId, x.Sku })
                .IsUnique()
                .HasDatabaseName("IX_Products_TenantId_Sku");

            entity.HasIndex(x => new { x.TenantId, x.IsActive })
                .HasDatabaseName("IX_Products_TenantId_IsActive");

            entity.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("IX_Products_CreatedAt");
        });

        // Configure Variant entity
        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.ProductId)
                .IsRequired();

            entity.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.PrimaryImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Barcode)
                .HasMaxLength(100);

            entity.Property(x => x.Weight)
                .HasMaxLength(50);

            entity.Property(x => x.Dimensions)
                .HasMaxLength(100);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign key relationship to Product
            entity.HasOne<Product>()
                .WithMany() // Variants are part of Product aggregate, no navigation property needed
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(x => x.ProductId)
                .HasDatabaseName("IX_Variants_ProductId");

            entity.HasIndex(x => new { x.ProductId, x.Sku })
                .IsUnique()
                .HasDatabaseName("IX_Variants_ProductId_Sku");

            entity.HasIndex(x => new { x.ProductId, x.IsActive })
                .HasDatabaseName("IX_Variants_ProductId_IsActive");
        });

        // Configure Product-Category many-to-many relationship
        modelBuilder.Entity<Product>()
            .HasMany<Category>() // This is a conceptual relationship, not stored
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "ProductCategories",
                j => j
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .HasConstraintName("FK_ProductCategories_Categories_CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .HasConstraintName("FK_ProductCategories_Products_ProductId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("ProductId", "CategoryId")
                        .HasName("PK_ProductCategories");

                    j.HasIndex(new[] { "CategoryId" }, "IX_ProductCategories_CategoryId");
                    j.HasIndex(new[] { "ProductId" }, "IX_ProductCategories_ProductId");
                });
    }
}
