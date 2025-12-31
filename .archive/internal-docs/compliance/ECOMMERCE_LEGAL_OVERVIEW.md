# 🛍️ E-Commerce Legal Compliance Overview

**Status:** P0.6 Component Ready | **Last Updated:** 28. Dezember 2025

---

## Quick Regulatory Reference

### B2C (Business to Consumer) - Checklist

**VVVG (Verbraucher-VO) - 14-Day Withdrawal Right**
- [ ] Withdrawal form prominently displayed
- [ ] 14 days calculated from delivery date (not order date)
- [ ] Return shipping cost: Seller pays (mandatory)
- [ ] Refund within 14 days of withdrawal notification
- [ ] Item must be in original condition (reasonable wear allowed)
- [ ] Audit log: all returns tracked

**PAngV (Preisangaben-VO) - Price Transparency**
- [ ] Final price visible BEFORE checkout (not after)
- [ ] Price includes VAT (never show price without tax first)
- [ ] Shipping costs shown separately but included in final price
- [ ] Discounts: original price shown with discount percentage
- [ ] No price reduction through vouchers that require purchase of other goods
- [ ] Base price per unit for bulk items

**TMG (Telemediengesetz) - Legal Documents**
- [ ] Impressum (company name, address, contact info) in footer
- [ ] Datenschutzerklärung (Privacy Policy) linked in footer
- [ ] AGB (Terms & Conditions) accepted before purchase
- [ ] E-Mail address for customer support visible
- [ ] Links must be on every page

**Invoicing (German Tax Law)**
- [ ] Invoice number: unique per shop, sequential
- [ ] Invoice date: order date
- [ ] Customer name/address (or anonymized)
- [ ] Product description, quantity, unit price
- [ ] VAT amount visible
- [ ] Total amount
- [ ] Payment terms (due date)
- [ ] Archival: 10 years (not 6, due to German tax law)

---

### B2B (Business to Business) - Checklist

**AStV (Umsatzsteuer-Systemrichtlinie) - Reverse Charge**
- [ ] VAT-ID validation via VIES API (required before checkout)
- [ ] If valid: NO VAT shown in invoice (reverse charge)
- [ ] If invalid: Standard VAT applied
- [ ] Validation caching: 1 year expiration
- [ ] Fallback: if VIES unavailable, assume NO VAT-ID valid

**B2B AGB (Terms & Conditions)**
- [ ] Different from B2C terms (different warranty, payment terms)
- [ ] Payment terms configurable: Net 14, 30, 60, 90, etc.
- [ ] Delivery terms: INCOTERMS support (DDP, DDU, CIF, FOB)
- [ ] Minimum order quantity enforceable
- [ ] Volume discounts configurable
- [ ] Acceptance checkbox before order

**Invoicing & Billing**
- [ ] Invoice shows VAT-ID instead of customer name (if available)
- [ ] Format: PDF + optional eInvoicing (UBL, ZUGFeRD)
- [ ] Payment terms: Net X days (calculation from invoice date)
- [ ] Archival: 10 years
- [ ] EDI/API integration for order automation

---

## Data Structure Reference

### B2C Order Object

```csharp
public class B2cOrder
{
    // Core
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public OrderType Type => OrderType.B2C;
    
    // Customer (encrypted)
    public string CustomerNameEncrypted { get; set; }
    public string CustomerEmailEncrypted { get; set; }
    public string BillingAddressEncrypted { get; set; }
    public string ShippingAddressEncrypted { get; set; }
    
    // Order Details
    public List<OrderItem> Items { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatRate { get; set; } = 0.19m;  // 19% Germany default
    public decimal VatAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal FinalPrice { get; set; }
    
    // Legal Compliance
    public bool TermsAccepted { get; set; }  // Checkbox: AGB
    public DateTime TermsAcceptanceTimestamp { get; set; }
    public string TermsVersionAccepted { get; set; }
    
    // Withdrawal Rights
    public DateTime DeliveryDate { get; set; }
    public int WithdrawalDaysRemaining => 14 - (int)(DateTime.Now - DeliveryDate).TotalDays;
    public bool CanBeReturned => WithdrawalDaysRemaining > 0;
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
```

