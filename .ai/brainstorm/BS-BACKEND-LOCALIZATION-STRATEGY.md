---
docid: BS-BACKEND-LOCALIZATION-STRATEGY
title: Backend Message Localization Strategy
owner: @Backend
status: Brainstorm
created: 2026-01-11
---

# Backend Message Localization Strategy

**Problem**: User-facing backend messages (API error responses, validation messages, success notifications) are not localized, leading to inconsistent user experience across supported languages (en, de, fr, es, it, pt, nl, pl).

**Goal**: Ensure all **user-facing** backend messages use translation keys and are properly localized based on request culture.

**Important**: Log messages, internal exceptions, and debugging output should remain in English for consistency across development and operations teams.

## Current State Analysis
- Backend uses .NET Core localization with `IStringLocalizer<T>`
- Resource files exist for supported languages
- Some user-facing messages are hardcoded in API responses
- No clear distinction between user-facing vs internal messages
- CLIs need language selection mechanism

## Strategy Components

### 1. Define User-Facing Message Boundaries (Week 1)
- **User-facing messages** (MUST localize):
  - API response messages (errors, success, warnings)
  - Validation error messages
  - Business rule violation messages
  - Notification messages sent to UI
  - CLI output messages and prompts
  
- **Internal messages** (Keep in English):
  - Log messages (structured logging)
  - Internal exceptions
  - Debug/trace messages
  - Developer-facing error details

### 2. Audit & Inventory (Week 1)
- **Tool**: Use Roslyn MCP to scan for hardcoded strings in:
  - Controllers/API endpoints
  - Command/Query handlers (Wolverine)
  - Validation classes
  - Result objects returned to frontend
  - CLI commands and output
- **Scope**: User-facing code paths only
- **Output**: List of files/lines with hardcoded user-facing messages
- **Action**: Create issue tickets for each violation
  
### 3. Consistency Enforcement Mechanisms (Weeks 2-4)

#### A. Naming Convention for Localization Keys
- **Pattern**: `{Module}.{Entity}.{MessageType}.{Specifics}`
- **Examples**:
  - `Catalog.Product.NotFound`
  - `Catalog.Product.ValidationFailed.InvalidPrice`
  - `Order.Payment.Success`
  - `Auth.Login.Failed.InvalidCredentials`

#### B. Dedicated Result Objects for User Messages
```csharp
// User-facing result (localized)
public class UserResult
{
    public bool IsSuccess { get; }
    public string MessageKey { get; }  // Translation key
    public object[] MessageArgs { get; }  // Parameters for translation
}

// Internal result (English logs)
public class InternalResult
{
    public bool IsSuccess { get; }
    public string ErrorDetails { get; }  // English, for logs only
}
```

#### C. Centralized Message Keys Registry
- Create `MessageKeys.cs` constants for all localization keys
- Prevents typos and enables IDE autocomplete
- Example:
```csharp
public static class CatalogMessages
{
    public const string ProductNotFound = "Catalog.Product.NotFound";
    public const string ProductCreated = "Catalog.Product.Created";
    public const string InvalidPrice = "Catalog.Product.ValidationFailed.InvalidPrice";
}
```

#### D. Implementation Pattern
- **Replace hardcoded strings** with `IStringLocalizer<T>[key, args]`
- **Injection**: Ensure `IStringLocalizer<T>` is injected in controllers, handlers, CLI commands
- **Separation**: User messages via localizer, internal logs stay English

### 4. Response Localization (Week 3)
- **API responses**: Use middleware to localize error messages before serialization
- **CQRS handlers**: Return message keys from Wolverine handlers, localize in API layer
- **Logging**: Keep logs in English with structured data for context

### 5. Resource Management & Consistency Validation (Ongoing)
- **Populate resources**: Add missing keys to all language files
- **CI validation**:
  - Check for missing keys across all locales
  - Validate key naming convention compliance
  - Detect unused keys in resource files
  - Verify MessageKeys.cs matches resource files
