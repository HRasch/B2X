# Testing Configuration Setup Guide

## Overview

The B2Connect AppHost now supports configurable testing environments that can operate in either **persisted** (PostgreSQL) or **temporary** (in-memory) modes. This allows for flexible testing strategies across different environments.

## Configuration Structure

### Testing Section

```json
{
  "Testing": {
    "Mode": "temporary",           // "temporary" | "persisted"
    "Environment": "development",  // "development" | "testing" | "ci"
    "SeedOnStartup": false,        // true | false
    "SeedData": {
      "DefaultTenantCount": 1,
      "UsersPerTenant": 5,
      "SampleProductCount": 50,
      "IncludeCatalogDemo": true,
      "IncludeCmsContent": false
    },
    "Security": {
      "EnforceEnvironmentGating": true,
      "EnableAuditLogging": true,
      "EnforceRoleBasedAccess": true,
      "AllowCrossTenantAccess": false
    }
  }
}
```

## Configuration Files

### Development Environment (`appsettings.Development.json`)
- **Mode**: `temporary` (fast in-memory for development)
- **Environment**: `development`
- **SeedOnStartup**: `false` (manual seeding preferred)
- **SeedData**: Minimal data for quick startup

### Testing Environment (`appsettings.Testing.json`)
- **Mode**: `persisted` (PostgreSQL for realistic testing)
- **Environment**: `testing`
- **SeedOnStartup**: `true` (automatic seeding for test consistency)
- **SeedData**: Full demo data including CMS content

### CI Environment (`appsettings.CI.json`)
- **Mode**: `temporary` (fast in-memory for CI speed)
- **Environment**: `ci`
- **SeedOnStartup**: `false` (no seeding in CI)
- **SeedData**: Minimal data for unit tests

## Usage in Services

### Conditional DbContext Registration

Each service should configure its DbContext conditionally based on the testing mode:

```csharp
// In your service's Program.cs
using B2Connect.AppHost.Configuration;
using B2Connect.AppHost.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load testing configuration
var testingConfig = builder.Configuration.GetSection("Testing").Get<TestingConfiguration>()
    ?? new TestingConfiguration();

// Configure DbContext conditionally
builder.Services.ConfigureConditionalDbContext<MyDbContext>(
    testingConfig,
    connectionString: builder.Configuration.GetConnectionString("MyDb"),
    configurePostgres: (sp, options) =>
    {
        options.UseNpgsql(connectionString);
    },
    configureInMemory: (sp, options) =>
    {
        options.UseInMemoryDatabase("MyServiceDb");
    });

// Log configuration for debugging
builder.Services.AddSingleton(testingConfig);
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

var app = builder.Build();

// Log testing configuration
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogTestingConfiguration("MyService", testingConfig);
```

### Conditional Infrastructure

For Redis, RabbitMQ, and other infrastructure:

```csharp
// Redis configuration
builder.Services.ConfigureConditionalRedis(
    testingConfig,
    redisConnectionString: builder.Configuration.GetConnectionString("Redis"),
    configureRedisCache: cache =>
    {
        // Configure Redis-specific settings
    },
    configureMemoryCache: cache =>
    {
        // Configure in-memory cache settings
    });
```

## Validation

Configuration is automatically validated on startup. Invalid configurations will throw `InvalidOperationException` with detailed error messages.

### Validation Rules

- **Mode**: Must be "temporary" or "persisted"
- **Environment**: Must be "development", "testing", or "ci"
- **SeedData**: All counts must be >= 0
- **Consistency**: Warnings for illogical combinations (e.g., SeedOnStartup=true with DefaultTenantCount=0)

## Testing the Configuration

### Unit Tests

Run the configuration tests:

```bash
dotnet test AppHost.Tests --filter TestingConfigurationTests
```

### Integration Testing

1. Start with temporary mode:
   ```json
   { "Testing": { "Mode": "temporary" } }
   ```

2. Verify in-memory databases are used (check logs)

3. Switch to persisted mode:
   ```json
   { "Testing": { "Mode": "persisted" } }
   ```

4. Verify PostgreSQL connections are used

### Environment Switching

To switch between modes:

1. Update the `Testing:Mode` in the appropriate appsettings file
2. Restart the AppHost
3. Verify the change in application logs

## Troubleshooting

### Common Issues

1. **"Invalid Testing:Mode" error**
   - Check spelling: must be exactly "temporary" or "persisted"
   - Case-insensitive but must match exactly

2. **"Invalid Testing:Environment" error**
   - Must be "development", "testing", or "ci"

3. **Services not using correct database**
   - Verify each service has the conditional configuration code
   - Check service logs for testing configuration messages

4. **Configuration not loading**
   - Ensure appsettings files are in the correct location
   - Check ASPNETCORE_ENVIRONMENT variable

### Debugging

Enable debug logging to see configuration details:

```json
{
  "Logging": {
    "LogLevel": {
      "B2Connect.AppHost": "Debug"
    }
  }
}
```

Look for log messages like:
```
Service 'MyService' configured for temporary testing in development environment. Seed on startup: false, Default tenants: 1
```

## Migration Guide

### For Existing Services

1. Add the conditional DbContext configuration code to Program.cs
2. Update appsettings files with Testing section
3. Test both modes (temporary and persisted)
4. Update service documentation

### For New Services

1. Use the conditional configuration pattern from the start
2. Include Testing configuration in all environment files
3. Add unit tests for configuration validation

## Security Considerations

- **Environment Gating**: Test endpoints are automatically gated by environment
- **Audit Logging**: All test operations can be audited
- **RBAC**: Test tenant management requires SuperAdmin role
- **Data Isolation**: Test data is marked and can be excluded from backups

## Performance Notes

- **Temporary Mode**: Fast startup, no persistence, ideal for unit tests
- **Persisted Mode**: Slower startup, full persistence, ideal for integration tests
- **CI Environment**: Optimized for speed with minimal data
- **Development**: Balanced for developer productivity

## Next Steps

After configuration is complete:

1. **BE-002**: Implement seeding infrastructure
2. **BE-003**: Create test tenant management API
3. **FE-001**: Build frontend components
4. **Integration Testing**: Test end-to-end functionality

---

**Last Updated**: 2026-01-07
**Version**: 1.0
**Owner**: @Backend Team
