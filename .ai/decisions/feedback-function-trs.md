# Fragen & Hinweise Funktion - Technical Requirements Specification (TRS)

**Owner:** @Architect
**Effective Date:** 1. Januar 2026
**Status:** Draft

## System Architecture

### High-Level Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Store/Admin   │    │   API Gateway    │    │  Support API    │
│   Frontend      │───▶│   (Wolverine)    │───▶│  (Clean Arch)   │
│                 │    │                  │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│ Context         │    │ Anonymization    │    │ GitHub Issues   │
│ Collector       │───▶│ Service          │───▶│ API             │
│ (JavaScript)    │    │ (.NET Core)      │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Component Overview

#### 1. Frontend Components (Vue.js 3)
- **FeedbackModal.vue:** Haupt-Modal für Feedback-Eingabe
- **ContextCollector.js:** Automatische Datenerfassung
- **FileUpload.vue:** Datei-Upload mit Validierung
- **ConfirmationDialog.vue:** Erfolgsbestätigung

#### 2. Backend Services (.NET 10 + Wolverine)
- **FeedbackController:** HTTP API Endpoints
- **CreateFeedbackCommand:** CQRS Command für Feedback-Erstellung
- **FeedbackHandler:** Business Logic Handler
- **AnonymizationService:** Daten-Anonymisierung
- **GitHubService:** GitHub API Integration

#### 3. Data Models
- **FeedbackRequest:** Eingabe-DTO
- **AnonymizedContext:** Anonymisierte Kontextdaten
- **GitHubIssuePayload:** GitHub API Payload

## Detailed Technical Specifications

### 1. Frontend Implementation

#### ContextCollector Composable

```typescript
// frontend/shared/composables/useContextCollector.ts
interface CollectedContext {
  browser: {
    userAgent: string;
    language: string;
    platform: string;
    screenResolution: string;
    viewportSize: string;
    timezone: string;
  };
  application: {
    version: string;
    environment: string;
    buildNumber: string;
    userRole?: string; // Admin only
    tenantId: string; // anonymized
  };
  session: {
    startTime: Date;
    duration: number;
    pageViews: number;
    lastAction: string;
  };
  performance: {
    loadTime: number;
    domReady: number;
    firstPaint: number;
    largestContentfulPaint: number;
  };
  url: {
    current: string; // sanitized
    referrer: string; // sanitized
  };
  errors: Array<{
    message: string;
    stack: string; // sanitized
    timestamp: Date;
  }>;
}

export function useContextCollector(): {
  collect(): Promise<CollectedContext>;
  collectAsync(): void; // background collection
}
```

#### FeedbackModal Component

```vue
<!-- frontend/shared/components/FeedbackModal.vue -->
<template>
  <Modal :open="isOpen" @close="close">
    <form @submit.prevent="submitFeedback">
      <select v-model="category" required>
        <option value="question">Frage</option>
        <option value="bug">Problem</option>
        <option value="enhancement">Verbesserungsvorschlag</option>
        <option value="other">Sonstiges</option>
      </select>

      <textarea
        v-model="description"
        maxlength="1000"
        placeholder="Beschreiben Sie Ihr Anliegen..."
        required
      />

      <FileUpload
        v-model="attachments"
        :max-size="5 * 1024 * 1024"
        accept="image/*,.log,.txt"
      />

      <div class="privacy-notice">
        Ihre Daten werden anonymisiert verarbeitet.
        Korrelations-ID: {{ correlationId }}
      </div>

      <button type="submit" :disabled="isSubmitting">
        {{ isSubmitting ? 'Wird gesendet...' : 'Senden' }}
      </button>
    </form>
  </Modal>
</template>
```

### 2. Backend Implementation

#### API Controller

```csharp
// backend/Gateway/Shared/src/Presentation/Controllers/FeedbackController.cs
[ApiController]
[Route("api/support/feedback")]
[Produces("application/json")]
public class FeedbackController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public FeedbackController(IMessageBus messageBus, ILogger<FeedbackController> logger)
        : base(logger) => _messageBus = messageBus;

    [HttpPost]
    [AllowAnonymous] // Public access for feedback
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
    {
        var command = new CreateFeedbackCommand
        {
            Category = request.Category,
            Description = request.Description,
            Attachments = request.Attachments,
            Context = request.Context,
            CorrelationId = Guid.NewGuid()
        };

        var result = await _messageBus.InvokeAsync<FeedbackResult>(command);

        return Ok(new {
            correlationId = result.CorrelationId,
            issueUrl = result.IssueUrl,
            message = "Vielen Dank für Ihr Feedback!"
        });
    }

    [HttpGet("{correlationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFeedbackStatus(Guid correlationId)
    {
        var query = new GetFeedbackStatusQuery { CorrelationId = correlationId };
        var result = await _messageBus.InvokeAsync<FeedbackStatusResult>(query);
        return Ok(result);
    }
}
```

