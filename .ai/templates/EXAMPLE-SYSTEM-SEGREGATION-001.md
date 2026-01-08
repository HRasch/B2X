---
docid: TPL-008
title: EXAMPLE SYSTEM SEGREGATION 001
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: EXAMPLE-SYSTEM-SEGREGATION-001
title: "Example: System-Segregated Documentation (Store vs Admin vs Management)"
category: examples
type: Documentation Example
status: Active
created: 2026-01-08
---

# Example: System-Segregated Documentation

**Purpose**: Show how the SAME feature (Product Management) is documented differently for each system with strict segregation.

---

## 🎯 Scenario

**Feature**: Product Management  
**How it differs**:
- **Store Customer**: Can view products, add to wishlist, read reviews
- **Admin**: Can create, edit, delete products; set prices; manage inventory
- **Management**: Can view sales analytics, track inventory costs, see profitability

Each system sees **only relevant documentation**.

---

## 📚 Three Documentation Sets

### SET 1: Store System - Customer Documentation

**DocID**: `USERDOC-STORE-HOW-001-browse-products`

```yaml
---
docid: USERDOC-STORE-HOW-001-browse-products
title: How to Browse and Find Products
category: user/store/howto
system: store

# SYSTEM SEGREGATION: Store only
audience:
  primary: [store_customer]
  systems: [B2X.Store]
  roles: [customer, shop_user]
  exclude_roles: [admin_user, management_user]  # ← Block admin/mgmt

system_access:
  store: true         # ✅ Visible in Store
  admin: false        # ❌ NOT visible in Admin
  management: false   # ❌ NOT visible in Management

# AI will only return this to Store users
ai_metadata:
  use_cases: [customer_support, user_onboarding]
  system_restriction: store
  prevent_cross_system_responses: true
---
```

**Content**:
```markdown
## How to Browse and Find Products

1. Go to Store homepage
2. Browse by category or search
3. Read product description and reviews
4. Check price and availability
5. Add to cart or wishlist

✅ Product found!

### Related Products
- See product recommendations
- Check similar items

### FAQ
- How do I save products to my wishlist?
- How are products reviewed?

### Related
- [USERDOC-STORE-HOW-002] How to add to cart
- [USERDOC-STORE-FAQ-001] Product questions

### Security Note
This documentation is for Store customers only.
Admin and Management users cannot access this.
```

**What's NOT in this doc**:
- ❌ How to create products (admin only)
- ❌ Product pricing strategy (management only)
- ❌ Inventory management (admin only)
- ❌ Sales analytics (management only)
- ❌ No mentions of admin/management systems

---

### SET 2: Admin System - Administrator Documentation

**DocID**: `USERDOC-ADMIN-HOW-001-manage-products`

```yaml
---
docid: USERDOC-ADMIN-HOW-001-manage-products
title: How to Manage Products and Inventory
category: user/admin/howto
system: admin

# SYSTEM SEGREGATION: Admin only
audience:
  primary: [store_admin]
  systems: [B2X.Admin]
  roles: [admin_user, system_admin]
  exclude_roles: [customer, shop_user, management_user]  # ← Block store/mgmt

system_access:
  store: false        # ❌ NOT visible to customers
  admin: true         # ✅ Visible in Admin
  management: false   # ❌ NOT visible in Management

# AI will only return this to Admin users
ai_metadata:
  use_cases: [admin_support, operations]
  system_restriction: admin
  prevent_cross_system_responses: true
---
```

**Content**:
```markdown
## How to Manage Products and Inventory

### Create a New Product
1. Admin Dashboard → Catalog → Products
2. Click "Add New Product"
3. Enter product details:
   - Name (customer-visible)
   - SKU (internal code)
   - Description
   - Category
4. Set price and cost
5. Add images
6. Click "Save"

✅ Product created!

### Edit Product
1. Find product in catalog
2. Click "Edit"
3. Update details as needed
4. Click "Save"

### Manage Inventory
1. Go to Inventory section
2. Adjust stock levels
3. Set low-stock warnings
4. Monitor stock movements

### Related
- [USERDOC-ADMIN-HOW-002] How to manage categories
- [USERDOC-ADMIN-HOW-003] How to set product permissions
- [USERDOC-ADMIN-FAQ-001] Admin operations

### Important Notes
- Changes are immediate (customers see updates)
- Cannot delete products (for audit trail)
- All changes are logged
- Requires admin privileges
```

**What's NOT in this doc**:
- ❌ How to use as a customer (store-only)
- ❌ Product sales analytics (management only)
- ❌ Financial profitability calculations (management only)
- ❌ No mentions of store customer features

---

### SET 3: Management System - Business Documentation

**DocID**: `USERDOC-MGMT-HOW-001-product-analytics`

