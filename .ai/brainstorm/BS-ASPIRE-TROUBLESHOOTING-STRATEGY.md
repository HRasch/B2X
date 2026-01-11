---
docid: BS-ASPIRE-001
title: Aspire Troubleshooting & Log Detection Strategy
owner: @DevOps
status: Brainstorm
created: 2026-01-10
---

# Aspire Troubleshooting & Log Detection Strategy

**DocID**: `BS-ASPIRE-001`  
**Owner**: @DevOps, @Backend  
**Status**: Brainstorm  
**Version**: 1.0

---

## 🎯 Objective

Establish a comprehensive strategy for detecting, diagnosing, and resolving Aspire orchestration problems through automated log analysis, pattern detection, and proactive monitoring.

---

## 📊 Problem Categories

### 1. Service Discovery & Registration Issues
**Symptoms**:
- Services can't find each other
- DNS resolution failures
- Service mesh communication breakdowns

**Log Patterns**:
```
❌ "Failed to resolve service name: {serviceName}"
❌ "Service endpoint not found"
❌ "No healthy endpoints available for {serviceName}"
❌ "Connection refused to {serviceName}:{port}"
```

**Detection Strategy**:
- Monitor for repeated DNS lookup failures
- Track endpoint registration/deregistration events
- Alert on service mesh connectivity drops

---

### 2. Startup Order & Dependency Problems
**Symptoms**:
- Services fail during startup
- Circular dependency deadlocks
- Missing required dependencies

**Log Patterns**:
```
❌ "Waiting for service {dependency} to be ready..."
❌ "Dependency {service} failed health check"
❌ "Timeout waiting for {dependency}"
❌ "Service initialization failed: dependency not available"
```

**Detection Strategy**:
- Parse startup sequence logs
- Track dependency wait times
- Identify circular dependencies through graph analysis
- Monitor health check cascades

---

### 3. Configuration & Environment Issues
**Symptoms**:
- Configuration not propagated correctly
- Environment variables missing
- Connection strings invalid

**Log Patterns**:
```
❌ "Configuration key not found: {key}"
❌ "Invalid connection string for {serviceName}"
❌ "Environment variable {variable} is not set"
❌ "Configuration validation failed"
```

**Detection Strategy**:
- Validate all required config keys at startup
- Check environment variable completeness
- Verify connection string format and reachability

---

### 4. Port Binding & Network Issues
**Symptoms**:
- Port conflicts
- Address already in use
- Network segmentation problems

**Log Patterns**:
```
❌ "Failed to bind to address {address}:{port}"
❌ "Address already in use: {port}"
❌ "Permission denied on port {port}"
❌ "Network unreachable: {address}"
```

**Detection Strategy**:
- Pre-flight port availability checks
- Monitor for binding failures
- Track network partition events

---

### 5. Resource Exhaustion
**Symptoms**:
- Memory leaks
- CPU throttling
- Connection pool exhaustion

**Log Patterns**:
```
❌ "Out of memory exception"
❌ "Thread pool exhaustion"
❌ "Connection pool timeout"
❌ "Disk space critical: {percentage}%"
⚠️ "High memory usage: {percentage}%"
⚠️ "CPU usage above threshold: {percentage}%"
```

**Detection Strategy**:
- Monitor resource metrics continuously
- Set graduated alert thresholds (warning → critical)
- Track resource trends over time

---

### 6. Health Check Failures
**Symptoms**:
- Services marked unhealthy
- Cascading health check failures
- False positives/negatives

**Log Patterns**:
```
❌ "Health check failed for {service}: {reason}"
⚠️ "Health check timeout after {duration}ms"
❌ "Readiness probe failed: {details}"
❌ "Liveness probe failed: {details}"
```

**Detection Strategy**:
- Differentiate between transient and persistent failures
- Track health check response times
- Identify flapping services (rapid healthy/unhealthy transitions)

---

### 7. Inter-Service Communication Failures
**Symptoms**:
- HTTP errors between services
- gRPC failures
- Message bus disconnections

