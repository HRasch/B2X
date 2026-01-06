using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;
using B2Connect.LocalizationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace B2Connect.LocalizationService.Tests.Services;

public class LocalizationServiceTests : IAsyncLifetime, IDisposable
{
    private LocalizationDbContext _dbContext = null!;
    private Mock<IDistributedCache> _cacheMock = null!;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
    private B2Connect.LocalizationService.Services.LocalizationService _service = null!;
    private Mock<HttpContext> _httpContextMock = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LocalizationDbContext>()
            .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
            .Options;

        _dbContext = new LocalizationDbContext(options);
        _cacheMock = new Mock<IDistributedCache>();

        _httpContextMock = new Mock<HttpContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        _service = new B2Connect.LocalizationService.Services.LocalizationService(_dbContext, _cacheMock.Object, _httpContextAccessorMock.Object);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    #region GetStringAsync Tests

    [Fact]
    public async Task GetStringAsync_WithValidKeyAndLanguage_ReturnsTranslation()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "login",
            Category = "auth",
            DefaultValue = "Login",
            Translations = new Dictionary<string, string>
            {
                { "en", "Login" },
                { "de", "Anmelden" }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetStringAsync("login", "auth", "de");

        // Assert
        Assert.Equal("Anmelden", result);
    }

    [Fact]
    public async Task GetStringAsync_WithUnsupportedLanguage_FallsBackToEnglish()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "title",
            Category = "auth",
            DefaultValue = "Login",
            Translations = new Dictionary<string, string>
            {
                { "en", "Login" }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetStringAsync("title", "auth", "xx");

        // Assert
        Assert.Equal("Login", result);
    }

    [Fact]
    public async Task GetStringAsync_WithMissingKey_ReturnsFormattedKeyPlaceholder()
    {
        // Act
        var result = await _service.GetStringAsync("nonexistent", "auth", "en");

        // Assert
        Assert.Equal("[auth.nonexistent]", result);
    }

    [Fact]
    public async Task GetStringAsync_WithCurrentCulture_UsesCurrentLanguage()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "logout",
            Category = "auth",
            DefaultValue = "Logout",
            Translations = new Dictionary<string, string>
            {
                { "en", "Logout" },
                { "de", "Abmelden" }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        var httpContext = new DefaultHttpContext();
        httpContext.Items["Language"] = "de";
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = await _service.GetStringAsync("logout", "auth");

        // Assert
        Assert.Equal("Abmelden", result);
    }

    [Fact]
    public async Task GetStringAsync_DefaultsToEnglish_WhenNoLanguageSet()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "default",
            Category = "ui",
            DefaultValue = "Default Text",
            Translations = new Dictionary<string, string>
            {
                { "en", "Default Text" }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        // Act
        var result = await _service.GetStringAsync("default", "ui");

        // Assert
        Assert.Equal("Default Text", result);
    }

    #endregion

    #region GetCategoryAsync Tests

