using B2X.Compliance.Jurisdictions;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Compliance.Tests.Jurisdictions;

public class JurisdictionComplianceEngineTests
{
    private readonly Mock<IJurisdictionRepository> _repoMock;
    private readonly Mock<IRegulatoryUpdateService> _regulatoryServiceMock;
    private readonly JurisdictionComplianceEngine _sut;

    public JurisdictionComplianceEngineTests()
    {
        _repoMock = new Mock<IJurisdictionRepository>();
        _regulatoryServiceMock = new Mock<IRegulatoryUpdateService>();

        // Setup mock to return test jurisdictions
        _repoMock
            .Setup(r => r.GetSupportedJurisdictionsAsync())
            .ReturnsAsync(new[] { "BR", "MX", "IN", "ZA", "TR", "US", "CA", "GB", "DE", "FR", "JP", "AU", "CN", "KR", "SG", "AR", "CO", "CL", "PE", "VE", "EC", "NG", "KE", "GH", "TZ", "UG", "RW" });

        _sut = new JurisdictionComplianceEngine(_repoMock.Object, _regulatoryServiceMock.Object);
    }

    #region GetSupportedJurisdictions Tests

    [Fact]
    public async Task GetSupportedJurisdictions_ShouldReturn15PlusJurisdictions()
    {
        // Act
        var jurisdictions = (await _sut.GetSupportedJurisdictions()).ToList();

        // Assert
        jurisdictions.Count.ShouldBeGreaterThanOrEqualTo(15);
    }

    [Theory]
    [InlineData("BR")]
    [InlineData("MX")]
    [InlineData("IN")]
    [InlineData("ZA")]
    [InlineData("TR")]
    public async Task GetSupportedJurisdictions_ShouldIncludeEmergingMarkets(string code)
    {
        // Act
        var jurisdictions = await _sut.GetSupportedJurisdictions();

        // Assert
        jurisdictions.ShouldContain(code);
    }

    [Fact]
    public async Task GetSupportedJurisdictions_ShouldIncludeLatinAmericaRegion()
    {
        // Arrange
        var latinAmericaCodes = new[] { "BR", "MX", "AR", "CO", "CL", "PE", "VE", "EC" };

        // Act
        var jurisdictions = (await _sut.GetSupportedJurisdictions()).ToList();

        // Assert
        foreach (var code in latinAmericaCodes)
        {
            jurisdictions.ShouldContain(code);
        }
    }

    [Fact]
    public async Task GetSupportedJurisdictions_ShouldIncludeAfricaRegion()
    {
        // Arrange
        var africaCodes = new[] { "ZA", "NG", "KE", "GH", "TZ", "UG", "RW" };

        // Act
        var jurisdictions = (await _sut.GetSupportedJurisdictions()).ToList();

        // Assert
        foreach (var code in africaCodes)
        {
            jurisdictions.ShouldContain(code);
        }
    }

    #endregion

    #region RegisterHandler Tests

    [Fact]
    public void RegisterHandler_WithValidHandler_ShouldRegister()
    {
        // Arrange
        var handlerMock = new Mock<IJurisdictionHandler>();

        // Act - should not throw
        _sut.RegisterHandler("BR", handlerMock.Object);

        // Assert - setup for compliance check
        _regulatoryServiceMock
            .Setup(r => r.GetUpdatesAsync("BR"))
            .ReturnsAsync(Enumerable.Empty<string>());

        handlerMock
            .Setup(h => h.AssessComplianceAsync(It.IsAny<ComplianceRequest>()))
            .ReturnsAsync(new ComplianceResult { IsCompliant = true });

        // Should be able to use the handler now
        var result = _sut.AssessComplianceAsync("BR", new ComplianceRequest()).Result;
        result.IsCompliant.ShouldBeTrue();
    }

    #endregion

    #region AssessComplianceAsync Tests

    [Fact]
    public async Task AssessComplianceAsync_WithRegisteredHandler_ShouldReturnResult()
    {
        // Arrange
        var handlerMock = new Mock<IJurisdictionHandler>();
        var expectedResult = new ComplianceResult
        {
            IsCompliant = true,
            RiskLevel = "Low",
            RiskScore = 0.2m
        };

        handlerMock
            .Setup(h => h.AssessComplianceAsync(It.IsAny<ComplianceRequest>()))
            .ReturnsAsync(expectedResult);

        _regulatoryServiceMock
            .Setup(r => r.GetUpdatesAsync("BR"))
            .ReturnsAsync(Enumerable.Empty<string>());

        _sut.RegisterHandler("BR", handlerMock.Object);

        var request = new ComplianceRequest
        {
            TransactionId = "txn-001",
            Amount = 1000m,
            Currency = "BRL",
            EntityType = "Individual"
        };

        // Act
        var result = await _sut.AssessComplianceAsync("BR", request);

        // Assert
        result.IsCompliant.ShouldBeTrue();
        result.RiskLevel.ShouldBe("Low");
    }

