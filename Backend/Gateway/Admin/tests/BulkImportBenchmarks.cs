using System.Diagnostics;
using B2Connect.Admin.Application.Commands.Products;
using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using B2Connect.Types.Localization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.Tests.Benchmarks;

/// <summary>
/// Test implementation of ITenantContext for benchmarks
/// </summary>
internal class TestTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Parse("12345678-1234-1234-1234-123456789012");
}

/// <summary>
/// Performance Benchmarks für ADR-025: Dapper/EF Extensions Evaluation
/// Vergleicht EF Core vs EFCore.BulkExtensions für Bulk Product Imports
///
/// Ziel: Nachweis der 10-50x Performance-Verbesserung bei Bulk-Operationen
/// </summary>
[MemoryDiagnoser]
[SimpleJob(iterationCount: 3, warmupCount: 1)]
public class BulkImportBenchmarks : IDisposable
{
    private CatalogDbContext _dbContext = null!;
    private List<BulkImportProductItem> _testData = null!;

    [Params(100, 1000, 10000)]
    public int ProductCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // In-Memory Database für Benchmarks
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase($"Benchmark_{Guid.NewGuid()}")
            .Options;

        _dbContext = new CatalogDbContext(options, new TestTenantContext());

        // Testdaten generieren
        _testData = GenerateTestProducts(ProductCount);

        // Datenbank initialisieren
        _dbContext.Database.EnsureCreated();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    private List<BulkImportProductItem> GenerateTestProducts(int count)
    {
        var products = new List<BulkImportProductItem>();
        for (int i = 0; i < count; i++)
        {
            products.Add(new BulkImportProductItem(
                Name: $"Test Product {i}",
                Sku: $"SKU-{i:00000}",
                Price: 10.99m + (i % 100),
                Description: $"Description for product {i}",
                CategoryId: Guid.NewGuid(),
                BrandId: Guid.NewGuid()));
        }
        return products;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Benchmark: EF Core Individual Inserts
    // ─────────────────────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)]
    public async Task<int> EfCore_IndividualInserts()
    {
        var tenantId = Guid.NewGuid();
        var products = _testData.Select(item => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", item.Name),
            Sku = item.Sku,
            Price = item.Price,
            Description = item.Description != null
                ? new LocalizedContent().Set("en", item.Description)
                : null,
            CategoryId = item.CategoryId,
            BrandId = item.BrandId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        foreach (var product in products)
        {
            _dbContext.Products.Add(product);
        }

        var count = await _dbContext.SaveChangesAsync();
        return count;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Benchmark: EFCore.BulkExtensions Bulk Insert
    // ─────────────────────────────────────────────────────────────────────────

    [Benchmark]
    public async Task<int> BulkExtensions_BulkInsert()
    {
        var tenantId = Guid.NewGuid();
        var products = _testData.Select(item => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", item.Name),
            Sku = item.Sku,
            Price = item.Price,
            Description = item.Description != null
                ? new LocalizedContent().Set("en", item.Description)
                : null,
            CategoryId = item.CategoryId,
            BrandId = item.BrandId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        var bulkConfig = new BulkConfig
        {
            BatchSize = 1000,
            UseTempDB = true,
            CalculateStats = false // Performance-Optimierung für Benchmarks
        };

        await _dbContext.BulkInsertAsync(products, bulkConfig);
        return products.Count;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Benchmark: EFCore.BulkExtensions mit verschiedenen Batch-Größen
    // ─────────────────────────────────────────────────────────────────────────

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task<int> BulkExtensions_CustomBatchSize(int batchSize)
    {
        var tenantId = Guid.NewGuid();
        var products = _testData.Select(item => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", item.Name),
            Sku = item.Sku,
            Price = item.Price,
            Description = item.Description != null
                ? new LocalizedContent().Set("en", item.Description)
                : null,
            CategoryId = item.CategoryId,
            BrandId = item.BrandId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        var bulkConfig = new BulkConfig
        {
            BatchSize = batchSize,
            UseTempDB = true,
            CalculateStats = false
        };

        await _dbContext.BulkInsertAsync(products, bulkConfig);
        return products.Count;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