- **Fallback**: Default to English if translation missing
- **Review process**: New keys require translation in at least 3 languages before merge

### 6. Testing & Validation (Week 4)
- **Unit tests**: Mock localizer and verify key usage via MessageKeys constants
- **Integration tests**: Test with different `Accept-Language` headers
- **E2E**: Verify localized messages in API responses
- **Consistency tests**: Verify all user-facing endpoints return localized messages
- **CLI tests**: Test language selection and localized output

### 7. CLI Localization Support (Week 3-4)
- **Language selection**:
  - Command-line flag: `--language=de` or `-l de`
  - Environment variable: `B2X_CLI_LANGUAGE=de`
  - Config file: `~/.b2x/config.json` with `"language": "de"`
  - Interactive prompt on first run
  
- **Priority order**:
  1. Command-line flag (highest)
  2. Environment variable
  3. Config file setting
  4. System locale detection
  5. English fallback (default)

- **Implementation**:
  - Use same `IStringLocalizer<T>` infrastructure
  - Set `CultureInfo.CurrentUICulture` based on selection
  - Store preference in user config file
  - Validate language code against supported locales

### 8. Frontend Text Customization (Tenant-Level) (Week 4-5)
- **Requirement**: Admin and Store frontend texts should be customizable per tenant
- **Use cases**:
  - Brand-specific terminology (e.g., "Products" → "Articles")
  - Industry-specific language (e.g., "Cart" → "Project Folder")
  - Custom promotional messages
  - Legal/compliance text variations
  
- **Architecture**:
  - **Storage**: Database table `TenantTranslationOverrides`
    ```sql
    CREATE TABLE TenantTranslationOverrides (
      TenantId GUID,
      LanguageCode VARCHAR(5),
      MessageKey VARCHAR(255),
      CustomValue NVARCHAR(MAX),
      PRIMARY KEY (TenantId, LanguageCode, MessageKey)
    );
    ```
  
  - **Fallback chain**: 
    1. Tenant-specific custom value (highest priority)
    2. Default translation from resource files
    3. English fallback
  
  - **Caching**: Cache tenant overrides in distributed cache (Redis)
  - **API**: Provide management API for tenant admins to customize texts
  
- **Implementation**:
  ```csharp
  public class TenantAwareStringLocalizer : IStringLocalizer
  {
      private readonly IStringLocalizer _baseLocalizer;
      private readonly ITenantTranslationService _tenantService;
      private readonly IMemoryCache _cache;
      
      public LocalizedString this[string name, params object[] arguments]
      {
          get
          {
              var tenantId = _tenantContext.CurrentTenantId;
              var culture = CultureInfo.CurrentUICulture.Name;
              
              // Check tenant override first
              var customValue = _tenantService.GetOverride(tenantId, culture, name);
              if (customValue != null)
                  return new LocalizedString(name, string.Format(customValue, arguments));
              
              // Fallback to default
              return _baseLocalizer[name, arguments];
          }
      }
  }
  ```

- **UI Features**:
  - Text editor in Admin panel for managing custom translations
  - Preview mode to see changes before publishing
  - Bulk import/export of custom translations
  - Search and filter by key or value
  - Reset to default option per key
  - **Translation key inspector** (debug mode):
    - Toggle button to enable/disable inspector mode
    - Shows translation key on hover or click
    - Visual indicator (border, tooltip) for translatable elements
    - Copy key to clipboard functionality
    - Jump to translation editor for that key
  
- **Developer/Debug Mode Features**:
  ```javascript
  // Vue.js implementation example
  // Enable via query parameter: ?i18n-debug=true
  // Or localStorage flag: localStorage.setItem('i18n-debug', 'true')
  
  const { t } = useI18n();
  const isDebugMode = ref(localStorage.getItem('i18n-debug') === 'true');
  
  // Custom directive or composable
  const vI18nDebug = {
    mounted(el, binding) {
      if (isDebugMode.value) {
        el.setAttribute('data-i18n-key', binding.value);
        el.style.outline = '1px dashed orange';
        el.title = `i18n: ${binding.value}`;
        el.addEventListener('click', (e) => {
          if (e.altKey) { // Alt+Click to copy key
            navigator.clipboard.writeText(binding.value);
            console.log(`Copied key: ${binding.value}`);
          }
        });
      }
    }
  };
  ```

