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

## 🏢 Enterprise Customer Scenario (Alternative Deployment Model)

**Profile**: Single large B2B distributor or marketplace with massive catalog

| Metric | Value | Notes |
|--------|-------|-------|
| **Customer Type** | Single enterprise tenant | Dedicated infrastructure |
| **Product Variants** | 2.5 million SKUs | Large B2B distributor catalog |
| **Daily Orders** | 200 orders/day | 6,000/month, 72,000/year |
| **Concurrent Users** | 50-100 buyers + staff | Peak: 200 during promotions |
| **Business Model** | B2B wholesale/distribution | High catalog complexity, moderate order volume |

### Storage Requirements (Enterprise Customer)

| Component | Calculation | Size |
|-----------|-------------|------|
| **Product Catalog** | 2.5M variants × 8KB avg | **20 GB** |
| - Base product data | SKU, descriptions (multi-language) | 5 GB |
| - Attributes & specifications | Custom fields, technical data | 8 GB |
| - B2B pricing tiers | Volume pricing, customer groups | 3 GB |
| - Media references | Image URLs, thumbnails | 2 GB |
| - Category navigation | Hierarchies, facets, filters | 2 GB |
| **Order History** | 72K orders/year × 15KB avg | **1 GB/year** |
| **Customer Data** | 5,000-10,000 B2B accounts | **0.5 GB** |
| **Elasticsearch Index** | 2.5M docs × 4KB | **10 GB** |
| **Logs & Audit Trail** | 1 year retention | **5 GB/year** |
| **Redis Cache** | Hot data, sessions | **5 GB** (in-memory) |
| **Backups** | 3× daily snapshots | **75 GB** |
| **TOTAL Year 1** | | **~50 GB** |
| **TOTAL Year 3** | With growth (+orders, +logs) | **~80 GB** |

### Infrastructure Requirements (Enterprise Customer)

| Component | Specification | Monthly Cost | Justification |
|-----------|---------------|--------------|---------------|
| **API/Compute** | 4 vCPU, 8 GB RAM (2 instances, HA) | **$350** | Handle catalog complexity + peak load |
| **Database Primary** | 8 vCPU, 16 GB RAM, 100 GB SSD | **$300** | 2.5M variants needs RAM for indexes |
| **Database Replica** | 4 vCPU, 8 GB RAM (read-only) | **$150** | Offload search queries, analytics |
| **Elasticsearch** | 2-node cluster, 8 vCPU, 16 GB RAM total | **$400** | **MANDATORY** for 2.5M variant search |
| **Redis Cache** | 10 GB (HA cluster, 2 nodes) | **$120** | Cache popular products, sessions |
| **Storage** | 50 GB + 150 GB backups | **$100** | Product images, attachments |
| **CDN** | Cloudflare Pro + caching | **$100** | Serve static assets, images |
| **Monitoring** | Application Insights Premium | **$200** | Deep observability for large catalog |
| **Email/Notifications** | SendGrid Pro (6K orders/month) | **$100** | Order confirmations, shipping |
| **Enhanced Backups** | Point-in-time recovery (15-min RPO) | **$150** | Enterprise SLA requirement |
| **Load Balancer** | Application Gateway | **$80** | HA across compute instances |
| **TOTAL MONTHLY** | | **$2,050** | Full production infrastructure |

**Annual Infrastructure Cost**: **~$24,600**  
**Per Order**: **$0.34** (higher due to catalog complexity)  
**Per Variant/Month**: **$0.00082** ($2,050 ÷ 2.5M variants)

### Performance Targets (Enterprise Customer)

| Metric | Target | Implementation |
|--------|--------|----------------|
| **Product Search (2.5M)** | < 200ms P95 | Elasticsearch with faceted search |
| **Catalog Page Load** | < 300ms P95 | Heavy Redis caching (80% hit rate) |
| **Product Detail** | < 150ms P95 | Cached for popular items |
| **Checkout** | < 500ms P95 | Standard order processing |
| **Order Confirmation** | < 1s P95 | Email + ERP sync |
| **Daily Order Volume** | 200 orders | Peak: 400 (promotional days) |
| **Concurrent Users** | 100 users | Peak: 200 (flash sales) |
| **SLA Uptime** | 99.9% | HA configuration required |

### Elasticsearch Configuration (Critical for Large Catalogs)

**Why Elasticsearch is Mandatory**:
- PostgreSQL FTS breaks down at 500K+ variants
- Faceted navigation requires specialized indexing
- Multi-language search needs language analyzers
- Real-time inventory updates need fast re-indexing

**Cluster Specifications**:
```
Elasticsearch 2-Node Cluster:
├── Node 1: 4 vCPU, 8 GB RAM, 50 GB SSD
├── Node 2: 4 vCPU, 8 GB RAM, 50 GB SSD (replica)
├── Indices:
│   ├── Products: 2.5M docs × 4KB = 10 GB
│   ├── Shards: 5 primary + 5 replicas
│   ├── Facets: Category, brand, attributes, price ranges
│   └── Full-text: Multi-language search (8 languages)
├── Query Performance:
│   ├── Simple search: < 50ms
│   ├── Faceted search: < 150ms
│   └── Complex filters: < 200ms
└── Cost: $400/month (cannot be avoided)
```

