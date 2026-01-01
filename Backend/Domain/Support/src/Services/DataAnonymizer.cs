using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.Support.Services;

/// <summary>
/// Service for anonymizing user context data to ensure GDPR compliance
/// </summary>
public interface IDataAnonymizer
{
    /// <summary>
    /// Anonymizes collected context data by removing or masking PII
    /// </summary>
    Task<AnonymizedContext> AnonymizeAsync(CollectedContext context);
}

/// <summary>
/// Raw context data collected from frontend (before anonymization)
/// </summary>
public class CollectedContext
{
    public BrowserContext Browser { get; set; }
    public ApplicationContext Application { get; set; }
    public SessionContext Session { get; set; }
    public PerformanceContext Performance { get; set; }
    public UrlContext Url { get; set; }
    public List<ErrorContext> Errors { get; set; } = new();
}

/// <summary>
/// Browser context data
/// </summary>
public class BrowserContext
{
    public string UserAgent { get; set; }
    public string Language { get; set; }
    public string Platform { get; set; }
    public string ScreenResolution { get; set; }
    public string ViewportSize { get; set; }
    public string Timezone { get; set; }
}

/// <summary>
/// Application context data
/// </summary>
public class ApplicationContext
{
    public string Version { get; set; }
    public string Environment { get; set; }
    public string BuildNumber { get; set; }
    public string UserRole { get; set; } // Admin only
    public string TenantId { get; set; }
}

/// <summary>
/// Session context data
/// </summary>
public class SessionContext
{
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
    public int PageViews { get; set; }
    public string LastAction { get; set; }
}

/// <summary>
/// Performance context data
/// </summary>
public class PerformanceContext
{
    public int LoadTime { get; set; }
    public int DomReady { get; set; }
    public int FirstPaint { get; set; }
    public int LargestContentfulPaint { get; set; }
}

/// <summary>
/// URL context data
/// </summary>
public class UrlContext
{
    public string Current { get; set; }
    public string Referrer { get; set; }
}

