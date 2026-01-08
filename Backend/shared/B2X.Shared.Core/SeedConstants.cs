namespace B2X.Shared.Core;

/// <summary>
/// Constants used for seeding demo/development data.
/// These values ensure consistency across all services for InMemory and demo modes.
/// </summary>
public static class SeedConstants
{
    /// <summary>
    /// Default tenant ID for demo data - matches frontend's VITE_DEFAULT_TENANT_ID.
    /// Use this for all seeded data to ensure visibility in Store and Admin frontends.
    /// </summary>
    public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Default tenant name for demo data.
    /// </summary>
    public const string DefaultTenantName = "Demo Tenant";

    /// <summary>
    /// Default tenant slug for demo data.
    /// </summary>
    public const string DefaultTenantSlug = "demo";

    /// <summary>
    /// Default admin user ID for demo data.
    /// </summary>
    public static readonly Guid DefaultAdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000100");

    /// <summary>
    /// Default admin email for demo data.
    /// </summary>
    public const string DefaultAdminEmail = "admin@example.com";

    /// <summary>
    /// Default domain for the demo tenant.
    /// </summary>
    public const string DefaultTenantDomain = "localhost";
}
