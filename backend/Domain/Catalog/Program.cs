using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Handlers;
using B2Connect.Shared.Search.Extensions;
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
var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(
    new Uri(elasticsearchUri));
var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
builder.Services.AddSingleton(client);

// Add application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductQueryHandler, ProductQueryHandler>();
builder.Services.AddScoped<ISearchIndexService, SearchIndexService>();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Map Wolverine HTTP Endpoints (replaces MapControllers)
app.MapWolverineEndpoints();

app.Run();
