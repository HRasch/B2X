# User Management - Integration Test Guide

Dokumentation für das Testen der neuen User Management Funktionalität.

## 🧪 Test Szenarien

### 1. User List View

```bash
# Startup Frontend
npm run dev

# Öffne im Browser
http://localhost:5174/users
```

**Verifikation:**

- [ ] Alle Benutzer sind sichtbar (falls Daten in Backend vorhanden)
- [ ] Tabelle zeigt: Name, Email, Telefon, Status, Erstellt, Letzter Login
- [ ] Pagination funktioniert (Previous/Next Buttons)
- [ ] Search-Feld funktioniert (Filter nach Email/Name/Telefon)
- [ ] Filter nach Status funktioniert (Active/Inactive)
- [ ] Sort funktioniert (nach Name, Email, Updated)
- [ ] Action-Buttons sichtbar (View, Edit, Delete)

### 2. User erstellen

**Flow:**

```
http://localhost:5174/users
  → [+ Create User] Button
  → /users/create
  → Formular ausfüllen
  → [Save] Klick
  → Zurück zu /users
```

**Formular validieren:**

- [ ] Vorname erforderlich
- [ ] Nachname erforderlich
- [ ] E-Mail erforderlich + gültig
- [ ] Telefon optional (Format validiert)
- [ ] Verifikationsstatus setzbar
- [ ] Profil-Felder ausfüllbar
- [ ] Error-Messages angezeigt bei Validierungsfehlern
- [ ] Success-Message nach Create
- [ ] Zurück-Button funktioniert

### 3. User anzeigen (Detail)

**Flow:**

```
/users
  → [User anklicken oder View Button]
  → /users/:id
```

**Elemente verifikation:**

- [ ] User-Karte mit Avatar (wenn vorhanden)
- [ ] Name, Email, Telefon angezeigt
- [ ] Status-Badge (Active/Inactive)
- [ ] Email/Phone Verifikationsstatus
- [ ] Erstellungsdatum angezeigt
- [ ] Letzter Login angezeigt
- [ ] Tabs vorhanden (Overview, Addresses)
- [ ] [Edit] Button funktioniert
- [ ] [Delete] Button funktioniert

### 4. User bearbeiten

**Flow:**

```
/users/:id
  → [Edit] Button
  → /users/:id/edit
  → Form mit existierenden Daten
  → Änderungen vornehmen
  → [Update] Klick
  → Zurück zu /users/:id
```

**Validierung:**

- [ ] Existierende Daten sind in Formular gefüllt
- [ ] Alle Felder editierbar
- [ ] Validierung funktioniert
- [ ] Success-Message nach Update
- [ ] Details-Seite zeigt aktualisierte Daten

### 5. User löschen

**Flow:**

```
/users
  → [Delete Button] oder /users/:id → [Delete]
  → Bestätigungs-Modal
  → [Confirm] Klick
  → Modal schließt
  → User entfernt aus Liste
```

**Validierung:**

- [ ] Bestätigungs-Dialog angezeigt
- [ ] User wird entfernt
- [ ] Success-Message angezeigt
- [ ] Liste aktualisiert sich

### 6. Adressen-Management

**In User Detail View:**

```
/users/:id
  → Addresses Tab
  → [+ Add Address] Button
  → Modal mit Adressformular
  → Speichern
  → Adresse in Liste sichtbar
```

**Validierung:**

- [ ] Adressen-Liste angezeigt
- [ ] Add-Button funktioniert
- [ ] Modal öffnet sich
- [ ] Pflichtfelder validiert
- [ ] Adresse nach Save sichtbar
- [ ] Delete-Button entfernt Adresse

### 7. Search & Filter

**Search Test:**

```
/users
  → Search-Feld: "john" eingeben
  → Enter drücken
  → Nur Benutzer mit "john" in Email/Name/Telefon angezeigt
```

**Filter Test:**

```
/users
  → Status Dropdown: "Active" wählen
  → Nur aktive Benutzer angezeigt
```

**Sort Test:**

```
/users
  → Sort Dropdown: "Name (A-Z)" wählen
  → Benutzer alphabetisch sortiert
```

---

## 📱 Responsive Design Test

### Desktop (1920px)

- [ ] Layout voll breit
- [ ] Tabelle mit allen Spalten sichtbar
- [ ] Keine Scrollbars

### Tablet (768px)

- [ ] Layout angepasst
- [ ] Tabelle scrollbar (horizontal)
- [ ] Buttons sichtbar

### Mobile (375px)

- [ ] Stack-Layout
- [ ] Touch-freundliche Buttons
- [ ] Kein Horizontal-Scroll

---

## 🔌 API Mock (Falls Backend nicht verfügbar)

Falls das Backend noch nicht implementiert ist, können Sie folgende Mock-Daten in `userService.ts` verwenden:

