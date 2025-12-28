# GitHub Issues Update: Bestandskunden-Registrierung Integration

## 🎯 Zu aktualisierende Issues

### Issue 1: customer-registration-flow.md (Hauptdokumentation)

**Änderungen:**

Folgende Sektion hinzufügen nach dem "Epic Breakdown" (nach Story 7):

```markdown
---

## 🏢 New Feature: Simplified Registration for Existing Customers

**Status:** New Feature / Sub-Epic  
**Priority:** High  
**Target Sprint:** KW 2 2026  
**Estimated Effort:** 8 Story Points (2,5 Tage)  
**Documentation:** [Bestandskunden Registrierung](../../docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md)

### Problem Statement
- Bestandskunden müssen aktuell alle Felder ausfüllen (schlechte UX)
- Hohe Fehlerquote durch Dateneingaben (Duplikate, Tippfehler)
- Keine Validierung gegen ERP-Masterdaten
- Registrierungsprozess dauert 5+ Minuten

### Solution Overview
Bestandskunden können sich mit Kundennummer oder E-Mail registrieren:
1. Kundendaten aus ERP laden
2. Zur Bestätigung anzeigen
3. Passwort setzen
4. Account sofort aktiv

### Benefits
- ✅ UX: Registration < 2 Min (statt 5+ Min)
- ✅ Datenkonsistenz: 0 Duplikate durch ERP-Validierung
- ✅ Datenqualität: Stammdaten aus ERP (keine Neueingabe)
- ✅ Customer Satisfaction: Schneller Prozess für bekannte Kunden

### User Stories

#### Story 8: Check Customer Type (2 SP)
**As a** store frontend  
**I want to** allow customers to verify if they are existing customers  
**So that** I can offer a simplified registration flow

**Acceptance Criteria:**
- [ ] New endpoint: `POST /api/registration/check-type`
- [ ] Accepts: customerNumber or email
- [ ] Returns: ERP customer data if found
- [ ] Response includes: companyName, contactPerson, email, phone, address
- [ ] Error handling for: not found (404), invalid format (400), ERP unavailable (503)
- [ ] Rate limiting: 3 attempts per 5 minutes per IP
- [ ] Audit logging: All lookup attempts logged
- [ ] Response time < 500ms (95th percentile)

**Technical Tasks:**
- [ ] Backend:
  - [ ] Create `CheckRegistrationTypeCommand` + Handler
  - [ ] Implement ERP lookup service (SAP, REST)
  - [ ] Create validator: `CheckRegistrationTypeCommandValidator`
  - [ ] Add endpoint to `RegistrationController`
- [ ] Frontend:
  - [ ] Create API service: `registrationService.ts`
  - [ ] Add loading + error states
  - [ ] Debounce API calls (500ms)
- [ ] Tests:
  - [ ] Unit tests: Handler, ERP service
  - [ ] Integration tests: API endpoint
  - [ ] E2E tests: Happy path + error cases

---

#### Story 9: Existing Customer Registration Form (3 SP)
**As an** existing customer  
**I want to** register with minimal data (Kundennummer or email + password)  
**So that** I can quickly create an account

**Acceptance Criteria:**
- [ ] Offer choice: "I am an existing customer" vs "I am a new customer"
- [ ] For existing: Input field for customer number OR email
- [ ] "Load data" button fetches from ERP
- [ ] Display confirmation: Company name, contact person, email, address
- [ ] Allow customer to edit address if needed
- [ ] Password field with strength indicator
- [ ] Terms acceptance required
- [ ] Account created and verified automatically
- [ ] No email verification needed (already verified via ERP)

**Technical Tasks:**
- [ ] Backend:
  - [ ] Create `RegisterExistingCustomerCommand` + Handler
  - [ ] Extend User entity: ErpCustomerId, RegistrationType, RegistrationSource
  - [ ] Add domain event: `ExistingCustomerRegisteredEvent`
  - [ ] Create validator: `RegisterExistingCustomerValidator`
  - [ ] Implement email validation against ERP
  - [ ] Add migration for new User fields
- [ ] Frontend:
  - [ ] Create `ExistingCustomerForm.vue` component
  - [ ] Add step: Customer data confirmation
  - [ ] Implement form state management (Pinia)
  - [ ] Error handling: not found, email mismatch, duplicate
- [ ] Tests:
  - [ ] Unit tests: Handler, validators
  - [ ] Integration tests: Registration flow
  - [ ] E2E tests: Complete user journey

---

#### Story 10: Duplicate Detection & Prevention (2 SP)
**As the** system  
**I want to** detect and prevent duplicate accounts  
**So that** data integrity is maintained

**Acceptance Criteria:**
- [ ] Check exact email match (highest priority, 100% confidence)
- [ ] Check ERP customer ID match (100% confidence)
- [ ] Check similar profiles: name + address fuzzy matching (> 85% confidence)
- [ ] Return duplicates with confidence score
- [ ] Suggest: login, password reset, or proceed as new customer
- [ ] Prevent registration if exact match found
- [ ] Warn if similar account detected
- [ ] Log all duplicate detection attempts

**Technical Tasks:**
- [ ] Backend:
  - [ ] Create `IDuplicateDetectionService` interface
  - [ ] Implement Levenshtein distance algorithm
  - [ ] Add fuzzy matching for name + address
  - [ ] Extend User repository: `FindSimilarAsync`, `FindByErpIdAsync`
  - [ ] Integrate into registration handler
- [ ] Frontend:
  - [ ] Display duplicate warning modal
  - [ ] Provide action buttons: login, password reset, proceed
- [ ] Tests:
  - [ ] Unit tests: Fuzzy matching algorithm
  - [ ] Integration tests: Duplicate detection scenarios
  - [ ] E2E tests: User interactions on duplicate

---

#### Story 11: ERP Integration & Data Validation (1 SP)
**As the** system  
**I want to** integrate with ERP (SAP/Oracle) for customer data  
**So that** registration data is accurate and consistent

**Acceptance Criteria:**
- [ ] Support REST API (OData, SOAP) to ERP
- [ ] Fallback to CSV for local development
- [ ] Validate: Customer exists, customer active, email matches
- [ ] Error handling: Timeout (5s), rate limit, invalid response
- [ ] Cache customer data: 1 hour TTL in Redis
- [ ] Log all ERP calls for audit
- [ ] Graceful degradation: Offline mode possible

**Technical Tasks:**
- [ ] Backend:
  - [ ] Create `IErpCustomerService` interface
  - [ ] Implement `SapCustomerService` (OData)
  - [ ] Implement `CsvCustomerService` (fallback)
  - [ ] Add caching layer (Redis)
  - [ ] Implement retry logic + timeouts
  - [ ] Add configuration: ERP endpoint, credentials
- [ ] Configuration:
  - [ ] appsettings.json: ERP endpoint, timeout, retry policy
  - [ ] Secrets: ERP username/password in Key Vault
  - [ ] Feature flag: Use ERP vs CSV

---

### Integration Points

#### With Story 2 (Private Customer Registration)
- Private customers see "Are you an existing customer?" prompt
- If yes: Route to Story 9 (existing customer form)
- If no: Continue with existing private customer form

#### With Story 3 (Business Customer Registration)
- Business customers skip existing customer check (only for B2C)
- Or: Allow business customer lookup by VAT/Tax ID

#### With Story 4 (Email Verification)
- **Existing customers:** No email verification (already verified via ERP)
- **New customers:** Continue with email verification flow

#### With Story 5 (Login)
- Both existing and new customers use same login
- Account created via simplified flow or full form

### API Endpoints Summary

```
New Endpoints:
POST /api/registration/check-type                    # Story 8
POST /api/auth/registration/existing-customer        # Story 9
POST /api/auth/registration/new-customer             # Story 2-3 (updated)

