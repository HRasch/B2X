using Microsoft.EntityFrameworkCore;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Repositories;
using B2Connect.CatalogService.Services;

var builder = WebApplication.CreateBuilder(args);

// ==================== SERVICES ====================

// Add controllers and API exploration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "B2Connect Catalog Service API",
        Version = "v1",
        Description = "Product catalog with multilingual support, variants, categories, brands, and documents",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "B2Connect Development Team"
        }
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("CatalogDb");
var provider = builder.Configuration.GetValue<string>("Database:Provider", "PostgreSQL").ToLower();

// Support for demo/development mode with in-memory database
var useInMemoryDemo = builder.Configuration.GetValue<bool>("CatalogService:UseInMemoryDemo", false)
    || (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("CatalogService:UseDemoDataByDefault", true));

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    if (useInMemoryDemo)
    {
        // Use in-memory database for development/demo
        options.UseInMemoryDatabase("CatalogDemoDb");
    }
    else
    {
        // Use configured provider
        switch (provider)
        {
            case "sqlserver":
                options.UseSqlServer(connectionString);
                break;
            case "inmemory":
                options.UseInMemoryDatabase("CatalogDb");
                break;
            default: // PostgreSQL
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
        }
    }

    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// Register CatalogDbContextFactory for demo data generation
builder.Services.AddScoped<ICatalogDbContextFactory, CatalogDbContextFactory>();

// Repository Registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();

// Service Registration
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5175"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>("CatalogDb");

// Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ==================== MIDDLEWARE ====================

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Service API V1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// CORS Middleware
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");

// Global exception handler (basic)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandler?.Error;

        await context.Response.WriteAsJsonAsync(new
        {
            error = "An internal server error occurred",
            message = app.Environment.IsDevelopment() ? exception?.Message : null,
            traceId = context.TraceIdentifier
        });
    });
});

app.UseRouting();

app.MapControllers();
app.MapHealthChecks("/health");

// ==================== DATABASE INITIALIZATION ====================

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        if (useInMemoryDemo)
        {
            logger.LogInformation("🔄 Using IN-MEMORY DEMO DATABASE with realistic test data");
            logger.LogInformation("📊 Seeding demo database with sample products, categories, and brands...");

            // Ensure database is created
            await dbContext.Database.EnsureCreatedAsync();

            // Seed demo data if empty
            if (!await dbContext.Products.AnyAsync())
            {
                var (categories, brands, products) =
                    CatalogDemoDataGenerator.GenerateDemoCatalog(
                        productCount: builder.Configuration.GetValue<int>("CatalogService:DemoProductCount", 50),
                        seed: 42); // Fixed seed for reproducible demo data

                dbContext.Categories.AddRange(categories);
                dbContext.Brands.AddRange(brands);
                dbContext.Products.AddRange(products);

                await dbContext.SaveChangesAsync();

                logger.LogInformation("✅ Demo database seeded successfully!");
                logger.LogInformation("   📦 Products: {ProductCount}", products.Count);
                logger.LogInformation("   🏷️  Categories: {CategoryCount}", categories.Count);
                logger.LogInformation("   🏢 Brands: {BrandCount}", brands.Count);
                logger.LogInformation("   🖼️  Product Variants: {VariantCount}",
                    products.Sum(p => p.Variants?.Count ?? 0));
                logger.LogInformation("   📸 Product Images: {ImageCount}",
                    products.Sum(p => p.Images?.Count ?? 0));
                logger.LogInformation("   📄 Product Documents: {DocumentCount}",
                    products.Sum(p => p.Documents?.Count ?? 0));
            }
            else
            {
                logger.LogInformation("✅ Demo database already initialized");
            }
        }
        else
        {
            logger.LogInformation("Applying database migrations...");

            // Ensure database is created and migrations applied
            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully");
        }

        logger.LogInformation("✅ Catalog Service started successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
        throw;
    }
}

// ==================== START APPLICATION ====================

app.Run();
