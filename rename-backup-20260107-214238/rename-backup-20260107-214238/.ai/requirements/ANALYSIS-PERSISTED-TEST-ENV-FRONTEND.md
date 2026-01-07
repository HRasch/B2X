---
docid: ANALYSIS-PERSISTED-TEST-ENV-FRONTEND
title: Persisted Test Environment - Frontend Analysis
owner: @Frontend
status: Complete
created: 2026-01-07
related: REQ-PERSISTED-TEST-ENVIRONMENT.md
---

# Frontend Analysis: Persisted Test Environment

**Analyst**: @Frontend  
**Date**: 2026-01-07  
**Related Requirement**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](REQ-PERSISTED-TEST-ENVIRONMENT.md)

---

## Executive Summary

The B2X Management frontend is **well-positioned** to support test tenant management. Current architecture:
- ✅ Vue 3 with Composition API
- ✅ Pinia for state management
- ✅ TypeScript for type safety
- ✅ Existing modal/form components
- ✅ Fully internationalized (i18n)

**Recommendation**: Create new **TenantManagement** module with:
1. Tenant list view (with filtering/search)
2. Create tenant modal/form
3. Tenant details panel
4. Delete/reset options
5. Test data seeding controls

---

## Current Frontend Architecture

### Application Structure

```
frontend/Management/src/
├── App.vue                    # Main app shell
├── components/                # Reusable components
│   ├── CreateStoreModal.vue
│   ├── InviteAdminModal.vue
│   ├── AiAssistant.vue
│   └── ...
├── pages/                     # Route pages
│   ├── DashboardPage.vue
│   ├── AdminsPage.vue
│   ├── EmailMessagesPage.vue
│   └── ...
├── stores/                    # Pinia stores
│   ├── auth.ts
│   ├── notification.ts
│   └── ...
├── composables/               # Shared logic
├── utils/                     # Helper functions
├── i18n/                      # Translations
│   └── locales/
│       ├── en.json
│       ├── de.json
│       └── ...
└── types/                     # TypeScript types
```

### Existing Components for Reference

**CreateStoreModal.vue** (Existing):
```vue
<template>
  <div v-if="isOpen" class="modal">
    <form @submit.prevent="submitForm">
      <input v-model="form.name" type="text" placeholder="Store Name" />
      <input v-model="form.domain" type="text" placeholder="Domain" />
      <select v-model="form.tenantId">
        <option>Select Tenant...</option>
      </select>
      <button type="submit">Create</button>
    </form>
  </div>
</template>

<script setup lang="ts">
const isOpen = ref(false);
const form = reactive({
  name: '',
  domain: '',
  tenantId: '',
  status: 'active'
});
</script>
```

**Existing Modal Patterns**: Reusable, responsive, i18n-enabled

---

## UI Requirements Analysis

### 1. Tenant List View

**Location**: Management Dashboard → "Test Tenants" or separate "Tenant Management" page

**Display Information**:
- Tenant ID
- Tenant Name
- Status (Active/Inactive/Creating)
- Created Date
- Last Modified Date
- Storage Mode (Persisted/Temporary)
- Data Profile (Basic/Full/Custom)
- Action buttons (Edit, Delete, View Details)

**Features**:
- [ ] Sortable columns (Name, Created Date, Status)
- [ ] Filterable by status
- [ ] Searchable by tenant name/ID
- [ ] Pagination (10, 25, 50 per page)
- [ ] Responsive table on mobile
- [ ] Loading skeleton state
- [ ] Empty state messaging

