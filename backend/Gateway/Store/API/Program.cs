using B2Connect.ServiceDefaults;
using B2Connect.Shared.Infrastructure.Extensions;
using B2Connect.Shared.Infrastructure.Authorization;
using B2Connect.Shared.Middleware;
using B2Connect.Utils.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Logging - Console + File
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        // .Enrich.WithSensitiveDataRedaction() // Redact credentials from logs - disabled pending infrastructure setup
        .WriteTo.Console()
        .WriteTo.File(
            "logs/store-gateway-.txt",
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
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "https://localhost:5173"
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

// Add CORS for Store Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStoreFrontend", policy =>
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
                .WithExposedHeaders("Content-Disposition");
        }
        else
        {
            // In production, use strict origins from configuration
            policy
                .WithOrigins(corsOrigins!)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        }
    });
});

// Add Rate Limiting
// builder.Services.AddB2ConnectRateLimiting(builder.Configuration); // Disabled pending infrastructure setup

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

// Add JWT Authentication (shared with Admin - same secret)
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

        // Optional: Allow tokens in query string for SignalR/WebSockets
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Store Frontend - All account types allowed (DU, SU, U, SR)
    options.AddPolicy("StoreAccess", policy =>
        policy.RequireAssertion(context =>
        {
            var accountTypeClaim = context.User.FindFirst("AccountType");
            return accountTypeClaim != null &&
                   (accountTypeClaim.Value == "DU" || accountTypeClaim.Value == "SU" ||
                    accountTypeClaim.Value == "U" || accountTypeClaim.Value == "SR");
        }));

    // ERP Service Account Policies - Based on permissions from ERP connector
    options.AddPolicy("ErpReadCustomers", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("ReadCustomers") == true;
        }));

    options.AddPolicy("ErpUpdateCustomers", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("UpdateCustomers") == true;
        }));

    options.AddPolicy("ErpReadProducts", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("ReadProducts") == true;
        }));

    options.AddPolicy("ErpUpdateProducts", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("UpdateProducts") == true;
        }));

    options.AddPolicy("ErpReadUsageStats", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("ReadUsageStats") == true;
        }));

    options.AddPolicy("ErpManageAccess", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("ManageAccess") == true;
        }));

    options.AddPolicy("ErpReceiveWebhooks", policy =>
        policy.RequireAssertion(context =>
        {
            var isErpServiceAccount = context.User.FindFirst("IsErpServiceAccount")?.Value == "true";
            var permissions = context.User.FindFirst("ErpPermissions")?.Value;
            return isErpServiceAccount && permissions?.Contains("ReceiveWebhooks") == true;
        }));
});

// Add MVC Controllers
builder.Services.AddControllers();

// Add Input Validation (FluentValidation)
// builder.Services.AddB2ConnectValidation(); // Disabled pending infrastructure setup

// Configure HSTS options (production)
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        // Forward authentication header
        builderContext.AddRequestTransform(async transformContext =>
        {
            var authHeader = transformContext.HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }
        });
    });

// Add HTTP Context Accessor (required for authorization accessors)
builder.Services.AddHttpContextAccessor();

// Add Tenant Context (required for tenant settings accessor)
builder.Services.AddScoped<B2Connect.Shared.Tenancy.Infrastructure.Context.ITenantContext, B2Connect.Shared.Tenancy.Infrastructure.Context.TenantContext>();

// Add Tenant Context Accessor (required for StoreAccessMiddleware)
builder.Services.AddScoped<B2Connect.Shared.Middleware.ITenantContextAccessor, B2Connect.Shared.Middleware.TenantContextAccessor>();

// Add Unified Authorization System
builder.Services.AddUnifiedAuthorization();
builder.Services.AddScoped<B2Connect.Shared.Infrastructure.Authorization.ITenantSettingsAccessor, B2Connect.Gateway.Store.Authorization.TenantSettingsAccessor>();
builder.Services.AddScoped<B2Connect.Shared.Infrastructure.Authorization.IUserPermissionAccessor, B2Connect.Gateway.Store.Authorization.UserPermissionAccessor>();
builder.Services.AddScoped<B2Connect.Shared.Infrastructure.Authorization.IRolePermissionAccessor, B2Connect.Gateway.Store.Authorization.RolePermissionAccessor>();

var app = builder.Build();
app.UseRouting();

// Security Headers - apply early in pipeline
// app.UseSecurityHeaders(); // Disabled pending infrastructure setup

// Rate Limiting - must be before routing
// app.UseRateLimiter(); // Disabled pending infrastructure setup

app.UseCors("AllowStoreFrontend");

// ==================== TENANT CONTEXT MIDDLEWARE ====================
// Custom middleware that extends the base TenantContextMiddleware to also set ITenantContext
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
        var tenantContextAccessor = context.RequestServices.GetRequiredService<B2Connect.Shared.Middleware.ITenantContextAccessor>();
        var tenantContext = (B2Connect.Shared.Tenancy.Infrastructure.Context.TenantContext)context.RequestServices.GetRequiredService<B2Connect.Shared.Tenancy.Infrastructure.Context.ITenantContext>();

        tenantContextAccessor.SetTenantId(tenantId);
        tenantContext.TenantId = tenantId;
        context.Items["TenantId"] = tenantId;
    }

    await next(context);
});

// HTTPS Redirection & HSTS (Security Headers)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS: Strict-Transport-Security header
    app.UseHttpsRedirection();
}

// Service Defaults (Health checks, Service Discovery)
app.UseServiceDefaults();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Store Access Control (must be after authentication)
app.UseStoreAccess();

// Mock endpoints for development (when services are not available)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/catalog/products", () => Results.Ok(new[]
    {
        new { id = 1, name = "Sample Product 1", price = 29.99, category = "Electronics" },
        new { id = 2, name = "Sample Product 2", price = 49.99, category = "Books" },
        new { id = 3, name = "Sample Product 3", price = 19.99, category = "Clothing" }
    }));

    app.MapGet("/api/catalog/brands", () => Results.Ok(new[]
    {
        new { id = 1, name = "Sample Brand 1" },
        new { id = 2, name = "Sample Brand 2" }
    }));

    app.MapGet("/api/catalog/categories", () => Results.Ok(new[]
    {
        new { id = 1, name = "Electronics" },
        new { id = 2, name = "Books" },
        new { id = 3, name = "Clothing" }
    }));
}

// Map API Controllers
app.MapControllers();

// YARP Reverse Proxy
app.MapReverseProxy();

app.Run();
