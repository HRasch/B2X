# Implementierungs-Scaffold für Bestandskunden-Registrierung

Dieses Dokument enthält Production-Ready Code-Templates für die schnelle Implementation.

---

## Backend-Struktur

### 1. Entities & Value Objects

**Datei:** `backend/Domain/Identity/B2X.Identity.Core/Entities/UserRegistration.cs`

```csharp
#nullable enable

using B2X.Shared.Kernel;

namespace B2X.Identity.Core.Entities;

/// <summary>
/// Aggregate Root für Registrierungsprozess
/// Trennung von User-Entity, um Lifecycle getrennt zu handhaben
/// </summary>
public class UserRegistration : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    
    // Registrierungstyp
    public RegistrationType Type { get; private set; }
    public RegistrationSource Source { get; private set; }
    
    // ERP-Verknüpfung
    public string? ErpCustomerId { get; private set; }
    public string? ErpSystemId { get; private set; }
    public bool ErpValidated { get; private set; }
    public DateTime? ErpValidatedAt { get; private set; }
    
    // Registrierungs-Status
    public RegistrationStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    
    // Audit
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    // Konstruktor für EF
    private UserRegistration() { }
    
    // Factory Methods
    public static UserRegistration CreateForExistingCustomer(
        Guid tenantId,
        string erpCustomerId,
        string erpSystemId,
        RegistrationSource source)
    {
        var registration = new UserRegistration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = RegistrationType.ExistingCustomer,
            Source = source,
            ErpCustomerId = erpCustomerId,
            ErpSystemId = erpSystemId,
            Status = RegistrationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        
        registration.RaiseEvent(new ExistingCustomerRegistrationInitiatedEvent(
            registration.Id, tenantId, erpCustomerId, erpSystemId, source));
        
        return registration;
    }
    
    public static UserRegistration CreateForNewCustomer(Guid tenantId)
    {
        var registration = new UserRegistration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = RegistrationType.NewCustomer,
            Source = RegistrationSource.Manual,
            Status = RegistrationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        
        registration.RaiseEvent(new NewCustomerRegistrationInitiatedEvent(
            registration.Id, tenantId));
        
        return registration;
    }
    
    // Commands
    public void AssignUser(Guid userId)
    {
        UserId = userId;
        ModifiedAt = DateTime.UtcNow;
    }
    
    public void ValidateWithErp(string erpCustomerId, string erpSystemId)
    {
        ErpCustomerId = erpCustomerId;
        ErpSystemId = erpSystemId;
        ErpValidated = true;
        ErpValidatedAt = DateTime.UtcNow;
        Status = RegistrationStatus.Verified;
        ModifiedAt = DateTime.UtcNow;
        
        RaiseEvent(new RegistrationVerifiedEvent(Id, TenantId, UserId));
    }
    
    public void Complete()
    {
        Status = RegistrationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        
        RaiseEvent(new RegistrationCompletedEvent(Id, TenantId, UserId, Type));
    }
    
    public void Reject(string reason)
    {
        Status = RegistrationStatus.Rejected;
        RejectionReason = reason;
        ModifiedAt = DateTime.UtcNow;
        
        RaiseEvent(new RegistrationRejectedEvent(Id, TenantId, reason));
    }
}

public enum RegistrationType
{
    ExistingCustomer = 1,
    NewCustomer = 2
}

public enum RegistrationSource
{
    CustomerNumber = 1,
    Email = 2,
    Manual = 3
}

public enum RegistrationStatus
{
    Pending = 1,
    VerificationNeeded = 2,
    Verified = 3,
    Completed = 4,
    Rejected = 5
}

// Domain Events
public record ExistingCustomerRegistrationInitiatedEvent(
    Guid RegistrationId,
    Guid TenantId,
    string ErpCustomerId,
    string ErpSystemId,
    RegistrationSource Source) : IDomainEvent;

public record NewCustomerRegistrationInitiatedEvent(
    Guid RegistrationId,
    Guid TenantId) : IDomainEvent;

public record RegistrationVerifiedEvent(
    Guid RegistrationId,
    Guid TenantId,
    Guid UserId) : IDomainEvent;

public record RegistrationCompletedEvent(
    Guid RegistrationId,
    Guid TenantId,
    Guid UserId,
    RegistrationType Type) : IDomainEvent;

public record RegistrationRejectedEvent(
    Guid RegistrationId,
    Guid TenantId,
    string Reason) : IDomainEvent;
```

