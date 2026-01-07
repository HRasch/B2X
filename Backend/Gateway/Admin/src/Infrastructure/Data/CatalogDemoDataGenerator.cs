using System.Globalization;
using B2Connect.Admin.Core.Entities;
using B2Connect.Shared.Core;
using B2Connect.Types.Localization;
using Bogus;
using LocalizedContent = B2Connect.Types.Localization.LocalizedContent;

namespace B2Connect.Admin.Infrastructure.Data;

/// <summary>
/// Demo data generator for development and testing using Bogus
/// Generates realistic product catalog data
/// </summary>
public static class CatalogDemoDataGenerator
{
    private static int _productCounter;
    private static int _brandCounter;

    /// <summary>
    /// Generates complete demo catalog data
    /// </summary>
    /// <param name="productCount">Number of products to generate (default: 100)</param>
    /// <param name="seed">Random seed for reproducible data (optional)</param>
    /// <returns>Complete catalog with categories, brands, products, variants, images, documents</returns>
    public static (List<Category> Categories, List<Brand> Brands, List<Product> Products)
        GenerateDemoCatalog(int productCount = 100, int? seed = null)
    {
        // Note: Randomizer seeding is optional and not used in this version
        // Seed functionality can be enabled by passing seed to individual Faker<T>() constructors if needed

        var categories = GenerateCategories();
        var brands = GenerateBrands();
        var products = GenerateProducts(productCount, categories, brands);

        return (categories, brands, products);
    }

    /// <summary>
    /// Generates category hierarchy (Electronics -> Laptops -> Gaming Laptops)
    /// </summary>
    private static List<Category> GenerateCategories()
    {
        var categories = new List<Category>();

        // Root Categories
        var electronics = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Slug = "electronics",
            Name = new LocalizedContent()
                .Set("en", "Electronics")
                .Set("de", "Elektronik")
                .Set("fr", "Électronique"),
            Description = new LocalizedContent()
                .Set("en", "Electronic devices and gadgets")
                .Set("de", "Elektronische Geräte und Zubehör")
                .Set("fr", "Appareils électroniques et accessoires"),
            DisplayOrder = 1,
            IsActive = true
        };

