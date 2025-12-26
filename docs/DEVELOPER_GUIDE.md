# B2Connect - Entwicklerdokumentation

> Deine umfassende Ressource für die Entwicklung an B2Connect

## 📋 Inhaltsverzeichnis

1. [Quick Start](#-quick-start)
2. [Projektstruktur](#-projektstruktur)
3. [Technologie-Stack](#-technologie-stack)
4. [Backend-Entwicklung](#-backend-entwicklung)
5. [Frontend-Entwicklung](#-frontend-entwicklung)
6. [Datenbank & Services](#-datenbank--services)
7. [Häufige Aufgaben](#-häufige-aufgaben)
8. [Troubleshooting](#-troubleshooting)

---

## 🚀 Quick Start

### Voraussetzungen

- **.NET 10** für Backend-Entwicklung
- **Node.js 18+** und **npm** für Frontend
- **Git** für Versionskontrolle
- Editor: **VS Code** (empfohlen)
- ⭐ **Keine Datenbank nötig!** (InMemory-Database wird verwendet)

### Setup in 5 Minuten (mit VS Code & InMemory)

**Option 1: Schnellstart ohne Terminal (Empfohlen!)**

```
1. Öffne das Projekt in VS Code
2. Gehe zu: Debug (Strg+Shift+D)
3. Wähle Dropdown: "Full Stack (Aspire + Frontend) - InMemory 🚀"
4. Drücke F5
5. Frontend öffnet sich automatisch auf http://localhost:5173
6. Fertig! 🎉
```

**Option 2: Terminal-basierter Aufbau**

```bash
# 1. Repository klonen
git clone <your-repo>
cd B2Connect

# 2. Backend starten (AppHost orchestriert alle Services)
cd backend/services/AppHost
export Database__Provider=inmemory
dotnet run

# 3. In einem neuen Terminal: Frontend starten
cd frontend
npm install
npm run dev

# 4. (Optional) Admin Frontend starten
cd frontend-admin
npm install
npm run dev
```

### 💡 Über InMemory-Database

Alle Services verwenden automatisch eine In-Memory-Datenbank beim Development-Start:
- ✅ **Keine PostgreSQL/Docker Installation nötig**
- ✅ **Startup in 2-3 Sekunden**
- ✅ **Perfekt für schnelle Entwicklung**
- ⚠️ **Daten gehen beim Neustart verloren** (das ist gewünscht)

👉 [Detaillierte InMemory-Konfiguration](./VSCODE_INMEMORY_SETUP.md)

**Verfügbare Services nach dem Start:**

| Service | URL | Port |
|---------|-----|------|
| Auth Service | http://localhost:9002 | 9002 |
| Tenant Service | http://localhost:9003 | 9003 |
| Localization Service | http://localhost:9004 | 9004 |
| API Gateway | http://localhost:15500 | 15500 |
| Frontend | http://localhost:5173 | 5173 |
| Admin Frontend | http://localhost:5174 | 5174 |

---

## 📁 Projektstruktur

```
B2Connect/
├── backend/
│   ├── services/
│   │   ├── AppHost/                    # Aspire-Orchestration (Einstiegspunkt)
│   │   ├── auth-service/               # Authentifizierung & Autorisierung
│   │   ├── tenant-service/             # Mandantenverwaltung
│   │   ├── api-gateway/                # API Gateway & Routing
│   │   ├── shop-service/               # Shop-Kern (Produkte, Orders)
│   │   ├── catalog-service/            # Produktkatalog-Management
│   │   ├── order-service/              # Bestellverwaltung
│   │   ├── payment-service/            # Zahlungsverarbeitung
│   │   ├── inventory-service/          # Bestandsverwaltung
│   │   ├── procurement-gateway/        # Beschaffungsplattform-Integration
│   │   ├── notification-service/       # Benachrichtigungen (E-Mail, SMS)
│   │   └── ServiceDefaults/            # Gemeinsame Service-Konfiguration
│   ├── shared/                         # Gemeinsame Libraries
│   └── docs/                           # Backend-Dokumentation
│
├── frontend/                           # Vue.js 3 Shop-Frontend
│   ├── src/
│   │   ├── views/                      # Seiten
│   │   ├── components/                 # UI-Komponenten
│   │   ├── stores/                     # Pinia State Management
│   │   ├── services/                   # API-Integration
│   │   └── styles/                     # Global Styles
│   └── tests/                          # Unit & Integration Tests
│
├── frontend-admin/                     # Vue.js 3 Admin-Panel
│   ├── src/
│   │   ├── views/                      # Admin-Seiten
│   │   ├── components/                 # Admin-Komponenten
│   │   ├── stores/                     # State Management
│   │   └── styles/                     # Admin-Styles
│   └── tests/
│
├── docs/
│   ├── DEVELOPER_GUIDE.md              # Diese Datei
│   ├── archived/                       # Alte Dokumentation
│   └── architecture/                   # Architektur-Guides
│
├── README.md                           # Projektübersicht
├── DEVELOPMENT.md                      # Setup-Details
├── BUSINESS_REQUIREMENTS.md            # Features & Roadmap
└── CODING_STANDARDS.md                 # Code-Style Richtlinien
```

---

## 💻 Technologie-Stack

### Backend

- **.NET 10** - Framework
- **Aspire** - Microservices-Orchestration
- **Entity Framework Core** - ORM für Datenbankzugriff
- **Quartz.NET** - Job-Scheduling
- **Elasticsearch** - Fulltext-Suche
- **PostgreSQL** - Datenbank (lokal Docker)

### Frontend

- **Vue.js 3** - UI Framework
- **Vite** - Build Tool
- **Pinia** - State Management
- **Tailwind CSS** - Styling
- **TypeScript** - Typsicherheit

### Tools

- **VS Code** - Editor (empfohlen)
- **Visual Studio** oder **Rider** - IDEs
- **Postman** - API Testing
- **Git** - Versionskontrolle

---

## 🔧 Backend-Entwicklung

### AppHost starten

```bash
cd backend/services/AppHost
dotnet run
```

**Was macht AppHost?**
- Startet alle Microservices
- Konfiguriert Datenbank
- Setzt Environment-Variablen
- Zeigt Liveness-Probes

### Neuen Service erstellen

```bash
# Template
dotnet new aspire-starter -n YourServiceName

# Im AppHost registrieren (Program.cs):
var yourService = builder.AddProject<Projects.YourServiceName>("your-service")
    .WithHttpEndpoint(port: 9005);
```

### Entity Framework Migrations

```bash
# In Service-Verzeichnis
cd backend/services/your-service

# Migration erstellen
dotnet ef migrations add AddYourTable

# Auf Datenbank anwenden
dotnet ef database update
```

### Häufige Backend-Aufgaben

#### 1. Neuen Endpoint hinzufügen

```csharp
// Controllers/YourController.cs
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<YourDto>> GetById(int id)
    {
        // Logik hier
        return Ok(result);
    }
}
```

#### 2. Service mit Dependency Injection

```csharp
// Services/YourService.cs
public interface IYourService
{
    Task<Result> DoSomethingAsync(int id);
}

public class YourService : IYourService
{
    private readonly ILogger<YourService> _logger;
    
    public YourService(ILogger<YourService> logger)
    {
        _logger = logger;
    }
    
    public async Task<Result> DoSomethingAsync(int id)
    {
        // Implementierung
    }
}

// In Program.cs
builder.Services.AddScoped<IYourService, YourService>();
```

#### 3. Tests schreiben

```csharp
// Tests/YourServiceTests.cs
public class YourServiceTests
{
    [Fact]
    public async Task DoSomethingAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var service = new YourService(mockLogger);
        
        // Act
        var result = await service.DoSomethingAsync(1);
        
        // Assert
        result.Should().BeSuccess();
    }
}
```

---

## 🎨 Frontend-Entwicklung

### Frontend starten

```bash
cd frontend
npm install
npm run dev
```

### Admin Frontend starten

```bash
cd frontend-admin
npm install
npm run dev
```

### Pinia Store erstellen

```typescript
// src/stores/yourStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useYourStore = defineStore('yourStore', () => {
  const items = ref<Item[]>([])
  
  const count = computed(() => items.value.length)
  
  const addItem = (item: Item) => {
    items.value.push(item)
  }
  
  return { items, count, addItem }
})
```

### Vue Component erstellen

```vue
<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold">{{ title }}</h1>
    
    <div v-if="loading" class="text-gray-500">Loading...</div>
    <div v-else>
      <ul>
        <li v-for="item in items" :key="item.id">{{ item.name }}</li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Item } from '@/types'

const title = ref('Items List')
const items = ref<Item[]>([])
const loading = ref(false)

onMounted(async () => {
  loading.value = true
  items.value = await fetchItems()
  loading.value = false
})

const fetchItems = async () => {
  // API-Call
  return []
}
</script>

<style scoped>
/* Deine Styles hier */
</style>
```

### API-Service integrieren

```typescript
// src/services/api.ts
import axios from 'axios'

const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:15500'
})

export const itemApi = {
  getAll: () => client.get('/api/items'),
  getById: (id: number) => client.get(`/api/items/${id}`),
  create: (data: any) => client.post('/api/items', data),
  update: (id: number, data: any) => client.put(`/api/items/${id}`, data),
  delete: (id: number) => client.delete(`/api/items/${id}`)
}
```

---

## 🗄️ Datenbank & Services

### Datenbank-Setup

```bash
# PostgreSQL im Docker starten (falls lokal)
docker run --name postgres -e POSTGRES_PASSWORD=password -p 5432:5432 postgres

# AppHost erstellt Datenbank automatisch
cd backend/services/AppHost
dotnet run
```

### Elasticsearch (für Suche)

```bash
# Elasticsearch starten
docker run -d -p 9200:9200 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:latest

# Mit AppHost wird automatisch indexiert
```

### Redis (Caching)

```bash
# Redis starten
docker run -d -p 6379:6379 redis

# Im Service nutzen
services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
});
```

---

## 📋 Häufige Aufgaben

### Build & Clean

```bash
# Backend aufräumen
cd backend
dotnet clean
dotnet build

# Frontend aufräumen
cd frontend
npm run clean
npm install
npm run build
```

### Tests ausführen

```bash
# Backend-Tests
cd backend/services/YourService.Tests
dotnet test

# Frontend-Tests
cd frontend
npm run test
```

### Git Workflow

```bash
# Feature-Branch erstellen
git checkout -b feature/your-feature

# Änderungen committen
git add .
git commit -m "feat: add your feature"

# Push
git push origin feature/your-feature

# Pull Request erstellen (GitHub)
```

### Logging

```csharp
// Backend
_logger.LogInformation("Something happened: {@Details}", data);
_logger.LogError(ex, "An error occurred");

// Frontend
console.log('Debug info:', data)
console.error('Error:', error)
```

### Debugging

**Backend (VS Code):**
```
1. F5 oder Debug-Button klicken
2. Breakpoint setzen (Zeile anklicken)
3. Code läuft und stoppt am Breakpoint
```

**Frontend (VS Code):**
```
1. Extensions installieren: Debugger for Chrome
2. F5 zum Debuggen
3. Breakpoints setzen im Code
```

---

## 🐛 Troubleshooting

### Häufige Probleme

#### Problem: "Port is already in use"

```bash
# Port freimachen (macOS/Linux)
lsof -i :9002
kill -9 <PID>

# Oder andere Port in AppHost verwenden
```

#### Problem: Datenbank-Verbindungsfehler

```bash
# Datenbank-Logs ansehen
docker logs postgres

# Oder neu starten
docker restart postgres
```

#### Problem: Frontend zeigt "Cannot connect to API"

```bash
# 1. API-URL in .env überprüfen
VITE_API_URL=http://localhost:15500

# 2. CORS-Settings im Backend überprüfen
# In Program.cs sollte CORS konfiguriert sein

# 3. AppHost läuft?
dotnet run  # im AppHost-Verzeichnis
```

#### Problem: npm-Module nicht gefunden

```bash
# node_modules löschen und neu installieren
rm -rf node_modules package-lock.json
npm install
```

### Logs überprüfen

```bash
# Backend-Logs (AppHost zeigt sie)
# Suche nach "error" oder "exception"

# Frontend-Logs
# Browser DevTools: F12 → Console
```

---

## 📚 Weitere Ressourcen

- [.NET Dokumentation](https://docs.microsoft.com/dotnet)
- [Vue.js 3 Guide](https://vuejs.org/guide/introduction.html)
- [Aspire Docs](https://learn.microsoft.com/dotnet/aspire/)
- [Tailwind CSS](https://tailwindcss.com)
- [Pinia Dokumentation](https://pinia.vuejs.org)

---

## 💬 Fragen oder Probleme?

Schau in die **archived/** Dokumentation oder frag im Team!

**Letzte Aktualisierung:** 26. Dezember 2025
