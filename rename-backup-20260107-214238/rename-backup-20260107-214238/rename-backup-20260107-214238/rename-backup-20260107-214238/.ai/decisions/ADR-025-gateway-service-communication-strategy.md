# ADR-025: Gateway-Service Communication Strategy

**Status**: Accepted  
**Date**: 2. Januar 2026  
**Owner**: @Architect, @DevOps  
**DocID**: `ADR-025`

---

## Context

The B2X architecture uses API Gateways (Store, Admin) that communicate with domain services (Identity, Catalog, Theming, etc.). Currently, we face recurring issues:

1. **Port conflicts** - Services fail to start due to occupied ports
2. **Configuration mismatches** - Hardcoded URLs don't match actual service addresses
3. **Silent failures** - Communication errors are not detected quickly
4. **Inconsistent health checks** - Different implementations across services

### Current Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         .NET Aspire AppHost                         │
│                        (Service Orchestrator)                       │
└─────────────────────────────────────────────────────────────────────┘
                                    │
        ┌───────────────────────────┼───────────────────────────┐
        │                           │                           │
        ▼                           ▼                           ▼
┌───────────────┐          ┌───────────────┐          ┌───────────────┐
│ Infrastructure│          │    Gateways   │          │Domain Services│
│    (Fixed)    │          │    (Fixed)    │          │   (Dynamic)   │
├───────────────┤          ├───────────────┤          ├───────────────┤
│ PostgreSQL:5432│          │ Store:8000    │          │ auth-service  │
│ Redis:6379    │          │ Admin:8080    │          │ tenant-service│
│ Elastic:9200  │          │               │          │ catalog-svc   │
│ RabbitMQ:5672 │          │               │          │ theming-svc   │
│ Dashboard:15500│         │               │          │ etc...        │
└───────────────┘          └───────────────┘          └───────────────┘
```

---

## Decision

### 1. Service Discovery via Aspire (No Hardcoded URLs)

**Rule**: All gateway-to-service communication MUST use Aspire service names, NOT URLs.

```csharp
// ✅ CORRECT - Service name resolved by Aspire
"Destinations": {
  "identity": { "Address": "http://auth-service" }
}

// ❌ WRONG - Hardcoded URL
"Destinations": {
  "identity": { "Address": "http://localhost:7002" }
}
```

### 2. Port Assignment Strategy

| Layer | Port Range | Assignment | Example |
|-------|------------|------------|---------|
| **Infrastructure** | 5000-5999, 6379, 9200 | Fixed | PostgreSQL:5432, Redis:6379 |
| **Gateways** | 8000-8099 | Fixed | Store:8000, Admin:8080 |
| **Frontends** | 5173-5199 | Fixed | Store:5173, Admin:5174 |
| **Domain Services** | Dynamic | Aspire-managed | Auto-assigned |
| **Aspire Dashboard** | 15500 | Fixed | Dashboard:15500 |

**Why Dynamic for Domain Services?**
- Aspire handles port conflicts automatically
- Service discovery resolves by name
- Scales better in container environments

### 3. Unified Health Check Implementation

All services MUST implement ASP.NET Core health checks consistently:

```csharp
// In Program.cs or ServiceDefaults
services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
    .AddNpgSql(connectionString, name: "database", tags: ["ready"])
    .AddRedis(redisConnection, name: "redis", tags: ["ready"])
    .AddElasticsearch(elasticUri, name: "elasticsearch", tags: ["ready"]);

// Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new() { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new() { Predicate = r => r.Tags.Contains("ready") });
```

**Health Check Endpoints**:
- `/health` - Full health (all checks)
- `/health/live` - Liveness probe (is process running?)
- `/health/ready` - Readiness probe (can handle requests?)

### 4. Gateway Communication Patterns

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │  YARP Proxy  │    │ Service      │    │  Direct      │      │
│  │  (Routing)   │    │ Clients      │    │  HTTP        │      │
│  │              │    │ (Typed)      │    │  (Rare)      │      │
│  └──────┬───────┘    └──────┬───────┘    └──────┬───────┘      │
│         │                   │                   │               │
└─────────┼───────────────────┼───────────────────┼───────────────┘
          │                   │                   │
          ▼                   ▼                   ▼
    ┌───────────┐      ┌───────────┐      ┌───────────┐
    │ auth-svc  │      │catalog-svc│      │ Other     │
    └───────────┘      └───────────┘      └───────────┘
```

