using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using B2Connect.Admin.Presentation.Filters;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// Admin Users Controller - BFF GATEWAY
/// This controller acts as a Backend-for-Frontend (BFF) gateway to the Identity Service.
/// Actual user management is delegated to the Identity microservice via HTTP.
/// 
/// Flow:
/// 1. Admin Frontend (Port 5174) → Admin API (Port 8080) - This controller
/// 2. Admin API → Identity Service (Port 7002) - Proxies requests
/// 3. Identity Service → PostgreSQL - User data persistence
/// 
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// - ValidateModelStateFilter: Validates request models
/// - ApiLoggingFilter: Logs all requests and responses
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize]
[ValidateTenant]
public class UsersController : ApiControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;

    public UsersController(
        ILogger<UsersController> logger,
        HttpClient httpClient,
        IConfiguration configuration) : base(logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        // Identity Service runs on port 7002
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
    }

    /// <summary>
    /// Get all users for the tenant
    /// Proxies to: GET /api/identity/users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetUsers(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching users for tenant: {TenantId}", tenantId);

        // Forward request to Identity Service
        var url = $"{_identityServiceUrl}/api/identity/users";
        var response = await _httpClient.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error fetching users from Identity Service");
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        return OkResponse(JsonDocument.Parse(content).RootElement, "Users retrieved successfully");
    }

    /// <summary>
    /// Get a specific user by ID
    /// Proxies to: GET /api/identity/users/{userId}
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUser(Guid userId, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching user {UserId} for tenant {TenantId}", userId, tenantId);

        // Forward request to Identity Service
        var url = $"{_identityServiceUrl}/api/identity/users/{userId}";
        var response = await _httpClient.GetAsync(url, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFoundResponse($"User {userId} not found");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error fetching user from Identity Service");
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        return OkResponse(JsonDocument.Parse(content).RootElement, "User retrieved successfully");
    }

    /// <summary>
    /// Create a new user
    /// Proxies to: POST /api/identity/users
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateUser([FromBody] object request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating new user for tenant {TenantId}", userId, tenantId);

        // Forward request to Identity Service
        var url = $"{_identityServiceUrl}/api/identity/users";
        var content = JsonContent.Create(request);
        var response = await _httpClient.PostAsync(url, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            return StatusCode((int)response.StatusCode, errorContent);
        }

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        var jsonData = JsonDocument.Parse(responseContent);
        var createdUserId = jsonData.RootElement.GetProperty("id").GetGuid();

        return CreatedResponse(nameof(GetUser), new { userId = createdUserId }, jsonData.RootElement);
    }

    /// <summary>
    /// Update an existing user
    /// Proxies to: PUT /api/identity/users/{userId}
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] object request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var currentUserId = GetUserId();
        _logger.LogInformation("User {UserId} updating user {TargetUserId} for tenant {TenantId}",
            currentUserId, userId, tenantId);

        // Forward request to Identity Service
        var url = $"{_identityServiceUrl}/api/identity/users/{userId}";
        var content = JsonContent.Create(request);
        var response = await _httpClient.PutAsync(url, content, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFoundResponse($"User {userId} not found");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            return StatusCode((int)response.StatusCode, errorContent);
        }

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return OkResponse(JsonDocument.Parse(responseContent).RootElement, "User updated successfully");
    }

    /// <summary>
    /// Delete a user
    /// Proxies to: DELETE /api/identity/users/{userId}
    /// </summary>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(Guid userId, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var currentUserId = GetUserId();
        _logger.LogInformation("User {UserId} deleting user {TargetUserId} for tenant {TenantId}",
            currentUserId, userId, tenantId);

        // Forward request to Identity Service
        var url = $"{_identityServiceUrl}/api/identity/users/{userId}";
        var response = await _httpClient.DeleteAsync(url, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFoundResponse($"User {userId} not found");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting user from Identity Service");
        }

        return NoContent();
    }
}
