# 🛒 Feature: Customer Registration Flow for Store Frontend

**Issue Type:** Epic  
**Priority:** High  
**Status:** Ready for Development  
**Target Sprint:** Q1 2026  
**Estimated Effort:** 13 Story Points (~ 2-3 Sprints)

---

## 📋 Product Requirements

### Overview
Implement a comprehensive customer registration flow in the Store Frontend that allows:
- **New Customers** to register as either Private (B2C) or Business (B2B) customers
- **Existing Customers** to log in with their credentials
- **Seamless onboarding** with progressive disclosure of required information
- **Multi-tenant support** with proper tenant isolation

### Business Goals
- Reduce cart abandonment by streamlining registration
- Capture customer type (Private vs Business) for personalized pricing and features
- Comply with GDPR and data protection requirements
- Support multi-tenant architecture with tenant-specific branding

---

## 👥 Stakeholder Alignment

### 🏗️ Software Architect Perspective
**Technical Architecture Requirements:**

#### Backend Services
- **Identity Service (Port 7002):** Handle user authentication and registration
- **Tenant Service (Port 7003):** Manage multi-tenant context
- **Store Gateway (Port 8000):** Route requests from Store Frontend

#### Data Model
```typescript
interface CustomerRegistration {
  // Common fields
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  customerType: 'PRIVATE' | 'BUSINESS';
  
  // Private customer specific
  dateOfBirth?: Date;
  gender?: 'MALE' | 'FEMALE' | 'OTHER' | 'PREFER_NOT_TO_SAY';
  
  // Business customer specific
  companyName?: string;
  vatNumber?: string;
  taxId?: string;
  commercialRegisterNumber?: string;
  
  // Address (required for business, optional for private)
  billingAddress?: Address;
  
  // Marketing
  newsletterOptIn: boolean;
  termsAccepted: boolean;
  privacyPolicyAccepted: boolean;
}

interface Address {
  street: string;
  houseNumber: string;
  addressLine2?: string;
  postalCode: string;
  city: string;
  country: string;
  addressType: AddressType; // enum from Core layer
}

enum AddressType {
  BILLING = 'Billing',
  SHIPPING = 'Shipping',
  RESIDENTIAL = 'Residential',
  COMMERCIAL = 'Commercial'
}
```

#### Security Requirements
- **PII Encryption:** Email, firstName, lastName, dateOfBirth encrypted at rest (AES-256)
- **Password Hashing:** bcrypt with minimum 12 rounds
- **JWT Authentication:** HS256, 1-hour access token, 7-day refresh token
- **Rate Limiting:** Max 5 registration attempts per IP per hour
- **CSRF Protection:** Token validation on form submission
- **Tenant Isolation:** All queries filtered by tenantId from JWT claims

#### API Endpoints
```
POST /api/auth/register/start           # Initialize registration flow
POST /api/auth/register/validate-email  # Check email availability
POST /api/auth/register/complete        # Complete registration
POST /api/auth/login                    # Existing customer login
POST /api/auth/verify-email             # Email verification after registration
```

---

### 🎨 UX Expert Perspective
**User Experience Requirements:**

#### User Flow - New Customer Registration

**Step 1: Customer Type Selection**
```
┌─────────────────────────────────────────┐
│  Welcome! How would you like to shop?  │
│                                         │
│  ┌──────────────┐  ┌──────────────┐   │
│  │ 🧑 Private   │  │ 🏢 Business  │   │
│  │              │  │              │   │
│  │ Shop as an   │  │ Shop for     │   │
│  │ individual   │  │ your company │   │
│  └──────────────┘  └──────────────┘   │
│                                         │
│  Already have an account? [Log in]     │
└─────────────────────────────────────────┘
```

**Step 2a: Private Customer Registration**
```
┌─────────────────────────────────────────┐
│  Create Your Account (Private)          │
│                                         │
│  Email *         [________________]     │
│  Password *      [________________]     │
│  First Name *    [________________]     │
│  Last Name *     [________________]     │
│  Date of Birth   [DD/MM/YYYY]          │
│  Gender          [▼ Select...]         │
│                                         │
│  ☐ Subscribe to newsletter             │
│  ☑ I accept the Terms & Conditions *   │
│  ☑ I accept the Privacy Policy *       │
│                                         │
│  [← Back]           [Create Account →] │
└─────────────────────────────────────────┘
```

