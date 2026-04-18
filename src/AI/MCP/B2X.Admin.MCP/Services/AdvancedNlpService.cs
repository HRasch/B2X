using B2X.Admin.MCP.Data;
using B2X.Admin.MCP.Middleware;
using Microsoft.EntityFrameworkCore;

namespace B2X.Admin.MCP.Services;

/// <summary>
/// Advanced NLP service for sophisticated tool routing and intent recognition
/// </summary>
public class AdvancedNlpService
{
    private readonly McpDbContext _dbContext;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<AdvancedNlpService> _logger;

    // Tool keywords and patterns for intent recognition
    private readonly Dictionary<string, ToolIntent> _toolIntents = new()
    {
        ["performance_optimization"] = new ToolIntent
        {
            Keywords = new[] { "performance", "optimize", "speed", "slow", "fast", "improve", "bottleneck", "latency" },
            Patterns = new[] { "how.*fast", "why.*slow", "optimize.*performance", "improve.*speed" },
            ContextWords = new[] { "database", "api", "frontend", "backend", "response", "time", "load" }
        },
        ["security_compliance"] = new ToolIntent
        {
            Keywords = new[] { "security", "audit", "compliance", "vulnerable", "safe", "risk", "threat" },
            Patterns = new[] { "security.*check", "audit.*security", "compliance.*check", "vulnerability.*scan" },
            ContextWords = new[] { "authentication", "authorization", "encryption", "access", "permissions" }
        },
        ["content_optimization"] = new ToolIntent
        {
            Keywords = new[] { "content", "seo", "engagement", "page", "article", "blog", "website" },
            Patterns = new[] { "optimize.*content", "improve.*seo", "content.*strategy", "page.*performance" },
            ContextWords = new[] { "keywords", "traffic", "ranking", "conversion", "user", "experience" }
        },
        ["store_operations"] = new ToolIntent
        {
            Keywords = new[] { "store", "sales", "inventory", "revenue", "product", "order", "customer" },
            Patterns = new[] { "store.*performance", "sales.*analysis", "inventory.*management", "revenue.*growth" },
            ContextWords = new[] { "ecommerce", "retail", "shopping", "cart", "checkout", "purchase" }
        },
        ["tenant_management"] = new ToolIntent
        {
            Keywords = new[] { "tenant", "resource", "onboard", "multi-tenant", "organization", "account" },
            Patterns = new[] { "tenant.*management", "resource.*allocation", "onboard.*tenant", "multi.*tenant" },
            ContextWords = new[] { "subscription", "billing", "usage", "limits", "quota", "isolation" }
        },
        ["integration_management"] = new ToolIntent
        {
            Keywords = new[] { "integration", "api", "webhook", "connect", "sync", "external" },
            Patterns = new[] { "api.*integration", "webhook.*setup", "external.*connection", "data.*sync" },
            ContextWords = new[] { "third-party", "service", "platform", "connector", "middleware" }
        },
        ["user_management_assistant"] = new ToolIntent
        {
            Keywords = new[] { "user", "admin", "role", "permission", "access", "account" },
            Patterns = new[] { "user.*management", "role.*assignment", "permission.*setup", "admin.*task" },
            ContextWords = new[] { "login", "registration", "profile", "authentication", "authorization" }
        },
        ["email_template_design"] = new ToolIntent
        {
            Keywords = new[] { "email", "template", "campaign", "newsletter", "mail", "communication" },
            Patterns = new[] { "email.*template", "design.*email", "campaign.*setup", "newsletter.*create" },
            ContextWords = new[] { "marketing", "transactional", "welcome", "promotional", "notification" }
        },
        ["cms_page_design"] = new ToolIntent
        {
            Keywords = new[] { "cms", "page", "layout", "design", "content", "website" },
            Patterns = new[] { "page.*design", "cms.*layout", "content.*structure", "website.*design" },
            ContextWords = new[] { "landing", "about", "contact", "blog", "portfolio", "responsive" }
        },
        ["system_health_analysis"] = new ToolIntent
        {
            Keywords = new[] { "health", "system", "monitor", "status", "diagnostic", "check" },
            Patterns = new[] { "system.*health", "monitor.*status", "diagnostic.*check", "health.*analysis" },
            ContextWords = new[] { "uptime", "availability", "error", "log", "metric", "alert" }
        }
    };

