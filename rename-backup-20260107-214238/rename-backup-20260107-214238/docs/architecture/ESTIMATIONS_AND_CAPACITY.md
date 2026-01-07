# B2X - Estimations & Capacity

**Owner**: @software-architect  
**Last Updated**: 29. Dezember 2025  
**Status**: Active - BASELINE ESTABLISHED  

⚠️ **GOVERNANCE NOTICE**: Only @software-architect can modify this document. Changes must occur during issue review when development starts (not mid-sprint). All estimation changes must be approved by @software-architect and logged in commit message.

---

## 📊 Current Scale (Year 1 Baseline) - STARTUP PHASE

### User & Data Volume

| Metric | Target | Status |
|--------|--------|--------|
| **Active Shops** | 50 | MVP/Startup phase |
| **Parallel Users** | 150-200 | Peak concurrent (3-4 users/shop) |
| **Products** | 5,000 - 50,000 | Total platform (100-1000 SKUs/shop) |
| **Monthly Orders** | 10,000 - 20,000 | Total platform |
| **Customers** | 5,000 - 10,000 | Mix of repeat + new |
| **Total Database Size** | 5 - 10 GB | Single PostgreSQL instance |

### Performance Targets

| Metric | Target | Notes |
|--------|--------|-------|
| **Product List (P95)** | < 200ms | Single database, indexed |
| **Checkout (P95)** | < 500ms | Acceptable for startup |
| **Search (P95)** | < 200ms | PostgreSQL FTS, no Elasticsearch |
| **Admin Dashboard (P95)** | < 1s | Simple aggregations |
| **API Upstream (P99)** | < 3s | ERP integration (acceptable) |
| **Auth Token Validation** | < 20ms | JWT token validation |

### Availability & Reliability

| Metric | Target | Calculation |
|--------|--------|-------------|
| **SLA Uptime** | 99.5% | 3.6 hours/year downtime (startup acceptable) |
| **MTTR (Mean Time to Recover)** | < 15 min | Rollback + manual intervention |
| **Planned Downtime/Month** | < 4 hours | Maintenance windows (off-peak) |
| **Unplanned Incidents/Month** | < 3 | Expected during growth phase |

---

## 📈 Growth Projections

### Multi-Year Capacity Planning (Startup Growth Path)

```
YEAR 1 (Current - STARTUP):
├── Shops: 50
├── Parallel Users: 150-200
├── Products: 5,000 - 50,000 (100-1000 SKUs/shop)
├── Orders/Month: 10,000 - 20,000
├── Customers: 5,000 - 10,000
├── Storage: 5 - 10 GB
├── Infrastructure Cost: ~$300/month
└── Per Shop Cost: $6/month

YEAR 2 (Projected - 5x Growth):
├── Shops: 250 (50 × 5x)
├── Parallel Users: 400-500
├── Products: 25,000 - 250,000 (scaled range)
├── Orders/Month: 50,000 - 100,000 (5x growth)
├── Customers: 25,000 - 50,000
├── Storage: 30 - 50 GB (proportional growth)
├── Infrastructure Cost: ~$1,130/month
└── Per Shop Cost: $4.52/month (improving)

YEAR 3 (Projected - 2x Growth from Y2):
├── Shops: 500 (250 × 2x)
├── Parallel Users: 800-1000
├── Products: 50,000 - 500,000 (2x from Y2)
├── Orders/Month: 100,000 - 200,000 (2x growth)
├── Customers: 50,000 - 100,000
├── Storage: 50 - 100 GB (continued growth)
├── Infrastructure Cost: ~$2,700/month
└── Per Shop Cost: $5.40/month (economies of scale)
```

### Growth Rationale

- **5x growth Year 1→Y2**: Early adoption, word-of-mouth, successful merchants expanding
- **2x growth Year 2→Y3**: Market consolidation, focus on SMB retention and loyalty
- **Parallel users scaling**: 50 shops × 3-4 users/shop = 150-200 baseline (Y1)
- **Variable product catalog**: SMB shops (100-1000 SKUs), mid-market (5K-50K SKUs)
- **Order volume**: 10-20K/month Y1 = ~500/day, ~0.02 orders/second baseline
- **Peak traffic**: 2-3x during promotional periods (manageable on single instance)