```yaml
---
docid: USERDOC-MGMT-HOW-001-product-analytics
title: How to View Product Sales Analytics
category: user/management/howto
system: management

# SYSTEM SEGREGATION: Management only
audience:
  primary: [manager, tenant_admin]
  systems: [B2X.Management]
  roles: [manager, tenant_admin, executive]
  exclude_roles: [customer, admin_user, shop_user]  # ← Block store/admin

system_access:
  store: false        # ❌ NOT visible to customers
  admin: false        # ❌ NOT visible to admins
  management: true    # ✅ Visible in Management

# AI will only return this to Management users
ai_metadata:
  use_cases: [business_analytics, reporting]
  system_restriction: management
  prevent_cross_system_responses: true
---
```

**Content**:
```markdown
## How to View Product Sales Analytics

### Dashboard Overview
1. Management Dashboard → Analytics
2. Select date range
3. View key metrics:
   - Total sales
   - Best-selling products
   - Average order value
   - Inventory turnover

### View Product Details
1. Click on product in analytics
2. See:
   - Revenue this period
   - Units sold
   - Customer reviews & ratings
   - Profit margin
   - Stock status

### Generate Sales Report
1. Go to Reports → Product Sales
2. Select date range
3. Choose metrics to include:
   - Revenue
   - Profit
   - Units sold
   - Customer acquisition cost
4. Export as CSV or PDF

### Monitor Profitability
1. View profit margin per product
2. Identify low-margin items
3. Review costs vs revenue
4. Make pricing decisions

### Related
- [USERDOC-MGMT-HOW-002] How to analyze customer behavior
- [USERDOC-MGMT-HOW-003] How to review financial reports
- [USERDOC-MGMT-FAQ-001] Business analytics FAQ
```

**What's NOT in this doc**:
- ❌ How to create/edit products (admin operations)
- ❌ How customers browse products (store features)
- ❌ Technical system configuration (admin only)
- ❌ No mentions of store customer or admin features

---

## 🔐 Information Segregation: Concrete Example

### Query 1: Store Customer Asks "How do I find a product?"

```
User System: B2X.Store
User Role: customer

Query: "How do I find a product?"

Documentation Retrieval:
├─ Search: [USERDOC-STORE-HOW-001] Browse products ✅
├─ Search: [USERDOC-ADMIN-HOW-001] Manage products ❌ FILTERED OUT
├─ Search: [USERDOC-MGMT-HOW-001] Sales analytics ❌ FILTERED OUT

Result Shown:
└─ [USERDOC-STORE-HOW-001] How to Browse Products
   (Other docs hidden - not in Store system)

Customer sees:
✅ How to search and filter products
✅ How to read reviews
❌ NOT: Admin product creation process
❌ NOT: Sales analytics
```

---

### Query 2: Admin Asks "How do I manage products?"

```
User System: B2X.Admin
User Role: admin_user

Query: "How do I manage products?"

Documentation Retrieval:
├─ Search: [USERDOC-ADMIN-HOW-001] Manage products ✅
├─ Search: [USERDOC-STORE-HOW-001] Browse products ❌ FILTERED OUT
├─ Search: [USERDOC-MGMT-HOW-001] Sales analytics ❌ FILTERED OUT

Result Shown:
└─ [USERDOC-ADMIN-HOW-001] How to Manage Products
   (Other docs hidden - not in Admin system)

Admin sees:
✅ How to create/edit products
✅ How to manage inventory
❌ NOT: Customer browsing features
❌ NOT: Sales analytics
```

---

### Query 3: Manager Asks "How do I view product sales?"

```
User System: B2X.Management
User Role: manager

Query: "Product sales analytics"

Documentation Retrieval:
├─ Search: [USERDOC-MGMT-HOW-001] Sales analytics ✅
├─ Search: [USERDOC-ADMIN-HOW-001] Manage products ❌ FILTERED OUT
├─ Search: [USERDOC-STORE-HOW-001] Browse products ❌ FILTERED OUT

Result Shown:
└─ [USERDOC-MGMT-HOW-001] How to View Product Sales Analytics
   (Other docs hidden - not in Management system)

Manager sees:
✅ How to view sales analytics
✅ How to analyze profitability
❌ NOT: How to create/edit products (admin function)
❌ NOT: Customer product browsing
```

---

## 🔗 No Cross-System Links

### Store Doc Links
```markdown
[USERDOC-STORE-HOW-001]
└─ Related:
   ├─ [USERDOC-STORE-HOW-002] ✅ (same system)
   ├─ [USERDOC-STORE-FAQ-001] ✅ (same system)
   └─ ❌ NOT: [USERDOC-ADMIN-HOW-001] (different system)
   └─ ❌ NOT: [USERDOC-MGMT-HOW-001] (different system)
```

