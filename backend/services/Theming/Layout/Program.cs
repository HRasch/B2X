using B2Connect.LayoutService.Data;
using B2Connect.LayoutService.Services;
using B2Connect.ServiceDefaults;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Add services
builder.Services.AddControllers();

// Add Layout Service Database (PostgreSQL, SQL Server, or InMemory)
builder.Services.AddLayoutDatabase(builder.Configuration);

// Register repository and service
builder.Services.AddScoped<ILayoutRepository, LayoutRepository>();
builder.Services.AddScoped<ILayoutService, LayoutService.Services.LayoutService>();

// Add default B2Connect services (Health checks, OpenTelemetry, etc.)
builder.AddServiceDefaults();

var app = builder.Build();

// Apply migrations and ensure database is created
await app.Services.EnsureDatabaseAsync();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Layout Service starting on {Environment}", app.Environment.EnvironmentName);

// Log database provider
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LayoutDbContext>();
    logger.LogInformation("Database Provider: {Provider}", dbContext.GetDatabaseProviderName());
}

await app.RunAsync();
