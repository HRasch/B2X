using B2X.Compliance.RiskAssessment;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Compliance.Tests.RiskAssessment;

public class RiskAssessmentEngineTests
{
    private readonly Mock<IRiskModel> _riskModelMock;
    private readonly Mock<IAlertService> _alertServiceMock;
    private readonly Mock<IMitigationService> _mitigationServiceMock;
    private readonly RiskAssessmentEngine _sut;

    public RiskAssessmentEngineTests()
    {
        _riskModelMock = new Mock<IRiskModel>();
        _alertServiceMock = new Mock<IAlertService>();
        _mitigationServiceMock = new Mock<IMitigationService>();
        _sut = new RiskAssessmentEngine(
            _riskModelMock.Object,
            _alertServiceMock.Object,
            _mitigationServiceMock.Object);
    }

    #region AssessRiskAsync Tests

    [Fact]
    public async Task AssessRiskAsync_LowRisk_ShouldNotSendAlert()
    {
        // Arrange
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.3m,
            RiskLevel = "Low"
        };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(Enumerable.Empty<string>());

        // Act
        var result = await _sut.AssessRiskAsync(request);

        // Assert
        result.RiskScore.ShouldBe(0.3m);
        _alertServiceMock.Verify(a => a.SendAlertAsync(It.IsAny<RiskAlert>()), Times.Never);
    }

    [Fact]
    public async Task AssessRiskAsync_HighRisk_ShouldSendAlert()
    {
        // Arrange
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.9m,
            RiskLevel = "Critical"
        };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(new[] { "Review transaction", "Contact compliance team" });

        // Act
        var result = await _sut.AssessRiskAsync(request);

        // Assert
        _alertServiceMock.Verify(
            a => a.SendAlertAsync(It.Is<RiskAlert>(alert =>
                alert.RiskScore == 0.9m &&
                alert.AssessmentId == "assess-001")),
            Times.Once);
    }

    [Fact]
    public async Task AssessRiskAsync_AtThreshold_ShouldSendAlert()
    {
        // Arrange - Risk score > 0.8 should trigger alert
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.81m,
            RiskLevel = "High"
        };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(Enumerable.Empty<string>());

        // Act
        await _sut.AssessRiskAsync(request);

        // Assert
        _alertServiceMock.Verify(a => a.SendAlertAsync(It.IsAny<RiskAlert>()), Times.Once);
    }

    [Fact]
    public async Task AssessRiskAsync_JustBelowThreshold_ShouldNotSendAlert()
    {
        // Arrange - Risk score <= 0.8 should not trigger alert
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.8m,
            RiskLevel = "Medium"
        };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(Enumerable.Empty<string>());

        // Act
        await _sut.AssessRiskAsync(request);

        // Assert
        _alertServiceMock.Verify(a => a.SendAlertAsync(It.IsAny<RiskAlert>()), Times.Never);
    }

    [Fact]
    public async Task AssessRiskAsync_ShouldGenerateRecommendations()
    {
        // Arrange
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.5m
        };
        var recommendations = new[] { "Verify identity", "Request additional documentation" };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(recommendations);

        // Act
        var result = await _sut.AssessRiskAsync(request);

        // Assert
        result.Recommendations.ShouldNotBeNull();
        result.Recommendations.Count().ShouldBe(2);
    }

    [Fact]
    public async Task AssessRiskAsync_ShouldCallRiskModel()
    {
        // Arrange
        var request = CreateTestRequest();
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.4m
        };

        _riskModelMock
            .Setup(r => r.PredictRiskAsync(request))
            .ReturnsAsync(assessment);

        _mitigationServiceMock
            .Setup(m => m.GenerateRecommendationsAsync(assessment))
            .ReturnsAsync(Enumerable.Empty<string>());

        // Act
        await _sut.AssessRiskAsync(request);

        // Assert
        _riskModelMock.Verify(r => r.PredictRiskAsync(request), Times.Once);
    }

    #endregion

    #region GetRiskAnalyticsAsync Tests

    [Fact]
    public async Task GetRiskAnalyticsAsync_ShouldReturnAnalytics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var expectedAnalytics = new RiskAnalytics
        {
            TotalAssessments = 100,
            AverageRiskScore = 0.45m,
            RiskLevelDistribution = new Dictionary<string, int>
            {
                { "Low", 60 },
                { "Medium", 30 },
                { "High", 8 },
                { "Critical", 2 }
            }
        };

        _riskModelMock
            .Setup(r => r.GetAnalyticsAsync(startDate, endDate))
            .ReturnsAsync(expectedAnalytics);

        // Act
        var result = await _sut.GetRiskAnalyticsAsync(startDate, endDate);

        // Assert
        result.TotalAssessments.ShouldBe(100);
        result.AverageRiskScore.ShouldBe(0.45m);
        result.RiskLevelDistribution["Low"].ShouldBe(60);
    }

    #endregion

    #region Model Tests

    [Fact]
    public void RiskAssessmentRequest_ShouldInitializeCorrectly()
    {
        // Act
        var request = new RiskAssessmentRequest
        {
            EntityId = "entity-001",
            TransactionType = "Transfer",
            Amount = 50000m,
            Jurisdiction = "US",
            ContextData = new Dictionary<string, object>
            {
                { "source", "Online" },
                { "frequency", "First" }
            }
        };

        // Assert
        request.EntityId.ShouldBe("entity-001");
        request.TransactionType.ShouldBe("Transfer");
        request.Amount.ShouldBe(50000m);
        request.ContextData["source"].ShouldBe("Online");
    }

    [Fact]
    public void RiskAssessment_ShouldInitializeCorrectly()
    {
        // Act
        var assessment = new RiskAssessmentResult
        {
            Id = "assess-001",
            RiskScore = 0.75m,
            RiskLevel = "High",
            RiskFactors = new[] { "Large amount", "New customer", "Cross-border" },
            Recommendations = new[] { "Manual review required" },
            Timestamp = DateTime.UtcNow
        };

        // Assert
        assessment.Id.ShouldBe("assess-001");
        assessment.RiskScore.ShouldBe(0.75m);
        assessment.RiskFactors.Count().ShouldBe(3);
    }

    [Fact]
    public void RiskAlert_ShouldInitializeCorrectly()
    {
        // Act
        var alert = new RiskAlert
        {
            AssessmentId = "assess-001",
            RiskScore = 0.95m,
            RiskLevel = "Critical",
            Recommendations = new[] { "Block transaction", "Contact fraud team" }
        };

        // Assert
        alert.AssessmentId.ShouldBe("assess-001");
        alert.RiskScore.ShouldBe(0.95m);
        alert.Recommendations.Count().ShouldBe(2);
    }

    [Fact]
    public void RiskAnalytics_ShouldInitializeCorrectly()
    {
        // Act
        var analytics = new RiskAnalytics
        {
            TotalAssessments = 500,
            AverageRiskScore = 0.35m,
            RiskLevelDistribution = new Dictionary<string, int>
            {
                { "Low", 350 },
                { "Medium", 100 },
                { "High", 40 },
                { "Critical", 10 }
            },
            Trends = new List<RiskTrend>
            {
                new RiskTrend { Date = DateTime.UtcNow.AddDays(-1), AverageScore = 0.32m, AssessmentCount = 50 },
                new RiskTrend { Date = DateTime.UtcNow, AverageScore = 0.38m, AssessmentCount = 55 }
            }
        };

        // Assert
        analytics.TotalAssessments.ShouldBe(500);
        analytics.Trends.Count.ShouldBe(2);
    }

    [Theory]
    [InlineData("Low", 0.2)]
    [InlineData("Medium", 0.5)]
    [InlineData("High", 0.75)]
    [InlineData("Critical", 0.95)]
    public void RiskAssessment_RiskLevels_ShouldCorrelateWithScore(string level, decimal score)
    {
        // Act
        var assessment = new RiskAssessmentResult
        {
            RiskLevel = level,
            RiskScore = score
        };

        // Assert
        assessment.RiskLevel.ShouldBe(level);
        assessment.RiskScore.ShouldBe(score);
    }

    #endregion

    #region Helper Methods

    private static RiskAssessmentRequest CreateTestRequest()
    {
        return new RiskAssessmentRequest
        {
            EntityId = "entity-001",
            TransactionType = "Payment",
            Amount = 10000m,
            Jurisdiction = "US",
            ContextData = new Dictionary<string, object>()
        };
    }

    #endregion
}
