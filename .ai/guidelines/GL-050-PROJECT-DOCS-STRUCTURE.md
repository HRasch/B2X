---
docid: GL-087
title: GL 050 PROJECT DOCS STRUCTURE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-050
title: Project Documentation Structure in docs/ Directory
owner: "@DocMaintainer"
status: Active
created: 2026-01-08
---

# Project Documentation Structure in docs/

**DocID**: `GL-050`  
**Purpose**: Define clear organization of docs/ directory with developer and user documentation  
**Owner**: @DocMaintainer  
**Status**: Active

---

## 🎯 Two Main Categories

All documentation in the `docs/` directory is clearly identified and organized into two distinct categories for different audiences.

---

## 📁 Directory Structure

```
docs/
├── developer/                     # DEVDOC-* (Technical for developers)
│   ├── architecture/              # System design, technical decisions
│   ├── api/                       # API documentation, interfaces, endpoints
│   ├── guides/                    # Developer guides, coding patterns
│   ├── features/                  # Feature technical documentation
│   ├── howto/                     # Technical how-to guides for developers
│   └── faq/                       # Technical FAQ
│
├── user/                          # USERDOC-* (User manual for end users)
│   ├── getting-started/           # Installation, setup, first steps
│   ├── features/                  # Feature descriptions (user perspective)
│   ├── howto/                     # User how-to guides (step-by-step)
│   ├── system-overview/           # System description, architecture overview
│   ├── process-guides/            # Business process explanations
│   ├── screen-explanations/       # Page/screen descriptions
│   └── faq/                       # User FAQ
│
├── QUICK_REFERENCE.md             # Quick access guide (both audiences)
└── README.md                      # Documentation index
```

---

## 1. 👨‍💻 DEVELOPER DOCUMENTATION (DEVDOC-*)

**Purpose**: Technical documentation for developers and architects

**Target Audience**: Software developers, architects, technical team members

**DocID Format**: `DEVDOC-###-short-description.md`

**Example DocIDs**:
- `DEVDOC-001-architecture-overview.md`
- `DEVDOC-002-api-reference.md`
- `DEVDOC-003-database-schema.md`
- `DEVDOC-004-authentication-flow.md`
- `DEVDOC-005-coding-patterns.md`
- `DEVDOC-006-deployment-architecture.md`

### Subdirectories & Content

#### **architecture/** - `DEVDOC-ARCH-*`
System design, technical decisions, architecture patterns

Files:
- `DEVDOC-ARCH-001-system-design.md` - Overall system architecture
- `DEVDOC-ARCH-002-microservices-layout.md` - Service decomposition
- `DEVDOC-ARCH-003-data-flow.md` - Data movement between services
- `DEVDOC-ARCH-004-integration-patterns.md` - How systems integrate
- Reference: Cross-link to [ADR-*] decisions

#### **api/** - `DEVDOC-API-*`
API documentation, interfaces, endpoints, contracts

Files:
- `DEVDOC-API-001-store-gateway.md` - Store API endpoints
- `DEVDOC-API-002-admin-gateway.md` - Admin API endpoints
- `DEVDOC-API-003-authentication.md` - Auth endpoints & flow
- Auto-generated from OpenAPI specs when available

#### **guides/** - `DEVDOC-GUIDE-*`
How-to guides for developers, coding patterns, development workflows

Files:
- `DEVDOC-GUIDE-001-local-development-setup.md` - Get dev environment running
- `DEVDOC-GUIDE-002-adding-new-domain.md` - Create bounded context
- `DEVDOC-GUIDE-003-database-migrations.md` - Schema changes
- `DEVDOC-GUIDE-004-testing-patterns.md` - Unit/integration testing
- `DEVDOC-GUIDE-005-debugging.md` - Troubleshooting & debugging

#### **features/** - `DEVDOC-FEAT-*`
Technical implementation documentation for specific features

Files:
- `DEVDOC-FEAT-001-catalog-management.md` - Product catalog implementation
- `DEVDOC-FEAT-002-order-processing.md` - Order workflow
- `DEVDOC-FEAT-003-payment-integration.md` - Payment system design
- `DEVDOC-FEAT-004-search-implementation.md` - Elasticsearch integration

#### **howto/** - `DEVDOC-HOW-*`
Step-by-step technical how-to guides for developers

Files:
- `DEVDOC-HOW-001-run-tests.md` - Execute test suite
- `DEVDOC-HOW-002-deploy-locally.md` - Deploy to local environment
- `DEVDOC-HOW-003-debug-issue.md` - Debug procedure
- `DEVDOC-HOW-004-add-localization.md` - Add translation keys
- `DEVDOC-HOW-005-configure-logging.md` - Set up logging