Modified Endpoints:
POST /api/auth/verify-email                          # Skip for existing customers
POST /api/auth/login                                 # Accept both customer types
```

### Database Changes

```sql
-- Extend User table
ALTER TABLE users ADD COLUMN erp_customer_id VARCHAR(100);
ALTER TABLE users ADD COLUMN erp_system_id VARCHAR(50);
ALTER TABLE users ADD COLUMN registration_type VARCHAR(50); -- ExistingCustomer, NewCustomer
ALTER TABLE users ADD COLUMN registration_source VARCHAR(50); -- CustomerNumber, Email, Manual

-- New table: UserRegistration (audit trail)
CREATE TABLE user_registrations (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    tenant_id UUID NOT NULL,
    type VARCHAR(50),
    source VARCHAR(50),
    erp_customer_id VARCHAR(100),
    status VARCHAR(50),
    created_at TIMESTAMPTZ,
    completed_at TIMESTAMPTZ,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id)
);
```

### Configuration Example

```json
{
  "Registration": {
    "AllowExistingCustomers": true,
    "RequireEmailVerificationForExisting": false,
    "DuplicateDetectionThreshold": 0.85,
    "MaxCheckTypeAttempts": 3,
    "CheckTypeWindowMinutes": 5
  },
  "Erp": {
    "Provider": "SAP",
    "Endpoint": "https://erp.company.com/odata/v4/",
    "Timeout": 5000,
    "CacheExpirationMinutes": 60,
    "RetryPolicy": {
      "MaxRetries": 3,
      "BackoffMultiplier": 2
    }
  }
}
```

### Success Metrics
- Registration time < 2 min for existing customers
- 95%+ ERP lookup success rate
- < 1% false positives in duplicate detection
- Zero data duplicates for ERP-linked accounts
- 99.5% API availability

### Dependencies
- ERP API availability & documentation
- ERP customer data quality
- Email service for verification

### Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| ERP not reachable | CSV fallback, retry logic, offline mode |
| Slow ERP API | Caching, async processing, timeout |
| Bad customer data | Validation rules, admin review option |
| Performance impact | Async processing, caching, indexing |

---

```