```typescript
// In userService.ts, getUsers()-Methode
if (process.env.VITE_USE_MOCK_DATA === 'true') {
  return {
    data: [
      {
        id: '1',
        email: 'john.doe@example.com',
        firstName: 'John',
        lastName: 'Doe',
        phoneNumber: '+49 30 123456789',
        isActive: true,
        isEmailVerified: true,
        isPhoneVerified: false,
        createdAt: '2024-01-15T10:30:00Z',
        updatedAt: '2024-01-20T14:45:00Z',
        lastLoginAt: '2024-01-22T08:15:00Z',
      },
      {
        id: '2',
        email: 'jane.smith@example.com',
        firstName: 'Jane',
        lastName: 'Smith',
        phoneNumber: '+49 30 987654321',
        isActive: true,
        isEmailVerified: true,
        isPhoneVerified: true,
        createdAt: '2024-01-10T09:00:00Z',
        updatedAt: '2024-01-21T16:20:00Z',
        lastLoginAt: '2024-01-22T11:30:00Z',
      },
    ],
    pagination: {
      currentPage: 1,
      pageSize: 20,
      totalPages: 1,
      totalCount: 2,
    },
  };
}
```

---

## 🐛 Häufige Fehler

### "Cannot read property 'value' of undefined"

**Ursache**: Store ist nicht richtig importiert
**Lösung**:

```typescript
import { useUserStore } from '@/stores/users';

// Im setup():
const userStore = useUserStore();
```

### "API returned 401 Unauthorized"

**Ursache**: JWT-Token ist abgelaufen oder nicht gesetzt
**Lösung**:

- Login durchführen
- Token in localStorage überprüfen
- Browser-Cookies clearen und neuladen

### "Cannot find module '@/...'"

**Ursache**: Path Alias nicht konfiguriert
**Lösung**: Überprüfe `vite.config.ts`:

```typescript
resolve: {
  alias: {
    '@': fileURLToPath(new URL('./src', import.meta.url))
  }
}
```

### Laden hängt sich auf

**Ursache**: Backend antwortet nicht / CORS-Error
**Lösung**:

- Öffne Browser DevTools (F12)
- Überprüfe Network-Tab
- Überprüfe Console auf CORS-Fehler
- Backend muss auf Port 8080 laufen (Admin API)

---

## ✅ Erfolgs-Checkliste

Vor Production sollten alle Punkte erfüllt sein:

```
Frontend-Tests:
[ ] User List lädt und zeigt Daten
[ ] Search funktioniert
[ ] Filter funktioniert
[ ] Pagination funktioniert
[ ] Create User funktioniert
[ ] Edit User funktioniert
[ ] Delete User funktioniert
[ ] User Detail anzeigbar
[ ] Adressen-Management funktioniert
[ ] Responsive Design OK (Desktop/Tablet/Mobile)
[ ] Error-Handling funktioniert
[ ] Loading-States sichtbar

Backend-Integration:
[ ] API auf http://localhost:8080 erreichbar
[ ] GET /api/admin/users antwortet
[ ] POST /api/admin/users funktioniert
[ ] PUT /api/admin/users/:id funktioniert
[ ] DELETE /api/admin/users/:id funktioniert
[ ] Search-Endpoint funktioniert
[ ] Pagination korrekt
[ ] JWT-Authorization funktioniert
[ ] Tenant-Isolation funktioniert
[ ] Error-Responses korrekt (400, 401, 403, 404, 500)

Security:
[ ] Token wird sicher gespeichert (httpOnly Cookie oder localStorage)
[ ] CORS-Headers korrekt
[ ] Sensitive Daten nicht in Logs
[ ] Rate-Limiting funktioniert
[ ] SQL-Injection unmöglich
```

---

## 🚀 Next Steps

1. **Backend API implementieren** (siehe [Admin API Guide](../../docs/ADMIN_API_GUIDE.md))
   - UserController erstellen
   - Repository implementieren
   - Datenbank-Migrations durchführen

2. **E2E Tests schreiben** (Playwright)
   - UserList.spec.ts
   - UserForm.spec.ts
   - UserDetail.spec.ts

3. **Unit Tests hinzufügen**
   - userService.test.ts
   - userStore.test.ts

4. **Performance optimieren**
   - Lazy-Loading aktivieren
   - Virtuelle Scrolling für große Listen
   - Caching-Strategie

5. **Audit-Logging aktivieren**
   - User-Änderungen tracken
   - Change-History anzeigen

---

## 📞 Support

Probleme beim Testing? Überprüfe:

1. Frontend läuft auf http://localhost:5173 (Store) oder 5174 (Admin)
2. Backend läuft auf http://localhost:8080 (Admin API)
3. Browser Console auf Fehler überprüfen
4. Network-Tab zeigt requests und responses