        var computers = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Slug = "computers",
            Name = new LocalizedContent()
                .Set("en", "Computers")
                .Set("de", "Computer")
                .Set("fr", "Ordinateurs"),
            Description = new LocalizedContent()
                .Set("en", "Desktop and laptop computers")
                .Set("de", "Desktop- und Laptop-Computer")
                .Set("fr", "Ordinateurs de bureau et portables"),
            DisplayOrder = 2,
            IsActive = true
        };

        var peripherals = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Slug = "peripherals",
            Name = new LocalizedContent()
                .Set("en", "Peripherals")
                .Set("de", "Zubehör")
                .Set("fr", "Périphériques"),
            Description = new LocalizedContent()
                .Set("en", "Keyboards, mice, and other peripherals")
                .Set("de", "Tastaturen, Maus und anderes Zubehör")
                .Set("fr", "Claviers, souris et autres périphériques"),
            DisplayOrder = 3,
            IsActive = true
        };

        // Subcategories
        var gamingLaptops = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Slug = "gaming-laptops",
            Name = new LocalizedContent()
                .Set("en", "Gaming Laptops")
                .Set("de", "Gaming-Laptops")
                .Set("fr", "Ordinateurs portables de jeu"),
            ParentCategoryId = computers.Id,
            DisplayOrder = 1,
            IsActive = true
        };

        var businessLaptops = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
            Slug = "business-laptops",
            Name = new LocalizedContent()
                .Set("en", "Business Laptops")
                .Set("de", "Business-Laptops")
                .Set("fr", "Ordinateurs portables professionnels"),
            ParentCategoryId = computers.Id,
            DisplayOrder = 2,
            IsActive = true
        };

        var keyboards = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
            Slug = "keyboards",
            Name = new LocalizedContent()
                .Set("en", "Keyboards")
                .Set("de", "Tastaturen")
                .Set("fr", "Claviers"),
            ParentCategoryId = peripherals.Id,
            DisplayOrder = 1,
            IsActive = true
        };

        var mice = new Category
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Slug = "mice",
            Name = new LocalizedContent()
                .Set("en", "Mice")
                .Set("de", "Maus")
                .Set("fr", "Souris"),
            ParentCategoryId = peripherals.Id,
            DisplayOrder = 2,
            IsActive = true
        };

        categories.AddRange(new[]
        {
            electronics, computers, peripherals,
            gamingLaptops, businessLaptops,
            keyboards, mice
        });

        return categories;
    }

    /// <summary>
    /// Generates brand data with realistic tech brands
    /// </summary>
    private static List<Brand> GenerateBrands()
    {
        var brands = new List<Brand>();
        var brandNames = new[]
        {
            ("Apple", "apple", "https://www.apple.com/favicon.ico"),
            ("Dell", "dell", "https://www1.dell.com/favicon.ico"),
            ("HP", "hp", "https://h20195.www2.hp.com/favicon.ico"),
            ("Lenovo", "lenovo", "https://lenovo.com/favicon.ico"),
            ("ASUS", "asus", "https://www.asus.com/favicon.ico"),
            ("Acer", "acer", "https://www.acer.com/favicon.ico"),
            ("MSI", "msi", "https://www.msi.com/favicon.ico"),
            ("Razer", "razer", "https://www.razer.com/favicon.ico"),
            ("Corsair", "corsair", "https://www.corsair.com/favicon.ico"),
            ("Logitech", "logitech", "https://www.logitech.com/favicon.ico"),
        };

        foreach (var (name, slug, logo) in brandNames)
        {
            brands.Add(new Brand
            {
                Id = Guid.NewGuid(),
                Slug = slug,
                Name = new LocalizedContent()
                    .Set("en", name)
                    .Set("de", name)
                    .Set("fr", name),
                Description = new LocalizedContent()
                    .Set("en", $"{name} - Premium technology products")
                    .Set("de", $"{name} - Premium-Technologieprodukte")
                    .Set("fr", $"{name} - Produits technologiques premium"),
                LogoUrl = logo,
                WebsiteUrl = $"https://www.{slug}.com",
                DisplayOrder = ++_brandCounter,
                IsActive = true
            });
        }

        return brands;
    }

    /// <summary>
    /// Generates realistic product data with variants, images, and documents
    /// </summary>
    private static List<Product> GenerateProducts(int count, List<Brand> brands)
    {
        var products = new List<Product>();
        var productFaker = CreateProductFaker(brands);

        for (int i = 0; i < count; i++)
        {
            var product = productFaker.Generate();
            products.Add(product);
        }

        return products;
    }

    /// <summary>
    /// Creates Bogus faker for realistic product generation
    /// </summary>
    private static Faker<Product> CreateProductFaker(List<Brand> brands)
    {
        var productTypes = new[]
        {
            "Gaming Laptop", "Business Laptop", "Ultrabook",
            "2-in-1 Laptop", "Workstation", "Chromebook",
            "Gaming Desktop", "Workstation Desktop", "Mini PC"
        };

        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, _ => Guid.NewGuid())
            .RuleFor(p => p.Sku, f => $"SKU-{++_productCounter:D4}")
            .RuleFor(p => p.Slug, (f, p) => p.Sku.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace("_", "-"))
            .RuleFor(p => p.Name, f => new LocalizedContent()
                .Set("en", f.PickRandom(productTypes) + " " + f.Commerce.ProductName())
                .Set("de", f.PickRandom(productTypes) + " " + f.Commerce.ProductName())
                .Set("fr", f.PickRandom(productTypes) + " " + f.Commerce.ProductName()))
            .RuleFor(p => p.ShortDescription, f => new LocalizedContent()
                .Set("en", f.Commerce.ProductDescription())
                .Set("de", f.Commerce.ProductDescription())
                .Set("fr", f.Commerce.ProductDescription()))
            .RuleFor(p => p.Description, f => new LocalizedContent()
                .Set("en", $"{f.Commerce.ProductDescription()}. Features: {string.Join(", ", f.Lorem.Words(5))}")
                .Set("de", $"{f.Commerce.ProductDescription()}. Features: {string.Join(", ", f.Lorem.Words(5))}")
                .Set("fr", $"{f.Commerce.ProductDescription()}. Features: {string.Join(", ", f.Lorem.Words(5))}"))
            .RuleFor(p => p.MetaDescription, f => new LocalizedContent()
                .Set("en", f.Lorem.Sentence(10))
                .Set("de", f.Lorem.Sentence(10))
                .Set("fr", f.Lorem.Sentence(10)))
            .RuleFor(p => p.MetaKeywords, f => new LocalizedContent()
                .Set("en", string.Join(", ", f.Lorem.Words(5)))
                .Set("de", string.Join(", ", f.Lorem.Words(5)))
                .Set("fr", string.Join(", ", f.Lorem.Words(5))))
            .RuleFor(p => p.BrandId, f => f.PickRandom(brands).Id)
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(500, 2500), CultureInfo.InvariantCulture))
            .RuleFor(p => p.SpecialPrice, f => f.Random.Bool(0.3f) ? decimal.Parse(f.Commerce.Price(400, 2000), CultureInfo.InvariantCulture) : null)
            .RuleFor(p => p.CostPrice, f => decimal.Parse(f.Commerce.Price(300, 1500), CultureInfo.InvariantCulture))
            .RuleFor(p => p.Weight, f => f.Random.Decimal(1.2m, 2.8m))
            .RuleFor(p => p.WeightUnit, _ => "kg")
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 500))
            .RuleFor(p => p.LowStockThreshold, f => f.Random.Int(5, 20))
            .RuleFor(p => p.IsStockTracked, _ => true)
            .RuleFor(p => p.IsAvailable, f => f.Random.Bool(0.85f))
            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.95f))
            .RuleFor(p => p.IsFeatured, f => f.Random.Bool(0.15f))
            .RuleFor(p => p.IsNew, f => f.Random.Bool(0.25f))
            .RuleFor(p => p.AverageRating, f => f.Random.Decimal(2.5m, 5.0m))
            .RuleFor(p => p.ReviewCount, f => f.Random.Int(0, 500))
            .RuleFor(p => p.ThumbnailUrl, f => $"https://picsum.photos/300/300?random={f.Random.Int()}")
            .RuleFor(p => p.CreatedAt, f => f.Date.PastDateOnly(365).ToDateTime(default(TimeOnly)))
            .RuleFor(p => p.UpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.UtcNow))
            .RuleFor(p => p.CreatedBy, f => f.Internet.UserName())
            .RuleFor(p => p.UpdatedBy, f => f.Internet.UserName())
            .RuleFor(p => p.TenantId, _ => SeedConstants.DefaultTenantId)

            // Variants
            .RuleFor(p => p.Variants, (f, p) => GenerateVariants(f, p))

            // Images
            .RuleFor(p => p.Images, (f, p) => GenerateImages(f, p))

            // Documents
            .RuleFor(p => p.Documents, (f, p) => GenerateDocuments(f, p))

            // Categories
            .RuleFor(p => p.ProductCategories, (f, p) => GenerateProductCategories(f, p))

            // Attributes
            .RuleFor(p => p.AttributeValues, (f, p) => new List<ProductAttributeValue>());

        return faker;
    }

    private static List<ProductVariant> GenerateVariants(Faker faker, Product product)
    {
        var variantCount = faker.Random.Int(1, 5);
        var variants = new List<ProductVariant>();

        var colors = new[] { "Black", "Silver", "Gold", "Red", "Blue" };
        var storages = new[] { "256GB", "512GB", "1TB", "2TB" };

        for (int i = 0; i < variantCount; i++)
        {
            var color = faker.PickRandom(colors);
            var storage = faker.PickRandom(storages);
            var variantPrice = product.Price * (faker.Random.Decimal(0.8m, 1.2m));

            variants.Add(new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Sku = $"{product.Sku}-{color.Substring(0, 3).ToUpper(System.Globalization.CultureInfo.CurrentCulture)}-{storage.Substring(0, 1)}",
                Name = new LocalizedContent()
                    .Set("en", $"{color}, {storage}")
                    .Set("de", $"{color}, {storage}")
                    .Set("fr", $"{color}, {storage}"),
                Price = variantPrice,
                SpecialPrice = faker.Random.Bool(0.3f) ? variantPrice * 0.85m : null,
                StockQuantity = faker.Random.Int(0, 200),
                IsActive = true,
                IsDefault = i == 0,
                DisplayOrder = i,
                ImageUrl = $"https://picsum.photos/400/400?random={faker.Random.Int()}",
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                TenantId = product.TenantId
            });
        }

        return variants;
    }

    private static List<ProductImage> GenerateImages(Faker faker, Product product)
    {
        var imageCount = faker.Random.Int(2, 6);
        var images = new List<ProductImage>();

        for (int i = 0; i < imageCount; i++)
        {
            images.Add(new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Url = $"https://picsum.photos/1200/800?random={faker.Random.Int()}",
                AltText = $"Product image {i + 1}",
                Title = $"Product view {i + 1}",
                ThumbnailUrl = $"https://picsum.photos/150/150?random={faker.Random.Int()}",
                MediumUrl = $"https://picsum.photos/400/400?random={faker.Random.Int()}",
                LargeUrl = $"https://picsum.photos/800/800?random={faker.Random.Int()}",
                MimeType = "image/jpeg",
                Width = 1200,
                Height = 800,
                FileSizeBytes = faker.Random.Long(500000, 2000000),
                IsPrimary = i == 0,
                DisplayOrder = i,
                IsActive = true,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                TenantId = product.TenantId
            });
        }

        return images;
    }

    private static List<ProductDocument> GenerateDocuments(Faker faker, Product product)
    {
        var documents = new List<ProductDocument>();
        var docTypes = new[] { "specification", "manual", "datasheet", "certification" };

        // English documents (always include)
        documents.Add(new ProductDocument
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Name = new LocalizedContent()
                .Set("en", "User Manual")
                .Set("de", "Benutzerhandbuch")
                .Set("fr", "Manuel d'utilisation"),
            Description = new LocalizedContent()
                .Set("en", "Complete user manual and troubleshooting guide")
                .Set("de", "Vollständiges Benutzerhandbuch und Fehlerbehebungsleitfaden"),
            DocumentType = "manual",
            Url = "https://example.com/docs/manual.pdf",
            Language = "en",
            Version = "1.0",
            ReleaseDate = product.CreatedAt,
            IsPublic = true,
            DisplayOrder = 0,
            IsActive = true,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            CreatedBy = product.CreatedBy,
            TenantId = product.TenantId
        });

        // German documents
        if (faker.Random.Bool(0.7f))
        {
            documents.Add(new ProductDocument
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = new LocalizedContent()
                    .Set("de", "Technische Spezifikation")
                    .Set("en", "Technical Specification"),
                DocumentType = "specification",
                Url = "https://example.com/docs/spec-de.pdf",
                Language = "de",
                Version = "1.0",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                TenantId = product.TenantId
            });
        }

        return documents;
    }

    private static List<ProductCategory> GenerateProductCategories(Faker faker, Product product)
    {
        var categoryIds = new[]
        {
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Guid.Parse("00000000-0000-0000-0000-000000000012"),
        };

        var selectedCategoryIds = faker.PickRandom(categoryIds, faker.Random.Int(1, 3));
        var categories = new List<ProductCategory>();

        int order = 0;
        foreach (var categoryId in selectedCategoryIds)
        {
            categories.Add(new ProductCategory
            {
                ProductId = product.Id,
                CategoryId = categoryId,
                IsPrimary = order == 0,
                DisplayOrder = order,
                CreatedAt = product.CreatedAt
            });
            order++;
        }

        return categories;
    }
}
