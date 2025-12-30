# ⚙️ DevOps Engineer - Documentation Guide

**Role:** DevOps Engineer | **P0 Components:** P0.3, P0.4, P0.5  
**Time to Read:** ~3 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als DevOps Engineer bist du verantwortlich für:
- **Network Segmentation** (VPC, Security Groups) - P0.4
- **Incident Response Infrastructure** (Monitoring, Alerting) - P0.3
- **Key Management Infrastructure** (Azure KeyVault) - P0.5
- **CI/CD Pipelines** (GitHub Actions)
- **Aspire Orchestration** (Service Discovery)

---

## 📚 Required Reading (P0)

### Week 1: Infrastructure Foundation

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Aspire Guide** | [architecture/ASPIRE_GUIDE.md](../architecture/ASPIRE_GUIDE.md) | 45 min |
| 2 | **Port Blocking Solution** | [PORT_BLOCKING_SOLUTION.md](../PORT_BLOCKING_SOLUTION.md) | 15 min |
| 3 | **Aspire Troubleshooting** | [ASPIRE_DASHBOARD_TROUBLESHOOTING.md](../ASPIRE_DASHBOARD_TROUBLESHOOTING.md) | 20 min |
| 4 | **Service Discovery** | [SERVICE_DISCOVERY.md](../SERVICE_DISCOVERY.md) | 20 min |
| 5 | **VS Code Aspire Config** | [architecture/VSCODE_ASPIRE_CONFIG.md](../architecture/VSCODE_ASPIRE_CONFIG.md) | 15 min |

### Week 2: Compliance Infrastructure

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 6 | **EU Compliance Roadmap** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 60 min |
| 7 | **Network Segmentation (P0.4)** | EU Roadmap §P0.4 | 30 min |
| 8 | **Incident Response (P0.3)** | EU Roadmap §P0.3 | 30 min |
| 9 | **Key Management (P0.5)** | EU Roadmap §P0.5 | 20 min |

### Week 3: CI/CD & Monitoring

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 10 | **GitHub Workflows** | [GITHUB_WORKFLOWS.md](../GITHUB_WORKFLOWS.md) | 20 min |
| 11 | **GitHub Pages** | [GITHUB_PAGES_SETUP.md](../GITHUB_PAGES_SETUP.md) | 15 min |
| 12 | **macOS CDP Fix** | [MACOS_CDP_PORTFIX.md](../MACOS_CDP_PORTFIX.md) | 10 min |

---

## 🔧 Your P0 Components

### P0.3: Incident Response (Week 5-6, with Security)
```
Effort: 45 hours (shared)
Your Tasks:
  ✅ Set up monitoring infrastructure (Prometheus/Grafana)
  ✅ Configure alerting (PagerDuty/Slack)
  ✅ SIEM integration (ELK/Splunk)
  ✅ Log aggregation
  
Acceptance:
  ✅ Alerts fire within 5 min of incident
  ✅ Logs centralized and searchable
  ✅ Runbooks documented
```

### P0.4: Network Segmentation (Week 3-4)
```
Effort: 40 hours
Your Tasks:
  ✅ VPC with 3 subnets (public, services, databases)
  ✅ Security Groups (principle of least privilege)
  ✅ Load Balancer (ALB) with TLS 1.3
  ✅ DDoS Protection (AWS Shield / Azure DDoS)
  ✅ WAF rules deployed
  
Acceptance:
  ✅ No direct internet access to databases
  ✅ mTLS between services
  ✅ Rate limiting enforced
```

### P0.5: Key Management (Week 7-8, with Security)
```
Effort: 20 hours (shared)
Your Tasks:
  ✅ Azure KeyVault provisioned
  ✅ Access policies configured
  ✅ Key rotation automation (Azure Functions)
  ✅ Audit logging for key access
  
Acceptance:
  ✅ No secrets in code/config
  ✅ Key rotation working (annual)
  ✅ Access logs available
```

---

## ⚡ Quick Commands

```bash
# Start Aspire orchestration
cd AppHost && dotnet run

# Kill stuck services
./scripts/kill-all-services.sh

# Check port status
./scripts/check-ports.sh

# Open Aspire Dashboard
open http://localhost:15500

# Run infrastructure tests
dotnet test --filter "Category=Infrastructure"

# Check service health
curl http://localhost:7002/health   # Identity
curl http://localhost:7005/health   # Catalog
```

---

## 🏗️ Infrastructure Architecture

```
┌─────────────────────────────────────────────┐
│ LOAD BALANCER (ALB)                         │
│ - DDoS Protection                           │
│ - WAF Rules                                 │
│ - TLS 1.3                                   │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│ PUBLIC SUBNET (10.0.1.0/24)                 │
│ - API Gateway (YARP)                        │
│ - Rate Limiting                             │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│ PRIVATE SUBNET - SERVICES (10.0.2.0/24)     │
│ - Identity (7002)                           │
│ - Catalog (7005)                            │
│ - CMS (7006)                                │
│ - mTLS between services                     │
└─────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────┐
│ PRIVATE SUBNET - DATA (10.0.3.0/24)         │
│ - PostgreSQL (5432)                         │
│ - Redis (6379)                              │
│ - Elasticsearch (9200)                      │
│ - No internet access!                       │
└─────────────────────────────────────────────┘
```

---

## 📊 Monitoring Checklist

| Metric | Tool | Threshold | Alert |
|--------|------|-----------|-------|
| CPU | Prometheus | > 70% | Slack |
| Memory | Prometheus | > 80% | Slack |
| API Latency | Grafana | > 500ms P95 | PagerDuty |
| Error Rate | Grafana | > 1% | PagerDuty |
| DDoS Attack | AWS Shield | Detected | PagerDuty + Email |
| Failed Logins | Custom | > 5/min/IP | Slack |

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Service Down | On-Call → DevOps Lead | < 15 min |
| Security Incident | Security Engineer | Immediate |
| Infrastructure Cost | Tech Lead → PO | < 24h |
| Network Issue | Cloud Provider Support | Per SLA |

---

## ✅ Definition of Done (DevOps)

Before marking any P0 component as complete:

- [ ] Infrastructure as Code (Terraform/Pulumi)
- [ ] Monitoring dashboards created
- [ ] Alerts configured
- [ ] Runbooks documented
- [ ] Disaster recovery tested
- [ ] Security review passed
- [ ] Cost estimate approved

---

**Next:** Start with [architecture/ASPIRE_GUIDE.md](../architecture/ASPIRE_GUIDE.md)
