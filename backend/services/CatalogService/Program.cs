using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Wolverine;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Repositories;
using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS.Validators;

var builder = WebApplication.CreateBuilder(args);

// ==================== SERVICES ====================

// Add controllers and API exploration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Authentication and Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // JWT configuration will be loaded from configuration
        options.Authority = builder.Configuration.GetValue<string>("Auth:Authority") ?? "https://localhost:5001";
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// Swagger/OpenAPI configuration is minimal - uses default configuration
builder.Services.AddSwaggerGen();

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

// ==================== CACHING SETUP ====================
// Register distributed cache for query result caching
// Development: In-memory cache
// Production: Redis cache (configure in appsettings.json)
if (builder.Environment.IsDevelopment())
{
    // In-memory cache for development
    builder.Services.AddDistributedMemoryCache();
}
else
{
    // Production: Configure Redis or other distributed cache
    var cacheType = builder.Configuration.GetValue<string>("Cache:Type", "Memory").ToLower();
    if (cacheType == "redis")
    {
        var redisConnection = builder.Configuration.GetConnectionString("Redis")
            ?? builder.Configuration["Cache:Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnection))
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () =>
                    StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection);
            });
        }
        else
        {
            // Fallback to in-memory if Redis not configured
            builder.Services.AddDistributedMemoryCache();
        }
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }
}

// ==================== CQRS READ MODEL SETUP ====================
// Register the denormalized read model database context
// This is separate from the write model (CatalogDbContext) for true CQRS separation

builder.Services.AddDbContext<CatalogReadDbContext>(options =>
{
    if (useInMemoryDemo)
    {
        // Use in-memory database for development/demo
        options.UseInMemoryDatabase("CatalogReadDb");
    }
    else
    {
        // Use same connection string but separate schema for read model
        // The read model tables are in the same database but organized separately
        switch (provider)
        {
            case "sqlserver":
                options.UseSqlServer(connectionString);
                break;
            case "inmemory":
                options.UseInMemoryDatabase("CatalogReadDb");
                break;
            default: // PostgreSQL
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                break;
        }
    }

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

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

// FluentValidation - Register all validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

// ==================== WOLVERINE CQRS SETUP ====================
builder.Services.AddWolverine(opts =>
{
    // Automatic handler discovery from this assembly
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // Configure message transport based on environment
    if (builder.Environment.IsDevelopment())
    {
        // In-memory transport for fast local development
        opts.UseInMemoryTransport();
        builder.Services.GetRequiredService<ILogger<Program>>().LogInformation("Using IN-MEMORY transport for development");
    }
    else
    {
        // Production: Configure based on settings
        var logger = builder.Services.GetRequiredService<ILogger<Program>>();
        var transportType = builder.Configuration.GetValue<string>("Wolverine:Transport", "RabbitMQ").ToLower();

        switch (transportType)
        {
            case "rabbitmq":
                var rabbitMqConnection = builder.Configuration["RabbitMQ:ConnectionString"]
                    ?? throw new InvalidOperationException("RabbitMQ:ConnectionString not configured");
                opts.UseRabbitMq(settings =>
                {
                    settings.ConnectionString = rabbitMqConnection;
                });
                logger.LogInformation("Using RABBITMQ transport for production");
                break;

            case "azure":
                var azureConnection = builder.Configuration["ServiceBus:ConnectionString"]
                    ?? throw new InvalidOperationException("ServiceBus:ConnectionString not configured");
                opts.UseAzureServiceBus(azureConnection);
                logger.LogInformation("Using AZURE SERVICE BUS transport for production");
                break;

            case "aws":
                // AWS SQS configuration
                var awsRegion = builder.Configuration["AWS:Region"] ?? "us-east-1";
                opts.UseAwsSqsTransport(options =>
                {
                    options.Region = awsRegion;
                });
                logger.LogInformation("Using AWS SQS transport for production in region {Region}", awsRegion);
                break;

            default:
                // Fallback to in-memory if no valid transport configured
                logger.LogWarning("Unknown transport type '{TransportType}', falling back to in-memory", transportType);
                opts.UseInMemoryTransport();
                break;
        }
    }

    // Configure error handling policies
    opts.Handlers.OnException<ValidationException>()
        .Discard();  // Don't retry validation errors

    opts.Handlers.OnException<TimeoutException>()
        .Retry
        .MaximumAttempts(3)
        .WithDelayInSeconds(1, 2, 5);  // Exponential backoff

    opts.Handlers.OnException<Exception>()
        .Retry
        .MaximumAttempts(5)
        .Then
        .MoveToDeadLetterQueue();  // Failed messages go to DLQ

    // Enable Dead Letter Queue
    opts.DeadLetterQueue.IsEnabled = true;
});

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
builder.Services.AddHealthChecks();
// .AddDbContextCheck<CatalogDbContext>("CatalogDb");

// Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ==================== MIDDLEWARE ====================

var app = builder.Build();

// Use Wolverine middleware for CQRS message handling
app.UseWolverine();

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

// Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

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
    var readDbContext = scope.ServiceProvider.GetRequiredService<CatalogReadDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        if (useInMemoryDemo)
        {
            logger.LogInformation("🔄 Using IN-MEMORY DEMO DATABASE with realistic test data");
            logger.LogInformation("📊 Seeding demo database with sample products, categories, and brands...");

            // Ensure write model database is created
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("✅ Write model database created");

            // Ensure read model database is created
            await readDbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("✅ Read model database created");

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

            // Ensure write model database is created and migrations applied
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("✅ Write model migrations applied");

            // Ensure read model database is created and migrations applied
            await readDbContext.Database.MigrateAsync();
            logger.LogInformation("✅ Read model migrations applied");

            logger.LogInformation("Database migrations applied successfully");
        }

        logger.LogInformation("✅ Catalog Service started successfully with CQRS architecture");
        logger.LogInformation("   📝 Write Model: CatalogDbContext (production transactions)");
        logger.LogInformation("   👁️  Read Model: CatalogReadDbContext (optimized queries)");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
        throw;
    }
}

// ==================== START APPLICATION ====================

app.Run();
