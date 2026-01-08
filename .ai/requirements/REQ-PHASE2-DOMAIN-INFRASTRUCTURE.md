---
docid: REQ-065
title: REQ PHASE2 DOMAIN INFRASTRUCTURE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Phase 2: DNS & SSL Infrastructure - Planning Document

**DocID**: `REQ-PHASE2-DNS-SSL`  
**Status**: Planning  
**Created**: 2. Januar 2026  
**Target Sprint**: Q1 2026  
**Owner**: @Architect, @DevOps

---

## Overview

Phase 2 implements automated DNS record management and SSL certificate provisioning to complete the Multi-Tenant Domain Management system defined in [ADR-022](../../.ai/decisions/ADR-022-multi-tenant-domain-management.md).

## Objectives

1. **Automated DNS Verification**: Background job to periodically verify pending domain DNS configurations
2. **DNS Record Automation**: Optional automated CNAME/A record creation for supported providers
3. **SSL Certificate Automation**: Automatic certificate issuance and renewal via Let's Encrypt or ACM
4. **Health Monitoring**: Domain health checks and alerting

---

## Technical Requirements

### 2.1 Background DNS Verification Service

**Purpose**: Periodically check pending domain verifications

```csharp
// Wolverine scheduled job
public class DnsVerificationJob
{
    [ScheduledJob("*/5 * * * *")] // Every 5 minutes
    public async Task CheckPendingVerifications(
        ITenantDomainRepository repository,
        IDnsVerificationService dnsService,
        IDomainLookupService lookupService)
    {
        // Get domains with pending verification
        // Verify each domain
        // Update status and invalidate cache
        // Send notifications
    }
}
```

**Acceptance Criteria**:
- [ ] Job runs every 5 minutes
- [ ] Max 10 verification attempts before marking as Failed
- [ ] Exponential backoff between retries
- [ ] Email notification on success/failure
- [ ] Metrics for verification success rate

### 2.2 DNS Provider Integration

**Supported Providers (Priority Order)**:
1. **Cloudflare** - Primary recommendation
2. **AWS Route 53** - For AWS customers
3. **Manual** - DNS instructions only (current)

**Interface**:
```csharp
public interface IDnsProvider
{
    string ProviderName { get; }
    Task<bool> CreateCnameRecordAsync(string domain, string target, CancellationToken ct);
    Task<bool> CreateTxtRecordAsync(string domain, string name, string value, CancellationToken ct);
    Task<bool> DeleteRecordAsync(string domain, string recordId, CancellationToken ct);
    Task<DnsRecordStatus> CheckRecordStatusAsync(string domain, string recordType, CancellationToken ct);
}
```

**Cloudflare Implementation**:
```csharp
public class CloudflareDnsProvider : IDnsProvider
{
    private readonly HttpClient _httpClient;
    private readonly CloudflareOptions _options;
    
    // Implementation using Cloudflare API v4
}
```

**Configuration**:
```json
{
  "Tenancy": {
    "DnsProvider": "Cloudflare",
    "Cloudflare": {
      "ApiToken": "${CLOUDFLARE_API_TOKEN}",
      "ZoneId": "${CLOUDFLARE_ZONE_ID}"
    },
    "Route53": {
      "HostedZoneId": "${ROUTE53_HOSTED_ZONE_ID}"
    }
  }
}
```

**Acceptance Criteria**:
- [ ] Cloudflare API integration working
- [ ] Route 53 API integration working
- [ ] Fallback to manual mode if no provider configured
- [ ] API credentials stored in Azure Key Vault / AWS Secrets Manager
- [ ] Rate limiting to respect API quotas

### 2.3 SSL Certificate Automation

**Options**:
1. **Let's Encrypt** via ACME protocol (cert-manager / Caddy)
2. **AWS ACM** for AWS-hosted infrastructure
3. **Cloudflare SSL** (easiest with Cloudflare proxy)

**Recommended: Cloudflare Flexible/Full SSL**
- Simplest setup when using Cloudflare as proxy
- Auto-renewing certificates
- No server-side certificate management needed

**Alternative: cert-manager (Kubernetes)**
```yaml
apiVersion: cert-manager.io/v1
kind: Certificate
metadata:
  name: tenant-domain-cert
spec:
  secretName: tenant-domain-tls
  issuerRef:
    name: letsencrypt-prod
    kind: ClusterIssuer
  dnsNames:
    - "*.B2X.de"
    - "custom-domain.com"
```

**Acceptance Criteria**:
- [ ] SSL certificates auto-provisioned for new domains
- [ ] Certificate renewal before expiration
- [ ] Monitoring for certificate expiry
- [ ] Fallback to HTTP for development environments

### 2.4 Domain Health Monitoring

**Health Checks**:
- DNS resolution working
- SSL certificate valid
- HTTP response from domain
- Application health endpoint responding

**Implementation**:
```csharp
public class DomainHealthService
{
    public async Task<DomainHealth> CheckHealthAsync(string domain)
    {
        return new DomainHealth
        {
            DnsStatus = await CheckDnsAsync(domain),
            SslStatus = await CheckSslAsync(domain),
            HttpStatus = await CheckHttpAsync(domain),
            AppStatus = await CheckAppHealthAsync(domain),
            CheckedAt = DateTime.UtcNow
        };
    }
}
```

