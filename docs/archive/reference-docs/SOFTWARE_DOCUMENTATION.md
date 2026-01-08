# 📚 Software-Dokumentation (Technische Dokumentation)

**Dokument-Owner:** Documentation Team  
**Status:** Comprehensive Technical Reference  
**Zielgruppe:** Entwickler, Architekten, DevOps Engineers  
**Aktualität:** 27. Dezember 2025

---

## Übersicht

Die B2X Software-Dokumentation bietet technische Referenzen, API-Spezifikationen, Deploymentanleitungen und Entwickler-Workflows.

---

## 📂 Dokumentations-Struktur

### 1. **Architektur-Dokumentation**

#### [ONION_ARCHITECTURE.md](../architecture-docs/ONION_ARCHITECTURE.md)
**Zweck:** Beschreibung der Schichtenarchitektur  
**Inhalte:**
- Layer-Definition (Presentation, Application, Infrastructure, Core)
- Dependency Direction Rules
- Cross-Cutting Concerns
- Service-Beispiele

#### [GATEWAY_SEPARATION.md](../../guides/GATEWAY_SEPARATION.md)
**Zweck:** API Gateway Pattern Implementation  
**Inhalte:**
- Admin Gateway (Port 8080)
- Store Gateway (Port 8000)
- Routing-Strategien
- Authentication Flow

#### [STORE_SEPARATION_STRUCTURE.md](../../architecture/STORE_SEPARATION_STRUCTURE.md)
**Zweck:** Bounded Context Separation für Store  
**Inhalte:**
- Store-spezifische Architektur
- Integration mit anderen Services
- Data Flow

---

### 2. **API-Dokumentation**

#### [APPLICATION_SPECIFICATIONS.md](docs/guides/index.md)
**Zweck:** Komplette API-Spezifikation  
**Inhalte:**
- Common Response Format
- Status Code Katalog (200, 201, 400, 401, 403, 404, 422, 429, 500, 503)
- Authentication Headers (Authorization, X-Tenant-ID, X-Request-ID)
- Pagination Format
- Error Response Format

**Beispiel Response:**
```json
{
  "status": "success",
  "data": { "id": "123", "name": "Product" },
  "errors": [],
  "meta": {
    "timestamp": "2025-12-27T10:00:00Z",
    "version": "1.0",
    "request_id": "uuid"
  }
}
```

#### API-Endpoints (nach Service)

**Store API (localhost:8000)**
```
GET  /api/products              List products with pagination
GET  /api/products/{id}         Get product details
POST /api/products              Create product (admin)
PUT  /api/products/{id}         Update product
DELETE /api/products/{id}        Delete product

GET  /api/orders                List user's orders
GET  /api/orders/{id}           Get order details
POST /api/orders                Create order
PUT  /api/orders/{id}/status    Update order status

GET  /api/categories            List categories
GET  /api/categories/{id}       Get category details
```

**Admin API (localhost:8080)**
```
GET  /api/admin/tenants         List tenants (super admin)
POST /api/admin/tenants         Create tenant
GET  /api/admin/users           List users
GET  /api/admin/settings        Get system settings
POST /api/admin/settings        Update settings
GET  /api/admin/audit-logs      Get audit trail
```

**Identity Service (Internal)**
```
POST /api/auth/login            User login
POST /api/auth/refresh          Refresh token
POST /api/auth/logout           User logout
POST /api/auth/register         User registration
POST /api/auth/password-reset   Password reset
```

---

### 3. **Database-Dokumentation**

#### Entity Relationship Diagram

```
Users (1) ──── (n) Orders
  │                 │
  │                 └──── (n) OrderItems
  │                       │
  │                       └──── (1) Products
  │
  ├──── (n) Tenants
  │
  └──── (n) Addresses

Products (1) ──── (n) Categories
  │
  ├──── (n) ProductImages
  │
  └──── (n) Reviews (by Users)
```

#### Core Tables

**Users**
```sql
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    Email VARCHAR(256) NOT NULL,  -- Encrypted (P0.3)
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100),  -- Encrypted (P0.3)
    LastName VARCHAR(100),   -- Encrypted (P0.3)
    PhoneNumber VARCHAR(20),  -- Encrypted (P0.3)
    CreatedAt TIMESTAMPTZ NOT NULL,
    CreatedBy UUID NOT NULL,
    ModifiedAt TIMESTAMPTZ NOT NULL,
    ModifiedBy UUID,
    DeletedAt TIMESTAMPTZ,  -- Soft delete (P0.4)
    DeletedBy UUID,
    IsDeleted BOOLEAN DEFAULT false,
    RowVersion BYTEA,  -- Concurrency token
    
    INDEX idx_tenant_email (TenantId, Email),
    INDEX idx_created_at (CreatedAt),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);
```

