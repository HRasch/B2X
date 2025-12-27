var builder = DistributedApplication.CreateBuilder(args);

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject<Projects.B2Connect_Store>("store-gateway")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7000");

// Admin API Gateway (for frontend-admin, protected CRUD endpoints)
var adminGateway = builder
    .AddProject<Projects.B2Connect_Admin>("admin-gateway")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7001");

// Auth Service (Identity)
var authService = builder
    .AddProject<Projects.B2Connect_Identity_API>("auth-service")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7002");

// Tenant Service
var tenantService = builder
    .AddProject<Projects.B2Connect_Tenancy_API>("tenant-service")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7003");

// Localization Service
var localizationService = builder
    .AddProject<Projects.B2Connect_Localization_API>("localization-service")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7004");

// Catalog Service
var catalogService = builder
    .AddProject<Projects.B2Connect_Catalog_API>("catalog-service")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7005");

// Theming Service
var themingService = builder
    .AddProject<Projects.B2Connect_Theming_API>("theming-service")
    .WithEnvironment("Database__Provider", "inmemory")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7008");

// Frontend Store (Vite dev server)
var frontendStore = builder
    .AddNpmApp("frontend-store", "../../frontend-store", "dev")
    .WithHttpEndpoint(port: 5173, targetPort: 5173, name: "vite-store", isProxied: false)
    .WithEnvironment("BROWSER", "none");

// Frontend Admin (Vite dev server)
var frontendAdmin = builder
    .AddNpmApp("frontend-admin", "../../frontend-admin", "dev")
    .WithHttpEndpoint(port: 5174, targetPort: 5174, name: "vite-admin", isProxied: false)
    .WithEnvironment("BROWSER", "none");

builder.Build().Run();