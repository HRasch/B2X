## PR Description

### What does this PR do?
[Clear description of changes]

### Related Issues
Closes #[issue number]

---

## Type of Change

- [ ] 🐛 Bug fix (non-breaking change fixing an issue)
- [ ] ✨ Feature (non-breaking change adding functionality)
- [ ] 🚀 Performance improvement
- [ ] 📚 Documentation update
- [ ] 🔒 Security fix
- [ ] 🔄 Refactoring
- [ ] ⚙️ Configuration change
- [ ] 🧪 Test update

---

## Changes Made

### Backend Changes
- [ ] New service/controller: [name]
- [ ] Modified service: [name]
- [ ] Database migration: [migration name]
- [ ] Configuration change: [description]
- [ ] Dependency update: [package name, version]

### Frontend Changes
- [ ] New component: [component name]
- [ ] Modified component: [component name]
- [ ] New page/view: [page name]
- [ ] Style changes: [description]

### Infrastructure/Config Changes
- [ ] Docker configuration
- [ ] Kubernetes configuration
- [ ] Environment variables
- [ ] GitHub Actions workflow

---

## Testing

### Tests Added/Updated
- [ ] Unit tests: [service/component name]
- [ ] Integration tests: [service name]
- [ ] E2E tests: [feature name]

### Test Coverage
- Current coverage: [X%]
- New coverage: [Y%]
- Coverage change: [+/- Z%]

### Manual Testing
- [ ] Tested locally with development environment
- [ ] Tested with production configuration
- [ ] No regressions in related features

#### Test Commands Run
```bash
# Example:
dotnet test backend/B2Connect.slnx
npm run test --prefix frontend-admin
```

---

## Security & Compliance

### Security Checklist
- [ ] No hardcoded secrets/credentials
- [ ] No SQL injection vulnerabilities
- [ ] No XSS vulnerabilities
- [ ] Proper input validation
- [ ] Proper authentication/authorization
- [ ] Sensitive data encrypted
- [ ] No PII in logs

### GDPR Compliance
- [ ] No unnecessary data collection
- [ ] User consent documented
- [ ] Data retention policy followed
- [ ] Right-to-be-forgotten considered

---

## Performance Impact

### Before
- [e.g., API response time: 200ms]
- [Database queries: N+1 issue]

### After
- [e.g., API response time: 50ms]
- [Database queries: 1 optimized query]

### Load Test Results
```
[Paste load test results if applicable]
```

---

## Breaking Changes

- [ ] No breaking changes
- [ ] Breaking changes (see below)

If breaking changes exist:
- [ ] Database schema change (migration required)
- [ ] API endpoint change (version bump required)
- [ ] Configuration change (documentation updated)
- [ ] Dependency version constraint changed

**Migration Guide:**
[Describe how to migrate from old behavior to new behavior]

---

## Checklist

- [ ] Code follows project style guidelines
- [ ] Self-review of code completed
- [ ] Comments added for complex logic
- [ ] Documentation updated (README, API docs, etc.)
- [ ] No new warnings generated
- [ ] All tests passing
- [ ] Build successful
- [ ] Screenshots/GIFs added (if UI change)
- [ ] Related issues linked

---

## Reviewers
@[reviewer1] @[reviewer2]

---

## Merge Requirements

- [ ] At least 2 approvals required
- [ ] All checks passing (build, tests, linting)
- [ ] No merge conflicts
- [ ] Commit history clean (squash if needed)
- [ ] Ready for production deployment

---

## Notes for Reviewers

[Add any additional context that reviewers should know about]
