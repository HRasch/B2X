# Project Structure Migration Plan

## Overview

This document provides the detailed implementation plan for reorganizing the B2Connect project structure from the current dual-backend layout to a unified `src/backend/` structure with clear bounded contexts.

## Current Structure Issues

1. **Dual Backend Locations**: Projects split between `backend/` and `src/` folders
2. **Inconsistent Organization**: Mixed bounded context and feature-based organization
3. **Scattered Shared Components**: Shared code in multiple locations
4. **Complex Dependencies**: Hard to understand project relationships

## Target Structure

```
src/
├── backend/
│   ├── Admin/              # Admin bounded context
│   │   ├── API/           # Gateway/API layer (including AI APIs)
│   │   ├── Domain/        # Business logic
│   │   ├── Infrastructure/# Data access
│   │   ├── AI/            # Admin-specific AI services & APIs
│   │   ├── CLI/           # Admin-specific CLI tools
│   │   └── Tests/         # Tests
│   ├── Store/             # Store bounded context
│   │   ├── API/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   ├── AI/            # Store-specific AI services & APIs (future)
│   │   ├── CLI/           # Store-specific CLI tools (future)
│   │   └── Tests/
│   ├── Management/        # Management bounded context
│   │   ├── API/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   ├── AI/            # Management-specific AI services & APIs
│   │   ├── CLI/           # Management-specific CLI tools
│   │   └── Tests/
│   ├── Infrastructure/    # Cross-cutting infrastructure
│   │   ├── Hosting/       # AppHost, ServiceDefaults
│   │   ├── Monitoring/
│   │   ├── Messaging/
│   │   ├── Search/
│   │   ├── AI/            # Shared AI infrastructure (MCP servers)
│   │   └── Connectors/
│   ├── Services/          # Background services
│   │   ├── PunchoutAdapters/
│   │   ├── Search/
│   │   ├── Scheduler/      # Job scheduling and orchestration
│   │   └── BackgroundJobs/ # Message handling and maintenance jobs
│   └── Shared/            # Shared kernel
│       ├── Domain/        # Shared domain models
│       └── Infrastructure/# Shared infrastructure
├── frontend/              # All frontend applications
│   ├── Admin/
│   │   ├── AI/            # Admin AI views (Dashboard, Consumption, etc.)
│   ├── Store/
│   │   ├── AI/            # Store AI components (future)
│   ├── Management/
│   │   ├── AI/            # Management AI components (AiAssistant)
│   └── shared/            # Shared frontend components
│       ├── AI/            # Shared AI infrastructure (logs, common components)
├── tools/                 # Development tools
│   ├── AI/
│   ├── MCP/
│   └── seeders/
└── tests/                 # All tests (organized by bounded context)
    ├── Admin/
    ├── Store/
    ├── Management/
    ├── Infrastructure/
    ├── Services/
    └── Shared/
```

## AI Architecture in Bounded Contexts

### AI Service Organization

Each bounded context will have its own AI services and APIs that are tailored to that context's specific needs and permissions:

- **Admin AI**: Administrative management APIs (consumption tracking, provider management, system prompts)
- **Management AI**: Content management APIs (AI-assisted CMS operations, content generation)
- **Store AI**: Customer-facing AI APIs (product recommendations, customer service automation)
- **Admin CLI**: Administrative CLI tools (tenant management, system configuration)
- **Management CLI**: Content management CLI tools (bulk operations, AI-assisted content creation)
- **Store CLI**: Store-specific CLI tools (inventory management, order processing)

### AI Gateway Services

AI functionality will be exposed through each bounded context's gateway/API layer:

- `src/backend/Admin/AI/` - Admin-specific AI services and controllers
- `src/backend/Management/AI/` - Management-specific AI services and controllers  
- `src/backend/Store/AI/` - Store-specific AI services and controllers (future)
- `src/backend/Admin/CLI/` - Admin-specific CLI tools with AI capabilities
- `src/backend/Management/CLI/` - Management-specific CLI tools with AI capabilities
- `src/backend/Store/CLI/` - Store-specific CLI tools with AI capabilities (future)

### Shared AI Infrastructure

Cross-cutting AI infrastructure remains shared:

- `src/backend/Infrastructure/AI/` - MCP servers, shared AI utilities
- `src/frontend/shared/AI/` - Shared AI components and logging

