using B2X.LocalizationService.Data;
using B2X.LocalizationService.Middleware;
using B2X.LocalizationService.Services;
using B2X.ServiceDefaults;
using B2X.Shared.Messaging.Extensions;
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
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "LocalizationService";

    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Add Controllers instead of Wolverine endpoints
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<LocalizationDbContext>(options =>
{
    var provider = builder.Configuration.GetValue<string>("Database:Provider", "inmemory").ToLower(System.Globalization.CultureInfo.CurrentCulture);
    if (string.Equals(provider, "inmemory", StringComparison.Ordinal))
    {
        options.UseInMemoryDatabase("LocalizationDb");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("LocalizationDb");
        if (string.IsNullOrEmpty(connectionString))
        {
            if (builder.Environment.IsDevelopment())
            {
                connectionString = "Host=localhost;Database=B2X_localization;Username=postgres;Password=postgres";
                System.Console.WriteLine(
                    "⚠️ Using DEVELOPMENT database credentials. This MUST be changed in production. " +
                    "Set 'ConnectionStrings:LocalizationDb' via environment variables or Azure Key Vault.");
            }
            else
            {
                throw new InvalidOperationException(
                    "Database connection string MUST be configured in production. " +
                    "Set 'ConnectionStrings:LocalizationDb' via: environment variables, Azure Key Vault, or Docker Secrets.");
            }
        }

        options.UseNpgsql(connectionString);
    }
});

// Services
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IEntityLocalizationService, EntityLocalizationService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis") ?? "localhost:6379";
    options.InstanceName = "localization:";
});
builder.Services.AddHttpContextAccessor();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();
app.UseHttpsRedirection();
app.UseMiddleware<LocalizationMiddleware>();
app.UseAuthorization();

// Map Controllers instead of Wolverine endpoints
app.MapControllers();

// Database migration and seeding (non-blocking with error handling)
_ = Task.Run(async () =>
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
        await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

        try
        {
            using var seedScope = app.Services.CreateScope();
            var seedDbContext = seedScope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
            await LocalizationSeeder.SeedAsync(seedDbContext, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error seeding localization data");
        }
    });

await app.RunAsync().ConfigureAwait(false);
