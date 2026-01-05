using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// Configuration for data sanitization rules
/// </summary>
public class DataSanitizationOptions
{
    /// <summary>
    /// Enable/disable data sanitization
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Patterns to detect and mask sensitive data
    /// </summary>
    public Dictionary<string, string> SensitiveDataPatterns { get; set; } = new()
    {
        // Email addresses
        { "email", @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}" },

        // Phone numbers (various formats)
        { "phone", @"(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}" },

        // Credit card numbers
        { "credit_card", @"\b\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}\b" },

        // Social Security Numbers (US)
        { "ssn", @"\b\d{3}[-]?\d{2}[-]?\d{4}\b" },

        // IP addresses
        { "ip_address", @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b" },

        // API keys and tokens (generic patterns)
        { "api_key", @"\b[A-Za-z0-9]{32,}\b" },
        { "bearer_token", @"Bearer\s+[A-Za-z0-9\-_\.]{20,}" },

        // Database connection strings
        { "connection_string", @"(Server|Host|Data Source)=[^;]+;(User|User Id)=[^;]+;Password=[^;]+" },

        // URLs with sensitive parameters
        { "sensitive_url", @"https?://[^\s]*?(password|token|key|secret)=[^&\s]+" }
    };

    /// <summary>
    /// Maximum content length to process (to prevent performance issues)
    /// </summary>
    public int MaxContentLength { get; set; } = 100000; // 100KB

    /// <summary>
    /// Mask to replace sensitive data with
    /// </summary>
    public string MaskReplacement { get; set; } = "[REDACTED]";

    /// <summary>
    /// Tenant-specific overrides for sensitive data patterns
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> TenantOverrides { get; set; } = new();
}

/// <summary>
/// Service for sanitizing data before sending to external AI providers
/// Prevents sensitive information leakage according to OWASP guidelines
/// </summary>
public class DataSanitizationService
{
    private readonly ILogger<DataSanitizationService> _logger;
    private readonly DataSanitizationOptions _options;
    private readonly Dictionary<string, Regex> _compiledPatterns;

    public DataSanitizationService(
        ILogger<DataSanitizationService> logger,
        IOptions<DataSanitizationOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _compiledPatterns = CompilePatterns();
    }

    /// <summary>
    /// Sanitizes content by detecting and masking sensitive data
    /// </summary>
    /// <param name="content">The content to sanitize</param>
    /// <param name="tenantId">Tenant ID for tenant-specific rules</param>
    /// <returns>Sanitized content and detection summary</returns>
    public SanitizationResult SanitizeContent(string content, string tenantId)
    {
        if (!_options.Enabled)
        {
            return new SanitizationResult
            {
                SanitizedContent = content,
                IsModified = false,
                DetectedPatterns = new List<string>(),
                OriginalLength = content.Length
            };
        }

        // Check content length limit
        if (content.Length > _options.MaxContentLength)
        {
            _logger.LogWarning("Content length {Length} exceeds maximum {MaxLength} for tenant {TenantId}",
                content.Length, _options.MaxContentLength, tenantId);

            return new SanitizationResult
            {
                SanitizedContent = string.Concat(content.AsSpan(0, _options.MaxContentLength), "...[TRUNCATED]"),
                IsModified = true,
                DetectedPatterns = new List<string> { "content_too_long" },
                OriginalLength = content.Length
            };
        }

        var sanitizedContent = content;
        var detectedPatterns = new List<string>();
        var isModified = false;

        // Get tenant-specific patterns
        var tenantPatterns = GetTenantPatterns(tenantId);

        // Apply sanitization patterns
        foreach (var (patternName, regex) in tenantPatterns)
        {
            var matches = regex.Matches(sanitizedContent);
            if (matches.Count > 0)
            {
                detectedPatterns.Add($"{patternName}({matches.Count})");
                sanitizedContent = regex.Replace(sanitizedContent, _options.MaskReplacement);
                isModified = true;

                _logger.LogInformation("Detected and masked {Count} instances of {Pattern} for tenant {TenantId}",
                    matches.Count, patternName, tenantId);
            }
        }

        // Additional custom sanitization rules
        (sanitizedContent, var customDetections) = ApplyCustomRules(sanitizedContent, tenantId);
        if (customDetections.Any())
        {
            detectedPatterns.AddRange(customDetections);
            isModified = true;
        }

        return new SanitizationResult
        {
            SanitizedContent = sanitizedContent,
            IsModified = isModified,
            DetectedPatterns = detectedPatterns,
            OriginalLength = content.Length
        };
    }

    /// <summary>
    /// Validates if content is safe to send to AI providers
    /// </summary>
    /// <param name="content">Content to validate</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Validation result</returns>
    public ValidationResult ValidateContent(string content, string tenantId)
    {
        var result = SanitizeContent(content, tenantId);

        var isValid = !result.DetectedPatterns.Contains("high_risk_content") &&
                     !result.DetectedPatterns.Contains("content_too_long");

        return new ValidationResult
        {
            IsValid = isValid,
            DetectedPatterns = result.DetectedPatterns,
            RiskLevel = CalculateRiskLevel(result.DetectedPatterns),
            Recommendations = GetRecommendations(result.DetectedPatterns)
        };
    }

