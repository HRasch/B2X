using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.Support.Services;

/// <summary>
/// Service for creating GitHub issues from feedback
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Creates a GitHub issue from anonymized feedback context
    /// </summary>
    Task<GitHubIssue> CreateIssueAsync(
        AnonymizedContext context,
        FeedbackCategory category,
        string description,
        IReadOnlyList<Attachment> attachments = null);
}

/// <summary>
/// GitHub issue response model
/// </summary>
public class GitHubIssue
{
    public int Number { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string HtmlUrl { get; set; }
    public string State { get; set; }
    public string[] Labels { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// GitHub service implementation
/// </summary>
public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubService> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public GitHubService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GitHubService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure GitHub API
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {GetGitHubToken()}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "B2Connect-Support-Bot/1.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    }

    public async Task<GitHubIssue> CreateIssueAsync(
        AnonymizedContext context,
        FeedbackCategory category,
        string description,
        IReadOnlyList<Attachment> attachments = null)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));

        try
        {
            var issuePayload = new
            {
                title = GenerateIssueTitle(category, description),
                body = GenerateIssueBody(context, description, attachments),
                labels = GetLabelsForCategory(category)
            };

            var owner = _configuration["GitHub:Owner"] ?? throw new InvalidOperationException("GitHub:Owner not configured");
            var repo = _configuration["GitHub:Repo"] ?? throw new InvalidOperationException("GitHub:Repo not configured");

            var response = await _httpClient.PostAsJsonAsync(
                $"repos/{owner}/{repo}/issues",
                issuePayload,
                _jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("GitHub API error: {StatusCode} - {ErrorContent}",
                    response.StatusCode, errorContent);

                // Check for rate limiting
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden &&
                    response.Headers.Contains("X-RateLimit-Remaining") &&
                    response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault() == "0")
                {
                    throw new GitHubRateLimitException("GitHub API rate limit exceeded");
                }

                throw new GitHubApiException($"GitHub API returned {response.StatusCode}: {errorContent}");
            }

            var issue = await response.Content.ReadFromJsonAsync<GitHubIssue>(_jsonOptions);
            if (issue == null)
            {
                throw new GitHubApiException("Failed to deserialize GitHub issue response");
            }

            _logger.LogInformation("Created GitHub issue {IssueNumber} for correlation {CorrelationHash}",
                issue.Number, context.CorrelationHash);

