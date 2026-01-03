# Admin Frontend Test Suite

Comprehensive testing documentation for the Admin Frontend application.

## Overview

This project implements a complete testing strategy covering:

- **Unit Tests** (Vitest): Store logic, API services, utilities
- **Component Tests** (Vue Test Utils): Vue component rendering and interactions
- **E2E Tests** (Playwright): Complete user workflows and integration scenarios

## Test Structure

```
tests/
├── setup.ts                 # Global test configuration
├── unit/
│   ├── stores/
│   │   ├── auth.spec.ts    # Authentication store tests
│   │   ├── cms.spec.ts     # CMS store tests
│   │   ├── shop.spec.ts    # Shop store tests
│   │   └── jobs.spec.ts    # Jobs store tests
│   ├── services/
│   │   └── api-client.spec.ts  # HTTP client tests
│   ├── components/
│   │   ├── LoginForm.spec.ts   # Login form tests
│   │   ├── Dashboard.spec.ts   # Dashboard tests
│   │   └── MainLayout.spec.ts  # Layout tests
│   └── utils/
│       └── index.spec.ts    # Validation & utility tests
└── e2e/
    ├── auth.spec.ts        # Authentication flows
    ├── cms.spec.ts         # Content management workflows
    ├── shop.spec.ts        # Product management workflows
    └── jobs.spec.ts        # Job monitoring workflows
```

## Running Tests

### All Tests

```bash
npm run test
```

### Unit Tests Only

```bash
npm run test unit/
```

### E2E Tests Only

```bash
npm run e2e
```

### Watch Mode

```bash
npm run test -- --watch
```

### Coverage Report

```bash
npm run test:coverage
```

## Test Configuration

### Vitest (`vitest.config.ts`)

- **Environment**: happy-dom (lightweight DOM)
- **Coverage Threshold**: 70% minimum
- **Globals**: true (no need to import test functions)

### Setup File (`tests/setup.ts`)

Global mocks for:

- `localStorage`
- `sessionStorage`
- `window.matchMedia`

## Unit Tests

### Store Tests

Each Pinia store has comprehensive unit tests covering:

- Initial state validation
- Async operations (loading states)
- Error handling
- State mutations
- Computed properties

**Example: Auth Store** (`tests/unit/stores/auth.spec.ts`)

```typescript
// Tests login flow
it('should login successfully', async () => {
  const store = useAuthStore();
  await store.login('admin@example.com', 'password');

  expect(store.isAuthenticated).toBe(true);
  expect(store.user).toBeDefined();
  expect(store.token).toBeDefined();
});
```

### API Client Tests

HTTP client tests verify:

- Request method handling (GET, POST, PUT, DELETE)
- Interceptor functionality (token injection, Tenant ID header)
- Error scenarios (401, network failures)
- Response parsing

### Utility Tests

Test coverage for:

- **Form Validation**
  - `validateEmail()` - Email format validation
  - `validatePassword()` - Password strength requirements
  - `validateURL()` - URL validation
  - `validatePhoneNumber()` - Phone number validation

- **Permission Checking**
  - `hasPermission()` - Single permission check
  - `hasAnyPermission()` - Multiple OR logic
  - `hasAllPermissions()` - Multiple AND logic

- **Date Formatting**
  - `formatDate()` - Date formatting
  - `formatDateTime()` - Date + time formatting
  - `formatTime()` - Time only formatting

## Component Tests

### LoginForm.vue

Tests the login form component:

- ✅ Form field rendering
- ✅ Input value updates
- ✅ Remember-me checkbox toggle
- ✅ Form validation
- ✅ Loading state (button disabled)
- ✅ Error message display
- ✅ Keyboard navigation (accessibility)
- ✅ Label associations (accessibility)

```typescript
it('should update email input on user input', async () => {
  const wrapper = mount(LoginForm);
  const emailInput = wrapper.find('input[type="email"]');
  await emailInput.setValue('admin@example.com');

  expect((emailInput.element as HTMLInputElement).value).toBe('admin@example.com');
});
```

### Dashboard.vue

Tests the dashboard component:

- ✅ Page rendering and heading
- ✅ Stats cards display
- ✅ Page count statistics
- ✅ Product count statistics
- ✅ Active jobs display
- ✅ Quick action links
- ✅ User greeting with name
- ✅ Responsive behavior
- ✅ Semantic HTML structure

### MainLayout.vue

Tests the main layout component:

- ✅ Header and navigation rendering
- ✅ Sidebar navigation items
- ✅ User menu display
- ✅ Logout button
- ✅ Mobile menu toggle
- ✅ Active item indicator
- ✅ RouterView outlet
- ✅ Responsive layout
- ✅ ARIA roles and labels
- ✅ Keyboard navigation

## E2E Tests

### Authentication Flow (`tests/e2e/auth.spec.ts`)

Tests the complete login workflow:

1. **Login Page Tests**
   - Login form visibility
   - Email and password input fields
   - Remember-me checkbox
   - Submit button

2. **Validation Tests**
   - Invalid credentials error handling
   - Form validation
   - Error message display

3. **Navigation Tests**
   - Navigation after successful login
   - Dashboard access after authentication
   - Redirect to login for unauthenticated users

4. **Responsive Design Tests**
   - Mobile (375x667px)
   - Tablet (768x1024px)
   - Desktop (1920x1080px)

5. **Accessibility Tests**
   - Keyboard navigation (Tab key)
   - Form labels
   - ARIA attributes

6. **Error Handling**
   - Invalid credentials
   - Network errors
   - Timeout scenarios

### CMS Module Flow (`tests/e2e/cms.spec.ts`)

Tests content management workflows:

1. **Page List Tests**
   - Pages list rendering
   - Empty state display
   - Data table display

