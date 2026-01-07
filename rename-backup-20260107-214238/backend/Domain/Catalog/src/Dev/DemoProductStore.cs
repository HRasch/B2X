using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using B2X.Shared.Core;
using Bogus;

namespace B2X.Catalog.Endpoints.Dev;

public static class DemoProductStore
{
    private static List<dynamic>? _products;
    private static readonly Lock _lock = new();
    private static readonly string[] stringArray = new[] { "general" };

    public static void EnsureInitialized(int count = 100, string? demoSector = null, string[]? locales = null)
    {
        if (_products != null)
        {
            return;
        }

        lock (_lock)
        {
            if (_products != null)
            {
                return;
            }

            _products = GenerateDemoProducts(count, demoSector, locales);
        }
    }

    private static List<dynamic> GenerateDemoProducts(int count, string? demoSector, string[]? locales)
    {
        var faker = new Faker();
        var products = new List<dynamic>(count);

        var sectors = new[] { "steel trading", "industry material supply", "sanitär", "construction", "automotive" };

        // default locales: English and German
        locales ??= new[] { "en", "de" };

        var productAreasBySector = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["steel trading"] = new[] { "Stahlträger", "Blech", "Profile", "Schrauben" },
            ["industry material supply"] = new[] { "Bohrmaschinen", "Schleifmaschinen", "Werkzeuge", "Dichtungen" },
            ["sanitär"] = new[] { "WC", "Waschtische", "Armaturen", "Duschköpfe" },
            ["construction"] = new[] { "Zement", "Mauersteine", "Dämmstoffe", "Gerüste" },
            ["automotive"] = new[] { "Batterien", "Bremsen", "Stoßdämpfer", "Filter" }
        };

        for (int i = 0; i < count; i++)
        {
            var product = CreateDemoProduct(faker, i, demoSector, locales, sectors, productAreasBySector);
            products.Add(product);
        }

        return products;
    }

    private static dynamic CreateDemoProduct(Faker faker, int index, string? demoSector, string[] locales, string[] sectors, Dictionary<string, string[]> productAreasBySector)
    {
        var sku = $"SKU-{index + 1:D8}";
        var name = faker.Commerce.ProductName();
        var price = decimal.Parse(faker.Commerce.Price(50, 5000), CultureInfo.InvariantCulture);
        // Use the shared default tenant ID so products are visible in Store frontend
        var tenantId = SeedConstants.DefaultTenantId;
        var sector = !string.IsNullOrWhiteSpace(demoSector) ? demoSector : faker.PickRandom(sectors);
        var locale = faker.PickRandom(locales);
        var brand = faker.Company.CompanyName();
        var desc = faker.Commerce.ProductDescription();
        var stock = faker.Random.Int(0, 2000);
        var isActive = faker.Random.Bool(0.95f);
        decimal? discount = faker.Random.Bool(0.1f) ? Math.Round(price * (decimal)faker.Random.Double(0.7, 0.95), 2) : (decimal?)null;
        var areas = productAreasBySector.TryGetValue(sector, out string[]? value) ? value : new[] { "General" };
        var selectedArea = faker.PickRandom(areas);
        var categories = new[] { selectedArea };
        var tags = new[] { faker.Commerce.ProductAdjective(), selectedArea };

        return new
        {
            id = Guid.NewGuid(),
            sku = sku,
            name = name,
            description = desc,
            price = price,
            discountPrice = discount,
            stockQuantity = stock,
            isActive = isActive,
            categories = categories,
            brandName = brand,
            tags = tags,
            tenantId = tenantId,
            locale = locale,
            // sector meta intentionally not included in DTOs; categories/tags reflect product area
            createdAt = DateTime.UtcNow,
            updatedAt = DateTime.UtcNow,
            isAvailable = isActive && stock > 0
        };
    }

    public static (IEnumerable<dynamic> Items, int Total) GetPage(int page, int pageSize)
    {
        if (_products == null)
        {
            EnsureInitialized(100);
        }

        var total = _products!.Count;
        var skip = (page - 1) * pageSize;
        var items = _products!.Skip(skip).Take(pageSize)
            .Select(p =>
            {
                // ensure categories and tags are non-null arrays for JSON serialization
                var cats = (p.categories as IEnumerable<string>)?.ToArray() ?? stringArray;
                var tags = (p.tags as IEnumerable<string>)?.ToArray() ?? stringArray;
                return new
                {
                    id = p.id,
                    sku = p.sku,
                    name = p.name,
                    description = p.description,
                    price = p.price,
                    discountPrice = p.discountPrice,
                    stockQuantity = p.stockQuantity,
                    isActive = p.isActive,
                    categories = cats,
                    brandName = p.brandName,
                    tags = tags,
                    tenantId = p.tenantId,
                    locale = p.locale,
                    createdAt = p.createdAt,
                    updatedAt = p.updatedAt,
                    isAvailable = p.isAvailable
                };
            });

        return (items, total);
    }
}
