---
docid: GL-090
title: GL 052 ROLE BASED DOCUMENTATION ACCESS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-052
title: Role-Based Documentation Access & Audience Segmentation
category: guidelines
type: Guideline
owner: @SARAH
status: Active
created: 2026-01-08
---

# Role-Based Documentation Access & Audience Segmentation

**DocID**: GL-052  
**Purpose**: Ensure documentation is segregated by user system/role to prevent information leakage and ensure relevant guidance.

---

## Overview

Documentation must be **strictly segregated** by user system and role. Users should only see guidance relevant to their system:

```
Store-User (B2X.Store)
├─ ✅ USERDOC-STORE-* (customer-facing features)
├─ ✅ How to browse, order, manage account
└─ ❌ NOT: Admin docs, Management docs, tenant config

Admin-User (B2X.Admin)
├─ ✅ USERDOC-ADMIN-* (admin operations)
├─ ✅ How to manage users, content, system
└─ ❌ NOT: Store docs, Management docs, billing

Management-User (B2X.Management)
├─ ✅ USERDOC-MGMT-* (management/business operations)
├─ ✅ How to manage business, analytics, reports
└─ ❌ NOT: Store docs, Admin docs, system config
```

---

## Documentation Categories by System

### Store System (B2X.Store)

**Audience**: End customers, store users  
**Prefix**: `USERDOC-STORE-*`  
**Location**: `docs/user/store/`

**Types**:
```
USERDOC-STORE-START-*   = Getting started as customer
USERDOC-STORE-HOW-*     = How to shop, browse, order, manage account
USERDOC-STORE-SCREEN-*  = Store pages/screens reference
USERDOC-STORE-FAQ-*     = Customer questions
USERDOC-STORE-PROC-*    = Customer workflows (order, return, etc)
```

**Example Docs**:
- USERDOC-STORE-HOW-001: How to create account
- USERDOC-STORE-HOW-002: How to place an order
- USERDOC-STORE-HOW-003: How to manage my account
- USERDOC-STORE-SCREEN-001: Store homepage reference
- USERDOC-STORE-FAQ-001: Common customer questions

**Information NOT included**:
- ❌ Admin user management
- ❌ System configuration
- ❌ Tenant management
- ❌ Business analytics
- ❌ Financial/billing operations
- ❌ Management dashboards

---

### Admin System (B2X.Admin)

**Audience**: Store administrators, system operators  
**Prefix**: `USERDOC-ADMIN-*`  
**Location**: `docs/user/admin/`

**Types**:
```
USERDOC-ADMIN-START-*   = Admin onboarding
USERDOC-ADMIN-HOW-*     = How to manage store/users/content
USERDOC-ADMIN-SCREEN-*  = Admin dashboard/screen reference
USERDOC-ADMIN-FAQ-*     = Admin questions
USERDOC-ADMIN-PROC-*    = Admin workflows (user mgmt, moderation, etc)
```

**Example Docs**:
- USERDOC-ADMIN-HOW-001: How to manage users
- USERDOC-ADMIN-HOW-002: How to manage product catalog
- USERDOC-ADMIN-HOW-003: How to moderate content
- USERDOC-ADMIN-SCREEN-001: Admin dashboard reference
- USERDOC-ADMIN-FAQ-001: Admin operations FAQ

**Information NOT included**:
- ❌ Customer/store user features
- ❌ Management business features
- ❌ Tenant settings (if accessible to management only)
- ❌ Financial/billing operations
- ❌ Business analytics/reporting

---

### Management System (B2X.Management)

**Audience**: Business managers, tenant admins, executives  
**Prefix**: `USERDOC-MGMT-*`  
**Location**: `docs/user/management/`

**Types**:
```
USERDOC-MGMT-START-*    = Management platform onboarding
USERDOC-MGMT-HOW-*      = How to manage business/analytics/reports
USERDOC-MGMT-SCREEN-*   = Management dashboard reference
USERDOC-MGMT-FAQ-*      = Business/management questions
USERDOC-MGMT-PROC-*     = Business workflows (analytics, billing, etc)
```

**Example Docs**:
- USERDOC-MGMT-HOW-001: How to view business analytics
- USERDOC-MGMT-HOW-002: How to manage billing
- USERDOC-MGMT-HOW-003: How to generate reports
- USERDOC-MGMT-SCREEN-001: Analytics dashboard reference
- USERDOC-MGMT-FAQ-001: Business operations FAQ

**Information NOT included**:
- ❌ Customer store features
- ❌ Admin system operations
- ❌ Technical system configuration
- ❌ User management (unless business-critical)

---

## YAML Metadata: Audience & Role Specification

### Required Fields

