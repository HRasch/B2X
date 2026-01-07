using Aspire.Hosting;
using Microsoft.AspNetCore.Identity;

namespace B2X.Aspire.Extensions;

/// <summary>
/// Aspire Extensions für Secret Store, PostgreSQL und Passkeys Integration
/// </summary>
public static class B2XAspireExtensions
{
    /// <summary>
    /// Registriert Azure Key Vault für Secret Store
    /// </summary>
    public static IDistributedApplicationBuilder AddAzureKeyVault(
        this IDistributedApplicationBuilder builder,
        string name = "keyvault")
    {
        // Azure Key Vault in Aspire 13.0 is configured via connection strings
        // Services should use Azure.Identity and DefaultAzureCredential
        // Configuration is handled via environment variables in service configuration
        return builder;
    }

    /// <summary>
    /// Registriert PostgreSQL server (databases are added separately)
    /// </summary>
    public static IResourceBuilder<PostgresServerResource> AddB2XPostgres(
        this IDistributedApplicationBuilder builder,
        string name = "postgres",
        int port = 5432,
        string username = "postgres")
    {
        var password = builder.Configuration["Database:Password"]
            ?? builder.Configuration["POSTGRES_PASSWORD"]
            ?? "postgres";

        var passwordParam = builder.AddParameter("postgres-password", password, secret: true);
        var postgres = builder.AddPostgres(name, password: passwordParam)
            .WithImageTag("16.11")
            .WithPgAdmin();

        return postgres;
    }

    /// <summary>
    /// Adds a database to PostgreSQL with B2X naming convention
    /// </summary>
    public static IResourceBuilder<PostgresDatabaseResource> AddB2XDatabase(
        this IResourceBuilder<PostgresServerResource> postgres,
        string name)
    {
        // Resource name: just the name (e.g., "auth")
        // Database name: B2X_{name} (e.g., "B2X_auth")
        return postgres.AddDatabase(name, $"B2X_{name}");
    }

    /// <summary>
    /// Konfiguriert einen Service mit PostgreSQL Connection
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithPostgresConnection(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<PostgresDatabaseResource> database)
    {
        return builder
            .WithEnvironment("Database:Provider", "postgres")
            .WithReference(database);
    }

    /// <summary>
    /// Registriert Redis für Cache & Session Storage
    /// </summary>
    public static IResourceBuilder<RedisResource> AddB2XRedis(
        this IDistributedApplicationBuilder builder,
        string name = "redis",
        int port = 6379)
    {
        var redisPassword = builder.Configuration["Redis:Password"]
            ?? builder.Configuration["REDIS_PASSWORD"]
            ?? null;

        var redis = builder.AddRedis(name, port: port)
            .WithImageTag("8.4.0");

        if (!string.IsNullOrEmpty(redisPassword))
        {
            redis.WithEnvironment("REDIS_PASSWORD", redisPassword);
        }

        return redis;
    }

    /// <summary>
    /// Registriert Elasticsearch für Full-Text Search & Analytics
    /// </summary>
    public static IResourceBuilder<ElasticsearchResource> AddB2XElasticsearch(
        this IDistributedApplicationBuilder builder,
        string name = "elasticsearch",
        int port = 9200)
    {
        var elasticPassword = builder.Configuration["Elasticsearch:Password"]
            ?? builder.Configuration["ELASTIC_PASSWORD"]
            ?? "elastic";

        var elasticPasswordParam = builder.AddParameter("elastic-password", elasticPassword, secret: true);
        var elasticsearch = builder.AddElasticsearch(name, password: elasticPasswordParam)
            .WithImageTag("9.2.3");

        return elasticsearch
            .WithEnvironment("ELASTIC_HEAP_SIZE", "512m")
            .WithEnvironment("xpack.security.enabled", "true")
            .WithEnvironment("discovery.type", "single-node");
    }

    /// <summary>
    /// Konfiguriert einen Service mit Redis Connection
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithRedisConnection(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<RedisResource> redis)
    {
        return builder
            .WithReference(redis)
            .WithEnvironment("Redis:Enabled", "true")
            .WithEnvironment("Caching:Provider", "redis");
    }

    /// <summary>
    /// Konfiguriert einen Service mit Elasticsearch Connection
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithElasticsearchConnection(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<ElasticsearchResource> elasticsearch,
        string? indexPrefix = null)
    {
        indexPrefix ??= "B2X";

        return builder
            .WithReference(elasticsearch)
            .WithEnvironment("Elasticsearch:Enabled", "true")
            .WithEnvironment("Elasticsearch:Provider", "elasticsearch")
            .WithEnvironment("Elasticsearch:IndexPrefix", indexPrefix)
            .WithEnvironment("Search:Engine", "elasticsearch");
    }

