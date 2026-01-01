using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Gateway.Shared.Application.DTOs;
using B2Connect.Domain.Support.Commands;
using B2Connect.Domain.Support.Queries;
using B2Connect.Domain.Support.Handlers;
using Wolverine;

namespace B2Connect.Gateway.Shared.Presentation.Controllers;

/// <summary>
/// Feedback Controller - Handles user feedback and support requests
///
/// Features:
/// - Anonymous feedback submission (GDPR compliant)
/// - Automatic GitHub issue creation
/// - Context data collection and anonymization
/// - File attachment support
/// - Rate limiting and validation
/// </summary>
[ApiController]
[Route("api/support/feedback")]
[Produces("application/json")]
public class FeedbackController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(
        IMessageBus messageBus,
        ILogger<FeedbackController> logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates feedback before submission
    /// </summary>
    /// <param name="request">Feedback validation request</param>
    /// <returns>Validation result with reasons if rejected</returns>
    /// <response code="200">Validation completed</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("validate")]
    [AllowAnonymous] // Public access for validation
    [ProducesResponseType(typeof(ValidateFeedbackResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ValidateFeedback([FromBody] CreateFeedbackRequest request)
    {
        try
        {
            // Validate request format
            if (!IsValidRequest(request, out var validationError))
            {
                _logger.LogWarning("Invalid validation request: {Error}", validationError);
                return BadRequest(new ValidateFeedbackResponse
                {
                    IsValid = false,
                    Status = "Rejected",
                    Message = "Invalid request format",
                    Reasons = new[] { validationError },
                    Severity = "High"
                });
            }

            // Convert DTOs to domain objects
            var attachments = request.Attachments.Select(a => new Attachment(
                a.FileName,
                a.ContentType,
                a.Content)).ToList();

            // Create validation command
            var command = new ValidateFeedbackCommand
            {
                Category = request.Category,
                Description = request.Description,
                Context = request.Context,
                Attachments = attachments,
                CorrelationId = Guid.NewGuid()
            };

            // Execute validation via Wolverine
            var result = await _messageBus.InvokeAsync<ValidationResult>(command);

            var response = new ValidateFeedbackResponse
            {
                IsValid = result.IsValid,
                Status = result.Status.ToString(),
                Message = result.Message,
                Reasons = result.Reasons,
                Severity = result.Severity.ToString()
            };

            _logger.LogInformation("Feedback validation completed: {Status} - {Message}",
                result.Status, result.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate feedback");
            return StatusCode(500, new ValidateFeedbackResponse
            {
                IsValid = false,
                Status = "Rejected",
                Message = "Validation failed due to an internal error. Please try again later.",
                Reasons = new[] { "Internal validation error" },
                Severity = "Critical"
            });
        }
    }

    /// <summary>
    /// Creates new feedback from user input
    /// </summary>
    /// <param name="request">Feedback creation request</param>
    /// <returns>Feedback creation result with correlation ID</returns>
    /// <response code="200">Feedback created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="429">Rate limit exceeded</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [AllowAnonymous] // Public access for feedback submission
    [ProducesResponseType(typeof(CreateFeedbackResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
    {
        try
        {
            // Validate request
            if (!IsValidRequest(request, out var validationError))
            {
                _logger.LogWarning("Invalid feedback request: {Error}", validationError);
                return BadRequest(new { error = validationError });
            }

            // Convert DTOs to domain objects
            var attachments = request.Attachments.Select(a => new Attachment(
                a.FileName,
                a.ContentType,
                a.Content)).ToList();

            // Create command
            var command = new CreateFeedbackCommand
            {
                Category = request.Category,
                Description = request.Description,
                Context = request.Context,
                Attachments = attachments,
                CorrelationId = Guid.NewGuid()
            };

            // Execute command via Wolverine
            var result = await _messageBus.InvokeAsync<FeedbackResult>(command);

            var response = new CreateFeedbackResponse
            {
                CorrelationId = result.CorrelationId,
                IssueUrl = result.IssueUrl,
                Status = result.Status,
                Message = "Vielen Dank für Ihr Feedback! Wir kümmern uns schnellstmöglich darum."
            };

            _logger.LogInformation("Feedback created successfully with correlation ID {CorrelationId}",
                result.CorrelationId);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Feedback rejected due to validation: {Message}", ex.Message);
            return BadRequest(new
            {
                error = "Feedback validation failed",
                message = ex.Message,
                reasons = ex.ValidationErrors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create feedback");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Gets feedback status by correlation ID
    /// </summary>
    /// <param name="correlationId">Correlation ID from feedback creation</param>
    /// <returns>Current feedback status</returns>
    /// <response code="200">Status retrieved successfully</response>
    /// <response code="404">Feedback not found</response>
    /// <response code="403">Access denied</response>
    [HttpGet("{correlationId}")]
    [Authorize(Roles = "Admin,Support")] // Admin/Support only for status checks
    [ProducesResponseType(typeof(FeedbackStatusResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetFeedbackStatus(Guid correlationId)
    {
        try
        {
            var query = new GetFeedbackStatusQuery { CorrelationId = correlationId };
            var result = await _messageBus.InvokeAsync<FeedbackStatusResult>(query);

            var response = new FeedbackStatusResponse
            {
                CorrelationId = result.CorrelationId,
                Status = result.Status.ToString().ToLower(),
                GitHubIssueUrl = result.GitHubIssueUrl,
                Resolution = result.Resolution,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get feedback status for {CorrelationId}", correlationId);
            return NotFound(new { error = "Feedback not found" });
        }
    }

    /// <summary>
    /// Validates the feedback creation request
    /// </summary>
    private bool IsValidRequest(CreateFeedbackRequest request, out string error)
    {
        // Basic validation
        if (request == null)
        {
            error = "Request cannot be null";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            error = "Description is required";
            return false;
        }

        if (request.Description.Length < FeedbackValidationRules.MinDescriptionLength ||
            request.Description.Length > FeedbackValidationRules.MaxDescriptionLength)
        {
            error = $"Description must be between {FeedbackValidationRules.MinDescriptionLength} and {FeedbackValidationRules.MaxDescriptionLength} characters";
            return false;
        }

        if (request.Context == null)
        {
            error = "Context information is required";
            return false;
        }

        // Validate attachments
        if (request.Attachments != null)
        {
            if (!FeedbackValidationRules.IsAttachmentCountValid(request.Attachments.Count))
            {
                error = $"Maximum {FeedbackValidationRules.MaxAttachments} attachments allowed";
                return false;
            }

            foreach (var attachment in request.Attachments)
            {
                if (string.IsNullOrWhiteSpace(attachment.FileName))
                {
                    error = "Attachment filename is required";
                    return false;
                }

                if (!FeedbackValidationRules.IsContentTypeAllowed(attachment.ContentType))
                {
                    error = $"Content type {attachment.ContentType} is not allowed";
                    return false;
                }

                if (!FeedbackValidationRules.IsAttachmentSizeValid(attachment.Size))
                {
                    error = $"Attachment size must be between 1 and {FeedbackValidationRules.MaxAttachmentSize} bytes";
                    return false;
                }
            }
        }

        // Check for malicious content
        if (ContainsMaliciousContent(request.Description))
        {
            error = "Invalid content detected";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>
    /// Basic check for malicious content in user input
    /// </summary>
    private bool ContainsMaliciousContent(string content)
    {
        if (string.IsNullOrEmpty(content)) return false;

        var maliciousPatterns = new[]
        {
            @"<script", @"javascript:", @"on\w+\s*=",
            @"<iframe", @"<object", @"<embed",
            @"<form", @"<input", @"<button"
        };

        return maliciousPatterns.Any(pattern =>
            content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Gateway/Shared/src/Presentation/Controllers/FeedbackController.cs