### Caching Strategy (Enterprise Customer)

```
Redis Cache Layers (10 GB total):
├── L1: Popular products (5K variants) → 2 GB → 95% hit rate
├── L2: Category listings → 1 GB → 85% hit rate
├── L3: Search results → 2 GB → 60% hit rate
├── L4: User sessions → 500 MB → 100% hit rate
└── L5: Price calculations → 500 MB → 90% hit rate

Cache Invalidation Strategy:
├── Product updates: Real-time (via events)
├── Inventory: 1-minute TTL
├── B2B pricing: 5-minute TTL (customer-specific)
└── Search results: 10-minute TTL
```

### Database Optimization (2.5M Variants)

```sql
-- Indexing strategy for large catalogs
CREATE INDEX idx_product_active ON products(tenant_id, is_active) 
  WHERE is_active = true;
  
CREATE INDEX idx_product_search ON products USING GIN(search_vector);

CREATE INDEX idx_product_category ON products(tenant_id, category_id, sort_order);

CREATE INDEX idx_product_sku ON products(tenant_id, sku) WHERE is_active = true;

-- Partitioning for order history (monthly partitions)
CREATE TABLE orders PARTITION BY RANGE (created_at);

-- Table statistics for query optimization
ANALYZE products;
```

### Scaling Projections (Enterprise Customer)

#### Year 1 (Baseline)
```
Orders:                  72,000/year
Variants:                2.5M
Database:                50 GB
Infrastructure:          $2,050/month
DevOps Support:          +$2,000-3,000/month (0.5 FTE)
Total Monthly:           $4,050-5,050
Per Order Cost:          $0.34
```

#### Year 2 (20% Growth)
```
Orders:                  86,400/year (+14,400)
Variants:                3M (+500K new products)
Database:                65 GB
Infrastructure:          $2,400/month
├── Database upgrade:    +$150 (more RAM for indexes)
├── Elasticsearch:       +$150 (3-node cluster)
├── Compute:             +$50 (vertical scaling)
└── Other:               Same
DevOps Support:          +$2,500-3,500/month
Total Monthly:           $4,900-5,900
Per Order Cost:          $0.33 (slight improvement)
```

#### Year 3 (40% Total Growth)
```
Orders:                  100,000/year (+28,000 from Y1)
Variants:                3.5M (+1M from Y1)
Database:                80 GB
Infrastructure:          $2,800/month
├── Database:            $600 (HA cluster)
├── Elasticsearch:       $550 (3-node, more RAM)
├── Compute:             $500 (4 instances)
├── Cache:               $150 (larger cluster)
└── Other services:      $1,000
DevOps Support:          +$3,000-4,000/month
Total Monthly:           $5,800-6,800
Per Order Cost:          $0.34 (stable efficiency)
```

### Cost Optimization Strategies (Enterprise)

#### 1. Reserved Instances (1-year commitment)
```
Standard pricing:        $2,050/month
1-year reserved:         $1,435/month (-30% on compute/DB)
Annual savings:          $7,380/year

3-year reserved:         $1,230/month (-40% on compute/DB)
Annual savings:          $9,840/year
```

#### 2. Phased Rollout (Reduce Initial Investment)
```
Phase 1 (MVP - 6 months):     $1,500/month
├── Single compute:           $175
├── Database (no replica):    $300
├── Elasticsearch:            $400 (cannot skip)
├── Redis:                    $80
├── Basic monitoring:         $100
└── Other services:           $445

Phase 2 (Production):         $2,050/month
└── Add HA, replicas, enhanced backups
```

#### 3. Auto-Scaling (Off-Peak Optimization)
```
Peak hours (8am-8pm, Mon-Fri):    Full capacity
Off-peak (nights/weekends):       Scale down 50%

Savings potential:
├── 60% of time at reduced capacity
├── Monthly savings: ~$420/month
└── Optimized cost: $1,630/month
```

**Optimized Year 1 Cost**: **~$1,640/month** (with reserved instances + auto-scaling)

### Comparison: Multi-Tenant vs. Enterprise Customer

| Metric | Multi-Tenant (50 shops) | Enterprise (1 customer) | Ratio |
|--------|------------------------|-------------------------|-------|
| **Products** | 50,000 total | 2,500,000 single catalog | **50x** |
| **Orders/Month** | 10-20K total | 6K single customer | **30-60%** |
| **Database Size** | 5-10 GB | 50 GB | **5-10x** |
| **Infrastructure** | $300/month | $2,050/month | **6.8x** |
| **Search Engine** | PostgreSQL FTS | Elasticsearch **mandatory** | **Critical** |
| **Cache Size** | 1 GB | 10 GB | **10x** |
| **Per Order Cost** | $0.15-0.30 | $0.34 | **Higher** |
| **Complexity** | Moderate | High | **Catalog-driven** |

**Key Insight**: Large catalog (2.5M variants) drives infrastructure costs significantly more than order volume. Elasticsearch becomes mandatory above 500K variants.