### Admin Doc Links
```markdown
[USERDOC-ADMIN-HOW-001]
└─ Related:
   ├─ [USERDOC-ADMIN-HOW-002] ✅ (same system)
   ├─ [USERDOC-ADMIN-FAQ-001] ✅ (same system)
   └─ ❌ NOT: [USERDOC-STORE-HOW-001] (different system)
   └─ ❌ NOT: [USERDOC-MGMT-HOW-001] (different system)
```

### Management Doc Links
```markdown
[USERDOC-MGMT-HOW-001]
└─ Related:
   ├─ [USERDOC-MGMT-HOW-002] ✅ (same system)
   ├─ [USERDOC-MGMT-FAQ-001] ✅ (same system)
   └─ ❌ NOT: [USERDOC-STORE-HOW-001] (different system)
   └─ NOT: [USERDOC-ADMIN-HOW-001] (different system)
```

**Rule**: Cross-system links are **forbidden** by pre-commit hooks.

---

## 📊 Access Control: Enforcement

### Layer 1: YAML Metadata
```
Each doc declares its system:
✅ system: store
✅ system: admin
✅ system: management
```

### Layer 2: AI Retrieval Filter
```python
# AI checks before returning docs
if user.system != doc.system:
    return []  # Block this doc
```

### Layer 3: Pre-Commit Validation
```bash
# Prevents committing cross-system links
[USERDOC-STORE-*] cannot link to [USERDOC-ADMIN-*]
[USERDOC-ADMIN-*] cannot link to [USERDOC-MGMT-*]
etc.
```

### Layer 4: Audit Logging
```
Access attempts are logged:
- 2026-01-08 14:23:45 | customer123 | STORE | Browse products | ALLOWED
- 2026-01-08 14:24:12 | admin456 | ADMIN | Manage products | ALLOWED
- 2026-01-08 14:25:33 | admin456 | STORE | Browse products | BLOCKED (wrong system)
```

---

## ✅ Security Validation

### For Store Documentation
```
✅ YAML has: system: store
✅ YAML has: audience.systems: [B2X.Store]
✅ YAML has: audience.exclude_roles: [admin_user, management_user]
✅ system_access.store: true
✅ system_access.admin: false
✅ system_access.management: false
✅ No links to [USERDOC-ADMIN-*]
✅ No links to [USERDOC-MGMT-*]
```

### For Admin Documentation
```
✅ YAML has: system: admin
✅ YAML has: audience.systems: [B2X.Admin]
✅ YAML has: audience.exclude_roles: [customer, shop_user, management_user]
✅ system_access.store: false
✅ system_access.admin: true
✅ system_access.management: false
✅ No links to [USERDOC-STORE-*]
✅ No links to [USERDOC-MGMT-*]
```

### For Management Documentation
```
✅ YAML has: system: management
✅ YAML has: audience.systems: [B2X.Management]
✅ YAML has: audience.exclude_roles: [customer, admin_user, shop_user]
✅ system_access.store: false
✅ system_access.admin: false
✅ system_access.management: true
✅ No links to [USERDOC-STORE-*]
✅ No links to [USERDOC-ADMIN-*]
```

---

## 📁 Directory Structure

```
docs/user/
├─ store/              (USERDOC-STORE-*)
│  ├─ howto/
│  ├─ faqs/
│  ├─ screen-ref/
│  └─ processes/
│
├─ admin/              (USERDOC-ADMIN-*)
│  ├─ howto/
│  ├─ faqs/
│  ├─ screen-ref/
│  └─ operations/
│
└─ management/         (USERDOC-MGMT-*)
   ├─ howto/
   ├─ faqs/
   ├─ screen-ref/
   └─ reporting/
```

**Key**: Each system has its own directory tree.  
**Benefit**: Easy to see separation; clear ownership.

---

## 🚀 Implementation Checklist

Before deploying segregated documentation:

- [ ] All existing docs categorized by system
- [ ] YAML metadata with `system:` field added to all docs
- [ ] No cross-system links exist (audit passed)
- [ ] Pre-commit hook validates segregation
- [ ] AI retrieval filtering implemented
- [ ] API gateway routing configured
- [ ] Audit logging enabled
- [ ] Access control layer operational
- [ ] Quarterly audit schedule set

---

## 🎯 Success Criteria

✅ **Information Segregation**:
- Store users cannot see admin docs
- Admin users cannot see store docs
- Management users cannot see operational docs

✅ **AI Safety**:
- AI retrieves only relevant docs for user system
- No information leakage across systems
- Query results properly filtered

✅ **Operational Efficiency**:
- Each user sees only relevant documentation
- No confusion from irrelevant docs
- Faster support resolution

✅ **Security Compliance**:
- Information restricted by design
- Audit trail for access attempts
- Pre-commit validation enforced

---

**Example Version**: 1.0  
**Last Updated**: 2026-01-08  
**Related**: [GL-052], [GL-051], [TPL-USERDOC-001]