/// <summary>
/// Error context data
/// </summary>
public class ErrorContext
{
    public string Message { get; set; }
    public string Stack { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Implementation of data anonymization service
/// </summary>
public class DataAnonymizer : IDataAnonymizer
{
    private readonly ILogger<DataAnonymizer> _logger;

    public DataAnonymizer(ILogger<DataAnonymizer> logger)
    {
        _logger = logger;
    }

    public async Task<AnonymizedContext> AnonymizeAsync(CollectedContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        try
        {
            var browserInfo = AnonymizeBrowser(context.Browser);
            var applicationInfo = AnonymizeApplication(context.Application);
            var sessionInfo = AnonymizeSession(context.Session);
            var performanceInfo = AnonymizePerformance(context.Performance);
            var urlInfo = AnonymizeUrl(context.Url);
            var errors = AnonymizeErrors(context.Errors);
            var correlationHash = GenerateCorrelationHash(context);

            var anonymizedContext = new AnonymizedContext(
                browserInfo,
                applicationInfo,
                sessionInfo,
                performanceInfo,
                urlInfo,
                errors,
                correlationHash
            );

            _logger.LogInformation("Successfully anonymized context data for correlation {CorrelationHash}",
                correlationHash);

            return anonymizedContext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to anonymize context data");
            throw new AnonymizationException("Failed to anonymize context data", ex);
        }
    }

    private BrowserInfo AnonymizeBrowser(BrowserContext browser)
    {
        if (browser == null) return null;

        return new BrowserInfo(
            userAgent: MaskUserAgent(browser.UserAgent),
            language: browser.Language, // Not PII
            platform: browser.Platform, // Not PII
            screenResolution: browser.ScreenResolution, // Not PII
            viewportSize: browser.ViewportSize, // Not PII
            timezone: browser.Timezone // Not PII
        );
    }

    private ApplicationInfo AnonymizeApplication(ApplicationContext application)
    {
        if (application == null) return null;

        return new ApplicationInfo(
            version: application.Version, // Not PII
            environment: application.Environment, // Not PII
            buildNumber: application.BuildNumber, // Not PII
            userRole: application.UserRole, // Admin role is not PII
            tenantId: HashTenantId(application.TenantId) // Hash for correlation without identification
        );
    }

    private SessionInfo AnonymizeSession(SessionContext session)
    {
        if (session == null) return null;

        return new SessionInfo(
            startTime: session.StartTime, // Not PII
            duration: session.Duration, // Not PII
            pageViews: session.PageViews, // Not PII
            lastAction: SanitizeAction(session.LastAction) // Remove potential PII
        );
    }

    private PerformanceInfo AnonymizePerformance(PerformanceContext performance)
    {
        if (performance == null) return null;

        // Performance data is not PII
        return new PerformanceInfo(
            loadTime: performance.LoadTime,
            domReady: performance.DomReady,
            firstPaint: performance.FirstPaint,
            largestContentfulPaint: performance.LargestContentfulPaint
        );
    }

    private UrlInfo AnonymizeUrl(UrlContext url)
    {
        if (url == null) return null;

        return new UrlInfo(
            current: SanitizeUrl(url.Current),
            referrer: SanitizeUrl(url.Referrer)
        );
    }

    private IReadOnlyList<ErrorInfo> AnonymizeErrors(List<ErrorContext> errors)
    {
        if (errors == null || !errors.Any()) return Array.Empty<ErrorInfo>();

        return errors.Select(error => new ErrorInfo(
            message: SanitizeErrorMessage(error.Message),
            stack: SanitizeStackTrace(error.Stack),
            timestamp: error.Timestamp
        )).ToList();
    }

    private string MaskUserAgent(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return userAgent;

        // Remove potential IP addresses
        var masked = Regex.Replace(userAgent, @"(\d{1,3}\.){3}\d{1,3}", "[IP_MASKED]");

        // Remove potential device IDs or other identifiers
        masked = Regex.Replace(masked, @"[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}", "[UUID_MASKED]");

        return masked;
    }

    private string HashTenantId(string tenantId)
    {
        if (string.IsNullOrEmpty(tenantId)) return tenantId;

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tenantId));
        return Convert.ToBase64String(hash).Substring(0, 16); // Short hash for correlation
    }

    private string SanitizeAction(string action)
    {
        if (string.IsNullOrEmpty(action)) return action;

        // Remove potential query parameters that might contain PII
        return Regex.Replace(action, @"\?.*", "?[PARAMS_MASKED]");
    }

    private string SanitizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return url;

        // Remove query parameters and fragments that might contain PII
        var sanitized = Regex.Replace(url, @"\?.*", "?[PARAMS_MASKED]");
        sanitized = Regex.Replace(sanitized, @"#.*", "#[FRAGMENT_MASKED]");

        return sanitized;
    }

    private string SanitizeErrorMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return message;

        // Remove potential file paths that might reveal system information
        var sanitized = Regex.Replace(message, @"[A-Za-z]:\\[^\""]*", "[PATH_MASKED]");
        sanitized = Regex.Replace(sanitized, @"/[^\""]*", "[PATH_MASKED]");

        return sanitized;
    }

    private string SanitizeStackTrace(string stack)
    {
        if (string.IsNullOrEmpty(stack)) return stack;

        // Remove file paths and line numbers that might reveal system structure
        var sanitized = Regex.Replace(stack, @"[A-Za-z]:\\[^\""]*", "[PATH_MASKED]");
        sanitized = Regex.Replace(sanitized, @"/[^\""]*", "[PATH_MASKED]");
        sanitized = Regex.Replace(sanitized, @"line \d+", "line [LINE_MASKED]");

        return sanitized;
    }

    private string GenerateCorrelationHash(CollectedContext context)
    {
        // Create a hash based on multiple context factors for correlation
        // This allows tracking related feedback without storing PII
        var hashInput = $"{context.Application?.TenantId ?? "unknown"}:{DateTime.UtcNow:yyyyMMddHHmmss}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// Exception thrown when anonymization fails
/// </summary>
public class AnonymizationException : Exception
{
    public AnonymizationException(string message, Exception innerException)
        : base(message, innerException) { }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Services/DataAnonymizer.cs