**Log Patterns**:
```
❌ "HTTP {statusCode} from {service}: {endpoint}"
❌ "gRPC call failed: {service}.{method}"
❌ "Message bus connection lost"
❌ "Retry limit exceeded for {service}"
⚠️ "Circuit breaker opened for {service}"
```

**Detection Strategy**:
- Monitor inter-service error rates
- Track circuit breaker state changes
- Analyze retry patterns and backoff behavior

---

### 8. Database & External Dependencies
**Symptoms**:
- Database connection failures
- Migration errors
- External API timeouts

**Log Patterns**:
```
❌ "Database connection failed: {connectionString}"
❌ "Migration failed: {migrationName}"
❌ "External API timeout: {apiName}"
❌ "Redis connection lost"
⚠️ "Slow query detected: {duration}ms"
```

**Detection Strategy**:
- Monitor all external dependency health
- Track database connection pool metrics
- Alert on migration failures immediately

---

## 🔍 Detection Mechanisms

### A. Real-Time Log Parsing

**Implementation**:
```powershell
# PowerShell log monitoring script
function Watch-AspireLogs {
    param(
        [string]$LogPath = "logs/aspire",
        [string[]]$ErrorPatterns = @(
            "Failed to resolve service",
            "Health check failed",
            "Connection refused",
            "Timeout waiting",
            "Configuration.*not found"
        )
    )
    
    Get-Content $LogPath -Wait -Tail 100 | ForEach-Object {
        foreach ($pattern in $ErrorPatterns) {
            if ($_ -match $pattern) {
                Write-Host "⚠️ ALERT: $_" -ForegroundColor Red
                # Trigger alert mechanism
                Send-Alert -Message $_ -Severity "Warning"
            }
        }
    }
}
```

**MCP Integration**:
```bash
# Use Monitoring MCP for log analysis
monitoring-mcp/analyze_logs filePath="logs/aspire/apphost.log"

# Track specific error patterns
monitoring-mcp/track_errors serviceName="AppHost" pattern="service discovery"
```

---

### B. Structured Logging with Serilog

**Backend Configuration**:
```csharp
// Add to AppHost Program.cs
builder.Services.AddSerilog(config => config
    .WriteTo.Console(new CompactJsonFormatter())
    .WriteTo.File(
        path: "logs/aspire/apphost-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
    )
    .Enrich.WithProperty("Application", "B2X.AppHost")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
);
```

**Log Enrichment**:
```csharp
// Add correlation IDs and service context
Log.Information(
    "Service {ServiceName} started on {Endpoint} with health check {HealthCheckUrl}",
    serviceName,
    endpoint,
    healthCheckUrl
);
```

---

### C. Aspire Dashboard Integration

**Metrics to Monitor**:
- Service startup times
- Health check success rates
- Inter-service request rates
- Error rates by service
- Resource utilization (CPU, memory, connections)

**Dashboard Queries**:
```promql
# Prometheus queries for Aspire metrics

# Service startup duration
aspire_service_startup_duration_seconds{service="store-gateway"}

# Health check failures
rate(aspire_health_check_failures_total[5m])

# Inter-service errors
rate(aspire_http_requests_failed_total{target_service="catalog"}[1m])

# Resource usage
aspire_process_memory_bytes / aspire_process_memory_limit_bytes > 0.8
```

---

### D. Automated Health Checks

**Pre-Flight Validation**:
```bash
#!/bin/bash
# scripts/aspire-preflight-check.sh

echo "🔍 Running Aspire Pre-Flight Checks..."

# Check port availability
check_port() {
    local port=$1
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo "❌ Port $port already in use"
        return 1
    fi
    echo "✅ Port $port available"
    return 0
}

# Check required services
check_dependencies() {
    local services=("postgres" "redis" "elasticsearch")
    for service in "${services[@]}"; do
        if ! docker ps | grep -q $service; then
            echo "❌ Required service $service not running"
            return 1
        fi
        echo "✅ Service $service running"
    done
}

# Validate configuration
check_config() {
    if [ ! -f "appsettings.Development.json" ]; then
        echo "❌ Missing appsettings.Development.json"
        return 1
    fi
    echo "✅ Configuration files present"
}

check_port 15500 && check_port 8000 && check_port 8080 && \
check_dependencies && check_config

if [ $? -eq 0 ]; then
    echo "✅ All pre-flight checks passed"
    exit 0
else
    echo "❌ Pre-flight checks failed"
    exit 1
fi
```

