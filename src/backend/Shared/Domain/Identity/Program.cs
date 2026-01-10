using System.Text;
using B2X.Identity.Data;
using B2X.Identity.Handlers;
using B2X.Identity.Infrastructure;
using B2X.Identity.Infrastructure.Middleware;
using B2X.Identity.Interfaces;
using B2X.Identity.Services;
using B2X.ServiceDefaults;
using B2X.Shared.Infrastructure;
using B2X.Shared.Infrastructure.Validation;
using B2X.Shared.Messaging.Extensions;
using B2X.Shared.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURATION VALIDATION =====
// Validate configuration early to fail fast with clear error messages
try
{
    using var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
    var validator = new ConfigurationValidator(builder.Configuration, loggerFactory.CreateLogger<ConfigurationValidator>(), loggerFactory);
    validator.ValidateAll();
}
catch (ConfigurationValidationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ CONFIGURATION VALIDATION FAILED");
    Console.WriteLine("===================================");
    foreach (var error in ex.Errors)
    {
        Console.WriteLine(error.ToString());
        Console.WriteLine();
    }
    Console.ResetColor();
    throw;
}

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
var databaseProvider = builder.Configuration["Database:Provider"] ?? builder.Configuration["Database__Provider"] ?? "sqlite";
if (string.Equals(databaseProvider.ToLower(System.Globalization.CultureInfo.InvariantCulture), "inmemory", StringComparison.Ordinal))
{
    builder.Services.AddDbContext<AuthDbContext>(options => options.UseInMemoryDatabase("AuthDb"));
}
else
{
    builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AuthDb") ?? "Data Source=auth.db"));
}

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

// Add JWT Authentication (cookie-based)
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2X",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2X.Admin",
        ValidateLifetime = true
    };

    // Configure JWT to read from httpOnly cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Try to get token from httpOnly cookie
            var token = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Authorization (REQUIRED for [Authorize] attributes)
// NOTE: We do NOT set a FallbackPolicy because:
// - No FallbackPolicy = anonymous access allowed by default
// - [AllowAnonymous] explicitly allows unauthenticated requests
// - [Authorize] explicitly requires authentication
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
            .AllowCredentials() // Required for httpOnly cookies
                                // .WithMaxAge(TimeSpan...) // Disabled
            ;
    });
});

// Add Rate Limiting
// builder.Services.AddB2XRateLimiting(builder.Configuration);

// Add Input Validation (FluentValidation)
// builder.Services.AddB2XValidation();

// Add services
builder.Services.AddControllers();

// Add MediatR for CQRS - NO! Using Wolverine instead
// builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add FluentValidation for input validation
// builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped<CheckRegistrationTypeCommandValidator>();

// Add distributed caching for ERP customer data
// Use memory cache for development (StackExchangeRedis can be added for production)
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

// CSRF Protection Middleware is activated via UseMiddleware<T>(), not DI registration

// Add custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IErpCustomerService, ErpCustomerService>();
builder.Services.AddScoped<IDuplicateDetectionService, DuplicateDetectionService>();
builder.Services.AddScoped<CheckRegistrationTypeService>();