**When to use which**:
- **YARP Proxy**: Pass-through requests (auth, static content, large payloads)
- **Service Clients**: Aggregation, transformation, business logic
- **Direct HTTP**: Avoid - use typed clients instead

---

## Implementation

### Phase 1: Standardize Health Checks (Priority: HIGH)

1. Update `ServiceDefaults/Extensions.cs` with comprehensive health checks
2. Remove all manual `/health` endpoint implementations
3. Add infrastructure health checks (database, Redis, Elasticsearch)

### Phase 2: Quick Detection Script

Create `/scripts/service-health.sh`:

```bash
#!/usr/bin/env bash
# B2X Service Health Quick Check
# Usage: ./scripts/service-health.sh [--verbose] [--json]

set -euo pipefail

# Configuration
SERVICES=(
    "Store Gateway|http://localhost:8000/health"
    "Admin Gateway|http://localhost:8080/health"
    "Aspire Dashboard|http://localhost:15500"
)

TIMEOUT=2
VERBOSE=false
JSON_OUTPUT=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--verbose) VERBOSE=true; shift ;;
        -j|--json) JSON_OUTPUT=true; shift ;;
        *) shift ;;
    esac
done

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

check_service() {
    local name=$1
    local url=$2
    
    local start_time=$(date +%s%N)
    local response
    local http_code
    
    http_code=$(curl -s -o /dev/null -w "%{http_code}" --connect-timeout $TIMEOUT "$url" 2>/dev/null || echo "000")
    local end_time=$(date +%s%N)
    local duration=$(( (end_time - start_time) / 1000000 ))
    
    if [[ "$http_code" == "200" ]]; then
        echo -e "${GREEN}✅${NC} $name ${CYAN}(${duration}ms)${NC}"
        return 0
    elif [[ "$http_code" == "000" ]]; then
        echo -e "${RED}❌${NC} $name - ${YELLOW}Connection refused${NC}"
        return 1
    else
        echo -e "${YELLOW}⚠️${NC} $name - HTTP $http_code"
        return 1
    fi
}

# Discovery: Check what's actually running via Aspire
check_aspire_services() {
    echo -e "\n${CYAN}📡 Checking Aspire-managed Services...${NC}"
    
    # Try to get service list from Aspire dashboard API
    local aspire_api="http://localhost:15500/api/v1/resources"
    local response
    
    if response=$(curl -s --connect-timeout 2 "$aspire_api" 2>/dev/null); then
        if [[ "$VERBOSE" == "true" ]]; then
            echo "$response" | jq -r '.[] | "  \(.name): \(.state)"' 2>/dev/null || echo "  (Could not parse response)"
        else
            local running=$(echo "$response" | jq -r '[.[] | select(.state=="Running")] | length' 2>/dev/null || echo "?")
            local total=$(echo "$response" | jq -r '. | length' 2>/dev/null || echo "?")
            echo -e "  Services: ${GREEN}$running${NC}/$total running"
        fi
    else
        echo -e "  ${YELLOW}Aspire Dashboard not accessible${NC}"
    fi
}

# Main
echo -e "${CYAN}═══════════════════════════════════════════════════════════${NC}"
echo -e "${CYAN}  B2X Service Health Check${NC}"
echo -e "${CYAN}═══════════════════════════════════════════════════════════${NC}"
echo ""

echo -e "${CYAN}🌐 Checking Fixed-Port Services...${NC}"
failed=0
for service_config in "${SERVICES[@]}"; do
    IFS='|' read -r name url <<< "$service_config"
    check_service "$name" "$url" || ((failed++))
done

check_aspire_services

echo ""
if [[ $failed -eq 0 ]]; then
    echo -e "${GREEN}✅ All gateway services healthy${NC}"
else
    echo -e "${YELLOW}⚠️ $failed service(s) need attention${NC}"
fi

echo ""
echo -e "${CYAN}📋 Quick Actions:${NC}"
echo "  Start all:    ./scripts/start-all.sh"
echo "  Check ports:  ./scripts/check-ports.sh"
echo "  Full diag:    ./scripts/diagnose.sh"
```

### Phase 3: Continuous Monitoring Dashboard

