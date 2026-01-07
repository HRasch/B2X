# GitHub PR Setup & Configuration

**Instructions for setting up GitHub to enforce the PR workflow**

---

## 📋 GitHub Branch Protection Rules

### Enable for `main` branch

**Location**: Repository Settings → Branches → Add Rule for `main`

```yaml
Rule Configuration:
├── Pattern: main
├── Require a pull request before merging: ✅ TRUE
│   ├── Require approvals: ✅ TRUE
│   │   └── Required number of approvals: 3 (Lead Dev + Architecture + Code Owner)
│   ├── Dismiss stale pull request approvals: ✅ TRUE
│   ├── Require code owner review: ✅ TRUE (if CODEOWNERS file exists)
│   └── Require conversation resolution: ✅ TRUE
│
├── Require status checks to pass before merging: ✅ TRUE
│   ├── Required status checks:
│   │   ├── ✅ build (GitHub Actions)
│   │   ├── ✅ test (GitHub Actions)
│   │   ├── ✅ coverage (GitHub Actions)
│   │   ├── ✅ code-style (GitHub Actions)
│   │   └── ✅ security-scan (GitHub Actions)
│   └── Require branches to be up to date: ✅ TRUE
│
├── Require commits to be signed: ✅ FALSE (optional, can enable later)
│
├── Restrict who can push to matching branches: ✅ FALSE (team can force-push if needed)
│
└── Allow force pushes: ✅ FALSE (prevent accidental overwrites)
```

**Result**: 
- ✅ Cannot merge without 3 approvals
- ✅ All checks must pass (build, test, coverage)
- ✅ Conversations must be resolved
- ✅ PR title/description required

---

## 🤖 GitHub Actions Workflows

### Build & Test Workflow

**File**: `.github/workflows/pr-checks.yml`

```yaml
name: PR Checks

on:
  pull_request:
    branches: [main]
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0'
      
      - name: Restore
        run: dotnet restore
      
      - name: Build
        run: dotnet build B2X.slnx --configuration Release --no-restore
        
      - name: Test
        run: dotnet test B2X.slnx --configuration Release --no-build --logger "trx" --collect:"XPlat Code Coverage"
        
      - name: Check Coverage
        run: |
          # Parse coverage and fail if < 80%
          COVERAGE_FILE=$(find . -name "coverage.cobertura.xml" | head -1)
          if [ -z "$COVERAGE_FILE" ]; then
            echo "Coverage file not found"
            exit 1
          fi
          
          # Extract line coverage percentage
          COVERAGE=$(grep -oP 'line-rate="\K[0-9.]+' "$COVERAGE_FILE" | head -1)
          COVERAGE_PCT=$(awk "BEGIN {print int($COVERAGE * 100)}")
          
          echo "Code Coverage: ${COVERAGE_PCT}%"
          if [ $COVERAGE_PCT -lt 80 ]; then
            echo "❌ Coverage too low (${COVERAGE_PCT}% < 80%)"
            exit 1
          fi
          echo "✅ Coverage acceptable"
      
      - name: Code Style
        run: dotnet format --verify-no-changes
      
      - name: Security Scan
        run: |
          echo "Scanning for hardcoded secrets..."
          if grep -r "password\|secret\|api_key" backend/ --include="*.cs"; then
            echo "❌ Hardcoded secrets detected"
            exit 1
          fi
          echo "✅ No hardcoded secrets"
      
      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: '**/TestResults/**'
      
      - name: Upload Coverage
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: coverage-report
          path: '**/coverage/**'
```

---

## 👥 GitHub CODEOWNERS File

**File**: `.github/CODEOWNERS`

