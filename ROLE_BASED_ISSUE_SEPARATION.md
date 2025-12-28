# 🎯 Role-Based GitHub Issue & Copilot Context Management

**Status:** Ready to implement  
**Last Updated:** 28. Dezember 2025  
**Purpose:** Minimiere Copilot-Context durch rollenspezifische Issue-Filterung

---

## 📋 Überblick

Anstatt alle 40 Issues im Context zu haben, lädt jede Rolle nur **ihre relevanten Issues**:

| Rolle | Max Context | Issues | Focus |
|-------|-------------|--------|-------|
| 💻 Backend | 3,500 tokens | 18 | APIs, Database, Business Logic |
| 🎨 Frontend | 2,500 tokens | 8 | Vue.js, CSS, UI/UX |
| 🔐 Security | 2,000 tokens | 9 | Encryption, Audit, Compliance |
| ⚙️ DevOps | 2,000 tokens | 5 | Infrastructure, CI/CD |
| 🧪 QA | 2,500 tokens | 3 | Testing, Automation |
| 📋 Product | 1,500 tokens | 5 | Documentation, Requirements |

**Gesamter Master-Context: 15,000 tokens**

---

## 🚀 Quickstart

### 1. Setup einmalig ausführen
```bash
cd /Users/holger/Documents/Projekte/B2Connect
./scripts/setup-copilot-context-per-role.sh
```

Dies erstellt:
- ✅ Role-basierte GitHub Labels
- ✅ Git hooks für automatisches Branch-Context-Switching
- ✅ VS Code Copilot-Einstellungen
- ✅ Kontext-Vorlagen pro Rolle

### 2. Zeige Issues pro Rolle
```bash
# Nur Backend Issues
./scripts/role-based-issue-filter.sh backend

# Nur Frontend Issues
./scripts/role-based-issue-filter.sh frontend

# Alle Rollen (für Überblick)
./scripts/role-based-issue-filter.sh all
```

### 3. Branch-Naming folgen
```bash
# Backend Feature
git checkout -b feature/backend-vat-calculation

# Frontend Feature
git checkout -b feature/frontend-price-display

# Security Feature
git checkout -b feature/security-audit-logging

# Automatisch: Git hook erkennt Role und lädt Context!
```

---

## 📊 Issue-Filterung nach Rolle

### **Backend Developer** (18 Issues)

**Labels:** `backend`, `api`, `database`, `P0.6`, `P0.7`, `P0.9`

**Sprint 1 - P0 (CRITICAL):**
- #30: B2C Price Transparency (PAngV)
- #31: B2B VAT-ID Validation
- #12: Registration Endpoints
- #5-11: Auth Flow (7 issues)

**Sprint 2-4 (Feature Development):**
- #20-28: Catalog, Orders, Invoices, Webhooks

**Command:** `./scripts/role-based-issue-filter.sh backend`

---

### **Frontend Developer** (8 Issues)

**Labels:** `frontend`, `ui`, `accessibility`, `P0.6`, `P0.8`

**Sprint 1-3 - P0.6 (E-Commerce UI):**
- #33: Price Display (Brutto inkl. MwSt)
- #40: Shipping Cost Transparency
- #41: Terms & Conditions Checkbox
- #42: Impressum & Datenschutz Links

**Sprint 2-4 (Admin & Theming):**
- #15-19: Admin Dashboard, Theme Builder

**Command:** `./scripts/role-based-issue-filter.sh frontend`

---

### **Security Engineer** (9 Issues)

**Labels:** `security`, `compliance`, `encryption`, `audit`, `P0.1-P0.7`

**P0 Components (Priority):**
- P0.1: Audit Logging
- P0.2: Encryption at Rest
- P0.3: Incident Response (< 24h)
- P0.5: Key Management
- P0.7: AI Act Risk Assessment

**Command:** `./scripts/role-based-issue-filter.sh security`

---

### **DevOps Engineer** (5 Issues)

**Labels:** `infrastructure`, `devops`, `ci-cd`, `P0.3-P0.5`

**Focus:**
- Network Segmentation (P0.4)
- Incident Response Monitoring (P0.3)
- Key Vault Setup (P0.5)
- CI/CD Pipelines
- Aspire Orchestration

**Command:** `./scripts/role-based-issue-filter.sh devops`

---

### **QA Engineer** (3 Issues + Cross-functional)

**Labels:** `testing`, `qa`, `P0.6`, `P0.7`, `P0.8`, `P0.9`

**Focus (52 Compliance Tests):**
- P0.6: E-Commerce Legal Tests (15)
- P0.7: AI Act Tests (15)
- P0.8: BITV Accessibility Tests (12)
- P0.9: E-Rechnung Tests (10)

**Command:** `./scripts/role-based-issue-filter.sh qa`

