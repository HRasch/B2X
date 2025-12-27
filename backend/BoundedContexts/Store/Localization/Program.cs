using Microsoft.EntityFrameworkCore;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Services;
using B2Connect.LocalizationService.Middleware;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, etc.)
builder.Host.AddServiceDefaults();

// Add Wolverine Messaging
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"] ?? "amqp://guest:guest@localhost:5672";
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

if (useRabbitMq)
{
    builder.Host.AddWolverineWithRabbitMq(rabbitMqUri, opts =>
    {
        opts.ServiceName = "LocalizationService";
        opts.Discovery.DisableConventionalDiscovery();
        opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    });
}
else
{
    builder.Host.AddWolverineMessaging(opts =>
    {
        opts.ServiceName = "LocalizationService";
        opts.Discovery.DisableConventionalDiscovery();
        opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    });
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
var connectionString = builder.Configuration.GetConnectionString("LocalizationDb");
var provider = builder.Configuration.GetValue<string>("Database:Provider", "PostgreSQL").ToLower();

builder.Services.AddDbContext<LocalizationDbContext>(options =>
{
    var provider = builder.Configuration.GetValue<string>("Database:Provider", "inmemory").ToLower();
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
                var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
                logger.LogWarning(
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

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();
app.UseHttpsRedirection();
app.UseMiddleware<LocalizationMiddleware>();
app.UseAuthorization();
app.MapControllers();

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
