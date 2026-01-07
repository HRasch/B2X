# Backend API Implementation Roadmap

**Objective:** Admin API Endpoints für User Management implementieren  
**Frontend Status:** ✅ Komplett (wartet auf Backend)  
**Timeline:** 4-6 Stunden  

---

## 📋 API Endpoints zu implementieren

### 1. User Listing & Pagination
```
GET /api/admin/users?page=1&pageSize=20
```

**Request:**
```
GET /api/admin/users?page=1&pageSize=20&sortBy=name&sortOrder=asc
Headers:
  Authorization: Bearer <jwt-token>
  X-Tenant-ID: <tenant-id>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "john.doe@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "phoneNumber": "+49 30 123456789",
      "isActive": true,
      "isEmailVerified": true,
      "isPhoneVerified": false,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-20T14:45:00Z",
      "lastLoginAt": "2024-01-22T08:15:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 95,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "timestamp": "2024-01-27T15:30:00Z"
}
```

### 2. Get Single User
```
GET /api/admin/users/{userId}
```

**Request:**
```
GET /api/admin/users/550e8400-e29b-41d4-a716-446655440000
Headers:
  Authorization: Bearer <jwt-token>
  X-Tenant-ID: <tenant-id>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+49 30 123456789",
    "isActive": true,
    "isEmailVerified": true,
    "isPhoneVerified": false,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-20T14:45:00Z",
    "lastLoginAt": "2024-01-22T08:15:00Z",
    "profile": {
      "avatarUrl": "https://...",
      "bio": "Software Developer",
      "dateOfBirth": "1990-05-15",
      "companyName": "Tech Corp",
      "jobTitle": "Senior Developer",
      "preferredLanguage": "de",
      "timezone": "Europe/Berlin",
      "receiveNewsletter": true,
      "receivePromotionalEmails": false
    },
    "addresses": [
      {
        "id": "addr-001",
        "addressType": "billing",
        "streetAddress": "Hauptstraße 123",
        "city": "Berlin",
        "postalCode": "10115",
        "country": "DE",
        "recipientName": "John Doe",
        "isDefault": true
      }
    ]
  },
  "timestamp": "2024-01-27T15:30:00Z"
}
```

### 3. Create User
```
POST /api/admin/users
```

**Request:**
```json
{
  "email": "new.user@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+49 30 987654321",
  "password": "SecurePassword123!",
  "isActive": true,
  "profile": {
    "companyName": "My Company",
    "jobTitle": "Manager",
    "preferredLanguage": "de",
    "timezone": "Europe/Berlin",
    "receiveNewsletter": true
  }
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "email": "new.user@example.com",
    "firstName": "Jane",
    "lastName": "Smith",
    "phoneNumber": "+49 30 987654321",
    "isActive": true,
    "isEmailVerified": false,
    "isPhoneVerified": false,
    "createdAt": "2024-01-27T15:35:00Z",
    "createdBy": "admin-id"
  },
  "timestamp": "2024-01-27T15:35:00Z"
}
```

### 4. Update User
```
PUT /api/admin/users/{userId}
```

**Request:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+49 30 987654321",
  "isActive": true,
  "profile": {
    "companyName": "Updated Company",
    "jobTitle": "Director"
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "email": "new.user@example.com",
    "firstName": "Jane",
    "lastName": "Smith",
    "phoneNumber": "+49 30 987654321",
    "isActive": true,
    "updatedAt": "2024-01-27T15:40:00Z",
    "updatedBy": "admin-id"
  },
  "timestamp": "2024-01-27T15:40:00Z"
}
```

### 5. Delete User (Soft Delete)
```
DELETE /api/admin/users/{userId}
```

**Request:**
```
DELETE /api/admin/users/550e8400-e29b-41d4-a716-446655440001
Headers:
  Authorization: Bearer <jwt-token>
  X-Tenant-ID: <tenant-id>