This architecture ensures AI capabilities are properly scoped to each bounded context while maintaining shared infrastructure efficiency.

### Preparation Script

```powershell
# Create target directory structure
$targetDirs = @(
    "src/backend/Admin/API",
    "src/backend/Admin/Domain",
    "src/backend/Admin/Infrastructure",
    "src/backend/Admin/AI",
    "src/backend/Admin/CLI",
    "src/backend/Admin/Tests",
    "src/backend/Store/API",
    "src/backend/Store/Domain",
    "src/backend/Store/Infrastructure",
    "src/backend/Store/AI",
    "src/backend/Store/CLI",
    "src/backend/Store/Tests",
    "src/backend/Management/API",
    "src/backend/Management/Domain",
    "src/backend/Management/Infrastructure",
    "src/backend/Management/AI",
    "src/backend/Management/CLI",
    "src/backend/Management/Tests",
    "src/backend/Infrastructure/Hosting",
    "src/backend/Infrastructure/Monitoring",
    "src/backend/Infrastructure/Messaging",
    "src/backend/Infrastructure/Search",
    "src/backend/Infrastructure/AI",
    "src/backend/Infrastructure/Connectors",
    "src/backend/Infrastructure/ERP",
    "src/backend/Services/PunchoutAdapters",
    "src/backend/Services/Search",
    "src/backend/Services/Scheduler",
    "src/backend/Services/BackgroundJobs",
    "src/backend/Shared/Domain",
    "src/backend/Shared/Infrastructure",
    "src/frontend/Admin",
    "src/frontend/Admin/AI",
    "src/frontend/Store",
    "src/frontend/Store/AI",
    "src/frontend/Management",
    "src/frontend/Management/AI",
    "src/frontend/shared",
    "src/frontend/shared/AI",
    "src/tools/AI",
    "src/tools/MCP",
    "src/tools/seeders",
    "src/tests/Admin",
    "src/tests/Store",
    "src/tests/Management",
    "src/tests/Infrastructure",
    "src/tests/Services",
    "src/tests/Shared"
)

foreach ($dir in $targetDirs) {
    New-Item -ItemType Directory -Path $dir -Force
}
```

### Phase 1: Infrastructure Migration

```powershell
# Move infrastructure components
Move-Item "AppHost" "src/backend/Infrastructure/Hosting/AppHost" -Force
Move-Item "ServiceDefaults" "src/backend/Infrastructure/Hosting/ServiceDefaults" -Force
Move-Item "tools" "src/tools" -Force

# Move MCP servers to shared AI infrastructure
Move-Item "src/tools/B2XMCP" "src/backend/Infrastructure/AI/MCP/B2XMCP" -Force
Move-Item "src/tools/DatabaseMCP" "src/backend/Infrastructure/AI/MCP/DatabaseMCP" -Force
Move-Item "src/tools/DocumentationMCP" "src/backend/Infrastructure/AI/MCP/DocumentationMCP" -Force
Move-Item "src/tools/RoslynMCP" "src/backend/Infrastructure/AI/MCP/RoslynMCP" -Force
Move-Item "src/tools/WolverineMCP" "src/backend/Infrastructure/AI/MCP/WolverineMCP" -Force
```

### Phase 2: Shared Components Consolidation

```powershell
# Consolidate shared components from backend/
Copy-Item "backend/shared/*" "src/backend/Shared/Infrastructure/" -Recurse -Force

# Consolidate shared components from src/
Copy-Item "src/shared/Domain/*" "src/backend/Shared/Domain/" -Recurse -Force
Copy-Item "src/shared/B2X.Shared.*" "src/backend/Shared/Infrastructure/" -Recurse -Force
```

### Phase 3: Bounded Context Migration

