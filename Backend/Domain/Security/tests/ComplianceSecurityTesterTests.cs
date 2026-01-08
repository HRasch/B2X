using B2X.Security.ComplianceTesting;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Security.Tests.ComplianceTesting;

public class ComplianceSecurityTesterTests
{
    private readonly Mock<ISecurityScanner> _scannerMock;
    private readonly Mock<IVulnerabilityReporter> _reporterMock;
    private readonly Mock<IComplianceRuleEngine> _ruleEngineMock;
    private readonly ComplianceSecurityTester _sut;

    public ComplianceSecurityTesterTests()
    {
        _scannerMock = new Mock<ISecurityScanner>();
        _reporterMock = new Mock<IVulnerabilityReporter>();
        _ruleEngineMock = new Mock<IComplianceRuleEngine>();
        _sut = new ComplianceSecurityTester(_scannerMock.Object, _reporterMock.Object, _ruleEngineMock.Object);
    }

    #region RunSecurityTestsAsync Tests

    [Fact]
    public async Task RunSecurityTestsAsync_WithNoVulnerabilities_ShouldReturnSecure()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<Vulnerability>());

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<ComplianceIssue>());

        // Act
        var result = await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        result.ShouldNotBeNull();
        result.ArtifactId.ShouldBe(artifactId);
        result.IsSecure.ShouldBeTrue();
        result.Vulnerabilities.ShouldBeEmpty();
        result.ComplianceIssues.ShouldBeEmpty();
    }

    [Fact]
    public async Task RunSecurityTestsAsync_WithCriticalVulnerability_ShouldReturnNotSecure()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        var criticalVulnerability = new Vulnerability
        {
            Id = "vuln-001",
            Title = "SQL Injection",
            Description = "Critical SQL injection vulnerability",
            Severity = "Critical",
            CWE = "CWE-89",
            Location = "/api/users"
        };

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(new[] { criticalVulnerability });

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<ComplianceIssue>());

        // Act
        var result = await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        result.IsSecure.ShouldBeFalse();
        result.Vulnerabilities.Count().ShouldBe(1);
    }

    [Fact]
    public async Task RunSecurityTestsAsync_WithHighComplianceIssue_ShouldReturnNotSecure()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        var highComplianceIssue = new ComplianceIssue
        {
            RuleId = "GDPR-001",
            Description = "Missing data retention policy",
            Severity = "High",
            Recommendation = "Implement data retention policy"
        };

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<Vulnerability>());

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(new[] { highComplianceIssue });

        // Act
        var result = await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        result.IsSecure.ShouldBeFalse();
        result.ComplianceIssues.Count().ShouldBe(1);
    }

    [Fact]
    public async Task RunSecurityTestsAsync_WithMediumVulnerability_ShouldReturnSecure()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        var mediumVulnerability = new Vulnerability
        {
            Id = "vuln-002",
            Title = "Missing Rate Limiting",
            Description = "API endpoint lacks rate limiting",
            Severity = "Medium",
            CWE = "CWE-770",
            Location = "/api/search"
        };

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(new[] { mediumVulnerability });

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<ComplianceIssue>());

        // Act
        var result = await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        result.IsSecure.ShouldBeTrue();
        result.Vulnerabilities.Count().ShouldBe(1);
    }

    [Fact]
    public async Task RunSecurityTestsAsync_ShouldGenerateReport()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<Vulnerability>());

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<ComplianceIssue>());

        // Act
        await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        _reporterMock.Verify(
            r => r.GenerateReportAsync(It.Is<SecurityTestResult>(
                result => result.ArtifactId == artifactId)),
            Times.Once);
    }

    [Fact]
    public async Task RunSecurityTestsAsync_WithMultipleVulnerabilities_ShouldReturnAll()
    {
        // Arrange
        var artifactId = "artifact-001";
        var artifact = CreateTestArtifact();

        var vulnerabilities = new[]
        {
            new Vulnerability { Id = "vuln-001", Severity = "Low" },
            new Vulnerability { Id = "vuln-002", Severity = "Medium" },
            new Vulnerability { Id = "vuln-003", Severity = "High" }
        };

        _scannerMock
            .Setup(s => s.ScanForVulnerabilitiesAsync(artifact))
            .ReturnsAsync(vulnerabilities);

        _ruleEngineMock
            .Setup(r => r.CheckComplianceAsync(artifact))
            .ReturnsAsync(Enumerable.Empty<ComplianceIssue>());

        // Act
        var result = await _sut.RunSecurityTestsAsync(artifactId, artifact);

        // Assert
        result.Vulnerabilities.Count().ShouldBe(3);
        result.IsSecure.ShouldBeTrue(); // No critical vulnerabilities
    }

    #endregion

    #region GetTestHistoryAsync Tests

    [Fact]
    public async Task GetTestHistoryAsync_ShouldCallReporter()
    {
        // Arrange
        var artifactId = "artifact-001";
        var since = DateTime.UtcNow.AddDays(-7);
        var expectedHistory = new[]
        {
            new SecurityTestResult { ArtifactId = artifactId, IsSecure = true },
            new SecurityTestResult { ArtifactId = artifactId, IsSecure = false }
        };

        _reporterMock
            .Setup(r => r.GetHistoryAsync(artifactId, since))
            .ReturnsAsync(expectedHistory);

        // Act
        var result = await _sut.GetTestHistoryAsync(artifactId, since);

        // Assert
        result.Count().ShouldBe(2);
        _reporterMock.Verify(r => r.GetHistoryAsync(artifactId, since), Times.Once);
    }

    #endregion

    #region Model Tests

    [Fact]
    public void ComplianceArtifact_ShouldInitializeCorrectly()
    {
        // Act
        var artifact = new ComplianceArtifact
        {
            Id = "artifact-001",
            Type = "Code",
            Content = "public class Test {}",
            Metadata = new Dictionary<string, object>
            {
                { "language", "csharp" },
                { "version", "10.0" }
            }
        };

        // Assert
        artifact.Id.ShouldBe("artifact-001");
        artifact.Type.ShouldBe("Code");
        artifact.Metadata["language"].ShouldBe("csharp");
    }

    [Fact]
    public void Vulnerability_ShouldInitializeCorrectly()
    {
        // Act
        var vulnerability = new Vulnerability
        {
            Id = "CVE-2024-001",
            Title = "Buffer Overflow",
            Description = "Buffer overflow vulnerability in parser",
            Severity = "Critical",
            CWE = "CWE-120",
            Location = "/src/parser.cs"
        };

        // Assert
        vulnerability.Id.ShouldBe("CVE-2024-001");
        vulnerability.Severity.ShouldBe("Critical");
        vulnerability.CWE.ShouldBe("CWE-120");
    }

    [Fact]
    public void ComplianceIssue_ShouldInitializeCorrectly()
    {
        // Act
        var issue = new ComplianceIssue
        {
            RuleId = "PCI-DSS-3.2",
            Description = "Credit card data not encrypted",
            Severity = "High",
            Recommendation = "Implement AES-256 encryption for card data"
        };

        // Assert
        issue.RuleId.ShouldBe("PCI-DSS-3.2");
        issue.Severity.ShouldBe("High");
    }

    [Fact]
    public void SecurityTestResult_ShouldInitializeCorrectly()
    {
        // Act
        var result = new SecurityTestResult
        {
            ArtifactId = "artifact-001",
            IsSecure = true,
            Vulnerabilities = Enumerable.Empty<Vulnerability>(),
            ComplianceIssues = Enumerable.Empty<ComplianceIssue>(),
            Timestamp = DateTime.UtcNow,
            ReportUrl = "https://reports.example.com/001"
        };

        // Assert
        result.ArtifactId.ShouldBe("artifact-001");
        result.IsSecure.ShouldBeTrue();
        result.ReportUrl.ShouldBe("https://reports.example.com/001");
    }

    #endregion

    #region Helper Methods

    private static ComplianceArtifact CreateTestArtifact()
    {
        return new ComplianceArtifact
        {
            Id = "test-artifact",
            Type = "Code",
            Content = "public class TestClass { }",
            Metadata = new Dictionary<string, object>()
        };
    }

    #endregion
}
