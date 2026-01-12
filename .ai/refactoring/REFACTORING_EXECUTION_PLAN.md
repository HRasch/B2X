---
docid: BS-REFACTOR-EXEC
title: Refactoring Execution Plan
owner: @SARAH
status: Active
created: 2026-01-09
---

# B2X Refactoring Execution Plan
# Token-Optimized Workflow with SubAgent Task Splitting

## 📊 Complexity Analysis (From SubAgent Analysis)
- **Files to Move/Rename**: 92 directories and files
- **Files with References**: 1,680+ files requiring updates
- **Reference Types**: .cs (843), .md (493), .json (84), .csproj (77), .sh (50)
- **Breaking Change Risk**: HIGH - Requires careful phased execution

## 🎯 Execution Strategy

### Phase 0: Pre-Flight Checks (Token-Optimized)
**Duration**: 2 hours
**Risk Level**: LOW

```bash
# SubAgent: Pre-flight validation
runSubagent description:"Validate refactoring readiness"
prompt: "
Analyze current state for refactoring readiness:
- Git status and branch validation
- File permission checks for all targets
- Build baseline (store build outputs)
- Dependency mapping for critical paths
- MCP server availability check

Return ONLY: readiness_score + blocking_issues + baseline_metrics
"
```

### Phase 1: Directory Structure Creation (Token-Optimized)
**Duration**: 1 hour
**Risk Level**: LOW

```bash
# SubAgent: Safe directory creation
runSubagent description:"Create directory structure safely"
prompt: "
Create new directory structure with validation:
- Create src/, docs/, tests/, build/, config/, data/, archive/
- Preserve all existing permissions
- Validate directory creation
- Check for conflicts with existing files

Return ONLY: created_dirs + conflicts_found + permission_status
"
```

### Phase 2: Low-Risk File Moves (Token-Optimized)
**Duration**: 4 hours
**Risk Level**: MEDIUM

```bash
# SubAgent: Batch file operations (Phase 1)
runSubagent description:"Execute Phase 1 file moves"
prompt: "
Move low-risk files in optimized batches:
- Move data files (mock-db.json, test-data/) to data/
- Move docs (README.md, *.md except root configs) to docs/project/
- Move config files (*.json, *.yml except root) to config/
- Validate each move immediately
- Use temp files for large operation logs

Return ONLY: moved_files_count + validation_results + temp_file_paths
"
```

### Phase 3: Source Code Migration (HIGH RISK - Token-Optimized)
**Duration**: 8 hours
**Risk Level**: HIGH

#### 3.1: Backend Source Move
```bash
# SubAgent: Backend migration with validation
runSubagent description:"Migrate Backend/ to src/Backend/"
prompt: "
Execute Backend directory migration:
- Move Backend/ to src/Backend/ preserving casing
- Validate .csproj files still reference correctly
- Check namespace declarations
- Run Roslyn analysis on moved files
- Store analysis results in temp files

Return ONLY: migration_status + roslyn_errors + temp_analysis_file
"
```

#### 3.2: Frontend Source Move
```bash
# SubAgent: Frontend migration with validation
runSubagent description:"Migrate Frontend/ to src/Frontend/"
prompt: "
Execute Frontend directory migration:
- Move Frontend/ to src/Frontend/ preserving casing
- Validate package.json references
- Check TypeScript import paths
- Run TypeScript MCP analysis
- Store validation results in temp files

Return ONLY: migration_status + typescript_errors + temp_validation_file
"
```

#### 3.3: AppHost Migration
```bash
# SubAgent: AppHost migration with validation
runSubagent description:"Migrate AppHost/ to src/AppHost/"
prompt: "
Execute AppHost directory migration:
- Move AppHost/ to src/AppHost/ preserving casing
- Update Aspire configuration references
- Validate .csproj dependencies
- Check service discovery configurations

Return ONLY: migration_status + dependency_errors + config_validation
"
```

### Phase 4: Reference Updates (CRITICAL - Maximum Token Optimization)
**Duration**: 16 hours
**Risk Level**: HIGH

#### 4.1: C# Reference Updates (Split by Domain)
```bash
# SubAgent: Backend C# references
runSubagent description:"Update Backend C# references"
prompt: "
Update C# references in Backend domain:
- Scan all .cs files for 'Backend/' references
- Replace with 'src/Backend/' using pattern matching
- Validate namespace consistency
- Use Roslyn MCP for semantic validation
- Process in 50-file batches to manage tokens

Return ONLY: updated_files + validation_errors + batch_progress
"
```

```bash
# SubAgent: Frontend C# references
runSubagent description:"Update Frontend C# references"
prompt: "
Update C# references in Frontend domain:
- Scan all .cs files for 'Frontend/' references
- Replace with 'src/Frontend/' using pattern matching
- Validate using declarations
- Process in 50-file batches

Return ONLY: updated_files + validation_errors + batch_progress
"
```

