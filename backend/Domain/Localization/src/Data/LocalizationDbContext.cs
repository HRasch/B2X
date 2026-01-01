using Microsoft.EntityFrameworkCore;
using B2Connect.LocalizationService.Models;
using B2Connect.Shared.Core.Interceptors;

namespace B2Connect.LocalizationService.Data;

/// <summary>
/// Entity Framework Core DbContext for localization data with automatic tenant filtering
/// </summary>
public class LocalizationDbContext : B2Connect.Shared.Core.TenantDbContext
{
    /// <summary>Gets or sets the LocalizedStringEntity DbSet</summary>
    public DbSet<LocalizedStringEntity> LocalizedStringEntities { get; set; }

    public LocalizationDbContext(
        DbContextOptions<LocalizationDbContext> options,
        B2Connect.Shared.Core.ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalizedStringEntity>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // Note: EF Core doesn't support indexes on owned type properties directly
            // Unique constraint will be enforced at application level or through database triggers
            // Index for tenant queries
            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_LocalizedStringEntity_TenantId");

            // Index for pagination by CreatedAt
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_LocalizedStringEntity_CreatedAt");

            // Index for pagination by CreatedAt
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_LocalizedStringEntity_CreatedAt");

            // Properties
            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure LocalizedString as owned type
            entity.OwnsOne(e => e.LocalizedString, localizedString =>
            {
                localizedString.Property(ls => ls.Key)
                    .IsRequired()
                    .HasMaxLength(100);

                localizedString.Property(ls => ls.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                localizedString.Property(ls => ls.DefaultValue)
                    .IsRequired()
                    .HasMaxLength(5000);

                localizedString.Property(ls => ls.Translations)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                    )
                    .Metadata.SetValueComparer(
                        new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyDictionary<string, string>>(
                            (c1, c2) => (c1 ?? new Dictionary<string, string>()).SequenceEqual(c2 ?? new Dictionary<string, string>()),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => new Dictionary<string, string>(c ?? new Dictionary<string, string>())
                        )
                    );
            });

            // Note: Query filters are handled by TenantCommandInterceptor for better test compatibility
            // entity.HasQueryFilter(e => e.TenantId == _tenantContext.GetCurrentTenantId());
        });
    }
}
