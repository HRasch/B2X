using B2X.AppHost.Configuration;
using B2X.AppHost.Extensions;
using B2X.Aspire.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// ===== TESTING CONFIGURATION =====
// Note: TestingConfiguration and TestDataOrchestrator are not registered as services
// in the AppHost since seeding functionality is now handled by the separate B2X.Seeding.API
// builder.Services.AddSingleton(testingConfig);
// builder.Services.AddTestDataOrchestrator(testingConfig);

// Add MVC services for web interface
// builder.Services.AddControllersWithViews(); // Removed - using separate seeding API

// JWT Secret Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? builder.Configuration["JWT_SECRET"]
    ?? "dev-jwt-secret-min-32-chars-required!!!";

// Check database provider
var databaseProvider = builder.Configuration["Database:Provider"]
    ?? builder.Configuration["Database__Provider"]
    ?? "postgres";

// ===== INFRASTRUCTURE =====
// Only add Docker-based infrastructure when not using in-memory databases

IResourceBuilder<PostgresDatabaseResource>? authDb = null;
IResourceBuilder<PostgresDatabaseResource>? tenantDb = null;
IResourceBuilder<PostgresDatabaseResource>? localizationDb = null;
IResourceBuilder<PostgresDatabaseResource>? catalogDb = null;
IResourceBuilder<PostgresDatabaseResource>? layoutDb = null;
IResourceBuilder<PostgresDatabaseResource>? adminDb = null;
IResourceBuilder<PostgresDatabaseResource>? storeDb = null;
IResourceBuilder<PostgresDatabaseResource>? monitoringDb = null;
IResourceBuilder<PostgresDatabaseResource>? smartDataIntegrationDb = null;

IResourceBuilder<RedisResource>? redis = null;
IResourceBuilder<ElasticsearchResource>? elasticsearch = null;
IResourceBuilder<RabbitMQServerResource>? rabbitmq = null;
IResourceBuilder<PostgresServerResource>? postgres = null;

if (!string.Equals(databaseProvider.ToLower(System.Globalization.CultureInfo.InvariantCulture), "inmemory", StringComparison.Ordinal))
{
    // PostgreSQL Database
    postgres = builder.AddB2XPostgres(
        name: "postgres",
        port: 5432,
        username: "postgres");

    // Create all databases
    authDb = postgres.AddB2XDatabase("auth");
    tenantDb = postgres.AddB2XDatabase("tenant");
    localizationDb = postgres.AddB2XDatabase("localization");
    catalogDb = postgres.AddB2XDatabase("catalog");
    layoutDb = postgres.AddB2XDatabase("layout");
    adminDb = postgres.AddB2XDatabase("admin");
    storeDb = postgres.AddB2XDatabase("store");
    monitoringDb = postgres.AddB2XDatabase("monitoring");
    smartDataIntegrationDb = postgres.AddB2XDatabase("smartdataintegration");

    // Redis Cache
    redis = builder.AddB2XRedis(
        name: "redis",
        port: 6379);

    // Elasticsearch (Full-Text Search & Analytics)
    elasticsearch = builder.AddB2XElasticsearch(
        name: "elasticsearch",
        port: 9200);

    // RabbitMQ (Asynchronous Message Queue)
    rabbitmq = builder.AddB2XRabbitMQ(
        name: "rabbitmq",
        port: 5672);
}
else
{
    // When running the full Aspire AppHost with in-memory DB for demos,
    // also provide an Elasticsearch resource to enable realistic search/indexing
    // for local development/demo scenarios.
    elasticsearch = builder.AddB2XElasticsearch(
        name: "elasticsearch",
        port: 9200);

    // RabbitMQ is also useful for demo indexing flows; add if needed
    rabbitmq = builder.AddB2XRabbitMQ(
        name: "rabbitmq",
        port: 5672);
}

// Azure Key Vault (Secret Store)

// ===== MICROSERVICES =====
// Using dynamic ports with Aspire Service Discovery (no more port conflicts!)
// Services are discovered via service name, not fixed ports.
// Aspire automatically assigns dynamic ports - no WithHttpEndpoint needed for internal services.