// Add HttpClient for ERP integration with Polly resilience policies
builder.Services
    .AddHttpClient<IErpCustomerService, ErpCustomerService>()
    .AddErpResiliencePolicies();

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

    db.Database.EnsureCreatedAsync().GetAwaiter().GetResult();

    // Seed demo roles if they don't exist
    if (!await roleManager.RoleExistsAsync("SuperAdmin").ConfigureAwait(false))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Id = "superadmin-role",
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN",
            Description = "Platform-level administrator with full access to Management portal"
        }).ConfigureAwait(false);
        logger.LogInformation("✅ SuperAdmin role created");
    }

    if (!await roleManager.RoleExistsAsync("Admin").ConfigureAwait(false))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Id = "admin-role",
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Administrator role with full access"
        }).ConfigureAwait(false);
        logger.LogInformation("✅ Admin role created");
    }

    if (!await roleManager.RoleExistsAsync("User").ConfigureAwait(false))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Id = "user-role",
            Name = "User",
            NormalizedName = "USER",
            Description = "Standard user role"
        }).ConfigureAwait(false);
        logger.LogInformation("✅ User role created");
    }

    // Seed demo admin account if it doesn't exist
    var adminUser = await userManager.FindByEmailAsync("admin@example.com").ConfigureAwait(false);
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
            IsTwoFactorRequired = !app.Environment.IsDevelopment(), // 2FA required in production for DomainAdmins
            IsDeletable = false, // DomainAdmins cannot be deleted
            AccountType = AccountType.DU, // DomainAdmin
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newAdminUser, "password").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Admin").ConfigureAwait(false);
            logger.LogInformation("✅ Demo DomainAdmin account created (admin@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create demo DomainAdmin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed demo user account if it doesn't exist
    var demoUser = await userManager.FindByEmailAsync("user@example.com").ConfigureAwait(false);
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
            AccountType = AccountType.U, // User
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newDemoUser, "password").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newDemoUser, "User").ConfigureAwait(false);
            logger.LogInformation("✅ Demo user account created (user@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create demo user account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed domain admin account if it doesn't exist
    var domainAdminUser = await userManager.FindByEmailAsync("domanadmin.example.com").ConfigureAwait(false);
    if (domainAdminUser == null)
    {
        var newDomainAdminUser = new AppUser
        {
            Id = "domainadmin-001",
            Email = "domanadmin.example.com",
            NormalizedEmail = "DOMANADMIN.EXAMPLE.COM",
            UserName = "domanadmin.example.com",
            NormalizedUserName = "DOMANADMIN.EXAMPLE.COM",
            FirstName = "Domain",
            LastName = "Admin",
            TenantId = "default",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = !app.Environment.IsDevelopment(), // 2FA required in production for DomainAdmins
            IsDeletable = false, // DomainAdmins cannot be deleted
            AccountType = AccountType.DU, // DomainAdmin
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newDomainAdminUser, "password").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newDomainAdminUser, "Admin").ConfigureAwait(false);
            logger.LogInformation("✅ Domain admin account created (domanadmin.example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create domain admin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed tenant admin account if it doesn't exist
    var tenantAdminUser = await userManager.FindByEmailAsync("tenantadmin@example.com").ConfigureAwait(false);
    if (tenantAdminUser == null)
    {
        var newTenantAdminUser = new AppUser
        {
            Id = "tenantadmin-001",
            Email = "tenantadmin@example.com",
            NormalizedEmail = "TENANTADMIN@EXAMPLE.COM",
            UserName = "tenantadmin@example.com",
            NormalizedUserName = "TENANTADMIN@EXAMPLE.COM",
            FirstName = "Tenant",
            LastName = "Admin",
            TenantId = "tenant-001",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = false, // TenantAdmins can have 2FA optional
            IsDeletable = true,
            AccountType = AccountType.SU, // TenantAdmin
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newTenantAdminUser, "password").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newTenantAdminUser, "Admin").ConfigureAwait(false);
            logger.LogInformation("✅ Tenant admin account created (tenantadmin@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create tenant admin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed SuperAdmin account for Management portal if it doesn't exist
    var superAdminUser = await userManager.FindByEmailAsync("superadmin@B2X.io").ConfigureAwait(false);
    if (superAdminUser == null)
    {
        var newSuperAdminUser = new AppUser
        {
            Id = "superadmin-001",
            Email = "superadmin@B2X.io",
            NormalizedEmail = "SUPERADMIN@B2X.IO",
            UserName = "superadmin@B2X.io",
            NormalizedUserName = "SUPERADMIN@B2X.IO",
            FirstName = "Super",
            LastName = "Admin",
            TenantId = null, // Platform-level, no tenant
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = !app.Environment.IsDevelopment(), // 2FA required in production
            IsDeletable = false, // SuperAdmins cannot be deleted
            AccountType = AccountType.SA, // SuperAdmin
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newSuperAdminUser, "SuperAdmin123!").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newSuperAdminUser, "SuperAdmin").ConfigureAwait(false);
            logger.LogInformation("✅ SuperAdmin account created for Management portal (superadmin@B2X.io / SuperAdmin123!)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create SuperAdmin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed sales rep account if it doesn't exist
    var salesRepUser = await userManager.FindByEmailAsync("salesrep@example.com").ConfigureAwait(false);
    if (salesRepUser == null)
    {
        var newSalesRepUser = new AppUser
        {
            Id = "salesrep-001",
            Email = "salesrep@example.com",
            NormalizedEmail = "SALESREP@EXAMPLE.COM",
            UserName = "salesrep@example.com",
            NormalizedUserName = "SALESREP@EXAMPLE.COM",
            FirstName = "Sales",
            LastName = "Representative",
            TenantId = "tenant-001",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = false,
            IsDeletable = true,
            AccountType = AccountType.SR, // SalesRep
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(newSalesRepUser, "password").ConfigureAwait(false);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newSalesRepUser, "User").ConfigureAwait(false);
            logger.LogInformation("✅ Sales rep account created (salesrep@example.com / password)");
        }
        else
        {
            logger.LogWarning("❌ Failed to create sales rep account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// Service defaults middleware
app.UseServiceDefaults();

// Global API exception handling (MUST be early in pipeline)
app.UseApiExceptionHandling();

// CORS middleware - MUST be before auth
app.UseCors("AllowFrontend");

// HTTPS Redirection & HSTS (Security Headers)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS: Strict-Transport-Security header
    app.UseHttpsRedirection();
}

// Middleware
app.UseAuthentication();

// CSRF Protection - must be after authentication for stateful requests
app.UseMiddleware<CsrfProtectionMiddleware>();

app.UseAuthorization();

// Map Controllers (for AuthController)
app.MapControllers();

// Map Wolverine HTTP Endpoints (includes all [WolverineHttpPost] handlers)
app.MapWolverineEndpoints();

app.MapGet("/", () => "Auth Service is running");
// Health endpoints provided by UseServiceDefaults() - see ADR-025

await app.RunAsync().ConfigureAwait(false);

