---
docid: ARCH-CLOUD-001
title: Cloud Infrastructure Architecture
owner: @CloudArchitect
status: Active
created: 2026-01-11
---

# Cloud Infrastructure Architecture

**DocID**: `ARCH-CLOUD-001`  
**Related**: [PROJECT_DEPENDENCY_GRAPH.md](PROJECT_DEPENDENCY_GRAPH.md) (Application Architecture)

---

## Overview

Dieses Dokument beschreibt die Cloud-Infrastruktur für B2XGate als Multi-Tenant SaaS-Plattform.

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart TB
    subgraph Internet["🌐 Internet"]
        Users["👥 Tenants & Users"]
        Admins["👨‍💼 Administrators"]
    end

    subgraph Edge["Edge Layer"]
        CDN["☁️ CDN<br/>Static Assets"]
        WAF["🛡️ WAF<br/>Web Application Firewall"]
        DDoS["🔒 DDoS Protection"]
    end

    subgraph LoadBalancing["Load Balancing Layer"]
        GLB["⚖️ Global Load Balancer<br/>DNS-based Routing"]
        RProxy["🔀 Reverse Proxy Cluster<br/>Traefik / YARP"]
    end

    subgraph Compute["Compute Layer"]
        subgraph K8s["Kubernetes Cluster"]
            GW["Gateway Pods"]
            Workers["Worker Pods"]
            MCP["MCP Server Pods"]
        end
    end

    subgraph Data["Data Layer"]
        subgraph Primary["Primary Region"]
            PG_P["🐘 PostgreSQL Primary"]
            Redis_P["📦 Redis Primary"]
            ES_P["🔍 Elasticsearch"]
        end
        subgraph Replica["Replica Region"]
            PG_R["🐘 PostgreSQL Replica"]
            Redis_R["📦 Redis Replica"]
        end
    end

    subgraph AI["AI Services"]
        OpenAI["🤖 Azure OpenAI"]
        LocalLLM["🧠 Local LLM<br/>Fallback"]
    end

    Users --> CDN
    Admins --> WAF
    CDN --> WAF
    WAF --> DDoS
    DDoS --> GLB
    GLB --> RProxy
    RProxy --> K8s
    GW --> Primary
    Workers --> Primary
    MCP --> AI
    Primary -.->|Replication| Replica

    classDef edge fill:#e1f5fe,stroke:#01579b
    classDef compute fill:#fff3e0,stroke:#e65100
    classDef data fill:#f3e5f5,stroke:#7b1fa2
    classDef ai fill:#fce4ec,stroke:#c2185b

    class CDN,WAF,DDoS,GLB,RProxy edge
    class K8s,GW,Workers,MCP compute
    class PG_P,Redis_P,ES_P,PG_R,Redis_R data
    class OpenAI,LocalLLM ai
```

---

## Deployment Topology

### Region Strategy

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart LR
    subgraph EU["🇪🇺 EU Region (Primary)"]
        EU_K8s["K8s Cluster<br/>3 Availability Zones"]
        EU_DB["PostgreSQL<br/>Primary + Standby"]
        EU_Cache["Redis Cluster<br/>3 Nodes"]
    end

    subgraph DR["🇩🇪 DR Region (Standby)"]
        DR_K8s["K8s Cluster<br/>Scaled Down"]
        DR_DB["PostgreSQL<br/>Async Replica"]
        DR_Cache["Redis<br/>Read Replica"]
    end

    EU_DB -->|"Async Replication<br/>< 1s lag"| DR_DB
    EU_Cache -->|"Cross-Region Sync"| DR_Cache
    EU_K8s -.->|"Failover"| DR_K8s

    classDef primary fill:#c8e6c9,stroke:#2e7d32
    classDef standby fill:#ffecb3,stroke:#ff8f00

    class EU_K8s,EU_DB,EU_Cache primary
    class DR_K8s,DR_DB,DR_Cache standby
```

### Availability Zones

| Component | Zone A | Zone B | Zone C |
|-----------|--------|--------|--------|
| Gateway Pods | 2 | 2 | 2 |
| Worker Pods | 3 | 3 | 3 |
| MCP Server Pods | 1 | 1 | 1 |
| PostgreSQL | Primary | Standby | - |
| Redis | Node 1 | Node 2 | Node 3 |
| Elasticsearch | Data 1 | Data 2 | Data 3 |

