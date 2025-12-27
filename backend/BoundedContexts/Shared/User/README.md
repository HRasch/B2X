# B2Connect.Shared.User - Shared Kernel Domain

## 📋 Übersicht

Der **User Shared Kernel** verwaltet zentral alle benutzer- und adressbezogenen Daten, die **über mehrere Bounded Contexts** hinweg benötigt werden (Store, Admin, etc.):

- **User**: Kundenkonten und Basisinformationen
- **Profile**: Erweiterte Profilinformationen und Präferenzen  
- **Address**: Liefer- und Rechnungsadressen

## 🏗️ Architektur

### Shared Kernel Pattern
```
Shared/User/ ← Ein Single Source of Truth für User-Daten
    ↓
Store/Customers/     ← Injiziert IUserRepository
Admin/API/          ← Injiziert IUserRepository
```

### Repository-basierte Isolation
- **IUserRepository**: Zentrale Schnittstelle für User-Zugriff
- **IAddressRepository**: Zentrale Schnittstelle für Adressen-Zugriff
- Implementierungen in Infrastructure Layer

### Layers
```
Models/              → Domain Entities (User, Profile, Address)
Interfaces/          → Repository Contracts
Infrastructure/
  └── Data/          → DbContext, EF Core Konfiguration
    └── Repositories/ → Repository Implementations (planned)
```

## 🔄 Kommunikation mit anderen Contexts

### Store/Customers nutzt Shared/User:
```csharp
// In Dependency Injection:
services.AddScoped<IUserRepository>(sp => 
    new UserRepository(sp.GetRequiredService<UserDbContext>()));

// In ShoppingCart-Handler:
public class CreateShoppingCartHandler
{
    public CreateShoppingCartHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task Handle(CreateShoppingCartCommand cmd)
    {
        // Validiere dass User existiert
        var user = await _userRepository.GetByIdAsync(cmd.UserId);
        if (user == null)
            throw new UserNotFoundException();
    }
}
```

### Admin/API nutzt Shared/User:
```csharp
// Verwaltet User-CRUD Operationen
public class UpdateUserHandler
{
    public UpdateUserHandler(IUserRepository userRepository) { }
    
    public async Task Handle(UpdateUserCommand cmd)
    {
        var user = await _userRepository.GetByIdAsync(cmd.UserId);
        user.UpdateProfile(...);
        await _userRepository.UpdateAsync(user);
    }
}
```

## 📊 Entity Relationships

```
User (Aggregate Root)
├── Profile (1-to-1)
└── Addresses (1-to-many)
    ├── Shipping addresses
    ├── Billing addresses
    └── Default address per type
```

## 🎯 Core Domain Models

### User Entity
- Multi-Tenant (TenantId)
- Email/Phone Verification Tracking
- Login History
- Audit Trail (CreatedAt, UpdatedAt)

### Profile Entity
- Extended user information
- Preferences (language, timezone, newsletter opt-in)
- Personal data (DOB, gender, company)

### Address Entity
- Multiple per user
- Type-based (shipping, billing, both)
- Default address per type
- Full address formatting

## 💾 Datenbank

**Single Database** für alle User-Daten:
```sql
users
├── id (UUID)
├── tenant_id (UUID)
├── email (encrypted)
├── first_name, last_name
├── is_active, is_email_verified
└── created_at, updated_at

user_profiles
├── id (UUID)
├── user_id (FK)
├── tenant_id (UUID)
├── date_of_birth (encrypted)
└── preferences (language, timezone, newsletter)

addresses
├── id (UUID)
├── user_id (FK)
├── tenant_id (UUID)
├── address_type (shipping|billing|both)
├── is_default
└── full address fields
```

## ✅ Nächste Schritte

- [ ] Infrastructure/Data/UserDbContext.cs
- [ ] Repository Implementations
- [ ] EF Core Entity Type Configurations
- [ ] Database Migrations
- [ ] Unit Tests
- [ ] Integration Tests

## 📚 Referenzen

- [DDD Shared Kernel](https://martinfowler.com/bliki/BoundedContext.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Onion Architecture](../../docs/architecture/ONION_ARCHITECTURE.md)