```powershell
# Admin bounded context
Copy-Item "src/Admin/*" "src/backend/Admin/" -Recurse -Force
Copy-Item "backend/BoundedContexts/Admin/*" "src/backend/Admin/Domain/" -Recurse -Force
# Admin AI services and APIs will be moved to src/backend/Admin/AI/
# Admin CLI tools will be moved to src/backend/Admin/CLI/

# Store bounded context
Copy-Item "src/Store/*" "src/backend/Store/" -Recurse -Force
Copy-Item "backend/Domain/Catalog" "src/backend/Store/Domain/" -Recurse -Force
Copy-Item "backend/Domain/Customer" "src/backend/Store/Domain/" -Recurse -Force
Copy-Item "backend/Domain/Orders" "src/backend/Store/Domain/" -Recurse -Force
Copy-Item "backend/Domain/Search" "src/backend/Store/Domain/" -Recurse -Force
# Store AI services and APIs will be moved to src/backend/Store/AI/ (future)
# Store CLI tools will be moved to src/backend/Store/CLI/ (future)

# Management bounded context
Copy-Item "src/Management/*" "src/backend/Management/" -Recurse -Force
Copy-Item "backend/Domain/CMS" "src/backend/Management/Domain/" -Recurse -Force
Copy-Item "backend/Domain/Email" "src/backend/Management/Domain/" -Recurse -Force
# Management AI services and APIs will be moved to src/backend/Management/AI/
# Management CLI tools will be moved to src/backend/Management/CLI/
```

### Phase 3.5: CLI Tools Migration

```powershell
# Current CLI structure analysis shows:
# - backend/CLI/B2X.CLI.Administration/ (administration tools)
# - backend/CLI/B2X.CLI.Operations/ (store operations)
# - backend/CLI/B2X.CLI/ (management tools)
# - backend/CLI/B2X.CLI.Shared/ (shared utilities)
# - src/services/admin/cli/B2X.CLI.Administration/ (duplicate)
# - src/services/management/CLI/B2X.CLI/ (duplicate)
# - src/services/store/CLI/B2X.CLI.Operations/ (duplicate)

# Migrate CLI tools to bounded contexts (consolidate duplicates)

# Admin CLI tools (tenant management, system configuration)
Copy-Item "backend/CLI/B2X.CLI.Administration/*" "src/backend/Admin/CLI/" -Recurse -Force
# Remove duplicate if it exists
Remove-Item "src/services/admin/cli/B2X.CLI.Administration" -Recurse -Force -ErrorAction SilentlyContinue

# Management CLI tools (content management, bulk operations)
Copy-Item "backend/CLI/B2X.CLI/*" "src/backend/Management/CLI/" -Recurse -Force
# Remove duplicate if it exists
Remove-Item "src/services/management/CLI/B2X.CLI" -Recurse -Force -ErrorAction SilentlyContinue

# Store CLI tools (inventory, orders)
Copy-Item "backend/CLI/B2X.CLI.Operations/*" "src/backend/Store/CLI/" -Recurse -Force
# Remove duplicate if it exists
Remove-Item "src/services/store/CLI/B2X.CLI.Operations" -Recurse -Force -ErrorAction SilentlyContinue

# Shared CLI utilities (move to Shared infrastructure)
Copy-Item "backend/CLI/B2X.CLI.Shared/*" "src/backend/Shared/Infrastructure/" -Recurse -Force
```

### Phase 4: Services and Infrastructure

```powershell
# Move services
Copy-Item "backend/services/*" "src/backend/Services/" -Recurse -Force
Copy-Item "src/services/*" "src/backend/Services/" -Recurse -Force

# Move infrastructure components
Copy-Item "backend/Connectors/*" "src/backend/Infrastructure/Connectors/" -Recurse -Force
Copy-Item "src/Connectors/*" "src/backend/Infrastructure/Connectors/" -Recurse -Force
Copy-Item "src/erp-connector/*" "src/backend/Infrastructure/ERP/" -Recurse -Force
Copy-Item "src/IdsConnectAdapter/*" "src/backend/Services/PunchoutAdapters/" -Recurse -Force

# Create new scheduler and background jobs projects
# Note: These are new projects to be created post-migration
# B2X.Scheduler - Job scheduling and orchestration service
# B2X.BackgroundJobs - Message handling and maintenance jobs service
```

### Phase 5: Frontend Migration

```powershell
# Move frontend applications
Copy-Item "Frontend/*" "src/frontend/" -Recurse -Force

# Move bounded context-specific AI components
# Admin AI components
Copy-Item "src/frontend/Admin/src/views/ai" "src/frontend/Admin/AI/" -Recurse -Force

# Management AI components
Copy-Item "src/frontend/Management/src/components/AiAssistant.vue" "src/frontend/Management/AI/" -Force
Copy-Item "src/frontend/Management/src/services/aiService.ts" "src/frontend/Management/AI/" -Force

# Shared AI infrastructure (logs)
Copy-Item "src/frontend/.ai" "src/frontend/shared/AI/" -Recurse -Force
```

