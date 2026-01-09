using B2X.AppHost.Configuration;

namespace B2X.AppHost.Validation;

/// <summary>
/// Validates testing configuration consistency and prevents invalid states.
/// </summary>
public static class TestingConfigurationValidator
{
    /// <summary>
    /// Validates the entire testing configuration for consistency.
    /// Returns a list of validation errors (empty if valid).
    /// </summary>
    public static ValidationResult Validate(TestingConfiguration config)
    {
        var errors = new List<string>();

        // Validate Mode
        ValidateMode(config.Mode, errors);

        // Validate Environment
        ValidateEnvironment(config.Environment, errors);

        // Validate SeedData
        ValidateSeedData(config.SeedData, errors);

        // Validate Security
        ValidateSecurity(config.Security, errors);

        // Validate consistency
        ValidateConsistency(config, errors);

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    /// <summary>
    /// Validates the Mode setting.
    /// </summary>
    private static void ValidateMode(string mode, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(mode))
        {
            errors.Add("Testing:Mode cannot be empty");
            return;
        }

        var validModes = new[] { "temporary", "persisted" };
        if (!validModes.Contains(mode, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add($"Testing:Mode '{mode}' is invalid. Must be one of: {string.Join(", ", validModes)}");
        }
    }

    /// <summary>
    /// Validates the Environment setting.
    /// </summary>
    private static void ValidateEnvironment(string environment, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(environment))
        {
            errors.Add("Testing:Environment cannot be empty");
            return;
        }

        var validEnvironments = new[] { "development", "testing", "ci" };
        if (!validEnvironments.Contains(environment, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add($"Testing:Environment '{environment}' is invalid. Must be one of: {string.Join(", ", validEnvironments)}");
        }
    }

    /// <summary>
    /// Validates SeedData configuration with comprehensive checks.
    /// </summary>
    private static void ValidateSeedData(SeedDataOptions seedData, List<string> errors)
    {
        // Basic range validation
        if (seedData.DefaultTenantCount < 0)
            errors.Add("Testing:SeedData:DefaultTenantCount must be >= 0");

        if (seedData.DefaultTenantCount > 100)
            errors.Add("Warning: Testing:SeedData:DefaultTenantCount > 100 may impact performance");

        if (seedData.UsersPerTenant < 0)
            errors.Add("Testing:SeedData:UsersPerTenant must be >= 0");

        if (seedData.UsersPerTenant > 1000)
            errors.Add("Warning: Testing:SeedData:UsersPerTenant > 1000 may impact performance");

        if (seedData.SampleProductCount < 0)
            errors.Add("Testing:SeedData:SampleProductCount must be >= 0");

        if (seedData.SampleProductCount > 10000)
            errors.Add("Warning: Testing:SeedData:SampleProductCount > 10000 may impact performance");

        // CMS-specific validation
        if (seedData.IncludeCmsDemo)
        {
            if (seedData.SamplePageCount < 0)
                errors.Add("Testing:SeedData:SamplePageCount must be >= 0 when IncludeCmsDemo is true");

            if (seedData.SamplePageCount > 500)
                errors.Add("Warning: Testing:SeedData:SamplePageCount > 500 may impact seeding performance");

            if (seedData.SampleTemplateCount < 0)
                errors.Add("Testing:SeedData:SampleTemplateCount must be >= 0 when IncludeCmsDemo is true");

            if (seedData.SampleTemplateCount > 100)
                errors.Add("Warning: Testing:SeedData:SampleTemplateCount > 100 may impact seeding performance");

            if (seedData.SampleTemplateCount == 0 && seedData.SamplePageCount > 0)
                errors.Add("Warning: SampleTemplateCount is 0 but SamplePageCount > 0. Pages may not render properly without templates.");
        }

        // Cross-field validation
        var totalUsers = seedData.DefaultTenantCount * seedData.UsersPerTenant;
        if (totalUsers > 10000)
            errors.Add("Warning: Total users (tenants × users/tenant) > 10000 may impact performance");

        if (seedData.DefaultTenantCount > 0 && seedData.UsersPerTenant == 0)
            errors.Add("Warning: DefaultTenantCount > 0 but UsersPerTenant = 0. Tenants will have no users.");
    }

    /// <summary>
    /// Validates Security configuration.
    /// </summary>
    private static void ValidateSecurity(TestSecurityOptions security, List<string> errors)
    {
        // Security settings are all boolean, so they're inherently valid
        // Add future validation rules here as needed
    }

    /// <summary>
    /// Validates consistency across configuration sections with comprehensive checks.
    /// </summary>
    private static void ValidateConsistency(TestingConfiguration config, List<string> errors)
    {
        // Mode and environment consistency
        if (config.IsTemporaryTestMode && string.Equals(config.Environment, "ci", StringComparison.Ordinal))
        {
            errors.Add("Warning: Using temporary mode in CI environment. Consider using persisted mode for reliable CI testing.");
        }

        // Seeding configuration consistency
        if (config.SeedOnStartup)
        {
            if (config.SeedData.DefaultTenantCount == 0)
            {
                errors.Add("SeedOnStartup is true but DefaultTenantCount is 0. No data will be seeded.");
            }

            if (config.IsTemporaryTestMode)
            {
                errors.Add("Warning: SeedOnStartup with temporary mode. Data will be lost on application restart.");
            }
        }

        // Performance validations
        if (config.IsPersistentTestMode && string.Equals(config.Environment, "ci", StringComparison.Ordinal))
        {
            errors.Add("Warning: Using persistent mode in CI. Ensure database cleanup between test runs.");
        }

        // CMS seeding consistency
        if (config.SeedData.IncludeCmsDemo && !config.SeedData.IncludeCmsContent)
        {
            errors.Add("Warning: IncludeCmsDemo is true but IncludeCmsContent is false. CMS demo may not work properly.");
        }

        // Catalog seeding consistency
        if (config.SeedData.IncludeCatalogDemo && config.SeedData.SampleProductCount == 0)
        {
            errors.Add("Warning: IncludeCatalogDemo is true but SampleProductCount is 0. Catalog will be empty.");
        }

        // Environment-specific validations
        ValidateEnvironmentSpecificRules(config, errors);

        // Resource usage estimation
        ValidateResourceUsage(config, errors);
    }

    /// <summary>
    /// Validates environment-specific configuration rules.
    /// </summary>
    private static void ValidateEnvironmentSpecificRules(TestingConfiguration config, List<string> errors)
    {
        switch (config.Environment.ToLowerInvariant())
        {
            case "development":
                // Development allows more flexibility
                if (config.SeedData.DefaultTenantCount > 50)
                    errors.Add("Warning: High tenant count in development may slow startup.");
                break;

            case "testing":
                // Testing should be balanced
                if (config.SeedData.DefaultTenantCount > 20)
                    errors.Add("Warning: High tenant count in testing environment may slow test execution.");
                if (!config.SeedOnStartup)
                    errors.Add("Warning: SeedOnStartup is false in testing. Tests may fail due to missing data.");
                break;

            case "ci":
                // CI should be optimized for speed
                if (config.IsPersistentTestMode && config.SeedData.DefaultTenantCount > 5)
                    errors.Add("Warning: High tenant count in CI with persistent mode may slow pipeline.");
                if (config.SeedOnStartup && config.SeedData.DefaultTenantCount == 0)
                    errors.Add("CI environment with SeedOnStartup=true but no tenants will cause test failures.");
                break;
        }
    }

    /// <summary>
    /// Validates estimated resource usage and provides performance warnings.
    /// </summary>
    private static void ValidateResourceUsage(TestingConfiguration config, List<string> errors)
    {
        var estimatedUsers = config.SeedData.DefaultTenantCount * config.SeedData.UsersPerTenant;
        var estimatedProducts = config.SeedData.IncludeCatalogDemo ? config.SeedData.SampleProductCount : 0;
        var estimatedPages = config.SeedData.IncludeCmsDemo ? config.SeedData.SamplePageCount : 0;

        // Memory usage estimation (rough)
        var estimatedMemoryMB = (estimatedUsers * 0.1) + (estimatedProducts * 0.05) + (estimatedPages * 0.2);

        if (estimatedMemoryMB > 500 && config.IsTemporaryTestMode)
            errors.Add($"Warning: Estimated memory usage ~{estimatedMemoryMB:F0}MB in temporary mode. Consider reducing data volume.");

        // Database size estimation for persistent mode
        if (config.IsPersistentTestMode)
        {
            var estimatedDbMB = (estimatedUsers * 0.5) + (estimatedProducts * 0.2) + (estimatedPages * 0.3);
            if (estimatedDbMB > 1000)
                errors.Add($"Warning: Estimated database size ~{estimatedDbMB:F0}MB. Ensure adequate storage in test environment.");
        }

        // Startup time estimation
        var estimatedStartupSeconds = (estimatedUsers * 0.01) + (estimatedProducts * 0.005) + (estimatedPages * 0.02);
        if (estimatedStartupSeconds > 30)
            errors.Add($"Warning: Estimated startup time ~{estimatedStartupSeconds:F0}s. Consider reducing data volume for faster development cycles.");
    }
}

/// <summary>
/// Result of configuration validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the configuration is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors and warnings.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Throws InvalidOperationException if validation failed with errors.
    /// Warnings are not thrown (only logged).
    /// </summary>
    public void ThrowIfInvalid()
    {
        var criticalErrors = Errors
            .Where(e => !e.StartsWith("Warning:", StringComparison.Ordinal))
            .ToList();

        if (criticalErrors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Testing configuration validation failed:\n{string.Join("\n", criticalErrors)}");
        }
    }

    /// <summary>
    /// Returns all validation messages as a formatted string.
    /// </summary>
    public override string ToString()
    {
        if (IsValid)
            return "Configuration is valid";

        return $"Configuration has {Errors.Count} issue(s):\n{string.Join("\n", Errors)}";
    }
}
