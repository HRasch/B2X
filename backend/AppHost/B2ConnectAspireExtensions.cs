using Aspire.Hosting;
using Microsoft.AspNetCore.Identity;

namespace B2Connect.Aspire.Extensions;

/// <summary>
/// Aspire Extensions für Secret Store, PostgreSQL und Passkeys Integration
/// </summary>
public static class B2ConnectAspireExtensions
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
    public static IResourceBuilder<PostgresServerResource> AddB2ConnectPostgres(
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
    /// Adds a database to PostgreSQL with B2Connect naming convention
    /// </summary>
    public static IResourceBuilder<PostgresDatabaseResource> AddB2ConnectDatabase(
        this IResourceBuilder<PostgresServerResource> postgres,
        string name)
    {
        // Resource name: just the name (e.g., "auth")
        // Database name: b2connect_{name} (e.g., "b2connect_auth")
        return postgres.AddDatabase(name, $"b2connect_{name}");
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
    public static IResourceBuilder<RedisResource> AddB2ConnectRedis(
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
    public static IResourceBuilder<ElasticsearchResource> AddB2ConnectElasticsearch(
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
        indexPrefix ??= "b2connect";

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
    public static IResourceBuilder<RabbitMQServerResource> AddB2ConnectRabbitMQ(
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
            .WithEnvironment("Auth:Passkeys:RP:Name", "B2Connect")
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
            .WithEnvironment("Jwt:Issuer", "B2Connect")
            .WithEnvironment("Jwt:Audience", "B2Connect")
            .WithEnvironment("Jwt:TokenExpirationMinutes", "60")
            .WithEnvironment("Jwt:RefreshTokenExpirationDays", "7")
            .WithEnvironment("Jwt:UsePasskeys", "true");
    }

    /// <summary>
    /// Konfiguriert CORS für lokale Entwicklung oder Production
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithB2ConnectCors(
        this IResourceBuilder<ProjectResource> builder,
        params string[] origins)
    {
        var corsOrigins = origins.Length > 0
            ? string.Join(";", origins)
            : "http://localhost:5173;http://localhost:5174;https://localhost:5173;https://localhost:5174";

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
            .WithEnvironment("ASPNETCORE_HTTPS_PORT", httpsPort.ToString())
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
            .WithB2ConnectCors()
            .WithAuditLogging()
            .WithEncryption()
            .WithRateLimiting()
            .WithOpenTelemetry();
    }
}

/// <summary>
/// Resource Extensions für PostgreSQL
/// </summary>
public static class PostgresServerResourceExtensions
{
    /// <summary>
}
