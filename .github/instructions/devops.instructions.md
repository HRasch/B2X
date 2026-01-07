---
applyTo: ".github/**,Dockerfile,docker-compose*,*.yml,*.yaml,**/infra/**,**/terraform/**"
---

# DevOps Instructions

## CI/CD Pipelines
- Keep pipeline stages atomic and focused
- Fail fast (run quick checks first)
- Cache dependencies appropriately
- Use matrix builds for multi-environment testing

## Docker
- Use multi-stage builds for smaller images
- Don't run containers as root
- Pin base image versions
- Use .dockerignore to exclude unnecessary files

## Infrastructure as Code
- Version control all infrastructure
- Use modules for reusable components
- Document infrastructure decisions
- Test infrastructure changes in staging first

## Environment Management
- Maintain parity between environments
- Use environment-specific configurations
- Never hardcode environment values
- Document environment setup requirements

## Monitoring & Logging
- Implement health checks on all services
- Use structured logging
- Set up alerting for critical metrics
- Document runbook procedures

## Security
- Scan images for vulnerabilities
- Use secrets management (not env files in repos)
- Implement network policies
- Regular security audits of infrastructure

## MCP-Enhanced DevOps Workflow

**Reference**: See [KB-055] Security MCP Best Practices and MCP Operations Guide for comprehensive tooling.

### Docker MCP Integration (Always Enabled)

**Reference**: See Docker MCP tools in MCP Operations Guide.

**Container Security Validation**:
```bash
# Scan Docker images for vulnerabilities
docker-mcp/check_container_security imageName="b2connect/api:latest"

# Validate Dockerfile best practices
docker-mcp/analyze_dockerfile filePath="Dockerfile"

# Check Kubernetes manifests
docker-mcp/validate_kubernetes_manifests filePath="k8s/deployment.yaml"

# Monitor container health
docker-mcp/monitor_container_health containerName="b2connect-api"
```

**Docker Compose Analysis**:
```bash
# Analyze compose configuration
docker-mcp/analyze_docker_compose filePath="docker-compose.yml"

# Validate service dependencies
docker-mcp/check_service_dependencies filePath="docker-compose.yml"
```

### Monitoring MCP for Infrastructure Observability

**Reference**: See [KB-061] Monitoring MCP Usage Guide.

**Application Metrics Collection**:
```bash
# Collect application performance metrics
monitoring-mcp/collect_application_metrics serviceName="api-gateway"

# Monitor system performance
monitoring-mcp/monitor_system_performance hostName="prod-server-01"

# Track application errors
monitoring-mcp/track_errors serviceName="catalog-service"

# Analyze application logs
monitoring-mcp/analyze_logs filePath="logs/application.log"
```

**Health Checks and Alerting**:
```bash
# Validate health check endpoints
monitoring-mcp/validate_health_checks serviceName="all"

# Configure alerting rules
monitoring-mcp/configure_alerts serviceName="database" metric="connection_pool_usage" threshold="90%"

# Profile performance issues
monitoring-mcp/profile_performance serviceName="search-service"
```

### Git MCP for Deployment and Release Management

**Reference**: See Git MCP tools in MCP Operations Guide.

**Commit Quality Validation**:
```bash
# Validate commit messages follow conventional commits
git-mcp/validate_commit_messages workspacePath="." count=20

# Analyze code churn for deployment risk assessment
git-mcp/analyze_code_churn workspacePath="." since="last-deployment"

# Check branch strategy compliance
git-mcp/check_branch_strategy workspacePath="." branchName="feature/new-api"
```

### Security MCP for Infrastructure Security

**Reference**: See [KB-055] Security MCP Best Practices.

**Infrastructure Security Audits**:
```bash
# Scan for container vulnerabilities
security-mcp/scan_vulnerabilities workspacePath="." target="docker-images"

# Validate secrets management
security-mcp/check_secrets_management workspacePath="infrastructure"

# Audit network security policies
security-mcp/validate_network_policies filePath="k8s/network-policies.yaml"
```

### Documentation MCP for Infrastructure Documentation

**Reference**: See [KB-062] Documentation MCP Usage Guide.

**Infrastructure Documentation Validation**:
```bash
# Validate deployment documentation
docs-mcp/validate_documentation filePath="docs/deployment/runbook.md"

# Check documentation links
docs-mcp/check_links workspacePath="docs/infrastructure"

# Analyze documentation quality
docs-mcp/analyze_content_quality filePath="README.md"
```

### MCP-Powered DevOps Checklist

**Pre-Deployment Validation**:
- [ ] `docker-mcp/analyze_dockerfile` - Dockerfile follows best practices
- [ ] `docker-mcp/check_container_security` - No critical vulnerabilities
- [ ] `security-mcp/scan_vulnerabilities` - Infrastructure secure
- [ ] `monitoring-mcp/validate_health_checks` - Health checks configured
- [ ] `git-mcp/validate_commit_messages` - Conventional commits
- [ ] `docs-mcp/validate_documentation` - Documentation complete

**Post-Deployment Monitoring**:
- [ ] `monitoring-mcp/collect_application_metrics` - Metrics collection active
- [ ] `monitoring-mcp/monitor_system_performance` - System performance baseline
- [ ] `docker-mcp/monitor_container_health` - Container health checks

