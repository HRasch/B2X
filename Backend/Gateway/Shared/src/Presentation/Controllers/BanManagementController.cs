using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Domain.Support.Services;

namespace B2Connect.Gateway.Shared.Presentation.Controllers;

/// <summary>
/// Ban Management Controller - Admin endpoints for managing IP/session bans
///
/// Features:
/// - View active bans and statistics
/// - Lift permanent bans (admin override)
/// - Monitor ban effectiveness
/// - Security audit trail
/// </summary>
[ApiController]
[Route("api/admin/bans")]
[Authorize(Policy = "AdminOnly")]
[Produces("application/json")]
public class BanManagementController : ControllerBase
{
    private readonly IBanManagementService _banService;
    private readonly ILogger<BanManagementController> _logger;

    public BanManagementController(
        IBanManagementService banService,
        ILogger<BanManagementController> logger)
    {
        _banService = banService ?? throw new ArgumentNullException(nameof(banService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get ban statistics for monitoring
    /// </summary>
    /// <returns>Ban statistics including active bans and trends</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(BanStatisticsResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBanStatistics()
    {
        try
        {
            var adminUserId = User.Identity?.Name ?? "system";
            _logger.LogInformation("Admin {AdminUserId} requested ban statistics", adminUserId);

            var stats = await _banService.GetBanStatisticsAsync();

            var response = new BanStatisticsResponse
            {
                ActiveTemporaryBans = stats.ActiveTemporaryBans,
                ActivePermanentBans = stats.ActivePermanentBans,
                TotalBansToday = stats.TotalBansToday,
                TotalBansThisWeek = stats.TotalBansThisWeek,
                BansByThreatLevel = stats.BansByThreatLevel,
                BansByRequestType = stats.BansByRequestType,
                RetrievedAt = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve ban statistics");
            return StatusCode(500, new { error = "Failed to retrieve ban statistics" });
        }
    }

    /// <summary>
    /// Lift a permanent ban (admin override)
    /// </summary>
    /// <param name="identifier">The identifier (IP/session) to unban</param>
    /// <returns>Success status</returns>
    /// <response code="200">Ban lifted successfully</response>
    /// <response code="400">Invalid request or ban not found</response>
    /// <response code="403">Not authorized to lift this ban</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("{identifier}/lift")]
    [ProducesResponseType(typeof(LiftBanResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> LiftPermanentBan(string identifier)
    {
        try
        {
            var adminUserId = User.Identity?.Name ?? "system";
            _logger.LogInformation("Admin {AdminUserId} attempting to lift permanent ban for identifier {Identifier}",
                adminUserId, identifier);

            var success = await _banService.LiftPermanentBanAsync(identifier, adminUserId);

            if (!success)
            {
                _logger.LogWarning("Failed to lift permanent ban for identifier {Identifier} by admin {AdminUserId}",
                    identifier, adminUserId);
                return BadRequest(new LiftBanResponse
                {
                    Success = false,
                    Message = "Ban not found or not a permanent ban",
                    Identifier = identifier
                });
            }

            _logger.LogInformation("Admin {AdminUserId} successfully lifted permanent ban for identifier {Identifier}",
                adminUserId, identifier);

            return Ok(new LiftBanResponse
            {
                Success = true,
                Message = "Permanent ban lifted successfully",
                Identifier = identifier,
                LiftedBy = adminUserId,
                LiftedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lift permanent ban for identifier {Identifier}", identifier);
            return StatusCode(500, new { error = "Failed to lift permanent ban" });
        }
    }

    /// <summary>
    /// Check ban status for a specific identifier
    /// </summary>
    /// <param name="identifier">The identifier to check</param>
    /// <param name="requestType">The type of request (optional)</param>
    /// <returns>Ban status information</returns>
    /// <response code="200">Ban status retrieved</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{identifier}/status")]
    [ProducesResponseType(typeof(BanStatusResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CheckBanStatus(string identifier, [FromQuery] string? requestType = null)
    {
        try
        {
            var adminUserId = User.Identity?.Name ?? "system";
            _logger.LogInformation("Admin {AdminUserId} checking ban status for identifier {Identifier}",
                adminUserId, identifier);

            var banCheck = await _banService.CheckBanStatusAsync(identifier, requestType ?? "feedback");

            var response = new BanStatusResponse
            {
                Identifier = identifier,
                IsBanned = banCheck.IsBanned,
                BanType = banCheck.BanType.ToString(),
                BanExpiresAt = banCheck.BanExpiresAt,
                Reason = banCheck.Reason,
                StrikeCount = banCheck.StrikeCount,
                CheckedAt = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check ban status for identifier {Identifier}", identifier);
            return StatusCode(500, new { error = "Failed to check ban status" });
        }
    }
}

/// <summary>
/// Response DTO for ban statistics
/// </summary>
public class BanStatisticsResponse
{
    public int ActiveTemporaryBans { get; set; }
    public int ActivePermanentBans { get; set; }
    public int TotalBansToday { get; set; }
    public int TotalBansThisWeek { get; set; }
    public Dictionary<string, int> BansByThreatLevel { get; set; } = new();
    public Dictionary<string, int> BansByRequestType { get; set; } = new();
    public DateTime RetrievedAt { get; set; }
}

/// <summary>
/// Response DTO for lifting a ban
/// </summary>
public class LiftBanResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? LiftedBy { get; set; }
    public DateTime? LiftedAt { get; set; }
}

/// <summary>
/// Response DTO for ban status check
/// </summary>
public class BanStatusResponse
{
    public string Identifier { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public string BanType { get; set; } = string.Empty;
    public DateTime? BanExpiresAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int StrikeCount { get; set; }
    public DateTime CheckedAt { get; set; }
}