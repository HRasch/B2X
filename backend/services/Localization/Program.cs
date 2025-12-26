using Microsoft.EntityFrameworkCore;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Services;
using B2Connect.LocalizationService.Middleware;
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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
var connectionString = builder.Configuration.GetConnectionString("LocalizationDb");
var provider = builder.Configuration.GetValue<string>("Database:Provider", "PostgreSQL").ToLower();

builder.Services.AddDbContext<LocalizationDbContext>(options =>
{
    switch (provider)
    {
        case "sqlserver":
            options.UseSqlServer(connectionString);
            break;
        case "inmemory":
            options.UseInMemoryDatabase("LocalizationDb");
            break;
        default: // PostgreSQL
            options.UseNpgsql(connectionString);
            break;
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
    .Database.MigrateAsync()
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
            logger.LogError(t.Exception, "Error migrating database");
        }
    });

await app.RunAsync();
