# GitHub Issues Update Summary - Bestandskunden-Registrierung

**Datum:** 28. Dezember 2025  
**Status:** ✅ Bereit zur Implementierung  
**Zieltermin:** KW 2 2026

---

## 📋 Übersicht der Änderungen

### Hauptdatei aktualisiert:
**`.github/ISSUE_TEMPLATE/customer-registration-flow.md`**

- ✅ Gesamtaufwand erhöht: 13 SP → **21 SP** (8 SP für Bestandskunden-Feature)
- ✅ Neue Sub-Epic hinzugefügt: "Simplified Registration for Existing Customers"
- ✅ 4 neue Stories (8-11) mit vollständiger Beschreibung
- ✅ Integration Points dokumentiert
- ✅ Database Changes & API Endpoints definiert
- ✅ Verlinkt zu detaillierter Dokumentation

---

## 🎯 Neue Stories (KW 2 2026)

### Story 8: Check Customer Type (2 SP)
```
Ziel: Prüfe, ob Kunde im ERP existiert (Kundennummer oder E-Mail)
Backend: CheckRegistrationTypeCommand + ERP-Lookup-Service
Frontend: Registration service + API integration
Tests: Unit, Integration, E2E
```

### Story 9: Existing Customer Registration Form (3 SP)
```
Ziel: Vereinfachte Registrierung mit ERP-Daten
Backend: RegisterExistingCustomerCommand + User-Extension
Frontend: ExistingCustomerForm.vue + Confirmation step
Tests: Validators, API, E2E
```

### Story 10: Duplicate Detection & Prevention (2 SP)
```
Ziel: Vermeide Duplikate (exakt E-Mail, fuzzy matching)
Backend: DuplicateDetectionService + Levenshtein-Algorithmus
Frontend: Duplicate warning modal
Tests: Fuzzy matching, scenarios
```

### Story 11: ERP Integration & Data Validation (1 SP)
```
Ziel: ERP-Verbindung mit Fallback & Caching
Backend: SapCustomerService, CsvCustomerService, Redis-Caching
Configuration: appsettings.json, Key Vault
Tests: Error handling, timeouts
```

**Gesamtaufwand:** 8 Story Points = ~2,5 Tage mit 2 Entwicklern (parallel)

---

## 📁 Verlinkte Dokumentationen

Alle Issues verweisen auf:

1. **Spezifikation** (detailliert)
   - `docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md`

2. **Implementation Scaffold** (Copy-Paste-Ready Code)
   - `docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md`

3. **Quick-Start Guide** (Schritt-für-Schritt)
   - `docs/features/BESTANDSKUNDEN_QUICK_START.md`

---

## 🔄 Integration mit bestehenden Stories

### Mit Story 2 (Private Customer Registration):
```
Neuer Flow:
1. Kunde wählt "Bestehender Kunde?" → Story 8/9
2. Falls nein → Original Story 2 (Private Form)
3. Falls ja → Story 9 (Simplified Form)
```

### Mit Story 4 (Email Verification):
```
Bestandskunden: KEINE E-Mail-Verifikation nötig (ERP-validiert)
Neukunden: Normal E-Mail-Verifikation wie vorher
```

### Mit Story 5 (Login):
```
Beide Typen (Bestands- & Neukunden) nutzen gleichen Login
```

---

## 📊 Story Point Summary

| Story | Titel | SP | Team | Tage |
|-------|-------|-----|------|------|
| 1 | Customer Type Selection | 2 | Frontend | 0,5 |
| 2 | Private Customer Form | 3 | Backend + Frontend | 1 |
| 3 | Business Customer Form | 3 | Backend + Frontend | 1 |
| 4 | Email Verification | 2 | Backend + Frontend | 0,75 |
| 5 | Existing Customer Login | 1 | Backend + Frontend | 0,5 |
| 6 | Email Availability Check | 1 | Backend + Frontend | 0,5 |
| 7 | Password Strength Indicator | 1 | Frontend | 0,25 |
| **Subtotal Sprint 1** | | **13** | | **4,5** |
| | | | | |
| **8** | **Check Customer Type** | **2** | **Backend** | **0,75** |
| **9** | **Existing Customer Form** | **3** | **Backend + Frontend** | **1** |
| **10** | **Duplicate Detection** | **2** | **Backend** | **0,75** |
| **11** | **ERP Integration** | **1** | **Backend** | **0,5** |
| **Subtotal Sprint 2** | | **8** | | **3** |
| | | | | |
| **TOTAL** | | **21** | | **7,5 Tage** |

**Mit 2 Entwicklern parallel: 3-4 Tage für Sprint 2 (KW 2)**

---

## 🔐 Security Updates

Die folgenden Security-Anforderungen wurden hinzugefügt:

