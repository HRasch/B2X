# 🚀 Compliance Quick Reference - Developer Guide

**One-pager for developers** - Print & keep handy!

---

## Before You Start Coding

### 1. Understand What You're Building
```bash
# Read the GitHub issue
# Check: Does it involve...
- [ ] UI/Frontend changes? → WCAG 2.1 required
- [ ] Personal data? → GDPR required
- [ ] Pricing/Store? → PAngV required
- [ ] Sensitive operation (auth, payment)? → Security + Audit Logging
```

### 2. Add Compliance Label
```bash
# When creating/updating GitHub issue
gh issue edit ISSUE_ID --add-label "wcag-2.1-review"    # if UI
gh issue edit ISSUE_ID --add-label "gdpr"               # if data
gh issue edit ISSUE_ID --add-label "pangv"              # if store
gh issue edit ISSUE_ID --add-label "security"           # all code
gh issue edit ISSUE_ID --add-label "audit-logging"      # if sensitive
```

### 3. Request Compliance Review
```bash
# Assign compliance experts to the issue
gh issue edit ISSUE_ID --add-assignee "@UI"             # WCAG
gh issue edit ISSUE_ID --add-assignee "@Security"       # GDPR, Security
gh issue edit ISSUE_ID --add-assignee "@Legal"          # PAngV, Legal
```

---

## WCAG 2.1 AA Checklist (Frontend)

**Quick check before submitting PR:**

```markdown
- [ ] Can I use TAB to navigate all interactive elements?
- [ ] Do buttons/links have visible focus indicator (`:focus`)?
- [ ] Text contrast ≥ 4.5:1 (use https://webaim.org/resources/contrastchecker/)
- [ ] Form labels properly associated (`<label for="id">`)
- [ ] Images have alt text (`:alt="description"`)
- [ ] Headings in correct order (`<h1>` → `<h2>` → `<h3>`)
- [ ] No keyboard traps (can always TAB to next element)
- [ ] Color not the only way to convey info (use text + icon)
- [ ] Videos have captions (if applicable)
```

**Example Code**:
```html
<!-- ✅ Good -->
<label for="email">Email:</label>
<input id="email" type="email" required />

<!-- ❌ Bad -->
<input type="email" placeholder="Email" />
```

```css
/* ✅ Good - High contrast */
color: #333;          /* 99.9% on white */
color: #2563eb;       /* 8.59:1 on white */

/* ❌ Bad - Low contrast */
color: #999;          /* 2.1:1 on white - FAILS */
```

---

## GDPR Checklist (Data Protection)

**If your code touches personal data:**

```markdown
- [ ] Privacy notice shown to user
- [ ] Consent obtained (checkbox, explicit action)
- [ ] Data retention TTL specified (e.g., 30 days, 7 years)
- [ ] No sensitive data in logs (passwords, tokens, SSNs)
- [ ] Passwords/tokens encrypted or hashed
- [ ] HTTPS used for data transmission
- [ ] User can export their data (data access right)
- [ ] User can delete their data (right to erasure)
- [ ] Audit log shows who accessed what when
```

**Example Code**:
```typescript
// ✅ Good - Privacy notice + encryption
const encryptedPassword = await bcrypt.hash(password, 10);
logger.info('User created', { userId, email }); // No password!
// User sees: "Your data is protected under GDPR..."

// ❌ Bad - Clear password + no notice
logger.info('User created', { userId, password }); // EXPOSED!
```

---

## Store Legal Compliance (Issue #15) - Current Status

**Issue #15: "Implement Store Legal Compliance" - EU E-Commerce Requirements**

### Critical Gaps Identified (Blockers for Sprint Implementation)

#### 1. Legal Documents Missing (TMG & GDPR Violation)
- ❌ **No Impressum page** - Required by TMG §5
- ❌ **No full Privacy Policy page** - Required by GDPR Art. 13/14  
- ❌ **No Terms & Conditions page** - Required for all e-commerce
- ✅ Partial: Footer links exist but lead to "#" (placeholders)

