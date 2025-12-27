using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Shared.User.Models;
using B2Connect.Shared.User.Interfaces;
using B2Connect.Admin.Application.DTOs;

namespace B2Connect.Admin.Presentation.Controllers;

[ApiController]
[Route("api/admin/users")]
// [Authorize] // TODO: Re-enable after authentication is configured
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IAddressRepository addressRepository,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _addressRepository = addressRepository;
        _logger = logger;
    }

    private Guid GetTenantId()
    {
        var tenantId = Request.Headers["X-Tenant-ID"].ToString();
        return Guid.TryParse(tenantId, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedUserResponse>> GetUsers(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        if (tenantId == Guid.Empty)
            return Unauthorized("Missing X-Tenant-ID header");

        var users = await _userRepository.GetByTenantAsync(tenantId, ct);
        var dtos = users.Select(u => new UserDto(u.Id, u.TenantId, u.Email, u.FirstName, u.LastName,
            u.PhoneNumber, u.IsActive, u.IsEmailVerified, u.CreatedAt, u.UpdatedAt)).ToList();

        var pagination = new PaginationInfo(1, dtos.Count, dtos.Count, 1, false, false);
        return Ok(new PaginatedUserResponse(true, dtos, pagination, DateTime.UtcNow));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<SingleUserResponse>> GetUser(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            return NotFound();

        var addresses = await _addressRepository.GetByUserAsync(userId, ct);
        var addressDtos = addresses.Select(a => new AddressDto(a.Id, a.UserId, a.AddressType, a.StreetAddress,
            a.StreetAddress2, a.City, a.PostalCode, a.Country, a.State, a.RecipientName, a.PhoneNumber, a.IsDefault)).ToList();

        var profileDto = user.Profile != null ? new UserProfileDto(user.Profile.Id, user.Profile.UserId,
            user.Profile.AvatarUrl, user.Profile.Bio, user.Profile.DateOfBirth, user.Profile.CompanyName,
            user.Profile.JobTitle, user.Profile.PreferredLanguage, user.Profile.Timezone,
            user.Profile.ReceiveNewsletter, user.Profile.ReceivePromotionalEmails) : null;

        var userDetailDto = new UserDetailDto(user.Id, user.TenantId, user.Email, user.FirstName, user.LastName,
            user.PhoneNumber, user.IsActive, user.IsEmailVerified, user.CreatedAt, user.UpdatedAt, user.LastLoginAt,
            profileDto, addressDtos);

        return Ok(new SingleUserResponse(true, userDetailDto, DateTime.UtcNow));
    }

    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        if (tenantId == Guid.Empty)
            return Unauthorized("Missing X-Tenant-ID header");

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.CreateAsync(user, ct);
        var userDetailDto = new UserDetailDto(created.Id, created.TenantId, created.Email, created.FirstName, created.LastName,
            created.PhoneNumber, created.IsActive, created.IsEmailVerified, created.CreatedAt, created.UpdatedAt, created.LastLoginAt,
            null, new List<AddressDto>());

        return CreatedAtAction(nameof(GetUser), new { userId = created.Id },
            new CreateUserResponse(true, userDetailDto, DateTime.UtcNow));
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<SingleUserResponse>> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email;
        if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
        if (request.IsEmailVerified.HasValue) user.IsEmailVerified = request.IsEmailVerified.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, ct);

        var addresses = await _addressRepository.GetByUserAsync(userId, ct);
        var addressDtos = addresses.Select(a => new AddressDto(a.Id, a.UserId, a.AddressType, a.StreetAddress,
            a.StreetAddress2, a.City, a.PostalCode, a.Country, a.State, a.RecipientName, a.PhoneNumber, a.IsDefault)).ToList();

        var profileDto = user.Profile != null ? new UserProfileDto(user.Profile.Id, user.Profile.UserId,
            user.Profile.AvatarUrl, user.Profile.Bio, user.Profile.DateOfBirth, user.Profile.CompanyName,
            user.Profile.JobTitle, user.Profile.PreferredLanguage, user.Profile.Timezone,
            user.Profile.ReceiveNewsletter, user.Profile.ReceivePromotionalEmails) : null;

        var userDetailDto = new UserDetailDto(user.Id, user.TenantId, user.Email, user.FirstName, user.LastName,
            user.PhoneNumber, user.IsActive, user.IsEmailVerified, user.CreatedAt, user.UpdatedAt, user.LastLoginAt,
            profileDto, addressDtos);

        return Ok(new SingleUserResponse(true, userDetailDto, DateTime.UtcNow));
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUser(Guid userId, CancellationToken ct)
    {
        await _userRepository.DeleteAsync(userId, ct);
        return NoContent();
    }

    [HttpPost("{userId}/addresses")]
    public async Task<ActionResult<SingleAddressResponse>> CreateAddress(Guid userId, [FromBody] CreateAddressRequest request, CancellationToken ct)
    {
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = GetTenantId(),
            AddressType = request.AddressType,
            StreetAddress = request.StreetAddress,
            StreetAddress2 = request.StreetAddress2,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            State = request.State,
            RecipientName = request.RecipientName,
            PhoneNumber = request.PhoneNumber,
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _addressRepository.CreateAsync(address, ct);
        var dto = new AddressDto(created.Id, created.UserId, created.AddressType, created.StreetAddress,
            created.StreetAddress2, created.City, created.PostalCode, created.Country, created.State,
            created.RecipientName, created.PhoneNumber, created.IsDefault);

        return CreatedAtAction(nameof(GetUser), new { userId }, new SingleAddressResponse(true, dto, DateTime.UtcNow));
    }

    [HttpDelete("{userId}/addresses/{addressId}")]
    public async Task<ActionResult> DeleteAddress(Guid userId, Guid addressId, CancellationToken ct)
    {
        await _addressRepository.DeleteAsync(addressId, ct);
        return NoContent();
    }
}
