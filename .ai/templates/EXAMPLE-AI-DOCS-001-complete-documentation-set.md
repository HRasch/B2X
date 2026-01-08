---
docid: TPL-007
title: EXAMPLE AI DOCS 001 Complete Documentation Set
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: EXAMPLE-AI-DOCS-001
title: "Example: Complete AI-Ready Documentation Set"
category: examples
type: Documentation Example
status: Active
created: 2026-01-08
---

# Example: Complete AI-Ready Documentation Set

**Purpose**: Show how templates work together for a complete feature with AI integration.

---

## 🎯 Scenario

You're documenting the **"Create Product"** feature for your e-commerce platform. You need documentation for:
- **Users** who need to create products
- **Developers** building the feature
- **Support team** helping users
- **Sales team** explaining to customers

---

## 📚 The Documentation Set

### 1. USERDOC-HOW-001: "How to Create a Product"

**DocID**: `USERDOC-HOW-001-create-product`  
**Template Used**: TPL-USERDOC-001  
**Audience**: End users, store managers  
**AI Use**: Support chatbot, onboarding  
**Location**: `docs/user/howto/USERDOC-HOW-001-create-product.md`

**Key Sections**:
```
Overview (what & time)
├─ Quick Links (jump to what you need)
├─ Quick Steps (for experienced users)
├─ Before You Start (checklist)
├─ Step-by-Step Guide (7 steps with screenshots)
├─ Screenshots (product form, categories, pricing)
├─ Troubleshooting (common problems)
├─ Get Help (support options)
├─ FAQ (pricing, editing, images)
└─ Related Articles
   ├─ USERDOC-HOW-002 (Manage Inventory)
   ├─ USERDOC-SCREEN-001 (Product Form Fields)
   └─ USERDOC-FAQ-001 (Product Questions)
```

**YAML Metadata**:
```yaml
ai_metadata:
  use_cases:
    - customer_support      # Support bot can answer "How do I create a product?"
    - step_by_step_guidance # Onboarding wizard guides through steps
    - sales_enablement      # Sales team demo
    - user_onboarding       # New user training
  time_to_complete_minutes: 10
  step_count: 7
  includes_screenshots: true
```

**AI Routing Example**:
```
Customer Question: "How do I add a product?"

AI Agent Processing:
1. Search for "add product" → Match [USERDOC-HOW-001]
2. Check ai_metadata.use_cases → Matches "customer_support"
3. Extract steps 1-7 → Present as numbered guidance
4. Add related: [USERDOC-SCREEN-001] for field reference
5. Provide: "In a hurry? See [USERDOC-HOW-001] Quick Steps"
```

---

### 2. USERDOC-SCREEN-001: "Product Form Fields"

**DocID**: `USERDOC-SCREEN-001-product-form`  
**Template Used**: TPL-USERDOC-001 (adapted for reference)  
**Audience**: End users, support team  
**AI Use**: Context-aware help, field-level tooltips  
**Location**: `docs/user/reference/USERDOC-SCREEN-001-product-form.md`

**Key Content**:
```
Overview
└─ Screenshot of Product Creation Form
   └─ Field Reference Table
      ├─ Basic Information
      │  ├─ Product Name | Required | String | "Blue T-Shirt"
      │  ├─ SKU | Required | Code | "SHIRT-BLUE-001"
      │  └─ Description | Optional | Text | "Comfortable cotton..."
      │
      ├─ Pricing
      │  ├─ Price | Required | Currency | "$19.99"
      │  ├─ Cost | Optional | Currency | "$10.00"
      │  └─ Help: Margin calculated automatically
      │
      └─ Categories
         └─ Multi-select | Required | Checkboxes | [Apparel, Accessories]

Related Articles
├─ USERDOC-HOW-001 (How to create product)
├─ USERDOC-FAQ-001 (Pricing questions)
└─ USERDOC-HOW-002 (Manage inventory)
```

**YAML Metadata**:
```yaml
ai_metadata:
  use_cases:
    - customer_support      # "What does SKU mean?"
    - step_by_step_guidance # Field-level help in wizard
  time_to_read_minutes: 3
  includes_screenshots: true
```

**AI Use Example**:
```
User in Wizard: Hovers over "SKU" field

AI Assistant:
1. Detect user on step 3
2. Load [USERDOC-SCREEN-001]
3. Find "SKU" row in table
4. Display: "SKU = Product code (example: SHIRT-BLUE-001)"
5. Offer: "Need full guide? See [USERDOC-HOW-001]"
```

---