**Step 2b: Business Customer Registration**
```
┌─────────────────────────────────────────┐
│  Create Business Account                │
│                                         │
│  Company Information                    │
│  Company Name *       [________________]│
│  VAT Number          [________________]│
│  Tax ID              [________________]│
│  Commercial Register [________________]│
│                                         │
│  Contact Person                         │
│  Email *             [________________]│
│  Password *          [________________]│
│  First Name *        [________________]│
│  Last Name *         [________________]│
│                                         │
│  Billing Address *                      │
│  Street *            [________________]│
│  House Number *      [___]             │
│  Postal Code *       [_______]         │
│  City *              [________________]│
│  Country *           [▼ Germany]       │
│                                         │
│  ☐ Subscribe to newsletter             │
│  ☑ I accept the Terms & Conditions *   │
│  ☑ I accept the Privacy Policy *       │
│                                         │
│  [← Back]           [Create Account →] │
└─────────────────────────────────────────┘
```

**Step 3: Email Verification**
```
┌─────────────────────────────────────────┐
│  ✉️ Verify Your Email                   │
│                                         │
│  We've sent a verification email to:    │
│  john.doe@example.com                   │
│                                         │
│  Please click the link in the email     │
│  to activate your account.              │
│                                         │
│  Didn't receive it?                     │
│  [Resend verification email]            │
│                                         │
│  [Go to Login]                          │
└─────────────────────────────────────────┘
```

#### Existing Customer Login Flow
```
┌─────────────────────────────────────────┐
│  Welcome Back!                          │
│                                         │
│  Email         [________________]       │
│  Password      [________________]       │
│                                         │
│  ☐ Remember me                          │
│  [Forgot password?]                     │
│                                         │
│  [Login]                                │
│                                         │
│  Don't have an account?                 │
│  [Create new account]                   │
└─────────────────────────────────────────┘
```

#### UX Principles
1. **Progressive Disclosure:** Only show fields relevant to customer type
2. **Inline Validation:** Real-time feedback on email availability, password strength
3. **Clear Error Messages:** Specific, actionable error messages
4. **Accessibility:** WCAG 2.1 AA compliant, keyboard navigation, screen reader support
5. **Mobile-First:** Responsive design, optimized for touch
6. **Performance:** Form submission < 500ms, validation < 100ms

#### Design Tokens (Tenant-Specific)
```scss
// Customer type selection
--customer-type-card-hover: var(--primary-color);
--customer-type-border: var(--border-color);

// Form elements
--input-border: var(--border-color);
--input-focus-border: var(--primary-color);
--error-color: #dc2626;
--success-color: #16a34a;

// Typography
--heading-font: var(--font-primary);
--body-font: var(--font-secondary);
```

---

## 📊 User Stories & Acceptance Criteria

### Epic Breakdown (13 Story Points Total)

#### Story 1: Customer Type Selection (2 SP)
**As a** new customer  
**I want to** choose between Private and Business account types  
**So that** I get a registration form tailored to my needs

**Acceptance Criteria:**
- [ ] Display two clearly distinct options: "Private Customer" and "Business Customer"
- [ ] Each option shows icon, title, and short description
- [ ] Clicking an option highlights it and enables "Continue" button
- [ ] "Already have an account?" link redirects to login page
- [ ] Selection is stored in localStorage to preserve on navigation back
- [ ] Mobile: Cards stack vertically, touch-friendly sizing (min 48x48px)

**Technical Tasks:**
- [ ] Create `CustomerTypeSelection.vue` component
- [ ] Add route `/register/customer-type`
- [ ] Implement localStorage persistence
- [ ] Add unit tests (selection, navigation, localStorage)

---

#### Story 2: Private Customer Registration Form (3 SP)
**As a** private customer  
**I want to** fill out a simple registration form  
**So that** I can create an account quickly

