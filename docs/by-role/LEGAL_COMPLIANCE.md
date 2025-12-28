# ⚖️ Legal / Compliance Officer - Documentation Guide

**Role:** Legal / Compliance Officer | **P0 Components:** P0.6, P0.7, P0.8, P0.9  
**Time to Read:** ~3 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als Legal/Compliance Officer bist du verantwortlich für:
- **Regulatory Interpretation** (NIS2, GDPR, AI Act, BITV, ERechnungsverordnung)
- **Legal Review** (Features, Policies, Documentation)
- **Vendor Contracts** (Data Processing Agreements)
- **Incident Response Legal** (Authority Notification)
- **Audit Support** (External Auditors, Certifications)

---

## 📚 Required Reading (P0)

### Week 1: Regulatory Framework

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **EU Compliance Roadmap (FULL!)** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 90 min |
| 2 | **Application Specs** | [APPLICATION_SPECIFICATIONS.md](../APPLICATION_SPECIFICATIONS.md) | 45 min |

### Week 2: Specific Regulations

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 3 | **E-Commerce Legal Overview** | [compliance/ECOMMERCE_LEGAL_OVERVIEW.md](../compliance/ECOMMERCE_LEGAL_OVERVIEW.md) | 30 min |
| 4 | **AI Act Overview** | [compliance/AI_ACT_OVERVIEW.md](../compliance/AI_ACT_OVERVIEW.md) | 30 min |
| 5 | **E-Rechnung Overview** | [compliance/ERECHNUNG_OVERVIEW.md](../compliance/ERECHNUNG_OVERVIEW.md) | 20 min |

### Week 3: Test Specifications (for Verification)

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 6 | **E-Commerce Tests (P0.6)** | [compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md](../compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md) | 20 min |
| 7 | **AI Act Tests (P0.7)** | [compliance/P0.7_AI_ACT_TESTS.md](../compliance/P0.7_AI_ACT_TESTS.md) | 20 min |
| 8 | **BITV Tests (P0.8)** | [compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md) | 15 min |
| 9 | **E-Rechnung Tests (P0.9)** | [compliance/P0.9_ERECHNUNG_TESTS.md](../compliance/P0.9_ERECHNUNG_TESTS.md) | 15 min |

---

## 📋 Regulatory Deadline Overview

### CRITICAL DEADLINES

| Regulation | Deadline | Status | Penalty | Action Required |
|-----------|----------|--------|---------|-----------------|
| **BITV 2.0 / BFSG** | 28. Juni 2025 | 🔴 **ACTIVE** | €5,000-100,000 | P0.8 immediately |
| **NIS2 Directive** | 17. Okt 2025 | 🟡 6 months | Business shutdown | P0.3 priority |
| **AI Act** | 12. Mai 2026 | 🟡 12 months | €30M or 6% revenue | P0.7 documentation |
| **E-Rechnung (B2G)** | 1. Jan 2026 | 🟡 12 months | Contract loss | P0.9 priority |
| **E-Rechnung (B2B empfangen)** | 1. Jan 2027 | 🟢 24 months | Market requirement | Planning |
| **E-Rechnung (B2B senden)** | 1. Jan 2028 | 🟢 36 months | Market requirement | Planning |

### Penalty Framework

| Regulation | Violation | Penalty Range |
|-----------|-----------|---------------|
| **GDPR** | Data breach without notification | Up to €20M or 4% global revenue |
| **NIS2** | Lack of incident response | Up to €10M or 2% global revenue |
| **AI Act** | Deploying prohibited AI | Up to €35M or 7% global revenue |
| **AI Act** | Non-compliance high-risk AI | Up to €15M or 3% global revenue |
| **BITV/BFSG** | Inaccessible services | €5,000-100,000 per violation |
| **ERechnungsVO** | Non-compliant invoice format | Contract rejection, audit issues |

---

## 🔧 Your Review Responsibilities

### P0.6: E-Commerce Legal (15 Tests)

**Your Review Tasks:**
- [ ] AGB (B2C) legally correct for DACH region
- [ ] AGB (B2B) modified warranty clauses valid
- [ ] Widerrufsbelehrung (14 days) compliant with VVVG
- [ ] Datenschutzerklärung GDPR-compliant
- [ ] Impressum complete per TMG §5
- [ ] Price display PAngV-compliant (inkl. MwSt)
- [ ] VAT calculation Reverse Charge rules correct

**Key Legal Questions:**
1. Is the 14-day withdrawal period correctly calculated (from delivery, not order)?
2. Are B2B terms excluding withdrawal right valid?
3. Is the VAT-ID validation sufficient for Reverse Charge?

### P0.7: AI Act (15 Tests)

**Your Review Tasks:**
- [ ] AI Risk Register complete and accurate
- [ ] Fraud Detection classified as HIGH-RISK (Annex III)
- [ ] Responsible Person (Art. 22) designated for each AI system
- [ ] User Right to Explanation text legally sufficient
- [ ] Bias Testing methodology acceptable
- [ ] AI Decision Log retention (how long?)

**Key Legal Questions:**
1. Is Fraud Detection definitely HIGH-RISK (Annex III creditworthiness)?
2. What happens if AI decision is legally challenged?
3. Is our Bias Testing sufficient to demonstrate non-discrimination?
4. What transparency disclosures are required for recommendations?

### P0.8: Barrierefreiheit / BITV 2.0 (12 Tests)

