using B2Connect.ThemeService.Models;
using B2Connect.ThemeService.Services;
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
    opts.ServiceName = "ThemingService";
    opts.Http.EnableEndpoints = true;
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Remove Controllers - using Wolverine HTTP Endpoints
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Theming Services
builder.Services.AddSingleton<IThemeRepository, InMemoryThemeRepository>();
builder.Services.AddScoped<IThemeService, ThemeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseServiceDefaults();
app.UseHttpsRedirection();
app.UseAuthorization();

// Map Wolverine HTTP Endpoints
app.MapWolverineEndpoints();

app.Run();