#### 2. Checkout Compliance Missing (GDPR & Contract Law Violation)
- ❌ **No Terms acceptance checkbox** in checkout - Required before purchase
- ❌ **No Privacy Policy acceptance** in checkout - Recommended
- ❌ **No acceptance logging** with timestamp & version

#### 3. Price Display Non-Compliant (PAngV Violation)
- ❌ **Tax shown separately** (€100 + 19% VAT) instead of included (€119 inkl. MwSt)
- ❌ **Shipping costs not visible** before checkout completion
- ❌ **No dynamic shipping recalculation** on country change

#### 4. Backend Services Missing (Multiple Violations)
- ❌ **No LegalDocumentsService** - Cannot store/manage shop-specific legal docs
- ❌ **No VatIdValidationService** - Cannot validate EU VAT-IDs for B2B
- ❌ **No ReturnManagementService** - Cannot enforce 14-day withdrawal period
- ❌ **InvoiceService exists** but lacks VAT breakdown & encryption

#### 5. B2B VAT Handling Missing (AStV Violation)
- ❌ **No VIES API integration** for VAT-ID validation
- ❌ **No reverse charge logic** for intra-EU B2B transactions
- ❌ **No VAT-ID storage** with validation expiry

### Required Changes for Approval

#### Frontend (@Frontend Coordination Required)
1. **Create legal document pages**:
   - `/legal/impressum` - Shop-specific Impressum
   - `/legal/privacy` - GDPR-compliant Privacy Policy  
   - `/legal/terms` - Terms & Conditions with withdrawal notice

2. **Update Checkout.vue**:
   - Add Terms & Privacy checkboxes (Terms required, Privacy optional)
   - Block order placement without Terms acceptance
   - Log acceptance with timestamp, version, IP

3. **Update price displays**:
   - Show "€119,00 inkl. MwSt" everywhere
   - Show shipping costs before checkout
   - Dynamic shipping on country selection

#### Backend (New Services Required)
1. **LegalDocumentsService** - CRUD for shop legal documents with versioning
2. **VatIdValidationService** - VIES API integration for EU VAT validation  
3. **ReturnManagementService** - 14-day window calculation & enforcement
4. **Enhanced InvoiceService** - VAT breakdown, encryption, 10-year archival

### Validation Against Laws

#### GDPR (Data Protection)
- ❌ **Privacy Policy**: Must include data processing purposes, legal basis, retention periods
- ❌ **Consent**: Must be explicit, granular, withdrawable
- ❌ **Data Subject Rights**: Access, rectification, erasure, portability
- ❌ **Audit Logging**: All data processing must be logged

#### PAnGV (Price Transparency)  
- ❌ **Final Price Display**: Must show price including VAT
- ❌ **Shipping Visibility**: Costs must be shown before purchase
- ❌ **No Hidden Costs**: All fees visible upfront

#### TMG (Legal Disclosure)
- ❌ **Impressum**: Required company info, contact details, VAT-ID
- ❌ **Link Visibility**: Must be accessible on every page

#### VVVG (Consumer Rights)
- ❌ **14-Day Returns**: Must be enforced from delivery date
- ❌ **Return Form**: Must be downloadable/printable
- ❌ **Refund Processing**: Within 14 days of withdrawal request

### Approval Status
❌ **NOT APPROVED** for Sprint Implementation

**Reason**: Multiple critical compliance violations that would expose B2X to fines (€5,000-€300,000) and legal action.

**Next Steps**:
1. Implement missing backend services (LegalDocumentsService, VatIdValidationService, ReturnManagementService)
2. Create frontend legal document pages with @Frontend coordination
3. Update checkout flow for mandatory Terms acceptance
4. Convert all price displays to include VAT
5. Re-request legal review before sprint start

**Coordinated with**: @Frontend for UI changes required

---

## PAngV Checklist (Store/Pricing)

**If you handle prices, shipping, or invoicing:**

