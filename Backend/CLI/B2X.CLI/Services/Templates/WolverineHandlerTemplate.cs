using System.Text;

namespace B2X.CLI.Services;

public class WolverineHandlerTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var className = $"{name}Handler";
        var requestName = $"{name}Request";
        var responseName = $"{name}Response";

        var content = new StringBuilder();
        content.AppendLine("using Wolverine;");
        content.AppendLine("using Wolverine.Attributes;");
        content.AppendLine("using Microsoft.Extensions.Logging;");
        content.AppendLine("using FluentValidation;");
        content.AppendLine("using System.ComponentModel.DataAnnotations;");
        content.AppendLine();
        content.AppendLine($"namespace B2X.{GetNamespaceSuffix(name)};");
        content.AppendLine();
        content.AppendLine($"// Request DTO with validation");
        content.AppendLine($"public record {requestName}");
        content.AppendLine("(");
        if (tenantAware)
        {
            content.AppendLine("    [Required] string TenantId,");
        }
        content.AppendLine("    [Required] string Data");
        content.AppendLine(");");
        content.AppendLine();
        content.AppendLine($"// Response DTO");
        content.AppendLine($"public record {responseName}");
        content.AppendLine("(");
        content.AppendLine("    bool Success,");
        content.AppendLine("    string? Message,");
        content.AppendLine("    object? Data");
        content.AppendLine(");");
        content.AppendLine();
        content.AppendLine($"// Handler with resilience patterns");
        content.AppendLine($"[WolverineHandler]");
        content.AppendLine($"public class {className}");
        content.AppendLine("{");
        content.AppendLine("    private readonly ILogger<{className}> _logger;");
        if (tenantAware)
        {
            content.AppendLine("    private readonly ITenantContext _tenantContext;");
        }
        content.AppendLine();
        content.AppendLine($"    public {className}(");
        content.AppendLine("        ILogger<{className}> logger");
        if (tenantAware)
        {
            content.AppendLine("        , ITenantContext tenantContext");
        }
        content.AppendLine("    )");
        content.AppendLine("    {");
        content.AppendLine("        _logger = logger;");
        if (tenantAware)
        {
            content.AppendLine("        _tenantContext = tenantContext;");
        }
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    [Transactional]");
        content.AppendLine($"    public async Task<{responseName}> Handle({requestName} request)");
        content.AppendLine("    {");
        content.AppendLine("        try");
        content.AppendLine("        {");
        if (tenantAware)
        {
            content.AppendLine("            // Validate tenant access");
            content.AppendLine("            if (!Guid.TryParse(request.TenantId, out var tenantGuid))");
            content.AppendLine("            {");
            content.AppendLine("                _logger.LogWarning(\"Invalid tenant ID format: {{TenantId}}\", request.TenantId);");
            content.AppendLine("                return new {responseName}(false, \"Invalid tenant ID format\", null);");
            content.AppendLine("            }");
            content.AppendLine();
            content.AppendLine("            // Set tenant context for this operation");
            content.AppendLine("            _tenantContext.SetTenantId(tenantGuid);");
            content.AppendLine();
        }
        content.AppendLine("            _logger.LogInformation(\"Processing {requestName}: {{Data}}\", request.Data);");
        content.AppendLine();
        content.AppendLine("            // TODO: Implement business logic here");
        content.AppendLine("            // Use Polly resilience pipeline for external calls");
        content.AppendLine("            // Validate input data");
        content.AppendLine("            // Perform database operations");
        content.AppendLine();
        content.AppendLine("            return new {responseName}(true, \"Operation completed successfully\", new { /* result data */ });");
        content.AppendLine("        }");
        content.AppendLine("        catch (ValidationException ex)");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogWarning(ex, \"Validation failed for {requestName}\");");
        content.AppendLine("            return new {responseName}(false, ex.Message, null);");
        content.AppendLine("        }");
        content.AppendLine("        catch (Exception ex)");
        content.AppendLine("        {");
        content.AppendLine("            _logger.LogError(ex, \"Unexpected error processing {requestName}\");");
        content.AppendLine("            return new {responseName}(false, \"An unexpected error occurred\", null);");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// FluentValidation validator");
        content.AppendLine($"public class {requestName}Validator : AbstractValidator<{requestName}>");
        content.AppendLine("{");
        content.AppendLine($"    public {requestName}Validator()");
        content.AppendLine("    {");
        if (tenantAware)
        {
            content.AppendLine("        RuleFor(x => x.TenantId)");
            content.AppendLine("            .NotEmpty().WithMessage(\"Tenant ID is required\")");
            content.AppendLine("            .Must(BeValidGuid).WithMessage(\"Tenant ID must be a valid GUID\");");
            content.AppendLine();
        }
        content.AppendLine("        RuleFor(x => x.Data)");
        content.AppendLine("            .NotEmpty().WithMessage(\"Data is required\")");
        content.AppendLine("            .MaximumLength(1000).WithMessage(\"Data must not exceed 1000 characters\");");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private bool BeValidGuid(string value)");
        content.AppendLine("    {");
        content.AppendLine("        return Guid.TryParse(value, out _);");
        content.AppendLine("    }");
        content.AppendLine("}");

        var warnings = new List<string>();
        if (tenantAware)
        {
            warnings.Add("Ensure ITenantContext is properly registered in DI container");
            warnings.Add("Verify tenant validation logic matches your domain requirements");
        }
        warnings.Add("Replace TODO comments with actual business logic");
        warnings.Add("Consider adding integration tests for this handler");

        return new Template
        {
            FileName = $"{className}.cs",
            Content = content.ToString(),
            Warnings = warnings
        };
    }

    private string GetNamespaceSuffix(string name)
    {
        // Simple heuristic - could be made more sophisticated
        if (name.Contains("Catalog") || name.Contains("Product")) return "Catalog.Handlers";
        if (name.Contains("Cms") || name.Contains("Content")) return "CMS.Handlers";
        if (name.Contains("Identity") || name.Contains("Auth")) return "Identity.Handlers";
        if (name.Contains("Search")) return "Search.Handlers";
        return "Handlers";
    }
}