---

## High Availability

### Service Level Objectives (SLO)

| Metric | Target | Measurement |
|--------|--------|-------------|
| Availability | 99.9% | Monthly uptime |
| API Latency (p95) | < 200ms | Per request |
| API Latency (p99) | < 500ms | Per request |
| Error Rate | < 0.1% | 5xx responses |
| Recovery Time (RTO) | < 15 min | From incident |
| Data Loss (RPO) | < 1 min | Transaction loss |

### Redundancy Model

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart TB
    subgraph Stateless["Stateless Services (N+2)"]
        GW1["Gateway 1"]
        GW2["Gateway 2"]
        GW3["Gateway 3"]
        GWn["Gateway N"]
    end

    subgraph Stateful["Stateful Services (Active-Passive)"]
        DB_A["PostgreSQL<br/>Active"]
        DB_P["PostgreSQL<br/>Passive"]
    end

    subgraph Distributed["Distributed Services (Quorum)"]
        R1["Redis 1"]
        R2["Redis 2"]
        R3["Redis 3"]
    end

    LB["Load Balancer"] --> GW1 & GW2 & GW3 & GWn
    GW1 & GW2 & GW3 & GWn --> DB_A
    DB_A -->|"Sync Replication"| DB_P
    GW1 & GW2 & GW3 & GWn --> R1 & R2 & R3

    classDef active fill:#c8e6c9,stroke:#2e7d32
    classDef passive fill:#ffecb3,stroke:#ff8f00
    classDef distributed fill:#e1f5fe,stroke:#01579b

    class GW1,GW2,GW3,GWn,DB_A active
    class DB_P passive
    class R1,R2,R3 distributed
```

### Health Checks

```yaml
# Kubernetes Probes Configuration
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
  failureThreshold: 2

startupProbe:
  httpGet:
    path: /health/startup
    port: 8080
  initialDelaySeconds: 0
  periodSeconds: 5
  failureThreshold: 30
```

---

## Auto-Scaling Strategy

### Horizontal Pod Autoscaler (HPA)

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart LR
    subgraph Metrics["📊 Metrics"]
        CPU["CPU Usage"]
        Memory["Memory Usage"]
        RPS["Requests/sec"]
        Queue["Queue Depth"]
    end

    subgraph HPA["⚖️ HPA Controller"]
        Eval["Evaluate Metrics"]
        Scale["Scale Decision"]
    end

    subgraph Pods["🔲 Pod Replicas"]
        P1["Pod 1"]
        P2["Pod 2"]
        Pn["Pod N"]
    end

    CPU & Memory & RPS & Queue --> Eval
    Eval --> Scale
    Scale --> P1 & P2 & Pn

    classDef metrics fill:#fff3e0,stroke:#e65100
    classDef hpa fill:#e8f5e9,stroke:#2e7d32
    classDef pods fill:#e1f5fe,stroke:#01579b

    class CPU,Memory,RPS,Queue metrics
    class Eval,Scale hpa
    class P1,P2,Pn pods
```

### Scaling Configuration

| Service | Min | Max | CPU Target | Memory Target | Custom Metric |
|---------|-----|-----|------------|---------------|---------------|
| Store Gateway | 3 | 20 | 70% | 80% | RPS > 1000 |
| Admin Gateway | 2 | 10 | 70% | 80% | - |
| Management Gateway | 2 | 5 | 70% | 80% | - |
| MCP Servers | 2 | 8 | 60% | 70% | Queue > 100 |
| Workers | 3 | 30 | 80% | 85% | Jobs pending |

### Vertical Pod Autoscaler (VPA)

```yaml
# VPA für MCP Server (AI-intensive)
apiVersion: autoscaling.k8s.io/v1
kind: VerticalPodAutoscaler
metadata:
  name: mcp-server-vpa
spec:
  targetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: mcp-server
  updatePolicy:
    updateMode: Auto
  resourcePolicy:
    containerPolicies:
    - containerName: mcp-server
      minAllowed:
        cpu: 500m
        memory: 1Gi
      maxAllowed:
        cpu: 4
        memory: 8Gi
```

