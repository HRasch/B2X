# Deployment Guide: Price Calculation & VAT Validation

**Issues**: #30, #31  
**Services Affected**: Catalog Service (7005)  
**Database**: PostgreSQL (vat_id_validations table)  
**Cache**: Redis (IDistributedCache)  
**Status**: Production Ready

---

## Prerequisites

### System Requirements
- .NET 10.0 runtime
- PostgreSQL 16+
- Redis 7.0+ (for caching)
- 512MB RAM minimum (per service)
- 100MB disk (for codebase)

### Software Dependencies
```xml
<!-- Already in Directory.Packages.props -->
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="10.0.0" />
```

### Network Requirements
- Outbound HTTPS to: `https://ec.europa.eu/taxation_customs/vies/` (VIES API)
- Inbound HTTP on port 7005 (Catalog service)
- Redis connectivity on port 6379 (default)

---

## Dependency Injection Configuration

Add to `backend/Domain/Catalog/Program.cs`:

```csharp
// Register VAT validation services (Issue #31)
services.AddScoped<IViesApiClient, ViesApiClient>();
services.AddScoped<IVatIdValidationService, VatIdValidationService>();
services.AddScoped<ValidateVatIdRequestValidator>();

// Register price calculation service (Issue #30 - already done)
services.AddScoped<IPriceCalculationService, PriceCalculationService>();
services.AddScoped<ValidateVatIdRequestValidator>();

// HTTP Client for VIES API
services.AddHttpClient<ViesApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("User-Agent", "B2X/1.0");
    });

// Distributed cache (Redis)
services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = configuration.GetConnectionString("Redis") 
        ?? "localhost:6379";
    options.Configuration = redisConnection;
});

// Validation
services.AddValidatorsFromAssemblyContaining<ValidateVatIdRequestValidator>();
```

---

## Environment Variables

Create `.env` file in catalog service:

```bash
# VIES API Configuration
VIES_API_TIMEOUT_SECONDS=10
VIES_API_RETRY_COUNT=3
VIES_API_RETRY_BACKOFF_BASE=2  # Exponential backoff: 2^n

# VAT Caching
VAT_CACHE_TTL_VALID_DAYS=365      # Valid VAT IDs cached 365 days
VAT_CACHE_TTL_INVALID_HOURS=24     # Invalid VAT IDs cached 24 hours

# Database
DatabaseProvider=PostgreSQL
DATABASE_CONNECTION_STRING=Host=postgres;Database=B2X_catalog;Username=postgres;Password=***

# Redis
REDIS_CONNECTION_STRING=redis:6379

# Logging
LOG_LEVEL=Information
```

---

## Database Migration

### Step 1: Create Migration

```bash
cd backend/Domain/Catalog/src

# Generate migration for VatIdValidationCache entity
dotnet ef migrations add AddVatIdValidationCache \
  --project B2X.Catalog.API.csproj \
  --output-dir Infrastructure/Migrations

# Review the generated migration
cat Infrastructure/Migrations/*_AddVatIdValidationCache.cs
```

### Step 2: Apply Migration

```bash
# Update database
dotnet ef database update \
  --project B2X.Catalog.API.csproj

# Verify table created
psql -h postgres -U postgres -d B2X_catalog \
  -c "\dt vat_id_validations"
```

### Step 3: Verify Schema

```sql
-- Check table structure
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'vat_id_validations';

-- Check indexes
SELECT indexname FROM pg_indexes 
WHERE tablename = 'vat_id_validations';

-- Expected output:
-- id (UUID) - Primary key
-- tenant_id (UUID) - Multi-tenant isolation
-- country_code (VARCHAR(2))
-- vat_number (VARCHAR(17))
-- is_valid (BOOLEAN)
-- company_name (VARCHAR(255))
-- company_address (VARCHAR(512))
-- validated_at (TIMESTAMP)
-- expires_at (TIMESTAMP)
-- is_deleted (BOOLEAN)
-- deleted_at (TIMESTAMP)
-- Indexes: UNIQUE(tenant_id, country_code, vat_number), idx_expires_at
```

---

## Docker Deployment

### Build Image

```dockerfile
# backend/Domain/Catalog/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 7005

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["backend/Domain/Catalog/B2X.Catalog.API.csproj", "Catalog/"]
RUN dotnet restore "Catalog/B2X.Catalog.API.csproj"
COPY . .
RUN dotnet build "backend/Domain/Catalog/B2X.Catalog.API.csproj" -c Release

FROM build AS publish
RUN dotnet publish "backend/Domain/Catalog/B2X.Catalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:7005
ENTRYPOINT ["dotnet", "B2X.Catalog.API.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  catalog:
    build:
      context: .
      dockerfile: backend/Domain/Catalog/Dockerfile
    ports:
      - "7005:7005"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DatabaseProvider=PostgreSQL
      - DATABASE_CONNECTION_STRING=Host=postgres;Database=B2X_catalog;Username=postgres;Password=postgres
      - REDIS_CONNECTION_STRING=redis:6379
      - VIES_API_TIMEOUT_SECONDS=10
    depends_on:
      - postgres
      - redis
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:7005/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=B2X_catalog
      - POSTGRES_PASSWORD=postgres
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

volumes:
  postgres_data:
  redis_data:
```

### Start Services

