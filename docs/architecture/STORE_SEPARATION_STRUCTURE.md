# Projektstruktur Reorganisation - Store / Admin / Common Separation

## ✅ Abgeschlossene Struktur

```
src/
├── Core/
│   ├── Common/
│   │   ├── Entities/
│   │   │   ├── Store.cs
│   │   │   ├── Language.cs
│   │   │   └── Country.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── IStoreRepository.cs
│   │       ├── ILanguageRepository.cs
│   │       └── ICountryRepository.cs
│   │
│   └── Store/
│       ├── Entities/
│       │   ├── PaymentMethod.cs
│       │   └── ShippingMethod.cs
│       └── Interfaces/
│           ├── IPaymentMethodRepository.cs
│           └── IShippingMethodRepository.cs
│
├── Application/
│   ├── Common/
│   │   └── (Reserved for shared services)
│   │
│   ├── Store/
│   │   ├── Services/
│   │   │   ├── StoreService.cs (Write)
│   │   │   ├── LanguageService.cs (Write)
│   │   │   ├── CountryService.cs (Write)
│   │   │   ├── PaymentMethodService.cs (Write)
│   │   │   └── ShippingMethodService.cs (Write)
│   │   │
│   │   └── ReadServices/
│   │       ├── StoreReadService.cs (Read-only, optimized)
│   │       ├── LanguageReadService.cs (Read-only, optimized)
│   │       ├── CountryReadService.cs (Read-only, optimized)
│   │       ├── PaymentMethodReadService.cs (Read-only, optimized)
│   │       └── ShippingMethodReadService.cs (Read-only, optimized)
│   │
│   └── Admin/
│       └── Services/
│           └── (Reserved for admin-specific services)
│
├── Infrastructure/
│   ├── Common/
│   │   ├── Data/
│   │   │   └── StoreDbContext.cs
│   │   └── Repositories/
│   │       ├── Repository.cs (Generic base)
│   │       ├── StoreRepository.cs
│   │       ├── LanguageRepository.cs
│   │       └── CountryRepository.cs
│   │
│   ├── Store/
│   │   └── Repositories/
│   │       ├── PaymentMethodRepository.cs
│   │       └── ShippingMethodRepository.cs
│   │
│   └── Admin/
│       └── Repositories/
│           └── (Reserved for admin-specific repositories)
│
└── Presentation/
    └── Controllers/
        ├── Admin/
        │   ├── StoresController.cs (Write, [Authorize(Roles = "Admin")])
        │   ├── LanguagesController.cs (Write, [Authorize(Roles = "Admin")])
        │   ├── CountriesController.cs (Write, [Authorize(Roles = "Admin")])
        │   ├── PaymentMethodsController.cs (Write, [Authorize(Roles = "Admin")])
        │   └── ShippingMethodsController.cs (Write, [Authorize(Roles = "Admin")])
        │
        └── Public/
            ├── PublicStoresController.cs (Read-only, public)
            ├── PublicLanguagesController.cs (Read-only, public)
            ├── PublicCountriesController.cs (Read-only, public)
            ├── PublicPaymentMethodsController.cs (Read-only, public)
            └── PublicShippingMethodsController.cs (Read-only, public)
```

## 📋 Namespaces

### Core Entities
- **Common Entities**: `B2X.Store.Core.Common.Entities`
  - Store, Language, Country (Shared across all domains)
  
- **Store Entities**: `B2X.Store.Core.Store.Entities`
  - PaymentMethod, ShippingMethod (Store-specific)

### Core Interfaces
- **Common Interfaces**: `B2X.Store.Core.Common.Interfaces`
  - IRepository, IStoreRepository, ILanguageRepository, ICountryRepository
  
- **Store Interfaces**: `B2X.Store.Core.Store.Interfaces`
  - IPaymentMethodRepository, IShippingMethodRepository

### Application Services
- **Write Services**: `B2X.Store.Application.Store.Services`
  - StoreService, LanguageService, CountryService, PaymentMethodService, ShippingMethodService
  
- **Read Services**: `B2X.Store.Application.Store.ReadServices`
  - StoreReadService, LanguageReadService, CountryReadService, PaymentMethodReadService, ShippingMethodReadService

