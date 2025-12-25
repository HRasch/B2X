# B2Connect Internationalization (i18n) Specification

**Version**: 1.0  
**Date**: 25. Dezember 2025  
**Status**: ✅ Approved  

---

## Executive Summary

B2Connect jetzt vollständig mehrsprachig. Die Anwendung unterstützt 8 Sprachen mit:
- **Backend**: Database-gestützte Localization mit EF Core
- **Frontend**: vue-i18n für Vue 3 Composition API
- **Middleware**: Automatische Spracherkennung
- **Tenant-Override**: Lokalisierungen pro Tenant anpassbar

---

## 📋 Implementierungs-Roadmap

### Phase 1: Backend Setup (Priority: HIGH)
- [ ] `LocalizationDbContext` mit `LocalizedString` Entity erstellen
- [ ] `ILocalizationService` + `LocalizationService` implementieren
- [ ] `LocalizationMiddleware` für Spracherkennung
- [ ] `LocalizationController` mit REST Endpoints
- [ ] `LocalizationSeeder` mit Basis-Translations
- [ ] Unit Tests für Service & Controller

**Timeline**: 2-3 Tage  
**Tests**: 20+ Unit Tests erforderlich

### Phase 2: Frontend Setup (Priority: HIGH)
- [ ] vue-i18n Installation & Konfiguration
- [ ] Basis-Translation JSON Dateien (en.json, de.json, etc.)
- [ ] `useLocale()` Composable erstellen
- [ ] `LanguageSwitcher` Komponente
- [ ] API Integration für Runtime-Translations
- [ ] Component Tests

**Timeline**: 2-3 Tage  
**Tests**: 15+ Component Tests erforderlich

### Phase 3: Service Integration (Priority: MEDIUM)
- [ ] Auth Service: Benutzer-Sprachpräferenz speichern
- [ ] Tenant Service: Tenant-spezifische Übersetzungen
- [ ] CMS: Layout/Theme Translations
- [ ] API Responses mit lokalisierten Messages

**Timeline**: 3-4 Tage

### Phase 4: Content Localization (Priority: MEDIUM)
- [ ] CMS Pages mehrsprachig unterstützen
- [ ] Themes mehrsprachig konfigurierbar
- [ ] Frontend Builder: Translations im UI
- [ ] Admin Panel für Translations

**Timeline**: 4-5 Tage

---

## 🌍 Unterstützte Sprachen

| Code | Sprache | Markierungszeichen | Status |
|------|---------|-------------------|--------|
| `en` | English | 🇬🇧 | ✅ Default |
| `de` | Deutsch | 🇩🇪 | ✅ Primary |
| `fr` | Français | 🇫🇷 | ✅ Supported |
| `es` | Español | 🇪🇸 | ✅ Supported |
| `it` | Italiano | 🇮🇹 | ✅ Supported |
| `pt` | Português | 🇵🇹 | ✅ Supported |
| `nl` | Nederlands | 🇳🇱 | ✅ Supported |
| `pl` | Polski | 🇵🇱 | ✅ Supported |

**Expandierbar**: Neue Sprachen einfach in Konfiguration hinzufügen.

---

## 🏗️ Architektur-Übersicht

### Backend (C# / .NET 8)

```
LocalizationService/
├── Models/
│   └── LocalizedString.cs          # Translation Storage Model
├── Services/
│   ├── ILocalizationService.cs      # Contract
│   └── LocalizationService.cs       # Implementation
├── Controllers/
│   └── LocalizationController.cs    # REST Endpoints
├── Middleware/
│   └── LocalizationMiddleware.cs    # Language Detection
├── Data/
│   ├── LocalizationDbContext.cs     # EF Core DbContext
│   └── LocalizationSeeder.cs        # Basis-Daten
└── Tests/
    ├── LocalizationServiceTests.cs  # Service Tests
    └── LocalizationControllerTests.cs # API Tests
```

### Frontend (Vue 3 / TypeScript)

```
frontend/src/
├── locales/
│   ├── index.ts                    # i18n Setup
│   ├── en.json                     # English Translations
│   ├── de.json                     # Deutsch Translations
│   └── [...]                       # Weitere Sprachen
├── composables/
│   └── useLocale.ts               # Language Switcher Logic
├── components/
│   └── LanguageSwitcher.vue       # UI Language Selector
├── services/
│   └── localizationApi.ts         # API Client
└── tests/
    └── i18n.spec.ts              # i18n Tests
```

---

## 🔧 Konfiguration

### Backend: appsettings.json
```json
{
  "Localization": {
    "DefaultLanguage": "en",
    "SupportedLanguages": ["en", "de", "fr", "es", "it", "pt", "nl", "pl"],
    "CacheDuration": 3600,
    "EnableTenantOverrides": true
  },
  "ConnectionStrings": {
    "LocalizationDb": "Provider=PostgreSQL;Host=localhost;Database=b2connect_i18n;..."
  }
}
```

### Frontend: .env.local
```bash
VITE_DEFAULT_LOCALE=en
VITE_SUPPORTED_LOCALES=en,de,fr,es,it,pt,nl,pl
VITE_API_URL=http://localhost:5000/api
```

---

## 🔌 API Endpoints

### Get Translation
```
GET /api/localization/{category}/{key}?language=de
Response: { "key": "auth.login", "value": "Anmelden", "language": "de" }
```