**Acceptance Criteria:**
- [ ] Form displays: Email, Password, First Name, Last Name, Date of Birth (optional), Gender (optional)
- [ ] Email field validates format on blur (RFC 5322 compliant)
- [ ] Password field shows strength indicator (weak/medium/strong)
- [ ] Password requirements visible: min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special char
- [ ] All required fields marked with asterisk (*)
- [ ] Terms & Conditions checkbox must be checked to submit
- [ ] Privacy Policy checkbox must be checked to submit
- [ ] Newsletter opt-in is optional and unchecked by default
- [ ] "Create Account" button disabled until all required fields valid
- [ ] Inline validation shows errors below each field

**Technical Tasks:**
- [ ] Create `PrivateCustomerForm.vue` component
- [ ] Implement FluentValidation on backend: `RegisterPrivateCustomerValidator.cs`
- [ ] Add password strength calculation utility
- [ ] API endpoint: `POST /api/auth/register/private`
- [ ] Encrypt PII fields (email, firstName, lastName, dateOfBirth) before saving
- [ ] Add unit tests (validation, submission, error handling)
- [ ] Add E2E test (happy path registration)

---

#### Story 3: Business Customer Registration Form (3 SP)
**As a** business customer  
**I want to** provide company details during registration  
**So that** I can access B2B pricing and features

**Acceptance Criteria:**
- [ ] Form displays: Company Name, VAT Number, Tax ID, Commercial Register Number
- [ ] Contact person fields: Email, Password, First Name, Last Name
- [ ] Billing address fields: Street, House Number, Postal Code, City, Country
- [ ] VAT Number validation: format check (EU VAT number format)
- [ ] All company and contact fields required, marked with asterisk (*)
- [ ] Billing address marked as required
- [ ] Country dropdown pre-populated with tenant's default country
- [ ] Terms & Conditions and Privacy Policy checkboxes required
- [ ] Newsletter opt-in optional
- [ ] "Create Account" button disabled until all required fields valid

**Technical Tasks:**
- [ ] Create `BusinessCustomerForm.vue` component
- [ ] Implement FluentValidation: `RegisterBusinessCustomerValidator.cs`
- [ ] Add VAT number format validator (EU standards)
- [ ] API endpoint: `POST /api/auth/register/business`
- [ ] Create `Address` entity with `AddressType` enum
- [ ] Encrypt PII fields (email, firstName, lastName, companyName)
- [ ] Add unit tests (validation, VAT check, submission)
- [ ] Add E2E test (business registration flow)

---

#### Story 4: Email Verification Flow (2 SP)
**As a** newly registered customer  
**I want to** verify my email address  
**So that** my account is activated and secure

**Acceptance Criteria:**
- [ ] After registration, display "Check your email" message
- [ ] Show the email address where verification was sent
- [ ] "Resend verification email" button with 60-second cooldown
- [ ] Verification email contains unique token (JWT with 24-hour expiry)
- [ ] Clicking verification link redirects to `/verify-email?token=...`
- [ ] Success page displays "Email verified! You can now log in"
- [ ] Invalid/expired token shows error with option to resend
- [ ] Verified users can log in immediately

**Technical Tasks:**
- [ ] Create `EmailVerification.vue` component
- [ ] API endpoints:
  - [ ] `POST /api/auth/verify-email` (verify token)
  - [ ] `POST /api/auth/resend-verification` (resend email)
- [ ] Implement email sending service (SendGrid/Azure Communication Services)
- [ ] Create email template: `verification-email.html`
- [ ] Add token generation and validation logic
- [ ] Add unit tests (token validation, expiry)
- [ ] Add E2E test (full verification flow)

---

#### Story 5: Existing Customer Login (1 SP)
**As an** existing customer  
**I want to** log in with my credentials  
**So that** I can access my account and continue shopping

**Acceptance Criteria:**
- [ ] Form displays: Email, Password
- [ ] "Remember me" checkbox (optional, default unchecked)
- [ ] "Forgot password?" link redirects to password reset
- [ ] Invalid credentials show error: "Invalid email or password"
- [ ] Successful login redirects to previous page or dashboard
- [ ] JWT token stored in httpOnly cookie (not localStorage)
- [ ] Tenant ID extracted from user profile after login

