# GitHub Issue Template: Vereinfachte Registrierung für Bestandskunden

```markdown
# Feature: Vereinfachte Registrierung für Bestandskunden

## 🎯 Ziel
Für Kunden, die bereits im ERP erfasst sind, eine vereinfachte Registrierung ermöglichen, um:
- **UX zu verbessern**: Registrierung in < 2 Min (statt 5+ Min)
- **Datenkonsistenz zu erhöhen**: Keine Duplikate durch ERP-Validierung
- **Datenqualität zu sichern**: Stammdaten aus ERP laden statt neu eingeben

## 📋 Anforderungen

### Functional Requirements
- [ ] Bestandskunde kann sich mit Kundennummer registrieren
- [ ] Bestandskunde kann sich mit E-Mail registrieren
- [ ] ERP-Daten werden automatisch geladen und zur Bestätigung angezeigt
- [ ] Duplikat-Erkennung (exakte E-Mail, ähnliche Profile, ERP-ID)
- [ ] Neukunden-Registrierung mit vollständigem Formular
- [ ] Admin-Validierung optional konfigurierbar
- [ ] Rate Limiting auf Registrierungs-Endpoints
- [ ] Audit Logging für alle Registrierungsversuche

### API Endpoints
- `POST /api/registration/check-type` - Kundendaten abfragen (E-Mail oder Kundennummer)
- `POST /api/auth/registration/existing-customer` - Bestandskunde registrieren
- `POST /api/auth/registration/new-customer` - Neukunde registrieren
- `POST /api/admin/registrations/{id}/validate` - Admin-Validierung

### Security Requirements
- JWT Token-basierte Authentifizierung
- ERP-Daten über verschlüsselte Verbindung (TLS)
- E-Mail-Validierung erforderlich
- Tenant-Isolation (X-Tenant-ID Header)
- Rate Limiting: 3 Versuche pro 5 Min
- Kein Logging sensibler Daten

## 🏗️ Technische Architektur

### Backend
- **Framework**: ASP.NET Core 10, EF Core
- **Pattern**: DDD + CQRS + Repository
- **Service**: Identity Service
- **ERP-Integration**: REST-API (SAP, Oracle) oder CSV (Development)

### Frontend
- **Framework**: Vue 3 + TypeScript
- **State Management**: Pinia
- **UI**: Tailwind CSS

### Database
- **Entity**: UserRegistration (neu)
- **Migrations**: EF Core Migrations
- **Fields**: RegistrationType, RegistrationSource, ErpCustomerId, Status

## 🔄 User Flows

### UC1: Bestandskunde (Kundennummer)
1. Kunde wählt "Ich bin Bestandskunde"
2. Gibt Kundennummer ein
3. System validiert gegen ERP
4. Zeigt Daten zur Bestätigung: Name, Adresse, E-Mail
5. Kunde bestätigt oder korrigiert
6. Gibt Passwort ein
7. Account erstellt, E-Mail-Bestätigung versendet

### UC2: Bestandskunde (E-Mail)
1. Kunde gibt E-Mail ein
2. System prüft ERP (kann mehrdeutig sein)
3. Wenn mehrdeutig: Kundennummer abfragen
4. Rest wie UC1

### UC3: Neukunde
1. Vollständiges Registrierungsformular
2. Duplikat-Prüfung
3. Optional: Admin-Review
4. Account mit Status "Pending"

## 📊 Duplikat-Prävention

- **Exakte E-Mail**: Höchste Priorität (100% Konfidenz)
- **ERP-Kundennummer**: Eindeutig (100% Konfidenz)
- **Fuzzy Matching**: Name + Adresse (Levenshtein Distance > 85%)
- **Ambiguity Resolution**: Benutzer auffordern, eindeutig zu machen

## 🧪 Testing Strategy

### Unit Tests
- [ ] Entity Tests (UserRegistration)
- [ ] Handler Tests (CheckRegistrationType, Register...)
- [ ] Store Tests (registrationStore)
- [ ] Component Tests (RegistrationForm)
- Ziel: 80%+ Coverage

### Integration Tests
- [ ] API Endpoint Tests
- [ ] ERP Lookup + Registration Flow
- [ ] Duplikat-Erkennung
- [ ] Database Interaction

### E2E Tests (Playwright)
- [ ] Happy Path: Bestandskunde registriert sich
- [ ] Error Path: Ungültige Kundennummer
- [ ] Duplikat erkannt
- [ ] Neukunde registriert sich

## 📅 Implementation Roadmap

### Phase 1: Backend (3 Tage)
- Datenmodell (Entities, Repositories)
- ERP-Integration (REST-Client)
- CQRS Handler
- API Endpoints
- Unit Tests

### Phase 2: Frontend (2,5 Tage)
- Pinia Store
- Vue Komponenten
- Form Validation
- Unit Tests

### Phase 3: Integration & QA (1 Tag)
- E2E Tests
- Integration Tests
- Performance Tests

## 🔐 Security Checklist
- [ ] Keine hardcodierten Secrets
- [ ] ERP-Verbindung verschlüsselt (TLS)
- [ ] Tenant-Isolation validiert
- [ ] Rate Limiting implementiert
- [ ] Audit Logging aktiv
- [ ] Input Validation auf Server-Seite
- [ ] Fehler-Messages nicht zu detailliert
- [ ] Kein PII in Logs

## 📚 Dokumentation
- [Spezifikation](./docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md)
- [Implementation Scaffold](./docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md)
- [Quick-Start Guide](./docs/features/BESTANDSKUNDEN_QUICK_START.md)

## 🎯 Success Criteria
- ✅ Bestandskunde registriert sich in < 2 Min
- ✅ 0 Duplikate durch ERP-Verknüpfung
- ✅ 95%+ ERP-Lookup-Erfolgsrate
- ✅ < 1% False Positives bei Duplikat-Erkennung
- ✅ 99%+ API-Verfügbarkeit
- ✅ Alle Security-Anforderungen erfüllt
- ✅ 80%+ Test-Coverage

## 🔗 Related Issues
- Duplikat-Prävention: #XXX
- ERP-Integration: #XXX
- Email-Verifikation: #XXX

## 👥 Assigned To
- Backend: @developer1
- Frontend: @developer2
- QA: @qa-engineer

## 📍 Labels
- `enhancement`
- `registration`
- `erp-integration`
- `p1-high`
- `backend`
- `frontend`

## 🏷️ Milestone
- Target: KW 2 2026 (3. Januar - 10. Januar)
- Duration: 3 Tage
- Effort: 40-48 Stunden (2 Entwickler parallel)

---

**Created**: 28. Dezember 2025  
**Status**: 🟢 Ready to Start  
**Priority**: P1 (High)
```

---

## Alternativ: Für bestehendes Issue

Falls du ein existierendes Issue haben solltest, kannst du diese Inhalte dort ergänzen/aktualisieren.

### Zu aktualisierende Felder:

**Title:**
```
Feature: Vereinfachte Registrierung für Bestandskunden
```

**Description:**
[Obige Markdown einfügen]

**Labels:**
- `enhancement`, `registration`, `erp-integration`, `p1-high`

**Assignees:**
- Backend Developer
- Frontend Developer

**Milestone:**
- KW 2 2026

**Project:**
- B2Connect (Board: In Progress → Development)