```
# Define code ownership for automatic reviewer requests

# Lead Developer (reviews all code)
* @lead-developer

# Backend ownership
/backend/Domain/Identity/ @backend-lead @identity-owner
/backend/Domain/Catalog/ @backend-lead @catalog-owner
/backend/Domain/CMS/ @backend-lead @cms-owner
/backend/Domain/Localization/ @backend-lead @localization-owner

# Frontend ownership
/Frontend/Store/ @frontend-lead @store-owner
/Frontend/Admin/ @frontend-lead @admin-owner

# Infrastructure ownership
/backend/Orchestration/ @devops-lead
/kubernetes/ @devops-lead
/docker-compose.yml @devops-lead

# Documentation ownership
/docs/ @documentation-lead @tech-lead
README.md @tech-lead

# Process ownership
/.github/ @scrum-master @tech-lead
```

**Result**: GitHub automatically requests appropriate reviewers

---

## 📝 GitHub PR Templates

### Main PR Template

**File**: `.github/pull_request_template.md`

```markdown
## 🎯 What
<!-- Brief description of what changed -->

## 📋 Why  
<!-- Why this change was needed - link to issue -->

Closes #XXX

## 🔍 How
<!-- Technical approach taken -->

- Point 1
- Point 2
- Point 3

## 🧪 Testing
<!-- What was tested -->

- [ ] Unit tests added (X new tests)
- [ ] Integration tests passing
- [ ] Manual testing completed
- [ ] Coverage: __% (target: 80%+)

## 📚 Documentation
<!-- What documentation was updated -->

- [ ] Swagger/OpenAPI updated
- [ ] Changelog entry added
- [ ] Code comments added
- [ ] Runbook updated (if needed)

## 🔐 Security & Compliance
<!-- Security & compliance verification -->

- [ ] No PII exposed
- [ ] Credentials in vault
- [ ] Multi-tenant isolation verified
- [ ] Audit logging in place

## 🤖 Agent Changes
<!-- Mark any AI-generated code -->

<!-- Option A: No AI code -->
None

<!-- Option B: AI-generated sections -->
<!-- 
Sections:
- [Service] created (Issue #XXX) - Contact: Lead Dev for modifications
-->

## 📸 Screenshots (if UI change)
<!-- Add before/after screenshots -->

## Related Issues
<!-- Link related issues -->

- Closes #XXX
- Related #YYY
- Blocks #ZZZ

## Checklist
- [ ] Code compiles and tests pass
- [ ] Code coverage >= 80%
- [ ] Documentation complete
- [ ] No hardcoded secrets
- [ ] Ready for review
```

---

## 🏷️ GitHub Labels

### Setup Labels

**Location**: Repository → Labels → Create New

```yaml
# Status Labels
status:ready-for-dev
├── Description: "Issue ready for development (DoR approved)"
├── Color: Green (#0366d6)
└── Used: By Scrum Master when issue passes DoR review

status:in-progress
├── Description: "Developer actively working on this"
├── Color: Blue (#1f6feb)
└── Used: Auto-assigned when PR created

status:waiting-review
├── Description: "Waiting for code review (PR opened)"
├── Color: Yellow (#fff8c5)
└── Used: Auto-assigned when PR created

status:blocked
├── Description: "Cannot progress (blocked by dependency)"
├── Color: Red (#d1242f)
└── Used: When issue is blocked

# Type Labels
type:feat
├── Description: "New feature"
├── Color: Green (#a2eeef)

type:fix
├── Description: "Bug fix"
├── Color: Red (#ee0701)

type:docs
├── Description: "Documentation only"
├── Color: Blue (#0075ca)

type:test
├── Description: "Test addition/update"
├── Color: Purple (#7057ff)

type:chore
├── Description: "Maintenance (no code change)"
├── Color: Gray (#bfdadc)

# Priority Labels
priority:critical
├── Description: "Critical priority (P0)"
├── Color: Dark Red (#e11d21)

priority:high
├── Description: "High priority (P1)"
├── Color: Orange (#eb6420)

priority:medium
├── Description: "Medium priority (P2)"
├── Color: Yellow (#fbca04)

priority:low
├── Description: "Low priority (P3)"
├── Color: Light Green (#c5def5)

# Component Labels
comp:backend
comp:frontend
comp:devops
comp:docs
comp:infrastructure

# Approval Labels
approved:lead-dev
approved:architecture
approved:code-owner
needs-qa-test
qa-approved
```