// STARTUP ORDER STRATEGY:
// 1. Infrastructure (Postgres, Redis, RabbitMQ, Elasticsearch) - via WaitFor
// 2. Core Services (Auth, Tenant, Localization) - no inter-service dependencies
// 3. Domain Services (Catalog, Theming, Monitoring) - may depend on core services
// 4. Gateways (Store, Admin) - depend on all services they aggregate
// 5. Frontends - depend on their respective gateways

// Auth Service (Identity) - with Passkeys & JWT
var authService = builder
    .AddProject("auth-service", "../../../Shared/Domain/Identity/B2X.Identity.API.csproj")
    .WithConditionalPostgresConnection(authDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithPasskeysAuth()
    .WithPasskeysJwt(builder.Configuration["Jwt:Secret"] ?? "dev-jwt-secret-min-32-chars-required!!!")
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting auth service
if (postgres != null)
    authService.WaitFor(postgres);
if (redis != null)
    authService.WaitFor(redis);
if (rabbitmq != null)
    authService.WaitFor(rabbitmq);

// Tenant Service
var tenantService = builder
    .AddProject("tenant-service", "../../../Shared/Domain/Tenancy/B2X.Tenancy.API.csproj")
    .WithConditionalPostgresConnection(tenantDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting tenant service
if (postgres != null)
    tenantService.WaitFor(postgres);
if (redis != null)
    tenantService.WaitFor(redis);
if (rabbitmq != null)
    tenantService.WaitFor(rabbitmq);

// Localization Service
var localizationService = builder
    .AddProject("localization-service", "../../../Shared/Domain/Localization/B2X.Localization.API.csproj")
    .WithConditionalPostgresConnection(localizationDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting localization service
if (postgres != null)
    localizationService.WaitFor(postgres);
if (redis != null)
    localizationService.WaitFor(redis);
if (rabbitmq != null)
    localizationService.WaitFor(rabbitmq);

// Catalog Service (with Elasticsearch for Product Search)
var catalogService = builder
    .AddProject("catalog-service", "../../../Store/Domain/Catalog/B2X.Catalog.API.csproj")
    .WithConditionalPostgresConnection(catalogDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithConditionalElasticsearchConnection(elasticsearch, databaseProvider, "catalog")
    .WithConditionalElasticsearchIndexing(databaseProvider)
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 90) // Longer timeout for Elasticsearch
    .WithResilienceConfiguration();

// Wait for infrastructure before starting catalog service
if (postgres != null)
    catalogService.WaitFor(postgres);
if (redis != null)
    catalogService.WaitFor(redis);
if (rabbitmq != null)
    catalogService.WaitFor(rabbitmq);
if (elasticsearch != null)
    catalogService.WaitFor(elasticsearch);

// Variants Service (Product Variants/SKUs)
var variantsService = builder
    .AddProject("variants-service", "../../../Store/Domain/Variants/src/B2X.Variants.csproj")
    .WithConditionalPostgresConnection(storeDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting variants service
if (postgres != null)
    variantsService.WaitFor(postgres);
if (redis != null)
    variantsService.WaitFor(redis);
if (rabbitmq != null)
    variantsService.WaitFor(rabbitmq);

// Categories Service (Product Categories)
var categoriesService = builder
    .AddProject("categories-service", "../../../Store/Domain/Categories/B2X.Categories.csproj")
    .WithConditionalPostgresConnection(storeDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting categories service
if (postgres != null)
    categoriesService.WaitFor(postgres);
if (redis != null)
    categoriesService.WaitFor(redis);
if (rabbitmq != null)
    categoriesService.WaitFor(rabbitmq);

// Theming Service
var themingService = builder
    .AddProject("theming-service", "../../../Shared/Domain/Theming/B2X.Theming.API.csproj")
    .WithConditionalPostgresConnection(layoutDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting theming service
if (postgres != null)
    themingService.WaitFor(postgres);
if (redis != null)
    themingService.WaitFor(redis);
if (rabbitmq != null)
    themingService.WaitFor(rabbitmq);

// Smart Data Integration Service
var smartDataIntegrationService = builder
    .AddProject("smartdataintegration-service", "../../../Shared/Domain/SmartDataIntegration/API/B2X.SmartDataIntegration.API.csproj")
    .WithConditionalPostgresConnection(smartDataIntegrationDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting smart data integration service
if (postgres != null)
    smartDataIntegrationService.WaitFor(postgres);
if (redis != null)
    smartDataIntegrationService.WaitFor(redis);
if (rabbitmq != null)
    smartDataIntegrationService.WaitFor(rabbitmq);

// Monitoring Service (Phase 2: Connected services monitoring, error logging)
var monitoringService = builder
    .AddProject("monitoring-service", "c:/Users/Holge/repos/B2Connect/src/BoundedContexts/Monitoring/API/B2X.Monitoring.csproj")
    .WithConditionalPostgresConnection(monitoringDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Wait for infrastructure before starting monitoring service
if (postgres != null)
    monitoringService.WaitFor(postgres);
if (redis != null)
    monitoringService.WaitFor(redis);
if (rabbitmq != null)
    monitoringService.WaitFor(rabbitmq);

// MCP Server (AI Assistant for Management Tasks)
var mcpServer = builder
    .AddProject("mcp-server", "c:/Users/Holge/repos/B2Connect/src/AI/MCP/B2X.Admin.MCP/B2X.Admin.MCP.csproj")
    .WithHttpEndpoint(port: 8090, name: "mcp-http")  // Fixed port for MCP server
    .WithReference(authService)
    .WithReference(tenantService)
    .WithReference(monitoringService)
    .WithConditionalPostgresConnection(adminDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 90)
    .WithResilienceConfiguration();

// MCP Server waits for its referenced services
mcpServer.WaitFor(authService);
mcpServer.WaitFor(tenantService);
mcpServer.WaitFor(monitoringService);

// Seeding API (Test Data Management)
var seedingApi = builder
    .AddProject("seeding-api", "c:/Users/Holge/repos/B2Connect/src/tools/seeders/seeding/B2X.Seeding.API.csproj")
    .WithHttpEndpoint(port: 8095, name: "seeding-http")  // Fixed port for seeding API
    .WithReference(authService)
    .WithReference(tenantService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithConditionalPostgresConnection(authDb, databaseProvider)
    .WithConditionalPostgresConnection(tenantDb, databaseProvider)
    .WithConditionalPostgresConnection(catalogDb, databaseProvider)
    .WithConditionalPostgresConnection(localizationDb, databaseProvider)
    .WithConditionalPostgresConnection(layoutDb, databaseProvider)
    .WithConditionalPostgresConnection(adminDb, databaseProvider)
    .WithConditionalPostgresConnection(storeDb, databaseProvider)
    .WithConditionalRedisConnection(redis, databaseProvider)
    .WithConditionalRabbitMQConnection(rabbitmq, databaseProvider)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry()
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 60)
    .WithResilienceConfiguration();

// Seeding API waits for its referenced services
seedingApi.WaitFor(authService);
seedingApi.WaitFor(tenantService);
seedingApi.WaitFor(catalogService);
seedingApi.WaitFor(localizationService);

// ===== API GATEWAYS =====
// Gateways keep fixed ports because frontends connect directly to them.
// Internal service communication uses Aspire Service Discovery.
// CRITICAL: Gateways wait for all services they aggregate to prevent hanging.

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject("store-gateway", "../../../Store/API/B2X.Store.csproj")
    .WithHttpEndpoint(port: 8000, name: "store-http")  // Fixed port for frontend
    .WithReference(authService)
    .WithReference(catalogService)
    .WithReference(variantsService)
    .WithReference(categoriesService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2XCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret)
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 120)
    .WithResilienceConfiguration();

// Store Gateway waits for all its referenced services
storeGateway.WaitFor(authService);
storeGateway.WaitFor(catalogService);
storeGateway.WaitFor(variantsService);
storeGateway.WaitFor(categoriesService);
storeGateway.WaitFor(localizationService);
storeGateway.WaitFor(themingService);

// Admin API Gateway 
var adminGateway = builder
    .AddProject("admin-gateway", "../../../Admin/Gateway/B2X.Admin.csproj")
    .WithHttpEndpoint(port: 8080, name: "admin-http")  // Fixed port for frontend
    .WithReference(authService)
    .WithReference(tenantService)
    .WithReference(catalogService)
    .WithReference(variantsService)
    .WithReference(categoriesService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithReference(monitoringService)
    .WithReference(mcpServer)
    .WithB2XCors("http://localhost:5174", "https://localhost:5174")
    .WithSecurityDefaults(jwtSecret)
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 120)
    .WithResilienceConfiguration();

// Admin Gateway waits for all its referenced services
adminGateway.WaitFor(authService);
adminGateway.WaitFor(tenantService);
adminGateway.WaitFor(catalogService);
adminGateway.WaitFor(variantsService);
adminGateway.WaitFor(categoriesService);
adminGateway.WaitFor(localizationService);
adminGateway.WaitFor(themingService);
adminGateway.WaitFor(monitoringService);
adminGateway.WaitFor(mcpServer);

// ===== FRONTENDS (Vite Vue.js Applications) =====
// Using native Aspire.Hosting.JavaScript integration (AddViteApp)
// Documentation: https://aspire.dev/integrations/frameworks/javascript/
// Fixed ports configured via WithEndpoint (see: https://github.com/dotnet/aspire/issues/12942)
// Frontends wait for their gateways to be ready before starting

// Frontend Store (Vue 3 + Vite) - Fixed port 5173 for predictable URLs
var frontendStore = builder
    .AddViteApp("frontend-store", "../frontend/Store")
    .WithEndpoint("http", endpoint => endpoint.Port = 5173)  // Workaround: modify existing endpoint
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])  // Force install to handle platform-specific packages
    .WithEnvironment("PORT", "5173")  // Vite reads PORT env var
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000")
    .WithEnvironment("NODE_ENV", "development");

// Frontend Store waits for Store Gateway
frontendStore.WaitFor(storeGateway);

// Frontend Admin (Vue 3 + Vite) - Fixed port 5174 for predictable URLs
var frontendAdmin = builder
    .AddViteApp("frontend-admin", "../frontend/Admin")
    .WithEndpoint("http", endpoint => endpoint.Port = 5174)  // Workaround: modify existing endpoint
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])  // Force install to handle platform-specific packages
    .WithEnvironment("PORT", "5174")  // Vite reads PORT env var
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8080")
    .WithEnvironment("NODE_ENV", "development");

// Frontend Admin waits for Admin Gateway
frontendAdmin.WaitFor(adminGateway);

// Frontend Management (Vue 3 + Vite) - Tenant Management Portal - Fixed port 5175
var frontendManagement = builder
    .AddViteApp("frontend-management", "../frontend/Management")
    .WithEndpoint("http", endpoint => endpoint.Port = 5175)  // Workaround: modify existing endpoint
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])  // Force install to handle platform-specific packages
    .WithEnvironment("PORT", "5175")  // Vite reads PORT env var
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8080")
    .WithEnvironment("VITE_MCP_SERVER_URL", "http://localhost:8090")
    .WithEnvironment("NODE_ENV", "development");

// Frontend Management waits for Admin Gateway and MCP Server
frontendManagement.WaitFor(adminGateway);
frontendManagement.WaitFor(mcpServer);

// Issue #50: Vite build errors are automatically captured by Aspire's built-in logging
// Logs appear in Aspire Dashboard under each frontend resource
// No custom code needed - AddViteApp() handles stdout/stderr forwarding

var app = builder.Build();
await app.RunAsync().ConfigureAwait(false);
