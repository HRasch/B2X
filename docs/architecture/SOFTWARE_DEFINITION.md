# B2X - Software Definition

**Owner**: @software-architect  
**Last Updated**: 29. Dezember 2025  
**Status**: Active - COMPLETE  

⚠️ **GOVERNANCE NOTICE**: Only @software-architect can modify this document. Scope changes must be logged here with rationale and impact assessment. Changes require @product-owner approval and @software-architect documentation during quarterly reviews or when scope shifts are identified.

---

## 🎯 Vision

B2X is a **multi-tenant SaaS e-commerce platform** enabling German/EU businesses (B2C retailers, B2B wholesalers, digital merchants) to sell online with full regulatory compliance, integrated ERP connectivity, and AI-powered optimization.

---

## 📊 System Scope

### IN Scope ✅

**Core E-Commerce**:
- Multi-tenant store management (100+ independent shops)
- Product catalog with variants, pricing, inventory tracking
- Shopping cart & checkout (B2C & B2B flows)
- Order management & fulfillment
- Customer accounts & profiles

**Commerce Features**:
- Payment processing (Stripe, PayPal, SOFORT, Kreditkarte)
- Shipping integration (DHL, DPD, Hermes)
- Inventory synchronization
- Price management (dynamic pricing, promotions)
- Email notifications (order confirm, shipping, returns)

**Compliance & Legal**:
- VAT/GST handling (19% DE, per-country rates)
- Reverse charge for B2B EU
- Invoice generation & archiving (ZUGFeRD)
- GDPR data protection & encryption
- E-commerce directive compliance (PAngV, AStV)
- Cookie consent & tracking
- Accessibility (WCAG 2.1 AA)

**Admin Features**:
- Dashboard with KPIs (orders, revenue, conversions)
- Customer management & segmentation
- Analytics & reporting (sales, traffic, behavior)
- Email marketing integration
- Campaign management

**Operations**:
- ERP integration (SAP, Oracle, Shopware, custom APIs)
- Warehouse management system (WMS) sync
- Business intelligence (BI) dashboards
- Audit logging & compliance reports

### OUT of Scope ❌

- Physical inventory management (WMS is integrated, not built)
- Accounting software (data exported to external systems)
- Advanced AI/ML (basic fraud detection only)
- Mobile native apps (web-responsive only)
- POS systems (web-only)
- Multi-currency support (EUR primary)
- Subscription/recurring billing
- Marketplace features (single merchant per shop)

---

## 🏗️ Core Functions

### 1. Multi-Tenant Store Platform

**What**: Each shop (customer) operates independently with:
- Isolated data (customer data, orders, products, settings)
- Independent branding (domain, theme, logo)
- Separate billing & analytics
- Multi-user team access

**Why**: Enables 100+ shops on shared infrastructure while maintaining complete data isolation, independent scaling, and cost efficiency.

**Technical Implementation**:
- TenantId in every database table
- Global query filter: `Where(x => x.TenantId == tenantId)`
- Separate API gateways for Store & Admin
- Isolated Elasticsearch indices per tenant (for search)

---

### 2. Product Management

**What**: SKU-based catalog with:
- Product master data (name, description, images, SKU)
- Variants (size, color, material with price/inventory deltas)
- Pricing (base price, VAT, discounts, bulk rates)
- Inventory (stock levels, reservations, backorder tracking)
- Attributes (category, tags, SEO metadata)

**Why**: Retailers need flexible product structure to handle 100-10,000 SKUs per shop.

**Technical Implementation**:
- Product (parent) + ProductVariant (child) entities
- PriceCalculator service (handles VAT, discounts, bulk pricing)
- InventoryReservation for cart/order locking
- Elasticsearch for fast search/filtering

---

### 3. Checkout & Orders

**What**: Complete purchase flow supporting:
- Shopping cart with variants, quantities, discounts
- Guest & registered customer checkout
- Shipping address + billing address
- Payment methods (card, SEPA, instant transfer, PayPal)
- Order confirmation & tracking
- Returns & refunds workflow

**Why**: Conversion-critical feature. B2C needs fast checkout, B2B needs invoicing & terms.