---

## Security Zones

### Network Segmentation

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart TB
    subgraph DMZ["🟡 DMZ (Public)"]
        LB["Load Balancer"]
        WAF["WAF"]
    end

    subgraph App["🟢 Application Zone"]
        GW["Gateways"]
        MCP["MCP Servers"]
    end

    subgraph Data["🔴 Data Zone (Restricted)"]
        DB["PostgreSQL"]
        Redis["Redis"]
        ES["Elasticsearch"]
    end

    subgraph Mgmt["🟣 Management Zone"]
        Bastion["Bastion Host"]
        Monitoring["Monitoring"]
        Secrets["Vault"]
    end

    Internet((Internet)) --> DMZ
    DMZ --> App
    App --> Data
    Mgmt -.->|"Admin Access"| App
    Mgmt -.->|"Admin Access"| Data

    classDef dmz fill:#fff9c4,stroke:#f57f17
    classDef app fill:#c8e6c9,stroke:#2e7d32
    classDef data fill:#ffcdd2,stroke:#c62828
    classDef mgmt fill:#e1bee7,stroke:#7b1fa2

    class LB,WAF dmz
    class GW,MCP app
    class DB,Redis,ES data
    class Bastion,Monitoring,Secrets mgmt
```

### Network Policies

| Source Zone | Target Zone | Allowed Ports | Protocol |
|-------------|-------------|---------------|----------|
| DMZ | Application | 8000, 8080, 8090 | HTTPS |
| Application | Data | 5432, 6379, 9200 | TCP |
| Application | AI Services | 443 | HTTPS |
| Management | All | 22, 443 | SSH/HTTPS |
| Data | Data | Cluster ports | TCP |

### Zero Trust Principles

1. **Identity Verification**: Alle Service-to-Service Calls via mTLS
2. **Least Privilege**: RBAC für alle Kubernetes Resources
3. **Micro-Segmentation**: Network Policies pro Namespace
4. **Continuous Verification**: JWT Token Validation bei jedem Request

---

## Secrets Management

### Architecture

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart LR
    subgraph Vault["🔐 HashiCorp Vault"]
        KV["KV Store"]
        PKI["PKI Engine"]
        Transit["Transit Engine"]
    end

    subgraph K8s["Kubernetes"]
        CSI["Secrets Store CSI"]
        Pods["Application Pods"]
    end

    subgraph Secrets["Secret Types"]
        DB_Creds["DB Credentials"]
        API_Keys["API Keys"]
        Certs["TLS Certificates"]
        Tokens["Service Tokens"]
    end

    KV --> DB_Creds & API_Keys & Tokens
    PKI --> Certs
    CSI --> Pods
    Vault --> CSI

    classDef vault fill:#fce4ec,stroke:#c2185b
    classDef k8s fill:#e1f5fe,stroke:#01579b
    classDef secrets fill:#fff3e0,stroke:#e65100

    class KV,PKI,Transit vault
    class CSI,Pods k8s
    class DB_Creds,API_Keys,Certs,Tokens secrets
```

### Secret Rotation

| Secret Type | Rotation Interval | Method |
|-------------|-------------------|--------|
| Database Passwords | 30 days | Vault Dynamic Secrets |
| API Keys | 90 days | Automated Rotation |
| TLS Certificates | 90 days | cert-manager + Let's Encrypt |
| Service Tokens | 24 hours | Short-lived JWT |
| Encryption Keys | 365 days | Manual with Audit |

---

## Observability Stack

### Three Pillars

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart TB
    subgraph Apps["Applications"]
        GW["Gateways"]
        MCP["MCP Servers"]
        Workers["Workers"]
    end

    subgraph Collect["Collection Layer"]
        OTel["OpenTelemetry<br/>Collector"]
    end

    subgraph Metrics["📊 Metrics"]
        Prom["Prometheus"]
        Grafana["Grafana"]
    end

    subgraph Logs["📝 Logs"]
        Loki["Loki"]
    end

    subgraph Traces["🔍 Traces"]
        Tempo["Tempo"]
    end

    subgraph Alerts["🚨 Alerting"]
        AM["AlertManager"]
        PD["PagerDuty"]
    end

    Apps -->|"OTLP"| OTel
    OTel --> Prom & Loki & Tempo
    Prom --> Grafana
    Loki --> Grafana
    Tempo --> Grafana
    Prom --> AM
    AM --> PD

    classDef apps fill:#c8e6c9,stroke:#2e7d32
    classDef collect fill:#fff3e0,stroke:#e65100
    classDef observe fill:#e1f5fe,stroke:#01579b
    classDef alert fill:#ffcdd2,stroke:#c62828

    class GW,MCP,Workers apps
    class OTel collect
    class Prom,Grafana,Loki,Tempo observe
    class AM,PD alert
