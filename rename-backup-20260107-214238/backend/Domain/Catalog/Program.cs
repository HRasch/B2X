using B2X.ServiceDefaults;
using B2X.Shared.Messaging.Extensions;
using B2X.Shared.Search.Extensions;
using EFCore.NamingConventions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, Service Discovery)
builder.Host.AddServiceDefaults();

// Add Wolverine with HTTP Endpoints
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"];
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "CatalogService";

    // Enable HTTP Endpoints (Wolverine Mediator)
    // Enable HTTP Endpoints (Wolverine Mediator)
    // opts.Http.EnableEndpoints = true; // Disabled: leave Wolverine endpoint discovery off to avoid FormBindingFrame issues

    // Discovery configuration
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // Add RabbitMQ if enabled (requires Wolverine.RabbitMq package)
    // if (useRabbitMq)
    // {
    //     opts.UseRabbitMq(rabbitMqUri);
    // }
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Register Controllers to enable model binding support (e.g. [FromForm], IFormFile)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Elasticsearch
try
{
    var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"];
    // CA2000: ElasticsearchClientSettings is passed to ElasticsearchClient which manages its lifetime
    // The client is registered as Singleton and lives for the application lifetime
#pragma warning disable CA2000 // Dispose objects before losing scope
    var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(
        new Uri(elasticsearchUri));
#pragma warning restore CA2000
    var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
    builder.Services.AddSingleton(client);
}
catch (Exception ex)
{
    // CA2000: LoggerFactory is only used briefly for startup logging and will be GC'd
#pragma warning disable CA2000 // Dispose objects before losing scope
    using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
    var logger = loggerFactory.CreateLogger("Program");
#pragma warning restore CA2000
    logger.LogWarning(ex, "Failed to configure Elasticsearch client. Search functionality may be limited.");
}

// Add Database Context (Issue #30: Price Calculation Persistence)
// Uses PostgreSQL with snake_case naming convention
var connectionString = builder.Configuration.GetConnectionString("CatalogDb");

// Allow selecting an in-memory provider for local/demo runs via configuration
var dbProvider = builder.Configuration["Database:Provider"] ?? builder.Configuration["Database__Provider"];
builder.Services.AddDbContext<B2X.Catalog.Infrastructure.Data.CatalogDbContext>(options =>
{
    if (string.Equals(dbProvider, "inmemory", StringComparison.OrdinalIgnoreCase))
    {
        options.UseInMemoryDatabase("B2X_catalog_inmemory");
    }
    else
    {
        options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }
});

// Add Caching (Required for TaxRateService)
builder.Services.AddMemoryCache();

// Issue #30: B2C Price Transparency (PAngV)
// Add Tax Rate Services for VAT calculation
builder.Services.AddScoped<B2X.Catalog.Core.Interfaces.ITaxRateRepository,
    B2X.Catalog.Infrastructure.Data.TaxRateRepository>();
builder.Services.AddScoped<B2X.Catalog.Core.Interfaces.ITaxRateService,
    B2X.Catalog.Application.Handlers.TaxRateService>();
builder.Services.AddScoped<B2X.Catalog.Application.Handlers.PriceCalculationService>();
builder.Services.AddScoped<B2X.Catalog.Application.Validators.CalculatePriceValidator>();

// BMEcat Import Services (REQ-002)
builder.Services.AddScoped<B2X.Catalog.Application.Adapters.ICatalogImportAdapter,
    B2X.Catalog.Application.Adapters.BmecatImportAdapter>();
builder.Services.AddScoped<B2X.Catalog.Core.Interfaces.ICatalogImportRepository,
    B2X.Catalog.Infrastructure.Data.CatalogImportRepository>();
builder.Services.AddScoped<B2X.Catalog.Core.Interfaces.ICatalogProductRepository,
    B2X.Catalog.Infrastructure.Data.CatalogProductRepository>();
builder.Services.AddScoped<B2X.Catalog.Application.Handlers.CatalogImportService>();

// TODO: Add application services (ProductService, QueryHandlers, etc.) once implemented
// NOTE: Return Management Services moved to Customer domain (Story 8: Widerrufsmanagement)
// The ReturnApiHandler, Validators, Repositories have been migrated to Customer domain
// per domain migration strategy (PR #41)
// Register CatalogService implementations and endpoint adapters
builder.Services.AddScoped<B2X.Catalog.Services.ISearchIndexService, B2X.Catalog.Services.SearchIndexService>();
builder.Services.AddScoped<B2X.Catalog.Services.IProductService, B2X.Catalog.Services.ProductService>();

