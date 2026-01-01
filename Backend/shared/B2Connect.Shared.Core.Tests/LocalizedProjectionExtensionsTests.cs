using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2Connect.Shared.Core;
using B2Connect.Shared.Core.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace B2Connect.Shared.Core.Tests;

public class LocalizedProjectionExtensionsTests
{
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    public async Task SelectLocalized_WithAttribute_ReturnsLocalizedDto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);

        var product = new TestProduct
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "TEST-001",
            Name = "Test Product",
            NameTranslations = new LocalizedContent(new Dictionary<string, string>
            {
                ["de"] = "Test Produkt"
            }),
            Price = 99.99m
        };

        await context.TestProducts.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        var result = await context.TestProducts
            .SelectLocalized<TestProduct, LocalizedTestProductDto>("de")
            .FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Sku.Should().Be("TEST-001");
        result.Name.Should().Be("Test Produkt"); // German translation
        result.Price.Should().Be(99.99m);
    }

    [Fact]
    public async Task SelectLocalized_WithoutTranslations_ReturnsDefaultValue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);

        var product = new TestProduct
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "TEST-002",
            Name = "Default Product",
            NameTranslations = null, // No translations
            Price = 49.99m
        };

        await context.TestProducts.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        var result = await context.TestProducts
            .SelectLocalized<TestProduct, LocalizedTestProductDto>("de")
            .FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Default Product"); // Default value
    }

    // Test entities and DTOs
    private class TestProduct : ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public LocalizedContent? NameTranslations { get; set; }
        public decimal Price { get; set; }
    }

    private class LocalizedTestProductDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;

        [Localizable("Name", "NameTranslations")]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestProduct> TestProducts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestProduct>(entity =>
            {
                entity.OwnsOne(p => p.NameTranslations, t => t.ToJson("NameTranslations"));
            });
        }
    }
}