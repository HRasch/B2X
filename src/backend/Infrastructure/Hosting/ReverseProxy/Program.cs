using B2X.ReverseProxy.Middleware;
using B2X.ReverseProxy.Services;
using B2X.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (health checks, telemetry, etc.)
builder.AddServiceDefaults();

// Add YARP Reverse Proxy
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Tenant Resolution Services
builder.Services.AddSingleton<ITenantDomainResolver, TenantDomainResolver>();
builder.Services.AddMemoryCache();

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware Pipeline
app.UseHealthChecks("/health");

// Tenant Resolution - MUST be before YARP
app.UseTenantResolution();

// YARP Reverse Proxy
app.MapReverseProxy();

app.Run();
