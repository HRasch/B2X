using Microsoft.EntityFrameworkCore;
using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Store.Entities;

namespace B2X.Store.Infrastructure.Common.Data;

/// <summary>
/// Entity Framework Core DbContext for Store Service
/// Manages Store, Language, Country, PaymentMethod, and ShippingMethod entities
/// Common data context used by all domains (Store, Admin, Common)
/// </summary>
public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    public DbSet<Shop> Shops { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
    public DbSet<ShippingMethod> ShippingMethods { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Store configuration
        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsMainShop);

            entity.HasOne(e => e.Country)
                .WithMany(c => c.Shops)
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.DefaultLanguage)
                .WithMany(l => l.Shops)
                .HasForeignKey(e => e.DefaultLanguageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Language configuration
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.DisplayOrder);

            entity.Property(e => e.Code).HasMaxLength(10);
        });

        // Country configuration
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => e.IsShippingEnabled);
            entity.HasIndex(e => e.DisplayOrder);

            entity.Property(e => e.Code).HasMaxLength(2);
            entity.Property(e => e.CodeAlpha3).HasMaxLength(3);
        });

        // PaymentMethod configuration
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.DisplayOrder);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);

            entity.HasOne(e => e.Shop)
                .WithMany()
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ShippingMethod configuration
        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.DisplayOrder);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Carrier).HasMaxLength(50);
            entity.Property(e => e.ServiceName).HasMaxLength(100);

            entity.HasOne(e => e.Shop)
                .WithMany()
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

