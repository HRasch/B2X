using B2X.Admin.MCP.Controllers;
using B2X.Admin.MCP.Data;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NlpConversationContext = B2X.Admin.MCP.Services.ConversationContext;

namespace B2X.Admin.MCP.Controllers;

/// <summary>
/// API controller for conversation memory management
/// </summary>
[ApiController]
[Route("api/mcp/conversations")]
[Authorize]
public class ConversationController : ControllerBase
{
    private readonly ConversationService _conversationService;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(
        ConversationService conversationService,
        TenantContext tenantContext,
        ILogger<ConversationController> logger)
    {
        _conversationService = conversationService;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all conversations for the current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConversationDto>), 200)]
    public async Task<IActionResult> GetConversations([FromQuery] int? limit = 50)
    {
        var conversations = await _conversationService.GetConversationsAsync(limit ?? 50);
        var dtos = conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            Title = c.Title,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = c.Messages.Count,
            LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Content
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Get a specific conversation with messages
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConversationDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversation(int id)
    {
        var conversation = await _conversationService.GetConversationAsync(id);
        if (conversation == null)
            return NotFound();

        var dto = new ConversationDetailDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Sender = m.Sender,
                    ToolName = m.ToolName,
                    IsError = m.IsError,
                    CreatedAt = m.CreatedAt
                })
                .ToList(),
            Context = conversation.Context
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefault()?.Value
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), 201)]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var conversation = await _conversationService.CreateConversationAsync(userId, request.Title);
        var dto = new ConversationDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            MessageCount = 0,
            LastMessage = null
        };

        return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, dto);
    }

    /// <summary>
    /// Add a message to a conversation
    /// </summary>
    [HttpPost("{id}/messages")]
    [ProducesResponseType(typeof(MessageDto), 201)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddMessage(int id, [FromBody] AddMessageRequest request)
    {
        var message = await _conversationService.AddMessageAsync(
            id,
            request.Sender,
            request.Content,
            request.ToolName,
            isError: request.IsError);

        var dto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            Sender = message.Sender,
            ToolName = message.ToolName,
            IsError = message.IsError,
            CreatedAt = message.CreatedAt
        };

        return Created("", dto);
    }

    /// <summary>
    /// Update conversation context
    /// </summary>
    [HttpPut("{id}/context")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateContext(int id, [FromBody] UpdateContextRequest request)
    {
        var conversation = await _conversationService.GetConversationAsync(id);
        if (conversation == null)
            return NotFound();

        // Store context as JSON under a default key
        var contextJson = System.Text.Json.JsonSerializer.Serialize(request.ContextData);
        await _conversationService.SetContextAsync(id, "context", contextJson);

        return NoContent();
    }

    /// <summary>
    /// Delete a conversation
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var success = await _conversationService.DeleteConversationAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Search conversations by content
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ConversationDto>), 200)]
    public async Task<IActionResult> SearchConversations([FromQuery] string query, [FromQuery] int? limit = 20)
    {
        var conversations = await _conversationService.SearchConversationsAsync(query, limit ?? 20);
        var dtos = conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            Title = c.Title,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = c.Messages.Count,
            LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Content
        });

        return Ok(dtos);
    }
}

/// <summary>
/// API controller for A/B testing management
/// </summary>
[ApiController]
[Route("api/mcp/ab-testing")]
[Authorize]
public class AbTestingController : ControllerBase
{
    private readonly AbTestingService _abTestingService;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<AbTestingController> _logger;