**Orders**
```sql
CREATE TABLE Orders (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    UserId UUID NOT NULL,
    OrderNumber VARCHAR(50) UNIQUE,
    Status VARCHAR(50),  -- Draft, Pending, Confirmed, Shipped, Delivered
    TotalAmount DECIMAL(18, 2),
    Currency VARCHAR(3),
    ShippingAddress VARCHAR(500),  -- Encrypted
    CreatedAt TIMESTAMPTZ NOT NULL,
    CreatedBy UUID NOT NULL,
    ModifiedAt TIMESTAMPTZ NOT NULL,
    
    INDEX idx_tenant_user (TenantId, UserId),
    INDEX idx_status (Status),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

**Products**
```sql
CREATE TABLE Products (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(18, 2),
    Sku VARCHAR(100),
    StockQuantity INT DEFAULT 0,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMPTZ NOT NULL,
    CreatedBy UUID,
    
    INDEX idx_tenant_sku (TenantId, Sku),
    INDEX idx_is_active (IsActive),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);
```

**AuditLogs (P0.4)**
```sql
CREATE TABLE AuditLogs (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    UserId UUID,
    EntityType VARCHAR(255),  -- "User", "Order", "Product"
    EntityId UUID NOT NULL,
    Action VARCHAR(50),  -- "Create", "Update", "Delete"
    OldValues JSONB,  -- Previous values
    NewValues JSONB,  -- New values
    CreatedAt TIMESTAMPTZ NOT NULL,
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    
    INDEX idx_tenant_entity (TenantId, EntityType, EntityId),
    INDEX idx_created_at (CreatedAt),
    INDEX idx_user (UserId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

---

### 4. **Authentication & Authorization**

#### JWT Token Structure

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-id-uuid",
    "email": "user@example.com",
    "role": "user|manager|admin",
    "x-tenant-id": "tenant-id-uuid",
    "permissions": ["read:products", "write:orders"],
    "iat": 1703688000,
    "exp": 1703691600,  -- 1 hour
    "jti": "token-id"
  },
  "signature": "..."
}
```

#### Refresh Token Flow

```
1. User logs in with credentials
2. Server returns: 
   {
     "accessToken": "jwt-expires-in-1h",
     "refreshToken": "jwt-expires-in-7days",
     "expiresIn": 3600
   }
3. Client stores both tokens
4. When accessToken expires:
   - Send refreshToken to POST /api/auth/refresh
   - Get new accessToken
5. If refreshToken expired:
   - User must login again
```

#### Role-Based Access Control (RBAC)

```csharp
[Authorize(Roles = "Administrator")]
[HttpDelete("/api/admin/users/{id}")]
public async Task<IActionResult> DeleteUser(Guid id)
{
    // Only admins can delete users
}

[Authorize]  // Any authenticated user
[HttpGet("/api/orders")]
public async Task<IActionResult> GetMyOrders()
{
    // User can only see own orders (enforced by query filter)
}
```

#### Tenant Isolation

```csharp
// In JWT Claims
{
  "x-tenant-id": "abc123def456"
}

// In Query Filter (automatically applied)
public static IQueryable<T> ApplyTenantFilter<T>(
    this IQueryable<T> query,
    string tenantId) where T : BaseEntity
{
    return query.Where(e => e.TenantId == tenantId);
}

// Usage
var users = await db.Users
    .ApplyTenantFilter(tenantId)
    .ToListAsync();
// Only returns users from the specified tenant
```

---

### 5. **Deployment-Dokumentation**

#### Environment Configuration

**Development**
```json
{
  "Database__Provider": "inmemory",
  "Jwt__Secret": "dev-secret-minimum-32-chars!!!",
  "Cors__AllowedOrigins": ["http://localhost:5174"],
  "Logging__LogLevel__Default": "Debug"
}
```

**Staging**
```json
{
  "Database__Provider": "postgresql",
  "Database__ConnectionString": "Host=db.staging;...",
  "Jwt__Secret": "${JWT_SECRET}",  -- From secrets manager
  "Cors__AllowedOrigins": ["https://staging.admin.B2X.com"],
  "Logging__LogLevel__Default": "Information"
}
```

**Production**
```json
{
  "Database__Provider": "postgresql",
  "Database__ConnectionString": "${DB_CONNECTION_STRING}",
  "Jwt__Secret": "${JWT_SECRET}",  -- Must be set
  "Cors__AllowedOrigins": ["https://admin.B2X.com"],
  "Logging__LogLevel__Default": "Error",
  "EnableDataEncryption": true,
  "EnableAuditLogging": true
}
```

#### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY backend/ .
RUN dotnet restore
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /src/bin/Release/net10.0/publish .
ENTRYPOINT ["dotnet", "B2X.Orchestration.dll"]

EXPOSE 8000 8080
HEALTHCHECK --interval=30s CMD curl -f http://localhost:8000/health
```

#### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: B2X-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: B2X-api
  template:
    metadata:
      labels:
        app: B2X-api
    spec:
      containers:
      - name: api
        image: B2X:latest
        ports:
        - containerPort: 8000
        env:
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: B2X-secrets
              key: jwt-secret
        - name: DATABASE_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: B2X-secrets
              key: db-connection
        livenessProbe:
          httpGet:
            path: /health
            port: 8000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8000
          initialDelaySeconds: 10
          periodSeconds: 5