- **Production Features** (for tenant admins):
  - **Edit mode toggle** in Admin panel
  - Click on any text to see its translation key
  - Direct link to edit that specific translation
  - Side panel showing key, current value, and edit option

- **Store Frontend Inline Editing** (for users with `content-editor` role):
  - **Role-based access**: Only users with `content-editor` or `admin` role can enable edit mode
  - **Edit mode toggle**: Floating button or header option (only visible for authorized users)
  - **Visual indicators**: Editable texts highlighted with subtle border/icon
  - **Inline edit workflow**:
    1. Click on any translatable text
    2. Modal/popover shows:
       - Translation key
       - Current value
       - Quick edit field
       - Preview of change
       - Save/Cancel buttons
    3. Changes saved immediately to tenant overrides
    4. Live preview of edited text
  - **Edit history**: Track who edited which keys and when
  - **Revert option**: Undo recent changes or reset to default
  - **Context-aware editing**: Show related keys (e.g., all product card texts)
  
  ```vue
  <template>
    <!-- Store Frontend with inline editing -->
    <div class="product-card">
      <h3 v-i18n-editable="'catalog.product.title'" 
          :class="{ 'editable': canEdit }">
        {{ t('catalog.product.title') }}
      </h3>
    </div>
    
    <!-- Edit mode overlay (only for content-editor role) -->
    <TranslationEditOverlay 
      v-if="canEdit && editMode" 
      @save="handleSave" 
    />
  </template>
  
  <script setup>
  import { useAuth } from '@/composables/useAuth';
  
  const { hasRole } = useAuth();
  const canEdit = computed(() => 
    hasRole('content-editor') || hasRole('admin')
  );
  const editMode = ref(false);
  
  const handleSave = async (key, value) => {
    await api.saveTenantTranslation(key, value);
    // Invalidate cache, refresh translations
  };
  </script>
  ```
  
- **Security & Authorization**:
  - Backend API endpoint: `POST /api/tenant/translations/{key}`
  - Requires `content-editor` or `admin` role
  - Audit log for all translation changes
  - Rate limiting to prevent abuse
  
- **Validation**:
  - Prevent HTML/script injection in custom values
  - Length validation for UI elements
  - Format string parameter validation

### 9. Rollout & Monitoring (Week 5)
- **Gradual rollout**: Feature flags for localization
- **Monitoring**: 
  - Track usage of fallback messages
  - Log when message keys are missing
  - Monitor Accept-Language header distribution (API)
  - Track CLI language preference distribution
  - Track tenant customization usage (which keys are most customized)
  - Monitor cache hit/miss rates for tenant overrides
- **Feedback loop**: User reports for missing/incorrect translations

## Technical Implementation

### Message Keys Registry (Consistency Enforcement)
```csharp
// MessageKeys/CatalogMessages.cs
public static class CatalogMessages
{
    public const string ProductNotFound = "Catalog.Product.NotFound";
    public const string ProductCreated = "Catalog.Product.Created";
    public const string InvalidPrice = "Catalog.Product.ValidationFailed.InvalidPrice";
}

// Usage in code
_localizer[CatalogMessages.ProductNotFound, productId]
// Prevents typos, enables refactoring, IDE autocomplete
```

