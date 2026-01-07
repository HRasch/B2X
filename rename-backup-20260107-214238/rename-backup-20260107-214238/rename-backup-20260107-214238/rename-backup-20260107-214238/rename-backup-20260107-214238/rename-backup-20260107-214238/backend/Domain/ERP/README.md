# ERP Domain - enventa Integration Implementation

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                        B2Connect (.NET 10)                          │
│  ┌────────────────────┐    ┌────────────────────────────────────┐  │
│  │ ERP.Abstractions   │    │ ERP Service                        │  │
│  │ - DTOs             │    │ - HTTP Client                      │  │
│  │ - HTTP Contracts   │◄───┤ - Delta Sync Consumer              │  │
│  │ - INVContext       │    │ - Batch Import                     │  │
│  └────────────────────┘    └────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                                    │ HTTP/REST
                                    │ (JSON, GZip)
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│              ERP Connector (.NET Framework 4.8)                     │
│  ┌────────────────────┐    ┌────────────────────────────────────┐  │
│  │ enventa DAL        │    │ HTTP API (Web API 2)               │  │
│  │ - FSGlobalPool     │◄───┤ - /api/articles/sync               │  │
│  │ - FSUtil           │    │ - /api/articles/stream             │  │
│  │ - NVContext        │    │ - /api/articles/batch              │  │
│  └────────────────────┘    └────────────────────────────────────┘  │
│                                                                     │
│  Runs in Windows Container (enventa assemblies require .NET 4.8)   │
└─────────────────────────────────────────────────────────────────────┘
```

## High-Volume Data Transfer Techniques

All techniques are compatible with both **.NET Framework 4.8** and **.NET 10**.

### 1. Cursor-Based Pagination
- Consistent results during sync (unlike offset pagination)
- Efficient for large datasets with indexed cursor column

```csharp
// Request
GET /api/articles?cursor=eyJpZCI6MTAwMH0&pageSize=1000

// Response includes NextCursor for next page
{ "items": [...], "nextCursor": "eyJpZCI6MjAwMH0", "hasMore": true }
```

### 2. Delta Sync (Watermark-Based)
- Only transfer changed records since last sync
- Uses watermark (rowversion/timestamp) for tracking
- Includes tombstones for deleted records

```csharp
// Request - only changes since watermark 12345
GET /api/articles/sync?watermark=12345&batchSize=5000

// Response with new watermark
{ "changes": [...], "newWatermark": 67890, "hasMore": false }
```

### 3. Chunked Streaming (JSON Lines)
- Memory-efficient for massive datasets
- GZip compression reduces bandwidth 70-80%
- JSON Lines format allows streaming parsing

```csharp
// Request streaming export
GET /api/articles/stream?format=jsonl&useCompression=true
Accept-Encoding: gzip

// Response: newline-delimited JSON (streaming)
{"articleId":1,"name":"Product 1"}
{"articleId":2,"name":"Product 2"}
...
```

### 4. Batch Operations
- Bulk insert/update with configurable chunk sizes
- Continue-on-error for partial failures
- Detailed error reporting per item

```csharp
// Batch upsert
POST /api/articles/batch
{ "items": [...], "mode": "Upsert", "continueOnError": true }
```

## Project Structure

```
backend/Domain/ERP/
├── Abstractions/                    # .NET 10 - Shared contracts
│   ├── B2Connect.ERP.Abstractions.csproj
│   ├── ErpDtos.cs                   # Entity DTOs
│   ├── NVShopTypes.cs               # INVContext interface & models
│   └── Http/
│       └── ErpHttpContracts.cs      # HTTP communication contracts
│
├── src/                             # .NET 10 - B2Connect ERP service
│   ├── B2Connect.ERP.csproj
│   ├── Services/                    # ERP service implementation
│   ├── Sync/                        # Delta sync consumer
│   └── Infrastructure/              # HTTP client, resilience
│
└── tests/                           # Unit & integration tests
```

## Key HTTP Contracts

### CursorPageRequest / CursorPageResponse<T>
Cursor-based pagination for consistent results during sync.

### DeltaSyncRequest / DeltaSyncResponse<T>
Watermark-based delta sync with change tracking.

### StreamRequest / ChunkHeader
Chunked streaming with GZip compression.

### BatchWriteRequest<T> / BatchWriteResponse<T>
Bulk operations with detailed error reporting.

## HTTP Headers

| Header | Purpose |
|--------|---------|
| `X-Tenant-Id` | Multi-tenant isolation |
| `X-Watermark` | Delta sync tracking |
| `X-Continuation-Token` | Pagination state |
| `X-Total-Count` | Total records (optional) |
| `X-Content-Checksum` | Data integrity |
| `X-Correlation-Id` | Request tracing |

## API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/health` | GET | Connector health check |
| `/api/articles` | GET | Cursor-paginated list |
| `/api/articles/sync` | GET | Delta sync |
| `/api/articles/stream` | GET | Streaming export |
| `/api/articles/batch` | POST | Bulk upsert |