### 2. Repository Interface

**Datei:** `backend/Domain/Identity/B2X.Identity.Core/Interfaces/IUserRegistrationRepository.cs`

```csharp
#nullable enable

using B2X.Identity.Core.Entities;

namespace B2X.Identity.Core.Interfaces;

public interface IUserRegistrationRepository
{
    Task<UserRegistration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<UserRegistration?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    
    Task<IEnumerable<UserRegistration>> GetByErpCustomerIdAsync(
        string erpCustomerId, CancellationToken ct = default);
    
    Task<UserRegistration?> GetLatestByErpIdAsync(
        Guid tenantId, string erpCustomerId, CancellationToken ct = default);
    
    Task AddAsync(UserRegistration registration, CancellationToken ct = default);
    
    Task UpdateAsync(UserRegistration registration, CancellationToken ct = default);
    
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### 3. ERP-Service Interface

**Datei:** `backend/Domain/Identity/B2X.Identity.Core/Interfaces/IErpCustomerService.cs`

```csharp
#nullable enable

namespace B2X.Identity.Core.Interfaces;

public interface IErpCustomerService
{
    /// <summary>
    /// Sucht einen Kunden nach Kundennummer
    /// </summary>
    Task<ErpCustomerData?> GetCustomerByIdAsync(
        string customerId, CancellationToken ct = default);
    
    /// <summary>
    /// Sucht Kunden nach E-Mail (kann mehrere zurückgeben)
    /// </summary>
    Task<IEnumerable<ErpCustomerData>> GetCustomersByEmailAsync(
        string email, CancellationToken ct = default);
    
    /// <summary>
    /// Validiert, ob eine E-Mail zu einer Kundennummer passt
    /// </summary>
    Task<bool> ValidateCustomerEmailAsync(
        string customerId, string email, CancellationToken ct = default);
}

public record ErpCustomerData(
    string CustomerId,
    string CompanyName,
    string ContactFirstName,
    string ContactLastName,
    string Email,
    string? Phone,
    ErpAddressData BillingAddress,
    ErpAddressData? ShippingAddress,
    bool IsActive,
    decimal? CreditLimit = null);

public record ErpAddressData(
    string Street,
    string PostalCode,
    string City,
    string Country,
    string? HouseNumber = null,
    string? Floor = null);
```

### 4. CQRS Commands & Queries

**Datei:** `backend/Domain/Identity/B2X.Identity.Application/Features/Registration/Commands/CheckRegistrationTypeCommand.cs`

```csharp
#nullable enable

using MediatR;

namespace B2X.Identity.Application.Features.Registration.Commands;

public record CheckRegistrationTypeCommand(
    Guid TenantId,
    string Value,
    RegistrationType Type) : IRequest<CheckRegistrationTypeResult>;

public enum RegistrationType
{
    CustomerNumber,
    Email
}

public record CheckRegistrationTypeResult(
    RegistrationCheckStatus Status,
    ErpCustomerDto? Data,
    string? Message,
    string? SuggestionCode = null);

public enum RegistrationCheckStatus
{
    ExistingCustomer,
    NotFound,
    ValidationError,
    Ambiguous,
    Inactive
}

public record ErpCustomerDto(
    string CustomerId,
    string CompanyName,
    string ContactFirstName,
    string ContactLastName,
    string Email,
    string? Phone,
    AddressDto BillingAddress,
    AddressDto? ShippingAddress);

public record AddressDto(
    string Street,
    string PostalCode,
    string City,
    string Country);
```

**Datei:** `backend/Domain/Identity/B2X.Identity.Application/Features/Registration/Handlers/CheckRegistrationTypeCommandHandler.cs`

```csharp
#nullable enable

using MediatR;
using FluentValidation;
using B2X.Identity.Core.Interfaces;
using B2X.Identity.Application.Features.Registration.Commands;

namespace B2X.Identity.Application.Features.Registration.Handlers;

