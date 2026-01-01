# Kubernetes Deployment

## Overview

The B2Connect platform implements comprehensive Kubernetes deployment strategies for production environments, supporting both direct YAML manifests and Helm chart-based installations. This document covers deployment patterns, configuration management, and scaling strategies for the microservices architecture.

## Deployment Architecture

### Infrastructure Components

#### Database Layer
- **PostgreSQL StatefulSet**: Persistent database storage with automatic failover
- **Redis StatefulSet**: Distributed caching with persistence
- **Elasticsearch Deployment**: Search and analytics capabilities
- **RabbitMQ StatefulSet**: Message queuing with clustering

#### Service Layer
- **API Gateway**: Entry point for external traffic with load balancing
- **Domain Services**: Individual microservices with horizontal scaling
- **Frontend Applications**: Static content served via Nginx ingress

### Namespace Organization
```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: b2connect
```

#### Multi-Environment Support
- **Development**: Resource-constrained deployments for testing
- **Staging**: Production-like environment for integration testing
- **Production**: Full-scale deployment with high availability

## Deployment Strategies

### Rolling Updates
```yaml
strategy:
  type: RollingUpdate
  rollingUpdate:
    maxUnavailable: 25%
    maxSurge: 25%
```

#### Zero-Downtime Deployments
- **Health Checks**: Readiness and liveness probes for all services
- **Graceful Shutdown**: Proper termination handling
- **Service Mesh**: Istio integration for traffic management

### Blue-Green Deployments
- **Separate Namespaces**: Blue and green environments
- **Traffic Switching**: Ingress configuration updates
- **Rollback Capability**: Instant reversion to previous version

### Canary Deployments
- **Percentage-Based**: Gradual traffic shifting
- **Metrics Monitoring**: Performance and error rate tracking
- **Automated Rollback**: Threshold-based reversion

## Configuration Management

### ConfigMaps for Application Settings
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aspire-config
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  Services__ApiGateway: "http://api-gateway:5000"
  HealthCheck__Enabled: "true"
```

#### Environment-Specific Configurations
- **Development**: Debug logging, relaxed security
- **Production**: Structured logging, full security
- **Feature Flags**: Runtime configuration for feature toggles

### Secrets Management
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: db-secrets
type: Opaque
data:
  postgres-password: <base64-encoded>
  jwt-secret: <base64-encoded>
```

#### External Secret Management
- **Azure Key Vault**: Cloud-native secret storage
- **HashiCorp Vault**: Enterprise secret management
- **Kubernetes Secrets**: Encrypted at rest

## Scaling Patterns

### Horizontal Pod Autoscaling
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-gateway-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-gateway
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

#### Resource Management
- **CPU/Memory Requests**: Guaranteed resource allocation
- **Limits**: Maximum resource usage prevention
- **Quality of Service**: Pod scheduling priorities

### Vertical Scaling
- **Resource Optimization**: Right-sizing containers
- **Performance Monitoring**: Usage pattern analysis
- **Cost Optimization**: Efficient resource utilization

## Helm Chart Structure

### Chart Organization
```
helm/
├── Chart.yaml          # Chart metadata
├── values.yaml         # Default configuration
├── templates/          # Kubernetes manifests
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   ├── secret.yaml
│   └── ingress.yaml
└── charts/             # Sub-charts
```

#### Values Configuration
```yaml
# Global settings
global:
  environment: production
  domain: b2connect.local

# Service-specific overrides
apiGateway:
  replicas: 3
  resources:
    requests:
      cpu: "500m"
      memory: "512Mi"
```

### Chart Dependencies
- **PostgreSQL**: Bitnami PostgreSQL chart
- **Redis**: Bitnami Redis chart
- **Ingress Controller**: NGINX or Traefik
- **Cert Manager**: TLS certificate management

## Networking & Ingress

### Ingress Configuration
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: b2connect-ingress
  annotations:
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
spec:
  tls:
  - hosts:
    - api.b2connect.com
    secretName: b2connect-tls
  rules:
  - host: api.b2connect.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: api-gateway
            port:
              number: 80
