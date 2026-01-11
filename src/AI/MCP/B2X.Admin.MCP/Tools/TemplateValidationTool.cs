using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Template Validation Tool - Validates CMS templates for syntax, security, and best practices (ADR-030 Phase 2)
/// </summary>
public class TemplateValidationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly CmsValidationClient _cmsValidationClient;
    private readonly ILogger<TemplateValidationTool> _logger;

    public TemplateValidationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        CmsValidationClient cmsValidationClient,
        ILogger<TemplateValidationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _cmsValidationClient = cmsValidationClient;
        _logger = logger;
    }

    public async Task<TemplateValidationResult> ExecuteAsync(TemplateValidationArgs args)
    {
        _logger.LogInformation("Validating template for tenant {TenantId}, template key {TemplateKey}",
            _tenantContext.TenantId, args.TemplateKey);

        // Call the CMS domain validation service for comprehensive validation
        var result = await _cmsValidationClient.ValidateTemplateAsync(
            _tenantContext.TenantId,
            args.TemplateKey,
            args.TemplateContent);

        _logger.LogInformation(
            "Template validation completed for {TemplateKey}: Status={Status}, Issues={IssueCount}, Confidence={Confidence}",
            args.TemplateKey, result.OverallStatus, result.ValidationResults.Count, result.ConfidenceScore);

        return result;
    }

    private async Task<List<ValidationIssue>> ValidateSyntaxAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Basic syntax checks
        if (string.IsNullOrWhiteSpace(templateContent))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "error",
                Category = "syntax",
                Message = "Template content is empty",
                Suggestion = "Provide template content to validate"
            });
            return issues;
        }

        // Check for basic HTML structure
        if (!templateContent.Contains('<') || !templateContent.Contains('>'))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "syntax",
                Message = "Template does not appear to contain HTML markup",
                Suggestion = "Ensure template contains proper HTML structure"
            });
        }

        // Check for unclosed tags (basic check)
        var openTags = new Stack<string>();
        var tagRegex = new Regex(@"<(/?)(\w+)[^>]*>");
        var matches = tagRegex.Matches(templateContent);

        foreach (Match match in matches)
        {
            var isClosing = match.Groups[1].Value == "/";
            var tagName = match.Groups[2].Value.ToLower();

            if (!isClosing)
            {
                // Self-closing tags don't need to be closed
                if (!tagName.EndsWith('/') && !new[] { "br", "hr", "img", "input", "meta", "link" }.Contains(tagName))
                {
                    openTags.Push(tagName);
                }
            }
            else
            {
                if (openTags.Count == 0 || openTags.Pop() != tagName)
                {
                    issues.Add(new ValidationIssue
                    {
                        Severity = "error",
                        Category = "syntax",
                        Message = $"Mismatched closing tag: </{tagName}>",
                        Suggestion = "Check HTML tag structure and ensure proper nesting"
                    });
                }
            }
        }

        if (openTags.Count > 0)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "error",
                Category = "syntax",
                Message = $"Unclosed tags: {string.Join(", ", openTags)}",
                Suggestion = "Close all opened HTML tags"
            });
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidateSecurityAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for potentially dangerous patterns
        var dangerousPatterns = new[]
        {
            ("<script[^>]*>.*?</script>", "Inline JavaScript detected", "Move JavaScript to external files"),
            ("javascript:", "JavaScript URL scheme detected", "Avoid javascript: URLs for security"),
            ("on\\w+\\s*=", "Inline event handlers detected", "Use addEventListener instead of inline handlers"),
            ("eval\\(", "eval() function usage detected", "Avoid eval() for security reasons"),
            ("innerHTML\\s*=", "innerHTML assignment detected", "Use textContent or createElement for security"),
            ("document\\.write\\(", "document.write() usage detected", "Avoid document.write() as it can lead to XSS")
        };

        foreach (var (pattern, message, suggestion) in dangerousPatterns)
        {
            var regex = new Regex(pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            if (regex.IsMatch(templateContent))
            {
                issues.Add(new ValidationIssue
                {
                    Severity = "error",
                    Category = "security",
                    Message = message,
                    Suggestion = suggestion
                });
            }
        }

        // Check for proper escaping of user input placeholders
        if (templateContent.Contains("{{") && templateContent.Contains("}}"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "security",
                Message = "Template variables detected - ensure proper escaping",
                Suggestion = "Verify that all user input is properly escaped to prevent XSS"
            });
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidatePerformanceAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for large inline styles
        var styleRegex = new Regex(@"<style[^>]*>(.*?)</style>",
            RegexOptions.IgnoreCase |
            RegexOptions.Singleline);

        var styleMatch = styleRegex.Match(templateContent);
        if (styleMatch.Success && styleMatch.Groups[1].Value.Length > 2000)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "performance",
                Message = "Large inline CSS detected (>2KB)",
                Suggestion = "Move CSS to external stylesheet for better caching and performance"
            });
        }

        // Check for excessive DOM depth (basic heuristic)
        var nestingLevel = 0;
        var maxNesting = 0;
        for (int i = 0; i < templateContent.Length; i++)
        {
            char c = templateContent[i];
            if (c == '<' && i + 1 < templateContent.Length)
            {
                if (templateContent[i + 1] != '/')
                {
                    nestingLevel++;
                    maxNesting = Math.Max(maxNesting, nestingLevel);
                }
                else
                {
                    nestingLevel = Math.Max(0, nestingLevel - 1);
                }
            }
        }

        if (maxNesting > 10)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "performance",
                Message = $"Deep DOM nesting detected (max depth: {maxNesting})",
                Suggestion = "Consider flattening DOM structure for better rendering performance"
            });
        }

        // Check for missing alt attributes on images
        var imgRegex = new Regex(@"<img[^>]*>",
            RegexOptions.IgnoreCase);

        foreach (Match match in imgRegex.Matches(templateContent))
        {
            if (!match.Value.Contains("alt="))
            {
                issues.Add(new ValidationIssue
                {
                    Severity = "warning",
                    Category = "accessibility",
                    Message = "Image without alt attribute found",
                    Suggestion = "Add alt attribute to all images for accessibility"
                });
            }
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidateBestPracticesAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for semantic HTML usage
        var semanticTags = new[] { "header", "nav", "main", "section", "article", "aside", "footer" };
        var hasSemanticTags = semanticTags.Any(tag =>
            templateContent.Contains($"<{tag}") || templateContent.Contains($"<{tag} "));

        if (!hasSemanticTags)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "info",
                Category = "best_practice",
                Message = "No semantic HTML tags detected",
                Suggestion = "Consider using semantic HTML tags (header, nav, main, section, etc.) for better SEO and accessibility"
            });
        }

        // Check for proper doctype
        if (!templateContent.TrimStart().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "info",
                Category = "best_practice",
                Message = "DOCTYPE declaration missing",
                Suggestion = "Add <!DOCTYPE html> declaration at the beginning of the template"
            });
        }

        // Check for viewport meta tag
        if (!templateContent.Contains("<meta name=\"viewport\"", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "best_practice",
                Message = "Viewport meta tag missing",
                Suggestion = "Add <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> for mobile responsiveness"
            });
        }

        return issues;
    }

    private async Task<AiValidationResult> PerformAiAnalysisAsync(TemplateValidationArgs args)
    {
        var prompt = await GetSystemPromptAsync("template_validation");

        var userMessage = $@"
Analyze the following CMS template for validation and improvement suggestions:

Template Key: {args.TemplateKey}
Template Content:
{args.TemplateContent}

Please provide:
1. Overall template quality assessment
2. Specific improvement suggestions
3. Code quality and maintainability analysis
4. User experience considerations
5. SEO and accessibility recommendations

Format your response as a structured analysis with confidence scores for each suggestion.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "template_validation",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        // Parse AI response and extract structured data
        // For now, return a basic structure - in production this would parse the AI response
        return new AiValidationResult
        {
            Issues = new List<ValidationIssue>(),
            Suggestions = aiResponse.Response?.Content ?? "AI analysis completed",
            ConfidenceScore = 0.85
        };
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        // TODO: Integrate with ITemplateValidationService for comprehensive validation
        // This will replace the basic validation with full CMS domain validation
        // including syntax, security, performance, and best practices checks

        await Task.CompletedTask; // Placeholder for async database/config lookup

        return @"
You are an expert CMS template validator and frontend development consultant. Analyze templates for quality, security, performance, and best practices.

Guidelines:
- Focus on production-ready template validation
- Prioritize security and performance concerns
- Provide actionable, specific recommendations
- Consider both technical and user experience aspects
- Rate suggestions with confidence levels
- Follow modern web development standards
- Ensure accessibility and SEO best practices

Provide detailed analysis with prioritized improvement suggestions and implementation guidance.
";
    }
}