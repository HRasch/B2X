using B2Connect.Catalog.ImportAdapters;
using B2Connect.Catalog.ImportAdapters.Formats;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.ImportAdapters;

/// <summary>
/// Unit tests for CsvImportAdapter
/// Tests CSV parsing with various delimiters and column formats
/// </summary>
public class CsvImportAdapterTests
{
    private readonly CsvImportAdapter _adapter;
    private readonly Mock<ILogger<CsvImportAdapter>> _loggerMock;

    public CsvImportAdapterTests()
    {
        _loggerMock = new Mock<ILogger<CsvImportAdapter>>();
        _adapter = new CsvImportAdapter(_loggerMock.Object);
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
    public void FormatId_ShouldBe_Csv()
    {
        Assert.Equal("csv", _adapter.FormatId);
    }

    [Fact]
    public void FormatName_ShouldBe_CSV()
    {
        Assert.Equal("CSV", _adapter.FormatName);
    }

    [Fact]
    public void SupportedExtensions_ShouldContain_CsvAndTxt()
    {
        Assert.Contains(".csv", _adapter.SupportedExtensions);
        Assert.Contains(".txt", _adapter.SupportedExtensions);
    }

    [Theory]
    [InlineData("sku,name,price\nSKU-001,Product,99.99", "products.csv")]
    [InlineData("sku;name;price\nSKU-001;Product;99.99", "products.csv")]
    public void DetectFormat_WithCsvExtension_ShouldReturn_HighConfidence(
        string content, string fileName)
    {
        var confidence = _adapter.DetectFormat(content, fileName);
        Assert.True(confidence >= 0.9, "Should have high confidence for .csv files");
    }

    [Fact]
    public void DetectFormat_WithCsvContent_ShouldDetect()
    {
        var content = "sku,name,price\nSKU-001,Product One,99.99\nSKU-002,Product Two,149.99";
        var confidence = _adapter.DetectFormat(content, "data.txt");
        Assert.True(confidence > 0.5, "Should detect CSV format from content structure");
    }

    [Fact]
    public async Task ValidateAsync_WithValidCsv_ShouldSucceed()
    {
        // Arrange
        var content = LoadFixture("example-products.csv");

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Errors.Any(),
            $"Should not have errors: {string.Join(", ", result.Errors.Select(e => e.Message))}");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingRequiredColumns_ShouldReturnError()
    {
        // Arrange - CSV without sku/article_number column
        var content = "name,price\nProduct,99.99";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Any(),
            "Should have error about missing required column");
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyCsv_ShouldReturnError()
    {
        // Arrange
        var content = "";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Any(), "Should have error for empty content");
    }

    [Fact]
    public async Task ValidateAsync_WithHeaderOnly_ShouldReturnIssue()
    {
        // Arrange
        var content = "sku,name,price";

        // Act
        var result = await _adapter.ValidateAsync(content);

        // Assert
        Assert.NotNull(result);
        // Either error or warning about no data rows
        Assert.True(result.Errors.Any() || result.Warnings.Any(),
            "Should have error or warning about no data");
    }

    [Fact]
    public async Task ParseAsync_WithValidCsv_ShouldReturnEntities()
    {
        // Arrange
        var content = LoadFixture("example-products.csv");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "CSV-TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Entities.Count > 0, "Should parse entities from fixture");
        Assert.True(result.Statistics.TotalItems > 0);
    }

    [Fact]
    public async Task ParseAsync_ShouldDetect_SemicolonDelimiter()
    {
        // Arrange - Semicolon-delimited CSV (German format)
        var content = "sku;name;price\nSKU-001;Product One;99.99\nSKU-002;Product Two;149.99";
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.True(result.Entities.Count >= 2, "Should parse semicolon-delimited CSV");
    }

    [Fact]
    public async Task ParseAsync_ShouldDetect_CommaDelimiter()
    {
        // Arrange - Comma-delimited CSV
        var content = "sku,name,price\nSKU-001,Product One,99.99\nSKU-002,Product Two,149.99";
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        Assert.True(result.Entities.Count >= 2, "Should parse comma-delimited CSV");
    }

    [Fact]
    public async Task ParseAsync_ShouldExtract_AllFields()
    {
        // Arrange
        var content = LoadFixture("example-products.csv");
        var metadata = new ImportMetadata(
            TenantId: "test-tenant",
            SupplierId: Guid.NewGuid().ToString(),
            SourceIdentifier: "CSV-TEST");

        // Act
        var result = await _adapter.ParseAsync(content, metadata);

        // Assert
        var entity = result.Entities[0];
        Assert.NotNull(entity);
        Assert.False(string.IsNullOrEmpty(entity.ExternalId), "Should have ExternalId (sku)");
        Assert.False(string.IsNullOrEmpty(entity.Name), "Should have Name");
    }
}
