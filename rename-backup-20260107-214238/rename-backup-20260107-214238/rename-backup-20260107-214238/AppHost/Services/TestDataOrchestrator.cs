using B2X.AppHost.Configuration;
using B2X.AppHost.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace B2X.AppHost.Services;

/// <summary>
/// Orchestrates deterministic test data seeding across all B2X services.
/// Ensures proper initialization order and data consistency.
/// </summary>
public class TestDataOrchestrator : ITestDataOrchestrator
{
    private readonly ILogger<TestDataOrchestrator> _logger;
    private readonly TestingConfiguration _testingConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly TestDataSeedingOptions _seedingOptions;

    // HTTP clients for service communication
    private readonly HttpClient _authServiceClient;
    private readonly HttpClient _tenantServiceClient;
    private readonly HttpClient _localizationServiceClient;
    private readonly HttpClient _catalogServiceClient;
    private readonly HttpClient _cmsServiceClient;

    // Status tracking
    private readonly SeedingStatusTracker _statusTracker;

    public TestDataOrchestrator(
        ILogger<TestDataOrchestrator> logger,
        TestingConfiguration testingConfig,
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        SeedingStatusTracker statusTracker)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _testingConfig = testingConfig ?? throw new ArgumentNullException(nameof(testingConfig));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var httpFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _statusTracker = statusTracker ?? throw new ArgumentNullException(nameof(statusTracker));

        // Get HTTP clients for service communication
        _authServiceClient = httpFactory.CreateClient("auth-service");
        _tenantServiceClient = httpFactory.CreateClient("tenant-service");
        _localizationServiceClient = httpFactory.CreateClient("localization-service");
        _catalogServiceClient = httpFactory.CreateClient("catalog-service");
        _cmsServiceClient = httpFactory.CreateClient("cms-service");

