# B2Connect Scripts

## 📋 Übersicht

Utility-Scripts für Entwicklung und Testen.

## 🛠️ Skripte

### `kill-all-services.sh`
Stoppt alle B2Connect Services und gibt Ports frei.

**Verwendung:**
```bash
./scripts/kill-all-services.sh
```

**Wann verwenden:**
- 🔴 **Port-Konflikte** - Wenn Services nicht mehr starten weil Ports belegt sind
- 🔄 **Aspire hängt** - Wenn das Aspire Dashboard nicht antwortet
- 🧹 **Cleanup vor Neustart** - Nach Crashes oder Force-Stops
- 🐛 **Debugging** - Wenn Sie sicherstellen wollen dass alle Prozesse weg sind

**Beispiel:**
```bash
# Port freigeben und erneut starten
./scripts/kill-all-services.sh
dotnet run --project backend/AppHost/B2Connect.AppHost.csproj
```

### `start-aspire.sh`
Startet Aspire mit Port-Cleanup.

### `check-ports.sh`
Prüft welche Services welche Ports belegen.

```bash
./scripts/check-ports.sh
```

**Output:**
```
=== B2Connect Service Port Status ===
Port 7002 (Auth): AVAILABLE / ACTIVE (PID: 1234)
Port 7003 (Tenant): AVAILABLE
Port 8080 (Admin Gateway): ACTIVE (PID: 5678)
...
```

## 🚀 Best Practices

1. **Aspire verwenden** - Für normale Entwicklung
   ```bash
   dotnet run --project backend/AppHost/B2Connect.AppHost.csproj
   ```

2. **Manueller Cleanup** - Nur wenn nötig
   ```bash
   ./scripts/kill-all-services.sh
   ```

3. **Port-Status checken** - Vor dem Starten
   ```bash
   ./scripts/check-ports.sh
   ```

## ⚙️ Automatisches Cleanup

Mit Aspire DCP wird Cleanup automatisch gehandhabt:
- ✅ Services werden beim Herunterfahren sauber beendet
- ✅ Ports werden freigegeben
- ✅ Keine manuellen Interventionen normalerweise nötig

Nur bei Edge-Cases (Crashes, Force-Stops) das manuelle Script verwenden.

## 📝 Fehlerbehebung

| Problem | Lösung |
|---------|--------|
| "Address already in use" | `./scripts/check-ports.sh` dann `./scripts/kill-all-services.sh` |
| Aspire Dashboard nicht erreichbar | Kill-Script ausführen und erneut starten |
| Service-Prozess hängt | `./scripts/kill-all-services.sh` |
| DCP-Controller blockiert | Force-kill über Neubau oder Rechner-Neustart |

## ❤️ Heartbeat-System (Produktion)

Das Heartbeat-System überwacht kontinuierlich die Gesundheit der Backend-Services und führt automatische Neustarts bei Fehlern durch.

### Setup

1. **Slack-Webhook konfigurieren:**
   - Erstelle einen Slack-Webhook in deinem Workspace
   - Setze die URL als Environment-Variable: `export SLACK_WEBHOOK_URL="https://hooks.slack.com/services/YOUR/WEBHOOK"`

2. **Systemd Service einrichten (empfohlen für Linux-Produktion):**
   ```bash
   # Service-Datei kopieren
   sudo cp scripts/b2connect-heartbeat.service /etc/systemd/system/
   sudo cp scripts/b2connect-heartbeat.timer /etc/systemd/system/

   # Pfade anpassen in der .service Datei:
   # - WorkingDirectory=/path/to/B2Connect
   # - ExecStart=/path/to/B2Connect/scripts/runtime-health-check.sh ...
   # - User=b2connect (oder entsprechender User)

   # Service aktivieren und starten
   sudo systemctl daemon-reload
   sudo systemctl enable b2connect-heartbeat.timer
   sudo systemctl start b2connect-heartbeat.timer
   ```

3. **Cron-Job Alternative (falls systemd nicht verfügbar):**
   ```bash
   # Cron-Job hinzufügen (zwei Einträge für 30s Intervall)
   crontab -e
   # Füge hinzu:
   * * * * * /path/to/B2Connect/scripts/runtime-health-check.sh --heartbeat --slack-webhook https://hooks.slack.com/services/YOUR/WEBHOOK
   * * * * * sleep 30; /path/to/B2Connect/scripts/runtime-health-check.sh --heartbeat --slack-webhook https://hooks.slack.com/services/YOUR/WEBHOOK
   ```

### Testen in Staging

1. **Staging-Umgebung starten:**
   ```bash
   # Services in Staging starten
   ./scripts/start-aspire.sh
   ```

2. **Heartbeat testen:**
   ```bash
   # Einzel-Check
   ./scripts/runtime-health-check.sh

   # Heartbeat-Modus (für Test)
   timeout 120 ./scripts/runtime-health-check.sh --heartbeat --slack-webhook YOUR_TEST_WEBHOOK
   ```

3. **Logs prüfen:**
   ```bash
   journalctl -u b2connect-heartbeat.service -f  # Für systemd
   # Oder Script-Output direkt
   ```

### Monitoring

- **Systemd:** `systemctl status b2connect-heartbeat.timer`
- **Logs:** `journalctl -u b2connect-heartbeat.service`
- **Slack-Alerts:** Bei Fehlern werden automatische Benachrichtigungen gesendet
