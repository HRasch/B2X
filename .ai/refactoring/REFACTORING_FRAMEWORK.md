---
docid: BS-REFACTOR-FRAMEWORK
title: Refactoring Framework
owner: @SARAH
status: Active
created: 2026-01-09
---

# B2X Refactoring: Complete Execution Framework

## 🎯 Overview

This framework provides a comprehensive, token-optimized approach to refactoring the B2X project structure using subAgent task splitting. The refactoring moves from the current flat structure to a modern `src/docs/tests` layout while preserving all functionality.

## 📋 Available Resources

### 1. Execution Plan
**File**: `REFACTORING_EXECUTION_PLAN.md`
- Detailed 6-phase execution strategy
- Risk assessment and mitigation plans
- Token optimization strategies
- Timeline and success criteria

### 2. SubAgent Prompts
**File**: `REFACTORING_SUBAGENT_PROMPTS.md`
- Ready-to-use subAgent prompts for each phase
- Token-optimized task splitting
- Parallel execution guidelines
- Error handling protocols

### 3. Helper Scripts
**Master Script**: `scripts/refactor-project.sh`
```bash
./scripts/refactor-project.sh all        # Complete refactoring
./scripts/refactor-project.sh structure  # Just create directories
./scripts/refactor-project.sh phase1     # Move data/docs/config
./scripts/refactor-project.sh phase2     # Move source code
```

**Quick Start**: `scripts/refactor-start.sh`
```bash
./scripts/refactor-start.sh              # Interactive menu
./scripts/refactor-start.sh preflight    # Pre-flight checks
./scripts/refactor-start.sh structure    # Directory creation
```

### 4. Individual Phase Scripts
- `scripts/create-refactor-structure.sh` - Directory creation
- `scripts/move-files-phase1.sh` - Low-risk moves
- `scripts/move-files-phase2.sh` - Source code moves
- `scripts/validate-moves.sh` - Validation
- `scripts/update-references.sh` - Reference updates
- `scripts/test-builds.sh` - Build validation
- `scripts/rollback-refactor.sh` - Emergency rollback

### 5. Documentation
**Guide**: `REFACTORING_GUIDE.md`
- Complete user guide with safety procedures
- Troubleshooting and recovery steps
- Team communication templates

## 🚀 Quick Start Workflow

### Step 1: Preparation (5 minutes)
```bash
# Ensure you're on the working branch
git checkout refactor/working

# Run quick start script
./scripts/refactor-start.sh
```

### Step 2: Choose Starting Point
The interactive menu offers:
1. **Full pre-flight + structure** - Recommended for first run
2. **Structure only** - If pre-flight already done
3. **Low-risk moves only** - For incremental approach

### Step 3: Execute Phases
For complex phases (3+), use individual subAgent prompts from `REFACTORING_SUBAGENT_PROMPTS.md`

## 🔧 SubAgent Task Splitting Strategy

### Token Optimization
- **Batch Processing**: Files processed in 20-50 file batches
- **Temp Files**: Large outputs saved to temp files
- **Focused Contexts**: Each subAgent handles single domains
- **Parallel Execution**: Up to 3 subAgents simultaneously

### Risk Mitigation
- **Incremental Commits**: Commit after each phase
- **Validation Gates**: Automated checks at each step
- **Rollback Ready**: Complete undo capability
- **Progress Tracking**: Daily status monitoring

## 📊 Phase Overview

| Phase | Description | Risk | Duration | SubAgents |
|-------|-------------|------|----------|-----------|
| 0 | Pre-flight checks | Low | 2h | 1 |
| 1 | Directory creation | Low | 1h | 1 |
| 2 | Low-risk moves | Medium | 4h | 1 |
| 3 | Source migration | High | 8h | 3 parallel |
| 4 | Reference updates | High | 16h | 6 parallel |
| 5 | Build validation | Medium | 8h | 3 parallel |
| 6 | Final docs | Low | 4h | 1 |

**Total Timeline**: 8-10 working days

## 🎛️ Key SubAgent Operations

### High-Impact SubAgents
```bash
# Critical path operations
runSubagent description:"Backend C# references"     # 843 files
runSubagent description:"Documentation updates"     # 493 files
runSubagent description:"Project references"        # 77 files
runSubagent description:"TypeScript imports"        # 36 files
```

### Parallel Execution Groups
- **Backend**: C# refs, project files, build validation
- **Frontend**: TypeScript, Vue, build validation
- **Infrastructure**: Configs, docs, integration tests

## 🚨 Emergency Protocols

### If Issues Occur
1. **Immediate**: Run rollback subAgent
2. **Recovery**: Reset to `refactor/backup` branch
3. **Analysis**: Review temp files for error details
4. **Resume**: Start from last successful phase

### Monitoring
- **Daily**: Run progress monitoring subAgent
- **Alerts**: High token usage, build failures, blocking errors
- **Reports**: Automatic status updates to GitHub issues

## ✅ Success Validation

### Automated Checks
- Build success across all projects
- Test execution without failures
- Service health endpoints responding
- Import resolution working
- Documentation links valid

### Manual Verification
- Team can build and run locally
- CI/CD pipelines working
- External integrations functional
- Performance unchanged

## 📞 Support

### For Execution Issues
- Check `REFACTORING_SUBAGENT_PROMPTS.md` for specific prompts
- Review temp files in `.ai/temp/` for detailed logs
- Use rollback script for recovery

### For Planning Questions
- Reference `REFACTORING_EXECUTION_PLAN.md`
- Contact @SARAH for coordination
- Check GitHub issues for blockers

---

## 🎯 Next Steps

1. **Start Here**: Run `./scripts/refactor-start.sh` for guided execution
2. **Review Plan**: Read `REFACTORING_EXECUTION_PLAN.md` for detailed strategy
3. **Execute**: Follow phase-by-phase approach with subAgent prompts
4. **Monitor**: Use daily progress monitoring subAgent
5. **Validate**: Complete build and integration testing

**The framework is designed for safe, incremental execution with maximum automation and minimal risk.**