2. **Navigation Tests**
   - Navigate to page editor
   - Navigate to new page creation
   - Route transitions

3. **Pagination & Filtering**
   - Pagination controls
   - Filter functionality (scaffold)
   - Data loading states

4. **Responsive Design**
   - Mobile list display
   - Tablet data table
   - Desktop full table view

5. **Error Scenarios**
   - API errors (500)
   - Network timeouts
   - No data states

6. **Accessibility**
   - Keyboard navigation
   - Screen reader compatibility
   - Focus management

### Shop Module Flow (`tests/e2e/shop.spec.ts`)

Tests product management workflows:

1. **Product List**
   - Display products list
   - Empty state
   - Stock levels display
   - Pricing display

2. **Navigation**
   - Edit product navigation
   - New product creation
   - Shop page transitions

3. **Category Management**
   - Category list access
   - Category operations

4. **Pricing Management**
   - Pricing rules display
   - Pricing operations

5. **Responsive Design**
   - Mobile view
   - Tablet view
   - Desktop table view

6. **Error Handling**
   - API failures
   - Network timeouts

### Jobs Module Flow (`tests/e2e/jobs.spec.ts`)

Tests job monitoring workflows:

1. **Job Queue Monitoring**
   - Job list display
   - Empty state
   - Progress bars
   - Status indicators

2. **Job Actions**
   - Retry button for failed jobs
   - Cancel button for running jobs
   - Retry functionality

3. **Job Details**
   - Navigate to job details
   - Job information display
   - Log viewing

4. **Job History**
   - Completed jobs display
   - Job history filtering
   - Duration calculation

5. **Scheduled Jobs**
   - Scheduled jobs list
   - Job scheduling interface
   - Cron expression display

6. **Job Metrics**
   - Success rate statistics
   - Average execution time
   - Job statistics dashboard

## Mocking Strategy

### Unit Tests (Vitest)

**Store/API Mocking**

```typescript
vi.mock('@/services/api/auth')

// In test:
vi.mocked(authApi.login).mockResolvedValue({
  success: true,
  data: { user: {...}, token: '...' }
})
```

**Global Mocks** (`tests/setup.ts`)

```typescript
// localStorage mock
const localStorage = {};

// sessionStorage mock
const sessionStorage = {};

// window.matchMedia mock
window.matchMedia = (query: string) => ({
  matches: false,
  media: query,
  onchange: null,
  addListener: () => {},
  removeListener: () => {},
  addEventListener: () => {},
  removeEventListener: () => {},
  dispatchEvent: () => true,
});
```

### E2E Tests (Playwright)

**API Route Mocking**

```typescript
await page.route('**/api/admin/auth/login', route => {
  route.fulfill({
    status: 200,
    body: JSON.stringify({
      success: true,
      data: { user: {...}, token: '...' }
    })
  })
})
```

## Coverage Goals

### Current Coverage Targets

- **Statements**: 70%
- **Branches**: 70%
- **Functions**: 70%
- **Lines**: 70%

### Coverage by Module

| Module        | Target | Status     |
| ------------- | ------ | ---------- |
| Stores        | 80%    | ✅ 100%    |
| API Services  | 80%    | ✅ 100%    |
| Utils         | 90%    | ✅ 100%    |
| Components    | 75%    | ✅ 85%     |
| Composables   | 75%    | 🟡 Pending |
| Router/Guards | 80%    | 🟡 Pending |

## Continuous Integration

### GitHub Actions Setup

Tests run on:

- Pull requests
- Commits to main branch
- Scheduled daily

### Test Output

- Test results in GitHub Actions
- Coverage reports uploaded to Codecov
- Failed tests block merge

## Debugging Tests

### Debug Unit Tests

```bash
npm run test -- --inspect-brk
```

### Debug E2E Tests

```bash
# Run in headed mode to see browser
npm run e2e -- --headed

# Use slow motion to see interactions
npm run e2e -- --headed --debug
```

### View Test Reports

```bash
# Playwright test report
npx playwright show-report

# Coverage report
open coverage/index.html
```

## Best Practices

### Unit Tests

1. ✅ One assertion focus per test
2. ✅ Use descriptive test names
3. ✅ Arrange-Act-Assert pattern
4. ✅ Mock external dependencies
5. ✅ Test both success and error paths

### E2E Tests

1. ✅ Test user-centric workflows
2. ✅ Use data-testid for selectors
3. ✅ Wait for async operations
4. ✅ Test responsive viewports
5. ✅ Include accessibility checks

### General

1. ✅ Keep tests isolated and independent
2. ✅ Use meaningful assertions
3. ✅ Avoid test interdependencies
4. ✅ Clean up after tests
5. ✅ Document complex test scenarios

## Common Issues & Solutions

### Tests Timeout

```bash
# Increase timeout
npm run test -- --testTimeout 10000
```

### Mock Not Working

- Check mock path matches import
- Mock must be before first import
- Clear mocks between tests: `vi.clearAllMocks()`

### Playwright Headless Mode Issues

- Use `--headed` to debug
- Check selectors with `page.pause()`
- Enable debug logs: `DEBUG=pw:api`

## Future Enhancements

- [ ] Visual regression testing with Percy
- [ ] Performance benchmarking
- [ ] Integration test suite
- [ ] Load testing setup
- [ ] Accessibility audit automation
- [ ] Test analytics dashboard

## Resources

- [Vitest Documentation](https://vitest.dev/)
- [Vue Test Utils](https://test-utils.vuejs.org/)
- [Playwright Documentation](https://playwright.dev/)
- [Pinia Testing](https://pinia.vuejs.org/cookbook/testing.html)

## Contact & Support

For test-related questions or issues:

1. Check this documentation
2. Review test comments in spec files
3. Check CI/CD logs for failures
4. Consult team lead
