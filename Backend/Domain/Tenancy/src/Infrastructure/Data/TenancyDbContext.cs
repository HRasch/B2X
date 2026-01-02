using B2Connect.Shared.Core;
using B2Connect.Tenancy.Models;
using B2Connect.Types.Domain;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Tenancy.Infrastructure.Data;

/// <summary>
/// Database context for Tenancy Service.
/// Manages Tenants and their associated domains.
/// </summary>
public class TenancyDbContext : DbContext
{
    public TenancyDbContext(DbContextOptions<TenancyDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tenants in the system.
    /// </summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>
    /// Domains associated with tenants.
    /// </summary>
    public DbSet<TenantDomain> TenantDomains => Set<TenantDomain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Tenant entity
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Slug)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("ix_tenants_slug");

            entity.Property(x => x.LogoUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.SuspensionReason)
                .HasMaxLength(500);

            entity.Property(x => x.Metadata)
                .HasColumnType("jsonb");

            entity.Property(x => x.LocalizedDescription)
                .HasColumnType("jsonb");

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.CreatedBy)
                .HasMaxLength(255);

            entity.Property(x => x.UpdatedBy)
                .HasMaxLength(255);
        });

        // Configure TenantDomain entity
        modelBuilder.Entity<TenantDomain>(entity =>
        {
            entity.ToTable("tenant_domains");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.DomainName)
                .HasMaxLength(255)
                .IsRequired();

            // Domain name must be unique across all tenants
            entity.HasIndex(x => x.DomainName)
                .IsUnique()
                .HasDatabaseName("ix_tenant_domains_domain_name");

            // Index for fast tenant lookup by domain
            entity.HasIndex(x => new { x.TenantId, x.IsPrimary })
                .HasDatabaseName("ix_tenant_domains_tenant_primary");

            // Index for verification status queries
            entity.HasIndex(x => x.VerificationStatus)
                .HasDatabaseName("ix_tenant_domains_verification_status");

            // SSL indexes will be added in Phase 3
            // entity.HasIndex(x => x.SslExpiresAt)
            //     .HasDatabaseName("ix_tenant_domains_ssl_expires");

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.VerificationToken)
                .HasMaxLength(100);

            entity.Property(x => x.SslStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.CreatedBy)
                .HasMaxLength(255);

            entity.Property(x => x.UpdatedBy)
                .HasMaxLength(255);

            // Foreign key to Tenant
            entity.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default tenant for InMemory/demo mode
        SeedDefaultTenant(modelBuilder);
    }

    /// <summary>
    /// Seeds the default tenant and domain for InMemory/demo mode.
    /// Uses SeedConstants.DefaultTenantId for consistency across all services.
    /// </summary>
    private static void SeedDefaultTenant(ModelBuilder modelBuilder)
    {
        // Seed default tenant
        modelBuilder.Entity<Tenant>().HasData(new Tenant
        {
            Id = SeedConstants.DefaultTenantId,
            Name = SeedConstants.DefaultTenantName,
            Slug = SeedConstants.DefaultTenantSlug,
            Status = TenantStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            UpdatedBy = "system"
        });

        // Seed default domain for the tenant
        modelBuilder.Entity<TenantDomain>().HasData(new TenantDomain
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            TenantId = SeedConstants.DefaultTenantId,
            DomainName = SeedConstants.DefaultTenantDomain,
            Type = DomainType.Subdomain,
            IsPrimary = true,
            VerificationStatus = DomainVerificationStatus.Verified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            UpdatedBy = "system"
        });
    }
}