**Technical Tasks:**
- [ ] Create `CustomerLogin.vue` component (separate from admin login)
- [ ] Update route: `/login` for Store Frontend
- [ ] API endpoint: `POST /api/auth/login` (existing, ensure works for customers)
- [ ] Implement "Remember me" (extend refresh token to 30 days)
- [ ] Add rate limiting: max 5 login attempts per IP per 15 minutes
- [ ] Add unit tests (login, error handling)
- [ ] Add E2E test (login flow)

---

#### Story 6: Real-time Email Availability Check (1 SP)
**As a** new customer  
**I want to** know immediately if my email is already registered  
**So that** I don't waste time filling out the form

**Acceptance Criteria:**
- [ ] Email field triggers availability check on blur (not on every keystroke)
- [ ] Debounce check by 500ms to avoid excessive API calls
- [ ] Show spinner during check
- [ ] Display "✓ Email available" in green if not registered
- [ ] Display "✗ Email already registered. [Log in instead]" in red if exists
- [ ] Link in error message redirects to login page
- [ ] Check respects tenant isolation (email may exist in different tenant)

**Technical Tasks:**
- [ ] API endpoint: `POST /api/auth/check-email-availability`
  - Request: `{ email: string, tenantId: string }`
  - Response: `{ available: boolean }`
- [ ] Add debounce utility to Vue component
- [ ] Implement loading state and error handling
- [ ] Add unit tests (API, debounce, error states)

---

#### Story 7: Password Strength Indicator (1 SP)
**As a** new customer  
**I want to** see if my password is strong enough  
**So that** my account is secure

**Acceptance Criteria:**
- [ ] Indicator displays below password field
- [ ] Three levels: Weak (red), Medium (orange), Strong (green)
- [ ] Criteria displayed:
  - "At least 8 characters"
  - "Contains uppercase letter"
  - "Contains lowercase letter"
  - "Contains number"
  - "Contains special character"
- [ ] Each met criterion shows green checkmark
- [ ] Unmet criteria show red X
- [ ] Password must be at least "Medium" to submit form
- [ ] Indicator updates in real-time on input

**Technical Tasks:**
- [ ] Create `PasswordStrengthIndicator.vue` component
- [ ] Implement password strength calculation function
- [ ] Add zxcvbn library for advanced strength checking
- [ ] Add unit tests (strength calculation, criteria validation)

---

## 🛠️ Technical Implementation Details

### Backend Architecture (Onion + DDD)

#### Domain Layer (`backend/BoundedContexts/Shared/Identity/Core/`)
```csharp
// Entities/Customer.cs
public class Customer : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } // Encrypted
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; } // Encrypted
    public string LastName { get; private set; } // Encrypted
    public CustomerType CustomerType { get; private set; }
    public bool EmailVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Business customer specific
    public string? CompanyName { get; private set; } // Encrypted
    public string? VatNumber { get; private set; }
    public string? TaxId { get; private set; }
    
    // Private customer specific
    public DateTime? DateOfBirth { get; private set; } // Encrypted
    public Gender? Gender { get; private set; }
    
    public ICollection<Address> Addresses { get; private set; }
    
    // Factory methods
    public static Customer CreatePrivateCustomer(
        Guid tenantId, string email, string passwordHash,
        string firstName, string lastName, 
        DateTime? dateOfBirth, Gender? gender)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            CustomerType = CustomerType.PRIVATE,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };
        
        customer.RaiseEvent(new CustomerRegisteredEvent(customer.Id, customer.Email));
        return customer;
    }
    
    public static Customer CreateBusinessCustomer(
        Guid tenantId, string email, string passwordHash,
        string firstName, string lastName,
        string companyName, string? vatNumber, string? taxId,
        Address billingAddress)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            CustomerType = CustomerType.BUSINESS,
            CompanyName = companyName,
            VatNumber = vatNumber,
            TaxId = taxId,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            Addresses = new List<Address> { billingAddress }
        };
        
        customer.RaiseEvent(new CustomerRegisteredEvent(customer.Id, customer.Email));
        return customer;
    }
    
    public void VerifyEmail()
    {
        EmailVerified = true;
        RaiseEvent(new EmailVerifiedEvent(Id, Email));
    }
}

// Enums (use enums for restricted types!)
public enum CustomerType
{
    PRIVATE,
    BUSINESS
}

public enum Gender
{
    MALE,
    FEMALE,
    OTHER,
    PREFER_NOT_TO_SAY
}

public enum AddressType
{
    BILLING,
    SHIPPING,
    RESIDENTIAL,
    COMMERCIAL
}
```

