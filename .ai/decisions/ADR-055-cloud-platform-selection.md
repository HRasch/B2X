---
docid: ADR-055
title: Cloud Platform Selection
status: Draft
created: 2026-01-10
---

# ADR-055: Cloud Platform Selection for B2X Production

**Status**: Draft  
**Created**: 2026-01-10  
**Decision Date**: TBD  
**Proposed by**: @CloudArchitect  
**Approvers**: @Architect, @TechLead, @ProductOwner

---

## Context

B2X is approaching production readiness and requires a cloud platform selection for:
- Multi-tenant SaaS hosting
- Multi-gateway architecture (Store, Admin, Management)
- PostgreSQL multi-tenancy (ADR-004)
- Elasticsearch search infrastructure
- .NET Aspire orchestration (ADR-003)
- Scalable, cost-effective operations

The decision will have long-term implications on:
- Development velocity (.NET/Aspire ecosystem alignment)
- Operational costs (managed services vs self-hosted)
- Compliance requirements (GDPR data residency)
- Technical capabilities (scaling, monitoring, DR)

---

## Decision Drivers

### Technical Requirements
- **Platform Compatibility**: Strong .NET 10 support, Aspire integration
- **Database Services**: Managed PostgreSQL with multi-tenant capabilities
- **Container Orchestration**: Kubernetes or managed container services
- **Search Integration**: Elasticsearch or equivalent managed service
- **Developer Experience**: CLI tools, IDE integration, local development
- **Observability**: Built-in monitoring, logging, tracing

### Business Requirements
- **Cost Efficiency**: Predictable pricing, cost optimization tools
- **Scalability**: Auto-scaling for multi-tenant SaaS
- **Geographic Coverage**: EU-first (GDPR), potential US expansion
- **Vendor Lock-in**: Minimize where possible, use open standards
- **Support & SLA**: Enterprise-grade support for production

### Compliance & Security
- **Data Residency**: EU data storage for GDPR compliance
- **Certifications**: ISO 27001, SOC 2, GDPR compliance
- **Network Security**: VPC, private endpoints, encryption
- **Identity Management**: Azure AD/Entra ID integration preferred

---

## Options Considered

### Option 1: Microsoft Azure (Recommended)

**Summary**: Microsoft Azure with native .NET/Aspire support

**Architecture**:
```
Azure Front Door (CDN/WAF)
    ↓
Azure Application Gateway (Load Balancer)
    ↓
Azure Kubernetes Service (AKS)
    ├─ Store Gateway
    ├─ Admin Gateway
    └─ Management Gateway
    ↓
Azure Database for PostgreSQL Flexible Server
Azure Cognitive Search (or self-hosted Elasticsearch)
Azure Blob Storage (media)
Azure Monitor + Application Insights
```

**Pros**:
- ✅ **Best .NET/Aspire Integration**: Native support, Aspire dashboard compatibility
- ✅ **Managed PostgreSQL**: Azure Database for PostgreSQL with high availability
- ✅ **EU Data Residency**: Multiple EU regions (West Europe, North Europe)
- ✅ **Cost Optimization**: Reserved instances, Azure Hybrid Benefit
- ✅ **Developer Experience**: Visual Studio integration, Azure CLI, strong tooling
- ✅ **Observability**: Application Insights deeply integrated with .NET
- ✅ **Identity**: Azure AD/Entra ID for enterprise authentication
- ✅ **Compliance**: ISO 27001, SOC 2, GDPR certified

**Cons**:
- ⚠️ **Vendor Lock-in**: Heavy Azure-specific services (Application Insights, Entra ID)
- ⚠️ **Learning Curve**: Azure-specific concepts (Resource Groups, ARM templates)
- ⚠️ **Search**: Azure Cognitive Search different from Elasticsearch (migration needed)

**Cost Estimate** (EU West, production):
- AKS Cluster: ~€150/month (3 nodes, B4ms)
- PostgreSQL Flexible Server: ~€120/month (GP_Standard_D2s_v3)
- Application Gateway: ~€100/month (Standard_v2)
- Blob Storage: ~€20/month (Hot tier, 100GB)
- Monitoring: ~€50/month (Application Insights)
- **Total**: ~€440/month (~€5,280/year)

---

### Option 2: Amazon Web Services (AWS)

**Summary**: AWS with broad service catalog and maturity

**Architecture**:
```
CloudFront (CDN)
    ↓
Application Load Balancer
    ↓
Amazon EKS (Elastic Kubernetes Service)
    ├─ Store Gateway
    ├─ Admin Gateway
    └─ Management Gateway
    ↓
Amazon RDS for PostgreSQL
Amazon OpenSearch Service (Elasticsearch)
Amazon S3 (media)
Amazon CloudWatch + X-Ray
```