    public AbTestingController(
        AbTestingService abTestingService,
        TenantContext tenantContext,
        ILogger<AbTestingController> logger)
    {
        _abTestingService = abTestingService;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all A/B tests for the current tenant
    /// </summary>
    [HttpGet("tests")]
    [ProducesResponseType(typeof(IEnumerable<AbTestDto>), 200)]
    public async Task<IActionResult> GetTests()
    {
        var tests = await _abTestingService.GetTestsAsync();
        var dtos = tests.Select(t => new AbTestDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            Variants = t.Variants.Select(v => new AbTestVariantDto
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                Weight = (double)v.Weight,
                IsControl = v.IsControl
            }).ToList(),
            Statistics = new AbTestStatisticsDto
            {
                TotalParticipants = t.Results.Count,
                ConversionRate = t.Results.Count > 0 ? t.Results.Count(r => r.MetricType.Equals("conversion")) / (double)t.Results.Count : 0,
                BestVariant = t.Variants
                    .OrderByDescending(v => v.Weight)
                    .FirstOrDefault()?.Name ?? "None"
            }
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Get a specific A/B test with detailed statistics
    /// </summary>
    [HttpGet("tests/{id}")]
    [ProducesResponseType(typeof(AbTestDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTest(int id)
    {
        var test = await _abTestingService.GetTestAsync(id);
        if (test == null)
            return NotFound();

        var statistics = await _abTestingService.GetTestStatisticsAsync(id);

        var dto = new AbTestDetailDto
        {
            Id = test.Id,
            Name = test.Name,
            Description = test.Description,
            Status = test.Status,
            CreatedAt = test.CreatedAt,
            Variants = test.Variants.Select(v => new AbTestVariantDto
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                Weight = (double)v.Weight,
                IsControl = v.IsControl
            }).ToList(),
            Results = test.Results.Select(r => new AbTestResultDto
            {
                Id = r.Id,
                UserId = r.UserId,
                VariantId = r.VariantId,
                Converted = r.MetricType.Equals("conversion"),
                ConversionValue = r.MetricValue,
                CreatedAt = r.CreatedAt
            }).ToList(),
            Statistics = MapToStatisticsDto(statistics)
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new A/B test
    /// </summary>
    [HttpPost("tests")]
    [ProducesResponseType(typeof(AbTestDto), 201)]
    public async Task<IActionResult> CreateTest([FromBody] CreateAbTestRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var test = await _abTestingService.CreateTestAsync(
            request.Name,
            request.Description,
            "default", // toolName
            userId);   // createdBy

        // Add variants to the test
        foreach (var variant in request.Variants)
        {
            await _abTestingService.AddVariantAsync(
                test.Id,
                variant.Name,
                variant.Description,
                string.Empty, // promptTemplate - simplified API
                (decimal)variant.Weight,
                variant.IsControl);
        }

        // Reload to get variants
        test = await _abTestingService.GetTestAsync(test.Id);

        var dto = new AbTestDto
        {
            Id = test!.Id,
            Name = test.Name,
            Description = test.Description,
            Status = test.Status,
            CreatedAt = test.CreatedAt,
            Variants = test.Variants.Select(v => new AbTestVariantDto
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                Weight = (double)v.Weight,
                IsControl = v.IsControl
            }).ToList(),
            Statistics = new AbTestStatisticsDto
            {
                TotalParticipants = 0,
                ConversionRate = 0,
                BestVariant = test.Variants.FirstOrDefault()?.Name ?? "None"
            }
        };

        return CreatedAtAction(nameof(GetTest), new { id = test.Id }, dto);
    }

    /// <summary>
    /// Select a variant for a user in an A/B test
    /// </summary>
    [HttpPost("tests/{testId}/select-variant")]
    [ProducesResponseType(typeof(SelectVariantResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SelectVariant(int testId, [FromBody] SelectVariantRequest request)
    {
        var variant = await _abTestingService.SelectVariantAsync(testId, request.UserId);
        if (variant == null)
            return NotFound();

        var response = new SelectVariantResponse
        {
            TestId = testId,
            UserId = request.UserId,
            SelectedVariant = new AbTestVariantDto
            {
                Id = variant.Id,
                Name = variant.Name,
                Description = variant.Description,
                Weight = (double)variant.Weight,
                IsControl = variant.IsControl
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Record a result for an A/B test
    /// </summary>
    [HttpPost("tests/{testId}/results")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RecordResult(int testId, [FromBody] RecordResultRequest request)
    {
        // Get the variant that was selected for this user
        var variant = await _abTestingService.SelectVariantAsync(testId, request.UserId);

        await _abTestingService.RecordResultAsync(
            testId,
            variant.Id,
            0, // conversationId (not tracked in this simplified API)
            request.UserId,
            request.Converted ? "conversion" : "view",
            request.ConversionValue ?? 0m);

        return Created("", new { message = "Result recorded successfully" });
    }

    /// <summary>
    /// Update test status
    /// </summary>
    [HttpPut("tests/{id}/status")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTestStatus(int id, [FromBody] UpdateTestStatusRequest request)
    {
        var success = await _abTestingService.UpdateTestStatusAsync(id, request.Status);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete an A/B test
    /// </summary>
    [HttpDelete("tests/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTest(int id)
    {
        var success = await _abTestingService.DeleteTestAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    private static AbTestStatisticsDto MapToStatisticsDto(AbTestStatistics statistics)
    {
        var variantRates = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var v in statistics.VariantStatistics)
        {
            var conversionMetric = v.Metrics.GetValueOrDefault("conversion");
            variantRates[v.VariantName] = conversionMetric != null ? (double)conversionMetric.Mean : 0;
        }

        return new AbTestStatisticsDto
        {
            TotalParticipants = statistics.VariantStatistics.Sum(v => v.SampleSize),
            ConversionRate = variantRates.Values.Any() ? variantRates.Values.Average() : 0,
            BestVariant = variantRates.OrderByDescending(kv => kv.Value).FirstOrDefault().Key ?? "None",
            VariantConversionRates = variantRates,
            StatisticalSignificance = null // Could calculate later
        };
    }
}

/// <summary>
/// API controller for advanced NLP analysis
/// </summary>
[ApiController]
[Route("api/mcp/nlp")]
[Authorize]
public class NlpController : ControllerBase
{
    private readonly AdvancedNlpService _nlpService;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<NlpController> _logger;

    public NlpController(
        AdvancedNlpService nlpService,
        TenantContext tenantContext,
        ILogger<NlpController> logger)
    {
        _nlpService = nlpService;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Analyze user message and determine appropriate tool
    /// </summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(NlpAnalysisResult), 200)]
    public async Task<IActionResult> AnalyzeIntent([FromBody] AnalyzeIntentRequest request)
    {
        var result = await _nlpService.AnalyzeIntentAsync(request.Message, request.ConversationId);
        return Ok(result);
    }

    /// <summary>
    /// Learn from user feedback to improve NLP model
    /// </summary>
    [HttpPost("learn")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> LearnFromFeedback([FromBody] LearnFromFeedbackRequest request)
    {
        await _nlpService.LearnFromFeedbackAsync(request.Message, request.CorrectTool, request.UserId);
        return Ok(new { message = "Feedback recorded for learning" });
    }

    /// <summary>
    /// Get conversation context for better intent recognition
    /// </summary>
    [HttpGet("context/{conversationId}")]
    [ProducesResponseType(typeof(NlpConversationContext), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversationContext(int conversationId)
    {
        try
        {
            var context = await _nlpService.GetConversationContextAsync(conversationId);
            return Ok(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get conversation context for ID {ConversationId}", conversationId);
            return NotFound();
        }
    }
}

// DTOs for API responses

public class ConversationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
    public string? LastMessage { get; set; }
}

public class ConversationDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
    public object? Context { get; set; }
}

public class MessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string? ToolName { get; set; }
    public bool IsError { get; set; }
    public DateTime CreatedAt { get; set; }
    public object? Entities { get; set; }
    public object? Parameters { get; set; }
}

public class AbTestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AbTestVariantDto> Variants { get; set; } = new();
    public AbTestStatisticsDto Statistics { get; set; } = new();
}

public class AbTestDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AbTestVariantDto> Variants { get; set; } = new();
    public List<AbTestResultDto> Results { get; set; } = new();
    public AbTestStatisticsDto Statistics { get; set; } = new();
}

public class AbTestVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Weight { get; set; }
    public bool IsControl { get; set; }
}

public class AbTestResultDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int VariantId { get; set; }
    public bool Converted { get; set; }
    public decimal? ConversionValue { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AbTestStatisticsDto
{
    public int TotalParticipants { get; set; }
    public double ConversionRate { get; set; }
    public string BestVariant { get; set; } = string.Empty;
    public Dictionary<string, double>? VariantConversionRates { get; set; }
    public double? StatisticalSignificance { get; set; }
}

// Request DTOs

public class CreateConversationRequest
{
    public string Title { get; set; } = string.Empty;
}

public class AddMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string? ToolName { get; set; }
    public bool IsError { get; set; }
    public object? Entities { get; set; }
    public object? Parameters { get; set; }
}

public class UpdateContextRequest
{
    public object ContextData { get; set; } = new();
}

public class CreateAbTestRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<CreateAbTestVariantRequest> Variants { get; set; } = new();
}

public class CreateAbTestVariantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Weight { get; set; } = 1.0;
    public bool IsControl { get; set; }
}

public class SelectVariantRequest
{
    public string UserId { get; set; } = string.Empty;
}

public class SelectVariantResponse
{
    public int TestId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public AbTestVariantDto SelectedVariant { get; set; } = new();
}

public class RecordResultRequest
{
    public string UserId { get; set; } = string.Empty;
    public bool Converted { get; set; }
    public decimal? ConversionValue { get; set; }
}

public class UpdateTestStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class AnalyzeIntentRequest
{
    public string Message { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}

public class LearnFromFeedbackRequest
{
    public string Message { get; set; } = string.Empty;
    public string CorrectTool { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
