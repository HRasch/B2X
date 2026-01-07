using B2Connect.Aspire.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

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

IResourceBuilder<RedisResource>? redis = null;
IResourceBuilder<ElasticsearchResource>? elasticsearch = null;
IResourceBuilder<RabbitMQServerResource>? rabbitmq = null;
IResourceBuilder<PostgresServerResource>? postgres = null;

// IResourceBuilder<AzureCdnResource>? cdn = null;

if (!string.Equals(databaseProvider.ToLower(), "inmemory", StringComparison.Ordinal))
{
    // PostgreSQL Database
    postgres = builder.AddB2ConnectPostgres(
        name: "postgres",
        port: 5432,
        username: "postgres");

    // Create all databases
    authDb = postgres.AddB2ConnectDatabase("auth");
    tenantDb = postgres.AddB2ConnectDatabase("tenant");
    localizationDb = postgres.AddB2ConnectDatabase("localization");
    catalogDb = postgres.AddB2ConnectDatabase("catalog");
    layoutDb = postgres.AddB2ConnectDatabase("layout");
    adminDb = postgres.AddB2ConnectDatabase("admin");
    storeDb = postgres.AddB2ConnectDatabase("store");
    monitoringDb = postgres.AddB2ConnectDatabase("monitoring");

    // Redis Cache
    redis = builder.AddB2ConnectRedis(
        name: "redis",
        port: 6379);

    // Elasticsearch (Full-Text Search & Analytics)
    elasticsearch = builder.AddB2ConnectElasticsearch(
        name: "elasticsearch",
        port: 9200);

    // RabbitMQ (Asynchronous Message Queue)
    rabbitmq = builder.AddB2ConnectRabbitMQ(
        name: "rabbitmq",
        port: 5672);

    // Azure CDN for translation assets and API caching
    // cdn = builder.AddAzureCdnFrontDoor("cdn")
    //     .WithOrigin(storeGateway, "store-api")
    //     .WithOrigin(frontendStore, "store-frontend");
}
else
{
    // When running the full Aspire AppHost with in-memory DB for demos,
    // also provide an Elasticsearch resource to enable realistic search/indexing
    // for local development/demo scenarios.
    elasticsearch = builder.AddB2ConnectElasticsearch(
        name: "elasticsearch",
        port: 9200);

    // RabbitMQ is also useful for demo indexing flows; add if needed
    rabbitmq = builder.AddB2ConnectRabbitMQ(
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
    .AddProject("auth-service", "../backend/Domain/Identity/B2Connect.Identity.API.csproj")
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
    .AddProject("tenant-service", "../backend/Domain/Tenancy/B2Connect.Tenancy.API.csproj")
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
    .AddProject("localization-service", "../backend/Domain/Localization/B2Connect.Localization.API.csproj")
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
    .AddProject("catalog-service", "../backend/Domain/Catalog/B2Connect.Catalog.API.csproj")
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

// Theming Service
var themingService = builder
    .AddProject("theming-service", "../backend/Domain/Theming/B2Connect.Theming.API.csproj")
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

// Monitoring Service (Phase 2: Connected services monitoring, error logging)
var monitoringService = builder
    .AddProject("monitoring-service", "../backend/BoundedContexts/Monitoring/API/B2Connect.Monitoring.csproj")
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
    .AddProject("mcp-server", "../backend/BoundedContexts/Admin/MCP/B2Connect.Admin.MCP/B2Connect.Admin.MCP.csproj")
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

// ===== API GATEWAYS =====
// Gateways keep fixed ports because frontends connect directly to them.
// Internal service communication uses Aspire Service Discovery.
// CRITICAL: Gateways wait for all services they aggregate to prevent hanging.

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject("store-gateway", "../backend/Gateway/Store/API/B2Connect.Store.csproj")
    .WithHttpEndpoint(port: 8000, name: "store-http")  // Fixed port for frontend
    .WithReference(authService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret)
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 120)
    .WithResilienceConfiguration();

// Store Gateway waits for all its referenced services
storeGateway.WaitFor(authService);
storeGateway.WaitFor(catalogService);
storeGateway.WaitFor(localizationService);
storeGateway.WaitFor(themingService);

// Admin API Gateway 
var adminGateway = builder
    .AddProject("admin-gateway", "../backend/Gateway/Admin/B2Connect.Admin.csproj")
    .WithHttpEndpoint(port: 8080, name: "admin-http")  // Fixed port for frontend
    .WithReference(authService)
    .WithReference(tenantService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithReference(monitoringService)
    .WithReference(mcpServer)
    .WithB2ConnectCors("http://localhost:5174", "https://localhost:5174")
    .WithSecurityDefaults(jwtSecret)
    .WithHealthCheckEndpoint()
    .WithStartupConfiguration(startupTimeoutSeconds: 120)
    .WithResilienceConfiguration();

// Admin Gateway waits for all its referenced services
adminGateway.WaitFor(authService);
adminGateway.WaitFor(tenantService);
adminGateway.WaitFor(catalogService);
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

builder.Build().Run();