## Compatibility Notes

### .NET Framework 4.8 (ERP Connector)
- Uses `System.Text.Json` via NuGet (or Newtonsoft.Json)
- Web API 2 for HTTP endpoints
- `Task<T>` for async (no IAsyncEnumerable)
- GZip via `System.IO.Compression`

### .NET 10 (B2Connect)
- Native `System.Text.Json`
- `IAsyncEnumerable<T>` for streaming consumption
- `HttpClient` with `SocketsHttpHandler`
- Built-in GZip/Brotli support

## Performance Targets

| Metric | Target |
|--------|--------|
| Articles full sync | 100,000+ records |
| Delta sync batch | 5,000 records |
| Throughput | 10,000 records/sec |
| Response time (paged) | < 500ms |
| Memory (streaming) | Constant ~50MB |
- Runs in .NET Framework 4.8 container
- Translates DTO queries to enventa ORM calls
- Thread-safe via Actor pattern

### 6. Service Interface (`ErpService.cs`)
- Clean interface for B2Connect application
- Async streaming for large datasets
- Batch operations for performance

## Usage Examples

### Simple Article Query
```csharp
var articles = await _erpService.QueryArticlesAsync(q =>
    q.WhereCategory("ELECTRONICS")
     .IncludePricing()
     .OrderBy("Name")
     .Take(50));
```

### Complex Query with Multiple Conditions
```csharp
await foreach (var article in _erpService.QueryArticlesAsync(q =>
    q.WhereCategory("TOOLS")
     .WherePriceBetween(10, 100)
     .WhereInStock()
     .IncludeAll()
     .OrderBy("BasePrice")))
{
    // Process article
}
```

### Batch Operations
```csharp
var articleIds = new[] { "ART001", "ART002", "ART003" };
var articles = await _erpService.GetArticlesByIdsAsync(articleIds);
```

### Sync Operations
```csharp
var result = await _erpService.SyncArticlesAsync(updatedArticles);
Console.WriteLine($"Synced {result.SuccessCount}/{result.ProcessedCount} articles");
```

## Architecture Benefits

✅ **Type Safety**: Strongly typed DTOs and query builders  
✅ **Performance**: Batch operations, streaming, connection pooling  
✅ **Maintainability**: Clean separation between B2Connect and ERP logic  
✅ **Scalability**: Per-tenant isolation, async operations  
✅ **Compatibility**: Cross-framework communication via gRPC  
✅ **Testability**: Mockable interfaces, isolated components  

## Based on eGate Patterns

- **FSGlobalPool**: Connection pooling with health checks
- **Query Builder**: Fluent API for complex queries
- **Sync Records**: ID mapping and delta sync
- **Repository Pattern**: Abstraction layers for ERP operations
- **Batch Processing**: Chunking for large datasets (1000 items)
- **Actor Pattern**: Thread-safe ERP access

This implementation provides a robust, scalable foundation for enventa ERP integration while maintaining clean architecture principles.