```markdown
- [ ] All prices show VAT (€99,99 inkl. MwSt)
- [ ] Shipping costs visible BEFORE checkout
- [ ] Shipping recalculates on country change
- [ ] Discounted items show original price (strikethrough)
- [ ] Invoice shows: Subtotal + Tax + Total
- [ ] Estimated delivery time displayed
- [ ] 14-day return/withdrawal right enforced
- [ ] Return form can be downloaded/printed
- [ ] Refunds processed within 14 days
```

**Example Code**:
```typescript
// ✅ Good - Full transparency
const invoice = {
  subtotal: 84.20,
  tax: 15.99,          // 19% VAT explicitly shown
  total: 100.19,
  formatted: '€100,19 inkl. MwSt'
};

// ❌ Bad - Hidden costs
const invoice = {
  price: 100,          // Where's the VAT breakdown?
  shipping: 5.50
};
```

---

## Security Checklist (All Code)

**Every PR must pass:**

```markdown
- [ ] No hardcoded secrets (API keys, tokens, passwords)
  → Use environment variables: process.env.API_KEY
- [ ] Input validation on all endpoints
  → Check length, type, allowed values
- [ ] SQL injection prevention
  → Use parameterized queries, ORM (not string concatenation)
- [ ] XSS protection
  → Output encoded, CSP headers set, no eval()
- [ ] CSRF protection
  → Anti-CSRF tokens in forms
- [ ] Authentication enforced
  → Protected endpoints require valid token/session
- [ ] Authorization checked
  → User can only access own data/resources
- [ ] Sensitive data not in logs
  → Don't log passwords, tokens, SSNs, credit cards
- [ ] Dependencies scanned
  → npm audit, dotnet audit - no critical vulns
- [ ] HTTPS enforced
  → No HTTP for sensitive operations
```

**Example Code**:
```typescript
// ✅ Good - Secure
const apiKey = process.env.API_KEY;     // From environment
const user = db.query('SELECT * FROM users WHERE id = ?', [userId]);
const encrypted = await bcrypt.hash(password, 10);

// ❌ Bad - Insecure
const apiKey = "sk-1234567890";         // HARDCODED!
const user = db.query(`SELECT * FROM users WHERE id = ${userId}`);  // SQL injection!
logger.info('Login', { email, password }); // EXPOSED!
```

---

## Audit Logging Checklist (Sensitive Ops)

**For payments, refunds, auth, data deletion:**

```markdown
- [ ] WHO: User ID and role captured
- [ ] WHAT: Action and resource clearly logged
- [ ] WHEN: Timestamp with timezone
- [ ] WHERE: IP address recorded (if network)
- [ ] WHY: Reason documented (if applicable)
- [ ] HOW MUCH: Before/after values if data changed
- [ ] SENSITIVE DATA NOT LOGGED: No passwords, tokens, PII
- [ ] IMMUTABLE: Logs cannot be deleted (append-only)
- [ ] RETENTION: 7 years for financial data
```

**Example Code**:
```typescript
// ✅ Good - Complete audit trail
auditLog.create({
  timestamp: new Date().toISOString(),
  userId: 'user-123',
  userRole: 'customer',
  action: 'order.refund',
  resource: 'order-456',
  status: 'success',
  changes: {
    status: 'completed -> refunded',
    amount: '€99.99'
  },
  ipAddress: request.ip,
  reason: 'Customer withdrawal request'
});

// ❌ Bad - Missing info
logger.info('Refund processed');  // WHO? WHEN? WHAT AMOUNT?
```

---

## GitHub CLI Quick Commands

### Create Issue with Compliance Label
```bash
gh issue create \
  --title "feat: [Your feature]" \
  --label "wcag-2.1-review"     # Add applicable labels
  --assign @UI                   # Assign reviewer
```

### Check Compliance Reviews Before Merge
```bash
# Are all compliance issues closed?
gh issue list --label "wcag-2.1-review,gdpr,security" --state open

# Request compliance review on PR
gh pr create --required-review-count 2 --reviewers @Security,@TechLead
```