```

### Key Metrics

#### Business Metrics
- Active Tenants / Orders per Hour / Revenue
- Cart Abandonment Rate / Conversion Rate

#### Technical Metrics
- Request Rate (RPS) / Error Rate / Latency (p50, p95, p99)
- CPU / Memory / Disk Usage
- Database Connections / Query Time
- Cache Hit Rate / Queue Depth

### Alerting Rules

| Alert | Condition | Severity | Response |
|-------|-----------|----------|----------|
| High Error Rate | 5xx > 1% for 5m | Critical | PagerDuty |
| High Latency | p99 > 2s for 10m | Warning | Slack |
| Pod CrashLoop | Restarts > 3 in 5m | Critical | PagerDuty |
| DB Connection Pool | Usage > 80% | Warning | Slack |
| Disk Space | Usage > 85% | Warning | Slack |
| Certificate Expiry | < 14 days | Warning | Email |

---

## Disaster Recovery

### Backup Strategy

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart TB
    subgraph Source["Production Data"]
        DB["PostgreSQL"]
        ES["Elasticsearch"]
        Files["Blob Storage"]
    end

    subgraph Backup["Backup Types"]
        Snap["Hourly Snapshots"]
        Daily["Daily Full Backup"]
        WAL["Continuous WAL<br/>Archiving"]
    end

    subgraph Storage["Backup Storage"]
        S3_1["S3 Bucket<br/>(Same Region)"]
        S3_2["S3 Bucket<br/>(Cross Region)"]
        Glacier["S3 Glacier<br/>(Long Term)"]
    end

    DB --> WAL --> S3_1
    DB --> Snap --> S3_1
    DB --> Daily --> S3_2
    ES --> Daily --> S3_2
    Files --> Daily --> S3_2
    S3_2 -->|"After 30 days"| Glacier

    classDef source fill:#c8e6c9,stroke:#2e7d32
    classDef backup fill:#fff3e0,stroke:#e65100
    classDef storage fill:#e1f5fe,stroke:#01579b

    class DB,ES,Files source
    class Snap,Daily,WAL backup
    class S3_1,S3_2,Glacier storage
```

### Recovery Procedures

| Scenario | RTO | RPO | Procedure |
|----------|-----|-----|-----------|
| Pod Failure | < 30s | 0 | Auto-restart by K8s |
| Node Failure | < 2m | 0 | Pod rescheduling |
| Zone Failure | < 5m | 0 | Cross-zone failover |
| Region Failure | < 15m | < 1m | DR region activation |
| Data Corruption | < 1h | < 1h | Point-in-time recovery |
| Total Loss | < 4h | < 24h | Full restore from backup |

### DR Runbook

```mermaid
%%{init: {'theme': 'base'}}%%
sequenceDiagram
    participant Alert as AlertManager
    participant OnCall as On-Call Engineer
    participant DR as DR System
    participant DNS as DNS Provider

    Alert->>OnCall: Region Failure Detected
    OnCall->>OnCall: Verify Incident
    OnCall->>DR: Initiate Failover
    DR->>DR: Scale up DR Cluster
    DR->>DR: Promote DB Replica
    DR->>DR: Update Config
    OnCall->>DNS: Update DNS Records
    DNS->>DNS: TTL Expiry (5m)
    DR-->>OnCall: Services Online
    OnCall->>Alert: Acknowledge Resolution
```

---

## Cost Optimization

### Resource Tiers

| Environment | Compute | Database | Purpose |
|-------------|---------|----------|---------|
| Production | Standard_D4s_v3 | Gen5 8 vCore | Full capacity |
| Staging | Standard_D2s_v3 | Gen5 4 vCore | Pre-production |
| Development | Standard_B2s | Gen5 2 vCore | Dev/Test |

