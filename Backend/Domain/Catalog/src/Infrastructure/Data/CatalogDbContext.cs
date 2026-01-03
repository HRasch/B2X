using B2Connect.Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Catalog.Infrastructure.Data;

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

    /// <summary>EU Tax Rates for VAT calculations</summary>
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();

    /// <summary>Catalog imports for tracking imported catalogs</summary>
    public DbSet<CatalogImport> CatalogImports => Set<CatalogImport>();

    /// <summary>Products from imported catalogs</summary>
    public DbSet<CatalogProduct> CatalogProducts => Set<CatalogProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TaxRate entity
        modelBuilder.Entity<TaxRate>(entity =>
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
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "AT", StandardVatRate = 20m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "BE", StandardVatRate = 21m, ReducedVatRate = 6m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "BG", StandardVatRate = 20m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "HR", StandardVatRate = 25m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "CY", StandardVatRate = 19m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "CZ", StandardVatRate = 21m, ReducedVatRate = 15m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "DK", StandardVatRate = 25m, ReducedVatRate = 0m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "EE", StandardVatRate = 20m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "FI", StandardVatRate = 24m, ReducedVatRate = 14m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "FR", StandardVatRate = 20m, ReducedVatRate = 5.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "DE", StandardVatRate = 19m, ReducedVatRate = 7m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "GR", StandardVatRate = 24m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "HU", StandardVatRate = 27m, ReducedVatRate = 18m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "IE", StandardVatRate = 23m, ReducedVatRate = 13.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "IT", StandardVatRate = 22m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "LV", StandardVatRate = 21m, ReducedVatRate = 12m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "LT", StandardVatRate = 21m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "LU", StandardVatRate = 17m, ReducedVatRate = 8m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "MT", StandardVatRate = 18m, ReducedVatRate = 7m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "NL", StandardVatRate = 21m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "PL", StandardVatRate = 23m, ReducedVatRate = 8m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "PT", StandardVatRate = 23m, ReducedVatRate = 13m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "RO", StandardVatRate = 19m, ReducedVatRate = 9m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "SK", StandardVatRate = 20m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "SI", StandardVatRate = 22m, ReducedVatRate = 9.5m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "ES", StandardVatRate = 21m, ReducedVatRate = 10m, EffectiveDate = DateTime.UtcNow, EndDate = null },
                new TaxRate { Id = Guid.NewGuid(), CountryCode = "SE", StandardVatRate = 25m, ReducedVatRate = 12m, EffectiveDate = DateTime.UtcNow, EndDate = null }
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
    }
}
