# Docker Security Configuration

## Overview

This directory contains Docker Compose configurations with environment-specific security settings for the B2X platform.

## Files

- `docker-compose.yml` - Main development configuration with conditional port exposure
- `docker-compose.prod.yml` - Production configuration with no infrastructure port exposure
- `.env.development` - Development environment variables (ports exposed)
- `.env.production` - Production environment variables (ports not exposed)

## Security Features

### Port Exposure Control

Infrastructure services (PostgreSQL, Redis, RabbitMQ, Elasticsearch, Kibana, Prometheus, Grafana) have their ports conditionally exposed based on environment variables:

- **Development**: Ports exposed when environment variables are set (e.g., `POSTGRES_PORT=5432`)
- **Production**: Ports NOT exposed - services only accessible via internal Docker network

### Environment-Specific Configurations

#### Development Usage
```bash
# Copy development environment file
cp config/.env.development config/.env

# Start services with ports exposed
docker-compose --env-file config/.env up -d
```

#### Production Usage
```bash
# Use production compose file (no ports exposed)
docker-compose -f config/docker-compose.prod.yml --env-file config/.env.production up -d
```

## Exposed Ports by Environment

### Development (when env vars set)
- PostgreSQL: 5432
- Redis: 6379
- RabbitMQ AMQP: 5672
- RabbitMQ Management: 15672
- Elasticsearch: 9200
- Kibana: 5601
- Prometheus: 9090
- Grafana: 3000

### Production (never exposed)
- All infrastructure ports remain internal
- Only application ports exposed via reverse proxy/load balancer

## Security Benefits

1. **No Exposed Infrastructure**: Production deployments don't expose database/cache ports
2. **Internal Networking**: Services communicate via secure Docker networks
3. **Environment Isolation**: Different configurations for dev/prod
4. **Credential Management**: Secrets managed via environment variables
5. **Access Control**: Infrastructure access limited to application layer

## Deployment Instructions

### Development
```bash
cd config
cp .env.development .env
docker-compose up -d
```

### Production
```bash
cd config
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d
```

## Monitoring

Use the following commands to verify port exposure:

```bash
# Check which ports are exposed
docker-compose ps
docker port <container_name>

# Verify internal networking
docker-compose exec <service> curl http://<other_service>:port
```

## Security Audit

This configuration addresses the following security issues:
- ✅ Infrastructure ports not exposed in production
- ✅ Environment-specific configurations
- ✅ Internal service communication
- ✅ Credential management via environment variables
- ✅ No hardcoded secrets in compose files