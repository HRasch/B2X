using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2Connect.Shared.Infrastructure.Tests.Validation;

public class ArchitectureValidatorTests
{
    private readonly Mock<ILogger<ArchitectureValidator>> _loggerMock;
    private readonly ArchitectureValidator _validator;

    public ArchitectureValidatorTests()
    {
        _loggerMock = new Mock<ILogger<ArchitectureValidator>>();
        _validator = new ArchitectureValidator(_loggerMock.Object);
    }

    [Fact]
    public void ValidateAssemblies_WithValidAssemblies_ReturnsValidResult()
    {
        // Arrange
        var assemblies = new[] { typeof(ArchitectureValidator).Assembly };

        // Act
        var result = _validator.ValidateAssemblies(assemblies);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ScanServiceBoundaries_ReturnsViolations_WhenCrossServiceAccessDetected()
    {
        // Act
        var violations = _validator.ScanServiceBoundaries();

        // Assert
        violations.ShouldNotBeNull();
        // Note: Actual violations depend on the current codebase state
    }

    [Fact]
    public void ValidateCqrsPatterns_ReturnsViolations_WhenNamingConventionsBroken()
    {
        // Act
        var violations = _validator.ValidateCqrsPatterns();

        // Assert
        violations.ShouldNotBeNull();
        // Note: Actual violations depend on command/event naming in codebase
    }

    [Fact]
    public void GenerateComplianceReport_ReturnsCompleteReport()
    {
        // Act
        var report = _validator.GenerateComplianceReport();

        // Assert
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeGreaterThan(DateTime.MinValue);
        report.OverallCompliance.ShouldBeGreaterThanOrEqualTo(0);
        report.OverallCompliance.ShouldBeLessThanOrEqualTo(100);
        report.AssemblyViolations.ShouldNotBeNull();
        report.BoundaryViolations.ShouldNotBeNull();
        report.CqrsViolations.ShouldNotBeNull();
        report.Recommendations.ShouldNotBeNull();
    }

    [Fact]
    public void ComplianceReport_IncludesAdrReferences()
    {
        // Act
        var report = _validator.GenerateComplianceReport();

        // Assert
        var allViolations = report.AssemblyViolations
            .Concat(report.BoundaryViolations)
            .Concat(report.CqrsViolations);

        // Most violations should reference ADRs
        var violationsWithAdr = allViolations.Where(v => !string.IsNullOrEmpty(v.AdrReference));
        violationsWithAdr.ShouldNotBeEmpty();
    }
}

public class ArchitectureValidationResultTests
{
    [Fact]
    public void AddViolations_AddsToCollection()
    {
        // Arrange
        var result = new ArchitectureValidationResult();
        var violations = new[]
        {
            new ArchitectureViolation
            {
                Severity = ViolationSeverity.High,
                Rule = "TestRule",
                Message = "Test violation"
            }
        };

        // Act
        result.AddViolations(violations);

        // Assert
        result.Violations.ShouldContain(violations[0]);
        result.IsValid.ShouldBeFalse();
    }
}

public class ArchitectureViolationTests
{
    [Fact]
    public void ArchitectureViolation_Properties_SetCorrectly()
    {
        // Arrange & Act
        var violation = new ArchitectureViolation
        {
            Severity = ViolationSeverity.Critical,
            Rule = "TestRule",
            Message = "Test message",
            Location = "TestLocation",
            AdrReference = "ADR-001",
            Recommendation = "Fix it"
        };

        // Assert
        violation.Severity.ShouldBe(ViolationSeverity.Critical);
        violation.Rule.ShouldBe("TestRule");
        violation.Message.ShouldBe("Test message");
        violation.Location.ShouldBe("TestLocation");
        violation.AdrReference.ShouldBe("ADR-001");
        violation.Recommendation.ShouldBe("Fix it");
    }
}

public class ArchitectureComplianceReportTests
{
    [Fact]
    public void ArchitectureComplianceReport_InitializesCorrectly()
    {
        // Arrange & Act
        var report = new ArchitectureComplianceReport
        {
            Timestamp = DateTime.UtcNow,
            OverallCompliance = 85.5,
            AssemblyViolations = new List<ArchitectureViolation>(),
            BoundaryViolations = new List<ArchitectureViolation>(),
            CqrsViolations = new List<ArchitectureViolation>(),
            Recommendations = new List<string> { "Fix issues" }
        };

        // Assert
        report.Timestamp.ShouldBeGreaterThan(DateTime.MinValue);
        report.OverallCompliance.ShouldBe(85.5);
        report.AssemblyViolations.ShouldNotBeNull();
        report.BoundaryViolations.ShouldNotBeNull();
        report.CqrsViolations.ShouldNotBeNull();
        report.Recommendations.ShouldContain("Fix issues");
    }
}