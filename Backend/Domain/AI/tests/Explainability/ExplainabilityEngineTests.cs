using B2X.AI.Explainability;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.AI.Tests.Explainability;

public class ExplainabilityEngineTests
{
    private readonly Mock<IModelInterpreter> _interpreterMock;
    private readonly Mock<IAuditLogger> _auditLoggerMock;
    private readonly ExplainabilityEngine _sut;

    public ExplainabilityEngineTests()
    {
        _interpreterMock = new Mock<IModelInterpreter>();
        _auditLoggerMock = new Mock<IAuditLogger>();
        _sut = new ExplainabilityEngine(_interpreterMock.Object, _auditLoggerMock.Object);
    }

    #region ExplainPredictionAsync Tests

    [Fact]
    public async Task ExplainPredictionAsync_ValidInput_ShouldReturnExplanation()
    {
        // Arrange
        var modelId = "fraud-detection-v1";
        var inputData = new { amount = 1000, category = "Electronics" };
        var prediction = new { isFraud = false, confidence = 0.95 };
        var userId = "user-001";

        var expectedExplanation = new ModelExplanation
        {
            ModelId = modelId,
            FeatureImportances = new Dictionary<string, double>
            {
                { "amount", 0.45 },
                { "category", 0.30 },
                { "time_of_day", 0.25 }
            },
            ExplanationText = "Transaction classified as legitimate due to normal amount and common category",
            Confidence = 0.95
        };

        _interpreterMock
            .Setup(i => i.GenerateExplanationAsync(modelId, inputData, prediction))
            .ReturnsAsync(expectedExplanation);

        // Act
        var result = await _sut.ExplainPredictionAsync(modelId, inputData, prediction, userId);

        // Assert
        result.ShouldNotBeNull();
        result.ModelId.ShouldBe(modelId);
        result.Confidence.ShouldBe(0.95);
        result.FeatureImportances.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ExplainPredictionAsync_ShouldCallInterpreter()
    {
        // Arrange
        var modelId = "recommendation-model";
        var inputData = new { userId = "123", context = "browse" };
        var prediction = new { productId = "prod-001" };
        var userId = "user-001";

        _interpreterMock
            .Setup(i => i.GenerateExplanationAsync(modelId, inputData, prediction))
            .ReturnsAsync(new ModelExplanation());

        // Act
        await _sut.ExplainPredictionAsync(modelId, inputData, prediction, userId);

        // Assert
        _interpreterMock.Verify(
            i => i.GenerateExplanationAsync(modelId, inputData, prediction),
            Times.Once);
    }

    [Fact]
    public async Task ExplainPredictionAsync_ShouldLogAuditEntry()
    {
        // Arrange
        var modelId = "price-prediction";
        var inputData = new { features = new[] { 1.0, 2.0, 3.0 } };
        var prediction = new { price = 99.99 };
        var userId = "analyst-001";
        AuditEntry? capturedEntry = null;

        _interpreterMock
            .Setup(i => i.GenerateExplanationAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
            .ReturnsAsync(new ModelExplanation { ModelId = modelId });

        _auditLoggerMock
            .Setup(a => a.LogExplanationAsync(It.IsAny<AuditEntry>()))
            .Callback<AuditEntry>(e => capturedEntry = e)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.ExplainPredictionAsync(modelId, inputData, prediction, userId);

        // Assert
        _auditLoggerMock.Verify(a => a.LogExplanationAsync(It.IsAny<AuditEntry>()), Times.Once);
        capturedEntry.ShouldNotBeNull();
        capturedEntry.ModelId.ShouldBe(modelId);
        capturedEntry.UserId.ShouldBe(userId);
        capturedEntry.Timestamp.ShouldNotBe(default);
    }

    [Fact]
    public async Task ExplainPredictionAsync_AuditEntry_ShouldContainAllData()
    {
        // Arrange
        var modelId = "sentiment-analysis";
        var inputData = new { text = "Great product!" };
        var prediction = new { sentiment = "positive", score = 0.92 };
        var userId = "reviewer-001";
        var explanation = new ModelExplanation
        {
            ModelId = modelId,
            ExplanationText = "Positive sentiment detected"
        };

        AuditEntry? capturedEntry = null;

        _interpreterMock
            .Setup(i => i.GenerateExplanationAsync(modelId, inputData, prediction))
            .ReturnsAsync(explanation);

        _auditLoggerMock
            .Setup(a => a.LogExplanationAsync(It.IsAny<AuditEntry>()))
            .Callback<AuditEntry>(e => capturedEntry = e)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.ExplainPredictionAsync(modelId, inputData, prediction, userId);

        // Assert
        capturedEntry.ShouldNotBeNull();
        capturedEntry.InputData.ShouldBe(inputData);
        capturedEntry.Prediction.ShouldBe(prediction);
        capturedEntry.Explanation.ShouldBe(explanation);
    }

    #endregion

    #region Model Tests

    [Fact]
    public void ModelExplanation_ShouldInitializeCorrectly()
    {
        // Act
        var explanation = new ModelExplanation
        {
            ModelId = "model-001",
            FeatureImportances = new Dictionary<string, double>
            {
                { "feature1", 0.5 },
                { "feature2", 0.3 },
                { "feature3", 0.2 }
            },
            ExplanationText = "Feature1 has the highest impact on this prediction",
            Confidence = 0.87
        };

        // Assert
        explanation.ModelId.ShouldBe("model-001");
        explanation.FeatureImportances.Count.ShouldBe(3);
        explanation.FeatureImportances["feature1"].ShouldBe(0.5);
        explanation.Confidence.ShouldBe(0.87);
    }

    [Fact]
    public void AuditEntry_ShouldInitializeCorrectly()
    {
        // Act
        var timestamp = DateTime.UtcNow;
        var entry = new AuditEntry
        {
            ModelId = "model-001",
            UserId = "user-001",
            Timestamp = timestamp,
            InputData = new { value = 100 },
            Prediction = new { result = "approved" },
            Explanation = new ModelExplanation { ModelId = "model-001" }
        };

        // Assert
        entry.ModelId.ShouldBe("model-001");
        entry.UserId.ShouldBe("user-001");
        entry.Timestamp.ShouldBe(timestamp);
        entry.Explanation.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void ModelExplanation_Confidence_ShouldAcceptValidRange(double confidence)
    {
        // Act
        var explanation = new ModelExplanation
        {
            Confidence = confidence
        };

        // Assert
        explanation.Confidence.ShouldBe(confidence);
    }

    [Fact]
    public void ModelExplanation_FeatureImportances_ShouldSumToReasonableValue()
    {
        // Arrange
        var importances = new Dictionary<string, double>
        {
            { "feature1", 0.4 },
            { "feature2", 0.35 },
            { "feature3", 0.25 }
        };

        // Act
        var explanation = new ModelExplanation
        {
            FeatureImportances = importances
        };

        // Assert - SHAP values typically sum to prediction contribution
        var sum = explanation.FeatureImportances.Values.Sum();
        sum.ShouldBe(1.0, 0.001); // Within tolerance
    }

    #endregion
}
