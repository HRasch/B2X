using Xunit;
using B2Connect.Admin.Core.Entities;
using B2Connect.Shared.Core;

namespace B2Connect.Shared.Core.Tests;

/// <summary>
/// Tests for Hybrid Localization Pattern in Admin Entities.
/// Verifies GetLocalized* helper methods and fallback behavior.
/// </summary>
public class HybridLocalizationEntityTests
{
    #region Product Entity Tests

    [Fact]
    public void Product_GetLocalizedName_ReturnsTranslation_WhenExists()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "TEST-001",
            Slug = "test-product",
            Name = "Test Product",
            NameTranslations = LocalizedContent.Create(new Dictionary<string, string>
            {
                ["de"] = "Testprodukt",
                ["fr"] = "Produit de test"
            })
        };

        // Act & Assert
        Assert.Equal("Testprodukt", product.GetLocalizedName("de"));
        Assert.Equal("Produit de test", product.GetLocalizedName("fr"));
    }

    [Fact]
    public void Product_GetLocalizedName_FallbacksToDefault_WhenTranslationMissing()
    {
        // Arrange
        var product = new Product
        {
            Name = "Default Name",
            NameTranslations = LocalizedContent.Create("de", "Deutscher Name")
        };

        // Act
        var result = product.GetLocalizedName("es"); // Spanish not available

        // Assert
        Assert.Equal("Default Name", result);
    }

    [Fact]
    public void Product_GetLocalizedName_FallbacksToDefault_WhenTranslationsNull()
    {
        // Arrange
        var product = new Product
        {
            Name = "Default Name",
            NameTranslations = null
        };

        // Act
        var result = product.GetLocalizedName("de");

        // Assert
        Assert.Equal("Default Name", result);
    }

    [Fact]
    public void Product_GetLocalizedDescription_ReturnsTranslation()
    {
        // Arrange
        var product = new Product
        {
            Description = "Default description",
            DescriptionTranslations = LocalizedContent.Create("de", "Deutsche Beschreibung")
        };

        // Act & Assert
        Assert.Equal("Deutsche Beschreibung", product.GetLocalizedDescription("de"));
        Assert.Equal("Default description", product.GetLocalizedDescription("en"));
    }

    [Fact]
    public void Product_GetLocalizedShortDescription_ReturnsTranslation()
    {
        // Arrange
        var product = new Product
        {
            ShortDescription = "Short desc",
            ShortDescriptionTranslations = LocalizedContent.Create("de", "Kurzbeschreibung")
        };

        // Act & Assert
        Assert.Equal("Kurzbeschreibung", product.GetLocalizedShortDescription("de"));
        Assert.Equal("Short desc", product.GetLocalizedShortDescription("fr"));
    }

    #endregion

    #region Category Entity Tests

    [Fact]
    public void Category_GetLocalizedName_ReturnsTranslation()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Slug = "electronics",
            Name = "Electronics",
            NameTranslations = LocalizedContent.Create(new Dictionary<string, string>
            {
                ["de"] = "Elektronik",
                ["fr"] = "Électronique"
            })
        };

        // Act & Assert
        Assert.Equal("Elektronik", category.GetLocalizedName("de"));
        Assert.Equal("Électronique", category.GetLocalizedName("fr"));
        Assert.Equal("Electronics", category.GetLocalizedName("es")); // Fallback
    }

    [Fact]
    public void Category_GetLocalizedDescription_ReturnsTranslation()
    {
        // Arrange
        var category = new Category
        {
            Description = "All electronic products",
            DescriptionTranslations = LocalizedContent.Create("de", "Alle elektronischen Produkte")
        };

        // Act
        var result = category.GetLocalizedDescription("de");

        // Assert
        Assert.Equal("Alle elektronischen Produkte", result);
    }

    #endregion

    #region Brand Entity Tests

    [Fact]
    public void Brand_GetLocalizedName_ReturnsTranslation()
    {
        // Arrange
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Slug = "apple",
            Name = "Apple Inc.",
            NameTranslations = LocalizedContent.Create("de", "Apple GmbH")
        };

        // Act & Assert
        Assert.Equal("Apple GmbH", brand.GetLocalizedName("de"));
        Assert.Equal("Apple Inc.", brand.GetLocalizedName("en")); // Fallback
    }

    [Fact]
    public void Brand_GetLocalizedDescription_FallbacksToDefault()
    {
        // Arrange
        var brand = new Brand
        {
            Description = "Technology company",
            DescriptionTranslations = null
        };

        // Act
        var result = brand.GetLocalizedDescription("de");

        // Assert
        Assert.Equal("Technology company", result);
    }

    #endregion

    #region ProductVariant Entity Tests

    [Fact]
    public void ProductVariant_GetLocalizedName_ReturnsTranslation()
    {
        // Arrange
        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            Sku = "VAR-001",
            Name = "Black, 256GB",
            NameTranslations = LocalizedContent.Create(new Dictionary<string, string>
            {
                ["de"] = "Schwarz, 256GB",
                ["fr"] = "Noir, 256Go"
            })
        };

        // Act & Assert
        Assert.Equal("Schwarz, 256GB", variant.GetLocalizedName("de"));
        Assert.Equal("Noir, 256Go", variant.GetLocalizedName("fr"));
        Assert.Equal("Black, 256GB", variant.GetLocalizedName("es"));
    }

    #endregion

    #region ProductDocument Entity Tests

    [Fact]
    public void ProductDocument_GetLocalizedName_ReturnsTranslation()
    {
        // Arrange
        var document = new ProductDocument
        {
            Id = Guid.NewGuid(),
            Name = "User Manual",
            NameTranslations = LocalizedContent.Create("de", "Benutzerhandbuch"),
            DocumentType = "manual",
            Url = "https://example.com/manual.pdf"
        };

        // Act & Assert
        Assert.Equal("Benutzerhandbuch", document.GetLocalizedName("de"));
        Assert.Equal("User Manual", document.GetLocalizedName("en"));
    }

    #endregion

    #region ProductAttribute Entity Tests

    [Fact]
    public void ProductAttribute_GetLocalizedName_ReturnsTranslation()
    {
        // Arrange
        var attribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Code = "color",
            Name = "Color",
            NameTranslations = LocalizedContent.Create(new Dictionary<string, string>
            {
                ["de"] = "Farbe",
                ["fr"] = "Couleur"
            })
        };

        // Act & Assert
        Assert.Equal("Farbe", attribute.GetLocalizedName("de"));
        Assert.Equal("Couleur", attribute.GetLocalizedName("fr"));
        Assert.Equal("Color", attribute.GetLocalizedName("es"));
    }

    [Fact]
    public void ProductAttributeOption_GetLocalizedLabel_ReturnsTranslation()
    {
        // Arrange
        var option = new ProductAttributeOption
        {
            Id = Guid.NewGuid(),
            Code = "red",
            Label = "Red",
            LabelTranslations = LocalizedContent.Create(new Dictionary<string, string>
            {
                ["de"] = "Rot",
                ["fr"] = "Rouge"
            })
        };

        // Act & Assert
        Assert.Equal("Rot", option.GetLocalizedLabel("de"));
        Assert.Equal("Rouge", option.GetLocalizedLabel("fr"));
        Assert.Equal("Red", option.GetLocalizedLabel("es"));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void AllEntities_HandleCaseInsensitiveLanguageCodes()
    {
        // Arrange
        var product = new Product
        {
            Name = "Default",
            NameTranslations = LocalizedContent.Create("DE", "Deutsch")
        };

        // Act & Assert
        Assert.Equal("Deutsch", product.GetLocalizedName("de"));
        Assert.Equal("Deutsch", product.GetLocalizedName("DE"));
        Assert.Equal("Deutsch", product.GetLocalizedName("De"));
    }

    [Fact]
    public void AllEntities_HandleEmptyTranslationValues()
    {
        // Arrange
        var product = new Product
        {
            Name = "Default Name",
            NameTranslations = LocalizedContent.Create("de", "")
        };

        // Act - Empty string IS a valid translation (returns empty, not fallback)
        var result = product.GetLocalizedName("de");

        // Assert
        Assert.Equal("", result);
    }

    #endregion
}