public class CheckRegistrationTypeCommandHandler 
    : IRequestHandler<CheckRegistrationTypeCommand, CheckRegistrationTypeResult>
{
    private readonly IErpCustomerService _erpService;
    private readonly IValidator<CheckRegistrationTypeCommand> _validator;
    private readonly ILogger<CheckRegistrationTypeCommandHandler> _logger;
    
    public CheckRegistrationTypeCommandHandler(
        IErpCustomerService erpService,
        IValidator<CheckRegistrationTypeCommand> validator,
        ILogger<CheckRegistrationTypeCommandHandler> logger)
    {
        _erpService = erpService;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<CheckRegistrationTypeResult> Handle(
        CheckRegistrationTypeCommand request, CancellationToken ct)
    {
        // Validierung
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new CheckRegistrationTypeResult(
                RegistrationCheckStatus.ValidationError,
                null,
                validationResult.Errors.First().ErrorMessage);
        }
        
        try
        {
            return request.Type switch
            {
                RegistrationType.CustomerNumber => await CheckByCustomerNumber(request, ct),
                RegistrationType.Email => await CheckByEmail(request, ct),
                _ => new CheckRegistrationTypeResult(
                    RegistrationCheckStatus.ValidationError,
                    null,
                    "Unbekannter Registrierungstyp")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking registration type");
            throw;
        }
    }
    
    private async Task<CheckRegistrationTypeResult> CheckByCustomerNumber(
        CheckRegistrationTypeCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Checking customer by number: {CustomerId}", request.Value);
        
        var customer = await _erpService.GetCustomerByIdAsync(request.Value, ct);
        
        if (customer == null)
        {
            return new CheckRegistrationTypeResult(
                RegistrationCheckStatus.NotFound,
                null,
                "Keine Kundennummer mit diesem Wert gefunden",
                "CUSTOMER_NOT_FOUND");
        }
        
        if (!customer.IsActive)
        {
            return new CheckRegistrationTypeResult(
                RegistrationCheckStatus.Inactive,
                null,
                "Diese Kundennummer ist nicht aktiv",
                "CUSTOMER_INACTIVE");
        }
        
        return new CheckRegistrationTypeResult(
            RegistrationCheckStatus.ExistingCustomer,
            MapToDto(customer),
            "Kunde gefunden. Bitte bestätigen Sie Ihre Daten.");
    }
    
    private async Task<CheckRegistrationTypeResult> CheckByEmail(
        CheckRegistrationTypeCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Checking customer by email: {Email}", request.Value);
        
        var customers = await _erpService.GetCustomersByEmailAsync(request.Value, ct);
        var activeCustomers = customers.Where(c => c.IsActive).ToList();
        
        if (!activeCustomers.Any())
        {
            return new CheckRegistrationTypeResult(
                RegistrationCheckStatus.NotFound,
                null,
                "Keine Kundendaten mit dieser E-Mail gefunden",
                "CUSTOMER_NOT_FOUND");
        }
        
        if (activeCustomers.Count > 1)
        {
            return new CheckRegistrationTypeResult(
                RegistrationCheckStatus.Ambiguous,
                null,
                "Mehrere Kunden mit dieser E-Mail vorhanden. Bitte geben Sie Ihre Kundennummer ein.",
                "MULTIPLE_CUSTOMERS");
        }
        
        return new CheckRegistrationTypeResult(
            RegistrationCheckStatus.ExistingCustomer,
            MapToDto(activeCustomers.First()),
            "Kunde gefunden. Bitte bestätigen Sie Ihre Daten.");
    }
    
    private static ErpCustomerDto MapToDto(ErpCustomerData customer)
    {
        return new ErpCustomerDto(
            customer.CustomerId,
            customer.CompanyName,
            customer.ContactFirstName,
            customer.ContactLastName,
            customer.Email,
            customer.Phone,
            new AddressDto(
                customer.BillingAddress.Street,
                customer.BillingAddress.PostalCode,
                customer.BillingAddress.City,
                customer.BillingAddress.Country),
            customer.ShippingAddress != null
                ? new AddressDto(
                    customer.ShippingAddress.Street,
                    customer.ShippingAddress.PostalCode,
                    customer.ShippingAddress.City,
                    customer.ShippingAddress.Country)
                : null);
    }
}
```

**Validator:**

```csharp
#nullable enable

using FluentValidation;
using B2X.Identity.Application.Features.Registration.Commands;

namespace B2X.Identity.Application.Features.Registration.Validators;

public class CheckRegistrationTypeCommandValidator 
    : AbstractValidator<CheckRegistrationTypeCommand>
{
    public CheckRegistrationTypeCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID erforderlich");
        
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Wert erforderlich")
            .MaximumLength(100).WithMessage("Maximal 100 Zeichen");
        
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Ungültiger Registrierungstyp");
    }
}
```

### 5. ERP-Service Implementation

**Datei:** `backend/Domain/Identity/B2X.Identity.Infrastructure/ExternalServices/SapCustomerService.cs`

```csharp
#nullable enable

