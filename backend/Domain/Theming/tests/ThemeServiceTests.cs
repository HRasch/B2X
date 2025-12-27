using Xunit;
using Moq;
using B2Connect.ThemeService.Models;
using B2Connect.ThemeService.Services;

namespace B2Connect.ThemeService.Tests;

/// <summary>
/// Test Suite für Theme Service
/// TDD: Tests zuerst, dann Implementation
/// Verwaltet Design-Variablen, Farben, Typografie und Theme-Publishing
/// </summary>
public class ThemeServiceTests
{
    private readonly Mock<IThemeRepository> _mockRepository;
    private readonly IThemeService _service;

    public ThemeServiceTests()
    {
        _mockRepository = new Mock<IThemeRepository>();
        _service = new ThemeService.Services.ThemeService(_mockRepository.Object);
    }

    #region Theme CRUD Tests

    [Fact]
    public async Task CreateTheme_WithValidThemeData_ShouldReturnCreatedTheme()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var createRequest = new CreateThemeRequest
        {
            Name = "Default Theme",
            Description = "Default storefront theme",
            PrimaryColor = "#007bff",
            SecondaryColor = "#6c757d"
        };
        var expectedThemeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.CreateThemeAsync(tenantId, It.IsAny<Theme>()))
            .ReturnsAsync(new Theme
            {
                Id = expectedThemeId,
                TenantId = tenantId,
                Name = createRequest.Name,
                Description = createRequest.Description,
                PrimaryColor = createRequest.PrimaryColor,
                SecondaryColor = createRequest.SecondaryColor,
                Version = 1,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });

        // Act
        var result = await _service.CreateThemeAsync(tenantId, createRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedThemeId, result.Id);
        Assert.Equal(createRequest.Name, result.Name);
        Assert.Equal(1, result.Version);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task CreateTheme_WithDuplicateName_ShouldThrowInvalidOperation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var createRequest = new CreateThemeRequest { Name = "Existing Theme" };

        _mockRepository
            .Setup(r => r.ThemeNameExistsAsync(tenantId, createRequest.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateThemeAsync(tenantId, createRequest));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public async Task GetThemeById_WithValidId_ShouldReturnTheme()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var theme = new Theme
        {
            Id = themeId,
            TenantId = tenantId,
            Name = "Test Theme",
            Version = 1
        };

        _mockRepository
            .Setup(r => r.GetThemeByIdAsync(tenantId, themeId))
            .ReturnsAsync(theme);

        // Act
        var result = await _service.GetThemeByIdAsync(tenantId, themeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(themeId, result.Id);
        Assert.Equal("Test Theme", result.Name);
    }

    [Fact]
    public async Task GetThemeById_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var invalidThemeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetThemeByIdAsync(tenantId, invalidThemeId))
            .ReturnsAsync((Theme?)null);

        // Act
        var result = await _service.GetThemeByIdAsync(tenantId, invalidThemeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveTheme_ShouldReturnPublishedTheme()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var activeTheme = new Theme
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Active Theme",
            IsActive = true,
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepository
            .Setup(r => r.GetActiveThemeAsync(tenantId))
            .ReturnsAsync(activeTheme);

        // Act
        var result = await _service.GetActiveThemeAsync(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UpdateTheme_WithValidData_ShouldIncrementVersion()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var updateRequest = new UpdateThemeRequest { Name = "Updated Theme" };

        _mockRepository
            .Setup(r => r.UpdateThemeAsync(tenantId, themeId, It.IsAny<Theme>()))
            .ReturnsAsync(new Theme
            {
                Id = themeId,
                TenantId = tenantId,
                Name = updateRequest.Name,
                Version = 2,
                UpdatedAt = DateTime.UtcNow
            });

        // Act
        var result = await _service.UpdateThemeAsync(tenantId, themeId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Version);
        Assert.Equal("Updated Theme", result.Name);
    }

    [Fact]
    public async Task DeleteTheme_WithValidId_ShouldSucceed()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.DeleteThemeAsync(tenantId, themeId))
            .Returns(Task.CompletedTask);

        // Act & Assert - should not throw
        await _service.DeleteThemeAsync(tenantId, themeId);
        _mockRepository.Verify(r => r.DeleteThemeAsync(tenantId, themeId), Times.Once);
    }

    #endregion

    #region Design Variable Tests

    [Fact]
    public async Task AddDesignVariable_WithValidData_ShouldCreateVariable()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var varRequest = new AddDesignVariableRequest
        {
            Name = "primary-color",
            Value = "#007bff",
            Category = "Colors"
        };

        _mockRepository
            .Setup(r => r.AddDesignVariableAsync(tenantId, themeId, It.IsAny<DesignVariable>()))
            .ReturnsAsync(new DesignVariable
            {
                Id = Guid.NewGuid(),
                Name = varRequest.Name,
                Value = varRequest.Value,
                Category = varRequest.Category
            });

        // Act
        var result = await _service.AddDesignVariableAsync(tenantId, themeId, varRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("primary-color", result.Name);
        Assert.Equal("#007bff", result.Value);
    }

    [Fact]
    public async Task UpdateDesignVariable_WithValidData_ShouldUpdateValue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var variableId = Guid.NewGuid();
        var updateRequest = new UpdateDesignVariableRequest { Value = "#0056b3" };

        _mockRepository
            .Setup(r => r.UpdateDesignVariableAsync(tenantId, themeId, variableId, It.IsAny<DesignVariable>()))
            .ReturnsAsync(new DesignVariable
            {
                Id = variableId,
                Name = "primary-color",
                Value = updateRequest.Value,
                Category = "Colors"
            });

        // Act
        var result = await _service.UpdateDesignVariableAsync(tenantId, themeId, variableId, updateRequest);

        // Assert
        Assert.Equal("#0056b3", result.Value);
    }

    [Fact]
    public async Task GetDesignVariables_ShouldReturnAllVariablesForTheme()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var variables = new List<DesignVariable>
        {
            new() { Id = Guid.NewGuid(), Name = "primary-color", Value = "#007bff" },
            new() { Id = Guid.NewGuid(), Name = "secondary-color", Value = "#6c757d" }
        };

        _mockRepository
            .Setup(r => r.GetDesignVariablesAsync(tenantId, themeId))
            .ReturnsAsync(variables);

        // Act
        var result = await _service.GetDesignVariablesAsync(tenantId, themeId);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task RemoveDesignVariable_WithValidId_ShouldDelete()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var variableId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.RemoveDesignVariableAsync(tenantId, themeId, variableId))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await _service.RemoveDesignVariableAsync(tenantId, themeId, variableId);
        _mockRepository.Verify(r => r.RemoveDesignVariableAsync(tenantId, themeId, variableId), Times.Once);
    }

    #endregion

    #region CSS Generation Tests

    [Fact]
    public async Task GenerateCSS_WithValidTheme_ShouldReturnCSSString()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var cssContent = ":root { --primary-color: #007bff; }";

        _mockRepository
            .Setup(r => r.GenerateCSSAsync(tenantId, themeId))
            .ReturnsAsync(cssContent);

        // Act
        var result = await _service.GenerateCSSAsync(tenantId, themeId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("--primary-color", result);
    }

    [Fact]
    public async Task GenerateThemeJSON_WithValidTheme_ShouldReturnJSON()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GenerateThemeJSONAsync(tenantId, themeId))
            .ReturnsAsync("{\"primaryColor\": \"#007bff\"}");

        // Act
        var result = await _service.GenerateThemeJSONAsync(tenantId, themeId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("primaryColor", result);
    }

    #endregion

    #region Theme Publishing Tests

    [Fact]
    public async Task PublishTheme_WithValidTheme_ShouldMarkAsActive()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.PublishThemeAsync(tenantId, themeId))
            .ReturnsAsync(new Theme
            {
                Id = themeId,
                TenantId = tenantId,
                IsActive = true,
                PublishedAt = DateTime.UtcNow,
                Version = 1
            });

        // Act
        var result = await _service.PublishThemeAsync(tenantId, themeId);

        // Assert
        Assert.True(result.IsActive);
        Assert.NotNull(result.PublishedAt);
    }

    [Fact]
    public async Task UnpublishTheme_WithActiveTheme_ShouldDeactivate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.UnpublishThemeAsync(tenantId, themeId))
            .ReturnsAsync(new Theme
            {
                Id = themeId,
                TenantId = tenantId,
                IsActive = false,
                Version = 1
            });

        // Act
        var result = await _service.UnpublishThemeAsync(tenantId, themeId);

        // Assert
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task GetPublishedThemes_ShouldReturnOnlyPublishedThemes()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var publishedThemes = new List<Theme>
        {
            new() { Id = Guid.NewGuid(), Name = "Theme 1", IsActive = true, PublishedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Theme 2", IsActive = false, PublishedAt = DateTime.UtcNow.AddDays(-1) }
        };

        _mockRepository
            .Setup(r => r.GetPublishedThemesAsync(tenantId))
            .ReturnsAsync(publishedThemes);

        // Act
        var result = await _service.GetPublishedThemesAsync(tenantId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.NotNull(t.PublishedAt));
    }

    #endregion

    #region Theme Variants Tests

    [Fact]
    public async Task CreateThemeVariant_WithValidData_ShouldCreateVariant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var variantRequest = new CreateThemeVariantRequest
        {
            Name = "Dark Mode",
            Description = "Dark theme variant"
        };

        _mockRepository
            .Setup(r => r.CreateThemeVariantAsync(tenantId, themeId, It.IsAny<ThemeVariant>()))
            .ReturnsAsync(new ThemeVariant
            {
                Id = Guid.NewGuid(),
                ThemeId = themeId,
                Name = variantRequest.Name,
                Description = variantRequest.Description
            });

        // Act
        var result = await _service.CreateThemeVariantAsync(tenantId, themeId, variantRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dark Mode", result.Name);
    }

    [Fact]
    public async Task GetThemeVariants_ShouldReturnAllVariants()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var variants = new List<ThemeVariant>
        {
            new() { Id = Guid.NewGuid(), Name = "Light Mode" },
            new() { Id = Guid.NewGuid(), Name = "Dark Mode" }
        };

        _mockRepository
            .Setup(r => r.GetThemeVariantsAsync(tenantId, themeId))
            .ReturnsAsync(variants);

        // Act
        var result = await _service.GetThemeVariantsAsync(tenantId, themeId);

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion
}
