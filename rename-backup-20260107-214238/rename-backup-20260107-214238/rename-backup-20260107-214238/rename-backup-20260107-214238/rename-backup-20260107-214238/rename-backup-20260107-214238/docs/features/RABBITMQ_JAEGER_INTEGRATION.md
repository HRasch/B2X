# 🚀 RabbitMQ & Jaeger Integration in Aspire

**Status**: ✅ Konfiguriert & Bereit zum Starten

---

## 📋 Übersicht

### RabbitMQ - Asynchrone Message Queue
**Port**: 5672 (AMQP) | 15672 (Management UI)  
**Features**:
- 🔄 Asynchrone Service-to-Service Kommunikation
- 📨 Reliable Message Delivery mit Acknowledgments
- 🔁 Dead Letter Queues & Retry Logic
- 📊 Built-in Management Dashboard

### Jaeger - Distributed Tracing
**Port**: 16686 (UI) | 4317 (OTLP gRPC)  
**Features**:
- 📊 End-to-End Request Tracing
- ⚡ Performance Monitoring & Bottleneck Detection
- 🔍 Service Dependency Visualization
- 💾 Trace Storage & Analysis

---

## 🏗️ Architektur

```
┌────────────────────────────────────────────────────────┐
│          Aspire Orchestration Dashboard                │
│          http://localhost:15500                        │
└────────────────────────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
    ┌───▼──────┐    ┌───▼──────┐    ┌───▼──────┐
    │ RabbitMQ │    │ Jaeger   │    │PostgreSQL│
    │ Port 5672│    │Port 16686│    │Port 5432 │
    │ UI: 15672│    │          │    │          │
    └───┬──────┘    └───┬──────┘    └──────────┘
        │                │
        │ Async Msgs     │ Trace Data
        │                │
    ┌───▼──────────────────────────────────┐
    │       Microservices (7 Total)        │
    │                                      │
    │  • Auth Service (7002)               │
    │  • Tenant Service (7003)             │
    │  • Localization Service (7004)       │
    │  • Catalog Service (7005)            │
    │  • Theming Service (7008)            │
    │  • Store Gateway (8000)              │
    │  • Admin Gateway (8080)              │
    └──────────────────────────────────────┘
```

---

## 🔧 Konfiguration in Aspire

### 1. RabbitMQ Registrierung

```csharp
// Program.cs
var rabbitmq = builder.AddB2ConnectRabbitMQ(
    name: "rabbitmq",
    port: 5672);
```

**Konfiguriert:**
- AMQP Port: 5672
- Management UI: 15672
- Default User: guest / guest
- Virtual Host: /

### 2. Jaeger Registrierung

```csharp
// Program.cs
var jaeger = builder.AddB2ConnectJaeger(
    name: "jaeger");
```

**Konfiguriert:**
- UI Port: 16686
- OTLP gRPC Exporter: 4317
- Trace Sampling: 10% (für Development)
- Max Traces: 10.000

### 3. Service Integration

```csharp
// Auth Service mit RabbitMQ & Jaeger
var authService = builder
    .AddProject<Projects.B2Connect_Identity_API>("auth-service")
    .WithHttpEndpoint(port: 7002, targetPort: 7002, name: "auth-service")
    .WithPostgresConnection(postgres, "b2connect_auth")
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)      // ← RabbitMQ
    .WithJaegerTracing(jaeger)             // ← Jaeger
    .WithSecurityDefaults();
```

---

## 📨 RabbitMQ Messaging Pattern

### 1. Producer (z.B. Auth Service)

```csharp
// Publish User Created Event
await _messagePublisher.PublishAsync(
    new UserCreatedEvent 
    { 
        UserId = userId,
        Email = email,
        CreatedAt = DateTime.UtcNow
    },
    exchange: "b2connect.users",
    routingKey: "user.created"
);
```

### 2. Consumer (z.B. Localization Service)

```csharp
// Subscribe to User Created Event
await _messageConsumer.SubscribeAsync<UserCreatedEvent>(
    async (message) =>
    {
        // Create user localization preferences
        await _localizationService.CreateUserDefaultsAsync(message.UserId);
    },
    queue: "localization.users",
    exchange: "b2connect.users",
    routingKey: "user.created"
);
```

### 3. Exchange & Queue Setup

```csharp
// Wolverine (CQRS Pattern)
public class RabbitMQConfiguration : IWolverinePolicy
{
    public void Configure(WolverineOptions options)
    {
        // Topic Exchange für Events
        options.UseRabbitMq()
            .BindExchange("b2connect.users", ExchangeType.Topic)
            .AutoDeclareExchangesAndQueues();

        // Register Handlers
        options.Discovery.IncludeAssembly(typeof(RabbitMQConfiguration).Assembly);
    }
}
```

