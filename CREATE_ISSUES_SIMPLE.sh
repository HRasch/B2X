#!/bin/bash

# P0.6 Backlog Issues - Simplified version with only existing labels

echo "🚀 Creating P0.6 Backlog Issues (10 total)..."
echo ""

# Issue 1: B2C Price Transparency
gh issue create \
  --title "P0.6-US-001: B2C Price Transparency (PAngV Compliance)" \
  --body "## Story
As a B2C customer, I want to see prices with VAT included, so that I know the exact cost before checkout (PAngV requirement).

## Acceptance Criteria
- Product listing displays price as '€99,99 inkl. MwSt'
- Checkout shows price breakdown: Base + VAT
- Currency symbols correct per country
- Discount prices show original price crossed out

## Technical
Backend: PriceCalculationService
Frontend: ProductCard component

## Dependencies
- VAT master data must be configured" \
  --label "P0.6,legal,pricing,backend,sprint-1" && echo "✅ Issue 1 created"

# Issue 2: B2B VAT-ID Validation
gh issue create \
  --title "P0.6-US-002: B2B VAT-ID Validation (Reverse Charge)" \
  --body "## Story
As a B2B customer, I want to validate my VAT-ID at checkout, so that I can apply reverse charge and avoid double VAT.

## Acceptance Criteria
- VAT-ID lookup via VIES API
- Valid IDs show company name
- Reverse charge applied automatically
- No VAT charged if VAT-ID valid

## Technical
Backend: VatIdValidationService
Integrations: VIES API (ec.europa.eu)

## Subtasks
- [ ] Implement VIES API client
- [ ] Add VAT-ID validation logic
- [ ] Create database schema for VAT lookups
- [ ] Add tests

## Dependencies
- Requires P0.6-US-001 (price data)" \
  --label "P0.6,legal,backend,sprint-1" && echo "✅ Issue 2 created"

# Issue 3: 14-Day Withdrawal Right
gh issue create \
  --title "P0.6-US-003: 14-Day Withdrawal Right (VVVG Compliance)" \
  --body "## Story
As a B2C customer, I want to return items within 14 days of delivery, so that I can exercise my legal withdrawal right.

## Acceptance Criteria
- Return form accessible from order details
- 14-day counter shows days remaining
- Return label auto-generated
- Refund processed within 14 days of return

## Technical
Backend: ReturnManagementService
Integrations: Carrier APIs for label generation

## Dependencies
- Order system (F1.3) must be deployed" \
  --label "P0.6,legal,backend,sprint-2" && echo "✅ Issue 3 created"

# Issue 4: Order Confirmation Email
gh issue create \
  --title "P0.6-US-004: Order Confirmation Email (TMG Compliance)" \
  --body "## Story
As a customer, I want to receive order confirmation with legal information, so I have proof of purchase and legal terms.

## Acceptance Criteria
- Email sent immediately after order
- Includes order details, items, tracking
- Links to Terms & Privacy Policy
- Responsive design (mobile-friendly)
- Available in customer language

## Technical
Backend: OrderConfirmationEmailService
Frontend: Email template editor

## Dependencies
- Order system (F1.3) must be deployed" \
  --label "P0.6,legal,backend,sprint-2" && echo "✅ Issue 4 created"

# Issue 5: Invoice Generation & Archival
gh issue create \
  --title "P0.6-US-005: Invoice Generation & 10-Year Archival" \
  --body "## Story
As a business, I want to generate invoices and archive them for 10 years encrypted, so I comply with German tax law.

## Acceptance Criteria
- Invoice generated with order data
- PDF created with all required fields
- Encrypted storage (AES-256)
- 10-year retention enforced
- Immutable (cannot be modified after creation)

## Technical
Backend: InvoiceGenerationService
Storage: S3/Azure Blob (encrypted)
Format: PDF + ZUGFeRD XML (future)

## Subtasks
- [ ] Create invoice entity
- [ ] Implement PDF generation
- [ ] Add encryption/storage
- [ ] Create archival policy

