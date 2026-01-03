using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.MCP.Data;

/// <summary>
/// Database context for MCP server
/// </summary>
public class McpDbContext : DbContext
{
    public McpDbContext(DbContextOptions<McpDbContext> options)
        : base(options)
    {
    }

    // AI Configuration tables
    public DbSet<TenantAiConfiguration> TenantAiConfigurations { get; set; } = null!;
    public DbSet<AiProviderConfiguration> AiProviderConfigurations { get; set; } = null!;

    // System Prompts tables
    public DbSet<SystemPrompt> SystemPrompts { get; set; } = null!;
    public DbSet<PromptVersion> PromptVersions { get; set; } = null!;
    public DbSet<PromptAuditLog> PromptAuditLogs { get; set; } = null!;

    // AI Consumption tracking
    public DbSet<AiConsumptionRecord> AiConsumptionRecords { get; set; } = null!;
    public DbSet<AiUsageMetrics> AiUsageMetrics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant AI Configuration
        modelBuilder.Entity<TenantAiConfiguration>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Provider });
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.ApiKeyEncrypted).IsRequired();
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        });

        // AI Provider Configuration
        modelBuilder.Entity<AiProviderConfiguration>(entity =>
        {
            entity.HasKey(e => e.Provider);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.BaseUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // System Prompts
        modelBuilder.Entity<SystemPrompt>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.ToolType, e.Key });
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.ToolType).HasMaxLength(100);
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Version).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        });

        // Prompt Versions
        modelBuilder.Entity<PromptVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.ToolType).HasMaxLength(100);
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => new { e.TenantId, e.ToolType, e.Key, e.Version });
        });

        // Prompt Audit Log
        modelBuilder.Entity<PromptAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.UserId).HasMaxLength(36);
            entity.Property(e => e.ToolType).HasMaxLength(100);
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // AI Consumption Records
        modelBuilder.Entity<AiConsumptionRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.ToolName).HasMaxLength(100);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.RequestId).HasMaxLength(36);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("NOW()");
            entity.Property(e => e.Cost).HasPrecision(10, 4);
            entity.HasIndex(e => new { e.TenantId, e.Timestamp });
            entity.HasIndex(e => e.RequestId).IsUnique();
        });

        // AI Usage Metrics
        modelBuilder.Entity<AiUsageMetrics>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Date });
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.TotalTokens).HasDefaultValue(0);
            entity.Property(e => e.TotalCost).HasPrecision(10, 4).HasDefaultValue(0);
            entity.Property(e => e.RequestCount).HasDefaultValue(0);
        });
    }
}

// Entity Models

public class TenantAiConfiguration
{
    public string TenantId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ApiKeyEncrypted { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal MonthlyBudget { get; set; } = 1000m;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AiProviderConfiguration
{
    public string Provider { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string SupportedModels { get; set; } = string.Empty; // JSON array
}

public class SystemPrompt
{
    public string TenantId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PromptVersion
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Content { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PromptAuditLog
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AiConsumptionRecord
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public int TokensUsed { get; set; }
    public decimal Cost { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class AiUsageMetrics
{
    public string TenantId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int TotalTokens { get; set; }
    public decimal TotalCost { get; set; }
    public int RequestCount { get; set; }
}