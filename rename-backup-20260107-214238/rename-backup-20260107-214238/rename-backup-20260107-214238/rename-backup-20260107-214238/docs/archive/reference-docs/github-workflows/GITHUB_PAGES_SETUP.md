# GitHub Pages Setup & Deployment Guide

**Zielgruppe:** Development Team, DevOps Engineers  
**Status:** Setup Guide für Dokumentation als Website  
**Version:** 1.0  
**Aktualisiert:** 27. Dezember 2025

---

## 📋 Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Voraussetzungen](#voraussetzungen)
3. [Schritt-für-Schritt Setup](#schritt-für-schritt-setup)
4. [CI/CD Pipeline Setup](#cicd-pipeline-setup)
5. [Troubleshooting](#troubleshooting)

---

## 🎯 Übersicht

GitHub Pages verwandelt Markdown-Dateien in eine statische Website mit Jekyll.

**Architektur:**

```
docs/ Verzeichnis
    ├── _config.yml           (Jekyll Konfiguration)
    ├── index.md             (Homepage)
    ├── USER_GUIDE.md        (Benutzerdokumentation)
    ├── ADMIN_GUIDE.md       (Admin-Dokumentation)
    ├── FAQ.md               (Häufige Fragen)
    ├── _layouts/            (HTML Templates)
    ├── _includes/           (Wiederverwendbare Komponenten)
    ├── assets/
    │   ├── css/             (Styling)
    │   └── images/          (Bilder)
    └── _site/               (Generierte Website - Git ignorieren!)

GitHub Settings
    → Pages → Build from: docs/ on main branch
    → Custom domain: (optional) docs.B2X.com
    → HTTPS: Enabled (automatisch)

Website verfügbar unter:
    https://B2X.github.io
    oder
    https://docs.B2X.com (mit Custom Domain)
```

---

## ✅ Voraussetzungen

### 1. Lokale Entwicklung (Optional, für Preview)

**Ruby installieren:**
```bash
# macOS
brew install ruby

# Ubuntu/Debian
sudo apt-get install ruby-full

# Windows
# Download von: https://rubyinstaller.org/
```

**Jekyll installieren:**
```bash
gem install jekyll bundler
```

**Abhängigkeiten installieren:**
```bash
cd docs/
bundle install
```

### 2. Repository-Zugriff

```bash
# Stellen Sie sicher, dass Sie Push-Zugriff auf B2X haben
git remote -v
# Sollte zeigen:
# origin  https://github.com/B2X/B2X.git (fetch)
# origin  https://github.com/B2X/B2X.git (push)
```

### 3. Git-Grundlagen

```bash
# Aktuellen Branch überprüfen
git branch

# Main Branch auschecken
git checkout main
```

---

## 🚀 Schritt-für-Schritt Setup

### Schritt 1: Repository-Struktur prüfen

```bash
cd /Users/holger/Documents/Projekte/B2X

# Überprüfen Sie, dass docs/ Verzeichnis existiert
ls -la docs/

# Sollte enthalten:
# _config.yml
# index.md
# USER_GUIDE.md
# assets/
```

---

### Schritt 2: _config.yml erstellen (Falls nicht vorhanden)

**Bereits erstellt!** Datei: `/docs/_config.yml`

Überprüfen Sie die Einstellungen:
```yaml
title: B2X Documentation
theme: jekyll-theme-minimal
plugins:
  - jekyll-feed
  - jekyll-seo-tag
  - jekyll-sitemap
```

---

### Schritt 3: index.md Homepage erstellen

Falls nicht vorhanden, erstellen Sie:

```bash
cat > docs/index.md << 'EOF'
---
layout: default
title: B2X Documentation
---

# 📚 B2X Dokumentation

Willkommen zur offiziellen B2X Dokumentation!

## Für Wen?

### 👥 [Kundenguide](../USER_GUIDE.md#-für-kunden-store)
- Konto erstellen & verwalten
- Einkaufen & Bestellungen
- Support & FAQs

### ⚙️ [Admin-Guide](../USER_GUIDE.md#-für-admin-benutzer)
- Produkte & Bestellungen verwalten
- Kunden & Settings
- Reports & Analytics

## 🆘 Hilfe & Support

- **E-Mail:** support@B2X.com
- **Telefon:** +1-800-xxx-xxxx
- **Live Chat:** Mo-Fr 09:00-18:00 CET

---

**Letzte Aktualisierung:** 27. Dezember 2025
EOF
```

---

### Schritt 4: Markdown-Dateien in docs/ verschieben

Alle Dokumentationsdateien sollten im `docs/` Verzeichnis sein:

```bash
cd /Users/holger/Documents/Projekte/B2X

# Überprüfen Sie Dateien
ls docs/*.md

# Sollte zeigen:
# docs/index.md
# docs/USER_GUIDE.md
# docs/_config.yml
```

---

### Schritt 5: GitHub Repository Settings

**Öffnen Sie:**
```
https://github.com/B2X/B2X/settings/pages
```

**Konfigurieren Sie:**

1. **Source (Quelle):** 
   - ☑️ Deploy from branch (nicht Actions)
   - Branch: `main`
   - Folder: `/docs`

2. **Enforce HTTPS:**
   - ☑️ Ja (Empfohlen)

3. **Custom domain (Optional):**
   - Hostname: `docs.B2X.com`
   - Klicken Sie "Save"
   - DNS-Records konfigurieren (siehe unten)

**Screenshot:**
```
Settings → Pages
├─ Source: GitHub Actions (neu)
│  oder Deploy from branch (alt)
├─ Branch: main
├─ Folder: /docs
└─ Enforce HTTPS: ✓
```

---

### Schritt 6: DNS Configuration (Optional, für Custom Domain)

Falls Sie `docs.B2X.com` verwenden möchten:

**Bei Ihrem Domain-Provider (GoDaddy, Namecheap, etc.):**

```
Type: CNAME
Name: docs
Value: B2X.github.io
TTL: 3600
```

**oder mit GitHub's IP-Adressen:**

```
Type: A
Name: @
Values:
  185.199.108.153
  185.199.109.153
  185.199.110.153
  185.199.111.153
TTL: 3600
```

**Verifizierung:**
```bash
# Warten Sie 24 Stunden, dann:
nslookup docs.B2X.com
# Sollte zeigen: B2X.github.io
```

---

### Schritt 7: Git Changes hochladen

```bash
# In Ihr B2X Repository gehen
cd /Users/holger/Documents/Projekte/B2X

# Status überprüfen
git status

# Changes adden
git add docs/

# Commit
git commit -m "docs: Add user documentation and GitHub Pages setup

- Add comprehensive user guide (customers & admins)
- Add Jekyll configuration for GitHub Pages
- Add deployment instructions
- Configure GitHub Pages settings"

# Zu main pushen
git push origin main
```

---

### Schritt 8: GitHub Actions überprüfen

Nach dem Push prüfen Sie die **Actions**:

```
https://github.com/B2X/B2X/actions
```

**Sie sollten sehen:**
```
✅ pages build and deployment (Erfolgreich)
   └─ Deployment
      └─ github-pages (Deployed)
```

Falls es nicht funktioniert, überprüfen Sie Fehler:
```
Klicken Sie auf die fehlgeschlagene Action
→ Sehen Sie Fehlermeldungen
→ Beheben Sie die Fehler
→ Pushen Sie erneut
```

---

### Schritt 9: Website prüfen

**Website sollte jetzt verfügbar sein:**

```
https://B2X.github.io
```

**Was Sie sehen sollten:**
- Homepage mit Navigation
- Kundenguide Link
- Admin-Guide Link
- Support-Informationen

---

## 🔄 CI/CD Pipeline Setup

### Automatisches Deployment bei Dokumentationsänderungen

Erstellen Sie: `.github/workflows/docs-deploy.yml`

```yaml
name: Deploy Documentation

on:
  push:
    branches:
      - main
    paths:
      - 'docs/**'
      - '.github/workflows/docs-deploy.yml'

  pull_request:
    branches:
      - main
    paths:
      - 'docs/**'

permissions:
  contents: read
  pages: write
  id-token: write

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup Ruby
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: '3.2'
          bundler-cache: true

      - name: Build Jekyll
        run: |
          cd docs/
          bundle exec jekyll build

      - name: Upload Pages artifact
        uses: actions/upload-pages-artifact@v3
        with:
          path: 'docs/_site'

  deploy:
    if: github.ref == 'refs/heads/main'
    needs: build
    runs-on: ubuntu-latest
    
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}

    steps:
      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
```

**Was dies macht:**
1. Triggert bei Push zu `main` (nur wenn docs/ geändert)
2. Baut Jekyll Site
3. Deployed zu GitHub Pages
4. ✅ Dokumentation ist nach 2-3 Minuten live

---

### Lokale Preview (Optional)

```bash
cd docs/

# Lokalen Server starten
bundle exec jekyll serve

# Öffnen Sie
http://localhost:4000

# Änderungen werden automatisch aktualisiert (Live Reload)

# Beenden: Ctrl+C
```

---

## 🔧 Troubleshooting

### Problem 1: "404 Not Found" auf GitHub Pages

**Ursache:** Falscher Branch oder Folder

**Lösung:**
```
Settings → Pages
→ Überprüfen Sie Branch: main
→ Überprüfen Sie Folder: /docs
→ Klicken Sie "Save"
→ Warten Sie 2-3 Minuten
```

---

### Problem 2: Jekyll Build schlägt fehl

**Überprüfen Sie den Error in GitHub Actions:**

```
Actions → Letzte Action → Sehen Sie Fehler
```

**Häufige Fehler:**

```yaml
# ❌ Fehler: Unbekanntes Plugin
plugins:
  - jekyll-xyz

# ✅ Lösung: Verwenden Sie unterstützte Plugins
plugins:
  - jekyll-feed
  - jekyll-seo-tag
  - jekyll-sitemap
```

---

### Problem 3: Markdown wird nicht richtig angezeigt

**Überprüfen Sie:**

1. **YAML Front Matter:** Jede Datei sollte mit diesem beginnen:
```yaml
---
layout: default
title: Seitentitel
---

# Seiteninhalt
```

2. **Dateinamen:** Sollten Unterstriche oder Bindestriche verwenden:
```
✅ USER_GUIDE.md
✅ admin-guide.md
❌ USER GUIDE.md (Spaces nicht erlaubt)
```

3. **Relative Links:** Verwenden Sie `.md` Erweiterung:
```markdown
✅ [Link](../USER_GUIDE.md#section)
❌ [Link](USER_GUIDE)
```

---

### Problem 4: Custom Domain funktioniert nicht

**Überprüfen Sie DNS:**
```bash
# Überprüfen Sie DNS-Einträge
nslookup docs.B2X.com

# Sollte zeigen: B2X.github.io
```

**Falls nicht funktioniert:**

1. Warten Sie 24 Stunden (DNS Propagation)
2. Entfernen Sie Custom Domain in Settings
3. Setzen Sie es neu
4. Überprüfen Sie DNS-Records erneut

---

## 📊 Monitoring & Maintenance

### Website Performance überprüfen

```bash
# Überprüfen Sie Ladezeit
curl -w '@curl-format.txt' -o /dev/null -s https://B2X.github.io

# Überprüfen Sie Links (lokal)
cd docs/
bundle exec jekyll build
bundle exec htmlproofer ./_site/ --disable-external
```

### Sicherheits-Header überprüfen

```bash
curl -I https://B2X.github.io

# Sollte zeigen:
# Strict-Transport-Security: max-age=31536000
# X-Content-Type-Options: nosniff
# Content-Security-Policy: ...
```

### Regelmäßige Updates

```bash
# Wöchentlich: Dokumentation aktualisieren
# Monatlich: Jekyll & Plugins updaten
cd docs/
bundle update jekyll

# Commit
git add Gemfile.lock
git commit -m "chore: Update Jekyll dependencies"
git push origin main
```

---

## 📝 Best Practices

### 1. Dokumentation aktuell halten

```bash
# Erstellen Sie einen Eintrag im Repository
# Alle 2 Wochen: Dokumentation überprüfen & aktualisieren

# In DOCUMENTATION_POLICY.md:
- Every feature must have documentation
- Update docs BEFORE merging PRs
- Schedule monthly review
```

### 2. Versionierung

```markdown
---
layout: default
version: 1.0
last_updated: 2025-12-27
---

**Version:** 1.0  
**Letzte Aktualisierung:** 27. Dezember 2025
```

### 3. Interne Links

```markdown
# ✅ Korrekt
[Kundenguide](../USER_GUIDE.md#-für-kunden-store)

# ❌ Falsch
[Kundenguide](USER_GUIDE#customers)
```

### 4. Bilder einbinden

```bash
# Erstellen Sie assets/ Verzeichnis
mkdir -p docs/assets/images
cp screenshot.png docs/assets/images/

# In Markdown:
![Screenshot](assets/images/screenshot.png)
```

---

## 🎯 Nächste Schritte

1. ✅ **Schritt 7 ausführen:** Git Changes hochladen
2. ⏳ **Schritt 8 überprüfen:** GitHub Actions erfolgreich?
3. ⏳ **Schritt 9 prüfen:** Website online?
4. ⏳ **CI/CD Setup:** Automatisches Deployment konfigurieren

---

**Dokumentation Deployment Guide Version:** 1.0  
**Letzte Aktualisierung:** 27. Dezember 2025  
**Nächste Überarbeitung:** Q1 2026

Viel Spaß mit GitHub Pages! 🚀📚