```yaml
---
docid: USERDOC-STORE-HOW-001-place-order
title: How to Place an Order
category: user/store

# CRITICAL: Audience specification
audience: 
  primary: 
    - store_customer      # Primary audience
  systems:
    - B2X.Store           # Only accessible in Store system
  roles:
    - customer
    - buyer
    - shop_user
  exclude_roles:          # Explicitly block other systems
    - admin_user
    - management_user
    - system_admin

# System-specific metadata
system_access:
  store: true             # ✅ Visible in Store system
  admin: false            # ❌ Hidden from Admin system
  management: false       # ❌ Hidden from Management system
  api_gateway: store      # Route to Store gateway API only

# Content filtering
content_visibility:
  applies_to_stores: true
  applies_to_admins: false
  applies_to_management: false

status: Active
last_updated: 2026-01-08
---
```

---

## Metadata Validation Rules

### For Store Docs (USERDOC-STORE-*)

✅ **Must have**:
```yaml
audience:
  systems: [B2X.Store]
  roles: [customer, buyer, shop_user]
  exclude_roles: [admin_user, management_user]

system_access:
  store: true
  admin: false
  management: false
```

❌ **Must NOT have**:
```yaml
audience:
  systems: [B2X.Admin, B2X.Management]  # ERROR!
```

---

### For Admin Docs (USERDOC-ADMIN-*)

✅ **Must have**:
```yaml
audience:
  systems: [B2X.Admin]
  roles: [admin_user, system_admin]
  exclude_roles: [customer, shop_user, management_user]

system_access:
  store: false
  admin: true
  management: false
```

❌ **Must NOT have**:
```yaml
audience:
  systems: [B2X.Store]  # ERROR!
```

---

### For Management Docs (USERDOC-MGMT-*)

✅ **Must have**:
```yaml
audience:
  systems: [B2X.Management]
  roles: [manager, tenant_admin, executive]
  exclude_roles: [customer, admin_user, shop_user]

system_access:
  store: false
  admin: false
  management: true
```

---

## AI Assistant Filtering

### At Retrieval Time

When an AI assistant receives a user query, it must:

```python
def filter_documentation(user_system, user_role, query):
    """Filter docs by user system and role"""
    
    # 1. Check user system
    if user_system not in doc.audience.systems:
        return []  # Block this doc
    
    # 2. Check user role
    if user_role in doc.audience.exclude_roles:
        return []  # Block this doc
    
    # 3. Check system_access flags
    if user_system == "B2X.Store" and not doc.system_access.store:
        return []  # Block
    
    if user_system == "B2X.Admin" and not doc.system_access.admin:
        return []  # Block
    
    if user_system == "B2X.Management" and not doc.system_access.management:
        return []  # Block
    
    # 4. Content is safe to return
    return [doc]
```

### Example: Query Routing

```
User Query: "How do I create a product?"
User System: B2X.Store
User Role: customer

Search Results (before filtering):
├─ USERDOC-STORE-HOW-001: Create account ✅ Show
├─ USERDOC-ADMIN-HOW-002: Create product ❌ BLOCK
├─ USERDOC-MGMT-HOW-003: Add to catalog ❌ BLOCK

Final Results (after filtering):
└─ USERDOC-STORE-HOW-001: ...
   (Only store-relevant docs shown)
```

---

## Cross-Linking Rules

### Within Same System (ALLOWED)

```markdown
✅ USERDOC-STORE-HOW-001 links to:
   - [USERDOC-STORE-HOW-002] (another store doc)
   - [USERDOC-STORE-SCREEN-001] (store screen ref)
   - [USERDOC-STORE-FAQ-001] (store FAQs)
   
   All these are in same system → Safe
```

### Across Systems (BLOCKED)

```markdown
❌ USERDOC-STORE-HOW-001 CANNOT link to:
   - [USERDOC-ADMIN-HOW-001] (admin-only doc)
   - [USERDOC-MGMT-HOW-001] (management-only doc)
   
   Different systems → Reveals information
```

### Shared Developer Docs

```markdown
✅ USERDOC-STORE-HOW-001 CAN link to:
   - [DEVDOC-GUIDE-001] (if marked as "public API")
   - [KB-006] (if marked as "shareable knowledge")
   
   Only if explicitly marked as shared content
```

**Rule**: Always validate `system_access` and `audience.exclude_roles` before allowing links.

---

## Template Updates

### TPL-USERDOC-001 Update: System-Specific Header

Add to template:

```yaml
---
# SYSTEM & AUDIENCE SPECIFICATION (REQUIRED)
system: store  # Options: store | admin | management
audience:
  primary: [customer]  # Store-specific roles
  systems: [B2X.Store]
  exclude_roles: [admin_user, management_user]

system_access:
  store: true
  admin: false
  management: false

# SECURITY: Document must not contain:
# - Admin configuration instructions
# - Management business logic
# - System settings beyond user's system
security:
  audience_segregation: required
  cross_system_links: forbidden
  role_validation: required
---
```

