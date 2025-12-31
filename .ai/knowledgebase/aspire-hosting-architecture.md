# Aspire Hosting Architecture

## Overview

The B2Connect platform leverages Microsoft's .NET Aspire framework for comprehensive service orchestration, providing a unified development and deployment experience for microservices, infrastructure, and frontend applications. This document details the Aspire hosting patterns, dashboard usage, and development workflows implemented in the project.

## Core Architecture Principles

### Service Orchestration
- **Distributed Application Model**: All services defined in a single `Program.cs` file
- **Service Discovery**: Automatic service location using service names instead of fixed ports
- **Resource Management**: Centralized infrastructure provisioning and configuration
- **Development Dashboard**: Real-time monitoring and debugging interface

### Infrastructure Components

#### Database Layer
- **PostgreSQL**: Primary relational database with per-service databases
- **Connection Management**: Automatic connection string injection via `WithPostgresConnection()`
- **Database Isolation**: Separate databases for auth, tenant, catalog, CMS, etc.

#### Caching & Messaging
- **Redis**: Distributed caching with automatic connection configuration
- **RabbitMQ**: Asynchronous messaging with service-specific queues
- **Elasticsearch**: Full-text search and analytics capabilities

#### Observability
- **Jaeger Tracing**: Distributed tracing across all services
- **OpenTelemetry**: Comprehensive telemetry collection
- **Health Checks**: Built-in service health monitoring

## Service Architecture Patterns

### Microservice Registration

```csharp
// Domain Service Pattern
var catalogService = builder
    .AddProject("catalog-service", "../backend/Domain/Catalog/B2Connect.Catalog.API.csproj")
    .WithPostgresConnection(catalogDb)
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithElasticsearchConnection(elasticsearch, "catalog")
    .WithJaegerTracing()
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();
```

#### Service Extensions
- **Cross-Cutting Concerns**: Security, logging, monitoring applied consistently
- **Infrastructure Binding**: Automatic resource connection injection
- **Configuration Management**: Environment-specific settings via `appsettings.json`

### Gateway Pattern

```csharp
// API Gateway Pattern
var storeGateway = builder
    .AddProject("store-gateway", "../backend/Gateway/Store/API/B2Connect.Store.csproj")
    .WithHttpEndpoint(port: 8000, name: "store-http")
    .WithReference(authService)
    .WithReference(catalogService)
    .WithB2ConnectCors("http://localhost:5173", "https://localhost:5173")
    .WithSecurityDefaults(jwtSecret);
```

#### Gateway Responsibilities
- **API Composition**: Aggregating multiple service endpoints
- **Cross-Cutting Concerns**: Authentication, CORS, rate limiting
- **Frontend Integration**: Direct connection points for client applications

## Frontend Integration

### Vite Application Hosting

```csharp
// Frontend Hosting Pattern
var frontendStore = builder
    .AddViteApp("frontend-store", "../Frontend/Store")
    .WithEndpoint("http", endpoint => endpoint.Port = 5173)
    .WithExternalHttpEndpoints()
    .WithNpm(installArgs: ["--force"])
    .WithEnvironment("VITE_API_GATEWAY_URL", "http://localhost:8000");
```

#### Development Features
- **Hot Module Replacement**: Real-time code updates during development
- **Automatic Port Assignment**: Dynamic port allocation with fixed external endpoints
- **NPM Integration**: Automatic dependency installation and script execution
- **Environment Injection**: API gateway URLs and configuration via environment variables

## Aspire Dashboard Usage

### Development Monitoring
- **Service Status**: Real-time health and status of all services
- **Log Aggregation**: Centralized logging from all components
- **Resource Usage**: CPU, memory, and network monitoring
- **Service Discovery**: Visual representation of service dependencies

### Debugging Capabilities
- **Distributed Tracing**: End-to-end request tracing across services
- **Log Correlation**: Linking logs across service boundaries
- **Error Tracking**: Exception aggregation and analysis
- **Performance Metrics**: Response times and throughput monitoring

## Development Workflows

### Local Development Setup

1. **Single Command Start**: `dotnet run --project AppHost/B2Connect.AppHost.csproj`
2. **Service Discovery**: Automatic service location via Aspire naming
3. **Hot Reload**: Code changes automatically reflected in running services
4. **Dashboard Access**: http://localhost:15500 for monitoring

### Configuration Management

#### Environment-Specific Settings
- **Development**: In-memory databases, relaxed security
- **Production**: External databases, full security configuration
- **Configuration Sources**: `appsettings.json`, environment variables, Key Vault

#### Secret Management
- **Development**: Inline secrets with fallback defaults
- **Production**: Azure Key Vault integration
- **JWT Secrets**: Configurable via environment or configuration

## Deployment Patterns

### Container Orchestration
- **Docker Integration**: Services containerized for consistent deployment
- **Kubernetes Manifests**: Generated manifests for K8s deployment
- **Helm Charts**: Templated deployments with configuration management

### Scaling Strategies
- **Horizontal Scaling**: Multiple service instances behind load balancers
- **Resource Optimization**: Automatic scaling based on metrics
- **Service Mesh**: Istio integration for advanced traffic management

## Cross-Cutting Concerns

### Security Implementation
- **Authentication**: JWT-based auth with Passkeys support
- **Authorization**: Role-based access control across services
- **Encryption**: Data encryption at rest and in transit
- **Rate Limiting**: DDoS protection and resource management

### Observability Stack
- **Logging**: Structured logging with correlation IDs
- **Metrics**: Application and infrastructure metrics
- **Tracing**: Distributed tracing with Jaeger
- **Health Checks**: Comprehensive health endpoint implementation

## Extension Patterns

### Custom Aspire Extensions

```csharp
// Custom Extension Usage
.WithB2ConnectPostgres()  // Custom PostgreSQL setup
.WithB2ConnectRedis()     // Custom Redis configuration
.WithB2ConnectCors()      // CORS configuration
.WithSecurityDefaults()   // Security baseline
```

#### Extension Benefits
- **Consistency**: Standardized configuration across services
- **Maintainability**: Centralized infrastructure logic
- **Reusability**: Common patterns extracted into reusable extensions

## Performance Optimization

### Resource Management
- **Connection Pooling**: Efficient database connection management
- **Caching Strategies**: Redis integration for performance optimization
- **Async Patterns**: Non-blocking I/O operations throughout

### Monitoring & Alerting
- **Health Checks**: Automatic service health validation
- **Metrics Collection**: Performance and reliability metrics
- **Alert Configuration**: Automated alerting for service degradation

## Troubleshooting Guide

### Common Issues
- **Port Conflicts**: Use dynamic ports with service discovery
- **Service Dependencies**: Ensure proper reference ordering
- **Configuration Errors**: Validate environment-specific settings

### Debugging Tools
- **Dashboard Logs**: Real-time log streaming
- **Distributed Tracing**: Request flow visualization
- **Health Endpoints**: Individual service health validation

## Migration & Evolution

### Version Compatibility
- **Framework Updates**: Regular Aspire version updates
- **Breaking Changes**: Migration guides for major updates
- **Deprecation Handling**: Gradual migration from deprecated features

### Future Enhancements
- **Service Mesh Integration**: Advanced traffic management
- **Multi-Cluster Support**: Cross-region deployment capabilities
- **AI/ML Integration**: Machine learning service orchestration

---

*This document is maintained by @Architect and @DevOps. Last updated: 2025-12-31*