### Critical Success Factors (Enterprise Deployment)

1. ✅ **Elasticsearch is non-negotiable** - PostgreSQL FTS cannot scale to 2.5M variants
2. ✅ **Database needs 16GB RAM minimum** - Large catalogs require memory for indexes
3. ✅ **Heavy caching essential** - 80%+ cache hit rate required for acceptable performance
4. ✅ **Point-in-time backups mandatory** - Enterprise SLA demands <15min RPO
5. ✅ **HA configuration required** - 99.9% uptime SLA needs redundancy
6. ✅ **Dedicated DevOps support** - 0.5-1 FTE needed for this complexity

### When to Choose Each Model

**Multi-Tenant Model (Current Baseline)**:
- ✓ Many small-to-medium shops (100-10K SKUs each)
- ✓ Moderate total product count (<500K variants)
- ✓ Cost-sensitive customers ($6/shop/month)
- ✓ Simpler infrastructure management

**Enterprise Customer Model**:
- ✓ Single large B2B distributor/marketplace
- ✓ Massive product catalog (500K+ variants)
- ✓ Complex search and filtering requirements
- ✓ Higher budget tolerance ($2,000-5,000/month)
- ✓ Enterprise SLAs and support expectations

---

## 🏭 Mid-Market Customer Scenario (500K Variants)

**Profile**: Medium-sized B2B distributor at the Elasticsearch threshold

| Metric | Value | Notes |
|--------|-------|-------|
| **Customer Type** | Medium enterprise tenant | Dedicated or premium tier |
| **Product Variants** | 500,000 SKUs | Critical threshold for search technology |
| **Daily Orders** | 80-100 orders/day | 2,400-3,000/month, ~30,000/year |
| **Concurrent Users** | 30-50 buyers + staff | Peak: 100 during promotions |
| **Business Model** | Regional B2B distributor | Moderate catalog, moderate volume |

### Storage Requirements (500K Variants)

| Component | Calculation | Size |
|-----------|-------------|------|
| **Product Catalog** | 500K variants × 7KB avg | **3.5 GB** |
| - Base product data | SKU, descriptions | 1 GB |
| - Attributes & specifications | Custom fields | 1.5 GB |
| - Pricing tiers | Volume pricing | 0.5 GB |
| - Media references | Image URLs | 0.3 GB |
| - Category navigation | Hierarchies, facets | 0.2 GB |
| **Order History** | 30K orders/year × 15KB | **0.45 GB/year** |
| **Customer Data** | 2,000-3,000 B2B accounts | **0.2 GB** |
| **Elasticsearch Index** | 500K docs × 4KB | **2 GB** |
| **Logs & Audit Trail** | 1 year retention | **2 GB/year** |
| **Redis Cache** | Hot data, sessions | **2 GB** (in-memory) |
| **Backups** | 3× daily snapshots | **20 GB** |
| **TOTAL Year 1** | | **~12 GB** |
| **TOTAL Year 3** | With growth | **~20 GB** |

### Infrastructure Requirements (500K Variants)

| Component | Specification | Monthly Cost | Justification |
|-----------|---------------|--------------|---------------|
| **API/Compute** | 2 vCPU, 8 GB RAM (2 instances, HA) | **$200** | Moderate load, HA recommended |
| **Database Primary** | 4 vCPU, 8 GB RAM, 25 GB SSD | **$120** | 500K variants benefits from good RAM |
| **Database Replica** | 2 vCPU, 4 GB RAM (read-only) | **$80** | Offload reports and analytics |
| **Elasticsearch** | 1 node, 4 vCPU, 8 GB RAM | **$180** | **Recommended** at 500K threshold |
| **Redis Cache** | 3 GB (single instance) | **$50** | Session + hot products |
| **Storage** | 15 GB + 50 GB backups | **$35** | Product images, attachments |
| **CDN** | Cloudflare Pro | **$50** | Performance optimization |
| **Monitoring** | Application Insights Standard | **$100** | Good observability |
| **Email/Notifications** | SendGrid Standard (3K orders/month) | **$50** | Order confirmations |
| **Backups (Standard)** | Daily snapshots (7-day retention) | **Included** | Standard backup strategy |
| **Load Balancer** | Basic load balancing | **$50** | HA across compute |
| **TOTAL MONTHLY** | | **$915** | Mid-market infrastructure |

**Annual Infrastructure Cost**: **~$10,980**  
**Per Order**: **$0.31** (efficient for this scale)  
**Per Variant/Month**: **$0.00183** ($915 ÷ 500K variants)

### Technology Decision Point: PostgreSQL FTS vs. Elasticsearch

**At 500K variants, you're at a critical decision point:**

#### Option A: PostgreSQL FTS Only ($915 - $180 = $735/month)
```
Advantages:
✓ Lower cost: $735/month vs $915/month
✓ Simpler architecture (one less system)
✓ Easier maintenance
✓ Sufficient for simple searches

Disadvantages:
✗ Search performance degrades (250-400ms)
✗ Limited faceted navigation
✗ Complex filters struggle (>500ms)
✗ No multi-language analyzers
✗ Will need migration later as catalog grows

Verdict: Acceptable for MVP, but plan for Elasticsearch
```

