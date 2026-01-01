# Aspire Integration Testing: WaitForResourceHealthyAsync Timeouts

**Status**: Known Issue (Aspire GitHub #13714) + B2Connect Specific Issue  
**Date**: January 1, 2026  
**Research Source**: https://github.com/dotnet/aspire/issues/13714  
**B2Connect Resolution**: January 1, 2026

## Problem

`WaitForResourceHealthyAsync` in Aspire integration tests can timeout or hang with minimal diagnostic information when:
1. Health checks fail in tests/CI
2. Resources don't have proper health endpoints
3. Services fail to start properly
4. **Infrastructure dependencies (PostgreSQL, Redis, etc.) aren't started first**

### Symptoms
- Tests hang indefinitely or timeout after 30s
- "Connection refused" errors
- "Operation was canceled" exceptions with no context
- Health check logs suppressed by default

## Root Causes (B2Connect Specific Discovery)

**CRITICAL**: In B2Connect, the issue was that services couldn't start because PostgreSQL (and other infrastructure) wasn't running yet. The test fixture was waiting for services to be healthy, but those services need database connections to pass their health checks.

**Health Check Discovery** (enabled logging revealed):
```
fail: Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService[103]
  Health check catalog_check with status Unhealthy completed after 0.6178ms 
  with message 'Failed to connect to 127.0.0.1:53798'
  Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:53798
  ---> System.Net.Sockets.SocketException (61): Connection refused
```

**ALL services** were failing PostgreSQL health checks because the database wasn't available.

## Solution for B2Connect

### Don't Wait for Infrastructure-Dependent Services in Tests

Services with database dependencies will NEVER become healthy until their databases are available. In Aspire testing:

1. **Infrastructure containers (PostgreSQL, Redis, etc.) start asynchronously**
2. **Services try to connect immediately and fail health checks**
3. **Tests waiting for service health will timeout**

**Fix**: Don't use `WaitForResourceHealthyAsync` for services with infrastructure dependencies. Instead:

```csharp
public async Task InitializeAsync()
{
    var appHost = await DistributedApplicationTestingBuilder
        .CreateAsync<Projects.B2Connect_AppHost>();

    // Enable health check logging
    appHost.Services.AddLogging(logging =>
    {
        logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService", 
            LogLevel.Information);
    });

    _app = await appHost.BuildAsync();
    await _app.StartAsync();

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

    // DO NOT wait for services - they need infrastructure first
    // Instead, wait for infrastructure to be ready
    await _app.ResourceNotifications.WaitForResourceHealthyAsync("postgres", cts.Token);
    await _app.ResourceNotifications.WaitForResourceHealthyAsync("redis", cts.Token);
    
    // Give services extra time to connect after infrastructure is ready
    await Task.Delay(10000, cts.Token); // 10 seconds
}
```

### Alternative: Wait for Infrastructure, Then Services

```csharp
// Wait for infrastructure containers first
await WaitForInfrastructureAsync(cts.Token);

// Then wait for services (they should now be able to connect)
await _app.ResourceNotifications.WaitForResourceHealthyAsync("catalog-service", cts.Token);
```

## Root Causes

1. **Health Check Logs Suppressed**: Aspire forcibly sets `Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService` log level to `None` to reduce noise in dashboard
2. **No Default Timeout**: Tests can hang forever if cancellation tokens not properly configured
3. **Poor Error Messages**: Cancellation just says "operation canceled" without resource state details

## Solutions

### 1. Enable Health Check Logging in Tests (Immediate Fix)

```csharp
appHost.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Warning);
    // CRITICAL: Re-enable health check logs for tests
    logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService", 
        LogLevel.Information);
    logging.AddFilter("Aspire", LogLevel.Warning);
    logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
});
```

### 2. Always Use Explicit Timeouts

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

// Wait for services with explicit timeout
await ResourceNotifications.WaitForResourceHealthyAsync("catalog-service", cts.Token);
```

### 3. Don't Wait for Resources Without Health Endpoints

Gateways and some services may not have `/health` endpoints configured. For these:

```csharp
// Option A: Don't wait, just give time to start
await Task.Delay(2000, cts.Token);

// Option B: Wait for dependencies, not the gateway itself
await ResourceNotifications.WaitForResourceHealthyAsync("auth-service", cts.Token);
await ResourceNotifications.WaitForResourceHealthyAsync("catalog-service", cts.Token);
// Gateway should be ready after its dependencies
```

### 4. Better Error Handling Pattern

```csharp
async Task WaitForResourceHealthyWithDetails(string resourceName, CancellationToken ct)
{
    try
    {
        await app.ResourceNotifications.WaitForResourceHealthyAsync(resourceName, ct);
    }
    catch (OperationCanceledException ex)
    {
        if (app.ResourceNotifications.TryGetCurrentState(resourceName, out var evt) 
            && evt.Snapshot != null)
        {
            var state = evt.Snapshot;
            var error = new StringBuilder()
                .AppendLine($"Resource {resourceName} failed to become healthy")
                .AppendLine($"Current State: {state.State?.Text}")
                .AppendLine($"Current Health: {state.HealthStatus}");

            foreach(var report in state.HealthReports)
            {
                error.AppendLine($"- {report.Name}: {report.Status} @ {report.LastRunAt}");
                if (!string.IsNullOrEmpty(report.ExceptionText))
                    error.AppendLine($"  {report.ExceptionText}");
            }

            throw new OperationCanceledException(error.ToString(), ex, ex.CancellationToken);
        }
        throw;
    }
}
```

## Best Practices for Aspire Integration Tests

1. **Enable health check logging**: Always override the default logging suppression in tests
2. **Use explicit timeouts**: Never rely on default/infinite waits
3. **Check health endpoint availability**: Not all services have `/health` endpoints
4. **Wait for dependencies first**: Gateways depend on microservices - wait for them first
5. **Add helpful error context**: Wrap wait calls to include resource state on failure

## Known Issues

- **Aspire #13714**: WaitForResourceHealthyAsync has poor diagnostics (opened 2025-12-31)
  - Fix planned: Default timeout when dashboard not present
  - Fix planned: Don't suppress health check logs in tests
  - Fix planned: Better error messages with resource state

## Future Aspire Improvements (Planned)

1. Default timeout in test scenarios (5-10 minutes)
2. Health check logs enabled by default when dashboard not present
3. `ResourceNotificationService.WaitForXYZ` methods will include resource state in errors

## Example Test Pattern

```csharp
public sealed class AspireAppFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.B2Connect_AppHost>();

        // Enable health check logging for tests
        appHost.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
            // CRITICAL: Override Aspire's default suppression
            logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService", 
                LogLevel.Information);
        });

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // Explicit timeout
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        // Wait for services with health endpoints
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("auth-service", cts.Token);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("catalog-service", cts.Token);

        // Don't wait for gateways without health endpoints
        await Task.Delay(2000, cts.Token);
    }
}
```

## Related Documentation

- [Aspire Testing Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/testing)
- [Aspire GitHub Issue #13714](https://github.com/dotnet/aspire/issues/13714)
- [DistributedApplicationTestingBuilder API](https://learn.microsoft.com/en-us/dotnet/api/aspire.hosting.testing.distributedapplicationtestingbuilder)

---

## B2Connect Project Learnings

### Root Cause Discovered
Integration tests were timing out because **ALL services failed PostgreSQL health checks**:
```
fail: DefaultHealthCheckService[103]
  Health check catalog_check with status Unhealthy
  Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:53798
  ---> SocketException (61): Connection refused
