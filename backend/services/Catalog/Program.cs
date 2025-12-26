using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Handlers;
using B2Connect.Shared.Search.Extensions;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

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

// Configure logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
