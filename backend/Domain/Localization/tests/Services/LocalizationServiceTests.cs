using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;
using B2Connect.LocalizationService.Services;
using B2Connect.Shared.Core;

namespace B2Connect.LocalizationService.Tests.Services;

public class LocalizationServiceTests : IAsyncLifetime
{
    private LocalizationDbContext _dbContext = null!;
    private IMemoryCache _cache = null!;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
    private B2Connect.LocalizationService.Services.LocalizationService _service = null!;
    private Mock<HttpContext> _httpContextMock = null!;
    private Mock<ITenantContext> _tenantContextMock = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LocalizationDbContext>()
            .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
            .Options;

        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.Setup(x => x.GetCurrentTenantId()).Returns(Guid.NewGuid());

        _dbContext = new LocalizationDbContext(options, _tenantContextMock.Object);
        _dbContext.SkipTenantValidation = true; // Disable tenant validation for tests
        _dbContext.SkipTenantFilters = true; // Disable tenant filters for tests
        _cache = new MemoryCache(new MemoryCacheOptions());

        _httpContextMock = new Mock<HttpContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        _service = new B2Connect.LocalizationService.Services.LocalizationService(_dbContext, _cache, _httpContextAccessorMock.Object);
    }

    private LocalizedStringEntity CreateTestLocalizedStringEntity(
        string key,
        string category,
        string defaultValue,
        Dictionary<string, string>? translations = null,
        Guid? tenantId = null)
    {
        var localizedString = new LocalizedString(key, category, defaultValue, translations);
        return new LocalizedStringEntity(
            tenantId: tenantId ?? _tenantContextMock.Object.GetCurrentTenantId(),
            localizedString: localizedString
        );
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        _cache.Dispose();
    }

    #region GetStringAsync Tests

    [Fact]
    public async Task GetStringAsync_WithValidKeyAndLanguage_ReturnsTranslation()
    {
        // Arrange
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "login",
            category: "auth",
            defaultValue: "Login",
            translations: new Dictionary<string, string>
            {
                { "en", "Login" },
                { "de", "Anmelden" }
            }
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
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
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "title",
            category: "auth",
            defaultValue: "Login",
            translations: new Dictionary<string, string>
            {
                { "en", "Login" }
            }
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
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
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "logout",
            category: "auth",
            defaultValue: "Logout",
            translations: new Dictionary<string, string>
            {
                { "en", "Logout" },
                { "de", "Abmelden" }
            }
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
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
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "default",
            category: "ui",
            defaultValue: "Default Text",
            translations: new Dictionary<string, string>
            {
                { "en", "Default Text" }
            }
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
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
            CreateTestLocalizedStringEntity(
                key: "save",
                category: "ui",
                defaultValue: "Save",
                translations: new() { { "en", "Save" }, { "de", "Speichern" } }
            ),
            CreateTestLocalizedStringEntity(
                key: "cancel",
                category: "ui",
                defaultValue: "Cancel",
                translations: new() { { "en", "Cancel" }, { "de", "Abbrechen" } }
            )
        };
        _dbContext.LocalizedStringEntities.AddRange(strings);
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
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "item1",
            category: "test",
            defaultValue: "Default Item",
            translations: new() { { "en", "English Item" } } // Only English
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
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
        var saved = await _dbContext.LocalizedStringEntities
            .FirstOrDefaultAsync(x => x.LocalizedString.Key == "newkey" && x.LocalizedString.Category == "test");

        Assert.NotNull(saved);
        Assert.Equal("New String", saved.LocalizedString.Translations["en"]);
        Assert.Equal("Neuer String", saved.LocalizedString.Translations["de"]);
        Assert.Equal("New String", saved.LocalizedString.DefaultValue);
    }

    [Fact]
    public async Task SetStringAsync_WithExistingKey_UpdatesTranslations()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(x => x.GetCurrentTenantId()).Returns(tenantId); // Mock returns the test tenant ID
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "update",
            category: "test",
            defaultValue: "Old Value",
            translations: new() { { "en", "Old Value" } },
            tenantId: tenantId
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
        await _dbContext.SaveChangesAsync();

        var newTranslations = new Dictionary<string, string>
        {
            { "en", "Updated Value" },
            { "de", "Aktualisierter Wert" }
        };

        // Act
        await _service.SetStringAsync(tenantId, "update", "test", newTranslations);

        // Assert
        var updated = await _dbContext.LocalizedStringEntities
            .FirstOrDefaultAsync(x => x.LocalizedString.Key == "update" && x.LocalizedString.Category == "test" && x.TenantId == tenantId);

        Assert.NotNull(updated);
        Assert.Equal("Updated Value", updated.LocalizedString.Translations["en"]);
        Assert.Equal("Aktualisierter Wert", updated.LocalizedString.Translations["de"]);
        Assert.True(updated.UpdatedAt > localizedEntity.CreatedAt);
    }

    [Fact]
    public async Task SetStringAsync_WithTenantId_CreatesTenantSpecificOverride()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(x => x.GetCurrentTenantId()).Returns(tenantId); // Mock returns the test tenant ID
        var translations = new Dictionary<string, string>
        {
            { "en", "Tenant-Specific" },
            { "de", "Mieterspezifisch" }
        };

        // Act
        await _service.SetStringAsync(tenantId, "tenant_key", "test", translations);

        // Assert
        var saved = await _dbContext.LocalizedStringEntities
            .FirstOrDefaultAsync(x => x.LocalizedString.Key == "tenant_key" && x.LocalizedString.Category == "test" && x.TenantId == tenantId);

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

    [Fact]
    public async Task GetStringAsync_CachesResult_AvoidsDatabaseHits()
    {
        // Arrange
        var localizedEntity = CreateTestLocalizedStringEntity(
            key: "cached",
            category: "test",
            defaultValue: "Cached Value",
            translations: new() { { "en", "Cached Value" } }
        );
        _dbContext.LocalizedStringEntities.Add(localizedEntity);
        await _dbContext.SaveChangesAsync();

        // First call - hits database
        var result1 = await _service.GetStringAsync("cached", "test", "en");

        // Remove from database to prove caching works
        _dbContext.LocalizedStringEntities.Remove(localizedEntity);
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

    #endregion
}
