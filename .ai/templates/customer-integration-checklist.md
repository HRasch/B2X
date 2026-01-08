---
docid: TPL-006
title: Customer Integration Checklist
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Customer Integration Checklists
# Reference: ADR-038 Customer Integration Stages Framework
#
# Quick reference checklists for each integration stage
# Use with customer-integration-tracker.yml for detailed tracking

## Stage 1: EVALUATE - Quick Checklist

### Discovery
- [ ] Initial requirements call completed
- [ ] Use cases documented
- [ ] Feature requirements gathered

### Sandbox
- [ ] Trial tenant provisioned
- [ ] Demo catalog loaded
- [ ] API credentials shared
- [ ] Customer received access emails

### Assessment
- [ ] Product demo completed
- [ ] API playground tested
- [ ] Admin console walkthrough done
- [ ] Feature match score: ____%

### Decision
- [ ] Proposal sent
- [ ] Contract negotiated
- [ ] **Contract signed** ✓

---

## Stage 2: ONBOARD - Quick Checklist

### Provisioning
- [ ] Production tenant created
- [ ] Database provisioned
- [ ] Storage allocated
- [ ] CDN configured

### Configuration
- [ ] Timezone set
- [ ] Languages configured: _______
- [ ] Currencies configured: _______
- [ ] Tax settings defined
- [ ] Shipping zones configured

### Identity
- [ ] Admin users created (count: ___)
- [ ] Roles assigned
- [ ] SSO configured (provider: _______)
- [ ] MFA enabled for admins

### Domain
- [ ] Custom domain: _______________
- [ ] DNS records created
- [ ] DNS verified
- [ ] SSL certificate active
- [ ] Redirects configured

### Branding
- [ ] Logo uploaded
- [ ] Colors: Primary #______ Secondary #______
- [ ] Fonts configured
- [ ] Favicon set
- [ ] Email templates customized
- [ ] **Customer approved branding** ✓

### Content
- [ ] Homepage created
- [ ] About page created
- [ ] Contact page created
- [ ] FAQ page created

### Legal
- [ ] Privacy policy published
- [ ] Terms of service published
- [ ] Cookie consent configured
- [ ] Imprint added
- [ ] GDPR compliance verified
- [ ] DPA signed

### Verification
- [ ] Smoke tests passed
- [ ] Mobile verified
- [ ] Performance baseline set
- [ ] **Customer sign-off** ✓

---

## Stage 3: INTEGRATE - Quick Checklist

### Connector Deployment
- [ ] Connector downloaded (version: ___)
- [ ] Connector installed
- [ ] Connector configured
- [ ] Service registered
- [ ] Firewall rules set
- [ ] Health check passing

### Connection Test
- [ ] ERP connectivity verified
- [ ] Authentication working
- [ ] API permissions validated
- [ ] Latency: ___ms

### Initial Sync
- [ ] Article mapping defined
- [ ] Category mapping defined
- [ ] Catalog sync completed
  - Articles: ___
  - Categories: ___
- [ ] Images synced: ___
- [ ] Data quality validated

### Delta Sync
- [ ] Delta sync configured
- [ ] Schedule: __________
- [ ] Delta sync tested
- [ ] Conflict resolution set

### Customer Sync
- [ ] Customer mapping defined
- [ ] Customers imported: ___
- [ ] Customer groups configured
- [ ] Pricing tiers mapped
- [ ] B2B portals enabled (if applicable)

### Order Flow
- [ ] Order flow configured
- [ ] Order mapping defined
- [ ] Test order created
- [ ] Test order in ERP
- [ ] Status sync working
- [ ] Shipping updates flowing

### Payment Setup
- [ ] Gateway: __________
- [ ] Test payment processed
- [ ] Refund flow tested
- [ ] Invoice generation working
- [ ] Reconciliation configured

### Go-Live Readiness
- [ ] E2E flow tested
- [ ] UAT completed
- [ ] Load test passed
- [ ] Monitoring configured
- [ ] Runbook documented
- [ ] Support handover done
- [ ] **Go-live approved** ✓

---

## Stage 4: OPTIMIZE - Quick Checklist

### Stabilize (First 30 days)
- [ ] 30-day monitoring completed
- [ ] Issue backlog cleared
- [ ] Performance baseline documented
- [ ] Escalation paths tested

### Enhance
- [ ] Analytics enabled
- [ ] Search tuned
- [ ] Recommendations enabled
- [ ] Advanced pricing configured

### Scale
- [ ] CDN optimized
- [ ] Caching optimized
- [ ] Marketplace feeds: __________
- [ ] Multi-channel sync

### Innovate
- [ ] AI features evaluated
- [ ] Visual search enabled
- [ ] Custom integrations scoped
- [ ] Roadmap review scheduled

---

## Sign-Off Summary

| Stage | Completed | Signed By | Date |
|-------|-----------|-----------|------|
| Evaluate | ☐ | | |
| Onboard | ☐ | | |
| Integrate | ☐ | | |
| Go-Live | ☐ | | |

---

## Key Contacts

| Role | Name | Email | Phone |
|------|------|-------|-------|
| Customer PM | | | |
| Customer Technical | | | |
| B2X CSM | | | |
| B2X Technical | | | |

---

## Important Dates

| Milestone | Target | Actual |
|-----------|--------|--------|
| Contract signed | | |
| Onboarding complete | | |
| Integration complete | | |
| Go-live | | |
| 30-day review | | |

---

*Template version: 1.0 - ADR-038*
