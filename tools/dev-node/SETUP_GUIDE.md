# Dev Node Setup - Step by Step Guide

Quick setup guide for the B2Connect GPU-accelerated dev node (RTX 5090, 64GB DDR5, Windows 11 DE).

---

## Part 1: Auf dem Windows PC (192.168.1.117)

### Schritt 1: OpenSSH Server aktivieren

1. Drücke **Win + I** um die Einstellungen zu öffnen
2. Gehe zu **System** → **Optionale Features** (oder **Apps** → **Optionale Features**)
3. Klicke auf **Features anzeigen** (oben bei "Optionales Feature hinzufügen")
4. Suche nach **"OpenSSH-Server"**
5. Haken setzen und **Weiter** → **Installieren** klicken
6. Warte bis die Installation abgeschlossen ist

**SSH-Dienst starten:**
1. Drücke **Win + R**, tippe `services.msc`, drücke Enter
2. Finde **"OpenSSH SSH Server"** in der Liste
3. Rechtsklick → **Eigenschaften**
4. Setze **Starttyp**: `Automatisch`
5. Klicke auf **Starten**
6. Klicke **OK**

**Prüfen ob SSH läuft (PowerShell als Admin):**
```powershell
Get-Service sshd
# Sollte zeigen: Status = Running
```

---

### Schritt 2: Firewall-Ports öffnen

**PowerShell als Administrator öffnen** (Rechtsklick auf Start → "Terminal (Administrator)"):

```powershell
# SSH erlauben (Port 22)
New-NetFirewallRule -DisplayName "SSH" -Direction Inbound -Protocol TCP -LocalPort 22 -Action Allow

# Ollama API erlauben (Port 11434)
New-NetFirewallRule -DisplayName "Ollama" -Direction Inbound -Protocol TCP -LocalPort 11434 -Action Allow
```

---

### Schritt 3: Docker Desktop installieren

1. Docker Desktop herunterladen: https://www.docker.com/products/docker-desktop/
2. Installer ausführen
3. **Wichtig:** Bei der Installation **"WSL 2 anstelle von Hyper-V verwenden"** aktivieren
4. Windows neu starten wenn gefragt
5. Nach dem Neustart Docker Desktop öffnen und Setup abschließen

**Prüfen ob Docker funktioniert (PowerShell):**
```powershell
docker --version
docker ps
```

---

### Schritt 4: GPU-Unterstützung in Docker aktivieren (WSL2)

1. Öffne **Docker Desktop** → **Einstellungen** (Zahnrad-Symbol)
2. Gehe zu **Ressourcen** → **WSL-Integration**
3. Aktiviere die Integration mit deiner WSL2-Distribution (z.B. Ubuntu)
4. Klicke **Anwenden & Neu starten**

**NVIDIA Container Toolkit in WSL2 installieren:**

Öffne **WSL2 Terminal** (Suche "Ubuntu" im Startmenü):

```bash
# NVIDIA Repository hinzufügen
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -s -L https://nvidia.github.io/libnvidia-container/gpgkey | sudo apt-key add -
curl -s -L https://nvidia.github.io/libnvidia-container/$distribution/libnvidia-container.list | \
    sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list

# Toolkit installieren
sudo apt-get update
sudo apt-get install -y nvidia-container-toolkit

# Docker konfigurieren
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker
```

**GPU-Zugriff testen (in WSL2):**
```bash
docker run --rm --gpus all nvidia/cuda:12.4.0-base-ubuntu22.04 nvidia-smi
```

Du solltest deine RTX 5090 in der Ausgabe sehen!

---

### Schritt 5: Dev Node Verzeichnis erstellen

**In PowerShell:**
```powershell
mkdir $HOME\b2connect-dev-node
cd $HOME\b2connect-dev-node
```

Merke dir den Pfad (z.B. `C:\Users\Holger\b2connect-dev-node`)

---

## Part 2: Auf deinem Mac (Entwicklungsrechner)

### Schritt 6: Verbindung konfigurieren

```bash
cd /Users/holger/Documents/Projekte/B2Connect

# Konfiguration starten
python3 scripts/dev-node.py configure
```

Eingaben bei den Prompts:
- **IP**: `192.168.1.117`
- **Ollama port**: `11434` (Standard)
- **SSH port**: `22` (Standard)
- **SSH user**: Dein Windows-Benutzername (z.B. `holger`)

---

### Schritt 7: SSH-Verbindung testen

```bash
# SSH manuell testen
ssh holger@192.168.1.117

# Bei Fingerprint-Frage 'yes' eingeben
# Windows-Passwort eingeben
```

Wenn du eine Eingabeaufforderung siehst, funktioniert SSH! Tippe `exit` zum Trennen.