---

### E. Service Dependency Graph Analysis

**Build Dependency Graph**:
```csharp
// AppHost configuration analysis
public class ServiceDependencyAnalyzer
{
    public static Dictionary<string, List<string>> BuildDependencyGraph(
        IDistributedApplicationBuilder builder)
    {
        var graph = new Dictionary<string, List<string>>();
        
        foreach (var resource in builder.Resources)
        {
            var dependencies = resource.Annotations
                .OfType<ServiceReferenceAnnotation>()
                .Select(a => a.ServiceName)
                .ToList();
                
            graph[resource.Name] = dependencies;
        }
        
        return graph;
    }
    
    public static bool HasCircularDependency(Dictionary<string, List<string>> graph)
    {
        // Implement cycle detection algorithm
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();
        
        foreach (var node in graph.Keys)
        {
            if (DetectCycle(node, graph, visited, recursionStack))
                return true;
        }
        
        return false;
    }
}
```

---

## 🛠️ Troubleshooting Workflows

### Workflow 1: Service Won't Start

**Detection**: Service stuck in "Starting" state for >30s

**Investigation Steps**:
1. Check service logs for startup errors
   ```bash
   docker logs {container-name} --tail 100
   ```

2. Verify dependencies are healthy
   ```bash
   scripts/service-health.sh
   ```

3. Check port availability
   ```powershell
   Get-NetTCPConnection -LocalPort {port}
   ```

4. Validate configuration
   ```bash
   docker-mcp/inspect_container containerName="{service}"
   ```

5. Check resource availability
   ```bash
   monitoring-mcp/monitor_system_performance hostName="localhost"
   ```

**Automated Remediation**:
```powershell
# Auto-restart stuck services
function Restart-StuckService {
    param([string]$ServiceName)
    
    $container = docker ps -a --filter "name=$ServiceName" --format "{{.ID}}"
    if ($container) {
        Write-Host "Restarting stuck service: $ServiceName"
        docker restart $container
        Start-Sleep -Seconds 10
        
        # Verify health
        $health = docker inspect $container --format "{{.State.Health.Status}}"
        if ($health -eq "healthy") {
            Write-Host "✅ Service $ServiceName recovered"
        } else {
            Write-Host "❌ Service $ServiceName still unhealthy - manual intervention required"
        }
    }
}
```

---

### Workflow 2: Service Discovery Failure

**Detection**: Logs show "service not found" or DNS errors

**Investigation Steps**:
1. Check service registration
   ```bash
   # Verify service is registered in Aspire
   curl http://localhost:15500/api/resources
   ```

2. Test DNS resolution
   ```bash
   nslookup {service-name}
   ```

3. Check network connectivity
   ```bash
   docker network inspect aspire-network
   ```

4. Verify service endpoint configuration
   ```bash
   docker-mcp/check_service_dependencies filePath="docker-compose.aspire.yml"
   ```

**Fix Procedure**:
```bash
# Restart Aspire networking
docker network disconnect aspire-network {container}
docker network connect aspire-network {container}
docker restart {container}
```

---

### Workflow 3: Configuration Problems

**Detection**: "Configuration key not found" or "Invalid connection string"

**Investigation Steps**:
1. Validate environment variables
   ```powershell
   docker exec {container} env | Select-String "ASPNETCORE"
   ```

2. Check appsettings.json structure
   ```bash
   jq . appsettings.Development.json
   ```

3. Verify configuration binding
   ```csharp
   // Add configuration diagnostics
   var config = app.Services.GetRequiredService<IConfiguration>();
   Log.Information("Configuration: {@Config}", config.AsEnumerable());
   ```

4. Test connection strings
   ```bash
   # Use database MCP to validate
   database-mcp/validate_schema connectionString="{conn-string}"
   ```