#### Application Layer (CQRS)
```csharp
// Commands/RegisterPrivateCustomerCommand.cs
public record RegisterPrivateCustomerCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    Gender? Gender,
    bool NewsletterOptIn,
    bool TermsAccepted,
    bool PrivacyPolicyAccepted
) : IRequest<Result<CustomerDto>>;

// Validators/RegisterPrivateCustomerValidator.cs
public class RegisterPrivateCustomerValidator : AbstractValidator<RegisterPrivateCustomerCommand>
{
    public RegisterPrivateCustomerValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain number")
            .Matches(@"[\W_]").WithMessage("Password must contain special character");
            
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);
            
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
            
        RuleFor(x => x.TermsAccepted)
            .Equal(true).WithMessage("You must accept the Terms & Conditions");
            
        RuleFor(x => x.PrivacyPolicyAccepted)
            .Equal(true).WithMessage("You must accept the Privacy Policy");
    }
}

// Handlers/RegisterPrivateCustomerHandler.cs
public class RegisterPrivateCustomerHandler 
    : IRequestHandler<RegisterPrivateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEncryptionService _encryptionService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    
    public async Task<Result<CustomerDto>> Handle(
        RegisterPrivateCustomerCommand request, 
        CancellationToken cancellationToken)
    {
        // Check email availability (with tenant isolation)
        var tenantId = GetCurrentTenantId();
        var existingCustomer = await _repository.GetByEmailAsync(tenantId, request.Email);
        if (existingCustomer != null)
            return Result<CustomerDto>.Failure("Email already registered");
        
        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        
        // Create customer entity
        var customer = Customer.CreatePrivateCustomer(
            tenantId,
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender
        );
        
        // Save to database (encryption happens in EF Core value converter)
        await _repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Send verification email
        var verificationToken = GenerateEmailVerificationToken(customer.Id);
        await _emailService.SendVerificationEmailAsync(
            customer.Email, 
            customer.FirstName, 
            verificationToken
        );
        
        // Audit log
        await _auditService.LogAsync(
            tenantId, 
            customer.Id, 
            "Customer", 
            "CREATE", 
            after: customer
        );
        
        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }
}
```

#### Infrastructure Layer
```csharp
// Data/Configurations/CustomerConfiguration.cs
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasConversion(
                v => EncryptionService.Encrypt(v),
                v => EncryptionService.Decrypt(v)
            );
        
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                v => EncryptionService.Encrypt(v),
                v => EncryptionService.Decrypt(v)
            );
        
        builder.Property(c => c.CustomerType)
            .HasConversion<string>() // Store as string in DB
            .IsRequired();
        
        builder.Property(c => c.Gender)
            .HasConversion<string>();
        
        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey("CustomerId");
        
        // Indexes
        builder.HasIndex(c => new { c.TenantId, c.Email }).IsUnique();
        builder.HasIndex(c => c.EmailVerified);
    }
}
```

### Frontend Architecture (Vue 3 Composition API)

#### Component Structure
```
Frontend/Store/src/
├── views/
│   ├── CustomerTypeSelection.vue
│   ├── PrivateCustomerRegistration.vue
│   ├── BusinessCustomerRegistration.vue
│   ├── EmailVerification.vue
│   └── CustomerLogin.vue
├── components/
│   ├── PasswordStrengthIndicator.vue
│   ├── AddressForm.vue
│   └── RegistrationSuccess.vue
├── composables/
│   ├── useCustomerRegistration.ts
│   ├── useEmailValidation.ts
│   └── usePasswordStrength.ts
├── services/
│   └── api/
│       └── customerAuth.ts
├── stores/
│   └── customerAuth.ts
└── types/
    └── customer.ts
```