#### Option B: Elasticsearch from Day 1 ($915/month - RECOMMENDED)
```
Advantages:
✓ Fast search: <100ms for complex queries
✓ Rich faceted navigation (category, brand, price)
✓ Multi-language support (8 languages)
✓ Scales to 1M+ variants easily
✓ No migration needed later

Disadvantages:
✗ Higher initial cost: +$180/month
✗ Additional complexity (one more system)
✗ Requires Elasticsearch expertise

Verdict: Recommended for production deployments
```

**Recommendation**: Start with Elasticsearch if:
- Complex filtering requirements (facets, multi-attribute)
- Multi-language search needed
- Expected catalog growth to 750K+ in 18 months
- Budget allows $915/month

**Alternative**: PostgreSQL FTS if:
- Simple keyword search only
- Tight budget (<$750/month)
- Catalog expected to stay <750K
- Willing to migrate later (adds 2-4 weeks dev time)

### Performance Targets (500K Variants)

| Metric | PostgreSQL FTS | Elasticsearch | Notes |
|--------|---------------|---------------|-------|
| **Product Search** | 250-400ms P95 | **100-150ms P95** | Simple keyword search |
| **Faceted Search** | 500-800ms P95 | **150-200ms P95** | Category + filters |
| **Complex Filters** | 800-1200ms P95 | **200-300ms P95** | Multi-attribute |
| **Catalog Page Load** | 200-300ms P95 | **150-200ms P95** | With caching |
| **Product Detail** | <100ms P95 | <100ms P95 | Cached equally |
| **Checkout** | <400ms P95 | <400ms P95 | Same performance |

### Caching Strategy (500K Variants)

```
Redis Cache Layers (3 GB total):
├── L1: Popular products (2K variants) → 800 MB → 90% hit rate
├── L2: Category listings → 500 MB → 80% hit rate
├── L3: Search results → 1 GB → 50% hit rate
└── L4: User sessions → 200 MB → 100% hit rate

Cache Invalidation:
├── Product updates: Real-time
├── Inventory: 2-minute TTL
├── Pricing: 5-minute TTL
└── Search: 15-minute TTL
```

### Database Optimization (500K Variants)

```sql
-- Critical indexes for 500K catalog
CREATE INDEX idx_product_active_sku ON products(tenant_id, is_active, sku) 
  WHERE is_active = true;

CREATE INDEX idx_product_category_sort ON products(tenant_id, category_id, sort_order)
  WHERE is_active = true;

-- Full-text search (if not using Elasticsearch)
CREATE INDEX idx_product_fts ON products USING GIN(search_vector)
  WHERE is_active = true;

-- Covering index for list queries
CREATE INDEX idx_product_list ON products(tenant_id, category_id, is_active, sort_order)
  INCLUDE (name, sku, price, image_url);

-- Regular maintenance
VACUUM ANALYZE products;
```

### Cost Breakdown and Comparison

| Scenario | Products | Monthly Cost | Per Order | Per Variant | Search Engine |
|----------|----------|--------------|-----------|-------------|---------------|
| **Multi-Tenant (50 shops)** | 50K total | $300 | $0.15-0.30 | $0.006 | PostgreSQL FTS |
| **Mid-Market (500K)** | 500K catalog | $915 | $0.31 | $0.00183 | **Threshold** |
| - With PostgreSQL FTS | 500K | $735 | $0.25 | $0.00147 | PostgreSQL FTS |
| - With Elasticsearch | 500K | $915 | $0.31 | $0.00183 | Elasticsearch ✓ |
| **Enterprise (2.5M)** | 2.5M catalog | $2,050 | $0.34 | $0.00082 | Elasticsearch ✓ |

**Key Insights**:
- **Cost per variant decreases** as catalog size increases (economies of scale)
- **Cost per order increases** with larger catalogs (infrastructure overhead)
- **500K is the decision point** for search technology choice
- **Elasticsearch adds $180/month** but provides 2-4x better search performance

### Scaling Projections (500K Variants Customer)

#### Year 1 (Baseline)
```
Orders:                  30,000/year
Variants:                500K
Database:                12 GB
Infrastructure:          $915/month (with Elasticsearch)
DevOps Support:          +$1,000-1,500/month (0.25 FTE)
Total Monthly:           $1,915-2,415
Per Order Cost:          $0.31
```

#### Year 2 (30% Growth)
```
Orders:                  39,000/year (+9K)
Variants:                650K (+150K new)
Database:                16 GB
Infrastructure:          $1,080/month
├── Database upgrade:    +$40 (more storage)
├── Elasticsearch:       +$80 (more RAM/storage)
├── Cache:               +$20 (larger cache)
└── Other:               +$25
Total Monthly:           $2,080-2,580
Per Order Cost:          $0.29 (improving)
```