---

### Workflow 4: Resource Exhaustion

**Detection**: Memory/CPU alerts or slow performance

**Investigation Steps**:
1. Check resource usage
   ```bash
   docker stats --no-stream
   ```

2. Analyze memory leaks
   ```bash
   monitoring-mcp/profile_performance serviceName="{service}"
   ```

3. Review connection pools
   ```bash
   # Check database connections
   docker exec postgres psql -c "SELECT count(*) FROM pg_stat_activity"
   ```

4. Identify slow operations
   ```bash
   monitoring-mcp/analyze_logs filePath="logs/{service}.log" pattern="duration"
   ```

**Remediation**:
```powershell
# Increase resource limits
docker update --memory 2g --cpus 2 {container}

# Or update docker-compose
docker-compose up -d --force-recreate {service}
```

---

## 🤖 Automated Monitoring System

### Implementation: Aspire Health Monitor Service

**Core Monitoring Loop**:
```csharp
// src/backend/Infrastructure/Monitoring/AspireHealthMonitor.cs

public class AspireHealthMonitor : BackgroundService
{
    private readonly ILogger<AspireHealthMonitor> _logger;
    private readonly HttpClient _httpClient;
    private readonly IAlertingService _alerting;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await MonitorAllServices();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
    
    private async Task MonitorAllServices()
    {
        var services = await GetRegisteredServices();
        
        foreach (var service in services)
        {
            var health = await CheckServiceHealth(service);
            
            if (health.Status != HealthStatus.Healthy)
            {
                await HandleUnhealthyService(service, health);
            }
            
            await CheckServiceMetrics(service);
        }
    }
    
    private async Task HandleUnhealthyService(Service service, HealthReport health)
    {
        _logger.LogWarning(
            "Service {ServiceName} is {Status}: {Details}",
            service.Name,
            health.Status,
            health.Details
        );
        
        // Auto-remediation based on error type
        if (health.ErrorType == "DependencyFailure")
        {
            await RestartDependencies(service);
        }
        else if (health.ErrorType == "ConfigurationError")
        {
            await ValidateConfiguration(service);
        }
        
        // Alert if still unhealthy
        if (health.ConsecutiveFailures > 3)
        {
            await _alerting.SendAlert(new Alert
            {
                Severity = AlertSeverity.Critical,
                Service = service.Name,
                Message = $"Service unhealthy for {health.ConsecutiveFailures} checks"
            });
        }
    }
}
```

---

### Log Pattern Detection Engine

**Implementation**:
```csharp
public class LogPatternDetector
{
    private readonly Dictionary<string, Regex> _errorPatterns = new()
    {
        ["ServiceDiscovery"] = new Regex(@"Failed to resolve service name: (?<service>\w+)"),
        ["DependencyTimeout"] = new Regex(@"Timeout waiting for (?<dependency>\w+)"),
        ["PortBinding"] = new Regex(@"Address already in use: (?<port>\d+)"),
        ["ConfigMissing"] = new Regex(@"Configuration key not found: (?<key>[\w:]+)"),
        ["HealthCheck"] = new Regex(@"Health check failed for (?<service>\w+): (?<reason>.+)"),
    };
    
    public IEnumerable<DetectedIssue> AnalyzeLogs(string logContent)
    {
        var issues = new List<DetectedIssue>();
        var lines = logContent.Split('\n');
        
        foreach (var line in lines)
        {
            foreach (var (category, pattern) in _errorPatterns)
            {
                var match = pattern.Match(line);
                if (match.Success)
                {
                    issues.Add(new DetectedIssue
                    {
                        Category = category,
                        Timestamp = ExtractTimestamp(line),
                        Details = match.Groups.Cast<Group>()
                            .Skip(1)
                            .ToDictionary(g => g.Name, g => g.Value),
                        OriginalLine = line
                    });
                }
            }
        }
        
        return issues;
    }
    
    public TroubleshootingGuide GetGuide(string category)
    {
        return category switch
        {
            "ServiceDiscovery" => new TroubleshootingGuide
            {
                Title = "Service Discovery Failure",
                Steps = new[]
                {
                    "1. Verify service is registered in Aspire",
                    "2. Check DNS resolution",
                    "3. Inspect network configuration",
                    "4. Restart service registration"
                },
                AutoRemediation = async () => await RestartServiceMesh()
            },
            "DependencyTimeout" => new TroubleshootingGuide
            {
                Title = "Dependency Startup Timeout",
                Steps = new[]
                {
                    "1. Check dependency service health",
                    "2. Review startup order configuration",
                    "3. Increase timeout threshold",
                    "4. Check for circular dependencies"
                },
                AutoRemediation = async () => await RestartInDependencyOrder()
            },
            // ... other patterns
            _ => TroubleshootingGuide.Default
        };
    }
}
```

