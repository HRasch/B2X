using System.Text;

namespace B2X.CLI.Services;

public class RepositoryTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var repositoryName = $"I{name}Repository";
        var implementationName = $"{name}Repository";
        var entityName = name.Replace("Repository", "").Replace("Repo", "");

        var content = new StringBuilder();
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.Extensions.Logging;");
        content.AppendLine("using System.ComponentModel.DataAnnotations;");
        content.AppendLine("using Polly;");
        content.AppendLine("using Polly.Retry;");
        content.AppendLine();
        content.AppendLine($"namespace B2X.{GetNamespaceSuffix(name)}.Data;");
        content.AppendLine();
        content.AppendLine($"// Repository interface with common operations");
        content.AppendLine($"public interface {repositoryName}");
        content.AppendLine("{");
        content.AppendLine($"    Task<{entityName}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);");
        content.AppendLine($"    Task<IEnumerable<{entityName}>> GetAllAsync(CancellationToken cancellationToken = default);");
        content.AppendLine($"    Task<{entityName}> CreateAsync({entityName} entity, CancellationToken cancellationToken = default);");
        content.AppendLine($"    Task<{entityName}> UpdateAsync({entityName} entity, CancellationToken cancellationToken = default);");
        content.AppendLine($"    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);");
        content.AppendLine($"    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Repository implementation with resilience patterns");
        content.AppendLine($"public class {implementationName} : {repositoryName}");
        content.AppendLine("{");
        content.AppendLine("    private readonly DbContext _context;");
        content.AppendLine("    private readonly ILogger<{implementationName}> _logger;");
        content.AppendLine("    private readonly ResiliencePipeline _resiliencePipeline;");
        if (tenantAware)
        {
            content.AppendLine("    private readonly ITenantContext _tenantContext;");
        }
        content.AppendLine();
        content.AppendLine($"    public {implementationName}(");
        content.AppendLine("        DbContext context,");
        content.AppendLine("        ILogger<{implementationName}> logger,");
        content.AppendLine("        ResiliencePipeline resiliencePipeline");
        if (tenantAware)
        {
            content.AppendLine("        , ITenantContext tenantContext");
        }
        content.AppendLine("    )");
        content.AppendLine("    {");
        content.AppendLine("        _context = context ?? throw new ArgumentNullException(nameof(context));");
        content.AppendLine("        _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
        content.AppendLine("        _resiliencePipeline = resiliencePipeline ?? throw new ArgumentNullException(nameof(resiliencePipeline));");
        if (tenantAware)
        {
            content.AppendLine("        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));");
        }
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<{entityName}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogDebug(\"Retrieving {entityName} with ID: {{Id}}\", id);");
        content.AppendLine();
        content.AppendLine($"            var entity = await _context.Set<{entityName}>()");
        if (tenantAware)
        {
            content.AppendLine("                .Where(e => e.Id == id && e.TenantId == _tenantContext.TenantId)");
        }
        else
        {
            content.AppendLine("                .Where(e => e.Id == id)");
        }
        content.AppendLine("                .FirstOrDefaultAsync(ctx.CancellationToken);");
        content.AppendLine();
        content.AppendLine("            if (entity == null)");
        content.AppendLine("            {");
        content.AppendLine("                _logger.LogWarning(\"{entityName} with ID {{Id}} not found\", id);");
        content.AppendLine("            }");
        content.AppendLine();
        content.AppendLine("            return entity;");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<IEnumerable<{entityName}>> GetAllAsync(CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogDebug(\"Retrieving all {entityName} entities\");");
        content.AppendLine();
        content.AppendLine($"            var query = _context.Set<{entityName}>().AsQueryable();");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("            query = query.Where(e => e.TenantId == _tenantContext.TenantId);");
        }
        content.AppendLine();
        content.AppendLine("            return await query");
        content.AppendLine("                .OrderByDescending(e => e.CreatedAt)");
        content.AppendLine("                .ToListAsync(ctx.CancellationToken);");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<{entityName}> CreateAsync({entityName} entity, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogDebug(\"Creating new {entityName}\");");
        content.AppendLine();
        content.AppendLine("            // Validate entity");
        content.AppendLine("            if (entity == null)");
        content.AppendLine("            {");
        content.AppendLine("                throw new ArgumentNullException(nameof(entity));");
        content.AppendLine("            }");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("            // Ensure tenant context");
            content.AppendLine("            entity.TenantId = _tenantContext.TenantId;");
        }
        content.AppendLine();
        content.AppendLine("            // Set audit properties");
        content.AppendLine("            entity.CreatedAt = DateTimeOffset.UtcNow;");
        content.AppendLine("            entity.Id = Guid.NewGuid();");
        content.AppendLine();
        content.AppendLine($"            _context.Set<{entityName}>().Add(entity);");
        content.AppendLine("            await _context.SaveChangesAsync(ctx.CancellationToken);");
        content.AppendLine();
        content.AppendLine("            _logger.LogInformation(\"Created {entityName} with ID: {{Id}}\", entity.Id);");
        content.AppendLine("            return entity;");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<{entityName}> UpdateAsync({entityName} entity, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogDebug(\"Updating {entityName} with ID: {{Id}}\", entity?.Id);");
        content.AppendLine();
        content.AppendLine("            // Validate entity");
        content.AppendLine("            if (entity == null)");
        content.AppendLine("            {");
        content.AppendLine("                throw new ArgumentNullException(nameof(entity));");
        content.AppendLine("            }");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("            // Ensure tenant isolation");
            content.AppendLine("            if (entity.TenantId != _tenantContext.TenantId)");
            content.AppendLine("            {");
            content.AppendLine("                throw new InvalidOperationException(\"Cannot update entity from different tenant\");");
            content.AppendLine("            }");
        }
        content.AppendLine();
        content.AppendLine("            // Set audit properties");
        content.AppendLine("            entity.ModifiedAt = DateTimeOffset.UtcNow;");
        content.AppendLine();
        content.AppendLine($"            _context.Set<{entityName}>().Update(entity);");
        content.AppendLine("            await _context.SaveChangesAsync(ctx.CancellationToken);");
        content.AppendLine();
        content.AppendLine("            _logger.LogInformation(\"Updated {entityName} with ID: {{Id}}\", entity.Id);");
        content.AppendLine("            return entity;");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogDebug(\"Deleting {entityName} with ID: {{Id}}\", id);");
        content.AppendLine();
        content.AppendLine($"            var entity = await _context.Set<{entityName}>()");
        if (tenantAware)
        {
            content.AppendLine("                .Where(e => e.Id == id && e.TenantId == _tenantContext.TenantId)");
        }
        else
        {
            content.AppendLine("                .Where(e => e.Id == id)");
        }
        content.AppendLine("                .FirstOrDefaultAsync(ctx.CancellationToken);");
        content.AppendLine();
        content.AppendLine("            if (entity == null)");
        content.AppendLine("            {");
        content.AppendLine("                _logger.LogWarning(\"{entityName} with ID {{Id}} not found for deletion\", id);");
        content.AppendLine("                return;");
        content.AppendLine("            }");
        content.AppendLine();
        content.AppendLine($"            _context.Set<{entityName}>().Remove(entity);");
        content.AppendLine("            await _context.SaveChangesAsync(ctx.CancellationToken);");
        content.AppendLine();
        content.AppendLine("            _logger.LogInformation(\"Deleted {entityName} with ID: {{Id}}\", id);");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _resiliencePipeline.ExecuteAsync(async (ctx) =>");
        content.AppendLine("        {");
        content.AppendLine($"            return await _context.Set<{entityName}>()");
        if (tenantAware)
        {
            content.AppendLine("                .Where(e => e.Id == id && e.TenantId == _tenantContext.TenantId)");
        }
        else
        {
            content.AppendLine("                .Where(e => e.Id == id)");
        }
        content.AppendLine("                .AnyAsync(ctx.CancellationToken);");
        content.AppendLine("        }, cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Entity base class");
        content.AppendLine($"public class {entityName} : AuditableEntity");
        content.AppendLine("{");
        content.AppendLine("    [Key]");
        content.AppendLine("    public Guid Id { get; set; }");
        content.AppendLine();
        content.AppendLine("    // Add your entity properties here");
        content.AppendLine("    // public string Name { get; set; } = string.Empty;");
        content.AppendLine("    // public string Description { get; set; } = string.Empty;");
        content.AppendLine("}");

        var warnings = new List<string>();
        warnings.Add("Replace placeholder properties with actual entity fields");
        warnings.Add("Ensure ResiliencePipeline is properly configured in DI container");
        warnings.Add("Add appropriate indexes in DbContext for query performance");
        if (tenantAware)
        {
            warnings.Add("Implement ITenantContext interface in your domain");
            warnings.Add("Add tenant-aware query filters in DbContext");
        }
        warnings.Add("Consider adding soft delete functionality if needed");
        warnings.Add("Add unit tests for repository operations");

        return new Template
        {
            FileName = $"{implementationName}.cs",
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