### Infrastructure
- **Common Repositories**: `B2X.Store.Infrastructure.Common.Repositories`
  - Repository<T>, StoreRepository, LanguageRepository, CountryRepository
  
- **Common Data**: `B2X.Store.Infrastructure.Common.Data`
  - StoreDbContext
  
- **Store Repositories**: `B2X.Store.Infrastructure.Store.Repositories`
  - PaymentMethodRepository, ShippingMethodRepository

### Presentation Controllers
- **Admin**: `B2X.Store.Presentation.Controllers.Admin`
  - StoresController, LanguagesController, CountriesController, PaymentMethodsController, ShippingMethodsController
  
- **Public**: `B2X.Store.Presentation.Controllers.Public`
  - PublicStoresController, PublicLanguagesController, PublicCountriesController, PublicPaymentMethodsController, PublicShippingMethodsController

## 🔑 Key Separation Principles

### Common Domain
**Shared by Store, Admin, and future domains**
- Store, Language, Country entities
- Common repositories and interfaces
- Central DbContext
- Used by both read and write operations

### Store Domain
**Store-specific features**
- PaymentMethod and ShippingMethod entities
- Store-specific repositories
- Write services (CRUD with validation)
- Read-only services (optimized public API)
- Admin controllers (protected with JWT and Role authorization)
- Public controllers (unauthenticated read-only access)

### Admin Domain
**Reserved for future admin-specific functionality**
- Admin-only operations beyond simple CRUD
- Admin-specific services and repositories
- Dashboard endpoints
- Reporting and analytics

## 🔄 Service Pattern: CQRS

### Write Services (Command)
Located in: `Application/Store/Services/`
- Traditional CRUD operations
- Full validation
- Business logic enforcement
- Used by Admin controllers via JWT-protected endpoints

### Read Services (Query)
Located in: `Application/Store/ReadServices/`
- Optimized for public API access
- No write operations
- Structured logging
- Used by Public controllers for unauthenticated access

## 🔐 Access Control

### Admin Endpoints
- Route: `/api/stores`, `/api/languages`, etc.
- Auth: JWT Bearer token + Admin role
- Operations: POST (create), PUT (update), DELETE (delete)
- Services: Write Services (StoreService, LanguageService, etc.)

### Public Endpoints  
- Route: `/api/public/stores`, `/api/public/languages`, etc.
- Auth: None (public access)
- Operations: GET only (read-only)
- Services: Read Services (StoreReadService, LanguageReadService, etc.)

## 📦 Dependencies Flow

```
Presentation (Controllers)
    ↓
Application (Services)
    ↓
Core (Entities & Interfaces)
    ↓
Infrastructure (Repositories & DbContext)
    ↓
Database
```

## ✨ Benefits of This Structure

1. **Clear Separation of Concerns**
   - Common vs Store vs Admin domains clearly separated
   - Easy to add new domains in future

2. **CQRS Pattern Implementation**
   - Read and Write operations are fully separated
   - Optimized query handling for public API
   - Better performance and scalability

3. **Security & Authorization**
   - Admin operations protected with JWT + Role checks
   - Public operations fully exposed for storefront
   - Clear distinction between internal and external APIs

4. **Maintainability**
   - Each domain is independent and focused
   - Easy to test and modify individual domains
   - Clear dependency management

5. **Future Extensibility**
   - Common domain can be extended with new shared entities
   - Store domain can grow with additional features
   - Admin domain reserved for advanced operations

## 📝 Migration Status

- ✅ Core entities migrated to proper locations
- ✅ Core interfaces migrated to proper locations
- ✅ Repositories migrated and reorganized
- ✅ DbContext migrated to Common.Data
- ✅ Services created in new locations with updated namespaces
- ✅ Controllers created in Admin/Public subdirectories
- ✅ Program.cs updated with new using statements
- ✅ Old files preserved for reference (can be deleted later)

## 🛠️ Next Steps

1. **Verify Build** - Ensure all namespace references are correct
2. **Test Controllers** - Validate admin and public endpoints work
3. **Test Services** - Verify write and read services function correctly
4. **Database Migration** - Create EF Core migrations if needed
5. **Clean Up** - Remove old files from original locations once verified