```

#### Load Balancing
- **External Load Balancer**: Cloud provider integration
- **Internal Load Balancing**: Service mesh traffic distribution
- **Global Load Balancing**: Multi-region traffic routing

### Service Mesh Integration

#### Istio Service Mesh
- **Traffic Management**: Intelligent routing and load balancing
- **Security**: mTLS encryption between services
- **Observability**: Distributed tracing and metrics

#### Linkerd Service Mesh
- **Lightweight**: Minimal resource overhead
- **Automatic mTLS**: Zero-configuration encryption
- **Dashboard**: Traffic visualization and debugging

## Storage Management

### Persistent Volumes
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: postgres-pvc
spec:
  accessModes:
    - ReadWriteOnce
  storageClassName: fast-ssd
  resources:
    requests:
      storage: 100Gi
```

#### Storage Classes
- **Standard**: Cost-effective general purpose
- **Fast SSD**: High-performance databases
- **Regional**: Cross-zone replication

### Backup & Recovery
- **Automated Backups**: Scheduled database dumps
- **Point-in-Time Recovery**: Continuous data protection
- **Disaster Recovery**: Cross-region failover

## Monitoring & Observability

### Prometheus Metrics
```yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: api-gateway-monitor
spec:
  selector:
    matchLabels:
      app: api-gateway
  endpoints:
  - port: metrics
    path: /metrics
```

#### Grafana Dashboards
- **Service Health**: Pod status and resource usage
- **Application Metrics**: Request rates and error rates
- **Infrastructure Monitoring**: Node and cluster health

### Logging Aggregation
- **Fluent Bit**: Log collection and forwarding
- **Elasticsearch**: Log storage and search
- **Kibana**: Log visualization and analysis

## Security Implementation

### Network Policies
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: api-gateway-policy
spec:
  podSelector:
    matchLabels:
      app: api-gateway
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          name: ingress-nginx
```

#### Pod Security Standards
- **Privileged**: Restricted access for security
- **Baseline**: Standard security controls
- **Restricted**: Maximum security constraints

### RBAC Configuration
- **Service Accounts**: Minimal required permissions
- **Role-Based Access**: Namespace and cluster-level controls
- **Admission Controllers**: Policy enforcement

## CI/CD Integration

### GitOps Workflows
- **ArgoCD**: Git-based continuous deployment
- **Flux**: Kubernetes native GitOps
- **Jenkins X**: Automated CI/CD pipelines

### Pipeline Stages
1. **Build**: Container image creation
2. **Test**: Automated testing in Kubernetes
3. **Deploy**: Progressive rollout to production
4. **Verify**: Post-deployment validation

## Troubleshooting Guide

### Common Issues
- **Pod Crashes**: Resource constraints or configuration errors
- **Service Unavailable**: Network policy or DNS issues
- **Slow Performance**: Resource limits or database bottlenecks

### Debugging Tools
- **kubectl**: Direct cluster interaction
- **k9s**: Terminal-based cluster management
- **Lens**: GUI for Kubernetes management
- **stern**: Multi-pod log tailing

## Performance Optimization

### Resource Tuning
- **Right-Sizing**: Optimal CPU and memory allocation
- **HPA Optimization**: Appropriate scaling thresholds
- **Pod Affinity**: Workload co-location for performance

### Network Optimization
- **Service Mesh**: Efficient service-to-service communication
- **Ingress Tuning**: Connection pooling and keep-alive
- **DNS Optimization**: Local DNS caching

## Cost Management

### Resource Optimization
- **Spot Instances**: Cost-effective compute resources
- **Auto-Scaling**: Demand-based resource allocation
- **Storage Optimization**: Appropriate storage classes

### Monitoring Costs
- **Usage Analytics**: Resource consumption tracking
- **Budget Alerts**: Cost threshold notifications
- **Optimization Recommendations**: Automated cost-saving suggestions

---

*This document is maintained by @DevOps. Last updated: 2025-12-31*