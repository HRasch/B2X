#!/bin/bash
# path-mapping.sh
# Quick reference for B2X refactoring path mappings

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  B2X Path Mapping Reference${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_section() {
    echo -e "${GREEN}[SECTION]${NC} $1"
}

print_mapping() {
    echo -e "${YELLOW}FROM:${NC} $1"
    echo -e "${GREEN}TO:  ${NC} $2"
    echo
}

show_csharp_mappings() {
    print_section "C# Project References (.csproj)"
    print_mapping "../AppHost/B2X.AppHost.csproj" "../src/AppHost/B2X.AppHost.csproj"
    print_mapping "../src/Api/B2X.Api.csproj" "../src/src/Api/B2X.Api.csproj"
    print_mapping "../src/Domain/B2X.Domain.csproj" "../src/src/Domain/B2X.Domain.csproj"
    print_mapping "../src/Store/package.json" "../src/src/Store/package.json"
}

show_code_mappings() {
    print_section "C# Code References"
    print_mapping "using B2X.Backend.Domain;" "using B2X.src.Backend.Domain;"
    print_mapping "namespace B2X.Backend.Api" "namespace B2X.src.Backend.Api"
    print_mapping "\"src/Domain/Entities/\"" "\"src/src/Domain/Entities/\""
}

show_typescript_mappings() {
    print_section "TypeScript/JavaScript Imports"
    print_mapping "import { Api } from '../../src/Api'" "import { Api } from '../../src/src/Api'"
    print_mapping "import config from '../../../src/Store/config'" "import config from '../../../src/src/Store/config'"
}

show_markdown_mappings() {
    print_section "Documentation Links"
    print_mapping "[Backend API](../src/Api/README.md)" "[Backend API](../src/src/Api/README.md)"
    print_mapping "[Frontend Guide](../src/Store/docs/guide.md)" "[Frontend Guide](../src/src/Store/docs/guide.md)"
    print_mapping "[AppHost Config](../AppHost/appsettings.json)" "[AppHost Config](../src/AppHost/appsettings.json)"
}

show_config_mappings() {
    print_section "Configuration Files"
    print_mapping "../src/appsettings.Development.json" "../src/src/appsettings.Development.json"
    print_mapping "../src/Store/package.json" "../src/src/Store/package.json"
}

show_script_mappings() {
    print_section "Build Scripts"
    print_mapping "./src/run-tests.sh" "./src/src/run-tests.sh"
    print_mapping "./src/Store/build.sh" "./src/src/Store/build.sh"
}

show_regex_patterns() {
    print_section "Regex Patterns for Bulk Replacement"

    echo -e "${YELLOW}C# Files (.cs):${NC}"
    echo "Namespace: s/namespace B2X\.Backend\./namespace B2X.src.Backend./g"
    echo "Using:     s/using B2X\.Backend\./using B2X.src.Backend./g"
    echo "Paths:     s/\"Backend\//\"src/Backend\//g"
    echo

    echo -e "${YELLOW}TypeScript (.ts/.js/.vue):${NC}"
    echo "Imports:   s/from ['\"](\.\./)+\.\./Backend\//from \$1src/Backend\//g"
    echo

    echo -e "${YELLOW}Markdown (.md):${NC}"
    echo "Links:     s/\(\.\./Backend\//\(../src/Backend\//g"
    echo

    echo -e "${YELLOW}Project Files (.csproj):${NC}"
    echo "Refs:      s/<ProjectReference Include=\"\.\./Backend\//<ProjectReference Include=\"../src/Backend\//g"
}

show_validation_checklist() {
    print_section "Post-Update Validation Checklist"
    echo "□ .NET projects build successfully (dotnet build)"
    echo "□ TypeScript compilation passes (npm run build)"
    echo "□ All relative imports resolve"
    echo "□ Documentation links work"
    echo "□ Build scripts execute"
    echo "□ Configuration files load"
    echo "□ Tests run successfully"
}

show_file_counts() {
    print_section "Files Requiring Updates"
    echo -e "${RED}HIGH PRIORITY (Break Builds):${NC}"
    echo "• 77 .csproj files: Project references"
    echo "• 843 .cs files: Namespace declarations, using statements"
    echo "• 36 .ts/.js/.vue files: Import statements"
    echo
    echo -e "${YELLOW}MEDIUM PRIORITY (Break Runtime):${NC}"
    echo "• 84 .json files: Configuration paths"
    echo "• 14 .yml/.yaml files: Docker, CI/CD paths"
    echo "• 50 .sh files: Build script paths"
    echo
    echo -e "${GREEN}LOW PRIORITY (Break Documentation):${NC}"
    echo "• 493 .md files: Documentation links"
    echo "• 5 .txt files: Path references"
    echo "• 5 .html files: Link references"
}

show_directory_moves() {
    print_section "Directory Move Summary"
    echo -e "${GREEN}MOVED TO src/:${NC}"
    echo "• AppHost/ → src/AppHost/"
    echo "• src/ → src/src/"
    echo "• src/ → src/src/"
    echo "• ServiceDefaults/ → src/ServiceDefaults/"
    echo "• IdsConnectAdapter/ → src/IdsConnectAdapter/"
    echo "• erp-connector/ → src/erp-connector/"
    echo
    echo -e "${GREEN}MOVED TO tests/:${NC}"
    echo "• tests/tests/tests/AppHost.Tests/ → tests/tests/tests/tests/AppHost.Tests/"
    echo
    echo -e "${GREEN}MOVED TO docs/project/:${NC}"
    echo "• *.md (root level) → docs/project/"
    echo
    echo -e "${GREEN}MOVED TO data/:${NC}"
    echo "• mock-db*.json → data/"
    echo "• test-data/ → data/test-data/"
}

main() {
    print_header

    case "${1:-help}" in
        "csharp"|"cs")
            show_csharp_mappings
            ;;
        "code"|"references")
            show_code_mappings
            ;;
        "typescript"|"ts"|"js")
            show_typescript_mappings
            ;;
        "markdown"|"md"|"docs")
            show_markdown_mappings
            ;;
        "config"|"json"|"yml")
            show_config_mappings
            ;;
        "scripts"|"sh")
            show_script_mappings
            ;;
        "regex"|"patterns")
            show_regex_patterns
            ;;
        "validation"|"checklist")
            show_validation_checklist
            ;;
        "counts"|"files")
            show_file_counts
            ;;
        "moves"|"directories")
            show_directory_moves
            ;;
        "all")
            show_directory_moves
            echo
            show_file_counts
            echo
            show_csharp_mappings
            show_code_mappings
            show_typescript_mappings
            show_markdown_mappings
            show_config_mappings
            show_script_mappings
            echo
            show_regex_patterns
            echo
            show_validation_checklist
            ;;
        "help"|*)
            echo "B2X Path Mapping Quick Reference"
            echo
            echo "Usage: $0 [section]"
            echo
            echo "Sections:"
            echo "  csharp     - .csproj project references"
            echo "  code       - C# code references"
            echo "  typescript - TypeScript/JavaScript imports"
            echo "  markdown   - Documentation links"
            echo "  config     - Configuration file paths"
            echo "  scripts    - Build script paths"
            echo "  regex      - Bulk replacement patterns"
            echo "  validation - Post-update checklist"
            echo "  counts     - File counts by type"
            echo "  moves      - Directory move summary"
            echo "  all        - Show everything"
            echo "  help       - This help message"
            echo
            echo "Examples:"
            echo "  $0 csharp     # Show .csproj mappings"
            echo "  $0 regex      # Show regex patterns"
            echo "  $0 all        # Show complete reference"
            ;;
    esac
}

main "$@"