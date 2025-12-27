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
    .AddProject<Projects.B2Connect_Identity_API>("auth-service")
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
    .AddProject<Projects.B2Connect_Tenancy_API>("tenant-service")
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
    .AddProject<Projects.B2Connect_Localization_API>("localization-service")
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
    .AddProject<Projects.B2Connect_Catalog_API>("catalog-service")
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
    .AddProject<Projects.B2Connect_Theming_API>("theming-service")
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
    .AddProject<Projects.B2Connect_Store>("store-gateway")
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:8000")
    .WithReference(authService)
    .WithReference(catalogService)
    .WithReference(localizationService)
    .WithReference(themingService)
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret);

// Admin API Gateway (for frontend-admin, protected CRUD endpoints)
var adminGateway = builder
    .AddProject<Projects.B2Connect_Admin>("admin-gateway")
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
    .AddNpmApp("frontend-store", "../../frontend-store", "dev")
    .WithEnvironment("PORT", "5173")
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000");

// Frontend Admin (Vite dev server)
var frontendAdmin = builder
    .AddNpmApp("frontend-admin", "../../frontend-admin", "dev")
    .WithEnvironment("PORT", "5174")
    .WithEnvironment("BROWSER", "none")
    // VITE_ADMIN_API_URL should NOT be set - defaults to "/api" which gets proxied by Vite
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8080");

builder.Build().Run();