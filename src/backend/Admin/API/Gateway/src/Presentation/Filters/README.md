# Admin API Filters

Diese Dokumentation beschreibt die Attribute Filter, die in der Admin API zur Zentralisierung von Cross-Cutting-Concerns verwendet werden.

## 📁 Dateistruktur

```
Presentation/
├── Filters/
│   ├── ValidateTenantAttribute.cs        # Tenant-ID Validierung
│   ├── ApiExceptionHandlingFilter.cs     # Exception Handling
│   ├── ValidateModelStateFilter.cs       # Model Validation
│   └── ApiLoggingFilter.cs               # Request/Response Logging
│
├── Controllers/
│   ├── ApiControllerBase.cs              # Base Controller mit Helpers
│   ├── UsersController.cs                # Refaktoriert ✅
│   ├── ProductsController.cs             # Refaktoriert ✅
│   ├── CategoriesController.cs           # Refaktoriert ✅
│   └── BrandsController.cs               # Refaktoriert ✅
│
└── Program.cs                            # Filter-Registrierung
```

## 🔧 Filter-Übersicht

### ValidateTenantAttribute
- **Typ**: Action Filter (Attribute)
- **Anwendungsbereich**: Per-Controller via `[ValidateTenant]`
- **Verantwortung**: Extraktion und Validierung der `X-Tenant-ID` Header
- **Response bei Fehler**: 401 Unauthorized

### ApiExceptionHandlingFilter
- **Typ**: Exception Filter
- **Anwendungsbereich**: Global (in `Program.cs` registriert)
- **Verantwortung**: Zentralisierte Exception-Behandlung
- **Features**: Exception-Typ-Mapping, Stack Trace in Development

### ValidateModelStateFilter
- **Typ**: Action Filter
- **Anwendungsbereich**: Global (in `Program.cs` registriert)
- **Verantwortung**: Automatische Model State Validierung
- **Response bei Fehler**: 400 Bad Request mit Validierungsfehlern

### ApiLoggingFilter
- **Typ**: Action Filter
- **Anwendungsbereich**: Global (in `Program.cs` registriert)
- **Verantwortung**: Logging von Requests, Responses und Performance
- **Features**: Performance-Warnung für langsame Requests (>1000ms)

## 🚀 Verwendung

### Schritt 1: Controller von ApiControllerBase erben
```csharp
public class ProductsController : ApiControllerBase
{
    // ...
}
```

### Schritt 2: ValidateTenant Attribute anwenden
```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateTenant]
public class ProductsController : ApiControllerBase
{
    // ...
}
```

### Schritt 3: Response-Helper nutzen
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
{
    var tenantId = GetTenantId();
    var product = await _service.GetProductAsync(id);
    
    if (product == null)
        return NotFoundResponse($"Product {id} not found");
    
    return OkResponse(product);
}
```

## 🔐 Tenant-Isolation

Die `ValidateTenantAttribute` sorgt dafür, dass jeder Request eine gültige `X-Tenant-ID` enthält:

```bash
# ✅ Gültig
curl -H "X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000" http://localhost:8080/api/products

# ❌ Ungültig - kein Header
curl http://localhost:8080/api/products
# → 401 Unauthorized: Missing X-Tenant-ID header

# ❌ Ungültig - falsches Format
curl -H "X-Tenant-ID: invalid-guid" http://localhost:8080/api/products
# → 401 Unauthorized: Invalid X-Tenant-ID format (must be GUID)
```

## 📊 Exception Handling

Alle Exceptions werden automatisch zu standardisierten HTTP-Responses gemappt:

| Exception Type | HTTP Status | Error Code |
|---|---|---|
| `ArgumentNullException` | 400 | `VALIDATION_ERROR` |
| `ArgumentException` | 400 | `VALIDATION_ERROR` |
| `KeyNotFoundException` | 404 | `NOT_FOUND` |
| `InvalidOperationException` | 409 | `CONFLICT` |
| `UnauthorizedAccessException` | 403 | `FORBIDDEN` |
| `TimeoutException` | 504 | `TIMEOUT` |
| `HttpRequestException` | 502 | `SERVICE_ERROR` |
| Alle anderen | 500 | `INTERNAL_ERROR` |

## 💾 Logging-Ausgabe

### Request Logging
```
Inbound Request: GET /api/products/123 | TenantId: 550e8400-e29b-41d4-a716-446655440000 | User: admin
```

### Response Logging
```
Outbound Response: GET /api/products/123 | Status: 200 | Duration: 45ms | TenantId: 550e8400-e29b-41d4-a716-446655440000
```

### Slow Request Warning
```
Slow Request: POST /api/products took 1523ms
```

### Server Error Logging
```
Server Error: DELETE /api/products/123 returned 500
```

## 🧪 Testing

Bei Unit Tests müssen die Filter manuell gepuffert werden:

```csharp
[Fact]
public async Task CreateProduct_WithValidInput_ReturnsCreated()
{
    // Mock HttpContext für GetTenantId()
    var mockHttpContext = new Mock<HttpContext>();
    var items = new Dictionary<object, object?>
    {
        { "TenantId", Guid.NewGuid() }
    };
    mockHttpContext.Setup(x => x.Items).Returns(items);
    
    var controller = new ProductsController(_mockService, _mockLogger)
    {
        ControllerContext = new ControllerContext 
        { 
            HttpContext = mockHttpContext.Object 
        }
    };
    
    // Act & Assert
    var result = await controller.CreateProduct(new CreateProductDto { Name = "Test" });
    Assert.IsType<CreatedAtActionResult>(result.Result);
}
```

## 📖 API Response Format

Alle erfolgreichen Responses folgen diesem Format:

```json
{
  "success": true,
  "data": { ... },
  "message": "Optional success message",
  "timestamp": "2025-12-27T10:00:00Z"
}
```

Alle Error-Responses folgen diesem Format:

```json
{
  "success": false,
  "error": "Error description",
  "errorCode": "ERROR_CODE",
  "timestamp": "2025-12-27T10:00:00Z",
  "traceId": "0HN1GBCD5KICB:00000001",
  "stackTrace": "..."  // Nur in Development!
}
```

## 🎯 Best Practices

1. **Immer ApiControllerBase erben**
   ```csharp
   public class MyController : ApiControllerBase { }  // ✅
   public class MyController : ControllerBase { }      // ❌
   ```

2. **ValidateTenant Attribute verwenden**
   ```csharp
   [ValidateTenant]  // ✅ Zentrale Validierung
   public class MyController : ApiControllerBase { }
   ```

3. **Keine try-catch in Controller**
   ```csharp
   var entity = await _service.Create(dto);  // ✅ Exceptions werden automatisch gehandhabt
   
   try                                         // ❌ Nicht nötig
   {
       // ...
   }
   catch { }
   ```

4. **Response-Helper nutzen**
   ```csharp
   return OkResponse(product);                         // ✅
   return Ok(new { success = true, data = product }); // ❌
   ```

5. **Tenant-Context in Logs**
   ```csharp
   var tenantId = GetTenantId();
   _logger.LogInformation("Creating product for tenant {TenantId}", tenantId);  // ✅
   ```

---

**Version**: 1.0  
**Letztes Update**: 27. Dezember 2025
