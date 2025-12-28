#!/bin/bash

# B2Connect Backlog Issue Creator - P0.6 Store Legal Compliance
# This script creates all 10 GitHub issues from the backlog refinement

set -e

echo "🚀 Creating P0.6 Backlog Issues..."

# Issue 1: B2C Price Transparency
gh issue create \
  --title "P0.6-US-001: B2C Price Transparency (PAngV Compliance)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 12 hours | **Sprint:** Sprint 1

## User Story
As a shop owner, I want customers to always see final prices including VAT, so that I comply with PAngV and customers know actual cost before checkout.

## Acceptance Criteria
- Product listing displays price as '€99,99 inkl. MwSt'
- Product detail page shows VAT breakdown
- Cart displays 'Subtotal + VAT = Total'
- Checkout confirms final price before payment
- Invoice shows VAT breakdown per line item
- All prices dynamically update when VAT rate changes
- Mobile responsive (tested at 320px+)

## Technical Details
**Backend Service:** Catalog | **Components:** ProductCard, ProductDetail, Cart, Checkout

## Subtasks
- [ ] Create PriceCalculationService (backend)
- [ ] Implement VAT rate lookup per country/product
- [ ] Update ProductCard component to show VAT
- [ ] Update Cart component to show breakdown
- [ ] Update Checkout to confirm final price
- [ ] Create E2E tests (Playwright)
- [ ] Update documentation

## Dependencies
- VAT master data must be configured in system" \
  --label "P0.6,legal,pricing,backend,sprint-1"

echo "✅ Issue 1 created"

# Issue 2: B2B VAT-ID Validation
gh issue create \
  --title "P0.6-US-002: B2B VAT-ID Validation (Reverse Charge)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 20 hours | **Sprint:** Sprint 1

## User Story
As a B2B customer, I want to enter my VAT-ID during checkout and have it validated, so that the seller can apply reverse charge and not charge me VAT.

## Acceptance Criteria
- Checkout form displays VAT-ID field (optional for B2C, required for B2B)
- Real-time validation via VIES API (EU VAT registry)
- Clear error message if VAT-ID invalid
- Reverse charge automatically applied if valid
- Validation result logged in audit trail
- Fallback to standard VAT if VIES unavailable
- B2B order remembers validated VAT-ID for future orders

## Technical Details
**Backend Service:** Catalog / Order | **External API:** VIES (EU VAT registry)

## Subtasks
- [ ] Create VatIdValidationService (VIES API integration)
- [ ] Add VAT-ID field to Checkout component
- [ ] Implement real-time validation (debounced)
- [ ] Add reverse charge logic to PriceCalculationService
- [ ] Create error messages (i18n: DE, EN)
- [ ] Unit tests (VIES mock)
- [ ] Integration tests (with VIES)
- [ ] Audit logging for validations

## Dependencies
- P0.6-US-001 (Price Transparency) must be complete
- VIES API credentials configured

