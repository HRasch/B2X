using B2Connect.ServiceDefaults;
using B2Connect.AuthService.Data;
using B2Connect.Shared.Infrastructure;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        // // .Enrich.WithSensitiveDataRedaction() // Disabled
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, Service Discovery)
builder.Host.AddServiceDefaults();

// Add Wolverine with HTTP Endpoints
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"] ?? "amqp://guest:guest@localhost:5672";
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "IdentityService";

    // Enable HTTP Endpoints
    // opts.Http.EnableEndpoints = true;  // TODO: Enable when Wolverine HTTP is properly configured

    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // RabbitMQ integration (requires Wolverine.RabbitMq package)
    // if (useRabbitMq)
    // {
    //     opts.UseRabbitMq(rabbitMqUri);
    // }
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Add Database
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDb") ?? "Data Source=auth.db");
});

// Add Identity
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

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
            "http://localhost:5174",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5174"
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

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2Connect",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2Connect.Admin",
        ValidateLifetime = true
    };
});

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            // .WithMaxAge(TimeSpan...) // Disabled
            ;
    });
});

// Add Rate Limiting
// builder.Services.AddB2ConnectRateLimiting(builder.Configuration);

// Add Input Validation (FluentValidation)
// builder.Services.AddB2ConnectValidation();

// Add services
// builder.Services.AddControllers(); // Removed - using Wolverine HTTP Endpoints
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure HSTS options (production)
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

// Security Headers - apply early in pipeline
// app.UseSecurityHeaders();

// Rate Limiting - must be before routing
// app.UseRateLimiter();

// Migrate database and seed demo data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    db.Database.EnsureCreated();

    // Seed demo roles if they don't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Id = "admin-role",
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Administrator role with full access"
        });
        logger.LogInformation("✅ Admin role created");
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Id = "user-role",
            Name = "User",
            NormalizedName = "USER",
            Description = "Standard user role"
        });
        logger.LogInformation("✅ User role created");
    }

    // Seed demo admin account if it doesn't exist
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        var newAdminUser = new AppUser
        {
            Id = "admin-001",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            FirstName = "Admin",
            LastName = "User",
            TenantId = "default",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newAdminUser, "password");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Admin");
            logger.LogInformation("✅ Demo admin account created (admin@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create demo admin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed demo user account if it doesn't exist
    var demoUser = await userManager.FindByEmailAsync("user@example.com");
    if (demoUser == null)
    {
        var newDemoUser = new AppUser
        {
            Id = "user-001",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            UserName = "user@example.com",
            NormalizedUserName = "USER@EXAMPLE.COM",
            FirstName = "Demo",
            LastName = "User",
            TenantId = "default",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newDemoUser, "password");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newDemoUser, "User");
            logger.LogInformation("✅ Demo user account created (user@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create demo user account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// Service defaults middleware
app.UseServiceDefaults();

// CORS middleware - MUST be before auth
app.UseCors("AllowFrontend");

// HTTPS Redirection & HSTS (Security Headers)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS: Strict-Transport-Security header
}
app.UseHttpsRedirection();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Wolverine HTTP Endpoints (replaces MapControllers)
app.MapWolverineEndpoints();

app.MapGet("/", () => "Auth Service is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

await app.RunAsync();

