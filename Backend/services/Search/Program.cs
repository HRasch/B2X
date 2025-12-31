using B2Connect.Domain.Search.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Elasticsearch
// Read Elasticsearch config from Aspire / environment
var elasticUri = builder.Configuration["Elasticsearch:Uri"] ?? builder.Configuration["ELASTIC_URI"] ?? "http://localhost:9200";
var elasticUser = builder.Configuration["Elasticsearch:Username"] ?? builder.Configuration["ELASTICSEARCH_USERNAME"];
var elasticPassword = builder.Configuration["Elasticsearch:Password"] ?? builder.Configuration["Elasticsearch__Password"] ?? builder.Configuration["ELASTICSEARCH_PASSWORD"];
var elasticSecurity = builder.Configuration["ELASTICSEARCH_SECURITY_ENABLED"] ?? builder.Configuration["Elasticsearch:SecurityEnabled"];

builder.Services.AddSingleton<ITenantCredentialProvider, ConfigTenantCredentialProvider>();
builder.Services.AddSingleton<IElasticService>(sp =>
{
    var provider = sp.GetRequiredService<ITenantCredentialProvider>();
    return new ElasticService(elasticUri, elasticUser, elasticPassword, provider);
});
builder.Services.AddSingleton<ITenantResolver, ConfigTenantResolver>();
// seed demo catalog into tenant indices on startup (development only)
builder.Services.AddHostedService<B2Connect.Services.Search.Services.CatalogIndexSeeder>();
// Catalog indexing services/providers
builder.Services.AddSingleton<ICatalogProductProvider, DemoCatalogProvider>();
var catalogApi = builder.Configuration["Catalog:ApiUrl"];
if (!string.IsNullOrWhiteSpace(catalogApi))
{
    builder.Services.AddHttpClient<HttpCatalogProvider>();
    builder.Services.AddSingleton<ICatalogProductProvider, HttpCatalogProvider>(sp => sp.GetRequiredService<HttpCatalogProvider>());
}
builder.Services.AddSingleton<ICatalogIndexer, CatalogIndexer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { }
