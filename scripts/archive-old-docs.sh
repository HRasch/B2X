#!/bin/bash
# Archive Old Documents Script
# Automatically archives documents > 90 days old to .ai/archive/
# Run: bash scripts/archive-old-docs.sh [--dry-run]

set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
ARCHIVE_DIR="$REPO_ROOT/.ai/archive"
DRY_RUN="${1:---}"
DAYS_THRESHOLD=90
ARCHIVE_LOG="$REPO_ROOT/.ai/cleanup-logs/archive-$(date +%Y-%m-%d-%H%M%S).log"

# Create log directory if needed
mkdir -p "$REPO_ROOT/.ai/cleanup-logs"

echo "📦 Archive Old Documents Script" | tee -a "$ARCHIVE_LOG"
echo "================================" | tee -a "$ARCHIVE_LOG"
echo "Threshold: > $DAYS_THRESHOLD days" | tee -a "$ARCHIVE_LOG"
echo "Dry Run: ${DRY_RUN}" | tee -a "$ARCHIVE_LOG"
echo "Log: $ARCHIVE_LOG" | tee -a "$ARCHIVE_LOG"
echo "" | tee -a "$ARCHIVE_LOG"

# Files to check for archival
CHECK_DIRS=(
  ".ai/logs"
  ".ai/brainstorm"
  ".ai/requirements"
  ".ai/decisions"
)

ARCHIVE_COUNT=0
ARCHIVED_FILES=""

for DIR in "${CHECK_DIRS[@]}"; do
  if [[ ! -d "$REPO_ROOT/$DIR" ]]; then
    continue
  fi
  
  echo "🔍 Scanning $DIR..." | tee -a "$ARCHIVE_LOG"
  
  while IFS= read -r -d '' file; do
    # Get file age in days
    MOD_TIME=$(stat -f %m "$file" 2>/dev/null || stat -c %Y "$file" 2>/dev/null)
    CURRENT_TIME=$(date +%s)
    AGE_DAYS=$(( ($CURRENT_TIME - $MOD_TIME) / 86400 ))
    
    # Skip if too recent
    if [[ $AGE_DAYS -lt $DAYS_THRESHOLD ]]; then
      continue
    fi
    
    BASENAME=$(basename "$file")
    RELATIVE_PATH="${file#$REPO_ROOT/}"
    
    # Skip if already archived
    if [[ "$RELATIVE_PATH" =~ ^\.ai/archive ]]; then
      continue
    fi
    
    # Check if file should be archived (has version suffix or is old report)
    if [[ "$BASENAME" =~ (PHASE_|PHASE-|_results|_report|_analysis) ]] || [[ $AGE_DAYS -gt $((DAYS_THRESHOLD + 30)) ]]; then
      
      ARCHIVE_BASENAME="${BASENAME%.md}-archived.md"
      ARCHIVE_PATH="$ARCHIVE_DIR/$ARCHIVE_BASENAME"
      
      echo "  📦 [$AGE_DAYS days] $RELATIVE_PATH" | tee -a "$ARCHIVE_LOG"
      
      if [[ "$DRY_RUN" != "--dry-run" ]]; then
        cp "$file" "$ARCHIVE_PATH"
        git rm --cached "$file" 2>/dev/null || true
        rm "$file"
        echo "     → Archived to .ai/archive/$ARCHIVE_BASENAME" | tee -a "$ARCHIVE_LOG"
      else
        echo "     → [DRY-RUN] Would archive to .ai/archive/$ARCHIVE_BASENAME" | tee -a "$ARCHIVE_LOG"
      fi
      
      ARCHIVE_COUNT=$((ARCHIVE_COUNT + 1))
      ARCHIVED_FILES="$ARCHIVED_FILES\n    - $RELATIVE_PATH → .ai/archive/$ARCHIVE_BASENAME"
    fi
  done < <(find "$REPO_ROOT/$DIR" -maxdepth 1 -type f -name "*.md" -print0 2>/dev/null || true)
done

echo "" | tee -a "$ARCHIVE_LOG"
if [[ $ARCHIVE_COUNT -gt 0 ]]; then
  echo "✅ Archived $ARCHIVE_COUNT documents:" | tee -a "$ARCHIVE_LOG"
  echo -e "$ARCHIVED_FILES" | tee -a "$ARCHIVE_LOG"
  
  if [[ "$DRY_RUN" != "--dry-run" ]]; then
    echo "" | tee -a "$ARCHIVE_LOG"
    echo "📝 Next steps:" | tee -a "$ARCHIVE_LOG"
    echo "  1. Review archived files: ls -la .ai/archive/" | tee -a "$ARCHIVE_LOG"
    echo "  2. Update .ai/DOCUMENT_REGISTRY.md if needed" | tee -a "$ARCHIVE_LOG"
    echo "  3. Commit: git add .ai/archive/ && git commit -m 'chore: archive old documents'" | tee -a "$ARCHIVE_LOG"
  fi
else
  echo "✅ No documents to archive (all recent)" | tee -a "$ARCHIVE_LOG"
fi

echo "" | tee -a "$ARCHIVE_LOG"
echo "📋 Log saved to: .ai/cleanup-logs/" | tee -a "$ARCHIVE_LOG"