---

## 📋 Monitoring Dashboard Specifications

### Real-Time Metrics Panel

**Key Metrics**:
1. **Service Health Overview**
   - Total services registered
   - Healthy / Unhealthy / Unknown
   - Health check success rate (%)

2. **Startup Metrics**
   - Average startup time by service
   - Failed startups (last 24h)
   - Dependency wait times

3. **Communication Health**
   - Inter-service request rate
   - Error rate by service pair
   - Circuit breaker states
   - Average response times

4. **Resource Utilization**
   - CPU usage by service
   - Memory usage by service
   - Connection pool status
   - Thread pool availability

5. **Error Trends**
   - Error rate over time
   - Top error categories
   - Most affected services
   - Resolution times

**Dashboard Technology**:
- Grafana for visualization
- Prometheus for metrics collection
- Aspire built-in dashboard for service topology
- Custom PowerShell/web dashboard for pattern detection

---

## 🚨 Alerting Rules

### Alert Severity Levels

**P0 - Critical** (Immediate Response):
- Any service completely down for >2 minutes
- Database connectivity lost
- >50% of services unhealthy
- Cascading failures detected
- Security-related errors

**P1 - High** (Response within 15 min):
- Service degraded performance (>5s response time)
- Health check failure rate >20%
- Memory usage >90%
- Circuit breaker open for critical service
- Configuration errors preventing startup

**P2 - Medium** (Response within 1 hour):
- Individual service flapping (healthy/unhealthy)
- Slow queries detected
- Connection pool utilization >80%
- Retry rate elevated
- Non-critical dependency timeout

**P3 - Low** (Response within 24 hours):
- Warning-level log entries
- Resource usage trending upward
- Minor configuration inconsistencies
- Deprecated API usage detected

---

### Alert Configuration

**Prometheus Alerting Rules**:
```yaml
# config/monitoring/aspire_alert_rules.yml

groups:
  - name: aspire_service_health
    interval: 30s
    rules:
      - alert: ServiceDown
        expr: up{job="aspire-services"} == 0
        for: 2m
        labels:
          severity: critical
        annotations:
          summary: "Service {{ $labels.service }} is down"
          description: "Service has been down for more than 2 minutes"
          
      - alert: HighErrorRate
        expr: rate(http_requests_failed_total[5m]) > 0.1
        for: 5m
        labels:
          severity: high
        annotations:
          summary: "High error rate for {{ $labels.service }}"
          description: "Error rate is {{ $value | humanize }}%"
          
      - alert: HealthCheckFailing
        expr: health_check_success == 0
        for: 3m
        labels:
          severity: high
        annotations:
          summary: "Health check failing for {{ $labels.service }}"
          
      - alert: MemoryHigh
        expr: (process_memory_bytes / process_memory_limit_bytes) > 0.9
        for: 10m
        labels:
          severity: medium
        annotations:
          summary: "Memory usage high for {{ $labels.service }}"
          description: "Memory at {{ $value | humanizePercentage }}"
```

---

## 🔧 MCP Tool Integration

### Monitoring MCP Workflow

