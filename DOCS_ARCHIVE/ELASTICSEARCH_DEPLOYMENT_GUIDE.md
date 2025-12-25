# Elasticsearch Integration - Deployment & Configuration Guide

## Overview

This guide covers deployment of the Elasticsearch-based product search system, including all necessary infrastructure components and configurations.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    StoreFront (Vue.js)                      │
│                  /api/catalog/products/search                │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│              API Gateway (Reverse Proxy)                     │
│                   /api/catalog/*                             │
└────────────────────────────┬────────────────────────────────┘
                             │
          ┌──────────────────┼──────────────────┐
          │                  │                  │
          ▼                  ▼                  ▼
    ┌──────────┐      ┌──────────┐      ┌──────────┐
    │ Catalog  │      │  Search  │      │  Layout  │
    │ Service  │      │ Service  │      │ Service  │
    └────┬─────┘      └────┬─────┘      └──────────┘
         │                 │
         │      ┌──────────┘
         │      │
         ▼      ▼
    ┌──────────────────────┐
    │   RabbitMQ (Broker)  │
    │  product-events      │
    │  exchange            │
    └──────────┬───────────┘
               │
         ┌─────┴─────┐
         │           │
         ▼           ▼
    ┌──────────┐  ┌──────────┐
    │ Postgres │  │Search    │
    │(Products)│  │Index Svc │
    └──────────┘  └────┬─────┘
                       │
                       ▼
                  ┌──────────────┐
                  │Elasticsearch │
                  │  (products)  │
                  │   index      │
                  └────┬─────────┘
                       │
                  ┌────┴─────┐
                  │           │
                  ▼           ▼
                ┌───┐     ┌─────────┐
                │ L │     │  Cache  │
                │ B │     │(Redis)  │
                │ S │     │         │
                └───┘     └─────────┘
```

## Infrastructure Components

### 1. Elasticsearch Cluster

#### Docker Deployment

```yaml
# docker-compose.yml
elasticsearch:
  image: docker.elastic.co/elasticsearch/elasticsearch:8.12.0
  environment:
    - discovery.type=single-node
    - xpack.security.enabled=false
    - ELASTIC_PASSWORD=changeme
    - ES_JAVA_OPTS=-Xms512m -Xmx512m
  ports:
    - "9200:9200"
  volumes:
    - elasticsearch-data:/usr/share/elasticsearch/data
  healthcheck:
    test: ["CMD-SHELL", "curl -f http://localhost:9200/_cluster/health || exit 1"]
    interval: 10s
    timeout: 5s
    retries: 5

volumes:
  elasticsearch-data:
```

#### Kubernetes Deployment

```yaml
# kubernetes/elasticsearch-deployment.yaml
apiVersion: v1
kind: Service
metadata:
  name: elasticsearch
spec:
  ports:
    - port: 9200
      name: rest
    - port: 9300
      name: inter-node
  clusterIP: None
  selector:
    app: elasticsearch
---
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: es-cluster
spec:
  serviceName: elasticsearch
  replicas: 3
  selector:
    matchLabels:
      app: elasticsearch
  template:
    metadata:
      labels:
        app: elasticsearch
    spec:
      containers:
      - name: elasticsearch
        image: docker.elastic.co/elasticsearch/elasticsearch:8.12.0
        resources:
          limits:
            cpu: 1000m
            memory: 2Gi
          requests:
            cpu: 500m
            memory: 1Gi
        ports:
        - containerPort: 9200
          name: rest
        - containerPort: 9300
          name: inter-node
        env:
        - name: cluster.name
          value: b2connect
        - name: node.name
          valueFrom:
            fieldRef:
              fieldPath: metadata.name
        - name: discovery.seed_hosts
          value: "es-cluster-0.elasticsearch,es-cluster-1.elasticsearch,es-cluster-2.elasticsearch"
        - name: cluster.initial_master_nodes
          value: "es-cluster-0,es-cluster-1,es-cluster-2"
        volumeMounts:
        - name: data
          mountPath: /usr/share/elasticsearch/data
  volumeClaimTemplates:
  - metadata:
      name: data
    spec:
      accessModes: [ "ReadWriteOnce" ]
      storageClassName: fast
      resources:
        requests:
          storage: 30Gi
```

### 2. RabbitMQ Configuration

#### Docker Deployment

```yaml
rabbitmq:
  image: rabbitmq:3.13-management
  environment:
    - RABBITMQ_DEFAULT_USER=guest
    - RABBITMQ_DEFAULT_PASS=guest
  ports:
    - "5672:5672"    # AMQP
    - "15672:15672"  # Management UI
  volumes:
    - rabbitmq-data:/var/lib/rabbitmq
  healthcheck:
    test: ["CMD-SHELL", "rabbitmq-diagnostics -q ping"]
    interval: 10s
    timeout: 5s
    retries: 5

volumes:
  rabbitmq-data:
```

#### Queue Configuration

```bash
#!/bin/bash
# Configure RabbitMQ from Catalog Service on startup

docker exec rabbitmq rabbitmqctl add_user b2connect b2connect123 || true
docker exec rabbitmq rabbitmqctl set_permissions -p / b2connect ".*" ".*" ".*"

# Create exchange and queues
docker exec rabbitmq rabbitmq-plugins enable rabbitmq_management

# These would be created by the services on startup:
# - Exchange: product-events (topic)
# - Queue: search-index-updates
# - Binding: product-events -> search-index-updates (routing keys: product.*, products.*)
```

### 3. Redis Cache

#### Docker Deployment

```yaml
redis:
  image: redis:7-alpine
  ports:
    - "6379:6379"
  volumes:
    - redis-data:/data
  command: redis-server --appendonly yes
  healthcheck:
    test: ["CMD", "redis-cli", "ping"]
    interval: 10s
    timeout: 5s
    retries: 5

volumes:
  redis-data:
```

### 4. PostgreSQL (Existing)

Ensure PostgreSQL is running with products table:

```sql
CREATE TABLE products (
  id UUID PRIMARY KEY,
  sku VARCHAR(100) NOT NULL UNIQUE,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  category VARCHAR(100),
  price DECIMAL(10, 2) NOT NULL,
  b2b_price DECIMAL(10, 2),
  stock_quantity INT DEFAULT 0,
  tenant_id UUID NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (tenant_id) REFERENCES tenants(id)
);

CREATE INDEX idx_products_tenant ON products(tenant_id);
CREATE INDEX idx_products_category ON products(category);
```

## Service Configuration

### Catalog Service Configuration

**appsettings.json:**

```json
{
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=postgres;Database=b2connect;User Id=postgres;Password=postgres;"
  }
}
```

**Program.cs:**

```csharp
// Add RabbitMQ and Event Publisher
var connectionFactory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"],
    Port = int.Parse(builder.Configuration["RabbitMQ:Port"]),
    Username = builder.Configuration["RabbitMQ:Username"],
    Password = builder.Configuration["RabbitMQ:Password"]
};

builder.Services.AddSingleton<IConnectionFactory>(connectionFactory);
builder.Services.AddSingleton<IEventPublisher>(sp =>
{
    var publisher = new RabbitMqEventPublisher(
        connectionFactory,
        sp.GetRequiredService<ILogger<RabbitMqEventPublisher>>());
    publisher.Initialize();
    return publisher;
});
```

### Search Service Configuration

**appsettings.json:**

```json
{
  "Elasticsearch": {
    "Nodes": ["http://elasticsearch:9200"],
    "Username": "elastic",
    "Password": "changeme"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  },
  "Redis": {
    "Connection": "redis:6379"
  }
}
```

**Program.cs:**

```csharp
// Add Search Services
builder.Services.AddSearchServices(builder.Configuration);

var app = builder.Build();

// Middleware
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Health checks
app.MapHealthChecks("/health");

await app.RunAsync();
```

## Aspire Integration

### AppHost Configuration

```csharp
// Program.cs in AppHost
var builder = DistributedApplication.CreateBuilder(args);

// Add infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithHealthCheck();

var redis = builder.AddRedis("redis");

// Add services
var catalogService = builder
    .AddProject<Projects.CatalogService>("catalog-service")
    .WithReference(postgres)
    .WithReference(rabbitmq);

var searchService = builder
    .AddProject<Projects.SearchService>("search-service")
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(redis);

var layoutService = builder
    .AddProject<Projects.LayoutService>("layout-service")
    .WithReference(postgres);

var apiGateway = builder
    .AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(catalogService)
    .WithReference(searchService)
    .WithReference(layoutService);

var frontend = builder
    .AddNpmApp("frontend", "../../frontend")
    .WithReference(apiGateway)
    .WithEnvironment("VITE_API_URL", apiGateway.GetEndpoint("http"));

builder.Build().Run();
```

## Deployment Steps

### Local Development

1. **Start Infrastructure:**
   ```bash
   docker-compose up -d elasticsearch rabbitmq redis postgres
   
   # Wait for health checks
   docker-compose ps
   ```

2. **Start Services (Option A - Using Aspire):**
   ```bash
   cd backend/services/AppHost
   dotnet run
   ```

3. **Start Services (Option B - Manual):**
   ```bash
   # Terminal 1: Catalog Service
   cd backend/services/CatalogService
   dotnet run
   
   # Terminal 2: Search Service
   cd backend/services/SearchService
   dotnet run
   
   # Terminal 3: API Gateway
   cd backend/services/api-gateway
   dotnet run
   
   # Terminal 4: Frontend
   cd frontend
   npm run dev
   ```

4. **Verify Everything:**
   ```bash
   # Check Elasticsearch
   curl http://localhost:9200/_cluster/health
   
   # Check RabbitMQ Management
   # http://localhost:15672 (guest:guest)
   
   # Check health endpoints
   curl http://localhost:5006/health        # Catalog
   curl http://localhost:5007/health        # Search
   ```

### Docker Deployment

1. **Build Images:**
   ```bash
   # Build Catalog Service
   docker build -f backend/services/CatalogService/Dockerfile \
     -t b2connect/catalog-service:latest .
   
   # Build Search Service
   docker build -f backend/services/SearchService/Dockerfile \
     -t b2connect/search-service:latest .
   ```

2. **Deploy Stack:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.aspire.yml up -d
   ```

### Kubernetes Deployment

1. **Create Namespace:**
   ```bash
   kubectl create namespace b2connect
   ```

2. **Deploy Elasticsearch:**
   ```bash
   kubectl apply -f kubernetes/elasticsearch-deployment.yaml -n b2connect
   
   # Wait for cluster to be ready
   kubectl wait --for=condition=ready pod \
     -l app=elasticsearch -n b2connect --timeout=300s
   ```

3. **Deploy RabbitMQ:**
   ```bash
   kubectl apply -f kubernetes/rabbitmq-deployment.yaml -n b2connect
   ```

4. **Deploy Services:**
   ```bash
   kubectl apply -f kubernetes/catalog-service.yaml -n b2connect
   kubectl apply -f kubernetes/search-service.yaml -n b2connect
   kubectl apply -f kubernetes/api-gateway.yaml -n b2connect
   ```

5. **Verify Deployment:**
   ```bash
   kubectl get pods -n b2connect
   kubectl logs -f deployment/search-service -n b2connect
   ```

## Monitoring & Health Checks

### Health Check Endpoints

```bash
# Elasticsearch cluster health
curl http://elasticsearch:9200/_cluster/health

# Search Service health
curl http://localhost:5007/health

# Catalog Service health
curl http://localhost:5006/health
```

### Prometheus Metrics

Configure Prometheus scrape config:

```yaml
# prometheus.yml
scrape_configs:
  - job_name: 'search-service'
    static_configs:
      - targets: ['localhost:5007']
    metrics_path: '/metrics'
  
  - job_name: 'catalog-service'
    static_configs:
      - targets: ['localhost:5006']
    metrics_path: '/metrics'
```

### Grafana Dashboard

Key metrics to track:

- **Search Performance**: 95th percentile query latency
- **Indexing Lag**: Time between event published and document indexed
- **Cache Hit Rate**: Percentage of searches served from cache
- **RabbitMQ Queue Length**: Unprocessed events in queue
- **Elasticsearch Cluster Health**: Node count, shard allocation
- **Error Rates**: Failed indexing operations, failed searches

## Troubleshooting

### Elasticsearch Issues

```bash
# Check cluster status
curl http://elasticsearch:9200/_cluster/health?pretty

# Check index status
curl http://elasticsearch:9200/products/_stats?pretty

# Reindex (if needed)
curl -X POST http://elasticsearch:9200/_reindex \
  -H "Content-Type: application/json" \
  -d '{
    "source": { "index": "products" },
    "dest": { "index": "products-v2" }
  }'
```

### RabbitMQ Issues

```bash
# Check queue length
docker exec rabbitmq rabbitmqctl list_queues

# Purge queue (warning: deletes messages)
docker exec rabbitmq rabbitmqctl purge_queue search-index-updates

# Reset connection (if stuck)
docker exec rabbitmq rabbitmqctl reset
```

### Search Service Issues

```bash
# Check logs
docker logs search-service

# Check RabbitMQ connection
# Add logging in SearchIndexService

# Verify index exists
curl http://elasticsearch:9200/_cat/indices
```

## Performance Tuning

### Elasticsearch

1. **Increase heap size** for large datasets:
   ```yaml
   environment:
     - ES_JAVA_OPTS=-Xms2g -Xmx2g
   ```

2. **Adjust shard count** based on data size:
   - Small (<1GB): 1 shard
   - Medium (1-10GB): 3 shards
   - Large (>10GB): 5+ shards

3. **Tune refresh interval**:
   ```bash
   curl -X PUT http://elasticsearch:9200/products/_settings \
     -H "Content-Type: application/json" \
     -d '{"index.refresh_interval":"30s"}'
   ```

### Redis Cache

1. **Enable persistence**:
   ```bash
   redis-server --appendonly yes --appendfsync everysec
   ```

2. **Monitor memory**:
   ```bash
   redis-cli info memory
   ```

### RabbitMQ

1. **Prefetch settings**:
   ```csharp
   channel.BasicQos(0, 10, false); // Per-consumer limit
   ```

2. **Increase VM limits**:
   ```bash
   docker update --memory 2g rabbitmq
   ```

## Maintenance

### Backup Elasticsearch

```bash
# Create snapshot repository
curl -X PUT http://elasticsearch:9200/_snapshot/backup \
  -H "Content-Type: application/json" \
  -d '{
    "type": "fs",
    "settings": {
      "location": "/backup"
    }
  }'

# Take snapshot
curl -X PUT http://elasticsearch:9200/_snapshot/backup/snapshot-1
```

### Clean Old Indices

```bash
# Delete indices older than 30 days
curator delete indices --filter_list '[{"filtertype":"age","source":"creation_date","direction":"older","unit":"days","unit_count":30}]'
```

## Security Considerations

1. **Enable Elasticsearch Security**:
   ```yaml
   environment:
     - xpack.security.enabled=true
   ```

2. **RabbitMQ Authentication**:
   - Change default username/password
   - Use SSL/TLS for connections

3. **API Gateway**:
   - Require authentication for /api/admin/* endpoints
   - Rate limiting on search endpoints

4. **Network Security**:
   - Run services in private network
   - Use VPC for cloud deployments
   - Restrict Elasticsearch to internal traffic only
