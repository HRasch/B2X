using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using B2X.CMS.Core.Domain.Pages;
using B2X.Shared.Tenancy.Infrastructure.Context;
using Microsoft.Extensions.Logging;

namespace B2X.CMS.Application.Pages;

/// <summary>
/// Default implementation of template validation service (ADR-030)
/// Performs syntax, security, performance, and best practices checks
/// </summary>
public class DefaultTemplateValidationService : ITemplateValidationService
{
    private readonly TenantContext _tenantContext;
    private readonly ILogger<DefaultTemplateValidationService> _logger;

    // Security patterns to detect
    private static readonly string[] _xssPatterns = new[]
    {
        @"<script[^>]*>[\s\S]{0,1000}?</script>",
        @"on\w+\s*=",
        @"javascript:",
        @"eval\s*\(",
        @"innerHTML\s*="
    };

    private static readonly string[] _sqlPatterns = new[]
    {
        @"('\s*OR\s*'1'\s*=\s*'1')",
        @"(DROP\s+TABLE)",
        @"(INSERT\s+INTO)",
        @"(DELETE\s+FROM)",
        @"(UPDATE\s+[A-Z_][A-Z0-9_]*\s+SET)"
    };

    public DefaultTemplateValidationService(
        TenantContext tenantContext,
        ILogger<DefaultTemplateValidationService> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<TemplateValidationResult> ValidateTemplateAsync(
        PageDefinition template,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var issues = new List<ValidationIssue>();

        _logger.LogInformation("Starting template validation for {TemplateKey}",
            template.BaseTemplateKey ?? template.PageType);

        // Run all validation checks
        var syntaxIssues = await ValidateSyntaxAsync(template.TemplateLayout).ConfigureAwait(false);
        issues.AddRange(syntaxIssues);

        var securityIssues = await ValidateSecurityAsync(template.TemplateLayout).ConfigureAwait(false);
        issues.AddRange(securityIssues);

        var performanceIssues = await ValidatePerformanceAsync(template.TemplateLayout).ConfigureAwait(false);
        issues.AddRange(performanceIssues);

        var a11yIssues = await ValidateAccessibilityAsync(template.TemplateLayout).ConfigureAwait(false);
        issues.AddRange(a11yIssues);

        // Determine overall status
        var status = DetermineValidationStatus(issues);
        var executionTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

        _logger.LogInformation(
            "Template validation completed: Status={Status}, IssueCount={IssueCount}, Time={TimeMs}ms",
            status, issues.Count, executionTime);

        return new TemplateValidationResult
        {
            OverallStatus = status,
            ValidationResults = issues,
            ExecutionTimeMs = (long)executionTime
        };
    }

    public async Task<TemplateValidationResult> ValidateTemplateContentAsync(
        string tenantId,
        string templateKey,
        string content,
        CancellationToken cancellationToken = default)
    {
        var template = new PageDefinition
        {
            TenantId = tenantId,
            BaseTemplateKey = templateKey,
            TemplateLayout = content
        };

        return await ValidateTemplateAsync(template, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<AiSuggestion>> GetSuggestionsAsync(
        PageDefinition template,
        CancellationToken cancellationToken = default)
    {
        var suggestions = new List<AiSuggestion>();

        // Performance optimization suggestions
        if (!template.TemplateLayout.Contains("loading=\"lazy\""))
        {
            suggestions.Add(new AiSuggestion
            {
                Category = "Performance",
                Text = "Add lazy loading to images to improve page load performance",
                Confidence = 0.85m,
                Priority = 2,
                Reasoning = "Lazy loading images below the fold improves initial page load time and reduces bandwidth usage"
            });
        }

        // Accessibility suggestions
        if (!template.TemplateLayout.Contains("alt="))
        {
            suggestions.Add(new AiSuggestion
            {
                Category = "Accessibility",
                Text = "All images should have descriptive alt text for screen readers",
                Confidence = 0.95m,
                Priority = 1,
                Reasoning = "Alt text is required for WCAG 2.1 AA compliance and improves SEO"
            });
        }

        // Mobile responsiveness suggestions
        if (!template.TemplateLayout.Contains("viewport"))
        {
            suggestions.Add(new AiSuggestion
            {
                Category = "ResponsiveDesign",
                Text = "Add viewport meta tag to ensure proper mobile rendering",
                Confidence = 0.90m,
                Priority = 1,
                Reasoning = "Viewport meta tag is essential for responsive design on mobile devices"
            });
        }

        // SEO suggestions
        if (template.TemplateLayout.Length > 5000)
        {
            suggestions.Add(new AiSuggestion
            {
                Category = "SEO",
                Text = "Consider splitting this large template into smaller, focused components for better SEO",
                Confidence = 0.65m,
                Priority = 3,
                Reasoning = "Smaller, focused pages typically rank better in search results"
            });
        }

        return await Task.FromResult(suggestions).ConfigureAwait(false);
    }

    public async Task RecordSuggestionFeedbackAsync(
        Guid suggestionId,
        bool accepted,
        CancellationToken cancellationToken = default)
    {
        // In a real implementation, store feedback for model training
        _logger.LogInformation(
            "Recorded suggestion feedback: SuggestionId={SuggestionId}, Accepted={Accepted}",
            suggestionId, accepted);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task<List<ValidationIssue>> ValidateSyntaxAsync(
        string content)
    {
        var issues = new List<ValidationIssue>();

        // Basic HTML structure validation
        var openTags = new Stack<string>();
        var tagPattern = new Regex(@"<(/?)(\w+)[^>]*>");

        foreach (Match match in tagPattern.Matches(content))
        {
            var isClosing = !string.IsNullOrEmpty(match.Groups[1].Value);
            var tagName = match.Groups[2].Value.ToLowerInvariant();

            if (isClosing)
            {
                if (openTags.Count == 0 || !string.Equals(openTags.Peek(), tagName))
                {
                    issues.Add(new ValidationIssue
                    {
                        Type = ValidationType.Syntax,
                        Severity = IssueSeverity.Error,
                        Message = $"Mismatched closing tag: {tagName}",
                        LineNumber = GetLineNumber(content, match.Index)
                    });
                }
                else
                {
                    openTags.Pop();
                }
            }
            else if (!IsSelfClosingTag(tagName))
            {
                openTags.Push(tagName);
            }
        }

        // Check for unclosed tags
        foreach (var unclosedTag in openTags)
        {
            issues.Add(new ValidationIssue
            {
                Type = ValidationType.Syntax,
                Severity = IssueSeverity.Warning,
                Message = $"Unclosed tag: {unclosedTag}"
            });
        }

        return await Task.FromResult(issues).ConfigureAwait(false);
    }

    private async Task<List<ValidationIssue>> ValidateSecurityAsync(
        string content)
    {
        var issues = new List<ValidationIssue>();

        // Check XSS patterns
        foreach (var pattern in _xssPatterns)
        {
            var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                issues.Add(new ValidationIssue
                {
                    Type = ValidationType.Security,
                    Severity = IssueSeverity.Critical,
                    Message = $"Potential XSS vulnerability detected: {match.Value}",
                    LineNumber = GetLineNumber(content, match.Index),
                    Suggestion = "Remove or sanitize user-generated content"
                });
            }
        }

        // Check SQL patterns
        foreach (var pattern in _sqlPatterns)
        {
            var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                issues.Add(new ValidationIssue
                {
                    Type = ValidationType.Security,
                    Severity = IssueSeverity.Critical,
                    Message = $"Potential SQL injection vulnerability: {match.Value}",
                    LineNumber = GetLineNumber(content, match.Index),
                    Suggestion = "Use parameterized queries and avoid string concatenation"
                });
            }
        }

        return await Task.FromResult(issues).ConfigureAwait(false);
    }

    private async Task<List<ValidationIssue>> ValidatePerformanceAsync(
        string content)
    {
        var issues = new List<ValidationIssue>();

        // Check for unoptimized images
        var imgPattern = new Regex(@"<img[^>]*src=[""']([^""']*)[""'][^>]*>");
        var imgMatches = imgPattern.Matches(content);

        if (imgMatches.Count > 10)
        {
            issues.Add(new ValidationIssue
            {
                Type = ValidationType.Performance,
                Severity = IssueSeverity.Warning,
                Message = $"Template contains {imgMatches.Count} images. Consider lazy loading to improve performance.",
                Suggestion = "Add loading=\"lazy\" attribute to images below the fold"
            });
        }

        // Check for inline styles
        var inlineStyleCount = Regex.Matches(content, @"style\s*=").Count;
        if (inlineStyleCount > 5)
        {
            issues.Add(new ValidationIssue
            {
                Type = ValidationType.Performance,
                Severity = IssueSeverity.Warning,
                Message = $"Found {inlineStyleCount} inline styles. Consider using external stylesheets.",
                Suggestion = "Move inline styles to external CSS files for better caching"
            });
        }

        return await Task.FromResult(issues).ConfigureAwait(false);
    }

    private async Task<List<ValidationIssue>> ValidateAccessibilityAsync(
        string content)
    {
        var issues = new List<ValidationIssue>();

        // Check for images without alt text
        var imgPattern = new Regex(@"<img[^>]*>");
        var imgMatches = imgPattern.Matches(content);

        var imagesWithoutAlt = imgMatches.Where(match => !match.Value.Contains("alt="));
        foreach (Match match in imagesWithoutAlt)
        {
            issues.Add(new ValidationIssue
            {
                Type = ValidationType.Accessibility,
                Severity = IssueSeverity.Warning,
                Message = "Image missing alt text attribute",
                LineNumber = GetLineNumber(content, match.Index),
                Suggestion = "Add descriptive alt text: alt=\"description of image\""
            });
        }

        // Check for missing language attribute
        if (!content.Contains("lang="))
        {
            issues.Add(new ValidationIssue
            {
                Type = ValidationType.Accessibility,
                Severity = IssueSeverity.Warning,
                Message = "HTML element missing language attribute",
                Suggestion = "Add lang attribute to html element: <html lang=\"en\">"
            });
        }

        // Check for proper heading hierarchy
        var headingPattern = new Regex(@"<h(\d)[^>]*>");
        var headings = new List<int>();
        foreach (Match match in headingPattern.Matches(content))
        {
            headings.Add(int.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture));
        }

        for (int i = 1; i < headings.Count; i++)
        {
            if (headings[i] > headings[i - 1] + 1)
            {
                issues.Add(new ValidationIssue
                {
                    Type = ValidationType.Accessibility,
                    Severity = IssueSeverity.Warning,
                    Message = $"Heading hierarchy skipped from h{headings[i - 1]} to h{headings[i]}",
                    Suggestion = "Use proper heading hierarchy: h1 → h2 → h3, etc."
                });
            }
        }

        return await Task.FromResult(issues).ConfigureAwait(false);
    }

    private static TemplateValidationStatus DetermineValidationStatus(List<ValidationIssue> issues)
    {
        if (!issues.Any())
            return TemplateValidationStatus.Valid;

        if (issues.Any(i => i.Severity == IssueSeverity.Critical || i.Severity == IssueSeverity.Error))
            return TemplateValidationStatus.Invalid;

        if (issues.Any(i => i.Severity == IssueSeverity.Warning))
            return TemplateValidationStatus.ValidWithWarnings;

        return TemplateValidationStatus.Valid;
    }

    private static bool IsSelfClosingTag(string tagName)
    {
        var selfClosingTags = new[] { "img", "br", "hr", "input", "meta", "link", "area", "base", "col", "embed", "source", "track", "wbr" };
        return selfClosingTags.Contains(tagName);
    }

    private static int GetLineNumber(string content, int index)
    {
        return content.Substring(0, index).Count(c => c == '\n') + 1;
    }
}
