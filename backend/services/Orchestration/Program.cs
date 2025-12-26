var builder = DistributedApplication.CreateBuilder(args);

// API Gateway
var gateway = builder
    .AddProject("api-gateway", "../Gateway/B2Connect.Gateway.csproj")
    .WithHttpEndpoint(port: 6000, targetPort: 80, name: "http-gateway");

// Auth Service (Identity)
var authService = builder
    .AddProject("auth-service", "../Identity/B2Connect.Identity.API.csproj")
    .WithHttpEndpoint(port: 9002, targetPort: 80, name: "http-auth")
    .WithEnvironment("Database__Provider", "inmemory");

// Tenant Service
var tenantService = builder
    .AddProject("tenant-service", "../Tenancy/B2Connect.Tenancy.API.csproj")
    .WithHttpEndpoint(port: 9003, targetPort: 80, name: "http-tenant")
    .WithEnvironment("Database__Provider", "inmemory");

// Localization Service
var localizationService = builder
    .AddProject("localization-service", "../Localization/B2Connect.Localization.API.csproj")
    .WithHttpEndpoint(port: 9004, targetPort: 80, name: "http-localization")
    .WithEnvironment("Database__Provider", "inmemory");

// Catalog Service
var catalogService = builder
    .AddProject("catalog-service", "../Catalog/B2Connect.Catalog.API.csproj")
    .WithHttpEndpoint(port: 9001, targetPort: 80, name: "http-catalog")
    .WithEnvironment("Database__Provider", "inmemory");

// Layout Service
var layoutService = builder
    .AddProject("layout-service", "../Theming/Layout/B2Connect.Theming.Layout.csproj")
    .WithHttpEndpoint(port: 9005, targetPort: 80, name: "http-layout")
    .WithEnvironment("Database__Provider", "inmemory");

builder.Build().Run();