### Separate User vs Internal Messages
```csharp
public class ProductService
{
    private readonly IStringLocalizer<ProductService> _localizer;
    private readonly ILogger<ProductService> _logger;
    
    public async Task<UserResult> CreateProduct(CreateProductCommand cmd)
    {
        // Internal logging - English only
        _logger.LogInformation("Creating product with SKU {Sku}", cmd.Sku);
        
        if (!ValidatePrice(cmd.Price))
        {
            // User-facing - localized
            return UserResult.Failure(CatalogMessages.InvalidPrice, cmd.Price);
        }
        
        try 
        {
            var product = await _repository.CreateAsync(cmd);
            _logger.LogInformation("Product {Id} created successfully", product.Id);
            
            // User-facing - localized
            return UserResult.Success(CatalogMessages.ProductCreated, product.Id);
        }
        catch (Exception ex)
        {
            // Internal logging - English
            _logger.LogError(ex, "Failed to create product with SKU {Sku}", cmd.Sku);
            
        }
    }
}
```

### CLI Language Configuration
```csharp
// CLI startup configuration
public class CliApp
{
    public static async Task Main(string[] args)
    {
        // 1. Parse language flag
        var language = ParseLanguageFlag(args); // --language=de or -l de
        
        // 2. Check environment variable
        language ??= Environment.GetEnvironmentVariable("B2X_CLI_LANGUAGE");
        
        // 3. Check config file
        language ??= await LoadConfigLanguage();
        
        // 4. Detect system locale
        language ??= CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        
        // 5. Validate and set culture
        if (IsValidLanguage(language))
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language);
        }
        else
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en"); // Fallback
        }
        
        // Now all CLI messages use localized strings
        var localizer = serviceProvider.GetRequiredService<IStringLocalizer<CliApp>>();
        Console.WriteLine(localizer[CliMessages.Welcome]);
    }
}

// Example CLI command with localization
public class ProductImportCommand
{
    private readonly IStringLocalizer<ProductImportCommand> _localizer;
    private readonly ILogger<ProductImportCommand> _logger;
    
    public async Task ExecuteAsync(ImportOptions options)
    {
        // User output - localized
        Console.WriteLine(_localizer[CliMessages.ImportStarting, options.FilePath]);
        
        try
        {
            var result = await ImportAsync(options);
            // User output - localized
            Console.WriteLine(_localizer[CliMessages.ImportSuccess, result.Count]);
        }
        catch (Exception ex)
        {
            // Internal log - English
            _logger.LogError(ex, "Import failed for file {FilePath}", options.FilePath);
            
            // User output - localized
            Console.Error.WriteLine(_localizer[CliMessages.ImportFailed, ex.Message]);
        }
    }
}
```

### Wolverine Handler Pattern (with Consistency)
```csharp
public class CreateProductHandler
{
    private readonly IStringLocalizer<CreateProductHandler> _localizer;
    private readonly ILogger<CreateProductHandler> _logger;
    
    public async Task<UserResult> Handle(CreateProductCommand command)
    {
        // Internal log - English
        _logger.LogDebug("Processing CreateProductCommand for SKU {Sku}", command.Sku);
        
        if (validation.Failed)
        {
            // User message - localized with constants
            return UserResult.Failure(CatalogMessages.InvalidPrice, command.Price);
        }
            
        // User message - localized with constants
        return UserResult.Success(CatalogMessages.ProductCreated, product.Id);
    }
}
```

### CI Validation Script
```bash
# Check for missing translation keys
dotnet run --project tools/LocalizationValidator \
  --check-missing-keys \
  --verify-naming-convention \
  --compare-message-keys-constants

# Fails build if:
# - Keys missing in any locale
# - Key naming doesn't follow convention
# - MessageKeys.cs out of sync with resource files
```

## Consistency Guarantees
1. **Centralized keys**: All keys defined in `MessageKeys` constants
2. **Naming convention**: Enforced via CI validation
3. **Completeness**: CI fails if keys missing in any locale
4. **Separation**: Clear distinction between user-facing (localized) and internal (English) messages
5. **Type safety**: Compile-time checking via constants
6. **CLI consistency**: Same localization infrastructure for API and CLI tools
7. **Language selection**: Multiple mechanisms with clear priority order
8. **Tenant customization**: Per-tenant override capability with proper fallback chain
9. **Cache performance**: Distributed caching for tenant-specific translations

