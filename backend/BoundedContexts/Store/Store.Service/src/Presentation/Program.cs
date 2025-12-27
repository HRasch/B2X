using B2Connect.ServiceDefaults;
using B2Connect.Store.Core.Common.Interfaces;
using B2Connect.Store.Core.Store.Interfaces;
using B2Connect.Store.Application.Store.Services;
using B2Connect.Store.Application.Store.ReadServices;
using B2Connect.Store.Infrastructure.Common.Repositories;
using B2Connect.Store.Infrastructure.Common.Data;
using B2Connect.Store.Infrastructure.Store.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, etc.)
builder.Host.AddServiceDefaults();

// Add CORS for Store Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStoreFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "X-Total-Count");
    });
});

// Get JWT Secret from configuration
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSecret = "dev-only-secret-minimum-32-chars-required!";
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        logger.LogWarning(
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

// Add Authentication
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
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2Connect-Store",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Database
var dbProvider = builder.Configuration["Database:Provider"] ?? "inmemory";
if (dbProvider.Equals("inmemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<StoreDbContext>(opt =>
        opt.UseInMemoryDatabase("StoreDb"));
}
else if (dbProvider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<StoreDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Store")));
}
else if (dbProvider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<StoreDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Store")));
}

// Register Repositories (Common)
builder.Services.AddScoped<IRepository<B2Connect.Store.Core.Common.Entities.Shop>, Repository<B2Connect.Store.Core.Common.Entities.Shop>>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

// Register Repositories (Store-specific)
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IShippingMethodRepository, ShippingMethodRepository>();

// Register Application Services (Write Services)
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IShippingMethodService, ShippingMethodService>();

// Register Read Services (Optimized for public API)
builder.Services.AddScoped<IShopReadService, ShopReadService>();
builder.Services.AddScoped<ILanguageReadService, LanguageReadService>();
builder.Services.AddScoped<ICountryReadService, CountryReadService>();
builder.Services.AddScoped<IPaymentMethodReadService, PaymentMethodReadService>();
builder.Services.AddScoped<IShippingMethodReadService, ShippingMethodReadService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "B2Connect Store Configuration API",
        Version = "v1",
        Description = "Store, Language, Country, Payment and Shipping Methods Management",
    });

    // Add JWT Bearer auth to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Service defaults middleware
app.UseServiceDefaults();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Configuration API v1");
        c.RoutePrefix = "swagger";
    });
}

// CORS must come before routing
app.UseCors("AllowStoreFrontend");

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();
app.MapGet("/", () => "Store Configuration Service is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "store-config" }));

await app.RunAsync();

