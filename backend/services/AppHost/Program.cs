using Serilog;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services
builder.Services.AddControllersWithViews();
builder.Services
    .AddHttpClient("default")
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// CORS middleware (before routing)
app.UseCors("AllowAll");

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Lightweight health check (no blocking service calls)
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

// Service status endpoint (lazy-loaded, non-blocking)
app.MapGet("/api/health", async (IHttpClientFactory httpClientFactory) =>
{
    var services = new Dictionary<string, object>();
    var client = httpClientFactory.CreateClient("default");

    var endpoints = new Dictionary<string, string>
    {
        { "apiGateway", "http://localhost:5000/health" },
        { "authService", "http://localhost:5001/health" },
        { "tenantService", "http://localhost:5002/health" },
        { "localizationService", "http://localhost:5003/health" }
    };

    foreach (var (name, url) in endpoints)
    {
        try
        {
            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(2));
            var response = await client.GetAsync(url, cts.Token);
            services[name] = new { status = response.IsSuccessStatusCode ? "healthy" : "unhealthy" };
        }
        catch
        {
            services[name] = new { status = "unavailable" };
        }
    }

    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow, services });
});

app.Run();