```

**Response (204 No Content or 200 OK):**
```json
{
  "success": true,
  "message": "User deleted successfully",
  "timestamp": "2024-01-27T15:45:00Z"
}
```

### 6. Search Users
```
GET /api/admin/users/search?q=query
```

**Request:**
```
GET /api/admin/users/search?q=john&limit=10
Headers:
  Authorization: Bearer <jwt-token>
  X-Tenant-ID: <tenant-id>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "john.doe@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "timestamp": "2024-01-27T15:50:00Z"
}
```

### 7. Get User Addresses
```
GET /api/admin/users/{userId}/addresses
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": "addr-001",
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "addressType": "billing",
      "streetAddress": "Hauptstraße 123",
      "city": "Berlin",
      "postalCode": "10115",
      "country": "DE",
      "recipientName": "John Doe",
      "phoneNumber": "+49 30 123456789",
      "isDefault": true,
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "timestamp": "2024-01-27T15:55:00Z"
}
```

### 8. Create Address
```
POST /api/admin/users/{userId}/addresses
```

**Request:**
```json
{
  "addressType": "shipping",
  "streetAddress": "Nebenstraße 456",
  "city": "Munich",
  "postalCode": "80331",
  "country": "DE",
  "recipientName": "John Doe",
  "phoneNumber": "+49 89 111222333",
  "isDefault": false
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": "addr-002",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "addressType": "shipping",
    "streetAddress": "Nebenstraße 456",
    "city": "Munich",
    "postalCode": "80331",
    "country": "DE",
    "recipientName": "John Doe",
    "phoneNumber": "+49 89 111222333",
    "isDefault": false,
    "createdAt": "2024-01-27T16:00:00Z"
  },
  "timestamp": "2024-01-27T16:00:00Z"
}
```

### 9. Update Address
```
PUT /api/admin/users/{userId}/addresses/{addressId}
```

### 10. Delete Address
```
DELETE /api/admin/users/{userId}/addresses/{addressId}
```

### 11. Verify Email
```
POST /api/admin/users/{userId}/verify-email
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Email verified",
  "timestamp": "2024-01-27T16:05:00Z"
}
```

### 12. Reset Password
```
POST /api/admin/users/{userId}/reset-password
```

**Request:**
```json
{
  "newPassword": "NewSecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset successfully",
  "timestamp": "2024-01-27T16:10:00Z"
}
```

---

## 🏗️ Implementation Steps

### Schritt 1: Create Controller
**Datei:** `/backend/BoundedContexts/Admin/API/Controllers/UsersController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using B2X.Shared.User.Interfaces;
using MediatR;

