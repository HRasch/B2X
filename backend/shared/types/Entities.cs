namespace B2Connect.Types.Domain;

/// <summary>
/// Base class for domain entities with common properties
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Represents a tenant in the multitenant system
/// </summary>
public class Tenant : Entity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

/// <summary>
/// Tenant status enumeration
/// </summary>
public enum TenantStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Archived = 3
}

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : Entity
{
    public Guid TenantId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
}

/// <summary>
/// User status enumeration
/// </summary>
public enum UserStatus
{
    Pending = 0,
    Active = 1,
    Inactive = 2,
    Suspended = 3
}

/// <summary>
/// Represents a user role within a tenant
/// </summary>
public class Role : Entity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<Permission> Permissions { get; set; } = [];
    public bool IsDefault { get; set; }
}

/// <summary>
/// Represents a permission that can be assigned to roles
/// </summary>
public class Permission : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