#### Example: PasswordStrengthIndicator.vue
```vue
<script setup lang="ts">
import { computed } from 'vue'
import { usePasswordStrength } from '@/composables/usePasswordStrength'

const props = defineProps<{
  password: string
}>()

const { strength, criteria, score } = usePasswordStrength(computed(() => props.password))

const strengthColor = computed(() => {
  if (score.value < 40) return 'red'
  if (score.value < 70) return 'orange'
  return 'green'
})

const strengthLabel = computed(() => {
  if (score.value < 40) return 'Weak'
  if (score.value < 70) return 'Medium'
  return 'Strong'
})
</script>

<template>
  <div class="password-strength">
    <div class="strength-bar">
      <div 
        class="strength-fill" 
        :style="{ width: `${score}%`, backgroundColor: strengthColor }"
      />
    </div>
    <p class="strength-label" :style="{ color: strengthColor }">
      {{ strengthLabel }}
    </p>
    
    <ul class="criteria-list">
      <li v-for="criterion in criteria" :key="criterion.label" :class="criterion.met ? 'met' : 'unmet'">
        <span class="icon">{{ criterion.met ? '✓' : '✗' }}</span>
        {{ criterion.label }}
      </li>
    </ul>
  </div>
</template>

<style scoped>
.strength-bar {
  height: 4px;
  background: var(--border-color);
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 8px;
}

.strength-fill {
  height: 100%;
  transition: width 0.3s ease, background-color 0.3s ease;
}

.criteria-list {
  list-style: none;
  padding: 0;
  font-size: 0.875rem;
}

.criteria-list li {
  margin: 4px 0;
}

.criteria-list .met {
  color: var(--success-color);
}

.criteria-list .unmet {
  color: var(--error-color);
}
</style>
```

#### Composable: useCustomerRegistration.ts
```typescript
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { customerAuthService } from '@/services/api/customerAuth'
import type { RegisterPrivateCustomerRequest, RegisterBusinessCustomerRequest } from '@/types/customer'

export function useCustomerRegistration() {
  const router = useRouter()
  const loading = ref(false)
  const error = ref<string | null>(null)
  
  const registerPrivateCustomer = async (data: RegisterPrivateCustomerRequest) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await customerAuthService.registerPrivate(data)
      
      // Redirect to email verification page
      router.push({
        name: 'EmailVerification',
        params: { email: data.email }
      })
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      throw err
    } finally {
      loading.value = false
    }
  }
  
  const registerBusinessCustomer = async (data: RegisterBusinessCustomerRequest) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await customerAuthService.registerBusiness(data)
      
      router.push({
        name: 'EmailVerification',
        params: { email: data.email }
      })
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      throw err
    } finally {
      loading.value = false
    }
  }
  
  return {
    loading,
    error,
    registerPrivateCustomer,
    registerBusinessCustomer
  }
}
```

---

## 🧪 Testing Strategy

### Backend Tests

#### Unit Tests
```csharp
// RegisterPrivateCustomerHandlerTests.cs
[Fact]
public async Task Handle_ValidRequest_CreatesCustomerSuccessfully()
{
    // Arrange
    var command = new RegisterPrivateCustomerCommand(
        Email: "test@example.com",
        Password: "SecurePass123!",
        FirstName: "John",
        LastName: "Doe",
        DateOfBirth: new DateTime(1990, 1, 1),
        Gender: Gender.MALE,
        NewsletterOptIn: true,
        TermsAccepted: true,
        PrivacyPolicyAccepted: true
    );
    
    _mockRepository
        .Setup(x => x.GetByEmailAsync(It.IsAny<Guid>(), command.Email))
        .ReturnsAsync((Customer?)null);
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Data.Email.Should().Be(command.Email);
    
    _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    _mockEmailService.Verify(x => x.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
}

[Fact]
public async Task Handle_DuplicateEmail_ReturnsFailure()
{
    // Arrange
    var existingCustomer = new Customer { /* ... */ };
    
    _mockRepository
        .Setup(x => x.GetByEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()))
        .ReturnsAsync(existingCustomer);
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("already registered");
}
```