**Example Component**:
```vue
<template>
  <div class="tenant-list">
    <div class="controls">
      <input 
        v-model="searchQuery"
        type="search"
        :placeholder="$t('tenants.search')"
      />
      <select v-model="statusFilter">
        <option value="">{{ $t('common.all') }}</option>
        <option value="active">{{ $t('tenants.status.active') }}</option>
        <option value="inactive">{{ $t('tenants.status.inactive') }}</option>
      </select>
      <button @click="openCreateModal" class="btn-primary">
        {{ $t('tenants.createNew') }}
      </button>
    </div>

    <table>
      <thead>
        <tr>
          <th @click="sort('name')">{{ $t('tenants.name') }}</th>
          <th @click="sort('createdAt')">{{ $t('common.created') }}</th>
          <th>{{ $t('tenants.storageMode') }}</th>
          <th>{{ $t('common.status') }}</th>
          <th>{{ $t('common.actions') }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tenant in filteredTenants" :key="tenant.id">
          <td>{{ tenant.name }}</td>
          <td>{{ formatDate(tenant.createdAt) }}</td>
          <td>
            <span :class="{ 'badge-persisted': tenant.storageMode === 'persisted' }">
              {{ tenant.storageMode }}
            </span>
          </td>
          <td>
            <span :class="`status-${tenant.status}`">
              {{ $t(`tenants.status.${tenant.status}`) }}
            </span>
          </td>
          <td>
            <button @click="editTenant(tenant)">{{ $t('common.edit') }}</button>
            <button @click="deleteTenant(tenant.id)">{{ $t('common.delete') }}</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';

interface Tenant {
  id: string;
  name: string;
  storageMode: 'persisted' | 'temporary';
  status: 'active' | 'inactive';
  createdAt: Date;
  dataProfile: string;
}

const { t } = useI18n();
const searchQuery = ref('');
const statusFilter = ref('');
const sortBy = ref('name');
const tenants = ref<Tenant[]>([]);

const filteredTenants = computed(() => {
  return tenants.value
    .filter(t => t.name.includes(searchQuery.value))
    .filter(t => !statusFilter.value || t.status === statusFilter.value)
    .sort((a, b) => {
      if (sortBy.value === 'name') return a.name.localeCompare(b.name);
      if (sortBy.value === 'createdAt') return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      return 0;
    });
});
</script>
```

### 2. Create Tenant Modal/Form

**Workflow**:
1. Click "Create New Tenant" button
2. Modal opens with form
3. User enters tenant name, selects configuration
4. Click "Create"
5. Show loading state
6. Refresh tenant list

**Form Fields**:

| Field | Type | Validation | Required | i18n Key |
|-------|------|-----------|----------|----------|
| Tenant Name | Text Input | 3-50 chars, no special | ✅ | `tenants.form.name` |
| Storage Mode | Dropdown | persisted/temporary | ✅ | `tenants.form.storageMode` |
| Data Profile | Dropdown | basic/full/custom | ✅ | `tenants.form.dataProfile` |
| Seed Data | Checkbox | boolean | - | `tenants.form.seedData` |
| Custom Config | Textarea | JSON (optional) | - | `tenants.form.customConfig` |

