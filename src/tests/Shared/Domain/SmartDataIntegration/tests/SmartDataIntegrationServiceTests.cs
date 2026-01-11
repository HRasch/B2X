using System;
using System.Threading.Tasks;
using B2X.SmartDataIntegration.Core;
using B2X.SmartDataIntegration.Models;
using B2X.SmartDataIntegration.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.SmartDataIntegration.Tests;

public class SmartDataIntegrationServiceTests
{
    private readonly Mock<IMappingRepository> _repositoryMock;
    private readonly Mock<IMappingEngine> _engineMock;
    private readonly Mock<IMappingValidator> _validatorMock;
    private readonly Mock<ILogger<SmartDataIntegrationService>> _loggerMock;
    private readonly SmartDataIntegrationService _service;

    public SmartDataIntegrationServiceTests()
    {
        _repositoryMock = new Mock<IMappingRepository>();
        _engineMock = new Mock<IMappingEngine>();
        _validatorMock = new Mock<IMappingValidator>();
        _loggerMock = new Mock<ILogger<SmartDataIntegrationService>>();

        _service = new SmartDataIntegrationService(
            _repositoryMock.Object,
            _engineMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateMappingConfigurationAsync_ShouldCreateConfiguration()
    {
        // Arrange
        var tenantIdString = "550e8400-e29b-41d4-a716-446655440000";
        var tenantIdGuid = Guid.Parse(tenantIdString);
        var name = "Test Mapping";
        var description = "Test description";
        var sourceSystem = "ERP";
        var targetSystem = "Catalog";
        var createdBy = "test-user";

        var expectedConfig = new DataMappingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantIdGuid,
            Name = name,
            Description = description,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            CreatedBy = null, // "test-user" is not a valid Guid
            ModifiedBy = null
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<DataMappingConfiguration>()))
            .ReturnsAsync(expectedConfig);

        // Act
        var result = await _service.CreateMappingConfigurationAsync(
            tenantIdString, name, description, sourceSystem, targetSystem, createdBy);

        // Assert
        result.ShouldNotBeNull();
        result.TenantId.ShouldBe(tenantIdGuid);
        result.Name.ShouldBe(name);
        result.Description.ShouldBe(description);
        result.SourceSystem.ShouldBe(sourceSystem);
        result.TargetSystem.ShouldBe(targetSystem);
        result.CreatedBy.ShouldBeNull();
        result.ModifiedBy.ShouldBeNull();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<DataMappingConfiguration>()), Times.Once);
    }

    [Fact]
    public async Task GetMappingConfigurationAsync_ShouldReturnConfiguration()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var expectedConfig = new DataMappingConfiguration
        {
            Id = configId,
            TenantId = tenantId,
            Name = "Test Config"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(configId))
            .ReturnsAsync(expectedConfig);

        // Act
        var result = await _service.GetMappingConfigurationAsync(configId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(configId);
        result.Name.ShouldBe("Test Config");

        _repositoryMock.Verify(r => r.GetByIdAsync(configId), Times.Once);
    }

    [Fact]
    public async Task GenerateMappingSuggestionsAsync_ShouldReturnSuggestions()
    {
        // Arrange
        var sourceFields = new[]
        {
            new DataField { Name = "product_name", DisplayName = "Product Name", DataType = DataFieldType.String },
            new DataField { Name = "price", DisplayName = "Price", DataType = DataFieldType.Decimal }
        };

        var targetFields = new[]
        {
            new DataField { Name = "name", DisplayName = "Name", DataType = DataFieldType.String },
            new DataField { Name = "cost", DisplayName = "Cost", DataType = DataFieldType.Decimal }
        };

        var expectedSuggestions = new[]
        {
            new MappingSuggestion
            {
                Id = Guid.NewGuid(),
                SourceField = sourceFields[0],
                SuggestedTargetField = targetFields[0],
                ConfidenceScore = 85.0,
                Reasoning = "High name similarity"
            }
        };

        _engineMock
            .Setup(e => e.AnalyzeAndSuggestMappingsAsync(
                It.IsAny<IEnumerable<DataField>>(),
                It.IsAny<IEnumerable<DataField>>(),
                It.IsAny<MappingContext>()))
            .ReturnsAsync(expectedSuggestions);

        // Act
        var result = await _service.GenerateMappingSuggestionsAsync(
            "ERP", sourceFields, "Catalog", targetFields);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedSuggestions);

        _engineMock.Verify(e => e.AnalyzeAndSuggestMappingsAsync(
            It.IsAny<IEnumerable<DataField>>(),
            It.IsAny<IEnumerable<DataField>>(),
            It.IsAny<MappingContext>()), Times.Once);
    }
}