    /// <summary>
    /// Konfiguriert Elasticsearch Indexing für Search-Services
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithElasticsearchIndexing(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("Elasticsearch:AutoIndexing", "true")
            .WithEnvironment("Elasticsearch:BatchSize", "100")
            .WithEnvironment("Elasticsearch:RefreshInterval", "1s")
            .WithEnvironment("Elasticsearch:NumberOfShards", "1")
            .WithEnvironment("Elasticsearch:NumberOfReplicas", "0");
    }

    /// <summary>
    /// Registriert RabbitMQ für asynchrone Message Queue
    /// </summary>
    public static IResourceBuilder<RabbitMQServerResource> AddB2XRabbitMQ(
        this IDistributedApplicationBuilder builder,
        string name = "rabbitmq",
        int port = 5672)
    {
        var rabbitPassword = builder.Configuration["RabbitMQ:Password"]
            ?? builder.Configuration["RABBITMQ_DEFAULT_PASS"]
            ?? "guest";

        var rabbitPasswordParam = builder.AddParameter("rabbit-password", rabbitPassword, secret: true);
        var rabbitmq = builder.AddRabbitMQ(name, password: rabbitPasswordParam)
            .WithImageTag("4.2.2-management")
            .WithManagementPlugin();

        return rabbitmq
            .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", rabbitPassword)
            .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "/");
    }

    /// <summary>
    /// Konfiguriert einen Service mit RabbitMQ Connection
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithRabbitMQConnection(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<RabbitMQServerResource> rabbitmq)
    {
        return builder
            .WithReference(rabbitmq)
            .WithEnvironment("RabbitMQ:Enabled", "true")
            .WithEnvironment("MessageQueue:Provider", "rabbitmq")
            .WithEnvironment("RabbitMQ:PrefetchCount", "10")
            .WithEnvironment("RabbitMQ:AutomaticRecovery", "true");
    }

    /// <summary>
    /// Konfiguriert einen Service mit OpenTelemetry Jaeger Tracing (ohne Aspire Jaeger Host)
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithJaegerTracing(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENABLED", "true")
            .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
            .WithEnvironment("OTEL_TRACES_EXPORTER", "otlp")
            .WithEnvironment("OTEL_METRICS_EXPORTER", "otlp")
            .WithEnvironment("OTEL_TRACES_SAMPLER", "parentbased_traceidratio")
            .WithEnvironment("OTEL_TRACES_SAMPLER_ARG", "0.1");
    }


    /// <summary>
    /// Aktiviert Passkeys (FIDO2/WebAuthn) Authentifizierung
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithPasskeysAuth(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("Auth:Passkeys:Enabled", "true")
            .WithEnvironment("Auth:Passkeys:RP:Name", "B2X")
            .WithEnvironment("Auth:Passkeys:RP:Origin", "https://localhost:5174")
            .WithEnvironment("Auth:Passkeys:AttestationConveyance", "none")
            .WithEnvironment("Auth:Passkeys:UserVerification", "preferred")
            .WithEnvironment("Auth:Passkeys:AuthenticatorSelection:Resident", "true")
            .WithEnvironment("Auth:Passkeys:Challenge:Timeout", "60000");
    }

    /// <summary>
    /// Konfiguriert JWT mit Passkeys Support
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithPasskeysJwt(
        this IResourceBuilder<ProjectResource> builder,
        string jwtSecret)
    {
        return builder
            .WithEnvironment("Jwt:Secret", jwtSecret)
            .WithEnvironment("Jwt:Issuer", "B2X")
            .WithEnvironment("Jwt:Audience", "B2X")
            .WithEnvironment("Jwt:TokenExpirationMinutes", "60")
            .WithEnvironment("Jwt:RefreshTokenExpirationDays", "7")
            .WithEnvironment("Jwt:UsePasskeys", "true");
    }

    /// <summary>
    /// Konfiguriert CORS für lokale Entwicklung oder Production
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithB2XCors(
        this IResourceBuilder<ProjectResource> builder,
        params string[] origins)
    {
        return builder
            .WithEnvironment("Cors:AllowedOrigins:0", "http://localhost:5173")
            .WithEnvironment("Cors:AllowedOrigins:1", "http://localhost:5174")
            .WithEnvironment("Cors:AllowedOrigins:2", "https://localhost:5173")
            .WithEnvironment("Cors:AllowedOrigins:3", "https://localhost:5174");
    }

    /// <summary>
    /// Konfiguriert Audit Logging mit PostgreSQL
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithAuditLogging(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("Audit:Enabled", "true")
            .WithEnvironment("Audit:LogToDatenbank", "true")
            .WithEnvironment("Audit:LogToFile", "false")
            .WithEnvironment("Audit:IncludeSensitiveData", "false");
    }

    /// <summary>
    /// Konfiguriert Encryption at Rest für sensitive Daten
    /// Note: Encryption key must be provided via environment variable ENCRYPTION_KEY
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithEncryption(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("Encryption:Enabled", "true")
            .WithEnvironment("Encryption:Algorithm", "AES-256")
            .WithEnvironment("Encryption:EncryptPII", "true")
            .WithEnvironment("Encryption:EncryptFinancial", "true");
    }

    /// <summary>
    /// Konfiguriert TLS/HTTPS für Service
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithHttps(
        this IResourceBuilder<ProjectResource> builder,
        int httpsPort = 443)
    {
        return builder
            .WithEnvironment("ASPNETCORE_HTTPS_PORT", httpsPort.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .WithEnvironment("ASPNETCORE_Kestrel:Certificates:Default:Path", "/app/certs/server.pfx")
            .WithEnvironment("ASPNETCORE_Kestrel:Certificates:Default:Password", "")
            .WithEnvironment("ASPNETCORE_Kestrel:Protocols", "Http1AndHttp2");
    }

    /// <summary>
    /// Konfiguriert Rate Limiting für Service
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithRateLimiting(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("RateLimiting:Enabled", "true")
            .WithEnvironment("RateLimiting:GeneralLimit", "100")
            .WithEnvironment("RateLimiting:AuthLimit", "5")
            .WithEnvironment("RateLimiting:RegisterLimit", "3")
            .WithEnvironment("RateLimiting:StrictLimit", "2");
    }

    /// <summary>
    /// Konfiguriert Open Telemetry für Distributed Tracing
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithOpenTelemetry(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENABLED", "true")
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
            .WithEnvironment("OTEL_SDK_DISABLED", "false")
            .WithEnvironment("OTEL_TRACES_EXPORTER", "otlp")
            .WithEnvironment("OTEL_METRICS_EXPORTER", "otlp");
    }

    /// <summary>
    /// Registriert all Security Best Practices
    /// Note: JWT secret must be provided via environment variable JWT_SECRET
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithSecurityDefaults(
        this IResourceBuilder<ProjectResource> builder,
        string jwtSecret)
    {
        return builder
            .WithPasskeysAuth()
            .WithPasskeysJwt(jwtSecret)
            .WithB2XCors()
            .WithAuditLogging()
            .WithEncryption()
            .WithRateLimiting()
            .WithOpenTelemetry();
    }

    // ===== CONDITIONAL CONNECTION METHODS =====
    // These methods only add connections when infrastructure resources are available
    // Used for in-memory database development mode

    /// <summary>
    /// Conditionally adds PostgreSQL connection if database is available
    /// </summary>
    public static IResourceBuilder<T> WithConditionalPostgresConnection<T>(
        this IResourceBuilder<T> resource,
        IResourceBuilder<PostgresDatabaseResource>? postgresDb,
        string databaseProvider) where T : IResourceWithEnvironment
    {
        if (!string.Equals(databaseProvider, "inmemory", StringComparison.OrdinalIgnoreCase) && postgresDb != null)
        {
            return resource.WithReference(postgresDb);
        }
        return resource;
    }

    /// <summary>
    /// Conditionally adds Redis connection if Redis is available
    /// </summary>
    public static IResourceBuilder<T> WithConditionalRedisConnection<T>(
        this IResourceBuilder<T> resource,
        IResourceBuilder<RedisResource>? redis,
        string databaseProvider) where T : IResourceWithEnvironment
    {
        if (!string.Equals(databaseProvider, "inmemory", StringComparison.OrdinalIgnoreCase) && redis != null)
        {
            return resource.WithReference(redis);
        }
        return resource;
    }

    /// <summary>
    /// Conditionally adds RabbitMQ connection if RabbitMQ is available
    /// </summary>
    public static IResourceBuilder<T> WithConditionalRabbitMQConnection<T>(
        this IResourceBuilder<T> resource,
        IResourceBuilder<RabbitMQServerResource>? rabbitmq,
        string databaseProvider) where T : IResourceWithEnvironment
    {
        if (!string.Equals(databaseProvider, "inmemory", StringComparison.OrdinalIgnoreCase) && rabbitmq != null)
        {
            return resource.WithReference(rabbitmq);
        }
        return resource;
    }

    /// <summary>
    /// Conditionally adds Elasticsearch connection if Elasticsearch is available
    /// </summary>
    public static IResourceBuilder<T> WithConditionalElasticsearchConnection<T>(
        this IResourceBuilder<T> resource,
        IResourceBuilder<ElasticsearchResource>? elasticsearch,
        string databaseProvider,
        string indexName) where T : IResourceWithEnvironment
    {
        if (!string.Equals(databaseProvider, "inmemory", StringComparison.OrdinalIgnoreCase) && elasticsearch != null)
        {
            return resource.WithReference(elasticsearch);
        }
        return resource;
    }

    /// <summary>
    /// Conditionally adds Elasticsearch indexing if Elasticsearch is available
    /// </summary>
    public static IResourceBuilder<T> WithConditionalElasticsearchIndexing<T>(
        this IResourceBuilder<T> resource,
        string databaseProvider) where T : IResourceWithEnvironment
    {
        if (!string.Equals(databaseProvider, "inmemory", StringComparison.OrdinalIgnoreCase))
        {
            // Add Elasticsearch indexing logic here if needed
            return resource;
        }
        return resource;
    }

    // ===== NATIVE ASPIRE JAVASCRIPT INTEGRATION =====
    // Die folgenden Extension-Methoden verwenden das offizielle Aspire.Hosting.JavaScript Package
    // Dokumentation: https://aspire.dev/integrations/frameworks/javascript/

    // HINWEIS: AddViteApp, AddJavaScriptApp, und AddNodeApp sind jetzt direkt 
    // von Aspire.Hosting.JavaScript verfügbar. Keine Custom Extensions nötig!
    //
    // Verwendung in AppHost/Program.cs:
    //
    // var frontendStore = builder.AddViteApp("frontend-store", "../Frontend/Store")
    //     .WithHttpEndpoint(port: 5173, env: "VITE_PORT")
    //     .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000");
    //
    // var frontendAdmin = builder.AddViteApp("frontend-admin", "../Frontend/Admin")
    //     .WithHttpEndpoint(port: 5174, env: "VITE_PORT");

    // ===== HEALTH CHECK & STARTUP COORDINATION =====
    // These extensions help manage service startup order and health checks

    /// <summary>
    /// Configures HTTP health check endpoint for readiness probes.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithHealthCheckEndpoint(
        this IResourceBuilder<ProjectResource> builder,
        string path = "/health")
    {
        return builder
            .WithEnvironment("HealthChecks:Enabled", "true")
            .WithEnvironment("HealthChecks:Path", path)
            .WithEnvironment("HealthChecks:ReadyPath", "/health/ready")
            .WithEnvironment("HealthChecks:LivePath", "/health/live");
    }

    /// <summary>
    /// Configures graceful startup with timeout handling.
    /// Services will wait for dependencies but have a maximum timeout to prevent hangs.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithStartupConfiguration(
        this IResourceBuilder<ProjectResource> builder,
        int startupTimeoutSeconds = 120,
        int healthCheckIntervalSeconds = 10)
    {
        return builder
            .WithEnvironment("Startup:TimeoutSeconds", startupTimeoutSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .WithEnvironment("Startup:HealthCheckIntervalSeconds", healthCheckIntervalSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .WithEnvironment("Startup:RetryCount", "12")
            .WithEnvironment("Startup:RetryDelayMs", "5000");
    }

    /// <summary>
    /// Configures resilience policies for service connections.
    /// This helps services handle temporary unavailability of dependencies gracefully.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithResilienceConfiguration(
        this IResourceBuilder<ProjectResource> builder)
    {
        return builder
            .WithEnvironment("Resilience:Enabled", "true")
            .WithEnvironment("Resilience:CircuitBreaker:FailureThreshold", "5")
            .WithEnvironment("Resilience:CircuitBreaker:SamplingDuration", "30")
            .WithEnvironment("Resilience:CircuitBreaker:MinimumThroughput", "10")
            .WithEnvironment("Resilience:CircuitBreaker:BreakDuration", "30")
            .WithEnvironment("Resilience:Retry:MaxRetries", "3")
            .WithEnvironment("Resilience:Retry:DelayMs", "1000")
            .WithEnvironment("Resilience:Timeout:TimeoutMs", "30000");
    }
}
