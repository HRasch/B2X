# ADR-023: ERP/CRM/PIM Plugin Architecture

**Status:** Proposed  
**Date:** 2026-01-02  
**Owner:** @Architect  
**Stakeholders:** @Backend, @DevOps, @ProductOwner, @Security

## Context

B2Connect requires integration with various ERP/CRM/PIM systems (starting with enventa Trade ERP) while maintaining:
- Multi-tenant isolation
- Customer-specific customizations
- System stability and maintainability
- Modern .NET 10 architecture compatibility

Legacy ERP systems often use .NET Framework 4.8, requiring isolation strategies.

## Decision

Implement a **Plugin-based Architecture** using containerized ERP providers with standardized interfaces.

### Architecture Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   B2Connect     │────│  Provider        │────│  ERP Container  │
│   Core System   │    │  Manager         │    │  (Docker)       │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │  Plugin         │
                       │  Registry       │
                       └─────────────────┘
```

### Key Components

1. **Standardized Interfaces**
   - `IErpProvider`: Core ERP operations
   - `ICrmProvider`: CRM functionality
   - `IPimProvider`: Product Information Management

2. **Containerized Providers**
   - Each ERP system runs in isolated Docker containers
   - Windows containers for .NET Framework compatibility
   - Linux containers for modern implementations

3. **Provider Manager**
   - Dynamic container orchestration
   - Tenant-specific provider instantiation
   - Health monitoring and failover

4. **Plugin Registry**
   - Centralized catalog of available providers
   - Version management and compatibility matrix
   - Customer-specific provider deployments

## Implementation Details

### Provider Interface Contracts

```csharp
public interface IErpProvider
{
    // Single Operations (for simple queries)
    Task<PimData> GetProductAsync(string productId, TenantContext context);
    Task<CrmData> GetCustomerAsync(string customerId, TenantContext context);
    Task<ErpOrder> CreateOrderAsync(OrderRequest request, TenantContext context);
    
    // Bulk Operations (to reduce chatty interfaces)
    Task<IEnumerable<PimData>> GetProductsAsync(IEnumerable<string> productIds, TenantContext context);
    Task<IEnumerable<CrmData>> GetCustomersAsync(IEnumerable<string> customerIds, TenantContext context);
    Task<BatchResult> CreateOrdersAsync(IEnumerable<OrderRequest> requests, TenantContext context);
    
    // Streaming Operations (for large datasets)
    IAsyncEnumerable<PimData> StreamProductsAsync(ProductFilter filter, TenantContext context);
    IAsyncEnumerable<CrmData> StreamCustomersAsync(CustomerFilter filter, TenantContext context);
    
    // Batch Sync Operations (optimized for bulk data transfer)
    Task SyncProductsAsync(IEnumerable<Product> products, TenantContext context);
    Task SyncCustomersAsync(IEnumerable<Customer> customers, TenantContext context);
    Task<SyncResult> BulkSyncAsync(BulkSyncRequest request, TenantContext context);
}

public interface ICrmProvider
{
    Task<CrmContact> GetContactAsync(string contactId, TenantContext context);
    Task<IEnumerable<CrmActivity>> GetActivitiesAsync(string customerId, TenantContext context);
    
    // Bulk operations to minimize network roundtrips
    Task<IEnumerable<CrmContact>> GetContactsAsync(IEnumerable<string> contactIds, TenantContext context);
    Task<Dictionary<string, IEnumerable<CrmActivity>>> GetActivitiesBatchAsync(IEnumerable<string> customerIds, TenantContext context);
}

public interface IPimProvider
{
    Task<PimProduct> GetProductDetailsAsync(string productId, TenantContext context);
    Task<IEnumerable<PimCategory>> GetCategoriesAsync(TenantContext context);
    