#### Integration Tests
```csharp
// CustomerRegistrationIntegrationTests.cs
[Fact]
public async Task RegisterPrivateCustomer_ValidInput_ReturnsCreated()
{
    // Arrange
    var request = new
    {
        email = "newcustomer@example.com",
        password = "SecurePass123!",
        firstName = "John",
        lastName = "Doe",
        dateOfBirth = "1990-01-01",
        gender = "MALE",
        newsletterOptIn = false,
        termsAccepted = true,
        privacyPolicyAccepted = true
    };
    
    var httpRequest = _fixture.CreateRequest(HttpMethod.Post, "/api/auth/register/private");
    httpRequest.Content = new StringContent(
        JsonSerializer.Serialize(request),
        Encoding.UTF8,
        "application/json"
    );
    
    // Act
    var response = await _fixture.Client.SendAsync(httpRequest);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    
    var responseBody = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<CustomerDto>(responseBody);
    result.Email.Should().Be(request.email);
    result.EmailVerified.Should().BeFalse();
}
```

### Frontend Tests

#### Component Unit Tests (Vitest)
```typescript
// PasswordStrengthIndicator.spec.ts
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import PasswordStrengthIndicator from '@/components/PasswordStrengthIndicator.vue'

describe('PasswordStrengthIndicator', () => {
  it('shows weak for short password', () => {
    const wrapper = mount(PasswordStrengthIndicator, {
      props: { password: 'abc' }
    })
    
    expect(wrapper.find('.strength-label').text()).toBe('Weak')
    expect(wrapper.find('.strength-fill').attributes('style')).toContain('red')
  })
  
  it('shows strong for complex password', () => {
    const wrapper = mount(PasswordStrengthIndicator, {
      props: { password: 'MySecureP@ssw0rd!' }
    })
    
    expect(wrapper.find('.strength-label').text()).toBe('Strong')
    expect(wrapper.find('.strength-fill').attributes('style')).toContain('green')
  })
  
  it('displays all criteria', () => {
    const wrapper = mount(PasswordStrengthIndicator, {
      props: { password: 'Test123!' }
    })
    
    const criteria = wrapper.findAll('.criteria-list li')
    expect(criteria).toHaveLength(5)
    expect(criteria[0].text()).toContain('At least 8 characters')
  })
})
```

#### E2E Tests (Playwright)
```typescript
// customer-registration.spec.ts
import { test, expect } from '@playwright/test'

test.describe('Private Customer Registration', () => {
  test('complete registration flow', async ({ page }) => {
    // Navigate to registration
    await page.goto('/register/customer-type')
    
    // Select private customer
    await page.click('[data-testid="customer-type-private"]')
    await page.click('[data-testid="continue-button"]')
    
    // Fill registration form
    await page.fill('[data-testid="email"]', 'newcustomer@example.com')
    await page.fill('[data-testid="password"]', 'SecurePass123!')
    await page.fill('[data-testid="firstName"]', 'John')
    await page.fill('[data-testid="lastName"]', 'Doe')
    
    // Accept terms
    await page.check('[data-testid="terms-checkbox"]')
    await page.check('[data-testid="privacy-checkbox"]')
    
    // Submit form
    await page.click('[data-testid="submit-button"]')
    
    // Verify redirect to email verification
    await expect(page).toHaveURL('/verify-email')
    await expect(page.locator('h1')).toContainText('Verify Your Email')
  })
  
  test('shows error for duplicate email', async ({ page }) => {
    await page.goto('/register/private')
    
    await page.fill('[data-testid="email"]', 'existing@example.com')
    await page.fill('[data-testid="password"]', 'SecurePass123!')
    
    // Blur email field to trigger availability check
    await page.locator('[data-testid="email"]').blur()
    
    // Wait for error message
    await expect(page.locator('[data-testid="email-error"]')).toContainText('Email already registered')
  })
})
```

---

## 🔒 Security Checklist