### Phase 6: Test Reorganization

```powershell
# Reorganize tests by bounded context
Copy-Item "tests/Admin/*" "src/tests/Admin/" -Recurse -Force
Copy-Item "tests/Store/*" "src/tests/Store/" -Recurse -Force
Copy-Item "tests/Management/*" "src/tests/Management/" -Recurse -Force
Copy-Item "backend/Tests/*" "src/tests/" -Recurse -Force
Copy-Item "tests/shared/*" "src/tests/Shared/" -Recurse -Force
Copy-Item "tests/services/*" "src/tests/Services/" -Recurse -Force
```

## Reference Update Scripts

### Update Project References

```powershell
# Function to update project references
function Update-ProjectReferences {
    param([string]$projectFile, [string]$oldPath, [string]$newPath)

    $content = Get-Content $projectFile -Raw
    $updatedContent = $content -replace [regex]::Escape($oldPath), $newPath
    Set-Content $projectFile $updatedContent
}

# Update all .csproj files
Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | ForEach-Object {
    # Update relative paths based on new structure
    Update-ProjectReferences $_.FullName "../../../shared/" "../../Shared/"
    Update-ProjectReferences $_.FullName "../../../../src/shared/" "../../Shared/"
    Update-ProjectReferences $_.FullName "../../../Hosting/" "../Infrastructure/Hosting/"
    # Add more path updates as needed
}
```

### Update Solution File

```powershell
# Update B2X.slnx with new structure
$solutionContent = Get-Content "B2X.slnx" -Raw

# Update folder paths
$solutionContent = $solutionContent -replace "/src/", "/src/backend/"
$solutionContent = $solutionContent -replace "/tests/", "/src/tests/"

# Update project paths
$solutionContent = $solutionContent -replace "src/([^/]+)" , "src/backend/$1"

Set-Content "B2X.slnx" $solutionContent
```

## Validation Scripts

### Build Validation

```powershell
# Test build after each phase
function Test-Build {
    dotnet build B2X.slnx
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        exit 1
    }
}

# Test specific projects
function Test-ProjectBuild {
    param([string]$projectPath)
    dotnet build $projectPath
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed for $projectPath"
        exit 1
    }
}
```

### Reference Validation

```powershell
# Check for broken references
function Test-References {
    Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | ForEach-Object {
        $projectFile = $_.FullName
        $content = Get-Content $projectFile -Raw

        # Find all ProjectReference elements
        $references = [regex]::Matches($content, '<ProjectReference Include="([^"]*)"')

        foreach ($ref in $references) {
            $refPath = $ref.Groups[1].Value
            if ($refPath -notmatch "^\$\(MSBuildThisFileDirectory\)") {
                # Convert relative path to absolute
                $absolutePath = [System.IO.Path]::GetFullPath(
                    [System.IO.Path]::Combine(
                        [System.IO.Path]::GetDirectoryName($projectFile),
                        $refPath
                    )
                )

                if (-not (Test-Path $absolutePath)) {
                    Write-Warning "Broken reference in $($_.Name): $refPath"
                }
            }
        }
    }
}
```

## Implementation Timeline

### Phase 3.5: Namespace Renaming (ADDED)
**Duration**: 1-2 days
**Purpose**: Update all namespaces to match new bounded context structure

#### Namespace Mapping Strategy

**Current → Target Namespace Changes:**