    // Optimized for catalog operations
    Task<CatalogData> GetCatalogAsync(CatalogFilter filter, TenantContext context);
    IAsyncEnumerable<PimProduct> StreamCatalogAsync(CatalogFilter filter, TenantContext context);
}
```

### Performance Optimization Strategies

#### 1. **Batch Processing Pattern**
- **Problem**: Multiple individual calls create network overhead
- **Solution**: Bundle related operations into single requests
- **Implementation**: `GetProductsAsync(IEnumerable<string>)` instead of multiple `GetProductAsync(string)`

#### 2. **Streaming for Large Datasets**
- **Problem**: Loading entire result sets into memory
- **Solution**: Async streaming with `IAsyncEnumerable<T>`
- **Benefits**: Reduced memory usage, faster time-to-first-result

#### 3. **Local Caching in Provider Containers**
- **Strategy**: Redis cache within provider containers for frequently accessed data
- **TTL**: 5-15 minutes for master data, 1-5 minutes for transactional data
- **Invalidation**: Event-driven cache invalidation via Wolverine messages

#### 4. **Connection Pooling and Keep-Alive**
- **Database Connections**: Maintain persistent connections to ERP databases
- **HTTP Clients**: Reuse connections with keep-alive headers
- **Circuit Breaker**: Polly policies for resilient communication

#### 5. **Bulk Synchronization Optimization**
- **Change Detection**: Only sync changed records using timestamps/modified flags
- **Delta Sync**: Incremental updates instead of full dataset transfers
- **Parallel Processing**: Multiple threads for independent sync operations

### Performance Benchmarks

| Operation Type | Target Latency | Max Network Calls | Memory Usage |
|----------------|----------------|-------------------|--------------|
| Single Product Query | <100ms | 1 | <50MB |
| Bulk Product Query (100 items) | <500ms | 1 | <200MB |
| Catalog Stream (1000+ items) | <2s time-to-first | 1 initial | <100MB |
| Bulk Sync (10k records) | <30s | 1-5 batches | <500MB |
| Order Creation | <200ms | 1-2 | <100MB |

### Monitoring and Performance Metrics

#### Key Performance Indicators
- **Network Latency**: P95 response time for provider calls
- **Throughput**: Operations per second per tenant
- **Error Rate**: Failed operations percentage
- **Cache Hit Rate**: >80% for frequently accessed data
- **Memory Usage**: Container memory consumption trends

#### Observability Setup
```yaml
# Prometheus metrics for ERP performance
erp_provider_requests_total{operation="GetProduct", tenant="tenant-123", status="success"} 1547
erp_provider_request_duration_seconds{operation="BulkSync", quantile="0.95"} 2.5
erp_provider_cache_hit_ratio{provider="enventa"} 0.85
```

### Container Orchestration

- **Kubernetes-based deployment** with tenant namespaces
- **Docker Compose** for development environments
- **Health checks** via HTTP probes with performance thresholds
- **Auto-scaling** based on tenant usage and latency metrics

### Tenant Configuration

```json
{
  "tenantId": "tenant-123",
  "erpProvider": {
    "type": "enventa",
    "image": "b2connect/erp-enventa:v1.2.3",
    "config": {
      "connectionString": "...",
      "syncBatchSize": 1000
    }
  },
  "crmProvider": {
    "type": "salesforce",
    "image": "b2connect/crm-salesforce:v2.1.0"
  }
}
```

## Consequences

### Positive

- **Flexibility**: Customer-specific ERP integrations without core system changes
- **Isolation**: Legacy systems don't impact modern architecture
- **Scalability**: Independent scaling of ERP providers
- **Maintainability**: Modular updates and testing
- **Future-proof**: Easy addition of new ERP systems
- **Performance**: Optimized batch/streaming operations reduce network overhead

### Negative

- **Complexity**: Additional orchestration layer
- **Latency**: Network calls between containers (mitigated by batch operations)
- **Resource overhead**: Container management
- **Development effort**: Standardized interfaces required

### Risks

- **Provider compatibility**: Ensuring interface contracts are met
- **Performance**: Bulk data synchronization efficiency (addressed via batch processing)
- **Security**: Inter-container communication security
- **Operational complexity**: Container orchestration management
- **Chatty Interfaces**: Multiple network calls causing latency (mitigated by bulk APIs)

## Alternatives Considered

### 1. Direct Assembly Integration
- **Rejected**: Incompatible with .NET Framework → .NET 10 migration

### 2. Monolithic ERP Service
- **Rejected**: No customer-specific customizations, tight coupling

### 3. API Gateway Pattern
- **Rejected**: Less isolation, harder tenant separation

## Implementation Plan

### Phase 1: Foundation (2 weeks)
- Define interface contracts with batch/streaming operations
- Create base Docker images with caching infrastructure
- Implement Provider Manager skeleton with performance monitoring
- Setup basic performance benchmarks

### Phase 2: enventa Integration (4 weeks)
- Develop enventa provider container with optimized bulk operations
- Implement PIM/CRM/ERP interfaces with caching strategies
- Integration testing with B2Connect focusing on performance
- Optimize for "chatty" ERP operations via batch processing

### Phase 3: Production Ready (3 weeks)
- Plugin registry system with performance metrics
- Advanced monitoring and health checks
- Security hardening with performance impact assessment
- Documentation and performance tuning guides

## Success Metrics

- **Functionality**: All ERP operations working across tenants
- **Performance**: 
  - <100ms latency for single operations
  - <500ms for bulk operations (up to 100 items)
  - <2s time-to-first-result for streaming operations
  - <30s for bulk sync (10k records)
- **Reliability**: 99.9% uptime for ERP services
- **Maintainability**: New provider integration in <2 weeks
- **Security**: Zero data leakage between tenants
- **Efficiency**: >80% cache hit rate, <5 network calls per complex operation

## Related Documents

- [ADR-022] Multi-Tenant Domain Management
- [KB-006] Wolverine Patterns
- [GL-002] Subagent Delegation

---

**Next Steps:**
1. @Backend: Define detailed interface contracts
2. @DevOps: Create base Docker image templates
3. @Architect: Review and approve implementation approach
4. @QA: Define integration test strategy