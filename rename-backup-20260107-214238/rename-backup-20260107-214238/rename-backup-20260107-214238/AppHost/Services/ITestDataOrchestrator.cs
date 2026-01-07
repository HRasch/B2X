namespace B2X.AppHost.Services;

/// <summary>
/// Orchestrates deterministic test data seeding across all services.
/// Ensures consistent initialization order and data availability for testing.
/// </summary>
public interface ITestDataOrchestrator
{
    /// <summary>
    /// Seeds all test data in the correct order.
    /// Order: Auth → Tenant → Localization → Catalog → CMS → Other services
    /// </summary>
    Task SeedAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds only core services required by most other services.
    /// Core: Auth → Tenant → Localization
    /// </summary>
    Task SeedCoreServicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds catalog-related data (products, categories, etc.).
    /// Depends on: Tenant, Localization
    /// </summary>
    Task SeedCatalogAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds CMS-related data (pages, templates, etc.).
    /// Depends on: Tenant, Localization
    /// </summary>
    Task SeedCmsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all test data and resets to initial state.
    /// Used between test runs for isolation.
    /// </summary>
    Task ResetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns seeding status information.
    /// </summary>
    Task<TestDataStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Status information about test data seeding.
/// </summary>
public class TestDataStatus
{
    /// <summary>
    /// Whether test data has been seeded.
    /// </summary>
    public bool IsSeeded { get; set; }

    /// <summary>
    /// Timestamp of last seeding operation.
    /// </summary>
    public DateTime? LastSeededAt { get; set; }

    /// <summary>
    /// Testing mode that was used for seeding.
    /// </summary>
    public string? SeededWith { get; set; }

    /// <summary>
    /// Number of tenants seeded.
    /// </summary>
    public int TenantCount { get; set; }

    /// <summary>
    /// Number of users seeded.
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Number of products seeded (if applicable).
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Detailed messages about seeding operations.
    /// </summary>
    public List<string> Messages { get; set; } = [];

    /// <summary>
    /// Any errors encountered during seeding.
    /// </summary>
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// Configuration for test data seeding orchestration.
/// </summary>
public class TestDataSeedingOptions
{
    /// <summary>
    /// Whether to automatically seed on startup.
    /// </summary>
    public bool SeedOnStartup { get; set; }

    /// <summary>
    /// Number of default tenants to create.
    /// </summary>
    public int DefaultTenantCount { get; set; } = 1;

    /// <summary>
    /// Number of users per tenant.
    /// </summary>
    public int UsersPerTenant { get; set; } = 5;

    /// <summary>
    /// Number of sample products.
    /// </summary>
    public int SampleProductCount { get; set; } = 50;

    /// <summary>
    /// Whether to include catalog demo data.
    /// </summary>
    public bool IncludeCatalogDemo { get; set; } = true;

    /// <summary>
    /// Whether to include CMS content.
    /// </summary>
    public bool IncludeCmsContent { get; set; } = false;

    /// <summary>
    /// Timeout for individual seeding operations.
    /// </summary>
    public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