Add to `AppHost/Program.cs`:

```csharp
// Health check aggregation endpoint
app.MapGet("/api/health/all", async (HttpClient httpClient) =>
{
    var services = new[]
    {
        ("store-gateway", "http://store-gateway/health"),
        ("admin-gateway", "http://admin-gateway/health"),
        ("auth-service", "http://auth-service/health"),
        ("catalog-service", "http://catalog-service/health"),
    };
    
    var results = await Task.WhenAll(services.Select(async s =>
    {
        try
        {
            var response = await httpClient.GetAsync(s.Item2);
            return new { Name = s.Item1, Healthy = response.IsSuccessStatusCode };
        }
        catch
        {
            return new { Name = s.Item1, Healthy = false };
        }
    }));
    
    return Results.Ok(results);
});
```

---

## Detection Strategy: "Fast Fail, Fast Fix"

### Level 1: Instant Detection (< 5 seconds)

| Check | Command | What It Detects |
|-------|---------|-----------------|
| Port availability | `nc -z localhost 8000` | Service not started |
| Health endpoint | `curl localhost:8000/health` | Service unhealthy |
| Aspire dashboard | Browser: http://localhost:15500 | All service states |

### Level 2: Diagnostic Detection (< 30 seconds)

```bash
# One-liner for quick status
curl -s http://localhost:8000/health && echo "Store OK" || echo "Store FAIL"
curl -s http://localhost:8080/health && echo "Admin OK" || echo "Admin FAIL"

# Or use script
./scripts/service-health.sh
```

### Level 3: Deep Diagnosis (< 2 minutes)

```bash
# Full diagnostic
./scripts/diagnose.sh

# Port conflicts
./scripts/check-ports.sh --free

# Service logs
docker-compose logs --tail=50 [service-name]
```

### Detection Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    Communication Issue Detected                  │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│ Step 1: Check Aspire Dashboard (http://localhost:15500)         │
│         → Shows all service states, errors, logs                │
└─────────────────────────────────────────────────────────────────┘
                                │
                    Service shows as "Running"?
                    ┌───────┴───────┐
                   Yes              No
                    │               │
                    ▼               ▼
┌──────────────────────┐  ┌──────────────────────┐
│ Step 2a: Check Health│  │ Step 2b: Check Ports │
│ curl /health         │  │ ./check-ports.sh     │
│ → Returns 200?       │  │ → Port in use?       │
└──────────────────────┘  └──────────────────────┘
          │                         │
          ▼                         ▼
┌──────────────────────┐  ┌──────────────────────┐
│ Step 3a: Check Logs  │  │ Step 3b: Free Ports  │
│ In Aspire Dashboard  │  │ ./check-ports.sh     │
│ → Look for errors    │  │   --free             │
└──────────────────────┘  └──────────────────────┘
          │                         │
          ▼                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ Step 4: Restart Service(s)                                      │
│         - Individual: Kill + restart in Aspire                  │
│         - All: ./scripts/start-all.sh                           │
└─────────────────────────────────────────────────────────────────┘
```

---

## Common Issues & Quick Fixes

### Issue 1: "Connection Refused"

**Symptom**: `curl: (7) Failed to connect to localhost port 8000`

**Quick Fix**:
```bash
# Check if Aspire is running
curl http://localhost:15500 >/dev/null && echo "Aspire OK" || echo "Start Aspire first"

# Check port
lsof -i :8000 || echo "Port 8000 not in use - service not started"

# Start services
./scripts/start-all.sh
```

### Issue 2: "502 Bad Gateway"

**Symptom**: Gateway responds but upstream service unreachable

**Quick Fix**:
```bash
# Check downstream service health
curl http://localhost:8000/health  # Gateway health
# Look at Aspire dashboard for individual service status

# Check service discovery
# In AppHost logs, look for "Service discovery" messages
```

### Issue 3: "Address Already in Use"

**Symptom**: `System.Net.Sockets.SocketException: Address already in use`

**Quick Fix**:
```bash
# Find what's using the port
lsof -i :8000

# Kill specific process
kill -9 $(lsof -t -i:8000)