- [ ] **P0.1 - No hardcoded secrets:** All JWT secrets, encryption keys in Azure Key Vault
- [ ] **P0.2 - CORS configured:** Only allow Store Frontend origin (environment-specific)
- [ ] **P0.3 - PII encrypted:** Email, firstName, lastName, dateOfBirth, companyName encrypted at rest
- [ ] **P0.4 - Audit logging:** All customer registrations logged with timestamp, IP, user-agent
- [ ] **Rate limiting:** Max 5 registration attempts per IP per hour
- [ ] **Password hashing:** bcrypt with 12 rounds minimum
- [ ] **Input validation:** Server-side validation with FluentValidation
- [ ] **SQL injection prevention:** EF Core parameterized queries only
- [ ] **XSS prevention:** Content-Security-Policy headers, input sanitization
- [ ] **CSRF protection:** Anti-forgery tokens on all POST requests
- [ ] **Email verification:** Prevent unverified accounts from logging in
- [ ] **Tenant isolation:** All queries filtered by tenantId from JWT claims

---

## 📅 Timeline & Dependencies

### Sprint 1 (Week 1-2)
- [ ] Story 1: Customer Type Selection
- [ ] Story 2: Private Customer Registration Form
- [ ] Story 7: Password Strength Indicator

### Sprint 2 (Week 3-4)
- [ ] Story 3: Business Customer Registration Form
- [ ] Story 4: Email Verification Flow
- [ ] Story 6: Email Availability Check

### Sprint 3 (Week 5-6)
- [ ] Story 5: Existing Customer Login
- [ ] Integration testing across all flows
- [ ] E2E tests (Playwright)
- [ ] Security audit
- [ ] Performance optimization

### Dependencies
- **Identity Service:** Must support customer entity and endpoints
- **Email Service:** SendGrid or Azure Communication Services configured
- **Tenant Service:** Tenant resolution from domain/subdomain
- **Encryption Service:** AES-256 field-level encryption implemented
- **Design Tokens:** Theme API provides tenant-specific branding

---

## 📏 Definition of Done

### Backend
- [ ] All API endpoints implemented with CQRS pattern
- [ ] FluentValidation for all commands
- [ ] PII fields encrypted at rest
- [ ] Unit test coverage > 80%
- [ ] Integration tests for all endpoints
- [ ] Audit logging functional
- [ ] Rate limiting configured
- [ ] Security review passed

### Frontend
- [ ] All UI components implemented with Vue 3 Composition API
- [ ] Responsive design (mobile, tablet, desktop)
- [ ] WCAG 2.1 AA accessibility compliance
- [ ] Unit tests for all components (Vitest)
- [ ] E2E tests for critical flows (Playwright)
- [ ] Error handling and user feedback
- [ ] Loading states for all async operations
- [ ] Browser compatibility (Chrome, Firefox, Safari, Edge)

### Documentation
- [ ] API documentation (OpenAPI/Swagger)
- [ ] Component documentation (Storybook)
- [ ] User guide for registration flows
- [ ] Admin guide for customer management

---

## 🎯 Success Metrics

### User Experience
- **Registration completion rate:** > 70%
- **Form abandonment rate:** < 30%
- **Email verification rate:** > 85%
- **Time to complete registration:** < 3 minutes

### Technical
- **API response time:** < 500ms (p95)
- **Error rate:** < 1%
- **Uptime:** > 99.5%
- **Test coverage:** > 80%

### Business
- **New customer registrations:** Track weekly growth
- **B2B vs B2C ratio:** Monitor customer type distribution
- **Newsletter opt-in rate:** Target > 40%

---

## 📚 Related Documentation

- [User Management Requirements](../docs/APPLICATION_SPECIFICATIONS.md#user-management)
- [Security Hardening Guide](../SECURITY_HARDENING_GUIDE.md)
- [DDD Bounded Contexts](../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- [Testing Strategy](../TESTING_STRATEGY.md)
- [Frontend Architecture](../docs/ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md)

---

## 🏷️ Labels

`feature` `store-frontend` `authentication` `registration` `high-priority` `epic`

---

**Issue Created By:** Product Owner  
**Stakeholders:** Software Architect, UX Expert, Frontend Team, Backend Team  
**Next Steps:** Break down into individual tasks in Sprint Planning
