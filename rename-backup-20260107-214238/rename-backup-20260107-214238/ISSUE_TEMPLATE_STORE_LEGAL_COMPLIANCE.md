# Store Legal Compliance Implementation (P0.6)

## Overview

Implement EU e-commerce legal compliance for B2X store (B2C & B2B modes). This ensures all shops can legally operate in the DACH region (Germany, Austria, Switzerland) and wider EU.

**Regulatory Framework:**
- **PAngV** (Preisangaben-Verordnung): Price transparency requirements
- **VVVG** (Verbraucher-Verordnung): 14-day withdrawal/return rights
- **TMG** (Telemediengesetz): Impressum & legal disclosure requirements
- **GDPR Art. 13/14**: Privacy policy requirements
- **AStV** (Umsatzsteuer): VAT handling & reverse charge for B2B
- **ODR-VO**: Alternative dispute resolution links

**Priority:** 🔴 P0 (Compliance-Critical)  
**Effort:** 60 hours (2 Backend Devs, 1.5 weeks)  
**Tests:** 15 test cases required (see P0.6_ECOMMERCE_LEGAL_TESTS.md)

---

## Acceptance Criteria

### 1. B2C Price Transparency (PAngV)

- [ ] **Final Price Display**: Always show price **including VAT** (inkl. MwSt)
  - Example: "€99,99 inkl. MwSt" not "€84,20 + 19% MwSt"
  - Display on product listing, product detail, cart, checkout

- [ ] **Shipping Costs Visible**: Show shipping costs **before checkout**
  - Calculated per country selection
  - Dynamic update when customer changes country
  - Display format: "Shipping: €4,99 to Germany"

- [ ] **Original Price on Discounts**: When discounted, show original price
  - Format: "~~€199,99~~ €99,99" (strikethrough original)
  - Only show if actually discounted

- [ ] **Tax Breakdown on Invoice**: Show VAT amount separately
  - Line items: Product Price × VAT Rate = VAT Amount
  - Total format: Subtotal + VAT Amount = Final Total

**Tests:**
```gherkin
Given a product costs €100 (net)
And VAT rate is 19%
When customer views product
Then price displayed as "€119,00 inkl. MwSt"
And NOT displayed as "€100,00 + 19%"

Given customer in Germany
When customer adds item to cart
And selects different shipping country (Austria)
Then shipping cost recalculates automatically
And new total displayed
```

---

### 2. B2C Withdrawal Rights (14-Day Returns, VVVG)

- [ ] **Withdrawal Period**: 14 days from delivery (not from order date)
  - Auto-calculated based on delivery date
  - Show remaining days in customer account
  - Block return requests after 14 days

- [ ] **Return Form**: Downloadable/printable return form
  - Pre-filled with order number
  - Includes customer address for return shipping
  - Contains withdrawal statement

- [ ] **Return Label Generation**: Auto-generate return shipping label
  - Integrated with carrier API (DHL, DPD, etc.)
  - PDF download with tracking number
  - Show refund policy for return costs

- [ ] **Refund Processing**: Process within 14 days of withdrawal request
  - Automated refund via original payment method
  - Status notification email to customer
  - Audit log entry for each refund

- [ ] **Withdrawal Notification UI**: Email template
  - "Widerrufsbelehrung" (withdrawal notice) in order confirmation
  - Link to withdrawal form
  - Instructions for return process

**Tests:**
```gherkin
Given an order delivered on 2025-12-28
When customer requests withdrawal on 2026-01-05
Then withdrawal is accepted (7 days < 14 days)

Given an order delivered on 2025-12-28
When customer requests withdrawal on 2026-01-15
Then withdrawal is rejected (17 days > 14 days)
And error message: "Withdrawal period expired"
```

---

### 3. Legal Documents & Disclosure (TMG & GDPR)

- [ ] **Impressum (Legal Notice)**: Shop-specific, editable in admin
  - Company name
  - Address
  - Email & phone
  - Legal form (GmbH, e.K., etc.)
  - VAT-ID / Tax number
  - Responsible person name & contact
  - Link in footer on every page

- [ ] **Privacy Policy**: GDPR-compliant, shop-specific
  - Data processing purposes (orders, marketing, analytics)
  - Legal basis for each processing
  - Recipient of data (e.g., payment processor)
  - Retention periods
  - User rights (access, deletion, portability)
  - Data Protection Officer contact (if applicable)
  - Link in footer on every page

- [ ] **Terms & Conditions (AGB)**: Shop-specific, B2C/B2B variants
  - Mandatory acceptance before checkout (checkbox + timestamp)
  - Include withdrawal statement (VVVG)
  - Payment terms
  - Shipping terms
  - Liability limitations
  - Dispute resolution (ODR)
  - Shop-specific customizations

- [ ] **Audit Trail for Legal Changes**
  - Version history of all legal documents
  - Customer notification when significant terms change
  - Archive of old versions (7-year legal requirement)

