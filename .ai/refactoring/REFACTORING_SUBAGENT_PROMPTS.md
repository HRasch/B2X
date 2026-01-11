---
docid: BS-REFACTOR-SUBAGENT-PROMPTS
title: Refactoring SubAgent Prompts
owner: @SARAH
status: Active
created: 2026-01-09
---

# SubAgent Prompts for B2X Refactoring
# Token-Optimized Task Splitting

## Phase 0: Pre-Flight Checks
```bash
runSubagent description:"Pre-flight validation and baseline"
prompt: "
Execute comprehensive pre-flight checks:
1. Git status validation (clean working directory)
2. Branch verification (on refactor/working)
3. File permission checks for all target directories
4. Build baseline capture (store dotnet build output)
5. Dependency mapping for critical paths
6. MCP server connectivity validation
7. Disk space verification for operations

Return ONLY: readiness_score + blocking_issues + baseline_build_log + mcp_status
"
```

## Phase 1: Directory Structure Creation
```bash
runSubagent description:"Create new directory structure"
prompt: "
Create new directory structure safely:
1. Create: src/, docs/, tests/, build/, config/, data/, archive/
2. Set appropriate permissions (755 for dirs, 644 for files)
3. Validate directory creation
4. Check for naming conflicts
5. Create .gitkeep files in empty directories

Return ONLY: created_directories + permission_status + conflicts_detected
"
```

## Phase 2: Low-Risk File Moves (Data/Docs/Config)
```bash
runSubagent description:"Phase 1 file migration (low-risk)"
prompt: "
Execute Phase 1 file moves in batches:
1. Move data files: mock-db*.json, test-data/ → data/
2. Move docs: *.md files (except root configs) → docs/project/
3. Move configs: *.json, *.yml (except root) → config/
4. Validate each move immediately
5. Update .gitignore if needed
6. Store operation logs in temp files

Return ONLY: moved_files_count + validation_results + temp_log_path + gitignore_updated
"
```

## Phase 3: Source Code Migration (HIGH RISK)

### 3.1 Backend Migration
```bash
runSubagent description:"Backend source code migration"
prompt: "
Migrate Backend/ to src/Backend/:
1. Move Backend/ directory preserving exact casing
2. Validate .csproj files still parse correctly
3. Check namespace declarations match new path
4. Run Roslyn analysis on key files
5. Store analysis results in temp file

Return ONLY: migration_success + roslyn_analysis_results + temp_analysis_file + namespace_validation
"
```

### 3.2 Frontend Migration
```bash
runSubagent description:"Frontend source code migration"
prompt: "
Migrate Frontend/ to src/Frontend/:
1. Move Frontend/ directory preserving exact casing
2. Validate package.json scripts and dependencies
3. Check TypeScript configuration paths
4. Run TypeScript MCP analysis on key files
5. Store validation results in temp file

Return ONLY: migration_success + typescript_analysis_results + temp_validation_file + config_validation
"
```

### 3.3 AppHost Migration
```bash
runSubagent description:"AppHost migration"
prompt: "
Migrate AppHost/ to src/AppHost/:
1. Move AppHost/ directory preserving exact casing
2. Update Aspire configuration references
3. Validate .csproj project references
4. Check service discovery configurations
5. Update docker-compose paths if needed

Return ONLY: migration_success + dependency_validation + config_updates + docker_validation
"
```

## Phase 4: Reference Updates (MAXIMUM TOKEN OPTIMIZATION)

### 4.1 C# Backend References
```bash
runSubagent description:"Update Backend C# references"
prompt: "
Update C# references for Backend domain:
1. Scan all .cs files for 'Backend/' path references
2. Replace with 'src/Backend/' using exact pattern matching
3. Validate namespace consistency
4. Process in 25-file batches to manage tokens
5. Use Roslyn MCP for semantic validation

Return ONLY: files_processed + replacements_made + validation_errors + batch_complete
"
```

### 4.2 C# Frontend References
```bash
runSubagent description:"Update Frontend C# references"
prompt: "
Update C# references for Frontend domain:
1. Scan all .cs files for 'Frontend/' path references
2. Replace with 'src/Frontend/' using exact pattern matching
3. Validate using declarations and imports
4. Process in 25-file batches to manage tokens

Return ONLY: files_processed + replacements_made + validation_errors + batch_complete
"
```

### 4.3 Project File References (.csproj)
```bash
runSubagent description:"Update .csproj project references"
prompt: "
Update project references in .csproj files:
1. Find all <ProjectReference Include=...> elements
2. Update relative paths to new directory structure
3. Validate project dependency resolution
4. Check for circular reference issues
5. Process all .csproj files

Return ONLY: projects_updated + dependencies_validated + circular_refs_found + resolution_errors
"
```

