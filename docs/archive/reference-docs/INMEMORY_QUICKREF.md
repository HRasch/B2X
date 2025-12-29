# ⚡ InMemory Database - Schnellreferenz

## Sofort Starten (empfohlen)

```
F5 → Wähle "Full Stack - InMemory 🚀" → Done! 🎉
```

## Verfügbare Konfigurationen

| Konfiguration | Backend | Frontend | DB |
|---------------|---------|----------|-----|
| **Full Stack (Aspire + Frontend) - InMemory 🚀** | ✅ | ✅ | InMemory |
| **Full Stack (Aspire + Admin Frontend) - InMemory 🚀** | ✅ | ✅ | InMemory |
| **Full Stack (All Services + Both Frontends) - InMemory 🚀** | ✅ | ✅✅ | InMemory |
| **🚀 Aspire AppHost (Orchestration) - InMemory** | ✅ | ❌ | InMemory |
| Catalog Service (Debug) - InMemory | ✅ | ❌ | InMemory |

## Alle Services (InMemory) - Ports

- API Gateway: http://localhost:15500
- Auth Service: http://localhost:9002
- Tenant Service: http://localhost:9003
- Catalog Service: http://localhost:9001
- Localization Service: http://localhost:9004
- Frontend: http://localhost:5173
- Admin Frontend: http://localhost:5174

## Umgebungsvariablen

```bash
# InMemory (Schnelle Entwicklung)
export Database__Provider=inmemory

# PostgreSQL (Production-like)
export Database__Provider=PostgreSQL
```

## Konfigurationsdateien

```
backend/services/LocalizationService/appsettings.Development.json
backend/services/auth-service/appsettings.Development.json
backend/services/tenant-service/appsettings.Development.json
```

## Probleme beheben

| Problem | Lösung |
|---------|--------|
| "Port in use" | VS Code neustarten oder alt service killen |
| "Provider not found" | appsettings.Development.json prüfen |
| "Datenbank leer" | Ist normal! InMemory ≠ persistent |

## Mehr Info

→ Siehe [VSCODE_INMEMORY_SETUP.md](./VSCODE_INMEMORY_SETUP.md)