```

---

### 6. **Testing-Dokumentation**

#### Unit Test Example

```csharp
[Fact]
public async Task CreateOrder_WithValidData_CreatesOrderSuccessfully()
{
    // Arrange
    var orderRepository = new Mock<IOrderRepository>();
    var userRepository = new Mock<IUserRepository>();
    var service = new OrderService(orderRepository.Object, userRepository.Object);

    var command = new CreateOrderCommand
    {
        UserId = Guid.NewGuid(),
        Items = new[] { new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1 } }
    };

    // Act
    var result = await service.CreateOrderAsync(command);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.OrderNumber.StartsWith("ORD-"));
    orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
}
```

#### Integration Test Example

```csharp
[Collection("Database collection")]
public class OrderIntegrationTests : IAsyncLifetime
{
    private readonly PostgresFixture _fixture = new();

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
    }

    [Fact]
    public async Task CreateOrder_SavesOrderToDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var user = new User { Email = "test@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var order = new Order 
        { 
            UserId = user.Id,
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending 
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Assert
        var savedOrder = await context.Orders.FirstAsync();
        Assert.Equal("ORD-001", savedOrder.OrderNumber);
    }
}
```

---

### 7. **Performance-Dokumentation**

#### Query Optimization

**❌ Bad: N+1 Problem**
```csharp
var orders = await db.Orders.ToListAsync();
foreach (var order in orders)
{
    var items = await db.OrderItems
        .Where(oi => oi.OrderId == order.Id)
        .ToListAsync();  // Query per order!
}
```

**✅ Good: Eager Loading**
```csharp
var orders = await db.Orders
    .Include(o => o.Items)
    .ThenInclude(oi => oi.Product)
    .ToListAsync();  // Single query
```

#### Caching Strategy

```csharp
// Cache product catalog
var products = await cache.GetOrCreateAsync(
    "products:all",
    async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
        return await db.Products
            .AsNoTracking()
            .ToListAsync();
    });
```

#### Database Indexing

```sql
-- Create indexes for frequent queries
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_orders_user_id ON Orders(UserId);
CREATE INDEX idx_orders_created_at ON Orders(CreatedAt DESC);
CREATE INDEX idx_tenant_user ON Users(TenantId, Email);  -- Composite

-- Monitor index usage
SELECT schemaname, tablename, indexname, idx_scan
FROM pg_stat_user_indexes
ORDER BY idx_scan DESC;
```

---

### 8. **Monitoring & Logging**

#### Structured Logging mit Serilog

```csharp
Log.ForContext<OrderService>()
   .Information("Creating order for user {UserId} in tenant {TenantId}", 
       userId, tenantId);

Log.ForContext("OrderId", orderId)
   .ForContext("Amount", amount)
   .Warning("Order amount exceeds limits");

Log.ForContext<OrderService>()
   .Error(ex, "Failed to create order for user {UserId}", userId);
```

#### Health Check Endpoints

```csharp
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Response
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "cache": "Healthy",
    "elasticsearch": "Degraded"
  }
}
```

---

### 9. **Troubleshooting Guide**

#### Häufige Fehler

**Issue: "Connection string not found"**
```
Cause: Umgebungsvariable nicht gesetzt
Solution: Set DATABASE_CONNECTION_STRING=...
```

**Issue: "JWT secret too short"**
```
Cause: Secret < 32 characters
Solution: Use minimum 32-character secret
```

**Issue: "Tenant ID mismatch"**
```
Cause: X-Tenant-Id nicht im JWT
Solution: Check JWT claims contain x-tenant-id
```

#### Debug-Befehle

```bash
# Check health
curl http://localhost:8000/health

# Test auth endpoint
curl -X POST http://localhost:8000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password"}'

# View logs
dotnet run | grep -i error

# Database query
psql -c "SELECT COUNT(*) FROM Users;"
```

---

### 10. **Best Practices**

#### Code Style

✅ **DO**
```csharp
public async Task<Result<OrderDto>> CreateOrderAsync(CreateOrderCommand command)
{
    if (command == null)
        throw new ArgumentNullException(nameof(command));
    
    var order = new Order { /* ... */ };
    await _repository.AddAsync(order);
    return Result.Success(MapToDto(order));
}
```

❌ **DON'T**
```csharp
public Order Create(dynamic command)
{
    var order = new Order { /* ... */ };
    _repository.Add(order);
    return order;
}
```

#### Async/Await Pattern

```csharp
// Always use async/await for I/O
public async Task<User> GetUserAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}

// Return Task, not void
public async Task ProcessOrderAsync(Order order)
{
    await _service.SendConfirmationAsync(order);
}
```

#### Dependency Injection

```csharp
// Register in startup
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Inject into class
public class OrderController
{
    public OrderController(IOrderService service)
    {
        _service = service;
    }
}
```

---

## 📖 Zusätzliche Ressourcen

- [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Vue.js 3 Guide](https://vuejs.org/)

---

**Dokument-Status:** ✅ Production Ready  
**Letzte Aktualisierung:** 27. Dezember 2025  
**Nächste Review:** Q1 2026