**Technical Implementation**:
- Order aggregate with OrderLine items
- PaymentProcessor service (multiple providers)
- ShippingCalculator service (weight + destination)
- PriceBreakdown service (product + shipping + VAT)
- OrderState machine (pending → confirmed → shipped → delivered)

---

### 4. Compliance & Regulatory

**What**: Automated compliance across:
- **PAngV (Price Indication Ordinance)**: Show VAT separately on all prices
- **AStV (Distance Selling Ordinance)**: Collect proper shipping address, validate
- **ZUGFeRD (E-Invoice Standard)**: Generate invoices in ZUGFeRD format for B2B
- **GDPR**: Encrypt PII, audit logs, data deletion, consent management
- **NIS2**: Incident response, security monitoring
- **AI Act**: Log all AI-based decisions (fraud detection, recommendations)

**Why**: EU law requirements. Non-compliance = fines 1-4% revenue + liability.

**Technical Implementation**:
- IEncryptionService (AES-256-GCM for PII)
- AuditLogEntry (all CRUD operations logged, tamper-sealed)
- TaxCalculator service (per-country VAT rates)
- InvoiceGenerator service (ZUGFeRD XML output)
- ConsentManager (cookie + marketing consent tracking)

---

### 5. Analytics & Reporting

**What**: Dashboard showing:
- Sales KPIs (daily/monthly revenue, order count, AOV)
- Traffic metrics (visitors, sessions, bounce rate)
- Conversion funnel (product view → cart → checkout → order)
- Customer analytics (acquisition, lifetime value, churn)
- Inventory health (stock levels, turnover, stockouts)
- Marketing ROI (campaign performance, channel attribution)

**Why**: Retailers need data to optimize store performance.

**Technical Implementation**:
- Event stream (Wolverine messaging captures all events)
- Elasticsearch for analytics aggregation
- BI tool (Metabase/Looker) for dashboards
- Real-time dashboards (WebSocket push updates)

---

## 🔒 Critical Constraints

### Security Constraints
- ✅ **Encryption at Rest**: All PII encrypted with AES-256-GCM
- ✅ **Encryption in Transit**: TLS 1.3+ for all connections
- ✅ **Multi-Tenancy Isolation**: Complete data separation per tenant
- ✅ **Authentication**: JWT with 15-min token expiry
- ✅ **Authorization**: Role-based (admin, manager, customer)
- ✅ **Audit Trail**: Immutable logs of all data changes
- ✅ **Key Management**: Azure KeyVault, annual rotation

### Compliance Constraints
- ✅ **Data Residency**: EU data centers only (GDPR)
- ✅ **PII Retention**: Delete after 3 years (inactive accounts)
- ✅ **Invoice Retention**: 10 years (legal requirement)
- ✅ **Right to Forget**: GDPR data deletion within 30 days
- ✅ **Incident Notification**: <24h to authorities (NIS2)

### Performance Constraints
- ✅ **P95 Latency**: < 500ms for product list, < 1s for checkout
- ✅ **Availability**: 99.9% uptime SLA
- ✅ **Scalability**: Handle 2x traffic during peak (Black Friday)
- ✅ **Search**: < 100ms for product search

### Technical Constraints
- ✅ **Framework**: .NET 10 only (no legacy)
- ✅ **Database**: PostgreSQL 16 only (no NoSQL)
- ✅ **Messaging**: Wolverine (no other message brokers)
- ✅ **Frontend**: Vue.js 3 (Composition API)
- ✅ **Architecture**: DDD microservices with Onion pattern

### Deployment Constraints
- ✅ **Deployment Window**: Anytime safe (velocity-based, no fixed windows)
- ✅ **Downtime**: Zero downtime deployments required
- ✅ **Rollback**: < 5 min rollback capability
- ✅ **Database Migrations**: Blue-green deployments

---

## 📐 Key Assumptions

