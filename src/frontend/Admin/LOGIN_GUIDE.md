# 🔐 Admin Frontend - Login & Authentication

## Vordefinierte Test-Credentials

### Standard Admin Account

**Email:** `admin@example.com`
**Passwort:** `password` (oder `password123`)

### Verwendung

1. Starten Sie das Frontend: `npm run dev`
2. Navigieren Sie zu: [http://localhost:5174](http://localhost:5174)
3. Sie werden zur Login-Seite weitergeleitet
4. Geben Sie die Credentials ein:
   - **Email**: `admin@example.com`
   - **Passwort**: `password`
5. Klicken Sie auf "Login"

---

## 🔄 Authentication Flow

```
Login-Seite (Login.vue)
    ↓
    Credentials → Backend /auth/login
    ↓
    Response: { user, accessToken, refreshToken }
    ↓
    Token in localStorage speichern
    ↓
    Redirect zu Dashboard
```

### Was passiert beim Login:

1. **Credentials senden** → Backend validiert Email und Passwort
2. **Token erhalten** → accessToken + refreshToken zurückbekommen
3. **Tokens speichern** → In localStorage für weitere Requests
4. **User-Daten speichern** → Im Pinia Auth Store
5. **Redirect** → Zur Dashboard-Seite

---

## 💾 Stored Data nach erfolgreichem Login

Nach dem Login werden folgende Daten gespeichert:

```javascript
// localStorage
localStorage.getItem('authToken'); // JWT Access Token
localStorage.getItem('refreshToken'); // Refresh Token
localStorage.getItem('tenantId'); // Tenant ID

// Pinia Store
authStore.user; // User Objekt
authStore.token; // Access Token
authStore.isAuthenticated; // Boolean Flag
```

---

## 🔒 Session Management

### Token Refresh

- **Access Token**: Gültig für ~1 Stunde
- **Refresh Token**: Kann neue Access Tokens generieren
- **Automatisches Refresh**: Bei API-Requests wird Token bei Bedarf aktualisiert

### Logout

- Alle Tokens werden gelöscht
- localStorage wird geleert
- Redirect zur Login-Seite

---

## 🛡️ Auth Guard / Route Protection

Folgende Routes sind geschützt:

```
/dashboard          ✅ Nur authentifizierte Benutzer
/cms/*             ✅ Nur authentifizierte Benutzer
/shop/*            ✅ Nur authentifizierte Benutzer
/jobs/*            ✅ Nur authentifizierte Benutzer

/login             ✅ Nur für nicht authentifizierte Benutzer
```

Wenn Sie ohne Token versuchen, auf `/dashboard` zuzugreifen → Redirect zu `/login`

---

## 👥 User Permissions & Roles

Das System unterstützt rollenbasierte Zugriffskontrolle (RBAC):

```typescript
interface AdminUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: Array<{ id: string; name: string }>;
  permissions: Array<{ id: string; name: string }>;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}
```

### Überprüfung von Permissions im Code:

```typescript
// Check single permission
if (authStore.hasPermission('create-product')) {
  // Show button
}

// Check single role
if (authStore.hasRole('admin')) {
  // Show admin features
}

// Check multiple roles
if (authStore.hasAnyRole(['admin', 'manager'])) {
  // Show management features
}
```

---

## 🔧 Backend Authentication Endpoints

Die Login-Form kommuniziert mit folgenden Endpoints:

### POST /auth/login

```json
Request:
{
  "email": "admin@example.com",
  "password": "password",
  "rememberMe": false
}

Response:
{
  "user": { ... },
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600
}
```

### POST /auth/logout

Logout-Request mit aktuellen Tokens

### POST /auth/refresh

```json
Request:
{
  "refreshToken": "..."
}

Response:
{
  "accessToken": "...",
  "refreshToken": "...",
  "expiresIn": 3600
}
```

### GET /auth/me

Gibt aktuellen User zurück (erfordert gültigen Token)

---

## 🧪 Testing mit Credentials

### Automatisches Login in Tests

```typescript
// In E2E Tests
await page.fill('input[type="email"]', 'admin@example.com');
await page.fill('input[type="password"]', 'password');
await page.click('button[type="submit"]');
await page.waitForURL('/dashboard');
```

### In Unit Tests

```typescript
// Mit Pinia
const authStore = useAuthStore();
await authStore.login('admin@example.com', 'password');
expect(authStore.isAuthenticated).toBe(true);
```

---

## 🔐 Sicherheitshinweise

### ⚠️ Wichtig für Produktion:

1. **Keine hardcodierten Credentials** - In Production externe Credentials-Manager nutzen
2. **HTTPS nur** - Tokens nur über HTTPS übertragen
3. **Token Rotation** - Regelmäßige Token-Rotation implementieren
4. **CORS konfigurieren** - Nur vertraute Domains erlauben
5. **HTTP-Only Cookies** - Optional: Tokens in HTTP-Only Cookies speichern
6. **CSP Header** - Content Security Policy setzen
7. **Rate Limiting** - Login Versuche limitieren (z.B. Max 5 Versuche/Minute)

### Development vs Production

**Development:**

```
Vordefinierte Credentials: admin@example.com / password
Tokens in localStorage gespeichert
CORS offen
```

**Production:**

```
Echte Benutzer-Accounts erforderlich
Tokens mit HttpOnly Flag
CORS restriktiv
Rate Limiting aktiv
2FA/MFA aktiviert
```

---

## 🐛 Häufige Probleme

### "Login failed"

- Credentials falsch
- Backend nicht erreichbar (localhost:9000)
- CORS-Fehler (prüfen Sie Browser Console)

### Token ungültig

- Session abgelaufen
- Token wurde gelöscht
- Browser-Cache leeren

### Keine Navigation nach Login

- Token nicht gespeichert
- Router Guards nicht korrekt konfiguriert
- Prüfen Sie Redux State in Vue DevTools

---

## 📖 Weitere Ressourcen

- [Auth Store Documentation](../docs/stores/auth.md)
- [API Client Documentation](../docs/services/api.md)
- [Testing Guide](../docs/TESTING_GUIDE.md)
- [Security Guide](../docs/SECURITY.md)

---

## ✅ Checkliste für Login-Test

- [ ] Frontend läuft auf http://localhost:5174
- [ ] Backend läuft auf http://localhost:9000
- [ ] Email eingeben: `admin@example.com`
- [ ] Passwort eingeben: `password`
- [ ] "Login" Button klicken
- [ ] Auf Dashboard weitergeleitet
- [ ] Username oben rechts angezeigt
- [ ] Logout funktioniert