### Get Category Translations
```
GET /api/localization/category/auth?language=de
Response: { "category": "auth", "language": "de", "translations": {...} }
```

### Get Supported Languages
```
GET /api/localization/languages
Response: { "languages": ["en", "de", "fr", ...] }
```

### Set Translation (Admin)
```
POST /api/localization/auth/login
Authorization: Bearer {token}
Body: { "en": "Login", "de": "Anmelden", "fr": "Connexion" }
```

---

## 📊 Translation Kategorien

| Kategorie | Verwendung | Beispiele |
|-----------|-----------|----------|
| `auth` | Authentication | login, logout, register |
| `ui` | User Interface | save, cancel, delete, next |
| `errors` | Error Messages | required, invalid_email, unauthorized |
| `validation` | Form Validation | min_length, max_length, pattern |
| `cms` | CMS Feature | page, layout, theme, component |
| `tenant` | Tenant-specific | welcome, branding, custom |
| `email` | Email Templates | welcome_subject, reset_link |
| `common` | Common Strings | loading, success, warning |

---

## 🧪 Test-Strategie

### Backend Tests
```csharp
// LocalizationServiceTests.cs
- GetStringAsync returns correct translation ✓
- GetStringAsync falls back to English ✓
- GetCategoryAsync returns all translations ✓
- SetStringAsync updates translations ✓
- Tenant overrides take precedence ✓
- Caching works correctly ✓
```

### Frontend Tests
```typescript
// i18n.spec.ts
- Locale switches correctly ✓
- Missing translations fall back ✓
- Component uses correct locale ✓
- Language switcher updates store ✓
- localStorage persists selection ✓
```

---

## 🚀 Erste Schritte (für Entwickler)

### 1. Backend Setup
```bash
# 1. LocalizationDbContext hinzufügen
cd backend/services
mkdir -p LocalizationService/{Models,Services,Data,Controllers}

# 2. Dependencies installieren
dotnet add package Microsoft.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.Extensions.Localization

# 3. Service registrieren (Program.cs)
builder.Services.AddDbContext<LocalizationDbContext>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
app.UseMiddleware<LocalizationMiddleware>();

# 4. Migration erstellen
dotnet ef migrations add InitialLocalization
dotnet ef database update
```

### 2. Frontend Setup
```bash
# 1. vue-i18n installieren
cd frontend
npm install vue-i18n

# 2. Locales erstellen
mkdir src/locales
touch src/locales/{en.json,de.json,fr.json}

# 3. main.ts aktualisieren
import i18n from './locales'
app.use(i18n)

# 4. Component nutzen
const { t, locale } = useI18n()
```

### 3. Translations hinzufügen
```json
// locales/en.json
{
  "auth": {
    "login": "Login",
    "register": "Register"
  }
}

// locales/de.json
{
  "auth": {
    "login": "Anmelden",
    "register": "Registrieren"
  }
}
```

---

## 🔒 Security Considerations

- ✅ Nur Admins können Translations ändern (`[Authorize(Roles = "Admin")]`)
- ✅ SQL Injection Protection durch EF Core Parameter
- ✅ XSS Protection: Translations werden nicht als HTML interpretiert
- ✅ Tenant Isolation: Tenant-spezifische Overrides per `TenantId`
- ✅ Cache Invalidation bei Änderungen
- ✅ Rate Limiting auf `/api/localization` Endpoints empfohlen

---

## 📈 Performance Considerations

- **Caching**: 1 Stunde TTL für Translations in Memory
- **Database**: Indexed auf (Key, Category, TenantId)
- **Frontend**: JSON Dateien mit Vite optimiert (Tree-shaking)
- **Lazy Loading**: Languages nur bei Bedarf laden
- **CDN**: Statische JSON Dateien via CDN servieren

---

## 🔄 Verwandte Dokumentationen

- [.copilot-specs.md](.copilot-specs.md) - Section 18: i18n Guidelines
- [CMS_OVERVIEW.md](CMS_OVERVIEW.md) - CMS Localization
- [DATABASE_CONFIGURATION.md](DATABASE_CONFIGURATION.md) - LocalizationDbContext Setup
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development Setup

---

## ✅ Checklist für Implementierung

### Backend
- [ ] LocalizationDbContext erstellen
- [ ] ILocalizationService + Impl. schreiben
- [ ] LocalizationMiddleware implementieren
- [ ] LocalizationController erstellen
- [ ] LocalizationSeeder schreiben
- [ ] 20+ Unit Tests
- [ ] Integration mit Auth Service
- [ ] API Dokumentation (OpenAPI)

### Frontend
- [ ] vue-i18n konfigurieren
- [ ] Translation JSON Dateien
- [ ] useLocale() Composable
- [ ] LanguageSwitcher Komponente
- [ ] API Integration
- [ ] 15+ Component Tests
- [ ] localStorage Persistierung
- [ ] Dark Mode berücksichtigen

### Testing
- [ ] Alle 8 Sprachen testen
- [ ] Fallback-Mechanismen
- [ ] Tenant Overrides
- [ ] Cache Invalidation
- [ ] Performance unter Last

---

## 📞 Support & Fragen

Für Fragen zur i18n Implementierung:
1. Siehe [.copilot-specs.md](.copilot-specs.md) Section 18
2. Prüfe bestehende Tests
3. Nutze bestehende Implementierungen als Referenz

**Stand**: 25.12.2025  
**Nächste Review**: Nach Phase 2 Implementierung
