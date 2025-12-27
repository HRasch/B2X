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

// Auth Service (Identity) - with Passkeys & JWT
var authService = builder
    .AddProject("auth-service", "../Domain/Identity/B2Connect.Identity.API.csproj")
    .WithHttpEndpoint(port: 7002, targetPort: 7002, name: "auth-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7002")
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
    .AddProject("tenant-service", "../Domain/Tenancy/B2Connect.Tenancy.API.csproj")
    .WithHttpEndpoint(port: 7003, targetPort: 7003, name: "tenant-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7003")
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
    .AddProject("localization-service", "../Domain/Localization/B2Connect.Localization.API.csproj")
    .WithHttpEndpoint(port: 7004, targetPort: 7004, name: "localization-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7004")
    .WithPostgresConnection(localizationDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Catalog Service (with Elasticsearch for Product Search)
var catalogService = builder
    .AddProject("catalog-service", "../Domain/Catalog/B2Connect.Catalog.API.csproj")
    .WithHttpEndpoint(port: 7005, targetPort: 7005, name: "catalog-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7005")
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
    .AddProject("theming-service", "../Domain/Theming/B2Connect.Theming.API.csproj")
    .WithHttpEndpoint(port: 7008, targetPort: 7008, name: "theming-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7008")
    .WithPostgresConnection(layoutDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// ===== API GATEWAYS =====

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject("store-gateway", "../Gateway/Store/API/B2Connect.Store.csproj")
    .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "store-gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:8000")
    .WithReference(authService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret);

// Admin API Gateway 
var adminGateway = builder
    .AddProject("admin-gateway", "../Gateway/Admin/B2Connect.Admin.csproj")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "admin-gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:8080")
    .WithReference(authService)
    .WithReference(tenantService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2ConnectCors("http://localhost:5174", "https://localhost:5174")
    .WithSecurityDefaults(jwtSecret);

// Frontend Store (Vite dev server)
var frontendStore = builder
    .AddNpmApp("frontend-store", "../../Frontend/Store", "dev")
    .WithHttpEndpoint(port: 5173, targetPort: 5173, name: "frontend-store", isProxied: false)
    .WithEnvironment("PORT", "5173")
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8085");

// Frontend Admin
var frontendAdmin = builder
    .AddNpmApp("frontend-admin", "../../Frontend/Admin", "dev")
    .WithHttpEndpoint(port: 5174, targetPort: 5174, name: "frontend-admin", isProxied: false)
    .WithEnvironment("PORT", "5174")
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8086");

builder.Build().Run();