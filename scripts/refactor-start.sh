#!/bin/bash
# refactor-start.sh
# Quick-start script for B2X refactoring using subAgent optimization

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  B2X Project Refactoring - SubAgent${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_step() {
    echo -e "${GREEN}[STEP]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to run subAgent with proper formatting
run_subagent() {
    local description="$1"
    local prompt="$2"

    echo
    print_step "Running subAgent: $description"
    echo "Prompt preview: ${prompt:0:100}..."

    # Note: In actual execution, replace this with actual runSubagent call
    echo "runSubagent description:\"$description\""
    echo "prompt:\"$prompt\""
    echo
}

# Pre-flight checks
check_prerequisites() {
    print_step "Checking prerequisites..."

    # Check git status
    if [ -n "$(git status --porcelain)" ]; then
        print_error "Git working directory is not clean. Please commit or stash changes."
        exit 1
    fi

    # Check current branch
    CURRENT_BRANCH=$(git branch --show-current)
    if [ "$CURRENT_BRANCH" != "refactor/working" ]; then
        print_warning "Not on refactor/working branch. Current: $CURRENT_BRANCH"
        echo "Switch to refactor/working branch? (y/n)"
        read -r response
        if [ "$response" = "y" ]; then
            git checkout refactor/working
        else
            exit 1
        fi
    fi

    # Check backup branch exists
    if ! git show-ref --verify --quiet refs/heads/refactor/backup; then
        print_error "Backup branch refactor/backup does not exist."
        exit 1
    fi

    print_step "Prerequisites check passed ✓"
}

# Phase 0: Pre-flight validation
phase_0_preflight() {
    print_header
    echo "Phase 0: Pre-Flight Checks"
    echo

    run_subagent "Pre-flight validation and baseline" "
Execute comprehensive pre-flight checks:
1. Git status validation (clean working directory)
2. Branch verification (on refactor/working)
3. File permission checks for all target directories
4. Build baseline capture (store dotnet build output)
5. Dependency mapping for critical paths
6. MCP server connectivity validation
7. Disk space verification for operations

Return ONLY: readiness_score + blocking_issues + baseline_build_log + mcp_status
"
}

# Phase 1: Directory creation
phase_1_structure() {
    echo
    print_header
    echo "Phase 1: Directory Structure Creation"
    echo

    run_subagent "Create new directory structure" "
Create new directory structure safely:
1. Create: src/, docs/, tests/, build/, config/, data/, archive/
2. Set appropriate permissions (755 for dirs, 644 for files)
3. Validate directory creation
4. Check for naming conflicts
5. Create .gitkeep files in empty directories

Return ONLY: created_directories + permission_status + conflicts_detected
"
}

# Phase 2: Low-risk moves
phase_2_moves() {
    echo
    print_header
    echo "Phase 2: Low-Risk File Moves"
    echo

    run_subagent "Phase 1 file migration (low-risk)" "
Execute Phase 1 file moves in batches:
1. Move data files: mock-db*.json, test-data/ → data/
2. Move docs: *.md files (except root configs) → docs/project/
3. Move configs: *.json, *.yml (except root) → config/
4. Validate each move immediately
5. Update .gitignore if needed
6. Store operation logs in temp files

Return ONLY: moved_files_count + validation_results + temp_log_path + gitignore_updated
"
}

# Quick start menu
show_menu() {
    echo
    print_header
    echo "B2X Refactoring Quick Start"
    echo
    echo "Choose starting point:"
    echo "1) Full pre-flight check + directory creation"
    echo "2) Directory creation only"
    echo "3) Low-risk file moves only"
    echo "4) Show available phases"
    echo "5) Exit"
    echo
    read -r choice

    case $choice in
        1)
            check_prerequisites
            phase_0_preflight
            phase_1_structure
            ;;
        2)
            check_prerequisites
            phase_1_structure
            ;;
        3)
            check_prerequisites
            phase_2_moves
            ;;
        4)
            echo
            echo "Available phases:"
            echo "0) Pre-flight checks"
            echo "1) Directory structure"
            echo "2) Low-risk moves (data/docs/config)"
            echo "3) Source code migration (Backend/Frontend/AppHost)"
            echo "4) Reference updates (C#, TypeScript, docs, configs)"
            echo "5) Build validation"
            echo "6) Final validation & documentation"
            echo
            echo "Use individual scripts for phases 3+ due to complexity"
            ;;
        5)
            exit 0
            ;;
        *)
            print_error "Invalid choice"
            show_menu
            ;;
    esac
}

# Main execution
main() {
    if [ $# -eq 0 ]; then
        show_menu
    else
        case $1 in
            "preflight")
                check_prerequisites
                phase_0_preflight
                ;;
            "structure")
                check_prerequisites
                phase_1_structure
                ;;
            "moves")
                check_prerequisites
                phase_2_moves
                ;;
            "help"|*)
                echo "Usage: $0 [phase]"
                echo
                echo "Phases:"
                echo "  preflight    - Pre-flight checks"
                echo "  structure    - Directory creation"
                echo "  moves        - Low-risk file moves"
                echo "  (no arg)     - Interactive menu"
                echo
                echo "For complex phases (3+), use individual scripts"
                ;;
        esac
    fi
}

main "$@"