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

    // Conversation Memory
    public DbSet<Conversation> Conversations { get; set; } = null!;
    public DbSet<ConversationMessage> ConversationMessages { get; set; } = null!;
    public DbSet<ConversationContext> ConversationContexts { get; set; } = null!;

    // A/B Testing
    public DbSet<AbTest> AbTests { get; set; } = null!;
    public DbSet<AbTestVariant> AbTestVariants { get; set; } = null!;
    public DbSet<AbTestResult> AbTestResults { get; set; } = null!;

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

        // Conversation Memory
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.UserId).HasMaxLength(36);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => new { e.TenantId, e.UserId, e.CreatedAt });
        });

        modelBuilder.Entity<ConversationMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.ConversationId).IsRequired();
            entity.Property(e => e.Sender).HasMaxLength(20);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.ToolName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => e.ConversationId);
            entity.HasIndex(e => new { e.ConversationId, e.CreatedAt });
        });

        modelBuilder.Entity<ConversationContext>(entity =>
        {
            entity.HasKey(e => new { e.ConversationId, e.Key });
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        });

        // A/B Testing
        modelBuilder.Entity<AbTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TenantId).HasMaxLength(36);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ToolName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => new { e.TenantId, e.Status });
        });

        modelBuilder.Entity<AbTestVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TestId).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Weight).HasDefaultValue(1.0m);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => e.TestId);
        });

        modelBuilder.Entity<AbTestResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.TestId).IsRequired();
            entity.Property(e => e.VariantId).IsRequired();
            entity.Property(e => e.ConversationId).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(36);
            entity.Property(e => e.MetricType).HasMaxLength(50);
            entity.Property(e => e.MetricValue).HasPrecision(10, 4);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => new { e.TestId, e.CreatedAt });
            entity.HasIndex(e => new { e.VariantId, e.MetricType });
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

// Conversation Memory Models
public class Conversation
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CurrentPage { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
    public ICollection<ConversationContext> Context { get; set; } = new List<ConversationContext>();
}

public class ConversationMessage
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Sender { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public string? ToolName { get; set; }
    public string? ToolArgs { get; set; } // JSON
    public bool IsError { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Conversation Conversation { get; set; } = null!;
}

public class ConversationContext
{
    public int ConversationId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Conversation Conversation { get; set; } = null!;
}

// A/B Testing Models
public class AbTest
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public string Status { get; set; } = "draft"; // draft, active, paused, completed
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<AbTestVariant> Variants { get; set; } = new List<AbTestVariant>();
    public ICollection<AbTestResult> Results { get; set; } = new List<AbTestResult>();
}

public class AbTestVariant
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
    public decimal Weight { get; set; } = 1.0m;
    public bool IsControl { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public AbTest Test { get; set; } = null!;
}

public class AbTestResult
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int VariantId { get; set; }
    public int ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MetricType { get; set; } = string.Empty; // "user_satisfaction", "task_completion", "response_time"
    public decimal MetricValue { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public AbTest Test { get; set; } = null!;
    public AbTestVariant Variant { get; set; } = null!;
}