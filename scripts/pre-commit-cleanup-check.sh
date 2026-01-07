#!/bin/bash
# Pre-commit Cleanup Check
# Validates root-level files and prevents duplicates from being committed
# Install: cp scripts/pre-commit-cleanup-check.sh .git/hooks/pre-commit && chmod +x .git/hooks/pre-commit

set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
ALLOWED_ROOT_FILES=(
  "README.md"
  "QUICK_START_GUIDE.md"
  "CONTRIBUTING.md"
  "GOVERNANCE.md"
  "SECURITY.md"
  "LICENSE"
  "B2X.slnx"
  "Directory.Packages.props"
  "docker-compose.yml"
  "package.json"
  "DEPLOYMENT_PLAN_ENVENTA_ERP_INTEGRATION.md"
  "LEGACY_CODE_MIGRATION_GUIDE.md"
  "LEGACY_CODE_CLEANUP_STATUS.md"
  "MODEL_LICENSES.md"
  "PROJECT_DASHBOARD.md"
  "PROJECT_STRUCTURE.md"
  "QUICK_LOGIN.md"
)

echo "🔍 Running pre-commit cleanup checks..."

# Check 1: Root-level .md files
ROOT_MD_FILES=$(git diff --cached --name-only | grep -E "^[^/]*\.md$" | sort || true)
if [[ ! -z "$ROOT_MD_FILES" ]]; then
  INVALID_FILES=""
  while IFS= read -r file; do
    BASENAME=$(basename "$file")
    if [[ ! " ${ALLOWED_ROOT_FILES[@]} " =~ " ${BASENAME} " ]]; then
      INVALID_FILES="$INVALID_FILES\n  ❌ $BASENAME"
    fi
  done <<< "$ROOT_MD_FILES"
  
  if [[ ! -z "$INVALID_FILES" ]]; then
    echo -e "\n⚠️  Root-level .md files detected that are not in whitelist:"
    echo -e "$INVALID_FILES"
    echo ""
    echo "📋 Allowed root-level files:"
    printf '  ✅ %s\n' "${ALLOWED_ROOT_FILES[@]}"
    echo ""
    echo "💡 Other files should be placed in:"
    echo "  - Analysis docs: .ai/requirements/ (REQ-###-*.md)"
    echo "  - Architecture: .ai/decisions/ (ADR-###-*.md)"
    echo "  - Strategy: .ai/brainstorm/ (BS-*.md)"
    echo "  - Implementation logs: .ai/logs/ (YYYY-MM-DD-*.md)"
    echo ""
    echo "📖 See .ai/brainstorm/BS-PROJECT-CLEANLINESS-STRATEGY.md for details"
    exit 1
  fi
fi

# Check 2: Duplicate files (version suffixes)
DUPLICATES=$(git diff --cached --name-only | grep -E " [0-9]\.(md|json|js|cs)$" || true)
if [[ ! -z "$DUPLICATES" ]]; then
  echo ""
  echo "⚠️  Potential duplicate files detected (files with version suffixes):"
  echo "$DUPLICATES" | sed 's/^/  ❌ /'
  echo ""
  echo "💡 Instead of duplicating files:"
  echo "  1. Consolidate into single file with unique name"
  echo "  2. Use DocID system (REQ-###, ADR-###, etc.)"
  echo "  3. Archive old versions to .ai/archive/"
  exit 1
fi

# Check 3: Temporary files
TEMP_FILES=$(git diff --cached --name-only | grep -E "(temp-|test-|sample-|audit-|analysis-)" | grep -E "\.(json|js|cs|md|txt)$" || true)
if [[ ! -z "$TEMP_FILES" ]]; then
  echo ""
  echo "⚠️  Temporary files detected:"
  echo "$TEMP_FILES" | sed 's/^/  ❌ /'
  echo ""
  echo "💡 Temporary files should not be committed:"
  echo "  1. Remove temporary files"
  echo "  2. Use .gitignore for patterns"
  echo "  3. Archive to .ai/archive/ if needed for reference"
  exit 1
fi

# Check 4: DocID naming for .ai/ files
AI_FILES=$(git diff --cached --name-only | grep "^\.ai/" | grep "\.md$" || true)
if [[ ! -z "$AI_FILES" ]]; then
  NAMING_ISSUES=""
  while IFS= read -r file; do
    BASENAME=$(basename "$file")
    # Check if it matches DocID pattern: PREFIX-###-* or KB-*
    if ! [[ "$BASENAME" =~ ^(REQ|ADR|KB|GL|WF|BS|SPR|CMP|FH|AGT|PRM|INS|TPL|CFG|DOC|LOG|STATUS|ANALYSIS|CONSOLIDATION|COMM|REVIEW|PLAN|QS|REFACTOR|INDEX)-[0-9A-Za-z_-]+\.md$ ]]; then
      NAMING_ISSUES="$NAMING_ISSUES\n  ⚠️  $BASENAME (should follow PREFIX-###-name.md pattern)"
    fi
  done <<< "$AI_FILES"
  
  if [[ ! -z "$NAMING_ISSUES" ]]; then
    echo ""
    echo "⚠️  .ai/ files with non-standard naming detected:"
    echo -e "$NAMING_ISSUES"
    echo ""
    echo "💡 Use DocID naming convention:"
    echo "  - REQ-### for requirements"
    echo "  - ADR-### for architecture decisions"
    echo "  - KB-### for knowledge base"
    echo "  - GL-### for guidelines"
    echo "  - etc. (see .ai/DOCUMENT_REGISTRY.md)"
    echo ""
    echo "⚠️  This is a WARNING only - you can still commit"
    echo "   but please rename files to follow conventions"
  fi
fi

echo ""
echo "✅ Pre-commit cleanup checks passed!"
exit 0
