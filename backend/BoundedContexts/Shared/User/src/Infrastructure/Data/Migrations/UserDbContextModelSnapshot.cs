using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using B2Connect.Shared.User.Infrastructure.Data;

#nullable disable

namespace B2Connect.Shared.User.Infrastructure.Data.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("B2Connect.Shared.User.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedNever()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("AddressType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnName("address_type")
                        .HasColumnType("character varying(50)")
                        .HasDefaultValue("shipping");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnName("city")
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnName("country")
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnName("deleted_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("is_active")
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDefault")
                        .HasColumnName("is_default")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnName("phone_number")
                        .HasColumnType("character varying(20)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnName("postal_code")
                        .HasColumnType("character varying(20)");

                    b.Property<string>("RecipientName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnName("recipient_name")
                        .HasColumnType("character varying(255)");

                    b.Property<string>("State")
                        .HasMaxLength(100)
                        .HasColumnName("state")
                        .HasColumnType("character varying(100)");

                    b.Property<string>("StreetAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnName("street_address")
                        .HasColumnType("character varying(255)");

                    b.Property<string>("StreetAddress2")
                        .HasMaxLength(255)
                        .HasColumnName("street_address2")
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("TenantId")
                        .HasColumnName("tenant_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_addresses");

                    b.HasIndex("TenantId", "IsDeleted")
                        .HasDatabaseName("ix_addresses_tenant_id_is_deleted");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_addresses_user_id");

                    b.ToTable("addresses", (string)null);
                });

            modelBuilder.Entity("B2Connect.Shared.User.Models.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedNever()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("AvatarUrl")
                        .HasMaxLength(500)
                        .HasColumnName("avatar_url")
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Bio")
                        .HasMaxLength(500)
                        .HasColumnName("bio")
                        .HasColumnType("character varying(500)");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(255)
                        .HasColumnName("company_name")
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnName("date_of_birth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Gender")
                        .HasMaxLength(50)
                        .HasColumnName("gender")
                        .HasColumnType("character varying(50)");

                    b.Property<string>("JobTitle")
                        .HasMaxLength(255)
                        .HasColumnName("job_title")
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Nationality")
                        .HasMaxLength(100)
                        .HasColumnName("nationality")
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PreferredLanguage")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasColumnName("preferred_language")
                        .HasColumnType("character varying(10)")
                        .HasDefaultValue("en");

                    b.Property<bool>("ReceiveNewsletter")
                        .HasColumnName("receive_newsletter")
                        .HasColumnType("boolean");

                    b.Property<bool>("ReceivePromotionalEmails")
                        .HasColumnName("receive_promotional_emails")
                        .HasColumnType("boolean");

                    b.Property<string>("Timezone")
                        .HasMaxLength(50)
                        .HasColumnName("timezone")
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TenantId")
                        .HasColumnName("tenant_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_user_profiles");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_user_profiles_tenant_id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_profiles_user_id");

                    b.ToTable("user_profiles", (string)null);
                });

            modelBuilder.Entity("B2Connect.Shared.User.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedNever()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .HasColumnName("created_by")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnName("deleted_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeletedBy")
                        .HasColumnName("deleted_by")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnName("email")
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnName("first_name")
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("is_active")
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnName("is_email_verified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPhoneVerified")
                        .HasColumnName("is_phone_verified")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnName("last_name")
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnName("last_login_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnName("phone_number")
                        .HasColumnType("character varying(20)");

                    b.Property<Guid>("TenantId")
                        .HasColumnName("tenant_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnName("updated_by")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("ix_users_created_at");

                    b.HasIndex("TenantId", "Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_tenant_id_email");

                    b.HasIndex("TenantId", "IsDeleted")
                        .HasDatabaseName("ix_users_tenant_id_is_deleted");

                    b.HasIndex("UpdatedAt")
                        .HasDatabaseName("ix_users_updated_at");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("B2Connect.Shared.User.Models.Address", b =>
                {
                    b.HasOne("B2Connect.Shared.User.Models.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_addresses_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("B2Connect.Shared.User.Models.Profile", b =>
                {
                    b.HasOne("B2Connect.Shared.User.Models.User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("B2Connect.Shared.User.Models.Profile", "UserId")
                        .IsRequired()
                        .HasConstraintName("fk_user_profiles_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("B2Connect.Shared.User.Models.User", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Profile");
                });

#pragma warning restore 612, 618
        }
    }
}