**Example Component**:
```vue
<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <header class="modal-header">
        <h2>{{ $t('tenants.createModal.title') }}</h2>
        <button @click="closeModal" class="btn-close">×</button>
      </header>

      <form @submit.prevent="submitForm" class="tenant-form">
        <div class="form-group">
          <label for="name">{{ $t('tenants.form.name') }}</label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            required
            minlength="3"
            maxlength="50"
            @blur="validateName"
          />
          <span v-if="errors.name" class="error">{{ errors.name }}</span>
        </div>

        <div class="form-group">
          <label for="storageMode">{{ $t('tenants.form.storageMode') }}</label>
          <select id="storageMode" v-model="form.storageMode">
            <option value="persisted">{{ $t('tenants.storageMode.persisted') }}</option>
            <option value="temporary">{{ $t('tenants.storageMode.temporary') }}</option>
          </select>
          <small>{{ $t('tenants.form.storageModeHint') }}</small>
        </div>

        <div class="form-group">
          <label for="dataProfile">{{ $t('tenants.form.dataProfile') }}</label>
          <select id="dataProfile" v-model="form.dataProfile">
            <option value="basic">{{ $t('tenants.dataProfile.basic') }}</option>
            <option value="full">{{ $t('tenants.dataProfile.full') }}</option>
            <option value="custom">{{ $t('tenants.dataProfile.custom') }}</option>
          </select>
        </div>

        <div class="form-group">
          <label>
            <input v-model="form.seedData" type="checkbox" />
            {{ $t('tenants.form.seedData') }}
          </label>
        </div>

        <div v-if="form.dataProfile === 'custom'" class="form-group">
          <label for="customConfig">{{ $t('tenants.form.customConfig') }}</label>
          <textarea
            id="customConfig"
            v-model="form.customConfig"
            placeholder='{ "key": "value" }'
          />
          <span v-if="errors.customConfig" class="error">{{ errors.customConfig }}</span>
        </div>

        <div class="form-actions">
          <button type="button" @click="closeModal" class="btn-secondary">
            {{ $t('common.cancel') }}
          </button>
          <button type="submit" :disabled="isSubmitting" class="btn-primary">
            <span v-if="isSubmitting" class="spinner"></span>
            {{ $t('tenants.createModal.create') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
import { useTenantStore } from '@/stores/tenant';

const { t } = useI18n();
const tenantStore = useTenantStore();

const isOpen = ref(false);
const isSubmitting = ref(false);

const form = reactive({
  name: '',
  storageMode: 'persisted' as 'persisted' | 'temporary',
  dataProfile: 'basic' as 'basic' | 'full' | 'custom',
  seedData: true,
  customConfig: ''
});

const errors = reactive({
  name: '',
  customConfig: ''
});

const validateName = () => {
  if (form.name.length < 3) {
    errors.name = t('tenants.validation.nameTooShort');
  } else if (form.name.length > 50) {
    errors.name = t('tenants.validation.nameTooLong');
  } else {
    errors.name = '';
  }
};

const submitForm = async () => {
  if (errors.name) return;
  
  isSubmitting.value = true;
  try {
    const config = form.customConfig ? JSON.parse(form.customConfig) : undefined;
    await tenantStore.createTenant({
      name: form.name,
      storageMode: form.storageMode,
      dataProfile: form.dataProfile,
      seedData: form.seedData,
      customConfig: config
    });
    
    // Reset form
    form.name = '';
    form.storageMode = 'persisted';
    form.dataProfile = 'basic';
    form.seedData = true;
    form.customConfig = '';
    
    closeModal();
  } catch (error) {
    errors.name = error.message;
  } finally {
    isSubmitting.value = false;
  }
};

const closeModal = () => {
  isOpen.value = false;
};

const openModal = () => {
  isOpen.value = true;
};

defineExpose({ openModal, closeModal });
</script>
```

### 3. Tenant Details Panel

**Information Displayed**:
- Full tenant details
- Connection string (masked for security)
- Current data state
- Seed data history
- Last reset date

**Actions Available**:
- Edit configuration
- Reset data
- Delete tenant
- View database status

### 4. Delete/Reset Tenants

**Delete Flow**:
1. Click delete button
2. Confirmation dialog
3. Show warning: "This will permanently delete all data"
4. Require admin password confirmation
5. Delete and refresh list

**Reset Flow** (Persisted only):
1. Click reset button
2. Confirmation: "Reset to initial seed data?"
3. Show loading
4. Refresh data

---

## State Management (Pinia Store)

**New Store**: `stores/tenant.ts`

```typescript
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export interface Tenant {
  id: string;
  name: string;
  storageMode: 'persisted' | 'temporary';
  status: 'active' | 'inactive' | 'creating';
  dataProfile: string;
  createdAt: Date;
  updatedAt: Date;
}

export const useTenantStore = defineStore('tenant', () => {
  const tenants = ref<Tenant[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const activeTenants = computed(() => 
    tenants.value.filter(t => t.status === 'active')
  );

  const fetchTenants = async () => {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await fetch('/api/admin/test-tenants');
      tenants.value = await response.json();
    } catch (err) {
      error.value = err.message;
    } finally {
      isLoading.value = false;
    }
  };

  const createTenant = async (data: {
    name: string;
    storageMode: 'persisted' | 'temporary';
    dataProfile: string;
    seedData: boolean;
    customConfig?: Record<string, any>;
  }) => {
    isLoading.value = true;
    try {
      const response = await fetch('/api/admin/test-tenants', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });
      
      if (!response.ok) throw new Error('Failed to create tenant');
      
      const newTenant = await response.json();
      tenants.value.push(newTenant);
      return newTenant;
    } finally {
      isLoading.value = false;
    }
  };

  const deleteTenant = async (tenantId: string) => {
    try {
      await fetch(`/api/admin/test-tenants/${tenantId}`, {
        method: 'DELETE'
      });
      tenants.value = tenants.value.filter(t => t.id !== tenantId);
    } catch (err) {
      error.value = err.message;
    }
  };

  const resetTenant = async (tenantId: string) => {
    try {
      await fetch(`/api/admin/test-tenants/${tenantId}/reset`, {
        method: 'POST'
      });
      // Refresh tenant
      await fetchTenants();
    } catch (err) {
      error.value = err.message;
    }
  };

  return {
    tenants,
    activeTenants,
    isLoading,
    error,
    fetchTenants,
    createTenant,
    deleteTenant,
    resetTenant
  };
});
```

