var builder = DistributedApplication.CreateBuilder(args);

// API Gateway
var gateway = builder
    .AddProject("api-gateway", "../Gateway/B2Connect.Gateway.csproj")
    .WithHttpEndpoint(port: 6000, name: "http-gateway")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:6000");

// Auth Service (Identity)
var authService = builder
    .AddProject("auth-service", "../Identity/B2Connect.Identity.API.csproj")
    .WithHttpEndpoint(port: 9002, name: "http-auth")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9002");

// Tenant Service
var tenantService = builder
    .AddProject("tenant-service", "../Tenancy/B2Connect.Tenancy.API.csproj")
    .WithHttpEndpoint(port: 9003, name: "http-tenant")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9003");

// Localization Service
var localizationService = builder
    .AddProject("localization-service", "../Localization/B2Connect.Localization.API.csproj")
    .WithHttpEndpoint(port: 9004, name: "http-localization")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9004");

// Catalog Service
var catalogService = builder
    .AddProject("catalog-service", "../Catalog/B2Connect.Catalog.API.csproj")
    .WithHttpEndpoint(port: 9005, name: "http-catalog")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9005");

// Layout Service
var layoutService = builder
    .AddProject("layout-service", "../Theming/Layout/B2Connect.Theming.Layout.csproj")
    .WithHttpEndpoint(port: 9006, name: "http-layout")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9006");

builder.Build().Run();