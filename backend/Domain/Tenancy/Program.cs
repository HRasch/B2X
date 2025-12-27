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
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "TenancyService";
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

// Remove Controllers - using Wolverine HTTP Endpoints
// builder.Services.AddControllers();

var app = builder.Build();

// Service defaults middleware
app.UseServiceDefaults();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Map Wolverine HTTP Endpoints
app.MapWolverineEndpoints();
app.MapGet("/", () => "Tenant Service is running");

await app.RunAsync();
