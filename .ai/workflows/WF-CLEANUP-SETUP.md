---
docid: WF-CLEANUP-SETUP
title: Project Cleanup Setup Guide
owner: "@DevOps"
status: Active
---

# Project Cleanup Setup Guide

**DocID**: `WF-CLEANUP-SETUP`  
**Created**: 2026-01-07  
**Owner**: @DevOps, @TechLead

## Overview

This guide explains how to set up and use the automated cleanup and validation scripts for the B2X project.

## Components

### 1. Pre-Commit Hook
**File**: `scripts/pre-commit-cleanup-check.sh`

Validates commits before they're pushed:
- ✅ Prevents root-level file violations
- ✅ Detects duplicate files
- ✅ Warns on non-standard naming
- ✅ Blocks temporary files

### 2. Archive Script
**File**: `scripts/archive-old-docs.sh`

Automatically archives old documents:
- ✅ Finds docs > 90 days old
- ✅ Moves to `.ai/archive/`
- ✅ Creates audit log
- ✅ Supports dry-run mode

### 3. Duplicate Detection
**File**: `scripts/detect-duplicates.sh`

Scans for naming issues:
- ✅ Detects version suffixes
- ✅ Finds duplicate prefixes
- ✅ Validates DocID naming
- ✅ Generates reports

### 4. CI/CD Workflow
**File**: `.github/workflows/cleanup-check.yml`

GitHub Actions integration:
- ✅ Weekly cleanup scans
- ✅ PR duplicate detection
- ✅ Archive automation
- ✅ Cleanup reports

---

## Installation

### Option A: Pre-Commit Hook (Local Development)

The pre-commit hook prevents bad commits before they leave your machine.

**On macOS/Linux**:
```bash
# Copy script to git hooks
cp scripts/pre-commit-cleanup-check.sh .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit

# Verify installation
git hook show pre-commit
```

**On Windows (Git Bash)**:
```bash
# Copy script to git hooks
cp scripts/pre-commit-cleanup-check.sh .git/hooks/pre-commit

# Verify installation
cat .git/hooks/pre-commit
```

**Note**: The hook automatically runs before `git commit`. To bypass (not recommended):
```bash
git commit --no-verify
```

### Option B: CI/CD Automation

The GitHub Actions workflow runs automatically:
- **Weekly**: Every Monday at 09:00 UTC
- **Manual**: Trigger via workflow dispatch
- **PR**: Runs on pull requests (optional configuration)

No installation needed—GitHub Actions picks it up automatically.

---

## Usage

### Pre-Commit Hook

**Automatic** on commit:
```bash
git add .
git commit -m "feat: add new feature"
# Hook runs automatically
# If validation fails → commit is blocked
```

**Example Output - PASS**:
```
🔍 Running pre-commit cleanup checks...
✅ Pre-commit cleanup checks passed!
```

**Example Output - FAIL**:
```
🔍 Running pre-commit cleanup checks...

⚠️  Root-level .md files detected that are not in whitelist:
  ❌ my-analysis.md
  
💡 Other files should be placed in:
  - Analysis docs: .ai/requirements/ (REQ-###-*.md)
  - Architecture: .ai/decisions/ (ADR-###-*.md)
  ...
```

**How to Fix**:
1. Move file to appropriate `.ai/` subdirectory
2. Rename with DocID pattern (e.g., `REQ-007-feature.md`)
3. Run `git add` again
4. Commit again

### Archive Script

**Run manually**:
```bash
# Dry-run (see what would be archived, no changes)
bash scripts/archive-old-docs.sh --dry-run

# Actually archive
bash scripts/archive-old-docs.sh
```

**Output**:
```
📦 Archive Old Documents Script
================================
Threshold: > 90 days
Dry Run: --dry-run
Log: .ai/cleanup-logs/archive-2026-01-07-153022.log

🔍 Scanning .ai/logs...
  📦 [127 days] .ai/logs/old-build-log.md
     → [DRY-RUN] Would archive to .ai/archive/old-build-log-archived.md

✅ Archived 1 documents:
  - .ai/logs/old-build-log.md → .ai/archive/old-build-log-archived.md
```

**After Running**:
1. Check `.ai/cleanup-logs/` for audit trail
2. Review archived files: `ls .ai/archive/`
3. Commit changes: `git add .ai/archive/ && git commit -m "chore: archive old documents"`

### Duplicate Detection

**Run manually**:
```bash
bash scripts/detect-duplicates.sh
```

**Output**:
```
🔎 Scanning for duplicates and naming issues...

### Check 1: Version Suffixes ###
✅ No versioned files found

### Check 2: Potential Duplicates (Same Prefix) ###
❌ Files sharing base names (potential consolidation candidates):
  Prefix: REQ-007
    - REQ-007-backend-analysis.md
    - REQ-007-backend-analysis 2.md
    
✅ All .ai/ documents follow DocID naming

📊 Full report: cat .ai/cleanup-logs/duplicates-2026-01-07-153022.report
```

