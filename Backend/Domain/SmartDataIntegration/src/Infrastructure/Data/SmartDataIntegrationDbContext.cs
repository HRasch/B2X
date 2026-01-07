using B2Connect.SmartDataIntegration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace B2Connect.SmartDataIntegration.Infrastructure.Data;

/// <summary>
/// Database context for Smart Data Integration Service
///
/// Handles all data access for:
/// - Data mapping configurations
/// - Mapping rules and suggestions
/// - Validation results and execution history
/// - AI learning data
///
/// Multi-tenant isolation: All queries must include TenantId filter
/// </summary>
public class SmartDataIntegrationDbContext : DbContext
{
    public SmartDataIntegrationDbContext(DbContextOptions<SmartDataIntegrationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Data mapping configurations</summary>
    public DbSet<DataMappingConfiguration> DataMappingConfigurations => Set<DataMappingConfiguration>();

    /// <summary>Individual mapping rules within configurations</summary>
    public DbSet<MappingRule> MappingRules => Set<MappingRule>();

    /// <summary>AI-generated mapping suggestions</summary>
    public DbSet<MappingSuggestion> MappingSuggestions => Set<MappingSuggestion>();

    /// <summary>Results of mapping validations</summary>
    public DbSet<MappingValidationResult> MappingValidationResults => Set<MappingValidationResult>();

    /// <summary>Results of mapping executions</summary>
    public DbSet<MappingExecutionResult> MappingExecutionResults => Set<MappingExecutionResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DataMappingConfiguration entity
        modelBuilder.Entity<DataMappingConfiguration>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.TenantId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(x => x.SourceSystem)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.TargetSystem)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Version)
                .IsRequired()
                .HasDefaultValue(1);

            entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(x => x.AiConfidenceScore)
                .HasPrecision(5, 2);

            entity.Property(x => x.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.ModifiedBy)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.ModifiedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes
            entity.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_DataMappingConfiguration_TenantId");

            entity.HasIndex(x => new { x.TenantId, x.SourceSystem, x.TargetSystem })
                .HasDatabaseName("IX_DataMappingConfiguration_Tenant_Source_Target");

            entity.HasIndex(x => new { x.TenantId, x.IsActive })
                .HasDatabaseName("IX_DataMappingConfiguration_Tenant_Active");
        });

        // Configure MappingRule entity
        modelBuilder.Entity<MappingRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.TransformationParameters)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.ValidationRules)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.AiConfidenceScore)
                .HasPrecision(5, 2)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(x => x.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.ModifiedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            entity.HasOne(x => x.DataMappingConfiguration)
                .WithMany(x => x.MappingRules)
                .HasForeignKey(x => x.DataMappingConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(x => x.DataMappingConfigurationId)
                .HasDatabaseName("IX_MappingRule_ConfigurationId");

            entity.HasIndex(x => new { x.DataMappingConfigurationId, x.IsActive })
                .HasDatabaseName("IX_MappingRule_Configuration_Active");
        });

        // Configure MappingSuggestion entity
        modelBuilder.Entity<MappingSuggestion>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.ConfidenceScore)
                .HasPrecision(5, 2)
                .IsRequired();

            entity.Property(x => x.TransformationParameters)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.Reasoning)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(x => x.UserFeedback)
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(x => x.IsAccepted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.IsRejected)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes
            entity.HasIndex(x => x.DataMappingConfigurationId)
                .HasDatabaseName("IX_MappingSuggestion_ConfigurationId");

            entity.HasIndex(x => new { x.DataMappingConfigurationId, x.IsAccepted, x.IsRejected })
                .HasDatabaseName("IX_MappingSuggestion_Configuration_Status");
        });

        // Configure MappingValidationResult entity
        modelBuilder.Entity<MappingValidationResult>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.IsValid)
                .IsRequired();

            entity.Property(x => x.ValidationScore)
                .HasPrecision(5, 2)
                .IsRequired();

            entity.Property(x => x.SampleData)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.ValidatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(x => x.ValidatedBy)
                .HasMaxLength(100)
                .IsRequired();

            // Indexes
            entity.HasIndex(x => x.DataMappingConfigurationId)
                .HasDatabaseName("IX_MappingValidationResult_ConfigurationId");

            entity.HasIndex(x => new { x.DataMappingConfigurationId, x.ValidatedAt })
                .HasDatabaseName("IX_MappingValidationResult_Configuration_Date");
        });

        // Configure MappingExecutionResult entity
        modelBuilder.Entity<MappingExecutionResult>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.IsSuccessful)
                .IsRequired();

            entity.Property(x => x.OutputData)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.InputData)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.Property(x => x.StartedAt)
                .IsRequired();

            entity.Property(x => x.CompletedAt)
                .IsRequired();

            entity.Property(x => x.ExecutedBy)
                .HasMaxLength(100)
                .IsRequired();

            // Indexes
            entity.HasIndex(x => x.DataMappingConfigurationId)
                .HasDatabaseName("IX_MappingExecutionResult_ConfigurationId");

            entity.HasIndex(x => new { x.DataMappingConfigurationId, x.StartedAt })
                .HasDatabaseName("IX_MappingExecutionResult_Configuration_Date");

            entity.HasIndex(x => new { x.DataMappingConfigurationId, x.IsSuccessful })
                .HasDatabaseName("IX_MappingExecutionResult_Configuration_Success");
        });

        // Configure owned entities for MappingExecutionResult
        modelBuilder.Entity<MappingExecutionResult>()
            .OwnsMany(x => x.Errors, errors =>
            {
                errors.Property(e => e.Field).HasMaxLength(100).IsRequired();
                errors.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                errors.Property(e => e.RuleId).HasMaxLength(36).IsRequired(false);
            });

        modelBuilder.Entity<MappingExecutionResult>()
            .OwnsMany(x => x.Warnings, warnings =>
            {
                warnings.Property(w => w.Field).HasMaxLength(100).IsRequired();
                warnings.Property(w => w.Message).HasMaxLength(1000).IsRequired();
                warnings.Property(w => w.RuleId).HasMaxLength(36).IsRequired(false);
            });

        modelBuilder.Entity<MappingExecutionResult>()
            .OwnsOne(x => x.Metrics);

        // Configure owned entities for MappingValidationResult
        modelBuilder.Entity<MappingValidationResult>()
            .OwnsMany(x => x.Errors, errors =>
            {
                errors.Property(e => e.Field).HasMaxLength(100).IsRequired();
                errors.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                errors.Property(e => e.Severity).IsRequired();
            });

        modelBuilder.Entity<MappingValidationResult>()
            .OwnsMany(x => x.Warnings, warnings =>
            {
                warnings.Property(w => w.Field).HasMaxLength(100).IsRequired();
                warnings.Property(w => w.Message).HasMaxLength(1000).IsRequired();
            });

        modelBuilder.Entity<MappingValidationResult>()
            .OwnsOne(x => x.PerformanceMetrics);

        // Configure owned entities for complex properties
        modelBuilder.Entity<MappingRule>(entity =>
        {
            entity.OwnsOne(x => x.SourceField, field =>
            {
                field.Property(f => f.Name).HasMaxLength(100).IsRequired();
                field.Property(f => f.DisplayName).HasMaxLength(200).IsRequired();
                field.Property(f => f.MaxLength).IsRequired(false);
                field.Property(f => f.DefaultValue).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.Description).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.BusinessContext).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.SampleValues).HasColumnType("jsonb").IsRequired(false);
            });

            entity.OwnsOne(x => x.TargetField, field =>
            {
                field.Property(f => f.Name).HasMaxLength(100).IsRequired();
                field.Property(f => f.DisplayName).HasMaxLength(200).IsRequired();
                field.Property(f => f.MaxLength).IsRequired(false);
                field.Property(f => f.DefaultValue).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.Description).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.BusinessContext).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.SampleValues).HasColumnType("jsonb").IsRequired(false);
            });
        });

        modelBuilder.Entity<MappingSuggestion>(entity =>
        {
            entity.OwnsOne(x => x.SourceField, field =>
            {
                field.Property(f => f.Name).HasMaxLength(100).IsRequired();
                field.Property(f => f.DisplayName).HasMaxLength(200).IsRequired();
                field.Property(f => f.MaxLength).IsRequired(false);
                field.Property(f => f.DefaultValue).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.Description).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.BusinessContext).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.SampleValues).HasColumnType("jsonb").IsRequired(false);
            });

            entity.OwnsOne(x => x.SuggestedTargetField, field =>
            {
                field.Property(f => f.Name).HasMaxLength(100).IsRequired();
                field.Property(f => f.DisplayName).HasMaxLength(200).IsRequired();
                field.Property(f => f.MaxLength).IsRequired(false);
                field.Property(f => f.DefaultValue).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.Description).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.BusinessContext).HasMaxLength(500).IsRequired(false);
                field.Property(f => f.SampleValues).HasColumnType("jsonb").IsRequired(false);
            });
        });
    }
}

/// <summary>
/// Design-time factory for EF Core migrations
/// </summary>
public class SmartDataIntegrationDbContextFactory : IDesignTimeDbContextFactory<SmartDataIntegrationDbContext>
{
    public SmartDataIntegrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartDataIntegrationDbContext>();

        // Use a default connection string for migrations
        // In production, this will be overridden by the actual connection string
        var connectionString = "Host=localhost;Port=5432;Database=b2connect_smartdataintegration;Username=postgres;Password=password";

        optionsBuilder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new SmartDataIntegrationDbContext(optionsBuilder.Options);
    }
}