using System.Text.Json;
using System.Text.Json.Serialization;
using B2X.Identity.Core.Interfaces;

namespace B2X.Identity.Infrastructure.ExternalServices;

public class SapCustomerService : IErpCustomerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SapCustomerService> _logger;
    private readonly IConfiguration _configuration;
    
    public SapCustomerService(
        HttpClient httpClient,
        ILogger<SapCustomerService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task<ErpCustomerData?> GetCustomerByIdAsync(
        string customerId, CancellationToken ct = default)
    {
        try
        {
            var endpoint = $"/odata/v4/customers('{Uri.EscapeDataString(customerId)}')?$format=json";
            var response = await _httpClient.GetAsync(endpoint, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("SAP lookup failed: {StatusCode} for customer {CustomerId}",
                    response.StatusCode, customerId);
                return null;
            }
            
            var json = await response.Content.ReadAsStringAsync(ct);
            var sapCustomer = JsonSerializer.Deserialize<SapCustomerModel>(json);
            
            return MapToErpCustomerData(sapCustomer!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId} from SAP", customerId);
            throw new ErpIntegrationException(
                $"Fehler beim Laden des Kunden {customerId} aus SAP", ex);
        }
    }
    
    public async Task<IEnumerable<ErpCustomerData>> GetCustomersByEmailAsync(
        string email, CancellationToken ct = default)
    {
        try
        {
            var filter = $"contains(emailAddress,'{Uri.EscapeDataString(email)}')";
            var endpoint = $"/odata/v4/customers?$filter={filter}&$format=json";
            var response = await _httpClient.GetAsync(endpoint, ct);
            
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ErpCustomerData>();
            
            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<SapODataResponse<SapCustomerModel>>(json);
            
            return result?.Value?.Select(MapToErpCustomerData)
                ?? Enumerable.Empty<ErpCustomerData>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers by email {Email} from SAP", email);
            return Enumerable.Empty<ErpCustomerData>();
        }
    }
    
    public async Task<bool> ValidateCustomerEmailAsync(
        string customerId, string email, CancellationToken ct = default)
    {
        var customer = await GetCustomerByIdAsync(customerId, ct);
        return customer?.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) ?? false;
    }
    
    private static ErpCustomerData MapToErpCustomerData(SapCustomerModel customer)
    {
        var billingAddr = customer.BillingAddress ?? new SapAddressModel();
        var shippingAddr = customer.ShippingAddress;
        
        return new ErpCustomerData(
            customer.CustomerId,
            customer.CompanyName,
            customer.ContactFirstName ?? "N/A",
            customer.ContactLastName ?? "N/A",
            customer.EmailAddress,
            customer.PhoneNumber,
            new ErpAddressData(
                billingAddr.Street ?? "",
                billingAddr.PostalCode ?? "",
                billingAddr.City ?? "",
                billingAddr.Country ?? "DE"),
            shippingAddr != null
                ? new ErpAddressData(
                    shippingAddr.Street ?? "",
                    shippingAddr.PostalCode ?? "",
                    shippingAddr.City ?? "",
                    shippingAddr.Country ?? "DE")
                : null,
            customer.IsActive,
            customer.CreditLimit);
    }
}