**Tests:**
```gherkin
Given a shop with updated Terms & Conditions
When a customer checks out
Then Terms & Conditions checkbox is **required** (not optional)
And customer cannot proceed without accepting

When customer accepts Terms
Then acceptance logged with:
  - Timestamp
  - Document version
  - Customer ID
  - IP address
```

---

### 4. VAT Handling & Invoicing (AStV)

#### B2C: Simple VAT
- [ ] **VAT Calculation**: Apply shop's country VAT rate
  - Germany: 19% standard, 7% reduced
  - Austria: 20% standard, 10% reduced
  - Different rates per product category (e.g., books 7%)
  - Show in price: "Incl. 19% VAT"

#### B2B: Reverse Charge Logic
- [ ] **VAT-ID Validation**: Verify via VIES API (VAT Information Exchange System)
  - Input: EU country code + VAT number
  - Lookup: Official EU VIES database
  - Valid response includes company name & address
  - Store validation result & expiry (1 year)

- [ ] **Reverse Charge Rule**: No VAT if buyer has valid VAT-ID
  - IF buyer is EU business with valid VAT-ID
  - AND buyer country ≠ seller country
  - THEN no VAT charged (reverse charge applies)
  - Buyer's country must pay VAT instead

- [ ] **Invoice Modifications for B2B**
  - Show "Reverse Charge (Art. 199a Directive 2006/112/EC)" on invoice
  - VAT Amount: 0€
  - Show buyer's VAT-ID on invoice
  - Include seller's VAT-ID

**Tests:**
```gherkin
Given a B2C German customer with €100 product
When checking out
Then VAT 19% = €19
And total = €119

Given a B2B Austrian customer with valid VAT-ID
When checking out
Then VIES validation called for AT-VAT-ID
And returns valid company name
And VAT 0% (reverse charge)
And total = €100 (no VAT)
And invoice shows "Reverse Charge"
```

---

### 5. Invoice Management (10-Year Archival)

- [ ] **Invoice Generation**: Automatic PDF generation per order
  - Invoice number (unique per shop)
  - Order items with VAT breakdown
  - Customer details (encrypted)
  - Payment method
  - Delivery address
  - Issue date & due date (for B2B)
  - Shop's legal entity info (from Impressum)
  - Signature placeholder (eIDAS-ready)

- [ ] **Invoice Encryption**: Encrypted PDF storage
  - AES-256 encryption
  - Stored securely in cloud storage
  - Accessible only to shop & customer
  - Immutable (once created, cannot be modified)

- [ ] **10-Year Retention**: Comply with German tax law
  - Minimum 6 years (GDPR)
  - Minimum 10 years (German tax law § 257 HGB)
  - Archive old invoices (> 10 years) automatically
  - Retention policy per tenant configurable

- [ ] **Customer Access**: Invoice download in customer account
  - Retrieve all past invoices
  - Filter by date range
  - Download as PDF
  - Audit log: who accessed which invoice, when

- [ ] **Email Delivery**: Send invoice immediately after order
  - HTML + PDF attachment
  - Include order details
  - Include return instructions

**Tests:**
```gherkin
Given an order is completed
When payment is confirmed
Then invoice PDF generated within 1 second
And invoice encrypted with AES-256
And stored with immutability verification
And email sent to customer with PDF

Given 10+ year old invoices
When archival job runs daily
Then old invoices moved to archive
And marked IsArchived = true
And still accessible via customer account (read-only)
```

---

### 6. Shop-Specific Legal Document Management

- [ ] **Admin UI**: Edit legal documents per shop
  - Impressum form (company info, contact)
  - Privacy Policy (copy-paste or template)
  - Terms & Conditions (copy-paste or template, B2C/B2B toggle)
  - Version control & audit trail
  - Preview before publishing
  - Activation timestamp

- [ ] **Frontend Display**: Legal documents accessible
  - Impressum link in footer (visible on every page)
  - Privacy Policy link in footer
  - Terms & Conditions link in footer
  - Full text readable without login
  - Clear, accessible typography

- [ ] **Checkout Integration**: Legal acceptance
  - Show Terms & Conditions checkbox (required)
  - Show Privacy Policy checkbox (optional but recommended)
  - Store acceptance with timestamp & version
  - Cannot checkout without accepting Terms

**Tests:**
```gherkin
Given a shop admin is logged in
When navigating to Legal Documents settings
Then can edit:
  - Impressum (company name, address, contact, VAT-ID)
  - Privacy Policy (GDPR-compliant template)
  - Terms & Conditions (with withdrawal notice pre-filled)

When customer views checkout page
Then must see:
  - "I accept Terms & Conditions" checkbox (required)
  - "I accept Privacy Policy" checkbox (recommended)
  - Cannot proceed without accepting Terms
```

---

### 7. B2B-Specific: Payment Terms & Shipping Terms