    [Fact]
    public async Task AssessComplianceAsync_WithUnregisteredHandler_ShouldThrowException()
    {
        // Arrange
        var request = new ComplianceRequest
        {
            TransactionId = "txn-001",
            Amount = 1000m
        };

        // Act & Assert
        await Should.ThrowAsync<NotSupportedException>(async () =>
            await _sut.AssessComplianceAsync("UNKNOWN", request));
    }

    [Fact]
    public async Task AssessComplianceAsync_WithRegulatoryUpdates_ShouldIncludeNotes()
    {
        // Arrange
        var handlerMock = new Mock<IJurisdictionHandler>();
        var updates = new[] { "New regulation effective 2026-01-01", "Rate change notice" };

        handlerMock
            .Setup(h => h.AssessComplianceAsync(It.IsAny<ComplianceRequest>()))
            .ReturnsAsync(new ComplianceResult { IsCompliant = true });

        _regulatoryServiceMock
            .Setup(r => r.GetUpdatesAsync("MX"))
            .ReturnsAsync(updates);

        _sut.RegisterHandler("MX", handlerMock.Object);

        var request = new ComplianceRequest { TransactionId = "txn-001" };

        // Act
        var result = await _sut.AssessComplianceAsync("MX", request);

        // Assert
        result.RegulatoryNotes.ShouldNotBeNull();
        result.RegulatoryNotes.Count().ShouldBe(2);
    }

    [Fact]
    public async Task AssessComplianceAsync_ShouldCallHandlerWithRequest()
    {
        // Arrange
        var handlerMock = new Mock<IJurisdictionHandler>();
        ComplianceRequest? capturedRequest = null;

        handlerMock
            .Setup(h => h.AssessComplianceAsync(It.IsAny<ComplianceRequest>()))
            .Callback<ComplianceRequest>(r => capturedRequest = r)
            .ReturnsAsync(new ComplianceResult { IsCompliant = true });

        _regulatoryServiceMock
            .Setup(r => r.GetUpdatesAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<string>());

        _sut.RegisterHandler("IN", handlerMock.Object);

        var request = new ComplianceRequest
        {
            TransactionId = "txn-123",
            Amount = 50000m,
            Currency = "INR",
            EntityType = "Business"
        };

        // Act
        await _sut.AssessComplianceAsync("IN", request);

        // Assert
        capturedRequest.ShouldNotBeNull();
        capturedRequest.TransactionId.ShouldBe("txn-123");
        capturedRequest.Amount.ShouldBe(50000m);
        capturedRequest.Currency.ShouldBe("INR");
    }

    #endregion

    #region Model Tests

    [Fact]
    public void ComplianceRequest_ShouldInitializeCorrectly()
    {
        // Act
        var request = new ComplianceRequest
        {
            TransactionId = "txn-001",
            Amount = 10000m,
            Currency = "USD",
            EntityType = "Corporation",
            AdditionalData = new Dictionary<string, object>
            {
                { "industry", "Finance" },
                { "risk_factor", 0.5 }
            }
        };

        // Assert
        request.TransactionId.ShouldBe("txn-001");
        request.Amount.ShouldBe(10000m);
        request.AdditionalData["industry"].ShouldBe("Finance");
    }

    [Fact]
    public void ComplianceResult_ShouldInitializeCorrectly()
    {
        // Act
        var result = new ComplianceResult
        {
            IsCompliant = false,
            RiskLevel = "High",
            RiskScore = 0.85m,
            Violations = new[] { "Missing documentation", "Amount exceeds threshold" },
            RegulatoryNotes = new[] { "Pending regulatory update" }
        };

        // Assert
        result.IsCompliant.ShouldBeFalse();
        result.RiskLevel.ShouldBe("High");
        result.Violations.Count().ShouldBe(2);
    }

    [Fact]
    public void JurisdictionData_ShouldInitializeCorrectly()
    {
        // Act
        var data = new JurisdictionData
        {
            Code = "BR",
            Name = "Brazil",
            Regulations = new Dictionary<string, object>
            {
                { "tax_rate", 0.15 },
                { "threshold", 100000 }
            }
        };

        // Assert
        data.Code.ShouldBe("BR");
        data.Name.ShouldBe("Brazil");
        data.Regulations["tax_rate"].ShouldBe(0.15);
    }

    #endregion
}
