# ⚖️ Legal / Compliance Officer Quick Start

**Role Focus:** Regulatory interpretation, legal review, risk assessment, contract negotiation  
**Time to Productivity:** 2 weeks  
**Critical Components:** P0.6, P0.7, P0.8, P0.9

---

## ⚡ Week 1: EU Regulatory Framework

### Day 1: Primary Regulations
```
Applicable to B2Connect SaaS:

1. NIS2 (Netz- und Informationssicherheitsgesetz 2.0)
   In Force: 13. Sept 2024 → Umgesetzt bis 17. Okt 2024
   Scope: Supply chain security, incident response
   Key: Art. 21 (Risk measures), Art. 23 (Incident notification < 24h)

2. GDPR (Datenschutz-Grundverordnung)
   In Force: 25. Mai 2018 → Active
   Scope: Personal data protection, privacy rights
   Key: Art. 32 (Encryption), Art. 33 (Breach notification)

3. DORA (Digital Operational Resilience Act)
   In Force: 16. Dez 2022 → Umgesetzt bis 17. Jan 2025
   Scope: ICT risk management, operational resilience
   Key: Art. 6 (Risk framework), Art. 19 (Incident reporting)

4. EU AI Act
   In Force: 12. Mai 2026 (compliance window open now!)
   Scope: AI risk management, transparency, audit
   Key: Art. 6 (Prohibited/High-risk classification)
   Fines: Up to €30M or 6% global revenue

5. BITV 2.0 / BFSG (Barrierefreiheit- Gesetz)
   In Force: 28. Juni 2025 (ALREADY ACTIVE!)
   Scope: Accessibility for disabilities
   Fines: €5,000-100,000 per violation
   Standard: WCAG 2.1 Level AA

6. E-Commerce Regulations (DE)
   PAngV: Price transparency (final price, shipping visible)
   VVVG: 14-day withdrawal right
   TMG: Impressum (company info), Privacy statement
   VerpackG: Packaging registration
```

### Day 2: E-Commerce Legal (P0.6)
```
B2C (Consumer) Requirements:

MUST HAVE:
✅ Final Price (inkl. MwSt) → PAngV (§1)
✅ Shipping Costs visible BEFORE checkout → PAngV (§1)
✅ 14-Day Withdrawal Right → VVVG (§312g)
✅ Withdrawal Form → VVVG (§312g, §312i)
✅ Terms & Conditions (AGB) acceptance → BGB (§305)
✅ Privacy Policy (Datenschutz) → GDPR (Art. 13)
✅ Impressum → TMG (§5)
✅ Order Confirmation Email → VVVG (§312d)
✅ Invoice with all details → EstV/AStV
✅ 10-Year Invoice Archival → AStV (§19)

PENALTIES FOR NON-COMPLIANCE:
❌ Missing withdrawal info → €2,500-25,000 fine (pro violation)
❌ Wrong VAT calculation → €5,000-300,000 fine
❌ No impressum → Abmahnung (cease & desist)
❌ No data privacy → €20M GDPR fine or 4% revenue

---

B2B (Business) Requirements:

ADDITIONAL REQUIREMENTS:
✅ VAT-ID Validation (VIES API) → UStG (§18)
✅ Reverse Charge Logic (No VAT if buyer has VAT-ID) → UStG (§13b)
✅ Payment Terms configurable (Net 30, 60, etc.) → AGB
✅ INCOTERMS support (DDP, DDU, FOB, etc.) → international trade
✅ EDI/API integration ready → large partner support
✅ Digital Invoice Option (ZUGFeRD/UBL) → eIDAS 2.0

PENALTIES:
❌ Wrong VAT calculation → €5,000-300,000 fine
❌ Reverse charge not applied → Tax audit + back taxes + penalties
```