#### CQRS Commands & Queries

```csharp
// backend/Gateway/Shared/src/Application/Commands/Feedback/CreateFeedbackCommand.cs
public record CreateFeedbackCommand : ICommand<FeedbackResult>
{
    public FeedbackCategory Category { get; init; }
    public string Description { get; init; }
    public IReadOnlyList<Attachment> Attachments { get; init; }
    public CollectedContext Context { get; init; }
    public Guid CorrelationId { get; init; }
}

public record GetFeedbackStatusQuery : IQuery<FeedbackStatusResult>
{
    public Guid CorrelationId { get; init; }
}
```

#### Anonymization Service

```csharp
// backend/Gateway/Shared/src/Application/Services/AnonymizationService.cs
public interface IDataAnonymizer
{
    Task<AnonymizedContext> AnonymizeAsync(CollectedContext context);
}

public class DataAnonymizer : IDataAnonymizer
{
    private readonly ILogger<DataAnonymizer> _logger;

    public DataAnonymizer(ILogger<DataAnonymizer> logger)
        => _logger = logger;

    public async Task<AnonymizedContext> AnonymizeAsync(CollectedContext context)
    {
        return new AnonymizedContext
        {
            Browser = AnonymizeBrowser(context.Browser),
            Application = AnonymizeApplication(context.Application),
            Session = AnonymizeSession(context.Session),
            Performance = context.Performance, // Performance data is not PII
            Url = AnonymizeUrl(context.Url),
            Errors = AnonymizeErrors(context.Errors),
            CorrelationHash = GenerateCorrelationHash(context)
        };
    }

    private BrowserInfo AnonymizeBrowser(BrowserInfo browser)
    {
        return new BrowserInfo
        {
            UserAgent = MaskUserAgent(browser.UserAgent),
            Language = browser.Language, // Not PII
            Platform = browser.Platform, // Not PII
            ScreenResolution = browser.ScreenResolution, // Not PII
            ViewportSize = browser.ViewportSize, // Not PII
            Timezone = browser.Timezone // Not PII
        };
    }

    private string MaskUserAgent(string userAgent)
    {
        // Remove potential PII from User-Agent string
        return Regex.Replace(userAgent, @"([0-9]{1,3}\.){3}[0-9]{1,3}", "[IP_MASKED]");
    }

    private string GenerateCorrelationHash(CollectedContext context)
    {
        var hashInput = $"{context.Application.TenantId}:{DateTime.UtcNow:yyyyMMddHHmm}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
        return Convert.ToBase64String(hash);
    }
}
```

#### GitHub Service

```csharp
// backend/Gateway/Shared/src/Infrastructure/Services/GitHubService.cs
public interface IGitHubService
{
    Task<GitHubIssue> CreateIssueAsync(AnonymizedContext context, FeedbackCategory category, string description);
}

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        // Configure GitHub API
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {_configuration["GitHub:Token"]}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "B2Connect-Support-Bot");
    }

    public async Task<GitHubIssue> CreateIssueAsync(AnonymizedContext context, FeedbackCategory category, string description)
    {
        var issuePayload = new
        {
            title = GenerateIssueTitle(category, description),
            body = GenerateIssueBody(context, description),
            labels = GetLabelsForCategory(category)
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"repos/{_configuration["GitHub:Owner"]}/{_configuration["GitHub:Repo"]}/issues",
            issuePayload);

        response.EnsureSuccessStatusCode();

        var issue = await response.Content.ReadFromJsonAsync<GitHubIssue>();
        _logger.LogInformation("Created GitHub issue {IssueNumber} for correlation {Correlation}",
            issue.Number, context.CorrelationHash);

        return issue;
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

        var truncatedDesc = description.Length > 50
            ? description.Substring(0, 47) + "..."
            : description;

        return $"{prefix} {truncatedDesc}";
    }

    private string GenerateIssueBody(AnonymizedContext context, string description)
    {
        var body = new StringBuilder();
        body.AppendLine("## User Feedback");
        body.AppendLine();
        body.AppendLine(description);
        body.AppendLine();
        body.AppendLine("## Context Information");
        body.AppendLine();
        body.AppendLine("### Browser");
        body.AppendLine($"- User Agent: {context.Browser.UserAgent}");
        body.AppendLine($"- Language: {context.Browser.Language}");
        body.AppendLine($"- Platform: {context.Browser.Platform}");
        body.AppendLine($"- Screen: {context.Browser.ScreenResolution}");
        body.AppendLine();
        body.AppendLine("### Application");
        body.AppendLine($"- Version: {context.Application.Version}");
        body.AppendLine($"- Environment: {context.Application.Environment}");
        body.AppendLine();
        body.AppendLine("### Session");
        body.AppendLine($"- Duration: {context.Session.Duration}ms");
        body.AppendLine($"- Page Views: {context.Session.PageViews}");
        body.AppendLine();
        body.AppendLine("### Performance");
        body.AppendLine($"- Load Time: {context.Performance.LoadTime}ms");
        body.AppendLine($"- DOM Ready: {context.Performance.DomReady}ms");
        body.AppendLine();
        body.AppendLine($"**Correlation ID:** {context.CorrelationHash}");

        return body.ToString();
    }

    private string[] GetLabelsForCategory(FeedbackCategory category)
    {
        return category switch
        {
            FeedbackCategory.Question => new[] { "question", "support" },
            FeedbackCategory.Bug => new[] { "bug", "support" },
            FeedbackCategory.Enhancement => new[] { "enhancement", "support" },
            _ => new[] { "feedback", "support" }
        };
    }
}
```