    [Fact]
    public async Task GetCategoryAsync_WithValidCategory_ReturnsAllTranslationsForLanguage()
    {
        // Arrange
        var strings = new[]
        {
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "save",
                Category = "ui",
                DefaultValue = "Save",
                Translations = new() { { "en", "Save" }, { "de", "Speichern" } },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = "cancel",
                Category = "ui",
                DefaultValue = "Cancel",
                Translations = new() { { "en", "Cancel" }, { "de", "Abbrechen" } },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        _dbContext.LocalizedStrings.AddRange(strings);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCategoryAsync("ui", "de");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Speichern", result["save"]);
        Assert.Equal("Abbrechen", result["cancel"]);
    }

    [Fact]
    public async Task GetCategoryAsync_EmptyCategory_ReturnsEmptyDictionary()
    {
        // Act
        var result = await _service.GetCategoryAsync("nonexistent", "en");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCategoryAsync_MixedLanguages_FallsBackToDefaultValue()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "item1",
            Category = "test",
            DefaultValue = "Default Item",
            Translations = new() { { "en", "English Item" } }, // Only English
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetCategoryAsync("test", "de");

        // Assert
        Assert.Single(result);
        Assert.Equal("Default Item", result["item1"]);
    }

    #endregion

    #region SetStringAsync Tests

    [Fact]
    public async Task SetStringAsync_WithNewKey_CreatesNewLocalizedString()
    {
        // Arrange
        var translations = new Dictionary<string, string>
        {
            { "en", "New String" },
            { "de", "Neuer String" }
        };

        // Act
        await _service.SetStringAsync(null, "newkey", "test", translations);

        // Assert
        var saved = await _dbContext.LocalizedStrings
            .FirstOrDefaultAsync(x => x.Key == "newkey" && x.Category == "test");

        Assert.NotNull(saved);
        Assert.Equal("New String", saved.Translations["en"]);
        Assert.Equal("Neuer String", saved.Translations["de"]);
        Assert.Equal("New String", saved.DefaultValue);
    }

    [Fact]
    public async Task SetStringAsync_WithExistingKey_UpdatesTranslations()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "update",
            Category = "test",
            DefaultValue = "Old Value",
            Translations = new() { { "en", "Old Value" } },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        var newTranslations = new Dictionary<string, string>
        {
            { "en", "Updated Value" },
            { "de", "Aktualisierter Wert" }
        };

        // Act
        await _service.SetStringAsync(null, "update", "test", newTranslations);

        // Assert
        var updated = await _dbContext.LocalizedStrings
            .FirstOrDefaultAsync(x => x.Key == "update" && x.Category == "test");

        Assert.NotNull(updated);
        Assert.Equal("Updated Value", updated.Translations["en"]);
        Assert.Equal("Aktualisierter Wert", updated.Translations["de"]);
        Assert.True(updated.UpdatedAt > localized.CreatedAt);
    }

    [Fact]
    public async Task SetStringAsync_WithTenantId_CreatesTenantSpecificOverride()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var translations = new Dictionary<string, string>
        {
            { "en", "Tenant-Specific" },
            { "de", "Mieterspezifisch" }
        };

        // Act
        await _service.SetStringAsync(tenantId, "tenant_key", "test", translations);

        // Assert
        var saved = await _dbContext.LocalizedStrings
            .FirstOrDefaultAsync(x => x.Key == "tenant_key" && x.Category == "test" && x.TenantId == tenantId);

        Assert.NotNull(saved);
        Assert.Equal(tenantId, saved.TenantId);
    }

    #endregion

    #region GetSupportedLanguagesAsync Tests

    [Fact]
    public async Task GetSupportedLanguagesAsync_ReturnsAllSupportedLanguages()
    {
        // Act
        var result = await _service.GetSupportedLanguagesAsync();

        // Assert
        Assert.NotNull(result);
        var languages = result.ToList();
        Assert.Contains("en", languages);
        Assert.Contains("de", languages);
        Assert.Contains("fr", languages);
        Assert.Equal(8, languages.Count);
    }

    #endregion

    #region Caching Tests

    // [Fact] - Disabled due to Moq limitation with extension methods on IDistributedCache
    [Fact]
    public async Task GetStringAsync_CachesResult_AvoidsDatabaseHits()
    {
        // Arrange
        var localized = new LocalizedString
        {
            Id = Guid.NewGuid(),
            Key = "cached",
            Category = "test",
            DefaultValue = "Cached Value",
            Translations = new() { { "en", "Cached Value" } },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.LocalizedStrings.Add(localized);
        await _dbContext.SaveChangesAsync();

        // Setup cache mock to return cached value on first call, then null
        _cacheMock.SetupSequence(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null) // First call - cache miss
            .ReturnsAsync("Cached Value"); // Second call - cache hit

        // First call - hits database and caches
        var result1 = await _service.GetStringAsync("cached", "test", "en");

        // Remove from database to prove caching works
        _dbContext.LocalizedStrings.Remove(localized);
        await _dbContext.SaveChangesAsync();

        // Act - Second call should return cached value
        var result2 = await _service.GetStringAsync("cached", "test", "en");

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal("Cached Value", result2);
    }

    #endregion

    #region Current Language Tests

    [Fact]
    public void GetCurrentLanguage_WithLanguageInItems_ReturnsLanguage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Items["Language"] = "fr";
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _service.GetCurrentLanguage();

        // Assert
        Assert.Equal("fr", result);
    }

    [Fact]
    public void GetCurrentLanguage_WithoutLanguage_ReturnsDefault()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _service.GetCurrentLanguage();

        // Assert
        Assert.Equal("en", result);
    }

    public void Dispose()
    {
        // No cleanup needed - using mocked services
        GC.SuppressFinalize(this);
    }

    #endregion
}
