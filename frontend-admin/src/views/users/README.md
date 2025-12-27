# User Management Module

Umfangreiche Benutzerverwaltung für das Admin-Frontend mit Funktionen zum Anzeigen, Erstellen, Bearbeiten und Löschen von Benutzerkonten.

## 📁 Struktur

```
src/
├── views/users/
│   ├── UserList.vue          ← Benutzer-Übersicht mit Suchfilter
│   ├── UserForm.vue          ← Formular für Erstellen/Bearbeiten
│   └── UserDetail.vue        ← Detailseite mit Adressen-Verwaltung
├── stores/
│   └── users.ts              ← Pinia Store für State Management
├── services/api/
│   └── userService.ts        ← API Service für User-Operationen
└── types/
    └── user.ts               ← TypeScript Interfaces
```

## 🎯 Features

### UserList.vue
- ✅ Alle Benutzer anzeigen (mit Pagination)
- ✅ Nach E-Mail, Name, Telefon suchen
- ✅ Nach Status filtern (Aktiv/Inaktiv)
- ✅ Nach Name, Email oder Datum sortieren
- ✅ Benutzer anzeigen, bearbeiten, löschen
- ✅ Bestätigung vor dem Löschen

### UserForm.vue
- ✅ Benutzer erstellen (neue Konten)
- ✅ Benutzer bearbeiten (existierende Konten)
- ✅ Grundinformationen: Vorname, Nachname, E-Mail, Telefon
- ✅ Verifikationsstatus anpassen
- ✅ Profil-Erweiterungen: Unternehmen, Job, Bio
- ✅ Sprache & Zeitzone konfigurieren
- ✅ Newsletter-Einstellungen
- ✅ Validierung mit Fehlermeldungen

### UserDetail.vue
- ✅ Detailansicht mit allen Informationen
- ✅ E-Mail und Telefon Verifikationsstatus
- ✅ Beitrittsdatum & Letzter Login
- ✅ Adressen-Management (Liefer-/Rechnungsadressen)
- ✅ Direkt bearbeiten & löschen
- ✅ Tab-basierte Navigation

## 🚀 Verwendung

### Navigation

```typescript
// Benutzer-Übersicht
/users

// Neuen Benutzer erstellen
/users/create

// Benutzerdetails anzeigen
/users/:id

// Benutzer bearbeiten
/users/:id/edit
```

### Store verwenden

```typescript
import { useUserStore } from '@/stores/users'

export default {
  setup() {
    const userStore = useUserStore()

    // Alle Benutzer laden
    await userStore.fetchUsers(page, pageSize)

    // Einzelnen Benutzer laden
    await userStore.fetchUser(userId)

    // Benutzer erstellen
    await userStore.createUser({ firstName, lastName, email })

    // Benutzer aktualisieren
    await userStore.updateUser(userId, { firstName, lastName })

    // Benutzer löschen
    await userStore.deleteUser(userId)

    // Benutzer suchen
    await userStore.searchUsers(query)

    return { userStore }
  }
}
```

### API Service verwenden

```typescript
import { userService } from '@/services/api/userService'

// Alle Benutzer
const users = await userService.getUsers(page, pageSize)

// Benutzer nach ID
const user = await userService.getUserById(userId)

// Benutzer erstellen
const newUser = await userService.createUser(userData)

// Benutzer aktualisieren
const updated = await userService.updateUser(userId, userData)

// Benutzer löschen
await userService.deleteUser(userId)

// Profil abrufen
const profile = await userService.getUserProfile(userId)

// Profil aktualisieren
await userService.updateUserProfile(userId, profileData)

// Adressen abrufen
const addresses = await userService.getUserAddresses(userId)

// Adresse erstellen
const address = await userService.createAddress(userId, addressData)

// Adresse aktualisieren
await userService.updateAddress(userId, addressId, addressData)

// Adresse löschen
await userService.deleteAddress(userId, addressId)

// Benutzer suchen
const results = await userService.searchUsers(query)

// E-Mail verifizieren
await userService.verifyEmail(userId)

// Passwort zurücksetzen
await userService.resetPassword(userId, newPassword)
```

## 📋 TypeScript Types

