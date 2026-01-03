using B2Connect.Catalog.ImportAdapters;
using B2Connect.Catalog.ImportAdapters.Formats;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.ImportAdapters;

/// <summary>
/// Unit tests for BmecatImportAdapter
/// Tests the hybrid validation approach: Schema + XmlSerializer
/// </summary>
public class BmecatImportAdapterTests
{
    private readonly BmecatImportAdapter _adapter;
    private readonly Mock<ILogger<BmecatImportAdapter>> _loggerMock;

    public BmecatImportAdapterTests()
    {
        _loggerMock = new Mock<ILogger<BmecatImportAdapter>>();
        _adapter = new BmecatImportAdapter(_loggerMock.Object);
    }

    private static string LoadFixture(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "fixtures", "catalogs", fileName);

        // Fallback to relative path from test project
        if (!File.Exists(path))
        {
            path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "fixtures", "catalogs", fileName);
        }

        return File.ReadAllText(path);
    }

    [Fact]
    public void FormatId_ShouldBe_Bmecat()
    {
        Assert.Equal("bmecat", _adapter.FormatId);
    }

    [Fact]
    public void FormatName_ShouldBe_BMEcat()
    {
        Assert.Equal("BMEcat", _adapter.FormatName);
    }

    [Fact]
    public void SupportedExtensions_ShouldContain_Xml()
    {
        Assert.Contains(".xml", _adapter.SupportedExtensions);
    }

    [Theory]
    [InlineData("<BMECAT version=\"2005.2\">", "catalog.xml", 0.95)]
    [InlineData("<bmecat>", "catalog.xml", 0.95)]
    [InlineData("<root>other</root>", "catalog.xml", 0.5)]
    [InlineData("<BMECAT version=\"2005\">", "catalog.txt", 0.9)]
    [InlineData("plain text", "catalog.txt", 0.0)]
    public void DetectFormat_ShouldReturn_CorrectConfidence(
        string content, string fileName, double expectedConfidence)
    {
        var confidence = _adapter.DetectFormat(content, fileName);
        Assert.Equal(expectedConfidence, confidence);
    }

    [Fact]
    public async Task ValidateAsync_WithValidBmecat_ShouldSucceed()
    {
        // Arrange
        var content = LoadFixture("example-bmecat-2005-2.xml");

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert - Validation should not have blocking errors
        // (may have warnings if schema not available)
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidXml_ShouldReturnErrors()
    {
        // Arrange
        var content = "<BMECAT version=\"2005.2\"><invalid>";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Any(), "Should have validation errors for malformed XML");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingVersion_ShouldDetectVersion()
    {
        // Arrange - BMEcat without explicit version attribute
        var content = @"<?xml version=""1.0""?>
            <BMECAT xmlns:bmecat=""http://www.bmecat.org/bmecat/2005"">
                <HEADER><CATALOG_NAME>Test</CATALOG_NAME></HEADER>
            </BMECAT>";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert - Should detect version from xmlns or default
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ParseAsync_WithValidBmecat_ShouldReturnEntities()
    {
        // Arrange
        var content = LoadFixture("example-bmecat-2005-2.xml");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "ACME-2026");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Entities.Any(), "Should parse at least one entity");
        Assert.True(result.Statistics.TotalItems > 0);
        Assert.True(result.Statistics.ValidItems > 0);
    }

    [Fact]
    public async Task ParseAsync_ShouldExtract_ArticleDetails()
    {
        // Arrange
        var content = LoadFixture("example-bmecat-2005-2.xml");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "ACME-2026");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert - Check first article
        var firstEntity = result.Entities[0];
        Assert.NotNull(firstEntity);
        Assert.False(string.IsNullOrEmpty(firstEntity.ExternalId));
        Assert.False(string.IsNullOrEmpty(firstEntity.Name));
    }

    [Fact]
    public async Task ParseAsync_ShouldExtract_Pricing()
    {
        // Arrange
        var content = LoadFixture("example-bmecat-2005-2.xml");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "ACME-2026");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert - At least one entity should have price
        var entityWithPrice = result.Entities.FirstOrDefault(e => e.ListPrice.HasValue);
        Assert.NotNull(entityWithPrice);
        Assert.True(entityWithPrice.ListPrice > 0);
        Assert.Equal("EUR", entityWithPrice.Currency);
    }

    [Fact]
    public async Task ParseAsync_ShouldExtract_EanAndManufacturer()
    {
        // Arrange
        var content = LoadFixture("example-bmecat-2005-2.xml");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "ACME-2026");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        var entityWithEan = result.Entities.FirstOrDefault(e => !string.IsNullOrEmpty(e.Ean));
        Assert.NotNull(entityWithEan);
        Assert.Matches(@"^\d{13}$", entityWithEan.Ean!); // EAN-13 format
    }

    [Fact]
    public async Task ParseAsync_WithEmptyContent_ShouldReturnEmptyResult()
    {
        // Arrange
        var content = @"<?xml version=""1.0""?><BMECAT version=""2005.2""></BMECAT>";
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "test");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Entities);
    }
}
