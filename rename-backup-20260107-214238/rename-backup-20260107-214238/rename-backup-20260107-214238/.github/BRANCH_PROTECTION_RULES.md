# Branch Protection Rules für B2X

## Empfohlene GitHub Branch Protection Rules

### Für `main` Branch:
- ✅ **Require pull request reviews before merging**
  - Required approving reviews: 1
  - Dismiss stale pull request approvals when new commits are pushed: true
  - Require review from Code Owners: true

- ✅ **Require status checks to pass before merging**
  - Require branches to be up to date before merging: true
  - Status checks:
    - `backend-build` (Backend Build)
    - `frontend-lint` (Frontend Linting)
    - `frontend-format` (Code Formatting)
    - `test-backend` (Backend Tests)
    - `test-frontend` (Frontend Tests)

- ✅ **Require conversation resolution before merging**
- ✅ **Restrict pushes that create matching branches**
- ✅ **Require linear history**

### Für `develop` Branch:
- ✅ **Require pull request reviews before merging**
  - Required approving reviews: 1
- ✅ **Require status checks to pass**
- ✅ **Require conversation resolution**

### Für Feature Branches:
- ❌ Keine Protection (für schnelle Iteration)
- ✅ Pre-commit Hooks erzwingen lokale Qualität

## Automatisierte Checks

### CI/CD Pipeline (`pr-quality-gate.yml`):
- Backend: Build + Test + Format Check
- Frontend: Install + Format + Lint + Type Check
- Security: Secret Scanning

### Pre-commit Hooks:
- Prettier: Code Formatierung
- ESLint: Code Linting
- TypeScript: Type Checking

## Quality Gates

### Pull Request Requirements:
- ✅ Titel folgt Convention: `feat|fix|docs|refactor|test|chore: description`
- ✅ Beschreibung enthält Changes + Testing
- ✅ Labels: `frontend`, `backend`, `urgent`, etc.
- ✅ Assignees gesetzt
- ✅ CI/CD erfolgreich

### Code Review Checklist:
- ✅ Funktionalität getestet
- ✅ Code Style konform
- ✅ Tests hinzugefügt/geändert
- ✅ Dokumentation aktualisiert
- ✅ Breaking Changes dokumentiert

## Branch Strategy

```
main (protected)
├── develop (protected)
│   ├── feature/* (unprotected)
│   ├── bugfix/* (unprotected)
│   └── hotfix/* (protected)
```

## Monitoring

### Metriken tracken:
- PR Merge Time
- CI/CD Build Time
- Code Coverage
- Style Violation Count
- Review Comments per PR

### Alerts:
- Build failures
- Coverage below threshold
- Security vulnerabilities
- Style violations > 10 per PR