**Your Review Tasks:**
- [ ] BFSG scope applies to B2Connect (e-commerce service)
- [ ] WCAG 2.1 Level AA is sufficient standard
- [ ] Exception claims (if any) are valid
- [ ] Accessibility statement template compliant

**Key Legal Questions:**
1. Does the "unverhältnismäßige Belastung" exception apply?
2. Are we a "Dienstleister" or "Hersteller" under BFSG?
3. What accessibility statement must be published?

### P0.9: E-Rechnung (10 Tests)

**Your Review Tasks:**
- [ ] ZUGFeRD 3.0 format meets EN 16931
- [ ] UBL 2.3 alternative is legally equivalent
- [ ] Invoice archival requirements (10 years, immutable) met
- [ ] Digital signature (XAdES) legally valid
- [ ] Leitweg-ID format for B2G invoices

**Key Legal Questions:**
1. Is XAdES signature sufficient for legal validity?
2. Can we use cloud storage for 10-year archival (GDPR)?
3. What happens if recipient can't process ZUGFeRD?

---

## 📝 Legal Review Templates

### Feature Legal Review Checklist

```markdown
# Legal Review - [Feature Name]

**Reviewer:** [Name]
**Date:** [Date]
**Status:** Draft / Approved / Rejected

## Regulatory Compliance
| Regulation | Applicable | Compliant | Notes |
|-----------|------------|-----------|-------|
| GDPR | ✅/❌ | ✅/❌ | |
| NIS2 | ✅/❌ | ✅/❌ | |
| AI Act | ✅/❌ | ✅/❌ | |
| BITV 2.0 | ✅/❌ | ✅/❌ | |
| ERechnungsVO | ✅/❌ | ✅/❌ | |
| PAngV | ✅/❌ | ✅/❌ | |
| VVVG | ✅/❌ | ✅/❌ | |

## Data Processing
- [ ] Personal data processed: [List]
- [ ] Legal basis: [Consent / Contract / Legitimate Interest]
- [ ] Retention period: [X days/years]
- [ ] Data subject rights implemented

## Contracts Required
- [ ] DPA with subprocessors
- [ ] Terms of Service update
- [ ] Privacy Policy update

## Risks Identified
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| [Risk 1] | L/M/H | L/M/H | [Action] |

## Approval
- [ ] **APPROVED** - No legal issues
- [ ] **APPROVED WITH CONDITIONS** - [Conditions]
- [ ] **REJECTED** - [Reason]

**Signature:** _______________
**Date:** _______________
```

### Incident Response Legal Checklist

```markdown
# Incident Legal Checklist - [Incident ID]

**Incident Type:** [Data Breach / Service Outage / Security Incident]
**Detected:** [DateTime]
**Severity:** [Critical / High / Medium / Low]

## NIS2 Notification Requirements
| Authority | Deadline | Status | Reference |
|-----------|----------|--------|-----------|
| BSI (DE) | < 24h | ⏳/✅ | [Ticket#] |
| DSK (DE) | < 72h (GDPR) | ⏳/✅ | [Ticket#] |

## GDPR Notification Requirements
- [ ] Personal data affected: [Yes/No]
- [ ] Risk to individuals: [High/Low]
- [ ] Supervisory authority notification required: [Yes/No]
- [ ] Individual notification required: [Yes/No]

## Legal Hold
- [ ] Evidence preservation order issued
- [ ] Relevant logs secured
- [ ] Third-party forensics engaged: [Yes/No]

## Communication
- [ ] Internal communication approved
- [ ] External statement approved
- [ ] Customer notification drafted

## Post-Incident
- [ ] Root cause documented
- [ ] Lessons learned captured
- [ ] Process improvements identified
```

---

## 📞 Authority Contacts

### Germany
| Authority | Jurisdiction | Contact |
|-----------|-------------|---------|
| **BSI** | NIS2, Cybersecurity | incident@bsi.bund.de |
| **BfDI** | GDPR (Federal) | poststelle@bfdi.bund.de |
| **BNetzA** | E-Commerce, Telecoms | poststelle@bnetza.de |
| **BAFin** | Financial Services (AI Act) | poststelle@bafin.de |

### Austria
| Authority | Jurisdiction | Contact |
|-----------|-------------|---------|
| **DSB** | GDPR | dsb@dsb.gv.at |
| **NICS** | NIS2 | incident@nics.gv.at |

### EU Level
| Authority | Jurisdiction | Contact |
|-----------|-------------|---------|
| **ENISA** | NIS2 Coordination | info@enisa.europa.eu |
| **EDPB** | GDPR Coordination | edpb@edpb.europa.eu |
| **AI Office** | AI Act Coordination | TBD |

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Security Incident | Security Engineer → CEO | Immediate |
| Data Breach | Legal Officer → DPO → CEO | < 24h |
| Compliance Gap | Legal Officer → Tech Lead | < 48h |
| Contract Issue | Legal Officer → CEO | < 72h |
| Audit Request | Legal Officer → Finance | < 1 week |

---

## ✅ Definition of Done (Legal Review)

Before approving any P0 component:

- [ ] All regulatory requirements identified
- [ ] Compliance tests specified
- [ ] Legal texts reviewed (AGB, Privacy, etc.)
- [ ] Risk assessment completed
- [ ] Authority notification procedures documented
- [ ] Vendor contracts in place
- [ ] Audit trail available
- [ ] Sign-off documented

---

**Next:** Start with [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)
