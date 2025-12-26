using Serilog;
using System.Net.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with structured logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "AppHost")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .ReadFrom.Configuration(context.Configuration);
});

// Load service configuration
var serviceConfig = builder.Configuration.GetSection("Services");
var apiGatewayUrl = serviceConfig["ApiGateway"] ?? "http://localhost:5000";
var authServiceUrl = serviceConfig["AuthService"] ?? "http://localhost:5001";
var tenantServiceUrl = serviceConfig["TenantService"] ?? "http://localhost:5002";
var localizationServiceUrl = serviceConfig["LocalizationService"] ?? "http://localhost:5003";

// Service Discovery - Register service URLs
builder.Services.AddSingleton(sp => new ServiceRegistry
{
    ApiGateway = apiGatewayUrl,
    AuthService = authServiceUrl,
    TenantService = tenantServiceUrl,
    LocalizationService = localizationServiceUrl
});

// Add CORS - Production-ready configuration
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:3000", "http://localhost:5173", "http://localhost:5174" };

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("AllowInternal", policy =>
    {
        policy
            .WithOrigins(apiGatewayUrl, authServiceUrl, tenantServiceUrl, localizationServiceUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add health checks with detailed status
builder.Services.AddHealthChecks()
    .AddCheck("apphost-self", () => HealthCheckResult.Healthy("AppHost is healthy"));

// Add services
builder.Services.AddControllersWithViews();
builder.Services
    .AddHttpClient("default")
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("User-Agent", "B2Connect-AppHost/1.0");
    });

builder.Services
    .AddHttpClient("service-discovery")
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(5);
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var serviceRegistry = app.Services.GetRequiredService<ServiceRegistry>();

logger.LogInformation("B2Connect AppHost starting...");
logger.LogInformation("Service Discovery: ApiGateway={ApiGateway}, AuthService={AuthService}, TenantService={TenantService}, LocalizationService={LocalizationService}",
    serviceRegistry.ApiGateway, serviceRegistry.AuthService, serviceRegistry.TenantService, serviceRegistry.LocalizationService);

// CORS middleware (before routing)
app.UseCors("AllowFrontend");

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Health endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.ToDictionary(
                x => x.Key,
                x => new
                {
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description,
                    duration = x.Value.Duration.TotalMilliseconds,
                    exception = x.Value.Exception?.Message
                })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

// Service status endpoint (comprehensive diagnostics)
app.MapGet("/api/health", async (IHttpClientFactory httpClientFactory, ServiceRegistry registry) =>
{
    var services = new Dictionary<string, ServiceStatus>();
    var client = httpClientFactory.CreateClient("service-discovery");
    var startTime = DateTime.UtcNow;

    var endpoints = new Dictionary<string, string>
    {
        { "apiGateway", registry.ApiGateway + "/health" },
        { "authService", registry.AuthService + "/health" },
        { "tenantService", registry.TenantService + "/health" },
        { "localizationService", registry.LocalizationService + "/health" }
    };

    foreach (var (name, url) in endpoints)
    {
        var serviceStartTime = DateTime.UtcNow;
        try
        {
            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(2));
            var response = await client.GetAsync(url, cts.Token);
            var responseTime = (DateTime.UtcNow - serviceStartTime).TotalMilliseconds;

            services[name] = new ServiceStatus
            {
                Status = response.IsSuccessStatusCode ? "healthy" : "unhealthy",
                ResponseTime = responseTime,
                Timestamp = serviceStartTime,
                StatusCode = (int)response.StatusCode
            };
        }
        catch (OperationCanceledException)
        {
            services[name] = new ServiceStatus
            {
                Status = "timeout",
                ResponseTime = 2000,
                Timestamp = serviceStartTime,
                Error = "Service health check timeout (2s)"
            };
        }
        catch (Exception ex)
        {
            services[name] = new ServiceStatus
            {
                Status = "unavailable",
                Timestamp = serviceStartTime,
                Error = ex.Message
            };
        }
    }

    var totalTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
    var overallStatus = services.Values.All(s => s.Status == "healthy") ? "healthy" : "degraded";

    return Results.Ok(new
    {
        status = overallStatus,
        timestamp = DateTime.UtcNow,
        version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0",
        uptime = GC.GetTotalMemory(false) / (1024 * 1024) + "MB",
        diagnostics = new
        {
            totalCheckTime = totalTime,
            serviceCount = services.Count,
            healthyServices = services.Count(s => s.Value.Status == "healthy")
        },
        services = services
    });
});

// Root endpoint
app.MapGet("/", () => Results.Redirect("/health"));

app.Run();

// ============================================================================
// Service Registry - Central service discovery
// ============================================================================
public class ServiceRegistry
{
    public string ApiGateway { get; set; } = "http://localhost:5000";
    public string AuthService { get; set; } = "http://localhost:5001";
    public string TenantService { get; set; } = "http://localhost:5002";
    public string LocalizationService { get; set; } = "http://localhost:5003";
}

// Service Status Model
public class ServiceStatus
{
    public string Status { get; set; } = "unknown";
    public double ResponseTime { get; set; }
    public DateTime Timestamp { get; set; }
    public int? StatusCode { get; set; }
    public string? Error { get; set; }
}