| Current Namespace Pattern | Target Namespace Pattern | Reason |
|---------------------------|--------------------------|---------|
| `B2X.Catalog.*` | `B2X.Store.Catalog.*` | Move to Store bounded context |
| `B2X.Orders.*` | `B2X.Store.Orders.*` | Move to Store bounded context |
| `B2X.Customer.*` | `B2X.Store.Customer.*` | Move to Store bounded context |
| `B2X.Search.*` | `B2X.Store.Search.*` | Move to Store bounded context |
| `B2X.CMS.*` | `B2X.Management.CMS.*` | Move to Management bounded context |
| `B2X.Email.*` | `B2X.Management.Email.*` | Move to Management bounded context |
| `B2X.Legal.*` | `B2X.Admin.Legal.*` | Move to Admin bounded context |
| `B2X.Compliance.*` | `B2X.Admin.Compliance.*` | Move to Admin bounded context |
| `B2X.AI.*` | `B2X.Admin.AI.*` or `B2X.Management.AI.*` or `B2X.Store.AI.*` | Context-specific AI |
| `B2X.CLI.*` | `B2X.Admin.CLI.*` or `B2X.Management.CLI.*` or `B2X.Store.CLI.*` | Context-specific CLI |
| `B2X.Shared.*` | `B2X.Shared.*` | No change (shared kernel) |
| `B2X.Tools.*` | `B2X.Tools.*` | No change (development tools) |
| `B2X.ERP.*` | `B2X.Shared.ERP.*` | Move to shared domain |
| `B2X.Identity.*` | `B2X.Shared.Identity.*` | Move to shared domain |
| `B2X.Localization.*` | `B2X.Shared.Localization.*` | Move to shared domain |
| `B2X.Tenancy.*` | `B2X.Shared.Tenancy.*` | Move to shared domain |
| `B2X.Security.*` | `B2X.Shared.Security.*` | Move to shared domain |
| `B2X.Theming.*` | `B2X.Shared.Theming.*` | Move to shared domain |

#### Files Requiring Namespace Updates:

1. **All .cs Files**: Update `namespace` declarations
2. **All .csproj Files**: Update `RootNamespace` property
3. **AssemblyInfo.cs Files**: Update assembly attributes
4. **Test Files**: Update test class namespaces
5. **Migration Files**: Update EF Core migration namespaces
6. **Configuration Files**: Update any hardcoded namespace references

#### Automated Namespace Renaming Process:

1. **Create Namespace Mapping File**:
   ```json
   {
     "B2X.Catalog": "B2X.Store.Catalog",
     "B2X.Orders": "B2X.Store.Orders",
     "B2X.CMS": "B2X.Management.CMS",
     // ... all mappings
   }
   ```

2. **Use Roslyn MCP for Bulk Renaming**:
   ```bash
   # Analyze current namespaces
   roslyn-mcp/analyze_namespaces workspacePath="src/backend"

   # Bulk rename namespaces
   roslyn-mcp/rename_namespaces mappingFile="namespace-mapping.json"

   # Validate changes
   roslyn-mcp/check_namespace_consistency workspacePath="src/backend"
   ```

3. **Update Project Files**:
   ```bash
   # Update RootNamespace in all .csproj files
   find src/backend -name "*.csproj" -exec sed -i 's/<RootNamespace>B2X\./<RootNamespace>B2X.Store./g' {} \;
   ```

4. **Update Assembly Names** (if needed):
   ```bash
   # Update AssemblyName in .csproj files where it differs from RootNamespace
   find src/backend -name "*.csproj" -exec sed -i 's/<AssemblyName>B2X\./<AssemblyName>B2X.Store./g' {} \;
   ```

#### Validation Steps:

1. **Build Validation**: Ensure all projects compile after namespace changes
2. **Reference Validation**: Check that all project references still work
3. **Test Execution**: Run full test suite to catch any missed references
4. **API Contract Validation**: Ensure public APIs maintain same contracts
5. **Database Migration Validation**: Check EF Core migrations still work

#### Risk Mitigation for Namespace Changes:

- **Backup First**: Create full backup before bulk renaming
- **Incremental Changes**: Rename one bounded context at a time
- **Build After Each**: Validate build after each context rename
- **Reference Checking**: Use automated tools to find broken references
- **Revert Plan**: Ability to rollback namespace changes if needed

## Additional Missing Items (Identified During Review)

### 1. Assembly Names and Package Names
**Issue**: .csproj files may have AssemblyName different from RootNamespace
**Impact**: NuGet packages, assembly loading, reflection
**Action Required**:
- Review all .csproj files for AssemblyName properties
- Update to match new namespace structure
- Update any hardcoded assembly name references

### 2. Database Migrations
**Issue**: EF Core migrations contain namespace references
**Impact**: Migration files become invalid after namespace changes
**Action Required**:
- Identify all migration files with namespace references
- Update migration namespaces during rename phase
- Test migrations still execute correctly

### 3. Configuration Files
**Issue**: appsettings.json, launchSettings.json may reference namespaces
**Impact**: Configuration binding, dependency injection
**Action Required**:
- Search for namespace references in config files
- Update to match new namespaces
- Test configuration loading