### B2B Order Object

```csharp
public class B2bOrder
{
    // Core
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public OrderType Type => OrderType.B2B;
    
    // Customer (VAT-ID primary)
    public string CustomerVatId { get; set; }
    public VatValidationResult VatValidation { get; set; }
    public string CustomerCompanyName { get; set; }  // From VIES
    public string CustomerCompanyAddress { get; set; }  // From VIES
    
    // Order Details
    public List<OrderItem> Items { get; set; }
    public decimal SubTotal { get; set; }
    
    // Tax Handling
    public bool ReverseChargeApplies { get; set; }  // If valid VAT-ID
    public decimal VatAmount => ReverseChargeApplies ? 0 : (SubTotal * 0.19m);
    public decimal FinalPrice { get; set; }
    
    // Payment & Shipping Terms
    public int NetDays { get; set; } = 30;  // Net 14, 30, 60, 90, etc.
    public DateTime DueDate => CreatedAt.AddDays(NetDays);
    public string Incoterm { get; set; }  // DDP, DDU, CIF, FOB
    public bool BuyerPaysShipping => Incoterm == "DDU" || Incoterm == "FOB";
    
    // Minimum Order
    public decimal MinimumOrderValue { get; set; } = 0;
    
    // Legal Compliance
    public bool TermsAccepted { get; set; }  // Different AGB for B2B
    public string B2bTermsVersionAccepted { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
```

### Invoice Object

```csharp
public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }  // Unique per shop, sequential
    public Guid TenantId { get; set; }
    
    // Dates
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
    
    // Customer (encrypted)
    public string CustomerNameEncrypted { get; set; }  // B2C
    public string CustomerVatId { get; set; }  // B2B (unencrypted)
    public string CustomerAddressEncrypted { get; set; }
    
    // Items
    public List<InvoiceLineItem> LineItems { get; set; }
    
    // Amounts
    public decimal SubTotal { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    
    // Digital Signature (eIDAS)
    public string DigitalSignature { get; set; }  // Optional for B2C, mandatory for eInvoicing
    public DateTime? SignedAt { get; set; }
    
    // Archival
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public bool IsEncrypted { get; set; } = true;  // AES-256
}
```

---

## Country-Specific VAT Rates

```csharp
public static readonly Dictionary<string, decimal> VatRatesByCountry = new()
{
    // Standard rates
    { "DE", 0.19m },   // Germany 19%
    { "AT", 0.20m },   // Austria 20%
    { "FR", 0.20m },   // France 20%
    { "NL", 0.21m },   // Netherlands 21%
    { "IT", 0.22m },   // Italy 22%
    { "ES", 0.21m },   // Spain 21%
    { "PL", 0.23m },   // Poland 23%
    { "SE", 0.25m },   // Sweden 25%
    { "DK", 0.25m },   // Denmark 25%
    
    // Reduced rates
    { "DE_REDUCED", 0.07m },   // Germany reduced 7%
    { "AT_REDUCED", 0.10m },   // Austria reduced 10%
    { "DE_BOOKS", 0.07m },     // Books 7%
    { "DE_FOOD", 0.07m },      // Food 7%
};
```

---

## INCOTERMS Explanation

```csharp
public enum Incoterm
{
    // Seller pays shipping + duties
    DDP = 1,  // Delivered Duty Paid (most buyer-friendly)
    
    // Seller pays shipping, buyer pays duties
    DDU = 2,  // Delivered Duty Unpaid
    
    // Seller arranges and pays shipping, buyer arranges duties
    CIF = 3,  // Cost, Insurance & Freight
    
    // Buyer arranges and pays shipping
    FOB = 4,  // Free on Board (most seller-friendly)
    
    // International
    EXW = 5,  // Ex Works (buyer arranges everything)
}
```