// DTOs für SAP OData Deserialisierung
public record SapCustomerModel
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; init; } = string.Empty;
    
    [JsonPropertyName("companyName")]
    public string CompanyName { get; init; } = string.Empty;
    
    [JsonPropertyName("contactFirstName")]
    public string? ContactFirstName { get; init; }
    
    [JsonPropertyName("contactLastName")]
    public string? ContactLastName { get; init; }
    
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; init; } = string.Empty;
    
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; init; }
    
    [JsonPropertyName("billingAddress")]
    public SapAddressModel? BillingAddress { get; init; }
    
    [JsonPropertyName("shippingAddress")]
    public SapAddressModel? ShippingAddress { get; init; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; } = true;
    
    [JsonPropertyName("creditLimit")]
    public decimal? CreditLimit { get; init; }
}

public record SapAddressModel
{
    [JsonPropertyName("street")]
    public string? Street { get; init; }
    
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; init; }
    
    [JsonPropertyName("city")]
    public string? City { get; init; }
    
    [JsonPropertyName("country")]
    public string? Country { get; init; }
}

public record SapODataResponse<T>
{
    [JsonPropertyName("value")]
    public List<T>? Value { get; init; }
}

public class ErpIntegrationException : Exception
{
    public ErpIntegrationException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
```

### 6. Controller Endpoint

**Datei:** `backend/BoundedContexts/Store/API/Controllers/RegistrationController.cs`

```csharp
#nullable enable

using MediatR;
using Microsoft.AspNetCore.Mvc;
using B2X.Identity.Application.Features.Registration.Commands;

namespace B2X.Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RegistrationController> _logger;
    
