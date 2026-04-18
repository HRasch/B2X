using B2X.AppHost.Configuration;
using Microsoft.Extensions.Configuration;

namespace B2X.AppHost.Extensions;

/// <summary>
/// Extensions for Testing Configuration binding and validation.
/// </summary>
public static class TestingConfigurationExtensions
{
    /// <summary>
    /// Binds and validates the Testing configuration section.
    /// Throws if configuration is invalid.
    /// </summary>
    public static TestingConfiguration GetTestingConfiguration(this IConfiguration configuration)
    {
        var testConfig = new TestingConfiguration();
        configuration.GetSection("Testing").Bind(testConfig);

        ValidateTestingConfiguration(testConfig);

        return testConfig;
    }

    /// <summary>
    /// Validates the Testing configuration for consistency and correctness.
    /// </summary>
    private static void ValidateTestingConfiguration(TestingConfiguration config)
    {
        // Validate Mode
        var validModes = new[] { "temporary", "persisted" };
        if (!validModes.Contains(config.Mode, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Invalid Testing:Mode '{config.Mode}'. Must be one of: {string.Join(", ", validModes)}");
        }

        // Validate Environment
        var validEnvironments = new[] { "development", "testing", "ci" };
        if (!validEnvironments.Contains(config.Environment, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Invalid Testing:Environment '{config.Environment}'. Must be one of: {string.Join(", ", validEnvironments)}");
        }

        // Validate SeedData
        if (config.SeedData.DefaultTenantCount < 0)
            throw new InvalidOperationException("Testing:SeedData:DefaultTenantCount must be >= 0");
        if (config.SeedData.UsersPerTenant < 0)
            throw new InvalidOperationException("Testing:SeedData:UsersPerTenant must be >= 0");
        if (config.SeedData.SampleProductCount < 0)
            throw new InvalidOperationException("Testing:SeedData:SampleProductCount must be >= 0");
    }

    /// <summary>
    /// Returns whether testing is configured for persistent storage.
    /// </summary>
    public static bool IsPersistenTestMode(this TestingConfiguration config)
    {
        return string.Equals(config.Mode, "persisted", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns whether testing is configured for temporary (in-memory) storage.
    /// </summary>
    public static bool IsTemporaryTestMode(this TestingConfiguration config)
    {
        return string.Equals(config.Mode, "temporary", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns whether we're in a seeding context (has test data available).
    /// </summary>
    public static bool ShouldSeedTestData(this TestingConfiguration config)
    {
        return config.SeedData.DefaultTenantCount > 0 || config.SeedOnStartup;
    }
}
