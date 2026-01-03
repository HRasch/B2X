using System.Text;
using B2Connect.Admin.Application.Services;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Admin.Infrastructure.Repositories;
using B2Connect.Admin.Presentation.Filters;
using B2Connect.ERP;
using B2Connect.ServiceDefaults;
using B2Connect.Shared.Infrastructure.Extensions;
using B2Connect.Shared.Infrastructure.Logging;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using B2Connect.Shared.Tenancy.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging - Console + File
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        // .Enrich.WithSensitiveDataRedaction() // Disabled
        .WriteTo.Console()
        .WriteTo.File(
            "logs/admin-gateway-.txt",
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: Serilog.RollingInterval.Day)
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, Service Discovery)
builder.Host.AddServiceDefaults();

// Add Service Clients with Service Discovery
builder.Services.AddAllServiceClients();

// Get CORS origins from configuration
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (corsOrigins == null || corsOrigins.Length == 0)
{
    if (builder.Environment.IsDevelopment())
    {
        corsOrigins = new[]
        {
            "http://localhost:5174",
            "http://127.0.0.1:5174",
            "https://localhost:5174"
        };
        System.Console.WriteLine(
            "⚠️ CORS origins not configured. Using default development origins. " +
            "Configure 'Cors:AllowedOrigins' in appsettings.json for custom values.");
    }
    else
    {
        throw new InvalidOperationException(
            "CORS allowed origins MUST be configured in production. " +
            "Set 'Cors:AllowedOrigins' in appsettings.Production.json or environment variables (Cors__AllowedOrigins__0, etc.).");
    }
}

// Add CORS for Admin Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any localhost origin (Aspire uses dynamic ports)
            policy
                .SetIsOriginAllowed(origin =>
                {
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return false;
                    }

                    return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "X-Total-Count");
        }
        else
        {
            // In production, use strict origins from configuration
            policy
                .WithOrigins(corsOrigins!)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "X-Total-Count");
        }
    });
});

// Add Rate Limiting
// builder.Services.AddB2ConnectRateLimiting(builder.Configuration);

// Get JWT Secret from configuration
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSecret = "dev-only-secret-minimum-32-chars-required!";
        System.Console.WriteLine(
            "⚠️ Using DEVELOPMENT JWT secret. This MUST be changed in production via environment variables or Azure Key Vault. " +
            "Set 'Jwt:Secret' via environment variable 'Jwt__Secret' or key vault in production.");
    }
    else
    {
        throw new InvalidOperationException(
            "JWT Secret MUST be configured in production. " +
            "Set 'Jwt:Secret' via: environment variable 'Jwt__Secret', Azure Key Vault, AWS Secrets Manager, or Docker Secrets.");
    }
}

// Validate key length
if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret must be at least 32 characters long for secure AES encryption.");
}

// Add Authentication (shared JWT config with Store - allows Store tokens in Admin)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2Connect",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2Connect",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization(options => options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")));

// Database
var dbProvider = builder.Configuration["Database:Provider"] ?? "inmemory";
if (dbProvider.Equals("inmemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseInMemoryDatabase("CatalogDb"));

    // Error log storage - use in-memory for development
    builder.Services.AddInMemoryErrorLogStorage();
}
else if (dbProvider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Catalog")));

    // Error log storage - not supported on SQL Server, use in-memory fallback
    builder.Services.AddInMemoryErrorLogStorage();
}
else if (dbProvider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Catalog")));

    // Error log storage - use PostgreSQL
    var errorLogConnectionString = builder.Configuration.GetConnectionString("ErrorLogs")
        ?? builder.Configuration.GetConnectionString("Catalog");
    if (!string.IsNullOrEmpty(errorLogConnectionString))
    {
        builder.Services.AddPostgreSqlErrorLogStorage(errorLogConnectionString);
    }
}

// ==================== DATA ACCESS (ADR-025) ====================
// Add hybrid EF Core + Dapper data access for performance optimization
var connectionString = builder.Configuration.GetConnectionString("Catalog");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDataAccess(connectionString);
}

// ==================== TENANT CONTEXT ====================
// Register scoped tenant context service for request-level tenant isolation
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Register Repositories
builder.Services.AddScoped<IRepository<B2Connect.Admin.Core.Entities.Product>, Repository<B2Connect.Admin.Core.Entities.Product>>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();

// Legacy Application Services - DISABLED (Controllers use CQRS handlers via Wolverine)
// builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<IBrandService, BrandService>();

// Email Services
builder.Services.AddScoped<B2Connect.Email.Interfaces.IEmailService, B2Connect.Email.Services.SmtpEmailService>();
builder.Services.AddScoped<B2Connect.Email.Interfaces.IEmailQueueService, B2Connect.Email.Services.EmailQueueService>();
builder.Services.AddHostedService<B2Connect.Email.Handlers.ProcessEmailQueueJob>();

// Add services
builder.Services.AddControllers(options =>
{
    // Register global filters für alle Controller
    options.Filters.Add<ApiExceptionHandlingFilter>();
    options.Filters.Add<ValidateModelStateFilter>();
    options.Filters.Add<ApiLoggingFilter>();
});
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add Input Validation (FluentValidation)
// builder.Services.AddB2ConnectValidation();

// Configure HSTS options (production)
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Initialize databases (Store Context only)
try
{
    // EnsureUserDatabaseAsync removed - User management is handled by Identity Service
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<object>>();
    logger.LogError(ex, "Failed to initialize User database");
    throw;
}

// Service defaults middleware
app.UseServiceDefaults();

// Security Headers - apply early in pipeline
// app.UseSecurityHeaders();

// Rate Limiting - must be before routing
// app.UseRateLimiter();

// CORS must come before routing
app.UseCors("AllowAdminFrontend");

// ==================== TENANT CONTEXT MIDDLEWARE ====================
// Extract X-Tenant-ID header and populate scoped ITenantContext
// Must be before authentication/authorization for proper tenant isolation
app.UseMiddleware<TenantContextMiddleware>();

// HTTPS Redirection & HSTS (Security Headers)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS: Strict-Transport-Security header
    app.UseHttpsRedirection();
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();
app.MapGet("/", () => "Admin API Gateway is running");
// Health endpoints provided by UseServiceDefaults() - see ADR-025

await app.RunAsync();
