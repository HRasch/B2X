using B2X.Types.Localization;
using B2X.Types.Utilities;
using Xunit;

namespace B2X.LocalizationService.Tests.EntityLocalization;

public class LocalizedContentTests
{
    [Fact]
    public void Set_AddsTranslation()
    {
        var content = new LocalizedContent();
        content.Set("en", "Hello");

        Assert.Equal("Hello", content.Get("en"));
    }

    [Fact]
    public void Set_WithChaining()
    {
        var content = new LocalizedContent()
            .Set("en", "English")
            .Set("de", "Deutsch")
            .Set("fr", "Français");

        Assert.Equal("English", content.Get("en"));
        Assert.Equal("Deutsch", content.Get("de"));
        Assert.Equal("Français", content.Get("fr"));
    }

    [Fact]
    public void Get_ReturnsFallbackWhenLanguageNotFound()
    {
        var content = new LocalizedContent();
        content.Set("en", "Default")
               .Set("de", "Deutsch");

        var spanish = content.Get("es");
        Assert.Equal("Default", spanish);
    }

    [Fact]
    public void Get_WithNullLanguageReturnsFallback()
    {
        var content = new LocalizedContent();
        content.Set("en", "English");

        var result = content.Get(null);
        Assert.Equal("English", result);
    }

    [Fact]
    public void SetMany_AddsMultipleTranslations()
    {
        var content = new LocalizedContent();
        var translations = new Dictionary<string, string>
        {
            { "en", "English" },
            { "de", "Deutsch" },
            { "fr", "Français" }
        };

        content.SetMany(translations);

        Assert.Equal(3, content.Count);
        Assert.Equal("Deutsch", content.Get("de"));
    }

    [Fact]
    public void HasTranslation_ReturnsTrueWhenExists()
    {
        var content = new LocalizedContent();
        content.Set("de", "Deutsch");

        Assert.True(content.HasTranslation("de"));
        Assert.False(content.HasTranslation("fr"));
    }

    [Fact]
    public void GetAvailableLanguages_ReturnsAllLanguages()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch")
               .Set("fr", "Français");

        var languages = content.GetAvailableLanguages().ToList();
        Assert.Contains("en", languages);
        Assert.Contains("de", languages);
        Assert.Contains("fr", languages);
        Assert.Equal(3, languages.Count);
    }

    [Fact]
    public void IsEmpty_ReturnsTrueWhenNoTranslations()
    {
        var content = new LocalizedContent();
        Assert.True(content.IsEmpty);

        content.Set("en", "English");
        Assert.False(content.IsEmpty);
    }

    [Fact]
    public void Remove_RemovesTranslation()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch");

        Assert.True(content.Remove("de"));
        Assert.False(content.HasTranslation("de"));
        Assert.Single(content.GetAvailableLanguages());
    }

    [Fact]
    public void Clear_RemovesAllTranslations()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch");

        content.Clear();

        Assert.Empty(content.GetAvailableLanguages());
        Assert.True(content.IsEmpty);
    }

    [Fact]
    public void FromJson_DeserializesValidJson()
    {
        var json = @"{
            ""translations"": {
                ""en"": ""English"",
                ""de"": ""Deutsch""
            },
            ""defaultLanguage"": ""en""
        }";

        var content = LocalizedContent.FromJson(json);

        Assert.Equal("English", content.Get("en"));
        Assert.Equal("Deutsch", content.Get("de"));
        Assert.Equal("en", content.DefaultLanguage);
    }

    [Fact]
    public void FromJson_ReturnsEmptyForInvalidJson()
    {
        var content = LocalizedContent.FromJson("invalid json");
        Assert.NotNull(content);
        Assert.True(content.IsEmpty);
    }

    [Fact]
    public void ToJson_SerializesCorrectly()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch");

        var json = content.ToJson();

        Assert.Contains("en", json);
        Assert.Contains("de", json);
        Assert.Contains("English", json);
        Assert.Contains("Deutsch", json);
    }

    [Fact]
    public void Clone_CreatesIndependentCopy()
    {
        var original = new LocalizedContent();
        original.Set("en", "English");

        var clone = original.Clone();
        clone.Set("de", "Deutsch");

        Assert.False(original.HasTranslation("de"));
        Assert.True(clone.HasTranslation("de"));
    }

    [Fact]
    public void Merge_CombinesTranslations()
    {
        var content1 = new LocalizedContent();
        content1.Set("en", "English")
                .Set("de", "Deutsch");

        var content2 = new LocalizedContent();
        content2.Set("fr", "Français")
                .Set("de", "Deutsch (updated)");

        content1.Merge(content2);

        Assert.Equal(3, content1.Count);
        Assert.Equal("Deutsch (updated)", content1.Get("de"));
        Assert.Equal("Français", content1.Get("fr"));
    }

    [Fact]
    public void GetMany_ReturnsMultipleLanguages()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch")
               .Set("fr", "Français");

        var result = content.GetMany("en", "de", "fr");

        Assert.Equal(3, result.Count);
        Assert.Equal("English", result["en"]);
        Assert.Equal("Deutsch", result["de"]);
        Assert.Equal("Français", result["fr"]);
    }

    [Fact]
    public void HasAllLanguages_ValidatesAllLanguagesPresent()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch")
               .Set("fr", "Français");

        Assert.True(content.HasAllLanguages("en", "de", "fr"));
        Assert.False(content.HasAllLanguages("en", "de", "es"));
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch");

        var str = content.ToString();

        Assert.Contains("en", str);
        Assert.Contains("de", str);
        Assert.Contains("English", str);
        Assert.Contains("Deutsch", str);
    }

    [Fact]
    public void CaseInsensitivity_NormalizesLanguageCodes()
    {
        var content = new LocalizedContent();
        content.Set("EN", "English");

        Assert.Equal("English", content.Get("en"));
        Assert.Equal("English", content.Get("EN"));
        Assert.True(content.HasTranslation("eN"));
    }
}

