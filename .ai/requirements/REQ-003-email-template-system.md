# REQ-003: Email Template System

**DocID**: `REQ-003`  
**Status**: Draft  
**Created**: 2026-01-03  
**Owner**: @ProductOwner  
**Domain**: CMS / Notifications

---

## 1. Executive Summary

Das Email-Template-System ermöglicht Tenant-Admins und Content-Creators, individuelle Email-Vorlagen zu erstellen, zu bearbeiten und zu verwalten. Emails werden im Backend mit Liquid-Templates gerendert und asynchron über konfigurierbare Provider versendet.

---

## 2. Business Context

### 2.1 Problem Statement

- Aktuell keine Möglichkeit für Tenants, Email-Inhalte anzupassen
- Hardcoded Email-Templates sind nicht wartbar und nicht skalierbar
- Fehlende Personalisierung führt zu schlechter Customer Experience
- Keine Compliance-konforme Archivierung von versendeten Emails
- Keine Analytics über Email-Engagement

### 2.2 Business Value

| Value | Impact |
|-------|--------|
| **Conversion** | +15-25% durch personalisierte Emails |
| **Support-Reduktion** | -30% weniger "Wo ist meine Bestellung"-Anfragen |
| **Compliance** | GDPR-konforme Archivierung & Löschung |
| **Time-to-Market** | Content-Creator können ohne Dev-Support arbeiten |
| **Brand Consistency** | Einheitliches Look & Feel über alle Emails |

### 2.3 Target Users

| Role | Permissions | Primary Tasks |
|------|-------------|---------------|
| **Tenant-Admin** | Full Access | Provider Config, Accounts, Policies, Analytics |
| **Content-Creator** | Limited | Create/Edit Templates, Preview, Test Send |
| **Manager** | Read-only | View Statistics, Export Reports |

---

## 3. User Stories

### 3.1 Template Management

#### US-3.1.1: Template erstellen
**Als** Content-Creator  
**möchte ich** eine neue Email-Vorlage erstellen  
**damit** ich kundenspezifische Emails versenden kann.

**Akzeptanzkriterien:**
- [ ] Vorlage hat Name, Schlüssel (unique), Beschreibung
- [ ] Subject-Zeile unterstützt Liquid-Variablen
- [ ] HTML-Body unterstützt Liquid-Variablen
- [ ] Plain-Text-Body wird automatisch generiert (optional override)
- [ ] Vorlage wird als Draft gespeichert
- [ ] Vorlage kann in verschiedenen Sprachen erstellt werden

#### US-3.1.2: Template bearbeiten
**Als** Content-Creator  
**möchte ich** eine bestehende Email-Vorlage bearbeiten  
**damit** ich den Inhalt aktualisieren kann.

**Akzeptanzkriterien:**
- [ ] Bestehende Vorlage wird geladen
- [ ] Änderungen werden als neue Version gespeichert
- [ ] Alte Versionen bleiben erhalten (History)
- [ ] Live-Preview zeigt Änderungen in Echtzeit
- [ ] Änderungen können verworfen werden

#### US-3.1.3: Template Preview
**Als** Content-Creator  
**möchte ich** eine Email-Vorlage mit Beispieldaten vorschauen  
**damit** ich sehe, wie die Email beim Kunden aussieht.

**Akzeptanzkriterien:**
- [ ] Preview zeigt gerenderten Subject und Body
- [ ] Sample Data kann ausgewählt werden (Minimal, Full, etc.)
- [ ] Preview ist responsive (Desktop/Mobile Ansicht)
- [ ] Preview zeigt auch CC/BCC Empfänger
- [ ] Preview zeigt Anhänge

#### US-3.1.4: Test-Email versenden
**Als** Content-Creator  
**möchte ich** eine Test-Email an mich selbst versenden  
**damit** ich die Email in meinem Postfach prüfen kann.

**Akzeptanzkriterien:**
- [ ] Test-Email kann an beliebige Adresse gesendet werden
- [ ] Test-Email verwendet Sample Data
- [ ] Test-Email wird im Log markiert als "Test"
- [ ] Test-Email enthält alle konfigurierten Anhänge

---

### 3.2 Token/Variable Management

#### US-3.2.1: Variablen einfügen
**Als** Content-Creator  
**möchte ich** Variablen per Drag & Drop einfügen  
**damit** ich dynamische Inhalte ohne Coding einfügen kann.

**Akzeptanzkriterien:**
- [ ] Token-Palette zeigt alle verfügbaren Variablen
- [ ] Variablen sind nach Kategorien gruppiert (Order, Customer, Shipping, etc.)
- [ ] Variablen können per Drag & Drop eingefügt werden
- [ ] Variablen können per Klick eingefügt werden
- [ ] Variablen können per `{{` Autocomplete eingefügt werden
- [ ] Variablen im Subject werden genauso unterstützt