### 3. USERDOC-FAQ-001: "Product & Pricing FAQs"

**DocID**: `USERDOC-FAQ-001-product-pricing`  
**Template Used**: TPL-USERDOC-001 (adapted for Q&A)  
**Audience**: End users, support team  
**AI Use**: FAQ chatbot, common question resolution  
**Location**: `docs/user/faqs/USERDOC-FAQ-001-product-pricing.md`

**Key Q&A**:
```
Q: Can I edit a product after publishing?
A: Yes! [Steps to edit with links to USERDOC-HOW-001]

Q: What's the difference between Price and Cost?
A: Price = what customers pay
   Cost = your cost (only you see this)
   Margin = Price - Cost (calculated automatically)

Q: Can I delete products?
A: No, but you can unpublish them. [See USERDOC-HOW-001 Step 3]

Q: How many images can I upload?
A: Up to 5 images per product. Max 5MB each.

Q: What if my price is lower than cost?
A: System prevents this for profit protection.
   [See USERDOC-HOW-001 Troubleshooting]

Related:
├─ USERDOC-HOW-001 (How to create product)
├─ USERDOC-HOW-002 (Manage inventory)
└─ USERDOC-SCREEN-001 (Pricing field reference)
```

**YAML Metadata**:
```yaml
ai_metadata:
  use_cases:
    - customer_support      # Direct Q&A matching
    - sales_enablement      # Sales FAQs to customers
  time_to_read_minutes: 5
  q_and_a_pairs: 6
```

**AI Use Example**:
```
Customer Chat: "What's the difference between Price and Cost?"

AI Agent:
1. Search FAQ for "price" AND "cost"
2. Find exact Q&A match in [USERDOC-FAQ-001]
3. Return formatted answer with links
4. Suggest: "Learn how to set pricing: [USERDOC-HOW-001]"
```

---

### 4. DEVDOC-GUIDE-001: "Product Creation Feature"

**DocID**: `DEVDOC-GUIDE-001-product-creation`  
**Template Used**: TPL-DEVDOC-001  
**Audience**: Developers, architects  
**AI Use**: Code generation, onboarding engineers, architecture review  
**Location**: `docs/developer/guides/DEVDOC-GUIDE-001-product-creation.md`

**Key Sections**:
```
Overview: What is Product Creation Feature?

Core Concepts:
├─ Product = Catalog item with price, description, images
├─ SKU = Unique identifier in system
├─ Catalog = Collection of products per tenant
└─ Related: [KB-006] Wolverine Patterns

Architecture:
├─ Diagram: Request → Gateway → ProductService → Database
├─ Components:
│  ├─ ProductController (handles HTTP requests)
│  ├─ CreateProductCommand (Wolverine CQRS)
│  ├─ ProductService (business logic)
│  └─ ProductRepository (data access)
└─ Related: [ADR-001] Why Wolverine?

Code Examples:
├─ Example 1: Create Product (C# handler)
   └─ Shows: Command creation, validation, save
├─ Example 2: Error Handling
   └─ Shows: Duplicate SKU handling
└─ Example 3: Testing
   └─ Shows: Unit test + integration test

Common Patterns:
├─ Validation Pattern → Link to code
├─ Error Handling Pattern → Link to code
└─ Multi-tenant Pattern → Link to [DEVDOC-GUIDE-002]

Getting Started:
├─ Prerequisites: .NET 10, PostgreSQL
├─ Step 1: Read [KB-006] Wolverine Patterns
├─ Step 2: Review [DEVDOC-ARCH-001] System Architecture
├─ Step 3: Study examples above
├─ Step 4: Implement feature following pattern

Troubleshooting:
├─ "Duplicate SKU error" → [See DEVDOC-GUIDE-002]
├─ "Migration issues" → [See backend/Domain/Catalog/migrations]
└─ "Performance" → [See DEVDOC-GUIDE-003] Optimization

Related Resources:
├─ DEVDOC-ARCH-001 (System Architecture)
├─ DEVDOC-API-001 (Product API Reference)
├─ DEVDOC-GUIDE-002 (Inventory Management)
├─ ADR-001 (Why Wolverine)
├─ KB-006 (Wolverine Patterns)
└─ src/Domain/Catalog/ (Implementation)
```

**YAML Metadata**:
```yaml
ai_metadata:
  use_cases:
    - code_generation        # Generate handlers from pattern
    - system_understanding   # Understand feature architecture
    - onboarding_engineers   # New dev learning
  time_to_read_minutes: 20
  includes_code_examples: true
  includes_diagrams: true
  difficulty_level: intermediate
```

