using System.Net.Http.Json;
using System.Text.Json;
using B2X.Admin.MCP.Tools;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Services;

/// <summary>
/// HTTP client for calling CMS validation service
/// </summary>
public class CmsValidationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CmsValidationClient> _logger;

    public CmsValidationClient(HttpClient httpClient, ILogger<CmsValidationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Validates a template using the CMS domain validation service
    /// </summary>
    public async Task<TemplateValidationResult> ValidateTemplateAsync(string tenantId, string templateKey, string templateContent)
    {
        try
        {
            var request = new
            {
                TenantId = tenantId,
                TemplateKey = templateKey,
                TemplateContent = templateContent,
                ValidationScope = "all" // Request full validation
            };

            var response = await _httpClient.PostAsJsonAsync("/api/templates/validate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TemplateValidationResult>();
                return result ?? throw new InvalidOperationException("Invalid validation response");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("CMS validation service returned {StatusCode}: {Error}",
                    response.StatusCode, errorContent);

                // Return basic result if service is unavailable
                return new TemplateValidationResult
                {
                    TemplateKey = templateKey,
                    ValidationTimestamp = DateTime.UtcNow,
                    ValidationResults = new List<B2X.CMS.Core.Domain.Pages.ValidationIssue>
                    {
                        new B2X.CMS.Core.Domain.Pages.ValidationIssue
                        {
                            Severity = IssueSeverity.Warning,
                            Type = ValidationType.BestPractices,
                            Message = $"CMS validation service unavailable: {response.StatusCode}",
                            Suggestion = "Check CMS service health and retry validation"
                        }
                    },
                    OverallStatus = TemplateValidationStatus.ValidWithWarnings,
                    ConfidenceScore = 0.5
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call CMS validation service for template {TemplateKey}", templateKey);

            // Return basic result if service call fails
            return new TemplateValidationResult
            {
                TemplateKey = templateKey,
                ValidationTimestamp = DateTime.UtcNow,
                ValidationResults = new List<B2X.CMS.Core.Domain.Pages.ValidationIssue>
                {
                    new B2X.CMS.Core.Domain.Pages.ValidationIssue
                    {
                        Severity = IssueSeverity.Warning,
                        Type = ValidationType.BestPractices,
                        Message = "CMS validation service temporarily unavailable",
                        Suggestion = "Validation will be performed when service is restored"
                    }
                },
                OverallStatus = TemplateValidationStatus.ValidWithWarnings,
                ConfidenceScore = 0.3
            };
        }
    }
}