#### **faq/** - `DEVDOC-FAQ-*`
Technical FAQ for developers

Files:
- `DEVDOC-FAQ-001.md` - "Why does X fail with Y error?"
- `DEVDOC-FAQ-002.md` - "How do I configure Z?"
- `DEVDOC-FAQ-003.md` - "What's the difference between A and B?"

---

## 2. 👥 USER DOCUMENTATION (USERDOC-*)

**Purpose**: User manual for end users and business users

**Target Audience**: Shop users, admins, business users, end customers

**DocID Format**: `USERDOC-###-short-description.md`

**Example DocIDs**:
- `USERDOC-001-getting-started.md`
- `USERDOC-002-catalog-overview.md`
- `USERDOC-003-search-guide.md`
- `USERDOC-004-checkout-process.md`
- `USERDOC-005-account-management.md`
- `USERDOC-006-order-history.md`

### Subdirectories & Content

#### **getting-started/** - `USERDOC-START-*`
Installation, setup, first-time use, onboarding

Files:
- `USERDOC-START-001-installation.md` - How to install/access the system
- `USERDOC-START-002-account-creation.md` - Create user account
- `USERDOC-START-003-first-login.md` - First-time login walkthrough
- `USERDOC-START-004-profile-setup.md` - Set up user profile/preferences
- `USERDOC-START-005-language-selection.md` - Choose language/region

#### **features/** - `USERDOC-FEAT-*`
Feature descriptions from user perspective with examples

Files:
- `USERDOC-FEAT-001-product-search.md` - Search functionality for users
- `USERDOC-FEAT-002-filters-sorting.md` - How to use filters & sorting
- `USERDOC-FEAT-003-wishlists.md` - Wishlists functionality
- `USERDOC-FEAT-004-shopping-cart.md` - Cart management
- `USERDOC-FEAT-005-my-orders.md` - Order history & tracking
- `USERDOC-FEAT-006-account-settings.md` - Account & profile settings

#### **howto/** - `USERDOC-HOW-*`
Step-by-step how-to guides (procedural, user-focused)

Files:
- `USERDOC-HOW-001-search-products.md` - "How to find products"
- `USERDOC-HOW-002-add-to-cart.md` - "How to add items to cart"
- `USERDOC-HOW-003-checkout.md` - "How to complete purchase"
- `USERDOC-HOW-004-track-order.md` - "How to track my order"
- `USERDOC-HOW-005-return-product.md` - "How to return an item"
- `USERDOC-HOW-006-update-address.md` - "How to update shipping address"
- `USERDOC-HOW-007-change-password.md` - "How to change password"

#### **system-overview/** - `USERDOC-SYS-*`
System description, feature overview, business processes

Files:
- `USERDOC-SYS-001-platform-overview.md` - What is this platform?
- `USERDOC-SYS-002-user-roles.md` - User types & roles
- `USERDOC-SYS-003-permissions.md` - What can each user type do?
- `USERDOC-SYS-004-workflow.md` - Overall business workflow
- `USERDOC-SYS-005-supported-browsers.md` - System requirements

#### **process-guides/** - `USERDOC-PROC-*`
Business process explanations, workflows, order fulfillment

Files:
- `USERDOC-PROC-001-ordering-process.md` - How orders work
- `USERDOC-PROC-002-payment-process.md` - Payment workflow
- `USERDOC-PROC-003-delivery-process.md` - Delivery & shipping
- `USERDOC-PROC-004-return-process.md` - Return workflow
- `USERDOC-PROC-005-subscription-process.md` - Subscription management

#### **screen-explanations/** - `USERDOC-SCREEN-*`
Page/screen descriptions, button explanations, form guides

Files:
- `USERDOC-SCREEN-001-homepage.md` - Home page elements & navigation
- `USERDOC-SCREEN-002-product-page.md` - Product detail page explanation
- `USERDOC-SCREEN-003-cart-page.md` - Shopping cart interface
- `USERDOC-SCREEN-004-checkout-page.md` - Checkout form & fields
- `USERDOC-SCREEN-005-account-dashboard.md` - Account overview page
- `USERDOC-SCREEN-006-order-details.md` - Order detail page

#### **faq/** - `USERDOC-FAQ-*`
User FAQ - common questions from users

Files:
- `USERDOC-FAQ-001.md` - "What payment methods do you accept?"
- `USERDOC-FAQ-002.md` - "How long does delivery take?"
- `USERDOC-FAQ-003.md` - "What is your return policy?"
- `USERDOC-FAQ-004.md` - "How do I contact support?"
- `USERDOC-FAQ-005.md` - "Is my personal data safe?"

---

## 📋 Quick Reference: DocID Mapping

