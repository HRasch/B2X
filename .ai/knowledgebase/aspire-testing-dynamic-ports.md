# Aspire Testing with Dynamic Ports

**Created**: 2026-01-01  
**Owner**: @Backend, @DevOps  
**Status**: ✅ Resolved

## Problem

.NET Aspire assigns **dynamic ports** to internal services at runtime. This causes issues when:
- Writing integration tests that need to connect to services
- Running Playwright E2E tests that need API gateway URLs
- Port discovery via `lsof` or process inspection is unreliable

## Solution: `DistributedApplicationTestingBuilder`

The official Aspire solution uses the `Aspire.Hosting.Testing` NuGet package:

```csharp
// Create test app from AppHost
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.B2Connect_AppHost>();

await using var app = await appHost.BuildAsync();
await app.StartAsync();

// Get HttpClient for any resource - Aspire resolves the dynamic port automatically!
using var httpClient = app.CreateHttpClient("store-gateway");
```

### Key APIs

| Method | Purpose |
|--------|---------|
| `app.CreateHttpClient("resource-name")` | Get HttpClient with correct base URL (dynamic port resolved) |
| `app.GetEndpoint("resource-name", "http")` | Get the endpoint URL directly |
| `ResourceNotifications.WaitForResourceHealthyAsync()` | Wait for service to be healthy before testing |

## B2Connect Port Configuration

Our AppHost **fixes ports** for frontend-facing resources:

| Resource | Port | Configuration |
|----------|------|---------------|
| `store-gateway` | 8000 | `WithHttpEndpoint(port: 8000, name: "store-http")` |
| `admin-gateway` | 8080 | `WithHttpEndpoint(port: 8080, name: "admin-http")` |
| `frontend-store` | 5173 | `WithEndpoint("http", e => e.Port = 5173)` |
| `frontend-admin` | 5174 | `WithEndpoint("http", e => e.Port = 5174)` |

**Internal services** (auth-service, catalog-service, etc.) use **dynamic ports** managed by Aspire Service Discovery.

## Integration Test Setup

### Project Reference
```xml
<!-- B2Connect.Integration.Tests.csproj -->
<PackageReference Include="Aspire.Hosting.Testing" />
<ProjectReference Include="../../../AppHost/B2Connect.AppHost.csproj" />
```

### Test Fixture Pattern
```csharp
public sealed class AspireAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public HttpClient StoreGatewayClient => _app!.CreateHttpClient("store-gateway");
    
    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.B2Connect_AppHost>();
        
        _app = await appHost.BuildAsync();
        await _app.StartAsync();
        
        // Wait for services to be healthy
        var notifications = _app.Services.GetRequiredService<ResourceNotificationService>();
        await notifications.WaitForResourceHealthyAsync("store-gateway");
    }
}
```

## Playwright E2E Tests

Since gateways and frontends have **fixed ports**, Playwright configs can use static URLs:

```typescript
// playwright.config.ts
export default defineConfig({
  use: {
    baseURL: process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:5173',
  },
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
  },
});
```

### Environment Variables (optional)
```bash
export PLAYWRIGHT_STORE_URL="http://localhost:5173"
export PLAYWRIGHT_ADMIN_URL="http://localhost:5174"
export PLAYWRIGHT_STORE_GATEWAY_URL="http://localhost:8000"
export PLAYWRIGHT_ADMIN_GATEWAY_URL="http://localhost:8080"
```

## Related Files

- [AppHost/Program.cs](../../AppHost/Program.cs) - Port configuration
- [AspireAppFixture.cs](../../backend/Tests/B2Connect.Integration.Tests/AspireAppFixture.cs) - Test fixture
- [scripts/aspire-test-env.sh](../../scripts/aspire-test-env.sh) - Environment setup script

## References

- [Official Aspire Testing Docs](https://learn.microsoft.com/en-us/dotnet/aspire/testing)
- [GitHub Discussion #878](https://github.com/dotnet/aspire/discussions/878) - Integration testing patterns
- [aspire-samples TestFixtures](https://github.com/dotnet/aspire-samples) - Example implementations
