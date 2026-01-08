---
docid: KB-130
title: Quality E2e Testing 2026
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: quality-e2e-testing-2026
title: 6. Januar 2026 - Systematic E2E Testing Implementation
category: quality
migrated: 2026-01-08
---
### Chunked E2E Testing Strategy

**Issue**: Running full e2e test suite caused overwhelming failures and debugging complexity.

**Root Cause**: Attempting to run all tests simultaneously without infrastructure validation.

**Lesson**: Break e2e testing into manageable chunks with progressive complexity.

**Solution**: Implement systematic chunked testing approach:
1. **Build Health Tests**: Verify basic functionality and server connectivity
2. **Authentication Tests**: Test login/logout flows and session management
3. **Responsive Design Tests**: Validate layouts across breakpoints
4. **Visual Regression Tests**: Establish baseline screenshots for future comparison

**Benefits**:
- Isolates issues to specific functional areas
- Allows incremental fixes and validation
- Builds confidence with each successful chunk
- Provides clear progress tracking

### Playwright BaseURL Configuration Management

**Issue**: Tests failing with connection refused errors despite server running.

**Root Cause**: Playwright baseURL hardcoded to wrong port, server using alternative ports.

**Lesson**: Use dynamic baseURL configuration that matches actual server ports.

**Solution**: Configure Playwright to use correct baseURL:
```typescript
// playwright.config.ts
export default defineConfig({
  use: {
    baseURL: 'http://localhost:3000', // Match actual server port
    // ... other config
  },
  webServer: {
    command: 'npm run dev',
    port: 3000,
    reuseExistingServer: !process.env.CI,
  },
});
```

### Server Port Conflict Resolution

**Issue**: Multiple services competing for ports, WebSocket server errors.

**Root Cause**: Concurrent frontend services (Store, Admin, Management) using overlapping ports.

**Lesson**: Implement proper port allocation and conflict resolution.

**Solution**: Use distinct ports for each service:
- Store: Port 3000 (Nuxt)
- Admin: Port 5179 (Vite)
- Management: Port 5176 (Vite)
- Kill conflicting processes: `pkill -f "nuxt\|vite"`

### Visual Regression Baseline Management

**Issue**: No established baseline for visual regression testing.

**Root Cause**: First-time setup required baseline screenshot creation.

**Lesson**: Always run visual regression tests first to create baselines after UI changes.

**Solution**: Establish baseline workflow:
1. Run tests to generate initial snapshots
2. Store in `tests/e2e/*.spec.ts-snapshots/` directory
3. Update baselines after intentional UI changes
4. Use in CI/CD for automated regression detection

### CSS Import Path Resolution for Testing

**Issue**: Server failing to start during e2e testing due to CSS import errors.

**Root Cause**: DaisyUI import paths not resolving correctly in test environment.

**Lesson**: Use root-relative paths for shared dependencies in monorepo structures.

**Solution**: Update CSS imports to use correct relative paths:
```css
/* For Store frontend (nested in monorepo) */
@import "../../../node_modules/daisyui/daisyui.css";

/* NOT this (doesn't work from nested directories) */
@import "../../node_modules/daisyui/daisyui.css";
```

### Systematic Debugging for E2E Tests

**Issue**: Difficult to diagnose connectivity and configuration issues.

**Root Cause**: Lack of systematic debugging approach for e2e test failures.

**Lesson**: Use structured debugging process for e2e test issues.

**Solution**: Implement debugging checklist:
1. **Server Status**: `curl -I http://localhost:PORT` to verify server response
2. **Port Usage**: `lsof -i :PORT` to check port allocation
3. **Process Management**: `ps aux | grep nuxt` to verify server processes
4. **Configuration**: Verify baseURL matches actual server port
5. **Cleanup**: Kill conflicting processes before restarting

### Background Server Management for Testing

**Issue**: Server instability during long test runs.

**Root Cause**: Foreground server processes interrupted by test execution.

**Lesson**: Run development servers in background for stable testing.

**Solution**: Use background execution for server management:
```bash
# Start server in background
npm run dev > /dev/null 2>&1 &

# Wait for startup
sleep 10

# Verify server is responding
curl -s http://localhost:3000/ | head -1

# Run tests
npx playwright test

# Cleanup
pkill -f "npm run dev"
```