- [ ] ERP-Verbindung über verschlüsselt (TLS)
- [ ] Rate Limiting: 3 Attempts per 5 Min pro IP
- [ ] Audit Logging für alle Lookups
- [ ] Duplikat-Erkennung verhindert PII-Durchsickern
- [ ] Tenant-Isolation in allen Queries
- [ ] Fehlerbehandlung ohne sensitive Infos

---

## 📱 API Endpoints (Neu/Geändert)

### Neu (Stories 8-11):
```
POST /api/registration/check-type
├─ Input: customerNumber oder email
├─ Output: ERP customer data oder error
└─ Rate limit: 3/5min

POST /api/auth/registration/existing-customer
├─ Input: erpCustomerId, email, password
├─ Output: user + token
└─ Validation: ERP-Daten + Duplikate
```

### Geändert:
```
POST /api/auth/registration/new-customer (Story 2-3 erweitert)
├─ Private: Simplified form
└─ Business: Full form + address

POST /api/auth/verify-email (Story 4)
├─ Existing customer: Skip this
└─ New customer: Normal flow
```

---

## 🗄️ Database Changes

```sql
-- User Table Extension
ALTER TABLE users ADD COLUMN erp_customer_id VARCHAR(100);
ALTER TABLE users ADD COLUMN erp_system_id VARCHAR(50);
ALTER TABLE users ADD COLUMN registration_type VARCHAR(50);
ALTER TABLE users ADD COLUMN registration_source VARCHAR(50);

-- New Audit Table
CREATE TABLE user_registrations (
    id UUID PRIMARY KEY,
    user_id UUID,
    tenant_id UUID,
    type VARCHAR(50),
    source VARCHAR(50),
    erp_customer_id VARCHAR(100),
    status VARCHAR(50),
    created_at TIMESTAMPTZ,
    completed_at TIMESTAMPTZ
);

-- Indexes
CREATE INDEX idx_users_erp_id ON users(erp_customer_id);
CREATE INDEX idx_registrations_tenant ON user_registrations(tenant_id);
```

---

## ✅ Checkliste für Issue-Manager

Nach Update von `.github/ISSUE_TEMPLATE/customer-registration-flow.md`:

### GitHub Issues:
- [ ] Issue aktualisiert in Repository
- [ ] Labels aktualisiert: `epic`, `registration`, `erp-integration`, `p1-high`
- [ ] Milestone gesetzt: "Q1 2026 Sprint 2 (KW 2)"
- [ ] Assignees: Backend Lead, Frontend Lead
- [ ] Description verweist auf Dokumentationen

### Team Benachrichtigung:
- [ ] Product Owner informiert (21 SP statt 13 SP)
- [ ] Backend Team: Stories 8, 9, 10, 11 zugewiesen
- [ ] Frontend Team: Story 9 Komponenten zugewiesen
- [ ] QA Team: E2E Test Plan für KW 2
- [ ] Architects: Design Review für Stories 10-11

### Abhängigkeiten:
- [ ] ERP-Team: API-Dokumentation für Integration
- [ ] DevOps: Redis-Caching setup
- [ ] Security: ERP-Verbindung (TLS) validieren

---

## 🚀 Nächste Schritte

### Sofort (Today):
1. ✅ GitHub Issue aktualisieren
2. ✅ Team-Meeting: Neue Stories präsentieren
3. ✅ Spezifikationen verteilen

### Diese Woche (28.-31. Dez):
1. Story Refinement für Stories 8-11
2. Definition of Done Review
3. Architektur-Review (Stories 10-11)

### KW 1 2026 (31. Dez - 3. Jan):
1. Sprint Planning Sprint 1 (Stories 1-7)
2. Initial Setup: Backend + Frontend Repos
3. Environment Setup: SAP API / CSV Test-Data

### KW 2 2026 (6. Jan - 10. Jan):
1. Sprint Start Sprint 2 (Stories 8-11)
2. Daily Standups
3. Mid-week Check-in

### KW 3 2026 (13. Jan):
1. Integration Tests
2. E2E Tests
3. Staging Deployment

---

## 📞 Support & Questions

Für Fragen zu:
- **Spezifikation:** `docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md`
- **Code-Scaffold:** `docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md`
- **Implementation:** `docs/features/BESTANDSKUNDEN_QUICK_START.md`
- **Architecture:** `.github/copilot-instructions.md`

---

## 📊 Erfolgs-Metriken (KW 2)

Nach Implementierung sollten folgende Kriterien erfüllt sein:

- ✅ Bestandskunde registriert sich in < 2 Min
- ✅ 0 Duplikate durch ERP-Verknüpfung
- ✅ 95%+ ERP-Lookup-Erfolgsrate
- ✅ < 1% False Positives bei Duplikat-Erkennung
- ✅ 99%+ API-Verfügbarkeit (99.5% Uptime)
- ✅ 80%+ Test-Coverage
- ✅ Security Review ✅ erfolgreich

---

**Dokument Status:** 🟢 Ready  
**Letzte Aktualisierung:** 28. Dezember 2025  
**Next Review:** 3. Januar 2026 (Post-Holidays)