**Pre-Startup Validation**:
```bash
# Use MCP tools for comprehensive pre-flight checks

# 1. Validate Docker configuration
docker-mcp/analyze_docker_compose filePath="config/docker-compose.yml"

# 2. Check container security
docker-mcp/check_container_security imageName="b2x/apphost:latest"

# 3. Validate system resources
monitoring-mcp/monitor_system_performance hostName="localhost"

# 4. Check application metrics baseline
monitoring-mcp/collect_application_metrics serviceName="apphost"
```

**Runtime Monitoring**:
```bash
# Continuous health tracking
monitoring-mcp/validate_health_checks serviceName="all"

# Error tracking
monitoring-mcp/track_errors serviceName="store-gateway"

# Log analysis
monitoring-mcp/analyze_logs filePath="logs/aspire/apphost.log"

# Performance profiling
monitoring-mcp/profile_performance serviceName="catalog-service"
```

---

### Git MCP for Change Correlation

**Track Changes That Broke Services**:
```bash
# When a service starts failing, correlate with recent changes
git-mcp/analyze_code_churn workspacePath="." since="last-deploy"

# Check commit messages for deployment-related changes
git-mcp/validate_commit_messages workspacePath="." count=20

# Identify risky changes
git-mcp/check_branch_strategy workspacePath="." branchName="main"
```

---

### Security MCP for Security-Related Failures

**Validate Security Configuration**:
```bash
# Check for security issues in configuration
security-mcp/validate_network_policies filePath="config/docker-compose.yml"

# Scan for vulnerabilities that might cause failures
security-mcp/scan_vulnerabilities workspacePath="."

# Validate secrets management
security-mcp/check_secrets_management workspacePath="src/backend"
```

---

## 📖 Runbooks

### Runbook 1: Complete Service Failure

**Scenario**: All Aspire services down

**Steps**:
1. Check if AppHost process is running
   ```powershell
   Get-Process -Name "dotnet" | Where-Object {$_.MainWindowTitle -like "*AppHost*"}
   ```

2. Verify Docker daemon is running
   ```powershell
   docker info
   ```

3. Check for port conflicts
   ```bash
   scripts/aspire-preflight-check.sh
   ```

4. Review AppHost logs
   ```bash
   tail -f logs/aspire/apphost.log
   ```

5. Restart entire stack
   ```powershell
   scripts/kill-all-services.sh
   dotnet run --project src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj
   ```

6. Validate startup sequence
   ```bash
   scripts/service-health.sh --watch
   ```

---

### Runbook 2: Single Service Repeatedly Failing

**Scenario**: One service keeps restarting

**Steps**:
1. Get service logs
   ```bash
   docker logs {service-container} --tail 200
   ```

2. Check dependency health
   ```bash
   # Get dependencies from docker-compose
   docker-mcp/check_service_dependencies filePath="config/docker-compose.yml"
   ```

3. Validate configuration
   ```bash
   docker exec {service-container} cat appsettings.json | jq .
   ```

4. Check resource constraints
   ```bash
   docker stats {service-container} --no-stream
   ```

5. Test with increased resources
   ```bash
   docker update --memory 2g --cpus 2 {service-container}
   docker restart {service-container}
   ```

6. If still failing, isolate and debug
   ```bash
   docker run -it --entrypoint /bin/bash {service-image}
   ```

---

### Runbook 3: Slow Performance / Degradation

**Scenario**: Services running but slow

**Steps**:
1. Profile all services
   ```bash
   monitoring-mcp/profile_performance serviceName="all"
   ```

2. Check for database bottlenecks
   ```bash
   database-mcp/analyze_queries workspacePath="src/backend"
   ```

3. Review connection pool status
   ```sql
   -- PostgreSQL
   SELECT count(*), state FROM pg_stat_activity GROUP BY state;
   ```

4. Check for memory leaks
   ```bash
   # Monitor memory over time
   while true; do docker stats --no-stream; sleep 30; done
   ```

5. Identify slow endpoints
   ```bash
   # Parse logs for slow requests
   monitoring-mcp/analyze_logs filePath="logs/gateway.log" pattern="duration>1000"
   ```

6. Scale up if needed
   ```bash
   docker-compose up -d --scale catalog-service=3
   ```

