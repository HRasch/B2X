using B2X.Catalog.ImportAdapters;
using B2X.Catalog.ImportAdapters.Formats;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Catalog.Tests.ImportAdapters;

/// <summary>
/// Unit tests for DatanormImportAdapter
/// Tests German wholesale catalog format parsing
/// </summary>
public class DatanormImportAdapterTests
{
    private readonly DatanormImportAdapter _adapter;
    private readonly Mock<ILogger<DatanormImportAdapter>> _loggerMock;

    public DatanormImportAdapterTests()
    {
        _loggerMock = new Mock<ILogger<DatanormImportAdapter>>();
        _adapter = new DatanormImportAdapter(_loggerMock.Object);
    }

    private static string LoadFixture(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "fixtures", "catalogs", fileName);

        if (!File.Exists(path))
        {
            path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "fixtures", "catalogs", fileName);
        }

        return File.ReadAllText(path);
    }

    [Fact]
    public void FormatId_ShouldBe_Datanorm()
    {
        Assert.Equal("datanorm", _adapter.FormatId);
    }

    [Fact]
    public void FormatName_ShouldBe_Datanorm()
    {
        Assert.Equal("Datanorm", _adapter.FormatName);
    }

    [Fact]
    public void SupportedExtensions_ShouldContain_ExpectedExtensions()
    {
        Assert.Contains(".txt", _adapter.SupportedExtensions);
        Assert.Contains(".dn", _adapter.SupportedExtensions);
        Assert.Contains(".datanorm", _adapter.SupportedExtensions);
    }

    [Theory]
    [InlineData("catalog.dn")]
    [InlineData("catalog.datanorm")]
    [InlineData("catalog.txt")]
    public void DetectFormat_WithKnownExtension_ShouldReturn_PositiveConfidence(string fileName)
    {
        var content = "0KATALOG              20260103Lieferant              EUR\n1SKU001";
        var confidence = _adapter.DetectFormat(content, fileName);
        Assert.True(confidence > 0.0, $"Should detect Datanorm for {fileName}");
    }

    [Fact]
    public void DetectFormat_WithDatanormContent_ShouldDetect()
    {
        // Datanorm content - lines starting with digits
        var content = "0KATALOG              20260103Lieferant              EUR\n" +
                      "1SKU001   ART-001                     Test Product\n" +
                      "2SKU001   0100009999EUR1234567890123ManufacturerMFG-001\n" +
                      "9END";

        var confidence = _adapter.DetectFormat(content, "catalog.txt");
        Assert.True(confidence > 0.5, "Should detect Datanorm format");
    }

    [Fact]
    public async Task ValidateAsync_WithValidDatanorm_ShouldSucceed()
    {
        // Arrange
        var content = LoadFixture("example-datanorm.txt");

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Errors.Any(),
            $"Should not have errors: {string.Join(", ", result.Errors.Select(e => e.Message))}");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingHeader_ShouldReturnError()
    {
        // Arrange - Datanorm without header record (type 0)
        var content = "1SKU001   ART-001                     Test Product\n9END";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Any(e => e.Code.Contains("HEADER")),
            "Should have error about missing header");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingFooter_ShouldReturnWarning()
    {
        // Arrange - Datanorm without footer record (type 9)
        var content = "0KATALOG              20260103Lieferant              EUR\n" +
                      "1SKU001   ART-001                     Test Product";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Warnings.Any(w => w.Code.Contains("FOOTER")),
            "Should have warning about missing footer");
    }

    [Fact]
    public async Task ValidateAsync_WithNoArticles_ShouldReturnError()
    {
        // Arrange - Header and footer only
        var content = "0KATALOG              20260103Lieferant              EUR\n" +
                      "9Lieferant              20260103";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Any(e => e.Code.Contains("ARTICLE")),
            "Should have error about missing articles");
    }

    [Fact]
    public async Task ParseAsync_WithValidDatanorm_ShouldReturnEntities()
    {
        // Arrange
        var content = LoadFixture("example-datanorm.txt");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "DATANORM-TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Entities.Count > 0, "Should parse at least one entity from fixture");
        Assert.True(result.Statistics.TotalItems > 0);
    }

    [Fact]
    public async Task ParseAsync_ShouldExtract_ArticleDetails()
    {
        // Arrange
        var content = LoadFixture("example-datanorm.txt");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "DATANORM-TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        var firstEntity = result.Entities[0];
        Assert.NotNull(firstEntity);
        Assert.False(string.IsNullOrEmpty(firstEntity.ExternalId), "Should have ExternalId");
        Assert.False(string.IsNullOrEmpty(firstEntity.Name), "Should have Name");
    }

    [Fact]
    public async Task ParseAsync_WithEmptyContent_ShouldReturnEmptyResult()
    {
        // Arrange
        var content = "";
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
