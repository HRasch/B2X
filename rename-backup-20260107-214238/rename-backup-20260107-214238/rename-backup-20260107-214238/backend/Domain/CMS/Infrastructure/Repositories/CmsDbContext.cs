using System;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.EntityFrameworkCore;

namespace B2X.CMS.Infrastructure.Repositories
{
    /// <summary>
    /// Entity Framework DbContext for CMS domain (ADR-030 Phase 3)
    /// Handles template overrides persistence with tenant isolation
    /// </summary>
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> options)
            : base(options)
        {
        }

        public DbSet<TemplateOverride> TemplateOverrides { get; set; } = null!;
        public DbSet<TemplateOverrideMetadata> TemplateOverrideMetadata { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TemplateOverride configuration
            modelBuilder.Entity<TemplateOverride>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TenantId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TemplateKey)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.BaseTemplateKey)
                    .HasMaxLength(200);

                entity.Property(e => e.TemplateContent)
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.PublishedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.Property(e => e.IsPublished)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Index for efficient tenant + template queries
                entity.HasIndex(e => new { e.TenantId, e.TemplateKey })
                    .IsUnique()
                    .HasDatabaseName("IX_TemplateOverride_Tenant_Template");

                // Index for published overrides
                entity.HasIndex(e => new { e.TenantId, e.IsPublished })
                    .HasDatabaseName("IX_TemplateOverride_Tenant_Published");

                // Index for template key lookups
                entity.HasIndex(e => e.TemplateKey)
                    .HasDatabaseName("IX_TemplateOverride_TemplateKey");
            });

            // TemplateOverrideMetadata configuration
            modelBuilder.Entity<TemplateOverrideMetadata>(entity =>
            {
                entity.HasKey(e => e.OverrideId);

                entity.Property(e => e.OverrideId)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasDefaultValue(1);

                entity.Property(e => e.ValidationStatus)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.IsValidated)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.LastModifiedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.LastModifiedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.ChangeDescription)
                    .HasMaxLength(500);

                // One-to-one relationship with TemplateOverride
                entity.HasOne<TemplateOverride>()
                    .WithOne()
                    .HasForeignKey<TemplateOverrideMetadata>(m => m.OverrideId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for validation status queries
                entity.HasIndex(e => e.ValidationStatus)
                    .HasDatabaseName("IX_TemplateOverrideMetadata_ValidationStatus");

                // Index for live status
                entity.HasIndex(e => e.IsLive)
                    .HasDatabaseName("IX_TemplateOverrideMetadata_IsLive");
            });

            // Configure JSON columns for complex types
            modelBuilder.Entity<TemplateOverride>(entity =>
            {
                entity.Property(e => e.OverrideSections)
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<TemplateOverrideMetadata>(entity =>
            {
                entity.Property(e => e.ValidationResults)
                    .HasColumnType("jsonb");

                entity.Property(e => e.PreviewErrors)
                    .HasColumnType("jsonb");

                entity.Property(e => e.AiSuggestions)
                    .HasColumnType("jsonb");

                entity.Property(e => e.SecurityMetadata)
                    .HasColumnType("jsonb");

                entity.Property(e => e.PerformanceMetrics)
                    .HasColumnType("jsonb");
            });
        }
    }
}
