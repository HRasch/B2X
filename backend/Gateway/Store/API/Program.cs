using B2Connect.ServiceDefaults;
using B2Connect.Shared.Infrastructure.Extensions;
using B2Connect.Middleware;
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
            rollingInterval: Serilog.RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
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
                    if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                    }
                    return false;
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

builder.Services.AddAuthorization();

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

var app = builder.Build();
app.UseRouting();

// Security Headers - apply early in pipeline
// app.UseSecurityHeaders(); // Disabled pending infrastructure setup

// Rate Limiting - must be before routing
// app.UseRateLimiter(); // Disabled pending infrastructure setup

app.UseCors("AllowStoreFrontend");

// HTTPS Redirection & HSTS (Security Headers)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS: Strict-Transport-Security header
    app.UseHttpsRedirection();
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// YARP Reverse Proxy
app.MapReverseProxy();

app.Run();
