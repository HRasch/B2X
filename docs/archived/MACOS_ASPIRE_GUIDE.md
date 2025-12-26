# macOS Aspire Setup Guide

## Das Problem: DCP auf macOS

Aspire benötigt **DCP (Docker Container Platform)**, das nicht automatisch mit Docker Desktop installiert wird. 

### Lösung: Native Service Orchestration

Wir haben zwei Ansätze:

## ✅ Option 1: Direktes Service Startup (Empfohlen für macOS)

Das ist die einfachste Methode für Entwicklung auf macOS:

```bash
./start-all-services.sh
```

Das startet:
- Auth Service (Port 9002)
- Tenant Service (Port 9003)
- Localization Service (Port 9004)

Alles läuft in separaten Processes. Logs sind in `/tmp/b2connect_*.log`

## 📋 Option 2: Aspire mit DCP installieren (Optional)

Falls du Aspire Dashboard verwenden möchtest:

```bash
# Installiere Aspire Workload
dotnet workload install aspire

# Oder update
dotnet workload update aspire

# Dann starte AppHost
cd backend/services/AppHost
dotnet run
```

### Auf macOS kann das Probleme geben - hier ist die Fix:

1. **Installiere DCP auf macOS:**
```bash
brew install aspire-dashboard
```

2. **Setze Environment-Variablen:**
```bash
export ASPIRE_SKIP_DASHBOARD=false
export ASPIRE_CLI_PATH="/path/to/dcp"
dotnet run
```

## 🚀 Schnellstart

### Alle Services + Frontends starten:

**Terminal 1 - Backend Services:**
```bash
./start-all-services.sh
```

**Terminal 2 - Customer Frontend:**
```bash
cd frontend
npm run dev
```

**Terminal 3 - Admin Frontend:**
```bash
cd frontend-admin
npm run dev -- --port 5174
```

## 🔍 Service Status prüfen

```bash
# Auth Service Health
curl http://localhost:9002/health

# Tenant Service Health
curl http://localhost:9003/health

# Localization Service Health
curl http://localhost:9004/health
```

## 📊 Laufende Services ansehen

```bash
# Alle B2Connect Prozesse
ps aux | grep dotnet | grep B2Connect

# Logs live ansehen
tail -f /tmp/b2connect_auth.log
tail -f /tmp/b2connect_tenant.log
tail -f /tmp/b2connect_localization.log
```

## 🛑 Services stoppen

Mit `start-all-services.sh`: Einfach `Ctrl+C` drücken

Oder manuell:
```bash
pkill -f "B2Connect.AuthService"
pkill -f "B2Connect.TenantService"
pkill -f "B2Connect.LocalizationService"
```

## 🔄 Docker mit Aspire (Alternative für Zukunft)

Falls später CatalogService + weitere Services hinzukommen, kann man zu Docker compose oder Aspire mit vollständigem DCP-Setup wechseln.

---

**Status:** ✅ Services laufen mit nativer Orchestration auf macOS
