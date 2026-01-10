using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging - Console + File
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/admin-gateway-.txt",
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: Serilog.RollingInterval.Day)
        .ReadFrom.Configuration(context.Configuration);
});

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

                    return string.Equals(uri.Host, "localhost", StringComparison.Ordinal) ||
                           string.Equals(uri.Host, "127.0.0.1", StringComparison.Ordinal);
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "X-Total-Count");
        }
        else
        {
            // In production, use strict origins from configuration
            var corsOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            if (corsOrigins != null && corsOrigins.Length > 0)
            {
                policy
                    .WithOrigins(corsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition", "X-Total-Count");
            }
        }
    });
});

// Add Authentication (shared JWT config with Store)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2X",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2X",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Secret"] ?? "dev-only-secret-minimum-32-chars-required!"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Admin Frontend - DomainAdmins (DU) and TenantAdmins (SU) allowed
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        {
            // Check AccountType claim (DU or SU)
            var accountTypeClaim = context.User.FindFirst("AccountType");
            var isAdminAccountType = accountTypeClaim != null &&
                   (string.Equals(accountTypeClaim.Value, "DU", StringComparison.Ordinal) ||
                    string.Equals(accountTypeClaim.Value, "SU", StringComparison.Ordinal));

            // Check Admin role
            var isAdminRole = context.User.IsInRole("Admin") ||
                             context.User.IsInRole("admin");

            // Check if user has any admin role
            var hasAdminRole = context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Any(c => c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                         c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase));

            return isAdminAccountType || isAdminRole || hasAdminRole;
        }));

    // Policy for content_manager role
    options.AddPolicy("ContentManager", policy =>
        policy.RequireAssertion(context =>
            context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Any(c => c.Value.Equals("content_manager", StringComparison.OrdinalIgnoreCase) ||
                         c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))));

    // Policy for catalog_manager role
    options.AddPolicy("CatalogManager", policy =>
        policy.RequireAssertion(context =>
            context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Any(c => c.Value.Equals("catalog_manager", StringComparison.OrdinalIgnoreCase) ||
                         c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))));
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// CORS must come before routing
app.UseCors("AllowAdminFrontend");

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

await app.RunAsync().ConfigureAwait(false);
