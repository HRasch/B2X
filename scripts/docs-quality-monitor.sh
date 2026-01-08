#!/bin/bash
# scripts/docs-quality-monitor.sh
# Documentation Quality Monitoring Script
# Tracks metrics: completeness, accuracy, usage patterns
# Generates weekly reports in .ai/logs/documentation/

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
LOGS_DIR="$REPO_ROOT/.ai/logs/documentation"
REPORT_FILE="$LOGS_DIR/docs-quality-report_$(date +%Y-%m-%d_%H-%M-%S).md"

# Create logs directory if it doesn't exist
mkdir -p "$LOGS_DIR"

echo "📊 Starting documentation quality monitoring..."

# Initialize metrics
declare -A METRICS
METRICS[TOTAL_DOCS]=0
METRICS[FRONTMATTER_COMPLETE]=0
METRICS[DOCID_VALID]=0
METRICS[CROSSREF_VALID]=0
METRICS[FRESH_DOCS]=0
METRICS[STALE_DOCS]=0
METRICS[BROKEN_REFS]=0
METRICS[USAGE_PATTERNS]=0

# Find all documentation files
DOCS=$(find .ai .github docs -name "*.md" -type f 2>/dev/null | sort || true)
METRICS[TOTAL_DOCS]=$(echo "$DOCS" | wc -l)

echo "Found ${METRICS[TOTAL_DOCS]} documentation files"

# Arrays for tracking issues
declare -a MISSING_FRONTMATTER
declare -a INVALID_DOCIDS
declare -a BROKEN_CROSSREFS
declare -a STALE_FILES
declare -a FRESH_FILES

