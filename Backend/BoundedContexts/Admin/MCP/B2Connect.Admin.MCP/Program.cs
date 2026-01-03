using B2Connect.Admin.MCP.Services;
using B2Connect.Admin.MCP.Middleware;
using B2Connect.Admin.MCP.Configuration;
using B2Connect.Admin.MCP.Data;
using B2Connect.Admin.MCP;
using Wolverine;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container
builder.Services.AddOpenApi();

// Database configuration
builder.Services.AddDbContext<McpDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantAdmin", policy =>
        policy.RequireClaim("role", "tenant-admin"));
});

// AI Consumption Gateway (Exclusive AI Access)
builder.Services.AddSingleton<AiConsumptionGateway>();

// MCP Server
builder.Services.AddSingleton<IMcpServer, McpServer>();

// AI Providers using Microsoft.Extensions.AI
builder.Services.AddSingleton<OpenAiProvider>();
builder.Services.AddSingleton<AnthropicProvider>();
builder.Services.AddSingleton<AzureOpenAiProvider>();
builder.Services.AddSingleton<OllamaProvider>();
builder.Services.AddSingleton<GitHubModelsProvider>();

// Data Sanitization Service (OWASP compliance)
builder.Services.Configure<DataSanitizationOptions>(builder.Configuration.GetSection("DataSanitization"));
builder.Services.AddSingleton<DataSanitizationService>();

// AI Provider Selector
builder.Services.AddSingleton<AiProviderSelector>();

// Tenant Context
builder.Services.AddScoped<TenantContext>();

// MCP Tools
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.CmsPageDesignTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.EmailTemplateDesignTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.SystemHealthAnalysisTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.UserManagementAssistantTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.ContentOptimizationTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.TenantManagementTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.StoreOperationsTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.SecurityComplianceTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.PerformanceOptimizationTool>();
builder.Services.AddScoped<B2Connect.Admin.MCP.Tools.IntegrationManagementTool>();

// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Wolverine CQRS
builder.Services.AddWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Security middleware
app.UseAuthentication();
app.UseAuthorization();

// Tenant context middleware (must be after auth)
app.UseMiddleware<TenantContextMiddleware>();

// AI Consumption Control middleware (exclusive access)
app.UseMiddleware<AiConsumptionControlMiddleware>();

// MCP endpoints
app.MapMcpEndpoints();

// Health check
app.MapHealthChecks("/health");

// Remove default weather forecast endpoint
// (keeping for now as placeholder)

app.Run();