#### US-3.2.2: Variablen im Subject
**Als** Content-Creator  
**möchte ich** Variablen im Email-Betreff verwenden  
**damit** ich personalisierte Betreffzeilen erstellen kann.

**Akzeptanzkriterien:**
- [ ] Subject unterstützt alle Liquid-Variablen
- [ ] Subject-Länge wird angezeigt (Warnung bei >75 Zeichen)
- [ ] Subject-Preview zeigt gerenderten Betreff
- [ ] Subject-Beispiele werden angeboten

---

### 3.3 Layout & Styles

#### US-3.3.1: Layout erstellen
**Als** Tenant-Admin  
**möchte ich** ein wiederverwendbares Layout erstellen  
**damit** alle Emails ein einheitliches Design haben.

**Akzeptanzkriterien:**
- [ ] Layout enthält Header, Footer, Content-Bereich
- [ ] Logo kann konfiguriert werden
- [ ] Farben können angepasst werden (Primary, Secondary, etc.)
- [ ] Social Links können konfiguriert werden
- [ ] Legal Links (Privacy, Terms) können konfiguriert werden
- [ ] Layout kann als Default gesetzt werden

#### US-3.3.2: Styles definieren
**Als** Tenant-Admin  
**möchte ich** globale Styles definieren  
**damit** alle Emails konsistent aussehen.

**Akzeptanzkriterien:**
- [ ] Farben (Primary, Secondary, Background, Text)
- [ ] Typografie (Font Family, Sizes, Weights)
- [ ] Button Styles (Background, Border Radius, Padding)
- [ ] Spacing (Padding, Margins)
- [ ] Custom CSS für erweiterte Anpassungen

---

### 3.4 Recipient Management

#### US-3.4.1: CC/BCC konfigurieren
**Als** Tenant-Admin  
**möchte ich** CC/BCC-Empfänger konfigurieren  
**damit** interne Mitarbeiter automatisch informiert werden.

**Akzeptanzkriterien:**
- [ ] Internal Sales Rep (dynamisch aus Order Context)
- [ ] External Sales Rep (dynamisch aus Partner Context)
- [ ] Admin Notification (alle/einer/nach Rolle)
- [ ] Compliance Archive (BCC für Audit)
- [ ] Custom Recipients (statische Adressen)
- [ ] Conditional CC/BCC (nur bei Bedingung X)

---

### 3.5 Attachments

#### US-3.5.1: Static Attachments verwalten
**Als** Tenant-Admin  
**möchte ich** statische Anhänge (PDFs) hochladen  
**damit** diese an Emails angehängt werden können.

**Akzeptanzkriterien:**
- [ ] Upload von PDF-Dateien (max 10 MB pro Datei)
- [ ] Locale-spezifische Versionen (de-DE, en-US)
- [ ] Kategorien: Widerrufsbelehrung, AGB, Datenschutz, etc.
- [ ] Preview der Anhänge
- [ ] Versionierung (Replace existing)

#### US-3.5.2: Attachments an Template anhängen
**Als** Content-Creator  
**möchte ich** Anhänge zu einer Email-Vorlage zuordnen  
**damit** sie automatisch mitgesendet werden.

**Akzeptanzkriterien:**
- [ ] Auswahl aus verfügbaren Attachments
- [ ] Reihenfolge festlegen
- [ ] Conditional Attachments (nur wenn Bedingung X)
- [ ] Locale-Matching (Attachment Sprache = Email Sprache)
- [ ] Size Warning (wenn >25 MB gesamt)

---

### 3.6 Email Provider & Accounts

#### US-3.6.1: Email Provider konfigurieren
**Als** Tenant-Admin  
**möchte ich** einen Email-Provider konfigurieren  
**damit** Emails versendet werden können.

**Akzeptanzkriterien:**
- [ ] Provider: SendGrid, Mailjet, Custom SMTP
- [ ] API Key / Credentials (verschlüsselt gespeichert)
- [ ] Sender Identity (From Name, From Email, Reply-To)
- [ ] Test Connection Button
- [ ] Rate Limits & Quotas

#### US-3.6.2: Multiple Accounts
**Als** Tenant-Admin  
**möchte ich** mehrere Email-Accounts verwalten  
**damit** verschiedene Email-Typen über verschiedene Accounts gesendet werden.

**Akzeptanzkriterien:**
- [ ] Mehrere Accounts pro Tenant
- [ ] Routing Rules (Template X → Account Y)
- [ ] Fallback Account bei Fehler
- [ ] Quota Monitoring pro Account
- [ ] Health Status pro Account

---

### 3.7 Email Lifecycle

#### US-3.7.1: Email-Log einsehen
**Als** Tenant-Admin  
**möchte ich** versendete Emails einsehen  
**damit** ich den Versandstatus prüfen kann.

