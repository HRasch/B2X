using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

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

// Add Elasticsearch
var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri)).DefaultIndex("b2connect_search");
builder.Services.AddSingleton<ElasticsearchClient>(new ElasticsearchClient(settings));

// Add Wolverine with HTTP Endpoints
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"] ?? "amqp://guest:guest@localhost:5672";
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "SearchService";

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

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=b2connect_search;Username=postgres;Password=postgres";
builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// Add Search Services
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IValidator<SearchQuery>, SearchQueryValidator>();

// Add Caching
builder.Services.AddMemoryCache();

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Auto-discover and register Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();