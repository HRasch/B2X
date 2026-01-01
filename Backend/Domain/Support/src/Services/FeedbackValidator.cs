using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace B2Connect.Domain.Support.Services;

/// <summary>
/// Service for validating feedback before processing
/// </summary>
public interface IFeedbackValidator
{
    /// <summary>
    /// Validates feedback content and returns validation result
    /// </summary>
    Task<ValidationResult> ValidateAsync(CreateFeedbackCommand command);

    /// <summary>
    /// Checks if feedback should be rejected based on content analysis
    /// </summary>
    Task<bool> ShouldRejectAsync(CreateFeedbackCommand command);
}

/// <summary>
/// Result of feedback validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public ValidationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<string> Reasons { get; set; } = Array.Empty<string>();
    public ValidationSeverity Severity { get; set; }
}

/// <summary>
/// Status of validation
/// </summary>
public enum ValidationStatus
{
    Valid,
    Warning,
    Rejected,
    RequiresReview
}

/// <summary>
/// Severity level of validation issues
/// </summary>
public enum ValidationSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Validation rules configuration
/// </summary>
public class ValidationRules
{
    public IReadOnlyList<string> BlockedKeywords { get; set; } = Array.Empty<string>();
    public int MinWordsRequired { get; set; } = 3;
    public int MaxRepeatedWords { get; set; } = 5;
    public bool RequireContextData { get; set; } = true;
    public bool CheckForSpamPatterns { get; set; } = true;
    public int MaxUrlsAllowed { get; set; } = 2;
    public int MinTimeBetweenRequests { get; set; } = 30; // seconds
}

/// <summary>
/// Implementation of feedback validator
/// </summary>
public class FeedbackValidator : IFeedbackValidator
{
    private readonly ValidationRules _rules;
    private readonly ILogger<FeedbackValidator> _logger;
    private readonly IMaliciousRequestAnalyzer _securityAnalyzer;

    public FeedbackValidator(
        IOptions<ValidationRules> rules,
        ILogger<FeedbackValidator> logger,
        IMaliciousRequestAnalyzer securityAnalyzer)
    {
        _rules = rules.Value;
        _logger = logger;
        _securityAnalyzer = securityAnalyzer ?? throw new ArgumentNullException(nameof(securityAnalyzer));
    }

    public async Task<ValidationResult> ValidateAsync(CreateFeedbackCommand command)
    {
        var reasons = new List<string>();
        var severity = ValidationSeverity.Low;
        var status = ValidationStatus.Valid;

        // Step 1: ML-based security analysis
        var securityAnalysis = await _securityAnalyzer.AnalyzeAsync(command);
        if (securityAnalysis.IsMalicious)
        {
            reasons.Add($"Security threat detected: {string.Join(", ", securityAnalysis.DetectedPatterns)}");
            severity = securityAnalysis.ThreatLevel switch
            {
                SecurityThreatLevel.Critical => ValidationSeverity.Critical,
                SecurityThreatLevel.Malicious => ValidationSeverity.High,
                SecurityThreatLevel.Suspicious => ValidationSeverity.Medium,
                _ => ValidationSeverity.Medium
            };
            status = ValidationStatus.Rejected;

            _logger.LogWarning("Security threat detected in feedback {CorrelationId}: {Patterns} (Confidence: {Confidence:P2})",
                command.CorrelationId, string.Join(", ", securityAnalysis.DetectedPatterns), securityAnalysis.ConfidenceScore);
        }

        // Step 2: Content validation (existing logic)
        if (string.IsNullOrWhiteSpace(command.Description))
        {
            reasons.Add("Description is required");
            severity = ValidationSeverity.Critical;
            status = ValidationStatus.Rejected;
        }

        // Check minimum length
        if (command.Description.Length < _rules.MinWordsRequired)
        {
            reasons.Add($"Description must be at least {_rules.MinWordsRequired} words long");
            severity = ValidationSeverity.High;
            status = ValidationStatus.Rejected;
        }

        // Check for blocked keywords
        var blockedKeywords = CheckBlockedKeywords(command.Description);
        if (blockedKeywords.Any())
        {
            reasons.Add($"Content contains blocked keywords: {string.Join(", ", blockedKeywords)}");
            severity = ValidationSeverity.Critical;
            status = ValidationStatus.Rejected;
        }

        // Check for spam patterns
        if (_rules.CheckForSpamPatterns)
        {
            var spamPatterns = CheckSpamPatterns(command.Description);
            if (spamPatterns.Any())
            {
                reasons.Add($"Content contains spam patterns: {string.Join(", ", spamPatterns)}");
                severity = ValidationSeverity.High;
                status = ValidationStatus.Rejected;
            }
        }

        // Check for excessive repeated words
        var repeatedWords = CheckRepeatedWords(command.Description);
        if (repeatedWords.Any())
        {
            reasons.Add($"Content contains excessive repetition: {string.Join(", ", repeatedWords)}");
            severity = ValidationSeverity.Medium;
            status = ValidationStatus.Warning;
        }

        // Check URL count
        var urlCount = CountUrls(command.Description);
        if (urlCount > _rules.MaxUrlsAllowed)
        {
            reasons.Add($"Too many URLs ({urlCount}). Maximum allowed: {_rules.MaxUrlsAllowed}");
            severity = ValidationSeverity.Medium;
            status = ValidationStatus.Warning;
        }

        // Check context data
        if (_rules.RequireContextData && command.Context == null)
        {
            reasons.Add("Context data is required for validation");
            severity = ValidationSeverity.Medium;
            status = ValidationStatus.Warning;
        }

        // Check attachments
        var attachmentIssues = ValidateAttachments(command.Attachments);
        if (attachmentIssues.Any())
        {
            reasons.AddRange(attachmentIssues);
            if (attachmentIssues.Any(i => i.Contains("invalid") || i.Contains("too large")))
            {
                severity = ValidationSeverity.High;
                status = ValidationStatus.Rejected;
            }
        }

        var message = status switch
        {
            ValidationStatus.Valid => "Feedback is valid and can be processed.",
            ValidationStatus.Warning => "Feedback can be processed but has some issues.",
            ValidationStatus.Rejected => "Feedback cannot be processed due to validation errors.",
            ValidationStatus.RequiresReview => "Feedback requires manual review before processing.",
            _ => "Unknown validation status."
        };

        _logger.LogInformation("Validation completed for feedback {CorrelationId}: {Status} - {Message}",
            command.CorrelationId, status, string.Join("; ", reasons));

        return new ValidationResult
        {
            IsValid = status != ValidationStatus.Rejected,
            Status = status,
            Message = message,
            Reasons = reasons,
            Severity = severity
        };
    }

    public async Task<bool> ShouldRejectAsync(CreateFeedbackCommand command)
    {
        var result = await ValidateAsync(command);
        return result.Status == ValidationStatus.Rejected;
    }

    private IReadOnlyList<string> CheckBlockedKeywords(string text)
    {
        var found = new List<string>();
        var lowerText = text.ToLower();

        foreach (var keyword in _rules.BlockedKeywords)
        {
            if (lowerText.Contains(keyword.ToLower()))
            {
                found.Add(keyword);
            }
        }

        return found;
    }

    private IReadOnlyList<string> CheckSpamPatterns(string text)
    {
        var patterns = new List<string>();

        // Check for excessive caps
        var capsRatio = (double)text.Count(char.IsUpper) / text.Length;
        if (capsRatio > 0.7)
        {
            patterns.Add("excessive capitalization");
        }

        // Check for repeated characters
        if (Regex.IsMatch(text, @"(.)\1{4,}"))
        {
            patterns.Add("repeated characters");
        }

        // Check for excessive punctuation
        var punctuationCount = text.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        var punctuationRatio = (double)punctuationCount / text.Length;
        if (punctuationRatio > 0.3)
        {
            patterns.Add("excessive punctuation");
        }

        return patterns;
    }

    private IReadOnlyList<string> CheckRepeatedWords(string text)
    {
        var words = Regex.Split(text.ToLower(), @"\W+")
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .GroupBy(w => w)
            .Where(g => g.Count() > _rules.MaxRepeatedWords)
            .Select(g => $"{g.Key} ({g.Count()} times)")
            .ToList();

        return words;
    }

    private int CountUrls(string text)
    {
        var urlPattern = @"https?://[^\s]+";
        return Regex.Matches(text, urlPattern).Count;
    }

    private IReadOnlyList<string> ValidateAttachments(IReadOnlyList<Attachment> attachments)
    {
        var issues = new List<string>();

        if (attachments.Count > 3)
        {
            issues.Add("Too many attachments. Maximum allowed: 3");
        }

        foreach (var attachment in attachments)
        {
            if (attachment.Size > 5 * 1024 * 1024) // 5MB
            {
                issues.Add($"Attachment '{attachment.FileName}' is too large. Maximum size: 5MB");
            }

            if (!IsAllowedContentType(attachment.ContentType))
            {
                issues.Add($"Attachment '{attachment.FileName}' has invalid content type: {attachment.ContentType}");
            }
        }

        return issues;
    }

    private bool IsAllowedContentType(string contentType)
    {
        var allowedTypes = new[] {
            "image/jpeg", "image/png", "image/gif",
            "text/plain", "application/json"
        };

        return allowedTypes.Contains(contentType.ToLower());
    }
}