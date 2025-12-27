using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models = B2Connect.Shared.User.Models;

namespace B2Connect.Shared.User.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for User domain
/// Manages User, Profile, and Address entities
/// Implements multi-tenancy via TenantId
/// Implements soft deletes via IsDeleted flag
/// </summary>
public class UserDbContext : DbContext
{
    public DbSet<Models.User> Users => Set<Models.User>();
    public DbSet<Models.Profile> Profiles => Set<Models.Profile>();
    public DbSet<Models.Address> Addresses => Set<Models.Address>();

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User entity
        ConfigureUser(builder);

        // Configure Profile entity
        ConfigureProfile(builder);

        // Configure Address entity
        ConfigureAddress(builder);
    }

    private static void ConfigureUser(ModelBuilder builder)
    {
        var entity = builder.Entity<Models.User>();

        entity.ToTable("users");

        entity.HasKey(u => u.Id);

        entity.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        entity.Property(u => u.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        entity.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(u => u.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        entity.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        entity.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasDefaultValue(false);

        entity.Property(u => u.IsPhoneVerified)
            .HasColumnName("is_phone_verified")
            .HasDefaultValue(false);

        entity.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        entity.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        entity.Property(u => u.CreatedBy)
            .HasColumnName("created_by");

        entity.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        entity.Property(u => u.UpdatedBy)
            .HasColumnName("updated_by");

        entity.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at");

        entity.Property(u => u.DeletedBy)
            .HasColumnName("deleted_by");

        entity.Property(u => u.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        // Relationships
        entity.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Models.Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique(false)
            .HasDatabaseName("ix_users_tenant_email");

        entity.HasIndex(u => new { u.TenantId, u.IsDeleted })
            .HasDatabaseName("ix_users_tenant_deleted");

        entity.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("ix_users_deleted");

        entity.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("ix_users_created_at");

        entity.HasIndex(u => u.UpdatedAt)
            .HasDatabaseName("ix_users_updated_at");
    }

    private static void ConfigureProfile(ModelBuilder builder)
    {
        var entity = builder.Entity<Models.Profile>();

        entity.ToTable("user_profiles");

        entity.HasKey(p => p.Id);

        entity.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        entity.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        entity.Property(p => p.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        entity.Property(p => p.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(500);

        entity.Property(p => p.Bio)
            .HasColumnName("bio")
            .HasMaxLength(500);

        entity.Property(p => p.DateOfBirth)
            .HasColumnName("date_of_birth")
            .HasMaxLength(10);

        entity.Property(p => p.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(255);

        entity.Property(p => p.JobTitle)
            .HasColumnName("job_title")
            .HasMaxLength(255);

        entity.Property(p => p.PreferredLanguage)
            .HasColumnName("preferred_language")
            .HasMaxLength(10)
            .HasDefaultValue("de");

        entity.Property(p => p.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50)
            .HasDefaultValue("Europe/Berlin");

        entity.Property(p => p.ReceiveNewsletter)
            .HasColumnName("receive_newsletter")
            .HasDefaultValue(true);

        entity.Property(p => p.ReceivePromotionalEmails)
            .HasColumnName("receive_promotional_emails")
            .HasDefaultValue(false);

        entity.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        entity.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        // Indexes
        entity.HasIndex(p => p.UserId)
            .HasDatabaseName("ix_user_profiles_user_id");

        entity.HasIndex(p => p.TenantId)
            .HasDatabaseName("ix_user_profiles_tenant_id");
    }

    private static void ConfigureAddress(ModelBuilder builder)
    {
        var entity = builder.Entity<Models.Address>();

        entity.ToTable("addresses");

        entity.HasKey(a => a.Id);

        entity.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        entity.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        entity.Property(a => a.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        entity.Property(a => a.AddressType)
            .HasColumnName("address_type")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Models.Enums.AddressType.Shipping);

        entity.Property(a => a.StreetAddress)
            .HasColumnName("street_address")
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(a => a.StreetAddress2)
            .HasColumnName("street_address2")
            .HasMaxLength(255);

        entity.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(a => a.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(a => a.Country)
            .HasColumnName("country")
            .HasMaxLength(2)
            .IsRequired();

        entity.Property(a => a.State)
            .HasColumnName("state")
            .HasMaxLength(100);

        entity.Property(a => a.RecipientName)
            .HasColumnName("recipient_name")
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(a => a.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        entity.Property(a => a.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false);

        entity.Property(a => a.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        entity.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        entity.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        entity.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at");

        entity.Property(a => a.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        // Indexes
        entity.HasIndex(a => new { a.UserId, a.IsDeleted })
            .HasDatabaseName("ix_addresses_user_deleted");

        entity.HasIndex(a => a.TenantId)
            .HasDatabaseName("ix_addresses_tenant_id");

        entity.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("ix_addresses_created_at");
    }
}
