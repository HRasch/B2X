using Microsoft.EntityFrameworkCore;
using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Data;

/// <summary>
/// Entity Framework Core DbContext for Layout Service
/// Supports PostgreSQL, SQL Server, and InMemory databases
/// </summary>
public class LayoutDbContext : DbContext
{
    public LayoutDbContext(DbContextOptions<LayoutDbContext> options)
        : base(options)
    {
    }

    // Main Entities
    public DbSet<CmsPage> Pages { get; set; }
    public DbSet<CmsSection> Sections { get; set; }
    public DbSet<CmsComponent> Components { get; set; }
    public DbSet<ComponentDefinition> ComponentDefinitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CmsPage Configuration
        modelBuilder.Entity<CmsPage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.TenantId, e.Slug })
                .IsUnique()
                .HasDatabaseName("IX_Pages_TenantId_Slug");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_Pages_TenantId");

            entity.HasIndex(e => new { e.TenantId, e.Visibility })
                .HasDatabaseName("IX_Pages_TenantId_Visibility");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Ignore translation dictionaries for InMemory database
            entity.Ignore(e => e.TitleTranslations);
            entity.Ignore(e => e.SlugTranslations);
            entity.Ignore(e => e.DescriptionTranslations);

            // Relationships
            entity.HasMany(e => e.Sections)
                .WithOne()
                .HasForeignKey("PageId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CmsSection Configuration
        modelBuilder.Entity<CmsSection>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.PageId)
                .HasDatabaseName("IX_Sections_PageId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Ignore complex types for InMemory database
            entity.Ignore(e => e.Settings);
            entity.Ignore(e => e.Styling);

            // Relationships
            entity.HasMany(e => e.Components)
                .WithOne()
                .HasForeignKey("SectionId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CmsComponent Configuration
        modelBuilder.Entity<CmsComponent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.SectionId)
                .HasDatabaseName("IX_Components_SectionId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Content)
                .HasMaxLength(5000);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Ignore complex types for InMemory database
            entity.Ignore(e => e.Variables);
            entity.Ignore(e => e.DataBinding);
            entity.Ignore(e => e.Styling);
            entity.Ignore(e => e.ContentTranslations);
        });

        // ComponentDefinition Configuration
        modelBuilder.Entity<ComponentDefinition>(entity =>
        {
            entity.HasKey(e => e.ComponentType);

            entity.Property(e => e.ComponentType)
                .HasMaxLength(100);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            // Ignore complex types for InMemory database
            entity.Ignore(e => e.Props);
            entity.Ignore(e => e.Slots);
            entity.Ignore(e => e.PresetVariants);
        });

        // Seed Default Components
        SeedDefaultComponents(modelBuilder);
    }

    /// <summary>
    /// Seeds default component definitions into the database
    /// </summary>
    private static void SeedDefaultComponents(ModelBuilder modelBuilder)
    {
        var components = new[]
        {
            new ComponentDefinition
            {
                ComponentType = "Button",
                DisplayName = "Button",
                Description = "A clickable button component",
                Category = "Interactive",
                Icon = "button"
            },
            new ComponentDefinition
            {
                ComponentType = "TextBlock",
                DisplayName = "Text Block",
                Description = "A text content block",
                Category = "Content",
                Icon = "text"
            },
            new ComponentDefinition
            {
                ComponentType = "Image",
                DisplayName = "Image",
                Description = "An image component",
                Category = "Media",
                Icon = "image"
            },
            new ComponentDefinition
            {
                ComponentType = "Form",
                DisplayName = "Form",
                Description = "A form component",
                Category = "Interactive",
                Icon = "form"
            },
            new ComponentDefinition
            {
                ComponentType = "Card",
                DisplayName = "Card",
                Description = "A card container component",
                Category = "Container",
                Icon = "card"
            }
        };

        modelBuilder.Entity<ComponentDefinition>().HasData(components);
    }
}
