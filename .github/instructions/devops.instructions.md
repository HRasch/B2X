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

## Infrastructure Policies
- Establish infrastructure-as-code policies requiring all changes to be version-controlled and reviewed
- Implement automated deployment policies with canary releases and rollback procedures
- Create monitoring and alerting policies with defined SLIs/SLOs and incident response
- Mandate Kubernetes configuration standards and security best practices

## 🤖 AI Governance Integration

**MANDATORY**: All DevOps operations must comply with [ai-governance.instructions.md](ai-governance.instructions.md):

### Performance Standards
- Infrastructure response times: <200ms simple, <2s complex operations
- Resource utilization: <70% CPU, <80% memory across all services
- Deployment accuracy: >99.9% success rate

### Security Requirements
- Zero-trust authentication for all infrastructure access
- AES-256-GCM encryption for all secrets and configurations
- GDPR/NIS2/AI Act compliance for data handling
- Comprehensive audit logging for all infrastructure changes

### Operational Boundaries
- Infrastructure domain expertise restrictions
- Mandatory escalation for security-critical changes
- Quality gates for all infrastructure modifications
- Ethical AI constraints in automated deployments

### Quality Assurance
- Peer review for infrastructure changes
- Automated testing integration for IaC
- Bias mitigation in deployment strategies
- Human oversight for production deployments