**Pros**:
- ✅ **Mature Platform**: Largest cloud provider, extensive service catalog
- ✅ **Managed Elasticsearch**: Amazon OpenSearch (Elasticsearch fork)
- ✅ **Global Reach**: Most regions worldwide
- ✅ **Cost Tools**: Cost Explorer, Savings Plans, Spot Instances
- ✅ **Compliance**: ISO 27001, SOC 2, GDPR certified
- ✅ **Open Standards**: Less vendor lock-in than Azure

**Cons**:
- ❌ **.NET Experience**: Not as integrated as Azure for .NET workloads
- ❌ **Aspire Support**: Limited Aspire dashboard integration
- ❌ **Observability**: X-Ray requires more configuration vs Application Insights
- ⚠️ **Complexity**: Steeper learning curve, more configuration needed
- ⚠️ **Cost Transparency**: More complex pricing, harder to estimate

**Cost Estimate** (EU Frankfurt, production):
- EKS Cluster: ~€220/month (3 t3.medium nodes + control plane)
- RDS PostgreSQL: ~€150/month (db.t3.medium, Multi-AZ)
- Application Load Balancer: ~€25/month
- S3 Storage: ~€25/month (100GB)
- OpenSearch: ~€200/month (t3.small.search)
- Monitoring: ~€30/month (CloudWatch)
- **Total**: ~€650/month (~€7,800/year)

---

### Option 3: Google Cloud Platform (GCP)

**Summary**: GCP with strong Kubernetes and data analytics

**Architecture**:
```
Cloud CDN
    ↓
Cloud Load Balancing
    ↓
Google Kubernetes Engine (GKE)
    ├─ Store Gateway
    ├─ Admin Gateway
    └─ Management Gateway
    ↓
Cloud SQL for PostgreSQL
Elasticsearch (self-hosted on GKE)
Cloud Storage (media)
Cloud Monitoring + Cloud Trace
```

**Pros**:
- ✅ **Kubernetes Expertise**: GKE is industry-leading
- ✅ **Data Analytics**: Strong BigQuery, AI/ML capabilities
- ✅ **Open Source**: More open-source friendly
- ✅ **Pricing**: Sustained use discounts, per-second billing
- ✅ **Compliance**: ISO 27001, SOC 2, GDPR certified

**Cons**:
- ❌ **.NET Ecosystem**: Weakest .NET support among big three
- ❌ **Aspire Integration**: No native Aspire support
- ❌ **Market Share**: Smaller community, fewer resources
- ❌ **Managed Elasticsearch**: No managed Elasticsearch (requires self-hosting)
- ⚠️ **EU Presence**: Fewer EU regions than Azure/AWS

**Cost Estimate** (EU Belgium, production):
- GKE Cluster: ~€180/month (3 n1-standard-2 nodes)
- Cloud SQL PostgreSQL: ~€140/month (db-n1-standard-2)
- Load Balancer: ~€20/month
- Cloud Storage: ~€20/month (100GB)
- Elasticsearch (self-hosted): ~€100/month (additional nodes)
- Monitoring: ~€40/month
- **Total**: ~€500/month (~€6,000/year)

---

### Option 4: Hybrid/Multi-Cloud

**Summary**: Use best-of-breed services across multiple clouds

**Example**: Azure for .NET hosting + AWS for Elasticsearch

**Pros**:
- ✅ **Flexibility**: Choose best service per category
- ✅ **Vendor Independence**: Reduced lock-in

**Cons**:
- ❌ **Complexity**: Multiple consoles, billing, security models
- ❌ **Network Costs**: Cross-cloud data transfer expensive
- ❌ **Operational Overhead**: Managing multiple platforms
- ❌ **Not Recommended**: For current project scale

---

## Decision Matrix

| Criterion | Weight | Azure | AWS | GCP |
|-----------|--------|-------|-----|-----|
| .NET/Aspire Integration | 25% | 10 | 6 | 4 |
| Cost Efficiency | 20% | 9 | 7 | 8 |
| EU Data Residency | 15% | 10 | 9 | 7 |
| Managed PostgreSQL | 15% | 9 | 9 | 8 |
| Developer Experience | 10% | 10 | 7 | 6 |
| Observability | 10% | 10 | 7 | 7 |
| Elasticsearch Support | 5% | 7 | 10 | 6 |
| **Weighted Score** | | **9.1** | **7.5** | **6.6** |

**Scoring**: 1-10, where 10 is best

---

## Recommended Decision

### **Option 1: Microsoft Azure**

**Rationale**:
1. **Ecosystem Alignment**: Native .NET 10 and Aspire support aligns with current tech stack
2. **Cost Efficiency**: Lowest estimated cost (~€440/month) with strong optimization tools
3. **Developer Productivity**: Best developer experience for .NET teams
4. **GDPR Compliance**: Multiple EU regions, strong data residency controls
5. **Observability**: Application Insights deeply integrated with .NET applications
6. **Future-Proof**: Microsoft's investment in .NET and Aspire ensures long-term support

**Trade-offs Accepted**:
- Migrate from Elasticsearch to Azure Cognitive Search (requires data migration)
- Some Azure-specific lock-in (Application Insights, Entra ID)
- Alternative: Self-host Elasticsearch on AKS if Azure Cognitive Search insufficient

