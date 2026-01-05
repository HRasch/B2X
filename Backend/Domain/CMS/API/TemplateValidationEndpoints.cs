using B2Connect.CMS.Application.Pages;
using B2Connect.CMS.Core.Domain.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2Connect.CMS.API.Endpoints;

/// <summary>
/// Template validation endpoints for ADR-030 Phase 2
/// Provides REST API for validating templates via MCP server
/// </summary>
public static class TemplateValidationEndpoints
{
    public static void MapTemplateValidationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/templates")
            .WithName("TemplateValidation");

        group.MapPost("/validate", ValidateTemplateAsync)
            .WithName("ValidateTemplate")
            .WithSummary("Validate a CMS template")
            .WithDescription("Performs comprehensive validation including syntax, security, performance, and best practices checks");
    }

    private static async Task<IResult> ValidateTemplateAsync(
        HttpContext context,
        ITemplateValidationService validationService,
        ValidateTemplateRequest request,
        ILogger<object> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Template validation requested for tenant {TenantId}, key {TemplateKey}",
                request.TenantId, request.TemplateKey);

            // Extract tenant from request or context
            var tenantId = request.TenantId ?? context.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Results.BadRequest(new { error = "TenantId is required" });
            }

            // Validate based on requested scope
            var result = request.ValidationScope?.ToLower() switch
            {
                "content" => await validationService.ValidateTemplateContentAsync(
                    tenantId, request.TemplateKey, request.TemplateContent, cancellationToken),
                _ => await validationService.ValidateTemplateAsync(
                    new PageDefinition
                    {
                        TenantId = tenantId,
                        PageType = "template",
                        PagePath = $"/templates/{request.TemplateKey}",
                        PageTitle = request.TemplateKey,
                        TemplateLayout = request.TemplateContent
                    }, cancellationToken)
            };

            logger.LogInformation(
                "Template validation completed for {TemplateKey}: Status={Status}, Issues={IssueCount}",
                request.TemplateKey, result.OverallStatus, result.ValidationResults.Count);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Template validation failed for {TemplateKey}", request.TemplateKey);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}

/// <summary>
/// Request model for template validation
/// </summary>
public class ValidateTemplateRequest
{
    public string? TenantId { get; set; }
    public string TemplateKey { get; set; } = null!;
    public string TemplateContent { get; set; } = null!;
    public string? ValidationScope { get; set; } // "all", "content", "syntax", "security", "performance"
}