```typescript
interface User {
  id: string
  tenantId: string
  email: string
  phoneNumber?: string
  firstName: string
  lastName: string
  isEmailVerified: boolean
  isPhoneVerified: boolean
  isActive: boolean
  createdAt: string
  updatedAt: string
  lastLoginAt?: string
  createdBy?: string
  updatedBy?: string
}

interface UserProfile {
  id: string
  userId: string
  tenantId: string
  avatarUrl?: string
  bio?: string
  dateOfBirth?: string
  gender?: string
  nationality?: string
  companyName?: string
  jobTitle?: string
  preferredLanguage?: string
  timezone?: string
  receiveNewsletter: boolean
  receivePromotionalEmails: boolean
  createdAt: string
  updatedAt: string
}

interface Address {
  id: string
  userId: string
  tenantId: string
  addressType: string
  streetAddress: string
  streetAddress2?: string
  city: string
  postalCode: string
  country: string
  state?: string
  recipientName: string
  phoneNumber?: string
  isDefault: boolean
  isActive: boolean
  createdAt: string
  updatedAt: string
}
```

## 🔌 API Endpoints

Die Komponenten erwarten folgende Backend-Endpoints:

```
GET    /api/admin/users              - Alle Benutzer abrufen
GET    /api/admin/users?page=1&pageSize=20
GET    /api/admin/users/:id          - Benutzer anzeigen
POST   /api/admin/users              - Benutzer erstellen
PUT    /api/admin/users/:id          - Benutzer aktualisieren
DELETE /api/admin/users/:id          - Benutzer löschen
GET    /api/admin/users/search?q=    - Benutzer suchen

GET    /api/admin/users/:id/profile           - Profil abrufen
PUT    /api/admin/users/:id/profile           - Profil aktualisieren

GET    /api/admin/users/:id/addresses         - Adressen abrufen
POST   /api/admin/users/:id/addresses         - Adresse erstellen
PUT    /api/admin/users/:id/addresses/:addrId - Adresse aktualisieren
DELETE /api/admin/users/:id/addresses/:addrId - Adresse löschen

POST   /api/admin/users/:id/verify-email      - E-Mail verifizieren
POST   /api/admin/users/:id/reset-password    - Passwort zurücksetzen
```

## 🎨 Design & Styling

- **Framework**: Tailwind CSS (teilweise) + Custom CSS
- **Icons**: Verwendete Icon-Klassen (anpassbar zu deinem Icon-Set)
- **Responsive**: Vollständig mobil-optimiert
- **Dark Mode**: Vorbereitet für Dark Mode Support

## 🧪 Testing

Die Komponenten enthalten `data-testid` Attribute für E2E-Tests:

```typescript
// Beispiel mit Playwright
await page.click('[data-testid="create-user-btn"]')
await page.fill('[data-testid="email-input"]', 'test@example.com')
await page.click('[data-testid="search-btn"]')
```

## 🔐 Sicherheit

- ✅ JWT-basierte Authentifizierung
- ✅ Tenant-Isolation (X-Tenant-ID Header)
- ✅ Role-basierte Autorisierung (requiredRole: "admin")
- ✅ CSRF Protection via Axios Interceptors
- ✅ Input-Validierung (Client & Server)
- ✅ Sensitive Daten nicht in Logs

## 📝 State Management

Der `useUserStore()` verwaltet:
- Liste von Benutzern
- Aktuell ausgewählter Benutzer
- Loading-Status
- Fehlerbehandlung
- Pagination-Info
- Suchqueries

## 🚨 Error Handling

- Automatische Error-Messages in UI
- Retry-Mechanismen für fehlgeschlagene Requests
- Benutzerfreundliche Fehlermeldungen
- Automatic 401 Redirect bei Auth-Fehlern

## 📦 Dependencies

```json
{
  "pinia": "^2.x",
  "vue": "^3.x",
  "vue-router": "^4.x",
  "axios": "^1.x"
}
```

## 🔄 Zukünftige Erweiterungen

- [ ] Bulk-Operationen (Mehrere Benutzer löschen)
- [ ] Import/Export (CSV-Import)
- [ ] Audit-Log (Wer hat was geändert)
- [ ] Permissions-Management
- [ ] Two-Factor Authentication Setup
- [ ] Activity-Timeline