### Storage Breakdown (Year 1 = 5-10 GB - STARTUP)

| Component | Size | Growth Per Year |
|-----------|------|-----------|
| **Product Data** | 0.5 GB | +0.2 GB (50 shops × 100-1000 SKUs) |
| **Order History** | 1.5 GB | +1.5 GB (10-20K orders/month) |
| **Customer Data** | 0.2 GB | +0.1 GB (5-10K customers, encrypted) |
| **Logs** | 2 GB | +1 GB (1-year retention) |
| **Search Index** | 0.3 GB | +0.1 GB (PostgreSQL FTS only) |
| **Cache** | 0.05 GB | Ephemeral (Redis in-memory) |
| **Backups** | 3 × 5 GB | Daily snapshots, auto-cleanup |
| **Total** | **5-10 GB** | **+3 GB/year** |

---

## 🖥️ Infrastructure & Compute Requirements

### Development Environment (Local)

| Resource | Required | Justification |
|----------|----------|---------------|
| **RAM** | 8 GB | 5 services × 1.5 GB + OS + IDE |
| **CPU** | 4 cores | Compile + run + database |
| **Storage** | 10 GB | Code + databases + node_modules |
| **PostgreSQL** | 5 GB | Local dev databases |

**Developer Setup Time**: 30 minutes (git clone + dotnet run)

### Staging Environment

| Component | Spec | Cost |
|-----------|------|------|
| **Compute** | 2 vCPU, 4 GB RAM | ~$50/month |
| **Database** | PostgreSQL 5 GB | Included in compute |
| **Cache** | Redis 1 GB | ~$20/month |
| **Storage** | 50 GB SSD | ~$5/month |
| **CDN** | Cloudflare free tier | $0 |
| **Monthly Total** | | ~$75/month |

### Production Environment (Year 1 - 50 Shops Startup)

| Component | Spec | Cost | Notes |
|-----------|------|------|-------|
| **Compute (API)** | 2 vCPU, 4 GB RAM (single instance) | ~$100/month | All services on single server |
| **Database Primary** | PostgreSQL 2vCPU, 4GB RAM, 20 GB SSD | ~$50/month | All databases in single instance |
| **Database Replicas** | None | $0 | Add in Year 2 when HA needed |
| **Cache** | Redis 1 GB (single instance) | ~$30/month | Session cache only |
| **Search** | PostgreSQL FTS (built-in) | $0 | No Elasticsearch needed |
| **Storage** | 5-10 GB + 30 GB backups | ~$20/month | Daily snapshots |
| **CDN/WAF** | Cloudflare Free Tier | $0-20/month | Sufficient for startup |
| **Monitoring** | Cloud provider built-in | ~$50/month | Basic logging + alerts |
| **Email** | SendGrid Free/Starter | ~$50/month | 10-20K orders/month |
| **Monthly Total** | | **~$300/month** | Minimal viable infrastructure |

**Annual Cost Year 1**: ~$3,600 (not including team)  
**Per Shop/Month**: $6  
**Per Order**: $0.15-0.30 (startup pricing)

### Production Environment (Year 2 & Year 3 Projections)

#### Year 2 (250 Shops - 5x Growth)

| Component | Spec | Cost | Notes |
|-----------|------|------|-------|
| **Compute (API)** | 4 vCPU, 8 GB RAM (2 instances, HA) | ~$400/month | Horizontal scaling for redundancy |
| **Database Primary** | PostgreSQL 4vCPU, 8GB RAM, 50 GB SSD | ~$150/month | Scale up from Y1 |
| **Database Replica** | 2vCPU, 4GB RAM | ~$80/month | Redundancy + read scaling |
| **Cache** | Redis 5 GB (2-node cluster) | ~$100/month | Distributed sessions |
| **Search** | PostgreSQL FTS (sufficient) | $0 | Consider Elasticsearch in Y3 |
| **Storage** | 30-50 GB + 150 GB backups | ~$100/month | Growing data |
| **CDN/WAF** | Cloudflare Pro | ~$50/month | DDoS + performance |
| **Monitoring** | Application Insights | ~$150/month | Better observability |
| **Email** | SendGrid Standard Plan | ~$100/month | 50-100K orders/month |
| **Monthly Total** | | **~$1,130/month** | Scaling infrastructure |

