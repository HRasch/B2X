#!/bin/bash
# Duplicate Detection Script
# Finds files with version suffixes and naming inconsistencies
# Run: bash scripts/detect-duplicates.sh [--fix]

set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
REPORT_DIR="$REPO_ROOT/.ai/cleanup-logs"
REPORT_FILE="$REPORT_DIR/duplicates-$(date +%Y-%m-%d-%H%M%S).report"
FIX_MODE="${1:---}"

mkdir -p "$REPORT_DIR"

echo "🔎 Duplicate Detection Script" > "$REPORT_FILE"
echo "=============================" >> "$REPORT_FILE"
echo "Generated: $(date)" >> "$REPORT_FILE"
echo "Fix Mode: $FIX_MODE" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

echo "🔎 Scanning for duplicates and naming issues..."

DUPLICATE_COUNT=0
ISSUES_COUNT=0

# Check 1: Files with version suffixes (e.g., "file 2.md", "file_v2.md")
echo "### Check 1: Version Suffixes ###" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

VERSIONED_FILES=$(find "$REPO_ROOT" \
  -type f \
  \( -name "* [0-9].md" -o -name "* [0-9].json" -o -name "*_v[0-9].md" -o -name "*_v[0-9].json" \) \
  ! -path "./.git/*" \
  ! -path "./node_modules/*" \
  ! -path "./bin/*" \
  ! -path "./obj/*" \
  ! -path "./.ai/archive/*" \
  2>/dev/null || true)

if [[ ! -z "$VERSIONED_FILES" ]]; then
  echo "❌ Files with version suffixes found:" >> "$REPORT_FILE"
  while IFS= read -r file; do
    RELATIVE_PATH="${file#$REPO_ROOT/}"
    DUPLICATE_COUNT=$((DUPLICATE_COUNT + 1))
    echo "  - $RELATIVE_PATH" >> "$REPORT_FILE"
    echo "    ⚠️  $RELATIVE_PATH"
  done <<< "$VERSIONED_FILES"
else
  echo "✅ No versioned files found" >> "$REPORT_FILE"
fi
echo "" >> "$REPORT_FILE"

# Check 2: Potential duplicates by name pattern
echo "### Check 2: Potential Duplicates (Same Prefix) ###" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

PREFIXES=$(find "$REPO_ROOT" \
  -type f \
  -name "*.md" \
  ! -path "./.git/*" \
  ! -path "./node_modules/*" \
  ! -path "./bin/*" \
  ! -path "./obj/*" \
  -print0 2>/dev/null | xargs -0 basename -a | sed 's/-[0-9].md$//' | sort | uniq -c | awk '$1 > 1 {print $2}' || true)

if [[ ! -z "$PREFIXES" ]]; then
  echo "❌ Files sharing base names (potential consolidation candidates):" >> "$REPORT_FILE"
  while IFS= read -r prefix; do
    echo "" >> "$REPORT_FILE"
    echo "  Prefix: $prefix" >> "$REPORT_FILE"
    find "$REPO_ROOT" -type f -name "${prefix}*" ! -path "./.git/*" ! -path "./.ai/archive/*" -print0 2>/dev/null | xargs -0 -I {} bash -c 'echo "    - $(basename {})"' >> "$REPORT_FILE"
    
    ISSUES_COUNT=$((ISSUES_COUNT + 1))
    echo "  ⚠️  Multiple files with prefix: $prefix"
  done <<< "$PREFIXES"
else
  echo "✅ No duplicate prefixes found" >> "$REPORT_FILE"
fi
echo "" >> "$REPORT_FILE"

# Check 3: DocID naming compliance
echo "### Check 3: DocID Naming Compliance ###" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

BAD_NAMES=$(find "$REPO_ROOT/.ai" \
  -type f \
  -name "*.md" \
  ! -path "./.ai/archive/*" \
  ! -regex ".*/\(REQ\|ADR\|KB\|GL\|WF\|BS\|SPR\|CMP\|FH\|AGT\|PRM\|INS\|TPL\|CFG\|DOC\|LOG\|STATUS\|ANALYSIS\|CONSOLIDATION\|COMM\|REVIEW\|PLAN\|QS\|REFACTOR\|INDEX\|DOCUMENT_REGISTRY\|README\)-[0-9A-Za-z_-]*\.md" \
  2>/dev/null || true)

if [[ ! -z "$BAD_NAMES" ]]; then
  echo "❌ Files not following DocID naming convention:" >> "$REPORT_FILE"
  while IFS= read -r file; do
    RELATIVE_PATH="${file#$REPO_ROOT/}"
    ISSUES_COUNT=$((ISSUES_COUNT + 1))
    echo "  - $RELATIVE_PATH" >> "$REPORT_FILE"
    echo "    ⚠️  $RELATIVE_PATH (should be PREFIX-###-name.md)"
  done <<< "$BAD_NAMES"
else
  echo "✅ All .ai/ documents follow DocID naming" >> "$REPORT_FILE"
fi
echo "" >> "$REPORT_FILE"

# Summary
echo "### Summary ###" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo "Total Duplicates Found: $DUPLICATE_COUNT" >> "$REPORT_FILE"
echo "Total Issues Found: $ISSUES_COUNT" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

if [[ $DUPLICATE_COUNT -gt 0 ]] || [[ $ISSUES_COUNT -gt 0 ]]; then
  echo "❌ Issues detected: $((DUPLICATE_COUNT + ISSUES_COUNT))"
  echo "" >> "$REPORT_FILE"
  echo "📋 Remediation Steps:" >> "$REPORT_FILE"
  if [[ $DUPLICATE_COUNT -gt 0 ]]; then
    echo "1. Review version-suffixed files" >> "$REPORT_FILE"
    echo "2. Consolidate duplicates into single file with DocID" >> "$REPORT_FILE"
    echo "3. Archive old versions: mv file .ai/archive/file-archived.md" >> "$REPORT_FILE"
  fi
  if [[ $ISSUES_COUNT -gt 0 ]]; then
    echo "2. Rename files to follow PREFIX-###-name.md pattern" >> "$REPORT_FILE"
    echo "3. Update .ai/DOCUMENT_REGISTRY.md" >> "$REPORT_FILE"
  fi
else
  echo "✅ No duplicates or naming issues detected!"
fi

echo ""
echo "📊 Full report: cat $REPORT_FILE"
cat "$REPORT_FILE"

exit $(( DUPLICATE_COUNT + ISSUES_COUNT ))
