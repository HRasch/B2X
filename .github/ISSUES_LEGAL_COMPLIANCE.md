# Legal / Compliance Officer - Zugeordnete Issues

**Status**: 0/5 Assigned  
**Effort**: 20-30 Stunden  
**Kritischer Pfad**: Sprint 1 (alle 5 Issues)

---

## Sprint 1 (P0.6 - German E-Commerce Law)

| # | Titel | Effort | Abhängigkeiten |
|---|-------|--------|-----------------|
| #29 | VVVG 14-Day Return Policy Specs | 4h | - |
| #41 | AGB (Allgemeine Geschäftsbedingungen) Review | 6h | - |
| #42 | Datenschutzerklärung & Impressum | 2h | - |

**Summe Sprint 1**: 12 Stunden

---

## Sprint 2-3 (BITV + AI Act)

| # | Titel | Effort | Abhängigkeiten |
|---|-------|--------|-----------------|
| #43 | BITV 2.0 Accessibility Review | 4h | Frontend #15, #16 (UI) |
| #44 | AI Act Risk Assessment | 4h | Product Definition |

**Summe Sprint 2-3**: 8 Stunden

---

## Detaillierte Anforderungen

### #29 - VVVG 14-Day Return Policy

**German Law Reference**: Verbrauchergüteverordnung (VVVG)  
**Deliverables**:
- [ ] Return window specification (14 days ab Lieferung)
- [ ] Return process documentation (wie Kunde returnen kann)
- [ ] Shipping cost responsibility (wer zahlt Rückversand?)
- [ ] Refund timeline (max. 30 Tage)
- [ ] Exception handling (Verschleiß, falsche Nutzung)

**Tech Integration**:
- Backend: #29 (Implementation) + #23 (Return Management)
- Frontend: #40 (Transparency Display)

---

### #41 - AGB (Terms & Conditions)

**Deliverables**:
- [ ] B2B vs B2C Unterscheidung
- [ ] VAT handling (Reverse Charge für B2B)
- [ ] Payment terms specification
- [ ] Liability limitations (PAngV compliant)
- [ ] Dispute resolution procedure
- [ ] Checkbox design (Frontend #41)

**References**:
- PAngV (Preisangabenverordnung)
- BGB (Bürgerliches Gesetzbuch)
- BDSG (Bundesdatenschutzgesetz)

---

### #42 - Datenschutz & Impressum

**Deliverables**:
- [ ] Privacy Policy (GDPR compliant)
- [ ] Legal Notice (Impressum per TMG §5)
- [ ] Data Processing Agreement (DPA for Subprocessors)
- [ ] Cookie Policy
- [ ] Right to access/delete information

**Links/Pages**:
- `/impressum` (Imprint)
- `/datenschutz` (Privacy Policy)
- `/widerrufsrecht` (Right of Withdrawal)

---

### #43 - BITV 2.0 Barrierefreiheit

**Deadline**: **28. Juni 2025** (BFSG - Barrierefreiheitsstärkungsgesetz)

**Review Scope**:
- [ ] WCAG 2.1 Level AA compliance
- [ ] Accessibility testing (screen readers, keyboard navigation)
- [ ] Color contrast ratios
- [ ] Alt text for images
- [ ] Form labels & error messages

**Frontend Owner**: #15, #16, #19

---

### #44 - AI Act Risk Assessment

**German/EU Regulation**: Artificial Intelligence Act (effective Feb 2024)

**Assessment Required**:
- [ ] Identify AI/ML systems in platform
- [ ] Risk classification (Prohibited, High, Limited, Minimal)
- [ ] Compliance documentation
- [ ] Human oversight procedures
- [ ] Transparency requirements

**Likely Items**:
- Payment Fraud Detection (#35) → High Risk
- Price Calculation? (#20) → Minimal Risk (rules-based)
- Any ML recommendation engine? → High Risk

---

## Regulatory References

| Regulation | Scope | Owner |
|-----------|-------|-------|
| **PAngV** | Price Display, Transparency | #41 (AGB) |
| **VVVG** | Return Rights, Withdrawal | #29 |
| **GDPR** | Data Protection, Privacy | #42 |
| **BDSG** | German Data Protection | #42 |
| **TMG §5** | Legal Notice (Impressum) | #42 |
| **BITV 2.0** | Accessibility Standards | #43 |
| **BFSG** | Accessibility Law (28.06.2025 deadline) | #43 |
| **AI Act** | Artificial Intelligence Compliance | #44 |
| **ERechnungsVO** | E-Invoice Requirements | Backend #21 |

---

## Frontend Integration Points

| Legal Issue | Frontend Issue | Status |
|------------|----------------|--------|
| #41 (AGB) | #41 (Checkbox) | Need Legal Text First |
| #42 (Privacy) | #42 (Links) | Need Legal Text First |
| #43 (BITV) | #15, #16, #19 | Concurrent Review |

**Workflow**: 
1. Legal drafts text/requirements (#41, #42, #43, #44)
2. Frontend implements UI (#41, #42)
3. Legal reviews implementation

---

## Deliverable Format

For each issue, provide:

```markdown
## #[Issue Number] - [Title]

### Legal Requirement
[Reference to law/regulation]

### Specification
- Requirement 1
- Requirement 2

### Acceptance Criteria
- [ ] Criteria 1
- [ ] Criteria 2

### Implementation Notes
[For Dev teams]
```

---

## Nächste Schritte

1. **1 Legal/Compliance Officer zuweisen** (5 Issues, 30-40 Stunden)
2. **Sprint 1 Fokus**: #29, #41, #42 (parallel Frontend)
3. **Sprint 3 Fokus**: #43 (BITV Review), #44 (AI Act Assessment)
4. **Parallel**: Coordinate with Frontend (#41, #42) and Backend (#29, #23)

**Hinweis**: Legal Officer ist **Part-Time** möglich (10-15h/Woche)