### 4. API Documentation and OpenAPI Specs
**Issue**: Swagger/OpenAPI documentation references old namespaces
**Impact**: API documentation becomes inaccurate
**Action Required**:
- Update OpenAPI specifications
- Regenerate API documentation
- Update any hardcoded namespace references in docs

### 5. Import Statements and Using Directives
**Issue**: Code files import old namespaces
**Impact**: Compilation failures
**Action Required**:
- Use Roslyn MCP to update import statements
- Validate all using directives are correct
- Check for any hardcoded namespace strings

### 6. Reflection and Dynamic Loading
**Issue**: Code that uses reflection with hardcoded namespace strings
**Impact**: Runtime failures
**Action Required**:
- Search for reflection usage with namespaces
- Update any typeof() expressions
- Test dynamic loading functionality

### 7. Test Project References
**Issue**: Test projects reference old namespaces in assertions
**Impact**: Test failures
**Action Required**:
- Update test code that references namespaces
- Update mock setups and verifications
- Validate test data and assertions

### 8. Build Scripts and CI/CD
**Issue**: Build scripts, Docker files reference old paths/namespaces
**Impact**: Build failures, deployment issues
**Action Required**:
- Update Dockerfile references
- Update docker-compose files
- Update CI/CD pipeline configurations
- Update build scripts

### 9. Documentation and README Files
**Issue**: Documentation references old structure and namespaces
**Impact**: Developer confusion
**Action Required**:
- Update all README files
- Update architecture documentation
- Update API documentation
- Update contribution guides

### 10. External References
**Issue**: Other projects or services reference old namespaces
**Impact**: Integration failures
**Action Required**:
- Identify external dependencies
- Coordinate with other teams
- Update integration contracts

## Comprehensive Validation Checklist

### Pre-Migration Validation:
- [ ] Full backup created
- [ ] All current builds pass
- [ ] All tests pass
- [ ] No broken references exist
- [ ] Documentation current

### Post-Migration Validation:
- [ ] All projects build successfully
- [ ] All tests pass
- [ ] No broken project references
- [ ] No broken namespace references
- [ ] No broken import statements
- [ ] Configuration files load correctly
- [ ] Database migrations work
- [ ] API documentation accurate
- [ ] CI/CD pipelines work
- [ ] Docker builds work
- [ ] External integrations work

### Tools for Validation:
1. **Roslyn MCP**: Namespace analysis and validation
2. **Build Scripts**: Automated build validation
3. **Test Runner**: Full test suite execution
4. **Reference Checker**: Automated reference validation
5. **Documentation Scanner**: Find outdated references

## Updated Risk Assessment

**High Risk Items (Added):**
- Namespace renaming breaking compilation
- Database migrations failing after namespace changes
- Configuration binding failures
- Reflection-based code breaking
- External integrations failing

**Medium Risk Items (Added):**
- API documentation becoming outdated
- Build scripts needing updates
- CI/CD pipeline failures
- Docker configuration issues

**Low Risk Items (Added):**
- Documentation updates
- README file updates
- Minor configuration tweaks

## Updated Implementation Timeline

### Phase 0: Pre-Migration Analysis (1 day)
- [ ] Complete namespace impact analysis
- [ ] Create comprehensive reference map
- [ ] Identify all files requiring updates
- [ ] Create backup and rollback plan

### Phase 3.5: Namespace Renaming (2 days)
- [ ] Update all .cs files with new namespaces
- [ ] Update all .csproj RootNamespace properties
- [ ] Update AssemblyName properties where needed
- [ ] Update import statements and using directives
- [ ] Update configuration files
- [ ] Update database migrations
- [ ] Update API documentation
- [ ] Validate all changes compile and tests pass

### Day 1: Preparation
- [ ] Create backup branch
- [ ] Run analysis scripts
- [ ] Create migration scripts
- [ ] Test scripts on small subset

### Day 2: Infrastructure Migration
- [ ] Move AppHost, ServiceDefaults, tools
- [ ] Update solution file
- [ ] Run build validation
- [ ] Commit changes

### Day 3: Shared Components
- [ ] Consolidate shared projects
- [ ] Update references
- [ ] Run build and reference validation
- [ ] Commit changes

