using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Handlers;
using B2Connect.Shared.Search.Extensions;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
using Serilog;
using Wolverine;

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
        opts.ServiceName = "CatalogService";
        opts.Discovery.DisableConventionalDiscovery();
        opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    });
}
else
{
    builder.Host.AddWolverineMessaging(opts =>
    {
        opts.ServiceName = "CatalogService";
        opts.Discovery.DisableConventionalDiscovery();
        opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    });
}

// Add controllers
builder.Services.AddControllers();
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

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