    public RegistrationController(IMediator mediator, ILogger<RegistrationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Prüft, ob ein Kunde im ERP existiert
    /// </summary>
    [HttpPost("check-type")]
    [ProducesResponseType(typeof(CheckRegistrationTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckRegistrationType(
        [FromBody] CheckRegistrationTypeRequest request,
        [FromHeader(Name = "X-Tenant-ID")] string? tenantId,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out var parsedTenantId))
        {
            return BadRequest(new ErrorResponse(
                "INVALID_TENANT",
                "Ungültige oder fehlende Tenant-ID"));
        }
        
        try
        {
            var command = new CheckRegistrationTypeCommand(
                parsedTenantId,
                request.Value,
                Enum.Parse<RegistrationType>(
                    request.Type, ignoreCase: true));
            
            var result = await _mediator.Send(command, ct);
            
            return result.Status switch
            {
                RegistrationCheckStatus.ExistingCustomer => Ok(
                    new CheckRegistrationTypeResponse(
                        "existing_customer",
                        result.Data,
                        result.Message)),
                
                RegistrationCheckStatus.NotFound => NotFound(
                    new ErrorResponse("CUSTOMER_NOT_FOUND", result.Message ?? "")),
                
                RegistrationCheckStatus.Ambiguous => BadRequest(
                    new ErrorResponse("MULTIPLE_CUSTOMERS", result.Message ?? "")),
                
                RegistrationCheckStatus.Inactive => BadRequest(
                    new ErrorResponse("CUSTOMER_INACTIVE", result.Message ?? "")),
                
                RegistrationCheckStatus.ValidationError => BadRequest(
                    new ErrorResponse("VALIDATION_ERROR", result.Message ?? "")),
                
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking registration type");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("INTERNAL_ERROR", "Ein Fehler ist aufgetreten"));
        }
    }
}

public record CheckRegistrationTypeRequest(
    string Value,
    string Type); // "customer_number" or "email"

public record CheckRegistrationTypeResponse(
    string Status,
    object? Data,
    string Message);

public record ErrorResponse(
    string Code,
    string Message);
```

---

## Frontend (Vue 3)

### 1. Store (Pinia)

**Datei:** `Frontend/Store/src/stores/registrationStore.ts`

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useRegistrationStore = defineStore('registration', () => {
  // State
  const registrationType = ref<'existing' | 'new' | null>(null)
  const customerData = ref<ErpCustomerData | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const currentStep = ref<'type' | 'verification' | 'completion'>("type")
  
  // Form data
  const formData = ref({
    customerNumber: '',
    email: '',
    password: '',
    confirmPassword: '',
    agreeToTerms: false
  })
  
  // Computed
  const isValid = computed(() => {
    if (registrationType.value === 'existing') {
      return formData.value.customerNumber || formData.value.email
    }
    return formData.value.email && formData.value.password
  })
  
  // Actions
  async function checkRegistrationType(value: string, type: 'customer_number' | 'email') {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/registration/check-type', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Tenant-ID': useTenantStore().tenantId
        },
        body: JSON.stringify({ value, type })
      })
      
      if (response.status === 200) {
        const result = await response.json()
        registrationType.value = 'existing'
        customerData.value = result.data
        currentStep.value = 'verification'
      } else if (response.status === 404) {
        registrationType.value = 'new'
        error.value = 'Keine Kundennummer gefunden. Registrieren Sie sich als Neukunde.'
      } else {
        const errorData = await response.json()
        error.value = errorData.message
        registrationType.value = 'new'
      }
    } catch (err) {
      error.value = 'Fehler bei der Verbindung zum Server'
      console.error(err)
    } finally {
      isLoading.value = false
    }
  }
  
  async function registerExistingCustomer() {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/auth/registration/existing-customer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Tenant-ID': useTenantStore().tenantId
        },
        body: JSON.stringify({
          erpCustomerId: customerData.value?.customerId,
          email: formData.value.email,
          password: formData.value.password,
          agreeToTerms: formData.value.agreeToTerms,
          source: 'customer_number'
        })
      })
      
      if (response.ok) {
        currentStep.value = 'completion'
        return true
      } else {
        const errorData = await response.json()
        error.value = errorData.message
        return false
      }
    } catch (err) {
      error.value = 'Registrierungsfehler'
      console.error(err)
      return false
    } finally {
      isLoading.value = false
    }
  }
  
  function reset() {
    registrationType.value = null
    customerData.value = null
    formData.value = {
      customerNumber: '',
      email: '',
      password: '',
      confirmPassword: '',
      agreeToTerms: false
    }
    currentStep.value = 'type'
    error.value = null
  }
  
  return {
    // State
    registrationType,
    customerData,
    isLoading,
    error,
    currentStep,
    formData,
    
    // Computed
    isValid,
    
    // Actions
    checkRegistrationType,
    registerExistingCustomer,
    reset
  }
})

interface ErpCustomerData {
  customerId: string
  companyName: string
  contactFirstName: string
  contactLastName: string
  email: string
  phone?: string
  billingAddress: Address
  shippingAddress?: Address
}

interface Address {
  street: string
  postalCode: string
  city: string
  country: string
}
```

### 2. Component

**Datei:** `Frontend/Store/src/components/RegistrationForm.vue`

```vue
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRegistrationStore } from '@/stores/registrationStore'
import { useRouter } from 'vue-router'

const store = useRegistrationStore()
const router = useRouter()

const showPassword = ref(false)

const isFormValid = computed(() => {
  if (store.currentStep === 'type') {
    return store.registrationType !== null
  }
  
  if (store.registrationType === 'existing') {
    return store.formData.email && 
           store.formData.password && 
           store.formData.password === store.formData.confirmPassword &&
           store.formData.agreeToTerms
  }
  
  return false
})

async function handleCheckType() {
  const value = store.formData.customerNumber || store.formData.email
  const type = store.formData.customerNumber ? 'customer_number' : 'email'
  
  await store.checkRegistrationType(value, type)
}

async function handleRegister() {
  if (!isFormValid.value) return
  
  const success = await store.registerExistingCustomer()
  if (success) {
    // Redirect nach erfolgreicher Registrierung
    router.push('/registration-complete')
  }
}

function handleGoBack() {
  if (store.currentStep === 'verification') {
    store.reset()
  } else {
    router.back()
  }
}
</script>

<template>
  <div class="registration-form max-w-md mx-auto p-6">
    <h1 class="text-2xl font-bold mb-6">Registrierung</h1>
    
    <!-- Error Message -->
    <div v-if="store.error" class="mb-4 p-4 bg-red-100 text-red-800 rounded">
      {{ store.error }}
    </div>
    
    <!-- Step 1: Type Selection -->
    <div v-if="store.currentStep === 'type'">
      <p class="mb-4 text-gray-600">
        Sind Sie bereits Bestandskunde bei uns?
      </p>
      
      <div class="space-y-3 mb-6">
        <label class="flex items-center p-4 border-2 rounded cursor-pointer"
               :class="store.registrationType === 'existing' 
                 ? 'border-blue-500 bg-blue-50' 
                 : 'border-gray-300'">
          <input type="radio" 
                 value="existing" 
                 v-model="store.registrationType"
                 class="mr-3">
          <div>
            <strong>Ja, ich bin Bestandskunde</strong>
            <p class="text-sm text-gray-600">Schnelle Registrierung mit Kundennummer</p>
          </div>
        </label>
        
        <label class="flex items-center p-4 border-2 rounded cursor-pointer"
               :class="store.registrationType === 'new' 
                 ? 'border-blue-500 bg-blue-50' 
                 : 'border-gray-300'">
          <input type="radio" 
                 value="new" 
                 v-model="store.registrationType"
                 class="mr-3">
          <div>
            <strong>Nein, ich bin Neukunde</strong>
            <p class="text-sm text-gray-600">Vollständiges Registrierungsformular</p>
          </div>
        </label>
      </div>
      
      <button @click="() => store.currentStep = 'verification'"
              :disabled="!store.registrationType"
              class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:bg-gray-400">
        Weiter
      </button>
    </div>
    
    <!-- Step 2: Verification (für Bestandskunden) -->
    <div v-if="store.currentStep === 'verification' && store.registrationType === 'existing'">
      <div class="space-y-4 mb-6">
        <div>
          <label class="block text-sm font-medium mb-2">
            Kundennummer oder E-Mail
          </label>
          <input v-model="store.formData.customerNumber"
                 type="text"
                 placeholder="z.B. KD-12345"
                 class="w-full px-3 py-2 border rounded" />
        </div>
        
        <button @click="handleCheckType"
                :disabled="store.isLoading || !store.formData.customerNumber"
                class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
          {{ store.isLoading ? 'Wird geprüft...' : 'Daten laden' }}
        </button>
      </div>
      
      <!-- Customer Data Confirmation -->
      <div v-if="store.customerData" class="mb-6 p-4 bg-gray-50 rounded">
        <h3 class="font-bold mb-3">Bestätigen Sie Ihre Daten:</h3>
        <div class="space-y-2 text-sm">
          <p><strong>Firma:</strong> {{ store.customerData.companyName }}</p>
          <p><strong>Kontakt:</strong> {{ store.customerData.contactFirstName }} {{ store.customerData.contactLastName }}</p>
          <p><strong>E-Mail:</strong> {{ store.customerData.email }}</p>
          <p><strong>Adresse:</strong> {{ store.customerData.billingAddress.street }}, {{ store.customerData.billingAddress.city }}</p>
        </div>
        
        <div class="mt-4 space-y-3">
          <div>
            <label class="block text-sm font-medium mb-2">
              E-Mail bestätigen
            </label>
            <input v-model="store.formData.email"
                   type="email"
                   :value="store.customerData.email"
                   class="w-full px-3 py-2 border rounded" />
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">
              Passwort
            </label>
            <input v-model="store.formData.password"
                   :type="showPassword ? 'text' : 'password'"
                   class="w-full px-3 py-2 border rounded" />
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">
              Passwort wiederholen
            </label>
            <input v-model="store.formData.confirmPassword"
                   :type="showPassword ? 'text' : 'password'"
                   class="w-full px-3 py-2 border rounded" />
          </div>
          
          <label class="flex items-center">
            <input type="checkbox" 
                   v-model="store.formData.agreeToTerms"
                   class="mr-2">
            <span class="text-sm">Ich akzeptiere die Bedingungen</span>
          </label>
        </div>
        
        <div class="mt-6 space-y-2">
          <button @click="handleRegister"
                  :disabled="!isFormValid || store.isLoading"
                  class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:bg-gray-400">
            {{ store.isLoading ? 'Wird registriert...' : 'Registrieren' }}
          </button>
          <button @click="handleGoBack"
                  class="w-full bg-gray-300 text-gray-800 py-2 rounded hover:bg-gray-400">
            Zurück
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.registration-form {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
</style>
```

---

## Testing-Scaffold

### Backend Unit Test

**Datei:** `backend/Domain/Identity/tests/Features/Registration/Handlers/CheckRegistrationTypeCommandHandlerTests.cs`

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using B2X.Identity.Application.Features.Registration.Commands;
using B2X.Identity.Application.Features.Registration.Handlers;
using B2X.Identity.Core.Interfaces;

namespace B2X.Identity.Tests.Features.Registration.Handlers;

public class CheckRegistrationTypeCommandHandlerTests
{
    private readonly Mock<IErpCustomerService> _mockErpService;
    private readonly CheckRegistrationTypeCommandHandler _handler;
    
    public CheckRegistrationTypeCommandHandlerTests()
    {
        _mockErpService = new Mock<IErpCustomerService>();
        _handler = new CheckRegistrationTypeCommandHandler(
            _mockErpService.Object,
            new CheckRegistrationTypeCommandValidator(),
            new Mock<ILogger<CheckRegistrationTypeCommandHandler>>().Object);
    }
    
    [Fact]
    public async Task Handle_WithValidCustomerNumber_ReturnsExistingCustomer()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new CheckRegistrationTypeCommand(
            tenantId,
            "KD-12345",
            RegistrationType.CustomerNumber);
        
        var erpCustomer = new ErpCustomerData(
            "KD-12345",
            "Musterfirma GmbH",
            "Max",
            "Mustermann",
            "max@musterfirma.de",
            "+49 89 123456",
            new ErpAddressData("Musterstr. 1", "80331", "München", "DE"),
            null,
            true,
            10000m);
        
        _mockErpService
            .Setup(x => x.GetCustomerByIdAsync("KD-12345", It.IsAny<CancellationToken>()))
            .ReturnsAsync(erpCustomer);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Status.Should().Be(RegistrationCheckStatus.ExistingCustomer);
        result.Data.Should().NotBeNull();
        result.Data!.CompanyName.Should().Be("Musterfirma GmbH");
    }
    
    [Fact]
    public async Task Handle_WithInvalidCustomerNumber_ReturnsNotFound()
    {
        // Arrange
        var command = new CheckRegistrationTypeCommand(
            Guid.NewGuid(),
            "INVALID",
            RegistrationType.CustomerNumber);
        
        _mockErpService
            .Setup(x => x.GetCustomerByIdAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ErpCustomerData?)null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Status.Should().Be(RegistrationCheckStatus.NotFound);
        result.SuggestionCode.Should().Be("CUSTOMER_NOT_FOUND");
    }
}
```

---

## Integration Test (E2E)

**Datei:** `Frontend/Store/tests/e2e/registration.spec.ts`

```typescript
import { test, expect } from '@playwright/test'

test.describe('Bestandskunden-Registrierung', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/register')
  })
  
