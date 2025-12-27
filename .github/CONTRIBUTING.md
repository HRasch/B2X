# Contributing to B2Connect

Thank you for considering contributing to B2Connect! This document provides guidelines and instructions for contributing.

---

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [Getting Started](#getting-started)
3. [Development Setup](#development-setup)
4. [Making Changes](#making-changes)
5. [Submitting Changes](#submitting-changes)
6. [Code Review Process](#code-review-process)
7. [Community](#community)

---

## Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inspiring community for all. Please read and abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

### Expected Behavior

- Be respectful and inclusive
- Focus on constructive feedback
- Assume good intent
- Welcome different perspectives

### Unacceptable Behavior

- Harassment, discrimination, or abuse
- Intentionally disruptive behavior
- Publishing private information

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL (or use in-memory for development)

### Installation

```bash
# Clone repository
git clone https://github.com/b2connect/B2Connect.git
cd B2Connect

# Install backend dependencies (automatic with .NET)
dotnet restore

# Install frontend dependencies
npm install --prefix frontend-admin
npm install --prefix frontend-store

# Setup database (optional - in-memory by default)
# See docs/guides/DEVELOPMENT.md for database setup
```

### Quick Start

```bash
# Start backend
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj

# Start frontend (in separate terminal)
npm run dev --prefix frontend-admin

# Navigate to http://localhost:5174
```

---

## Development Setup

### Branch from develop

```bash
# Update local develop branch
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/your-feature-name
```

### Branch Naming

```
feature/     - New features
bugfix/      - Bug fixes
hotfix/      - Critical fixes (from main)
refactor/    - Code refactoring
docs/        - Documentation
test/        - Tests
perf/        - Performance improvements
sec/         - Security improvements

Examples:
✅ feature/gdpr-data-export
✅ bugfix/cors-validation
❌ fix-stuff (too vague)
```

### Commit Early & Often

```bash
# Make frequent, atomic commits
git add .
git commit -m "feat(auth): add JWT refresh token"

# Push daily
git push origin feature/your-feature-name
```

---

## Making Changes

### Code Style

#### Backend (.NET)

```csharp
// ✅ Good: Clear naming, proper structure
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;
    
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        var user = await _repository.GetByIdAsync(id);
        if (user is null)
            throw new UserNotFoundException(id);
            
        return user;
    }
}

// ❌ Bad: Unclear naming, poor structure
public class Us
{
    public User GetUser(Guid id)
    {
        return db.Users.FirstOrDefault(u => u.Id == id);
    }
}
```

**Guidelines:**
- Use meaningful variable/method names
- Keep methods focused (single responsibility)
- Use async/await for I/O operations
- Add XML comments for public APIs
- Use nullable reference types
- Prefer immutability

#### Frontend (Vue 3 + TypeScript)

```vue
<!-- ✅ Good: TypeScript, reactive data, clear structure -->
<template>
  <div class="user-list">
    <div v-for="user in filteredUsers" :key="user.id" class="user-item">
      {{ user.name }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import type { User } from '@/types'

const users = ref<User[]>([])
const searchTerm = ref('')

const filteredUsers = computed(() =>
  users.value.filter(u => 
    u.name.toLowerCase().includes(searchTerm.value.toLowerCase())
  )
)
</script>

<style scoped>
.user-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>

<!-- ❌ Bad: No TypeScript, no structure -->
<template>
  <div>
    <div v-for="u in u" :key="u.id">{{ u.n }}</div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      u: []
    }
  }
}
</script>
```

**Guidelines:**
- Use TypeScript for all components
- Use Composition API (setup syntax)
- Keep components focused
- Use scoped styles
- Add comments for complex logic
- Use meaningful naming

### Testing

#### Write Tests for:
- All business logic
- Service methods
- Complex calculations
- Validation rules
- Authentication/authorization

#### Don't Write Tests for:
- Trivial getters/setters
- UI framework behavior
- Third-party library behavior

#### Example Unit Test

```csharp
[Fact]
public async Task CreateUser_WithValidData_CreatesUserSuccessfully()
{
    // Arrange
    var userRepository = new Mock<IUserRepository>();
    var logger = new Mock<ILogger<UserService>>();
    var service = new UserService(userRepository.Object, logger.Object);
    
    var command = new CreateUserCommand 
    { 
        Email = "test@example.com",
        FirstName = "Test"
    };
    
    // Act
    var result = await service.CreateUserAsync(command);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("test@example.com", result.Email);
    userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
}
```

#### Run Tests

```bash
# Backend tests
dotnet test backend/B2Connect.slnx

# Frontend tests
npm run test --prefix frontend-admin

# E2E tests
npm run test:e2e --prefix frontend-admin
```

### Security Checklist

Before submitting, verify:

- [ ] No hardcoded secrets/credentials
- [ ] No PII in logs
- [ ] Input validation present
- [ ] SQL injection protection (parameterized queries)
- [ ] XSS protection (output encoding)
- [ ] Authentication/authorization checks
- [ ] Sensitive data encrypted
- [ ] Error messages don't expose internals

### Documentation

#### Update Documentation For:
- New features
- API changes
- Configuration changes
- Breaking changes
- Setup instructions

#### Documentation Template

```markdown
## Feature Name

### Description
What does this feature do?

### Usage
How do you use it?

### Examples
```csharp
// Code examples
```

### Configuration
What configuration is needed?

### Related Documentation
Links to related docs
```

---

## Submitting Changes

### Create a Pull Request

1. **Push your branch**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create PR on GitHub**
   - Use the PR template from `.github/pull_request_template.md`
   - Link related issues
   - Describe what changed and why

3. **PR Checklist**
   - [ ] Tests added/updated
   - [ ] Code follows style guide
   - [ ] Documentation updated
   - [ ] No breaking changes (or documented)
   - [ ] Commit messages follow convention

### Commit Message Format

**Use Conventional Commits:**

```
<type>(<scope>): <subject>

<body>

<footer>

Types: feat, fix, docs, style, refactor, perf, test, chore, ci
Scope: area affected (auth, catalog, etc.)
Subject: Imperative, lowercase, no period, max 50 chars
Footer: Fixes #123, Related to #124
```

**Examples:**

```
feat(auth): implement JWT refresh token

Add automatic token refresh on token expiration.
Refreshes happen transparently to the user.

Fixes #123

chore(deps): update .NET to 10.0.1

fix(cors): allow production domains

Move CORS configuration to environment variables
to support production deployment.

Related to #45
BREAKING: Requires CORS__ALLOWEDORIGINS env var
```

---

## Code Review Process

### What Reviewers Look For

✅ **Functionality:** Does it work as intended?  
✅ **Quality:** Is the code clean and maintainable?  
✅ **Testing:** Are tests adequate?  
✅ **Security:** Are there security issues?  
✅ **Performance:** Will it impact performance?  
✅ **Documentation:** Is it documented?  

### Review Comments

**If You Receive Comments:**

1. Read the comment carefully
2. Don't dismiss without understanding
3. Respond to every comment
4. Make requested changes or discuss
5. Re-request review
6. Update PR as needed

**Example Response:**
```
"Good catch! Updated in commit abc123 to validate 
email format before saving."
```

### Approval Requirements

- **For main:** 2 approvals required
- **For develop:** 1 approval minimum
- **Special cases:** Lead architect, security reviewer, or DB expert approval may be required

---

## Community

### Getting Help

- **Documentation:** [docs/DOCUMENTATION_INDEX.md](docs/DOCUMENTATION_INDEX.md)
- **Issues:** [GitHub Issues](https://github.com/b2connect/B2Connect/issues)
- **Discussions:** [GitHub Discussions](https://github.com/b2connect/B2Connect/discussions)
- **Slack:** [B2Connect Slack Channel](#)

### Questions?

- Check [docs/guides/DEBUG_QUICK_REFERENCE.md](docs/guides/DEBUG_QUICK_REFERENCE.md)
- Search existing issues
- Create a new issue if needed
- Ask in GitHub Discussions

### Report Security Issues

**Do not open public issues for security vulnerabilities.**

Email: security@b2connect.com

---

## Development Workflow Example

### Week 1: GDPR Feature Development

```bash
# Monday morning
git checkout develop
git pull origin develop
git checkout -b feature/gdpr-data-export

# During week - make changes
echo "Working on data export..." > file.cs
git add .
git commit -m "feat(gdpr): add data export endpoint"
git push origin feature/gdpr-data-export

# Wednesday - ready for review
# Create PR on GitHub
# Link to issue #234

# Thursday - code review
# Address comments
# Make updates
git commit -m "review: address feedback on data export"
git push origin feature/gdpr-data-export

# Friday - approved & merged
# PR merged to develop
# Deploy to staging next week
```

---

## Tips for Success

### ✅ DO

```
✅ Write clear, descriptive commit messages
✅ Test your changes locally
✅ Keep PRs focused and reasonably sized
✅ Respond to review comments quickly
✅ Keep learning and asking questions
✅ Help other developers with reviews
✅ Document as you go
✅ Break large changes into smaller PRs
```

### ❌ DON'T

```
❌ Commit directly to main/develop
❌ Force push shared branches
❌ Ignore failing tests
❌ Commit secrets or credentials
❌ Leave PRs unreviewed for days
❌ Dismiss code review feedback
❌ Create massive 1000-line PRs
❌ Ignore documentation
```

---

## Recognition

Contributors will be recognized in:
- [CONTRIBUTORS.md](CONTRIBUTORS.md) (weekly update)
- Release notes (for each release)
- Project dashboard

---

## Questions?

- 📖 Read the [docs/DOCUMENTATION_INDEX.md](docs/DOCUMENTATION_INDEX.md)
- 💬 Ask in GitHub Discussions
- 🐛 Report issues on GitHub Issues
- 📧 Email: dev-team@b2connect.com

---

**Last Updated:** 27. Dezember 2025  
**Maintained By:** B2Connect Engineering Team
