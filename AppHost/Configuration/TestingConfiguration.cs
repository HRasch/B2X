namespace B2Connect.AppHost.Configuration;

/// <summary>
/// Configuration for test environment and storage mode selection.
/// Enables both persistent (PostgreSQL) and temporary (in-memory) test infrastructure.
/// </summary>
public class TestingConfiguration
{
    /// <summary>
    /// The mode for test data storage: "persisted" (PostgreSQL) or "temporary" (in-memory).
    /// Default: "temporary" (in-memory for faster unit tests).
    /// </summary>
    public string Mode { get; set; } = "temporary";

    /// <summary>
    /// The testing environment context: "development", "testing", or "ci".
    /// Used to determine which test data seeding profile to use.
    /// Default: "development"
    /// </summary>
    public string Environment { get; set; } = "development";

    /// <summary>
    /// Whether to seed test data automatically on startup.
    /// Useful for demo/testing environments; disabled in CI to speed up tests.
    /// Default: false (explicit seeding preferred in unit tests)
    /// </summary>
    public bool SeedOnStartup { get; set; } = false;

    /// <summary>
    /// Seed data configuration options.
    /// </summary>
    public SeedDataOptions SeedData { get; set; } = new();

    /// <summary>
    /// Security options for test environments.
    /// </summary>
    public TestSecurityOptions Security { get; set; } = new();

    /// <summary>
    /// Gets whether the current configuration is set to temporary test mode (in-memory).
    /// </summary>
    public bool IsTemporaryTestMode => Mode.Equals("temporary", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether the current configuration is set to persistent test mode (PostgreSQL).
    /// </summary>
    public bool IsPersistentTestMode => Mode.Equals("persisted", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Configuration for test data seeding behavior.
/// </summary>
public class SeedDataOptions
{
    /// <summary>
    /// Number of default tenants to create during seeding.
    /// Default: 1
    /// </summary>
    public int DefaultTenantCount { get; set; } = 1;

    /// <summary>
    /// Number of test users to create per tenant.
    /// Default: 5
    /// </summary>
    public int UsersPerTenant { get; set; } = 5;

    /// <summary>
    /// Number of sample products to seed in catalog.
    /// Default: 50
    /// </summary>
    public int SampleProductCount { get; set; } = 50;

    /// <summary>
    /// Whether to populate catalog with full demo data.
    /// Default: true
    /// </summary>
    public bool IncludeCatalogDemo { get; set; } = true;

    /// <summary>
    /// Whether to populate CMS pages with sample content.
    /// Default: false
    /// </summary>
    public bool IncludeCmsContent { get; set; } = false;

    /// <summary>
    /// Whether to populate CMS with full demo data including pages and templates.
    /// Default: false
    /// </summary>
    public bool IncludeCmsDemo { get; set; } = false;

    /// <summary>
    /// Number of sample pages to create during CMS seeding.
    /// Default: 10
    /// </summary>
    public int SamplePageCount { get; set; } = 10;

    /// <summary>
    /// Number of sample templates to create during CMS seeding.
    /// Default: 5
    /// </summary>
    public int SampleTemplateCount { get; set; } = 5;
}

/// <summary>
/// Security configuration for test environments.
/// </summary>
public class TestSecurityOptions
{
    /// <summary>
    /// Whether to enforce environment gating for test endpoints.
    /// When true, test endpoints will return 404 in production.
    /// Default: true
    /// </summary>
    public bool EnforceEnvironmentGating { get; set; } = true;

    /// <summary>
    /// Whether to enable audit logging for all test data operations.
    /// Default: true
    /// </summary>
    public bool EnableAuditLogging { get; set; } = true;

    /// <summary>
    /// Whether to enforce RBAC for test tenant management endpoints.
    /// Default: true (requires SuperAdmin role)
    /// </summary>
    public bool EnforceRoleBasedAccess { get; set; } = true;

    /// <summary>
    /// Whether to allow cross-tenant data access in test endpoints.
    /// Default: false (strict tenant isolation)
    /// </summary>
    public bool AllowCrossTenantAccess { get; set; } = false;
}
