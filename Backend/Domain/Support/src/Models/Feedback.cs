using System.ComponentModel.DataAnnotations;

namespace B2Connect.Domain.Support.Models;

/// <summary>
/// Represents a feedback item submitted by a user
/// </summary>
public class Feedback
{
    public Guid Id { get; private set; }
    public FeedbackCategory Category { get; private set; }
    public string Description { get; private set; }
    public AnonymizedContext Context { get; private set; }
    public IReadOnlyList<Attachment> Attachments { get; private set; }
    public Guid CorrelationId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public FeedbackStatus Status { get; private set; }
    public string? GitHubIssueUrl { get; private set; }

    private Feedback() { } // EF Core constructor

    public Feedback(
        FeedbackCategory category,
        string description,
        AnonymizedContext context,
        IReadOnlyList<Attachment> attachments,
        Guid correlationId)
    {
        Id = Guid.NewGuid();
        Category = category;
        Description = description;
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Attachments = attachments ?? Array.Empty<Attachment>();
        CorrelationId = correlationId;
        CreatedAt = DateTime.UtcNow;
        Status = FeedbackStatus.Created;
    }

    public void MarkAsSubmitted(string gitHubIssueUrl)
    {
        Status = FeedbackStatus.Submitted;
        GitHubIssueUrl = gitHubIssueUrl;
    }

    public void MarkAsResolved()
    {
        Status = FeedbackStatus.Resolved;
    }
}

/// <summary>
/// Category of feedback
/// </summary>
public enum FeedbackCategory
{
    Question,
    Bug,
    Enhancement,
    Other
}

/// <summary>
/// Status of feedback processing
/// </summary>
public enum FeedbackStatus
{
    Created,
    Submitted,
    InProgress,
    Resolved,
    Closed
}

/// <summary>
/// Anonymized context information collected from the user's session
/// </summary>
public class AnonymizedContext
{
    public Guid Id { get; private set; }
    public BrowserInfo Browser { get; private set; }
    public ApplicationInfo Application { get; private set; }
    public SessionInfo Session { get; private set; }
    public PerformanceInfo Performance { get; private set; }
    public UrlInfo Url { get; private set; }
    public IReadOnlyList<ErrorInfo> Errors { get; private set; }
    public string CorrelationHash { get; private set; }
    public DateTime CollectedAt { get; private set; }

    private AnonymizedContext() { } // EF Core constructor

    public AnonymizedContext(
        BrowserInfo browser,
        ApplicationInfo application,
        SessionInfo session,
        PerformanceInfo performance,
        UrlInfo url,
        IReadOnlyList<ErrorInfo> errors,
        string correlationHash)
    {
        Id = Guid.NewGuid();
        Browser = browser ?? throw new ArgumentNullException(nameof(browser));
        Application = application ?? throw new ArgumentNullException(nameof(application));
        Session = session ?? throw new ArgumentNullException(nameof(session));
        Performance = performance ?? throw new ArgumentNullException(nameof(performance));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Errors = errors ?? Array.Empty<ErrorInfo>();
        CorrelationHash = correlationHash ?? throw new ArgumentNullException(nameof(correlationHash));
        CollectedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Browser-related information
/// </summary>
public class BrowserInfo
{
    public string UserAgent { get; private set; }
    public string Language { get; private set; }
    public string Platform { get; private set; }
    public string ScreenResolution { get; private set; }
    public string ViewportSize { get; private set; }
    public string Timezone { get; private set; }

    private BrowserInfo() { }

    public BrowserInfo(
        string userAgent,
        string language,
        string platform,
        string screenResolution,
        string viewportSize,
        string timezone)
    {
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        Language = language ?? throw new ArgumentNullException(nameof(language));
        Platform = platform ?? throw new ArgumentNullException(nameof(platform));
        ScreenResolution = screenResolution ?? throw new ArgumentNullException(nameof(screenResolution));
        ViewportSize = viewportSize ?? throw new ArgumentNullException(nameof(viewportSize));
        Timezone = timezone ?? throw new ArgumentNullException(nameof(timezone));
    }
}

/// <summary>
/// Application-related information
/// </summary>
public class ApplicationInfo
{
    public string Version { get; private set; }
    public string Environment { get; private set; }
    public string BuildNumber { get; private set; }
    public string? UserRole { get; private set; } // Admin only
    public string TenantId { get; private set; } // Anonymized

    private ApplicationInfo() { }

    public ApplicationInfo(
        string version,
        string environment,
        string buildNumber,
        string? userRole,
        string tenantId)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        BuildNumber = buildNumber ?? throw new ArgumentNullException(nameof(buildNumber));
        UserRole = userRole;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
    }
}

/// <summary>
/// Session-related information
/// </summary>
public class SessionInfo
{
    public DateTime StartTime { get; private set; }
    public int Duration { get; private set; } // in milliseconds
    public int PageViews { get; private set; }
    public string LastAction { get; private set; }

    private SessionInfo() { }

    public SessionInfo(DateTime startTime, int duration, int pageViews, string lastAction)
    {
        StartTime = startTime;
        Duration = duration;
        PageViews = pageViews;
        LastAction = lastAction ?? throw new ArgumentNullException(nameof(lastAction));
    }
}

/// <summary>
/// Performance-related information
/// </summary>
public class PerformanceInfo
{
    public int LoadTime { get; private set; } // in milliseconds
    public int DomReady { get; private set; } // in milliseconds
    public int FirstPaint { get; private set; } // in milliseconds
    public int LargestContentfulPaint { get; private set; } // in milliseconds

    private PerformanceInfo() { }

    public PerformanceInfo(int loadTime, int domReady, int firstPaint, int largestContentfulPaint)
    {
        LoadTime = loadTime;
        DomReady = domReady;
        FirstPaint = firstPaint;
        LargestContentfulPaint = largestContentfulPaint;
    }
}

/// <summary>
/// URL-related information (sanitized)
/// </summary>
public class UrlInfo
{
    public string Current { get; private set; }
    public string Referrer { get; private set; }

    private UrlInfo() { }

    public UrlInfo(string current, string referrer)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Referrer = referrer ?? throw new ArgumentNullException(nameof(referrer));
    }
}

/// <summary>
/// Error information (sanitized)
/// </summary>
public class ErrorInfo
{
    public string Message { get; private set; }
    public string Stack { get; private set; } // Sanitized
    public DateTime Timestamp { get; private set; }

    private ErrorInfo() { }

    public ErrorInfo(string message, string stack, DateTime timestamp)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Stack = stack ?? throw new ArgumentNullException(nameof(stack));
        Timestamp = timestamp;
    }
}

/// <summary>
/// File attachment
/// </summary>
public class Attachment
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public byte[] Content { get; private set; }
    public long Size { get; private set; }

    private Attachment() { }

    public Attachment(string fileName, string contentType, byte[] content)
    {
        Id = Guid.NewGuid();
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Size = content.Length;
    }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Models/Feedback.cs