### 3. Data Models

#### DTOs

```csharp
// backend/Gateway/Shared/src/Application/DTOs/FeedbackDtos.cs
public enum FeedbackCategory
{
    Question,
    Bug,
    Enhancement,
    Other
}

public record CreateFeedbackRequest
{
    [Required]
    public FeedbackCategory Category { get; init; }

    [Required]
    [StringLength(1000)]
    public string Description { get; init; }

    public IReadOnlyList<AttachmentDto> Attachments { get; init; } = Array.Empty<AttachmentDto>();

    [Required]
    public CollectedContext Context { get; init; }
}

public record AttachmentDto
{
    [Required]
    public string FileName { get; init; }

    [Required]
    public string ContentType { get; init; }

    [Required]
    public byte[] Content { get; init; }

    public long Size => Content.Length;
}

public record FeedbackResult
{
    public Guid CorrelationId { get; init; }
    public string IssueUrl { get; init; }
}

public record FeedbackStatusResult
{
    public Guid CorrelationId { get; init; }
    public string Status { get; init; } // "created", "in-progress", "resolved"
    public string? Resolution { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
```

### 4. Configuration

#### appsettings.json

```json
{
  "GitHub": {
    "Token": "your-github-personal-access-token",
    "Owner": "your-org",
    "Repo": "B2Connect",
    "Labels": {
      "Support": "support",
      "Question": "question",
      "Bug": "bug",
      "Enhancement": "enhancement"
    }
  },
  "Feedback": {
    "MaxDescriptionLength": 1000,
    "MaxAttachments": 3,
    "MaxAttachmentSize": 5242880, // 5MB
    "AllowedFileTypes": ["image/jpeg", "image/png", "text/plain", "application/octet-stream"],
    "RateLimit": {
      "RequestsPerHour": 5,
      "RequestsPerDay": 10
    }
  }
}
```

### 5. Security Implementation

#### Input Validation

```csharp
// backend/Gateway/Shared/src/Presentation/Filters/FeedbackValidationFilter.cs
public class FeedbackValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("request", out var requestObj) &&
            requestObj is CreateFeedbackRequest request)
        {
            // Validate description for malicious content
            if (ContainsMaliciousContent(request.Description))
            {
                context.Result = new BadRequestObjectResult("Invalid content detected");
                return;
            }

            // Validate attachments
            if (request.Attachments.Any(a => !IsAllowedFileType(a.ContentType)))
            {
                context.Result = new BadRequestObjectResult("Invalid file type");
                return;
            }

            // Rate limiting check
            if (IsRateLimitExceeded(context.HttpContext))
            {
                context.Result = new TooManyRequestsObjectResult("Rate limit exceeded");
                return;
            }
        }
    }

    private bool ContainsMaliciousContent(string content)
    {
        // Basic XSS and injection prevention
        var maliciousPatterns = new[]
        {
            @"<script", @"javascript:", @"on\w+\s*=",
            @"<iframe", @"<object", @"<embed"
        };

        return maliciousPatterns.Any(pattern =>
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
    }

    private bool IsAllowedFileType(string contentType)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "text/plain" };
        return allowedTypes.Contains(contentType.ToLower());
    }

    private bool IsRateLimitExceeded(HttpContext context)
    {
        // Implement rate limiting logic based on IP/User
        // This is a simplified example
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        // Check against distributed cache or database
        return false; // Placeholder
    }
}
```

### 6. Testing Strategy

#### Unit Tests