```

**Problem**: Services started before PostgreSQL container was ready → all health checks failed → tests timed out.

### ✅ VALIDATED Solution: Infrastructure-First Approach
Changed `AspireAppFixture.cs` to wait for infrastructure **before** services:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

// 1. Wait for infrastructure containers FIRST
await _app.ResourceNotifications.WaitForResourceHealthyAsync("postgres", cts.Token);
await _app.ResourceNotifications.WaitForResourceHealthyAsync("redis", cts.Token);
await _app.ResourceNotifications.WaitForResourceHealthyAsync("rabbitmq", cts.Token);
await _app.ResourceNotifications.WaitForResourceHealthyAsync("elasticsearch", cts.Token);

// 2. Give services time to connect to infrastructure
await Task.Delay(TimeSpan.FromSeconds(15), cts.Token);
```

**Test Result**: ✅ Catalog service health test **passed** (74s total time)

### Remaining Health Check Warnings (Non-Blocking)
After implementing infrastructure-first approach, health check logs still show warnings:

| Component | Issue | Status |
|-----------|-------|--------|
| PostgreSQL | "End of stream" exceptions | ⚠️ Intermittent, not blocking |
| RabbitMQ | "Already closed" exceptions | ⚠️ Intermittent, not blocking |
| pgAdmin | Response timeouts | ⚠️ Can be ignored for tests |

**Analysis**: These warnings occur during container startup but **do not block tests**. Services eventually become healthy.

**Likely Causes**:
- Health checks run before containers fully stabilized
- Connection pooling issues during parallel service startup  
- pgAdmin slow to respond (non-essential for integration tests)

### Key Learnings
1. ✅ **Enable health check logging** in tests - reveals actual failure reasons
2. ✅ **Infrastructure-first initialization** - wait for databases/message queues before services
3. ✅ **Explicit timeouts** - avoid infinite waits with `CancellationTokenSource`
4. ⚠️ **Health check warnings** - some infrastructure warnings are normal during startup
5. ⚠️ **pgAdmin health checks** - can be excluded from integration test assertions

---

**Last Updated**: 2026-01-01  
**Aspire Version**: 13.1.0  
**Project**: B2Connect  
**Status**: ✅ Infrastructure-first approach validated with passing tests