- [ ] **Payment Terms**: Configurable per shop
  - Options: Net 30, Net 60, Net 90, COD (Cash on Delivery), etc.
  - Due date calculated from invoice date
  - Shown on invoice
  - Enforced in payment processing (e.g., don't process until due)

- [ ] **INCOTERMS Support**: International commercial terms
  - DDP (Delivered Duty Paid) - seller pays all costs
  - DDU (Delivered Duty Unpaid) - buyer pays import duty
  - CIF (Cost, Insurance & Freight) - for ocean shipping
  - FOB (Free on Board) - buyer arranges shipping from port
  - Selected at checkout, shown on invoice
  - Affects shipping cost calculation & responsibility

- [ ] **Minimum Order Values**: Configurable per shop/product
  - Prevent orders below minimum
  - Show message on checkout: "Minimum order value: €500"
  - Can be waived by admin override

**Tests:**
```gherkin
Given a B2B customer with Net 60 terms
When creating order on 2025-12-28
Then invoice due date = 2026-02-26

Given DDP shipping selected
When customer checks shipping cost
Then cost includes all import duties (seller's responsibility)
And invoice shows "DDP (Delivered Duty Paid)"
```

---

## Implementation Tasks

### Backend (C# / .NET)

1. **Price Calculation Service** (/backend/Domain/Catalog/Application)
   - `PriceCalculationService`: Calculate price by country, product, VAT rate
   - Return: `PriceBreakdown` with base, VAT, shipping, total
   - Tests: 5 test cases

2. **VAT Service** (/backend/Domain/Catalog/Application)
   - `VatIdValidationService`: Call VIES API for B2B VAT-ID validation
   - `ReverseChargeService`: Determine if reverse charge applies
   - Tests: 3 test cases

3. **Invoice Service** (/backend/Domain/Catalog/Infrastructure)
   - `InvoiceGenerationService`: Create PDF invoices
   - `InvoiceStorageService`: Encrypt & store PDFs (10-year archival)
   - `InvoiceRetrievalService`: Return encrypted invoices to customers
   - Tests: 4 test cases

4. **Return/Withdrawal Service** (/backend/Domain/Orders/Application)
   - `ReturnManagementService`: Check 14-day window, generate labels
   - `RefundService`: Process refunds within 14 days
   - Tests: 3 test cases

5. **Legal Documents Service** (/backend/Domain/Catalog/Application)
   - `LegalDocumentsService`: Store & retrieve shop-specific legal docs
   - Version control & audit trail
   - Tests: 2 test cases

### Frontend (Vue.js / TypeScript)

6. **Checkout Flow Updates** (/frontend/Store/src/views)
   - Display final price with VAT
   - Show shipping cost before payment
   - Mandatory accept Terms & Conditions checkbox
   - Display return instructions

7. **Legal Documents Display** (/frontend/Store/src/components)
   - Footer links (Impressum, Privacy, Terms)
   - Full-page legal document viewers
   - Responsive, accessible typography

8. **Customer Account** (/frontend/Store/src/views)
   - Invoice history & download
   - Return management UI
   - Withdrawal form download

---

## Definition of Done

Before closing this issue:

- [ ] All 15 test cases passing (xUnit backend + Playwright E2E)
- [ ] Price displays with VAT everywhere (product, cart, checkout, invoice)
- [ ] Shipping costs calculated & visible before checkout
- [ ] 14-day return period enforced with return label generation
- [ ] Legal documents editable per shop in admin
- [ ] B2B: VAT-ID validation via VIES API working
- [ ] B2B: Reverse charge logic tested
- [ ] Invoices encrypted, stored 10 years, accessible to customers
- [ ] Acceptance of Terms & Conditions logged with version + timestamp
- [ ] Code review approved by Tech Lead
- [ ] Legal review approved by Compliance Officer
- [ ] Performance: Invoice generation < 2 seconds
- [ ] Documentation updated (admin guide, user guide)

---

## Related Documentation

- [P0.6_ECOMMERCE_LEGAL_TESTS.md](docs/P0.6_ECOMMERCE_LEGAL_TESTS.md) - 15 test specifications
- [ECOMMERCE_LEGAL_OVERVIEW.md](docs/ECOMMERCE_LEGAL_OVERVIEW.md) - Regulatory details
- [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) - Full compliance roadmap (Phase 0)
- [APPLICATION_SPECIFICATIONS.md](docs/APPLICATION_SPECIFICATIONS.md) - E-Commerce requirements

---

## Labels

- `compliance` `P0` `e-commerce` `legal` `backend` `frontend` `high-priority`

## Assigned To

- Backend Developer (VVVG, VAT, Invoicing)
- Frontend Developer (UI/UX)
- QA Engineer (Testing)
- Tech Lead (Architecture review)

## Due Date

Target completion: End of Phase 0 (Week 10)

---

## Notes

This issue implements **P0.6 E-Commerce Legal Compliance** - a critical compliance requirement for operating shops in the EU. Without this, stores cannot legally operate and face fines of €5,000-€300,000.

**Key regulations affected:**
- PAngV (Price Transparency) - Affects all B2C shops
- VVVG (14-Day Returns) - Mandatory for all B2C shops
- AStV (VAT Handling) - Affects B2B operations
- TMG (Legal Disclosure) - Impressum requirement
- GDPR (Privacy) - Data protection