            return issue;
        }
        catch (Exception ex) when (ex is not GitHubRateLimitException && ex is not GitHubApiException)
        {
            _logger.LogError(ex, "Failed to create GitHub issue for correlation {CorrelationHash}",
                context.CorrelationHash);
            throw new GitHubServiceException("Failed to create GitHub issue", ex);
        }
    }

    private string GenerateIssueTitle(FeedbackCategory category, string description)
    {
        var prefix = category switch
        {
            FeedbackCategory.Question => "[QUESTION]",
            FeedbackCategory.Bug => "[BUG]",
            FeedbackCategory.Enhancement => "[ENHANCEMENT]",
            _ => "[FEEDBACK]"
        };

        // Extract first meaningful sentence or truncate
        var firstSentence = description.Split('.', '!', '?').FirstOrDefault()?.Trim();
        if (string.IsNullOrEmpty(firstSentence) || firstSentence.Length > 60)
        {
            firstSentence = description.Length > 57 ? description.Substring(0, 57) + "..." : description;
        }

        return $"{prefix} {firstSentence}";
    }

    private string GenerateIssueBody(
        AnonymizedContext context,
        string description,
        IReadOnlyList<Attachment> attachments = null)
    {
        var body = new StringBuilder();

        // User feedback section
        body.AppendLine("## User Feedback");
        body.AppendLine();
        body.AppendLine(description);
        body.AppendLine();

        // Attachments section (if any)
        if (attachments != null && attachments.Any())
        {
            body.AppendLine("## Attachments");
            body.AppendLine();
            foreach (var attachment in attachments)
            {
                body.AppendLine($"- **{attachment.FileName}** ({attachment.ContentType}, {FormatFileSize(attachment.Size)})");
            }
            body.AppendLine();
        }

        // Context information section
        body.AppendLine("## Context Information");
        body.AppendLine();

        // Browser info
        body.AppendLine("### Browser");
        body.AppendLine($"- User Agent: `{context.Browser.UserAgent}`");
        body.AppendLine($"- Language: {context.Browser.Language}");
        body.AppendLine($"- Platform: {context.Browser.Platform}");
        body.AppendLine($"- Screen Resolution: {context.Browser.ScreenResolution}");
        body.AppendLine($"- Viewport: {context.Browser.ViewportSize}");
        body.AppendLine($"- Timezone: {context.Browser.Timezone}");
        body.AppendLine();

        // Application info
        body.AppendLine("### Application");
        body.AppendLine($"- Version: {context.Application.Version}");
        body.AppendLine($"- Environment: {context.Application.Environment}");
        body.AppendLine($"- Build: {context.Application.BuildNumber}");
        if (!string.IsNullOrEmpty(context.Application.UserRole))
        {
            body.AppendLine($"- User Role: {context.Application.UserRole}");
        }
        body.AppendLine();

        // Session info
        body.AppendLine("### Session");
        body.AppendLine($"- Duration: {context.Session.Duration}ms");
        body.AppendLine($"- Page Views: {context.Session.PageViews}");
        body.AppendLine($"- Last Action: {context.Session.LastAction}");
        body.AppendLine($"- Started: {context.Session.StartTime:yyyy-MM-dd HH:mm:ss} UTC");
        body.AppendLine();

        // Performance info
        body.AppendLine("### Performance");
        body.AppendLine($"- Load Time: {context.Performance.LoadTime}ms");
        body.AppendLine($"- DOM Ready: {context.Performance.DomReady}ms");
        body.AppendLine($"- First Paint: {context.Performance.FirstPaint}ms");
        body.AppendLine($"- Largest Contentful Paint: {context.Performance.LargestContentfulPaint}ms");
        body.AppendLine();

        // URL info
        body.AppendLine("### URLs");
        body.AppendLine($"- Current: {context.Url.Current}");
        body.AppendLine($"- Referrer: {context.Url.Referrer}");
        body.AppendLine();

        // Errors (if any)
        if (context.Errors.Any())
        {
            body.AppendLine("### Recent Errors");
            foreach (var error in context.Errors.Take(5)) // Limit to 5 most recent
            {
                body.AppendLine($"- **{error.Timestamp:HH:mm:ss}**: {error.Message}");
                if (!string.IsNullOrEmpty(error.Stack))
                {
                    body.AppendLine($"  ```{error.Stack}```");
                }
            }
            body.AppendLine();
        }

        // Correlation and metadata
        body.AppendLine("### Metadata");
        body.AppendLine($"- **Correlation ID:** `{context.CorrelationHash}`");
        body.AppendLine($"- **Collected At:** {context.CollectedAt:yyyy-MM-dd HH:mm:ss} UTC");
        body.AppendLine($"- **Tenant:** `{context.Application.TenantId}`");
        body.AppendLine();

        // Support instructions
        body.AppendLine("---");
        body.AppendLine();
        body.AppendLine("**Support Instructions:**");
        body.AppendLine("- Use the Correlation ID to track this feedback");
        body.AppendLine("- All user data has been anonymized for privacy compliance");
        body.AppendLine("- Contact the user through the original application if more information is needed");

        return body.ToString();
    }

    private string[] GetLabelsForCategory(FeedbackCategory category)
    {
        var baseLabels = new[] { "support", "user-feedback" };

        var categoryLabels = category switch
        {
            FeedbackCategory.Question => new[] { "question" },
            FeedbackCategory.Bug => new[] { "bug", "needs-triage" },
            FeedbackCategory.Enhancement => new[] { "enhancement", "feature-request" },
            _ => new[] { "feedback" }
        };

        return baseLabels.Concat(categoryLabels).ToArray();
    }

    private string FormatFileSize(long bytes)
    {
        const int scale = 1024;
        string[] orders = { "GB", "MB", "KB", "Bytes" };
        var maxOrder = orders.Length - 1;
        var order = 0;
        var len = bytes;
        while (len >= scale && order < maxOrder)
        {
            len /= scale;
            order++;
        }
        return $"{len:0.##} {orders[order]}";
    }

    private string GetGitHubToken()
    {
        var token = _configuration["GitHub:Token"];
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("GitHub token not configured. Set GitHub:Token in configuration.");
        }
        return token;
    }
}

/// <summary>
/// Exception thrown when GitHub API rate limit is exceeded
/// </summary>
public class GitHubRateLimitException : Exception
{
    public GitHubRateLimitException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when GitHub API returns an error
/// </summary>
public class GitHubApiException : Exception
{
    public GitHubApiException(string message) : base(message) { }
}

/// <summary>
/// General exception for GitHub service failures
/// </summary>
public class GitHubServiceException : Exception
{
    public GitHubServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Services/GitHubService.cs