### AI Assistant Constraint

Add to `ai_metadata`:

```yaml
ai_metadata:
  use_cases:
    - customer_support
    - user_onboarding
  
  # NEW: System-aware AI constraints
  system_restriction: store          # AI must validate this
  role_restriction: [customer]       # AI must validate role
  
  # Prevent AI from mixing systems
  prevent_cross_system_responses: true
  
  # Audit trail for security
  log_access: true
```

---

## Content Creation Guidelines

### Store Documentation (USERDOC-STORE-*)

**Focus**:
- Customer-facing features
- Account management
- Shopping workflows
- Order management
- Support information

**Language**:
- Friendly, customer-focused
- Minimize technical jargon
- Emphasize benefits, not admin details

**DO**:
- ✅ Explain how to use store features
- ✅ Help with account, orders, payments
- ✅ Provide customer support
- ✅ Explain store policies (returns, shipping, etc)

**DO NOT**:
- ❌ Explain admin operations
- ❌ Discuss system configuration
- ❌ Mention tenant/management features
- ❌ Link to admin docs
- ❌ Reveal backend systems

**Example Section**:
```markdown
## How to Place an Order

1. Browse products in catalog
2. Add items to cart
3. Proceed to checkout
4. Enter shipping address
5. Review and confirm

✅ Your order is complete!

Related:
- [USERDOC-STORE-HOW-003] How to track order
- [USERDOC-STORE-FAQ-001] Questions about shipping
```

---

### Admin Documentation (USERDOC-ADMIN-*)

**Focus**:
- Store management
- User administration
- Content moderation
- Store operations
- System administration

**Language**:
- Professional, operational
- Technical details relevant to operations
- Assume administrator knowledge

**DO**:
- ✅ Explain admin features and operations
- ✅ How to manage users, products, content
- ✅ System settings and configuration
- ✅ Troubleshooting admin issues

**DO NOT**:
- ❌ Explain customer features
- ❌ Management business operations
- ❌ Tenant billing/financial systems
- ❌ Link to store customer docs
- ❌ Link to management docs

**Example Section**:
```markdown
## How to Manage Users

1. Go to Admin Dashboard → Users
2. Click "Add New User"
3. Enter email and role
4. Set permissions
5. Click "Save"

Related:
- [USERDOC-ADMIN-HOW-002] How to set user permissions
- [USERDOC-ADMIN-FAQ-001] Admin operations FAQ
```

---

### Management Documentation (USERDOC-MGMT-*)

**Focus**:
- Business analytics
- Financial operations
- Reporting
- Business management
- Tenant administration

**Language**:
- Business-focused
- KPI and metric terminology
- Assume business knowledge

**DO**:
- ✅ Explain business operations and analytics
- ✅ Financial and billing operations
- ✅ Business metrics and reporting
- ✅ Tenant administration

**DO NOT**:
- ❌ Explain customer store features
- ❌ Admin system operations
- ❌ Technical system configuration
- ❌ Link to store or admin docs

**Example Section**:
```markdown
## How to View Sales Analytics

1. Go to Management Dashboard → Analytics
2. Select date range
3. Choose metrics (revenue, orders, AOV)
4. View chart and export data

Related:
- [USERDOC-MGMT-HOW-002] How to generate reports
- [USERDOC-MGMT-FAQ-001] Business analytics FAQ
```

---

## Registry: System-Specific Documentation

Update [DOCUMENT_REGISTRY.md] to include system indicator:

```markdown
## Registry: User Documentation (USERDOC-*)

| DocID | Type | System | Title | Path | Status |
|-------|------|--------|-------|------|--------|
| `USERDOC-STORE-HOW-001` | Store | B2X.Store | How to Create Account | `docs/user/store/howto/...` | Active |
| `USERDOC-ADMIN-HOW-001` | Admin | B2X.Admin | How to Manage Users | `docs/user/admin/howto/...` | Active |
| `USERDOC-MGMT-HOW-001` | Mgmt | B2X.Management | How to View Analytics | `docs/user/management/howto/...` | Active |

**NOTE**: Documents are strictly segregated by system.
Store users cannot access Admin or Management docs.
Admin users cannot access Store or Management docs.
```

---

## Validation & Enforcement

### Pre-Commit Hook

```bash
#!/bin/bash
# Check: No cross-system links

for file in docs/user/**/*.md; do
  system=$(grep "system:" "$file" | cut -d' ' -f2)
  
  # If Store doc, check for admin/mgmt links
  if [[ $system == "store" ]]; then
    if grep -q "USERDOC-ADMIN-\|USERDOC-MGMT-" "$file"; then
      echo "❌ ERROR: Store doc links to Admin/Mgmt doc"
      exit 1
    fi
  fi
  
  # Similar checks for admin and management
done
```