# 1. Frontmatter completeness check
echo "  ✓ Checking frontmatter completeness..."
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi

    # Check if file should have frontmatter (.ai/ and .github/ docs)
    if [[ "$file" == .ai/* ]] || [[ "$file" == .github/* ]]; then
        REQUIRED_FIELDS=("docid:" "title:" "status:" "created:")
        MISSING_COUNT=0

        for field in "${REQUIRED_FIELDS[@]}"; do
            if ! grep -q "^$field" "$file"; then
                MISSING_COUNT=$((MISSING_COUNT+1))
            fi
        done

        if [ $MISSING_COUNT -eq 0 ]; then
            METRICS[FRONTMATTER_COMPLETE]=$((METRICS[FRONTMATTER_COMPLETE]+1))
        else
            MISSING_FRONTMATTER+=("$file (missing $MISSING_COUNT fields)")
        fi
    fi
done <<< "$DOCS"

# 2. DocID uniqueness and format compliance
echo "  ✓ Checking DocID validity..."
declare -A DOCID_MAP
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi

    if [[ "$file" == .ai/* ]] || [[ "$file" == .github/* ]]; then
        DOCID=$(grep -m1 "^docid:" "$file" | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)

        if [ -n "$DOCID" ]; then
            # Check format: PREFIX-NNN or PREFIX-NNN-SHORTNAME
            if [[ "$DOCID" =~ ^[A-Z]+[-]?[0-9]{3}(-[A-Z0-9-]+)?$ ]]; then
                # Check uniqueness
                if [ -n "${DOCID_MAP[$DOCID]}" ]; then
                    INVALID_DOCIDS+=("$file: Duplicate DocID '$DOCID' (also in ${DOCID_MAP[$DOCID]})")
                else
                    DOCID_MAP[$DOCID]="$file"
                    METRICS[DOCID_VALID]=$((METRICS[DOCID_VALID]+1))
                fi
            else
                INVALID_DOCIDS+=("$file: Invalid DocID format '$DOCID'")
            fi
        fi
    fi
done <<< "$DOCS"

# 3. Cross-reference validity
echo "  ✓ Checking cross-references..."
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi

    # Find all DocID references like [DOC-001], [KB-053], etc.
    REFERENCES=$(grep -o '\[[A-Z]+[-]?[0-9]\{3\}(-[A-Z0-9-]\+)?\]' "$file" | sed 's/\[\(.*\)\]/\1/' || true)

    while IFS= read -r ref; do
        if [ -n "$ref" ]; then
            # Check if referenced DocID exists in registry
            if ! grep -q "^| \`$ref\`" .ai/DOCUMENT_REGISTRY.md 2>/dev/null; then
                BROKEN_CROSSREFS+=("$file: Broken reference to '$ref'")
                METRICS[BROKEN_REFS]=$((METRICS[BROKEN_REFS]+1))
            else
                METRICS[CROSSREF_VALID]=$((METRICS[CROSSREF_VALID]+1))
            fi
        fi
    done <<< "$REFERENCES"
done <<< "$DOCS"

# 4. File freshness analysis
echo "  ✓ Analyzing file freshness..."
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi

    # Get file modification time
    MOD_TIME=$(stat -c %Y "$file" 2>/dev/null || stat -f %m "$file" 2>/dev/null || echo 0)
    NOW=$(date +%s)
    DAYS_OLD=$(( ($NOW - $MOD_TIME) / 86400 ))

    if [ "$DAYS_OLD" -le 30 ]; then
        METRICS[FRESH_DOCS]=$((METRICS[FRESH_DOCS]+1))
        FRESH_FILES+=("$file (${DAYS_OLD} days old)")
    elif [ "$DAYS_OLD" -gt 180 ]; then
        METRICS[STALE_DOCS]=$((METRICS[STALE_DOCS]+1))
        STALE_FILES+=("$file (${DAYS_OLD} days old)")
    fi
done <<< "$DOCS"

# 5. Usage patterns (basic analysis - files with many references)
echo "  ✓ Analyzing usage patterns..."
declare -A USAGE_COUNT
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi

    DOCID=$(grep -m1 "^docid:" "$file" | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)

    if [ -n "$DOCID" ]; then
        # Count references to this DocID across all files
        COUNT=$(grep -r "\[$DOCID\]" .ai .github docs --include="*.md" 2>/dev/null | wc -l || echo 0)
        USAGE_COUNT[$DOCID]=$COUNT

        if [ "$COUNT" -gt 5 ]; then
            METRICS[USAGE_PATTERNS]=$((METRICS[USAGE_PATTERNS]+1))
        fi
    fi
done <<< "$DOCS"

# Generate report
echo "📝 Generating quality report..."

cat > "$REPORT_FILE" << EOF
---
docid: REPORT-DOCS-QUALITY-$(date +%Y%m%d)
title: Documentation Quality Report
status: Generated
created: $(date +%Y-%m-%d)
---

# 📊 Documentation Quality Report
**Generated:** $(date)
**Total Files:** ${METRICS[TOTAL_DOCS]}

## 📈 Quality Metrics

| Metric | Value | Percentage |
|--------|-------|------------|
| Frontmatter Complete | ${METRICS[FRONTMATTER_COMPLETE]} | $(echo "scale=1; ${METRICS[FRONTMATTER_COMPLETE]} * 100 / ${METRICS[TOTAL_DOCS]}" | bc -l 2>/dev/null || echo "N/A")% |
| Valid DocIDs | ${METRICS[DOCID_VALID]} | $(echo "scale=1; ${METRICS[DOCID_VALID]} * 100 / ${METRICS[TOTAL_DOCS]}" | bc -l 2>/dev/null || echo "N/A")% |
| Valid Cross-References | ${METRICS[CROSSREF_VALID]} | - |
| Fresh Files (≤30 days) | ${METRICS[FRESH_DOCS]} | $(echo "scale=1; ${METRICS[FRESH_DOCS]} * 100 / ${METRICS[TOTAL_DOCS]}" | bc -l 2>/dev/null || echo "N/A")% |
| Stale Files (>180 days) | ${METRICS[STALE_DOCS]} | $(echo "scale=1; ${METRICS[STALE_DOCS]} * 100 / ${METRICS[TOTAL_DOCS]}" | bc -l 2>/dev/null || echo "N/A")% |
| Broken References | ${METRICS[BROKEN_REFS]} | - |
| Highly Referenced Docs | ${METRICS[USAGE_PATTERNS]} | - |

## ⚠️ Issues Found

### Missing Frontmatter
$(printf '%s\n' "${MISSING_FRONTMATTER[@]}" | head -10 | sed 's/^/- /' || echo "None")

### Invalid DocIDs
$(printf '%s\n' "${INVALID_DOCIDS[@]}" | head -10 | sed 's/^/- /' || echo "None")

### Broken Cross-References
$(printf '%s\n' "${BROKEN_CROSSREFS[@]}" | head -10 | sed 's/^/- /' || echo "None")

### Stale Files (>180 days)
$(printf '%s\n' "${STALE_FILES[@]}" | head -10 | sed 's/^/- /' || echo "None")

## 📈 Usage Patterns (Top 10 Most Referenced)

$(for docid in "${!USAGE_COUNT[@]}"; do
    echo "${USAGE_COUNT[$docid]} $docid"
done | sort -nr | head -10 | while read count docid; do
    echo "- $docid: $count references"
done)

## 🎯 Recommendations

$(if [ ${#MISSING_FRONTMATTER[@]} -gt 0 ]; then
    echo "- Add missing frontmatter fields to ${#MISSING_FRONTMATTER[@]} files"
fi
if [ ${#INVALID_DOCIDS[@]} -gt 0 ]; then
    echo "- Fix ${#INVALID_DOCIDS[@]} invalid DocID formats"
fi
if [ ${#BROKEN_CROSSREFS[@]} -gt 0 ]; then
    echo "- Repair ${#BROKEN_CROSSREFS[@]} broken cross-references"
fi
if [ ${METRICS[STALE_DOCS]} -gt 0 ]; then
    echo "- Review ${METRICS[STALE_DOCS]} stale documentation files"
fi
echo "- Consider archiving files not modified in 180+ days"
echo "- Update DOCUMENT_REGISTRY.md for any new DocIDs")

## 📋 Quality Score

**Overall Score:** $(echo "scale=1; ((${METRICS[FRONTMATTER_COMPLETE]} + ${METRICS[DOCID_VALID]} + ${METRICS[FRESH_DOCS]}) * 100) / (${METRICS[TOTAL_DOCS]} * 3)" | bc -l 2>/dev/null || echo "N/A")/100

**Grade:** $(SCORE=$(echo "scale=1; ((${METRICS[FRONTMATTER_COMPLETE]} + ${METRICS[DOCID_VALID]} + ${METRICS[FRESH_DOCS]}) * 100) / (${METRICS[TOTAL_DOCS]} * 3)" | bc -l 2>/dev/null || echo 0); if (( $(echo "$SCORE >= 90" | bc -l) )); then echo "A (Excellent)"; elif (( $(echo "$SCORE >= 80" | bc -l) )); then echo "B (Good)"; elif (( $(echo "$SCORE >= 70" | bc -l) )); then echo "C (Fair)"; else echo "D (Needs Improvement)"; fi)

---
*Report generated by docs-quality-monitor.sh*
EOF

echo "✅ Quality report generated: $REPORT_FILE"

# Check for alerts (quality degradation)
ALERTS=0
if [ ${#MISSING_FRONTMATTER[@]} -gt 10 ]; then
    echo "🚨 ALERT: High number of missing frontmatter fields (${#MISSING_FRONTMATTER[@]})"
    ALERTS=$((ALERTS+1))
fi
if [ ${#BROKEN_CROSSREFS[@]} -gt 5 ]; then
    echo "🚨 ALERT: High number of broken cross-references (${#BROKEN_CROSSREFS[@]})"
    ALERTS=$((ALERTS+1))
fi
if [ ${METRICS[STALE_DOCS]} -gt 50 ]; then
    echo "🚨 ALERT: High number of stale files (${METRICS[STALE_DOCS]})"
    ALERTS=$((ALERTS+1))
fi

if [ $ALERTS -gt 0 ]; then
    echo "⚠️  $ALERTS quality alerts triggered. Review report for details."
else
    echo "✅ No quality alerts triggered."
fi

echo "📊 Documentation quality monitoring completed."
echo "Report: $REPORT_FILE"