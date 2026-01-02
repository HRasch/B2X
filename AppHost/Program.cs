using B2Connect.Aspire.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// JWT Secret Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? builder.Configuration["JWT_SECRET"]
    ?? "dev-jwt-secret-min-32-chars-required!!!";

// ===== INFRASTRUCTURE =====

// PostgreSQL Database
var postgres = builder.AddB2ConnectPostgres(
    name: "postgres",
    port: 5432,
    username: "postgres");


// Create all databases
var authDb = postgres.AddB2ConnectDatabase("auth");
var tenantDb = postgres.AddB2ConnectDatabase("tenant");
var localizationDb = postgres.AddB2ConnectDatabase("localization");
var catalogDb = postgres.AddB2ConnectDatabase("catalog");
var layoutDb = postgres.AddB2ConnectDatabase("layout");
var adminDb = postgres.AddB2ConnectDatabase("admin");
var storeDb = postgres.AddB2ConnectDatabase("store");
var monitoringDb = postgres.AddB2ConnectDatabase("monitoring");

// Redis Cache
var redis = builder.AddB2ConnectRedis(
    name: "redis",
    port: 6379);

// Elasticsearch (Full-Text Search & Analytics)
var elasticsearch = builder.AddB2ConnectElasticsearch(
    name: "elasticsearch",
    port: 9200);

// RabbitMQ (Asynchronous Message Queue)
var rabbitmq = builder.AddB2ConnectRabbitMQ(
    name: "rabbitmq",
    port: 5672);

// Azure Key Vault (Secret Store)

// ===== MICROSERVICES =====
// Using dynamic ports with Aspire Service Discovery (no more port conflicts!)
// Services are discovered via service name, not fixed ports.
// Aspire automatically assigns dynamic ports - no WithHttpEndpoint needed for internal services.

// Auth Service (Identity) - with Passkeys & JWT
var authService = builder
    .AddProject("auth-service", "../backend/Domain/Identity/B2Connect.Identity.API.csproj")
    .WithPostgresConnection(authDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithPasskeysAuth()
    .WithPasskeysJwt(builder.Configuration["Jwt:Secret"] ?? "dev-jwt-secret-min-32-chars-required!!!")
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Tenant Service
var tenantService = builder
    .AddProject("tenant-service", "../backend/Domain/Tenancy/B2Connect.Tenancy.API.csproj")
    .WithPostgresConnection(tenantDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithEncryption()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Localization Service
var localizationService = builder
    .AddProject("localization-service", "../backend/Domain/Localization/B2Connect.Localization.API.csproj")
    .WithPostgresConnection(localizationDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Catalog Service (with Elasticsearch for Product Search)
var catalogService = builder
    .AddProject("catalog-service", "../backend/Domain/Catalog/B2Connect.Catalog.API.csproj")
    .WithPostgresConnection(catalogDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithElasticsearchConnection(elasticsearch, "catalog")
    .WithElasticsearchIndexing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Theming Service
var themingService = builder
    .AddProject("theming-service", "../backend/Domain/Theming/B2Connect.Theming.API.csproj")
    .WithPostgresConnection(layoutDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Monitoring Service (Phase 2: Connected services monitoring, error logging)
var monitoringService = builder
    .AddProject("monitoring-service", "../backend/BoundedContexts/Monitoring/API/B2Connect.Monitoring.csproj")
    .WithPostgresConnection(monitoringDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// ===== API GATEWAYS =====
// Gateways keep fixed ports because frontends connect directly to them.
// Internal service communication uses Aspire Service Discovery.

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject("store-gateway", "../backend/Gateway/Store/API/B2Connect.Store.csproj")
    .WithHttpEndpoint(port: 8000, name: "store-http")  // Fixed port for frontend
    .WithReference(authService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret);

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
    .WithB2ConnectCors("http://localhost:5174", "https://localhost:5174")
    .WithSecurityDefaults(jwtSecret);

// ===== FRONTENDS (Vite Vue.js Applications) =====
// Using native Aspire.Hosting.JavaScript integration (AddViteApp)
// Documentation: https://aspire.dev/integrations/frameworks/javascript/
// Fixed ports configured via WithEndpoint (see: https://github.com/dotnet/aspire/issues/12942)

// Frontend Store (Vue 3 + Vite) - Fixed port 5173 for predictable URLs
var frontendStore = builder
    .AddViteApp("frontend-store", "../Frontend/Store")
    .WithEndpoint("http", endpoint => endpoint.Port = 5173)  // Workaround: modify existing endpoint
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])  // Force install to handle platform-specific packages
    .WithEnvironment("PORT", "5173")  // Vite reads PORT env var
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000")
    .WithEnvironment("NODE_ENV", "development");

// Frontend Admin (Vue 3 + Vite) - Fixed port 5174 for predictable URLs
var frontendAdmin = builder
    .AddViteApp("frontend-admin", "../Frontend/Admin")
    .WithEndpoint("http", endpoint => endpoint.Port = 5174)  // Workaround: modify existing endpoint
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])  // Force install to handle platform-specific packages
    .WithEnvironment("PORT", "5174")  // Vite reads PORT env var
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8080")
    .WithEnvironment("NODE_ENV", "development");

// Issue #50: Vite build errors are automatically captured by Aspire's built-in logging
// Logs appear in Aspire Dashboard under each frontend resource
// No custom code needed - AddViteApp() handles stdout/stderr forwarding

builder.Build().Run();