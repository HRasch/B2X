using Microsoft.EntityFrameworkCore;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Shared.Tenancy.Infrastructure.Context;

namespace B2Connect.Admin.Infrastructure.Data;

/// <summary>
/// Factory for creating and configuring CatalogDbContext instances
/// Supports both production databases and in-memory testing databases
/// </summary>
public interface ICatalogDbContextFactory
{
    /// <summary>
    /// Creates a configured DbContext for production use
    /// </summary>
    CatalogDbContext CreateProductionContext();

    /// <summary>
    /// Creates an in-memory DbContext with demo data for development/testing
    /// </summary>
    CatalogDbContext CreateDemoContext(int productCount = 100, int? seed = null);
}

/// <summary>
/// Implementation of ICatalogDbContextFactory
/// </summary>
public class CatalogDbContextFactory : ICatalogDbContextFactory
{
    private readonly IDbContextFactory<CatalogDbContext> _contextFactory;
    private readonly ILogger<CatalogDbContextFactory> _logger;

    public CatalogDbContextFactory(IDbContextFactory<CatalogDbContext> contextFactory,
        ILogger<CatalogDbContextFactory> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Creates a configured DbContext for production use
    /// Uses the connection string from appsettings.json
    /// </summary>
    public CatalogDbContext CreateProductionContext()
    {
        _logger.LogInformation("Creating production CatalogDbContext");
        return _contextFactory.CreateDbContext();
    }

    /// <summary>
    /// Creates an in-memory DbContext populated with realistic demo data using Bogus
    /// Useful for development, testing, and demonstrations
    /// </summary>
    /// <param name="productCount">Number of products to generate (default: 100)</param>
    /// <param name="seed">Optional seed for reproducible data generation</param>
    /// <returns>Fully seeded in-memory DbContext</returns>
    public CatalogDbContext CreateDemoContext(int productCount = 100, int? seed = null)
    {
        _logger.LogInformation("Creating demo CatalogDbContext with InMemory provider (productCount: {ProductCount})", productCount);

        // Create options for in-memory database
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: $"CatalogDemo_{Guid.NewGuid()}")
            .LogTo(message => _logger.LogDebug(message)) // Log EF Core queries in debug mode
            .Options;

        // Create a demo tenant context
        var tenantContext = new DemoTenantContext();

        // Create context with options and tenant context
        var context = new CatalogDbContext(options, tenantContext);

        // Ensure database is created (for InMemory, this just initializes the structure)
        context.Database.EnsureCreated();

        // Seed demo data
        SeedDemoData(context, productCount, seed);

        _logger.LogInformation("Demo context created and seeded with {ProductCount} products", productCount);
        return context;
    }

    /// <summary>
    /// Simple tenant context for demo/testing purposes
    /// </summary>
    private class DemoTenantContext : ITenantContext
    {
        public Guid TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001"); // Demo tenant
    }

    /// <summary>
    /// Seeds the database with realistic demo data
    /// </summary>
    private void SeedDemoData(CatalogDbContext context, int productCount, int? seed)
    {
        try
        {
            // Check if data already exists
            if (context.Products.Any())
            {
                _logger.LogInformation("Demo data already exists, skipping seed");
                return;
            }

            _logger.LogInformation("Starting demo data generation...");

            // Generate demo catalog using CatalogDemoDataGenerator
            var (categories, brands, products) = CatalogDemoDataGenerator.GenerateDemoCatalog(productCount, seed);

            _logger.LogInformation("Generated {CategoryCount} categories, {BrandCount} brands, {ProductCount} products",
                categories.Count, brands.Count, products.Count);

            // Add to context
            context.Categories.AddRange(categories);
            context.Brands.AddRange(brands);
            context.Products.AddRange(products);

            // Save changes
            context.SaveChanges();

            _logger.LogInformation("Demo data successfully saved to context");

            // Log summary
            LogDemoDataSummary(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding demo data");
            throw;
        }
    }

    /// <summary>
    /// Logs a summary of the seeded demo data
    /// </summary>
    private void LogDemoDataSummary(CatalogDbContext context)
    {
        try
        {
            var productCount = context.Products.Count();
            var categoryCount = context.Categories.Count();
            var brandCount = context.Brands.Count();
            var variantCount = context.ProductVariants.Count();
            var imageCount = context.ProductImages.Count();
            var documentCount = context.ProductDocuments.Count();

            _logger.LogInformation(
                "Demo Data Summary: {Products} products, {Variants} variants, {Categories} categories, " +
                "{Brands} brands, {Images} images, {Documents} documents",
                productCount, variantCount, categoryCount, brandCount, imageCount, documentCount);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not log demo data summary");
        }
    }
}

/// <summary>
/// Extension methods for demo database setup
/// </summary>
public static class CatalogDemoDbExtensions
{
    /// <summary>
    /// Registers the CatalogDbContextFactory in the DI container
    /// </summary>
    public static IServiceCollection AddCatalogDemoDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        var environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var useInMemory = configuration.GetValue<bool>("CatalogService:UseInMemoryDatabase",
            environment == "Development");

        if (useInMemory)
        {
            services.AddDbContextFactory<CatalogDbContext>(options =>
            {
                options.UseInMemoryDatabase("CatalogDemo");
            });
        }
        else
        {
            // Use the default PostgreSQL or SQL Server configuration from Program.cs
            services.AddDbContextFactory<CatalogDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("CatalogDb");
                var usePostgres = configuration.GetValue<bool>("CatalogService:UsePostgres", true);

                if (usePostgres)
                {
                    options.UseNpgsql(connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });
        }

        services.AddScoped<ICatalogDbContextFactory, CatalogDbContextFactory>();
        return services;
    }

    /// <summary>
    /// Initializes the catalog database with demo data if configured
    /// Call this after all services are registered but before the app starts
    /// </summary>
    public static async Task<IApplicationBuilder> InitializeCatalogDemoDatabase(
        this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var environment = scope.ServiceProvider
                .GetRequiredService<IConfiguration>()
                .GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var useInMemory = scope.ServiceProvider
                .GetRequiredService<IConfiguration>()
                .GetValue<bool>("CatalogService:UseInMemoryDatabase", environment == "Development");

            if (useInMemory)
            {
                var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatalogDbContext>>();

                try
                {
                    logger.LogInformation("Initializing in-memory catalog database with demo data");

                    // Ensure database is created
                    await context.Database.EnsureCreatedAsync();

                    // Seed if empty
                    if (!await context.Products.AnyAsync())
                    {
                        var (categories, brands, products) =
                            CatalogDemoDataGenerator.GenerateDemoCatalog(productCount: 50);

                        context.Categories.AddRange(categories);
                        context.Brands.AddRange(brands);
                        context.Products.AddRange(products);

                        await context.SaveChangesAsync();
                        logger.LogInformation("Demo database seeded successfully");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error initializing demo database");
                }
            }
        }

        return app;
    }
}

