#!/bin/bash
# scripts/validate-metadata.sh
# Validates documentation metadata completeness
# Runs as part of CI/CD weekly audit

echo "🔍 Validating documentation metadata..."

METADATA_ERRORS=0

# Find all documentation files
DOCS=$(find .ai .github docs -name "*.md" -type f 2>/dev/null || true)

echo "Found $(echo "$DOCS" | wc -l) documentation files"

# Check each doc for required metadata
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi
    
    # Extract metadata
    DOCID=$(grep -m1 "^docid:" "$file" | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    TITLE=$(grep -m1 "^title:" "$file" | sed 's/^title:[[:space:]]*"*\(.*\)"*/\1/' || true)
    STATUS=$(grep -m1 "^status:" "$file" | sed 's/^status:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    CREATED=$(grep -m1 "^created:" "$file" | sed 's/^created:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    
    # Should have metadata if in .ai/ or .github/
    if [[ "$file" == .ai/* ]] || [[ "$file" == .github/* ]]; then
        if [ -z "$DOCID" ]; then
            echo "  ⚠️  Missing docid in $file"
            METADATA_ERRORS=$((METADATA_ERRORS+1))
        fi
        if [ -z "$TITLE" ]; then
            echo "  ⚠️  Missing title in $file"
            METADATA_ERRORS=$((METADATA_ERRORS+1))
        fi
        if [ -z "$STATUS" ]; then
            echo "  ⚠️  Missing status in $file"
            METADATA_ERRORS=$((METADATA_ERRORS+1))
        fi
        if [ -z "$CREATED" ]; then
            echo "  ⚠️  Missing created date in $file"
            METADATA_ERRORS=$((METADATA_ERRORS+1))
        fi
    fi
    
    # Check for stale docs (created > 180 days ago, status = Active)
    if [ -n "$CREATED" ] && [ "$STATUS" = "Active" ]; then
        CREATED_TS=$(date -d "$CREATED" +%s 2>/dev/null || echo 0)
        NOW_TS=$(date +%s)
        DAYS_OLD=$(( ($NOW_TS - $CREATED_TS) / 86400 ))
        
        if [ "$DAYS_OLD" -gt 180 ]; then
            echo "  🕐 Potentially stale (>180 days old): $file ($DAYS_OLD days)"
        fi
    fi
done <<< "$DOCS"

echo ""
if [ $METADATA_ERRORS -eq 0 ]; then
    echo "✅ Metadata validation passed (no critical issues)"
    exit 0
else
    echo "⚠️  Metadata validation found $METADATA_ERRORS issues"
    echo "These are warnings, not blocking errors"
    exit 0  # Don't fail CI, just warn
fi