### Day 4-5: Bounded Contexts
- [ ] Migrate Admin bounded context
- [ ] Test and commit
- [ ] Migrate Store bounded context
- [ ] Test and commit
- [ ] Migrate Management bounded context
- [ ] Test and commit
- [ ] Migrate CLI tools to bounded contexts
- [ ] Test and commit

### Day 6: Services & Infrastructure
- [ ] Move services and infrastructure
- [ ] Update references
- [ ] Run full validation
- [ ] Commit changes

### Day 7: Frontend & Tests
- [ ] Move frontend applications
- [ ] Reorganize tests
- [ ] Update configurations
- [ ] Run full test suite

### Day 8: Cleanup & Documentation
- [ ] Remove old directories
- [ ] Update documentation
- [ ] Final validation
- [ ] Merge to main branch

## Migration Scripts and Tools

### Namespace Renaming Script

Create `scripts/namespace-renamer.ps1`:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$MappingFile,
    
    [Parameter(Mandatory=$true)]
    [string]$TargetPath,
    
    [switch]$DryRun
)

# Load namespace mapping
$mapping = Get-Content $MappingFile | ConvertFrom-Json

Write-Host "Loaded $($mapping.PSObject.Properties.Count) namespace mappings"

# Find all .cs files
$csFiles = Get-ChildItem -Path $TargetPath -Filter "*.cs" -Recurse

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    
    $modified = $false
    
    foreach ($prop in $mapping.PSObject.Properties) {
        $oldNamespace = $prop.Name
        $newNamespace = $prop.Value
        
        # Update namespace declarations
        if ($content -match "namespace $oldNamespace") {
            $newContent = $content -replace "namespace $oldNamespace", "namespace $newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $modified = $true
                Write-Host "Updated namespace in $($file.FullName): $oldNamespace → $newNamespace"
            }
        }
        
        # Update using directives
        if ($content -match "using $oldNamespace") {
            $newContent = $content -replace "using $oldNamespace", "using $newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $modified = $true
                Write-Host "Updated using directive in $($file.FullName): $oldNamespace → $newNamespace"
            }
        }
    }
    
    if ($modified -and -not $DryRun) {
        Set-Content -Path $file.FullName -Value $content
    }
}

Write-Host "Namespace renaming complete"
```

### Usage:
```bash
# Dry run first
.\scripts\namespace-renamer.ps1 -MappingFile "namespace-mapping.json" -TargetPath "src/backend" -DryRun

# Actual rename
.\scripts\namespace-renamer.ps1 -MappingFile "namespace-mapping.json" -TargetPath "src/backend"
```

### Namespace Mapping File (`namespace-mapping.json`):

```json
{
  "B2X.Catalog": "B2X.Store.Catalog",
  "B2X.Orders": "B2X.Store.Orders", 
  "B2X.Customer": "B2X.Store.Customer",
  "B2X.Search": "B2X.Store.Search",
  "B2X.CMS": "B2X.Management.CMS",
  "B2X.Email": "B2X.Management.Email",
  "B2X.Legal": "B2X.Admin.Legal",
  "B2X.Compliance": "B2X.Admin.Compliance",
  "B2X.ERP": "B2X.Shared.ERP",
  "B2X.Identity": "B2X.Shared.Identity",
  "B2X.Localization": "B2X.Shared.Localization",
  "B2X.Tenancy": "B2X.Shared.Tenancy",
  "B2X.Security": "B2X.Shared.Security",
  "B2X.Theming": "B2X.Shared.Theming"
}
```

### Project File Update Script

Create `scripts/update-project-namespaces.ps1`:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$MappingFile,
    
    [Parameter(Mandatory=$true)]
    [string]$TargetPath
)

# Load namespace mapping
$mapping = Get-Content $MappingFile | ConvertFrom-Json

# Find all .csproj files
$csprojFiles = Get-ChildItem -Path $TargetPath -Filter "*.csproj" -Recurse

foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    
    $modified = $false
    
    foreach ($prop in $mapping.PSObject.Properties) {
        $oldNamespace = $prop.Name
        $newNamespace = $prop.Value
        
        # Update RootNamespace
        if ($content -match "<RootNamespace>$oldNamespace") {
            $newContent = $content -replace "<RootNamespace>$oldNamespace", "<RootNamespace>$newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $modified = $true
                Write-Host "Updated RootNamespace in $($file.FullName): $oldNamespace → $newNamespace"
            }
        }
        
        # Update AssemblyName if it matches the old namespace
        if ($content -match "<AssemblyName>$oldNamespace") {
            $newContent = $content -replace "<AssemblyName>$oldNamespace", "<AssemblyName>$newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $modified = $true
                Write-Host "Updated AssemblyName in $($file.FullName): $oldNamespace → $newNamespace"
            }
        }
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content
    }
}

Write-Host "Project file updates complete"
```

