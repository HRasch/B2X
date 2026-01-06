using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// Base Controller für alle API Endpoints
/// Zentralisiert gemeinsame Funktionalität:
/// - Tenant-ID Extraction
/// - StandardisierteResponse-Format
/// - Logger Access
///
/// Verwendung:
/// public class ProductsController : ApiControllerBase { }
/// </summary>
[ApiController]
// [Authorize] // Temporarily disabled for testing
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected readonly ILogger _logger;
#pragma warning restore CA1051 // Do not declare visible instance fields

    protected ApiControllerBase(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extrahiere Tenant-ID aus HttpContext (wurde vom ValidateTenantAttribute gesetzt)
    /// </summary>
    protected Guid GetTenantId()
    {
        if (HttpContext.Items.TryGetValue("TenantId", out var tenantId) && tenantId is Guid guid)
        {
            return guid;
        }

        throw new InvalidOperationException("TenantId not found in request context. " +
            "Ensure ValidateTenantAttribute is applied to the controller or action.");
    }

    /// <summary>
    /// Extrahiere aktuellen User-ID aus JWT Claims
    /// </summary>
    protected Guid GetUserId()
    {
        // Temporarily return a test user ID for development/testing
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Guid.Parse("12345678-1234-1234-1234-123456789abc"); // Test user ID
        }

        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("User ID not found in JWT claims");
    }

    /// <summary>
    /// Überprüfe ob aktueller User eine bestimmte Role hat
    /// </summary>
    protected bool HasRole(string role)
    {
        return User.IsInRole(role);
    }

    /// <summary>
    /// Standardisierte Success Response (200 OK) - Generic version
    /// </summary>
    protected ActionResult<T> OkResponse<T>(T data, string? message = null)
    {
        return Ok(new
        {
            success = true,
            data,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Success Response (200 OK) - Non-generic version for dynamic content
    /// Use this when the return type is ActionResult (non-generic) or when returning dynamic/anonymous types
    /// </summary>
    protected ActionResult OkResponseDynamic(object data, string? message = null)
    {
        return Ok(new
        {
            success = true,
            data,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Created Response (201 Created) - Generic version
    /// </summary>
    protected ActionResult<T> CreatedResponse<T>(string routeName, object? routeValues, T data)
    {
        return CreatedAtAction(routeName, routeValues, new
        {
            success = true,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Created Response (201 Created) - Non-generic version for dynamic content
    /// Use this when the return type is ActionResult (non-generic) or when returning dynamic/anonymous types
    /// </summary>
    protected ActionResult CreatedResponseDynamic(string routeName, object? routeValues, object data)
    {
        return CreatedAtAction(routeName, routeValues, new
        {
            success = true,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Error Response (400 Bad Request)
    /// </summary>
    protected ActionResult BadRequestResponse(string error, string errorCode = "VALIDATION_ERROR")
    {
        return BadRequest(new
        {
            success = false,
            error,
            errorCode,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte NotFound Response (404 Not Found)
    /// </summary>
    protected ActionResult NotFoundResponse(string error = "Resource not found")
    {
        return NotFound(new
        {
            success = false,
            error,
            errorCode = "NOT_FOUND",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Conflict Response (409 Conflict)
    /// </summary>
    protected ActionResult ConflictResponse(string error)
    {
        return Conflict(new
        {
            success = false,
            error,
            errorCode = "CONFLICT",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte Forbidden Response (403 Forbidden)
    /// </summary>
    protected ActionResult ForbiddenResponse(string error = "Access denied")
    {
        return StatusCode(StatusCodes.Status403Forbidden, new
        {
            success = false,
            error,
            errorCode = "FORBIDDEN",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Standardisierte InternalError Response (500 Internal Server Error)
    /// </summary>
    protected ActionResult InternalErrorResponse(string error = "An internal error occurred")
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new
        {
            success = false,
            error,
            errorCode = "INTERNAL_ERROR",
            timestamp = DateTime.UtcNow
        });
    }
}