### Cost Allocation by Tenant

```mermaid
%%{init: {'theme': 'base'}}%%
pie title Monthly Cost Distribution
    "Compute (K8s)" : 40
    "Database (PostgreSQL)" : 25
    "AI Services (OpenAI)" : 15
    "Storage & CDN" : 10
    "Monitoring & Logging" : 5
    "Network & Security" : 5
```

### Optimization Strategies

1. **Reserved Instances**: 1-3 Jahr Reservierungen für Base Load
2. **Spot Instances**: Worker Pods für batch processing
3. **Right-sizing**: VPA recommendations umsetzen
4. **Auto-shutdown**: Dev/Staging außerhalb Arbeitszeiten
5. **Data Lifecycle**: Cold storage für alte Logs

---

## Compliance & Governance

### Compliance Frameworks

| Framework | Scope | Status |
|-----------|-------|--------|
| GDPR | All EU tenant data | ✅ Compliant |
| ISO 27001 | Information Security | 🔄 In Progress |
| SOC 2 Type II | Security & Availability | 📋 Planned |
| PCI DSS | Payment Processing | ✅ via Provider |

### Data Residency

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart LR
    subgraph EU["🇪🇺 EU Data Center"]
        EU_Data["EU Tenant Data"]
    end

    subgraph US["🇺🇸 US Data Center"]
        US_Data["US Tenant Data"]
    end

    subgraph Global["🌍 Global"]
        CDN["CDN (Edge)"]
        Analytics["Anonymized<br/>Analytics"]
    end

    EU_Data -.->|"GDPR Compliant"| Analytics
    US_Data -.->|"Anonymized"| Analytics
    CDN --> EU & US

    classDef eu fill:#bbdefb,stroke:#1565c0
    classDef us fill:#c8e6c9,stroke:#2e7d32
    classDef global fill:#fff3e0,stroke:#e65100

    class EU_Data eu
    class US_Data us
    class CDN,Analytics global
```

---

## Infrastructure as Code

### Repository Structure

```
infrastructure/
├── terraform/
│   ├── modules/
│   │   ├── kubernetes/
│   │   ├── database/
│   │   ├── networking/
│   │   └── monitoring/
│   ├── environments/
│   │   ├── production/
│   │   ├── staging/
│   │   └── development/
│   └── global/
├── kubernetes/
│   ├── base/
│   ├── overlays/
│   │   ├── production/
│   │   ├── staging/
│   │   └── development/
│   └── charts/
└── ansible/
    ├── playbooks/
    └── roles/
```

### GitOps Workflow

```mermaid
%%{init: {'theme': 'base'}}%%
flowchart LR
    Dev["Developer"] -->|"PR"| Git["Git Repository"]
    Git -->|"Merge"| CI["CI Pipeline"]
    CI -->|"Build & Test"| Artifacts["Container<br/>Registry"]
    CI -->|"Update"| GitOps["GitOps Repo"]
    GitOps -->|"Sync"| ArgoCD["ArgoCD"]
    ArgoCD -->|"Deploy"| K8s["Kubernetes"]

    classDef human fill:#fff3e0,stroke:#e65100
    classDef vcs fill:#e1f5fe,stroke:#01579b
    classDef ci fill:#c8e6c9,stroke:#2e7d32
    classDef deploy fill:#f3e5f5,stroke:#7b1fa2

    class Dev human
    class Git,GitOps vcs
    class CI,Artifacts ci
    class ArgoCD,K8s deploy
```

---

## References

- [PROJECT_DEPENDENCY_GRAPH.md](PROJECT_DEPENDENCY_GRAPH.md) - Application Architecture
- [ADR-055](../../.ai/decisions/ADR-055-cloud-platform-selection.md) - Cloud Platform Selection
- [HOSTING_INFRASTRUCTURE.md](components/HOSTING_INFRASTRUCTURE.md) - Hosting Details
- [KB-070](../../.ai/knowledgebase/tools-and-tech/aspire-net10-compatibility.md) - Aspire Compatibility

---

**Last Updated**: 11. Januar 2026  
**Owner**: @CloudArchitect  
**Review Cycle**: Quarterly