## CLI Language Selection Examples

```bash
# Method 1: Command-line flag
b2x-admin import --file products.csv --language de
b2x-admin import --file products.csv -l fr

# Method 2: Environment variable
export B2X_CLI_LANGUAGE=es
b2x-admin import --file products.csv

# Method 3: Config file
b2x-admin config set language de
# Saves to ~/.b2x/config.json: { "language": "de" }
b2x-admin import --file products.csv

# Interactive setup on first run
b2x-admin setup
# > Select your preferred language:
# > 1. English (en)
# > 2. Deutsch (de)
# > 3. Français (fr)
# > ...
```

## Alternative Approaches Considered
```csharp
public class LocalizedException : Exception
{
    public LocalizedException(string key, params object[] args)
        : base(key) // Store key as message
    {
        Key = key;
        Args = args;
    }

    public string Key { get; }
    public object[] Args { get; }
}
```

### Alternative 1: Frontend-Only Localization
**Rejected**: Backend messages would remain in English, inconsistent UX

### Alternative 2: Separate Translation Systems
**Rejected**: Maintenance overhead, consistency issues between backend/frontend

### Alternative 3: Runtime Translation Service (e.g., Google Translate)
**Rejected**: Latency, cost, quality control issues, offline capability lost

## Risks & Mitigations
- **Performance**: Localization lookup overhead → Cache localizers, use async where possible
- **Missing keys**: Fallback to English → Implement key validation in CI
- **Cultural differences**: Message length/format → Design flexible templates
- **Tenant override performance**: Database lookups per request → Distributed cache (Redis) with invalidation
- **Custom text injection**: XSS/script injection → Sanitize all custom values, validate format strings
- **Cache synchronization**: Stale data after tenant updates → Cache invalidation on update
- **Unauthorized editing**: Content editors abuse inline edit → Role-based access control, audit logging, change approval workflow
- **Edit conflicts**: Multiple users editing same key → Optimistic locking, last-write-wins with notification

## Success Metrics
- 100% of **user-facing** messages use localization keys
- All supported languages have complete translations
- No hardcoded user-facing strings in production code
- CI validation passes for all localization rules
- User satisfaction with localized error messages
- Tenant adoption rate of customization features (target: >30% of tenants customize at least 5 keys)
- Translation key inspector used by >80% of tenant admins for customization tasks
- Store frontend inline editing adoption by content editors (target: >50% of tenants use feature)
- Average time to customize a translation reduced by 70% with inline editing

## Timeline
- **Phase 1 (Audit)**: Complete by end of Week 1
- **Phase 2 (Implementation)**: Complete by end of Week 4
- **Phase 3 (Testing)**: Complete by end of Week 5
- **Phase 4 (Tenant Customization)**: Complete by end of Week 6
- **Phase 5 (Rollout)**: Gradual rollout Week 7-9

## Next Steps
1. Schedule kickoff meeting with @Backend team
2. Create `MessageKeys` registry structure and tooling (include CLI messages)
3. Set up CI validation for localization rules
4. Begin audit with Roslyn MCP scanning (user-facing code only, including CLIs)
5. Create guidelines document for team reference
6. Implement CLI language selection mechanism
7. Design tenant translation override database schema
8. Create Admin UI mockups for text customization feature
9. Implement translation key inspector for developer and admin use
10. Design Store frontend inline editing UI/UX for content-editor role
11. Implement role-based authorization for translation editing
12. Create audit logging system for translation changes

**Owner**: @Backend  
**Review**: @SARAH (coordination), @TechLead (quality), @Frontend (UI customization), @Security (role-based access)  
**Dependencies**: 
- KB-005 (.NET Localization)
- ADR-XXX (Localization Architecture - to be created)
- ADR-040 (Tenant-Customizable Language Resources - see related ADR)
- Identity system with `content-editor` role support

---

## Backlog Refinement

### Epic: Backend Message Localization
**Priority**: P1 - High  
**Estimated Timeline**: 9 weeks  
**Teams**: @Backend, @Frontend, @Security

