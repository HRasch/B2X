using B2X.Categories.Handlers;
using B2X.ServiceDefaults;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Service Defaults (OpenTelemetry, Health Checks, etc.)
builder.AddServiceDefaults();

// Configure Wolverine for CQRS
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "CategoriesService";
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(CategoryCommandHandler).Assembly);
});

var app = builder.Build();

// Map Aspire endpoints (health, metrics)
app.UseServiceDefaults();

// Map Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();
