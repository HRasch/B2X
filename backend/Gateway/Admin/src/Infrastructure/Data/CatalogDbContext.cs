using Microsoft.EntityFrameworkCore;
using B2Connect.Admin.Core.Entities;
using B2Connect.Shared.Core.Entities;
using B2Connect.Shared.Core;
using ITenantContext = B2Connect.Shared.Tenancy.Infrastructure.Context.ITenantContext;

namespace B2Connect.Admin.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for Catalog Service
/// Supports PostgreSQL, SQL Server, and InMemory databases
/// Includes multilingual support via Hybrid Localization Pattern:
/// - Default values in indexed string columns (for admin search)
/// - Translations in JSON LocalizedContent properties
/// </summary>
public class CatalogDbContext : DbContext
{
    /// <summary>Gets or sets the Categories DbSet</summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>Gets or sets the Brands DbSet</summary>
    public DbSet<Brand> Brands { get; set; } = null!;

    /// <summary>Gets or sets the Products DbSet</summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>Gets or sets the ProductVariants DbSet</summary>
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    /// <summary>Gets or sets the ProductAttributes DbSet</summary>
    public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;

    /// <summary>Gets or sets the ProductAttributeOptions DbSet</summary>
    public DbSet<ProductAttributeOption> ProductAttributeOptions { get; set; } = null!;

    /// <summary>Gets or sets the ProductImages DbSet</summary>
    public DbSet<ProductImage> ProductImages { get; set; } = null!;

    /// <summary>Gets or sets the ProductDocuments DbSet</summary>
    public DbSet<ProductDocument> ProductDocuments { get; set; } = null!;

    /// <summary>Gets or sets the ProductCategories DbSet (junction table)</summary>
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;

    /// <summary>Gets or sets the ProductAttributeValues DbSet (junction table)</summary>
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; } = null!;

    /// <summary>Gets or sets the VariantAttributeValues DbSet</summary>
    public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; } = null!;

    private readonly ITenantContext _tenantContext;

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== GLOBAL TENANT FILTER ====================
        // Automatically filter all BaseEntity-derived entities by TenantId
        ApplyGlobalTenantFilter(modelBuilder);

        // ==================== CATEGORY CONFIGURATION ====================
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(4000);

            entity.Property(e => e.MetaDescription)
                .HasMaxLength(500);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);
            entity.ConfigureTranslations(e => e.MetaDescriptionTranslations);

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.ChildCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.ProductCategories)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== BRAND CONFIGURATION ====================
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(4000);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);

            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500);

            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(500);

            entity.HasMany(e => e.Products)
                .WithOne(e => e.Brand)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== PRODUCT ATTRIBUTE CONFIGURATION ====================
        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(100);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);

            entity.Property(e => e.AttributeType)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(e => e.Options)
                .WithOne(e => e.Attribute)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== PRODUCT ATTRIBUTE OPTION CONFIGURATION ====================
        modelBuilder.Entity<ProductAttributeOption>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(100);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(255);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.LabelTranslations);

            entity.Property(e => e.ColorValue)
                .HasMaxLength(7);

            entity.HasOne(e => e.Attribute)
                .WithMany(e => e.Options)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.AttributeId, e.Code }).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Label); // Index for admin search
        });

        // ==================== PRODUCT CONFIGURATION ====================
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.ShortDescription)
                .HasMaxLength(500);

            entity.Property(e => e.Description)
                .HasMaxLength(8000);

            entity.Property(e => e.MetaDescription)
                .HasMaxLength(500);

            entity.Property(e => e.MetaKeywords)
                .HasMaxLength(500);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.ShortDescriptionTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);
            entity.ConfigureTranslations(e => e.MetaDescriptionTranslations);
            entity.ConfigureTranslations(e => e.MetaKeywordsTranslations);

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            entity.Property(e => e.SpecialPrice)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.CostPrice)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(500);

            entity.HasOne(e => e.Brand)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.ProductCategories)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Variants)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Images)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Documents)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AttributeValues)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsFeatured);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== PRODUCT VARIANT CONFIGURATION ====================
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(100);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(4000);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.SpecialPrice)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.CostPrice)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AttributeValues)
                .WithOne(e => e.Variant)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== VARIANT ATTRIBUTE VALUE CONFIGURATION ====================
        modelBuilder.Entity<VariantAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Variant)
                .WithMany(e => e.AttributeValues)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Attribute)
                .WithMany()
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Option)
                .WithMany()
                .HasForeignKey(e => e.OptionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.VariantId, e.AttributeId }).IsUnique();
        });

        // ==================== PRODUCT IMAGE CONFIGURATION ====================
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.AltText)
                .HasMaxLength(255);

            entity.Property(e => e.Title)
                .HasMaxLength(255);

            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(500);

            entity.Property(e => e.MediumUrl)
                .HasMaxLength(500);

            entity.Property(e => e.LargeUrl)
                .HasMaxLength(500);

            entity.Property(e => e.MimeType)
                .HasMaxLength(50);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.IsPrimary);
            entity.HasIndex(e => e.IsActive);
        });

        // ==================== PRODUCT DOCUMENT CONFIGURATION ====================
        modelBuilder.Entity<ProductDocument>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Hybrid Localization Pattern: Default value (indexed) + Translations (JSON)
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            // Configure translations as JSON
            entity.ConfigureTranslations(e => e.NameTranslations);
            entity.ConfigureTranslations(e => e.DescriptionTranslations);

            entity.Property(e => e.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.FileName)
                .HasMaxLength(255);

            entity.Property(e => e.MimeType)
                .HasMaxLength(50);

            entity.Property(e => e.Language)
                .HasMaxLength(10);

            entity.Property(e => e.Version)
                .HasMaxLength(50);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Documents)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.Language);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Name); // Index for admin search
        });

        // ==================== PRODUCT CATEGORY CONFIGURATION ====================
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.CategoryId });

            entity.HasOne(e => e.Product)
                .WithMany(e => e.ProductCategories)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(e => e.ProductCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsPrimary);
        });

        // ==================== PRODUCT ATTRIBUTE VALUE CONFIGURATION ====================
        modelBuilder.Entity<ProductAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.AttributeValues)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Attribute)
                .WithMany()
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Option)
                .WithMany()
                .HasForeignKey(e => e.OptionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.ProductId, e.AttributeId }).IsUnique();
        });

        // Note: Seed data removed - use CatalogDemoDataGenerator for development data
    }

    /// <summary>
    /// Applies global query filters to all BaseEntity-derived entities
    /// Ensures automatic tenant isolation at the DbContext level
    /// Every query automatically filters by the current tenant ID
    /// </summary>
    /// <remarks>
    /// This is a critical security feature: It's impossible to query across tenants
    /// because the filter is applied automatically by Entity Framework Core.
    /// 
    /// If TenantId is empty/default, no entities will match (default behavior).
    /// This prevents accidental data exposure if tenant context is not properly set.
    /// </remarks>
    private void ApplyGlobalTenantFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity inherits from BaseEntity
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Build expression: e => e.TenantId == _tenantContext.TenantId
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "TenantId");
                var constant = System.Linq.Expressions.Expression.Constant(_tenantContext.TenantId);
                var equality = System.Linq.Expressions.Expression.Equal(property, constant);
                var lambda = System.Linq.Expressions.Expression.Lambda(equality, parameter);

                // Apply filter
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