#### Year 3 (60% Total Growth)
```
Orders:                  48,000/year (+18K from Y1)
Variants:                800K (+300K from Y1)
Database:                20 GB
Infrastructure:          $1,280/month
├── Database:            $250 (more RAM for indexes)
├── Elasticsearch:       $280 (2-node cluster needed)
├── Compute:             $250 (scale up)
├── Cache:               $80
└── Other services:      $420
Total Monthly:           $2,280-2,780
Per Order Cost:          $0.27 (better efficiency)
```

### Migration Path from PostgreSQL FTS to Elasticsearch

**If starting with PostgreSQL FTS** ($735/month), here's the migration cost:

| Phase | Duration | Cost | Activities |
|-------|----------|------|------------|
| **Planning** | 1 week | Included | Architecture design, data mapping |
| **Development** | 2 weeks | $4,000-6,000 | Integration code, indexing pipeline |
| **Testing** | 1 week | $2,000-3,000 | Performance testing, validation |
| **Migration** | 3 days | $1,000-2,000 | Data import, cutover, monitoring |
| **Ongoing** | Monthly | +$180/month | Elasticsearch infrastructure |
| **TOTAL ONE-TIME** | 4-5 weeks | **$7,000-11,000** | Full migration cost |

**Recommendation**: If budget allows, **start with Elasticsearch** to avoid this migration later.

### Critical Success Factors (500K Variants)

1. ✅ **Decide on search technology early** - Migration later costs $7K-11K
2. ✅ **Database needs 8GB RAM minimum** - Index performance critical
3. ✅ **Implement aggressive caching** - 80%+ hit rate essential
4. ✅ **Monitor query performance** - Watch for PostgreSQL FTS degradation
5. ✅ **Plan for growth** - Catalog likely to expand to 750K-1M in 3 years
6. ✅ **HA recommended** - Mid-market customers expect good uptime

### When to Choose This Model

**500K Variant Model is ideal for**:
- ✓ Regional B2B distributors
- ✓ Specialized industry suppliers (technical products)
- ✓ Mid-market companies ($10M-100M revenue)
- ✓ Growing catalogs (started at 100K, now 500K)
- ✓ Budget range: $1,000-2,500/month
- ✓ Professional-grade expectations (99.5% uptime)

**Upgrade to 2.5M Enterprise Model when**:
- Catalog exceeds 1M variants
- Search complexity increases significantly
- Orders exceed 200/day consistently
- Enterprise SLA required (99.9%+)
- Budget increases to $2,500+ available

**Downgrade to Multi-Tenant Model when**:
- Catalog can be split into smaller shops
- Search requirements are simple
- Budget constraints (<$750/month)
- Willing to accept PostgreSQL FTS limitations

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

## 📐 Reference Architectures Summary

### Architecture 1: Multi-Tenant Startup (Current Baseline)
- **Target**: 50-500 small shops
- **Products**: 50K-500K total (distributed across shops)
- **Orders**: 10K-200K/month total platform
- **Cost**: $300-2,700/month (Year 1-3)
- **Search**: PostgreSQL FTS → Elasticsearch (optional Y3)
- **Best For**: Platform play, many small merchants
- **Per Shop**: $6/month (Year 1) → $5.40/month (Year 3)

### Architecture 2: Mid-Market Single Customer (500K Variants)
- **Target**: 1 regional B2B distributor
- **Products**: 500K variants (single catalog)
- **Orders**: 2.4K-3K/month single customer
- **Cost**: $915/month (with Elasticsearch) or $735/month (PostgreSQL FTS)
- **Search**: **Decision point** - Elasticsearch recommended
- **Best For**: Regional distributors, specialized suppliers
- **Per Order**: $0.31 (Elasticsearch) or $0.25 (PostgreSQL FTS)

### Architecture 3: Enterprise Single Customer (2.5M Variants)
- **Target**: 1 large B2B distributor/marketplace
- **Products**: 2.5M variants (massive single catalog)
- **Orders**: 6K-10K/month single customer
- **Cost**: $2,050/month (Year 1, optimized to $1,640)
- **Search**: Elasticsearch **mandatory**
- **Best For**: White-label deployment, enterprise contracts, marketplaces
- **Per Order**: $0.34

### Architecture 4: Hybrid Model (Future Consideration)
- **Target**: 10-20 large + 50-100 small shops
- **Products**: 3M+ total (mixed catalog sizes)
- **Orders**: 50K-150K/month combined
- **Cost**: $3,500-5,000/month
- **Search**: Elasticsearch clusters per tier
- **Best For**: Mature platform with diverse customer segments

### Quick Selection Guide

| Your Situation | Recommended Architecture | Monthly Cost |
|----------------|-------------------------|--------------|
| **Just starting, many small shops** | Architecture 1 (Multi-Tenant) | $300 |
| **100K-300K variants, simple search** | Architecture 1 or 2 (PostgreSQL FTS) | $300-735 |
| **500K variants, growth expected** | Architecture 2 (with Elasticsearch) | $915 |
| **750K-1M variants** | Architecture 2 → 3 transition | $1,080-1,500 |
| **2M+ variants, enterprise SLA** | Architecture 3 (Enterprise) | $2,050 |
| **Mix of large + small customers** | Architecture 4 (Hybrid) | $3,500+ |