---

## Implementierungsanleitung

### Schritt 1: Existierendes Issue aktualisieren

1. GitHub Repository öffnen
2. Issues → `customer-registration-flow.md` (aus `.github/ISSUE_TEMPLATE`)
3. Folgende Inhalte nach "Story 7" einfügen:
   - New Feature: Simplified Registration for Existing Customers Sektion (oben)
   - 4 neue Stories: 8, 9, 10, 11
   - Integration Points, API Endpoints, Database Changes

### Schritt 2: Neue Sub-Issues erstellen (optional)

Falls GitHub Issues verwendet werden (nicht nur Templates):

```
Issue #XXX: Story 8 - Check Customer Type [Backend]
Issue #XXX: Story 9 - Existing Customer Registration Form [Backend + Frontend]
Issue #XXX: Story 10 - Duplicate Detection [Backend]
Issue #XXX: Story 11 - ERP Integration [Backend]
```

### Schritt 3: Parent-Child Beziehung setzen

In GitHub (mit Projects):
- Parent: "Customer Registration Flow Epic"
  - Child: Story 8
  - Child: Story 9
  - Child: Story 10
  - Child: Story 11

### Schritt 4: Labels hinzufügen

```
Stories:
- label: registration
- label: erp-integration
- label: p1-high
- label: backend
- label: frontend
- label: 2-story-points (Story 8)
- label: 3-story-points (Story 9)
- label: 2-story-points (Story 10)
- label: 1-story-point (Story 11)

Epic:
- label: epic
- label: registration
- label: 8-story-points
- label: kw2-2026
```

### Schritt 5: Milestone setzen

- Milestone: "Q1 2026 Sprint 1 (KW 2)"
- Due Date: 10. Januar 2026
- Target Sprint: KW 2 2026

---

## Verlinkte Dokumentation

- Spezifikation: `docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md`
- Implementation Scaffold: `docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md`
- Quick-Start: `docs/features/BESTANDSKUNDEN_QUICK_START.md`

---

## Checkliste für Issue-Manager

- [ ] Existierendes Template aktualisieren: customer-registration-flow.md
- [ ] Neue Stories hinzugefügt (8-11)
- [ ] Integration Points dokumentiert
- [ ] API Endpoints definiert
- [ ] Database Changes aufgelistet
- [ ] Configuration Examples bereitgestellt
- [ ] Success Metrics definiert
- [ ] Risks dokumentiert
- [ ] Links zu Dokumentationen gesetzt
- [ ] Labels aktualisiert
- [ ] Milestone zugewiesen
- [ ] Team benachrichtigt

---

**Letzte Aktualisierung:** 28. Dezember 2025  
**Status:** Ready to Update Issues