**Akzeptanzkriterien:**
- [ ] Liste aller versendeten Emails
- [ ] Filter nach Datum, Template, Status, Empfänger
- [ ] Details: Subject, Body, Recipients, Attachments
- [ ] Status: Sent, Delivered, Opened, Clicked, Bounced, Failed
- [ ] Re-send Button

#### US-3.7.2: Email erneut senden
**Als** Tenant-Admin  
**möchte ich** eine Email erneut versenden  
**damit** ich bei Problemen manuell eingreifen kann.

**Akzeptanzkriterien:**
- [ ] Re-send an gleiche Adresse
- [ ] Re-send an andere Adresse
- [ ] Option: Mit frischen Daten rendern
- [ ] Option: Mit anderem Account senden
- [ ] Option: Zeitgesteuert senden

#### US-3.7.3: Retention & Archivierung
**Als** Tenant-Admin  
**möchte ich** Aufbewahrungsrichtlinien konfigurieren  
**damit** Emails GDPR-konform gelöscht werden.

**Akzeptanzkriterien:**
- [ ] Retention Dauer pro Email-Typ (30/60/90/365 Tage)
- [ ] Auto-Archivierung in Cold Storage
- [ ] Auto-Löschung nach Ablauf
- [ ] Export vor Löschung möglich
- [ ] Audit Log für Löschvorgänge

---

### 3.8 Analytics

#### US-3.8.1: Email Statistics Dashboard
**Als** Manager  
**möchte ich** Email-Statistiken einsehen  
**damit** ich die Performance messen kann.

**Akzeptanzkriterien:**
- [ ] Sent, Delivered, Opened, Clicked, Bounced Metriken
- [ ] Trend Charts (7 Tage, 30 Tage, 90 Tage)
- [ ] Breakdown nach Template-Typ
- [ ] Breakdown nach Sprache
- [ ] Export als CSV/PDF

---

## 4. Non-Functional Requirements

### 4.1 Performance

| Metric | Target |
|--------|--------|
| Template Load Time | < 500ms |
| Preview Render Time | < 1s |
| Email Render Time (Backend) | < 2s |
| Email Send Time | < 5s |
| Throughput | 100 emails/minute pro Tenant |

### 4.2 Security

- [ ] API Keys werden verschlüsselt gespeichert (AES-256)
- [ ] Templates sind Tenant-isoliert
- [ ] Liquid-Engine ist sandboxed (keine Code Execution)
- [ ] Attachments werden auf Malware gescannt
- [ ] BCC-Adressen sind in Logs maskiert
- [ ] PII wird in Logs reduziert

### 4.3 Scalability

- [ ] Horizontal scalable Background Workers
- [ ] Template Caching (Redis)
- [ ] Attachment Storage (Azure Blob / S3)
- [ ] Database Partitioning nach TenantId

### 4.4 Reliability

- [ ] Retry-Mechanismus bei Provider-Fehlern (3 Versuche)
- [ ] Dead Letter Queue für permanente Fehler
- [ ] Failover zu Backup-Account
- [ ] Health Checks für Provider-Verbindung

### 4.5 Compliance

- [ ] GDPR: Löschung auf Anfrage
- [ ] GDPR: Data Export auf Anfrage
- [ ] Audit Trail für alle Änderungen
- [ ] Email-Archivierung für Compliance (7 Jahre für Rechnungen)

---

## 5. Data Model (High-Level)

```
EmailTemplate
├── Id (Guid)
├── TenantId (Guid)
├── TemplateKey (string, unique per tenant)
├── Locale (string)
├── Name (string)
├── Subject (string, Liquid)
├── HtmlBody (string, Liquid)
├── PlainTextBody (string, Liquid, optional)
├── LayoutId (Guid, optional)
├── Version (int)
├── Status (Draft, Published, Archived)
├── CreatedAt, UpdatedAt, CreatedBy, UpdatedBy

EmailLayout
├── Id (Guid)
├── TenantId (Guid)
├── Name (string)
├── Header (JSON: logo, tagline, colors)
├── Footer (JSON: contact, social, legal)
├── Styles (JSON: colors, typography, buttons)
├── CustomCss (string, optional)
├── IsDefault (bool)
├── Version (int)

EmailSendingAccount
├── Id (Guid)
├── TenantId (Guid)
├── Name (string)
├── Provider (SendGrid, Mailjet, SMTP)
├── Credentials (encrypted JSON)
├── FromName, FromEmail, ReplyTo
├── IsDefault (bool)
├── IsEnabled (bool)
├── MonthlyQuota, MonthlyUsed

EmailLog
├── Id (Guid)
├── TenantId (Guid)
├── TemplateKey (string)
├── Locale (string)
├── ToAddress, CcAddresses, BccAddresses
├── Subject, HtmlBody (rendered)
├── Context (JSON, for re-render)
├── AccountId (which account sent)
├── ExternalMessageId (provider ID)
├── Status (Sent, Delivered, Opened, Bounced, Failed)
├── AttachmentCount, SizeBytes
├── SentAt, DeliveredAt, OpenedAt
├── LifecycleState (Active, Archived, Deleted)

EmailAttachment
├── Id (Guid)
├── TenantId (Guid)
├── Name, DisplayName
├── Type (Static, Dynamic)
├── Versions (Dictionary<Locale, AttachmentVersion>)
├── UsedInTemplates (List<string>)

EmailEvent (for analytics)
├── Id (Guid)
├── EmailLogId (Guid)
├── EventType (Sent, Delivered, Opened, Clicked, Bounced)
├── Timestamp
├── Metadata (JSON: IP, UserAgent, Link)
```

