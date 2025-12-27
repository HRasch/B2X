using B2Connect.Shared.User.Models.Enums;

namespace B2Connect.Admin.Application.DTOs;

// User DTOs
public record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record UserDetailDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastLoginAt,
    UserProfileDto? Profile,
    List<AddressDto> Addresses);

// User Profile DTOs
public record UserProfileDto(
    Guid Id,
    Guid UserId,
    string? AvatarUrl,
    string? Bio,
    DateTime? DateOfBirth,
    string? CompanyName,
    string? JobTitle,
    string? PreferredLanguage,
    string? Timezone,
    bool ReceiveNewsletter,
    bool ReceivePromotionalEmails);

// Address DTOs
public record AddressDto(
    Guid Id,
    Guid UserId,
    AddressType AddressType,
    string StreetAddress,
    string? StreetAddress2,
    string City,
    string PostalCode,
    string Country,
    string? State,
    string RecipientName,
    string? PhoneNumber,
    bool IsDefault);

// Request DTOs
public record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber);

public record UpdateUserRequest(
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    bool? IsActive,
    bool? IsEmailVerified);

public record CreateAddressRequest(
    AddressType AddressType,
    string StreetAddress,
    string? StreetAddress2,
    string City,
    string PostalCode,
    string Country,
    string? State,
    string RecipientName,
    string? PhoneNumber,
    bool IsDefault);

public record UpdateAddressRequest(
    AddressType? AddressType,
    string? StreetAddress,
    string? StreetAddress2,
    string? City,
    string? PostalCode,
    string? Country,
    string? State,
    string? RecipientName,
    string? PhoneNumber,
    bool? IsDefault);

public record ResetPasswordRequest(
    string NewPassword);

public record VerifyEmailRequest(
    string Code);

// Response DTOs
public record PaginatedUserResponse(
    bool Success,
    List<UserDto> Data,
    PaginationInfo Pagination,
    DateTime Timestamp);

public record SingleUserResponse(
    bool Success,
    UserDetailDto Data,
    DateTime Timestamp);

public record CreateUserResponse(
    bool Success,
    UserDetailDto Data,
    DateTime Timestamp);

public record AddressListResponse(
    bool Success,
    List<AddressDto> Data,
    DateTime Timestamp);

public record SingleAddressResponse(
    bool Success,
    AddressDto Data,
    DateTime Timestamp);

public record PaginationInfo(
    int Page,
    int PageSize,
    int Total,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