### Find Compliance Docs
```bash
# WCAG accessibility
cat ACCESSIBILITY_COMPLIANCE_REPORT.md

# EU legal requirements
cat ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md

# Governance & specs
cat ../../.ai/knowledgebase/governance.md

# Implementation guide
cat .github/prompts/compliance-integration.prompt.md
```

---

## Common Mistakes (DON'T DO)

| Mistake | Why It's Bad | Fix |
|---------|-------------|-----|
| Hardcoded API key in code | Secret exposed in Git | Use `process.env.API_KEY` |
| Password in logs | Security breach | Remove from logger |
| No password hashing | Easy to crack | Use bcrypt, Argon2, etc. |
| `<input>` without `<label>` | Accessibility fail | Add `<label for="id">` |
| Low color contrast (< 4.5:1) | Hard to read for visually impaired | Use contrast checker tool |
| Price without VAT | PAngV violation | Show €99,99 inkl. MwSt |
| No shipping cost info | PAngV violation | Calculate & show before checkout |
| No input validation | SQL injection, XSS risk | Validate & sanitize all inputs |
| Deleting audit logs | GDPR/compliance violation | Make logs append-only |
| No data retention policy | GDPR violation | Define TTL for all data |

---

## Review Your PR Before Submitting

**Compliance Checklist**:
```markdown
## Pre-Submission Compliance Check

### All Code
- [ ] No hardcoded secrets (passwords, API keys, tokens)
- [ ] No sensitive data in logs (PII, payment info)
- [ ] Input validation on all user inputs
- [ ] SQL injection prevention (parameterized queries)

### If Frontend/UI Changes
- [ ] Keyboard navigable (Tab through all elements)
- [ ] Focus indicators visible on all buttons/links
- [ ] Color contrast ≥ 4.5:1
- [ ] Form labels properly associated
- [ ] Alt text on images

### If Personal Data Involved
- [ ] Privacy notice displayed
- [ ] HTTPS used (no HTTP)
- [ ] Data encrypted at rest + in transit
- [ ] User can export/delete their data
- [ ] Audit log tracks access

### If Pricing/Store Feature
- [ ] Prices include VAT (€X,XX inkl. MwSt)
- [ ] Shipping shown before checkout
- [ ] Tax breakdown on invoice
- [ ] 14-day return period enforced

### If Sensitive Operation (Auth, Payment, Delete)
- [ ] Audit log captures: who, what, when, where, why
- [ ] No sensitive data in audit log
- [ ] All compliance labels applied
- [ ] Compliance reviewers assigned (@Security, @Legal, etc.)

### Documentation
- [ ] Compliance requirements documented in PR description
- [ ] Reference to applicable documents
- [ ] Review by appropriate compliance expert
```

---

## Who to Ask?

| Question | Ask |
|----------|-----|
| Is this accessible? | @UI |
| Is this GDPR compliant? | @Security, @Legal |
| Is this PAngV compliant? | @Legal, @ProductOwner |
| Is this secure? | @Security |
| Should we log this action? | @Security |
| Is this design compliant? | @UI, @Architect |
| How do I implement X securely? | @TechLead, @Security |
| What's the business requirement? | @ProductOwner |
| Is this approved? | @SARAH (if blocked) |

---

## Resources

- **WCAG 2.1 Guidelines**: https://www.w3.org/WAI/WCAG21/quickref/
- **Color Contrast Checker**: https://webaim.org/resources/contrastchecker/
- **GDPR**: https://gdpr-info.eu/
- **PAngV**: https://www.gesetze-im-internet.de/pangv/
- **OWASP Top 10**: https://owasp.org/www-project-top-ten/
- **GitHub CLI Docs**: https://cli.github.com/

---

## Remember

✅ **Compliance is not a burden** — it protects users and the business  
✅ **Compliance is everyone's responsibility** — not just lawyers/security  
✅ **Compliance is cheaper upfront** — than fixing violations later  
✅ **Compliance is automated** — GitHub CLI and CI/CD handle much of it  

---

**Last Updated**: 7. Januar 2026  
**Print This & Keep It Handy!** 📋
