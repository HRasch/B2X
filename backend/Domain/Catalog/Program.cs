using FluentValidation;
using B2Connect.Shared.Search.Extensions;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
using Serilog;
using Wolverine;
using Wolverine.Http;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;

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
    opts.ServiceName = "CatalogService";

    // Enable HTTP Endpoints (Wolverine Mediator)
    // opts.Http.EnableEndpoints = true;  // TODO: Enable when Wolverine HTTP is properly configured

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

// Remove Controllers - using Wolverine HTTP Endpoints instead
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Elasticsearch
try
{
    var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
    var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(
        new Uri(elasticsearchUri));
    var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
    builder.Services.AddSingleton(client);
}
catch (Exception ex)
{
    var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("Program");
    logger.LogWarning(ex, "Failed to configure Elasticsearch client. Search functionality may be limited.");
}

// Add Database Context (Issue #30: Price Calculation Persistence)
// Uses PostgreSQL with snake_case naming convention
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=b2connect_catalog;Username=postgres;Password=postgres";
builder.Services.AddDbContext<B2Connect.CatalogService.Infrastructure.Data.CatalogDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// TODO: Add application services (ProductService, QueryHandlers, etc.) once implemented
// NOTE: Return Management Services moved to Customer domain (Story 8: Widerrufsmanagement)
// The ReturnApiHandler, Validators, Repositories have been migrated to Customer domain
// per domain migration strategy (PR #41)

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Map Wolverine HTTP Endpoints (replaces MapControllers)
app.MapWolverineEndpoints();

app.Run();