```csharp
// backend/Gateway/Shared/tests/UnitTests/AnonymizationServiceTests.cs
public class AnonymizationServiceTests
{
    [Fact]
    public async Task AnonymizeAsync_ShouldMaskUserAgent()
    {
        // Arrange
        var service = new DataAnonymizer(Mock.Of<ILogger<DataAnonymizer>>());
        var context = new CollectedContext
        {
            Browser = new BrowserInfo
            {
                UserAgent = "Mozilla/5.0 (192.168.1.100) Safari/537.36"
            }
        };

        // Act
        var result = await service.AnonymizeAsync(context);

        // Assert
        Assert.DoesNotContain("192.168.1.100", result.Browser.UserAgent);
        Assert.Contains("[IP_MASKED]", result.Browser.UserAgent);
    }

    [Fact]
    public async Task AnonymizeAsync_ShouldGenerateCorrelationHash()
    {
        // Arrange
        var service = new DataAnonymizer(Mock.Of<ILogger<DataAnonymizer>>());
        var context = new CollectedContext
        {
            Application = new ApplicationInfo { TenantId = "tenant-123" }
        };

        // Act
        var result = await service.AnonymizeAsync(context);

        // Assert
        Assert.NotNull(result.CorrelationHash);
        Assert.NotEqual(string.Empty, result.CorrelationHash);
    }
}
```

#### Integration Tests

```csharp
// backend/Gateway/Shared/tests/IntegrationTests/GitHubServiceTests.cs
public class GitHubServiceIntegrationTests : IClassFixture<GitHubTestFixture>
{
    private readonly GitHubTestFixture _fixture;

    public GitHubServiceIntegrationTests(GitHubTestFixture fixture)
        => _fixture = fixture;

    [Fact]
    public async Task CreateIssueAsync_ShouldCreateGitHubIssue()
    {
        // Arrange
        var service = new GitHubService(_fixture.HttpClient, _fixture.Configuration, _fixture.Logger);
        var context = new AnonymizedContext { /* test data */ };
        var category = FeedbackCategory.Bug;
        var description = "Test issue description";

        // Act
        var issue = await service.CreateIssueAsync(context, category, description);

        // Assert
        Assert.NotNull(issue);
        Assert.True(issue.Number > 0);
        Assert.Contains("[BUG]", issue.Title);
    }
}
```

### 7. Deployment Configuration

#### Docker Configuration

```dockerfile
# backend/Gateway/Shared/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["B2Connect.Shared.csproj", "."]
RUN dotnet restore "B2Connect.Shared.csproj"
COPY . .
RUN dotnet build "B2Connect.Shared.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "B2Connect.Shared.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "B2Connect.Shared.dll"]
```

#### Kubernetes Deployment

```yaml
# k8s/support-api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: support-api
spec:
  replicas: 2
  selector:
    matchLabels:
      app: support-api
  template:
    metadata:
      labels:
        app: support-api
    spec:
      containers:
      - name: support-api
        image: b2connect/support-api:latest
        ports:
        - containerPort: 80
        env:
        - name: GitHub__Token
          valueFrom:
            secretKeyRef:
              name: github-secrets
              key: token
        - name: GitHub__Owner
          value: "your-org"
        - name: GitHub__Repo
          value: "B2Connect"
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
```

## Performance & Scalability

### Performance Targets
- **API Response Time:** <500ms (p95)
- **GitHub API Calls:** Async processing, circuit breaker pattern
- **Database Operations:** Minimal, nur für Korrelation tracking
- **File Upload:** Streaming processing für große Anhänge

### Monitoring & Observability

```csharp
// backend/Gateway/Shared/src/Infrastructure/Monitoring/FeedbackMetrics.cs
public static class FeedbackMetrics
{
    public static readonly Counter<int> FeedbackCreated = Meter.CreateCounter<int>(
        "feedback_created_total",
        description: "Total number of feedback items created");

    public static readonly Histogram<double> FeedbackProcessingDuration = Meter.CreateHistogram<double>(
        "feedback_processing_duration_seconds",
        description: "Time spent processing feedback");

    public static readonly Counter<int> GitHubApiErrors = Meter.CreateCounter<int>(
        "github_api_errors_total",
        description: "Total GitHub API errors");
}
```

## Migration & Rollout Strategy

### Phase 1: Database Migration
- Erstellung Correlation-Tracking Table (optional)
- Migration Scripts für bestehende Daten

### Phase 2: Service Deployment
- Blue-Green Deployment für Support API
- Feature Flags für graduelle Aktivierung

### Phase 3: Frontend Rollout
- Store Frontend zuerst
- Admin Frontend danach
- A/B Testing für UX Validation

### Phase 4: Monitoring & Optimization
- Performance Monitoring Setup
- Error Rate Tracking
- User Feedback Analysis

---

**Diese TRS definiert die vollständige technische Implementierung der Fragen & Hinweise-Funktion. Die Implementierung erfolgt in enger Abstimmung mit @Backend, @Frontend, @Security und @DevOps.**</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/feedback-function-trs.md