---

## 6. API Endpoints (High-Level)

```
# Templates
GET    /api/admin/email/templates
POST   /api/admin/email/templates
GET    /api/admin/email/templates/{id}
PUT    /api/admin/email/templates/{id}
DELETE /api/admin/email/templates/{id}
POST   /api/admin/email/templates/{id}/preview
POST   /api/admin/email/templates/{id}/test-send
POST   /api/admin/email/templates/{id}/publish
GET    /api/admin/email/templates/{id}/versions

# Layouts
GET    /api/admin/email/layouts
POST   /api/admin/email/layouts
GET    /api/admin/email/layouts/{id}
PUT    /api/admin/email/layouts/{id}
DELETE /api/admin/email/layouts/{id}

# Accounts
GET    /api/admin/email/accounts
POST   /api/admin/email/accounts
GET    /api/admin/email/accounts/{id}
PUT    /api/admin/email/accounts/{id}
DELETE /api/admin/email/accounts/{id}
POST   /api/admin/email/accounts/{id}/test

# Attachments
GET    /api/admin/email/attachments
POST   /api/admin/email/attachments
GET    /api/admin/email/attachments/{id}
PUT    /api/admin/email/attachments/{id}
DELETE /api/admin/email/attachments/{id}
POST   /api/admin/email/attachments/{id}/upload/{locale}

# Logs
GET    /api/admin/email/logs
GET    /api/admin/email/logs/{id}
POST   /api/admin/email/logs/{id}/resend

# Analytics
GET    /api/admin/email/analytics/summary
GET    /api/admin/email/analytics/by-template
GET    /api/admin/email/analytics/by-locale

# Settings
GET    /api/admin/email/settings
PUT    /api/admin/email/settings
GET    /api/admin/email/retention-policy
PUT    /api/admin/email/retention-policy
```

---

## 7. Out of Scope (Future Phases)

- Newsletter/Marketing Campaign Management
- Email Drip Campaigns
- Segmentation & Targeting
- A/B Testing (Subject Lines) - Basic support in Phase 3
- Unsubscribe Management (Link only in Phase 1)
- Email Design Templates (pre-built themes)
- Integration mit External Marketing Tools (Mailchimp, etc.)

---

## 8. Dependencies

| Dependency | Status | Owner |
|------------|--------|-------|
| Tenant System | ✅ Exists | @Backend |
| User/Role System | ✅ Exists | @Backend |
| Blob Storage | ✅ Exists | @DevOps |
| Background Jobs (Wolverine) | ✅ Exists | @Backend |
| Admin Frontend (Vue 3) | ✅ Exists | @Frontend |
| Email Provider Account | ⏳ Required | @DevOps |

---

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Provider Rate Limits | High | Multiple accounts, rate limiting |
| Template Injection | Critical | Liquid sandbox, no C# execution |
| Large Attachments | Medium | Size limits, compression |
| Provider Outages | High | Failover to backup provider |
| Data Breach (PII) | Critical | Encryption, access controls |

---

## 10. Acceptance Criteria Summary

**Phase 1 (MVP):**
- [ ] Content-Creator kann Template erstellen/bearbeiten
- [ ] Templates unterstützen Liquid-Variablen
- [ ] Live-Preview mit Sample Data
- [ ] Email Provider kann konfiguriert werden
- [ ] Emails werden im Background gerendert und versendet
- [ ] Alle versendeten Emails werden geloggt

**Phase 2 (UX):**
- [ ] Drag & Drop Token-Insertion
- [ ] WYSIWYG Editor
- [ ] Layout System (Header, Footer, Styles)
- [ ] Test Mode & Sandbox

**Phase 3 (Advanced):**
- [ ] CC/BCC Configuration
- [ ] Multiple Accounts & Routing
- [ ] Attachments (Static & Dynamic)
- [ ] Retention & Archival
- [ ] Analytics Dashboard

---

**Agents**: @ProductOwner, @SARAH | Owner: @ProductOwner