builder.Services.AddScoped<B2X.Catalog.Endpoints.IProductService, B2X.Catalog.Endpoints.ProductServiceAdapter>();
builder.Services.AddScoped<B2X.Catalog.Endpoints.ISearchIndexService, B2X.Catalog.Endpoints.SearchIndexAdapter>();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();



// Development-only stub services to enable frontend/gateway integration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<B2X.Catalog.Endpoints.IProductService, B2X.Catalog.Endpoints.DevProductService>();
    builder.Services.AddSingleton<B2X.Catalog.Endpoints.ISearchIndexService, B2X.Catalog.Endpoints.DevSearchIndexService>();
}

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Development-only: seed in-memory database with demo catalog products
try
{
    var configuration = app.Services.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
    var runtimeDbProvider = configuration?[("Database:Provider")] ?? configuration?[("Database__Provider")];
    if (string.Equals(runtimeDbProvider, "inmemory", StringComparison.OrdinalIgnoreCase) || builder.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var cfg = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        // Only seed when demo count configured or when running dev
        var demoCount = cfg.GetValue<int?>("CatalogService:DemoProductCount") ?? 0;
        if (demoCount <= 0)
            demoCount = 100; // default demo size

        // Ensure database created and seed CatalogImports/CatalogProducts
        var db = scope.ServiceProvider.GetService<B2X.Catalog.Infrastructure.Data.CatalogDbContext>();
        if (db != null)
        {
            db.Database.EnsureCreated();
            var seedLogger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("CatalogSeed");
            seedLogger?.LogInformation("Starting demo seeding: demoCount={DemoCount}", demoCount);

            // Use existing demo product generator
            B2X.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount, cfg.GetValue<string?>("CatalogService:DemoSector"));
            var (items, total) = B2X.Catalog.Endpoints.Dev.DemoProductStore.GetPage(1, demoCount);

            // Add a CatalogImport record and CatalogProduct rows
            var import = new B2X.Catalog.Core.Entities.CatalogImport
            {
                Id = Guid.NewGuid(),
                TenantId = B2X.Shared.Core.SeedConstants.DefaultTenantId,
                SupplierId = "demo",
                CatalogId = "demo-catalog",
                ImportTimestamp = DateTime.UtcNow,
                Status = B2X.Catalog.Core.Entities.ImportStatus.Completed,
                ProductCount = total,
                CreatedAt = DateTime.UtcNow
            };

            db.CatalogImports.Add(import);

            foreach (var p in items)
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(p);
                    var product = new B2X.Catalog.Core.Entities.CatalogProduct
                    {
                        Id = Guid.NewGuid(),
                        CatalogImportId = import.Id,
                        SupplierAid = p.sku?.ToString() ?? Guid.NewGuid().ToString(),
                        ProductData = json,
                        CreatedAt = DateTime.UtcNow
                    };
                    db.CatalogProducts.Add(product);
                }
                catch (Exception) { /* swallow individual product serialization errors during seeding */ }
            }

            db.SaveChanges();

            // Trigger indexing for realistic search infrastructure if available
            var searchIndex = scope.ServiceProvider.GetService<B2X.Catalog.Services.ISearchIndexService>();
            if (searchIndex != null)
            {
                seedLogger?.LogInformation("SearchIndexService resolved; indexing {Count} seeded products (fire-and-forget)", total);
                foreach (var p in items)
                {
                    try
                    {
                        var json = System.Text.Json.JsonSerializer.Serialize(p);
                        var productModel = System.Text.Json.JsonSerializer.Deserialize<B2X.Catalog.Models.Product>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (productModel != null)
                        {
                            // Index product (async fire-and-forget)
                            _ = searchIndex.IndexProductAsync(productModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        seedLogger?.LogWarning(ex, "Indexing of a seeded product failed (non-fatal)");
                    }
                }
                seedLogger?.LogInformation("Indexing trigger completed (fire-and-forget)");
            }
            else
            {
                seedLogger?.LogInformation("SearchIndexService not registered; skipping indexing.");
            }
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger("CatalogSeed");
    logger?.LogWarning(ex, "Demo seeding failed (non-fatal)");
}

// Map endpoints
// Prefer MVC controllers mapping to avoid Wolverine Form binding discovery crash in some environments
app.MapControllers();

app.Run();
