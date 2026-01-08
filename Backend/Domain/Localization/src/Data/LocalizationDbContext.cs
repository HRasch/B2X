using B2X.LocalizationService.Models;
using Microsoft.EntityFrameworkCore;

namespace B2X.LocalizationService.Data;

/// <summary>
/// Entity Framework Core DbContext for localization data
/// </summary>
public class LocalizationDbContext : DbContext
{
    /// <summary>Gets or sets the LocalizedString DbSet</summary>
    public DbSet<LocalizedString> LocalizedStrings { get; set; }

    public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalizedString>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // Unique index on Key + Category (per tenant)
            entity.HasIndex(e => new { e.Key, e.Category, e.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_LocalizedString_KeyCategoryTenant");

            // Index for tenant queries
            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_LocalizedString_TenantId");

            // Index for category queries
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("IX_LocalizedString_Category");

            // Index for pagination by CreatedAt
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_LocalizedString_CreatedAt");

            // Properties
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DefaultValue)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedBy)
                .IsRequired(false);

            entity.Property(e => e.Translations)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>(StringComparer.Ordinal)
                )
                .Metadata.SetValueComparer(
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, string>>(
                        (c1, c2) => (c1 ?? new(StringComparer.Ordinal)).SequenceEqual(c2 ?? new(StringComparer.Ordinal)),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => new Dictionary<string, string>(c ?? new(StringComparer.Ordinal), StringComparer.Ordinal)
                    )
                );

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