#### 4.2: Project File Updates (.csproj)
```bash
# SubAgent: Project reference updates
runSubagent description:"Update .csproj project references"
prompt: "
Update project references in .csproj files:
- Find all <ProjectReference> elements
- Update paths from relative to new structure
- Validate project dependencies
- Check for circular references

Return ONLY: updated_projects + dependency_validation + circular_refs_detected
"
```

#### 4.3: TypeScript/JavaScript Updates
```bash
# SubAgent: Frontend import updates
runSubagent description:"Update TypeScript/JavaScript imports"
prompt: "
Update import statements in frontend code:
- Scan .ts, .js, .vue files for relative imports
- Update paths to new directory structure
- Validate import resolution
- Use TypeScript MCP for analysis

Return ONLY: updated_imports + resolution_errors + mcp_validation
"
```

#### 4.4: Documentation Updates
```bash
# SubAgent: Documentation reference updates
runSubagent description:"Update documentation references"
prompt: "
Update file references in documentation:
- Scan all .md files for hardcoded paths
- Update relative links and references
- Preserve anchor links and images
- Process in batches to manage token usage

Return ONLY: updated_docs + broken_links + batch_progress
"
```

#### 4.5: Configuration File Updates
```bash
# SubAgent: Configuration reference updates
runSubagent description:"Update configuration file references"
prompt: "
Update paths in configuration files:
- Docker Compose files
- GitHub Actions workflows
- Build scripts and package.json
- Environment-specific configs

Return ONLY: updated_configs + validation_status + breaking_changes
"
```

### Phase 5: Build Validation (Token-Optimized)
**Duration**: 8 hours
**Risk Level**: MEDIUM

#### 5.1: Backend Build Validation
```bash
# SubAgent: Backend build validation
runSubagent description:"Validate Backend builds post-refactoring"
prompt: "
Execute comprehensive Backend validation:
- Build all .NET projects
- Run unit tests
- Check API endpoints
- Validate database connections
- Store build logs in temp files

Return ONLY: build_status + test_results + temp_log_files
"
```

#### 5.2: Frontend Build Validation
```bash
# SubAgent: Frontend build validation
runSubagent description:"Validate Frontend builds post-refactoring"
prompt: "
Execute comprehensive Frontend validation:
- Install dependencies
- Build all frontend projects
- Run linting and type checking
- Validate asset loading
- Store build outputs in temp files

Return ONLY: build_status + lint_results + temp_output_files
"
```

#### 5.3: Integration Testing
```bash
# SubAgent: Integration test validation
runSubagent description:"Run integration tests"
prompt: "
Execute end-to-end validation:
- Start all services
- Run integration test suite
- Validate service communication
- Check external API integrations
- Monitor for runtime errors

Return ONLY: integration_status + service_health + error_logs
"
```

### Phase 6: Final Validation & Documentation (Token-Optimized)
**Duration**: 4 hours
**Risk Level**: LOW

```bash
# SubAgent: Final validation and documentation
runSubagent description:"Final validation and documentation update"
prompt: "
Complete post-refactoring tasks:
- Update all documentation with new structure
- Validate all links and references
- Create migration guide for team
- Generate final status report
- Archive temp files and logs

Return ONLY: documentation_status + validation_complete + archive_paths
"
```

## 🔄 Rollback Strategy (Token-Optimized)

```bash
# SubAgent: Emergency rollback
runSubagent description:"Execute emergency rollback"
prompt: "
Perform complete rollback to pre-refactoring state:
- Restore from refactor/backup branch
- Clean any partially moved files
- Reset git to clean state
- Validate rollback success
- Generate rollback report

Return ONLY: rollback_status + restored_files + validation_report
"
```

## 📈 Progress Tracking

### Daily Checkpoints
- **Morning**: SubAgent status check across all active operations
- **Midday**: Progress validation and blocker identification
- **Evening**: Daily summary and next-day planning

### Metrics Collection
- Files processed per hour
- Token usage per operation
- Error rates and types
- Build success rates

## 🚨 Emergency Protocols

### High Token Usage Detected
- Pause all subAgents
- Switch to manual processing for complex files
- Implement file fragment processing

### Build Failures
- Immediate rollback to last good commit
- Isolate failing component
- Manual fix with subAgent assistance

### Data Loss Concerns
- Verify backup integrity
- Use git reflog for recovery
- Implement file-level recovery

## ✅ Success Criteria

- [ ] All directories created successfully
- [ ] All source files moved with correct casing
- [ ] Zero broken references in code
- [ ] All builds pass successfully
- [ ] All tests execute without errors
- [ ] Services start and communicate properly
- [ ] Documentation updated and accurate
- [ ] Team can work in new structure

## 📊 Token Budget Management

- **Daily Limit**: 100K tokens
- **Per Operation**: Max 10K tokens
- **Optimization**: Use temp files for large outputs
- **Monitoring**: Track usage per subAgent

---

**Total Timeline**: 8-10 working days
**Parallel Processing**: Up to 3 subAgents simultaneously
**Risk Mitigation**: Incremental commits + automated rollback