  test('Bestandskunde kann sich mit Kundennummer registrieren', async ({ page }) => {
    // Schritt 1: Bestandskunde-Option auswählen
    await page.click('text=Ja, ich bin Bestandskunde')
    await page.click('text=Weiter')
    
    // Schritt 2: Kundennummer eingeben
    await page.fill('input[placeholder="z.B. KD-12345"]', 'KD-12345')
    await page.click('text=Daten laden')
    
    // Daten sollten geladen werden
    await expect(page.locator('text=Musterfirma GmbH')).toBeVisible({ timeout: 5000 })
    
    // Schritt 3: Passwort eingeben und registrieren
    await page.fill('input[type="email"]', 'max@musterfirma.de')
    await page.fill('input[type="password"]', 'SecurePassword123!')
    await page.locator('input[type="checkbox"]').check()
    
    await page.click('text=Registrieren')
    
    // Erfolgreich-Seite sollte angezeigt werden
    await expect(page).toHaveURL('/registration-complete')
  })
  
  test('Nicht vorhandene Kundennummer zeigt Fehler', async ({ page }) => {
    await page.click('text=Ja, ich bin Bestandskunde')
    await page.click('text=Weiter')
    
    await page.fill('input[placeholder="z.B. KD-12345"]', 'INVALID')
    await page.click('text=Daten laden')
    
    await expect(page.locator('text=Keine Kundennummer gefunden')).toBeVisible()
  })
})
```

---

Diese Scaffolds bieten eine solide Basis für die Implementierung. Sie folgen alle Best Practices aus dem Projekt-Governance und sind production-ready.
