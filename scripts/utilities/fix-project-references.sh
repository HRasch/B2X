#!/bin/bash
# fix-project-references.sh
# Automatically fix project references in .csproj files for the new directory structure

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  Fix Project References${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_section() {
    echo -e "${GREEN}[SECTION]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

# Function to backup files before modification
backup_files() {
    print_section "Creating Backups"

    local backup_dir="backup-$(date +%Y%m%d-%H%M%S)"
    mkdir -p "$backup_dir"

    echo "Backing up .csproj files to $backup_dir..."
    find . -name "*.csproj" -not -path "./.git/*" -not -path "./node_modules/*" | while read -r file; do
        local relative_path="${file#./}"
        local backup_path="$backup_dir/$relative_path"
        mkdir -p "$(dirname "$backup_path")"
        cp "$file" "$backup_path"
    done

    print_success "Backups created in $backup_dir"
    echo "$backup_dir" > .last_backup
}

# Function to fix project references
fix_project_references() {
    print_section "Fixing Project References"

    local fixed_count=0
    local total_files=0

    # Find all .csproj files
    find . -name "*.csproj" -not -path "./.git/*" -not -path "./node_modules/*" | while read -r file; do
        total_files=$((total_files + 1))
        local modified=false

        echo "Processing $file..."

        # Create a temporary file for modifications
        local temp_file="${file}.tmp"

        # Read the file and apply transformations
        while IFS= read -r line; do
            local new_line="$line"

            # Fix Backend references
            new_line=$(echo "$new_line" | sed 's|Include="\.\./\.\./src/|Include="src/src/|g')
            new_line=$(echo "$new_line" | sed 's|Include="\.\./src/|Include="src/src/|g')
            new_line=$(echo "$new_line" | sed 's|Include="src/|Include="src/src/|g')

            # Fix Frontend references
            new_line=$(echo "$new_line" | sed 's|Include="\.\./\.\./src/|Include="src/src/|g')
            new_line=$(echo "$new_line" | sed 's|Include="\.\./src/|Include="src/src/|g')
            new_line=$(echo "$new_line" | sed 's|Include="src/|Include="src/src/|g')

            # Fix ServiceDefaults references
            new_line=$(echo "$new_line" | sed 's|Include="\.\./\.\./ServiceDefaults/|Include="src/ServiceDefaults/|g')
            new_line=$(echo "$new_line" | sed 's|Include="\.\./ServiceDefaults/|Include="src/ServiceDefaults/|g')
            new_line=$(echo "$new_line" | sed 's|Include="ServiceDefaults/|Include="src/ServiceDefaults/|g')

            # Fix AppHost references
            new_line=$(echo "$new_line" | sed 's|Include="\.\./\.\./AppHost/|Include="src/AppHost/|g')
            new_line=$(echo "$new_line" | sed 's|Include="\.\./AppHost/|Include="src/AppHost/|g')
            new_line=$(echo "$new_line" | sed 's|Include="AppHost/|Include="src/AppHost/|g')

            # Check if line was modified
            if [ "$new_line" != "$line" ]; then
                modified=true
                echo "  Fixed: $line"
                echo "  To:    $new_line"
            fi

            echo "$new_line" >> "$temp_file"
        done < "$file"

        # Replace original file if modified
        if [ "$modified" = true ]; then
            mv "$temp_file" "$file"
            fixed_count=$((fixed_count + 1))
            print_success "Updated $file"
        else
            rm -f "$temp_file"
        fi
    done

    echo
    print_success "Processed $total_files files, fixed $fixed_count files"
}

# Function to validate fixes
validate_fixes() {
    print_section "Validating Fixes"

    echo "Running build configuration audit to verify fixes..."

    if [ -f "scripts/check-build-configs.sh" ]; then
        bash scripts/check-build-configs.sh > check-build-configs-after-fix.log 2>&1

        # Count remaining issues
        local remaining_warnings=$(grep -c "WARNING\|⚠" check-build-configs-after-fix.log 2>/dev/null || echo "0")
        local remaining_errors=$(grep -c "ERROR\|✗\|CRITICAL" check-build-configs-after-fix.log 2>/dev/null || echo "0")

        echo "Validation results:"
        echo "  Remaining warnings: $remaining_warnings"
        echo "  Remaining errors: $remaining_errors"

        if [ "$remaining_errors" -gt 0 ]; then
            print_error "Critical issues still remain - manual review needed"
        elif [ "$remaining_warnings" -gt 0 ]; then
            print_warning "Some warnings remain - may need manual fixes"
        else
            print_success "All project references successfully fixed!"
        fi
    else
        print_warning "Validation script not found"
    fi
}

# Function to generate summary
generate_summary() {
    print_section "Summary"

    local backup_dir=$(cat .last_backup 2>/dev/null || echo "unknown")

    echo "Project reference fixes completed:"
    echo "- Backup location: $backup_dir"
    echo "- Fixed files: Review the output above"
    echo "- Validation: Check check-build-configs-after-fix.log"
    echo
    print_warning "Test builds immediately to ensure references work correctly!"
    echo
    echo "Next steps:"
    echo "1. Run: dotnet build B2X.slnx"
    echo "2. Fix any remaining compilation errors"
    echo "3. Run tests to validate functionality"
}

main() {
    print_header
    echo "Fixing project references for new directory structure..."
    echo "This will update all .csproj files to use src/ paths instead of relative paths."
    echo

    read -p "Do you want to proceed? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Operation cancelled."
        exit 0
    fi

    backup_files
    echo
    fix_project_references
    echo
    validate_fixes
    echo
    generate_summary
}

main "$@"