### Cost per Variant Comparison

| Catalog Size | Architecture | Monthly Cost | Cost per Variant | Cost per Order |
|--------------|-------------|--------------|------------------|----------------|
| **50K** | Multi-Tenant | $300 | $0.006 | $0.15-0.30 |
| **500K** | Mid-Market | $915 | $0.00183 | $0.31 |
| **2.5M** | Enterprise | $2,050 | $0.00082 | $0.34 |

**Insight**: Larger catalogs have **lower cost per variant** but **higher cost per order** due to infrastructure complexity.

---

## ☁️ Cloud Platform Cost Comparison

### Overview

Based on **ADR-055: Cloud Platform Selection**, here's a detailed cost comparison across the three major cloud providers (Azure, AWS, GCP) for the B2X infrastructure scenarios.

### Methodology

**Pricing Date**: January 2026  
**Region**: EU (Azure West Europe, AWS Frankfurt, GCP Belgium)  
**Currency**: EUR (€) for EU deployment  
**Assumptions**: 
- Standard pay-as-you-go pricing (no reserved instances)
- 24/7 uptime (730 hours/month)
- EU data residency (GDPR compliance)
- Production-grade services (HA where applicable)

---

### Multi-Tenant Startup (50 Shops - Year 1)

**Infrastructure**: 2 vCPU compute, 4GB RAM database, minimal caching

| Component | Azure | AWS | GCP | Notes |
|-----------|-------|-----|-----|-------|
| **Compute** | €90 | €130 | €85 | Container hosting |
| - Service | App Service B1 | ECS Fargate | Cloud Run | - |
| **Database** | €50 | €85 | €60 | PostgreSQL managed |
| - Service | PostgreSQL Flexible | RDS db.t3.small | Cloud SQL db-f1-micro | - |
| **Cache** | €25 | €40 | €30 | Redis/Memcached |
| - Service | Redis Cache Basic | ElastiCache t3.micro | Memorystore Basic | - |
| **Storage** | €15 | €20 | €12 | Blob/object storage |
| - Service | Blob Storage (Hot) | S3 Standard | Cloud Storage Standard | - |
| **CDN** | €0 | €10 | €0 | Content delivery |
| - Service | Cloudflare Free | CloudFront | Cloudflare Free | External CDN |
| **Monitoring** | €40 | €25 | €30 | Logs, metrics |
| - Service | Application Insights | CloudWatch | Cloud Monitoring | - |
| **Email** | €50 | €50 | €50 | Transactional email |
| - Service | SendGrid | SendGrid/SES | SendGrid | External service |
| **Load Balancer** | €15 | €20 | €18 | Application LB |
| - Service | App Gateway Basic | ALB | Cloud Load Balancing | - |
| **TOTAL/Month** | **€285** | **€380** | **€285** | |
| **TOTAL/Year** | **€3,420** | **€4,560** | **€3,420** | |

**Winner for Startup**: **Azure/GCP tied** (€285/month)  
**Advantage**: Azure offers better .NET integration, GCP offers per-second billing

---

### Mid-Market Customer (500K Variants)

**Infrastructure**: HA compute, read replica, Elasticsearch, enhanced monitoring

| Component | Azure | AWS | GCP | Notes |
|-----------|-------|-----|-----|-------|
| **Compute (HA)** | €180 | €280 | €160 | 2 instances, 4 vCPU |
| - Service | App Service S1 (2×) | ECS Fargate (2×) | Cloud Run (2×) | - |
| **Database Primary** | €110 | €180 | €140 | PostgreSQL 4vCPU, 8GB |
| - Service | PostgreSQL GP D2s | RDS db.m5.large | Cloud SQL db-n1-standard-2 | - |
| **Database Replica** | €75 | €120 | €90 | Read replica |
| - Service | PostgreSQL GP D1s | RDS db.m5.large | Cloud SQL db-n1-standard-1 | - |
| **Elasticsearch** | €160 | €200 | €100 | Search engine |
| - Service | **Cognitive Search** | OpenSearch t3.small | **Self-hosted on GKE** | ⚠️ Migration needed |
| **Redis Cache (3GB)** | €45 | €65 | €50 | Session cache |
| - Service | Redis Cache Standard | ElastiCache t3.small | Memorystore Standard | - |
| **Storage** | €30 | €35 | €20 | 15GB + 50GB backups |
| - Service | Blob + Backups | S3 + Backups | Cloud Storage + Snapshots | - |
| **CDN** | €45 | €60 | €40 | Pro tier |
| - Service | Front Door | CloudFront | Cloud CDN | - |
| **Monitoring** | €90 | €60 | €70 | Advanced metrics |
| - Service | App Insights Premium | CloudWatch Insights | Cloud Monitoring + Trace | - |
| **Email** | €50 | €50 | €50 | Standard plan |
| - Service | SendGrid | SES | SendGrid | - |
| **Load Balancer** | €45 | €50 | €35 | Application Gateway |
| - Service | App Gateway Standard | ALB | Cloud Load Balancing | - |
| **TOTAL/Month** | **€830** | **€1,100** | **€755** | |
| **TOTAL/Year** | **€9,960** | **€13,200** | **€9,060** | |