## Dependencies
- Requires P0.6-US-001 (price data)" \
  --label "P0.6,legal,backend,sprint-2" && echo "✅ Issue 5 created"

# Issue 6: AGB & Datenschutz Acceptance
gh issue create \
  --title "P0.6-US-006: AGB & Datenschutz Acceptance (Legal Gate)" \
  --body "## Story
As the business, I want customers to accept Terms & Privacy Policy before ordering, so I have legal protection.

## Acceptance Criteria
- Checkbox at checkout (must-check before payment)
- Both Terms AND Privacy Policy must be accepted
- Timestamp of acceptance logged
- Cannot proceed without acceptance
- Links to full documents

## Technical
Frontend: Checkout form
Backend: Acceptance logging

## Dependencies
- Checkout system (F1.3) must exist" \
  --label "P0.6,legal,frontend,backend,sprint-3" && echo "✅ Issue 6 created"

# Issue 7: Impressum & Privacy Links
gh issue create \
  --title "P0.6-US-007: Impressum & Privacy Links (TMG §5)" \
  --body "## Story
As a customer, I want to easily find company info and privacy policy in the footer, so I can understand data handling and contact the business.

## Acceptance Criteria
- Footer links to Impressum (company info)
- Footer links to Datenschutzerklärung (privacy)
- Links visible on all pages
- Accessible and compliant with TMG §5
- Mobile-friendly footer layout

## Technical
Frontend: Footer component
Content: CMS-managed pages

## Dependencies
- Shop settings (company info) must exist" \
  --label "P0.6,legal,frontend,sprint-3" && echo "✅ Issue 7 created"

# Issue 8: B2B Payment Terms Configuration
gh issue create \
  --title "P0.6-US-008: B2B Payment Terms (Net 30/60/90)" \
  --body "## Story
As a B2B shop owner, I want to configure payment terms (Net 30/60/90 days), so customers know when payment is due.

## Acceptance Criteria
- Payment terms configurable in admin
- Customer sees due date at checkout
- Invoice shows payment terms
- Payment reminders sent automatically
- Configurable per product/customer

## Technical
Backend: PaymentTermsService
Admin: Settings page

## Dependencies
- Invoice system (P0.6-US-005) must exist" \
  --label "P0.6,legal,backend,sprint-3" && echo "✅ Issue 8 created"

# Issue 9: Shipping Cost Transparency
gh issue create \
  --title "P0.6-US-009: Shipping Cost Transparency (PAngV)" \
  --body "## Story
As a customer, I want to see shipping costs BEFORE checkout, so I can decide if I want to proceed (PAngV requirement).

## Acceptance Criteria
- Shipping cost visible in cart
- Calculated based on destination country
- Real-time updates when address changes
- No hidden fees
- Clear breakdown (product + shipping + tax)

## Technical
Backend: ShippingCalculationService
Frontend: Cart/Checkout display

## Dependencies
- Cart system (F1.3) must exist" \
  --label "P0.6,legal,frontend,backend,sprint-4" && echo "✅ Issue 9 created"

# Issue 10: Dispute Resolution
gh issue create \
  --title "P0.6-US-010: Dispute Resolution (ODR-VO Compliance)" \
  --body "## Story
As a customer with a dispute, I want access to EU online dispute resolution, so I can resolve conflicts outside court.

## Acceptance Criteria
- Link to EU ODR platform (ec.europa.eu/odr)
- Contact form for disputes
- Information about dispute process
- Link available in footer
- Accessible and in customer language

## Technical
Frontend: Dispute page + footer link
Integration: EU ODR platform link

## Dependencies
- None (independent)" \
  --label "P0.6,legal,frontend,sprint-4" && echo "✅ Issue 10 created"

echo ""
echo "🎉 All 10 backlog issues created successfully!"
echo "📍 View at: https://github.com/HRasch/B2Connect/issues?q=label%3AP0.6"