**AI Use Example**:
```
New Developer: "How do I implement a new product field?"

AI Assistant:
1. Load [DEVDOC-GUIDE-001] Code Examples
2. Extract handler pattern
3. Load [KB-006] Wolverine Patterns
4. Suggest: "Follow pattern in DEVDOC-GUIDE-001 Example 1"
5. Generate code stub from pattern + examples
6. Link to: [DEVDOC-ARCH-001] for validation placement
```

---

### 5. DEVDOC-API-001: "Product API Reference"

**DocID**: `DEVDOC-API-001-product-api`  
**Template Used**: TPL-DEVDOC-001 (adapted for API)  
**Audience**: Developers, API consumers  
**AI Use**: API code generation, integration testing  
**Location**: `docs/developer/api/DEVDOC-API-001-product-api.md`

**Key Content**:
```
Overview: Product API Endpoints

Endpoints:
├─ POST /api/products
│  ├─ Request: { name, sku, price, cost, description, images[] }
│  ├─ Response: { id, ...product, createdAt }
│  ├─ Errors: 400 (validation), 409 (duplicate SKU), 401 (auth)
│  └─ Example: [Full request/response JSON]
│
├─ GET /api/products/:id
│  └─ ...
│
└─ PUT /api/products/:id
   └─ ...

Code Examples:
├─ C# Client
├─ JavaScript/TypeScript
├─ cURL
└─ Postman Collection

Related:
├─ DEVDOC-GUIDE-001 (Feature implementation)
├─ DEVDOC-SCREEN-001 (User interface)
└─ OpenAPI Specification: [openapi.yaml]
```

---

## 🔗 How They Connect (Knowledge Graph)

```
USERDOC-HOW-001 (Create Product - User Guide)
├─ Links to: [USERDOC-SCREEN-001] (Field reference)
├─ Links to: [USERDOC-FAQ-001] (Common questions)
├─ Links to: [USERDOC-HOW-002] (Next: Manage inventory)
└─ AI learns: User journey path

USERDOC-SCREEN-001 (Product Form - Field Reference)
├─ Links to: [USERDOC-HOW-001] (How to use form)
├─ Links to: [USERDOC-FAQ-001] (Field help)
└─ AI learns: Context-aware field help

DEVDOC-GUIDE-001 (Product Feature - Developer Guide)
├─ Links to: [DEVDOC-API-001] (API reference)
├─ Links to: [DEVDOC-ARCH-001] (System architecture)
├─ Links to: [KB-006] (Wolverine patterns)
├─ Links to: [ADR-001] (Why this architecture)
└─ AI learns: Implementation path

DEVDOC-API-001 (Product API)
├─ Links to: [DEVDOC-GUIDE-001] (Implementation)
└─ AI learns: Integration patterns
```

---

## 🤖 AI Assistant Conversations Using This Set

### Conversation 1: Customer Support Chatbot

```
Customer: "How do I create a product?"

AI Processing:
1. Retrieve: [USERDOC-HOW-001]
2. Check metadata: ai_metadata.use_cases includes "customer_support" ✓
3. Generate response using "Quick Steps" section
4. Offer: "Full guide with screenshots: [USERDOC-HOW-001]"
5. Offer: "Questions? Check FAQs: [USERDOC-FAQ-001]"

Customer: "What does SKU mean?"

AI Processing:
1. Retrieve: [USERDOC-SCREEN-001] field reference table
2. Extract SKU row: "Product code (e.g., SHIRT-BLUE-001)"
3. Link to: [USERDOC-FAQ-001] for more
4. Suggest: [USERDOC-HOW-001] Step 3 for guidance
```

### Conversation 2: Sales Enablement

```
Sales Rep: "What should I tell customer about pricing?"

AI Processing:
1. Retrieve: [USERDOC-FAQ-001] "Price vs Cost"
2. Retrieve: [USERDOC-HOW-001] pricing section
3. Generate: Sales-friendly explanation with benefits
4. Add demo: [USERDOC-HOW-001] Quick Steps
5. Provide: Talking points + demo guide
```

### Conversation 3: Developer Onboarding

```
New Dev: "How do I add a new field to products?"

AI Processing:
1. Retrieve: [DEVDOC-GUIDE-001] Code Examples
2. Extract: Product handler pattern
3. Load: [KB-006] Wolverine patterns
4. Generate: Code stub for new field
5. Link to: [DEVDOC-ARCH-001] for validation placement
6. Suggest: Run tests per [DEVDOC-GUIDE-001] Example 3
```