        _seedingOptions = new TestDataSeedingOptions
        {
            SeedOnStartup = testingConfig.SeedOnStartup,
            DefaultTenantCount = testingConfig.SeedData.DefaultTenantCount,
            UsersPerTenant = testingConfig.SeedData.UsersPerTenant,
            SampleProductCount = testingConfig.SeedData.SampleProductCount,
            IncludeCatalogDemo = testingConfig.SeedData.IncludeCatalogDemo,
            IncludeCmsContent = testingConfig.SeedData.IncludeCmsContent
        };
    }

    /// <summary>
    /// Seeds all test data in the correct order with comprehensive error handling and rollback.
    /// Order: Auth → Tenant → Localization → Catalog → CMS
    /// </summary>
    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var seedingContext = new SeedingContext(_testingConfig, _logger);

        try
        {
            _logger.LogInformation("Starting test data seeding orchestration with error handling");

            // Record seeding start
            _statusTracker.RecordSeedingStart(_testingConfig);

            // Phase 1: Core Services (Auth, Tenant, Localization) - Critical path
            await SeedCoreServicesWithErrorHandlingAsync(seedingContext, cancellationToken);

            // Phase 2: Catalog Services - Can fail independently
            await SeedCatalogWithErrorHandlingAsync(seedingContext, cancellationToken);

            // Phase 3: CMS Services - Can fail independently
            await SeedCmsWithErrorHandlingAsync(seedingContext, cancellationToken);

            // Record successful completion
            var statistics = TestDataContext.Current.GetStatistics();
            _statusTracker.RecordSeedingComplete(statistics);

            _logger.LogInformation("Test data seeding completed successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (SeedingException ex)
        {
            _logger.LogError(ex, "Test data seeding failed with structured error: {ErrorCode}", ex.ErrorCode);

            // Perform selective rollback based on what was seeded
            await PerformSelectiveRollbackAsync(seedingContext, cancellationToken);

            // Record seeding failure with detailed information
            _statusTracker.RecordSeedingFailure(ex.Message, ex);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test data seeding failed with unexpected error");

            // Perform full rollback on unexpected errors
            await PerformFullRollbackAsync(seedingContext, cancellationToken);

            // Record seeding failure
            _statusTracker.RecordSeedingFailure(ex.Message, ex);

            throw new SeedingException("SEEDING_UNEXPECTED_ERROR", "Unexpected error during seeding", ex);
        }
        finally
        {
            stopwatch.Stop();
            seedingContext.Dispose();
        }
    }

    /// <summary>
    /// Seeds only core services required by most other services.
    /// Core: Auth → Tenant → Localization
    /// </summary>
    public async Task SeedCoreServicesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding core services (Auth, Tenant, Localization)");

        // 1. Seed Auth Service (users, roles)
        await SeedAuthServiceAsync(cancellationToken);

        // 2. Seed Tenant Service (tenants)
        await SeedTenantServiceAsync(cancellationToken);

        // 3. Seed Localization Service (languages, translations)
        await SeedLocalizationServiceAsync(cancellationToken);

        _logger.LogInformation("Core services seeding completed");
    }

    /// <summary>
    /// Seeds catalog-related data (products, categories, etc.).
    /// Depends on: Tenant, Localization
    /// </summary>
    public async Task SeedCatalogAsync(CancellationToken cancellationToken = default)
    {
        if (!_seedingOptions.IncludeCatalogDemo)
        {
            _logger.LogInformation("Catalog seeding skipped (IncludeCatalogDemo=false)");
            return;
        }

        _logger.LogInformation("Seeding catalog data ({ProductCount} products)", _seedingOptions.SampleProductCount);

        // Seed catalog service with demo data
        await SeedCatalogServiceAsync(cancellationToken);

        _logger.LogInformation("Catalog seeding completed");
    }

    /// <summary>
    /// Seeds CMS-related data (pages, templates, etc.).
    /// Depends on: Tenant, Localization
    /// </summary>
    public async Task SeedCmsAsync(CancellationToken cancellationToken = default)
    {
        if (!_seedingOptions.IncludeCmsContent)
        {
            _logger.LogInformation("CMS seeding skipped (IncludeCmsContent=false)");
            return;
        }

        _logger.LogInformation("Seeding CMS content");

        // Seed CMS service with demo pages/content
        await SeedCmsServiceAsync(cancellationToken);

        _logger.LogInformation("CMS seeding completed");
    }

    /// <summary>
    /// Clears all test data and resets to initial state.
    /// Used between test runs for isolation.
    /// </summary>
    public async Task ResetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Resetting all test data - this will clear all seeded data");

        // Reset in reverse order to avoid foreign key constraints
        await ResetCmsServiceAsync(cancellationToken);
        await ResetCatalogServiceAsync(cancellationToken);
        await ResetLocalizationServiceAsync(cancellationToken);
        await ResetTenantServiceAsync(cancellationToken);
        await ResetAuthServiceAsync(cancellationToken);

        _logger.LogInformation("Test data reset completed");
    }

    /// <summary>
    /// Returns seeding status information.
    /// </summary>
    public async Task<TestDataStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var status = new TestDataStatus
        {
            IsSeeded = false, // TODO: Implement persistence check
            LastSeededAt = null, // TODO: Implement timestamp tracking
            SeededWith = _testingConfig.Mode,
            TenantCount = _seedingOptions.DefaultTenantCount,
            UserCount = _seedingOptions.DefaultTenantCount * _seedingOptions.UsersPerTenant,
            ProductCount = _seedingOptions.IncludeCatalogDemo ? _seedingOptions.SampleProductCount : 0
        };

        // Get status from tracker
        var seedingSummary = _statusTracker.GetStatusSummary();
        if (seedingSummary.HasSeededData)
        {
            status.IsSeeded = true;
            status.LastSeededAt = seedingSummary.LastSeededAt;
            status.SeededWith = seedingSummary.Mode;
        }

        status.Messages.Add($"Configured for {_testingConfig.Mode} mode in {_testingConfig.Environment} environment");

        if (seedingSummary.IsCurrentlySeeding)
        {
            status.Messages.Add("Seeding is currently in progress");
        }
        else if (seedingSummary.HasSeededData)
        {
            status.Messages.Add($"Last seeded: {seedingSummary}");
        }
        else
        {
            status.Messages.Add("No seeding data found");
        }

        if (!string.IsNullOrEmpty(seedingSummary.LastError))
        {
            status.Errors.Add($"Last seeding error: {seedingSummary.LastError}");
        }

        status.Messages.Add($"Will seed {_seedingOptions.DefaultTenantCount} tenants with {_seedingOptions.UsersPerTenant} users each");

        if (_seedingOptions.IncludeCatalogDemo)
            status.Messages.Add($"Will seed {_seedingOptions.SampleProductCount} catalog products");

        if (_seedingOptions.IncludeCmsContent)
            status.Messages.Add("Will seed CMS content");

        return status;
    }

    #region Private Seeding Methods

    private async Task SeedAuthServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding Auth service with users for {TenantCount} tenants", _seedingOptions.DefaultTenantCount);
        await UserSeedingUtilities.SeedUsersAsync(_authServiceClient, _testingConfig, cancellationToken);
    }

    private async Task SeedTenantServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding Tenant service with {TenantCount} tenants", _seedingOptions.DefaultTenantCount);
        await TenantSeedingUtilities.SeedTenantsAsync(_tenantServiceClient, _testingConfig, cancellationToken);
    }

    private async Task SeedLocalizationServiceAsync(CancellationToken cancellationToken)
    {
        // TODO: Implement Localization service seeding via HTTP client
        _logger.LogInformation("Seeding Localization service");
        await Task.Delay(100, cancellationToken); // Placeholder
    }

    private async Task SeedCatalogServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding Catalog service with {ProductCount} products", _seedingOptions.SampleProductCount);
        await CatalogSeedingUtilities.SeedCatalogAsync(_catalogServiceClient, _testingConfig, cancellationToken);
    }

    private async Task SeedCmsServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding CMS service");
        await CmsSeedingUtilities.SeedCmsAsync(_cmsServiceClient, _testingConfig, cancellationToken);
    }

    #endregion

    #region Private Reset Methods

    private async Task ResetAuthServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting Auth service");
        await UserSeedingUtilities.ResetUsersAsync(_authServiceClient, cancellationToken);
    }

    private async Task ResetTenantServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting Tenant service");
        await TenantSeedingUtilities.ResetTenantsAsync(_tenantServiceClient, cancellationToken);
    }

    private async Task ResetLocalizationServiceAsync(CancellationToken cancellationToken)
    {
        // TODO: Implement Localization service reset
        _logger.LogInformation("Resetting Localization service");
        await Task.Delay(50, cancellationToken);
    }

    private async Task ResetCatalogServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting Catalog service");
        await CatalogSeedingUtilities.ResetCatalogAsync(_catalogServiceClient, cancellationToken);
    }

    private async Task ResetCmsServiceAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting CMS service");
        await CmsSeedingUtilities.ResetCmsAsync(_cmsServiceClient, cancellationToken);
    }

    #endregion

    private async Task TryRollbackAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogWarning("Attempting to rollback partial seeding");
            await ResetAllAsync(cancellationToken);
            _logger.LogInformation("Rollback completed successfully");
        }
        catch (Exception rollbackEx)
        {
            _logger.LogError(rollbackEx, "Rollback failed - manual cleanup may be required");
        }
    }

    #region Error Handling and Rollback Implementation (BE-002.8)

    /// <summary>
    /// Seeds core services with comprehensive error handling and rollback capabilities.
    /// </summary>
    private async Task SeedCoreServicesWithErrorHandlingAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        var phase = "CoreServices";

        try
        {
            context.StartPhase(phase);
            _logger.LogInformation("Starting core services seeding phase");

            // Auth service seeding (critical - no rollback possible if this fails)
            await SeedAuthWithRetryAsync(context, cancellationToken);
            context.MarkServiceSeeded("Auth");

            // Tenant service seeding (critical - depends on Auth)
            await SeedTenantWithRetryAsync(context, cancellationToken);
            context.MarkServiceSeeded("Tenant");

            // Localization service seeding (can be retried independently)
            await SeedLocalizationWithRetryAsync(context, cancellationToken);
            context.MarkServiceSeeded("Localization");

            context.CompletePhase(phase);
            _logger.LogInformation("Core services seeding phase completed successfully");
        }
        catch (Exception ex)
        {
            context.FailPhase(phase, ex);
            throw new SeedingException("CORE_SERVICES_FAILED", $"Core services seeding failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Seeds catalog services with error handling (can fail independently of core services).
    /// </summary>
    private async Task SeedCatalogWithErrorHandlingAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        var phase = "Catalog";

        try
        {
            context.StartPhase(phase);
            _logger.LogInformation("Starting catalog services seeding phase");

            await SeedCatalogWithRetryAsync(context, cancellationToken);
            context.MarkServiceSeeded("Catalog");

            context.CompletePhase(phase);
            _logger.LogInformation("Catalog services seeding phase completed successfully");
        }
        catch (Exception ex)
        {
            context.FailPhase(phase, ex);
            _logger.LogWarning(ex, "Catalog seeding failed, but core services remain intact");
            // Don't throw - catalog failure shouldn't prevent CMS seeding
        }
    }

    /// <summary>
    /// Seeds CMS services with error handling (can fail independently).
    /// </summary>
    private async Task SeedCmsWithErrorHandlingAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        var phase = "CMS";

        try
        {
            context.StartPhase(phase);
            _logger.LogInformation("Starting CMS services seeding phase");

            await SeedCmsWithRetryAsync(context, cancellationToken);
            context.MarkServiceSeeded("CMS");

            context.CompletePhase(phase);
            _logger.LogInformation("CMS services seeding phase completed successfully");
        }
        catch (Exception ex)
        {
            context.FailPhase(phase, ex);
            _logger.LogWarning(ex, "CMS seeding failed, but other services remain intact");
            // Don't throw - CMS failure is non-critical
        }
    }

    /// <summary>
    /// Seeds auth service with retry logic for transient failures.
    /// </summary>
    private async Task SeedAuthWithRetryAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            () => SeedAuthServiceAsync(cancellationToken),
            "Auth",
            maxRetries: 2,
            cancellationToken);
    }

    /// <summary>
    /// Seeds tenant service with retry logic.
    /// </summary>
    private async Task SeedTenantWithRetryAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            () => SeedTenantServiceAsync(cancellationToken),
            "Tenant",
            maxRetries: 2,
            cancellationToken);
    }

    /// <summary>
    /// Seeds localization service with retry logic.
    /// </summary>
    private async Task SeedLocalizationWithRetryAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            () => SeedLocalizationServiceAsync(cancellationToken),
            "Localization",
            maxRetries: 3,
            cancellationToken);
    }

    /// <summary>
    /// Seeds catalog service with retry logic.
    /// </summary>
    private async Task SeedCatalogWithRetryAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            () => SeedCatalogAsync(cancellationToken),
            "Catalog",
            maxRetries: 2,
            cancellationToken);
    }

    /// <summary>
    /// Seeds CMS service with retry logic.
    /// </summary>
    private async Task SeedCmsWithRetryAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            () => SeedCmsAsync(cancellationToken),
            "CMS",
            maxRetries: 2,
            cancellationToken);
    }

    /// <summary>
    /// Executes an operation with retry logic for transient failures.
    /// </summary>
    private async Task ExecuteWithRetryAsync(
        Func<Task> operation,
        string operationName,
        int maxRetries,
        CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (attempt <= maxRetries)
        {
            try
            {
                await operation();
                return; // Success
            }
            catch (HttpRequestException ex) when (IsTransientError(ex) && attempt < maxRetries)
            {
                attempt++;
                _logger.LogWarning(ex, "{Operation} failed with transient error (attempt {Attempt}/{MaxRetries}), retrying...",
                    operationName, attempt, maxRetries + 1);

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Operation} failed with non-transient error", operationName);
                throw;
            }
        }
    }

    /// <summary>
    /// Determines if an HTTP exception represents a transient error that should be retried.
    /// </summary>
    private bool IsTransientError(HttpRequestException ex)
    {
        // Consider 5xx errors and network timeouts as transient
        return ex.StatusCode.HasValue && (int)ex.StatusCode.Value >= 500;
    }

    /// <summary>
    /// Performs selective rollback based on what was successfully seeded.
    /// </summary>
    private async Task PerformSelectiveRollbackAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Performing selective rollback based on seeding progress");

        try
        {
            // Rollback in reverse order of seeding
            var seededServices = context.GetSeededServices();

            if (seededServices.Contains("CMS"))
            {
                await ResetCmsServiceAsync(cancellationToken);
                _logger.LogInformation("Rolled back CMS service");
            }

            if (seededServices.Contains("Catalog"))
            {
                await ResetCatalogServiceAsync(cancellationToken);
                _logger.LogInformation("Rolled back Catalog service");
            }

            if (seededServices.Contains("Localization"))
            {
                await ResetLocalizationServiceAsync(cancellationToken);
                _logger.LogInformation("Rolled back Localization service");
            }

            if (seededServices.Contains("Tenant"))
            {
                await ResetTenantServiceAsync(cancellationToken);
                _logger.LogInformation("Rolled back Tenant service");
            }

            if (seededServices.Contains("Auth"))
            {
                await ResetAuthServiceAsync(cancellationToken);
                _logger.LogInformation("Rolled back Auth service");
            }

            _logger.LogInformation("Selective rollback completed successfully");
        }
        catch (Exception rollbackEx)
        {
            _logger.LogError(rollbackEx, "Selective rollback failed - manual cleanup may be required");
            throw new SeedingException("ROLLBACK_FAILED", "Selective rollback failed", rollbackEx);
        }
    }

    /// <summary>
    /// Performs full rollback when unexpected errors occur.
    /// </summary>
    private async Task PerformFullRollbackAsync(SeedingContext context, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Performing full rollback due to unexpected error");

        try
        {
            await ResetAllAsync(cancellationToken);
            _logger.LogInformation("Full rollback completed successfully");
        }
        catch (Exception rollbackEx)
        {
            _logger.LogError(rollbackEx, "Full rollback failed - manual cleanup may be required");
            throw new SeedingException("FULL_ROLLBACK_FAILED", "Full rollback failed", rollbackEx);
        }
    }

    #endregion
}
