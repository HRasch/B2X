using Microsoft.EntityFrameworkCore;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Services;
using B2Connect.LocalizationService.Middleware;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
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
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"] ?? "amqp://guest:guest@localhost:5672";
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "LocalizationService";

    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // RabbitMQ integration (requires Wolverine.RabbitMq package)
    // if (useRabbitMq)
    // {
    //     opts.UseRabbitMq(rabbitMqUri);
    // }
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Remove Controllers - using Wolverine HTTP Endpoints
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
var connectionString = builder.Configuration.GetConnectionString("LocalizationDb");
var provider = builder.Configuration.GetValue<string>("Database:Provider", "PostgreSQL").ToLower(System.Globalization.CultureInfo.CurrentCulture);

builder.Services.AddDbContext<LocalizationDbContext>(options =>
{
    var provider = builder.Configuration.GetValue<string>("Database:Provider", "inmemory").ToLower(System.Globalization.CultureInfo.CurrentCulture);
    if (provider == "inmemory")
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
                connectionString = "Host=localhost;Database=b2connect_localization;Username=postgres;Password=postgres";
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
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();
app.UseHttpsRedirection();
app.UseMiddleware<LocalizationMiddleware>();
app.UseAuthorization();

// Map Wolverine HTTP Endpoints (replaces MapControllers)
app.MapWolverineEndpoints();

// Database migration and seeding (non-blocking with error handling)
_ = app.Services.CreateScope().ServiceProvider
    .GetRequiredService<LocalizationDbContext>()
    .Database.EnsureCreatedAsync()
    .ContinueWith(async t =>
    {
        if (t.IsCompletedSuccessfully)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
                await LocalizationSeeder.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error seeding localization data");
            }
        }
        else if (t.IsFaulted)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(t.Exception, "Error ensuring database created");
        }
    });

await app.RunAsync();
