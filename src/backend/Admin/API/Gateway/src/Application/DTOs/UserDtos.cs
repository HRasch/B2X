namespace B2X.Admin.Application.DTOs;

// AddressType Enum - represents different address purposes
public enum AddressType
{
    Shipping = 0,
    Billing = 1,
    Residential = 2,
    Commercial = 3,
    Other = 4
}

/// <summary>
/// User DTOs for the Admin Gateway
/// These DTOs are used when proxying requests to the Identity Service
/// </summary>

// Simple User DTO for list responses
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

// Address DTO
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

// Request DTOs for creating/updating users
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

// Pagination Info
public record PaginationInfo(
    int Page,
    int PageSize,
    int Total,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);

// API Response types
public record PaginatedUserResponse(
    bool Success,
    List<UserDto> Data,
    PaginationInfo Pagination,
    DateTime Timestamp);

public record SingleUserResponse(
    bool Success,
    UserDto Data,
    DateTime Timestamp);

public record CreateUserResponse(
    bool Success,
    UserDto Data,
    DateTime Timestamp);

public record AddressListResponse(
    bool Success,
    List<AddressDto> Data,
    DateTime Timestamp);

public record SingleAddressResponse(
    bool Success,
    AddressDto Data,
    DateTime Timestamp);