namespace B2X.Admin.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IMediator mediator,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "name",
        [FromQuery] string? sortOrder = "asc",
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();
        
        // Get users from repository
        var (users, total) = await _userRepository.GetPagedAsync(
            tenantId, 
            page, 
            pageSize, 
            sortBy, 
            sortOrder, 
            ct);

        return Ok(new
        {
            success = true,
            data = users,
            pagination = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalPages = (total + pageSize - 1) / pageSize,
                totalCount = total,
                hasNextPage = page * pageSize < total,
                hasPreviousPage = page > 1
            },
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult> GetUser(
        [FromRoute] Guid userId,
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();
        var user = await _userRepository.GetByIdAsync(tenantId, userId, ct);

        if (user == null)
            return NotFound(new { success = false, message = "User not found" });

        return Ok(new
        {
            success = true,
            data = user,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();
        var userId = User.GetUserId();

        // Validation
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        // Create user via command handler
        var command = new CreateUserCommand(
            tenantId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Password);

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetUser), new { userId = result.Data.Id }, new
        {
            success = true,
            data = result.Data,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> UpdateUser(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();
        var currentUserId = User.GetUserId();

        var command = new UpdateUserCommand(
            tenantId,
            userId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.IsActive);

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new
        {
            success = true,
            data = result.Data,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUser(
        [FromRoute] Guid userId,
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();
        var currentUserId = User.GetUserId();

        var command = new DeleteUserCommand(tenantId, userId);
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new
        {
            success = true,
            message = "User deleted successfully",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchUsers(
        [FromQuery] string q,
        [FromQuery] int limit = 10,
        CancellationToken ct = default)
    {
        var tenantId = User.GetTenantId();

        var users = await _userRepository.SearchAsync(
            tenantId,
            q,
            limit,
            ct);

        return Ok(new
        {
            success = true,
            data = users,
            timestamp = DateTime.UtcNow
        });
    }

    // ... Weitere Actions für Addresses, Verify Email, Reset Password
}
```

### Schritt 2: Create DTOs

**Datei:** `/backend/BoundedContexts/Admin/API/Requests/CreateUserRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace B2X.Admin.API.Requests;

public record CreateUserRequest(
    [Required] string Email,
    [Required] string FirstName,
    [Required] string LastName,
    string? PhoneNumber,
    [Required] string Password,
    CreateUserProfileRequest? Profile = null);

public record CreateUserProfileRequest(
    string? CompanyName,
    string? JobTitle,
    string? PreferredLanguage = "de",
    string? Timezone = "Europe/Berlin",
    bool ReceiveNewsletter = true,
    bool ReceivePromotionalEmails = false);
```

### Schritt 3: Implement Repository
**Datei:** `/backend/BoundedContexts/Shared/User/src/Infrastructure/Repositories/UserRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using B2X.Shared.User.Core.Models;
using B2X.Shared.User.Core.Interfaces;
using B2X.Shared.User.Infrastructure.Data;

namespace B2X.Shared.User.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        Guid tenantId, 
        Guid userId, 
        CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.Id == userId && !u.IsDeleted)
            .Include(u => u.Profile)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IEnumerable<User> Users, int Total)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        string? sortBy = "name",
        string? sortOrder = "asc",
        CancellationToken ct = default)
    {
        var query = _context.Users
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .AsNoTracking()
            .Include(u => u.Profile);

        // Sorting
        var isDescending = sortOrder?.ToLower() == "desc";
        query = sortBy?.ToLower() switch
        {
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "updated" => isDescending ? query.OrderByDescending(u => u.ModifiedAt) : query.OrderBy(u => u.ModifiedAt),
            _ => isDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName)
        };

        var total = await query.CountAsync(ct);
        var skip = (page - 1) * pageSize;

        var users = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return (users, total);
    }

    public async Task<User?> GetByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.Email == email && !u.IsDeleted)
            .Include(u => u.Profile)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken ct = default)
    {
        return await _context.Users
            .AnyAsync(u => u.TenantId == tenantId && u.Email == email && !u.IsDeleted, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.TenantId == tenantId && u.Id == userId, ct);

        if (user != null)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(user, ct);
        }
    }

    public async Task<IEnumerable<User>> SearchAsync(
        Guid tenantId,
        string query,
        int limit,
        CancellationToken ct = default)
    {
        var searchTerm = query.ToLower();
        
        return await _context.Users
            .Where(u => u.TenantId == tenantId && !u.IsDeleted &&
                (u.Email.ToLower().Contains(searchTerm) ||
                 u.FirstName.ToLower().Contains(searchTerm) ||
                 u.LastName.ToLower().Contains(searchTerm) ||
                 u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)))
            .AsNoTracking()
            .Take(limit)
            .ToListAsync(ct);
    }
}
```

### Schritt 4: Create DbContext
**Datei:** `/backend/BoundedContexts/Shared/User/src/Infrastructure/Data/UserDbContext.cs`

### Schritt 5: Create Migrations
```bash
cd backend/BoundedContexts/Shared/User
dotnet ef migrations add InitialUserDomain
dotnet ef database update
```

### Schritt 6: Update Program.cs
```csharp
// In Admin API Program.cs
services
    .AddSharedUserDomain()
    .AddSharedUserInfrastructure(configuration);
```

---

## 🧪 Testing

Nach der Implementierung testen mit:

```bash
# Benutzer auflisten
curl -H "Authorization: Bearer <token>" http://localhost:8080/api/admin/users

# Benutzer erstellen
curl -X POST http://localhost:8080/api/admin/users \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","firstName":"John","lastName":"Doe","password":"Password123!"}'

# Benutzer löschen
curl -X DELETE http://localhost:8080/api/admin/users/{userId} \
  -H "Authorization: Bearer <token>"
```

---

## 📝 Checkliste

- [ ] Controller erstellt
- [ ] DTOs definiert
- [ ] Repository implementiert
- [ ] DbContext erstellt
- [ ] Migrations durchgeführt
- [ ] Program.cs aktualisiert
- [ ] Endpoints getestet
- [ ] Error-Handling implementiert
- [ ] Authorization überprüft
- [ ] Tests geschrieben
- [ ] Frontend testing durchgeführt

---

**Geschätzte Dauer:** 4-6 Stunden  
**Abhängigkeiten:** Shared/User Domain bereits erstellt ✅