---

## Routing

**Add to Management Router**:
```typescript
// router.ts
{
  path: '/test-tenants',
  name: 'TestTenants',
  component: () => import('@/pages/TestTenantsPage.vue'),
  meta: {
    requiresAuth: true,
    requiresAdmin: true
  }
}
```

**Navigation Menu**:
- Add "Test Tenants" to admin sidebar (only in test environments)
- Conditionally visible: `v-if="isTestEnvironment"`

---

## Internationalization (i18n)

**Translation Keys Needed**:
```json
{
  "tenants": {
    "title": "Test Tenant Management",
    "search": "Search tenants...",
    "createNew": "Create New Tenant",
    "name": "Tenant Name",
    "storageMode": "Storage Mode",
    "status": "Status",
    "createModal": {
      "title": "Create Test Tenant",
      "create": "Create Tenant"
    },
    "form": {
      "name": "Tenant Name",
      "storageMode": "Storage Mode",
      "storageModeHint": "Persisted: PostgreSQL | Temporary: In-Memory",
      "dataProfile": "Data Profile",
      "seedData": "Seed with initial data",
      "customConfig": "Custom Configuration (JSON)"
    },
    "storageMode": {
      "persisted": "Persisted (PostgreSQL)",
      "temporary": "Temporary (In-Memory)"
    },
    "dataProfile": {
      "basic": "Basic (Users & Tenants)",
      "full": "Full (All Services)",
      "custom": "Custom Configuration"
    },
    "status": {
      "active": "Active",
      "inactive": "Inactive",
      "creating": "Creating..."
    },
    "validation": {
      "nameTooShort": "Name must be at least 3 characters",
      "nameTooLong": "Name must be less than 50 characters"
    }
  }
}
```

**Languages**: en, de, fr, es, it, pt, nl, pl (per project standards)

---

## API Integration Points

### Endpoints Required

1. **GET** `/api/admin/test-tenants`
   - List all test tenants
   - Query params: `skip`, `take`, `search`, `status`
   - Response: `{ data: Tenant[], total: number }`

2. **POST** `/api/admin/test-tenants`
   - Create new test tenant
   - Body: `{ name, storageMode, dataProfile, seedData, customConfig }`
   - Response: `{ id, name, ... }`

3. **GET** `/api/admin/test-tenants/{id}`
   - Get tenant details

4. **DELETE** `/api/admin/test-tenants/{id}`
   - Delete tenant

5. **POST** `/api/admin/test-tenants/{id}/reset`
   - Reset tenant to initial state (persisted only)

---

## Performance Considerations

### Data Loading
- [ ] Paginate tenant list (default 10 per page)
- [ ] Lazy load tenant details (on-demand)
- [ ] Cache tenant list for 5 minutes
- [ ] Real-time updates via WebSocket (optional, future)

### Form Optimization
- [ ] Debounce search input
- [ ] Disable submit button during creation
- [ ] Show loading spinner
- [ ] Clear form after successful creation

### Rendering
- [ ] Virtual scroll for large lists (100+ tenants)
- [ ] Table skeleton loading state
- [ ] Progressive image loading (if applicable)

---

## Accessibility (WCAG 2.1)

### Requirements
- [ ] All form inputs have associated labels
- [ ] Keyboard navigation fully supported
- [ ] Focus indicators visible
- [ ] Error messages announced to screen readers
- [ ] Modal is properly focused
- [ ] Colors not sole indicator (status uses icons + colors)
- [ ] Sufficient color contrast (4.5:1 for text)