---

### Sprint 1: Foundation & Audit (Week 1)

#### Story 1.1: Define User-Facing Message Boundaries
**Priority**: P0 - Critical  
**Estimate**: 3 points  
**Owner**: @Backend + @TechLead

**Acceptance Criteria**:
- [ ] Document created listing all user-facing message types
- [ ] Document created listing all internal message types (keep English)
- [ ] Guidelines shared with team for review
- [ ] Examples provided for each category

**Tasks**:
- Create classification guidelines document
- Review with team for consensus
- Update code standards documentation

---

#### Story 1.2: Audit Hardcoded Strings (Backend)
**Priority**: P0 - Critical  
**Estimate**: 8 points  
**Owner**: @Backend  
**Tool**: Roslyn MCP

**Acceptance Criteria**:
- [ ] Scan completed for Controllers, Wolverine handlers, validators
- [ ] Report generated with file paths and line numbers
- [ ] Hardcoded strings categorized (user-facing vs internal)
- [ ] GitHub issues created for each violation (tagged: `i18n-debt`)

**Tasks**:
- Run `roslyn-mcp/analyze_types` on backend codebase
- Generate CSV report of hardcoded strings
- Create GitHub issues with labels
- Estimate remediation effort per module

**Output**: `.ai/reports/i18n-audit-backend.csv`

---

#### Story 1.3: Audit Hardcoded Strings (CLI)
**Priority**: P1 - High  
**Estimate**: 5 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] CLI commands scanned for hardcoded output
- [ ] Report includes Console.WriteLine, error messages
- [ ] GitHub issues created (tagged: `i18n-debt`, `cli`)

---

#### Story 1.4: Create MessageKeys Registry Structure
**Priority**: P0 - Critical  
**Estimate**: 5 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] `MessageKeys/` directory created in shared project
- [ ] Naming convention documented
- [ ] Example files created: `CatalogMessages.cs`, `CommonMessages.cs`
- [ ] Code generation script created (optional)

**Tasks**:
- Create directory structure
- Implement example message key classes
- Document naming patterns
- Create T4 template or source generator (optional)

**Output**: `src/backend/Shared/MessageKeys/`

---

### Sprint 2-3: Core Implementation (Weeks 2-4)

#### Story 2.1: Implement TenantAwareStringLocalizer
**Priority**: P0 - Critical  
**Estimate**: 13 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] `TenantAwareStringLocalizer` implements `IStringLocalizer`
- [ ] Fallback chain: Tenant override → Default → English
- [ ] Distributed cache integration (Redis)
- [ ] Unit tests with 90%+ coverage
- [ ] Integration tests with multiple tenants

**Tasks**:
- Implement localizer class
- Add Redis cache dependency
- Write unit tests
- Write integration tests
- Performance benchmark (target: <50ms lookup)

**Dependencies**: Database schema (Story 2.2)

---

#### Story 2.2: Create Database Schema for Tenant Overrides
**Priority**: P0 - Critical  
**Estimate**: 5 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] `TenantTranslationOverrides` table created
- [ ] Migration script tested
- [ ] Indexes added for performance (TenantId, LanguageCode, MessageKey)
- [ ] Audit columns added (CreatedBy, CreatedAt, ModifiedBy, ModifiedAt)

**Schema**:
```sql
CREATE TABLE TenantTranslationOverrides (
  Id BIGINT PRIMARY KEY,
  TenantId GUID NOT NULL,
  LanguageCode VARCHAR(5) NOT NULL,
  MessageKey VARCHAR(255) NOT NULL,
  CustomValue NVARCHAR(MAX) NOT NULL,
  CreatedBy GUID NOT NULL,
  CreatedAt TIMESTAMP NOT NULL,
  ModifiedBy GUID,
  ModifiedAt TIMESTAMP,
  INDEX IX_Tenant_Lang_Key (TenantId, LanguageCode, MessageKey)
);
```

---