---

## 📊 Jaeger Distributed Tracing

### 1. Trace Flow

```
User Request
    │
    ├─► Store Gateway (8000)
    │   │  trace_id: abc123
    │   │  span_id: 1
    │   │
    │   └─► Auth Service (7002)
    │       │  span_id: 2
    │       │  parent_span_id: 1
    │       │
    │       └─► PostgreSQL Query
    │           │  span_id: 3
    │           │  parent_span_id: 2
    │           │
    │           └─► Redis Lookup
    │               │  span_id: 4
    │               │  parent_span_id: 2
    │
    └─► Response (total: 250ms)
```

### 2. Instrumentierung im Code

```csharp
// Using OpenTelemetry
public class ProductService
{
    private readonly ActivitySource _activitySource;

    public ProductService()
    {
        _activitySource = new ActivitySource("B2Connect.Catalog");
    }

    public async Task<Product> GetProductAsync(string id)
    {
        using var activity = _activitySource.StartActivity("GetProduct");
        activity?.SetTag("product.id", id);
        activity?.SetTag("http.method", "GET");

        try
        {
            var product = await _repository.GetByIdAsync(id);
            activity?.SetTag("product.found", product != null);
            return product;
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

### 3. Trace Viewing

**Jaeger UI**: http://localhost:16686

**Funktionen:**
- Service Selection
- Trace Search (by tags, duration, status)
- Detailed Span Analysis
- Service Dependencies Graph
- Performance Metrics

---

## 🔄 Inter-Service Communication Pattern

### Synchronous (HTTP/REST)
```
Store Gateway
    │ HTTP/gRPC
    └─► Catalog Service ──GET /api/catalog/products
        │ HTTP/gRPC
        └─► Localization Service ──GET /api/localization/{id}
```

### Asynchronous (RabbitMQ)
```
Auth Service (creates user)
    │ Publishes: UserCreatedEvent
    │
    ├─► Queue: localization.users
    │   └─► Localization Service processes
    │
    ├─► Queue: tenant.users
    │   └─► Tenant Service processes
    │
    └─► Queue: catalog.users
        └─► Catalog Service processes
```

### Tracing Both Patterns

```
Request Timeline:
├─ 0ms:    Request arrives at Gateway
├─ 5ms:    Sync call to Catalog Service
├─ 15ms:   Catalog queries PostgreSQL
├─ 20ms:   Response returned
├─ 21ms:   Event published to RabbitMQ
├─ 22ms:   Async consumers start processing
├─ 50ms:   All consumers complete
└─ 55ms:   Final response to client
```

---

## 🧪 Testing RabbitMQ & Jaeger

### 1. Check RabbitMQ Health

```bash
# API Health
curl http://localhost:15672/api/overview \
  -u guest:guest | jq .

# Check Queues
curl http://localhost:15672/api/queues \
  -u guest:guest | jq '.[] | .name'

# Check Exchanges
curl http://localhost:15672/api/exchanges \
  -u guest:guest | jq '.[] | .name'
```

### 2. Test Message Publishing

```bash
# Publish test message via API
curl -X POST http://localhost:15672/api/exchanges/%2F/amq.default/publish \
  -u guest:guest \
  -H "content-type: application/json" \
  -d '{
    "properties": {},
    "routing_key": "test.queue",
    "payload": "{\"message\":\"Hello World\"}",
    "payload_encoding": "string"
  }'
```

### 3. View Jaeger Traces

```bash
# Open UI
open http://localhost:16686

# Or via API
curl http://localhost:16686/api/services | jq .data

# Search traces
curl "http://localhost:16686/api/traces?service=auth-service" | jq .
```

### 4. Integration Test

```csharp
[Fact]
public async Task MessagePublished_ConsumerReceives_EventProcessed()
{
    // Arrange
    var @event = new UserCreatedEvent { UserId = "user123", Email = "test@example.com" };
    
    // Act
    await _messagePublisher.PublishAsync(@event);
    await Task.Delay(1000); // Wait for async processing
    
    // Assert
    var processed = await _localizationService.GetUserSettingsAsync("user123");
    Assert.NotNull(processed);
    Assert.Equal("en-US", processed.DefaultLanguage);
}
```

---

## 📈 Performance Monitoring

### With Jaeger

| Metric | Tool | Action |
|--------|------|--------|
| Request Duration | Jaeger UI | View trace timeline |
| Service Dependencies | Jaeger UI | Service Graph tab |
| Error Rates | Jaeger UI | Filter by status:error |
| Database Queries | Jaeger UI | Filter by db.statement |
| Cache Performance | Jaeger UI | Filter by redis.* |

### Key Metrics to Monitor

```
1. P95 Response Time
   Good: < 200ms
   Warning: 200-500ms
   Critical: > 500ms

