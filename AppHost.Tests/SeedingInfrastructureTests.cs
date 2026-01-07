using B2Connect.AppHost.Configuration;
using B2Connect.AppHost.Services;
using B2Connect.AppHost.Validation;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2Connect.AppHost.Tests.Services;

/// <summary>
/// Unit tests for seeding infrastructure components.
/// </summary>
public class SeedingInfrastructureTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void SeedingContext_ShouldTrackSeedingPhases()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions(),
            Security = new TestSecurityOptions()
        };
        var logger = new Mock<ILogger>().Object;
        var context = new SeedingContext(config, logger);

        // Act
        context.StartPhase("Auth");
        context.MarkServiceSeeded("auth-service");
        context.CompletePhase("Auth");

        // Assert
        context.GetCompletedPhases().ShouldContain("Auth");
        context.GetSeededServices().ShouldContain("auth-service");
    }

    [Fact]
    public void SeedingContext_ShouldHandlePhaseFailures()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions(),
            Security = new TestSecurityOptions()
        };
        var logger = new Mock<ILogger>().Object;
        var context = new SeedingContext(config, logger);
        var exception = new Exception("Test failure");

        // Act
        context.StartPhase("Tenant");
        context.FailPhase("Tenant", exception);

        // Assert
        context.GetFailedPhases().ShouldContain("Tenant");
        context.GetPhaseErrors("Tenant").ShouldContain(exception);
    }

    [Fact]
    public void SeedingException_ShouldProvideStructuredErrorInformation()
    {
        // Arrange
        var errorCode = "SERVICE_FAILURE";
        var failedPhase = "Catalog";
        var context = new Dictionary<string, object> { ["ServiceName"] = "catalog-service" };

        // Act
        var exception = SeedingException.ServiceFailure(failedPhase, "catalog-service");

        // Assert
        exception.ErrorCode.ShouldBe(errorCode);
        exception.FailedPhase.ShouldBe(failedPhase);
        exception.Message.ShouldContain("Service 'catalog-service' failed");
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateMode()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "invalid",
            Environment = "development",
            SeedData = new SeedDataOptions(),
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Contains("Mode 'invalid' is invalid"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateEnvironment()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "invalid",
            SeedData = new SeedDataOptions(),
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Contains("Environment 'invalid' is invalid"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateSeedDataRanges()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions
            {
                DefaultTenantCount = -1,
                UsersPerTenant = -5,
                SampleProductCount = 15000 // Over limit
            },
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Contains("DefaultTenantCount must be >= 0"));
        result.Errors.ShouldContain(e => e.Contains("UsersPerTenant must be >= 0"));
        result.Errors.ShouldContain(e => e.Contains("SampleProductCount > 10000 may impact performance"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateCmsConsistency()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions
            {
                IncludeCmsDemo = true,
                SamplePageCount = 10,
                SampleTemplateCount = 0 // Inconsistent
            },
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Contains("SampleTemplateCount is 0 but SamplePageCount > 0"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldProvidePerformanceWarnings()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions
            {
                DefaultTenantCount = 100, // High count
                UsersPerTenant = 100,
                IncludeCmsDemo = true,
                SamplePageCount = 200
            },
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeTrue(); // Warnings don't make it invalid
        result.Errors.ShouldContain(e => e.Contains("High tenant count"));
        result.Errors.ShouldContain(e => e.Contains("Estimated memory usage"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateEnvironmentSpecificRules()
    {
        // Arrange - CI environment with high tenant count
        var config = new TestingConfiguration
        {
            Mode = "persisted",
            Environment = "ci",
            SeedData = new SeedDataOptions
            {
                DefaultTenantCount = 10 // High for CI
            },
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldContain(e => e.Contains("High tenant count in CI"));
    }

    [Fact]
    public void TestingConfigurationValidator_ShouldValidateConsistencyRules()
    {
        // Arrange - Temporary mode with SeedOnStartup
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedOnStartup = true,
            SeedData = new SeedDataOptions
            {
                DefaultTenantCount = 0 // No tenants but seeding enabled
            },
            Security = new TestSecurityOptions()
        };

        // Act
        var result = TestingConfigurationValidator.Validate(config);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Contains("SeedOnStartup is true but DefaultTenantCount is 0"));
        result.Errors.ShouldContain(e => e.Contains("Data will be lost on application restart"));
    }

    [Fact]
    public void ValidationResult_ShouldThrowOnCriticalErrors()
    {
        // Arrange
        var result = new B2Connect.AppHost.Validation.ValidationResult
        {
            IsValid = false,
            Errors = ["Critical error", "Warning: This is just a warning"]
        };

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => result.ThrowIfInvalid());
        exception.Message.ShouldContain("Critical error");
        exception.Message.ShouldNotContain("Warning");
    }

    [Fact]
    public void ValidationResult_ShouldNotThrowOnWarningsOnly()
    {
        // Arrange
        var result = new B2Connect.AppHost.Validation.ValidationResult
        {
            IsValid = true,
            Errors = ["Warning: This is just a warning"]
        };

        // Act & Assert
        Should.NotThrow(() => result.ThrowIfInvalid());
    }

    [Fact]
    public void SeedingContext_ShouldSupportSelectiveRollback()
    {
        // Arrange
        var config = new TestingConfiguration
        {
            Mode = "temporary",
            Environment = "development",
            SeedData = new SeedDataOptions(),
            Security = new TestSecurityOptions()
        };
        var logger = new Mock<ILogger>().Object;
        var context = new SeedingContext(config, logger);

        // Act
        context.StartPhase("Auth");
        context.MarkServiceSeeded("auth-service");
        context.CompletePhase("Auth");

        context.StartPhase("Tenant");
        context.MarkServiceSeeded("tenant-service");
        context.FailPhase("Tenant", new Exception("Test failure"));

        // Assert
        context.GetSeededServices("Auth").ShouldContain("auth-service");
        context.GetSeededServices("Tenant").ShouldContain("tenant-service");
        context.GetCompletedPhases().ShouldContain("Auth");
        context.GetFailedPhases().ShouldContain("Tenant");
    }

    [Fact]
    public void SeedingException_ShouldSupportDifferentErrorTypes()
    {
        // Test ServiceFailure
        var serviceFailure = SeedingException.ServiceFailure("Catalog", "catalog-service");
        serviceFailure.ErrorCode.ShouldBe("SERVICE_FAILURE");

        // Test NetworkFailure
        var networkFailure = SeedingException.NetworkFailure("Localization", "timeout");
        networkFailure.ErrorCode.ShouldBe("NETWORK_FAILURE");

        // Test ValidationFailure
        var validationFailure = SeedingException.ValidationFailure("CMS", "invalid data");
        validationFailure.ErrorCode.ShouldBe("VALIDATION_FAILURE");

        // Test TimeoutFailure
        var timeoutFailure = SeedingException.TimeoutFailure("User", 30);
        timeoutFailure.ErrorCode.ShouldBe("TIMEOUT_FAILURE");

        // Test ConfigurationError
        var configFailure = SeedingException.ConfigurationError("invalid setting");
        configFailure.ErrorCode.ShouldBe("CONFIGURATION_ERROR");
    }
}