### Documentation Audit

Quarterly audit must verify:

- [ ] All USERDOC files have `system:` field
- [ ] No cross-system links exist
- [ ] `system_access` flags are accurate
- [ ] `audience.exclude_roles` is populated
- [ ] No information leakage between systems
- [ ] All docs properly categorized

```bash
# Run audit
scripts/docs-system-audit.sh

# Output:
# Store docs: 42 (all verified)
# Admin docs: 38 (1 with cross-system link!)
# Management docs: 15 (all verified)
```

---

## AI Assistant Implementation

### Retrieval Augmentation

```python
class DocumentationRetriever:
    def retrieve(self, query, user_system, user_role):
        """Retrieve docs with system/role filtering"""
        
        # 1. Vector search
        results = vector_db.search(query, limit=10)
        
        # 2. Filter by system & role
        filtered = [
            doc for doc in results
            if self._is_accessible(doc, user_system, user_role)
        ]
        
        # 3. Rank and return
        return ranked(filtered)
    
    def _is_accessible(self, doc, user_system, user_role):
        """Check if user can access document"""
        
        # Check system access
        if user_system not in doc.audience.systems:
            return False
        
        # Check role exclusion
        if user_role in doc.audience.exclude_roles:
            return False
        
        # Check system_access flag
        if user_system == "B2X.Store" and not doc.system_access.store:
            return False
        
        return True
```

### API Gateway Routing

```
User Request (Store System, customer role)
├─ Query: "How do I place an order?"
├─ System: B2X.Store
├─ Role: customer
│
└─ AI Assistant
   ├─ Load docs with system_access.store = true
   ├─ Filter out audience.exclude_roles = [admin_user]
   ├─ Return only USERDOC-STORE-* docs
   └─ Response: [USERDOC-STORE-HOW-002]
      (Never shows admin/management docs)
```

---

## Security Considerations

### Data Leakage Prevention

```
Threat: Admin documentation reveals system architecture

Prevention:
✅ YAML metadata enforces system segregation
✅ AI retrieval filters by audience
✅ Cross-system links are forbidden
✅ Pre-commit hooks catch violations
✅ Quarterly audits verify separation
```

### Access Control

```
Layer 1: YAML Metadata
  → Declares which system + roles can access

Layer 2: AI Retrieval Filtering
  → Only returns docs for user's system

Layer 3: API Gateway Routing
  → Routes to correct documentation system

Layer 4: Audit Logging
  → Tracks access attempts (security event)
```

---

## Documentation by System: Quick Reference

### Store System

```
✅ Accessible:
  - USERDOC-STORE-* (all types)
  - Public DEVDOC-* (if marked shareable)
  - Public KB-* (if marked shareable)

❌ NOT Accessible:
  - USERDOC-ADMIN-*
  - USERDOC-MGMT-*
  - Admin DEVDOC-*
  - Admin KB-*
```

### Admin System

```
✅ Accessible:
  - USERDOC-ADMIN-* (all types)
  - DEVDOC-GUIDE-* (admin operations)
  - Admin-focused DEVDOC-*
  - System KB-*

❌ NOT Accessible:
  - USERDOC-STORE-*
  - USERDOC-MGMT-*
  - Customer-facing docs
  - Business analytics docs
```

### Management System

```
✅ Accessible:
  - USERDOC-MGMT-* (all types)
  - Business DEVDOC-*
  - Analytics KB-*
  - Reporting guides

❌ NOT Accessible:
  - USERDOC-STORE-*
  - USERDOC-ADMIN-*
  - Technical system docs
  - Customer support docs
```

---

## Summary: Role-Based Segregation

```
System Segregation:
├─ Store System (B2X.Store)
│  ├─ Docs: USERDOC-STORE-*
│  ├─ Users: Customers, shop users
│  └─ Content: Shopping, orders, support
│
├─ Admin System (B2X.Admin)
│  ├─ Docs: USERDOC-ADMIN-*
│  ├─ Users: Store admins, moderators
│  └─ Content: Management, moderation, operations
│
└─ Management System (B2X.Management)
   ├─ Docs: USERDOC-MGMT-*
   ├─ Users: Managers, executives, tenant admins
   └─ Content: Analytics, reports, business operations

Rules:
✅ Each system only sees its own docs
✅ Cross-system links are forbidden
✅ YAML metadata enforces separation
✅ AI retrieval filters by system + role
✅ Quarterly audits verify segregation
```

---

**Guideline Version**: 1.0  
**Last Updated**: 2026-01-08  
**Owner**: @SARAH  
**Related**: [GL-051], [TPL-USERDOC-001], [DOCUMENT_REGISTRY]