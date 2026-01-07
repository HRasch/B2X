using B2Connect.ServiceDefaults;
using B2Connect.Shared.Monitoring.Abstractions;
using B2Connect.Shared.Monitoring.Services;
using B2Connect.Shared.Monitoring.Data;
using B2Connect.Shared.Middleware;
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
    opts.ServiceName = "MonitoringService";
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Add Wolverine HTTP support
builder.Services.AddWolverineHttp();

// Add Database Context
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MonitoringDb")));

// Register Monitoring Services
builder.Services.AddScoped<IServiceMonitor, ServiceMonitor>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Map Wolverine endpoints
app.MapWolverineEndpoints();

app.Run();