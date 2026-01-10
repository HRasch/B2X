#!/bin/bash
# scripts/validate-segregation.sh
# GL-052 Enforcement: Validates system segregation rules
# Runs monthly as part of CI/CD audit

echo "🔍 Validating system segregation (GL-052)..."

SEGREGATION_ERRORS=0

# Find all USERDOC files
USERDOCS=$(find . -name "*.md" -type f | grep -i "USERDOC-" || true)

echo "Found $(echo "$USERDOCS" | wc -l) USERDOC files"

# Check each USERDOC file for segregation violations
while IFS= read -r file; do
    if [ -z "$file" ] || [ ! -f "$file" ]; then
        continue
    fi
    
    # Extract system from filename and metadata
    SYSTEM=$(grep -m1 "^system:" "$file" | sed 's/^system:[[:space:]]*\([^[:space:]]*\).*/\1/' || true)
    
    if [ -z "$SYSTEM" ]; then
        echo "  ⚠️  Missing 'system:' field in $file"
        SEGREGATION_ERRORS=$((SEGREGATION_ERRORS+1))
        continue
    fi
    
    # Check for forbidden cross-system links
    if [[ "$SYSTEM" == "store" ]]; then
        # Store system docs cannot link to admin or management docs
        if grep -q "\[USERDOC-ADMIN-\|USERDOC-MGMT-" "$file"; then
            echo "  ❌ VIOLATION in $file: Store doc links to Admin/Management doc"
            grep -n "\[USERDOC-ADMIN-\|USERDOC-MGMT-" "$file" | sed 's/^/       Line /'
            SEGREGATION_ERRORS=$((SEGREGATION_ERRORS+1))
        fi
    elif [[ "$SYSTEM" == "admin" ]]; then
        # Admin system docs cannot link to store or management docs
        if grep -q "\[USERDOC-STORE-\|USERDOC-MGMT-" "$file"; then
            echo "  ❌ VIOLATION in $file: Admin doc links to Store/Management doc"
            grep -n "\[USERDOC-STORE-\|USERDOC-MGMT-" "$file" | sed 's/^/       Line /'
            SEGREGATION_ERRORS=$((SEGREGATION_ERRORS+1))
        fi
    elif [[ "$SYSTEM" == "management" ]]; then
        # Management system docs cannot link to store or admin docs
        if grep -q "\[USERDOC-STORE-\|USERDOC-ADMIN-" "$file"; then
            echo "  ❌ VIOLATION in $file: Management doc links to Store/Admin doc"
            grep -n "\[USERDOC-STORE-\|USERDOC-ADMIN-" "$file" | sed 's/^/       Line /'
            SEGREGATION_ERRORS=$((SEGREGATION_ERRORS+1))
        fi
    fi
    
    # Check that audience metadata is populated
    if ! grep -q "audience.systems:" "$file"; then
        echo "  ⚠️  Missing 'audience.systems:' in $file"
    fi
    if ! grep -q "audience.exclude_roles:" "$file"; then
        echo "  ⚠️  Missing 'audience.exclude_roles:' in $file"
    fi
done <<< "$USERDOCS"

# Summary by system
echo ""
echo "System segregation audit:"
STORE_DOCS=$(echo "$USERDOCS" | grep -i "USERDOC-STORE-" | wc -l || echo 0)
ADMIN_DOCS=$(echo "$USERDOCS" | grep -i "USERDOC-ADMIN-" | wc -l || echo 0)
MGMT_DOCS=$(echo "$USERDOCS" | grep -i "USERDOC-MGMT-" | wc -l || echo 0)

echo "  Store system: $STORE_DOCS docs"
echo "  Admin system: $ADMIN_DOCS docs"
echo "  Management system: $MGMT_DOCS docs"

echo ""
if [ $SEGREGATION_ERRORS -eq 0 ]; then
    echo "✅ System segregation validation passed (GL-052 compliant)"
    exit 0
else
    echo "❌ System segregation validation failed ($SEGREGATION_ERRORS violations)"
    echo ""
    echo "GL-052 Rules:"
    echo "  ❌ USERDOC-STORE-* cannot link to USERDOC-ADMIN-* or USERDOC-MGMT-*"
    echo "  ❌ USERDOC-ADMIN-* cannot link to USERDOC-STORE-* or USERDOC-MGMT-*"
    echo "  ❌ USERDOC-MGMT-* cannot link to USERDOC-STORE-* or USERDOC-ADMIN-*"
    echo ""
    exit 1
fi