**Alerting**:
- Slack/Teams notification for domain issues
- Email to tenant admin for SSL expiry warnings
- Dashboard in Management UI

**Acceptance Criteria**:
- [ ] Health check runs every 15 minutes
- [ ] Alerts within 5 minutes of issue detection
- [ ] Health history stored for 30 days
- [ ] Dashboard shows domain health status

---

## Infrastructure Changes

### 2.5 Reverse Proxy Configuration

**Current**: nginx/Traefik with wildcard `*.B2X.de`

**Required Changes**:
1. Dynamic upstream configuration for custom domains
2. SSL certificate injection for custom domains
3. Host-based routing to tenant applications

**Traefik IngressRoute Example**:
```yaml
apiVersion: traefik.containo.us/v1alpha1
kind: IngressRoute
metadata:
  name: tenant-custom-domain
spec:
  entryPoints:
    - websecure
  routes:
    - match: Host(`custom-domain.com`)
      kind: Rule
      services:
        - name: store-gateway
          port: 80
  tls:
    secretName: custom-domain-tls
```

### 2.6 Database Schema Changes

```sql
-- Add health monitoring columns
ALTER TABLE tenant_domains ADD COLUMN last_health_check TIMESTAMP;
ALTER TABLE tenant_domains ADD COLUMN health_status VARCHAR(20);
ALTER TABLE tenant_domains ADD COLUMN health_details JSONB;

-- Add DNS provider tracking
ALTER TABLE tenant_domains ADD COLUMN dns_provider VARCHAR(50);
ALTER TABLE tenant_domains ADD COLUMN dns_record_id VARCHAR(100);

-- Add SSL tracking
ALTER TABLE tenant_domains ADD COLUMN ssl_expires_at TIMESTAMP;
ALTER TABLE tenant_domains ADD COLUMN ssl_issuer VARCHAR(100);
```

---

## Security Considerations

### 2.7 API Security

- [ ] DNS provider API keys stored in secrets manager
- [ ] API calls audited in logs
- [ ] Rate limiting on domain operations
- [ ] Validation of domain ownership before DNS changes

### 2.8 Domain Takeover Prevention

- [ ] Verify domain ownership via TXT record before CNAME creation
- [ ] Monitor for dangling DNS records
- [ ] Alert on unexpected DNS changes
- [ ] Regular audit of configured domains

---

## Estimated Effort

| Task | Agent | Story Points | Priority |
|------|-------|--------------|----------|
| DNS Verification Background Job | @Backend | 3 | P1 |
| Cloudflare DNS Provider | @Backend | 5 | P1 |
| AWS Route 53 DNS Provider | @Backend | 5 | P2 |
| SSL Certificate Automation | @DevOps | 8 | P1 |
| Domain Health Service | @Backend | 3 | P2 |
| Health Monitoring Dashboard | @Frontend | 5 | P2 |
| Traefik Dynamic Configuration | @DevOps | 5 | P1 |
| Database Migrations | @Backend | 2 | P1 |
| Security Audit | @Security | 3 | P1 |
| **Total** | | **39 SP** | |

---

## Dependencies

### External Dependencies
- Cloudflare API access (API Token with DNS edit permissions)
- AWS credentials (for Route 53)
- cert-manager installed in Kubernetes cluster
- Traefik with CRD support

### Internal Dependencies
- Phase 1 Complete (Tenant & Domain CRUD) ✅
- Kubernetes cluster ready
- Secrets management configured

---

## Rollout Plan

### Week 1-2: Core Infrastructure
- [ ] DNS Verification Background Job
- [ ] Cloudflare DNS Provider integration
- [ ] Database migrations

### Week 3-4: SSL & Proxy
- [ ] SSL certificate automation
- [ ] Traefik dynamic configuration
- [ ] Integration testing

### Week 5-6: Monitoring & Polish
- [ ] Health monitoring service
- [ ] Management UI dashboard
- [ ] Security audit
- [ ] Documentation

---

## Success Metrics

| Metric | Target |
|--------|--------|
| Domain verification time | < 10 minutes |
| SSL provisioning time | < 5 minutes |
| Health check false positive rate | < 1% |
| Domain uptime | 99.9% |
| Certificate renewal success rate | 100% |

---

## Open Questions

1. **Cloudflare vs self-managed SSL**: Should we require Cloudflare proxy for custom domains to simplify SSL?
2. **DNS propagation delay**: How to handle DNS propagation delays (up to 48 hours)?
3. **Subdomain wildcards**: Should tenants be able to create `*.tenant.B2X.de` subdomains?
4. **Custom domain limits**: Maximum custom domains per tenant?

---

## References

- [ADR-022: Multi-Tenant Domain Management](../../.ai/decisions/ADR-022-multi-tenant-domain-management.md)
- [Cloudflare API Documentation](https://developers.cloudflare.com/api/)
- [AWS Route 53 API](https://docs.aws.amazon.com/Route53/latest/APIReference/)
- [cert-manager Documentation](https://cert-manager.io/docs/)
- [Traefik IngressRoute](https://doc.traefik.io/traefik/routing/providers/kubernetes-crd/)

---

**Next Steps**:
1. Review with @Architect for infrastructure decisions
2. Review with @DevOps for Kubernetes configuration
3. Review with @Security for security assessment
4. Estimate and schedule in upcoming sprint

---

*Agents: @Architect, @Backend, @DevOps, @Security | Owner: @SARAH*
