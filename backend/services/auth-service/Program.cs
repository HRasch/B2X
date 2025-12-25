using B2Connect.ServiceDefaults;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, etc.)
builder.Host.AddServiceDefaults();

// Add services
builder.Services.AddControllers();

var app = builder.Build();

// Service defaults middleware
app.UseServiceDefaults();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Auth Service is running");

await app.RunAsync();
