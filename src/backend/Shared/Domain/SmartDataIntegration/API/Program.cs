using B2X.ServiceDefaults;
using B2X.Shared.Messaging.Extensions;
using B2X.SmartDataIntegration;
using EFCore.NamingConventions;
using FluentValidation;
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
// builder.Host.AddServiceDefaults();

// Add Wolverine with HTTP Endpoints
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"];
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "SmartDataIntegrationService";

    // Discovery configuration
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // Add RabbitMQ if enabled (requires Wolverine.RabbitMq package)
    // if (useRabbitMq)
    // {
    //     opts.UseRabbitMq(rabbitMqUri);
    // }
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Register Controllers to enable model binding support (e.g. [FromForm], IFormFile)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("SmartDataIntegrationDb");

// Allow selecting an in-memory provider for local/demo runs via configuration
var dbProvider = builder.Configuration["Database:Provider"] ?? builder.Configuration["Database__Provider"];

// Add Smart Data Integration Services
builder.Services.AddSmartDataIntegration(options =>
{
    if (string.Equals(dbProvider, "inmemory", StringComparison.OrdinalIgnoreCase))
    {
        options.UseInMemoryDatabase("B2X_smartdataintegration_inmemory");
    }
    else
    {
        options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }
});

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();