### Day 3: AI Act Compliance (P0.7)
```
EU AI Act Risk Classification (Effective May 12, 2026):

PROHIBITED AI (Banned):
❌ Social credit scoring
❌ Subliminal manipulation
❌ Exploitation of children
(These = automatic fine + ban)

---

HIGH-RISK AI (Requires Full Compliance):
⚠️ Credit/loan decisions
⚠️ Employment decisions
⚠️ Fraud detection in payments
⚠️ Recommendation engines (if they significantly affect purchasing)

For HIGH-RISK B2Connect AI (e.g., Fraud Detection):
MUST IMPLEMENT:
✅ Risk Register: Document all AI systems & risks
✅ Technical Documentation: Training data, validation, limitations
✅ Responsible Person: Named individual (Art. 22)
✅ Decision Logging: Record every AI decision affecting users
✅ Bias Testing: Detect discriminatory outcomes
✅ Performance Monitoring: Alert if accuracy drops > 5%
✅ Human Review: Qualified person reviews high-risk outputs
✅ User Transparency: "This decision was AI-assisted"
✅ User Right to Explanation: API to request why AI made decision
✅ Opt-Out Capability: Users can disable AI-based decisions

PENALTIES FOR HIGH-RISK VIOLATIONS:
❌ Non-compliance → €15M or 3% global revenue (whichever higher)

---

LIMITED-RISK AI (Requires Transparency):
⚠️ Recommendation engines
⚠️ Content moderation

MUST: Disclose to users "This is AI-powered"

---

MINIMAL-RISK AI (General compliance):
✅ Dynamic pricing
✅ Search ranking (if not heavily biased)
```

### Day 4: BITV / Accessibility (P0.8 - MOST CRITICAL DEADLINE!)
```
BITV 2.0 / BFSG (Barrierefreiheit-Gesetz)

EFFECTIVE: 28. Juni 2025 (⚠️ LESS THAN 6 MONTHS!)

Scope: Online shops, e-commerce services (YES, B2Connect included!)

Standard: WCAG 2.1 Level AA

REQUIREMENTS:
✅ Keyboard navigation: All functions without mouse
✅ Screen readers: Content announced correctly
✅ Color contrast: >= 4.5:1 (dark text on light)
✅ Text resize: Works at 200% zoom
✅ Video captions: German + English minimum
✅ Alt text: All images described
✅ Heading hierarchy: Correct H1-H6 structure
✅ Forms: Labels, error messages clear
✅ Focus indicators: Visible on all elements

WHO IS LIABLE:
- Shops using B2Connect platform
- B2Connect (if shop UI generated by platform)
- Both can be liable for violations

PENALTIES:
🚨 €5,000 - €100,000 per violation
🚨 Can apply PER USER COMPLAINT
🚨 Class-action lawsuits possible (from disability rights orgs)

DEADLINE IS NON-NEGOTIABLE!
If not compliant by 28. Juni 2025:
  → Shop owners can be fined
  → B2Connect liable if platform caused non-compliance
  → Contracts can be terminated
  → Reputation damage (public lawsuit list)
```

### Day 5: E-Rechnung (P0.9)
```
E-Rechnung / ERechnungsVO (E-Invoice Regulation)

Scope: B2B and B2G (government) invoices

DEADLINES:
📅 1. Jan 2026: Mandatory for B2G (government procurement)
📅 1. Jan 2027: Mandatory for B2B receiving (accepting e-invoices)
📅 1. Jan 2028: Mandatory for B2B sending (issuing e-invoices)

REQUIREMENTS:
✅ ZUGFeRD 3.0 format (XML embedded in PDF)
✅ Digital signature (XAdES - eIDAS standard)
✅ 10-year archival (immutable, encrypted)
✅ UBL 2.3 alternative format
✅ ERP integration (Leitweg-ID for B2G)
✅ Automated processing capability

BENEFITS:
✅ Automated invoice processing (saves manual entry)
✅ Faster payment cycles
✅ Reduced errors
✅ Compliance with government requirements

PENALTIES FOR NON-COMPLIANCE:
❌ B2G invoices rejected (no payment)
❌ Contract termination with government buyers
❌ Business impact: Loss of revenue
```

---

## ⚡ Quick Commands (Legal Review)

