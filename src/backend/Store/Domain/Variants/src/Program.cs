using B2X.ServiceDefaults;
using B2X.Variants.Handlers;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Service Defaults (OpenTelemetry, Health Checks, etc.)
builder.AddServiceDefaults();

// Add Wolverine HTTP (required for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Configure Wolverine for CQRS
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "VariantsService";
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(VariantCommandHandler).Assembly);
});

var app = builder.Build();

// Map Aspire endpoints (health, metrics)
app.UseServiceDefaults();

// Map Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();
