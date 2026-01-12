namespace B2X.Types.DTOs;

/// <summary>
/// DTO for tenant creation/update requests
/// </summary>
public record TenantDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public string Status { get; init; } = "Active";

    /// <summary>
    /// Whether the store frontend is publicly accessible without authentication.
    /// When false, users must authenticate to access the store (closed shop / B2B).
    /// </summary>
    public bool IsPublicStore { get; init; } = true;
}

/// <summary>
/// DTO for user information
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Avatar { get; init; }
    public string Status { get; init; } = "Active";
    public DateTime LastLoginAt { get; init; }
    public bool EmailConfirmed { get; init; }
}

/// <summary>
/// DTO for authentication request
/// </summary>
public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public Guid? TenantId { get; init; }
}

/// <summary>
/// DTO for authentication response
/// </summary>
public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public required UserDto User { get; init; }
}

/// <summary>
/// DTO for API response envelope
/// </summary>
public record ApiResponse<T>
{
    public required T Data { get; init; }
    public bool Success { get; init; } = true;
    public string? Message { get; init; }
    public Dictionary<string, string>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// DTO for paginated results
/// </summary>
public record PaginatedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Paged result wrapper for queries
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
