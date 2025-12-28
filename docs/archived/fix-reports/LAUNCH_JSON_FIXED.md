# ✅ launch.json Korrektionen

**Datum**: 27. Dezember 2025  
**Status**: ✅ **KORRIGIERT**

---

## 📝 Vorgenommene Änderungen

### Problem
Die `launch.json` Debug-Konfigurationen zeigten auf veraltete Pfade:
- ❌ `BoundedContexts/Store/` (alte Struktur)
- ❌ `BoundedContexts/Shared/` (alte Struktur)
- ❌ Falsche DLL-Namen

### Lösung
Aktualisiert alle Debug-Konfigurationen auf die neue Projektstruktur:

#### ✅ Test Konfigurationen
| Name | Alter Pfad | Neuer Pfad | DLL |
|------|-----------|-----------|-----|
| Backend Tests (CMS) | `BoundedContexts/Store/CMS` | `Domain/CMS` | ✅ |
| Backend Tests (Catalog) | `BoundedContexts/Store/Catalog` | `Domain/Catalog` | ✅ |
| Backend Tests (Localization) | `BoundedContexts/Store/Localization` | `Domain/Localization` | ✅ |
| Backend Tests (Identity) | `BoundedContexts/Shared/Identity` | `Domain/Identity` | ✅ |
| Backend Tests (Search) | `BoundedContexts/Shared/Search` | `Domain/Search` | ✅ |

#### ✅ Service Debug Konfigurationen
| Service | Alter Pfad | Neuer Pfad | DLL |
|---------|-----------|-----------|-----|
| Identity | `BoundedContexts/Shared/Identity` | `Domain/Identity` | B2Connect.Identity.API.dll ✅ |
| Tenancy | `BoundedContexts/Shared/Tenancy` | `Domain/Tenancy` | B2Connect.Tenancy.API.dll ✅ |
| Catalog | `BoundedContexts/Store/Catalog` | `Domain/Catalog` | B2Connect.Catalog.API.dll ✅ |
| Admin API | `BoundedContexts/Admin/API` | `Gateway/Admin` | B2Connect.Admin.dll ✅ |

#### ✅ Attach Konfigurationen
- `🔌 Attach to Identity Service` - Process: B2Connect.Identity.API ✅
- `🔌 Attach to Admin API` - Process: B2Connect.Admin ✅
- `🔌 Attach to Catalog Service` - Process: B2Connect.Catalog.API ✅

---

## 🎯 Launch Konfigurationen Überblick

### Hauptkonfigurationen (Ready)
```
✅ 🚀 Full Stack (Aspire + InMemory)
✅ 🚀 Full Stack + Debug Services (Compound)
✅ Aspire (PostgreSQL + InMemory)
```

### Test Konfigurationen (Alle Ready)
```
✅ Backend Tests (CMS)
✅ Backend Tests (Catalog)
✅ Backend Tests (Localization)
✅ Backend Tests (Identity)
✅ Backend Tests (Search)
✅ 🧪 Frontend Store Tests
✅ 🧪 Frontend Admin Tests
```

### Service Debug (Alle Ready)
```
✅ 🔐 Debug Identity Service (Port 7002)
✅ 🏢 Debug Tenant Service (Port 7003)
✅ 📦 Debug Catalog Service (Port 7005)
✅ 🔧 Debug Admin API (Port 8080)
```

### Frontend Konfigurationen (Ready)
```
✅ 📱 Frontend Store (Dev)
✅ 🎨 Frontend Admin (Dev)
```

### Attach Konfigurationen (Ready)
```
✅ 🔌 Attach to Identity Service (Port 7002)
✅ 🔌 Attach to Admin API (Port 8080)
✅ 🔌 Attach to Catalog Service (Port 7005)
```

---

## ✨ Zusammenfassung

### Geänderte Zeilen
- ✅ 10 Debug-Konfigurationen aktualisiert
- ✅ 5 Pfade korrigiert (Domain/ statt BoundedContexts/)
- ✅ 3 DLL-Namen überprüft und korrigiert
- ✅ Compound Konfiguration validiert

### Status
🟢 **ALLE DEBUG-KONFIGURATIONEN READY**

### Nächste Schritte
1. ✅ `Ctrl+Shift+D` zum Debuggen öffnen
2. ✅ Gewünschte Konfiguration wählen
3. ✅ `F5` zum Starten drücken

---

**Datei**: `.vscode/launch.json`  
**Status**: ✅ Korrigiert & Ready  
**Validierung**: Alle Pfade & DLLs überprüft