---

## 🔔 Notification Settings

### GitHub → Team Slack Integration

**Setup**: GitHub Apps → Slack → Configure

```yaml
Notifications:
├── PR created: @channel in #pr-reviews
├── PR approved: Notify author in thread
├── PR changes requested: @author in #dev-questions
├── PR merged: Log to #deployments
└── Build failed: @lead-dev in #alerts
```

---

## ⚙️ PR Merge Settings

**Location**: Repository → Settings → General

```yaml
Pull Requests
├── Allow merge commits: ❌ FALSE
├── Allow squash merging: ✅ TRUE (default)
├── Allow rebase merging: ❌ FALSE (use squash instead)
└── Automatically delete head branches: ✅ TRUE
```

---

## 🚀 Deployment on Merge

### GitHub Actions: Auto-Deploy

**File**: `.github/workflows/deploy.yml`

```yaml
name: Deploy on Merge

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Build Docker images
        run: docker-compose build
      
      - name: Deploy to staging
        run: |
          docker-compose -f docker-compose.yml up -d
          
      - name: Run smoke tests
        run: |
          # Test that service is responding
          curl -f http://localhost:7002/health || exit 1
          
      - name: Notify deployment
        run: |
          echo "✅ Deployed to staging"
          # Send to Slack, email, etc.
```

---

## 📊 Metrics & Dashboards

### Track PR Metrics

**Manual tracking** (while waiting for integration):

```
Weekly PR Review:
├── Total PRs created: X
├── Average review time: X days
├── Average approvals time: X hours
├── Failed builds: X
├── Coverage avg: X%
└── Merged successfully: X%
```

**Spreadsheet Template**:
| PR # | Author | Title | Created | Merged | Days | Reviews | Status |
|------|--------|-------|---------|--------|------|---------|--------|

---

## ✅ Verification Checklist

Before going live with new PR process:

```
GitHub Configuration:
 [ ] Branch protection rules set on `main`
 [ ] Require 3 approvals
 [ ] Require status checks (build, test, coverage)
 [ ] Require conversation resolution
 
Workflows:
 [ ] .github/workflows/pr-checks.yml exists and runs
 [ ] Build succeeds on test PR
 [ ] Tests run on test PR
 [ ] Coverage calculated on test PR
 
CODEOWNERS:
 [ ] .github/CODEOWNERS file created
 [ ] All teams/owners listed
 [ ] Test: Create PR, reviewers auto-assigned
 
Templates:
 [ ] .github/pull_request_template.md created
 [ ] Test: Create new PR, see template
 
Labels:
 [ ] All labels created
 [ ] Team trained on label usage
 
Notifications:
 [ ] Slack integration working
 [ ] Notifications in correct channels
 [ ] Team receives alerts
 
Documentation:
 [ ] Team read PR_WORKFLOW_QUICK_GUIDE.md
 [ ] Team trained on process
 [ ] Questions answered
```

---

## 🔄 Weekly Maintenance

Every Friday, Scrum Master should:

```
[ ] Review PR metrics (see dashboard)
[ ] Check branch protection rules still active
[ ] Clean up old PR branches (GitHub auto-deletes, but verify)
[ ] Update labels if needed
[ ] Report metrics to team (retrospective)
[ ] Celebrate merged PRs! 🎉
```

---

## 📞 Support

**GitHub PR Process Issues?**

→ Ask in #dev-process or @tech-lead

**Specific to B2X?**

→ See [DEVELOPMENT_PROCESS_FRAMEWORK.md §7](../../../guides/DEVELOPMENT_PROCESS_FRAMEWORK.md#-pull-request-workflow)

**Quick guide?**

→ See [PR_WORKFLOW_QUICK_GUIDE.md](./PR_WORKFLOW_QUICK_GUIDE.md)