**Annual Cost Year 2**: ~$13,500  
**Per Shop/Month**: $4.52 (improving)  
**Per Order**: $0.13-0.27 (improving)

#### Year 3 (500 Shops - 2x Growth from Y2)

| Component | Spec | Cost | Notes |
|-----------|------|------|-------|
| **Compute (API)** | 8 vCPU, 16 GB RAM (4 instances, load-balanced) | ~$800/month | Full horizontal scaling |
| **Database Primary** | PostgreSQL 8vCPU, 16GB RAM, 100 GB SSD | ~$300/month | Growing order volume |
| **Database Replicas** | 2 × (4vCPU, 8GB RAM) | ~$300/month | HA + read scaling |
| **Cache** | Redis 20 GB (multi-node cluster) | ~$300/month | Distributed sessions |
| **Search** | Elasticsearch (optional, 2 nodes) | ~$200/month | Complex searches at scale |
| **Storage** | 50-100 GB + 300 GB backups | ~$200/month | Continued growth |
| **CDN/WAF** | Cloudflare Pro + origin shield | ~$100/month | Performance optimization |
| **Monitoring** | Application Insights + analytics | ~$300/month | Full observability |
| **Email** | SendGrid Pro Plan | ~$200/month | 100-200K orders/month |
| **Monthly Total** | | **~$2,700/month** | Production-ready infrastructure |

**Annual Cost Year 3**: ~$32,400  
**Per Shop/Month**: $5.40 (improving margins)  
**Per Order**: $0.16-0.27 (better efficiency)

---

## 👥 Team & Staffing (Startup Scale)

### Current Team (Year 1 - MVP Phase)

| Role | FTE | Allocation | Notes |
|------|-----|-----------|-------|
| **Backend Engineer** | 1 | 40h/week | Core API, microservices |
| **Frontend Engineer** | 1 | 40h/week | Store + Admin UIs |
| **DevOps/Infra** | 0.5 | 20h/week | AWS/Azure operations, CI/CD |
| **Product Manager** | 1 | 30h/week | Product roadmap, requirements |
| **QA/Testing** | 0.5 | 20h/week | Manual + basic automation |
| **Total** | **4 FTE** | **150h/week** | Lean startup team |

**Year 1 Cost**: ~$280,000-350,000 (assuming $70-85k per FTE)  
**Infrastructure**: ~$3,600/year  
**Total**: ~$283,600-353,600/year

### Growth Plan (Year 2 - 250 Shops)

| Role | Year 1 | Year 2 | Justification |
|------|--------|--------|---------------|
| **Backend** | 1 | 2 | Second backend for parallel development |
| **Frontend** | 1 | 1.5 | Focus on UX/polish, add contractor for specific features |
| **DevOps** | 0.5 | 1 | Full-time ops needed, multi-region planning |
| **QA** | 0.5 | 1 | Automation growing, compliance testing |
| **Product** | 1 | 1 | Same PM, more complex prioritization |
| **Security** | 0 | 0.5 | Part-time security review |
| **Total** | **4** | **7** | ~280h/week |

**Year 2 Cost**: ~$490,000-560,000  
**Infrastructure**: ~$13,500/year  
**Total**: ~$503,500-573,500/year

### Further Growth (Year 3 - 500 Shops)

| Role | Year 2 | Year 3 | Justification |
|------|--------|--------|---------------|
| **Backend** | 2 | 3 | Specialization (Catalog, Orders, Platform) |
| **Frontend** | 1.5 | 2 | Store UX focus + Admin enhancements |
| **DevOps** | 1 | 1.5 | Infrastructure at scale, cost optimization |
| **QA** | 1 | 1.5 | Load testing, multi-region validation |
| **Product** | 1 | 1 | Same PM, market validation |
| **Security** | 0.5 | 1 | GDPR/compliance audits, security reviews |
| **Total** | **7** | **10** | ~400h/week |

**Year 3 Cost**: ~$700,000-800,000  
**Infrastructure**: ~$32,400/year  
**Total**: ~$732,400-832,400/year

---

## 📊 Database Performance Estimates (Startup Scale)

