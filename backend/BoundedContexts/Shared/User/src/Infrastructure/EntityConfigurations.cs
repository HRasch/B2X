using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B2Connect.Shared.User.Infrastructure;

/// <summary>
/// Entity Type Configuration für Extensible Entities
/// </summary>
public class ExtensibleEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IExtensibleEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // CustomProperties als TEXT (JSON)
        builder
            .Property(e => e.CustomProperties)
            .HasColumnType("text")
            .IsRequired(false);

        // Version für Optimistic Concurrency Control
        builder
            .Property(e => e.Version)
            .HasDefaultValue(1)
            .IsRowVersion();

        // Index für Tenant-Isolation
        builder
            .HasIndex(e => e.TenantId)
            .HasName("idx_tenant_id");
    }
}

/// <summary>
/// User Entity Configuration mit Extension Support
/// </summary>
public class UserConfiguration : ExtensibleEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder
            .Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder
            .HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasName("idx_tenant_email");

        builder
            .HasIndex(u => u.CreatedAt)
            .HasName("idx_created_at");

        // Soft Delete Index
        builder
            .HasIndex(u => new { u.TenantId, u.IsDeleted })
            .HasName("idx_soft_delete");

        // Navigation
        builder
            .HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Extension Schema Configuration
/// </summary>
public class EntityExtensionSchemaConfiguration : IEntityTypeConfiguration<EntityExtensionSchema>
{
    public void Configure(EntityTypeBuilder<EntityExtensionSchema> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.EntityTypeName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.DataType)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(e => e.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(e => e.Description)
            .HasMaxLength(1000);

        builder
            .Property(e => e.ValidationPattern)
            .HasMaxLength(500);

        builder
            .Property(e => e.DefaultValue)
            .HasMaxLength(500);

        builder
            .Property(e => e.IntegrationSource)
            .HasMaxLength(100);

        builder
            .Property(e => e.MappingPath)
            .HasMaxLength(500);

        // Unique: Pro Tenant + Entity Type + Field Name kann es nur ein Schema geben
        builder
            .HasIndex(e => new { e.TenantId, e.EntityTypeName, e.FieldName })
            .IsUnique()
            .HasName("idx_unique_extension_schema");

        builder
            .HasIndex(e => new { e.TenantId, e.IntegrationSource })
            .HasName("idx_integration_source");

        builder
            .HasIndex(e => new { e.TenantId, e.IsActive })
            .HasName("idx_active_schemas");
    }
}

/// <summary>
/// Extension Audit Log Configuration
/// </summary>
public class EntityExtensionAuditLogConfiguration : IEntityTypeConfiguration<EntityExtensionAuditLog>
{
    public void Configure(EntityTypeBuilder<EntityExtensionAuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.EntityTypeName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.OldValue)
            .HasColumnType("text");

        builder
            .Property(e => e.NewValue)
            .HasColumnType("text");

        builder
            .Property(e => e.Reason)
            .HasMaxLength(500);

        // Composite Indexes für schnelle Abfragen
        builder
            .HasIndex(e => new { e.TenantId, e.EntityId, e.EntityTypeName })
            .HasName("idx_entity_audit_logs");

        builder
            .HasIndex(e => new { e.TenantId, e.ChangedAt })
            .HasName("idx_tenant_audit_timeline");

        builder
            .HasIndex(e => e.ChangedAt)
            .HasName("idx_audit_timestamp");

        // Immutable - Never update audit logs
        builder.HasNoDiscriminatorProperty();
    }
}
