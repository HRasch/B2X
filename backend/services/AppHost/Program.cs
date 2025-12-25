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

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

var app = builder.Build();

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

// Health check endpoint
app.MapGet("/api/health", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    var services = new[]
    {
        new { name = "Auth Service", url = "http://localhost:5001/health" },
        new { name = "Tenant Service", url = "http://localhost:5002/health" },
        new { name = "API Gateway", url = "http://localhost:5000/health" }
    };

    var health = new List<object>();
    foreach (var service in services)
    {
        try
        {
            var response = await client.GetAsync(service.url);
            health.Add(new
            {
                service.name,
                status = response.IsSuccessStatusCode ? "healthy" : "unhealthy",
                statusCode = response.StatusCode
            });
        }
        catch
        {
            health.Add(new
            {
                service.name,
                status = "unavailable",
                statusCode = 0
            });
        }
    }

    return health;
});

// Dashboard routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

await app.RunAsync();