# Or kill all B2X processes
./scripts/kill-all-services.sh
```

### Issue 4: "Service Not Found"

**Symptom**: `HttpRequestException: Name or service not known: 'auth-service'`

**Quick Fix**:
- Ensure Aspire AppHost is running (not standalone service)
- Check `.WithReference()` in AppHost/Program.cs
- Verify service name matches exactly

---

## Monitoring Integration

### VS Code Tasks

Add to `.vscode/tasks.json`:

```json
{
    "label": "health-check",
    "type": "shell",
    "command": "./scripts/service-health.sh",
    "group": "test",
    "presentation": {
        "reveal": "always",
        "panel": "shared"
    }
}
```

### Pre-commit Hook (Optional)

```bash
# .git/hooks/pre-push
./scripts/service-health.sh --quick || {
    echo "⚠️ Services not healthy. Push anyway? (y/n)"
    read answer
    [[ "$answer" != "y" ]] && exit 1
}
```

---

## Consequences

### Positive
- ✅ Fast detection of communication issues (< 30 seconds)
- ✅ Consistent health check implementation
- ✅ No more hardcoded URLs that drift out of sync
- ✅ Aspire handles port conflicts for domain services
- ✅ Clear diagnostic flow for troubleshooting

### Negative
- ⚠️ Dependency on Aspire for local development
- ⚠️ Need to update existing hardcoded configurations
- ⚠️ Additional health check dependencies to maintain

### Risks
- Domain services with dynamic ports harder to debug individually
- Aspire dashboard single point of visibility

---

## Related Documents

- [KB-012] Repository Mapping
- [ADR-003] Aspire Orchestration
- [WF-004] GitHub CLI Quick Reference
- Scripts: `check-ports.sh`, `diagnose.sh`, `health-check.sh`

---

## Appendix: Dependency Version Management (Added 2. Jan 2026)

### The Problem: "Version Ping-Pong"

Having multiple `Directory.Packages.props` files causes version conflicts where packages resolve to unexpected versions. Example:

```
Root:    EF Core 10.0.0
Backend: EF Core 10.0.1  ← Identity.EF requires this
Result:  Build fails with CS1705 assembly version mismatch
```

### The Solution: Single Source of Truth

**Rule**: ONE `Directory.Packages.props` file at repository root. No duplicates.

```
B2X/
├── Directory.Packages.props    ← ONLY file for package versions
├── Directory.Build.props       ← Build settings (no versions)
├── backend/
│   └── Directory.Build.props   ← Build settings (no versions, NO packages!)
└── ...
```

### Version Compatibility Matrix

Keep these package groups in sync:

| Package Group | Constraint |
|---------------|------------|
| `Microsoft.EntityFrameworkCore.*` | All must match (e.g., 10.0.1) |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Must match EF Core major (10.0.x) |
| `Aspire.Hosting.*` | All must match (e.g., 13.1.0) |
| `OpenTelemetry.*` | All must match (e.g., 1.14.0) |
| `WolverineFx.*` | All must match (e.g., 5.9.2) |
| `Microsoft.Extensions.*` | Keep at .NET version (10.0.x) |

### When Upgrading Packages

1. **Check compatibility** - Visit nuget.org, check dependencies
2. **Update ONE file** - Edit `/Directory.Packages.props` only
3. **Run restore** - `dotnet restore --force`
4. **Build** - `dotnet build`
5. **Test** - `dotnet test`

### Known Constraints (as of Jan 2026)

| Package | Version | Note |
|---------|---------|------|
| `EFCore.NamingConventions` | 10.0.0-rc.2 | Stable 10.0.0 not yet released |
| `Aspire.Hosting.Elasticsearch` | 13.0.0 | 13.1.0 not yet published |
| `Pomelo.EntityFrameworkCore.MySql` | 8.0.2 | Not yet updated for EF 10 |
| `Oracle.EntityFrameworkCore` | 8.23.70 | Not yet updated for EF 10 |

---

## Checklist for Implementation

- [x] Update ServiceDefaults with unified health checks
- [x] Create `service-health.sh` script
- [x] Remove hardcoded health endpoints from services (Admin, Store, Identity, Layout)
- [x] Add VS Code tasks for health check (`service-health`, `service-health-watch`)
- [x] Update `health-check.sh` with current port configuration
- [x] Consolidate Directory.Packages.props (single source of truth)
- [ ] Document in QUICK_START_GUIDE.md

---

**Agents**: @Architect, @DevOps | Owner: @Architect