```bash
# Check compliance test status
dotnet test --filter "Category=Compliance" -v minimal

# Verify encryption implementation
dotnet test backend/Domain/Identity/tests -v minimal

# Check audit logging
dotnet test --filter "FullyQualifiedName~AuditLog"

# BITV accessibility audit
npx @axe-core/cli http://localhost:5173  # Should show 0 critical issues
```

---

## 📚 Critical Documents

| Document | Purpose | Review Time |
|----------|---------|------------|
| `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | Full roadmap | 90 min |
| `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | E-Commerce legal tests | 30 min |
| `docs/P0.7_AI_ACT_TESTS.md` | AI Act compliance tests | 30 min |
| `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | Accessibility tests | 30 min |
| `docs/P0.9_ERECHNUNG_TESTS.md` | E-Invoice tests | 30 min |
| `docs/ECOMMERCE_LEGAL_OVERVIEW.md` | E-Commerce legal overview | 20 min |
| `docs/AI_ACT_OVERVIEW.md` | AI Act overview | 20 min |

---

## ⚖️ Legal Review Checklist

### P0.6: E-Commerce Legal
- [ ] Terms & Conditions DACH-compliant
- [ ] Withdrawal form (14-day period)
- [ ] Price display: Final price + shipping + VAT
- [ ] Impressum complete (company info)
- [ ] Privacy statement (GDPR Art. 13)
- [ ] VAT calculation (B2C vs B2B)
- [ ] VIES integration (B2B VAT-ID validation)
- [ ] Invoice retention (10 years)

### P0.7: AI Act Compliance
- [ ] Risk register documented
- [ ] Fraud detection classified as HIGH-RISK
- [ ] Responsible person named
- [ ] Decision logging implemented
- [ ] Bias testing framework
- [ ] User transparency ("AI-powered")
- [ ] Opt-out capability

### P0.8: BITV / Accessibility
- [ ] Keyboard navigation tested
- [ ] Screen reader compatible
- [ ] Color contrast >= 4.5:1
- [ ] WCAG 2.1 AA compliant
- [ ] Lighthouse score >= 90
- [ ] axe DevTools: 0 critical

### P0.9: E-Rechnung
- [ ] ZUGFeRD 3.0 validation
- [ ] Digital signature (XAdES)
- [ ] 10-year archival (immutable)
- [ ] ERP integration ready
- [ ] UBL 2.3 alternative

---

## 🚨 Regulatory Deadlines (Non-Negotiable!)

| Deadline | Regulation | Action Required | Penalty |
|----------|-----------|-----------------|---------|
| **28. Juni 2025** | **BITV 2.0** | **P0.8 complete** | **€5K-100K** |
| 17. Okt 2025 | NIS2 | Incident response procedures | Business shutdown |
| 1. Jan 2026 | E-Rechnung B2G | ZUGFeRD 3.0 generation | Contract loss |
| 12. Mai 2026 | AI Act | Risk management framework | €30M fine |
| 1. Jan 2027 | E-Rechnung B2B Recv | Accept ZUGFeRD invoices | Market requirement |

---

## 📞 Your Key Responsibilities

1. **Regulatory Interpretation:**
   - Translate regulations into requirements
   - Identify compliance gaps
   - Recommend technical solutions

2. **Legal Review:**
   - Review all customer contracts
   - Approve Terms & Conditions
   - Verify Privacy Statement compliance

3. **Risk Assessment:**
   - Identify legal risks per feature
   - Quantify penalty exposure
   - Recommend mitigation

4. **Vendor Contracts:**
   - Data Processing Agreements (DPAs)
   - Third-party compliance verification
   - SLA negotiations

5. **Incident Response:**
   - Provide authority notification procedures (< 24h for NIS2)
   - Customer notification guidance (< 72h for GDPR)
   - Legal documentation for audits

---

**Key Reminders:**
- BITV deadline = 28. Juni 2025 (€5K-100K penalties!)
- NIS2 incident notification = < 24 hours (mandatory)
- GDPR breach notification = < 72 hours (mandatory)
- AI Act = €30M fine exposure for non-compliance
- E-Commerce = €5K-300K per VAT violation
- All deadlines are LAWS, not suggestions
