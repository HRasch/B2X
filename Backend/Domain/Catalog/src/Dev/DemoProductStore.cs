using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace B2Connect.Catalog.Endpoints.Dev;

public static class DemoProductStore
{
    private static List<dynamic>? _products;
    private static readonly object _lock = new();

    public static void EnsureInitialized(int count = 100, string? demoSector = null)
    {
        if (_products != null) return;

        lock (_lock)
        {
            if (_products != null) return;

            var faker = new Faker();
            _products = new List<dynamic>(count);

            var sectors = new[] { "steel trading", "industry material supply", "sanitär", "construction", "automotive" };

            var productAreasBySector = new Dictionary<string, string[]>
            {
                ["steel trading"] = new[] { "Stahlträger", "Blech", "Profile", "Schrauben" },
                ["industry material supply"] = new[] { "Bohrmaschinen", "Schleifmaschinen", "Werkzeuge", "Dichtungen" },
                ["sanitär"] = new[] { "WC", "Waschtische", "Armaturen", "Duschköpfe" },
                ["construction"] = new[] { "Zement", "Mauersteine", "Dämmstoffe", "Gerüste" },
                ["automotive"] = new[] { "Batterien", "Bremsen", "Stoßdämpfer", "Filter" }
            };

            for (int i = 0; i < count; i++)
            {
                var sku = $"SKU-{i + 1:D8}";
                var name = faker.Commerce.ProductName();
                var price = decimal.Parse(faker.Commerce.Price(50, 5000));
                var tenantId = Guid.NewGuid();
                var sector = !string.IsNullOrWhiteSpace(demoSector) ? demoSector : faker.PickRandom(sectors);
                var brand = faker.Company.CompanyName();
                var desc = faker.Commerce.ProductDescription();
                var stock = faker.Random.Int(0, 2000);
                var isActive = faker.Random.Bool(0.95f);
                decimal? discount = faker.Random.Bool(0.1f) ? Math.Round(price * (decimal)faker.Random.Double(0.7, 0.95), 2) : (decimal?)null;
                var areas = productAreasBySector.ContainsKey(sector) ? productAreasBySector[sector] : new[] { "General" };
                var selectedArea = faker.PickRandom(areas);
                var categories = new[] { selectedArea };
                var tags = new[] { faker.Commerce.ProductAdjective(), selectedArea };

                _products.Add(new
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
                    // sector meta intentionally not included in DTOs; categories/tags reflect product area
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow,
                    isAvailable = isActive && stock > 0
                });
            }
        }
    }

    public static (IEnumerable<dynamic> Items, int Total) GetPage(int page, int pageSize)
    {
        if (_products == null) EnsureInitialized(100);
        var total = _products!.Count;
        var skip = (page - 1) * pageSize;
        var items = _products!.Skip(skip).Take(pageSize)
            .Select(p =>
            {
                // ensure categories and tags are non-null arrays for JSON serialization
                var cats = (p.categories as IEnumerable<string>)?.ToArray() ?? new[] { "general" };
                var tags = (p.tags as IEnumerable<string>)?.ToArray() ?? new[] { "general" };
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
                    createdAt = p.createdAt,
                    updatedAt = p.updatedAt,
                    isAvailable = p.isAvailable
                };
            });

        return (items, total);
    }
}
