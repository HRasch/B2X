#!/bin/bash

# B2Connect CLI Migration Script
# Automates migration from B2Connect.CLI to specialized CLIs
# Version: 1.0.0
# Date: January 5, 2026

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
OPERATIONS_CLI="B2Connect.CLI.Operations"
ADMINISTRATION_CLI="B2Connect.CLI.Administration"
OLD_CLI="B2Connect.CLI"

# Functions
print_header() {
    echo -e "${BLUE}=====================================${NC}"
    echo -e "${BLUE}B2Connect CLI Migration Tool${NC}"
    echo -e "${BLUE}=====================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_info() {
    echo -e "${BLUE}ℹ${NC} $1"
}

check_prerequisites() {
    print_info "Checking prerequisites..."
    
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK not found. Please install .NET SDK first."
        exit 1
    fi
    
    print_success ".NET SDK found: $(dotnet --version)"
}

detect_old_cli() {
    print_info "Checking for old CLI installation..."
    
    if dotnet tool list -g | grep -q "$OLD_CLI"; then
        print_warning "Old CLI ($OLD_CLI) is installed"
        return 0
    else
        print_info "Old CLI ($OLD_CLI) not found"
        return 1
    fi
}

detect_user_role() {
    echo ""
    echo "Which CLI tools do you need?"
    echo "1) Operations CLI (Platform Operators - DevOps/SRE)"
    echo "2) Administration CLI (Tenant Administrators)"
    echo "3) Both CLIs"
    echo ""
    read -p "Enter choice (1-3): " choice
    
    case $choice in
        1)
            INSTALL_OPERATIONS=true
            INSTALL_ADMINISTRATION=false
            ;;
        2)
            INSTALL_OPERATIONS=false
            INSTALL_ADMINISTRATION=true
            ;;
        3)
            INSTALL_OPERATIONS=true
            INSTALL_ADMINISTRATION=true
            ;;
        *)
            print_error "Invalid choice"
            exit 1
            ;;
    esac
}

install_new_clis() {
    echo ""
    print_info "Installing new CLI tools..."
    
    if [ "$INSTALL_OPERATIONS" = true ]; then
        print_info "Installing $OPERATIONS_CLI..."
        if dotnet tool install -g $OPERATIONS_CLI; then
            print_success "$OPERATIONS_CLI installed successfully"
        else
            print_error "Failed to install $OPERATIONS_CLI"
            exit 1
        fi
    fi
    
    if [ "$INSTALL_ADMINISTRATION" = true ]; then
        print_info "Installing $ADMINISTRATION_CLI..."
        if dotnet tool install -g $ADMINISTRATION_CLI; then
            print_success "$ADMINISTRATION_CLI installed successfully"
        else
            print_error "Failed to install $ADMINISTRATION_CLI"
            exit 1
        fi
    fi
}

uninstall_old_cli() {
    echo ""
    read -p "Uninstall old CLI ($OLD_CLI)? (y/n): " uninstall_choice
    
    if [ "$uninstall_choice" = "y" ] || [ "$uninstall_choice" = "Y" ]; then
        print_info "Uninstalling $OLD_CLI..."
        if dotnet tool uninstall -g $OLD_CLI; then
            print_success "$OLD_CLI uninstalled successfully"
        else
            print_warning "Failed to uninstall $OLD_CLI (may not be installed)"
        fi
    else
        print_warning "Skipping old CLI uninstallation. You can keep both during transition."
    fi
}

update_scripts() {
    echo ""
    read -p "Search for and update script files? (y/n): " update_choice
    
    if [ "$update_choice" = "y" ] || [ "$update_choice" = "Y" ]; then
        read -p "Enter directory to search (default: .): " search_dir
        search_dir=${search_dir:-.}
        
        print_info "Searching for scripts in $search_dir..."
        
        # Find shell scripts
        script_files=$(find "$search_dir" -type f \( -name "*.sh" -o -name "*.bash" \) 2>/dev/null || true)
        
        if [ -z "$script_files" ]; then
            print_info "No script files found"
        else
            echo "$script_files" | while read -r file; do
                if grep -q "b2connect " "$file"; then
                    print_warning "Found b2connect commands in: $file"
                    echo "  Please review and update manually."
                fi
            done
        fi
    fi
}

show_migration_summary() {
    echo ""
    print_header
    echo -e "${GREEN}Migration Summary:${NC}"
    echo ""
    
    if [ "$INSTALL_OPERATIONS" = true ]; then
        print_success "Operations CLI installed: b2connect-ops"
        echo "  Example: b2connect-ops health check"
    fi
    
    if [ "$INSTALL_ADMINISTRATION" = true ]; then
        print_success "Administration CLI installed: b2connect-admin"
        echo "  Example: b2connect-admin tenant create --name 'Test'"
    fi
    
    echo ""
    echo "Next Steps:"
    echo "1. Update your scripts with new command names"
    echo "2. Update environment variables:"
    if [ "$INSTALL_OPERATIONS" = true ]; then
        echo "   - Operations: B2CONNECT_OPS_TOKEN"
    fi
    if [ "$INSTALL_ADMINISTRATION" = true ]; then
        echo "   - Administration: B2CONNECT_TENANT_TOKEN"
    fi
    echo "3. Test your workflows"
    echo "4. Review migration guide: https://docs.b2connect.com/cli/migration"
    echo ""
}

verify_installation() {
    echo ""
    print_info "Verifying installation..."
    
    if [ "$INSTALL_OPERATIONS" = true ]; then
        if command -v b2connect-ops &> /dev/null; then
            print_success "b2connect-ops is available"
        else
            print_error "b2connect-ops command not found. Try: source ~/.bashrc"
        fi
    fi
    
    if [ "$INSTALL_ADMINISTRATION" = true ]; then
        if command -v b2connect-admin &> /dev/null; then
            print_success "b2connect-admin is available"
        else
            print_error "b2connect-admin command not found. Try: source ~/.bashrc"
        fi
    fi
}

# Main execution
main() {
    print_header
    
    check_prerequisites
    detect_old_cli
    detect_user_role
    install_new_clis
    uninstall_old_cli
    update_scripts
    verify_installation
    show_migration_summary
    
    echo -e "${GREEN}Migration completed successfully!${NC}"
}

# Run main function
main