    public AdvancedNlpService(
        McpDbContext dbContext,
        TenantContext tenantContext,
        ILogger<AdvancedNlpService> logger)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Analyze user message and determine the most appropriate tool
    /// </summary>
    public async Task<NlpAnalysisResult> AnalyzeIntentAsync(string message, int? conversationId = null)
    {
        var normalizedMessage = NormalizeText(message);
        var scores = new Dictionary<string, double>();

        // Calculate intent scores for each tool
        foreach (var (toolName, intent) in _toolIntents)
        {
            var score = CalculateIntentScore(normalizedMessage, intent);
            scores[toolName] = score;
        }

        // Consider conversation context if available
        if (conversationId.HasValue)
        {
            await AdjustScoresByContextAsync(scores, conversationId.Value);
        }

        // Get the highest scoring tool
        var bestTool = scores.OrderByDescending(s => s.Value).First();
        var confidence = bestTool.Value;

        // Extract entities and parameters
        var entities = ExtractEntities(normalizedMessage, bestTool.Key);
        var parameters = await InferParametersAsync(normalizedMessage, bestTool.Key, conversationId);

        return new NlpAnalysisResult
        {
            ToolName = bestTool.Key,
            Confidence = confidence,
            Entities = entities,
            Parameters = parameters,
            AlternativeTools = scores
                .Where(s => s.Key != bestTool.Key)
                .OrderByDescending(s => s.Value)
                .Take(3)
                .ToDictionary(s => s.Key, s => s.Value)
        };
    }

    /// <summary>
    /// Learn from user feedback to improve intent recognition
    /// </summary>
    public async Task LearnFromFeedbackAsync(string message, string correctTool, string userId)
    {
        // Store learning data for future model training
        var learningRecord = new NlpLearningRecord
        {
            TenantId = _tenantContext.TenantId,
            UserId = userId,
            OriginalMessage = message,
            CorrectTool = correctTool,
            PredictedTool = await AnalyzeIntentAsync(message).ContinueWith(t => t.Result.ToolName),
            CreatedAt = DateTime.UtcNow
        };

        // In a real implementation, this would be stored in a learning database
        // and used to retrain the model periodically
        _logger.LogInformation("NLP Learning: Message '{Message}' should use {CorrectTool}",
            message, correctTool);
    }

    /// <summary>
    /// Get conversation context for better intent recognition
    /// </summary>
    public async Task<ConversationContext> GetConversationContextAsync(int conversationId)
    {
        var messages = await _dbContext.ConversationMessages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(5)
            .ToListAsync();

        var context = new ConversationContext
        {
            RecentTopics = ExtractTopics(messages),
            ToolUsage = messages.Where(m => !string.IsNullOrEmpty(m.ToolName))
                               .GroupBy(m => m.ToolName!)
                               .ToDictionary(g => g.Key, g => g.Count()),
            UserPreferences = ExtractUserPreferences(messages)
        };

        return context;
    }

    private string NormalizeText(string text)
    {
        return text.ToLower()
                  .Replace(".", " ")
                  .Replace(",", " ")
                  .Replace("?", " ")
                  .Replace("!", " ")
                  .Replace("  ", " ")
                  .Trim();
    }