**Winner for Mid-Market**: **GCP** (€755/month, -9% vs Azure)  
**BUT**: Azure **recommended** despite higher cost due to:
- Native .NET/Aspire support (30% faster development)
- Azure Cognitive Search vs self-hosted Elasticsearch
- Better Application Insights integration
- Lower operational overhead

**Cost Trade-off**: +€75/month (+9%) for significantly better developer experience

---

### Enterprise Customer (2.5M Variants)

**Infrastructure**: Multi-instance HA, Elasticsearch cluster, full observability

| Component | Azure | AWS | GCP | Notes |
|-----------|-------|-----|-----|-------|
| **Compute (HA)** | €320 | €450 | €300 | 4 instances, HA |
| - Service | App Service P1v2 (4×) | ECS Fargate (4×) | Cloud Run (4×) | - |
| **Database Primary** | €280 | €380 | €300 | PostgreSQL 8vCPU, 16GB |
| - Service | PostgreSQL GP D4s | RDS db.m5.xlarge | Cloud SQL db-n1-standard-4 | - |
| **Database Replicas (2×)** | €280 | €380 | €240 | 2 read replicas |
| - Service | PostgreSQL GP D2s (2×) | RDS db.m5.large (2×) | Cloud SQL db-n1-standard-2 (2×) | - |
| **Elasticsearch Cluster** | €350 | €550 | €280 | 2-3 node cluster |
| - Service | **Cognitive Search** | OpenSearch m5.large (2×) | **Self-hosted GKE** | ⚠️ Migration |
| **Redis Cache (10GB)** | €110 | €150 | €100 | Large cache cluster |
| - Service | Redis Premium | ElastiCache m5.large | Memorystore Standard 10GB | - |
| **Storage** | €90 | €100 | €70 | 50GB + 150GB backups |
| - Service | Blob Premium + Backups | S3 + Backups | Cloud Storage + Snapshots | - |
| **CDN** | €95 | €120 | €80 | Premium tier |
| - Service | Front Door Premium | CloudFront Pro | Cloud CDN Premium | - |
| **Monitoring** | €180 | €120 | €150 | Full observability |
| - Service | App Insights Enterprise | CloudWatch + X-Ray | Cloud Monitoring + Trace | - |
| **Email** | €100 | €100 | €100 | Pro plan |
| - Service | SendGrid Pro | SES Pro | SendGrid Pro | - |
| **Load Balancer** | €75 | €90 | €60 | Premium LB |
| - Service | App Gateway WAF | ALB + WAF | Cloud Load Balancing | - |
| **TOTAL/Month** | **€1,880** | **€2,440** | **€1,680** | |
| **TOTAL/Year** | **€22,560** | **€29,280** | **€20,160** | |

**Winner for Enterprise**: **GCP** (€1,680/month, -11% vs Azure)  
**BUT**: Azure **still recommended** for enterprise due to:
- Mission-critical Application Insights integration
- Enterprise Azure AD/Entra ID authentication
- Better compliance tooling (Azure Policy, Blueprints)
- Superior .NET ecosystem support

**Cost Trade-off**: +€200/month (+11%) for enterprise-grade .NET tooling

---

### Cost Comparison Summary

| Scenario | Azure | AWS | GCP | Winner (Price) | Recommended |
|----------|-------|-----|-----|----------------|-------------|
| **Startup (50 shops)** | €285 | €380 | €285 | **Tie: Azure/GCP** | **Azure** (.NET) |
| **Mid-Market (500K)** | €830 | €1,100 | €755 | **GCP** (-9%) | **Azure** (DX) |
| **Enterprise (2.5M)** | €1,880 | €2,440 | €1,680 | **GCP** (-11%) | **Azure** (Enterprise) |

**Key Insights**:
1. **GCP is cheapest** for larger deployments (-9% to -11%)
2. **AWS is most expensive** across all scenarios (+25-30%)
3. **Azure provides best value** when factoring in .NET developer productivity
4. **Cost difference is manageable**: €75-200/month premium for Azure vs GCP at scale

---

### Hidden Costs & Considerations

#### Azure Hidden Costs
- ✅ **Lower**: Outbound data transfer (€0.05/GB vs €0.08-0.09/GB)
- ✅ **Included**: Application Insights basic tier (1GB/month free)
- ⚠️ **Higher**: Premium Redis cache more expensive than competitors
- ⚠️ **Lock-in**: Azure Cognitive Search migration cost if switching

#### AWS Hidden Costs
- ❌ **Higher**: EKS control plane (€70/month) not in estimates above
- ❌ **Higher**: Data transfer between AZ (€0.01/GB)
- ❌ **Complex**: 100+ pricing dimensions, hard to estimate
- ⚠️ **Surprise bills**: CloudWatch Logs can get expensive (>€100/month)

#### GCP Hidden Costs
- ✅ **Lower**: Per-second billing (vs per-hour)
- ✅ **Sustained use**: Automatic discounts (up to 30%)
- ❌ **Self-hosted**: Elasticsearch requires operational overhead
- ❌ **Smaller ecosystem**: Fewer third-party integrations

