using System.Security.Claims;
using B2X.Core.Interfaces;
using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Api.Models;
using B2X.Shared.Monitoring.Data;
using B2X.Shared.Monitoring.Data.Entities;
using B2X.Shared.Monitoring.Infrastructure.Context;
using B2X.Types.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Monitoring.Api.Controllers;

/// <summary>
/// Controller for realtime debug functionality
/// </summary>
[ApiController]
[Route("api/debug")]
[Authorize] // Require authentication for debug operations
public class DebugController : ControllerBase
{
    private readonly MonitoringDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IDebugEventBroadcaster _broadcaster;
    private readonly ILogger<DebugController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugController"/> class.
    /// </summary>
    public DebugController(
        MonitoringDbContext context,
        ITenantContext tenantContext,
        IDebugEventBroadcaster broadcaster,
        ILogger<DebugController> logger)
    {
        _context = context;
        _tenantContext = tenantContext;
        _broadcaster = broadcaster;
        _logger = logger;
    }

    /// <summary>
    /// Start a new debug session
    /// </summary>
    [HttpPost("session/start")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StartSession([FromBody] StartDebugSessionRequest request)
    {
        try
        {
            var session = new DebugSessionEntity
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                CorrelationId = HttpContext.GetCorrelationId(),
                UserId = GetCurrentUserId(),
                UserAgent = request.UserAgent,
                Viewport = request.Viewport,
                Environment = request.Environment,
                Metadata = request.Metadata,
                StartedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            _context.DebugSessions.Add(session);
            await _context.SaveChangesAsync();

            // Broadcast session start via SignalR
            await _broadcaster.BroadcastSessionStartedAsync(session);

            _logger.LogInformation("Debug session started: {SessionId}, CorrelationId: {CorrelationId}",
                session.Id, session.CorrelationId);

            return Ok(session.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start debug session");
            return BadRequest("Failed to start debug session");
        }
    }

    /// <summary>
    /// Capture an error during a debug session
    /// </summary>
    [HttpPost("error/capture")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CaptureError([FromBody] CaptureErrorRequest request)
    {
        try
        {
            // Verify session exists
            var session = await _context.DebugSessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId);

            if (session == null)
            {
                return NotFound("Debug session not found");
            }

            var error = new DebugErrorEntity
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                CorrelationId = HttpContext.GetCorrelationId(),
                Level = request.Level,
                Message = request.Message,
                StackTrace = request.StackTrace,
                SourceFile = request.SourceFile,
                LineNumber = request.LineNumber,
                ColumnNumber = request.ColumnNumber,
                Url = request.Url,
                UserAgent = request.UserAgent,
                Viewport = request.Viewport,
                Context = request.Context,
                OccurredAt = DateTime.UtcNow,
                IsResolved = false
            };

            _context.DebugErrors.Add(error);
            await _context.SaveChangesAsync();

            // Broadcast error capture via SignalR
            var tenantId = _tenantContext.TenantId.ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                await _broadcaster.BroadcastErrorCapturedAsync(error, tenantId);
            }

            _logger.LogInformation("Error captured: {ErrorId}, Session: {SessionId}, Message: {Message}",
                error.Id, request.SessionId, request.Message);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture error for session {SessionId}", request.SessionId);
            return BadRequest("Failed to capture error");
        }
    }

    /// <summary>
    /// Submit feedback during a debug session
    /// </summary>
    [HttpPost("feedback/submit")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
    {
        try
        {
            // Verify session exists
            var session = await _context.DebugSessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId);

            if (session == null)
            {
                return NotFound("Debug session not found");
            }

            var feedback = new DebugFeedbackEntity
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                CorrelationId = HttpContext.GetCorrelationId(),
                Type = request.Type,
                Title = request.Title,
                Description = request.Description,
                Rating = request.Rating,
                Url = request.Url,
                UserAgent = request.UserAgent,
                Viewport = request.Viewport,
                Screenshot = request.Screenshot,
                Metadata = request.Metadata,
                SubmittedAt = DateTime.UtcNow,
                IsReviewed = false,
                Priority = "medium"
            };

            _context.DebugFeedback.Add(feedback);
            await _context.SaveChangesAsync();

            // Broadcast feedback submission via SignalR
            var tenantId = _tenantContext.TenantId.ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                await _broadcaster.BroadcastFeedbackSubmittedAsync(feedback, tenantId);
            }

            _logger.LogInformation("Feedback submitted: {FeedbackId}, Session: {SessionId}, Type: {Type}",
                feedback.Id, request.SessionId, request.Type);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit feedback for session {SessionId}", request.SessionId);
            return BadRequest("Failed to submit feedback");
        }
    }

    /// <summary>
    /// Capture a user action during a debug session
    /// </summary>
    [HttpPost("action/capture")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CaptureAction([FromBody] CaptureActionRequest request)
    {
        try
        {
            // Verify session exists
            var session = await _context.DebugSessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId);

            if (session == null)
            {
                return NotFound("Debug session not found");
            }

            // Get the next sequence number for this session
            var maxSequence = await _context.DebugActions
                .Where(a => a.SessionId == request.SessionId)
                .MaxAsync(a => (int?)a.SequenceNumber) ?? 0;

            var action = new DebugActionEntity
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                CorrelationId = HttpContext.GetCorrelationId(),
                ActionType = request.ActionType,
                Description = request.Description,
                TargetSelector = request.TargetSelector,
                Url = request.Url,
                Coordinates = request.Coordinates,
                FormData = request.FormData,
                Metadata = request.Metadata,
                OccurredAt = DateTime.UtcNow,
                SequenceNumber = maxSequence + 1
            };

            _context.DebugActions.Add(action);
            await _context.SaveChangesAsync();

            // Broadcast action capture via SignalR
            var tenantId = _tenantContext.TenantId.ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                await _broadcaster.BroadcastActionRecordedAsync(action, tenantId);
            }

            _logger.LogDebug("Action captured: {ActionId}, Session: {SessionId}, Type: {ActionType}",
                action.Id, request.SessionId, request.ActionType);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture action for session {SessionId}", request.SessionId);
            return BadRequest("Failed to capture action");
        }
    }

    /// <summary>
    /// Update session last activity timestamp
    /// </summary>
    [HttpPost("session/{sessionId}/heartbeat")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SessionHeartbeat(Guid sessionId)
    {
        try
        {
            var session = await _context.DebugSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound("Debug session not found");
            }

            session.LastActivityAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update session heartbeat for {SessionId}", sessionId);
            return BadRequest("Failed to update session");
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                          User.FindFirst("sub");

        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId)
            ? userId
            : null;
    }
}

/// <summary>
/// Extension methods for HttpContext to access correlation ID
/// </summary>
public static class HttpContextExtensions
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private const string CorrelationIdItemKey = "CorrelationId";

    /// <summary>
    /// Gets the correlation ID from the current HttpContext
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns>The correlation ID, or generates a new one if not present</returns>
    public static string GetCorrelationId(this HttpContext context)
    {
        // Try to get from request headers first
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue) &&
            !string.IsNullOrEmpty(headerValue.ToString()))
        {
            var correlationId = headerValue.ToString()!;
            context.Items[CorrelationIdItemKey] = correlationId;
            return correlationId;
        }

        // Try to get from context items (set by middleware)
        if (context.Items.TryGetValue(CorrelationIdItemKey, out var itemValue) &&
            itemValue is string existingId)
        {
            return existingId;
        }

        // Generate new correlation ID
        var newId = Guid.NewGuid().ToString();
        context.Items[CorrelationIdItemKey] = newId;
        return newId;
    }
}