# B2Connect - Dokumentations-Index

> Übersicht aller Dokumentationen und wo man sie findet

## 🎯 Für Anfänger

**👉 START HIER:**
1. [README.md](README.md) - Projektübersicht (5 min)
2. [docs/DEVELOPER_GUIDE.md](docs/DEVELOPER_GUIDE.md) - Kompletter Entwickler-Guide (20 min)
3. [DEVELOPMENT.md](DEVELOPMENT.md) - Setup-Details (15 min)

---

## 📚 Dokumentations-Struktur

### Root-Level Dokumentation (Hauptdateien)

| Datei | Zweck | Lesedauer |
|-------|-------|-----------|
| **README.md** | Projektübersicht, Features, Tech-Stack | 10 min |
| **DEVELOPMENT.md** | Lokale Setup-Anleitung | 15 min |
| **GETTING_STARTED.md** | Quick Start & Aufgaben-Guides | 20 min |
| **BUSINESS_REQUIREMENTS.md** | Features & Roadmap | 20 min |
| **APPLICATION_SPECIFICATIONS.md** | Technische Spezifikationen | 30 min |
| **CODING_STANDARDS.md** | Code-Style & Best Practices | 15 min |
| **.copilot-specs.md** | GitHub Copilot Richtlinien | Variabel |

### Entwickler-Guide (Neu!)

**📍 Ort:** `docs/DEVELOPER_GUIDE.md`

Einheitliche Dokumentation mit:
- Quick Start (5 min)
- Projektstruktur
- Tech-Stack
- Backend-Entwicklung
- Frontend-Entwicklung
- DB & Services
- Häufige Aufgaben
- Troubleshooting

---

## 🔍 Nach Aufgabe suchen

### Ich will schnell starten
→ [docs/DEVELOPER_GUIDE.md - Quick Start](docs/DEVELOPER_GUIDE.md#-quick-start)

### Ich arbeite am Backend
→ [docs/DEVELOPER_GUIDE.md - Backend-Entwicklung](docs/DEVELOPER_GUIDE.md#-backend-entwicklung)

### Ich arbeite am Frontend
→ [docs/DEVELOPER_GUIDE.md - Frontend-Entwicklung](docs/DEVELOPER_GUIDE.md#-frontend-entwicklung)

### Ich habe Probleme
→ [docs/DEVELOPER_GUIDE.md - Troubleshooting](docs/DEVELOPER_GUIDE.md#-troubleshooting)

### Ich brauche die Architektur
→ `docs/archived/` (siehe unten)

---

## 📦 Archivierte Dokumentation

**📍 Ort:** `docs/archived/`

Die alte Dokumentation wurde archiviert. Dort findest du:

### Feature-Guides (Detailliert)
- `ASPIRE_*.md` - .NET Aspire Setup & Orchestration
- `CATALOG_*.md` - Produktkatalog-Implementation
- `ELASTICSEARCH_*.md` - Fulltext-Suche
- `LOCALIZATION_*.md` - Multi-Sprachen-Support
- `QUARTZ_*.md` - Job-Scheduling
- `EVENT_VALIDATION_*.md` - Event-Handling

### Implementation-Berichte
- `ADMIN_FRONTEND_*.md` - Admin-Panel Details
- `CQRS_IMPLEMENTATION_*.md` - Command-Query-Responsibility
- `AOP_*.md` - Aspect-Oriented Programming
- `ADMIN_CRUD_*.md` - CRUD-Operationen

### Testing & Qualität
- `TEST_EXECUTION_*.md` - Test-Reports
- `TESTS_COMPLETE_*.md` - Test-Status
- `COMPILE_ERRORS_*.md` - Fehler-Behebung

### Setup & Deployment
- `PORT_MANAGEMENT_*.md` - Port-Konfiguration
- `DATABASE_CONFIGURATION.md` - DB-Setup
- `VSCODE_CONFIGURATION.md` - VS Code Einrichtung

---

## 🚀 Shell-Skripte

**📍 Ort:** `scripts/`

Verfügbare Skripte:
```bash
./scripts/aspire-run.sh          # AppHost starten
./scripts/aspire-watch.sh        # AppHost mit Watch-Mode
./scripts/start-frontend.sh      # Frontend starten
./scripts/health-check.sh        # Health-Checks alle Services
./scripts/check-ports.sh         # Port-Status überprüfen
./scripts/start-all-services.sh  # Alle Services zusammen
./scripts/start-services-local.sh # Lokale Services starten
./scripts/stop-services-local.sh  # Services stoppen
```

---

## 📊 Schnelle Übersicht

```
B2Connect/
├── docs/
│   ├── DEVELOPER_GUIDE.md          ⭐ START HIER (Neuer Guide)
│   ├── archived/                    (Alte detaillierte Docs)
│   │   ├── ASPIRE_*.md
│   │   ├── CATALOG_*.md
│   │   ├── ELASTICSEARCH_*.md
│   │   └── ...mehr
│   └── architecture/                (Architektur-Guides)
│
├── scripts/                         (Shell-Skripte)
│   ├── aspire-run.sh
│   ├── start-frontend.sh
│   └── ...mehr
│
├── README.md                        (Übersicht)
├── DEVELOPMENT.md                   (Setup)
├── GETTING_STARTED.md               (Quick Start)
├── BUSINESS_REQUIREMENTS.md         (Features)
├── APPLICATION_SPECIFICATIONS.md    (Tech-Specs)
├── CODING_STANDARDS.md              (Code-Style)
└── .copilot-specs.md                (Copilot Guide)
```

---

## 💡 Pro-Tipps

### Schnelle Navigation
```bash
# Alle Dokumentationsdateien auflisten
ls -la docs/

# Alte Dokumentation durchsuchen
grep -r "keyword" docs/archived/

# Skripte ausführbar machen
chmod +x scripts/*.sh
```

### Suche in Dokumentation
```bash
# In dieser Datei suchen
grep "Elasticsearch" docs/DEVELOPER_GUIDE.md

# In allen Dateien suchen
grep -r "Pinia" docs/
```

### Offline lesen
```bash
# README.md in Terminal anzeigen
cat README.md | less
```

---

## 📝 Dokumentation aktualisieren

Wenn du Änderungen machst:

1. **Kleine Änderungen:** UPDATE DEVELOPER_GUIDE.md
   ```bash
   # Öffne docs/DEVELOPER_GUIDE.md
   # Bearbeite die relevante Sektion
   ```

2. **Neue Feature:** Erstelle neue Datei in `docs/`
   ```markdown
   # Feature Name
   
   [Deine Dokumentation hier]
   
   ---
   **Letzte Aktualisierung:** [Datum]
   ```

3. **Archivierung:** Alte Dateien gehen in `docs/archived/`

---

## ❓ FAQ

**F: Wo finde ich die Quick Start?**
A: → [docs/DEVELOPER_GUIDE.md](docs/DEVELOPER_GUIDE.md#-quick-start)

**F: Wie starte ich alle Services?**
A: → `./scripts/start-all-services.sh` oder [DEVELOPMENT.md](DEVELOPMENT.md)

**F: Wo ist die alte Dokumentation?**
A: → `docs/archived/`

**F: Wie aktualisiere ich die Dokumentation?**
A: → Bearbeite `docs/DEVELOPER_GUIDE.md` oder erstelle neue Dateien in `docs/`

---

**Letzte Aktualisierung:** 26. Dezember 2025

*Für Fragen oder Feedback: Schau in die relevante Dokumentation oder frag im Team!*