### Query Performance (Expected P95 at 50 Shops)

| Operation | Scale | Expected Time | Notes |
|-----------|-------|----------------|-------|
| **Product Search** | 50K products | < 200ms | PostgreSQL FTS, indexed |
| **Product List** | 100 products/page | < 100ms | Single database, indexed |
| **Customer Orders** | 10 orders/customer | < 50ms | Simple query |
| **Create Order** | Transaction | < 250ms | Write + event notification |
| **Daily Report** | 1 month sales (10-20K) | < 2s | Single table aggregation |
| **Bulk Import** | 1,000 SKUs | < 5s | Simple insert batch |
| **Login** | JWT generation | < 50ms | Token caching |
| **Inventory Update** | Broadcast to 50 shops | < 50ms | Event-driven update |

### Database Scaling Strategy (Startup Growth Path)

**Year 1** (5-10 GB, 50 shops):
```
Single PostgreSQL instance
├── All databases in one instance (simpler)
├── RAM: 4 GB (plenty for working set)
├── CPU: 2 vCPU (lightly loaded)
├── Storage: 20 GB SSD (with growth buffer)
├── Backups: Daily snapshots
└── Cost: ~$50/month
```

**Year 2** (30-50 GB, 250 shops):
```
Scaled PostgreSQL with read replica
├── Primary: 4 vCPU, 8 GB RAM, 50 GB SSD
├── Read Replica: 2 vCPU, 4 GB RAM (for reports)
├── Connection pooling with PgBouncer
├── Regular backups + point-in-time recovery
└── Cost: ~$230/month (primary + replica)
```

**Year 3** (50-100 GB, 500 shops):
```
Multi-instance architecture
├── Primary: 8 vCPU, 16 GB RAM, 100 GB SSD (transactions)
├── Read Replicas: 2 × (4 vCPU, 8 GB RAM) (reporting, analytics)
├── Connection pooling with PgBouncer
├── Elasticsearch for complex search (optional)
├── Continuous backup with WAL archiving
└── Cost: ~$800/month (distributed)
```

### Connection Pool Management