### Conversation 4: User Onboarding Wizard

```
New User Day 1:

AI Lesson Plan:
1. Read: [USERDOC-HOW-001] Overview (2 min)
2. Watch: Screenshot walkthrough [USERDOC-HOW-001] (3 min)
3. Practice: Create first product (10 min)
   └─ Step-by-step guidance from [USERDOC-HOW-001]
4. Check: FAQs [USERDOC-FAQ-001] (2 min)

New User Day 2:
1. Learn: [USERDOC-HOW-002] Manage Inventory
2. Related: [USERDOC-PROC-001] Product lifecycle

✓ Progressive difficulty increase
✓ Links to next learning
```

---

## 📊 Quality Metrics

### Content Completeness

| Aspect | Coverage | Status |
|--------|----------|--------|
| How to create | ✅ USERDOC-HOW-001 | Complete |
| Field reference | ✅ USERDOC-SCREEN-001 | Complete |
| FAQs | ✅ USERDOC-FAQ-001 | 6 Q&A pairs |
| Developer guide | ✅ DEVDOC-GUIDE-001 | Complete |
| API reference | ✅ DEVDOC-API-001 | Complete |
| Architecture | ✅ DEVDOC-ARCH-001 | (existing) |
| Cross-linking | ✅ 20+ links | Complete |

### AI Integration Readiness

| Requirement | Status | Notes |
|---|---|---|
| YAML metadata | ✅ | All docs have ai_metadata |
| use_cases specified | ✅ | Mapped to 4 use cases |
| Structured sections | ✅ | Follow templates |
| DocID linking | ✅ | No generic links |
| Screenshots | ✅ | 3+ per user doc |
| Code examples | ✅ | Simple to advanced |
| FAQ section | ✅ | 6+ questions |
| Maintenance date | ✅ | Fresh (< 30 days) |

---

## 🎓 Learning Paths Created by This Set

### Path 1: User Learning (5 days)

**Day 1**: Get started
- Read: [USERDOC-START-001] Getting started (5 min)
- Read: [USERDOC-HOW-001] Create product (10 min)
- Practice: Create first product

**Day 2**: Core workflows
- Read: [USERDOC-HOW-002] Manage inventory (10 min)
- Practice: Add and adjust inventory

**Day 3**: Advanced features
- Read: [USERDOC-HOW-003] Pricing & discounts (10 min)
- Read: [USERDOC-HOW-004] Promotions (10 min)

**Days 4-5**: Reference as needed
- Search: FAQs for common questions
- Reference: Field guide [USERDOC-SCREEN-001]
- Contact: Support if stuck

### Path 2: Developer Learning (1 week)

**Day 1**: Foundation
- Read: [DEVDOC-ARCH-001] System architecture (20 min)
- Read: [KB-006] Wolverine patterns (15 min)
- Study: [ADR-001] Architecture decisions (10 min)

**Day 2**: Feature implementation
- Read: [DEVDOC-GUIDE-001] Product creation (20 min)
- Study: Code examples (15 min)
- Review: Test examples (10 min)

**Day 3**: API integration
- Read: [DEVDOC-API-001] Product API (15 min)
- Review: cURL examples (10 min)
- Implement: Simple integration (30 min)

**Day 4-5**: Hands-on practice
- Implement: New field following pattern
- Test: Unit + integration tests
- Review: Architecture per [DEVDOC-ARCH-001]

---

## ✅ Success Criteria: Met

✅ **Unified structure**: All docs follow templates  
✅ **Clear linking**: 20+ DocID links create knowledge graph  
✅ **AI metadata**: Each doc has complete YAML front-matter  
✅ **Audience clarity**: Separate paths for users vs developers  
✅ **Complete coverage**: How-to, reference, FAQ, architecture, API  
✅ **Learning paths**: Progressive complexity from beginner to advanced  
✅ **AI-ready**: Can be used by chatbot, wizard, training system  
✅ **Maintenance plan**: Version tracking, update dates  

---

## 🚀 Next Steps

1. **Use These Templates**: Start new features with TPL-DEVDOC/USERDOC
2. **Build Knowledge Graph**: Link new docs to related content
3. **Train AI Assistants**: Feed this documentation set to LLM
4. **Measure Success**: Track support ticket reduction, user completion rates
5. **Iterate**: Update based on support questions and AI feedback

---

**Example Version**: 1.0  
**Last Updated**: 2026-01-08  
**Related**: [TPL-DEVDOC-001], [TPL-USERDOC-001], [GL-051], [QS-AI-DOCS]