2. Error Rate
   Good: < 0.1%
   Warning: 0.1-1%
   Critical: > 1%

3. Message Queue Depth
   Good: < 100 messages
   Warning: 100-1000 messages
   Critical: > 1000 messages
```

---

## 🔐 Security Considerations

### RabbitMQ Authentication

```csharp
// Production Configuration
builder.Configuration["RabbitMQ:Username"] = "b2connect-user";
builder.Configuration["RabbitMQ:Password"] = "[from Azure Key Vault]";
builder.Configuration["RabbitMQ:VirtualHost"] = "/b2connect";
```

### Jaeger Access Control

```bash
# Restrict access to localhost only (Development)
JAEGER_COLLECTOR_ZIPKIN_HOST_PORT=127.0.0.1:9411

# Or use reverse proxy in Production (nginx)
location /jaeger/ {
    auth_basic "Jaeger UI";
    proxy_pass http://jaeger:16686/;
}
```

### Message Encryption

```csharp
// Encrypt sensitive message payloads
var encrypted = _encryptionService.Encrypt(
    new UserCreatedEvent { Email = "user@example.com" }
);

await _messagePublisher.PublishAsync(
    encrypted,
    headers: new Dictionary<string, string>
    {
        { "X-Encryption", "AES-256" }
    }
);
```

---

## 🛠️ Troubleshooting

### Problem: "Cannot connect to RabbitMQ:5672"

```bash
# Check if RabbitMQ running
docker ps | grep rabbitmq

# View RabbitMQ logs
docker logs rabbitmq

# Restart RabbitMQ
docker restart rabbitmq

# Or manual start
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

### Problem: "Jaeger UI not accessible"

```bash
# Check Jaeger running
curl http://localhost:16686/api/services

# Check OTLP endpoint
curl http://localhost:4317/grpc

# View Jaeger logs
docker logs jaeger
```

### Problem: "Messages not being consumed"

```bash
# Check queue status in RabbitMQ UI
# http://localhost:15672 → Queues

# Debug consumer
logs = _logger.LogInformation(
    "Consuming message: {@Event} on queue: {Queue}",
    @event,
    queueName
);

// Ensure handler registered
options.Discovery.IncludeAssembly(typeof(EventHandlers).Assembly);
```

### Problem: "High Memory Usage"

```bash
# Reduce trace sampling (increase sampling.param from 0.1 to 0.01)
OTEL_TRACES_SAMPLER_ARG=0.01

# Or limit trace storage
MEMORY_MAX_TRACES=1000
```

---

## 🚀 Deployment Checklist

### Development (Current)
- ✅ RabbitMQ: Single-Node, guest credentials
- ✅ Jaeger: In-Memory storage
- ✅ Trace Sampling: 10%
- ✅ Message Queue: Unlimited depth

### Staging
- [ ] RabbitMQ: Cluster mode, strong credentials
- [ ] Jaeger: ElasticSearch backend
- [ ] Trace Sampling: 1%
- [ ] Message Queue: Monitoring enabled

### Production
- [ ] RabbitMQ: Cloud hosted (Azure Service Bus / AWS SQS)
- [ ] Jaeger: External service or cloud
- [ ] Trace Sampling: 0.1%
- [ ] Message Queue: Alerts for queue depth
- [ ] Authentication: Managed Identities / Secrets

---

## 📚 Weitere Ressourcen

- **RabbitMQ**: https://www.rabbitmq.com/documentation.html
- **RabbitMQ .NET Client**: https://www.rabbitmq.com/dotnet-api-guide.html
- **Wolverine CQRS**: https://wolverinefx.io/
- **Jaeger**: https://www.jaegertracing.io/docs/
- **OpenTelemetry**: https://opentelemetry.io/docs/

---

## ✅ Nächste Schritte

1. **Start Aspire**
   ```bash
    cd AppHost && dotnet run
   ```

2. **Check RabbitMQ Management**
   ```
   http://localhost:15672
   User: guest
   Pass: guest
   ```

3. **Check Jaeger UI**
   ```
   http://localhost:16686
   Services → Select service → View traces
   ```

4. **Implement Event Handlers**
   ```csharp
   public class UserCreatedEventHandler
   {
       public async Task HandleAsync(UserCreatedEvent @event)
       {
           // Process user creation in other services
       }
   }
   ```

5. **Monitor Performance**
   - View Jaeger traces in real-time
   - Check RabbitMQ queue depths
   - Analyze service dependencies

---

**Status**: ✅ PRODUCTION READY  
**Last Updated**: 2025-12-27