    private Dictionary<string, Regex> CompilePatterns()
    {
        var patterns = new Dictionary<string, Regex>();

        foreach (var (name, pattern) in _options.SensitiveDataPatterns)
        {
            try
            {
                patterns[name] = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compile regex pattern for {PatternName}: {Pattern}",
                    name, pattern);
            }
        }

        return patterns;
    }

    private Dictionary<string, Regex> GetTenantPatterns(string tenantId)
    {
        var patterns = new Dictionary<string, Regex>(_compiledPatterns);

        // Apply tenant-specific overrides
        if (_options.TenantOverrides.TryGetValue(tenantId, out var overrides))
        {
            foreach (var (name, pattern) in overrides)
            {
                try
                {
                    patterns[name] = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to compile tenant override pattern for {TenantId}.{PatternName}: {Pattern}",
                        tenantId, name, pattern);
                }
            }
        }

        return patterns;
    }

    private (string content, List<string> detections) ApplyCustomRules(string content, string tenantId)
    {
        var detections = new List<string>();

        // Custom rule: Detect potential secrets in JSON-like structures
        var jsonSecretPattern = new Regex(@"""(password|secret|key|token)"":\s*""[^""]+""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        if (jsonSecretPattern.IsMatch(content))
        {
            content = jsonSecretPattern.Replace(content, "\"$1\": \"[REDACTED]\"");
            detections.Add("json_secrets");
        }

        // Custom rule: Detect base64 encoded data that might contain sensitive info
        var base64Pattern = new Regex(@"\b[A-Za-z0-9+/]{20,}={0,2}\b", RegexOptions.Compiled);
        var base64Matches = base64Pattern.Matches(content);
        if (base64Matches.Count > 2) // More than 2 base64 strings might indicate encoded data
        {
            detections.Add("potential_encoded_data");
            // Don't mask base64 as it might be legitimate image data
        }

        // Custom rule: Detect high-risk keywords
        var highRiskKeywords = new[] { "password", "ssn", "social security", "credit card", "bank account" };
        var keywordPattern = new Regex($@"\b({string.Join("|", highRiskKeywords)})\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        if (keywordPattern.IsMatch(content))
        {
            detections.Add("high_risk_keywords");
            // Mask the keywords themselves
            content = keywordPattern.Replace(content, _options.MaskReplacement);
        }

        return (content, detections);
    }

    private RiskLevel CalculateRiskLevel(List<string> detectedPatterns)
    {
        if (detectedPatterns.Any(p => p.Contains("high_risk") || p.Contains("json_secrets")))
        {
            return RiskLevel.High;
        }

        if (detectedPatterns.Any(p => p.Contains("email") || p.Contains("phone") || p.Contains("ip_address")))
        {
            return RiskLevel.Medium;
        }

        if (detectedPatterns.Any())
        {
            return RiskLevel.Low;
        }

        return RiskLevel.None;
    }

    private List<string> GetRecommendations(List<string> detectedPatterns)
    {
        var recommendations = new List<string>();

        if (detectedPatterns.Contains("high_risk_content"))
        {
            recommendations.Add("Content contains high-risk sensitive data. Consider blocking this request.");
        }

        if (detectedPatterns.Contains("json_secrets"))
        {
            recommendations.Add("JSON structure contains potential secrets. Ensure proper data validation.");
        }

        if (detectedPatterns.Any(p => p.Contains("email") || p.Contains("phone")))
        {
            recommendations.Add("Personal identifiable information detected. Verify this is intended for AI processing.");
        }

        if (detectedPatterns.Contains("content_too_long"))
        {
            recommendations.Add("Content exceeds maximum length. Consider truncating or rejecting.");
        }

        return recommendations;
    }
}

/// <summary>
/// Result of content sanitization
/// </summary>
public class SanitizationResult
{
    /// <summary>
    /// The sanitized content
    /// </summary>
    public string SanitizedContent { get; set; } = string.Empty;

    /// <summary>
    /// Whether the content was modified
    /// </summary>
    public bool IsModified { get; set; }

    /// <summary>
    /// Patterns that were detected and masked
    /// </summary>
    public List<string> DetectedPatterns { get; set; } = new();

    /// <summary>
    /// Original content length
    /// </summary>
    public int OriginalLength { get; set; }
}

/// <summary>
/// Result of content validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the content is valid for AI processing
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Detected patterns
    /// </summary>
    public List<string> DetectedPatterns { get; set; } = new();

    /// <summary>
    /// Risk level assessment
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Recommendations for handling the content
    /// </summary>
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Risk level for content validation
/// </summary>
public enum RiskLevel
{
    None,
    Low,
    Medium,
    High
}