---

### Reserved Instance Savings

**Commitment**: 1-year or 3-year reserved capacity

| Scenario | Azure 1yr | Azure 3yr | AWS 1yr | AWS 3yr | GCP 1yr | GCP 3yr |
|----------|-----------|-----------|---------|---------|---------|---------|
| **Startup** | €200 (-30%) | €165 (-42%) | €265 (-30%) | €228 (-40%) | €200 (-30%) | €170 (-40%) |
| **Mid-Market** | €580 (-30%) | €498 (-40%) | €770 (-30%) | €660 (-40%) | €530 (-30%) | €453 (-40%) |
| **Enterprise** | €1,316 (-30%) | €1,128 (-40%) | €1,708 (-30%) | €1,464 (-40%) | €1,176 (-30%) | €1,008 (-40%) |

**Recommendation**: 
- **Year 1**: Pay-as-you-go (flexibility)
- **Year 2+**: 1-year reserved instances (-30% savings)
- **Year 3+**: 3-year reserved instances (-40% savings) if confident in platform

**Potential Savings (Enterprise, 3-year Azure)**:
- Annual: €22,560 → €13,536/year
- **Total 3-year savings**: €27,072 (€752/month average)

---

### Cost Optimization Strategies by Platform

#### Azure Optimization
- ✅ **Azure Hybrid Benefit**: Use existing Windows Server licenses (-40% on VMs)
- ✅ **Dev/Test Pricing**: Separate dev environments (-55% discount)
- ✅ **Azure Advisor**: Free cost optimization recommendations
- ✅ **Auto-shutdown**: Dev/test VMs auto-stop nights/weekends

**Estimated Savings**: 15-20% with basic optimization

#### AWS Optimization
- ✅ **Savings Plans**: Flexible commitment model (-30-40%)
- ✅ **Spot Instances**: For non-critical workloads (-70% but interruptible)
- ✅ **S3 Intelligent Tiering**: Automatic storage class optimization
- ✅ **Compute Optimizer**: ML-based right-sizing recommendations

**Estimated Savings**: 20-30% with aggressive optimization

#### GCP Optimization
- ✅ **Sustained Use Discounts**: Automatic (up to 30%, already in estimates)
- ✅ **Committed Use Discounts**: Additional -30-40% on top
- ✅ **Preemptible VMs**: For batch workloads (-80% but interruptible)
- ✅ **Per-second billing**: Minimize waste vs hourly billing

**Estimated Savings**: 10-15% additional (sustained use already applied)

---

### Migration Costs (One-Time)

If switching platforms after initial deployment:

| Activity | Azure → AWS | Azure → GCP | AWS → Azure | Effort |
|----------|-------------|-------------|-------------|--------|
| **IaC Rewrite** | Terraform | Terraform | ARM → Terraform | 2-4 weeks |
| **App Code Changes** | Minimal | Minimal | Minimal | 1-2 weeks |
| **CI/CD Pipeline** | GitHub Actions | GitHub Actions | GitHub Actions | 1 week |
| **Monitoring Setup** | CloudWatch | Cloud Monitoring | App Insights | 2 weeks |
| **Search Migration** | OpenSearch | Self-hosted ES | Cognitive Search | **4-6 weeks** |
| **Data Migration** | DMS | Database Migration Service | Azure Migrate | 1-2 weeks |
| **Testing & Validation** | Full test suite | Full test suite | Full test suite | 2-3 weeks |
| **TOTAL EFFORT** | **12-18 weeks** | **12-18 weeks** | **12-18 weeks** | 3-4.5 months |
| **COST ESTIMATE** | **€40K-60K** | **€40K-60K** | **€40K-60K** | Dev team cost |

**Key Risk**: Search technology migration (Elasticsearch ↔ Cognitive Search ↔ OpenSearch) is highest effort

---

### Final Recommendation

**Primary Choice: Microsoft Azure** (ADR-055)

**Rationale**:
1. **Developer Productivity**: Native .NET/Aspire support worth €75-200/month premium
2. **Total Cost of Ownership**: Faster development reduces team costs (>€5K/month)
3. **Cost Competitiveness**: Only 9-11% more expensive than GCP
4. **Ecosystem**: Best tooling for .NET teams (Application Insights, Visual Studio, Azure DevTools)
5. **Risk Mitigation**: No need for self-hosted Elasticsearch (operational complexity)

**When to Consider Alternatives**:
- **GCP**: If cost is absolute priority and willing to self-host Elasticsearch
- **AWS**: If already have AWS expertise or need best-in-class managed Elasticsearch (OpenSearch)

**Cost Decision**:
- Pay **€75-200/month** more for Azure = **€900-2,400/year**
- Save **€15,000-25,000/year** in development productivity
- **Net savings**: €12,600-22,600/year with Azure

---

**Last Updated**: 10. Januar 2026  
**Next Review**: 2026-03-29 (quarterly)  
**Owner**: @software-architect  
**Maintainer**: @devops-engineer (infrastructure metrics)