| Configuration | Value | Rationale |
|---------------|-------|-----------|
| **Max Connections** | 200 | 50 shops × 3-4 users/shop max |
| **Pool Size per Service** | 20-30 | PgBouncer multiplexing |
| **Idle Timeout** | 300 sec | Auto-close unused connections |
| **Prepared Statements** | Pre-cached | Performance + SQL injection prevention |
| **Pooling Tool** | PgBouncer | External connection manager (Y2+) |
├── Read Replicas: 2 (Catalog, Orders)
├── CPU: 16+ vCPU (database server)
├── RAM: 64 GB (working set + cache)
└── Cost: ~$3,000/month (primary) + $1,500 (replicas) = $4,500/month
```

**Year 2** (30-50 GB, 250 shops):
```
Read replica architecture
├── Primary: 4 vCPU, 8 GB RAM, 50 GB SSD
├── Read Replica: 2 vCPU, 4 GB RAM (reporting)
├── Connection pooling with PgBouncer
├── Regular backups + point-in-time recovery
└── Cost: ~$230/month (primary + replica)
```

**Year 3** (50-100 GB, 500 shops):
```
Multi-instance HA architecture
├── Primary: 8 vCPU, 16 GB RAM, 100 GB SSD (transactions)
├── Read Replicas: 2 × (4 vCPU, 8 GB RAM) (reporting, analytics)
├── Connection pooling with PgBouncer
├── Elasticsearch for complex search (optional)
├── Continuous backup with WAL archiving
└── Cost: ~$800/month (distributed)
```

### Connection Pool Management

| Metric | Value | Impact |
|--------|-------|--------|
| **Max Connections** | 200 | 50 shops × 3-4 users/shop max |
| **Pool Size per Service** | 20-30 | PgBouncer multiplexing |
| **Idle Timeout** | 300 sec | Auto-close unused connections |
| **Prepared Statements** | Pre-cached | Performance + SQL injection prevention |
| **Pooling Tool** | PgBouncer | External connection manager (Y2+) |

**Scaling Decision**: When Y2 reaches 250 shops (~600-800 parallel users), upgrade to PgBouncer and monitor connection pool saturation. If P95 query latency > 200ms, add read replica.

---

## 🔍 Observability & Monitoring Estimates

### Metrics Collected (Per Service)

| Metric | Volume | Cost |
|--------|--------|------|
| **Application Logs** | 10 GB/month | Included in Datadog |
| **Metrics** | 100 time-series | ~$50/month |
| **Traces (APM)** | 10,000 traces/hour | ~$250/month |
| **Uptime Checks** | 5 endpoints | Included |
| **Dashboards** | 10 custom | Free tier |

**Total Monitoring Cost**: ~$300/month (year 1)

### SLA Reporting

| Metric | Target | How Measured |
|--------|--------|--------------|
| **Uptime** | 99.9% | Datadog uptime monitor |
| **P95 Latency** | < 500ms | Application metrics |
| **Error Rate** | < 1% | Error logs + metrics |
| **Incident MTTR** | < 5 min | Incident tracking |

---

## 🚀 Capacity Assumptions & Risks

### Assumptions Made

1. **50% Utilization**: Infrastructure provisioned for 2x peak (50% utilized on average)
2. **Steady Growth**: Linear growth (not exponential or zero)
3. **No Viral Growth**: No 100x spikes (would need emergency scaling)
4. **ERP Sync**: ERP integrations don't overload system (batch, throttled)
5. **Cache Hit Rate**: 80%+ (Redis caches popular products)
6. **Team Stability**: Same team for 3 years (no major turnover)

### Risk Factors

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **Faster growth** | Over-capacity | Monitor growth monthly, scale up 1-2 months early |
| **Larger products** | Database bloat | Implement archiving strategy (>1 year old logs) |
| **Search popularity** | Elasticsearch overload | Add read replicas if needed |
| **ERP sync delays** | Customer impact | Queue processing, set SLAs with ERP teams |
| **Cache failure** | Performance cliff | Redis clustering, fallback to DB queries |

---

## 📋 Capacity Monitoring Checklist

**Weekly**:
- [ ] Database size growth (is it tracking projections?)
- [ ] Query latency (P95 < 500ms?)
- [ ] Cache hit rate (should be >80%)
- [ ] Error rate (should be <1%)

**Monthly**:
- [ ] Disk space remaining (>30% free?)
- [ ] CPU/RAM utilization (should be <70%)
- [ ] Growth rate (compare to projections)
- [ ] Team velocity (can we deliver?)

**Quarterly**:
- [ ] Update projections (actual vs forecast)
- [ ] Scaling decisions (do we need bigger hardware?)
- [ ] Cost review (is pricing growing as expected?)
- [ ] Team capacity (do we need to hire?)

---

## 🎯 Scaling Decision Tree

```
Is database >70% capacity?
├─ YES: Scale up database (increase CPU/RAM)
│
Is query latency >500ms P95?
├─ YES: Add caching or indices
├─ (Check slow query log)
│
Is compute >80% utilization?
├─ YES: Scale up compute (more cores/RAM)
│
Is traffic >2x baseline?
├─ YES: Emergency scaling (add more servers)
├─ Post-incident: Review capacity model
│
Nothing triggered?
└─ OK: Continue with current capacity
```

---

## 📝 Notes

- **Costs are estimates** (actual depends on cloud provider, region, commitment)
- **Team allocation flexible** (can be full-time for one role, part-time for another)
- **Growth projections conservative** (real-world may vary 50% either direction)
- **Storage assumes 1-year log retention** (adjust based on compliance needs)
- **No multi-region** considered in estimates (add 2-3x cost for failover)

---

## Historical Actuals (Updated Quarterly)

### Q4 2025 (Current)

| Metric | Estimate | Actual | Variance |
|--------|----------|--------|----------|
| **Shops** | 100 | — | (Pre-launch) |
| **Users** | 1,000 | — | (Pre-launch) |
| **Database Size** | 50 GB | — | (Pre-launch) |
| **Monthly Cost** | $960 | — | (Pre-launch) |

---

**Last Updated**: 29. Dezember 2025  
**Next Review**: 2026-03-29 (quarterly)  
**Owner**: @software-architect  
**Maintainer**: @devops-engineer (infrastructure metrics)