```bash
# Build and start all services
docker-compose up -d

# Check logs
docker-compose logs -f catalog

# Verify service health
curl http://localhost:7005/health

# Stop services
docker-compose down
```

---

## Verification Steps

### 1. Service Startup

```bash
# Check service is running
curl -I http://localhost:7005/health

# Expected response:
# HTTP/1.1 200 OK
```

### 2. Endpoint Registration

```bash
# Verify endpoints are discoverable (Wolverine auto-discovery)
curl http://localhost:7005/api/endpoints 2>/dev/null | jq .

# Should include:
# - POST /calculateprice
# - POST /validatevatid
```

### 3. Price Calculation Test

```bash
curl -X POST http://localhost:7005/calculateprice \
  -H "Content-Type: application/json" \
  -d '{"basePrice": 100.00, "destinationCountry": "DE"}' \
  | jq .

# Expected response:
# {
#   "destinationCountry": "DE",
#   "basePrice": 100.00,
#   "vatRate": 19.0,
#   "vatAmount": 19.00,
#   "priceIncludingVat": 119.00,
#   "finalPrice": 119.00,
#   "currencyCode": "EUR"
# }
```

### 4. VAT Validation Test

```bash
curl -X POST http://localhost:7005/validatevatid \
  -H "Content-Type: application/json" \
  -d '{
    "countryCode": "DE",
    "vatNumber": "123456789",
    "buyerCountry": "AT",
    "sellerCountry": "DE"
  }' | jq .

# Expected response:
# {
#   "isValid": true/false,
#   "vatId": "DE123456789",
#   "companyName": "Company Name" (if valid),
#   "reverseChargeApplies": true/false,
#   "message": "...",
#   "expiresAt": "2026-12-29T..."
# }
```

### 5. Cache Verification

```bash
# Check Redis cache
redis-cli KEYS "vat:*"

# Expected output (after first VAT validation):
# vat:DE:123456789

# Check cache TTL
redis-cli TTL "vat:DE:123456789"

# Expected: ~31,536,000 seconds (365 days)
```

### 6. Database Verification

```bash
# Check vat_id_validations table
psql -h postgres -U postgres -d B2X_catalog \
  -c "SELECT COUNT(*) FROM vat_id_validations;"

# Expected: >0 (after first validation)

# List recent validations
psql -h postgres -U postgres -d B2X_catalog \
  -c "SELECT vat_id, is_valid, company_name, validated_at 
      FROM vat_id_validations 
      ORDER BY validated_at DESC 
      LIMIT 5;"
```

---

## Monitoring & Logging

### Log Levels

- **DEBUG**: VIES API request/response details
- **INFO**: Validation success, cache hits/misses
- **WARNING**: API retries, cache expiration
- **ERROR**: API failures, validation errors

### Key Log Points

```csharp
// Price calculation
_logger.LogInformation("Calculating price: {BasePrice} for {Country}", basePrice, country);
_logger.LogWarning("Country {Country} not found, using default VAT rate", country);

// VAT validation
_logger.LogInformation("Starting VAT validation: {CountryCode}{VatNumber}", code, number);
_logger.LogWarning("VIES API call failed (attempt {Attempt}/{Max})", attempt, maxRetries);
_logger.LogError("VIES API failed after {Max} retries", maxRetries);

// Caching
_logger.LogInformation("VAT validation cache hit: {Key}", cacheKey);
_logger.LogInformation("VAT validation cache miss, calling VIES API");
_logger.LogInformation("Result cached for {Days} days", ttlDays);
```

### Alerts to Configure

1. **VIES API Unavailable** (ERROR logs)
2. **Cache Failure** (ERROR logs)
3. **Database Connection Loss** (ERROR logs)
4. **Service Health Check Failure** (HTTP 503)

---

## Rollback Procedure

If issues occur after deployment:

### Option 1: Revert Code

```bash
# Find working commit
git log --oneline | grep "feat(catalog)"

# Revert to previous version
git revert HEAD

# Rebuild and redeploy
dotnet build && docker-compose up -d --build
```

### Option 2: Disable VAT Validation

If VIES API is the issue:

```csharp
// In VatIdValidationService.cs
public async Task<VatValidationResult> ValidateVatIdAsync(...)
{
    // Temporarily disable VIES calls
    return new VatValidationResult
    {
        IsValid = false,  // Always invalid until API recovers
        VatId = $"{countryCode}{vatNumber}",
        ValidatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddHours(1)
    };
}
```

### Option 3: Database Rollback

```bash
# Revert migration (if database issue)
dotnet ef database update <previous-migration-name>

# Example:
dotnet ef database update 20251228_InitialCatalogSchema
```

---

## Post-Deployment Checklist

- [ ] Service started successfully (logs show no errors)
- [ ] Health check endpoint responds (200 OK)
- [ ] POST /calculateprice works with test data
- [ ] POST /validatevatid works with test VAT IDs
- [ ] Redis cache populated after first validation
- [ ] Database table created with correct schema
- [ ] Logging configured and working
- [ ] VIES API connectivity verified
- [ ] Performance baseline captured (cache hit <5ms, miss <2s)
- [ ] Monitoring/alerts configured
- [ ] Documentation updated with deployment info

---

**Deployment Status**: ✅ Ready for Production  
**Last Updated**: 29 December 2025  
**Supported Versions**: .NET 10.0, PostgreSQL 16+, Redis 7.0+