### 4.4 TypeScript/JavaScript Imports
```bash
runSubagent description:"Update frontend import statements"
prompt: "
Update import statements in frontend code:
1. Scan .ts, .js, .vue files for relative imports
2. Update import paths to new directory structure
3. Handle both relative and absolute imports
4. Use TypeScript MCP for import resolution validation
5. Process in 20-file batches

Return ONLY: files_processed + imports_updated + resolution_errors + mcp_validation_status
"
```

### 4.5 Documentation References
```bash
runSubagent description:"Update documentation file references"
prompt: "
Update file references in documentation:
1. Scan all .md files for hardcoded relative paths
2. Update links and file references to new structure
3. Preserve anchor links and image references
4. Process in 30-file batches to manage tokens
5. Validate link integrity

Return ONLY: docs_processed + links_updated + broken_links_found + batch_complete
"
```

### 4.6 Configuration File References
```bash
runSubagent description:"Update configuration file paths"
prompt: "
Update paths in configuration files:
1. Docker Compose files (relative paths)
2. GitHub Actions workflows (working-directory)
3. Build scripts and package.json scripts
4. Environment-specific configuration files
5. Validate all path updates

Return ONLY: configs_updated + validation_status + breaking_changes_detected + path_resolution
"
```

## Phase 5: Build Validation

### 5.1 Backend Build Validation
```bash
runSubagent description:"Backend build and test validation"
prompt: "
Execute Backend validation suite:
1. Build all .NET projects (dotnet build)
2. Run unit tests (dotnet test)
3. Validate API project references
4. Check database connection strings
5. Store build/test outputs in temp files

Return ONLY: build_success + test_results + api_validation + temp_log_files
"
```

### 5.2 Frontend Build Validation
```bash
runSubagent description:"Frontend build and lint validation"
prompt: "
Execute Frontend validation suite:
1. Install dependencies (npm install)
2. Run linting (npm run lint)
3. Build all frontend projects (npm run build)
4. Validate asset paths and imports
5. Store build outputs in temp files

Return ONLY: build_success + lint_results + asset_validation + temp_output_files
"
```

### 5.3 Integration Testing
```bash
runSubagent description:"Integration and service validation"
prompt: "
Execute integration validation:
1. Start AppHost services (dotnet run)
2. Validate service health endpoints
3. Test inter-service communication
4. Run integration test suite
5. Monitor for runtime errors

Return ONLY: services_started + health_checks_passed + integration_tests + error_logs
"
```

## Phase 6: Final Validation & Documentation
```bash
runSubagent description:"Final validation and documentation"
prompt: "
Complete post-refactoring validation:
1. Update README.md with new structure
2. Validate all documentation links
3. Create team migration guide
4. Generate final status report
5. Clean up temp files and logs
6. Archive operation artifacts

Return ONLY: documentation_updated + links_validated + migration_guide_created + final_report + cleanup_status
"
```

## Emergency Rollback
```bash
runSubagent description:"Emergency rollback execution"
prompt: "
Execute complete rollback to pre-refactoring state:
1. Verify backup branch integrity (refactor/backup)
2. Reset working directory to backup state
3. Clean any orphaned files from partial moves
4. Validate rollback success
5. Generate rollback report with lessons learned

Return ONLY: rollback_success + files_restored + validation_status + rollback_report
"
```

## Progress Monitoring SubAgent
```bash
runSubagent description:"Daily progress monitoring"
prompt: "
Monitor refactoring progress across all phases:
1. Check completion status of each phase
2. Identify current blockers and bottlenecks
3. Calculate progress percentage
4. Estimate time to completion
5. Flag high-risk items requiring attention

Return ONLY: overall_progress + current_blockers + time_estimate + high_risk_items
"
```

---

## Token Optimization Guidelines

### For Large File Operations:
- Process files in batches (20-50 files max per subAgent)
- Use temp files for operation outputs >1KB
- Split complex operations across multiple subAgents
- Use file fragments for large single files

### Context Management:
- Keep prompts focused on single operations
- Use exact file paths to avoid ambiguity
- Include validation steps in each prompt
- Return structured results only

### Parallel Execution:
- Maximum 2-3 subAgents running simultaneously
- Coordinate through status files
- Use different domains (backend, frontend, docs) for parallel work

### Error Handling:
- Each subAgent validates its own work
- Return specific error types and locations
- Include recovery suggestions in results