---

## 🎓 Knowledge Base Integration

### Auto-Capture Lessons Learned

**After Resolving an Issue**:
```bash
# Automatically generate lesson entry
/auto-lessons-learned
Bug: [Aspire service discovery failure after network restart]
Fix: [Added retry logic with exponential backoff in service registration]
Lesson: [Always implement resilient service discovery with retries]
```

**This automatically updates** `.ai/knowledgebase/lessons.md` with:
- Problem description
- Root cause analysis
- Solution applied
- Prevention measures
- Related documentation

---

### Searchable Problem Database

**Structure** `.ai/knowledgebase/aspire-troubleshooting.md`:
```markdown
## Problem: Service Won't Start - Port Binding Error

**Symptoms**:
- Error: "Address already in use: 8000"
- Service stuck in "Starting" state

**Root Cause**: Another process using the port

**Solution**:
1. Identify process: `Get-Process -Id (Get-NetTCPConnection -LocalPort 8000).OwningProcess`
2. Kill process or change port in configuration
3. Restart service

**Prevention**:
- Use pre-flight port check script
- Configure dynamic port allocation
- Document all used ports

**Related Issues**: #142, #298
```

---

## 🔄 Continuous Improvement Loop

### Metrics to Track

**Problem Detection**:
- Time to detect issue (MTTD)
- False positive rate
- Coverage of detection rules

**Problem Resolution**:
- Time to resolve (MTTR)
- Auto-remediation success rate
- Manual intervention required (%)

**System Reliability**:
- Service uptime (%)
- Health check success rate
- Startup success rate
- Mean time between failures (MTBF)

### Monthly Review Process

1. **Analyze failure patterns** - Identify recurring issues
2. **Update detection rules** - Add new patterns discovered
3. **Refine auto-remediation** - Improve automated fixes
4. **Update documentation** - Keep runbooks current
5. **Train team** - Share lessons learned
6. **Optimize thresholds** - Reduce false positives/negatives

---

## 🚀 Implementation Roadmap

### Phase 1: Foundation (Week 1-2)
- [ ] Set up structured logging with Serilog
- [ ] Create log pattern detection engine
- [ ] Implement basic health monitoring
- [ ] Write pre-flight check script

### Phase 2: Automation (Week 3-4)
- [ ] Build AspireHealthMonitor background service
- [ ] Configure Prometheus alerting rules
- [ ] Implement auto-remediation for common issues
- [ ] Create PowerShell monitoring scripts

### Phase 3: Integration (Week 5-6)
- [ ] Integrate MCP tools into workflows
- [ ] Set up Grafana dashboards
- [ ] Configure alert channels (email, Slack, etc.)
- [ ] Write comprehensive runbooks

### Phase 4: Optimization (Week 7-8)
- [ ] Analyze metrics and refine thresholds
- [ ] Improve auto-remediation success rate
- [ ] Build knowledge base integration
- [ ] Train team on new tools and processes

### Phase 5: Advanced Features (Ongoing)
- [ ] ML-based anomaly detection
- [ ] Predictive failure analysis
- [ ] Automated capacity planning
- [ ] Self-healing infrastructure

---

## 📚 References

**Internal Documentation**:
- [ARCH-DEF] Software Definition
- [KB-061] Monitoring MCP Usage Guide
- [KB-057] Database MCP Usage Guide
- [DOC-APPHOST-SPEC] AppHost Specifications
- [GL-055] Security MCP Best Practices

**External Resources**:
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki)
- [Prometheus Alerting](https://prometheus.io/docs/alerting/latest/)

---

## ✅ Success Criteria

**Strategy is successful when**:
- ✅ 90% of issues auto-detected within 1 minute
- ✅ 50% of common issues auto-remediated
- ✅ MTTR reduced by 70%
- ✅ Zero unplanned downtime for >30 days
- ✅ Team resolves issues without consulting documentation
- ✅ New team members can troubleshoot using runbooks

---

**Last Updated**: 2026-01-10  
**Next Review**: 2026-02-10  
**Owner**: @DevOps, @Backend

