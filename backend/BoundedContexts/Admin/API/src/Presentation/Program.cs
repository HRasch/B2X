using B2Connect.ServiceDefaults;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Application.Services;
using B2Connect.Admin.Infrastructure.Repositories;
using B2Connect.Admin.Infrastructure.Data;
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

// Add CORS for Admin Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5174",
                "http://127.0.0.1:5174",
                "https://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "X-Total-Count");
    });
});

// Add Authentication (shared JWT config with Store - allows Store tokens in Admin)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "B2Connect-Super-Secret-Key-For-Development-Only-32chars!";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2Connect",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2Connect", // Same as Store
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
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseInMemoryDatabase("CatalogDb"));
}
else if (dbProvider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Catalog")));
}
else if (dbProvider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<CatalogDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Catalog")));
}

// Register Repositories
builder.Services.AddScoped<IRepository<B2Connect.Admin.Core.Entities.Product>, Repository<B2Connect.Admin.Core.Entities.Product>>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "B2Connect Admin API",
        Version = "v1",
        Description = "Admin Gateway for Product Catalog Management",
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API v1");
        c.RoutePrefix = "swagger";
    });
}

// CORS must come before routing
app.UseCors("AllowAdminFrontend");

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();
app.MapGet("/", () => "Admin API Gateway is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", gateway = "admin" }));

await app.RunAsync();
