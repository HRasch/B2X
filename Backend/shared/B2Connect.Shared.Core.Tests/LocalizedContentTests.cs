using Xunit;
using B2Connect.Shared.Core;

namespace B2Connect.Shared.Core.Tests;

/// <summary>
/// Tests for LocalizedContent Value Object used in Hybrid Localization Pattern.
/// Verifies translation storage, retrieval, immutability, and edge cases.
/// </summary>
public class LocalizedContentTests
{
    #region Creation Tests

    [Fact]
    public void Create_WithDictionary_StoresTranslations()
    {
        // Arrange
        var translations = new Dictionary<string, string>
        {
            ["de"] = "Deutsch",
            ["fr"] = "Français"
        };

        // Act
        var content = LocalizedContent.Create(translations);

        // Assert
        Assert.Equal("Deutsch", content.GetValue("de"));
        Assert.Equal("Français", content.GetValue("fr"));
    }

    [Fact]
    public void Create_WithSingleLanguage_StoresTranslation()
    {
        // Act
        var content = LocalizedContent.Create("de", "Hallo Welt");

        // Assert
        Assert.Equal("Hallo Welt", content.GetValue("de"));
    }

    [Fact]
    public void Empty_ReturnsEmptyLocalizedContent()
    {
        // Act
        var content = LocalizedContent.Empty;

        // Assert
        Assert.Empty(content.Translations);
    }

    [Fact]
    public void Constructor_WithNullDictionary_CreatesEmptyTranslations()
    {
        // Act
        var content = new LocalizedContent(null);

        // Assert
        Assert.Empty(content.Translations);
    }

    #endregion

    #region GetValue Tests

    [Fact]
    public void GetValue_WithExistingLanguage_ReturnsTranslation()
    {
        // Arrange
        var content = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["en"] = "Hello",
            ["de"] = "Hallo"
        });

        // Act & Assert
        Assert.Equal("Hello", content.GetValue("en"));
        Assert.Equal("Hallo", content.GetValue("de"));
    }

    [Fact]
    public void GetValue_WithNonExistingLanguage_ReturnsNull()
    {
        // Arrange
        var content = LocalizedContent.Create("en", "Hello");

        // Act
        var result = content.GetValue("fr");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_WithNullLanguage_ReturnsNull()
    {
        // Arrange
        var content = LocalizedContent.Create("en", "Hello");

        // Act
        var result = content.GetValue(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_WithEmptyLanguage_ReturnsNull()
    {
        // Arrange
        var content = LocalizedContent.Create("en", "Hello");

        // Act
        var result = content.GetValue("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_IsCaseInsensitive()
    {
        // Arrange
        var content = LocalizedContent.Create("DE", "Deutsch");

        // Act & Assert
        Assert.Equal("Deutsch", content.GetValue("de"));
        Assert.Equal("Deutsch", content.GetValue("DE"));
        Assert.Equal("Deutsch", content.GetValue("De"));
    }

    #endregion

    #region WithTranslation Tests (Immutability)

    [Fact]
    public void WithTranslation_ReturnsNewInstance()
    {
        // Arrange
        var original = LocalizedContent.Create("en", "Hello");

        // Act
        var modified = original.WithTranslation("de", "Hallo");

        // Assert
        Assert.NotSame(original, modified);
        Assert.Null(original.GetValue("de"));
        Assert.Equal("Hallo", modified.GetValue("de"));
    }

    [Fact]
    public void WithTranslation_PreservesExistingTranslations()
    {
        // Arrange
        var original = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["en"] = "Hello",
            ["de"] = "Hallo"
        });

        // Act
        var modified = original.WithTranslation("fr", "Bonjour");

        // Assert
        Assert.Equal("Hello", modified.GetValue("en"));
        Assert.Equal("Hallo", modified.GetValue("de"));
        Assert.Equal("Bonjour", modified.GetValue("fr"));
    }

    [Fact]
    public void WithTranslation_UpdatesExistingLanguage()
    {
        // Arrange
        var original = LocalizedContent.Create("de", "Alt");

        // Act
        var modified = original.WithTranslation("de", "Neu");

        // Assert
        Assert.Equal("Neu", modified.GetValue("de"));
        Assert.Equal("Alt", original.GetValue("de"));
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameTranslations_ReturnsTrue()
    {
        // Arrange
        var content1 = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["en"] = "Hello",
            ["de"] = "Hallo"
        });
        var content2 = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["en"] = "Hello",
            ["de"] = "Hallo"
        });

        // Assert
        Assert.Equal(content1, content2);
        Assert.True(content1.Equals(content2));
    }

    [Fact]
    public void Equals_WithDifferentTranslations_ReturnsFalse()
    {
        // Arrange
        var content1 = LocalizedContent.Create("en", "Hello");
        var content2 = LocalizedContent.Create("en", "Hi");

        // Assert
        Assert.NotEqual(content1, content2);
    }

    [Fact]
    public void GetHashCode_SameTranslations_ReturnsSameHash()
    {
        // Arrange
        var content1 = LocalizedContent.Create("en", "Hello");
        var content2 = LocalizedContent.Create("en", "Hello");

        // Assert
        Assert.Equal(content1.GetHashCode(), content2.GetHashCode());
    }

    #endregion

    #region GetSearchableText Tests

    [Fact]
    public void GetSearchableText_ReturnsAllTranslations()
    {
        // Arrange
        var content = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["en"] = "Hello World",
            ["de"] = "Hallo Welt"
        });

        // Act
        var searchable = content.GetSearchableText();

        // Assert
        Assert.Contains("Hello World", searchable);
        Assert.Contains("Hallo Welt", searchable);
    }

    [Fact]
    public void GetSearchableText_EmptyContent_ReturnsEmpty()
    {
        // Arrange
        var content = LocalizedContent.Empty;

        // Act
        var searchable = content.GetSearchableText();

        // Assert
        Assert.Empty(searchable);
    }

    #endregion

    #region Hybrid Pattern Integration Tests

    [Fact]
    public void HybridPattern_FallbackToDefault_WhenTranslationMissing()
    {
        // Arrange - Simulates entity pattern: Name (default) + NameTranslations
        var defaultName = "Default Product Name";
        var nameTranslations = LocalizedContent.Create(new Dictionary<string, string>
        {
            ["de"] = "Deutscher Produktname",
            ["fr"] = "Nom du produit français"
        });

        // Act - Simulate GetLocalizedName() helper
        string GetLocalizedName(string lang) => nameTranslations.GetValue(lang) ?? defaultName;

        // Assert
        Assert.Equal("Deutscher Produktname", GetLocalizedName("de"));
        Assert.Equal("Nom du produit français", GetLocalizedName("fr"));
        Assert.Equal("Default Product Name", GetLocalizedName("es")); // Fallback
        Assert.Equal("Default Product Name", GetLocalizedName("en")); // Fallback
    }

    [Fact]
    public void HybridPattern_NullTranslations_FallbackToDefault()
    {
        // Arrange - Simulates entity with no translations
        var defaultName = "Default Product Name";
        LocalizedContent? nameTranslations = null;

        // Act - Simulate GetLocalizedName() helper
        string GetLocalizedName(string lang) => nameTranslations?.GetValue(lang) ?? defaultName;

        // Assert
        Assert.Equal("Default Product Name", GetLocalizedName("de"));
        Assert.Equal("Default Product Name", GetLocalizedName("en"));
    }

    #endregion
}