**Report Location**: `.ai/cleanup-logs/duplicates-YYYY-MM-DD-HHMMSS.report`

### CI/CD Workflow

**Automatic Execution**:
- Every Monday 09:00 UTC (scheduled)
- On pull requests (check for violations)
- Manual trigger via GitHub Actions UI

**View Results**:
1. Go to GitHub → Actions
2. Find "Cleanup & Duplicate Detection" workflow
3. Click run → View logs
4. Download artifacts: cleanup-logs

**PR Comments**:
If cleanup issues are detected on a PR, a comment is automatically posted:
```
## 🧹 Project Cleanup Report

Archived documents: 12
Active .ai/ documents: 145
Root-level .md files: 12

⚠️ Issues: Review root-level files
```

---

## Governance Policies

### Per-Commit Policy
The pre-commit hook enforces:
- ✅ Allowed root files only
- ❌ No duplicate files
- ❌ No temporary files
- ⚠️ Warning on naming conventions

**Bypass only with justification** (use `--no-verify` with caution).

### Weekly Cleanup
The archive script:
- Auto-archives docs > 90 days old
- Creates PR for review
- Generates audit logs
- Maintains 6-month history

### Quarterly Audit
Manual review:
- Check `.ai/archive/` size
- Delete documents > 6 months old
- Update [DOCUMENT_REGISTRY.md](.ai/DOCUMENT_REGISTRY.md)
- Report to @SARAH

---

## Troubleshooting

### "Pre-commit hook not running"

**Cause**: Hook not installed or file permissions

**Fix**:
```bash
# Check if hook exists
ls -la .git/hooks/pre-commit

# Reinstall
cp scripts/pre-commit-cleanup-check.sh .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

### "Hook keeps failing but file is whitelisted"

**Cause**: File may be matching a duplicate pattern

**Fix**:
```bash
# Check what's being detected
git diff --cached --name-only

# Look for version suffixes or temp patterns
# Rename file accordingly
```

### "Archive script not finding files"

**Cause**: Files are too recent or in archive already

**Fix**:
```bash
# Check file age
ls -la .ai/logs/
# If < 90 days old, won't be archived

# Run dry-run to see what would happen
bash scripts/archive-old-docs.sh --dry-run
```

### "CI workflow not running"

**Cause**: Workflow may be disabled or branch not configured

**Fix**:
1. Go to GitHub → Actions
2. Check "Cleanup & Duplicate Detection" is enabled
3. Verify it's on `main` or `master` branch
4. Manually trigger: Actions → Cleanup → Run workflow

---

## Integration with Development Workflow

### In Code Reviews
Reviewers check:
```
✅ Files in correct .ai/ directories
✅ Duplicate files consolidated
✅ No temporary files committed
✅ DocID naming followed
```

### In CI/CD Pipeline
Added as optional gate:
```yaml
- Run duplicate detection
- Check root-level files
- Generate cleanup report
- (Does NOT block merge, only reports)
```

### In PR Templates
Add checklist:
```markdown
- [ ] Files placed in correct .ai/ subdirectories
- [ ] No duplicate files committed
- [ ] DocID naming followed (if applicable)
- [ ] No temporary files included
```

---

## Maintenance

### Script Updates
When cleanup rules change:
1. Update scripts in `scripts/`
2. Update this guide
3. Notify team in CONTRIBUTING.md
4. Bump version in script headers

### Archive Policy Changes
When retention or thresholds change:
1. Update `archive-old-docs.sh` (DAYS_THRESHOLD)
2. Update [BS-PROJECT-CLEANLINESS-STRATEGY.md](.ai/brainstorm/BS-PROJECT-CLEANLINESS-STRATEGY.md)
3. Announce in team docs

### Monitoring
Check cleanup health:
```bash
# Archive size
du -sh .ai/archive/

# Active documents
find .ai -name "*.md" ! -path ".ai/archive/*" ! -path ".ai/logs/*" | wc -l

# Cleanup logs
ls -la .ai/cleanup-logs/
```

---

## Next Steps

1. **Install pre-commit hook** (macOS/Linux users recommended)
2. **Test archive script** with `--dry-run` first
3. **Review** first duplicate detection report
4. **Enable** GitHub Actions workflow
5. **Add cleanup checklist** to PR template

---

**Questions?** Contact @DevOps or @TechLead

**Related Documents**:
- [BS-PROJECT-CLEANLINESS-STRATEGY.md](.ai/brainstorm/BS-PROJECT-CLEANLINESS-STRATEGY.md)
- [GL-010 Agent & Artifact Organization](.ai/guidelines/GL-010-AGENT-ARTIFACT-ORGANIZATION.md)
- [CONTRIBUTING.md](CONTRIBUTING.md)