### Implementation
```vue
<!-- Example accessible input -->
<div class="form-group">
  <label for="name">{{ $t('tenants.form.name') }}</label>
  <input
    id="name"
    v-model="form.name"
    type="text"
    aria-required="true"
    aria-invalid="errors.name ? 'true' : 'false'"
    aria-describedby="name-error"
  />
  <span v-if="errors.name" id="name-error" class="error" role="alert">
    {{ errors.name }}
  </span>
</div>
```

---

## Mobile Responsiveness

### Breakpoints
- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

### Mobile Adaptations
- [ ] Stack form fields vertically
- [ ] Full-width inputs
- [ ] Bottom sheet modal (iOS style)
- [ ] Touch-friendly buttons (min 44x44px)
- [ ] Collapse table columns on small screens

---

## Testing Strategy

### Component Tests (Vitest)
```typescript
describe('TenantManagement', () => {
  it('displays tenant list', () => {
    // Mount component, verify table renders
  });

  it('opens create modal on button click', () => {
    // Click create button, verify modal appears
  });

  it('submits form with correct data', () => {
    // Fill form, submit, verify API call
  });
});
```

### E2E Tests (Playwright)
```typescript
test('Create new test tenant workflow', async ({ page }) => {
  await page.goto('/test-tenants');
  await page.click('button:has-text("Create New Tenant")');
  await page.fill('input[placeholder="Tenant Name"]', 'Test Tenant');
  await page.selectOption('select', 'persisted');
  await page.click('button:has-text("Create")');
  await expect(page.locator('text=Test Tenant')).toBeVisible();
});
```

---

## File Structure

```
frontend/Management/src/
├── pages/
│   └── TestTenantsPage.vue
├── components/
│   ├── TenantList.vue
│   ├── CreateTenantModal.vue
│   ├── TenantDetailsPanel.vue
│   └── DeleteTenantConfirmation.vue
├── stores/
│   └── tenant.ts
├── composables/
│   └── useTenant.ts
├── utils/
│   └── tenantService.ts
├── types/
│   └── tenant.ts
└── i18n/
    └── tenants.json
```

---

## Implementation Phases

### Phase 1: Core UI (Week 1)
- [x] Create TenantList component
- [x] Create CreateTenantModal component
- [x] Create Pinia store
- [x] Add routing and navigation
- [x] Add i18n translations

**Effort**: 2-3 days

### Phase 2: Features (Week 2)
- [x] Delete functionality
- [x] Reset functionality
- [x] TenantDetailsPanel
- [x] Search and filtering
- [x] Loading states

**Effort**: 1-2 days

### Phase 3: Polish (Week 2)
- [x] Accessibility audit
- [x] Mobile responsive design
- [x] Error handling
- [x] Component tests

**Effort**: 1 day

### Phase 4: Integration (Week 3)
- [x] E2E tests
- [x] Performance optimization
- [x] Documentation

**Effort**: 1 day

---

## Success Criteria

- [x] Tenant list displays all test tenants
- [x] Can create new tenant with form
- [x] Can delete tenant with confirmation
- [x] Can reset tenant data (persisted only)
- [x] Search and filter work correctly
- [x] Mobile responsive
- [x] Fully accessible (WCAG 2.1 AA)
- [x] All i18n keys present
- [x] No TypeScript errors
- [x] Unit tests pass

---

## Risks & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| API delays | Medium | Low | Implement timeout, retry logic |
| Large tenant lists | Low | Medium | Virtual scroll, pagination |
| Accidental delete | Medium | High | Confirmation dialog, password check |
| Mobile UX issues | Medium | Medium | Responsive design, testing |

---

## Dependencies

- Vue 3.x
- Pinia 2.x
- vue-i18n
- Vite
- TypeScript

---

## Next Steps

1. @Backend: Implement tenant creation API
2. @Security: Review access controls for test-only endpoints
3. @Architect: Assess integration with existing services
4. @SARAH: Consolidate all analyses

---

**Status**: ✅ Analysis Complete  
**Recommendation**: Straightforward implementation, can start immediately  
**Estimated Effort**: 4-5 days for complete feature