    private double CalculateIntentScore(string message, ToolIntent intent)
    {
        double score = 0;

        // Keyword matching (40% weight)
        var keywordMatches = intent.Keywords.Count(k => message.Contains(k));
        score += (keywordMatches * 4.0) / intent.Keywords.Length;

        // Pattern matching (30% weight)
        foreach (var pattern in intent.Patterns)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(message, pattern.Replace("*", ".*")))
            {
                score += 3.0;
            }
        }

        // Context word matching (20% weight)
        var contextMatches = intent.ContextWords.Count(c => message.Contains(c));
        score += (contextMatches * 2.0) / Math.Max(intent.ContextWords.Length, 1);

        // Message length factor (10% weight) - longer messages are more specific
        var lengthFactor = Math.Min(message.Length / 100.0, 1.0);
        score += lengthFactor;

        return score;
    }

    private async Task AdjustScoresByContextAsync(Dictionary<string, double> scores, int conversationId)
    {
        var context = await GetConversationContextAsync(conversationId);

        // Boost scores for recently used tools
        foreach (var (toolName, usageCount) in context.ToolUsage)
        {
            if (scores.ContainsKey(toolName))
            {
                scores[toolName] += usageCount * 0.5; // Boost by usage frequency
            }
        }

        // Consider recent topics
        foreach (var topic in context.RecentTopics)
        {
            var relatedTools = GetToolsForTopic(topic);
            foreach (var tool in relatedTools)
            {
                if (scores.ContainsKey(tool))
                {
                    scores[tool] += 1.0; // Context boost
                }
            }
        }
    }

    private Dictionary<string, object> ExtractEntities(string message, string toolName)
    {
        var entities = new Dictionary<string, object>();

        // Extract common entities based on tool type
        switch (toolName)
        {
            case "performance_optimization":
                entities["component"] = ExtractComponent(message) ?? "unknown";
                entities["metric"] = ExtractMetric(message) ?? "unknown";
                break;

            case "store_operations":
                entities["operation"] = ExtractOperation(message) ?? "unknown";
                entities["timeframe"] = ExtractTimeframe(message) ?? "unknown";
                break;

            case "email_template_design":
                entities["email_type"] = ExtractEmailType(message) ?? "unknown";
                entities["purpose"] = ExtractPurpose(message) ?? "unknown";
                break;

            case "cms_page_design":
                entities["page_type"] = ExtractPageType(message) ?? "unknown";
                entities["audience"] = ExtractAudience(message) ?? "unknown";
                break;
        }

        return entities;
    }

    private async Task<Dictionary<string, object>> InferParametersAsync(string message, string toolName, int? conversationId)
    {
        var parameters = new Dictionary<string, object>();

        // Get conversation context if available
        ConversationContext? context = null;
        if (conversationId.HasValue)
        {
            context = await GetConversationContextAsync(conversationId.Value);
        }

        // Infer parameters based on tool and context
        switch (toolName)
        {
            case "performance_optimization":
                parameters["component"] = ExtractComponent(message) ?? context?.PreferredComponent ?? "system";
                parameters["include_historical_data"] = true;
                parameters["time_range"] = ExtractTimeRange(message) ?? "24h";
                break;

            case "security_compliance":
                parameters["assessment_type"] = "comprehensive";
                parameters["scope"] = ExtractScope(message) ?? "current";
                parameters["include_recommendations"] = true;
                break;

            case "content_optimization":
                parameters["content_type"] = "web_content";
                parameters["target_audience"] = context?.PreferredAudience ?? "general";
                break;

            case "store_operations":
                parameters["operation"] = "analyze";
                parameters["analysis_type"] = "performance";
                parameters["store_id"] = "current";
                break;
        }

        return parameters;
    }

    private List<string> ExtractTopics(List<ConversationMessage> messages)
    {
        var topics = new List<string>();
        var topicKeywords = new[] { "performance", "security", "content", "store", "tenant", "user", "email", "cms" };

        foreach (var message in messages.Where(m => m.Sender == "user"))
        {
            var words = message.Content.ToLower().Split(' ');
            topics.AddRange(topicKeywords.Where(k => words.Contains(k)));
        }

        return topics.Distinct().ToList();
    }

    private Dictionary<string, string> ExtractUserPreferences(List<ConversationMessage> messages)
    {
        var preferences = new Dictionary<string, string>();

        // Analyze successful interactions to infer preferences
        var successfulTools = messages
            .Where(m => m.Sender == "assistant" && !m.IsError && !string.IsNullOrEmpty(m.ToolName))
            .GroupBy(m => m.ToolName!)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        if (successfulTools.Any())
        {
            preferences["preferred_tool"] = successfulTools.First();
        }

        return preferences;
    }

    private List<string> GetToolsForTopic(string topic)
    {
        return topic switch
        {
            "performance" => new[] { "performance_optimization", "system_health_analysis" }.ToList(),
            "security" => new[] { "security_compliance" }.ToList(),
            "content" => new[] { "content_optimization", "cms_page_design" }.ToList(),
            "store" => new[] { "store_operations" }.ToList(),
            "user" => new[] { "user_management_assistant" }.ToList(),
            "email" => new[] { "email_template_design" }.ToList(),
            _ => new List<string>()
        };
    }

    // Entity extraction helpers
    private string? ExtractComponent(string message) =>
        message.Contains("database") || message.Contains("db") ? "database" :
        message.Contains("api") ? "api" :
        message.Contains("frontend") ? "frontend" :
        message.Contains("backend") ? "backend" : null;

    private string? ExtractMetric(string message) =>
        message.Contains("response") ? "response_time" :
        message.Contains("load") ? "load_time" :
        message.Contains("memory") ? "memory_usage" : "response_time";

    private string? ExtractOperation(string message) =>
        message.Contains("analyze") ? "analyze" :
        message.Contains("optimize") ? "optimize" :
        message.Contains("report") ? "report" : "analyze";

    private string? ExtractTimeframe(string message) =>
        message.Contains("today") ? "today" :
        message.Contains("week") ? "week" :
        message.Contains("month") ? "month" : "current";

    private string? ExtractEmailType(string message) =>
        message.Contains("welcome") ? "welcome" :
        message.Contains("marketing") ? "marketing" :
        message.Contains("newsletter") ? "newsletter" : "marketing";

    private string? ExtractPurpose(string message) =>
        message.Contains("engagement") ? "engagement" :
        message.Contains("conversion") ? "conversion" :
        message.Contains("notification") ? "notification" : "engagement";

    private string? ExtractPageType(string message) =>
        message.Contains("landing") ? "landing" :
        message.Contains("about") ? "about" :
        message.Contains("contact") ? "contact" : "content";

    private string? ExtractAudience(string message) =>
        message.Contains("business") ? "business" :
        message.Contains("consumer") ? "consumer" : "general";

    private string? ExtractTimeRange(string message) =>
        message.Contains("hour") ? "1h" :
        message.Contains("day") ? "24h" :
        message.Contains("week") ? "7d" : "24h";

    private string? ExtractScope(string message) =>
        message.Contains("all") ? "all" :
        message.Contains("current") ? "current" : "current";
}

/// <summary>
/// Tool intent definition for NLP analysis
/// </summary>
public class ToolIntent
{
    public string[] Keywords { get; set; } = Array.Empty<string>();
    public string[] Patterns { get; set; } = Array.Empty<string>();
    public string[] ContextWords { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Result of NLP intent analysis
/// </summary>
public class NlpAnalysisResult
{
    public string ToolName { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, object> Entities { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, double> AlternativeTools { get; set; } = new();
}

/// <summary>
/// Conversation context for improved intent recognition
/// </summary>
public class ConversationContext
{
    public List<string> RecentTopics { get; set; } = new();
    public Dictionary<string, int> ToolUsage { get; set; } = new();
    public Dictionary<string, string> UserPreferences { get; set; } = new();
    public string? PreferredComponent { get; set; }
    public string? PreferredAudience { get; set; }
}

/// <summary>
/// Learning record for NLP model improvement
/// </summary>
public class NlpLearningRecord
{
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string OriginalMessage { get; set; } = string.Empty;
    public string CorrectTool { get; set; } = string.Empty;
    public string PredictedTool { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
