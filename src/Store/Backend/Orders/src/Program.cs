using B2X.Orders.Application.Handlers;
using B2X.Orders.Core.Interfaces;
using B2X.Orders.Infrastructure.Data;
using B2X.Orders.Infrastructure.Repositories;
using B2X.ServiceDefaults;
using EFCore.NamingConventions;
using Microsoft.EntityFrameworkCore;
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
    opts.ServiceName = "OrdersService";

    // Discovery configuration
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Register Controllers to enable model binding support
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("OrdersDb");

// Allow selecting an in-memory provider for local/demo runs via configuration
var dbProvider = builder.Configuration["Database:Provider"] ?? builder.Configuration["Database__Provider"];
builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    if (string.Equals(dbProvider, "inmemory", StringComparison.OrdinalIgnoreCase))
    {
        options.UseInMemoryDatabase("B2X_orders_inmemory");
    }
    else
    {
        options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }
});

// Register Orders services
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Enable Wolverine HTTP endpoints
app.MapWolverineEndpoints();

// Map Controllers
app.MapControllers();

// Database migration (only in development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
