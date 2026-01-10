using B2X.ServiceDefaults;
using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Services;
using B2X.Shared.Monitoring.Data;
using B2X.Shared.Middleware;
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

// Add SignalR
builder.Services.AddSignalR();

// Add Database Context
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MonitoringDb")));

// Register Monitoring Services
builder.Services.AddScoped<IServiceMonitor, ServiceMonitor>();
builder.Services.AddScoped<B2X.Shared.Monitoring.Abstractions.IDebugEventBroadcaster, DebugEventBroadcaster>();

// Add tenant context
builder.Services.AddScoped<B2X.Shared.Tenancy.Infrastructure.Context.ITenantContext, B2X.Shared.Tenancy.Infrastructure.Context.TenantContext>();
builder.Services.AddScoped<B2X.Shared.Middleware.ITenantContextAccessor, B2X.Shared.Middleware.TenantContextAccessor>();

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

// ==================== TENANT CONTEXT MIDDLEWARE ====================
app.Use(async (context, next) =>
{
    var tenantId = context.User.GetTenantId();

    if (tenantId == Guid.Empty && context.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue) && Guid.TryParse(headerValue.ToString(), out var headerId))
    {
        tenantId = headerId;
    }

    if (tenantId != Guid.Empty)
    {
        // Set in both contexts
        var tenantContextAccessor = context.RequestServices.GetRequiredService<B2X.Shared.Middleware.ITenantContextAccessor>();
        var tenantContext = (B2X.Shared.Tenancy.Infrastructure.Context.TenantContext)context.RequestServices.GetRequiredService<B2X.Shared.Tenancy.Infrastructure.Context.ITenantContext>();

        tenantContextAccessor.SetTenantId(tenantId);
        tenantContext.TenantId = tenantId;
        context.Items["TenantId"] = tenantId;
    }

    await next(context).ConfigureAwait(false);
});

// Map Wolverine endpoints
app.MapWolverineEndpoints();

// Map SignalR hubs
app.MapHub<B2X.Monitoring.Hubs.DebugHub>("/debug-hub");

app.Run();