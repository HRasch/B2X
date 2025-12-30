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