---

## Implementation Plan

### Phase 1: Infrastructure Setup (2 weeks)
- [ ] Create Azure subscription and resource groups
- [ ] Set up Azure AD/Entra ID tenants
- [ ] Configure networking (VNet, subnets, NSGs)
- [ ] Provision AKS cluster (development environment)

### Phase 2: Managed Services (2 weeks)
- [ ] Deploy Azure Database for PostgreSQL Flexible Server
- [ ] Configure Azure Blob Storage for media
- [ ] Set up Azure Application Gateway
- [ ] Evaluate Azure Cognitive Search vs self-hosted Elasticsearch

### Phase 3: CI/CD & Monitoring (1 week)
- [ ] Configure GitHub Actions for Azure deployment
- [ ] Set up Application Insights and Azure Monitor
- [ ] Implement log aggregation and alerting
- [ ] Configure cost monitoring and budgets

### Phase 4: Migration & Testing (2 weeks)
- [ ] Migrate development environment to Azure
- [ ] Load testing and performance tuning
- [ ] Security audit and compliance verification
- [ ] Disaster recovery testing

### Phase 5: Production Deployment (1 week)
- [ ] Production AKS cluster deployment
- [ ] Database migration and verification
- [ ] DNS cutover and traffic routing
- [ ] Post-deployment monitoring

---

## Consequences

### Positive
- ✅ Leverages Microsoft's .NET and Aspire expertise
- ✅ Strong developer productivity with familiar tools
- ✅ Cost-effective managed services reduce operational burden
- ✅ GDPR compliance out-of-the-box with EU regions
- ✅ Unified billing and management console

### Negative
- ⚠️ Azure-specific knowledge required for team
- ⚠️ Some vendor lock-in with Azure-specific services
- ⚠️ Elasticsearch migration to Azure Cognitive Search (if chosen)

### Risks & Mitigations
| Risk | Mitigation |
|------|------------|
| Azure outage impacts availability | Multi-region DR plan, Azure Front Door failover |
| Cost overruns | Budget alerts, monthly cost reviews, reserved instances |
| Team lacks Azure expertise | Training, Azure certifications, documentation |
| Vendor lock-in | Use Kubernetes (portable), open standards where possible |
| Cognitive Search insufficient | Option to self-host Elasticsearch on AKS |

---

## Alternative Scenarios

### Scenario A: US Expansion Required
**Action**: Deploy secondary region in Azure US (East US 2)  
**Cost Impact**: +€300/month for US region resources  
**Timeline**: 2-3 weeks for multi-region setup

### Scenario B: Cost Reduction Needed
**Actions**:
- Use Azure Spot Instances for non-production workloads
- Implement auto-scaling to reduce baseline capacity
- Use Azure Reserved Instances (1-year commitment: ~30% savings)
**Potential Savings**: ~€130/month (~30% reduction)

### Scenario C: Multi-Cloud Required (Regulatory)
**Action**: Hybrid approach with AWS as secondary provider  
**Complexity**: High - requires cross-cloud networking, duplicate infrastructure  
**Cost**: +40% operational overhead

---

## Open Questions

1. **Elasticsearch vs Azure Cognitive Search**: Requires POC to validate feature parity
2. **Azure Kubernetes Service vs Azure Container Apps**: AKS chosen for Aspire compatibility
3. **Budget Approval**: €5,280/year baseline cost requires ProductOwner approval
4. **Disaster Recovery SLA**: What RTO/RPO required? (Affects multi-region decision)
5. **Team Training**: Timeline and budget for Azure certifications?

---

## References

- [ADR-003] Aspire Orchestration
- [ADR-004] PostgreSQL Multitenancy
- [KB-TBD] Azure Services Guide
- [GL-008] Governance Policies (Cloud Architecture Decision Authority)
- Azure Pricing Calculator: https://azure.microsoft.com/pricing/calculator/
- .NET Aspire Azure Integration: https://learn.microsoft.com/dotnet/aspire/deployment/azure/

---

## Approval Tracking

| Approver | Status | Date | Comments |
|----------|--------|------|----------|
| @CloudArchitect | Proposed | 2026-01-10 | Initial draft |
| @Architect | Pending | - | Architecture review needed |
| @TechLead | Pending | - | Technical feasibility review |
| @ProductOwner | Pending | - | Budget approval required |
| @Security | Pending | - | Compliance verification |
| @DevOps | Pending | - | Implementation feasibility |

---

**Next Steps**:
1. Schedule review meeting with @Architect, @TechLead, @ProductOwner
2. Conduct Azure Cognitive Search vs Elasticsearch POC
3. Get budget approval for €5,280/year
4. Create detailed migration plan (Phase 1-5)
5. Update status to "Accepted" or "Rejected" after approval

---

**Maintained by**: @CloudArchitect  
**Last Updated**: 2026-01-10
