#!/bin/bash
# scripts/docs-validation.sh
# Pre-commit hook for documentation validation
# Validates: DocID format, duplicates, registry entries, system segregation, YAML metadata

set -e

echo "🔍 Validating documentation..."

# Get list of staged .md files in .ai/ and .github/
STAGED_DOCS=$(git diff --cached --name-only --diff-filter=ACM | grep -E '\.ai/|\.github/' | grep '\.md$' || true)

if [ -z "$STAGED_DOCS" ]; then
    echo "✅ No documentation changes to validate"
    exit 0
fi

ERRORS=0

# 1. Validate DocID format and existence
echo "  ✓ Checking DocID format and registry entries..."
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Extract docid from frontmatter
    DOCID=$(grep -m1 "^docid:" "$file" | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    
    # Check if file should have a docid (all .ai/ and .github/ docs should)
    if [[ "$file" == .ai/* ]] || [[ "$file" == .github/* ]]; then
        if [ -z "$DOCID" ]; then
            echo "    ❌ Missing docid in $file"
            ERRORS=$((ERRORS+1))
        else
            # Validate DocID format: PREFIX-NNN or PREFIX-NNN-SHORTNAME
            if ! [[ "$DOCID" =~ ^[A-Z]+[-]?[0-9]{3}(-[A-Z0-9-]+)?$ ]]; then
                echo "    ❌ Invalid DocID format '$DOCID' in $file (use PREFIX-NNN or PREFIX-NNN-NAME)"
                ERRORS=$((ERRORS+1))
            fi
            
            # Check if DocID exists in DOCUMENT_REGISTRY.md
            if ! grep -q "^\| \`$DOCID\`" .ai/DOCUMENT_REGISTRY.md 2>/dev/null; then
                echo "    ❌ DocID '$DOCID' in $file not found in DOCUMENT_REGISTRY.md"
                ERRORS=$((ERRORS+1))
            fi
        fi
    fi
done <<< "$STAGED_DOCS"

# 2. Check for duplicate DocIDs in staged docs
echo "  ✓ Checking for duplicate DocIDs..."
DOCIDS=$(grep -h "^docid:" $STAGED_DOCS 2>/dev/null | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' | sort)
DUPLICATES=$(echo "$DOCIDS" | uniq -d || true)
if [ -n "$DUPLICATES" ]; then
    echo "    ❌ Duplicate DocIDs detected:"
    echo "$DUPLICATES" | sed 's/^/       /'
    ERRORS=$((ERRORS+1))
fi

# 3. Validate YAML metadata completeness
echo "  ✓ Validating YAML metadata..."
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check required fields
    for field in "^docid:" "^title:" "^status:" "^created:"; do
        if ! grep -q "$field" "$file"; then
            echo "    ❌ Missing field '$field' in $file"
            ERRORS=$((ERRORS+1))
        fi
    done
done <<< "$STAGED_DOCS"

# 4. Check cross-system links (GL-052 enforcement)
echo "  ✓ Checking system segregation (GL-052)..."
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Extract system from YAML
    SYSTEM=$(grep -m1 "^system:" "$file" | sed 's/^system:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    
    # Check forbidden cross-system links
    if [[ "$SYSTEM" == "store" ]]; then
        # Store docs cannot link to admin or management docs
        if grep -q "\[USERDOC-ADMIN-\|USERDOC-MGMT-" "$file"; then
            echo "    ❌ Cross-system link violation in $file: Store doc cannot link to Admin/Management docs"
            ERRORS=$((ERRORS+1))
        fi
    elif [[ "$SYSTEM" == "admin" ]]; then
        # Admin docs cannot link to store or management docs
        if grep -q "\[USERDOC-STORE-\|USERDOC-MGMT-" "$file"; then
            echo "    ❌ Cross-system link violation in $file: Admin doc cannot link to Store/Management docs"
            ERRORS=$((ERRORS+1))
        fi
    elif [[ "$SYSTEM" == "management" ]]; then
        # Management docs cannot link to store or admin docs
        if grep -q "\[USERDOC-STORE-\|USERDOC-ADMIN-" "$file"; then
            echo "    ❌ Cross-system link violation in $file: Management doc cannot link to Store/Admin docs"
            ERRORS=$((ERRORS+1))
        fi
    fi
done <<< "$STAGED_DOCS"

# 5. Validate USERDOC audience fields
echo "  ✓ Checking audience metadata for USERDOC files..."
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check if file is a USERDOC file
    DOCID=$(grep -m1 "^docid:" "$file" | sed 's/^docid:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    if [[ "$DOCID" == USERDOC-* ]]; then
        # Must have system, audience.systems, and exclude_roles
        if ! grep -q "^system:" "$file"; then
            echo "    ❌ USERDOC file missing 'system:' in $file"
            ERRORS=$((ERRORS+1))
        fi
        if ! grep -q "audience.systems:" "$file"; then
            echo "    ❌ USERDOC file missing 'audience.systems:' in $file"
            ERRORS=$((ERRORS+1))
        fi
        if ! grep -q "audience.exclude_roles:" "$file"; then
            echo "    ❌ USERDOC file missing 'audience.exclude_roles:' in $file"
            ERRORS=$((ERRORS+1))
        fi
    fi
done <<< "$STAGED_DOCS"

# Print summary
echo ""
if [ $ERRORS -eq 0 ]; then
    echo "✅ Documentation validation passed (all checks passed)"
    exit 0
else
    echo "❌ Documentation validation failed ($ERRORS errors found)"
    echo ""
    echo "To fix:"
    echo "1. Add missing docid: fields"
    echo "2. Register DocIDs in .ai/DOCUMENT_REGISTRY.md"
    echo "3. Remove cross-system links (GL-052 rules)"
    echo "4. Complete YAML metadata (docid, title, status, created)"
    echo ""
    exit 1
fi
