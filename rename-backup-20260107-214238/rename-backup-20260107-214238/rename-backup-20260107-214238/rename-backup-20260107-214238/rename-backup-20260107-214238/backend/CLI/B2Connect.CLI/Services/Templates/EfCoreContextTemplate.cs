using System.Text;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.CLI.Services;

public class EfCoreContextTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var contextName = $"{name}DbContext";
        var factoryName = $"{name}DbContextFactory";

        var content = new StringBuilder();
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.EntityFrameworkCore.Design;");
        content.AppendLine("using Microsoft.Extensions.Configuration;");
        content.AppendLine("using Microsoft.Extensions.Logging;");
        content.AppendLine("using System.ComponentModel.DataAnnotations;");
        content.AppendLine();
        content.AppendLine($"namespace B2Connect.{GetNamespaceSuffix(name)}.Data;");
        content.AppendLine();
        content.AppendLine($"// EF Core DbContext with proper configuration and relationships");
        content.AppendLine($"public class {contextName} : DbContext");
        content.AppendLine("{");
        content.AppendLine("    private readonly ILoggerFactory _loggerFactory;");
        if (tenantAware)
        {
            content.AppendLine("    private readonly ITenantContext _tenantContext;");
        }
        content.AppendLine();
        content.AppendLine("    // DbSets - Add your entities here");
        content.AppendLine("    // public DbSet<YourEntity> YourEntities { get; set; } = null!;");
        content.AppendLine();
        content.AppendLine($"    public {contextName}(");
        content.AppendLine("        DbContextOptions<{contextName}> options,");
        content.AppendLine("        ILoggerFactory loggerFactory");
        if (tenantAware)
        {
            content.AppendLine("        , ITenantContext tenantContext");
        }
        content.AppendLine("    ) : base(options)");
        content.AppendLine("    {");
        content.AppendLine("        _loggerFactory = loggerFactory;");
        if (tenantAware)
        {
            content.AppendLine("        _tenantContext = tenantContext;");
        }
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
        content.AppendLine("    {");
        content.AppendLine("        base.OnConfiguring(optionsBuilder);");
        content.AppendLine();
        content.AppendLine("        // Enable sensitive data logging in development only");
        content.AppendLine("        #if DEBUG");
        content.AppendLine("        optionsBuilder");
        content.AppendLine("            .EnableSensitiveDataLogging()");
        content.AppendLine("            .EnableDetailedErrors();");
        content.AppendLine("        #endif");
        content.AppendLine();
        content.AppendLine("        // Use logger factory for EF Core logging");
        content.AppendLine("        optionsBuilder.UseLoggerFactory(_loggerFactory);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    protected override void OnModelCreating(ModelBuilder modelBuilder)");
        content.AppendLine("    {");
        content.AppendLine("        base.OnModelCreating(modelBuilder);");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("        // Apply tenant-aware query filters");
            content.AppendLine("        // modelBuilder.Entity<YourEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);");
            content.AppendLine();
        }
        content.AppendLine("        // Configure entity relationships and constraints");
        content.AppendLine("        // Example:");
        content.AppendLine("        // modelBuilder.Entity<ParentEntity>()");
        content.AppendLine("        //     .HasMany(p => p.Children)");
        content.AppendLine("        //     .WithOne(c => c.Parent)");
        content.AppendLine("        //     .HasForeignKey(c => c.ParentId)");
        content.AppendLine("        //     .OnDelete(DeleteBehavior.Cascade);");
        content.AppendLine();
        content.AppendLine("        // Configure indexes for performance");
        content.AppendLine("        // modelBuilder.Entity<YourEntity>()");
        content.AppendLine("        //     .HasIndex(e => e.TenantId)");
        content.AppendLine("        //     .HasIndex(e => new { e.TenantId, e.CreatedAt });");
        content.AppendLine();
        content.AppendLine("        // Configure value converters for complex types");
        content.AppendLine("        // modelBuilder.Entity<YourEntity>()");
        content.AppendLine("        //     .Property(e => e.JsonData)");
        content.AppendLine("        //     .HasConversion(");
        content.AppendLine("        //         v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),");
        content.AppendLine("        //         v => JsonSerializer.Deserialize<YourType>(v, (JsonSerializerOptions?)null));");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        // Set audit properties for entities being tracked");
        content.AppendLine("        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())");
        content.AppendLine("        {");
        content.AppendLine("            switch (entry.State)");
        content.AppendLine("            {");
        content.AppendLine("                case EntityState.Added:");
        content.AppendLine("                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;");
        if (tenantAware)
        {
            content.AppendLine("                    entry.Entity.TenantId = _tenantContext.TenantId;");
        }
        content.AppendLine("                    break;");
        content.AppendLine("                case EntityState.Modified:");
        content.AppendLine("                    entry.Entity.ModifiedAt = DateTimeOffset.UtcNow;");
        content.AppendLine("                    break;");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        return await base.SaveChangesAsync(cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Design-time factory for EF Core migrations");
        content.AppendLine($"public class {factoryName} : IDesignTimeDbContextFactory<{contextName}>");
        content.AppendLine("{");
        content.AppendLine($"    public {contextName} CreateDbContext(string[] args)");
        content.AppendLine("    {");
        content.AppendLine("        var configuration = new ConfigurationBuilder()");
        content.AppendLine("            .SetBasePath(Directory.GetCurrentDirectory())");
        content.AppendLine("            .AddJsonFile(\"appsettings.json\")");
        content.AppendLine("            .AddJsonFile(\"appsettings.Development.json\", optional: true)");
        content.AppendLine("            .AddEnvironmentVariables()");
        content.AppendLine("            .Build();");
        content.AppendLine();
        content.AppendLine("        var connectionString = configuration.GetConnectionString(\"DefaultConnection\")");
        content.AppendLine("            ?? \"Host=localhost;Port=5432;Database=B2Connect;Username=postgres;Password=password;\";");
        content.AppendLine();
        content.AppendLine("        var optionsBuilder = new DbContextOptionsBuilder<{contextName}>()");
        content.AppendLine("            .UseNpgsql(connectionString, npgsqlOptions =>");
        content.AppendLine("            {");
        content.AppendLine("                npgsqlOptions.EnableRetryOnFailure(");
        content.AppendLine("                    maxRetryCount: 3,");
        content.AppendLine("                    maxRetryDelay: TimeSpan.FromSeconds(30),");
        content.AppendLine("                    errorCodesToAdd: null);");
        content.AppendLine("            });");
        content.AppendLine();
        content.AppendLine("        // Create logger factory for design-time");
        content.AppendLine("        using var loggerFactory = LoggerFactory.Create(builder =>");
        content.AppendLine("        {");
        content.AppendLine("            builder.AddConsole();");
        content.AppendLine("        });");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("        // For migrations, use a design-time tenant context");
            content.AppendLine("        var tenantContext = new DesignTimeTenantContext();");
            content.AppendLine();
            content.AppendLine($"        return new {contextName}(optionsBuilder.Options, loggerFactory, tenantContext);");
        }
        else
        {
            content.AppendLine($"        return new {contextName}(optionsBuilder.Options, loggerFactory);");
        }
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Base class for auditable entities");
        content.AppendLine($"public abstract class AuditableEntity");
        content.AppendLine("{");
        if (tenantAware)
        {
            content.AppendLine("    [Required]");
            content.AppendLine("    public Guid TenantId { get; set; }");
            content.AppendLine();
        }
        content.AppendLine("    public DateTimeOffset CreatedAt { get; set; }");
        content.AppendLine("    public DateTimeOffset? ModifiedAt { get; set; }");
        content.AppendLine("}");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine($"// Design-time tenant context for migrations");
            content.AppendLine($"public class DesignTimeTenantContext : ITenantContext");
            content.AppendLine("{");
            content.AppendLine("    public Guid TenantId => Guid.Empty; // Default for migrations");
            content.AppendLine();
            content.AppendLine("    public void SetTenantId(Guid tenantId)");
            content.AppendLine("    {");
            content.AppendLine("        // No-op for design-time");
            content.AppendLine("    }");
            content.AppendLine("}");
        }

        var warnings = new List<string>();
        warnings.Add("Replace placeholder DbSets with actual entity types");
        warnings.Add("Configure entity relationships in OnModelCreating method");
        warnings.Add("Add appropriate indexes for query performance");
        warnings.Add("Ensure connection string is properly configured in appsettings.json");
        if (tenantAware)
        {
            warnings.Add("Implement ITenantContext interface in your domain");
            warnings.Add("Add tenant-aware query filters for multi-tenant entities");
        }
        warnings.Add("Test migrations with: dotnet ef migrations add Initial --project YourProject.csproj");

        return new Template
        {
            FileName = $"{contextName}.cs",
            Content = content.ToString(),
            Warnings = warnings
        };
    }

    private string GetNamespaceSuffix(string name)
    {
        if (name.Contains("Catalog")) return "Catalog";
        if (name.Contains("Cms")) return "CMS";
        if (name.Contains("Identity")) return "Identity";
        if (name.Contains("Search")) return "Search";
        return "Shared";
    }
}