---

### Schritt 8: Docker Stack auf Windows PC deployen

```bash
python3 scripts/dev-node.py deploy
```

Dies kopiert die Docker-Dateien auf deinen Windows PC.

---

## Part 3: Zurück auf dem Windows PC

### Schritt 9: Docker Stack bauen und starten

**PowerShell öffnen** und zum Dev Node Ordner navigieren:

```powershell
cd $HOME\b2connect-dev-node

# Docker Image bauen
docker compose build

# Stack starten
docker compose up -d

# Prüfen ob es läuft
docker compose ps
```

Du solltest `b2connect-ollama` als "running" sehen.

---

### Schritt 10: KI-Modelle herunterladen

```powershell
# Standard-Modell herunterladen (~4GB)
docker compose exec ollama ollama pull llama3.2

# Optional: Code-fokussiertes Modell
docker compose exec ollama ollama pull codellama
```

Warte bis der Download abgeschlossen ist (kann einige Minuten dauern).

---

## Part 4: Test vom Mac

### Schritt 11: Alles überprüfen

```bash
cd /Users/holger/Documents/Projekte/B2Connect

# Status prüfen
python3 scripts/dev-node.py status
```

**Erwartete Ausgabe:**
```
📡 Network connectivity... ✅ Reachable
🤖 Ollama (port 11434)... ✅ Running (2 models)
🔐 SSH (port 22)... ✅ Open
```

---

### Schritt 12: Ersten KI-Prompt ausführen

```bash
# Verfügbare Modelle auflisten
python3 scripts/dev-node.py ollama list

# Test-Prompt ausführen
python3 scripts/dev-node.py ollama run -m llama3.2 -p "Write a C# function that adds two numbers"

# Benchmark ausführen
python3 scripts/dev-node.py benchmark
```

🎉 **Setup abgeschlossen!**

---

## Kurzreferenz

### Vom Mac (nach Setup):

```bash
# Status prüfen
python3 scripts/dev-node.py status

# KI-Prompts
python3 scripts/dev-node.py ollama run -m llama3.2 -p "Dein Prompt hier"
python3 scripts/dev-node.py ollama run -m codellama -p "Review this code"

# Neue Modelle herunterladen
python3 scripts/dev-node.py ollama pull -m mistral

# Docker-Verwaltung
python3 scripts/dev-node.py docker ps      # Container auflisten
python3 scripts/dev-node.py docker logs    # Logs anzeigen
python3 scripts/dev-node.py docker stop    # Stack stoppen
python3 scripts/dev-node.py docker start   # Stack starten
```

### Vom Windows PC (PowerShell):

```powershell
cd $HOME\b2connect-dev-node

docker compose up -d      # Starten
docker compose down       # Stoppen
docker compose logs -f    # Logs verfolgen
docker compose exec ollama ollama list    # Modelle auflisten
docker compose exec ollama ollama pull mistral  # Modell herunterladen
```

---

## Problembehandlung

### "Connection refused" bei SSH
- Prüfe ob Windows-Firewall Port 22 erlaubt
- Prüfe ob OpenSSH Server läuft (PowerShell): `Get-Service sshd`
- Stelle sicher dass Starttyp auf "Automatisch" steht

### "Connection refused" bei Ollama
- Prüfe ob Container läuft: `docker compose ps`
- Prüfe ob Firewall Port 11434 erlaubt
- Prüfe Ollama Logs: `docker compose logs ollama`

### GPU wird in Docker nicht erkannt
```powershell
# NVIDIA Treiber in Windows prüfen
nvidia-smi  # Sollte RTX 5090 zeigen

# Im WSL2 Terminal, Docker GPU-Zugriff testen:
docker run --rm --gpus all nvidia/cuda:12.4.0-base-ubuntu22.04 nvidia-smi
```

### Modelle laden nicht / Speicher voll
```powershell
# Verfügbaren Speicherplatz prüfen
docker system df

# Container-Speichernutzung prüfen
docker stats b2connect-ollama
```

### Docker-Befehle funktionieren nicht in PowerShell
Stelle sicher dass Docker Desktop läuft (prüfe das Symbol im System-Tray / Infobereich).

---

## Optional: Web UI aktivieren

Bearbeite `$HOME\b2connect-dev-node\docker-compose.yml` auf dem Windows PC:
1. Entferne die Kommentare beim `webui` Service-Abschnitt
2. Entferne die Kommentare bei `webui-data` Volume
3. Neu starten: `docker compose up -d`
4. Zugriff unter: http://192.168.1.117:3000

---

**Dein Dev Node ist jetzt bereit für GPU-beschleunigte KI-Entwicklung!** 🎉
