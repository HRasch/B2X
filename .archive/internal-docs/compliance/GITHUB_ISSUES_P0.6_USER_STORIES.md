# P0.6 User Stories - GitHub Issues Batch

**Created:** 28. Dezember 2025  
**Epic:** [#15 - P0.6: E-Commerce Legal Compliance (B2B & B2C)](../../../issues/15)  
**Total Issues:** 10  
**Total Effort:** 128 hours  
**Sprints:** Sprint 1-3

---

## How to Create These Issues

### Option 1: Using GitHub Web UI
1. Go to: https://github.com/HRasch/B2Connect/issues
2. Click "New Issue"
3. Copy each issue below (title + body)
4. Paste into the issue form
5. Add labels: `p0.6`, `backend`, `frontend` (as appropriate), plus sprint labels
6. Click "Create"

### Option 2: Using GitHub CLI
```bash
gh issue create --title "..." --body "..." --label "p0.6,backend,sprint-1"
```

### Option 3: Bulk Import (if you export to CSV)
Use GitHub's bulk import feature if available in your enterprise plan.

---

## 📋 Issue Templates

### Issue #P0.6-US-001: B2C Price Transparency

**Title:** 
```
P0.6-US-001: B2C Price Transparency (VAT, Shipping, Final Total)
```

**Body:**
```markdown
## Description
B2C customers must see transparent pricing at all points in the purchase journey.

## User Story
AS A B2C customer
I WANT TO see the final price (including VAT and shipping) at checkout
SO THAT I know exactly what I will be charged

## Acceptance Criteria
- [ ] Product detail page shows: Base Price + VAT Rate + VAT Amount + Final Price (brutto)
- [ ] Shipping selection shows cost per method (DHL, DPD, PostNL) with price (gross)
- [ ] Checkout: Cart shows itemized totals
  - [ ] Subtotal (net)
  - [ ] VAT by rate (7%, 19%)
  - [ ] Shipping Cost
  - [ ] Order Total (gross)
- [ ] Price display format: €XX,XX inkl. 19% MwSt (PAngV compliant)
- [ ] Tests: 6 unit tests (VAT calculation, shipping lookup, price formatting)

## Technical Details
**Backend Implementation:**
- PriceCalculationService: Calculate VAT based on country
- ShippingCostService: Lookup cost per carrier & destination
- EF Core for product pricing

**Frontend Implementation:**
- ProductDetail component: Display formatted prices
- CheckoutSummary component: Itemized breakdown

**Files to Create/Modify:**
- `backend/Domain/Catalog/src/Application/Handlers/PriceCalculationService.cs`
- `backend/Domain/Catalog/src/Application/Handlers/ShippingCostService.cs`
- `Frontend/Store/src/components/ProductDetail.vue`
- `Frontend/Store/src/components/CheckoutSummary.vue`

## Effort Estimate: 12 hours
**Sprint:** Sprint 1
**Owner:** Backend Dev 1
**Dependencies:** None

## Definition of Done
- [ ] Code complete
- [ ] All tests passing (6 unit tests)
- [ ] Code review approved
- [ ] Regulatory review (PAngV) approved
- [ ] Deployed to staging environment
- [ ] Manual testing on multiple currencies (EUR, CHF, GBP)

## Acceptance Criteria Test Cases
- Price calculation: Product 100€ + 19% VAT = 119€
- Shipping lookup: Germany → DHL = 4.99€
- Final total display: 119€ + 4.99€ = 123.99€
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-1`, `legal-compliance`

---

### Issue #P0.6-US-002: B2C Shipping Cost Display

**Title:**
```
P0.6-US-002: B2C Shipping Cost Display (Before Checkout)
```

**Body:**
```markdown
## Description
Per PAngV, shipping costs must be visible BEFORE checkout so customers can make informed decisions.

## User Story
AS A B2C customer
I WANT TO see shipping cost options BEFORE clicking 'Checkout'
SO THAT I can choose the best shipping method without surprises

## Acceptance Criteria
- [ ] Cart page: 'Shipping Method' selector with costs per method
- [ ] Shipping methods displayed:
  - [ ] DHL Express (€4.99)
  - [ ] DPD Standard (€3.99)
  - [ ] PostNL (€5.99)
- [ ] Cost updates dynamically based on destination country
- [ ] Total price updates in real-time when shipping method changes
- [ ] Mobile-responsive shipping selector
- [ ] Tests: 4 unit tests + 2 E2E tests

## Technical Details
**Backend:**
- ShippingCostService.GetCost(destination: string, method: ShippingMethod): decimal
- Shipping rates table: `shipping_rates` with (carrier, country, cost)
- Endpoint: `GET /api/cart/shipping-methods?country={country}`

**Frontend:**
- CartShipping component: Radio button group or dropdown
- RealTime total calculation: Listen to shipping method change
- Display format: "DHL Express: €4.99"

**Files to Create/Modify:**
- `backend/Domain/Catalog/src/Application/Handlers/ShippingService.cs`
- `backend/Domain/Catalog/src/Core/Entities/ShippingRate.cs`
- `Frontend/Store/src/components/CartShipping.vue`

## Effort Estimate: 8 hours
**Sprint:** Sprint 1
**Owner:** Backend Dev 1 (backend), Frontend Dev (UI)
**Dependencies:** P0.6-US-001 (uses PriceCalculationService)

## Definition of Done
- [ ] Shipping selector visible on cart page
- [ ] All 4 unit tests passing
- [ ] E2E tests for shipping selection + total update
- [ ] Deployed to staging
- [ ] Manual test: Select different methods, verify total updates

## Test Cases
1. GET /api/cart/shipping-methods?country=DE → Returns 3 methods
2. Select DHL → Total updates (+€4.99)
3. Select DPD → Total updates (+€3.99)
4. Change country to CH → Shipping options update
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-1`, `legal-compliance`

---

### Issue #P0.6-US-003: B2C 14-Day Withdrawal Period

**Title:**
```
P0.6-US-003: B2C 14-Day Withdrawal Period Enforcement (VVVG)
```

**Body:**
```markdown
## Description
German law (VVVG §355) grants B2C customers 14 days to withdraw from a purchase.

## User Story
AS A B2C customer
I WANT TO return items within 14 days of delivery
SO THAT I can withdraw from the purchase per German law

## Acceptance Criteria
- [ ] Order entity tracks:
  - [ ] Order date (CreatedAt)
  - [ ] Delivery date (when carrier marks delivered)
  - [ ] Withdrawal deadline (delivery + 14 days)
- [ ] Customer can request return ONLY within 14-day window
- [ ] Return request rejected if outside window (show error with deadline)
- [ ] Withdrawal form auto-generated with:
  - [ ] Order number
  - [ ] Return shipping address (carrier-specific)
  - [ ] Return deadline date (in customer's language)
  - [ ] Refund explanation (payment method, timeline)
  - [ ] Terms: restock fee (if any), shipping cost refund
- [ ] Return label generated via DHL API integration
- [ ] Refund processed within 14 days of withdrawal request approval
- [ ] Tests: 6 unit tests + 2 E2E tests

## Technical Details
**Backend:**
- ReturnManagementService with methods:
  - CanRequestReturn(orderId): bool
  - RequestReturn(orderId, reason): ReturnRequest
  - GenerateReturnLabel(returnId): ReturnLabel (DHL API)
  - ProcessRefund(returnId): RefundResult
- Order entity: `delivery_date`, `withdrawal_deadline`, `is_withdrawn`
- DHL API: https://developer.dhl.com/api-reference/returns-api
- Refund: Call Stripe/PayPal API

**Frontend:**
- ReturnRequestForm component
- ReturnStatus component (show deadline if still available)

**Files to Create/Modify:**
- `backend/Domain/Order/src/Core/Entities/Order.cs`
- `backend/Domain/Order/src/Application/Handlers/ReturnManagementService.cs`
- `backend/Domain/Order/src/Infrastructure/External/DhlReturnLabelClient.cs`
- `Frontend/Store/src/components/ReturnRequestForm.vue`
- `Frontend/Store/src/components/ReturnStatus.vue`

## Effort Estimate: 16 hours
**Sprint:** Sprint 2
**Owner:** Backend Dev 1, Frontend Dev
**Dependencies:** P0.6-US-001 (order data), P0.6-US-008 (invoice data for return form)

## Definition of Done
- [ ] All CRUD operations for returns working
- [ ] DHL API integration tested with sandbox
- [ ] Refund processing working (Stripe/PayPal)
- [ ] 6 unit tests passing
- [ ] 2 E2E tests (happy path + outside deadline)
- [ ] Legal review (VVVG compliance)
- [ ] Deployed to staging

## Test Cases
1. CreateReturnRequest within deadline → Success
2. CreateReturnRequest after deadline → Rejected with message
3. GenerateReturnLabel → DHL API called, label PDF returned
4. ProcessRefund → Payment gateway called, refund verified
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-2`, `legal-compliance`

---

### Issue #P0.6-US-004: Legal Documents Management

**Title:**
```
P0.6-US-004: Shop-Specific Legal Documents Management
```

**Body:**
```markdown
## Description
Each shop needs editable legal documents (Terms, Privacy, Impressum) with version control.

## User Story
AS A shop owner
I WANT TO manage my shop's legal documents (AGB, Datenschutz, Impressum)
SO THAT customers see my correct legal terms at all times

## Acceptance Criteria
- [ ] Admin Dashboard: Legal Documents section (separate from Settings)
- [ ] Document editors (separate B2C and B2B forms):
  - [ ] Terms & Conditions (B2C) - max 50KB
  - [ ] Terms & Conditions (B2B) - max 50KB
  - [ ] Privacy Statement (GDPR Art. 13/14) - max 50KB
  - [ ] Impressum (company info per TMG §5) - max 10KB
  - [ ] Withdrawal Notice (14-day VVVG text) - auto-filled, editable
  - [ ] General Terms (ODR-VO, dispute resolution) - max 20KB
- [ ] Version control:
  - [ ] Save full history with timestamp
  - [ ] Show "Updated by [User] on [Date]"
  - [ ] View previous versions (read-only)
  - [ ] Restore previous version (admin only)
- [ ] Preview button: Show how documents appear to customers
- [ ] Required field validation: Reject if incomplete
- [ ] Tests: 8 unit tests

## Technical Details
**Backend:**
- LegalDocumentsService with CRUD operations
- DocumentHistory entity: Audit trail
- Validation: Check all required fields present
- Endpoint: `POST/PUT /api/shops/{shopId}/legal-documents`

**Frontend:**
- LegalDocumentsEditor component (rich text editor)
- DocumentTabs (B2C, B2B, Common)
- VersionHistoryViewer: Timeline of changes
- PreviewModal: Show final render

**Files to Create/Modify:**
- `backend/Domain/Tenancy/src/Core/Entities/LegalDocument.cs`
- `backend/Domain/Tenancy/src/Core/Entities/DocumentHistory.cs`
- `backend/Domain/Tenancy/src/Application/Handlers/LegalDocumentsService.cs`
- `Frontend/Admin/src/components/LegalDocumentsEditor.vue`
- `Frontend/Admin/src/components/VersionHistoryViewer.vue`

## Effort Estimate: 20 hours
**Sprint:** Sprint 3
**Owner:** Backend Dev 2, Frontend Dev
**Dependencies:** None (can be done in parallel)

## Definition of Done
- [ ] Editor UI complete and tested
- [ ] All CRUD operations working
- [ ] Version history tracking immutable (audit log)
- [ ] Legal review of document templates (by compliance officer)
- [ ] Admin access control enforced (only shop admins can edit own shop)
- [ ] Deployed to staging

## Test Cases
1. Create legal documents → Version 1 saved with timestamp
2. Edit terms → Version 2 created, old version preserved
3. Restore version 1 → Content reverted, Version 3 created with note "Restored from version 1"
4. Validate missing field → Error message shown
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-3`, `legal-compliance`

---

### Issue #P0.6-US-005: Mandatory Terms Acceptance

**Title:**
```
P0.6-US-005: Mandatory Terms & Conditions Acceptance
```

**Body:**
```markdown
## Description
Per PAngV/VVVG, customers must actively accept Terms before order completion.

## User Story
AS A B2C customer
I WANT TO review and accept Terms before purchase
SO THAT I acknowledge my rights and responsibilities per German law

## Acceptance Criteria
- [ ] Checkout flow: Step 4 "Agree to Terms" (after shipping, before payment)
- [ ] Required checkboxes (all must be checked):
  - [ ] "I accept the Terms & Conditions"
  - [ ] "I acknowledge the 14-day withdrawal right and return conditions"
  - [ ] "I accept the Privacy Policy"
- [ ] Each checkbox has clickable link to full document (opens modal/new tab)
- [ ] "Continue to Payment" button disabled until all checked
- [ ] Log acceptance: 
  - [ ] Timestamp
  - [ ] Customer ID
  - [ ] IP Address
  - [ ] Document version hashes (GDPR: prove which version accepted)
- [ ] Order record: Store `accepted_terms_version`, `accepted_at`, `accepted_from_ip`
- [ ] Tests: 4 E2E tests

## Technical Details
**Backend:**
- TermsAcceptanceLog entity
- Validation middleware: Check all checkboxes true
- Endpoint: `POST /api/orders/accept-terms` (returns token for payment)
- Store document version SHA256 hash

**Frontend:**
- CheckoutTermsStep component
- TermsCheckboxGroup component
- DocumentModal component (show on link click)

**Files to Create/Modify:**
- `backend/Domain/Order/src/Core/Entities/TermsAcceptanceLog.cs`
- `backend/Domain/Order/src/Application/Handlers/CheckoutService.cs`
- `Frontend/Store/src/components/CheckoutTermsStep.vue`
- `Frontend/Store/src/components/TermsCheckboxGroup.vue`

## Effort Estimate: 8 hours
**Sprint:** Sprint 2
**Owner:** Frontend Dev (primarily), Backend Dev
**Dependencies:** P0.6-US-004 (legal documents must exist)

## Definition of Done
- [ ] Checkout workflow includes terms step
- [ ] All 3 checkboxes required and working
- [ ] Document modals display correctly
- [ ] Logging working (acceptance recorded in DB)
- [ ] Continue button disabled/enabled correctly
- [ ] 4 E2E tests passing
- [ ] Deployed to staging

## Test Cases
1. Try to continue without any checkboxes checked → Button disabled
2. Check 2/3 boxes → Button still disabled
3. Check all 3 boxes → Button enabled
4. Click document link → Modal shows correct document
```

**Labels:** `p0.6`, `frontend`, `sprint-2`, `legal-compliance`

---

### Issue #P0.6-US-006: B2B VAT-ID Validation

**Title:**
```
P0.6-US-006: B2B VAT-ID Validation (VIES API)
```

**Body:**
```markdown
## Description
B2B customers must provide valid EU VAT-IDs. We validate via VIES (EU VAT lookup service).

## User Story
AS A B2B shop owner
I WANT TO provide my VAT-ID and have it validated
SO THAT I can benefit from reverse charge (no VAT on invoice)

## Acceptance Criteria
- [ ] B2B Checkout (separate from B2C): VAT-ID input field
- [ ] Format validation: ^[A-Z]{2}[0-9A-Z]{2,}$ (2 letter country + alphanumeric)
- [ ] Example: DE123456789, AT123456789
- [ ] VIES API call: https://ec.europa.eu/taxation_customs/vies/
  - [ ] API call on blur / async validation
  - [ ] Show spinner while validating
  - [ ] Success: Display ✓ + company name + address from VIES
  - [ ] Failure (invalid): Show error message, allow to continue as B2C (with VAT)
  - [ ] Failure (API down): Show warning, allow to proceed
  - [ ] Caching: Valid VAT-IDs cached 365 days (for performance)
- [ ] Tests: 4 unit tests

## Technical Details
**Backend:**
- VatIdValidationService
- VIES API client (with retry + timeout)
  - Timeout: 10s
  - Retry: 3 attempts on timeout
  - Rate limit: Max 1 request per VAT-ID per day (cache)
- VatIdValidationCache entity: (vatId, country, companyName, address, validatedAt)
- Fallback: If API down, proceed with warning

**Frontend:**
- VatIdInput component: Async validation on blur
- ValidationMessage component (success ✓, error, warning ⚠️)

**Files to Create/Modify:**
- `backend/Domain/Order/src/Application/Handlers/VatIdValidationService.cs`
- `backend/Domain/Order/src/Infrastructure/External/ViesApiClient.cs`
- `Backend/Domain/Order/src/Core/Entities/VatIdValidationCache.cs`
- `Frontend/Store/src/components/VatIdInput.vue`

## Effort Estimate: 12 hours
**Sprint:** Sprint 2
**Owner:** Backend Dev 2, Frontend Dev
**Dependencies:** None

## Definition of Done
- [ ] VIES API integration tested with sandbox (use test VAT-IDs)
- [ ] Format validation working
- [ ] Caching working (365-day TTL)
- [ ] Fallback logic tested (API down scenario)
- [ ] 4 unit tests passing
- [ ] Frontend spinner + messages working
- [ ] Deployed to staging

## Test Cases
1. Enter valid VAT-ID (DE123456789) → VIES lookup succeeds, shows company name
2. Enter invalid VAT-ID (INVALID) → VIES lookup fails, shows error
3. Second request for same VAT-ID → Cached, no API call
4. API timeout → Shows warning, allows to continue
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-2`, `legal-compliance`

---

### Issue #P0.6-US-007: B2B Reverse Charge Logic

**Title:**
```
P0.6-US-007: B2B Reverse Charge Logic (No VAT for EU Businesses)
```

**Body:**
```markdown
## Description
When B2B buyer has valid VAT-ID, seller doesn't charge VAT (reverse charge per AStV).

## User Story
AS A B2Connect operator
I WANT TO apply reverse charge for intra-EU B2B sales
SO THAT VAT is handled correctly per EU tax law (AStV §3)

## Acceptance Criteria
- [ ] Reverse charge applied automatically when ALL conditions met:
  - [ ] Customer is B2B (has valid VAT-ID)
  - [ ] VAT-ID is VIES-validated (from P0.6-US-006)
  - [ ] Buyer country ≠ Seller country (intra-EU)
  - [ ] Both countries are EU members
- [ ] Invoice shows:
  - [ ] "0% VAT - Reverse Charge per AStV Art. 199a"
  - [ ] Customer's VAT-ID displayed
  - [ ] Buyer's country code
- [ ] Tax reporting:
  - [ ] B2B sales with reverse charge logged separately
  - [ ] VAT report: 0% VAT line item
- [ ] Price calculation:
  - [ ] No VAT added (final price = product price + shipping)
  - [ ] Example: 100€ product + 5€ shipping = 105€ (NOT 125€ with VAT)
- [ ] Tests: 3 unit tests

## Technical Details
**Backend:**
- ReverseChargeService.ShouldApplyReverseCharge(order: Order): bool
  - Check: order.Customer.VatIdValidated
  - Check: order.BillingCountry != order.ShippingCountry
  - Check: IsEuCountry(buyerCountry) && IsEuCountry(sellerCountry)
- PriceCalculationService: If reverse charge → VAT% = 0
- Invoice generation: Print reverse charge explanation
- Tax report: Separate line for reverse charge sales

**Files to Create/Modify:**
- `backend/Domain/Order/src/Application/Handlers/ReverseChargeService.cs`
- `backend/Domain/Catalog/src/Application/Handlers/PriceCalculationService.cs` (update)
- `backend/Domain/Order/src/Application/Handlers/InvoiceGenerationService.cs` (update)

## Effort Estimate: 10 hours
**Sprint:** Sprint 3
**Owner:** Backend Dev 2
**Dependencies:** P0.6-US-006 (VAT-ID validation), P0.6-US-001 (price calculation), P0.6-US-008 (invoice)

## Definition of Done
- [ ] Logic implemented
- [ ] Tested with multiple country combinations (DE→AT, AT→DE, DE→IT, etc.)
- [ ] Reverse charge flag set on order entity
- [ ] Invoice correctly reflects reverse charge
- [ ] Tax accountant review (compliance sign-off)
- [ ] 3 unit tests passing
- [ ] Deployed to staging

## Test Cases
1. B2B order: Buyer DE with VAT-ID, Seller AT
   - Should apply reverse charge
   - Price: 100€ (no VAT)
   - Invoice: "0% VAT - Reverse Charge"

2. B2B order: Buyer UK (post-Brexit), Seller DE
   - Should NOT apply reverse charge (UK not in EU)
   - Price: 100€ + 19€ VAT = 119€

3. B2C order: Buyer has VAT-ID but not B2B
   - Should NOT apply reverse charge (B2C customer)
   - Price: 100€ + 19€ VAT = 119€
```

**Labels:** `p0.6`, `backend`, `sprint-3`, `legal-compliance`

---

### Issue #P0.6-US-008: Invoice Generation & Encryption

**Title:**
```
P0.6-US-008: Invoice Generation & Customer Download (PDF)
```

**Body:**
```markdown
## Description
Generate PDF invoices, encrypt, store, and allow customer download.

## User Story
AS A customer
I WANT TO download my invoice as PDF
SO THAT I have proof of purchase for accounting/returns

## Acceptance Criteria
- [ ] Order confirmation email: Includes PDF invoice as attachment
- [ ] Customer dashboard: Download button for each order → Opens/downloads PDF
- [ ] Invoice content (all required):
  - [ ] Invoice header: "RECHNUNG" (B2C) / "RECHNUNG" (B2B)
  - [ ] Invoice number: Unique per shop (Format: SHOP-2025-000001)
  - [ ] Order date: "Bestelldatum: [Date]"
  - [ ] Delivery date: "Lieferdatum: [Date]" (if delivered)
  - [ ] Customer info:
    - [ ] Name (encrypted in DB, decrypted for PDF)
    - [ ] Address (encrypted in DB, decrypted for PDF)
    - [ ] Email address (masked in PDF for privacy)
  - [ ] Line items table:
    - [ ] Product name
    - [ ] SKU
    - [ ] Quantity
    - [ ] Unit price (net)
    - [ ] VAT rate (7% or 19%)
    - [ ] Total (gross per item)
  - [ ] Totals section:
    - [ ] Subtotal (net)
    - [ ] VAT breakdown (7% line, 19% line, if both apply)
    - [ ] Shipping cost
    - [ ] Order total (gross)
  - [ ] Payment method: "Zahlungsart: [Kreditkarte / PayPal / etc.]"
  - [ ] Payment status: "Bezahlt" or "Ausstehend"
  - [ ] Seller info: Shop address (from shop settings)
  - [ ] B2C only: "Widerrufsrecht: 14 Tage ab Lieferdatum..."
  - [ ] B2B only: "Reverse Charge Applied" (if applicable)
  - [ ] Footer: Shop contact email, phone
- [ ] PDF generation performance: < 2 seconds
- [ ] Storage:
  - [ ] Location: S3/Azure Blob (encrypted AES-256)
  - [ ] Immutable: Once created, cannot be modified
  - [ ] Retention: 10 years per German law
  - [ ] Naming: `invoices/{tenantId}/{year}/{invoiceNumber}.pdf`
- [ ] Tests: 3 unit tests + 2 E2E tests

## Technical Details
**Backend:**
- InvoiceGenerationService (iText library for PDF generation)
  - Method: GenerateAsync(order: Order): byte[]
  - Performance target: < 2s
- EncryptionService: AES-256 encrypt PDF before storage
- InvoiceStorageService:
  - Method: SaveAsync(invoiceNumber, content): Task
  - Backend: Azure Blob Immutable Storage / S3 Object Lock
- EmailService: Send PDF attachment on order confirmation
- Endpoints:
  - `GET /api/orders/{orderId}/invoice` → Download PDF
  - `POST /api/orders/{orderId}/invoice/email` → Re-send email

**Frontend:**
- OrderDetail component: "Download Invoice" button
- Invoice download triggers browser download

**Files to Create/Modify:**
- `backend/Domain/Order/src/Application/Handlers/InvoiceGenerationService.cs`
- `backend/Domain/Order/src/Application/Handlers/InvoiceStorageService.cs`
- `backend/Domain/Order/src/Infrastructure/External/PdfGenerationClient.cs` (iText)
- `Frontend/Store/src/components/OrderDetail.vue`

## Effort Estimate: 18 hours
**Sprint:** Sprint 3
**Owner:** Backend Dev 1, Frontend Dev
**Dependencies:** P0.6-US-001 (pricing), P0.6-US-004 (legal text for footer), P0.6-US-003 (delivery date)

## Definition of Done
- [ ] PDF generation working with all fields
- [ ] PDF encryption working (AES-256)
- [ ] Storage integrated (S3 / Azure Blob)
- [ ] Email delivery working with PDF attachment
- [ ] Download link working (browser download)
- [ ] Performance: 2s generation time verified
- [ ] 3 unit tests passing
- [ ] 2 E2E tests (create order → generate invoice → download)
- [ ] Deployed to staging
- [ ] Manual testing: Generate PDF, verify content, decrypt file

## Test Cases
1. Create B2C order → Invoice generated → Contains all fields
2. Create B2B order with reverse charge → Invoice shows "0% VAT - Reverse Charge"
3. Download invoice → PDF opens in browser
4. Generate same invoice twice → Same file overwritten (idempotent)
5. Email with attachment → Customer receives PDF invoice
```

**Labels:** `p0.6`, `backend`, `frontend`, `sprint-3`, `legal-compliance`

---

### Issue #P0.6-US-009: Invoice Archival & 10-Year Retention

**Title:**
```
P0.6-US-009: Invoice Archival & 10-Year Retention Policy
```

**Body:**
```markdown
## Description
German tax law requires 10-year invoice retention. Implement automated archival with immutability.

## User Story
AS A shop owner
I WANT invoices automatically archived for 10 years
SO THAT I comply with German tax law (§257 HGB, §6 AStV)

## Acceptance Criteria
- [ ] Invoice storage strategy:
  - [ ] Hot storage (S3 Standard / Azure Hot): Current year + 1 year (most frequent access)
  - [ ] Warm storage (S3 Intelligent-Tiering / Azure Cool): Years 2-5 (occasional access)
  - [ ] Cold storage (S3 Glacier / Azure Archive): Years 6-10 (rare access, cheaper)
- [ ] Immutability enforced:
  - [ ] S3 Object Lock: Retention mode (can't delete before 10 years)
  - [ ] Azure Blob: Immutable Storage + Legal Hold
  - [ ] Once written, cannot be modified/deleted (legal requirement)
- [ ] Archive job:
  - [ ] Runs monthly (1st of month)
  - [ ] Moves invoices older than 1 year to warm tier
  - [ ] Moves invoices older than 5 years to cold tier
  - [ ] Marks older than 10 years for deletion
- [ ] Retrieval:
  - [ ] Still accessible for 10 years (for customer download, audits)
  - [ ] Warm/cold tier retrieval takes 1-5 min (acceptable)
- [ ] Tests: 2 unit tests + 1 integration test

## Technical Details
**Backend:**
- InvoiceArchivalService
  - Method: ArchiveOldInvoicesAsync(): Task
  - Runs as scheduled job (IHostedService)
  - Move files between tiers based on age
- Monitoring: Log all archive operations (audit trail)
- Access logging: Track all customer downloads from archive

**Infrastructure:**
- S3 Bucket:
  ```
  - Default policy: Standard (hot)
  - Lifecycle rule: Move to Intelligent-Tiering after 365 days
  - Object Lock: Retention period 10 years
  ```
- Azure Blob:
  ```
  - Storage account: Hot/Cool/Archive tiers
  - Lifecycle policy: Hot → Cool (1yr), Cool → Archive (5yr)
  - Immutable storage: 10-year minimum hold
  ```

**Files to Create/Modify:**
- `backend/Domain/Order/src/Application/Handlers/InvoiceArchivalService.cs`
- `backend/Infrastructure/Jobs/ArchiveInvoicesJob.cs`
- Infrastructure as Code: Terraform / CloudFormation for S3/Azure config

## Effort Estimate: 6 hours
**Sprint:** Sprint 3
**Owner:** DevOps / Backend Dev 2
**Dependencies:** P0.6-US-008 (invoice generation)

## Definition of Done
- [ ] Archival job implemented and tested
- [ ] Lifecycle policies configured (S3/Azure)
- [ ] Object Lock/Immutable storage enabled
- [ ] Retrieval tested (download from archive tier)
- [ ] Cost optimization verified (S3 Glacier ~10% of Standard cost)
- [ ] 2 unit tests passing
- [ ] 1 integration test (move files, verify retrieval)
- [ ] Deployed to staging
- [ ] 10-year retention verified in infrastructure

## Test Cases
1. Archive job: Move invoices >365 days to warm tier
   - Old invoice in S3 Intelligent-Tiering
   - New invoice still in Standard tier

2. Retrieve old invoice: Download from warm/cold tier
   - Takes 1-5 min (S3 restore)
   - PDF decrypted correctly

3. Deletion protection: Try to delete invoice < 10 years
   - S3 Object Lock returns error (retention not expired)
   - Legal compliance verified
```

**Labels:** `p0.6`, `backend`, `sprint-3`, `legal-compliance`

---

### Issue #P0.6-US-010: Admin Dashboard - Legal Documents Overview

**Title:**
```
P0.6-US-010: Admin Dashboard - Legal Documents Overview
```

**Body:**
```markdown
## Description
Admin dashboard showing all shops' legal documents, compliance status, and warnings.

## User Story
AS A B2Connect compliance officer
I WANT TO see a dashboard of all shops' legal documents
SO THAT I can monitor compliance across all shops

## Acceptance Criteria
- [ ] Admin dashboard section: "Legal Compliance" (new tab)
- [ ] Table showing all shops with:
  - [ ] Shop name (clickable → go to edit page)
  - [ ] Country (flag icon)
  - [ ] Last updated date for each document:
    - [ ] Terms (B2C)
    - [ ] Terms (B2B)
    - [ ] Privacy Statement
    - [ ] Impressum
    - [ ] Withdrawal Notice
  - [ ] Completion percentage (5/5 documents = 100%)
  - [ ] Status badge:
    - [ ] ✓ Green: All documents up-to-date
    - [ ] ⚠️ Yellow: Missing or outdated (> 1 year)
    - [ ] 🔴 Red: Critical gaps (< 1 month no update)
  - [ ] Actions:
    - [ ] View (icon to see all documents)
    - [ ] Edit (icon to edit)
    - [ ] Request Update (button to email shop owner request)
- [ ] Filters/Search:
  - [ ] By country (dropdown)
  - [ ] By completeness (% filter)
  - [ ] By last updated (date range)
  - [ ] By status (All / Compliant / Warning / Critical)
- [ ] Sorting: By shop name, country, last updated, status
- [ ] Bulk actions:
  - [ ] Select multiple shops → "Request Updates from Selected"
- [ ] Tests: 3 E2E tests

## Technical Details
**Backend:**
- Endpoint: `GET /api/admin/legal-compliance` → Returns:
  ```json
  {
    "shops": [
      {
        "id": "uuid",
        "name": "Shop Name",
        "country": "DE",
        "documents": {
          "terms_b2c": { "exists": true, "lastUpdated": "2025-12-28", "days_old": 5 },
          "terms_b2b": { "exists": false, "lastUpdated": null, "days_old": null },
          "privacy": { "exists": true, "lastUpdated": "2025-01-15", "days_old": 348 },
          ...
        },
        "completeness_percent": 80,
        "status": "warning"
      }
    ]
  }
  ```
- Endpoint: `POST /api/admin/shops/{shopId}/request-legal-update` → Send email to shop owner

**Frontend:**
- ComplianceDashboard component: Main page
- ComplianceTable component: Sortable, filterable table
- FilterBar component: Country, completeness, date range, status filters
- StatusBadge component: Colored badge with icon
- BulkActions component: Select multiple shops

**Files to Create/Modify:**
- `backend/Domain/Tenancy/src/Application/Handlers/LegalComplianceService.cs` (new)
- `Frontend/Admin/src/components/ComplianceDashboard.vue` (new)
- `Frontend/Admin/src/components/ComplianceTable.vue` (new)
- `Frontend/Admin/src/components/FilterBar.vue` (new)

## Effort Estimate: 8 hours
**Sprint:** Sprint 3 (⚠️ Can be deferred to Phase 1 if capacity needed)
**Owner:** Frontend Dev, Backend Dev 2
**Dependencies:** P0.6-US-004 (legal documents CRUD)

## Definition of Done
- [ ] Dashboard displays all shops with compliance data
- [ ] Filtering working (country, completeness, date range)
- [ ] Sorting working
- [ ] Status badges displaying correctly
- [ ] Edit actions functional
- [ ] Request update email working
- [ ] 3 E2E tests passing:
  - [ ] Load dashboard → All shops displayed
  - [ ] Filter by country → Correct shops shown
  - [ ] Request update → Email sent
- [ ] Deployed to staging

## Test Cases
1. Load compliance dashboard:
   - All shops displayed with last update dates
   - Completeness % calculated
   - Status badge color correct

2. Filter by country "DE":
   - Only German shops shown
   - Row count matches expectation

3. Request updates from multiple shops:
   - Select 3 shops
   - Click "Request Updates"
   - Email sent to all 3 shop admins
```

**Labels:** `p0.6`, `frontend`, `sprint-3`, `legal-compliance`, `optional`

---

## 📊 Summary Statistics

| Metric | Value |
|--------|-------|
| Total Issues | 10 |
| Total Effort | 128 hours |
| Backend Hours | 77 hours |
| Frontend Hours | 35 hours |
| Backend Developers | 2 FTE |
| Frontend Developers | 1 FTE |
| Sprints | 3 (Sprint 1: 20h, Sprint 2: 36h, Sprint 3: 72h) |
| Go/No-Go Gate | All P0.6 issues completed |

---

## Next Steps

1. **Create Issues in GitHub:**
   - Go to https://github.com/HRasch/B2Connect/issues
   - Create one issue per user story using templates above
   - Add labels: `p0.6`, `backend`, `frontend`, `legal-compliance`, plus sprint labels

2. **Hold Backlog Refinement Meeting:**
   - Review all 10 issues with team
   - Validate effort estimates
   - Assign owners
   - Confirm Sprint 1 commitment (P0.6-US-001, P0.6-US-002, P0.6-US-005)

3. **Link to Epic:**
   - Link all issues to Epic #15 (P0.6: E-Commerce Legal Compliance)

4. **Create Milestones:**
   - Milestone "P0.6: E-Commerce" (due date: 30. November 2025)

---

**Document Owner:** Product Team  
**Last Updated:** 28. Dezember 2025  
**Status:** Ready for GitHub Issue Import