#### Story 2.3: Implement UserResult/InternalResult Pattern
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] `UserResult` class created with MessageKey property
- [ ] `InternalResult` class created for English logs
- [ ] Conversion helpers implemented
- [ ] Example Wolverine handlers updated
- [ ] Documentation with usage examples

---

#### Story 2.4: Backend API Localization (Catalog Module)
**Priority**: P1 - High  
**Estimate**: 13 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] All Catalog API endpoints return localized messages
- [ ] Middleware localizes error responses
- [ ] Integration tests validate Accept-Language header
- [ ] No hardcoded user-facing strings remain
- [ ] Logs remain in English

**Scope**: Catalog module as pilot

---

#### Story 2.5: CLI Language Selection Implementation
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @Backend

**Acceptance Criteria**:
- [ ] `--language` flag implemented
- [ ] `B2X_CLI_LANGUAGE` environment variable supported
- [ ] Config file integration (`~/.b2x/config.json`)
- [ ] System locale detection fallback
- [ ] Interactive language picker on first run
- [ ] Tests for all selection methods

---

### Sprint 4: Frontend Integration (Week 4-5)

#### Story 3.1: Translation Key Inspector (Debug Mode)
**Priority**: P2 - Medium  
**Estimate**: 8 points  
**Owner**: @Frontend

**Acceptance Criteria**:
- [ ] `?i18n-debug=true` query parameter enables inspector
- [ ] `v-i18n-debug` Vue directive implemented
- [ ] Visual indicators show on translatable elements
- [ ] Alt+Click copies key to clipboard
- [ ] Works in Store and Admin frontends

---

#### Story 3.2: Admin Panel - Translation Management UI
**Priority**: P1 - High  
**Estimate**: 13 points  
**Owner**: @Frontend

**Acceptance Criteria**:
- [ ] CRUD interface for tenant translation overrides
- [ ] Search and filter by key/value
- [ ] Bulk import/export (CSV/JSON)
- [ ] Preview mode before publishing
- [ ] Reset to default option per key
- [ ] Pagination for large datasets

---

#### Story 3.3: Store Inline Editing (Content-Editor Role)
**Priority**: P2 - Medium  
**Estimate**: 13 points  
**Owner**: @Frontend + @Security

**Acceptance Criteria**:
- [ ] Role-based access control (`content-editor` role)
- [ ] Edit mode toggle visible only to authorized users
- [ ] Click any text to edit modal
- [ ] Live preview of changes
- [ ] Save API integration
- [ ] Audit logging
- [ ] Edit history tracking

**Dependencies**: Backend API (Story 3.4), Role implementation

---

#### Story 3.4: Backend API for Translation Management
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @Backend + @Security

**Acceptance Criteria**:
- [ ] `POST /api/tenant/translations/{key}` endpoint
- [ ] `GET /api/tenant/translations` (list with pagination)
- [ ] `DELETE /api/tenant/translations/{key}` (reset to default)
- [ ] Authorization checks (`content-editor` or `admin` role)
- [ ] Rate limiting (10 requests/minute per tenant)
- [ ] Audit logging
- [ ] Cache invalidation on update

---

### Sprint 5: CI/CD & Quality Gates (Week 5)

#### Story 4.1: CI Validation Tool
**Priority**: P0 - Critical  
**Estimate**: 13 points  
**Owner**: @Backend + @DevOps

**Acceptance Criteria**:
- [ ] Tool checks for missing keys across all locales
- [ ] Validates naming convention compliance
- [ ] Compares MessageKeys.cs with resource files
- [ ] Detects unused keys
- [ ] Integrated into GitHub Actions
- [ ] Fails build on violations

**Output**: `tools/LocalizationValidator`

---

#### Story 4.2: Resource File Population
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @Backend + Translation Team

**Acceptance Criteria**:
- [ ] All MessageKeys exist in base English resource
- [ ] At least 3 languages populated for each key
- [ ] Translation quality review completed
- [ ] Format string validation passed

**Languages**: en, de, fr (minimum for merge)

