using B2Connect.Domain.Support.Services;
using B2Connect.Gateway.Shared.Application;
using B2Connect.Gateway.Shared.Presentation.Controllers;
using B2Connect.Shared.Core;
using B2Connect.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== SECURITY =====

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "dev-jwt-secret-min-32-chars-required!!!";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SupportAccess", policy => policy.RequireRole("Admin", "Support"));
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Store Frontend
                "http://localhost:5174", // Admin Frontend
                "https://localhost:5173",
                "https://localhost:5174"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ===== FEEDBACK DOMAIN SERVICES =====

// Register Domain Services
builder.Services.AddScoped<IDataAnonymizer, DataAnonymizer>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IFeedbackValidator, FeedbackValidator>();
builder.Services.AddScoped<IMaliciousRequestAnalyzer, MLMaliciousRequestAnalyzer>();

// Register Repositories (in-memory for now, can be replaced with EF Core later)
builder.Services.AddSingleton<IFeedbackRepository, InMemoryFeedbackRepository>();

// Configure Validation Rules
builder.Services.Configure<ValidationRules>(builder.Configuration.GetSection("Feedback:Validation"));

// ===== WOLVERINE CQRS =====
builder.Services.AddWolverine(opts =>
{
    // Register command handlers
    opts.Discovery.IncludeAssembly(typeof(CreateFeedbackHandler).Assembly);

    // Register query handlers
    opts.Discovery.IncludeAssembly(typeof(GetFeedbackStatusHandler).Assembly);
});

// ===== HTTP CLIENT =====
builder.Services.AddHttpClient<GitHubService>((sp, client) =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ===== HEALTH CHECKS =====
builder.Services.AddHealthChecks();

// ===== LOGGING =====
builder.Services.AddLogging(logging =>
{
    logging.AddSerilog();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Security middleware
app.UseCors("AllowFrontends");
app.UseAuthentication();
app.UseAuthorization();

// Health checks
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

// Wolverine middleware
app.MapWolverineEndpoints();

app.Run();</ content >
< parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Gateway / Shared / src / Program.cs