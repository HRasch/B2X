---
docid: INS-010
title: Devops.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

﻿---
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

## CI Quality Gates (Proposed)
- Canonical gate sequence: build → analyzers/format → unit tests → smoke e2e → security scans
- Fail fast: fail PR early on build/analyzer failures to reduce wasted cycles

## Nightly Audits & KB Sync (Proposed)
- Schedule nightly/weekly jobs that aggregate CI failures, SCA alerts, and flaky-test metrics
- Create remediation issues automatically
- Include weekly KB sync job (see `.github/workflows/kb-sync.yml`)

## Dashboards & Retention (Proposed)
- Publish gate pass/fail rates to GitHub Checks + README summary or SonarCloud
- Retain CI artifacts for minimum 30 days to support Control evidence

## Secrets & Policies (Proposed)
- Enforce secret scanning in CI and block merges on detected secrets
- Use a central secret store for runtime values

## MCP-Enhanced DevOps Workflow

**Reference**: See [KB-055] Security MCP Best Practices and MCP Operations Guide for comprehensive tooling.

### Docker MCP Integration (Always Enabled)

**Reference**: See Docker MCP tools in MCP Operations Guide.

**Container Security Validation**:
```bash
# Scan Docker images for vulnerabilities
docker-mcp/check_container_security imageName="B2X/api:latest"

# Validate Dockerfile best practices
docker-mcp/analyze_dockerfile filePath="Dockerfile"

# Check Kubernetes manifests
docker-mcp/validate_kubernetes_manifests filePath="k8s/deployment.yaml"

# Monitor container health
docker-mcp/monitor_container_health containerName="B2X-api"
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

### runSubagent for Deployment Validation (Token-Optimized)

For comprehensive pre-deployment checks, use `#runSubagent` to execute all validations in isolation:

```text
Validate deployment readiness with #runSubagent:
- Analyze Dockerfile for best practices
- Check container security for all images
- Scan infrastructure vulnerabilities
- Validate health check endpoints
- Verify commit messages follow conventions
- Check documentation completeness

Return ONLY: deployment_readiness_score + blocking_issues + warnings
```

**Benefits**:
- ~3000 Token savings per deployment validation
- All 6 checks run in isolated context
- Clear pass/fail decision returned
- Detailed findings stay in subagent context

**When to use**: Before any staging/production deployment

---

### MCP-Powered DevOps Checklist

**Pre-Deployment Validation** (or use runSubagent above for automated validation):
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

## Large File Editing Strategy ([GL-043])

When editing large infrastructure files (>200 lines), use the Multi-Language Fragment Editing approach with Docker MCP for infrastructure:

### Pre-Edit Analysis
```bash
# Infrastructure dependency analysis
docker-mcp/analyze_docker_compose filePath="docker-compose.yml"

# Security vulnerability assessment
docker-mcp/check_container_security imageName="B2X/api:latest"

# Kubernetes manifest validation
docker-mcp/validate_kubernetes_manifests filePath="k8s/deployment.yaml"
```

### Fragment-Based Editing Patterns
```yaml
# Fragment: Service configuration (82% token savings)
services:
  api:
    image: b2x/api:latest
    # Edit only service-specific config
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Database__Provider=PostgreSQL
    ports:
      - "8080:80"
    # Reference external networks/volumes
    networks:
      - b2x-network
    volumes:
      - logs:/app/logs
```

**Docker MCP Workflows**:
```bash
# 1. Dockerfile analysis and optimization
docker-mcp/analyze_dockerfile filePath="Dockerfile"
docker-mcp/optimize_dockerfile filePath="Dockerfile"

# 2. Container security scanning
docker-mcp/check_container_security imageName="B2X/api:latest"
docker-mcp/scan_vulnerabilities workspacePath="." target="docker-images"

# 3. Compose file validation
docker-mcp/analyze_docker_compose filePath="docker-compose.yml"
docker-mcp/check_service_dependencies filePath="docker-compose.yml"

# 4. Kubernetes manifest validation
docker-mcp/validate_kubernetes_manifests filePath="k8s/deployment.yaml"
docker-mcp/validate_kubernetes_manifests filePath="k8s/service.yaml"

# 5. Infrastructure monitoring setup
docker-mcp/monitor_container_health containerName="B2X-api"
```

### Integration with Infrastructure MCP Tools
```bash
# Monitoring configuration
monitoring-mcp/configure_alerts serviceName="api-gateway" metric="response_time" threshold="500ms"

# Security policy validation
security-mcp/validate_network_policies filePath="k8s/network-policies.yaml"

# GitOps validation
git-mcp/validate_commit_messages workspacePath="." count=10
```

### Quality Gates
- Always run `docker-mcp/analyze_dockerfile` after Dockerfile edits
- Use `docker-mcp/check_container_security` for security validation
- Validate Kubernetes manifests with `docker-mcp/validate_kubernetes_manifests`
- Monitor container health with dedicated MCP tools

**Token Savings**: 82% vs. reading entire infrastructure files | **Quality**: Security and dependency validation with container health monitoring

## Large File Editing Strategy ([GL-053])

When editing large infrastructure files (>200 lines), use the Multi-Language Fragment Editing approach:

### Pre-Edit Analysis
```bash
# Infrastructure validation
docker-mcp/analyze_dockerfile filePath="Dockerfile"
docker-mcp/validate_kubernetes_manifests filePath="k8s/deployment.yaml"

# Security audit
security-mcp/scan_vulnerabilities workspacePath="infrastructure"
```

### Fragment-Based Editing
```bash
# Read targeted sections only
read_file("docker-compose.yml", startLine: 45, endLine: 85)

# Apply infrastructure refactoring
docker-mcp/analyze_docker_compose filePath="docker-compose.yml"

# Validate changes
docker-mcp/check_container_security imageName="nginx:latest"
```

### Quality Gates
- Always run infrastructure validation after edits
- Use `docker-mcp` and `security-mcp` for compliance checks
- Test deployments in staging environment

**Token Savings**: 70-85% vs. reading entire infrastructure files | **Quality**: Infrastructure security validation