---

## API Endpoints Needed

### B2C Specific

```
POST /api/orders/b2c/create
  Body: { items: [], billingAddress: {}, termsAccepted: true }
  Returns: { orderId, invoiceNumber, withdrawalFormUrl }

POST /api/orders/{orderId}/return
  Body: { reason: "" }
  Returns: { returnLabel, carrier, trackingNumber }

GET /api/orders/{orderId}/invoice
  Returns: { pdf, invoiceNumber, downloadUrl }

GET /api/legal/terms-b2c
  Returns: { version, content, acceptanceRequired: true }

GET /api/legal/withdrawal-form
  Returns: { form, html }
```

### B2B Specific

```
POST /api/orders/b2b/validate-vat
  Body: { country: "DE", vatId: "123456789" }
  Returns: { isValid: true, companyName, reversedChargeApplies: true }

POST /api/orders/b2b/create
  Body: { items: [], vatId: "", paymentTerms: "Net30", incoterm: "DDP" }
  Returns: { orderId, invoiceNumber, dueDate }

GET /api/orders/{orderId}/invoice-edi
  Returns: { xml: "<ubl:Invoice>...", format: "UBL2.1" }

GET /api/legal/terms-b2b
  Returns: { version, content, incotermsExplained: {} }
```

---

## Implementation Priority

### Phase 0.6a (Weeks 1-2): Core Legal

1. **Invoice Management**
   - Create Invoice entity
   - Generate invoice on order
   - Archive logic (10 years)
   - Encryption

2. **B2C Legal Documents**
   - Withdrawal form
   - Terms acceptance checkbox
   - Return request system

### Phase 0.6b (Weeks 3-4): B2B Tax & Commerce

3. **VAT-ID Validation**
   - VIES API integration
   - Caching (1 year)
   - Reverse charge logic

4. **Payment Terms & Shipping**
   - Payment terms (Net 14/30/60/90)
   - INCOTERMS support
   - Minimum order quantities

---

## Testing Depth

| Component | Unit Tests | Integration Tests | E2E Tests |
|-----------|-----------|------------------|-----------|
| Withdrawal (14-day) | 5 | 3 | 2 |
| Price calculation | 8 | 4 | 1 |
| VAT-ID validation | 6 | 4 | 2 |
| Reverse charge | 5 | 4 | 1 |
| Invoice generation | 6 | 4 | 1 |
| Payment terms | 4 | 3 | 1 |
| INCOTERMS | 4 | 2 | 1 |
| **Total** | **38** | **24** | **9** |

**Total Tests: 71 test cases**

---

## Legal Review Checklist

Before going live, have Legal Team verify:

- [ ] **B2C Terms & Conditions**
  - [ ] 14-day withdrawal right prominently stated
  - [ ] Return shipping cost stated (seller pays)
  - [ ] Impressum included
  - [ ] Privacy statement link

- [ ] **B2B Terms & Conditions**
  - [ ] Payment terms and conditions detailed
  - [ ] INCOTERMS explained
  - [ ] Warranty terms (B2B often differs from B2C)
  - [ ] Dispute resolution clause

- [ ] **General**
  - [ ] AGB (German) or equivalent translated
  - [ ] GDPR compliance statements
  - [ ] Cookie consent if applicable
  - [ ] ODR (Online Dispute Resolution) contact info

---

## Resources

- [P0.6 Implementation Guide](EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md#-p06-e-commerce-legal-compliance-b2b--b2c)
- [P0.6 Testing Examples](P0.6_ECOMMERCE_LEGAL_TESTS.md)
- [VIES VAT Lookup API](https://ec.europa.eu/taxation_customs/vies/)
- [INCOTERMS 2020 Explanation](https://en.wikipedia.org/wiki/Incoterms)
- [German PAngV (Preisangabenverordnung)](https://www.gesetze-im-internet.de/pangv/)
- [German VVVG (Verbrauchervertrag)](https://www.gesetze-im-internet.de/bgb/__312g.html)