## References
- [VIES SOAP Service](https://ec.europa.eu/taxation_customs/vies/)" \
  --label "P0.6,legal,b2b,vat,backend,sprint-1"

echo "✅ Issue 2 created"

# Issue 3: Withdrawal Right Implementation
gh issue create \
  --title "P0.6-US-003: 14-Day Withdrawal Right (VVVG Compliance)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 18 hours | **Sprint:** Sprint 2

## User Story
As a B2C customer, I want to return products within 14 days of delivery and get a full refund, so that I exercise my consumer rights under VVVG.

## Acceptance Criteria
- Customer can request withdrawal within 14 days of delivery date
- System automatically generates return label (shipping carrier integration)
- Return shipping label emailed to customer
- Refund processed automatically upon receiving returned item
- Withdrawal request tracks: request date, delivery date, expiry date
- After 14 days, withdrawal button disabled with clear message
- B2B orders can refuse withdrawals if agreed in terms
- Return tracking integrated into order status

## Technical Details
**Backend Service:** Order | **External APIs:** Shipping carrier (DHL, DPD, etc.)

## Subtasks
- [ ] Create WithdrawalManagementService
- [ ] Implement 14-day countdown logic
- [ ] Integrate with shipping carrier API
- [ ] Add return label generation
- [ ] Create refund processing logic
- [ ] Add withdrawal request form to frontend
- [ ] Add return tracking to order dashboard
- [ ] Create email notifications
- [ ] Unit tests for date calculations
- [ ] Integration tests with mock carrier API

## Dependencies
- Order system must be deployed (F1.3)
- Shipping carrier accounts configured

## Testing Scenarios
- Withdrawal within 14 days: Accept ✅
- Withdrawal after 14 days: Reject ❌
- Return label generation works
- Refund processes correctly" \
  --label "P0.6,legal,b2c,returns,backend,sprint-2"

echo "✅ Issue 3 created"

# Issue 4: Order Confirmation Email
gh issue create \
  --title "P0.6-US-004: Order Confirmation & Terms Email (TMG Compliance)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 10 hours | **Sprint:** Sprint 2

## User Story
As a customer, I want to receive an order confirmation email immediately after purchase that includes the terms I accepted, so that I have proof of the transaction.

## Acceptance Criteria
- Order confirmation email sent within 5 seconds of order creation
- Email includes: order number, date, items, price, VAT breakdown
- Email includes full terms & conditions (accepted version with date/version)
- Email includes privacy policy notice
- Email includes withdrawal right notice (14 days for B2C)
- Email includes contact information for customer support
- Email is i18n (DE, EN minimum)
- Resend capability if customer requests
- Email format: Plain text + HTML with company branding

## Technical Details
**Backend Service:** Order | **Template Engine:** Liquid or Razor

## Subtasks
- [ ] Create OrderConfirmationEmailService
- [ ] Design email template (HTML + plain text)
- [ ] Implement i18n for email content
- [ ] Add resend functionality
- [ ] Create unit tests (mock SMTP)
- [ ] Integration tests (Ethereal email testing)
- [ ] Add to order creation workflow
- [ ] Document email configuration

## Dependencies
- Order system must be deployed
- Email service configured (SMTP credentials)

## Testing Checklist
- Email sent within 5 seconds
- All required fields present
- Proper branding
- Links work correctly" \
  --label "P0.6,legal,email,backend,sprint-2"

echo "✅ Issue 4 created"

# Issue 5: Invoice Management & Archival
gh issue create \
  --title "P0.6-US-005: Invoice Generation & 10-Year Archival" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 25 hours | **Sprint:** Sprint 2-3

## User Story
As a shop owner, I want invoices to be generated automatically and archived for 10 years in encrypted storage, so that I comply with German tax law and can provide audit trails.

## Acceptance Criteria
- Invoice generated automatically when order confirmed
- Invoice number unique per shop (never reused)
- Invoice saved as PDF with digital signature option
- PDF encrypted with AES-256 at rest
- Invoice archived for 10 years minimum
- Auto-delete after 10 years (compliance)
- Archive immutable (no modifications possible)
- Customer can download invoice from order page
- Admin can search/retrieve invoices
- Bulk invoice export for accounting systems

## Technical Details
**Backend Service:** Order | **Storage:** S3/Azure Blob | **PDF Library:** iText or SelectPdf

## Subtasks
- [ ] Create InvoiceGenerationService
- [ ] Generate unique invoice numbers per shop
- [ ] Create PDF invoice template (HTML to PDF)
- [ ] Implement encryption service (AES-256)
- [ ] Set up encrypted storage (S3/Azure)
- [ ] Create invoice retrieval service
- [ ] Add invoice download to order page
- [ ] Create admin invoice search
- [ ] Implement 10-year archive policy
- [ ] Create unit tests
- [ ] Create integration tests

## Dependencies
- Order system deployed
- Cloud storage configured (S3/Azure Blob)
- PDF generation library configured

## Compliance Notes
- German law requires 6 years (GDPR), but 10 years is safer for tax purposes
- Once archived, invoices must not be modifiable" \
  --label "P0.6,legal,invoices,backend,sprint-2,sprint-3"

echo "✅ Issue 5 created"

# Issue 6: Checkout Terms Acceptance
gh issue create \
  --title "P0.6-US-006: AGB & Datenschutz Acceptance (Legal Gate)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 12 hours | **Sprint:** Sprint 3

## User Story
As a shop owner, I want customers to explicitly accept the shop's Terms & Conditions and Privacy Policy before placing an order, so that I have documented consent for the transaction.

## Acceptance Criteria
- Checkout displays mandatory checkboxes:
  - [ ] I accept the Terms & Conditions (link to shop-specific T&Cs)
  - [ ] I have read the Privacy Policy (link to shop privacy policy)
  - [ ] For B2C: I understand my 14-day withdrawal right
- Checkboxes must be checked to proceed
- Checkbox state logged with order (version, timestamp)
- If shop updates T&Cs, new version required on next order
- Customer can review T&Cs in modal before accepting
- Mobile responsive checkbox styling
- Audit trail: who accepted what version when

## Technical Details
**Frontend:** Vue.js Checkout component | **Backend:** Order service

## Subtasks
- [ ] Create LegalDocumentVersion entity
- [ ] Add checkboxes to Checkout component
- [ ] Implement validation (must be checked)
- [ ] Add T&Cs modal viewer
- [ ] Create audit logging for acceptance
- [ ] Update order entity to store accepted versions
- [ ] Add version management to shop settings
- [ ] Create E2E tests (Playwright)
- [ ] Create unit tests

## Dependencies
- Checkout system deployed (F1.3)
- Shop settings to manage T&Cs URLs

## Audit Requirements
- Store: who accepted (user ID), what (T&C version), when (timestamp)
- Show in order details for customer reference" \
  --label "P0.6,legal,checkout,frontend,audit,sprint-3"

echo "✅ Issue 6 created"

# Issue 7: Impressum & Datenschutz Footer
gh issue create \
  --title "P0.6-US-007: Impressum & Privacy Links (TMG Compliance)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 8 hours | **Sprint:** Sprint 3

## User Story
As a shop owner, I want Impressum (company information) and Privacy Policy links in the footer of my store, so that I comply with German Telemediengesetz (TMG).

## Acceptance Criteria
- Footer displays Impressum link (legal company information)
- Footer displays Datenschutz/Privacy Policy link
- Links open in new tab or modal
- Impressum page includes:
  - Shop name and legal form
  - Owner/responsible person name
  - Business address
  - Contact email
  - Business registration number (if applicable)
  - Tax ID
- Impressum editable per shop in settings
- Privacy policy maintained separately (can link to external)
- Mobile responsive footer layout
- Links visible on all pages (no exceptions)

## Technical Details
**Frontend:** Footer component (Vue.js) | **Backend:** Shop settings service

## Subtasks
- [ ] Create footer component with links
- [ ] Add Impressum editor to shop settings
- [ ] Create Impressum display page
- [ ] Link privacy policy (internal or external URL)
- [ ] Add to all page layouts
- [ ] Mobile responsive testing
- [ ] i18n for footer text (DE, EN)
- [ ] E2E tests to verify links

## Dependencies
- Shop settings system deployed

## Legal Requirements (TMG §5)
- Impressum MUST be immediately accessible
- Must contain all required information
- Non-compliance can result in court orders" \
  --label "P0.6,legal,legal-pages,frontend,sprint-3"

echo "✅ Issue 7 created"

# Issue 8: B2B Payment Terms Configuration
gh issue create \
  --title "P0.6-US-008: B2B Payment Terms (Net 30/60/90)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 15 hours | **Sprint:** Sprint 3-4

## User Story
As a B2B shop owner, I want to configure flexible payment terms for business customers (Net 30, 60, 90 days), so that I can negotiate different terms with different customers.

## Acceptance Criteria
- Shop admin can configure default payment terms
- Admin can set different terms per customer/order
- Order displays payment due date (order date + net days)
- Invoice shows payment due date clearly
- Payment terms appear in order confirmation email
- B2C orders ignore payment terms (pay at checkout)
- B2B orders show terms at checkout
- Payment reminders sent at: due date - 3 days, due date, due date + 7 days
- Terms editable per shop in settings

## Technical Details
**Backend Service:** Order/Invoice | **Frontend:** Admin settings

## Subtasks
- [ ] Add PaymentTerms enum (NET_30, NET_60, NET_90, custom)
- [ ] Add payment_terms field to Order
- [ ] Add terms configuration to shop settings
- [ ] Implement due date calculation
- [ ] Add payment reminder job (scheduled task)
- [ ] Update order/invoice display
- [ ] Update checkout to show terms
- [ ] Create email reminder templates
- [ ] Unit tests for date calculations
- [ ] Integration tests

## Dependencies
- Order system deployed
- Invoice system deployed (P0.6-US-005)

## Testing Scenarios
- Net 30: Due date = order date + 30 days ✓
- Net 60: Due date = order date + 60 days ✓
- Reminders sent at correct intervals ✓" \
  --label "P0.6,legal,b2b,payments,backend,sprint-3,sprint-4"

echo "✅ Issue 8 created"

# Issue 9: Shipping Cost Transparency
gh issue create \
  --title "P0.6-US-009: Shipping Cost Display Before Checkout" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 14 hours | **Sprint:** Sprint 4

## User Story
As a customer, I want to see shipping costs clearly displayed before entering checkout, so that I know the total cost before committing to the purchase (PAngV requirement).

## Acceptance Criteria
- Cart page shows estimated shipping cost
- Shipping cost updated when address changed
- Shipping cost breakdown shown (per weight/destination)
- Free shipping threshold indicated (if applicable)
- Final price displayed prominently
- Mobile responsive display
- Shipping method selector (Standard, Express, etc.)
- Cost per method displayed
- No hidden fees (all costs must be transparent)
- Country-specific shipping rules applied

## Technical Details
**Backend Service:** Catalog/Order | **Frontend:** Cart component

## Subtasks
- [ ] Create ShippingCalculationService
- [ ] Implement shipping costs per country/weight
- [ ] Add shipping method selector to cart
- [ ] Real-time shipping cost updates
- [ ] Display in cart component
- [ ] Display in checkout summary
- [ ] Add shipping threshold logic
- [ ] Mobile responsive testing
- [ ] Unit tests
- [ ] E2E tests

## Dependencies
- Cart system deployed (F1.3)
- Shipping rates configured per country

## Compliance Note
PAngV: Customers must see ALL costs (including shipping) before entering checkout. 
Non-compliance can result in Abmahnung (cease-and-desist notice)" \
  --label "P0.6,legal,shipping,frontend,sprint-4"

echo "✅ Issue 9 created"

# Issue 10: Dispute Resolution (ODR-VO)
gh issue create \
  --title "P0.6-US-010: Dispute Resolution Contact & Process (ODR-VO)" \
  --body "**Epic:** P0.6 - Store Legal Compliance
**Effort:** 10 hours | **Sprint:** Sprint 4

## User Story
As a customer with a complaint, I want easy access to dispute resolution information and contact details, so that I can escalate unresolved issues to mediation services as required by EU law.

## Acceptance Criteria
- Shop provides contact information for complaints
- Accessible link to dispute resolution in footer/contact page
- Information about available mediation services
- Shop displays: 'We participate in the EU platform for online dispute resolution'
- Link to EU ODR platform (ec.europa.eu/odr)
- Contact email for complaints clearly visible
- Response time SLA stated (e.g., 'We respond within 7 days')
- Complaint form available (or email address)
- Information available in shop language (DE, EN)

## Technical Details
**Frontend:** Static page + footer link | **Backend:** Contact form submission

## Subtasks
- [ ] Create Dispute Resolution page
- [ ] Add link to footer
- [ ] Create contact form
- [ ] Add email routing for complaints
- [ ] Add EU ODR platform link
- [ ] Configure response SLA
- [ ] i18n for all text (DE, EN)
- [ ] Mobile responsive
- [ ] Link testing
- [ ] E2E tests

## Dependencies
- None (independent component)

## Legal References
- [Online Dispute Resolution Regulation (ODR-VO)](https://ec.europa.eu/commission/presscorner/detail/en/IP_15_4713)
- [EU ODR Platform](https://ec.europa.eu/odr)

## Compliance Note
EU law requires this information for all online merchants selling to EU consumers.
Non-compliance can affect seller ratings and customer trust." \
  --label "P0.6,legal,compliance,dispute-resolution,frontend,sprint-4"

echo "✅ Issue 10 created"

echo ""
echo "🎉 All 10 backlog issues created successfully!"
echo "📍 View them at: https://github.com/holger-b2c/B2Connect/issues"
