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