### Validation Script

Create `scripts/validate-migration.ps1`:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$TargetPath
)

Write-Host "Running migration validation..."

# Check for build errors
Write-Host "Checking build..."
dotnet build $TargetPath --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

# Check for namespace consistency
Write-Host "Checking namespace consistency..."
$csFiles = Get-ChildItem -Path $TargetPath -Filter "*.cs" -Recurse
$namespaceIssues = @()

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Check if file path matches namespace
    $relativePath = $file.FullName.Replace($TargetPath, "").TrimStart("\\")
    $expectedNamespace = "B2X." + ($relativePath -replace "\\", "." -replace "\.cs$", "")
    
    if ($content -match "namespace (B2X\.[^;]+)") {
        $actualNamespace = $matches[1]
        if ($actualNamespace -ne $expectedNamespace) {
            $namespaceIssues += @{
                File = $file.FullName
                Expected = $expectedNamespace
                Actual = $actualNamespace
            }
        }
    }
}

if ($namespaceIssues.Count -gt 0) {
    Write-Warning "Found $($namespaceIssues.Count) namespace inconsistencies:"
    $namespaceIssues | ForEach-Object {
        Write-Host "  $($_.File): Expected '$($_.Expected)', Found '$($_.Actual)'"
    }
} else {
    Write-Host "✅ All namespaces are consistent"
}

# Check for broken references
Write-Host "Checking for broken project references..."
# Implementation would check .csproj files for invalid reference paths

Write-Host "Validation complete"
```

## Risk Mitigation

1. **Frequent Commits**: Commit after each successful phase
2. **Build Validation**: Run builds after every major change
3. **Reference Checking**: Automated reference validation
4. **Rollback Plan**: Ability to revert changes if needed
5. **Testing**: Run test suite after each bounded context migration

## Success Metrics

- [ ] All projects build successfully
- [ ] All tests pass
- [ ] No broken project references
- [ ] CI/CD pipelines work
- [ ] Documentation updated
- [ ] Team can navigate new structure easily

## New Projects to Create

### B2X.Scheduler
**Location**: `src/backend/Services/Scheduler/`
**Purpose**: Job scheduling and orchestration service
**Responsibilities**:
- Scheduled task execution (cron-like jobs)
- Job queue management
- Periodic maintenance tasks
- Report generation scheduling
- Data cleanup scheduling

**Implementation Notes**:
- Use Quartz.NET for job scheduling
- Implement job persistence with PostgreSQL and SQL Server
- Include health checks for job execution monitoring
- Support cron expressions and recurring jobs

### B2X.BackgroundJobs
**Location**: `src/backend/Services/BackgroundJobs/`
**Purpose**: Message handling and maintenance jobs service  
**Responsibilities**:
- Wolverine message processing
- Event-driven background tasks
- Maintenance operations (database cleanup, cache invalidation)
- Long-running data processing jobs
- Integration with external systems

**Implementation Notes**:
- Integrate with Wolverine message bus
- Implement circuit breaker patterns for external integrations
- Add dead letter queue processing
- Include job retry mechanisms with exponential backoff

Both projects should follow the standard service structure:
- `API/` - Controllers and endpoints
- `Domain/` - Business logic and job definitions
- `Infrastructure/` - Data access and external integrations
- `Tests/` - Unit and integration tests

1. **Create New Projects**: 
   - `src/backend/Services/Scheduler/` - Job scheduling and orchestration service (B2X.Scheduler)
   - `src/backend/Services/BackgroundJobs/` - Message handling and maintenance jobs service (B2X.BackgroundJobs)
2. **Update Documentation**: README, contribution guides, architecture docs
3. **Update CI/CD**: Build scripts, Docker files, deployment configs
4. **Team Training**: Document new structure and navigation
5. **Tool Updates**: Update any scripts or tools that reference old paths
6. **Dependency Analysis**: Review and optimize project dependencies</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\PROJECT_RESTRUCTURE_MIGRATION_PLAN.md