---

### Sprint 6: Testing & Rollout (Week 6-9)

#### Story 5.1: Integration Testing Suite
**Priority**: P1 - High  
**Estimate**: 13 points  
**Owner**: @QA

**Acceptance Criteria**:
- [ ] Tests for Accept-Language header handling
- [ ] Tests for tenant override fallback chain
- [ ] Tests for CLI language selection
- [ ] Tests for cache invalidation
- [ ] Performance tests (latency <50ms)

---

#### Story 5.2: Documentation
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @DocMaintainer

**Acceptance Criteria**:
- [ ] Developer guide created
- [ ] User guide for tenant admins created
- [ ] Troubleshooting guide created
- [ ] API documentation updated
- [ ] README updated with i18n section

**Outputs**:
- `docs/developer/i18n-guide.md`
- `docs/user/customizing-translations.md`
- `docs/guides/troubleshooting-i18n.md`

---

#### Story 5.3: Gradual Rollout with Feature Flags
**Priority**: P0 - Critical  
**Estimate**: 5 points  
**Owner**: @DevOps

**Acceptance Criteria**:
- [ ] Feature flag `backend-localization` implemented
- [ ] Feature flag `tenant-translation-override` implemented
- [ ] Feature flag `store-inline-editing` implemented
- [ ] Rollout plan documented (10% → 50% → 100%)
- [ ] Rollback procedure tested

---

#### Story 5.4: Monitoring & Alerting
**Priority**: P1 - High  
**Estimate**: 8 points  
**Owner**: @DevOps

**Acceptance Criteria**:
- [ ] Dashboard for missing translation key errors
- [ ] Dashboard for fallback usage rates
- [ ] Dashboard for tenant customization adoption
- [ ] Alert on cache hit rate drop below 80%
- [ ] Weekly report generation

---

### Backlog (Future Sprints)

#### Story 6.1: Expand to All Modules
**Priority**: P2 - Medium  
**Estimate**: 21+ points (per module)

Repeat Story 2.4 for:
- [ ] Orders module
- [ ] CMS module
- [ ] Identity module
- [ ] Admin Gateway
- [ ] Management Gateway

---

#### Story 6.2: Advanced Translation Features
**Priority**: P3 - Low  
**Estimate**: TBD

- [ ] Pluralization support
- [ ] Gender-specific translations
- [ ] Context-aware translations
- [ ] Translation memory integration
- [ ] Machine translation suggestions

---

## Story Point Reference
- **1-2 points**: Few hours, straightforward
- **3-5 points**: 1-2 days, some complexity
- **8 points**: 3-5 days, moderate complexity
- **13 points**: 1 week, high complexity
- **21+ points**: Epic-level, needs breakdown

---

## Dependencies Graph

```
Story 1.1 (Boundaries) → Story 1.2, 1.3 (Audits)
Story 1.4 (MessageKeys) → Story 2.1, 2.3, 2.4
Story 2.2 (DB Schema) → Story 2.1 (Localizer)
Story 2.1 (Localizer) → Story 2.4 (API Localization)
Story 3.4 (Backend API) → Story 3.2, 3.3 (Frontend UIs)
Story 4.1 (CI Tool) → All future development
Story 5.3 (Feature Flags) → Story 5.4 (Monitoring)
```

---

## Priority Matrix

| Priority | Stories | Timeline |
|----------|---------|----------|
| P0 - Critical | 1.1, 1.2, 1.4, 2.1, 2.2, 4.1, 5.3 | Weeks 1-6 |
| P1 - High | 1.3, 2.3, 2.4, 2.5, 3.2, 3.4, 4.2, 5.1, 5.2, 5.4 | Weeks 2-9 |
| P2 - Medium | 3.1, 3.3, 6.1 | Weeks 5-12 |
| P3 - Low | 6.2 | Future |

---

## Ready for Sprint Planning
- All P0 and P1 stories are well-defined
- Acceptance criteria documented
- Dependencies identified
- Team assignments proposed