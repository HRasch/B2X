using System.Security.Claims;
using B2Connect.LocalizationService.Controllers;
using B2Connect.LocalizationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace B2Connect.LocalizationService.Tests.Controllers;

public class LocalizationControllerTests
{
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly LocalizationController _controller;

    public LocalizationControllerTests()
    {
        _localizationServiceMock = new Mock<ILocalizationService>();
        _controller = new LocalizationController(_localizationServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetString_WithValidKeyAndLanguage_ReturnsOkWithTranslation()
    {
        // Arrange
        _localizationServiceMock
            .Setup(s => s.GetStringAsync("login", "auth", "de", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Anmelden");

        // Act
        var result = await _controller.GetString("auth", "login", "de");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        _localizationServiceMock.Verify(
            s => s.GetStringAsync("login", "auth", "de", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetString_WithDefaultLanguage_ReturnsEnglish()
    {
        // Arrange
        _localizationServiceMock
            .Setup(s => s.GetStringAsync("save", "ui", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Save");

        // Act
        var result = await _controller.GetString("ui", "save", "en");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetCategory_WithValidCategory_ReturnsAllTranslations()
    {
        // Arrange
        var translations = new Dictionary<string, string>
        {
            { "save", "Speichern" },
            { "cancel", "Abbrechen" },
            { "delete", "Löschen" }
        };

        _localizationServiceMock
            .Setup(s => s.GetCategoryAsync("ui", "de", It.IsAny<CancellationToken>()))
            .ReturnsAsync(translations);

        // Act
        var result = await _controller.GetCategory("ui", "de");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        _localizationServiceMock.Verify(
            s => s.GetCategoryAsync("ui", "de", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetLanguages_ReturnsAllSupportedLanguages()
    {
        // Arrange
        var languages = new[] { "en", "de", "fr", "es", "it", "pt", "nl", "pl" };
        _localizationServiceMock
            .Setup(s => s.GetSupportedLanguagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(languages);

        // Act
        var result = await _controller.GetLanguages();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        _localizationServiceMock.Verify(
            s => s.GetSupportedLanguagesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetString_WithValidData_CallsServiceAndReturnsNoContent()
    {
        // Arrange
        var translations = new Dictionary<string, string>
        {
            { "en", "New String" },
            { "de", "Neuer String" }
        };

        _localizationServiceMock
            .Setup(s => s.SetStringAsync(
                It.IsAny<Guid?>(),
                "newkey",
                "test",
                translations,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Add admin user to controller
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        // Act
        var result = await _controller.SetString("test", "newkey", translations);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _localizationServiceMock.Verify(
            s => s.SetStringAsync(
                It.IsAny<Guid?>(),
                "newkey",
                "test",
                translations,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetString_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var translations = new Dictionary<string, string>
        {
            { "en", "New String" }
        };

        // Add non-admin user
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        // Act
        var result = await _controller.SetString("test", "newkey", translations);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetString_WithMissingKey_ReturnsKeyPlaceholder()
    {
        // Arrange
        _localizationServiceMock
            .Setup(s => s.GetStringAsync("missing", "auth", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync("[auth.missing]");

        // Act
        var result = await _controller.GetString("auth", "missing", "en");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetCategory_WithEmptyCategory_ReturnsEmptyDictionary()
    {
        // Arrange
        var emptyTranslations = new Dictionary<string, string>();
        _localizationServiceMock
            .Setup(s => s.GetCategoryAsync("nonexistent", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTranslations);

        // Act
        var result = await _controller.GetCategory("nonexistent", "en");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
