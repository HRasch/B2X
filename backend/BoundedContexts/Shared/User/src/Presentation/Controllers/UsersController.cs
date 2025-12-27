using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Shared.User.Presentation.Controllers;

/// <summary>
/// Users API mit vollständigem Custom Properties Support
/// Model Binding funktioniert automatisch für IExtensibleEntity
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IEntityExtensionService _extensionService;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IMediator mediator,
        IUserRepository userRepository,
        IEntityExtensionService extensionService,
        IMapper mapper,
        ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _extensionService = extensionService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/users/{id}
    /// Hole User mit Custom Properties
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserWithExtensionsDto>> GetUser(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var user = await _userRepository.GetByIdAsync(tenantId, id);

        if (user == null)
            return NotFound();

        var dto = _mapper.Map<UserWithExtensionsDto>(user);
        dto.CustomProperties = _extensionService.GetAllCustomProperties(user);

        return Ok(dto);
    }

    /// <summary>
    /// POST /api/users
    /// Erstelle neuen User mit Custom Properties
    /// 
    /// Request Body:
    /// {
    ///   "email": "john@example.com",
    ///   "firstName": "John",
    ///   "lastName": "Doe",
    ///   "phoneNumber": "+49 123 456789",
    ///   "customProperties": {
    ///     "erp_customer_id": "CUST-12345",
    ///     "warehouse_code": "WH-001",
    ///     "customer_segment": "Premium"
    ///   }
    /// }
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    public async Task<ActionResult<UserWithExtensionsDto>> CreateUser(
        [FromBody] CreateUserRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var command = new CreateUserCommand(
            TenantId: tenantId,
            Email: request.Email,
            FirstName: request.FirstName,
            LastName: request.LastName,
            PhoneNumber: request.PhoneNumber);

        var user = await _mediator.Send(command);

        // Wenn Custom Properties im Request enthalten sind, setze diese
        if (request.CustomProperties != null && request.CustomProperties.Any())
        {
            foreach (var kvp in request.CustomProperties)
            {
                var isValid = await _extensionService.ValidateCustomPropertyAsync(
                    tenantId,
                    nameof(User),
                    kvp.Key,
                    kvp.Value);

                if (isValid)
                {
                    _extensionService.SetCustomProperty(user, kvp.Key, kvp.Value);
                }
            }

            await _userRepository.SaveChangesAsync();
        }

        var dto = _mapper.Map<UserWithExtensionsDto>(user);
        dto.CustomProperties = _extensionService.GetAllCustomProperties(user);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, dto);
    }

    /// <summary>
    /// PUT /api/users/{id}
    /// Update User mit Custom Properties
    /// 
    /// Request Body:
    /// {
    ///   "firstName": "John",
    ///   "lastName": "Doe",
    ///   "phoneNumber": "+49 123 456789",
    ///   "customProperties": {
    ///     "erp_customer_number": "123456",
    ///     "credit_limit": 75000.00
    ///   }
    /// }
    /// </summary>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    public async Task<ActionResult<UserWithExtensionsDto>> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var user = await _userRepository.GetByIdAsync(tenantId, id);

        if (user == null)
            return NotFound();

        // Update standard properties
        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        // Update Custom Properties
        if (request.CustomProperties != null && request.CustomProperties.Any())
        {
            foreach (var kvp in request.CustomProperties)
            {
                var isValid = await _extensionService.ValidateCustomPropertyAsync(
                    tenantId,
                    nameof(User),
                    kvp.Key,
                    kvp.Value);

                if (!isValid)
                {
                    ModelState.AddModelError($"customProperties.{kvp.Key}",
                        "Invalid custom property value");
                    continue;
                }

                var oldValue = _extensionService.GetCustomProperty<object>(user, kvp.Key);
                _extensionService.SetCustomProperty(user, kvp.Key, kvp.Value);

                // Audit Log
                await _extensionService.LogCustomPropertyChangeAsync(
                    user.Id,
                    tenantId,
                    nameof(User),
                    kvp.Key,
                    oldValue,
                    kvp.Value,
                    changedBy: GetCurrentUserId());
            }
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        var dto = _mapper.Map<UserWithExtensionsDto>(user);
        dto.CustomProperties = _extensionService.GetAllCustomProperties(user);

        return Ok(dto);
    }

    /// <summary>
    /// PATCH /api/users/{id}/custom-properties/{fieldName}
    /// Update einzelnes Custom Field mit Validierung
    /// 
    /// Request Body: "CUST-99999"
    /// </summary>
    [HttpPatch("{id}/custom-properties/{fieldName}")]
    [Consumes("application/json")]
    public async Task<ActionResult> UpdateCustomProperty(
        Guid id,
        string fieldName,
        [FromBody] object? value,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var user = await _userRepository.GetByIdAsync(tenantId, id);

        if (user == null)
            return NotFound();

        // Validiere
        var isValid = await _extensionService.ValidateCustomPropertyAsync(
            tenantId,
            nameof(User),
            fieldName,
            value);

        if (!isValid)
            return BadRequest(new { error = $"Invalid value for field '{fieldName}'" });

        // Update
        var oldValue = _extensionService.GetCustomProperty<object>(user, fieldName);
        _extensionService.SetCustomProperty(user, fieldName, value);

        // Audit Log
        await _extensionService.LogCustomPropertyChangeAsync(
            user.Id,
            tenantId,
            nameof(User),
            fieldName,
            oldValue,
            value,
            changedBy: GetCurrentUserId(),
            reason: "Manual update via API");

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Updated custom property '{FieldName}' for user {UserId}",
            fieldName, id);

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/users/{id}/custom-properties/{fieldName}
    /// Lösche einzelnes Custom Field
    /// </summary>
    [HttpDelete("{id}/custom-properties/{fieldName}")]
    public async Task<ActionResult> DeleteCustomProperty(
        Guid id,
        string fieldName,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var user = await _userRepository.GetByIdAsync(tenantId, id);

        if (user == null)
            return NotFound();

        var oldValue = _extensionService.GetCustomProperty<object>(user, fieldName);

        // Setze auf null (oder lösche aus Dictionary)
        _extensionService.SetCustomProperty(user, fieldName, null);

        // Audit Log
        await _extensionService.LogCustomPropertyChangeAsync(
            user.Id,
            tenantId,
            nameof(User),
            fieldName,
            oldValue,
            null,
            changedBy: GetCurrentUserId(),
            reason: "Deleted via API");

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value
            ?? User.FindFirst("userId")?.Value
            ?? Guid.Empty.ToString();

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

// ============ DTOs ============

/// <summary>
/// User DTO mit Custom Properties Support
/// </summary>
public class UserWithExtensionsDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Custom Properties werden automatisch vom Model Binder geparst
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("customProperties")]
    public Dictionary<string, object?> CustomProperties { get; set; } = new();
}

public class CreateUserRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Email { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    public string FirstName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    public string LastName { get; set; } = "";

    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Custom Properties werden vom Model Binder automatisch validiert
    /// </summary>
    public Dictionary<string, object?>? CustomProperties { get; set; }
}

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Custom Properties werden vom Model Binder automatisch validiert
    /// </summary>
    public Dictionary<string, object?>? CustomProperties { get; set; }
}