public class LocalizationJsonUtilityTests
{
    [Fact]
    public void Serialize_ProducesValidJson()
    {
        var content = new LocalizedContent();
        content.Set("en", "English")
               .Set("de", "Deutsch");

        var json = LocalizationJsonUtility.Serialize(content);

        Assert.NotNull(json);
        Assert.Contains("translations", json);
        Assert.Contains("defaultLanguage", json);
    }

    [Fact]
    public void Deserialize_RestoresContent()
    {
        var original = new LocalizedContent();
        original.Set("en", "English")
                .Set("de", "Deutsch");

        var json = LocalizationJsonUtility.Serialize(original);
        var restored = LocalizationJsonUtility.Deserialize(json);

        Assert.Equal("English", restored.Get("en"));
        Assert.Equal("Deutsch", restored.Get("de"));
    }

    [Fact]
    public void TryDeserialize_ReturnsTrueForValidJson()
    {
        var json = @"{ ""translations"": { ""en"": ""English"" }, ""defaultLanguage"": ""en"" }";

        var result = LocalizationJsonUtility.TryDeserialize(json, out var content);

        Assert.True(result);
        Assert.NotNull(content);
        Assert.Equal("English", content.Get("en"));
    }

    [Fact]
    public void TryDeserialize_ReturnsFalseForInvalidJson()
    {
        var result = LocalizationJsonUtility.TryDeserialize("invalid", out _);
        Assert.False(result);
    }

    [Fact]
    public void IsValidLocalizedJson_ValidatesFormat()
    {
        var validJson = @"{ ""translations"": { ""en"": ""test"" } }";
        var invalidJson = "not json";

        Assert.True(LocalizationJsonUtility.IsValidLocalizedJson(validJson));
        Assert.False(LocalizationJsonUtility.IsValidLocalizedJson(invalidJson));
    }

    [Fact]
    public void MergeJsonStrings_CombinesMultipleJsons()
    {
        var json1 = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string> { { "en", "English" }, { "de", "Deutsch" } });
        var json2 = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string> { { "fr", "Français" } });

        var merged = LocalizationJsonUtility.MergeJsonStrings(json1, json2);
        var result = LocalizationJsonUtility.Deserialize(merged);

        Assert.Equal(3, result.Count);
        Assert.True(result.HasTranslation("en"));
        Assert.True(result.HasTranslation("fr"));
    }

    [Fact]
    public void ExtractLanguages_FiltersLanguages()
    {
        var json = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string>
            {
                { "en", "English" },
                { "de", "Deutsch" },
                { "fr", "Français" },
                { "es", "Español" }
            });

        var extracted = LocalizationJsonUtility.ExtractLanguages(json, "en", "de");
        var result = LocalizationJsonUtility.Deserialize(extracted);

        Assert.Equal(2, result.Count);
        Assert.True(result.HasTranslation("en"));
        Assert.True(result.HasTranslation("de"));
        Assert.False(result.HasTranslation("fr"));
    }

    private static readonly string[] requiredLanguages = new[] { "en", "de", "fr" };

    [Fact]
    public void FillMissingLanguages_AddsDefaults()
    {
        var json = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string> { { "en", "English" } });

        var filled = LocalizationJsonUtility.FillMissingLanguages(json, requiredLanguages, "missing");
        var result = LocalizationJsonUtility.Deserialize(filled);

        Assert.Equal(3, result.Count);
        Assert.Equal("English", result.Get("en"));
        Assert.Equal("missing", result.Get("de"));
        Assert.Equal("missing", result.Get("fr"));
    }

    [Fact]
    public void GetLanguagesFromJson_ReturnsAllLanguages()
    {
        var json = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string>
            {
                { "en", "English" },
                { "de", "Deutsch" },
                { "fr", "Français" }
            });

        var languages = LocalizationJsonUtility.GetLanguagesFromJson(json).ToList();

        Assert.Equal(3, languages.Count);
        Assert.Contains("en", languages);
        Assert.Contains("de", languages);
        Assert.Contains("fr", languages);
    }

    [Fact]
    public void GetStats_ProvidesSummary()
    {
        var json = LocalizationJsonUtility.SerializeDictionary(
            new Dictionary<string, string>
            {
                { "en", "English" },
                { "de", "Deutsch" }
            });

        var stats = LocalizationJsonUtility.GetStats(json);

        Assert.Equal(2, stats.TotalLanguages);
        Assert.Equal("en", stats.DefaultLanguage);
        Assert.False(stats.IsEmpty);
        Assert.True(stats.TotalCharacters > 0);
    }
}
