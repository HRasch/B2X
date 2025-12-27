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

// ===== API GATEWAYS =====

// Store API Gateway (for frontend-store, public read-only endpoints)
var storeGateway = builder
    .AddProject<Projects.B2Connect_Store>("store-gateway")
    .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "store-gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:8000")
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret);

// Admin API Gateway (for frontend-admin, protected CRUD endpoints)
var adminGateway = builder
    .AddProject<Projects.B2Connect_Admin>("admin-gateway")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "admin-gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:8080")
    .WithB2ConnectCors("http://localhost:5174", "https://localhost:5174")
    .WithSecurityDefaults(jwtSecret);

// ===== MICROSERVICES =====

// Auth Service (Identity) - with Passkeys & JWT
var authService = builder
    .AddProject<Projects.B2Connect_Identity_API>("auth-service")
    .WithHttpEndpoint(port: 7002, targetPort: 7002, name: "auth-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7002")
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
    .AddProject<Projects.B2Connect_Tenancy_API>("tenant-service")
    .WithHttpEndpoint(port: 7003, targetPort: 7003, name: "tenant-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7003")
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
    .AddProject<Projects.B2Connect_Localization_API>("localization-service")
    .WithHttpEndpoint(port: 7004, targetPort: 7004, name: "localization-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7004")
    .WithPostgresConnection(localizationDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Catalog Service (with Elasticsearch for Product Search)
var catalogService = builder
    .AddProject<Projects.B2Connect_Catalog_API>("catalog-service")
    .WithHttpEndpoint(port: 7005, targetPort: 7005, name: "catalog-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7005")
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
    .AddProject<Projects.B2Connect_Theming_API>("theming-service")
    .WithHttpEndpoint(port: 7008, targetPort: 7008, name: "theming-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7008")
    .WithPostgresConnection(layoutDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();

// Frontend Store (Vite dev server)
var frontendStore = builder
    .AddNpmApp("frontend-store", "../../frontend-store", "dev")
    .WithHttpEndpoint(port: 5173, targetPort: 5173, name: "vite-store", isProxied: false)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000")
    .WithEnvironment("VITE_PORT", "5173");

// Frontend Admin (Vite dev server)
var frontendAdmin = builder
    .AddNpmApp("frontend-admin", "../../frontend-admin", "dev")
    .WithHttpEndpoint(port: 5174, targetPort: 5174, name: "vite-admin", isProxied: false)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_ADMIN_API_URL", "/api")
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8080")
    .WithEnvironment("VITE_PORT", "5174");

builder.Build().Run();