---

### **Product Owner** (5 Issues)

**Labels:** `documentation`, `epic`, `research`

**Focus:**
- Roadmap & Requirements
- Acceptance Criteria
- User Stories & Specs

**Command:** `./scripts/role-based-issue-filter.sh product`

---

## 🔧 Automatisches Context-Switching

Sobald `setup-copilot-context-per-role.sh` läuft:

### Branch checkout → Automatische Context-Umschaltung

```bash
# Git Hook erkennt: feature/backend-* → Lädt Backend Context
git checkout -b feature/backend-vat

# Git Hook erkennt: feature/frontend-* → Lädt Frontend Context
git checkout -b feature/frontend-checkout

# Git Hook erkennt: feature/security-* → Lädt Security Context
git checkout -b feature/security-encryption
```

**Keine manuellen Befehle nötig!** Copilot wechselt Kontext automatisch.

---

## 📝 VS Code Integration

Nach Setup in `.vscode/settings.json`:

```json
{
  "copilot.context": {
    "enable": true,
    "scope": "role-based",
    "roles": {
      "backend": {
        "maxTokens": 3500,
        "searchPaths": ["backend/Domain/**"]
      },
      "frontend": {
        "maxTokens": 2500,
        "searchPaths": ["frontend-store/**", "frontend-admin/**"]
      }
      // ... etc
    }
  }
}
```

---

## 🎯 Implementation Checklist

- [ ] Run `./scripts/setup-copilot-context-per-role.sh`
- [ ] Verify labels created: `gh label list`
- [ ] Test branch switching: `git checkout -b feature/backend-test`
- [ ] Verify Git hook executed: Check `.git/hooks/post-checkout`
- [ ] Test issue filtering: `./scripts/role-based-issue-filter.sh all`
- [ ] Set team branch naming conventions (see below)

---

## 📌 Branch Naming Conventions

Damit Git hooks funktionieren:

```
feature/backend-{description}       → Backend Context
feature/frontend-{description}      → Frontend Context
feature/security-{description}      → Security Context
feature/devops-{description}        → DevOps Context
feature/test-{description}          → QA Context
chore/docs-{description}            → Product Context

fix/backend-{description}
fix/frontend-{description}
etc...
```

**Examples:**
```bash
feature/backend-vat-calculation
feature/frontend-price-display
feature/security-audit-logging
feature/devops-ci-cd-pipeline
feature/test-compliance-automation
chore/docs-roadmap-update
```

---

## 💡 Benefits

✅ **Kleinerer Context** (3-3.5K tokens statt 15K)  
✅ **Schnellere Copilot-Responses** (weniger Kontext zu durchsuchen)  
✅ **Weniger Token-Verbrauch** (kostengünstiger)  
✅ **Rollenspezifisches Wissen** (nur relevante Issues)  
✅ **Automatisches Switching** (keine manuellen Befehle)  
✅ **Bessere Fokussierung** (Copilot konzentriert sich auf relevante Code)  

---

## 📞 Verwendung

### Für Einzelne Frage
```bash
# Im Backend arbeiten
./scripts/role-based-issue-filter.sh backend

# Kopiere relevante Issue #20 in Copilot-Context
# Stelle Frage zu VAT-Berechnung
```

### Für Brainstorming
```bash
# Alle Rollen-Übersicht
./scripts/role-based-issue-filter.sh all

# Sehe gesamten Scope aller Issues
# Identifiziere Abhängigkeiten zwischen Rollen
```

### Für Sprint Planning
```bash
# Frontend-Sprintplan
./scripts/role-based-issue-filter.sh frontend

# Kopiere alle Sprint-1 Issues in Roadmap
# Priorisiere nach Dependencies
```

---

## 🔄 Wartung

**Wöchentlich:**
```bash
# Öffne neue Issues in richtige Label-Kategorie
# Labels beim Issue-Erstellen zuordnen
# GitHub Labels checken: gh label list
```

**Monatlich:**
```bash
# Alte abgeschlossene Issues archivieren
# Context-Größe überprüfen
# Branch-Naming Konventionen überprüfen
```

---

## 📖 Weiterführende Dokumentation

Spezifische Rollen-Guides:
- [🎨 Frontend Developer](../docs/by-role/FRONTEND_DEVELOPER.md)
- [💻 Backend Developer](../docs/by-role/BACKEND_DEVELOPER.md)
- [🔐 Security Engineer](../docs/by-role/SECURITY_ENGINEER.md)
- [⚙️ DevOps Engineer](../docs/by-role/DEVOPS_ENGINEER.md)
- [🧪 QA Engineer](../docs/by-role/QA_ENGINEER.md)

---

**Document Owner:** Architecture Team  
**Status:** Ready for Implementation  
**Last Updated:** 28. Dezember 2025