1. **Tenant Sizes**: 10-10,000 products per shop (not 1M+ SKU)
2. **Traffic Profile**: 80% during business hours (9 AM - 6 PM CET)
3. **Peak Load**: 2x average during sales events (Black Friday, Cyber Monday)
4. **Geographic**: EU-only customers, shipping to EU + UK
5. **Payment**: Credit cards + local payment methods (SEPA, iDEAL)
6. **Inventory**: Real-time sync with ERP/WMS (batch updates insufficient)
7. **Compliance**: Proactive (not reactive) - implement before required
8. **Team**: 4-6 engineers, 1 product manager, 1 DevOps
9. **Data Volume**: <100 GB in Year 1, <1 TB by Year 5
10. **Regulatory**: Will need to update for new EU laws (assume 1-2 per year)

---

## 🔗 External Dependencies

### Required Systems
- **ERP Systems**: SAP, Oracle, Shopware, custom APIs (for product/inventory sync)
- **Payment Processors**: Stripe, PayPal, SOFORT, Klarna
- **Shipping Providers**: DHL, DPD, Hermes, GLS APIs
- **Email Service**: SendGrid, Mailgun, or similar (transactional)
- **Search Engine**: Elasticsearch 8.x+ (product search)
- **Cache**: Redis 6.x+ (session, cart, rate limiting)
- **BI Tools**: Metabase, Looker (analytics dashboards)
- **CDN**: Cloudflare, Akamai (static assets, DDoS protection)

### Standards & Specifications
- **ZUGFeRD 2.x**: E-invoice XML standard
- **OpenID Connect**: Identity federation (if needed)
- **REST API**: Open standards (JSON, HTTP)
- **WebHooks**: For ERP/payment provider integrations

### Regulations & Laws
- **PAngV**: German Price Indication Ordinance
- **AStV**: Distance Selling Ordinance
- **GDPR**: General Data Protection Regulation
- **NIS2**: Network & Information Security Directive
- **AI Act**: EU regulation for high-risk AI systems
- **E-Commerce Directive**: Cross-border digital sales rules

---

## ⚡ Non-Functional Requirements

| Category | Requirement | Target |
|----------|-------------|--------|
| **Performance** | Product list P95 latency | < 500ms |
| **Performance** | Checkout P95 latency | < 1s |
| **Performance** | Search response time | < 100ms |
| **Performance** | Admin dashboard load | < 2s |
| **Availability** | System uptime SLA | 99.9% |
| **Availability** | Max unplanned downtime/year | 44 minutes |
| **Availability** | MTTR (Mean Time To Recover) | < 5 minutes |
| **Scalability** | Support peak load | 2x average traffic |
| **Scalability** | Concurrent users | 1,000 simultaneous |
| **Scalability** | Database size Year 1 | < 50 GB |
| **Scalability** | Database growth/year | < 50 GB |
| **Security** | Encryption | AES-256-GCM |
| **Security** | PII fields encrypted | 100% |
| **Security** | Audit trail coverage | All CRUD operations |
| **Security** | Authentication timeout | 15 minutes |
| **Compliance** | GDPR compliance | 100% |
| **Compliance** | Data deletion SLA | < 30 days |
| **Compliance** | Incident notification | < 24h |
| **Maintainability** | Code coverage | >= 80% |
| **Maintainability** | Documentation coverage | 100% of architecture |
| **Accessibility** | WCAG compliance | Level AA 2.1 |

---

## 🎯 Success Metrics

**B2X is successful when**:

✅ **100+ shops** operational (Year 1)  
✅ **<500ms** average product list response time  
✅ **>4%** e-commerce conversion rate (vs. 2% industry avg)  
✅ **<1%** unplanned downtime  
✅ **100% compliant** with PAngV, GDPR, NIS2, AI Act  
✅ **50K orders/month** by end of Year 1  
✅ **>90%** customer satisfaction (NPS)  
✅ **40min** team training time (onboard new engineer)  

---

## 📝 Notes

- **Regulatory Landscape**: EU laws change frequently. Budget for 20-40 hours/quarter for compliance updates.
- **Performance Budgeting**: As traffic grows, database queries will slow. Need quarterly optimization reviews.
- **Multi-Tenancy Complexity**: Highest risk. Implement strict isolation from day 1 (migrations, testing, monitoring).
- **ERP Integration**: Most variable. Each integration takes 40-80 hours. Plan for custom connector framework.

---

**Last Updated**: 29. Dezember 2025  
**Next Review**: 2026-03-29 (quarterly)  
**Owner**: @software-architect