### Developer Documentation
| Location | DocID Pattern | Examples |
|----------|---|---|
| `developer/architecture/` | `DEVDOC-ARCH-*` | DEVDOC-ARCH-001 |
| `developer/api/` | `DEVDOC-API-*` | DEVDOC-API-001 |
| `developer/guides/` | `DEVDOC-GUIDE-*` | DEVDOC-GUIDE-001 |
| `developer/features/` | `DEVDOC-FEAT-*` | DEVDOC-FEAT-001 |
| `developer/howto/` | `DEVDOC-HOW-*` | DEVDOC-HOW-001 |
| `developer/faq/` | `DEVDOC-FAQ-*` | DEVDOC-FAQ-001 |

### User Documentation
| Location | DocID Pattern | Examples |
|----------|---|---|
| `user/getting-started/` | `USERDOC-START-*` | USERDOC-START-001 |
| `user/features/` | `USERDOC-FEAT-*` | USERDOC-FEAT-001 |
| `user/howto/` | `USERDOC-HOW-*` | USERDOC-HOW-001 |
| `user/system-overview/` | `USERDOC-SYS-*` | USERDOC-SYS-001 |
| `user/process-guides/` | `USERDOC-PROC-*` | USERDOC-PROC-001 |
| `user/screen-explanations/` | `USERDOC-SCREEN-*` | USERDOC-SCREEN-001 |
| `user/faq/` | `USERDOC-FAQ-*` | USERDOC-FAQ-001 |

---

## 🔗 Cross-Referencing

Use DocIDs consistently:

```markdown
# For developers:
See [DEVDOC-ARCH-001] for system architecture overview
Reference [DEVDOC-API-001] for Store API endpoints
Follow [DEVDOC-GUIDE-001] to set up local development

# For users:
Start with [USERDOC-START-001] for installation
See [USERDOC-HOW-001] for step-by-step search guide
Check [USERDOC-FAQ-001] for payment questions
```

---

## 📊 Front-matter Template

Every documentation file should include metadata:

```yaml
---
docid: DEVDOC-001-architecture-overview
title: System Architecture Overview
category: developer/architecture
audience: Developers, Architects
status: Active
last_updated: 2026-01-08
version: 1.0
language: en
---
```

---

## 🎯 Index Files

### `docs/README.md`
- Links to DEVDOC and USERDOC indexes
- Search guide
- Common paths for different user types

### `docs/QUICK_REFERENCE.md`
- Most frequently accessed docs
- Quick links for both developers and users
- Keyboard shortcuts (if applicable)

### `docs/developer/README.md`
- Developer documentation index
- Organized by topic
- Learning paths for new developers

### `docs/user/README.md`
- User documentation index
- Organized by task
- Step-by-step learning paths for new users

---

## 📝 Writing Guidelines

### For Developer Docs (DEVDOC-*)
- Assume technical knowledge
- Include code examples, configurations
- Link to source code
- Reference [KB-*] and [ADR-*] for decisions
- Use technical terminology accurately

### For User Docs (USERDOC-*)
- Simple, clear language (avoid jargon)
- Step-by-step instructions
- Include screenshots where helpful
- Real-world examples
- Link to related user docs
- Provide context: "Why would I do this?"

---

## 🔄 Migration Path

### Phase 1: Create New Structure
- Create `docs/developer/` and `docs/user/` directories
- Create subdirectories per structure

### Phase 2: Inventory Existing Docs
- Scan existing `docs/` files
- Classify as DEVDOC or USERDOC
- Assign DocID based on content

### Phase 3: Move & Rename
- Move files to appropriate subdirectory
- Rename with DocID prefix
- Update all cross-references

### Phase 4: Update INDEX Files
- Create index files for each category
- Link from main README
- Update DOCUMENT_REGISTRY.md

---

## ✅ Quality Checklist

For each documentation file:
- [ ] Has clear DocID (DEVDOC-* or USERDOC-*)
- [ ] In correct directory for category
- [ ] Has YAML front-matter with metadata
- [ ] Written for correct audience
- [ ] Includes relevant links/references
- [ ] Up-to-date (no outdated info)
- [ ] Has clear title and structure
- [ ] Screenshots or examples where helpful
- [ ] Links are verified and not broken

---

## 🔗 Related Documents

- [GL-049] Documentation Structure Framework (5 categories)
- [BS-DOCUMENTATION-CLEANUP-STRATEGY] Cleanup roadmap
- [DOCUMENT_REGISTRY.md] Full document registry
- [DOC-005] Development Guide

---

**Effective Date**: 2026-01-08  
**Status**: Active  
**Next Review**: 2026-02-08 (after Phase 1 migration completion)